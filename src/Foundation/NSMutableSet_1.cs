//
// Copyright 2015 Xamarin Inc (http://www.xamarin.com)
//
// This file contains a generic version of NSMutableSet.
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
//

#if XAMCORE_2_0

using System;
using System.Collections;
using System.Collections.Generic;

using ObjCRuntime;

namespace Foundation {
	[Register ("NSMutableSet", SkipRegistration = true)]
	public sealed partial class NSMutableSet<TKey> : NSMutableSet, IEnumerable<TKey>
		where TKey : class, INativeObject
	{
		public NSMutableSet ()
		{
		}

		public NSMutableSet (NSCoder coder)
			: base (coder)
		{
		}

		internal NSMutableSet (IntPtr handle)
			: base (handle)
		{
		}

		public NSMutableSet (params TKey [] objs)
			: base (objs)
		{
		}

		public NSMutableSet (NSSet<TKey> other)
			: base (other)
		{
		}

		public NSMutableSet (NSMutableSet<TKey> other)
			: base (other)
		{
		}

		public NSMutableSet (nint capacity)
			: base (capacity)
		{
		}

		// Strongly typed versions of API from NSSet

		public TKey LookupMember (TKey probe)
		{
			if (probe == null)
				throw new ArgumentNullException (nameof (probe));

			return Runtime.GetINativeObject<TKey> (_LookupMember (probe.Handle), false);
		}

		public TKey AnyObject {
			get {
				return Runtime.GetINativeObject<TKey> (_AnyObject, false);
			}
		}

		public bool Contains (TKey obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof(obj));

			return _Contains (obj.Handle);
		}

		public TKey [] ToArray ()
		{
			return base.ToArray<TKey> ();
		}

		public static NSMutableSet<TKey> operator + (NSMutableSet<TKey> first, NSMutableSet<TKey> second)
		{
			if (first == null || first.Count == 0)
				return new NSMutableSet<TKey> (second);
			if (second == null || second.Count == 0)
				return new NSMutableSet<TKey> (first);
			return new NSMutableSet<TKey> (first._SetByAddingObjectsFromSet (second.Handle));
		}

		public static NSMutableSet<TKey> operator - (NSMutableSet<TKey> first, NSMutableSet<TKey> second)
		{
			if (first == null || first.Count == 0)
				return null;
			if (second == null || second.Count == 0)
				return new NSMutableSet<TKey> (first);
			var copy = new NSMutableSet<TKey> (first);
			copy.MinusSet (second);
			return copy;
		}

		// Strongly typed versions of API from NSMutableSet
		public void Add (TKey obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			_Add (obj.Handle);
		}

		public void Remove (TKey obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			_Remove (obj.Handle);
		}

		public void AddObjects (params TKey [] objects)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));

			for (int i = 0; i < objects.Length; i++)
				if (objects [i] == null)
					throw new ArgumentNullException (nameof (objects) + "[" + i.ToString () + "]");

			using (var array = NSArray.From<TKey> (objects))
				_AddObjects (array.Handle);
		}

#region IEnumerable<T> implementation
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
	}
}

#endif // XAMCORE_2_0
