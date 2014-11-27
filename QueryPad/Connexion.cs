﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altrr;
using Dapper;

namespace QueryPad
{
    public class Connexion
    {
        public CnxParameter CnxParameter { get; set; }

        private DbProviderFactory factory { get; set; }
        private DbConnection db { get; set; }
        private DbDataAdapter da { get; set; }
        private DbCommand dc { get; set; }
        private DbTransaction transaction { get; set; }
        private string[] cache_tables;
        private Dictionary<string, List<Column>> cache_columns;

        public Connexion(CnxParameter CnxParameter)
        {
            this.CnxParameter = CnxParameter;
        }

        public void Open()
        {
            // Open the connexion
            factory = DbProviderFactories.GetFactory(CnxParameter.Provider);
            db = factory.CreateConnection();
            db.ConnectionString = CnxParameter.CnxString;
            db.Open();
            da = factory.CreateDataAdapter();
            dc = db.CreateCommand();
            transaction = db.BeginTransaction();
        }

        public void Close()
        {
            // Close current connexion
            transaction.Commit();
            db.Close();
        }

        public string[] GetTables()
        {
            // Return cached list
            if (cache_tables != null) return cache_tables;

            // Get all tables for current connection
            if (CnxParameter.Provider == "System.Data.Odbc")
            {
                var dt = db.GetSchema("Tables");
                cache_tables = dt.Rows.Cast<DataRow>()
                    .Where(r => r["TABLE_TYPE"].ToString() == "TABLE")
                    .Select(r => r["TABLE_NAME"].ToString()).ToArray();
            }
            else
            {
                cache_tables = db.Query<string>(this.SqlTables(), transaction: transaction).ToArray();
            }

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

            // Need a specific connexion
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
            var restrictions = new[] { null, null, table, null };
            if (CnxParameter.Provider == "Oracle.DataAccess.Client") restrictions = new[] { null, table.ToUpper(), null };
            dt = db.GetSchema("Columns", restrictions);
            var ordinal = -1;
            if (CnxParameter.Provider == "System.Data.SQLite") ordinal = 0;
            for (var x = 0; x < count; x++)
            {
                var row = dt.Rows[x];
                var i = (CnxParameter.Provider == "Oracle.DataAccess.Client")
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
                            AND    (Table_Name <> '__MigrationHistory')
                            AND    (Table_Catalog = db_name())
                            ORDER BY CASE WHEN Table_Schema = 'dbo' THEN '' ELSE Table_Schema + '.' END, Table_Name";
                    break;
                case "System.Data.SqlServerCe.4.0":
                    sql = @"SELECT Table_Name AS [Table]
                            FROM   Information_Schema.Tables
                            WHERE  (Table_Type = 'TABLE')
                            AND    (Table_Name <> '__MigrationHistory')
                            ORDER BY Table_Name";
                    break;
                case "System.Data.SQLite":
                    sql = @"SELECT name AS [Table]
                            FROM   sqlite_master
                            WHERE  (type = 'table')
                            AND    (name NOT LIKE 'sqlite_%')
                            ORDER BY name";
                    break;
                case "Oracle.DataAccess.Client":
                    sql = @"SELECT INITCAP(Table_Name)
                            FROM   User_Tables
                            ORDER BY 1";
                    break;
                case "Npgsql":
                    sql = @"SELECT Table_Name AS Table
                            FROM   Information_Schema.Tables
                            WHERE  (Table_Type = 'BASE TABLE')
                            AND    (Table_Name <> '__MigrationHistory')
                            AND    (Table_Schema = 'public')
                            ORDER BY Table_Name";
                    break;
            }

            return sql;
        }

        public DataTableResult ExecuteDataTable(string sql)
        {
            var result = new DataTableResult();

            try
            {
                dc.CommandText = sql;
                dc.Transaction = transaction;
                da.SelectCommand = dc;
                var start = DateTime.Now;
                if (CnxParameter.Provider == "Oracle.DataAccess.Client")
                {
                    for (int i = 0; i < 500; i += 100)
                    {
                        da.Fill(i, 100, result.DataTable);
                        result.RowCount = result.DataTable.Rows.Count;
                        result.IsSlow = DateTime.Now.Subtract(start).TotalSeconds > 1;
                        result.IsFull = (result.RowCount < i + 100);
                        if (result.IsFull) break;
                        if (result.IsSlow) break;
                    }
                }
                else
                {
                    da.Fill(result.DataTable);
                    result.RowCount = result.DataTable.Rows.Count;
                    result.IsSlow = DateTime.Now.Subtract(start).TotalSeconds > 1;
                    result.IsFull = true;
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

        public async Task<int> ExecuteNonQueryAsync(string sql, CancellationToken token)
        {
            // await Task.Delay(1500, token);
            var supper = sql.ToUpper();
            if (supper == "COMMIT")
            {
                transaction.Commit();
                transaction = db.BeginTransaction();
                return 0;
            }
            if (supper == "ROLLBACK")
            {
                transaction.Rollback();
                transaction = db.BeginTransaction();
                return 0;
            }
            int count = -1;
            var command = db.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            try
            {
                var t = command.ExecuteNonQueryAsync(token);
                await t;
                count = t.Result;
            }
            catch { throw; }

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
        public int SortIndex { get; set; }
        public ListSortDirection SortDirection { get; set; }

        public DataTableResult() { this.Clear(); }

        public void Clear()
        {
            this.DataTable = new DataTable();
            this.IsSlow = false;
            this.IsFull = false;
            this.RowCount = 0;
            this.RowIndex = -1;
        }
    }
}
