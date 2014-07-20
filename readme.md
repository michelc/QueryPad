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
  , "Name": "@Sdf.Department"
  , "Provider": "System.Data.SqlServerCe.4.0"
  },
  {
    "CnxString": "Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=C:\\DB\\Department_Express.mdf;User Instance=true;Database=Department_Express"
  , "Environment": "Test"
  , "Name": "@Mdf.Department"
  , "Provider": "System.Data.SqlClient"
  },
  {
    "CnxString": "Server=xxxxx.sqlserver.sequelizer.com;Database=yyyyy;User ID=zzzzz;Password=xyz"
  , "Environment": "Release"
  , "Name": "@AppHarbor.Department"
  , "Provider": "System.Data.SqlClient"
  }
]
```
