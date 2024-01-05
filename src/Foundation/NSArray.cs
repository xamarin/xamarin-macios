//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc
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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
	public delegate bool NSOrderedCollectionDifferenceEquivalenceTest (NSObject first, NSObject second);
#endif

	public partial class NSArray : IEnumerable<NSObject> {

		//
		// Creates an array with the elements;   If the value passed is null, it
		// still creates an NSArray object, but the Handle is set to IntPtr.Zero,
		// this is so it makes it simpler for the generator to support
		// [NullAllowed] on array parameters.
		//
		static public NSArray FromNSObjects (params NSObject [] items)
		{
			return FromNativeObjects (items);
		}

		static public NSArray FromNSObjects (int count, params NSObject [] items)
		{
			return FromNativeObjects (items, count);
		}

		public static NSArray FromNSObjects (params INativeObject [] items)
		{
			return FromNativeObjects (items);
		}

		public static NSArray FromNSObjects (int count, params INativeObject [] items)
		{
			return FromNativeObjects (items, count);
		}

		public static NSArray FromNSObjects<T> (params T [] items) where T : class, INativeObject
		{
			return FromNativeObjects (items);
		}
		public static NSArray FromNSObjects<T> (params T [] [] items) where T : class, INativeObject
		{
			if (items is null)
				return null;

			var ret = new NSMutableArray ((nuint) items.Length);
			for (var i = 0; i < items.Length; i++) {
				var row = items [i];
				if (row is null)
					throw new ArgumentNullException (nameof (items), $"Element [{i}] is null");
				for (var j = 0; j < row.Length; j++) {
					var element = row [j];
					if (element is null)
						throw new ArgumentNullException (nameof (items), $"Element [{i}][{j}] is null");
				}
				ret.Add (NSArray.FromNSObjects (row));
			}

			return ret;
		}
		public static NSArray FromNSObjects<T> (T [,] items) where T : class, INativeObject
		{
			if (items is null)
				return null;

			var width = items.GetLength (0);
			var height = items.GetLength (1);
			var ret = new T [height] [];
			for (var y = 0; y < height; y++) {
				var row = new T [width];
				for (var x = 0; x < width; x++) {
					row [x] = items [x, y];
				}
				ret [y] = row;
			}
			return FromNSObjects (ret);
		}
		public static NSArray FromNSObjects<T> (int count, params T [] items) where T : class, INativeObject
		{
			return FromNativeObjects (items, count);
		}

		public static NSArray FromNSObjects<T> (Func<T, NSObject> nsobjectificator, params T [] items)
		{
			if (nsobjectificator is null)
				throw new ArgumentNullException (nameof (nsobjectificator));
			if (items is null)
				return null;

			var arr = new NSObject [items.Length];
			for (int i = 0; i < items.Length; i++) {
				arr [i] = nsobjectificator (items [i]);
			}

			return FromNativeObjects (arr);
		}

		public static NSArray FromObjects (params object [] items)
		{
			return From<object> (items);
		}

		public static NSArray FromObjects (nint count, params object [] items)
		{
			return From<object> (items, count);
		}

		internal static NSArray From<T> (T [] items, long count = -1)
		{
			if ((items is null) || (count == 0))
				return new NSArray ();

			if (count == -1)
				count = items.Length;
			else if (count > items.Length)
				throw new ArgumentException ("count is larger than the number of items", "count");

			NSObject [] nsoa = new NSObject [count];
			for (nint i = 0; i < count; i++) {
				var k = NSObject.FromObject (items [i]);
				if (k is null)
					throw new ArgumentException (String.Format ("Do not know how to marshal object of type '{0}' to an NSObject", items [i].GetType ()));
				nsoa [i] = k;
			}
			return FromNSObjects (nsoa);
		}

		internal static NSArray FromNativeObjects<T> (T [] items) where T : class, INativeObject
		{
			if (items is null)
				return new NSArray ();

			return FromNativeObjects<T> (items, items.Length);
		}

		internal static NSArray FromNativeObjects<T> (T [] items, nint count) where T : class, INativeObject
		{
			if (items is null)
				return new NSArray ();

			if (count > items.Length)
				throw new ArgumentException ("count is larger than the number of items", "count");

			IntPtr buf = Marshal.AllocHGlobal ((IntPtr) (count * IntPtr.Size));
			for (nint i = 0; i < count; i++) {
				var item = items [i];
				IntPtr h = item is null ? NSNull.Null.Handle : item.Handle;
				Marshal.WriteIntPtr (buf, (int) (i * IntPtr.Size), h);
			}
			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		internal static NSArray FromNSObjects (IList<NSObject> items)
		{
			if (items is null)
				return new NSArray ();

			int count = items.Count;
			IntPtr buf = Marshal.AllocHGlobal (count * IntPtr.Size);
			for (int i = 0; i < count; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, items [i].Handle);
			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		static public NSArray FromStrings (params string [] items) => FromStrings ((IReadOnlyList<string>) items);

		static public NSArray FromStrings (IReadOnlyList<string> items)
		{
			if (items is null)
				throw new ArgumentNullException (nameof (items));

			IntPtr buf = Marshal.AllocHGlobal (items.Count * IntPtr.Size);
			try {
				for (int i = 0; i < items.Count; i++) {
					IntPtr val;

					if (items [i] is null)
						val = NSNull.Null.Handle;
					else {
						val = NSString.CreateNative (items [i], true);
					}

					Marshal.WriteIntPtr (buf, i * IntPtr.Size, val);
				}
				NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, items.Count));
				return arr;
			} finally {
				Marshal.FreeHGlobal (buf);
			}
		}

		static public NSArray FromIntPtrs (NativeHandle [] vals)
		{
			if (vals is null)
				throw new ArgumentNullException ("vals");
			int n = vals.Length;
			IntPtr buf = Marshal.AllocHGlobal (n * IntPtr.Size);
			for (int i = 0; i < n; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, vals [i]);

			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, vals.Length));

			Marshal.FreeHGlobal (buf);
			return arr;
		}

		internal static nuint GetCount (IntPtr handle)
		{
#if MONOMAC
			return (nuint) Messaging.UIntPtr_objc_msgSend (handle, selCountXHandle);
#else
			return (nuint) Messaging.UIntPtr_objc_msgSend (handle, Selector.GetHandle ("count"));
#endif
		}

		internal static NativeHandle GetAtIndex (NativeHandle handle, nuint i)
		{
#if NET
			return Messaging.NativeHandle_objc_msgSend_UIntPtr (handle, Selector.GetHandle ("objectAtIndex:"), (UIntPtr) i);
#else
#if MONOMAC
			return Messaging.IntPtr_objc_msgSend_UIntPtr (handle, selObjectAtIndex_XHandle, (UIntPtr) i);
#else
			return Messaging.IntPtr_objc_msgSend_UIntPtr (handle, Selector.GetHandle ("objectAtIndex:"), (UIntPtr) i);
#endif
#endif
		}

		[Obsolete ("Use of 'CFArray.StringArrayFromHandle' offers better performance.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		static public string [] StringArrayFromHandle (NativeHandle handle)
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = GetCount (handle);
			string [] ret = new string [c];

			for (nuint i = 0; i < c; i++)
				ret [i] = CFString.FromHandle (GetAtIndex (handle, i));
			return ret;
		}

		static public T [] ArrayFromHandle<T> (NativeHandle handle) where T : class, INativeObject
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++) {
				ret [i] = UnsafeGetItem<T> (handle, i);
			}
			return ret;
		}

		static Array ArrayFromHandle (NativeHandle handle, Type elementType)
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = (int) GetCount (handle);
			var rv = Array.CreateInstance (elementType, c);
			for (int i = 0; i < c; i++) {
				rv.SetValue (UnsafeGetItem (handle, (nuint) i, elementType), i);
			}
			return rv;
		}

		static public T [] EnumsFromHandle<T> (NativeHandle handle) where T : struct, IConvertible
		{
			if (handle == NativeHandle.Zero)
				return null;
			if (!typeof (T).IsEnum)
				throw new ArgumentException ("T must be an enum");

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++) {
				ret [i] = (T) Convert.ChangeType (UnsafeGetItem<NSNumber> (handle, i).LongValue, typeof (T));
			}
			return ret;
		}

		static public T [] FromArray<T> (NSArray weakArray) where T : NSObject
		{
			if (weakArray is null || weakArray.Handle == NativeHandle.Zero)
				return null;
			try {
				nuint n = weakArray.Count;
				T [] ret = new T [n];
				for (nuint i = 0; i < n; i++) {
					ret [i] = Runtime.GetNSObject<T> (weakArray.ValueAt (i));
				}
				return ret;
			} catch {
				return null;
			}
		}

		static public T [] FromArrayNative<T> (NSArray weakArray) where T : class, INativeObject
		{
			if (weakArray is null || weakArray.Handle == NativeHandle.Zero)
				return null;
			try {
				nuint n = weakArray.Count;
				T [] ret = new T [n];
				for (nuint i = 0; i < n; i++) {
					ret [i] = Runtime.GetINativeObject<T> (weakArray.ValueAt (i), false);
				}
				return ret;
			} catch {
				return null;
			}
		}

		// Used when we need to provide our constructor
		static public T [] ArrayFromHandleFunc<T> (NativeHandle handle, Func<NativeHandle, T> createObject)
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++)
				ret [i] = createObject (GetAtIndex (handle, i));

			return ret;
		}

		static public T [] ArrayFromHandle<T> (NativeHandle handle, Converter<NativeHandle, T> creator)
		{
			if (handle == NativeHandle.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++)
				ret [i] = creator (GetAtIndex (handle, i));

			return ret;
		}

		static public T [] ArrayFromHandle<T> (NativeHandle handle, Converter<NativeHandle, T> creator, bool releaseHandle)
		{
			var rv = ArrayFromHandle<T> (handle, creator);
			if (releaseHandle && handle == NativeHandle.Zero)
				NSObject.DangerousRelease (handle);
			return rv;
		}

		// FIXME: before proving a real `this` indexer we need to clean the issues between
		// NSObject and INativeObject coexistance across all the API (it can not return T)

		static T UnsafeGetItem<T> (NativeHandle handle, nuint index) where T : class, INativeObject
		{
			var val = GetAtIndex (handle, index);
			// A native code could return NSArray with NSNull.Null elements
			// and they should be valid for things like T : NSDate so we handle
			// them as just null values inside the array
			if (val == NSNull.Null.Handle)
				return null;

			return Runtime.GetINativeObject<T> (val, false);
		}

		static object UnsafeGetItem (NativeHandle handle, nuint index, Type type)
		{
			var val = GetAtIndex (handle, index);
			// A native code could return NSArray with NSNull.Null elements
			// and they should be valid for things like T : NSDate so we handle
			// them as just null values inside the array
			if (val == NSNull.Null.Handle)
				return null;

			return Runtime.GetINativeObject (val, false, type);
		}

		// can return an INativeObject or an NSObject
		public T GetItem<T> (nuint index) where T : class, INativeObject
		{
			if (index >= GetCount (Handle))
				throw new ArgumentOutOfRangeException ("index");

			return UnsafeGetItem<T> (Handle, index);
		}

		public static NSObject [] [] FromArrayOfArray (NSArray weakArray)
		{
			if (weakArray is null || weakArray.Handle == IntPtr.Zero)
				return null;

			try {
				nuint n = weakArray.Count;
				var ret = new NSObject [n] [];
				for (nuint i = 0; i < n; i++)
					ret [i] = NSArray.FromArray<NSObject> (weakArray.GetItem<NSArray> (i));
				return ret;
			} catch {
				return null;
			}
		}

		public static NSArray From (NSObject [] [] items)
		{
			if (items is null)
				return null;

			try {
				var ret = new NSMutableArray ((nuint) items.Length);
				for (int i = 0; i < items.Length; i++)
					ret.Add (NSArray.FromNSObjects (items [i]));
				return ret;
			} catch {
				return null;
			}
		}

		public TKey [] ToArray<TKey> () where TKey : class, INativeObject
		{
			var rv = new TKey [GetCount (Handle)];
			for (var i = 0; i < rv.Length; i++)
				rv [i] = GetItem<TKey> ((nuint) i);
			return rv;
		}

		public NSObject [] ToArray ()
		{
			return ToArray<NSObject> ();
		}

		IEnumerator<NSObject> IEnumerable<NSObject>.GetEnumerator ()
		{
			return new NSFastEnumerator<NSObject> (this);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new NSFastEnumerator<NSObject> (this);
		}

#if false // https://github.com/xamarin/xamarin-macios/issues/15577

		static readonly NSOrderedCollectionDifferenceEquivalenceTestProxy static_DiffEquality = DiffEqualityHandler;

		[MonoPInvokeCallback (typeof (NSOrderedCollectionDifferenceEquivalenceTestProxy))]
		static bool DiffEqualityHandler (IntPtr block, IntPtr first, IntPtr second)
		{
			var callback = BlockLiteral.GetTarget<NSOrderedCollectionDifferenceEquivalenceTest> (block);
			if (callback is not null) {
				var nsFirst = Runtime.GetNSObject<NSObject> (first, false);
				var nsSecond = Runtime.GetNSObject<NSObject> (second, false);
				return callback (nsFirst, nsSecond);
			}
			return false;
		}

#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public NSOrderedCollectionDifference GetDifferenceFromArray (NSArray other, NSOrderedCollectionDifferenceCalculationOptions options, NSOrderedCollectionDifferenceEquivalenceTest equivalenceTest) 
		{
			if (equivalenceTest is null)
				throw new ArgumentNullException (nameof (equivalenceTest));

			var block = new BlockLiteral ();
			block.SetupBlock (static_DiffEquality, equivalenceTest);
			try {
				return Runtime.GetNSObject<NSOrderedCollectionDifference> (_GetDifferenceFromArray (other, options, ref block));
			} finally {
				block.CleanupBlock ();
			}
		}
#endif
	}
}
