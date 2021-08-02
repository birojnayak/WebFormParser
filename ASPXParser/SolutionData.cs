using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ASPXParser
{
    public class SolutionData
    {
        public IDictionary<string, object> DataDictionary { get; set; }

        public IEnumerable<string> AllFiles { get; set; }

        public IEnumerable<string> AllWebFormsFiles { get; set; }

        public SolutionData()
        {
            DataDictionary = new Dictionary<string, object>();
            AllFiles = new List<string>();
            AllWebFormsFiles = new List<string>();
        }

        public void GetData(string directoryPath)
        {
            AllFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            AllWebFormsFiles = AllFiles.Where(s => s.EndsWith(".aspx")
                                                   || s.EndsWith(".aspx.cs")
                                                   || s.EndsWith(".ascx")
                                                   || s.EndsWith(".ascx.cs")
                                                   || s.EndsWith(".master")
                                                   || s.EndsWith(".master.cs"))
                .OrderByDescending(filename => filename);

            AddProjectCountToDataDictionary(DataDictionary, AllFiles);
            AddFileCountsToDataDictionary(DataDictionary, AllWebFormsFiles);
        }

        public string GetJsonResult()
        {
            return JsonConvert.SerializeObject(DataDictionary);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Solution-Level Data:");
            foreach (var kvp in DataDictionary)
            {
                var fieldName = kvp.Key;
                var value = kvp.Value;

                sb.AppendLine($"-- {fieldName}: {value}");
            }

            return sb.ToString();
        }

        private void AddProjectCountToDataDictionary(
            IDictionary<string, object> dataDictionary, 
            IEnumerable<string> filesInSolutionDir)
        {
            var projectFiles = filesInSolutionDir.Where(file => file.EndsWith(".csproj"));
            dataDictionary["Projects"] = projectFiles.Count();
        }

        private void AddFileCountsToDataDictionary(IDictionary<string, object> dataDictionary, IEnumerable<string> filesInSolutionDir)
        {
            var webFormsViewFiles = filesInSolutionDir.Count(file =>
                file.EndsWith(FileExtension.WebFormsView));
            dataDictionary["WebFormsViewFiles"] = webFormsViewFiles;

            var webFormsCodeBehindFiles = filesInSolutionDir.Count(file => 
                file.EndsWith(FileExtension.WebFormsCodeBehind));
            dataDictionary["WebFormsCodeBehindFiles"] = webFormsCodeBehindFiles;

            var userControlFiles = filesInSolutionDir.Count(file => 
                file.EndsWith(FileExtension.UserControl));
            dataDictionary["UserControlFiles"] = userControlFiles;

            var userControlCodeBehindFiles = filesInSolutionDir.Count(file => 
                file.EndsWith(FileExtension.UserControlCodeBehind));
            dataDictionary["UserControlCodeBehindFiles"] = userControlCodeBehindFiles;

            var masterFiles = filesInSolutionDir.Count(file => 
                file.EndsWith(FileExtension.MasterFile));
            dataDictionary["MasterFiles"] = masterFiles;

            var masterCodeBehindFiles = filesInSolutionDir.Count(file => 
                file.EndsWith(FileExtension.MasterFileCodeBehind));
            dataDictionary["MasterCodeBehindFiles"] = masterCodeBehindFiles;

            dataDictionary["TotalWebFormsFiles"] = webFormsViewFiles
                                                   + webFormsCodeBehindFiles
                                                   + userControlFiles
                                                   + userControlCodeBehindFiles
                                                   + masterFiles
                                                   + masterCodeBehindFiles;
        }
    }
}