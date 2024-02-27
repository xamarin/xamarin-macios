//
// NSStream extensions
//
// If you add or change any of the NSStream convenience constructors, update
// the same code in CFStream.
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011, Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Runtime.Versioning;
using CoreFoundation;
using System.Net;
using System.Net.Sockets;
using ObjCRuntime;
#if !WATCH
#if NET
using CFNetwork;
#else
using CoreServices;
#endif
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {
	public enum NSStreamSocketSecurityLevel {
		None, SslV2, SslV3, TlsV1, NegotiatedSsl, Unknown
	}

	public enum NSStreamServiceType {
		Default, VoIP, Video, Background, Voice
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class NSStreamSocksOptions {
		public string HostName;
		public int HostPort;
		public int Version;
		public string Username;
		public string Password;
	}

	public partial class NSStream {
		public NSObject this [NSString key] {
			get {
				return GetProperty (key);
			}
			set {
				SetProperty (value, key);
			}
		}

		public NSStreamSocksOptions SocksOptions {
			get {
				var d = this [SocksProxyConfigurationKey] as NSDictionary;
				if (d is null)
					return null;
				var ret = new NSStreamSocksOptions ();
				var host = d [SocksProxyHostKey] as NSString;
				var port = d [SocksProxyPortKey] as NSNumber;
				var version = d [SocksProxyVersionKey] as NSString;
				var user = d [SocksProxyUserKey] as NSString;
				var pass = d [SocksProxyPasswordKey] as NSString;
				if (host is not null)
					ret.HostName = (string) host;
				if (port is not null)
					ret.HostPort = port.Int32Value;
				if (version is not null)
					ret.Version = (version == SocksProxyVersion4) ? 4 : (version == SocksProxyVersion5 ? 5 : -1);
				if (user is not null)
					ret.Username = (string) user;
				if (pass is not null)
					ret.Password = (string) pass;
				return ret;
			}
			set {
				if (value is null) {
					this [SocksProxyConfigurationKey] = null;
					return;
				}
				var d = new NSMutableDictionary ();
				if (value.HostName is not null)
					d [SocksProxyHostKey] = new NSString (value.HostName);
				if (value.HostPort != 0)
					d [SocksProxyPortKey] = new NSNumber (value.HostPort);
				if (value.Version == 4)
					d [SocksProxyVersionKey] = SocksProxyVersion4;
				if (value.Version == 5)
					d [SocksProxyVersionKey] = SocksProxyVersion5;
				if (value.Username is not null)
					d [SocksProxyUserKey] = new NSString (value.Username);
				if (value.Password is not null)
					d [SocksProxyPasswordKey] = new NSString (value.Password);
				this [SocksProxyConfigurationKey] = d;
			}
		}

		public NSStreamSocketSecurityLevel SocketSecurityLevel {
			get {
				var k = this [SocketSecurityLevelKey] as NSString;
				if (k == SocketSecurityLevelNone)
					return NSStreamSocketSecurityLevel.None;
				if (k == SocketSecurityLevelSslV2)
					return NSStreamSocketSecurityLevel.SslV2;
				if (k == SocketSecurityLevelSslV3)
					return NSStreamSocketSecurityLevel.SslV3;
				if (k == SocketSecurityLevelTlsV1)
					return NSStreamSocketSecurityLevel.TlsV1;
				if (k == SocketSecurityLevelNegotiatedSsl)
					return NSStreamSocketSecurityLevel.NegotiatedSsl;
				return NSStreamSocketSecurityLevel.Unknown;
			}
			set {
				NSString v = null;
				switch (value) {
				case NSStreamSocketSecurityLevel.None:
					v = SocketSecurityLevelNone;
					break;
				case NSStreamSocketSecurityLevel.SslV2:
					v = SocketSecurityLevelSslV2;
					break;
				case NSStreamSocketSecurityLevel.SslV3:
					v = SocketSecurityLevelSslV3;
					break;
				case NSStreamSocketSecurityLevel.TlsV1:
					v = SocketSecurityLevelTlsV1;
					break;
				case NSStreamSocketSecurityLevel.NegotiatedSsl:
					v = SocketSecurityLevelNegotiatedSsl;
					break;
				}
				if (v is not null)
					this [SocketSecurityLevelKey] = v;
			}
		}

		public NSData DataWrittenToMemoryStream {
			get {
				return this [DataWrittenToMemoryStreamKey] as NSData;
			}
		}

		public NSNumber FileCurrentOffset {
			get {
				return this [FileCurrentOffsetKey] as NSNumber;
			}
		}

		public NSStreamServiceType ServiceType {
			get {
				var v = this [NetworkServiceType] as NSString;
				if (v == NetworkServiceTypeBackground)
					return NSStreamServiceType.Background;
				if (v == NetworkServiceTypeVideo)
					return NSStreamServiceType.Video;
				if (v == NetworkServiceTypeVoice)
					return NSStreamServiceType.Voice;
				if (v == NetworkServiceTypeVoIP)
					return NSStreamServiceType.VoIP;
				return NSStreamServiceType.Default;
			}
			set {
				NSString v = null;
				switch (value) {
				case NSStreamServiceType.Background:
					v = NetworkServiceTypeBackground;
					break;
				case NSStreamServiceType.Video:
					v = NetworkServiceTypeVideo;
					break;
				case NSStreamServiceType.Voice:
					v = NetworkServiceTypeVoIP;
					break;
				case NSStreamServiceType.VoIP:
					v = NetworkServiceTypeVoIP;
					break;
				case NSStreamServiceType.Default:
					break;
				}
				this [NetworkServiceType] = v;
			}
		}

		static void AssignStreams (IntPtr read, IntPtr write,
					out NSInputStream readStream, out NSOutputStream writeStream)
		{
			readStream = Runtime.GetNSObject<NSInputStream> (read);
			writeStream = Runtime.GetNSObject<NSOutputStream> (write);
		}

		public static void CreatePairWithSocket (CFSocket socket,
							 out NSInputStream readStream,
												 out NSOutputStream writeStream)
		{
			if (socket is null)
				throw new ArgumentNullException ("socket");

			IntPtr read, write;
			unsafe {
				CFStream.CFStreamCreatePairWithSocket (IntPtr.Zero, socket.GetNative (), &read, &write);
			}
			AssignStreams (read, write, out readStream, out writeStream);
		}

		public static void CreatePairWithPeerSocketSignature (AddressFamily family, SocketType type,
															  ProtocolType proto, IPEndPoint endpoint,
															  out NSInputStream readStream,
															  out NSOutputStream writeStream)
		{
			using (var address = new CFSocketAddress (endpoint)) {
				var sig = new CFSocketSignature (family, type, proto, address);
				IntPtr read, write;
				unsafe {
					CFStream.CFStreamCreatePairWithPeerSocketSignature (IntPtr.Zero, &sig, &read, &write);
				}
				AssignStreams (read, write, out readStream, out writeStream);
			}
		}

#if !WATCH // There's no CFStreamCreatePairWithSocketToCFHost in WatchOS
		public static void CreatePairWithSocketToHost (IPEndPoint endpoint,
													   out NSInputStream readStream,
													   out NSOutputStream writeStream)
		{
			using (var host = CFHost.Create (endpoint)) {
				IntPtr read, write;
				unsafe {
					CFStream.CFStreamCreatePairWithSocketToCFHost (IntPtr.Zero, host.Handle, endpoint.Port, &read, &write);
				}
				AssignStreams (read, write, out readStream, out writeStream);
			}
		}
#endif

		public static void CreateBoundPair (out NSInputStream readStream, out NSOutputStream writeStream, nint bufferSize)
		{
			IntPtr read, write;
			unsafe {
				CFStream.CFStreamCreateBoundPair (IntPtr.Zero, &read, &write, bufferSize);
			}
			AssignStreams (read, write, out readStream, out writeStream);
		}
	}
}
