//
// NEVpnManager.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#nullable enable

#if MONOMAC
using System;
using Foundation;
using ObjCRuntime;
using Security;

namespace NetworkExtension {
	public partial class NEVpnManager {

#if NET
		[SupportedOSPlatform ("macos10.11")]
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
#else
		[Mac (10,11)]
#endif
		public void SetAuthorization (Authorization authorization)
		{
			if (authorization is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (authorization));

			_SetAuthorization (authorization.Handle);
		}
	}
}
#endif
