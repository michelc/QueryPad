using System.Collections.Generic;
using System.Linq;

namespace QueryPad
{
    public class CnxParameter
    {
        public string Name { get; set; }
        public string Environment { get; set; }
        public string Provider { get; set; }
        public string CnxString { get; set; }

        public static List<CnxParameter> GetList()
        {
            var list = new List<CnxParameter>();

            list.Add(new CnxParameter
            {
                Name = "@Sdf.Department",
                Environment = "Debug",
                Provider = "System.Data.SqlServerCe.4.0",
                CnxString = @"Data Source=C:\MVC\Pif\Pif\App_Data\Department.sdf"
            });

            list.Add(new CnxParameter
            {
                Name = "@Sdf.Production",
                Environment = "Release",
                Provider = "System.Data.SqlServerCe.4.0",
                CnxString = @"Data Source=C:\MVC\Pif\Pif\App_Data\Department.sdf"
            });

            list.Add(new CnxParameter
            {
                Name = "@Sdf.Tests",
                Environment = "Test",
                Provider = "System.Data.SqlServerCe.4.0",
                CnxString = @"Data Source=C:\MVC\Pif\Pif\App_Data\Department.sdf"
            });

            return list.OrderBy(c => c.Name).ThenBy(c => c.Environment).ToList();
        }
    }
}
