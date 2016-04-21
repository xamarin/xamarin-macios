#if XAMARIN_APPLETLS
//
// MobileAuthenticatedStream.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc.
//
using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;

namespace XamCore.Security.Tls
{
	static class MobileTlsStreamFactory
	{
		internal static MobileTlsStream CreateTlsStream (MobileAuthenticatedStream parent, MonoTlsSettings settings, MobileTlsProvider provider, bool serverMode, string targetHost, SslProtocols enabledProtocols, X509Certificate serverCertificate, X509CertificateCollection clientCertificates, bool clientCertRequired)
		{
			return new AppleTlsContext (parent, settings, provider, serverMode, targetHost, enabledProtocols, serverCertificate, clientCertificates, clientCertRequired);
		}
	}
}
#endif
