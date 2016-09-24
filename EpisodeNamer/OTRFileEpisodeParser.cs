using System.IO;

namespace EpisodeNamer
{
    public class OtrFileEpisodeParser
    {
        private string filePath;
        public OtrFileEpisodeParser(string otrFilePath)
        {
            filePath = otrFilePath;
        }

        public string GetEpisodeName()
        {
            var fileName = Path.GetFileName(filePath);
            var doubleUnderscoreIdx = fileName.IndexOf("__");

            if (doubleUnderscoreIdx == -1)
            {
                // dateiname ohne episodenname
                return string.Empty;
            }

            var firstColonIdx = fileName.IndexOf('.') - 3;

            var episodeName = fileName.Substring(doubleUnderscoreIdx + 1, firstColonIdx - doubleUnderscoreIdx);
            episodeName = episodeName.Replace("_", " ").TrimStart().TrimEnd();

            return episodeName;
        }
    }
}