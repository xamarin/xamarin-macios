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

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if NET
using CFIndex = System.IntPtr;
#else
using CFIndex = System.nint;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreText {

#if !NET
	public static class CTFontCollectionOptionKey {
		public static readonly NSString? RemoveDuplicates;

		static CTFontCollectionOptionKey ()
		{
			RemoveDuplicates = Dlfcn.GetStringConstant (Libraries.CoreText.Handle, "kCTFontCollectionRemoveDuplicatesOption");
		}
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontCollectionOptions {

		public CTFontCollectionOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontCollectionOptions (NSDictionary dictionary)
		{
			if (dictionary is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionary));
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary { get; private set; }

		// The docs (and headers) only imply that this is a numeric value ('set to non-zero to ...')
		// No mention of the expected type (int? NSNumber?)
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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CTFontCollection : NativeObject {
		[Preserve (Conditional = true)]
		internal CTFontCollection (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		#region Collection Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateFromAvailableFonts (IntPtr options);
		public CTFontCollection (CTFontCollectionOptions? options)
			: base (CTFontCollectionCreateFromAvailableFonts (options.GetHandle ()), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateWithFontDescriptors (IntPtr queryDescriptors, IntPtr options);
		static IntPtr Create (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
		{
			using var descriptors = queryDescriptors is null ? null : CFArray.FromNativeObjects (queryDescriptors);
			return CTFontCollectionCreateWithFontDescriptors (descriptors.GetHandle (), options.GetHandle ());
		}
		public CTFontCollection (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
			: base (Create (queryDescriptors, options), true, true)
		{
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateCopyWithFontDescriptors (IntPtr original, IntPtr queryDescriptors, IntPtr options);
		public CTFontCollection? WithFontDescriptors (CTFontDescriptor []? queryDescriptors, CTFontCollectionOptions? options)
		{
			using var descriptors = queryDescriptors is null ? null : CFArray.FromNativeObjects (queryDescriptors);
			var h = CTFontCollectionCreateCopyWithFontDescriptors (Handle, descriptors.GetHandle (), options.GetHandle ());
			if (h == IntPtr.Zero)
				return null;
			return new CTFontCollection (h, true);
		}
		#endregion

		#region Retrieving Matching Descriptors
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptors (IntPtr collection);
		public CTFontDescriptor [] GetMatchingFontDescriptors ()
		{
			var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptors (Handle);
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontDescriptor> ();
			return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (fd, false), true)!;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[Watch (5, 0)]
#endif
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsWithOptions (IntPtr collection, IntPtr options);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[Watch (5, 0)]
#endif
		public CTFontDescriptor [] GetMatchingFontDescriptors (CTFontCollectionOptions? options)
		{
			var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsWithOptions (Handle, options.GetHandle ());
			if (cfArrayRef == IntPtr.Zero)
				return Array.Empty<CTFontDescriptor> ();
			return CFArray.ArrayFromHandleFunc (cfArrayRef, fd => new CTFontDescriptor (fd, false), true)!;
		}

#if NET
		[DllImport (Constants.CoreTextLibrary)]
		static unsafe extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
				IntPtr collection, delegate* unmanaged<IntPtr, IntPtr, IntPtr, CFIndex> sortCallback, 
				IntPtr refCon);
#else
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
				IntPtr collection, CTFontCollectionSortDescriptorsCallback sortCallback, IntPtr refCon);

		delegate CFIndex CTFontCollectionSortDescriptorsCallback (IntPtr first, IntPtr second, IntPtr refCon);
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CTFontCollectionSortDescriptorsCallback))]
#endif
		static CFIndex CompareDescriptors (IntPtr first, IntPtr second, IntPtr context)
		{
			GCHandle c = GCHandle.FromIntPtr (context);
			var comparer = c.Target as Comparison<CTFontDescriptor>;
			if (comparer is null)
				return default (CFIndex);
			var rv = comparer (new CTFontDescriptor (first, false), new CTFontDescriptor (second, false));
			return (CFIndex) rv;
		}

		public CTFontDescriptor? []? GetMatchingFontDescriptors (Comparison<CTFontDescriptor> comparer)
		{
			GCHandle comparison = GCHandle.Alloc (comparer);
			try {
				IntPtr cfArrayRef;
#if NET
				unsafe {
					cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
						Handle,
						&CompareDescriptors,
						GCHandle.ToIntPtr (comparison));
				}
#else
				cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
						Handle,
						new CTFontCollectionSortDescriptorsCallback (CompareDescriptors),
						GCHandle.ToIntPtr (comparison));
#endif
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
