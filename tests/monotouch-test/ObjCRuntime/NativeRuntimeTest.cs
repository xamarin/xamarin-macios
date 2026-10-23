using System.Text;

namespace MonoTouchFixtures.ObjCRuntime {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NativeRuntimeTest {
		// bool xamarin_collapse_struct_name (const char *type, char struct_name[], int max_char, GCHandle *exception_gchandle)
		[DllImport ("__Internal")]
		unsafe static extern byte xamarin_collapse_struct_name (string type, StringBuilder struct_name, int max_char, ref IntPtr exception_gchandle);

		[Test]
		public void XamarinCollapseStructName ()
		{
			AssertCollapsed ("{MKCoordinateRegion={CLLocationCoordinate2D=dd}{MKCoordinateSpan=dd}}", "dddd", "1");
			AssertCollapsed ("{CGRect=dddd}", "dddd", "2");
			AssertCollapsed ("^q", "^", "3");
			AssertCollapsed ("@?", "@", "4");
			AssertCollapsed ("^{CGRect={CGPoint=dd}{CGSize=dd}}", "^", "5");
		}

		void AssertCollapsed (string input, string expected, string message)
		{
			var sb = new StringBuilder (255);
			var exception_gchandle = IntPtr.Zero;
			var rv = xamarin_collapse_struct_name (input, sb, sb.Length - 1, ref exception_gchandle);
			Assert.That (rv, Is.Not.EqualTo (0), $"rv/{message}");
			Assert.That (exception_gchandle, Is.EqualTo (IntPtr.Zero), $"exc/{message}");
			Assert.That (sb.ToString (), Is.EqualTo (expected), $"actual/{message}");
		}

		[DllImport ("__Internal")]
		static extern int x_get_nsobject_data_with_association_lock (IntPtr handle, out IntPtr data);

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_nsobject_data_trampoline (IntPtr handle, IntPtr selector);

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_get_gchandle_with_flags (IntPtr handle, out uint flags);

		[DllImport ("__Internal")]
		static extern byte xamarin_set_gchandle_trampoline (IntPtr handle, IntPtr selector, IntPtr gchandle, uint flags, IntPtr data);

		[Test]
		public void NSObjectDataWithoutAssociationLock ()
		{
			if (!Runtime.DynamicRegistrationSupported || global::XamarinTests.ObjCRuntime.Registrar.IsStaticRegistrar)
				Assert.Ignore ("This test requires the dynamic registrar.");

			using var obj = new DynamicNSObjectData ();
			var expected = xamarin_get_nsobject_data_trampoline (obj.Handle, IntPtr.Zero);
			Assert.That (expected, Is.Not.EqualTo (IntPtr.Zero), "Initial data");

			var result = x_get_nsobject_data_with_association_lock (obj.Handle, out var actual);
			Assert.That (result, Is.EqualTo (0), "Data lookup must complete while another thread holds the association lock (1: lock not acquired, 2: lookup timed out)");
			Assert.That (actual, Is.EqualTo (expected), "Data");
		}

		[Test]
		public void NSObjectDataGCHandleUpdates ()
		{
			if (!Runtime.DynamicRegistrationSupported || global::XamarinTests.ObjCRuntime.Registrar.IsStaticRegistrar)
				Assert.Ignore ("This test requires the dynamic registrar.");

			var obj = new DynamicNSObjectData ();
			var handle = obj.Handle;
			var expected = xamarin_get_nsobject_data_trampoline (handle, IntPtr.Zero);
			Assert.That (expected, Is.Not.EqualTo (IntPtr.Zero), "Initial data");
			var gchandle = xamarin_get_gchandle_with_flags (handle, out var flags);
			Assert.That (gchandle, Is.Not.EqualTo (IntPtr.Zero), "GCHandle");

			try {
				Assert.That (xamarin_set_gchandle_trampoline (handle, IntPtr.Zero, gchandle, flags, IntPtr.Zero), Is.Not.EqualTo (0), "Update GCHandle");
				Assert.That (xamarin_get_nsobject_data_trampoline (handle, IntPtr.Zero), Is.EqualTo (expected), "Null data must preserve existing data");

				Assert.That (xamarin_set_gchandle_trampoline (handle, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero), Is.Not.EqualTo (0), "Clear GCHandle");
				Assert.That (xamarin_get_gchandle_with_flags (handle, out var clearedFlags), Is.EqualTo (IntPtr.Zero), "Cleared GCHandle");
				Assert.That (clearedFlags, Is.EqualTo (0), "Cleared flags");
				Assert.That (xamarin_get_nsobject_data_trampoline (handle, IntPtr.Zero), Is.EqualTo (expected), "Clearing a GCHandle must preserve data");
			} finally {
				xamarin_set_gchandle_trampoline (handle, IntPtr.Zero, gchandle, flags, expected);
				obj.Dispose ();
			}

			Assert.That (xamarin_get_nsobject_data_trampoline (handle, IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "Data must be removed before the native pointer can be reused");
		}

		[Register ("NativeRuntimeTest_NSObjectData")]
		class DynamicNSObjectData : NSObject {
		}
	}
}
