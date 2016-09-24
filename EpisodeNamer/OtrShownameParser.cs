using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EpisodeNamer
{
    public class OtrShowNameParser
    {
        // pattern matcht auf alles, was vor der ersten abfolge von drei zweistelligen zahlen,
        // die durch punkte getrennt sind, kommt
        // bspw: 'some_show_10.10.10_xxxxxxxx' würde 'some_show_' ergeben
        private static string showNamePattern = @"^.*?(?=([\d]{2}.[\d]{2}.[\d]{2}))";

        public string GetShowName(string file)
        {
            var fileName = Path.GetFileName(file);
            var show = "";
            // no episode name in filename
            if (fileName.IndexOf("__") == -1)
            {
                show = Regex.Match(fileName, showNamePattern).Captures[0].Value;
                show = show.Replace("_", " ").Trim();
            }
            else
            {
                show = fileName.Substring(0, fileName.IndexOf("__")).Replace("_", " ");
            }
            return show;
        }
    }
}
