#if XAMARIN_APPLETLS
//
// AppleTlsProvider.cs
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
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Mono.Security.Interface;
using MNS = Mono.Net.Security;

namespace XamCore.Security.Tls
{
	class AppleTlsProvider : MonoTlsProvider
	{
		static readonly Guid id = new Guid ("981af8af-a3a3-419a-9f01-a518e3a17c1c");

		public override string Name {
			get { return "apple-tls"; }
		}

		public override Guid ID {
			get { return id; }
		}

		public override IMonoSslStream CreateSslStream (
			Stream innerStream, bool leaveInnerStreamOpen,
			MonoTlsSettings settings = null)
		{
			return new AppleTlsStream (innerStream, leaveInnerStreamOpen, settings, this);
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

		internal override bool HasCustomSystemCertificateValidator {
			get { return true; }
		}

		internal override bool InvokeSystemCertificateValidator (
			ICertificateValidator2 validator, string targetHost, bool serverMode,
			X509CertificateCollection certificates, bool wantsChain, ref X509Chain chain,
			out bool success, ref MonoSslPolicyErrors errors, ref int status11)
		{
			if (wantsChain)
				chain = MNS.SystemCertificateValidator.CreateX509Chain (certificates);
			return MobileCertificateHelper.InvokeSystemCertificateValidator (validator, targetHost, serverMode, certificates, out success, ref errors, ref status11);
		}
	}
}
#endif
