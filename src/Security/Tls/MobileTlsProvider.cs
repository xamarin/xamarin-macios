#if XAMARIN_APPLETLS
//
// MobileTlsProvider.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc.
//
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

using SSA = System.Security.Authentication;
using SslProtocols = System.Security.Authentication.SslProtocols;

using Mono.Security.Interface;

namespace XamCore.Security.Tls
{
	abstract class MobileTlsProvider : MonoTlsProvider
	{
		public override IMonoSslStream CreateSslStream (
			Stream innerStream, bool leaveInnerStreamOpen,
			MonoTlsSettings settings = null)
		{
			return new MobileAuthenticatedStream (innerStream, leaveInnerStreamOpen, settings, this);
		}

		internal override IMonoTlsContext CreateTlsContext (
			string hostname, bool serverMode, TlsProtocols protocolFlags,
			X509Certificate serverCertificate, X509CertificateCollection clientCertificates,
			bool remoteCertRequired, MonoEncryptionPolicy encryptionPolicy,
			MonoTlsSettings settings)
		{
			throw new NotSupportedException ();
		}

		public override bool SupportsSslStream {
			get { return true; }
		}

		public override bool SupportsMonoExtensions {
			get { return true; }
		}

		public override bool SupportsConnectionInfo {
			get { return true; }
		}

		public override SslProtocols SupportedProtocols {
			get { return SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls; }
		}

		internal override bool SupportsTlsContext {
			get { return false; }
		}
	}
}
#endif
