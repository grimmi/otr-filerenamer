using System.IO;

namespace EpisodeNamer
{
    public class FileNameGenerator
    {
        public string GenerateFileName(EpisodeFile episodeFile)
        {
            var ep = episodeFile.Episode;
            var s = ep.Season;

            var ext = Path.GetExtension(episodeFile.File);

            return $"{s.ShowName} {s.Number}x{ep.Number} {ep.Name}{ext}";
        }
    }
}
