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
using System.Runtime.Versioning;

namespace NetworkExtension {
	public partial class NEVpnManager {

#if !NET
		[Mac (10,11)]
#endif
		public void SetAuthorization (Authorization authorization)
		{
			if (authorization is null)
				throw new ArgumentNullException (nameof (authorization));

			_SetAuthorization (authorization.Handle);
		}
	}
}
#endif
