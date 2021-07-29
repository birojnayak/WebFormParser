using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ASPXParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the directory of your Webform project");
            string directoryPath = Console.ReadLine();
            if(isValidPath(directoryPath))
            {
                ControlsData controls =  ReadAllNodes(directoryPath);

                Console.WriteLine("====================================Result in Console=================================");
                Console.WriteLine(controls.ToString());
                Console.WriteLine("====================================Result in Console=================================");
                try
                {
                    String currentDir = System.IO.Directory.GetCurrentDirectory();
                    String fileName = currentDir + "\\" + "controls" + Guid.NewGuid().ToString() + ".json";
                    File.AppendAllText(fileName, controls.GetJsonResult());
                    Console.WriteLine("====================================Result in Json=================================");
                    Console.WriteLine("||     " + fileName + "   ||");
                    Console.WriteLine("====================================Result in Json=================================");

                }
                catch (Exception ex)
                {
                    // don't throw as we have processed, show in the console
                }
            }
            else
            {
                Console.WriteLine("Entered path is invalid");
            }
            Console.Read();
        }

        private static ControlsData ReadAllNodes(String directoryPath)
        {
            try
            {
                var allFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".aspx") || s.EndsWith(".aspx.cs") || s.EndsWith(".ascx") || s.EndsWith(".ascx.cs") || s.EndsWith(".master") || s.EndsWith(".master.cs"))
            .OrderByDescending(filename => filename);

                ControlsData controls = new ControlsData();
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                List<string> allListOfFiles = allFiles.ToList();
                for(int i=0; i< allListOfFiles.Count; i++)
                {
                    if (allListOfFiles[i].EndsWith(".cs")) // since we have processed already
                    {
                        continue;
                    }

                    //get all codebehind events for a file
                    List<string> codeBehindEventLines = null;
                    if ( allListOfFiles[i-1].EndsWith(".cs"))
                    {
                        codeBehindEventLines = getCodeBehindEventsLines(allListOfFiles[i - 1]);

                    }

                    htmlDoc.Load(allListOfFiles[i]);
                    var nodes = htmlDoc.DocumentNode.Descendants();
                    foreach (HtmlNode nd in nodes)
                    {
                        List<string> attributes = new List<string>();
                        bool isServer = false;
                        string id = "";
                        foreach (HtmlAttribute att in nd.Attributes)
                        {
                            if (att.Name.Contains("runat", StringComparison.OrdinalIgnoreCase))
                            {
                                isServer = true;
                                continue;
                            }
                            if(att.Name.Contains("id", StringComparison.OrdinalIgnoreCase))
                            {
                                id = att.Value;
                                continue;
                            }
                            attributes.Add(att.Name);

                        }
                        if(isServer && id.Length > 0 && codeBehindEventLines != null && codeBehindEventLines.Count>0)
                        {
                           getCodeBehindEvents(codeBehindEventLines, id, attributes);
                        }
                        controls.AddControl(nd.Name, attributes.ToArray());
                    }
                }
                return controls;
            }
            catch(DirectoryNotFoundException ex)
            {
                throw new Exception("Directory path not found." + ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                throw new Exception("Access to path denied." + ex.Message);
            }
            catch(PathTooLongException ex)
            {
                throw new Exception("Path is too long" + ex.Message);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static bool isValidPath(String directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        private static List<string> getCodeBehindEventsLines(String filePath)
        {
            String pattern = ".+=.*EventHandler\\(.*\\)";
            List<string> serverEvents = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return serverEvents;
        }

        private static void getCodeBehindEvents(List<String> allLines, String id, List<string> attributes)
        {
            foreach (String line in allLines)
            {
                if (line.Contains( id + ".") && line.Contains(id+"_"))
                {
                    String fullLine = line.Trim();
                    int startIndex = fullLine.IndexOf(id + ".");
                    int endIndex = fullLine.IndexOf("+=");
                    int position = startIndex + id.Length + 1;
                    if (startIndex >= 0 && endIndex > startIndex && endIndex> position)
                    {
                        string attr = fullLine.Substring(position, endIndex- position);
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
