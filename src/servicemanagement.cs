using System;

using Foundation;

using ObjCRuntime;

namespace ServiceManagement {
	[Native]
	[NoWatch, NoTV, NoiOS, MacCatalyst (16, 0), Mac (13, 0)]
	public enum SMAppServiceStatus : long {
		NotRegistered,
		Enabled,
		RequiresApproval,
		NotFound,
	}

	// @interface SMAppService : NSObject
	[NoWatch, NoTV, NoiOS, MacCatalyst (16, 0), Mac (13, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SMAppService {
		[Static]
		[Export ("loginItemServiceWithIdentifier:")]
		SMAppService CreateLoginItemService (string identifier);

		[Static]
		[Export ("mainAppService")]
		SMAppService MainApp { get; }

		[Static]
		[Export ("agentServiceWithPlistName:")]
		SMAppService CreateAgentService (string plistName);

		[Static]
		[Export ("daemonServiceWithPlistName:")]
		SMAppService CreateDaemonService (string plistName);

		[Export ("registerAndReturnError:")]
		bool Register ([NullAllowed] out NSError error);

		[Wrap ("Register (out var _)")]
		bool Register ();

		[Export ("unregisterAndReturnError:")]
		bool Unregister ([NullAllowed] out NSError error);

		[Wrap ("Unregister (out var _)")]
		bool Unregister ();

		[Async]
		[Export ("unregisterWithCompletionHandler:")]
		void Unregister (Action<NSError> handler);

		[Export ("status")]
		SMAppServiceStatus Status { get; }

		[Static]
		[Export ("statusForLegacyURL:")]
		SMAppServiceStatus GetStatus (NSUrl legacyUrl);

		[Static]
		[Export ("openSystemSettingsLoginItems")]
		void OpenSystemSettingsLoginItems ();
	}
}
