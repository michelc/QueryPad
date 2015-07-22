# QueryPad

Quick & dirty query tool with just what I need (maybe less).


## TODO

* Stop with non query SQL (what about transaction ?)
* Use schema informations to find foreign keys
* Use Dapper NuGet ?
* Etc...


## IDEAS

* Modern UI (so WPF ?)


## Configuration

Connections are defined in the file "App.Connections.secret". It's a JSON file
with the following informations:

```JSON
[
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
    "CnxString": "Data Source=C:\\DB\\Department.sdf"
  , "Environment": "Debug"
  , "Name": "Department.SDF"
  , "Provider": "System.Data.SqlServerCe.4.0"
  },
  {
    "CnxString": "Server=xxxxx.amazonaws.com;Database=yyyyy;User Id=zzzzz;Password=xyz;Port=5432;Ssl=true"
  , "Environment": "Release"
  , "Name": "Department.Heroku"
  , "Provider": "Npgsql"
  },
  {
    "CnxString": "Data Source=C:\\DB\\Department.db"
  , "Environment": "Test"
  , "Name": "Department.DB"
  , "Provider": "System.Data.SQLite"
  },
  {
    "CnxString": "Data Source=DEPARTMENT;User ID=zzzzz;Password=xyz"
  , "Environment": "Release"
  , "Name": "Department.Oracle"
  , "Provider": "Oracle.DataAccess.Client"
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
* Oracle.DataAccess.Client (and System.Data.OracleClient)
* System.Data.Odbc


## Usage

Connection tab
* double click => open selected connection
* file drop => open file (if extension in "mdb", "mdf", sdf", "db", "db3", "sqlite")
* delete key => remove connection from list

Connection title
* double click => refresh table list

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


## Format command

After a SELECT, FORMAT is a pseudo-command to rearrange informations from the
data grid.

Syntax: `FORMAT { LIST | GRID | TEXT | specific format }`

SELECT query
```
SELECT * FROM Categories
=>
Id  Caption      Description                          Created     Total
--  -----------  -----------------------------------  ----------  -----
1   Confections  Desserts, candies, and sweet breads  04/08/2014  10
2   Produce      Dried fruit and bean curd            04/17/2014  12
3   Seafood      Seaweed and fish                     05/23/2014  5
```

LIST syntax
```
FORMAT LIST
=>
Id  Caption      Description                          Created     Total
--  -----------  -----------------------------------  ----------  -----
 1  Confections  Desserts, candies, and sweet breads  04/08/2014     10
 2  Produce      Dried fruit and bean curd            04/17/2014     12
 3  Seafood      Seaweed and fish                     05/23/2014      5
```

GRID syntax
```
FORMAT GRID
=>
| Id | Caption     | Description                         | Created    | Total |
|----|-------------|-------------------------------------|------------|-------|
|  1 | Confections | Desserts, candies, and sweet breads | 04/08/2014 |    10 |
|  2 | Produce     | Dried fruit and bean curd           | 04/17/2014 |    12 |
|  3 | Seafood     | Seaweed and fish                    | 05/23/2014 |     5 |
```

TEXT syntax
```
FORMAT TEXT
=>
1→"Confections"→"Desserts, candies, and sweet breads"→04/08/2014→10
2→"Produce"→"Dried fruit and bean curd"→04/17/2014→12
3→"Seafood"→"Seaweed and fish"→05/23/2014→5
```

FORMAT example 1
```
FORMAT {0};"{Caption}";{created:yyyy-MM-dd};{total}
=>
1;"Confections";2014-04-08;10
2;"Produce";2014-04-17;12
3;"Seafood";2014-05-23;5
```

FORMAT example 2
```
FORMAT UPDATE Reports SET Caption = '{1}', Total = {4} WHERE (Id = {ID});
=>
UPDATE Reports SET Caption = 'Confections', Total = 10 WHERE (Id = 1);
UPDATE Reports SET Caption = 'Produce', Total = 12 WHERE (Id = 2);
UPDATE Reports SET Caption = 'Seafood', Total = 5 WHERE (Id = 3);
```


## Credit

* FastColoredTextBox (https://github.com/PavelTorgashov/FastColoredTextBox)
