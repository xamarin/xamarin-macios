// 
// CGLayer.cs: Implements the managed CGLayer
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
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

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGLayer.h
	public class CGLayer : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CGLayer (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGLayerRelease (/* CGLayerRef */ IntPtr layer);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGLayerRef */ IntPtr CGLayerRetain (/* CGLayerRef */ IntPtr layer);

		protected internal override void Retain ()
		{
			CGLayerRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGLayerRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGSize CGLayerGetSize (/* CGLayerRef */ IntPtr layer);

		public CGSize Size {
			get {
				return CGLayerGetSize (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGContextRef */ IntPtr CGLayerGetContext (/* CGLayerRef */ IntPtr layer);

		public CGContext Context {
			get {
				return new CGContext (CGLayerGetContext (Handle), false);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGLayerRef */ IntPtr CGLayerCreateWithContext (/* CGContextRef */ IntPtr context, CGSize size, /* CFDictionaryRef */ IntPtr auxiliaryInfo);

		public static CGLayer Create (CGContext? context, CGSize size)
		{
			// note: auxiliaryInfo is reserved and should be null
			return new CGLayer (CGLayerCreateWithContext (context.GetHandle (), size, IntPtr.Zero), true);
		}
#endif
	}
}
