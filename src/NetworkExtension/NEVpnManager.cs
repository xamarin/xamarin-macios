//
// NEVpnManager.cs
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && MONOMAC
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.Security;

namespace XamCore.NetworkExtension {
	public partial class NEVpnManager {

		[Mac (10,11)]
		public void SetAuthorization (Authorization authorization)
		{
			if (authorization == null)
				throw new ArgumentNullException (nameof (authorization));

			_SetAuthorization (authorization.Handle);
		}
	}
}
#endif
