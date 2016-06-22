//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014-2015, Xamarin Inc.
//
//
using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.CoreGraphics;
using XamCore.CoreLocation;
using XamCore.HealthKit;
using XamCore.PassKit;
using XamCore.UIKit;
using XamCore.MapKit;
using System;
using System.ComponentModel;

namespace XamCore.WatchKit {
	[iOS (8,2)]
	[BaseType (typeof (NSObject))]
	[Abstract] // <quote>To use this class, subclass it</quote> 
	// FIXME: that's not 100% obvious - OTOH beta2 crash if we call `init`
	interface WKInterfaceController {

		// `init` is now (beta 2) the designated initializer

		[Export ("awakeWithContext:")]
		void Awake ([NullAllowed] NSObject context);

		[Export ("contentFrame")]
		CGRect ContentFrame { get; }

		// The super implementation of this method does nothing.
		[Export ("willActivate")]
		void WillActivate ();

		// The super implementation of this method does nothing.
		[Export ("didDeactivate")]
		void DidDeactivate ();

#if WATCH
		[Export ("didAppear")]
		void DidAppear ();

		[Export ("willDisappear")]
		void WillDisappear ();
#endif

		[Export ("table:didSelectRowAtIndex:")]
		void DidSelectRow (WKInterfaceTable table, nint rowIndex);

		[Export ("handleActionWithIdentifier:forRemoteNotification:")]
		void HandleRemoteNotificationAction ([NullAllowed] string identifier, NSDictionary remoteNotification);

		[Export ("handleActionWithIdentifier:forLocalNotification:")]
		void HandleLocalNotificationAction ([NullAllowed] string identifier, UILocalNotification localNotification);

		[Export ("handleUserActivity:")]
		// This NSDictionary is OK, it is arbitrary and user specific
		void HandleUserActivity ([NullAllowed] NSDictionary userActivity);

		[Export ("setTitle:")]
		void SetTitle ([NullAllowed] string title);

		[ThreadSafe (false)]
		[Export ("pushControllerWithName:context:")]
		void PushController (string name, [NullAllowed] NSObject context);

		[ThreadSafe (false)]
		[Export ("popController")]
		void PopController ();

		[ThreadSafe (false)]
		[Export ("popToRootController")]
		void PopToRootController ();

		[ThreadSafe (false)]
		[Export ("becomeCurrentPage")]
		void BecomeCurrentPage ();

		[ThreadSafe (false)]
		[Export ("presentControllerWithName:context:")]
		void PresentController (string name, [NullAllowed] NSObject context);

		[ThreadSafe (false)]
		[Export ("presentControllerWithNames:contexts:")]
		void PresentController (string [] names, [NullAllowed] NSObject [] contexts);

		[ThreadSafe (false)]
		[Export ("dismissController")]
		void DismissController ();

		[Export ("dismissTextInputController")]
		void DismissTextInputController ();

		[Export ("contextForSegueWithIdentifier:")]
		NSObject GetContextForSegue (string segueIdentifier);

		[Export ("contextsForSegueWithIdentifier:")]
		NSObject [] GetContextsForSegue (string segueIdentifier);

		[Export ("contextForSegueWithIdentifier:inTable:rowIndex:")]
		NSObject GetContextForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex);

		[Export ("contextsForSegueWithIdentifier:inTable:rowIndex:")]
		NSObject [] GetContextsForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithImage:title:action:")]
		void AddMenuItem (UIImage image, string title, Selector action);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithImageNamed:title:action:")]
		void AddMenuItem (string imageName, string title, Selector action);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithItemIcon:title:action:")]
		void AddMenuItem (WKMenuItemIcon itemIcon, string title, Selector action);

		[Export ("clearAllMenuItems")]
		void ClearAllMenuItems ();

		[Export ("updateUserActivity:userInfo:webpageURL:")]
		// This NSDictionary is OK, it is arbitrary and user specific
		void UpdateUserActivity (string type, [NullAllowed] NSDictionary userInfo, [NullAllowed] NSUrl webpageURL);

		[Export ("invalidateUserActivity")]
		void InvalidateUserActivity ();

		[ThreadSafe (false)]
		[Export ("presentTextInputControllerWithSuggestions:allowedInputMode:completion:")]
		void PresentTextInputController ([NullAllowed] string [] suggestions, WKTextInputMode inputMode, Action<NSArray> completion);

		[iOS (9,0)]
		[Export ("presentTextInputControllerWithSuggestionsForLanguage:allowedInputMode:completion:")]
		void PresentTextInputController ([NullAllowed] Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode, Action<NSArray> completion);

		[NoWatch]
		[Static, Export ("openParentApplication:reply:")]
		bool OpenParentApplication (NSDictionary userInfo, [NullAllowed] Action<NSDictionary, NSError> reply);

		[Static, Export ("reloadRootControllersWithNames:contexts:")]
		void ReloadRootControllers (string [] names, [NullAllowed] NSObject [] contexts);

#if !XAMCORE_3_0
		// now exposed with the corresponding WKErrorCode enum
		[Field ("WatchKitErrorDomain")]
		NSString ErrorDomain { get; }
#endif

#if WATCH
		[Export ("dismissMediaPlayerController")]
		void DismissMediaPlayerController ();

		[Watch (2,0)]
		[Export ("presentAudioRecorderControllerWithOutputURL:preset:options:completion:")]
		void PresentAudioRecorderController (NSUrl outputUrl, WKAudioRecorderPreset preset, [NullAllowed] NSDictionary options, Action<bool, NSError> completion);

		[Watch (2,0)]
		[Export ("dismissAudioRecorderController")]
		void DismissAudioRecorderController ();

		[Export ("animateWithDuration:animations:")]
		void AnimateWithDuration (double duration, Action animations);

		[Export ("presentAlertControllerWithTitle:message:preferredStyle:actions:")]
		void PresentAlertController ([NullAllowed] string title, [NullAllowed] string message, WKAlertControllerStyle preferredStyle, WKAlertAction[] actions);

		[Export ("presentAddPassesControllerWithPasses:completion:")]
		void PresentAddPassesController (PKPass[] passes, Action completion);

		[Export ("dismissAddPassesController")]
		void DismissAddPassesController ();

		[Export ("beginGlanceUpdates")]
		void BeginGlanceUpdates ();

		[Export ("endGlanceUpdates")]
		void EndGlanceUpdates ();

		[Export ("pickerDidFocus:")]
		void PickerDidFocus (WKInterfacePicker picker);

		[Export ("pickerDidResignFocus:")]
		void PickerDidResignFocus (WKInterfacePicker picker);

		[Export ("pickerDidSettle:")]
		void PickerDidSettle (WKInterfacePicker picker);
#endif
		[NoiOS]
		[Export ("presentMediaPlayerControllerWithURL:options:completion:")]
		void PresentMediaPlayerController (NSUrl url, [NullAllowed] NSDictionary options, Action<bool, double, NSError> completion);
	}

	[iOS (8,2)]
	[BaseType (typeof (WKInterfaceController))]
	interface WKUserNotificationInterfaceController {
		[Export ("didReceiveRemoteNotification:withCompletion:")]
		void DidReceiveRemoteNotification (NSDictionary remoteNotification, Action<WKUserNotificationInterfaceType> completionHandler);

		[Export ("didReceiveLocalNotification:withCompletion:")]
		void DidReceiveLocalNotification (UILocalNotification localNotification, Action<WKUserNotificationInterfaceType> completionHandler);

#if WATCH
		[Export ("suggestionsForResponseToActionWithIdentifier:forRemoteNotification:inputLanguage:")]
		string[] GetSuggestionsForResponseToAction (string identifier, NSDictionary remoteNotification, string inputLanguage);

		[Export ("suggestionsForResponseToActionWithIdentifier:forLocalNotification:inputLanguage:")]
		string[] GetSuggestionsForResponseToAction (string identifier, UILocalNotification localNotification, string inputLanguage);
#endif
	}
	
	[iOS (8,2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	// just like we inlined UIAccessibility into UIViewm UIImage and UIBarItem instead of stuffing it all into NSObject
	interface WKInterfaceObject : UIAccessibility {
		[Export ("interfaceProperty")]
		string InterfaceProperty { get; }

		[Export ("setHidden:")]
		void SetHidden (bool hidden);

		[Export ("setAlpha:")]
		void SetAlpha (nfloat alpha);

		[Export ("setWidth:")]
		void SetWidth (nfloat width);

		[Export ("setHeight:")]
		void SetHeight (nfloat height);

		[Watch (2,1), NoiOS]
		[Export ("setSemanticContentAttribute:")]
		void SetSemanticContentAttribute (WKInterfaceSemanticContentAttribute semanticContentAttribute);

#if WATCH
		[NoiOS]
		[Export ("setHorizontalAlignment:")]
		void SetHorizontalAlignment (WKInterfaceObjectHorizontalAlignment horizontalAlignment);

		[NoiOS]
		[Export ("setVerticalAlignment:")]
		void SetVerticalAlignment (WKInterfaceObjectVerticalAlignment verticalAlignment);
#endif

		[NoiOS]
		[Export ("setRelativeWidth:withAdjustment:")]
		void SetRelativeWidth (nfloat width, nfloat adjustment);

		[NoiOS]
		[Export ("setRelativeHeight:withAdjustment:")]
		void SetRelativeHeight (nfloat height, nfloat adjustment);

		[NoiOS]
		[Export ("sizeToFitWidth")]
		void SizeToFitWidth ();

		[NoiOS]
		[Export ("sizeToFitHeight")]
		void SizeToFitHeight ();
	}

	[iOS (8,2)]
	[Category]
	[BaseType (typeof (WKInterfaceObject))]
	interface WKAccessibility {
		[Export ("setAccessibilityLabel:")]
		void SetAccessibilityLabel ([NullAllowed] string accessibilityLabel);

		[Export ("setAccessibilityHint:")]
		void SetAccessibilityHint ([NullAllowed] string accessibilityHint);

		[Export ("setAccessibilityValue:")]
		void SetAccessibilityValue ([NullAllowed] string accessibilityValue);

		[Export ("setAccessibilityImageRegions:")]
		void SetAccessibilityImageRegions (WKAccessibilityImageRegion[] accessibilityImageRegions);

		[Export ("setAccessibilityTraits:")]
		void SetAccessibilityTraits (UIAccessibilityTrait accessibilityTraits);

		[Export ("setIsAccessibilityElement:")]
		void SetIsAccessibilityElement (bool isAccessibilityElement);

		[Export ("setAccessibilityIdentifier:")]
		void SetAccessibilityIdentifier ([NullAllowed] string accessibilityIdentifier);

		[Watch (2,0)][NoiOS]
		[Notification]
		[Field ("WKAccessibilityVoiceOverStatusChanged")]
		NSString VoiceOverStatusChanged { get; }
	}

	[iOS (8,2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // does not make sense to create, it should only be used thru the singleton
	interface WKInterfaceDevice {
		[Export ("screenBounds")]
		CGRect ScreenBounds { get; }

		[Export ("screenScale")]
		nfloat ScreenScale { get; }

		[NoWatch]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("cachedImages")]
		NSDictionary WeakCachedImages { get; }

		[Internal]
		[Export ("preferredContentSizeCategory")]
		NSString _PreferredContentSizeCategory { get; }

		[Static, Export ("currentDevice")]
		WKInterfaceDevice CurrentDevice { get; }

		[NoWatch]
		[Export ("addCachedImage:name:")]
		bool AddCachedImage (UIImage image, string name);

		[NoWatch]
		[Export ("addCachedImageWithData:name:")]
		bool AddCachedImage (NSData imageData, string name);

		[NoWatch]
		[Export ("removeCachedImageWithName:")]
		void RemoveCachedImage (string name);

		[NoWatch]
		[Export ("removeAllCachedImages")]
		void RemoveAllCachedImages ();

		[iOS (9,0), Watch (2,0)]
		[Export ("systemVersion")]
		string SystemVersion { get; }

		[iOS (9,0), Watch (2,0)]
		[Export ("name")]
		string Name { get; }

		[iOS (9,0), Watch (2,0)]
		[Export ("model")]
		string Model { get; }

		[iOS (9,0), Watch (2,0)]
		[Export ("localizedModel")]
		string LocalizedModel { get; }

		[iOS (9,0)][Watch (2,0)]
		[Export ("systemName")]
		string SystemName { get; }

		[Watch (2,0), NoiOS]
		[Export ("playHaptic:")]
		void PlayHaptic (WKHapticType type);

		[Watch (2,1), NoiOS]
		[Export ("layoutDirection")]
		WKInterfaceLayoutDirection LayoutDirection { get; }

		[Watch (2,1), NoiOS]
		[Static]
		[Export ("interfaceLayoutDirectionForSemanticContentAttribute:")]
		WKInterfaceLayoutDirection GetInterfaceLayoutDirection (WKInterfaceSemanticContentAttribute semanticContentAttribute);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceButton {
		[Export ("setTitle:")]
		void SetTitle ([NullAllowed] string title);

		[Export ("setAttributedTitle:")]
		void SetTitle ([NullAllowed] NSAttributedString attributedTitle);

		[Export ("setBackgroundColor:")]
		void SetBackgroundColor ([NullAllowed] UIColor color);

		[Export ("setBackgroundImage:")]
		void SetBackgroundImage ([NullAllowed] UIImage image);

		[Export ("setBackgroundImageData:")]
		void SetBackgroundImage ([NullAllowed] NSData imageData);

		[Export ("setBackgroundImageNamed:")]
		void SetBackgroundImage ([NullAllowed] string imageName);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);
	}

	[iOS (8,2)]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceGroup : WKImageAnimatable {
		[Export ("setBackgroundColor:")]
		void SetBackgroundColor ([NullAllowed] UIColor color);

		[Export ("setBackgroundImage:")]
		void SetBackgroundImage ([NullAllowed] UIImage image);

		[Export ("setBackgroundImageData:")]
		void SetBackgroundImage ([NullAllowed] NSData imageData);

		[Export ("setBackgroundImageNamed:")]
		void SetBackgroundImage ([NullAllowed] string imageName);

		[Export ("setCornerRadius:")]
		void SetCornerRadius (nfloat cornerRadius);

		[NoiOS]
		[Export ("setContentInset:")]
		void SetContentInset (UIEdgeInsets contentInset);
	}

	[iOS (8,2)]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceImage : WKImageAnimatable {
		[Export ("setImage:")]
		void SetImage ([NullAllowed] UIImage image);

		[Export ("setImageData:")]
		void SetImage ([NullAllowed] NSData imageData);

		[Export ("setImageNamed:")]
		void SetImage ([NullAllowed] string imageName);

		[Export ("setTintColor:")]
		void SetTintColor ([NullAllowed] UIColor color);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceLabel {

		[Export ("setText:")]
		void SetText ([NullAllowed] string text);

		[Export ("setTextColor:")]
		void SetTextColor ([NullAllowed] UIColor color);

		[Export ("setAttributedText:")]
		void SetText ([NullAllowed] NSAttributedString attributedText);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceDate {
		[Export ("setTextColor:")]
		void SetTextColor ([NullAllowed] UIColor color);

		[Export ("setTimeZone:")]
		void SetTimeZone ([NullAllowed] NSTimeZone timeZone);

		[Export ("setCalendar:")]
		void SetCalendar ([NullAllowed] NSCalendar calendar);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceTimer {

		[Export ("setTextColor:")]
		void SetTextColor ([NullAllowed] UIColor color);

		[Export ("setDate:")]
		void SetDate (NSDate date);

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceTable {
		[Export ("numberOfRows")]
		nint NumberOfRows { get; }

		[Export ("setRowTypes:")]
		void SetRowTypes (string [] rowTypes);

		[Export ("setNumberOfRows:withRowType:")]
		void SetNumberOfRows (nint numberOfRows, string rowType);

		[Export ("rowControllerAtIndex:")]
		NSObject GetRowController (nint index);

		[Export ("insertRowsAtIndexes:withRowType:")]
		void InsertRows (NSIndexSet rowIndexes, string rowType);

		[Export ("removeRowsAtIndexes:")]
		void RemoveRows (NSIndexSet rowIndexes);

		[Export ("scrollToRowAtIndex:")]
		void ScrollToRow (nint index);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceMap {

		[Export ("addAnnotation:withImage:centerOffset:")]
		void AddAnnotation (CLLocationCoordinate2D location, [NullAllowed] UIImage image, CGPoint offset);

		[Export ("addAnnotation:withImageNamed:centerOffset:")]
		void AddAnnotation (CLLocationCoordinate2D location, [NullAllowed] string name, CGPoint offset);

		[Export ("addAnnotation:withPinColor:")]
		void AddAnnotation (CLLocationCoordinate2D location, WKInterfaceMapPinColor pinColor);

		[Export ("setRegion:")]
		void SetRegion (MKCoordinateRegion coordinateRegion);

		[Export ("setVisibleMapRect:")]
		void SetVisible (MKMapRect mapRect);

		[Export ("removeAllAnnotations")]
		void RemoveAllAnnotations ();
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceSeparator {
		[Export ("setColor:")]
		void SetColor ([NullAllowed] UIColor color);
	}
	
	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceSlider {
		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);

		[Export ("setValue:")]
		void SetValue (float value);

		[Export ("setColor:")]
		void SetColor ([NullAllowed] UIColor color);

		[Export ("setNumberOfSteps:")]
		void SetNumberOfSteps (nint numberOfSteps);
	}

	[iOS (8,2)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceSwitch {

		[Export ("setColor:")]
		void SetColor ([NullAllowed] UIColor color);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);

		[Export ("setOn:")]
		void SetOn (bool on);

		[Export ("setTitle:")]
		void SetTitle ([NullAllowed] string title);

		[Export ("setAttributedTitle:")]
		void SetTitle ([NullAllowed] NSAttributedString attributedTitle);
	}

	[iOS (8,2)]
	[BaseType (typeof (NSObject))]
	interface WKAccessibilityImageRegion {

		[Export ("frame")]
		CGRect Frame { get; set; }

		[NullAllowed]
		[Export ("label")]
		string Label { get; set; }
	}

	interface IWKImageAnimatable {}

	[iOS (9,0)]
	[Protocol]
	interface WKImageAnimatable {
		[iOS (8,2)]
		[Abstract]
		[Export ("startAnimating")]
		void StartAnimating ();

		[iOS (8,2)]
		[Abstract]
		[Export ("startAnimatingWithImagesInRange:duration:repeatCount:")]
		void StartAnimating (NSRange imageRange, double duration, nint repeatCount);

		[iOS (8,2)]
		[Abstract]
		[Export ("stopAnimating")]
		void StopAnimating ();
	}

#if WATCH
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKImage : NSCopying, NSSecureCoding {
		[Static]
		[Export ("imageWithImage:")]
		WKImage CreateFromImage (UIImage image);

		[Static]
		[Export ("imageWithImageData:")]
		WKImage CreateFromData (NSData imageData);

		[Static]
		[Export ("imageWithImageName:")]
		WKImage CreateFromName (string imageName);

		[NullAllowed, Export ("image")]
		UIImage Image { get; }

		[NullAllowed, Export ("imageData")]
		NSData ImageData { get; }

		[NullAllowed, Export ("imageName")]
		string ImageName { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKAlertAction {
		[Static]
		[Export ("actionWithTitle:style:handler:")]
		WKAlertAction Create (string title, WKAlertActionStyle style, Action handler);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKAudioFileAsset {
		[Static]
		[Export ("assetWithURL:")]
		WKAudioFileAsset Create (NSUrl url);

		[Static]
		[Export ("assetWithURL:title:albumTitle:artist:")]
		WKAudioFileAsset Create (NSUrl url, [NullAllowed] string title, [NullAllowed] string albumTitle, [NullAllowed] string artist);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("duration")]
		double Duration { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("albumTitle")]
		string AlbumTitle { get; }

		[NullAllowed, Export ("artist")]
		string Artist { get; }
	}

	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface WKAudioFilePlayer {
		[Static]
		[Export ("playerWithPlayerItem:")]
		WKAudioFilePlayer Create (WKAudioFilePlayerItem item);

		[Export ("play")]
		void Play ();

		[Export ("pause")]
		void Pause ();

		[Export ("replaceCurrentItemWithPlayerItem:")]
		void ReplaceCurrentItem ([NullAllowed] WKAudioFilePlayerItem item);

		[NullAllowed, Export ("currentItem")]
		WKAudioFilePlayerItem CurrentItem { get; }

		[Export ("status")]
		WKAudioFilePlayerStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[Export ("rate")]
		float Rate { get; set; }

		[Export ("currentTime")]
		double CurrentTime { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKAudioFilePlayerItem {
		[Static]
		[Export ("playerItemWithAsset:")]
		WKAudioFilePlayerItem Create (WKAudioFileAsset asset);

		[Export ("asset")]
		WKAudioFileAsset Asset { get; }

		[Export ("status")]
		WKAudioFilePlayerItemStatus Status { get; }

		[NullAllowed, Export ("error")]
		NSError Error { get; }

		[Export ("currentTime")]
		double CurrentTime { get; }

		[Watch (2,0), NoiOS]
		[Notification]
		[Field ("WKAudioFilePlayerItemTimeJumpedNotification")]
		NSString TimeJumpedNotification { get; }

		[Watch (2,0), NoiOS]
		[Notification]
		[Field ("WKAudioFilePlayerItemDidPlayToEndTimeNotification")]
		NSString DidPlayToEndTimeNotification { get; }

		[Watch (2,0), NoiOS]
		[Notification]
		[Field ("WKAudioFilePlayerItemFailedToPlayToEndTimeNotification")]
		NSString FailedToPlayToEndTimeNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	interface WKExtension {
		[Static]
		[Export ("sharedExtension")]
		WKExtension SharedExtension { get; }

		[Export ("openSystemURL:")]
		void OpenSystemUrl (NSUrl url);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		WKExtensionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("rootInterfaceController")]
		WKInterfaceController RootInterfaceController { get; }
	}

	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface WKExtensionDelegate {
		[Export ("applicationDidFinishLaunching")]
		void ApplicationDidFinishLaunching ();

		[Export ("applicationDidBecomeActive")]
		void ApplicationDidBecomeActive ();

		[Export ("applicationWillResignActive")]
		void ApplicationWillResignActive ();

		[Export ("handleActionWithIdentifier:forRemoteNotification:")]
		void HandleAction ([NullAllowed] string identifier, NSDictionary remoteNotification);

		[Export ("handleActionWithIdentifier:forLocalNotification:")]
		void HandleAction ([NullAllowed] string identifier, UILocalNotification localNotification);

		[Export ("handleActionWithIdentifier:forRemoteNotification:withResponseInfo:")]
		void HandleAction ([NullAllowed] string identifier, NSDictionary remoteNotification, NSDictionary responseInfo);

		[Export ("handleActionWithIdentifier:forLocalNotification:withResponseInfo:")]
		void HandleAction ([NullAllowed] string identifier, UILocalNotification localNotification, NSDictionary responseInfo);

		[Export ("handleUserActivity:")]
		void HandleUserActivity ([NullAllowed] NSDictionary userInfo);

		[Export ("didReceiveRemoteNotification:")]
		void DidReceiveRemoteNotification (NSDictionary userInfo);

		[Export ("didReceiveLocalNotification:")]
		void DidReceiveLocalNotification (UILocalNotification notification);
	}

	[Watch (2,2), NoiOS]
	[BaseType (typeof(WKInterfaceObject))]
	[DisableDefaultCtor] // The super class' init method is unavailable.
	interface WKInterfaceActivityRing
	{
		[Export ("setActivitySummary:animated:")]
		void SetActivitySummary ([NullAllowed] HKActivitySummary activitySummary, bool animated);
	}

	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // The super class' init method is unavailable.
	interface WKInterfaceMovie {
		[Export ("setMovieURL:")]
		void SetMovieUrl (NSUrl url);

		[Export ("setVideoGravity:")]
		void SetVideoGravity (WKVideoGravity videoGravity);

		[Export ("setLoops:")]
		void SetLoops (bool loops);

		[Export ("setPosterImage:")]
		void SetPosterImage ([NullAllowed] WKImage posterImage);
	}

	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // The super class' init method is unavailable.
	interface WKInterfacePicker {
		[Export ("focus")]
		void Focus ();

		[Export ("resignFocus")]
		void ResignFocus ();

		[Export ("setSelectedItemIndex:")]
		void SetSelectedItem (nint itemIndex);

		[Export ("setItems:")]
		void SetItems ([NullAllowed] WKPickerItem[] items);

		[Export ("setCoordinatedAnimations:")]
		void SetCoordinatedAnimations ([NullAllowed] IWKImageAnimatable [] coordinatedAnimations);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);
	}

	[BaseType (typeof (NSObject))]
	interface WKPickerItem : NSSecureCoding {
		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("caption")]
		string Caption { get; set; }

		[NullAllowed, Export ("accessoryImage", ArgumentSemantic.Copy)]
		WKImage AccessoryImage { get; set; }

		[NullAllowed, Export ("contentImage", ArgumentSemantic.Copy)]
		WKImage ContentImage { get; set; }
	}

	[BaseType (typeof (WKAudioFilePlayer))]
	[DisableDefaultCtor]
	interface WKAudioFileQueuePlayer {
		[Static]
		[Export ("queuePlayerWithItems:")]
		WKAudioFileQueuePlayer FromItems (WKAudioFilePlayerItem[] items);

		[Export ("advanceToNextItem")]
		void AdvanceToNextItem ();

		[Export ("appendItem:")]
		void AppendItem (WKAudioFilePlayerItem item);

		[Export ("removeItem:")]
		void RemoveItem (WKAudioFilePlayerItem item);

		[Export ("removeAllItems")]
		void RemoveAllItems ();

		[Export ("items")]
		WKAudioFilePlayerItem[] Items { get; }
	}
#endif

	// to be made [Internal] once #34656 is fixed
	[Static]
	[Watch (2,0)][NoiOS]
	interface WKMediaPlayerControllerOptionsKeys {
		[Field ("WKMediaPlayerControllerOptionsAutoplayKey")]
		NSString AutoplayKey { get; }

		[Field ("WKMediaPlayerControllerOptionsStartTimeKey")]
		NSString StartTimeKey { get; }

		[Field ("WKMediaPlayerControllerOptionsVideoGravityKey")]
		NSString VideoGravityKey { get; }

		[Field ("WKMediaPlayerControllerOptionsLoopsKey")]
		NSString LoopsKey { get; }
	}

	// to be made [Internal] once #34656 is fixed
	[Static]
	[Watch (2,0)][NoiOS]
	interface WKAudioRecorderControllerOptionsKey {
		[Field ("WKAudioRecorderControllerOptionsActionTitleKey")]
		NSString ActionTitleKey { get; }

		[Field ("WKAudioRecorderControllerOptionsAlwaysShowActionTitleKey")]
		NSString AlwaysShowActionTitleKey { get; }

		[Field ("WKAudioRecorderControllerOptionsAutorecordKey")]
		NSString AutorecordKey { get; }

		[Field ("WKAudioRecorderControllerOptionsMaximumDurationKey")]
		NSString MaximumDurationKey { get; }
	}
}
