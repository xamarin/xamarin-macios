#if __MACOS__
using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[Register ("SimpleRegistrarTestClass")]
	class SimpleRegistrarTestClass : NSObject {
		public virtual string Value {
			[Export ("value")]
			get {
				return "RegistrarTestClass";
			}
		}
	}

	[Register ("RegistrarTestDerivedClass")]
	class RegistrarTestDerivedClass : SimpleRegistrarTestClass {
		public override string Value {
			get {
				return "RegistrarTestDerivedClass";
			}
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class SimpleRegistrarTest {
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

		[Test]
		public void SimpleRegistrarSmokeTest ()
		{
			SimpleRegistrarTestClass obj = new SimpleRegistrarTestClass ();
			IntPtr receiver = obj.Handle;

			RegistrarTestDerivedClass derivedObj = new RegistrarTestDerivedClass ();
			IntPtr derivedReceiver = derivedObj.Handle;

			Assert.AreEqual (Runtime.GetNSObject<NSString> (IntPtr_objc_msgSend (receiver, Selector.GetHandle ("value"))), (NSString) "RegistrarTestClass");

			Assert.AreEqual (Runtime.GetNSObject<NSString> (IntPtr_objc_msgSend (derivedReceiver, Selector.GetHandle ("value"))), (NSString) "RegistrarTestDerivedClass");
		}

		[Test]
		public void SimpleRegistrar_XamarinMacRegistered ()
		{
			// __NSObject_Disposer is registered by src/Foundation/NSObject2.cs and should exist
			// This will throw is for some reason it is not
			Class c = new Class ("__NSObject_Disposer");
		}
	}
}
#endif // __MACOS__
