// Copyright 2015 Xamarin Inc. All rights reserved.

#nullable enable

using System;

namespace CoreFoundation {

	// note: Make sure names are identical/consistent with NSUrlError.*
	// they share the same values but there's more entries in CFNetworkErrors
	public enum CFNetworkErrors {
		/// <summary>To be added.</summary>
		HostNotFound = 1,
		/// <summary>To be added.</summary>
		HostUnknown = 2,

		/// <summary>To be added.</summary>
		SocksUnknownClientVersion = 100,
		/// <summary>To be added.</summary>
		SocksUnsupportedServerVersion = 101,
		/// <summary>To be added.</summary>
		Socks4RequestFailed = 110,
		/// <summary>To be added.</summary>
		Socks4IdentdFailed = 111,
		/// <summary>To be added.</summary>
		Socks4IdConflict = 112,
		/// <summary>To be added.</summary>
		Socks4UnknownStatusCode = 113,
		/// <summary>To be added.</summary>
		Socks5BadState = 120,
		/// <summary>To be added.</summary>
		Socks5BadResponseAddr = 121,
		/// <summary>To be added.</summary>
		Socks5BadCredentials = 122,
		/// <summary>To be added.</summary>
		Socks5UnsupportedNegotiationMethod = 123,
		/// <summary>To be added.</summary>
		Socks5NoAcceptableMethod = 124,

		/// <summary>To be added.</summary>
		FtpUnexpectedStatusCode = 200,

		/// <summary>To be added.</summary>
		HttpAuthenticationTypeUnsupported = 300,
		/// <summary>To be added.</summary>
		HttpBadCredentials = 301,
		/// <summary>To be added.</summary>
		HttpConnectionLost = 302,
		/// <summary>To be added.</summary>
		HttpParseFailure = 303,
		/// <summary>To be added.</summary>
		HttpRedirectionLoopDetected = 304,
		/// <summary>To be added.</summary>
		HttpBadURL = 305,
		/// <summary>To be added.</summary>
		HttpProxyConnectionFailure = 306,
		/// <summary>To be added.</summary>
		HttpBadProxyCredentials = 307,
		/// <summary>To be added.</summary>
		PacFileError = 308,
		/// <summary>To be added.</summary>
		PacFileAuth = 309,
		/// <summary>To be added.</summary>
		HttpsProxyConnectionFailure = 310,
		/// <summary>To be added.</summary>
		HttpsProxyFailureUnexpectedResponseToConnectMethod = 311,

		// same names as NSUrlError - begin
		/// <summary>To be added.</summary>
		BackgroundSessionInUseByAnotherProcess = -996,
		/// <summary>To be added.</summary>
		BackgroundSessionWasDisconnected = -997,
		// same names as NSUrlError - end

		/// <summary>To be added.</summary>
		Unknown = -998,

		// same names as NSUrlError - begin
		/// <summary>To be added.</summary>
		Cancelled = -999,
		/// <summary>To be added.</summary>
		BadURL = -1000,
		/// <summary>To be added.</summary>
		TimedOut = -1001,
		/// <summary>To be added.</summary>
		UnsupportedURL = -1002,
		/// <summary>To be added.</summary>
		CannotFindHost = -1003,
		/// <summary>To be added.</summary>
		CannotConnectToHost = -1004,
		/// <summary>To be added.</summary>
		NetworkConnectionLost = -1005,
		/// <summary>To be added.</summary>
		DNSLookupFailed = -1006,
		/// <summary>To be added.</summary>
		HTTPTooManyRedirects = -1007,
		/// <summary>To be added.</summary>
		ResourceUnavailable = -1008,
		/// <summary>To be added.</summary>
		NotConnectedToInternet = -1009,
		/// <summary>To be added.</summary>
		RedirectToNonExistentLocation = -1010,
		/// <summary>To be added.</summary>
		BadServerResponse = -1011,
		/// <summary>To be added.</summary>
		UserCancelledAuthentication = -1012,
		/// <summary>To be added.</summary>
		UserAuthenticationRequired = -1013,
		/// <summary>To be added.</summary>
		ZeroByteResource = -1014,
		/// <summary>To be added.</summary>
		CannotDecodeRawData = -1015,
		/// <summary>To be added.</summary>
		CannotDecodeContentData = -1016,
		/// <summary>To be added.</summary>
		CannotParseResponse = -1017,
		/// <summary>To be added.</summary>
		InternationalRoamingOff = -1018,
		/// <summary>To be added.</summary>
		CallIsActive = -1019,
		/// <summary>To be added.</summary>
		DataNotAllowed = -1020,
		/// <summary>To be added.</summary>
		RequestBodyStreamExhausted = -1021,
		/// <summary>To be added.</summary>
		AppTransportSecurityRequiresSecureConnection = -1022,

		/// <summary>To be added.</summary>
		FileDoesNotExist = -1100,
		/// <summary>To be added.</summary>
		FileIsDirectory = -1101,
		/// <summary>To be added.</summary>
		NoPermissionsToReadFile = -1102,
		/// <summary>To be added.</summary>
		DataLengthExceedsMaximum = -1103,
		/// <summary>To be added.</summary>
		FileOutsideSafeArea = -1104,

		/// <summary>To be added.</summary>
		SecureConnectionFailed = -1200,
		/// <summary>To be added.</summary>
		ServerCertificateHasBadDate = -1201,
		/// <summary>To be added.</summary>
		ServerCertificateUntrusted = -1202,
		/// <summary>To be added.</summary>
		ServerCertificateHasUnknownRoot = -1203,
		/// <summary>To be added.</summary>
		ServerCertificateNotYetValid = -1204,
		/// <summary>To be added.</summary>
		ClientCertificateRejected = -1205,
		/// <summary>To be added.</summary>
		ClientCertificateRequired = -1206,

		/// <summary>To be added.</summary>
		CannotLoadFromNetwork = -2000,

		/// <summary>To be added.</summary>
		CannotCreateFile = -3000,
		/// <summary>To be added.</summary>
		CannotOpenFile = -3001,
		/// <summary>To be added.</summary>
		CannotCloseFile = -3002,
		/// <summary>To be added.</summary>
		CannotWriteToFile = -3003,
		/// <summary>To be added.</summary>
		CannotRemoveFile = -3004,
		/// <summary>To be added.</summary>
		CannotMoveFile = -3005,
		/// <summary>To be added.</summary>
		DownloadDecodingFailedMidStream = -3006,
		/// <summary>To be added.</summary>
		DownloadDecodingFailedToComplete = -3007,
		// same names as NSUrlError - end

		/// <summary>To be added.</summary>
		CannotParseCookieFile = -4000,

		/// <summary>To be added.</summary>
		NetServiceUnknown = -72000,
		/// <summary>To be added.</summary>
		NetServiceCollision = -72001,
		/// <summary>To be added.</summary>
		NetServiceNotFound = -72002,
		/// <summary>To be added.</summary>
		NetServiceInProgress = -72003,
		/// <summary>To be added.</summary>
		NetServiceBadArgument = -72004,
		/// <summary>To be added.</summary>
		NetServiceCancel = -72005,
		/// <summary>To be added.</summary>
		NetServiceInvalid = -72006,
		/// <summary>To be added.</summary>
		NetServiceTimeout = -72007,
		NetServiceMissingRequiredConfiguration = -72008,
		/// <summary>To be added.</summary>
		NetServiceDnsServiceFailure = -73000
	}
}
