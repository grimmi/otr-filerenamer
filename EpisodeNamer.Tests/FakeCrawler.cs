using System;
using System.Threading.Tasks;
using TvShowManager;
using TvShowManager.Interfaces;

namespace EpisodeNamer.Tests
{
    public class FakeCrawler : IEpisodeCrawler
    {
        public Task<EpisodeList> DownloadEpisodeListAsync(string showName)
        {
            return Task.FromResult(new EpisodeList
            {
                ShowName = showName,
                Seasons = new[]
                {
                    Season1(showName)
                }
            });
        }

        private static Season Season1(string showName)
        {
            var s1 = new Season
            {
                ShowName = showName,
                Number = 1
            };
            s1.Episodes = new[]
            {
                new Episode
                {
                    FirstAired = new DateTime(2010, 1, 1),
                    Name = "Episode 1 (Pilot)",
                    Number = 1,
                    Season = s1
                },
                new Episode
                {
                    FirstAired = new DateTime(2010, 1, 8),
                    Name = "Episode 2",
                    Number = 2,
                    Season = s1
                },
                new Episode
                {
                    FirstAired = new DateTime(2010, 1, 15),
                    Name = @"Some, episode with#special chars!",
                    Number = 3,
                    Season = s1
                }
            };
            return s1;
        }
    }
}