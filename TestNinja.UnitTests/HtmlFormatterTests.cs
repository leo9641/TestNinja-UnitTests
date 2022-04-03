using NUnit.Framework;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests
{
    [TestFixture]
    public class HtmlFormatterTests
    {
        [Test]
        public void FormatAsBold_WhenCalled_ShouldEncloseTheStringWithStrongElement()
        {
            var formatter = new HtmlFormatter();

            var result = formatter.FormatAsBold("abc");

            // Specific - Will break if we change error message in future
            StringAssert.AreEqualIgnoringCase(result, "<strong>abc</strong>");

            // More General
            Assert.That(result.StartsWith("<strong>"));
            Assert.That(result.EndsWith("</strong>"));
            Assert.That(result.Contains("abc"));
        }
    }
}
