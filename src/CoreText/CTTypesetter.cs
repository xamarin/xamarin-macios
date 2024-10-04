// 
// CTTypesetter.cs: Implements the managed CTTypesetter
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014 Xamarin Inc
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

	#region Typesetter Values

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTTypesetterOptions {

		public CTTypesetterOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTTypesetterOptions (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios6.0")]
#else
		[Deprecated (PlatformName.iOS, 6, 0)]
#endif
		public bool DisableBidiProcessing {
			get {
				return CFDictionary.GetBooleanValue (Dictionary.Handle,
						CTTypesetterOptionKey.DisableBidiProcessing.Handle);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary.Handle,
						CTTypesetterOptionKey.DisableBidiProcessing.Handle, value);
			}
		}

		// The documentation says this is an NSNumber (not exactly which type), so 'int' is as good as anything else.
		public int? ForceEmbeddingLevel {
			get { return Adapter.GetInt32Value (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel); }
			set { Adapter.SetValue (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel, value); }
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (5, 0)]
#endif
		public bool AllowUnboundedLayout {
			get => CFDictionary.GetBooleanValue (Dictionary.Handle, CTTypesetterOptionKey.AllowUnboundedLayout.Handle);
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary.Handle, CTTypesetterOptionKey.AllowUnboundedLayout.Handle, value);
			}
		}
	}

	static class CTTypesetterOptionsExtensions {
		public static IntPtr GetHandle (this CTTypesetterOptions? self)
		{
			if (self is null)
				return IntPtr.Zero;
			return self.Dictionary.GetHandle ();
		}
	}
	#endregion

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTTypesetter : NativeObject {
		[Preserve (Conditional = true)]
		internal CTTypesetter (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		#region Typesetter Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedString (IntPtr @string);
		public CTTypesetter (NSAttributedString value)
			: base (CTTypesetterCreateWithAttributedString (Runtime.ThrowOnNull (value, nameof (value)).Handle), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedStringAndOptions (IntPtr @string, IntPtr options);
		public CTTypesetter (NSAttributedString value, CTTypesetterOptions? options)
			: base (CTTypesetterCreateWithAttributedStringAndOptions (Runtime.ThrowOnNull (value, nameof (value)).Handle, options.GetHandle ()), true, true)
		{
		}
		#endregion

		#region Typeset Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLineWithOffset (IntPtr typesetter, NSRange stringRange, double offset);
		public CTLine? GetLine (NSRange stringRange, double offset)
		{
			var h = CTTypesetterCreateLineWithOffset (Handle, stringRange, offset);

			if (h == IntPtr.Zero)
				return null;

			return new CTLine (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLine (IntPtr typesetter, NSRange stringRange);
		public CTLine? GetLine (NSRange stringRange)
		{
			var h = CTTypesetterCreateLine (Handle, stringRange);

			if (h == IntPtr.Zero)
				return null;

			return new CTLine (h, true);
		}
		#endregion

		#region Typeset Line Breaking
		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestLineBreakWithOffset (IntPtr typesetter, nint startIndex, double width, double offset);
		public nint SuggestLineBreak (int startIndex, double width, double offset)
		{
			return CTTypesetterSuggestLineBreakWithOffset (Handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestLineBreak (IntPtr typesetter, nint startIndex, double width);
		public nint SuggestLineBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestLineBreak (Handle, startIndex, width);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreakWithOffset (IntPtr typesetter, nint startIndex, double width, double offset);
		public nint SuggestClusterBreak (int startIndex, double width, double offset)
		{
			return CTTypesetterSuggestClusterBreakWithOffset (Handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreak (IntPtr typesetter, nint startIndex, double width);
		public nint SuggestClusterBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestClusterBreak (Handle, startIndex, width);
		}
		#endregion
	}
}
