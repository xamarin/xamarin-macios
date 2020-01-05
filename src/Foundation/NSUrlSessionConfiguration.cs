//
// NSUrlSessionHandlerConfiguration.cs:
//
// Authors:
//     Manuel de la Pena <mandel@microsoft.com>
using System;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch.Foundation;
#endif

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

		SessionConfigurationType configurationType = SessionConfigurationType.Default; 

		public SessionConfigurationType SessionType {
			get => configurationType;
			private set => configurationType = value;
		}

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

		public static NSUrlSessionConfiguration CreateBackgroundSessionConfiguration (string identifier)
		{
			var config = NSUrlSessionConfiguration._CreateBackgroundSessionConfiguration (identifier);
			config.SessionType = SessionConfigurationType.Background;
			return config;
		}
	}
}