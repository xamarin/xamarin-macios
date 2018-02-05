//
// Extension to the NSFileManager class to allow users to set the attribute 
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved.
//

#if !MONOMAC

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using UIKit;

namespace Foundation {
	public unsafe partial class NSFileManager {
		
		public static NSError SetSkipBackupAttribute (string filename, bool skipBackup)
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");
			
			using (NSUrl url = NSUrl.FromFilename (filename)) {
				NSError error;
				url.SetResource (NSUrl.IsExcludedFromBackupKey, (NSNumber) (skipBackup ? 1 : 0), out error);
				return error;
			}
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
			
			using (NSUrl url = NSUrl.FromFilename (filename)) {
				NSObject value;
				url.TryGetResource (NSUrl.IsExcludedFromBackupKey, out value, out error);
				return (error == null) && ((long) ((NSNumber) value) == 1);
			}
		}
	}
}

#endif // !MONOMAC

