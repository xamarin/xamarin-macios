// 
// CTFrame.cs: Implements the managed CTFrame
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
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
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace CoreText {

	[Flags]
	public enum CTFrameProgression : uint {
		TopToBottom = 0,
		RightToLeft = 1
	}

	public enum CTFramePathFillRule {
		EvenOdd,
		WindingNumber
	}

	public static class CTFrameAttributeKey {

		public static readonly NSString Progression;

		public static readonly NSString PathFillRule;
		public static readonly NSString PathWidth;
		public static readonly NSString ClippingPaths;
		public static readonly NSString PathClippingPath;
		
		static CTFrameAttributeKey ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				Progression = Dlfcn.GetStringConstant (handle, "kCTFrameProgressionAttributeName");
				PathFillRule = Dlfcn.GetStringConstant (handle, "kCTFramePathFillRuleAttributeName");
				PathWidth = Dlfcn.GetStringConstant (handle, "kCTFramePathWidthAttributeName");
				ClippingPaths = Dlfcn.GetStringConstant (handle, "kCTFrameClippingPathsAttributeName");
				PathClippingPath = Dlfcn.GetStringConstant (handle, "kCTFramePathClippingPathAttributeName");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}

	public class CTFrameAttributes {

		public CTFrameAttributes ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFrameAttributes (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		public CTFrameProgression? Progression {
			get {
				var value = Adapter.GetUInt32Value (Dictionary, CTFrameAttributeKey.Progression);
				return !value.HasValue ? null : (CTFrameProgression?) value.Value;
			}
			set {
				Adapter.SetValue (Dictionary, CTFrameAttributeKey.Progression,
						value.HasValue ? (uint?) value.Value : null);
			}
		}
	}

	public class CTFrame : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTFrame (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get { return handle; }
		}

		~CTFrame ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static NSRange CTFrameGetStringRange (IntPtr handle);
		[DllImport (Constants.CoreTextLibrary)]
		extern static NSRange CTFrameGetVisibleStringRange (IntPtr handle);
		
		public NSRange GetStringRange ()
		{
			return CTFrameGetStringRange (handle);
		}

		public NSRange GetVisibleStringRange ()
		{
			return CTFrameGetVisibleStringRange (handle);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static IntPtr CTFrameGetPath (IntPtr handle);
		
		public CGPath GetPath ()
		{
			IntPtr h = CTFrameGetPath (handle);
			return h == IntPtr.Zero ? null : new CGPath (h, false);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static IntPtr CTFrameGetFrameAttributes (IntPtr handle);

		public CTFrameAttributes GetFrameAttributes ()
		{
			var attrs = (NSDictionary) Runtime.GetNSObject (CTFrameGetFrameAttributes (handle));
			return attrs == null ? null : new CTFrameAttributes (attrs);
		}
		
		[DllImport (Constants.CoreTextLibrary)]
		extern static IntPtr CTFrameGetLines (IntPtr handle);

		public CTLine [] GetLines ()
		{
			var cfArrayRef = CTFrameGetLines (handle);
			if (cfArrayRef == IntPtr.Zero)
				return new CTLine [0];

			return NSArray.ArrayFromHandleFunc<CTLine> (cfArrayRef, (p) => {
					// We need to take a ref, since we dispose it later.
					return new CTLine (p, false);
				});
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTFrameGetLineOrigins(IntPtr handle, NSRange range, [Out] CGPoint[] origins);
		public void GetLineOrigins (NSRange range, CGPoint[] origins)
		{
			if (origins == null)
				throw new ArgumentNullException ("origins");
			if (range.Length != 0 && origins.Length < range.Length)
				throw new ArgumentException ("origins must contain at least range.Length elements.", "origins");
			else if (origins.Length < CFArray.GetCount (CTFrameGetLines (handle)))
				throw new ArgumentException ("origins must contain at least GetLines().Length elements.", "origins");
			CTFrameGetLineOrigins (handle, range, origins);
		}

		[DllImport (Constants.CoreTextLibrary)]
		extern static void CTFrameDraw (IntPtr handle, IntPtr context);

		public void Draw (CGContext ctx)
		{
			if (ctx == null)
				throw new ArgumentNullException ("ctx");

			CTFrameDraw (handle, ctx.Handle);
		}
	}
}

