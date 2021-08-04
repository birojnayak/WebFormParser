using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser.Models
{
    public class SolutionData
    {
        public IDictionary<string, object> DataDictionary { get; set; }

        public IEnumerable<string> AllFiles { get; set; }

        public IEnumerable<string> AllWebFormsFiles { get; set; }

        public SolutionData()
        {
            DataDictionary = new Dictionary<string, object>();
            AllFiles = new List<string>();
            AllWebFormsFiles = new List<string>();
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