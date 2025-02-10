#if IOS
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

#nullable enable

namespace Metal {
	public partial class MTLRasterizationRateLayerDescriptor
	{
#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("tvos")]
#else
		[MacCatalyst (15,0)]
#endif
		public double[] HorizontalSampleStorage { 
			get {
				var width = (int)SampleCount.Width;
				var floatArray = new double[width];
				Marshal.Copy (_HorizontalSampleStorage, floatArray, 0, width);
				return floatArray;
			}
		}

#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[MacCatalyst (15,0)]
#endif
		public double[] VerticalSampleStorage {
			get {
				var height = (int)SampleCount.Height;
				var floatArray = new double[height];
				Marshal.Copy (_VerticalSampleStorage, floatArray, 0, height);
				return floatArray;
			}
		}

#if NET
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("ios13.0")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
#else
		[MacCatalyst (15,0)]
#endif
		static public MTLRasterizationRateLayerDescriptor Create (MTLSize sampleCount, float[] horizontal, float[] vertical)
		{
			if (horizontal is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (horizontal));
			if (vertical is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (vertical));
			if (sampleCount.Width != horizontal.Length)
				throw new ArgumentOutOfRangeException ("Horizontal length should be equal to the sampleCount.Width.");
			if (sampleCount.Height != vertical.Length)
				throw new ArgumentOutOfRangeException ("Vertical length should be equal to the sampleCount.Height.");

			unsafe {
				fixed (void* horizontalHandle = horizontal)
				fixed (void* verticalHandle = vertical) {
					return new MTLRasterizationRateLayerDescriptor (sampleCount, (IntPtr) horizontalHandle, (IntPtr) verticalHandle);
				}
			}
		}
	}
}
#endif
