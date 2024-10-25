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

namespace Foundation {
	public partial class NSFileManagerDelegate {
		public virtual bool ShouldCopyItemAtPath (NSFileManager fileManager, string srcPath, string dstPath)
		{
			return ShouldCopyItemAtPath (fileManager, (NSString) srcPath, (NSString) dstPath);
		}
	}
	public static partial class NSFileManagerDelegate_Extensions {
		// This was a duplicate [Export] so in order to avoid breaking the API we expose it this way.
		// NOTE: this is an Extension method, (NSFileManagerDelegate is a [Protocol]) so the exported methods are, by default, extensions. 
		public static bool ShouldCopyItemAtPath (this INSFileManagerDelegate This, NSFileManager fileManager, string srcPath, string dstPath)
		{
			return This.ShouldCopyItemAtPath (fileManager, (NSString) srcPath, (NSString) dstPath);
		}
	}
}
