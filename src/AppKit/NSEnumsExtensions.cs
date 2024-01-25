//
// NSEnumsExtensions.cs:
//
// Copyright 2021, Microsoft Corp
//

using System;

using Foundation;

using ObjCRuntime;

#nullable enable

namespace AppKit {

#if !__MACCATALYST__
	public static class NSImageResizingModeExtensions {
		public static nint ToNative (NSImageResizingMode value)
		{
			// For backwards compat reasons, the values we have in managed code corresponds with the X64 values.
			if (Runtime.IsARM64CallingConvention) {
				// Stretch and Tile are switched on arm64 on macOS
				switch (value) {
				case (NSImageResizingMode) 0:
					value = (NSImageResizingMode) 1;
					break;
				case (NSImageResizingMode) 1:
					value = (NSImageResizingMode) 0;
					break;
				}
			}
			return (nint) (long) value;
		}

		public static NSImageResizingMode ToManaged (nint value)
		{
			// For backwards compat reasons, the values we have in managed code corresponds with the X64 values.
			if (Runtime.IsARM64CallingConvention) {
				// Stretch and Tile are switched on arm64 on macOS
				switch (value) {
				case 0:
					value = (nint) 1;
					break;
				case 1:
					value = (nint) 0;
					break;
				}
			}
			return (NSImageResizingMode) (long) value;
		}
	}

	public static class NSTextAlignmentExtensions {
		public static nuint ToNative (NSTextAlignment value)
		{
			// For backwards compat reasons, the values we have in managed code corresponds with the X64 values.
			if (Runtime.IsARM64CallingConvention) {
				// Center and Right are switched on arm64 on macOS
				switch (value) {
				case (NSTextAlignment) 2:
					value = (NSTextAlignment) 1;
					break;
				case (NSTextAlignment) 1:
					value = (NSTextAlignment) 2;
					break;
				}
			}
			return (nuint) (ulong) value;
		}

		public static NSTextAlignment ToManaged (nuint value)
		{
			// For backwards compat reasons, the values we have in managed code corresponds with the X64 values.
			if (Runtime.IsARM64CallingConvention) {
				// Center and Right are switched on arm64 on macOS
				switch (value) {
				case 1:
					value = (nuint) 2;
					break;
				case 2:
					value = (nuint) 1;
					break;
				}
			}
			return (NSTextAlignment) (ulong) value;
		}
	}
#endif // !__MACCATALYST__
}

