//
// Linker interfaces-related Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using NUnit.Framework;

namespace LinkAll.Interfaces {

	interface I {
		void Foo ();
		void Bar ();
	}

	class A : I {
		public void Foo () { }
		public void Bar () { }
	}

	class B : I {
		public void Foo () { }
		public void Bar () { }
	}

	class UTF8Marshaler : ICustomMarshaler {

		public object MarshalNativeToManaged (IntPtr pNativeData)
		{
			return null;
		}

		public IntPtr MarshalManagedToNative (object managedObject)
		{
			return IntPtr.Zero;
		}

		public int GetNativeDataSize ()
		{
			return 0;
		}

		public void CleanUpManagedData (object managedObject)
		{
		}

		public void CleanUpNativeData (IntPtr pNativeData)
		{
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class InterfaceTest {

		static Type type_a = typeof (A);
		static Type type_b = typeof (B);
		static Type type_i = typeof (I);

		static void F (I i)
		{
			i.Bar ();
		}

		[Test]
		public void Bug10866 ()
		{
			var a = new A ();
			a.Foo ();
			a.Bar ();
			F (new A ());

			// Foo and Bar methods are both used on A and must be present
			Assert.NotNull (type_a.GetMethod ("Foo", BindingFlags.Instance | BindingFlags.Public), "A::Foo");
			Assert.NotNull (type_a.GetMethod ("Bar", BindingFlags.Instance | BindingFlags.Public), "A::Bar");

			// I::Foo is never used and can be removed
			Assert.Null (type_i.GetMethod ("Foo", BindingFlags.Instance | BindingFlags.Public), "I::Foo");
			// I::Bar is used in F so everyone implementing I needs Bar 
			Assert.NotNull (type_i.GetMethod ("Bar", BindingFlags.Instance | BindingFlags.Public), "I::Bar");

			// Foo and Bar are never used on B - so they can be removed
			Assert.Null (type_b.GetMethod ("Foo", BindingFlags.Instance | BindingFlags.Public), "B::Foo");
#if !NET // This is actually a bug in the linker that's been fixed in .NET
			Assert.Null (type_b.GetMethod ("Bar", BindingFlags.Instance | BindingFlags.Public), "B::Bar");
#endif
		}

		[Test]
#if !NET
		[Ignore ("https://github.com/xamarin/xamarin-macios/issues/9566")]
#endif
		public void Issue9566 ()
		{
			var ifaces = (I []) (object) new B [0];
			Assert.IsNotNull (ifaces, "Array cast");
		}

		[DllImport ("/usr/lib/system/libsystem_dnssd.dylib")]
		public static extern int DNSServiceGetProperty (
			[MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof (UTF8Marshaler))] string name,
			IntPtr result, ref uint size);

		uint size;

		[Test]
		public void Bug16485 ()
		{
			if (size == 1)
				DNSServiceGetProperty ("Xamarin", IntPtr.Zero, ref size);
			// we don't want to call the function - we only want the AOT compiler to build the CustomMarshaler
			Assert.That (size, Is.EqualTo (0), "size");
		}
	}
}
