using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser
{
    public class ControlsData
    {
        IDictionary<String, ControlData> controlsNumbers = new Dictionary<String, ControlData>();
        public ControlsData()
        {
        }

        public bool AddControl(string controlName, string[] events)
        {
            ControlType type = ControlType.Html;
            if (controlName.Contains("asp:", StringComparison.OrdinalIgnoreCase))
            {
                type = ControlType.Asp;
            }
            else if (controlName.Contains(":"))
            {
                type = ControlType.Custom;
            }
            else
            {
                return false; // basic html we don't care 
            }

            if (controlsNumbers.ContainsKey(controlName))
            {
                controlsNumbers[controlName].Increase(events);
            }
            else
            {
                controlsNumbers.Add(controlName, new ControlData(controlName, type, events));
            }
            return true;
        }
        public String GetJsonResult()
        {
            List<ControlData> allControls = controlsNumbers.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurences.CompareTo(a.NumberOfOccurences));
            return JsonConvert.SerializeObject(allControls);
        }
        public override String ToString()
        {
            List<ControlData> allControls = controlsNumbers.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurences.CompareTo(a.NumberOfOccurences));
            StringBuilder sb = new StringBuilder();
            foreach(ControlData control in allControls)
            {
                sb.Append(control);
            }
            return sb.ToString();
        }
        private class ControlData
        {
            IDictionary<string, int> attributes = new Dictionary<string, int>();
            public ControlData(string name, ControlType typ, string[] attrs)
            {
                ControlName = name;
                TypeOfControl = typ;
                NumberOfOccurences = 1;
                addAttributes(attrs);
            }
            public String ControlName { get; set; }
            internal ControlType TypeOfControl { get; set; }
            public int NumberOfOccurences { get; set; }
            public IDictionary<string, int> Attributes { get { return attributes; }}
            public void Increase(string[] attrs)
            {
                NumberOfOccurences += 1;
                addAttributes(attrs);
            }

            private void addAttributes(string[] attrs)
            {
                foreach(string attr in attrs)
                {
                   if(attributes.ContainsKey(attr))
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
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("-- Name :{0}, Type: {1}, Count: {2}", ControlName, TypeOfControl.ToString(), NumberOfOccurences).AppendLine();
                attributes.OrderBy(v => v.Value);
                foreach (string key in attributes.Keys)
                {
                    sb.AppendFormat("  |-- Attribute:{0}, Count:{1}", key, attributes[key]).AppendLine();
                }
                return sb.ToString();
            }

        }
        private enum ControlType
        {
            Html,
            Asp,
            Custom
        }
    }

}
