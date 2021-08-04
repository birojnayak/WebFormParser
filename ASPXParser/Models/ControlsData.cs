using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser.Models
{
    public class ControlsData
    {
        public IDictionary<string, ControlData> Controls = new Dictionary<string, ControlData>();
        
        public string GetJsonResult()
        {
            var allControls = Controls.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurrences.CompareTo(a.NumberOfOccurrences));
            return JsonConvert.SerializeObject(allControls);
        }

        public override string ToString()
        {
            List<ControlData> allControls = Controls.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurrences.CompareTo(a.NumberOfOccurrences));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Controls Data:");
            foreach (ControlData control in allControls)
            {
                sb.Append(control);
            }
            return sb.ToString();
        }
    }
}
