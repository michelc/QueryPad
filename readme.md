# QueryPad

Quick & dirty query tool with just what I need (maybe less).


## TODO

* Use schema informations to find foreign keys
* Use Dapper NuGet
* Display results as Grid or Text (maybe)
* Etc...


## Configuration

Connections are defined in the file "App.Connections.secret". It's a JSON file
with the following informations:

```
[
  {
    "CnxString": "Data Source=C:\\DB\\Department.sdf"
  , "Environment": "Debug"
  , "Name": "@SqlServer.CompactEdition"
  , "Provider": "System.Data.SqlServerCe.4.0"
  },
  {
    "CnxString": "Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=C:\\DB\\Department_Express.mdf;User Instance=true;Database=Department_Express"
  , "Environment": "Test"
  , "Name": "@SqlServer.Express"
  , "Provider": "System.Data.SqlClient"
  },
  {
    "CnxString": "Server=xxxxx.sqlserver.sequelizer.com;Database=yyyyy;User ID=zzzzz;Password=xyz"
  , "Environment": "Release"
  , "Name": "@SqlServer.AppHarbor"
  , "Provider": "System.Data.SqlClient"
  },
  {
    "CnxString": "Data Source=C:\\DB\\Department.db"
  , "Environment": "Test"
  , "Name": "@SQLite"
  , "Provider": "System.Data.SQLite"
  },
  {
    "CnxString": "Server=xxxxx.amazonaws.com;Database=yyyyy;User Id=zzzzz;Password=xyz;Port=5432;Ssl=true"
  , "Environment": "Release"
  , "Name": "@PostgreSQL.Heroku"
  , "Provider": "Npgsql"
  },
  {
    "CnxString": "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\DB\\Department.mdb"
  , "Environment": "Debug"
  , "Name": "@MsAccess.MDB"
  , "Provider": "System.Data.OleDb"
  }
]
```
