using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser
{
    public class ControlsData
    {
        IDictionary<string, ControlData> controlsNumbers = new Dictionary<string, ControlData>();

        public bool AddControl(string controlName, string[] events)
        {
            ControlType type;
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

        public string GetJsonResult()
        {
            var allControls = controlsNumbers.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurrences.CompareTo(a.NumberOfOccurrences));
            return JsonConvert.SerializeObject(allControls);
        }

        public override string ToString()
        {
            List<ControlData> allControls = controlsNumbers.Values.ToList();
            allControls.Sort((a, b) => b.NumberOfOccurrences.CompareTo(a.NumberOfOccurrences));
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
                NumberOfOccurrences = 1;
                AddAttributes(attrs);
            }
            public string ControlName { get; }

            internal ControlType TypeOfControl { get; }

            public int NumberOfOccurrences { get; set; }

            public IDictionary<string, int> Attributes => attributes;

            public void Increase(string[] attrs)
            {
                NumberOfOccurrences += 1;
                AddAttributes(attrs);
            }

            private void AddAttributes(string[] attrs)
            {
                foreach(var attr in attrs)
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
                sb.AppendFormat("-- Name :{0}, Type: {1}, Count: {2}", ControlName, TypeOfControl.ToString(), NumberOfOccurrences).AppendLine();
                attributes.OrderBy(v => v.Value);
                foreach (var key in attributes.Keys)
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
