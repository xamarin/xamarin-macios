//
// SslConnection
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc.
//

#nullable enable

using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Security {

#if !NET
	unsafe delegate SslStatus SslReadFunc (IntPtr connection, IntPtr data, /* size_t* */ nint* dataLength);

	unsafe delegate SslStatus SslWriteFunc (IntPtr connection, IntPtr data, /* size_t* */ nint* dataLength);
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("macos10.15", Constants.UseNetworkInstead)]
	[ObsoletedOSPlatform ("tvos13.0", Constants.UseNetworkInstead)]
	[ObsoletedOSPlatform ("ios13.0", Constants.UseNetworkInstead)]
	[ObsoletedOSPlatform ("maccatalyst13.0", Constants.UseNetworkInstead)]
#else
	[Deprecated (PlatformName.MacOSX, 10, 15, message: Constants.UseNetworkInstead)]
	[Deprecated (PlatformName.iOS, 13, 0, message: Constants.UseNetworkInstead)]
	[Deprecated (PlatformName.TvOS, 13, 0, message: Constants.UseNetworkInstead)]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: Constants.UseNetworkInstead)]
#endif
	public abstract class SslConnection : IDisposable {

		GCHandle handle;

		protected SslConnection ()
		{
			handle = GCHandle.Alloc (this);
			ConnectionId = GCHandle.ToIntPtr (handle);
#if !NET
			unsafe {
				ReadFunc = Read;
				WriteFunc = Write;
			}
#endif
		}

		~SslConnection ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (handle.IsAllocated)
				handle.Free ();
		}

		public IntPtr ConnectionId { get; private set; }

#if NET
		unsafe internal delegate* unmanaged<IntPtr, IntPtr, nint*, SslStatus> ReadFunc { get { return &Read; } }
		unsafe internal delegate* unmanaged<IntPtr, IntPtr, nint*, SslStatus> WriteFunc { get { return &Write; } }
#else
		internal SslReadFunc ReadFunc { get; private set; }
		internal SslWriteFunc WriteFunc { get; private set; }
#endif

		public abstract SslStatus Read (IntPtr data, ref nint dataLength);

		public abstract SslStatus Write (IntPtr data, ref nint dataLength);

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (SslReadFunc))]
#endif
		unsafe static SslStatus Read (IntPtr connection, IntPtr data, nint* dataLength)
		{
			var c = (SslConnection) GCHandle.FromIntPtr (connection).Target!;
			return c.Read (data, ref System.Runtime.CompilerServices.Unsafe.AsRef<nint> (dataLength));
		}

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (SslWriteFunc))]
#endif
		unsafe static SslStatus Write (IntPtr connection, IntPtr data, nint* dataLength)
		{
			var c = (SslConnection) GCHandle.FromIntPtr (connection).Target!;
			return c.Write (data, ref System.Runtime.CompilerServices.Unsafe.AsRef<nint> (dataLength));
		}
	}


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// a concrete connection based on a managed Stream
	public class SslStreamConnection : SslConnection {

		byte [] buffer;

		public SslStreamConnection (Stream stream)
		{
			if (stream is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (stream));
			InnerStream = stream;
			// a bit higher than the default maximum fragment size
			buffer = new byte [16384];
		}

		public Stream InnerStream { get; private set; }

		public override SslStatus Read (IntPtr data, ref nint dataLength)
		{
			// SSL state prevents multiple simultaneous reads (internal MAC would break)
			// so it's possible to reuse a single buffer (not re-allocate one each time)
			int len = (int) Math.Min (dataLength, buffer.Length);
			int read = InnerStream.Read (buffer, 0, len);
			Marshal.Copy (buffer, 0, data, read);
			bool block = (read < dataLength);
			dataLength = read;
			return block ? SslStatus.WouldBlock : SslStatus.Success;
		}

		public unsafe override SslStatus Write (IntPtr data, ref nint dataLength)
		{
			using (var ms = new UnmanagedMemoryStream ((byte*) data, dataLength)) {
				try {
					ms.CopyTo (InnerStream);
				} catch (IOException) {
					return SslStatus.ClosedGraceful;
				} catch {
					return SslStatus.Internal;
				}
			}
			return SslStatus.Success;
		}
	}
}
