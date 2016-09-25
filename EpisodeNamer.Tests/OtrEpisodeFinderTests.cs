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

        private IFileParser GetFileParser()
        {
            return new FakeParser();
        }

        private IEpisodeCrawler GetShowCrawler()
        {
            return new FakeCrawler();
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
