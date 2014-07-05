using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            int count = -1;
            var command = db.CreateCommand();
            command.CommandText = sql;
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
}
