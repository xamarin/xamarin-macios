//
// StatusCode.cs: SystemConfiguration error handling
//
// Authors:
//    Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012, 2016 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.SystemConfiguration {

	// https://developer.apple.com/library/mac/#documentation/SystemConfiguration/Reference/SystemConfiguration_Utilities/Reference/reference.html
	public static class StatusCodeError
	{
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern internal static StatusCode /* int */ SCError ();
		
		[DllImport (Constants.SystemConfigurationLibrary)]
		extern static IntPtr /* const char* */ SCErrorString (int code);
		
		public static string GetErrorDescription (StatusCode statusCode)
		{
			var ptr = SCErrorString ((int) statusCode);
			return Marshal.PtrToStringAnsi (ptr);
		}
	}
}
