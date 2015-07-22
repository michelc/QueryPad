using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Altrr
{
    [DataContract]
    public class CnxParameter
    {
        public static string[] DropExtensions = { "mdb", "mdf", "sdf", "db", "db3", "sqlite" };

        [DataMember]
        public string Environment { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Provider { get; set; }

        [Browsable(false)]
        [DataMember]
        public string CnxString { get; set; }

        [Browsable(false)]
        [DataMember]
        public string LastUse { get; set; }

        public CnxParameter(string file)
        {
            var extension = file.Substring(1 + file.LastIndexOf(".")).ToLower();
            if (!CnxParameter.DropExtensions.Contains(extension)) return;

            this.Name = file.Substring(1 + file.LastIndexOf("\\"));
            var dbname = this.Name.Substring(0, this.Name.LastIndexOf("."));
            this.Environment = "Release";

            switch (extension)
            {
                case "mdb":
                    this.CnxString = string.Format("Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};DBQ={0};ExtendedAnsiSQL=1", file);
                    this.Provider = "System.Data.Odbc";
                    break;
                case "mdf":
                    this.CnxString = string.Format("Data Source=.\\SQLEXPRESS;Integrated Security=SSPI;User Instance=true;AttachDBFilename={0};Database={1}", file, dbname);
                    this.Provider = "System.Data.SqlClient";
                    break;
                case "sdf":
                    this.CnxString = string.Format("Data Source={0}", file);
                    this.Provider = "System.Data.SqlServerCe.4.0";
                    break;
                default:
                    this.CnxString = string.Format("Data Source={0};Version=3", file);
                    this.Provider = "System.Data.SQLite";
                    break;
            }
        }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(CnxString)) return CnxString;

                // Hide password in connection string
                var safe = Regex.Replace(CnxString, "(password|pwd)=(.*?);", "Password=*****;", RegexOptions.IgnoreCase);
                if (safe == CnxString)
                {
                    safe = Regex.Replace(CnxString, "(password|pwd)=(.*?)$", "Password=*****", RegexOptions.IgnoreCase);
                }

                return safe;
            }
        }

        [Browsable(false)]
        public bool IsOracle
        {
            get
            {
                return Provider.ToLower().Contains("oracle");
            }
        }

        [Browsable(false)]
        public bool NoTransaction
        {
            get
            {
                return CnxString.ToLower().Contains("transaction=no");
            }
        }

        public static List<CnxParameter> Load()
        {
            // Load connections parameters from a JSON file
            using (var stream = File.OpenRead(App.ConnectionsFile))
            {
                var serializer = new DataContractJsonSerializer(typeof(List<CnxParameter>));
                return (serializer.ReadObject(stream) as List<CnxParameter>)
                        .OrderByDescending(c => c.LastUse)
                        .ThenBy(c => c.Name)
                        .ToList();
            }
        }

        public static void Save(List<CnxParameter> CnxParameters)
        {
            // Sort connections parameters by name
            CnxParameters = CnxParameters.OrderBy(c => c.Name).ToList();

            // Save connections parameters as a JSON file
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(List<CnxParameter>));
                serializer.WriteObject(stream, CnxParameters);
                string json = Encoding.UTF8.GetString(stream.ToArray());
                // Prettify JSON
                json = json.Replace("{\"", "\n  {\n    \"");
                json = json.Replace("\"}", "\"\n  }");
                json = json.Replace("\":\"", "\": \"");
                json = json.Replace("\",\"", "\"\n  , \"");
                json = json.Replace("}]", "}\n]\n");
                json = json.Replace("\"LastUse\":null,", "");
                File.WriteAllText(App.ConnectionsFile, json);
            }
        }
    }
}
