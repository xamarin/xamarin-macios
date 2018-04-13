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
using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

using CFIndex = System.nint;

namespace CoreText {

	public static class CTFontCollectionOptionKey {
		public static readonly NSString RemoveDuplicates;

		static CTFontCollectionOptionKey ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreTextLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				RemoveDuplicates = Dlfcn.GetStringConstant (handle, "kCTFontCollectionRemoveDuplicatesOption");
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
	}

	public class CTFontCollectionOptions {

		public CTFontCollectionOptions ()
			: this (new NSMutableDictionary ())
		{
		}

		public CTFontCollectionOptions (NSDictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException ("dictionary");
			Dictionary = dictionary;
		}

		public NSDictionary Dictionary {get; private set;}

		// The docs (and headers) only imply that this is a numeric value ('set to non-zero to ...')
		// No mention of the expected type (int? NSNumber?)
		public bool RemoveDuplicates {
			get {
				var v = Adapter.GetInt32Value (Dictionary, CTFontCollectionOptionKey.RemoveDuplicates);
				return v.HasValue ? v.Value != 0 : false;
			}
			set {
				var v = value ? (int?) 1 : null;
				Adapter.SetValue (Dictionary, CTFontCollectionOptionKey.RemoveDuplicates, v);
			}
		}
	}

	public class CTFontCollection : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CTFontCollection (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw ConstructorError.ArgumentNull (this, "handle");
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CTFontCollection ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}

#region Collection Creation
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateFromAvailableFonts (IntPtr options);
		public CTFontCollection (CTFontCollectionOptions options)
		{
			handle = CTFontCollectionCreateFromAvailableFonts (
					options == null ? IntPtr.Zero : options.Dictionary.Handle);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateWithFontDescriptors (IntPtr queryDescriptors, IntPtr options);
		public CTFontCollection (CTFontDescriptor[] queryDescriptors, CTFontCollectionOptions options)
		{
			using (var descriptors = queryDescriptors == null
					? null 
					: CFArray.FromNativeObjects (queryDescriptors))
				handle = CTFontCollectionCreateWithFontDescriptors (
						descriptors == null ? IntPtr.Zero : descriptors.Handle,
						options == null ? IntPtr.Zero : options.Dictionary.Handle);
			if (handle == IntPtr.Zero)
				throw ConstructorError.Unknown (this);
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateCopyWithFontDescriptors (IntPtr original, IntPtr queryDescriptors, IntPtr options);
		public CTFontCollection WithFontDescriptors (CTFontDescriptor[] queryDescriptors, CTFontCollectionOptions options)
		{
			IntPtr h;
			using (var descriptors = queryDescriptors == null 
					? null 
					: CFArray.FromNativeObjects (queryDescriptors)) {
				h = CTFontCollectionCreateCopyWithFontDescriptors (
						handle,
						descriptors == null ? IntPtr.Zero : descriptors.Handle,
						options == null ? IntPtr.Zero : options.Dictionary.Handle);
			}
			if (h == IntPtr.Zero)
				return null;
			return new CTFontCollection (h, true);
		}
#endregion

#region Retrieving Matching Descriptors
		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptors (IntPtr collection);
		public CTFontDescriptor[] GetMatchingFontDescriptors ()
		{
			var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptors (handle);
			if (cfArrayRef == IntPtr.Zero)
				return new CTFontDescriptor [0];
			var matches = NSArray.ArrayFromHandle (cfArrayRef,
					fd => new CTFontDescriptor (fd, false));
			CFObject.CFRelease (cfArrayRef);
			return matches;
		}

		[DllImport (Constants.CoreTextLibrary)]
		static extern IntPtr CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
				IntPtr collection, CTFontCollectionSortDescriptorsCallback sortCallback, IntPtr refCon);

		delegate CFIndex CTFontCollectionSortDescriptorsCallback (IntPtr first, IntPtr second, IntPtr refCon);

		[MonoPInvokeCallback (typeof (CTFontCollectionSortDescriptorsCallback))]
		static CFIndex CompareDescriptors (IntPtr first, IntPtr second, IntPtr context)
		{
			GCHandle c = GCHandle.FromIntPtr (context);
			var comparer = c.Target as Comparison<CTFontDescriptor>;
			if (comparer == null)
				return 0;
			return comparer (new CTFontDescriptor (first, false), new CTFontDescriptor (second, false));
		}

		public CTFontDescriptor[] GetMatchingFontDescriptors (Comparison<CTFontDescriptor> comparer)
		{
			GCHandle comparison = GCHandle.Alloc (comparer);
			try {
				var cfArrayRef = CTFontCollectionCreateMatchingFontDescriptorsSortedWithCallback (
						handle, 
						new CTFontCollectionSortDescriptorsCallback (CompareDescriptors),
						GCHandle.ToIntPtr (comparison));
				if (cfArrayRef == IntPtr.Zero)
					return new CTFontDescriptor [0];
				var matches = NSArray.ArrayFromHandle (cfArrayRef,
						fd => new CTFontDescriptor (fd, false));
				CFObject.CFRelease (cfArrayRef);
				return matches;
			}
			finally {
				comparison.Free ();
			}
		}
#endregion
	}
}

