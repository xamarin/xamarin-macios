// 
// CTFontCollection.cs: Implements the managed CTFontCollection
//
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc (http://www.xamarin.com)
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

using CFIndex = System.IntPtr;

namespace CoreText {
	/// <summary>Options that can be used for creating <see cref="CoreText.CTFontCollection" /> objects.</summary>
	///     <remarks>
	///     </remarks>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontCollectionOptions {

		/// <summary>Default constructor, creates an empty set of options.</summary>
		///         <remarks>
		///         </remarks>
		public CTFontCollectionOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		/// <param name="dictionary">Dictionary with parameters.</param>
		///         <summary>Creates a strongly typed CTFontCollectionOptions from the contents of an NSDictionary that contains CTFontCollectionOptions keys.</summary>
		///         <remarks>
		///         </remarks>
		public CTFontCollectionOptions (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		/// <summary>The NSDictionary that represents the current values set.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public NSDictionary Dictionary { get; private set; }

		// The docs (and headers) only imply that this is a numeric value ('set to non-zero to ...')
		// No mention of the expected type (int? NSNumber?)
		/// <summary>If set, removes duplicate font descriptors.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		public bool RemoveDuplicates {
			get {
				if (CTFontCollectionOptionKey.RemoveDuplicates is null)
					return false;
				var v = Adapter.GetInt32Value (Dictionary, CTFontCollectionOptionKey.RemoveDuplicates);
				return v.HasValue ? v.Value != 0 : false;
			}
			set {
				if (CTFontCollectionOptionKey.RemoveDuplicates is null)
					throw new ArgumentOutOfRangeException (nameof (CTFontCollectionOptionKey.RemoveDuplicates));
				var v = value ? (int?) 1 : null;
				Adapter.SetValue (Dictionary, CTFontCollectionOptionKey.RemoveDuplicates!, v);
			}
		}
	}

	internal static class CTFontCollectionOptionsExtensions {
		public static IntPtr GetHandle (this CTFontCollectionOptions? @self)
		{
			if (@self is null)
				return IntPtr.Zero;
			return self.Dictionary.GetHandle ();
		}
	}

	/// <summary>Font collections are the standard mechanism used to enumerate fonts descriptors.</summary>
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CTFontCollection : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFontCollection (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		#region Collection Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateFromAvailableFonts (IntPtr options);
		/// <param name="options">Configuration options for creating the font collection, can be null.</param>
		///         <summary>Creates a CTFontCollection that contains all of the available font descriptors.</summary>
		///         <remarks>
		///         </remarks>
		public CTFontCollection (CTFontCollectionOptions? options)
			: base (CreateFromAvailableFonts (options), true, true)
		{
		}

		static IntPtr CreateFromAvailableFonts (CTFontCollectionOptions? options)
		{
			var rv = CTFontCollectionCreateFromAvailableFonts (options.GetHandle ());
			GC.KeepAlive (options);
			return rv;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateWithFontDescriptors (IntPtr queryDescriptors, IntPtr options);
		static IntPtr Create (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
		{
			using var descriptors = queryDescriptors is null ? null : CFArray.FromNativeObjects (queryDescriptors);
			IntPtr result = CTFontCollectionCreateWithFontDescriptors (descriptors.GetHandle (), options.GetHandle ());
			GC.KeepAlive (descriptors);
			GC.KeepAlive (options);
			return result;
		}
		/// <summary>Creates a CTFontCollection from the specified set of queryDescriptors.</summary>
		/// <param name="queryDescriptors">An array of font descriptors, can be null.</param>
		/// <param name="options">The options for matching font descriptors.</param>
		public CTFontCollection (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
			: base (Create (queryDescriptors, options), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateCopyWithFontDescriptors (IntPtr original, IntPtr queryDescriptors, IntPtr options);
		/// <param name="queryDescriptors">The font descriptors to add.</param>
		///         <param name="options">Configuration options for creating the font collection, can be null.</param>
		///         <summary>Creates a copy of the CTFontCollection while adding the specified font descriptors.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		public CTFontCollection? WithFontDescriptors (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
		{
			using var descriptors = queryDescriptors is null ? null : CFArray.FromNativeObjects (queryDescriptors);
			var h = CTFontCollectionCreateCopyWithFontDescriptors (Handle, descriptors.GetHandle (), options.GetHandle ());
			GC.KeepAlive (descriptors);
			GC.KeepAlive (options);
			if (h == IntPtr.Zero)
				return null;
			return new CTFontCollection (h, true);
		}
		#endregion

		#region Retrieving Matching Descriptors
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptors (IntPtr collection);
		/// <summary>Gets the mathching font descriptors from this collection.</summary>
		///         <returns>An array of font descriptors.</returns>
		///         <remarks>
		///         </remarks>
		public CTFontDescriptor [] GetMatchingFontDescriptors ()
		{
			var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptors (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontDescriptor> ();
			return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (fd, false), true)!;
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsWithOptions (IntPtr collection, IntPtr options);

		/// <param name="options">The options to match.</param>
		///         <summary>Returns an array of font descriptors that have the specified options.</summary>
		///         <returns>An array of font descriptors that have the specified options.</returns>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		public CTFontDescriptor [] GetMatchingFontDescriptors (CTFontCollectionOptions? options)
		{
			var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsWithOptions (Handle, options.GetHandle ());
			GC.KeepAlive (options);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontDescriptor> ();
			return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (fd, false), true)!;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static unsafe extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
				IntPtr collection, delegate* unmanaged<IntPtr, IntPtr, IntPtr, CFIndex> sortCallback,
				IntPtr refCon);

		[UnmanagedCallersOnly]
		static CFIndex CompareDescriptors (IntPtr first, IntPtr second, IntPtr context)
		{
			GCHandle c = GCHandle.FromIntPtr (context);
			var comparer = c.Target as Comparison<CTFontDescriptor>;
			if (comparer is null)
				return default (CFIndex);
			var rv = comparer (new CTFontDescriptor (first, false), new CTFontDescriptor (second, false));
			return (CFIndex) rv;
		}

		/// <param name="comparer">Sorting method.</param>
		///         <summary>Gets an array of font descriptors sorted by the specified sorting function.</summary>
		///         <returns>An array of font descriptors.</returns>
		///         <remarks>
		///         </remarks>
		public CTFontDescriptor? []? GetMatchingFontDescriptors (Comparison<CTFontDescriptor> comparer)
		{
			GCHandle comparison = GCHandle.Alloc (comparer);
			try {
				IntPtr cfArrayRef;
				unsafe {
					cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
						Handle,
						&CompareDescriptors,
						GCHandle.ToIntPtr (comparison));
					GC.KeepAlive (this);
				}
				if (cfArrayRef == IntPtr.Zero)
					return Array.Empty<CTFontDescriptor> ();
				return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (fd, false), true)!;
			} finally {
				comparison.Free ();
			}
		}
		#endregion
	}
}
