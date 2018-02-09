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
#endif

namespace CoreAudioKit {
#if XAMCORE_2_0 || !MONOMAC
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
