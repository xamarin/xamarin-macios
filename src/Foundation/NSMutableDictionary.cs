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
// Copyright 2011 - 2014 Xamarin Inc (http://www.xamarin.com)
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using CoreFoundation;

#nullable enable

namespace Foundation {

	public partial class NSMutableDictionary : NSDictionary, IDictionary, IDictionary<NSObject, NSObject> {
		/// <summary>Creates a mutable dictionary from the specified arrays of objects and keys.</summary>
		/// <param name="objects">The array of objects to add to the dictionary.</param>
		/// <param name="keys">The array of keys for the objects.</param>
		/// <returns>A new mutable dictionary containing the specified objects and keys.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="objects"/> or <paramref name="keys"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown when the arrays have different sizes.</exception>
		public static NSMutableDictionary FromObjectsAndKeys (NSObject? [] objects, NSObject? [] keys)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys))
				return new NSMutableDictionary ();

			return FromObjectsAndKeys (objects, keys, objects.Length);
		}

		/// <summary>Creates a mutable dictionary from the specified arrays of objects and keys.</summary>
		/// <param name="objects">The array of objects to add to the dictionary.</param>
		/// <param name="keys">The array of keys for the objects.</param>
		/// <returns>A new mutable dictionary containing the specified objects and keys.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="objects"/> or <paramref name="keys"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown when the arrays have different sizes.</exception>
		public static NSMutableDictionary FromObjectsAndKeys (object [] objects, object [] keys)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys))
				return new NSMutableDictionary ();

			return FromObjectsAndKeys (objects, keys, objects.Length);
		}

		/// <summary>Creates a mutable dictionary from the specified number of objects and keys from the arrays.</summary>
		/// <param name="objects">The array of objects to add to the dictionary.</param>
		/// <param name="keys">The array of keys for the objects.</param>
		/// <param name="count">The number of elements to copy from the arrays.</param>
		/// <returns>A new mutable dictionary containing the specified objects and keys.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="objects"/> or <paramref name="keys"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="count"/> is invalid.</exception>
		public static NSMutableDictionary FromObjectsAndKeys (NSObject? [] objects, NSObject? [] keys, nint count)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys, count))
				return new NSMutableDictionary ();

			using (var no = NSArray.FromNativeObjects (objects, count))
			using (var nk = NSArray.FromNativeObjects (keys, count))
				return FromObjectsAndKeysInternal (no, nk);
		}

		/// <summary>Creates a mutable dictionary from the specified number of objects and keys from the arrays.</summary>
		/// <param name="objects">The array of objects to add to the dictionary.</param>
		/// <param name="keys">The array of keys for the objects.</param>
		/// <param name="count">The number of elements to copy from the arrays.</param>
		/// <returns>A new mutable dictionary containing the specified objects and keys.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="objects"/> or <paramref name="keys"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown when <paramref name="count"/> is invalid.</exception>
		public static NSMutableDictionary FromObjectsAndKeys (object [] objects, object [] keys, nint count)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys, count))
				return new NSMutableDictionary ();

			using (var no = NSArray.FromObjects (count, objects))
			using (var nk = NSArray.FromObjects (count, keys))
				return FromObjectsAndKeysInternal (no, nk);
		}

		#region ICollection<KeyValuePair<NSObject, NSObject>>
		void ICollection<KeyValuePair<NSObject, NSObject>>.Add (KeyValuePair<NSObject, NSObject> item)
		{
			SetObject (item.Value, item.Key);
		}

		/// <summary>Removes all objects from the dictionary.</summary>
		public void Clear ()
		{
			RemoveAllObjects ();
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.Contains (KeyValuePair<NSObject, NSObject> keyValuePair)
		{
			return ContainsKeyValuePair (keyValuePair);
		}

		void ICollection<KeyValuePair<NSObject, NSObject>>.CopyTo (KeyValuePair<NSObject, NSObject> [] array, int index)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			if (index < 0)
				throw new ArgumentOutOfRangeException (nameof (index));
			// we want no exception for index==array.Length && Count == 0
			if (index > array.Length)
				throw new ArgumentException (nameof (index) + " larger than largest valid index of array");
			if (array.Length - index < (int) Count)
				throw new ArgumentException ("Destination array cannot hold the requested elements!");

			var e = GetEnumerator ();
			while (e.MoveNext ())
				array [index++] = e.Current;
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.Remove (KeyValuePair<NSObject, NSObject> keyValuePair)
		{
			var count = Count;
			RemoveObjectForKey (keyValuePair.Key);
			return count != Count;
		}

		int ICollection<KeyValuePair<NSObject, NSObject>>.Count {
			get { return (int) Count; }
		}

		bool ICollection<KeyValuePair<NSObject, NSObject>>.IsReadOnly {
			get { return false; }
		}
		#endregion

		#region IDictionary
		/// <summary>Adds an element with the provided key and value to the dictionary.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add.</param>
		/// <exception cref="ArgumentException">Thrown when the key or value is not an <see cref="NSObject"/>.</exception>
		void IDictionary.Add (object key, object? value)
		{
			var nsokey = key as NSObject;
			var nsovalue = value as NSObject;

			if (nsokey is null || nsovalue is null)
				throw new ArgumentException ("You can only use NSObjects for keys and values in an NSMutableDictionary");

			// Inverted args
			SetObject (nsovalue, nsokey);
		}

		/// <summary>Determines whether the dictionary contains an element with the specified key.</summary>
		/// <param name="key">The key to locate in the dictionary.</param>
		/// <returns><see langword="true"/> if the dictionary contains an element with the key; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
		bool IDictionary.Contains (object key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));
			var _key = key as INativeObject;
			if (_key is null)
				return false;
			bool result = ContainsKey (_key.Handle);
			GC.KeepAlive (_key);
			return result;
		}

		/// <summary>Returns an enumerator that iterates through the dictionary.</summary>
		/// <returns>An <see cref="IDictionaryEnumerator"/> for the dictionary.</returns>
		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return (IDictionaryEnumerator) ((IEnumerable<KeyValuePair<NSObject, NSObject>>) this).GetEnumerator ();
		}

		/// <summary>Removes the element with the specified key from the dictionary.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown when the key is not an <see cref="INativeObject"/>.</exception>
		void IDictionary.Remove (object key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));
			var nskey = key as INativeObject;
			if (nskey is null)
				throw new ArgumentException ("The key must be an INativeObject");

			_RemoveObjectForKey (nskey.Handle);
			GC.KeepAlive (nskey);
		}

		/// <summary>Gets a value indicating whether the dictionary has a fixed size.</summary>
		/// <value><see langword="false"/> since the dictionary is mutable.</value>
		bool IDictionary.IsFixedSize {
			get { return false; }
		}

		/// <summary>Gets a value indicating whether the dictionary is read-only.</summary>
		/// <value><see langword="false"/> since the dictionary is mutable.</value>
		bool IDictionary.IsReadOnly {
			get { return false; }
		}

		/// <summary>Gets or sets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get or set.</param>
		/// <value>The object associated with the specified key.</value>
		/// <exception cref="ArgumentException">Thrown when setting a value and the key or value is not an <see cref="INativeObject"/>.</exception>
		object? IDictionary.this [object key] {
			get {
				var _key = key as INativeObject;
				if (_key is null)
					return null;
				object? result = _ObjectForKey (_key.Handle);
				GC.KeepAlive (_key);
				return result;
			}
			set {
				var nsokey = key as INativeObject;
				var nsovalue = value as INativeObject;

				if (nsokey is null || nsovalue is null)
					throw new ArgumentException ("You can only use INativeObjects for keys and values in an NSMutableDictionary");

				_SetObject (nsovalue.Handle, nsokey.Handle);
				GC.KeepAlive (nsovalue);
				GC.KeepAlive (nsokey);
			}
		}

		/// <summary>Gets an <see cref="ICollection"/> containing the keys of the dictionary.</summary>
		/// <value>An <see cref="ICollection"/> containing the keys of the dictionary.</value>
		ICollection IDictionary.Keys {
			get { return Keys; }
		}

		/// <summary>Gets an <see cref="ICollection"/> containing the values in the dictionary.</summary>
		/// <value>An <see cref="ICollection"/> containing the values in the dictionary.</value>
		ICollection IDictionary.Values {
			get { return Values; }
		}
		#endregion

		#region IDictionary<NSObject, NSObject>
		/// <summary>Adds an element with the provided key and value to the dictionary.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add.</param>
		public void Add (NSObject key, NSObject value)
		{
			// Inverted args.
			SetObject (value, key);
		}

		/// <summary>Removes the element with the specified key from the dictionary.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
		public bool Remove (NSObject key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			var last = Count;
			RemoveObjectForKey (key);
			return last != Count;
		}

		/// <summary>Gets the value associated with the specified key.</summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true"/> if the dictionary contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
#pragma warning disable CS8767 // Nullability of reference types in type of parameter 'value' of 'bool NSMutableDictionary.TryGetValue(NSObject key, out NSObject? value)' doesn't match implicitly implemented member 'bool IDictionary<NSObject, NSObject>.TryGetValue(NSObject key, out NSObject value)' (possibly because of nullability attributes).
		public bool TryGetValue (NSObject key, [NotNullWhen (true)] out NSObject? value)
#pragma warning restore CS8767
		{
			// Can't put null in NSDictionaries, so if null is returned, the key wasn't found.
			return (value = ObjectForKey (key)) is not null;
		}

		/// <summary>Gets or sets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get or set.</param>
		/// <value>The object associated with the specified key.</value>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public override NSObject? this [NSObject key] {
#pragma warning disable CS8766 // Nullability of reference types in return type of 'NSObject? NSMutableDictionary.this[NSObject key].get' doesn't match implicitly implemented member 'NSObject IDictionary<NSObject, NSObject>.this[NSObject key].get'
			get {
#pragma warning restore CS8766
				return ObjectForKey (key);
			}
			set {
				SetObject (value, key);
			}
		}

		/// <summary>Gets or sets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get or set.</param>
		/// <value>The object associated with the specified key.</value>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public override NSObject? this [NSString key] {
			get {
				return ObjectForKey (key);
			}
			set {
				SetObject (value, key);
			}
		}

		/// <summary>Gets or sets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get or set.</param>
		/// <value>The object associated with the specified key.</value>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public override NSObject? this [string key] {
			get {
				if (key is null)
					throw new ArgumentNullException (nameof (key));
				var nss = NSString.CreateNative (key, false);
				try {
					return Runtime.GetNSObject (LowlevelObjectForKey (nss));
				} finally {
					NSString.ReleaseNative (nss);
				}
			}
			set {
				if (key is null)
					throw new ArgumentNullException (nameof (key));
				var nss = NSString.CreateNative (key, false);
				try {
					LowlevelSetObject (value, nss);
				} finally {
					NSString.ReleaseNative (nss);
				}
			}
		}

		/// <summary>Gets an <see cref="ICollection{T}"/> containing the keys of the dictionary.</summary>
		/// <value>An <see cref="ICollection{T}"/> containing the keys of the dictionary.</value>
		ICollection<NSObject> IDictionary<NSObject, NSObject>.Keys {
			get { return Keys; }
		}

		/// <summary>Gets an <see cref="ICollection{T}"/> containing the values in the dictionary.</summary>
		/// <value>An <see cref="ICollection{T}"/> containing the values in the dictionary.</value>
		ICollection<NSObject> IDictionary<NSObject, NSObject>.Values {
			get { return Values; }
		}
		#endregion

		#region IEnumerable
		/// <summary>Returns an enumerator that iterates through the dictionary.</summary>
		/// <returns>An <see cref="IEnumerator"/> for the dictionary.</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<KeyValuePair<NSObject, NSObject>>) this).GetEnumerator ();
		}
		#endregion

		#region IEnumerable<K,V>
		/// <summary>Returns an enumerator that iterates through the dictionary.</summary>
		/// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
		public IEnumerator<KeyValuePair<NSObject, NSObject>> GetEnumerator ()
		{
			foreach (var key in Keys) {
				yield return new KeyValuePair<NSObject, NSObject> (key, this [key]!);
			}
		}
		#endregion

		/// <summary>Creates a mutable dictionary from a low-level object and key pointer.</summary>
		/// <param name="obj">The object pointer.</param>
		/// <param name="key">The key pointer.</param>
		/// <returns>A new mutable dictionary containing the specified object and key.</returns>
		public static NSMutableDictionary LowlevelFromObjectAndKey (IntPtr obj, IntPtr key)
		{
#if MONOMAC
			return (NSMutableDictionary) Runtime.GetNSObject (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (class_ptr, selDictionaryWithObject_ForKey_XHandle, obj, key))!;
#else
			return (NSMutableDictionary) Runtime.GetNSObject (ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (class_ptr, Selector.GetHandle ("dictionaryWithObject:forKey:"), obj, key))!;
#endif
		}

		/// <summary>Sets an object for a key using low-level pointers.</summary>
		/// <param name="obj">The object pointer.</param>
		/// <param name="key">The key pointer.</param>
		public void LowlevelSetObject (IntPtr obj, IntPtr key)
		{
#if MONOMAC
			ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, selSetObject_ForKey_XHandle, obj, key);
#else
			ObjCRuntime.Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, Selector.GetHandle ("setObject:forKey:"), obj, key);
#endif
		}

		/// <summary>Sets an object for a key using a low-level key pointer.</summary>
		/// <param name="obj">The object to set.</param>
		/// <param name="key">The key pointer.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
		public void LowlevelSetObject (NSObject obj, IntPtr key)
		{
			if (obj is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (obj));

			LowlevelSetObject (obj.Handle, key);
			GC.KeepAlive (obj);
		}

		/// <summary>Sets an object for a key using a low-level key pointer.</summary>
		/// <param name="obj">The object to set.</param>
		/// <param name="key">The key pointer.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <see langword="null"/>.</exception>
		internal void LowlevelSetObject (NSString obj, IntPtr key)
		{
			if (obj is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (obj));

			LowlevelSetObject (obj.Handle, key);
			GC.KeepAlive (obj);
		}

		/// <summary>Sets an object for a key using a low-level key pointer.</summary>
		/// <param name="obj">The object to set.</param>
		/// <param name="key">The key whose value to set.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> or <paramref name="key" /> is <see langword="null"/>.</exception>
		internal void LowlevelSetObject (INativeObject obj, INativeObject key)
		{
			if (obj is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (obj));

			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			LowlevelSetObject (obj.Handle, key.Handle);
			GC.KeepAlive (obj);
			GC.KeepAlive (key);
		}

		/// <summary>Sets an object for a key using a low-level key pointer.</summary>
		/// <param name="obj">The object to set.</param>
		/// <param name="key">The key whose value to set.</param>
		/// <exception cref="ArgumentNullException">Thrown when  <paramref name="key" /> is <see langword="null"/>.</exception>
		internal void LowlevelSetObject (IntPtr obj, INativeObject key)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			LowlevelSetObject (obj, key.Handle);
			GC.KeepAlive (key);
		}

		/// <summary>Sets a string value for a key using a low-level key pointer.</summary>
		/// <param name="str">The string to set.</param>
		/// <param name="key">The key pointer.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is <see langword="null"/>.</exception>
		public void LowlevelSetObject (string str, IntPtr key)
		{
			if (str is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (str));

			var ptr = CFString.CreateNative (str);
			LowlevelSetObject (ptr, key);
			CFString.ReleaseNative (ptr);
		}
	}
}
