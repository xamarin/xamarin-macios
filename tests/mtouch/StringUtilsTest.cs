using System;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests.Utils {

	[TestFixture]
	public class StringUtilsTest {

		[Test]
		public void ParseVersion ()
		{
			// just like Version.Parse
			var v = StringUtils.ParseVersion ("11.0");
			Assert.That (v.ToString (), Is.EqualTo ("11.0"), "11.0");
			// unless only the major part is provided
			v = StringUtils.ParseVersion ("6");
			Assert.That (v.ToString (), Is.EqualTo ("6.0"), "6.0");
			// with major and minor
			v = StringUtils.ParseVersion ("7.1");
			Assert.That (v.ToString (), Is.EqualTo ("7.1"), "7.1");
			// with major and minor
			v = StringUtils.ParseVersion ("7.1");
			Assert.That (v.ToString (), Is.EqualTo ("7.1"), "7.1");
			// with major, minor and build
			v = StringUtils.ParseVersion ("10.13.2");
			Assert.That (v.ToString (), Is.EqualTo ("10.13.2"), "10.13.2");
		}

		[Test]
		public void QuoteForProcess ()
		{
			Assert.That (StringUtils.QuoteForProcess ("a"), Is.EqualTo ("a"), "normal");
			Assert.That (StringUtils.QuoteForProcess ("😁"), Is.EqualTo ("😁"), "😁");
			Assert.That (StringUtils.QuoteForProcess ("b b"), Is.EqualTo ("\"b b\""), "space");
			Assert.That (StringUtils.QuoteForProcess ("'"), Is.EqualTo ("\"'\""), "single quote");
			Assert.That (StringUtils.QuoteForProcess ("\\"), Is.EqualTo ("\"\\\\\""), "backslash");
			Assert.That (StringUtils.QuoteForProcess ("\""), Is.EqualTo ("\"\\\"\""), "double quote");
			Assert.That (StringUtils.QuoteForProcess (@"C:\double "" quote\single ' quote\space here\"), Is.EqualTo (@"""C:\\double \"" quote\\single ' quote\\space here\\"""), "windows path");
		}
	}
}
