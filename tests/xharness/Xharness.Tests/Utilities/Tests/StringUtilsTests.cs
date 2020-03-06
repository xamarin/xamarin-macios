using NUnit.Framework;
using Xharness.Utilities;

namespace Xharness.Tests.Utilities.Tests {

	[TestFixture]
	public class StringUtilsTests {
		
		[TestCase ("foo", "foo", Description = "No escaping")]
		[TestCase ("foo bar", "foo\\ bar", Description = "Space")]
		public void QuoteForProcessTest (string input, string expected)
		{
			Assert.AreEqual (expected, StringUtils.Quote (input));
		}
	}
}
