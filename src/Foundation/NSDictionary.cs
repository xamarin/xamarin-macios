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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Foundation {

	public partial class NSDictionary : NSObject, IDictionary, IDictionary<NSObject, NSObject> {
		/// <summary>
		/// Creates an <see cref="NSDictionary"/> from a list of <see cref="NSObject"/> keys and <see cref="NSObject"/> values.
		/// </summary>
		/// <param name="first">First key.</param>
		/// <param name="second">First value.</param>
		/// <param name="args">Remaining pairs of keys and values.</param>
		/// <remarks>
		/// <para>
		/// The list of keys and values are used to create the dictionary. The number of parameters passed to this function must be even.
		/// </para>
		/// <example>
		/// <code lang="csharp lang-csharp"><![CDATA[
		/// var key1 = new NSString ("key1");
		/// var value1 = new NSNumber ((byte) 1);
		/// var key2 = new NSString ("key2");
		/// var value2 = new NSNumber ((byte) 2);
		///
		/// var dict2 = new NSDictionary (key1, value1, key2, value2);
		/// ]]></code>
		/// </example>
		/// </remarks>
		public NSDictionary (NSObject? first, NSObject? second, params NSObject? [] args) : this (PickOdd (second, args), PickEven (first, args))
		{
		}

		/// <summary>
		/// Creates an <see cref="NSDictionary"/> from a list of keys and values.
		/// </summary>
		/// <param name="first">First key.</param>
		/// <param name="second">First value.</param>
		/// <param name="args">Remaining pairs of keys and values.</param>
		/// <remarks>
		/// <para>
		/// Each C# object is boxed as an <see cref="NSObject"/> by calling <see cref="NSObject.FromObject(object)"/>.
		/// </para>
		/// <para>
		/// The list of keys and values are used to create the dictionary. The number of parameters passed to this function must be even.
		/// </para>
		/// <example>
		/// <code lang="csharp lang-csharp"><![CDATA[
		/// //
		/// // Using C# objects, strings and ints, produces
		/// // a dictionary with 2 NSString keys, "key1" and "key2"
		/// // and two NSNumbers with the values 1 and 2
		/// //
		/// var dict = new NSDictionary ("key1", 1, "key2", 2);
		/// ]]></code>
		/// </example>
		/// </remarks>
		public NSDictionary (object first, object second, params object [] args) : this (PickOdd (second, args), PickEven (first, args))
		{
		}

		internal NSDictionary (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

		internal static NSArray PickEven (NSObject? f, NSObject? [] args)
		{
			int al = args.Length;
			if ((al % 2) != 0)
				throw new ArgumentException ("The arguments to NSDictionary should be a multiple of two", "args");
			var ret = new NSObject? [1 + al / 2];
			ret [0] = f;
			for (int i = 0, target = 1; i < al; i += 2)
				ret [target++] = args [i];
			return NSArray.FromNSObjects (ret);
		}

		internal static NSArray PickOdd (NSObject? f, NSObject? [] args)
		{
			var ret = new NSObject? [1 + args.Length / 2];
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
			var ret = new object [1 + al / 2];
			ret [0] = f;
			for (int i = 0, target = 1; i < al; i += 2)
				ret [target++] = args [i];
			return NSArray.FromObjects (ret);
		}

		internal static NSArray PickOdd (object f, object [] args)
		{
			var ret = new object [1 + args.Length / 2];
			ret [0] = f;
			for (int i = 1, target = 1; i < args.Length; i += 2)
				ret [target++] = args [i];
			return NSArray.FromObjects (ret);
		}

		// Checks:
		// * 'objects' and 'keys' for null
		// * count isn't negative
		// * count isn't higher than the number of elements in either array
		// returns false if an empty dictionary can be returned
		private protected static bool ValidateFromObjectsAndKeys<T, K> (T [] objects, K [] keys, nint count)
		{
			ArgumentNullException.ThrowIfNull (objects);
			ArgumentNullException.ThrowIfNull (keys);

			if (count < 0)
				throw new ArgumentOutOfRangeException (nameof (count), "Must be non-negative and not greater than the length of either array");

			if (objects.Length < count || keys.Length < count)
				throw new ArgumentException ("Must be non-negative and not greater than the length of either array", nameof (count));

			return count > 0;
		}

		// Checks:
		// * 'objects' and 'keys' for null
		// * 'objects' and 'keys' have the same number of elements
		// returns false if an empty dictionary can be returned
		private protected static bool ValidateFromObjectsAndKeys<T, K> (T [] objects, K [] keys)
		{
			ArgumentNullException.ThrowIfNull (objects);
			ArgumentNullException.ThrowIfNull (keys);

			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");

			return objects.Length > 0;
		}

		/// <summary>
		/// Creates a dictionary from a set of values and keys.
		/// </summary>
		/// <param name="objects">Array of values for the dictionary. Null elements are stored as <see cref="NSNull.Null"/>.</param>
		/// <param name="keys">Array of keys for the dictionary.</param>
		/// <returns>A new <see cref="NSDictionary"/> containing the specified key-value pairs.</returns>
		public static NSDictionary FromObjectsAndKeys (NSObject? [] objects, NSObject [] keys)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys))
				return new NSDictionary ();

			return FromObjectsAndKeys (objects, keys, objects.Length);
		}

		/// <summary>
		/// Creates a dictionary from a set of values and keys.
		/// </summary>
		/// <param name="objects">Array of values for the dictionary.</param>
		/// <param name="keys">Array of keys for the dictionary.</param>
		/// <returns>A new <see cref="NSDictionary"/> containing the specified key-value pairs.</returns>
		/// <remarks>
		/// <para>
		/// The keys and values will first be boxed into <see cref="NSObject"/>s using <see cref="NSObject.FromObject(object)"/>.
		/// </para>
		/// </remarks>
		public static NSDictionary FromObjectsAndKeys (object [] objects, object [] keys)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys))
				return new NSDictionary ();

			return FromObjectsAndKeys (objects, keys, objects.Length);
		}

		/// <summary>
		/// Creates a dictionary from a set of values and keys.
		/// </summary>
		/// <param name="objects">Array of values for the dictionary. Null elements are stored as <see cref="NSNull.Null"/>.</param>
		/// <param name="keys">Array of keys for the dictionary.</param>
		/// <param name="count">Number of items to use in the creation; the number must be less than or equal to the number of elements in both arrays.</param>
		/// <returns>A new <see cref="NSDictionary"/> containing the specified key-value pairs.</returns>
		public static NSDictionary FromObjectsAndKeys (NSObject? [] objects, NSObject [] keys, nint count)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys, count))
				return new NSDictionary ();

			using (var no = NSArray.FromNativeObjects (objects, count))
			using (var nk = NSArray.FromNativeObjects (keys, count))
				return FromObjectsAndKeysInternal (no, nk);
		}

		/// <summary>
		/// Creates a dictionary from a set of values and keys.
		/// </summary>
		/// <param name="objects">Array of values for the dictionary.</param>
		/// <param name="keys">Array of keys for the dictionary.</param>
		/// <param name="count">Number of items to use in the creation; the number must be less than or equal to the number of elements in both arrays.</param>
		/// <returns>A new <see cref="NSDictionary"/> containing the specified key-value pairs.</returns>
		/// <remarks>
		/// <para>
		/// The keys and values will first be boxed into <see cref="NSObject"/>s using <see cref="NSObject.FromObject(object)"/>.
		/// </para>
		/// </remarks>
		public static NSDictionary FromObjectsAndKeys (object [] objects, object [] keys, nint count)
		{
			if (!ValidateFromObjectsAndKeys (objects, keys, count))
				return new NSDictionary ();

			using (var no = NSArray.FromObjects (count, objects))
			using (var nk = NSArray.FromObjects (count, keys))
				return FromObjectsAndKeysInternal (no, nk);
		}

		internal bool ContainsKeyValuePair (KeyValuePair<NSObject, NSObject> pair)
		{
			if (!TryGetValue (pair.Key, out var value))
				return false;

			return EqualityComparer<NSObject>.Default.Equals (pair.Value, value);
		}

		#region ICollection
		/// <summary>
		/// Copies the elements of the dictionary to an <see cref="Array"/>, starting at a particular index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		void ICollection.CopyTo (Array array, int arrayIndex)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException (nameof (arrayIndex));
			if (array.Rank > 1)
				throw new ArgumentException (nameof (array) + " is multidimensional");
			if ((array.Length > 0) && (arrayIndex >= array.Length))
				throw new ArgumentException (nameof (arrayIndex) + " is equal to or greater than " + nameof (array) + ".Length");
			if (arrayIndex + (int) Count > array.Length)
				throw new ArgumentException ("Not enough room from " + nameof (arrayIndex) + " to end of " + nameof (array) + " for this Hashtable");
			IDictionaryEnumerator e = ((IDictionary) this).GetEnumerator ();
			int i = arrayIndex;
			while (e.MoveNext ())
				array.SetValue (e.Entry, i++);
		}

		/// <summary>
		/// Gets the number of elements contained in the dictionary.
		/// </summary>
		/// <value>The number of elements contained in the dictionary.</value>
		int ICollection.Count {
			get { return (int) Count; }
		}

		/// <summary>
		/// Gets a value indicating whether access to the dictionary is synchronized (thread safe).
		/// </summary>
		/// <value>Always returns <see langword="false"/>.</value>
		bool ICollection.IsSynchronized {
			get { return false; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the dictionary.
		/// </summary>
		/// <value>An object that can be used to synchronize access to the dictionary.</value>
		object ICollection.SyncRoot {
			get { return this; }
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

		void ICollection<KeyValuePair<NSObject, NSObject>>.CopyTo (KeyValuePair<NSObject, NSObject> [] array, int index)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			if (index < 0)
				throw new ArgumentOutOfRangeException (nameof (index));
			// we want no exception for index==array.Length && Count == 0
			if (index > array.Length)
				throw new ArgumentException (nameof (index) + " larger than largest valid index of " + nameof (array));
			if (array.Length - index < (int) Count)
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

		/// <summary>
		/// Adds an element with the provided key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add.</param>
		/// <exception cref="NotSupportedException">Always thrown as the dictionary is read-only.</exception>
		void IDictionary.Add (object key, object? value)
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		/// Removes all elements from the dictionary.
		/// </summary>
		/// <exception cref="NotSupportedException">Always thrown as the dictionary is read-only.</exception>
		void IDictionary.Clear ()
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		/// Determines whether the dictionary contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the dictionary.</param>
		/// <returns><see langword="true"/> if the dictionary contains an element with the key; otherwise, <see langword="false"/>.</returns>
		bool IDictionary.Contains (object key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));
			var _key = key as NSObject;
			if (_key is null)
				return false;
			return ContainsKey (_key);
		}

		/// <summary>
		/// Returns an <see cref="IDictionaryEnumerator"/> for the dictionary.
		/// </summary>
		/// <returns>An <see cref="IDictionaryEnumerator"/> for the dictionary.</returns>
		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return (IDictionaryEnumerator) ((IEnumerable<KeyValuePair<NSObject, NSObject>>) this).GetEnumerator ();
		}

		/// <summary>
		/// Removes the element with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="NotSupportedException">Always thrown as the dictionary is read-only.</exception>
		void IDictionary.Remove (object key)
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		/// Gets a value indicating whether the dictionary has a fixed size.
		/// </summary>
		/// <value>Always returns <see langword="true"/>.</value>
		bool IDictionary.IsFixedSize {
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the dictionary is read-only.
		/// </summary>
		/// <value>Always returns <see langword="true"/>.</value>
		bool IDictionary.IsReadOnly {
			get { return true; }
		}

		[DisallowNull] // don't allow setting null values
		object? IDictionary.this [object key] {
			get {
				var _key = key as NSObject;
				if (_key is null)
					return null;
				return ObjectForKey (_key);
			}
#pragma warning disable CS8769 // Nullability of reference types in type of parameter 'value' doesn't match implemented member 'void IDictionary.this[object key].set'
			set {
#pragma warning restore CS8769
				throw new NotSupportedException ();
			}
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> containing the keys of the dictionary.
		/// </summary>
		/// <value>An <see cref="ICollection"/> containing the keys of the dictionary.</value>
		ICollection IDictionary.Keys {
			get { return Keys; }
		}

		/// <summary>
		/// Gets an <see cref="ICollection"/> containing the values in the dictionary.
		/// </summary>
		/// <value>An <see cref="ICollection"/> containing the values in the dictionary.</value>
		ICollection IDictionary.Values {
			get { return Values; }
		}

		#endregion

		#region IDictionary<NSObject, NSObject>

		void IDictionary<NSObject, NSObject>.Add (NSObject key, NSObject value)
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		/// Determines whether the specified key exists in the dictionary.
		/// </summary>
		/// <param name="key">Key to look up in the dictionary.</param>
		/// <returns><see langword="true"/> if the key exists in the dictionary; otherwise, <see langword="false"/>.</returns>
		public bool ContainsKey (NSObject key)
		{
			return ObjectForKey (key) is not null;
		}

		internal bool ContainsKey (IntPtr key)
		{
			return LowlevelObjectForKey (key) != IntPtr.Zero;
		}

		bool IDictionary<NSObject, NSObject>.Remove (NSObject key)
		{
			throw new NotSupportedException ();
		}

		internal bool TryGetValue<T> (INativeObject key, [NotNullWhen (true)] out T? value) where T : class, INativeObject
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			var ptr = _ObjectForKey (key.Handle);
			GC.KeepAlive (key);
			value = Runtime.GetINativeObject<T> (ptr, false);
			// NSDictionary can not contain NULLs, if you want a NULL, it exists as an NSNull
			return value is not null;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, <see langword="null"/>.</param>
		/// <returns><see langword="true"/> if the dictionary contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
		public bool TryGetValue (NSObject key, [NotNullWhen (true)] out NSObject? value)
		{
			return TryGetValue<NSObject> (key, out value);
		}

		/// <summary>Gets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get.</param>
		/// <value>The object associated with the specified key.</value>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public virtual NSObject? this [NSObject key] {
#pragma warning disable CS8766 // Nullability of reference types in return type of 'NSObject? NSDictionary.this[NSObject key].get' doesn't match implicitly implemented member 'NSObject IDictionary<NSObject, NSObject>.this[NSObject key].get'
			get {
#pragma warning restore CS8766
				return ObjectForKey (key);
			}
			set {
				throw new NotSupportedException ();
			}
		}

		/// <summary>Gets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get.</param>
		/// <value>The object associated with the specified key.</value>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public virtual NSObject? this [NSString key] {
			get {
				return ObjectForKey (key);
			}
			set {
				throw new NotSupportedException ();
			}
		}

		/// <summary>Gets the object associated with the specified key.</summary>
		/// <param name="key">The key of the object to get.</param>
		/// <value>The object associated with the specified key.</value>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
		/// <remarks>Returns <see langword="null" /> if the key wasn't found.</remarks>
		[DisallowNull] // don't allow setting null values
		public virtual NSObject? this [string key] {
			get {
				if (key is null)
					throw new ArgumentNullException ("key");
				var nss = NSString.CreateNative (key, false);
				try {
					return Runtime.GetNSObject (LowlevelObjectForKey (nss));
				} finally {
					NSString.ReleaseNative (nss);
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

		/// <summary>
		/// Returns an enumerator that iterates through the dictionary.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		/// <summary>Returns an enumerator that iterates through the dictionary.</summary>
		/// <returns>An enumerator that can be used to iterate through the dictionary.</returns>
		public IEnumerator<KeyValuePair<NSObject, NSObject>> GetEnumerator ()
		{
			foreach (var key in Keys) {
				yield return new KeyValuePair<NSObject, NSObject> (key, ObjectForKey (key)!);
			}
		}

		/// <summary>
		/// Low-level key lookup.
		/// </summary>
		/// <param name="key">A handle to an <see cref="NSObject"/> that might be in the dictionary.</param>
		/// <returns>Handle to an object, or <see cref="IntPtr.Zero"/> if the key does not exist in the dictionary.</returns>
		/// <remarks>
		/// In some cases, where you might be iterating over a loop, or you have not surfaced a bound type,
		/// but you have the handle to the key, you can use <see cref="LowlevelObjectForKey(IntPtr)"/>
		/// which takes a handle for the key and returns a handle for the returned object.
		/// </remarks>
		public IntPtr LowlevelObjectForKey (IntPtr key)
		{
#if MONOMAC
			return ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, selObjectForKey_XHandle, key);
#else
			return ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("objectForKey:"), key);
#endif
		}

		/// <summary>Gets the pointer for a key.</summary>
		/// <param name="key">The key.</param>
		internal IntPtr LowlevelObjectForKey (INativeObject? key)
		{
			var rv = LowlevelObjectForKey (key.GetHandle ());
			GC.KeepAlive (key);
			return rv;
		}

		/// <summary>
		/// Converts the dictionary to an <see cref="NSFileAttributes"/> object.
		/// </summary>
		/// <returns>An <see cref="NSFileAttributes"/> object representing the dictionary contents.</returns>
		public NSFileAttributes ToFileAttributes ()
		{
			return NSFileAttributes.FromDictionary (this);
		}
	}
}
