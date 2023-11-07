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
using Foundation;
using ObjCRuntime;
using UIKit;
using NUnit.Framework;
using Xamarin.Utils;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiPInvokeTest : ApiPInvokeTest {

		protected override bool Skip (string symbolName)
		{
			var simulator = TestRuntime.IsSimulatorOrDesktop;
			switch (symbolName) {
			// Metal support inside simulator is only available in recent iOS9 SDK
#if !__WATCHOS__
			case "MTLCreateSystemDefaultDevice":
				return simulator && !UIDevice.CurrentDevice.CheckSystemVersion (9, 0);
#endif
			// still most Metal helpers are not available on the simulator (even when the framework is present, it's missing symbols)
			case "MPSSupportsMTLDevice":
			case "MPSGetPreferredDevice":
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
			case "MPSImageBatchIncrementReadCount":
			case "MPSImageBatchSynchronize":
			case "MPSImageBatchResourceSize":
			case "MPSStateBatchIncrementReadCount":
			case "MPSStateBatchSynchronize":
			case "MPSStateBatchResourceSize":
			case "MPSHintTemporaryMemoryHighWaterMark":
			case "MPSSetHeapCacheDuration":
			case "MPSGetImageType":
				return simulator;
			case "CVPixelBufferGetIOSurface":
			case "CVPixelBufferCreateWithIOSurface":
				return simulator && !TestRuntime.CheckXcodeVersion (11, 0);

			default:
				// MLCompute not available in simulator as of Xcode 12 beta 3
				if (simulator && symbolName.StartsWith ("MLC", StringComparison.Ordinal))
					return true;
				return base.Skip (symbolName);
			}
		}

		protected override bool SkipAssembly (Assembly a)
		{
			// we only want to check this on a version of iOS that
			// 1. is the current SDK target (or a newer one)
			var sdk = new Version (Constants.SdkVersion);
#if __WATCHOS__
			if (!TestRuntime.CheckSystemVersion (ApplePlatform.WatchOS, sdk.Major, sdk.Minor))
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
			return (a == typeof (NSObject).Assembly && TestRuntime.IsSimulatorOrDesktop);
		}

		[Test]
		public void MonoNativeFunctionWrapper ()
		{
			var nativeDelegates = from type in Assembly.GetTypes ()
								  where !Skip (type)
								  let attr = type.GetCustomAttribute<MonoNativeFunctionWrapperAttribute> ()
								  where attr is not null
								  select type;

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
			AssertIfErrors ("{0} errors found in {1} native delegate validated", Errors, n);
		}

		[Test]
		public void MonoPInvokeCallback ()
		{
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
					if (attr is null)
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
			AssertIfErrors ("{0} errors found in {1} native delegate validated", Errors, n);
		}
	}
}
