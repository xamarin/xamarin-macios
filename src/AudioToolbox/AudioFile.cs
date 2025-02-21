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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ObjCRuntime;
using CoreFoundation;
using Foundation;

using AudioFileID = System.IntPtr;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AudioToolbox {

	public enum AudioFileType {  // UInt32 AudioFileTypeID
		/// <summary>Audio Interchange File Format.</summary>
		AIFF = 0x41494646, // AIFF
		/// <summary>Compressed Audio Interchange File Format.</summary>
		AIFC = 0x41494643, // AIFC
		/// <summary>Microsoft WAVE format.</summary>
		WAVE = 0x57415645, // WAVE
#if NET
		/// <summary>BWF-compatible RF64 multichannel sound format.</summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		RF64 = 0x52463634, // RF64
		/// <summary>Sound Designer 2 file.</summary>
		SoundDesigner2 = 0x53643266, // Sd2f
		/// <summary>NeXT/Sun audio file format</summary>
		Next = 0x4e655854, // NeXT
		/// <summary>MPEG-1 Audio Layer 3.</summary>
		MP3 = 0x4d504733, // MPG3
		/// <summary>MPEG-1 Audio Layer 2.</summary>
		MP2 = 0x4d504732, // MPG2
		/// <summary>MPEG-1 Audio Layer 1, largely outdated</summary>
		MP1 = 0x4d504731, // MPG1
		/// <summary>Digital Audio Compression Standard (also known as Dolby Digital or Audio Codec 3)</summary>
		AC3 = 0x61632d33, // ac-3
		/// <summary>Audio Transport Stream, a contains for Advanced Audio Coding (AAC) data.</summary>
		AAC_ADTS = 0x61647473, // adts
		/// <summary>MPEG-4 file.</summary>
		MPEG4 = 0x6d703466, // mp4f
		/// <summary>MPEG-4 Audio Layer with no bookmark metadata (use M4B for that).</summary>
		M4A = 0x6d346166, // m4af
		/// <summary>MPEG-4 Audio Layer with metadata for bookmarks, chapter markers, images and hyperlinks.</summary>
		M4B = 0x6d346266, // m4bf
		/// <summary>Apple Core Audio Format.   CAF files are containers that can contain multiple audio formats, metadata tracks.   Uses 64-bit offsetes, so it is not limited to 4GB.</summary>
		CAF = 0x63616666, // caff
		/// <summary>3GP (3GPP file format) is a multimedia container format defined by the Third Generation Partnership Project (3GPP) for 3G UMTS multimedia services. It is used on 3G mobile phones but can also be played on some 2G and 4G phones.</summary>
		ThreeGP = 0x33677070, // 3gpp
		/// <summary>3G2 (3GPP2 file format) is a multimedia container format defined by the 3GPP2 for 3G CDMA2000 multimedia services. It is very similar to the 3GP file format, but has some extensions and limitations in comparison to 3GP.</summary>
		ThreeGP2 = 0x33677032, // 3gp2
		/// <summary>Adaptive Multi-Rate format, optimized for speech coding   Used widely in GSM an UMTS.</summary>
		AMR = 0x616d7266, // amrf
#if NET
		/// <summary>Free Lossless Audio Codec format.</summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		FLAC = 0x666c6163, // flac
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		LatmInLoas = 0x6c6f6173, // loas
	}

	public enum AudioFileError {// Implictly cast to OSType in AudioFile.h
		/// <summary>To be added.</summary>
		Success = 0, // noErr
		/// <summary>An unspecified error has occurred.</summary>
		Unspecified = 0x7768743f, // wht?
		/// <summary>The file type is not supported.</summary>
		UnsupportedFileType = 0x7479703f, // typ?
		/// <summary>The data format is not supported.</summary>
		UnsupportedDataFormat = 0x666d743f, // fmt?
		/// <summary>The property is not supported.</summary>
		UnsupportedProperty = 0x7074793f, // pty?
		/// <summary>The size of the property data was invalid.</summary>
		BadPropertySize = 0x2173697a, // !siz
		/// <summary>To be added.</summary>
		Permissions = 0x70726d3f, // prm?
		/// <summary>The file must be optimized in order to write more audio data.</summary>
		NotOptimized = 0x6f70746d, // optm
		/// <summary>The chunk does not exist or is not supported by the file.</summary>
		InvalidChunk = 0x63686b3f, // chk?
		/// <summary>The a file offset was too large for the file type.</summary>
		DoesNotAllow64BitDataSize = 0x6f66663f, // off?
		/// <summary>A packet offset is not valid.</summary>
		InvalidPacketOffset = 0x70636b3f, // pck?
		/// <summary>The file is invalid.</summary>
		InvalidFile = 0x6474613f, // dta?
		/// <summary>To be added.</summary>
		OperationNotSupported = 0x6F703F3F, // op??
		/// <summary>The file is not opened.</summary>
		FileNotOpen = -38,
		/// <summary>The end of file.</summary>
		EndOfFile = -39,
		/// <summary>File not found.</summary>
		FileNotFound = -43,
		/// <summary>Invalid file position.</summary>
		FilePosition = -40,
	}

	[Flags]
	public enum AudioFilePermission {
		/// <summary>To be added.</summary>
		Read = 0x01,
		/// <summary>To be added.</summary>
		Write = 0x02,
		/// <summary>To be added.</summary>
		ReadWrite = 0x03
	}

	[Flags]
	public enum AudioFileFlags { // UInt32 in AudioFileCreateWithURL()
		/// <summary>To be added.</summary>
		EraseFlags = 1,
		/// <summary>If this flag is set, audio data will be written without page alignment. This will make the data more compact but possibly slow readout.</summary>
		DontPageAlignAudioData = 2
	}

	public enum AudioFileProperty { // typedef UInt32 AudioFilePropertyID
		/// <summary>To be added.</summary>
		FileFormat = 0x66666d74,
		/// <summary>To be added.</summary>
		DataFormat = 0x64666d74,
		/// <summary>To be added.</summary>
		IsOptimized = 0x6f70746d,
		/// <summary>To be added.</summary>
		MagicCookieData = 0x6d676963,
		/// <summary>To be added.</summary>
		AudioDataByteCount = 0x62636e74,
		/// <summary>To be added.</summary>
		AudioDataPacketCount = 0x70636e74,
		/// <summary>To be added.</summary>
		MaximumPacketSize = 0x70737a65,
		/// <summary>To be added.</summary>
		DataOffset = 0x646f6666,
		/// <summary>To be added.</summary>
		ChannelLayout = 0x636d6170,
		/// <summary>To be added.</summary>
		DeferSizeUpdates = 0x64737a75,
		/// <summary>To be added.</summary>
		DataFormatName = 0x666e6d65,
		/// <summary>To be added.</summary>
		MarkerList = 0x6d6b6c73,
		/// <summary>To be added.</summary>
		RegionList = 0x72676c73,
		/// <summary>To be added.</summary>
		PacketToFrame = 0x706b6672,
		/// <summary>To be added.</summary>
		FrameToPacket = 0x6672706b,
		/// <summary>To be added.</summary>
		PacketToByte = 0x706b6279,
		/// <summary>To be added.</summary>
		ByteToPacket = 0x6279706b,
		/// <summary>To be added.</summary>
		ChunkIDs = 0x63686964,
		/// <summary>To be added.</summary>
		InfoDictionary = 0x696e666f,
		/// <summary>To be added.</summary>
		PacketTableInfo = 0x706e666f,
		/// <summary>To be added.</summary>
		FormatList = 0x666c7374,
		/// <summary>To be added.</summary>
		PacketSizeUpperBound = 0x706b7562,
		/// <summary>To be added.</summary>
		ReserveDuration = 0x72737276,
		/// <summary>To be added.</summary>
		EstimatedDuration = 0x65647572,
		/// <summary>To be added.</summary>
		BitRate = 0x62726174,
		/// <summary>To be added.</summary>
		ID3Tag = 0x69643374,
		/// <summary>To be added.</summary>
		SourceBitDepth = 0x73627464,
		/// <summary>To be added.</summary>
		AlbumArtwork = 0x61617274,
		/// <summary>To be added.</summary>
		ReadyToProducePackets = 0x72656479,
		/// <summary>The average number of bytes per audio packet.</summary>
		AverageBytesPerPacket = 0x61627070,
		/// <summary>To be added.</summary>
		AudioTrackCount = 0x61746374,
		/// <summary>To be added.</summary>
		UseAudioTrack = 0x7561746b,
	}

	public enum AudioFileLoopDirection { // Unused?
		/// <summary>To be added.</summary>
		NoLooping = 0,
		/// <summary>To be added.</summary>
		Forward = 1,
		/// <summary>To be added.</summary>
		ForwardAndBackward = 2,
		/// <summary>To be added.</summary>
		Backward = 3
	}

	public enum AudioFileChunkType : uint // CoreAudio.framework - CoreAudioTypes.h - "four char code IDs"
	{
		/// <summary>To be added.</summary>
		CAFStreamDescription = 0x64657363,  // 'desc'
		/// <summary>To be added.</summary>
		CAFAudioData = 0x64617461,  // 'data'
		/// <summary>To be added.</summary>
		CAFChannelLayout = 0x6368616e,  // 'chan'
		/// <summary>To be added.</summary>
		CAFFiller = 0x66726565, // 'free'
		/// <summary>To be added.</summary>
		CAFMarker = 0x6d61726b, // 'mark'
		/// <summary>To be added.</summary>
		CAFRegion = 0x7265676e, // 'regn'
		/// <summary>To be added.</summary>
		CAFInstrument = 0x696e7374, // 'inst'
		/// <summary>To be added.</summary>
		CAFMagicCookieID = 0x6b756b69,  // 'kuki'
		/// <summary>To be added.</summary>
		CAFInfoStrings = 0x696e666f,    // 'info'
		/// <summary>To be added.</summary>
		CAFEditComments = 0x65646374,   // 'edct'
		/// <summary>To be added.</summary>
		CAFPacketTable = 0x70616b74,    // 'pakt'
		/// <summary>To be added.</summary>
		CAFStrings = 0x73747267,    // 'strg'
		/// <summary>To be added.</summary>
		CAFUUID = 0x75756964,   // 'uuid'
		/// <summary>To be added.</summary>
		CAFPeak = 0x7065616b,   // 'peak'
		/// <summary>To be added.</summary>
		CAFOverview = 0x6f767677,   // 'ovvw'
		/// <summary>To be added.</summary>
		CAFMIDI = 0x6d696469,   // 'midi'
		/// <summary>To be added.</summary>
		CAFUMID = 0x756d6964,   // 'umid'
		/// <summary>To be added.</summary>
		CAFFormatListID = 0x6c647363,   // 'ldsc'
		/// <summary>To be added.</summary>
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
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public sbyte Hours;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public byte Minutes;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public byte Seconds;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public byte Frames;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double FramePosition;
		internal IntPtr Name_cfstringref;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public int MarkerID;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public AudioFileSmpteTime SmpteTime;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public AudioFileMarkerType Type;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public ushort Reserved;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public ushort Channel;

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		Generic = 0,

		/// <summary>To be added.</summary>
		CAFProgramStart = 0x70626567,   // 'pbeg'
		/// <summary>To be added.</summary>
		CAFProgramEnd = 0x70656e64, // 'pend'
		/// <summary>To be added.</summary>
		CAFTrackStart = 0x74626567, // 'tbeg'
		/// <summary>To be added.</summary>
		CAFTrackEnd = 0x74656e54,   // 'tend'
		/// <summary>To be added.</summary>
		CAFIndex = 0x696e6478,  // 'indx'
		/// <summary>To be added.</summary>
		CAFRegionStart = 0x72626567,    // 'rbeg'
		/// <summary>To be added.</summary>
		CAFRegionEnd = 0x72626567,  // 'rend'
		/// <summary>To be added.</summary>
		CAFRegionSyncPoint = 0x72737963,    // 'rsyc'
		/// <summary>To be added.</summary>
		CAFSelectionStart = 0x73626567, // 'sbeg'
		/// <summary>To be added.</summary>
		CAFSelectionEnd = 0x73626567,   // 'send'
		/// <summary>To be added.</summary>
		CAFEditSourceBegin = 0x63626567,    // 'cbeg'
		/// <summary>To be added.</summary>
		CAFEditSourceEnd = 0x63626567,  // 'cend'
		/// <summary>To be added.</summary>
		CAFEditDestinationBegin = 0x64626567,   // 'dbeg'
		/// <summary>To be added.</summary>
		CAFEditDestinationEnd = 0x64626567, // 'dend'
		/// <summary>To be added.</summary>
		CAFSustainLoopStart = 0x736c6267,   // 'slbg'
		/// <summary>To be added.</summary>
		CAFSustainLoopEnd = 0x736c6265, // 'slen'
		/// <summary>To be added.</summary>
		CAFReleaseLoopStart = 0x726c6267,   // 'rlbg'
		/// <summary>To be added.</summary>
		CAFReleaseLoopEnd = 0x726c6265, // 'rlen'
		/// <summary>To be added.</summary>
		CAFSavedPlayPosition = 0x73706c79,  // 'sply'
		/// <summary>To be added.</summary>
		CAFTempo = 0x746d706f,  // 'tmpo'
		/// <summary>To be added.</summary>
		CAFTimeSignature = 0x74736967,  // 'tsig'
		/// <summary>To be added.</summary>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public long ValidFrames;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public int PrimingFrames;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public uint RegionID {
			get {
				return (uint) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public unsafe AudioFileRegionFlags Flags {
			get {
				return (AudioFileRegionFlags) Marshal.ReadInt32 (ptr, sizeof (uint) + sizeof (IntPtr));
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>In conjunction with at least one other flag, loops the region.</summary>
		LoopEnable = 1,
		/// <summary>The region is played normally.</summary>
		PlayForward = 2,
		/// <summary>The region is played in reverse.</summary>
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>Audio file size, in bytes.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		public long Length {
			get {
				return GetLong (AudioFileProperty.AudioDataByteCount);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileCreateWithURL (IntPtr cfurlref_infile, AudioFileType inFileType, AudioStreamBasicDescription* inFormat, AudioFileFlags inFlags, AudioFileID* file_id);

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

			var h = default (IntPtr);

			unsafe {
				if (AudioFileCreateWithURL (url.Handle, fileType, &format, inFlags, &h) == 0)
					return new AudioFile (h, true);
			}
			return null;
		}

		public static AudioFile? Create (NSUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			var h = default (IntPtr);

			unsafe {
				if (AudioFileCreateWithURL (url.Handle, fileType, &format, inFlags, &h) == 0)
					return new AudioFile (h, true);
			}
			return null;
		}


		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileOpenURL (IntPtr cfurlref_infile, byte permissions, AudioFileType fileTypeHint, IntPtr* file_id);

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
			var file = default (IntPtr);
			unsafe {
				error = AudioFileOpenURL (urlHandle, (byte) permissions, fileTypeHint, &file);
			}
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
		unsafe extern static OSStatus AudioFileReadBytes (AudioFileID inAudioFile, byte useCache, long startingByte, int* numBytes, IntPtr outBuffer);

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
					var res = AudioFileReadBytes (Handle, useCache ? (byte) 1 : (byte) 0, startingByte, &count, (IntPtr) p);

					if (res == (int) AudioFileError.EndOfFile)
						return count <= 0 ? -1 : count;

					if (res == 0)
						return count;

					return -1;
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileWriteBytes (AudioFileID audioFile, byte useCache, long startingByte, int* numBytes, IntPtr buffer);

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
					if (AudioFileWriteBytes (Handle, useCache ? (byte) 1 : (byte) 0, startingByte, &count, (IntPtr) p) == 0)
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
					errorCode = AudioFileWriteBytes (Handle, useCache ? (byte) 1 : (byte) 0, startingByte, &count, (IntPtr) p);
					if (errorCode == 0)
						return count;
					else
						return -1;
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileReadPacketData (
			AudioFileID audioFile, byte useCache, int* numBytes,
			AudioStreamPacketDescription* packetDescriptions, long inStartingPacket, int* numPackets, IntPtr outBuffer);

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
				r = AudioFileReadPacketData (Handle,
						useCache ? (byte) 1 : (byte) 0,
						(int*) Unsafe.AsPointer<int> (ref count),
						pdesc,
						inStartingPacket,
						(int*) Unsafe.AsPointer<int> (ref nPackets),
						buffer);
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
					r = AudioFileReadPacketData (Handle, useCache ? (byte) 1 : (byte) 0, &count, pdesc, inStartingPacket, &nPackets, (IntPtr) bop);
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
		unsafe extern static AudioFileError AudioFileWritePackets (
			AudioFileID audioFile, byte useCache, int inNumBytes, AudioStreamPacketDescription* inPacketDescriptions,
						long inStartingPacket, int* numPackets, IntPtr buffer);

		public int WritePackets (bool useCache, long startingPacket, int numPackets, IntPtr buffer, int byteCount)
		{
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));

			unsafe {
				if (AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, null, startingPacket, &numPackets, buffer) == 0)
					return numPackets;
			}

			return -1;
		}

		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, IntPtr buffer, int byteCount)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			int nPackets = packetDescriptions.Length;
			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					if (AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, buffer) == 0)
						return nPackets;
				}
			}
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
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					if (AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, (IntPtr) bop) == 0)
						return nPackets;
				}
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

			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					errorCode = (int) AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, buffer);
				}
			}
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
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					errorCode = (int) AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, (IntPtr) bop);
				}
				if (errorCode == 0)
					return nPackets;
				return -1;
			}
		}

		public AudioFileError WritePackets (bool useCache, int numBytes, AudioStreamPacketDescription [] packetDescriptions, long startingPacket, ref int numPackets, IntPtr buffer)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentException ("buffer");

			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					return AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, numBytes, packetDescriptionsPtr, startingPacket, (int*) Unsafe.AsPointer<int> (ref numPackets), buffer);
				}
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileCountUserData (AudioFileID handle, uint userData, int* count);

		/// <summary>Get the number of user data for the specified chunk type.</summary>
		/// <param name="userData">The fourcc of the ID whose count to retrieve.</param>
		/// <returns>The number of user udata for the specified ID.</returns>
		public int CountUserData (uint userData)
		{
			int count;
			unsafe {
				if (AudioFileCountUserData (Handle, userData, &count) == 0)
					return count;
			}
			return -1;
		}

		/// <summary>Get the number of user data for the specified chunk type.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <returns>The number of user data for the specified chunk type.</returns>
		public int CountUserData (AudioFileChunkType chunkType)
		{
			return CountUserData ((uint) chunkType);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserDataSize (AudioFileID audioFile, uint userDataID, int index, int* userDataSize);

		/// <summary>Get the size of the specified user data.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <returns>Returns the (non-negative) size on success, otherwise -1.</returns>
		public int GetUserDataSize (uint userDataId, int index)
		{
			int ds;

			unsafe {
				if (AudioFileGetUserDataSize (Handle, userDataId, index, &ds) != 0)
					return -1;
			}
			return ds;
		}

		/// <summary>Get the size of the specified user data.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <returns>Returns the (non-negative) size on success, otherwise -1.</returns>
		public int GetUserDataSize (AudioFileChunkType chunkType, int index)
		{
			return GetUserDataSize ((uint) chunkType, index);
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserDataSize64 (AudioFileID audioFile, uint userDataID, int index, ulong* userDataSize);

		/// <summary>Get the 64-bit size of the specified user data.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="size">The retrieved 64-bit size of the specified user data.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserDataSize (uint userDataId, int index, out ulong size)
		{
			size = 0;
			unsafe {
				return (AudioFileError) AudioFileGetUserDataSize64 (Handle, userDataId, index, (ulong*) Unsafe.AsPointer<ulong> (ref size));
			}
		}

		/// <summary>Get the 64-bit size of the specified user data.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="size">The retrieved 64-bit size of the specified user data.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserDataSize (AudioFileChunkType chunkType, int index, out ulong size)
		{
			return GetUserDataSize ((uint) chunkType, index, out size);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserData (AudioFileID audioFile, int userDataID, int index, int* userDataSize, IntPtr userData);

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataID">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if XAMCORE_5_0
		public AudioFileError GetUserData (int userDataID, int index, ref int size, IntPtr userData)
#else
		public int GetUserData (int userDataID, int index, ref int size, IntPtr userData)
#endif
		{
			unsafe {
				return AudioFileGetUserData (Handle, userDataID, index, (int*) Unsafe.AsPointer<int> (ref size), userData);
			}
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, ref int size, IntPtr userData)
		{
			return (AudioFileError) GetUserData ((int) chunkType, index, ref size, userData);
		}

#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserDataAtOffset (AudioFileID audioFile, uint userDataID, int index, long inOffset, int* userDataSize, IntPtr userData);

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserData (uint userDataId, int index, long offset, ref int size, IntPtr userData)
		{
			unsafe {
				return (AudioFileError) AudioFileGetUserDataAtOffset (Handle, userDataId, index, offset, (int*) Unsafe.AsPointer<int> (ref size), userData);
			}
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, long offset, ref int size, IntPtr userData)
		{
			return GetUserData ((uint) chunkType, index, offset, ref size, userData);
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">The number of bytes written into the byte array.</param>
		/// <param name="data">An array of bytes where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserData (uint userDataId, int index, long offset, byte [] data, out int size)
		{
			size = data.Length;
			unsafe {
				fixed (byte* dataPtr = data)
					return GetUserData (userDataId, index, offset, ref size, (IntPtr) dataPtr);
			}
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunk.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">The number of bytes written into the byte array.</param>
		/// <param name="data">An array of bytes where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		[SupportedOSPlatform ("macos14.0")]
#else
		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
#endif
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, long offset, byte [] data, out int size)
		{
			return GetUserData ((uint) chunkType, index, offset, data, out size);
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
		unsafe extern static OSStatus AudioFileGetPropertyInfo (AudioFileID audioFile, AudioFileProperty propertyID, int* outDataSize, int* isWritable);

		public bool GetPropertyInfo (AudioFileProperty property, out int size, out int writable)
		{
			size = default;
			writable = default;
			unsafe {
				return AudioFileGetPropertyInfo (Handle, property, (int*) Unsafe.AsPointer<int> (ref size), (int*) Unsafe.AsPointer<int> (ref writable)) == 0;
			}
		}

		public bool IsPropertyWritable (AudioFileProperty property)
		{
			return GetPropertyInfo (property, out var _, out var writable) && writable != 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetProperty (AudioFileID audioFile, AudioFileProperty property, int* dataSize, IntPtr outdata);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetProperty (AudioFileID audioFile, AudioFileProperty property, int* dataSize, void* outdata);

		public bool GetProperty (AudioFileProperty property, ref int dataSize, IntPtr outdata)
		{
			unsafe {
				return AudioFileGetProperty (Handle, property, (int*) Unsafe.AsPointer<int> (ref dataSize), outdata) == 0;
			}
		}

		public IntPtr GetProperty (AudioFileProperty property, out int size)
		{
			int writable;

			if (!GetPropertyInfo (property, out size, out writable))
				return IntPtr.Zero;

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return IntPtr.Zero;

			unsafe {
				var rv = AudioFileGetProperty (Handle, property, (int*) Unsafe.AsPointer<int> (ref size), buffer);
				if (rv == 0)
					return buffer;
			}
			Marshal.FreeHGlobal (buffer);
			return IntPtr.Zero;
		}
#if NET
		unsafe T? GetProperty<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (AudioFileProperty property) where T : unmanaged
#else
		unsafe T? GetProperty<T> (AudioFileProperty property) where T : unmanaged
#endif
		{
			int size, writable;

			if (!GetPropertyInfo (property, out size, out writable))
				return null;
			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return null;
			try {
				var ptype = typeof (T);
				var r = AudioFileGetProperty (Handle, property, &size, buffer);
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
				if (AudioFileGetProperty (Handle, property, &size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		IntPtr GetIntPtr (AudioFileProperty property)
		{
			unsafe {
				IntPtr val = IntPtr.Zero;
				int size = sizeof (IntPtr);
				if (AudioFileGetProperty (Handle, property, &size, (IntPtr) (&val)) == 0)
					return val;
				return IntPtr.Zero;
			}
		}

		double GetDouble (AudioFileProperty property)
		{
			unsafe {
				double val = 0;
				int size = 8;
				if (AudioFileGetProperty (Handle, property, &size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		long GetLong (AudioFileProperty property)
		{
			unsafe {
				long val = 0;
				int size = 8;
				if (AudioFileGetProperty (Handle, property, &size, (IntPtr) (&val)) == 0)
					return val;
				return 0;
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileSetProperty (AudioFileID audioFile, AudioFileProperty property, int dataSize, IntPtr propertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileSetProperty (AudioFileID audioFile, AudioFileProperty property, int dataSize, AudioFilePacketTableInfo* propertyData);

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

		/// <summary>Audio file type.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileType FileType {
			get {
				return (AudioFileType) GetInt (AudioFileProperty.FileFormat);
			}
		}

		/// <summary>The audio basic description, as determined by decoding the file.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		[Advice ("Use 'DataFormat' instead.")]
		public AudioStreamBasicDescription StreamBasicDescription {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat) ?? default (AudioStreamBasicDescription);
			}
		}

		/// <summary>Gets the status of the stream's basic description.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileError StreamBasicDescriptionStatus { get; private set; }

		/// <summary>Gets the <see cref="T:AudioToolbox.AudioStreamBasicDescription" />, if present, that describes the format of the audio data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioStreamBasicDescription? DataFormat {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat);
			}
		}

		/// <summary>Returns a list of the supported audio formats.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>Gets a Boolean value that tells whether the audio file has been optimized and is ready to receive sound data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool IsOptimized {
			get {
				return GetInt (AudioFileProperty.IsOptimized) == 1;
			}
		}

		/// <summary>The magic cookie for this file.</summary>
		///         <value />
		///         <remarks>Certain files require the magic cookie to be set before they can be written to.   Set this property before you write packets from your source (AudioQueue).</remarks>
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

		/// <summary>Gets the number of audio data packets in the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public long DataPacketCount {
			get {
				return GetLong (AudioFileProperty.AudioDataPacketCount);
			}
		}

		/// <summary>Gets the maximum audio packet size.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int MaximumPacketSize {
			get {
				return GetInt (AudioFileProperty.MaximumPacketSize);
			}
		}

		/// <summary>Gets the offset, in bytes, to the beginning of the audio data in the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public long DataOffset {
			get {
				return GetLong (AudioFileProperty.DataOffset);
			}
		}

		/// <summary>Gets the album artwork for the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSData? AlbumArtwork {
			get {
				return Runtime.GetNSObject<NSData> (GetIntPtr (AudioFileProperty.AlbumArtwork));
			}
		}

		/// <summary>Gets the channel layout of the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <summary>Gets or sets a Boolean value that controls whether the updating of file size information in the header will be deferred until the file is read, optimized, or closed. The default, which is safer, is <see langword="false" /></summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool DeferSizeUpdates {
			get {
				return GetInt (AudioFileProperty.DeferSizeUpdates) == 1;
			}
			set {
				SetInt (AudioFileProperty.DeferSizeUpdates, value ? 1 : 0);
			}
		}

		/// <summary>Audio file bit rate.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int BitRate {
			get {
				return GetInt (AudioFileProperty.BitRate);
			}
		}

		/// <summary>Gets the estimated duration, in seconds, of the audio data in the file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public double EstimatedDuration {
			get {
				return GetDouble (AudioFileProperty.EstimatedDuration);
			}
		}

		/// <summary>Gets the theoretical upper bound for the audio packet size for audio data in the file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int PacketSizeUpperBound {
			get {
				return GetInt (AudioFileProperty.PacketSizeUpperBound);
			}
		}

		/// <summary>Gets the amount of recording time to reserve in the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public double ReserveDuration {
			get {
				return GetDouble (AudioFileProperty.ReserveDuration);
			}
		}

		/// <summary>Gets the <see cref="T:AudioToolbox.AudioFileMarkerList" /> that contains the markers for the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileMarkerList? MarkerList {
			get {
				var ptr = GetProperty (AudioFileProperty.MarkerList, out var _);
				if (ptr == IntPtr.Zero)
					return null;

				return new AudioFileMarkerList (ptr, true);
			}
		}

		/// <summary>Gets a list of all the audio regions in the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileRegionList? RegionList {
			get {
				var ptr = GetProperty (AudioFileProperty.RegionList, out var _);
				if (ptr == IntPtr.Zero)
					return null;

				return new AudioFileRegionList (ptr, true);
			}
		}

		/// <summary>Gets the status of the audio packet table..</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioFileError PacketTableInfoStatus { get; private set; }

		/// <summary>Gets or sets the <see cref="T:AudioToolbox.AudioFilePacketTableInfo" /> structure that describes the audio file packet table.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public unsafe AudioFilePacketTableInfo? PacketTableInfo {
			get {
				return GetProperty<AudioFilePacketTableInfo> (AudioFileProperty.PacketTableInfo);
			}
			set {
				if (value is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

				AudioFilePacketTableInfo afpti = value.Value;
				var res = AudioFileSetProperty (Handle, AudioFileProperty.PacketTableInfo, sizeof (AudioFilePacketTableInfo), &afpti);
				if (res != 0)
					throw new ArgumentException (res.ToString ());
			}
		}

		/// <summary>Gets an array of four-character codes that describe the kind of each chunk in the audio file.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public unsafe AudioFileChunkType []? ChunkIDs {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.ChunkIDs, out size, out writable);
				if (size == 0)
					return null;

				var data = new AudioFileChunkType [size / sizeof (AudioFileChunkType)];
				fixed (AudioFileChunkType* ptr = data) {
					if (AudioFileGetProperty (Handle, AudioFileProperty.ChunkIDs, &size, (IntPtr) ptr) != 0)
						return null;

					return data;
				}
			}
		}

		/// <summary>Gets a byte array that contains the ID3Tag for the audio data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public unsafe byte []? ID3Tag {
			get {
				int size;
				int writable;
				var res = GetPropertyInfo (AudioFileProperty.ID3Tag, out size, out writable);
				if (size == 0)
					return null;

				var data = new byte [size];
				fixed (byte* ptr = data) {
					if (AudioFileGetProperty (Handle, AudioFileProperty.ID3Tag, &size, (IntPtr) ptr) != 0)
						return null;

					return data;
				}
			}
		}

		/// <summary>Gets the CF dictionary that contains audio file metadata.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
			AudioFramePacketTranslation buffer = default;
			buffer.Packet = packet;

			unsafe {
				int size = sizeof (AudioFramePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.PacketToFrame, &size, &buffer) == 0)
					return buffer.Frame;
				return -1;
			}
		}

		public long FrameToPacket (long frame, out int frameOffsetInPacket)
		{
			AudioFramePacketTranslation buffer = default;
			buffer.Frame = frame;

			unsafe {
				int size = sizeof (AudioFramePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.FrameToPacket, &size, &buffer) == 0) {
					frameOffsetInPacket = buffer.FrameOffsetInPacket;
					return buffer.Packet;
				}
				frameOffsetInPacket = 0;
				return -1;
			}
		}

		public long PacketToByte (long packet, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer = default;
			buffer.Packet = packet;

			unsafe {
				int size = sizeof (AudioBytePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.PacketToByte, &size, &buffer) == 0) {
					isEstimate = (buffer.Flags & BytePacketTranslationFlags.IsEstimate) != 0;
					return buffer.Byte;
				}
				isEstimate = false;
				return -1;
			}
		}

		public long ByteToPacket (long byteval, out int byteOffsetInPacket, out bool isEstimate)
		{
			AudioBytePacketTranslation buffer = default;
			buffer.Byte = byteval;

			unsafe {
				int size = sizeof (AudioBytePacketTranslation);
				if (AudioFileGetProperty (Handle, AudioFileProperty.ByteToPacket, &size, &buffer) == 0) {
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

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Album {
			get {
				return GetStringValue ("album");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? ApproximateDurationInSeconds {
			get {
				return GetStringValue ("approximate duration in seconds");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Artist {
			get {
				return GetStringValue ("artist");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? ChannelLayout {
			get {
				return GetStringValue ("channel layout");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Composer {
			get {
				return GetStringValue ("composer");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Comments {
			get {
				return GetStringValue ("comments");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Copyright {
			get {
				return GetStringValue ("copyright");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? EncodingApplication {
			get {
				return GetStringValue ("encoding application");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Genre {
			get {
				return GetStringValue ("genre");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? ISRC {
			get {
				return GetStringValue ("ISRC");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? KeySignature {
			get {
				return GetStringValue ("key signature");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Lyricist {
			get {
				return GetStringValue ("lyricist");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? NominalBitRate {
			get {
				return GetStringValue ("nominal bit rate");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? RecordedDate {
			get {
				return GetStringValue ("recorded date");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? SourceBitDepth {
			get {
				return GetStringValue ("source bit depth");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? SourceEncoder {
			get {
				return GetStringValue ("source encoder");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? SubTitle {
			get {
				return GetStringValue ("subtitle");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Tempo {
			get {
				return GetStringValue ("tempo");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? TimeSignature {
			get {
				return GetStringValue ("time signature");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? Title {
			get {
				return GetStringValue ("title");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? TrackNumber {
			get {
				return GetStringValue ("track number");
			}
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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
		/// <summary>Used to set or get the size of the audio stream.</summary>
		///         <value>The size of the file.</value>
		///         <remarks>If the AudioSource is created in reading mode, this method should return the size of the audio data.   If the AudioSource is created to write data, this method is invoked to set the audio file size.</remarks>
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

