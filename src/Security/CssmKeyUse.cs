//
// CssmKeyUse.cs
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

	// typedef uint32 CSSM_KEYUSE;
	// Security.framework/Versions/A/Headers/cssmtype.h (OSX only)
	[Flags]
	public enum CssmKeyUse : uint {
		Encrypt              = 0x00000001,
		Decrypt              = 0x00000002,
		Sign                 = 0x00000004,
		Verify               = 0x00000008,
		SignRecover          = 0x00000010,
		VerifyRecover        = 0x00000020,
		Wrap                 = 0x00000040,
		Unwrap               = 0x00000080,
		Derive               = 0x00000100,
		Any                  = 0x80000000,
	}
}

#endif