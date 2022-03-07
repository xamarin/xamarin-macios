//
// ChipKeypair.cs
//
// Authors:
//	Rachel Kang  <rachelkang@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//
#if !NET

using System;
using Security;

#nullable enable

namespace Chip {
    public partial class ChipKeypair {

#if !NET
		[Mac (12,1), Watch (8,3), TV (15,2), iOS (15,2), MacCatalyst (15,2)]
#endif
		static public SecKey? GetPubKey ()
		{
			var key = GetPubKeyRef ();
			return key == IntPtr.Zero ? null : new SecKey (key, true);
		}
    }
}
#endif // !NET
