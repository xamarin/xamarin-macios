#if XAMARIN_APPLETLS
#if XAMARIN_NO_TLS
#error THIS SHOULD NEVER HAPPEN!!!
#endif
//
// MobileTlsStream.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc.
//
using System;
using System.IO;
using System.Linq;
using SD = System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

using MX = Mono.Security.X509;
using Mono.Security.Interface;

namespace XamCore.Security.Tls
{
	abstract class MobileTlsStream : IDisposable
	{
		MonoTlsSettings settings;
		MobileTlsProvider provider;
		
		public MobileTlsStream (MonoTlsSettings settings, MobileTlsProvider provider)
		{
			this.settings = settings;
			this.provider = provider;
		}

		public MonoTlsSettings Settings {
			get { return settings; }
		}

		public MobileTlsProvider Provider {
			get { return provider; }
		}

		[SD.Conditional ("MARTIN_DEBUG")]
		protected void Debug (string message, params object[] args)
		{
			Console.Error.WriteLine ("MobileTlsStream: {0}", string.Format (message, args));
		}

		public abstract bool HasContext {
			get;
		}

		public abstract bool IsAuthenticated {
			get;
		}

		public abstract bool IsServer {
			get;
		}

		public abstract void StartHandshake ();

		public abstract bool ProcessHandshake ();

		public abstract void FinishHandshake ();

		public abstract MonoTlsConnectionInfo ConnectionInfo {
			get;
		}

		internal abstract X509Certificate LocalServerCertificate {
			get;
		}

		internal abstract bool IsRemoteCertificateAvailable {
			get;
		}

		internal abstract X509Certificate LocalClientCertificate {
			get;
		}

		public abstract X509Certificate RemoteCertificate {
			get;
		}

		public abstract TlsProtocols NegotiatedProtocol {
			get;
		}

		public abstract void Flush ();

		public abstract int Read (byte[] buffer, int offset, int count, out bool wantMore);

		public abstract int Write (byte[] buffer, int offset, int count, out bool wantMore);

		public abstract void Close ();

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
		}

		~MobileTlsStream ()
		{
			Dispose (false);
		}
	}
}
#endif
