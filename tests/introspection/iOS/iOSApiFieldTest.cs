//
// Test the generated API fields (e.g. against typos or OSX-only values)
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using Foundation;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiFieldTest : ApiFieldTest {

		public iOSApiFieldTest ()
		{
			ContinueOnFailure = true;
			//LogProgress = true;
		}

		protected override bool Skip (Type type)
		{
			return base.Skip (type);
		}

		protected override bool Skip (PropertyInfo p)
		{
			switch (p.DeclaringType.Namespace) {
			case "CoreAudioKit":
			case "MonoTouch.CoreAudioKit":
			case "Metal":
			case "MonoTouch.Metal":
				// they works with iOS9 beta 4 (but won't work on older simulators)
				if (TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (7, 0))
					return true;
				break;
			case "MetalKit":
			case "MonoTouch.MetalKit":
			case "MetalPerformanceShaders":
			case "MonoTouch.MetalPerformanceShaders":
			case "CoreNFC": // Only available on devices that support NFC, so check if NFCNDEFReaderSession is present.
				if (Class.GetHandle ("NFCNDEFReaderSession") == IntPtr.Zero)
					return true;
				break;
#if __TVOS__
			case "MetalPerformanceShadersGraph":
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
#endif // __TVOS__
			case "Phase":
			case "DeviceCheck": // Only available on device
				if (TestRuntime.IsSimulatorOrDesktop)
					return true;
				break;
			case "IOSurface":
				// Available in the simulator starting with iOS 11
				return TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (9, 0);
			case "iAd":
				// largely removed in xcode 13, including ADClient.ErrorDomain
				// since using this code leads to rejections it's totally removed (so no version check)
				return true;
			case "NewsstandKit":
				// largely removed in xcode 15
				return true;
			}

			switch (p.Name) {
			case "AutoConfigurationHTTPResponseKey":        // kCFProxyAutoConfigurationHTTPResponseKey
			case "CFNetworkProxiesProxyAutoConfigJavaScript":   // kCFNetworkProxiesProxyAutoConfigJavaScript
				return true;

			// defined in Apple PDF (online) but not in the HTML documentation
			// but also inside CLError.h from iOS 5.1 SDK...
			case "ErrorUserInfoAlternateRegionKey":         // kCLErrorUserInfoAlternateRegionKey
				return true;

			// ImageIO: documented since iOS 4.3 but null in iOS5 (works on iOS 6.1)
			// https://developer.apple.com/library/ios/releasenotes/General/iOS43APIDiffs/
			case "ExifCameraOwnerName":
			case "ExifBodySerialNumber":
			case "ExifLensSpecification":
			case "ExifLensMake":
			case "ExifLensModel":
			case "ExifLensSerialNumber":
				return !TestRuntime.CheckXcodeVersion (4, 6);

			// ImageIO: new in iOS 8 but returns nil (at least in beta 1) seems fixed in iOS9
			case "PNGLoopCount":
			case "PNGDelayTime":
			case "PNGUnclampedDelayTime":
				return !TestRuntime.CheckXcodeVersion (7, 0);

			// CoreServices.CFHTTPMessage - document in 10.9 but returns null
			case "_AuthenticationSchemeOAuth1":
				return true;

			// Apple does not ship a PushKit for every arch on some devices :(
			case "Voip":
				return TestRuntime.IsDevice;
			// Just available on device
			case "UsageKey":
				return TestRuntime.IsSimulatorOrDesktop;
			// Xcode 12.2 Beta 1 does not ship this but it is available in Xcode 12.0...
			case "BarometricPressure":
				return true;
			default:
				return base.Skip (p);
			}
		}

		protected override bool Skip (string constantName, string libraryName)
		{
			switch (libraryName) {
			case "CoreNFC": // Only available on devices that support NFC, so check if NFCNDEFReaderSession is present.
				if (Class.GetHandle ("NFCNDEFReaderSession") == IntPtr.Zero)
					return true;
				break;
			case "IOSurface":
				return TestRuntime.IsSimulatorOrDesktop && !TestRuntime.CheckXcodeVersion (9, 0);
			}

			switch (constantName) {
			// grep ImageIO binary shows those symbols are not part of the binary
			// that match older results (nil) when loading them (see above)
			case "kCGImagePropertyAPNGLoopCount":
			case "kCGImagePropertyAPNGDelayTime":
			case "kCGImagePropertyAPNGUnclampedDelayTime":
			case "kCGImagePropertyMakerFujiDictionary":
			case "kCGImagePropertyMakerMinoltaDictionary":
			case "kCGImagePropertyMakerOlympusDictionary":
			case "kCGImagePropertyMakerPentaxDictionary":
			// 
			case "kCFHTTPAuthenticationSchemeOAuth1":
				return true;
			// Apple does not ship a PushKit for every arch on some devices :(
			case "PKPushTypeVoIP":
				return TestRuntime.IsDevice;
			// there's only partial support for metal on the simulator (on iOS9 beta 5) but most other frameworks
			// that interop with it are not (yet) supported
			case "kCVMetalTextureCacheMaximumTextureAgeKey":
			case "kCVMetalTextureUsage":
			case "MPSRectNoClip":
			case "MTLCommandBufferErrorDomain":
			case "MTKTextureLoaderErrorDomain":
			case "MTKTextureLoaderErrorKey":
			case "MTKTextureLoaderOptionAllocateMipmaps":
			case "MTKTextureLoaderOptionSRGB":
			case "MTKTextureLoaderOptionTextureUsage":
			case "MTKTextureLoaderOptionTextureCPUCacheMode":
			case "MTKModelErrorDomain":
			case "MTKModelErrorKey":
				return TestRuntime.IsSimulatorOrDesktop;
			// Xcode 12.2 Beta 1 does not ship this but it is available in Xcode 12.0...
			case "HKMetadataKeyBarometricPressure":
				return true;
#if __WATCHOS__
			case "AVCaptureLensPositionCurrent": // looks like this was bound by mistake in watchOS
				return true;
#endif
			default:
				return false;
			}
		}
	}
}
