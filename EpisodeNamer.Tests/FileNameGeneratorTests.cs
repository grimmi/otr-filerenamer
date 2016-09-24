using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class FileNameGeneratorTests
    {
        [Test]
        public async Task GenerateFileName_ValidFileInfo_SensibleFileName()
        {
            var generator = new FileNameGenerator();
            var fakeCrawler = new FakeCrawler();
            var episodes = await fakeCrawler.DownloadEpisodeListAsync("some show");
            var epFile = new EpisodeFile { Episode = episodes.Seasons.First().Episodes.First(), File = @"c:\temp\somefile.avi" };

            var generatedName = generator.GenerateFileName(epFile);

            Assert.AreEqual("some show 1x1 Episode 1 (Pilot).avi", generatedName);
        }
    }
}
