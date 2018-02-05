//
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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
using System.Collections;
using System.Collections.Generic;
using ObjCRuntime;

namespace Foundation {

	public partial class NSDictionary : NSObject, IDictionary, IDictionary<NSObject, NSObject> {
		public NSDictionary (NSObject first, NSObject second, params NSObject [] args) : this (PickOdd (second, args), PickEven (first, args))
		{
		}

		public NSDictionary (object first, object second, params object [] args) : this (PickOdd (second, args), PickEven (first, args))
		{
		}

		internal NSDictionary (IntPtr handle, bool alloced) : base (handle, alloced)
		{
		}
		
		internal static NSArray PickEven (NSObject f, NSObject [] args)
		{
			int al = args.Length;
			if ((al % 2) != 0)
				throw new ArgumentException ("The arguments to NSDictionary should be a multiple of two", "args");
			var ret = new NSObject [1+al/2];
			ret [0] = f;
			for (int i = 0, target = 1; i < al; i += 2)
				ret [target++] = args [i];
			return NSArray.FromNSObjects (ret);
		}

		internal static NSArray PickOdd (NSObject f, NSObject [] args)
		{
			var ret = new NSObject [1+args.Length/2];
			ret [0] = f;
			for (int i = 1, target = 1; i < args.Length; i += 2)
				ret [target++] = args [i];
			return NSArray.FromNSObjects (ret);
		}

		internal static NSArray PickEven (object f, object [] args)
		{
			int al = args.Length;
			if ((al % 2) != 0)
				throw new ArgumentException ("The arguments to NSDictionary should be a multiple of two", "args");
			var ret = new object [1+al/2];
			ret [0] = f;
			for (int i = 0, target = 1; i < al; i += 2)
				ret [target++] = args [i];
			return NSArray.FromObjects (ret);
		}

		internal static NSArray PickOdd (object f, object [] args)
		{
			var ret = new object [1+args.Length/2];
			ret [0] = f;
			for (int i = 1, target = 1; i < args.Length; i += 2)
				ret [target++] = args [i];
			return NSArray.FromObjects (ret);
		}
		
		public static NSDictionary FromObjectsAndKeys (NSObject [] objects, NSObject [] keys)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));
			if (keys == null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			
			using (var no = NSArray.FromNSObjects (objects))
				using (var nk = NSArray.FromNSObjects (keys))
					return FromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary FromObjectsAndKeys (object [] objects, object [] keys)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));
			if (keys == null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			
			using (var no = NSArray.FromObjects (objects))
				using (var nk = NSArray.FromObjects (keys))
					return FromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary FromObjectsAndKeys (NSObject [] objects, NSObject [] keys, nint count)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));
			if (keys == null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			if (count < 1 || objects.Length < count || keys.Length < count)
				throw new ArgumentException ("count");
			
			using (var no = NSArray.FromNativeObjects (objects, count))
				using (var nk = NSArray.FromNativeObjects (keys, count))
					return FromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary FromObjectsAndKeys (object [] objects, object [] keys, nint count)
		{
			if (objects == null)
				throw new ArgumentNullException (nameof (objects));
			if (keys == null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			if (count < 1 || objects.Length < count || keys.Length < count)
				throw new ArgumentException ("count");
			
			using (var no = NSArray.FromObjects (count, objects))
				using (var nk = NSArray.FromObjects (count, keys))
					return FromObjectsAndKeysInternal (no, nk);
		}
		
		internal bool ContainsKeyValuePair (KeyValuePair<NSObject, NSObject> pair)
		{
			NSObject value;
			if (!TryGetValue (pair.Key, out value))
				return false;

			return EqualityComparer<NSObject>.Default.Equals (pair.Value, value);
		}

		#region ICollection
		void ICollection.CopyTo (Array array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException (nameof (arrayIndex));
			if (array.Rank > 1)
				throw new ArgumentException (nameof (array) + " is multidimensional");
			if ((array.Length > 0) && (arrayIndex >= array.Length))
				throw new ArgumentException (nameof (arrayIndex) + " is equal to or greater than " + nameof (array) + ".Length");
			if (arrayIndex + (int)Count > array.Length)
				throw new ArgumentException ("Not enough room from " + nameof (arrayIndex) + " to end of " + nameof (array) + " for this Hashtable");
			IDictionaryEnumerator e = ((IDictionary) this).GetEnumerator ();
			int i = arrayIndex;
			while (e.MoveNext ())
				array.SetValue (e.Entry, i++);
		}

		int ICollection.Count {
			get {return (int) Count;}
		}

		bool ICollection.IsSynchronized {
			get {return false;}
		}

		object ICollection.SyncRoot {
			get {return this;}
		}
		#endregion

		#region ICollection<KeyValuePair<NSObject, NSObject>>
		void ICollection<KeyValuePair<NSObject, NSObject>>.Add (KeyValuePair<NSObject, NSObject> item)
		{
			throw new NotSupportedException ();
		}

		void ICollection<KeyValuePair<NSObject, NSObject>>.Clear ()
		{
			throw new NotSupportedException ();
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.Contains (KeyValuePair<NSObject, NSObject> keyValuePair)
		{
			return ContainsKeyValuePair (keyValuePair);
		}

		void ICollection<KeyValuePair<NSObject, NSObject>>.CopyTo (KeyValuePair<NSObject, NSObject>[] array, int index)
		{
			if (array == null)
				throw new ArgumentNullException (nameof (array));
			if (index < 0)
				throw new ArgumentOutOfRangeException (nameof (index));
			// we want no exception for index==array.Length && Count == 0
			if (index > array.Length)
				throw new ArgumentException (nameof (index) + " larger than largest valid index of " + nameof (array));
			if (array.Length - index < (int)Count)
				throw new ArgumentException ("Destination array cannot hold the requested elements!");

			var e = GetEnumerator ();
			while (e.MoveNext ())
				array [index++] = e.Current;
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.Remove (KeyValuePair<NSObject, NSObject> keyValuePair)
		{
			throw new NotSupportedException ();
		}

		int ICollection<KeyValuePair<NSObject, NSObject>>.Count {
			get { return (int) Count; }
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.IsReadOnly {
			get { return true; }
		}
		#endregion

		#region IDictionary

		void IDictionary.Add (object key, object value)
		{
			throw new NotSupportedException ();
		}

		void IDictionary.Clear ()
		{
			throw new NotSupportedException ();
		}

		bool IDictionary.Contains (object key)
		{
			if (key == null)
				throw new ArgumentNullException (nameof (key));
			NSObject _key = key as NSObject;
			if (_key == null)
				return false;
			return ContainsKey (_key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return (IDictionaryEnumerator) ((IEnumerable<KeyValuePair<NSObject, NSObject>>) this).GetEnumerator ();
		}

		void IDictionary.Remove (object key)
		{
			throw new NotSupportedException ();
		}

		bool IDictionary.IsFixedSize {
			get { return true; }
		}

		bool IDictionary.IsReadOnly {
			get { return true; }
		}

		object IDictionary.this [object key] {
			get {
				NSObject _key = key as NSObject;
				if (_key == null)
					return null;
				return ObjectForKey (_key);
			}
			set {
				throw new NotSupportedException ();
			}
		}

		ICollection IDictionary.Keys {
			get { return Keys; }
		}

		ICollection IDictionary.Values {
			get { return Values; }
		}

		#endregion

		#region IDictionary<NSObject, NSObject>

		void IDictionary<NSObject, NSObject>.Add (NSObject key, NSObject value)
		{
			throw new NotSupportedException ();
		}

		public bool ContainsKey (NSObject key)
		{
			return ObjectForKey (key) != null;
		}

		internal bool ContainsKey (IntPtr key)
		{
			return LowlevelObjectForKey (key) != IntPtr.Zero;
		}

		bool IDictionary<NSObject, NSObject>.Remove (NSObject key)
		{
			throw new NotSupportedException ();
		}

		internal bool TryGetValue<T> (INativeObject key, out T value) where T: class, INativeObject
		{
			if (key == null)
				throw new ArgumentNullException (nameof (key));

			value = null;

			var ptr = _ObjectForKey (key.Handle);
			if (ptr == IntPtr.Zero)
				return false;

			value = Runtime.GetINativeObject<T> (ptr, false);
			return true;
		}

		public bool TryGetValue (NSObject key, out NSObject value)
		{
			value = ObjectForKey (key);
			// NSDictionary can not contain NULLs, if you want a NULL, it exists as an NSNull
			return value != null;
		}

		public virtual NSObject this [NSObject key] {
			get {
				return ObjectForKey (key);
			}
			set {
				throw new NotSupportedException ();
			}
		}

		public virtual NSObject this [NSString key] {
			get {
				return ObjectForKey (key);
			}
			set {
				throw new NotSupportedException ();
			}
		}

		public virtual NSObject this [string key] {
			get {
				if (key == null)
					throw new ArgumentNullException ("key");
				using (var nss = new NSString (key)){
					return ObjectForKey (nss);
				}
			}
			set {
				throw new NotSupportedException ();
			}
		}

		ICollection<NSObject> IDictionary<NSObject, NSObject>.Keys {
			get { return Keys; }
		}

		ICollection<NSObject> IDictionary<NSObject, NSObject>.Values {
			get { return Values; }
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<KeyValuePair<NSObject, NSObject>> GetEnumerator ()
		{
			foreach (var key in Keys) {
				yield return new KeyValuePair<NSObject, NSObject> (key, ObjectForKey (key));
			}
		}

		public IntPtr LowlevelObjectForKey (IntPtr key)
		{
#if MONOMAC
			return ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selObjectForKey_Handle, key);
#else
			return ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("objectForKey:"), key);
#endif
		}

		public NSFileAttributes ToFileAttributes ()
		{
			return NSFileAttributes.FromDictionary (this);
		}
	}
}
