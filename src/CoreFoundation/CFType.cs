//
// Copyright 2012-2014 Xamarin
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace CoreFoundation {
	public class CFType {
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFGetTypeID")]
		public static extern nint GetTypeID (IntPtr typeRef);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFCopyDescription (IntPtr ptr);

		public string GetDescription (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new ArgumentNullException ("handle");
			
			using (var s = new CFString (CFCopyDescription (handle)))
				return s.ToString ();
		}
		
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint="CFEqual")]
		extern static bool CFEqual (/*CFTypeRef*/ IntPtr cf1, /*CFTypeRef*/ IntPtr cf2);

		public static bool Equal (IntPtr cf1, IntPtr cf2)
		{
			// CFEqual is not happy (but crashy) when it receive null
			if (cf1 == IntPtr.Zero)
				return cf2 == IntPtr.Zero;
			else if (cf2 == IntPtr.Zero)
				return false;
			return CFEqual (cf1, cf2);
		}
	}

	public interface ICFType : INativeObject {
	}
}
