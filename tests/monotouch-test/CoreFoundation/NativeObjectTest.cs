using System;
using System.Runtime.CompilerServices;
using CoreFoundation;
using Foundation;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreFoundation {

	class FakeNativeObject : NativeObject {

		int rc;

		public FakeNativeObject (IntPtr handle, bool own) : base (handle, own)
		{
		}

		protected override void Retain ()
		{
			rc++;
		}

		protected override void Release ()
		{
			rc--;
		}

		public int RetainCount => rc;

		public override bool Equals (object obj)
		{
			return (obj is null);
		}

		public override int GetHashCode ()
		{
			return 0;
		}
	}

	class NativeObjectPoker : NativeObject {

		// `true` since it's a fake handle - and we don't want to call native API with it
		public NativeObjectPoker (IntPtr handle) : base (handle, true)
		{
		}

		public bool CallNative { get; set; }

		protected override void Retain ()
		{
			if (CallNative)
				base.Retain ();
		}

		protected override void Release ()
		{
			if (CallNative)
				base.Release ();
		}

		public void _Retain () => Retain ();
		public void _Release () => Release ();
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NativeObjectTest {

		[Test]
		public void Create ()
		{
			using (var x = new FakeNativeObject ((IntPtr) 1, false)) {
				Assert.That (x.RetainCount, Is.EqualTo (1), "x.RetainCount");
			}
			using (var y = new FakeNativeObject ((IntPtr) 2, true)) {
				Assert.That (y.RetainCount, Is.EqualTo (0), "y.RetainCount");
			}
		}

		[Test]
		public void Dispose ()
		{
			var x = new FakeNativeObject ((IntPtr) 1, false);
			Assert.That (x.RetainCount, Is.EqualTo (1), "1");
			x.Dispose ();
			Assert.That (x.RetainCount, Is.EqualTo (0), "0");
			Assert.Throws<ObjectDisposedException> (() => x.GetCheckedHandle (), "Dispose");
			// Dispose should be safe to call multiple times
			x.Dispose ();
		}

		[Test]
		public void CreateInvalidHandle ()
		{
			Assert.Throws<Exception> (() => new NativeObjectPoker (IntPtr.Zero));
		}

		[Test]
		public void RetainAfterDispose ()
		{
			var x = new NativeObjectPoker ((IntPtr) 1);
			x.CallNative = false; // handle does not exists natively
			x.Dispose ();
			x.CallNative = true; // handle is null
			Assert.Throws<ObjectDisposedException> (() => x._Retain ());
		}

		[Test]
		public void ReleaseAfterDispose ()
		{
			var x = new NativeObjectPoker ((IntPtr) 1);
			x.CallNative = false; // handle does not exists natively
			x.Dispose ();
			x.CallNative = true; // handle is null
			Assert.Throws<ObjectDisposedException> (() => x._Release ());
		}

		[Test]
		public void Equals ()
		{
			// overriden Equals with weird behavior to confirm it can't be anything else
			using (var x = new FakeNativeObject ((IntPtr) 42, true)) {
				Assert.False (x.Equals (x), "self");
				Assert.True (x.Equals (null), "null");
			}
#if NET
			// check that equality is based on handle only (and not System.Object.Equals)
			using (var n1 = new NativeObjectPoker ((IntPtr) 42))
			using (var n2 = new NativeObjectPoker ((IntPtr) 42))
				Assert.True (n1.Equals (n2), "1");
#endif
		}

		[Test]
		public void GetHashCodeValue ()
		{
			// check that overriding GetHashCode is fine
			using (var n = new FakeNativeObject ((IntPtr) 42, true)) {
				Assert.That (n.GetHashCode, Is.EqualTo (0), "GetHashCode");
			}
#if NET
			// check that only the handle is used to compute the hash code of the selector
			// which means DisposableObject base implementation (not System.Object) is used
			using (var n = new NativeObjectPoker ((IntPtr) 42)) {
				Assert.AreNotEqual (n.GetHashCode (), RuntimeHelpers.GetHashCode (n), "GetHashCode-old");
				Assert.AreEqual (n.GetHashCode (), n.Handle.GetHashCode (), "GetHashCode-net");
			}
#endif
		}
	}
}
