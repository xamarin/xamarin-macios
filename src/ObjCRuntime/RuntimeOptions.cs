using System;
using System.IO;
using System.Text;

#if MTOUCH || MMP || BUNDLER
using Mono.Cecil;
using Xamarin.Linker;
#else
using System.Net.Http;
using Foundation;
using ObjCRuntime;
#endif

#nullable enable

#if MMP || MMP_TEST || MTOUCH || BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	class RuntimeOptions {
#if NET
		const string SocketsHandlerValue = "SocketsHttpHandler";
#else
		const string HttpClientHandlerValue = "HttpClientHandler";
#endif
		const string CFNetworkHandlerValue = "CFNetworkHandler";
		const string NSUrlSessionHandlerValue = "NSUrlSessionHandler";

		string? http_message_handler;

#if MTOUCH || MMP || BUNDLER
		/*
		 * This section is only used by the tools
		 */
		internal static RuntimeOptions Create (Application app, string? http_message_handler, string? tls_provider)
		{
			var options = new RuntimeOptions ();
			options.http_message_handler = ParseHttpMessageHandler (app, http_message_handler);
   			#if NET
    			options.UseCFNetworkHandler = _IsCFNetworkHandlerFeature;
    			options.UseNSUrlSessionHandler = _IsNSUrlSessionHandlerFeature;
   			#endif
			return options;
		}

		static string ParseHttpMessageHandler (Application app, string? value)
		{
			switch (value) {
			// default
			case null:
#if NET
				return NSUrlSessionHandlerValue;
#else
				return (app.Platform == Utils.ApplePlatform.WatchOS) ? NSUrlSessionHandlerValue : HttpClientHandlerValue;
#endif
			case CFNetworkHandlerValue:
#if NET
			case SocketsHandlerValue:
#else
			case HttpClientHandlerValue:
#endif
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, Errors.MT2015, value);
					return NSUrlSessionHandlerValue;
				}
				return value;
			case NSUrlSessionHandlerValue:
				return value;
			default:
				if (app.Platform == Utils.ApplePlatform.WatchOS) // This is value we don't know about at all, show as error instead of warning.
					throw ErrorHelper.CreateError (2015, Errors.MT2015, value);
				throw ErrorHelper.CreateError (2010, Errors.MT2010, value);
			}
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
			content.Append (http_message_handler);
			content.AppendLine ("</string>");
			content.AppendLine ("</dict>");
			content.AppendLine ("</plist>");

			var file_name = GetFileName (app_dir);
			Xamarin.Bundler.Driver.WriteIfDifferent (file_name, content.ToString ());
		}

		// Called from CoreHttpMessageHandler
		internal static TypeDefinition GetHttpMessageHandler (Application app, RuntimeOptions options, ModuleDefinition httpModule, ModuleDefinition? platformModule = null)
		{
			string? handler;

			if (options is not null) {
				handler = options.http_message_handler;
			} else if (app.Platform == Utils.ApplePlatform.WatchOS) {
				handler = NSUrlSessionHandlerValue;
			} else {
#if NET
				if (UseCFNetworkHandler)
        			return new CFNetworkHandler ();
			    else
			    {
			        if (handler_name is not null && handler_name != NSUrlSessionHandlerValue)
			            Runtime.NSLog ($"{handler_name} is not a valid HttpMessageHandler, defaulting to System.Net.Http.NSUrlSessionHandlerValue");
			        return new NSUrlSessionHandler ();
			    }
#else
				handler = HttpClientHandlerValue;
#endif
			}
			TypeDefinition type;
			switch (handler) {
#if MONOMAC
			case HttpClientHandlerValue:
				type = httpModule.GetType ("System.Net.Http", "HttpClientHandler");
				break;
			case CFNetworkHandlerValue:
				type = platformModule!.GetType ("System.Net.Http", "CFNetworkHandler");
				break;
			case NSUrlSessionHandlerValue:
				type = platformModule!.GetType ("Foundation", "NSUrlSessionHandler");
				break;
#else
#if NET
			case SocketsHandlerValue:
				type = httpModule.GetType ("System.Net.Http", "SocketsHttpHandler");
				break;
#else
			case HttpClientHandlerValue:
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, Errors.MT2015, handler);
					type = platformModule!.GetType ("System.Net.Http", "NSUrlSessionHandler");
				} else {
					type = httpModule.GetType ("System.Net.Http", "HttpClientHandler");
				}
				break;
#endif
			case CFNetworkHandlerValue:
				if (app.Platform == Utils.ApplePlatform.WatchOS) {
					ErrorHelper.Warning (2015, Errors.MT2015, handler);
					type = platformModule!.GetType ("System.Net.Http", "NSUrlSessionHandler");
				} else {
					type = platformModule!.GetType ("System.Net.Http", "CFNetworkHandler");
				}
				break;
			case NSUrlSessionHandlerValue:
				type = platformModule!.GetType ("System.Net.Http", "NSUrlSessionHandler");
				break;
#endif
			default:
				throw new InvalidOperationException (string.Format ("Unknown HttpMessageHandler `{0}`.", handler));
			}
			if (type is null)
				throw new InvalidOperationException (string.Format ("Cannot load HttpMessageHandler `{0}`.", handler));
			return type;
		}
#else

		internal static RuntimeOptions? Read ()
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

		// This is invoked by
		// System.Net.Http.dll!System.Net.Http.HttpClient.cctor
		internal static HttpMessageHandler GetHttpMessageHandler ()
		{
			var options = RuntimeOptions.Read ();
			// all types will be present as this is executed only when the linker is not enabled
			var handler_name = options?.http_message_handler;
#if NET
			// Note: no need to handle SocketsHandlerValue here because System.Net.Http handles
			// creating a SocketsHttpHandler when configured to do so.
			switch (handler_name) {
			case CFNetworkHandlerValue:
				return new CFNetworkHandler ();
			default:
				if (handler_name is not null && handler_name != NSUrlSessionHandlerValue)
					Runtime.NSLog ($"{handler_name} is not a valid HttpMessageHandler, defaulting to System.Net.Http.NSUrlSessionHandlerValue");
				return new NSUrlSessionHandler ();
			}
#elif __WATCHOS__
			if (handler_name is not null && handler_name != NSUrlSessionHandlerValue)
				Runtime.NSLog ($"{handler_name} is not a valid HttpMessageHandler, defaulting to NSUrlSessionHandler");
			return new NSUrlSessionHandler ();
#else
			switch (handler_name) {
			case CFNetworkHandlerValue:
				return new CFNetworkHandler ();
			case NSUrlSessionHandlerValue:
				return new NSUrlSessionHandler ();
			default:
				if (handler_name is not null && handler_name != HttpClientHandlerValue)
					Runtime.NSLog ($"{handler_name} is not a valid HttpMessageHandler, defaulting to System.Net.Http.HttpClientHandler");
				return new HttpClientHandler ();
			}
#endif
		}
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
