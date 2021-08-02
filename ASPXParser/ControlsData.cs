using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ASPXParser
{
    public class ControlsData
    {
        IDictionary<string, ControlData> controlsNumbers = new Dictionary<string, ControlData>();

        public void GetData(IEnumerable<string> webFormsFiles)
        {
            try
            {
                var htmlDoc = new HtmlDocument
                {
                    OptionFixNestedTags = true
                };
                var webFormsFilesList = webFormsFiles
                    .OrderByDescending(filename => filename)
                    .ToList();
                for (var i = 0; i < webFormsFilesList.Count; i++)
                {
                    if (webFormsFilesList[i].EndsWith(".cs")) // since we have processed already
                    {
                        continue;
                    }

                    // get all codebehind events for a file
                    List<string> codeBehindEventLines = null;
                    if (webFormsFilesList[i - 1].EndsWith(".cs"))
                    {
                        codeBehindEventLines = GetCodeBehindEventsLines(webFormsFilesList[i - 1]);
                    }

                    htmlDoc.Load(webFormsFilesList[i]);
                    var nodes = htmlDoc.DocumentNode.Descendants();
                    foreach (var htmlNode in nodes)
                    {
                        var attributes = new List<string>();
                        var isServer = false;
                        var id = "";
                        foreach (var att in htmlNode.Attributes)
                        {
                            if (att.Name.Contains("runat", StringComparison.OrdinalIgnoreCase))
                            {
                                isServer = true;
                                continue;
                            }
                            if (att.Name.Contains("id", StringComparison.OrdinalIgnoreCase))
                            {
                                id = att.Value;
                                continue;
                            }
                            attributes.Add(att.Name);

                        }
                        if (isServer && id.Length > 0 && codeBehindEventLines != null && codeBehindEventLines.Count > 0)
                        {
                            GetCodeBehindEvents(codeBehindEventLines, id, attributes);
                        }
                        AddControl(htmlNode.Name, attributes.ToArray());
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new Exception("Directory path not found." + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Access to path denied." + ex.Message);
            }
            catch (PathTooLongException ex)
            {
                throw new Exception("Path is too long" + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddControl(string controlName, string[] events)
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
                return; // basic html we don't care 
            }

            if (controlsNumbers.ContainsKey(controlName))
            {
                controlsNumbers[controlName].Increase(events);
            }
            else
            {
                controlsNumbers.Add(controlName, new ControlData(controlName, type, events));
            }
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
            sb.AppendLine("Controls Data:");
            foreach (ControlData control in allControls)
            {
                sb.Append(control);
            }
            return sb.ToString();
        }

        private List<string> GetCodeBehindEventsLines(string filePath)
        {
            var pattern = ".+=.*EventHandler\\(.*\\)";
            var serverEvents = new List<string>();

            try
            {
                using StreamReader reader = new StreamReader(filePath);
                string line;
                while ((line = reader.ReadLine()) != null && !line.StartsWith("//"))
                {
                    Match m = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        serverEvents.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return serverEvents;
        }

        private void GetCodeBehindEvents(List<string> allLines, string id, List<string> attributes)
        {
            foreach (var line in allLines)
            {
                if (line.Contains(id + ".") && line.Contains(id + "_"))
                {
                    var fullLine = line.Trim();
                    var startIndex = fullLine.IndexOf(id + ".");
                    var endIndex = fullLine.IndexOf("+=");
                    var position = startIndex + id.Length + 1;
                    if (startIndex >= 0 && endIndex > startIndex && endIndex > position)
                    {
                        var attr = fullLine.Substring(position, endIndex - position);
                        if (!string.IsNullOrEmpty(attr))
                        {
                            attributes.Add(attr.Trim());
                        }
                    }
                }
            }
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
            private string ControlName { get; }

            private ControlType TypeOfControl { get; }

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
