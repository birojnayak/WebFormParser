using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                string[] allaspxFiles = Directory.GetFiles(directoryPath, "*.aspx", SearchOption.AllDirectories);
                var files = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".aspx") || s.EndsWith(".ascx") || s.EndsWith(".master"));
                ControlsData controls = new ControlsData();
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                foreach (String file in allaspxFiles)
                {
                    htmlDoc.Load(file);
                    var nodes = htmlDoc.DocumentNode.Descendants();
                    foreach (HtmlNode nd in nodes)
                    {
                        List<string> attributes = new List<string>();
                        foreach (HtmlAttribute att in nd.Attributes)
                        {
                            if (att.Name.Contains("runat", StringComparison.OrdinalIgnoreCase) || att.Name.Contains("id", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            attributes.Add(att.Name);

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
    }
}
