#if XAMARIN_APPLETLS || XAMARIN_NO_TLS
//
// OldTlsProvider.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2016 Xamarin, Inc.
//
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Interface;
using MNS = Mono.Net.Security;

#if XAMARIN_NO_TLS
namespace XamCore.Security.NoTls
#else
namespace XamCore.Security.Tls
#endif
{
	class OldTlsProvider : MonoTlsProvider
	{
		static readonly Guid id = new Guid ("97d31751-d0b3-4707-99f7-a6456b972a19");

		public override Guid ID {
			get { return id; }
		}

		public override string Name {
			get { return "old-tls"; }
		}

		public override bool SupportsSslStream {
			get { return true; }
		}

		public override bool SupportsMonoExtensions {
			get { return false; }
		}

		public override bool SupportsConnectionInfo {
			get { return false; }
		}

		internal override bool SupportsTlsContext {
			get { return false; }
		}

		public override SslProtocols SupportedProtocols {
			get { return SslProtocols.Tls; }
		}

		public override IMonoSslStream CreateSslStream (
			Stream innerStream, bool leaveInnerStreamOpen,
			MonoTlsSettings settings = null)
		{
			var impl = new MNS.Private.LegacySslStream (innerStream, leaveInnerStreamOpen, this, settings);
			return new MNS.Private.MonoSslStreamImpl (impl);
		}

		internal override IMonoTlsContext CreateTlsContext (
			string hostname, bool serverMode, TlsProtocols protocolFlags,
			X509Certificate serverCertificate, X509CertificateCollection clientCertificates,
			bool remoteCertRequired, MonoEncryptionPolicy encryptionPolicy,
			MonoTlsSettings settings)
		{
			throw new NotSupportedException ();
		}
	}
}
#endif

