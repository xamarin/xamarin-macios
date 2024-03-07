//
// Authors:
//  Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2014-2015 Xamarin, Inc.
//
// While the framework exists on both platforms, they share no common API
//

using System;
using System.ComponentModel;

using AudioUnit;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreGraphics;
#if MONOMAC
using AppKit;
using AUViewControllerBase = AppKit.NSViewController;
using UIViewController = AppKit.NSViewController;
#else
using UIKit;
using AUViewControllerBase = UIKit.UIViewController;
using NSView = Foundation.NSObject;
using NSWindow = Foundation.NSObject;
using NSWindowController = Foundation.NSObject;
using NSViewController = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreAudioKit {
	[NoiOS]
	[NoMacCatalyst]
	[Flags]
	public enum AUGenericViewDisplayFlags : uint {
		TitleDisplay = 1u << 0,
		PropertiesDisplay = 1u << 1,
		ParametersDisplay = 1u << 2,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUViewControllerBase))]
	interface AUViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface AUAudioUnitViewConfiguration : NSSecureCoding {
		[Export ("initWithWidth:height:hostHasController:")]
		NativeHandle Constructor (nfloat width, nfloat height, bool hostHasController);

		[Export ("width")]
		nfloat Width { get; }

		[Export ("height")]
		nfloat Height { get; }

		[Export ("hostHasController")]
		bool HostHasController { get; }
	}

	[Category]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (AUAudioUnit))]
	interface AUAudioUnitViewControllerExtensions {
		[Export ("supportedViewConfigurations:")]
		NSIndexSet GetSupportedViewConfigurations (AUAudioUnitViewConfiguration [] availableViewConfigurations);

		[Export ("selectViewConfiguration:")]
		void SelectViewConfiguration (AUAudioUnitViewConfiguration viewConfiguration);
	}

	[NoiOS]
	[NoMacCatalyst]
	[Protocol]
	interface AUCustomViewPersistentData {

		[Abstract]
		[NullAllowed, Export ("customViewPersistentData", ArgumentSemantic.Assign)]
		NSDictionary<NSString, NSObject> CustomViewPersistentData { get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[DisableDefaultCtor] // Crashes
	[BaseType (typeof (NSView))]
	interface AUGenericView : AUCustomViewPersistentData {

		[Export ("audioUnit")]
		AudioUnit.AudioUnit AudioUnit { get; }

		[Export ("showsExpertParameters")]
		bool ShowsExpertParameters { get; set; }

		[Export ("initWithAudioUnit:")]
		NativeHandle Constructor (AudioUnit.AudioUnit au);

		[Export ("initWithAudioUnit:displayFlags:")]
		NativeHandle Constructor (AudioUnit.AudioUnit au, AUGenericViewDisplayFlags inFlags);
	}

	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSView))]
	[DisableDefaultCtor]
	interface AUPannerView {

		[Export ("audioUnit")]
		AudioUnit.AudioUnit AudioUnit { get; }

		[Static]
		[Export ("AUPannerViewWithAudioUnit:")]
		AUPannerView Create (AudioUnit.AudioUnit au);
	}

	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSWindowController), Name = "CABTLEMIDIWindowController")]
	interface CABtleMidiWindowController {

		[Export ("initWithWindow:")]
		NativeHandle Constructor ([NullAllowed] NSWindow window);
	}

	[NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSViewController))]
	interface CAInterDeviceAudioViewController {

		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
	}

	[NoiOS]
	[NoMacCatalyst]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSWindowController))]
	interface CANetworkBrowserWindowController {

		[Export ("initWithWindow:")]
		NativeHandle Constructor ([NullAllowed] NSWindow window);

		[Static]
		[Export ("isAVBSupported")]
		bool IsAvbSupported { get; }
	}

#if !MONOMAC
	[NoMac]
	[MacCatalyst (13, 1)]
	// in iOS 8.3 (Xcode 6.3 SDK) the base type was changed from UIViewController to UITableViewController
	[BaseType (typeof (UITableViewController), Name = "CABTMIDICentralViewController")]
	interface CABTMidiCentralViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[MacCatalyst (13, 1)]
		[Export ("initWithStyle:")]
		NativeHandle Constructor (UITableViewStyle withStyle);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Name = "CABTMIDILocalPeripheralViewController")]
	interface CABTMidiLocalPeripheralViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioUnit' instead.")]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AudioUnit' instead.")]
	[BaseType (typeof (UIView))]
	interface CAInterAppAudioSwitcherView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect bounds);

		[Export ("showingAppNames")]
		bool ShowingAppNames { [Bind ("isShowingAppNames")] get; set; }

		[Export ("setOutputAudioUnit:")]
		void SetOutputAudioUnit ([NullAllowed] AudioUnit.AudioUnit audioUnit);

		[Export ("contentWidth")]
		nfloat ContentWidth ();
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'AudioUnit' instead.")]
	[NoMacCatalyst]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AudioUnit' instead.")]
	[BaseType (typeof (UIView))]
	interface CAInterAppAudioTransportView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect bounds);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("playing")]
		bool Playing { [Bind ("isPlaying")] get; }

		[Export ("recording")]
		bool Recording { [Bind ("isRecording")] get; }

		[Export ("connected")]
		bool Connected { [Bind ("isConnected")] get; }

		[Export ("labelColor", ArgumentSemantic.Retain)]
		UIColor LabelColor { get; set; }

		// [NullAllowed] // by default this property is null
		// NSInvalidArgumentException *** -[__NSPlaceholderDictionary initWithObjects:forKeys:count:]: attempt to insert nil object from objects[0]
		[Export ("currentTimeLabelFont", ArgumentSemantic.Retain)]
		UIFont CurrentTimeLabelFont { get; set; }

		[Export ("rewindButtonColor", ArgumentSemantic.Retain)]
		UIColor RewindButtonColor { get; set; }

		[Export ("playButtonColor", ArgumentSemantic.Retain)]
		UIColor PlayButtonColor { get; set; }

		[Export ("pauseButtonColor", ArgumentSemantic.Retain)]
		UIColor PauseButtonColor { get; set; }

		[Export ("recordButtonColor", ArgumentSemantic.Retain)]
		UIColor RecordButtonColor { get; set; }

		[Export ("setOutputAudioUnit:")]
		void SetOutputAudioUnit (AudioUnit.AudioUnit audioUnit);
	}
#endif

	[Mac (13, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (UIViewController))]
	interface AUGenericViewController {

		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed, Export ("auAudioUnit", ArgumentSemantic.Strong)]
		AUAudioUnit AuAudioUnit { get; set; }
	}
}
