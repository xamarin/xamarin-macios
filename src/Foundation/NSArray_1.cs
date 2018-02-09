//
// This file contains a generic version of NSArray
//
// Authors:
//		Alex Soto	(alex.soto@xamarin.com)
//
// Copyright 2015, Xamarin Inc.
//

#if XAMCORE_2_0

using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {
	[Register (SkipRegistration = true)]
	public sealed partial class NSArray<TKey> : NSArray, IEnumerable<TKey> 
		where TKey : class, INativeObject {

		public NSArray ()
		{			
		}

		public NSArray (NSCoder coder) : base (coder)
		{
		}

		internal NSArray (IntPtr handle) : base (handle)
		{
		}

		static public NSArray<TKey> FromNSObjects (params TKey [] items)
		{
			if (items == null)
				throw new ArgumentNullException (nameof (items));

			return FromNSObjects (items.Length, items);
		}

		static public NSArray<TKey> FromNSObjects (int count, params TKey [] items)
		{
			if (items == null)
				throw new ArgumentNullException (nameof (items));

			if (count > items.Length)
				throw new ArgumentException ("count is larger than the number of items", "count");

			IntPtr buf = Marshal.AllocHGlobal ((IntPtr) (count * IntPtr.Size));
			for (nint i = 0; i < count; i++) {
				var item = items [i];
				IntPtr h = item == null ? NSNull.Null.Handle : item.Handle;
				Marshal.WriteIntPtr (buf, (int)(i * IntPtr.Size), h);
			}
			IntPtr ret = NSArray.FromObjects (buf, count);
			NSArray<TKey> arr = Runtime.GetNSObject<NSArray<TKey>> (ret);
			Marshal.FreeHGlobal (buf);
			return arr;
		}

		#region IEnumerable<TKey>
		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator ()
		{
			return new NSFastEnumerator<TKey> (this);
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new NSFastEnumerator<TKey> (this);
		}
		#endregion

		public TKey this [nint idx] {
			get {
				return GetItem<TKey> ((nuint)idx);
			}
		}
	}
}
#endif // XAMCORE_2_0