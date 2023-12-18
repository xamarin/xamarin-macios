//
// This file contains a generic version of NSOrderedSet
//
// Authors:
//		Alex Soto	(alex.soto@xamarin.com)
//
// Copyright 2015, Xamarin Inc.
//
#nullable enable

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
	[Register ("NSOrderedSet", SkipRegistration = true)]
	public sealed partial class NSOrderedSet<TKey> : NSOrderedSet, IEnumerable<TKey>
		where TKey : class, INativeObject {

		public NSOrderedSet ()
		{
		}

		public NSOrderedSet (NSCoder coder) : base (coder)
		{
		}

		internal NSOrderedSet (NativeHandle handle) : base (handle)
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

		public new TKey? this [nint idx] {
			get {
				var ret = _GetObject (idx);
				return Runtime.GetINativeObject<TKey> (ret, false);
			}
		}

		public TKey [] ToArray ()
		{
			return base.ToArray<TKey> ();
		}

		public bool Contains (TKey obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			return _Contains (obj.Handle);
		}

		public nint IndexOf (TKey obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			return _IndexOf (obj.Handle);
		}

		public TKey? FirstObject ()
		{
			var ret = _FirstObject ();
			return Runtime.GetINativeObject<TKey> (ret, false);
		}

		public TKey? LastObject ()
		{
			var ret = _LastObject ();
			return Runtime.GetINativeObject<TKey> (ret, false);
		}

		public NSSet<TKey>? AsSet ()
		{
			var ret = _AsSet ();
			return Runtime.GetINativeObject<NSSet<TKey>> (ret, false);
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

		public static NSOrderedSet<TKey>? operator + (NSOrderedSet<TKey>? first, NSOrderedSet<TKey>? second)
		{
			if (first is null)
				return second is not null ? new NSOrderedSet<TKey> (second) : null;
			if (second is null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey>? operator + (NSOrderedSet<TKey>? first, NSSet<TKey>? second)
		{
			if (first is null)
				return second is not null ? new NSOrderedSet<TKey> (second) : null;
			if (second is null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.UnionSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey>? operator - (NSOrderedSet<TKey>? first, NSOrderedSet<TKey>? second)
		{
			if (first is null)
				return null;
			if (second is null)
				return new NSOrderedSet<TKey> (first);
			var copy = new NSMutableOrderedSet<TKey> (first);
			copy.MinusSet (second);
			var copyset = new NSOrderedSet<TKey> (copy);
			return copyset;
		}

		public static NSOrderedSet<TKey>? operator - (NSOrderedSet<TKey>? first, NSSet<TKey>? second)
		{
			if (first is null)
				return null;
			if (second is null)
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
			if (o is null)
				return false;
			return IsEqualToOrderedSet (o);
		}

		public override int GetHashCode ()
		{
			return (int) GetNativeHash ();
		}

#if false // https://github.com/xamarin/xamarin-macios/issues/15577

#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public NSOrderedCollectionDifference<TKey> GetDifference (NSOrderedSet<TKey> other, NSOrderedCollectionDifferenceCalculationOptions options)
			=> new NSOrderedCollectionDifference<TKey> (_GetDifference (other, options));

#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public NSOrderedCollectionDifference<TKey> GetDifference (NSOrderedSet other)
			=> new NSOrderedCollectionDifference<TKey> (_GetDifference (other));

#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public NSOrderedSet<TKey>? GetOrderedSet (NSOrderedCollectionDifference difference)
		{
			var ptr = _GetOrderedSet (difference); 
			return Runtime.GetNSObject<NSOrderedSet<TKey>> (ptr);
		}

		static readonly NSOrderedCollectionDifferenceEquivalenceTestProxy static_DiffEqualityGeneric = DiffEqualityHandlerGeneric;

		[MonoPInvokeCallback (typeof (NSOrderedCollectionDifferenceEquivalenceTestProxy))]
		static bool DiffEqualityHandlerGeneric (IntPtr block, IntPtr first, IntPtr second)
		{
			var callback = BlockLiteral.GetTarget<NSOrderedCollectionDifferenceEquivalenceTest<TKey>> (block);
			if (callback is not null) {
				var nsFirst = Runtime.GetINativeObject<TKey> (first, false);
				var nsSecond = Runtime.GetINativeObject<TKey> (second, false);
				return callback (nsFirst, nsSecond);
			}
			return false;
		}

#if !NET
		[Watch (6,0), TV (13,0), iOS (13,0)]
#else
		[SupportedOSPlatform ("ios13.0"), SupportedOSPlatform ("tvos13.0"), SupportedOSPlatform ("macos")]
#endif
		public NSOrderedCollectionDifference<TKey>? GetDifference (NSOrderedSet<TKey> other, NSOrderedCollectionDifferenceCalculationOptions options, NSOrderedCollectionDifferenceEquivalenceTest<TKey> equivalenceTest) 
		{
			if (equivalenceTest is null)
				throw new ArgumentNullException (nameof (equivalenceTest));

			var block = new BlockLiteral ();
			block.SetupBlock (static_DiffEqualityGeneric, equivalenceTest);
			try {
				return Runtime.GetNSObject<NSOrderedCollectionDifference<TKey>> (_GetDifference (other, options, ref block));
			} finally {
				block.CleanupBlock ();
			}
		}
#endif
	}
}
