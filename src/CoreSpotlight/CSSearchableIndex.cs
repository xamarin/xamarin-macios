//
// CSSearchableIndex.cs: Implements some nicer methods for CSSearchableIndex
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if IOS

using System;
using Foundation;
using ObjCRuntime;

namespace CoreSpotlight {

	[iOS (9,0)]
	public enum CSFileProtection {
		None,
		Complete,
		CompleteUnlessOpen,
		CompleteUntilFirstUserAuthentication
	}

	public partial class CSSearchableIndex {

		// Strongly typed version of initWithName:protectionClass:
		public CSSearchableIndex (string name, CSFileProtection protectionOption = CSFileProtection.None) : this (name, Translate (protectionOption))
			{}

		static NSString Translate (CSFileProtection protectionOption)
		{
			switch (protectionOption) {
			case CSFileProtection.None:
				return null;
			case CSFileProtection.Complete:
				return NSFileManager.FileProtectionComplete;
			case CSFileProtection.CompleteUnlessOpen:
				return NSFileManager.FileProtectionCompleteUnlessOpen;
			case CSFileProtection.CompleteUntilFirstUserAuthentication:
				return NSFileManager.FileProtectionCompleteUntilFirstUserAuthentication;
			default:
				throw new ArgumentOutOfRangeException ("protectionOption");
			}
		}
	}
}

#endif // IOS
