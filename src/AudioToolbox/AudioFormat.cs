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
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

using OSStatus = System.Int32;
using AudioFileID = System.IntPtr;

namespace AudioToolbox {

	// AudioFormatListItem
	[StructLayout(LayoutKind.Sequential)]
	public struct AudioFormat
	{
		public AudioStreamBasicDescription AudioStreamBasicDescription;
		public AudioChannelLayoutTag AudioChannelLayoutTag;

		public unsafe static AudioFormat? GetFirstPlayableFormat (AudioFormat[] formatList)
		{
			if (formatList == null)
				throw new ArgumentNullException ("formatList");
			if (formatList.Length < 2)
				throw new ArgumentException ("formatList");

 			fixed (AudioFormat* item = &formatList[0]) {
				uint index;
				int size = sizeof (uint);
				var ptr_size = sizeof (AudioFormat) * formatList.Length;
				if (AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.FirstPlayableFormatFromList, ptr_size, item, ref size, out index) != 0)
					return null;

				return formatList [index];
			}
		}

		public override string ToString ()
		{
			return AudioChannelLayoutTag + ":" + AudioStreamBasicDescription.ToString ();
		}
	}

	public enum AudioFormatError : int // Implictly cast to OSType
	{
		None					= 0,
		Unspecified				= 0x77686174,	// 'what'
		UnsupportedProperty 	= 0x70726f70,	// 'prop'
		BadPropertySize			= 0x2173697a,	// '!siz'
		BadSpecifierSize		= 0x21737063,	// '!spc'
		UnsupportedDataFormat	= 0x666d743f,	// 'fmt?'
		UnknownFormat			= 0x21666d74	// '!fmt'

        // TODO: Not documented
        // '!dat'
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct AudioValueRange
	{
    	public double Minimum;
    	public double Maximum;
	}

	public enum AudioBalanceFadeType : uint // UInt32 in AudioBalanceFades
	{
		MaxUnityGain = 0,
		EqualPower = 1
	}

	public class AudioBalanceFade
	{
#if !COREBUILD
		[StructLayout (LayoutKind.Sequential)]
		struct Layout
		{
			public float LeftRightBalance;
			public float BackFrontFade;
			public AudioBalanceFadeType Type;
			public IntPtr ChannelLayoutWeak;
		}

		public AudioBalanceFade (AudioChannelLayout channelLayout)
		{
			if (channelLayout == null)
				throw new ArgumentNullException ("channelLayout");

			this.ChannelLayout = channelLayout;
		}

		public float LeftRightBalance { get; set; }
		public float BackFrontFade { get; set; }
		public AudioBalanceFadeType Type { get; set; }
		public AudioChannelLayout ChannelLayout { get; private set; }

		public unsafe float[] GetBalanceFade ()
		{
			var type_size = sizeof (Layout);

			var str = ToStruct ();
			var ptr = Marshal.AllocHGlobal (type_size);
			(*(Layout *) ptr) = str;

			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.BalanceFade, type_size, ptr, out size) != 0)
				return null;

			AudioFormatError res;
			var data = new float[size / sizeof (float)];
			fixed (float* data_ptr = data) {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.BalanceFade, type_size, ptr, ref size, data_ptr);
			}

			Marshal.FreeHGlobal (str.ChannelLayoutWeak);
			Marshal.FreeHGlobal (ptr);

			return res == 0 ? data : null;
		}

		Layout ToStruct ()
		{
			var l = new Layout ()
			{
				LeftRightBalance = LeftRightBalance,
				BackFrontFade = BackFrontFade,
				Type = Type,
			};

			if (ChannelLayout != null) {
				int temp;
				l.ChannelLayoutWeak = ChannelLayout.ToBlock (out temp);
			}
			
			return l;
		}	
#endif // !COREBUILD
	}

	public enum PanningMode : uint // UInt32 in AudioPanningInfo
	{
		SoundField					= 3,
		VectorBasedPanning			= 4
	}

	public class AudioPanningInfo
	{
#if !COREBUILD
		[StructLayout (LayoutKind.Sequential)]
		struct Layout
		{
			public PanningMode PanningMode;
			public AudioChannelFlags CoordinateFlags;
			public float Coord0, Coord1, Coord2;
			public float GainScale;
			public IntPtr OutputChannelMapWeak;
		}

		public AudioPanningInfo (AudioChannelLayout outputChannelMap)
		{
			if (outputChannelMap == null)
				throw new ArgumentNullException ("outputChannelMap");

			this.OutputChannelMap = outputChannelMap;
		}

		public PanningMode PanningMode { get; set; }
		public AudioChannelFlags CoordinateFlags { get; set; }
		public float[] Coordinates { get; private set; }
		public float GainScale { get; set; }
		public AudioChannelLayout OutputChannelMap { get; private set; }

		public unsafe float[] GetPanningMatrix ()
		{
			var type_size = sizeof (Layout);

			var str = ToStruct ();
			var ptr = Marshal.AllocHGlobal (type_size);
			*((Layout *)ptr) = str;

			int size;
			if (AudioFormatPropertyNative.AudioFormatGetPropertyInfo (AudioFormatProperty.PanningMatrix, type_size, ptr, out size) != 0)
				return null;

			AudioFormatError res;
			var data = new float[size / sizeof (float)];
			fixed (float* data_ptr = data) {
				res = AudioFormatPropertyNative.AudioFormatGetProperty (AudioFormatProperty.PanningMatrix, type_size, ptr, ref size, data_ptr);
			}

			Marshal.FreeHGlobal (str.OutputChannelMapWeak);
			Marshal.FreeHGlobal (ptr);

			return res == 0 ? data : null;
		}

		Layout ToStruct ()
		{
			var l = new Layout ()
			{
				PanningMode = PanningMode,
				CoordinateFlags = CoordinateFlags,
				Coord0 = Coordinates [0],
				Coord1 = Coordinates [1],
				Coord2 = Coordinates [2],
				GainScale = GainScale
			};

			if (OutputChannelMap != null) {
				int temp;
				l.OutputChannelMapWeak = OutputChannelMap.ToBlock (out temp);
			}
			
			return l;
		}	
#endif // !COREBUILD
	}

	static partial class AudioFormatPropertyNative
	{
		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioFormatType inSpecifier,
			out uint outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioStreamBasicDescription inSpecifier,
			out uint outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioFormatInfo inSpecifier,
			out uint outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, ref int inSpecifier,
			out int outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetPropertyInfo (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			out int outPropertyDataSize);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioFormatType inSpecifier,
			ref uint ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref int inSpecifier,
			ref int ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			ref int ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			ref int ioDataSize, out IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			ref int ioDataSize, out int outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref int inSpecifier,
			ref int ioDataSize, out int outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			IntPtr ioDataSize, IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioFormatInfo inSpecifier,
			ref uint ioDataSize, AudioFormat* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref AudioStreamBasicDescription inSpecifier,
			ref uint ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, ref int inSpecifier,
			ref int ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr* inSpecifier,
			ref int ioDataSize, int* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr* inSpecifier,
			ref int ioDataSize, float* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty propertyID, int inSpecifierSize, IntPtr inSpecifier,
			ref int ioDataSize, float* outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, ref AudioStreamBasicDescription inSpecifier,
			ref int ioPropertyDataSize, out IntPtr outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, ref AudioStreamBasicDescription inSpecifier,
			ref int ioPropertyDataSize, out uint outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public  extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, IntPtr inSpecifier, ref int ioPropertyDataSize,
			ref AudioStreamBasicDescription outPropertyData);

		[DllImport (Constants.AudioToolboxLibrary)]
		public unsafe extern static AudioFormatError AudioFormatGetProperty (AudioFormatProperty inPropertyID, int inSpecifierSize, AudioFormat* inSpecifier, ref int ioPropertyDataSize,
			out uint outPropertyData);
	}

	// Properties are used from various types (most suitable should be used)
	enum AudioFormatProperty : uint // UInt32 AudioFormatPropertyID
	{
		FormatInfo					= 0x666d7469,	// 'fmti'
		FormatName					= 0x666e616d,	// 'fnam'
		EncodeFormatIDs				= 0x61636f66,	// 'acof'
		DecodeFormatIDs				= 0x61636966,	// 'acif'
		FormatList					= 0x666c7374,	// 'flst'
		ASBDFromESDS 				= 0x65737364,	// 'essd'	// TODO: FromElementaryStreamDescriptor
		ChannelLayoutFromESDS 		= 0x6573636c,	// 'escl'	// TODO:
		OutputFormatList			= 0x6f666c73,	// 'ofls'
		FirstPlayableFormatFromList	= 0x6670666c,	// 'fpfl'
		FormatIsVBR 				= 0x66766272,	// 'fvbr'
		FormatIsExternallyFramed	= 0x66657866,	// 'fexf'
		FormatIsEncrypted			= 0x63727970,	// 'cryp'
		Encoders					= 0x6176656e,	// 'aven'	
		Decoders					= 0x61766465,	// 'avde'
		AvailableEncodeChannelLayoutTags	= 0x6165636c,	// 'aecl'
		AvailableEncodeNumberChannels		= 0x61766e63,	// 'avnc'
		AvailableEncodeBitRates		= 0x61656272,	// 'aebr'
		AvailableEncodeSampleRates	= 0x61657372,	// 'aesr'
		ASBDFromMPEGPacket			= 0x61646d70,	// 'admp'	// TODO:

		BitmapForLayoutTag			= 0x626d7467,	// 'bmtg'
		MatrixMixMap				= 0x6d6d6170,	// 'mmap'
		ChannelMap					= 0x63686d70,	// 'chmp'
		NumberOfChannelsForLayout	= 0x6e63686d,	// 'nchm'
		AreChannelLayoutsEquivalent	= 0x63686571,	// 'cheq'	// TODO:
		ChannelLayoutHash           = 0x63686861,   // 'chha'
		ValidateChannelLayout		= 0x7661636c,	// 'vacl'
		ChannelLayoutForTag			= 0x636d706c,	// 'cmpl'
		TagForChannelLayout			= 0x636d7074,	// 'cmpt'
		ChannelLayoutName			= 0x6c6f6e6d,	// 'lonm'
		ChannelLayoutSimpleName		= 0x6c6f6e6d,	// 'lsnm'
		ChannelLayoutForBitmap		= 0x636d7062,	// 'cmpb'
		ChannelName					= 0x636e616d,	// 'cnam'
		ChannelShortName			= 0x63736e6d,	// 'csnm'

		TagsForNumberOfChannels		= 0x74616763,	// 'tagc'
		PanningMatrix				= 0x70616e6d,	// 'panm'
		BalanceFade					= 0x62616c66,	// 'balf'

		ID3TagSize					= 0x69643373,	// 'id3s' // TODO:
		ID3TagToDictionary			= 0x69643364,	// 'id3d' // TODO:

#if !MONOMAC
		[Deprecated (PlatformName.iOS, 8, 0)]
		HardwareCodecCapabilities	= 0x68776363,	// 'hwcc'
#endif
	}
}
