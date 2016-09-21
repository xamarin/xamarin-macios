using System;
using System.IO;
using System.Text;

#if MTOUCH || MMP
using Mono.Cecil;
using Xamarin.Linker;
#elif SYSTEM_NET_HTTP || (MONOMAC && XAMCORE_2_0)
using System.Net.Http;
#endif

#if XAMCORE_2_0
using Foundation;
#elif MONOMAC && !MMP
using MonoMac.Foundation;
#elif !MTOUCH && !MMP && !MMP_TEST
using MonoTouch.Foundation;
#endif

#if !COREBUILD && (XAMARIN_APPLETLS || XAMARIN_NO_TLS)
#if !MTOUCH && !MMP && !MMP_TEST
using Mono.Security.Interface;
#if XAMARIN_APPLETLS
using XamCore.Security.Tls;
#elif XAMARIN_NO_TLS
using XamCore.Security.NoTls;
#endif
#endif
#endif

#if MMP || MMP_TEST || MTOUCH
namespace Xamarin.Bundler {
#elif SYSTEM_NET_HTTP
namespace System.Net.Http {
#else
namespace XamCore.ObjCRuntime {
#endif
	class RuntimeOptions
	{
		const string HttpClientHandlerValue = "HttpClientHandler";
		const string CFNetworkHandlerValue = "CFNetworkHandler";
		const string NSUrlSessionHandlerValue = "NSUrlSessionHandler";

		const string DefaultTlsProviderValue = "default";
		const string LegacyTlsProviderValue = "legacy";
		const string AppleTlsProviderValue = "appletls";

		string http_message_handler;
		string tls_provider;

#if MTOUCH || MMP
		/*
		 * This section is only used by the tools
		 */
		internal static RuntimeOptions Create (string http_message_handler, string tls_provider)
		{
			var options = new RuntimeOptions ();
			options.http_message_handler = ParseHttpMessageHandler (http_message_handler);
			options.tls_provider = ParseTlsProvider (tls_provider);
			return options;
		}

		static string ParseHttpMessageHandler (string value)
		{
			switch (value) {
			// default
			case null:
				return HttpClientHandlerValue;
			case HttpClientHandlerValue:
				if (Driver.App.Platform == Utils.ApplePlatform.WatchOS)
					throw ErrorHelper.CreateError (2015, "Invalid HttpMessageHandler `{0}` for watchOS. Valid values are NSUrlSessionHandler (default) or CFNetworkHandler", value);
				return value;
			case CFNetworkHandlerValue:
			case NSUrlSessionHandlerValue:
				return value;
			default:
				throw ErrorHelper.CreateError (2010, "Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler", value);
			}
		}

		static string ParseTlsProvider (string value)
		{
			switch (value) {
			// default
			case null:
				return DefaultTlsProviderValue;
			case LegacyTlsProviderValue:
			case DefaultTlsProviderValue:
			case AppleTlsProviderValue:
				return value;
			default:
				throw ErrorHelper.CreateError (2011, "Unknown TlsProvider `{0}`.  Valid values are default, legacy or appletls", value);
			}
		}



		string GenerateMessageHandlerValue ()
		{
#if MONOMAC
			// Unlike iOS, XM has this each in different namespaces...
			switch (http_message_handler)
			{
				case HttpClientHandlerValue:
					return "System.Net.Http.HttpClientHandler, System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a</string>";
				case CFNetworkHandlerValue:
					return "System.Net.Http.CFNetworkHandler, Xamarin.Mac, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065</string>";
				case NSUrlSessionHandlerValue:
					return "Foundation.NSUrlSessionHandler, Xamarin.Mac, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065</string>";
				default:
					throw ErrorHelper.CreateError (2010, "Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler", http_message_handler);
			}
#else
			return "System.Net.Http." + http_message_handler + ", System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a</string>";
#endif
		}

		internal void Write (string app_dir)
		{
			// note: we always create the file because the simulator won't remove old files
			// that might become useful if we add new options in the future
			var content = new StringBuilder ();
			content.AppendLine ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			content.AppendLine ("<!DOCTYPE plist PUBLIC \\\"-//Apple//DTD PLIST 1.0//EN\\\" \\\"http://www.apple.com/DTDs/PropertyList-1.0.dtd\\\">");
			content.AppendLine ("<plist version=\\\"1.0\\\">");
			content.AppendLine ("<dict>");
			content.AppendLine ("<key>HttpMessageHandler</key>");
			content.Append ("<string>");
			content.AppendLine (GenerateMessageHandlerValue ());
			content.AppendLine ("<key>TlsProvider</key>");
			content.Append ("<string>");
			content.Append (tls_provider);
			content.AppendLine ("</string>");
			content.AppendLine ("</dict>");
			content.AppendLine ("</plist>");

			var file_name = GetFileName (app_dir);
			File.WriteAllText (file_name, content.ToString ());
		}

		// Called from CoreTlsProviderStep
		internal static TypeDefinition GetTlsProvider (RuntimeOptions options, ModuleDefinition module)
		{
			var provider = options != null ? options.tls_provider : DefaultTlsProviderValue;
			TypeDefinition type;
			switch (provider) {
			case DefaultTlsProviderValue:
			case AppleTlsProviderValue:
				type = module.GetType (Namespaces.Security + ".Tls.AppleTlsProvider");
				break;
			case LegacyTlsProviderValue:
				type = module.GetType (Namespaces.Security + ".Tls.OldTlsProvider");
				break;
			default:
				throw new InvalidOperationException (string.Format ("Unknown TlsProvider `{0}`.", provider));
			}
			if (type == null)
				throw new InvalidOperationException (string.Format ("Cannot load TlsProvider `{0}`.", provider));
			return type;
		}

		// Called from CoreHttpMessageHandler
		internal static TypeDefinition GetHttpMessageHandler (RuntimeOptions options, ModuleDefinition httpModule, ModuleDefinition platformModule = null)
		{
			string handler;

			if (options != null) {
				handler = options.http_message_handler;
			} else if (Driver.App.Platform == Utils.ApplePlatform.WatchOS) {
				handler = NSUrlSessionHandlerValue;
			} else {
				handler = HttpClientHandlerValue;
			}
			TypeDefinition type;
			switch (handler) {
#if MONOMAC
			case HttpClientHandlerValue:
				type = httpModule.GetType ("System.Net.Http", "HttpClientHandler");
				break;
			case CFNetworkHandlerValue:
				type = platformModule.GetType ("System.Net.Http", "CFNetworkHandler");
				break;
			case NSUrlSessionHandlerValue:
				type = platformModule.GetType ("Foundation", "NSUrlSessionHandler");
				break;
#else
			case HttpClientHandlerValue:
				if (Driver.App.Platform == Utils.ApplePlatform.WatchOS)
					throw ErrorHelper.CreateError (2015, "Invalid HttpMessageHandler `{0}` for watchOS. Valid values are NSUrlSessionHandler (default) or CFNetworkHandler", handler);
				type = httpModule.GetType ("System.Net.Http", "HttpClientHandler");
				break;
			case CFNetworkHandlerValue:
				type = httpModule.GetType ("System.Net.Http", "CFNetworkHandler");
				break;
			case NSUrlSessionHandlerValue:
				type = httpModule.GetType ("System.Net.Http", "NSUrlSessionHandler");
				break;
#endif
			default:
				throw new InvalidOperationException (string.Format ("Unknown HttpMessageHandler `{0}`.", handler));
			}
			if (type == null)
				throw new InvalidOperationException (string.Format ("Cannot load HttpMessageHandler `{0}`.", handler));
			return type;
		}
#else
		internal static RuntimeOptions Read ()
		{
			var top_level = NSBundle.MainBundle.BundlePath;
			var plist_path = GetFileName (top_level);

			if (!File.Exists (plist_path))
				return null;

			using (var plist = NSDictionary.FromFile (plist_path)) {
				var options = new RuntimeOptions ();
				options.http_message_handler = (NSString) plist ["HttpMessageHandler"];
				options.tls_provider = (NSString) plist ["TlsProvider"];
				return options;
			}
		}
		
#if !COREBUILD && (XAMARIN_APPLETLS || XAMARIN_NO_TLS)
		internal static MonoTlsProvider GetTlsProvider ()
		{
#if XAMARIN_NO_TLS
			return new OldTlsProvider ();
#else
			var options = Read ();
			if (options == null)
				return null;

			switch (options.tls_provider) {
			case null:
			case DefaultTlsProviderValue:
			case AppleTlsProviderValue:
				return new AppleTlsProvider ();
			case LegacyTlsProviderValue:
				return new OldTlsProvider ();
			default:
				throw new InvalidOperationException (string.Format ("Invalid TLS Provider `{0}'.", options.tls_provider));
			}
#endif
		}
#endif

#if SYSTEM_NET_HTTP || (MONOMAC && XAMCORE_2_0)
#if MONOMAC
		[Preserve]
#endif
		internal static HttpMessageHandler GetHttpMessageHandler ()
		{
			var options = RuntimeOptions.Read ();
			if (options == null) {
#if MONOTOUCH_WATCH
				return new NSUrlSessionHandler ();
#else
				return new HttpClientHandler ();
#endif
			}

			// all types will be present as this is executed only when the linker is not enabled
			var handler_name = options.http_message_handler;
			var t = Type.GetType (handler_name, false);

			HttpMessageHandler handler = null;
			if (t != null)
				handler = Activator.CreateInstance (t) as HttpMessageHandler;
			if (handler != null)
				return handler;
#if MONOTOUCH_WATCH
			Console.WriteLine ("{0} is not a valid HttpMessageHandler, defaulting to NSUrlSessionHandler", handler_name);
			return new NSUrlSessionHandler ();
#else
			Console.WriteLine ("{0} is not a valid HttpMessageHandler, defaulting to System.Net.Http.HttpClientHandler", handler_name);
			return new HttpClientHandler ();
#endif
		}
#endif
#endif

		// Use either Create() or Read().
		RuntimeOptions ()
		{
		}

		static string GetFileName (string app_dir)
		{
#if MONOMAC
			return Path.Combine (app_dir, "Contents", "Resources", "runtime-options.plist");
#else
			return Path.Combine (app_dir, "runtime-options.plist");
#endif
		}
	}
}
