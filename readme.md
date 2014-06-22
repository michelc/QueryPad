# QueryPad

Quick & dirty query tool with just what I need (maybe less).


## TODO

* Test with Sql Server
* Use Dapper NuGet
* Add OleDb support for Access (no?)
* Display results as Grid or Text (maybe)
* Handle SQL transactions (maybe)
* Etc...


## Configuration

Connections are defined in the file "QueryPad.secret". It's a JSON file with the
following informations:

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
