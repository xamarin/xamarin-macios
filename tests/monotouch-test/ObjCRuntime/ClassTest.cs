//
// Unit tests for Class
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Foundation;
using ObjCRuntime;
using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ClassTest {
		[DllImport ("/usr/lib/libobjc.dylib")]
		extern static IntPtr objc_getClass (string name);

		// based on https://xamarin.assistly.com/agent/case/6816
		[Register ("ZählerObject")]
		class ZählerObject : NSObject {
		}

		[Test]
		public void getClassTest ()
		{
			IntPtr p = objc_getClass ("ZählerNotExists");
			Assert.That (p, Is.EqualTo (IntPtr.Zero), "DoesNotExists");

			p = objc_getClass ("ZählerObject");
			Assert.That (p, Is.Not.EqualTo (IntPtr.Zero), "ä");
		}

		[Test]
		public void LookupTest ()
		{
			IntPtr p = objc_getClass ("ZählerObject");
			var m = typeof (Class).GetMethod ("Lookup", BindingFlags.NonPublic | BindingFlags.Static, null, new Type [] { typeof (IntPtr) }, null);
			Type t = (Type) m.Invoke (null, new object [] { objc_getClass ("ZählerObject") });
			Assert.That (t, Is.EqualTo (typeof (ZählerObject)), "Lookup");
			Assert.That (p, Is.Not.EqualTo (IntPtr.Zero), "Class");
		}

		[Test]
		public void Ctor ()
		{
			Assert.DoesNotThrow (() => new Class (typeof (NSObject)), "NSObject");
			if (Runtime.DynamicRegistrationSupported) {
				Assert.AreEqual (NativeHandle.Zero, new Class (typeof (string)).Handle, "string");
			} else {
				try {
					new Class (typeof (string));
				} catch (Exception e) {
					Assert.AreEqual (typeof (RuntimeException), e.GetType (), "string exc");
					Assert.AreEqual ("Can't register the class System.String when the dynamic registrar has been linked away.", e.Message, "exc message");
				}
			}
		}

		[Test]
		public void GetHandle ()
		{
			Assert.AreNotEqual (NativeHandle.Zero, Class.GetHandle (typeof (NSObject)), "NSObject");
			if (Runtime.DynamicRegistrationSupported) {
				Assert.AreEqual (NativeHandle.Zero, Class.GetHandle (typeof (string)), "string 1");
			} else {
				try {
					Class.GetHandle (typeof (string));
				} catch (Exception e) {
					Assert.AreEqual (typeof (RuntimeException), e.GetType (), "string exc");
					Assert.AreEqual ("Can't register the class System.String when the dynamic registrar has been linked away.", e.Message, "exc message");
				}
			}
			Assert.AreEqual (NativeHandle.Zero, Class.GetHandle (typeof (NSObject).MakeByRefType ()), "NSObject&");
			Assert.AreEqual (NativeHandle.Zero, Class.GetHandle (typeof (NSObject).MakeArrayType ()), "NSObject[]");
			Assert.AreEqual (NativeHandle.Zero, Class.GetHandle (typeof (NSObject).MakePointerType ()), "NSObject*");
		}

		[Test]
		public void Lookup ()
		{
			Assert.AreEqual (typeof (NSObject), Class.Lookup (new Class (typeof (NSObject))), "NSObject");
			Assert.AreEqual (typeof (NSString), Class.Lookup (new Class (typeof (NSString))), "NSString");
			Assert.AreNotEqual (typeof (NSObject), Class.Lookup (new Class (typeof (NSString))), "neq");
			try {
				Class.Lookup (new Class ("NSProxy"));
			} catch (Exception e) {
				Assert.AreEqual (typeof (RuntimeException), e.GetType (), "NSProxy exception");
				if (Runtime.DynamicRegistrationSupported) {
					Assert.AreEqual ("The ObjectiveC class 'NSProxy' could not be registered, it does not seem to derive from any known ObjectiveC class (including NSObject).", e.Message, "NSProxy exception message");
				} else {
					Assert.That (e.Message, Does.Match ("Can't lookup the Objective-C class 0x.* w"), "NSProxy exception message 2");
				}
			}
			Assert.Throws<ArgumentException> (() => new Class ("InexistentClass"), "inexistent");
			// Private class which we've obviously not bound, but we've bound a super class.
			// And yes, NSMutableString is the first public superclass of __NSCFConstantString.
			Assert.AreEqual (typeof (NSMutableString), Class.Lookup (new Class ("__NSCFConstantString")), "private class");
		}

		[Test]
		public void IsCustomType ()
		{
			using (var str = new DirtyType ())
				str.MarkDirty ();
		}

		class DirtyType : NSObject {
			public new void MarkDirty ()
			{
				base.MarkDirty ();
			}
		}

		// Not sure what to do about this one, it doesn't compile with the static registrar (since linking fails)
#if DYNAMIC_REGISTRAR
		[Test]
		public void ThrowOnMissingNativeClassTest ()
		{
			bool saved = Class.ThrowOnInitFailure;

			Class.ThrowOnInitFailure = true;
			try {
				Assert.Throws<Exception> (() => new InexistentClass (), "a");
			} finally {
				Class.ThrowOnInitFailure = saved;
			}
		}

		[Register ("Inexistent", true)]
		public class InexistentClass : NSObject {
			public override NativeHandle ClassHandle {
				get {
					return Class.GetHandle (GetType ().Name);
				}
			}
		}
#endif

		[Test]
		public void Bug33981 ()
		{
			var types = new List<Type> ();
			foreach (var type in GetType ().Assembly.GetTypes ()) {
				if (type.IsSubclassOf (typeof (NSObject)) && type.Name.StartsWith ("BUG33981"))
					types.Add (type);
			}

			Assert.That (types.Count, Is.GreaterThan (50), "test type enumeration");

			const int n = 5;
			var threads = new Thread [n];
			var cntr = new int [n];
			Exception ex = null;
			var startPistol = new ManualResetEvent (false);
			var stopLine = new CountdownEvent (n);
			for (int i = 0; i < n; i++) {
				var idx = i;
				threads [i] = new Thread (() => {
					startPistol.WaitOne ();
					try {
						foreach (var type in types) {
							var c = Class.GetHandle (type.Name);
							if (c != IntPtr.Zero) {
								try {
									Class.Lookup (new Class (c));
								} catch (Exception e) {
									ex = e;
									return;
								}
								cntr [idx]++;
							}
						}
					} finally {
						stopLine.Signal ();
					}
				});
				threads [i].IsBackground = true;
				threads [i].Start ();
			}
			startPistol.Set ();
			stopLine.Wait ();

			Assert.IsNull (ex);
		}
	}
}
