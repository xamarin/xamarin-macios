#if __MACOS__
using System;
using System.Runtime.InteropServices;

using AppKit;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MonoMacFixtures.AppKit {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CellTest {
		[Test]
		public void CopyTest ()
		{
			using (var cell = new CustomCell ())
				Check (cell.Handle);
		}

		[Test]
		public void CopyDerivedTest ()
		{
			using (var cell = new DerivedCell ())
				Check (cell.Handle);
		}

		void Check (IntPtr cell_handle)
		{
			var clone_ptr = IntPtr_objc_msgSend (cell_handle, Selector.GetHandle ("copyWithZone:"), IntPtr.Zero);
			//			Console.WriteLine ("Created cell 0x{0} (GCHandle: 0x{2}) with clone 0x{1} (GCHandle: 0x{3})", cell_handle.ToString ("x"), clone_ptr.ToString ("x"), GetGCHandle (cell_handle).ToString ("x"), GetGCHandle (clone_ptr).ToString ("x"));

			Assert.AreNotEqual (GetGCHandle (cell_handle), GetGCHandle (clone_ptr), "gchandle #1");
			CustomCell.expectedHandle = cell_handle;
			objc_msgSend (Class.GetHandle (typeof (CustomCell)), Selector.GetHandle ("foo:"), cell_handle);

			Assert.AreNotEqual (GetGCHandle (cell_handle), GetGCHandle (clone_ptr), "gchandle #2");
			CustomCell.expectedHandle = clone_ptr;
			objc_msgSend (Class.GetHandle (typeof (CustomCell)), Selector.GetHandle ("foo:"), clone_ptr);

			Assert.AreNotEqual (GetGCHandle (cell_handle), GetGCHandle (clone_ptr), "gchandle #3");

			objc_msgSend (clone_ptr, Selector.GetHandle ("release"));
		}

		[DllImport ("__Internal", EntryPoint = "xamarin_get_gchandle")]
		extern static int GetGCHandle (IntPtr ptr);

		const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector, IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void objc_msgSend (IntPtr receiver, IntPtr selector, IntPtr p1);

		[DllImport (LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		public extern static void objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport (LIBOBJC_DYLIB)]
		internal extern static IntPtr object_getInstanceVariable (IntPtr cls, string name, out IntPtr value);
	}

	class CustomCell : NSCell {
		public static NativeHandle expectedHandle;

		public CustomCell (NativeHandle ptr) : base (ptr) { }
		public CustomCell () { }

		[Export ("foo:")]
		public static void Foo (CustomCell mySelf)
		{
			Assert.AreEqual (expectedHandle, mySelf.Handle, "Handle");
		}
	}

	class DerivedCell : CustomCell {
		public DerivedCell (NativeHandle ptr) : base (ptr) { }
		public DerivedCell () { }

		public override NSObject Copy (NSZone zone)
		{
			return base.Copy (zone);
		}
	}
}

#endif // __MACOS__
