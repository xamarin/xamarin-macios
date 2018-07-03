
#if XAMCORE_2_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Foundation;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	public class NSMutableDictionary2Test {

		[Test]
		public void Ctor ()
		{
			var dict = new NSMutableDictionary<NSDate, NSSet> ();
			Assert.AreEqual (0, dict.Count, "Count");
		}

		[Test]
		public void Ctor_NSDictionary ()
		{
			var other = new NSDictionary<NSString, NSString> ((NSString) "key", (NSString) "value");
			var j = new NSMutableDictionary<NSString, NSString> (other);

			Assert.AreEqual (j.Count, 1, "count");
			Assert.AreEqual ((string)(NSString)(j [(NSString) "key"]), "value", "key lookup");
		}

		[Test]
		public void Ctor_NSMutableDictionary ()
		{
			var other = new NSMutableDictionary<NSString, NSString> ();
			other.Add ((NSString) "key", (NSString) "value");
			var j = new NSMutableDictionary<NSString, NSString> (other);

			Assert.AreEqual (j.Count, 1, "count");
			Assert.AreEqual ((string)(NSString)(j [(NSString) "key"]), "value", "key lookup");
		}

		[Test]
		public void FromObjectsAndKeysGenericTest ()
		{
			var keys = new [] {
				new NSString ("Key1"),
				new NSString ("Key2"),
				new NSString ("Key3"),
				new NSString ("Key4"),
				new NSString ("Key5"),
			};
			var values = new [] {
				NSNumber.FromByte (0x1),
				NSNumber.FromFloat (8.5f),
				NSNumber.FromDouble (10.5),
				NSNumber.FromInt32 (42),
				NSNumber.FromBoolean (true),
			};

			var dict = NSMutableDictionary<NSString, NSNumber>.FromObjectsAndKeys (values, keys, values.Length);
			Assert.AreEqual (dict.Count, 5, "count");
			for (int i = 0; i < values.Length; i++)
				Assert.AreEqual (dict [keys [i]], values [i], $"key lookup, Iteration: {i}");
		}

		[Test]
		public void KeyValue_Autorelease ()
		{
			using (var k = new NSString ("keyz")) 
				using (var v = new NSString ("valuez")) {
					var k1 = k.RetainCount;
					if (k1 >= int.MaxValue)
						Assert.Ignore ("RetainCount unusable for testing");
					var k2 = k1;
					Assert.That (k.RetainCount, Is.EqualTo ((nint) 1), "Key.RetainCount-a");
					var v1 = v.RetainCount;
					var v2 = v1;
					Assert.That (v.RetainCount, Is.EqualTo ((nint) 1), "Value.RetainCount-a");
					using (var d = new NSMutableDictionary<NSString, NSString> (k, v)) {
						k2 = k.RetainCount;
						Assert.That (k2, Is.GreaterThan (k1), "Key.RetainCount-b");
						v2 = v.RetainCount;
						Assert.That (v2, Is.GreaterThan (v1), "Value.RetainCount-b");

						Assert.NotNull (d.Keys, "Keys");
						// accessing `allKeys` should *NOT* change the retainCount
						// that would happen without an [Autorelease] and can lead to memory exhaustion
						// https://bugzilla.xamarin.com/show_bug.cgi?id=7723
						Assert.That (k.RetainCount, Is.EqualTo (k2), "Key.RetainCount-c");

						Assert.NotNull (d.Values, "Values");
						Assert.That (v.RetainCount, Is.EqualTo (v2), "Value.RetainCount-c");
					}
					Assert.That (k.RetainCount, Is.LessThan (k2), "Key.RetainCount-d");
					Assert.That (v.RetainCount, Is.LessThan (v2), "Value.RetainCount-d");
				}
		}

		[Test]
		public void XForY_Autorelease ()
		{
			using (var k = new NSString ("keyz")) 
				using (var v = new NSString ("valuez")) {
					var k1 = k.RetainCount;
					if (k1 >= int.MaxValue)
						Assert.Ignore ("RetainCount unusable for testing");
					var k2 = k1;
					Assert.That (k.RetainCount, Is.EqualTo ((nint) 1), "Key.RetainCount-a");
					var v1 = v.RetainCount;
					var v2 = v1;
					Assert.That (v.RetainCount, Is.EqualTo ((nint) 1), "Value.RetainCount-a");
					using (var d = new NSMutableDictionary<NSString, NSString> (k, v)) {
						k2 = k.RetainCount;
						Assert.That (k2, Is.GreaterThan (k1), "Key.RetainCount-b");
						v2 = v.RetainCount;
						Assert.That (v2, Is.GreaterThan (v1), "Value.RetainCount-b");

						var x = d.KeysForObject (v);
						Assert.That (x [0], Is.SameAs (k), "KeysForObject");

						var y = d.ObjectForKey (k);
						Assert.NotNull (y, "ObjectForKey");

						using (var a = new NSMutableArray ()) {
							a.Add (k);
							var z = d.ObjectsForKeys (a, k);
							Assert.That (z [0], Is.SameAs (v), "ObjectsForKeys");
						}

						Assert.That (k.RetainCount, Is.EqualTo (k2), "Key.RetainCount-c");
						Assert.That (v.RetainCount, Is.EqualTo (v2), "Value.RetainCount-c");
					}
					Assert.That (k.RetainCount, Is.LessThan (k2), "Key.RetainCount-d");
					Assert.That (v.RetainCount, Is.LessThan (v2), "Value.RetainCount-d");
				}
		}

		[Test]
		public void Copy ()
		{
			var isMutableCopy = false;
#if __MACOS__
			if (!TestRuntime.CheckMacSystemVersion (10, 8))
				isMutableCopy = true;
#endif
			using (var k = new NSString ("key")) 
				using (var v = new NSString ("value"))
					using (var d = new NSMutableDictionary <NSString, NSString> (k, v)) {
						// NSObject.Copy works because NSDictionary conforms to NSCopying
						using (var copy1 = (NSDictionary) d.Copy ()) {
							Assert.AreNotSame (d, copy1, "1");
							if (isMutableCopy) {
								Assert.That (copy1, Is.TypeOf<NSMutableDictionary> (), "NSDictionary-1");
							} else {
								Assert.That (copy1, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-1");
							}
							Assert.That (copy1.Count, Is.EqualTo ((nuint) 1), "Count-1");
						}

						using (var copy2 = (NSDictionary) d.Copy (null)) {
							Assert.AreNotSame (d, copy2, "2");
							if (isMutableCopy) {
								Assert.That (copy2, Is.TypeOf<NSMutableDictionary> (), "NSDictionary-2");
							} else {
								Assert.That (copy2, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-2");
							}
							Assert.That (copy2.Count, Is.EqualTo ((nuint) 1), "Count-2");
						}

						using (var copy3 = (NSDictionary) d.Copy (NSZone.Default)) {
							Assert.AreNotSame (d, copy3, "3");
							if (isMutableCopy) {
								Assert.That (copy3, Is.TypeOf<NSMutableDictionary> (), "NSDictionary-3");
							} else {
								Assert.That (copy3, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-3");
							}
							Assert.That (copy3.Count, Is.EqualTo ((nuint) 1), "Count-3");
						}
					}
		}

		[Test]
		public void MutableCopy ()
		{
			using (var k = new NSString ("key")) 
				using (var v = new NSString ("value"))
					using (var d = new NSMutableDictionary<NSString, NSString> (k, v)) {
						// NSObject.Copy works because NSDictionary conforms to NSMutableCopying
						using (var copy = (NSDictionary) d.MutableCopy ()) {
							Assert.That (copy, Is.TypeOf<NSMutableDictionary> (), "NSMutableDictionary");
							Assert.That (copy.Count, Is.EqualTo ((nuint) 1), "Count");
						}

						using (var copy = (NSDictionary) d.MutableCopy (null)) {
							Assert.That (copy, Is.TypeOf<NSMutableDictionary> (), "NSMutableDictionary-2");
							Assert.That (copy.Count, Is.EqualTo ((nuint) 1), "Count-2");
						}

						using (var copy = (NSDictionary) d.MutableCopy (NSZone.Default)) {
							Assert.That (copy, Is.TypeOf<NSMutableDictionary> (), "NSMutableDictionary-3");
							Assert.That (copy.Count, Is.EqualTo ((nuint) 1), "Count-3");
						}
					}
		}

		[Test]
		public void ObjectForKeyTest ()
		{
			var value = NSDate.FromTimeIntervalSinceNow (23);
			var key = new NSString ("right key");
			var dict = new NSMutableDictionary<NSString, NSDate> (key, value);

			Assert.Throws<ArgumentNullException> (() => dict.ObjectForKey ((NSString) null), "ANE");
			Assert.AreSame (value, dict.ObjectForKey (key), "right");
			Assert.IsNull (dict.ObjectForKey ((NSString) "wrong key"), "wrong");
		}

		[Test]
		public void KeysTest ()
		{
			var value = NSDate.FromTimeIntervalSinceNow (23);
			var key = new NSString ("right key");
			var dict = new NSMutableDictionary<NSString, NSDate> (key, value);

			var keys = dict.Keys;
			Assert.AreEqual (1, keys.Length, "Length");
			Assert.AreSame (key, keys [0], "1");
		}

		[Test]
		public void KeysForObjectTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dict = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2, key3 },
				new NSDate[] { value1, value1, value2 }
			);

			var rv = dict.KeysForObject (value1);
			Assert.AreEqual (2, rv.Length, "v1");

			rv = dict.KeysForObject (value2);
			Assert.AreEqual (1, rv.Length, "v2");
			Assert.AreSame (key3, rv [0], "v2 key");

			rv = dict.KeysForObject (value3);
			Assert.AreEqual (0, rv.Length, "v3");

			Assert.Throws<ArgumentNullException> (() => dict.KeysForObject (null), "ANE");
		}

		[Test]
		public void ValuesTest ()
		{
			var value = NSDate.FromTimeIntervalSinceNow (23);
			var key = new NSString ("right key");
			var dict = new NSMutableDictionary<NSString, NSDate> (key, value);

			var keys = dict.Values;
			Assert.AreEqual (1, dict.Values.Length, "Length");
			Assert.AreSame (value, dict [key], "1");
		}

		[Test]
		public void ObjectsForKeysTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");
			var key4 = new NSString ("key4");

			var dict = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2, key3 },
				new NSDate[] { value1, value1, value2 }
			);

			var rv = dict.ObjectsForKeys (new NSString [] { key1, key4 }, value3);
			Assert.AreEqual (2, rv.Length, "a");
			Assert.AreSame (value1, rv [0], "a 0");
			Assert.AreSame (value3, rv [1], "a 1");

			rv = dict.ObjectsForKeys (new NSString [] { }, value3);
			Assert.AreEqual (0, rv.Length, "b length");

			Assert.Throws<ArgumentNullException> (() => dict.ObjectsForKeys ((NSString []) null, value3), "c");
			Assert.Throws<ArgumentNullException> (() => dict.ObjectsForKeys (new NSString [] { }, null), "d");
		}

		[Test]
		public void ContainsKeyTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dict = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			Assert.True (dict.ContainsKey (key1), "a");
			Assert.False (dict.ContainsKey (key3), "b");
			Assert.Throws<ArgumentNullException> (() => dict.ContainsKey ((NSString) null), "ANE");
		}

		[Test]
		public void TryGetValueTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dict = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			NSDate value;

			Assert.True (dict.TryGetValue (key1, out value), "a");
			Assert.AreSame (value1, value, "a same");

			Assert.False (dict.TryGetValue (key3, out value), "b");
			Assert.IsNull (value, "b null");
		}

		[Test]
		public void IndexerTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dict = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			Assert.AreSame (value1, dict [key1], "a");
			Assert.IsNull (dict [key3], "b");
			Assert.Throws<ArgumentNullException> (() => GC.KeepAlive (dict [(NSString) null]), "c");
		}

		[Test]
		public void IDictionary2Test ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dictobj = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			var dict = (IDictionary<NSString, NSDate>) dictobj;

			// Add
			Assert.Throws<ArgumentNullException> (() => dict.Add (new KeyValuePair<NSString, NSDate> (null, value1)), "Add ANE 1");
			Assert.Throws<ArgumentNullException> (() => dict.Add (new KeyValuePair<NSString, NSDate> (key1, null)), "Add ANE 2");
			dict.Add (new KeyValuePair<NSString, NSDate> (key3, value3));
			Assert.AreSame (value3, dictobj [key3], "Add 1");
			Assert.AreEqual (3, dict.Count, "Add Count");
			dictobj.Remove (key3); // restore state.

			// Clear
			dict.Clear ();
			Assert.AreEqual (0, dict.Count, "Clear Count");
			dictobj.Add (key1, value1); // restore state
			dictobj.Add (key2, value1); // restore state

			// Contains
			Assert.IsTrue (dict.Contains (new KeyValuePair<NSString, NSDate> (key1, value1)), "Contains 1"); // both key and value matches
			Assert.IsFalse (dict.Contains (new KeyValuePair<NSString, NSDate> (key1, value2)), "Contains 2"); // found key, wrong value
			Assert.IsFalse (dict.Contains (new KeyValuePair<NSString, NSDate> (key3, value2)), "Contains 3"); // wrong key

			// ContainsKey
			Assert.IsTrue (dict.ContainsKey (key1), "ContainsKey 1");
			Assert.IsFalse (dict.ContainsKey (key3), "ContainsKey 2");

			// CopyTo
			var kvp_array = new KeyValuePair<NSString, NSDate> [1];
			Assert.Throws<ArgumentNullException> (() => dict.CopyTo (null, 0), "CopyTo ANE");
			Assert.Throws<ArgumentOutOfRangeException> (() => dict.CopyTo (kvp_array, -1), "CopyTo AOORE");
			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, kvp_array.Length), "CopyTo AE 2");
			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, 0), "CopyTo AE 3");

			kvp_array = new KeyValuePair<NSString, NSDate> [dictobj.Count];

			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, 1), "CopyTo AE 4");
			dict.CopyTo (kvp_array, 0);
			Assert.That (key1, Is.SameAs (kvp_array [0].Key).Or.SameAs (kvp_array [1].Key), "CopyTo K1");
			Assert.AreSame (value1, kvp_array [0].Value, "CopyTo V1");
			Assert.That (key2, Is.SameAs (kvp_array [0].Key).Or.SameAs (kvp_array [1].Key), "CopyTo K2");
			Assert.AreSame (value1, kvp_array [1].Value, "CopyTo V2");

			// Count
			Assert.AreEqual (2, dict.Count, "Count");

			// GetEnumerator
			var enumerated = Enumerable.ToArray (dict);
			Assert.AreEqual (2, enumerated.Length, "Enumerator Count");

			// IsReadOnly
			Assert.IsFalse (dict.IsReadOnly, "IsReadOnly");

			// Keys
			Assert.AreEqual (2, dict.Keys.Count, "Keys Count");

			// Remove
			Assert.Throws<ArgumentNullException> (() => dict.Remove (new KeyValuePair<NSString, NSDate> (null, value3)), "Remove ANE 1");
			Assert.Throws<ArgumentNullException> (() => dict.Remove (new KeyValuePair<NSString, NSDate> (key3, null)), "Remove ANE 2");
			Assert.IsFalse (dict.Remove (new KeyValuePair<NSString, NSDate> (key3, value3)), "Remove 1"); // inexistent key
			Assert.AreEqual (2, dict.Count, "Remove 1 Count");

			Assert.IsFalse (dict.Remove (new KeyValuePair<NSString, NSDate> (key1, value2)), "Remove 2"); // existing key, wrong value
			Assert.AreEqual (2, dict.Count, "Remove 2 Count");

			Assert.IsTrue (dict.Remove (new KeyValuePair<NSString, NSDate> (key1, value1)), "Remove 3"); // existing key,value pair
			Assert.AreEqual (1, dict.Count, "Remove 3 Count");
			dictobj.Add (key1, value1); // restore state

			// TryGetValue
			NSDate value;
			Assert.Throws<ArgumentNullException> (() => dict.TryGetValue (null, out value), "TryGetValue ANE");
			Assert.IsTrue (dict.TryGetValue (key1, out value), "TryGetValue K1");
			Assert.AreSame (value1, value, "TryGetValue V1");
			Assert.IsFalse (dict.TryGetValue (key3, out value), "TryGetValue K2");

			// Values
			Assert.AreEqual (2, dict.Values.Count, "Values Count");

			// Indexer
			Assert.AreSame (value1, dict [key1], "this [1]");
			Assert.IsNull (dict [key3], "this [2]");
			Assert.Throws<ArgumentNullException> (() => GC.KeepAlive (dict [null]), "this [null]");

			dict [key3] = value3;
			Assert.AreEqual (3, dict.Count, "this [3] Count");
			Assert.AreSame (value3, dict [key3], "this [3] = 3");
			dictobj.Remove (key3); // restore state

			Assert.Throws<ArgumentNullException> (() => dict [key3] = null, "this [4] = null");
		}

		[Test]
		public void ICollection2Test ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dictobj = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			var dict = (ICollection<KeyValuePair<NSString, NSDate>>) dictobj;

			// Add
			Assert.Throws<ArgumentNullException> (() => dict.Add (new KeyValuePair<NSString, NSDate> (null, value1)), "Add ANE 1");
			Assert.Throws<ArgumentNullException> (() => dict.Add (new KeyValuePair<NSString, NSDate> (key1, null)), "Add ANE 2");
			dict.Add (new KeyValuePair<NSString, NSDate> (key3, value3));
			Assert.AreSame (value3, dictobj [key3], "Add 1");
			Assert.AreEqual (3, dict.Count, "Add Count");
			dictobj.Remove (key3); // restore state.

			// Clear
			dict.Clear ();
			Assert.AreEqual (0, dict.Count, "Clear Count");
			dictobj.Add (key1, value1); // restore state
			dictobj.Add (key2, value1); // restore state

			// Contains
			Assert.IsTrue (dict.Contains (new KeyValuePair<NSString, NSDate> (key1, value1)), "Contains 1"); // both key and value matches
			Assert.IsFalse (dict.Contains (new KeyValuePair<NSString, NSDate> (key1, value2)), "Contains 2"); // found key, wrong value
			Assert.IsFalse (dict.Contains (new KeyValuePair<NSString, NSDate> (key3, value2)), "Contains 3"); // wrong key


			// CopyTo
			var kvp_array = new KeyValuePair<NSString, NSDate> [1];
			Assert.Throws<ArgumentNullException> (() => dict.CopyTo (null, 0), "CopyTo ANE");
			Assert.Throws<ArgumentOutOfRangeException> (() => dict.CopyTo (kvp_array, -1), "CopyTo AOORE");
			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, kvp_array.Length), "CopyTo AE 2");
			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, 0), "CopyTo AE 3");

			kvp_array = new KeyValuePair<NSString, NSDate> [dictobj.Count];

			Assert.Throws<ArgumentException> (() => dict.CopyTo (kvp_array, 1), "CopyTo AE 4");
			dict.CopyTo (kvp_array, 0);
			Assert.That (key1, Is.SameAs (kvp_array [0].Key).Or.SameAs (kvp_array [1].Key), "CopyTo K1");
			Assert.AreSame (value1, kvp_array [0].Value, "CopyTo V1");
			Assert.That (key2, Is.SameAs (kvp_array [0].Key).Or.SameAs (kvp_array [1].Key), "CopyTo K2");
			Assert.AreSame (value1, kvp_array [1].Value, "CopyTo V2");

			// Count
			Assert.AreEqual (2, dict.Count, "Count");

			// GetEnumerator
			var enumerated = Enumerable.ToArray (dict);
			Assert.AreEqual (2, enumerated.Length, "Enumerator Count");

			// IsReadOnly
			Assert.IsFalse (dict.IsReadOnly, "IsReadOnly");

			// Remove
			Assert.Throws<ArgumentNullException> (() => dict.Remove (new KeyValuePair<NSString, NSDate> (null, value3)), "Remove ANE 1");
			Assert.Throws<ArgumentNullException> (() => dict.Remove (new KeyValuePair<NSString, NSDate> (key3, null)), "Remove ANE 2");
			Assert.IsFalse (dict.Remove (new KeyValuePair<NSString, NSDate> (key3, value3)), "Remove 1"); // inexistent key
			Assert.AreEqual (2, dict.Count, "Remove 1 Count");

			Assert.IsFalse (dict.Remove (new KeyValuePair<NSString, NSDate> (key1, value2)), "Remove 2"); // existing key, wrong value
			Assert.AreEqual (2, dict.Count, "Remove 2 Count");

			Assert.IsTrue (dict.Remove (new KeyValuePair<NSString, NSDate> (key1, value1)), "Remove 3"); // existing key,value pair
			Assert.AreEqual (1, dict.Count, "Remove 3 Count");
			dictobj.Add (key1, value1); // restore state
		}

		[Test]
		public void IEnumerable_KVP2Test ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dictobj = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			var dict = (IEnumerable<KeyValuePair<NSString, NSDate>>) dictobj;

			// GetEnumerator
			var enumerated = Enumerable.ToArray (dict);
			Assert.AreEqual (2, enumerated.Length, "Enumerator Count");
		}

		[Test]
		public void IEnumerableTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var value3 = NSDate.FromTimeIntervalSinceNow (3);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");
			var key3 = new NSString ("key3");

			var dictobj = new NSMutableDictionary<NSString, NSDate> (
				new NSString [] { key1, key2 },
				new NSDate[] { value1, value1 }
			);

			var dict = (IEnumerable) dictobj;

			// GetEnumerator
			var c = 0;
			foreach (var obj in dict)
				c++;
			Assert.AreEqual (2, c, "Enumerator Count");
		}

		[Test]
		public void AddTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");

			var dict = new NSMutableDictionary<NSString, NSDate> ();

			Assert.Throws<ArgumentNullException> (() => dict.Add (null, value1), "ANE 1");
			Assert.Throws<ArgumentNullException> (() => dict.Add (key1, null), "ANE 2");

			dict.Add (key1, value1);
			Assert.AreEqual (1, dict.Count, "a Count");
			Assert.AreSame (value1, dict [key1], "a idx");

			dict.Add (key1, value1);
			Assert.AreEqual (1, dict.Count, "b Count");
			Assert.AreSame (value1, dict [key1], "b idx");

			dict.Add (key2, value1);
			Assert.AreEqual (2, dict.Count, "c Count");
			Assert.AreSame (value1, dict [key2], "c idx");
		}

		[Test]
		public void RemoveTest ()
		{
			var value1 = NSDate.FromTimeIntervalSinceNow (1);
			var value2 = NSDate.FromTimeIntervalSinceNow (2);
			var key1 = new NSString ("key1");
			var key2 = new NSString ("key2");

			var dict = new NSMutableDictionary<NSString, NSDate> ();

			Assert.Throws<ArgumentNullException> (() => dict.Remove ((NSString) null), "ANE 1");

			dict.Add (key1, value1);

			dict.Remove (key2);
			Assert.AreEqual (1, dict.Count, "a Count");
			Assert.AreSame (value1, dict [key1], "a idx");

			dict.Remove (key1);
			Assert.AreEqual (0, dict.Count, "b Count");
		}

		[Test]
		public void InvalidType ()
		{
			var kv = (NSString) "a";
			var dt = NSDate.FromTimeIntervalSinceNow (1);
			var obj = new NSDictionary (kv, kv);
			NSDate value = NSDate.FromTimeIntervalSinceNow (3);

			// dict where TValue is wrong
			var dict = new NSMutableDictionary<NSString, NSDate> ();
			dict.Add (kv, kv);
			Assert.Throws<InvalidCastException> (() => GC.KeepAlive (dict [kv]), "idx 1");
			Assert.Throws<InvalidCastException> (() => dict.ObjectForKey (kv), "ObjectForKey");
			Assert.Throws<InvalidCastException> (() => dict.ObjectsForKeys (new NSString [] { kv }, value), "ObjectsForKeys");
			Assert.Throws<InvalidCastException> (() => dict.TryGetValue (kv, out value), "TryGetValue");
			Assert.Throws<InvalidCastException> (() => GC.KeepAlive (dict.Values), "Values");

			// dict where TKey is wrong
			var dictK = new NSMutableDictionary<NSDate, NSString> ();
			dictK.Add (kv, kv);
			Assert.Throws<InvalidCastException> (() => GC.KeepAlive (dictK.Keys), "K Keys");
			Assert.Throws<InvalidCastException> (() => dictK.KeysForObject (kv), "K KeysForObject");
		}
	}
}

#endif // XAMCORE_2_0
