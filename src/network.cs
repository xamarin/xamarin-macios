using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace Network {

	[TV (12,0), Mac (10,14), iOS (12,0)]
	public enum NWErrorDomain {
		Invalid = 0,
		[Field ("kNWErrorDomainPOSIX")]
		Posix = 1,
		[Field ("kNWErrorDomainDNS")]
		Dns = 2,
		[Field ("kNWErrorDomainTLS")]
		Tls = 3,
	}
}