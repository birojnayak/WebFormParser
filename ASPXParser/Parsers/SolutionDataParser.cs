using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASPXParser.Models;

namespace ASPXParser.Parsers
{
    public class SolutionDataParser
    {
        private string DirectoryPath { get; }
        public SolutionData SolutionData { get; set; }

        public SolutionDataParser(string directoryPath)
        {
            DirectoryPath = directoryPath;
            SolutionData = new SolutionData();
        }

        public void GetData()
        {
            SolutionData.AllFiles = Directory.EnumerateFiles(DirectoryPath, "*.*", SearchOption.AllDirectories).ToList();
            AggregateFiles(SolutionData.AllFiles, SolutionData.DataDictionary, SolutionData.AllWebFormsFiles as List<string>);
        }

        private void AggregateFiles(IEnumerable<string> allFiles, 
            IDictionary<string, object> dataDictionary, 
            List<string> allWebFormsFiles)
        {
            foreach(var filename in allFiles)
            {
                if (filename.EndsWith(FileExtension.WebFormsView,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("WebFormsViewFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.WebFormsCodeBehind,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("WebFormsCodeBehindFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.UserControl,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("UserControlFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.UserControlCodeBehind, 
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("UserControlCodeBehindFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.MasterFile,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("MasterFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.MasterFileCodeBehind,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("MasterFileCodeBehindFiles", dataDictionary);
                    allWebFormsFiles.Add(filename);
                }
                else if (filename.EndsWith(FileExtension.Project,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    IncrementFileTypeCountInDict("Projects", dataDictionary);
                }
                // File is ignored if extension is not specified

                dataDictionary["TotalWebFormsFiles"] = allWebFormsFiles.Count();
            }
        }

        private void IncrementFileTypeCountInDict(string fileType, IDictionary<string, object> dataDictionary)
        {
            if (dataDictionary.ContainsKey(fileType))
            {
                var currentCount = (int) dataDictionary[fileType];
                dataDictionary[fileType] = currentCount + 1;
            }
            else 
            {
                dataDictionary[fileType] = 1;
            }
        }
    }
}