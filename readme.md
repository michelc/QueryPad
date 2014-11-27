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
  , "Name": "Department.SDF"
  , "Provider": "System.Data.SqlServerCe.4.0"
  },
  {
    "CnxString": "Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=C:\\DB\\Department_Express.mdf;User Instance=true;Database=Department_Express"
  , "Environment": "Test"
  , "Name": "Department.MDF"
  , "Provider": "System.Data.SqlClient"
  },
  {
    "CnxString": "Server=xxxxx.sqlserver.sequelizer.com;Database=yyyyy;User ID=zzzzz;Password=xyz"
  , "Environment": "Release"
  , "Name": "Department.AppHarbor"
  , "Provider": "System.Data.SqlClient"
  },
  {
    "CnxString": "Data Source=C:\\DB\\Department.db"
  , "Environment": "Test"
  , "Name": "Department.DB"
  , "Provider": "System.Data.SQLite"
  },
  {
    "CnxString": "Server=xxxxx.amazonaws.com;Database=yyyyy;User Id=zzzzz;Password=xyz;Port=5432;Ssl=true"
  , "Environment": "Release"
  , "Name": "Department.Heroku"
  , "Provider": "Npgsql"
  },
  {
    "CnxString": "Driver={Microsoft Access Driver (*.mdb, *.accdb)};DBQ=C:\\DB\\Department.mdb;ExtendedAnsiSQL=1"
  , "Environment": "Debug"
  , "Name": "Department.MDB"
  , "Provider": "System.Data.Odbc"
  }
]
```

Supported providers:
* System.Data.SqlClient
* System.Data.SqlServerCe.4.0
* Npgsql
* System.Data.SQLite
* Oracle.DataAccess.Client
* System.Data.Odbc


## Usage

Table list
* double click => SELECT * FROM table name
* control + double click => DESC table name

Grid cell
* click => select cell row
* double click => select cell
* control + double clic => SELECT * FROM table parent WHERE ID = cell value

Row detail (after a rotation)
* right arrow => next row
* left arrow => previous row
