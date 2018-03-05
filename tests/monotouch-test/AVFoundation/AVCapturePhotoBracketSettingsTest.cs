#if XAMCORE_2_0 && !__WATCHOS__ && !__TVOS__ && !MONOMAC

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
			AVCaptureAutoExposureBracketedStillImageSettings [] array = new AVCaptureAutoExposureBracketedStillImageSettings [3];
			array [0] = AVCaptureAutoExposureBracketedStillImageSettings.Create (-2f);
			array [1] = AVCaptureAutoExposureBracketedStillImageSettings.Create (0f);
			array [2] = AVCaptureAutoExposureBracketedStillImageSettings.Create (2f);
			var output = new AVCapturePhotoOutput ();
			if (output.AvailablePhotoPixelFormatTypes.Length > 0) {
				using (var settings = AVCapturePhotoBracketSettings.FromRawPixelFormatType ((uint) output.AvailablePhotoPixelFormatTypes [0], null, array))
					Assert.That (settings.Handle, Is.Not.EqualTo (IntPtr.Zero));
			}
		}
	}
}

#endif 
