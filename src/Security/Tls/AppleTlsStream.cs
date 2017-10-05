#if XAMARIN_APPLETLS
//
// AppleTlsStream.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2016 Xamarin, Inc.
//
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Mono.Security.Interface;
using MNS = Mono.Net.Security;

namespace XamCore.Security.Tls
{
	class AppleTlsStream : MNS.MobileAuthenticatedStream
	{
		public AppleTlsStream (Stream innerStream, bool leaveInnerStreamOpen, SslStream owner,
							   MonoTlsSettings settings, MonoTlsProvider provider)
				: base (innerStream, leaveInnerStreamOpen, owner, settings, provider)
		{
		}

		protected override MNS.MobileTlsContext CreateContext (
			bool serverMode, string targetHost, SslProtocols enabledProtocols,
			X509Certificate serverCertificate, X509CertificateCollection clientCertificates,
			bool askForClientCert)
		{
			return new AppleTlsContext (
				this, serverMode, targetHost,
				enabledProtocols, serverCertificate,
				clientCertificates, askForClientCert);
		}
	}
}
#endif
