// 
// ABAddressFormatting.cs: 
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright (C) 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.Runtime.InteropServices;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AddressBookUI {
	
	// http://developer.apple.com/library/ios/#DOCUMENTATION/AddressBookUI/Reference/AddressBookUI_Functions/Reference/reference.html#//apple_ref/c/func/ABCreateStringWithAddressDictionary
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use Contacts API instead")]
	static public class ABAddressFormatting {
		
		[DllImport (Constants.AddressBookUILibrary)]
		static extern IntPtr /* NSString */ ABCreateStringWithAddressDictionary (IntPtr /* NSDictionary */ address, bool addCountryName);
		
		static public string ToString (NSDictionary address, bool addCountryName)
		{
			if (address == null)
				throw new ArgumentNullException ("address");
			
			using (NSString s = new NSString (ABCreateStringWithAddressDictionary (address.Handle, addCountryName)))
				return s.ToString ();
		}
	}
}
