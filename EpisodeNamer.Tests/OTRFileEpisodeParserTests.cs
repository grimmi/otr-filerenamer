using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class OtrFileEpisodeParserTests
    {
        [Test]
        [TestCase("some_show__some_episode_(pilot)_16.04.23.mpg.avi", "some episode (pilot)")]
        [TestCase("some_show__some_episode_with_special_chars_16.04.23.mpg.avi", "some episode with special chars")]
        public void GetEpisodeName_FileFormatWithEpisodeName_CorrectEpisodeName(string testFile, string expectedName)
        {
            var parser = new OtrFileEpisodeParser(testFile);
            var parsedEpisodeName = parser.GetEpisodeName();

            Assert.AreEqual(parsedEpisodeName, expectedName);
        }

        [Test]
        [TestCase("some_show_16.04.23.mpg.avi")]
        public void GetEpisodeName_FileFormatWithoutEpisodeName_EmptyString(string testFile)
        {
            var parser = new OtrFileEpisodeParser(testFile);
            var parsedEpisodeName = parser.GetEpisodeName();

            Assert.True(parsedEpisodeName.Length == 0);
        }
    }
}
