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

        private DbConnection db { get; set; }
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
            tables = db.Query<string>(this.SqlTables(), transaction: transaction).ToArray();

            // Return list
            return tables;
        }

        public List<Column> GetColumns(string table)
        {
            // Get schema columns
            var dt = db.GetSchema("Columns", new[] { null, null, table, null });
            var rows = dt.Rows.Cast<DataRow>().ToList();

            // Get columns informations
            var cols = new List<dynamic>();            
            foreach (var c in rows)
            {
                var col = new
                {
                    Index = Convert.ToInt32(c["Ordinal_Position"]),
                    Name = Convert.ToString(c["Column_Name"]),
                    Is_Nullable = Convert.ToString(c["Is_Nullable"]),
                    Type = Convert.ToString(c["Data_Type"]),
                    Length = Convert.ToString(c["Character_Maximum_Length"]),
                    Precision = Convert.ToString(c["Numeric_Precision"]),
                    Scale = Convert.ToString(c["Numeric_Scale"]),
                    Default = Convert.ToString(c["Column_Default"])
                };
                cols.Add(col);
            }

            // Get usefull columns informations
            var columns = new List<Column>();
            foreach (var col in cols.OrderBy(c => c.Index))
            {
                // Check Nullable property
                var _nullable = false;
                bool.TryParse(col.Is_Nullable.Replace("YES", "True"), out _nullable);
                // Check Size and Scale properties
                var _size = col.Length;
                var _scale = col.Scale;
                if (col.Type == "numeric")
                {
                    if (!string.IsNullOrEmpty(col.Precision))
                    {
                        _size = col.Precision;
                    }
                    else
                    {
                        _scale = "";
                    }
                }
                else if (col.Type == "nvarchar")
                {
                    _scale = "";
                }
                else
                {
                    _size = "";
                }
                // Update Type property
                if (!string.IsNullOrEmpty(_size))
                {
                    _size = "(" + _size;
                    if (!string.IsNullOrEmpty(_scale)) _size += "," + _scale;
                    _size += ")";
                }
                // Check Default property
                var _default = string.IsNullOrEmpty(col.Default) ? "" : col.Default.Trim();
                while ((_default.StartsWith("(")) && (_default.EndsWith(")")))
                {
                    _default = _default.Substring(1, _default.Length - 2);
                }
                // Define table's column
                var column = new Column
                {
                    Index = col.Index,
                    Name = col.Name,
                    Nullable = _nullable,
                    Type = col.Type + _size,
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
                case "System.Data.OracleClient":
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

        public string SelectTop(string sql)
        {
            // Limit select row count to 10000
            // depending on provider

            switch (CnxParameter.Provider)
            {
                case "System.Data.SqlClient":
                case "System.Data.SqlServerCe.4.0":
                    sql = sql.Substring(6).Trim();
                    if (!sql.ToUpper().StartsWith("TOP")) sql = "TOP 10000 " + sql;
                    sql = "SELECT " + sql;
                    break;
                case "System.Data.SQLite":
                case "Npgsql":
                    if (!sql.ToUpper().Contains("LIMIT")) sql += " LIMIT 10000";
                    break;
                case "System.Data.OracleClient":
                    if (!sql.ToUpper().Contains("ROWNUM")) sql = "SELECT * FROM (" + sql + ") WHERE ROWNUM <= 2500";
                    break;
            }

            return sql;
        }

        public DataSet ExecuteDataSet(string sql)
        {
            var ds = new DataSet();

            var factory = DbProviderFactories.GetFactory(CnxParameter.Provider);
            var da = factory.CreateDataAdapter();

            var command = db.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;

            da.SelectCommand = command;
            try
            {
                da.Fill(ds);
            }
            catch { throw; }

            return ds;
        }

        public async Task<DataTable> ExecuteDataTableAsync(string sql, CancellationToken token)
        {
            // await Task.Delay(1500, token);
            var dt = new DataTable();

            var command = db.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            try
            {
                var t = command.ExecuteReaderAsync(token);
                await t;
                dt.Load(t.Result);
                return dt;
            }
            catch { throw; }
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
        public int Index { get; set; }
        public string Name { get; set; }
        [DisplayName("Null?")]
        public bool Nullable { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
    }
}
