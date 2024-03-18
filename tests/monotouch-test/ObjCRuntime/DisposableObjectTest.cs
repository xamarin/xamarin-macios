using System;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoTouchFixtures.ObjCRuntime {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DisposableObjectTest {
		class Subclassed : DisposableObject {
			public Subclassed () : base () { }
			public Subclassed (NativeHandle handle, bool owns) : base (handle, owns) { }
			public Subclassed (NativeHandle handle, bool owns, bool verify) : base (handle, owns, verify) { }

			public new NativeHandle Handle {
				get => base.Handle;
				set => base.Handle = value;
			}

			public new bool Owns { get => base.Owns; }
		}

		[Test]
		public void DefaultCtor ()
		{
			var obj = new Subclassed ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle");
			Assert.AreEqual (false, obj.Owns, "Owns");
		}

		[Test]
		public void CtorOwns ()
		{
			Subclassed obj;

			var ex = Assert.Throws<Exception> (() => obj = new Subclassed (NativeHandle.Zero, true), "Handle 1");
			Assert.That (ex.Message, Does.Contain ("Could not initialize an instance of the type"), "Ex 1");

			ex = Assert.Throws<Exception> (() => obj = new Subclassed (NativeHandle.Zero, false), "Handle 2");
			Assert.That (ex.Message, Does.Contain ("Could not initialize an instance of the type"), "Ex 2");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, true);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 3");
			Assert.AreEqual (true, obj.Owns, "Owns 3");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 3");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 3b");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 3b");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, false);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 4");
			Assert.AreEqual (false, obj.Owns, "Owns 4");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 4");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 4b");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 4b");
		}

		[Test]
		public void CtorOwnsVerify ()
		{
			var obj = new Subclassed (NativeHandle.Zero, true, false);
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 1");
			Assert.AreEqual (true, obj.Owns, "Owns 1");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 1");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 1b");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 1b");

			obj = new Subclassed (NativeHandle.Zero, false, false);
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 2");
			Assert.AreEqual (false, obj.Owns, "Owns 2");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 2");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 2b");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 2b");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, true, false);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 3");
			Assert.AreEqual (true, obj.Owns, "Owns 3");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 3");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 3b");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 3b");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, false, false);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 4");
			Assert.AreEqual (false, obj.Owns, "Owns 4");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 4");


			var ex = Assert.Throws<Exception> (() => obj = new Subclassed (NativeHandle.Zero, true, true), "Handle 1V");
			Assert.That (ex.Message, Does.Contain ("Could not initialize an instance of the type"), "Ex 1V");

			ex = Assert.Throws<Exception> (() => obj = new Subclassed (NativeHandle.Zero, false, true), "Handle 2V");
			Assert.That (ex.Message, Does.Contain ("Could not initialize an instance of the type"), "Ex 2V");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, true, true);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 3V");
			Assert.AreEqual (true, obj.Owns, "Owns 3V");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 3V");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 3Vb");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 3Vb");

			obj = new Subclassed ((NativeHandle) (IntPtr) 1, false, true);
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "Handle 4V");
			Assert.AreEqual (false, obj.Owns, "Owns 4V");
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.GetCheckedHandle (), "GetCheckedHandle 4V");
			obj.Dispose ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle 4Vb");
			Assert.Throws<ObjectDisposedException> (() => obj.GetCheckedHandle (), "GetCheckedHandle 4Vb");
		}

		[Test]
		public void Handle ()
		{
			var obj = new Subclassed ();
			Assert.AreEqual (NativeHandle.Zero, obj.Handle, "Handle");
			var ex = Assert.Throws<Exception> (() => obj.Handle = NativeHandle.Zero, "SetHandle ex");
			obj.Handle = (NativeHandle) (IntPtr) 1;
			Assert.AreEqual ((NativeHandle) (IntPtr) 1, obj.Handle, "GetHandle");
		}
	}
}
