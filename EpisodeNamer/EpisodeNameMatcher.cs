using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowManager;

namespace EpisodeNamer
{
    public class EpisodeNameMatcher
    {
        private string ExtractedName { get; }
        public EpisodeNameMatcher(string extractedName)
        {
            ExtractedName = extractedName;
        }

        public Episode GetMatchingEpisode(EpisodeList episodeList)
        {
            Episode match = null;
            var episodes = episodeList.Seasons.SelectMany(s => s.Episodes).ToList();

            match = episodes.FirstOrDefault(e => e.Name.ToLower().Equals(ExtractedName.ToLower()));
            if (match != null)
                return match;

            var episodeNameLow = ToLowerWithOnlyLettersAndNumbers(ExtractedName);
            match = episodes.FirstOrDefault(e => ToLowerWithOnlyLettersAndNumbers(e.Name).Equals(episodeNameLow));
            if (match != null)
                return match;

            match = episodes.FirstOrDefault(e => episodeNameLow.StartsWith(ToLowerWithOnlyLettersAndNumbers(e.Name)));
            if (match != null)
                return match;

            match = episodes.FirstOrDefault(e => ToLowerWithOnlyLettersAndNumbers(e.Name).StartsWith(episodeNameLow));
            if (match != null)
                return match;

            match = episodes.FirstOrDefault(e => episodeNameLow.EndsWith(ToLowerWithOnlyLettersAndNumbers(e.Name)));
            if (match != null)
                return match;

            match = episodes.FirstOrDefault(e => ToLowerWithOnlyLettersAndNumbers(e.Name).EndsWith(episodeNameLow));

            return match;
        }

        private string ToLowerWithOnlyLettersAndNumbers(string name)
        {
            return string.Concat(name.Where(c => char.IsLetter(c) || char.IsDigit(c))).ToLower();
        }
    }
}
