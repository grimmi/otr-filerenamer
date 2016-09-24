using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TvShowManager.Interfaces;

namespace EpisodeNamer
{
    public class SingleFileRenamer
    {
        private IEpisodeCrawler Crawler { get; }
        private IEpisodeFinder Finder { get; }
        private string FileToRename { get; }
        private string Show { get; }

        public SingleFileRenamer(string filePath, string showName, IEpisodeCrawler crawler, IEpisodeFinder finder)
        {
            FileToRename = filePath;
            Show = showName;
            Crawler = crawler;
            Finder = finder;
        }

        public async Task<string> RenameFile(string targetFolder)
        {
            var episodeFile = await Finder.GetEpisodeAsync(FileToRename, Show, Crawler);

            var generator = new FileNameGenerator();

            var newFileName = generator.GenerateFileName(episodeFile);
            newFileName = CleanFileName(newFileName);

            return Path.Combine(targetFolder, newFileName);
        }

        private string CleanFileName(string newFileName)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPathChars = Path.GetInvalidPathChars();

            var cleanFileName =
                new string(newFileName.Where(c => !invalidFileNameChars.Contains(c) && !invalidPathChars.Contains(c)).ToArray());

            return cleanFileName;
        }
    }
}