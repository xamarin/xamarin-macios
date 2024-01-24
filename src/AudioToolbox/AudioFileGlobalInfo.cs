// 
// AudioFileGlobalInfo.cs:
//
// Authors:
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012 Xamarin Inc.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	public unsafe static class AudioFileGlobalInfo {
		public static AudioFileType []? ReadableTypes {
			get {
				uint size;
				if (!TryGetGlobalInfoSize (AudioFileGlobalProperty.ReadableTypes, out size))
					return null;

				if (!TryGetGlobalInfo (AudioFileGlobalProperty.ReadableTypes, size, out var data))
					return null;

				return data;
			}
		}

		public static AudioFileType []? WritableTypes {
			get {
				uint size;
				if (!TryGetGlobalInfoSize (AudioFileGlobalProperty.WritableTypes, out size))
					return null;

				if (!TryGetGlobalInfo (AudioFileGlobalProperty.WritableTypes, size, out var data))
					return null;

				return data;
			}
		}

		public static string? GetFileTypeName (AudioFileType fileType)
		{
			if (!TryGetGlobalInfo (AudioFileGlobalProperty.FileTypeName, fileType, out var ptr))
				return null;

			return CFString.FromHandle (ptr);
		}

		public static AudioFormatType []? GetAvailableFormats (AudioFileType fileType)
		{
			if (!TryGetGlobalInfoSize (AudioFileGlobalProperty.AvailableFormatIDs, fileType, out var size))
				return null;

			if (!TryGetGlobalInfo (AudioFileGlobalProperty.AvailableFormatIDs, fileType, size, out var data))
				return null;

			return data;
		}

		public static AudioStreamBasicDescription []? GetAvailableStreamDescriptions (AudioFileType fileType, AudioFormatType formatType)
		{
			AudioFileTypeAndFormatID input;
			input.FileType = fileType;
			input.FormatType = formatType;

			if (!TryGetGlobalInfoSize (AudioFileGlobalProperty.AvailableStreamDescriptionsForFormat, input, out var size))
				return null;

			if (!TryGetGlobalInfo (AudioFileGlobalProperty.AvailableStreamDescriptionsForFormat, input, size, out var data))
				return null;

			return data;
		}

		public static string? []? AllExtensions {
			get {
				if (!TryGetGlobalInfo (AudioFileGlobalProperty.AllExtensions, out var ptr))
					return null;

				return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
			}
		}

		public static string? []? AllUTIs {
			get {
				if (!TryGetGlobalInfo (AudioFileGlobalProperty.AllUTIs, out var ptr))
					return null;

				return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
			}
		}

		public static string? []? AllMIMETypes {
			get {
				if (!TryGetGlobalInfo (AudioFileGlobalProperty.AllMIMETypes, out var ptr))
					return null;

				return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
			}
		}

		/*
		// TODO: Don't have HFSTypeCode 
		public static HFSTypeCode[] AllHFSTypeCodes {
			get {
				uint size;
				if (AudioFileGetGlobalInfoSize (AudioFileGlobalProperty.AllHFSTypeCodes, 0, IntPtr.Zero, out size) != 0)
					return null;

				var data = new HFSTypeCode[size / sizeof (HFSTypeCode)];
				fixed (AudioFileType* ptr = data) {
					var res = AudioFileGetGlobalInfo (AudioFileGlobalProperty.AllHFSTypeCodes, 0, IntPtr.Zero, ref size, ptr);
					if (res != 0)
						return null;

					return data;
				}
			}
		}
		*/

		public static string? []? GetExtensions (AudioFileType fileType)
		{
			if (!TryGetGlobalInfo (AudioFileGlobalProperty.ExtensionsForType, fileType, out var ptr))
				return null;

			return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
		}

		public static string? []? GetUTIs (AudioFileType fileType)
		{
			if (!TryGetGlobalInfo (AudioFileGlobalProperty.UTIsForType, fileType, out var ptr))
				return null;

			return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
		}

		public static string? []? GetMIMETypes (AudioFileType fileType)
		{
			if (!TryGetGlobalInfo (AudioFileGlobalProperty.MIMETypesForType, fileType, out var ptr))
				return null;

			return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
		}

		/*
				// TODO: Always returns 0
				public static AudioFileType? GetTypesForExtension (string extension)
				{
					using (var cfs = new CFString (extension)) {
						uint value;
						uint size = sizeof (AudioFileType);
						if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.TypesForExtension, (uint) IntPtr.Size, cfs.Handle, ref size, out value) != 0)
							return null;

						return (AudioFileType) value;
					}
				}
		*/

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfoSize (AudioFileGlobalProperty propertyId, uint size, void* inSpecifier, uint* outDataSize);

		static bool TryGetGlobalInfoSize (AudioFileGlobalProperty propertyId, out uint size)
		{
			size = 0;
			var rv = AudioFileGetGlobalInfoSize (propertyId, 0, null, (uint*) Unsafe.AsPointer<uint> (ref size));
			return rv == 0;
		}

		static bool TryGetGlobalInfoSize (AudioFileGlobalProperty propertyId, AudioFileType fileType, out uint size)
		{
			size = 0;
			var rv = AudioFileGetGlobalInfoSize (propertyId, sizeof (AudioFileType), (void*) &fileType, (uint*) Unsafe.AsPointer<uint> (ref size));
			return rv == 0;
		}

		static bool TryGetGlobalInfoSize (AudioFileGlobalProperty propertyId, AudioFileTypeAndFormatID audioFileTypeAndFormatId, out uint size)
		{
			size = 0;
			var rv = AudioFileGetGlobalInfoSize (propertyId, (uint) sizeof (AudioFileTypeAndFormatID), (void*) &audioFileTypeAndFormatId, (uint*) Unsafe.AsPointer<uint> (ref size));
			return rv == 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyId, uint size, void* inSpecifier, uint* ioDataSize, void* outPropertyData);

		static bool TryGetGlobalInfo (AudioFileGlobalProperty propertyId, uint size, [NotNullWhen (true)] out AudioFileType []? outPropertyData)
		{
			int res;
			var data = new AudioFileType [size / sizeof (AudioFileType)];
			outPropertyData = default;
			fixed (AudioFileType* ptr = data) {
				res = AudioFileGetGlobalInfo (propertyId, 0, null, &size, ptr);
			}
			if (res == 0)
				outPropertyData = data;
			return res == 0;
		}

		static bool TryGetGlobalInfo (AudioFileGlobalProperty propertyId, AudioFileType fileType, uint size, [NotNullWhen (true)] out AudioFormatType []? outPropertyData)
		{
			int res;
			var data = new AudioFormatType [size / sizeof (AudioFormatType)];
			outPropertyData = default;
			fixed (AudioFormatType* ptr = data) {
				res = AudioFileGetGlobalInfo (propertyId, sizeof (AudioFormatType), &fileType, &size, ptr);
			}
			if (res == 0)
				outPropertyData = data;
			return res == 0;
		}

		static bool TryGetGlobalInfo (AudioFileGlobalProperty propertyId, AudioFileTypeAndFormatID audioFileTypeAndFormatId, uint size, [NotNullWhen (true)] out AudioStreamBasicDescription []? outPropertyData)
		{
			int res;
			var data = new AudioStreamBasicDescription [size / sizeof (AudioStreamBasicDescription)];
			outPropertyData = default;
			fixed (AudioStreamBasicDescription* ptr = data) {
				res = AudioFileGetGlobalInfo (propertyId, (uint) sizeof (AudioFileTypeAndFormatID), &audioFileTypeAndFormatId, &size, ptr);
			}
			if (res == 0)
				outPropertyData = data;
			return res == 0;
		}

		static bool TryGetGlobalInfo (AudioFileGlobalProperty propertyId, AudioFileType fileType, out IntPtr outPropertyData)
		{
			var size = (uint) sizeof (IntPtr);
			outPropertyData = default;
			var rv = AudioFileGetGlobalInfo (propertyId, sizeof (AudioFileType), &fileType, &size, (IntPtr*) Unsafe.AsPointer<IntPtr> (ref outPropertyData));
			return rv == 0;

		}

		static bool TryGetGlobalInfo (AudioFileGlobalProperty propertyId, out IntPtr outPropertyData)
		{
			var size = (uint) sizeof (IntPtr);
			outPropertyData = default;
			var rv = AudioFileGetGlobalInfo (propertyId, 0, null, &size, (IntPtr*) Unsafe.AsPointer<IntPtr> (ref outPropertyData));
			return rv == 0;
		}
	}

	[StructLayout (LayoutKind.Sequential)]
	struct AudioFileTypeAndFormatID {
		public AudioFileType FileType;
		public AudioFormatType FormatType;
	}

	enum AudioFileGlobalProperty : uint // UInt32 AudioFileTypeID
	{
		ReadableTypes = 0x61667266, // 'afrf'
		WritableTypes = 0x61667766, // 'afwf'
		FileTypeName = 0x66746e6d,  // 'ftnm'
		AvailableStreamDescriptionsForFormat = 0x73646964,  // 'sdid'
		AvailableFormatIDs = 0x666d6964,    // 'fmid'

		AllExtensions = 0x616c7874, // 'alxt'
		AllHFSTypeCodes = 0x61686673,   // 'ahfs'
		AllUTIs = 0x61757469,   // 'auti'
		AllMIMETypes = 0x616d696d,  // 'amim'

		ExtensionsForType = 0x66657874, // 'fext'
										//		HFSTypeCodesForType					= 'fhfs',
		UTIsForType = 0x66757469,   // 'futi'
		MIMETypesForType = 0x666d696d,  // 'fmim'

		TypesForMIMEType = 0x746d696d,  // 'tmim'
		TypesForUTI = 0x74757469,   // 'tuti'
									//		TypesForHFSTypeCode					= 'thfs',
		TypesForExtension = 0x74657874, // 'text'
	}
}
