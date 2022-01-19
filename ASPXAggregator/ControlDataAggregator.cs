using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASPXParser.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ASPXAggregator
{
    public class ControlDataAggregator
    {
        public Dictionary<string, ControlData> Totals { get; set; }

        public ControlDataAggregator()
        {
            Totals = new Dictionary<string, ControlData>();
        }

        public void Add(IEnumerable<ControlData> controlDataList)
        {
            foreach (var controlData in controlDataList)
            {
                Add(controlData);
            }
        }

        public void Add(ControlData controlData)
        {
            if (Totals.ContainsKey(controlData.ControlName))
            {
                Totals[controlData.ControlName].NumberOfOccurrences += controlData.NumberOfOccurrences;
                AddAttributes(controlData);
            }
            else
            {
                Totals[controlData.ControlName] = controlData;
            }
        }

        public string GetJsonResult()
        {
            // Sort controls and control attributes by usage
            var sortedControlCountsEnumerable = Totals
                .Select(kvp =>
                {
                    var controlData = kvp.Value;
                    controlData.SortAttributes();
                    return controlData;
                })
                .OrderByDescending(controlData => controlData.NumberOfOccurrences);

            return JsonConvert.SerializeObject(sortedControlCountsEnumerable, Formatting.Indented);
        }

        public string GetCountTotalsInCsvFormat()
        {
            // Sort controls and control attributes by usage
            var sortedControlCountsEnumerable = Totals
                .Select(kvp =>
                {
                    var controlData = kvp.Value;
                    controlData.SortAttributes();
                    return controlData;
                })
                .OrderByDescending(controlData => controlData.NumberOfOccurrences);

            var sb = new StringBuilder();
            foreach (var controlCounts in sortedControlCountsEnumerable)
            {
                sb.AppendLine($"{controlCounts.ControlName},{controlCounts.NumberOfOccurrences}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            List<ControlData> controls = Totals.Values.ToList();
            controls.Sort((a, b) => b.NumberOfOccurrences.CompareTo(a.NumberOfOccurrences));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Controls Data:");
            foreach (ControlData control in controls)
            {
                sb.Append(control);
            }
            return sb.ToString();
        }

        private void AddAttributes(ControlData controlData)
        {
            var attributes = Totals[controlData.ControlName].Attributes;
            foreach (var kvp in controlData.Attributes)
            {
                var attributeName = kvp.Key;
                var attributeCounts = kvp.Value;

                if (attributes.ContainsKey(attributeName))
                {
                    attributes[attributeName] += attributeCounts;
                }
                else
                {
                    attributes[attributeName] = attributeCounts;
                }
            }
        }
    }
}