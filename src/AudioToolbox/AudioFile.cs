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

using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using CoreFoundation;

using AudioFileID = System.IntPtr;

namespace AudioToolbox {

	/// <summary>Known audio file types. Used to specify the kind of audio file to create, or as a hint to the audio parser about the contents of the file.</summary>
	public enum AudioFileType {  // UInt32 AudioFileTypeID
		/// <summary>Audio Interchange File Format.</summary>
		AIFF = 0x41494646, // AIFF
		/// <summary>Compressed Audio Interchange File Format.</summary>
		AIFC = 0x41494643, // AIFC
		/// <summary>Microsoft WAVE format.</summary>
		WAVE = 0x57415645, // WAVE
		/// <summary>BWF-compatible RF64 multichannel sound format.</summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
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
		/// <summary>Apple Core Audio Format. CAF files are containers that can contain multiple audio formats and metadata tracks. Uses 64-bit offsets, so the format is not limited to 4GB.</summary>
		CAF = 0x63616666, // caff
		/// <summary>3GP (3GPP file format) is a multimedia container format defined by the Third Generation Partnership Project (3GPP) for 3G UMTS multimedia services. It is used on 3G mobile phones but can also be played on some 2G and 4G phones.</summary>
		ThreeGP = 0x33677070, // 3gpp
		/// <summary>3G2 (3GPP2 file format) is a multimedia container format defined by the 3GPP2 for 3G CDMA2000 multimedia services. It is very similar to the 3GP file format, but has some extensions and limitations in comparison to 3GP.</summary>
		ThreeGP2 = 0x33677032, // 3gp2
		/// <summary>Adaptive Multi-Rate format, optimized for speech coding   Used widely in GSM an UMTS.</summary>
		AMR = 0x616d7266, // amrf
		/// <summary>Free Lossless Audio Codec format.</summary>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		FLAC = 0x666c6163, // flac
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		LatmInLoas = 0x6c6f6173, // loas
	}

	/// <summary>The error codes returned by <see cref="AudioFile" />.</summary>
	public enum AudioFileError {// Implictly cast to OSType in AudioFile.h
		/// <summary>The operation was successful.</summary>
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
		/// <summary>A packet dependency is not valid.</summary>
		InvalidPacketDependencyError = 0x6465703f, // dep?
		/// <summary>The file is invalid.</summary>
		InvalidFile = 0x6474613f, // dta?
		/// <summary>The operation is not supported.</summary>
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

	/// <summary>An enumeration whose values specify the <c>permissions</c> argument when opening an <see cref="AudioFile" />.</summary>
	[Flags]
	[NativeName ("AudioFilePermissions")]
	public enum AudioFilePermission {
		/// <summary>The file is opened for reading.</summary>
		Read = 0x01,
		/// <summary>The file is opened for writing.</summary>
		Write = 0x02,
		/// <summary>The file is opened for both reading and writing.</summary>
		ReadWrite = 0x03,
	}

	/// <summary>An enumeration whose values to select creation options for <see cref="AudioFile" />.</summary>
	[Flags]
	public enum AudioFileFlags { // UInt32 in AudioFileCreateWithURL()
#if !XAMCORE_5_0
		/// <summary>If this flag is set, the existing file will be erased when creating an <see cref="AudioFile" />.</summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'EraseFile' instead.")]
		EraseFlags = 1,
#endif // !XAMCORE_5_0
		/// <summary>If this flag is set, the existing file will be erased when creating an <see cref="AudioFile" />.</summary>
		EraseFile = 1,
		/// <summary>If this flag is set, audio data will be written without page alignment. This will make the data more compact but possibly slow readout.</summary>
		DontPageAlignAudioData = 2,
	}

	/// <summary>An enumeration whose values represent information about a <see cref="AudioFile" />.</summary>
	/// <seealso cref="AudioToolbox.AudioFileStream.GetProperty(AudioToolbox.AudioFileStreamProperty,ref System.Int32,System.IntPtr)" />
	/// <seealso cref="AudioToolbox.AudioFile.SetProperty(AudioToolbox.AudioFileProperty,System.Int32,System.IntPtr)" />
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

	/// <summary>An enumeration whose values specify an audio-loop's direction.</summary>
	public enum AudioFileLoopDirection { // Unused?
		/// <summary>To be added.</summary>
		NoLooping = 0,
		/// <summary>To be added.</summary>
		Forward = 1,
		/// <summary>To be added.</summary>
		ForwardAndBackward = 2,
		/// <summary>To be added.</summary>
		Backward = 3,
	}

	/// <summary>An enumeration whose values specify different types of chunks appropriate to audio files.</summary>
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
	[NativeName ("AudioBytePacketTranslationFlags")]
	enum BytePacketTranslationFlags : uint  // Stored in UInt32 in AudioBytePacketTranslation - AudioFile.h
	{
		IsEstimate = 1,
	}

	/// <summary>A struct that encapsulates a Society of Motion Picture and Television Engineers time.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFileSmpteTime { // AudioFile_SMPTE_Time
		/// <summary>To be added.</summary>
		public sbyte Hours;
		/// <summary>To be added.</summary>
		public byte Minutes;
		/// <summary>To be added.</summary>
		public byte Seconds;
		/// <summary>To be added.</summary>
		public byte Frames;
		/// <summary>To be added.</summary>
		public uint SubFrameSampleOffset;
	}

	/// <summary>A class that represents a specific named position within an audio file.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFileMarker {
		/// <summary>To be added.</summary>
		public double FramePosition;
		internal IntPtr Name_cfstringref;
		/// <summary>To be added.</summary>
		public int MarkerID;
		/// <summary>To be added.</summary>
		public AudioFileSmpteTime SmpteTime;
		/// <summary>To be added.</summary>
		public AudioFileMarkerType Type;
		/// <summary>To be added.</summary>
		public ushort Reserved;
		/// <summary>To be added.</summary>
		public ushort Channel;

		/// <summary>To be added.</summary>
		public string? Name {
			get {
				return CFString.FromHandle (Name_cfstringref);
			}
		}
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioPacketRangeByteCountTranslation {
		public long Packet;
		public long PacketCount;
		public long ByteCountUpperBound;
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioPacketRollDistanceTranslation {
		public long Packet;
		public long RollDistance;
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioIndependentPacketTranslation {
		public long Packet;
		public long IndependentlyDecodablePacket;
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
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

	/// <summary>An enumeration whose values specify the <see cref="AudioFileMarker.Type" /> property.</summary>
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

	/// <summary>A list of <see cref="AudioFileMarker" /> objects.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioFileMarkerList : IDisposable {
		IntPtr ptr;
		readonly bool owns;

		/// <summary>Create a new <see cref="AudioFileMarkerList" /> instance.</summary>
		/// <param name="ptr">The pointer to the native value.</param>
		/// <param name="owns">Whether the native pointer should be freed when this managed instance is disposed.</param>
		public AudioFileMarkerList (IntPtr ptr, bool owns)
		{
			this.ptr = ptr;
			this.owns = owns;
		}

		~AudioFileMarkerList ()
		{
			Dispose (false);
		}

		/// <summary>Get the <see cref="SmpteTimeType" /> for this list.</summary>
		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>Get the number of elements in the list.</summary>
		public uint Count {
			get {
				return (uint) Marshal.ReadInt32 (ptr, 4);
			}
		}

		/// <summary>Get the element at the specified 0-based index.</summary>
		/// <param name="index">The 0-based index to return.</param>
		/// <returns>The element at the specified 0-based index.</returns>
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

		/// <summary>Releases the resources used by the <see cref="AudioFileMarkerList" /> object.</summary>
		public void Dispose ()
		{
			Dispose (true);
		}

		/// <summary>Releases the resources used by the <see cref="AudioFileMarkerList" /> object.</summary>
		/// <param name="disposing">If set to <see langword="true" />, the method is invoked directly and will dispose managed and unmanaged resources; If set to <see langword="false" /> the method is being called by the garbage collector finalizer and should only release unmanaged resources.</param>
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

	/// <summary>Represents the number of valid frames in a file and where they begin or end.</summary>
	/// <remarks>Not all audio file data formats guarantee that their contents are 100% valid; some have priming or remainder frames. This class can be used with such data formats to identify the valid frames in a file.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFilePacketTableInfo {
		/// <summary>The number of valid frames in the file.</summary>
		public long ValidFrames;
		/// <summary>The number of priming frames (invalid frames at the beginning) in the file.</summary>
		public int PrimingFrames;
		/// <summary>The number of remainder frames (invalid frames at the end) in the file.</summary>
		public int RemainderFrames;
	}

	/// <summary>Represents a named region within an audio file.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
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

		/// <summary>Create a new <see cref="AudioFileRegion" /> instance, wrapping a native pointer.</summary>
		/// <param name="ptr">The native pointer to wrap.</param>
		public AudioFileRegion (IntPtr ptr)
		{
			this.ptr = ptr;
		}

		/// <summary>The region identifier for this region.</summary>
		public uint RegionID {
			get {
				return (uint) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>The name for this region.</summary>
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

		/// <summary>The flags for this region.</summary>
		public unsafe AudioFileRegionFlags Flags {
			get {
				return (AudioFileRegionFlags) Marshal.ReadInt32 (ptr, sizeof (uint) + sizeof (IntPtr));
			}
		}

		/// <summary>The number of elements in this region.</summary>
		public unsafe int Count {
			get {
				return Marshal.ReadInt32 (ptr, 2 * sizeof (uint) + sizeof (IntPtr));
			}
		}

		/// <summary>Get the element at the specified 0-based index.</summary>
		/// <param name="index">The 0-based index to return.</param>
		/// <returns>The element at the specified 0-based index.</returns>
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

	/// <summary>A flagging enumeration whose values are used in the <see cref="AudioFileRegion.Flags" /> property.</summary>
	[Flags]
	public enum AudioFileRegionFlags : uint // UInt32 in AudioFileRegion
	{
		/// <summary>In conjunction with at least one other flag, loops the region.</summary>
		LoopEnable = 1,
		/// <summary>The region is played normally.</summary>
		PlayForward = 2,
		/// <summary>The region is played in reverse.</summary>
		PlayBackward = 4,
	}

	/// <summary>A list of <see cref="AudioFileRegion" /> values.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioFileRegionList : IDisposable {
		IntPtr ptr;
		readonly bool owns;

		/// <summary>Create a new <see cref="AudioFileRegionList" /> instance.</summary>
		/// <param name="ptr">The pointer to the native value.</param>
		/// <param name="owns">Whether the native pointer should be freed when this managed instance is disposed.</param>
		public AudioFileRegionList (IntPtr ptr, bool owns)
		{
			this.ptr = ptr;
			this.owns = owns;
		}

		~AudioFileRegionList ()
		{
			Dispose (false);
		}

		/// <summary>Get the <see cref="SmpteTimeType" /> for this list.</summary>
		public SmpteTimeType SmpteTimeType {
			get {
				return (SmpteTimeType) Marshal.ReadInt32 (ptr);
			}
		}

		/// <summary>Get the number of elements in the list.</summary>
		public uint Count {
			get {
				return (uint) Marshal.ReadInt32 (ptr, sizeof (uint));
			}
		}

		/// <summary>Get the element at the specified 0-based index.</summary>
		/// <param name="index">The 0-based index to return.</param>
		/// <returns>The element at the specified 0-based index.</returns>
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

		/// <summary>Releases the resources used by the <see cref="AudioFileRegionList" /> object.</summary>
		public void Dispose ()
		{
			Dispose (true);
		}

		/// <summary>Releases the resources used by the <see cref="AudioFileRegionList" /> object.</summary>
		/// <param name="disposing">If set to <see langword="true" />, the method is invoked directly and will dispose managed and unmanaged resources; If set to <see langword="false" /> the method is being called by the garbage collector finalizer and should only release unmanaged resources.</param>
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

	/// <summary>Class used to create or read audio files.</summary>
	/// <remarks>
	///   <para>Use the Create, Open and OpenRead factory methods to create instances of this class. </para>
	///   <para>This class provides access to the encoder and decoder for compressed audio files.</para>
	/// </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioFile : DisposableObject {
		internal AudioFile ()
		{
			// This ctor is used by AudioSource that will set the handle later.
		}

		[Preserve (Conditional = true)]
		internal AudioFile (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioFileClose (AudioFileID handle);

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero && Owns)
				AudioFileClose (Handle);
			base.Dispose (disposing);
		}

		/// <summary>Audio file size, in bytes.</summary>
		public long Length {
			get {
				return GetLong (AudioFileProperty.AudioDataByteCount);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileCreateWithURL (IntPtr cfurlref_infile, AudioFileType inFileType, AudioStreamBasicDescription* inFormat, AudioFileFlags inFlags, AudioFileID* file_id);

		/// <inheritdoc cref="Create(CFUrl,AudioFileType,AudioStreamBasicDescription,AudioFileFlags)" />
		public static AudioFile? Create (string url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			using (var cfurl = CFUrl.FromUrlString (url, null)!)
				return Create (cfurl, fileType, format, inFlags);
		}

		/// <summary>Create a new audio file.</summary>
		/// <param name="url">The url of the file to create.</param>
		/// <param name="fileType">The file type for the created file.</param>
		/// <param name="format">Description of the data that is going to be passed to the <see cref="AudioFile" /> object.</param>
		/// <param name="inFlags">Creation flags.</param>
		/// <returns>The initialized audio file, or <see langword="null" /> if there is an error creating the file.</returns>
		public static AudioFile? Create (CFUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			return Create (url, fileType, format, inFlags, out var _);
		}

		/// <summary>Create a new audio file.</summary>
		/// <param name="url">The url of the file to create.</param>
		/// <param name="fileType">The file type for the created file.</param>
		/// <param name="format">Description of the data that is going to be passed to the <see cref="AudioFile" /> object.</param>
		/// <param name="inFlags">Creation flags.</param>
		/// <param name="status"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>The initialized audio file, or <see langword="null" /> if there is an error creating the file.</returns>
		public static AudioFile? Create (CFUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags, out AudioFileError status)
		{
			var rv = Create (url.GetNonNullHandle (nameof (url)), fileType, format, inFlags, out status);
			GC.KeepAlive (url);
			return rv;
		}

		/// <inheritdoc cref="Create(CFUrl,AudioFileType,AudioStreamBasicDescription,AudioFileFlags)" />
		public static AudioFile? Create (NSUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags)
		{
			return Create (url, fileType, format, inFlags, out var _);
		}

		/// <inheritdoc cref="Create(CFUrl,AudioFileType,AudioStreamBasicDescription,AudioFileFlags, out AudioFileError)" />
		public static AudioFile? Create (NSUrl url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags, out AudioFileError status)
		{
			var rv = Create (url.GetNonNullHandle (nameof (url)), fileType, format, inFlags, out status);
			GC.KeepAlive (url);
			return rv;
		}

		static AudioFile? Create (IntPtr url, AudioFileType fileType, AudioStreamBasicDescription format, AudioFileFlags inFlags, out AudioFileError status)
		{
			var h = default (IntPtr);

			unsafe {
				status = AudioFileCreateWithURL (url, fileType, &format, inFlags, &h);
			}

			if (status == AudioFileError.Success && h != IntPtr.Zero)
				return new AudioFile (h, true);

			return null;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileOpenURL (IntPtr cfurlref_infile, byte permissions, AudioFileType fileTypeHint, IntPtr* file_id);

		/// <summary>Opens an audio file for reading.</summary>
		/// <param name="url">A url to a local file name.</param>
		/// <param name="fileTypeHint">A hint indicating the file format expected, this is necessary for audio files where the operating system can not probe the type by looking at the file signature or file extension (for example AC3). Pass zero to auto detect the format.</param>
		/// <returns>An instance of <see cref="AudioFile" /> on success, or <see langword="null" /> on error.</returns>
		/// <remarks>
		///   <para>The hint is necessary as sometimes it is not possible to determine the file type merely based on the contents of the file.</para>
		///   <para>Once you have opened the file for reading, you can use the various Read methods to decode the audio packets contained in the file.</para>
		/// </remarks>
		public static AudioFile? OpenRead (string url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		/// <summary>Opens an audio file for reading.</summary>
		/// <param name="url">A url to a local file name.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <param name="fileTypeHint">A hint indicating the file format expected, this is necessary for audio files where the operating system can not probe the type by looking at the file signature or file extension (for example AC3). Pass zero to auto detect the format.</param>
		/// <returns>An instance of <see cref="AudioFile" /> on success, or <see langword="null" /> on error.</returns>
		/// <remarks>
		///   <para>The hint is necessary as sometimes it is not possible to determine the file type merely based on the contents of the file.</para>
		///   <para>Once you have opened the file for reading, you can use the various Read methods to decode the audio packets contained in the file.</para>
		/// </remarks>
		public static AudioFile? OpenRead (string url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		/// <inheritdoc cref="OpenRead(string,AudioFileType)" />
		public static AudioFile? OpenRead (CFUrl url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		/// <inheritdoc cref="OpenRead(string,out AudioFileError,AudioFileType)" />
		public static AudioFile? OpenRead (CFUrl url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		/// <inheritdoc cref="OpenRead(string,AudioFileType)" />
		public static AudioFile? OpenRead (NSUrl url, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, fileTypeHint);
		}

		/// <inheritdoc cref="OpenRead(string,out AudioFileError,AudioFileType)" />
		public static AudioFile? OpenRead (NSUrl url, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			return Open (url, AudioFilePermission.Read, out error, fileTypeHint);
		}

		/// <summary>Opens an audio file for reading.</summary>
		/// <param name="url">A url to a local file name.</param>
		/// <param name="permissions">The permissions to use when opening the file.</param>
		/// <param name="fileTypeHint">A hint indicating the file format expected, this is necessary for audio files where the operating system can not probe the type by looking at the file signature or file extension (for example AC3). Pass zero to auto detect the format.</param>
		/// <returns>An instance of <see cref="AudioFile" /> on success, or <see langword="null" /> on error.</returns>
		/// <remarks>
		///   <para>The hint is necessary as sometimes it is not possible to determine the file type merely based on the contents of the file.</para>
		///   <para>Once you have opened the file for reading, you can use the various Read methods to decode the audio packets contained in the file.</para>
		/// </remarks>
		public static AudioFile? Open (string url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		/// <summary>Opens an audio file for reading.</summary>
		/// <param name="url">A url to a local file name.</param>
		/// <param name="permissions">The permissions to use when opening the file.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <param name="fileTypeHint">A hint indicating the file format expected, this is necessary for audio files where the operating system can not probe the type by looking at the file signature or file extension (for example AC3). Pass zero to auto detect the format.</param>
		/// <returns>An instance of <see cref="AudioFile" /> on success, or <see langword="null" /> on error.</returns>
		/// <remarks>
		///   <para>The hint is necessary as sometimes it is not possible to determine the file type merely based on the contents of the file.</para>
		///   <para>Once you have opened the file for reading, you can use the various Read methods to decode the audio packets contained in the file.</para>
		/// </remarks>
		public static AudioFile? Open (string url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			using (var cfurl = CFUrl.FromUrlString (url, null)!)
				return Open (cfurl, permissions, out error, fileTypeHint);
		}

		/// <inheritdoc cref="Open(string,AudioFilePermission,AudioFileType)" />
		public static AudioFile? Open (CFUrl url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		/// <inheritdoc cref="Open(string,AudioFilePermission,out AudioFileError,AudioFileType)" />
		public static AudioFile? Open (CFUrl url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			AudioFile? audioFile = Open (url.Handle, permissions, fileTypeHint, out error);
			GC.KeepAlive (url);
			return audioFile;
		}

		/// <inheritdoc cref="Open(string,AudioFilePermission,AudioFileType)" />
		public static AudioFile? Open (NSUrl url, AudioFilePermission permissions, AudioFileType fileTypeHint = 0)
		{
			AudioFileError error;
			return Open (url, permissions, out error, fileTypeHint);
		}

		/// <inheritdoc cref="Open(string,AudioFilePermission,out AudioFileError,AudioFileType)" />
		public static AudioFile? Open (NSUrl url, AudioFilePermission permissions, out AudioFileError error, AudioFileType fileTypeHint = 0)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			AudioFile? audioFile = Open (url.Handle, permissions, fileTypeHint, out error);
			GC.KeepAlive (url);
			return audioFile;
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

		/// <summary>Optimizes the audio file, thus preparing it to receive audio data.</summary>
		public bool Optimize ()
		{
			return AudioFileOptimize (Handle) == 0;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileReadBytes (AudioFileID inAudioFile, byte useCache, long startingByte, int* numBytes, IntPtr outBuffer);

		/// <summary>Reads <paramref name="count" /> bytes from <paramref name="buffer" />, starting at <paramref name="startingByte" />.</summary>
		/// <param name="startingByte">The starting byte in the file where the data will be read.</param>
		/// <param name="buffer">The buffer that holds the data.</param>
		/// <param name="offset">The offset within the buffer where the audio data will stored.</param>
		/// <param name="count">The number of bytes to read from the file.</param>
		/// <param name="useCache">Whether the data should be cached.</param>
		/// <returns>The number of bytes read from the file, or -1 on error.</returns>
		/// <remarks>This API merely reads bytes from the file without any encoding. Use any of the <c>ReadPacketData</c> to read with encoding.</remarks>
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
		unsafe extern static AudioFileError AudioFileWriteBytes (AudioFileID audioFile, byte useCache, long startingByte, int* numBytes, IntPtr buffer);

		/// <summary>Writes a block of data to the audio file.</summary>
		/// <param name="startingByte">The starting byte in the file where the data will be written.</param>
		/// <param name="buffer">The buffer that holds the data.</param>
		/// <param name="offset">The offset within the buffer where the data to be saved starts.</param>
		/// <param name="count">The number of bytes to write to the file.</param>
		/// <param name="useCache">Whether the data should be cached.</param>
		/// <returns>The number of bytes written to the stream, or -1 on error.</returns>
		/// <remarks>This API merely writes bytes to the file without any encoding. Use any of the <c>WritePackets</c> overloads to write with encoding.</remarks>
		public int Write (long startingByte, byte [] buffer, int offset, int count, bool useCache)
		{
			return Write (startingByte, buffer, offset, count, useCache, out AudioFileError _);
		}

#if !XAMCORE_5_0
		/// <inheritdoc cref="Write(long,byte[],int,int,bool,out AudioFileError)" />
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'Write (long, byte[], int, int, bool, out AudioFileError)' instead.")]
		public int Write (long startingByte, byte [] buffer, int offset, int count, bool useCache, out int errorCode)
		{
			var rv = Write (startingByte, buffer, offset, count, useCache, out AudioFileError error);
			errorCode = (int) error;
			return rv;
		}
#endif // !XAMCORE_5_0

		/// <summary>Writes a block of data to the audio file.</summary>
		/// <param name="startingByte">The starting byte in the file where the data will be written.</param>
		/// <param name="buffer">The buffer that holds the data.</param>
		/// <param name="offset">The offset within the buffer where the data to be saved starts.</param>
		/// <param name="count">The number of bytes to write to the file.</param>
		/// <param name="useCache">Whether the data should be cached.</param>
		/// <param name="errorCode"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>The number of bytes written to the stream, or -1 on error.</returns>
		/// <remarks>This API merely writes bytes to the file without any encoding. Use any of the <c>WritePackets</c> overloads to write with encoding.</remarks>
		public int Write (long startingByte, byte [] buffer, int offset, int count, bool useCache, out AudioFileError errorCode)
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

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">The number of packets to read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (long inStartingPacket, int nPackets, byte [] buffer)
		{
			AudioFileError error;
			return ReadPacketData (inStartingPacket, nPackets, buffer, out error);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">The number of packets to read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (long inStartingPacket, int nPackets, byte [] buffer, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			int count = buffer.Length;
			return RealReadPacketData (false, inStartingPacket, ref nPackets, buffer, 0, ref count, out error);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">The number of packets to read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">The size of the output buffer (in bytes).</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count)
		{
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">The number of packets to read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">The size of the output buffer (in bytes).</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
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

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">On input the size of the output buffer (in bytes), upon return the actual number of bytes read.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, byte [] buffer, int offset, ref int count)
		{
			AudioFileError error;
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, offset, ref count, out error);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">On input the size of the output buffer (in bytes), upon return the actual number of bytes read.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
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

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="count">On input the size of the output buffer (in bytes), upon return the actual number of bytes read.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count)
		{
			AudioFileError error;
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, ref count, out error);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="count">On input the size of the output buffer (in bytes), upon return the actual number of bytes read.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadPacketData (bool useCache, long inStartingPacket, ref int nPackets, IntPtr buffer, ref int count, out AudioFileError error)
		{
			var descriptions = new AudioStreamPacketDescription [nPackets];
			return ReadPacketData (useCache, inStartingPacket, ref nPackets, buffer, ref count, out error, descriptions);
		}

		/// <summary>Reads packets of audio data from an audio file.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="count">On input the size of the output buffer (in bytes), upon return the actual number of bytes read.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <param name="descriptions">An array of packet descriptions describing the returned packets.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
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
				fixed (int* countPtr = &count, nPacketsPtr = &nPackets) {
					r = AudioFileReadPacketData (Handle,
							useCache ? (byte) 1 : (byte) 0,
							countPtr,
							pdesc,
							inStartingPacket,
							nPacketsPtr,
							buffer);
				}
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

		/// <summary>Reads <paramref name="nPackets" /> bytes into <paramref name="buffer" />, starting at <paramref name="inStartingPacket" />.</summary>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadFixedPackets (long inStartingPacket, int nPackets, byte [] buffer)
		{
			AudioFileError error;
			return ReadFixedPackets (inStartingPacket, nPackets, buffer, out error);
		}

		/// <summary>Reads <paramref name="nPackets" /> bytes into <paramref name="buffer" />, starting at <paramref name="inStartingPacket" />.</summary>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadFixedPackets (long inStartingPacket, int nPackets, byte [] buffer, out AudioFileError error)
		{
			if (buffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			return RealReadFixedPackets (false, inStartingPacket, nPackets, buffer, 0, buffer.Length, out error);
		}

		/// <summary>Reads <paramref name="nPackets" /> bytes into <paramref name="buffer" />, starting at <paramref name="inStartingPacket" />.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
		public AudioStreamPacketDescription []? ReadFixedPackets (bool useCache, long inStartingPacket, int nPackets, byte [] buffer, int offset, int count)
		{
			AudioFileError error;
			return ReadFixedPackets (useCache, inStartingPacket, nPackets, buffer, offset, count, out error);
		}

		/// <summary>Reads <paramref name="nPackets" /> bytes into <paramref name="buffer" />, starting at <paramref name="inStartingPacket" />.</summary>
		/// <param name="useCache">If the data should be cached.</param>
		/// <param name="inStartingPacket">The index of the first packet to read.</param>
		/// <param name="nPackets">On input the number of packets to read, upon return the number of packets actually read.</param>
		/// <param name="buffer">The output buffer where packets are written.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Array of packet descriptors for the packets that were read.</returns>
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

		/// <summary>Write audio packets to the audio file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="startingPacket">The starting packet that should be written.</param>
		/// <param name="numPackets">The number of packets to write.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <param name="byteCount">The number of bytes to write.</param>
		/// <returns>The number of packets written or -1 on error.</returns>
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

		/// <summary>Write audio packets to the audio file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="startingPacket">The starting packet in the <paramref name="packetDescriptions" /> that should be written.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <param name="byteCount">The number of bytes to write.</param>
		/// <returns>The number of packets written or -1 on error.</returns>
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

		/// <summary>Write audio packets to the audio file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="startingPacket">The starting packet in the <paramref name="packetDescriptions" /> that should be written.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="byteCount">The number of bytes to write.</param>
		/// <returns>The number of packets written or -1 on error.</returns>
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

		/// <summary>Write audio packets to the audio file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="startingPacket">The starting packet in the <paramref name="packetDescriptions" /> that should be written.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <param name="byteCount">The number of bytes to write.</param>
		/// <param name="errorCode"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>The number of packets written or -1 on error.</returns>
		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, IntPtr buffer, int byteCount, out AudioFileError errorCode)
		{
			if (packetDescriptions is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (packetDescriptions));
			if (buffer == IntPtr.Zero)
				throw new ArgumentException (nameof (buffer));
			int nPackets = packetDescriptions.Length;

			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					errorCode = AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, buffer);
				}
			}
			if (errorCode == 0)
				return nPackets;
			return -1;
		}

#if !XAMCORE_5_0
		/// <inheritdoc cref="WritePackets(bool,long,AudioStreamPacketDescription[],IntPtr,int,out AudioFileError)" />
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'WritePackets (bool, long, AudioStreamPacketDescription[], IntPtr, int, out AudioFileError)' instead.")]
		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, IntPtr buffer, int byteCount, out int errorCode)
		{
			var rv = WritePackets (useCache, startingPacket, packetDescriptions, buffer, byteCount, out AudioFileError error);
			errorCode = (int) error;
			return rv;
		}
#endif // !XAMCORE_5_0

		/// <summary>Write audio packets to the audio file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="startingPacket">The starting packet in the <paramref name="packetDescriptions" /> that should be written.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <param name="offset">The offset in the output buffer where to start writing packets to.</param>
		/// <param name="byteCount">The number of bytes to write.</param>
		/// <param name="errorCode"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>The number of packets written or -1 on error.</returns>
		unsafe public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, byte [] buffer, int offset, int byteCount, out AudioFileError errorCode)
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
					errorCode = AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, byteCount, packetDescriptionsPtr, startingPacket, &nPackets, (IntPtr) bop);
				}
				if (errorCode == 0)
					return nPackets;
				return -1;
			}
		}

#if !XAMCORE_5_0
		/// <inheritdoc cref="WritePackets(bool,long,AudioStreamPacketDescription[],byte[],int,int,out AudioFileError)" />
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'WritePackets (bool, long, AudioStreamPacketDescription[], byte[], int, int, out AudioFileError)' instead.")]
		public int WritePackets (bool useCache, long startingPacket, AudioStreamPacketDescription [] packetDescriptions, byte [] buffer, int offset, int byteCount, out int errorCode)
		{
			var rv = WritePackets (useCache, startingPacket, packetDescriptions, buffer, offset, byteCount, out AudioFileError error);
			errorCode = (int) error;
			return rv;
		}
#endif // !XAMCORE_5_0

		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="numBytes">The number of bytes to write.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="startingPacket">The starting packet in the <paramref name="packetDescriptions" /> that should be written.</param>
		/// <param name="numPackets">On input the number of packets to write, upon return the number of packets actually written.</param>
		/// <param name="buffer">The buffer containing the audio data.</param>
		/// <summary>Write audio packets to the audio file.</summary>
		/// <returns><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</returns>
		public AudioFileError WritePackets (bool useCache, int numBytes, AudioStreamPacketDescription [] packetDescriptions, long startingPacket, ref int numPackets, IntPtr buffer)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentException ("buffer");

			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					fixed (int* numPacketsPtr = &numPackets) {
						return AudioFileWritePackets (Handle, useCache ? (byte) 1 : (byte) 0, numBytes, packetDescriptionsPtr, startingPacket, numPacketsPtr, buffer);
					}
				}
			}
		}

		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[SupportedOSPlatform ("macos26.0")]
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static /* OSStatus */ AudioFileError AudioFileWritePacketsWithDependencies (
			AudioFileID inAudioFile,
			byte /* Boolean */ inUseCache,
			uint /* UInt32 */ inNumBytes,
			AudioStreamPacketDescription* /* const AudioStreamPacketDescription * __nullable */ inPacketDescriptions,
			AudioStreamPacketDependencyDescription* /* const AudioStreamPacketDependencyDescription * */ inPacketDependencies,
			long /* SInt64 */ inStartingPacket,
			uint* /* UInt32 * */ ioNumPackets,
			IntPtr /* const void * */inBuffer);

		/// <summary>Writes audio packets to the file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="packetDependencies">An array of packet dependencies for the audio data.</param>
		/// <param name="startingPacket">The index the first packet in the buffer to write.</param>
		/// <param name="numPackets">The number of packets to write replaced with the number of packets actually written.</param>
		/// <param name="buffer">The buffer containing the audio data to write.</param>
		/// <param name="numBytes">The number of bytes to write.</param>
		/// <returns><see cref="AudioFileError.Success" /> if successful, otherwise a status error code.</returns>
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[SupportedOSPlatform ("macos26.0")]
		public AudioFileError WritePackets (bool useCache, AudioStreamPacketDescription []? packetDescriptions, AudioStreamPacketDependencyDescription [] packetDependencies, long startingPacket, ref int numPackets, IntPtr buffer, int numBytes)
		{
			if (buffer == IntPtr.Zero)
				throw new ArgumentException (nameof (buffer));

			if (packetDependencies is null)
				ThrowHelper.ThrowArgumentNullException (nameof (packetDependencies));

			unsafe {
				fixed (AudioStreamPacketDescription* packetDescriptionsPtr = packetDescriptions) {
					fixed (AudioStreamPacketDependencyDescription* packetDependenciesPtr = packetDependencies) {
						fixed (int* numPacketsPtr = &numPackets) {
							return AudioFileWritePacketsWithDependencies (
								GetCheckedHandle (),
								useCache.AsByte (),
								(uint) numBytes,
								packetDescriptionsPtr,
								packetDependenciesPtr,
								startingPacket,
								(uint*) numPacketsPtr,
								buffer);
						}
					}
				}
			}
		}

		/// <summary>Writes audio packets to the file.</summary>
		/// <param name="useCache">Whether the data should be kept in the cache.</param>
		/// <param name="numBytes">The number of bytes to write.</param>
		/// <param name="packetDescriptions">An array of packet descriptions that describe the content of the buffer.</param>
		/// <param name="packetDependencies">An array of packet dependencies for the audio data.</param>
		/// <param name="startingPacket">The index the first packet in the buffer to write.</param>
		/// <param name="numPackets">The number of packets to write replaced with the number of packets actually written.</param>
		/// <param name="buffer">The buffer containing the audio data to write.</param>
		/// <param name="offset">An offset into <paramref name="buffer" /> where the audio data to write starts.</param>
		/// <returns><see cref="AudioFileError.Success" /> if successful, otherwise a status error code.</returns>
		[SupportedOSPlatform ("ios26.0")]
		[SupportedOSPlatform ("tvos26.0")]
		[SupportedOSPlatform ("maccatalyst26.0")]
		[SupportedOSPlatform ("macos26.0")]
		public AudioFileError WritePackets (bool useCache, AudioStreamPacketDescription []? packetDescriptions, AudioStreamPacketDependencyDescription [] packetDependencies, long startingPacket, ref int numPackets, byte [] buffer, int offset, int numBytes)
		{
			if (buffer is null)
				ThrowHelper.ThrowArgumentNullException (nameof (buffer));
			if (offset < 0)
				throw new ArgumentOutOfRangeException (nameof (offset), "< 0");
			if (numBytes < 0)
				throw new ArgumentOutOfRangeException (nameof (numBytes), "< 0");
			if (offset > buffer.Length - numBytes)
				throw new ArgumentException ("Reading would overrun buffer");
			unsafe {
				fixed (byte* bufferPtr = &buffer [offset])
					return WritePackets (useCache, packetDescriptions, packetDependencies, startingPacket, ref numPackets, (IntPtr) bufferPtr, numBytes);
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
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
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
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <returns>Returns the (non-negative) size on success, otherwise -1.</returns>
		public int GetUserDataSize (AudioFileChunkType chunkType, int index)
		{
			return GetUserDataSize ((uint) chunkType, index);
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserDataSize64 (AudioFileID audioFile, uint userDataID, int index, ulong* userDataSize);

		/// <summary>Get the 64-bit size of the specified user data.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="size">The retrieved 64-bit size of the specified user data.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public AudioFileError GetUserDataSize (uint userDataId, int index, out ulong size)
		{
			size = 0;
			unsafe {
				fixed (ulong* sizePtr = &size)
					return (AudioFileError) AudioFileGetUserDataSize64 (Handle, userDataId, index, sizePtr);
			}
		}

		/// <summary>Get the 64-bit size of the specified user data.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="size">The retrieved 64-bit size of the specified user data.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public AudioFileError GetUserDataSize (AudioFileChunkType chunkType, int index, out ulong size)
		{
			return GetUserDataSize ((uint) chunkType, index, out size);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserData (AudioFileID audioFile, int userDataID, int index, int* userDataSize, IntPtr userData);

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataID">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
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
				fixed (int* sizePtr = &size)
					return AudioFileGetUserData (Handle, userDataID, index, sizePtr, userData);
			}
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, ref int size, IntPtr userData)
		{
			return (AudioFileError) GetUserData ((int) chunkType, index, ref size, userData);
		}

		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static OSStatus AudioFileGetUserDataAtOffset (AudioFileID audioFile, uint userDataID, int index, long inOffset, int* userDataSize, IntPtr userData);

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public AudioFileError GetUserData (uint userDataId, int index, long offset, ref int size, IntPtr userData)
		{
			unsafe {
				fixed (int* sizePtr = &size)
					return (AudioFileError) AudioFileGetUserDataAtOffset (Handle, userDataId, index, offset, sizePtr, userData);
			}
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">On input the size of the memory <paramref name="userData" /> points to. On output the number of bytes written.</param>
		/// <param name="userData">A pointer to memory where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, long offset, ref int size, IntPtr userData)
		{
			return GetUserData ((uint) chunkType, index, offset, ref size, userData);
		}

		/// <summary>Get part of the data of a chunk in a file.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">The number of bytes written into the byte array.</param>
		/// <param name="data">An array of bytes where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
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
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="offset">The offset from the first byte of the chunk of the data to get.</param>
		/// <param name="size">The number of bytes written into the byte array.</param>
		/// <param name="data">An array of bytes where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public AudioFileError GetUserData (AudioFileChunkType chunkType, int index, long offset, byte [] data, out int size)
		{
			return GetUserData ((uint) chunkType, index, offset, data, out size);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileSetUserData (AudioFileID inAudioFile, int userDataID, int index, int userDataSize, IntPtr userData);

#if !XAMCORE_5_0
		/// <summary>Sets the value at the specified <paramref name="index" /> into the specified <paramref name="userDataId" /> to <paramref name="userData" />, which must have the size that is specified in <paramref name="userDataSize" />.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="userDataSize">The number of bytes to write.</param>
		/// <param name="userData">An array of bytes where the data will be copied.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'SetUserData (AudioFileChunkType, int, byte[])' instead.")]
		public int SetUserData (int userDataId, int index, int userDataSize, IntPtr userData)
		{
			if (userData == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (userData));
			return (int) AudioFileSetUserData (Handle, userDataId, index, userDataSize, userData);
		}
#endif // !XAMCORE_5_0

		/// <summary>Set the data of a chunk in a file.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <param name="data">An array of bytes to set.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		public AudioFileError SetUserData (AudioFileChunkType chunkType, int index, byte [] data)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			unsafe {
				fixed (byte* dataPtr = data)
					return AudioFileSetUserData (GetCheckedHandle (), (int) chunkType, index, data.Length, (IntPtr) dataPtr);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioFileError AudioFileRemoveUserData (AudioFileID audioFile, int userDataID, int index);

#if !XAMCORE_5_0
		/// <summary>Removes the chunk of user data at the specified <paramref name="index" /> in the user data that is identified by <paramref name="userDataId" />.</summary>
		/// <param name="userDataId">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'RemoveUserData (AudioFileChunkType, int)' instead.")]
		public int RemoveUserData (int userDataId, int index)
		{
			return (int) AudioFileRemoveUserData (Handle, userDataId, index);
		}
#endif

		/// <summary>Removes the specified chunk of user data.</summary>
		/// <param name="chunkType">The fourcc of the chunk.</param>
		/// <param name="index">The index of the chunk if there are more than one chunks.</param>
		/// <returns>Returns <see cref="AudioFileError.Success" /> on success, otherwise an <see cref="AudioFileError" /> error code.</returns>
		public AudioFileError RemoveUserData (AudioFileChunkType chunkType, int index)
		{
			return AudioFileRemoveUserData (GetCheckedHandle (), (int) chunkType, index);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileGetPropertyInfo (AudioFileID audioFile, AudioFileProperty propertyID, int* outDataSize, int* isWritable);

#if !XAMCORE_5_0
		/// <summary>Returns the value of the specified audio property, and stores the number of bytes allocated to store it in <paramref name="size" />, and indicates whether the value is writeable.</summary>
		/// <param name="property">The property whose info to get.</param>
		/// <param name="size">The size of the property.</param>
		/// <param name="writable">Whether the property is writable or not.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'GetPropertyInfo (AudioFileProperty, out int, out bool)' instead.")]
		public bool GetPropertyInfo (AudioFileProperty property, out int size, out int writable)
		{
			return GetPropertyInfo (property, out size, out writable, out var _);
		}

		/// <summary>Returns the value of the specified audio property, and stores the number of bytes allocated to store it in <paramref name="size" />, and indicates whether the value is writeable.</summary>
		/// <param name="property">The property whose info to get.</param>
		/// <param name="size">The size of the property.</param>
		/// <param name="writable">Whether the property is writable or not.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'GetPropertyInfo (AudioFileProperty, out int, out bool, out AudioFileError)' instead.")]
		public bool GetPropertyInfo (AudioFileProperty property, out int size, out int writable, out AudioFileError error)
		{
			size = default;
			writable = default;
			unsafe {
				fixed (int* sizePtr = &size, writablePtr = &writable) {
					error = AudioFileGetPropertyInfo (Handle, property, sizePtr, writablePtr);
				}
			}
			return error == AudioFileError.Success;
		}
#endif

		/// <summary>Returns the value of the specified audio property, and stores the number of bytes allocated to store it in <paramref name="size" />, and indicates whether the value is writeable.</summary>
		/// <param name="property">The property whose info to get.</param>
		/// <param name="size">The size of the property.</param>
		/// <param name="writable">Whether the property is writable or not.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
		public bool GetPropertyInfo (AudioFileProperty property, out int size, out bool writable)
		{
			return GetPropertyInfo (property, out size, out writable, out var _);
		}

		/// <summary>Returns the value of the specified audio property, and stores the number of bytes allocated to store it in <paramref name="size" />, and indicates whether the value is writeable.</summary>
		/// <param name="property">The property whose info to get.</param>
		/// <param name="size">The size of the property.</param>
		/// <param name="writable">Whether the property is writable or not.</param>
		/// <param name="error"><see cref="AudioFileError.Success" /> if successful, or an error code otherwise.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
		public bool GetPropertyInfo (AudioFileProperty property, out int size, out bool writable, out AudioFileError error)
		{
			int writableValue = 0;
			size = default;

			unsafe {
				fixed (int* sizePtr = &size)
					error = AudioFileGetPropertyInfo (Handle, property, sizePtr, &writableValue);
			}
			writable = writableValue != 0;

			return error == AudioFileError.Success;
		}

		/// <param name="property">The property being queried.</param>
		/// <summary>Checks whether the property value is settable.</summary>
		/// <returns>Whether the property value is settable or not.</returns>
		public bool IsPropertyWritable (AudioFileProperty property)
		{
			return GetPropertyInfo (property, out var _, out bool writable) && writable;
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileGetProperty (AudioFileID audioFile, AudioFileProperty property, int* dataSize, IntPtr outdata);

		[DllImport (Constants.AudioToolboxLibrary)]
		unsafe extern static AudioFileError AudioFileGetProperty (AudioFileID audioFile, AudioFileProperty property, int* dataSize, void* outdata);

		/// <summary>Returns the value of the specified audio property, stores it in <paramref name="outdata" />, and stores the number of bytes allocated to store it in <paramref name="dataSize" />.</summary>
		/// <param name="property">The property to get.</param>
		/// <param name="dataSize">On input the number of size of <paramref name="outdata" />, upon return the number of packets bytes written.</param>
		/// <param name="outdata">A pointer to the memory where the property value will be stored.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
		public bool GetProperty (AudioFileProperty property, ref int dataSize, IntPtr outdata)
		{
			unsafe {
				fixed (int* dataSizePtr = &dataSize)
					return AudioFileGetProperty (Handle, property, dataSizePtr, outdata) == 0;
			}
		}

		/// <summary>Returns the value of the specified audio property, and stores the number of bytes allocated to store it in <paramref name="size" />.</summary>
		/// <param name="property">The property to get.</param>
		/// <param name="size">Upon return, the size of the property.</param>
		/// <returns>A pointer to the value of the specified audio property, or <see cref="IntPtr.Size" /> in case of failure.</returns>
		/// <remarks>The caller is responsible for freeing the returned pointer, using <see cref="Marshal.FreeHGlobal" />.</remarks>
		public IntPtr GetProperty (AudioFileProperty property, out int size)
		{
			if (!GetPropertyInfo (property, out size, out bool _))
				return IntPtr.Zero;

			var buffer = Marshal.AllocHGlobal (size);
			if (buffer == IntPtr.Zero)
				return IntPtr.Zero;

			unsafe {
				fixed (int* sizePtr = &size) {
					var rv = AudioFileGetProperty (Handle, property, sizePtr, buffer);
					if (rv == 0)
						return buffer;
				}
			}
			Marshal.FreeHGlobal (buffer);
			return IntPtr.Zero;
		}

		unsafe T? GetProperty<[DynamicallyAccessedMembers (DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T> (AudioFileProperty property) where T : unmanaged
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

		/// <summary>Sets the value of the specified <paramref name="property" /> to <paramref name="propertyData" />, which must have the size that is specified in <paramref name="dataSize" />.</summary>
		/// <param name="property">The property to set.</param>
		/// <param name="dataSize">The size of <paramref name="propertyData" />.</param>
		/// <param name="propertyData">A pointer to the data to set.</param>
		/// <returns>Whether the operation succeeded or not.</returns>
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
		public AudioFileType FileType {
			get {
				return (AudioFileType) GetInt (AudioFileProperty.FileFormat);
			}
		}

		/// <summary>The audio basic description, as determined by decoding the file.</summary>
		[Advice ("Use 'DataFormat' instead.")]
		public AudioStreamBasicDescription StreamBasicDescription {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat) ?? default (AudioStreamBasicDescription);
			}
		}

		/// <summary>Gets the status of the stream's basic description.</summary>
		public AudioFileError StreamBasicDescriptionStatus { get; private set; }

		/// <summary>Gets the <see cref="AudioToolbox.AudioStreamBasicDescription" />, if present, that describes the format of the audio data.</summary>
		public AudioStreamBasicDescription? DataFormat {
			get {
				return GetProperty<AudioStreamBasicDescription> (AudioFileProperty.DataFormat);
			}
		}

		/// <summary>Returns a list of the supported audio formats.</summary>
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
		public bool IsOptimized {
			get {
				return GetInt (AudioFileProperty.IsOptimized) == 1;
			}
		}

		/// <summary>The magic cookie for this file.</summary>
		///  <remarks>Certain files require the magic cookie to be set before they can be written to. Set this property before you write packets from your source (AudioQueue).</remarks>
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
		public long DataPacketCount {
			get {
				return GetLong (AudioFileProperty.AudioDataPacketCount);
			}
		}

		/// <summary>Gets the maximum audio packet size.</summary>
		public int MaximumPacketSize {
			get {
				return GetInt (AudioFileProperty.MaximumPacketSize);
			}
		}

		/// <summary>Gets the offset, in bytes, to the beginning of the audio data in the audio file.</summary>
		public long DataOffset {
			get {
				return GetLong (AudioFileProperty.DataOffset);
			}
		}

		/// <summary>Gets the album artwork for the audio file.</summary>
		public NSData? AlbumArtwork {
			get {
				return Runtime.GetNSObject<NSData> (GetIntPtr (AudioFileProperty.AlbumArtwork));
			}
		}

		/// <summary>Gets the channel layout of the audio file.</summary>
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

		/// <summary>Gets or sets a boolean value that controls whether the updating of file size information in the header will be deferred until the file is read, optimized, or closed. The default, which is safer, is <see langword="false" /></summary>
		public bool DeferSizeUpdates {
			get {
				return GetInt (AudioFileProperty.DeferSizeUpdates) == 1;
			}
			set {
				SetInt (AudioFileProperty.DeferSizeUpdates, value ? 1 : 0);
			}
		}

		/// <summary>Audio file bit rate.</summary>
		public int BitRate {
			get {
				return GetInt (AudioFileProperty.BitRate);
			}
		}

		/// <summary>Gets the estimated duration, in seconds, of the audio data in the file.</summary>
		public double EstimatedDuration {
			get {
				return GetDouble (AudioFileProperty.EstimatedDuration);
			}
		}

		/// <summary>Gets the theoretical upper bound for the audio packet size for audio data in the file.</summary>
		public int PacketSizeUpperBound {
			get {
				return GetInt (AudioFileProperty.PacketSizeUpperBound);
			}
		}

		/// <summary>Gets the amount of recording time to reserve in the audio file.</summary>
		public double ReserveDuration {
			get {
				return GetDouble (AudioFileProperty.ReserveDuration);
			}
		}

		/// <summary>Gets the <see cref="AudioFileMarkerList" /> that contains the markers for the audio file.</summary>
		public AudioFileMarkerList? MarkerList {
			get {
				var ptr = GetProperty (AudioFileProperty.MarkerList, out var _);
				if (ptr == IntPtr.Zero)
					return null;

				return new AudioFileMarkerList (ptr, true);
			}
		}

		/// <summary>Gets a list of all the audio regions in the audio file.</summary>
		public AudioFileRegionList? RegionList {
			get {
				var ptr = GetProperty (AudioFileProperty.RegionList, out var _);
				if (ptr == IntPtr.Zero)
					return null;

				return new AudioFileRegionList (ptr, true);
			}
		}

		/// <summary>Gets the status of the audio packet table..</summary>
		public AudioFileError PacketTableInfoStatus { get; private set; }

		/// <summary>Gets or sets the <see cref="AudioFilePacketTableInfo" /> structure that describes the audio file packet table.</summary>
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

		/// <summary>Gets the dictionary that contains audio file metadata.</summary>
		public AudioFileInfoDictionary? InfoDictionary {
			get {
				var ptr = GetIntPtr (AudioFileProperty.InfoDictionary);
				var dict = Runtime.GetNSObject<NSMutableDictionary> (ptr, owns: true);
				if (dict is null)
					return null;

				return new AudioFileInfoDictionary (dict);
			}
		}

		/// <summary>Returns the frame number for the specified <paramref name="packet" />.</summary>
		/// <param name="packet">The packet whose frame number to get.</param>
		/// <returns>The frame number for the specified <paramref name="packet" />, or -1 in case of failure.</returns>
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

		/// <summary>Converts an audio frame into a packet offset.</summary>
		/// <param name="frame">The frame whose packet offset to get.</param>
		/// <param name="frameOffsetInPacket">The offset inside the packet that the frame points to.</param>
		/// <returns>-1 on failure, otherwise the packet that represents the specified frame. Additionally, the offset within the packet is returned in <paramref name="frameOffsetInPacket" />.</returns>
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

		/// <summary>Returns the byte offset for the <paramref name="packet" /> and indicates whether this is an estimated value in <paramref name="isEstimate" />.</summary>
		/// <param name="packet">The packet whose byte offset to get.</param>
		/// <param name="isEstimate">Whether the returned value is accurate or an estimate.</param>
		/// <returns>The byte offset for the specified <paramref name="packet" />.</returns>
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

		/// <summary>Converts a position on a stream to its packet location.</summary>
		/// <param name="byteval">The byte position.</param>
		/// <param name="byteOffsetInPacket">Offset within the packet.</param>
		/// <param name="isEstimate"><see langword="true" /> if the return value is an estimate.</param>
		/// <returns>The packet where the byte position would be, or -1 on error.</returns>
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

	/// <summary>Metadata-like information relating to a particular audio file.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioFileInfoDictionary : DictionaryContainer {
		internal AudioFileInfoDictionary (NSDictionary dict)
			: base (dict)
		{
		}

		/// <summary>The album.</summary>
		public string? Album {
			get {
				return GetStringValue ("album");
			}
		}

		/// <summary>An approximate duration in seconds.</summary>
		public string? ApproximateDurationInSeconds {
			get {
				return GetStringValue ("approximate duration in seconds");
			}
		}

		/// <summary>The artist.</summary>
		public string? Artist {
			get {
				return GetStringValue ("artist");
			}
		}

		/// <summary>The channel layout.</summary>
		public string? ChannelLayout {
			get {
				return GetStringValue ("channel layout");
			}
		}

		/// <summary>The composer.</summary>
		public string? Composer {
			get {
				return GetStringValue ("composer");
			}
		}

		/// <summary>Any comments.</summary>
		public string? Comments {
			get {
				return GetStringValue ("comments");
			}
		}

		/// <summary>The copyright.</summary>
		public string? Copyright {
			get {
				return GetStringValue ("copyright");
			}
		}

		/// <summary>The encoding application.</summary>
		public string? EncodingApplication {
			get {
				return GetStringValue ("encoding application");
			}
		}

		/// <summary>The genre.</summary>
		public string? Genre {
			get {
				return GetStringValue ("genre");
			}
		}

		/// <summary>The ISRC (International Standard Recording Code) value.</summary>
		public string? ISRC {
			get {
				return GetStringValue ("ISRC");
			}
		}

		/// <summary>The key signature.</summary>
		public string? KeySignature {
			get {
				return GetStringValue ("key signature");
			}
		}

		/// <summary>The lyricist.</summary>
		public string? Lyricist {
			get {
				return GetStringValue ("lyricist");
			}
		}

		/// <summary>The nominal bitrate.</summary>
		public string? NominalBitRate {
			get {
				return GetStringValue ("nominal bit rate");
			}
		}

		/// <summary>The recorded date.</summary>
		public string? RecordedDate {
			get {
				return GetStringValue ("recorded date");
			}
		}

		/// <summary>The source bit depth.</summary>
		public string? SourceBitDepth {
			get {
				return GetStringValue ("source bit depth");
			}
		}

		/// <summary>The source encoder.</summary>
		public string? SourceEncoder {
			get {
				return GetStringValue ("source encoder");
			}
		}

		/// <summary>The subtitle.</summary>
		public string? SubTitle {
			get {
				return GetStringValue ("subtitle");
			}
		}

		/// <summary>The tempo.</summary>
		public string? Tempo {
			get {
				return GetStringValue ("tempo");
			}
		}

		/// <summary>The time signature.</summary>
		public string? TimeSignature {
			get {
				return GetStringValue ("time signature");
			}
		}

		/// <summary>The title.</summary>
		public string? Title {
			get {
				return GetStringValue ("title");
			}
		}

		/// <summary>The track number.</summary>
		public string? TrackNumber {
			get {
				return GetStringValue ("track number");
			}
		}

		/// <summary>The year.</summary>
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

	/// <summary>A derived class from <see cref="AudioFile" /> that exposes virtual methods that can be hooked into (for reading and writing).</summary>
	/// <remarks>
	///   <para>
	///     <see cref="AudioSource" /> is an abstract class that derives from <see cref="AudioFile" /> that allows developers to hook up into the reading and writing stages of the <see cref="AudioFile" />.
	///     This can be used for example to read from an in-memory audio file, or to write to an in-memory buffer.
	///   </para>
	///   <para>
	///       When you write data into the <see cref="AudioSource" /> using any of the methods from <see cref="AudioFile" />, instead of writing the encoded data into a file, the data is sent to the <see cref="Read" /> abstract method.
	///   </para>
	///   <para>
	///     To use this class, you must create a class that derives from <see cref="AudioSource" /> and override the <see cref="Read" />, <see cref="Write" /> methods and the <see cref="Size" /> property.
	///   </para>
	/// </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public abstract class AudioSource : AudioFile {
		GCHandle gch;

		[UnmanagedCallersOnly]
		static unsafe int SourceRead (IntPtr clientData, long inPosition, int requestCount, IntPtr buffer, int* actualCount)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
			var localCount = 0;
			var result = audioSource?.Read (inPosition, requestCount, buffer, out localCount) == true ? 0 : 1;
			*actualCount = localCount;
			return result;
		}

		/// <summary>Callback invoked to read encoded audio data.</summary>
		/// <param name="position">Position in the audio stream that the data should be read from.</param>
		/// <param name="requestCount">Number of bytes to read.</param>
		/// <param name="buffer">Pointer to the buffer where the data should be stored.</param>
		/// <param name="actualCount">On return, set this value to the number of bytes actually read.</param>
		/// <returns>true on success, false on failure.</returns>
		/// <remarks>This method is called by the <see cref="AudioSource" /> when more data is requested.</remarks>
		public abstract bool Read (long position, int requestCount, IntPtr buffer, out int actualCount);

		[UnmanagedCallersOnly]
		static unsafe int SourceWrite (IntPtr clientData, long position, int requestCount, IntPtr buffer, int* actualCount)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
			var localCount = 0;
			var result = audioSource?.Write (position, requestCount, buffer, out localCount) == true ? 0 : 1;
			*actualCount = localCount;
			return result;
		}
		/// <summary>Callback used to write audio data into the audio stream.</summary>
		/// <param name="position">Position where the data should be stored.</param>
		/// <param name="requestCount">Number of bytes to write.</param>
		/// <param name="buffer">Pointer to the buffer that contains the data to be written.</param>
		/// <param name="actualCount">Set this value to indicate the number of bytes actually written.</param>
		/// <returns>True on success, false on failure.</returns>
		/// <remarks>This method is called by the <see cref="AudioSource" /> when it has encoded the data and it need to write it out.</remarks>
		public abstract bool Write (long position, int requestCount, IntPtr buffer, out int actualCount);

		[UnmanagedCallersOnly]
		static long SourceGetSize (IntPtr clientData)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;
			return audioSource?.Size ?? 0;
		}

		[UnmanagedCallersOnly]
		static int SourceSetSize (IntPtr clientData, long size)
		{
			GCHandle handle = GCHandle.FromIntPtr (clientData);
			var audioSource = handle.Target as AudioSource;

			if (audioSource is not null)
				audioSource.Size = size;
			return 0;
		}
		/// <summary>Used to set or get the size of the audio stream.</summary>
		/// <value>The size of the file.</value>
		/// <remarks>If the <see cref="AudioSource" /> is created in reading mode, this method should return the size of the audio data. If the <see cref="AudioSource" /> is created to write data, this method is invoked to set the audio file size.</remarks>
		public abstract long Size { get; set; }

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (gch.IsAllocated)
				gch.Free ();
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern unsafe static AudioFileError AudioFileInitializeWithCallbacks (
			IntPtr inClientData,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inReadFunc,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inWriteFunc,
			delegate* unmanaged<IntPtr, long> inGetSizeFunc,
			delegate* unmanaged<IntPtr, long, int> inSetSizeFunc,
			AudioFileType inFileType, AudioStreamBasicDescription* format, uint flags, IntPtr* id);

		/// <summary>Create a new <see cref="AudioSource" /> instance, with the specified file type and format.</summary>
		/// <param name="inFileType">The file type for the new audio source.</param>
		/// <param name="format">The audio format for the new audio source.</param>
		public AudioSource (AudioFileType inFileType, AudioStreamBasicDescription format)
		{
			Initialize (inFileType, format);
		}

		/// <summary>Constructor used when creating subclasses</summary>
		/// <remarks>
		///   This constructor is provided as a convenience for developers that need to decouple the creation of the <see cref="AudioSource" /> from starting the read and write process.
		///   Once you have created this object, you need to invoke the <see cref="Initialize(AudioToolbox.AudioFileType,AudioToolbox.AudioStreamBasicDescription)" /> method to complete the setup.
		/// </remarks>
		public AudioSource ()
		{
		}

		/// <summary>Initialize the audio source with the specified file type and format.</summary>
		/// <param name="inFileType">The file type for the new audio source.</param>
		/// <param name="format">The audio format for the new audio source.</param>
		protected void Initialize (AudioFileType inFileType, AudioStreamBasicDescription format)
		{
			gch = GCHandle.Alloc (this);
			AudioFileError code;
			IntPtr handle = IntPtr.Zero;
			unsafe {
				code = AudioFileInitializeWithCallbacks (GCHandle.ToIntPtr (gch), &SourceRead, &SourceWrite, &SourceGetSize, &SourceSetSize, inFileType, &format, 0, &handle);
			}
			if (code == 0) {
				InitializeHandle (handle);
				return;
			}
			throw new Exception (String.Format ("Unable to create AudioSource: {0}", code));
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static unsafe AudioFileError AudioFileOpenWithCallbacks (
			IntPtr inClientData,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inReadFunc,
			delegate* unmanaged<IntPtr, long, int, IntPtr, int*, int> inWriteFunc,
			delegate* unmanaged<IntPtr, long> inGetSizeFunc,
			delegate* unmanaged<IntPtr, long, int> inSetSizeFunc,
			AudioFileType inFileTypeHint, IntPtr* outAudioFile);

		/// <summary>Create a new <see cref="AudioSource" /> instance, with the specified file type.</summary>
		/// <param name="fileTypeHint">The file type for the new audio source.</param>
		public AudioSource (AudioFileType fileTypeHint)
		{
			Open (fileTypeHint);
		}

		/// <summary>Open the audio source for reading and/or writing.</summary>
		/// <param name="fileTypeHint">The file type for the audio source.</param>
		protected void Open (AudioFileType fileTypeHint)
		{
			gch = GCHandle.Alloc (this);
			AudioFileError code;
			IntPtr handle = IntPtr.Zero;
			unsafe {
				code = AudioFileOpenWithCallbacks (GCHandle.ToIntPtr (gch), &SourceRead, &SourceWrite, &SourceGetSize, &SourceSetSize, fileTypeHint, &handle);
			}
			if (code == 0) {
				InitializeHandle (handle);
				return;
			}
			throw new Exception (String.Format ("Unable to create AudioSource: {0}", code));
		}
	}
}
