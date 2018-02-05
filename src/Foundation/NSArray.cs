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
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSArray {

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

		public static NSArray FromNSObjects<T> (Func<T, NSObject> nsobjectificator, params T [] items)
		{
			if (nsobjectificator == null)
				throw new ArgumentNullException (nameof (nsobjectificator));
			if (items == null)
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

		internal static NSArray From<T> (T[] items, long count = -1)
		{
			if ((items == null) || (count == 0))
				return new NSArray ();

			if (count == -1)
				count = items.Length;
			else if (count > items.Length)
				throw new ArgumentException ("count is larger than the number of items", "count");

			NSObject [] nsoa = new NSObject [count];
			for (nint i = 0; i < count; i++){
				var k = NSObject.FromObject (items [i]);
				if (k == null)
					throw new ArgumentException (String.Format ("Do not know how to marshal object of type '{0}' to an NSObject", items [i].GetType ()));
				nsoa [i] = k;
			}
			return FromNSObjects (nsoa);
		}
		
		internal static NSArray FromNativeObjects (INativeObject[] items)
		{
			if (items == null)
				return new NSArray ();

			return FromNativeObjects (items, items.Length);
		}

		internal static NSArray FromNativeObjects (INativeObject [] items, nint count)
		{
			if (items == null)
				return new NSArray ();

			if (count > items.Length)
				throw new ArgumentException ("count is larger than the number of items", "count");

			IntPtr buf = Marshal.AllocHGlobal ((IntPtr) (count * IntPtr.Size));
			for (nint i = 0; i < count; i++) {
				var item = items [i];
				IntPtr h = item == null ? NSNull.Null.Handle : item.Handle;
				Marshal.WriteIntPtr (buf, (int)(i * IntPtr.Size), h);
			}
			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);
			return arr;
		}
		
		internal static NSArray FromNSObjects (IList<NSObject> items)
		{
			if (items == null)
				return new NSArray ();

			int count = items.Count;
			IntPtr buf = Marshal.AllocHGlobal (count * IntPtr.Size);
			for (int i = 0; i < count; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, items [i].Handle);
			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, count));
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		static public NSArray FromStrings (params string [] items)
		{
			if (items == null)
				throw new ArgumentNullException (nameof (items));
			
			IntPtr buf = Marshal.AllocHGlobal (items.Length * IntPtr.Size);
			try {
				NSString [] strings = new NSString [items.Length];
				
				for (int i = 0; i < items.Length; i++){
					IntPtr val;
					
					if (items [i] == null)
						val = NSNull.Null.Handle;
					else {
						strings [i] = new NSString (items [i]);
						val = strings [i].Handle;
					}
	
					Marshal.WriteIntPtr (buf, i * IntPtr.Size, val);
				}
				NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, items.Length));
				foreach (NSString ns in strings) {
					if (ns != null)
						ns.Dispose ();
				}
				return arr;
			} finally {
				Marshal.FreeHGlobal (buf);
			}
		}

		static public NSArray FromIntPtrs (IntPtr [] vals)
		{
			if (vals == null)
				throw new ArgumentNullException ("vals");
			int n = vals.Length;
			IntPtr buf = Marshal.AllocHGlobal (n * IntPtr.Size);
			for (int i = 0; i < n; i++)
				Marshal.WriteIntPtr (buf, i * IntPtr.Size, vals [i]);

			NSArray arr = Runtime.GetNSObject<NSArray> (NSArray.FromObjects (buf, vals.Length));

			Marshal.FreeHGlobal (buf);
			return arr;
		}

		static nuint GetCount (IntPtr handle)
		{
#if XAMCORE_2_0
	#if MONOMAC
			return Messaging.nuint_objc_msgSend (handle, selCountHandle);
	#else
			return Messaging.nuint_objc_msgSend (handle, Selector.GetHandle ("count"));
	#endif
#else
	#if MONOMAC
			return Messaging.UInt32_objc_msgSend (handle, selCountHandle);
	#else
			return Messaging.UInt32_objc_msgSend (handle, Selector.GetHandle ("count"));
	#endif
#endif
		}

		static IntPtr GetAtIndex (IntPtr handle, nuint i)
		{
#if XAMCORE_2_0
	#if MONOMAC
			return Messaging.IntPtr_objc_msgSend_nuint (handle, selObjectAtIndex_Handle, i);
	#else
			return Messaging.IntPtr_objc_msgSend_nuint (handle, Selector.GetHandle ("objectAtIndex:"), i);
	#endif
#else
	#if MONOMAC
			return Messaging.IntPtr_objc_msgSend_UInt32 (handle, selObjectAtIndex_Handle, i);
	#else
			return Messaging.IntPtr_objc_msgSend_UInt32 (handle, Selector.GetHandle ("objectAtIndex:"), i);
	#endif
#endif
		}
			
		static public string [] StringArrayFromHandle (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			var c = GetCount (handle);
			string [] ret = new string [c];

			for (nuint i = 0; i < c; i++)
				ret [i] = NSString.FromHandle (GetAtIndex (handle, i));
			return ret;
		}

		static public T [] ArrayFromHandle<T> (IntPtr handle) where T : class, INativeObject
		{
			if (handle == IntPtr.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++) {
				ret [i] = UnsafeGetItem<T> (handle, i);
			}
			return ret;
		}

		static public T [] EnumsFromHandle<T> (IntPtr handle) where T : struct, IConvertible
		{
			if (handle == IntPtr.Zero)
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
			if (weakArray == null || weakArray.Handle == IntPtr.Zero)
				return null;
			try {
				nuint n = weakArray.Count;
				T [] ret = new T [n];
				for (nuint i = 0; i < n; i++){
					ret [i] = Runtime.GetNSObject<T> (weakArray.ValueAt (i));
				}
				return ret;
			} catch {
				return null;
			}
		}

		static public T [] FromArrayNative<T> (NSArray weakArray) where T : class, INativeObject
		{
			if (weakArray == null || weakArray.Handle == IntPtr.Zero)
				return null;
			try {
				nuint n = weakArray.Count;
				T [] ret = new T [n];
				for (nuint i = 0; i < n; i++){
					ret [i] = Runtime.GetINativeObject<T> (weakArray.ValueAt (i), false);
				}
				return ret;
			} catch {
				return null;
			}
		}
		
		// Used when we need to provide our constructor
		static public T [] ArrayFromHandleFunc<T> (IntPtr handle, Func<IntPtr,T> createObject) 
		{
			if (handle == IntPtr.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++)
				ret [i] = createObject (GetAtIndex (handle, i));

			return ret;
		}
		
		static public T [] ArrayFromHandle<T> (IntPtr handle, Converter<IntPtr, T> creator) 
		{
			if (handle == IntPtr.Zero)
				return null;

			var c = GetCount (handle);
			T [] ret = new T [c];

			for (uint i = 0; i < c; i++)
				ret [i] = creator (GetAtIndex (handle, i));

			return ret;
		}

		// FIXME: before proving a real `this` indexer we need to clean the issues between
		// NSObject and INativeObject coexistance across all the API (it can not return T)

		static T UnsafeGetItem<T> (IntPtr handle, nuint index) where T : class, INativeObject
		{
			var val = GetAtIndex (handle, index);
			// A native code could return NSArray with NSNull.Null elements
			// and they should be valid for things like T : NSDate so we handle
			// them as just null values inside the array
			if (val == NSNull.Null.Handle)
				return null;

			return Runtime.GetINativeObject<T> (val, false);
		}

		// can return an INativeObject or an NSObject
#if XAMCORE_2_0
		public T GetItem<T> (nuint index) where T : class, INativeObject
		{
			if (index >= GetCount (Handle))
				throw new ArgumentOutOfRangeException ("index");

			return UnsafeGetItem<T> (Handle, index);
		}
#else
		public T GetItem<T> (int index) where T : class, INativeObject
		{
			if (index < 0 || index >= GetCount (Handle))
				throw new ArgumentOutOfRangeException ("index");

			return UnsafeGetItem<T> (Handle, (uint) index);
		}
#endif

		public static NSObject[][] FromArrayOfArray (NSArray weakArray)
		{
			if (weakArray == null || weakArray.Handle == IntPtr.Zero)
				return null;

			try {
				nuint n = weakArray.Count;
				var ret = new NSObject[n][];
				for (nuint i = 0; i < n; i++)
					ret [i] = NSArray.FromArray<NSObject> (weakArray.GetItem<NSArray> (
#if !XAMCORE_2_0
						(int)
#endif
						i
					));
				return ret;
			} catch {
				return null;
			}
		}

		public static NSArray From (NSObject[][] items)
		{
			if (items == null)
				return null;

			try {
				var ret = new NSMutableArray ((nuint)items.Length);
				for (int i = 0; i < items.Length; i++)
					ret.Add (NSArray.FromNSObjects (items [i]));
				return ret;
			} catch {
				return null;
			}
		}
	}
}
