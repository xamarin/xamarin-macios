//
// These just contain skeletons to build API contracts, the real code is included for
// some odd reson not with the "shared" setup we have here, but directly in the
// monotoch Makefile.
//
// If the above was fixed, we could do #ifdef to ifdef the files over there, but
// this is so simple, that i rather just put this here
//
// Copyright 2014 Xamarin Inc
//
// Authors: miguel de icaza
//
#if COREBUILD
using Foundation;
using ObjCRuntime;
using System;
namespace AddressBook {
	public class ABAddressBook : INativeObject {
		public IntPtr Handle { get; set; }
	}
	public class ABRecord : INativeObject {
		public IntPtr Handle { get; set; }
	}
	public class ABPerson : ABRecord {
	}
}
#endif

