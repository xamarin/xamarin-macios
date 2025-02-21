// 
// AudioType.cs:
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//    AKIHIRO Uehara (u-akihiro@reinforce-lab.com)
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2009, 2010 Novell, Inc
// Copyright 2010, Reinforce Lab.
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
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace AudioToolbox {
	public enum AudioFormatType : uint { // UInt32 in AudioStreamBasicDescription -- CoreAudio.framework CoreAudioTypes.h
		/// <summary>Uncompressed Linear Pulse Code Modulation (LCPM) format.  Each packet contains a single frame.</summary>
		LinearPCM = 0x6c70636d,
		/// <summary>AC-3  codec.</summary>
		AC3 = 0x61632d33,
		/// <summary>CAC-3 codec.</summary>
		CAC3 = 0x63616333,
		/// <summary>Apple's IMA 4:1 ADPCM codec.</summary>
		AppleIMA4 = 0x696d6134,
		/// <summary>MPEG-4 AAC codec.</summary>
		MPEG4AAC = 0x61616320,
		/// <summary>MPEG-4 CELP codec.</summary>
		MPEG4CELP = 0x63656c70,
		/// <summary>MPEG-4 HVXC codec.</summary>
		MPEG4HVXC = 0x68767863,
		/// <summary>MPEG-4 TwinVQ codec.</summary>
		MPEG4TwinVQ = 0x74777671,
		/// <summary>MACE 3:1 codec.</summary>
		MACE3 = 0x4d414333,
		/// <summary>MACE 6:1 codec.</summary>
		MACE6 = 0x4d414336,
		/// <summary>μ-Law codec.</summary>
		ULaw = 0x756c6177,
		/// <summary>A-law 2:1 compression scheme.</summary>
		ALaw = 0x616c6177,
		/// <summary>QDesign audio.</summary>
		QDesign = 0x51444d43,
		/// <summary>QDesign2 audio.</summary>
		QDesign2 = 0x51444d32,
		/// <summary>QUALCOMM PureVoice data.</summary>
		QUALCOMM = 0x51636c70,
		/// <summary>MPEG-1/2, layer 1 codec.</summary>
		MPEGLayer1 = 0x2e6d7031,
		/// <summary>MPEG-1/2, layer 2 codec.</summary>
		MPEGLayer2 = 0x2e6d7032,
		/// <summary>MPEG-1/2, layer 3 codec.</summary>
		MPEGLayer3 = 0x2e6d7033,
		/// <summary>IOAudioTimeStamp data.</summary>
		TimeCode = 0x74696d65,
		/// <summary>MIDI stream.</summary>
		MIDIStream = 0x6d696469,
		/// <summary>32-bit floating point data on a side chain.</summary>
		ParameterValueStream = 0x61707673,
		/// <summary>Apple Lossless format.</summary>
		AppleLossless = 0x616c6163,
		/// <summary>MPEG-4 high efficiency AAC codec.</summary>
		MPEG4AAC_HE = 0x61616368,
		/// <summary>MPEG-4 AAC low delay codec.</summary>
		MPEG4AAC_LD = 0x6161636c,
		/// <summary>MPEG-4 AAC enhanced low delay audio.</summary>
		MPEG4AAC_ELD = 0x61616365, // 'aace'
		/// <summary>MPEG-4 AAC enhanced low delay codec with spectral band replication.</summary>
		MPEG4AAC_ELD_SBR = 0x61616366, // 'aacf',
		/// <summary>MPEG-4 AAC enhanced low delay codec, version 2.</summary>
		MPEG4AAC_ELD_V2 = 0x61616367, // 'aacg',    
		/// <summary>MPEG-4 high efficiency AAC codec, version 2.</summary>
		MPEG4AAC_HE_V2 = 0x61616370,
		/// <summary>MPEG-4 spatial codec.</summary>
		MPEG4AAC_Spatial = 0x61616373,
		MpegD_Usac = 0x75736163, // 'usac' (Unified Speech and Audio Coding)
		/// <summary>Adaptive multi-rate codec.</summary>
		AMR = 0x73616d72, // 'samr'
		/// <summary>Wide-band adaptive multi-rate (AMR-WB) codec.</summary>
		AMRWideBand = 0x73617762, // 'sawb'
		/// <summary>Audible audiobook codec.</summary>
		Audible = 0x41554442,
		/// <summary>Internet low bitrate codec for speech.</summary>
		iLBC = 0x696c6263,
		/// <summary>DVI/Intel IMA ADPCM codec.</summary>
		DVIIntelIMA = 0x6d730011,
		/// <summary>Microsoft GSM 6.10 codec.</summary>
		MicrosoftGSM = 0x6d730031,
		/// <summary>AES3-2003 codec.</summary>
		AES3 = 0x61657333, // 'aes3'
#if !XAMCORE_5_0
		/// <summary>Enhanced AES-3.</summary>
		[Obsolete ("Use 'EnhancedAC3' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		EnhancedAES3 = 0x65632d33, // 'ec-3'
#endif
		/// <summary>Enhanced AC-3.</summary>
		EnhancedAC3 = 0x65632d33, // 'ec-3'
		/// <summary>To be added.</summary>
		Flac = 0x666c6163, // 'flac'
#if NET
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (13, 0)]
		[TV (13, 0)]
#endif
		LatmInLoas = 0x6c6f6173, // 'loas'
		/// <summary>To be added.</summary>
		Opus = 0x6f707573, // 'opus'
		/// <summary>Apple Positional Audio Codec.</summary>
#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
#else
		[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), Mac (15, 0)]
#endif
		Apac = 0x61706163, // 'apac'
	}

	[Flags]
	public enum AudioFormatFlags : uint // UInt32 in AudioStreamBasicDescription
	{
		IsFloat = (1 << 0),     // 0x1
		IsBigEndian = (1 << 1),     // 0x2
		IsSignedInteger = (1 << 2),     // 0x4
		IsPacked = (1 << 3),     // 0x8
		IsAlignedHigh = (1 << 4),     // 0x10
		IsNonInterleaved = (1 << 5),     // 0x20
		IsNonMixable = (1 << 6),     // 0x40
		FlagsAreAllClear = unchecked((uint) (1 << 31)),

		LinearPCMIsFloat = (1 << 0),     // 0x1
		LinearPCMIsBigEndian = (1 << 1),     // 0x2
		LinearPCMIsSignedInteger = (1 << 2),     // 0x4
		LinearPCMIsPacked = (1 << 3),     // 0x8
		LinearPCMIsAlignedHigh = (1 << 4),     // 0x10
		LinearPCMIsNonInterleaved = (1 << 5),     // 0x20
		LinearPCMIsNonMixable = (1 << 6),     // 0x40

		LinearPCMSampleFractionShift = 7,
		LinearPCMSampleFractionMask = 0x3F << (int) LinearPCMSampleFractionShift,
		LinearPCMFlagsAreAllClear = FlagsAreAllClear,

		AppleLossless16BitSourceData = 1,
		AppleLossless20BitSourceData = 2,
		AppleLossless24BitSourceData = 3,
		AppleLossless32BitSourceData = 4,

		CafIsFloat = (1 << 0),
		CafIsLittleEndian = (1 << 1)
	}

	[StructLayout (LayoutKind.Sequential)]
	unsafe struct AudioFormatInfo {
		public AudioStreamBasicDescription AudioStreamBasicDescription;
		public byte* MagicCookieWeak;
		public int MagicCookieSize;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[DebuggerDisplay ("{FormatName}")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioStreamBasicDescription {
		public double SampleRate;
		public AudioFormatType Format;
		public AudioFormatFlags FormatFlags;
		public int BytesPerPacket; // uint
		public int FramesPerPacket; // uint
		public int BytesPerFrame; // uint
		public int ChannelsPerFrame; // uint
		public int BitsPerChannel; // uint
		public int Reserved; // uint

#if !COREBUILD
		public const double AudioStreamAnyRate = 0;

		const int AudioUnitSampleFractionBits = 24;
		const AudioFormatFlags AudioFormatFlagIsBigEndian = 0;

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos10.10", "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
		[ObsoletedOSPlatform ("ios8.0", "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
		[ObsoletedOSPlatform ("tvos9.0", "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
		[ObsoletedOSPlatform ("maccatalyst13.1", "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
#else
		[Deprecated (PlatformName.iOS, 8, 0, message: "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Canonical is no longer encouraged, since fixed-point no longer provides a performance advantage over floating point. 'AudioFormatFlagsNativeFloatPacked' is preferred instead.")]
#endif
		public static readonly AudioFormatFlags AudioFormatFlagsAudioUnitCanonical = AudioFormatFlags.IsSignedInteger | (BitConverter.IsLittleEndian ? 0 : AudioFormatFlags.IsBigEndian) |
			AudioFormatFlags.IsPacked | AudioFormatFlags.IsNonInterleaved | (AudioFormatFlags) (AudioUnitSampleFractionBits << (int) AudioFormatFlags.LinearPCMSampleFractionShift);


		public static readonly AudioFormatFlags AudioFormatFlagsNativeFloat = AudioFormatFlags.IsFloat | AudioFormatFlags.IsPacked | (BitConverter.IsLittleEndian ? 0 : AudioFormatFlags.IsBigEndian);

		public static readonly AudioFormatFlags AudioFormatFlagsAudioUnitNativeFloat = AudioFormatFlags.IsFloat | AudioFormatFlags.IsPacked | (BitConverter.IsLittleEndian ? 0 : AudioFormatFlags.IsBigEndian) | AudioFormatFlags.IsNonInterleaved;

		public AudioStreamBasicDescription (AudioFormatType formatType)
			: this ()
		{
			Format = formatType;
		}

		public static AudioStreamBasicDescription CreateLinearPCM (double sampleRate = 44100, uint channelsPerFrame = 2, uint bitsPerChannel = 16, bool bigEndian = false)
		{
			var desc = new AudioStreamBasicDescription (AudioFormatType.LinearPCM);
			desc.SampleRate = sampleRate;
			desc.ChannelsPerFrame = (int) channelsPerFrame;
			desc.BitsPerChannel = (int) bitsPerChannel;
			desc.BytesPerPacket = desc.BytesPerFrame = (int) channelsPerFrame * sizeof (Int16);
			desc.FramesPerPacket = 1;
			desc.FormatFlags = AudioFormatFlags.IsSignedInteger | AudioFormatFlags.IsPacked;
			if (bigEndian)
				desc.FormatFlags |= AudioFormatFlags.IsBigEndian;

			return desc;
		}

		public unsafe static AudioChannelLayoutTag []? GetAvailableEncodeChannelLayoutTags (AudioStreamBasicDescription format)
		{
			var type_size = sizeof (AudioStreamBasicDescription);
			uint size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.AvailableEncodeChannelLayoutTags, type_size, &format, &size) != 0)
				return null;

			var data = new AudioChannelLayoutTag [size / sizeof (AudioChannelLayoutTag)];
			fixed (AudioChannelLayoutTag* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.AvailableEncodeChannelLayoutTags, type_size, &format, &size, (int*) ptr);
				if (res != 0)
					return null;

				return data;
			}
		}

		public unsafe static int []? GetAvailableEncodeNumberChannels (AudioStreamBasicDescription format)
		{
			uint size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.AvailableEncodeNumberChannels, sizeof (AudioStreamBasicDescription), &format, &size) != 0)
				return null;

			var data = new int [size / sizeof (int)];
			fixed (int* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.AvailableEncodeNumberChannels, sizeof (AudioStreamBasicDescription), &format, &size, ptr);
				if (res != 0)
					return null;

				return data;
			}
		}

		public unsafe AudioFormat []? GetOutputFormatList (byte []? magicCookie = null)
		{
			var afi = new AudioFormatInfo ();
			afi.AudioStreamBasicDescription = this;

			var type_size = sizeof (AudioFormat);
			uint size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.OutputFormatList, type_size, &afi, &size) != 0)
				return null;

			Debug.Assert (sizeof (AudioFormat) == type_size);

			var data = new AudioFormat [size / type_size];
			fixed (AudioFormat* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.OutputFormatList, type_size, &afi, &size, ptr);
				if (res != 0)
					return null;

				Array.Resize (ref data, (int) size / type_size);
				return data;
			}
		}

		public unsafe AudioFormat []? GetFormatList (byte [] magicCookie)
		{
			if (magicCookie is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (magicCookie));

			var afi = new AudioFormatInfo ();
			afi.AudioStreamBasicDescription = this;

			fixed (byte* b = magicCookie) {
				afi.MagicCookieWeak = b;
				afi.MagicCookieSize = magicCookie.Length;

				var type_size = sizeof (AudioFormat);
				uint size;
				if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.FormatList, type_size, &afi, &size) != 0)
					return null;

				Debug.Assert (sizeof (AudioFormat) == type_size);

				var data = new AudioFormat [size / type_size];
				fixed (AudioFormat* ptr = data) {
					var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatList, type_size, &afi, &size, ptr);
					if (res != 0)
						return null;

					Array.Resize (ref data, (int) size / type_size);
					return data;
				}
			}
		}

		public static AudioFormatError GetFormatInfo (ref AudioStreamBasicDescription format)
		{
			unsafe {
				var size = sizeof (AudioStreamBasicDescription);
				return AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatInfo, 0, IntPtr.Zero, &size, (AudioStreamBasicDescription*) Unsafe.AsPointer<AudioStreamBasicDescription> (ref format));
			}
		}

		public unsafe string? FormatName {
			get {
				IntPtr ptr;
				var size = sizeof (IntPtr);

				fixed (AudioStreamBasicDescription* self = &this) {
					if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatName, sizeof (AudioStreamBasicDescription), self, &size, &ptr) != 0)
						return null;
				}

				return new CFString (ptr, true);
			}
		}

		public unsafe bool IsEncrypted {
			get {
				uint data;
				var size = sizeof (uint);

				fixed (AudioStreamBasicDescription* self = &this) {
					if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatIsEncrypted, sizeof (AudioStreamBasicDescription), self, &size, &data) != 0)
						return false;
				}

				return data != 0;
			}
		}

		public unsafe bool IsExternallyFramed {
			get {
				uint data;
				var size = sizeof (uint);

				fixed (AudioStreamBasicDescription* self = &this) {
					if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatIsExternallyFramed, sizeof (AudioStreamBasicDescription), self, &size, &data) != 0)
						return false;
				}

				return data != 0;
			}
		}

		public unsafe bool IsVariableBitrate {
			get {
				uint data;
				var size = sizeof (uint);

				fixed (AudioStreamBasicDescription* self = &this) {
					if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FormatIsVBR, sizeof (AudioStreamBasicDescription), self, &size, &data) != 0)
						return false;
				}

				return data != 0;
			}
		}

		public override string ToString ()
		{
			return String.Format ("[SampleRate={0} FormatID={1} FormatFlags={2} BytesPerPacket={3} FramesPerPacket={4} BytesPerFrame={5} ChannelsPerFrame={6} BitsPerChannel={7}]",
						  SampleRate, Format, FormatFlags, BytesPerPacket, FramesPerPacket, BytesPerFrame, ChannelsPerFrame, BitsPerChannel);
		}
#endif // !COREBUILD
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioStreamPacketDescription {
		public long StartOffset;
		public int VariableFramesInPacket;
		public int DataByteSize;

		public override string ToString ()
		{
			return String.Format ("StartOffset={0} VariableFramesInPacket={1} DataByteSize={2}", StartOffset, VariableFramesInPacket, DataByteSize);
		}
	}

	[Flags]
	public enum AudioChannelFlags : uint { // UInt32 in AudioPanningInfo -- AudioFormat.h
		AllOff = 0,
		RectangularCoordinates = 1 << 0,
		SphericalCoordinates = 1 << 1,
		Meters = 1 << 2
	}

	public enum AudioChannelLabel : int { // UInt32 AudioChannelLabel
		/// <summary>To be added.</summary>
		Unknown = -1,
		Unused = 0,
		/// <summary>To be added.</summary>
		UseCoordinates = 100,

		/// <summary>To be added.</summary>
		Left = 1,
		/// <summary>To be added.</summary>
		Right = 2,
		/// <summary>To be added.</summary>
		Center = 3,
		/// <summary>To be added.</summary>
		LFEScreen = 4,
		/// <summary>To be added.</summary>
		LeftSurround = 5,
		/// <summary>To be added.</summary>
		RightSurround = 6,
		/// <summary>To be added.</summary>
		LeftCenter = 7,
		/// <summary>To be added.</summary>
		RightCenter = 8,
		/// <summary>To be added.</summary>
		CenterSurround = 9,
		/// <summary>To be added.</summary>
		LeftSurroundDirect = 10,
		RightSurroundDirect = 11,
		TopCenterSurround = 12,
		VerticalHeightLeft = 13,
		/// <summary>To be added.</summary>
		VerticalHeightCenter = 14,
		/// <summary>To be added.</summary>
		VerticalHeightRight = 15,
		TopBackLeft = 16,
		TopBackCenter = 17,
		TopBackRight = 18,
		RearSurroundLeft = 33,
		RearSurroundRight = 34,
		/// <summary>To be added.</summary>
		LeftWide = 35,
		RightWide = 36,
		LFE2 = 37,
		LeftTotal = 38,
		RightTotal = 39,
		HearingImpaired = 40,
		Narration = 41,
		Mono = 42,
		DialogCentricMix = 43,
		CenterSurroundDirect = 44,
		Haptic = 45,

		LeftTopFront = VerticalHeightLeft,
		CenterTopFront = VerticalHeightCenter,
		RightTopFront = VerticalHeightRight,
		LeftTopMiddle = 49,
		CenterTopMiddle = TopCenterSurround,

		RightTopMiddle = 51,
		LeftTopRear = 52,
		CenterTopRear = 53,
		RightTopRear = 54,

		// first order ambisonic channels
		Ambisonic_W = 200,
		Ambisonic_X = 201,
		Ambisonic_Y = 202,
		Ambisonic_Z = 203,

		// Mid/Side Recording
		MS_Mid = 204,
		MS_Side = 205,

		// X-Y Recording
		XY_X = 206,
		XY_Y = 207,

		// Binaural Recording
		BinauralLeft = 208,
		BinauralRight = 209,

		// other
		HeadphonesLeft = 301,
		HeadphonesRight = 302,
		ClickTrack = 304,
		ForeignLanguage = 305,

		// generic discrete channel
		Discrete = 400,

		// numbered discrete channel
		Discrete_0 = (1 << 16) | 0,
		Discrete_1 = (1 << 16) | 1,
		Discrete_2 = (1 << 16) | 2,
		Discrete_3 = (1 << 16) | 3,
		Discrete_4 = (1 << 16) | 4,
		Discrete_5 = (1 << 16) | 5,
		Discrete_6 = (1 << 16) | 6,
		Discrete_7 = (1 << 16) | 7,
		Discrete_8 = (1 << 16) | 8,
		Discrete_9 = (1 << 16) | 9,
		Discrete_10 = (1 << 16) | 10,
		Discrete_11 = (1 << 16) | 11,
		Discrete_12 = (1 << 16) | 12,
		Discrete_13 = (1 << 16) | 13,
		Discrete_14 = (1 << 16) | 14,
		Discrete_15 = (1 << 16) | 15,
		Discrete_65535 = (1 << 16) | 65535,

		// HOA ACN channels

		// generic
		HoaAcn = 500,

		// numbered
		HoaAcn0 = (2 << 16) | 0,
		HoaAcn1 = (2 << 16) | 1,
		HoaAcn2 = (2 << 16) | 2,
		HoaAcn3 = (2 << 16) | 3,
		HoaAcn4 = (2 << 16) | 4,
		HoaAcn5 = (2 << 16) | 5,
		HoaAcn6 = (2 << 16) | 6,
		HoaAcn7 = (2 << 16) | 7,
		HoaAcn8 = (2 << 16) | 8,
		HoaAcn9 = (2 << 16) | 9,
		HoaAcn10 = (2 << 16) | 10,
		HoaAcn11 = (2 << 16) | 11,
		HoaAcn12 = (2 << 16) | 12,
		HoaAcn13 = (2 << 16) | 13,
		HoaAcn14 = (2 << 16) | 14,
		HoaAcn15 = (2 << 16) | 15,
		HoaAcn65024 = (2 << 16) | 65024,
		HoaSn3d = HoaAcn0,
		HoaN3d = (3 << 16),
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class AudioChannelLabelExtensions {
		public static bool IsReserved (this AudioChannelLabel value)
		{
			return (uint) value >= 0xF0000000 && (uint) value <= 0xFFFFFFFE;
		}
	}
#endif

	[Flags]
	[NativeName ("AudioChannelBitmap")]
	public enum AudioChannelBit : uint // UInt32 mChannelBitmap in AudioChannelLayout
	{
		Left = 1 << 0,
		Right = 1 << 1,
		Center = 1 << 2,
		LFEScreen = 1 << 3,
		LeftSurround = 1 << 4,
		RightSurround = 1 << 5,
		LeftCenter = 1 << 6,
		RightCenter = 1 << 7,
		CenterSurround = 1 << 8,
		LeftSurroundDirect = 1 << 9,
		RightSurroundDirect = 1 << 10,
		TopCenterSurround = 1 << 11,
		VerticalHeightLeft = 1 << 12,
		VerticalHeightCenter = 1 << 13,
		VerticalHeightRight = 1 << 14,
		TopBackLeft = 1 << 15,
		TopBackCenter = 1 << 16,
		TopBackRight = 1 << 17,

		LeftTopFront = VerticalHeightLeft,
		CenterTopFront = VerticalHeightCenter,
		RightTopFront = VerticalHeightRight,
		LeftTopMiddle = 1 << 21,
		CenterTopMiddle = TopCenterSurround,

		RightTopMiddle = 1 << 23,
		LeftTopRear = 1 << 24,
		CenterTopRear = 1 << 25,
		RightTopRear = 1 << 26,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioChannelDescription {
		public AudioChannelLabel Label;
		public AudioChannelFlags Flags;
		float Coord0, Coord1, Coord2;
#if !COREBUILD

		public float [] Coords {
			get {
				return new float [3] { Coord0, Coord1, Coord2 };
			}
			set {
				if (value.Length != 3)
					throw new ArgumentException ("Must contain three floats");
				Coord0 = value [0];
				Coord1 = value [1];
				Coord2 = value [2];
			}
		}

		public unsafe string? Name {
			get {
				IntPtr sptr;
				int size = sizeof (IntPtr);
				int ptr_size = sizeof (AudioChannelDescription);
				var ptr = ToPointer ();

				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ChannelName, ptr_size, ptr, &size, &sptr);
				Marshal.FreeHGlobal (ptr);
				if (res != 0)
					return null;

				return new CFString (sptr, true);
			}
		}

		public unsafe string? ShortName {
			get {
				IntPtr sptr;
				int size = sizeof (IntPtr);
				int ptr_size = sizeof (AudioChannelDescription);
				var ptr = ToPointer ();

				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ChannelShortName, ptr_size, ptr, &size, &sptr);
				Marshal.FreeHGlobal (ptr);
				if (res != 0)
					return null;

				return new CFString (sptr, true);
			}
		}

		internal unsafe IntPtr ToPointer ()
		{
			var ptr = (AudioChannelDescription*) Marshal.AllocHGlobal (sizeof (AudioChannelLabel) + sizeof (AudioChannelFlags) + 3 * sizeof (float));
			*ptr = this;
			return (IntPtr) ptr;
		}

		public override string ToString ()
		{
			return String.Format ("[id={0} {1} - {2},{3},{4}", Label, Flags, Coords [0], Coords [1], Coords [2]);
		}
#endif // !COREBUILD
	}

	// CoreAudioTypes.framework/Headers/CoreAudioBaseTypes.h
	public enum AudioChannelLayoutTag : uint { // UInt32 AudioChannelLayoutTag
		UseChannelDescriptions = (0 << 16) | 0,
		UseChannelBitmap = (1 << 16) | 0,

		Mono = (100 << 16) | 1,
		Stereo = (101 << 16) | 2,
		StereoHeadphones = (102 << 16) | 2,
		MatrixStereo = (103 << 16) | 2,
		MidSide = (104 << 16) | 2,
		XY = (105 << 16) | 2,
		Binaural = (106 << 16) | 2,
		Ambisonic_B_Format = (107 << 16) | 4,

		Quadraphonic = (108 << 16) | 4,
		Pentagonal = (109 << 16) | 5,
		Hexagonal = (110 << 16) | 6,
		Octagonal = (111 << 16) | 8,
		Cube = (112 << 16) | 8,


		MPEG_1_0 = Mono,
		MPEG_2_0 = Stereo,
		MPEG_3_0_A = (113 << 16) | 3,
		MPEG_3_0_B = (114 << 16) | 3,
		MPEG_4_0_A = (115 << 16) | 4,
		MPEG_4_0_B = (116 << 16) | 4,
		MPEG_5_0_A = (117 << 16) | 5,
		MPEG_5_0_B = (118 << 16) | 5,
		MPEG_5_0_C = (119 << 16) | 5,
		MPEG_5_0_D = (120 << 16) | 5,
		MPEG_5_1_A = (121 << 16) | 6,
		MPEG_5_1_B = (122 << 16) | 6,
		MPEG_5_1_C = (123 << 16) | 6,
		MPEG_5_1_D = (124 << 16) | 6,
		MPEG_6_1_A = (125 << 16) | 7,
		MPEG_7_1_A = (126 << 16) | 8,
		MPEG_7_1_B = (127 << 16) | 8,
		MPEG_7_1_C = (128 << 16) | 8,
		Emagic_Default_7_1 = (129 << 16) | 8,
		SMPTE_DTV = (130 << 16) | 8,

		ITU_1_0 = Mono,
		ITU_2_0 = Stereo,

		ITU_2_1 = (131 << 16) | 3,
		ITU_2_2 = (132 << 16) | 4,
		ITU_3_0 = MPEG_3_0_A,
		ITU_3_1 = MPEG_4_0_A,

		ITU_3_2 = MPEG_5_0_A,
		ITU_3_2_1 = MPEG_5_1_A,
		ITU_3_4_1 = MPEG_7_1_C,


		DVD_0 = Mono,
		DVD_1 = Stereo,
		DVD_2 = ITU_2_1,
		DVD_3 = ITU_2_2,
		DVD_4 = (133 << 16) | 3,
		DVD_5 = (134 << 16) | 4,
		DVD_6 = (135 << 16) | 5,
		DVD_7 = MPEG_3_0_A,
		DVD_8 = MPEG_4_0_A,
		DVD_9 = MPEG_5_0_A,
		DVD_10 = (136 << 16) | 4,
		DVD_11 = (137 << 16) | 5,
		DVD_12 = MPEG_5_1_A,

		DVD_13 = DVD_8,
		DVD_14 = DVD_9,
		DVD_15 = DVD_10,
		DVD_16 = DVD_11,
		DVD_17 = DVD_12,
		DVD_18 = (138 << 16) | 5,
		DVD_19 = MPEG_5_0_B,
		DVD_20 = MPEG_5_1_B,

		AudioUnit_4 = Quadraphonic,
		AudioUnit_5 = Pentagonal,
		AudioUnit_6 = Hexagonal,
		AudioUnit_8 = Octagonal,

		AudioUnit_5_0 = MPEG_5_0_B,
		AudioUnit_6_0 = (139 << 16) | 6,
		AudioUnit_7_0 = (140 << 16) | 7,
		AudioUnit_7_0_Front = (148 << 16) | 7,
		AudioUnit_5_1 = MPEG_5_1_A,
		AudioUnit_6_1 = MPEG_6_1_A,
		AudioUnit_7_1 = MPEG_7_1_C,
		AudioUnit_7_1_Front = MPEG_7_1_A,

		AAC_3_0 = MPEG_3_0_B,
		AAC_Quadraphonic = Quadraphonic,
		AAC_4_0 = MPEG_4_0_B,
		AAC_5_0 = MPEG_5_0_D,
		AAC_5_1 = MPEG_5_1_D,
		AAC_6_0 = (141 << 16) | 6,
		AAC_6_1 = (142 << 16) | 7,
		AAC_7_0 = (143 << 16) | 7,
		AAC_7_1 = MPEG_7_1_B,
		AAC_7_1_B = (183 << 16) | 8,
		AAC_7_1_C = (184 << 16) | 8,
		AAC_Octagonal = (144 << 16) | 8,

		TMH_10_2_std = (145 << 16) | 16,
		TMH_10_2_full = (146 << 16) | 21,

		AC3_1_0_1 = (149 << 16) | 2,
		AC3_3_0 = (150 << 16) | 3,
		AC3_3_1 = (151 << 16) | 4,
		AC3_3_0_1 = (152 << 16) | 4,
		AC3_2_1_1 = (153 << 16) | 4,
		AC3_3_1_1 = (154 << 16) | 5,

		EAC_6_0_A = (155 << 16) | 6,
		EAC_7_0_A = (156 << 16) | 7,

		EAC3_6_1_A = (157 << 16) | 7,
		EAC3_6_1_B = (158 << 16) | 7,
		EAC3_6_1_C = (159 << 16) | 7,
		EAC3_7_1_A = (160 << 16) | 8,
		EAC3_7_1_B = (161 << 16) | 8,
		EAC3_7_1_C = (162 << 16) | 8,
		EAC3_7_1_D = (163 << 16) | 8,
		EAC3_7_1_E = (164 << 16) | 8,

		EAC3_7_1_F = (165 << 16) | 8,
		EAC3_7_1_G = (166 << 16) | 8,
		EAC3_7_1_H = (167 << 16) | 8,

		DTS_3_1 = (168 << 16) | 4,
		DTS_4_1 = (169 << 16) | 5,
		DTS_6_0_A = (170 << 16) | 6,
		DTS_6_0_B = (171 << 16) | 6,
		DTS_6_0_C = (172 << 16) | 6,
		DTS_6_1_A = (173 << 16) | 7,
		DTS_6_1_B = (174 << 16) | 7,
		DTS_6_1_C = (175 << 16) | 7,
		DTS_7_0 = (176 << 16) | 7,
		DTS_7_1 = (177 << 16) | 8,
		DTS_8_0_A = (178 << 16) | 8,
		DTS_8_0_B = (179 << 16) | 8,
		DTS_8_1_A = (180 << 16) | 9,
		DTS_8_1_B = (181 << 16) | 9,
		DTS_6_1_D = (182 << 16) | 7,

		Wave_2_1 = DVD_4,
		Wave_3_0 = MPEG_3_0_A,
		Wave_4_0_A = ITU_2_2,
		Wave_4_0_B = (185 << 16) | 4,
		Wave_5_0_A = MPEG_5_0_A,
		Wave_5_0_B = (186 << 16) | 5,
		Wave_5_1_A = MPEG_5_1_A,
		Wave_5_1_B = (187 << 16) | 6,
		Wave_6_1 = (188 << 16) | 7,
		Wave_7_1 = (189 << 16) | 8,

		HOA_ACN_SN3D = (190U << 16),
		HOA_ACN_N3D = (191U << 16),

		Atmos_7_1_4 = (192 << 16) | 12,
		Atmos_9_1_6 = (193 << 16) | 16,
		Atmos_5_1_2 = (194 << 16) | 8,
		Atmos_5_1_4 = (195U << 16) | 10,
		Atmos_7_1_2 = (196U << 16) | 10,

		Logic_Mono = Mono,
		Logic_Stereo = Stereo,
		Logic_Quadraphonic = Quadraphonic,
		Logic_4_0_A = MPEG_4_0_A,
		Logic_4_0_B = MPEG_4_0_B,
		Logic_4_0_C = (197U << 16) | 4,
		Logic_5_0_A = MPEG_5_0_A,
		Logic_5_0_B = MPEG_5_0_B,
		Logic_5_0_C = MPEG_5_0_C,
		Logic_5_0_D = MPEG_5_0_D,
		Logic_5_1_A = MPEG_5_1_A,
		Logic_5_1_B = MPEG_5_1_B,
		Logic_5_1_C = MPEG_5_1_C,
		Logic_5_1_D = MPEG_5_1_D,
		Logic_6_0_A = AAC_6_0,
		Logic_6_0_B = (198U << 16) | 6,
		Logic_6_0_C = AudioUnit_6_0,
		Logic_6_1_A = AAC_6_1,
		Logic_6_1_B = (199U << 16) | 7,
		Logic_6_1_C = MPEG_6_1_A,
		Logic_6_1_D = (200U << 16) | 7,
		Logic_7_1_A = AudioUnit_7_1,
		Logic_7_1_B = (201U << 16) | 8,
		Logic_7_1_C = MPEG_7_1_C,
		Logic_7_1_SDDS_A = MPEG_7_1_A,
		Logic_7_1_SDDS_B = MPEG_7_1_B,
		Logic_7_1_SDDS_C = Emagic_Default_7_1,
		Logic_Atmos_5_1_2 = Atmos_5_1_2,
		Logic_Atmos_5_1_4 = Atmos_5_1_4,
		Logic_Atmos_7_1_2 = Atmos_7_1_2,
		Logic_Atmos_7_1_4_A = Atmos_7_1_4,
		Logic_Atmos_7_1_4_B = (202U << 16) | 12,
		Logic_Atmos_7_1_6 = (203U << 16) | 14,

		DiscreteInOrder = (147 << 16) | 0,                       // needs to be ORed with the actual number of channels  


#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_3_0 = AC3_3_0,
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_4_0 = Wave_4_0_B,
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_5_0 = (212U << 16) | 5,
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_5_1 = (213U << 16) | 6,
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_6_1 = (214U << 16) | 7,
#if NET
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
#else
		[Mac (14, 0)]
		[iOS (17, 0)]
		[TV (17, 0)]
		[MacCatalyst (17, 0)]
#endif
		Ogg_7_1 = (215U << 16) | 8,

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		MPEG_5_0_E = (216U << 16) | 5,

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		MPEG_5_1_E = (217U << 16) | 6,

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		MPEG_6_1_B = (218U << 16) | 7,

#if NET
		[SupportedOSPlatform ("ios18.0")]
		[SupportedOSPlatform ("maccatalyst18.0")]
		[SupportedOSPlatform ("macos15.0")]
		[SupportedOSPlatform ("tvos18.0")]
#else
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
#endif
		MPEG_7_1_D = (219U << 16) | 8,

		Unknown = 0xFFFF0000                           // needs to be ORed with the actual number of channels  
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class AudioChannelLayoutTagExtensions {
		public static AudioChannelBit? ToAudioChannel (this AudioChannelLayoutTag layoutTag)
		{
			int value;
			int size = sizeof (uint);
			int layout = (int) layoutTag;

			unsafe {
				if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.BitmapForLayoutTag, sizeof (AudioChannelLayoutTag), &layout, &size, &value) != 0)
					return null;
			}

			return (AudioChannelBit) value;
		}

		public static uint GetNumberOfChannels (this AudioChannelLayoutTag inLayoutTag)
		{
			return (uint) inLayoutTag & 0x0000FFFF;
		}

		public static bool IsReserved (this AudioChannelLayoutTag value)
		{
			return (uint) value >= 0xF0000000 && (uint) value <= 0xFFFFFFFE;
		}
	}
#endif // !COREBUILD

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[DebuggerDisplay ("{Name}")]
	public class AudioChannelLayout {
#if !COREBUILD
		public AudioChannelLayout ()
		{
		}

		internal unsafe AudioChannelLayout (IntPtr h)
		{
			AudioTag = (AudioChannelLayoutTag) Marshal.ReadInt32 (h, 0);
			ChannelUsage = (AudioChannelBit) Marshal.ReadInt32 (h, 4);
			Channels = new AudioChannelDescription [Marshal.ReadInt32 (h, 8)];
			int p = 12;
			int size = sizeof (AudioChannelDescription);
			for (int i = 0; i < Channels.Length; i++) {
				Channels [i] = *(AudioChannelDescription*) (unchecked(((byte*) h) + p));
				p += size;
			}
		}

		[Advice ("Use the strongly typed 'AudioTag' instead.")]
		public int Tag {
			get {
				return (int) AudioTag;
			}
			set {
				AudioTag = (AudioChannelLayoutTag) value;
			}
		}

		[Advice ("Use 'ChannelUsage' instead.")]
		public int Bitmap {
			get {
				return (int) ChannelUsage;
			}
			set {
				ChannelUsage = (AudioChannelBit) value;
			}
		}

		public AudioChannelLayoutTag AudioTag;
		public AudioChannelBit ChannelUsage;
		public AudioChannelDescription []? Channels;

		public unsafe string? Name {
			get {
				IntPtr sptr;
				int size = sizeof (IntPtr);
				int ptr_size;
				var ptr = ToBlock (out ptr_size);

				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ChannelLayoutName, ptr_size, ptr, &size, &sptr);
				Marshal.FreeHGlobal (ptr);
				if (res != 0)
					return null;

				return new CFString (sptr, true);
			}
		}

		public unsafe string? SimpleName {
			get {
				IntPtr sptr;
				int size = sizeof (IntPtr);
				int ptr_size;
				var ptr = ToBlock (out ptr_size);

				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ChannelLayoutSimpleName, ptr_size, ptr, &size, &sptr);
				Marshal.FreeHGlobal (ptr);
				if (res != 0)
					return null;

				return new CFString (sptr, true);
			}
		}

		public static AudioChannelLayout? FromAudioChannelBitmap (AudioChannelBit channelBitmap)
		{
			return GetChannelLayout (AudioFormatProperty.ChannelLayoutForBitmap, (int) channelBitmap);
		}

		public static AudioChannelLayout? FromAudioChannelLayoutTag (AudioChannelLayoutTag channelLayoutTag)
		{
			return GetChannelLayout (AudioFormatProperty.ChannelLayoutForTag, (int) channelLayoutTag);
		}

		static AudioChannelLayout? GetChannelLayout (AudioFormatProperty property, int value)
		{
			int size;
			unsafe {
				AudioFormatPropertyNative.AudioFormatGetPropertyInfo (property, sizeof (AudioFormatProperty), &value, &size);
			}

			AudioChannelLayout? layout;
			IntPtr ptr = Marshal.AllocHGlobal (size);
			AudioFormatError rv;
			unsafe {
				rv = AudioFormatPropertyNative.AudioFormatGetProperty (property, sizeof (AudioFormatProperty), &value, &size, ptr);
			}

			if (rv == 0)
				layout = new AudioChannelLayout (ptr);
			else
				layout = null;

			Marshal.FreeHGlobal (ptr);
			return layout;
		}

		internal static AudioChannelLayout? FromHandle (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			return new AudioChannelLayout (handle);
		}

		public override string ToString ()
		{
			return String.Format ("AudioChannelLayout: Tag={0} Bitmap={1} Channels={2}", AudioTag, ChannelUsage, Channels!.Length);
		}

		// The returned block must be released with FreeHGlobal
		internal unsafe IntPtr ToBlock (out int size)
		{
			if (Channels is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (Channels));

			var desc_size = sizeof (AudioChannelDescription);

			size = 12 + Channels.Length * desc_size;
			IntPtr buffer = Marshal.AllocHGlobal (size);
			Marshal.WriteInt32 (buffer, (int) AudioTag);
			Marshal.WriteInt32 (buffer, 4, (int) ChannelUsage);
			Marshal.WriteInt32 (buffer, 8, Channels.Length);

			AudioChannelDescription* dest = (AudioChannelDescription*) ((byte*) buffer + 12);
			foreach (var desc in Channels) {
				*dest = desc;
				dest++;
			}

			return buffer;
		}

		public static AudioFormatError Validate (AudioChannelLayout layout)
		{
			if (layout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (layout));

			int ptr_size;
			var ptr = layout.ToBlock (out ptr_size);

			var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ValidateChannelLayout, ptr_size, ptr, IntPtr.Zero, IntPtr.Zero);
			Marshal.FreeHGlobal (ptr);
			return res;
		}

		public unsafe static int []? GetChannelMap (AudioChannelLayout inputLayout, AudioChannelLayout outputLayout)
		{
			if (inputLayout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inputLayout));
			if (outputLayout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputLayout));

			var channels_count = GetNumberOfChannels (outputLayout);
			if (channels_count is null)
				throw new ArgumentException ("outputLayout");

			int ptr_size;
			var input_ptr = inputLayout.ToBlock (out ptr_size);
			var output_ptr = outputLayout.ToBlock (out ptr_size);
			var array = new IntPtr [] { input_ptr, output_ptr };
			ptr_size = sizeof (IntPtr) * array.Length;

			int [] value;
			AudioFormatError res;

			fixed (IntPtr* ptr = array) {
				value = new int [channels_count.Value];
				var size = sizeof (int) * value.Length;
				fixed (int* value_ptr = value) {
					res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.ChannelMap, ptr_size, ptr, &size, value_ptr);
				}
			}

			Marshal.FreeHGlobal (input_ptr);
			Marshal.FreeHGlobal (output_ptr);

			return res == 0 ? value : null;
		}

		public unsafe static float [,]? GetMatrixMixMap (AudioChannelLayout inputLayout, AudioChannelLayout outputLayout)
		{
			if (inputLayout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inputLayout));
			if (outputLayout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputLayout));

			var channels_count_output = GetNumberOfChannels (outputLayout);
			if (channels_count_output is null)
				throw new ArgumentException ("outputLayout");

			var channels_count_input = GetNumberOfChannels (inputLayout);
			if (channels_count_input is null)
				throw new ArgumentException ("inputLayout");

			int ptr_size;
			var input_ptr = inputLayout.ToBlock (out ptr_size);
			var output_ptr = outputLayout.ToBlock (out ptr_size);
			var array = new IntPtr [] { input_ptr, output_ptr };
			ptr_size = sizeof (IntPtr) * array.Length;

			float [,] value;
			AudioFormatError res;

			fixed (IntPtr* ptr = array) {
				value = new float [channels_count_input.Value, channels_count_output.Value];
				var size = sizeof (float) * channels_count_input.Value * channels_count_output.Value;
				fixed (float* value_ptr = &value [0, 0]) {
					res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.MatrixMixMap, ptr_size, ptr, &size, value_ptr);
				}
			}

			Marshal.FreeHGlobal (input_ptr);
			Marshal.FreeHGlobal (output_ptr);

			return res == 0 ? value : null;
		}

		public static int? GetNumberOfChannels (AudioChannelLayout layout)
		{
			if (layout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (layout));

			int ptr_size;
			var ptr = layout.ToBlock (out ptr_size);
			int size = sizeof (int);
			int value;
			AudioFormatError res;
			unsafe {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.NumberOfChannelsForLayout, ptr_size, ptr, &size, &value);
			}
			Marshal.FreeHGlobal (ptr);
			return res != 0 ? null : (int?) value;
		}

		public static AudioChannelLayoutTag? GetTagForChannelLayout (AudioChannelLayout layout)
		{
			if (layout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (layout));

			int ptr_size;
			var ptr = layout.ToBlock (out ptr_size);
			int size = sizeof (AudioChannelLayoutTag);
			int value;

			AudioFormatError res;
			unsafe {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.TagForChannelLayout, ptr_size, ptr, &size, &value);
			}
			Marshal.FreeHGlobal (ptr);
			return res != 0 ? null : (AudioChannelLayoutTag?) value;
		}

		public unsafe static AudioChannelLayoutTag []? GetTagsForNumberOfChannels (int count)
		{
			const int type_size = sizeof (uint);
			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.TagsForNumberOfChannels, type_size, &count, &size) != 0)
				return null;

			var data = new AudioChannelLayoutTag [size / type_size];
			fixed (AudioChannelLayoutTag* ptr = data) {
				var res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.TagsForNumberOfChannels, type_size, &count, &size, (int*) ptr);
				if (res != 0)
					return null;

				return data;
			}
		}

		public NSData AsData ()
		{
			int size;

			var p = ToBlock (out size);
			var result = NSData.FromBytes (p, (uint) size);
			Marshal.FreeHGlobal (p);
			return result;
		}
#endif // !COREBUILD
	}

	[Flags]
	public enum SmpteTimeFlags : uint { // UInt32
		Unknown = 0,
		TimeValid = 1 << 0,
		TimeRunning = 1 << 1
	}

	public enum MPEG4ObjectID { // long
		AacMain = 1,
		AacLc = 2,
		AacSsr = 3,
		AacLtp = 4,
		AacSbr = 5,
		AacScalable = 6,
		TwinVq = 7,
		Celp = 8,
		Hvxc = 9
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct SmpteTime { // CoreAudio.framework - CoreAudioTypes.h
		public short Subframes;
		public short SubframeDivisor;
		public uint Counter;
		public uint Type;
		public uint Flags;
		public short Hours;
		public short Minutes;
		public short Seconds;
		public short Frames;

		public SmpteTimeFlags FlagsStrong {
			get {
				return (SmpteTimeFlags) Flags;
			}
			set {
				Flags = (uint) value;
			}
		}
		public SmpteTimeType TypeStrong {
			get {
				return (SmpteTimeType) Type;
			}
			set {
				Type = (uint) value;
			}
		}

		public override string ToString ()
		{
			return String.Format ("[Subframes={0},Divisor={1},Counter={2},Type={3},Flags={4},Hours={5},Minutes={6},Seconds={7},Frames={8}]",
						  Subframes, SubframeDivisor, Counter, Type, Flags, Hours, Minutes, Seconds, Frames);
		}
	}

	public enum SmpteTimeType : uint // UInt32 in AudioFileRegionList
	{
#if !NET
		[Obsolete ("Value is not to be used with any API.")]
		None = uint.MaxValue,
#endif
		Type24 = 0,
		Type25 = 1,
		Type30Drop = 2,
		Type30 = 3,
		Type2997 = 4,
		Type2997Drop = 5,
		Type60 = 6,
		Type5994 = 7,
		Type60Drop = 8,
		Type5994Drop = 9,
		Type50 = 10,
		Type2398 = 11,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioTimeStamp {

		[Flags]
		public enum AtsFlags : uint { // UInt32 in AudioTimeStamp
			NothingValid = 0,
			SampleTimeValid = (1 << 0),
			HostTimeValid = (1 << 1),
			RateScalarValid = (1 << 2),
			WordClockTimeValid = (1 << 3),
			SmpteTimeValid = (1 << 4),
			SampleHostTimeValid = SampleTimeValid | HostTimeValid
		}

		public double SampleTime;
		public ulong HostTime;
		public double RateScalar;
		public ulong WordClockTime;
		public SmpteTime SMPTETime;
		public AtsFlags Flags;
		public uint Reserved;

		public override string ToString ()
		{
			var sb = new StringBuilder ("{");
			if ((Flags & AtsFlags.SampleTimeValid) != 0)
				sb.Append ("SampleTime=" + SampleTime.ToString ());

			if ((Flags & AtsFlags.HostTimeValid) != 0) {
				if (sb.Length > 0)
					sb.Append (',');
				sb.Append ("HostTime=" + HostTime.ToString ());
			}

			if ((Flags & AtsFlags.RateScalarValid) != 0) {
				if (sb.Length > 0)
					sb.Append (',');

				sb.Append ("RateScalar=" + RateScalar.ToString ());
			}

			if ((Flags & AtsFlags.WordClockTimeValid) != 0) {
				if (sb.Length > 0)
					sb.Append (',');
				sb.Append ("WordClock=" + HostTime.ToString () + ",");
			}

			if ((Flags & AtsFlags.SmpteTimeValid) != 0) {
				if (sb.Length > 0)
					sb.Append (',');
				sb.Append ("SmpteTime=" + SMPTETime.ToString ());
			}
			sb.Append ("}");

			return sb.ToString ();
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioBuffer {
		public int NumberChannels;
		public int DataByteSize;
		public IntPtr Data;

		public override string ToString ()
		{
			return string.Format ("[channels={0},dataByteSize={1},ptrData=0x{2:x}]", NumberChannels, DataByteSize, Data);
		}
	}

	/// <summary>This struct represents the native <see href="https://developer.apple.com/documentation/coreaudiotypes/audiobufferlist">AudioBufferList</see> struct.</summary>
	/// <remarks>
	///   <para>
	///     Typically it's better to use the <see cref="AudioBuffers" /> class to wrap a pointer to a native AudioBufferList,
	///     but some audio code needs to minimize memory allocations due to being executed in a realtime thread. In that case,
	///     using this struct is better, because it's possible to use it without incurring any memory allocations.
	///   </para>
	///
	///   <para>
	///     Note that this struct should never be created in C#, the only valid way to use it is to cast a pointer (<see cref="IntPtr" />)
	///     to a pointer of this struct:
	///   </para>
	///
	///   <example>
	///     <code lang="csharp lang-csharp"><![CDATA[
	/// public unsafe static void Callback (IntPtr audioBufferListPtr) {
	///     AudioBufferList* audioBufferList = (AudioBufferList* ) audioBufferListPtr;
	///     for (var i = 0; i < audioBufferList->Count; i++) {
	///         AudioBuffer* buffer = audioBufferList->GetBuffer (index),
	///         // Use the buffer for something
	///     }
	/// }
	///   ]]></code>
	///   </example>
	/// </remarks>
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public unsafe readonly ref struct AudioBufferList {
		readonly uint mNumberOfBuffers;

		/// <summary>Returns the number of audio buffers in this list.</summary>
		public uint Count { get => mNumberOfBuffers; }

		/// <summary>Return a pointer to the <see cref="AudioBuffer" /> at the specified index.</summary>
		/// <param name="index">The index of the <see cref="AudioBuffer" /> to retrieve.</param>
		/// <returns>A pointer to the <see cref="AudioBuffer" /> at the specified index.</returns>
		public AudioBuffer* GetBuffer (int index)
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException (nameof (index));

			//
			// Decodes
			//
			// struct AudioBufferList
			// {
			//    UInt32      mNumberBuffers;
			//    AudioBuffer mBuffers[1]; // this is a variable length array of mNumberBuffers elements
			// }
			//
			fixed (uint* bufferPtr = &mNumberOfBuffers) {
				byte* baddress = (byte*) bufferPtr;

				var ptr = baddress + IntPtr.Size + index * sizeof (AudioBuffer);
				return (AudioBuffer*) ptr;
			}
		}
	}

	// CoreAudioClock.h (inside AudioToolbox)
	// It was a confusion between CA (CoreAudio) and CA (CoreAnimation)
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct CABarBeatTime {
		public /* SInt32 */ int Bar;
		public /* UInt16 */ ushort Beat;
		public /* UInt16 */ ushort Subbeat;
		public /* UInt16 */ ushort SubbeatDivisor;
		public /* UInt16 */ ushort Reserved;
	}
}
