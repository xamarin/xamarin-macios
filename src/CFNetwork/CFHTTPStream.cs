//
// MonoMac.CoreServices.CFHTTPStream
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012-2015 Xamarin Inc. (http://www.xamarin.com)
//

#nullable enable

using System;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

// CFHttpStream is in CFNetwork.framework, no idea why it ended up in CoreServices when it was bound.
#if NET
namespace CFNetwork {
#else
namespace CoreServices {
#endif

	// all fields constants that this is using are deprecated in Xcode 7
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("macos10.11", "Use 'NSUrlSession'.")]
	[ObsoletedOSPlatform ("ios9.0", "Use 'NSUrlSession'.")]
#else
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession'.")]
	[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession'.")]
#endif
	// Dotnet attributes are included in partial class inside cfnetwork.cs
	public partial class CFHTTPStream : CFReadStream {

		[Preserve (Conditional = true)]
		internal CFHTTPStream (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public Uri? FinalURL {
			get {
				var handle = GetProperty (_FinalURL);
				if (handle == IntPtr.Zero)
					return null;

				if (CFType.GetTypeID (handle) != CFUrl.GetTypeID ()) {
					CFObject.CFRelease (handle);
					throw new InvalidCastException ();
				}

				using (var url = new CFUrl (handle, false))
					return new Uri (url.ToString ()!);
			}
		}

		public CFHTTPMessage? GetFinalRequest ()
		{
			var handle = GetProperty (_FinalRequest);
			if (handle == IntPtr.Zero)
				return null;

			if (CFType.GetTypeID (handle) != CFHTTPMessage.GetTypeID ()) {
				CFObject.CFRelease (handle);
				throw new InvalidCastException ();
			}

			return new CFHTTPMessage (handle, true);
		}

		public CFHTTPMessage? GetResponseHeader ()
		{
			var handle = GetProperty (_ResponseHeader);
			if (handle == IntPtr.Zero)
				return null;

			if (CFType.GetTypeID (handle) != CFHTTPMessage.GetTypeID ()) {
				CFObject.CFRelease (handle);
				throw new InvalidCastException ();
			}
			return new CFHTTPMessage (handle, true);
		}

		public bool AttemptPersistentConnection {
			get {
				var handle = GetProperty (_AttemptPersistentConnection);
				if (handle == IntPtr.Zero)
					return false;
				else if (handle == CFBoolean.FalseHandle)
					return false;
				else if (handle == CFBoolean.TrueHandle)
					return true;
				else
					throw new InvalidCastException ();
			}
			set {
				SetProperty (_AttemptPersistentConnection,
							 CFBoolean.FromBoolean (value));
			}
		}

		public int RequestBytesWrittenCount {
			get {
				var handle = GetProperty (_RequestBytesWrittenCount);
				if (handle == IntPtr.Zero)
					return 0;

				using (var number = new NSNumber (handle))
					return number.Int32Value;
			}
		}

		public bool ShouldAutoredirect {
			get {
				var handle = GetProperty (_ShouldAutoredirect);
				if (handle == IntPtr.Zero)
					return false;
				else if (handle == CFBoolean.FalseHandle)
					return false;
				else if (handle == CFBoolean.TrueHandle)
					return true;
				else
					throw new InvalidCastException ();
			}
			set {
				SetProperty (_ShouldAutoredirect,
							 CFBoolean.FromBoolean (value));
			}
		}

		internal CFDictionary Proxy {
			set {
				SetProperty (_Proxy, value);
			}
		}

#if !WATCHOS
		public void SetProxy (CFProxySettings proxySettings)
		{
			if (proxySettings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxySettings));

			SetProperty (_Proxy, proxySettings.Dictionary);
		}
#endif // !WATCHOS
	}
}
