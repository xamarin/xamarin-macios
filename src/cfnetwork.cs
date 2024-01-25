//
// cfnetwork.cs: Definitions for CFNetwork
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;

using Foundation;

using ObjCRuntime;

// Both CFHttpStream and CFHTTPMessage are in CFNetwork.framework, no idea why they ended up in CoreServices when they were bound.
#if NET
namespace CFNetwork {
#else
namespace CoreServices {
#endif

	[Partial]
	interface CFHTTPStream {

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPAttemptPersistentConnection", "CFNetwork")]
		NSString _AttemptPersistentConnection { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPFinalURL", "CFNetwork")]
		NSString _FinalURL { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPFinalRequest", "CFNetwork")]
		NSString _FinalRequest { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPProxy", "CFNetwork")]
		NSString _Proxy { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPRequestBytesWrittenCount", "CFNetwork")]
		NSString _RequestBytesWrittenCount { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPResponseHeader", "CFNetwork")]
		NSString _ResponseHeader { get; }

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Internal]
		[Field ("kCFStreamPropertyHTTPShouldAutoredirect", "CFNetwork")]
		NSString _ShouldAutoredirect { get; }
	}

	[Partial]
	interface CFHTTPMessage {

		[Internal]
		[Field ("kCFHTTPVersion1_0", "CFNetwork")]
		IntPtr _HTTPVersion1_0 { get; }

		[Internal]
		[Field ("kCFHTTPVersion1_1", "CFNetwork")]
		IntPtr _HTTPVersion1_1 { get; }

		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCFHTTPVersion2_0", "CFNetwork")]
		IntPtr _HTTPVersion2_0 { get; }

		[Mac (11, 0), iOS (14, 0), TV (14, 0)]
		[MacCatalyst (14, 0)]
		[Internal]
		[Field ("kCFHTTPVersion3_0", "CFNetwork")]
		IntPtr _HTTPVersion3_0 { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationSchemeBasic", "CFNetwork")]
		IntPtr _AuthenticationSchemeBasic { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationSchemeNegotiate", "CFNetwork")]
		IntPtr _AuthenticationSchemeNegotiate { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationSchemeNTLM", "CFNetwork")]
		IntPtr _AuthenticationSchemeNTLM { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationSchemeDigest", "CFNetwork")]
		IntPtr _AuthenticationSchemeDigest { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationUsername", "CFNetwork")]
		NSString _AuthenticationUsername { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationPassword", "CFNetwork")]
		NSString _AuthenticationPassword { get; }

		[Internal]
		[Field ("kCFHTTPAuthenticationAccountDomain", "CFNetwork")]
		NSString _AuthenticationAccountDomain { get; }

		// misdocumented by Apple (feedback left)
		// OSX headers says it's 10.9 only
		// iOS headers says it's iOS 7.0 only (but comments talks about OSX)
		// yet both 7.0+ and 10.9 returns null
		[MacCatalyst (13, 1)]
		[Internal]
		[Field ("kCFHTTPAuthenticationSchemeOAuth1", "CFNetwork")]
		IntPtr _AuthenticationSchemeOAuth1 { get; }
	}
}
