//
// Copyright 2011, Novell, Inc.
// Copyright 2012 Xamarin Inc.
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
using CoreGraphics;

namespace AppKit {

	public partial class NSImage {
		public CGImage CGImage {
			get {
				var rect = CGRect.Empty;
				return AsCGImage (ref rect, null, null);
			}
		}

		public static NSImage FromStream (System.IO.Stream stream)
		{
			using (NSData data = NSData.FromStream (stream)) {
				return new NSImage (data);
			}
		}

		public NSImage (string fileName, bool lazy)
		{
			if (lazy)
				Handle = InitByReferencingFile (fileName);
			else
				Handle = InitWithContentsOfFile (fileName);
		}

		public NSImage (NSData data, bool ignoresOrientation)
		{
			if (ignoresOrientation) {
				Handle = InitWithDataIgnoringOrientation (data);
			} else {
				Handle = InitWithData (data);
			}
		}

		// note: if needed override the protected Get|Set methods
		public string Name { 
			get { return GetName (); }
			// ignore return value (bool)
			set { SetName (value); }
		}

		public static NSImage ImageNamed (NSImageName name)
		{
			return ImageNamed (name.GetConstant ()); 
		}
	}

	public partial class NSImageRep {

		public CGImage CGImage {
			get {
				var rect = CGRect.Empty;
				return AsCGImage (ref rect, null, null);
			}
		}
	}
}
