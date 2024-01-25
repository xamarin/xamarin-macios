using System;
using System.Runtime.InteropServices;

using Foundation;

using ObjCRuntime;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

#if INCLUDED_ADDITIONAL_CODE
			GC.KeepAlive (typeof (AdditionalClass));
#endif

			return StaticRegistrarValidationTest ();
		}

		static int StaticRegistrarValidationTest ()
		{
			try {
				using var obj = new SomeObj ();
				obj.Whatever ();
				xamarin_IntPtr_objc_msgSend_IntPtr_ref_IntPtr_exception (obj.Handle, Selector.GetHandle ("whatever"), IntPtr.Zero, IntPtr.Zero, out var gchandle);
				Console.WriteLine ($"GCH: {gchandle}");
				if (gchandle != IntPtr.Zero) {
					var gch = GCHandle.FromIntPtr (gchandle);
					var exc = (Exception) gch.Target;
					gch.Free ();
					throw exc;
				}
				return 1; // We're not supposed to get here
			} catch (Exception e) {
				Console.WriteLine ($"E: {e}");
				if (e.Message.Contains ("The assembly MyRegistrarApp has been modified since the app was built, invalidating the generated static registrar code."))
					return 0;
				return 2;
			}
		}

		[DllImport ("__Internal")]
		static extern IntPtr xamarin_IntPtr_objc_msgSend_IntPtr_ref_IntPtr_exception (IntPtr handle, IntPtr selector, IntPtr p0, IntPtr p1, out IntPtr gchandle);
	}

	public class SomeObj : NSObject {
		[Export ("whatever")]
		public IntPtr Whatever ()
		{
			return new IntPtr (0xdeadf00d);
		}
	}

	public class DeadClass { } // Some code for the linker to remove

#if INCLUDED_ADDITIONAL_CODE
	public class AdditionalClass {
	}
#endif
}
