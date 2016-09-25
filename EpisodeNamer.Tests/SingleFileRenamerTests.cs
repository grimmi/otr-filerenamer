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

        [Test]
        [TestCase("The_Simpsons_16.03.06_20-00_uswnyw_30_TVOON_DE.mpg.avi", 15, 27, "Lisa the Veterinarian")]
        public async Task RenameFile_SimpsonsEpisode_CorrectSeasonAndEpisodeNumber(string file, int epNr, int sNr, string epName)
        {
            var crawler = new SimpsonsCrawler();
            var finder = new OtrEpisodeFinder();
            var renamer = new SingleFileRenamer(file, "The Simpsons", crawler, finder);

            var newName = await renamer.RenameFile("");
            var expectedPath = "The Simpsons " + sNr + "x" + epNr + " " + epName + ".avi";

            Assert.AreEqual(expectedPath, newName);
        }
    }
}
