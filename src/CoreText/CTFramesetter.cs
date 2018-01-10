// 
// CTFramesetter.cs: Implements the managed CTFramesetter
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
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

	public class CTFramesetter : INativeObject, IDisposable {

		internal IntPtr handle;

		internal CTFramesetter (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}
		
		public IntPtr Handle {
			get {return handle;}
		}

		~CTFramesetter ()
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

#region Framesetter Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterCreateWithAttributedString (IntPtr @string);
		public CTFramesetter (NSAttributedString value)
		{
			if (value == null)
				throw ConstructorError.ArgumentNull (this, "value");
			handle = CTFramesetterCreateWithAttributedString (value.Handle);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}
#endregion

#region Frame Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterCreateFrame (IntPtr framesetter, NSRange stringRange, IntPtr path, IntPtr frameAttributes);
		public CTFrame GetFrame (NSRange stringRange, CGPath path, CTFrameAttributes frameAttributes)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			var frame = CTFramesetterCreateFrame (handle, stringRange, path.Handle,
					frameAttributes == null ? IntPtr.Zero : frameAttributes.Dictionary.Handle);
			if (frame == IntPtr.Zero)
				return null;
			return new CTFrame (frame, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterGetTypesetter (IntPtr framesetter);
		public CTTypesetter GetTypesetter ()
		{
			var h = CTFramesetterGetTypesetter (handle);

			if (h == IntPtr.Zero)
				return null;
			return new CTTypesetter (h, false);
		}
#endregion

#region Frame Sizing
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGSize CTFramesetterSuggestFrameSizeWithConstraints (
				IntPtr framesetter, NSRange stringRange, IntPtr frameAttributes, CGSize constraints, out NSRange fitRange);
		public CGSize SuggestFrameSize (NSRange stringRange, CTFrameAttributes frameAttributes, CGSize constraints, out NSRange fitRange)
		{
			return CTFramesetterSuggestFrameSizeWithConstraints (
					handle, stringRange,
					frameAttributes == null ? IntPtr.Zero : frameAttributes.Dictionary.Handle,
					constraints, out fitRange);
		}
#endregion
	}
}

