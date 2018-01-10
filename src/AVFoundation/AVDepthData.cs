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
using Foundation;
using ObjCRuntime;
using CoreVideo;
using ImageIO;

namespace AVFoundation {
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