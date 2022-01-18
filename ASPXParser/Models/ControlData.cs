using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASPXParser.Models
{
    public class ControlData
    {
        public enum ControlType
        {
            Html,
            Asp,
            Custom
        }

        public string ControlName { get; set; }
        private ControlType TypeOfControl { get; }
        public int NumberOfOccurrences { get; set; }
        public IDictionary<string, int> Attributes { get; set; } = new Dictionary<string, int>();

        public ControlData()
        {
        }

        public ControlData(string name, ControlType typ, string[] attrs)
        {
            ControlName = name;
            TypeOfControl = typ;
            NumberOfOccurrences = 1;
            AddAttributes(attrs);
        }

        public void Increase(string[] attrs)
        {
            NumberOfOccurrences += 1;
            AddAttributes(attrs);
        }

        private void AddAttributes(string[] attrs)
        {
            foreach (var attr in attrs)
            {
                if (Attributes.ContainsKey(attr))
                {
                    Attributes[attr] += 1;
                }
                else
                {
                    Attributes[attr] = 1;
                }
            }
        }

        public IDictionary<string, int> SortAttributes()
        {
            Attributes = Attributes.OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return Attributes;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-- Name :{0}, Type: {1}, Count: {2}", ControlName, TypeOfControl.ToString(), NumberOfOccurrences).AppendLine();
            Attributes.OrderBy(v => v.Value);
            foreach (var key in Attributes.Keys)
            {
                sb.AppendFormat("  |-- Attribute:{0}, Count:{1}", key, Attributes[key]).AppendLine();
            }
            return sb.ToString();
        }
    }
}