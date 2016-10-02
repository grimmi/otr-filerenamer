using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TvShowManager;
using TvShowManager.Interfaces;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class OtrEpisodeFinderTests
    {
        [Test]
        [TestCase("some_show__episode_1_(pilot)_16.04.23.mpg.avi", "some show", "Episode 1 (Pilot)")]
        [TestCase("some_show__some_episode_with_special_chars_16.04.23.mpg.avi", "some show", "Some, episode with#special chars!")]
        public async Task GetEpisodeAsync_EpisodeNameInFileNameAndKnownShowname_CorrectlyRenameFile(string file, string show, string expectedEpisodeName)
        {
            var crawler = GetShowCrawler();
            var parser = GetFileParser();

            var episodeFinder = new OtrEpisodeFinder();
            var episodeInfo = await episodeFinder.GetEpisodeAsync(file, show, crawler);

            Assert.AreEqual(expectedEpisodeName, episodeInfo.Episode.Name);
        }

        [Test]
        [TestCase("some_show_10.01.01_.mpg.avi", "some show", "Episode 1 (Pilot)")]
        [TestCase("some_show_10.01.15_.mpg.avi", "some show", "Some, episode with#special chars!")]
        public async Task GetEpisodeAsync_EpisodeWithoutEpisodeName_CorrectEpisode(string file, string show, string expectedEpisode)
        {
            var crawler = GetShowCrawler();

            var episodeFinder = new OtrEpisodeFinder();
            var episodeInfo = await episodeFinder.GetEpisodeAsync(file, show, crawler);

            Assert.AreEqual(expectedEpisode, episodeInfo.Episode.Name);
        }

        [Test]
        [TestCase("The_Simpsons_16.03.06_20-00_uswnyw_30_TVOON_DE.mpg.avi.otrkey", 15, 27, "Lisa the Veterinarian")]
        public async Task GetEpisodeAsync_SimpsonsEpisode_CorrectSimpsonsEpisodeAndSeason(string file, int episodeNr, int seasonNr, string episodeName)
        {
            var crawler = new SimpsonsCrawler();
            var episodeFinder = new OtrEpisodeFinder();
            var episodeInfo = await episodeFinder.GetEpisodeAsync(file, "The Simpsons", crawler);

            var episode = episodeInfo.Episode;
            Assert.AreEqual(episodeNr, episode.Number);
            Assert.AreEqual(seasonNr, episode.Season.Number);
        }

        [Test]
        [TestCase("some_show__episode_1_(pilot)_s01e01_10.01.01_.mpg.avi", "Episode 1 (Pilot)")]
        public async Task GetEpisodeAsync_EpisodeWithExtraText_CorrectEpisode(string file, string expName)
        {
            var crawler = GetShowCrawler();
            var finder = new OtrEpisodeFinder();

            var episodeInfo = await finder.GetEpisodeAsync(file, "some show", crawler);

            Assert.AreEqual(expName, episodeInfo.Episode.Name);
        }

        [Test]
        [TestCase("some_show__episode_1_(pilot)..mpg.avi", "some show", "Episode 1 (Pilot)")]
        public async Task GetEpisodeAsync_EpisodeWithoutDate_CorrectEpisode(string file, string show, string expName)
        {
            var crawler = GetShowCrawler();
            var finder = new OtrEpisodeFinder();

            var episode = await finder.GetEpisodeAsync(file, show, crawler);

            Assert.AreEqual(expName, episode.Episode.Name);
        }

        [Test]
        public async Task GetEpisodeAsync_iZombieS02E05_CorrectEpisode()
        {
            var crawler = GetIZombieCrawler();
            var finder = new OtrEpisodeFinder();

            var episode =
                await finder.GetEpisodeAsync("iZombie__Love_Basketball_15.11.03_21-00_uswpix.mpg.HQ.avi", "iZombie", crawler);

            Assert.AreEqual("Love & Basketball", episode.Episode.Name);
            Assert.AreEqual(2, episode.Episode.Season.Number);
            Assert.AreEqual(5, episode.Episode.Number);
        }

        private IFileParser GetFileParser()
        {
            return new FakeParser();
        }

        private IEpisodeCrawler GetShowCrawler()
        {
            return new FakeCrawler();
        }

        private IEpisodeCrawler GetIZombieCrawler()
        {
            return new IZombieCrawler();
        }
    }

    public class IZombieCrawler : IEpisodeCrawler
    {
        public Task<EpisodeList> DownloadEpisodeListAsync(string showName)
        {
            var episodeList = new EpisodeList();
            var s2 = new Season
            {
                Number = 2,
                ShowName = "iZombie"
            };
            var s2e1 = new Episode
            {
                FirstAired = new DateTime(2015, 10, 6),
                Name = "Grumpy Old Liv",
                Number = 1,
                Season = s2
            };
            var s2e2 = new Episode
            {
                FirstAired = new DateTime(2015, 10, 13),
                Name = "Zombie Bro",
                Number = 2,
                Season = s2
            };
            var s2e3 = new Episode
            {
                FirstAired = new DateTime(2015, 10, 20),
                Name = "Real Dead Housewife of Seattle",
                Number = 3,
                Season = s2
            };
            var s2e4 = new Episode
            {
                FirstAired = new DateTime(2015, 10, 27),
                Name = "Even Cowgirls Get the Black and Blues",
                Number = 4,
                Season = s2
            };
            var s2e5 = new Episode
            {
                FirstAired = new DateTime(2015, 11, 3),
                Name = "Love & Basketball",
                Number = 5,
                Season = s2
            };

            s2.Episodes = new[] { s2e1, s2e2, s2e3, s2e4, s2e5 };
            episodeList.Seasons = new[] { s2 };
            return Task.FromResult(episodeList);
        }
    }

    public class SimpsonsCrawler : IEpisodeCrawler
    {
        public Task<EpisodeList> DownloadEpisodeListAsync(string showName)
        {
            var epList = new EpisodeList();
            var episode = new Episode
            {
                FirstAired = new DateTime(2016, 3, 6),
                Name = "Lisa the Veterinarian",
                Number = 15
            };
            var season = new Season { Episodes = new[] { episode }, Number = 27, ShowName = "The Simpsons" };
            episode.Season = season;

            epList.Seasons = new[] { season };
            return Task.FromResult(epList);
        }
    }

    internal class FakeParser : IFileParser
    {

        public Task<string> GetEpisodeNameAsync(string file, IEpisodeCrawler crawler)
        {
            return Task.FromResult("");
        }
    }
}
