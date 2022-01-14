using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser.Models
{
    public class SolutionData
    {
        public IDictionary<string, object> DataDictionary { get; set; }

        public IEnumerable<string> AllFiles { get; set; }

        public WebFormsFileDictionary AllWebFormsFiles { get; set; }
        
        public SolutionData()
        {
            // DataDictionary will be written to a json file.
            // Initializing the keys here will set the order.
            DataDictionary = new Dictionary<string, object>
            {
                { "Projects", 0 },
                { "WebFormsViewFiles", 0 },
                { "WebFormsCodeBehindFiles", 0 },
                { "UserControlFiles", 0 },
                { "UserControlCodeBehindFiles", 0 },
                { "MasterFiles", 0 },
                { "MasterFileCodeBehindFiles", 0 },
                { "TotalWebFormsFiles", 0 }
            };
            AllFiles = new List<string>();
            AllWebFormsFiles = new WebFormsFileDictionary();
        }
        
        public string GetJsonResult()
        {
            return JsonConvert.SerializeObject(DataDictionary);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Solution-Level Data:");
            foreach (var kvp in DataDictionary)
            {
                var fieldName = kvp.Key;
                var value = kvp.Value;

                sb.AppendLine($"-- {fieldName}: {value}");
            }

            return sb.ToString();
        }
    }
}