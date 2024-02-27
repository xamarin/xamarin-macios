using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace monotouchtest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSStringTest {
		[Test]
		public void LocalizedFormatTest ()
		{
			// Strings and NSstring
			Assert.AreEqual ("hello", NSString.LocalizedFormat ("hello").ToString ());
			Assert.AreEqual ("hello", NSString.LocalizedFormat (new NSString ("hello")).ToString ());

			// Test the overloads with numbers
			Assert.AreEqual ("hello", NSString.LocalizedFormat ("hello").ToString ());
			Assert.AreEqual ("hello0", NSString.LocalizedFormat ("hello%@", 0).ToString ());
			Assert.AreEqual ("hello01", NSString.LocalizedFormat ("hello%@%@", 0, 1).ToString ());
			Assert.AreEqual ("hello012", NSString.LocalizedFormat ("hello%@%@%@", 0, 1, 2).ToString ());
			Assert.AreEqual ("hello0123", NSString.LocalizedFormat ("hello%@%@%@%@", 0, 1, 2, 3).ToString ());
			Assert.AreEqual ("hello01234", NSString.LocalizedFormat ("hello%@%@%@%@%@", 0, 1, 2, 3, 4).ToString ());
			Assert.AreEqual ("hello012345", NSString.LocalizedFormat ("hello%@%@%@%@%@%@", 0, 1, 2, 3, 4, 5).ToString ());
			Assert.AreEqual ("hello0123456", NSString.LocalizedFormat ("hello%@%@%@%@%@%@%@", 0, 1, 2, 3, 4, 5, 6).ToString ());
			Assert.AreEqual ("hello01234567", NSString.LocalizedFormat ("hello%@%@%@%@%@%@%@%@", 0, 1, 2, 3, 4, 5, 6, 7).ToString ());
			Assert.AreEqual ("hello012345678", NSString.LocalizedFormat ("hello%@%@%@%@%@%@%@%@%@", 0, 1, 2, 3, 4, 5, 6, 7, 8).ToString ());
		}

		[TestCase ("asdf", -1, 0, "start")]
		[TestCase ("asdf", 0, -1, "length")]
		[TestCase ("asdf", 5, 0, "start")]
		[TestCase ("asdf", 0, 5, "length")]
		public void NSStringSubstringExceptions (string input, int start, int length, string paramName)
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException> (() => new NSString (input, start, length));

			Assert.AreEqual (paramName, exception.ParamName);

			exception = Assert.Throws<ArgumentOutOfRangeException> (() => NSString.CreateNative (input, start, length));

			Assert.AreEqual (paramName, exception.ParamName);
		}

		[TestCase ("asdf", 0, 4)] // Whole string
		[TestCase ("asdf", 0, 2)] // Substring length
		[TestCase ("asdf", 1, 3)] // Substring offset and length
		[TestCase ("asdf", 4, 0)] // Empty string
		public void TestNSStringSubstrings (string input, int start, int length)
		{
			var str = new NSString (input.Substring (start, length));
			var substring = new NSString (input, start, length);
			var substringHandle = NSString.CreateNative (input, start, length);
			try {
				Assert.AreEqual (str, substring);

				substring = (NSString) NSString.FromHandle (substringHandle);
				Assert.AreEqual (str, substring);
			} finally {
				NSString.ReleaseNative (substringHandle);
			}
		}

		[TestCase (false)]
		[TestCase (true)]
		public void TestFromHandle_owns (bool owns)
		{
			const string testString = "a random and long string that should not be cached by the OS";
			using var str = new NSString (testString);
			for (var i = 0; i < 100; i++) {
				if (owns)
					str.DangerousRetain ();
				Assert.AreEqual (testString, NSString.FromHandle (str.Handle, owns), $"true #{i}");
			}
			// If there was a leak, RetainCount would be 100+ because we looped 100 times above.
			Assert.That (str.RetainCount, Is.LessThan ((nuint) 10), "RetainCount");
		}
	}
}
