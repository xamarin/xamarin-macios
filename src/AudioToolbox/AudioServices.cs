// 
// AudioServices.cs:
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace AudioToolbox {

	public enum AudioServicesError { // Implictly cast to OSType
		None = 0,
		UnsupportedProperty = 0x7074793f,		// 'pty?'
		BadPropertySize = 0x2173697a,			// '!siz'
		BadSpecifierSizeError = 0x21737063,		// '!spc'
		SystemSoundUnspecifiedError = -1500,
		SystemSoundClientTimedOutError = -1501,
		[iOS (10,0), Mac (10,12, onlyOn64: true)]
		SystemSoundExceededMaximumDurationError = -1502,
	}

	enum AudioServicesPropertyKey : uint // UInt32 AudioServicesPropertyID
	{
		IsUISound 					= 0x69737569, // 'isui'
		CompletePlaybackIfAppDies	= 0x69666469  // 'ifdi'
	}
	
	static class AudioServices {

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesGetPropertyInfo (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, out uint propertyDataSize, out bool writable);

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesGetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, out uint propertyDataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public static extern AudioServicesError AudioServicesGetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, ref uint specifier, out uint propertyDataSize, out uint propertyData);

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesSetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, uint propertyDataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public static extern AudioServicesError AudioServicesSetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, ref uint specifier, uint propertyDataSize, ref uint propertyData);
	}
}
