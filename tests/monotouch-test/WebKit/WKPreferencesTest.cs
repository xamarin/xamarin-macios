#if __IOS__ || MONOMAC

using System;
using System.IO;
using NUnit.Framework;

using Foundation;
using WebKit;

namespace MonoTouchFixtures.WebKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class WKPreferencesTest {
		[Test]
		public void TextInteractionEnabledTest ()
		{
			TestRuntime.AssertXcodeVersion (13, 1);
			using var preferences = new WKPreferences ();
			// ignore the OS version, the property should always work
			Assert.DoesNotThrow (() => preferences.TextInteractionEnabled = true, "Getter");
			Assert.DoesNotThrow (() => {
				var enabled = preferences.TextInteractionEnabled;
			}, "Setter");
		}
	}
}
#endif
