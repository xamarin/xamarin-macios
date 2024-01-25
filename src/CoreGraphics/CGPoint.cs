#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;

using ObjCRuntime;

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// the remaining of the struct is defined inside src/NativeTypes/Drawing.tt
	public partial struct CGPoint {

#if NET
		public override int GetHashCode ()
		{
			return HashCode.Combine (x, y);
		}

#if MONOMAC
		// <quote>When building for 64 bit systems, or building 32 bit like 64 bit, NSPoint is typedef’d to CGPoint.</quote>
		// https://developer.apple.com/documentation/foundation/nspoint?language=objc
		[DllImport (Constants.FoundationLibrary, EntryPoint = "NSStringFromPoint")]
		extern static /* NSString* */ IntPtr NSStringFromCGPoint (/* NSPoint */ CGPoint point);
#else
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSString* */ IntPtr NSStringFromCGPoint (CGPoint point);
#endif // MONOMAC

#if !COREBUILD
		public override string? ToString ()
		{
			return CFString.FromHandle (NSStringFromCGPoint (this));
		}
#endif

#endif // !NET
	}
}
