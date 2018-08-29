#if __IOS__
using System;
#if XAMCORE_2_0
using Foundation;
using AVFoundation;
using ObjCRuntime;
#else
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;
namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptureDeviceTest {

		void Compare (NSString constant, AVMediaTypes value)
		{
			Assert.That (AVCaptureDevice.GetDefaultDevice (constant), Is.EqualTo (AVCaptureDevice.GetDefaultDevice (value)), value.ToString ());
#if !XAMCORE_4_0
			Assert.That (AVCaptureDevice.GetDefaultDevice (constant), Is.EqualTo (AVCaptureDevice.DefaultDeviceWithMediaType ((string) constant)), value.ToString () + ".compat");
#endif
		}

		[Test]
		public void CompareConstantEnum ()
		{
			Compare (AVMediaType.Audio, AVMediaTypes.Audio);
			Compare (AVMediaType.ClosedCaption, AVMediaTypes.ClosedCaption);
			Compare (AVMediaType.Metadata, AVMediaTypes.Metadata);
			Compare (AVMediaType.Muxed, AVMediaTypes.Muxed);
			Compare (AVMediaType.Subtitle, AVMediaTypes.Subtitle);
			Compare (AVMediaType.Text, AVMediaTypes.Text);
			Compare (AVMediaType.Timecode, AVMediaTypes.Timecode);
			Compare (AVMediaType.Video, AVMediaTypes.Video);

			if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 9,0))
				Compare (AVMediaType.MetadataObject, AVMediaTypes.MetadataObject);

			// obsoleted in iOS 6, removed in iOS12
			if (TestRuntime.CheckSystemVersion (PlatformName.iOS, 12,0))
				Assert.Null (AVMediaType.TimedMetadata, "AVMediaTypeTimedMetadata");
			else
				Compare (AVMediaType.TimedMetadata, AVMediaTypes.TimedMetadata);
		}
	}
}
#endif
