using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace QueryPad
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

        [DataMember]
        public string CnxString { get; set; }

        [Browsable(false)]
        [DataMember]
        public string LastUse { get; set; }

        public static List<CnxParameter> Load()
        {
            // Load connections parameters from a JSON file
            using (var stream = File.OpenRead(App.ConfigFile))
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
                File.WriteAllText(App.ConfigFile, json);
            }
        }
    }
}
