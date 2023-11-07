//
// This file contains a generic version of NSMutableOrderedSet
//
// Authors:
//		Alex Soto	(alex.soto@xamarin.com)
//
// Copyright 2015, Xamarin Inc.
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Versioning;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[Register ("NSMutableOrderedSet", SkipRegistration = true)]
	public sealed partial class NSMutableOrderedSet<TKey> : NSMutableOrderedSet, IEnumerable<TKey>
		where TKey : class, INativeObject {

		public NSMutableOrderedSet ()
		{
		}

		public NSMutableOrderedSet (NSCoder coder) : base (coder)
		{
		}

		internal NSMutableOrderedSet (NativeHandle handle) : base (handle)
		{
		}

		public NSMutableOrderedSet (nint capacity) : base (capacity)
		{
		}

		public NSMutableOrderedSet (TKey start) : base (start)
		{
		}

		public NSMutableOrderedSet (params TKey [] objs) : base (objs)
		{
		}

		public NSMutableOrderedSet (NSSet<TKey> source) : base (source)
		{
		}

		public NSMutableOrderedSet (NSOrderedSet<TKey> other) : base (other)
		{
		}

		public NSMutableOrderedSet (NSMutableOrderedSet<TKey> other) : base (other)
		{
		}

		public new TKey this [nint idx] {
			get {
				var ret = _GetObject (idx);
				return Runtime.GetINativeObject<TKey> (ret, false);
			}

			set {
				if (value is null) // You can't pass nil here
					throw new ArgumentNullException (nameof (value));

				_SetObject (value.Handle, idx);
			}
		}

		public NSSet<TKey> AsSet ()
		{
			var ret = _AsSet ();
			return Runtime.GetINativeObject<NSSet<TKey>> (ret, false);
		}

		public void Insert (TKey obj, nint atIndex)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			_Insert (obj.Handle, atIndex);
		}

		public void Replace (nint objectAtIndex, TKey newObject)
		{
			if (newObject is null)
				throw new ArgumentNullException (nameof (newObject));

			_Replace (objectAtIndex, newObject.Handle);
		}

		public void Add (TKey obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			_Add (obj.Handle);
		}

		public void AddObjects (params TKey [] source)
		{
			if (source is null)
				throw new ArgumentNullException (nameof (source));

			_AddObjects (NSArray.FromNativeObjects (source));
		}

		public void InsertObjects (TKey [] objects, NSIndexSet atIndexes)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (atIndexes is null)
				throw new ArgumentNullException (nameof (atIndexes));

			_InsertObjects (NSArray.FromNativeObjects (objects), atIndexes);
		}

		public void ReplaceObjects (NSIndexSet indexSet, params TKey [] replacementObjects)
		{
			if (replacementObjects is null)
				throw new ArgumentNullException (nameof (replacementObjects));
			if (indexSet is null)
				throw new ArgumentNullException (nameof (indexSet));

			_ReplaceObjects (indexSet, NSArray.FromNativeObjects (replacementObjects));
		}

		public void RemoveObject (TKey obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			_RemoveObject (obj.Handle);
		}

		public void RemoveObjects (params TKey [] objects)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));

			_RemoveObjects (NSArray.FromNativeObjects (objects));
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

		public static NSMutableOrderedSet<TKey> operator + (NSMutableOrderedSet<TKey> first, NSMutableOrderedSet<TKey> second)
		{
			if (first is null)
				return second is not null ? new NSMutableOrderedSet<TKey> (second) : null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			return copy;
		}

		public static NSMutableOrderedSet<TKey> operator + (NSMutableOrderedSet<TKey> first, NSSet<TKey> second)
		{
			if (first is null)
				return second is not null ? new NSMutableOrderedSet<TKey> (second) : null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			return copy;
		}

		public static NSMutableOrderedSet<TKey> operator + (NSMutableOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			if (first is null)
				return second is not null ? new NSMutableOrderedSet<TKey> (second) : null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			return copy;
		}

		public static NSMutableOrderedSet<TKey> operator - (NSMutableOrderedSet<TKey> first, NSMutableOrderedSet<TKey> second)
		{
			if (first is null)
				return null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			return copy;
		}

		public static NSMutableOrderedSet<TKey> operator - (NSMutableOrderedSet<TKey> first, NSSet<TKey> second)
		{
			if (first is null)
				return null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			return copy;
		}

		public static NSMutableOrderedSet<TKey> operator - (NSMutableOrderedSet<TKey> first, NSOrderedSet<TKey> second)
		{
			if (first is null)
				return null;
			if (second is null)
				return new NSMutableOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			return copy;
		}

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public void ApplyDifference (NSOrderedCollectionDifference<TKey> difference)
		{
			if (difference is null)
				throw new ArgumentNullException (nameof (difference));
			_ApplyDifference (difference.Handle);
		}
#endif
	}
}
