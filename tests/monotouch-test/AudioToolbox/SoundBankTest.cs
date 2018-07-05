//
// SoundBank unit tests
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved
//

#if !__WATCHOS__

using System;
#if XAMCORE_2_0
using Foundation;
using AudioToolbox;
using CoreFoundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;
using MonoTouch.CoreFoundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SoundBankTest {

		const string local_dls = "file:///System/Library/Components/CoreAudio.component/Contents/Resources/gs_instruments.dls";

		[Test]
		public void GetName ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);

			Assert.Throws<ArgumentNullException> (delegate { SoundBank.GetName (null); }, "null");

			using (NSUrl url = new NSUrl ("http://www.xamarin.com")) {
				Assert.Null (SoundBank.GetName (url), "Not a SoundBank");
			}
		}

#if !MONOMAC // No sim on mac
		[Test]
		public void GetName_DLS_SimOnly ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("Use local file system (need a smaller sample)");

			using (NSUrl url = new NSUrl (local_dls)) {
				Assert.That (SoundBank.GetName (url), Is.EqualTo ("QuickTime Music Synthesizer  "), "Name");
			}
		}
#endif

		[Test]
		public void GetInstrumentInfo ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 9, throwIfOtherPlatform: false);

			Assert.Throws<ArgumentNullException> (delegate { SoundBank.GetInstrumentInfo (null); }, "null");

			using (NSUrl url = new NSUrl ("http://www.xamarin.com")) {
				Assert.Null (SoundBank.GetInstrumentInfo (url), "Not a SoundBank");
			}
		}

#if !MONOMAC // No sim on mac
		[Test]
		public void GetInstrumentInfo_DLS_SimOnly ()
		{
			TestRuntime.AssertSystemVersion (PlatformName.iOS, 7, 0, throwIfOtherPlatform: false);
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("Use local file system (need a smaller sample)");

			using (NSUrl url = new NSUrl (local_dls)) {
				var info = SoundBank.GetInstrumentInfo (url);
				Assert.That (info.Length, Is.EqualTo (235), "Length");

				var first = info [0];
				Assert.That (first.Dictionary.Count, Is.EqualTo ((nuint) 4), "first.Count");
				Assert.That (first.Name, Is.EqualTo ("Piano 1     "), "first.name");
				Assert.That (first.MSB, Is.EqualTo (121), "first.MSB");
				Assert.That (first.LSB, Is.EqualTo (0), "first.LSB");
				Assert.That (first.Program, Is.EqualTo (0), "first.program");

				var last = info [234].Dictionary;
				Assert.That (last.Count, Is.EqualTo ((nuint) 4), "last.Count");
				Assert.That (last [InstrumentInfo.NameKey].ToString (), Is.EqualTo ("SFX         "), "last.Name");
				Assert.That ((last [InstrumentInfo.MSBKey] as NSNumber).Int32Value, Is.EqualTo (120), "last.MSB");
				Assert.That ((last [InstrumentInfo.LSBKey] as NSNumber).Int32Value, Is.EqualTo (0), "last.LSB");
				Assert.That ((last [InstrumentInfo.ProgramKey] as NSNumber).Int32Value, Is.EqualTo (56), "last.Program");
			}
		}
#endif
	}
}

#endif // !__WATCHOS__
