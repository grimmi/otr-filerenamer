﻿using System.Linq;
using System.Threading.Tasks;
using TvShowManager;
using TvShowManager.Interfaces;

namespace EpisodeNamer
{
    public class OtrEpisodeFinder : IEpisodeFinder
    {
        private string EpisodeFile { get; set; }
        private string Show { get; set; }
        private IEpisodeCrawler Crawler { get; set; }
        
        public async Task<EpisodeFile> GetEpisodeAsync(string episodeFile, string show, IEpisodeCrawler crawler)
        {
            EpisodeFile = episodeFile;
            Show = show;
            Crawler = crawler;
            var episodes = await Crawler.DownloadEpisodeListAsync(Show);
            Episode match = null;
            var episodeName = ExtractEpisodeNameFromOTRFileName(EpisodeFile);
            if (string.IsNullOrWhiteSpace(episodeName))
            {
                match = FindEpisodeByDate(EpisodeFile, episodes);
            }
            else
            {
                match = FindEpisodeForEpisodeName(episodeName, episodes);
            }

            return new EpisodeFile { Episode = match, File = EpisodeFile };
        }

        private Episode FindEpisodeByDate(string fileToRename, EpisodeList episodes)
        {
            var dateParser = new OtrFileDateParser();
            var episodeDate = dateParser.GetDateOfFile(fileToRename);

            return episodes.Seasons.SelectMany(s => s.Episodes).FirstOrDefault(e => e.FirstAired == episodeDate);
        }

        private Episode FindEpisodeForEpisodeName(string episodeName, EpisodeList episodeList)
        {
            Episode match = null;
            var episodes = episodeList.Seasons.SelectMany(s => s.Episodes).ToList();

            match = episodes.FirstOrDefault(e => e.Name.ToLower().Equals(episodeName.ToLower()));

            if (match != null)
                return match;

            var lowOnlyLetters = string.Concat(episodeName.Where(char.IsLetter)).ToLower();
            match = episodes.FirstOrDefault(e => string.Concat(e.Name.Where(char.IsLetter)).ToLower().Equals(lowOnlyLetters));

            if (match != null)
                return match;

            return match;
        }

        public string ExtractEpisodeNameFromOTRFileName(string filePath)
        {
            var parser = new OtrFileEpisodeParser(filePath);
            return parser.GetEpisodeName();
        }
    }
}
