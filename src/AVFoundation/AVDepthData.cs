//
// AVDepthData.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//
// Copyright 2017 Xamarin Inc.
//

#if !WATCH
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;

using XamCore.CoreVideo;
using XamCore.ImageIO;

namespace XamCore.AVFoundation {
	public partial class AVDepthData {

		public static AVDepthData Create (CGImageAuxiliaryDataInfo dataInfo, out NSError error)
		{
			return Create (dataInfo.Dictionary, out error);
		}

		public CVPixelFormatType [] AvailableDepthDataTypes {
			get {
				var values = WeakAvailableDepthDataTypes;
				if (values == null)
					return null;

				var count = values.Length;
				var arr = new CVPixelFormatType [count];
				for (int i = 0; i < count; i++)
					arr [i] = (CVPixelFormatType) values [i].UInt32Value; // CVPixelFormatType is uint.

				return arr;
			}
		}
	}
}
#endif