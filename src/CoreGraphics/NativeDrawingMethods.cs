using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ObjCRuntime;

#nullable enable

namespace CoreGraphics {
	static class NativeDrawingMethods {
#if MONOMAC
		internal const string CG = Constants.ApplicationServicesCoreGraphicsLibrary;
#else
		internal const string CG = Constants.CoreGraphicsLibrary;
#endif
		[DllImport (CG)]
		internal unsafe extern static byte CGRectMakeWithDictionaryRepresentation (IntPtr dict, CGRect* rect);
		[DllImport (CG)]
		internal unsafe extern static byte CGPointMakeWithDictionaryRepresentation (IntPtr dict, CGPoint* point);
		[DllImport (CG)]
		internal unsafe extern static byte CGSizeMakeWithDictionaryRepresentation (IntPtr dict, CGSize* point);

		[DllImport (CG)]
		internal extern static IntPtr CGRectCreateDictionaryRepresentation (CGRect rect);
		[DllImport (CG)]
		internal extern static IntPtr CGSizeCreateDictionaryRepresentation (CGSize size);
		[DllImport (CG)]
		internal extern static IntPtr CGPointCreateDictionaryRepresentation (CGPoint point);
	}
}
