using System;
using System.IO;
using System.Linq;

namespace ASPXParser
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
                Console.WriteLine("Please enter the directory of your Web Forms project: ");
                directoryPath = Console.ReadLine();
            }

            if (IsValidPath(directoryPath))
            {
                var solutionData = new SolutionData();
                solutionData.GetData(directoryPath);

                var controlsData = new ControlsData();
                controlsData.GetData(solutionData.AllWebFormsFiles);

                Console.WriteLine("====================================Result in Console=================================");
                Console.WriteLine(solutionData.ToString());
                Console.WriteLine(controlsData.ToString());
                Console.WriteLine("====================================Result in Console=================================");
                try
                {
                    var currentDir = Directory.GetCurrentDirectory();
                    var runId = Guid.NewGuid();
                    var solutionDataFile = currentDir + "\\" + "solution" + runId + ".json";
                    File.AppendAllText(solutionDataFile, solutionData.GetJsonResult());
                    var controlsDataFile = currentDir + "\\" + "controls" + runId + ".json";
                    File.AppendAllText(controlsDataFile, controlsData.GetJsonResult());

                    Console.WriteLine("====================================Result in Json=================================");
                    Console.WriteLine("||     " + solutionDataFile + "   ||");
                    Console.WriteLine("||     " + controlsDataFile + "   ||");
                    Console.WriteLine("====================================Result in Json=================================");
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