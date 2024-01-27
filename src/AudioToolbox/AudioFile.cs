// 
// AudioFile.cs:
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2013 Xamarin Inc.
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using OSStatus = System.Int32;
using AudioFileID = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioToolbox {

	public enum AudioFileType {  // UInt32 AudioFileTypeID
		AIFF = 0x41494646, // AIFF
		AIFC = 0x41494643, // AIFC
		WAVE = 0x57415645, // WAVE
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		RF64 = 0x52463634, // RF64
		SoundDesigner2 = 0x53643266, // Sd2f
		Next = 0x4e655854, // NeXT
		MP3 = 0x4d504733, // MPG3
		MP2 = 0x4d504732, // MPG2
		MP1 = 0x4d504731, // MPG1
		AC3 = 0x61632d33, // ac-3
		AAC_ADTS = 0x61647473, // adts
		MPEG4 = 0x6d703466, // mp4f
		M4A = 0x6d346166, // m4af
		M4B = 0x6d346266, // m4bf
		CAF = 0x63616666, // caff
		ThreeGP = 0x33677070, // 3gpp
		ThreeGP2 = 0x33677032, // 3gp2
		AMR = 0x616d7266, // amrf
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
#endif
		FLAC = 0x666c6163, // flac
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[NoWatch]
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		LatmInLoas = 0x6c6f6173, // loas
	}

	public enum AudioFileError {// Implictly cast to OSType in AudioFile.h
		Success = 0, // noErr
		Unspecified = 0x7768743f, // wht?
		UnsupportedFileType = 0x7479703f, // typ?
		UnsupportedDataFormat = 0x666d743f, // fmt?
		UnsupportedProperty = 0x7074793f, // pty?
		BadPropertySize = 0x2173697a, // !siz
		Permissions = 0x70726d3f, // prm?
		NotOptimized = 0x6f70746d, // optm
		InvalidChunk = 0x63686b3f, // chk?
		DoesNotAllow64BitDataSize = 0x6f66663f, // off?
		InvalidPacketOffset = 0x70636b3f, // pck?
		InvalidFile = 0x6474613f, // dta?
		OperationNotSupported = 0x6F703F3F, // op??
		FileNotOpen = -38,
		EndOfFile = -39,
		FileNotFound = -43,
		FilePosition = -40,
	}

	[Flags]
	public enum AudioFilePermission {
		Read = 0x01,
		Write = 0x02,
		ReadWrite = 0x03
	}

	[Flags]
	public enum AudioFileFlags { // UInt32 in AudioFileCreateWithURL()
		EraseFlags = 1,
		DontPageAlignAudioData = 2
	}

	public enum AudioFileProperty { // typedef UInt32 AudioFilePropertyID
		FileFormat = 0x66666d74,
		DataFormat = 0x64666d74,
		IsOptimized = 0x6f70746d,
		MagicCookieData = 0x6d676963,
		AudioDataByteCount = 0x62636e74,
		AudioDataPacketCount = 0x70636e74,
		MaximumPacketSize = 0x70737a65,
		DataOffset = 0x646f6666,
		ChannelLayout = 0x636d6170,
		DeferSizeUpdates = 0x64737a75,
		DataFormatName = 0x666e6d65,
		MarkerList = 0x6d6b6c73,
		RegionList = 0x72676c73,
		PacketToFrame = 0x706b6672,
		FrameToPacket = 0x6672706b,
		PacketToByte = 0x706b6279,
		ByteToPacket = 0x6279706b,
		ChunkIDs = 0x63686964,
		InfoDictionary = 0x696e666f,
		PacketTableInfo = 0x706e666f,
		FormatList = 0x666c7374,
		PacketSizeUpperBound = 0x706b7562,
		ReserveDuration = 0x72737276,
		EstimatedDuration = 0x65647572,
		BitRate = 0x62726174,
		ID3Tag = 0x69643374,
		SourceBitDepth = 0x73627464,
		AlbumArtwork = 0x61617274,
		ReadyToProducePackets = 0x72656479,
		AverageBytesPerPacket = 0x61627070,
		AudioTrackCount = 0x61746374,
		UseAudioTrack = 0x7561746b,
	}

	public enum AudioFileLoopDirection { // Unused?
		NoLooping = 0,
		Forward = 1,
		ForwardAndBackward = 2,
		Backward = 3
	}

	public enum AudioFileChunkType : uint // CoreAudio.framework - CoreAudioTypes.h - "four char code IDs"
	{
		CAFStreamDescription = 0x64657363,  // 'desc'
		CAFAudioData = 0x64617461,  // 'data'
		CAFChannelLayout = 0x6368616e,  // 'chan'
		CAFFiller = 0x66726565, // 'free'
		CAFMarker = 0x6d61726b, // 'mark'
		CAFRegion = 0x7265676e, // 'regn'
		CAFInstrument = 0x696e7374, // 'inst'
		CAFMagicCookieID = 0x6b756b69,  // 'kuki'
		CAFInfoStrings = 0x696e666f,    // 'info'
		CAFEditComments = 0x65646374,   // 'edct'
		CAFPacketTable = 0x70616b74,    // 'pakt'
		CAFStrings = 0x73747267,    // 'strg'
		CAFUUID = 0x75756964,   // 'uuid'
		CAFPeak = 0x7065616b,   // 'peak'
		CAFOverview = 0x6f767677,   // 'ovvw'
		CAFMIDI = 0x6d696469,   // 'midi'
		CAFUMID = 0x756d6964,   // 'umid'
		CAFFormatListID = 0x6c647363,   // 'ldsc'
		CAFiXML = 0x69584d4c,   // 'iXML'
	}

	[StructLayout (LayoutKind.Sequential)]
	struct AudioFramePacketTranslation {
		public long Frame;
		public long Packet;
		public int FrameOffsetInPacket;
	}

	[StructLayout (LayoutKind.Sequential)]
	struct AudioBytePacketTranslation {
		public long Byte;
		public long Packet;
		public int ByteOffsetInPacket;
		public BytePacketTranslationFlags Flags;
	}

	[Flags]
	enum BytePacketTranslationFlags : uint  // Stored in UInt32 in AudioBytePacketTranslation - AudioFile.h
	{
		IsEstimate = 1
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFileSmpteTime { // AudioFile_SMPTE_Time
		public sbyte Hours;
		public byte Minutes;
		public byte Seconds;
		public byte Frames;
		public uint SubFrameSampleOffset;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFileMarker {
		public double FramePosition;
		internal IntPtr Name_cfstringref;
		public int MarkerID;
		public AudioFileSmpteTime SmpteTime;
		public AudioFileMarkerType Type;
		public ushort Reserved;
		public ushort Channel;

		public string? Name {
			get {
				return CFString.FromHandle (Name_cfstringref);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[iOS (13, 0)]
	[TV (13, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioPacketRangeByteCountTranslation {
		public long Packet;
		public long PacketCount;
		public long ByteCountUpperBound;
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[iOS (13, 0)]
	[TV (13, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioPacketRollDistanceTranslation {
		public long Packet;
		public long RollDistance;
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[iOS (13, 0)]
	[TV (13, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioIndependentPacketTranslation {
		public long Packet;
		public long IndependentlyDecodablePacket;
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[NoWatch]
	[iOS (13, 0)]
	[TV (13, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioPacketDependencyInfoTranslation {
		public long Packet;
		uint isIndependentlyDecodable;
		public uint NumberPrerollPackets;
		public bool IsIndependentlyDecodable {
			get { return isIndependentlyDecodable != 0; }
			set { isIndependentlyDecodable = (value) ? 1U : 0U; }
		}
	}

	public enum AudioFileMarkerType : uint // UInt32 in AudioFileMarkerType - AudioFile.h
	{
		Generic = 0,

		CAFProgramStart = 0x70626567,   // 'pbeg'
		CAFProgramEnd = 0x70656e64, // 'pend'
		CAFTrackStart = 0x74626567, // 'tbeg'
		CAFTrackEnd = 0x74656e54,   // 'tend'
		CAFIndex = 0x696e6478,  // 'indx'
		CAFRegionStart = 0x72626567,    // 'rbeg'
		CAFRegionEnd = 0x72626567,  // 'rend'
		CAFRegionSyncPoint = 0x72737963,    // 'rsyc'
		CAFSelectionStart = 0x73626567, // 'sbeg'
		CAFSelectionEnd = 0x73626567,   // 'send'
		CAFEditSourceBegin = 0x63626567,    // 'cbeg'
		CAFEditSourceEnd = 0x63626567,  // 'cend'
		CAFEditDestinationBegin = 0x64626567,   // 'dbeg'
		CAFEditDestinationEnd = 0x64626567, // 'dend'
		CAFSustainLoopStart = 0x736c6267,   // 'slbg'
		CAFSustainLoopEnd = 0x736c6265, // 'slen'
		CAFReleaseLoopStart = 0x726c6267,   // 'rlbg'
		CAFReleaseLoopEnd = 0x726c6265, // 'rlen'
		CAFSavedPlayPosition = 0x73706c79,  // 'sply'
		CAFTempo = 0x746d706f,  // 'tmpo'
		CAFTimeSignature = 0x74736967,  // 'tsig'
		CAFKeySignature = 0x6b736967,   // 'ksig'
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioFileMarkerList : IDisposable {
		IntPtr ptr;
		readonly bool owns;

		public AudioFileMarkerList (IntPtr ptr, bool owns)
		{
			this.ptr = ptr;
			this.owns = owns;
		}

		~AudioFileMarkerList ()
		{
			Dispose (false);
		}

		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		public uint Count {
			get {
				return (uint) Marshal.ReadInt32 (ptr, 4);
			}
		}

		public AudioFileMarker this [int index] {
			get {
				if (index >= Count || index < 0)
					throw new ArgumentOutOfRangeException (nameof (index));

				//
				// Decodes
				//
				// struct AudioFileMarkerList
				// {
				//	UInt32				mSMPTE_TimeType;
				//	UInt32				mNumberMarkers;
				//	AudioFileMarker		mMarkers[1]; // this is a variable length array of mNumberMarkers elements
				// }
				//
				unsafe {
					var ptr = (AudioFileMarker*) this.ptr + 2 * sizeof (int) + index * sizeof (AudioFileMarker);
					return *ptr;
				}
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!owns || ptr == IntPtr.Zero)
				return;

			for (int i = 0; i < Count; ++i) {
				CFObject.CFRelease (this [i].Name_cfstringref);
			}

			Marshal.FreeHGlobal (ptr);
			ptr = IntPtr.Zero;
			GC.SuppressFinalize (this);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFilePacketTableInfo {
		public long ValidFrames;
		public int PrimingFrames;
		public int RemainderFrames;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFileRegion {
		readonly IntPtr ptr;
		//
		// Wraps
		//
		// struct AudioFileRegion
		// {
		//	UInt32				mRegionID;
		//	CFStringRef			mName;
		//	UInt32				mFlags;
		//	UInt32				mNumberMarkers;
		//	AudioFileMarker		mMarkers[1]; // this is a variable length array of mNumberMarkers elements
		// }

		public AudioFileRegion (IntPtr ptr)
		{
			this.ptr = ptr;
		}

		public uint RegionID {
			get {
				return (uint) Marshal.ReadInt32 (ptr);
			}
		}

		public string? Name {
			get {
				return CFString.FromHandle (NameWeak);
			}
		}

		internal IntPtr NameWeak {
			get {
				return Marshal.ReadIntPtr (ptr, sizeof (uint));
			}
		}

		public unsafe AudioFileRegionFlags Flags {
			get {
				return (AudioFileRegionFlags) Marshal.ReadInt32 (ptr, sizeof (uint) + sizeof (IntPtr));
			}
		}

		public unsafe int Count {
			get {
				return Marshal.ReadInt32 (ptr, 2 * sizeof (uint) + sizeof (IntPtr));
			}
		}

		public AudioFileMarker this [int index] {
			get {
				if (index >= Count || index < 0)
					throw new ArgumentOutOfRangeException (nameof (index));

				unsafe {
					var ptr = (AudioFileMarker*) this.ptr + 3 * sizeof (int) + sizeof (IntPtr) + index * sizeof (AudioFileMarker);
					return *ptr;
				}
			}
		}

		internal unsafe int TotalSize {
			get {
				return Count * sizeof (AudioFileMarker);
			}
		}
	}

	[Flags]
	public enum AudioFileRegionFlags : uint // UInt32 in AudioFileRegion
	{
		LoopEnable = 1,
		PlayForward = 2,
		PlayBackward = 4
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioFileRegionList : IDisposable {
		IntPtr ptr;
		readonly bool owns;

		public AudioFileRegionList (IntPtr ptr, bool owns)
		{
			this.ptr = ptr;
			this.owns = owns;
		}

		~AudioFileRegionList ()
		{
			Dispose (false);
		}

		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		public uint Count {
			get {
				return (uint) Marshal.ReadInt32 (ptr, sizeof (uint));
			}
		}

		public AudioFileRegion this [int index] {
			get {
				if (index >= Count || index < 0)
					throw new ArgumentOutOfRangeException (nameof (index));

				//
				// Decodes
				//
				// struct AudioFileRegionList
				// {
				//	UInt32				mSMPTE_TimeType;
				//	UInt32				mNumberRegions;
				//	AudioFileRegion		mRegions[1]; // this is a variable length array of mNumberRegions elements
				// }
				//
				unsafe {
					var ptr = (byte*) this.ptr + 2 * sizeof (uint);
					for (int i = 0; i < index; ++i) {
						var region = new AudioFileRegion ((IntPtr) ptr);
						ptr += region.TotalSize;
					}

					return new AudioFileRegion ((IntPtr) ptr);
				}
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!owns || ptr == IntPtr.Zero)
				return;

			for (int i = 0; i < Count; ++i) {
				CFObject.CFRelease (this [i].NameWeak);
			}

			Marshal.FreeHGlobal (ptr);
			ptr = IntPtr.Zero;
			GC.SuppressFinalize (this);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioFile : DisposableObject {
		internal AudioFile ()
		{
			// This ctor is used by AudioSource that will set the handle later.
		}

#if !NET
		protected internal AudioFile (bool x)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal AudioFile (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileClose (AudioFileID handle);

		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns)
				AudioFileClose (Handle);
			base.Dispose (disposing);
		}

		public long Length {
			get {
				return GetLong (AudioFileProperty.AudioDataByteCount);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileCreateWithURL (IntPtr cfurlref_infile, AudioFileType inFileType, ref AudioStreamBasicDescription inFormat, AudioFileFlags inFlags, out AudioFileID file_id);

		public static AudioFile? Create (string url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			using (var cfurl = CFUrl.FromUrlString (url, null)!)
				return Create (cfurl, fileType, format, inFlags);
		}

		public static AudioFile? Create (CFUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			IntPtr h;

			if (AudioFileCreateWithURL (url.Handle, fileType, ref format, inFlags, out h) == 0)
				return new AudioFile (h, true);
			return null;
		}

		public static AudioFile? Create (NSUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			IntPtr h;

			if (AudioFileCreateWithURL (url.Handle, fileType, ref format, inFlags, out h) == 0)
				return new AudioFile (h, true);
			return null;
		}


		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileOpenURL (IntPtr cfurlref_infile, byte permissions, AudioFileType fileTypeHint, out IntPtr file_id);

		public static AudioFile? OpenRead (string url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		public static AudioFile? OpenRead (string url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		public static AudioFile? OpenRead (CFUrl url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		public static AudioFile? OpenRead (CFUrl url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		public static AudioFile? OpenRead (NSUrl url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		public static AudioFile? OpenRead (NSUrl url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		public static AudioFile? Open (string url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		public static AudioFile? Open (string url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			using (var cfurl = CFUrl.FromUrlString (url, null)!)
				return Open (cfurl, permissions, out error, fileTypeHint);
		}

		public static AudioFile? Open (CFUrl url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		public static AudioFile? Open (CFUrl url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return Open (url.Handle, permissions, fileTypeHint, out error);
		}

		public static AudioFile? Open (NSUrl url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		public static AudioFile? Open (NSUrl url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			return Open (url.Handle, permissions, fileTypeHint, out error);
		}

		static AudioFile? Open (IntPtr urlHandle, AudioFilePermission permissions, AudioFileType fileTypeHint, out AudioFileError error)
		{
			IntPtr file;
			error = AudioFileOpenURL (urlHandle, (byte) permissions, fileTypeHint, out file);
			if (error == AudioFileError.Success)
				return new AudioFile (file, true);
			return null;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileOptimize (AudioFileID handle);

		public bool Optimize ()
		{
			return AudioFileOptimize (Handle) == 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileReadBytes (AudioFileID inAudioFile, [MarshalAs (UnmanagedType.I1)] bool useCache, long startingByte, ref int numBytes, IntPtr outBuffer);

		public int Read (long startingByte, byte [] buffer, int offset, int count, bool useCache)
		{
			if (offset < 0)
				throw new ArgumentException (nameof (offset), "<0");
			if (count < 0)
				throw new ArgumentException (nameof (count), "<0");
			if (startingByte < 0)
				throw new ArgumentException (nameof (startingByte), "<0");
			int len = buffer.Length;
			if (offset > len)
				throw new ArgumentException ("destination offset is beyond array size");
			// reordered to avoid possible integer overflow
			if (offset > len - count)
				throw new ArgumentException ("Reading would overrun buffer");

			unsafe {
				fixed (byte* p = &buffer [offset]) {
					var res = AudioFileReadBytes (Handle, useCache, startingByte, ref count, (IntPtr) p);

					if (res == (int) AudioFileError.EndOfFile)
						return count <= 0 ? -1 : count;

					if (res == 0)
						return count;

					return -1;
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileWriteBytes (AudioFileID audioFile, [MarshalAs (UnmanagedType.I1)] bool useCache, long startingByte, ref int numBytes, IntPtr buffer);

		public int Write (long startingByte, byte [] buffer, int offset, int count, bool useCache)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset), "< 0");
			if (count < 0)
				throw new ArgumentOutOfRangeException (nameof (count), "< 0");
			if (offset > buffer.Length - count)
				throw new ArgumentException ("Reading would overrun buffer");

			unsafe {
				fixed (byte* p = &buffer [offset]) {
					if (AudioFileWriteBytes (Handle, useCache, startingByte, ref count, (IntPtr) p) == 0)
						return count;
					else
						return -1;
				}
			}
		}

		public int Write (long startingByte, byte [] buffer, int offset, int count, bool useCache, out int errorCode)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset), "< 0");
			if (count < 0)
				throw new ArgumentOutOfRangeException (nameof (count), "< 0");
			if (offset > buffer.Length - count)
				throw new ArgumentException ("Reading would overrun buffer");

			unsafe {
				fixed (byte* p = &buffer [offset]) {
					errorCode = AudioFileWriteBytes (Handle, useCache, startingByte, ref count, (IntPtr) p);
					if (errorCode == 0)
						return count;
					else
						return -1;
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileReadPacketData (
			AudioFileID audioFile, [MarshalAs (UnmanagedType.I1)] bool useCache, ref int numBytes,
			AudioStreamPacketDescription* packetDescriptions, long inStartingPacket, ref int numPackets, IntPtr outBuffer);

		public AudioStreamPacketDescription []? ReadPacketData (long inStartingPacket, int nPackets, byte [] buffer)
		{
			AudioFileError error;
			return ReadPacketData (inStartingPacket, nPackets, buffer, out error);
		}

		public AudioStreamPacketDescription []? ReadPacketData (long inStartingPacket, int nPackets, byte [] buffer, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			int count = buffer.Length;
			return RealReadPacketData (false, inStartingPacket, ref nPackets, buffer, 0, ref count, out error);
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count)
		{
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count);
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count, out AudioFileError error)
		{
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count, out error);
		}

		static internal AudioStreamPacketDescription []? PacketDescriptionFrom (int nPackets, IntPtr b)
		{
			if (b == IntPtr.Zero)
				return new AudioStreamPacketDescription [0];

			var ret = new AudioStreamPacketDescription [nPackets];
			int p = 0;
			for (int i = 0; i < nPackets; i++) {
				ret [i].StartOffset = Marshal.ReadInt64 (b, p);
				ret [i].VariableFramesInPacket = Marshal.ReadInt32 (b, p + 8);
				ret [i].DataByteSize = Marshal.ReadInt32 (b, p + 12);
				p += 16;
			}

			return ret;
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, byte [] buffer, int offset, ref int count)
		{
			AudioFileError error;
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count, out error);
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, byte [] buffer, int offset, ref int count, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new ArgumentException (nameof (offset), "<0");
			if (count < 0)
				throw new ArgumentException (nameof (count), "<0");
			int len = buffer.Length;
			if (offset > len)
				throw new ArgumentException ("destination offset is beyond array size");
			// reordered to avoid possible integer overflow
			if (offset > len - count)
				throw new ArgumentException ("Reading would overrun buffer");
			return RealReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count, out error);
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count)
		{
			AudioFileError error;
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, ref count, out error);
		}

		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count, out AudioFileError error)
		{
			var descriptions = new AudioStreamPacketDescription [nPackets];
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, ref count, out error, descriptions);
		}

		public unsafe AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count, out AudioFileError error, AudioStreamPacketDescription [] descriptions)
		{
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (count < 0)
				throw new ArgumentException (nameof (count), "<0");
			if (descriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (descriptions));

			fixed (AudioStreamPacketDescription* p = descriptions) {
				return RealReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, ref count, out error, descriptions);
			}
		}

		unsafe AudioStreamPacketDescription []? RealReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, byte [] buffer, int offset, ref int count, out AudioFileError error)
		{
			var descriptions = new AudioStreamPacketDescription [nPackets];
			fixed (byte* bop = &buffer [offset]) {
				fixed (AudioStreamPacketDescription* p = descriptions) {
					return RealReadPacketData (useCache, inStartingPacket, ref nPackets, (IntPtr) bop, ref count, out error, descriptions);
				}
			}
		}

		unsafe AudioStreamPacketDescription []? RealReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count, out AudioFileError error, AudioStreamPacketDescription [] descriptions)
		{
			OSStatus r;
			fixed (AudioStreamPacketDescription* pdesc = descriptions) {
				r = AudioFileReadPacketData (Handle, useCache, ref count, pdesc, inStartingPacket, ref nPackets, buffer);
			}

			error = (AudioFileError) r;

			if (r == (int) AudioFileError.EndOfFile) {
				if (count == 0)
					return null;
			} else if (r != 0) {
				return null;
			}

			if (descriptions.Length > nPackets) {
				// Didn't read as many descriptions as we requested.
				Array.Resize (ref descriptions, nPackets);
			}

			return descriptions;
		}

		public AudioStreamPacketDescription []? ReadFixedPackets (long inStartingPacket, int nPackets, byte [] buffer)
		{
			AudioFileError error;
			return ReadFixedPackets (inStartingPacket, nPackets, buffer, out error);
		}

		public AudioStreamPacketDescription []? ReadFixedPackets (long inStartingPacket, int nPackets, byte [] buffer, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			return RealReadFixedPackets (false, inStartingPacket, nPackets, buffer, 0, buffer.Length, out error);
		}

		public AudioStreamPacketDescription []? ReadFixedPackets (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count)
		{
			AudioFileError error;
			return ReadFixedPackets (useCache, inStartingPacket, nPackets, buffer, offset, count, out error);
		}

		public AudioStreamPacketDescription []? ReadFixedPackets (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new ArgumentException (nameof (offset), "<0");
			if (count < 0)
				throw new ArgumentException (nameof (count), "<0");
			int len = buffer.Length;
			if (offset > len)
				throw new ArgumentException ("destination offset is beyond array size");
			// reordered to avoid possible integer overflow
			if (offset > len - count)
				throw new ArgumentException ("Reading would overrun buffer");
			return RealReadFixedPackets (useCache, inStartingPacket, nPackets, buffer, offset, count, out error);
		}

		unsafe AudioStreamPacketDescription []? RealReadFixedPackets (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count, out AudioFileError error)
		{
			var descriptions = new AudioStreamPacketDescription [nPackets];
			fixed (byte* bop = &buffer [offset]) {
				OSStatus r;
				fixed (AudioStreamPacketDescription* pdesc = descriptions) {
					r = AudioFileReadPacketData (Handle, useCache, ref count, pdesc, inStartingPacket, ref nPackets, (IntPtr) bop);
				}
				error = (AudioFileError) r;
				if (r == (int) AudioFileError.EndOfFile) {
					if (count == 0)
						return null;
				} else if (r != 0) {
					return null;
				}
			}
			return descriptions;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileWritePackets (
			AudioFileID audioFile, [MarshalAs (UnmanagedType.I1)] bool useCache, int inNumBytes, AudioStreamPacketDescription []? inPacketDescriptions,
						long inStartingPacket, ref int numPackets, IntPtr buffer);

		public int WritePackets (bool useCache, long startingPacket, int numPackets, IntPtr buffer, int byteCount)
		{
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));

			if (AudioFileWritePackets (Handle, useCache, byteCount, null, startingPacket, ref numPackets, buffer) == 0)
				return numPackets;

			return -1;
		}

		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, IntPtr buffer, int byteCount)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			int nPackets = packetDescriptions.Length;
			if (AudioFileWritePackets (Handle, useCache, byteCount, packetDescriptions, startingPacket, ref nPackets, buffer) == 0)
				return nPackets;
			return -1;
		}

		unsafe public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, byte [] buffer, int offset, int byteCount)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset), "< 0");
			if (byteCount < 0)
				throw new ArgumentOutOfRangeException (nameof (byteCount), "< 0");
			if (offset > buffer.Length - byteCount)
				throw new ArgumentException ("Reading would overrun buffer");

			int nPackets = packetDescriptions.Length;
			fixed (byte* bop = &buffer [offset]) {
				if (AudioFileWritePackets (Handle, useCache, byteCount, packetDescriptions, startingPacket, ref nPackets, (IntPtr) bop) == 0)
					return nPackets;
				return -1;
			}
		}

		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, IntPtr buffer, int byteCount, out int errorCode)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer == IntPtr.Zero)
				throw new ArgumentException (nameof (buffer));
			int nPackets = packetDescriptions.Length;

			errorCode = (int) AudioFileWritePackets (Handle, useCache, byteCount, packetDescriptions, startingPacket, ref nPackets, buffer);
			if (errorCode == 0)
				return nPackets;
			return -1;
		}

		unsafe public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, byte [] buffer, int offset, int byteCount, out int errorCode)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset), "< 0");
			if (byteCount < 0)
				throw new ArgumentOutOfRangeException (nameof (byteCount), "< 0");
			if (offset > buffer.Length - byteCount)
				throw new ArgumentException ("Reading would overrun buffer");

			int nPackets = packetDescriptions.Length;
			fixed (byte* bop = &buffer [offset]) {
				errorCode = (int) AudioFileWritePackets (Handle, useCache, byteCount, packetDescriptions, startingPacket, ref nPackets, (IntPtr) bop);
				if (errorCode == 0)
					return nPackets;
				return -1;
			}
		}

		public AudioFileError WritePackets (bool useCache, int numBytes, AudioStreamPacketDescription [] packetDescriptions, long startingPacket, ref int numPackets, IntPtr buffer)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentException ("buffer");

			return AudioFileWritePackets (Handle, useCache, numBytes, packetDescriptions, startingPacket, ref numPackets, buffer);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileCountUserData (AudioFileID handle, uint userData, out int count);

		public int CountUserData (uint userData)
		{
			int count;
			if (AudioFileCountUserData (Handle, userData, out count) == 0)
				return count;
			return -1;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileGetUserDataSize (AudioFileID audioFile, uint userDataID, int index, out int userDataSize);
		public int GetUserDataSize (uint userDataId, int index)
		{
			int ds;

			if (AudioFileGetUserDataSize (Handle, userDataId, index, out ds) == 0)
				return -1;
			return ds;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileGetUserData (AudioFileID audioFile, int userDataID, int index, ref int userDataSize, IntPtr userData);

		public int GetUserData (int userDataID, int index, ref int size, IntPtr userData)
		{
			return AudioFileGetUserData (Handle, userDataID, index, ref size, userData);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileSetUserData (AudioFileID inAudioFile, int userDataID, int index, int userDataSize, IntPtr userData);

		public int SetUserData (int userDataId, int index, int userDataSize, IntPtr userData)
		{
			if (userData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (userData));
			return AudioFileSetUserData (Handle, userDataId, index, userDataSize, userData);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileRemoveUserData (AudioFileID audioFile, int userDataID, int index);

		public int RemoveUserData (int userDataId, int index)
		{
			return AudioFileRemoveUserData (Handle, userDataId, index);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileGetPropertyInfo (AudioFileID audioFile, AudioFileProperty propertyID, out int outDataSize, out int isWritable);

		public bool GetPropertyInfo (AudioFileProperty property, out int size, out int writable)
		{
			return AudioFileGetPropertyInfo (Handle, property, out size, out writable) == 0;
		}

		public bool IsPropertyWritable (AudioFileProperty property)
		{
			int writable;
			int size;
			return AudioFileGetPropertyInfo (Handle, property, out size, out writable) == 0 && writable != 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileGetProperty (AudioFileID audioFile, AudioFileProperty property, ref int dataSize, IntPtr outdata);

		public bool GetProperty (AudioFileProperty property, ref int dataSize, IntPtr outdata)
		{
			return AudioFileGetProperty (Handle, property, ref dataSize, outdata) == 0;
		}

		public IntPtr GetProperty (AudioFileProperty property, out int size)
		{
			int writable;

			var r = AudioFileGetPropertyInfo (Handle, property, out size, out writable);
			if (r != 0)
				return IntPtr.Zero;

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return IntPtr.Zero;

			r = AudioFileGetProperty (Handle, property, ref size, buffer);
			if (r == 0)
				return buffer;
			Marshal.FreeHGlobal (buffer);
			return IntPtr.Zero;
		}
#if NET
		unsafe T? GetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (AudioFileProperty property) where T : unmanaged
#else
		unsafe T? GetProperty<T> (AudioFileProperty property) where T : unmanaged
#endif
		{
			int size, writable;

			if (AudioFileGetPropertyInfo (Handle, property, out size, out writable) != 0)
				return null;
			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return null;
			try {
				var ptype = typeof (T);
				var r = AudioFileGetProperty (Handle, property, ref size, buffer);
				switch (ptype.Name) {
				case nameof (AudioFilePacketTableInfo):
					PacketTableInfoStatus = (AudioFileError) r;
					break;
				case nameof (AudioStreamBasicDescription):
					StreamBasicDescriptionStatus = (AudioFileError) r;
					break;
				}
				if (r == 0) {
					return Marshal.PtrToStructure<T> (buffer)!;
				}

				return null;
			} finally {
				Marshal.FreeHGlobal (buffer);
			}
		}

		int GetInt (AudioFileProperty property)
		{
			unsafe {
				int val = 0;
				int size = 4;
				if (AudioFileGetProperty (Handle, property, ref size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		IntPtr GetIntPtr (AudioFileProperty property)
		{
			unsafe {
				IntPtr val = IntPtr.Zero;
				int size = sizeof (IntPtr);
				if (AudioFileGetProperty (Handle, property, ref size, (IntPtr) (&val)) == 0)
					return val;
				return IntPtr.Zero;
			}
		}

		double GetDouble (AudioFileProperty property)
		{
			unsafe {
				double val = 0;
				int size = 8;
				if (AudioFileGetProperty (Handle, property, ref size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		long GetLong (AudioFileProperty property)
		{
			unsafe {
				long val = 0;
				int size = 8;
				if (AudioFileGetProperty (Handle, property, ref size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileSetProperty (AudioFileID audioFile, AudioFileProperty property, int dataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileSetProperty (AudioFileID audioFile, AudioFileProperty property, int dataSize, ref AudioFilePacketTableInfo propertyData);

		public bool SetProperty (AudioFileProperty property, int dataSize, IntPtr propertyData)
		{
			if (propertyData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyData));
			return AudioFileSetProperty (Handle, property, dataSize, propertyData) == 0;
		}

		void SetInt (AudioFileProperty property, int value)
		{
			unsafe {
				AudioFileSetProperty (Handle, property, 4, (IntPtr) (&value));
			}
		}

		unsafe AudioFileError SetDouble (AudioFileProperty property, double value)
		{
			return AudioFileSetProperty (Handle, property, sizeof (double), (IntPtr) (&value));
		}

		public AudioFileType FileType {
			get {
				return (AudioFileType) GetInt (AudioFileProperty.FileFormat);
			}
		}

		[Advice ("Use 'DataFormat' instead.")]
		public AudioStreamBasicDescription StreamBasicDescription {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat) ?? default (AudioStreamBasicDescription);
			}
		}

		public AudioFileError StreamBasicDescriptionStatus { get; private set; }

		public AudioStreamBasicDescription? DataFormat {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat);
			}
		}

		public AudioFormat []? AudioFormats {
			get {
				unsafe {
					int size;
					var r = GetProperty (AudioFileProperty.FormatList, out size);
					var records = (AudioFormat*) r;
					if (r == IntPtr.Zero)
						return null;
					int itemSize = sizeof (AudioFormat);
					int items = size / itemSize;
					var ret = new AudioFormat [items];

					for (int i = 0; i < items; i++)
						ret [i] = records [i];

					Marshal.FreeHGlobal (r);
					return ret;
				}
			}
		}

		public bool IsOptimized {
			get {
				return GetInt (AudioFileProperty.IsOptimized) == 1;
			}
		}

		public byte [] MagicCookie {
			get {
				int size;
				var h = GetProperty (AudioFileProperty.MagicCookieData, out size);
				if (h == IntPtr.Zero)
					return Array.Empty<byte> ();

				byte [] cookie = new byte [size];
				Marshal.Copy (h, cookie, 0, size);
				Marshal.FreeHGlobal (h);

				return cookie;
			}

			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				unsafe {
					fixed (byte* bp = value) {
						SetProperty (AudioFileProperty.MagicCookieData, value.Length, (IntPtr) bp);
					}
				}
			}
		}

		public long DataPacketCount {
			get {
				return GetLong (AudioFileProperty.AudioDataPacketCount);
			}
		}

		public int MaximumPacketSize {
			get {
				return GetInt (AudioFileProperty.MaximumPacketSize);
			}
		}

		public long DataOffset {
			get {
				return GetLong (AudioFileProperty.DataOffset);
			}
		}

		public NSData? AlbumArtwork {
			get {
				return Runtime.GetNSObject<NSData> (GetIntPtr (AudioFileProperty.AlbumArtwork));
			}
		}

		public AudioChannelLayout? ChannelLayout {
			get {
				int size;
				var h = GetProperty (AudioFileProperty.ChannelLayout, out size);
				if (h == IntPtr.Zero)
					return null;

				var layout = AudioChannelLayout.FromHandle (h);
				Marshal.FreeHGlobal (h);

				return layout;
			}
		}

		public bool DeferSizeUpdates {
			get {
				return GetInt (AudioFileProperty.DeferSizeUpdates) == 1;
			}
			set {
				SetInt (AudioFileProperty.DeferSizeUpdates, value ? 1 : 0);
			}
		}

		public int BitRate {
			get {
				return GetInt (AudioFileProperty.BitRate);
			}
		}

		public double EstimatedDuration {
			get {
				return GetDouble (AudioFileProperty.EstimatedDuration);
			}
		}

		public int PacketSizeUpperBound {
			get {
				return GetInt (AudioFileProperty.PacketSizeUpperBound);
			}
		}

		public double ReserveDuration {
			get {
				return GetDouble (AudioFileProperty.ReserveDuration);
			}
		}

		public AudioFileMarkerList? MarkerList {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.MarkerList, out size, out writable);
				if (size == 0)
					return null;

				IntPtr ptr = Marshal.AllocHGlobal (size);
				if (AudioFileGetProperty (Handle, AudioFileProperty.MarkerList, ref size, (IntPtr) ptr) != 0) {
					Marshal.FreeHGlobal (ptr);
					return null;
				}

				return new AudioFileMarkerList (ptr, true);
			}
		}

		public AudioFileRegionList? RegionList {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.RegionList, out size, out writable);
				if (size == 0)
					return null;

				IntPtr ptr = Marshal.AllocHGlobal (size);
				if (AudioFileGetProperty (Handle, AudioFileProperty.RegionList, ref size, (IntPtr) ptr) != 0) {
					Marshal.FreeHGlobal (ptr);
					return null;
				}

				return new AudioFileRegionList (ptr, true);
			}
		}

		public AudioFileError PacketTableInfoStatus { get; private set; }

		public unsafe AudioFilePacketTableInfo? PacketTableInfo {
			get {
				return GetProperty<AudioFilePacketTableInfo> (AudioFileProperty.PacketTableInfo);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				AudioFilePacketTableInfo afpti = value.Value;
				var res = AudioFileSetProperty (Handle, AudioFileProperty.PacketTableInfo, sizeof (AudioFilePacketTableInfo), ref afpti);
				if (res != 0)
					throw new ArgumentException (res.ToString ());
			}
		}

		public unsafe AudioFileChunkType []? ChunkIDs {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.ChunkIDs, out size, out writable);
				if (size == 0)
					return null;

				var data = new AudioFileChunkType [size / sizeof (AudioFileChunkType)];
				fixed (AudioFileChunkType* ptr = data) {
					if (AudioFileGetProperty (Handle, AudioFileProperty.ChunkIDs, ref size, (IntPtr) ptr) != 0)
						return null;

					return data;
				}
			}
		}

		public unsafe byte []? ID3Tag {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.ID3Tag, out size, out writable);
				if (size == 0)
					return null;

				var data = new byte [size];
				fixed (byte* ptr = data) {
					if (AudioFileGetProperty (Handle, AudioFileProperty.ID3Tag, ref size, (IntPtr) ptr) != 0)
						return null;

					return data;
				}
			}
		}

		public AudioFileInfoDictionary? InfoDictionary {
			get {
				var ptr = GetIntPtr (AudioFileProperty.InfoDictionary);
				if (ptr == IntPtr.Zero)
					return null;

				return new AudioFileInfoDictionary (new NSMutableDictionary (ptr, true));
			}
		}

		public long PacketToFrame (long packet)
		{
			AudioFramePacketTranslation buffer;
			buffer.Packet = packet;

			unsafe {
				AudioFramePacketTranslation* p = &buffer;
				int size = sizeof (AudioFramePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.PacketToFrame, ref size, (IntPtr) p) == 0)
					return buffer.Frame;
				return -1;
			}
		}

		public long FrameToPacket (long frame, out int frameOffsetInPacket)
		{
			AudioFramePacketTranslation buffer;
			buffer.Frame = frame;

			unsafe {
				AudioFramePacketTranslation* p = &buffer;
				int size = sizeof (AudioFramePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.FrameToPacket, ref size, (IntPtr) p) == 0) {
					frameOffsetInPacket = buffer.FrameOffsetInPacket;
					return buffer.Packet;
				}
				frameOffsetInPacket = 0;
				return -1;
			}
		}

		public long PacketToByte (long packet, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer;
			buffer.Packet = packet;

			unsafe {
				AudioBytePacketTranslation* p = &buffer;
				int size = sizeof (AudioBytePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.PacketToByte, ref size, (IntPtr) p) == 0) {
					isEstimate = (buffer.Flags & BytePacketTranslationFlags.IsEstimate) != 0;
					return buffer.Byte;
				}
				isEstimate = false;
				return -1;
			}
		}

		public long ByteToPacket (long byteval, out int byteOffsetInPacket, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer;
			buffer.Byte = byteval;

			unsafe {
				AudioBytePacketTranslation* p = &buffer;
				int size = sizeof (AudioBytePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.ByteToPacket, ref size, (IntPtr) p) == 0) {
					isEstimate = (buffer.Flags & BytePacketTranslationFlags.IsEstimate) != 0;
					byteOffsetInPacket = buffer.ByteOffsetInPacket;
					return buffer.Packet;
				}
				byteOffsetInPacket = 0;
				isEstimate = false;
				return -1;
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class AudioFileInfoDictionary : DictionaryContainer {
		internal AudioFileInfoDictionary (NSDictionary dict)
			: base (dict)
		{
		}

		public string? Album {
			get {
				return GetStringValue ("album");
			}
		}

		public string? ApproximateDurationInSeconds {
			get {
				return GetStringValue ("approximate duration in seconds");
			}
		}

		public string? Artist {
			get {
				return GetStringValue ("artist");
			}
		}

		public string? ChannelLayout {
			get {
				return GetStringValue ("channel layout");
			}
		}

		public string? Composer {
			get {
				return GetStringValue ("composer");
			}
		}

		public string? Comments {
			get {
				return GetStringValue ("comments");
			}
		}

		public string? Copyright {
			get {
				return GetStringValue ("copyright");
			}
		}

		public string? EncodingApplication {
			get {
				return GetStringValue ("encoding application");
			}
		}

		public string? Genre {
			get {
				return GetStringValue ("genre");
			}
		}

		public string? ISRC {
			get {
				return GetStringValue ("ISRC");
			}
		}

		public string? KeySignature {
			get {
				return GetStringValue ("key signature");
			}
		}

		public string? Lyricist {
			get {
				return GetStringValue ("lyricist");
			}
		}

		public string? NominalBitRate {
			get {
				return GetStringValue ("nominal bit rate");
			}
		}

		public string? RecordedDate {
			get {
				return GetStringValue ("recorded date");
			}
		}

		public string? SourceBitDepth {
			get {
				return GetStringValue ("source bit depth");
			}
		}

		public string? SourceEncoder {
			get {
				return GetStringValue ("source encoder");
			}
		}

		public string? SubTitle {
			get {
				return GetStringValue ("subtitle");
			}
		}

		public string? Tempo {
			get {
				return GetStringValue ("tempo");
			}
		}

		public string? TimeSignature {
			get {
				return GetStringValue ("time signature");
			}
		}

		public string? Title {
			get {
				return GetStringValue ("title");
			}
		}

		public string? TrackNumber {
			get {
				return GetStringValue ("track number");
			}
		}

		public string? Year {
			get {
				return GetStringValue ("year");
			}
		}
	}

	delegate int ReadProc (IntPtr clientData, long position, int requestCount, IntPtr buffer, out int actualCount);
	delegate int WriteProc (IntPtr clientData, long position, int requestCount, IntPtr buffer, out int actualCount);
	delegate long GetSizeProc (IntPtr clientData);
	delegate int SetSizeProc (IntPtr clientData, long size);

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public abstract class AudioSource : AudioFile {
		GCHandle gch;
#if !NET
		static ReadProc dRead;
		static WriteProc dWrite;
		static GetSizeProc dGetSize;
		static SetSizeProc dSetSize;

		static AudioSource ()
		{
			dRead = SourceRead;
			dWrite = SourceWrite;
			dGetSize = SourceGetSize;
			dSetSize = SourceSetSize;
		}
#endif

#if NET
		[UnmanagedCallersOnly]
		static unsafe int SourceRead (IntPtr clientData, long inPosition, int requestCount, IntPtr buffer, int* actualCount)
#else
		[MonoPInvokeCallback (typeof (ReadProc))]
		static int SourceRead (IntPtr clientData, long inPosition, int requestCount, IntPtr buffer, out int actualCount)
#endif
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
#if NET
			var localCount = 0;
			var result = audioSource?.Read (inPosition, requestCount, buffer, out localCount) == true ? 0 : 1;
			*actualCount = localCount;
			return result;
#else
			actualCount = 0;
			return audioSource?.Read (inPosition, requestCount, buffer, out actualCount) == true ? 0 : 1;
#endif
		}

		public abstract bool Read (long position, int requestCount, IntPtr buffer, out int actualCount);

#if NET
		[UnmanagedCallersOnly]
		static unsafe int SourceWrite (IntPtr clientData, long position, int requestCount, IntPtr buffer, int* actualCount)
#else
		[MonoPInvokeCallback (typeof (WriteProc))]
		static int SourceWrite (IntPtr clientData, long position, int requestCount, IntPtr buffer, out int actualCount)
#endif
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
#if NET
			var localCount = 0;
			var result = audioSource?.Write (position, requestCount, buffer, out localCount) == true ? 0 : 1;
			*actualCount = localCount;
			return result;
#else
			actualCount = 0;
			return audioSource?.Write (position, requestCount, buffer, out actualCount) == true ? 0 : 1;
#endif
		}
		public abstract bool Write (long position, int requestCount, IntPtr buffer, out int actualCount);

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (GetSizeProc))]
#endif
		static long SourceGetSize (IntPtr clientData)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
			return audioSource?.Size ?? 0;
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (SetSizeProc))]
#endif
		static int SourceSetSize (IntPtr clientData, long size)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;

			if (audioSource is not null)
				audioSource.Size = size;
			return 0;
		}
		public abstract long Size { get; set; }

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (gch.IsAllocated)
				gch.Free ();
		}

		[DllImport (Constants.AudioToolboxLibrary)]
#if NET
		extern unsafe static OSStatus AudioFileInitializeWithCallbacks (
			IntPtr inClientData,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inReadFunc,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inWriteFunc,
			delegate* unmanaged<IntPtr, long> inGetSizeFunc,
			delegate* unmanaged<IntPtr, long, int> inSetSizeFunc,
			AudioFileType inFileType, AudioStreamBasicDescription* format, uint flags, IntPtr* id);
#else
		extern static OSStatus AudioFileInitializeWithCallbacks (
			IntPtr inClientData, ReadProc inReadFunc, WriteProc inWriteFunc, GetSizeProc inGetSizeFunc, SetSizeProc inSetSizeFunc,
			AudioFileType inFileType, ref AudioStreamBasicDescription format, uint flags, out IntPtr id);
#endif

		public AudioSource (AudioFileType inFileType, AudioStreamBasicDescription format)
		{
			Initialize (inFileType, format);
		}

		public AudioSource ()
		{
		}

		protected void Initialize (AudioFileType inFileType, AudioStreamBasicDescription format)
		{
			gch = GCHandle.Alloc (this);
#if NET
			int code = 0;
			IntPtr handle = IntPtr.Zero;
			unsafe {
				code = AudioFileInitializeWithCallbacks (GCHandle.ToIntPtr (gch), &SourceRead, &SourceWrite, &SourceGetSize, &SourceSetSize, inFileType, &format, 0, &handle);
			}
#else
			var code = AudioFileInitializeWithCallbacks (GCHandle.ToIntPtr (gch), dRead, dWrite, dGetSize, dSetSize, inFileType, ref format, 0, out var handle);
#endif
			if (code == 0) {
				InitializeHandle (handle);
				return;
			}
			throw new Exception (String.Format ("Unable to create AudioSource, code: 0x{0:x}", code));
		}

#if NET
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static unsafe int AudioFileOpenWithCallbacks (
			IntPtr inClientData,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inReadFunc,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inWriteFunc,
			delegate* unmanaged<IntPtr, long> inGetSizeFunc,
			delegate* unmanaged<IntPtr, long, int> inSetSizeFunc,
			AudioFileType inFileTypeHint, IntPtr* outAudioFile);
#else
		[DllImport (Constants.AudioToolboxLibrary)]
		extern static int AudioFileOpenWithCallbacks (
			IntPtr inClientData, ReadProc inReadFunc, WriteProc inWriteFunc,
			GetSizeProc inGetSizeFunc, SetSizeProc inSetSizeFunc, AudioFileType inFileTypeHint, out IntPtr outAudioFile);
#endif

		public AudioSource (AudioFileType fileTypeHint)
		{
			Open (fileTypeHint);
		}

		protected void Open (AudioFileType fileTypeHint)
		{
			gch = GCHandle.Alloc (this);
#if NET
			int code = 0;
			IntPtr handle = IntPtr.Zero;
			unsafe {
				code = AudioFileOpenWithCallbacks (GCHandle.ToIntPtr (gch), &SourceRead, &SourceWrite, &SourceGetSize, &SourceSetSize, fileTypeHint, &handle);
			}
#else
			var code = AudioFileOpenWithCallbacks (GCHandle.ToIntPtr (gch), dRead, dWrite, dGetSize, dSetSize, fileTypeHint, out var handle);
#endif
			if (code == 0) {
				InitializeHandle (handle);
				return;
			}
			throw new Exception (String.Format ("Unable to create AudioSource, code: 0x{0:x}", code));
		}
	}
}

