//
// This file contains a generic version of NSOrderedSet
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

using ObjCRuntime;

namespace Foundation {
	[Register ("NSOrderedSet", SkipRegistration = true)]
	public sealed partial class NSOrderedSet<TKey> : NSOrderedSet, IEnumerable<TKey>
		where TKey : class, INativeObject {

		public NSOrderedSet ()
		{
		}

		public NSOrderedSet (NSCoder coder) : base (coder)
		{
		}

		internal NSOrderedSet (IntPtr handle) : base (handle)
		{
		}

		public NSOrderedSet (TKey start) : base (start)
		{
		}

		public NSOrderedSet (params TKey [] objs) : base (objs)
		{
		}

		public NSOrderedSet (NSSet<TKey> source) : base (source)
		{
		}

		public NSOrderedSet (NSOrderedSet<TKey> other) : base (other)
		{
		}

		public NSOrderedSet (NSMutableOrderedSet<TKey> other) : base (other)
		{
		}

		public new TKey this [nint idx] {
			get {
				var ret = _GetObject (idx);
				return Runtime.GetINativeObject <TKey> (ret, false);
			}
		}

		public TKey [] ToArray ()
		{
			return base.ToArray<TKey> ();
		}

		public bool Contains (TKey obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			return _Contains (obj.Handle);
		}

		public nint IndexOf (TKey obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			return _IndexOf (obj.Handle);
		}

		public TKey FirstObject ()
		{
			var ret = _FirstObject ();
			return Runtime.GetINativeObject <TKey> (ret, false);
		}

		public TKey LastObject ()
		{
			var ret = _LastObject ();
			return Runtime.GetINativeObject <TKey> (ret, false);
		}

		public NSSet<TKey> AsSet ()
		{
			var ret = _AsSet ();
			return Runtime.GetINativeObject <NSSet<TKey>> (ret, false);
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

		public static NSOrderedSet<TKey> operator + (NSOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			if (first == null)
				return second != null ? new NSOrderedSet<TKey> (second) : null;
			if (second == null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey> operator + (NSOrderedSet<TKey> first, NSSet<TKey> second)
		{
			if (first == null)
				return second != null ? new NSOrderedSet<TKey> (second) : null;
			if (second == null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey> operator - (NSOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			if (first == null)
				return null;
			if (second == null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey> operator - (NSOrderedSet<TKey> first, NSSet<TKey> second)
		{
			if (first == null)
				return null;
			if (second == null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static bool operator == (NSOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			// IsEqualToOrderedSet does not allow null
			if (object.ReferenceEquals (null, first))
				return object.ReferenceEquals (null, second);
			if (object.ReferenceEquals (null, second))
				return false;

			return first.IsEqualToOrderedSet (second);
		}

		public static bool operator != (NSOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			// IsEqualToOrderedSet does not allow null
			if (object.ReferenceEquals (null, first))
				return !object.ReferenceEquals (null, second);
			if (object.ReferenceEquals (null, second))
				return true;

			return !first.IsEqualToOrderedSet (second);
		}

		public override bool Equals (object other)
		{
			var o = other as NSOrderedSet<TKey>;
			if (o == null)
				return false;
			return IsEqualToOrderedSet (o);
		}

		public override int GetHashCode ()
		{
			return (int) GetNativeHash ();
		}
	}
}

#endif // XAMCORE_2_0