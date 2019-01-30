//
// Test the existing of p/invoked symbols
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiPInvokeTest : ApiPInvokeTest {

		protected override bool Skip (string symbolName)
		{
			bool simulator = Runtime.Arch == Arch.SIMULATOR;
			switch (symbolName) {
			// Metal support inside simulator is only available in recent iOS9 SDK
#if !__WATCHOS__
			case "MTLCreateSystemDefaultDevice":
				return simulator && !UIDevice.CurrentDevice.CheckSystemVersion (9, 0);
#endif
			// still most Metal helpers are not available on the simulator (even when the framework is present, it's missing symbols)
			case "MPSSupportsMTLDevice":
			// neither are the CoreVideo extensions for Metal
			case "CVMetalTextureGetTexture":
			case "CVMetalTextureIsFlipped":
			case "CVMetalTextureGetCleanTexCoords":
			case "CVMetalTextureCacheCreate":
			case "CVMetalTextureCacheFlush":
			case "CVMetalTextureCacheCreateTextureFromImage":
			case "MTKMetalVertexDescriptorFromModelIO":
			case "MTKModelIOVertexDescriptorFromMetal":
			case "MTKModelIOVertexFormatFromMetal":
			case "MTKMetalVertexFormatFromModelIO":
			case "CVPixelBufferGetIOSurface":
			case "CVPixelBufferCreateWithIOSurface":
			case "MPSImageBatchIncrementReadCount":
			case "MPSImageBatchSynchronize":
			case "MPSImageBatchResourceSize":
			case "MPSStateBatchIncrementReadCount":
			case "MPSStateBatchSynchronize":
			case "MPSStateBatchResourceSize":
			case "MPSHintTemporaryMemoryHighWaterMark":
			case "MPSSetHeapCacheDuration":
			case "MPSGetPreferredDevice":
				return simulator;

			// it's not needed for ARM64 and Apple does not have stubs for them in libobjc.dylib
			case "objc_msgSend_stret":
			case "objc_msgSendSuper_stret":
				return IntPtr.Size == 8 && !simulator;

			default:
				return base.Skip (symbolName);
			}
		}

		protected override bool SkipAssembly (Assembly a)
		{
			// we only want to check this on a version of iOS that
			// 1. is the current SDK target (or a newer one)
			var sdk = new Version (Constants.SdkVersion);
#if __WATCHOS__
			if (!TestRuntime.CheckSystemVersion (PlatformName.WatchOS, sdk.Major, sdk.Minor))
				return true;
#elif __IOS__ || __TVOS__
			if (!UIDevice.CurrentDevice.CheckSystemVersion (sdk.Major, sdk.Minor))
				return true;
#else
	#error unknown target
#endif
			// 2. on the real target for Xamarin.iOS.dll/monotouch.dll
			//    as the simulator miss some libraries and symbols
			//    but the rest of the BCL is fine to test
			return (a == typeof (NSObject).Assembly && (Runtime.Arch == Arch.SIMULATOR));
		}

		[Test]
		public void MonoNativeFunctionWrapper ()
		{
			var nativeDelegates = from type in Assembly.GetTypes () where !Skip (type)
				let attr = type.GetCustomAttribute<MonoNativeFunctionWrapperAttribute> () where attr != null
				select type;

			var failed_api = new List<string> ();
			Errors = 0;
			int c = 0, n = 0;
			foreach (var t in nativeDelegates) {
				if (LogProgress)
					Console.WriteLine ("{0}. {1}", c++, t);

				foreach (var mi in t.GetMethods ()) {
					if (mi.DeclaringType == t)
						CheckSignature (mi);
				}
				n++;
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} native delegate validated: {2}", Errors, n, string.Join (", ", failed_api));
		}

		[Test]
		public void MonoPInvokeCallback ()
		{
			var failed_api = new List<string> ();
			Errors = 0;
			int c = 0, n = 0;
			foreach (var type in Assembly.GetTypes ()) {
				if (Skip (type))
					continue;
				foreach (var mi in type.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
					if (mi.DeclaringType != type)
						continue;
					if (Skip (mi))
						continue;
					var attr = mi.GetCustomAttribute<MonoPInvokeCallbackAttribute> ();
					if (attr == null)
						continue;
					
					if (LogProgress)
						Console.WriteLine ("{0}. {1}", c++, mi);

					var at = attr.DelegateType;
					foreach (var m in at.GetMethods ()) {
						if (m.DeclaringType != at)
							continue;
						CheckSignature (m);
					}
					n++;
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} native delegate validated: {2}", Errors, n, string.Join (", ", failed_api));
		}

		[Test]
		public void NUnitLite ()
		{
			var a = typeof (TestAttribute).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

#if !__WATCHOS__
		[Test]
		public void MonoTouchDialog ()
		{
			// there's no direct reference to MTD - but it's there
			var a = AppDelegate.Runner.NavigationController.TopViewController.GetType ().Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}
#endif
	}
}