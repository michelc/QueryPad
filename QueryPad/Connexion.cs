﻿using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;

namespace QueryPad
{
    public class Connexion
    {
        public CnxParameter CnxParameter { get; set; }

        private DbConnection db { get; set; }
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
        }

        public void Close()
        {
            // Close current connexion
            db.Close();
        }

        public string[] GetTables()
        {
            // Return cached list
            if (tables != null) return tables;

            // Get all tables for current connection
            tables = db.Query<string>(this.SqlTables()).ToArray();

            // Return list
            return tables;
        }

        public string SqlTables()
        {
            // Query to get all tables for a connection
            // depending on provider

            var sql = "";
            switch (CnxParameter.Provider)
            {
                case "System.Data.SqlClient":
                    sql = @"SELECT Table_Name AS [Table]
                            FROM   Information_Schema.Tables
                            WHERE  (Table_Type = 'BASE TABLE')
                            AND    (Table_Name <> '__MigrationHistory')
                            AND    (Table_Catalog = db_name())
                            ORDER BY Table_Name";
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
                    if (!sql.StartsWith("TOP")) sql = "TOP 10000 " + sql;
                    sql = "SELECT " + sql;
                    break;
                case "System.Data.SQLite":
                    if (!sql.ToUpper().Contains("LIMIT"))
                    {
                        var limit = false;
                        var end = sql.LastIndexOf(" ");
                        if (end != -1)
                        {
                            var start = sql.LastIndexOf(" ", end - 1);
                            limit = (sql.Substring(start, end - start).Trim().ToUpper() == "LIMIT");
                        }
                        if (!limit) sql += " LIMIT 10000";
                    }
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

            da.SelectCommand = command;
            try
            {
                da.Fill(ds);
            }
            catch { throw; }

            return ds;
        }

        public int ExecuteNonQuery(string sql)
        {
            int count = -1;
            var command = db.CreateCommand();
            command.CommandText = sql;
            try
            {
                count = command.ExecuteNonQuery();
            }
            catch { throw; }

            return count;
        }
    }
}
