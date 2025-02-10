#nullable enable

using System;
using System.Buffers;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;
using MetalPerformanceShaders;

namespace MetalPerformanceShadersGraph {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="MPSGraphExecutable" />.</summary>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
#else
	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
	public enum MPSGraphExecutableInitializationOption {
		/// <summary>The <c>packageUrl</c> parameter passed to the constructor is a url to a CoreML package.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		CoreMLPackage,

		/// <summary>The <c>packageUrl</c> parameter passed to the constructor is a url to a MPSGraph package.</summary>
		MPSGraphPackage,
	}

	public partial class MPSGraphExecutable {
		/// <summary>Create a new MPSGraphExecutable instance from a package url and a compilation descriptor..</summary>
		/// <param name="packageUrl">The url to the package to use.</param>
		/// <param name="compilationDescriptor">The optional compilation descriptor use.</param>
		/// <param name="option">Use this option to specify whether the package url points to a CoreML package or an MPSGraph package.</param>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
#else
		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
#endif
		public MPSGraphExecutable (NSUrl packageUrl, MPSGraphCompilationDescriptor? compilationDescriptor, MPSGraphExecutableInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case MPSGraphExecutableInitializationOption.CoreMLPackage:
				InitializeHandle (_InitWithCoreMLPackage (packageUrl, compilationDescriptor));
				break;
			case MPSGraphExecutableInitializationOption.MPSGraphPackage:
				InitializeHandle (_InitWithMPSGraphPackage (packageUrl, compilationDescriptor));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
