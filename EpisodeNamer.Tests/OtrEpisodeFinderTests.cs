﻿using System.Threading.Tasks;
using NUnit.Framework;
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
            var parser = GetFileParser();

            var episodeFinder = new OtrEpisodeFinder();
            var episodeInfo = await episodeFinder.GetEpisodeAsync(file, show, crawler);

            Assert.AreEqual(expectedEpisode, episodeInfo.Episode.Name);
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

    internal class FakeParser : IFileParser
    {

        public Task<string> GetEpisodeNameAsync(string file, IEpisodeCrawler crawler)
        {
            return Task.FromResult("");
        }
    }
}