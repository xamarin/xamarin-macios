//
// Enums.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2016, Xamarin Inc.
// Copyright 2020, Microsoft Corp.
//

using System;

using ObjCRuntime;

namespace ImageIO {

	// untyped enum -> CGImageMetadata.h
	// note: not used in any API
	[ErrorDomain ("kCFErrorDomainCGImageMetadata")]
	public enum CGImageMetadataErrors {
		Unknown = 0,
		UnsupportedFormat = 1,
		BadArgument = 2,
		ConflictingArguments = 3,
		PrefixConflict = 4,
	}

	// untyped enum -> CGImageMetadata.h
	public enum CGImageMetadataType {
		Invalid = -1,
		Default = 0,
		String = 1,
		ArrayUnordered = 2,
		ArrayOrdered = 3,
		AlternateArray = 4,
		AlternateText = 5,
		Structure = 6
	}

	public enum CGImagePropertyOrientation {
		Up = 1,
		UpMirrored,
		Down,
		DownMirrored,
		LeftMirrored,
		Right,
		RightMirrored,
		Left
	}

	// untyped enum / #defines
	// used with kCGImagePropertyPNGCompressionFilter
	[MacCatalyst (13, 1)]
	[Flags]
	public enum CGImagePropertyPngFilters {
		No = 0,
		None = 0x08,
		Sub = 0x10,
		Up = 0x20,
		Average = 0x40,
		Paeth = 0x80
	}

	[iOS (13, 0), TV (13, 0), Watch (6, 0)]
	[MacCatalyst (13, 1)]
	public enum CGImageAnimationStatus {
		Ok = 0,
		ParameterError = -22140,
		CorruptInputImage = -22141,
		UnsupportedFormat = -22142,
		IncompleteInputImage = -22143,
		AllocationFailure = -22144,
	}

	// Yes, no [Native] here
	[Mac (11, 0), iOS (14, 1), TV (14, 2), Watch (7, 1)]
	[MacCatalyst (14, 1)]
	public enum CGImagePropertyTgaCompression : uint {
		None = 0,
		Rle,
	}
}
