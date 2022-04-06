//
// NSUrlSessionHandlerConfiguration.cs:
//
// Authors:
//     Manuel de la Pena <mandel@microsoft.com>
using System;
using ObjCRuntime;

using Foundation;

namespace Foundation {

	// the following was added to make the use of the configuration easier for the NUrlSessionHandler. 
	// Apple APIs do not give an easy way to know the type of configuration that was created, this is an 
	// issue when we want to interact with the cookie containers, since depending on the configuration type
	// the cookie container can be shared or not. This code should be transparent to the user, and is only used internaly.
	public partial class NSUrlSessionConfiguration 
	{
		public enum SessionConfigurationType {
			Default,
			Background,
			Ephemeral,
		}

		public SessionConfigurationType SessionType { get; private set; } = SessionConfigurationType.Default; 

		public static NSUrlSessionConfiguration DefaultSessionConfiguration {
			get {
				var config = NSUrlSessionConfiguration._DefaultSessionConfiguration;
				config.SessionType = SessionConfigurationType.Default;
				return config;
			}
		}

		public static NSUrlSessionConfiguration EphemeralSessionConfiguration {
			get {
				var config = NSUrlSessionConfiguration._EphemeralSessionConfiguration;
				config.SessionType = SessionConfigurationType.Ephemeral;
				return config;
			}
		}

#if NET
		[UnsupportedOSPlatform ("macos10.10")]
		[UnsupportedOSPlatform ("ios8.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.10 use 'CreateBackgroundSessionConfiguration' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios8.0 use 'CreateBackgroundSessionConfiguration' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'CreateBackgroundSessionConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'CreateBackgroundSessionConfiguration' instead.")]
#endif
		public static NSUrlSessionConfiguration BackgroundSessionConfiguration (string identifier)
		{
			var config = NSUrlSessionConfiguration._BackgroundSessionConfiguration (identifier);
			config.SessionType = SessionConfigurationType.Background;
			return config;
		}

		public static NSUrlSessionConfiguration CreateBackgroundSessionConfiguration (string identifier)
		{
			var config = NSUrlSessionConfiguration._CreateBackgroundSessionConfiguration (identifier);
			config.SessionType = SessionConfigurationType.Background;
			return config;
		}

	}
}
