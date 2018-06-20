//
// CarPlay bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Microsoft Corporation. All rights reserved.
//

#if XAMCORE_2_0

using System;
using Foundation;
using ObjCRuntime;
using UIKit;
using CoreGraphics;
using MapKit;

namespace CarPlay {

	// Just to please the generator that at this point does not know the hierarchy
	interface NSUnitLength : NSUnit { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPAlertStyle : ulong {
		ActionSheet = 0,
		FullScreen,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPAlertActionStyle : ulong {
		Default = 0,
		Cancel,
		Destructive,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPBarButtonType : ulong {
		Text,
		Image,
	}

	[Flags, NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPPanDirection : long {
		None = 0,
		Left = 1L << 0,
		Right = 1L << 1,
		Up = 1L << 2,
		Down = 1L << 3,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPNavigationAlertDismissalContext : ulong {
		Timeout = 0,
		UserDismissed,
		SystemDismissed,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPTripPauseReason : ulong {
		Arrived = 1,
		Loading = 2,
		Locating = 3,
		Rerouting = 4,
		ProceedToRoute = 5,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Flags]
	[Native]
	enum CPLimitableUserInterface : ulong {
		Keyboard = 1uL << 0,
		Lists = 1uL << 1,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Flags]
	[Native]
	enum CPManeuverDisplayStyle : long {
		Default,
		LeadingSymbol,
		TrailingSymbol,
		SymbolOnly,
		InstructionOnly,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPTimeRemainingColor : ulong {
		Default = 0,
		Green,
		Orange,
		Red,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Native]
	enum CPTripEstimateStyle : ulong {
		Light = 0,
		Dark,
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPAlert : NSSecureCoding {

		[Export ("initWithTitleVariants:message:style:actions:")]
		IntPtr Constructor (string [] titleVariants, [NullAllowed] string message, CPAlertStyle style, CPAlertAction [] actions);

		[Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[NullAllowed, Export ("message")]
		string Message { get; }

		[Export ("style")]
		CPAlertStyle Style { get; }

		[Export ("actions", ArgumentSemantic.Strong)]
		CPAlertAction [] Actions { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPAlertAction : NSSecureCoding {

		[Export ("initWithTitle:style:handler:")]
		IntPtr Constructor (string title, CPAlertActionStyle style, Action<CPAlertAction> handler);

		[Export ("title")]
		string Title { get; }

		[Export ("style", ArgumentSemantic.Assign)]
		CPAlertActionStyle Style { get; }

		[Export ("handler", ArgumentSemantic.Copy)]
		Action<CPAlertAction> Handler { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPBarButton : NSSecureCoding {

		[Export ("initWithType:handler:")]
		[DesignatedInitializer]
		IntPtr Constructor (CPBarButtonType type, [NullAllowed] Action<CPBarButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("buttonType", ArgumentSemantic.Assign)]
		CPBarButtonType ButtonType { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }
	}

	interface ICPBarButtonProviding { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol]
	interface CPBarButtonProviding {

		[Abstract]
		[Export ("leadingNavigationBarButtons", ArgumentSemantic.Strong)]
		CPBarButton [] LeadingNavigationBarButtons { get; set; }

		[Abstract]
		[Export ("trailingNavigationBarButtons", ArgumentSemantic.Strong)]
		CPBarButton [] TrailingNavigationBarButtons { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPGridButton : NSSecureCoding {

		[Export ("initWithTitleVariants:image:handler:")]
		[DesignatedInitializer]
		IntPtr Constructor (string [] titleVariants, UIImage image, [NullAllowed] Action<CPGridButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("image")]
		UIImage Image { get; }

		[Export ("titleVariants")]
		string [] TitleVariants { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPGridTemplate : CPBarButtonProviding{

		[Export ("initWithTitle:gridButtons:")]
		IntPtr Constructor ([NullAllowed] string title, CPGridButton [] gridButtons);

		[Export ("gridButtons")]
		CPGridButton [] GridButtons { get; }

		[Export ("title")]
		string Title { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPInterfaceController {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPInterfaceControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("setRootTemplate:animated:")]
		void SetRootTemplate (CPTemplate rootTemplate, bool animated);

		[Export ("pushTemplate:animated:")]
		void PushTemplate (CPTemplate templateToPush, bool animated);

		[Export ("popTemplateAnimated:")]
		void PopTemplate (bool animated);

		[Export ("popToRootTemplateAnimated:")]
		void PopToRootTemplate (bool animated);

		[Export ("popToTemplate:animated:")]
		void PopToTemplate (CPTemplate targetTemplate, bool animated);

		[Export ("rootTemplate")]
		CPTemplate RootTemplate { get; }

		[NullAllowed, Export ("topTemplate", ArgumentSemantic.Strong)]
		CPTemplate TopTemplate { get; }

		[Export ("templates", ArgumentSemantic.Strong)]
		CPTemplate [] Templates { get; }

		[Export ("presentAlert:")]
		void PresentAlert (CPAlert alert);

		[Export ("dismissAlertAnimated:")]
		void DismissAlert (bool animated);
	}

	interface ICPInterfaceControllerDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model (AutoGeneratedName = true)]
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

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject))]
	interface CPApplicationDelegate : UIApplicationDelegate {

		[Abstract]
		[Export ("application:didConnectCarInterfaceController:toWindow:")]
		void DidConnectCarInterfaceController (UIApplication application, CPInterfaceController interfaceController, CPMapContentWindow window);

		[Abstract]
		[Export ("application:didDisconnectCarInterfaceController:fromWindow:")]
		void DidDisconnectCarInterfaceController (UIApplication application, CPInterfaceController interfaceController, CPMapContentWindow window);

		[Export ("application:didSelectNavigationAlert:")]
		void DidSelectNavigationAlert (UIApplication application, CPNavigationAlert navigationAlert);

		[Export ("application:didSelectManeuver:")]
		void DidSelectManeuver (UIApplication application, CPManeuver maneuver);
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPListItem : NSSecureCoding {

		[Field ("CPMaximumListItemImageSize")]
		CGSize MaximumListItemImageSize { get; }

		[Export ("initWithText:detailText:image:showsDisclosureIndicator:")]
		IntPtr Constructor ([NullAllowed] string text, [NullAllowed] string detailText, [NullAllowed] UIImage image, bool showsDisclosureIndicator);

		[Export ("initWithText:detailText:image:")]
		IntPtr Constructor ([NullAllowed] string text, [NullAllowed] string detailText, [NullAllowed] UIImage image);

		[Export ("initWithText:detailText:")]
		IntPtr Constructor ([NullAllowed] string text, [NullAllowed] string detailText);

		[NullAllowed, Export ("text")]
		string Text { get; }

		[NullAllowed, Export ("detailText")]
		string DetailText { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; }

		[Export ("showsDisclosureIndicator")]
		bool ShowsDisclosureIndicator { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPListSection : NSSecureCoding {

		[Export ("initWithItems:header:sectionIndexTitle:")]
		IntPtr Constructor (CPListItem [] items, [NullAllowed] string header, [NullAllowed] string sectionIndexTitle);

		[Export ("initWithItems:")]
		IntPtr Constructor (CPListItem [] items);

		[NullAllowed, Export ("header")]
		string Header { get; }

		[NullAllowed, Export ("sectionIndexTitle")]
		string SectionIndexTitle { get; }

		[Export ("items", ArgumentSemantic.Copy)]
		CPListItem [] Items { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPListTemplate : CPBarButtonProviding {

		[Export ("initWithSections:")]
		IntPtr Constructor (CPListSection [] sections);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPListTemplateDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("sections", ArgumentSemantic.Copy)]
		CPListSection [] Sections { get; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[Export ("updateSections:")]
		void UpdateSections (CPListSection [] sections);
	}

	interface ICPListTemplateDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model (AutoGeneratedName = true)]
	[BaseType (typeof (NSObject))]
	interface CPListTemplateDelegate {

		[Abstract]
		[Export ("listTemplate:didSelectListItem:completionHandler:")]
		void DidSelectListItem (CPListTemplate listTemplate, CPListItem item, Action completionHandler);
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	interface CPManeuver : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("symbolSet", ArgumentSemantic.Strong)]
		CPImageSet SymbolSet { get; set; }

		[Export ("instructionVariants", ArgumentSemantic.Copy)]
		string [] InstructionVariants { get; set; }

		[NullAllowed, Export ("initialTravelEstimates", ArgumentSemantic.Strong)]
		CPTravelEstimates InitialTravelEstimates { get; set; }

		[Export ("attributedInstructionVariants", ArgumentSemantic.Copy)]
		NSAttributedString [] AttributedInstructionVariants { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPMapButton : NSSecureCoding {

		[Export ("initWithHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] Action<CPMapButton> handler);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("focusedImage", ArgumentSemantic.Strong)]
		UIImage FocusedImage { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPMapTemplate : CPBarButtonProviding {

		[Export ("initWithConfiguration:")]
		IntPtr Constructor ([NullAllowed] CPMapTemplateConfiguration configuration);

		[Export ("configuration")]
		CPMapTemplateConfiguration Configuration { get; }

		[Export ("mapButtons", ArgumentSemantic.Strong)]
		CPMapButton [] MapButtons { get; set; }

		[Export ("showTripPreviews:textConfiguration:")]
		void ShowTripPreviews (CPTrip [] tripPreviews, [NullAllowed] CPTripPreviewTextConfiguration textConfiguration);

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
	}

	interface ICPMapTemplateDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model (AutoGeneratedName = true)]
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

		[Export ("mapTemplate:didUpdatePanGestureWithDelta:velocity:")]
		void DidUpdatePanGesture (CPMapTemplate mapTemplate, CGPoint delta, CGPoint velocity);

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

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPNavigationAlert : NSSecureCoding {

		[Export ("initWithTitleVariants:subtitleVariants:imageSet:primaryAction:secondaryAction:duration:")]
		IntPtr Constructor (string [] titleVariants, string [] subtitleVariants, [NullAllowed] CPImageSet imageSet, CPAlertAction primaryAction, [NullAllowed] CPAlertAction secondaryAction, double duration);

		[Export ("updateTitleVariants:subtitleVariants:")]
		void UpdateTitleVariants (string [] newTitleVariants, string [] newSubtitleVariants);

		[Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[Export ("subtitleVariants", ArgumentSemantic.Copy)]
		string [] SubtitleVariants { get; }

		[NullAllowed, Export ("imageSet", ArgumentSemantic.Copy)]
		CPImageSet ImageSet { get; }

		[Export ("primaryAction", ArgumentSemantic.Strong)]
		CPAlertAction PrimaryAction { get; }

		[NullAllowed, Export ("secondaryAction", ArgumentSemantic.Strong)]
		CPAlertAction SecondaryAction { get; }

		[Export ("duration")]
		double Duration { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPNavigationSession {

		[Export ("pauseTripForReason:description:")]
		void PauseTrip (CPTripPauseReason reason, [NullAllowed] string description);

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

	[NoWatch, NoTV, NoMac, iOS (12,0)]
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

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model (AutoGeneratedName = true)]
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

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPSessionConfiguration {

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		IntPtr Constructor (ICPSessionConfigurationDelegate @delegate);

		[Export ("limitedUserInterfaces")]
		CPLimitableUserInterface LimitedUserInterfaces { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICPSessionConfigurationDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	interface ICPSessionConfigurationDelegate { }

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[Protocol, Model ( AutoGeneratedName = true)]
	[BaseType (typeof (NSObject))]
	interface CPSessionConfigurationDelegate {

		[Abstract]
		[Export ("sessionConfiguration:limitedUserInterfacesChanged:")]
		void LimitedUserInterfacesChanged (CPSessionConfiguration sessionConfiguration, CPLimitableUserInterface limitedUserInterfaces);
	}

	[Abstract]
	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	interface CPTemplate : NSSecureCoding {

	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPRouteChoice : NSCopying, NSSecureCoding {

		[Export ("initWithSummaryVariants:additionalInformationVariants:selectionSummaryVariants:")]
		[DesignatedInitializer]
		IntPtr Constructor (string [] summaryVariants, string [] additionalInformationVariants, string [] selectionSummaryVariants);

		[Export ("summaryVariants", ArgumentSemantic.Copy)]
		string [] SummaryVariants { get; }

		[Export ("selectionSummaryVariants", ArgumentSemantic.Copy)]
		string [] SelectionSummaryVariants { get; }

		[Export ("additionalInformationVariants", ArgumentSemantic.Copy)]
		string [] AdditionalInformationVariants { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPTrip : NSSecureCoding {

		[Export ("initWithOrigin:destination:routeChoices:")]
		[DesignatedInitializer]
		IntPtr Constructor (MKMapItem origin, MKMapItem destination, CPRouteChoice [] routeChoices);

		[Export ("origin", ArgumentSemantic.Strong)]
		MKMapItem Origin { get; }

		[Export ("destination", ArgumentSemantic.Strong)]
		MKMapItem Destination { get; }

		[Export ("routeChoices", ArgumentSemantic.Copy)]
		CPRouteChoice [] RouteChoices { get; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	interface CPVoiceControlState : NSSecureCoding {

		[Export ("initWithIdentifier:titleVariants:image:repeats:")]
		IntPtr Constructor (string identifier, [NullAllowed] string [] titleVariants, [NullAllowed] UIImage image, bool repeats);

		[NullAllowed, Export ("titleVariants", ArgumentSemantic.Copy)]
		string [] TitleVariants { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("repeats")]
		bool Repeats { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (CPTemplate))]
	[DisableDefaultCtor]
	interface CPVoiceControlTemplate {

		[Export ("initWithVoiceControlStates:")]
		IntPtr Constructor (CPVoiceControlState [] voiceControlStates);

		[NullAllowed, Export ("voiceControlStates", ArgumentSemantic.Copy)]
		CPVoiceControlState [] VoiceControlStates { get; }

		[Export ("activateVoiceControlStateWithIdentifier:")]
		void ActivateVoiceControlState (string identifier);

		[NullAllowed, Export ("activeStateIdentifier")]
		string ActiveStateIdentifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPImageSet : NSSecureCoding {

		[Export ("initWithLightContentImage:darkContentImage:")]
		IntPtr Constructor (UIImage lightImage, UIImage darkImage);

		[Export ("lightContentImage")]
		UIImage LightContentImage { get; }

		[Export ("darkContentImage")]
		UIImage DarkContentImage { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (UIWindow))]
	interface CPMapContentWindow {

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("mapButtonSafeAreaLayoutGuide")]
		UILayoutGuide MapButtonSafeAreaLayoutGuide { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPMapTemplateConfiguration : NSSecureCoding {

		[Export ("initWithGuidanceBackgroundColor:tripEstimateStyle:")]
		IntPtr Constructor (UIColor guidanceBackgroundColor, CPTripEstimateStyle tripEstimateStyle);

		[Export ("guidanceBackgroundColor")]
		UIColor GuidanceBackgroundColor { get; }

		[Export ("tripEstimateStyle")]
		CPTripEstimateStyle TripEstimateStyle { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CPTravelEstimates : NSSecureCoding {

		[Export ("initWithDistanceRemaining:timeRemaining:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSMeasurement<NSUnitLength> distance, double time);

		[Export ("distanceRemaining", ArgumentSemantic.Copy)]
		NSMeasurement<NSUnitLength> DistanceRemaining { get; }

		[Export ("timeRemaining")]
		double TimeRemaining { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (12,0)]
	[BaseType (typeof (NSObject))]
	interface CPTripPreviewTextConfiguration : NSSecureCoding {

		[Export ("initWithStartButtonTitle:additionalRoutesButtonTitle:overviewButtonTitle:")]
		IntPtr Constructor ([NullAllowed] string startButtonTitle, [NullAllowed] string additionalRoutesButtonTitle, [NullAllowed] string overviewButtonTitle);

		[NullAllowed, Export ("startButtonTitle")]
		string StartButtonTitle { get; }

		[NullAllowed, Export ("additionalRoutesButtonTitle")]
		string AdditionalRoutesButtonTitle { get; }

		[NullAllowed, Export ("overviewButtonTitle")]
		string OverviewButtonTitle { get; }
	}
}

#endif // XAMCORE_2_0
