// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//
// An in-process TLS (HTTPS) server used by the networking tests. It uses a self-signed
// certificate and returns a minimal "200 OK" response to any request, which is enough to
// exercise the client's TLS handling (server trust, client certificates, proxy tunneling, ...).
//

using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

using CoreFoundation;
using Network;
using Security;

namespace MonoTests.System.Net.Http {
	static class TlsTestServer {
		// Creates a secure (TLS) NWListener bound to 127.0.0.1 on an available port. The listener
		// answers every connection with a minimal "HTTP/1.1 200 OK" response and then closes it.
		public static NWListener CreateNWTlsListener (bool requireClientCert)
		{
			var (pfxData, pfxPassword) = CreateSelfSignedServerCertificatePfx ();
			using var secIdentity = SecIdentity.Import (pfxData, pfxPassword);
			using var secIdentity2 = new SecIdentity2 (secIdentity);
			using var readyEvent = new ManualResetEventSlim (false);
			NWError? listenerError = null;

			var parameters = NWParameters.CreateSecureTcp (
				configureTls: tlsOptions => {
					var tls = (NWProtocolTlsOptions) tlsOptions;
					var secOptions = tls.ProtocolOptions;
					secOptions.SetLocalIdentity (secIdentity2);
					secOptions.SetPeerAuthenticationRequired (requireClientCert);
				});
			using var localEndpoint = NWEndpoint.Create ("127.0.0.1", "0");
			parameters.LocalEndpoint = localEndpoint;

			var listener = NWListener.Create (parameters);
			parameters.Dispose ();

			listener.SetQueue (DispatchQueue.DefaultGlobalQueue);

			listener.SetStateChangedHandler ((state, error) => {
				if (state == NWListenerState.Failed)
					listenerError = error;
				if (state == NWListenerState.Ready || state == NWListenerState.Failed)
					readyEvent.Set ();
			});

			listener.SetNewConnectionHandler (connection => {
				connection.SetQueue (DispatchQueue.DefaultGlobalQueue);
				connection.SetStateChangeHandler ((connState, connError) => {
					if (connState == NWConnectionState.Ready) {
						// Read the HTTP request (just consume it), then send a response
						connection.ReceiveReadOnlyData (1, 4096, (data, context, isComplete, error) => {
							var response = Encoding.UTF8.GetBytes ("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nOK");
							connection.Send (response, NWContentContext.FinalMessage, true, sendError => {
								connection.Cancel ();
							});
						});
					}
				});
				connection.Start ();
			});

			listener.Start ();

			if (!readyEvent.Wait (TimeSpan.FromSeconds (10)))
				throw new TimeoutException ("NWListener did not become ready in time.");

			if (listenerError is not null)
				throw new InvalidOperationException ($"NWListener failed to start: {listenerError}");

			return listener;
		}

		public static (byte [] Data, string Password) CreateSelfSignedServerCertificatePfx ()
		{
			using var rsa = RSA.Create (2048);
			var certRequest = new CertificateRequest (
				"CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
			var sanBuilder = new SubjectAlternativeNameBuilder ();
			sanBuilder.AddIpAddress (IPAddress.Loopback);
			sanBuilder.AddDnsName ("localhost");
			certRequest.CertificateExtensions.Add (sanBuilder.Build ());
			var cert = certRequest.CreateSelfSigned (DateTimeOffset.UtcNow.AddDays (-1), DateTimeOffset.UtcNow.AddYears (1));
			var password = Guid.NewGuid ().ToString ();
			return (cert.Export (X509ContentType.Pfx, password), password);
		}
	}
}
