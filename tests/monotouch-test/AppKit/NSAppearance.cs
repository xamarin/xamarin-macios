#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSAppearanceTests {
		[Test]
		public void NSAppearanceShouldLoadAppearanceNamed ()
		{
			Asserts.EnsureYosemite ();

			var appearance = NSAppearance.GetAppearance (NSAppearance.NameVibrantDark);
			Assert.IsNotNull (appearance, "NSAppearanceShouldLoadAppearanceNamed - Failed to initialize appearance VibrantDark");
			Assert.AreEqual (appearance.Name, NSAppearance.NameVibrantDark.ToString (), "NSAppearanceShouldLoadAppearanceNamed - Appearance initialized with incorrect name.");
		}

#if FALSE // Test failing, exception doesn't appear to be thrown during test, throw correctly running in an app.
		[Test]
		public void NSAppearanceConstructorShouldFailWithInvalidName ()
		{
			bool exceptionHit = false;

			try {
				var appearance = new NSAppearance ("InvalidNameTest", null);
			} catch (ArgumentException e) {
				exceptionHit = true;
			}

			Assert.IsTrue (exceptionHit, "NSAppearanceConstructorShouldFailWithInvalidName - No exception thrown while initializing appearance with invalid name.");
		}
#endif

		[Test]
		public void NSAppearanceShouldChangeCurrentAppearance ()
		{
			Asserts.EnsureYosemite ();

			var appearance = NSAppearance.CurrentAppearance;

			NSAppearance.CurrentAppearance = NSAppearance.GetAppearance (NSAppearance.NameVibrantDark);

			Assert.AreNotEqual (appearance, NSAppearance.CurrentAppearance, "NSAppearanceShouldChangeCurrentAppearance - Failed to change appearance.");
		}

		[Test]
		public void NSAppearanceCustomizationNull ()
		{
			Asserts.EnsureYosemite ();

			using (NSButton b = new NSButton ()) {
#if NET
				b.Appearance = null;
#else
				b.SetAppearance (null);
#endif
			}
		}
	}
}
#endif // __MACOS__
