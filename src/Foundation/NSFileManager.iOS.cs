//
// Extension to the NSFileManager class to allow users to set the attribute 
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved.
//

#if !MONOMAC

using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;
using XamCore.UIKit;

namespace XamCore.Foundation {
	public unsafe partial class NSFileManager {
#if IOS
		// FIXME keep [get|set|remove]xattr code base for iOS 5.0.1 compatibility - remove when support is removed from iOS
		[DllImport ("/usr/lib/system/libsystem_kernel.dylib", SetLastError=true)]
		extern static int setxattr (string path, string name, IntPtr value, nint size /* size_t */, int position /* uint32_t */, int options /* int */); // returns: int

		[DllImport ("/usr/lib/system/libsystem_kernel.dylib", SetLastError=true)]
		extern static int getxattr (string path, string name, IntPtr value, nint size /* size_t */, int position /* uint32_t */, int options /* int */); // returns: int
                
		[DllImport ("/usr/lib/system/libsystem_kernel.dylib", SetLastError=true)]
		extern static int removexattr (string path, string name, int options /* int */); // returns: int

		const string key = "com.apple.MobileBackup";
		
		static bool xattr_compatibility;
#endif

		static NSFileManager ()
		{
#if IOS
			// not available before 5.0.1, deprecated in 5.1
			// but we want to use the former xattr code path for any earlier release
			xattr_compatibility = !UIDevice.CurrentDevice.CheckSystemVersion (5, 1);
#endif
		}
		
		public static NSError SetSkipBackupAttribute (string filename, bool skipBackup)
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");
			
#if IOS
			if (xattr_compatibility) {
				if (skipBackup) {
					byte attrValue = 1;
					byte *p = &attrValue;
					int code = setxattr (filename, key, (IntPtr) p, 1, 0, 0);
					if (code == 0)
						return null;
				} else {
					if (removexattr (filename, key, 0) == 0)
						return null;
				}
				return new NSError (NSError.PosixErrorDomain, Marshal.GetLastWin32Error ());
			} else {
#endif // IOS
				using (NSUrl url = NSUrl.FromFilename (filename)) {
					NSError error;
					url.SetResource (NSUrl.IsExcludedFromBackupKey, (NSNumber) (skipBackup ? 1 : 0), out error);
					return error;
				}
#if IOS
			}
#endif // IOS
		}

		public static bool GetSkipBackupAttribute (string filename)
		{
			NSError err;

			return GetSkipBackupAttribute (filename, out err);
		}
		
		public static bool GetSkipBackupAttribute (string filename, out NSError error)
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");
			
#if IOS
			if (xattr_compatibility) {
				byte attrValue = 0;
				unsafe {
					byte *p = &attrValue;

					int code = getxattr (filename, key, (IntPtr) p, 1, 0, 0);
					if (code == -1) {
						error = new NSError (NSError.PosixErrorDomain, Marshal.GetLastWin32Error ());
						return false;
					}
					error = null;
					return attrValue != 0;
				}
			} else {
#endif // IOS
				using (NSUrl url = NSUrl.FromFilename (filename)) {
					NSObject value;
					url.TryGetResource (NSUrl.IsExcludedFromBackupKey, out value, out error);
					return (error == null) && ((long) ((NSNumber) value) == 1);
				}
#if IOS
			}
#endif // IOS
		}
	}
}

#endif // !MONOMAC

