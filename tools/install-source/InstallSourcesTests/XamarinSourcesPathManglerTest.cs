using System;
using NUnit.Framework;

using InstallSources;

namespace InstallSourcesTests
{
	[TestFixture]
	public class XamarinSourcesPathManglerTest
	{
		string frameworkPath;
		string xamarinSourcePath;
		string installDir;
		string destinationDir;
		XamarinSourcesPathMangler mangler;

		[SetUp]
		public void SetUp ()
		{
			frameworkPath = "Xamarin.iOS.framework";
			xamarinSourcePath = "/Users/test/xamarin-macios/src/";
			installDir = "/Library/Frameworks/Xamarin.iOS.framework/Versions/4.1.0.402";
			destinationDir = "/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git";

			mangler = new XamarinSourcesPathMangler {
				FrameworkPath = frameworkPath,
				XamarinSourcePath = xamarinSourcePath,
				InstallDir = installDir,
				DestinationDir = destinationDir,
			};
		}

		[TestCase ("/Library/Frameworks/Xamarin.iOS.framework/Versions/4.1.0.402/src/Xamarin.iOS/CoreData/NSEntityMapping.g.cs",
			"/Users/test/xamarin-macios/src/build/ios/native/CoreData/NSEntityMapping.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/build/ios/native/AVFoundation/AVMutableTimedMetadataGroup.g.cs",
		    "/Users/test/xamarin-macios/src/build/ios/native/AVFoundation/AVMutableTimedMetadataGroup.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/build/ios/native/CloudKit/CKRecordZoneNotification.g.cs",
		    "/Users/test/xamarin-macios/src/build/ios/native/CloudKit/CKRecordZoneNotification.g.cs")]
		public void TestGetSourcePathGeneratediOSCode (string path, string expectedPath)
		{
			var result = mangler.GetSourcePath (path);
			Assert.IsTrue (result.Contains ("/build/"), "Path does not contain '/build/'");
			Assert.IsTrue (result.StartsWith(xamarinSourcePath, StringComparison.InvariantCulture), "Path does not start with the XamarinPath '{0}'", xamarinSourcePath);
			Assert.AreEqual (result, expectedPath);
		}

		[TestCase ("/Library/Frameworks/Xamarin.Mac.framework/Versions/4.1.0.402/src/Xamarin.iOS/CoreData/NSEntityMapping.g.cs",
			"/Users/test/xamarin-macios/src/build/mac/full/CoreData/NSEntityMapping.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/src/Xamarin.Mac/build/mac/full/AVFoundation/AVMutableTimedMetadataGroup.g.cs",
			"/Users/test/xamarin-macios/src/build/mac/full/AVFoundation/AVMutableTimedMetadataGroup.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/src/Xamarin.Mac/build/mac/full/CloudKit/CKRecordZoneNotification.g.cs",
			"/Users/test/xamarin-macios/src/build/mac/full/CloudKit/CKRecordZoneNotification.g.cs")]
		public void TestGetSourcePathGeneratedMacCode (string path, string expectedPath)
		{
			mangler.FrameworkPath = "Xamarin.Mac.framework"; // dealing with mac sources
			mangler.InstallDir = "/Library/Frameworks/Xamarin.Mac.framework/Versions/4.1.0.402/";
			var result = mangler.GetSourcePath (path);
			Assert.IsTrue (result.Contains ("/build/"), "Path does not contain '/build/'");
			Assert.IsTrue (result.StartsWith (xamarinSourcePath, StringComparison.InvariantCulture), "Path does not start with the XamarinPath '{0}'", xamarinSourcePath);
			Assert.AreEqual (expectedPath, result);
		}

		[TestCase ("/Users/test/xamarin-macios/runtime/Delegates.generated.cs", "/Users/test/xamarin-macios/runtime/Delegates.generated.cs")]
		public void TestGetSourceRuntimeCode (string path, string expectedPath)
		{
			var result = mangler.GetSourcePath (path);
			Assert.AreEqual (result, expectedPath);
		}

		[TestCase ("/Library/Frameworks/Xamarin.iOS.framework/Versions/4.1.0.402/src/Xamarin.iOS/AVFoundation/AVCaptureDeviceInput.cs",
			"/Users/test/xamarin-macios/src/AVFoundation/AVCaptureDeviceInput.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/CoreImage/CIImage.cs",
			"/Users/test/xamarin-macios/src/CoreImage/CIImage.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/CoreData/NSAttributeDescription.cs",
			"/Users/test/xamarin-macios/src/CoreData/NSAttributeDescription.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/CoreData/NSAttributeDescription.cs",
			"/Users/test/xamarin-macios/src/CoreData/NSAttributeDescription.cs")]
		public void TestGetSourcePathManualCode (string path, string expectedPath)
		{
			var result = mangler.GetSourcePath(path);
			Assert.IsFalse (result.Contains ("/build/"), "Path contains '/build/' when it is a manual path.");
			Assert.IsTrue (result.StartsWith (xamarinSourcePath, StringComparison.InvariantCulture), "Path does not start with the XamarinPath '{0}'", xamarinSourcePath);
			Assert.AreEqual (result, expectedPath);
		}

		[TestCase ("/Library/Frameworks/Xamarin.iOS.framework/Versions/4.1.0.402/src/Xamarin.iOS/NativeTypes/NMath.cs",
		           "/Users/test/xamarin-macios/src/NativeTypes/NMath.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/build/common/NativeTypes/Drawing.cs",
		           "/Users/test/xamarin-macios/src/build/common/NativeTypes/Drawing.cs")]
		[TestCase ("/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/build/common/NativeTypes/Primitives.cs",
		           "/Users/test/xamarin-macios/src/build/common/NativeTypes/Primitives.cs")]
		public void TestGetSourcePathNativeTypes (string path, string expectedPath)
		{
			var result = mangler.GetSourcePath(path);
			Assert.IsTrue(result.StartsWith(xamarinSourcePath, StringComparison.InvariantCulture), "Path does not start with the XamarinPath '{0}'", xamarinSourcePath);
			Assert.AreEqual (result, expectedPath);
		}

		[TestCase ("/Users/test/xamarin-macios/src/build/ios/native/AVFoundation/AVMutableMetadataItem.g.cs", "/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/AVFoundation/AVMutableMetadataItem.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/AVFoundation/AVCaptureDeviceInput.cs", "/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/AVFoundation/AVCaptureDeviceInput.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/NativeTypes/NMath.cs", "/Users/test/xamarin-macios/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/src/Xamarin.iOS/NativeTypes/NMath.cs")]
		public void TestGetTargetPathiOS (string src, string expectedTarget)
		{
			var target = mangler.GetTargetPath (src);

			Assert.IsTrue (target.StartsWith (destinationDir, StringComparison.InvariantCulture), "Does not contain the install dir.");
			Assert.AreEqual (expectedTarget, target, "Target is not the expected one.");
		}

		[TestCase ("/Users/test/xamarin-macios/src/build/mac/full/AVFoundation/AVMutableMetadataItem.g.cs", "/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/src/Xamarin.Mac/AVFoundation/AVMutableMetadataItem.g.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/AVFoundation/AVCaptureDeviceInput.cs", "/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/src/Xamarin.Mac/AVFoundation/AVCaptureDeviceInput.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/NativeTypes/NMath.cs", "/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/src/Xamarin.Mac/NativeTypes/NMath.cs")]
		public void TestGetTargetPathMac (string src, string expectedTarget)
		{
			mangler.FrameworkPath = "Xamarin.Mac.framework"; // dealing with mac sources
			mangler.InstallDir = "/Library/Frameworks/Xamarin.Mac.framework";
			mangler.DestinationDir = "/Users/test/xamarin-macios/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git";

			var target = mangler.GetTargetPath (src);

			Assert.IsTrue (target.StartsWith (mangler.DestinationDir, StringComparison.InvariantCulture), "Does not contain the install dir.");
			Assert.AreEqual (expectedTarget, target, "Target is not the expected one.");
		}

		[TestCase ("/Library/Frameworks/Xamarin.iOS.framework/Versions/4.1.0.402/src/Xamarin.iOS/Delegates.generated.cs",
			"/Users/test/xamarin-macios/runtime/Delegates.generated.cs")]
		[TestCase ("/Users/test/xamarin-macios/src/Xamarin.iOS/runtime/Delegates.generated.cs",
			"/Users/test/xamarin-macios/src/Xamarin.iOS/runtime/Delegates.generated.cs")]
		public void TestGetSourcePathRuntime (string path, string expectedPath)
		{
			var result = mangler.GetSourcePath (path);
			Assert.AreEqual (expectedPath, result);
		}

	}
}
