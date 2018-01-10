//
// SecKeyAttribute.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2013 Jeffrey Stedfast
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

#if MONOMAC

using System;

namespace Security {

	// this is only available on OSX (not iOS) it's been deprecated
	// untyped enum (KeyItemAttributeConstants)
	// Security.framework/Versions/A/Headers/SecKey.h
	public enum SecKeyAttribute {
		KeyClass             = 0,
		PrintName            = 1,
		Alias                = 2,
		Permanent            = 3,
		Private              = 4,
		Modifiable           = 5,
		Label                = 6,
		ApplicationTag       = 7,
		KeyCreator           = 8,
		KeyType              = 9,
		KeySizeInBits        = 10,
		EffectiveKeySize     = 11,
		StartDate            = 12,
		EndDate              = 13,
		Sensitive            = 14,
		AlwaysSensitive      = 15,
		Extractable          = 16,
		NeverExtractable     = 17,
		Encrypt              = 18,
		Decrypt              = 19,
		Derive               = 20,
		Sign                 = 21,
		Verify               = 22,
		SignRecover          = 23,
		VerifyRecover        = 24,
		Wrap                 = 25,
		Unwrap               = 26,
	}
}

#endif