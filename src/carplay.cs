//
// CarPlay bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018-2019 Microsoft Corporation. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;
using MapKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CarPlay {

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitLength : NSUnit { }

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPAlertActionStyle : ulong {
		Default = 0,
		Cancel,
		Destructive,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPBarButtonType : ulong {
		Text,
		Image,
	}

	[Flags, NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPPanDirection : long {
		None = 0,
		Left = 1L << 0,
		Right = 1L << 1,
		Up = 1L << 2,
		Down = 1L << 3,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPNavigationAlertDismissalContext : ulong {
		Timeout = 0,
		UserDismissed,
		SystemDismissed,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPTripPauseReason : ulong {
		Arrived = 1,
		Loading = 2,
		Locating = 3,
		Rerouting = 4,
		ProceedToRoute = 5,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Flags]
	[Native]
	enum CPLimitableUserInterface : ulong {
		Keyboard = 1uL << 0,
		Lists = 1uL << 1,
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[Flags]
	[Native]
	enum CPContentStyle : ulong {
		Light = 1uL << 0,
		Dark = 1uL << 1,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Flags]
	[Native]
	enum CPManeuverDisplayStyle : long {
		Default,
		LeadingSymbol,
		TrailingSymbol,
		SymbolOnly,
		InstructionOnly,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPTimeRemainingColor : ulong {
		Default = 0,
		Green,
		Orange,
		Red,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Native]
	enum CPTripEstimateStyle : ulong {
		Light = 0,
		Dark,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPBarButtonStyle : long {
		None,
		Rounded,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPInformationTemplateLayout : long {
		Leading = 0,
		TwoColumn,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPListItemAccessoryType : long {
		None = 0,
		DisclosureIndicator,
		Cloud,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPListItemPlayingIndicatorLocation : long {
		Leading = 0,
		Trailing,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPMessageLeadingItem : long {
		None = 0,
		Pin,
		Star,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPMessageTrailingItem : long {
		None,
		Mute,
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Native]
	public enum CPTextButtonStyle : long {
		Normal = 0,
		Cancel,
		Confirm,
	}

	[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum CPAssistantCellPosition : long {
		Top = 0,
		Bottom,
	}

	[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum CPAssistantCellVisibility : long {
		Off = 0,
		WhileLimitedUIActive,
		Always,
	}

	[iOS (15, 0), MacCatalyst (15, 0), NoMac, NoTV, NoWatch]
	[Native]
	public enum CPAssistantCellActionType : long {
		PlayMedia = 0,
		StartCall,
	}

	[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[Native]
	public enum CPInstrumentClusterSetting : ulong {
		Unspecified,
		Enabled,
		Disabled,
		UserPreference,
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPAlertAction : NSSecureCoding {

		[Export ("initWithTitle:style:handler:")]
		NativeHandle Constructor (string title, CPAlertActionStyle style, Action<CPAlertAction> handler);

		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("initWithTitle:color:handler:")]
		NativeHandle Constructor (string title, UIColor color, Action<CPAlertAction> handler);

		[Export ("title")]
		string Title { get; }

		[Export ("style", ArgumentSemantic.Assign)]
		CPAlertActionStyle Style { get; }

		[Export ("handler", ArgumentSemantic.Copy)]
		Action<CPAlertAction> Handler { get; }

		[NullAllowed]
		[NoWatch, NoTV, NoMac, iOS (16, 0)]
		[Export ("color", ArgumentSemantic.Copy)]
		UIColor Color { get; }
	}

	delegate void CPBarButtonHandler (CPBarButton button);

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPBarButton : NSSecureCoding {

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("initWithType:handler:")]
		NativeHandle Constructor (CPBarButtonType type, [NullAllowed] Action<CPBarButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("buttonType", ArgumentSemantic.Assign)]
		CPBarButtonType ButtonType { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[iOS (14, 0)]
		[Export ("initWithImage:handler:")]
		NativeHandle Constructor (UIImage image, [NullAllowed] CPBarButtonHandler handler);

		[iOS (14, 0)]
		[Export ("initWithTitle:handler:")]
		NativeHandle Constructor (string title, [NullAllowed] CPBarButtonHandler handler);

		[iOS (14, 0)]
		[Export ("buttonStyle", ArgumentSemantic.Assign)]
		CPBarButtonStyle ButtonStyle { get; set; }
	}

	interface ICPBarButtonProviding { }

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[Protocol]
	interface CPBarButtonProviding {

		[Abstract]
		[Export ("leadingNavigationBarButtons", ArgumentSemantic.Strong)]
		CPBarButton [] LeadingNavigationBarButtons { get; set; }

		[Abstract]
		[Export ("trailingNavigationBarButtons", ArgumentSemantic.Strong)]
		CPBarButton [] TrailingNavigationBarButtons { get; set; }

#if NET
		[Abstract]
#endif
		[iOS (12, 2)]
		[NullAllowed, Export ("backButton", ArgumentSemantic.Strong)]
		CPBarButton BackButton { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPGridButton : NSSecureCoding {

		[Export ("initWithTitleVariants:image:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string [] titleVariants, UIImage image, [NullAllowed] Action<CPGridButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("image")]
		UIImage Image { get; }

		[Export ("titleVariants")]
		string [] TitleVariants { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPGridTemplate : CPBarButtonProviding {

		[Export ("initWithTitle:gridButtons:")]
		NativeHandle Constructor ([NullAllowed] string title, CPGridButton [] gridButtons);

		[Export ("gridButtons")]
		CPGridButton [] GridButtons { get; }

		[Export ("title")]
		string Title { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("updateGridButtons:")]
		void UpdateGridButtons (CPGridButton [] gridButtons);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("updateTitle:")]
		void UpdateTitle (string title);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CPGridTemplateMaximumItems")]
		nuint MaximumItems { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPInterfaceController {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPInterfaceControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[iOS (13, 0)]
		[Export ("prefersDarkUserInterfaceStyle")]
		bool PrefersDarkUserInterfaceStyle { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("setRootTemplate:animated:")]
		void SetRootTemplate (CPTemplate rootTemplate, bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("pushTemplate:animated:")]
		void PushTemplate (CPTemplate templateToPush, bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("popTemplateAnimated:")]
		void PopTemplate (bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("popToRootTemplateAnimated:")]
		void PopToRootTemplate (bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("popToTemplate:animated:")]
		void PopToTemplate (CPTemplate targetTemplate, bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("presentTemplate:animated:")]
		void PresentTemplate (CPTemplate templateToPresent, bool animated);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("dismissTemplateAnimated:")]
		void DismissTemplate (bool animated);

		[Export ("presentedTemplate")]
		[NullAllowed]
		CPTemplate PresentedTemplate { get; }

		[Export ("rootTemplate")]
		CPTemplate RootTemplate { get; }

		[NullAllowed, Export ("topTemplate", ArgumentSemantic.Strong)]
		CPTemplate TopTemplate { get; }

		[Export ("templates", ArgumentSemantic.Strong)]
		CPTemplate [] Templates { get; }

		[iOS (14, 0)]
		[Async]
		[Export ("setRootTemplate:animated:completion:")]
		void SetRootTemplate (CPTemplate rootTemplate, bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("pushTemplate:animated:completion:")]
		void PushTemplate (CPTemplate templateToPush, bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("popTemplateAnimated:completion:")]
		void PopTemplate (bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("popToRootTemplateAnimated:completion:")]
		void PopToRootTemplate (bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("popToTemplate:animated:completion:")]
		void PopToTemplate (CPTemplate targetTemplate, bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("presentTemplate:animated:completion:")]
		void PresentTemplate (CPTemplate templateToPresent, bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Async]
		[Export ("dismissTemplateAnimated:completion:")]
		void DismissTemplate (bool animated, [NullAllowed] Action<bool, NSError> completion);

		[iOS (14, 0)]
		[Export ("carTraitCollection", ArgumentSemantic.Strong)]
		UITraitCollection CarTraitCollection { get; }
	}

	interface ICPInterfaceControllerDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPInterfaceControllerDelegate {

		[Export ("templateWillAppear:animated:")]
		void TemplateWillAppear (CPTemplate aTemplate, bool animated);

		[Export ("templateDidAppear:animated:")]
		void TemplateDidAppear (CPTemplate aTemplate, bool animated);

		[Export ("templateWillDisappear:animated:")]
		void TemplateWillDisappear (CPTemplate aTemplate, bool animated);

		[Export ("templateDidDisappear:animated:")]
		void TemplateDidDisappear (CPTemplate aTemplate, bool animated);
	}

	interface ICPApplicationDelegate { }

	[Introduced (PlatformName.iOS, 12, 0)]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CPTemplateApplicationSceneDelegate' instead.")]
	[NoWatch, NoTV, NoMac]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPApplicationDelegate : UIApplicationDelegate {

		[Abstract]
		[Export ("application:didConnectCarInterfaceController:toWindow:")]
		void DidConnectCarInterfaceController (UIApplication application, CPInterfaceController interfaceController, CPWindow window);

		[Abstract]
		[Export ("application:didDisconnectCarInterfaceController:fromWindow:")]
		void DidDisconnectCarInterfaceController (UIApplication application, CPInterfaceController interfaceController, CPWindow window);

		[Export ("application:didSelectNavigationAlert:")]
		void DidSelectNavigationAlert (UIApplication application, CPNavigationAlert navigationAlert);

		[Export ("application:didSelectManeuver:")]
		void DidSelectManeuver (UIApplication application, CPManeuver maneuver);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPListItem : CPSelectableListItem, NSSecureCoding {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Do not use; this API was removed.")]
		[Field ("CPMaximumListItemImageSize")]
		CGSize MaximumListItemImageSize { get; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("initWithText:detailText:image:showsDisclosureIndicator:")]
		NativeHandle Constructor ([NullAllowed] string text, [NullAllowed] string detailText, [NullAllowed] UIImage image, bool showsDisclosureIndicator);

		[Export ("initWithText:detailText:image:")]
		NativeHandle Constructor ([NullAllowed] string text, [NullAllowed] string detailText, [NullAllowed] UIImage image);

		[Export ("initWithText:detailText:")]
		NativeHandle Constructor ([NullAllowed] string text, [NullAllowed] string detailText);

		[iOS (14, 0)]
		[Export ("initWithText:detailText:image:accessoryImage:accessoryType:")]
		NativeHandle Constructor ([NullAllowed] string text, [NullAllowed] string detailText, [NullAllowed] UIImage image, [NullAllowed] UIImage accessoryImage, CPListItemAccessoryType accessoryType);

		[NullAllowed, Export ("detailText")]
		string DetailText { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Export ("showsDisclosureIndicator")]
		bool ShowsDisclosureIndicator { get; }

		[iOS (14, 0)]
		[Export ("explicitContent")]
		bool IsExplicitContent { [Bind ("isExplicitContent")] get; set; }

		[iOS (14, 0)]
		[Export ("playbackProgress")]
		nfloat PlaybackProgress { get; set; }

		[iOS (14, 0)]
		[Export ("playing")]
		bool IsPlaying { [Bind ("isPlaying")] get; set; }

		[iOS (14, 0)]
		[Export ("playingIndicatorLocation", ArgumentSemantic.Assign)]
		CPListItemPlayingIndicatorLocation PlayingIndicatorLocation { get; set; }

		[iOS (14, 0)]
		[Static]
		[Export ("maximumImageSize")]
		CGSize MaximumImageSize { get; }

		[iOS (14, 0)]
		[Export ("setDetailText:")]
		void SetDetailText ([NullAllowed] string detailText);

		[iOS (14, 0)]
		[Export ("setImage:")]
		void SetImage ([NullAllowed] UIImage image);

		[iOS (14, 0)]
		[Export ("setAccessoryImage:")]
		void SetAccessoryImage ([NullAllowed] UIImage accessoryImage);

		[iOS (14, 0)]
		[Export ("accessoryType", ArgumentSemantic.Assign)]
		CPListItemAccessoryType AccessoryType { get; set; }

		[iOS (14, 0)]
		[NullAllowed, Export ("accessoryImage", ArgumentSemantic.Strong)]
		UIImage AccessoryImage { get; }

		[iOS (14, 0)]
		[Export ("setText:")]
		void SetText (string text);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPListSection : NSSecureCoding {

#if !XAMCORE_5_0
		[Wrap ("base (true ? throw new InvalidOperationException (Constants.BrokenBinding) : NSObjectFlag.Empty)")]
		[Obsolete ("Use '.ctor (ICPListTemplateItem [], string, string)' constructor instead. Warning: this will throw InvalidOperationException at runtime.")]
		NativeHandle Constructor (CPListItem [] items, [NullAllowed] string header, [NullAllowed] string sectionIndexTitle);
#endif

#if !XAMCORE_5_0
		[Wrap ("base (true ? throw new InvalidOperationException (Constants.BrokenBinding) : NSObjectFlag.Empty)")]
		[Obsolete ("Use '.ctor (ICPListTemplateItem [], string, string)' constructor instead. Warning: this will throw InvalidOperationException at runtime.")]
		NativeHandle Constructor (CPListItem [] items);
#endif

		[Export ("initWithItems:header:sectionIndexTitle:")]
		NativeHandle Constructor (ICPListTemplateItem [] items, [NullAllowed] string header, [NullAllowed] string sectionIndexTitle);

		[Export ("initWithItems:")]
		NativeHandle Constructor (ICPListTemplateItem [] items);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithItems:header:headerSubtitle:headerImage:headerButton:sectionIndexTitle:")]
		NativeHandle Constructor (ICPListTemplateItem [] items, string header, [NullAllowed] string headerSubtitle, [NullAllowed] UIImage headerImage, [NullAllowed] CPButton headerButton, [NullAllowed] string sectionIndexTitle);

		[NullAllowed, Export ("header")]
		string Header { get; }

		[NullAllowed, Export ("sectionIndexTitle")]
		string SectionIndexTitle { get; }

#if !XAMCORE_5_0
		[Wrap ("true ? throw new InvalidOperationException (Constants.BrokenBinding) : new NSArray ()", IsVirtual = true)]
		[Obsolete ("Use 'Items2 : ICPListTemplateItem []' instead.")]
		CPListItem [] Items { get; }
#endif

		[Export ("items", ArgumentSemantic.Copy)]
#if !XAMCORE_5_0
		ICPListTemplateItem [] Items2 { get; }
#else
		ICPListTemplateItem [] Items { get; }
#endif

		[iOS (14, 0)]
		[Export ("indexOfItem:")]
		nuint GetIndex (ICPListTemplateItem item);

		[iOS (14, 0)]
		[Export ("itemAtIndex:")]
		ICPListTemplateItem GetItem (nuint index);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("headerSubtitle")]
		string HeaderSubtitle { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("headerImage", ArgumentSemantic.Copy)]
		UIImage HeaderImage { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed]
		[Export ("headerButton", ArgumentSemantic.Copy)]
		CPButton HeaderButton { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CPMaximumListSectionImageSize")]
		CGSize MaximumImageSize { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPListTemplate : CPBarButtonProviding {

		[Export ("initWithTitle:sections:")]
		NativeHandle Constructor ([NullAllowed] string title, CPListSection [] sections);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithTitle:sections:assistantCellConfiguration:")]
		NativeHandle Constructor ([NullAllowed] string title, CPListSection [] sections, [NullAllowed] CPAssistantCellConfiguration assistantCellConfiguration);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'CPListItem.Handler' instead.")]
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPListTemplateDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'CPListItem.Handler' instead.")]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("sections", ArgumentSemantic.Copy)]
		CPListSection [] Sections { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("updateSections:")]
		void UpdateSections (CPListSection [] sections);

		[iOS (14, 0)]
		[Static]
		[Export ("maximumItemCount")]
		nuint MaximumItemCount { get; }

		[iOS (14, 0)]
		[Static]
		[Export ("maximumSectionCount")]
		nuint MaximumSectionCount { get; }

		[iOS (14, 0)]
		[Export ("sectionCount")]
		nuint SectionCount { get; }

		[iOS (14, 0)]
		[Export ("itemCount")]
		nuint ItemCount { get; }

		[iOS (14, 0)]
		[Export ("indexPathForItem:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPath (ICPListTemplateItem item);

		[iOS (14, 0)]
		[Export ("emptyViewTitleVariants", ArgumentSemantic.Copy)]
		string [] EmptyViewTitleVariants { get; set; }

		[iOS (14, 0)]
		[Export ("emptyViewSubtitleVariants", ArgumentSemantic.Copy)]
		string [] EmptyViewSubtitleVariants { get; set; }

		[NullAllowed]
		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("assistantCellConfiguration", ArgumentSemantic.Strong)]
		CPAssistantCellConfiguration AssistantCellConfiguration { get; set; }
	}

	interface ICPListTemplateDelegate { }

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'CPListItem.Handler' instead.")]
	[NoWatch, NoTV, NoMac, iOS (12, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPListTemplateDelegate {

		[Abstract]
		[Export ("listTemplate:didSelectListItem:completionHandler:")]
		void DidSelectListItem (CPListTemplate listTemplate, CPListItem item, Action completionHandler);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	interface CPManeuver : NSCopying, NSSecureCoding {

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CPManeuver.SymbolImage' instead.")]
		[NullAllowed, Export ("symbolSet", ArgumentSemantic.Strong)]
		CPImageSet SymbolSet { get; set; }

		[NullAllowed]
		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("cardBackgroundColor", ArgumentSemantic.Strong)]
		UIColor CardBackgroundColor { get; set; }

		[iOS (13, 0)]
		[NullAllowed, Export ("symbolImage", ArgumentSemantic.Strong)]
		UIImage SymbolImage { get; set; }

		[Export ("instructionVariants", ArgumentSemantic.Copy)]
		string [] InstructionVariants { get; set; }

		[NullAllowed, Export ("initialTravelEstimates", ArgumentSemantic.Strong)]
		CPTravelEstimates InitialTravelEstimates { get; set; }

		[Export ("attributedInstructionVariants", ArgumentSemantic.Copy)]
		NSAttributedString [] AttributedInstructionVariants { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }

		[iOS (12, 2)]
		[NullAllowed, Export ("junctionImage", ArgumentSemantic.Strong)]
		UIImage JunctionImage { get; set; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("dashboardSymbolImage", ArgumentSemantic.Strong)]
		UIImage DashboardSymbolImage { get; set; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("dashboardJunctionImage", ArgumentSemantic.Strong)]
		UIImage DashboardJunctionImage { get; set; }

		[iOS (14, 0)]
		[Export ("dashboardInstructionVariants", ArgumentSemantic.Copy)]
		string [] DashboardInstructionVariants { get; set; }

		[iOS (14, 0)]
		[Export ("dashboardAttributedInstructionVariants", ArgumentSemantic.Copy)]
		NSAttributedString [] DashboardAttributedInstructionVariants { get; set; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("notificationSymbolImage", ArgumentSemantic.Strong)]
		UIImage NotificationSymbolImage { get; set; }

		[iOS (14, 0)]
		[Export ("notificationInstructionVariants", ArgumentSemantic.Copy)]
		string [] NotificationInstructionVariants { get; set; }

		[iOS (14, 0)]
		[Export ("notificationAttributedInstructionVariants", ArgumentSemantic.Copy)]
		NSAttributedString [] NotificationAttributedInstructionVariants { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPMapButton : NSSecureCoding {

		[Export ("initWithHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] Action<CPMapButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("focusedImage", ArgumentSemantic.Strong)]
		UIImage FocusedImage { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPMapTemplate : CPBarButtonProviding {

		[Export ("guidanceBackgroundColor", ArgumentSemantic.Strong)]
		UIColor GuidanceBackgroundColor { get; set; }

		[Export ("tripEstimateStyle", ArgumentSemantic.Assign)]
		CPTripEstimateStyle TripEstimateStyle { get; set; }

		[Export ("mapButtons", ArgumentSemantic.Strong)]
		CPMapButton [] MapButtons { get; set; }

		[Export ("showTripPreviews:textConfiguration:")]
		void ShowTripPreviews (CPTrip [] tripPreviews, [NullAllowed] CPTripPreviewTextConfiguration textConfiguration);

		[Export ("showRouteChoicesPreviewForTrip:textConfiguration:")]
		void ShowRouteChoicesPreview (CPTrip tripPreview, [NullAllowed] CPTripPreviewTextConfiguration textConfiguration);

		[Export ("hideTripPreviews")]
		void HideTripPreviews ();

		[Export ("updateTravelEstimates:forTrip:")]
		void UpdateTravelEstimates (CPTravelEstimates estimates, CPTrip trip);

		[Export ("updateTravelEstimates:forTrip:withTimeRemainingColor:")]
		void UpdateTravelEstimates (CPTravelEstimates estimates, CPTrip trip, CPTimeRemainingColor timeRemainingColor);

		[Export ("startNavigationSessionForTrip:")]
		CPNavigationSession StartNavigationSession (CPTrip trip);

		[Export ("automaticallyHidesNavigationBar")]
		bool AutomaticallyHidesNavigationBar { get; set; }

		[Export ("hidesButtonsWithNavigationBar")]
		bool HidesButtonsWithNavigationBar { get; set; }

		[Wrap ("WeakMapDelegate")]
		[NullAllowed]
		ICPMapTemplateDelegate MapDelegate { get; set; }

		[NullAllowed, Export ("mapDelegate", ArgumentSemantic.Weak)]
		NSObject WeakMapDelegate { get; set; }

		[Export ("showPanningInterfaceAnimated:")]
		void ShowPanningInterface (bool animated);

		[Export ("dismissPanningInterfaceAnimated:")]
		void DismissPanningInterface (bool animated);

		[Export ("panningInterfaceVisible")]
		bool PanningInterfaceVisible { [Bind ("isPanningInterfaceVisible")] get; }

		[NullAllowed, Export ("currentNavigationAlert", ArgumentSemantic.Strong)]
		CPNavigationAlert CurrentNavigationAlert { get; }

		[Export ("presentNavigationAlert:animated:")]
		void PresentNavigationAlert (CPNavigationAlert navigationAlert, bool animated);

		[Async]
		[Export ("dismissNavigationAlertAnimated:completion:")]
		void DismissNavigationAlert (bool animated, Action<bool> completion);

		[iOS (14, 0)]
		[Export ("showTripPreviews:selectedTrip:textConfiguration:")]
		void ShowTripPreviews (CPTrip [] tripPreviews, [NullAllowed] CPTrip selectedTrip, [NullAllowed] CPTripPreviewTextConfiguration textConfiguration);
	}

	interface ICPMapTemplateDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPMapTemplateDelegate {

		[Export ("mapTemplate:shouldShowNotificationForManeuver:")]
		bool ShouldShowNotificationForManeuver (CPMapTemplate mapTemplate, CPManeuver maneuver);

		[Export ("mapTemplate:shouldUpdateNotificationForManeuver:withTravelEstimates:")]
		bool ShouldUpdateNotificationForManeuver (CPMapTemplate mapTemplate, CPManeuver maneuver, CPTravelEstimates travelEstimates);

		[Export ("mapTemplate:shouldShowNotificationForNavigationAlert:")]
		bool ShouldShowNotificationForNavigationAlert (CPMapTemplate mapTemplate, CPNavigationAlert navigationAlert);

		[Export ("mapTemplateDidShowPanningInterface:")]
		void DidShowPanningInterface (CPMapTemplate mapTemplate);

		[Export ("mapTemplateWillDismissPanningInterface:")]
		void WillDismissPanningInterface (CPMapTemplate mapTemplate);

		[Export ("mapTemplateDidDismissPanningInterface:")]
		void DidDismissPanningInterface (CPMapTemplate mapTemplate);

		[Export ("mapTemplate:panBeganWithDirection:")]
		void PanBegan (CPMapTemplate mapTemplate, CPPanDirection direction);

		[Export ("mapTemplate:panEndedWithDirection:")]
		void PanEnded (CPMapTemplate mapTemplate, CPPanDirection direction);

		[Export ("mapTemplate:panWithDirection:")]
		void Pan (CPMapTemplate mapTemplate, CPPanDirection direction);

		[Export ("mapTemplateDidBeginPanGesture:")]
		void DidBeginPanGesture (CPMapTemplate mapTemplate);

		[Export ("mapTemplate:didUpdatePanGestureWithTranslation:velocity:")]
		void DidUpdatePanGesture (CPMapTemplate mapTemplate, CGPoint translation, CGPoint velocity);

		[Export ("mapTemplate:didEndPanGestureWithVelocity:")]
		void DidEndPanGesture (CPMapTemplate mapTemplate, CGPoint velocity);

		[Export ("mapTemplate:willShowNavigationAlert:")]
		void WillShowNavigationAlert (CPMapTemplate mapTemplate, CPNavigationAlert navigationAlert);

		[Export ("mapTemplate:didShowNavigationAlert:")]
		void DidShowNavigationAlert (CPMapTemplate mapTemplate, CPNavigationAlert navigationAlert);

		[Export ("mapTemplate:willDismissNavigationAlert:dismissalContext:")]
		void WillDismissNavigationAlert (CPMapTemplate mapTemplate, CPNavigationAlert navigationAlert, CPNavigationAlertDismissalContext dismissalContext);

		[Export ("mapTemplate:didDismissNavigationAlert:dismissalContext:")]
		void DidDismissNavigationAlert (CPMapTemplate mapTemplate, CPNavigationAlert navigationAlert, CPNavigationAlertDismissalContext dismissalContext);

		[Export ("mapTemplate:selectedPreviewForTrip:usingRouteChoice:")]
		void SelectedPreview (CPMapTemplate mapTemplate, CPTrip trip, CPRouteChoice routeChoice);

		[Export ("mapTemplate:startedTrip:usingRouteChoice:")]
		void StartedTrip (CPMapTemplate mapTemplate, CPTrip trip, CPRouteChoice routeChoice);

		[Export ("mapTemplateDidCancelNavigation:")]
		void DidCancelNavigation (CPMapTemplate mapTemplate);

		[Export ("mapTemplate:displayStyleForManeuver:")]
		CPManeuverDisplayStyle GetDisplayStyle (CPMapTemplate mapTemplate, CPManeuver maneuver);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPNavigationAlert : NSSecureCoding {


		[Deprecated (PlatformName.iOS, 13, 0, message: "Use constructor that takes in 'UIImage' instead of 'CPImageSet'.")]
		[Export ("initWithTitleVariants:subtitleVariants:imageSet:primaryAction:secondaryAction:duration:")]
		NativeHandle Constructor (string [] titleVariants, [NullAllowed] string [] subtitleVariants, [NullAllowed] CPImageSet imageSet, CPAlertAction primaryAction, [NullAllowed] CPAlertAction secondaryAction, double duration);

		[iOS (13, 0)]
		[Export ("initWithTitleVariants:subtitleVariants:image:primaryAction:secondaryAction:duration:")]
		NativeHandle Constructor (string [] titleVariants, [NullAllowed] string [] subtitleVariants, [NullAllowed] UIImage image, CPAlertAction primaryAction, [NullAllowed] CPAlertAction secondaryAction, double duration);

		[Export ("updateTitleVariants:subtitleVariants:")]
		void UpdateTitleVariants (string [] newTitleVariants, string [] newSubtitleVariants);

		[Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[Export ("subtitleVariants", ArgumentSemantic.Copy)]
		string [] SubtitleVariants { get; }

		[NullAllowed, Export ("imageSet", ArgumentSemantic.Copy)]
		CPImageSet ImageSet { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; }

		[Export ("primaryAction", ArgumentSemantic.Strong)]
		CPAlertAction PrimaryAction { get; }

		[NullAllowed, Export ("secondaryAction", ArgumentSemantic.Strong)]
		CPAlertAction SecondaryAction { get; }

		[Export ("duration")]
		double Duration { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPNavigationSession {

		[Export ("pauseTripForReason:description:")]
		void PauseTrip (CPTripPauseReason reason, [NullAllowed] string description);

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("pauseTripForReason:description:turnCardColor:")]
		void PauseTrip (CPTripPauseReason reason, [NullAllowed] string description, [NullAllowed] UIColor turnCardColor);

		[Export ("finishTrip")]
		void FinishTrip ();

		[Export ("cancelTrip")]
		void CancelTrip ();

		[Export ("upcomingManeuvers", ArgumentSemantic.Copy)]
		CPManeuver [] UpcomingManeuvers { get; set; }

		[Export ("trip", ArgumentSemantic.Strong)]
		CPTrip Trip { get; }

		[Export ("updateTravelEstimates:forManeuver:")]
		void UpdateTravelEstimates (CPTravelEstimates estimates, CPManeuver maneuver);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	interface CPSearchTemplate {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPSearchTemplateDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface ICPSearchTemplateDelegate { }
	delegate void CPSearchTemplateDelegateUpdateHandler (CPListItem [] searchResults);

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPSearchTemplateDelegate {

		[Abstract]
		[Export ("searchTemplate:updatedSearchText:completionHandler:")]
		void UpdatedSearchText (CPSearchTemplate searchTemplate, string searchText, CPSearchTemplateDelegateUpdateHandler completionHandler);

		[Abstract]
		[Export ("searchTemplate:selectedResult:completionHandler:")]
		void SelectedResult (CPSearchTemplate searchTemplate, CPListItem item, Action completionHandler);

		[Export ("searchTemplateSearchButtonPressed:")]
		void SearchButtonPressed (CPSearchTemplate searchTemplate);
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPSessionConfiguration {

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (ICPSessionConfigurationDelegate @delegate);

		[Export ("limitedUserInterfaces")]
		CPLimitableUserInterface LimitedUserInterfaces { get; }

		[iOS (13, 0)]
		[Export ("contentStyle")]
		CPContentStyle ContentStyle { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPSessionConfigurationDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface ICPSessionConfigurationDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPSessionConfigurationDelegate {

#if !NET
		[Abstract]
#endif
		[Export ("sessionConfiguration:limitedUserInterfacesChanged:")]
		void LimitedUserInterfacesChanged (CPSessionConfiguration sessionConfiguration, CPLimitableUserInterface limitedUserInterfaces);

		[iOS (13, 0)]
		[Export ("sessionConfiguration:contentStyleChanged:")]
		void ContentStyleChanged (CPSessionConfiguration sessionConfiguration, CPContentStyle contentStyle);
	}

	[Abstract]
	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	interface CPTemplate : NSSecureCoding {
		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("tabTitle")]
		string TabTitle { get; set; }

		[iOS (14, 0)]
		[NullAllowed]
		[Export ("tabImage", ArgumentSemantic.Strong)]
		UIImage TabImage { get; set; }

		[iOS (14, 0)]
		[Export ("tabSystemItem", ArgumentSemantic.Assign)]
		UITabBarSystemItem TabSystemItem { get; set; }

		[iOS (14, 0)]
		[Export ("showsTabBadge")]
		bool ShowsTabBadge { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPRouteChoice : NSCopying, NSSecureCoding {

		[Export ("initWithSummaryVariants:additionalInformationVariants:selectionSummaryVariants:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string [] summaryVariants, string [] additionalInformationVariants, string [] selectionSummaryVariants);

		[Export ("summaryVariants", ArgumentSemantic.Copy)]
		string [] SummaryVariants { get; }

		[Export ("selectionSummaryVariants", ArgumentSemantic.Copy)]
		[NullAllowed]
		string [] SelectionSummaryVariants { get; }

		[Export ("additionalInformationVariants", ArgumentSemantic.Copy)]
		[NullAllowed]
		string [] AdditionalInformationVariants { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPTrip : NSSecureCoding {

		[Export ("initWithOrigin:destination:routeChoices:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKMapItem origin, MKMapItem destination, CPRouteChoice [] routeChoices);

		[Export ("origin", ArgumentSemantic.Strong)]
		MKMapItem Origin { get; }

		[Export ("destination", ArgumentSemantic.Strong)]
		MKMapItem Destination { get; }

		[Export ("routeChoices", ArgumentSemantic.Copy)]
		CPRouteChoice [] RouteChoices { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	interface CPVoiceControlState : NSSecureCoding {

		[Export ("initWithIdentifier:titleVariants:image:repeats:")]
		NativeHandle Constructor (string identifier, [NullAllowed] string [] titleVariants, [NullAllowed] UIImage image, bool repeats);

		[NullAllowed, Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("repeats")]
		bool Repeats { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPVoiceControlTemplate {

		[Export ("initWithVoiceControlStates:")]
		NativeHandle Constructor (CPVoiceControlState [] voiceControlStates);

		[Export ("voiceControlStates", ArgumentSemantic.Copy)]
		CPVoiceControlState [] VoiceControlStates { get; }

		[Export ("activateVoiceControlStateWithIdentifier:")]
		void ActivateVoiceControlState (string identifier);

		[NullAllowed, Export ("activeStateIdentifier")]
		string ActiveStateIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPImageSet : NSSecureCoding {

		[Export ("initWithLightContentImage:darkContentImage:")]
		NativeHandle Constructor (UIImage lightImage, UIImage darkImage);

		[Export ("lightContentImage")]
		UIImage LightContentImage { get; }

		[Export ("darkContentImage")]
		UIImage DarkContentImage { get; }
	}

	interface ICPTemplateApplicationSceneDelegate { }

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPTemplateApplicationSceneDelegate : UISceneDelegate {
		[Export ("templateApplicationScene:didConnectInterfaceController:toWindow:")]
		void DidConnect (CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController, CPWindow window);

		[Export ("templateApplicationScene:didDisconnectInterfaceController:fromWindow:")]
		void DidDisconnect (CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController, CPWindow window);

		[Export ("templateApplicationScene:didSelectNavigationAlert:")]
		void DidSelect (CPTemplateApplicationScene templateApplicationScene, CPNavigationAlert navigationAlert);

		[Export ("templateApplicationScene:didSelectManeuver:")]
		void DidSelect (CPTemplateApplicationScene templateApplicationScene, CPManeuver maneuver);

		[iOS (14, 0)]
		[Export ("templateApplicationScene:didConnectInterfaceController:")]
		void DidConnect (CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController);

		[iOS (14, 0)]
		[Export ("templateApplicationScene:didDisconnectInterfaceController:")]
		void DidDisconnect (CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController);

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("contentStyleDidChange:")]
		void ContentStyleDidChange (UIUserInterfaceStyle contentStyle);
	}

	[NoWatch, NoTV, NoMac, iOS (13, 0)]
	[BaseType (typeof (UIScene))]
	interface CPTemplateApplicationScene {
		[Export ("initWithSession:connectionOptions:")]
		NativeHandle Constructor (UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPTemplateApplicationSceneDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Strong)]
		NSObject WeakDelegate { get; set; }

		[Export ("interfaceController", ArgumentSemantic.Strong)]
		CPInterfaceController InterfaceController { get; }

		[Export ("carWindow", ArgumentSemantic.Strong)]
		CPWindow CarWindow { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("contentStyle")]
		UIUserInterfaceStyle ContentStyle { get; }

		[Field ("CPTemplateApplicationSceneSessionRoleApplication")]
		[Advice ("Use 'UIWindowSceneSessionRole.CarTemplateApplication' instead.")]
		NSString SessionRoleApplication { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (UIWindow))]
	interface CPWindow {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("mapButtonSafeAreaLayoutGuide")]
		UILayoutGuide MapButtonSafeAreaLayoutGuide { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("templateApplicationScene", ArgumentSemantic.Weak)]
		CPTemplateApplicationScene TemplateApplicationScene { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPTravelEstimates : NSSecureCoding {

		[Export ("initWithDistanceRemaining:timeRemaining:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSMeasurement<NSUnitLength> distance, double time);

		[Export ("distanceRemaining", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemaining { get; }

		[Export ("timeRemaining")]
		double TimeRemaining { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (NSObject))]
	interface CPTripPreviewTextConfiguration : NSSecureCoding {

		[Export ("initWithStartButtonTitle:additionalRoutesButtonTitle:overviewButtonTitle:")]
		NativeHandle Constructor ([NullAllowed] string startButtonTitle, [NullAllowed] string additionalRoutesButtonTitle, [NullAllowed] string overviewButtonTitle);

		[NullAllowed, Export ("startButtonTitle")]
		string StartButtonTitle { get; }

		[NullAllowed, Export ("additionalRoutesButtonTitle")]
		string AdditionalRoutesButtonTitle { get; }

		[NullAllowed, Export ("overviewButtonTitle")]
		string OverviewButtonTitle { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPActionSheetTemplate {

		[Export ("initWithTitle:message:actions:")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] string message, CPAlertAction [] actions);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("message")]
		string Message { get; }

		[Export ("actions", ArgumentSemantic.Strong)]
		CPAlertAction [] Actions { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPAlertTemplate {

		[Export ("initWithTitleVariants:actions:")]
		NativeHandle Constructor (string [] titleVariants, CPAlertAction [] actions);

		[Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[Export ("actions", ArgumentSemantic.Strong)]
		CPAlertAction [] Actions { get; }

		[iOS (14, 0)]
		[Static]
		[Export ("maximumActionCount")]
		nuint MaximumActionCount { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPDashboardButton : NSSecureCoding {

		[Export ("initWithTitleVariants:subtitleVariants:image:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string [] titleVariants, string [] subtitleVariants, UIImage image, [NullAllowed] Action<CPDashboardButton> handler);

		[Export ("image")]
		UIImage Image { get; }

		[Export ("titleVariants")]
		string [] TitleVariants { get; }

		[Export ("subtitleVariants")]
		string [] SubtitleVariants { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPDashboardController {

		[Export ("shortcutButtons", ArgumentSemantic.Copy)]
		CPDashboardButton [] ShortcutButtons { get; set; }
	}

	interface ICPTemplateApplicationDashboardSceneDelegate { }

	[NoWatch, NoTV, NoMac, iOS (13, 4)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPTemplateApplicationDashboardSceneDelegate : UISceneDelegate {

		[Export ("templateApplicationDashboardScene:didConnectDashboardController:toWindow:")]
		void DidConnectDashboardController (CPTemplateApplicationDashboardScene templateApplicationDashboardScene, CPDashboardController dashboardController, UIWindow window);

		[Export ("templateApplicationDashboardScene:didDisconnectDashboardController:fromWindow:")]
		void DidDisconnectDashboardController (CPTemplateApplicationDashboardScene templateApplicationDashboardScene, CPDashboardController dashboardController, UIWindow window);
	}

	[NoWatch, NoTV, NoMac, iOS (13, 4)]
	[BaseType (typeof (UIScene))]
	interface CPTemplateApplicationDashboardScene {

		[Field ("CPTemplateApplicationDashboardSceneSessionRoleApplication")]
		NSString SessionRoleApplication { get; }

		[Export ("initWithSession:connectionOptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Wrap ("WeakDelegate")]
		[NullAllowed, New]
		ICPTemplateApplicationDashboardSceneDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Strong), New]
		NSObject WeakDelegate { get; set; }

		[Export ("dashboardController", ArgumentSemantic.Strong)]
		CPDashboardController DashboardController { get; }

		[Export ("dashboardWindow", ArgumentSemantic.Strong)]
		UIWindow DashboardWindow { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPButton {
		[Export ("initWithImage:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIImage image, [NullAllowed] Action<CPButton> handler);

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Field ("CPButtonMaximumImageSize")]
		CGSize MaximumImageSize { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	interface CPContact : NSSecureCoding {
		[Export ("initWithName:image:")]
		NativeHandle Constructor (string name, UIImage image);

		[Export ("name")]
		string Name { get; set; }

		[Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("actions", ArgumentSemantic.Copy)]
		CPButton [] Actions { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NullAllowed, Export ("informativeText")]
		string InformativeText { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPButton))]
	[DisableDefaultCtor]
	interface CPContactCallButton {
		[Export ("initWithImage:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIImage image, [NullAllowed] Action<CPButton> handler);

		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPButton))]
	[DisableDefaultCtor]
	interface CPContactDirectionsButton {
		[Export ("initWithImage:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIImage image, [NullAllowed] Action<CPButton> handler);

		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPContactTemplate : CPBarButtonProviding {
		[Export ("initWithContact:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CPContact contact);

		[Export ("contact", ArgumentSemantic.Strong)]
		CPContact Contact { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPInformationItem : NSSecureCoding {
		[Export ("initWithTitle:detail:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] string detail);

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("detail")]
		string Detail { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPInformationTemplate : CPBarButtonProviding {
		[Export ("initWithTitle:layout:items:actions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, CPInformationTemplateLayout layout, CPInformationItem [] items, CPTextButton [] actions);

		[Export ("layout")]
		CPInformationTemplateLayout Layout { get; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("items", ArgumentSemantic.Copy)]
		CPInformationItem [] Items { get; set; }

		[Export ("actions", ArgumentSemantic.Copy)]
		CPTextButton [] Actions { get; set; }
	}

	delegate void CPListImageRowItemHandler (CPListImageRowItem item, nint index, [BlockCallback] Action completionBlock);

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPListImageRowItem : CPSelectableListItem {
		[Export ("initWithText:images:")]
		NativeHandle Constructor (string text, UIImage [] images);

		[Export ("gridImages", ArgumentSemantic.Strong)]
		UIImage [] GridImages { get; }

		[Export ("updateImages:")]
		void UpdateImages (UIImage [] gridImages);

		[NullAllowed, Export ("listImageRowHandler", ArgumentSemantic.Copy)]
		CPListImageRowItemHandler ListImageRowHandler { get; set; }

		[Static]
		[Export ("maximumImageSize")]
		CGSize MaximumImageSize { get; }

		[Field ("CPMaximumNumberOfGridImages")]
		nuint MaximumNumberOfGridImages { get; }

		[NullAllowed, Export ("text")]
		new string Text { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPTextButton {
		[Export ("initWithTitle:textStyle:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, CPTextButtonStyle textStyle, [NullAllowed] Action<CPTextButton> handler);

		[Export ("title")]
		string Title { get; set; }

		[Export ("textStyle", ArgumentSemantic.Assign)]
		CPTextButtonStyle TextStyle { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPBarButton))]
	interface CPMessageComposeBarButton {
		[Static]
		[Export ("new")]
		[return: Release]
		CPMessageComposeBarButton Create ();

		[Export ("initWithImage:")]
		NativeHandle Constructor (UIImage image);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	interface CPMessageListItem : CPListTemplateItem {
		[Internal]
		[Export ("initWithConversationIdentifier:text:leadingConfiguration:trailingConfiguration:detailText:trailingText:")]
		IntPtr InitWithConversationIdentifier (string conversationIdentifier, string text, CPMessageListItemLeadingConfiguration leadingConfiguration, [NullAllowed] CPMessageListItemTrailingConfiguration trailingConfiguration, [NullAllowed] string detailText, [NullAllowed] string trailingText);

		[Internal]
		[Export ("initWithFullName:phoneOrEmailAddress:leadingConfiguration:trailingConfiguration:detailText:trailingText:")]
		IntPtr InitWithFullName (string fullName, string phoneOrEmailAddress, CPMessageListItemLeadingConfiguration leadingConfiguration, [NullAllowed] CPMessageListItemTrailingConfiguration trailingConfiguration, [NullAllowed] string detailText, [NullAllowed] string trailingText);

		[NullAllowed, Export ("conversationIdentifier")]
		string ConversationIdentifier { get; set; }

		[NullAllowed, Export ("phoneOrEmailAddress")]
		string PhoneOrEmailAddress { get; set; }

		[Export ("leadingConfiguration", ArgumentSemantic.Strong)]
		CPMessageListItemLeadingConfiguration LeadingConfiguration { get; set; }

		[NullAllowed, Export ("trailingConfiguration", ArgumentSemantic.Strong)]
		CPMessageListItemTrailingConfiguration TrailingConfiguration { get; set; }

		[NullAllowed, Export ("detailText")]
		string DetailText { get; set; }

		[NullAllowed, Export ("trailingText")]
		string TrailingText { get; set; }

		[Field ("CPMaximumMessageItemImageSize")]
		CGSize MaximumMessageItemImageSize { get; }

		[NullAllowed, Export ("text")]
		new string Text { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPMessageListItemLeadingConfiguration {
		[Export ("unread")]
		bool Unread { [Bind ("isUnread")] get; }

		[Export ("leadingItem")]
		CPMessageLeadingItem LeadingItem { get; }

		[NullAllowed, Export ("leadingImage")]
		UIImage LeadingImage { get; }

		[Export ("initWithLeadingItem:leadingImage:unread:")]
		NativeHandle Constructor (CPMessageLeadingItem leadingItem, [NullAllowed] UIImage leadingImage, bool unread);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPMessageListItemTrailingConfiguration {
		[Export ("trailingItem")]
		CPMessageTrailingItem TrailingItem { get; }

		[NullAllowed, Export ("trailingImage")]
		UIImage TrailingImage { get; }

		[Export ("initWithTrailingItem:trailingImage:")]
		NativeHandle Constructor (CPMessageTrailingItem trailingItem, [NullAllowed] UIImage trailingImage);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPNowPlayingButton : NSSecureCoding {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Field ("CPNowPlayingButtonMaximumImageSize")]
		CGSize MaximumImageSize { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	[DisableDefaultCtor]
	interface CPNowPlayingImageButton {
		[Export ("initWithImage:handler:")]
		NativeHandle Constructor (UIImage image, [NullAllowed] Action<CPNowPlayingButton> handler);

		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPPointOfInterest : NSSecureCoding {
		[Export ("initWithLocation:title:subtitle:summary:detailTitle:detailSubtitle:detailSummary:pinImage:")]
		NativeHandle Constructor (MKMapItem location, string title, [NullAllowed] string subtitle, [NullAllowed] string summary, [NullAllowed] string detailTitle, [NullAllowed] string detailSubtitle, [NullAllowed] string detailSummary, [NullAllowed] UIImage pinImage);

		[iOS (16, 0)]
		[Export ("initWithLocation:title:subtitle:summary:detailTitle:detailSubtitle:detailSummary:pinImage:selectedPinImage:")]
		[DesignatedInitializer]
		NativeHandle Constructor (MKMapItem location, string title, [NullAllowed] string subtitle, [NullAllowed] string summary, [NullAllowed] string detailTitle, [NullAllowed] string detailSubtitle, [NullAllowed] string detailSummary, [NullAllowed] UIImage pinImage, [NullAllowed] UIImage selectedPinImage);

		[Export ("location", ArgumentSemantic.Strong)]
		MKMapItem Location { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NullAllowed, Export ("summary")]
		string Summary { get; set; }

		[NullAllowed, Export ("detailTitle")]
		string DetailTitle { get; set; }

		[NullAllowed, Export ("detailSubtitle")]
		string DetailSubtitle { get; set; }

		[NullAllowed, Export ("detailSummary")]
		string DetailSummary { get; set; }

		[NullAllowed, Export ("pinImage", ArgumentSemantic.Strong)]
		UIImage PinImage { get; set; }

		[NullAllowed, Export ("primaryButton", ArgumentSemantic.Strong)]
		CPTextButton PrimaryButton { get; set; }

		[NullAllowed, Export ("secondaryButton", ArgumentSemantic.Strong)]
		CPTextButton SecondaryButton { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }

		[iOS (16, 0)]
		[Static]
		[Export ("pinImageSize")]
		CGSize PinImageSize { get; }

		[iOS (16, 0)]
		[Static]
		[Export ("selectedPinImageSize")]
		CGSize SelectedPinImageSize { get; }

		[iOS (16, 0)]
		[NullAllowed, Export ("selectedPinImage", ArgumentSemantic.Strong)]
		UIImage SelectedPinImage { get; set; }
	}

	interface ICPPointOfInterestTemplateDelegate { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPPointOfInterestTemplateDelegate {
		[Abstract]
		[Export ("pointOfInterestTemplate:didChangeMapRegion:")]
		void DidChangeMapRegion (CPPointOfInterestTemplate pointOfInterestTemplate, MKCoordinateRegion region);

		[Export ("pointOfInterestTemplate:didSelectPointOfInterest:")]
		void DidSelectPointOfInterest (CPPointOfInterestTemplate pointOfInterestTemplate, CPPointOfInterest pointOfInterest);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPPointOfInterestTemplate : CPBarButtonProviding {
		[Export ("initWithTitle:pointsOfInterest:selectedIndex:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, CPPointOfInterest [] pointsOfInterest, nint selectedIndex);

		[Export ("title")]
		string Title { get; set; }

		[Export ("setPointsOfInterest:selectedIndex:")]
		void SetPointsOfInterest (CPPointOfInterest [] pointsOfInterest, nint selectedIndex);

		[Export ("pointsOfInterest")]
		CPPointOfInterest [] PointsOfInterest { get; }

		[Export ("selectedIndex")]
		nint SelectedIndex { get; set; }

		[Wrap ("WeakPointOfInterestDelegate")]
		[NullAllowed]
		ICPPointOfInterestTemplateDelegate PointOfInterestDelegate { get; set; }

		[NullAllowed, Export ("pointOfInterestDelegate", ArgumentSemantic.Weak)]
		NSObject WeakPointOfInterestDelegate { get; set; }
	}

	interface ICPTabBarTemplateDelegate { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPTabBarTemplateDelegate {
		[Abstract]
		[Export ("tabBarTemplate:didSelectTemplate:")]
		void DidSelectTemplate (CPTabBarTemplate tabBarTemplate, CPTemplate selectedTemplate);
	}


	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPTabBarTemplate {
		[Export ("initWithTemplates:")]
		NativeHandle Constructor (CPTemplate [] templates);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPTabBarTemplateDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Static]
		[Export ("maximumTabCount")]
		nuint MaximumTabCount { get; }

		[Export ("templates", ArgumentSemantic.Strong)]
		CPTemplate [] Templates { get; }

		[NullAllowed, Export ("selectedTemplate", ArgumentSemantic.Strong)]
		CPTemplate SelectedTemplate { get; }

		[Export ("updateTemplates:")]
		void UpdateTemplates (CPTemplate [] newTemplates);

		[iOS (17, 0)]
		[Export ("selectTemplate:")]
		void SelectTemplate (CPTemplate newTemplate);

		[iOS (17, 0)]
		[Export ("selectTemplateAtIndex:")]
		void SelectTemplate (nint index);
	}

	interface ICPNowPlayingTemplateObserver { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Protocol]
	interface CPNowPlayingTemplateObserver {
		[Export ("nowPlayingTemplateUpNextButtonTapped:")]
		void UpNextButtonTapped (CPNowPlayingTemplate nowPlayingTemplate);

		[Export ("nowPlayingTemplateAlbumArtistButtonTapped:")]
		void AlbumArtistButtonTapped (CPNowPlayingTemplate nowPlayingTemplate);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPNowPlayingTemplate {
		[Static]
		[Export ("sharedTemplate", ArgumentSemantic.Strong)]
		CPNowPlayingTemplate SharedTemplate { get; }

		[Export ("addObserver:")]
		void AddObserver (ICPNowPlayingTemplateObserver observer);

		[Export ("removeObserver:")]
		void RemoveObserver (ICPNowPlayingTemplateObserver observer);

		[Export ("nowPlayingButtons", ArgumentSemantic.Strong)]
		CPNowPlayingButton [] NowPlayingButtons { get; }

		[Export ("upNextButtonEnabled")]
		bool IsUpNextButtonEnabled { [Bind ("isUpNextButtonEnabled")] get; set; }

		[Export ("upNextTitle")]
		string UpNextTitle { get; set; }

		[Export ("albumArtistButtonEnabled")]
		bool IsAlbumArtistButtonEnabled { [Bind ("isAlbumArtistButtonEnabled")] get; set; }

		[Export ("updateNowPlayingButtons:")]
		void UpdateNowPlayingButtons (CPNowPlayingButton [] nowPlayingButtons);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPButton))]
	[DisableDefaultCtor]
	interface CPContactMessageButton {
		[Export ("initWithImage:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIImage image, [NullAllowed] Action<CPButton> handler);

		[Export ("initWithPhoneOrEmail:")]
		NativeHandle Constructor (string phoneOrEmail);

		[Export ("phoneOrEmail")]
		string PhoneOrEmail { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	interface CPNowPlayingShuffleButton {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	interface CPNowPlayingAddToLibraryButton {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	interface CPNowPlayingMoreButton {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	interface CPNowPlayingPlaybackRateButton {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPNowPlayingButton))]
	interface CPNowPlayingRepeatButton {
		[Export ("initWithHandler:")]
		NativeHandle Constructor ([NullAllowed] Action<CPNowPlayingButton> handler);
	}

	interface ICPListTemplateItem { }

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Protocol]
	interface CPListTemplateItem {
		[Abstract]
		[NullAllowed, Export ("text")]
		string Text { get; }

		[Abstract]
		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }

#if NET
		[Abstract]
#endif
		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	interface ICPSelectableListItem { }

	delegate void CPSelectableListItemHandler (ICPSelectableListItem item, [BlockCallback] Action completionBlock);

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[Protocol]
	interface CPSelectableListItem : CPListTemplateItem {
		[Abstract]
		[NullAllowed, Export ("handler", ArgumentSemantic.Copy)]
		CPSelectableListItemHandler Handler { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (14, 0)]
	[BaseType (typeof (CPInformationItem))]
	[DisableDefaultCtor]
	interface CPInformationRatingItem {
		[Export ("initWithRating:maximumRating:title:detail:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSNumber rating, [NullAllowed] NSNumber maximumRating, [NullAllowed] string title, [NullAllowed] string detail);

		[NullAllowed, Export ("rating")]
		NSNumber Rating { get; }

		[NullAllowed, Export ("maximumRating")]
		NSNumber MaximumRating { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPAssistantCellConfiguration : NSSecureCoding {
		[Export ("initWithPosition:visibility:assistantAction:")]
		NativeHandle Constructor (CPAssistantCellPosition position, CPAssistantCellVisibility visibility, CPAssistantCellActionType assistantAction);

		[Export ("position")]
		CPAssistantCellPosition Position { get; }

		[Export ("visibility")]
		CPAssistantCellVisibility Visibility { get; }

		[Export ("assistantAction")]
		CPAssistantCellActionType AssistantAction { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPInstrumentClusterController {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPInstrumentClusterControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("instrumentClusterWindow", ArgumentSemantic.Strong)]
		UIWindow InstrumentClusterWindow { get; }

		[Export ("speedLimitSetting")]
		CPInstrumentClusterSetting SpeedLimitSetting { get; }

		[Export ("compassSetting")]
		CPInstrumentClusterSetting CompassSetting { get; }

		[Export ("inactiveDescriptionVariants", ArgumentSemantic.Copy)]
		string [] InactiveDescriptionVariants { get; set; }

		[Export ("attributedInactiveDescriptionVariants", ArgumentSemantic.Copy)]
		NSAttributedString [] AttributedInactiveDescriptionVariants { get; set; }
	}

	interface ICPInstrumentClusterControllerDelegate { }

	[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPInstrumentClusterControllerDelegate {

		[Abstract]
		[Export ("instrumentClusterControllerDidConnectWindow:")]
		void DidConnectWindow (UIWindow instrumentClusterWindow);

		[Abstract]
		[Export ("instrumentClusterControllerDidDisconnectWindow:")]
		void DidDisconnectWindow (UIWindow instrumentClusterWindow);

		[Export ("instrumentClusterControllerDidZoomIn:")]
		void DidZoomIn (CPInstrumentClusterController instrumentClusterController);

		[Export ("instrumentClusterControllerDidZoomOut:")]
		void DidZoomOut (CPInstrumentClusterController instrumentClusterController);

		[Export ("instrumentClusterController:didChangeCompassSetting:")]
		void DidChangeCompassSetting (CPInstrumentClusterController instrumentClusterController, CPInstrumentClusterSetting compassSetting);

		[Export ("instrumentClusterController:didChangeSpeedLimitSetting:")]
		void DidChangeSpeedLimitSetting (CPInstrumentClusterController instrumentClusterController, CPInstrumentClusterSetting speedLimitSetting);
	}

	interface ICPTemplateApplicationInstrumentClusterSceneDelegate { }

	[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface CPTemplateApplicationInstrumentClusterSceneDelegate : UISceneDelegate {

		[Export ("templateApplicationInstrumentClusterScene:didConnectInstrumentClusterController:")]
		void DidConnectInstrumentClusterController (CPTemplateApplicationInstrumentClusterScene templateApplicationInstrumentClusterScene, CPInstrumentClusterController instrumentClusterController);

		[Export ("templateApplicationInstrumentClusterScene:didDisconnectInstrumentClusterController:")]
		void DidDisconnectInstrumentClusterController (CPTemplateApplicationInstrumentClusterScene templateApplicationInstrumentClusterScene, CPInstrumentClusterController instrumentClusterController);

		[Export ("contentStyleDidChange:")]
		void ContentStyleDidChange (UIUserInterfaceStyle contentStyle);
	}

	[NoWatch, NoTV, NoMac, iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (UIScene))]
	[DisableDefaultCtor]
	interface CPTemplateApplicationInstrumentClusterScene {

		[Field ("CPTemplateApplicationInstrumentClusterSceneSessionRoleApplication")]
		NSString SessionRoleApplication { get; }

		[Export ("initWithSession:connectionOptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPTemplateApplicationInstrumentClusterSceneDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Strong)]
		NSObject WeakDelegate { get; set; }

		[Export ("instrumentClusterController", ArgumentSemantic.Strong)]
		CPInstrumentClusterController InstrumentClusterController { get; }

		[Export ("contentStyle")]
		UIUserInterfaceStyle ContentStyle { get; }
	}

}
