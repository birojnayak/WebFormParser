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

        private IDictionary<string, int> attributes = new Dictionary<string, int>();
        public string ControlName { get; }
        private ControlType TypeOfControl { get; }
        public int NumberOfOccurrences { get; set; }
        public IDictionary<string, int> Attributes => attributes;

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
                if (attributes.ContainsKey(attr))
                {
                    attributes[attr] += 1;
                }
                else
                {
                    attributes[attr] = 1;
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-- Name :{0}, Type: {1}, Count: {2}", ControlName, TypeOfControl.ToString(), NumberOfOccurrences).AppendLine();
            attributes.OrderBy(v => v.Value);
            foreach (var key in attributes.Keys)
            {
                sb.AppendFormat("  |-- Attribute:{0}, Count:{1}", key, attributes[key]).AppendLine();
            }
            return sb.ToString();
        }
    }
}