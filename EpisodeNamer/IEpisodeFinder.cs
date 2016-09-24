using System.Threading.Tasks;
using TvShowManager.Interfaces;

namespace EpisodeNamer
{
    public interface IEpisodeFinder
    {
        Task<EpisodeFile> GetEpisodeAsync(string file, string show, IEpisodeCrawler crawler);
    }
}
