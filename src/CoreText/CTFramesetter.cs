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

#nullable enable

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFramesetter : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFramesetter (NativeHandle handle, bool owns)
			: base (handle, owns, true)
		{
		}

		#region Framesetter Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterCreateWithAttributedString (IntPtr @string);
		public CTFramesetter (NSAttributedString value)
			: base (CTFramesetterCreateWithAttributedString (Runtime.ThrowOnNull (value, nameof (value)).Handle), true, true)
		{
		}
		#endregion

		#region Frame Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterCreateFrame (IntPtr framesetter, NSRange stringRange, IntPtr path, IntPtr frameAttributes);
		public CTFrame? GetFrame (NSRange stringRange, CGPath path, CTFrameAttributes? frameAttributes)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));
			var frame = CTFramesetterCreateFrame (Handle, stringRange, path.Handle, frameAttributes.GetHandle ());
			if (frame == IntPtr.Zero)
				return null;
			return new CTFrame (frame, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterGetTypesetter (IntPtr framesetter);
		public CTTypesetter? GetTypesetter ()
		{
			var h = CTFramesetterGetTypesetter (Handle);

			if (h == IntPtr.Zero)
				return null;
			return new CTTypesetter (h, false);
		}
		#endregion

		#region Frame Sizing
		[DllImport (Constants.CoreTextLibrary)]
		static extern CGSize CTFramesetterSuggestFrameSizeWithConstraints (
				IntPtr framesetter, NSRange stringRange, IntPtr frameAttributes, CGSize constraints, out NSRange fitRange);
		public CGSize SuggestFrameSize (NSRange stringRange, CTFrameAttributes? frameAttributes, CGSize constraints, out NSRange fitRange)
		{
			return CTFramesetterSuggestFrameSizeWithConstraints (
					Handle, stringRange,
					frameAttributes.GetHandle (),
					constraints, out fitRange);
		}
		#endregion
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFramesetterCreateWithTypesetter (IntPtr typesetter);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios12.0")]
		[SupportedOSPlatform ("tvos12.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (12, 0)]
		[TV (12, 0)]
		[Watch (5, 0)]
#endif
		public static CTFramesetter? Create (CTTypesetter typesetter)
		{
			if (typesetter is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (typesetter));

			var ret = CTFramesetterCreateWithTypesetter (typesetter.Handle);
			if (ret == IntPtr.Zero)
				return null;

			return new CTFramesetter (ret, owns: true);
		}
	}
}
