// 
// Authors: Mono Team
//     
// Copyright (C) 2009 Novell, Inc
// Copyright 2011-2013, 2016 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//

#if !MONOMAC

using System;

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.ObjCRuntime;

namespace XamCore.AddressBook {
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use Contacts API instead")]
	[Native]
	[ErrorDomain ("ABAddressBookErrorDomain")]
	public enum ABAddressBookError : nint {
		OperationNotPermittedByStore = 0,
		OperationNotPermittedByUserError
	}

	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use Contacts API instead")]
	[Native]
	public enum ABAuthorizationStatus : nint {
		NotDetermined = 0,
		Restricted,
		Denied,
		Authorized
	}
}

#endif