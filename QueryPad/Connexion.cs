using System;
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

        private DbConnection db { get; set; }
        private DbDataAdapter da { get; set; }
        private DbCommand dc { get; set; }
        private DbTransaction transaction { get; set; }
        private string[] tables;

        public Connexion(CnxParameter CnxParameter)
        {
            this.CnxParameter = CnxParameter;
        }

        public void Open()
        {
            // Open the connexion
            var factory = DbProviderFactories.GetFactory(CnxParameter.Provider);
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
            if (tables != null) return tables;

            // Get all tables for current connection
            if (CnxParameter.Provider == "System.Data.OleDb")
            {
                var dt = db.GetSchema("Tables", new string[] {null, null, null, "TABLE"});
                tables = dt.Rows.Cast<DataRow>().Select(r => r["TABLE_NAME"].ToString()).ToArray();
            }
            else
            {
                tables = db.Query<string>(this.SqlTables(), transaction: transaction).ToArray();
            }

            // Return list
            return tables;
        }

        public List<Column> GetColumns(string table)
        {
            // Get all columns for table
            var cols = db.Query<dynamic>(this.SqlColumns(table), transaction: transaction).ToArray();

            // Get usefull columns informations
            var columns = new List<Column>();
            bool first = true;
            foreach (var col in cols)
            {
                // Check Nullable property
                var _nullable = false;
                if (!string.IsNullOrEmpty(col.is_nullable))
                {
                    bool.TryParse(col.is_nullable.Replace("YES", "True").Replace("Y", "True"), out _nullable);
                }
                else
                {
                    _nullable = (col.notnull == 0);
                }
                // Check Size and Scale properties
                var _size = col.size == null ? 0 : (int)col.size;
                var _scale = col.scale == null ? -1 : (int)col.scale;
                var _type = ((string)col.type).ToLower();
                if ((_type == "numeric") || (_type == "number"))
                {
                    if ((_type == "number") && (_scale == 0))
                    {
                        var _length = col.length == null ? 0 : (int)col.length;
                        if (_length == 22) _type = "integer";
                        _scale = 0;
                    }
                    if (_type != "integer")
                    {
                        if (col.precision != null)
                        {
                            _size = (int)col.precision;
                        }
                        else
                        {
                            _scale = 0;
                        }
                    }
                }
                else if (_type.Contains("char"))
                {
                    _scale = 0;
                }
                else
                {
                    _size = 0;
                }
                // Update Type property
                if (_size > 0)
                {
                    _type += "(" + _size.ToString();
                    if (_scale > 0) _type += "," + _scale.ToString();
                    _type += ")";
                }
                // Check Default property
                var _default = col.dflt_value == null ? "" : col.dflt_value.Trim();
                while ((_default.StartsWith("(")) && (_default.EndsWith(")")))
                {
                    _default = _default.Substring(1, _default.Length - 2);
                }
                // Check Autoincrement
                if (first)
                {
                    first = false;
                    if ((col.pk != null) && (col.pk == 1))
                    {
                        // SQLite
                        if (_type.ToLower() == "integer") _type = "int identity(1,1)";
                    }
                    else if (_type == "serial")
                    {
                        // PostgreSQL
                        _type = "int identity(1,1)";
                    }
                    else if ((_type == "int") && (col.auto != null))
                    {
                        if (col.auto is int)
                        {
                            // SQL Server
                            if (col.auto > 0) _type = "int identity(1,1)";
                        }
                        else if (col.auto is long)
                        {
                            // SQL Server Compact
                            if (col.auto > 0) _type = "int identity(1,1)";
                        }
                    }
                }
                // Define table's column
                var column = new Column
                {
                    Index = 1 + (int)col.cid,
                    Name = col.name,
                    Nullable = _nullable,
                    Type = _type,
                    Default = _default
                };
                columns.Add(column);
            }

            // Return table's columns
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

        public string SqlColumns(string table)
        {
            // Query to get all columns for a table
            // depending on provider

            var sql = "";
            switch (CnxParameter.Provider)
            {
                case "System.Data.SqlClient":
                    sql = @"SELECT Ordinal_Position - 1 AS [cid]
                                 , Column_Name AS [name]
                                 , Data_Type AS [type]
                                 , Character_Maximum_Length AS [size]
                                 , Numeric_Precision AS [precision]
                                 , Numeric_Scale AS [scale]
                                 , COLUMNPROPERTY(object_id(Table_Schema + '.' + Table_Name), Column_Name, 'IsIdentity') AS [auto]
                                 , Column_Default AS [dflt_value]
                                 , Is_Nullable AS [is_nullable]
                            FROM   Information_Schema.Columns
                            WHERE  (Table_Name LIKE '{0}')
                            ORDER BY Ordinal_Position";
                    break;
                case "System.Data.SqlServerCe.4.0":
                    sql = @"SELECT Ordinal_Position - 1 AS [cid]
                                 , Column_Name AS [name]
                                 , Data_Type AS [type]
                                 , Character_Maximum_Length AS [size]
                                 , Numeric_Precision AS [precision]
                                 , Numeric_Scale AS [scale]
                                 , Autoinc_Increment AS [auto]
                                 , Column_Default AS [dflt_value]
                                 , Is_Nullable AS [is_nullable]
                            FROM   Information_Schema.Columns
                            WHERE  (Table_Name LIKE '{0}')
                            ORDER BY Ordinal_Position";
                    break;
                case "System.Data.SQLite":
                    sql = @"PRAGMA table_info('{0}')";
                    break;
                case "Oracle.DataAccess.Client":
                    table = table.ToUpper();
                    sql = @"SELECT Column_ID - 1 AS [cid]
                                 , Column_Name AS [name]
                                 , Data_Type AS [type]
                                 , Char_Col_Decl_Length AS [size]
                                 , Data_Length AS [length]
                                 , Data_Precision AS [precision]
                                 , Data_Scale AS [scale]
                                 , Data_Default AS [dflt_value]
                                 , Nullable AS [is_nullable]
                            FROM   Cols
                            WHERE  (Table_Name LIKE '{0}')
                            ORDER BY Column_ID";
                    break;
                case "Npgsql":
                    table = table.ToLower();
                    sql = @"SELECT Ordinal_Position - 1 AS [cid]
                                 , Column_Name AS [name]
                                 , Data_Type AS [type]
                                 , Character_Maximum_Length AS [size]
                                 , Numeric_Precision AS [precision]
                                 , Numeric_Scale AS [scale]
                                 , (Column_Default LIKE 'nextval(''' || Table_Name || '_' || Column_Name || '_seq''%') AS [auto]
                                 , Column_Default AS [dflt_value]
                                 , Is_Nullable AS [is_nullable]
                            FROM   Information_Schema.Columns
                            WHERE  (Table_Name LIKE '{0}')
                            ORDER BY Ordinal_Position";
                    break;
            }

            sql = sql.Replace("[", "\"").Replace("]", "\"");
            return string.Format(sql, table);
        }

        public DataTable ExecuteDataTable(string sql)
        {
            var dt = new DataTable();

            try
            {
                dc.CommandText = sql;
                dc.Transaction = transaction;
                da.SelectCommand = dc;
                if (CnxParameter.Provider == "Oracle.DataAccess.Client")
                {
                    var start = DateTime.Now;
                    for (int i = 0; i < 500; i += 100)
                    {
                        da.Fill(i, 100, dt);
                        if (dt.Rows.Count < i + 100) break;
                        if (DateTime.Now.Subtract(start).TotalSeconds > 1) break;
                    }
                }
                else
                {
                    da.Fill(dt);
                }
            }
            catch { throw; }

            return dt;
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
    }
}
