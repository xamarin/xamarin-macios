//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2014-2016, Xamarin Inc.
// Copyright 2019 Microsoft Corporation
//
//
using ObjCRuntime;
using Foundation;
using CloudKit;
using CoreGraphics;
using CoreLocation;
using HealthKit;
using HomeKit;
using Intents;
using PassKit;
using SpriteKit;
using SceneKit;
using UIKit;
using MapKit;
using UserNotifications;
using System;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace WatchKit {
	[Unavailable (PlatformName.iOS)]
	[BaseType (typeof (NSObject))]
	[Abstract] // <quote>To use this class, subclass it</quote> 
	[DisableDefaultCtor] // DesignatedInitializer below
	interface WKInterfaceController {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

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

		[Export ("didAppear")]
		void DidAppear ();

		[Export ("willDisappear")]
		void WillDisappear ();

		[Export ("table:didSelectRowAtIndex:")]
		void DidSelectRow (WKInterfaceTable table, nint rowIndex);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forRemoteNotification:")]
		void HandleRemoteNotificationAction ([NullAllowed] string identifier, NSDictionary remoteNotification);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forLocalNotification:")]
		void HandleLocalNotificationAction ([NullAllowed] string identifier, UILocalNotification localNotification);

		[NoWatch]
		[Export ("handleActionWithIdentifier:forNotification:")]
		void HandleAction ([NullAllowed] string identifier, UNNotification notification);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'WKExtensionDelegate.HandleUserActivity' instead.")]
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

		[return: NullAllowed]
		[Export ("contextForSegueWithIdentifier:")]
		NSObject GetContextForSegue (string segueIdentifier);

		[return: NullAllowed]
		[Export ("contextsForSegueWithIdentifier:")]
		NSObject [] GetContextsForSegue (string segueIdentifier);

		[return: NullAllowed]
		[Export ("contextForSegueWithIdentifier:inTable:rowIndex:")]
		NSObject GetContextForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex);

		[return: NullAllowed]
		[Export ("contextsForSegueWithIdentifier:inTable:rowIndex:")]
		NSObject [] GetContextsForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex);

		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithImage:title:action:")]
		void AddMenuItem (UIImage image, string title, Selector action);

		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithImageNamed:title:action:")]
		void AddMenuItem (string imageName, string title, Selector action);

		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("addMenuItemWithItemIcon:title:action:")]
		void AddMenuItem (WKMenuItemIcon itemIcon, string title, Selector action);

		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[Export ("clearAllMenuItems")]
		void ClearAllMenuItems ();

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'UpdateUserActivity(NSUserActivity)' instead.")]
		[Export ("updateUserActivity:userInfo:webpageURL:")]
		// This NSDictionary is OK, it is arbitrary and user specific
		void UpdateUserActivity (string type, [NullAllowed] NSDictionary userInfo, [NullAllowed] NSUrl webpageURL);

		[Watch (5, 0)]
		[Export ("updateUserActivity:")]
		void UpdateUserActivity (NSUserActivity userActivity);

		[Export ("invalidateUserActivity")]
		void InvalidateUserActivity ();

		[ThreadSafe (false)]
		[Export ("presentTextInputControllerWithSuggestions:allowedInputMode:completion:")]
		[Async]
		void PresentTextInputController ([NullAllowed] string [] suggestions, WKTextInputMode inputMode, Action<NSArray> completion);

		[Export ("presentTextInputControllerWithSuggestionsForLanguage:allowedInputMode:completion:")]
		[Async]
		void PresentTextInputController ([NullAllowed] Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode, Action<NSArray> completion);

		[NoWatch]
		[Static, Export ("openParentApplication:reply:")]
		bool OpenParentApplication (NSDictionary userInfo, [NullAllowed] Action<NSDictionary, NSError> reply);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ReloadRootPageControllers' instead.")]
		[Static, Export ("reloadRootControllersWithNames:contexts:")]
		void ReloadRootControllers (string [] names, [NullAllowed] NSObject [] contexts);

		[Static]
		[Export ("reloadRootPageControllersWithNames:contexts:orientation:pageIndex:")]
		void ReloadRootPageControllers (string [] names, [NullAllowed] NSObject [] contexts, WKPageOrientation orientation, nint pageIndex);

#if !XAMCORE_3_0 && !NET
		// now exposed with the corresponding WKErrorCode enum
		[Field ("WatchKitErrorDomain")]
		NSString ErrorDomain { get; }
#endif

		[Export ("dismissMediaPlayerController")]
		void DismissMediaPlayerController ();

		[Export ("presentAudioRecorderControllerWithOutputURL:preset:options:completion:")]
		[Async]
		void PresentAudioRecorderController (NSUrl outputUrl, WKAudioRecorderPreset preset, [NullAllowed] NSDictionary options, Action<bool, NSError> completion);

		[Export ("dismissAudioRecorderController")]
		void DismissAudioRecorderController ();

		[Export ("animateWithDuration:animations:")]
		void AnimateWithDuration (double duration, Action animations);

		[Export ("presentAlertControllerWithTitle:message:preferredStyle:actions:")]
		void PresentAlertController ([NullAllowed] string title, [NullAllowed] string message, WKAlertControllerStyle preferredStyle, WKAlertAction [] actions);

		[Export ("presentAddPassesControllerWithPasses:completion:")]
		[Async]
		void PresentAddPassesController (PKPass [] passes, Action completion);

		[Export ("dismissAddPassesController")]
		void DismissAddPassesController ();

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Glances support was removed.")]
		[Export ("beginGlanceUpdates")]
		void BeginGlanceUpdates ();

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Glances support was removed.")]
		[Export ("endGlanceUpdates")]
		void EndGlanceUpdates ();

		[Export ("pickerDidFocus:")]
		void PickerDidFocus (WKInterfacePicker picker);

		[Export ("pickerDidResignFocus:")]
		void PickerDidResignFocus (WKInterfacePicker picker);

		[Export ("pickerDidSettle:")]
		void PickerDidSettle (WKInterfacePicker picker);

#if WATCH
		[Export ("presentMediaPlayerControllerWithURL:options:completion:")]
		[Async (ResultType = typeof (WKPresentMediaPlayerResult))]
		void PresentMediaPlayerController (NSUrl url, [NullAllowed] NSDictionary options, Action<bool, double, NSError> completion);
#endif

		[Export ("crownSequencer", ArgumentSemantic.Strong)]
		WKCrownSequencer CrownSequencer { get; }

		[Export ("scrollToObject:atScrollPosition:animated:")]
		void ScrollTo (WKInterfaceObject @object, WKInterfaceScrollPosition scrollPosition, bool animated);

		[Export ("interfaceDidScrollToTop")]
		void InterfaceDidScrollToTop ();

		[Export ("interfaceOffsetDidScrollToTop")]
		void InterfaceOffsetDidScrollToTop ();

		[Export ("interfaceOffsetDidScrollToBottom")]
		void InterfaceOffsetDidScrollToBottom ();

		[Watch (5, 0)]
		[Export ("contentSafeAreaInsets")]
		UIEdgeInsets ContentSafeAreaInsets { get; }

		[Watch (5, 0)]
		[Export ("systemMinimumLayoutMargins")]
		NSDirectionalEdgeInsets SystemMinimumLayoutMargins { get; }

		[Watch (5, 0)]
		[Export ("tableScrollingHapticFeedbackEnabled")]
		bool TableScrollingHapticFeedbackEnabled { [Bind ("isTableScrollingHapticFeedbackEnabled")] get; set; }
	}

	[Unavailable (PlatformName.iOS)]
	[BaseType (typeof (WKInterfaceController))]
	[DisableDefaultCtor] // DesignatedInitializer below
	interface WKUserNotificationInterfaceController {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'DidReceiveNotification' instead.")]
		[Export ("didReceiveRemoteNotification:withCompletion:")]
		void DidReceiveRemoteNotification (NSDictionary remoteNotification, Action<WKUserNotificationInterfaceType> completionHandler);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'DidReceiveNotification' instead.")]
		[Export ("didReceiveLocalNotification:withCompletion:")]
		void DidReceiveLocalNotification (UILocalNotification localNotification, Action<WKUserNotificationInterfaceType> completionHandler);

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'DidReceiveNotification(UNNotification)' instead.")]
		[Export ("didReceiveNotification:withCompletion:")]
		void DidReceiveNotification (UNNotification notification, Action<WKUserNotificationInterfaceType> completionHandler);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use overload accepting an 'UNNotification' parameter.")]
		[Export ("suggestionsForResponseToActionWithIdentifier:forRemoteNotification:inputLanguage:")]
		string [] GetSuggestionsForResponseToAction (string identifier, NSDictionary remoteNotification, string inputLanguage);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use overload accepting an 'UNNotification' parameter.")]
		[Export ("suggestionsForResponseToActionWithIdentifier:forLocalNotification:inputLanguage:")]
		string [] GetSuggestionsForResponseToAction (string identifier, UILocalNotification localNotification, string inputLanguage);

		[Export ("suggestionsForResponseToActionWithIdentifier:forNotification:inputLanguage:")]
		string [] GetSuggestionsForResponseToAction (string identifier, UNNotification notification, string inputLanguage);

		[Watch (5, 0)]
		[Export ("notificationActions", ArgumentSemantic.Copy)]
		UNNotificationAction [] NotificationActions { get; set; }

		[Watch (5, 0)]
		[Export ("didReceiveNotification:")]
		void DidReceiveNotification (UNNotification notification);

		[Watch (5, 0)]
		[Export ("performNotificationDefaultAction")]
		void PerformNotificationDefaultAction ();

		[Watch (5, 0)]
		[Export ("performDismissAction")]
		void PerformDismissAction ();

		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'PerformDismissAction' instead.")]
		[Export ("dismissController")]
		void DismissController ();

	}

	[Unavailable (PlatformName.iOS)]
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

		[Export ("setSemanticContentAttribute:")]
		void SetSemanticContentAttribute (WKInterfaceSemanticContentAttribute semanticContentAttribute);

		[Export ("setHorizontalAlignment:")]
		void SetHorizontalAlignment (WKInterfaceObjectHorizontalAlignment horizontalAlignment);

		[Export ("setVerticalAlignment:")]
		void SetVerticalAlignment (WKInterfaceObjectVerticalAlignment verticalAlignment);

		[Export ("setRelativeWidth:withAdjustment:")]
		void SetRelativeWidth (nfloat width, nfloat adjustment);

		[Export ("setRelativeHeight:withAdjustment:")]
		void SetRelativeHeight (nfloat height, nfloat adjustment);

		[Export ("sizeToFitWidth")]
		void SizeToFitWidth ();

		[Export ("sizeToFitHeight")]
		void SizeToFitHeight ();
	}

	[Unavailable (PlatformName.iOS)]
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
		void SetAccessibilityImageRegions (WKAccessibilityImageRegion [] accessibilityImageRegions);

		[Export ("setAccessibilityTraits:")]
		void SetAccessibilityTraits (UIAccessibilityTrait accessibilityTraits);

		[Export ("setIsAccessibilityElement:")]
		void SetIsAccessibilityElement (bool isAccessibilityElement);

		[Export ("setAccessibilityIdentifier:")]
		void SetAccessibilityIdentifier ([NullAllowed] string accessibilityIdentifier);

		[Notification]
		[Field ("WKAccessibilityVoiceOverStatusChanged")]
		NSString VoiceOverStatusChanged { get; }

		[Notification]
		[Field ("WKAccessibilityReduceMotionStatusDidChangeNotification")]
		NSString ReduceMotionStatusDidChangeNotification { get; }
	}

	[Unavailable (PlatformName.iOS)]
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

		[Export ("systemVersion")]
		string SystemVersion { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("model")]
		string Model { get; }

		[Export ("localizedModel")]
		string LocalizedModel { get; }

		[Export ("systemName")]
		string SystemName { get; }

		[Export ("waterResistanceRating")]
		WKWaterResistanceRating WaterResistanceRating { get; }

		[Export ("playHaptic:")]
		void PlayHaptic (WKHapticType type);

		[Export ("layoutDirection")]
		WKInterfaceLayoutDirection LayoutDirection { get; }

		[Static]
		[Export ("interfaceLayoutDirectionForSemanticContentAttribute:")]
		WKInterfaceLayoutDirection GetInterfaceLayoutDirection (WKInterfaceSemanticContentAttribute semanticContentAttribute);

		[Export ("wristLocation")]
		WKInterfaceDeviceWristLocation WristLocation { get; }

		[Export ("crownOrientation")]
		WKInterfaceDeviceCrownOrientation CrownOrientation { get; }

		[Export ("batteryMonitoringEnabled")]
		bool BatteryMonitoringEnabled { [Bind ("isBatteryMonitoringEnabled")] get; set; }

		[Export ("batteryLevel")]
		float BatteryLevel { get; }

		[Export ("batteryState")]
		WKInterfaceDeviceBatteryState BatteryState { get; }

		[Watch (6, 0)]
		[Export ("supportsAudioStreaming")]
		bool SupportsAudioStreaming { get; }

		[Watch (6, 2)]
		[NullAllowed, Export ("identifierForVendor", ArgumentSemantic.Strong)]
		NSUuid IdentifierForVendor { get; }

		[Watch (6, 1)]
		[Export ("enableWaterLock")]
		void EnableWaterLock ();

		[Watch (6, 1)]
		[Export ("isWaterLockEnabled")]
		bool IsWaterLockEnabled { get; }
	}

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
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

		[Export ("setContentInset:")]
		void SetContentInset (UIEdgeInsets contentInset);
	}

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceTable {
		[Export ("numberOfRows")]
		nint NumberOfRows { get; }

		[Export ("setRowTypes:")]
		void SetRowTypes (string [] rowTypes);

		[Export ("setNumberOfRows:withRowType:")]
		void SetNumberOfRows (nint numberOfRows, string rowType);

		[return: NullAllowed]
		[Export ("rowControllerAtIndex:")]
		NSObject GetRowController (nint index);

		[Export ("insertRowsAtIndexes:withRowType:")]
		void InsertRows (NSIndexSet rowIndexes, string rowType);

		[Export ("removeRowsAtIndexes:")]
		void RemoveRows (NSIndexSet rowIndexes);

		[Export ("scrollToRowAtIndex:")]
		void ScrollToRow (nint index);

		[Export ("performSegueForRow:")]
		void PerformSegue (nint row);

		[Watch (5, 1)]
		[Export ("curvesAtTop")]
		bool CurvesAtTop { get; set; }

		[Watch (5, 1)]
		[Export ("curvesAtBottom")]
		bool CurvesAtBottom { get; set; }
	}

	[Unavailable (PlatformName.iOS)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceMap {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'MKMapView' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

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

		[Watch (6, 1)]
		[Export ("setShowsUserHeading:")]
		void SetShowsUserHeading (bool showsUserHeading);

		[Watch (6, 1)]
		[Export ("setShowsUserLocation:")]
		void SetShowsUserLocation (bool showsUserLocation);

		[Watch (6, 1)]
		[Export ("setUserTrackingMode:animated:")]
		void SetUserTrackingMode (WKInterfaceMapUserTrackingMode mode, bool animated);
	}

	[Unavailable (PlatformName.iOS)]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	[BaseType (typeof (WKInterfaceObject))]
	interface WKInterfaceSeparator {
		[Export ("setColor:")]
		void SetColor ([NullAllowed] UIColor color);
	}

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
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

	[Unavailable (PlatformName.iOS)]
	[BaseType (typeof (NSObject))]
	interface WKAccessibilityImageRegion {

		[Export ("frame")]
		CGRect Frame { get; set; }

		[Export ("label")]
		string Label { get; set; }
	}

	interface IWKImageAnimatable { }

	[Unavailable (PlatformName.iOS)]
	[Protocol]
	interface WKImageAnimatable {

		[Abstract]
		[Export ("startAnimating")]
		void StartAnimating ();

		[Abstract]
		[Export ("startAnimatingWithImagesInRange:duration:repeatCount:")]
		void StartAnimating (NSRange imageRange, double duration, nint repeatCount);

		[Abstract]
		[Export ("stopAnimating")]
		void StopAnimating ();
	}

	[NoiOS]
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

	[NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKAlertAction {
		[Static]
		[Export ("actionWithTitle:style:handler:")]
		WKAlertAction Create (string title, WKAlertActionStyle style, Action handler);
	}

	[NoiOS]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AVPlayer' or 'AVQueuePlayer' instead.")]
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

	[NoiOS]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AVPlayer' or 'AVQueuePlayer' instead.")]
	[BaseType (typeof (NSObject))]
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

	[NoiOS]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AVPlayer' or 'AVQueuePlayer' instead.")]
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
		double CurrentTime {
			get;
#if NET
			set;
		}
#else
		}
		[Export ("setCurrentTime:")]
		void SetCurrentTime (double time);
#endif

		[Notification]
		[Field ("WKAudioFilePlayerItemTimeJumpedNotification")]
		NSString TimeJumpedNotification { get; }

		[Notification]
		[Field ("WKAudioFilePlayerItemDidPlayToEndTimeNotification")]
		NSString DidPlayToEndTimeNotification { get; }

		[Notification]
		[Field ("WKAudioFilePlayerItemFailedToPlayToEndTimeNotification")]
		NSString FailedToPlayToEndTimeNotification { get; }
	}

	[NoiOS]
	[BaseType (typeof (NSObject))]
	interface WKExtension {
		[Static]
		[Export ("sharedExtension")]
		WKExtension SharedExtension { get; }

		[Export ("openSystemURL:")]
		void OpenSystemUrl (NSUrl url);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKExtensionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("rootInterfaceController")]
		WKInterfaceController RootInterfaceController { get; }

		[Export ("applicationState")]
		WKApplicationState ApplicationState { get; }

		[NullAllowed, Export ("visibleInterfaceController")]
		WKInterfaceController VisibleInterfaceController { get; }

		[Export ("isApplicationRunningInDock")]
		bool IsApplicationRunningInDock { get; }

		[Export ("autorotating")]
		bool Autorotating { [Bind ("isAutorotating")] get; set; }

		[Watch (4, 2)]
		[Export ("autorotated")]
		bool Autorotated { [Bind ("isAutorotated")] get; }

		[Deprecated (PlatformName.WatchOS, 7, 0)]
		[Export ("frontmostTimeoutExtended")]
		bool FrontmostTimeoutExtended { [Bind ("isFrontmostTimeoutExtended")] get; set; }

		[Export ("enableWaterLock")]
		[Deprecated (PlatformName.WatchOS, 6, 1, message: "Use 'WKInterfaceDevice.EnableWaterLock' instead.")]
		void EnableWaterLock ();

		[Watch (6, 0)]
		[Export ("registerForRemoteNotifications")]
		void RegisterForRemoteNotifications ();

		[Watch (6, 0)]
		[Export ("unregisterForRemoteNotifications")]
		void UnregisterForRemoteNotifications ();

		[Watch (6, 0)]
		[Export ("registeredForRemoteNotifications")]
		bool RegisteredForRemoteNotifications { [Bind ("isRegisteredForRemoteNotifications")] get; }

		[Watch (7, 0)]
		[Export ("globalTintColor")]
		UIColor GlobalTintColor { get; }

		[Watch (7, 0)]
		[Notification, Field ("WKApplicationDidFinishLaunchingNotification")]
		NSString DidFinishLaunchingNotification { get; }

		[Watch (7, 0)]
		[Notification, Field ("WKApplicationDidBecomeActiveNotification")]
		NSString DidBecomeActiveNotification { get; }

		[Watch (7, 0)]
		[Notification, Field ("WKApplicationWillResignActiveNotification")]
		NSString WillResignActiveNotification { get; }

		[Watch (7, 0)]
		[Notification, Field ("WKApplicationWillEnterForegroundNotification")]
		NSString WillEnterForegroundNotification { get; }

		[Watch (7, 0)]
		[Notification, Field ("WKApplicationDidEnterBackgroundNotification")]
		NSString DidEnterBackgroundNotification { get; }
	}

	interface IWKExtensionDelegate { }

	[NoiOS]
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

		[Export ("applicationWillEnterForeground")]
		void ApplicationWillEnterForeground ();

		[Export ("applicationDidEnterBackground")]
		void ApplicationDidEnterBackground ();

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forRemoteNotification:")]
		void HandleAction ([NullAllowed] string identifier, NSDictionary remoteNotification);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forLocalNotification:")]
		void HandleAction ([NullAllowed] string identifier, UILocalNotification localNotification);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forRemoteNotification:withResponseInfo:")]
		void HandleAction ([NullAllowed] string identifier, NSDictionary remoteNotification, NSDictionary responseInfo);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("handleActionWithIdentifier:forLocalNotification:withResponseInfo:")]
		void HandleAction ([NullAllowed] string identifier, UILocalNotification localNotification, NSDictionary responseInfo);

		[Export ("handleUserActivity:")]
		void HandleUserActivity ([NullAllowed] NSDictionary userInfo);

		[Export ("handleActivity:")]
		void HandleUserActivity (NSUserActivity userActivity);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("didReceiveRemoteNotification:")]
		void DidReceiveRemoteNotification (NSDictionary userInfo);

		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'UNUserNotificationCenterDelegate' instead.")]
		[Export ("didReceiveLocalNotification:")]
		void DidReceiveLocalNotification (UILocalNotification notification);

		[Export ("handleBackgroundTasks:")]
		void HandleBackgroundTasks (NSSet<WKRefreshBackgroundTask> backgroundTasks);

		[Export ("handleWorkoutConfiguration:")]
		void HandleWorkoutConfiguration (HKWorkoutConfiguration workoutConfiguration);

		[Export ("deviceOrientationDidChange")]
		void DeviceOrientationDidChange ();

		[Watch (5, 0)]
		[Export ("handleActiveWorkoutRecovery")]
		void HandleActiveWorkoutRecovery ();

		[Watch (5, 0)]
		[Export ("handleRemoteNowPlayingActivity")]
		void HandleRemoteNowPlayingActivity ();

		[Watch (5, 0)]
		[Export ("handleIntent:completionHandler:")]
		void HandleIntent (INIntent intent, Action<INIntentResponse> completionHandler);

		[Watch (6, 0)]
		[Export ("handleExtendedRuntimeSession:")]
		void HandleExtendedRuntimeSession (WKExtendedRuntimeSession extendedRuntimeSession);

		[Watch (6, 0)]
		[Export ("didRegisterForRemoteNotificationsWithDeviceToken:")]
		void DidRegisterForRemoteNotifications (NSData deviceToken);

		[Watch (6, 0)]
		[Export ("didFailToRegisterForRemoteNotificationsWithError:")]
		void DidFailToRegisterForRemoteNotifications (NSError error);

		[Watch (6, 0)]
		[Export ("didReceiveRemoteNotification:fetchCompletionHandler:")]
		void DidReceiveRemoteNotification (NSDictionary userInfo, Action<WKBackgroundFetchResult> completionHandler);

		[Watch (7, 0)]
		[Export ("userDidAcceptCloudKitShareWithMetadata:")]
		void UserDidAcceptCloudKitShare (CKShareMetadata cloudKitShareMetadata);
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // The super class' init method is unavailable.
	interface WKInterfaceActivityRing {
		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("setActivitySummary:animated:")]
		void SetActivitySummary ([NullAllowed] HKActivitySummary activitySummary, bool animated);
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // The super class' init method is unavailable.
	interface WKInterfaceMovie {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'AVVideoPlayer' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("setMovieURL:")]
		void SetMovieUrl (NSUrl url);

		[Export ("setVideoGravity:")]
		void SetVideoGravity (WKVideoGravity videoGravity);

		[Export ("setLoops:")]
		void SetLoops (bool loops);

		[Export ("setPosterImage:")]
		void SetPosterImage ([NullAllowed] WKImage posterImage);
	}

	[NoiOS]
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
		void SetItems ([NullAllowed] WKPickerItem [] items);

		[Export ("setCoordinatedAnimations:")]
		void SetCoordinatedAnimations ([NullAllowed] IWKImageAnimatable [] coordinatedAnimations);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);
	}

	[NoiOS]
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

	[NoiOS]
	[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'AVPlayer' or 'AVQueuePlayer' instead.")]
	[BaseType (typeof (WKAudioFilePlayer))]
	[DisableDefaultCtor]
	interface WKAudioFileQueuePlayer {
		[Static]
		[Export ("queuePlayerWithItems:")]
		WKAudioFileQueuePlayer FromItems (WKAudioFilePlayerItem [] items);

		[Export ("advanceToNextItem")]
		void AdvanceToNextItem ();

		[Export ("appendItem:")]
		void AppendItem (WKAudioFilePlayerItem item);

		[Export ("removeItem:")]
		void RemoveItem (WKAudioFilePlayerItem item);

		[Export ("removeAllItems")]
		void RemoveAllItems ();

		[Export ("items")]
		WKAudioFilePlayerItem [] Items { get; }
	}

	// to be made [Internal] once #34656 is fixed
	[Static]
	[NoiOS]
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
	[NoiOS]
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

	[NoiOS]
	[BaseType (typeof (NSObject))]
	interface WKRefreshBackgroundTask {

		[NullAllowed, Export ("userInfo")]
		INSSecureCoding UserInfo { get; }

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'SetTaskCompleted (false)' instead.")]
		[Export ("setTaskCompleted")]
		void SetTaskCompleted ();

		[Export ("setTaskCompletedWithSnapshot:")]
		void SetTaskCompleted (bool refreshSnapshot);

		[Watch (8, 0)]
		[NullAllowed, Export ("expirationHandler", ArgumentSemantic.Strong)]
		Action ExpirationHandler { get; set; }
	}

	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKApplicationRefreshBackgroundTask {
	}

	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKSnapshotRefreshBackgroundTask {

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'ReasonForSnapshot' instead.")]
		[Export ("returnToDefaultState")]
		bool ReturnToDefaultState { get; }

		[Export ("reasonForSnapshot")]
		WKSnapshotReason ReasonForSnapshot { get; }

		[Export ("setTaskCompletedWithDefaultStateRestored:estimatedSnapshotExpiration:userInfo:")]
		void SetTaskCompleted (bool restoredDefaultState, [NullAllowed] NSDate estimatedSnapshotExpiration, [NullAllowed] INSSecureCoding userInfo);
	}

	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask), Name = "WKURLSessionRefreshBackgroundTask")]
	interface WKUrlSessionRefreshBackgroundTask {

		[Export ("sessionIdentifier")]
		string SessionIdentifier { get; }
	}

	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKWatchConnectivityRefreshBackgroundTask {
	}

	[Watch (5, 0)]
	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKRelevantShortcutRefreshBackgroundTask {
	}

	[Watch (5, 0)]
	[NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKIntentDidRunRefreshBackgroundTask {
	}

	[NoiOS]
	[Category]
	[BaseType (typeof (WKExtension))]
	interface WKExtension_WKBackgroundTasks {

		[Export ("scheduleBackgroundRefreshWithPreferredDate:userInfo:scheduledCompletion:")]
		void ScheduleBackgroundRefresh (NSDate preferredFireDate, [NullAllowed] INSSecureCoding userInfo, Action<NSError> scheduledCompletion);

		[Export ("scheduleSnapshotRefreshWithPreferredDate:userInfo:scheduledCompletion:")]
		void ScheduleSnapshotRefresh (NSDate preferredFireDate, [NullAllowed] INSSecureCoding userInfo, Action<NSError> scheduledCompletion);
	}

	[NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKCrownSequencer {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IWKCrownDelegate Delegate { get; set; }

		[Export ("rotationsPerSecond")]
		double RotationsPerSecond { get; }

		[Export ("idle")]
		bool Idle { [Bind ("isIdle")] get; }

		[Export ("focus")]
		void Focus ();

		[Export ("resignFocus")]
		void ResignFocus ();

		[Watch (5, 0)]
		[Export ("hapticFeedbackEnabled")]
		bool HapticFeedbackEnabled { [Bind ("isHapticFeedbackEnabled")] get; set; }
	}

	interface IWKCrownDelegate { }

	[NoiOS]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface WKCrownDelegate {
		[Export ("crownDidRotate:rotationalDelta:")]
		void CrownDidRotate ([NullAllowed] WKCrownSequencer crownSequencer, double rotationalDelta);

		[Export ("crownDidBecomeIdle:")]
		void CrownDidBecomeIdle ([NullAllowed] WKCrownSequencer crownSequencer);
	}

	[NoiOS]
	[Abstract]
	[BaseType (typeof (NSObject))]
	interface WKGestureRecognizer {

		[Export ("state")]
		WKGestureRecognizerState State { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("locationInObject")]
		CGPoint LocationInObject { get; }

		[Export ("objectBounds")]
		CGRect ObjectBounds { get; }
	}

	[NoiOS]
	[BaseType (typeof (WKGestureRecognizer))]
	interface WKTapGestureRecognizer {

		[Export ("numberOfTapsRequired")]
		nuint NumberOfTapsRequired { get; set; }
	}

	[NoiOS]
	[BaseType (typeof (WKGestureRecognizer))]
	interface WKLongPressGestureRecognizer {

		[Export ("minimumPressDuration")]
		double MinimumPressDuration { get; set; }

		[Export ("numberOfTapsRequired")]
		nuint NumberOfTapsRequired { get; set; }

		[Export ("allowableMovement")]
		nfloat AllowableMovement { get; set; }
	}

	[NoiOS]
	[BaseType (typeof (WKGestureRecognizer))]
	interface WKSwipeGestureRecognizer {

		[Export ("direction", ArgumentSemantic.Assign)]
		WKSwipeGestureRecognizerDirection Direction { get; set; }
	}

	[NoiOS]
	[BaseType (typeof (WKGestureRecognizer))]
	interface WKPanGestureRecognizer {
		[Export ("translationInObject")]
		CGPoint TranslationInObject { get; }

		[Export ("velocityInObject")]
		CGPoint VelocityInObject { get; }
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceHMCamera {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'HMCameraView' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("setCameraSource:")]
		void SetCameraSource ([NullAllowed] HMCameraSource cameraSource);
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceInlineMovie {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("setMovieURL:")]
		void SetMovieUrl (NSUrl url);

		[Export ("setVideoGravity:")]
		void SetVideoGravity (WKVideoGravity videoGravity);

		[Export ("setLoops:")]
		void SetLoops (bool loops);

		[Export ("setAutoplays:")]
		void SetAutoplays (bool autoplays);

		[Export ("setPosterImage:")]
		void SetPosterImage ([NullAllowed] WKImage posterImage);

		[Export ("play")]
		void Play ();

		[Export ("playFromBeginning")]
		void PlayFromBeginning ();

		[Export ("pause")]
		void Pause ();
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfacePaymentButton {

		[Watch (6, 0)]
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor ([NullAllowed] NSObject target, Selector action);
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceSCNScene : SCNSceneRenderer {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'SCNSceneView' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("snapshot")]
		UIImage GetSnapshot ();

		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[Export ("antialiasingMode", ArgumentSemantic.Assign)]
		SCNAntialiasingMode AntialiasingMode { get; set; }
	}

	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Do not subclass or create instances of this class yourself. -> Handle is nil if init is called
	interface WKInterfaceSKScene {

		[Watch (6, 0)]
		[Advice ("This API exists for SwiftUI and is not generally needed.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'SKSpriteView' instead.")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[Export ("presentScene:")]
		void PresentScene ([NullAllowed] SKScene scene);

		[Export ("presentScene:transition:")]
		void PresentScene (SKScene scene, SKTransition transition);

		[NullAllowed, Export ("scene")]
		SKScene Scene { get; }

		[Export ("textureFromNode:")]
		[return: NullAllowed]
		SKTexture CreateTexture (SKNode node);

		[Export ("textureFromNode:crop:")]
		[return: NullAllowed]
		SKTexture CreateTexture (SKNode node, CGRect crop);
	}

	[Watch (5, 0)]
	[NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor]
	interface WKInterfaceVolumeControl {
		[Watch (6, 0)]
		[Export ("initWithOrigin:")]
		NativeHandle Constructor (WKInterfaceVolumeControlOrigin origin);

		[Export ("setTintColor:")]
		void SetTintColor ([NullAllowed] UIColor tintColor);

		[Watch (6, 0)]
		[Export ("focus")]
		void Focus ();

		[Watch (6, 0)]
		[Export ("resignFocus")]
		void ResignFocus ();
	}

	[Watch (6, 0), NoiOS]
	[Native]
	enum WKBackgroundFetchResult : ulong {
		NewData,
		NoData,
		Failed,
	}

	[Watch (6, 0), NoiOS]
	[Native]
	[ErrorDomain ("WKExtendedRuntimeSessionErrorDomain")]
	enum WKExtendedRuntimeSessionErrorCode : long {
		Unknown = 1,
		ScheduledTooFarInAdvance = 2,
		MustBeActiveToStartOrSchedule = 3,
		NotYetStarted = 4,
		ExceededResourceLimits = 5,
		BARDisabled = 6,
		NotApprovedToStartSession = 7,
		NotApprovedToSchedule = 8,
		MustBeActiveToPrompt = 9,
		UnsupportedSessionType = 10,
	}

	[Watch (6, 0), NoiOS]
	[Native]
	enum WKExtendedRuntimeSessionInvalidationReason : long {
		None,
		SessionInProgress,
		Expired,
		ResignedFrontmost,
		SuppressedBySystem,
		Error = -1,
	}

	[Watch (6, 0), NoiOS]
	[Native]
	enum WKExtendedRuntimeSessionState : long {
		NotStarted,
		Scheduled,
		Running,
		Invalid,
	}

	[Watch (6, 0), NoiOS]
	[Native]
	enum WKInterfaceVolumeControlOrigin : long {
		Local,
		Companion,
	}

	[Watch (6, 1), NoiOS]
	[Native]
	enum WKInterfaceAuthorizationAppleIdButtonStyle : long {
		Default,
		White,
	}

	[Watch (6, 1), NoiOS]
	[Native]
	enum WKInterfaceMapUserTrackingMode : long {
		None,
		Follow,
	}

	interface IWKExtendedRuntimeSessionDelegate { }

	[Watch (6, 0), NoiOS]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface WKExtendedRuntimeSessionDelegate {

		[Abstract]
		[Export ("extendedRuntimeSession:didInvalidateWithReason:error:")]
		void DidInvalidate (WKExtendedRuntimeSession extendedRuntimeSession, WKExtendedRuntimeSessionInvalidationReason reason, [NullAllowed] NSError error);

		[Abstract]
		[Export ("extendedRuntimeSessionDidStart:")]
		void DidStart (WKExtendedRuntimeSession extendedRuntimeSession);

		[Abstract]
		[Export ("extendedRuntimeSessionWillExpire:")]
		void WillExpire (WKExtendedRuntimeSession extendedRuntimeSession);
	}

	[Watch (6, 0), NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Create method exists and `NSInvalidArgumentException Reason: *** -[__NSSetM addObject:]: object cannot be nil`
	interface WKExtendedRuntimeSession {

		[Static]
		[Export ("session")]
		WKExtendedRuntimeSession Create ();

		[Watch (9, 0), NoiOS]
		[Static]
		[Async]
		[Export ("requestAutoLaunchAuthorizationStatusWithCompletion:")]
		void RequestAutoLaunchAuthorizationStatus (WKRequestAutoLaunchAuthorizationStatusCompletionHandler completion);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKExtendedRuntimeSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("state")]
		WKExtendedRuntimeSessionState State { get; }

		[NullAllowed, Export ("expirationDate")]
		NSDate ExpirationDate { get; }

		[Export ("start")]
		void Start ();

		[Export ("startAtDate:")]
		void Start (NSDate date);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("notifyUserWithHaptic:repeatHandler:")]
		void NotifyUser (WKHapticType type, [NullAllowed] WKNofityUserIntervalHandler repeatHandler);
	}

	delegate double WKNofityUserIntervalHandler (WKHapticType type);
	delegate void WKRequestAutoLaunchAuthorizationStatusCompletionHandler (WKExtendedRuntimeSessionAutoLaunchAuthorizationStatus authorizationStatus, [NullAllowed] NSError error);

	[Watch (6, 0), NoiOS]
	[BaseType (typeof (WKInterfaceObject), Name = "WKInterfaceAuthorizationAppleIDButton")]
	[DisableDefaultCtor] // Handle is `nil`
	interface WKInterfaceAuthorizationAppleIdButton {
		[Export ("initWithTarget:action:")]
		[Deprecated (PlatformName.WatchOS, 6, 1, message: "Use 'new WKInterfaceAuthorizationAppleIdButton (WKInterfaceVolumeControlOrigin,NSObject,Selector)' instead.")]
		NativeHandle Constructor ([NullAllowed] NSObject target, Selector action);

		[Watch (6, 1)]
		[Export ("initWithStyle:target:action:")]
		NativeHandle Constructor (WKInterfaceAuthorizationAppleIdButtonStyle style, [NullAllowed] NSObject target, Selector action);
	}

	[Watch (6, 0), NoiOS]
	enum WKTextContentType {
		[DefaultEnumValue]
		[Field (null)] // API using fields accept `nil`
		None,
		[Field ("WKTextContentTypeName")]
		Name,
		[Field ("WKTextContentTypeNamePrefix")]
		NamePrefix,
		[Field ("WKTextContentTypeGivenName")]
		GivenName,
		[Field ("WKTextContentTypeMiddleName")]
		MiddleName,
		[Field ("WKTextContentTypeFamilyName")]
		FamilyName,
		[Field ("WKTextContentTypeNameSuffix")]
		NameSuffix,
		[Field ("WKTextContentTypeNickname")]
		Nickname,
		[Field ("WKTextContentTypeJobTitle")]
		JobTitle,
		[Field ("WKTextContentTypeOrganizationName")]
		OrganizationName,
		[Field ("WKTextContentTypeLocation")]
		Location,
		[Field ("WKTextContentTypeFullStreetAddress")]
		FullStreetAddress,
		[Field ("WKTextContentTypeStreetAddressLine1")]
		StreetAddressLine1,
		[Field ("WKTextContentTypeStreetAddressLine2")]
		StreetAddressLine2,
		[Field ("WKTextContentTypeAddressCity")]
		AddressCity,
		[Field ("WKTextContentTypeAddressState")]
		AddressState,
		[Field ("WKTextContentTypeAddressCityAndState")]
		AddressCityAndState,
		[Field ("WKTextContentTypeSublocality")]
		Sublocality,
		[Field ("WKTextContentTypeCountryName")]
		CountryName,
		[Field ("WKTextContentTypePostalCode")]
		PostalCode,
		[Field ("WKTextContentTypeTelephoneNumber")]
		TelephoneNumber,
		[Field ("WKTextContentTypeEmailAddress")]
		EmailAddress,
		[Field ("WKTextContentTypeURL")]
		Url,
		[Field ("WKTextContentTypeCreditCardNumber")]
		CreditCardNumber,
		[Field ("WKTextContentTypeUsername")]
		Username,
		[Field ("WKTextContentTypePassword")]
		Password,
		[Field ("WKTextContentTypeNewPassword")]
		NewPassword,
		[Field ("WKTextContentTypeOneTimeCode")]
		OneTimeCode,
	}

	[Watch (6, 0), NoiOS]
	[BaseType (typeof (WKInterfaceObject))]
	[DisableDefaultCtor] // Handle is `nil`
	interface WKInterfaceTextField {

		[Export ("setText:")]
		void SetText ([NullAllowed] string text);

		[Export ("setAttributedText:")]
		void SetText ([NullAllowed] NSAttributedString attributedText);

		[Export ("setPlaceholder:")]
		void SetPlaceholder ([NullAllowed] string placeholder);

		[Export ("setAttributedPlaceholder:")]
		void SetPlaceholder ([NullAllowed] NSAttributedString attributedPlaceholder);

		[Export ("setTextColor:")]
		void SetTextColor ([NullAllowed] UIColor color);

		[Export ("setEnabled:")]
		void SetEnabled (bool enabled);

		[Export ("setTextContentType:")]
		void SetTextContentType ([BindAs (typeof (WKTextContentType?))][NullAllowed] NSString textContentType);

		[Export ("setSecureTextEntry:")]
		void SetSecureTextEntry (bool secureTextEntry);
	}

	[Watch (7, 0), NoiOS]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKApplication {
		[Static]
		[Export ("sharedApplication")]
		WKApplication SharedApplication { get; }

		[Export ("openSystemURL:")]
		void OpenSystemUrl (NSUrl url);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKApplicationDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[NullAllowed, Export ("rootInterfaceController")]
		WKInterfaceController RootInterfaceController { get; }

		[NullAllowed, Export ("visibleInterfaceController")]
		WKInterfaceController VisibleInterfaceController { get; }

		[Export ("applicationState")]
		WKApplicationState ApplicationState { get; }

		[Export ("isApplicationRunningInDock")]
		bool IsApplicationRunningInDock { get; }

		[Export ("autorotating")]
		bool IsAutorotating { [Bind ("isAutorotating")] get; set; }

		[Export ("autorotated")]
		bool Autorotated { [Bind ("isAutorotated")] get; }

		[Export ("registerForRemoteNotifications")]
		void RegisterForRemoteNotifications ();

		[Export ("unregisterForRemoteNotifications")]
		void UnregisterForRemoteNotifications ();

		[Export ("registeredForRemoteNotifications")]
		bool RegisteredForRemoteNotifications { [Bind ("isRegisteredForRemoteNotifications")] get; }

		[Export ("globalTintColor")]
		UIColor GlobalTintColor { get; }

		// from interface WKBackgroundTasks (WKApplication)
		[Async]
		[Export ("scheduleBackgroundRefreshWithPreferredDate:userInfo:scheduledCompletion:")]
		void ScheduleBackgroundRefresh (NSDate preferredFireDate, [NullAllowed] NSObject userInfo, Action<NSError> scheduledCompletion);

		[Async]
		[Export ("scheduleSnapshotRefreshWithPreferredDate:userInfo:scheduledCompletion:")]
		void ScheduleSnapshotRefresh (NSDate preferredFireDate, [NullAllowed] NSObject userInfo, Action<NSError> scheduledCompletion);
	}

	interface IWKApplicationDelegate { }

	[Watch (7, 0), NoiOS]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface WKApplicationDelegate {
		[Export ("applicationDidFinishLaunching")]
		void ApplicationDidFinishLaunching ();

		[Export ("applicationDidBecomeActive")]
		void ApplicationDidBecomeActive ();

		[Export ("applicationWillResignActive")]
		void ApplicationWillResignActive ();

		[Export ("applicationWillEnterForeground")]
		void ApplicationWillEnterForeground ();

		[Export ("applicationDidEnterBackground")]
		void ApplicationDidEnterBackground ();

		[Export ("handleWorkoutConfiguration:")]
		void HandleWorkoutConfiguration (HKWorkoutConfiguration workoutConfiguration);

		[Export ("handleActiveWorkoutRecovery")]
		void HandleActiveWorkoutRecovery ();

		[Export ("handleExtendedRuntimeSession:")]
		void HandleExtendedRuntimeSession (WKExtendedRuntimeSession extendedRuntimeSession);

		[Export ("handleRemoteNowPlayingActivity")]
		void HandleRemoteNowPlayingActivity ();

		[Export ("handleUserActivity:")]
		void HandleUserActivity ([NullAllowed] NSDictionary userInfo);

		[Export ("handleActivity:")]
		void HandleActivity (NSUserActivity userActivity);

		[Export ("handleIntent:completionHandler:")]
		void HandleIntent (INIntent intent, Action<INIntentResponse> completionHandler);

		[Export ("handleBackgroundTasks:")]
		void HandleBackgroundTasks (NSSet<WKRefreshBackgroundTask> backgroundTasks);

		[Export ("deviceOrientationDidChange")]
		void DeviceOrientationDidChange ();

		[Export ("didRegisterForRemoteNotificationsWithDeviceToken:")]
		void DidRegisterForRemoteNotifications (NSData deviceToken);

		[Export ("didFailToRegisterForRemoteNotificationsWithError:")]
		void DidFailToRegisterForRemoteNotifications (NSError error);

		[Export ("didReceiveRemoteNotification:fetchCompletionHandler:")]
		void DidReceiveRemoteNotification (NSDictionary userInfo, Action<WKBackgroundFetchResult> completionHandler);

		[Export ("userDidAcceptCloudKitShareWithMetadata:")]
		void UserDidAcceptCloudKitShare (CKShareMetadata cloudKitShareMetadata);
	}

	[Watch (9, 0), NoiOS]
	[BaseType (typeof (WKRefreshBackgroundTask))]
	interface WKBluetoothAlertRefreshBackgroundTask { }
}
