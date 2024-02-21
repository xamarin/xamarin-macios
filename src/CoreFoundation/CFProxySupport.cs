// 
// CFProxySupport.cs: Implements the managed binding for CFProxySupport.h
//
// Authors: Jeffrey Stedfast <jeff@xamarin.com>
//     
// Copyright (C) 2011 Xamarin, Inc.
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

#nullable enable

#if !WATCH

using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;
#if NET
using CFNetwork;
#endif

namespace CoreFoundation {
	// Utility enum for string constants in ObjC
	public enum CFProxyType {
		None,
		AutoConfigurationUrl,
		AutoConfigurationJavaScript,
		FTP,
		HTTP,
		HTTPS,
		SOCKS
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFProxy {
		NSDictionary settings;

		internal CFProxy (NSDictionary settings)
		{
			this.settings = settings;
		}

		#region Property Keys

#if !MONOMAC
		static NSString? kCFProxyAutoConfigurationHTTPResponseKey;
		static NSString? AutoConfigurationHTTPResponseKey {
			get {
				if (kCFProxyAutoConfigurationHTTPResponseKey is null)
					kCFProxyAutoConfigurationHTTPResponseKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyAutoConfigurationHTTPResponseKey");

				return kCFProxyAutoConfigurationHTTPResponseKey;
			}
		}
#endif

		static NSString? kCFProxyAutoConfigurationJavaScriptKey;
		static NSString? AutoConfigurationJavaScriptKey {
			get {
				if (kCFProxyAutoConfigurationJavaScriptKey is null)
					kCFProxyAutoConfigurationJavaScriptKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyAutoConfigurationJavaScriptKey");

				return kCFProxyAutoConfigurationJavaScriptKey;
			}
		}

		static NSString? kCFProxyAutoConfigurationURLKey;
		static NSString? AutoConfigurationURLKey {
			get {
				if (kCFProxyAutoConfigurationURLKey is null)
					kCFProxyAutoConfigurationURLKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyAutoConfigurationURLKey");

				return kCFProxyAutoConfigurationURLKey;
			}
		}

		static NSString? kCFProxyHostNameKey;
		static NSString? HostNameKey {
			get {
				if (kCFProxyHostNameKey is null)
					kCFProxyHostNameKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyHostNameKey");

				return kCFProxyHostNameKey;
			}
		}

		static NSString? kCFProxyPasswordKey;
		static NSString? PasswordKey {
			get {
				if (kCFProxyPasswordKey is null)
					kCFProxyPasswordKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyPasswordKey");

				return kCFProxyPasswordKey;
			}
		}

		static NSString? kCFProxyPortNumberKey;
		static NSString? PortNumberKey {
			get {
				if (kCFProxyPortNumberKey is null)
					kCFProxyPortNumberKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyPortNumberKey");

				return kCFProxyPortNumberKey;
			}
		}

		static NSString? kCFProxyTypeKey;
		static NSString? ProxyTypeKey {
			get {
				if (kCFProxyTypeKey is null)
					kCFProxyTypeKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeKey");

				return kCFProxyTypeKey;
			}
		}

		static NSString? kCFProxyUsernameKey;
		static NSString? UsernameKey {
			get {
				if (kCFProxyUsernameKey is null)
					kCFProxyUsernameKey = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyUsernameKey");

				return kCFProxyUsernameKey;
			}
		}
		#endregion Property Keys

		#region Proxy Types
		static NSString? kCFProxyTypeNone;
		static NSString? CFProxyTypeNone {
			get {
				if (kCFProxyTypeNone is null)
					kCFProxyTypeNone = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeNone");

				return kCFProxyTypeNone;
			}
		}

		static NSString? kCFProxyTypeAutoConfigurationURL;
		static NSString? CFProxyTypeAutoConfigurationURL {
			get {
				if (kCFProxyTypeAutoConfigurationURL is null)
					kCFProxyTypeAutoConfigurationURL = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeAutoConfigurationURL");

				return kCFProxyTypeAutoConfigurationURL;
			}
		}

		static NSString? kCFProxyTypeAutoConfigurationJavaScript;
		static NSString? CFProxyTypeAutoConfigurationJavaScript {
			get {
				if (kCFProxyTypeAutoConfigurationJavaScript is null)
					kCFProxyTypeAutoConfigurationJavaScript = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeAutoConfigurationJavaScript");

				return kCFProxyTypeAutoConfigurationJavaScript;
			}
		}

		static NSString? kCFProxyTypeFTP;
		static NSString? CFProxyTypeFTP {
			get {
				if (kCFProxyTypeFTP is null)
					kCFProxyTypeFTP = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeFTP");

				return kCFProxyTypeFTP;
			}
		}

		static NSString? kCFProxyTypeHTTP;
		static NSString? CFProxyTypeHTTP {
			get {
				if (kCFProxyTypeHTTP is null)
					kCFProxyTypeHTTP = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeHTTP");

				return kCFProxyTypeHTTP;
			}
		}

		static NSString? kCFProxyTypeHTTPS;
		static NSString? CFProxyTypeHTTPS {
			get {
				if (kCFProxyTypeHTTPS is null)
					kCFProxyTypeHTTPS = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeHTTPS");

				return kCFProxyTypeHTTPS;
			}
		}

		static NSString? kCFProxyTypeSOCKS;
		static NSString? CFProxyTypeSOCKS {
			get {
				if (kCFProxyTypeSOCKS is null)
					kCFProxyTypeSOCKS = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFProxyTypeSOCKS");

				return kCFProxyTypeSOCKS;
			}
		}
		#endregion Proxy Types

		static CFProxyType CFProxyTypeToEnum (NSString type)
		{
			if (CFProxyTypeAutoConfigurationJavaScript is not null) {
				if (type.Handle == CFProxyTypeAutoConfigurationJavaScript.Handle)
					return CFProxyType.AutoConfigurationJavaScript;
			}

			if (CFProxyTypeAutoConfigurationURL is not null) {
				if (type.Handle == CFProxyTypeAutoConfigurationURL.Handle)
					return CFProxyType.AutoConfigurationUrl;
			}

			if (CFProxyTypeFTP is not null) {
				if (type.Handle == CFProxyTypeFTP.Handle)
					return CFProxyType.FTP;
			}

			if (CFProxyTypeHTTP is not null) {
				if (type.Handle == CFProxyTypeHTTP.Handle)
					return CFProxyType.HTTP;
			}

			if (CFProxyTypeHTTPS is not null) {
				if (type.Handle == CFProxyTypeHTTPS.Handle)
					return CFProxyType.HTTPS;
			}

			if (CFProxyTypeSOCKS is not null) {
				if (type.Handle == CFProxyTypeSOCKS.Handle)
					return CFProxyType.SOCKS;
			}

			return CFProxyType.None;
		}

#if false
		// AFAICT these get used with CFNetworkExecuteProxyAutoConfiguration*()
		
		// TODO: bind CFHTTPMessage so we can return the proper type here.
		public NSObject AutoConfigurationHTTPResponse {
			get { return settings[AutoConfigurationHTTPResponseKey]; }
		}
#endif

		public NSString? AutoConfigurationJavaScript {
			get {
				if (AutoConfigurationJavaScriptKey is null)
					return null;

				return (NSString) settings [AutoConfigurationJavaScriptKey];
			}
		}

		public NSUrl? AutoConfigurationUrl {
			get {
				if (AutoConfigurationURLKey is null)
					return null;

				return (NSUrl) settings [AutoConfigurationURLKey];
			}
		}

		public string? HostName {
			get {
				if (HostNameKey is null)
					return null;

				NSString v = (NSString) settings [HostNameKey];

				return v?.ToString ();
			}
		}

		public string? Password {
			get {
				if (PasswordKey is null)
					return null;

				NSString v = (NSString) settings [PasswordKey];

				return v?.ToString ();
			}
		}

		public int Port {
			get {
				if (PortNumberKey is null)
					return 0;

				NSNumber v = (NSNumber) settings [PortNumberKey];

				return v?.Int32Value ?? 0;
			}
		}

		public CFProxyType ProxyType {
			get {
				if (ProxyTypeKey is null)
					return CFProxyType.None;

				return CFProxyTypeToEnum ((NSString) settings [ProxyTypeKey]);
			}
		}

		public string? Username {
			get {
				if (UsernameKey is null)
					return null;

				NSString v = (NSString) settings [UsernameKey];

				return v?.ToString ();
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFProxySettings {
		NSDictionary settings;

		internal CFProxySettings (NSDictionary settings)
		{
			this.settings = settings;
		}

		public NSDictionary Dictionary {
			get { return settings; }
		}

		#region Global Proxy Setting Constants
		static NSString? kCFNetworkProxiesHTTPEnable;
		static NSString? CFNetworkProxiesHTTPEnable {
			get {
				if (kCFNetworkProxiesHTTPEnable is null)
					kCFNetworkProxiesHTTPEnable = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesHTTPEnable");

				return kCFNetworkProxiesHTTPEnable;
			}
		}

		static NSString? kCFNetworkProxiesHTTPPort;
		static NSString? CFNetworkProxiesHTTPPort {
			get {
				if (kCFNetworkProxiesHTTPPort is null)
					kCFNetworkProxiesHTTPPort = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesHTTPPort");

				return kCFNetworkProxiesHTTPPort;
			}
		}

		static NSString? kCFNetworkProxiesHTTPProxy;
		static NSString? CFNetworkProxiesHTTPProxy {
			get {
				if (kCFNetworkProxiesHTTPProxy is null)
					kCFNetworkProxiesHTTPProxy = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesHTTPProxy");

				return kCFNetworkProxiesHTTPProxy;
			}
		}

		static NSString? kCFNetworkProxiesProxyAutoConfigEnable;
		static NSString? CFNetworkProxiesProxyAutoConfigEnable {
			get {
				if (kCFNetworkProxiesProxyAutoConfigEnable is null)
					kCFNetworkProxiesProxyAutoConfigEnable = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesProxyAutoConfigEnable");

				return kCFNetworkProxiesProxyAutoConfigEnable;
			}
		}

#if !MONOMAC
		static NSString? kCFNetworkProxiesProxyAutoConfigJavaScript;
		static NSString? CFNetworkProxiesProxyAutoConfigJavaScript {
			get {
				if (kCFNetworkProxiesProxyAutoConfigJavaScript is null)
					kCFNetworkProxiesProxyAutoConfigJavaScript = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesProxyAutoConfigJavaScript");

				return kCFNetworkProxiesProxyAutoConfigJavaScript;
			}
		}
#endif

		static NSString? kCFNetworkProxiesProxyAutoConfigURLString;
		static NSString? CFNetworkProxiesProxyAutoConfigURLString {
			get {
				if (kCFNetworkProxiesProxyAutoConfigURLString is null)
					kCFNetworkProxiesProxyAutoConfigURLString = Dlfcn.GetStringConstant (Libraries.CFNetwork.Handle, "kCFNetworkProxiesProxyAutoConfigURLString");

				return kCFNetworkProxiesProxyAutoConfigURLString;
			}
		}
		#endregion Global Proxy Setting Constants

		public bool HTTPEnable {
			get {
				if (CFNetworkProxiesHTTPEnable is null)
					return false;

				NSNumber v = (NSNumber) settings [CFNetworkProxiesHTTPEnable];

				return v?.BoolValue ?? false;
			}
		}

		public int HTTPPort {
			get {
				if (CFNetworkProxiesHTTPPort is null)
					return 0;

				NSNumber v = (NSNumber) settings [CFNetworkProxiesHTTPPort];

				return v?.Int32Value ?? 0;
			}
		}

		public string? HTTPProxy {
			get {
				if (CFNetworkProxiesHTTPProxy is null)
					return null;

				NSString v = (NSString) settings [CFNetworkProxiesHTTPProxy];

				return v?.ToString ();
			}
		}

		public bool ProxyAutoConfigEnable {
			get {
				if (CFNetworkProxiesProxyAutoConfigEnable is null)
					return false;

				NSNumber v = (NSNumber) settings [CFNetworkProxiesProxyAutoConfigEnable];

				return v?.BoolValue ?? false;
			}
		}

#if !MONOMAC
		public string? ProxyAutoConfigJavaScript {
			get {
				if (CFNetworkProxiesProxyAutoConfigJavaScript is null)
					return null;

				NSString v = (NSString) settings [CFNetworkProxiesProxyAutoConfigJavaScript];

				return v?.ToString ();
			}
		}
#endif

		public string? ProxyAutoConfigURLString {
			get {
				if (CFNetworkProxiesProxyAutoConfigURLString is null)
					return null;

				NSString v = (NSString) settings [CFNetworkProxiesProxyAutoConfigURLString];

				return v?.ToString ();
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static partial class CFNetwork {
		[DllImport (Constants.CFNetworkLibrary)]
		unsafe extern static /* CFArrayRef __nullable */ IntPtr CFNetworkCopyProxiesForAutoConfigurationScript (
			/* CFStringRef __nonnull */ IntPtr proxyAutoConfigurationScript,
			/* CFURLRef __nonnull */ IntPtr targetURL, /* CFErrorRef  __nullable * __nullable */ IntPtr* error);

		static NSArray? CopyProxiesForAutoConfigurationScript (NSString proxyAutoConfigurationScript, NSUrl targetURL)
		{
			IntPtr err;
			IntPtr native;
			unsafe {
				native = CFNetworkCopyProxiesForAutoConfigurationScript (proxyAutoConfigurationScript.Handle, targetURL.Handle, &err);
			}
			return native == IntPtr.Zero ? null : new NSArray (native);
		}

		public static CFProxy []? GetProxiesForAutoConfigurationScript (NSString proxyAutoConfigurationScript, NSUrl targetURL)
		{
			if (proxyAutoConfigurationScript is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyAutoConfigurationScript));

			if (targetURL is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetURL));

			using (var array = CopyProxiesForAutoConfigurationScript (proxyAutoConfigurationScript, targetURL)) {
				if (array is null)
					return null;

				NSDictionary [] dictionaries = NSArray.ArrayFromHandle<NSDictionary> (array.Handle);
				if (dictionaries is null)
					return null;

				CFProxy [] proxies = new CFProxy [dictionaries.Length];
				for (int i = 0; i < dictionaries.Length; i++)
					proxies [i] = new CFProxy (dictionaries [i]);

				return proxies;
			}
		}

		public static CFProxy []? GetProxiesForAutoConfigurationScript (NSString proxyAutoConfigurationScript, Uri targetUri)
		{
			// proxyAutoConfigurationScript checked later
			if (targetUri is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUri));

			using (var targetURL = NSUrl.FromString (targetUri.AbsoluteUri)) {
				if (targetURL is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetURL));
				return GetProxiesForAutoConfigurationScript (proxyAutoConfigurationScript, targetURL);
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		// CFArrayRef CFNetworkCopyProxiesForURL (CFURLRef url, CFDictionaryRef proxySettings);
		extern static /* CFArrayRef __nonnull */ IntPtr CFNetworkCopyProxiesForURL (
			/* CFURLRef __nonnull */ IntPtr url,
			/* CFDictionaryRef __nonnull */ IntPtr proxySettings);

		static NSArray? CopyProxiesForURL (NSUrl url, NSDictionary proxySettings)
		{
			IntPtr native = CFNetworkCopyProxiesForURL (url.Handle, proxySettings.Handle);
			return native == IntPtr.Zero ? null : new NSArray (native);
		}

		public static CFProxy []? GetProxiesForURL (NSUrl url, CFProxySettings? proxySettings)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			if (proxySettings is null)
				proxySettings = GetSystemProxySettings ();

			if (proxySettings is null)
				return null;

			using (NSArray? array = CopyProxiesForURL (url, proxySettings.Dictionary)) {
				if (array is null)
					return null;

				NSDictionary [] dictionaries = NSArray.ArrayFromHandle<NSDictionary> (array.Handle);
				if (dictionaries is null)
					return null;

				CFProxy [] proxies = new CFProxy [dictionaries.Length];
				for (int i = 0; i < dictionaries.Length; i++)
					proxies [i] = new CFProxy (dictionaries [i]);

				return proxies;
			}
		}

		public static CFProxy []? GetProxiesForUri (Uri uri, CFProxySettings? proxySettings)
		{
			if (uri is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (uri));

			using (NSUrl? url = NSUrl.FromString (uri.AbsoluteUri)) {
				if (url is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
				return GetProxiesForURL (url, proxySettings);
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CFNetworkCopySystemProxySettings ();

		public static CFProxySettings? GetSystemProxySettings ()
		{
			IntPtr native = CFNetworkCopySystemProxySettings ();

			if (native == IntPtr.Zero)
				return null;

			var dict = new NSDictionary (native);
			// Must release since the IntPtr constructor calls Retain and
			// CFNetworkCopySystemProxySettings return value is already retained
			dict.DangerousRelease ();
			return new CFProxySettings (dict);
		}

#if !NET
		delegate void CFProxyAutoConfigurationResultCallbackInternal (IntPtr client, IntPtr proxyList, IntPtr error);
		// helper delegate to reuse code
#endif
#if NET
		unsafe delegate IntPtr CreatePACCFRunLoopSource (delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb, ref CFStreamClientContext context);
#else
		delegate IntPtr CreatePACCFRunLoopSource (CFProxyAutoConfigurationResultCallbackInternal cb, ref CFStreamClientContext context);
#endif

		static CFProxy []? ParseProxies (IntPtr proxyList)
		{
			CFProxy []? proxies = null;
			if (proxyList != IntPtr.Zero) {
				// it was retained in the cbs.
				using (var array = new CFArray (proxyList, false)) {
					proxies = new CFProxy [array.Count];
					for (int i = 0; i < proxies.Length; i++) {
						var dict = Runtime.GetNSObject<NSDictionary> (array.GetValue (i));
						proxies [i] = new CFProxy (dict!);
					}
				}
			}
			return proxies;
		}

		// helper struct to contain all the data that was used in the callback
		struct PACProxyCallbackData {
			public IntPtr ProxyListPtr; // Pointer to a CFArray to later be parsed
			public IntPtr ErrorPtr; // Pointer to the Error
			public IntPtr CFRunLoopPtr; // Pointer to the runloop, needed to be stopped

			public CFProxy []? ProxyList {
				get {
					if (ProxyListPtr != IntPtr.Zero)
						return ParseProxies (ProxyListPtr);
					return null;
				}
			}

			public NSError? Error {
				get {
					if (ErrorPtr != IntPtr.Zero)
						return Runtime.GetNSObject<NSError> (ErrorPtr);
					return null;
				}
			}
		}

		// callback that will sent the client info
#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (CFProxyAutoConfigurationResultCallbackInternal))]
#endif
		static void ExecutePacCallback (IntPtr client, IntPtr proxyList, IntPtr error)
		{
			// grab the required structure and set the data, according apple docs:
			// client
			// The client reference originally passed in the clientContext parameter of the 
			// CFNetworkExecuteProxyAutoConfigurationScript or CFNetworkExecuteProxyAutoConfigurationURL call
			// that triggered this callback.
			// Well, that is NOT TRUE, the client passed is the client.Info pointer not the client.
			var pacCbData = Marshal.PtrToStructure<PACProxyCallbackData> (client)!;
			// make sure is not released, will be released by the parsing method.
			if (proxyList != IntPtr.Zero) {
				CFObject.CFRetain (proxyList);
				pacCbData.ProxyListPtr = proxyList;
			}
			if (error != IntPtr.Zero) {
				NSObject.DangerousRetain (error);
				pacCbData.ErrorPtr = error;
			}
			// stop the CFRunLoop
			var runLoop = new CFRunLoop (pacCbData.CFRunLoopPtr, false);
			Marshal.StructureToPtr<PACProxyCallbackData> (pacCbData, client, false);
			runLoop.Stop ();
		}

		static async Task<(CFProxy []? proxies, NSError? error)> ExecutePacCFRunLoopSourceAsync (CreatePACCFRunLoopSource factory, CancellationToken cancellationToken)
		{
			CFProxy []? proxies = null;
			NSError? outError = null;
			if (cancellationToken.IsCancellationRequested)
				throw new OperationCanceledException ("Operation was cancelled.");

			await Task.Run (() => {
				// we need the runloop of THIS thread, so it is important to get it in the correct context
				var runLoop = CFRunLoop.Current;

				// build a struct that will have all the needed info for the callback
				var pacCbData = new PACProxyCallbackData ();
				pacCbData.CFRunLoopPtr = runLoop.Handle;
				var pacDataPtr = Marshal.AllocHGlobal (Marshal.SizeOf<PACProxyCallbackData> ());
				try {
					Marshal.StructureToPtr<PACProxyCallbackData> (pacCbData, pacDataPtr, false);

					var clientContext = new CFStreamClientContext ();
					clientContext.Info = pacDataPtr;

#if NET
					unsafe {
					using (var loopSource = new CFRunLoopSource (factory (&ExecutePacCallback, ref clientContext), false))
#else
					using (var loopSource = new CFRunLoopSource (factory (ExecutePacCallback, ref clientContext), false))
#endif
					using (var mode = new NSString ("Xamarin.iOS.Proxy")) {

						if (cancellationToken.IsCancellationRequested)
							throw new OperationCanceledException ("Operation was cancelled.");

						cancellationToken.Register (() => {
							//if user cancels, we invalidte the source, stop the runloop and remove the source
							loopSource.Invalidate ();
							runLoop.RemoveSource (loopSource, mode);
							runLoop.Stop ();
						});
						runLoop.AddSource (loopSource, mode);
						// blocks until stop is called, will be done in the cb set previously
						runLoop.RunInMode (mode, double.MaxValue, false);
						// does not raise an error if source is not longer present, so no need to worry
						runLoop.RemoveSource (loopSource, mode);
					}
#if NET
					} // matches the unsafe block
#endif

					if (cancellationToken.IsCancellationRequested)
						throw new OperationCanceledException ("Operation was cancelled.");

					pacCbData = Marshal.PtrToStructure<PACProxyCallbackData> (pacDataPtr)!;
					// get data from the struct
					proxies = pacCbData.ProxyList;
					outError = pacCbData.Error;
				} finally {
					// clean resources
					if (pacCbData.ProxyListPtr != IntPtr.Zero)
						CFObject.CFRelease (pacCbData.ProxyListPtr);
					if (pacCbData.ErrorPtr != IntPtr.Zero)
						NSObject.DangerousRelease (pacCbData.ErrorPtr);
					Marshal.FreeHGlobal (pacDataPtr);
				}
			}, cancellationToken).ConfigureAwait (false);

			if (cancellationToken.IsCancellationRequested)
				throw new OperationCanceledException ("Operation was cancelled.");
			return (proxies: proxies, error: outError);
		}

		static CFProxy []? ExecutePacCFRunLoopSourceBlocking (CreatePACCFRunLoopSource factory, out NSError? outError)
		{
			var runLoop = CFRunLoop.Current;
			outError = null;

			// build a struct that will have all the needed info for the callback
			var pacCbData = new PACProxyCallbackData ();
			pacCbData.CFRunLoopPtr = runLoop.Handle;
			var pacDataPtr = Marshal.AllocHGlobal (Marshal.SizeOf<PACProxyCallbackData> ());
			try {
				Marshal.StructureToPtr<PACProxyCallbackData> (pacCbData, pacDataPtr, false);

				var clientContext = new CFStreamClientContext ();
				clientContext.Info = pacDataPtr;

#if NET
				unsafe {
				using (var loopSource = new CFRunLoopSource (factory (&ExecutePacCallback, ref clientContext), false))
#else
				using (var loopSource = new CFRunLoopSource (factory (ExecutePacCallback, ref clientContext), false))
#endif
				using (var mode = new NSString ("Xamarin.iOS.Proxy")) {
					runLoop.AddSource (loopSource, mode);
					runLoop.RunInMode (mode, double.MaxValue, false);
					runLoop.RemoveSource (loopSource, mode);
				}
				pacCbData = Marshal.PtrToStructure<PACProxyCallbackData> (pacDataPtr)!;
				// get data from the struct
				outError = pacCbData.Error;
#if NET
				} // unsafe
#endif
				return pacCbData.ProxyList;
			} finally {
				if (pacCbData.ProxyListPtr != IntPtr.Zero)
					CFObject.CFRelease (pacCbData.ProxyListPtr);
				if (pacCbData.ErrorPtr != IntPtr.Zero)
					NSObject.DangerousRelease (pacCbData.ErrorPtr);
				Marshal.FreeHGlobal (pacDataPtr);
			}
		}

		[DllImport (Constants.CFNetworkLibrary)]
		extern unsafe static /* CFRunLoopSourceRef __nonnull */ IntPtr CFNetworkExecuteProxyAutoConfigurationScript (
			/* CFStringRef __nonnull */ IntPtr proxyAutoConfigurationScript,
			/* CFURLRef __nonnull */ IntPtr targetURL,
#if NET
			/* CFProxyAutoConfigurationResultCallback __nonnull */ delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb,
#else
			/* CFProxyAutoConfigurationResultCallback __nonnull */ CFProxyAutoConfigurationResultCallbackInternal cb,
#endif
			/* CFStreamClientContext * __nonnull */ CFStreamClientContext* clientContext);

		public static CFProxy []? ExecuteProxyAutoConfigurationScript (string proxyAutoConfigurationScript, Uri targetUrl, out NSError? outError)
		{
			outError = null;
			if (proxyAutoConfigurationScript is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyAutoConfigurationScript));

			if (targetUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUrl));

			using (var pacScript = new NSString (proxyAutoConfigurationScript))
			using (var url = new NSUrl (targetUrl.AbsoluteUri)) {
				CreatePACCFRunLoopSource factory;
				unsafe {
#if NET
					factory = delegate (delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb, ref CFStreamClientContext context)
#else
					factory = delegate (CFProxyAutoConfigurationResultCallbackInternal cb, ref CFStreamClientContext context)
#endif
					{
						return CFNetworkExecuteProxyAutoConfigurationScript (pacScript.Handle, url.Handle, cb, (CFStreamClientContext *) Unsafe.AsPointer<CFStreamClientContext> (ref context));
					};
				}
				return ExecutePacCFRunLoopSourceBlocking (factory, out outError);
			}
		}

		public static async Task<(CFProxy []? proxies, NSError? error)> ExecuteProxyAutoConfigurationScriptAsync (string proxyAutoConfigurationScript, Uri targetUrl, CancellationToken cancellationToken)
		{
			if (proxyAutoConfigurationScript is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyAutoConfigurationScript));

			if (targetUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUrl));

			using (var pacScript = new NSString (proxyAutoConfigurationScript))
			using (var url = new NSUrl (targetUrl.AbsoluteUri)) {
				CreatePACCFRunLoopSource factory;
				unsafe {
#if NET
					factory = delegate (delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb, ref CFStreamClientContext context)
#else
					factory = delegate (CFProxyAutoConfigurationResultCallbackInternal cb, ref CFStreamClientContext context)
#endif
					{
						return CFNetworkExecuteProxyAutoConfigurationScript (pacScript.Handle, url.Handle, cb, (CFStreamClientContext *) Unsafe.AsPointer<CFStreamClientContext> (ref context));
					};
				}
				// use the helper task with a factory for this method
				return await ExecutePacCFRunLoopSourceAsync (factory, cancellationToken).ConfigureAwait (false);
			}
		}

#if NET
		[DllImport (Constants.CFNetworkLibrary)]
		extern unsafe static /* CFRunLoopSourceRef __nonnull */ IntPtr CFNetworkExecuteProxyAutoConfigurationURL (
			/* CFURLRef __nonnull */ IntPtr proxyAutoConfigurationURL,
			/* CFURLRef __nonnull */ IntPtr targetURL,
			/* CFProxyAutoConfigurationResultCallback __nonnull */ delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb,
			/* CFStreamClientContext * __nonnull */ CFStreamClientContext* clientContext);
#else
		[DllImport (Constants.CFNetworkLibrary)]
		extern unsafe static /* CFRunLoopSourceRef __nonnull */ IntPtr CFNetworkExecuteProxyAutoConfigurationURL (
			/* CFURLRef __nonnull */ IntPtr proxyAutoConfigurationURL,
			/* CFURLRef __nonnull */ IntPtr targetURL,
			/* CFProxyAutoConfigurationResultCallback __nonnull */ CFProxyAutoConfigurationResultCallbackInternal cb,
			/* CFStreamClientContext * __nonnull */ CFStreamClientContext* clientContext);
#endif

		public static CFProxy []? ExecuteProxyAutoConfigurationUrl (Uri proxyAutoConfigurationUrl, Uri targetUrl, out NSError? outError)
		{
			outError = null;
			if (proxyAutoConfigurationUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyAutoConfigurationUrl));

			if (targetUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUrl));

			using (var pacUrl = new NSUrl (proxyAutoConfigurationUrl.AbsoluteUri)) // toll free bridge to CFUrl
			using (var url = new NSUrl (targetUrl.AbsoluteUri)) {
				CreatePACCFRunLoopSource factory;
				unsafe {
#if NET
					factory = delegate (delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb, ref CFStreamClientContext context)
#else
					factory = delegate (CFProxyAutoConfigurationResultCallbackInternal cb, ref CFStreamClientContext context)
#endif
					{
						return CFNetworkExecuteProxyAutoConfigurationURL (pacUrl.Handle, url.Handle, cb, (CFStreamClientContext *) Unsafe.AsPointer<CFStreamClientContext> (ref context));
					};
				}
				return ExecutePacCFRunLoopSourceBlocking (factory, out outError);
			}
		}

		public static async Task<(CFProxy []? proxies, NSError? error)> ExecuteProxyAutoConfigurationUrlAsync (Uri proxyAutoConfigurationUrl, Uri targetUrl, CancellationToken cancellationToken)
		{
			// similar to the sync method, but we will spawn a thread and wait in an async manner to an autoreset event to be fired
			if (proxyAutoConfigurationUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (proxyAutoConfigurationUrl));

			if (targetUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUrl));

			using (var pacUrl = new NSUrl (proxyAutoConfigurationUrl.AbsoluteUri)) // toll free bridge to CFUrl
			using (var url = new NSUrl (targetUrl.AbsoluteUri)) {
				CreatePACCFRunLoopSource factory;
				unsafe {
#if NET
					factory = delegate (delegate* unmanaged<IntPtr, IntPtr, IntPtr, void> cb, ref CFStreamClientContext context)
#else
					factory = delegate (CFProxyAutoConfigurationResultCallbackInternal cb, ref CFStreamClientContext context)
#endif
					{
						return CFNetworkExecuteProxyAutoConfigurationURL (pacUrl.Handle, url.Handle, cb, (CFStreamClientContext *) Unsafe.AsPointer<CFStreamClientContext> (ref context));
					};
				}
				// use the helper task with a factory for this method
				return await ExecutePacCFRunLoopSourceAsync (factory, cancellationToken).ConfigureAwait (false);
			}
		}

		class CFWebProxy : IWebProxy {
			ICredentials? credentials;
			bool userSpecified;

			public CFWebProxy ()
			{
			}

			public ICredentials? Credentials {
				get { return credentials; }
				set {
					userSpecified = true;
					credentials = value;
				}
			}

			static Uri? GetProxyUri (CFProxy proxy, out NetworkCredential? credentials)
			{
				string protocol;

				switch (proxy.ProxyType) {
				case CFProxyType.FTP:
					protocol = "ftp://";
					break;
				case CFProxyType.HTTP:
				case CFProxyType.HTTPS:
					protocol = "http://";
					break;
				default:
					credentials = null;
					return null;
				}

				//[SuppressMessage ("Microsoft.Security", "CS002:SecretInNextLine", Justification="No credentials are stored, they are retrived from the OS Proxy settings.")]
				var username = proxy?.Username;
				var password = proxy?.Password;
				var hostname = proxy?.HostName;
				var port = proxy?.Port;
				string uri;

				if (username is not null)
					credentials = new NetworkCredential (username, password);
				else
					credentials = null;

				uri = protocol + hostname + (port != 0 ? ':' + port.ToString () : string.Empty);

				return new Uri (uri, UriKind.Absolute);
			}

			static Uri? GetProxyUriFromScript (NSString script, Uri targetUri, out NetworkCredential? credentials)
			{
				CFProxy []? proxies = CFNetwork.GetProxiesForAutoConfigurationScript (script, targetUri);

				if (proxies is null) {
					credentials = null;
					return targetUri;
				}

				for (int i = 0; i < proxies.Length; i++) {
					switch (proxies [i].ProxyType) {
					case CFProxyType.HTTPS:
					case CFProxyType.HTTP:
					case CFProxyType.FTP:
						// create a Uri based on the hostname/port/etc info
						return GetProxyUri (proxies [i], out credentials);
					case CFProxyType.SOCKS:
					default:
						// unsupported proxy type, try the next one
						break;
					case CFProxyType.None:
						// no proxy should be used
						credentials = null;
						return targetUri;
					}
				}

				credentials = null;

				return null;
			}

			public Uri GetProxy (Uri targetUri)
			{
				NetworkCredential? credentials = null;
				Uri? proxy = null;

				if (targetUri is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUri));

				try {
					CFProxySettings? settings = CFNetwork.GetSystemProxySettings ();
					CFProxy []? proxies = CFNetwork.GetProxiesForUri (targetUri, settings);

					if (proxies is not null) {
						for (int i = 0; i < proxies.Length && proxy is null; i++) {
							switch (proxies [i].ProxyType) {
							case CFProxyType.AutoConfigurationJavaScript:
								proxy = GetProxyUriFromScript (proxies [i].AutoConfigurationJavaScript!, targetUri, out credentials);
								break;
							case CFProxyType.AutoConfigurationUrl:
								// unsupported proxy type (requires fetching script from remote url)
								break;
							case CFProxyType.HTTPS:
							case CFProxyType.HTTP:
							case CFProxyType.FTP:
								// create a Uri based on the hostname/port/etc info
								proxy = GetProxyUri (proxies [i], out credentials);
								break;
							case CFProxyType.SOCKS:
								// unsupported proxy type, try the next one
								break;
							case CFProxyType.None:
								// no proxy should be used
								proxy = targetUri;
								break;
							}
						}

						if (proxy is null) {
							// no supported proxies for this Uri, fall back to trying to connect to targetUri directly
							proxy = targetUri;
						}
					} else {
						proxy = targetUri;
					}
				} catch {
					// ignore errors while retrieving proxy data
					proxy = targetUri;
				}

				if (!userSpecified)
					this.credentials = credentials;

				return proxy;
			}

			public bool IsBypassed (Uri targetUri)
			{
				if (targetUri is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetUri));

				return GetProxy (targetUri) == targetUri;
			}
		}

		public static IWebProxy GetDefaultProxy ()
		{
			return new CFWebProxy ();
		}
	}
}

#endif // !WATCH
