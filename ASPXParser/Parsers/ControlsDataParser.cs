using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ASPXParser.Models;
using HtmlAgilityPack;

namespace ASPXParser.Parsers
{
    public class ControlsDataParser
    {
        private WebFormsFileDictionary WebFormsFiles { get; }
        public ControlsData ControlsData { get; set; }

        public ControlsDataParser(WebFormsFileDictionary webFormsFiles)
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

                foreach(var (fileBaseName, fileCouple) in  WebFormsFiles.Lookup)
                {
                    // Codebehind events for a file
                    List<string> codeBehindEventLines = null;
                    if (!string.IsNullOrEmpty(fileCouple.CodeBehindFile))
                    {
                        codeBehindEventLines = GetCodeBehindEventsLines(fileCouple.CodeBehindFile);
                    }

                    // Identify controls, control attributes, and codebehind events
                    if (!string.IsNullOrEmpty(fileCouple.ViewFile))
                    {
                        htmlDoc.Load(fileCouple.ViewFile);
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
                            if (isServer 
                                && id.Length > 0 
                                && codeBehindEventLines != null 
                                && codeBehindEventLines.Count > 0)
                            {
                                GetCodeBehindEvents(codeBehindEventLines, id, attributes);
                            }
                            AddControl(htmlNode.Name, attributes.ToArray());
                        }
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
            catch (Exception)
            {
                Console.WriteLine("Error while parsing controls data.");
                throw;
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
                return; // ignore basic html elements 
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
            catch (Exception)
            {
                Console.WriteLine($"Error while processing codebehind event lines in: {filePath}");
                throw;
            }

            return serverEvents;
        }

        private void GetCodeBehindEvents(List<string> allLines, string id, List<string> attributes)
        {
            foreach (var line in allLines)
            {
                if (line.Contains(id + ".") 
                    && line.Contains(id + "_")
                    && line.Contains("+="))
                {
                    var fullLine = line.Trim();
                    var startIndex = fullLine.IndexOf(id + ".");
                    var position = startIndex + id.Length + 1;
                    var endIndex = fullLine.IndexOf("+=");
                    if (IsValidIndex(fullLine, startIndex) 
                        && IsValidIndex(fullLine, position)
                        && IsValidIndex(fullLine, endIndex)
                        && endIndex > position)
                    {
                        var attr = fullLine.Substring(position, endIndex - position);
                        if (!string.IsNullOrEmpty(attr))
                        {
                            attributes.Add(attr.Trim());
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not extract codebehind event from line: {fullLine}");
                    }
                }
            }
        }

        private bool IsValidIndex(string content, int index)
        {
            return index > -1
                   && index < content.Length;
        }
    }
}