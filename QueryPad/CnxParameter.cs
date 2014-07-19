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
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Environment { get; set; }

        [DataMember]
        public string Provider { get; set; }

        [Browsable(false)]
        [DataMember]
        public string CnxString { get; set; }

        [Browsable(false)]
        [DataMember]
        public string LastUse { get; set; }

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
            // Sort connections parameters by recent use
            CnxParameters = CnxParameters.OrderByDescending(c => c.LastUse)
                                         .ThenBy(c => c.Name)
                                         .ToList();

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
