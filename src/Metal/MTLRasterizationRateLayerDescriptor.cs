#if IOS
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace Metal {
	public partial class MTLRasterizationRateLayerDescriptor
	{
/*  Selectors reported as not working by instrospection: https://github.com/xamarin/maccore/issues/1976
		[NoMac, NoTV, iOS (13,0)]
		public double[] HorizontalSampleStorage { 
			get {
				var width = (int)SampleCount.Width;
				var floatArray = new double[width];
				Marshal.Copy (_HorizontalSampleStorage, floatArray, 0, width);
				return floatArray;
			}
		}

		[NoMac, NoTV, iOS (13,0)]
		public double[] VerticalSampleStorage {
			get {
				var height = (int)SampleCount.Height;
				var floatArray = new double[height];
				Marshal.Copy (_VerticalSampleStorage, floatArray, 0, height);
				return floatArray;
			}
		}
*/
		[NoMac, NoTV, iOS (13,0)]
		static public MTLRasterizationRateLayerDescriptor Create (MTLSize sampleCount, float[] horizontal, float[] vertical)
		{
			if (sampleCount.Width != horizontal.Length)
				throw new ArgumentOutOfRangeException ("horizontal lenght should be equal to the sampleCount.Witdh.");
			if (sampleCount.Height != vertical.Length)
				throw new ArgumentOutOfRangeException ("horizontal lenght should be equal to the sampleCount.Witdh.");

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