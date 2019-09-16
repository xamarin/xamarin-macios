#if IOS
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace Metal {
	public partial class MTLRasterizationRateLayerDescriptor
	{
		[NoMac, NoTV, iOS (13,0)]
		public double[] HorizontalSampleStorage { 
			get {
				unsafe {
					var width = (int)SampleCount.Width;
					var floatArray = new double[width];
					Marshal.Copy (_HorizontalSampleStorage, floatArray, 0, width);
					return floatArray;
				}
			}
		}

		[NoMac, NoTV, iOS (13,0)]
		public double[] VerticalSampleStorage {
			get {
				unsafe {
					var height = (int)SampleCount.Height;
					var floatArray = new double[height];
					Marshal.Copy (_VerticalSampleStorage, floatArray, 0, height);
					return floatArray;
				}
			}
		}

		[NoMac, NoTV, iOS (13,0)]
		static public MTLRasterizationRateLayerDescriptor Create (MTLSize sampleCount, float[] horizontal, float[] vertical) {
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