using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if LEGACY_TOOLS || BUNDLER
using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Registrar;
#endif

using Xamarin.Utils;

#nullable enable

public class Framework {
	public required string Namespace { get; set; }
	public required string Name { get; set; } // this is the name to pass to the linker when linking. This can be an umbrella framework.
	public string? SubFramework { get; set; } // if Name is an umbrella framework, this is the name of the actual sub framework.
	public required Version Version { get; set; }
	public Version? VersionAvailableInSimulator { get; set; }
	public bool AlwaysWeakLinked { get; set; }
	public bool Unavailable { private get; set; } // Call IsFrameworkUnavailable () to determine if a framework is available or not
	public Version? VersionUnavailable { get; set; } // if null, always unavailable (if Unavailable=true)

	public string LibraryPath {
		get {
			if (string.IsNullOrEmpty (SubFramework)) {
				return $"/System/Library/Frameworks/{Name}.framework/{Name}";
			} else {
				return $"/System/Library/Frameworks/{Name}.framework/Versions/A/Frameworks/{SubFramework}.framework/{SubFramework}";
			}
		}
	}

#if LEGACY_TOOLS || BUNDLER
	bool IsFrameworkAvailableInSimulator (Application app)
	{
		if (VersionAvailableInSimulator is null)
			return false;

		if (VersionAvailableInSimulator > app.SdkVersion)
			return false;

		return true;
	}
#endif

#if LEGACY_TOOLS || BUNDLER
	public bool IsFrameworkUnavailable (Application app)
	{
		if (app.IsSimulatorBuild && !IsFrameworkAvailableInSimulator (app))
			return true;

		if (!Unavailable)
			return false;

		if (VersionUnavailable is null)
			return true;

		return app.SdkVersion >= VersionUnavailable;
	}
#else
	public bool IsFrameworkUnavailable ()
	{
		return Unavailable;
	}
#endif
}

public class Frameworks : Dictionary<string, Framework> {
	public void Add (string @namespace, int major_version)
	{
		Add (@namespace, @namespace, new Version (major_version, 0));
	}

	public void Add (string @namespace, string framework, int major_version)
	{
		Add (@namespace, framework, new Version (major_version, 0));
	}

	public void Add (string @namespace, int major_version, int minor_version)
	{
		Add (@namespace, @namespace, new Version (major_version, minor_version));
	}

	public void Add (string @namespace, int major_version, int minor_version, string? subFramework = null)
	{
		Add (@namespace, @namespace, new Version (major_version, minor_version), subFramework: subFramework);
	}

	public void Add (string @namespace, string framework, int major_version, bool alwaysWeakLink)
	{
		Add (@namespace, framework, new Version (major_version, 0), null, alwaysWeakLink);
	}

	public void Add (string @namespace, string framework, int major_version, int minor_version)
	{
		Add (@namespace, framework, new Version (major_version, minor_version));
	}

	public void Add (string @namespace, string framework, int major_version, int minor_version, string? umbrellaFramework = null)
	{
		Add (@namespace, framework, new Version (major_version, minor_version), subFramework: umbrellaFramework);
	}

	public void Add (string @namespace, string framework, int major_version, int minor_version, int build_version)
	{
		Add (@namespace, framework, new Version (major_version, minor_version, build_version));
	}

	public void Add (string @namespace, string framework, Version version, Version? version_available_in_simulator = null, bool alwaysWeakLink = false, string? subFramework = null, Version? version_unavailable = null)
	{
		var fr = new Framework () {
			Namespace = @namespace,
			Name = framework,
			Version = version,
			VersionAvailableInSimulator = version_available_in_simulator ?? version,
			AlwaysWeakLinked = alwaysWeakLink,
			SubFramework = subFramework,
			Unavailable = version_unavailable is not null,
			VersionUnavailable = version_unavailable,
		};
		base.Add (fr.Namespace, fr);
	}

	public Framework? Find (string framework)
	{
		foreach (var kvp in this)
			if (kvp.Value.Name == framework)
				return kvp.Value;
		return null;
	}

	static readonly Version NotAvailableInSimulator = new Version (int.MaxValue, int.MaxValue);

	static Frameworks? mac_frameworks;
	public static Frameworks MacFrameworks {
		get {
			if (mac_frameworks is null) {
				mac_frameworks = new Frameworks () {
					{ "Accelerate", 10, 0 },
					{ "AppKit", 10, 0 },
					{ "CoreAudio", "CoreAudio", 10, 0 },
					{ "CoreFoundation", "CoreFoundation", 10, 0 },
					{ "CoreGraphics", "ApplicationServices", 10, 0, "CoreGraphics" },
					// The CoreImage framework by itself was introduced in 10.11
					// Up until 10.10 it was a sub framework in the QuartzCore umbrella framework
					// They both existed until 10.13, when the sub framework was removed.
					{ "CoreImage", 10, 0 },
					{ "Foundation", 10, 0 },
					{ "ImageKit", "Quartz", 10, 0, "ImageKit" },
					{ "PdfKit", "Quartz", 10, 0, "PDFKit" },
					{ "Security", 10, 0 },

					{ "GSS", "GSS", 10, 1 },

					{ "AddressBook", 10, 2 },
					{ "AudioUnit", 10, 2 },
					{ "CoreMidi", "CoreMIDI", 10, 2 },
					{ "IOBluetooth", 10, 2 },
					{ "IOBluetoothUI", 10, 2 },
					{ "WebKit", 10, 2},

					{ "AudioToolbox", 10, 3 },
					{ "CoreServices", 10, 3 },
					{ "CoreVideo", 10, 3 },
					{ "MobileCoreServices", "CoreServices", 10, 3 },
					{ "OpenGL", 10, 3 },
					{ "SearchKit", "CoreServices", 10,3, "SearchKit" },
					{ "SecurityInterface", 10, 3 },
					{ "SystemConfiguration", 10, 3 },

					{ "CoreData", 10, 4 },
					{ "ImageIO", 10, 4 },  // it's own framework since at least 10.9
					{ "OpenAL", 10, 4 },

					{ "CoreAnimation", "QuartzCore", 10, 5 },
					{ "CoreText", 10, 5 }, // it's own framework since at least 10.9
					{ "PrintCore", "ApplicationServices", 10,5, "PrintCore" },
					{ "ScriptingBridge", 10, 5 },
					{ "QuickLook", 10, 5 },
					{ "QuartzComposer", "Quartz", 10, 5, "QuartzComposer" },
					{ "ImageCaptureCore", "ImageCaptureCore", 10,5 },

					{ "ServiceManagement", 10, 6 },
					{ "QuickLookUI", "Quartz", 10, 6, "QuickLookUI" },

					{ "MediaToolbox", 10, 9 },
					{ "AVFoundation", 10, 7 },
					{ "CoreLocation", 10, 7 },
					{ "CoreMedia", 10, 7 },
					{ "CoreWlan", "CoreWLAN", 10, 7 },
					{ "StoreKit", 10, 7 },

					{ "Accounts", 10, 8 },
					{ "AudioVideoBridging", 10, 8 },
					{ "CFNetwork", 10, 8 },
					{ "EventKit", 10, 8 },
					{ "GameKit", 10, 8 },
					{ "GLKit", 10, 8 },
					{ "SceneKit", 10, 8 },
					{ "Social", 10, 8 },
					{ "VideoToolbox", 10, 8 },

					{ "AVKit", 10, 9 },
					// The CoreBluetooth framework was added as a sub framework of the IOBluetooth framework in 10.7
					// Then it was moved to its own top-level framework in 10.10
					// and the sub framework was deleted in 10.12
					{ "CoreBluetooth", 10, 9 },
					{ "GameController", 10, 9 },
					{ "MapKit", 10, 9 },
					{ "MediaAccessibility", 10, 9 },
					{ "MediaLibrary", 10, 9 },
					{ "SpriteKit", 10, 9 },
					{ "JavaScriptCore", "JavaScriptCore", 10, 9 },

					{ "CloudKit", 10, 10 },
					{ "CryptoTokenKit", 10, 10 },
					{ "FinderSync", 10, 10 },
					{ "Hypervisor", 10, 10 },
					{ "LocalAuthentication", 10, 10 },
					{ "MultipeerConnectivity", 10, 10 },
					{ "NetworkExtension", 10, 10 },
					{ "NotificationCenter", 10, 10 },

					{ "Contacts", 10, 11 },
					{ "ContactsUI", 10, 11 },
					{ "CoreAudioKit", 10,11 },
					{ "GameplayKit", 10, 11 },
					{ "Metal", 10, 11 },
					{ "MetalKit", 10, 11 },
					{ "ModelIO", 10, 11 },

					{ "Intents", 10, 12 },
					{ "IntentsUI", 12, 0 },
					{ "IOSurface", "IOSurface", 10, 12 },
					{ "Photos", "Photos", 10,12 },
					{ "PhotosUI", "PhotosUI", 10,12 },
					{ "SafariServices", "SafariServices", 10, 12 },
					{ "MediaPlayer", "MediaPlayer", 10, 12, 1 },

					{ "CoreML", "CoreML", 10, 13 },
					{ "CoreSpotlight", "CoreSpotlight", 10,13 },
					{ "ExternalAccessory", "ExternalAccessory", 10, 13 },
					{ "MetalPerformanceShaders", "MetalPerformanceShaders", 10, 13 },
					{ "Vision", "Vision", 10, 13 },

					{ "BusinessChat", "BusinessChat", 10, 13, 4 },

					{ "AdSupport", "AdSupport", 10,14 },
					{ "NaturalLanguage", "NaturalLanguage", 10,14 },
					{ "Network", "Network", 10, 14 },
					{ "VideoSubscriberAccount", "VideoSubscriberAccount", 10,14 },
					{ "UserNotifications", "UserNotifications", 10,14 },
					{ "iTunesLibrary", "iTunesLibrary", 10,14 },

					{ "AuthenticationServices", "AuthenticationServices", 10,15 },
					{ "CoreMotion", "CoreMotion", 10,15 },
					{ "DeviceCheck", "DeviceCheck", 10,15 },
					{ "ExecutionPolicy", "ExecutionPolicy", 10,15 },
					{ "FileProvider", "FileProvider", 10,15 },
					{ "FileProviderUI", "FileProviderUI", 10,15 },
					{ "PushKit", "PushKit", 10,15 },
					{ "QuickLookThumbnailing", "QuickLookThumbnailing", 10,15 },
					{ "SoundAnalysis", "SoundAnalysis", 10,15 },
					{ "PencilKit", "PencilKit", 10,15 },
					{ "Speech", "Speech", 10,15 },
					{ "LinkPresentation", "LinkPresentation", 10,15 },
					{ "CoreHaptics", "CoreHaptics", 10, 15 },

					{ "AutomaticAssessmentConfiguration", "AutomaticAssessmentConfiguration", 10,15,4 },

					{ "Accessibility", "Accessibility", 11,0 },
					{ "AppTrackingTransparency", "AppTrackingTransparency", 11,0 },
					{ "CallKit", "CallKit", 11,0 },
					{ "ClassKit", "ClassKit", 11,0 },
					{ "MetalPerformanceShadersGraph", "MetalPerformanceShadersGraph", 11, 0 },
					{ "MLCompute", "MLCompute", 11,0 },
					{ "NearbyInteraction", "NearbyInteraction", 11,0 },
					{ "OSLog", "OSLog", 11,0 },
					{ "PassKit", "PassKit", 11,0 },
					{ "ReplayKit", "ReplayKit", 11,0 },
					{ "ScreenTime", "ScreenTime", 11,0 },
					{ "UniformTypeIdentifiers", "UniformTypeIdentifiers", 11,0 },
					{ "UserNotificationsUI", "UserNotificationsUI", 11,0 },

					{ "AdServices", "AdServices", 11,1 },

					{ "DataDetection", "DataDetection", 12, 0 },
					{ "LocalAuthenticationEmbeddedUI", "LocalAuthenticationEmbeddedUI", 12, 0 },
					{ "MailKit", "MailKit", 12, 0 },
					{ "MetricKit", 12, 0 },
					{ "Phase", "PHASE", 12, 0 },
					{ "ShazamKit", "ShazamKit", 12,0 },

					{ "ScreenCaptureKit", "ScreenCaptureKit", 12,3 },

					{ "AVRouting", "AVRouting", 13,0},
					{ "BackgroundAssets", "BackgroundAssets", 13,0},
					{ "HealthKit", "HealthKit", 13,0 },
					{ "MetalFX", "MetalFX", 13, 0 },
					{ "SafetyKit", "SafetyKit", 13, 0 },
					{ "SharedWithYou", "SharedWithYou", 13,0 },
					{ "SharedWithYouCore", "SharedWithYouCore", 13, 0 },
					{ "ExtensionKit", "ExtensionKit", 13,0 },
					{ "ThreadNetwork", "ThreadNetwork", 13,0 },

					{ "Cinematic", "Cinematic", 14,0 },
					{ "Symbols", "Symbols", 14, 0 },
					{ "SensitiveContentAnalysis", "SensitiveContentAnalysis", 14, 0 },
					{ "BrowserEngineKit", "BrowserEngineKit", 14, 3},

					{ "DeviceDiscoveryExtension", "DeviceDiscoveryExtension", 15, 0},
					{ "MediaExtension", "MediaExtension", 15, 0 },

					{ "FSKit", "FSKit", 15, 4 },
					{ "SecurityUI", "SecurityUI", 15, 4 },

					{ "GameSave", "GameSave", 26, 0 },

					{ "AccessoryAccess", "AccessoryAccess", 27, 0 },
					{ "LinkSecurity", "LinkSecurity", 27, 0 },
					{ "StateReporting", "StateReporting", 27, 0 },
				};
			}
			return mac_frameworks;
		}
	}

	static Frameworks? ios_frameworks;
	public static Frameworks iOSFrameworks {
		get {
			if (ios_frameworks is null) {
				ios_frameworks = new Frameworks () {
					{ "AddressBook", "AddressBook", 3 },
					{ "Security", "Security", 3 },
					{ "AudioUnit", "AudioToolbox", 3 },
					{ "AddressBookUI", "AddressBookUI", 3 },
					{ "AudioToolbox", "AudioToolbox", 3 },
					{ "AVFoundation", "AVFoundation", 3 },
					{ "CFNetwork", "CFNetwork", 3 },
					{ "CoreAnimation", "QuartzCore", 3 },
					{ "CoreAudio", "CoreAudio", 3 },
					{ "CoreData", "CoreData", 3 },
					{ "CoreFoundation", "CoreFoundation", 3 },
					{ "CoreGraphics", "CoreGraphics", 3 },
					{ "CoreLocation", "CoreLocation", 3 },
					{ "ExternalAccessory", "ExternalAccessory", 3 },
					{ "Foundation", "Foundation", 3 },
					{ "GameKit", "GameKit", 3 },
					{ "MapKit", "MapKit", 3 },
					{ "MediaPlayer", "MediaPlayer", 3 },
					{ "MessageUI", "MessageUI", 3 },
					{ "MobileCoreServices", "MobileCoreServices", 3 },
					{ "StoreKit", "StoreKit", 3 },
					{ "SystemConfiguration", "SystemConfiguration", 3 },
					{ "OpenGLES", "OpenGLES", 3 },
					{ "UIKit", "UIKit", 3 },

					{ "Accelerate", "Accelerate", 4 },
					{ "AssetsLibrary", "AssetsLibrary", new Version (4, 0), null, false, null, /* version_unavailable = */ new Version (17, 4) },
					{ "EventKit", "EventKit", 4 },
					{ "EventKitUI", "EventKitUI", 4 },
					{ "CoreMotion", "CoreMotion", 4 },
					{ "CoreMedia", "CoreMedia", 4 },
					{ "CoreVideo", "CoreVideo", 4 },
					{ "CoreTelephony", "CoreTelephony", 4 },
					{ "QuickLook", "QuickLook", 4 },
					{ "ImageIO", "ImageIO", 4 },
					{ "CoreText", "CoreText", 4 },
					{ "CoreMidi", "CoreMIDI", 4 },

					{ "Accounts", "Accounts", 5 },
					{ "GLKit", "GLKit", 5 },
					{ "NewsstandKit", "NewsstandKit", new Version (5, 0), null, false, null, /* version_unavailable = */ new Version (17, 0) },
					{ "CoreImage", "CoreImage", 5 },
					{ "CoreBluetooth", "CoreBluetooth", 5 },
					{ "Twitter", "Twitter", 5 },
					{ "GSS", "GSS", 5 },

					{ "MediaToolbox", "MediaToolbox", 6 },
					{ "PassKit", "PassKit", 6 },
					{ "Social", "Social", 6 },
					{ "AdSupport", "AdSupport", 6 },

					{ "GameController", "GameController", 7 },
					{ "JavaScriptCore", "JavaScriptCore", 7 },
					{ "MediaAccessibility", "MediaAccessibility", 7 },
					{ "MultipeerConnectivity", "MultipeerConnectivity", 7 },
					{ "SafariServices", "SafariServices", 7 },
					{ "SpriteKit", "SpriteKit", 7 },

					{ "HealthKit", "HealthKit", 8 },
					{ "HomeKit", "HomeKit", 8 },
					{ "LocalAuthentication", "LocalAuthentication", 8 },
					{ "NotificationCenter", "NotificationCenter", 8 },
					{ "PushKit", "PushKit", 8 },
					{ "Photos", "Photos", 8 },
					{ "PhotosUI", "PhotosUI", 8 },
					{ "SceneKit", "SceneKit", 8 },
					{ "CloudKit", "CloudKit", 8 },
					{ "AVKit", "AVKit", 8 },
					{ "CoreAudioKit", "CoreAudioKit", new Version (8, 0), new Version (9, 0) },
					{ "Metal", "Metal", new Version (8, 0), new Version (9, 0) },
					{ "WebKit", "WebKit", 8 },
					{ "NetworkExtension", "NetworkExtension", 8 },
					{ "VideoToolbox", "VideoToolbox", 8 },

					{ "ReplayKit", "ReplayKit", 9 },
					{ "Contacts", "Contacts", 9 },
					{ "ContactsUI", "ContactsUI", 9 },
					{ "CoreSpotlight", "CoreSpotlight", 9 },
					{ "WatchConnectivity", "WatchConnectivity", 9 },
					{ "ModelIO", "ModelIO", 9 },
					{ "MetalKit", "MetalKit", 9 },
					{ "MetalPerformanceShaders", "MetalPerformanceShaders", new Version (9, 0), new Version (11, 0) /* MPS got simulator headers in Xcode 9 */ },
					{ "GameplayKit", "GameplayKit", 9 },
					{ "HealthKitUI", "HealthKitUI", 9, 3 },

					{ "CallKit", "CallKit", 10 },
					{ "Messages", "Messages", 10 },
					{ "Speech", "Speech", 10 },
					{ "VideoSubscriberAccount", "VideoSubscriberAccount", 10 },
					{ "UserNotifications", "UserNotifications", 10 },
					{ "UserNotificationsUI", "UserNotificationsUI", 10 },
					{ "Intents", "Intents", 10 },
					{ "IntentsUI", "IntentsUI", 10 },

					{ "ARKit", "ARKit", 11 },
					{ "CoreNFC", "CoreNFC", new Version (11, 0), new Version (15, 0), true }, /* not always present, e.g. iPad w/iOS 12, so must be weak linked; doesn't work in the simulator in Xcode 12 (https://stackoverflow.com/q/63915728/183422), but works in at least Xcode 15 (maybe earlier too) */
					{ "DeviceCheck", "DeviceCheck", new Version (11, 0), new Version (13, 0) },
					{ "IdentityLookup", "IdentityLookup", 11 },
					{ "IOSurface", "IOSurface", new Version (11, 0), new Version (26, 0) /* The headers were broken at some point, not sure when they started working again */ },
					{ "CoreML", "CoreML", 11 },
					{ "Vision", "Vision", 11 },
					{ "FileProvider", "FileProvider", 11 },
					{ "FileProviderUI", "FileProviderUI", 11 },
					{ "PdfKit", "PDFKit", 11 },

					{ "BusinessChat", "BusinessChat", 11, 3 },

					{ "ClassKit", "ClassKit", 11, 4 },

					{ "AuthenticationServices", "AuthenticationServices", 12, 0 },
					{ "CarPlay", "CarPlay", 12, 0 },
					{ "CoreServices", "MobileCoreServices", 12, 0 },
					{ "IdentityLookupUI", "IdentityLookupUI", 12, 0 },
					{ "NaturalLanguage", "NaturalLanguage", 12, 0 },
					{ "Network", "Network", 12, 0 },

					{ "BackgroundTasks", "BackgroundTasks", 13, 0 },
					{ "CoreHaptics", "CoreHaptics", 13, 0 },
					{ "CryptoTokenKit", "CryptoTokenKit", 13, 0 },
					{ "LinkPresentation", "LinkPresentation", 13, 0 },
					{ "MetricKit", "MetricKit", 13, 0 },
					{ "PencilKit", "PencilKit", 13, 0 },
					{ "QuickLookThumbnailing", "QuickLookThumbnailing", 13, 0 },
					{ "SoundAnalysis", "SoundAnalysis", 13, 0 },
					{ "VisionKit", "VisionKit", 13, 0 },

					{ "AutomaticAssessmentConfiguration", "AutomaticAssessmentConfiguration", 13, 4 },

					{ "Accessibility", "Accessibility", 14, 0 },
					{ "AppClip", "AppClip", 14, 0 },
					{ "AppTrackingTransparency", "AppTrackingTransparency", 14, 0 },
					{ "MediaSetup", "MediaSetup", new Version (14, 0), NotAvailableInSimulator /* no headers in beta 3 */ },
					{ "MetalPerformanceShadersGraph", "MetalPerformanceShadersGraph", 14, 0 },
					{ "MLCompute", "MLCompute", new Version (14, 0), NotAvailableInSimulator },
					{ "NearbyInteraction", "NearbyInteraction", 14, 0 },
					{ "ScreenTime", "ScreenTime", 14, 0 },
					{ "SensorKit", "SensorKit", new Version (14, 0), null, true }, /* not always present on device, e.g. any iPad, so must be weak linked; https://github.com/dotnet/macios/issues/9938 */
					{ "UniformTypeIdentifiers", "UniformTypeIdentifiers", 14, 0 },

					{ "AdServices", "AdServices", 14, 3 },

					{ "CoreLocationUI", "CoreLocationUI", 15, 0 },

					{ "DataDetection", "DataDetection", 15, 0 },
					{ "Phase", "PHASE", new Version (15, 0), new Version (26, 0) /* not certain about the exact version when this framework was added to the simulator, but this should be a safe default */ },
					{ "OSLog", "OSLog", 15, 0 },
					{ "ShazamKit", "ShazamKit", new Version (15, 0), new Version (16, 0) },
					{ "ThreadNetwork", "ThreadNetwork", new Version (15, 0), NotAvailableInSimulator },


					{ "AVRouting", "AVRouting", 16, 0 },
					{ "BackgroundAssets", "BackgroundAssets", 16, 0 },
					{ "DeviceDiscoveryExtension", "DeviceDiscoveryExtension", 16, 0 },
					{ "MetalFX", "MetalFX", new Version (16, 0), NotAvailableInSimulator },
					{ "PushToTalk", "PushToTalk", new Version (16, 0), new Version (16, 2) /* available to build with, although it's unusable */},
					{ "SafetyKit", "SafetyKit", 16, 0 },
					{ "SharedWithYou", "SharedWithYou", 16, 0 },
					{ "SharedWithYouCore", "SharedWithYouCore", 16, 0 },

					{ "Cinematic", "Cinematic", new Version (17, 0), NotAvailableInSimulator },
					{ "Symbols", "Symbols", 17, 0 },
					{ "SensitiveContentAnalysis", "SensitiveContentAnalysis", 17, 0 },
					{ "BrowserEngineCore", "BrowserEngineCore", 17, 4 },
					{ "BrowserEngineKit", "BrowserEngineKit", 17, 4 },

					{ "AccessorySetupKit", "AccessorySetupKit", 18, 0 },

					{ "SecurityUI", "SecurityUI", 18, 4 },

					{ "DeviceDiscoveryUI", "DeviceDiscoveryUI", 26, 0 },
					{ "ExtensionKit", "ExtensionKit", 26, 0 },
					{ "GameSave", "GameSave", 26, 0 },
					{ "TouchController", "TouchController", 26, 0 },

					{ "AVSystemRouting", "AVSystemRouting", new Version (27, 0), NotAvailableInSimulator },
					{ "LinkSecurity", "LinkSecurity", 27, 0 },
					{ "ScreenCaptureKit", "ScreenCaptureKit", new Version (27, 0), NotAvailableInSimulator },
					{ "StateReporting", "StateReporting", 27, 0 },
					// the above MUST be kept in sync with simlauncher
					// see tools/mtouch/Makefile
					// please also keep it sorted to ease comparison
					// 
					// The following tests also need to be updated:
					// 
					// * RegistrarTest.MT4134
				};
			}
			return ios_frameworks;
		}
	}

	static Frameworks? tvos_frameworks;
	public static Frameworks TVOSFrameworks {
		get {
			if (tvos_frameworks is null) {
				tvos_frameworks = new Frameworks () {
					{ "AVFoundation", "AVFoundation", 9 },
					{ "AVKit", "AVKit", 9 },
					{ "Accelerate", "Accelerate", 9 },
					{ "AdSupport", "AdSupport", 9 },
					{ "AudioToolbox", "AudioToolbox", 9 },
					{ "AudioUnit", "AudioToolbox", 9 },
					{ "CFNetwork", "CFNetwork", 9 },
					{ "CloudKit", "CloudKit", 9 },
					{ "CoreAnimation", "QuartzCore", 9 },
					{ "CoreAudio", "CoreAudio", 9 },
					{ "CoreBluetooth", "CoreBluetooth", 9 },
					{ "CoreData", "CoreData", 9 },
					{ "CoreFoundation", "CoreFoundation", 9 },
					{ "CoreGraphics", "CoreGraphics", 9 },
					{ "CoreImage", "CoreImage", 9 },
					{ "CoreLocation", "CoreLocation", 9 },
					{ "CoreMedia", "CoreMedia", 9 },
					{ "CoreSpotlight", "CoreSpotlight", 9 },
					{ "CoreText", "CoreText", 9 },
					{ "CoreVideo", "CoreVideo", 9 },
					{ "Foundation", "Foundation", 9 },
					{ "GLKit", "GLKit", 9 },
					{ "GameController", "GameController", 9 },
					{ "GameKit", "GameKit", 9 },
					{ "GameplayKit", "GameplayKit", 9 },
					{ "ImageIO", "ImageIO", 9 },
					{ "JavaScriptCore", "JavaScriptCore", 9 },
					{ "MediaAccessibility", "MediaAccessibility", 9 },
					{ "MediaPlayer", "MediaPlayer", 9 },
					{ "MediaToolbox", "MediaToolbox", 9 },
					{ "Metal", "Metal", 9 },
					{ "MetalKit", "MetalKit", new Version (9, 0), new Version (10, 0) },
					{ "MetalPerformanceShaders", "MetalPerformanceShaders", new Version (9, 0), new Version (18, 0) /* not available in the simulator in tvOS 9.0, but available in tvOS 18.0. Not investigated about earlier versions. */ },
					{ "MobileCoreServices", "MobileCoreServices", 9 },
					{ "ModelIO", "ModelIO", 9 },
					{ "OpenGLES", "OpenGLES", 9 },
					{ "SceneKit", "SceneKit", 9 },
					{ "Security", "Security", 9 },
					{ "SpriteKit", "SpriteKit", 9 },
					{ "StoreKit", "StoreKit", 9 },
					{ "SystemConfiguration", "SystemConfiguration", 9 },
					{ "TVMLKit", "TVMLKit", 9 },
					{ "TVServices", "TVServices", 9 },
					{ "UIKit", "UIKit", 9 },

					{ "MapKit", "MapKit", 9, 2 },

					{ "ExternalAccessory", "ExternalAccessory", 10 },
					{ "HomeKit", "HomeKit", 10 },
					{ "MultipeerConnectivity", 10 },
					{ "Photos", "Photos", 10 },
					{ "PhotosUI", "PhotosUI", 10 },
					{ "ReplayKit", "ReplayKit", 10 },
					{ "UserNotifications", "UserNotifications", 10 },
					{ "VideoSubscriberAccount", "VideoSubscriberAccount", 10 },
					{ "VideoToolbox", "VideoToolbox", 10,2 },

					{ "DeviceCheck", "DeviceCheck", new Version (11, 0), new Version (13, 0) },
					{ "CoreML", "CoreML", 11 },
					{ "IOSurface", "IOSurface", new Version (11, 0), new Version (26, 0) /* The headers were broken at some point, not sure when they started working again */ },
					{ "Vision", "Vision", 11 },

					{ "CoreServices", "MobileCoreServices", 12 },
					{ "NaturalLanguage", "NaturalLanguage", 12,0 },
					{ "Network", "Network", 12, 0 } ,
					{ "TVUIKit", "TVUIKit", 12,0 },

					{ "AuthenticationServices", "AuthenticationServices", 13,0 },
					{ "CryptoTokenKit", "CryptoTokenKit", 13, 0 },
					{ "SoundAnalysis", "SoundAnalysis", 13,0 },
					{ "BackgroundTasks", "BackgroundTasks", 13, 0 },

					{ "Accessibility", "Accessibility", 14,0 },
					{ "AppTrackingTransparency", "AppTrackingTransparency", 14,0 },
					{ "CoreHaptics", "CoreHaptics", 14, 0 },
					{ "LinkPresentation", "LinkPresentation", 14,0 },
					{ "MetalPerformanceShadersGraph", "MetalPerformanceShadersGraph", new Version (14, 0), NotAvailableInSimulator /* not available in the simulator */ },
					{ "MLCompute", "MLCompute", new Version (14,0), NotAvailableInSimulator },
					{ "UniformTypeIdentifiers", "UniformTypeIdentifiers", 14,0 },
					{ "Intents", "Intents", 14,0 },

					{ "DataDetection", "DataDetection", 15, 0 },
					{ "DeviceDiscoveryUI", "DeviceDiscoveryUI", 16,0 },
					{ "OSLog", "OSLog", 15,0 },
					{ "CoreMidi", "CoreMIDI", 15,0 },
					{ "ShazamKit", "ShazamKit", new Version (15, 0), new Version (16, 0) },
					{ "SharedWithYou", "SharedWithYou", 16,0 },
					{ "SharedWithYouCore", "SharedWithYouCore", 16,0 },

					{ "Cinematic", "Cinematic", new Version (17, 0), NotAvailableInSimulator },
					{ "Symbols", "Symbols", 17, 0 },
					{ "NetworkExtension", "NetworkExtension", 17, 0 },
					{ "Phase", "PHASE", new Version (17,0), NotAvailableInSimulator },
					{ "BrowserEngineKit", "BrowserEngineKit", new Version (17, 4), new Version (26, 0) },

					{ "PdfKit", "PDFKit", 18, 2 },

					{ "BackgroundAssets", "BackgroundAssets", 18, 4 },
					{ "MetalFX", "MetalFX", new Version (18, 4), NotAvailableInSimulator },
					{ "SecurityUI", "SecurityUI", 18, 4 },

					{ "AVRouting", "AVRouting", 26, 0 },

					{ "LinkSecurity", "LinkSecurity", 27, 0 },
					{ "ScreenCaptureKit", "ScreenCaptureKit", new Version (27, 0), NotAvailableInSimulator },
					{ "StateReporting", "StateReporting", 27, 0 },
				};
			}
			return tvos_frameworks;
		}
	}

	static Frameworks? catalyst_frameworks;
	public static Frameworks MacCatalystFrameworks {
		get {
			if (catalyst_frameworks is null) {
				catalyst_frameworks = iOSFrameworks;
				// We're going to mutate the value returned from iOSFrameworks, so clear the cached value so the next time iOSFrameworks is called it's re-generated.
				ios_frameworks = null;
				// not present in iOS but present in catalyst
				catalyst_frameworks.Add ("CoreWlan", "CoreWLAN", 15, 0);

				var min = new Version (13, 0);
				var v14_0 = new Version (14, 0);
				var v14_2 = new Version (14, 2);
				var v16_1 = new Version (16, 1);
				var v18_0 = new Version (18, 0);
				var v18_2 = new Version (18, 2);
				var v26_0 = new Version (26, 0);
				var v16_0 = new Version (16, 0);
				foreach (var f in catalyst_frameworks.Values) {
					switch (f.Name) {
					// These frameworks were added to Catalyst after they were added to iOS, so we have to adjust the Versions fields
					case "CoreTelephony":
					case "HomeKit":
					case "Messages":
						f.Version = v14_0;
						f.VersionAvailableInSimulator = v14_0;
						break;
					case "AddressBook":
					case "ClassKit":
					case "UserNotificationsUI":
						f.Version = v14_2;
						f.VersionAvailableInSimulator = v14_2;
						break;
					case "ThreadNetwork":
						f.Version = v16_1;
						break;
					case "Cinematic":
						f.Version = v26_0;
						break;
					case "MediaSetup":
						f.Version = v16_0;
						break;
					case "BrowserEngineKit":
					case "DeviceDiscoveryExtension":
						f.Version = v18_0;
						break;
					case "ScreenCaptureKit":
						f.Version = v18_2;
						f.VersionAvailableInSimulator = v18_2;
						break;
					// These frameworks are not available on Mac Catalyst
					case "DeviceDiscoveryUI": // xtro and introspection says it's not in Mac Catalyst, Apple's website says it is. For now, listen to xtro and introspection, until proven otherwise.
					case "OpenGLES":
					case "NotificationCenter":
					case "GLKit":
					case "VideoSubscriberAccount":
					case "AccessorySetupKit":
					case "AVSystemRouting":
					// The headers for FileProviderUI exist, but the native linker fails
					case "FileProviderUI":
					// The headers for Twitter are there, , but no documentation whatsoever online and the native linker fails too
					case "Twitter":
					// headers-based xtro reporting those are *all* unknown API for Catalyst
					case "AddressBookUI":
					case "ARKit":
					case "BrowserEngineCore":
					case "CarPlay":
					case "WatchConnectivity":
						f.Unavailable = true;
						break;
					// and nothing existed before Catalyst 13.0
					default:
						if (f.Version < min)
							f.Version = min;
						if (f.VersionAvailableInSimulator < min)
							f.VersionAvailableInSimulator = min;
						break;
					}
				}

				// Add frameworks that are not in iOS
				catalyst_frameworks.Add ("AppKit", 13, 0);
				catalyst_frameworks.Add ("ExecutionPolicy", 16, 0);
				catalyst_frameworks.Add ("ServiceManagement", 16, 0);
			}
			return catalyst_frameworks;
		}
	}

	public static bool TryGetFrameworks (ApplePlatform platform, [NotNullWhen (true)] out Frameworks? frameworks)
	{
		switch (platform) {
		case ApplePlatform.iOS:
			frameworks = iOSFrameworks;
			return true;
		case ApplePlatform.TVOS:
			frameworks = TVOSFrameworks;
			return true;
		case ApplePlatform.MacOSX:
			frameworks = MacFrameworks;
			return true;
		case ApplePlatform.MacCatalyst:
			frameworks = MacCatalystFrameworks;
			return true;
		default:
			frameworks = null;
			return false;
		}
	}

#if BUNDLER || LEGACY_TOOLS
	public static bool TryGetFramework (Application app, TypeDefinition? td, [NotNullWhen (true)] out string? framework)
	{
		framework = null;

		if (td is null)
			return false;

		if (td.HasCustomAttributes) {
			foreach (var attrib in td.CustomAttributes) {
				if (!attrib.AttributeType.Is ("ObjCRuntime", "ObjectiveCFrameworkAttribute"))
					continue;
				if (attrib.ConstructorArguments.Count != 1)
					continue;
				var arg = attrib.ConstructorArguments [0];
				if (arg.Value is not string stringArgument)
					continue;
				framework = stringArgument;
				return framework is not null;
			}
		}

#if !LEGACY_TOOLS
		if (!app.Profile.IsProductAssembly (td.Module.Assembly))
			return false;
#endif

		framework = td.Namespace;
		return framework is not null;
	}

	public static bool TryGetFramework (Application app, TypeDefinition? td, [NotNullWhen (true)] out Framework? framework)
	{
		framework = null;

		if (td is null)
			return false;

		if (!TryGetFramework (app, td, out string? frameworkName))
			return false;

		if (!TryGetFrameworks (app.Platform, out var all_frameworks))
			return false;

		return all_frameworks.TryGetValue (frameworkName, out framework);
	}

	static void Gather (Application app, IEnumerable<AssemblyDefinition> assemblies, HashSet<string> frameworks, HashSet<string> weak_frameworks, Func<Framework, bool> include_framework)
	{
		var namespaces = new HashSet<string> ();
		var hasLARight = false;

		// Process our product assembly + any assembly with the [ObjectiveCFramework] attribute, and collect all the namespaces that are used in those assemblies.
		// For non-product assemblies, we only look at types with the [ObjectiveCFramework] attribute.
		foreach (var assembly in assemblies) {
#if LEGACY_TOOLS
			var hasObjectiveCFrameworkAttribute = true;
#else
			var hasObjectiveCFrameworkAttribute = false;
			if (!app.Profile.IsProductAssembly (assembly)) {
				hasObjectiveCFrameworkAttribute = assembly.MainModule.HasTypeReference ("ObjCRuntime.ObjectiveCFrameworkAttribute");
				if (!hasObjectiveCFrameworkAttribute)
					continue;
			}
#endif

			// Collect all the namespaces.
			foreach (var md in assembly.Modules) {
				foreach (var td in md.Types) {
					if (hasObjectiveCFrameworkAttribute && !td.HasCustomAttribute ("ObjCRuntime", "ObjectiveCFrameworkAttribute"))
						continue;

					if (TryGetFramework (app, td, out string? framework)) {
						namespaces.Add (framework);
						if (td.Namespace == "LocalAuthentication" && td.Name == "LARight") {
#if !LEGACY_TOOLS
							if (app.Profile.IsProductAssembly (assembly))
#endif
							hasLARight = true;
						}
						continue;
					}
				}
			}
		}

		// Iterate over all the namespaces and check which frameworks we need to link with.
		if (!TryGetFrameworks (app.Platform, out var all_frameworks))
			return;

		foreach (var nspace in namespaces) {
			if (!all_frameworks.TryGetValue (nspace, out var framework))
				continue;

			if (!include_framework (framework))
				continue;

			if (app.SdkVersion < framework.Version) {
				// We're building with an old sdk, and the framework doesn't exist there.
				continue;
			}

			if (framework.IsFrameworkUnavailable (app))
				continue;

			var weak_link = framework.AlwaysWeakLinked || app.DeploymentTarget < framework.Version;
			var add_to = weak_link ? weak_frameworks : frameworks;
			add_to.Add (framework.Name);

			// LARight's presentation-context authorization is implemented by this framework's category.
			if (app.Platform == ApplePlatform.iOS && hasLARight && nspace == "LocalAuthentication" && app.SdkVersion is { Major: >= 16 })
				weak_frameworks.Add ("LocalAuthenticationEmbeddedUI");
		}

		// Make sure there are no duplicates between frameworks and weak frameworks.
		// Keep the weak ones.
		frameworks.ExceptWith (weak_frameworks);
	}

	static bool FilterFrameworks (Application app, Framework framework)
	{
		if (framework.IsFrameworkUnavailable (app)) {
			app.Log (3, "Not linking with the framework {0} because it's not available in the current SDK.", framework.Name);
			return false;
		}

		switch (app.Platform) {
		case ApplePlatform.iOS:
		case ApplePlatform.TVOS:
		case ApplePlatform.MacCatalyst:
		case ApplePlatform.MacOSX:
			return true; // Include all frameworks by default
		default:
			throw ErrorHelper.CreateError (71, Errors.MX0071 /* "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/dotnet/macios/issues/new with a test case." */, app.Platform, app.ProductName);
		}
	}

	public static void Gather (Application app, IEnumerable<AssemblyDefinition> assemblies, HashSet<string> frameworks, HashSet<string> weak_frameworks)
	{
		Gather (app, assemblies, frameworks, weak_frameworks, (framework) => FilterFrameworks (app, framework));
	}
#endif // BUNDLER
}
