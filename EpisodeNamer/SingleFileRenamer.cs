using System.IO;
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

            return Path.Combine(targetFolder, newFileName);
        }
    }
}