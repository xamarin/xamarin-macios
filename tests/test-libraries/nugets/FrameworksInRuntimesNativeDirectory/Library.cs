using System;
using System.Runtime.InteropServices;

namespace FrameworksInRuntimesNativeDirectory {
	public static class PInvokes {
		[DllImport ("@rpath/FrameworksInRuntimesNativeDirectory1.framework/FrameworksInRuntimesNativeDirectory1")]
		static extern IntPtr getFrameworksInRuntimesNativeDirectory1 ();

		public static string GetFrameworksInRuntimesNativeDirectory1 ()
		{
			return Marshal.PtrToStringAuto (getFrameworksInRuntimesNativeDirectory1 ());
		}

		[DllImport ("@rpath/FrameworksInRuntimesNativeDirectory2.framework/FrameworksInRuntimesNativeDirectory2")]
		static extern IntPtr getFrameworksInRuntimesNativeDirectory2 ();

		public static string GetFrameworksInRuntimesNativeDirectory2 ()
		{
			return Marshal.PtrToStringAuto (getFrameworksInRuntimesNativeDirectory2 ());
		}
	}
}
