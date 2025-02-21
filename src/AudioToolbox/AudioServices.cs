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

#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

namespace AudioToolbox {

	public enum AudioServicesError { // Implictly cast to OSType
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		UnsupportedProperty = 0x7074793f,       // 'pty?'
		/// <summary>To be added.</summary>
		BadPropertySize = 0x2173697a,           // '!siz'
		/// <summary>To be added.</summary>
		BadSpecifierSizeError = 0x21737063,     // '!spc'
		/// <summary>To be added.</summary>
		SystemSoundUnspecifiedError = -1500,
		/// <summary>To be added.</summary>
		SystemSoundClientTimedOutError = -1501,
		/// <summary>To be added.</summary>
		SystemSoundExceededMaximumDurationError = -1502,
	}

	enum AudioServicesPropertyKey : uint // UInt32 AudioServicesPropertyID
	{
		IsUISound = 0x69737569, // 'isui'
		CompletePlaybackIfAppDies = 0x69666469  // 'ifdi'
	}

	static class AudioServices {

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesGetPropertyInfo (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, out uint propertyDataSize, out bool writable);

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesGetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, out uint propertyDataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe static extern AudioServicesError AudioServicesGetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, uint* specifier, uint* propertyDataSize, uint* propertyData);

		//[DllImport (Constants.AudioToolboxLibrary)]
		//static extern AudioServicesError AudioServicesSetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, IntPtr specifier, uint propertyDataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe static extern AudioServicesError AudioServicesSetProperty (AudioServicesPropertyKey propertyId, uint specifierSize, uint* specifier, uint propertyDataSize, uint* propertyData);
	}
}
