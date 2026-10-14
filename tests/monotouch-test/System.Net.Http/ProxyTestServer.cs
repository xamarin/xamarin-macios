// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//
// An in-process HTTP forwarding proxy used to test NSUrlSessionHandler's proxy support.
//
// It handles absolute-form HTTP requests (the form a client sends to an HTTP proxy) to test
// proxying of the in-process HTTP test server (HttpbinTestServer), and it handles the CONNECT
// method to tunnel HTTPS requests (which is the only way NSUrlSession delivers proxy
// authentication challenges to the delegate).
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace MonoTests.System.Net.Http {
	[Preserve (AllMembers = true)]
	sealed class ProxyTestServer : IDisposable {
		readonly TcpListener listener;
		readonly string? requiredUser;
		readonly string? requiredPassword;
		readonly int forceTunnelPort;
		int requestCount;
		int authenticatedRequestCount;

		// The total number of requests received by the proxy (including any that were rejected with a 407).
		public int RequestCount => Volatile.Read (ref requestCount);

		// The number of requests that were successfully authenticated and forwarded.
		public int AuthenticatedRequestCount => Volatile.Read (ref authenticatedRequestCount);

		public int Port { get; }

		public string Url => $"http://127.0.0.1:{Port}";

		// forceTunnelPort: when non-zero, every CONNECT request is tunneled to 127.0.0.1:forceTunnelPort
		// regardless of the requested target host. This lets a test request an HTTPS URL with a non-local
		// hostname (so CFNetwork actually routes it through the proxy instead of bypassing the proxy for
		// localhost destinations) while still tunneling to a local TLS test server.
		public ProxyTestServer (string? requiredUser = null, string? requiredPassword = null, int forceTunnelPort = 0)
		{
			this.requiredUser = requiredUser;
			this.requiredPassword = requiredPassword;
			this.forceTunnelPort = forceTunnelPort;

			listener = new TcpListener (IPAddress.Loopback, 0);
			listener.Start ();
			Port = ((IPEndPoint) listener.LocalEndpoint).Port;
			_ = Task.Run (AcceptLoop);
		}

		async Task AcceptLoop ()
		{
			try {
				while (true) {
					var client = await listener.AcceptTcpClientAsync ().ConfigureAwait (false);
					_ = Task.Run (() => HandleClientSafe (client));
				}
			} catch (ObjectDisposedException) {
				// the listener was stopped
			} catch (SocketException) {
				// the listener was stopped
			}
		}

		async Task HandleClientSafe (TcpClient client)
		{
			try {
				using (client)
				using (var stream = client.GetStream ()) {
					await HandleClient (stream).ConfigureAwait (false);
				}
			} catch (Exception e) {
				// This is a test proxy, so don't let errors bring down the process, but log them so
				// test failures (which often manifest as timeouts) are actionable.
				Console.WriteLine ($"ProxyTestServer: error handling client: {e}");
			}
		}

		async Task HandleClient (NetworkStream stream)
		{
			var requestLine = await ReadLineAsync (stream).ConfigureAwait (false);
			if (string.IsNullOrEmpty (requestLine))
				return;

			var parts = requestLine.Split (' ');
			if (parts.Length < 3)
				return;

			var method = parts [0];
			var target = parts [1];

			var headers = new List<KeyValuePair<string, string>> ();
			while (true) {
				var line = await ReadLineAsync (stream).ConfigureAwait (false);
				if (string.IsNullOrEmpty (line))
					break;
				var idx = line.IndexOf (':');
				if (idx > 0)
					headers.Add (new KeyValuePair<string, string> (line.Substring (0, idx).Trim (), line.Substring (idx + 1).Trim ()));
			}

			if (string.Equals (method, "CONNECT", StringComparison.OrdinalIgnoreCase)) {
				Interlocked.Increment (ref requestCount);
				await HandleConnect (stream, target, headers).ConfigureAwait (false);
				return;
			}

			var body = await ReadBodyAsync (stream, headers).ConfigureAwait (false);

			Interlocked.Increment (ref requestCount);

			if (requiredUser is not null) {
				var proxyAuth = FindHeader (headers, "Proxy-Authorization");
				if (!IsValidProxyAuth (proxyAuth)) {
					await WriteResponseAsync (stream, 407, "Proxy Authentication Required",
						new List<KeyValuePair<string, string>> {
							new ("Proxy-Authenticate", "Basic realm=\"Test Proxy\""),
						},
						Encoding.UTF8.GetBytes ("Proxy authentication required")).ConfigureAwait (false);
					return;
				}
			}

			if (!Uri.TryCreate (target, UriKind.Absolute, out var targetUri) || targetUri.Scheme != Uri.UriSchemeHttp) {
				await WriteResponseAsync (stream, 400, "Bad Request", new List<KeyValuePair<string, string>> (),
					Encoding.UTF8.GetBytes ("Only absolute-form HTTP requests are supported")).ConfigureAwait (false);
				return;
			}

			Interlocked.Increment (ref authenticatedRequestCount);

			await ForwardAsync (stream, method, targetUri, headers, body).ConfigureAwait (false);
		}

		// Handles the CONNECT method: validate proxy authentication (if required), then establish a
		// raw TCP tunnel to the requested host:port and pipe bytes back and forth. This is what lets
		// an HTTPS request flow through the proxy while exercising proxy authentication.
		async Task HandleConnect (NetworkStream clientStream, string target, List<KeyValuePair<string, string>> headers)
		{
			if (requiredUser is not null) {
				var proxyAuth = FindHeader (headers, "Proxy-Authorization");
				if (!IsValidProxyAuth (proxyAuth)) {
					await WriteResponseAsync (clientStream, 407, "Proxy Authentication Required",
						new List<KeyValuePair<string, string>> {
							new ("Proxy-Authenticate", "Basic realm=\"Test Proxy\""),
						},
						Encoding.UTF8.GetBytes ("Proxy authentication required")).ConfigureAwait (false);
					return;
				}
			}

			var host = target;
			var port = 443;
			var colonIdx = target.LastIndexOf (':');
			if (colonIdx > 0) {
				host = target.Substring (0, colonIdx);
				int.TryParse (target.Substring (colonIdx + 1), out port);
			}
			if (forceTunnelPort != 0) {
				// Ignore the requested target and always tunnel to the local TLS test server.
				host = "127.0.0.1";
				port = forceTunnelPort;
			} else if (string.Equals (host, "localhost", StringComparison.OrdinalIgnoreCase)) {
				// The TLS test server binds to 127.0.0.1, so make sure we connect there (and not ::1).
				host = "127.0.0.1";
			}

			using var upstream = new TcpClient ();
			await upstream.ConnectAsync (host, port).ConfigureAwait (false);

			Interlocked.Increment (ref authenticatedRequestCount);

			var established = Encoding.ASCII.GetBytes ("HTTP/1.1 200 Connection Established\r\n\r\n");
			await clientStream.WriteAsync (established, 0, established.Length).ConfigureAwait (false);
			await clientStream.FlushAsync ().ConfigureAwait (false);

			using var upstreamStream = upstream.GetStream ();
			var clientToUpstream = clientStream.CopyToAsync (upstreamStream);
			var upstreamToClient = upstreamStream.CopyToAsync (clientStream);
			await Task.WhenAny (clientToUpstream, upstreamToClient).ConfigureAwait (false);
		}

		static async Task<byte []?> ReadBodyAsync (NetworkStream stream, List<KeyValuePair<string, string>> headers)
		{
			var contentLength = FindHeader (headers, "Content-Length");
			if (contentLength is null || !int.TryParse (contentLength, out var length) || length <= 0)
				return null;

			var body = new byte [length];
			var read = 0;
			while (read < length) {
				var r = await stream.ReadAsync (body, read, length - read).ConfigureAwait (false);
				if (r <= 0)
					break;
				read += r;
			}
			// If the client disconnected early, return only the bytes we actually read (rather than a
			// buffer padded with trailing zero bytes).
			if (read < length)
				Array.Resize (ref body, read);
			return body;
		}

		async Task ForwardAsync (NetworkStream clientStream, string method, Uri targetUri, List<KeyValuePair<string, string>> headers, byte []? body)
		{
			// Explicitly avoid using any proxy for the forwarded request, so we don't accidentally loop back into ourselves.
			using var handler = new SocketsHttpHandler {
				UseProxy = false,
				AllowAutoRedirect = false,
				AutomaticDecompression = DecompressionMethods.None,
			};
			using var client = new HttpClient (handler);
			using var request = new HttpRequestMessage (new HttpMethod (method), targetUri);

			if (body is not null)
				request.Content = new ByteArrayContent (body);

			foreach (var header in headers) {
				if (IsHopByHopHeader (header.Key))
					continue;
				if (string.Equals (header.Key, "Host", StringComparison.OrdinalIgnoreCase))
					continue;
				// Content-Length is set automatically by ByteArrayContent.
				if (string.Equals (header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase))
					continue;

				if (!request.Headers.TryAddWithoutValidation (header.Key, header.Value))
					request.Content?.Headers.TryAddWithoutValidation (header.Key, header.Value);
			}

			using var response = await client.SendAsync (request).ConfigureAwait (false);

			var responseHeaders = new List<KeyValuePair<string, string>> {
				// A marker header so tests can verify the response actually went through this proxy.
				new ("Via-Test-Proxy", "true"),
			};
			foreach (var header in response.Headers) {
				foreach (var value in header.Value)
					responseHeaders.Add (new (header.Key, value));
			}

			var content = await response.Content.ReadAsByteArrayAsync ().ConfigureAwait (false);

			foreach (var header in response.Content.Headers) {
				// We compute our own Content-Length below, and Transfer-Encoding is hop-by-hop.
				if (string.Equals (header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase))
					continue;
				foreach (var value in header.Value)
					responseHeaders.Add (new (header.Key, value));
			}

			await WriteResponseAsync (clientStream, (int) response.StatusCode, response.ReasonPhrase ?? "", responseHeaders, content).ConfigureAwait (false);
		}

		bool IsValidProxyAuth (string? proxyAuthorization)
		{
			if (proxyAuthorization is null || !proxyAuthorization.StartsWith ("Basic ", StringComparison.Ordinal))
				return false;

			try {
				var credentials = Encoding.UTF8.GetString (Convert.FromBase64String (proxyAuthorization.Substring ("Basic ".Length)));
				var colonIdx = credentials.IndexOf (':');
				if (colonIdx <= 0)
					return false;

				var user = credentials.Substring (0, colonIdx);
				var password = credentials.Substring (colonIdx + 1);
				return user == requiredUser && password == requiredPassword;
			} catch (FormatException) {
				// Invalid base64.
				return false;
			}
		}

		static string? FindHeader (List<KeyValuePair<string, string>> headers, string name)
		{
			foreach (var header in headers) {
				if (string.Equals (header.Key, name, StringComparison.OrdinalIgnoreCase))
					return header.Value;
			}
			return null;
		}

		static bool IsHopByHopHeader (string name)
		{
			switch (name.ToLowerInvariant ()) {
			case "connection":
			case "keep-alive":
			case "proxy-authenticate":
			case "proxy-authorization":
			case "te":
			case "trailer":
			case "transfer-encoding":
			case "upgrade":
				return true;
			default:
				return false;
			}
		}

		static async Task<string?> ReadLineAsync (NetworkStream stream)
		{
			var builder = new StringBuilder ();
			var buffer = new byte [1];
			while (true) {
				var read = await stream.ReadAsync (buffer, 0, 1).ConfigureAwait (false);
				if (read <= 0)
					return builder.Length == 0 ? null : builder.ToString ();

				var c = (char) buffer [0];
				if (c == '\n')
					break;
				if (c != '\r')
					builder.Append (c);
			}
			return builder.ToString ();
		}

		static async Task WriteResponseAsync (NetworkStream stream, int statusCode, string reasonPhrase, List<KeyValuePair<string, string>> headers, byte [] content)
		{
			var builder = new StringBuilder ();
			builder.Append ("HTTP/1.1 ").Append (statusCode).Append (' ').Append (reasonPhrase).Append ("\r\n");
			foreach (var header in headers)
				builder.Append (header.Key).Append (": ").Append (header.Value).Append ("\r\n");
			builder.Append ("Content-Length: ").Append (content.Length).Append ("\r\n");
			// Force the connection closed so we don't have to deal with keep-alive framing.
			builder.Append ("Connection: close\r\n");
			builder.Append ("\r\n");

			var headerBytes = Encoding.ASCII.GetBytes (builder.ToString ());
			await stream.WriteAsync (headerBytes, 0, headerBytes.Length).ConfigureAwait (false);
			if (content.Length > 0)
				await stream.WriteAsync (content, 0, content.Length).ConfigureAwait (false);
			await stream.FlushAsync ().ConfigureAwait (false);
		}

		public void Dispose ()
		{
			try {
				listener.Stop ();
			} catch (SocketException) {
				// ignore errors during cleanup
			} catch (ObjectDisposedException) {
				// already disposed
			}
		}
	}
}
