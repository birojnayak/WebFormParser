using System;
using System.Collections.Generic;

namespace ASPXParser.Models
{
    public class WebFormsFileDictionary
    {
        public Dictionary<string, WebFormsFileCouple> Lookup { get; set; } = new ();

        public void Add(string fileName, string fileExtension)
        {
            var endIndex = fileName.IndexOf(fileExtension, StringComparison.InvariantCultureIgnoreCase);

            if (endIndex == -1)
            {
                Console.WriteLine($"Extension {fileExtension} not found in {fileName}. Skipping file.");
                return;
            }

            var baseName = fileName.Substring(0, endIndex);

            // Create the entry if it does not exist
            if (!Lookup.ContainsKey(baseName))
            {
                Lookup[baseName] = new WebFormsFileCouple();
            }

            // Track file by type
            if (fileName.EndsWith(".cs"))
            {
                Lookup[baseName].CodeBehindFile = fileName;
            }
            else
            {
                Lookup[baseName].ViewFile = fileName;
            }
        }

        public int Count()
        {
            var count = 0;
            foreach (var (_, webFormsFileCouple) in Lookup)
            {
                if (!string.IsNullOrEmpty(webFormsFileCouple.CodeBehindFile))
                {
                    count++;
                }
                if (!string.IsNullOrEmpty(webFormsFileCouple.ViewFile))
                {
                    count++;
                }
            }

            return count;
        }
    }
}