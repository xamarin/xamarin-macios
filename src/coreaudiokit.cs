///
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
#else
using UIKit;
using AUViewControllerBase = UIKit.UIViewController;
using NSView = Foundation.NSObject;
using NSWindow = Foundation.NSObject;
using NSWindowController = Foundation.NSObject;
using NSViewController = Foundation.NSObject;
#endif

namespace CoreAudioKit {
#if XAMCORE_2_0 || !MONOMAC

	[NoiOS]
	[Mac (10,11, onlyOn64 : true)]
	[Flags]
	public enum AUGenericViewDisplayFlags : uint {
		TitleDisplay = 1u << 0,
		PropertiesDisplay = 1u << 1,
		ParametersDisplay = 1u << 2
	}

	[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
	[BaseType (typeof(AUViewControllerBase))]
	interface AUViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[iOS (11,0)][Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSObject))]
	interface AUAudioUnitViewConfiguration : NSSecureCoding {
		[Export ("initWithWidth:height:hostHasController:")]
		IntPtr Constructor (nfloat width, nfloat height, bool hostHasController);

		[Export ("width")]
		nfloat Width { get; }

		[Export ("height")]
		nfloat Height { get; }

		[Export ("hostHasController")]
		bool HostHasController { get; }
	}

	[Category]
	[iOS (11,0)][Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (AUAudioUnit))]
	interface AUAudioUnitViewControllerExtensions {
		[Export ("supportedViewConfigurations:")]
		NSIndexSet GetSupportedViewConfigurations (AUAudioUnitViewConfiguration [] availableViewConfigurations);

		[Export ("selectViewConfiguration:")]
		void SelectViewConfiguration (AUAudioUnitViewConfiguration viewConfiguration);
	}

	[NoiOS]
	[Mac (10,13, onlyOn64: true)]
	[Protocol]
	interface AUCustomViewPersistentData {

		[Abstract]
		[NullAllowed, Export ("customViewPersistentData", ArgumentSemantic.Assign)]
		NSDictionary<NSString, NSObject> CustomViewPersistentData { get; set; }
	}

	[NoiOS]
	[Mac (10,13, onlyOn64: true)]
	[DisableDefaultCtor] // Crashes
	[BaseType (typeof (NSView))]
	interface AUGenericView : AUCustomViewPersistentData {

		[Export ("audioUnit")]
		AudioUnit.AudioUnit AudioUnit { get; }

		[Export ("showsExpertParameters")]
		bool ShowsExpertParameters { get; set; }

		[Export ("initWithAudioUnit:")]
		IntPtr Constructor (AudioUnit.AudioUnit au);

		[Export ("initWithAudioUnit:displayFlags:")]
		IntPtr Constructor (AudioUnit.AudioUnit au, AUGenericViewDisplayFlags inFlags);
	}

	[NoiOS]
	[Mac (10,13, onlyOn64: true)]
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
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSWindowController), Name = "CABTLEMIDIWindowController")]
	interface CABtleMiDiWindowController {

		[Export ("initWithWindow:")]
		IntPtr Constructor ([NullAllowed] NSWindow window);
	}

	[NoiOS]
	[Mac (10,13, onlyOn64: true)]
	[BaseType (typeof (NSViewController))]
	interface CAInterDeviceAudioViewController {

		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);
	}

	[NoiOS]
	[Mac (10,13, onlyOn64: true)]
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSWindowController))]
	interface CANetworkBrowserWindowController {

		[Export ("initWithWindow:")]
		IntPtr Constructor ([NullAllowed] NSWindow window);

		[Static]
		[Export ("isAVBSupported")]
		bool IsAvbSupported { get; }
	}
#endif

#if !MONOMAC
	[iOS (8,0)]
	// in iOS 8.3 (Xcode 6.3 SDK) the base type was changed from UIViewController to UITableViewController
	[BaseType (typeof (UITableViewController), Name="CABTMIDICentralViewController")]
	interface CABTMidiCentralViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[iOS (8,3)]
		[Export ("initWithStyle:")]
		IntPtr Constructor (UITableViewStyle withStyle);
	}

	[iOS (8,0)]
	[BaseType (typeof (UIViewController), Name="CABTMIDILocalPeripheralViewController")]
	interface CABTMidiLocalPeripheralViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
	}

	[iOS (8,0)]
	[BaseType (typeof (UIView))]
	interface CAInterAppAudioSwitcherView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect bounds);
		
		[Export ("showingAppNames")]
		bool ShowingAppNames { [Bind ("isShowingAppNames")] get; set; }

		[Export ("setOutputAudioUnit:")]
		void SetOutputAudioUnit ([NullAllowed] AudioUnit.AudioUnit audioUnit);

		[Export ("contentWidth")]
		nfloat ContentWidth ();
	}

	[iOS (8,0)]
	[BaseType (typeof (UIView))]
	interface CAInterAppAudioTransportView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect bounds);
		
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
		void SetOutputAudioUnit ([NullAllowed] AudioUnit.AudioUnit audioUnit);
	}
#endif
}
