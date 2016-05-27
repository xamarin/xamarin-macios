using System;
#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.Security;
#endif
using NUnit.Framework;

namespace monotouchtest
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSStringTest
	{
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
	}
}

