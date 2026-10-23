// 
// AudioFormat.cs:
//
// Authors:
//    Miguel de Icaza (miguel@xamarin.com)
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

using System.IO;
using System.Collections.Generic;
using CoreFoundation;

using AudioFileID = System.IntPtr;

namespace AudioToolbox {

	// AudioFormatListItem
	/// <summary>Tuple structure that encapsulates both an AudioChannelLayoutTag and an AudioStreamBasicDescription.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioFormat {
		/// <summary>The AudioStreamBasicDescription.</summary>
		///         <remarks>
		///         </remarks>
		public AudioStreamBasicDescription AudioStreamBasicDescription;
		/// <summary>The AudioChannelLayoutTag</summary>
		///         <remarks>
		///         </remarks>
		public AudioChannelLayoutTag AudioChannelLayoutTag;

		/// <param name="formatList">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public unsafe static AudioFormat? GetFirstPlayableFormat (AudioFormat [] formatList)
		{
			if (formatList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (formatList));
			if (formatList.Length < 2)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (formatList));

			fixed (AudioFormat* item = formatList) {
				uint index;
				int size = sizeof (uint);
				var ptr_size = sizeof (AudioFormat) * formatList.Length;
				if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FirstPlayableFormatFromList, ptr_size, item, &size, &index) != 0)
					return null;
				return formatList [index];
			}
		}

		/// <summary>Returns a human-readable reprensetation of the tuple.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public override string ToString ()
		{
			return AudioChannelLayoutTag + ":" + AudioStreamBasicDescription.ToString ();
		}
	}

	/// <summary>An enumeration whose values specify various errors relating to audio formats.</summary>
	///     <remarks>To be added.</remarks>
	public enum AudioFormatError : int // Implictly cast to OSType
	{
		/// <summary>To be added.</summary>
		None = 0,
		/// <summary>To be added.</summary>
		Unspecified = 0x77686174,   // 'what'
		/// <summary>To be added.</summary>
		UnsupportedProperty = 0x70726f70,   // 'prop'
		/// <summary>To be added.</summary>
		BadPropertySize = 0x2173697a,   // '!siz'
		/// <summary>To be added.</summary>
		BadSpecifierSize = 0x21737063,  // '!spc'
		/// <summary>To be added.</summary>
		UnsupportedDataFormat = 0x666d743f, // 'fmt?'
		/// <summary>To be added.</summary>
		UnknownFormat = 0x21666d74, // '!fmt'
		/// <summary>The format is unsupported.</summary>
		UnsupportedFormat = 0x21646174, // '!dat'
	}

	/// <summary>A struct that holds minimum and maximum float values, indicating a range.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[StructLayout (LayoutKind.Sequential)]
	public struct AudioValueRange {
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double Minimum;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public double Maximum;
	}

	/// <summary>An enumeration whose values specify whether balance/fade manipulation should always have a gain of less than 1.0.</summary>
	///     <remarks>To be added.</remarks>
	public enum AudioBalanceFadeType : uint // UInt32 in AudioBalanceFades
	{
		/// <summary>Overall gain is not allowed to exceed 1.0.</summary>
		MaxUnityGain = 0,
		/// <summary>Overall loudness remains constant, but gain may be as high as 1.414 (+3dB).</summary>
		EqualPower = 1,
	}

	/// <summary>Holds left/right balance and front/back fade values.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioBalanceFade {
#if !COREBUILD
		[StructLayout (LayoutKind.Sequential)]
		struct Layout {
			public float LeftRightBalance;
			public float BackFrontFade;
			public AudioBalanceFadeType Type;
			public IntPtr ChannelLayoutWeak;
		}

		/// <param name="channelLayout">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public AudioBalanceFade (AudioChannelLayout channelLayout)
		{
			if (channelLayout is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (channelLayout));

			this.ChannelLayout = channelLayout;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float LeftRightBalance { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float BackFrontFade { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioBalanceFadeType Type { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioChannelLayout ChannelLayout { get; private set; }

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public unsafe float []? GetBalanceFade ()
		{
			var type_size = sizeof (Layout);

			var str = ToStruct ();
			var ptr = Marshal.AllocHGlobal (type_size);
			(*(Layout*) ptr) = str;

			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.BalanceFade, type_size, ptr, &size) != 0)
				return null;

			AudioFormatError res;
			var data = new float [size / sizeof (float)];
			fixed (float* data_ptr = data) {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.BalanceFade, type_size, ptr, &size, data_ptr);
			}

			Marshal.FreeHGlobal (str.ChannelLayoutWeak);
			Marshal.FreeHGlobal (ptr);

			return res == 0 ? data : null;
		}

		Layout ToStruct ()
		{
			var l = new Layout () {
				LeftRightBalance = LeftRightBalance,
				BackFrontFade = BackFrontFade,
				Type = Type,
			};

			if (ChannelLayout is not null) {
				int temp;
				l.ChannelLayoutWeak = ChannelLayout.ToBlock (out temp);
			}

			return l;
		}
#endif // !COREBUILD
	}

	/// <summary>An enumeration whose values specify the panning mode (sound-field vs. vector-based).</summary>
	///     <remarks>To be added.</remarks>
	[NativeName ("AudioPanningMode")]
	public enum PanningMode : uint // UInt32 in AudioPanningInfo
	{
		/// <summary>To be added.</summary>
		SoundField = 3,
		/// <summary>To be added.</summary>
		VectorBasedPanning = 4,
	}

	/// <summary>Information on audio panning.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class AudioPanningInfo {
#if !COREBUILD
		[StructLayout (LayoutKind.Sequential)]
		struct Layout {
			public PanningMode PanningMode;
			public AudioChannelFlags CoordinateFlags;
			public float Coord0;
			public float Coord1;
			public float Coord2;
			public float GainScale;
			public IntPtr OutputChannelMapWeak;
		}

		/// <param name="outputChannelMap">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public AudioPanningInfo (AudioChannelLayout outputChannelMap)
		{
			if (outputChannelMap is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputChannelMap));

			this.OutputChannelMap = outputChannelMap;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public PanningMode PanningMode { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioChannelFlags CoordinateFlags { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float [] Coordinates { get; private set; } = Array.Empty<float> ();
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public float GainScale { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public AudioChannelLayout OutputChannelMap { get; private set; }

		/// <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public unsafe float []? GetPanningMatrix ()
		{
			var type_size = sizeof (Layout);

			var str = ToStruct ();
			var ptr = Marshal.AllocHGlobal (type_size);
			*((Layout*) ptr) = str;

			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.PanningMatrix, type_size, ptr, &size) != 0)
				return null;

			AudioFormatError res;
			var data = new float [size / sizeof (float)];
			fixed (float* data_ptr = data) {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.PanningMatrix, type_size, ptr, &size, data_ptr);
			}

			Marshal.FreeHGlobal (str.OutputChannelMapWeak);
			Marshal.FreeHGlobal (ptr);

			return res == 0 ? data : null;
		}

		Layout ToStruct ()
		{
			var l = new Layout () {
				PanningMode = PanningMode,
				CoordinateFlags = CoordinateFlags,
				Coord0 = Coordinates [0],
				Coord1 = Coordinates [1],
				Coord2 = Coordinates [2],
				GainScale = GainScale,
			};

			if (OutputChannelMap is not null) {
				int temp;
				l.OutputChannelMapWeak = OutputChannelMap.ToBlock (out temp);
			}

			return l;
		}
#endif // !COREBUILD
	}

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	static partial class AudioFormatPropertyNative {
		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, AudioFormatType* inSpecifier,
			uint* outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, AudioStreamBasicDescription* inSpecifier,
			uint* outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, AudioFormatInfo* inSpecifier,
			uint* outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, int* inSpecifier,
			int* outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			int* outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, AudioFormatType* inSpecifier,
			uint* ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, int* inSpecifier,
			int* ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			int* ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			int* ioDataSize, IntPtr* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			int* ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, int* inSpecifier,
			int* ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			IntPtr ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, AudioFormatInfo* inSpecifier,
			uint* ioDataSize, AudioFormat* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, AudioStreamBasicDescription* inSpecifier,
			uint* ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr* inSpecifier,
			int* ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr* inSpecifier,
			int* ioDataSize, float* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			int* ioDataSize, float* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, AudioStreamBasicDescription* inSpecifier,
			int* ioPropertyDataSize, IntPtr* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, AudioStreamBasicDescription* inSpecifier,
			int* ioPropertyDataSize, uint* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, IntPtr inSpecifier, int* ioPropertyDataSize,
			AudioStreamBasicDescription* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, AudioFormat* inSpecifier, int* ioPropertyDataSize,
			uint* outPropertyData);
	}

	// Properties are used from various types (most suitable should be used)
	enum AudioFormatProperty : uint // UInt32 AudioFormatPropertyID
	{
		FormatInfo = 0x666d7469,    // 'fmti'
		FormatName = 0x666e616d,    // 'fnam'
		EncodeFormatIDs = 0x61636f66,   // 'acof'
		DecodeFormatIDs = 0x61636966,   // 'acif'
		FormatList = 0x666c7374,    // 'flst'
		ASBDFromESDS = 0x65737364,  // 'essd'	// TODO: FromElementaryStreamDescriptor
		ChannelLayoutFromESDS = 0x6573636c, // 'escl'	// TODO:
		OutputFormatList = 0x6f666c73,  // 'ofls'
		FirstPlayableFormatFromList = 0x6670666c,   // 'fpfl'
		FormatIsVBR = 0x66766272,   // 'fvbr'
		FormatIsExternallyFramed = 0x66657866,  // 'fexf'
		FormatEmploysDependentPackets = 0x66646570, // 'fdep'
		FormatIsEncrypted = 0x63727970, // 'cryp'
		Encoders = 0x6176656e,  // 'aven'	
		Decoders = 0x61766465,  // 'avde'
		AvailableEncodeChannelLayoutTags = 0x6165636c,  // 'aecl'
		AvailableEncodeNumberChannels = 0x61766e63, // 'avnc'
		AvailableEncodeBitRates = 0x61656272,   // 'aebr'
		AvailableEncodeSampleRates = 0x61657372,    // 'aesr'
		ASBDFromMPEGPacket = 0x61646d70,    // 'admp'	// TODO:

		BitmapForLayoutTag = 0x626d7467,    // 'bmtg'
		MatrixMixMap = 0x6d6d6170,  // 'mmap'
		ChannelMap = 0x63686d70,    // 'chmp'
		NumberOfChannelsForLayout = 0x6e63686d, // 'nchm'
		AreChannelLayoutsEquivalent = 0x63686571,   // 'cheq'	// TODO:
		ChannelLayoutHash = 0x63686861,   // 'chha'
		ValidateChannelLayout = 0x7661636c, // 'vacl'
		ChannelLayoutForTag = 0x636d706c,   // 'cmpl'
		TagForChannelLayout = 0x636d7074,   // 'cmpt'
		ChannelLayoutName = 0x6c6f6e6d, // 'lonm'
		ChannelLayoutSimpleName = 0x6c6f6e6d,   // 'lsnm'
		ChannelLayoutForBitmap = 0x636d7062,    // 'cmpb'
		ChannelName = 0x636e616d,   // 'cnam'
		ChannelShortName = 0x63736e6d,  // 'csnm'

		TagsForNumberOfChannels = 0x74616763,   // 'tagc'
		PanningMatrix = 0x70616e6d, // 'panm'
		BalanceFade = 0x62616c66,   // 'balf'

		ID3TagSize = 0x69643373,    // 'id3s' // TODO:
		ID3TagToDictionary = 0x69643364,    // 'id3d' // TODO:

#if !MONOMAC
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios8.0")]
		[ObsoletedOSPlatform ("maccatalyst13.1")]
		[ObsoletedOSPlatform ("tvos9.0")]
		HardwareCodecCapabilities = 0x68776363, // 'hwcc'
#endif
	}
}
