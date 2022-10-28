using System;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;
using UIKit;

namespace Introspection {

	[TestFixture]
	public class iOSApiTypoTest : ApiTypoTest {
#if !__WATCHOS__
		UITextChecker checker = new UITextChecker ();
#endif

		public override string GetTypo (string txt)
		{
#if __WATCHOS__
			return string.Empty;
#else
			var checkRange = new NSRange (0, txt.Length);
			var typoRange = checker.RangeOfMisspelledWordInString (txt, checkRange, checkRange.Location, false, "en_US");
			if (typoRange.Length == 0)
				return String.Empty;
			return txt.Substring ((int) typoRange.Location, (int) typoRange.Length);
#endif
		}

		public override void TypoTest ()
		{
#if __WATCHOS__
			Assert.Ignore ("Need to find alternative for UITextChecker on WatchOS.");
#else
			// the dictionary used by iOS varies with versions and
			// we don't want to maintain special cases for each version
			var sdk = new Version (Constants.SdkVersion);
			if (!UIDevice.CurrentDevice.CheckSystemVersion (sdk.Major, sdk.Minor))
				Assert.Ignore ("Typos only verified using the latest SDK");

			// that's slow and there's no value to run it on devices as the API names
			// being verified won't change from the simulator
			TestRuntime.AssertSimulatorOrDesktop ("Typos only detected on simulator");

			base.TypoTest ();
#endif
		}
	}
}
