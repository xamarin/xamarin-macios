// 
// CVImageBuffer.cs: Implements the managed CVImageBuffer
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace CoreVideo {

	// CVImageBuffer.h
	[Watch (4,0)]
	public partial class CVImageBuffer : CVBuffer {
#if !COREBUILD
		internal CVImageBuffer (IntPtr handle) : base (handle)
		{
		}

		internal CVImageBuffer () {}
		
		[Preserve (Conditional=true)]
		internal CVImageBuffer (IntPtr handle, bool owns) : base (handle, owns)
		{
		}
		
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CGRect CVImageBufferGetCleanRect (/* CVImageBufferRef __nonnull */ IntPtr imageBuffer);

		public CGRect CleanRect {
			get {
				return CVImageBufferGetCleanRect (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CGSize CVImageBufferGetDisplaySize (/* CVImageBufferRef __nonnull */ IntPtr imageBuffer);

		public CGSize DisplaySize {
			get {
				return CVImageBufferGetDisplaySize (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CGSize CVImageBufferGetEncodedSize (/* CVImageBufferRef __nonnull */ IntPtr imageBuffer);

		public CGSize EncodedSize {
			get {
				return CVImageBufferGetDisplaySize (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* Boolean */ bool CVImageBufferIsFlipped (/* CVImageBufferRef __nonnull */ IntPtr imageBuffer);
		
		public bool IsFlipped {
			get {
				return CVImageBufferIsFlipped (handle);
			}
		}

		// it was mentioned in iOS4 diff (without an architecture) but it's never been seen elsewhere for iOS
#if MONOMAC
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CVImageBufferGetColorSpace (/* CVImageBufferRef */ IntPtr imageBuffer);
		
		[Deprecated (PlatformName.MacOSX, 10, 4)]
		[Unavailable (PlatformName.iOS)]
		public CGColorSpace ColorSpace {
			get {
				var h = CVImageBufferGetColorSpace (handle);
				return h == IntPtr.Zero ? null : new CGColorSpace (h);
			}
		}
#elif !XAMCORE_3_0
		[Deprecated (PlatformName.MacOSX, 10, 4)]
		[Unavailable (PlatformName.iOS)]
		public CGColorSpace ColorSpace {
			get {
				return null;
			}
		}
#endif

#if MONOMAC 
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CVImageBufferCreateColorSpaceFromAttachments (/* CFDictionaryRef */ IntPtr attachments);

		public static CGColorSpace CreateFrom (NSDictionary attachments)
		{
			if (attachments == null)
				throw new ArgumentNullException ("attachments");
			var h = CVImageBufferCreateColorSpaceFromAttachments (attachments.Handle);
			return h == IntPtr.Zero ? null : new CGColorSpace (h);
		}
#endif

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static int CVYCbCrMatrixGetIntegerCodePointForString (IntPtr yCbCrMatrixString);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static int GetCodePoint (CVImageBufferYCbCrMatrix yCbCrMatrix)
		{
			return CVYCbCrMatrixGetIntegerCodePointForString (yCbCrMatrix.GetConstant ().Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static int CVColorPrimariesGetIntegerCodePointForString (IntPtr colorPrimariesString);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static int GetCodePoint (CVImageBufferColorPrimaries color)
		{
			return CVColorPrimariesGetIntegerCodePointForString (color.GetConstant ().Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static int CVTransferFunctionGetIntegerCodePointForString (IntPtr colorPrimariesString);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static int GetCodePoint (CVImageBufferTransferFunction function)
		{
			return CVTransferFunctionGetIntegerCodePointForString (function.GetConstant ().Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static IntPtr CVYCbCrMatrixGetStringForIntegerCodePoint (int yCbCrMatrixCodePoint);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static CVImageBufferYCbCrMatrix GetYCbCrMatrixOption (int yCbCrMatrixCodePoint)
		{
			var ret = Runtime.GetNSObject<NSString> (CVYCbCrMatrixGetStringForIntegerCodePoint (yCbCrMatrixCodePoint));
			return CVImageBufferYCbCrMatrixExtensions.GetValue (ret);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static IntPtr CVColorPrimariesGetStringForIntegerCodePoint (int colorPrimariesCodePoint);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static CVImageBufferColorPrimaries GetColorPrimariesOption (int colorPrimariesCodePoint)
		{
			var ret = Runtime.GetNSObject<NSString> (CVColorPrimariesGetStringForIntegerCodePoint (colorPrimariesCodePoint));
			return CVImageBufferColorPrimariesExtensions.GetValue (ret);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		extern static IntPtr CVTransferFunctionGetStringForIntegerCodePoint (int transferFunctionCodePoint);

		[iOS (11, 0), Mac (10, 13), TV (11, 0), Watch (4, 0)]
		public static CVImageBufferTransferFunction GetTransferFunctionOption (int transferFunctionCodePoint)
		{
			var ret = Runtime.GetNSObject<NSString> (CVTransferFunctionGetStringForIntegerCodePoint (transferFunctionCodePoint));
			return CVImageBufferTransferFunctionExtensions.GetValue (ret);
		}

#endif // !COREBUILD
	}
}
