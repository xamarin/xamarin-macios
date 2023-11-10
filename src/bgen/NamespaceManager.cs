using System;
using System.Collections.Generic;
using ObjCRuntime;

#nullable enable

public class NamespaceManager {
	PlatformName CurrentPlatform => Frameworks!.CurrentPlatform;
	Frameworks? Frameworks { get; set; }

	// Where user-overrideable messaging may live
	public string ObjCRuntime { get; private set; }

	public string Messaging { get; private set; }

	public ICollection<string> StandardNamespaces { get; private set; }
	public ICollection<string> UINamespaces { get; private set; }
	public ICollection<string> ImplicitNamespaces { get; private set; }
	public ICollection<string> NamespacesThatConflictWithTypes { get; private set; }

	public NamespaceManager (PlatformName currentPlatform, string customObjCRuntimeNS, bool skipSystemDrawing)
	{
		Frameworks = new (currentPlatform);
		ObjCRuntime = String.IsNullOrEmpty (customObjCRuntimeNS)
			? "ObjCRuntime"
			: customObjCRuntimeNS;

		Messaging = ObjCRuntime + ".Messaging";

		StandardNamespaces = new HashSet<string> {
			"Foundation",
			"ObjCRuntime",
			"CoreGraphics",
		};

		if (Frameworks is null)
			// we are in a bad state
			throw ErrorHelper.CreateError (3, CurrentPlatform);

		UINamespaces = new HashSet<string> ();
		if (Frameworks.HaveAppKit)
			UINamespaces.Add ("AppKit");
		if (Frameworks.HaveUIKit)
			UINamespaces.Add ("UIKit");
		if (Frameworks.HaveTwitter)
			UINamespaces.Add ("Twitter");
		if (Frameworks.HaveGameKit && CurrentPlatform != PlatformName.MacOSX && CurrentPlatform != PlatformName.WatchOS)
			UINamespaces.Add ("GameKit");
		if (Frameworks.HaveNewsstandKit)
			UINamespaces.Add ("NewsstandKit");
		if (Frameworks.HaveiAd)
			UINamespaces.Add ("iAd");
		if (Frameworks.HaveQuickLook)
			UINamespaces.Add ("QuickLook");
		if (Frameworks.HaveEventKitUI)
			UINamespaces.Add ("EventKitUI");
		if (Frameworks.HaveAddressBookUI)
			UINamespaces.Add ("AddressBookUI");
		if (Frameworks.HaveMapKit && CurrentPlatform != PlatformName.MacOSX && CurrentPlatform != PlatformName.TvOS && CurrentPlatform != PlatformName.WatchOS)
			UINamespaces.Add ("MapKit");
		if (Frameworks.HaveMessageUI)
			UINamespaces.Add ("MessageUI");
		if (Frameworks.HavePhotosUI)
			UINamespaces.Add ("PhotosUI");
		if (Frameworks.HaveHealthKitUI)
			UINamespaces.Add ("HealthKitUI");

		ImplicitNamespaces = new HashSet<string> ();
		ImplicitNamespaces.Add ("System");
		ImplicitNamespaces.Add ("System.Runtime.InteropServices");
		ImplicitNamespaces.Add ("System.Diagnostics");
		ImplicitNamespaces.Add ("System.Diagnostics.CodeAnalysis");
		ImplicitNamespaces.Add ("System.ComponentModel");
#if NET
		ImplicitNamespaces.Add ("System.Runtime.Versioning");
#endif
		ImplicitNamespaces.Add ("System.Threading.Tasks");
		ImplicitNamespaces.Add ("CoreFoundation");
		ImplicitNamespaces.Add ("Foundation");
		ImplicitNamespaces.Add ("ObjCRuntime");
		ImplicitNamespaces.Add ("CoreGraphics");
		ImplicitNamespaces.Add ("CoreML");
		ImplicitNamespaces.Add ("SceneKit");

		if (Frameworks.HaveAudioUnit)
			ImplicitNamespaces.Add ("AudioUnit");
		if (Frameworks.HaveContacts)
			ImplicitNamespaces.Add ("Contacts");
		if (Frameworks.HaveCoreAnimation)
			ImplicitNamespaces.Add ("CoreAnimation");
		if (Frameworks.HaveCoreLocation)
			ImplicitNamespaces.Add ("CoreLocation");
		if (Frameworks.HaveCoreVideo)
			ImplicitNamespaces.Add ("CoreVideo");
		if (Frameworks.HaveCoreMedia)
			ImplicitNamespaces.Add ("CoreMedia");
		if (Frameworks.HaveSecurity && CurrentPlatform != PlatformName.WatchOS)
			ImplicitNamespaces.Add ("Security");
		if (Frameworks.HaveAVFoundation)
			ImplicitNamespaces.Add ("AVFoundation");
		if (Frameworks.HaveOpenGL)
			ImplicitNamespaces.Add ("OpenGL");
#if !NET
		if (Frameworks.HaveQTKit)
			ImplicitNamespaces.Add ("QTKit");
#endif
		if (Frameworks.HaveAppKit)
			ImplicitNamespaces.Add ("AppKit");
		if (Frameworks.HaveCloudKit && CurrentPlatform != PlatformName.WatchOS && CurrentPlatform != PlatformName.TvOS && CurrentPlatform != PlatformName.iOS)
			ImplicitNamespaces.Add ("CloudKit");
		if (Frameworks.HaveCoreMotion && CurrentPlatform != PlatformName.WatchOS && CurrentPlatform != PlatformName.TvOS && CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add ("CoreMotion");
		if (Frameworks.HaveMapKit && CurrentPlatform != PlatformName.WatchOS && CurrentPlatform != PlatformName.TvOS && CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add ("MapKit");
		if (Frameworks.HaveUIKit)
			ImplicitNamespaces.Add ("UIKit");
		if (Frameworks.HaveNewsstandKit)
			ImplicitNamespaces.Add ("NewsstandKit");
		if (Frameworks.HaveGLKit && CurrentPlatform != PlatformName.WatchOS && CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add ("GLKit");
		if (Frameworks.HaveQuickLook && CurrentPlatform != PlatformName.WatchOS && CurrentPlatform != PlatformName.TvOS && CurrentPlatform != PlatformName.MacOSX)
			ImplicitNamespaces.Add ("QuickLook");
		if (Frameworks.HaveAddressBook)
			ImplicitNamespaces.Add ("AddressBook");

		if (Frameworks.HaveModelIO)
			ImplicitNamespaces.Add ("ModelIO");
		if (Frameworks.HaveMetal)
			ImplicitNamespaces.Add ("Metal");
		if (Frameworks.HaveMetalPerformanceShadersGraph)
			ImplicitNamespaces.Add ("MetalPerformanceShadersGraph");

		if (Frameworks.HaveCoreImage)
			ImplicitNamespaces.Add ("CoreImage");
		if (Frameworks.HavePhotos)
			ImplicitNamespaces.Add ("Photos");
		if (Frameworks.HaveMediaPlayer)
			ImplicitNamespaces.Add ("MediaPlayer");
		if (Frameworks.HaveMessages)
			ImplicitNamespaces.Add ("Messages");
		if (Frameworks.HaveGameplayKit)
			ImplicitNamespaces.Add ("GameplayKit");
		if (Frameworks.HaveSpriteKit)
			ImplicitNamespaces.Add ("SpriteKit");
		if (Frameworks.HaveFileProvider)
			ImplicitNamespaces.Add ("FileProvider");
		if (Frameworks.HaveNetworkExtension)
			ImplicitNamespaces.Add ("NetworkExtension");
		if (Frameworks.HaveNetwork)
			ImplicitNamespaces.Add ("Network");

		// These are both types and namespaces
		NamespacesThatConflictWithTypes = new HashSet<string> {
			"AudioUnit",
		};

		if (!skipSystemDrawing)
			ImplicitNamespaces.Add ("System.Drawing");
	}
}
