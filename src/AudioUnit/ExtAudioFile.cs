//
// ExtAudioFile.cs: ExtAudioFile wrapper class
//
// Authors:
//   AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2010 Reinforce Lab.
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
//

#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using AudioToolbox;
using Foundation;
using System.Runtime.Versioning;

namespace AudioUnit {
	public enum ExtAudioFileError // Implictly cast to OSType
	{
		OK = 0,
		CodecUnavailableInputConsumed = -66559,
		CodecUnavailableInputNotConsumed = -66560,
		InvalidProperty = -66561,
		InvalidPropertySize = -66562,
		NonPCMClientFormat = -66563,
		InvalidChannelMap = -66564,
		InvalidOperationOrder = -66565,
		InvalidDataFormat = -66566,
		MaxPacketSizeUnknown = -66567,
		InvalidSeek = -66568,
		AsyncWriteTooLarge = -66569,
		AsyncWriteBufferOverflow = -66570,

		// Shared error codes
		NotOpenError = -38,
		EndOfFileError = -39,
		PositionError = -40,
		FileNotFoundError = -43,
		BadFilePathError = 0x21707468, // '!pth'
		FilePermissionError = -54,
		TooManyFilesOpenError = -42,
	}

#if NET
    [SupportedOSPlatform ("ios")]
    [SupportedOSPlatform ("maccatalyst")]
    [SupportedOSPlatform ("macos")]
    [SupportedOSPlatform ("tvos")]
#endif
	public class ExtAudioFile : IDisposable {
		IntPtr _extAudioFile;

		public uint? ClientMaxPacketSize {
			get {
				uint size = sizeof (uint);
				uint value;
				if (ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.ClientMaxPacketSize, ref size, out value) != ExtAudioFileError.OK)
					return null;

				return value;
			}
		}

		public uint? FileMaxPacketSize {
			get {
				uint size = sizeof (uint);
				uint value;

				if (ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.FileMaxPacketSize, ref size, out value) != ExtAudioFileError.OK)
					return null;

				return value;
			}
		}


		public IntPtr? AudioFile {
			get {
				uint size = (uint) IntPtr.Size;
				IntPtr value;
				if (ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.AudioFile, ref size, out value) != ExtAudioFileError.OK)
					return null;

				return value;
			}
		}

		public AudioConverter? AudioConverter {
			get {
				uint size = sizeof (uint);
				IntPtr value;

				if (ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.AudioConverter, ref size, out value) != ExtAudioFileError.OK)
					return null;

				return new AudioConverter (value, false);
			}
		}

		public long FileLengthFrames {
			get {
				long length;
				uint size = sizeof (long);

				var err = ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.FileLengthFrames, ref size, out length);
				if (err != 0) {
					throw new InvalidOperationException (String.Format ("Error code:{0}", err));
				}

				return length;
			}
		}

		public AudioStreamBasicDescription FileDataFormat {
			get {
				AudioStreamBasicDescription dc = new AudioStreamBasicDescription ();
				uint size = (uint) Marshal.SizeOf<AudioStreamBasicDescription> ();
				int err = ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.FileDataFormat, ref size, ref dc);
				if (err != 0) {
					throw new InvalidOperationException (String.Format ("Error code:{0}", err));
				}

				return dc;
			}
		}

		public AudioStreamBasicDescription ClientDataFormat {
			get {
				uint size = (uint) Marshal.SizeOf<AudioStreamBasicDescription> ();
				AudioStreamBasicDescription value = new AudioStreamBasicDescription ();
				if (ExtAudioFileGetProperty (_extAudioFile, PropertyIDType.ClientDataFormat, ref size, ref value) != (int) ExtAudioFileError.OK)
					return default (AudioStreamBasicDescription);

				return value;
			}

			set {
				int err = ExtAudioFileSetProperty (_extAudioFile, PropertyIDType.ClientDataFormat,
					(uint) Marshal.SizeOf<AudioStreamBasicDescription> (), ref value);
				if (err != 0) {
					throw new InvalidOperationException (String.Format ("Error code:{0}", err));
				}
			}
		}

		private ExtAudioFile (IntPtr ptr)
		{
			_extAudioFile = ptr;
		}

		~ExtAudioFile ()
		{
			Dispose (false);
		}

		// Since throwing and ArgumentException seems like a bad idea due to you do not get acces programatically
		// to the actual error code from the native API and we are not allowed to make Breaking Changes
		// lets reimplement the method in a way to return the actual native value if any
		// also we can share the underliying implementation so we so not break api and reduce code suplication
		public static ExtAudioFile? OpenUrl (NSUrl url, out ExtAudioFileError error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return OpenUrl (url.Handle, out error);
		}

		public static ExtAudioFile? OpenUrl (CFUrl url, out ExtAudioFileError error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return OpenUrl (url.Handle, out error);
		}

		public static ExtAudioFile OpenUrl (CFUrl url)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			ExtAudioFileError err;
			var audioFile = OpenUrl (url.Handle, out err);

			if (err != ExtAudioFileError.OK) // if (err != 0)  <- to keep old implementation
				throw new ArgumentException (String.Format ("Error code:{0}", err));
			if (audioFile is null) // if (ptr == IntPtr.Zero)  <- to keep old implementation
				throw new InvalidOperationException ("Can not get object instance");

			return audioFile;
		}

		static ExtAudioFile? OpenUrl (IntPtr urlHandle, out ExtAudioFileError error)
		{
			IntPtr ptr;
			error = ExtAudioFileOpenUrl (urlHandle, out ptr);

			if (error != ExtAudioFileError.OK || ptr == IntPtr.Zero)
				return null;
			else
				return new ExtAudioFile (ptr);
		}

		// Since throwing and ArgumentException seems like a bad idea due to you do not get acces programatically
		// to the actual error code from the native API and we are not allowed to make Breaking Changes
		// lets reimplement the method in a way to return the actual native value if any
		// also we can share the underliying implementation so we so not break api and reduce code suplication
		public static ExtAudioFile? CreateWithUrl (NSUrl url, AudioFileType fileType, AudioStreamBasicDescription inStreamDesc, AudioFileFlags fileFlags, out ExtAudioFileError error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return CreateWithUrl (url.Handle, fileType, inStreamDesc, fileFlags, out error);
		}

		public static ExtAudioFile? CreateWithUrl (CFUrl url, AudioFileType fileType, AudioStreamBasicDescription inStreamDesc, AudioFileFlags flag, out ExtAudioFileError error)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return CreateWithUrl (url.Handle, fileType, inStreamDesc, flag, out error);
		}

		public static ExtAudioFile CreateWithUrl (CFUrl url,
			AudioFileType fileType,
			AudioStreamBasicDescription inStreamDesc,
			//AudioChannelLayout channelLayout, 
			AudioFileFlags flag)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			ExtAudioFileError err;
			var audioFile = CreateWithUrl (url.Handle, fileType, inStreamDesc, flag, out err);

			if (err != ExtAudioFileError.OK) // if (err != 0)  <- to keep old implementation
				throw new ArgumentException (String.Format ("Error code:{0}", err));
			if (audioFile is null) // if (ptr == IntPtr.Zero)  <- to keep old implementation
				throw new InvalidOperationException ("Can not get object instance");

			return audioFile;
		}

		static ExtAudioFile? CreateWithUrl (IntPtr urlHandle, AudioFileType fileType, AudioStreamBasicDescription inStreamDesc, AudioFileFlags flag, out ExtAudioFileError error)
		{
			IntPtr ptr;
			error = (ExtAudioFileError) ExtAudioFileCreateWithUrl (urlHandle, fileType, ref inStreamDesc, IntPtr.Zero, (uint) flag, out ptr);
			if (error != ExtAudioFileError.OK || ptr == IntPtr.Zero)
				return null;
			else
				return new ExtAudioFile (ptr);
		}

		public static ExtAudioFileError WrapAudioFileID (IntPtr audioFileID, bool forWriting, out ExtAudioFile? outAudioFile)
		{
			IntPtr ptr;
			ExtAudioFileError res;
			unsafe {
				res = ExtAudioFileWrapAudioFileID (audioFileID, forWriting, (IntPtr) (&ptr));
			}

			if (res != ExtAudioFileError.OK) {
				outAudioFile = null;
				return res;
			}

			outAudioFile = new ExtAudioFile (ptr);
			return res;
		}

		public void Seek (long frameOffset)
		{
			int err = ExtAudioFileSeek (_extAudioFile, frameOffset);
			if (err != 0) {
				throw new ArgumentException (String.Format ("Error code:{0}", err));
			}
		}
		public long FileTell ()
		{
			long frame = 0;

			int err = ExtAudioFileTell (_extAudioFile, ref frame);
			if (err != 0) {
				throw new ArgumentException (String.Format ("Error code:{0}", err));
			}

			return frame;
		}

		public uint Read (uint numberFrames, AudioBuffers audioBufferList, out ExtAudioFileError status)
		{
			if (audioBufferList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioBufferList));

			status = ExtAudioFileRead (_extAudioFile, ref numberFrames, (IntPtr) audioBufferList);
			return numberFrames;
		}

		public ExtAudioFileError WriteAsync (uint numberFrames, AudioBuffers audioBufferList)
		{
			if (audioBufferList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioBufferList));

			return ExtAudioFileWriteAsync (_extAudioFile, numberFrames, (IntPtr) audioBufferList);
		}

		public ExtAudioFileError Write (uint numberFrames, AudioBuffers audioBufferList)
		{
			if (audioBufferList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (audioBufferList));

			return ExtAudioFileWrite (_extAudioFile, numberFrames, (IntPtr) audioBufferList);
		}

		public ExtAudioFileError SynchronizeAudioConverter ()
		{
			IntPtr value = IntPtr.Zero;
			return ExtAudioFileSetProperty (_extAudioFile, PropertyIDType.ConverterConfig,
				IntPtr.Size, value);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (_extAudioFile != IntPtr.Zero) {
				ExtAudioFileDispose (_extAudioFile);
				_extAudioFile = IntPtr.Zero;
			}
		}

		#region Interop
		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileOpenURL")]
		static extern ExtAudioFileError ExtAudioFileOpenUrl (IntPtr inUrl, out IntPtr outExtAudioFile);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileWrapAudioFileID (IntPtr inFileID, [MarshalAs (UnmanagedType.I1)] bool inForWriting, IntPtr outExtAudioFile);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileRead (IntPtr inExtAudioFile, ref uint /* UInt32* */ ioNumberFrames, IntPtr ioData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileWrite (IntPtr inExtAudioFile, uint /* UInt32 */ inNumberFrames, IntPtr ioData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileWriteAsync (IntPtr inExtAudioFile, uint /* UInt32 */ inNumberFrames, IntPtr ioData);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileDispose")]
		static extern int /* OSStatus */ ExtAudioFileDispose (IntPtr inExtAudioFile);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileSeek")]
		static extern int /* OSStatus */ ExtAudioFileSeek (IntPtr inExtAudioFile, long /* SInt64 */ inFrameOffset);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileTell")]
		static extern int /* OSStatus */ ExtAudioFileTell (IntPtr inExtAudioFile, ref long /* SInt64* */ outFrameOffset);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileCreateWithURL")]
		static extern int /* OSStatus */ ExtAudioFileCreateWithUrl (IntPtr inURL,
			[MarshalAs (UnmanagedType.U4)] AudioFileType inFileType,
			ref AudioStreamBasicDescription inStreamDesc,
			IntPtr inChannelLayout, //AudioChannelLayout inChannelLayout, AudioChannelLayout results in compilation error (error code 134.)
			UInt32 /* UInt32 */ flags,
			out IntPtr outExtAudioFile);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileGetProperty")]
		static extern int ExtAudioFileGetProperty (
			IntPtr inExtAudioFile,
			PropertyIDType inPropertyID,
			ref uint /* UInt32* */ ioPropertyDataSize,
			IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileGetProperty")]
		static extern int ExtAudioFileGetProperty (
			IntPtr inExtAudioFile,
			PropertyIDType inPropertyID,
			ref uint /* UInt32* */ ioPropertyDataSize,
			ref AudioStreamBasicDescription outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileGetProperty (IntPtr inExtAudioFile, PropertyIDType inPropertyID, ref uint /* UInt32* */ ioPropertyDataSize, out IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileGetProperty (IntPtr inExtAudioFile, PropertyIDType inPropertyID, ref uint /* UInt32* */ ioPropertyDataSize, out long outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileGetProperty (IntPtr inExtAudioFile, PropertyIDType inPropertyID, ref uint /* UInt32* */ ioPropertyDataSize, out uint outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		static extern ExtAudioFileError ExtAudioFileSetProperty (IntPtr inExtAudioFile, PropertyIDType inPropertyID, int /* UInt32 */ ioPropertyDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileSetProperty")]
		static extern int ExtAudioFileSetProperty (
			IntPtr inExtAudioFile,
			PropertyIDType inPropertyID,
			uint /* UInt32 */ ioPropertyDataSize,
			ref AudioStreamBasicDescription outPropertyData);

		enum PropertyIDType { // UInt32 ExtAudioFilePropertyID
			FileDataFormat = 0x66666d74,       // 'ffmt'
											   //kExtAudioFileProperty_FileChannelLayout		= 'fclo',   // AudioChannelLayout

			ClientDataFormat = 0x63666d74, //'cfmt',   // AudioStreamBasicDescription
										   //kExtAudioFileProperty_ClientChannelLayout	= 'cclo',   // AudioChannelLayout
			CodecManufacturer = 0x636d616e,      // 'cman'

			// read-only:
			AudioConverter = 0x61636e76,      // 'acnv'
			AudioFile = 0x6166696c,      // 'afil'
			FileMaxPacketSize = 0x666d7073,      // 'fmps'
			ClientMaxPacketSize = 0x636d7073,      // 'cmps'
			FileLengthFrames = 0x2366726d,      // '#frm'

			// writable:
			ConverterConfig = 0x61636366,      // 'accf'
											   //kExtAudioFileProperty_IOBufferSizeBytes		= 'iobs',	// UInt32
											   //kExtAudioFileProperty_IOBuffer				= 'iobf',	// void *
											   //kExtAudioFileProperty_PacketTable			= 'xpti'	// AudioFilePacketTableInfo             
		};
		#endregion
	}
}
