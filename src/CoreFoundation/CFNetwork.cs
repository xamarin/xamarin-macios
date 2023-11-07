// Copyright 2015 Xamarin Inc. All rights reserved.

#nullable enable

using System;

namespace CoreFoundation {

	// note: Make sure names are identical/consistent with NSUrlError.*
	// they share the same values but there's more entries in CFNetworkErrors
	public enum CFNetworkErrors {
		HostNotFound = 1,
		HostUnknown = 2,

		SocksUnknownClientVersion = 100,
		SocksUnsupportedServerVersion = 101,
		Socks4RequestFailed = 110,
		Socks4IdentdFailed = 111,
		Socks4IdConflict = 112,
		Socks4UnknownStatusCode = 113,
		Socks5BadState = 120,
		Socks5BadResponseAddr = 121,
		Socks5BadCredentials = 122,
		Socks5UnsupportedNegotiationMethod = 123,
		Socks5NoAcceptableMethod = 124,

		FtpUnexpectedStatusCode = 200,

		HttpAuthenticationTypeUnsupported = 300,
		HttpBadCredentials = 301,
		HttpConnectionLost = 302,
		HttpParseFailure = 303,
		HttpRedirectionLoopDetected = 304,
		HttpBadURL = 305,
		HttpProxyConnectionFailure = 306,
		HttpBadProxyCredentials = 307,
		PacFileError = 308,
		PacFileAuth = 309,
		HttpsProxyConnectionFailure = 310,
		HttpsProxyFailureUnexpectedResponseToConnectMethod = 311,

		// same names as NSUrlError - begin
		BackgroundSessionInUseByAnotherProcess = -996,
		BackgroundSessionWasDisconnected = -997,
		// same names as NSUrlError - end

		Unknown = -998,

		// same names as NSUrlError - begin
		Cancelled = -999,
		BadURL = -1000,
		TimedOut = -1001,
		UnsupportedURL = -1002,
		CannotFindHost = -1003,
		CannotConnectToHost = -1004,
		NetworkConnectionLost = -1005,
		DNSLookupFailed = -1006,
		HTTPTooManyRedirects = -1007,
		ResourceUnavailable = -1008,
		NotConnectedToInternet = -1009,
		RedirectToNonExistentLocation = -1010,
		BadServerResponse = -1011,
		UserCancelledAuthentication = -1012,
		UserAuthenticationRequired = -1013,
		ZeroByteResource = -1014,
		CannotDecodeRawData = -1015,
		CannotDecodeContentData = -1016,
		CannotParseResponse = -1017,
		InternationalRoamingOff = -1018,
		CallIsActive = -1019,
		DataNotAllowed = -1020,
		RequestBodyStreamExhausted = -1021,
		AppTransportSecurityRequiresSecureConnection = -1022,

		FileDoesNotExist = -1100,
		FileIsDirectory = -1101,
		NoPermissionsToReadFile = -1102,
		DataLengthExceedsMaximum = -1103,
		FileOutsideSafeArea = -1104,

		SecureConnectionFailed = -1200,
		ServerCertificateHasBadDate = -1201,
		ServerCertificateUntrusted = -1202,
		ServerCertificateHasUnknownRoot = -1203,
		ServerCertificateNotYetValid = -1204,
		ClientCertificateRejected = -1205,
		ClientCertificateRequired = -1206,

		CannotLoadFromNetwork = -2000,

		CannotCreateFile = -3000,
		CannotOpenFile = -3001,
		CannotCloseFile = -3002,
		CannotWriteToFile = -3003,
		CannotRemoveFile = -3004,
		CannotMoveFile = -3005,
		DownloadDecodingFailedMidStream = -3006,
		DownloadDecodingFailedToComplete = -3007,
		// same names as NSUrlError - end

		CannotParseCookieFile = -4000,

		NetServiceUnknown = -72000,
		NetServiceCollision = -72001,
		NetServiceNotFound = -72002,
		NetServiceInProgress = -72003,
		NetServiceBadArgument = -72004,
		NetServiceCancel = -72005,
		NetServiceInvalid = -72006,
		NetServiceTimeout = -72007,
		NetServiceMissingRequiredConfiguration = -72008,
		NetServiceDnsServiceFailure = -73000
	}
}
