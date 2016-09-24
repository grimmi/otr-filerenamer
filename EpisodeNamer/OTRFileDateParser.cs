using System;
using System.IO;
using System.Linq;

namespace EpisodeNamer
{
    public class OtrFileDateParser
    {
        private static string DateFormat = "yy.MM.dd";

        public DateTime GetDateOfFile(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var dateStartIdx = fileName.IndexOf('.') - 2;
            var dateEndIdx = fileName.Select((c, i) => new {Char = c, Index = i})
                .FirstOrDefault(p => p.Char == '_' && p.Index > dateStartIdx).Index;

            var datePart = fileName.Substring(dateStartIdx, dateEndIdx - dateStartIdx);

            return DateTime.ParseExact(datePart, DateFormat, null);
        }
    }
}
