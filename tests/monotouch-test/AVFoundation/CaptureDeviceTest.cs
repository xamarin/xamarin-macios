#if __IOS__
using Foundation;
using AVFoundation;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.AVFoundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CaptureDeviceTest {

#if !NET
		void Compare (NSString constant, AVMediaTypes value)
		{
			Assert.That (AVCaptureDevice.GetDefaultDevice (constant), Is.EqualTo (AVCaptureDevice.GetDefaultDevice (value)), value.ToString ());
			Assert.That (AVCaptureDevice.GetDefaultDevice (constant), Is.EqualTo (AVCaptureDevice.DefaultDeviceWithMediaType ((string) constant)), value.ToString () + ".compat");
		}

		[Test]
		public void CompareConstantEnum ()
		{
			TestRuntime.RequestCameraPermission (AVMediaType.Audio, true);
			TestRuntime.RequestCameraPermission (AVMediaType.Video, true);

			Compare (AVMediaType.Audio, AVMediaTypes.Audio);
			Compare (AVMediaType.ClosedCaption, AVMediaTypes.ClosedCaption);
			Compare (AVMediaType.Metadata, AVMediaTypes.Metadata);
			Compare (AVMediaType.Muxed, AVMediaTypes.Muxed);
			Compare (AVMediaType.Subtitle, AVMediaTypes.Subtitle);
			Compare (AVMediaType.Text, AVMediaTypes.Text);
			Compare (AVMediaType.Timecode, AVMediaTypes.Timecode);
			Compare (AVMediaType.Video, AVMediaTypes.Video);

			if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 9,0))
				Compare (AVMediaType.MetadataObject, AVMediaTypes.MetadataObject);

			// obsoleted in iOS 6, removed in iOS12
#if !__MACCATALYST__
			if (TestRuntime.CheckSystemVersion (ApplePlatform.iOS, 12, 0))
				Assert.Null (AVMediaType.TimedMetadata, "AVMediaTypeTimedMetadata");
			else
				Compare (AVMediaType.TimedMetadata, AVMediaTypes.TimedMetadata);
#endif
		}
#endif // !NET
	}
}
#endif
