using System;
using NUnit.Framework;

namespace EpisodeNamer.Tests
{
    [TestFixture]
    public class OtrFileDateParserTests
    {
        [Test]
        [TestCase("Modern_Family_16.01.13_21-00_uswabc_31_TVOON_DE.mpg.avi", 2016, 1, 13)]
        public void GetDateOfFile_ValidFileName_CorrectDate(string fileName, int y, int m, int d)
        {
            var parser = new OtrFileDateParser();
            var date = parser.GetDateOfFile(fileName);

            var expectedDate = new DateTime(y, m, d);
            Assert.AreEqual(expectedDate, date);
        }
    }
}
