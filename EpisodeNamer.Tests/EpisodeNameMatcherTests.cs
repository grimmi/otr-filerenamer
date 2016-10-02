using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    public class EpisodeNameMatcherTests
    {
        [Test]
        public async Task GetMatchingEpisode_EqualName_GetMatchingEpisode()
        {
            var crawler = new FakeCrawler();
            var matcher = new EpisodeNameMatcher("episode 1 (pilot)");

            var matchingEpisode = matcher.GetMatchingEpisode(await crawler.DownloadEpisodeListAsync("foo"));

            Assert.AreEqual(1, matchingEpisode.Number);
            Assert.AreEqual(1, matchingEpisode.Season.Number);
            Assert.AreEqual("Episode 1 (Pilot)", matchingEpisode.Name);
        }

        [Test]
        public async Task GetMatchingEpisode_NameStartsWithRealEpisodeName_GetMatchingEpisode()
        {
            var crawler = new FakeCrawler();
            var matcher = new EpisodeNameMatcher("episode 1 (pilot) - erstaustrahlung");

            var matchingEpisode = matcher.GetMatchingEpisode(await crawler.DownloadEpisodeListAsync("foo"));

            Assert.AreEqual(1, matchingEpisode.Number);
            Assert.AreEqual(1, matchingEpisode.Season.Number);
            Assert.AreEqual("Episode 1 (Pilot)", matchingEpisode.Name);
        }

        [Test]
        public async Task GetMatchingEpisode_RealNameStartsWithExtractedName_GetMatchingEpisode()
        {
            var crawler = new FakeCrawler();
            var matcher = new EpisodeNameMatcher("episode 1");

            var matchingEpisode = matcher.GetMatchingEpisode(await crawler.DownloadEpisodeListAsync("foo"));

            Assert.AreEqual(1, matchingEpisode.Number);
            Assert.AreEqual(1, matchingEpisode.Season.Number);
            Assert.AreEqual("Episode 1 (Pilot)", matchingEpisode.Name);
        }
    }
}
