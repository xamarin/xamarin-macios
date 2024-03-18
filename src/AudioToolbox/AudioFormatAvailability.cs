// 
// AudioFormatAvailability.cs:
//
// Authors:
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012 Xamarin Inc
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

using ObjCRuntime;
using CoreFoundation;
using Foundation;
using System.Runtime.Versioning;

namespace AudioToolbox {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class AudioFormatAvailability {
		public static AudioValueRange []? GetAvailableEncodeBitRates (AudioFormatType format)
		{
			return GetAvailable<AudioValueRange> (AudioFormatProperty.AvailableEncodeBitRates, format);
		}

		public static AudioValueRange []? GetAvailableEncodeSampleRates (AudioFormatType format)
		{
			return GetAvailable<AudioValueRange> (AudioFormatProperty.AvailableEncodeSampleRates, format);
		}

		public static AudioClassDescription []? GetDecoders (AudioFormatType format)
		{
			return GetAvailable<AudioClassDescription> (AudioFormatProperty.Decoders, format);
		}

		public static AudioClassDescription []? GetEncoders (AudioFormatType format)
		{
			return GetAvailable<AudioClassDescription> (AudioFormatProperty.Encoders, format);
		}

		unsafe static T []? GetAvailable<T> (AudioFormatProperty prop, AudioFormatType format) where T : unmanaged
		{
			uint size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (prop, sizeof (AudioFormatType), &format, &size) != 0)
				return null;

			if (size == 0)
				return Array.Empty<T> ();

			var data = new T [size / Marshal.SizeOf<T> ()];
			fixed (T* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (prop, sizeof (AudioFormatType), &format, &size, (IntPtr) ptr);
				if (res != 0)
					return null;
			}


			Array.Resize (ref data, (int) size / sizeof (T));
			return data;
		}
	}

	static partial class AudioFormatPropertyNative {
		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, AudioClassDescription* inSpecifier, int* ioPropertyDataSize,
			uint* outPropertyData);
	}
}
