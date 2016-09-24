using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class OtrShowNameParserTests
    {
        [Test]
        [TestCase(@"c:\temp\Modern_Family_16.01.06_21-00_uswabc_31_TVOON_DE.mpg.HQ.avi", "Modern Family")]
        public void GetShowName_OtrFileNameWithoutEpisodeName_CorrectShowName(string file, string expectedName)
        {
            var showParser = new OtrShowNameParser();
            var parsedShowName = showParser.GetShowName(file);

            Assert.AreEqual(expectedName, parsedShowName);
        }

        [Test]
        [TestCase(@"\\nas\Shared\Downloads\Bob_s_Burgers__Bye_Bye_Boo_Boo_16.05.08_20-30_uswnyw_30_TVOON_DE.mpg.avi", "Bob s Burgers")]
        public void GetShowName_OtFileNameWithEpisodeName_CorrectShowName(string file, string expectedName)
        {
            var showParser = new OtrShowNameParser();
            var parsedShowName = showParser.GetShowName(file);

            Assert.AreEqual(expectedName, parsedShowName);
        }
    }
}
