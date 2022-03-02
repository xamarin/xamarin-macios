//
// NSLayoutConstraint.cs:
//
// Authors:
//  Paola Villarreal <paola.villarreal@xamarin.com>
//
// Copyright 2015, Xamarin Inc
//

using System;
using Foundation;

namespace Foundation
{
	public partial class NSFileManagerDelegate {
		public virtual bool ShouldCopyItemAtPath (NSFileManager fileManager, string srcPath, string dstPath)
		{
			return ShouldCopyItemAtPath (fileManager, (NSString) srcPath, (NSString) dstPath);
		}

#if !XAMCORE_3_0
		[Obsolete ("API removed after iOS 2.0 / macOS 10.5. It will never be called by the OS.")]
		public virtual bool ShouldProceedAfterError (NSFileManager fm, NSDictionary errorInfo)
		{
			return false;
		}
#endif
	}	
	public static partial class NSFileManagerDelegate_Extensions  {
		// This was a duplicate [Export] so in order to avoid breaking the API we expose it this way.
		// NOTE: this is an Extension method, (NSFileManagerDelegate is a [Protocol]) so the exported methods are, by default, extensions. 
		public static bool ShouldCopyItemAtPath (this INSFileManagerDelegate This, NSFileManager fileManager, string srcPath, string dstPath)
		{
			return This.ShouldCopyItemAtPath (fileManager, (NSString) srcPath, (NSString) dstPath);
		}

#if !XAMCORE_3_0
		[Obsolete ("API removed after iOS 2.0 / macOS 10.5. It will never be called by the OS.")]
		public static bool ShouldProceedAfterError (INSFileManagerDelegate This, NSFileManager fm, NSDictionary errorInfo)
		{
			return false;
		}
#endif
	}	
}
