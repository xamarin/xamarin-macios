using System;
using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif

namespace Introspection {

	[TestFixture]
	public class iOSApiTypoTest : ApiTypoTest {
#if !__WATCHOS__
		UITextChecker checker = new UITextChecker ();
#endif

		[SetUp]
		public void SetUp ()
		{
#if __WATCHOS__
			Assert.Ignore ("Need to find alternative for UITextChecker on WatchOS.");
#else
			// the dictionary used by iOS varies with versions and 
			// we don't want to maintain special cases for each version
			var sdk = new Version (Constants.SdkVersion);
			if (!UIDevice.CurrentDevice.CheckSystemVersion (sdk.Major, sdk.Minor))
				Assert.Ignore ("Typos only verified using the latest SDK");
#endif
		}

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
			// that's slow and there's no value to run it on devices as the API names
			// being verified won't change from the simulator
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("Typos only detected on simulator");

			base.TypoTest ();
		}
	}
}
