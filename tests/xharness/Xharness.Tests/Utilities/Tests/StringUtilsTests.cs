using System;
using NUnit.Framework;
using Xharness.Utilities;

namespace Xharness.Tests.Utilities.Tests {

	[TestFixture]
	public class StringUtilsTests {

		static readonly char shellQuoteChar =
			(int)Environment.OSVersion.Platform != 128
				&& Environment.OSVersion.Platform != PlatformID.Unix
				&& Environment.OSVersion.Platform != PlatformID.MacOSX
			? '"'   // Windows
			: '\''; // !Windows

		[Test]
		public void NoEscapingNeeded ()
		{
			Assert.AreEqual ("foo", StringUtils.Quote ("foo"));
		}

		[TestCase ("foo bar", "foo bar", Description = "Space")]
		[TestCase ("foo \"bar\"", "foo \\\"bar\\\"", Description = "Quotes")]
		[TestCase ("foo bar's", "foo bar\\\'s", Description = "Apostrophe")]
		[TestCase ("foo $bar's", "foo $bar\\\'s", Description = "Dollar sign")]
		public void QuoteForProcessTest (string input, string expected)
		{
			Assert.AreEqual (shellQuoteChar + expected + shellQuoteChar, StringUtils.Quote (input));
		}
		
		[Test]
		public void FormatArgumentsTest ()
		{
			var arguments = new [] {
				"foo $bar's",
				"x\"yz"
			};
			string expected = "\"foo $bar's\" \"x\\\"\"\"yz\"";

			Assert.AreEqual (expected, StringUtils.FormatArguments (arguments));
		}
	}
}
