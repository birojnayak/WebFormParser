using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASPXParser.Models;

namespace ASPXAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath;
            if (args.Any())
            {
                directoryPath = args[0];
            }
            else
            {
                Console.WriteLine("Please enter the directory of your Web Forms controls data: ");
                directoryPath = Console.ReadLine();
            }

            if (IsValidPath(directoryPath))
            {
                var files = Directory.EnumerateFiles(directoryPath);
                var controlDataAggregator = new ControlDataAggregator();
                foreach (var file in files)
                {
                    try
                    {
                        var controlDataCollection = JsonConvert.DeserializeObject<IEnumerable<ControlData>>(File.ReadAllText(file));
                        controlDataAggregator.Add(controlDataCollection);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error processing {file}: {e.Message}");
                    }
                }

                Console.WriteLine("====================================Result in Console=================================");
                Console.WriteLine(controlDataAggregator.ToString());
                Console.WriteLine("====================================Result in Console=================================");
                try
                {
                    var currentDir = Directory.GetCurrentDirectory();
                    var runId = Guid.NewGuid();
                    var totalsDataFile = currentDir + "\\" + "totals" + runId + ".json";
                    File.AppendAllText(totalsDataFile, controlDataAggregator.GetJsonResult());
                    
                    var countsOnlyDataFile = currentDir + "\\" + "counts" + runId + ".csv";
                    File.AppendAllText(countsOnlyDataFile, controlDataAggregator.GetCountTotalsInCsvFormat());

                    Console.WriteLine("====================================Result in Json=================================");
                    Console.WriteLine("||     " + totalsDataFile + "   ||");
                    Console.WriteLine("====================================Result in Json=================================");

                    Console.WriteLine("====================================Result in Csv=================================");
                    Console.WriteLine("||     " + countsOnlyDataFile + "   ||");
                    Console.WriteLine("====================================Result in Csv=================================");
                }
                catch
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

        private static bool IsValidPath(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
    }
}
