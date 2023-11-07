//
// SecItemAttr.cs
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

#nullable enable

#if MONOMAC

using System;

namespace Security {

	// this is only available on OSX (not iOS)
	// typedef FourCharCode         SecItemAttr;
	//      where FourCharCode is an UInt32
	// Security.framework/Versions/A/Headers/SecKeychainItem.h
	public enum SecItemAttr : int {
		CreationDate         = 1667522932,
		ModDate              = 1835295092,
		Description          = 1684370275,
		Comment              = 1768123764,
		Creator              = 1668445298,
		Type                 = 1954115685,
		ScriptCode           = 1935897200,
		Label                = 1818321516,
		Invisible            = 1768846953,
		Negative             = 1852139361,
		CustomIcon           = 1668641641,
		Account              = 1633903476,
		Service              = 1937138533,
		Generic              = 1734700641,
		SecurityDomain       = 1935961454,
		Server               = 1936881266,
		AuthType             = 1635023216,
		Port                 = 1886351988,
		Path                 = 1885434984,
		Volume               = 1986817381,
		Address              = 1633969266,
		Signature            = 1936943463,
		Protocol             = 1886675820,
		CertificateType      = 1668577648,
		CertificateEncoding  = 1667591779,
		CrlType              = 1668445296,
		CrlEncoding          = 1668443747,
		Alias                = 1634494835,
	}
}

#endif
