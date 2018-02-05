// Copyright 2015 Xamarin Inc
//
// This file contains a generic version of NSMutableArray.
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

#if XAMCORE_2_0

using System;
using System.Collections.Generic;

using ObjCRuntime;

namespace Foundation {

	[Register ("NSMutableArray", SkipRegistration = true)]
	public sealed partial class NSMutableArray<TValue> : NSMutableArray, IEnumerable<TValue>
		where TValue : class, INativeObject
	{
		public NSMutableArray ()
		{
		}

		public NSMutableArray (NSCoder coder)
			: base (coder)
		{
		}

		internal NSMutableArray (IntPtr handle)
			: base (handle)
		{
		}

		public NSMutableArray (nuint capacity)
			: base (capacity)
		{
		}

		public NSMutableArray (params TValue [] values)
		{
			if (values == null)
				throw new ArgumentNullException (nameof (values));
			
			for (int i = 0; i < values.Length; i++)
				Add (values [i]);
		}

		// Strongly typed methods from NSArray
		public bool Contains (TValue obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			return _Contains (obj.Handle);
		}

		public nuint IndexOf (TValue obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			return _IndexOf (obj.Handle);
		}

		// Strongly typed methods from NSMutableArray
		public void Add (TValue obj)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			_Add (obj.Handle);
		}

		public void Insert (TValue obj, nint index)
		{
			if (obj == null)
				throw new ArgumentNullException (nameof (obj));

			ValidateIndex (index);

			_Insert (obj.Handle, index);
		}

		public void ReplaceObject (nint index, TValue withObject)
		{
			if (withObject == null)
				throw new ArgumentNullException (nameof (withObject));

			ValidateIndex (index);

			_ReplaceObject (index, withObject.Handle);
		}

		public void AddObjects (params TValue[] source)
		{
			if (source == null)
				throw new ArgumentNullException (nameof (source));

			for (int i = 0; i < source.Length; i++)
				if (source [i] == null)
					throw new ArgumentNullException (nameof (source) + "[" + i.ToString () + "]");

			for (int i = 0; i < source.Length; i++)
				_Add (source [i].Handle);
		}

		public void InsertObjects (TValue[] objects, NSIndexSet atIndexes)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));

			if (atIndexes == null)
				throw new ArgumentNullException (nameof (atIndexes));

			if (objects.Length != atIndexes.Count)
				throw new ArgumentOutOfRangeException ("'" + nameof (objects) + "' and '" + nameof (atIndexes) + "' must contain the same number of elements");

			for (int i = 0; i < objects.Length; i++)
				if (objects [i] == null)
					throw new ArgumentNullException (nameof (objects) + "[" + i.ToString () + "]");

			nuint idx = atIndexes.FirstIndex;
			for (int i = 0; i < objects.Length; i++) {
				if (i > 0)
					idx = atIndexes.IndexGreaterThan (idx);

				Insert (objects [i], (nint) idx);
			}
		}

		// Additional implementations.

		public TValue this [nuint index] {
			get {
				ValidateIndex (index);
				return GetItem<TValue> (index);
			}
			set {
				if (value == null)
					throw new ArgumentNullException (nameof (value));
				ValidateIndex (index);
				_ReplaceObject ((nint) index, value.Handle);
			}
		}

		void ValidateIndex (nint index)
		{
			if (index < 0)
				throw new IndexOutOfRangeException (nameof (index));

			if ((nuint) index >= Count)
				throw new IndexOutOfRangeException (nameof (index));
		}

		void ValidateIndex (nuint index)
		{
			if (index >= Count)
				throw new IndexOutOfRangeException (nameof (index));
		}

#region IEnumerable<T> implementation
		public IEnumerator<TValue> GetEnumerator ()
		{
			return new NSFastEnumerator<TValue> (this);
		}
#endregion

#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
#endregion
	}
}

#endif // XAMCORE_2_0
