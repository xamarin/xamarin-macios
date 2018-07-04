using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

#if XAMCORE_2_0
using RectangleF=CoreGraphics.CGRect;
using SizeF=CoreGraphics.CGSize;
using PointF=CoreGraphics.CGPoint;
#else
using nfloat=global::System.Single;
using nint=global::System.Int32;
using nuint=global::System.UInt32;
#endif

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	public class NSDictionaryTest {

		//
		// Tests for the new NSDictionary from parameters constructors
		//
		[Test]
		public void DictionaryCtorKeyValues ()
		{
			var key = new NSString ("key");
			var value = new NSString ("value");
			var j = new NSDictionary (key, value);
		
			Assert.AreEqual (j.Count, 1, "count");
			Assert.AreEqual (j [key], value, "key lookup");

			j = new NSDictionary (new NSString ("first"), new NSString ("first-k"),
			                      new NSString ("second"), new NSString ("second-k"));
			Assert.AreEqual (j.Count, 2, "count");
			Assert.AreEqual ((string)(NSString)(j ["first"]), "first-k", "lookup1");
			Assert.AreEqual ((string)(NSString)(j ["second"]), "second-k", "lookup2");
		}

		[Test]
		public void DictionaryCtorKeyValuesObjects ()
		{
			var j = new NSDictionary ("key", "value");
			
			Assert.AreEqual (j.Count, 1, "count");
			Assert.AreEqual ((string)(NSString)(j ["key"]), "value", "key lookup");
			
			j = new NSDictionary (1, 2, 3, 4);

			Assert.AreEqual (j.Count, 2, "count");
			Assert.AreEqual (((NSNumber) j [new NSNumber (1)]).Int32Value, 2, "lookup1");
			Assert.AreEqual (((NSNumber) j [new NSNumber (3)]).Int32Value, 4, "lookup2");
		}

		[Test]
		public void InbalancedCtor()
		{
			try {
				var j = new NSDictionary (new NSString ("key"), new NSString ("value"), new NSString ("other"));
			} catch (ArgumentException) {
				return;
			}
			Assert.Fail ("Should have thrown an exception");
		}

		[Test]
		public void InbalancedCtor2()
		{
			try {
				var j = new NSDictionary (1, 2, 3);
			} catch (ArgumentException) {
				return;
			}
			Assert.Fail ("Should have thrown an exception");
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
				using (var d = new NSDictionary (k, v)) {
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
				using (var d = new NSDictionary (k, v)) {
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
		public void FromObjectsAndKeysTest ()
		{
			{
				var keys = new NSObject[] { new NSNumber(1), new NSNumber(2) };
				var objs = new NSObject[] { new NSNumber(1), new NSNumber(4) };
				NSDictionary ns = NSDictionary.FromObjectsAndKeys (objs, keys, 1);
				Console.WriteLine (ns.Count);
				Assert.AreEqual (1, ns.Count, "#1");
			}
			{
				var keys = new object[] { 1, 2 };
				var objs = new object[] { 3, 4 };
				NSDictionary ns = NSDictionary.FromObjectsAndKeys (objs, keys, 1);
				Assert.AreEqual (1, ns.Count, "#2");
			}
		}

		[Test]
		public void Copy ()
		{
			using (var k = new NSString ("key")) 
			using (var v = new NSString ("value"))
			using (var d = new NSDictionary (k, v)) {
				// NSObject.Copy works because NSDictionary conforms to NSCopying
				// note: we do not Dispose the "copies" because it's the same instance being returned
				var copy1 = (NSDictionary) d.Copy ();
				Assert.AreSame (d, copy1, "1");
				Assert.That (copy1, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-1");
				Assert.That (copy1.Count, Is.EqualTo ((nuint) 1), "Count-1");

				var copy2 = (NSDictionary) d.Copy (null);
				Assert.AreSame (d, copy2, "2");
				Assert.That (copy2, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-2");
				Assert.That (copy2.Count, Is.EqualTo ((nuint) 1), "Count-2");

				var copy3 = (NSDictionary) d.Copy (NSZone.Default);
				Assert.AreSame (d, copy3, "3");
				Assert.That (copy3, Is.Not.TypeOf<NSMutableDictionary> (), "NSDictionary-3");
				Assert.That (copy3.Count, Is.EqualTo ((nuint) 1), "Count-3");
			}
		}

		[Test]
		public void MutableCopy ()
		{
			using (var k = new NSString ("key")) 
			using (var v = new NSString ("value"))
			using (var d = new NSDictionary (k, v)) {
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
		public void IndexerTest ()
		{
			// This test doesn't work on Lion, because Lion returns mutable dictionaries in some places this test asserts that those dictionaries are non-mutable.
			TestRuntime.AssertSystemVersion (PlatformName.MacOSX, 10, 8, throwIfOtherPlatform: false);
			IntPtr strkeyptr = IntPtr.Zero;
			IntPtr strobjptr = IntPtr.Zero;
			IntPtr objptr;
			IntPtr keyptr;

			NSString obj, key;
			NSString v;

			try {
				strkeyptr = Marshal.StringToHGlobalAuto ("key");
				strobjptr = Marshal.StringToHGlobalAuto ("obj");

				// this[string]
				keyptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strkeyptr);
				objptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strobjptr);
				using (var dict = Runtime.GetNSObject<NSDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict ["key"];
					Assert.AreEqual ("obj", (string) v, "a");

					Assert.Throws<NotSupportedException> (() => dict ["key"] = (NSString) "value", "a ex");
				}

				// this[NSObject]
				keyptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strkeyptr);
				objptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strobjptr);
				using (var dict = Runtime.GetNSObject<NSDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict [(NSObject) (NSString) "key"];
					Assert.AreEqual ("obj", (string) v, "b");

					Assert.Throws<NotSupportedException> (() => dict [(NSObject) (NSString) "key"] = (NSString) "value", "a ex");
				}

				// this[NSString]
				keyptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strkeyptr);
				objptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strobjptr);
				using (var dict = Runtime.GetNSObject<NSDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict [(NSString) "key"];
					Assert.AreEqual ("obj", (string) v, "c");

					Assert.Throws<NotSupportedException> (() => dict [(NSString) "key"] = (NSString) "value", "a ex");
				}

			} finally {
				Marshal.FreeHGlobal (strkeyptr);
				Marshal.FreeHGlobal (strobjptr);
			}
		}
	}
}
