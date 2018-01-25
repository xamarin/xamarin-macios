//
// Copyright 2010, Novell, Inc.
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
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace QTKit {

	// Untyped anonymous enum in ObjC
	[Flags]
	public enum TimeFlags : int {
		TimeIsIndefinite = 1
	}

	// Untyped anonymous enum in ObjC
	[Flags]
	public enum  QTMovieFileTypeOptions : uint {
		StillImageTypes = 1 << 0,
		TranslatableTypes = 1 << 1,
		AggressiveTypes = 1 << 2,
		DynamicTypes = 1 << 3,
		CommonTypes = 0,
		AllTypes = 0xffff
	}

	[Native]
	public enum QTCaptureDevicePlaybackMode : ulong {
		NotPlaying,
		Playing
	}

	[Native]
	public enum QTCaptureDeviceControlsSpeed : long {
		FastestReverse = -19000,	
		VeryFastReverse = -16000,
		FastReverse = -13000,
		NormalReverse = -10000,
		SlowReverse = -7000,
		VerySlowReverse = -4000,
		SlowestReverse = -1000,
		Stopped = 0,	
		SlowestForward = 1000,
		VerySlowForward = 4000,
		SlowForward = 7000,
		NormalForward = 10000,
		FastForward = 13000,
		VeryFastForward = 16000,
		FastestForward = 19000,		
	}

	[Native]
	public enum QTCaptureDestination : ulong {
		NewFile = 1,
		OldFile = 2
	}

	[Native]
	public enum QTError : long {
		Unknown = -1,
		None,
		IncompatibleInput = 1002,
		IncompatibleOutput = 1003,
		InvalidInputsOrOutputs = 1100,
		DeviceAlreadyUsedbyAnotherSession = 1101,
		NoDataCaptured = 1200,
		SessionConfigurationChanged = 1201,
		DiskFull = 1202,
		DeviceWasDisconnected = 1203,
		MediaChanged = 1204,
		MaximumDurationReached = 1205,
		MaximumFileSizeReached = 1206,
		MediaDiscontinuity = 1207,
		MaximumNumberOfSamplesForFileFormatReached = 1208,
		DeviceNotConnected = 1300,
		DeviceInUseByAnotherApplication = 1301,
		DeviceExcludedByAnotherDevice = 1302,
	}

	// Convenience enum for strings in ObjC
	public enum QTMediaType : int {
		Video, Sound, Text, Base, Mpeg, Music, TimeCode, Sprite, Flash, Movie, Tween, Type3D, Skin, Qtvr, Hint, Stream, Muxed, QuartzComposer
	}

	public enum QTFileType : int {
		AIFF = 0x41494646, 		// 'AIFF'
		AIFC = 0x41494643, 		// 'AIFC'
		DVC = 0x64766321, 		// 'dvc!'
		MIDI = 0x4d696469, 		// 'Midi'
		Picture = 0x50494354, 		// 'PICT'
		Movie = 0x4d6f6f56, 		// 'MooV'
		Text = 0x54455854, 		// 'TEXT'
		Wave = 0x57415645, 		// 'WAVE'
		SystemSevenSound = 0x7366696c, 	// 'sfil'
		MuLaw = 0x554c4157, 		// 'ULAW'
		AVI = 0x56665720, 		// 'VfW '
		SoundDesignerII = 0x53643266, 	// 'Sd2f'
		AudioCDTrack = 0x7472616b, 	// 'trak'
		PICS = 0x50494353, 		// 'PICS'
		GIF = 0x47494666, 		// 'GIFf'
		PNG = 0x504e4766, 		// 'PNGf'
		TIFF = 0x54494646, 		// 'TIFF'
		PhotoShop = 0x38425053, 	// '8BPS'
		SGIImage = 0x2e534749, 		// '.SGI'
		BMP = 0x424d5066, 		// 'BMPf'
		JPEG = 0x4a504547, 		// 'JPEG'
		JFIF = 0x4a504547, 		// 'JPEG'
		MacPaint = 0x504e5447, 		// 'PNTG'
		TargaImage = 0x54504943, 	// 'TPIC'
		QuickDrawGXPicture = 0x71646778,// 'qdgx'
		QuickTimeImage = 0x71746966, 	// 'qtif'
		T3DMF = 0x33444d46, 		// '3DMF'
		FLC = 0x464c4320, 		// 'FLC '
		Flash = 0x5357464c, 		// 'SWFL'
		FlashPix = 0x46506978, 		// 'FPix'
		MP4 = 0x6d706734, 		// 'mpg4'
		PDF = 0x50444620, 		// 'PDF '
		T3GPP = 0x33677070, 		// '3gpp'
		AMR = 0x616d7220, 		// 'amr '
		SDV = 0x73647620, 		// 'sdv '
		T3GP2 = 0x33677032, 		// '3gp2'
		AMC = 0x616d6320, 		// 'amc '
		JPEG2000 = 0x6a703220, 		// 'jp2 
	}
} 
