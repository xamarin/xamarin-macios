#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;

namespace CoreGraphics {

	// the remaining of the struct is defined inside src/NativeTypes/Drawing.tt
	public partial struct CGSize {

#if NET
		public override int GetHashCode ()
		{
			return HashCode.Combine (width, height);
		}

#if MONOMAC
		// <quote>When building for 64 bit systems, or building 32 bit like 64 bit, NSSize is typedef’d to CGSize.</quote>
		// https://developer.apple.com/documentation/foundation/nssize?language=objc
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSStringFromSize")]
		extern static /* NSString* */ IntPtr NSStringFromCGSize (/* NSRect */ CGSize size);
#else
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString* */ IntPtr NSStringFromCGSize (CGSize size);
#endif // MONOMAC

#if !COREBUILD
		public override string? ToString ()
		{
			return CFString.FromHandle (NSStringFromCGSize (this));
		}
#endif

#endif // !NET
	}
}
