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

#nullable enable

#if COREBUILD
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using System;
namespace AddressBook {
	public class ABAddressBook : NativeObject {
	}
	public class ABRecord : NativeObject {
	}
	public class ABPerson : ABRecord {
	}
}
#endif
