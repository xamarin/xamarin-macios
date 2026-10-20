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

using CoreFoundation;
using CoreGraphics;

namespace CoreText {

	#region Typesetter Values

	/// <summary>Options applicable to a <see cref="CoreText.CTTypesetter" /> object.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTTypesetterOptions {

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTTypesetterOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTTypesetterOptions (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public NSDictionary Dictionary { get; private set; }

		/// <summary>Developers should not use this deprecated property. </summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("ios6.0")]
		[ObsoletedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos")]
		[ObsoletedOSPlatform ("maccatalyst")]
		public bool DisableBidiProcessing {
			get {
				return CFDictionary.GetBooleanValue (Dictionary,
						CTTypesetterOptionKey.DisableBidiProcessing);
			}
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary,
						CTTypesetterOptionKey.DisableBidiProcessing, value);
			}
		}

		// The documentation says this is an NSNumber (not exactly which type), so 'int' is as good as anything else.
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? ForceEmbeddingLevel {
			get { return Adapter.GetInt32Value (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel); }
			set { Adapter.SetValue (Dictionary, CTTypesetterOptionKey.ForceEmbeddingLevel, value); }
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public bool AllowUnboundedLayout {
			get => CFDictionary.GetBooleanValue (Dictionary, CTTypesetterOptionKey.AllowUnboundedLayout);
			set {
				Adapter.AssertWritable (Dictionary);
				CFMutableDictionary.SetValue (Dictionary, CTTypesetterOptionKey.AllowUnboundedLayout, value);
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

	/// <summary>A class that performs line layout.</summary>
	///     <remarks>To be added.</remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTTypesetter : NativeObject {
		[Preserve (Conditional = true)]
		internal CTTypesetter (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		#region Typesetter Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedString (IntPtr @string);
		/// <param name="value">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTTypesetter (NSAttributedString value)
			: base (CTTypesetterCreateWithAttributedString (value.GetNonNullHandle (nameof (value))), true, true)
		{
			GC.KeepAlive (value);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateWithAttributedStringAndOptions (IntPtr @string, IntPtr options);
		/// <param name="value">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public CTTypesetter (NSAttributedString value, CTTypesetterOptions? options)
			: base (CTTypesetterCreateWithAttributedStringAndOptions (value.GetNonNullHandle (nameof (value)), options.GetHandle ()), true, true)
		{
			GC.KeepAlive (value);
			GC.KeepAlive (options);
		}
		#endregion

		#region Typeset Line Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLineWithOffset (IntPtr typesetter, NSRange stringRange, double offset);
		/// <param name="stringRange">To be added.</param>
		///         <param name="offset">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public CTLine? GetLine (NSRange stringRange, double offset)
		{
			var h = CTTypesetterCreateLineWithOffset (Handle, stringRange, offset);

			if (h == IntPtr.Zero)
				return null;

			return new CTLine (h, true);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTTypesetterCreateLine (IntPtr typesetter, NSRange stringRange);
		/// <param name="stringRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
		/// <param name="startIndex">To be added.</param>
		///         <param name="width">To be added.</param>
		///         <param name="offset">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public nint SuggestLineBreak (int startIndex, double width, double offset)
		{
			return CTTypesetterSuggestLineBreakWithOffset (Handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestLineBreak (IntPtr typesetter, nint startIndex, double width);
		/// <param name="startIndex">To be added.</param>
		///         <param name="width">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public nint SuggestLineBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestLineBreak (Handle, startIndex, width);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreakWithOffset (IntPtr typesetter, nint startIndex, double width, double offset);
		/// <param name="startIndex">To be added.</param>
		///         <param name="width">To be added.</param>
		///         <param name="offset">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public nint SuggestClusterBreak (int startIndex, double width, double offset)
		{
			return CTTypesetterSuggestClusterBreakWithOffset (Handle, startIndex, width, offset);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern nint CTTypesetterSuggestClusterBreak (IntPtr typesetter, nint startIndex, double width);
		/// <param name="startIndex">To be added.</param>
		///         <param name="width">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public nint SuggestClusterBreak (int startIndex, double width)
		{
			return CTTypesetterSuggestClusterBreak (Handle, startIndex, width);
		}
		#endregion
	}
}
