using System.Threading.Tasks;
using TvShowManager.Interfaces;

namespace EpisodeNamer
{
    public interface IFileParser
    {
        Task<string> GetEpisodeNameAsync(string file, IEpisodeCrawler crawler);
    }
}