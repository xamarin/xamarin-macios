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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace CoreText {

#region Typesetter Values
	public static class CTTypesetterOptionKey {

		[Deprecated (PlatformName.iOS, 6, 0)]
		public static readonly NSString DisableBidiProcessing;
		public static readonly NSString ForceEmbeddingLevel;

		static CTTypesetterOptionKey ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
#pragma warning disable 618				
				DisableBidiProcessing = Dlfcn.GetStringConstant (handle, "kCTTypesetterOptionDisableBidiProcessing");
#pragma warning restore 618
				ForceEmbeddingLevel   = Dlfcn.GetStringConstant (handle, "kCTTypesetterOptionForcedEmbeddingLevel");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}

	public class CTTypesetterOptions {

		public CTTypesetterOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTTypesetterOptions (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		[Deprecated (PlatformName.iOS, 6, 0)]
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
			get {return Adapter.GetInt32Value (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel);}
			set {Adapter.SetValue (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel, value);}
		}
	}
#endregion

	public class CTTypesetter : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTTypesetter (IntPtr handle, bool owns)
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

		~CTTypesetter ()
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

#region Typesetter Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedString (IntPtr @string);
		public CTTypesetter (NSAttributedString value)
		{
			if (value == null)
				throw ConstructorError.ArgumentNull (this, "value");

			handle = CTTypesetterCreateWithAttributedString (value.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedStringAndOptions (IntPtr @string, IntPtr options);
		public CTTypesetter (NSAttributedString value, CTTypesetterOptions options)
		{
			if (value == null)
				throw ConstructorError.ArgumentNull (this, "value");

			handle = CTTypesetterCreateWithAttributedStringAndOptions (value.Handle,
					options == null ? IntPtr.Zero : options.Dictionary.Handle);

			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}
#endregion

#region Typeset Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLineWithOffset (IntPtr typesetter, NSRange stringRange, double offset);
		public CTLine GetLine (NSRange stringRange, double offset)
		{
			var h = CTTypesetterCreateLineWithOffset (handle, stringRange, offset);

			if (h == IntPtr.Zero)
				return null;

			return new CTLine (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLine (IntPtr typesetter, NSRange stringRange);
		public CTLine GetLine (NSRange stringRange)
		{
			var h = CTTypesetterCreateLine (handle, stringRange);

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
			return CTTypesetterSuggestLineBreakWithOffset (handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestLineBreak (IntPtr typesetter, nint startIndex, double width);
		public nint SuggestLineBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestLineBreak (handle, startIndex, width);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreakWithOffset (IntPtr typesetter, nint startIndex, double width, double offset);
		public nint SuggestClusterBreak (int startIndex, double width, double offset)
		{
			return CTTypesetterSuggestClusterBreakWithOffset (handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreak (IntPtr typesetter, nint startIndex, double width);
		public nint SuggestClusterBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestClusterBreak (handle, startIndex, width);
		}
#endregion
	}
}

