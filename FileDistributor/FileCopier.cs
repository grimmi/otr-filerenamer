using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributor
{
    public class FileCopier
    {
        private string RootPath { get; }
        public FileCopier(string rootPath)
        {
            RootPath = rootPath;
        }

        public void DistributeFiles(string group, Dictionary<string, string> sourceToTargetFiles)
        {
            var groupPath = Path.Combine(RootPath, group);
            if (!Directory.Exists(groupPath))
            {
                Directory.CreateDirectory(groupPath);
            }

            foreach (var kvp in sourceToTargetFiles)
            {
                var source = kvp.Key;
                var target = kvp.Value;

                var targetPath = Path.Combine(groupPath, target);

                if (!File.Exists(targetPath))
                {
                    Console.WriteLine("Kopiere Datei");
                    Console.WriteLine($"kopiere {source}");
                    Console.WriteLine($"nach {targetPath}...");
                    File.Copy(source, targetPath);
                    Console.WriteLine("Kopiervorgang abgeschlossen");
                    Console.WriteLine("---------------------------------------------------");
                }
            }
        }
    }
}
