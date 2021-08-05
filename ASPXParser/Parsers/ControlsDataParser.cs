using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ASPXParser.Models;
using HtmlAgilityPack;

namespace ASPXParser.Parsers
{
    public class ControlsDataParser
    {
        private IEnumerable<string> WebFormsFiles { get; }
        public ControlsData ControlsData { get; set; }

        public ControlsDataParser(IEnumerable<string> webFormsFiles)
        {
            WebFormsFiles = webFormsFiles;
            ControlsData = new ControlsData();
        }

        public void GetData()
        {
            try
            {
                var htmlDoc = new HtmlDocument
                {
                    OptionFixNestedTags = true
                };
                var webFormsFilesList = WebFormsFiles
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
            Models.ControlData.ControlType type;
            if (controlName.Contains("asp:", StringComparison.OrdinalIgnoreCase))
            {
                type = ControlData.ControlType.Asp;
            }
            else if (controlName.Contains(":"))
            {
                type = ControlData.ControlType.Custom;
            }
            else
            {
                return; // basic html we don't care 
            }

            if (ControlsData.Controls.ContainsKey(controlName))
            {
                ControlsData.Controls[controlName].Increase(events);
            }
            else
            {
                ControlsData.Controls.Add(controlName, new ControlData(controlName, type, events));
            }
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
    }
}