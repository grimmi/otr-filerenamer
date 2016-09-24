using System.Threading.Tasks;
using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class SingleFileRenamerTests
    {
        [Test]
        [TestCase(@"c:\temp\some_show__episode_1_(pilot)_16.04.10_somestation.mpg.avi", "some show", "some show 1x1 Episode 1 (Pilot).avi")]
        [TestCase(@"c:\temp\some_show_10.01.01_somestation.mpg.avi", "some show", "some show 1x1 Episode 1 (Pilot).avi")]
        public async Task RenameFile_ValidFile_SuccessfullyRenameFile(string file, string show, string expectedName)
        {
            var crawler = new FakeCrawler();
            var finder = new OtrEpisodeFinder();
            var renamer = new SingleFileRenamer(file, show, crawler, finder);

            var newName = await renamer.RenameFile(@"c:\output");
            var expectedPath = @"c:\output\" + expectedName;

            Assert.AreEqual(expectedPath, newName);
        }
    }
}
