using System;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace monotouchtest {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSMutableDictionaryTest {

		[Test]
		public void IndexerTest ()
		{
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
				using (var dict = Runtime.GetNSObject<NSMutableDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSMutableDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict ["key"];
					Assert.AreEqual ("obj", (string) v, "a");

					dict ["key"] = (NSString) "value";
					v = (NSString) dict ["key"];
					Assert.AreEqual ("value", (string) v, "a");
				}

				// this[NSObject]
				keyptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strkeyptr);
				objptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strobjptr);
				using (var dict = Runtime.GetNSObject<NSMutableDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSMutableDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict [(NSObject) (NSString) "key"];
					Assert.AreEqual ("obj", (string) v, "b");

					dict [(NSObject) (NSString) "key"] = (NSString) "value";
					v = (NSString) dict ["key"];
					Assert.AreEqual ("value", (string) v, "a");
				}

				// this[NSString]
				keyptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strkeyptr);
				objptr = Messaging.IntPtr_objc_msgSend_IntPtr (Class.GetHandle (typeof (NSString)), Selector.GetHandle ("stringWithUTF8String:"), strobjptr);
				using (var dict = Runtime.GetNSObject<NSMutableDictionary> (Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (Class.GetHandle (typeof (NSMutableDictionary)), Selector.GetHandle ("dictionaryWithObject:forKey:"), objptr, keyptr))) {
					v = (NSString) dict [(NSString) "key"];
					Assert.AreEqual ("obj", (string) v, "c");

					dict [(NSString) "key"] = (NSString) "value";
					v = (NSString) dict ["key"];
					Assert.AreEqual ("value", (string) v, "a");
				}

			} finally {
				Marshal.FreeHGlobal (strkeyptr);
				Marshal.FreeHGlobal (strobjptr);
			}
		}

		[Test]
		public void Bug39993 ()
		{
			using (NSMutableDictionary testDict = new NSMutableDictionary ()) {
				testDict.Add ((NSString) "Key1", (NSString) "Key1");
				testDict.Add ((NSString) "Key2", (NSString) "KeyTest2");
				Assert.NotNull (testDict ["Key1"], "Key1");
				Assert.NotNull (testDict ["Key2"], "Key2");
			}
		}

		[Test]
		public void AddEntries ()
		{
			using (var dic1 = new NSMutableDictionary ()) {
				using (var dic2 = NSDictionary.FromObjectAndKey ((NSString) "value", (NSString) "key")) {
					Assert.AreEqual ((nuint) 0, dic1.Count, "Count 0");

					dic1.AddEntries (dic2);

					Assert.AreEqual ((nuint) 1, dic1.Count, "Count 1");
					Assert.AreEqual ("value", dic1 ["key"].ToString (), "Value 1");

					dic1.AddEntries (dic2);

					Assert.AreEqual ((nuint) 1, dic1.Count, "Count 2");
					Assert.AreEqual ("value", dic1 ["key"].ToString (), "Value 2");
				}
			}
		}
	}
}
