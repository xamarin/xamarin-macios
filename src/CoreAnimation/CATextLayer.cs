// 
// CATextLayer: Support for CATextLayer
//
// Authors:
//   Miguel de Icaza.
//     
// Copyright 2010 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
//
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

using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreFoundation;
using CoreText;
#if MONOMAC
using AppKit;
#endif

#nullable enable

namespace CoreAnimation {

	public partial class CATextLayer {

		public NSAttributedString? AttributedString {
			get {
				return Runtime.GetNSObject (_AttributedString) as NSAttributedString;
			}
			set {
				_AttributedString = value.GetHandle ();
			}
		}

		public void SetFont (string fontName)
		{
			if (fontName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (fontName));
			using (var nss = new NSString (fontName))
				_Font = nss.Handle;
		}

		public void SetFont (CGFont font)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			_Font = font.Handle;
		}

		public void SetFont (CTFont font)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			_Font = font.Handle;
		}

#if MONOMAC
		public void SetFont (NSFont font)
		{
			if (font is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (font));
			_Font = font.Handle;
		}
#endif

		public object? WeakFont {
			get {
				var handle = _Font;
				nint type = CFType.GetTypeID (handle);
				if (type == CTFont.GetTypeID ())
					return new CTFont (handle, false);
				else if (type == CGFont.GetTypeID ())
					return new CGFont (handle, false);
				else if (type == CFString.GetTypeID ())
					return CFString.FromHandle (handle);
#if MONOMAC
				else return Runtime.GetNSObject<NSFont> (handle);
#else
				return null;
#endif
			}

			// Allows CTFont, CGFont, string and in OSX NSFont settings
			set {
#if MONOMAC
				var ns = value as NSFont;
				if (ns is not null){
					_Font = ns.Handle;
					return;
				}
#endif
				var ct = value as CTFont;
				if (ct is not null) {
					_Font = ct.Handle;
					return;
				}
				var cg = value as CGFont;
				if (cg is not null) {
					_Font = cg.Handle;
					return;
				}
				var nss = value as NSString;
				if (nss is not null) {
					_Font = nss.Handle;
					return;
				}
				var str = value as string;
				if (str is not null) {
					nss = new NSString (str);
					_Font = nss.Handle;
				}
			}
		}
#if !NET
		[Obsolete ("Use 'TextTruncationMode' instead.")]
		public virtual string TruncationMode {
			get { return (string) WeakTruncationMode; }
			set { WeakTruncationMode = (NSString) value; }
		}

		[Obsolete ("Use 'TextAlignmentMode' instead.")]
		public virtual string AlignmentMode {
			get { return (string) WeakAlignmentMode; }
			set { WeakAlignmentMode = (NSString) value; }
		}
#endif // !NET
		public CATextLayerTruncationMode TextTruncationMode {
			get { return CATextLayerTruncationModeExtensions.GetValue (WeakTruncationMode); }
			set { WeakTruncationMode = value.GetConstant ()!; }
		}

		public CATextLayerAlignmentMode TextAlignmentMode {
			get { return CATextLayerAlignmentModeExtensions.GetValue (WeakAlignmentMode); }
			set { WeakAlignmentMode = value.GetConstant ()!; }
		}
	}
}
