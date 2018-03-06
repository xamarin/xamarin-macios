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
using ObjCRuntime;
#elif MONOMAC && !MMP
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#elif !MTOUCH && !MMP && !MMP_TEST
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

#if !COREBUILD && (XAMARIN_APPLETLS || XAMARIN_NO_TLS)
#if !MTOUCH && !MMP && !MMP_TEST
using Mono.Security.Interface;
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

#if MTOUCH || MMP
		/*
		 * This section is only used by the tools
		 */
		internal static RuntimeOptions Create (Application app, string http_message_handler, string tls_provider)
		{
			var options = new RuntimeOptions ();
			options.http_message_handler = ParseHttpMessageHandler (app, http_message_handler);
			ParseTlsProvider (tls_provider);
			return options;
		}

		static string ParseHttpMessageHandler (Application app, string value)
		{
			switch (value) {
			// default
			case null:
				return HttpClientHandlerValue;
			case CFNetworkHandlerValue:
			case HttpClientHandlerValue:
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, "Invalid HttpMessageHandler `{0}` for watchOS. The only valid value is NSUrlSessionHandler.", value);
					return NSUrlSessionHandlerValue;
				}
				return value;
			case NSUrlSessionHandlerValue:
				return value;
			default:
				if (app.Platform == Utils.ApplePlatform.WatchOS) // This is value we don't know about at all, show as error instead of warning.
					throw ErrorHelper.CreateError (2015, "Invalid HttpMessageHandler `{0}` for watchOS. The only valid value is NSUrlSessionHandler.", value);
				throw ErrorHelper.CreateError (2010, "Unknown HttpMessageHandler `{0}`. Valid values are HttpClientHandler (default), CFNetworkHandler or NSUrlSessionHandler", value);
			}
		}

		static string ParseTlsProvider (string value)
		{
			switch (value) {
			// default
			case null:
				return DefaultTlsProviderValue;
			case DefaultTlsProviderValue:
			case AppleTlsProviderValue:
				return value;
			case LegacyTlsProviderValue:
				ErrorHelper.Warning (2016, "Invalid TlsProvider `{0}` option. The only valid value `{1}` will be used.", value, AppleTlsProviderValue);
				return AppleTlsProviderValue;
			default:
				throw ErrorHelper.CreateError (2011, "Unknown TlsProvider `{0}`.  Valid values are default or appletls", value);
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
			content.AppendLine ("</dict>");
			content.AppendLine ("</plist>");

			var file_name = GetFileName (app_dir);
			File.WriteAllText (file_name, content.ToString ());
		}

		// Called from CoreHttpMessageHandler
		internal static TypeDefinition GetHttpMessageHandler (Application app, RuntimeOptions options, ModuleDefinition httpModule, ModuleDefinition platformModule = null)
		{
			string handler;

			if (options != null) {
				handler = options.http_message_handler;
			} else if (app.Platform == Utils.ApplePlatform.WatchOS) {
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
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, "Invalid HttpMessageHandler `{0}` for watchOS. The only valid value is NSUrlSessionHandler.", handler);
					type = httpModule.GetType ("System.Net.Http", "NSUrlSessionHandler");
				} else {
					type = httpModule.GetType ("System.Net.Http", "HttpClientHandler");
				}
				break;
			case CFNetworkHandlerValue:
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, "Invalid HttpMessageHandler `{0}` for watchOS. The only valid value is NSUrlSessionHandler.", handler);
					type = httpModule.GetType ("System.Net.Http", "NSUrlSessionHandler");
				} else {
					type = httpModule.GetType ("System.Net.Http", "CFNetworkHandler");
				}
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
			// for iOS NSBundle.ResourcePath returns the path to the root of the app bundle
			// for macOS apps NSBundle.ResourcePath returns foo.app/Contents/Resources
			// for macOS frameworks NSBundle.ResourcePath returns foo.app/Versions/Current/Resources
			Class bundle_finder = new Class (typeof (NSObject.NSObject_Disposer));
			var resource_dir = NSBundle.FromClass (bundle_finder).ResourcePath;
			var plist_path = GetFileName (resource_dir);

			if (!File.Exists (plist_path))
				return null;

			using (var plist = NSDictionary.FromFile (plist_path)) {
				var options = new RuntimeOptions ();
				options.http_message_handler = (NSString) plist ["HttpMessageHandler"];
				return options;
			}
		}
		
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

		static string GetFileName (string resource_dir)
		{
			return Path.Combine (resource_dir, "runtime-options.plist");
		}
	}
}
