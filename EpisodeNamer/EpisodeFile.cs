using System.IO;
using TvShowManager;

namespace EpisodeNamer
{
    public class EpisodeFile
    {
        public Episode Episode { get; set; }
        public string File { get; set; }

        public override string ToString()
        {
            return $"{Path.GetFileName(File)} -> {Episode.Season.ShowName} {Episode.Season.Number}x{Episode.Number} {Episode.Name}";
        }
    }
}