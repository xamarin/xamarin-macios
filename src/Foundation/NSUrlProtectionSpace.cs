// Copyright 2013 Xamarin Inc.

using System;

using ObjCRuntime;
using Security;

namespace Foundation {

	public partial class NSUrlProtectionSpace {

		public NSUrlProtectionSpace (string host, int port, string protocol, string realm, string authenticationMethod)
			: base (NSObjectFlag.Empty)
		{
			Handle = Init (host, port, protocol, realm, authenticationMethod);
		}

		public NSUrlProtectionSpace (string host, int port, string protocol, string realm, string authenticationMethod, bool useProxy)
			: base (NSObjectFlag.Empty)
		{
			if (useProxy)
				Handle = InitWithProxy (host, port, protocol, realm, authenticationMethod);
			else
				Handle = Init (host, port, protocol, realm, authenticationMethod);
		}

		public SecTrust ServerSecTrust {
			get {
				IntPtr handle = ServerTrust;
				return (handle == IntPtr.Zero) ? null : new SecTrust (handle, false);
			}
		}

#if !XAMCORE_2_0
		[Obsolete ("Use AuthenticationMethodNTLM")]
		public static NSString AuthenticationMethodNTL {
			get { return AuthenticationMethodNTLM; }
		}

		[Obsolete ("Use AuthenticationMethodNegotiate")]
		public static NSString AuthenticationMethodNegotiat {
			get { return AuthenticationMethodNegotiate; }
		}

		[Obsolete ("Use AuthenticationMethodServerTrust")]
		public static NSString AuthenticationMethodServerTrus {
			get { return AuthenticationMethodServerTrust; }
		}
#endif
	}
}
