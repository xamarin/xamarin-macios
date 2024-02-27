#if !__WATCHOS__ && !__TVOS__ && !MONOMAC

using System;
using Foundation;
using AVFoundation;
using NUnit.Framework;

namespace monotouchtest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AVCapturePhotoBracketSettingsTest {

		[Test]
		public void TestConstructor ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			var output = new AVCapturePhotoOutput ();
			if (output.AvailablePhotoPixelFormatTypes.Length > 0 && output.MaxBracketedCapturePhotoCount > 0) {
				var array = new AVCaptureAutoExposureBracketedStillImageSettings [Math.Min (3, (int) output.MaxBracketedCapturePhotoCount)];
				for (var i = 0; i < array.Length; i++)
					array [i] = AVCaptureAutoExposureBracketedStillImageSettings.Create (-2f + 2f * i);
				using (var settings = AVCapturePhotoBracketSettings.FromRawPixelFormatType ((uint) output.AvailablePhotoPixelFormatTypes [0], null, array))
					Assert.That (settings.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}
	}
}

#endif
