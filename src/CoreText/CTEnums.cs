// 
// Authors: Mono Team
//          Rolf Bjarne Kvinge <rolf@xamarin.com>
//     
// Copyright 2010 Novell, Inc
// Copyright 2011 - 2014, 2016 Xamarin Inc
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
using System;

using ObjCRuntime;

namespace CoreText {

	[Native]
	[ErrorDomain ("kCTFontManagerErrorDomain")]
	// defined as CFIndex (signed long [long] = nint) - /System/Library/Frameworks/CoreText.framework/Headers/CTFontManagerError.h
	public enum CTFontManagerError : long {
		None = 0,
		FileNotFount = 101,
		InsufficientPermissions = 102,
		UnrecognizedFormat = 103,
		InvalidFontData = 104,
		AlreadyRegistered = 105,
		ExceededResourceLimit = 106,
		AssetNotFound = 107,
		NotRegistered = 201,
		InUse = 202,
		SystemRequired = 203,
		RegistrationFailed = 301,
		MissingEntitlement = 302,
		InsufficientInfo = 303,
		CancelledByUser = 304,
		DuplicatedName = 305,
		InvalidFilePath = 306,
		UnsupportedScope = 307,
	}
}
