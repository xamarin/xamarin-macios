using System;
using System.Diagnostics;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Utilities {

	[TestFixture]
	public class StringUtilsTests {

		static readonly char shellQuoteChar =
			(int) Environment.OSVersion.Platform != 128
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
			var p = new Process ();
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.FileName = "/bin/echo";

			var complexInput = "\\\"\"\"'";

			p.StartInfo.Arguments = StringUtils.FormatArguments ("-n", "foo", complexInput, "bar");
			p.Start ();
			var output = p.StandardOutput.ReadToEnd ();
			Assert.AreEqual ($"foo {complexInput} bar", output, "echo");
		}
	}
}
