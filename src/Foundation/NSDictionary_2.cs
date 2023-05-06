//
// Copyright 2015 Xamarin Inc
//
// This file contains a generic version of NSDictionary.
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
	[Register ("NSDictionary", SkipRegistration = true)]
	public sealed partial class NSDictionary<TKey, TValue> : NSDictionary, IDictionary<TKey, TValue>
		where TKey : class, INativeObject
		where TValue : class, INativeObject {
		public NSDictionary ()
		{
		}

		public NSDictionary (NSCoder coder)
			: base (coder)
		{
		}

		public NSDictionary (string filename)
			: base (filename)
		{
		}

		public NSDictionary (NSUrl url)
			: base (url)
		{
		}

		internal NSDictionary (NativeHandle handle)
			: base (handle)
		{
		}

		public NSDictionary (NSDictionary<TKey, TValue> other)
			: base (other)
		{
		}

		internal static bool ValidateKeysAndValues (TKey [] keys, TValue [] values)
		{
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));

			if (values is null)
				throw new ArgumentNullException (nameof (values));

			if (values.Length != keys.Length)
				throw new ArgumentException (nameof (values) + " and " + nameof (keys) + " arrays have different sizes");

			return true;
		}

		NSDictionary (TKey [] keys, TValue [] values, bool validation)
			: base (NSArray.FromNSObjects (values), NSArray.FromNSObjects (keys))
		{
		}

		public NSDictionary (TKey [] keys, TValue [] values)
			: this (keys, values, ValidateKeysAndValues (keys, values))
		{
		}

		public NSDictionary (TKey key, TValue value)
			: base (NSArray.FromNSObjects (value), NSArray.FromNSObjects (key))
		{
		}

		// Strongly typed methods from NSDictionary

		public TValue ObjectForKey (TKey key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			return Runtime.GetINativeObject<TValue> (_ObjectForKey (key.Handle), false);
		}

		public TKey [] Keys {
			get {
				using (var pool = new NSAutoreleasePool ())
					return NSArray.ArrayFromHandle<TKey> (_AllKeys ());
			}
		}

		public TKey [] KeysForObject (TValue obj)
		{
			if (obj is null)
				throw new ArgumentNullException (nameof (obj));

			using (var pool = new NSAutoreleasePool ())
				return NSArray.ArrayFromHandle<TKey> (_AllKeysForObject (obj.Handle));
		}

		public TValue [] Values {
			get {
				using (var pool = new NSAutoreleasePool ())
					return NSArray.ArrayFromHandle<TValue> (_AllValues ());
			}
		}

		public TValue [] ObjectsForKeys (TKey [] keys, TValue marker)
		{
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));

			if (marker is null)
				throw new ArgumentNullException (nameof (marker));

			if (keys.Length == 0)
				return new TValue [] { };

			using (var pool = new NSAutoreleasePool ())
				return NSArray.ArrayFromHandle<TValue> (_ObjectsForKeys (NSArray.From<TKey> (keys).Handle, marker.Handle));
		}

		static NSDictionary<TKey, TValue> GenericFromObjectsAndKeysInternal (NSArray objects, NSArray keys)
		{
			return Runtime.GetNSObject<NSDictionary<TKey, TValue>> (_FromObjectsAndKeysInternal (objects.Handle, keys.Handle));
		}

		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (TValue [] objects, TKey [] keys, nint count)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			if (count < 1 || objects.Length < count)
				throw new ArgumentException (nameof (count));

			using (var no = NSArray.FromNativeObjects (objects, count))
			using (var nk = NSArray.FromNativeObjects (keys, count))
				return GenericFromObjectsAndKeysInternal (no, nk);
		}

#if NET
		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (TValue [] objects, TKey [] keys)
#else
		[Obsolete ("'TKey' and 'TValue' are inversed and won't work unless both types are identical. Use the generic overload that takes a count parameter instead.")]
		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (TKey [] objects, TValue [] keys)
#endif
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");

			using (var no = NSArray.FromNSObjects (objects))
			using (var nk = NSArray.FromNSObjects (keys))
				return GenericFromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (object [] objects, object [] keys)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");

			using (var no = NSArray.FromObjects (objects))
			using (var nk = NSArray.FromObjects (keys))
				return GenericFromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (NSObject [] objects, NSObject [] keys, nint count)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			if (count < 1 || objects.Length < count || keys.Length < count)
				throw new ArgumentException (nameof (count));

			using (var no = NSArray.FromNativeObjects (objects, count))
			using (var nk = NSArray.FromNativeObjects (keys, count))
				return GenericFromObjectsAndKeysInternal (no, nk);
		}

		public static NSDictionary<TKey, TValue> FromObjectsAndKeys (object [] objects, object [] keys, nint count)
		{
			if (objects is null)
				throw new ArgumentNullException (nameof (objects));
			if (keys is null)
				throw new ArgumentNullException (nameof (keys));
			if (objects.Length != keys.Length)
				throw new ArgumentException (nameof (objects) + " and " + nameof (keys) + " arrays have different sizes");
			if (count < 1 || objects.Length < count || keys.Length < count)
				throw new ArgumentException (nameof (count));

			using (var no = NSArray.FromObjects (count, objects))
			using (var nk = NSArray.FromObjects (count, keys))
				return GenericFromObjectsAndKeysInternal (no, nk);
		}

		// Other implementations

		public bool ContainsKey (TKey key)
		{
			if (key is null)
				throw new ArgumentNullException (nameof (key));

			return _ObjectForKey (key.Handle) != IntPtr.Zero;
		}

		public bool TryGetValue (TKey key, out TValue value)
		{
			// NSDictionary can not contain NULLs, if you want a NULL, it exists as an NSNull
			return (value = ObjectForKey (key)) is not null;
		}

		public TValue this [TKey key] {
			get {
				return ObjectForKey (key);
			}
		}

		#region IDictionary<K,V> implementation
		bool IDictionary<TKey, TValue>.ContainsKey (TKey key)
		{
			return ContainsKey (key);
		}

		void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
		{
			throw new NotSupportedException ();
		}

		bool IDictionary<TKey, TValue>.Remove (TKey key)
		{
			throw new NotSupportedException ();
		}

		bool IDictionary<TKey, TValue>.TryGetValue (TKey key, out TValue value)
		{
			return TryGetValue (key, out value);
		}

		TValue IDictionary<TKey, TValue>.this [TKey key] {
			get {
				return this [key];
			}
			set {
				throw new NotSupportedException ();
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get {
				return Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get {
				return Values;
			}
		}
		#endregion

		#region ICollection<K,V> implementation
		void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException ();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
		{
			throw new NotSupportedException ();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			if (!TryGetValue<TValue> (item.Key, out value))
				return false;

			return (object) value == (object) item.Value;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue> [] array, int arrayIndex)
		{
			if (array is null)
				throw new ArgumentNullException (nameof (array));
			if (arrayIndex < 0)
				throw new ArgumentOutOfRangeException (nameof (arrayIndex));
			int c = array.Length;
			if ((c > 0) && (arrayIndex >= c))
				throw new ArgumentException (nameof (arrayIndex) + " is equal to or greater than " + nameof (array) + ".Length");
			if (arrayIndex + (int) Count > c)
				throw new ArgumentException ("Not enough room from " + nameof (arrayIndex) + " to end of " + nameof (array) + " for this dictionary");

			var idx = arrayIndex;
			foreach (var kvp in (IEnumerable<KeyValuePair<TKey, TValue>>) this)
				array [idx++] = kvp;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException ();
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count {
			get {
				return (int) base.Count;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
			get {
				return true;
			}
		}
		#endregion

		#region IEnumerable<KVP> implementation
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
		{
			foreach (var key in Keys) {
				yield return new KeyValuePair<TKey, TValue> (key, ObjectForKey (key));
			}
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		#endregion
	}
}
