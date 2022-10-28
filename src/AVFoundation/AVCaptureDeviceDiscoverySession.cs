// 
// AVCaptureDeviceDiscoverySession.cs
//
// Authors:
//	Alex Soto (alexsoto@microsoft.com)
//     
// Copyright 2016 Xamarin Inc.
//

using System;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace AVFoundation {
#if IOS && !NET
	public partial class AVCaptureDeviceDiscoverySession {

		public static AVCaptureDeviceDiscoverySession Create (AVCaptureDeviceType [] deviceTypes, string mediaType, AVCaptureDevicePosition position)
		{
			var arr = new NSMutableArray ();
			foreach (var device in deviceTypes)
				if (device.GetConstant () is NSString s)
					arr.Add (s);

			return _Create (arr, mediaType, position);
		}
	}
#endif
}
