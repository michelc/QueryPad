using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Altrr;
using Dapper;

namespace QueryPad
{
    public class Connection
    {
        public CnxParameter CnxParameter { get; set; }
        public bool UseTransaction { get; set; }

        private DbProviderFactory factory { get; set; }
        private DbConnection db { get; set; }
        private DbDataAdapter da { get; set; }
        private DbCommand dc { get; set; }
        private DbTransaction transaction { get; set; }
        private string[] cache_tables;
        private Dictionary<string, List<Column>> cache_columns;

        public Connection(CnxParameter CnxParameter)
        {
            this.CnxParameter = CnxParameter;
            this.UseTransaction = false;
        }

        public void Open()
        {
            // Open the connection
            factory = DbProviderFactories.GetFactory(CnxParameter.Provider);
            db = factory.CreateConnection();
            db.ConnectionString = Regex.Replace(CnxParameter.CnxString, "transaction=no", "", RegexOptions.IgnoreCase);
            db.Open();
            da = factory.CreateDataAdapter();
            dc = db.CreateCommand();
            transaction = null;
        }

        public void Close()
        {
            // Close current connection
            if (transaction != null) try { transaction.Commit(); } catch { }
            try { dc.Dispose(); } catch { }
            try { db.Close(); } catch { }
        }

        public string[] GetTables(bool use_cache)
        {
            // Return cached list
            if ((cache_tables != null) && use_cache) return cache_tables;

            // Get all tables for current connection
            var sql = this.SqlTables();
            if (string.IsNullOrEmpty(sql))
            {
                cache_tables = db.GetSchema("Tables")
                                 .Rows.Cast<DataRow>()
                                 .Where(r => r["TABLE_TYPE"].ToString().ToUpper() == "TABLE")
                                 .Select(r => r["TABLE_NAME"].ToString()).ToArray();
            }
            else
            {
                cache_tables = db.Query<string>(sql, transaction: transaction).ToArray();
            }
            cache_tables = cache_tables.Where(t => t.ToLower() != "__migrationhistory")
                                       .OrderBy(t => t)
                                       .ToArray();

            // Initialize columns cache
            cache_columns = new Dictionary<string, List<Column>>();

            // Return list
            return cache_tables;
        }

        public List<Column> GetColumns(string table)
        {
            // Return cached list
            if (cache_columns.ContainsKey(table.ToLower())) return cache_columns[table.ToLower()];
            var columns = new List<Column>();

            // Need a specific connection (outside a transaction)
            var db = factory.CreateConnection();
            db.ConnectionString = CnxParameter.CnxString;
            db.Open();

            // Get columns name and type
            var dc = db.CreateCommand();
            dc.CommandText = "SELECT * FROM " + table + " WHERE (1 = 0)";
            var dr = dc.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
            var count = dr.FieldCount;
            for (var i = 0; i < count; i++)
            {
                columns.Add(new Column
                {
                    Index = i,
                    Name = dr.GetName(i),
                    Type = dr.GetDataTypeName(i).ToLower()
                });
            }

            // Add nullable and primary key
            var dt = new DataTable();
            dt.Load(dr);
            foreach (DataColumn c in dt.Columns)
            {
                var i = c.Ordinal;
                if (c.AutoIncrement) columns[i].Type += " identity(1,1)";
                columns[i].Nullable = c.AllowDBNull;
                columns[i].Primary = dt.PrimaryKey.Contains(c);
            }
            dr.Close();

            // Add size and default
            var ti = CultureInfo.CurrentCulture.TextInfo;
            var restrictions = new[] { null, null, table, null };
            if (CnxParameter.IsOracle) restrictions = new[] { null, table.ToUpper(), null };
            dt = db.GetSchema("Columns", restrictions);
            var ordinal = -1;
            if (CnxParameter.Provider == "System.Data.SQLite") ordinal = 0;
            for (var x = 0; x < count; x++)
            {
                var row = dt.Rows[x];
                var i = (CnxParameter.IsOracle)
                        ? Convert.ToInt32(row["ID"]) - 1
                        : Convert.ToInt32(row["Ordinal_Position"]) + ordinal;
                // Get default value
                try
                {
                    columns[i].Default = Convert.ToString(row["Column_Default"]).Trim() + "::";
                    columns[i].Default = columns[i].Default.Substring(0, columns[i].Default.IndexOf("::"));
                    while ((columns[i].Default.StartsWith("(")) && (columns[i].Default.EndsWith(")")))
                    {
                        columns[i].Default = columns[i].Default.Substring(1, columns[i].Default.Length - 2);
                    }
                }
                catch { }
                // Complete type with size
                switch (CnxParameter.Provider)
                {
                    case "System.Data.SQLite":
                        // SQLite => GetDataTypeName = data type + size
                        if (Convert.ToString(row["Data_Type"]).ToLower() == "int identity") columns[i].Type = "int identity(1,1)";
                        if (columns[i].Type == "integer") columns[i].Type = "int identity(1,1)";
                        break;
                    case "Oracle.DataAccess.Client":
                    case "System.Data.OracleClient":
                        columns[i].Name = ti.ToTitleCase(columns[i].Name.ToLower());
                        if (columns[i].Name.EndsWith("_Id")) columns[i].Name = columns[i].Name.Substring(0, columns[i].Name.Length - 1) + "D";
                        columns[i].Type = row["DataType"].ToString().ToLower();
                        switch (columns[i].Type)
                        {
                            case "number":
                                columns[i].Type += string.Format("({0},{1})", row["Precision"], row["Scale"]);
                                columns[i].Type = columns[i].Type.Replace("number(,0)", "integer")
                                                                 .Replace("(,)", "");
                                break;
                            default:
                                if (columns[i].Type.Contains("char"))
                                {
                                    columns[i].Type += string.Format("({0})", row["Length"]);
                                }
                                break;
                        }
                        break;
                    case "System.Data.Odbc":
                        switch (columns[i].Type)
                        {
                            case "decimal":
                                columns[i].Type += string.Format("({0},{1})", row["Column_Size"], row["Decimal_Digits"]);
                                break;
                            default:
                                if (columns[i].Type.Contains("longchar"))
                                {
                                    columns[i].Type = "text";
                                }
                                else if (columns[i].Type.Contains("char"))
                                {
                                    columns[i].Type += string.Format("({0})", row["Column_Size"]);
                                }
                                break;
                        }
                        break;
                    default:
                        switch (columns[i].Type)
                        {
                            case "decimal":
                            case "number":
                            case "numeric":
                                columns[i].Type += string.Format("({0},{1})", row["Numeric_Precision"], row["Numeric_Scale"]);
                                break;
                            default:
                                if (columns[i].Type.Contains("char"))
                                {
                                    columns[i].Type += string.Format("({0})", row["Character_Maximum_Length"]);
                                }
                                break;
                        }
                        break;
                }
                columns[i].Type = columns[i].Type.Replace(",0)", ")");
            }
            db.Close();
            db = null;

            // Return table's columns
            cache_columns.Add(table.ToLower(), columns);
            return columns;
        }

        public string SqlTables()
        {
            // Query to get all tables for a connection
            // depending on provider

            var sql = "";
            switch (CnxParameter.Provider)
            {
                case "System.Data.SqlClient":
                    sql = @"SELECT CASE WHEN Table_Schema = 'dbo' THEN '' ELSE Table_Schema + '.' END
                                   + Table_Name AS [Table]
                            FROM   Information_Schema.Tables
                            WHERE  (Table_Type = 'BASE TABLE')
                            AND    (Table_Catalog = db_name())
                            ORDER BY CASE WHEN Table_Schema = 'dbo' THEN '' ELSE Table_Schema + '.' END, Table_Name";
                    break;
                case "Oracle.DataAccess.Client":
                case "System.Data.OracleClient":
                    sql = @"SELECT INITCAP(Table_Name)
                            FROM   User_Tables
                            ORDER BY 1";
                    break;
                case "Npgsql":
                    sql = @"SELECT Table_Name AS Table
                            FROM   Information_Schema.Tables
                            WHERE  (Table_Type = 'BASE TABLE')
                            AND    (Table_Schema = 'public')
                            ORDER BY Table_Name";
                    break;
            }

            return sql;
        }

        public DataTableResult ExecuteDataTable(string sql, CancellationToken token)
        {
            var result = new DataTableResult();
            var oracle = CnxParameter.IsOracle;

            try
            {
                // Read and load data asynchronously
                dc.CommandText = sql.Trim(";".ToCharArray());
                try { dc.CommandTimeout = 60; } catch { }
                if (transaction != null) dc.Transaction = transaction;
                da.SelectCommand = dc;
                var start = DateTime.Now;
                var page = oracle ? 100 : 1000;
                for (int i = 0; i < 100000; i += page)
                {
                    da.Fill(i, page, result.DataTable);
                    result.RowCount = result.DataTable.Rows.Count;
                    result.IsSlow = DateTime.Now.Subtract(start).TotalSeconds > 2.5;
                    result.IsFull = (result.RowCount < i + page);
                    if (result.IsFull) break;
                    if (result.IsSlow) break;
                    if (token.IsCancellationRequested) break;
                }

                // Set columns title
                var count = result.DataTable.Columns.Count;
                result.Titles = new string[count];
                var ti = CultureInfo.CurrentCulture.TextInfo;
                for (int i = 0; i < count; i++)
                {
                    result.Titles[i] = result.DataTable.Columns[i].Caption;
                    if (oracle)
                    {
                        result.Titles[i] = ti.ToTitleCase(result.Titles[i].ToLower());
                        if (result.Titles[i].EndsWith("_Id")) result.Titles[i] = result.Titles[i].Substring(0, result.Titles[i].Length - 1) + "D";
                    }
                }

                // Avoid sort error with comma in columns name
                for (int i = 0; i < count; i++)
                {
                    result.DataTable.Columns[i].ColumnName = result.DataTable.Columns[i].ColumnName.Replace(",", "_");
                }
            }
            catch { throw; }

            return result;

        }

        public DataTableResult ExecuteNextPage(DataTableResult result)
        {
            try
            {
                da.Fill(result.RowCount, 100, result.DataTable);
                result.IsFull = (result.DataTable.Rows.Count < result.RowCount + 100);
                result.RowCount = result.DataTable.Rows.Count;
            }
            catch { throw; }

            return result;
        }

        public int ExecuteNonQueries(string script)
        {
            // Get individual commands
            var commands = script.SplitCommands();
            // Or one big BEGIN ... END; command
            if (commands.First() + commands.Last() == "BEGINEND;") commands = new[] { script };

            // Execute each command
            var count = 0;
            foreach (var sql in commands)
            {
                ExecuteNonQuery(sql);
                count++;
            }

            return count;
        }

        public int ExecuteNonQuery(string sql)
        {
            var supper = sql.ToUpper();
            if (supper == "COMMIT")
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    transaction = null;
                }
                this.UseTransaction = false;
                return -10001;
            }
            if (supper == "ROLLBACK")
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                    transaction = null;
                }
                this.UseTransaction = false;
                return -10002;
            }
            if (transaction == null) transaction = CnxParameter.NoTransaction ? null : db.BeginTransaction();
            int count = -1;
            var command = db.CreateCommand();
            if (!sql.EndsWith("END;")) sql = sql.Trim(";".ToCharArray());
            command.CommandText = sql;
            try { command.CommandTimeout = 90; } catch { }
            if (transaction != null) command.Transaction = transaction;
            try
            {
                count = command.ExecuteNonQuery();
            }
            catch { throw; }

            if (transaction != null) this.UseTransaction = true;
            return count;
        }
    }

    public class Column
    {
        [DisplayName("#")]
        public int Index { get; set; }
        public string Name { get; set; }
        [DisplayName("Null?")]
        public bool Nullable { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public bool Primary { get; set; }
    }

    public class DataTableResult
    {
        public DataTable DataTable { get; set; }
        public bool IsSlow { get; set; }
        public bool IsFull { get; set; }
        public int RowCount { get; set; }
        public int RowIndex { get; set; }
        public DataGridState GridState { get; set; }
        public string[] Titles { get; set; }

        public DataTableResult() { this.Clear(); }

        public void Clear()
        {
            this.DataTable = new DataTable();
            this.IsSlow = false;
            this.IsFull = false;
            this.RowCount = 0;
            this.RowIndex = -1;
            this.GridState = new DataGridState();
            this.Titles = new string[0];
        }
    }

    public class DataGridState
    {
        public int CurrentRow { get; set; }
        public int SortedColumn { get; set; }
        public ListSortDirection SortDirection { get; set; }
        public int[] Widths { get; set; }
    }
}
