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
using System.IO;
using System.Collections.Generic;
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
				if (AudioFileGetGlobalInfoSize (AudioFileGlobalProperty.ReadableTypes, 0, IntPtr.Zero, out size) != 0)
					return null;

				var data = new AudioFileType [size / sizeof (AudioFileType)];
				fixed (AudioFileType* ptr = data) {
					var res = AudioFileGetGlobalInfo (AudioFileGlobalProperty.ReadableTypes, 0, IntPtr.Zero, ref size, ptr);
					if (res != 0)
						return null;

					return data;
				}
			}
		}

		public static AudioFileType []? WritableTypes {
			get {
				uint size;
				if (AudioFileGetGlobalInfoSize (AudioFileGlobalProperty.WritableTypes, 0, IntPtr.Zero, out size) != 0)
					return null;

				var data = new AudioFileType [size / sizeof (AudioFileType)];
				fixed (AudioFileType* ptr = data) {
					var res = AudioFileGetGlobalInfo (AudioFileGlobalProperty.WritableTypes, 0, IntPtr.Zero, ref size, ptr);
					if (res != 0)
						return null;

					return data;
				}
			}
		}

		public static string? GetFileTypeName (AudioFileType fileType)
		{
			IntPtr ptr;
			var size = (uint) sizeof (IntPtr);
			if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.FileTypeName, sizeof (AudioFileType), ref fileType, ref size, out ptr) != 0)
				return null;

			return CFString.FromHandle (ptr);
		}

		public static AudioFormatType []? GetAvailableFormats (AudioFileType fileType)
		{
			uint size;
			if (AudioFileGetGlobalInfoSize (AudioFileGlobalProperty.AvailableFormatIDs, sizeof (AudioFileType), ref fileType, out size) != 0)
				return null;

			var data = new AudioFormatType [size / sizeof (AudioFormatType)];
			fixed (AudioFormatType* ptr = data) {
				var res = AudioFileGetGlobalInfo (AudioFileGlobalProperty.AvailableFormatIDs, sizeof (AudioFormatType), ref fileType, ref size, ptr);
				if (res != 0)
					return null;

				return data;
			}
		}

		public static AudioStreamBasicDescription []? GetAvailableStreamDescriptions (AudioFileType fileType, AudioFormatType formatType)
		{
			AudioFileTypeAndFormatID input;
			input.FileType = fileType;
			input.FormatType = formatType;

			uint size;
			if (AudioFileGetGlobalInfoSize (AudioFileGlobalProperty.AvailableStreamDescriptionsForFormat, (uint) sizeof (AudioFileTypeAndFormatID), ref input, out size) != 0)
				return null;

			var data = new AudioStreamBasicDescription [size / sizeof (AudioStreamBasicDescription)];
			fixed (AudioStreamBasicDescription* ptr = data) {
				var res = AudioFileGetGlobalInfo (AudioFileGlobalProperty.AvailableStreamDescriptionsForFormat, (uint) sizeof (AudioFileTypeAndFormatID), ref input, ref size, ptr);
				if (res != 0)
					return null;

				return data;
			}
		}

		public static string? []? AllExtensions {
			get {
				IntPtr ptr;
				var size = (uint) sizeof (IntPtr);
				if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.AllExtensions, 0, IntPtr.Zero, ref size, out ptr) != 0)
					return null;

				return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
			}
		}

		public static string? []? AllUTIs {
			get {
				IntPtr ptr;
				var size = (uint) sizeof (IntPtr);
				if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.AllUTIs, 0, IntPtr.Zero, ref size, out ptr) != 0)
					return null;

				return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
			}
		}

		public static string? []? AllMIMETypes {
			get {
				IntPtr ptr;
				var size = (uint) sizeof (IntPtr);
				if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.AllMIMETypes, 0, IntPtr.Zero, ref size, out ptr) != 0)
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
			IntPtr ptr;
			var size = (uint) sizeof (IntPtr);
			if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.ExtensionsForType, sizeof (AudioFileType), ref fileType, ref size, out ptr) != 0)
				return null;

			return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
		}

		public static string? []? GetUTIs (AudioFileType fileType)
		{
			IntPtr ptr;
			var size = (uint) sizeof (IntPtr);
			if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.UTIsForType, sizeof (AudioFileType), ref fileType, ref size, out ptr) != 0)
				return null;

			return NSArray.ArrayFromHandleFunc (ptr, l => CFString.FromHandle (l));
		}

		public static string? []? GetMIMETypes (AudioFileType fileType)
		{
			IntPtr ptr;
			var size = (uint) sizeof (IntPtr);
			if (AudioFileGetGlobalInfo (AudioFileGlobalProperty.MIMETypesForType, sizeof (AudioFileType), ref fileType, ref size, out ptr) != 0)
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
		extern static int AudioFileGetGlobalInfoSize (AudioFileGlobalProperty propertyID, uint size, IntPtr inSpecifier, out uint outDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfoSize (AudioFileGlobalProperty propertyID, uint size, ref AudioFileType inSpecifier, out uint outDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfoSize (AudioFileGlobalProperty propertyID, uint size, ref AudioFileTypeAndFormatID inSpecifier, out uint outDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, IntPtr inSpecifier, ref uint ioDataSize, AudioFileType* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, ref AudioFileType inSpecifier, ref uint ioDataSize, AudioFormatType* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, ref AudioFileTypeAndFormatID inSpecifier, ref uint ioDataSize, AudioStreamBasicDescription* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, ref AudioFileType inSpecifier, ref uint ioDataSize, out IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, IntPtr inSpecifier, ref uint ioDataSize, out IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileGetGlobalInfo (AudioFileGlobalProperty propertyID, uint size, IntPtr inSpecifier, ref uint ioDataSize, out uint outPropertyData);
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
