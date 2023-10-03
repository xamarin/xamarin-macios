//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2009-2011, Novell, Inc.
// Copyrigh 2011-2013, Xamarin Inc.
// Copyrigh 2019, Microsoft Corporation.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using CloudKit;
#if !TVOS
using Contacts;
#else
using CNContact = System.Object;
#endif
#if !WATCH
using MediaPlayer;
using CoreImage;
using CoreAnimation;
#endif
using CoreData;
using UserNotifications;
using UniformTypeIdentifiers;
using Symbols;

#if IOS
using FileProvider;
using LinkPresentation;
#endif // IOS
#if TVOS || WATCH
using LPLinkMetadata = Foundation.NSObject;
#endif // TVOS || WATCH
using Intents;

// Unfortunately this file is a mix of #if's and [NoWatch] so we list
// some classes until [NoWatch] is used instead of #if's directives
// to avoid the usage of more #if's
#if WATCH
using CATransform3D = Foundation.NSObject;
using CALayer = Foundation.NSObject;
using CADisplayLink = Foundation.NSObject;
using CoreAnimation = Foundation.NSObject;
using CIColor = Foundation.NSObject;
using CIImage = Foundation.NSObject;

using MPMoviePlayerViewController = Foundation.NSObject;

using UIInteraction = Foundation.NSObjectProtocol;
using UIDynamicItem = Foundation.NSObjectProtocol;
using UITextFieldDelegate = Foundation.NSObjectProtocol;
using UITextPasteItem = Foundation.NSObjectProtocol;
using UICollectionViewDataSource = Foundation.NSObjectProtocol;
using UITableViewDataSource = Foundation.NSObjectProtocol;
using IUITextInput = Foundation.NSObjectProtocol;
using IUICoordinateSpace = Foundation.NSObjectProtocol;
using UIAccessibilityIdentification = Foundation.NSObjectProtocol;

using UIActivity = Foundation.NSObject;
using UICollectionViewLayout = Foundation.NSObject;
using UITraitCollection = Foundation.NSObject;
using UIButton = Foundation.NSObject;
using UIBlurEffect = Foundation.NSObject;
using UIControl = Foundation.NSObject;
using UIViewController = Foundation.NSObject;
using UIGestureRecognizer = Foundation.NSObject;
using UIAction = Foundation.NSObject;
using UITextField = Foundation.NSObject;
using UITextPosition = Foundation.NSObject;
using UITextRange = Foundation.NSObject;
using UITextSelectionRect = Foundation.NSObject;
using UIStoryboard = Foundation.NSObject;
using UIResponder = Foundation.NSObject;
using UIScreen = Foundation.NSObject;
using UIWindow = Foundation.NSObject;
using UIApplicationShortcutItem = Foundation.NSObject;
using UICollectionViewCell = Foundation.NSObject;
using UICollectionView = Foundation.NSObject;
using UITableViewCell = Foundation.NSObject;
using UITableView = Foundation.NSObject;
using UICollectionReusableView = Foundation.NSObject;
using UIVisualEffect = Foundation.NSObject;
using UILayoutGuide = Foundation.NSObject;
using UISwipeActionsConfiguration = Foundation.NSObject;
using UINib = Foundation.NSObject;
using UIFocusSystem = Foundation.NSObject;
using UIPrinterPickerController = Foundation.NSObject;
using UISplitViewController = Foundation.NSObject;
using UIAdaptivePresentationControllerDelegate = Foundation.NSObject;
using UIPresentationController = Foundation.NSObject;
using UIToolbar = Foundation.NSObject;
using UITabBar = Foundation.NSObject;
using UIEvent = Foundation.NSObject;
using UIUserActivityRestoring = Foundation.NSObject;
using UITouch = Foundation.NSObject;
using UIPress = Foundation.NSObject;
using UIPressesEvent = Foundation.NSObject;
using UIRegion = Foundation.NSObject;
using UITextInputPasswordRules = Foundation.NSObject;
using UIUserNotificationSettings = Foundation.NSObject;
using UIScrollView = Foundation.NSObject;
using UIFloatRange = Foundation.NSObject;
using NSSymbolEffect = Foundation.NSObject;
using NSSymbolContentTransition = Foundation.NSObject;
#endif // WATCH

#if !IOS
using UIPointerAccessoryPosition = Foundation.NSObject;
#endif // !IOS

#if __MACCATALYST__
using AppKit;
#else
using NSTouchBarProvider = Foundation.NSObject;
using NSTouchBar = Foundation.NSObject;
using NSToolbar = Foundation.NSObject;
#endif

#if !NET
using NSWritingDirection = UIKit.UITextWritingDirection;
#endif

using System;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace UIKit {

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	[Flags]
	public enum UIFocusHeading : ulong {
		None = 0,
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,
		Next = 1 << 4,
		Previous = 1 << 5,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		First = 1uL << 8,
		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		Last = 1uL << 9,
	}

	[Native] // NSInteger -> UIApplication.h
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIBackgroundRefreshStatus : long {
		Restricted, Denied, Available
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native] // NSUInteger -> UIApplication.h
	public enum UIBackgroundFetchResult : ulong {
		NewData, NoData, Failed
	}

	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIApplicationShortcutIconType : long {
		Compose,
		Play,
		Pause,
		Add,
		Location,
		Search,
		Share,
		// iOS 9.1 
		Prohibit,
		Contact,
		Home,
		MarkLocation,
		Favorite,
		Love,
		Cloud,
		Invitation,
		Confirmation,
		Mail,
		Message,
		Date,
		Time,
		CapturePhoto,
		CaptureVideo,
		Task,
		TaskCompleted,
		Alarm,
		Bookmark,
		Shuffle,
		Audio,
		Update
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIImpactFeedbackStyle : long {
		Light,
		Medium,
		Heavy,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Soft,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Rigid,
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UINotificationFeedbackType : long {
		Success,
		Warning,
		Error
	}

	[Native]
	[ErrorDomain ("UIGuidedAccessErrorDomain")]
	[NoWatch, NoTV, iOS (12, 2)]
	[MacCatalyst (13, 1)]
	public enum UIGuidedAccessErrorCode : long {
		PermissionDenied,
		Failed = long.MaxValue,
	}

	[Flags, NoWatch, NoTV, iOS (12, 2)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIGuidedAccessAccessibilityFeature : ulong {
		VoiceOver = 1uL << 0,
		Zoom = 1uL << 1,
		AssistiveTouch = 1uL << 2,
		InvertColors = 1uL << 3,
		GrayscaleDisplay = 1uL << 4,
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	public enum UIPrintRenderingQuality : long {
		Best,
		Responsive,
	}

	[NoWatch, TV (14, 5), iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	public enum UISplitViewControllerDisplayModeButtonVisibility : long {
		Automatic,
		Never,
		Always,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIButtonConfigurationIndicator : long {
		Automatic,
		None,
		Popup,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UISearchControllerScopeBarActivation : long {
		Automatic,
		Manual,
		OnTextEntry,
		OnSearchActivation,
	}

	delegate void UICompletionHandler (bool finished);
	delegate void UIOperationHandler (bool success);
	delegate void UICollectionViewLayoutInteractiveTransitionCompletion (bool completed, bool finished);
	delegate void UIPrinterContactPrinterHandler (bool available);
	delegate void UIPrinterPickerCompletionHandler (UIPrinterPickerController printerPickerController, bool userDidSelect, NSError error);

	delegate UISplitViewControllerDisplayMode UISplitViewControllerFetchTargetForActionHandler (UISplitViewController svc);
	delegate bool UISplitViewControllerDisplayEvent (UISplitViewController splitViewController, UIViewController vc, NSObject sender);
	delegate UIViewController UISplitViewControllerGetViewController (UISplitViewController splitViewController);
	delegate bool UISplitViewControllerCanCollapsePredicate (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController);
	delegate UIViewController UISplitViewControllerGetSecondaryViewController (UISplitViewController splitViewController, UIViewController primaryViewController);
	delegate void UIActivityViewControllerCompletion (NSString activityType, bool completed, NSExtensionItem [] returnedItems, NSError error);

	// In the hopes that the parameter is self document: this array  can contain either UIDocuments or UIResponders
	delegate void UIApplicationRestorationHandler (NSObject [] uidocumentOrResponderObjects);

#if !XAMCORE_3_0
	[NoWatch]
	[MacCatalyst (13, 1)]
#pragma warning disable 0618 // warning CS0618: 'CategoryAttribute.CategoryAttribute(bool)' is obsolete: 'Inline the static members in this category in the category's class (and remove this obsolete once fixed)'
	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
#pragma warning restore
	[BaseType (typeof (NSAttributedString))]
	interface NSAttributedStringAttachmentConveniences {
		[Internal]
		[Static, Export ("attributedStringWithAttachment:")]
		NSAttributedString FromTextAttachment (NSTextAttachment attachment);
	}
#endif

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[Abstract] // abstract class that should not be used directly
	[BaseType (typeof (NSObject))]
	interface UIFeedbackGenerator {

		[Export ("prepare")]
		void Prepare ();
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UIImpactFeedbackGenerator {

		[Export ("initWithStyle:")]
		NativeHandle Constructor (UIImpactFeedbackStyle style);

		[Export ("impactOccurred")]
		void ImpactOccurred ();

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("impactOccurredWithIntensity:")]
		void ImpactOccurred (nfloat intensity);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UINotificationFeedbackGenerator {

		[Export ("notificationOccurred:")]
		void NotificationOccurred (UINotificationFeedbackType notificationType);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UISelectionFeedbackGenerator {

		[Export ("selectionChanged")]
		void SelectionChanged ();
	}

	interface IUISheetPresentationControllerDelegate { }

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	interface UISheetPresentationControllerDelegate : UIAdaptivePresentationControllerDelegate {

		[Export ("sheetPresentationControllerDidChangeSelectedDetentIdentifier:")]
		void DidChangeSelectedDetentIdentifier (UISheetPresentationController sheetPresentationController);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIPresentationController))]
	[DisableDefaultCtor]
	interface UISheetPresentationController {

		[Export ("initWithPresentedViewController:presentingViewController:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUISheetPresentationControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("sourceView", ArgumentSemantic.Strong)]
		UIView SourceView { get; set; }

		[Export ("prefersEdgeAttachedInCompactHeight")]
		bool PrefersEdgeAttachedInCompactHeight { get; set; }

		[Export ("widthFollowsPreferredContentSizeWhenEdgeAttached")]
		bool WidthFollowsPreferredContentSizeWhenEdgeAttached { get; set; }

		[Export ("prefersGrabberVisible")]
		bool PrefersGrabberVisible { get; set; }

		[Export ("preferredCornerRadius")]
		nfloat PreferredCornerRadius { get; set; }

		[Export ("detents", ArgumentSemantic.Copy)]
		UISheetPresentationControllerDetent [] Detents { get; set; }

		[BindAs (typeof (UISheetPresentationControllerDetentIdentifier))]
		[NullAllowed, Export ("selectedDetentIdentifier")]
		NSString SelectedDetentIdentifier { get; set; }

		[BindAs (typeof (UISheetPresentationControllerDetentIdentifier))]
		[NullAllowed, Export ("largestUndimmedDetentIdentifier")]
		NSString LargestUndimmedDetentIdentifier { get; set; }

		[Export ("prefersScrollingExpandsWhenScrolledToEdge")]
		bool PrefersScrollingExpandsWhenScrolledToEdge { get; set; }

		[Export ("animateChanges:")]
		void AnimateChanges (Action changes);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("invalidateDetents")]
		void InvalidateDetents ();

		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("prefersPageSizing")]
		bool PrefersPageSizing { get; set; }
	}

	interface IUICloudSharingControllerDelegate { }

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UICloudSharingControllerDelegate {
		[Abstract]
		[Export ("cloudSharingController:failedToSaveShareWithError:")]
		void FailedToSaveShare (UICloudSharingController csc, NSError error);

		[Abstract]
		[Export ("itemTitleForCloudSharingController:")]
		[return: NullAllowed]
		string GetItemTitle (UICloudSharingController csc);

		[Export ("itemThumbnailDataForCloudSharingController:")]
		[return: NullAllowed]
		NSData GetItemThumbnailData (UICloudSharingController csc);

		[Export ("itemTypeForCloudSharingController:")]
		[return: NullAllowed]
		string GetItemType (UICloudSharingController csc);

		[Export ("cloudSharingControllerDidSaveShare:")]
		void DidSaveShare (UICloudSharingController csc);

		[Export ("cloudSharingControllerDidStopSharing:")]
		void DidStopSharing (UICloudSharingController csc);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	delegate void UICloudSharingControllerPreparationHandler (UICloudSharingController controller, [BlockCallback] UICloudSharingControllerPreparationCompletionHandler completion);

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	delegate void UICloudSharingControllerPreparationCompletionHandler ([NullAllowed] CKShare share, [NullAllowed] CKContainer container, [NullAllowed] NSError error);

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UICloudSharingController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the 'UIActivityViewController' class instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the 'UIActivityViewController' class instead.")]
		[Export ("initWithPreparationHandler:")]
		NativeHandle Constructor (UICloudSharingControllerPreparationHandler preparationHandler);

		[Export ("initWithShare:container:")]
		NativeHandle Constructor (CKShare share, CKContainer container);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUICloudSharingControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("share", ArgumentSemantic.Strong)]
		CKShare Share { get; }

		[Export ("availablePermissions", ArgumentSemantic.Assign)]
		UICloudSharingPermissionOptions AvailablePermissions { get; set; }

		[Export ("activityItemSource")]
		IUIActivityItemSource ActivityItemSource { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSAttributedString))]
	interface NSAttributedString_NSAttributedStringKitAdditions {
		[MacCatalyst (13, 1)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachments (NSRange range);
	}

	[Category, BaseType (typeof (NSMutableAttributedString))]
	interface NSMutableAttributedStringKitAdditions {
		[Export ("fixAttributesInRange:")]
		void FixAttributesInRange (NSRange range);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category, BaseType (typeof (NSLayoutConstraint))]
	interface NSIdentifier {
		[Export ("identifier")]
		string GetIdentifier ();

		[Export ("setIdentifier:")]
		void SetIdentifier ([NullAllowed] string id);
	}

	[Category]
	[BaseType (typeof (NSCoder))]
	interface NSCoder_UIGeometryKeyedCoding {
		[Export ("encodeCGPoint:forKey:")]
		void Encode (CGPoint point, string forKey);

		[MacCatalyst (13, 1)]
		[Export ("encodeCGVector:forKey:")]
		void Encode (CGVector vector, string forKey);

		[Export ("encodeCGSize:forKey:")]
		void Encode (CGSize size, string forKey);

		[Export ("encodeCGRect:forKey:")]
		void Encode (CGRect rect, string forKey);

		[Export ("encodeCGAffineTransform:forKey:")]
		void Encode (CGAffineTransform transform, string forKey);

		[Export ("encodeUIEdgeInsets:forKey:")]
		void Encode (UIEdgeInsets edgeInsets, string forKey);

		[MacCatalyst (13, 1)]
		[Export ("encodeDirectionalEdgeInsets:forKey:")]
		void Encode (NSDirectionalEdgeInsets directionalEdgeInsets, string forKey);

		[Export ("encodeUIOffset:forKey:")]
		void Encode (UIOffset uiOffset, string forKey);

		[Export ("decodeCGPointForKey:")]
		CGPoint DecodeCGPoint (string key);

		[MacCatalyst (13, 1)]
		[Export ("decodeCGVectorForKey:")]
		CGVector DecodeCGVector (string key);

		[Export ("decodeCGSizeForKey:")]
		CGSize DecodeCGSize (string key);

		[Export ("decodeCGRectForKey:")]
		CGRect DecodeCGRect (string key);

		[Export ("decodeCGAffineTransformForKey:")]
		CGAffineTransform DecodeCGAffineTransform (string key);

		[Export ("decodeUIEdgeInsetsForKey:")]
		UIEdgeInsets DecodeUIEdgeInsets (string key);

		[MacCatalyst (13, 1)]
		[Export ("decodeDirectionalEdgeInsetsForKey:")]
		NSDirectionalEdgeInsets DecodeDirectionalEdgeInsets (string key);

		[Export ("decodeUIOffsetForKey:")]
		UIOffset DecodeUIOffsetForKey (string key);
	}

	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'CoreMotion' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CoreMotion' instead.")]
	interface UIAcceleration {
		[Export ("timestamp")]
		double Time { get; }

		[Export ("x")]
		double X { get; }

		[Export ("y")]
		double Y { get; }

		[Export ("z")]
		double Z { get; }
	}

	[NoTV, NoWatch]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIAccelerometerDelegate) })]
	[Deprecated (PlatformName.iOS, 5, 0, message: "Use 'CoreMotion' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CoreMotion' instead.")]
	interface UIAccelerometer {
		[Static]
		[Export ("sharedAccelerometer")]
		UIAccelerometer SharedAccelerometer { get; }

		[Export ("updateInterval")]
		double UpdateInterval { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIAccelerometerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIAccelerometerDelegate {
#pragma warning disable 618
		[Export ("accelerometer:didAccelerate:"), EventArgs ("UIAccelerometer"), EventName ("Acceleration")]
		void DidAccelerate (UIAccelerometer accelerometer, UIAcceleration acceleration);
#pragma warning restore 618
	}

	interface UIAccessibility {
		[Export ("isAccessibilityElement")]
		bool IsAccessibilityElement { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityLabel", ArgumentSemantic.Copy)]
		string AccessibilityLabel { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityAttributedLabel", ArgumentSemantic.Copy)]
		NSAttributedString AccessibilityAttributedLabel { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityHint", ArgumentSemantic.Copy)]
		string AccessibilityHint { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityAttributedHint", ArgumentSemantic.Copy)]
		NSAttributedString AccessibilityAttributedHint { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityValue", ArgumentSemantic.Copy)]
		string AccessibilityValue { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityAttributedValue", ArgumentSemantic.Copy)]
		NSAttributedString AccessibilityAttributedValue { get; set; }

		[Export ("accessibilityTraits")]
		UIAccessibilityTrait AccessibilityTraits { get; set; }

		[Export ("accessibilityFrame")]
		CGRect AccessibilityFrame { get; set; }

		[Export ("accessibilityActivationPoint")]
		CGPoint AccessibilityActivationPoint { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityLanguage", ArgumentSemantic.Retain)]
		string AccessibilityLanguage { get; set; }

		[Export ("accessibilityElementsHidden")]
		bool AccessibilityElementsHidden { get; set; }

		[Export ("accessibilityViewIsModal")]
		bool AccessibilityViewIsModal { get; set; }

		[Export ("shouldGroupAccessibilityChildren")]
		bool ShouldGroupAccessibilityChildren { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("accessibilityNavigationStyle")]
		UIAccessibilityNavigationStyle AccessibilityNavigationStyle { get; set; }

		[TV (13, 0), iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityRespondsToUserInteraction")]
		bool AccessibilityRespondsToUserInteraction { get; set; }

		[TV (13, 0), iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityUserInputLabels", ArgumentSemantic.Strong)]
		string [] AccessibilityUserInputLabels { get; set; }

		[TV (13, 0), iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedUserInputLabels", ArgumentSemantic.Copy)]
		NSAttributedString [] AccessibilityAttributedUserInputLabels { get; set; }

		[TV (13, 0), iOS (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityTextualContext", ArgumentSemantic.Strong)]
		string AccessibilityTextualContext { get; set; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitNone")]
		long TraitNone { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitButton")]
		long TraitButton { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitLink")]
		long TraitLink { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitHeader")]
		long TraitHeader { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitSearchField")]
		long TraitSearchField { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitImage")]
		long TraitImage { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitSelected")]
		long TraitSelected { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitPlaysSound")]
		long TraitPlaysSound { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitKeyboardKey")]
		long TraitKeyboardKey { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitStaticText")]
		long TraitStaticText { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitSummaryElement")]
		long TraitSummaryElement { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitNotEnabled")]
		long TraitNotEnabled { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitUpdatesFrequently")]
		long TraitUpdatesFrequently { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitStartsMediaSession")]
		long TraitStartsMediaSession { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitAdjustable")]
		long TraitAdjustable { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitAllowsDirectInteraction")]
		long TraitAllowsDirectInteraction { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[Field ("UIAccessibilityTraitCausesPageTurn")]
		long TraitCausesPageTurn { get; }

		[Obsolete ("Use 'UIAccessibilityTraits' enum instead.")]
		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityTraitTabBar")]
		long TraitTabBar { get; }

		[Field ("UIAccessibilityAnnouncementDidFinishNotification")]
		[Notification (typeof (UIAccessibilityAnnouncementFinishedEventArgs))]
		NSString AnnouncementDidFinishNotification { get; }

#if !WATCH
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'VoiceOverStatusDidChangeNotification' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'VoiceOverStatusDidChangeNotification' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VoiceOverStatusDidChangeNotification' instead.")]
		[Field ("UIAccessibilityVoiceOverStatusChanged")]
		NSString VoiceOverStatusChanged { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityVoiceOverStatusDidChangeNotification")]
		[Notification]
		NSString VoiceOverStatusDidChangeNotification { get; }

		[Field ("UIAccessibilityMonoAudioStatusDidChangeNotification")]
		[Notification]
		NSString MonoAudioStatusDidChangeNotification { get; }

		[Field ("UIAccessibilityClosedCaptioningStatusDidChangeNotification")]
		[Notification]
		NSString ClosedCaptioningStatusDidChangeNotification { get; }

		[Field ("UIAccessibilityInvertColorsStatusDidChangeNotification")]
		[Notification]
		NSString InvertColorsStatusDidChangeNotification { get; }

		[Field ("UIAccessibilityGuidedAccessStatusDidChangeNotification")]
		[Notification]
		NSString GuidedAccessStatusDidChangeNotification { get; }
#endif

		[Field ("UIAccessibilityScreenChangedNotification")]
		int ScreenChangedNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityLayoutChangedNotification")]
		int LayoutChangedNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityAnnouncementNotification")]
		int AnnouncementNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityPageScrolledNotification")]
		int PageScrolledNotification { get; } // This is int, not nint

		[NullAllowed] // by default this property is null
		[Export ("accessibilityPath", ArgumentSemantic.Copy)]
		UIBezierPath AccessibilityPath { get; set; }

		[Export ("accessibilityActivate")]
		bool AccessibilityActivate ();

		[Field ("UIAccessibilitySpeechAttributePunctuation")]
		NSString SpeechAttributePunctuation { get; }

		[Field ("UIAccessibilitySpeechAttributeLanguage")]
		NSString SpeechAttributeLanguage { get; }

		[Field ("UIAccessibilitySpeechAttributePitch")]
		NSString SpeechAttributePitch { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Watch (10, 0)]
		[Field ("UIAccessibilitySpeechAttributeAnnouncementPriority")]
		NSString SpeechAttributeAnnouncementPriority { get; }

#if !WATCH
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityBoldTextStatusDidChangeNotification")]
		NSString BoldTextStatusDidChangeNotification { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Notification]
		[Field ("UIAccessibilityButtonShapesEnabledStatusDidChangeNotification")]
		NSString ButtonShapesEnabledStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityDarkerSystemColorsStatusDidChangeNotification")]
		NSString DarkerSystemColorsStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityGrayscaleStatusDidChangeNotification")]
		NSString GrayscaleStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityReduceMotionStatusDidChangeNotification")]
		NSString ReduceMotionStatusDidChangeNotification { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Notification]
		[Field ("UIAccessibilityPrefersCrossFadeTransitionsStatusDidChangeNotification")]
		NSString PrefersCrossFadeTransitionsStatusDidChangeNotification { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityVideoAutoplayStatusDidChangeNotification")]
		NSString VideoAutoplayStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityReduceTransparencyStatusDidChangeNotification")]
		NSString ReduceTransparencyStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilitySwitchControlStatusDidChangeNotification")]
		NSString SwitchControlStatusDidChangeNotification { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityNotificationSwitchControlIdentifier")]
		NSString NotificationSwitchControlIdentifier { get; }


		// Chose int because this should be UIAccessibilityNotifications type
		// just like UIAccessibilityAnnouncementNotification field
		[MacCatalyst (13, 1)]
		//[Notification] // int ScreenChangedNotification doesn't use this attr either
		[Field ("UIAccessibilityPauseAssistiveTechnologyNotification")]
		int PauseAssistiveTechnologyNotification { get; } // UIAccessibilityNotifications => uint32_t

		// Chose int because this should be UIAccessibilityNotifications type
		// just like UIAccessibilityAnnouncementNotification field
		[MacCatalyst (13, 1)]
		//[Notification] // int ScreenChangedNotification doesn't use this attr either
		[Field ("UIAccessibilityResumeAssistiveTechnologyNotification")]
		int ResumeAssistiveTechnologyNotification { get; } // UIAccessibilityNotifications => uint32_t

#if !WATCH
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilitySpeakScreenStatusDidChangeNotification")]
		NSString SpeakScreenStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilitySpeakSelectionStatusDidChangeNotification")]
		NSString SpeakSelectionStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityShakeToUndoDidChangeNotification")]
		NSString ShakeToUndoDidChangeNotification { get; }
#endif

		// FIXME: we only used this on a few types before, none of them available on tvOS
		// but a new member was added to the platform... 
		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[NullAllowed, Export ("accessibilityHeaderElements", ArgumentSemantic.Copy)]
		NSObject [] AccessibilityHeaderElements { get; set; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityElementFocusedNotification")]
		NSString ElementFocusedNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityFocusedElementKey")]
		NSString FocusedElementKey { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityUnfocusedElementKey")]
		NSString UnfocusedElementKey { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityAssistiveTechnologyKey")]
		NSString AssistiveTechnologyKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityNotificationVoiceOverIdentifier")]
		NSString NotificationVoiceOverIdentifier { get; }

#if !WATCH
		[NoTV]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityHearingDevicePairedEarDidChangeNotification")]
		NSString HearingDevicePairedEarDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityAssistiveTouchStatusDidChangeNotification")]
		NSString AssistiveTouchStatusDidChangeNotification { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotification")]
		NSString ShouldDifferentiateWithoutColorDidChangeNotification { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIAccessibilityOnOffSwitchLabelsDidChangeNotification")]
		NSString OnOffSwitchLabelsDidChangeNotification { get; }
#endif

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilitySpeechAttributeQueueAnnouncement")]
		NSString SpeechAttributeQueueAnnouncement { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilitySpeechAttributeIPANotation")]
		NSString SpeechAttributeIpaNotation { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityTextAttributeHeadingLevel")]
		NSString TextAttributeHeadingLevel { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityTextAttributeCustom")]
		NSString TextAttributeCustom { get; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityTextAttributeContext")]
		NSString TextAttributeContext { get; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilitySpeechAttributeSpellOut")]
		NSString SpeechAttributeSpellOut { get; }

		[iOS (17, 0)]
		[Export ("accessibilityDirectTouchOptions", ArgumentSemantic.Assign)]
		UIAccessibilityDirectTouchOptions AccessibilityDirectTouchOptions { get; set; }

	}

	interface UIAccessibilityAnnouncementFinishedEventArgs {
		[Export ("UIAccessibilityAnnouncementKeyStringValue")]
		string Announcement { get; }

		[Export ("UIAccessibilityAnnouncementKeyWasSuccessful")]
		bool WasSuccessful { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol (IsInformal = true)]
	interface UIAccessibilityContainer {
		[Export ("accessibilityElementCount")]
		nint AccessibilityElementCount ();

		[Export ("accessibilityElementAtIndex:")]
		NSObject GetAccessibilityElementAt (nint index);

		[Export ("indexOfAccessibilityElement:")]
		nint GetIndexOfAccessibilityElement (NSObject element);

		[Export ("accessibilityElements")]
		[MacCatalyst (13, 1)]
		NSObject GetAccessibilityElements ();

		[MacCatalyst (13, 1)]
		[Export ("setAccessibilityElements:")]
		void SetAccessibilityElements ([NullAllowed] NSObject elements);

		[MacCatalyst (13, 1)]
		[Export ("accessibilityContainerType", ArgumentSemantic.Assign)]
		UIAccessibilityContainerType AccessibilityContainerType { get; set; }
	}

	interface IUIAccessibilityContainerDataTableCell { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIAccessibilityContainerDataTableCell {
		[Abstract]
		[Export ("accessibilityRowRange")]
		NSRange GetAccessibilityRowRange ();

		[Abstract]
		[Export ("accessibilityColumnRange")]
		NSRange GetAccessibilityColumnRange ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIAccessibilityContainerDataTable {
		[Abstract]
		[Export ("accessibilityDataTableCellElementForRow:column:")]
		[return: NullAllowed]
		IUIAccessibilityContainerDataTableCell GetAccessibilityDataTableCellElement (nuint row, nuint column);

		[Abstract]
		[Export ("accessibilityRowCount")]
		nuint AccessibilityRowCount { get; }

		[Abstract]
		[Export ("accessibilityColumnCount")]
		nuint AccessibilityColumnCount { get; }

		[Export ("accessibilityHeaderElementsForRow:")]
		[return: NullAllowed]
		IUIAccessibilityContainerDataTableCell [] GetAccessibilityHeaderElementsForRow (nuint row);

		[Export ("accessibilityHeaderElementsForColumn:")]
		[return: NullAllowed]
		IUIAccessibilityContainerDataTableCell [] GetAccessibilityHeaderElementsForColumn (nuint column);
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	delegate bool UIAccessibilityCustomActionHandler (UIAccessibilityCustomAction customAction);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Please use the designated initializer
	partial interface UIAccessibilityCustomAction {
		[Export ("initWithName:target:selector:")]
		NativeHandle Constructor (string name, NSObject target, Selector selector);

		[MacCatalyst (13, 1)]
		[Export ("initWithAttributedName:target:selector:")]
		NativeHandle Constructor (NSAttributedString attributedName, [NullAllowed] NSObject target, Selector selector);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithName:actionHandler:")]
		NativeHandle Constructor (string name, [NullAllowed] UIAccessibilityCustomActionHandler actionHandler);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithAttributedName:actionHandler:")]
		NativeHandle Constructor (NSAttributedString attributedName, [NullAllowed] UIAccessibilityCustomActionHandler actionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithName:image:target:selector:")]
		NativeHandle Constructor (string name, [NullAllowed] UIImage image, [NullAllowed] NSObject target, Selector selector);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithAttributedName:image:target:selector:")]
		NativeHandle Constructor (NSAttributedString attributedName, [NullAllowed] UIImage image, [NullAllowed] NSObject target, Selector selector);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithName:image:actionHandler:")]
		NativeHandle Constructor (string name, [NullAllowed] UIImage image, UIAccessibilityCustomActionHandler actionHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithAttributedName:image:actionHandler:")]
		NativeHandle Constructor (NSAttributedString attributedName, [NullAllowed] UIImage image, UIAccessibilityCustomActionHandler actionHandler);

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("attributedName", ArgumentSemantic.Copy)]
		NSAttributedString AttributedName { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("target", ArgumentSemantic.Weak)]
		NSObject Target { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("selector", ArgumentSemantic.UnsafeUnretained)]
		Selector Selector { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Copy)]
		UIAccessibilityCustomActionHandler ActionHandler { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }
	}

	delegate UIAccessibilityCustomRotorItemResult UIAccessibilityCustomRotorSearch (UIAccessibilityCustomRotorSearchPredicate predicate);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIAccessibilityCustomRotor {

		[Export ("initWithName:itemSearchBlock:")]
		NativeHandle Constructor (string name, UIAccessibilityCustomRotorSearch itemSearchHandler);

		[MacCatalyst (13, 1)]
		[Export ("initWithAttributedName:itemSearchBlock:")]
		NativeHandle Constructor (NSAttributedString attributedName, UIAccessibilityCustomRotorSearch itemSearchBlock);

		[MacCatalyst (13, 1)]
		[Export ("initWithSystemType:itemSearchBlock:")]
		NativeHandle Constructor (UIAccessibilityCustomSystemRotorType type, UIAccessibilityCustomRotorSearch itemSearchBlock);

		[Export ("name")]
		string Name { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("attributedName", ArgumentSemantic.Copy)]
		NSAttributedString AttributedName { get; set; }

		[Export ("itemSearchBlock", ArgumentSemantic.Copy)]
		UIAccessibilityCustomRotorSearch ItemSearchHandler { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("systemRotorType")]
		UIAccessibilityCustomSystemRotorType SystemRotorType { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSObject))]
	interface NSObject_UIAccessibilityCustomRotor {

		[Export ("accessibilityCustomRotors")]
		[return: NullAllowed]
		UIAccessibilityCustomRotor [] GetAccessibilityCustomRotors ();

		[Export ("setAccessibilityCustomRotors:")]
		void SetAccessibilityCustomRotors ([NullAllowed] UIAccessibilityCustomRotor [] customRotors);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIAccessibilityCustomRotorItemResult {

		[Export ("initWithTargetElement:targetRange:")]
		NativeHandle Constructor (NSObject targetElement, [NullAllowed] UITextRange targetRange);

		[NullAllowed, Export ("targetElement", ArgumentSemantic.Weak)]
		NSObject TargetElement { get; set; }

		[NullAllowed, Export ("targetRange", ArgumentSemantic.Retain)]
		UITextRange TargetRange { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIAccessibilityCustomRotorSearchPredicate {

		[Export ("currentItem", ArgumentSemantic.Retain)]
		UIAccessibilityCustomRotorItemResult CurrentItem { get; set; }

		[Export ("searchDirection", ArgumentSemantic.Assign)]
		UIAccessibilityCustomRotorDirection SearchDirection { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIResponder))]
	// only happens on the simulator (not devices) on iOS8 (still make sense)
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Use initWithAccessibilityContainer:
	interface UIAccessibilityElement : UIAccessibilityIdentification {
		[Export ("initWithAccessibilityContainer:")]
		NativeHandle Constructor (NSObject container);

		[NullAllowed] // by default this property is null
		[Export ("accessibilityContainer", ArgumentSemantic.UnsafeUnretained)]
		NSObject AccessibilityContainer { get; set; }

		[Export ("isAccessibilityElement", ArgumentSemantic.UnsafeUnretained)]
		bool IsAccessibilityElement { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityLabel", ArgumentSemantic.Retain)]
		string AccessibilityLabel { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityHint", ArgumentSemantic.Retain)]
		string AccessibilityHint { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityValue", ArgumentSemantic.Retain)]
		string AccessibilityValue { get; set; }

		[Export ("accessibilityFrame", ArgumentSemantic.UnsafeUnretained)]
		CGRect AccessibilityFrame { get; set; }

		[Export ("accessibilityTraits", ArgumentSemantic.UnsafeUnretained)]
		ulong AccessibilityTraits { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("accessibilityFrameInContainerSpace", ArgumentSemantic.Assign)]
		CGRect AccessibilityFrameInContainerSpace { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIAccessibilityFocus {
		[Export ("accessibilityElementDidBecomeFocused")]
		void AccessibilityElementDidBecomeFocused ();

		[Export ("accessibilityElementDidLoseFocus")]
		void AccessibilityElementDidLoseFocus ();

		[Export ("accessibilityElementIsFocused")]
		bool AccessibilityElementIsFocused ();

		[MacCatalyst (13, 1)]
		[Export ("accessibilityAssistiveTechnologyFocusedIdentifiers")]
		NSSet<NSString> AccessibilityAssistiveTechnologyFocusedIdentifiers { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIAccessibilityAction {
		[Export ("accessibilityIncrement")]
		void AccessibilityIncrement ();

		[Export ("accessibilityDecrement")]
		void AccessibilityDecrement ();

		[Export ("accessibilityScroll:")]
		bool AccessibilityScroll (UIAccessibilityScrollDirection direction);

		[Export ("accessibilityPerformEscape")]
		bool AccessibilityPerformEscape ();

		[NoMacCatalyst]
		[Export ("accessibilityPerformMagicTap")]
		bool AccessibilityPerformMagicTap ();

		[MacCatalyst (13, 1)]
		[Export ("accessibilityCustomActions"), NullAllowed]
		UIAccessibilityCustomAction [] AccessibilityCustomActions { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	// NSObject category inlined in UIResponder
	interface UIAccessibilityDragging {
		[NullAllowed, Export ("accessibilityDragSourceDescriptors", ArgumentSemantic.Copy)]
		UIAccessibilityLocationDescriptor [] AccessibilityDragSourceDescriptors { get; set; }

		[NullAllowed, Export ("accessibilityDropPointDescriptors", ArgumentSemantic.Copy)]
		UIAccessibilityLocationDescriptor [] AccessibilityDropPointDescriptors { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIAccessibilityLocationDescriptor {
		[Export ("initWithName:view:")]
		NativeHandle Constructor (string name, UIView view);

		[Export ("initWithName:point:inView:")]
		NativeHandle Constructor (string name, CGPoint point, UIView view);

		[Export ("initWithAttributedName:point:inView:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedName, CGPoint point, UIView view);

		[NullAllowed, Export ("view", ArgumentSemantic.Weak)]
		UIView View { get; }

		[Export ("point")]
		CGPoint Point { get; }

		[Export ("name", ArgumentSemantic.Strong)]
		string Name { get; }

		[Export ("attributedName", ArgumentSemantic.Strong)]
		NSAttributedString AttributedName { get; }
	}

	[NoMac]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIAccessibilityContentSizeCategoryImageAdjusting {
		[Abstract]
		[Export ("adjustsImageSizeForAccessibilityContentSizeCategory")]
		bool AdjustsImageSizeForAccessibilityContentSizeCategory { get; set; }
	}

	[NoTV, NoWatch]
	[BaseType (typeof (UIView), KeepRefUntil = "Dismissed", Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIActionSheetDelegate) })]
	[Deprecated (PlatformName.iOS, 8, 3, message: "Use 'UIAlertController' with 'UIAlertControllerStyle.ActionSheet' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIAlertController' with 'UIAlertControllerStyle.ActionSheet' instead.")]
	interface UIActionSheet {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIAlertController' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIAlertController' instead.")]
		[Export ("initWithTitle:delegate:cancelButtonTitle:destructiveButtonTitle:otherButtonTitles:")]
		[Internal]
		[PostGet ("WeakDelegate")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] IUIActionSheetDelegate Delegate, [NullAllowed] string cancelTitle, [NullAllowed] string destroy, [NullAllowed] string other);

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIActionSheetDelegate Delegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("actionSheetStyle")]
		UIActionSheetStyle Style { get; set; }

		[Export ("addButtonWithTitle:")]
		nint AddButton (string title);

		[Export ("buttonTitleAtIndex:")]
		string ButtonTitle (nint index);

		[Export ("numberOfButtons")]
		nint ButtonCount { get; }

		[Export ("cancelButtonIndex")]
		nint CancelButtonIndex { get; set; }

		[Export ("destructiveButtonIndex")]
		nint DestructiveButtonIndex { get; set; }

		[Export ("firstOtherButtonIndex")]
		nint FirstOtherButtonIndex { get; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }

		[Export ("showFromToolbar:")]
		void ShowFromToolbar (UIToolbar view);

		[Export ("showFromTabBar:")]
		void ShowFromTabBar (UITabBar view);

		[Export ("showInView:")]
		void ShowInView (UIView view);

		[Export ("dismissWithClickedButtonIndex:animated:")]
		void DismissWithClickedButtonIndex (nint buttonIndex, bool animated);

		[Export ("showFromBarButtonItem:animated:")]
		void ShowFrom (UIBarButtonItem item, bool animated);

		[Export ("showFromRect:inView:animated:")]
		void ShowFrom (CGRect rect, UIView inView, bool animated);
	}

	delegate void UIActionHandler (UIAction action);

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIMenuElement))]
	[DisableDefaultCtor]
	interface UIAction : UIMenuLeaf {

		[Export ("title")]
		new string Title { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		new UIImage Image { get; set; }

		[NullAllowed, Export ("discoverabilityTitle")]
		new string DiscoverabilityTitle { get; set; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("attributes", ArgumentSemantic.Assign)]
		new UIMenuElementAttributes Attributes { get; set; }

		[Export ("state", ArgumentSemantic.Assign)]
		new UIMenuElementState State { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("sender")]
		new NSObject Sender { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("actionWithHandler:")]
		UIAction Create (UIActionHandler handler);

		[Static]
		[Export ("actionWithTitle:image:identifier:handler:")]
		UIAction Create (string title, [NullAllowed] UIImage image, [NullAllowed] string identifier, UIActionHandler handler);

		// From UIAction (UICaptureTextFromCameraSupporting) category

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("captureTextFromCameraActionForResponder:identifier:")]
		UIAction CaptureTextFromCameraAction (IUIKeyInput responder, [NullAllowed] string identifier);

	}

	interface IUIActionSheetDelegate { }

	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 8, 3)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	interface UIActionSheetDelegate {

		[Export ("actionSheet:clickedButtonAtIndex:"), EventArgs ("UIButton")]
		void Clicked (UIActionSheet actionSheet, nint buttonIndex);

		[Export ("actionSheetCancel:"), EventArgs ("UIActionSheet")]
		void Canceled (UIActionSheet actionSheet);

		[Export ("willPresentActionSheet:"), EventArgs ("UIActionSheet")]
		void WillPresent (UIActionSheet actionSheet);

		[Export ("didPresentActionSheet:"), EventArgs ("UIActionSheet")]
		void Presented (UIActionSheet actionSheet);

		[Export ("actionSheet:willDismissWithButtonIndex:"), EventArgs ("UIButton")]
		void WillDismiss (UIActionSheet actionSheet, nint buttonIndex);

		[Export ("actionSheet:didDismissWithButtonIndex:"), EventArgs ("UIButton")]
		void Dismissed (UIActionSheet actionSheet, nint buttonIndex);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIActivity {
		[Export ("activityType")]
		NSString Type { get; }

		[Export ("activityTitle")]
		string Title { get; }

		[Export ("activityImage")]
		UIImage Image { get; }

		[Export ("canPerformWithActivityItems:")]
		bool CanPerform (NSObject [] activityItems);

		[Export ("prepareWithActivityItems:")]
		void Prepare (NSObject [] activityItems);

		[Export ("activityViewController")]
		UIViewController ViewController { get; }

		[Export ("performActivity")]
		void Perform ();

		[Export ("activityDidFinish:")]
		void Finished (bool completed);

		[Export ("activityCategory"), Static]
		UIActivityCategory Category { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIActivityType {
		[Field ("UIActivityTypePostToFacebook")]
		NSString PostToFacebook { get; }

		[Field ("UIActivityTypePostToTwitter")]
		NSString PostToTwitter { get; }

		[Field ("UIActivityTypePostToWeibo")]
		NSString PostToWeibo { get; }

		[Field ("UIActivityTypeMessage")]
		NSString Message { get; }

		[Field ("UIActivityTypeMail")]
		NSString Mail { get; }

		[Field ("UIActivityTypePrint")]
		NSString Print { get; }

		[Field ("UIActivityTypeCopyToPasteboard")]
		NSString CopyToPasteboard { get; }

		[Field ("UIActivityTypeAssignToContact")]
		NSString AssignToContact { get; }

		[Field ("UIActivityTypeSaveToCameraRoll")]
		NSString SaveToCameraRoll { get; }

		[Field ("UIActivityTypeAddToReadingList")]
		NSString AddToReadingList { get; }

		[Field ("UIActivityTypePostToFlickr")]
		NSString PostToFlickr { get; }

		[Field ("UIActivityTypePostToVimeo")]
		NSString PostToVimeo { get; }

		[Field ("UIActivityTypePostToTencentWeibo")]
		NSString PostToTencentWeibo { get; }

		[Field ("UIActivityTypeAirDrop")]
		NSString AirDrop { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIActivityTypeOpenInIBooks")]
		NSString OpenInIBooks { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIActivityTypeMarkupAsPDF")]
		NSString MarkupAsPdf { get; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Field ("UIActivityTypeSharePlay")]
		NSString UIActivityTypeSharePlay { get; }

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UIActivityTypeCollaborationInviteWithLink")]
		NSString CollaborationInviteWithLink { get; }

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UIActivityTypeCollaborationCopyLink")]
		NSString CollaborationCopyLink { get; }

		[NoTV, iOS (16, 4), MacCatalyst (16, 4)]
		[Field ("UIActivityTypeAddToHomeScreen")]
		NSString AddToHomeScreen { get; }
	}

	//
	// You're supposed to implement this protocol in your UIView subclasses, not provide
	// a implementation for only this protocol, which is why there is no model to subclass.
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIInputViewAudioFeedback {
		[Export ("enableInputClicksWhenVisible")]
#if !NET
		[Abstract]
#endif
		bool EnableInputClicksWhenVisible { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSOperation))]
	[ThreadSafe]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use initWithPlaceholderItem: to instantiate an instance of UIActivityItemProvider
	interface UIActivityItemProvider : UIActivityItemSource {
		[DesignatedInitializer]
		[Export ("initWithPlaceholderItem:")]
		[PostGet ("PlaceholderItem")]
		NativeHandle Constructor (NSObject placeholderItem);

		[Export ("placeholderItem", ArgumentSemantic.Retain)]
		NSObject PlaceholderItem { get; }

		[Export ("activityType")]
		NSString ActivityType { get; }

		[Export ("item")]
		NSObject Item { get; }

	}

	interface IUIActivityItemSource { }

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIActivityItemSource {
		[Abstract]
		[Export ("activityViewControllerPlaceholderItem:")]
		NSObject GetPlaceholderData (UIActivityViewController activityViewController);

		[Abstract]
		[Export ("activityViewController:itemForActivityType:")]
		NSObject GetItemForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType);

		[Export ("activityViewController:dataTypeIdentifierForActivityType:")]
		string GetDataTypeIdentifierForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType);

		[Export ("activityViewController:subjectForActivityType:")]
		string GetSubjectForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType);

		[Export ("activityViewController:thumbnailImageForActivityType:suggestedSize:")]
		UIImage GetThumbnailImageForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType, CGSize suggestedSize);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("activityViewControllerLinkMetadata:")]
		[return: NullAllowed]
		LPLinkMetadata GetLinkMetadata (UIActivityViewController activityViewController);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use initWithActivityItems:applicationActivities: to instantiate an instance of UIActivityViewController
	interface UIActivityViewController {
		[DesignatedInitializer]
		[Export ("initWithActivityItems:applicationActivities:")]
		NativeHandle Constructor (NSObject [] activityItems, [NullAllowed] UIActivity [] applicationActivities);

		[NullAllowed] // by default this property is null
		[Export ("completionHandler", ArgumentSemantic.Copy)]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use the 'CompletionWithItemsHandler' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'CompletionWithItemsHandler' property instead.")]
		Action<NSString, bool> CompletionHandler { get; set; }

		[Export ("excludedActivityTypes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSString [] ExcludedActivityTypes { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("completionWithItemsHandler", ArgumentSemantic.Copy)]
		UIActivityViewControllerCompletion CompletionWithItemsHandler { get; set; }

		[NoWatch, iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("allowsProminentActivity")]
		bool AllowsProminentActivity { get; set; }

		// UIActivityViewController (UIActivityItemsConfiguration) category

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithActivityItemsConfiguration:")]
		NativeHandle Constructor (IUIActivityItemsConfigurationReading activityItemsConfiguration);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UIAlertAction : NSCopying, UIAccessibilityIdentification {
		[Export ("title")]
		string Title { get; }

		[Export ("style")]
		UIAlertActionStyle Style { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Static, Export ("actionWithTitle:style:handler:")]
		UIAlertAction Create (string title, UIAlertActionStyle style, [NullAllowed] Action<UIAlertAction> handler);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	partial interface UIAlertController
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("actions")]
		UIAlertAction [] Actions { get; }

		[Export ("textFields")]
		UITextField [] TextFields { get; }

		[Export ("title", ArgumentSemantic.Copy), NullAllowed]
		string Title { get; set; }

		[Export ("message", ArgumentSemantic.Copy), NullAllowed]
		string Message { get; set; }

		[Export ("preferredStyle")]
		UIAlertControllerStyle PreferredStyle { get; }

		[Static, Export ("alertControllerWithTitle:message:preferredStyle:")]
		UIAlertController Create ([NullAllowed] string title, [NullAllowed] string message, UIAlertControllerStyle preferredStyle);

		[Export ("addAction:")]
		void AddAction (UIAlertAction action);

		[Export ("addTextFieldWithConfigurationHandler:")]
		void AddTextField (Action<UITextField> configurationHandler);

		[MacCatalyst (13, 1)]
		[Export ("preferredAction")]
		[NullAllowed]
		UIAlertAction PreferredAction { get; set; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("severity", ArgumentSemantic.Assign)]
		UIAlertControllerSeverity Severity { get; set; }
	}

	interface IUIAlertViewDelegate { }

	[NoTV, NoWatch]
	[BaseType (typeof (UIView), KeepRefUntil = "Dismissed", Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIAlertViewDelegate) })]
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIAlertController' with a 'UIAlertControllerStyle.Alert' type instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIAlertController' with a 'UIAlertControllerStyle.Alert' type instead.")]
	interface UIAlertView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Sealed]
		[Export ("initWithTitle:message:delegate:cancelButtonTitle:otherButtonTitles:", IsVariadic = true)]
		[Internal]
		[PostGet ("WeakDelegate")]
		// The native function takes a variable number of arguments (otherButtonTitles), terminated with a nil value.
		// Unfortunately iOS/ARM64 (not the general ARM64 ABI as published by ARM) has a different calling convention for varargs methods
		// than regular methods: all variable arguments are passed on the stack, no matter how many normal arguments there are.
		// Normally 8 arguments are passed in registers, then the subsequent ones are passed on the stack, so what we do is to provide
		// 9 arguments, where the 9th is nil (this is the 'mustAlsoBeNull' argument). Remember that Objective-C always has two hidden
		// arguments (id, SEL), which means we only need 7 more. And 'mustAlsoBeNull' is that 7th argument.
		// So on ARM64 the 8th argument ('mustBeNull') is ignored, and iOS sees the 9th argument ('mustAlsoBeNull') as the 8th argument.
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UIAlertController' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIAlertController' instead.")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] IUIAlertViewDelegate viewDelegate, [NullAllowed] string cancelButtonTitle, IntPtr otherButtonTitles, IntPtr mustBeNull, IntPtr mustAlsoBeNull);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIAlertViewDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("message", ArgumentSemantic.Copy)]
		string Message { get; set; }

		[Export ("addButtonWithTitle:")]
		nint AddButton ([NullAllowed] string title);

		[Export ("buttonTitleAtIndex:")]
		string ButtonTitle (nint index);

		[Export ("numberOfButtons")]
		nint ButtonCount { get; }

		[Export ("cancelButtonIndex")]
		nint CancelButtonIndex { get; set; }

		[Export ("firstOtherButtonIndex")]
		nint FirstOtherButtonIndex { get; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }

		[Export ("show")]
		void Show ();

		[Export ("dismissWithClickedButtonIndex:animated:")]
		void DismissWithClickedButtonIndex (nint index, bool animated);

		[Export ("alertViewStyle", ArgumentSemantic.Assign)]
		UIAlertViewStyle AlertViewStyle { get; set; }

		[Export ("textFieldAtIndex:")]
		UITextField GetTextField (nint textFieldIndex);
	}

	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	interface UIAlertViewDelegate {
		[Export ("alertView:clickedButtonAtIndex:"), EventArgs ("UIButton")]
		void Clicked (UIAlertView alertview, nint buttonIndex);

		[Export ("alertViewCancel:"), EventArgs ("UIAlertView")]
		void Canceled (UIAlertView alertView);

		[Export ("willPresentAlertView:"), EventArgs ("UIAlertView")]
		void WillPresent (UIAlertView alertView);

		[Export ("didPresentAlertView:"), EventArgs ("UIAlertView")]
		void Presented (UIAlertView alertView);

		[Export ("alertView:willDismissWithButtonIndex:"), EventArgs ("UIButton")]
		void WillDismiss (UIAlertView alertView, nint buttonIndex);

		[Export ("alertView:didDismissWithButtonIndex:"), EventArgs ("UIButton")]
		void Dismissed (UIAlertView alertView, nint buttonIndex);

		[Export ("alertViewShouldEnableFirstOtherButton:"), DelegateName ("UIAlertViewPredicate"), DefaultValue (true)]
		bool ShouldEnableFirstOtherButton (UIAlertView alertView);
	}

	//
	// This is an empty class, we manually bind the couple of static methods
	// but it is used as a flag on handful of classes to generate the
	// UIAppearance binding.
	//
	// When a new class adopts UIAppearance, merely list it as one of the
	// base interfaces, this will generate the stubs for it.
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[DisableDefaultCtor]
	[Protocol]
	interface UIAppearance {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIStackView {
		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithArrangedSubviews:")]
		NativeHandle Constructor (UIView [] views);

		[Export ("arrangedSubviews")]
		UIView [] ArrangedSubviews { get; }

		[Export ("axis")]
		UILayoutConstraintAxis Axis { get; set; }

		[Export ("distribution")]
		UIStackViewDistribution Distribution { get; set; }

		[Export ("alignment")]
		UIStackViewAlignment Alignment { get; set; }

		[Export ("spacing")]
		nfloat Spacing { get; set; }

		[Export ("baselineRelativeArrangement")]
		bool BaselineRelativeArrangement { [Bind ("isBaselineRelativeArrangement")] get; set; }

		[Export ("layoutMarginsRelativeArrangement")]
		bool LayoutMarginsRelativeArrangement { [Bind ("isLayoutMarginsRelativeArrangement")] get; set; }

		[Export ("addArrangedSubview:")]
		void AddArrangedSubview (UIView view);

		[Export ("removeArrangedSubview:")]
		void RemoveArrangedSubview (UIView view);

		[Export ("insertArrangedSubview:atIndex:")]
		void InsertArrangedSubview (UIView view, nuint stackIndex);

		[MacCatalyst (13, 1)]
		[Export ("setCustomSpacing:afterView:")]
		void SetCustomSpacing (nfloat spacing, UIView arrangedSubview);

		[MacCatalyst (13, 1)]
		[Export ("customSpacingAfterView:")]
		nfloat GetCustomSpacing (UIView arrangedSubview);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIStateRestoration {
		[Field ("UIStateRestorationViewControllerStoryboardKey")]
		NSString ViewControllerStoryboardKey { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIStateRestoring {
		[Export ("restorationParent")]
		IUIStateRestoring RestorationParent { get; }

		[Export ("objectRestorationClass")]
		Class ObjectRestorationClass { get; }

		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[Export ("applicationFinishedRestoringState")]
		void ApplicationFinishedRestoringState ();
	}

	interface IUIStateRestoring { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIObjectRestoration {
#if false
		// a bit hard to support *static* as part of an interface / extension methods
		[Static][Export ("objectWithRestorationIdentifierPath:coder:")]
		IUIStateRestoring GetStateRestorationObjectFromPath (NSString [] identifierComponents, NSCoder coder);
#endif
	}

	interface IUIViewAnimating { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIViewAnimating {
		[Abstract]
		[Export ("state")]
		UIViewAnimatingState State { get; }

		[Abstract]
		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[Abstract]
		[Export ("reversed")]
		bool Reversed { [Bind ("isReversed")] get; set; }

		[Abstract]
		[Export ("fractionComplete")]
		nfloat FractionComplete { get; set; }

		[Abstract]
		[Export ("startAnimation")]
		void StartAnimation ();

		[Abstract]
		[Export ("startAnimationAfterDelay:")]
		void StartAnimation (double delay);

		[Abstract]
		[Export ("pauseAnimation")]
		void PauseAnimation ();

		[Abstract]
		[Export ("stopAnimation:")]
		void StopAnimation (bool withoutFinishing);

		[Abstract]
		[Export ("finishAnimationAtPosition:")]
		void FinishAnimation (UIViewAnimatingPosition finalPosition);
	}

	interface IUIViewImplicitlyAnimating { }
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIViewImplicitlyAnimating : UIViewAnimating {
		[Export ("addAnimations:delayFactor:")]
		void AddAnimations (Action animation, nfloat delayFactor);

		[Export ("addAnimations:")]
		void AddAnimations (Action animation);

		[Export ("addCompletion:")]
		void AddCompletion (Action<UIViewAnimatingPosition> completion);

		[Export ("continueAnimationWithTimingParameters:durationFactor:")]
		void ContinueAnimation ([NullAllowed] IUITimingCurveProvider parameters, nfloat durationFactor);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIViewPropertyAnimator : UIViewImplicitlyAnimating, NSCopying {
		[NullAllowed, Export ("timingParameters", ArgumentSemantic.Copy)]
		IUITimingCurveProvider TimingParameters { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("delay")]
		double Delay { get; }

		[Export ("userInteractionEnabled")]
		bool UserInteractionEnabled { [Bind ("isUserInteractionEnabled")] get; set; }

		[Export ("manualHitTestingEnabled")]
		bool ManualHitTestingEnabled { [Bind ("isManualHitTestingEnabled")] get; set; }

		[Export ("interruptible")]
		bool Interruptible { [Bind ("isInterruptible")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("scrubsLinearly")]
		bool ScrubsLinearly { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("pausesOnCompletion")]
		bool PausesOnCompletion { get; set; }

		[Export ("initWithDuration:timingParameters:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double duration, IUITimingCurveProvider parameters);

		[Export ("initWithDuration:curve:animations:")]
		NativeHandle Constructor (double duration, UIViewAnimationCurve curve, [NullAllowed] Action animations);

		[Export ("initWithDuration:controlPoint1:controlPoint2:animations:")]
		NativeHandle Constructor (double duration, CGPoint point1, CGPoint point2, [NullAllowed] Action animations);

		[Export ("initWithDuration:dampingRatio:animations:")]
		NativeHandle Constructor (double duration, nfloat ratio, [NullAllowed] Action animations);

		[Static]
		[Export ("runningPropertyAnimatorWithDuration:delay:options:animations:completion:")]
		UIViewPropertyAnimator CreateRunningPropertyAnimator (double duration, double delay, UIViewAnimationOptions options, Action animations, [NullAllowed] Action<UIViewAnimatingPosition> completion);
	}

	interface IUIViewControllerPreviewing { }

	[Protocol]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIViewControllerPreviewing {

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("previewingGestureRecognizerForFailureRelationship")]
		UIGestureRecognizer PreviewingGestureRecognizerForFailureRelationship { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate {
			get; // readonly
#if !XAMCORE_3_0
			[NotImplemented]
			set;
#endif
		}

		[Wrap ("WeakDelegate")]
		IUIViewControllerPreviewingDelegate Delegate { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("sourceView")]
		UIView SourceView { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("sourceRect")]
		CGRect SourceRect { get; set; }
	}

	interface IUIViewControllerPreviewingDelegate { }

	[Protocol]
	[Model]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIViewControllerPreviewingDelegate {
		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("previewingContext:viewControllerForLocation:")]
		UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Abstract]
		[Export ("previewingContext:commitViewController:")]
		void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIViewControllerRestoration {
#if false
		/* we don't generate anything for static members in protocols now, so just keep this out */
		[Static]
		[Export ("viewControllerWithRestorationIdentifierPath:coder:")]
		UIViewController GetStateRestorationViewController (NSString [] identifierComponents, NSCoder coder);
#endif
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIStatusBarFrameChangeEventArgs {
		[Export ("UIApplicationStatusBarFrameUserInfoKey")]
		CGRect StatusBarFrame { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIStatusBarOrientationChangeEventArgs {
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("UIApplicationStatusBarOrientationUserInfoKey")]
		UIInterfaceOrientation StatusBarOrientation { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIApplicationLaunchEventArgs {
		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsURLKey")]
		NSUrl Url { get; }

		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsSourceApplicationKey")]
		string SourceApplication { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsRemoteNotificationKey")]
		NSDictionary RemoteNotifications { get; }

		[ProbePresence]
		[Export ("UIApplicationLaunchOptionsLocationKey")]
		bool LocationLaunch { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("UIApplicationOpenUrlOptionKeys")]
	interface UIApplicationOpenUrlOptions {
		NSObject Annotation { get; set; }
		string SourceApplication { get; set; }
		bool OpenInPlace { get; set; }

		[MacCatalyst (13, 1)]
		bool UniversalLinksOnly { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal] // we'll make it public if there's a need for them (beside the strong dictionary we provide)
	interface UIApplicationOpenUrlOptionKeys {
		[Field ("UIApplicationOpenURLOptionsAnnotationKey")]
		NSString AnnotationKey { get; }

		[Field ("UIApplicationOpenURLOptionsSourceApplicationKey")]
		NSString SourceApplicationKey { get; }

		[Field ("UIApplicationOpenURLOptionsOpenInPlaceKey")]
		NSString OpenInPlaceKey { get; }

		[NoWatch, NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("UIApplicationOpenURLOptionsEventAttributionKey")]
		NSString OpenUrlOptionsEventAttributionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIApplicationOpenURLOptionUniversalLinksOnly")]
		NSString UniversalLinksOnlyKey { get; }

		[NoWatch, NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("UIApplicationOpenExternalURLOptionsEventAttributionKey")]
		NSString OpenExternalUrlOptionsEventAttributionKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIResponder))]
	interface UIApplication {
		[Static, ThreadSafe]
		[Export ("sharedApplication")]
		UIApplication SharedApplication { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[ThreadSafe, NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIApplicationDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Export ("beginIgnoringInteractionEvents")]
		void BeginIgnoringInteractionEvents ();

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Export ("endIgnoringInteractionEvents")]
		void EndIgnoringInteractionEvents ();

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use UIView's 'UserInteractionEnabled' property instead.")]
		[Export ("isIgnoringInteractionEvents")]
		bool IsIgnoringInteractionEvents { get; }

		[Export ("idleTimerDisabled")]
		bool IdleTimerDisabled { [Bind ("isIdleTimerDisabled")] get; set; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Please use the overload instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Please use the overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the overload instead.")]
		[Export ("openURL:")]
		bool OpenUrl (NSUrl url);

		[MacCatalyst (13, 1)]
		[Export ("openURL:options:completionHandler:")]
		void OpenUrl (NSUrl url, NSDictionary options, [NullAllowed] Action<bool> completion);

		[MacCatalyst (13, 1)]
		[Wrap ("OpenUrl (url, options.GetDictionary ()!, completion)")]
		[Async]
		void OpenUrl (NSUrl url, UIApplicationOpenUrlOptions options, [NullAllowed] Action<bool> completion);

		[Export ("canOpenURL:")]
		[PreSnippet ("if (url is null) return false;", Optimizable = true)] // null not really allowed (but it's a behaviour change with known bug reports)
		bool CanOpenUrl ([NullAllowed] NSUrl url);

		[Export ("sendEvent:")]
		void SendEvent (UIEvent uievent);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Should not be used for applications that support multiple scenes because it returns a key window across all connected scenes.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Should not be used for applications that support multiple scenes because it returns a key window across all connected scenes.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Should not be used for applications that support multiple scenes because it returns a key window across all connected scenes.")]
		[NullAllowed]
		[Export ("keyWindow")]
		[Transient]
		UIWindow KeyWindow { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'UIWindowScene.Windows' in the desired window scene object instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'UIWindowScene.Windows' in the desired window scene object instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'UIWindowScene.Windows' in the desired window scene object instead.")]
		[Export ("windows")]
		[Transient]
		UIWindow [] Windows { get; }

		[Export ("sendAction:to:from:forEvent:")]
		bool SendAction (Selector action, [NullAllowed] NSObject target, [NullAllowed] NSObject sender, [NullAllowed] UIEvent forEvent);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Provide a custom UI in your app instead if needed.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Provide a custom UI in your app instead if needed.")]
		[Export ("networkActivityIndicatorVisible"), ThreadSafe]
		bool NetworkActivityIndicatorVisible { [Bind ("isNetworkActivityIndicatorVisible")] get; set; }

		[NoTV]
		[Export ("statusBarStyle")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIViewController.PreferredStatusBarStyle' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIViewController.PreferredStatusBarStyle' instead.")]
		UIStatusBarStyle StatusBarStyle { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIViewController.PreferredStatusBarStyle' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIViewController.PreferredStatusBarStyle' instead.")]
		[Export ("setStatusBarStyle:animated:")]
		void SetStatusBarStyle (UIStatusBarStyle statusBarStyle, bool animated);

		[NoTV]
		[Export ("statusBarHidden")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIViewController.PrefersStatusBarHidden' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIViewController.PrefersStatusBarHidden' instead.")]
		bool StatusBarHidden { [Bind ("isStatusBarHidden")] get; set; }

		[NoTV]
		[Export ("setStatusBarHidden:withAnimation:")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIViewController.PrefersStatusBarHidden' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIViewController.PrefersStatusBarHidden' instead.")]
		void SetStatusBarHidden (bool state, UIStatusBarAnimation animation);

		[NoTV]
		[Export ("setStatusBarHidden:animated:")]
		[Deprecated (PlatformName.iOS, 3, 2, message: "Use 'SetStatusBarHidden (bool, UIStatusBarAnimation)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetStatusBarHidden (bool, UIStatusBarAnimation)' instead.")]
		void SetStatusBarHidden (bool hidden, bool animated);

		[NoTV]
		[Export ("statusBarOrientation")]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		UIInterfaceOrientation StatusBarOrientation { get; set; }

		[NoTV]
		[Export ("setStatusBarOrientation:animated:")]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void SetStatusBarOrientation (UIInterfaceOrientation orientation, bool animated);

		[NoTV]
		[Export ("statusBarOrientationAnimationDuration")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'InterfaceOrientation' property of the window scene instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'InterfaceOrientation' property of the window scene instead.")]
		double StatusBarOrientationAnimationDuration { get; }

		[NoTV]
		[Export ("statusBarFrame")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the 'StatusBarManager' property of the window scene instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'StatusBarManager' property of the window scene instead.")]
		CGRect StatusBarFrame { get; }

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use 'UNUserNotificationCenter.SetBadge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use 'UNUserNotificationCenter.SetBadge' instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Use 'UNUserNotificationCenter.SetBadge' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("applicationIconBadgeNumber")]
		nint ApplicationIconBadgeNumber { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("applicationSupportsShakeToEdit")]
		bool ApplicationSupportsShakeToEdit { get; set; }

		// From @interface UIApplication (UIRemoteNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'RegisterUserNotifications', 'RegisterForNotifications'  or 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RegisterUserNotifications', 'RegisterForNotifications'  or 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[Export ("registerForRemoteNotificationTypes:")]
		void RegisterForRemoteNotificationTypes (UIRemoteNotificationType types);

		// From @interface UIApplication (UIRemoteNotifications)
		[Export ("unregisterForRemoteNotifications")]
		void UnregisterForRemoteNotifications ();

		// From @interface UIApplication (UIRemoteNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'CurrentUserNotificationSettings' or 'UNUserNotificationCenter.GetNotificationSettings' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CurrentUserNotificationSettings' or 'UNUserNotificationCenter.GetNotificationSettings' instead.")]
		[Export ("enabledRemoteNotificationTypes")]
		UIRemoteNotificationType EnabledRemoteNotificationTypes { get; }

		[Field ("UIApplicationDidFinishLaunchingNotification")]
		[Notification (typeof (UIApplicationLaunchEventArgs))]
		NSString DidFinishLaunchingNotification { get; }

		[Field ("UIApplicationDidBecomeActiveNotification")]
		[Notification]
		NSString DidBecomeActiveNotification { get; }

		[Field ("UIApplicationWillResignActiveNotification")]
		[Notification]
		NSString WillResignActiveNotification { get; }

		[Field ("UIApplicationDidReceiveMemoryWarningNotification")]
		[Notification]
		NSString DidReceiveMemoryWarningNotification { get; }

		[Field ("UIApplicationWillTerminateNotification")]
		[Notification]
		NSString WillTerminateNotification { get; }

		[Field ("UIApplicationSignificantTimeChangeNotification")]
		[Notification]
		NSString SignificantTimeChangeNotification { get; }

		[NoTV]
		[Field ("UIApplicationWillChangeStatusBarOrientationNotification")]
		[Notification (typeof (UIStatusBarOrientationChangeEventArgs))]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		NSString WillChangeStatusBarOrientationNotification { get; }

		[NoTV]
		[Field ("UIApplicationDidChangeStatusBarOrientationNotification")]
		[Notification (typeof (UIStatusBarOrientationChangeEventArgs))]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		NSString DidChangeStatusBarOrientationNotification { get; }

		[NoTV]
		[Field ("UIApplicationStatusBarOrientationUserInfoKey")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		NSString StatusBarOrientationUserInfoKey { get; }

		[NoTV]
		[Field ("UIApplicationWillChangeStatusBarFrameNotification")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		[Notification (typeof (UIStatusBarFrameChangeEventArgs))]
		NSString WillChangeStatusBarFrameNotification { get; }

		[NoTV]
		[Field ("UIApplicationDidChangeStatusBarFrameNotification")]
		[Notification (typeof (UIStatusBarFrameChangeEventArgs))]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		NSString DidChangeStatusBarFrameNotification { get; }

		[NoTV]
		[Field ("UIApplicationStatusBarFrameUserInfoKey")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		NSString StatusBarFrameUserInfoKey { get; }

		[Field ("UIApplicationLaunchOptionsURLKey")]
		NSString LaunchOptionsUrlKey { get; }

		[Field ("UIApplicationLaunchOptionsSourceApplicationKey")]
		NSString LaunchOptionsSourceApplicationKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsRemoteNotificationKey")]
		NSString LaunchOptionsRemoteNotificationKey { get; }

		[Field ("UIApplicationLaunchOptionsAnnotationKey")]
		NSString LaunchOptionsAnnotationKey { get; }

		[Export ("applicationState")]
		UIApplicationState ApplicationState { get; }

		[ThreadSafe]
		[Export ("backgroundTimeRemaining")]
		double BackgroundTimeRemaining { get; }

		[ThreadSafe]
		[RequiresSuper]
		[Export ("beginBackgroundTaskWithExpirationHandler:")]
		nint BeginBackgroundTask ([NullAllowed] Action backgroundTimeExpired);

		[ThreadSafe]
		[RequiresSuper]
		[Export ("endBackgroundTask:")]
		void EndBackgroundTask (nint taskId);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PushKit' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PushKit' instead.")]
		[Export ("setKeepAliveTimeout:handler:")]
		bool SetKeepAliveTimeout (double timeout, [NullAllowed] Action handler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PushKit' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PushKit' instead.")]
		[Export ("clearKeepAliveTimeout")]
		void ClearKeepAliveTimeout ();

		[Export ("protectedDataAvailable")]
		bool ProtectedDataAvailable { [Bind ("isProtectedDataAvailable")] get; }

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[Export ("presentLocalNotificationNow:")]
		void PresentLocalNotificationNow (UILocalNotification notification);

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[Export ("scheduleLocalNotification:")]
		void ScheduleLocalNotification (UILocalNotification notification);

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RemovePendingNotificationRequests' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.RemovePendingNotificationRequests' instead.")]
		[Export ("cancelLocalNotification:")]
		void CancelLocalNotification (UILocalNotification notification);

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RemoveAllPendingNotificationRequests' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.RemoveAllPendingNotificationRequests' instead.")]
		[Export ("cancelAllLocalNotifications")]
		void CancelAllLocalNotifications ();

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.GetPendingNotificationRequests' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.GetPendingNotificationRequests' instead.")]
		[Export ("scheduledLocalNotifications", ArgumentSemantic.Copy)]
		UILocalNotification [] ScheduledLocalNotifications { get; set; }

		// from @interface UIApplication (UIRemoteControlEvents)
		[Export ("beginReceivingRemoteControlEvents")]
		void BeginReceivingRemoteControlEvents ();

		// from @interface UIApplication (UIRemoteControlEvents)
		[Export ("endReceivingRemoteControlEvents")]
		void EndReceivingRemoteControlEvents ();

		[Field ("UIBackgroundTaskInvalid")]
		nint BackgroundTaskInvalid { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'PushKit' for Voice Over IP applications.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'PushKit' for Voice Over IP applications.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PushKit' for Voice Over IP applications.")]
		[Field ("UIMinimumKeepAliveTimeout")]
		double /* NSTimeInternal */ MinimumKeepAliveTimeout { get; }

		[Field ("UIApplicationProtectedDataWillBecomeUnavailable")]
		[Notification]
		NSString ProtectedDataWillBecomeUnavailable { get; }

		[Field ("UIApplicationProtectedDataDidBecomeAvailable")]
		[Notification]
		NSString ProtectedDataDidBecomeAvailable { get; }

		[Field ("UIApplicationLaunchOptionsLocationKey")]
		NSString LaunchOptionsLocationKey { get; }

		[Field ("UIApplicationDidEnterBackgroundNotification")]
		[Notification]
		NSString DidEnterBackgroundNotification { get; }

		[Field ("UIApplicationWillEnterForegroundNotification")]
		[Notification]
		NSString WillEnterForegroundNotification { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Field ("UIApplicationLaunchOptionsLocalNotificationKey")]
		NSString LaunchOptionsLocalNotificationKey { get; }

		[Export ("userInterfaceLayoutDirection")]
		UIUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; }

		// from @interface UIApplication (UINewsstand)
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("setNewsstandIconImage:")]
		void SetNewsstandIconImage ([NullAllowed] UIImage image);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsNewsstandDownloadsKey")]
		NSString LaunchOptionsNewsstandDownloadsKey { get; }

		[Field ("UIApplicationLaunchOptionsBluetoothCentralsKey")]
		NSString LaunchOptionsBluetoothCentralsKey { get; }

		[Field ("UIApplicationLaunchOptionsBluetoothPeripheralsKey")]
		NSString LaunchOptionsBluetoothPeripheralsKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsShortcutItemKey")]
		NSString LaunchOptionsShortcutItemKey { get; }

		[NoWatch, NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("UIApplicationLaunchOptionsEventAttributionKey")]
		NSString LaunchOptionsEventAttributionKey { get; }

		//
		// 6.0
		//
		// from @interface UIApplication (UIStateRestoration)
		[Export ("extendStateRestoration")]
		void ExtendStateRestoration ();

		// from @interface UIApplication (UIStateRestoration)
		[Export ("completeStateRestoration")]
		void CompleteStateRestoration ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("supportedInterfaceOrientationsForWindow:")]
		UIInterfaceOrientationMask SupportedInterfaceOrientationsForWindow ([Transient] UIWindow window);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("UITrackingRunLoopMode")]
		NSString UITrackingRunLoopMode { get; }

		[Field ("UIApplicationStateRestorationBundleVersionKey")]
		NSString StateRestorationBundleVersionKey { get; }

		[Field ("UIApplicationStateRestorationUserInterfaceIdiomKey")]
		NSString StateRestorationUserInterfaceIdiomKey { get; }

		//
		// 7.0
		//
		[Field ("UIContentSizeCategoryDidChangeNotification")]
		[Notification (typeof (UIContentSizeCategoryChangedEventArgs))]
		NSString ContentSizeCategoryChangedNotification { get; }

		[ThreadSafe]
		[RequiresSuper]
		[Export ("beginBackgroundTaskWithName:expirationHandler:")]
		nint BeginBackgroundTask (string taskName, Action expirationHandler);

		[MacCatalyst (13, 1)]
		[Field ("UIApplicationBackgroundFetchIntervalMinimum")]
		double BackgroundFetchIntervalMinimum { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIApplicationBackgroundFetchIntervalNever")]
		double BackgroundFetchIntervalNever { get; }

		[Export ("setMinimumBackgroundFetchInterval:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		void SetMinimumBackgroundFetchInterval (double minimumBackgroundFetchInterval);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("preferredContentSizeCategory")]
		NSString PreferredContentSizeCategory { get; }

		[Wrap ("UIContentSizeCategoryExtensions.GetValue (SharedApplication.PreferredContentSizeCategory)")]
		UIContentSizeCategory GetPreferredContentSizeCategory ();

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("connectedScenes")]
		NSSet<UIScene> ConnectedScenes { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("openSessions")]
		NSSet<UISceneSession> OpenSessions { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportsMultipleScenes")]
		bool SupportsMultipleScenes { get; }

		[Deprecated (PlatformName.iOS, 17, 0, message: "Please use 'ActivateSceneSession' instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Please use 'ActivateSceneSession' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Please use 'ActivateSceneSession' instead.")]
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestSceneSessionActivation:userActivity:options:errorHandler:")]
		void RequestSceneSessionActivation ([NullAllowed] UISceneSession sceneSession, [NullAllowed] NSUserActivity userActivity, [NullAllowed] UISceneActivationRequestOptions options, [NullAllowed] Action<NSError> errorHandler);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestSceneSessionDestruction:options:errorHandler:")]
		void RequestSceneSessionDestruction (UISceneSession sceneSession, [NullAllowed] UISceneDestructionRequestOptions options, [NullAllowed] Action<NSError> errorHandler);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestSceneSessionRefresh:")]
		void RequestSceneSessionRefresh (UISceneSession sceneSession);

		// from @interface UIApplication (UIStateRestoration)
		[Export ("ignoreSnapshotOnNextApplicationLaunch")]
		void IgnoreSnapshotOnNextApplicationLaunch ();

		// from @interface UIApplication (UIStateRestoration)
		[Export ("registerObjectForStateRestoration:restorationIdentifier:")]
		[Static]
		void RegisterObjectForStateRestoration (IUIStateRestoring uistateRestoringObject, string restorationIdentifier);

		[Field ("UIApplicationStateRestorationTimestampKey")]
		NSString StateRestorationTimestampKey { get; }

		[Field ("UIApplicationStateRestorationSystemVersionKey")]
		NSString StateRestorationSystemVersionKey { get; }

		[MacCatalyst (13, 1)]
		[Export ("backgroundRefreshStatus")]
		UIBackgroundRefreshStatus BackgroundRefreshStatus { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIApplicationBackgroundRefreshStatusDidChangeNotification")]
		NSString BackgroundRefreshStatusDidChangeNotification { get; }

		[Notification]
		[Field ("UIApplicationUserDidTakeScreenshotNotification")]
		NSString UserDidTakeScreenshotNotification { get; }

		// 
		// 8.0
		//
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationOpenSettingsURLString")]
		NSString OpenSettingsUrlString { get; }

		[iOS (15, 4), MacCatalyst (15, 4), TV (15, 4), NoWatch]
		[Field ("UIApplicationOpenNotificationSettingsURLString")]
		NSString OpenNotificationSettingsUrl { get; }

		// from @interface UIApplication (UIUserNotificationSettings)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.GetNotificationSettings' and 'UNUserNotificationCenter.GetNotificationCategories' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.GetNotificationSettings' and 'UNUserNotificationCenter.GetNotificationCategories' instead.")]
		[Export ("currentUserNotificationSettings")]
		UIUserNotificationSettings CurrentUserNotificationSettings { get; }

		// from @interface UIApplication (UIRemoteNotifications)
		[MacCatalyst (13, 1)]
		[Export ("isRegisteredForRemoteNotifications")]
		bool IsRegisteredForRemoteNotifications { get; }

		// from @interface UIApplication (UIRemoteNotifications)
		[MacCatalyst (13, 1)]
		[Export ("registerForRemoteNotifications")]
		void RegisterForRemoteNotifications ();

		// from @interface UIApplication (UIUserNotificationSettings)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RequestAuthorization' and 'UNUserNotificationCenter.SetNotificationCategories' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.RequestAuthorization' and 'UNUserNotificationCenter.SetNotificationCategories' instead.")]
		[Export ("registerUserNotificationSettings:")]
		void RegisterUserNotificationSettings (UIUserNotificationSettings notificationSettings);

		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsUserActivityDictionaryKey")]
		NSString LaunchOptionsUserActivityDictionaryKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsUserActivityTypeKey")]
		NSString LaunchOptionsUserActivityTypeKey { get; }

		[NoTV, NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationLaunchOptionsCloudKitShareMetadataKey")]
		NSString LaunchOptionsCloudKitShareMetadataKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("shortcutItems", ArgumentSemantic.Copy)]
		UIApplicationShortcutItem [] ShortcutItems { get; set; }

		//
		// 10.0
		//
		// from @interface UIApplication (UIAlternateApplicationIcons)

		[MacCatalyst (13, 1)]
		[Export ("supportsAlternateIcons")]
		bool SupportsAlternateIcons { get; }

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("setAlternateIconName:completionHandler:")]
		void SetAlternateIconName ([NullAllowed] string alternateIconName, [NullAllowed] Action<NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("alternateIconName"), NullAllowed]
		string AlternateIconName { get; }

		[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("activateSceneSessionForRequest:errorHandler:")]
		void ActivateSceneSession (UISceneSessionActivationRequest request, [NullAllowed] Action<NSError> errorHandler);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIApplicationShortcutIcon : NSCopying {
		[Static]
		[Export ("iconWithType:")]
		UIApplicationShortcutIcon FromType (UIApplicationShortcutIconType type);

		[Static]
		[Export ("iconWithTemplateImageName:")]
		UIApplicationShortcutIcon FromTemplateImageName (string templateImageName);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("iconWithSystemImageName:")]
		UIApplicationShortcutIcon FromSystemImageName (string systemImageName);

		// This is inside ContactsUI.framework
		[NoMac]
		[NoTV]
		[NoWatch]
		[NoMacCatalyst]
		[Static, Export ("iconWithContact:")]
		UIApplicationShortcutIcon FromContact (CNContact contact);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIApplicationShortcutItem : NSMutableCopying {
		[Export ("initWithType:localizedTitle:localizedSubtitle:icon:userInfo:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string type, string localizedTitle, [NullAllowed] string localizedSubtitle, [NullAllowed] UIApplicationShortcutIcon icon, [NullAllowed] NSDictionary<NSString, NSObject> userInfo);

		[Export ("initWithType:localizedTitle:")]
		NativeHandle Constructor (string type, string localizedTitle);

		[Export ("type")]
		string Type { get; [NotImplemented] set; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; [NotImplemented] set; }

		[NullAllowed, Export ("localizedSubtitle")]
		string LocalizedSubtitle { get; [NotImplemented] set; }

		[NullAllowed, Export ("icon", ArgumentSemantic.Copy)]
		UIApplicationShortcutIcon Icon { get; [NotImplemented] set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; [NotImplemented] set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier", ArgumentSemantic.Copy)]
		NSObject TargetContentIdentifier { get; [NotImplemented] set; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIApplicationShortcutItem))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIApplicationShortcutItem init].
	interface UIMutableApplicationShortcutItem {
		// inlined
		[Export ("initWithType:localizedTitle:localizedSubtitle:icon:userInfo:")]
		NativeHandle Constructor (string type, string localizedTitle, [NullAllowed] string localizedSubtitle, [NullAllowed] UIApplicationShortcutIcon icon, [NullAllowed] NSDictionary<NSString, NSObject> userInfo);

		// inlined
		[Export ("initWithType:localizedTitle:")]
		NativeHandle Constructor (string type, string localizedTitle);

		[Export ("type")]
		[Override]
		string Type { get; set; }

		[Export ("localizedTitle")]
		[Override]
		string LocalizedTitle { get; set; }

		[NullAllowed, Export ("localizedSubtitle")]
		[Override]
		string LocalizedSubtitle { get; set; }

		[NullAllowed, Export ("icon", ArgumentSemantic.Copy)]
		[Override]
		UIApplicationShortcutIcon Icon { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		[Override]
		NSDictionary<NSString, NSObject> UserInfo { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier", ArgumentSemantic.Copy)]
		NSObject TargetContentIdentifier { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: init is undefined for objects of type UIAttachmentBehavior
	interface UIAttachmentBehavior {
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("attachedBehaviorType")]
		UIAttachmentBehaviorType AttachedBehaviorType { get; }

		[Export ("anchorPoint")]
		CGPoint AnchorPoint { get; set; }

		[Export ("length")]
		nfloat Length { get; set; }

		[Export ("damping")]
		nfloat Damping { get; set; }

		[Export ("frequency")]
		nfloat Frequency { get; set; }

		[Export ("initWithItem:attachedToAnchor:")]
		NativeHandle Constructor (IUIDynamicItem item, CGPoint anchorPoint);

		[DesignatedInitializer]
		[Export ("initWithItem:offsetFromCenter:attachedToAnchor:")]
		NativeHandle Constructor (IUIDynamicItem item, UIOffset offset, CGPoint anchorPoint);

		[Export ("initWithItem:attachedToItem:")]
		NativeHandle Constructor (IUIDynamicItem item, IUIDynamicItem attachedToItem);

		[DesignatedInitializer]
		[Export ("initWithItem:offsetFromCenter:attachedToItem:offsetFromCenter:")]
		NativeHandle Constructor (IUIDynamicItem item, UIOffset offsetFromCenter, IUIDynamicItem attachedToItem, UIOffset attachOffsetFromCenter);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("slidingAttachmentWithItem:attachedToItem:attachmentAnchor:axisOfTranslation:")]
		UIAttachmentBehavior CreateSlidingAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor, CGVector translationAxis);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("slidingAttachmentWithItem:attachmentAnchor:axisOfTranslation:")]
		UIAttachmentBehavior CreateSlidingAttachment (IUIDynamicItem item, CGPoint attachmentAnchor, CGVector translationAxis);

		// +(instancetype __nonnull)limitAttachmentWithItem:(id<UIDynamicItem> __nonnull)item1 offsetFromCenter:(UIOffset)offset1 attachedToItem:(id<UIDynamicItem> __nonnull)item2 offsetFromCenter:(UIOffset)offset2;
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("limitAttachmentWithItem:offsetFromCenter:attachedToItem:offsetFromCenter:")]
		UIAttachmentBehavior CreateLimitAttachment (IUIDynamicItem item1, UIOffset offsetFromCenter, IUIDynamicItem item2, UIOffset offsetFromCenter2);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("fixedAttachmentWithItem:attachedToItem:attachmentAnchor:")]
		UIAttachmentBehavior CreateFixedAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor);

		[Static]
		[MacCatalyst (13, 1)]
		[Export ("pinAttachmentWithItem:attachedToItem:attachmentAnchor:")]
		UIAttachmentBehavior CreatePinAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor);

		[Export ("attachmentRange")]
		[MacCatalyst (13, 1)]
		UIFloatRange AttachmentRange { get; set; }

		[Export ("frictionTorque")]
		[MacCatalyst (13, 1)]
		nfloat FrictionTorque { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIContentSizeCategoryAdjusting {
		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("adjustsFontForContentSizeCategory")]
		bool AdjustsFontForContentSizeCategory { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIContentSizeCategoryChangedEventArgs {
		[Export ("UIContentSizeCategoryNewValueKey")]
		NSString WeakNewValue { get; }
	}

	[Static]
	[NoWatch]
	[MacCatalyst (13, 1)]
	public enum UIContentSizeCategory {
		[MacCatalyst (13, 1)]
		[Field ("UIContentSizeCategoryUnspecified")]
		Unspecified,

		[Field ("UIContentSizeCategoryExtraSmall")]
		ExtraSmall,

		[Field ("UIContentSizeCategorySmall")]
		Small,

		[Field ("UIContentSizeCategoryMedium")]
		Medium,

		[Field ("UIContentSizeCategoryLarge")]
		Large,

		[Field ("UIContentSizeCategoryExtraLarge")]
		ExtraLarge,

		[Field ("UIContentSizeCategoryExtraExtraLarge")]
		ExtraExtraLarge,

		[Field ("UIContentSizeCategoryExtraExtraExtraLarge")]
		ExtraExtraExtraLarge,

		[Field ("UIContentSizeCategoryAccessibilityMedium")]
		AccessibilityMedium,

		[Field ("UIContentSizeCategoryAccessibilityLarge")]
		AccessibilityLarge,

		[Field ("UIContentSizeCategoryAccessibilityExtraLarge")]
		AccessibilityExtraLarge,

		[Field ("UIContentSizeCategoryAccessibilityExtraExtraLarge")]
		AccessibilityExtraExtraLarge,

		[Field ("UIContentSizeCategoryAccessibilityExtraExtraExtraLarge")]
		AccessibilityExtraExtraExtraLarge
	}

	[iOS (16, 0), MacCatalyst (16, 0), TV (16, 0), NoWatch]
	[Native]
	public enum UIAlertControllerSeverity : long {
		Default = 0,
		Critical,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UICalendarViewDecorationSize : long {
		Small = 0,
		Medium = 1,
		Large = 2,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UICollectionViewSelfSizingInvalidation : long {
		Disabled,
		Enabled,
		EnabledIncludingConstraints,
	}

	[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIContextMenuConfigurationElementOrder : long {
		Automatic = 0,
		Priority,
		Fixed,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIEditMenuArrowDirection : long {
		Automatic = 0,
		Up = 1,
		Down = 2,
		Left = 3,
		Right = 4,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIFindSessionSearchResultDisplayStyle : long {
		CurrentAndTotal,
		Total,
		None,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIMenuElementSize : long {
		Small = 0,
		Medium,
		Large,
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		Automatic = -1,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UINavigationBarNSToolbarSection : long {
		None,
		Sidebar,
		Supplementary,
		Content,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UINavigationItemSearchBarPlacement : long {
		Automatic,
		Inline,
		Stacked,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UINavigationItemStyle : long {
		Navigator,
		Browser,
		Editor,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UINSToolbarItemPresentationSize : long {
		Unspecified = -1,
		Regular = 0,
		Small = 1,
		Large = 3,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIPageControlDirection : long {
		Natural = 0,
		LeftToRight = 1,
		RightToLeft = 2,
		TopToBottom = 3,
		BottomToTop = 4,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIPasteControlDisplayMode : ulong {
		IconAndLabel,
		IconOnly,
		LabelOnly,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UIScreenReferenceDisplayModeStatus : long {
		NotSupported,
		NotEnabled,
		Limited,
		Enabled,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UITableViewSelfSizingInvalidation : long {
		Disabled,
		Enabled,
		EnabledIncludingConstraints,
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UITextSearchFoundTextStyle : long {
		Normal,
		Found,
		Highlighted,
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum UITextSearchMatchMethod : long {
		Contains,
		StartsWith,
		FullWord,
	}

	delegate UIViewController UIContextMenuContentPreviewProvider ();
	delegate UIMenu UIContextMenuActionProvider (UIMenuElement [] suggestedActions);

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIContextMenuConfiguration {

		[Export ("identifier")]
		INSCopying Identifier { get; }

		[Static]
		[Export ("configurationWithIdentifier:previewProvider:actionProvider:")]
		UIContextMenuConfiguration Create ([NullAllowed] INSCopying identifier, [NullAllowed] UIContextMenuContentPreviewProvider previewProvider, [NullAllowed] UIContextMenuActionProvider actionProvider);

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("secondaryItemIdentifiers", ArgumentSemantic.Copy)]
		NSSet<INSCopying> SecondaryItemIdentifiers { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("badgeCount")]
		nint BadgeCount { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredMenuElementOrder", ArgumentSemantic.Assign)]
		UIContextMenuConfigurationElementOrder PreferredMenuElementOrder { get; set; }
	}

	interface IUIContextMenuInteractionDelegate { }

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIContextMenuInteractionDelegate {

		[Abstract]
		[Export ("contextMenuInteraction:configurationForMenuAtLocation:")]
		[return: NullAllowed]
		UIContextMenuConfiguration GetConfigurationForMenu (UIContextMenuInteraction interaction, CGPoint location);

		[NoTV]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("contextMenuInteraction:previewForHighlightingMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForHighlightingMenu (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration);

		[NoTV]
		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("contextMenuInteraction:previewForDismissingMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForDismissingMenu (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration);

		[NoTV]
		[Export ("contextMenuInteraction:willPerformPreviewActionForMenuWithConfiguration:animator:")]
		void WillPerformPreviewAction (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration, IUIContextMenuInteractionCommitAnimating animator);

		[Export ("contextMenuInteraction:willDisplayMenuForConfiguration:animator:")]
		void WillDisplayMenu (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[Export ("contextMenuInteraction:willEndForConfiguration:animator:")]
		void WillEnd (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("contextMenuInteraction:configuration:highlightPreviewForItemWithIdentifier:")]
		[return: NullAllowed]
		UITargetedPreview GetHighlightPreview (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration, INSCopying identifier);

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("contextMenuInteraction:configuration:dismissalPreviewForItemWithIdentifier:")]
		[return: NullAllowed]
		UITargetedPreview GetDismissalPreview (UIContextMenuInteraction interaction, UIContextMenuConfiguration configuration, INSCopying identifier);
	}

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIContextMenuInteraction : UIInteraction {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIContextMenuInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("menuAppearance")]
		UIContextMenuInteractionAppearance MenuAppearance { get; }

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIContextMenuInteractionDelegate @delegate);

		[Export ("locationInView:")]
		CGPoint GetLocation ([NullAllowed] UIView inView);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("updateVisibleMenuWithBlock:")]
		void UpdateVisibleMenu (Func<UIMenu, UIMenu> handler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dismissMenu")]
		void DismissMenu ();
	}

	interface IUIContextMenuInteractionCommitAnimating { }

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIContextMenuInteractionCommitAnimating : UIContextMenuInteractionAnimating {
		[Abstract]
		[Export ("preferredCommitStyle", ArgumentSemantic.Assign)]
		UIContextMenuInteractionCommitStyle PreferredCommitStyle { get; set; }
	}

	interface IUICoordinateSpace { }

	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UICoordinateSpace {
		[Abstract]
		[Export ("bounds")]
		CGRect Bounds { get; }

		[Abstract]
		[Export ("convertPoint:toCoordinateSpace:")]
		CGPoint ConvertPointToCoordinateSpace (CGPoint point, IUICoordinateSpace coordinateSpace);

		[Abstract]
		[Export ("convertPoint:fromCoordinateSpace:")]
		CGPoint ConvertPointFromCoordinateSpace (CGPoint point, IUICoordinateSpace coordinateSpace);

		[Abstract]
		[Export ("convertRect:toCoordinateSpace:")]
		CGRect ConvertRectToCoordinateSpace (CGRect rect, IUICoordinateSpace coordinateSpace);

		[Abstract]
		[Export ("convertRect:fromCoordinateSpace:")]
		CGRect ConvertRectFromCoordinateSpace (CGRect rect, IUICoordinateSpace coordinateSpace);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIApplicationDelegate {

		[Export ("applicationDidFinishLaunching:")]
		void FinishedLaunching (UIApplication application);

		[Export ("application:didFinishLaunchingWithOptions:")]
		bool FinishedLaunching (UIApplication application, NSDictionary launchOptions);

		[Export ("applicationDidBecomeActive:")]
		void OnActivated (UIApplication application);

		[Export ("applicationWillResignActive:")]
		void OnResignActivation (UIApplication application);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
		[Export ("application:handleOpenURL:")]
		bool HandleOpenURL (UIApplication application, NSUrl url);

		[Export ("applicationDidReceiveMemoryWarning:")]
		void ReceiveMemoryWarning (UIApplication application);

		[Export ("applicationWillTerminate:")]
		void WillTerminate (UIApplication application);

		[Export ("applicationSignificantTimeChange:")]
		void ApplicationSignificantTimeChange (UIApplication application);

		[NoTV]
		[Export ("application:willChangeStatusBarOrientation:duration:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		void WillChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation newStatusBarOrientation, double duration);

		[NoTV]
		[Export ("application:didChangeStatusBarOrientation:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		void DidChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation oldStatusBarOrientation);

		[NoTV]
		[Export ("application:willChangeStatusBarFrame:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		void WillChangeStatusBarFrame (UIApplication application, CGRect newStatusBarFrame);

		[NoTV]
		[Export ("application:didChangeStatusBarFrame:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ViewWillTransitionToSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ViewWillTransitionToSize' instead.")]
		void ChangedStatusBarFrame (UIApplication application, CGRect oldStatusBarFrame);

		[Export ("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
		void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken);

		[Export ("application:didFailToRegisterForRemoteNotificationsWithError:")]
		void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' for user visible notifications and 'ReceivedRemoteNotification' for silent remote notifications.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' for user visible notifications and 'ReceivedRemoteNotification' for silent remote notifications.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' for user visible notifications and 'ReceivedRemoteNotification' for silent remote notifications.")]
		[Export ("application:didReceiveRemoteNotification:")]
		void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' instead.")]
		[Export ("application:didReceiveLocalNotification:")]
		void ReceivedLocalNotification (UIApplication application, UILocalNotification notification);

		[Export ("applicationDidEnterBackground:")]
		void DidEnterBackground (UIApplication application);

		[Export ("applicationWillEnterForeground:")]
		void WillEnterForeground (UIApplication application);

		[Export ("applicationProtectedDataWillBecomeUnavailable:")]
		void ProtectedDataWillBecomeUnavailable (UIApplication application);

		[Export ("applicationProtectedDataDidBecomeAvailable:")]
		void ProtectedDataDidBecomeAvailable (UIApplication application);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
		[Export ("application:openURL:sourceApplication:annotation:")]
		bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation);

		[MacCatalyst (13, 1)]
		[Export ("application:openURL:options:")]
		bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("OpenUrl(app, url, options.GetDictionary ())")]
		bool OpenUrl (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions options);

		[Export ("window", ArgumentSemantic.Retain), NullAllowed]
		UIWindow Window { get; set; }

		//
		// 6.0
		//
		[Export ("application:willFinishLaunchingWithOptions:")]
		bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("application:supportedInterfaceOrientationsForWindow:")]
		UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, [Transient] UIWindow forWindow);

		[Export ("application:viewControllerWithRestorationIdentifierPath:coder:")]
		UIViewController GetViewController (UIApplication application, string [] restorationIdentifierComponents, NSCoder coder);

		[Deprecated (PlatformName.iOS, 13, 2, message: "Use 'ShouldSaveSecureApplicationState' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 2, message: "Use 'ShouldSaveSecureApplicationState' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShouldSaveSecureApplicationState' instead.")]
		[Export ("application:shouldSaveApplicationState:")]
		bool ShouldSaveApplicationState (UIApplication application, NSCoder coder);

		[iOS (13, 2)]
		[TV (13, 2)]
		[MacCatalyst (13, 1)]
		[Export ("application:shouldSaveSecureApplicationState:")]
		bool ShouldSaveSecureApplicationState (UIApplication application, NSCoder coder);

		[Deprecated (PlatformName.iOS, 13, 2, message: "Use 'ShouldRestoreSecureApplicationState' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 2, message: "Use 'ShouldRestoreSecureApplicationState' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShouldRestoreSecureApplicationState' instead.")]
		[Export ("application:shouldRestoreApplicationState:")]
		bool ShouldRestoreApplicationState (UIApplication application, NSCoder coder);

		[iOS (13, 2)]
		[TV (13, 2)]
		[MacCatalyst (13, 1)]
		[Export ("application:shouldRestoreSecureApplicationState:")]
		bool ShouldRestoreSecureApplicationState (UIApplication application, NSCoder coder);

		[Export ("application:willEncodeRestorableStateWithCoder:")]
		void WillEncodeRestorableState (UIApplication application, NSCoder coder);

		[Export ("application:didDecodeRestorableStateWithCoder:")]
		void DidDecodeRestorableState (UIApplication application, NSCoder coder);

		// special case from UIAccessibilityAction. we added it (completly) on UIResponser but magic tap is also:
		// "If you’d like the Magic Tap gesture to perform the same action from anywhere within your app, it is more 
		// appropriate to implement the accessibilityPerformMagicTap method in your app delegate."
		// ref: http://developer.apple.com/library/ios/#featuredarticles/ViewControllerPGforiPhoneOS/Accessibility/AccessibilityfromtheViewControllersPerspective.html
		[NoTV]
		[NoMacCatalyst]
		[Export ("accessibilityPerformMagicTap")]
		bool AccessibilityPerformMagicTap ();

		[MacCatalyst (13, 1)]
		[Export ("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
		void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler);

		[Export ("application:handleEventsForBackgroundURLSession:completionHandler:")]
		void HandleEventsForBackgroundUrl (UIApplication application, string sessionIdentifier, Action completionHandler);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use a 'BGAppRefreshTask' from 'BackgroundTasks' framework.")]
		[Export ("application:performFetchWithCompletionHandler:")]
		void PerformFetch (UIApplication application, Action<UIBackgroundFetchResult> completionHandler);

		// 
		// 8.0
		//
		[MacCatalyst (13, 1)]
		[Export ("application:continueUserActivity:restorationHandler:")]
		bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("application:didFailToContinueUserActivityWithType:error:")]
#if NET
		void DidFailToContinueUserActivity (UIApplication application, string userActivityType, NSError error);
#else
		void DidFailToContinueUserActivitiy (UIApplication application, string userActivityType, NSError error);
#endif

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[Export ("application:didRegisterUserNotificationSettings:")]
		void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forLocalNotification:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, UILocalNotification localNotification, Action completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forLocalNotification:withResponseInfo:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, UILocalNotification localNotification, NSDictionary responseInfo, Action completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forRemoteNotification:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forRemoteNotification:withResponseInfo:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, NSDictionary responseInfo, Action completionHandler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("application:performActionForShortcutItem:completionHandler:")]
		void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("application:willContinueUserActivityWithType:")]
		bool WillContinueUserActivity (UIApplication application, string userActivityType);

		[MacCatalyst (13, 1)]
		[Export ("application:didUpdateUserActivity:")]
		void UserActivityUpdated (UIApplication application, NSUserActivity userActivity);

		[MacCatalyst (13, 1)]
		[Export ("application:shouldAllowExtensionPointIdentifier:")]
		bool ShouldAllowExtensionPointIdentifier (UIApplication application, NSString extensionPointIdentifier);

		[MacCatalyst (13, 1)]
		[Export ("application:handleWatchKitExtensionRequest:reply:")]
		void HandleWatchKitExtensionRequest (UIApplication application, NSDictionary userInfo, Action<NSDictionary> reply);

		[MacCatalyst (13, 1)]
		[Export ("applicationShouldRequestHealthAuthorization:")]
		void ShouldRequestHealthAuthorization (UIApplication application);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("application:userDidAcceptCloudKitShareWithMetadata:")]
		void UserDidAcceptCloudKitShare (UIApplication application, CKShareMetadata cloudKitShareMetadata);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'GetHandlerForIntent' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'GetHandlerForIntent' instead.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'GetHandlerForIntent' instead.")]
		[Export ("application:handleIntent:completionHandler:")]
		void HandleIntent (UIApplication application, INIntent intent, Action<INIntentResponse> completionHandler);

		[iOS (14, 0), TV (14, 0), Watch (7, 0)]
		[MacCatalyst (14, 0)]
		[Export ("application:handlerForIntent:")]
		[return: NullAllowed]
		NSObject GetHandlerForIntent (UIApplication application, INIntent intent);

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("application:configurationForConnectingSceneSession:options:")]
		UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options);

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("application:didDiscardSceneSessions:")]
		void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions);

		[iOS (15, 0), TV (15, 0), Watch (8, 0), MacCatalyst (15, 0)]
		[Export ("applicationShouldAutomaticallyLocalizeKeyCommands:")]
		bool ShouldAutomaticallyLocalizeKeyCommands (UIApplication application);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIExtensionPointIdentifier {
		[MacCatalyst (13, 1)]
		[Field ("UIApplicationKeyboardExtensionPointIdentifier")]
		NSString Keyboard { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIBarItem : NSCoding, UIAppearance, UIAccessibility, UIAccessibilityIdentification {
		[Export ("enabled")]
		[Abstract]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed]
		[Export ("title", ArgumentSemantic.Copy)]
		[Abstract]
		string Title { get; set; }

		[NullAllowed]
		[Export ("image", ArgumentSemantic.Retain)]
		[Abstract]
		UIImage Image { get; set; }

		[Export ("imageInsets")]
		[Abstract]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")]
		[Abstract]
		nint Tag { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("landscapeImagePhone", ArgumentSemantic.Retain)]
		UIImage LandscapeImagePhone { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("landscapeImagePhoneInsets")]
		UIEdgeInsets LandscapeImagePhoneInsets { get; set; }

		[Export ("setTitleTextAttributes:forState:"), Internal]
		[Appearance]
		void _SetTitleTextAttributes ([NullAllowed] NSDictionary attributes, UIControlState state);

		[Export ("titleTextAttributesForState:"), Internal]
		[Appearance]
		NSDictionary _GetTitleTextAttributes (UIControlState state);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("largeContentSizeImage", ArgumentSemantic.Strong)]
		UIImage LargeContentSizeImage { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("largeContentSizeImageInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets LargeContentSizeImageInsets { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarItem))]
	[DesignatedDefaultCtor]
	interface UIBarButtonItem : NSCoding
#if IOS
		, UISpringLoadedInteractionSupporting, UIPopoverPresentationControllerSourceItem
#endif
	 {
		[Export ("initWithImage:style:target:action:")]
		[PostGet ("Image")]
		[PostGet ("Target")]
		NativeHandle Constructor ([NullAllowed] UIImage image, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("initWithTitle:style:target:action:")]
		[PostGet ("Target")]
		NativeHandle Constructor ([NullAllowed] string title, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("initWithBarButtonSystemItem:target:action:")]
		[PostGet ("Target")]
		NativeHandle Constructor (UIBarButtonSystemItem systemItem, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("initWithCustomView:")]
		[PostGet ("CustomView")]
		NativeHandle Constructor (UIView customView);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithBarButtonSystemItem:primaryAction:")]
		NativeHandle Constructor (UIBarButtonSystemItem systemItem, [NullAllowed] UIAction primaryAction);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithPrimaryAction:")]
		NativeHandle Constructor ([NullAllowed] UIAction primaryAction);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithBarButtonSystemItem:menu:")]
		NativeHandle Constructor (UIBarButtonSystemItem systemItem, [NullAllowed] UIMenu menu);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithTitle:menu:")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] UIMenu menu);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithImage:menu:")]
		NativeHandle Constructor ([NullAllowed] UIImage image, [NullAllowed] UIMenu menu);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithPrimaryAction:menu:")]
		NativeHandle Constructor ([NullAllowed] UIAction primaryAction, [NullAllowed] UIMenu menu);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithBarButtonSystemItem:primaryAction:menu:")]
		NativeHandle Constructor (UIBarButtonSystemItem systemItem, [NullAllowed] UIAction primaryAction, [NullAllowed] UIMenu menu);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithTitle:image:target:action:menu:")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] UIImage image, [NullAllowed] NSObject target, [NullAllowed] Selector action, [NullAllowed] UIMenu menu);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("fixedSpaceItemOfWidth:")]
		UIBarButtonItem GetFixedSpaceItem (nfloat width);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("flexibleSpaceItem")]
		UIBarButtonItem FlexibleSpaceItem { get; }

		[Export ("style")]
		UIBarButtonItemStyle Style { get; set; }

		[Export ("width")]
		nfloat Width { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("possibleTitles", ArgumentSemantic.Copy)]
		NSSet PossibleTitles { get; set; }

		[Export ("customView", ArgumentSemantic.Retain), NullAllowed]
		UIView CustomView { get; set; }

		[Export ("action")]
		[NullAllowed]
		Selector Action { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("target", ArgumentSemantic.Assign)]
		NSObject Target { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("primaryAction", ArgumentSemantic.Copy)]
		UIAction PrimaryAction { get; set; }

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("menu", ArgumentSemantic.Copy)]
		UIMenu Menu { get; set; }

		[NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("changesSelectionAsPrimaryAction")]
		bool ChangesSelectionAsPrimaryAction { get; set; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("enabled")]
		[Override]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed]
		[Export ("title", ArgumentSemantic.Copy)]
		[Override]
		string Title { get; set; }

		[NullAllowed]
		[Export ("image", ArgumentSemantic.Retain)]
		[Override]
		UIImage Image { get; set; }

		[Export ("imageInsets")]
		[Override]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")]
		[Override]
		nint Tag { get; set; }

		[Export ("tintColor", ArgumentSemantic.Retain), NullAllowed]
		[Appearance]
		UIColor TintColor { get; set; }

		[Export ("initWithImage:landscapeImagePhone:style:target:action:"), PostGet ("Image")]
#if !TVOS
		[PostGet ("LandscapeImagePhone")]
#endif
		[PostGet ("Target")]
		NativeHandle Constructor ([NullAllowed] UIImage image, [NullAllowed] UIImage landscapeImagePhone, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("setBackgroundImage:forState:barMetrics:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState state, UIBarMetrics barMetrics);

		[Export ("backgroundImageForState:barMetrics:")]
		[Appearance]
		UIImage GetBackgroundImage (UIControlState state, UIBarMetrics barMetrics);

		[Export ("setBackgroundVerticalPositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetBackgroundVerticalPositionAdjustment (nfloat adjustment, UIBarMetrics forBarMetrics);

		[Export ("backgroundVerticalPositionAdjustmentForBarMetrics:")]
		[Appearance]
		nfloat GetBackgroundVerticalPositionAdjustment (UIBarMetrics forBarMetrics);

		[Export ("setTitlePositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetTitlePositionAdjustment (UIOffset adjustment, UIBarMetrics barMetrics);

		[Export ("titlePositionAdjustmentForBarMetrics:")]
		[Appearance]
		UIOffset GetTitlePositionAdjustment (UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setBackButtonBackgroundImage:forState:barMetrics:")]
		[Appearance]
		void SetBackButtonBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState forState, UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("backButtonBackgroundImageForState:barMetrics:")]
		[Appearance]
		UIImage GetBackButtonBackgroundImage (UIControlState forState, UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setBackButtonTitlePositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetBackButtonTitlePositionAdjustment (UIOffset adjustment, UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("backButtonTitlePositionAdjustmentForBarMetrics:")]
		[Appearance]
		UIOffset GetBackButtonTitlePositionAdjustment (UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setBackButtonBackgroundVerticalPositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetBackButtonBackgroundVerticalPositionAdjustment (nfloat adjustment, UIBarMetrics barMetrics);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("backButtonBackgroundVerticalPositionAdjustmentForBarMetrics:")]
		[Appearance]
		nfloat GetBackButtonBackgroundVerticalPositionAdjustment (UIBarMetrics barMetrics);

		[Appearance]
		[Export ("setBackgroundImage:forState:style:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState state, UIBarButtonItemStyle style, UIBarMetrics barMetrics);

		[Appearance]
		[Export ("backgroundImageForState:style:barMetrics:")]
		UIImage GetBackgroundImage (UIControlState state, UIBarButtonItemStyle style, UIBarMetrics barMetrics);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("buttonGroup", ArgumentSemantic.Weak)]
		UIBarButtonItemGroup ButtonGroup { get; }

		[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredMenuElementOrder", ArgumentSemantic.Assign)]
		UIContextMenuConfigurationElementOrder PreferredMenuElementOrder { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("menuRepresentation", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIMenuElement MenuRepresentation { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("creatingFixedGroup")]
		UIBarButtonItemGroup CreatingFixedGroup { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("creatingMovableGroupWithCustomizationIdentifier:")]
		UIBarButtonItemGroup CreatingMovableGroup (string customizationIdentifier);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("creatingOptionalGroupWithCustomizationIdentifier:inDefaultCustomization:")]
		UIBarButtonItemGroup CreatingOptionalGroup (string customizationIdentifier, bool inDefaultCustomization);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:options:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:options:animated:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:options:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:options:animated:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffects")]
		void RemoveAllSymbolEffects ();

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffectsWithOptions:")]
		void RemoveAllSymbolEffects (NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffectsWithOptions:animated:")]
		void RemoveAllSymbolEffects (NSSymbolEffectOptions options, bool animated);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setSymbolImage:withContentTransition:")]
		void SetSymbolImage (UIImage symbolImage, NSSymbolContentTransition transition);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setSymbolImage:withContentTransition:options:")]
		void SetSymbolImage (UIImage symbolImage, NSSymbolContentTransition transition, NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("symbolAnimationEnabled")]
		bool SymbolAnimationEnabled { [Bind ("isSymbolAnimationEnabled")] get; set; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIBarButtonItemGroup : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithBarButtonItems:representativeItem:")]
		NativeHandle Constructor (UIBarButtonItem [] barButtonItems, [NullAllowed] UIBarButtonItem representativeItem);

		[Export ("barButtonItems", ArgumentSemantic.Copy)]
		UIBarButtonItem [] BarButtonItems { get; set; }

		[NullAllowed, Export ("representativeItem", ArgumentSemantic.Strong)]
		UIBarButtonItem RepresentativeItem { get; set; }

		[Export ("displayingRepresentativeItem")]
		bool DisplayingRepresentativeItem { [Bind ("isDisplayingRepresentativeItem")] get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("alwaysAvailable")]
		bool AlwaysAvailable { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("menuRepresentation", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIMenuElement MenuRepresentation { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("optionalGroupWithCustomizationIdentifier:inDefaultCustomization:representativeItem:items:")]
		UIBarButtonItemGroup GetOptionalGroup (string customizationIdentifier, bool inDefaultCustomization, [NullAllowed] UIBarButtonItem representativeItem, UIBarButtonItem [] items);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("movableGroupWithCustomizationIdentifier:representativeItem:items:")]
		UIBarButtonItemGroup GetMovableGroup (string customizationIdentifier, [NullAllowed] UIBarButtonItem representativeItem, UIBarButtonItem [] items);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("fixedGroupWithRepresentativeItem:items:")]
		UIBarButtonItemGroup GetFixedGroup ([NullAllowed] UIBarButtonItem representativeItem, UIBarButtonItem [] items);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UICollectionReusableView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
		NSString ReuseIdentifier { get; }

		[RequiresSuper]
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Export ("applyLayoutAttributes:")]
		void ApplyLayoutAttributes ([NullAllowed] UICollectionViewLayoutAttributes layoutAttributes);

		[Export ("willTransitionFromLayout:toLayout:")]
		void WillTransition (UICollectionViewLayout oldLayout, UICollectionViewLayout newLayout);

		[Export ("didTransitionFromLayout:toLayout:")]
		void DidTransition (UICollectionViewLayout oldLayout, UICollectionViewLayout newLayout);

		[MacCatalyst (13, 1)]
		[Export ("preferredLayoutAttributesFittingAttributes:")]
		UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes (UICollectionViewLayoutAttributes layoutAttributes);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIScrollView))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: UICollectionView must be initialized with a non-nil layout parameter
	[DisableDefaultCtor]
	interface UICollectionView : NSCoding, UIDataSourceTranslating
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithFrame:collectionViewLayout:"), PostGet ("CollectionViewLayout")]
		NativeHandle Constructor (CGRect frame, UICollectionViewLayout layout);

		[Export ("collectionViewLayout", ArgumentSemantic.Retain)]
		UICollectionViewLayout CollectionViewLayout { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UICollectionViewDelegate Delegate { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		UICollectionViewDataSource DataSource { get; set; }

		[Export ("backgroundView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView BackgroundView { get; set; }

		[Export ("allowsSelection")]
		bool AllowsSelection { get; set; }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("registerClass:forCellWithReuseIdentifier:"), Internal]
		void RegisterClassForCell (IntPtr /* Class */cellClass, NSString reuseIdentifier);

		[Export ("registerNib:forCellWithReuseIdentifier:")]
		void RegisterNibForCell ([NullAllowed] UINib nib, NSString reuseIdentifier);

		[Export ("registerClass:forSupplementaryViewOfKind:withReuseIdentifier:"), Protected]
		void RegisterClassForSupplementaryView (IntPtr /*Class*/ viewClass, NSString kind, NSString reuseIdentifier);

		[Export ("registerNib:forSupplementaryViewOfKind:withReuseIdentifier:")]
		void RegisterNibForSupplementaryView ([NullAllowed] UINib nib, NSString kind, NSString reuseIdentifier);

		[Export ("dequeueReusableCellWithReuseIdentifier:forIndexPath:")]
		UICollectionReusableView DequeueReusableCell (NSString reuseIdentifier, NSIndexPath indexPath);

		[Export ("dequeueReusableSupplementaryViewOfKind:withReuseIdentifier:forIndexPath:")]
		UICollectionReusableView DequeueReusableSupplementaryView (NSString kind, NSString identifier, NSIndexPath indexPath);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dequeueConfiguredReusableCellWithRegistration:forIndexPath:item:")]
		UICollectionViewCell DequeueConfiguredReusableCell (UICollectionViewCellRegistration registration, NSIndexPath indexPath, NSObject item);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("dequeueConfiguredReusableSupplementaryViewWithRegistration:forIndexPath:")]
		UICollectionReusableView DequeueConfiguredReusableSupplementaryView (UICollectionViewSupplementaryRegistration registration, NSIndexPath indexPath);

		[Export ("indexPathsForSelectedItems")]
		[return: NullAllowed]
		NSIndexPath [] GetIndexPathsForSelectedItems ();

		[Export ("selectItemAtIndexPath:animated:scrollPosition:")]
		void SelectItem ([NullAllowed] NSIndexPath indexPath, bool animated, UICollectionViewScrollPosition scrollPosition);

		[Export ("deselectItemAtIndexPath:animated:")]
		void DeselectItem (NSIndexPath indexPath, bool animated);

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("setCollectionViewLayout:animated:")]
		void SetCollectionViewLayout (UICollectionViewLayout layout, bool animated);

		[Export ("numberOfSections")]
		nint NumberOfSections ();

		[Export ("numberOfItemsInSection:")]
		nint NumberOfItemsInSection (nint section);

		[Export ("layoutAttributesForItemAtIndexPath:")]
		[return: NullAllowed]
		UICollectionViewLayoutAttributes GetLayoutAttributesForItem (NSIndexPath indexPath);

		[Export ("layoutAttributesForSupplementaryElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		UICollectionViewLayoutAttributes GetLayoutAttributesForSupplementaryElement (NSString elementKind, NSIndexPath indexPath);

		[Export ("indexPathForItemAtPoint:")]
		[return: NullAllowed]
		NSIndexPath IndexPathForItemAtPoint (CGPoint point);

		[Export ("indexPathForCell:")]
		[return: NullAllowed]
		NSIndexPath IndexPathForCell (UICollectionViewCell cell);

		[Export ("cellForItemAtIndexPath:")]
		[return: NullAllowed]
		UICollectionViewCell CellForItem (NSIndexPath indexPath);

		[Export ("visibleCells")]
		UICollectionViewCell [] VisibleCells { get; }

		[Export ("indexPathsForVisibleItems")]
		NSIndexPath [] IndexPathsForVisibleItems { get; }

		[Export ("scrollToItemAtIndexPath:atScrollPosition:animated:")]
		void ScrollToItem (NSIndexPath indexPath, UICollectionViewScrollPosition scrollPosition, bool animated);

		[Export ("insertSections:")]
		void InsertSections (NSIndexSet sections);

		[Export ("deleteSections:")]
		void DeleteSections (NSIndexSet sections);

		[Export ("reloadSections:")]
		void ReloadSections (NSIndexSet sections);

		[Export ("moveSection:toSection:")]
		void MoveSection (nint section, nint newSection);

		[Export ("insertItemsAtIndexPaths:")]
		void InsertItems (NSIndexPath [] indexPaths);

		[Export ("deleteItemsAtIndexPaths:")]
		void DeleteItems (NSIndexPath [] indexPaths);

		[Export ("reloadItemsAtIndexPaths:")]
		void ReloadItems (NSIndexPath [] indexPaths);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reconfigureItemsAtIndexPaths:")]
		void ReconfigureItems (NSIndexPath [] indexPaths);

		[Export ("moveItemAtIndexPath:toIndexPath:")]
		void MoveItem (NSIndexPath indexPath, NSIndexPath newIndexPath);

		[Export ("performBatchUpdates:completion:")]
		[Async]
		void PerformBatchUpdates ([NullAllowed] Action updates, [NullAllowed] UICompletionHandler completed);

		//
		// 7.0
		//
		[Export ("startInteractiveTransitionToCollectionViewLayout:completion:")]
		[Async (ResultTypeName = "UICollectionViewTransitionResult")]
		UICollectionViewTransitionLayout StartInteractiveTransition (UICollectionViewLayout newCollectionViewLayout,
										 [NullAllowed] UICollectionViewLayoutInteractiveTransitionCompletion completion);

		[Export ("setCollectionViewLayout:animated:completion:")]
		[Async]
		void SetCollectionViewLayout (UICollectionViewLayout layout, bool animated, [NullAllowed] UICompletionHandler completion);

		[Export ("finishInteractiveTransition")]
		void FinishInteractiveTransition ();

		[Export ("cancelInteractiveTransition")]
		void CancelInteractiveTransition ();

		[MacCatalyst (13, 1)]
		[Export ("beginInteractiveMovementForItemAtIndexPath:")]
		bool BeginInteractiveMovementForItem (NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("updateInteractiveMovementTargetPosition:")]
		void UpdateInteractiveMovement (CGPoint targetPosition);

		[MacCatalyst (13, 1)]
		[Export ("endInteractiveMovement")]
		void EndInteractiveMovement ();

		[MacCatalyst (13, 1)]
		[Export ("cancelInteractiveMovement")]
		void CancelInteractiveMovement ();


		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("supplementaryViewForElementKind:atIndexPath:")]
		UICollectionReusableView GetSupplementaryView (NSString elementKind, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("visibleSupplementaryViewsOfKind:")]
		UICollectionReusableView [] GetVisibleSupplementaryViews (NSString elementKind);

		[MacCatalyst (13, 1)]
		[Export ("indexPathsForVisibleSupplementaryElementsOfKind:")]
		NSIndexPath [] GetIndexPathsForVisibleSupplementaryElements (NSString elementKind);

		[MacCatalyst (13, 1)]
		[Export ("remembersLastFocusedIndexPath")]
		bool RemembersLastFocusedIndexPath { get; set; }

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("selectionFollowsFocus")]
		bool SelectionFollowsFocus { get; set; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsFocus")]
		bool AllowsFocus { get; set; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsFocusDuringEditing")]
		bool AllowsFocusDuringEditing { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("prefetchDataSource", ArgumentSemantic.Weak)]
		IUICollectionViewDataSourcePrefetching PrefetchDataSource { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("prefetchingEnabled")]
		bool PrefetchingEnabled { [Bind ("isPrefetchingEnabled")] get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dragDelegate", ArgumentSemantic.Weak)]
		IUICollectionViewDragDelegate DragDelegate { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dropDelegate", ArgumentSemantic.Weak)]
		IUICollectionViewDropDelegate DropDelegate { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("dragInteractionEnabled")]
		bool DragInteractionEnabled { get; set; }

		[NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("contextMenuInteraction")]
		UIContextMenuInteraction ContextMenuInteraction { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("reorderingCadence", ArgumentSemantic.Assign)]
		UICollectionViewReorderingCadence ReorderingCadence { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hasUncommittedUpdates")]
		bool HasUncommittedUpdates { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasActiveDrag")]
		bool HasActiveDrag { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasActiveDrop")]
		bool HasActiveDrop { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowsSelectionDuringEditing")]
		bool AllowsSelectionDuringEditing { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowsMultipleSelectionDuringEditing")]
		bool AllowsMultipleSelectionDuringEditing { get; set; }

		[Watch (9, 0), TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("selfSizingInvalidation", ArgumentSemantic.Assign)]
		UICollectionViewSelfSizingInvalidation SelfSizingInvalidation { get; set; }
	}

	interface IUICollectionViewDataSourcePrefetching { }

	[Protocol]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UICollectionViewDataSourcePrefetching {

		[Abstract]
		[Export ("collectionView:prefetchItemsAtIndexPaths:")]
		void PrefetchItems (UICollectionView collectionView, NSIndexPath [] indexPaths);

		[Export ("collectionView:cancelPrefetchingForItemsAtIndexPaths:")]
		void CancelPrefetching (UICollectionView collectionView, NSIndexPath [] indexPaths);
	}

	//
	// Combined version of UICollectionViewDataSource, UICollectionViewDelegate
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol (IsInformal = true)]
	interface UICollectionViewSource : UICollectionViewDataSource, UICollectionViewDelegate {

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UICollectionViewDataSource {
		[Abstract]
		[Export ("collectionView:numberOfItemsInSection:")]
		nint GetItemsCount (UICollectionView collectionView, nint section);

		[Abstract]
		[Export ("collectionView:cellForItemAtIndexPath:")]
		UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("numberOfSectionsInCollectionView:")]
		nint NumberOfSections (UICollectionView collectionView);

		[Export ("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
		UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:canMoveItemAtIndexPath:")]
		bool CanMoveItem (UICollectionView collectionView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:moveItemAtIndexPath:toIndexPath:")]
		void MoveItem (UICollectionView collectionView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("indexTitlesForCollectionView:")]
		string [] GetIndexTitles (UICollectionView collectionView);

		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Export ("collectionView:indexPathForIndexTitle:atIndex:")]
		NSIndexPath GetIndexPath (UICollectionView collectionView, string title, nint atIndex);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
#if XAMCORE_3_0 && !NET
	// bind like UITableViewDelegate to avoid generating duplicate code
	// it's an API break (binary, source should be fine)
	[BaseType (typeof (UIScrollViewDelegate))]
	interface UICollectionViewDelegate {
#else
	[BaseType (typeof (NSObject))]
	interface UICollectionViewDelegate : UIScrollViewDelegate {
#endif
		[Export ("collectionView:shouldHighlightItemAtIndexPath:")]
		bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:didHighlightItemAtIndexPath:")]
		void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:didUnhighlightItemAtIndexPath:")]
		void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:shouldSelectItemAtIndexPath:")]
		bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:shouldDeselectItemAtIndexPath:")]
		bool ShouldDeselectItem (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:didSelectItemAtIndexPath:")]
		void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:didDeselectItemAtIndexPath:")]
		void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:willDisplayCell:forItemAtIndexPath:")]
		void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:willDisplaySupplementaryView:forElementKind:atIndexPath:")]
		void WillDisplaySupplementaryView (UICollectionView collectionView, UICollectionReusableView view, string elementKind, NSIndexPath indexPath);

		[Export ("collectionView:didEndDisplayingCell:forItemAtIndexPath:")]
		void CellDisplayingEnded (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath);

		[Export ("collectionView:didEndDisplayingSupplementaryView:forElementOfKind:atIndexPath:")]
		void SupplementaryViewDisplayingEnded (UICollectionView collectionView, UICollectionReusableView view, NSString elementKind, NSIndexPath indexPath);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("collectionView:shouldShowMenuForItemAtIndexPath:")]
		bool ShouldShowMenu (UICollectionView collectionView, NSIndexPath indexPath);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("collectionView:canPerformAction:forItemAtIndexPath:withSender:")]
		bool CanPerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("collectionView:performAction:forItemAtIndexPath:withSender:")]
		void PerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Export ("collectionView:transitionLayoutForOldLayout:newLayout:")]
		UICollectionViewTransitionLayout TransitionLayout (UICollectionView collectionView, UICollectionViewLayout fromLayout, UICollectionViewLayout toLayout);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'GetTargetIndexPathForMoveOfItemFromOriginalIndexPath' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'GetTargetIndexPathForMoveOfItemFromOriginalIndexPath' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'GetTargetIndexPathForMoveOfItemFromOriginalIndexPath' instead.")]
		[Export ("collectionView:targetIndexPathForMoveFromItemAtIndexPath:toProposedIndexPath:")]
		NSIndexPath GetTargetIndexPathForMove (UICollectionView collectionView, NSIndexPath originalIndexPath, NSIndexPath proposedIndexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:targetContentOffsetForProposedContentOffset:")]
		CGPoint GetTargetContentOffset (UICollectionView collectionView, CGPoint proposedContentOffset);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("collectionView:canEditItemAtIndexPath:")]
		bool CanEditItem (UICollectionView collectionView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:canFocusItemAtIndexPath:")]
		bool CanFocusItem (UICollectionView collectionView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UICollectionView collectionView, UICollectionViewFocusUpdateContext context);

		[MacCatalyst (13, 1)]
		[Export ("collectionView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UICollectionView collectionView, UICollectionViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[MacCatalyst (13, 1)]
		[Export ("indexPathForPreferredFocusedViewInCollectionView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UICollectionView collectionView);

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("collectionView:selectionFollowsFocusForItemAtIndexPath:")]
		bool GetSelectionFollowsFocusForItem (UICollectionView collectionView, NSIndexPath indexPath);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("collectionView:targetIndexPathForMoveOfItemFromOriginalIndexPath:atCurrentIndexPath:toProposedIndexPath:")]
		NSIndexPath GetTargetIndexPathForMoveOfItemFromOriginalIndexPath (UICollectionView collectionView, NSIndexPath originalIndexPath, NSIndexPath currentIndexPath, NSIndexPath proposedIndexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("collectionView:shouldSpringLoadItemAtIndexPath:withContext:")]
		bool ShouldSpringLoadItem (UICollectionView collectionView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("collectionView:shouldBeginMultipleSelectionInteractionAtIndexPath:")]
		bool ShouldBeginMultipleSelectionInteraction (UICollectionView collectionView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("collectionView:didBeginMultipleSelectionInteractionAtIndexPath:")]
		void DidBeginMultipleSelectionInteraction (UICollectionView collectionView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("collectionViewDidEndMultipleSelectionInteraction:")]
		void DidEndMultipleSelectionInteraction (UICollectionView collectionView);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("collectionView:contextMenuConfigurationForItemAtIndexPath:point:")]
		[return: NullAllowed]
		UIContextMenuConfiguration GetContextMenuConfiguration (UICollectionView collectionView, NSIndexPath indexPath, CGPoint point);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("collectionView:previewForHighlightingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForHighlightingContextMenu (UICollectionView collectionView, UIContextMenuConfiguration configuration);

		[Deprecated (PlatformName.iOS, 16, 0)]
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("collectionView:previewForDismissingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForDismissingContextMenu (UICollectionView collectionView, UIContextMenuConfiguration configuration);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("collectionView:willPerformPreviewActionForMenuWithConfiguration:animator:")]
		void WillPerformPreviewAction (UICollectionView collectionView, UIContextMenuConfiguration configuration, IUIContextMenuInteractionCommitAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("collectionView:willDisplayContextMenuWithConfiguration:animator:")]
		void WillDisplayContextMenu (UICollectionView collectionView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("collectionView:willEndContextMenuInteractionWithConfiguration:animator:")]
		void WillEndContextMenuInteraction (UICollectionView collectionView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("collectionView:sceneActivationConfigurationForItemAtIndexPath:point:")]
		[return: NullAllowed]
		UIWindowSceneActivationConfiguration GetSceneActivationConfigurationForItem (UICollectionView collectionView, NSIndexPath indexPath, CGPoint point);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("collectionView:canPerformPrimaryActionForItemAtIndexPath:")]
		bool CanPerformPrimaryActionForItem (UICollectionView collectionView, NSIndexPath indexPath);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("collectionView:performPrimaryActionForItemAtIndexPath:")]
		void PerformPrimaryActionForItem (UICollectionView collectionView, NSIndexPath indexPath);

		[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("collectionView:contextMenuConfigurationForItemsAtIndexPaths:point:")]
		[return: NullAllowed]
		UIContextMenuConfiguration GetContextMenuConfiguration (UICollectionView collectionView, NSIndexPath [] indexPaths, CGPoint point);

		[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("collectionView:contextMenuConfiguration:highlightPreviewForItemAtIndexPath:")]
		[return: NullAllowed]
		UITargetedPreview GetContextMenuConfigurationHighlightPreview (UICollectionView collectionView, UIContextMenuConfiguration configuration, NSIndexPath indexPath);

		[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("collectionView:contextMenuConfiguration:dismissalPreviewForItemAtIndexPath:")]
		[return: NullAllowed]
		UITargetedPreview GetContextMenuConfigurationDismissalPreview (UICollectionView collectionView, UIContextMenuConfiguration configuration, NSIndexPath indexPath);
	}

	[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	delegate void UICollectionViewCellConfigurationUpdateHandler (UICollectionViewCell cell, UICellConfigurationState state);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionReusableView))]
	interface UICollectionViewCell {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("configurationState")]
		UICellConfigurationState ConfigurationState { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setNeedsUpdateConfiguration")]
		void SetNeedsUpdateConfiguration ();

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("updateConfigurationUsingState:")]
		void UpdateConfiguration (UICellConfigurationState state);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("configurationUpdateHandler", ArgumentSemantic.Copy)]
		UICollectionViewCellConfigurationUpdateHandler ConfigurationUpdateHandler { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("contentConfiguration", ArgumentSemantic.Copy)]
		IUIContentConfiguration ContentConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesContentConfiguration")]
		bool AutomaticallyUpdatesContentConfiguration { get; set; }

		[Export ("contentView")]
		UIView ContentView { get; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[NullAllowed] // by default this property is null
		[Export ("backgroundView", ArgumentSemantic.Retain)]
		UIView BackgroundView { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("selectedBackgroundView", ArgumentSemantic.Retain)]
		UIView SelectedBackgroundView { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("dragStateDidChange:")]
		void DragStateDidChange (UICollectionViewCellDragState dragState);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("backgroundConfiguration", ArgumentSemantic.Copy)]
		UIBackgroundConfiguration BackgroundConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesBackgroundConfiguration")]
		bool AutomaticallyUpdatesBackgroundConfiguration { get; set; }

		[Watch (9, 0), TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("defaultBackgroundConfiguration")]
		UIBackgroundConfiguration DefaultBackgroundConfiguration { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UICollectionViewController : UICollectionViewSource, NSCoding {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("collectionView", ArgumentSemantic.Retain)]
		UICollectionView CollectionView { get; set; }

		[Export ("clearsSelectionOnViewWillAppear")]
		bool ClearsSelectionOnViewWillAppear { get; set; }

		// The PostSnippet is there to ensure that "layout" is alive
		// note: we can't use [PostGet] since it would not work before iOS7 so the hack must remain...
		[DesignatedInitializer]
		[Export ("initWithCollectionViewLayout:")]
		NativeHandle Constructor (UICollectionViewLayout layout);

		[Export ("collectionViewLayout")]
		UICollectionViewLayout Layout { get; }

		[Export ("useLayoutToLayoutNavigationTransitions", ArgumentSemantic.Assign)]
		bool UseLayoutToLayoutNavigationTransitions { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("installsStandardGestureForInteractiveMovement")]
		bool InstallsStandardGestureForInteractiveMovement { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewDelegate))]
	[Model]
	[Protocol]
	interface UICollectionViewDelegateFlowLayout {
		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath);

		[Export ("collectionView:layout:insetForSectionAtIndex:")]
		UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section);

		[Export ("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
		nfloat GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section);

		[Export ("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
		nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section);

		[Export ("collectionView:layout:referenceSizeForHeaderInSection:")]
		CGSize GetReferenceSizeForHeader (UICollectionView collectionView, UICollectionViewLayout layout, nint section);

		[Export ("collectionView:layout:referenceSizeForFooterInSection:")]
		CGSize GetReferenceSizeForFooter (UICollectionView collectionView, UICollectionViewLayout layout, nint section);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewLayout))]
	interface UICollectionViewFlowLayout {
		[Export ("minimumLineSpacing")]
		nfloat MinimumLineSpacing { get; set; }

		[Export ("minimumInteritemSpacing")]
		nfloat MinimumInteritemSpacing { get; set; }

		[Export ("itemSize")]
		CGSize ItemSize { get; set; }

		// Default value of this property is CGSize.Zero, setting to any other value causes each cell to be queried
		[MacCatalyst (13, 1)]
		[Export ("estimatedItemSize")]
		CGSize EstimatedItemSize { get; set; }

		[Export ("scrollDirection")]
		UICollectionViewScrollDirection ScrollDirection { get; set; }

		[Export ("headerReferenceSize")]
		CGSize HeaderReferenceSize { get; set; }

		[Export ("footerReferenceSize")]
		CGSize FooterReferenceSize { get; set; }

		[Export ("sectionInset")]
		UIEdgeInsets SectionInset { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("sectionInsetReference", ArgumentSemantic.Assign)]
		UICollectionViewFlowLayoutSectionInsetReference SectionInsetReference { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sectionHeadersPinToVisibleBounds")]
		bool SectionHeadersPinToVisibleBounds { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sectionFootersPinToVisibleBounds")]
		bool SectionFootersPinToVisibleBounds { get; set; }

		[MacCatalyst (13, 1)]
		[Field ("UICollectionViewFlowLayoutAutomaticSize")]
		CGSize AutomaticSize { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UICollectionViewLayout : NSCoding {

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UICollectionViewLayoutAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[Export ("collectionView")]
		UICollectionView CollectionView { get; }

		[Export ("invalidateLayout")]
		void InvalidateLayout ();

		[Export ("registerClass:forDecorationViewOfKind:"), Internal]
		void RegisterClassForDecorationView (IntPtr classPtr, NSString kind);

		[Export ("registerNib:forDecorationViewOfKind:")]
		void RegisterNibForDecorationView ([NullAllowed] UINib nib, NSString kind);

		//
		// Subclassing methods
		//
		[Export ("prepareLayout")]
		void PrepareLayout ();

		[Export ("layoutAttributesForElementsInRect:")]
		UICollectionViewLayoutAttributes [] LayoutAttributesForElementsInRect (CGRect rect);

		[Export ("layoutAttributesForItemAtIndexPath:")]
		UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath);

		[Export ("layoutAttributesForSupplementaryViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes LayoutAttributesForSupplementaryView (NSString kind, NSIndexPath indexPath);

		[Export ("layoutAttributesForDecorationViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes LayoutAttributesForDecorationView (NSString kind, NSIndexPath indexPath);

		[Export ("shouldInvalidateLayoutForBoundsChange:")]
		bool ShouldInvalidateLayoutForBoundsChange (CGRect newBounds);

		[Export ("targetContentOffsetForProposedContentOffset:withScrollingVelocity:")]
		CGPoint TargetContentOffset (CGPoint proposedContentOffset, CGPoint scrollingVelocity);

		[Export ("collectionViewContentSize")]
		CGSize CollectionViewContentSize { get; }


		[Export ("prepareForAnimatedBoundsChange:")]
		void PrepareForAnimatedBoundsChange (CGRect oldBounds);

		[MacCatalyst (13, 1)]
		[Export ("invalidationContextForPreferredLayoutAttributes:withOriginalAttributes:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContext (UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes);

		[MacCatalyst (13, 1)]
		[Export ("shouldInvalidateLayoutForPreferredLayoutAttributes:withOriginalAttributes:")]
		bool ShouldInvalidateLayout (UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes);


		//
		// Update Support Hooks
		//
		[Export ("prepareForCollectionViewUpdates:")]
		void PrepareForCollectionViewUpdates (UICollectionViewUpdateItem [] updateItems);

		[Export ("finalizeCollectionViewUpdates")]
		void FinalizeCollectionViewUpdates ();

		[Export ("initialLayoutAttributesForAppearingItemAtIndexPath:")]
		UICollectionViewLayoutAttributes InitialLayoutAttributesForAppearingItem (NSIndexPath itemIndexPath);

		[Export ("finalLayoutAttributesForDisappearingItemAtIndexPath:")]
		UICollectionViewLayoutAttributes FinalLayoutAttributesForDisappearingItem (NSIndexPath itemIndexPath);

		[Export ("initialLayoutAttributesForAppearingSupplementaryElementOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes InitialLayoutAttributesForAppearingSupplementaryElement (NSString elementKind, NSIndexPath elementIndexPath);

		[Export ("finalLayoutAttributesForDisappearingSupplementaryElementOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes FinalLayoutAttributesForDisappearingSupplementaryElement (NSString elementKind, NSIndexPath elementIndexPath);

		[Export ("initialLayoutAttributesForAppearingDecorationElementOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes InitialLayoutAttributesForAppearingDecorationElement (NSString elementKind, NSIndexPath decorationIndexPath);

		[Export ("finalLayoutAttributesForDisappearingDecorationElementOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes FinalLayoutAttributesForDisappearingDecorationElement (NSString elementKind, NSIndexPath decorationIndexPath);

		[Export ("finalizeAnimatedBoundsChange")]
		void FinalizeAnimatedBoundsChange ();

		[Static, Export ("layoutAttributesClass")]
		Class LayoutAttributesClass { get; }

		[Static, Export ("invalidationContextClass")]
		Class InvalidationContextClass ();

		[Export ("invalidationContextForBoundsChange:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForBoundsChange (CGRect newBounds);

		[Export ("indexPathsToDeleteForSupplementaryViewOfKind:")]
		NSIndexPath [] GetIndexPathsToDeleteForSupplementaryView (NSString kind);

		[Export ("indexPathsToDeleteForDecorationViewOfKind:")]
		NSIndexPath [] GetIndexPathsToDeleteForDecorationViewOfKind (NSString kind);

		[Export ("indexPathsToInsertForSupplementaryViewOfKind:")]
		NSIndexPath [] GetIndexPathsToInsertForSupplementaryView (NSString kind);

		[Export ("indexPathsToInsertForDecorationViewOfKind:")]
		NSIndexPath [] GetIndexPathsToInsertForDecorationView (NSString kind);

		[Export ("invalidateLayoutWithContext:")]
		void InvalidateLayout (UICollectionViewLayoutInvalidationContext context);

		[Export ("finalizeLayoutTransition")]
		void FinalizeLayoutTransition ();

		[Export ("prepareForTransitionFromLayout:")]
		void PrepareForTransitionFromLayout (UICollectionViewLayout oldLayout);

		[Export ("prepareForTransitionToLayout:")]
		void PrepareForTransitionToLayout (UICollectionViewLayout newLayout);

		[Export ("targetContentOffsetForProposedContentOffset:")]
		CGPoint TargetContentOffsetForProposedContentOffset (CGPoint proposedContentOffset);

		[MacCatalyst (13, 1)]
		[Export ("targetIndexPathForInteractivelyMovingItem:withPosition:")]
		NSIndexPath GetTargetIndexPathForInteractivelyMovingItem (NSIndexPath previousIndexPath, CGPoint position);

		[MacCatalyst (13, 1)]
		[Export ("layoutAttributesForInteractivelyMovingItemAtIndexPath:withTargetPosition:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForInteractivelyMovingItem (NSIndexPath indexPath, CGPoint targetPosition);

		[MacCatalyst (13, 1)]
		[Export ("invalidationContextForInteractivelyMovingItems:withTargetPosition:previousIndexPaths:previousPosition:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForInteractivelyMovingItems (NSIndexPath [] targetIndexPaths, CGPoint targetPosition, NSIndexPath [] previousIndexPaths, CGPoint previousPosition);

		[MacCatalyst (13, 1)]
		[Export ("invalidationContextForEndingInteractiveMovementOfItemsToFinalIndexPaths:previousIndexPaths:movementCancelled:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForEndingInteractiveMovementOfItems (NSIndexPath [] finalIndexPaths, NSIndexPath [] previousIndexPaths, bool movementCancelled);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("developmentLayoutDirection")]
		UIUserInterfaceLayoutDirection DevelopmentLayoutDirection { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("flipsHorizontallyInOppositeLayoutDirection")]
		bool FlipsHorizontallyInOppositeLayoutDirection { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewLayoutAttributes : UIDynamicItem, NSCopying {
		[Export ("frame")]
		CGRect Frame { get; set; }

		[Export ("center")]
		new CGPoint Center { get; set; }

		[Export ("size")]
		CGSize Size { get; set; }

		[Export ("transform3D")]
		CATransform3D Transform3D { get; set; }

		[Export ("alpha")]
		nfloat Alpha { get; set; }

		[Export ("zIndex")]
		nint ZIndex { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("indexPath", ArgumentSemantic.Retain)]
		NSIndexPath IndexPath { get; set; }

		[Export ("representedElementCategory")]
		UICollectionElementCategory RepresentedElementCategory { get; }

		[Export ("representedElementKind")]
		string RepresentedElementKind { get; }

		[Static]
		[Export ("layoutAttributesForCellWithIndexPath:")]
		UICollectionViewLayoutAttributes CreateForCell (NSIndexPath indexPath);

		[Static]
		[Export ("layoutAttributesForDecorationViewOfKind:withIndexPath:")]
		UICollectionViewLayoutAttributes CreateForDecorationView (NSString kind, NSIndexPath indexPath);

		[Static]
		[Export ("layoutAttributesForSupplementaryViewOfKind:withIndexPath:")]
		UICollectionViewLayoutAttributes CreateForSupplementaryView (NSString kind, NSIndexPath indexPath);

		[Export ("bounds")]
		new CGRect Bounds { get; set; }

		[Export ("transform")]
		new CGAffineTransform Transform { get; set; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewLayoutInvalidationContext {
		[Export ("invalidateDataSourceCounts")]
		bool InvalidateDataSourceCounts { get; }

		[Export ("invalidateEverything")]
		bool InvalidateEverything { get; }

		[MacCatalyst (13, 1)]
		[Export ("invalidatedItemIndexPaths")]
		NSIndexPath [] InvalidatedItemIndexPaths { get; }

		[MacCatalyst (13, 1)]
		[Export ("invalidatedSupplementaryIndexPaths")]
		NSDictionary InvalidatedSupplementaryIndexPaths { get; }

		[MacCatalyst (13, 1)]
		[Export ("invalidatedDecorationIndexPaths")]
		NSDictionary InvalidatedDecorationIndexPaths { get; }

		[MacCatalyst (13, 1)]
		[Export ("contentOffsetAdjustment")]
		CGPoint ContentOffsetAdjustment { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("contentSizeAdjustment")]
		CGSize ContentSizeAdjustment { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("invalidateItemsAtIndexPaths:")]
		void InvalidateItems (NSIndexPath [] indexPaths);

		[MacCatalyst (13, 1)]
		[Export ("invalidateSupplementaryElementsOfKind:atIndexPaths:")]
		void InvalidateSupplementaryElements (NSString elementKind, NSIndexPath [] indexPaths);

		[MacCatalyst (13, 1)]
		[Export ("invalidateDecorationElementsOfKind:atIndexPaths:")]
		void InvalidateDecorationElements (NSString elementKind, NSIndexPath [] indexPaths);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("previousIndexPathsForInteractivelyMovingItems")]
		NSIndexPath [] PreviousIndexPathsForInteractivelyMovingItems { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetIndexPathsForInteractivelyMovingItems")]
		NSIndexPath [] TargetIndexPathsForInteractivelyMovingItems { get; }

		[MacCatalyst (13, 1)]
		[Export ("interactiveMovementTarget")]
		CGPoint InteractiveMovementTarget { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewLayoutInvalidationContext))]
	partial interface UICollectionViewFlowLayoutInvalidationContext {
		[Export ("invalidateFlowLayoutDelegateMetrics")]
		bool InvalidateFlowLayoutDelegateMetrics { get; set; }

		[Export ("invalidateFlowLayoutAttributes")]
		bool InvalidateFlowLayoutAttributes { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewLayout))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: -[UICollectionViewTransitionLayout init] is not a valid initializer - use -initWithCurrentLayout:nextLayout: instead
	interface UICollectionViewTransitionLayout : NSCoding {
		[Export ("currentLayout")]
		UICollectionViewLayout CurrentLayout { get; }

		[Export ("nextLayout")]
		UICollectionViewLayout NextLayout { get; }

		[DesignatedInitializer]
		[Export ("initWithCurrentLayout:nextLayout:")]
		[PostGet ("CurrentLayout")]
		[PostGet ("NextLayout")]
		NativeHandle Constructor (UICollectionViewLayout currentLayout, UICollectionViewLayout newLayout);

		[Export ("updateValue:forAnimatedKey:")]
		void UpdateValue (nfloat value, string animatedKey);

		[Export ("valueForAnimatedKey:")]
		nfloat GetValueForAnimatedKey (string animatedKey);

		[Export ("transitionProgress", ArgumentSemantic.Assign)]
		nfloat TransitionProgress { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewUpdateItem {
		[NullAllowed]
		[Export ("indexPathBeforeUpdate")]
		NSIndexPath IndexPathBeforeUpdate { get; }

		[NullAllowed]
		[Export ("indexPathAfterUpdate")]
		NSIndexPath IndexPathAfterUpdate { get; }

		[Export ("updateAction")]
		UICollectionUpdateAction UpdateAction { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UICollectionElementKindSectionKey {
		[Field ("UICollectionElementKindSectionHeader")]
		NSString Header { get; }

		[Field ("UICollectionElementKindSectionFooter")]
		NSString Footer { get; }
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	// returns NIL handle causing exceptions in further calls, e.g. ToString
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -CGColor not defined for the UIColor <UIPlaceholderColor: 0x114f5ad0>; need to first convert colorspace.
	[DisableDefaultCtor]
	interface UIColor : NSSecureCoding, NSCopying
#if !TVOS && !WATCH
		, NSItemProviderWriting, NSItemProviderReading
#endif
	{
		[Export ("colorWithWhite:alpha:")]
		[Static]
		UIColor FromWhiteAlpha (nfloat white, nfloat alpha);

		[Export ("colorWithHue:saturation:brightness:alpha:")]
		[Static]
		UIColor FromHSBA (nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);

		[Export ("colorWithRed:green:blue:alpha:")]
		[Static]
		UIColor FromRGBA (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("colorWithCGColor:")]
		[Static]
		UIColor FromCGColor (CGColor color);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorNamed:")]
		[return: NullAllowed]
		UIColor FromName (string name);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorNamed:inBundle:compatibleWithTraitCollection:")]
		[return: NullAllowed]
		UIColor FromName (string name, [NullAllowed] NSBundle inBundle, [NullAllowed] UITraitCollection compatibleWithTraitCollection);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithDisplayP3Red:green:blue:alpha:")]
		UIColor FromDisplayP3 (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("colorWithPatternImage:")]
		[Static]
		UIColor FromPatternImage (UIImage image);

		[Export ("initWithRed:green:blue:alpha:")]
		NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("initWithPatternImage:")]
		NativeHandle Constructor (UIImage patternImage);

		[Export ("initWithWhite:alpha:")]
		NativeHandle Constructor (nfloat white, nfloat alpha);

		// [Export ("initWithHue:saturation:brightness:alpha:")]
		// NativeHandle Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);
		// 
		// This method is not bound as a constructor because the binding already has a constructor that
		// takes 4 doubles (RGBA constructor) meaning that we would need to use an enum to diff between them making the API
		// uglier when it is not needed. The developer can use colorWithHue:saturation:brightness:alpha:
		// instead.

		[Export ("initWithCGColor:")]
		NativeHandle Constructor (CGColor color);

		[Static]
		[Export ("clearColor")]
		UIColor Clear { get; }

		[Static]
		[Export ("blackColor")]
		UIColor Black { get; }

		[Static]
		[Export ("darkGrayColor")]
		UIColor DarkGray { get; }

		[Static]
		[Export ("lightGrayColor")]
		UIColor LightGray { get; }

		[Static]
		[Export ("whiteColor")]
		UIColor White { get; }

		[Static]
		[Export ("grayColor")]
		UIColor Gray { get; }

		[Static]
		[Export ("redColor")]
		UIColor Red { get; }

		[Static]
		[Export ("greenColor")]
		UIColor Green { get; }

		[Static]
		[Export ("blueColor")]
		UIColor Blue { get; }

		[Static]
		[Export ("cyanColor")]
		UIColor Cyan { get; }

		[Static]
		[Export ("yellowColor")]
		UIColor Yellow { get; }

		[Static]
		[Export ("magentaColor")]
		UIColor Magenta { get; }

		[Static]
		[Export ("orangeColor")]
		UIColor Orange { get; }

		[Static]
		[Export ("purpleColor")]
		UIColor Purple { get; }

		[Static]
		[Export ("brownColor")]
		UIColor Brown { get; }

		[Export ("set")]
		void SetColor ();

		[Export ("setFill")]
		void SetFill ();

		[Export ("setStroke")]
		void SetStroke ();

		[Export ("colorWithAlphaComponent:")]
		UIColor ColorWithAlpha (nfloat alpha);

		[Export ("CGColor")]
		CGColor CGColor { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("CIColor")]
		CIColor CIColor { get; }

#if !NET
		[Obsolete ("Use 'LightText' instead.")]
		[NoWatch]
		[NoTV]
		[Export ("lightTextColor")]
		[Static]
		UIColor LightTextColor { get; }
#endif

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("lightTextColor")]
		[Static]
		UIColor LightText { get; }

#if !NET
		[Obsolete ("Use 'DarkText' instead.")]
		[NoWatch]
		[NoTV]
		[Export ("darkTextColor")]
		[Static]
		UIColor DarkTextColor { get; }
#endif

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("darkTextColor")]
		[Static]
		UIColor DarkText { get; }

#if !NET
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SystemGroupedBackground' instead.")]
		[NoWatch]
		[NoTV]
		[Export ("groupTableViewBackgroundColor")]
		[Static]
		UIColor GroupTableViewBackgroundColor { get; }
#endif

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SystemGroupedBackground' instead.")]
		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SystemGroupedBackground' instead.")]
		[Export ("groupTableViewBackgroundColor")]
		[Static]
		UIColor GroupTableViewBackground { get; }

#if !NET
		[Obsolete ("Use 'ViewFlipsideBackground' instead.")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[Export ("viewFlipsideBackgroundColor")]
		[Static]
		UIColor ViewFlipsideBackgroundColor { get; }
#endif

		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("viewFlipsideBackgroundColor")]
		[Static]
		UIColor ViewFlipsideBackground { get; }

#if !NET
		[Obsolete ("Use 'ScrollViewTexturedBackground' instead.")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[Export ("scrollViewTexturedBackgroundColor")]
		[Static]
		UIColor ScrollViewTexturedBackgroundColor { get; }
#endif

		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("scrollViewTexturedBackgroundColor")]
		[Static]
		UIColor ScrollViewTexturedBackground { get; }

#if !NET
		[Obsolete ("Use 'UnderPageBackground' instead.")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[Static, Export ("underPageBackgroundColor")]
		UIColor UnderPageBackgroundColor { get; }
#endif

		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Static, Export ("underPageBackgroundColor")]
		UIColor UnderPageBackground { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static, Export ("colorWithCIColor:")]
		UIColor FromCIColor (CIColor color);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithCIColor:")]
		NativeHandle Constructor (CIColor ciColor);

		[Export ("getWhite:alpha:")]
		bool GetWhite (out nfloat white, out nfloat alpha);

#if false
		// for testing the managed implementations
		[Export ("getHue:saturation:brightness:alpha:")]
		bool GetHSBA (out nfloat hue, out nfloat saturation, out nfloat brightness, out nfloat alpha);
		
		[Export ("getRed:green:blue:alpha:")]
		bool GetRGBA2 (out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);
#endif

		// From the NSItemProviderReading protocol, a static method.
		[Static]
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string [] ReadableTypeIdentifiers { get; }

		// From the NSItemProviderReading protocol, a static method.
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
#if !WATCH && !TVOS
		new
#endif
		UIColor GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol, a static method.
		// NSItemProviderWriting doesn't seem to be implemented for tvOS/watchOS, even though the headers say otherwise.
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string [] WritableTypeIdentifiers { get; }

		// From UIColor (DynamicColors)

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("colorWithDynamicProvider:")]
		UIColor FromDynamicProvider (Func<UITraitCollection, UIColor> dynamicProvider);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithDynamicProvider:")]
		NativeHandle Constructor (Func<UITraitCollection, UIColor> dynamicProvider);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("resolvedColorWithTraitCollection:")]
		UIColor GetResolvedColor (UITraitCollection traitCollection);

		// From: UIColor (UIColorSystemColors)
		// this probably needs bindings to be moved into from appkit.cs to xkit.cs
		// and adjust accordingly since a lot of those are static properties
		// that cannot be exposed from a [Category]

#if !NET
		[Obsolete ("Use 'SystemRed' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemRedColor")]
		UIColor SystemRedColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemRedColor")]
		UIColor SystemRed { get; }

#if !NET
		[Obsolete ("Use 'SystemGreen' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemGreenColor")]
		UIColor SystemGreenColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGreenColor")]
		UIColor SystemGreen { get; }

#if !NET
		[Obsolete ("Use 'SystemBlue' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemBlueColor")]
		UIColor SystemBlueColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemBlueColor")]
		UIColor SystemBlue { get; }

#if !NET
		[Obsolete ("Use 'SystemOrange' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemOrangeColor")]
		UIColor SystemOrangeColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemOrangeColor")]
		UIColor SystemOrange { get; }

#if !NET
		[Obsolete ("Use 'SystemYellow' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemYellowColor")]
		UIColor SystemYellowColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemYellowColor")]
		UIColor SystemYellow { get; }

#if !NET
		[Obsolete ("Use 'SystemPink' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemPinkColor")]
		UIColor SystemPinkColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemPinkColor")]
		UIColor SystemPink { get; }

#if !NET
		[Obsolete ("Use 'SystemPurple' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemPurpleColor")]
		UIColor SystemPurpleColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemPurpleColor")]
		UIColor SystemPurple { get; }

#if !NET
		[Obsolete ("Use 'SystemTeal' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemTealColor")]
		UIColor SystemTealColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemTealColor")]
		UIColor SystemTeal { get; }

#if !NET
		[Obsolete ("Use 'SystemIndigo' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("systemIndigoColor")]
		UIColor SystemIndigoColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemIndigoColor")]
		UIColor SystemIndigo { get; }

#if !NET
		[Obsolete ("Use 'SystemBrown' instead.")]
		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemBrownColor")]
		UIColor SystemBrownColor { get; }
#endif

		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemBrownColor")]
		UIColor SystemBrown { get; }

#if !NET
		[Obsolete ("Use 'SystemMint' instead.")]
		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemMintColor")]
		UIColor SystemMintColor { get; }
#endif

		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemMintColor")]
		UIColor SystemMint { get; }

#if !NET
		[Obsolete ("Use 'SystemCyan' instead.")]
		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemCyanColor")]
		UIColor SystemCyanColor { get; }
#endif

		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemCyanColor")]
		UIColor SystemCyan { get; }

#if !NET
		[Obsolete ("Use 'SystemGray' instead.")]
		[NoWatch]
		[Static]
		[Export ("systemGrayColor")]
		UIColor SystemGrayColor { get; }
#endif

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGrayColor")]
		UIColor SystemGray { get; }

#if !NET
		[Obsolete ("Use 'SystemGray2' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGray2Color")]
		UIColor SystemGray2Color { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGray2Color")]
		UIColor SystemGray2 { get; }

#if !NET
		[Obsolete ("Use 'SystemGray3' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGray3Color")]
		UIColor SystemGray3Color { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGray3Color")]
		UIColor SystemGray3 { get; }

#if !NET
		[Obsolete ("Use 'SystemGray4' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGray4Color")]
		UIColor SystemGray4Color { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGray4Color")]
		UIColor SystemGray4 { get; }

#if !NET
		[Obsolete ("Use 'SystemGray5' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGray5Color")]
		UIColor SystemGray5Color { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGray5Color")]
		UIColor SystemGray5 { get; }

#if !NET
		[Obsolete ("Use 'SystemGray6' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGray6Color")]
		UIColor SystemGray6Color { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGray6Color")]
		UIColor SystemGray6 { get; }

#if !NET
		[Obsolete ("Use 'Tint' instead.")]
		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("tintColor")]
		UIColor TintColor { get; }
#endif

		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("tintColor")]
		UIColor Tint { get; }

#if !NET
		[Obsolete ("Use 'Label' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("labelColor")]
		UIColor LabelColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("labelColor")]
		UIColor Label { get; }

#if !NET
		[Obsolete ("Use 'SecondaryLabel' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("secondaryLabelColor")]
		UIColor SecondaryLabelColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("secondaryLabelColor")]
		UIColor SecondaryLabel { get; }

#if !NET
		[Obsolete ("Use 'TertiaryLabel' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("tertiaryLabelColor")]
		UIColor TertiaryLabelColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tertiaryLabelColor")]
		UIColor TertiaryLabel { get; }

#if !NET
		[Obsolete ("Use 'QuaternaryLabel' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("quaternaryLabelColor")]
		UIColor QuaternaryLabelColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("quaternaryLabelColor")]
		UIColor QuaternaryLabel { get; }

#if !NET
		[Obsolete ("Use 'Link' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("linkColor")]
		UIColor LinkColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("linkColor")]
		UIColor Link { get; }

#if !NET
		[Obsolete ("Use 'PlaceholderText' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("placeholderTextColor")]
		UIColor PlaceholderTextColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("placeholderTextColor")]
		UIColor PlaceholderText { get; }

#if !NET
		[Obsolete ("Use 'Separator' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("separatorColor")]
		UIColor SeparatorColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("separatorColor")]
		UIColor Separator { get; }

#if !NET
		[Obsolete ("Use 'OpaqueSeparator' instead.")]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[Static]
		[Export ("opaqueSeparatorColor")]
		UIColor OpaqueSeparatorColor { get; }
#endif

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("opaqueSeparatorColor")]
		UIColor OpaqueSeparator { get; }

#if !NET
		[Obsolete ("Use 'SystemBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemBackgroundColor")]
		UIColor SystemBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemBackgroundColor")]
		UIColor SystemBackground { get; }

#if !NET
		[Obsolete ("Use 'SecondarySystemBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("secondarySystemBackgroundColor")]
		UIColor SecondarySystemBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("secondarySystemBackgroundColor")]
		UIColor SecondarySystemBackground { get; }

#if !NET
		[Obsolete ("Use 'TertiarySystemBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("tertiarySystemBackgroundColor")]
		UIColor TertiarySystemBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tertiarySystemBackgroundColor")]
		UIColor TertiarySystemBackground { get; }

#if !NET
		[Obsolete ("Use 'SystemGroupedBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemGroupedBackgroundColor")]
		UIColor SystemGroupedBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemGroupedBackgroundColor")]
		UIColor SystemGroupedBackground { get; }

#if !NET
		[Obsolete ("Use 'SecondarySystemGroupedBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("secondarySystemGroupedBackgroundColor")]
		UIColor SecondarySystemGroupedBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("secondarySystemGroupedBackgroundColor")]
		UIColor SecondarySystemGroupedBackground { get; }

#if !NET
		[Obsolete ("Use 'TertiarySystemGroupedBackground' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("tertiarySystemGroupedBackgroundColor")]
		UIColor TertiarySystemGroupedBackgroundColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tertiarySystemGroupedBackgroundColor")]
		UIColor TertiarySystemGroupedBackground { get; }

#if !NET
		[Obsolete ("Use 'SystemFill' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("systemFillColor")]
		UIColor SystemFillColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemFillColor")]
		UIColor SystemFill { get; }

#if !NET
		[Obsolete ("Use 'SecondarySystemFill' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("secondarySystemFillColor")]
		UIColor SecondarySystemFillColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("secondarySystemFillColor")]
		UIColor SecondarySystemFill { get; }

#if !NET
		[Obsolete ("Use 'TertiarySystemFill' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("tertiarySystemFillColor")]
		UIColor TertiarySystemFillColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tertiarySystemFillColor")]
		UIColor TertiarySystemFill { get; }

#if !NET
		[Obsolete ("Use 'QuaternarySystemFill' instead.")]
		[NoWatch, NoTV, iOS (13, 0)]
		[Static]
		[Export ("quaternarySystemFillColor")]
		UIColor QuaternarySystemFillColor { get; }
#endif

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("quaternarySystemFillColor")]
		UIColor QuaternarySystemFill { get; }

		// UIColor (UIAccessibility) Category

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("accessibilityName")]
		string AccessibilityName { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior),
		   Delegates = new string [] { "CollisionDelegate" },
		   Events = new Type [] { typeof (UICollisionBehaviorDelegate) })]
	interface UICollisionBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		NativeHandle Constructor ([Params] IUIDynamicItem [] items);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("collisionMode")]
		UICollisionBehaviorMode CollisionMode { get; set; }

		[Export ("translatesReferenceBoundsIntoBoundary")]
		bool TranslatesReferenceBoundsIntoBoundary { get; set; }

		[Export ("boundaryIdentifiers", ArgumentSemantic.Copy)]
		NSObject [] BoundaryIdentifiers { get; }

		[Export ("collisionDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakCollisionDelegate { get; set; }

		[Wrap ("WeakCollisionDelegate")]
		[Protocolize]
		UICollisionBehaviorDelegate CollisionDelegate { get; set; }

		[Export ("addItem:")]
		void AddItem (IUIDynamicItem dynamicItem);

		[Export ("removeItem:")]
		void RemoveItem (IUIDynamicItem dynamicItem);

		[Export ("setTranslatesReferenceBoundsIntoBoundaryWithInsets:")]
		void SetTranslatesReferenceBoundsIntoBoundaryWithInsets (UIEdgeInsets insets);

		[Export ("addBoundaryWithIdentifier:forPath:")]
		[PostGet ("BoundaryIdentifiers")]
		void AddBoundary (NSObject boundaryIdentifier, UIBezierPath bezierPath);

		[Export ("addBoundaryWithIdentifier:fromPoint:toPoint:")]
		[PostGet ("BoundaryIdentifiers")]
		void AddBoundary (NSObject boundaryIdentifier, CGPoint fromPoint, CGPoint toPoint);

		[Export ("boundaryWithIdentifier:")]
		UIBezierPath GetBoundary (NSObject boundaryIdentifier);

		[Export ("removeBoundaryWithIdentifier:")]
		void RemoveBoundary (NSObject boundaryIdentifier);

		[Export ("removeAllBoundaries")]
		void RemoveAllBoundaries ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UICollisionBehaviorDelegate {
		[Export ("collisionBehavior:beganContactForItem:withItem:atPoint:")]
		[EventArgs ("UICollisionBeganContact")]
		void BeganContact (UICollisionBehavior behavior, IUIDynamicItem firstItem, IUIDynamicItem secondItem, CGPoint atPoint);

		[Export ("collisionBehavior:endedContactForItem:withItem:")]
		[EventArgs ("UICollisionEndedContact")]
		void EndedContact (UICollisionBehavior behavior, IUIDynamicItem firstItem, IUIDynamicItem secondItem);

		[Export ("collisionBehavior:beganContactForItem:withBoundaryIdentifier:atPoint:")]
		[EventArgs ("UICollisionBeganBoundaryContact")]
		void BeganBoundaryContact (UICollisionBehavior behavior, IUIDynamicItem dynamicItem, [NullAllowed] NSObject boundaryIdentifier, CGPoint atPoint);

		[Export ("collisionBehavior:endedContactForItem:withBoundaryIdentifier:")]
		[EventArgs ("UICollisionEndedBoundaryContact")]
		void EndedBoundaryContact (UICollisionBehavior behavior, IUIDynamicItem dynamicItem, [NullAllowed] NSObject boundaryIdentifier);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: do not call -[UIDocument init] - the designated initializer is -[UIDocument initWithFileURL:
	[DisableDefaultCtor]
	[ThreadSafe]
	interface UIDocument : NSFilePresenter, NSProgressReporting, UIUserActivityRestoring {
		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get; }

		[Export ("fileType", ArgumentSemantic.Copy)]
		string FileType { get; }

		[Export ("fileModificationDate", ArgumentSemantic.Copy)]
		NSDate FileModificationDate { get; set; }

		[Export ("documentState")]
		UIDocumentState DocumentState { get; }

		[DesignatedInitializer]
		[Export ("initWithFileURL:")]
		[PostGet ("FileUrl")]
		NativeHandle Constructor (NSUrl url);

		[Export ("fileURL")]
		NSUrl FileUrl { get; }

		[Export ("openWithCompletionHandler:")]
		[Async]
		void Open ([NullAllowed] UIOperationHandler completionHandler);

		[Export ("closeWithCompletionHandler:")]
		[Async]
		void Close ([NullAllowed] UIOperationHandler completionHandler);

		[Export ("loadFromContents:ofType:error:")]
		bool LoadFromContents (NSObject contents, [NullAllowed] string typeName, out NSError outError);

		[Export ("contentsForType:error:")]
		NSObject ContentsForType (string typeName, out NSError outError);

		[Export ("disableEditing")]
		void DisableEditing ();

		[Export ("enableEditing")]
		void EnableEditing ();

		[Export ("undoManager", ArgumentSemantic.Retain)]
		NSUndoManager UndoManager { get; set; }

		[Export ("hasUnsavedChanges")]
		bool HasUnsavedChanges { get; }

		[Export ("updateChangeCount:")]
		void UpdateChangeCount (UIDocumentChangeKind change);

		[Export ("changeCountTokenForSaveOperation:")]
		NSObject ChangeCountTokenForSaveOperation (UIDocumentSaveOperation saveOperation);

		[Export ("updateChangeCountWithToken:forSaveOperation:")]
		void UpdateChangeCount (NSObject changeCountToken, UIDocumentSaveOperation saveOperation);

		[Export ("saveToURL:forSaveOperation:completionHandler:")]
		[Async]
		void Save (NSUrl url, UIDocumentSaveOperation saveOperation, [NullAllowed] UIOperationHandler completionHandler);

		[Export ("autosaveWithCompletionHandler:")]
		[Async]
		void AutoSave ([NullAllowed] UIOperationHandler completionHandler);

		[Export ("savingFileType")]
		string SavingFileType { get; }

		[Export ("fileNameExtensionForType:saveOperation:")]
		string GetFileNameExtension ([NullAllowed] string typeName, UIDocumentSaveOperation saveOperation);

		[Export ("writeContents:andAttributes:safelyToURL:forSaveOperation:error:")]
		bool WriteContents (NSObject contents, [NullAllowed] NSDictionary additionalFileAttributes, NSUrl url, UIDocumentSaveOperation saveOperation, out NSError outError);

		[Export ("writeContents:toURL:forSaveOperation:originalContentsURL:error:")]
		bool WriteContents (NSObject contents, NSUrl toUrl, UIDocumentSaveOperation saveOperation, [NullAllowed] NSUrl originalContentsURL, out NSError outError);

		[Export ("fileAttributesToWriteToURL:forSaveOperation:error:")]
		NSDictionary GetFileAttributesToWrite (NSUrl forUrl, UIDocumentSaveOperation saveOperation, out NSError outError);

		[Export ("readFromURL:error:")]
		bool Read (NSUrl fromUrl, out NSError outError);

		[Export ("performAsynchronousFileAccessUsingBlock:")]
		[Async]
		void PerformAsynchronousFileAccess (/* non null*/ Action action);

		[Export ("handleError:userInteractionPermitted:")]
		void HandleError (NSError error, bool userInteractionPermitted);

		[Export ("finishedHandlingError:recovered:")]
		void FinishedHandlingError (NSError error, bool recovered);

		[Export ("userInteractionNoLongerPermittedForError:")]
		void UserInteractionNoLongerPermittedForError (NSError error);

		[Export ("revertToContentsOfURL:completionHandler:")]
		[Async]
		void RevertToContentsOfUrl (NSUrl url, [NullAllowed] UIOperationHandler completionHandler);

		[Field ("UIDocumentStateChangedNotification")]
		[Notification]
		NSString StateChangedNotification { get; }

		// ActivityContinuation Category
		[MacCatalyst (13, 1)]
		[Export ("userActivity", ArgumentSemantic.Retain)]
		NSUserActivity UserActivity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity userActivity);

		[MacCatalyst (13, 1)]
		[Field ("NSUserActivityDocumentURLKey")]
		NSString UserActivityDocumentUrlKey { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UIDynamicAnimatorDelegate {
#if !NET
		[Abstract]
#endif
		[Export ("dynamicAnimatorWillResume:")]
		void WillResume (UIDynamicAnimator animator);

#if !NET
		[Abstract]
#endif
		[Export ("dynamicAnimatorDidPause:")]
		void DidPause (UIDynamicAnimator animator);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIDynamicAnimator {
		[DesignatedInitializer]
		[Export ("initWithReferenceView:")]
		NativeHandle Constructor (UIView referenceView);

		[Export ("referenceView")]
		UIView ReferenceView { get; }

		[Export ("behaviors", ArgumentSemantic.Copy)]
		UIDynamicBehavior [] Behaviors { get; }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDynamicAnimatorDelegate Delegate { get; set; }

		[Export ("addBehavior:")]
		[PostGet ("Behaviors")]
		void AddBehavior ([NullAllowed] UIDynamicBehavior behavior);

		[Export ("removeBehavior:")]
		[PostGet ("Behaviors")]
		void RemoveBehavior ([NullAllowed] UIDynamicBehavior behavior);

		[Export ("removeAllBehaviors")]
		[PostGet ("Behaviors")]
		void RemoveAllBehaviors ();

		[Export ("itemsInRect:")]
		IUIDynamicItem [] GetDynamicItems (CGRect rect);

		[Export ("elapsedTime")]
		double ElapsedTime { get; }

		[Export ("updateItemUsingCurrentState:")]
		void UpdateItemUsingCurrentState (IUIDynamicItem uiDynamicItem);

		//
		// From UIDynamicAnimator (UICollectionViewAdditions)
		//
		[Export ("initWithCollectionViewLayout:")]
		NativeHandle Constructor (UICollectionViewLayout layout);

		[Export ("layoutAttributesForCellAtIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForCell (NSIndexPath cellIndexPath);

		[Export ("layoutAttributesForSupplementaryViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForSupplementaryView (NSString viewKind, NSIndexPath viewIndexPath);

		[Export ("layoutAttributesForDecorationViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForDecorationView (NSString viewKind, NSIndexPath viewIndexPath);

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior))]
	interface UIDynamicItemBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		NativeHandle Constructor ([Params] IUIDynamicItem [] items);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("elasticity")]
		nfloat Elasticity { get; set; }

		[Export ("friction")]
		nfloat Friction { get; set; }

		[Export ("density")]
		nfloat Density { get; set; }

		[Export ("resistance")]
		nfloat Resistance { get; set; }

		[Export ("angularResistance")]
		nfloat AngularResistance { get; set; }

		[Export ("allowsRotation")]
		bool AllowsRotation { get; set; }

		[Export ("addItem:")]
		[PostGet ("Items")]
		void AddItem (IUIDynamicItem dynamicItem);

		[Export ("removeItem:")]
		[PostGet ("Items")]
		void RemoveItem (IUIDynamicItem dynamicItem);

		[Export ("addLinearVelocity:forItem:")]
		void AddLinearVelocityForItem (CGPoint velocity, IUIDynamicItem dynamicItem);

		[Export ("linearVelocityForItem:")]
		CGPoint GetLinearVelocityForItem (IUIDynamicItem dynamicItem);

		[Export ("addAngularVelocity:forItem:")]
		void AddAngularVelocityForItem (nfloat velocity, IUIDynamicItem dynamicItem);

		[Export ("angularVelocityForItem:")]
		nfloat GetAngularVelocityForItem (IUIDynamicItem dynamicItem);

		[MacCatalyst (13, 1)]
		[Export ("charge", ArgumentSemantic.Assign)]
		nfloat Charge { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("anchored")]
		bool Anchored { [Bind ("isAnchored")] get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UIDynamicItem {
		[Abstract]
		[Export ("center")]
		CGPoint Center { get; set; }

		[Abstract]
		[Export ("bounds")]
		CGRect Bounds { get; }

		[Abstract]
		[Export ("transform")]
		CGAffineTransform Transform { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("collisionBoundsType")]
		UIDynamicItemCollisionBoundsType CollisionBoundsType { get; }

		[MacCatalyst (13, 1)]
		[Export ("collisionBoundingPath")]
		UIBezierPath CollisionBoundingPath { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIDynamicItemGroup : UIDynamicItem {
		[Export ("initWithItems:")]
		NativeHandle Constructor (IUIDynamicItem [] items);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }
	}


	interface IUIDynamicItem { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIDynamicBehavior {
		[Export ("childBehaviors", ArgumentSemantic.Copy)]
		UIDynamicBehavior [] ChildBehaviors { get; }

		[NullAllowed] // by default this property is null
		[Export ("action", ArgumentSemantic.Copy)]
		Action Action { get; set; }

		[Export ("addChildBehavior:")]
		[PostGet ("ChildBehaviors")]
		void AddChildBehavior (UIDynamicBehavior behavior);

		[Export ("removeChildBehavior:")]
		[PostGet ("ChildBehaviors")]
		void RemoveChildBehavior (UIDynamicBehavior behavior);

		[Export ("dynamicAnimator")]
		UIDynamicAnimator DynamicAnimator { get; }

		[Export ("willMoveToAnimator:")]
		void WillMoveToAnimator ([NullAllowed] UIDynamicAnimator targetAnimator);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior))]
	[DisableDefaultCtor]
	interface UIFieldBehavior {
		[Export ("addItem:")]
		void AddItem (IUIDynamicItem item);

		[Export ("removeItem:")]
		void RemoveItem (IUIDynamicItem item);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("position", ArgumentSemantic.Assign)]
		CGPoint Position { get; set; }

		[Export ("region", ArgumentSemantic.Strong)]
		UIRegion Region { get; set; }

		[Export ("strength", ArgumentSemantic.Assign)]
		nfloat Strength { get; set; }

		[Export ("falloff", ArgumentSemantic.Assign)]
		nfloat Falloff { get; set; }

		[Export ("minimumRadius", ArgumentSemantic.Assign)]
		nfloat MinimumRadius { get; set; }

		[Export ("direction", ArgumentSemantic.Assign)]
		CGVector Direction { get; set; }

		[Export ("smoothness", ArgumentSemantic.Assign)]
		nfloat Smoothness { get; set; }

		[Export ("animationSpeed", ArgumentSemantic.Assign)]
		nfloat AnimationSpeed { get; set; }

		[Static]
		[Export ("dragField")]
		UIFieldBehavior CreateDragField ();

		[Static]
		[Export ("vortexField")]
		UIFieldBehavior CreateVortexField ();

		[Static]
		[Export ("radialGravityFieldWithPosition:")]
		UIFieldBehavior CreateRadialGravityField (CGPoint position);

		[Static]
		[Export ("linearGravityFieldWithVector:")]
		UIFieldBehavior CreateLinearGravityField (CGVector direction);

		[Static]
		[Export ("velocityFieldWithVector:")]
		UIFieldBehavior CreateVelocityField (CGVector direction);

		[Static]
		[Export ("noiseFieldWithSmoothness:animationSpeed:")]
		UIFieldBehavior CreateNoiseField (nfloat smoothness, nfloat speed);

		[Static]
		[Export ("turbulenceFieldWithSmoothness:animationSpeed:")]
		UIFieldBehavior CreateTurbulenceField (nfloat smoothness, nfloat speed);

		[Static]
		[Export ("springField")]
		UIFieldBehavior CreateSpringField ();

		[Static]
		[Export ("electricField")]
		UIFieldBehavior CreateElectricField ();

		[Static]
		[Export ("magneticField")]
		UIFieldBehavior CreateMagneticField ();

		[Static]
		[Export ("fieldWithEvaluationBlock:")]
		UIFieldBehavior CreateCustomField (UIFieldCustomEvaluator evaluator);
	}

	delegate CGVector UIFieldCustomEvaluator (UIFieldBehavior field, CGPoint position, CGVector velocity, nfloat mass, nfloat charge, double deltaTime);

	[Static]
	[MacCatalyst (13, 1)]
	interface UIFontWeightConstants {
		[Field ("UIFontWeightUltraLight")]
		nfloat UltraLight { get; }
		[Field ("UIFontWeightThin")]
		nfloat Thin { get; }
		[Field ("UIFontWeightLight")]
		nfloat Light { get; }
		[Field ("UIFontWeightRegular")]
		nfloat Regular { get; }
		[Field ("UIFontWeightMedium")]
		nfloat Medium { get; }
		[Field ("UIFontWeightSemibold")]
		nfloat Semibold { get; }
		[Field ("UIFontWeightBold")]
		nfloat Bold { get; }
		[Field ("UIFontWeightHeavy")]
		nfloat Heavy { get; }
		[Field ("UIFontWeightBlack")]
		nfloat Black { get; }
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DisableDefaultCtor] // iOS7 -> Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -[UIFont ctFontRef]: unrecognized selector sent to instance 0x15b283c0
						 // note: because of bug 25511 (managed Dispose / native semi-factory) we need to return a copy of the UIFont for every static method that returns an UIFont
	interface UIFont : NSCopying, NSSecureCoding {
		[Static]
		[Export ("systemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _SystemFontOfSize (nfloat size);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // we prefer to show the one using the enum
		[Internal] // bug 25511
		[Static]
		[Export ("systemFontOfSize:weight:")]
		IntPtr _SystemFontOfSize (nfloat size, nfloat weight);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // we prefer to show the one using the enum
		[Internal] // bug 25511
		[Static]
		[Export ("monospacedDigitSystemFontOfSize:weight:")]
		IntPtr _MonospacedDigitSystemFontOfSize (nfloat fontSize, nfloat weight);

		[Static]
		[Export ("boldSystemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _BoldSystemFontOfSize (nfloat size);

		[Static]
		[Export ("italicSystemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _ItalicSystemFontOfSize (nfloat size);

		[Static]
		[Export ("fontWithName:size:")]
		[Internal] // bug 25511
		IntPtr _FromName (string name, nfloat size);

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Internal] // bug https://xamarin.github.io/bugzilla-archives/25/25511/bug.html
		[Export ("monospacedSystemFontOfSize:weight:")]
		IntPtr _MonospacedSystemFontOfSize (nfloat fontSize, nfloat weight);

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("labelFontSize")]
		nfloat LabelFontSize { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("buttonFontSize")]
		nfloat ButtonFontSize { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("smallSystemFontSize")]
		nfloat SmallSystemFontSize { get; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemFontSize")]
		nfloat SystemFontSize { get; }

		[Export ("fontWithSize:")]
		[Internal] // bug 25511
		IntPtr _WithSize (nfloat size);

		[Export ("familyName", ArgumentSemantic.Retain)]
		string FamilyName { get; }

		[Export ("fontName", ArgumentSemantic.Retain)]
		string Name { get; }

		[Export ("pointSize")]
		nfloat PointSize { get; }

		[Export ("ascender")]
		nfloat Ascender { get; }

		[Export ("descender")]
		nfloat Descender { get; }

		[Export ("leading")]
		nfloat Leading { get; }

		[Export ("capHeight")]
		nfloat CapHeight { get; }

		[Export ("xHeight")]
		nfloat XHeight { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'XHeight' property instead.")]
		[Wrap ("XHeight", IsVirtual = true)]
		nfloat xHeight { get; }
#endif

		[Export ("lineHeight")]
		nfloat LineHeight { get; }

		[Static]
		[Export ("familyNames")]
		string [] FamilyNames { get; }

		[Static]
		[Export ("fontNamesForFamilyName:")]
		string [] FontNamesForFamilyName (string familyName);

		[Export ("fontDescriptor")]
		UIFontDescriptor FontDescriptor { get; }

		[Static, Export ("fontWithDescriptor:size:")]
		[Internal] // bug 25511
		IntPtr _FromDescriptor (UIFontDescriptor descriptor, nfloat pointSize);

		[Static, Export ("preferredFontForTextStyle:")]
		[Internal] // bug 25511
		IntPtr _GetPreferredFontForTextStyle (NSString uiFontTextStyle);

		// FIXME the API is present but UITraitCollection is not exposed / rdar 27785753
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("preferredFontForTextStyle:compatibleWithTraitCollection:")]
		[Internal]
		IntPtr _GetPreferredFontForTextStyle (NSString uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);

		[Watch (9, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Internal]
		[Export ("systemFontOfSize:weight:width:")]
		IntPtr _SystemFontOfSize (nfloat fontSize, nfloat weight, nfloat width);

	}

	public enum UIFontTextStyle {
		[Field ("UIFontTextStyleHeadline")]
		Headline,

		[Field ("UIFontTextStyleBody")]
		Body,

		[Field ("UIFontTextStyleSubheadline")]
		Subheadline,

		[Field ("UIFontTextStyleFootnote")]
		Footnote,

		[Field ("UIFontTextStyleCaption1")]
		Caption1,

		[Field ("UIFontTextStyleCaption2")]
		Caption2,

		[MacCatalyst (13, 1)]
		[Field ("UIFontTextStyleTitle1")]
		Title1,

		[MacCatalyst (13, 1)]
		[Field ("UIFontTextStyleTitle2")]
		Title2,

		[MacCatalyst (13, 1)]
		[Field ("UIFontTextStyleTitle3")]
		Title3,

		[MacCatalyst (13, 1)]
		[Field ("UIFontTextStyleCallout")]
		Callout,

		[NoTV]
		[Watch (5, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UIFontTextStyleLargeTitle")]
		LargeTitle,

		[iOS (17, 0), TV (17, 0), Watch (10, 0), MacCatalyst (17, 0)]
		[Field ("UIFontTextStyleExtraLargeTitle")]
		ExtraLargeTitle,

		[iOS (17, 0), TV (17, 0), Watch (10, 0), MacCatalyst (17, 0)]
		[Field ("UIFontTextStyleExtraLargeTitle2")]
		ExtraLargeTitle2,
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	partial interface UIFontDescriptor : NSSecureCoding, NSCopying {

		[Export ("postscriptName")]
		string PostscriptName { get; }

		[Export ("pointSize")]
		nfloat PointSize { get; }

		[Export ("matrix")]
		CGAffineTransform Matrix { get; }

		[Export ("symbolicTraits")]
		UIFontDescriptorSymbolicTraits SymbolicTraits { get; }

		[Export ("objectForKey:")]
		NSObject GetObject (NSString anAttribute);

		[Export ("fontAttributes")]
		NSDictionary WeakFontAttributes { get; }

		[Wrap ("WeakFontAttributes")]
		UIFontAttributes FontAttributes { get; }

		[Export ("matchingFontDescriptorsWithMandatoryKeys:")]
		UIFontDescriptor [] GetMatchingFontDescriptors ([NullAllowed] NSSet mandatoryKeys);

		[Static, Export ("fontDescriptorWithFontAttributes:")]
		UIFontDescriptor FromAttributes (NSDictionary attributes);

		[Static, Wrap ("FromAttributes (attributes.GetDictionary ()!)")]
		UIFontDescriptor FromAttributes (UIFontAttributes attributes);

		[Static, Export ("fontDescriptorWithName:size:")]
		UIFontDescriptor FromName (string fontName, nfloat size);

		[Static, Export ("fontDescriptorWithName:matrix:")]
		UIFontDescriptor FromName (string fontName, CGAffineTransform matrix);

		[Static, Export ("preferredFontDescriptorWithTextStyle:")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (NSString uiFontTextStyle);

		[Static]
		[Wrap ("GetPreferredDescriptorForTextStyle (uiFontTextStyle.GetConstant ()!)")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (UIFontTextStyle uiFontTextStyle);

		// FIXME the API is present but UITraitCollection is not exposed / rdar #27785753
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("preferredFontDescriptorWithTextStyle:compatibleWithTraitCollection:")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (NSString uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetPreferredDescriptorForTextStyle (uiFontTextStyle.GetConstant ()!, traitCollection)")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (UIFontTextStyle uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);

		[DesignatedInitializer]
		[Export ("initWithFontAttributes:")]
		NativeHandle Constructor (NSDictionary attributes);

		[DesignatedInitializer]
		[Wrap ("this (attributes.GetDictionary ()!)")]
		NativeHandle Constructor (UIFontAttributes attributes);

		[Export ("fontDescriptorByAddingAttributes:")]
		UIFontDescriptor CreateWithAttributes (NSDictionary attributes);

		[Wrap ("CreateWithAttributes (attributes.GetDictionary ()!)")]
		UIFontDescriptor CreateWithAttributes (UIFontAttributes attributes);

		[Export ("fontDescriptorWithSymbolicTraits:")]
		UIFontDescriptor CreateWithTraits (UIFontDescriptorSymbolicTraits symbolicTraits);

		[iOS (13, 0), TV (13, 0)]
		[Watch (5, 2)]
		[MacCatalyst (13, 1)]
		[Export ("fontDescriptorWithDesign:")]
		[return: NullAllowed]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		UIFontDescriptor CreateWithDesign (NSString design);

		[iOS (13, 0), TV (13, 0)]
		[Watch (5, 2)]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
		[Wrap ("CreateWithDesign (design.GetConstant ()!)")]
		UIFontDescriptor CreateWithDesign (UIFontDescriptorSystemDesign design);

		[Export ("fontDescriptorWithSize:")]
		UIFontDescriptor CreateWithSize (nfloat newPointSize);

		[Export ("fontDescriptorWithMatrix:")]
		UIFontDescriptor CreateWithMatrix (CGAffineTransform matrix);

		[Export ("fontDescriptorWithFace:")]
		UIFontDescriptor CreateWithFace (string newFace);

		[Export ("fontDescriptorWithFamily:")]
		UIFontDescriptor CreateWithFamily (string newFamily);


		//
		// Internal fields
		//
		[Internal, Field ("UIFontDescriptorFamilyAttribute")]
		NSString FamilyAttribute { get; }

		[Internal, Field ("UIFontDescriptorNameAttribute")]
		NSString NameAttribute { get; }

		[Internal, Field ("UIFontDescriptorFaceAttribute")]
		NSString FaceAttribute { get; }

		[Internal, Field ("UIFontDescriptorSizeAttribute")]
		NSString SizeAttribute { get; }

		[Internal, Field ("UIFontDescriptorVisibleNameAttribute")]
		NSString VisibleNameAttribute { get; }

		[Internal, Field ("UIFontDescriptorMatrixAttribute")]
		NSString MatrixAttribute { get; }

		[Internal, Field ("UIFontDescriptorCharacterSetAttribute")]
		NSString CharacterSetAttribute { get; }

		[Internal, Field ("UIFontDescriptorCascadeListAttribute")]
		NSString CascadeListAttribute { get; }

		[Internal, Field ("UIFontDescriptorTraitsAttribute")]
		NSString TraitsAttribute { get; }

		[Internal, Field ("UIFontDescriptorFixedAdvanceAttribute")]
		NSString FixedAdvanceAttribute { get; }

		[Internal, Field ("UIFontDescriptorFeatureSettingsAttribute")]
		NSString FeatureSettingsAttribute { get; }

		[Internal, Field ("UIFontDescriptorTextStyleAttribute")]
		NSString TextStyleAttribute { get; }

		[Internal, Field ("UIFontSymbolicTrait")]
		NSString SymbolicTrait { get; }

		[Internal, Field ("UIFontWeightTrait")]
		NSString WeightTrait { get; }

		[Internal, Field ("UIFontWidthTrait")]
		NSString WidthTrait { get; }

		[Internal, Field ("UIFontSlantTrait")]
		NSString SlantTrait { get; }

		[Internal, Field ("UIFontFeatureSelectorIdentifierKey")]
		NSString UIFontFeatureSelectorIdentifierKey { get; }

		[Internal, Field ("UIFontFeatureTypeIdentifierKey")]
		NSString UIFontFeatureTypeIdentifierKey { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIGestureRecognizerDelegate) })]
	[Dispose ("OnDispose ();", Optimizable = true)]
	interface UIGestureRecognizer {
		[DesignatedInitializer]
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("initWithTarget:action:")]
		[Sealed]
		[Internal]
		NativeHandle Constructor (NSObject target, IntPtr /* SEL */ action);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIGestureRecognizerDelegate Delegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("state")]
		UIGestureRecognizerState State { get; [Advice ("Only subclasses of 'UIGestureRecognizer' can set this property.")] set; }

		[Export ("view")]
		[Transient]
		UIView View { get; }

		[Export ("addTarget:action:")]
		void AddTarget (NSObject target, Selector action);

		[Export ("addTarget:action:")]
		[Internal]
		[Sealed]
		void AddTarget (NSObject target, IntPtr action);

		[Export ("removeTarget:action:")]
		void RemoveTarget ([NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("removeTarget:action:")]
		[Internal]
		[Sealed]
		void RemoveTarget ([NullAllowed] NSObject target, IntPtr action);

		[Export ("locationInView:")]
		CGPoint LocationInView ([NullAllowed] UIView view);

		[Export ("cancelsTouchesInView")]
		bool CancelsTouchesInView { get; set; }

		[Export ("delaysTouchesBegan")]
		bool DelaysTouchesBegan { get; set; }

		[Export ("delaysTouchesEnded")]
		bool DelaysTouchesEnded { get; set; }

		[Export ("locationOfTouch:inView:")]
		CGPoint LocationOfTouch (nint touchIndex, [NullAllowed] UIView inView);

		[Export ("numberOfTouches")]
		nint NumberOfTouches { get; }

		[Export ("requireGestureRecognizerToFail:")]
		void RequireGestureRecognizerToFail (UIGestureRecognizer otherGestureRecognizer);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("buttonMask")]
		UIEventButtonMask ButtonMask { get; }

		//
		// These come from the UIGestureRecognizerProtected category, and you should only call
		// these methods from a subclass of UIGestureRecognizer, never externally
		//

		[Export ("ignoreTouch:forEvent:")]
		void IgnoreTouch (UITouch touch, UIEvent forEvent);

		[MacCatalyst (13, 1)]
		[Sealed] // Docs: This method is intended to be called, not overridden.
		[Export ("ignorePress:forEvent:")]
		void IgnorePress (UIPress button, UIPressesEvent @event);

		[Export ("reset")]
		void Reset ();

		[Export ("canPreventGestureRecognizer:")]
		bool CanPreventGestureRecognizer (UIGestureRecognizer preventedGestureRecognizer);

		[Export ("canBePreventedByGestureRecognizer:")]
		bool CanBePreventedByGestureRecognizer (UIGestureRecognizer preventingGestureRecognizer);

		[Export ("touchesBegan:withEvent:")]
		void TouchesBegan (NSSet touches, UIEvent evt);

		[Export ("touchesMoved:withEvent:")]
		void TouchesMoved (NSSet touches, UIEvent evt);

		[Export ("touchesEnded:withEvent:")]
		void TouchesEnded (NSSet touches, UIEvent evt);

		[Export ("touchesCancelled:withEvent:")]
		void TouchesCancelled (NSSet touches, UIEvent evt);

		[Export ("shouldRequireFailureOfGestureRecognizer:")]
		bool ShouldRequireFailureOfGestureRecognizer (UIGestureRecognizer otherGestureRecognizer);

		[Export ("shouldBeRequiredToFailByGestureRecognizer:")]
		bool ShouldBeRequiredToFailByGestureRecognizer (UIGestureRecognizer otherGestureRecognizer);

		[iOS (13, 4), TV (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("shouldReceiveEvent:")]
		bool ShouldReceive (UIEvent @event);

		[MacCatalyst (13, 1)]
		[Export ("touchesEstimatedPropertiesUpdated:")]
		void TouchesEstimatedPropertiesUpdated (NSSet touches);

		// FIXME: likely an array of UITouchType
		[MacCatalyst (13, 1)]
		[Export ("allowedTouchTypes", ArgumentSemantic.Copy)]
		NSNumber [] AllowedTouchTypes { get; set; }

		// FIXME: likely an array of UIPressType
		[MacCatalyst (13, 1)]
		[Export ("allowedPressTypes", ArgumentSemantic.Copy)]
		NSNumber [] AllowedPressTypes { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("requiresExclusiveTouchType")]
		bool RequiresExclusiveTouchType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("pressesBegan:withEvent:")]
		void PressesBegan (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesChanged:withEvent:")]
		void PressesChanged (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesEnded:withEvent:")]
		void PressesEnded (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesCancelled:withEvent:")]
		void PressesCancelled (NSSet<UIPress> presses, UIPressesEvent evt);
	}

	[NoWatch]
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIGestureRecognizerDelegate {
		[Export ("gestureRecognizer:shouldReceiveTouch:"), DefaultValue (true), DelegateName ("UITouchEventArgs")]
		bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch);

		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[Export ("gestureRecognizerShouldBegin:"), DelegateName ("UIGestureProbe"), DefaultValue (true)]
		bool ShouldBegin (UIGestureRecognizer recognizer);

		[Export ("gestureRecognizer:shouldBeRequiredToFailByGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldBeRequiredToFailBy (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[Export ("gestureRecognizer:shouldRequireFailureOfGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldRequireFailureOf (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[MacCatalyst (13, 1)]
		[Export ("gestureRecognizer:shouldReceivePress:"), DelegateName ("UIGesturesPress"), DefaultValue (false)]
		bool ShouldReceivePress (UIGestureRecognizer gestureRecognizer, UIPress press);

		[TV (13, 4), iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("gestureRecognizer:shouldReceiveEvent:"), DelegateName ("UIGesturesEvent"), DefaultValue (true)]
		bool ShouldReceiveEvent (UIGestureRecognizer gestureRecognizer, UIEvent @event);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIGraphicsRendererFormat : NSCopying {
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'PreferredFormat' instead.")]
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsRendererFormat DefaultFormat { get; }

		[Export ("bounds")]
		CGRect Bounds { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("preferredFormat")]
		UIGraphicsRendererFormat PreferredFormat { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIGraphicsRendererContext {
		[Export ("CGContext")]
		CGContext CGContext { get; }

		[Export ("format")]
		UIGraphicsRendererFormat Format { get; }

		[Export ("fillRect:")]
		void FillRect (CGRect rect);

		[Export ("fillRect:blendMode:")]
		void FillRect (CGRect rect, CGBlendMode blendMode);

		[Export ("strokeRect:")]
		void StrokeRect (CGRect rect);

		[Export ("strokeRect:blendMode:")]
		void StrokeRect (CGRect rect, CGBlendMode blendMode);

		[Export ("clipToRect:")]
		void ClipToRect (CGRect rect);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Abstract] // quote form headers "An abstract base class for creating graphics renderers. Do not use this class directly."
	interface UIGraphicsRenderer {
		[Export ("initWithBounds:")]
		NativeHandle Constructor (CGRect bounds);

		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect bounds, UIGraphicsRendererFormat format);

		[Export ("format")]
		UIGraphicsRendererFormat Format { get; }

		[Export ("allowsImageOutput")]
		bool AllowsImageOutput { get; }

		// From UIGraphicsRenderer (UIGraphicsRendererProtected) category

		[Static]
		[Export ("rendererContextClass")]
		Class RendererContextClass { get; }

		[Static]
		[Export ("contextWithFormat:")]
		[return: NullAllowed]
		CGContext GetContext (UIGraphicsRendererFormat format);

		[Static]
		[Export ("prepareCGContext:withRendererContext:")]
		void PrepareContext (CGContext context, UIGraphicsRendererContext rendererContext);

		[Export ("runDrawingActions:completionActions:error:")]
		bool Run (Action<UIGraphicsRendererContext> drawingActions, [NullAllowed] Action<UIGraphicsRendererContext> completionActions, [NullAllowed] out NSError error);
	}

	// Not worth it, Action<UIGraphicsImageRendererContext> conveys more data
	//delegate void UIGraphicsImageDrawingActions (UIGraphicsImageRendererContext context);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRendererFormat))]
	interface UIGraphicsImageRendererFormat {
		[Export ("scale")]
		nfloat Scale { get; set; }

		[Export ("opaque")]
		bool Opaque { get; set; }

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the 'PreferredRange' property instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use the 'PreferredRange' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'PreferredRange' property instead.")]
		[Export ("prefersExtendedRange")]
		bool PrefersExtendedRange { get; set; }

		[New] // kind of overloading a static member, make it return `instancetype`
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsImageRendererFormat DefaultFormat { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("formatForTraitCollection:")]
		UIGraphicsImageRendererFormat GetFormat (UITraitCollection traitCollection);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preferredRange", ArgumentSemantic.Assign)]
		UIGraphicsImageRendererFormatRange PreferredRange { get; set; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("supportsHighDynamicRange")]
		bool SupportsHighDynamicRange { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRendererContext))]
	interface UIGraphicsImageRendererContext {
		[Export ("currentImage")]
		UIImage CurrentImage { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRenderer))]
	interface UIGraphicsImageRenderer {
		[Export ("initWithSize:")]
		NativeHandle Constructor (CGSize size);

		[Export ("initWithSize:format:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGSize size, UIGraphicsImageRendererFormat format);

		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect bounds, UIGraphicsImageRendererFormat format);

		[Export ("imageWithActions:")]
		UIImage CreateImage (Action<UIGraphicsImageRendererContext> actions);

		[Export ("PNGDataWithActions:")]
		NSData CreatePng (Action<UIGraphicsImageRendererContext> actions);

		[Export ("JPEGDataWithCompressionQuality:actions:")]
		NSData CreateJpeg (nfloat compressionQuality, Action<UIGraphicsImageRendererContext> actions);
	}

	// Not worth it, Action<UIGraphicsImageRendererContext> conveys more data
	//delegate void UIGraphicsPdfDrawingActions (UIGraphicsPdfRendererContext context);
	// Action<UIGraphicsPdfRendererContext>

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRendererFormat), Name = "UIGraphicsPDFRendererFormat")]
	interface UIGraphicsPdfRendererFormat {
		[Export ("documentInfo", ArgumentSemantic.Copy)]
		// TODO: add strongly typed binding
		NSDictionary<NSString, NSObject> DocumentInfo { get; set; }

		[New] // kind of overloading a static member, make it return `instancetype`
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsPdfRendererFormat DefaultFormat { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRendererContext), Name = "UIGraphicsPDFRendererContext")]
	interface UIGraphicsPdfRendererContext {
		[Export ("pdfContextBounds")]
		CGRect PdfContextBounds { get; }

		[Export ("beginPage")]
		void BeginPage ();

		[Export ("beginPageWithBounds:pageInfo:")]
		void BeginPage (CGRect bounds, NSDictionary<NSString, NSObject> pageInfo);

		[Export ("setURL:forRect:")]
		void SetUrl (NSUrl url, CGRect rect);

		[Export ("addDestinationWithName:atPoint:")]
		void AddDestination (string name, CGPoint point);

		[Export ("setDestinationWithName:forRect:")]
		void SetDestination (string name, CGRect rect);
	}


	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGraphicsRenderer), Name = "UIGraphicsPDFRenderer")]
	interface UIGraphicsPdfRenderer {
		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect bounds, UIGraphicsPdfRendererFormat format);

		[Export ("writePDFToURL:withActions:error:")]
		bool WritePdf (NSUrl url, Action<UIGraphicsPdfRendererContext> actions, out NSError error);

		[Export ("PDFDataWithActions:")]
		NSData CreatePdf (Action<UIGraphicsPdfRendererContext> actions);
	}

	[BaseType (typeof (UIDynamicBehavior))]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIGravityBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		NativeHandle Constructor ([Params] IUIDynamicItem [] items);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("addItem:")]
		[PostGet ("Items")]
		void AddItem (IUIDynamicItem dynamicItem);

		[Export ("removeItem:")]
		[PostGet ("Items")]
		void RemoveItem (IUIDynamicItem dynamicItem);

		[Export ("gravityDirection")]
		CGVector GravityDirection { get; set; }

		[Export ("angle")]
		nfloat Angle { get; set; }

		[Export ("magnitude")]
		nfloat Magnitude { get; set; }

		[Export ("setAngle:magnitude:")]
		void SetAngleAndMagnitude (nfloat angle, nfloat magnitude);
	}

#if !NET
	// HACK: those members are not *required* in ObjC but we made them
	// abstract to have them inlined in UITextField and UITextView
	// Even more confusing it that respondToSelecttor return NO on them
	// even if it works in _real_ life (compare unit and introspection tests)
#endif
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextInputTraits {
#if !NET
		[Abstract]
#endif
		[Export ("autocapitalizationType")]
		UITextAutocapitalizationType AutocapitalizationType { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("autocorrectionType")]
		UITextAutocorrectionType AutocorrectionType { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("keyboardType")]
		UIKeyboardType KeyboardType { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("keyboardAppearance")]
		UIKeyboardAppearance KeyboardAppearance { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("returnKeyType")]
		UIReturnKeyType ReturnKeyType { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("enablesReturnKeyAutomatically")]
		bool EnablesReturnKeyAutomatically { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("secureTextEntry")]
		bool SecureTextEntry { [Bind ("isSecureTextEntry")] get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("spellCheckingType")]
		UITextSpellCheckingType SpellCheckingType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("textContentType")]
		NSString TextContentType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("smartQuotesType", ArgumentSemantic.Assign)]
		UITextSmartQuotesType SmartQuotesType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("smartDashesType", ArgumentSemantic.Assign)]
		UITextSmartDashesType SmartDashesType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("smartInsertDeleteType", ArgumentSemantic.Assign)]
		UITextSmartInsertDeleteType SmartInsertDeleteType { get; set; }

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("passwordRules", ArgumentSemantic.Copy)]
		UITextInputPasswordRules PasswordRules { get; set; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("inlinePredictionType", ArgumentSemantic.Assign)]
		UITextInlinePredictionType InlinePredictionType { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UIKeyboardEventArgs {
		[Export ("UIKeyboardFrameBeginUserInfoKey")]
		CGRect FrameBegin { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("UIKeyboardFrameEndUserInfoKey")]
		CGRect FrameEnd { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("UIKeyboardAnimationDurationUserInfoKey")]
		double AnimationDuration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("UIKeyboardAnimationCurveUserInfoKey")]
		UIViewAnimationCurve AnimationCurve { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIKeyboard {
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardWillShowNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillShowNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardDidShowNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidShowNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardWillHideNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillHideNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardDidHideNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidHideNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardWillChangeFrameNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillChangeFrameNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardDidChangeFrameNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidChangeFrameNotification { get; }

#if !XAMCORE_3_0
		//
		// Deprecated methods
		//

		[NoTV]
		[Deprecated (PlatformName.iOS, 3, 2)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("UIKeyboardCenterBeginUserInfoKey")]
		NSString CenterBeginUserInfoKey { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 3, 2)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("UIKeyboardCenterEndUserInfoKey")]
		NSString CenterEndUserInfoKey { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 3, 2)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Field ("UIKeyboardBoundsUserInfoKey")]
		NSString BoundsUserInfoKey { get; }
#endif
		//
		// Keys
		//
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardAnimationCurveUserInfoKey")]
		NSString AnimationCurveUserInfoKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardAnimationDurationUserInfoKey")]
		NSString AnimationDurationUserInfoKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardFrameEndUserInfoKey")]
		NSString FrameEndUserInfoKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardFrameBeginUserInfoKey")]
		NSString FrameBeginUserInfoKey { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIKeyboardIsLocalUserInfoKey")]
		NSString IsLocalUserInfoKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICommand))]
	[DesignatedDefaultCtor]
	interface UIKeyCommand {

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("title")]
		string Title { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("input")]
		NSString Input { get; }

		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("propertyList")]
		NSObject PropertyList { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("attributes", ArgumentSemantic.Assign)]
		UIMenuElementAttributes Attributes { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("state", ArgumentSemantic.Assign)]
		UIMenuElementState State { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("alternates")]
		UICommandAlternate [] Alternates { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Export ("wantsPriorityOverSystemBehavior")]
		bool WantsPriorityOverSystemBehavior { get; set; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsAutomaticLocalization")]
		bool AllowsAutomaticLocalization { get; set; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsAutomaticMirroring")]
		bool AllowsAutomaticMirroring { get; set; }

		[Static, Export ("keyCommandWithInput:modifierFlags:action:")]
		UIKeyCommand Create (NSString keyCommandInput, UIKeyModifierFlags modifierFlags, Selector action);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("commandWithTitle:image:action:input:modifierFlags:propertyList:")]
		UIKeyCommand Create (string title, [NullAllowed] UIImage image, Selector action, string input, UIKeyModifierFlags modifierFlags, [NullAllowed] NSObject propertyList);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("commandWithTitle:image:action:input:modifierFlags:propertyList:alternates:")]
		UIKeyCommand Create (string title, [NullAllowed] UIImage image, Selector action, string input, UIKeyModifierFlags modifierFlags, [NullAllowed] NSObject propertyList, UICommandAlternate [] alternates);

		[Field ("UIKeyInputUpArrow")]
		NSString UpArrow { get; }

		[Field ("UIKeyInputDownArrow")]
		NSString DownArrow { get; }

		[Field ("UIKeyInputLeftArrow")]
		NSString LeftArrow { get; }

		[Field ("UIKeyInputRightArrow")]
		NSString RightArrow { get; }

		[Field ("UIKeyInputEscape")]
		NSString Escape { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputPageUp")]
		NSString PageUp { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputPageDown")]
		NSString PageDown { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputHome")]
		NSString Home { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputEnd")]
		NSString End { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF1")]
		NSString F1 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF2")]
		NSString F2 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF3")]
		NSString F3 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF4")]
		NSString F4 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF5")]
		NSString F5 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF6")]
		NSString F6 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF7")]
		NSString F7 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF8")]
		NSString F8 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF9")]
		NSString F9 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF10")]
		NSString F10 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF11")]
		NSString F11 { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[Field ("UIKeyInputF12")]
		NSString F12 { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[Field ("UIKeyInputDelete")]
		NSString Delete { get; }

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIKeyCommand.Create (NSString, UIKeyModifierFlags, Selector)' overload instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'UIKeyCommand.Create (NSString, UIKeyModifierFlags, Selector)' overload instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIKeyCommand.Create (NSString, UIKeyModifierFlags, Selector)' overload instead.")]
		[Static]
		[Export ("keyCommandWithInput:modifierFlags:action:discoverabilityTitle:")]
		UIKeyCommand Create (NSString keyCommandInput, UIKeyModifierFlags modifierFlags, Selector action, NSString discoverabilityTitle);

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("discoverabilityTitle")]
		NSString DiscoverabilityTitle { get; set; }
	}

	interface IUIKeyInput { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIKeyInput : UITextInputTraits {
		[Abstract]
		[Export ("hasText")]
		bool HasText { get; }

		[Abstract]
		[Export ("insertText:")]
		void InsertText (string text);

		[Abstract]
		[Export ("deleteBackward")]
		void DeleteBackward ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextPosition {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextRange {
		[Export ("isEmpty")]
		bool IsEmpty { get; }

		[Export ("start")]
		UITextPosition Start { get; }

		[Export ("end")]
		UITextPosition End { get; }
	}

	interface IUITextInput { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextInput : UIKeyInput {
		[Abstract]
		[NullAllowed] // by default this property is null
					  // This is declared as ArgumentSemantic.Copy, but UITextRange doesn't conform to NSCopying.
					  // Also declaring it as ArgumentSemantic.Copy makes UIKIt crash: https://github.com/xamarin/xamarin-macios/issues/15677
		[Export ("selectedTextRange")]
		UITextRange SelectedTextRange { get; set; }

		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("markedTextStyle", ArgumentSemantic.Copy)]
		NSDictionary MarkedTextStyle { get; set; }

		[Abstract]
		[Export ("beginningOfDocument")]
		UITextPosition BeginningOfDocument { get; }

		[Abstract]
		[Export ("endOfDocument")]
		UITextPosition EndOfDocument { get; }

		[Abstract]
		[Export ("inputDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakInputDelegate { get; set; }

		[Wrap ("WeakInputDelegate")]
		[Protocolize]
		UITextInputDelegate InputDelegate { get; set; }

		[Abstract]
		[Export ("tokenizer")]
		NSObject WeakTokenizer { get; }

		[Wrap ("WeakTokenizer")]
		[Protocolize]
		UITextInputTokenizer Tokenizer { get; }

		[Export ("textInputView")]
		UIView TextInputView { get; }

		[Export ("selectionAffinity")]
		UITextStorageDirection SelectionAffinity { get; set; }

		[Abstract]
		[Export ("textInRange:")]
		string TextInRange (UITextRange range);

		[Abstract]
		[Export ("replaceRange:withText:")]
		void ReplaceText (UITextRange range, string text);

		[Abstract]
		[Export ("markedTextRange")]
		UITextRange MarkedTextRange { get; }

		[Abstract]
		[Export ("setMarkedText:selectedRange:")]
		void SetMarkedText (string markedText, NSRange selectedRange);

		[Abstract]
		[Export ("unmarkText")]
		void UnmarkText ();

		[Abstract]
		[Export ("textRangeFromPosition:toPosition:")]
		UITextRange GetTextRange (UITextPosition fromPosition, UITextPosition toPosition);

		[Abstract]
		[Export ("positionFromPosition:offset:")]
		UITextPosition GetPosition (UITextPosition fromPosition, nint offset);

		[Abstract]
		[Export ("positionFromPosition:inDirection:offset:")]
		UITextPosition GetPosition (UITextPosition fromPosition, UITextLayoutDirection inDirection, nint offset);

		[Abstract]
		[Export ("comparePosition:toPosition:")]
		NSComparisonResult ComparePosition (UITextPosition first, UITextPosition second);

		[Abstract]
		[Export ("offsetFromPosition:toPosition:")]
		nint GetOffsetFromPosition (UITextPosition fromPosition, UITextPosition toPosition);

		[Abstract]
		[Export ("positionWithinRange:farthestInDirection:")]
		UITextPosition GetPositionWithinRange (UITextRange range, UITextLayoutDirection direction);

		[Abstract]
		[Export ("characterRangeByExtendingPosition:inDirection:")]
		UITextRange GetCharacterRange (UITextPosition byExtendingPosition, UITextLayoutDirection direction);

		[Abstract]
		[Export ("baseWritingDirectionForPosition:inDirection:")]
		NSWritingDirection GetBaseWritingDirection (UITextPosition forPosition, UITextStorageDirection direction);

		[Abstract]
		[Export ("setBaseWritingDirection:forRange:")]
		void SetBaseWritingDirectionforRange (NSWritingDirection writingDirection, UITextRange range);

		[Abstract]
		[Export ("firstRectForRange:")]
		CGRect GetFirstRectForRange (UITextRange range);

		[Abstract]
		[Export ("caretRectForPosition:")]
		CGRect GetCaretRectForPosition ([NullAllowed] UITextPosition position);

		[Abstract]
		[Export ("closestPositionToPoint:")]
		UITextPosition GetClosestPositionToPoint (CGPoint point);

		[Abstract]
		[Export ("closestPositionToPoint:withinRange:")]
		UITextPosition GetClosestPositionToPoint (CGPoint point, UITextRange withinRange);

		[Abstract]
		[Export ("characterRangeAtPoint:")]
		UITextRange GetCharacterRangeAtPoint (CGPoint point);

		[Export ("textStylingAtPosition:inDirection:")]
		NSDictionary GetTextStyling (UITextPosition atPosition, UITextStorageDirection inDirection);

		[Export ("positionWithinRange:atCharacterOffset:")]
		UITextPosition GetPosition (UITextRange withinRange, nint atCharacterOffset);

		[Export ("characterOffsetOfPosition:withinRange:")]
		nint GetCharacterOffsetOfPosition (UITextPosition position, UITextRange range);

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'NSAttributedString.BackgroundColorAttributeName'.")]
		[Field ("UITextInputTextBackgroundColorKey")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSAttributedString.BackgroundColorAttributeName'.")]
		NSString TextBackgroundColorKey { get; }

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'NSAttributedString.ForegroundColorAttributeName'.")]
		[Field ("UITextInputTextColorKey")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSAttributedString.ForegroundColorAttributeName'.")]
		NSString TextColorKey { get; }

		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'NSAttributedString.FontAttributeName'.")]
		[Field ("UITextInputTextFontKey")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSAttributedString.FontAttributeName'.")]
		NSString TextFontKey { get; }

		[Field ("UITextInputCurrentInputModeDidChangeNotification")]
		[Notification]
		NSString CurrentInputModeDidChangeNotification { get; }

		[Export ("dictationRecognitionFailed")]
		void DictationRecognitionFailed ();

		[Export ("dictationRecordingDidEnd")]
		void DictationRecordingDidEnd ();

		[Export ("insertDictationResult:")]
		void InsertDictationResult (NSArray dictationResult);

		[Abstract]
		[Export ("selectionRectsForRange:")]
		UITextSelectionRect [] GetSelectionRects (UITextRange range);

		[Export ("shouldChangeTextInRange:replacementText:")]
		bool ShouldChangeTextInRange (UITextRange inRange, string replacementText);

		[Export ("frameForDictationResultPlaceholder:")]
		CGRect GetFrameForDictationResultPlaceholder (NSObject placeholder);

		[Export ("insertDictationResultPlaceholder")]
		NSObject InsertDictationResultPlaceholder ();

		[Export ("removeDictationResultPlaceholder:willInsertResult:")]
		void RemoveDictationResultPlaceholder (NSObject placeholder, bool willInsertResult);

		[MacCatalyst (13, 1)]
		[Export ("beginFloatingCursorAtPoint:")]
		void BeginFloatingCursor (CGPoint point);

		[MacCatalyst (13, 1)]
		[Export ("updateFloatingCursorAtPoint:")]
		void UpdateFloatingCursor (CGPoint point);

		[MacCatalyst (13, 1)]
		[Export ("endFloatingCursor")]
		void EndFloatingCursor ();

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("insertText:alternatives:style:")]
		void InsertText (string text, string [] alternatives, UITextAlternativeStyle style);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setAttributedMarkedText:selectedRange:")]
		void SetAttributedMarkedText ([NullAllowed] NSAttributedString markedText, NSRange selectedRange);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("insertTextPlaceholderWithSize:")]
		UITextPlaceholder InsertTextPlaceholder (CGSize size);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("removeTextPlaceholder:")]
		void RemoveTextPlaceholder (UITextPlaceholder textPlaceholder);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("willPresentEditMenuWithAnimator:")]
		void WillPresentEditMenu (IUIEditMenuInteractionAnimating animator);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("willDismissEditMenuWithAnimator:")]
		void WillDismissEditMenu (IUIEditMenuInteractionAnimating animator);

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("editMenuForTextRange:suggestedActions:")]
		[return: NullAllowed]
		UIMenu GetEditMenu (UITextRange textRange, UIMenuElement [] suggestedActions);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextInputAssistantItem {
		[Export ("allowsHidingShortcuts")]
		bool AllowsHidingShortcuts { get; set; }

		[Export ("leadingBarButtonGroups", ArgumentSemantic.Copy), NullAllowed]
		UIBarButtonItemGroup [] LeadingBarButtonGroups { get; set; }

		[Export ("trailingBarButtonGroups", ArgumentSemantic.Copy), NullAllowed]
		UIBarButtonItemGroup [] TrailingBarButtonGroups { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITextInputTokenizer {
		[Abstract]
		[Export ("rangeEnclosingPosition:withGranularity:inDirection:")]
		UITextRange GetRangeEnclosingPosition (UITextPosition position, UITextGranularity granularity, UITextDirection direction);

		[Abstract]
		[Export ("isPosition:atBoundary:inDirection:")]
		bool ProbeDirection (UITextPosition probePosition, UITextGranularity atBoundary, UITextDirection inDirection);

		[Abstract]
		[Export ("positionFromPosition:toBoundary:inDirection:")]
		UITextPosition GetPosition (UITextPosition fromPosition, UITextGranularity toBoundary, UITextDirection inDirection);

		[Abstract]
		[Export ("isPosition:withinTextUnit:inDirection:")]
		bool ProbeDirectionWithinTextUnit (UITextPosition probePosition, UITextGranularity withinTextUnit, UITextDirection inDirection);

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextInputStringTokenizer : UITextInputTokenizer {
		[Export ("initWithTextInput:")]
		NativeHandle Constructor (IUITextInput textInput);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITextInputDelegate {
		[Abstract]
		[Export ("selectionWillChange:")]
		void SelectionWillChange (IUITextInput uiTextInput);

		[Abstract]
		[Export ("selectionDidChange:")]
		void SelectionDidChange (IUITextInput uiTextInput);

		[Abstract]
		[Export ("textWillChange:")]
		void TextWillChange (IUITextInput textInput);

		[Abstract]
		[Export ("textDidChange:")]
		void TextDidChange (IUITextInput textInput);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextSelectionRect {
		[Export ("rect")]
		CGRect Rect { get; }

		[Export ("writingDirection")]
		NSWritingDirection WritingDirection { get; }

		[Export ("containsStart")]
		bool ContainsStart { get; }

		[Export ("containsEnd")]
		bool ContainsEnd { get; }

		[Export ("isVertical")]
		bool IsVertical { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UILexicon : NSCopying {

		[Export ("entries")]
		UILexiconEntry [] Entries { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UILexiconEntry : NSCopying {

		[Export ("documentText")]
		string DocumentText { get; }

		[Export ("userInput")]
		string UserInput { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UILocalizedIndexedCollation {
		[Export ("sectionTitles")]
		string [] SectionTitles { get; }

		[Export ("sectionIndexTitles")]
		string [] SectionIndexTitles { get; }

		[Static]
		[Export ("currentCollation")]
		UILocalizedIndexedCollation CurrentCollation ();

		[Export ("sectionForSectionIndexTitleAtIndex:")]
		nint GetSectionForSectionIndexTitle (nint indexTitleIndex);

		[Export ("sectionForObject:collationStringSelector:")]
		nint GetSectionForObject (NSObject obj, Selector collationStringSelector);

		[Export ("sortedArrayFromArray:collationStringSelector:")]
		NSObject [] SortedArrayFromArraycollationStringSelector (NSObject [] array, Selector collationStringSelector);
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationRequest' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationRequest' instead.")]
	[DisableDefaultCtor] // designated
	interface UILocalNotification : NSCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("fireDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate FireDate { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSTimeZone TimeZone { get; set; }

		[Export ("repeatInterval")]
		NSCalendarUnit RepeatInterval { get; set; }

		[Export ("repeatCalendar", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSCalendar RepeatCalendar { get; set; }

		[Export ("alertBody", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertBody { get; set; }

		[Export ("hasAction")]
		bool HasAction { get; set; }

		[Export ("alertAction", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertAction { get; set; }

		[Export ("alertLaunchImage", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertLaunchImage { get; set; }

		[Export ("soundName", ArgumentSemantic.Copy)]
		[NullAllowed]
		string SoundName { get; set; }

		[Export ("applicationIconBadgeNumber")]
		nint ApplicationIconBadgeNumber { get; set; }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDictionary UserInfo { get; set; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationSound.DefaultSound' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNNotificationSound.DefaultSound' instead.")]
		[Field ("UILocalNotificationDefaultSoundName")]
		NSString DefaultSoundName { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("regionTriggersOnce", ArgumentSemantic.UnsafeUnretained)]
		bool RegionTriggersOnce { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("category")]
		string Category { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("alertTitle")]
		string AlertTitle { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UILongPressGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }

		[Export ("minimumPressDuration")]
		double MinimumPressDuration { get; set; }

		[Export ("allowableMovement")]
		nfloat AllowableMovement { get; set; }

		[Export ("numberOfTapsRequired")]
		nint NumberOfTapsRequired { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UITapGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("numberOfTapsRequired")]
		nuint NumberOfTapsRequired { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }

		[NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("buttonMaskRequired", ArgumentSemantic.Assign)]
		UIEventButtonMask ButtonMaskRequired { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UIPanGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("minimumNumberOfTouches")]
		nuint MinimumNumberOfTouches { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("maximumNumberOfTouches")]
		nuint MaximumNumberOfTouches { get; set; }

		[Export ("setTranslation:inView:")]
		void SetTranslation (CGPoint translation, [NullAllowed] UIView view);

		[Export ("translationInView:")]
		CGPoint TranslationInView ([NullAllowed] UIView view);

		[Export ("velocityInView:")]
		CGPoint VelocityInView ([NullAllowed] UIView view);

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("allowedScrollTypesMask", ArgumentSemantic.Assign)]
		UIScrollTypeMask AllowedScrollTypesMask { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPanGestureRecognizer))]
	interface UIScreenEdgePanGestureRecognizer {

		// inherit .ctor
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("edges", ArgumentSemantic.Assign)]
		UIRectEdge Edges { get; set; }
	}

	//
	// This class comes with an "init" constructor (which we autogenerate)
	// and does not require us to call this with initWithFrame:
	//
	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UIRefreshControl : UIAppearance {
		[Export ("refreshing")]
		bool Refreshing { [Bind ("isRefreshing")] get; }

		[NullAllowed] // by default this property is null
		[Export ("attributedTitle", ArgumentSemantic.Retain)]
		[Appearance]
		NSAttributedString AttributedTitle { get; set; }

		[Export ("beginRefreshing")]
		void BeginRefreshing ();

		[Export ("endRefreshing")]
		void EndRefreshing ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIRegion : NSCopying, NSCoding {
		[Static]
		[Export ("infiniteRegion")]
		UIRegion Infinite { get; }

		[Export ("initWithRadius:")]
		NativeHandle Constructor (nfloat radius);

		[Export ("initWithSize:")]
		NativeHandle Constructor (CGSize size);

		[Export ("inverseRegion")]
		UIRegion Inverse ();

		[Export ("regionByUnionWithRegion:")]
		UIRegion Union (UIRegion region);

		[Export ("regionByDifferenceFromRegion:")]
		UIRegion Difference (UIRegion region);

		[Export ("regionByIntersectionWithRegion:")]
		UIRegion Intersect (UIRegion region);

		[Export ("containsPoint:")]
		bool Contains (CGPoint point);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UIRotationGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("rotation")]
		nfloat Rotation { get; set; }

		[Export ("velocity")]
		nfloat Velocity { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UIPinchGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("scale")]
		nfloat Scale { get; set; }

		[Export ("velocity")]
		nfloat Velocity { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	interface UISwipeGestureRecognizer {
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[Export ("direction")]
		UISwipeGestureRecognizerDirection Direction { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIActivityIndicatorView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithActivityIndicatorStyle:")]
		NativeHandle Constructor (UIActivityIndicatorViewStyle style);

		[Export ("activityIndicatorViewStyle")]
		UIActivityIndicatorViewStyle ActivityIndicatorViewStyle { get; set; }

		[Export ("hidesWhenStopped")]
		bool HidesWhenStopped { get; set; }

		[Export ("startAnimating")]
		void StartAnimating ();

		[Export ("stopAnimating")]
		void StopAnimating ();

		[Export ("isAnimating")]
		bool IsAnimating { get; }

		[Export ("color", ArgumentSemantic.Retain), NullAllowed]
		[Appearance]
		UIColor Color { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIItemProviderPresentationSizeProviding {
		[Abstract]
		[Export ("preferredPresentationSizeForItemProvider")]
		CGSize PreferredPresentationSizeForItemProvider { get; }
	}

	[BaseType (typeof (NSObject))]
	interface UIImage : NSSecureCoding
#if !WATCH
		, UIAccessibility, UIAccessibilityIdentification
#if !TVOS
		, NSItemProviderWriting, NSItemProviderReading, UIItemProviderPresentationSizeProviding
#endif
#endif // !WATCH
	{
		[ThreadSafe]
		[Export ("initWithContentsOfFile:")]
		NativeHandle Constructor (string filename);

		[ThreadSafe]
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		[ThreadSafe]
		[Export ("size")]
		[Autorelease]
		CGSize Size { get; }

		// Thread-safe in iOS 9 or later according to docs.
#if IOS
		// tvOS started with 9.0 code base (and watchOS 2.0 came later)
		[Advice ("This API is thread-safe only on 9.0 and later.")]
#endif
		[ThreadSafe]
		[Static]
		[Export ("imageNamed:")]
		[Autorelease]
		[return: NullAllowed]
		UIImage FromBundle (string name);

#if !WATCH
		// Thread-safe in iOS 9 or later according to docs.
#if IOS
		// tvOS started with 9.0 code base (and watchOS 2.0 came later)
		[Advice ("This API is thread-safe only on 9.0 and later.")]
#endif
		[ThreadSafe]
		[MacCatalyst (13, 1)]
		[Static, Export ("imageNamed:inBundle:compatibleWithTraitCollection:")]
		[return: NullAllowed]
		UIImage FromBundle (string name, [NullAllowed] NSBundle bundle, [NullAllowed] UITraitCollection traitCollection);
#endif // !WATCH

		[Static]
		[Export ("imageWithContentsOfFile:")]
		[Autorelease]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage FromFile (string filename);

		[Static]
		[Export ("imageWithData:")]
		[Autorelease]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage LoadFromData (NSData data);

		[Static]
		[Export ("imageWithCGImage:")]
		[Autorelease]
		[ThreadSafe]
		UIImage FromImage (CGImage image);

		[Static]
		[Export ("imageWithCGImage:scale:orientation:")]
		[Autorelease]
		[ThreadSafe]
		UIImage FromImage (CGImage image, nfloat scale, UIImageOrientation orientation);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithCIImage:")]
		[Autorelease]
		[ThreadSafe]
		UIImage FromImage (CIImage image);

		// From the NSItemProviderReading protocol, a static method.
		[Static]
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string [] ReadableTypeIdentifiers { get; }

		// From the NSItemProviderReading protocol, a static method.
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[return: NullAllowed]
#if !WATCH && !TVOS
		new
#endif
		UIImage GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		[Export ("renderingMode")]
		[ThreadSafe]
		UIImageRenderingMode RenderingMode { get; }

		[Export ("imageWithRenderingMode:")]
		[ThreadSafe]
		UIImage ImageWithRenderingMode (UIImageRenderingMode renderingMode);

		[Export ("CGImage")]
		[ThreadSafe]
		[NullAllowed]
		CGImage CGImage { get; }

		[Export ("imageOrientation")]
		[ThreadSafe]
		UIImageOrientation Orientation { get; }

		[Export ("drawAtPoint:")]
		[ThreadSafe]
		void Draw (CGPoint point);

		[Export ("drawAtPoint:blendMode:alpha:")]
		[ThreadSafe]
		void Draw (CGPoint point, CGBlendMode blendMode, nfloat alpha);

		[Export ("drawInRect:")]
		[ThreadSafe]
		void Draw (CGRect rect);

		[Export ("drawInRect:blendMode:alpha:")]
		[ThreadSafe]
		void Draw (CGRect rect, CGBlendMode blendMode, nfloat alpha);

		[Export ("drawAsPatternInRect:")]
		[ThreadSafe]
		void DrawAsPatternInRect (CGRect rect);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("stretchableImageWithLeftCapWidth:topCapHeight:")]
		[Autorelease]
		[ThreadSafe]
		UIImage StretchableImage (nint leftCapWidth, nint topCapHeight);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("leftCapWidth")]
		[ThreadSafe]
		nint LeftCapWidth { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("topCapHeight")]
		[ThreadSafe]
		nint TopCapHeight { get; }

		[Export ("scale")]
		[ThreadSafe]
		nfloat CurrentScale { get; }

		[Static, Export ("animatedImageNamed:duration:")]
		[Autorelease]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage CreateAnimatedImage (string name, double duration);

		[Static, Export ("animatedImageWithImages:duration:")]
		[Autorelease]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage CreateAnimatedImage (UIImage [] images, double duration);

		[Static, Export ("animatedResizableImageNamed:capInsets:duration:")]
		[Autorelease]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage CreateAnimatedImage (string name, UIEdgeInsets capInsets, double duration);

		[Export ("initWithCGImage:")]
		[ThreadSafe]
		NativeHandle Constructor (CGImage cgImage);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithCIImage:")]
		[ThreadSafe]
		NativeHandle Constructor (CIImage ciImage);

		[Export ("initWithCGImage:scale:orientation:")]
		[ThreadSafe]
		NativeHandle Constructor (CGImage cgImage, nfloat scale, UIImageOrientation orientation);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("CIImage")]
		[ThreadSafe]
		[NullAllowed]
		CIImage CIImage { get; }

		[Export ("images")]
		[ThreadSafe]
		[NullAllowed]
		UIImage [] Images { get; }

		[Export ("duration")]
		[ThreadSafe]
		double Duration { get; }

		[Export ("resizableImageWithCapInsets:")]
		[Autorelease]
		[ThreadSafe]
		UIImage CreateResizableImage (UIEdgeInsets capInsets);

		[Export ("capInsets")]
		[ThreadSafe]
		UIEdgeInsets CapInsets { get; }

		//
		// 6.0
		//
		[Export ("alignmentRectInsets")]
		[ThreadSafe]
		UIEdgeInsets AlignmentRectInsets { get; }

		[Static]
		[Export ("imageWithData:scale:")]
		[ThreadSafe, Autorelease]
		[return: NullAllowed]
		UIImage LoadFromData (NSData data, nfloat scale);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("imageWithCIImage:scale:orientation:")]
		[ThreadSafe, Autorelease]
		UIImage FromImage (CIImage ciImage, nfloat scale, UIImageOrientation orientation);

		[Export ("initWithData:scale:")]
		[ThreadSafe]
		NativeHandle Constructor (NSData data, nfloat scale);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("initWithCIImage:scale:orientation:")]
		[ThreadSafe]
		NativeHandle Constructor (CIImage ciImage, nfloat scale, UIImageOrientation orientation);

		[Export ("resizableImageWithCapInsets:resizingMode:")]
		[ThreadSafe]
		UIImage CreateResizableImage (UIEdgeInsets capInsets, UIImageResizingMode resizingMode);

		[Static]
		[Export ("animatedResizableImageNamed:capInsets:resizingMode:duration:")]
		[ThreadSafe]
		[return: NullAllowed]
		UIImage CreateAnimatedImage (string name, UIEdgeInsets capInsets, UIImageResizingMode resizingMode, double duration);

		[Export ("imageWithAlignmentRectInsets:")]
		[ThreadSafe, Autorelease]
		UIImage ImageWithAlignmentRectInsets (UIEdgeInsets alignmentInsets);

		[Export ("resizingMode")]
		[ThreadSafe]
		UIImageResizingMode ResizingMode { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("traitCollection")]
		[ThreadSafe]
		UITraitCollection TraitCollection { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("imageAsset")]
		[ThreadSafe]
		[NullAllowed]
		UIImageAsset ImageAsset { get; }

		[MacCatalyst (13, 1)]
		[Export ("imageFlippedForRightToLeftLayoutDirection")]
		UIImage GetImageFlippedForRightToLeftLayoutDirection ();

		[MacCatalyst (13, 1)]
		[Export ("flipsForRightToLeftLayoutDirection")]
		bool FlipsForRightToLeftLayoutDirection { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("imageRendererFormat")]
		UIGraphicsImageRendererFormat ImageRendererFormat { get; }

		[MacCatalyst (13, 1)]
		[Export ("imageWithHorizontallyFlippedOrientation")]
		UIImage GetImageWithHorizontallyFlippedOrientation ();

		// From the NSItemProviderWriting protocol, a static method.
		// NSItemProviderWriting doesn't seem to be implemented for tvOS/watchOS, even though the headers say otherwise.
		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string [] WritableTypeIdentifiers { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemImageNamed:")]
		[return: NullAllowed]
		UIImage GetSystemImage (string name);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemImageNamed:withConfiguration:")]
		[return: NullAllowed]
		UIImage GetSystemImage (string name, [NullAllowed] UIImageConfiguration configuration);

		[NoWatch, TV (13, 0), iOS (13, 0)] // UITraitCollection is not available on watch, it has been reported before.
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemImageNamed:compatibleWithTraitCollection:")]
		[return: NullAllowed]
		UIImage GetSystemImage (string name, [NullAllowed] UITraitCollection traitCollection);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[ThreadSafe]
		[Export ("imageNamed:inBundle:withConfiguration:")]
		[return: NullAllowed]
		UIImage FromBundle (string name, [NullAllowed] NSBundle bundle, [NullAllowed] UIImageConfiguration configuration);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("symbolImage")]
		bool SymbolImage { [Bind ("isSymbolImage")] get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("baselineOffsetFromBottom")]
		nfloat BaselineOffsetFromBottom { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasBaseline")]
		bool HasBaseline { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithBaselineOffsetFromBottom:")]
		UIImage GetImageFromBottom (nfloat baselineOffset);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithoutBaseline")]
		UIImage GetImageWithoutBaseline ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("configuration", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIImageConfiguration Configuration { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithConfiguration:")]
		UIImage ApplyConfiguration (UIImageConfiguration configuration);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("symbolConfiguration", ArgumentSemantic.Copy)]
		UIImageSymbolConfiguration SymbolConfiguration { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageByApplyingSymbolConfiguration:")]
		[return: NullAllowed]
		UIImage ApplyConfiguration (UIImageSymbolConfiguration configuration);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithTintColor:")]
		UIImage ApplyTintColor (UIColor color);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithTintColor:renderingMode:")]
		UIImage ApplyTintColor (UIColor color, UIImageRenderingMode renderingMode);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("imageByPreparingForDisplay")]
		[return: NullAllowed]
		UIImage GetImageByPreparingForDisplay ();

		[Async]
		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("prepareForDisplayWithCompletionHandler:")]
		void PrepareForDisplay (Action<UIImage> completionHandler);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("imageByPreparingThumbnailOfSize:")]
		[return: NullAllowed]
		UIImage GetImageByPreparingThumbnail (CGSize ofSize);

		[Async]
		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("prepareThumbnailOfSize:completionHandler:")]
		void PrepareThumbnail (CGSize OfSize, Action<UIImage> completionHandler);

		// Inlined from UIImage (PreconfiguredSystemImages)

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("actionsImage", ArgumentSemantic.Strong)]
		UIImage ActionsImage { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("addImage", ArgumentSemantic.Strong)]
		UIImage AddImage { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("removeImage", ArgumentSemantic.Strong)]
		UIImage RemoveImage { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("checkmarkImage", ArgumentSemantic.Strong)]
		UIImage CheckmarkImage { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("strokedCheckmarkImage", ArgumentSemantic.Strong)]
		UIImage StrokedCheckmarkImage { get; }

		[Watch (9, 0), TV (16, 0), MacCatalyst (16, 0), iOS (16, 0)]
		[Static]
		[Export ("systemImageNamed:variableValue:withConfiguration:")]
		[return: NullAllowed]
		UIImage GetSystemImage (string name, double value, [NullAllowed] UIImageConfiguration configuration);

		[Watch (9, 0), TV (16, 0), MacCatalyst (16, 0), iOS (16, 0)]
		[Static]
		[Export ("imageNamed:inBundle:variableValue:withConfiguration:")]
		[return: NullAllowed]
		UIImage FromBundle (string name, [NullAllowed] NSBundle bundle, double value, [NullAllowed] UIImageConfiguration configuration);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("isHighDynamicRange")]
		bool IsHighDynamicRange { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("imageRestrictedToStandardDynamicRange")]
		UIImage ImageRestrictedToStandardDynamicRange { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIImageConfiguration : NSCopying, NSSecureCoding {
		[NoWatch] // UITraitCollection is not available in WatchOS it has been reported before
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }

		[NoWatch] // UITraitCollection is not available in WatchOS it has been reported before
		[MacCatalyst (13, 1)]
		[Export ("configurationWithTraitCollection:")]
		UIImageConfiguration GetConfiguration ([NullAllowed] UITraitCollection traitCollection);

		[Export ("configurationByApplyingConfiguration:")]
		UIImageConfiguration GetConfiguration ([NullAllowed] UIImageConfiguration otherConfiguration);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("locale")]
		NSLocale Locale { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("configurationWithLocale:")]
		UIImageConfiguration GetConfiguration ([NullAllowed] NSLocale locale);

		[Static]
		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("configurationWithLocale:")]
		UIImageConfiguration FromConfiguration ([NullAllowed] NSLocale locale);

		[NoWatch, TV (17, 0), iOS (17, 0)]
		[Static]
		[Export ("configurationWithTraitCollection:")]
		UIImageConfiguration ConfigurationWithTraitCollection ([NullAllowed] UITraitCollection traitCollection);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIImageConfiguration))]
	interface UIImageSymbolConfiguration {

		[Static]
		[Export ("unspecifiedConfiguration")]
		UIImageSymbolConfiguration UnspecifiedConfiguration { get; }

		[Static]
		[Export ("configurationWithScale:")]
		UIImageSymbolConfiguration Create (UIImageSymbolScale scale);

		[Static]
		[Export ("configurationWithPointSize:")]
		UIImageSymbolConfiguration Create (nfloat pointSize);

		[Static]
		[Export ("configurationWithWeight:")]
		UIImageSymbolConfiguration Create (UIImageSymbolWeight weight);

		[Static]
		[Export ("configurationWithPointSize:weight:")]
		UIImageSymbolConfiguration Create (nfloat pointSize, UIImageSymbolWeight weight);

		[Static]
		[Export ("configurationWithPointSize:weight:scale:")]
		UIImageSymbolConfiguration Create (nfloat pointSize, UIImageSymbolWeight weight, UIImageSymbolScale scale);

		[Static]
		[Export ("configurationWithTextStyle:")]
		UIImageSymbolConfiguration Create ([BindAs (typeof (UIFontTextStyle))] NSString textStyle);

		[Static]
		[Export ("configurationWithTextStyle:scale:")]
		UIImageSymbolConfiguration Create ([BindAs (typeof (UIFontTextStyle))] NSString textStyle, UIImageSymbolScale scale);

		[Static]
		[Export ("configurationWithFont:")]
		UIImageSymbolConfiguration Create (UIFont font);

		[Static]
		[Export ("configurationWithFont:scale:")]
		UIImageSymbolConfiguration Create (UIFont font, UIImageSymbolScale scale);

		[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("configurationWithHierarchicalColor:")]
		UIImageSymbolConfiguration Create (UIColor hierarchicalColor);

		[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("configurationWithPaletteColors:")]
		UIImageSymbolConfiguration Create (UIColor [] paletteColors);

		[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("configurationPreferringMulticolor")]
		UIImageSymbolConfiguration ConfigurationPreferringMulticolor { get; }

		[Export ("configurationWithoutTextStyle")]
		UIImageSymbolConfiguration ConfigurationWithoutTextStyle { get; }

		[Export ("configurationWithoutScale")]
		UIImageSymbolConfiguration ConfigurationWithoutScale { get; }

		[Export ("configurationWithoutWeight")]
		UIImageSymbolConfiguration ConfigurationWithoutWeight { get; }

		[Export ("configurationWithoutPointSizeAndWeight")]
		UIImageSymbolConfiguration ConfigurationWithoutPointSizeAndWeight { get; }

		[Export ("isEqualToConfiguration:")]
		bool IsEqualTo ([NullAllowed] UIImageSymbolConfiguration otherConfiguration);

		[Watch (9, 0), TV (16, 0), MacCatalyst (16, 0), iOS (16, 0)]
		[Static]
		[Export ("configurationPreferringMonochrome")]
		UIImageSymbolConfiguration GetConfigurationPreferringMonochrome ();
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIMenuElement))]
	[DisableDefaultCtor]
	interface UIMenu {

		[BindAs (typeof (UIMenuIdentifier))]
		[Export ("identifier")]
		NSString Identifier { get; }

		[Export ("options")]
		UIMenuOptions Options { get; }

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Export ("selectedElements")]
#if XAMCORE_5_0
		UIMenuElement [] SelectedElements { get; }
#else
		[Internal]
		UIMenuElement [] _SelectedElements { get; }
#endif

		[Export ("children")]
		UIMenuElement [] Children { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("menuWithChildren:")]
		UIMenu Create (UIMenuElement [] children);

		[Static]
		[Export ("menuWithTitle:children:")]
		UIMenu Create (string title, UIMenuElement [] children);

		[Static]
		[Export ("menuWithTitle:image:identifier:options:children:")]
		UIMenu Create (string title, [NullAllowed] UIImage image, [NullAllowed][BindAs (typeof (UIMenuIdentifier))] NSString identifier, UIMenuOptions options, UIMenuElement [] children);

		[Export ("menuByReplacingChildren:")]
		UIMenu GetMenuByReplacingChildren (UIMenuElement [] newChildren);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredElementSize", ArgumentSemantic.Assign)]
		UIMenuElementSize PreferredElementSize { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIMenuElement : NSCopying, NSSecureCoding, UIAccessibilityIdentification {

		[Export ("title")]
		string Title { get; }

		[iOS (15, 0), TV (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NullAllowed, Export ("image")]
		UIImage Image { get; }
	}

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIPreviewParameters : NSCopying {

		[Export ("initWithTextLineRects:")]
		NativeHandle Constructor (NSValue [] textLineRects);

		[NullAllowed, Export ("visiblePath", ArgumentSemantic.Copy)]
		UIBezierPath VisiblePath { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("shadowPath", ArgumentSemantic.Copy)]
		UIBezierPath ShadowPath { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPreviewTarget : NSCopying {

		[Export ("initWithContainer:center:transform:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView container, CGPoint center, CGAffineTransform transform);

		[Export ("initWithContainer:center:")]
		NativeHandle Constructor (UIView container, CGPoint center);

		[Export ("container")]
		UIView Container { get; }

		[Export ("center")]
		CGPoint Center { get; }

		[Export ("transform")]
		CGAffineTransform Transform { get; }
	}

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITargetedPreview : NSCopying {

		[Export ("initWithView:parameters:target:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView view, UIPreviewParameters parameters, UIPreviewTarget target);

		[Export ("initWithView:parameters:")]
		NativeHandle Constructor (UIView view, UIPreviewParameters parameters);

		[Export ("initWithView:")]
		NativeHandle Constructor (UIView view);

		[Export ("target")]
		UIPreviewTarget Target { get; }

		[Export ("view")]
		UIView View { get; }

		[Export ("parameters", ArgumentSemantic.Copy)]
		UIPreviewParameters Parameters { get; }

		[Export ("size")]
		CGSize Size { get; }

		[Export ("retargetedPreviewWithTarget:")]
		UITargetedPreview GetRetargetedPreview (UIPreviewTarget newTarget);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface UIImageAsset : NSSecureCoding {

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageWithConfiguration:")]
		UIImage FromConfiguration (UIImageConfiguration configuration);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("registerImage:withConfiguration:")]
		void RegisterImage (UIImage image, UIImageConfiguration configuration);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("unregisterImageWithConfiguration:")]
		void UnregisterImage (UIImageConfiguration configuration);

		[Export ("imageWithTraitCollection:")]
		UIImage FromTraitCollection (UITraitCollection traitCollection);

		[Export ("registerImage:withTraitCollection:")]
		void RegisterImage (UIImage image, UITraitCollection traitCollection);

		[Export ("unregisterImageWithTraitCollection:")]
		void UnregisterImageWithTraitCollection (UITraitCollection traitCollection);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIEvent {
		[Export ("type")]
		UIEventType Type { get; }

		[Export ("subtype")]
		UIEventSubtype Subtype { get; }

		[Export ("timestamp")]
		double Timestamp { get; }

		[TV (13, 4), NoWatch, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("buttonMask")]
		UIEventButtonMask ButtonMask { get; }

		[Export ("allTouches")]
		NSSet AllTouches { get; }

		[Export ("touchesForView:")]
		NSSet TouchesForView (UIView view);

		[Export ("touchesForWindow:")]
		NSSet TouchesForWindow (UIWindow window);


		[Export ("touchesForGestureRecognizer:")]
		NSSet TouchesForGestureRecognizer (UIGestureRecognizer window);

		[MacCatalyst (13, 1)]
		[Export ("coalescedTouchesForTouch:")]
		[return: NullAllowed]
		UITouch [] GetCoalescedTouches (UITouch touch);

		[MacCatalyst (13, 1)]
		[Export ("predictedTouchesForTouch:")]
		[return: NullAllowed]
		UITouch [] GetPredictedTouches (UITouch touch);
	}

	// that's one of the few enums based on CGFloat - we expose the [n]float directly in the API
	// but we need a way to give access to the constants to developers
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIWindowLevel {
		[Field ("UIWindowLevelNormal")]
		nfloat Normal { get; }

		[Field ("UIWindowLevelAlert")]
		nfloat Alert { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIWindowLevelStatusBar")]
		nfloat StatusBar { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIWindow {

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithWindowScene:")]
		NativeHandle Constructor (UIWindowScene windowScene);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("windowScene", ArgumentSemantic.Weak)]
		UIWindowScene WindowScene { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("canResizeToFitContent")]
		bool CanResizeToFitContent { get; [Bind ("setCanResizeToFitContent:")] set; }

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("makeKeyAndVisible")]
		void MakeKeyAndVisible ();

		[Export ("makeKeyWindow")]
		void MakeKeyWindow ();

		[Export ("becomeKeyWindow")]
		void BecomeKeyWindow ();

		[Export ("resignKeyWindow")]
		void ResignKeyWindow ();

		[Export ("isKeyWindow")]
		bool IsKeyWindow { get; }

		[Export ("windowLevel")]
		nfloat WindowLevel { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("canBecomeKeyWindow")]
		bool CanBecomeKeyWindow { get; }

		[Export ("convertPoint:fromWindow:")]
		CGPoint ConvertPointFromWindow (CGPoint point, [NullAllowed] UIWindow window);

		[Export ("convertPoint:toWindow:")]
		CGPoint ConvertPointToWindow (CGPoint point, [NullAllowed] UIWindow window);

		[Export ("convertRect:fromWindow:")]
		CGRect ConvertRectFromWindow (CGRect rect, [NullAllowed] UIWindow window);

		[Export ("convertRect:toWindow:")]
		CGRect ConvertRectToWindow (CGRect rect, [NullAllowed] UIWindow window);

		[Export ("sendEvent:")]
		void SendEvent (UIEvent evt);

		[NullAllowed] // by default this property is null
		[Export ("rootViewController", ArgumentSemantic.Retain)]
		UIViewController RootViewController { get; set; }


		[Export ("screen", ArgumentSemantic.Retain)]
		UIScreen Screen {
			get;

			[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WindowScene' instead.")]
			[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'WindowScene' instead.")]
			[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WindowScene' instead.")]
			set;
		}

		[Field ("UIWindowDidBecomeVisibleNotification")]
		[Notification]
		NSString DidBecomeVisibleNotification { get; }

		[Field ("UIWindowDidBecomeHiddenNotification")]
		[Notification]
		NSString DidBecomeHiddenNotification { get; }

		[Field ("UIWindowDidBecomeKeyNotification")]
		[Notification]
		NSString DidBecomeKeyNotification { get; }

		[Field ("UIWindowDidResignKeyNotification")]
		[Notification]
		NSString DidResignKeyNotification { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("safeAreaAspectFitLayoutGuide", ArgumentSemantic.Strong)]
		IUILayoutGuideAspectFitting SafeAreaAspectFitLayoutGuide { get; }
	}

	delegate void UIControlEnumerateEventsIteratorHandler ([NullAllowed] UIAction actionHandler, [NullAllowed] NSObject target, [NullAllowed] Selector action, UIControlEvent controlEvents, out bool stop);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIControl : UIContextMenuInteractionDelegate {
		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frame);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithFrame:primaryAction:")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] UIAction primaryAction);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("contentVerticalAlignment")]
		UIControlContentVerticalAlignment VerticalAlignment { get; set; }

		[Export ("contentHorizontalAlignment")]
		UIControlContentHorizontalAlignment HorizontalAlignment { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("effectiveContentHorizontalAlignment")]
		UIControlContentHorizontalAlignment EffectiveContentHorizontalAlignment { get; }

		[Export ("state")]
		UIControlState State { get; }

		[Export ("isTracking")]
		bool Tracking { get; }

		[Export ("isTouchInside")]
		bool TouchInside { get; }

		[Export ("beginTrackingWithTouch:withEvent:")]
		bool BeginTracking (UITouch uitouch, [NullAllowed] UIEvent uievent);

		[Export ("continueTrackingWithTouch:withEvent:")]
		bool ContinueTracking (UITouch uitouch, [NullAllowed] UIEvent uievent);

		[Export ("endTrackingWithTouch:withEvent:")]
		void EndTracking (UITouch uitouch, [NullAllowed] UIEvent uievent);

		[Export ("cancelTrackingWithEvent:")]
		void CancelTracking ([NullAllowed] UIEvent uievent);

		[Export ("addTarget:action:forControlEvents:")]
		void AddTarget ([NullAllowed] NSObject target, Selector sel, UIControlEvent events);

		[Sealed]
		[Internal]
		[Export ("addTarget:action:forControlEvents:")]
		void AddTarget ([NullAllowed] NSObject target, IntPtr sel, UIControlEvent events);

		[Export ("removeTarget:action:forControlEvents:")]
		void RemoveTarget ([NullAllowed] NSObject target, [NullAllowed] Selector sel, UIControlEvent events);

		[Sealed]
		[Internal]
		[Export ("removeTarget:action:forControlEvents:")]
		void RemoveTarget ([NullAllowed] NSObject target, IntPtr sel, UIControlEvent events);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addAction:forControlEvents:")]
		void AddAction (UIAction action, UIControlEvent controlEvents);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeAction:forControlEvents:")]
		void RemoveAction (UIAction action, UIControlEvent controlEvents);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeActionForIdentifier:forControlEvents:")]
		void RemoveAction (string actionIdentifier, UIControlEvent controlEvents);

		[Export ("allTargets")]
		NSSet AllTargets { get; }

		[Export ("allControlEvents")]
		UIControlEvent AllControlEvents { get; }

		[Export ("actionsForTarget:forControlEvent:")]
		string [] GetActions ([NullAllowed] NSObject target, UIControlEvent events);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("enumerateEventHandlers:")]
		void EnumerateEventHandlers (UIControlEnumerateEventsIteratorHandler iteratorHandler);

		[Export ("sendAction:to:forEvent:")]
		void SendAction (Selector action, [NullAllowed] NSObject target, [NullAllowed] UIEvent uievent);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sendAction:")]
		void SendAction (UIAction action);

		[Export ("sendActionsForControlEvents:")]
		void SendActionForControlEvents (UIControlEvent events);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("contextMenuInteraction", ArgumentSemantic.Strong)]
		UIContextMenuInteraction ContextMenuInteraction { get; }

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("contextMenuInteractionEnabled")]
		bool ContextMenuInteractionEnabled { [Bind ("isContextMenuInteractionEnabled")] get; set; }

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("showsMenuAsPrimaryAction")]
		bool ShowsMenuAsPrimaryAction { get; set; }

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("menuAttachmentPointForConfiguration:")]
		CGPoint GetMenuAttachmentPoint (UIContextMenuConfiguration configuration);

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("toolTip")]
		string ToolTip { get; set; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("toolTipInteraction", ArgumentSemantic.Strong)]
		UIToolTipInteraction ToolTipInteraction { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("symbolAnimationEnabled")]
		bool SymbolAnimationEnabled { [Bind ("isSymbolAnimationEnabled")] get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIBarPositioning {
		[Abstract]
		[Export ("barPosition")]
		UIBarPosition BarPosition { get; }
	}

	interface IUIBarPositioning { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIBarPositioningDelegate {
		[Export ("positionForBar:")]
		[DelegateName ("Func<IUIBarPositioning,UIBarPosition>"), NoDefaultValue]
		UIBarPosition GetPositionForBar (IUIBarPositioning barPositioning);
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DisableDefaultCtor] // designated
	interface UIBezierPath : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		// initWithFrame: --> unrecognized selector

		[Export ("bezierPath"), Static]
		UIBezierPath Create ();

		[Export ("bezierPathWithArcCenter:radius:startAngle:endAngle:clockwise:"), Static]
		UIBezierPath FromArc (CGPoint center, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise);

		[Export ("bezierPathWithCGPath:"), Static]
		UIBezierPath FromPath (CGPath path);

		[Export ("bezierPathWithOvalInRect:"), Static]
		UIBezierPath FromOval (CGRect inRect);

		[Export ("bezierPathWithRect:"), Static]
		UIBezierPath FromRect (CGRect rect);

		[Export ("bezierPathWithRoundedRect:byRoundingCorners:cornerRadii:"), Static]
		UIBezierPath FromRoundedRect (CGRect rect, UIRectCorner corners, CGSize radii);

		[Export ("bezierPathWithRoundedRect:cornerRadius:"), Static]
		UIBezierPath FromRoundedRect (CGRect rect, nfloat cornerRadius);

		[Export ("CGPath")]
		[NullAllowed]
		CGPath CGPath { get; set; }

		[Export ("moveToPoint:")]
		void MoveTo (CGPoint point);

		[Export ("addLineToPoint:")]
		void AddLineTo (CGPoint point);

		[Export ("addCurveToPoint:controlPoint1:controlPoint2:")]
		void AddCurveToPoint (CGPoint endPoint, CGPoint controlPoint1, CGPoint controlPoint2);

		[Export ("addQuadCurveToPoint:controlPoint:")]
		void AddQuadCurveToPoint (CGPoint endPoint, CGPoint controlPoint);

		[Export ("closePath")]
		void ClosePath ();

		[Export ("removeAllPoints")]
		void RemoveAllPoints ();

		[Export ("appendPath:")]
		void AppendPath (UIBezierPath path);

		[Export ("applyTransform:")]
		void ApplyTransform (CGAffineTransform transform);

		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get; }

		[Export ("bounds")]
		CGRect Bounds { get; }

		[Export ("currentPoint")]
		CGPoint CurrentPoint { get; }

		[Export ("containsPoint:")]
		bool ContainsPoint (CGPoint point);


		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("lineCapStyle")]
		CGLineCap LineCapStyle { get; set; }

		[Export ("lineJoinStyle")]
		CGLineJoin LineJoinStyle { get; set; }

		[Export ("miterLimit")]
		nfloat MiterLimit { get; set; }

		[Export ("flatness")]
		nfloat Flatness { get; set; }

		[Export ("usesEvenOddFillRule")]
		bool UsesEvenOddFillRule { get; set; }

		[Export ("fill")]
		void Fill ();

		[Export ("stroke")]
		void Stroke ();

		[Export ("fillWithBlendMode:alpha:")]
		void Fill (CGBlendMode blendMode, nfloat alpha);

		[Export ("strokeWithBlendMode:alpha:")]
		void Stroke (CGBlendMode blendMode, nfloat alpha);

		[Export ("addClip")]
		void AddClip ();

		[Internal]
		[Export ("getLineDash:count:phase:")]
		void _GetLineDash (IntPtr pattern, out nint count, out nfloat phase);

		[Internal, Export ("setLineDash:count:phase:")]
		void SetLineDash (IntPtr fvalues, nint count, nfloat phase);

		[Export ("addArcWithCenter:radius:startAngle:endAngle:clockwise:")]
		void AddArc (CGPoint center, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockWise);

		[Export ("bezierPathByReversingPath")]
		UIBezierPath BezierPathByReversingPath ();
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	delegate UIPointerStyle UIButtonPointerStyleProvider (UIButton button, UIPointerEffect proposedEffect, UIPointerShape proposedShape);

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	delegate void UIButtonConfigurationUpdateHandler (UIButton button);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UIButton : UIAccessibilityContentSizeCategoryImageAdjusting
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithFrame:primaryAction:")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] UIAction primaryAction);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("systemButtonWithImage:target:action:")]
		UIButton GetSystemButton (UIImage image, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("systemButtonWithPrimaryAction:")]
		UIButton GetSystemButton ([NullAllowed] UIAction primaryAction);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("buttonWithType:primaryAction:")]
		UIButton FromType (UIButtonType buttonType, [NullAllowed] UIAction primaryAction);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("buttonWithConfiguration:primaryAction:")]
		UIButton GetButton (UIButtonConfiguration configuration, [NullAllowed] UIAction primaryAction);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("configuration", ArgumentSemantic.Copy)]
		UIButtonConfiguration Configuration { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("setNeedsUpdateConfiguration")]
		void SetNeedsUpdateConfiguration ();

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("updateConfiguration")]
		void UpdateConfiguration ();

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("configurationUpdateHandler", ArgumentSemantic.Copy)]
		UIButtonConfigurationUpdateHandler ConfigurationUpdateHandler { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("automaticallyUpdatesConfiguration")]
		bool AutomaticallyUpdatesConfiguration { get; set; }

		[Export ("buttonWithType:")]
		[Static]
		UIButton FromType (UIButtonType type);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Appearance]
		[Export ("contentEdgeInsets")]
		UIEdgeInsets ContentEdgeInsets { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("titleEdgeInsets")]
		UIEdgeInsets TitleEdgeInsets { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("reversesTitleShadowWhenHighlighted")]
		bool ReverseTitleShadowWhenHighlighted { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("imageEdgeInsets")]
		UIEdgeInsets ImageEdgeInsets { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("adjustsImageWhenHighlighted")]
		bool AdjustsImageWhenHighlighted { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("adjustsImageWhenDisabled")]
		bool AdjustsImageWhenDisabled { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'UIButtonConfiguration' is used.")]
		[Export ("showsTouchWhenHighlighted")]
		bool ShowsTouchWhenHighlighted { get; set; }

		[Export ("buttonType")]
		UIButtonType ButtonType { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("hovered")]
		bool Hovered { [Bind ("isHovered")] get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("held")]
		bool Held { [Bind ("isHeld")] get; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("role", ArgumentSemantic.Assign)]
		UIButtonRole Role { get; set; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[Export ("pointerInteractionEnabled")]
		bool PointerInteractionEnabled { [Bind ("isPointerInteractionEnabled")] get; set; }

		[NoWatch, NoTV, iOS (13, 4)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("pointerStyleProvider", ArgumentSemantic.Copy)]
		UIButtonPointerStyleProvider PointerStyleProvider { get; set; }

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("menu", ArgumentSemantic.Copy)]
		UIMenu Menu { get; set; }

		[NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("changesSelectionAsPrimaryAction")]
		bool ChangesSelectionAsPrimaryAction { get; set; }

		[Export ("setTitle:forState:")]
		void SetTitle ([NullAllowed] string title, UIControlState forState);

		[Export ("setTitleColor:forState:")]
		[Appearance]
		void SetTitleColor ([NullAllowed] UIColor color, UIControlState forState);

		[Export ("setTitleShadowColor:forState:")]
		[Appearance]
		void SetTitleShadowColor ([NullAllowed] UIColor color, UIControlState forState);

		[Export ("setImage:forState:")]
		[Appearance]
		void SetImage ([NullAllowed] UIImage image, UIControlState forState);

		[Export ("setBackgroundImage:forState:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage image, UIControlState forState);

		[Export ("titleForState:")]
		string Title (UIControlState state);

		[Export ("titleColorForState:")]
		[Appearance]
		UIColor TitleColor (UIControlState state);

		[Export ("titleShadowColorForState:")]
		[Appearance]
		UIColor TitleShadowColor (UIControlState state);

		[Export ("imageForState:")]
		[Appearance]
		UIImage ImageForState (UIControlState state);

		[Export ("backgroundImageForState:")]
		[Appearance]
		UIImage BackgroundImageForState (UIControlState state);

		[Export ("currentTitle", ArgumentSemantic.Retain)]
		string CurrentTitle { get; }

		[Export ("currentTitleColor", ArgumentSemantic.Retain)]
		[Appearance]
		UIColor CurrentTitleColor { get; }

		[Export ("currentTitleShadowColor", ArgumentSemantic.Retain)]
		[Appearance]
		UIColor CurrentTitleShadowColor { get; }

		[Export ("currentImage", ArgumentSemantic.Retain)]
		[Appearance]
		UIImage CurrentImage { get; }

		[Export ("currentBackgroundImage", ArgumentSemantic.Retain)]
		[Appearance]
		UIImage CurrentBackgroundImage { get; }

		[Export ("titleLabel", ArgumentSemantic.Retain)]
		UILabel TitleLabel { get; }

		[Export ("imageView", ArgumentSemantic.Retain)]
		UIImageView ImageView { get; }

		[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("subtitleLabel", ArgumentSemantic.Strong)]
		UILabel SubtitleLabel { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Export ("backgroundRectForBounds:")]
		CGRect BackgroundRectForBounds (CGRect rect);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Export ("contentRectForBounds:")]
		CGRect ContentRectForBounds (CGRect rect);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Export ("titleRectForContentRect:")]
		CGRect TitleRectForContentRect (CGRect rect);

		[Deprecated (PlatformName.iOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Ignored when 'Configuration' is used, override 'LayoutSubviews', call base, and set position views on your own.")]
		[Export ("imageRectForContentRect:")]
		CGRect ImageRectForContentRect (CGRect rect);

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 3, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; set; }

		[Deprecated (PlatformName.iOS, 3, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("lineBreakMode")]
		UILineBreakMode LineBreakMode { get; set; }

		[Deprecated (PlatformName.iOS, 3, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("titleShadowOffset")]
		CGSize TitleShadowOffset { get; set; }
#endif

		//
		// 6.0
		//
		[Export ("currentAttributedTitle", ArgumentSemantic.Retain)]
		NSAttributedString CurrentAttributedTitle { get; }

		[Export ("setAttributedTitle:forState:")]
		void SetAttributedTitle ([NullAllowed] NSAttributedString title, UIControlState state);

		[Export ("attributedTitleForState:")]
		NSAttributedString GetAttributedTitle (UIControlState state);

		[Appearance]
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setPreferredSymbolConfiguration:forImageInState:")]
		void SetPreferredSymbolConfiguration ([NullAllowed] UIImageSymbolConfiguration configuration, UIControlState state);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preferredSymbolConfigurationForImageInState:")]
		[return: NullAllowed]
		UIImageSymbolConfiguration GetPreferredSymbolConfiguration (UIControlState state);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("currentPreferredSymbolConfiguration", ArgumentSemantic.Strong)]
		UIImageSymbolConfiguration CurrentPreferredSymbolConfiguration { get; }

		// From UIButton (UIBehavioralStyle)

		[NoWatch, NoTV, MacCatalyst (15, 0), iOS (15, 0)]
		[Export ("behavioralStyle")]
		UIBehavioralStyle BehavioralStyle { get; }

		[NoWatch, TV (17, 0), MacCatalyst (15, 0), iOS (15, 0)]
		[Export ("preferredBehavioralStyle", ArgumentSemantic.Assign)]
		UIBehavioralStyle PreferredBehavioralStyle { get; set; }

		[NoWatch, TV (17, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredMenuElementOrder", ArgumentSemantic.Assign)]
		UIContextMenuConfigurationElementOrder PreferredMenuElementOrder { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UILabel : UIContentSizeCategoryAdjusting, UILetterformAwareAdjusting {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Text { get; set; }

		[Export ("font", ArgumentSemantic.Retain)]
		[Appearance]
		UIFont Font { get; set; }

		[Export ("textColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor TextColor { get; set; }

		[Export ("shadowColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor ShadowColor { get; set; }

		[Export ("shadowOffset")]
		[Appearance]
		CGSize ShadowOffset { get; set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("lineBreakMode")]
		UILineBreakMode LineBreakMode { get; set; }

		[Export ("highlightedTextColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor HighlightedTextColor { get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("numberOfLines")]
		nint Lines { get; set; }

		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; set; }

		[NoTV]
		[Export ("minimumFontSize")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'MinimumScaleFactor' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MinimumScaleFactor' instead.")]
		nfloat MinimumFontSize { get; set; }

		[Export ("baselineAdjustment")]
		UIBaselineAdjustment BaselineAdjustment { get; set; }

		[Export ("textRectForBounds:limitedToNumberOfLines:")]
		CGRect TextRectForBounds (CGRect bounds, nint numberOfLines);

		[Export ("drawTextInRect:")]
		void DrawText (CGRect rect);

		//
		// 6.0
		//
		[NullAllowed] // by default this property is null
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }

		[NoTV]
		[Export ("adjustsLetterSpacingToFitWidth")]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSKernAttributeName' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKernAttributeName' instead.")]
		bool AdjustsLetterSpacingToFitWidth { get; set; }

		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set; }

		[Export ("preferredMaxLayoutWidth")]
		nfloat PreferredMaxLayoutWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("lineBreakStrategy", ArgumentSemantic.Assign)]
		NSLineBreakStrategy LineBreakStrategy { get; set; }

		[TV (12, 0), NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("enablesMarqueeWhenAncestorFocused")]
		bool EnablesMarqueeWhenAncestorFocused { get; set; }

		[MacCatalyst (15, 0), NoWatch, iOS (15, 0), TV (15, 0)]
		[Export ("showsExpansionTextWhenTruncated")]
		bool ShowsExpansionTextWhenTruncated { get; set; }

		[Appearance]
		[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("preferredVibrancy", ArgumentSemantic.Assign)]
		UILabelVibrancy PreferredVibrancy { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIImageView
#if !WATCH
	: UIAccessibilityContentSizeCategoryImageAdjusting
#endif // !WATCH
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithImage:")]
		[PostGet ("Image")]
		NativeHandle Constructor ([NullAllowed] UIImage image);

		[Export ("initWithImage:highlightedImage:")]
		[PostGet ("Image")]
		[PostGet ("HighlightedImage")]
		NativeHandle Constructor ([NullAllowed] UIImage image, [NullAllowed] UIImage highlightedImage);

		[Export ("image", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage Image { get; set; }

		[Export ("highlightedImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage HighlightedImage { get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("animationImages", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIImage [] AnimationImages { get; set; }

		[Export ("highlightedAnimationImages", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIImage [] HighlightedAnimationImages { get; set; }

		[Export ("animationDuration")]
		double AnimationDuration { get; set; }

		[Export ("animationRepeatCount")]
		nint AnimationRepeatCount { get; set; }

		[Export ("startAnimating")]
		void StartAnimating ();

		[Export ("stopAnimating")]
		void StopAnimating ();

		[Export ("isAnimating")]
		bool IsAnimating { get; }

		[NoiOS] // UIKIT_AVAILABLE_TVOS_ONLY
		[NoMacCatalyst]
		[Export ("adjustsImageWhenAncestorFocused")]
		bool AdjustsImageWhenAncestorFocused { get; set; }

		[NoiOS] // UIKIT_AVAILABLE_TVOS_ONLY
		[NoMacCatalyst]
		[Export ("focusedFrameGuide")]
		UILayoutGuide FocusedFrameGuide { get; }

		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("overlayContentView", ArgumentSemantic.Strong)]
		UIView OverlayContentView { get; }

		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("masksFocusEffectToContents")]
		bool MasksFocusEffectToContents { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("preferredSymbolConfiguration", ArgumentSemantic.Strong)]
		UIImageSymbolConfiguration PreferredSymbolConfiguration { get; set; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("preferredImageDynamicRange", ArgumentSemantic.Assign)]
		UIImageDynamicRange PreferredImageDynamicRange { get; set; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("imageDynamicRange", ArgumentSemantic.Assign)]
		UIImageDynamicRange ImageDynamicRange { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:options:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:options:animated:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated);

		[Async]
		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("addSymbolEffect:options:animated:completion:")]
		void AddSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated, [NullAllowed] Action<UISymbolEffectCompletionContext> completionHandler);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:options:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:options:animated:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated);

		[Async]
		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeSymbolEffectOfType:options:animated:completion:")]
		void RemoveSymbolEffect (NSSymbolEffect symbolEffect, NSSymbolEffectOptions options, bool animated, [NullAllowed] Action<UISymbolEffectCompletionContext> completionHandler);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffects")]
		void RemoveAllSymbolEffects ();

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffectsWithOptions:")]
		void RemoveAllSymbolEffects (NSSymbolEffectOptions options);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("removeAllSymbolEffectsWithOptions:animated:")]
		void RemoveAllSymbolEffects (NSSymbolEffectOptions options, bool animated);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setSymbolImage:withContentTransition:")]
		void SetSymbolImage (UIImage symbolImage, NSSymbolContentTransition transition);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setSymbolImage:withContentTransition:options:")]
		void SetSymbolImage (UIImage symbolImage, NSSymbolContentTransition transition, NSSymbolEffectOptions options);

		[Async]
		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setSymbolImage:withContentTransition:options:completion:")]
		void SetSymbolImage (UIImage symbolImage, NSSymbolContentTransition transition, NSSymbolEffectOptions options, [NullAllowed] Action<UISymbolEffectCompletionContext> completionHandler);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UIDatePicker {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("datePickerMode")]
		UIDatePickerMode Mode { get; set; }

		[Export ("locale", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSLocale Locale { get; set; }

		[Export ("timeZone", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSTimeZone TimeZone { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSCalendar Calendar { get; set; }

		// not fully clear from docs but null is not allowed:
		// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Invalid parameter not satisfying: date
		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }

		[Export ("minimumDate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSDate MinimumDate { get; set; }

		[Export ("maximumDate", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSDate MaximumDate { get; set; }

		[Export ("countDownDuration")]
		double CountDownDuration { get; set; }

		[Export ("minuteInterval")]
		nint MinuteInterval { get; set; }

		[Export ("setDate:animated:")]
		void SetDate (NSDate date, bool animated);

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4)]
		[Export ("preferredDatePickerStyle", ArgumentSemantic.Assign)]
		UIDatePickerStyle PreferredDatePickerStyle { get; set; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4)]
		[Export ("datePickerStyle", ArgumentSemantic.Assign)]
		UIDatePickerStyle DatePickerStyle { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("roundsToMinuteInterval")]
		bool RoundsToMinuteInterval { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	interface UIDevice {
		[Static]
		[Export ("currentDevice")]
		UIDevice CurrentDevice { get; }

		[Export ("name", ArgumentSemantic.Retain)]
		string Name { get; }

		[Export ("model", ArgumentSemantic.Retain)]
		string Model { get; }

		[Export ("localizedModel", ArgumentSemantic.Retain)]
		string LocalizedModel { get; }

		[Export ("systemName", ArgumentSemantic.Retain)]
		string SystemName { get; }

		[Export ("systemVersion", ArgumentSemantic.Retain)]
		string SystemVersion { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("orientation")]
		UIDeviceOrientation Orientation { get; }

#if false
		[Obsolete ("Deprecated in iOS 5.0")]
		[Export ("uniqueIdentifier", ArgumentSemantic.Retain)]
		string UniqueIdentifier { get; }
#endif

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("generatesDeviceOrientationNotifications")]
		bool GeneratesDeviceOrientationNotifications { [Bind ("isGeneratingDeviceOrientationNotifications")] get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("beginGeneratingDeviceOrientationNotifications")]
		void BeginGeneratingDeviceOrientationNotifications ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("endGeneratingDeviceOrientationNotifications")]
		void EndGeneratingDeviceOrientationNotifications ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("batteryMonitoringEnabled")]
		bool BatteryMonitoringEnabled { [Bind ("isBatteryMonitoringEnabled")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("batteryState")]
		UIDeviceBatteryState BatteryState { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("batteryLevel")]
		float BatteryLevel { get; } // This is float, not nfloat

		[Export ("proximityMonitoringEnabled")]
		bool ProximityMonitoringEnabled { [Bind ("isProximityMonitoringEnabled")] get; set; }

		[Export ("proximityState")]
		bool ProximityState { get; }

		[Export ("userInterfaceIdiom")]
		UIUserInterfaceIdiom UserInterfaceIdiom { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIDeviceOrientationDidChangeNotification")]
		[Notification]
		NSString OrientationDidChangeNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIDeviceBatteryStateDidChangeNotification")]
		[Notification]
		NSString BatteryStateDidChangeNotification { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UIDeviceBatteryLevelDidChangeNotification")]
		[Notification]
		NSString BatteryLevelDidChangeNotification { get; }

		[Field ("UIDeviceProximityStateDidChangeNotification")]
		[Notification]
		NSString ProximityStateDidChangeNotification { get; }

		[Export ("isMultitaskingSupported")]
		bool IsMultitaskingSupported { get; }

		[Export ("playInputClick")]
		void PlayInputClick ();

		[Export ("identifierForVendor", ArgumentSemantic.Retain)]
		NSUuid IdentifierForVendor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIDictationPhrase {
		[Export ("alternativeInterpretations")]
		string [] AlternativeInterpretations { get; }

		[Export ("text")]
		string Text { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (14, 0)] // doc as 13.0 but throws: NSGenericException Reason: UIDocumentInteractionController not available
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIDocumentInteractionControllerDelegate) })]
	interface UIDocumentInteractionController {
		[Export ("interactionControllerWithURL:"), Static]
		UIDocumentInteractionController FromUrl (NSUrl url);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDocumentInteractionControllerDelegate Delegate { get; set; }

		// default value is null but it cannot be set back to null
		// NSInternalInconsistencyException Reason: UIDocumentInteractionController: invalid scheme (null).  Only the file scheme is supported.
		[Export ("URL", ArgumentSemantic.Retain)]
		NSUrl Url { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("UTI", ArgumentSemantic.Copy)]
		string Uti { get; set; }

		[Export ("annotation", ArgumentSemantic.Retain), NullAllowed]
		NSObject Annotation { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("icons")]
		UIImage [] Icons { get; }

		[Export ("dismissMenuAnimated:")]
		void DismissMenu (bool animated);

		[Export ("dismissPreviewAnimated:")]
		void DismissPreview (bool animated);

		[Export ("presentOpenInMenuFromBarButtonItem:animated:")]
		bool PresentOpenInMenu (UIBarButtonItem item, bool animated);

		[Export ("presentOpenInMenuFromRect:inView:animated:")]
		bool PresentOpenInMenu (CGRect rect, UIView inView, bool animated);

		[Export ("presentOptionsMenuFromBarButtonItem:animated:")]
		bool PresentOptionsMenu (UIBarButtonItem item, bool animated);

		[Export ("presentOptionsMenuFromRect:inView:animated:")]
		bool PresentOptionsMenu (CGRect rect, UIView inView, bool animated);

		[Export ("presentPreviewAnimated:")]
		bool PresentPreview (bool animated);

		[Export ("gestureRecognizers")]
		UIGestureRecognizer [] GestureRecognizers { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIDocumentInteractionControllerDelegate {
		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("documentInteractionController:canPerformAction:"), DelegateName ("UIDocumentInteractionProbe"), DefaultValue (false)]
		bool CanPerformAction (UIDocumentInteractionController controller, [NullAllowed] Selector action);

		[Deprecated (PlatformName.iOS, 6, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("documentInteractionController:performAction:"), DelegateName ("UIDocumentInteractionProbe"), DefaultValue (false)]
		bool PerformAction (UIDocumentInteractionController controller, [NullAllowed] Selector action);

		[Export ("documentInteractionController:didEndSendingToApplication:")]
		[EventArgs ("UIDocumentSendingToApplication")]
		void DidEndSendingToApplication (UIDocumentInteractionController controller, [NullAllowed] string application);

		[Export ("documentInteractionController:willBeginSendingToApplication:")]
		[EventArgs ("UIDocumentSendingToApplication")]
		void WillBeginSendingToApplication (UIDocumentInteractionController controller, [NullAllowed] string application);

		[Export ("documentInteractionControllerDidDismissOpenInMenu:")]
		void DidDismissOpenInMenu (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerDidDismissOptionsMenu:")]
		void DidDismissOptionsMenu (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerDidEndPreview:")]
		void DidEndPreview (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerRectForPreview:"), DelegateName ("UIDocumentInteractionRectangle"), DefaultValue (null)]
		CGRect RectangleForPreview (UIDocumentInteractionController controller);

		// in 2016 when this binding was done, there was not UIDocumentViewController, with
		// xcode154 apple added a class with the same name :/ 
#if XAMCORE_5_0
		[Export ("documentInteractionControllerViewControllerForPreview:"), DelegateName ("UIDocumentPreviewController"), DefaultValue (null)]
#else
		[Export ("documentInteractionControllerViewControllerForPreview:"), DelegateName ("UIDocumentViewController"), DefaultValue (null)]
#endif
		UIViewController ViewControllerForPreview (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerViewForPreview:"), DelegateName ("UIDocumentViewForPreview"), DefaultValue (null)]
		UIView ViewForPreview (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerWillBeginPreview:")]
		void WillBeginPreview (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerWillPresentOpenInMenu:")]
		void WillPresentOpenInMenu (UIDocumentInteractionController controller);

		[Export ("documentInteractionControllerWillPresentOptionsMenu:")]
		void WillPresentOptionsMenu (UIDocumentInteractionController controller);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UINavigationController), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (UIImagePickerControllerDelegate) })]
	interface UIImagePickerController {
		[Export ("isSourceTypeAvailable:")]
		[Static]
		bool IsSourceTypeAvailable (UIImagePickerControllerSourceType sourceType);

		[Export ("availableMediaTypesForSourceType:"), Static]
		string [] AvailableMediaTypes (UIImagePickerControllerSourceType sourceType);

		// This is the foundation to implement both id <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject Delegate { get; set; }

		[Export ("sourceType")]
		UIImagePickerControllerSourceType SourceType { get; set; }

		[Export ("mediaTypes", ArgumentSemantic.Copy)]
		string [] MediaTypes { get; set; }

#if !XAMCORE_3_0
		[Export ("allowsImageEditing")]
		[Deprecated (PlatformName.iOS, 3, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		bool AllowsImageEditing { get; set; }
#endif

		//
		// 3.1 APIs
		//
		[Export ("allowsEditing")]
		bool AllowsEditing { get; set; }

		[Export ("videoMaximumDuration")]
		double VideoMaximumDuration { get; set; }

		[Export ("videoQuality")]
		UIImagePickerControllerQualityType VideoQuality { get; set; }

		[Export ("showsCameraControls")]
		bool ShowsCameraControls { get; set; }

		[Export ("cameraOverlayView", ArgumentSemantic.Retain)]
		UIView CameraOverlayView { get; set; }

		[Export ("cameraViewTransform")]
		CGAffineTransform CameraViewTransform { get; set; }

		[Export ("takePicture")]
		void TakePicture ();

		[Export ("startVideoCapture")]
		bool StartVideoCapture ();

		[Export ("stopVideoCapture")]
		void StopVideoCapture ();

		[Export ("cameraCaptureMode")]
		UIImagePickerControllerCameraCaptureMode CameraCaptureMode { get; set; }

		[Static]
		[Export ("availableCaptureModesForCameraDevice:")]
		NSNumber [] AvailableCaptureModesForCameraDevice (UIImagePickerControllerCameraDevice cameraDevice);

		[Export ("cameraDevice")]
		UIImagePickerControllerCameraDevice CameraDevice { get; set; }

		[Export ("cameraFlashMode")]
		UIImagePickerControllerCameraFlashMode CameraFlashMode { get; set; }

		[Static, Export ("isCameraDeviceAvailable:")]
		bool IsCameraDeviceAvailable (UIImagePickerControllerCameraDevice cameraDevice);

		[Static, Export ("isFlashAvailableForCameraDevice:")]
		bool IsFlashAvailableForCameraDevice (UIImagePickerControllerCameraDevice cameraDevice);

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		[Export ("imageExportPreset", ArgumentSemantic.Assign)]
		UIImagePickerControllerImageUrlExportPreset ImageExportPreset { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		[Export ("videoExportPreset")]
		string VideoExportPreset { get; set; }

		// manually bound (const fields) in monotouch.dll - unlike the newer fields (static properties)

		[Field ("UIImagePickerControllerMediaType")]
		NSString MediaType { get; }

		[Field ("UIImagePickerControllerOriginalImage")]
		NSString OriginalImage { get; }

		[Field ("UIImagePickerControllerEditedImage")]
		NSString EditedImage { get; }

		[Field ("UIImagePickerControllerCropRect")]
		NSString CropRect { get; }

		[Field ("UIImagePickerControllerMediaURL")]
		NSString MediaURL { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIImagePickerController.PHAsset' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIImagePickerController.PHAsset' instead.")]
		[Field ("UIImagePickerControllerReferenceURL")]
		NSString ReferenceUrl { get; }

		[Field ("UIImagePickerControllerMediaMetadata")]
		NSString MediaMetadata { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIImagePickerControllerLivePhoto")]
		NSString LivePhoto { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'PHPicker' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'PHPicker' instead.")]
		[Field ("UIImagePickerControllerPHAsset")]
		NSString PHAsset { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIImagePickerControllerImageURL")]
		NSString ImageUrl { get; }
	}

	// UINavigationControllerDelegate, UIImagePickerControllerDelegate
	[BaseType (typeof (UINavigationControllerDelegate))]
	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	interface UIImagePickerControllerDelegate {
#if !XAMCORE_3_0
		[Obsoleted (PlatformName.iOS, 3, 0)]
		[Export ("imagePickerController:didFinishPickingImage:editingInfo:"), EventArgs ("UIImagePickerImagePicked")]
		void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo);
#endif

		[Export ("imagePickerController:didFinishPickingMediaWithInfo:"), EventArgs ("UIImagePickerMediaPicked")]
		void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info);

		[Export ("imagePickerControllerDidCancel:"), EventArgs ("UIImagePickerController")]
		void Canceled (UIImagePickerController picker);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDocument))]
	// *** Assertion failure in -[UIManagedDocument init], /SourceCache/UIKit_Sim/UIKit-1914.84/UIDocument.m:258
	[DisableDefaultCtor]
	interface UIManagedDocument {
		// note: ctor are not inherited, but this is how the documentation tells you to create an UIManagedDocument
		// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/UIManagedDocument_Class/Reference/Reference.html
		[Export ("initWithFileURL:")]
		[PostGet ("FileUrl")]
		NativeHandle Constructor (NSUrl url);

		[Export ("managedObjectContext", ArgumentSemantic.Retain)]
		NSManagedObjectContext ManagedObjectContext { get; }

		[Export ("managedObjectModel", ArgumentSemantic.Retain)]
		NSManagedObjectModel ManagedObjectModel { get; }

		[Export ("persistentStoreOptions", ArgumentSemantic.Copy)]
		NSDictionary PersistentStoreOptions { get; set; }

		[Export ("modelConfiguration", ArgumentSemantic.Copy)]
		string ModelConfiguration { get; set; }

		[Static]
		[Export ("persistentStoreName")]
		string PersistentStoreName { get; }

		[Export ("configurePersistentStoreCoordinatorForURL:ofType:modelConfiguration:storeOptions:error:")]
		bool ConfigurePersistentStoreCoordinator (NSUrl storeURL, string fileType, string configuration, NSDictionary storeOptions, NSError error);

		[Export ("persistentStoreTypeForFileType:")]
		string GetPersistentStoreType (string fileType);

		[Export ("readAdditionalContentFromURL:error:")]
		bool ReadAdditionalContent (NSUrl absoluteURL, out NSError error);

		[Export ("additionalContentForURL:error:")]
		NSObject AdditionalContent (NSUrl absoluteURL, out NSError error);

		[Export ("writeAdditionalContent:toURL:originalContentsURL:error:")]
		bool WriteAdditionalContent (NSObject content, NSUrl absoluteURL, NSUrl absoluteOriginalContentsURL, out NSError error);
	}

	[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'UIEditMenuInteraction' instead.")]
	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'UIEditMenuInteraction' instead.")]
	[BaseType (typeof (NSObject))]
	interface UIMenuController {
		[Static, Export ("sharedMenuController")]
		UIMenuController SharedMenuController { get; }

		[Export ("menuVisible")]
		bool MenuVisible {
			[Bind ("isMenuVisible")]
			get; [Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShowMenu' or 'HideMenu' instead.")]
			[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ShowMenu' or 'HideMenu' instead.")]
			set;
		}

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ShowMenu' or 'HideMenu' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShowMenu' or 'HideMenu' instead.")]
		[Export ("setMenuVisible:animated:")]
		void SetMenuVisible (bool visible, bool animated);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ShowMenu' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ShowMenu' instead.")]
		[Export ("setTargetRect:inView:")]
		void SetTargetRect (CGRect rect, UIView inView);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("showMenuFromView:rect:")]
		void ShowMenu (UIView targetView, CGRect targetRect);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hideMenuFromView:")]
		void HideMenu (UIView targetView);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hideMenu")]
		void HideMenu ();

		[Export ("update")]
		void Update ();

		[Export ("menuFrame")]
		CGRect MenuFrame { get; }

		[Export ("arrowDirection")]
		UIMenuControllerArrowDirection ArrowDirection { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("menuItems", ArgumentSemantic.Copy)]
		UIMenuItem [] MenuItems { get; set; }

		[Field ("UIMenuControllerWillShowMenuNotification")]
		[Notification]
		NSString WillShowMenuNotification { get; }

		[Field ("UIMenuControllerDidShowMenuNotification")]
		[Notification]
		NSString DidShowMenuNotification { get; }

		[Field ("UIMenuControllerWillHideMenuNotification")]
		[Notification]
		NSString WillHideMenuNotification { get; }

		[Field ("UIMenuControllerDidHideMenuNotification")]
		[Notification]
		NSString DidHideMenuNotification { get; }

		[Field ("UIMenuControllerMenuFrameDidChangeNotification")]
		[Notification]
		NSString MenuFrameDidChangeNotification { get; }
	}

	[Deprecated (PlatformName.iOS, 16, 0, message: "Use 'UIEditMenuInteraction' instead.")]
	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use 'UIEditMenuInteraction' instead.")]
	[BaseType (typeof (NSObject))]
	interface UIMenuItem {
		[DesignatedInitializer] // TODO: Add an overload that takes an Action maybe?
		[Export ("initWithTitle:action:")]
		NativeHandle Constructor (string title, Selector action);

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("action")]
		Selector Action { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UINavigationBar : UIBarPositioning, NSCoding {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NoTV]
		[MacCatalyst (13, 1)]
		// [Appearance] rdar://22818366
		[Appearance]
		[Export ("barStyle", ArgumentSemantic.Assign)]
		UIBarStyle BarStyle { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UINavigationBarDelegate Delegate { get; set; }

		[Appearance]
		[Export ("translucent", ArgumentSemantic.Assign)]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		[Export ("pushNavigationItem:animated:")]
		[PostGet ("Items")] // that will [PostGet] TopItem too
		void PushNavigationItem (UINavigationItem item, bool animated);

		[Export ("popNavigationItemAnimated:")]
		[PostGet ("Items")] // that will [PostGet] TopItem too
		UINavigationItem PopNavigationItem (bool animated);

		[Export ("topItem", ArgumentSemantic.Retain)]
		UINavigationItem TopItem { get; }

		[Export ("backItem", ArgumentSemantic.Retain)]
		UINavigationItem BackItem { get; }

		[Export ("items", ArgumentSemantic.Copy)]
		[PostGet ("TopItem")]
		UINavigationItem [] Items { get; set; }

		[Export ("setItems:animated:")]
		[PostGet ("Items")] // that will [PostGet] TopItem too
		void SetItems (UINavigationItem [] items, bool animated);

		[NullAllowed] // by default this property is null
		[Export ("titleTextAttributes", ArgumentSemantic.Copy), Internal]
		[Appearance]
		NSDictionary _TitleTextAttributes { get; set; }

		[Wrap ("_TitleTextAttributes")]
		[Appearance]
		UIStringAttributes TitleTextAttributes { get; set; }

		[Export ("setBackgroundImage:forBarMetrics:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarMetrics barMetrics);

		[Export ("backgroundImageForBarMetrics:")]
		[Appearance]
		UIImage GetBackgroundImage (UIBarMetrics forBarMetrics);

		[Export ("setTitleVerticalPositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetTitleVerticalPositionAdjustment (nfloat adjustment, UIBarMetrics barMetrics);


		[Export ("titleVerticalPositionAdjustmentForBarMetrics:")]
		[Appearance]
		nfloat GetTitleVerticalPositionAdjustment (UIBarMetrics barMetrics);

		//
		// 6.0
		//
		[Appearance]
		[NullAllowed]
		[Export ("shadowImage", ArgumentSemantic.Retain)]
		UIImage ShadowImage { get; set; }

		//
		// 7.0
		//
		[Appearance]
		[NullAllowed]
		[Export ("barTintColor", ArgumentSemantic.Retain)]
		UIColor BarTintColor { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Appearance]
		[NullAllowed]
		[Export ("backIndicatorImage", ArgumentSemantic.Retain)]
		UIImage BackIndicatorImage { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Appearance]
		[NullAllowed]
		[Export ("backIndicatorTransitionMaskImage", ArgumentSemantic.Retain)]
		UIImage BackIndicatorTransitionMaskImage { get; set; }

		[Appearance]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("standardAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance StandardAppearance { get; set; }

		[Appearance]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("compactAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance CompactAppearance { get; set; }

		[Appearance]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("scrollEdgeAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance ScrollEdgeAppearance { get; set; }

		[Appearance]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("compactScrollEdgeAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance CompactScrollEdgeAppearance { get; set; }

		[Appearance]
		[Export ("setBackgroundImage:forBarPosition:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarPosition barPosition, UIBarMetrics barMetrics);

		[Appearance]
		[Export ("backgroundImageForBarPosition:barMetrics:")]
		UIImage GetBackgroundImage (UIBarPosition barPosition, UIBarMetrics barMetrics);

		[Appearance]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("prefersLargeTitles")]
		bool PrefersLargeTitles { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Internal, NullAllowed, Export ("largeTitleTextAttributes", ArgumentSemantic.Copy)]
		[Appearance]
		NSDictionary _LargeTitleTextAttributes { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Wrap ("_LargeTitleTextAttributes")]
		[NullAllowed]
		[Appearance]
		UIStringAttributes LargeTitleTextAttributes { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredBehavioralStyle", ArgumentSemantic.Assign)]
		UIBehavioralStyle PreferredBehavioralStyle { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("currentNSToolbarSection", ArgumentSemantic.Assign)]
		UINavigationBarNSToolbarSection CurrentNSToolbarSection { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("behavioralStyle", ArgumentSemantic.Assign)]
		UIBehavioralStyle BehavioralStyle { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarPositioningDelegate))]
	[Model]
	[Protocol]
	interface UINavigationBarDelegate {
		[Export ("navigationBar:didPopItem:")]
		void DidPopItem (UINavigationBar navigationBar, UINavigationItem item);

		[Export ("navigationBar:shouldPopItem:")]
		bool ShouldPopItem (UINavigationBar navigationBar, UINavigationItem item);

		[Export ("navigationBar:didPushItem:")]
		void DidPushItem (UINavigationBar navigationBar, UINavigationItem item);

		[Export ("navigationBar:shouldPushItem:")]
		bool ShouldPushItem (UINavigationBar navigationBar, UINavigationItem item);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("navigationBarNSToolbarSection:")]
		UINavigationBarNSToolbarSection GetNSToolbarSection (UINavigationBar navigationBar);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UINavigationItem : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithTitle:")]
		NativeHandle Constructor (string title);

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("backBarButtonItem", ArgumentSemantic.Retain)]
		UIBarButtonItem BackBarButtonItem { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("backButtonTitle")]
		string BackButtonTitle { get; set; }

		[Export ("titleView", ArgumentSemantic.Retain), NullAllowed]
		UIView TitleView { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("prompt", ArgumentSemantic.Copy), NullAllowed]
		string Prompt { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBackButton", ArgumentSemantic.Assign)]
		bool HidesBackButton { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setHidesBackButton:animated:")]
		void SetHidesBackButton (bool hides, bool animated);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("backButtonDisplayMode", ArgumentSemantic.Assign)]
		UINavigationItemBackButtonDisplayMode BackButtonDisplayMode { get; set; }

		[Export ("leftBarButtonItem", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIBarButtonItem LeftBarButtonItem {
			get;
			// only on the setter to avoid endless recursion
			[PostGet ("LeftBarButtonItems")]
			set;
		}

		[Export ("rightBarButtonItem", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIBarButtonItem RightBarButtonItem {
			get;
			// only on the setter to avoid endless recursion
			[PostGet ("RightBarButtonItems")]
			set;
		}

		[Export ("setLeftBarButtonItem:animated:")]
		[PostGet ("LeftBarButtonItem")]
		void SetLeftBarButtonItem ([NullAllowed] UIBarButtonItem item, bool animated);

		[Export ("setRightBarButtonItem:animated:")]
		[PostGet ("RightBarButtonItem")]
		void SetRightBarButtonItem ([NullAllowed] UIBarButtonItem item, bool animated);

		[NullAllowed] // by default this property is null
		[Export ("leftBarButtonItems", ArgumentSemantic.Copy)]
		[PostGet ("LeftBarButtonItem")]
		UIBarButtonItem [] LeftBarButtonItems { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("rightBarButtonItems", ArgumentSemantic.Copy)]
		[PostGet ("RightBarButtonItem")]
		UIBarButtonItem [] RightBarButtonItems { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("leftItemsSupplementBackButton")]
		bool LeftItemsSupplementBackButton { get; set; }

		[Export ("setLeftBarButtonItems:animated:")]
		[PostGet ("LeftBarButtonItems")]
		void SetLeftBarButtonItems (UIBarButtonItem [] items, bool animated);

		[Export ("setRightBarButtonItems:animated:")]
		[PostGet ("RightBarButtonItems")]
		void SetRightBarButtonItems (UIBarButtonItem [] items, bool animated);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("largeTitleDisplayMode", ArgumentSemantic.Assign)]
		UINavigationItemLargeTitleDisplayMode LargeTitleDisplayMode { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("searchController", ArgumentSemantic.Retain)]
		UISearchController SearchController { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesSearchBarWhenScrolling")]
		bool HidesSearchBarWhenScrolling { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("standardAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance StandardAppearance { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("compactAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance CompactAppearance { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("scrollEdgeAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance ScrollEdgeAppearance { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("compactScrollEdgeAppearance", ArgumentSemantic.Copy)]
		UINavigationBarAppearance CompactScrollEdgeAppearance { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("pinnedTrailingGroup", ArgumentSemantic.Strong)]
		[NullAllowed]
		UIBarButtonItemGroup PinnedTrailingGroup { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("overflowPresentationSource", ArgumentSemantic.Strong)]
		[NullAllowed]
		IUIPopoverPresentationControllerSourceItem OverflowPresentationSource { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("trailingItemGroups", ArgumentSemantic.Copy)]
		UIBarButtonItemGroup [] TrailingItemGroups { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("additionalOverflowItems", ArgumentSemantic.Strong)]
		[NullAllowed]
		UIDeferredMenuElement AdditionalOverflowItems { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("titleMenuProvider", ArgumentSemantic.Copy)]
		[NullAllowed]
		Func<NSArray<UIMenuElement>, UIMenu> TitleMenuProvider { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Wrap ("WeakRenameDelegate")]
		IUINavigationItemRenameDelegate RenameDelegate { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("renameDelegate", ArgumentSemantic.Weak)]
		NSObject WeakRenameDelegate { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("documentProperties", ArgumentSemantic.Strong)]
		[NullAllowed]
		UIDocumentProperties DocumentProperties { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("customizationIdentifier")]
		[NullAllowed]
		string CustomizationIdentifier { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("leadingItemGroups", ArgumentSemantic.Copy)]
		UIBarButtonItemGroup [] LeadingItemGroups { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("centerItemGroups", ArgumentSemantic.Copy)]
		UIBarButtonItemGroup [] CenterItemGroups { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("style", ArgumentSemantic.Assign)]
		UINavigationItemStyle Style { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("backAction", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIAction BackAction { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredSearchBarPlacement", ArgumentSemantic.Assign)]
		UINavigationItemSearchBarPlacement PreferredSearchBarPlacement { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("searchBarPlacement", ArgumentSemantic.Assign)]
		UINavigationItemSearchBarPlacement SearchBarPlacement { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UINavigationController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Internal, Export ("initWithNavigationBarClass:toolbarClass:")]
		NativeHandle Constructor (IntPtr navigationBarClass, IntPtr toolbarClass);

		[DesignatedInitializer]
		[Export ("initWithRootViewController:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		NativeHandle Constructor (UIViewController rootViewController);

		[Export ("pushViewController:animated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		void PushViewController (UIViewController viewController, bool animated);

		[Export ("popViewControllerAnimated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		UIViewController PopViewController (bool animated);

		[Export ("popToViewController:animated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		UIViewController [] PopToViewController (UIViewController viewController, bool animated);

		[Export ("popToRootViewControllerAnimated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		UIViewController [] PopToRootViewController (bool animated);

		[Export ("topViewController", ArgumentSemantic.Retain)]
		[Transient] // it's always part of ViewControllers
		UIViewController TopViewController { get; }

		[Export ("visibleViewController", ArgumentSemantic.Retain)]
		[Transient] // it's always part of ViewControllers
		UIViewController VisibleViewController { get; }

		[Export ("viewControllers", ArgumentSemantic.Copy)]
		[PostGet ("ChildViewControllers")] // for base backing field
		[NullAllowed]
		UIViewController [] ViewControllers { get; set; }

		[Export ("setViewControllers:animated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		void SetViewControllers ([NullAllowed] UIViewController [] controllers, bool animated);

		[Export ("navigationBarHidden")]
		bool NavigationBarHidden { [Bind ("isNavigationBarHidden")] get; set; }

		[Export ("setNavigationBarHidden:animated:")]
		void SetNavigationBarHidden (bool hidden, bool animated);

		[Export ("navigationBar")]
		UINavigationBar NavigationBar { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("toolbarHidden")]
		bool ToolbarHidden { [Bind ("isToolbarHidden")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setToolbarHidden:animated:")]
		void SetToolbarHidden (bool hidden, bool animated);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("toolbar")]
		UIToolbar Toolbar { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UINavigationControllerDelegate Delegate { get; set; }

		[Field ("UINavigationControllerHideShowBarDuration")]
		nfloat HideShowBarDuration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("interactivePopGestureRecognizer", ArgumentSemantic.Copy)]
		UIGestureRecognizer InteractivePopGestureRecognizer { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBarsWhenVerticallyCompact", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsWhenVerticallyCompact { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBarsOnTap", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsOnTap { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBarsWhenKeyboardAppears", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsWhenKeyboardAppears { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBarsOnSwipe", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsOnSwipe { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("barHideOnSwipeGestureRecognizer", ArgumentSemantic.Retain)]
		UIPanGestureRecognizer BarHideOnSwipeGestureRecognizer { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("barHideOnTapGestureRecognizer", ArgumentSemantic.UnsafeUnretained)]
		UITapGestureRecognizer BarHideOnTapGestureRecognizer { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UINavigationControllerDelegate {

		[Export ("navigationController:willShowViewController:animated:"), EventArgs ("UINavigationController")]
		void WillShowViewController (UINavigationController navigationController, [Transient] UIViewController viewController, bool animated);

		[Export ("navigationController:didShowViewController:animated:"), EventArgs ("UINavigationController")]
		void DidShowViewController (UINavigationController navigationController, [Transient] UIViewController viewController, bool animated);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("navigationControllerSupportedInterfaceOrientations:")]
		[NoDefaultValue]
		[DelegateName ("Func<UINavigationController,UIInterfaceOrientationMask>")]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UINavigationController navigationController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("navigationControllerPreferredInterfaceOrientationForPresentation:")]
		[DelegateName ("Func<UINavigationController,UIInterfaceOrientation>")]
		[NoDefaultValue]
		UIInterfaceOrientation GetPreferredInterfaceOrientation (UINavigationController navigationController);

		[Export ("navigationController:interactionControllerForAnimationController:")]
		[DelegateName ("Func<UINavigationController,IUIViewControllerAnimatedTransitioning,IUIViewControllerInteractiveTransitioning>")]
		[NoDefaultValue]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UINavigationController navigationController, IUIViewControllerAnimatedTransitioning animationController);

		[Export ("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
		[DelegateName ("Func<UINavigationController,UINavigationControllerOperation,UIViewController,UIViewController,IUIViewControllerAnimatedTransitioning>")]
		[NoDefaultValue]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation (UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UINib {
		// note: the default `init` does not seems to create anything that can be used - but it does not crash when used

		[Static]
		[Export ("nibWithNibName:bundle:")]
		UINib FromName (string name, [NullAllowed] NSBundle bundleOrNil);

		[Static]
		[Export ("nibWithData:bundle:")]
		UINib FromData (NSData data, [NullAllowed] NSBundle bundleOrNil);

		[Export ("instantiateWithOwner:options:")]
		NSObject [] Instantiate ([NullAllowed] NSObject ownerOrNil, [NullAllowed] NSDictionary optionsOrNil);

		[Field ("UINibExternalObjects")]
		NSString ExternalObjectsKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UIPageControl : UIAppearance {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("numberOfPages")]
		nint Pages { get; set; }

		[Export ("currentPage")]
		nint CurrentPage { get; set; }

		[Export ("hidesForSinglePage")]
		bool HidesForSinglePage { get; set; }

		[Appearance]
		[NullAllowed]
		[Export ("pageIndicatorTintColor", ArgumentSemantic.Retain)]
		UIColor PageIndicatorTintColor { get; set; }

		[Appearance]
		[NullAllowed]
		[Export ("currentPageIndicatorTintColor", ArgumentSemantic.Retain)]
		UIColor CurrentPageIndicatorTintColor { get; set; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("backgroundStyle", ArgumentSemantic.Assign)]
		UIPageControlBackgroundStyle BackgroundStyle { get; set; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("interactionState", ArgumentSemantic.Assign)]
		UIPageControlInteractionState InteractionState { get; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowsContinuousInteraction")]
		bool AllowsContinuousInteraction { get; set; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("preferredIndicatorImage", ArgumentSemantic.Strong)]
		UIImage PreferredIndicatorImage { get; set; }

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("indicatorImageForPage:")]
		[return: NullAllowed]
		UIImage GetIndicatorImage (nint page);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setIndicatorImage:forPage:")]
		void SetIndicatorImage ([NullAllowed] UIImage image, nint page);

		[Export ("sizeForNumberOfPages:")]
		CGSize SizeForNumberOfPages (nint pageCount);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.TvOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("defersCurrentPageDisplay")]
		bool DefersCurrentPageDisplay { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.TvOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("updateCurrentPageDisplay")]
		void UpdateCurrentPageDisplay ();

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("direction", ArgumentSemantic.Assign)]
		UIPageControlDirection Direction { get; set; }

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("preferredCurrentPageIndicatorImage", ArgumentSemantic.Strong)]
		[NullAllowed]
		UIImage PreferredCurrentPageIndicatorImage { get; set; }

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("currentPageIndicatorImageForPage:")]
		[return: NullAllowed]
		UIImage GetCurrentPageIndicatorImage (nint page);

		[iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
		[Export ("setCurrentPageIndicatorImage:forPage:")]
		void SetCurrentPageIndicatorImage ([NullAllowed] UIImage image, nint page);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("progress", ArgumentSemantic.Strong)]
		UIPageControlProgress Progress { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController),
		   Delegates = new string [] { "WeakDelegate", "WeakDataSource" },
		   Events = new Type [] { typeof (UIPageViewControllerDelegate), typeof (UIPageViewControllerDataSource) })]
	interface UIPageViewController : NSCoding {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPageViewControllerDelegate Delegate { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		UIPageViewControllerDataSource DataSource { get; set; }

		[Export ("transitionStyle")]
		UIPageViewControllerTransitionStyle TransitionStyle { get; }

		[Export ("navigationOrientation")]
		UIPageViewControllerNavigationOrientation NavigationOrientation { get; }

		[Export ("spineLocation")]
		UIPageViewControllerSpineLocation SpineLocation { get; }

		[Export ("doubleSided")]
		bool DoubleSided { [Bind ("isDoubleSided")] get; set; }

		[Export ("gestureRecognizers")]
		UIGestureRecognizer [] GestureRecognizers { get; }

		[Export ("viewControllers")]
		UIViewController [] ViewControllers { get; }

		[DesignatedInitializer]
		[Export ("initWithTransitionStyle:navigationOrientation:options:")]
		NativeHandle Constructor (UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation navigationOrientation, [NullAllowed] NSDictionary options);

		[Export ("setViewControllers:direction:animated:completion:")]
		[PostGet ("ViewControllers")]
		[Async]
		void SetViewControllers (UIViewController [] viewControllers, UIPageViewControllerNavigationDirection direction, bool animated, [NullAllowed] UICompletionHandler completionHandler);

		[Field ("UIPageViewControllerOptionSpineLocationKey")]
		NSString OptionSpineLocationKey { get; }

		[Internal, Field ("UIPageViewControllerOptionInterPageSpacingKey")]
		NSString OptionInterPageSpacingKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPageViewControllerDelegate {
		[Export ("pageViewController:didFinishAnimating:previousViewControllers:transitionCompleted:"), EventArgs ("UIPageViewFinishedAnimation")]
		void DidFinishAnimating (UIPageViewController pageViewController, bool finished, UIViewController [] previousViewControllers, bool completed);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pageViewController:spineLocationForInterfaceOrientation:"), DelegateName ("UIPageViewSpineLocationCallback")]
		[DefaultValue (UIPageViewControllerSpineLocation.Mid)]
		UIPageViewControllerSpineLocation GetSpineLocation (UIPageViewController pageViewController, UIInterfaceOrientation orientation);

		[Export ("pageViewController:willTransitionToViewControllers:"), EventArgs ("UIPageViewControllerTransition")]
		void WillTransition (UIPageViewController pageViewController, UIViewController [] pendingViewControllers);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pageViewControllerSupportedInterfaceOrientations:")]
		[DelegateName ("Func<UIPageViewController,UIInterfaceOrientationMask>")]
		[DefaultValue (UIInterfaceOrientationMask.All)]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UIPageViewController pageViewController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pageViewControllerPreferredInterfaceOrientationForPresentation:")]
		[DelegateName ("Func<UIPageViewController,UIInterfaceOrientation>")]
		[DefaultValue (UIInterfaceOrientation.Portrait)]
		UIInterfaceOrientation GetPreferredInterfaceOrientationForPresentation (UIPageViewController pageViewController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPageViewControllerDataSource {
		[Abstract]
		[Export ("pageViewController:viewControllerBeforeViewController:"), DelegateName ("UIPageViewGetViewController"), DefaultValue (null)]
		UIViewController GetPreviousViewController (UIPageViewController pageViewController, UIViewController referenceViewController);

		[Abstract]
		[Export ("pageViewController:viewControllerAfterViewController:"), DelegateName ("UIPageViewGetViewController"), DefaultValue (null)]
		UIViewController GetNextViewController (UIPageViewController pageViewController, UIViewController referenceViewController);

		[Export ("presentationCountForPageViewController:"), DelegateName ("UIPageViewGetNumber"), DefaultValue (1)]
		nint GetPresentationCount (UIPageViewController pageViewController);

		[Export ("presentationIndexForPageViewController:"), DelegateName ("UIPageViewGetNumber"), DefaultValue (1)]
		nint GetPresentationIndex (UIPageViewController pageViewController);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	interface UIPasteboardChangeEventArgs {
		[Export ("UIPasteboardChangedTypesAddedKey")]
		string [] TypesAdded { get; }

		[Export ("UIPasteboardChangedTypesRemovedKey")]
		string [] TypesRemoved { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Calling -[UIPasteboard init] is not allowed.
	[DisableDefaultCtor]
	interface UIPasteboard {
		[Export ("generalPasteboard")]
		[Static]
		UIPasteboard General { get; }

		[Export ("pasteboardWithName:create:")]
		[Static]
		UIPasteboard FromName (string name, bool create);

		[Export ("pasteboardWithUniqueName")]
		[Static]
		UIPasteboard GetUnique ();

		[Export ("name")]
		string Name { get; }

		[Export ("removePasteboardWithName:"), Static]
		void Remove (string name);

		[Export ("persistent")]
		bool Persistent {
			[Bind ("isPersistent")]
			get;
			[Deprecated (PlatformName.iOS, 10, 0)]
			[Deprecated (PlatformName.MacCatalyst, 13, 1)]
			set;
		}

		[Export ("changeCount")]
		nint ChangeCount { get; }

		[Export ("pasteboardTypes")]
		string [] Types { get; }

		[Export ("containsPasteboardTypes:")]
		bool Contains (string [] pasteboardTypes);

		[Export ("dataForPasteboardType:")]
		NSData DataForPasteboardType (string pasteboardType);

		[Export ("valueForPasteboardType:")]
		NSObject GetValue (string pasteboardType);

		[Export ("setValue:forPasteboardType:")]
		void SetValue (NSObject value, string pasteboardType);

		[Export ("setData:forPasteboardType:")]
		void SetData (NSData data, string forPasteboardType);

		[Export ("numberOfItems")]
		nint Count { get; }

#if NET
		[Export ("pasteboardTypesForItemSet:")]
		[return: NullAllowed]
		NSArray<NSString> [] GetPasteBoardTypes ([NullAllowed] NSIndexSet itemSet);
#else
		[Export ("pasteboardTypesForItemSet:")]
		NSArray [] PasteBoardTypesForSet (NSIndexSet itemSet);
#endif
		[Export ("containsPasteboardTypes:inItemSet:")]
		bool Contains (string [] pasteboardTypes, [NullAllowed] NSIndexSet itemSet);

		[Export ("itemSetWithPasteboardTypes:")]
		NSIndexSet ItemSetWithPasteboardTypes (string [] pasteboardTypes);

		[Export ("valuesForPasteboardType:inItemSet:")]
		NSData [] GetValuesForPasteboardType (string pasteboardType, NSIndexSet itemSet);


		[Export ("dataForPasteboardType:inItemSet:")]
		NSData [] GetDataForPasteboardType (string pasteboardType, NSIndexSet itemSet);

		[Export ("items", ArgumentSemantic.Copy)]
		NSDictionary [] Items { get; set; }

		[Export ("addItems:")]
		void AddItems (NSDictionary [] items);

		[Field ("UIPasteboardChangedNotification")]
		[Notification (typeof (UIPasteboardChangeEventArgs))]
		NSString ChangedNotification { get; }

		[Field ("UIPasteboardChangedTypesAddedKey")]
		NSString ChangedTypesAddedKey { get; }

		[Field ("UIPasteboardChangedTypesRemovedKey")]
		NSString ChangedTypesRemovedKey { get; }

		[Field ("UIPasteboardRemovedNotification")]
		[Notification (typeof (UIPasteboardChangeEventArgs))]
		NSString RemovedNotification { get; }

		[Field ("UIPasteboardTypeListString")]
		NSArray TypeListString { get; }

		[Field ("UIPasteboardTypeListURL")]
		NSArray TypeListURL { get; }

		[Field ("UIPasteboardTypeListImage")]
		NSArray TypeListImage { get; }

		[Field ("UIPasteboardTypeListColor")]
		NSArray TypeListColor { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("UIPasteboardTypeAutomatic")]
		NSString Automatic { get; }

		[NullAllowed]
		[Export ("string", ArgumentSemantic.Copy)]
		string String { get; set; }

		[NullAllowed]
		[Export ("strings", ArgumentSemantic.Copy)]
		string [] Strings { get; set; }

		[NullAllowed]
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[NullAllowed]
		[Export ("URLs", ArgumentSemantic.Copy)]
		NSUrl [] Urls { get; set; }

		[NullAllowed]
		[Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; set; }

		// images bound manually (as it does not always returns UIImage)

		[NullAllowed]
		[Export ("color", ArgumentSemantic.Copy)]
		UIColor Color { get; set; }

		[NullAllowed]
		[Export ("colors", ArgumentSemantic.Copy)]
		UIColor [] Colors { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("setItems:options:")]
		void SetItems (NSDictionary<NSString, NSObject> [] items, NSDictionary options);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Wrap ("SetItems (items, pasteboardOptions.GetDictionary ()!)")]
		void SetItems (NSDictionary<NSString, NSObject> [] items, UIPasteboardOptions pasteboardOptions);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasStrings")]
		bool HasStrings { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasURLs")]
		bool HasUrls { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasImages")]
		bool HasImages { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasColors")]
		bool HasColors { get; }

		[Async]
		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("detectPatternsForPatterns:completionHandler:")]
		void DetectPatterns (NSSet<NSString> patterns, Action<NSSet<NSString>, NSError> completionHandler);

		[Async]
		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("detectPatternsForPatterns:inItemSet:completionHandler:")]
		void DetectPatterns (NSSet<NSString> patterns, [NullAllowed] NSIndexSet itemSet, Action<NSSet<NSString> [], NSError> completionHandler);

		[Async]
		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("detectValuesForPatterns:completionHandler:")]
		void DetectValues (NSSet<NSString> patterns, Action<NSDictionary<NSString, NSObject>, NSError> completionHandler);

		[Async]
		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("detectValuesForPatterns:inItemSet:completionHandler:")]
		void DetectValues (NSSet<NSString> patterns, [NullAllowed] NSIndexSet itemSet, Action<NSDictionary<NSString, NSObject> [], NSError> completionHandler);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("itemProviders", ArgumentSemantic.Copy)]
		NSItemProvider [] ItemProviders { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setItemProviders:localOnly:expirationDate:")]
		void SetItemProviders (NSItemProvider [] itemProviders, bool localOnly, [NullAllowed] NSDate expirationDate);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setObjects:")]
		void SetObjects (INSItemProviderWriting [] objects);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setObjects:localOnly:expirationDate:")]
		void SetObjects (INSItemProviderWriting [] objects, bool localOnly, [NullAllowed] NSDate expirationDate);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIPasteboardNames {
		[Field ("UIPasteboardNameGeneral")]
		NSString General { get; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "The 'Find' pasteboard is no longer available.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "The 'Find' pasteboard is no longer available.")]
		[Field ("UIPasteboardNameFind")]
		NSString Find { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[StrongDictionary ("UIPasteboardOptionKeys")]
	interface UIPasteboardOptions {
		NSDate ExpirationDate { get; set; }
		bool LocalOnly { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Static]
	interface UIPasteboardOptionKeys {
		[Field ("UIPasteboardOptionExpirationDate")]
		NSString ExpirationDateKey { get; }

		[Field ("UIPasteboardOptionLocalOnly")]
		NSString LocalOnlyKey { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" })]
	interface UIPickerView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

#if NET
		[NullAllowed]
		[Export ("dataSource", ArgumentSemantic.Assign)]
		NSObject WeakDataSource { get; set; }

		[NullAllowed]
		[Wrap ("WeakDataSource")]
		IUIPickerViewDataSource DataSource { get; set; }
#else
		// should have been WeakDataSource
		[NullAllowed] // by default this property is null
		[Export ("dataSource", ArgumentSemantic.Assign)]
		NSObject DataSource { get; set; }
#endif

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPickerViewDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Advice ("This property is a no-op since 7.0.")]
		[Export ("showsSelectionIndicator")]
		bool ShowSelectionIndicator { get; set; }

		[Export ("numberOfComponents")]
		nint NumberOfComponents { get; }

		[Export ("numberOfRowsInComponent:")]
		nint RowsInComponent (nint component);

		[Export ("rowSizeForComponent:")]
		CGSize RowSizeForComponent (nint component);

		[Export ("viewForRow:forComponent:")]
		UIView ViewFor (nint row, nint component);

		[Export ("reloadAllComponents")]
		void ReloadAllComponents ();

		[Export ("reloadComponent:")]
		void ReloadComponent (nint component);

		[Export ("selectRow:inComponent:animated:")]
		void Select (nint row, nint component, bool animated);

		[Export ("selectedRowInComponent:")]
		nint SelectedRowInComponent (nint component);

		// UITableViewDataSource - only implements the two required members
		// 	inlined both + UIPickerView.cs implements IUITableViewDataSource

		[Export ("tableView:numberOfRowsInSection:")]
#if NET
		nint RowsInSection (UITableView tableView, nint section);
#else
		nint RowsInSection (UITableView tableview, nint section);
#endif

		[Export ("tableView:cellForRowAtIndexPath:")]
		UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPickerViewDelegate {
		[Export ("pickerView:rowHeightForComponent:")]
		nfloat GetRowHeight (UIPickerView pickerView, nint component);

		[Export ("pickerView:widthForComponent:")]
		nfloat GetComponentWidth (UIPickerView pickerView, nint component);

		[Export ("pickerView:titleForRow:forComponent:")]
		string GetTitle (UIPickerView pickerView, nint row, nint component);

		[Export ("pickerView:viewForRow:forComponent:reusingView:")]
		UIView GetView (UIPickerView pickerView, nint row, nint component, [NullAllowed] UIView view);

		[Export ("pickerView:didSelectRow:inComponent:")]
		void Selected (UIPickerView pickerView, nint row, nint component);

		[Export ("pickerView:attributedTitleForRow:forComponent:")]
		NSAttributedString GetAttributedTitle (UIPickerView pickerView, nint row, nint component);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (UIPickerViewDelegate))]
	interface UIPickerViewAccessibilityDelegate {
		[Export ("pickerView:accessibilityLabelForComponent:")]
		string GetAccessibilityLabel (UIPickerView pickerView, nint acessibilityLabelForComponent);

		[Export ("pickerView:accessibilityHintForComponent:")]
		string GetAccessibilityHint (UIPickerView pickerView, nint component);

		[MacCatalyst (13, 1)]
		[Export ("pickerView:accessibilityAttributedLabelForComponent:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedLabel (UIPickerView pickerView, nint component);

		[MacCatalyst (13, 1)]
		[Export ("pickerView:accessibilityAttributedHintForComponent:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedHint (UIPickerView pickerView, nint component);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("pickerView:accessibilityUserInputLabelsForComponent:")]
		string [] GetAccessibilityUserInputLabels (UIPickerView pickerView, nint component);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("pickerView:accessibilityAttributedUserInputLabelsForComponent:")]
		NSAttributedString [] GetAccessibilityAttributedUserInputLabels (UIPickerView pickerView, nint component);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPickerViewDataSource {
		[Export ("numberOfComponentsInPickerView:")]
		[Abstract]
		nint GetComponentCount (UIPickerView pickerView);

		[Export ("pickerView:numberOfRowsInComponent:")]
		[Abstract]
		nint GetRowsInComponent (UIPickerView pickerView, nint component);
	}

	interface IUIPickerViewDataSource { }

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface UIPickerViewModel : UIPickerViewDataSource, UIPickerViewDelegate {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface UIContentContainer {
		[Abstract]
		[Export ("preferredContentSize")]
		CGSize PreferredContentSize { get; }

		[Abstract]
		[Export ("preferredContentSizeDidChangeForChildContentContainer:")]
		void PreferredContentSizeDidChangeForChildContentContainer (IUIContentContainer container);

		[Abstract]
		[Export ("systemLayoutFittingSizeDidChangeForChildContentContainer:")]
		void SystemLayoutFittingSizeDidChangeForChildContentContainer (IUIContentContainer container);

		[Abstract]
		[Export ("sizeForChildContentContainer:withParentContainerSize:")]
		CGSize GetSizeForChildContentContainer (IUIContentContainer contentContainer, CGSize parentContainerSize);

		[Abstract]
		[Export ("viewWillTransitionToSize:withTransitionCoordinator:")]
		void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator);

		[Abstract]
		[Export ("willTransitionToTraitCollection:withTransitionCoordinator:")]
		void WillTransitionToTraitCollection (UITraitCollection traitCollection, [NullAllowed] IUIViewControllerTransitionCoordinator coordinator);
	}

	[NoWatch, Protocol, Model]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UIAppearanceContainer {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIPresentationController init].
	partial interface UIPresentationController : UIAppearanceContainer, UITraitEnvironment, UIContentContainer, UIFocusEnvironment {
		[Export ("initWithPresentedViewController:presentingViewController:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController);

		[Export ("presentingViewController", ArgumentSemantic.Retain)]
		UIViewController PresentingViewController { get; }

		[Export ("presentedViewController", ArgumentSemantic.Retain)]
		UIViewController PresentedViewController { get; }

		[Export ("presentationStyle")]
		UIModalPresentationStyle PresentationStyle { get; }

		[Export ("containerView")]
		UIView ContainerView { get; }

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIAdaptivePresentationControllerDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the 'TraitOverrides' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the 'TraitOverrides' property instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Use the 'TraitOverrides' property instead.")]
		[Export ("overrideTraitCollection", ArgumentSemantic.Copy), NullAllowed]
		UITraitCollection OverrideTraitCollection { get; set; }

		[Export ("adaptivePresentationStyle")]
		UIModalPresentationStyle AdaptivePresentationStyle ();

		[MacCatalyst (13, 1)]
		[Export ("adaptivePresentationStyleForTraitCollection:")]
		UIModalPresentationStyle AdaptivePresentationStyle (UITraitCollection traitCollection);

		[Export ("containerViewWillLayoutSubviews")]
		void ContainerViewWillLayoutSubviews ();

		[Export ("containerViewDidLayoutSubviews")]
		void ContainerViewDidLayoutSubviews ();

		[Export ("presentedView")]
		UIView PresentedView { get; }

		[Export ("frameOfPresentedViewInContainerView")]
		CGRect FrameOfPresentedViewInContainerView { get; }

		[Export ("shouldPresentInFullscreen")]
		bool ShouldPresentInFullscreen { get; }

		[Export ("shouldRemovePresentersView")]
		bool ShouldRemovePresentersView { get; }

		[Export ("presentationTransitionWillBegin")]
		void PresentationTransitionWillBegin ();

		[Export ("presentationTransitionDidEnd:")]
		void PresentationTransitionDidEnd (bool completed);

		[Export ("dismissalTransitionWillBegin")]
		void DismissalTransitionWillBegin ();

		[Export ("dismissalTransitionDidEnd:")]
		void DismissalTransitionDidEnd (bool completed);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitOverrides")]
		IUITraitOverrides TraitOverrides { get; }
	}

	delegate void UIPreviewHandler (UIPreviewAction action, UIViewController previewViewController);

	[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
	[BaseType (typeof (NSObject))]
	interface UIPreviewAction : UIPreviewActionItem, NSCopying {
		[Static, Export ("actionWithTitle:style:handler:")]
		UIPreviewAction Create (string title, UIPreviewActionStyle style, UIPreviewHandler handler);

		[Export ("handler")]
		UIPreviewHandler Handler { get; }


	}

	[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
	[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
	[BaseType (typeof (NSObject))]
	interface UIPreviewActionGroup : UIPreviewActionItem, NSCopying {
		[Static, Export ("actionGroupWithTitle:style:actions:")]
		UIPreviewActionGroup Create (string title, UIPreviewActionStyle style, UIPreviewAction [] actions);
	}

	interface IUIPreviewActionItem { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIPreviewActionItem {
		[Abstract]
		[Export ("title")]
		string Title { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIProgressView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("initWithProgressViewStyle:")]
		NativeHandle Constructor (UIProgressViewStyle style);

		[Export ("progressViewStyle")]
		UIProgressViewStyle Style { get; set; }

		[Export ("progress")]
		float Progress { get; set; } // This is float, not nfloat.

		[Export ("progressTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor ProgressTintColor { get; set; }

		[Export ("trackTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor TrackTintColor { get; set; }

		[Export ("progressImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ProgressImage { get; set; }

		[Export ("trackImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage TrackImage { get; set; }

		[Export ("setProgress:animated:")]
		void SetProgress (float progress /* this is float, not nfloat */, bool animated);

		[MacCatalyst (13, 1)]
		[Export ("observedProgress")]
		[NullAllowed]
		NSProgress ObservedProgress { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior))]
	partial interface UIPushBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:mode:")]
		NativeHandle Constructor (IUIDynamicItem [] items, UIPushBehaviorMode mode);

		[Export ("addItem:")]
		[PostGet ("Items")]
		void AddItem (IUIDynamicItem dynamicItem);

		[Export ("removeItem:")]
		[PostGet ("Items")]
		void RemoveItem (IUIDynamicItem dynamicItem);

		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get; }

		[Export ("targetOffsetFromCenterForItem:")]
		UIOffset GetTargetOffsetFromCenter (IUIDynamicItem item);

		[Export ("setTargetOffsetFromCenter:forItem:")]
		void SetTargetOffset (UIOffset offset, IUIDynamicItem item);

		[Export ("mode")]
		UIPushBehaviorMode Mode { get; }

		[Export ("active")]
		bool Active { get; set; }

		[Export ("angle")]
		nfloat Angle { get; set; }

		[Export ("magnitude")]
		nfloat Magnitude { get; set; }

		[Export ("setAngle:magnitude:")]
		void SetAngleAndMagnitude (nfloat angle, nfloat magnitude);

		[Export ("pushDirection")]
		CGVector PushDirection { get; set; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDynamicBehavior))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: init is undefined for objects of type UISnapBehavior
	partial interface UISnapBehavior {
		[DesignatedInitializer]
		[Export ("initWithItem:snapToPoint:")]
		NativeHandle Constructor (IUIDynamicItem dynamicItem, CGPoint point);

		[Export ("damping", ArgumentSemantic.Assign)]
		nfloat Damping { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("snapPoint", ArgumentSemantic.Assign)]
		CGPoint SnapPoint { get; set; }
	}

	[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
	[NoTV, NoWatch]
	[BaseType (typeof (UIViewController))]
	// iOS6 returns the following (confusing) message with the default .ctor:
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[UIReferenceLibraryViewController initWithNibName:bundle:] is not a valid initializer. You must call -[UIReferenceLibraryViewController initWithTerm:].
	[DisableDefaultCtor]
	partial interface UIReferenceLibraryViewController : NSCoding {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("dictionaryHasDefinitionForTerm:"), Static]
		bool DictionaryHasDefinitionForTerm (string term);

		[DesignatedInitializer]
		[Export ("initWithTerm:")]
		NativeHandle Constructor (string term);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIResponder : UIAccessibilityAction, UIAccessibilityFocus, UIUserActivityRestoring, UIResponderStandardEditActions
#if !TVOS
	, UIAccessibilityDragging
#endif // !TVOS
#if IOS
	, UIPasteConfigurationSupporting, UIActivityItemsConfigurationProviding
#if __MACCATALYST__
	, NSTouchBarProvider
#endif // __MACCATALYST__
#endif // IOS
	{

		[Export ("nextResponder")]
		UIResponder NextResponder { get; }

		[Export ("canBecomeFirstResponder")]
		bool CanBecomeFirstResponder { get; }

		[Export ("becomeFirstResponder")]
		bool BecomeFirstResponder ();

		[Export ("canResignFirstResponder")]
		bool CanResignFirstResponder { get; }

		[Export ("resignFirstResponder")]
		bool ResignFirstResponder ();

		[Export ("isFirstResponder")]
		bool IsFirstResponder { get; }

		[Export ("touchesBegan:withEvent:")]
		void TouchesBegan (NSSet touches, [NullAllowed] UIEvent evt);

		[Export ("touchesMoved:withEvent:")]
		void TouchesMoved (NSSet touches, [NullAllowed] UIEvent evt);

		[Export ("touchesEnded:withEvent:")]
		void TouchesEnded (NSSet touches, [NullAllowed] UIEvent evt);

		[Export ("touchesCancelled:withEvent:")]
		void TouchesCancelled (NSSet touches, [NullAllowed] UIEvent evt);

		[Export ("motionBegan:withEvent:")]
		void MotionBegan (UIEventSubtype motion, [NullAllowed] UIEvent evt);

		[Export ("motionEnded:withEvent:")]
		void MotionEnded (UIEventSubtype motion, [NullAllowed] UIEvent evt);

		[Export ("motionCancelled:withEvent:")]
		void MotionCancelled (UIEventSubtype motion, [NullAllowed] UIEvent evt);

		[Export ("canPerformAction:withSender:")]
		bool CanPerform (Selector action, [NullAllowed] NSObject withSender);

		[Export ("undoManager"), NullAllowed]
		NSUndoManager UndoManager { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("editingInteractionConfiguration")]
		UIEditingInteractionConfiguration EditingInteractionConfiguration { get; }

		// 3.2

		[Export ("inputAccessoryView")]
		UIView InputAccessoryView { get; }

		[Export ("inputView")]
		UIView InputView { get; }

		[Export ("reloadInputViews")]
		void ReloadInputViews ();

		[Export ("remoteControlReceivedWithEvent:")]
		void RemoteControlReceived ([NullAllowed] UIEvent theEvent);

		//
		// 7.0
		//

		[Export ("keyCommands")]
		UIKeyCommand [] KeyCommands { get; }

		[Static, Export ("clearTextInputContextIdentifier:")]
		void ClearTextInputContextIdentifier (NSString identifier);

		[Export ("targetForAction:withSender:")]
		NSObject GetTargetForAction (Selector action, [NullAllowed] NSObject sender);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("buildMenuWithBuilder:")]
		void BuildMenu (IUIMenuBuilder builder);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("validateCommand:")]
		void ValidateCommand (UICommand command);

		[Export ("textInputContextIdentifier")]
		NSString TextInputContextIdentifier { get; }

		[Export ("textInputMode")]
		UITextInputMode TextInputMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("inputViewController")]
		UIInputViewController InputViewController { get; }

		[MacCatalyst (13, 1)]
		[Export ("inputAccessoryViewController")]
		UIInputViewController InputAccessoryViewController { get; }

		[MacCatalyst (13, 1)]
		[Export ("userActivity"), NullAllowed]
		NSUserActivity UserActivity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity activity);

		[MacCatalyst (13, 1)]
		[Export ("pressesBegan:withEvent:")]
		void PressesBegan (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesChanged:withEvent:")]
		void PressesChanged (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesEnded:withEvent:")]
		void PressesEnded (NSSet<UIPress> presses, UIPressesEvent evt);

		[MacCatalyst (13, 1)]
		[Export ("pressesCancelled:withEvent:")]
		void PressesCancelled (NSSet<UIPress> presses, UIPressesEvent evt);

		// from UIResponderInputViewAdditions (UIResponder) - other members already inlined

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("inputAssistantItem", ArgumentSemantic.Strong)]
		UITextInputAssistantItem InputAssistantItem { get; }

		[MacCatalyst (13, 1)]
		[Export ("touchesEstimatedPropertiesUpdated:")]
		void TouchesEstimatedPropertiesUpdated (NSSet touches);

		// from UIResponder (UIActivityItemsConfiguration)

#pragma warning disable 0109 // warning CS0109: The member 'UIResponder.ActivityItemsConfiguration' does not hide an accessible member. The new keyword is not required.
		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("activityItemsConfiguration", ArgumentSemantic.Strong)]
		new IUIActivityItemsConfigurationReading ActivityItemsConfiguration { get; set; }
#pragma warning restore

		// from UIResponder (UICaptureTextFromCameraSupporting)
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("captureTextFromCamera:")]
		void CaptureTextFromCamera ([NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[NoWatch, NoTV, NoiOS]
		[Export ("makeTouchBar")]
		[return: NullAllowed]
		NSTouchBar CreateTouchBar ();

#pragma warning disable 0108 // warning CS0108: 'NSFontAssetRequest.Progress' hides inherited member 'NSProgressReporting.Progress'. Use the new keyword if hiding was intended.
		[MacCatalyst (13, 1)]
		[NoWatch, NoTV, NoiOS]
		[Export ("touchBar", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSTouchBar TouchBar { get; set; }
#pragma warning restore
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIResponderStandardEditActions {
		[Export ("cut:")]
		void Cut ([NullAllowed] NSObject sender);

		[Export ("copy:")]
		void Copy ([NullAllowed] NSObject sender);

		[Export ("paste:")]
		void Paste ([NullAllowed] NSObject sender);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("pasteAndMatchStyle:")]
		void PasteAndMatchStyle ([NullAllowed] NSObject sender);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("pasteAndGo:")]
		void PasteAndGo ([NullAllowed] NSObject sender);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("pasteAndSearch:")]
		void PasteAndSearch ([NullAllowed] NSObject sender);

		[Export ("select:")]
		void Select ([NullAllowed] NSObject sender);

		[Export ("selectAll:")]
		void SelectAll ([NullAllowed] NSObject sender);

		[Export ("delete:")]
		void Delete ([NullAllowed] NSObject sender);

		[Export ("makeTextWritingDirectionLeftToRight:")]
		void MakeTextWritingDirectionLeftToRight ([NullAllowed] NSObject sender);

		[Export ("makeTextWritingDirectionRightToLeft:")]
		void MakeTextWritingDirectionRightToLeft ([NullAllowed] NSObject sender);

		[Export ("toggleBoldface:")]
		void ToggleBoldface ([NullAllowed] NSObject sender);

		[Export ("toggleItalics:")]
		void ToggleItalics ([NullAllowed] NSObject sender);

		[Export ("toggleUnderline:")]
		void ToggleUnderline ([NullAllowed] NSObject sender);

		[Export ("decreaseSize:")]
		void DecreaseSize ([NullAllowed] NSObject sender);

		[Export ("increaseSize:")]
		void IncreaseSize ([NullAllowed] NSObject sender);

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("updateTextAttributesWithConversionHandler:")]
		void UpdateTextAttributes (UITextAttributesConversionHandler conversionHandler);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("print:")]
		void Print ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("rename:")]
		void Rename ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("duplicate:")]
		void Duplicate ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("move:")]
		void Move ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("export:")]
		void Export ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("find:")]
		void Find ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("findAndReplace:")]
		void FindAndReplace ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("findNext:")]
		void FindNext ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("findPrevious:")]
		void FindPrevious ([NullAllowed] NSObject sender);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("useSelectionForFind:")]
		void UseSelectionForFind ([NullAllowed] NSObject sender);
	}

#if !NET // These two methods are in the UIResponderStandardEditActions protocol
	[NoWatch]
	[Category, BaseType (typeof (UIResponder))]
	interface UIResponder_NSObjectExtension {
		[Export ("decreaseSize:")]
		void DecreaseSize ([NullAllowed] NSObject sender);

		[Export ("increaseSize:")]
		void IncreaseSize ([NullAllowed] NSObject sender);
	}
#endif

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIScreen : UITraitEnvironment {
		[Export ("bounds")]
		CGRect Bounds { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use the 'Bounds' property.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'Bounds' property.")]
		[Export ("applicationFrame")]
		CGRect ApplicationFrame { get; }

		[Export ("mainScreen")]
		[Static]
		UIScreen MainScreen { get; }

		[NoTV] // Xcode 7.2
		[MacCatalyst (13, 1)]
		[Export ("availableModes", ArgumentSemantic.Copy)]
		UIScreenMode [] AvailableModes { get; }

		[NullAllowed] // by default this property is null
		[Export ("currentMode", ArgumentSemantic.Retain)]
		UIScreenMode CurrentMode {
			get;
#if !TVOS
			set;
#endif
		}

		[NoTV] // Xcode 7.2
		[MacCatalyst (13, 1)]
		[Export ("preferredMode", ArgumentSemantic.Retain)]
		UIScreenMode PreferredMode { get; }

		[Export ("mirroredScreen", ArgumentSemantic.Retain)]
		UIScreen MirroredScreen { get; }

		[Export ("screens")]
		[Static]
		UIScreen [] Screens { get; }

		[Export ("scale")]
		nfloat Scale { get; }

		[Export ("displayLinkWithTarget:selector:")]
		CADisplayLink CreateDisplayLink (NSObject target, Selector sel);

		[MacCatalyst (13, 1)]
		[Export ("maximumFramesPerSecond")]
		nint MaximumFramesPerSecond { get; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("calibratedLatency")]
		double CalibratedLatency { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("brightness")]
		nfloat Brightness { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("wantsSoftwareDimming")]
		bool WantsSoftwareDimming { get; set; }

		[Export ("overscanCompensation")]
		UIScreenOverscanCompensation OverscanCompensation { get; set; }

		[Field ("UIScreenBrightnessDidChangeNotification")]
		[Notification]
		NSString BrightnessDidChangeNotification { get; }

		[Field ("UIScreenModeDidChangeNotification")]
		[Notification]
		NSString ModeDidChangeNotification { get; }

		[Field ("UIScreenDidDisconnectNotification")]
		[Notification]
		NSString DidDisconnectNotification { get; }

		[Field ("UIScreenDidConnectNotification")]
		[Notification]
		NSString DidConnectNotification { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Notification]
		[Field ("UIScreenReferenceDisplayModeStatusDidChangeNotification")]
		NSString ReferenceDisplayModeStatusDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIScreenCapturedDidChangeNotification")]
		[Notification]
		NSString CapturedDidChangeNotification { get; }

		[return: NullAllowed]
		[Export ("snapshotViewAfterScreenUpdates:")]
		UIView SnapshotView (bool afterScreenUpdates);

		[MacCatalyst (13, 1)]
		[Export ("nativeBounds")]
		CGRect NativeBounds { get; }

		[MacCatalyst (13, 1)]
		[Export ("nativeScale")]
		nfloat NativeScale { get; }

		[MacCatalyst (13, 1)]
		[Export ("coordinateSpace")]
		IUICoordinateSpace CoordinateSpace { get; }

		[MacCatalyst (13, 1)]
		[Export ("fixedCoordinateSpace")]
		IUICoordinateSpace FixedCoordinateSpace { get; }

		[MacCatalyst (13, 1)]
		[Export ("overscanCompensationInsets")]
		UIEdgeInsets OverscanCompensationInsets { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("focusedView", ArgumentSemantic.Weak)]
		UIView FocusedView { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Check for 'UIWindowScene.FocusSystem is not null' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Check for 'UIWindowScene.FocusSystem is not null' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Check for 'UIWindowScene.FocusSystem is not null' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("supportsFocus")]
		bool SupportsFocus { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'UIWindowScene.FocusSystem.FocusedItem' instead.")]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("focusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem FocusedItem { get; }

		[MacCatalyst (13, 1)]
		[Export ("captured")]
		bool Captured { [Bind ("isCaptured")] get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("referenceDisplayModeStatus")]
		UIScreenReferenceDisplayModeStatus ReferenceDisplayModeStatus { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("currentEDRHeadroom")]
		nfloat CurrentEdrHeadroom { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("potentialEDRHeadroom")]
		nfloat PotentialEdrHeadroom { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIScrollViewDelegate) })]
	interface UIScrollView : UIFocusItemScrollableContainer {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		// moved to UIFocusItemScrollableContainer in iOS 12 - but that makes the availability information incorrect (so `new` is used to avoid compiler warnings)
		[Export ("contentOffset")]
		new CGPoint ContentOffset { get; set; }

		[Export ("contentSize")]
		new CGSize ContentSize { get; set; }

		[Export ("contentInset")]
		UIEdgeInsets ContentInset { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("adjustedContentInset")]
		UIEdgeInsets AdjustedContentInset { get; }

		[MacCatalyst (13, 1)]
		[Export ("adjustedContentInsetDidChange")]
		[RequiresSuper]
		void AdjustedContentInsetDidChange ();

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyAdjustsScrollIndicatorInsets")]
		bool AutomaticallyAdjustsScrollIndicatorInsets { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("contentInsetAdjustmentBehavior", ArgumentSemantic.Assign)]
		UIScrollViewContentInsetAdjustmentBehavior ContentInsetAdjustmentBehavior { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("contentLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide ContentLayoutGuide { get; }

		[MacCatalyst (13, 1)]
		[Export ("frameLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide FrameLayoutGuide { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIScrollViewDelegate Delegate { get; set; }

		[Export ("bounces")]
		bool Bounces { get; set; }

		[Export ("alwaysBounceVertical")]
		bool AlwaysBounceVertical { get; set; }

		[Export ("alwaysBounceHorizontal")]
		bool AlwaysBounceHorizontal { get; set; }

		[Export ("showsHorizontalScrollIndicator")]
		bool ShowsHorizontalScrollIndicator { get; set; }

		[Export ("showsVerticalScrollIndicator")]
		bool ShowsVerticalScrollIndicator { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("verticalScrollIndicatorInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets VerticalScrollIndicatorInsets { get; set; }

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("horizontalScrollIndicatorInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets HorizontalScrollIndicatorInsets { get; set; }

		[Export ("scrollIndicatorInsets")]
		UIEdgeInsets ScrollIndicatorInsets {
			[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'VerticalScrollIndicatorInsets' or 'HorizontalScrollIndicatorInsets' instead.")]
			[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'VerticalScrollIndicatorInsets' or 'HorizontalScrollIndicatorInsets' instead.")]
			[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'VerticalScrollIndicatorInsets' or 'HorizontalScrollIndicatorInsets' instead.")]
			get;
			set;
		}

		[Export ("indicatorStyle")]
		UIScrollViewIndicatorStyle IndicatorStyle { get; set; }

		[Export ("decelerationRate")]
		nfloat DecelerationRate { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("indexDisplayMode")]
		UIScrollViewIndexDisplayMode IndexDisplayMode { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pagingEnabled")]
		bool PagingEnabled { [Bind ("isPagingEnabled")] get; set; }

		[Export ("directionalLockEnabled")]
		bool DirectionalLockEnabled { [Bind ("isDirectionalLockEnabled")] get; set; }

		[Export ("scrollEnabled")]
		bool ScrollEnabled { [Bind ("isScrollEnabled")] get; set; }

		[Export ("tracking")]
		bool Tracking { [Bind ("isTracking")] get; }

		[Export ("dragging")]
		bool Dragging { [Bind ("isDragging")] get; }

		[Export ("decelerating")]
		bool Decelerating { [Bind ("isDecelerating")] get; }

		[Export ("setContentOffset:animated:")]
		void SetContentOffset (CGPoint contentOffset, bool animated);

		[Export ("scrollRectToVisible:animated:")]
		void ScrollRectToVisible (CGRect rect, bool animated);

		[Export ("flashScrollIndicators")]
		void FlashScrollIndicators ();

		[Export ("delaysContentTouches")]
		bool DelaysContentTouches { get; set; }

		[Export ("canCancelContentTouches")]
		bool CanCancelContentTouches { get; set; }

		[Export ("touchesShouldBegin:withEvent:inContentView:")]
		bool TouchesShouldBegin (NSSet touches, UIEvent withEvent, UIView inContentView);

		[Export ("touchesShouldCancelInContentView:")]
		bool TouchesShouldCancelInContentView (UIView view);

		[Export ("minimumZoomScale")]
		nfloat MinimumZoomScale { get; set; }

		[Export ("maximumZoomScale")]
		nfloat MaximumZoomScale { get; set; }

		[Export ("zoomScale")]
		nfloat ZoomScale { get; set; }

		[Export ("setZoomScale:animated:")]
		void SetZoomScale (nfloat scale, bool animated);

		[Export ("zoomToRect:animated:")]
		void ZoomToRect (CGRect rect, bool animated);

		[Export ("bouncesZoom")]
		bool BouncesZoom { get; set; }

		[Export ("zooming")]
		bool Zooming { [Bind ("isZooming")] get; }

		[Export ("zoomBouncing")]
		bool ZoomBouncing { [Bind ("isZoomBouncing")] get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("scrollsToTop")]
		bool ScrollsToTop { get; set; }

		[Export ("panGestureRecognizer")]
		UIPanGestureRecognizer PanGestureRecognizer { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("pinchGestureRecognizer")]
		UIPinchGestureRecognizer PinchGestureRecognizer { get; }

		[Field ("UIScrollViewDecelerationRateNormal")]
		nfloat DecelerationRateNormal { get; }

		[Field ("UIScrollViewDecelerationRateFast")]
		nfloat DecelerationRateFast { get; }

		[Export ("keyboardDismissMode")]
		UIScrollViewKeyboardDismissMode KeyboardDismissMode { get; set; }

		[NoWatch]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Configuring the 'PanGestureRecognizer' for indirect scrolling automatically supports directional presses now, so this property is no longer useful.")]
		[MacCatalyst (13, 1)]
		[Export ("directionalPressGestureRecognizer")]
		UIGestureRecognizer DirectionalPressGestureRecognizer { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("refreshControl", ArgumentSemantic.Strong)]
		UIRefreshControl RefreshControl { get; set; }

		[NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("allowsKeyboardScrolling")]
		bool AllowsKeyboardScrolling { get; set; }
	}

	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIScrollViewDelegate {

		[Export ("scrollViewDidScroll:"), EventArgs ("UIScrollView")]
		void Scrolled (UIScrollView scrollView);

		[Export ("scrollViewWillBeginDragging:"), EventArgs ("UIScrollView")]
		void DraggingStarted (UIScrollView scrollView);

		[Export ("scrollViewDidEndDragging:willDecelerate:"), EventArgs ("Dragging")]
		void DraggingEnded (UIScrollView scrollView, [EventName ("decelerate")] bool willDecelerate);

		[Export ("scrollViewWillBeginDecelerating:"), EventArgs ("UIScrollView")]
		void DecelerationStarted (UIScrollView scrollView);

		[Export ("scrollViewDidEndDecelerating:"), EventArgs ("UIScrollView")]
		void DecelerationEnded (UIScrollView scrollView);

		[Export ("scrollViewDidEndScrollingAnimation:"), EventArgs ("UIScrollView")]
		void ScrollAnimationEnded (UIScrollView scrollView);

		[Export ("viewForZoomingInScrollView:"), DelegateName ("UIScrollViewGetZoomView"), DefaultValue ("null")]
		UIView ViewForZoomingInScrollView (UIScrollView scrollView);

		[Export ("scrollViewShouldScrollToTop:"), DelegateName ("UIScrollViewCondition"), DefaultValue ("true")]
		bool ShouldScrollToTop (UIScrollView scrollView);

		[Export ("scrollViewDidScrollToTop:"), EventArgs ("UIScrollView")]
		void ScrolledToTop (UIScrollView scrollView);

		[Export ("scrollViewDidEndZooming:withView:atScale:"), EventArgs ("ZoomingEnded")]
		void ZoomingEnded (UIScrollView scrollView, UIView withView, nfloat atScale);

		[Export ("scrollViewDidZoom:"), EventArgs ("UIScrollView")]
		void DidZoom (UIScrollView scrollView);

		[Export ("scrollViewWillBeginZooming:withView:"), EventArgs ("UIScrollViewZooming")]
		void ZoomingStarted (UIScrollView scrollView, UIView view);

		[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:"), EventArgs ("WillEndDragging")]
		void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset);

		[MacCatalyst (13, 1)]
		[Export ("scrollViewDidChangeAdjustedContentInset:")]
		void DidChangeAdjustedContentInset (UIScrollView scrollView);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (UIScrollViewDelegate))]
	interface UIScrollViewAccessibilityDelegate {
		[Export ("accessibilityScrollStatusForScrollView:")]
		string GetAccessibilityScrollStatus (UIScrollView scrollView);

		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedScrollStatusForScrollView:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedScrollStatus (UIScrollView scrollView);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UISearchBarDelegate) })]
#if TVOS
	[DisableDefaultCtor] // - (instancetype)init __TVOS_PROHIBITED;
#endif
	interface UISearchBar : UIBarPositioning, UITextInputTraits, UILookToDictateCapable
#if !TVOS
		, NSCoding
#endif
	{
		[NoTV]
		[MacCatalyst (13, 1)]
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISearchBarDelegate Delegate { get; set; }

		[Export ("text", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Text { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("prompt", ArgumentSemantic.Copy)]
		string Prompt { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("placeholder", ArgumentSemantic.Copy)]
		string Placeholder { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showsBookmarkButton")]
		bool ShowsBookmarkButton { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showsCancelButton")]
		bool ShowsCancelButton { get; set; }

		[Export ("selectedScopeButtonIndex")]
		nint SelectedScopeButtonIndex { get; set; }

		[Export ("showsScopeBar")]
		bool ShowsScopeBar { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("setShowsScopeBar:animated:")]
		void SetShowsScopeBar (bool show, bool animate);

		[NullAllowed] // by default this property is null
		[Export ("scopeButtonTitles", ArgumentSemantic.Copy)]
		string [] ScopeButtonTitles { get; set; }

		[Export ("translucent", ArgumentSemantic.Assign)]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setShowsCancelButton:animated:")]
		void SetShowsCancelButton (bool showsCancelButton, bool animated);

		// 3.2
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("searchResultsButtonSelected")]
		bool SearchResultsButtonSelected { [Bind ("isSearchResultsButtonSelected")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showsSearchResultsButton")]
		bool ShowsSearchResultsButton { get; set; }

		// 5.0
		[Export ("backgroundImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage BackgroundImage { get; set; }

		[Export ("scopeBarBackgroundImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ScopeBarBackgroundImage { get; set; }

		[Appearance]
		[Export ("searchFieldBackgroundPositionAdjustment")]
		UIOffset SearchFieldBackgroundPositionAdjustment { get; set; }

		[Appearance]
		[Export ("searchTextPositionAdjustment")]
		UIOffset SearchTextPositionAdjustment { get; set; }

		[Export ("setSearchFieldBackgroundImage:forState:")]
		[Appearance]
		void SetSearchFieldBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState state);

		[Export ("searchFieldBackgroundImageForState:")]
		[Appearance]
		UIImage GetSearchFieldBackgroundImage (UIControlState state);

		[Export ("setImage:forSearchBarIcon:state:")]
		[Appearance]
		void SetImageforSearchBarIcon ([NullAllowed] UIImage iconImage, UISearchBarIcon icon, UIControlState state);

		[Export ("imageForSearchBarIcon:state:")]
		[Appearance]
		UIImage GetImageForSearchBarIcon (UISearchBarIcon icon, UIControlState state);

		[Export ("setScopeBarButtonBackgroundImage:forState:")]
		[Appearance]
		void SetScopeBarButtonBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState state);

		[Export ("scopeBarButtonBackgroundImageForState:")]
		[Appearance]
		UIImage GetScopeBarButtonBackgroundImage (UIControlState state);

		[Export ("setScopeBarButtonDividerImage:forLeftSegmentState:rightSegmentState:")]
		[Appearance]
		void SetScopeBarButtonDividerImage ([NullAllowed] UIImage dividerImage, UIControlState leftState, UIControlState rightState);

		[Export ("scopeBarButtonDividerImageForLeftSegmentState:rightSegmentState:")]
		[Appearance]
		UIImage GetScopeBarButtonDividerImage (UIControlState leftState, UIControlState rightState);

		[Export ("setScopeBarButtonTitleTextAttributes:forState:"), Internal]
		[Appearance]
		void _SetScopeBarButtonTitle (NSDictionary attributes, UIControlState state);

		[Export ("scopeBarButtonTitleTextAttributesForState:"), Internal]
		[Appearance]
		NSDictionary _GetScopeBarButtonTitleTextAttributes (UIControlState state);

		[Appearance]
		[Export ("setPositionAdjustment:forSearchBarIcon:")]
		void SetPositionAdjustmentforSearchBarIcon (UIOffset adjustment, UISearchBarIcon icon);

		[Appearance]
		[Export ("positionAdjustmentForSearchBarIcon:")]
		UIOffset GetPositionAdjustmentForSearchBarIcon (UISearchBarIcon icon);

		[Export ("inputAccessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView InputAccessoryView { get; set; }

		[TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Appearance]
		[Export ("setBackgroundImage:forBarPosition:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarPosition barPosition, UIBarMetrics barMetrics);

		[Export ("backgroundImageForBarPosition:barMetrics:")]
		[Appearance]
		UIImage BackgroundImageForBarPosition (UIBarPosition barPosition, UIBarMetrics barMetrics);

		[Export ("barTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor BarTintColor { get; set; }

		[Export ("searchBarStyle")]
		UISearchBarStyle SearchBarStyle { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("inputAssistantItem", ArgumentSemantic.Strong)]
		UITextInputAssistantItem InputAssistantItem { get; }

		// UISearchBar (UITokenSearch)

		[NoTV]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("searchTextField")]
		UISearchTextField SearchTextField { get; }

		[NoWatch, NoTV, NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("lookToDictateEnabled")]
		new bool LookToDictateEnabled { [Bind ("isLookToDictateEnabled")] get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarPositioningDelegate))]
	[Model]
	[Protocol]
	interface UISearchBarDelegate {
		[Export ("searchBarShouldBeginEditing:"), DefaultValue (true), DelegateName ("UISearchBarPredicate")]
		bool ShouldBeginEditing (UISearchBar searchBar);

		[Export ("searchBarTextDidBeginEditing:"), EventArgs ("UISearchBar")]
		void OnEditingStarted (UISearchBar searchBar);

		[Export ("searchBarShouldEndEditing:"), DelegateName ("UISearchBarPredicate"), DefaultValue (true)]
		bool ShouldEndEditing (UISearchBar searchBar);

		[Export ("searchBarTextDidEndEditing:"), EventArgs ("UISearchBar")]
		void OnEditingStopped (UISearchBar searchBar);

		[Export ("searchBar:textDidChange:"), EventArgs ("UISearchBarTextChanged")]
		void TextChanged (UISearchBar searchBar, string searchText);

		[Export ("searchBar:shouldChangeTextInRange:replacementText:"), DefaultValue (true), DelegateName ("UISearchBarRangeEventArgs")]
		bool ShouldChangeTextInRange (UISearchBar searchBar, NSRange range, string text);

		[Export ("searchBarSearchButtonClicked:"), EventArgs ("UISearchBar")]
		void SearchButtonClicked (UISearchBar searchBar);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("searchBarBookmarkButtonClicked:"), EventArgs ("UISearchBar")]
		void BookmarkButtonClicked (UISearchBar searchBar);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("searchBarCancelButtonClicked:"), EventArgs ("UISearchBar")]
		void CancelButtonClicked (UISearchBar searchBar);

		[Export ("searchBar:selectedScopeButtonIndexDidChange:"), EventArgs ("UISearchBarButtonIndex")]
		void SelectedScopeButtonIndexChanged (UISearchBar searchBar, nint selectedScope);

		[NoTV]
		[MacCatalyst (13, 1)]

		[Export ("searchBarResultsListButtonClicked:"), EventArgs ("UISearchBar")]
		void ListButtonClicked (UISearchBar searchBar);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UISearchContainerViewController {
		// inlined
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("searchController", ArgumentSemantic.Strong)]
		UISearchController SearchController { get; }

		[Export ("initWithSearchController:")]
		NativeHandle Constructor (UISearchController searchController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // designated
	partial interface UISearchController : UIViewControllerTransitioningDelegate, UIViewControllerAnimatedTransitioning {
		[Export ("init")]
		[Advice ("It's recommended to use the constructor that takes a 'UIViewController searchResultsController' in order to create/initialize an attached 'UISearchBar'.")]
		NativeHandle Constructor ();

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithSearchResultsController:")]
		[Advice ("You can pass a null 'UIViewController' to display the search results in the same view.")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] UIViewController searchResultsController);

		[NullAllowed] // by default this property is null
		[Export ("searchResultsUpdater", ArgumentSemantic.UnsafeUnretained)]
		NSObject WeakSearchResultsUpdater { get; set; }

		[Wrap ("WeakSearchResultsUpdater")]
		[Protocolize]
		UISearchResultsUpdating SearchResultsUpdater { get; set; }

		[Export ("active", ArgumentSemantic.UnsafeUnretained)]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISearchControllerDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ObscuresBackgroundDuringPresentation' instead.")]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ObscuresBackgroundDuringPresentation' instead.")]
		[Export ("dimsBackgroundDuringPresentation", ArgumentSemantic.UnsafeUnretained)]
		bool DimsBackgroundDuringPresentation { get; set; }

		[Export ("hidesNavigationBarDuringPresentation", ArgumentSemantic.UnsafeUnretained)]
		bool HidesNavigationBarDuringPresentation { get; set; }

		[Export ("searchResultsController", ArgumentSemantic.Retain)]
		UIViewController SearchResultsController { get; }

		[Export ("searchBar", ArgumentSemantic.Retain)]
		UISearchBar SearchBar { get; }

		[MacCatalyst (13, 1)]
		[Export ("obscuresBackgroundDuringPresentation")]
		bool ObscuresBackgroundDuringPresentation { get; set; }

		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyShowsSearchResultsController")]
		bool AutomaticallyShowsSearchResultsController { get; set; }

		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("showsSearchResultsController")]
		bool ShowsSearchResultsController { get; set; }

		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyShowsCancelButton")]
		bool AutomaticallyShowsCancelButton { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the 'ScopeBarActivation' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use the 'ScopeBarActivation' property instead.")]
		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use the 'ScopeBarActivation' property instead.")]
		[iOS (13, 0), TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("automaticallyShowsScopeBar")]
		bool AutomaticallyShowsScopeBar { get; set; }

		[TV (14, 0), NoWatch, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("searchSuggestions", ArgumentSemantic.Copy)]
		IUISearchSuggestion [] SearchSuggestions { get; set; }

		[Deprecated (PlatformName.TvOS, 16, 0, message: "Use UIViewController.SetContentScrollView on the SearchResultsController instead.")]
		[TV (14, 0), NoWatch, NoiOS]
		[NoMacCatalyst]
		[NullAllowed, Export ("searchControllerObservedScrollView", ArgumentSemantic.Strong)]
		UIScrollView SearchControllerObservedScrollView { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("scopeBarActivation", ArgumentSemantic.Assign)]
		UISearchControllerScopeBarActivation ScopeBarActivation { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("ignoresSearchSuggestionsForSearchBarPlacementStacked")]
		bool IgnoresSearchSuggestionsForSearchBarPlacementStacked { get; set; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("searchBarPlacement")]
		UINavigationItemSearchBarPlacement SearchBarPlacement { get; }

	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UISearchControllerDelegate {
		[Export ("willPresentSearchController:")]
		void WillPresentSearchController (UISearchController searchController);

		[Export ("didPresentSearchController:")]
		void DidPresentSearchController (UISearchController searchController);

		[Export ("willDismissSearchController:")]
		void WillDismissSearchController (UISearchController searchController);

		[Export ("didDismissSearchController:")]
		void DidDismissSearchController (UISearchController searchController);

		[Export ("presentSearchController:")]
		void PresentSearchController (UISearchController searchController);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("searchController:willChangeToSearchBarPlacement:")]
		void WillChangeToSearchBarPlacement (UISearchController searchController, UINavigationItemSearchBarPlacement newPlacement);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("searchController:didChangeFromSearchBarPlacement:")]
		void DidChangeFromSearchBarPlacement (UISearchController searchController, UINavigationItemSearchBarPlacement previousPlacement);
	}

	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController'.")]
	[NoMacCatalyst, NoWatch] // Objective-C exception thrown.  Name: NSGenericException Reason: UISearchDisplayController is no longer supported when linking against this version of iOS. Please migrate your application to UISearchController.
	[NoTV]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController'.")]
	interface UISearchDisplayController {
		[Export ("initWithSearchBar:contentsController:")]
		[PostGet ("SearchBar")]
		[PostGet ("SearchContentsController")]
		NativeHandle Constructor (UISearchBar searchBar, UIViewController viewController);

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISearchDisplayDelegate Delegate { get; set; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("setActive:animated:")]
		void SetActive (bool visible, bool animated);

		[Export ("searchBar")]
		UISearchBar SearchBar { get; }

		[Export ("searchContentsController")]
		UIViewController SearchContentsController { get; }

		[Export ("searchResultsTableView")]
		UITableView SearchResultsTableView { get; }

		[Export ("searchResultsDataSource", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject SearchResultsWeakDataSource { get; set; }

		[Wrap ("SearchResultsWeakDataSource")]
		[Protocolize]
		UITableViewDataSource SearchResultsDataSource { get; set; }

		[Export ("searchResultsDelegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject SearchResultsWeakDelegate { get; set; }

		[Wrap ("SearchResultsWeakDelegate")]
		[Protocolize]
		UITableViewDelegate SearchResultsDelegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("searchResultsTitle", ArgumentSemantic.Copy)]
		string SearchResultsTitle { get; set; }

		[Export ("displaysSearchBarInNavigationBar", ArgumentSemantic.Assign)]
		bool DisplaysSearchBarInNavigationBar { get; set; }

		[Export ("navigationItem")]
		UINavigationItem NavigationItem { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	[NoMacCatalyst, NoWatch]
	interface UISearchDisplayDelegate {

		[Export ("searchDisplayControllerWillBeginSearch:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillBeginSearch (UISearchDisplayController controller);

		[Export ("searchDisplayControllerDidBeginSearch:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidBeginSearch (UISearchDisplayController controller);

		[Export ("searchDisplayControllerWillEndSearch:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillEndSearch (UISearchDisplayController controller);

		[Export ("searchDisplayControllerDidEndSearch:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidEndSearch (UISearchDisplayController controller);

		[Export ("searchDisplayController:didLoadSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidLoadSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willUnloadSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillUnloadSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willShowSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillShowSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:didShowSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidShowSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willHideSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillHideSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:didHideSearchResultsTableView:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidHideSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:shouldReloadTableForSearchString:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		bool ShouldReloadForSearchString (UISearchDisplayController controller, string forSearchString);

		[Export ("searchDisplayController:shouldReloadTableForSearchScope:")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		bool ShouldReloadForSearchScope (UISearchDisplayController controller, nint forSearchOption);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UISearchResultsUpdating {
		[Abstract]
		[Export ("updateSearchResultsForSearchController:")]
		void UpdateSearchResultsForSearchController (UISearchController searchController);

		[TV (14, 0), NoWatch, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("updateSearchResultsForSearchController:selectingSearchSuggestion:")]
		void UpdateSearchResults (UISearchController searchController, IUISearchSuggestion searchSuggestion);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UISegmentedControl
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		NativeHandle Constructor (NSArray items);

		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithFrame:actions:")]
		NativeHandle Constructor (CGRect frame, UIAction [] actions);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("insertSegmentWithAction:atIndex:animated:")]
		void InsertSegment (UIAction action, nuint segment, bool animated);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setAction:forSegmentAtIndex:")]
		void SetAction (UIAction action, nuint segment);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("actionForSegmentAtIndex:")]
		[return: NullAllowed]
		UIAction GetAction (nuint segment);

		[NoWatch, TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("segmentIndexForActionIdentifier:")]
		nint GetSegmentIndex (string actionIdentifier);

		[Export ("segmentedControlStyle")]
		[NoTV]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 7, 0, message: "The 'SegmentedControlStyle' property no longer has any effect.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "The 'SegmentedControlStyle' property no longer has any effect.")]
		UISegmentedControlStyle ControlStyle { get; set; }

		[Export ("momentary")]
		bool Momentary { [Bind ("isMomentary")] get; set; }

		[Export ("numberOfSegments")]
		nint NumberOfSegments { get; }

		[Export ("insertSegmentWithTitle:atIndex:animated:")]
		void InsertSegment (string title, nint pos, bool animated);

		[Export ("insertSegmentWithImage:atIndex:animated:")]
		void InsertSegment (UIImage image, nint pos, bool animated);

		[Export ("removeSegmentAtIndex:animated:")]
		void RemoveSegmentAtIndex (nint segment, bool animated);

		[Export ("removeAllSegments")]
		void RemoveAllSegments ();

		[Export ("setTitle:forSegmentAtIndex:")]
		void SetTitle (string title, nint segment);

		[Export ("titleForSegmentAtIndex:")]
		string TitleAt (nint segment);

		[Export ("setImage:forSegmentAtIndex:")]
		void SetImage (UIImage image, nint segment);

		[Export ("imageForSegmentAtIndex:")]
		UIImage ImageAt (nint segment);

		[Export ("setWidth:forSegmentAtIndex:")]
		void SetWidth (nfloat width, nint segment);

		[Export ("widthForSegmentAtIndex:")]
		nfloat SegmentWidth (nint segment);

		[Export ("setContentOffset:forSegmentAtIndex:")]
		void SetContentOffset (CGSize offset, nint segment);

		[Export ("contentOffsetForSegmentAtIndex:")]
		CGSize GetContentOffset (nint segment);

		[Export ("setEnabled:forSegmentAtIndex:")]
		void SetEnabled (bool enabled, nint segment);

		[Export ("isEnabledForSegmentAtIndex:")]
		bool IsEnabled (nint segment);

		[Export ("selectedSegmentIndex")]
		nint SelectedSegment { get; set; }

		[Appearance]
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("selectedSegmentTintColor", ArgumentSemantic.Strong)]
		UIColor SelectedSegmentTintColor { get; set; }

		[Export ("apportionsSegmentWidthsByContent")]
		bool ApportionsSegmentWidthsByContent { get; set; }

		[Export ("setBackgroundImage:forState:barMetrics:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState state, UIBarMetrics barMetrics);

		[Export ("backgroundImageForState:barMetrics:")]
		[Appearance]
		UIImage GetBackgroundImage (UIControlState state, UIBarMetrics barMetrics);

		[Export ("setDividerImage:forLeftSegmentState:rightSegmentState:barMetrics:")]
		[Appearance]
		void SetDividerImage ([NullAllowed] UIImage dividerImage, UIControlState leftSegmentState, UIControlState rightSegmentState, UIBarMetrics barMetrics);

		[Export ("dividerImageForLeftSegmentState:rightSegmentState:barMetrics:")]
		[Appearance]
		[return: NullAllowed]
#if NET
		UIImage GetDividerImage (UIControlState leftState, UIControlState rightState, UIBarMetrics barMetrics);
#else
		UIImage DividerImageForLeftSegmentStaterightSegmentStatebarMetrics (UIControlState leftState, UIControlState rightState, UIBarMetrics barMetrics);
#endif

		[Export ("setTitleTextAttributes:forState:"), Internal]
		[Appearance]
		void _SetTitleTextAttributes (NSDictionary attributes, UIControlState state);

		[Export ("titleTextAttributesForState:"), Internal]
		[Appearance]
		NSDictionary _GetTitleTextAttributes (UIControlState state);

		[Export ("setContentPositionAdjustment:forSegmentType:barMetrics:")]
		[Appearance]
		void SetContentPositionAdjustment (UIOffset adjustment, UISegmentedControlSegment leftCenterRightOrAlone, UIBarMetrics barMetrics);

		[Export ("contentPositionAdjustmentForSegmentType:barMetrics:")]
		[Appearance]
		UIOffset ContentPositionAdjustment (UISegmentedControlSegment leftCenterRightOrAlone, UIBarMetrics barMetrics);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UISlider {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("value")]
		float Value { get; set; } // This is float, not nfloat

		[Export ("minimumValue")]
		float MinValue { get; set; } // This is float, not nfloat

		[Export ("maximumValue")]
		float MaxValue { get; set; } // This is float, not nfloat

		[Export ("minimumValueImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage MinValueImage { get; set; }

		[Export ("maximumValueImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage MaxValueImage { get; set; }

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")] get; set; }

		[Export ("currentThumbImage")]
		UIImage CurrentThumbImage { get; }

		[Export ("currentMinimumTrackImage")]
		UIImage CurrentMinTrackImage { get; }

		[Export ("currentMaximumTrackImage")]
		UIImage CurrentMaxTrackImage { get; }

		[Export ("setValue:animated:")]
		void SetValue (float value /* This is float, not nfloat */, bool animated);

		[Export ("setThumbImage:forState:")]
		[PostGet ("CurrentThumbImage")]
		[Appearance]
		void SetThumbImage ([NullAllowed] UIImage image, UIControlState forState);

		[Export ("setMinimumTrackImage:forState:")]
		[PostGet ("CurrentMinTrackImage")]
		[Appearance]
		void SetMinTrackImage ([NullAllowed] UIImage image, UIControlState forState);

		[Export ("setMaximumTrackImage:forState:")]
		[PostGet ("CurrentMaxTrackImage")]
		[Appearance]
		void SetMaxTrackImage ([NullAllowed] UIImage image, UIControlState forState);

		[Export ("thumbImageForState:")]
		[Appearance]
		UIImage ThumbImage (UIControlState forState);

		[Export ("minimumTrackImageForState:")]
		[Appearance]
		UIImage MinTrackImage (UIControlState forState);

		[Export ("maximumTrackImageForState:")]
		[Appearance]
		UIImage MaxTrackImage (UIControlState forState);

		[Export ("minimumValueImageRectForBounds:")]
		CGRect MinValueImageRectForBounds (CGRect forBounds);

		[Export ("maximumValueImageRectForBounds:")]
		CGRect MaxValueImageRectForBounds (CGRect forBounds);

		[Export ("trackRectForBounds:")]
		CGRect TrackRectForBounds (CGRect forBounds);

		[Export ("thumbRectForBounds:trackRect:value:")]
		CGRect ThumbRectForBounds (CGRect bounds, CGRect trackRect, float value /* This is float, not nfloat */);

		[Export ("minimumTrackTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor MinimumTrackTintColor { get; set; }

		[Export ("maximumTrackTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor MaximumTrackTintColor { get; set; }

		[Export ("thumbTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor ThumbTintColor { get; set; }

		// From UISlider (UIBehavioralStyle)

		[NoWatch, NoTV, MacCatalyst (15, 0), iOS (15, 0)]
		[Export ("behavioralStyle")]
		UIBehavioralStyle BehavioralStyle { get; }

		[NoWatch, NoTV, MacCatalyst (15, 0), iOS (15, 0)]
		[Export ("preferredBehavioralStyle", ArgumentSemantic.Assign)]
		UIBehavioralStyle PreferredBehavioralStyle { get; set; }
	}

	[Static]
	interface UIStringAttributeKey {
		[Field ("NSFontAttributeName")]
		NSString Font { get; }

		[Field ("NSForegroundColorAttributeName")]
		NSString ForegroundColor { get; }

		[Field ("NSBackgroundColorAttributeName")]
		NSString BackgroundColor { get; }

		[Field ("NSStrokeColorAttributeName")]
		NSString StrokeColor { get; }

		[Field ("NSStrikethroughStyleAttributeName")]
		NSString StrikethroughStyle { get; }

		[Field ("NSShadowAttributeName")]
		NSString Shadow { get; }

		[Field ("NSParagraphStyleAttributeName")]
		NSString ParagraphStyle { get; }

		[Field ("NSLigatureAttributeName")]
		NSString Ligature { get; }

		[Field ("NSKernAttributeName")]
		NSString KerningAdjustment { get; }

		[Field ("NSUnderlineStyleAttributeName")]
		NSString UnderlineStyle { get; }

		[Field ("NSStrokeWidthAttributeName")]
		NSString StrokeWidth { get; }

		[Field ("NSVerticalGlyphFormAttributeName")]
		NSString VerticalGlyphForm { get; }

		[Field ("NSTextEffectAttributeName")]
		NSString TextEffect { get; }

		[Field ("NSAttachmentAttributeName")]
		NSString Attachment { get; }

		[Field ("NSLinkAttributeName")]
		NSString Link { get; }

		[Field ("NSBaselineOffsetAttributeName")]
		NSString BaselineOffset { get; }

		[Field ("NSUnderlineColorAttributeName")]
		NSString UnderlineColor { get; }

		[Field ("NSStrikethroughColorAttributeName")]
		NSString StrikethroughColor { get; }

		[Field ("NSObliquenessAttributeName")]
		NSString Obliqueness { get; }

		[Field ("NSExpansionAttributeName")]
		NSString Expansion { get; }

		[Field ("NSWritingDirectionAttributeName")]
		NSString WritingDirection { get; }

		[TV (14, 0), Watch (7, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSTrackingAttributeName")]
		NSString Tracking { get; }

		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("UITextItemTagAttributeName")]
		NSString Name { get; }

		//
		// These are internal, if we choose to expose these, we should
		// put them on a better named class
		//
		[Internal, Field ("NSTextEffectLetterpressStyle")]
		NSString NSTextEffectLetterpressStyle { get; }

		// we do not seem to expose other options like NSDefaultAttributesDocumentOption so keeping these as is for now
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Internal, Field ("NSTargetTextScalingDocumentOption")]
		NSString TargetTextScalingDocumentOption { get; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Internal, Field ("NSSourceTextScalingDocumentOption")]
		NSString SourceTextScalingDocumentOption { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UISwitch : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("on")]
		bool On { [Bind ("isOn")] get; set; }

		[Export ("setOn:animated:")]
		void SetState (bool newState, bool animated);


		[Export ("onTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor OnTintColor { get; set; }

		[Appearance]
		[NullAllowed]
		[Export ("thumbTintColor", ArgumentSemantic.Retain)]
		UIColor ThumbTintColor { get; set; }

		[Appearance]
		[Export ("onImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage OnImage { get; set; }

		[Appearance]
		[NullAllowed]
		[Export ("offImage", ArgumentSemantic.Retain)]
		UIImage OffImage { get; set; }

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("title")]
		string Title { get; set; }

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("style")]
		UISwitchStyle Style { get; }

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredStyle", ArgumentSemantic.Assign)]
		UISwitchStyle PreferredStyle { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UITabBarDelegate) })]
	interface UITabBar
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		UITabBarDelegate Delegate { get; set; }

		[Export ("items", ArgumentSemantic.Copy)]
		[NullAllowed]
		[PostGet ("SelectedItem")]
		UITabBarItem [] Items { get; set; }

		[Export ("selectedItem", ArgumentSemantic.Weak), NullAllowed]
		UITabBarItem SelectedItem { get; set; }

		[Export ("setItems:animated:")]
		[PostGet ("Items")] // that will trigger a (required) PostGet on SelectedItems too
		void SetItems ([NullAllowed] UITabBarItem [] items, bool animated);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("beginCustomizingItems:")]
		void BeginCustomizingItems ([NullAllowed] UITabBarItem [] items);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("endCustomizingAnimated:")]
		bool EndCustomizing (bool animated);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("isCustomizing")]
		bool IsCustomizing { get; }

		[NoTV]
		[Export ("selectedImageTintColor", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[NullAllowed]
		[Appearance]
		UIColor SelectedImageTintColor { get; set; }

		[Export ("backgroundImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		[Appearance]
		UIImage BackgroundImage { get; set; }

		[Export ("selectionIndicatorImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		[Appearance]
		UIImage SelectionIndicatorImage { get; set; }

		[Export ("shadowImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ShadowImage { get; set; }

		[Export ("barTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor BarTintColor { get; set; }

		[Appearance]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("itemPositioning")]
		UITabBarItemPositioning ItemPositioning { get; set; }

		[Appearance]
		[Export ("itemWidth")]
		nfloat ItemWidth { get; set; }

		[Appearance]
		[Export ("itemSpacing")]
		nfloat ItemSpacing { get; set; }

		[Appearance]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[Export ("translucent")]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		[Appearance]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("unselectedItemTintColor", ArgumentSemantic.Copy)]
		UIColor UnselectedItemTintColor { get; set; }

		[Appearance]
		[TV (13, 0), iOS (13, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("standardAppearance", ArgumentSemantic.Copy)]
		UITabBarAppearance StandardAppearance { get; set; }

		[Appearance]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
		[NullAllowed, Export ("scrollEdgeAppearance", ArgumentSemantic.Copy)]
		UITabBarAppearance ScrollEdgeAppearance { get; set; }

		[TV (13, 0), NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("leadingAccessoryView", ArgumentSemantic.Strong)]
		UIView LeadingAccessoryView { get; }

		[TV (13, 0), NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("trailingAccessoryView", ArgumentSemantic.Strong)]
		UIView TrailingAccessoryView { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UITabBarControllerDelegate) })]
	interface UITabBarController : UITabBarDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[NullAllowed] // by default this property is null
		[Export ("viewControllers", ArgumentSemantic.Copy)]
#if !TVOS
		[PostGet ("CustomizableViewControllers")]
#endif
		[PostGet ("SelectedViewController")]
		UIViewController [] ViewControllers { get; set; }

		[Export ("setViewControllers:animated:")]
		[PostGet ("ViewControllers")] // indirectly call PostGet on CustomizableViewControllers and SelectedViewController
		void SetViewControllers (UIViewController [] viewControllers, bool animated);

		[NullAllowed] // by default this property is null
		[Export ("selectedViewController", ArgumentSemantic.Assign)]
		UIViewController SelectedViewController { get; set; }

		[Export ("selectedIndex")]
		nint SelectedIndex { get; [PostGet ("SelectedViewController")] set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("moreNavigationController")]
		UINavigationController MoreNavigationController { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("customizableViewControllers", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIViewController [] CustomizableViewControllers { get; set; }

		[Export ("tabBar")]
		UITabBar TabBar { get; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UITabBarControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITabBarDelegate {
		[Export ("tabBar:didSelectItem:"), EventArgs ("UITabBarItem")]
		void ItemSelected (UITabBar tabbar, UITabBarItem item);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBar:willBeginCustomizingItems:"), EventArgs ("UITabBarItems")]
		void WillBeginCustomizingItems (UITabBar tabbar, UITabBarItem [] items);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBar:didBeginCustomizingItems:"), EventArgs ("UITabBarItems")]
		void DidBeginCustomizingItems (UITabBar tabbar, UITabBarItem [] items);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBar:willEndCustomizingItems:changed:"), EventArgs ("UITabBarFinalItems")]
		void WillEndCustomizingItems (UITabBar tabbar, UITabBarItem [] items, bool changed);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBar:didEndCustomizingItems:changed:"), EventArgs ("UITabBarFinalItems")]
		void DidEndCustomizingItems (UITabBar tabbar, UITabBarItem [] items, bool changed);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITabBarControllerDelegate {

		[Export ("tabBarController:shouldSelectViewController:"), DefaultValue (true), DelegateName ("UITabBarSelection")]
		bool ShouldSelectViewController (UITabBarController tabBarController, UIViewController viewController);

		[Export ("tabBarController:didSelectViewController:"), EventArgs ("UITabBarSelection")]
		void ViewControllerSelected (UITabBarController tabBarController, UIViewController viewController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBarController:willBeginCustomizingViewControllers:"), EventArgs ("UITabBarCustomize")]
		void OnCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBarController:willEndCustomizingViewControllers:changed:"), EventArgs ("UITabBarCustomizeChange")]
		void OnEndCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers, bool changed);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBarController:didEndCustomizingViewControllers:changed:"), EventArgs ("UITabBarCustomizeChange")]
		void FinishedCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers, bool changed);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBarControllerSupportedInterfaceOrientations:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIInterfaceOrientationMask>")]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UITabBarController tabBarController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tabBarControllerPreferredInterfaceOrientationForPresentation:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIInterfaceOrientation>")]
		UIInterfaceOrientation GetPreferredInterfaceOrientation (UITabBarController tabBarController);

		[Export ("tabBarController:interactionControllerForAnimationController:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,IUIViewControllerAnimatedTransitioning,IUIViewControllerInteractiveTransitioning>")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UITabBarController tabBarController,
													 IUIViewControllerAnimatedTransitioning animationController);

		[Export ("tabBarController:animationControllerForTransitionFromViewController:toViewController:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIViewController,UIViewController,IUIViewControllerAnimatedTransitioning>")]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForTransition (UITabBarController tabBarController,
											   UIViewController fromViewController,
											   UIViewController toViewController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarItem))]
	[DesignatedDefaultCtor]
	interface UITabBarItem : NSCoding
#if IOS
		, UISpringLoadedInteractionSupporting, UIPopoverPresentationControllerSourceItem
#endif
	{
		[Export ("enabled")]
		[Override]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("title", ArgumentSemantic.Copy)]
		[Override]
		[NullAllowed]
		string Title { get; set; }

		[Export ("image", ArgumentSemantic.Retain)]
		[Override]
		[NullAllowed]
		UIImage Image { get; set; }

		[Export ("imageInsets")]
		[Override]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")]
		[Override]
		nint Tag { get; set; }

		[Export ("initWithTitle:image:tag:")]
		[PostGet ("Image")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] UIImage image, nint tag);

		[Export ("initWithTabBarSystemItem:tag:")]
		NativeHandle Constructor (UITabBarSystemItem systemItem, nint tag);

		[Export ("badgeValue", ArgumentSemantic.Copy)]
		[NullAllowed]
		string BadgeValue { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use the '(string, UIImage, UIImage)' constructor or the 'Image' and 'SelectedImage' properties along with 'RenderingMode = UIImageRenderingMode.AlwaysOriginal'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the '(string, UIImage, UIImage)' constructor or the 'Image' and 'SelectedImage' properties along with 'RenderingMode = UIImageRenderingMode.AlwaysOriginal'.")]
		[Export ("setFinishedSelectedImage:withFinishedUnselectedImage:")]
		[PostGet ("FinishedSelectedImage")]
		[PostGet ("FinishedUnselectedImage")]
		void SetFinishedImages ([NullAllowed] UIImage selectedImage, [NullAllowed] UIImage unselectedImage);

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("finishedSelectedImage")]
		UIImage FinishedSelectedImage { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("finishedUnselectedImage")]
		UIImage FinishedUnselectedImage { get; }

		[Export ("titlePositionAdjustment")]
		[Appearance]
		UIOffset TitlePositionAdjustment { get; set; }

		[Export ("initWithTitle:image:selectedImage:")]
		[PostGet ("Image")]
		[PostGet ("SelectedImage")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] UIImage image, [NullAllowed] UIImage selectedImage);

		[Export ("selectedImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage SelectedImage { get; set; }

		[Appearance]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("badgeColor", ArgumentSemantic.Copy)]
		UIColor BadgeColor { get; set; }

		[Appearance]
		[MacCatalyst (13, 1)]
		[Export ("setBadgeTextAttributes:forState:")]
		[Internal]
		void SetBadgeTextAttributes ([NullAllowed] NSDictionary textAttributes, UIControlState state);

		[MacCatalyst (13, 1)]
		[Wrap ("SetBadgeTextAttributes (textAttributes.GetDictionary (), state)")]
		void SetBadgeTextAttributes (UIStringAttributes textAttributes, UIControlState state);

		[Appearance]
		[MacCatalyst (13, 1)]
		[Export ("badgeTextAttributesForState:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetBadgeTextAttributesDictionary (UIControlState state);

		[MacCatalyst (13, 1)]
		[Wrap ("new UIStringAttributes (GetBadgeTextAttributesDictionary(state))")]
		UIStringAttributes GetBadgeTextAttributes (UIControlState state);

		[Appearance]
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("standardAppearance", ArgumentSemantic.Copy)]
		UITabBarAppearance StandardAppearance { get; set; }

		[Appearance]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("scrollEdgeAppearance", ArgumentSemantic.Copy)]
		UITabBarAppearance ScrollEdgeAppearance { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIScrollView))]
	interface UITableView : NSCoding, UIDataSourceTranslating
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithFrame:style:")]
		NativeHandle Constructor (CGRect frame, UITableViewStyle style);

		[Export ("style")]
		UITableViewStyle Style { get; }

		[Export ("dataSource", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		UITableViewDataSource DataSource { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[New]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[New]
		[Protocolize]
		UITableViewDelegate Delegate { get; set; }

		[Export ("rowHeight")]
		nfloat RowHeight { get; set; }

		[Export ("sectionHeaderHeight")]
		nfloat SectionHeaderHeight { get; set; }

		[Export ("sectionFooterHeight")]
		nfloat SectionFooterHeight { get; set; }

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("reloadSectionIndexTitles")]
		void ReloadSectionIndexTitles ();

		[Export ("numberOfSections")]
		nint NumberOfSections ();

		[Export ("numberOfRowsInSection:")]
		nint NumberOfRowsInSection (nint section);

		[Export ("rectForSection:")]
		CGRect RectForSection (nint section);

		[Export ("rectForHeaderInSection:")]
		CGRect RectForHeaderInSection (nint section);

		[Export ("rectForFooterInSection:")]
		CGRect RectForFooterInSection (nint section);

		[Export ("rectForRowAtIndexPath:")]
		CGRect RectForRowAtIndexPath (NSIndexPath indexPath);

		[Export ("indexPathForRowAtPoint:")]
		[return: NullAllowed]
		NSIndexPath IndexPathForRowAtPoint (CGPoint point);

		[Export ("indexPathForCell:")]
		[return: NullAllowed]
		NSIndexPath IndexPathForCell (UITableViewCell cell);

		[Export ("indexPathsForRowsInRect:")]
		[Internal]
		IntPtr _IndexPathsForRowsInRect (CGRect rect);

		[Export ("cellForRowAtIndexPath:")]
		[return: NullAllowed]
		UITableViewCell CellAt (NSIndexPath ns);

		[Export ("visibleCells")]
		UITableViewCell [] VisibleCells { get; }

		[Export ("indexPathsForVisibleRows")]
		[NullAllowed]
		NSIndexPath [] IndexPathsForVisibleRows { get; }

		[Export ("scrollToRowAtIndexPath:atScrollPosition:animated:")]
		void ScrollToRow (NSIndexPath indexPath, UITableViewScrollPosition atScrollPosition, bool animated);

		[Export ("scrollToNearestSelectedRowAtScrollPosition:animated:")]
		void ScrollToNearestSelected (UITableViewScrollPosition atScrollPosition, bool animated);

		[Export ("beginUpdates")]
		void BeginUpdates ();

		[Export ("endUpdates")]
		void EndUpdates ();

		[Export ("insertSections:withRowAnimation:")]
		void InsertSections (NSIndexSet sections, UITableViewRowAnimation withRowAnimation);

		[Export ("deleteSections:withRowAnimation:")]
		void DeleteSections (NSIndexSet sections, UITableViewRowAnimation withRowAnimation);

		[Export ("reloadSections:withRowAnimation:")]
		void ReloadSections (NSIndexSet sections, UITableViewRowAnimation withRowAnimation);

		[Export ("insertRowsAtIndexPaths:withRowAnimation:")]
		void InsertRows (NSIndexPath [] atIndexPaths, UITableViewRowAnimation withRowAnimation);

		[Export ("deleteRowsAtIndexPaths:withRowAnimation:")]
		void DeleteRows (NSIndexPath [] atIndexPaths, UITableViewRowAnimation withRowAnimation);

		[Export ("reloadRowsAtIndexPaths:withRowAnimation:")]
		void ReloadRows (NSIndexPath [] atIndexPaths, UITableViewRowAnimation withRowAnimation);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("reconfigureRowsAtIndexPaths:")]
		void ReconfigureRows (NSIndexPath [] indexPaths);

		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }

		[Export ("setEditing:animated:")]
		void SetEditing (bool editing, bool animated);

		[Export ("allowsSelection")]
		bool AllowsSelection { get; set; }

		[Export ("allowsSelectionDuringEditing")]
		bool AllowsSelectionDuringEditing { get; set; }

		[NullAllowed]
		[Export ("indexPathForSelectedRow")]
		NSIndexPath IndexPathForSelectedRow { get; }

		[Export ("selectRowAtIndexPath:animated:scrollPosition:")]
		void SelectRow ([NullAllowed] NSIndexPath indexPath, bool animated, UITableViewScrollPosition scrollPosition);

		[Export ("deselectRowAtIndexPath:animated:")]
		void DeselectRow (NSIndexPath indexPath, bool animated);

		[Export ("sectionIndexMinimumDisplayRowCount")]
		nint SectionIndexMinimumDisplayRowCount { get; set; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("separatorStyle")]
		UITableViewCellSeparatorStyle SeparatorStyle { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("separatorColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed] // nullable (and spotted by introspection on iOS9)
		UIColor SeparatorColor { get; set; }

		[Export ("tableHeaderView", ArgumentSemantic.Retain), NullAllowed]
		UIView TableHeaderView { get; set; }

		[Export ("tableFooterView", ArgumentSemantic.Retain), NullAllowed]
		UIView TableFooterView { get; set; }

		[Export ("dequeueReusableCellWithIdentifier:")]
		[return: NullAllowed]
		UITableViewCell DequeueReusableCell (string identifier);

		[Export ("dequeueReusableCellWithIdentifier:")]
		[Sealed]
		[return: NullAllowed]
		UITableViewCell DequeueReusableCell (NSString identifier);

		// 3.2
		[Export ("backgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView BackgroundView { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("UITableViewIndexSearch")]
		NSString IndexSearch { get; }

		[Field ("UITableViewAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("allowsMultipleSelectionDuringEditing")]
		bool AllowsMultipleSelectionDuringEditing { get; set; }

		[Export ("moveSection:toSection:")]
		void MoveSection (nint fromSection, nint toSection);

		[Export ("moveRowAtIndexPath:toIndexPath:")]
		void MoveRow (NSIndexPath fromIndexPath, NSIndexPath toIndexPath);

		[NullAllowed]
		[Export ("indexPathsForSelectedRows")]
		NSIndexPath [] IndexPathsForSelectedRows { get; }

		[Export ("registerNib:forCellReuseIdentifier:")]
		void RegisterNibForCellReuse ([NullAllowed] UINib nib, NSString reuseIdentifier);

		[Field ("UITableViewSelectionDidChangeNotification")]
		[Notification]
		NSString SelectionDidChangeNotification { get; }

		//
		// 6.0
		//
		[Appearance]
		[NullAllowed]
		[Export ("sectionIndexColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexColor { get; set; }

		[Appearance]
		[NullAllowed]
		[Export ("sectionIndexTrackingBackgroundColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexTrackingBackgroundColor { get; set; }

		[Export ("headerViewForSection:")]
		[return: NullAllowed]
		UITableViewHeaderFooterView GetHeaderView (nint section);

		[Export ("footerViewForSection:")]
		[return: NullAllowed]
		UITableViewHeaderFooterView GetFooterView (nint section);

		[Export ("dequeueReusableCellWithIdentifier:forIndexPath:")]
		UITableViewCell DequeueReusableCell (NSString reuseIdentifier, NSIndexPath indexPath);

		[Export ("dequeueReusableHeaderFooterViewWithIdentifier:")]
		[return: NullAllowed]
		UITableViewHeaderFooterView DequeueReusableHeaderFooterView (NSString reuseIdentifier);

		[Export ("registerClass:forCellReuseIdentifier:"), Internal]
		void RegisterClassForCellReuse (IntPtr /*Class*/ cellClass, NSString reuseIdentifier);

		[Export ("registerNib:forHeaderFooterViewReuseIdentifier:")]
		void RegisterNibForHeaderFooterViewReuse ([NullAllowed] UINib nib, NSString reuseIdentifier);

		[Export ("registerClass:forHeaderFooterViewReuseIdentifier:"), Internal]
		void RegisterClassForHeaderFooterViewReuse (IntPtr /*Class*/ aClass, NSString reuseIdentifier);

		//
		// 7.0
		//
		[Export ("estimatedRowHeight", ArgumentSemantic.Assign)]
		nfloat EstimatedRowHeight { get; set; }

		[Export ("estimatedSectionHeaderHeight", ArgumentSemantic.Assign)]
		nfloat EstimatedSectionHeaderHeight { get; set; }

		[Export ("estimatedSectionFooterHeight", ArgumentSemantic.Assign)]
		nfloat EstimatedSectionFooterHeight { get; set; }

		[Appearance]
		[NullAllowed] // by default this property is null
		[Export ("sectionIndexBackgroundColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexBackgroundColor { get; set; }

		[Appearance]
		[Export ("separatorInset")]
		UIEdgeInsets SeparatorInset { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("separatorEffect", ArgumentSemantic.Copy)]
		[Appearance]
		UIVisualEffect SeparatorEffect { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("cellLayoutMarginsFollowReadableWidth")]
		bool CellLayoutMarginsFollowReadableWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("remembersLastFocusedIndexPath")]
		bool RemembersLastFocusedIndexPath { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("prefetchDataSource", ArgumentSemantic.Weak)]
		IUITableViewDataSourcePrefetching PrefetchDataSource { get; set; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("prefetchingEnabled")]
		bool PrefetchingEnabled { [Bind ("isPrefetchingEnabled")] get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dragDelegate", ArgumentSemantic.Weak)]
		IUITableViewDragDelegate DragDelegate { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dropDelegate", ArgumentSemantic.Weak)]
		IUITableViewDropDelegate DropDelegate { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("separatorInsetReference", ArgumentSemantic.Assign)]
		UITableViewSeparatorInsetReference SeparatorInsetReference { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Async]
		[Export ("performBatchUpdates:completion:")]
		void PerformBatchUpdates ([NullAllowed] Action updates, [NullAllowed] Action<bool> completion);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("hasUncommittedUpdates")]
		bool HasUncommittedUpdates { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("dragInteractionEnabled")]
		bool DragInteractionEnabled { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasActiveDrag")]
		bool HasActiveDrag { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hasActiveDrop")]
		bool HasActiveDrop { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("insetsContentViewsToSafeArea")]
		bool InsetsContentViewsToSafeArea { get; set; }

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("selectionFollowsFocus")]
		bool SelectionFollowsFocus { get; set; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsFocus")]
		bool AllowsFocus { get; set; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("allowsFocusDuringEditing")]
		bool AllowsFocusDuringEditing { get; set; }

		[NoWatch, TV (17, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("contextMenuInteraction")]
		UIContextMenuInteraction ContextMenuInteraction { get; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("fillerRowHeight")]
		nfloat FillerRowHeight { get; set; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sectionHeaderTopPadding")]
		nfloat SectionHeaderTopPadding { get; set; }

		[Watch (9, 0), TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("selfSizingInvalidation", ArgumentSemantic.Assign)]
		UITableViewSelfSizingInvalidation SelfSizingInvalidation { get; set; }

	}

	interface IUITableViewDataSourcePrefetching { }
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITableViewDataSourcePrefetching {
		[Abstract]
		[Export ("tableView:prefetchRowsAtIndexPaths:")]
		void PrefetchRows (UITableView tableView, NSIndexPath [] indexPaths);

		[Export ("tableView:cancelPrefetchingForRowsAtIndexPaths:")]
		void CancelPrefetching (UITableView tableView, NSIndexPath [] indexPaths);
	}

	//
	// This mixed both the UITableViewDataSource and UITableViewDelegate in a single class
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (UIScrollViewDelegate))]
	[Synthetic]
	interface UITableViewSource {
		[Export ("tableView:numberOfRowsInSection:")]
		[Abstract]
#if NET
		nint RowsInSection (UITableView tableView, nint section);
#else
		nint RowsInSection (UITableView tableview, nint section);
#endif

		[Export ("tableView:cellForRowAtIndexPath:")]
		[Abstract]
		UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath);

		[Export ("numberOfSectionsInTableView:")]
		nint NumberOfSections (UITableView tableView);

		[Export ("tableView:titleForHeaderInSection:")]
		string TitleForHeader (UITableView tableView, nint section);

		[Export ("tableView:titleForFooterInSection:")]
		string TitleForFooter (UITableView tableView, nint section);

		[Export ("tableView:canEditRowAtIndexPath:")]
		bool CanEditRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:canMoveRowAtIndexPath:")]
		bool CanMoveRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("sectionIndexTitlesForTableView:")]
		string [] SectionIndexTitles (UITableView tableView);

		[MacCatalyst (13, 1)]
		[Export ("tableView:sectionForSectionIndexTitle:atIndex:")]
		nint SectionFor (UITableView tableView, string title, nint atIndex);

		[Export ("tableView:commitEditingStyle:forRowAtIndexPath:")]
		void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath);

		[Export ("tableView:moveRowAtIndexPath:toIndexPath:")]
		void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);

		[Export ("tableView:willDisplayCell:forRowAtIndexPath:")]
		void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[Export ("tableView:heightForRowAtIndexPath:")]
		nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:heightForHeaderInSection:")]
		nfloat GetHeightForHeader (UITableView tableView, nint section);

		[Export ("tableView:heightForFooterInSection:")]
		nfloat GetHeightForFooter (UITableView tableView, nint section);

		[Export ("tableView:viewForHeaderInSection:")]
		UIView GetViewForHeader (UITableView tableView, nint section);

		[Export ("tableView:viewForFooterInSection:")]
		UIView GetViewForFooter (UITableView tableView, nint section);

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 3, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("tableView:accessoryTypeForRowWithIndexPath:")]
		UITableViewCellAccessory AccessoryForRow (UITableView tableView, NSIndexPath indexPath);
#endif

		[Export ("tableView:accessoryButtonTappedForRowWithIndexPath:")]
		void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:willSelectRowAtIndexPath:")]
		NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:willDeselectRowAtIndexPath:")]
		NSIndexPath WillDeselectRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:didSelectRowAtIndexPath:")]
		void RowSelected (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:didDeselectRowAtIndexPath:")]
		void RowDeselected (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:editingStyleForRowAtIndexPath:")]
		UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:")]
		string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:shouldIndentWhileEditingRowAtIndexPath:")]
		bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:willBeginEditingRowAtIndexPath:")]
		void WillBeginEditing (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:didEndEditingRowAtIndexPath:")]
		void DidEndEditing (UITableView tableView, [NullAllowed] NSIndexPath indexPath);

		[Export ("tableView:targetIndexPathForMoveFromRowAtIndexPath:toProposedIndexPath:")]
		NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath);

		[Export ("tableView:indentationLevelForRowAtIndexPath:")]
		nint IndentationLevel (UITableView tableView, NSIndexPath indexPath);

		// Copy Paste support
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:shouldShowMenuForRowAtIndexPath:")]
		bool ShouldShowMenu (UITableView tableView, NSIndexPath rowAtindexPath);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:canPerformAction:forRowAtIndexPath:withSender:")]
		bool CanPerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, [NullAllowed] NSObject sender);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:performAction:forRowAtIndexPath:withSender:")]
		void PerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, [NullAllowed] NSObject sender);

		[Export ("tableView:willDisplayHeaderView:forSection:")]
		void WillDisplayHeaderView (UITableView tableView, UIView headerView, nint section);

		[Export ("tableView:willDisplayFooterView:forSection:")]
		void WillDisplayFooterView (UITableView tableView, UIView footerView, nint section);

		[Export ("tableView:didEndDisplayingCell:forRowAtIndexPath:")]
		void CellDisplayingEnded (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[Export ("tableView:didEndDisplayingHeaderView:forSection:")]
		void HeaderViewDisplayingEnded (UITableView tableView, UIView headerView, nint section);

		[Export ("tableView:didEndDisplayingFooterView:forSection:")]
		void FooterViewDisplayingEnded (UITableView tableView, UIView footerView, nint section);

		[Export ("tableView:shouldHighlightRowAtIndexPath:")]
		bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:didHighlightRowAtIndexPath:")]
		void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:didUnhighlightRowAtIndexPath:")]
		void RowUnhighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:estimatedHeightForRowAtIndexPath:")]
		nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:estimatedHeightForHeaderInSection:")]
		nfloat EstimatedHeightForHeader (UITableView tableView, nint section);

		[Export ("tableView:estimatedHeightForFooterInSection:")]
		nfloat EstimatedHeightForFooter (UITableView tableView, nint section);

		[NoTV]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetTrailingSwipeActionsConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetTrailingSwipeActionsConfiguration' instead.")]
		[Export ("tableView:editActionsForRowAtIndexPath:")]
		UITableViewRowAction [] EditActionsForRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("tableView:canFocusRowAtIndexPath:")]
		bool CanFocusRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context);

		[MacCatalyst (13, 1)]
		[Export ("tableView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[MacCatalyst (13, 1)]
		[Export ("indexPathForPreferredFocusedViewInTableView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UITableView tableView);

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tableView:selectionFollowsFocusForRowAtIndexPath:")]
		bool GetSelectionFollowsFocusForRow (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:leadingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:trailingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldSpringLoadRowAtIndexPath:withContext:")]
		bool ShouldSpringLoadRow (UITableView tableView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldBeginMultipleSelectionInteractionAtIndexPath:")]
		bool ShouldBeginMultipleSelectionInteraction (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:didBeginMultipleSelectionInteractionAtIndexPath:")]
		void DidBeginMultipleSelectionInteraction (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableViewDidEndMultipleSelectionInteraction:")]
		void DidEndMultipleSelectionInteraction (UITableView tableView);

		[NoWatch, TV (17, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:contextMenuConfigurationForRowAtIndexPath:point:")]
		[return: NullAllowed]
		UIContextMenuConfiguration GetContextMenuConfiguration (UITableView tableView, NSIndexPath indexPath, CGPoint point);

		[NoWatch, TV (17, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:previewForHighlightingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForHighlightingContextMenu (UITableView tableView, UIContextMenuConfiguration configuration);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:previewForDismissingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForDismissingContextMenu (UITableView tableView, UIContextMenuConfiguration configuration);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:willPerformPreviewActionForMenuWithConfiguration:animator:")]
		void WillPerformPreviewAction (UITableView tableView, UIContextMenuConfiguration configuration, IUIContextMenuInteractionCommitAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tableView:willDisplayContextMenuWithConfiguration:animator:")]
		void WillDisplayContextMenu (UITableView tableView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tableView:willEndContextMenuInteractionWithConfiguration:animator:")]
		void WillEndContextMenuInteraction (UITableView tableView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("tableView:canPerformPrimaryActionForRowAtIndexPath:")]
		bool CanPerformPrimaryAction (UITableView tableView, NSIndexPath rowIndexPath);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("tableView:performPrimaryActionForRowAtIndexPath:")]
		void PerformPrimaryAction (UITableView tableView, NSIndexPath rowIndexPath);

		// WARNING: If you add more methods here, add them to UITableViewControllerDelegate as well.
	}

	[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
	delegate void UITableViewCellConfigurationUpdateHandler (UITableViewCell cell, UICellConfigurationState state);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UITableViewCell : NSCoding, UIGestureRecognizerDelegate {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithStyle:reuseIdentifier:")]
		NativeHandle Constructor (UITableViewCellStyle style, [NullAllowed] NSString reuseIdentifier);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("configurationState")]
		UICellConfigurationState ConfigurationState { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setNeedsUpdateConfiguration")]
		void SetNeedsUpdateConfiguration ();

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("updateConfigurationUsingState:")]
		void UpdateConfiguration (UICellConfigurationState state);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("configurationUpdateHandler", ArgumentSemantic.Copy)]
		UITableViewCellConfigurationUpdateHandler ConfigurationUpdateHandler { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("defaultContentConfiguration")]
		UIListContentConfiguration DefaultContentConfiguration { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("contentConfiguration", ArgumentSemantic.Copy)]
		IUIContentConfiguration ContentConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesContentConfiguration")]
		bool AutomaticallyUpdatesContentConfiguration { get; set; }

		[Export ("contentView", ArgumentSemantic.Retain)]
		UIView ContentView { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Export ("imageView", ArgumentSemantic.Retain)]
		UIImageView ImageView { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Export ("textLabel", ArgumentSemantic.Retain)]
		UILabel TextLabel { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'UIListContentConfiguration' instead.")]
		[Export ("detailTextLabel", ArgumentSemantic.Retain)]
		UILabel DetailTextLabel { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("backgroundConfiguration", ArgumentSemantic.Copy)]
		UIBackgroundConfiguration BackgroundConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesBackgroundConfiguration")]
		bool AutomaticallyUpdatesBackgroundConfiguration { get; set; }

		[Export ("backgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView BackgroundView { get; set; }

		[Export ("selectedBackgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView SelectedBackgroundView { get; set; }

		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
		NSString ReuseIdentifier { get; }

		[RequiresSuper]
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Export ("selectionStyle")]
		UITableViewCellSelectionStyle SelectionStyle { get; set; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("setSelected:animated:")]
		void SetSelected (bool selected, bool animated);

		[Export ("setHighlighted:animated:")]
		void SetHighlighted (bool highlighted, bool animated);

		[Export ("editingStyle")]
		UITableViewCellEditingStyle EditingStyle { get; }

		[Export ("showsReorderControl")]
		bool ShowsReorderControl { get; set; }

		[Export ("shouldIndentWhileEditing")]
		bool ShouldIndentWhileEditing { get; set; }

		[Export ("accessoryType")]
		UITableViewCellAccessory Accessory { get; set; }

		[Export ("accessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView AccessoryView { get; set; }

		[Export ("editingAccessoryType")]
		UITableViewCellAccessory EditingAccessory { get; set; }

		[Export ("editingAccessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView EditingAccessoryView { get; set; }

		[Export ("indentationLevel")]
		nint IndentationLevel { get; set; }

		[Export ("indentationWidth")]
		nfloat IndentationWidth { get; set; }

		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }

		[Export ("setEditing:animated:")]
		void SetEditing (bool editing, bool animated);

		[Export ("showingDeleteConfirmation")]
		bool ShowingDeleteConfirmation { get; [NotImplemented] set; }

		[Export ("willTransitionToState:")]
		void WillTransitionToState (UITableViewCellState mask);

		[Export ("didTransitionToState:")]
		void DidTransitionToState (UITableViewCellState mask);

		[Export ("multipleSelectionBackgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView MultipleSelectionBackgroundView { get; set; }

		[Appearance]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("separatorInset")]
		UIEdgeInsets SeparatorInset { get; set; }

		[Appearance]
		[MacCatalyst (13, 1)]
		[Export ("focusStyle", ArgumentSemantic.Assign)]
		UITableViewCellFocusStyle FocusStyle { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("dragStateDidChange:")]
		void DragStateDidChange (UITableViewCellDragState dragState);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("userInteractionEnabledWhileDragging")]
		bool UserInteractionEnabledWhileDragging { get; set; }

		[Watch (9, 0), TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("defaultBackgroundConfiguration")]
		UIBackgroundConfiguration DefaultBackgroundConfiguration { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UITableViewController : UITableViewDataSource, UITableViewDelegate {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Export ("initWithStyle:")]
		NativeHandle Constructor (UITableViewStyle withStyle);

		[Export ("tableView", ArgumentSemantic.Retain)]
		UITableView TableView { get; set; }

		[Export ("clearsSelectionOnViewWillAppear")]
		bool ClearsSelectionOnViewWillAppear { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("refreshControl", ArgumentSemantic.Strong)]
		UIRefreshControl RefreshControl { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
#if NET
	[Protocol, Model]
#else
	[Model (AutoGeneratedName = true)]
	[Protocol]
#endif
	interface UITableViewDataSource {

		[Export ("tableView:numberOfRowsInSection:")]
		[Abstract]
		nint RowsInSection (UITableView tableView, nint section);

		[Export ("tableView:cellForRowAtIndexPath:")]
		[Abstract]
		UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath);

		[Export ("numberOfSectionsInTableView:")]
		nint NumberOfSections (UITableView tableView);

		[Export ("tableView:titleForHeaderInSection:")]
		string TitleForHeader (UITableView tableView, nint section);

		[Export ("tableView:titleForFooterInSection:")]
		string TitleForFooter (UITableView tableView, nint section);

		[Export ("tableView:canEditRowAtIndexPath:")]
		bool CanEditRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:canMoveRowAtIndexPath:")]
		bool CanMoveRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("sectionIndexTitlesForTableView:")]
		string [] SectionIndexTitles (UITableView tableView);

		[MacCatalyst (13, 1)]
		[Export ("tableView:sectionForSectionIndexTitle:atIndex:")]
		nint SectionFor (UITableView tableView, string title, nint atIndex);

		[Export ("tableView:commitEditingStyle:forRowAtIndexPath:")]
		void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath);

		[Export ("tableView:moveRowAtIndexPath:toIndexPath:")]
		void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIScrollViewDelegate))]
	[Model]
	[Protocol]
	interface UITableViewDelegate {

		[Export ("tableView:willDisplayCell:forRowAtIndexPath:")]
		void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[Export ("tableView:heightForRowAtIndexPath:")]
		nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:heightForHeaderInSection:")]
		nfloat GetHeightForHeader (UITableView tableView, nint section);

		[Export ("tableView:heightForFooterInSection:")]
		nfloat GetHeightForFooter (UITableView tableView, nint section);

		[Export ("tableView:viewForHeaderInSection:")]
		UIView GetViewForHeader (UITableView tableView, nint section);

		[Export ("tableView:viewForFooterInSection:")]
		UIView GetViewForFooter (UITableView tableView, nint section);

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 3, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("tableView:accessoryTypeForRowWithIndexPath:")]
		UITableViewCellAccessory AccessoryForRow (UITableView tableView, NSIndexPath indexPath);
#endif

		[Export ("tableView:accessoryButtonTappedForRowWithIndexPath:")]
		void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:willSelectRowAtIndexPath:")]
		NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:willDeselectRowAtIndexPath:")]
		NSIndexPath WillDeselectRow (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:didSelectRowAtIndexPath:")]
		void RowSelected (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:didDeselectRowAtIndexPath:")]
		void RowDeselected (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:editingStyleForRowAtIndexPath:")]
		UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:")]
		string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:shouldIndentWhileEditingRowAtIndexPath:")]
		bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:willBeginEditingRowAtIndexPath:")]
		void WillBeginEditing (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:didEndEditingRowAtIndexPath:")]
		void DidEndEditing (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:targetIndexPathForMoveFromRowAtIndexPath:toProposedIndexPath:")]
		NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath);

		[Export ("tableView:indentationLevelForRowAtIndexPath:")]
		nint IndentationLevel (UITableView tableView, NSIndexPath indexPath);

		// Copy Paste support
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:shouldShowMenuForRowAtIndexPath:")]
		bool ShouldShowMenu (UITableView tableView, NSIndexPath rowAtindexPath);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:canPerformAction:forRowAtIndexPath:withSender:")]
		bool CanPerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetContextMenuConfiguration' instead.")]
		[Export ("tableView:performAction:forRowAtIndexPath:withSender:")]
		void PerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Export ("tableView:willDisplayHeaderView:forSection:")]
		void WillDisplayHeaderView (UITableView tableView, UIView headerView, nint section);

		[Export ("tableView:willDisplayFooterView:forSection:")]
		void WillDisplayFooterView (UITableView tableView, UIView footerView, nint section);

		[Export ("tableView:didEndDisplayingCell:forRowAtIndexPath:")]
		void CellDisplayingEnded (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[Export ("tableView:didEndDisplayingHeaderView:forSection:")]
		void HeaderViewDisplayingEnded (UITableView tableView, UIView headerView, nint section);

		[Export ("tableView:didEndDisplayingFooterView:forSection:")]
		void FooterViewDisplayingEnded (UITableView tableView, UIView footerView, nint section);

		[Export ("tableView:shouldHighlightRowAtIndexPath:")]
		bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:didHighlightRowAtIndexPath:")]
		void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:didUnhighlightRowAtIndexPath:")]
		void RowUnhighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[Export ("tableView:estimatedHeightForRowAtIndexPath:")]
		nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:estimatedHeightForHeaderInSection:")]
		nfloat EstimatedHeightForHeader (UITableView tableView, nint section);

		[Export ("tableView:estimatedHeightForFooterInSection:")]
		nfloat EstimatedHeightForFooter (UITableView tableView, nint section);

		[NoTV]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetTrailingSwipeActionsConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetTrailingSwipeActionsConfiguration' instead.")]
		[Export ("tableView:editActionsForRowAtIndexPath:")]
		UITableViewRowAction [] EditActionsForRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("tableView:canFocusRowAtIndexPath:")]
		bool CanFocusRow (UITableView tableView, NSIndexPath indexPath);

		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context);

		[MacCatalyst (13, 1)]
		[Export ("tableView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[MacCatalyst (13, 1)]
		[Export ("indexPathForPreferredFocusedViewInTableView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UITableView tableView);

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("tableView:selectionFollowsFocusForRowAtIndexPath:")]
		bool GetSelectionFollowsFocusForRow (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:leadingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:trailingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldSpringLoadRowAtIndexPath:withContext:")]
		bool ShouldSpringLoadRow (UITableView tableView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:shouldBeginMultipleSelectionInteractionAtIndexPath:")]
		bool ShouldBeginMultipleSelectionInteraction (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:didBeginMultipleSelectionInteractionAtIndexPath:")]
		void DidBeginMultipleSelectionInteraction (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableViewDidEndMultipleSelectionInteraction:")]
		void DidEndMultipleSelectionInteraction (UITableView tableView);

		[NoWatch, TV (17, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:contextMenuConfigurationForRowAtIndexPath:point:")]
		[return: NullAllowed]
		UIContextMenuConfiguration GetContextMenuConfiguration (UITableView tableView, NSIndexPath indexPath, CGPoint point);

		[NoWatch, TV (17, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:previewForHighlightingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForHighlightingContextMenu (UITableView tableView, UIContextMenuConfiguration configuration);

		[NoWatch, TV (17, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:previewForDismissingContextMenuWithConfiguration:")]
		[return: NullAllowed]
		UITargetedPreview GetPreviewForDismissingContextMenu (UITableView tableView, UIContextMenuConfiguration configuration);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("tableView:willPerformPreviewActionForMenuWithConfiguration:animator:")]
		void WillPerformPreviewAction (UITableView tableView, UIContextMenuConfiguration configuration, IUIContextMenuInteractionCommitAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tableView:willDisplayContextMenuWithConfiguration:animator:")]
		void WillDisplayContextMenu (UITableView tableView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[NoWatch, TV (17, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("tableView:willEndContextMenuInteractionWithConfiguration:animator:")]
		void WillEndContextMenuInteraction (UITableView tableView, UIContextMenuConfiguration configuration, [NullAllowed] IUIContextMenuInteractionAnimating animator);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("tableView:canPerformPrimaryActionForRowAtIndexPath:")]
		bool CanPerformPrimaryAction (UITableView tableView, NSIndexPath rowIndexPath);

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("tableView:performPrimaryActionForRowAtIndexPath:")]
		void PerformPrimaryAction (UITableView tableView, NSIndexPath rowIndexPath);
	}

	[TV (15, 0), Watch (8, 0), iOS (15, 0), MacCatalyst (15, 0), NoWatch]
	delegate void UITableViewHeaderFooterViewConfigurationUpdateHandler (UITableViewHeaderFooterView headerFooterView, UIViewConfigurationState state);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UITableViewHeaderFooterView : UIAppearance, NSCoding {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("configurationState")]
		UIViewConfigurationState ConfigurationState { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setNeedsUpdateConfiguration")]
		void SetNeedsUpdateConfiguration ();

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("updateConfigurationUsingState:")]
		void UpdateConfiguration (UIViewConfigurationState state);

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("configurationUpdateHandler", ArgumentSemantic.Copy)]
		UITableViewHeaderFooterViewConfigurationUpdateHandler ConfigurationUpdateHandler { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("defaultContentConfiguration")]
		UIListContentConfiguration DefaultContentConfiguration { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("contentConfiguration", ArgumentSemantic.Copy)]
		IUIContentConfiguration ContentConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesContentConfiguration")]
		bool AutomaticallyUpdatesContentConfiguration { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.TvOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("textLabel", ArgumentSemantic.Retain)]
		UILabel TextLabel { get; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.TvOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("detailTextLabel", ArgumentSemantic.Retain)]
		UILabel DetailTextLabel { get; }

		[Export ("contentView", ArgumentSemantic.Retain)]
		UIView ContentView { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("backgroundConfiguration", ArgumentSemantic.Copy)]
		UIBackgroundConfiguration BackgroundConfiguration { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("automaticallyUpdatesBackgroundConfiguration")]
		bool AutomaticallyUpdatesBackgroundConfiguration { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("backgroundView", ArgumentSemantic.Retain)]
		UIView BackgroundView { get; set; }

		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
		NSString ReuseIdentifier { get; }

		[DesignatedInitializer]
		[Export ("initWithReuseIdentifier:")]
		NativeHandle Constructor (NSString reuseIdentifier);

		[RequiresSuper]
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Watch (9, 0), TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("defaultBackgroundConfiguration")]
		UIBackgroundConfiguration DefaultBackgroundConfiguration { get; }

	}

	[NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIContextualAction' and corresponding APIs instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIContextualAction' and corresponding APIs instead.")]
	interface UITableViewRowAction : NSCopying {
		[Export ("style")]
		UITableViewRowActionStyle Style { get; }

		[NullAllowed] // by default this property is null
		[Export ("title")]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("backgroundEffect", ArgumentSemantic.Copy)]
		UIVisualEffect BackgroundEffect { get; set; }

		[Static, Export ("rowActionWithStyle:title:handler:")]
		UITableViewRowAction Create (UITableViewRowActionStyle style, [NullAllowed] string title, Action<UITableViewRowAction, NSIndexPath> handler);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl), Delegates = new string [] { "WeakDelegate" })]
	// , Events=new Type [] {typeof(UITextFieldDelegate)})] custom logic needed, see https://bugzilla.xamarin.com/show_bug.cgi?id=53174
	interface UITextField : UITextInput, UIContentSizeCategoryAdjusting, UILetterformAwareAdjusting
#if IOS
	, UITextDraggable, UITextDroppable, UITextPasteConfigurationSupporting 
#endif // IOS
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Text { get; set; }

		[Export ("textColor", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIColor TextColor { get; set; }

		[Export ("font", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIFont Font { get; set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("borderStyle")]
		UITextBorderStyle BorderStyle { get; set; }

		[Export ("placeholder", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Placeholder { get; set; }

		[Export ("clearsOnBeginEditing")]
		bool ClearsOnBeginEditing { get; set; }

		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; set; }

		[Export ("minimumFontSize")]
		nfloat MinimumFontSize { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UITextFieldDelegate Delegate { get; set; }

		[Export ("background", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage Background { get; set; }

		[Export ("disabledBackground", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage DisabledBackground { get; set; }

		[Export ("isEditing")]
		bool IsEditing { get; }

		[Export ("clearButtonMode")]
		UITextFieldViewMode ClearButtonMode { get; set; }

		[Export ("leftView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView LeftView { get; set; }

		[Export ("rightView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView RightView { get; set; }

		[Export ("leftViewMode")]
		UITextFieldViewMode LeftViewMode { get; set; }

		[Export ("rightViewMode")]
		UITextFieldViewMode RightViewMode { get; set; }

		[Export ("borderRectForBounds:")]
		CGRect BorderRect (CGRect forBounds);

		[Export ("textRectForBounds:")]
		CGRect TextRect (CGRect forBounds);

		[Export ("placeholderRectForBounds:")]
		CGRect PlaceholderRect (CGRect forBounds);

		[Export ("editingRectForBounds:")]
		CGRect EditingRect (CGRect forBounds);

		[Export ("clearButtonRectForBounds:")]
		CGRect ClearButtonRect (CGRect forBounds);

		[Export ("leftViewRectForBounds:")]
		CGRect LeftViewRect (CGRect forBounds);

		[Export ("rightViewRectForBounds:")]
		CGRect RightViewRect (CGRect forBounds);

		[Deprecated (PlatformName.iOS, 15, 0, message: "No longer used.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "No longer used.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "No longer used.")]
		[Export ("drawTextInRect:")]
		void DrawText (CGRect rect);

		[Export ("drawPlaceholderInRect:")]
		void DrawPlaceholder (CGRect rect);

		// 3.2
		[Export ("inputAccessoryView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView InputAccessoryView { get; set; }

		[Export ("inputView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView InputView { get; set; }

		[Field ("UITextFieldTextDidBeginEditingNotification")]
		[Notification]
		NSString TextDidBeginEditingNotification { get; }

		[Field ("UITextFieldTextDidEndEditingNotification")]
		[Notification]
		NSString TextDidEndEditingNotification { get; }

		[Field ("UITextFieldTextDidChangeNotification")]
		[Notification]
		NSString TextFieldTextDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("UITextFieldDidEndEditingReasonKey")]
		NSString DidEndEditingReasonKey { get; }

		//
		// 6.0
		//

		[NullAllowed] // by default this property is null (on 6.0, not later)
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("attributedPlaceholder", ArgumentSemantic.Copy)]
		NSAttributedString AttributedPlaceholder { get; set; }

		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set; }

		[Export ("clearsOnInsertion")]
		bool ClearsOnInsertion { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("typingAttributes", ArgumentSemantic.Copy)]
		NSDictionary TypingAttributes { get; set; }

		[Export ("defaultTextAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakDefaultTextAttributes { get; set; }

		// Category UITextField (UIInteractionStateRestorable)

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("interactionState", ArgumentSemantic.Copy)]
		NSObject InteractionState { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITextFieldDelegate {

		[Export ("textFieldShouldBeginEditing:"), DelegateName ("UITextFieldCondition"), DefaultValue (true)]
		bool ShouldBeginEditing (UITextField textField);

		[Export ("textFieldDidBeginEditing:"), EventArgs ("UITextField"), EventName ("Started")]
		void EditingStarted (UITextField textField);

		[Export ("textFieldShouldEndEditing:"), DelegateName ("UITextFieldCondition"), DefaultValue (true)]
		bool ShouldEndEditing (UITextField textField);

		[Export ("textFieldDidEndEditing:"), EventArgs ("UITextField"), EventName ("Ended")]
		void EditingEnded (UITextField textField);

		[MacCatalyst (13, 1)]
		[Export ("textFieldDidEndEditing:reason:"), EventArgs ("UITextFieldEditingEnded"), EventName ("EndedWithReason")]
		void EditingEnded (UITextField textField, UITextFieldDidEndEditingReason reason);

		[Export ("textFieldShouldClear:"), DelegateName ("UITextFieldCondition"), DefaultValue ("true")]
		bool ShouldClear (UITextField textField);

		[Export ("textFieldShouldReturn:"), DelegateName ("UITextFieldCondition"), DefaultValue ("true")]
		bool ShouldReturn (UITextField textField);

		[Export ("textField:shouldChangeCharactersInRange:replacementString:"), DelegateName ("UITextFieldChange"), DefaultValue ("true")]
		bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString);

		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("textFieldDidChangeSelection:")]
		void DidChangeSelection (UITextField textField);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textField:willPresentEditMenuWithAnimator:")]
		void WillPresentEditMenu (UITextField textField, IUIEditMenuInteractionAnimating animator);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textField:willDismissEditMenuWithAnimator:")]
		void WillDismissEditMenu (UITextField textField, IUIEditMenuInteractionAnimating animator);

		[IgnoredInDelegate]
		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textField:editMenuForCharactersInRange:suggestedActions:")]
		[return: NullAllowed]
		UIMenu GetEditMenu (UITextField textField, NSRange range, UIMenuElement [] suggestedActions);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIScrollView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UITextViewDelegate) })]
	interface UITextView : UITextInput, NSCoding, UIContentSizeCategoryAdjusting, UILetterformAwareAdjusting
#if IOS
	, UITextDraggable, UITextDroppable, UITextPasteConfigurationSupporting 
#endif // IOS
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)]
		[NullAllowed]
		string Text { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textColor", ArgumentSemantic.Retain)]
		UIColor TextColor { get; set; }

		[Export ("editable")]
		[NoTV]
		[MacCatalyst (13, 1)]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("selectedRange")]
		NSRange SelectedRange { get; set; }

		[Export ("scrollRangeToVisible:")]
		void ScrollRangeToVisible (NSRange range);

		[Wrap ("WeakDelegate")]
		[New]
		[Protocolize]
		UITextViewDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[New]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("dataDetectorTypes")]
		[NoTV]
		[MacCatalyst (13, 1)]
		UIDataDetectorType DataDetectorTypes { get; set; }

		// 3.2

		[NullAllowed] // by default this property is null
		[Export ("inputAccessoryView", ArgumentSemantic.Retain)]
		UIView InputAccessoryView { get; set; }


		[Export ("inputView", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView InputView { get; set; }

		[Field ("UITextViewTextDidBeginEditingNotification")]
		[Notification]
		NSString TextDidBeginEditingNotification { get; }

		[Field ("UITextViewTextDidChangeNotification")]
		[Notification]
		NSString TextDidChangeNotification { get; }

		[Field ("UITextViewTextDidEndEditingNotification")]
		[Notification]
		NSString TextDidEndEditingNotification { get; }

		//
		// 6.0
		//

		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set; }

		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }

		[Export ("clearsOnInsertion")]
		bool ClearsOnInsertion { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("typingAttributes", ArgumentSemantic.Copy)]
		NSDictionary TypingAttributes {
			// this avoids a crash (see unit tests) and behave like UITextField does (return null)
			[PreSnippet ("if (SelectedRange.Length == 0) return null;", Optimizable = true)]
			get;
			set;
		}

		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")] get; set; }

		[DesignatedInitializer]
		[Export ("initWithFrame:textContainer:")]
		[PostGet ("TextContainer")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] NSTextContainer textContainer);

		[Export ("textContainer", ArgumentSemantic.Copy)]
		NSTextContainer TextContainer { get; }

		[Export ("textContainerInset", ArgumentSemantic.Assign)]
		UIEdgeInsets TextContainerInset { get; set; }

		[Export ("layoutManager", ArgumentSemantic.Copy)]
		NSLayoutManager LayoutManager { get; }

		[Export ("textStorage", ArgumentSemantic.Retain)]
		NSTextStorage TextStorage { get; }

		[Export ("linkTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakLinkTextAttributes { get; set; }

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("usesStandardTextScaling")]
		bool UsesStandardTextScaling { get; set; }

		// Category UITextView (UIInteractionStateRestorable)

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("interactionState", ArgumentSemantic.Copy)]
		NSObject InteractionState { get; set; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("textLayoutManager")]
		NSTextLayoutManager TextLayoutManager { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("findInteraction")]
		[NullAllowed]
		UIFindInteraction FindInteraction { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("findInteractionEnabled")]
		bool FindInteractionEnabled { [Bind ("isFindInteractionEnabled")] get; set; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("textViewUsingTextLayoutManager:")]
		UITextView GetTextView (bool usingTextLayoutManager);

		[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("borderStyle", ArgumentSemantic.Assign)]
		UITextViewBorderStyle BorderStyle { get; set; }
	}

	[BaseType (typeof (UIScrollViewDelegate))]
	[NoMac, NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	interface UITextViewDelegate {

		[Export ("textViewShouldBeginEditing:"), DelegateName ("UITextViewCondition"), DefaultValue ("true")]
		bool ShouldBeginEditing (UITextView textView);

		[Export ("textViewShouldEndEditing:"), DelegateName ("UITextViewCondition"), DefaultValue ("true")]
		bool ShouldEndEditing (UITextView textView);

		[Export ("textViewDidBeginEditing:"), EventArgs ("UITextView"), EventName ("Started")]
		void EditingStarted (UITextView textView);

		[Export ("textViewDidEndEditing:"), EventArgs ("UITextView"), EventName ("Ended")]
		void EditingEnded (UITextView textView);

		[Export ("textView:shouldChangeTextInRange:replacementText:"), DelegateName ("UITextViewChange"), DefaultValue ("true")]
		bool ShouldChangeText (UITextView textView, NSRange range, string text);

		[Export ("textViewDidChange:"), EventArgs ("UITextView")]
		void Changed (UITextView textView);

		[Export ("textViewDidChangeSelection:"), EventArgs ("UITextView")]
		void SelectionChanged (UITextView textView);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use the 'ShouldInteractWithUrl' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use the 'ShouldInteractWithUrl' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'ShouldInteractWithUrl' overload that takes 'UITextItemInteraction' instead.")]
		[Export ("textView:shouldInteractWithURL:inRange:"), DelegateName ("Func<UITextView,NSUrl,NSRange,bool>"), DefaultValue ("true")]
		bool ShouldInteractWithUrl (UITextView textView, NSUrl URL, NSRange characterRange);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Export ("textView:shouldInteractWithTextAttachment:inRange:"), DelegateName ("Func<UITextView,NSTextAttachment,NSRange,bool>"), DefaultValue ("true")]
		bool ShouldInteractWithTextAttachment (UITextView textView, NSTextAttachment textAttachment, NSRange characterRange);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("textView:shouldInteractWithURL:inRange:interaction:"), DelegateApiName ("AllowUrlInteraction"), DelegateName ("UITextViewDelegateShouldInteractUrlDelegate"), DefaultValue ("true")]
		bool ShouldInteractWithUrl (UITextView textView, NSUrl url, NSRange characterRange, UITextItemInteraction interaction);

		[Deprecated (PlatformName.iOS, 17, 0, message: "Replaced by 'GetPrimaryAction' and 'GetMenuConfiguration'.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Replaced by 'GetPrimaryAction' and 'GetMenuConfiguration'.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Replaced by 'GetPrimaryAction' and 'GetMenuConfiguration'.")]
		[MacCatalyst (13, 1)]
		[Export ("textView:shouldInteractWithTextAttachment:inRange:interaction:"), DelegateApiName ("AllowTextAttachmentInteraction"), DelegateName ("UITextViewDelegateShouldInteractTextDelegate"), DefaultValue ("true")]
		bool ShouldInteractWithTextAttachment (UITextView textView, NSTextAttachment textAttachment, NSRange characterRange, UITextItemInteraction interaction);

		[IgnoredInDelegate]
		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textView:editMenuForTextInRange:suggestedActions:")]
		[return: NullAllowed]
		UIMenu GetEditMenuForText (UITextView textView, NSRange range, UIMenuElement [] suggestedActions);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textView:willPresentEditMenuWithAnimator:")]
		void WillPresentEditMenu (UITextView textView, IUIEditMenuInteractionAnimating animator);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("textView:willDismissEditMenuWithAnimator:")]
		void WillDismissEditMenu (UITextView textView, IUIEditMenuInteractionAnimating aniamtor);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textView:primaryActionForTextItem:defaultAction:")]
		[return: NullAllowed]
		UIAction GetPrimaryAction (UITextView textView, UITextItem textItem, UIAction defaultAction);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textView:menuConfigurationForTextItem:defaultMenu:")]
		[return: NullAllowed]
		UITextItemMenuConfiguration GetMenuConfiguration (UITextView textView, UITextItem textItem, UIMenu defaultMenu);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textView:textItemMenuWillDisplayForTextItem:animator:")]
		void WillDisplay (UITextView textView, UITextItem textItem, IUIContextMenuInteractionAnimating animator);

		[IgnoredInDelegate]
		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("textView:textItemMenuWillEndForTextItem:animator:")]
		void WillEnd (UITextView textView, UITextItem textItem, IUIContextMenuInteractionAnimating animator);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIToolbar : UIBarPositioning {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Appearance]
		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[Export ("items", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIBarButtonItem [] Items { get; set; }

		[Appearance]
		[Export ("translucent", ArgumentSemantic.Assign)]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		// done manually so we can keep this "in sync" with 'Items' property
		//[Export ("setItems:animated:")][PostGet ("Items")]
		//void SetItems (UIBarButtonItem [] items, bool animated);

		[Export ("setBackgroundImage:forToolbarPosition:barMetrics:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIToolbarPosition position, UIBarMetrics barMetrics);

		[Export ("backgroundImageForToolbarPosition:barMetrics:")]
		[Appearance]
		UIImage GetBackgroundImage (UIToolbarPosition position, UIBarMetrics barMetrics);

		[Appearance]
		[Export ("setShadowImage:forToolbarPosition:")]
		void SetShadowImage ([NullAllowed] UIImage shadowImage, UIToolbarPosition topOrBottom);

		[Appearance]
		[Export ("shadowImageForToolbarPosition:")]
		UIImage GetShadowImage (UIToolbarPosition topOrBottom);

		[Appearance]
		[NullAllowed]
		[Export ("barTintColor", ArgumentSemantic.Retain)]
		UIColor BarTintColor { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Appearance]
		[Export ("standardAppearance", ArgumentSemantic.Copy)]
		UIToolbarAppearance StandardAppearance { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Appearance]
		[NullAllowed, Export ("compactAppearance", ArgumentSemantic.Copy)]
		UIToolbarAppearance CompactAppearance { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Appearance]
		[NullAllowed, Export ("scrollEdgeAppearance", ArgumentSemantic.Copy)]
		UIToolbarAppearance ScrollEdgeAppearance { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Appearance]
		[NullAllowed, Export ("compactScrollEdgeAppearance", ArgumentSemantic.Copy)]
		UIToolbarAppearance CompactScrollEdgeAppearance { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIToolbarDelegate Delegate { get; set; }
	}

	interface IUITimingCurveProvider { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITimingCurveProvider : NSCoding, NSCopying {
		[Abstract]
		[Export ("timingCurveType")]
		UITimingCurveType TimingCurveType { get; }

		[Abstract]
		[NullAllowed, Export ("cubicTimingParameters")]
		UICubicTimingParameters CubicTimingParameters { get; }

		[Abstract]
		[NullAllowed, Export ("springTimingParameters")]
		UISpringTimingParameters SpringTimingParameters { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarPositioningDelegate))]
	[Model]
	[Protocol]
	interface UIToolbarDelegate {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITouch {
		[Export ("locationInView:")]
		CGPoint LocationInView ([NullAllowed] UIView view);

		[Export ("previousLocationInView:")]
		CGPoint PreviousLocationInView ([NullAllowed] UIView view);

		[Export ("view", ArgumentSemantic.Retain)]
		UIView View { get; }

		[Export ("window", ArgumentSemantic.Retain)]
		[Transient]
		UIWindow Window { get; }

		[Export ("tapCount")]
		nint TapCount { get; }

		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("phase")]
		UITouchPhase Phase { get; }

		// 3.2
		[Export ("gestureRecognizers", ArgumentSemantic.Copy)]
		UIGestureRecognizer [] GestureRecognizers { get; }

		[MacCatalyst (13, 1)]
		[Export ("majorRadius")]
		nfloat MajorRadius { get; }

		[MacCatalyst (13, 1)]
		[Export ("majorRadiusTolerance")]
		nfloat MajorRadiusTolerance { get; }

		[MacCatalyst (13, 1)]
		[Export ("force")]
		nfloat Force { get; }

		[MacCatalyst (13, 1)]
		[Export ("maximumPossibleForce")]
		nfloat MaximumPossibleForce { get; }

		[MacCatalyst (13, 1)]
		[Export ("type")]
		UITouchType Type { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("preciseLocationInView:")]
		CGPoint GetPreciseLocation ([NullAllowed] UIView view);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("precisePreviousLocationInView:")]
		CGPoint GetPrecisePreviousLocation ([NullAllowed] UIView view);

		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[Export ("azimuthAngleInView:")]
		nfloat GetAzimuthAngle ([NullAllowed] UIView view);

		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[Export ("azimuthUnitVectorInView:")]
		CGVector GetAzimuthUnitVector ([NullAllowed] UIView view);

		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[Export ("altitudeAngle")]
		nfloat AltitudeAngle { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("estimationUpdateIndex")]
		NSNumber EstimationUpdateIndex { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[Export ("estimatedProperties")]
		UITouchProperties EstimatedProperties { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[MacCatalyst (13, 1)]
		[Export ("estimatedPropertiesExpectingUpdates")]
		UITouchProperties EstimatedPropertiesExpectingUpdates { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UINavigationController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIVideoEditorControllerDelegate) })]
	interface UIVideoEditorController {
		[Export ("canEditVideoAtPath:")]
		[Static]
		bool CanEditVideoAtPath (string path);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		// id<UINavigationControllerDelegate, UIVideoEditorControllerDelegate>
		UIVideoEditorControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("videoPath", ArgumentSemantic.Copy)]
		string VideoPath { get; set; }

		[Export ("videoMaximumDuration")]
		double VideoMaximumDuration { get; set; }

		[Export ("videoQuality")]
		UIImagePickerControllerQualityType VideoQuality { get; set; }
	}

	// id<UINavigationControllerDelegate, UIVideoEditorControllerDelegate>
	[BaseType (typeof (UINavigationControllerDelegate))]
	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Model]
	[Protocol]
	interface UIVideoEditorControllerDelegate {
		[Export ("videoEditorController:didSaveEditedVideoToPath:"), EventArgs ("UIPath"), EventName ("Saved")]
		void VideoSaved (UIVideoEditorController editor, [EventName ("path")] string editedVideoPath);

		[Export ("videoEditorController:didFailWithError:"), EventArgs ("NSError", true)]
		void Failed (UIVideoEditorController editor, NSError error);

		[Export ("videoEditorControllerDidCancel:")]
		void UserCancelled (UIVideoEditorController editor);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIResponder))]
	interface UIView : UIAppearance, UIAppearanceContainer, UIAccessibility, UIDynamicItem, NSCoding, UIAccessibilityIdentification, UITraitEnvironment, UICoordinateSpace, UIFocusItem, UIFocusItemContainer
#if !TVOS
		, UILargeContentViewerItem, UIPopoverPresentationControllerSourceItem
#endif
#if !WATCH
		, CALayerDelegate
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("addSubview:")]
		[PostGet ("Subviews")]
		void AddSubview (UIView view);

		[ThreadSafe, Export ("drawRect:")]
		void Draw (CGRect rect);

		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor BackgroundColor { get; set; }

		[ThreadSafe]
		[Export ("bounds")]
		new CGRect Bounds { get; set; }

		[Export ("userInteractionEnabled")]
		bool UserInteractionEnabled { [Bind ("isUserInteractionEnabled")] get; set; }

		[Export ("tag")]
		nint Tag { get; set; }

		[ThreadSafe]
		[Export ("layer", ArgumentSemantic.Retain)]
		CALayer Layer { get; }

		[Export ("frame")]
		new CGRect Frame { get; set; }

		[Export ("center")]
		new CGPoint Center { get; set; }

		[Export ("transform")]
		new CGAffineTransform Transform { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("multipleTouchEnabled")]
		bool MultipleTouchEnabled { [Bind ("isMultipleTouchEnabled")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("exclusiveTouch")]
		bool ExclusiveTouch { [Bind ("isExclusiveTouch")] get; set; }

		[Export ("hitTest:withEvent:")]
		[return: NullAllowed]
		UIView HitTest (CGPoint point, [NullAllowed] UIEvent uievent);

		[Export ("pointInside:withEvent:")]
		bool PointInside (CGPoint point, [NullAllowed] UIEvent uievent);

		[Export ("convertPoint:toView:")]
		CGPoint ConvertPointToView (CGPoint point, [NullAllowed] UIView toView);

		[Export ("convertPoint:fromView:")]
		CGPoint ConvertPointFromView (CGPoint point, [NullAllowed] UIView fromView);

		[Export ("convertRect:toView:")]
		CGRect ConvertRectToView (CGRect rect, [NullAllowed] UIView toView);

		[Export ("convertRect:fromView:")]
		CGRect ConvertRectFromView (CGRect rect, [NullAllowed] UIView fromView);

		[Export ("autoresizesSubviews")]
		bool AutosizesSubviews { get; set; }

		[Export ("autoresizingMask")]
		UIViewAutoresizing AutoresizingMask { get; set; }

		[Export ("sizeThatFits:")]
		CGSize SizeThatFits (CGSize size);

		[Export ("sizeToFit")]
		void SizeToFit ();

		[Export ("superview")]
		UIView Superview { get; }

		[Export ("subviews", ArgumentSemantic.Copy)]
		UIView [] Subviews { get; }

		[Export ("window")]
		[Transient]
		UIWindow Window { get; }

		[Export ("removeFromSuperview")]
		void RemoveFromSuperview ();

		[Export ("insertSubview:atIndex:")]
		[PostGet ("Subviews")]
		void InsertSubview (UIView view, nint atIndex);

		[Export ("exchangeSubviewAtIndex:withSubviewAtIndex:")]
		void ExchangeSubview (nint atIndex, nint withSubviewAtIndex);

		[Export ("insertSubview:belowSubview:")]
		[PostGet ("Subviews")]
		void InsertSubviewBelow (UIView view, UIView siblingSubview);

		[Export ("insertSubview:aboveSubview:")]
		[PostGet ("Subviews")]
		void InsertSubviewAbove (UIView view, UIView siblingSubview);

		[Export ("bringSubviewToFront:")]
		void BringSubviewToFront (UIView view);

		[Export ("sendSubviewToBack:")]
		void SendSubviewToBack (UIView view);

		[Export ("didAddSubview:")]
		void SubviewAdded (UIView uiview);

		[Export ("willRemoveSubview:")]
		void WillRemoveSubview (UIView uiview);

		[Export ("willMoveToSuperview:")]
		void WillMoveToSuperview ([NullAllowed] UIView newsuper);

		[Export ("didMoveToSuperview")]
		void MovedToSuperview ();

		[Export ("willMoveToWindow:")]
		void WillMoveToWindow ([NullAllowed] UIWindow window);

		[Export ("didMoveToWindow")]
		void MovedToWindow ();

		[Export ("isDescendantOfView:")]
		bool IsDescendantOfView (UIView view);

		[Export ("viewWithTag:")]
		UIView ViewWithTag (nint tag);

		[Export ("setNeedsLayout")]
		void SetNeedsLayout ();

		[Export ("layoutIfNeeded")]
		void LayoutIfNeeded ();

		[Export ("layoutSubviews")]
		void LayoutSubviews ();

		[Export ("setNeedsDisplay")]
		void SetNeedsDisplay ();

		[Export ("setNeedsDisplayInRect:")]
		void SetNeedsDisplayInRect (CGRect rect);

		[Export ("clipsToBounds")]
		bool ClipsToBounds { get; set; }

		[Export ("alpha")]
		nfloat Alpha { get; set; }

		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")] get; set; }

		[Export ("clearsContextBeforeDrawing")]
		bool ClearsContextBeforeDrawing { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("contentMode")]
		UIViewContentMode ContentMode { get; set; }

		[NoTV]
		[Export ("contentStretch")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'CreateResizableImage' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'CreateResizableImage' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CreateResizableImage' instead.")]
		CGRect ContentStretch { get; set; }

		[Static]
		[Export ("beginAnimations:context:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void BeginAnimations ([NullAllowed] string animationID, IntPtr context);

		[Static]
		[Export ("commitAnimations")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void CommitAnimations ();

		[Static]
		[Export ("setAnimationDelegate:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationDelegate (NSObject del);

		[Static]
		[Export ("setAnimationWillStartSelector:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationWillStartSelector (Selector sel);

		[Static]
		[Export ("setAnimationDidStopSelector:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationDidStopSelector (Selector sel);

		[Static]
		[Export ("setAnimationDuration:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationDuration (double duration);

		[Static]
		[Export ("setAnimationDelay:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationDelay (double delay);

		[Static]
		[Export ("setAnimationStartDate:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationStartDate (NSDate startDate);

		[Static]
		[Export ("setAnimationCurve:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationCurve (UIViewAnimationCurve curve);

		[Static]
		[Export ("setAnimationRepeatCount:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationRepeatCount (float repeatCount /* This is float, not nfloat */);

		[Static]
		[Export ("setAnimationRepeatAutoreverses:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationRepeatAutoreverses (bool repeatAutoreverses);

		[Static]
		[Export ("setAnimationBeginsFromCurrentState:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationBeginsFromCurrentState (bool fromCurrentState);

		[Static]
		[Export ("setAnimationTransition:forView:cache:")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Please use the 'Action' handler based animation APIs instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Please use the 'Action' handler based animation APIs instead.")]
		void SetAnimationTransition (UIViewAnimationTransition transition, UIView forView, bool cache);

		[Static]
		[Export ("areAnimationsEnabled")]
		bool AnimationsEnabled { [Bind ("areAnimationsEnabled")] get; [Bind ("setAnimationsEnabled:")] set; }

		// 3.2:
		[Export ("addGestureRecognizer:"), PostGet ("GestureRecognizers")]
		void AddGestureRecognizer (UIGestureRecognizer gestureRecognizer);

		[Export ("removeGestureRecognizer:"), PostGet ("GestureRecognizers")]
		void RemoveGestureRecognizer (UIGestureRecognizer gestureRecognizer);

		[NullAllowed] // by default this property is null
		[Export ("gestureRecognizers", ArgumentSemantic.Copy)]
		UIGestureRecognizer [] GestureRecognizers { get; set; }

		[Static, Export ("animateWithDuration:animations:")]
		void Animate (double duration, /* non null */ Action animation);

		[Static, Export ("animateWithDuration:animations:completion:")]
		[Async]
		void AnimateNotify (double duration, /* non null */ Action animation, [NullAllowed] UICompletionHandler completion);

		[Static, Export ("animateWithDuration:delay:options:animations:completion:")]
		[Async]
		void AnimateNotify (double duration, double delay, UIViewAnimationOptions options, /* non null */ Action animation, [NullAllowed] UICompletionHandler completion);

		[Static, Export ("transitionFromView:toView:duration:options:completion:")]
		[Async]
		void TransitionNotify (UIView fromView, UIView toView, double duration, UIViewAnimationOptions options, [NullAllowed] UICompletionHandler completion);

		[Static, Export ("transitionWithView:duration:options:animations:completion:")]
		[Async]
		void TransitionNotify (UIView withView, double duration, UIViewAnimationOptions options, [NullAllowed] Action animation, [NullAllowed] UICompletionHandler completion);

		[Export ("contentScaleFactor")]
		nfloat ContentScaleFactor { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("viewPrintFormatter")]
		UIViewPrintFormatter ViewPrintFormatter { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("drawRect:forViewPrintFormatter:")]
		void DrawRect (CGRect area, UIViewPrintFormatter formatter);

		//
		// 6.0
		//

		[Export ("constraints")]
		NSLayoutConstraint [] Constraints { get; }

		[Export ("addConstraint:")]
		void AddConstraint (NSLayoutConstraint constraint);

		[Export ("addConstraints:")]
		void AddConstraints (NSLayoutConstraint [] constraints);

		[Export ("removeConstraint:")]
		void RemoveConstraint (NSLayoutConstraint constraint);

		[Export ("removeConstraints:")]
		void RemoveConstraints (NSLayoutConstraint [] constraints);

		[Export ("needsUpdateConstraints")]
		bool NeedsUpdateConstraints ();

		[Export ("setNeedsUpdateConstraints")]
		void SetNeedsUpdateConstraints ();

		[Static]
		[Export ("requiresConstraintBasedLayout")]
		bool RequiresConstraintBasedLayout ();

		[Export ("alignmentRectForFrame:")]
		CGRect AlignmentRectForFrame (CGRect frame);

		[Export ("frameForAlignmentRect:")]
		CGRect FrameForAlignmentRect (CGRect alignmentRect);

		[Export ("alignmentRectInsets")]
		UIEdgeInsets AlignmentRectInsets { get; }

		[NoTV]
		[Export ("viewForBaselineLayout")]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Override 'ViewForFirstBaselineLayout' or 'ViewForLastBaselineLayout'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Override 'ViewForFirstBaselineLayout' or 'ViewForLastBaselineLayout'.")]
		UIView ViewForBaselineLayout { get; }

		[MacCatalyst (13, 1)]
		[Export ("viewForFirstBaselineLayout")]
		UIView ViewForFirstBaselineLayout { get; }

		[MacCatalyst (13, 1)]
		[Export ("viewForLastBaselineLayout")]
		UIView ViewForLastBaselineLayout { get; }


		[Export ("intrinsicContentSize")]
		CGSize IntrinsicContentSize { get; }

		[Export ("invalidateIntrinsicContentSize")]
		void InvalidateIntrinsicContentSize ();

		[Export ("contentHuggingPriorityForAxis:")]
		float ContentHuggingPriority (UILayoutConstraintAxis axis); // This returns a float, not nfloat.

		[Export ("setContentHuggingPriority:forAxis:")]
		void SetContentHuggingPriority (float priority /* this is a float, not nfloat */, UILayoutConstraintAxis axis);

		[Export ("contentCompressionResistancePriorityForAxis:")]
		float ContentCompressionResistancePriority (UILayoutConstraintAxis axis); // This returns a float, not nfloat.

		[Export ("setContentCompressionResistancePriority:forAxis:")]
		void SetContentCompressionResistancePriority (float priority /* this is a float, not nfloat */, UILayoutConstraintAxis axis);

		[Export ("constraintsAffectingLayoutForAxis:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (UILayoutConstraintAxis axis);

		[Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[Export ("exerciseAmbiguityInLayout")]
		void ExerciseAmbiguityInLayout ();

		[Export ("systemLayoutSizeFittingSize:")]
		CGSize SystemLayoutSizeFittingSize (CGSize size);

		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[NullAllowed] // by default this property is null
		[Export ("restorationIdentifier", ArgumentSemantic.Copy)]
		string RestorationIdentifier { get; set; }

		[Export ("gestureRecognizerShouldBegin:")]
		bool GestureRecognizerShouldBegin (UIGestureRecognizer gestureRecognizer);

		[Export ("translatesAutoresizingMaskIntoConstraints")]
		bool TranslatesAutoresizingMaskIntoConstraints { get; set; }

		[Export ("updateConstraintsIfNeeded")]
		void UpdateConstraintsIfNeeded ();

		[Export ("updateConstraints")]
		[RequiresSuper]
		void UpdateConstraints ();

		[Field ("UIViewNoIntrinsicMetric")]
		nfloat NoIntrinsicMetric { get; }

		[Field ("UILayoutFittingCompressedSize")]
		CGSize UILayoutFittingCompressedSize { get; }

		[Field ("UILayoutFittingExpandedSize")]
		CGSize UILayoutFittingExpandedSize { get; }

		[NullAllowed]
		[Export ("tintColor")]
		[Appearance]
		UIColor TintColor { get; set; }

		[Export ("tintAdjustmentMode")]
		UIViewTintAdjustmentMode TintAdjustmentMode { get; set; }

		[Export ("tintColorDidChange")]
		void TintColorDidChange ();

		[Static, Export ("performWithoutAnimation:")]
		void PerformWithoutAnimation (Action actionsWithoutAnimation);

		[Static, Export ("performSystemAnimation:onViews:options:animations:completion:")]
		[Async]
		void PerformSystemAnimation (UISystemAnimation animation, UIView [] views, UIViewAnimationOptions options, [NullAllowed] Action parallelAnimations, [NullAllowed] UICompletionHandler completion);

		[TV (13, 0), iOS (13, 0)] // Yep headers stated iOS 12 but they are such a liars...
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("modifyAnimationsWithRepeatCount:autoreverses:animations:")]
		void ModifyAnimations (nfloat count, bool autoreverses, Action animations);

		[Static, Export ("animateKeyframesWithDuration:delay:options:animations:completion:")]
		[Async]
		void AnimateKeyframes (double duration, double delay, UIViewKeyframeAnimationOptions options, Action animations, [NullAllowed] UICompletionHandler completion);

		[Static, Export ("addKeyframeWithRelativeStartTime:relativeDuration:animations:")]
		void AddKeyframeWithRelativeStartTime (double frameStartTime, double frameDuration, Action animations);

		[Export ("addMotionEffect:")]
		[PostGet ("MotionEffects")]
		void AddMotionEffect (UIMotionEffect effect);

		[Export ("removeMotionEffect:")]
		[PostGet ("MotionEffects")]
		void RemoveMotionEffect (UIMotionEffect effect);

		[NullAllowed] // by default this property is null
		[Export ("motionEffects", ArgumentSemantic.Copy)]
		UIMotionEffect [] MotionEffects { get; set; }

		[Export ("snapshotViewAfterScreenUpdates:")]
		UIView SnapshotView (bool afterScreenUpdates);

		[Export ("resizableSnapshotViewFromRect:afterScreenUpdates:withCapInsets:")]
		[return: NullAllowed]
		UIView ResizableSnapshotView (CGRect rect, bool afterScreenUpdates, UIEdgeInsets capInsets);

		[Export ("drawViewHierarchyInRect:afterScreenUpdates:")]
		bool DrawViewHierarchy (CGRect rect, bool afterScreenUpdates);

		[Static]
		[Export ("animateWithDuration:delay:usingSpringWithDamping:initialSpringVelocity:options:animations:completion:")]
		[Async]
		void AnimateNotify (double duration, double delay, nfloat springWithDampingRatio, nfloat initialSpringVelocity, UIViewAnimationOptions options, Action animations, [NullAllowed] UICompletionHandler completion);


		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("maskView", ArgumentSemantic.Retain)]
		UIView MaskView { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("systemLayoutSizeFittingSize:withHorizontalFittingPriority:verticalFittingPriority:")]
		// float, not CGFloat / nfloat, but we can't use an enum in the signature
		CGSize SystemLayoutSizeFittingSize (CGSize targetSize, /* UILayoutPriority */ float horizontalFittingPriority, /* UILayoutPriority */ float verticalFittingPriority);

		[MacCatalyst (13, 1)]
		[Export ("layoutMargins")]
		UIEdgeInsets LayoutMargins { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("directionalLayoutMargins", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets DirectionalLayoutMargins { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preservesSuperviewLayoutMargins")]
		bool PreservesSuperviewLayoutMargins { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("insetsLayoutMarginsFromSafeArea")]
		bool InsetsLayoutMarginsFromSafeArea { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("layoutMarginsDidChange")]
		void LayoutMarginsDidChange ();

		[MacCatalyst (13, 1)]
		[Export ("safeAreaInsets")]
		UIEdgeInsets SafeAreaInsets { get; }

		[MacCatalyst (13, 1)]
		[Export ("safeAreaInsetsDidChange")]
		void SafeAreaInsetsDidChange ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("userInterfaceLayoutDirectionForSemanticContentAttribute:")]
		UIUserInterfaceLayoutDirection GetUserInterfaceLayoutDirection (UISemanticContentAttribute attribute);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("userInterfaceLayoutDirectionForSemanticContentAttribute:relativeToLayoutDirection:")]
		UIUserInterfaceLayoutDirection GetUserInterfaceLayoutDirection (UISemanticContentAttribute semanticContentAttribute, UIUserInterfaceLayoutDirection layoutDirection);

		[MacCatalyst (13, 1)]
		[Export ("effectiveUserInterfaceLayoutDirection")]
		UIUserInterfaceLayoutDirection EffectiveUserInterfaceLayoutDirection { get; }

		[MacCatalyst (13, 1)]
		[Export ("semanticContentAttribute", ArgumentSemantic.Assign)]
		UISemanticContentAttribute SemanticContentAttribute { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("layoutMarginsGuide", ArgumentSemantic.Strong)]
		UILayoutGuide LayoutMarginsGuide { get; }

		[MacCatalyst (13, 1)]
		[Export ("readableContentGuide", ArgumentSemantic.Strong)]
		UILayoutGuide ReadableContentGuide { get; }

		[MacCatalyst (13, 1)]
		[Export ("safeAreaLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide SafeAreaLayoutGuide { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("keyboardLayoutGuide")]
		UIKeyboardLayoutGuide KeyboardLayoutGuide { get; }

		[MacCatalyst (13, 1)]
		[Export ("inheritedAnimationDuration")]
		[Static]
		double InheritedAnimationDuration { get; }

		[MacCatalyst (13, 1)]
		[Export ("leadingAnchor")]
		NSLayoutXAxisAnchor LeadingAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("trailingAnchor")]
		NSLayoutXAxisAnchor TrailingAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("leftAnchor")]
		NSLayoutXAxisAnchor LeftAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("rightAnchor")]
		NSLayoutXAxisAnchor RightAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("topAnchor")]
		NSLayoutYAxisAnchor TopAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("bottomAnchor")]
		NSLayoutYAxisAnchor BottomAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("widthAnchor")]
		NSLayoutDimension WidthAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("heightAnchor")]
		NSLayoutDimension HeightAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("centerXAnchor")]
		NSLayoutXAxisAnchor CenterXAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("centerYAnchor")]
		NSLayoutYAxisAnchor CenterYAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("firstBaselineAnchor")]
		NSLayoutYAxisAnchor FirstBaselineAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("lastBaselineAnchor")]
		NSLayoutYAxisAnchor LastBaselineAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("layoutGuides")]
		UILayoutGuide [] LayoutGuides { get; }

		[MacCatalyst (13, 1)]
		[Export ("addLayoutGuide:")]
		void AddLayoutGuide (UILayoutGuide guide);

		[MacCatalyst (13, 1)]
		[Export ("removeLayoutGuide:")]
		void RemoveLayoutGuide (UILayoutGuide guide);

		[MacCatalyst (13, 1)]
		[Export ("focused")]
		bool Focused { [Bind ("isFocused")] get; }

		[NullAllowed]
		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("focusGroupIdentifier")]
		new string FocusGroupIdentifier { get; set; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("focusGroupPriority")]
		new nint FocusGroupPriority { get; set; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("focusEffect", ArgumentSemantic.Copy)]
		new UIFocusEffect FocusEffect { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canBecomeFocused")]
		new bool CanBecomeFocused { get; }

		[Watch (5, 0), TV (13, 0)] // Headers state Watch 5.0
		[MacCatalyst (13, 1)]
		[Export ("addInteraction:")]
		void AddInteraction (IUIInteraction interaction);

		[Watch (5, 0), TV (13, 0)] // Headers state Watch 5.0
		[MacCatalyst (13, 1)]
		[Export ("removeInteraction:")]
		void RemoveInteraction (IUIInteraction interaction);

		[Watch (5, 0), TV (13, 0)] // Headers state Watch 5.0
		[MacCatalyst (13, 1)]
		[Export ("interactions", ArgumentSemantic.Copy)]
		IUIInteraction [] Interactions { get; set; }

		// UIAccessibilityInvertColors category
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityIgnoresInvertColors")]
		bool AccessibilityIgnoresInvertColors { get; set; }

		// From UserInterfaceStyle category

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("overrideUserInterfaceStyle", ArgumentSemantic.Assign)]
		UIUserInterfaceStyle OverrideUserInterfaceStyle { get; set; }

		[NoWatch, TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("transform3D", ArgumentSemantic.Assign)]
		CATransform3D Transform3D { get; set; }

		// Category UIView (UIContentSizeCategoryLimit)

		[BindAs (typeof (UIContentSizeCategory))]
		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("minimumContentSizeCategory")]
		NSString MinimumContentSizeCategory { get; set; }

		[BindAs (typeof (UIContentSizeCategory))]
		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("maximumContentSizeCategory")]
		NSString MaximumContentSizeCategory { get; set; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("appliedContentSizeCategoryLimitsDescription")]
		string AppliedContentSizeCategoryLimitsDescription { get; }

#if TVOS
#pragma warning disable 0109 // The member 'member' does not hide an inherited member. The new keyword is not required
#endif
		// From UIView (UILargeContentViewer)

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("showsLargeContentViewer")]
		new bool ShowsLargeContentViewer { get; set; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("largeContentTitle")]
		new string LargeContentTitle { get; set; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("largeContentImage", ArgumentSemantic.Strong)]
		new UIImage LargeContentImage { get; set; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("scalesLargeContentImage")]
		new bool ScalesLargeContentImage { get; set; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("largeContentImageInsets", ArgumentSemantic.Assign)]
		new UIEdgeInsets LargeContentImageInsets { get; set; }
#if TVOS
#pragma warning restore 0109
#endif

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("anchorPoint", ArgumentSemantic.Assign)]
		CGPoint AnchorPoint { get; set; }

		// from the category (UIView) <UITraitChangeObservable> 
		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitOverrides")]
		IUITraitOverrides TraitOverrides { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("updateTraitsIfNeeded")]
		void UpdateTraitsIfNeeded ();

		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("hoverStyle", ArgumentSemantic.Copy)]
		UIHoverStyle HoverStyle { get; set; }
		[Async]
		[iOS (17, 0)]
		[Static]
		[Export ("animateWithSpringDuration:bounce:initialSpringVelocity:delay:options:animations:completion:")]
		void Animate (double duration, nfloat bounce, nfloat velocity, double delay, UIViewAnimationOptions options, Action animations, [NullAllowed] Action<bool> completion);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category, BaseType (typeof (UIView))]
	interface UIView_UITextField {
		[Export ("endEditing:")]
		bool EndEditing (bool force);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (UILayoutGuide))]
	interface UILayoutGuide_UIConstraintBasedLayoutDebugging {

		[Export ("constraintsAffectingLayoutForAxis:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (UILayoutConstraintAxis axis);

		[Export ("hasAmbiguousLayout")]
		bool GetHasAmbiguousLayout ();
	}

	interface IUIContentContainer { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIResponder))]
	interface UIViewController : NSCoding, UIAppearanceContainer, UIContentContainer, UITraitEnvironment, UIFocusEnvironment, NSExtensionRequestHandling {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("view", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView View { get; set; }

		[Export ("loadView")]
		void LoadView ();

		[Export ("viewDidLoad")]
		void ViewDidLoad ();

		[NoTV]
		[Export ("viewDidUnload")]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void ViewDidUnload ();

		[Export ("isViewLoaded")]
		bool IsViewLoaded { get; }

		[NullAllowed]
		[Export ("nibName", ArgumentSemantic.Copy)]
		string NibName { get; }

		[NullAllowed]
		[Export ("nibBundle", ArgumentSemantic.Retain)]
		NSBundle NibBundle { get; }

		[Export ("viewWillAppear:")]
		void ViewWillAppear (bool animated);

		[Export ("viewDidAppear:")]
		void ViewDidAppear (bool animated);

		[Export ("viewWillDisappear:")]
		void ViewWillDisappear (bool animated);

		[Export ("viewDidDisappear:")]
		void ViewDidDisappear (bool animated);

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("didReceiveMemoryWarning")]
		void DidReceiveMemoryWarning ();

		[NoTV]
		[Export ("presentModalViewController:animated:")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'PresentViewController (UIViewController, bool, NSAction)' instead and set the 'ModalViewController' property to true.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PresentViewController (UIViewController, bool, NSAction)' instead and set the 'ModalViewController' property to true.")]
		void PresentModalViewController (UIViewController modalViewController, bool animated);

		[NoTV]
		[Export ("dismissModalViewControllerAnimated:")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'DismissViewController (bool, NSAction)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DismissViewController (bool, NSAction)' instead.")]
		void DismissModalViewController (bool animated);

		[NoTV]
		[Export ("modalViewController")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'PresentedViewController' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PresentedViewController' instead.")]
		UIViewController ModalViewController { get; }

		[Export ("modalTransitionStyle", ArgumentSemantic.Assign)]
		UIModalTransitionStyle ModalTransitionStyle { get; set; }

		[NoTV]
		[Export ("wantsFullScreenLayout", ArgumentSemantic.Assign)]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'EdgesForExtendedLayout', 'ExtendedLayoutIncludesOpaqueBars' and 'AutomaticallyAdjustsScrollViewInsets' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'EdgesForExtendedLayout', 'ExtendedLayoutIncludesOpaqueBars' and 'AutomaticallyAdjustsScrollViewInsets' instead.")]
		bool WantsFullScreenLayout { get; set; }

		[Export ("parentViewController")]
		[NullAllowed]
		UIViewController ParentViewController { get; }

		[Export ("tabBarItem", ArgumentSemantic.Retain)]
		UITabBarItem TabBarItem { get; set; }

		// UIViewControllerRotation category

		[NoTV]
		[Export ("shouldAutorotateToInterfaceOrientation:")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use both 'GetSupportedInterfaceOrientations' and 'PreferredInterfaceOrientationForPresentation' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use both 'GetSupportedInterfaceOrientations' and 'PreferredInterfaceOrientationForPresentation' instead.")]
		bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation);

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		[Export ("rotatingHeaderView")]
		UIView RotatingHeaderView { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		[Export ("rotatingFooterView")]
		UIView RotatingFooterView { get; }

		[NoTV]
		[Export ("interfaceOrientation")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		UIInterfaceOrientation InterfaceOrientation { get; }

		[NoTV]
		[Export ("willRotateToInterfaceOrientation:duration:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration);

		[NoTV]
		[Export ("didRotateFromInterfaceOrientation:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		void DidRotate (UIInterfaceOrientation fromInterfaceOrientation);

		[NoTV]
		[Export ("willAnimateRotationToInterfaceOrientation:duration:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration);

		[NoTV]
		[Export ("willAnimateFirstHalfOfRotationToInterfaceOrientation:duration:")]
		[Deprecated (PlatformName.iOS, 5, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillAnimateFirstHalfOfRotation (UIInterfaceOrientation toInterfaceOrientation, double duration);

		[NoTV]
		[Export ("didAnimateFirstHalfOfRotationToInterfaceOrientation:")]
		[Deprecated (PlatformName.iOS, 5, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void DidAnimateFirstHalfOfRotation (UIInterfaceOrientation toInterfaceOrientation);

		[NoTV]
		[Export ("willAnimateSecondHalfOfRotationFromInterfaceOrientation:duration:")]
		[Deprecated (PlatformName.iOS, 5, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		void WillAnimateSecondHalfOfRotation (UIInterfaceOrientation fromInterfaceOrientation, double duration);

		// These come from @interface UIViewController (UIViewControllerEditing)
		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }

		[Export ("setEditing:animated:")]
		void SetEditing (bool editing, bool animated);

		[Export ("editButtonItem")]
		UIBarButtonItem EditButtonItem { get; }

		// These come from @interface UIViewController (UISearchDisplayControllerSupport)
		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController' instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController' instead.")]
		[Export ("searchDisplayController", ArgumentSemantic.Retain)]
		UISearchDisplayController SearchDisplayController { get; }

		// These come from @interface UIViewController (UINavigationControllerItem)
		[Export ("navigationItem", ArgumentSemantic.Retain)]
		UINavigationItem NavigationItem { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("hidesBottomBarWhenPushed")]
		bool HidesBottomBarWhenPushed { get; set; }

		[NullAllowed]
		[Export ("splitViewController", ArgumentSemantic.Retain)]
		UISplitViewController SplitViewController { get; }

		[NullAllowed]
		[Export ("tabBarController", ArgumentSemantic.Retain)]
		UITabBarController TabBarController { get; }

		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'UIViewController.SetContentScrollView' instead.")]
		[TV (13, 0), NoWatch, NoiOS]
		[NoMacCatalyst]
		[NullAllowed, Export ("tabBarObservedScrollView", ArgumentSemantic.Strong)]
		UIScrollView TabBarObservedScrollView { get; set; }

		[NullAllowed]
		[Export ("navigationController", ArgumentSemantic.Retain)]
		UINavigationController NavigationController { get; }

		// These come from @interface UIViewController (UINavigationControllerContextualToolbarItems)
		[Export ("toolbarItems", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIBarButtonItem [] ToolbarItems { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setToolbarItems:animated:")]
		[PostGet ("ToolbarItems")]
		void SetToolbarItems ([NullAllowed] UIBarButtonItem [] items, bool animated);

		// These come in 3.2

		[Export ("modalPresentationStyle", ArgumentSemantic.Assign)]
		UIModalPresentationStyle ModalPresentationStyle { get; set; }

		// 3.2 extensions from MoviePlayer
		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AVPlayerViewController' (AVKit) instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Export ("presentMoviePlayerViewControllerAnimated:")]
		void PresentMoviePlayerViewController (MPMoviePlayerViewController moviePlayerViewController);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AVPlayerViewController' (AVKit) instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Export ("dismissMoviePlayerViewControllerAnimated")]
		void DismissMoviePlayerViewController ();


		// This is defined in a category in UIPopoverSupport.h: UIViewController (UIPopoverController)
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'PreferredContentSize' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredContentSize' instead.")]
		[Export ("contentSizeForViewInPopover")]
		CGSize ContentSizeForViewInPopover { get; set; }

		// This is defined in a category in UIPopoverSupport.h: UIViewController (UIPopoverController)
		[Export ("modalInPopover")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ModalInPresentation' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'ModalInPresentation' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ModalInPresentation' instead.")]
		bool ModalInPopover { [Bind ("isModalInPopover")] get; set; }

		// It seems apple added a setter now but seems it is a mistake on new property radar:27929872
		[Export ("disablesAutomaticKeyboardDismissal")]
		bool DisablesAutomaticKeyboardDismissal { get; }

		[Export ("storyboard", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIStoryboard Storyboard { get; }

		[Export ("presentedViewController")]
		[NullAllowed]
		UIViewController PresentedViewController { get; }

		[Export ("presentingViewController")]
		[NullAllowed]
		UIViewController PresentingViewController { get; }

		[Export ("definesPresentationContext", ArgumentSemantic.Assign)]
		bool DefinesPresentationContext { get; set; }

		[Export ("providesPresentationContextTransitionStyle", ArgumentSemantic.Assign)]
		bool ProvidesPresentationContextTransitionStyle { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("viewWillUnload")]
		void ViewWillUnload ();

		[Export ("performSegueWithIdentifier:sender:")]
		void PerformSegue (string identifier, [NullAllowed] NSObject sender);

		[Export ("prepareForSegue:sender:")]
		void PrepareForSegue (UIStoryboardSegue segue, [NullAllowed] NSObject sender);

		[Export ("viewWillLayoutSubviews")]
		void ViewWillLayoutSubviews ();

		[Export ("viewDidLayoutSubviews")]
		void ViewDidLayoutSubviews ();

		[Export ("isBeingPresented")]
		bool IsBeingPresented { get; }

		[Export ("isBeingDismissed")]
		bool IsBeingDismissed { get; }

		[Export ("isMovingToParentViewController")]
		bool IsMovingToParentViewController { get; }

		[Export ("isMovingFromParentViewController")]
		bool IsMovingFromParentViewController { get; }

		[Export ("presentViewController:animated:completion:")]
		[Async]
		void PresentViewController (UIViewController viewControllerToPresent, bool animated, [NullAllowed] Action completionHandler);

		[Export ("dismissViewControllerAnimated:completion:")]
		[Async]
		void DismissViewController (bool animated, [NullAllowed] Action completionHandler);

		// UIViewControllerRotation
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("attemptRotationToDeviceOrientation")]
		void AttemptRotationToDeviceOrientation ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 6, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers")]
		/*PROTECTED*/
		bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers { get; }

		[Export ("childViewControllers")]
		/*PROTECTED, MUSTCALLBASE*/
		UIViewController [] ChildViewControllers { get; }

		[Export ("addChildViewController:")]
		[PostGet ("ChildViewControllers")]
		/*PROTECTED, MUSTCALLBASE*/
		void AddChildViewController (UIViewController childController);

		[Export ("removeFromParentViewController")]
		/*PROTECTED, MUSTCALLBASE*/
		void RemoveFromParentViewController ();

		[Export ("transitionFromViewController:toViewController:duration:options:animations:completion:")]
		[Async]
		/*PROTECTED, MUSTCALLBASE*/
		void Transition (UIViewController fromViewController, UIViewController toViewController, double duration, UIViewAnimationOptions options, [NullAllowed] Action animations, [NullAllowed] UICompletionHandler completionHandler);

		[Export ("willMoveToParentViewController:")]
		void WillMoveToParentViewController ([NullAllowed] UIViewController parent);

		[Export ("didMoveToParentViewController:")]
		void DidMoveToParentViewController ([NullAllowed] UIViewController parent);

		//
		// Exposed in iOS 6.0, but they existed and are now officially supported on iOS 5.0
		//
		[Export ("beginAppearanceTransition:animated:")]
		void BeginAppearanceTransition (bool isAppearing, bool animated);

		[Export ("endAppearanceTransition")]
		void EndAppearanceTransition ();

		//
		// 6.0
		//
		[Export ("shouldPerformSegueWithIdentifier:sender:")]
		bool ShouldPerformSegue (string segueIdentifier, [NullAllowed] NSObject sender);

#if !NET
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CanPerformUnwindSegueAction' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'CanPerformUnwindSegueAction' instead.")]
#else
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'CanPerformUnwind' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'CanPerformUnwind' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'CanPerformUnwind' instead.")]
#endif
		[Export ("canPerformUnwindSegueAction:fromViewController:withSender:")]
#if !NET
		bool CanPerformUnwind (Selector segueAction, UIViewController fromViewController, NSObject sender);
#else
		bool CanPerformUnwindDeprecated (Selector segueAction, UIViewController fromViewController, NSObject sender);
#endif

		// Apple decided to rename the selector and it clashes with our current one
		// we will get the right name 'CanPerformUnwind' if NET happens, use CanPerformUnwindSegueAction for now.
		[TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("canPerformUnwindSegueAction:fromViewController:sender:")]
#if !NET
		bool CanPerformUnwindSegueAction (Selector segueAction, UIViewController fromViewController, [NullAllowed] NSObject sender);
#else
		bool CanPerformUnwind (Selector segueAction, UIViewController fromViewController, [NullAllowed] NSObject sender);
#endif

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("viewControllerForUnwindSegueAction:fromViewController:withSender:")]
		UIViewController GetViewControllerForUnwind (Selector segueAction, UIViewController fromViewController, NSObject sender);

		[Deprecated (PlatformName.iOS, 9, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("segueForUnwindingToViewController:fromViewController:identifier:")]
		UIStoryboardSegue GetSegueForUnwinding (UIViewController toViewController, UIViewController fromViewController, string identifier);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("supportedInterfaceOrientations")]
		UIInterfaceOrientationMask GetSupportedInterfaceOrientations ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("preferredInterfaceOrientationForPresentation")]
		UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ();

		[NoTV]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use Adaptive View Controllers instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use Adaptive View Controllers instead.")]
		[Export ("shouldAutomaticallyForwardRotationMethods")]
		bool ShouldAutomaticallyForwardRotationMethods { get; }

		[Export ("shouldAutomaticallyForwardAppearanceMethods")]
		bool ShouldAutomaticallyForwardAppearanceMethods { get; }

		[NullAllowed] // by default this property is null
		[Export ("restorationIdentifier", ArgumentSemantic.Copy)]
		string RestorationIdentifier { get; set; }

		[NullAllowed]
		[Export ("restorationClass")]
		Class RestorationClass { get; set; }

		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[Export ("updateViewConstraints")]
		void UpdateViewConstraints ();

		[Deprecated (PlatformName.iOS, 16, 0)]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 16, 0)]
		[Export ("shouldAutorotate")]
		bool ShouldAutorotate ();

		[Export ("edgesForExtendedLayout", ArgumentSemantic.Assign)]
		UIRectEdge EdgesForExtendedLayout { get; set; }

		[Export ("extendedLayoutIncludesOpaqueBars", ArgumentSemantic.Assign)]
		bool ExtendedLayoutIncludesOpaqueBars { get; set; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIScrollView.ContentInsetAdjustmentBehavior' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIScrollView.ContentInsetAdjustmentBehavior' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIScrollView.ContentInsetAdjustmentBehavior' instead.")]
		[Export ("automaticallyAdjustsScrollViewInsets", ArgumentSemantic.Assign)]
		bool AutomaticallyAdjustsScrollViewInsets { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("setContentScrollView:forEdge:")]
		void SetContentScrollView ([NullAllowed] UIScrollView scrollView, NSDirectionalRectEdge edge);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("contentScrollViewForEdge:")]
		[return: NullAllowed]
		UIScrollView GetContentScrollView (NSDirectionalRectEdge edge);

		[Export ("preferredContentSize", ArgumentSemantic.Copy)]
		new CGSize PreferredContentSize { get; set; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("preferredStatusBarStyle")]
		UIStatusBarStyle PreferredStatusBarStyle ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("prefersStatusBarHidden")]
		bool PrefersStatusBarHidden ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setNeedsStatusBarAppearanceUpdate")]
		void SetNeedsStatusBarAppearanceUpdate ();

		[Export ("applicationFinishedRestoringState")]
		void ApplicationFinishedRestoringState ();

		[Export ("transitioningDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakTransitioningDelegate { get; set; }

		[Wrap ("WeakTransitioningDelegate")]
		[Protocolize]
		UIViewControllerTransitioningDelegate TransitioningDelegate { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("childViewControllerForStatusBarStyle")]
		[return: NullAllowed]
		UIViewController ChildViewControllerForStatusBarStyle ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("childViewControllerForStatusBarHidden")]
		[return: NullAllowed]
		UIViewController ChildViewControllerForStatusBarHidden ();

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.TopAnchor' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.TopAnchor' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIView.SafeAreaLayoutGuide.TopAnchor' instead.")]
		[Export ("topLayoutGuide")]
		IUILayoutSupport TopLayoutGuide { get; }

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.BottomAnchor' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.BottomAnchor' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIView.SafeAreaLayoutGuide.BottomAnchor' instead.")]
		[Export ("bottomLayoutGuide")]
		IUILayoutSupport BottomLayoutGuide { get; }

		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("preferredStatusBarUpdateAnimation")]
		UIStatusBarAnimation PreferredStatusBarUpdateAnimation { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("modalPresentationCapturesStatusBarAppearance", ArgumentSemantic.Assign)]
		bool ModalPresentationCapturesStatusBarAppearance { get; set; }

		//
		// iOS 8
		//

		[MacCatalyst (13, 1)]
		[Export ("targetViewControllerForAction:sender:")]
		[return: NullAllowed]
		UIViewController GetTargetViewControllerForAction (Selector action, [NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("showDetailViewController:sender:")]
		void ShowDetailViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the 'TraitOverrides' property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the 'TraitOverrides' instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Use the 'TraitOverrides' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("setOverrideTraitCollection:forChildViewController:")]
		void SetOverrideTraitCollection (UITraitCollection collection, UIViewController childViewController);

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the 'TraitOverrides' property of th echild view controller instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the 'TraitOverrides' property of th echild view controller instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Use the 'TraitOverrides' property of th echild view controller instead.")]
		[MacCatalyst (13, 1)]
		[Export ("overrideTraitCollectionForChildViewController:")]
		UITraitCollection GetOverrideTraitCollectionForChildViewController (UIViewController childViewController);

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("extensionContext")]
		NSExtensionContext ExtensionContext { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("presentationController")]
		UIPresentationController PresentationController { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("popoverPresentationController")]
		UIPopoverPresentationController PopoverPresentationController { get; }

		[NoTV, NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("sheetPresentationController")]
		UISheetPresentationController SheetPresentationController { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIViewControllerShowDetailTargetDidChangeNotification")]
		[Notification]
		NSString ShowDetailTargetDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("loadViewIfNeeded")]
		void LoadViewIfNeeded ();

		[MacCatalyst (13, 1)]
		[Export ("viewIfLoaded", ArgumentSemantic.Strong), NullAllowed]
		UIView ViewIfLoaded { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowedChildViewControllersForUnwindingFromSource:")]
		UIViewController [] GetAllowedChildViewControllersForUnwinding (UIStoryboardUnwindSegueSource segueSource);

		[MacCatalyst (13, 1)]
		[Export ("childViewControllerContainingSegueSource:")]
		[return: NullAllowed]
		UIViewController GetChildViewControllerContainingSegueSource (UIStoryboardUnwindSegueSource segueSource);

		[MacCatalyst (13, 1)]
		[Export ("unwindForSegue:towardsViewController:")]
		void Unwind (UIStoryboardSegue unwindSegue, UIViewController subsequentVC);

		[MacCatalyst (13, 1)]
		[Export ("addKeyCommand:")]
		void AddKeyCommand (UIKeyCommand command);

		[MacCatalyst (13, 1)]
		[Export ("removeKeyCommand:")]
		void RemoveKeyCommand (UIKeyCommand command);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Export ("registerForPreviewingWithDelegate:sourceView:")]
		IUIViewControllerPreviewing RegisterForPreviewingWithDelegate (IUIViewControllerPreviewingDelegate previewingDelegate, UIView sourceView);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Export ("unregisterForPreviewingWithContext:")]
		void UnregisterForPreviewingWithContext (IUIViewControllerPreviewing previewing);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Replaced by 'UIContextMenuInteraction'.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'UIContextMenuInteraction'.")]
		[Export ("previewActionItems")]
		IUIPreviewActionItem [] PreviewActionItems { get; }

		[Field ("UIViewControllerHierarchyInconsistencyException")]
		NSString HierarchyInconsistencyException { get; }

		[MacCatalyst (13, 1)]
		[Export ("restoresFocusAfterTransition")]
		bool RestoresFocusAfterTransition { get; set; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("focusGroupIdentifier")]
		new string FocusGroupIdentifier { get; set; }

		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("preferredUserInterfaceStyle")]
		UIUserInterfaceStyle PreferredUserInterfaceStyle { get; }

		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[Export ("setNeedsUserInterfaceAppearanceUpdate")]
		void SetNeedsUserInterfaceAppearanceUpdate ();

		[NoWatch, NoiOS]
		[NoMacCatalyst]
		[NullAllowed, Export ("childViewControllerForUserInterfaceStyle")]
		UIViewController ChildViewControllerForUserInterfaceStyle { get; }

		[MacCatalyst (13, 1)]
		[Export ("additionalSafeAreaInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets AdditionalSafeAreaInsets { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("systemMinimumLayoutMargins")]
		NSDirectionalEdgeInsets SystemMinimumLayoutMargins { get; }

		[MacCatalyst (13, 1)]
		[Export ("viewRespectsSystemMinimumLayoutMargins")]
		bool ViewRespectsSystemMinimumLayoutMargins { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("viewLayoutMarginsDidChange")]
		[RequiresSuper]
		void ViewLayoutMarginsDidChange ();

		[MacCatalyst (13, 1)]
		[Export ("viewSafeAreaInsetsDidChange")]
		[RequiresSuper]
		void ViewSafeAreaInsetsDidChange ();

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("childViewControllerForScreenEdgesDeferringSystemGestures")]
		UIViewController ChildViewControllerForScreenEdgesDeferringSystemGestures { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("preferredScreenEdgesDeferringSystemGestures")]
		UIRectEdge PreferredScreenEdgesDeferringSystemGestures { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setNeedsUpdateOfScreenEdgesDeferringSystemGestures")]
		void SetNeedsUpdateOfScreenEdgesDeferringSystemGestures ();

		// UIHomeIndicatorAutoHidden (UIViewController) category

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("childViewControllerForHomeIndicatorAutoHidden")]
		UIViewController ChildViewControllerForHomeIndicatorAutoHidden { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("prefersHomeIndicatorAutoHidden")]
		bool PrefersHomeIndicatorAutoHidden { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("setNeedsUpdateOfHomeIndicatorAutoHidden")]
		void SetNeedsUpdateOfHomeIndicatorAutoHidden ();

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("overrideUserInterfaceStyle", ArgumentSemantic.Assign)]
		UIUserInterfaceStyle OverrideUserInterfaceStyle { get; set; }

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("modalInPresentation")]
		bool ModalInPresentation { [Bind ("isModalInPresentation")] get; set; }

		// From UIViewController (UIPerformsActions)

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("performsActionsWhilePresentingModally")]
		bool PerformsActionsWhilePresentingModally { get; }

		// From UIViewController (UIPointerLockSupport) category

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("childViewControllerForPointerLock")]
		UIViewController ChildViewControllerForPointerLock { get; }

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("prefersPointerLocked")]
		bool PrefersPointerLocked { get; }

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setNeedsUpdateOfPrefersPointerLocked")]
		void SetNeedsUpdateOfPrefersPointerLocked ();

		[NoiOS]
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("setNeedsTouchBarUpdate")]
		void SetNeedsTouchBarUpdate ();

		[NoiOS]
		[NoTV]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("childViewControllerForTouchBar")]
		UIViewController ChildViewControllerForTouchBar { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("interactionActivityTrackingBaseName")]
		[NullAllowed]
		string InteractionActivityTrackingBaseName { get; set; }

		[TV (16, 0), NoWatch, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("activePresentationController")]
		[NullAllowed]
		UIPresentationController ActivePresentationController { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("setNeedsUpdateOfSupportedInterfaceOrientations")]
		void SetNeedsUpdateOfSupportedInterfaceOrientations ();

		// from the category (UIViewController) <UITraitChangeObservable>

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitOverrides")]
		IUITraitOverrides TraitOverrides { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("updateTraitsIfNeeded")]
		void UpdateTraitsIfNeeded ();

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("contentUnavailableConfiguration", ArgumentSemantic.Copy)]
		IUIContentConfiguration ContentUnavailableConfiguration { get; set; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("contentUnavailableConfigurationState")]
		UIContentUnavailableConfigurationState ContentUnavailableConfigurationState { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setNeedsUpdateContentUnavailableConfiguration")]
		void SetNeedsUpdateContentUnavailableConfiguration ();

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("updateContentUnavailableConfigurationUsingState:")]
		void UpdateContentUnavailableConfiguration (UIContentUnavailableConfigurationState state);

		[TV (13, 0), NoWatch, iOS (13, 0), MacCatalyst (13, 1)]
		[Export ("viewIsAppearing:")]
		void ViewIsAppearing (bool animated);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model, BaseType (typeof (NSObject))]
	partial interface UIViewControllerContextTransitioning {
		[Abstract]
		[Export ("containerView")]
		UIView ContainerView { get; }

		[Abstract]
		[Export ("isAnimated")]
		bool IsAnimated { get; }

		[Abstract]
		[Export ("isInteractive")]
		bool IsInteractive { get; }

		[Abstract]
		[Export ("transitionWasCancelled")]
		bool TransitionWasCancelled { get; }

		[Abstract]
		[Export ("presentationStyle")]
		UIModalPresentationStyle PresentationStyle { get; }

		[Abstract]
		[Export ("updateInteractiveTransition:")]
		void UpdateInteractiveTransition (nfloat percentComplete);

		[Abstract]
		[Export ("finishInteractiveTransition")]
		void FinishInteractiveTransition ();

		[Abstract]
		[Export ("cancelInteractiveTransition")]
		void CancelInteractiveTransition ();

		[Abstract]
		[Export ("completeTransition:")]
		void CompleteTransition (bool didComplete);

		[Abstract]
		[Export ("viewControllerForKey:")]
		UIViewController GetViewControllerForKey (NSString uiTransitionKey);

		[Abstract]
		[Export ("initialFrameForViewController:")]
		CGRect GetInitialFrameForViewController (UIViewController vc);

		[Abstract]
		[Export ("finalFrameForViewController:")]
		CGRect GetFinalFrameForViewController (UIViewController vc);

		[Abstract]
		[Export ("viewForKey:")]
		UIView GetViewFor (NSString uiTransitionContextToOrFromKey);

		[Abstract]
		[Export ("targetTransform")]
		CGAffineTransform TargetTransform { get; }


#if NET // Can't break the world right now
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("pauseInteractiveTransition")]
		void PauseInteractiveTransition ();
	}

	interface IUIViewControllerContextTransitioning {
	}

	interface IUITraitEnvironment { }
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoWatch]
	[MacCatalyst (13, 1)]
	partial interface UITraitEnvironment {
		[Abstract]
		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }

		[Deprecated (PlatformName.iOS, 17, 0, message: "Use the 'UITraitChangeObservable' protocol instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Use the 'UITraitChangeObservable' protocol instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Use the 'UITraitChangeObservable' protocol instead.")]
		[Abstract]
		[Export ("traitCollectionDidChange:")]
		void TraitCollectionDidChange ([NullAllowed] UITraitCollection previousTraitCollection);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	[ThreadSafe] // Documentation doesn't say, but it this class doesn't seem to trigger Apple's Main Thread Checker.
	partial interface UITraitCollection : NSCopying, NSSecureCoding {
		[Export ("userInterfaceIdiom")]
		UIUserInterfaceIdiom UserInterfaceIdiom { get; }

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("userInterfaceStyle")]
		UIUserInterfaceStyle UserInterfaceStyle { get; }

		[Export ("displayScale")]
		nfloat DisplayScale { get; }

		[Export ("horizontalSizeClass")]
		UIUserInterfaceSizeClass HorizontalSizeClass { get; }

		[Export ("verticalSizeClass")]
		UIUserInterfaceSizeClass VerticalSizeClass { get; }

		[Deprecated (PlatformName.iOS, 17, 0, message: "Compare the values for the specific items instead.")]
		[Deprecated (PlatformName.MacCatalyst, 17, 0, message: "Compare the values for the specific items instead.")]
		[Deprecated (PlatformName.TvOS, 17, 0, message: "Compare the values for the specific items instead.")]
		[Export ("containsTraitsInCollection:")]
		bool Contains (UITraitCollection trait);

		[Static, Export ("traitCollectionWithTraitsFromCollections:")]
		UITraitCollection FromTraitsFromCollections (UITraitCollection [] traitCollections);

		[Static, Export ("traitCollectionWithUserInterfaceIdiom:")]
		UITraitCollection FromUserInterfaceIdiom (UIUserInterfaceIdiom idiom);

		[Static, Export ("traitCollectionWithDisplayScale:")]
		UITraitCollection FromDisplayScale (nfloat scale);

		[Static, Export ("traitCollectionWithHorizontalSizeClass:")]
		UITraitCollection FromHorizontalSizeClass (UIUserInterfaceSizeClass horizontalSizeClass);

		[Static, Export ("traitCollectionWithVerticalSizeClass:")]
		UITraitCollection FromVerticalSizeClass (UIUserInterfaceSizeClass verticalSizeClass);

		[MacCatalyst (13, 1)]
		[Static, Export ("traitCollectionWithForceTouchCapability:")]
		UITraitCollection FromForceTouchCapability (UIForceTouchCapability capability);

		[NoWatch, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithUserInterfaceStyle:")]
		UITraitCollection FromUserInterfaceStyle (UIUserInterfaceStyle userInterfaceStyle);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithDisplayGamut:")]
		UITraitCollection FromDisplayGamut (UIDisplayGamut displayGamut);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithLayoutDirection:")]
		UITraitCollection FromLayoutDirection (UITraitEnvironmentLayoutDirection layoutDirection);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithPreferredContentSizeCategory:")]
		[Internal]
		UITraitCollection FromPreferredContentSizeCategory (NSString preferredContentSizeCategory);

		[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithSceneCaptureState:")]
		UITraitCollection FromSceneCaptureState (UISceneCaptureState sceneCaptureState);

		[MacCatalyst (13, 1)]
		[Export ("forceTouchCapability")]
		UIForceTouchCapability ForceTouchCapability { get; }

		[MacCatalyst (13, 1)]
		[Export ("displayGamut")]
		UIDisplayGamut DisplayGamut { get; }

		[MacCatalyst (13, 1)]
		[Export ("preferredContentSizeCategory")]
		string PreferredContentSizeCategory { get; }

		[MacCatalyst (13, 1)]
		[Export ("layoutDirection")]
		UITraitEnvironmentLayoutDirection LayoutDirection { get; }

		[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("sceneCaptureState")]
		UISceneCaptureState SceneCaptureState { get; }

		// This class has other members using From*
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithAccessibilityContrast:")]
		UITraitCollection FromAccessibilityContrast (UIAccessibilityContrast accessibilityContrast);

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityContrast")]
		UIAccessibilityContrast AccessibilityContrast { get; }

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithUserInterfaceLevel:")]
		UITraitCollection FromUserInterfaceLevel (UIUserInterfaceLevel userInterfaceLevel);

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("userInterfaceLevel")]
		UIUserInterfaceLevel UserInterfaceLevel { get; }

		[NoWatch, TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("traitCollectionWithLegibilityWeight:")]
		UITraitCollection FromLegibilityWeight (UILegibilityWeight legibilityWeight);

		[NoWatch, TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("legibilityWeight")]
		UILegibilityWeight LegibilityWeight { get; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("traitCollectionWithActiveAppearance:")]
		UITraitCollection FromActiveAppearance (UIUserInterfaceActiveAppearance userInterfaceActiveAppearance); // We have other From* methods

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activeAppearance")]
		UIUserInterfaceActiveAppearance ActiveAppearance { get; }

		// From UITraitCollection (CurrentTraitCollection)

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentTraitCollection", ArgumentSemantic.Strong)]
		UITraitCollection CurrentTraitCollection { get; set; }

		[ThreadSafe]
		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("performAsCurrentTraitCollection:")]
		void PerformAsCurrentTraitCollection (Action actions);

		// From UITraitCollection (CurrentTraitCollection)

		[TV (13, 0), NoWatch, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("hasDifferentColorAppearanceComparedToTraitCollection:")]
		bool HasDifferentColorAppearanceComparedTo ([NullAllowed] UITraitCollection traitCollection);

		// From UITraitCollection (ImageConfiguration)

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("imageConfiguration", ArgumentSemantic.Strong)]
		UIImageConfiguration ImageConfiguration { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("traitCollectionWithToolbarItemPresentationSize:")]
		UITraitCollection GetTraitCollection (UINSToolbarItemPresentationSize toolbarItemPresentationSize);

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("toolbarItemPresentationSize")]
		UINSToolbarItemPresentationSize ToolbarItemPresentationSize { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithTraits:")]
		UITraitCollection GetTraitCollectionWithTraits (Func<IUIMutableTraits> mutations);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitCollectionByModifyingTraits:")]
		UITraitCollection GetTraitCollectionByModifyingTraits (Func<IUIMutableTraits> mutations);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithCGFloatValue:forTrait:")]
		UITraitCollection GetTraitCollectionWithValue (nfloat value, IUICGFloatTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitCollectionByReplacingCGFloatValue:forTrait:")]
		UITraitCollection GetTraitCollectionByReplacingValue (nfloat value, IUICGFloatTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("valueForCGFloatTrait:")]
		nfloat GetValueForTrait (IUICGFloatTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithNSIntegerValue:forTrait:")]
		UITraitCollection GetTraitCollectionWithValue (nint value, IUINSIntegerTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitCollectionByReplacingNSIntegerValue:forTrait:")]
		UITraitCollection TraitCollectionByReplacingValue (nint value, IUINSIntegerTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("valueForNSIntegerTrait:")]
		nint GetValueForTrait (IUINSIntegerTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithObject:forTrait:")]
		UITraitCollection GetTraitCollectionWithObject ([NullAllowed] NSObject @object, IUIObjectTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitCollectionByReplacingObject:forTrait:")]
		UITraitCollection TraitCollectionByReplacingObject ([NullAllowed] NSObject @object, IUIObjectTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("objectForTrait:")]
		[return: NullAllowed]
		NSObject GetObject (IUIObjectTraitDefinition trait);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("changedTraitsFromTraitCollection:")]
		NSSet<IUITraitDefinition> GetChangedTraits ([NullAllowed] UITraitCollection traitCollection);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("systemTraitsAffectingColorAppearance")]
		IUITraitDefinition [] SystemTraitsAffectingColorAppearance { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("systemTraitsAffectingImageLookup")]
		IUITraitDefinition [] SystemTraitsAffectingImageLookup { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("typesettingLanguage")]
		string TypesettingLanguage { get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithTypesettingLanguage:")]
		UITraitCollection GetTraitCollection (string language);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("traitCollectionWithImageDynamicRange:")]
		UITraitCollection GetTraitCollection (UIImageDynamicRange imageDynamicRange);

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("imageDynamicRange")]
		UIImageDynamicRange ImageDynamicRange { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Static]
	partial interface UITransitionContext {
		[Field ("UITransitionContextFromViewControllerKey")]
		NSString FromViewControllerKey { get; }

		[Field ("UITransitionContextToViewControllerKey")]
		NSString ToViewControllerKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("UITransitionContextFromViewKey")]
		NSString FromViewKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("UITransitionContextToViewKey")]
		NSString ToViewKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	partial interface UIViewControllerAnimatedTransitioning {
		[Abstract]
		[Export ("transitionDuration:")]
		double TransitionDuration (IUIViewControllerContextTransitioning transitionContext);

		[Abstract]
		[Export ("animateTransition:")]
		void AnimateTransition (IUIViewControllerContextTransitioning transitionContext);

		[MacCatalyst (13, 1)]
		[Export ("interruptibleAnimatorForTransition:")]
		IUIViewImplicitlyAnimating GetInterruptibleAnimator (IUIViewControllerContextTransitioning transitionContext);

		[Export ("animationEnded:")]
		void AnimationEnded (bool transitionCompleted);
	}
	interface IUIViewControllerAnimatedTransitioning { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	partial interface UIViewControllerInteractiveTransitioning {
		[Abstract]
		[Export ("startInteractiveTransition:")]
		void StartInteractiveTransition (IUIViewControllerContextTransitioning transitionContext);

		[Export ("completionSpeed")]
		nfloat CompletionSpeed { get; }

		[Export ("completionCurve")]
		UIViewAnimationCurve CompletionCurve { get; }

		[MacCatalyst (13, 1)]
		[Export ("wantsInteractiveStart")]
		bool WantsInteractiveStart { get; }
	}
	interface IUIViewControllerInteractiveTransitioning { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	partial interface UIViewControllerTransitioningDelegate {
		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source);

		[Export ("animationControllerForDismissedController:")]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed);

		[Export ("interactionControllerForPresentation:")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation (IUIViewControllerAnimatedTransitioning animator);

		[Export ("interactionControllerForDismissal:")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal (IUIViewControllerAnimatedTransitioning animator);

		[MacCatalyst (13, 1)]
		[Export ("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
		UIPresentationController GetPresentationControllerForPresentedViewController (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController, UIViewController sourceViewController);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UIPercentDrivenInteractiveTransition : UIViewControllerInteractiveTransitioning {
		[Export ("duration")]
		nfloat Duration { get; }

		[Export ("percentComplete")]
		nfloat PercentComplete { get; }

		[Export ("completionSpeed", ArgumentSemantic.Assign)]
		new nfloat CompletionSpeed { get; set; }

		[Export ("completionCurve", ArgumentSemantic.Assign)]
		new UIViewAnimationCurve CompletionCurve { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("timingCurve", ArgumentSemantic.Strong)]
		IUITimingCurveProvider TimingCurve { get; set; }

		// getter comes from UIViewControllerInteractiveTransitioning but
		// headers declares a setter here
		[MacCatalyst (13, 1)]
		[Export ("wantsInteractiveStart")]
		new bool WantsInteractiveStart { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("pauseInteractiveTransition")]
		void PauseInteractiveTransition ();

		[Export ("updateInteractiveTransition:")]
		void UpdateInteractiveTransition (nfloat percentComplete);

		[Export ("cancelInteractiveTransition")]
		void CancelInteractiveTransition ();

		[Export ("finishInteractiveTransition")]
		void FinishInteractiveTransition ();
	}

	//
	// This protocol is only for consumption (there is no API to set a transition coordinator context,
	// you'll be provided an existing one), so we do not provide a model to subclass.
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	partial interface UIViewControllerTransitionCoordinatorContext {
		[Abstract]
		[Export ("isAnimated")]
		bool IsAnimated { get; }

		[Abstract]
		[Export ("presentationStyle")]
		UIModalPresentationStyle PresentationStyle { get; }

		[Abstract]
		[Export ("initiallyInteractive")]
		bool InitiallyInteractive { get; }

		[Abstract]
		[Export ("isInteractive")]
		bool IsInteractive { get; }

		[Abstract]
		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Abstract]
		[Export ("transitionDuration")]
		double TransitionDuration { get; }

		[Abstract]
		[Export ("percentComplete")]
		nfloat PercentComplete { get; }

		[Abstract]
		[Export ("completionVelocity")]
		nfloat CompletionVelocity { get; }

		[Abstract]
		[Export ("completionCurve")]
		UIViewAnimationCurve CompletionCurve { get; }

		[Abstract]
		[Export ("viewControllerForKey:")]
		UIViewController GetViewControllerForKey (NSString uiTransitionKey);

		[Abstract]
		[Export ("containerView")]
		UIView ContainerView { get; }

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("targetTransform")]
		CGAffineTransform TargetTransform ();

		[Abstract]
		[MacCatalyst (13, 1)]
		[Export ("viewForKey:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		UIView GetTransitionViewControllerForKey (NSString key);

#if NET // This is abstract in headers but is a breaking change
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("isInterruptible")]
		bool IsInterruptible { get; }
	}
	interface IUIViewControllerTransitionCoordinatorContext { }

	//
	// This protocol is only for consumption (there is no API to set a transition coordinator,
	// only get an existing one), so we do not provide a model to subclass.
	//
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	partial interface UIViewControllerTransitionCoordinator : UIViewControllerTransitionCoordinatorContext {
		[Abstract]
		[Export ("animateAlongsideTransition:completion:")]
		bool AnimateAlongsideTransition (Action<IUIViewControllerTransitionCoordinatorContext> animate,
						 [NullAllowed] Action<IUIViewControllerTransitionCoordinatorContext> completion);

		[Abstract]
		[Export ("animateAlongsideTransitionInView:animation:completion:")]
		bool AnimateAlongsideTransitionInView (UIView view, Action<IUIViewControllerTransitionCoordinatorContext> animation, [NullAllowed] Action<IUIViewControllerTransitionCoordinatorContext> completion);

		[Abstract]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'NotifyWhenInteractionChanges' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'NotifyWhenInteractionChanges' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NotifyWhenInteractionChanges' instead.")]
		[Export ("notifyWhenInteractionEndsUsingBlock:")]
		void NotifyWhenInteractionEndsUsingBlock (Action<IUIViewControllerTransitionCoordinatorContext> handler);

#if NET // This is abstract in headers but is a breaking change
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("notifyWhenInteractionChangesUsingBlock:")]
		void NotifyWhenInteractionChanges (Action<IUIViewControllerTransitionCoordinatorContext> handler);
	}
	interface IUIViewControllerTransitionCoordinator { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category, BaseType (typeof (UIViewController))]
	partial interface TransitionCoordinator_UIViewController {
		[Export ("transitionCoordinator")]
		[return: NullAllowed]
		IUIViewControllerTransitionCoordinator GetTransitionCoordinator ();
	}

	[NoMacCatalyst, NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 12, 0, message: "No longer supported; please adopt 'WKWebView'.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "No longer supported; please adopt 'WKWebView'.")]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIWebViewDelegate) })]
	interface UIWebView : UIScrollViewDelegate {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIWebViewDelegate Delegate { get; set; }

		[Export ("loadRequest:")]
		void LoadRequest (NSUrlRequest r);

		[Export ("loadHTMLString:baseURL:")]
		void LoadHtmlString (string s, [NullAllowed] NSUrl baseUrl);

		[Export ("loadData:MIMEType:textEncodingName:baseURL:")]
		void LoadData (NSData data, string mimeType, string textEncodingName, NSUrl baseUrl);

		[Export ("request", ArgumentSemantic.Retain)]
		NSUrlRequest Request { get; }

		[Export ("reload")]
		void Reload ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Export ("goBack")]
		void GoBack ();

		[Export ("goForward")]
		void GoForward ();

		[Export ("canGoBack")]
		bool CanGoBack { get; }

		[Export ("canGoForward")]
		bool CanGoForward { get; }

		[Export ("isLoading")]
		bool IsLoading { get; }

		[Export ("stringByEvaluatingJavaScriptFromString:")]
		string EvaluateJavascript (string script);

		[Export ("scalesPageToFit")]
		bool ScalesPageToFit { get; set; }

		[Export ("dataDetectorTypes")]
		UIDataDetectorType DataDetectorTypes { get; set; }

		[Export ("allowsInlineMediaPlayback")]
		bool AllowsInlineMediaPlayback { get; set; }

		[Export ("mediaPlaybackRequiresUserAction")]
		bool MediaPlaybackRequiresUserAction { get; set; }

		[Export ("scrollView", ArgumentSemantic.Retain)]
		UIScrollView ScrollView { get; }

		[Export ("mediaPlaybackAllowsAirPlay")]
		bool MediaPlaybackAllowsAirPlay { get; set; }

		[Export ("suppressesIncrementalRendering")]
		bool SuppressesIncrementalRendering { get; set; }

		[Export ("keyboardDisplayRequiresUserAction")]
		bool KeyboardDisplayRequiresUserAction { get; set; }

		[Export ("paginationMode")]
		UIWebPaginationMode PaginationMode { get; set; }

		[Export ("paginationBreakingMode")]
		UIWebPaginationBreakingMode PaginationBreakingMode { get; set; }

		[Export ("pageLength")]
		nfloat PageLength { get; set; }

		[Export ("gapBetweenPages")]
		nfloat GapBetweenPages { get; set; }

		[Export ("pageCount")]
		nint PageCount { get; }

		[Export ("allowsPictureInPictureMediaPlayback")]
		bool AllowsPictureInPictureMediaPlayback { get; set; }

		[Export ("allowsLinkPreview")]
		bool AllowsLinkPreview { get; set; }
	}

	[NoMacCatalyst, NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 12, 0, message: "No longer supported; please adopt 'WKWebView' APIs.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "No longer supported; please adopt 'WKWebView' APIs.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIWebViewDelegate {
		[Export ("webView:shouldStartLoadWithRequest:navigationType:"), DelegateName ("UIWebLoaderControl"), DefaultValue ("true")]
		bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType);

		[Export ("webViewDidStartLoad:"), EventArgs ("UIWebView")]
		void LoadStarted (UIWebView webView);

		[Export ("webViewDidFinishLoad:"), EventArgs ("UIWebView"), EventName ("LoadFinished")]
		void LoadingFinished (UIWebView webView);

		[Export ("webView:didFailLoadWithError:"), EventArgs ("UIWebErrorArgs", false, true), EventName ("LoadError")]
		void LoadFailed (UIWebView webView, NSError error);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextChecker {
		[Export ("rangeOfMisspelledWordInString:range:startingAt:wrap:language:")]
		NSRange RangeOfMisspelledWordInString (string stringToCheck, NSRange range, nint startingOffset, bool wrapFlag, string language);

		[Export ("guessesForWordRange:inString:language:")]
		string [] GuessesForWordRange (NSRange range, string str, string language);

		[Export ("completionsForPartialWordRange:inString:language:")]
		string [] CompletionsForPartialWordRange (NSRange range, string str, string language);

		[Export ("ignoreWord:")]
		void IgnoreWord (string wordToIgnore);

		[NullAllowed] // by default this property is null
		[Export ("ignoredWords")]
		string [] IgnoredWords { get; set; }

		[Static]
		[Export ("learnWord:")]
		void LearnWord (string word);

		[Static]
		[Export ("unlearnWord:")]
		void UnlearnWord (string word);

		[Static]
		[Export ("hasLearnedWord:")]
		bool HasLearnedWord (string word);

		[Static]
		[Export ("availableLanguages")]
		string AvailableLangauges { get; }
	}

	[Static]
	[NoWatch]
	[MacCatalyst (13, 1)]
	interface UITextContentType {
		[Field ("UITextContentTypeName")]
		NSString Name { get; }

		[Field ("UITextContentTypeNamePrefix")]
		NSString NamePrefix { get; }

		[Field ("UITextContentTypeGivenName")]
		NSString GivenName { get; }

		[Field ("UITextContentTypeMiddleName")]
		NSString MiddleName { get; }

		[Field ("UITextContentTypeFamilyName")]
		NSString FamilyName { get; }

		[Field ("UITextContentTypeNameSuffix")]
		NSString NameSuffix { get; }

		[Field ("UITextContentTypeNickname")]
		NSString Nickname { get; }

		[Field ("UITextContentTypeJobTitle")]
		NSString JobTitle { get; }

		[Field ("UITextContentTypeOrganizationName")]
		NSString OrganizationName { get; }

		[Field ("UITextContentTypeLocation")]
		NSString Location { get; }

		[Field ("UITextContentTypeFullStreetAddress")]
		NSString FullStreetAddress { get; }

		[Field ("UITextContentTypeStreetAddressLine1")]
		NSString StreetAddressLine1 { get; }

		[Field ("UITextContentTypeStreetAddressLine2")]
		NSString StreetAddressLine2 { get; }

		[Field ("UITextContentTypeAddressCity")]
		NSString AddressCity { get; }

		[Field ("UITextContentTypeAddressState")]
		NSString AddressState { get; }

		[Field ("UITextContentTypeAddressCityAndState")]
		NSString AddressCityAndState { get; }

		[Field ("UITextContentTypeSublocality")]
		NSString Sublocality { get; }

		[Field ("UITextContentTypeCountryName")]
		NSString CountryName { get; }

		[Field ("UITextContentTypePostalCode")]
		NSString PostalCode { get; }

		[Field ("UITextContentTypeTelephoneNumber")]
		NSString TelephoneNumber { get; }

		[Field ("UITextContentTypeEmailAddress")]
		NSString EmailAddress { get; }

		[Field ("UITextContentTypeURL")]
		NSString Url { get; }

		[Field ("UITextContentTypeCreditCardNumber")]
		NSString CreditCardNumber { get; }

		[MacCatalyst (13, 1)]
		[Field ("UITextContentTypeUsername")]
		NSString Username { get; }

		[MacCatalyst (13, 1)]
		[Field ("UITextContentTypePassword")]
		NSString Password { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UITextContentTypeNewPassword")]
		NSString NewPassword { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UITextContentTypeOneTimeCode")]
		NSString OneTimeCode { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UITextContentTypeShipmentTrackingNumber")]
		NSString ShipmentTrackingNumber { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UITextContentTypeFlightNumber")]
		NSString FlightNumber { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UITextContentTypeDateTime")]
		NSString DateTime { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeBirthdate")]
		NSString Birthdate { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeBirthdateDay")]
		NSString BirthdateDay { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeBirthdateMonth")]
		NSString BirthdateMonth { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeBirthdateYear")]
		NSString BirthdateYear { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardSecurityCode")]
		NSString CreditCardSecurityCode { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardName")]
		NSString CreditCardName { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardGivenName")]
		NSString CreditCardGivenName { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardMiddleName")]
		NSString CreditCardMiddleName { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardFamilyName")]
		NSString CreditCardFamilyName { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardExpiration")]
		NSString CreditCardExpiration { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardExpirationMonth")]
		NSString CreditCardExpirationMonth { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardExpirationYear")]
		NSString CreditCardExpirationYear { get; }

		[iOS (17, 0)]
		[Field ("UITextContentTypeCreditCardType")]
		NSString CreditCardType { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UISplitViewControllerDelegate) })]
	interface UISplitViewController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UISplitViewControllerStyle style);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("style")]
		UISplitViewControllerStyle Style { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("showsSecondaryOnlyButton")]
		bool ShowsSecondaryOnlyButton { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredSplitBehavior", ArgumentSemantic.Assign)]
		UISplitViewControllerSplitBehavior PreferredSplitBehavior { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitBehavior")]
		UISplitViewControllerSplitBehavior SplitBehavior { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setViewController:forColumn:")]
		void SetViewController ([NullAllowed] UIViewController viewController, UISplitViewControllerColumn column);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("viewControllerForColumn:")]
		[return: NullAllowed]
		UIViewController GetViewController (UISplitViewControllerColumn column);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("hideColumn:")]
		void HideColumn (UISplitViewControllerColumn column);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("showColumn:")]
		void ShowColumn (UISplitViewControllerColumn column);

		[Export ("viewControllers", ArgumentSemantic.Copy)]
		[PostGet ("ChildViewControllers")]
		UIViewController [] ViewControllers { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISplitViewControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("presentsWithGesture")]
		bool PresentsWithGesture { get; set; }

		//
		// iOS 8
		//
		[MacCatalyst (13, 1)]
		[Export ("collapsed")]
		bool Collapsed { [Bind ("isCollapsed")] get; }

		[MacCatalyst (13, 1)]
		[Export ("preferredDisplayMode")]
		UISplitViewControllerDisplayMode PreferredDisplayMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("displayMode")]
		UISplitViewControllerDisplayMode DisplayMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("preferredPrimaryColumnWidthFraction", ArgumentSemantic.UnsafeUnretained)]
		nfloat PreferredPrimaryColumnWidthFraction { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredPrimaryColumnWidth")]
		nfloat PreferredPrimaryColumnWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("minimumPrimaryColumnWidth", ArgumentSemantic.UnsafeUnretained)]
		nfloat MinimumPrimaryColumnWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumPrimaryColumnWidth", ArgumentSemantic.UnsafeUnretained)]
		nfloat MaximumPrimaryColumnWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("primaryColumnWidth")]
		nfloat PrimaryColumnWidth { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredSupplementaryColumnWidthFraction")]
		nfloat PreferredSupplementaryColumnWidthFraction { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("preferredSupplementaryColumnWidth")]
		nfloat PreferredSupplementaryColumnWidth { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("minimumSupplementaryColumnWidth")]
		nfloat MinimumSupplementaryColumnWidth { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("maximumSupplementaryColumnWidth")]
		nfloat MaximumSupplementaryColumnWidth { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("supplementaryColumnWidth")]
		nfloat SupplementaryColumnWidth { get; }

		[MacCatalyst (13, 1)]
		[Export ("displayModeButtonItem")]
		UIBarButtonItem DisplayModeButtonItem { get; }

		[iOS (14, 5), TV (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("displayModeButtonVisibility", ArgumentSemantic.Assign)]
		UISplitViewControllerDisplayModeButtonVisibility DisplayModeButtonVisibility { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("showDetailViewController:sender:")]
		void ShowDetailViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[MacCatalyst (13, 1)]
		[Field ("UISplitViewControllerAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[MacCatalyst (13, 1)]
		[Export ("primaryEdge", ArgumentSemantic.Assign)]
		UISplitViewControllerPrimaryEdge PrimaryEdge { get; set; }

		[NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("primaryBackgroundStyle", ArgumentSemantic.Assign)]
		UISplitViewControllerBackgroundStyle PrimaryBackgroundStyle { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UISplitViewControllerDelegate {
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("splitViewControllerSupportedInterfaceOrientations:"), DelegateName ("Func<UISplitViewController,UIInterfaceOrientationMask>"), DefaultValue (UIInterfaceOrientationMask.All)]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UISplitViewController splitViewController);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("splitViewControllerPreferredInterfaceOrientationForPresentation:"), DelegateName ("Func<UISplitViewController,UIInterfaceOrientation>"), DefaultValue (UIInterfaceOrientation.Unknown)]
		UIInterfaceOrientation GetPreferredInterfaceOrientationForPresentation (UISplitViewController splitViewController);

		[NoTV]
		[Export ("splitViewController:popoverController:willPresentViewController:"), EventArgs ("UISplitViewPresent")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController' instead.")]
		void WillPresentViewController (UISplitViewController svc, UIPopoverController pc, UIViewController aViewController);

		[NoTV]
		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:"), EventArgs ("UISplitViewHide")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController' instead.")]
		void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc);

		[NoTV]
		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:"), EventArgs ("UISplitViewShow")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController' instead.")]
		void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button);

		[NoTV]
		[Export ("splitViewController:shouldHideViewController:inOrientation:"), DelegateName ("UISplitViewControllerHidePredicate"), DefaultValue (true)]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'UISearchController' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UISearchController' instead.")]
		bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation);

		[MacCatalyst (13, 1)]
		[Export ("splitViewController:willChangeToDisplayMode:"), EventArgs ("UISplitViewControllerDisplayMode")]
		void WillChangeDisplayMode (UISplitViewController svc, UISplitViewControllerDisplayMode displayMode);

		[MacCatalyst (13, 1)]
		[Export ("targetDisplayModeForActionInSplitViewController:"), DelegateName ("UISplitViewControllerFetchTargetForActionHandler"), DefaultValue (UISplitViewControllerDisplayMode.Automatic)]
		UISplitViewControllerDisplayMode GetTargetDisplayModeForAction (UISplitViewController svc);

		[MacCatalyst (13, 1)]
		[Export ("splitViewController:showViewController:sender:"), DelegateName ("UISplitViewControllerDisplayEvent"), DefaultValue (false)]
		bool EventShowViewController (UISplitViewController splitViewController, UIViewController vc, NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("splitViewController:showDetailViewController:sender:"), DelegateName ("UISplitViewControllerDisplayEvent"), DefaultValue (false)]
		bool EventShowDetailViewController (UISplitViewController splitViewController, UIViewController vc, NSObject sender);

		[MacCatalyst (13, 1)]
		[Export ("primaryViewControllerForCollapsingSplitViewController:"), DelegateName ("UISplitViewControllerGetViewController"), DefaultValue (null)]
		UIViewController GetPrimaryViewControllerForCollapsingSplitViewController (UISplitViewController splitViewController);

		[MacCatalyst (13, 1)]
		[Export ("primaryViewControllerForExpandingSplitViewController:"), DelegateName ("UISplitViewControllerGetViewController"), DefaultValue (null)]
		UIViewController GetPrimaryViewControllerForExpandingSplitViewController (UISplitViewController splitViewController);

		[MacCatalyst (13, 1)]
		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:"), DelegateName ("UISplitViewControllerCanCollapsePredicate"), DefaultValue (true)]
		bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController);

		[MacCatalyst (13, 1)]
		[Export ("splitViewController:separateSecondaryViewControllerFromPrimaryViewController:"), DelegateName ("UISplitViewControllerGetSecondaryViewController"), DefaultValue (null)]
		UIViewController SeparateSecondaryViewController (UISplitViewController splitViewController, UIViewController primaryViewController);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewController:topColumnForCollapsingToProposedTopColumn:"), DelegateName ("UISplitViewControllerGetTopColumnForCollapsing"), DefaultValueFromArgument ("proposedTopColumn")]
		UISplitViewControllerColumn GetTopColumnForCollapsing (UISplitViewController splitViewController, UISplitViewControllerColumn proposedTopColumn);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewController:displayModeForExpandingToProposedDisplayMode:"), DelegateName ("UISplitViewControllerGetDisplayModeForExpanding"), DefaultValueFromArgument ("proposedDisplayMode")]
		UISplitViewControllerDisplayMode GetDisplayModeForExpanding (UISplitViewController splitViewController, UISplitViewControllerDisplayMode proposedDisplayMode);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewControllerDidCollapse:"), EventArgs ("UISplitViewControllerDidExpandCollapse")]
		void DidCollapse (UISplitViewController splitViewController);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewControllerDidExpand:"), EventArgs ("UISplitViewControllerDidExpandCollapse")]
		void DidExpand (UISplitViewController splitViewController);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewController:willShowColumn:"), EventArgs ("UISplitViewControllerWillShowHideColumn")]
		void WillShowColumn (UISplitViewController splitViewController, UISplitViewControllerColumn column);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewController:willHideColumn:"), EventArgs ("UISplitViewControllerWillShowHideColumn")]
		void WillHideColumn (UISplitViewController splitViewController, UISplitViewControllerColumn column);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewControllerInteractivePresentationGestureWillBegin:")]
		void InteractivePresentationGestureWillBegin (UISplitViewController svc);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("splitViewControllerInteractivePresentationGestureDidEnd:")]
		void InteractivePresentationGestureDidEnd (UISplitViewController svc);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (UIViewController))]
	partial interface UISplitViewController_UIViewController {
		[MacCatalyst (13, 1)]
		[Export ("splitViewController", ArgumentSemantic.Retain)]
		[return: NullAllowed]
		UISplitViewController GetSplitViewController ();

		[MacCatalyst (13, 1)]
		[Export ("collapseSecondaryViewController:forSplitViewController:")]
		void CollapseSecondaryViewController (UIViewController secondaryViewController, UISplitViewController splitViewController);

		[MacCatalyst (13, 1)]
		[Export ("separateSecondaryViewControllerForSplitViewController:")]
		UIViewController SeparateSecondaryViewControllerForSplitViewController (UISplitViewController splitViewController);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIControl))]
	interface UIStepper {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")] get; set; }

		[Export ("autorepeat")]
		bool AutoRepeat { get; set; }

		[Export ("wraps")]
		bool Wraps { get; set; }

		[Export ("value")]
		double Value { get; set; }

		[Export ("minimumValue")]
		double MinimumValue { get; set; }

		[Export ("maximumValue")]
		double MaximumValue { get; set; }

		[Export ("stepValue")]
		double StepValue { get; set; }

		//
		// 6.0
		//

		[Appearance]
		[Export ("setBackgroundImage:forState:")]
		void SetBackgroundImage ([NullAllowed] UIImage image, UIControlState state);

		[Appearance]
		[Export ("backgroundImageForState:")]
		UIImage BackgroundImage (UIControlState state);

		[Appearance]
		[Export ("setDividerImage:forLeftSegmentState:rightSegmentState:")]
		void SetDividerImage ([NullAllowed] UIImage image, UIControlState leftState, UIControlState rightState);

		[Appearance]
		[Export ("dividerImageForLeftSegmentState:rightSegmentState:")]
		UIImage GetDividerImage (UIControlState leftState, UIControlState rightState);

		[Appearance]
		[Export ("setIncrementImage:forState:")]
		void SetIncrementImage ([NullAllowed] UIImage image, UIControlState state);

		[Appearance]
		[Export ("incrementImageForState:")]
		UIImage GetIncrementImage (UIControlState state);

		[Appearance]
		[Export ("setDecrementImage:forState:")]
		void SetDecrementImage ([NullAllowed] UIImage image, UIControlState state);

		[Appearance]
		[Export ("decrementImageForState:")]
		UIImage GetDecrementImage (UIControlState state);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	delegate UIViewController UIStoryboardViewControllerCreator (NSCoder coder);

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIStoryboard {
		[Static]
		[Export ("storyboardWithName:bundle:")]
		UIStoryboard FromName (string name, [NullAllowed] NSBundle storyboardBundleOrNil);

		[Export ("instantiateInitialViewController")]
		UIViewController InstantiateInitialViewController ();

		[Export ("instantiateViewControllerWithIdentifier:")]
		UIViewController InstantiateViewController (string identifier);

		[iOS (13, 0), TV (13, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("instantiateInitialViewControllerWithCreator:")]
		UIViewController InstantiateInitialViewController ([NullAllowed] UIStoryboardViewControllerCreator creator);

		[iOS (13, 0), TV (13, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("instantiateViewControllerWithIdentifier:creator:")]
		UIViewController InstantiateViewController (string identifier, [NullAllowed] UIStoryboardViewControllerCreator creator);
	}

	[NoWatch]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	[DisableDefaultCtor] // as it subclass UIStoryboardSegue we end up with the same error
	[BaseType (typeof (UIStoryboardSegue))]
	interface UIStoryboardPopoverSegue {
		[Export ("initWithIdentifier:source:destination:"), PostGet ("SourceViewController"), PostGet ("DestinationViewController")]
		NativeHandle Constructor ([NullAllowed] string identifier, UIViewController source, UIViewController destination);

		[Export ("popoverController", ArgumentSemantic.Retain)]
		UIPopoverController PopoverController { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIStoryboardSegue init]
	interface UIStoryboardSegue {
		[DesignatedInitializer]
		[Export ("initWithIdentifier:source:destination:"), PostGet ("SourceViewController"), PostGet ("DestinationViewController")]
		NativeHandle Constructor ([NullAllowed] string identifier, UIViewController source, UIViewController destination);

		[Export ("identifier")]
		[NullAllowed]
		string Identifier { get; }

		[Export ("sourceViewController")]
		UIViewController SourceViewController { get; }

		[Export ("destinationViewController")]
		UIViewController DestinationViewController { get; }

		[Export ("perform")]
		void Perform ();

		[Static]
		[Export ("segueWithIdentifier:source:destination:performHandler:")]
		UIStoryboardSegue Create ([NullAllowed] string identifier, UIViewController source, UIViewController destination, Action performHandler);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIStoryboardUnwindSegueSource {
		[Export ("sourceViewController")]
		UIViewController SourceViewController { get; }

		// ideally we would not expose a `Selector` but this is created by iOS (not user code) and it's a getter only property
		[Export ("unwindAction")]
		Selector UnwindAction { get; }

		[NullAllowed]
		[Export ("sender")]
		NSObject Sender { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIPopoverBackgroundViewMethods {
		// This method is required, but we don't generate the correct code for required static methods.
		// [Abstract]
		[Static, Export ("arrowHeight")]
		nfloat GetArrowHeight ();

		// This method is required, but we don't generate the correct code for required static methods.
		// [Abstract]
		[Static, Export ("arrowBase")]
		nfloat GetArrowBase ();

		// This method is required, but we don't generate the correct code for required static methods.
		// [Abstract]
		[Static, Export ("contentViewInsets")]
		UIEdgeInsets GetContentViewInsets ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIPopoverBackgroundView : UIPopoverBackgroundViewMethods {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("arrowOffset")]
		nfloat ArrowOffset { get; set; }

#pragma warning disable 618
		[Export ("arrowDirection")]
		UIPopoverArrowDirection ArrowDirection { get; set; }
#pragma warning restore 618

		[Deprecated (PlatformName.iOS, 13, 0, message: "Not supported anymore.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Not supported anymore.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Not supported anymore.")]
		[Static, Export ("wantsDefaultContentAppearance")]
		bool WantsDefaultContentAppearance { get; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIPopoverControllerDelegate) })]
	[DisableDefaultCtor] // bug #1786
	[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'UIViewController' with style of 'UIModalPresentationStyle.Popover' or UIPopoverPresentationController' instead.")]
	[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UIViewController' with style of 'UIModalPresentationStyle.Popover' or UIPopoverPresentationController' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIViewController' with style of 'UIModalPresentationStyle.Popover' or UIPopoverPresentationController' instead.")]
	interface UIPopoverController : UIAppearanceContainer {
		[Export ("initWithContentViewController:")]
		[PostGet ("ContentViewController")]
		NativeHandle Constructor (UIViewController viewController);

		[Export ("contentViewController", ArgumentSemantic.Retain)]
		UIViewController ContentViewController { get; set; }

		[Export ("setContentViewController:animated:")]
		[PostGet ("ContentViewController")]
		void SetContentViewController (UIViewController viewController, bool animated);

		[Export ("popoverContentSize")]
		CGSize PopoverContentSize { get; set; }

		[Export ("setPopoverContentSize:animated:")]
		void SetPopoverContentSize (CGSize size, bool animated);

		[Export ("passthroughViews", ArgumentSemantic.Copy)]
		UIView [] PassthroughViews { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPopoverControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("popoverVisible")]
		bool PopoverVisible { [Bind ("isPopoverVisible")] get; }

		[Export ("popoverArrowDirection")]
		UIPopoverArrowDirection PopoverArrowDirection { get; }

		[Export ("presentPopoverFromRect:inView:permittedArrowDirections:animated:")]
		void PresentFromRect (CGRect rect, UIView view, UIPopoverArrowDirection arrowDirections, bool animated);

		[Export ("presentPopoverFromBarButtonItem:permittedArrowDirections:animated:")]
		void PresentFromBarButtonItem (UIBarButtonItem item, UIPopoverArrowDirection arrowDirections, bool animated);

		[Export ("dismissPopoverAnimated:")]
		void Dismiss (bool animated);

		// @property (nonatomic, readwrite) UIEdgeInsets popoverLayoutMargins
		[Export ("popoverLayoutMargins")]
		UIEdgeInsets PopoverLayoutMargins { get; set; }

		// @property (nonatomic, readwrite, retain) Class popoverBackgroundViewClass
		// Class is not pretty so we'll expose it manually as a System.Type
		[Internal]
		[Export ("popoverBackgroundViewClass", ArgumentSemantic.Retain)]
		IntPtr PopoverBackgroundViewClass { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }
	}

	[NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.iOS, 9, 0)]
	[Deprecated (PlatformName.TvOS, 9, 0)]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1)]
	interface UIPopoverControllerDelegate {
		[Export ("popoverControllerDidDismissPopover:"), EventArgs ("UIPopoverController")]
		void DidDismiss (UIPopoverController popoverController);

		[Export ("popoverControllerShouldDismissPopover:"), DelegateName ("UIPopoverControllerCondition"), DefaultValue ("true")]
		bool ShouldDismiss (UIPopoverController popoverController);

		[Export ("popoverController:willRepositionPopoverToRect:inView:"), EventArgs ("UIPopoverControllerReposition")]
		void WillReposition (UIPopoverController popoverController, ref CGRect rect, ref UIView view);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPresentationController),
		Delegates = new string [] { "WeakDelegate" },
		Events = new Type [] { typeof (UIPopoverPresentationControllerDelegate) })]
	[DisableDefaultCtor] // NSGenericException Reason: -[UIPopoverController init] is not a valid initializer. You must call -[UIPopoverController initWithContentViewController:]
	partial interface UIPopoverPresentationController {
		// re-exposed from base class
		[Export ("initWithPresentedViewController:presentingViewController:")]
		NativeHandle Constructor (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController);

		[NullAllowed]
		[Export ("delegate", ArgumentSemantic.UnsafeUnretained)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		UIPopoverPresentationControllerDelegate Delegate { get; set; }

		[Export ("permittedArrowDirections", ArgumentSemantic.UnsafeUnretained)]
		UIPopoverArrowDirection PermittedArrowDirections { get; set; }

		[Export ("sourceView", ArgumentSemantic.Retain)]
		UIView SourceView { get; set; }

		[Export ("sourceRect", ArgumentSemantic.UnsafeUnretained)]
		CGRect SourceRect { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("canOverlapSourceViewRect")]
		bool CanOverlapSourceViewRect { get; set; }

		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the SourceItem property instead.")]
		[Deprecated (PlatformName.MacCatalyst, 16, 0, message: "Use the SourceItem property instead.")]
		[Export ("barButtonItem", ArgumentSemantic.Retain), NullAllowed]
		UIBarButtonItem BarButtonItem { get; set; }

		[Export ("arrowDirection")]
		UIPopoverArrowDirection ArrowDirection { get; }

		[Export ("passthroughViews", ArgumentSemantic.Copy)]
		UIView [] PassthroughViews { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy), NullAllowed]
		UIColor BackgroundColor { get; set; }

		[Export ("popoverLayoutMargins")]
		UIEdgeInsets PopoverLayoutMargins { get; set; }

		[Internal] // expose as Type
		[Export ("popoverBackgroundViewClass", ArgumentSemantic.Retain), NullAllowed]
		IntPtr /* Class */  PopoverBackgroundViewClass { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("adaptiveSheetPresentationController", ArgumentSemantic.Strong)]
		UISheetPresentationController AdaptiveSheetPresentationController { get; }

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("sourceItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		IUIPopoverPresentationControllerSourceItem SourceItem { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIAdaptivePresentationControllerDelegate {
		[IgnoredInDelegate]
		[Export ("adaptivePresentationStyleForPresentationController:")]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController);

		[Export ("presentationController:viewControllerForAdaptivePresentationStyle:"),
			DelegateName ("UIAdaptivePresentationWithStyleRequested"), DefaultValue (null)]
		UIViewController GetViewControllerForAdaptivePresentation (UIPresentationController controller, UIModalPresentationStyle style);

		[MacCatalyst (13, 1)]
		[Export ("adaptivePresentationStyleForPresentationController:traitCollection:"),
			DelegateName ("UIAdaptivePresentationStyleWithTraitsRequested"), DefaultValue (UIModalPresentationStyle.None)]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController controller, UITraitCollection traitCollection);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("presentationController:prepareAdaptivePresentationController:"),
			EventName ("PrepareAdaptive"), EventArgs ("UIPrepareAdaptivePresentationArgs")]
		void PrepareAdaptivePresentationController (UIPresentationController presentationController, UIPresentationController adaptivePresentationController);

		[MacCatalyst (13, 1)]
		[Export ("presentationController:willPresentWithAdaptiveStyle:transitionCoordinator:"),
			EventName ("WillPresentController"), EventArgs ("UIWillPresentAdaptiveStyle")]
		void WillPresent (UIPresentationController presentationController, UIModalPresentationStyle style, [NullAllowed] IUIViewControllerTransitionCoordinator transitionCoordinator);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("presentationControllerShouldDismiss:"),
			DelegateName ("UIAdaptivePresentationShouldDismiss"), DefaultValue (true)]
		bool ShouldDismiss (UIPresentationController presentationController);

		[iOS (13, 0),
			EventName ("WillDismissController"), EventArgs ("UIAdaptivePresentationArgs")]
		[MacCatalyst (13, 1)]
		[Export ("presentationControllerWillDismiss:")]
		void WillDismiss (UIPresentationController presentationController);

		[iOS (13, 0),
			EventName ("DidDismissController"), EventArgs ("UIAdaptivePresentationArgs")]
		[MacCatalyst (13, 1)]
		[Export ("presentationControllerDidDismiss:")]
		void DidDismiss (UIPresentationController presentationController);

		[iOS (13, 0),
			EventName ("DidAttemptToDismissController"), EventArgs ("UIAdaptivePresentationArgs")]
		[MacCatalyst (13, 1)]
		[Export ("presentationControllerDidAttemptToDismiss:")]
		void DidAttemptToDismiss (UIPresentationController presentationController);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (UIAdaptivePresentationControllerDelegate))]
	partial interface UIPopoverPresentationControllerDelegate {
		[Export ("prepareForPopoverPresentation:"), EventName ("PrepareForPresentation")]
		void PrepareForPopoverPresentation (UIPopoverPresentationController popoverPresentationController);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'ShouldDismiss'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'ShouldDismiss'.")]
		[Export ("popoverPresentationControllerShouldDismissPopover:"), DelegateName ("ShouldDismiss"), DefaultValue (true)]
		bool ShouldDismissPopover (UIPopoverPresentationController popoverPresentationController);

		[Deprecated (PlatformName.iOS, 13, 0, message: "Replaced by 'DidDismiss'.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Replaced by 'DidDismiss'.")]
		[Export ("popoverPresentationControllerDidDismissPopover:"), EventName ("DidDismiss")]
		void DidDismissPopover (UIPopoverPresentationController popoverPresentationController);

		[Export ("popoverPresentationController:willRepositionPopoverToRect:inView:"),
			EventName ("WillReposition"), EventArgs ("UIPopoverPresentationControllerReposition")]
		void WillRepositionPopover (UIPopoverPresentationController popoverPresentationController, ref CGRect targetRect, ref UIView inView);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIScreenMode {
		[Export ("pixelAspectRatio")]
		nfloat PixelAspectRatio { get; }

		[Export ("size")]
		CGSize Size { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextInputMode : NSSecureCoding {
		[Export ("currentInputMode"), NullAllowed]
		[Static]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		UITextInputMode CurrentInputMode { get; }

		[Export ("primaryLanguage", ArgumentSemantic.Retain)]
		string PrimaryLanguage { get; }

		[Field ("UITextInputCurrentInputModeDidChangeNotification")]
		[Notification]
		NSString CurrentInputModeDidChangeNotification { get; }

		[Static]
		[Export ("activeInputModes")]
		UITextInputMode [] ActiveInputModes { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: -[UIPrinter init] not allowed
	partial interface UIPrinter {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("displayLocation")]
		string DisplayLocation { get; }

		[Export ("supportedJobTypes")]
		UIPrinterJobTypes SupportedJobTypes { get; }

		[Export ("makeAndModel")]
		string MakeAndModel { get; }

		[Export ("supportsColor")]
		bool SupportsColor { get; }

		[Export ("supportsDuplex")]
		bool SupportsDuplex { get; }

		[Static, Export ("printerWithURL:")]
		UIPrinter FromUrl (NSUrl url);

		[Export ("contactPrinter:")]
		[Async]
		void ContactPrinter (UIPrinterContactPrinterHandler completionHandler);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: -[UIPrinterPickerController init] not allowed
	partial interface UIPrinterPickerController {
		[Export ("selectedPrinter")]
		UIPrinter SelectedPrinter { get; }

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPrinterPickerControllerDelegate Delegate { get; set; }

		[Static, Export ("printerPickerControllerWithInitiallySelectedPrinter:")]
		UIPrinterPickerController FromPrinter ([NullAllowed] UIPrinter printer);

		[Async (ResultTypeName = "UIPrinterPickerCompletionResult")]
		[Export ("presentAnimated:completionHandler:")]
		bool Present (bool animated, [NullAllowed] UIPrinterPickerCompletionHandler completion);

		[Async (ResultTypeName = "UIPrinterPickerCompletionResult")]
		[Export ("presentFromRect:inView:animated:completionHandler:")]
		bool PresentFromRect (CGRect rect, UIView view, bool animated, [NullAllowed] UIPrinterPickerCompletionHandler completion);

		[Async (ResultTypeName = "UIPrinterPickerCompletionResult")]
		[Export ("presentFromBarButtonItem:animated:completionHandler:")]
		bool PresentFromBarButtonItem (UIBarButtonItem item, bool animated, [NullAllowed] UIPrinterPickerCompletionHandler completion);

		[Export ("dismissAnimated:")]
		void Dismiss (bool animated);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIPrinterPickerControllerDelegate {

		[Export ("printerPickerControllerParentViewController:")]
		UIViewController GetParentViewController (UIPrinterPickerController printerPickerController);

		[Export ("printerPickerController:shouldShowPrinter:")]
		bool ShouldShowPrinter (UIPrinterPickerController printerPickerController, UIPrinter printer);

		[Export ("printerPickerControllerWillPresent:")]
		void WillPresent (UIPrinterPickerController printerPickerController);

		[Export ("printerPickerControllerDidPresent:")]
		void DidPresent (UIPrinterPickerController printerPickerController);

		[Export ("printerPickerControllerWillDismiss:")]
		void WillDismiss (UIPrinterPickerController printerPickerController);

		[Export ("printerPickerControllerDidDismiss:")]
		void DidDismiss (UIPrinterPickerController printerPickerController);

		[Export ("printerPickerControllerDidSelectPrinter:")]
		void DidSelectPrinter (UIPrinterPickerController printerPickerController);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIPrintPaper {
		[Export ("bestPaperForPageSize:withPapersFromArray:")]
		[Static]
		UIPrintPaper ForPageSize (CGSize pageSize, UIPrintPaper [] paperList);

		[Export ("paperSize")]
		CGSize PaperSize { get; }

		[Export ("printableRect")]
		CGRect PrintableRect { get; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIPrintPageRenderer {
		[Export ("footerHeight")]
		nfloat FooterHeight { get; set; }

		[Export ("headerHeight")]
		nfloat HeaderHeight { get; set; }

		[Export ("paperRect")]
		CGRect PaperRect { get; }

		[Export ("printableRect")]
		CGRect PrintableRect { get; }

		[NullAllowed] // by default this property is null
		[Export ("printFormatters", ArgumentSemantic.Copy)]
		UIPrintFormatter [] PrintFormatters { get; set; }

		[Export ("addPrintFormatter:startingAtPageAtIndex:")]
		void AddPrintFormatter (UIPrintFormatter formatter, nint pageIndex);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("currentRenderingQualityForRequestedRenderingQuality:")]
		UIPrintRenderingQuality GetCurrentRenderingQuality (UIPrintRenderingQuality requestedRenderingQuality);

		[Export ("drawContentForPageAtIndex:inRect:")]
		void DrawContentForPage (nint index, CGRect contentRect);

		[Export ("drawFooterForPageAtIndex:inRect:")]
		void DrawFooterForPage (nint index, CGRect footerRect);

		[Export ("drawHeaderForPageAtIndex:inRect:")]
		void DrawHeaderForPage (nint index, CGRect headerRect);

		[Export ("drawPageAtIndex:inRect:")]
		void DrawPage (nint index, CGRect pageRect);

		[Export ("drawPrintFormatter:forPageAtIndex:")]
		void DrawPrintFormatterForPage (UIPrintFormatter printFormatter, nint index);

		[Export ("numberOfPages")]
		nint NumberOfPages { get; }

		[Export ("prepareForDrawingPages:")]
		void PrepareForDrawingPages (NSRange range);

		[Export ("printFormattersForPageAtIndex:")]
		UIPrintFormatter [] PrintFormattersForPage (nint index);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPrintInteractionControllerDelegate {
		[Export ("printInteractionControllerParentViewController:"), DefaultValue (null), DelegateName ("UIPrintInteraction")]
		UIViewController GetViewController (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionController:choosePaper:"), DefaultValue (null), DelegateName ("UIPrintInteractionPaperList")]
		UIPrintPaper ChoosePaper (UIPrintInteractionController printInteractionController, UIPrintPaper [] paperList);

		[Export ("printInteractionControllerWillPresentPrinterOptions:"), EventArgs ("UIPrintInteraction")]
		void WillPresentPrinterOptions (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionControllerDidPresentPrinterOptions:"), EventArgs ("UIPrintInteraction")]
		void DidPresentPrinterOptions (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionControllerWillDismissPrinterOptions:"), EventArgs ("UIPrintInteraction")]
		void WillDismissPrinterOptions (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionControllerDidDismissPrinterOptions:"), EventArgs ("UIPrintInteraction")]
		void DidDismissPrinterOptions (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionControllerWillStartJob:"), EventArgs ("UIPrintInteraction")]
		void WillStartJob (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionControllerDidFinishJob:"), EventArgs ("UIPrintInteraction")]
		void DidFinishJob (UIPrintInteractionController printInteractionController);

		[Export ("printInteractionController:cutLengthForPaper:")]
		[NoDefaultValue]
		[DelegateName ("Func<UIPrintInteractionController,UIPrintPaper,nfloat>")]
		nfloat CutLengthForPaper (UIPrintInteractionController printInteractionController, UIPrintPaper paper);

		[MacCatalyst (13, 1)]
		[Export ("printInteractionController:chooseCutterBehavior:"), DefaultValue ("UIPrinterCutterBehavior.NoCut"), DelegateName ("UIPrintInteractionCutterBehavior")]
		UIPrinterCutterBehavior ChooseCutterBehavior (UIPrintInteractionController printInteractionController, NSNumber [] availableBehaviors);
	}

	delegate void UIPrintInteractionCompletionHandler (UIPrintInteractionController printInteractionController, bool completed, NSError error);

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (UIPrintInteractionControllerDelegate) })]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[UIPrintInteractionController init] not allowed
	[DisableDefaultCtor]
	interface UIPrintInteractionController {
		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPrintInteractionControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("printFormatter", ArgumentSemantic.Retain)]
		UIPrintFormatter PrintFormatter { get; set; }

		[Export ("printInfo", ArgumentSemantic.Retain)]
		UIPrintInfo PrintInfo { get; set; }

		[Export ("printingItem", ArgumentSemantic.Copy)]
		NSObject PrintingItem { get; set; }

		[Export ("printingItems", ArgumentSemantic.Copy)]
		NSObject [] PrintingItems { get; set; }

		[Export ("printPageRenderer", ArgumentSemantic.Retain)]
		UIPrintPageRenderer PrintPageRenderer { get; set; }

		[Export ("printPaper")]
		UIPrintPaper PrintPaper { get; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Page range is now always shown.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Page range is now always shown.")]
		[Export ("showsPageRange")]
		bool ShowsPageRange { get; set; }

		[Export ("canPrintData:")]
		[Static]
		bool CanPrint (NSData data);

		[Export ("canPrintURL:")]
		[Static]
		bool CanPrint (NSUrl url);

		[Export ("printingAvailable")]
		[Static]
		bool PrintingAvailable { [Bind ("isPrintingAvailable")] get; }

		[Export ("printableUTIs")]
		[Static]
		NSSet PrintableUTIs { get; }

		[Export ("sharedPrintController")]
		[Static]
		UIPrintInteractionController SharedPrintController { get; }

		[Export ("dismissAnimated:")]
		void Dismiss (bool animated);

		[Export ("presentAnimated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		bool Present (bool animated, [NullAllowed] UIPrintInteractionCompletionHandler completion);

		[Export ("presentFromBarButtonItem:animated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		bool PresentFromBarButtonItem (UIBarButtonItem item, bool animated, [NullAllowed] UIPrintInteractionCompletionHandler completion);

		[Export ("presentFromRect:inView:animated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		bool PresentFromRectInView (CGRect rect, UIView view, bool animated, [NullAllowed] UIPrintInteractionCompletionHandler completion);

		[Export ("showsNumberOfCopies")]
		bool ShowsNumberOfCopies { get; set; }


		[MacCatalyst (13, 1)]
		[Export ("showsPaperSelectionForLoadedPapers")]
		bool ShowsPaperSelectionForLoadedPapers { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("showsPaperOrientation")]
		bool ShowsPaperOrientation { get; set; }

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "UIPrintInteractionCompletionResult")]
		[Export ("printToPrinter:completionHandler:")]
		bool PrintToPrinter (UIPrinter printer, UIPrintInteractionCompletionHandler completion);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[UIPrintInfo init] not allowed
	[DisableDefaultCtor]
	interface UIPrintInfo : NSCoding, NSCopying {
		[Export ("duplex")]
		UIPrintInfoDuplex Duplex { get; set; }

		[Export ("jobName", ArgumentSemantic.Copy)]
		string JobName { get; set; }

		[Export ("orientation")]
		UIPrintInfoOrientation Orientation { get; set; }

		[Export ("outputType")]
		UIPrintInfoOutputType OutputType { get; set; }

		[Export ("printerID", ArgumentSemantic.Copy)]
		string PrinterID { get; set; }

		[Export ("printInfo")]
		[Static]
		UIPrintInfo PrintInfo { get; }

		[Export ("printInfoWithDictionary:")]
		[Static]
		UIPrintInfo FromDictionary (NSDictionary dictionary);

		[Export ("dictionaryRepresentation")]
		NSDictionary ToDictionary { get; }
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	interface UIPrintServiceExtension {

		[Export ("printerDestinationsForPrintInfo:")]
		UIPrinterDestination [] GetPrinterDestinations (UIPrintInfo printInfo);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPrintFormatter))]
	interface UIViewPrintFormatter {
		[Export ("view")]
		UIView View { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface UIVisualEffect : NSCopying, NSSecureCoding {
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIVisualEffect))]
	partial interface UIBlurEffect {
		[Static, Export ("effectWithStyle:")]
		UIBlurEffect FromStyle (UIBlurEffectStyle style);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIVisualEffect))]
	partial interface UIVibrancyEffect {
		[Static, Export ("effectForBlurEffect:")]
		UIVibrancyEffect FromBlurEffect (UIBlurEffect blurEffect);

		// From interface UIVibrancyEffect

		[NoWatch, NoTV, iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("effectForBlurEffect:style:")]
		UIVibrancyEffect FromBlurEffect (UIBlurEffect blurEffect, UIVibrancyEffectStyle style);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	partial interface UIVisualEffectView : NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithEffect:")]
		NativeHandle Constructor ([NullAllowed] UIVisualEffect effect);

		[Export ("contentView", ArgumentSemantic.Retain)]
		UIView ContentView { get; }

		[NullAllowed]
		[Export ("effect", ArgumentSemantic.Copy)]
		UIVisualEffect Effect { get; set; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPrintFormatter))]
	// accessing the properties fails with 7.0GM if the default `init` is used to create the instance, e.g. 
	// [UISimpleTextPrintFormatter color]: unrecognized selector sent to instance 0x18bd70d0
	[DisableDefaultCtor]
	interface UISimpleTextPrintFormatter {
		[NullAllowed]
		[Export ("color", ArgumentSemantic.Retain)]
		UIColor Color { get; set; }

		[NullAllowed]
		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; set; }

		[NullAllowed]
		[Export ("text", ArgumentSemantic.Copy)]
		string Text { get; set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("initWithText:")]
		NativeHandle Constructor ([NullAllowed] string text);

		[Export ("initWithAttributedText:")]
		NativeHandle Constructor ([NullAllowed] NSAttributedString text);

		[NullAllowed]
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIPrintFormatter : NSCopying {

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'PerPageContentInsets' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PerPageContentInsets' instead.")]
		[Export ("contentInsets")]
		UIEdgeInsets ContentInsets { get; set; }

		[Export ("maximumContentHeight")]
		nfloat MaximumContentHeight { get; set; }

		[Export ("maximumContentWidth")]
		nfloat MaximumContentWidth { get; set; }

		[Export ("pageCount")]
		nint PageCount { get; }

		[Export ("printPageRenderer", ArgumentSemantic.Assign)]
		UIPrintPageRenderer PrintPageRenderer { get; }

		[Export ("startPage")]
		nint StartPage { get; set; }

		[Export ("drawInRect:forPageAtIndex:")]
		void DrawRect (CGRect rect, nint pageIndex);

		[Export ("rectForPageAtIndex:")]
		CGRect RectangleForPage (nint pageIndex);

		[Export ("removeFromPrintPageRenderer")]
		void RemoveFromPrintPageRenderer ();

		[MacCatalyst (13, 1)]
		[Export ("perPageContentInsets")]
		UIEdgeInsets PerPageContentInsets { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("requiresMainThread")]
		bool RequiresMainThread { get; }
	}

	[MacCatalyst (14, 0)] // the headers lie, not usable until at least Mac Catalyst 14.0
	[NoTV, NoWatch]
	[BaseType (typeof (UIPrintFormatter))]
	[DisableDefaultCtor] // nonfunctional (and it doesn't show up in the header anyway)
	interface UIMarkupTextPrintFormatter {
		[NullAllowed] // by default this property is null
		[Export ("markupText", ArgumentSemantic.Copy)]
		string MarkupText { get; set; }

		[Export ("initWithMarkupText:")]
		NativeHandle Constructor ([NullAllowed] string text);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIMotionEffect : NSCoding, NSCopying {
		[Export ("keyPathsAndRelativeValuesForViewerOffset:")]
		NSDictionary ComputeKeyPathsAndRelativeValues (UIOffset viewerOffset);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIMotionEffect))]
	interface UIInterpolatingMotionEffect : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithKeyPath:type:")]
		NativeHandle Constructor (string keyPath, UIInterpolatingMotionEffectType type);

		[Export ("keyPath")]
		string KeyPath { get; }

		[Export ("type")]
		UIInterpolatingMotionEffectType Type { get; }

		[NullAllowed] // by default this property is null
		[Export ("minimumRelativeValue", ArgumentSemantic.Retain)]
		NSObject MinimumRelativeValue { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("maximumRelativeValue", ArgumentSemantic.Retain)]
		NSObject MaximumRelativeValue { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIMotionEffect))]
	interface UIMotionEffectGroup {
		[NullAllowed] // by default this property is null
		[Export ("motionEffects", ArgumentSemantic.Copy)]
		UIMotionEffect [] MotionEffects { get; set; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // designated
	interface UISpringTimingParameters : UITimingCurveProvider {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initialVelocity")]
		CGVector InitialVelocity { get; }

		[Export ("initWithDampingRatio:initialVelocity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nfloat ratio, CGVector velocity);

		[Export ("initWithMass:stiffness:damping:initialVelocity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (nfloat mass, nfloat stiffness, nfloat damping, CGVector velocity);

		[Export ("initWithDampingRatio:")]
		NativeHandle Constructor (nfloat ratio);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithDuration:bounce:initialVelocity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double duration, nfloat bounce, CGVector velocity);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("initWithDuration:bounce:")]
		NativeHandle Constructor (double duration, nfloat bounce);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Category, BaseType (typeof (NSString))]
	interface UIStringDrawing {
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGPoint, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGPoint, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:withFont:")]
		CGSize DrawString (CGPoint point, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:lineBreakMode:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:fontSize:lineBreakMode:baselineAdjustment:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:minFontSize:actualFontSize:lineBreakMode:baselineAdjustment:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:")]
		CGSize DrawString (CGRect rect, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:lineBreakMode:")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:lineBreakMode:alignment:")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.GetSizeUsingAttributes (UIStringAttributes)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.GetSizeUsingAttributes (UIStringAttributes)' instead.")]
		[Export ("sizeWithFont:")]
		CGSize StringSize (UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:forWidth:lineBreakMode:")]
		CGSize StringSize (UIFont font, nfloat forWidth, UILineBreakMode breakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:constrainedToSize:")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:constrainedToSize:lineBreakMode:")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		// Wait for guidance here: https://devforums.apple.com/thread/203655
		//[Obsolete ("Deprecated on iOS7.   No guidance.")]
		[Export ("sizeWithFont:minFontSize:actualFontSize:forWidth:lineBreakMode:")]
		CGSize StringSize (UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode);
	}

	[Category, BaseType (typeof (NSString))]
	interface NSStringDrawing {
		[Export ("sizeWithAttributes:")]
		CGSize WeakGetSizeUsingAttributes ([NullAllowed] NSDictionary attributes);

		[Wrap ("WeakGetSizeUsingAttributes (This, attributes.GetDictionary ())")]
		CGSize GetSizeUsingAttributes (UIStringAttributes attributes);

		[Export ("drawAtPoint:withAttributes:")]
		void WeakDrawString (CGPoint point, [NullAllowed] NSDictionary attributes);

		[Wrap ("WeakDrawString (This, point, attributes.GetDictionary ())")]
		void DrawString (CGPoint point, UIStringAttributes attributes);

		[Export ("drawInRect:withAttributes:")]
		void WeakDrawString (CGRect rect, [NullAllowed] NSDictionary attributes);

		[Wrap ("WeakDrawString (This, rect, attributes.GetDictionary ())")]
		void DrawString (CGRect rect, UIStringAttributes attributes);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIView))]
	interface UIInputView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:inputViewStyle:")]
		NativeHandle Constructor (CGRect frame, UIInputViewStyle inputViewStyle);

		[Export ("inputViewStyle")]
		UIInputViewStyle InputViewStyle { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsSelfSizing")]
		bool AllowsSelfSizing { get; set; }
	}

	interface IUITextInputDelegate { }

	interface IUITextDocumentProxy { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	partial interface UIInputViewController : UITextInputDelegate {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("inputView", ArgumentSemantic.Retain), NullAllowed]
		UIInputView InputView { get; set; }

		[Export ("textDocumentProxy"), NullAllowed]
		IUITextDocumentProxy TextDocumentProxy { get; }

		[Export ("dismissKeyboard")]
		void DismissKeyboard ();

		[Export ("advanceToNextInputMode")]
		void AdvanceToNextInputMode ();

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("requestSupplementaryLexiconWithCompletion:")]
		[Async]
		void RequestSupplementaryLexicon (Action<UILexicon> completionHandler);

		[NullAllowed] // by default this property is null
		[Export ("primaryLanguage")]
		string PrimaryLanguage { get; set; }

		[iOS (11, 1), TV (11, 1)]
		[MacCatalyst (13, 1)]
		[Export ("hasDictationKey")]
		bool HasDictationKey { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("handleInputModeListFromView:withEvent:")]
		void HandleInputModeList (UIView fromView, UIEvent withEvent);

		[MacCatalyst (13, 1)]
		[Export ("hasFullAccess")]
		bool HasFullAccess { get; }

		[MacCatalyst (13, 1)]
		[Export ("needsInputModeSwitchKey")]
		bool NeedsInputModeSwitchKey { get; }
	}

	[NoWatch, TV (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIInteraction {
		[Abstract]
		[Export ("view", ArgumentSemantic.Weak)]
		UIView View { get; }

		[Abstract]
		[Export ("willMoveToView:")]
		void WillMoveToView ([NullAllowed] UIView view);

		[Abstract]
		[Export ("didMoveToView:")]
		void DidMoveToView ([NullAllowed] UIView view);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UITextDocumentProxy : UIKeyInput {
		[Abstract]
		[Export ("documentContextBeforeInput")]
		string DocumentContextBeforeInput { get; }

		[Abstract]
		[Export ("documentContextAfterInput")]
		string DocumentContextAfterInput { get; }

		[Abstract]
		[Export ("adjustTextPositionByCharacterOffset:")]
		void AdjustTextPositionByCharacterOffset (nint offset);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract] // Adding required members is a breaking change
#endif
		[Export ("setMarkedText:selectedRange:")]
		void SetMarkedText (string markedText, NSRange selectedRange);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract] // Adding required members is a breaking change
#endif
		[Export ("unmarkText")]
		void UnmarkText ();

		// Another abstract that was introduced on this released, breaking ABI
		// Radar: 26867207
#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("documentInputMode")]
		UITextInputMode DocumentInputMode { get; }

		// New abstract, breaks ABI
		// Radar: 33685383
#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("selectedText")]
		string SelectedText { get; }

		// New abstract, breaks ABI
		// Radar: 33685383
#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("documentIdentifier", ArgumentSemantic.Copy)]
		NSUuid DocumentIdentifier { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UILayoutGuide : NSCoding
#if IOS
		, UIPopoverPresentationControllerSourceItem
#endif
	{
		[Export ("layoutFrame")]
		CGRect LayoutFrame { get; }

		[NullAllowed, Export ("owningView", ArgumentSemantic.Weak)]
		UIView OwningView { get; set; }

		[Export ("identifier")]
		string Identifier { get; set; }

		[Export ("leadingAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor LeadingAnchor { get; }

		[Export ("trailingAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor TrailingAnchor { get; }

		[Export ("leftAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor LeftAnchor { get; }

		[Export ("rightAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor RightAnchor { get; }

		[Export ("topAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor TopAnchor { get; }

		[Export ("bottomAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor BottomAnchor { get; }

		[Export ("widthAnchor", ArgumentSemantic.Strong)]
		NSLayoutDimension WidthAnchor { get; }

		[Export ("heightAnchor", ArgumentSemantic.Strong)]
		NSLayoutDimension HeightAnchor { get; }

		[Export ("centerXAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor CenterXAnchor { get; }

		[Export ("centerYAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor CenterYAnchor { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface UILayoutSupport {
		[Export ("length")]
		[Abstract]
		nfloat Length { get; }

		[MacCatalyst (13, 1)]
		[Export ("topAnchor", ArgumentSemantic.Strong)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutYAxisAnchor TopAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("bottomAnchor", ArgumentSemantic.Strong)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutYAxisAnchor BottomAnchor { get; }

		[MacCatalyst (13, 1)]
		[Export ("heightAnchor", ArgumentSemantic.Strong)]
#if NET
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutDimension HeightAnchor { get; }
	}

	interface IUILayoutSupport { }

	// This protocol is supposed to be an aggregate to existing classes,
	// at the moment there is no API that require a specific UIAccessibilityIdentification
	// implementation, so we don't provide a Model class (for now at least).
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIAccessibilityIdentification {
		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("accessibilityIdentifier", ArgumentSemantic.Copy)]
		string AccessibilityIdentifier { get; set; }
	}

	interface IUIAccessibilityIdentification { }

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationSettings' instead.")]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationSettings' instead.")]
	[BaseType (typeof (NSObject))]
	partial interface UIUserNotificationSettings : NSCoding, NSSecureCoding, NSCopying {

		[Export ("types")]
		UIUserNotificationType Types { get; }

		[Export ("categories", ArgumentSemantic.Copy)]
		NSSet Categories { get; }

		[Static, Export ("settingsForTypes:categories:")]
		UIUserNotificationSettings GetSettingsForTypes (UIUserNotificationType types, [NullAllowed] NSSet categories);
	}

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationCategory' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationCategory' instead.")]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface UIUserNotificationCategory : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("actionsForContext:")]
		UIUserNotificationAction [] GetActionsForContext (UIUserNotificationActionContext context);
	}

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationCategory' instead.")]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationCategory' instead.")]
	[BaseType (typeof (UIUserNotificationCategory))]
	partial interface UIMutableUserNotificationCategory {

		[NullAllowed] // by default this property is null
		[Export ("identifier")]
		string Identifier { get; set; }

		[Export ("setActions:forContext:")]
		void SetActions (UIUserNotificationAction [] actions, UIUserNotificationActionContext context);
	}

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
#if WATCH
	[Static]
#else
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
#endif
	partial interface UIUserNotificationAction
#if !WATCH
		: NSCopying, NSMutableCopying, NSSecureCoding
#endif
		{

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("identifier")]
		string Identifier { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("title")]
		string Title { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("activationMode", ArgumentSemantic.Assign)]
		UIUserNotificationActivationMode ActivationMode { get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("authenticationRequired", ArgumentSemantic.Assign)]
		bool AuthenticationRequired { [Bind ("isAuthenticationRequired")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("destructive", ArgumentSemantic.Assign)]
		bool Destructive { [Bind ("isDestructive")] get; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("parameters", ArgumentSemantic.Copy)]
		NSDictionary Parameters { get; [NotImplemented] set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("behavior", ArgumentSemantic.Assign)]
		UIUserNotificationActionBehavior Behavior { get; [NotImplemented] set; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNTextInputNotificationAction.TextInputButtonTitle' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNTextInputNotificationAction.TextInputButtonTitle' instead.")]
		[Field ("UIUserNotificationTextInputActionButtonTitleKey")]
		NSString TextInputActionButtonTitleKey { get; }

#if !WATCH
		// note: defined twice, where watchOS is defined it says it's not in iOS, the other one (for iOS 9) says it's not in tvOS
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNTextInputNotificationResponse.UserText' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UNTextInputNotificationResponse.UserText' instead.")]
		[Field ("UIUserNotificationActionResponseTypedTextKey")]
		NSString ResponseTypedTextKey { get; }
#else
#if !NET // No longer present in watchOS 7.0
		// note: defined twice, where watchOS is defined it says it's not in iOS, the other one (for iOS 9) says it's not in tvOS
		[Field ("UIUserNotificationActionResponseTypedTextKey")]
		NSString ResponseTypedTextKey { get; }
#endif // !NET
#endif // !WATCH
	}

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
	[BaseType (typeof (UIUserNotificationAction))]
	partial interface UIMutableUserNotificationAction {

		[NullAllowed] // by default this property is null
		[Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("activationMode", ArgumentSemantic.Assign)]
		UIUserNotificationActivationMode ActivationMode { get; set; }

		[Export ("authenticationRequired", ArgumentSemantic.Assign)]
		bool AuthenticationRequired { [Bind ("isAuthenticationRequired")] get; set; }

		[Export ("destructive", ArgumentSemantic.Assign)]
		bool Destructive { [Bind ("isDestructive")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("behavior", ArgumentSemantic.Assign)]
		UIUserNotificationActionBehavior Behavior { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("parameters", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary Parameters { get; set; }
	}

	[NoWatch]
	[NoTV]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIDocumentPickerViewController' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIDocumentPickerViewController' instead.")]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (UIDocumentMenuDelegate) })]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You cannot initialize a UIDocumentMenuViewController except by the initWithDocumentTypes:inMode: and initWithURL:inMode: initializers.
	partial interface UIDocumentMenuViewController : NSCoding {

		[DesignatedInitializer]
		[Export ("initWithDocumentTypes:inMode:")]
		NativeHandle Constructor (string [] allowedUTIs, UIDocumentPickerMode mode);

		[DesignatedInitializer]
		[Export ("initWithURL:inMode:")]
		NativeHandle Constructor (NSUrl url, UIDocumentPickerMode mode);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDocumentMenuDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("addOptionWithTitle:image:order:handler:")]
		[Async]
		void AddOption (string title, [NullAllowed] UIImage image, UIDocumentMenuOrder order, Action completionHandler);
	}

	[NoWatch]
	[NoTV]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'UIDocumentPickerViewController' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UIDocumentPickerViewController' instead.")]
	partial interface UIDocumentMenuDelegate {
		[Abstract]
		[Export ("documentMenu:didPickDocumentPicker:"), EventArgs ("UIDocumentMenuDocumentPicked")]
		void DidPickDocumentPicker (UIDocumentMenuViewController documentMenu, UIDocumentPickerViewController documentPicker);

#if !NET
		[Abstract]
#endif
		[Export ("documentMenuWasCancelled:")]
		void WasCancelled (UIDocumentMenuViewController documentMenu);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (UIDocumentPickerDelegate) })]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You cannot initialize a UIDocumentPickerViewController except by the initWithDocumentTypes:inMode: and initWithURL:inMode: initializers
	partial interface UIDocumentPickerViewController : NSCoding {

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[DesignatedInitializer]
		[Export ("initWithDocumentTypes:inMode:")]
		NativeHandle Constructor (string [] allowedUTIs, UIDocumentPickerMode mode);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initForOpeningContentTypes:asCopy:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UTType [] contentTypes, bool asCopy);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initForOpeningContentTypes:")]
		NativeHandle Constructor (UTType [] contentTypes);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Advice ("Use 'UTType' constructor overloads.")]
		[DesignatedInitializer]
		[Export ("initWithURL:inMode:")]
		NativeHandle Constructor (NSUrl url, UIDocumentPickerMode mode);

		[Deprecated (PlatformName.iOS, 14, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("initWithURLs:inMode:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl [] urls, UIDocumentPickerMode mode);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initForExportingURLs:asCopy:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl [] urls, bool asCopy);

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initForExportingURLs:")]
		NativeHandle Constructor (NSUrl [] urls);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDocumentPickerDelegate Delegate { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("documentPickerMode", ArgumentSemantic.Assign)]
		UIDocumentPickerMode DocumentPickerMode { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("shouldShowFileExtensions")]
		bool ShouldShowFileExtensions { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("directoryURL", ArgumentSemantic.Copy)]
		NSUrl DirectoryUrl { get; set; }
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIDocumentPickerDelegate {
		[Deprecated (PlatformName.iOS, 11, 0, message: "Implement 'DidPickDocument (UIDocumentPickerViewController, NSUrl[])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Implement 'DidPickDocument (UIDocumentPickerViewController, NSUrl[])' instead.")]
#if !NET
		[Abstract]
#endif
		[Export ("documentPicker:didPickDocumentAtURL:"), EventArgs ("UIDocumentPicked")]
		void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url);

		[MacCatalyst (13, 1)]
		[Export ("documentPicker:didPickDocumentsAtURLs:"), EventArgs ("UIDocumentPickedAtUrls"), EventName ("DidPickDocumentAtUrls")]
		void DidPickDocument (UIDocumentPickerViewController controller, NSUrl [] urls);

		[Export ("documentPickerWasCancelled:")]
		void WasCancelled (UIDocumentPickerViewController controller);
	}

	[Deprecated (PlatformName.iOS, 14, 0, message: "Use enumeration based 'NSFileProviderExtension' instead.")]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use enumeration based 'NSFileProviderExtension' instead.")]
	[BaseType (typeof (UIViewController))]
	partial interface UIDocumentPickerExtensionViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("documentPickerMode", ArgumentSemantic.Assign)]
		UIDocumentPickerMode DocumentPickerMode { get; }

		[Export ("originalURL", ArgumentSemantic.Copy)]
		NSUrl OriginalUrl { get; }

		[Export ("validTypes", ArgumentSemantic.Copy)]
		string [] ValidTypes { get; }

		[Export ("providerIdentifier")]
		string ProviderIdentifier { get; }

		[Export ("documentStorageURL", ArgumentSemantic.Copy)]
		NSUrl DocumentStorageUrl { get; }

		[Export ("dismissGrantingAccessToURL:")]
		void DismissGrantingAccess ([NullAllowed] NSUrl url);

		[Export ("prepareForPresentationInMode:")]
		void PrepareForPresentation (UIDocumentPickerMode mode);
	}

	// note: used (internally, not exposed) by UITableView and UICollectionView for state restoration
	// user objects must adopt the protocol
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDataSourceModelAssociation {

		[Abstract]
		[Export ("modelIdentifierForElementAtIndexPath:inView:")]
		string GetModelIdentifier (NSIndexPath idx, UIView view);

		[Abstract]
		[Export ("indexPathForElementWithModelIdentifier:inView:")]
		NSIndexPath GetIndexPath (string identifier, UIView view);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIAccessibilityReadingContent {

		[Abstract]
		[Export ("accessibilityLineNumberForPoint:")]
		nint GetAccessibilityLineNumber (CGPoint point);

		[Abstract]
		[Export ("accessibilityContentForLineNumber:")]
		string GetAccessibilityContent (nint lineNumber);

		[Abstract]
		[Export ("accessibilityFrameForLineNumber:")]
		CGRect GetAccessibilityFrame (nint lineNumber);

		[Abstract]
		[Export ("accessibilityPageContent")]
		string GetAccessibilityPageContent ();

		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedContentForLineNumber:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedContent (nint lineNumber);

		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedPageContent")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedPageContent ();
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIGuidedAccessRestrictionDelegate {
		[Abstract]
		[Export ("guidedAccessRestrictionIdentifiers")]
		string [] GetGuidedAccessRestrictionIdentifiers { get; }

		[Abstract]
		[Export ("guidedAccessRestrictionWithIdentifier:didChangeState:")]
		[EventArgs ("UIGuidedAccessRestriction")]
		void GuidedAccessRestrictionChangedState (string restrictionIdentifier, UIGuidedAccessRestrictionState newRestrictionState);

		[Abstract]
		[Export ("textForGuidedAccessRestrictionWithIdentifier:")]
		string GetTextForGuidedAccessRestriction (string restrictionIdentifier);

		// Optional
		[Export ("detailTextForGuidedAccessRestrictionWithIdentifier:")]
		string GetDetailTextForGuidedAccessRestriction (string restrictionIdentifier);
	}

	[DisableDefaultCtor] // [Assert] -init is not a useful initializer for this class. Use one of the designated initializers instead
	[NoWatch]
	[NoWatch] // added in Xcode 7.1 / iOS 9.1 SDK
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIFocusUpdateContext))]
	interface UICollectionViewFocusUpdateContext {

		[Export ("previouslyFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath PreviouslyFocusedIndexPath { [return: NullAllowed] get; }

		[Export ("nextFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath NextFocusedIndexPath { [return: NullAllowed] get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UICubicTimingParameters : UITimingCurveProvider {
		[Export ("animationCurve")]
		UIViewAnimationCurve AnimationCurve { get; }

		[Export ("controlPoint1")]
		CGPoint ControlPoint1 { get; }

		[Export ("controlPoint2")]
		CGPoint ControlPoint2 { get; }

		[Export ("initWithAnimationCurve:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIViewAnimationCurve curve);

		[Export ("initWithControlPoint1:controlPoint2:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGPoint point1, CGPoint point2);
	}

	interface IUIFocusAnimationContext { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusAnimationContext {
		[Abstract]
		[Export ("duration")]
		double Duration { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIFocusAnimationCoordinator {
		[Export ("addCoordinatedAnimations:completion:")]
		[Async]
		void AddCoordinatedAnimations ([NullAllowed] Action animations, [NullAllowed] Action completion);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("addCoordinatedFocusingAnimations:completion:")]
		void AddCoordinatedFocusingAnimations ([NullAllowed] Action<IUIFocusAnimationContext> animations, [NullAllowed] Action completion);

		[Async]
		[MacCatalyst (13, 1)]
		[Export ("addCoordinatedUnfocusingAnimations:completion:")]
		void AddCoordinatedUnfocusingAnimations ([NullAllowed] Action<IUIFocusAnimationContext> animations, [NullAllowed] Action completion);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UILayoutGuide))]
	interface UIFocusGuide {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'PreferredFocusEnvironments' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'PreferredFocusEnvironments' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredFocusEnvironments' instead.")]
		[NullAllowed, Export ("preferredFocusedView", ArgumentSemantic.Weak)]
		UIView PreferredFocusedView { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("preferredFocusEnvironments", ArgumentSemantic.Copy), NullAllowed] // null_resettable
		IUIFocusEnvironment [] PreferredFocusEnvironments { get; set; }
	}

	[TV (12, 0), iOS (12, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIFocusMovementHint : NSCopying {
		[Export ("movementDirection")]
		CGVector MovementDirection { get; }

		[Export ("perspectiveTransform")]
		CATransform3D PerspectiveTransform { get; }

		[Export ("rotation")]
		CGVector Rotation { get; }

		[Export ("translation")]
		CGVector Translation { get; }

		[Export ("interactionTransform")]
		CATransform3D InteractionTransform { get; }
	}

	interface IUIFocusItem { }

	[NoWatch]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusItem : UIFocusEnvironment {
		[Abstract]
		[Export ("canBecomeFocused")]
		bool CanBecomeFocused { get; }

		// FIXME: declared as a @required, but this breaks compatibility
		// Radar: 41121416
		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("frame")]
		CGRect Frame { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("focusEffect", ArgumentSemantic.Copy)]
		UIFocusEffect FocusEffect { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("focusGroupPriority")]
		UIFocusGroupPriority FocusGroupPriority { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("isTransparentFocusItem")]
		bool IsTransparentFocusItem { get; }

		[TV (12, 0), iOS (12, 0), NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("didHintFocusMovement:")]
		void DidHintFocusMovement (UIFocusMovementHint hint);
	}

	[DisableDefaultCtor] // [Assert] -init is not a useful initializer for this class. Use one of the designated initializers instead
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIFocusUpdateContext {
		[NullAllowed, Export ("previouslyFocusedView", ArgumentSemantic.Weak)]
		UIView PreviouslyFocusedView { get; }

		[NullAllowed, Export ("nextFocusedView", ArgumentSemantic.Weak)]
		UIView NextFocusedView { get; }

		[Export ("focusHeading", ArgumentSemantic.Assign)]
		UIFocusHeading FocusHeading { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("previouslyFocusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem PreviouslyFocusedItem { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("nextFocusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem NextFocusedItem { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIFocusDidUpdateNotification")]
		NSString DidUpdateNotification { get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("UIFocusMovementDidFailNotification")]
		NSString MovementDidFailNotification { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIFocusUpdateContextKey")]
		NSString Key { get; }

		[MacCatalyst (13, 1)]
		[Field ("UIFocusUpdateAnimationCoordinatorKey")]
		NSString AnimationCoordinatorKey { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIFocusSystem {
		[Static]
		[Export ("environment:containsEnvironment:")]
		bool Contains (IUIFocusEnvironment environment, IUIFocusEnvironment otherEnvironment);

		[NoiOS]
		[NoMacCatalyst]
		[Static]
		[Export ("registerURL:forSoundIdentifier:")]
		void RegisterUrl (NSUrl soundFileUrl, NSString identifier);

		// The 2 values associated with the 'UIFocusSoundIdentifier' smart enum cannot be used.
		// See https://developer.apple.com/documentation/uikit/uifocussystem/2887479-register
		// Do not specify one of the UIKit sound identifiers (such as default); doing so will cause an immediate assertion failure and crash your app.

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("focusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem FocusedItem { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("focusSystemForEnvironment:")]
		[return: NullAllowed]
		UIFocusSystem Create (IUIFocusEnvironment environment);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requestFocusUpdateToEnvironment:")]
		void RequestFocusUpdate (IUIFocusEnvironment environment);

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("updateFocusIfNeeded")]
		void UpdateFocusIfNeeded ();
	}

	interface IUIFocusDebuggerOutput { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusDebuggerOutput { }

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIFocusDebugger {
		[Static]
		[Export ("help")]
		IUIFocusDebuggerOutput Help { get; }

		[Static]
		[Export ("status")]
		IUIFocusDebuggerOutput Status { get; }

		[Static]
		[Export ("checkFocusabilityForItem:")]
		IUIFocusDebuggerOutput CheckFocusability (IUIFocusItem item);

		[Static]
		[Export ("simulateFocusUpdateRequestFromEnvironment:")]
		IUIFocusDebuggerOutput SimulateFocusUpdateRequest (IUIFocusEnvironment environment);

		// Removed from headers in Xcode 14
		[Wrap ("true ? throw new InvalidOperationException (Constants.ApiRemovedGeneral) : false", IsVirtual = true)]
		[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("checkFocusGroupTreeForEnvironment:")]
		string CheckFocusGroupTree (IUIFocusEnvironment environment);

		[TV (16, 0), NoWatch, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("preferredFocusEnvironmentsForEnvironment:")]
		IUIFocusDebuggerOutput GetPreferredFocusEnvironments (IUIFocusEnvironment environment);

		[TV (16, 0), NoWatch, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("focusGroupsForEnvironment:")]
		IUIFocusDebuggerOutput GetFocusGroups (IUIFocusEnvironment environment);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIPress {
		[Export ("timestamp")]
		double /* NSTimeInterval */ Timestamp { get; }

		[Export ("phase")]
		UIPressPhase Phase { get; }

		[Export ("type")]
		UIPressType Type { get; }

		[NullAllowed, Export ("window", ArgumentSemantic.Strong)]
		UIWindow Window { get; }

		[NullAllowed, Export ("responder", ArgumentSemantic.Strong)]
		UIResponder Responder { get; }

		[NullAllowed, Export ("gestureRecognizers", ArgumentSemantic.Copy)]
		UIGestureRecognizer [] GestureRecognizers { get; }

		[Export ("force")]
		nfloat Force { get; }

		[Introduced (PlatformName.MacCatalyst, 13, 4)]
		[iOS (13, 4), TV (13, 4)]
		[NullAllowed, Export ("key")]
		UIKey Key { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIEvent))]
	interface UIPressesEvent {
		[Export ("allPresses")]
		NSSet<UIPress> AllPresses { get; }

		[Export ("pressesForGestureRecognizer:")]
		NSSet<UIPress> GetPresses (UIGestureRecognizer gesture);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (UIPreviewInteractionDelegate) })]
	[DisableDefaultCtor]
	interface UIPreviewInteraction {

		[Export ("initWithView:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView view);

		[NullAllowed, Export ("view", ArgumentSemantic.Weak)]
		UIView View { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUIPreviewInteractionDelegate Delegate { get; set; }

		[Export ("locationInCoordinateSpace:")]
		CGPoint GetLocationInCoordinateSpace ([NullAllowed] IUICoordinateSpace coordinateSpace);

		[Export ("cancelInteraction")]
		void CancelInteraction ();
	}

	interface IUIPreviewInteractionDelegate { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIPreviewInteractionDelegate {

		[Abstract]
		[Export ("previewInteraction:didUpdatePreviewTransition:ended:")]
		[EventArgs ("NSPreviewInteractionPreviewUpdate")]
		void DidUpdatePreviewTransition (UIPreviewInteraction previewInteraction, nfloat transitionProgress, bool ended);

		[Abstract]
		[Export ("previewInteractionDidCancel:")]
		void DidCancel (UIPreviewInteraction previewInteraction);

		[Export ("previewInteractionShouldBegin:")]
		[DelegateName ("Func<UIPreviewInteraction,bool>"), DefaultValue (true)]
		bool ShouldBegin (UIPreviewInteraction previewInteraction);

		[Export ("previewInteraction:didUpdateCommitTransition:ended:")]
		[EventArgs ("NSPreviewInteractionPreviewUpdate")]
		void DidUpdateCommit (UIPreviewInteraction previewInteraction, nfloat transitionProgress, bool ended);
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIFocusUpdateContext))]
	interface UITableViewFocusUpdateContext {

		[Export ("previouslyFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath PreviouslyFocusedIndexPath { [return: NullAllowed] get; }

		[Export ("nextFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath NextFocusedIndexPath { [return: NullAllowed] get; }
	}

	[NoWatch, NoiOS]
	[NoMacCatalyst]
	public enum UIFocusSoundIdentifier {

		[Field ("UIFocusSoundIdentifierNone")]
		None,

		[Field ("UIFocusSoundIdentifierDefault")]
		Default,
	}

	interface IUIFocusEnvironment { }

	[NoWatch]
	[NoMac]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusEnvironment {
#if !NET
		[Abstract]
#endif
		[NullAllowed, Export ("preferredFocusedView", ArgumentSemantic.Weak)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'PreferredFocusEnvironments' instead.")]
		[Deprecated (PlatformName.TvOS, 10, 0, message: "Use 'PreferredFocusEnvironments' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredFocusEnvironments' instead.")]
		UIView PreferredFocusedView { get; }

		[Abstract]
		[Export ("setNeedsFocusUpdate")]
		void SetNeedsFocusUpdate ();

		[Abstract]
		[Export ("updateFocusIfNeeded")]
		void UpdateFocusIfNeeded ();

		[Abstract]
		[Export ("shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UIFocusUpdateContext context);

		[Abstract]
		[Export ("didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UIFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		//
		// FIXME: declared as a @required, but this breaks compatibility
		// Radar: 26825293
		//
#if NET
		[Abstract]
#endif
		[MacCatalyst (13, 1)]
		[Export ("preferredFocusEnvironments", ArgumentSemantic.Copy)]
		IUIFocusEnvironment [] PreferredFocusEnvironments { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("soundIdentifierForFocusUpdateInContext:")]
		[return: NullAllowed]
		NSString GetSoundIdentifier (UIFocusUpdateContext context);

		// FIXME: declared as a @required, but this breaks compatibility
		// Radar: 41121293
		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("parentFocusEnvironment", ArgumentSemantic.Weak)]
		IUIFocusEnvironment ParentFocusEnvironment { get; }

		[TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[NullAllowed, Export ("focusItemContainer")]
		IUIFocusItemContainer FocusItemContainer { get; }

		[NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("focusGroupIdentifier")]
		string FocusGroupIdentifier { get; }
	}

	[TV (12, 0), iOS (12, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextInputPasswordRules : NSSecureCoding, NSCopying {
		[Export ("passwordRulesDescriptor")]
		string PasswordRulesDescriptor { get; }

		[Static]
		[Export ("passwordRulesWithDescriptor:")]
		UITextInputPasswordRules Create (string passwordRulesDescriptor);
	}

	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Static]
	[Internal]
	interface UITextAttributesConstants {
		[Field ("UITextAttributeFont")]
		NSString Font { get; }

		[Field ("UITextAttributeTextColor")]
		NSString TextColor { get; }

		[Field ("UITextAttributeTextShadowColor")]
		NSString TextShadowColor { get; }

		[Field ("UITextAttributeTextShadowOffset")]
		NSString TextShadowOffset { get; }
	}

	interface IUIInteraction { }
	interface IUIDropSession { }
	interface IUIDragDropSession { }
	interface IUIDragAnimating { }
	interface IUIDragSession { }
	interface IUIDragInteractionDelegate { }
	interface IUIDropInteractionDelegate { }
	interface IUICollectionViewDragDelegate { }
	interface IUICollectionViewDropDelegate { }
	interface IUICollectionViewDropCoordinator { }
	interface IUICollectionViewDropItem { }
	interface IUICollectionViewDropPlaceholderContext { }
	interface IUITableViewDragDelegate { }
	interface IUITableViewDropDelegate { }
	interface IUITableViewDropCoordinator { }
	interface IUITableViewDropItem { }
	interface IUITableViewDropPlaceholderContext { }
	interface IUITextDragDelegate { }
	interface IUITextDraggable { }
	interface IUITextDragRequest { }
	interface IUITextDroppable { }
	interface IUITextDropDelegate { }
	interface IUITextDropRequest { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDragAnimating {
		[Abstract]
		[Export ("addAnimations:")]
		void AddAnimations (Action animations);

		[Abstract]
		[Export ("addCompletion:")]
		void AddCompletion (Action<UIViewAnimatingPosition> completion);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDragDropSession {
		[Abstract]
		[Export ("items")]
		UIDragItem [] Items { get; }

		[Abstract]
		[Export ("locationInView:")]
		CGPoint LocationInView ([NullAllowed] UIView view);

		[Abstract]
		[Export ("allowsMoveOperation")]
		bool AllowsMoveOperation { get; }

		[Abstract]
		[Export ("restrictedToDraggingApplication")]
		bool RestrictedToDraggingApplication { [Bind ("isRestrictedToDraggingApplication")] get; }

		[Abstract]
		[Export ("hasItemsConformingToTypeIdentifiers:")]
		bool HasConformingItems (string [] typeIdentifiers);

		[Abstract]
		[Export ("canLoadObjectsOfClass:")]
		bool CanLoadObjects (Class itemProviderReadingClass);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDragItem {
		[Export ("initWithItemProvider:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSItemProvider itemProvider);

		[Export ("itemProvider")]
		NSItemProvider ItemProvider { get; }

		[NullAllowed, Export ("localObject", ArgumentSemantic.Strong)]
		NSObject LocalObject { get; set; }

		[NullAllowed, Export ("previewProvider", ArgumentSemantic.Copy)]
		Func<UIDragPreview> PreviewProvider { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDragPreview : NSCopying {
		[Export ("initWithView:parameters:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView view, UIDragPreviewParameters parameters);

		[Export ("initWithView:")]
		NativeHandle Constructor (UIView view);

		[Export ("view")]
		UIView View { get; }

		[Export ("parameters", ArgumentSemantic.Copy)]
		UIDragPreviewParameters Parameters { get; }

		// From URLPreviews (UIDragPreview) category

		[Static]
		[Export ("previewForURL:")]
		UIDragPreview GetPreview (NSUrl url);

		[Static]
		[Export ("previewForURL:title:")]
		UIDragPreview GetPreview (NSUrl url, [NullAllowed] string title);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPreviewParameters))]
	[DesignatedDefaultCtor]
	interface UIDragPreviewParameters : NSCopying {

		[Export ("initWithTextLineRects:")]
		NativeHandle Constructor (NSValue [] textLineRects);

		// Now they come from the base class

		// [NullAllowed, Export ("visiblePath", ArgumentSemantic.Copy)]
		// UIBezierPath VisiblePath { get; set; }

		// [Export ("backgroundColor", ArgumentSemantic.Copy)]
		// UIColor BackgroundColor { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPreviewTarget))]
	[DisableDefaultCtor]
	interface UIDragPreviewTarget : NSCopying {
		[Export ("initWithContainer:center:transform:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView container, CGPoint center, CGAffineTransform transform);

		[Export ("initWithContainer:center:")]
		NativeHandle Constructor (UIView container, CGPoint center);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDragSession : UIDragDropSession {
		[Abstract]
		[NullAllowed, Export ("localContext", ArgumentSemantic.Strong)]
		NSObject LocalContext { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDragInteraction : UIInteraction {
		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIDragInteractionDelegate @delegate);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUIDragInteractionDelegate Delegate { get; }

		[Export ("allowsSimultaneousRecognitionDuringLift")]
		bool AllowsSimultaneousRecognitionDuringLift { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Static]
		[Export ("enabledByDefault")]
		bool EnabledByDefault { [Bind ("isEnabledByDefault")] get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIDragInteractionDelegate {
		[Abstract]
		[Export ("dragInteraction:itemsForBeginningSession:")]
		UIDragItem [] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:previewForLiftingItem:session:")]
		[return: NullAllowed]
		UITargetedDragPreview GetPreviewForLiftingItem (UIDragInteraction interaction, UIDragItem item, IUIDragSession session);

		[Export ("dragInteraction:willAnimateLiftWithAnimator:session:")]
		void WillAnimateLift (UIDragInteraction interaction, IUIDragAnimating animator, IUIDragSession session);

		[Export ("dragInteraction:sessionWillBegin:")]
		void SessionWillBegin (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:sessionAllowsMoveOperation:")]
		bool SessionAllowsMoveOperation (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:sessionIsRestrictedToDraggingApplication:")]
		bool SessionIsRestrictedToDraggingApplication (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:prefersFullSizePreviewsForSession:")]
		bool PrefersFullSizePreviews (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:sessionDidMove:")]
		void SessionDidMove (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:session:willEndWithOperation:")]
		void SessionWillEnd (UIDragInteraction interaction, IUIDragSession session, UIDropOperation operation);

		[Export ("dragInteraction:session:didEndWithOperation:")]
		void SessionDidEnd (UIDragInteraction interaction, IUIDragSession session, UIDropOperation operation);

		[Export ("dragInteraction:sessionDidTransferItems:")]
		void SessionDidTransferItems (UIDragInteraction interaction, IUIDragSession session);

		[Export ("dragInteraction:itemsForAddingToSession:withTouchAtPoint:")]
		UIDragItem [] GetItemsForAddingToSession (UIDragInteraction interaction, IUIDragSession session, CGPoint point);

		[Export ("dragInteraction:sessionForAddingItems:withTouchAtPoint:")]
		[return: NullAllowed]
		IUIDragSession GetSessionForAddingItems (UIDragInteraction interaction, IUIDragSession [] sessions, CGPoint point);

		[Export ("dragInteraction:session:willAddItems:forInteraction:")]
		void WillAddItems (UIDragInteraction interaction, IUIDragSession session, UIDragItem [] items, UIDragInteraction addingInteraction);

		[Export ("dragInteraction:previewForCancellingItem:withDefault:")]
		[return: NullAllowed]
		UITargetedDragPreview GetPreviewForCancellingItem (UIDragInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview);

		[Export ("dragInteraction:item:willAnimateCancelWithAnimator:")]
		void WillAnimateCancel (UIDragInteraction interaction, UIDragItem item, IUIDragAnimating animator);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))] // If Apple adds a delegate setter: Delegates=new string [] {"Delegate"}, Events=new Type [] { typeof (UIDropInteractionDelegate)})]
	[DisableDefaultCtor]
	interface UIDropInteraction : UIInteraction {
		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIDropInteractionDelegate @delegate);

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IUIDropInteractionDelegate Delegate { get; }

		[Export ("allowsSimultaneousDropSessions")]
		bool AllowsSimultaneousDropSessions { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIDropInteractionDelegate {
		[Export ("dropInteraction:canHandleSession:"), DelegateName ("Func<UIDropInteraction,IUIDropSession,bool>"), NoDefaultValue]
		bool CanHandleSession (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:sessionDidEnter:"), EventArgs ("UIDropInteraction")]
		void SessionDidEnter (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:sessionDidUpdate:"), DelegateName ("Func<UIDropInteraction,IUIDropSession,UIDropProposal>"), NoDefaultValue]
		UIDropProposal SessionDidUpdate (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:sessionDidExit:"), EventArgs ("UIDropInteraction")]
		void SessionDidExit (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:performDrop:"), EventArgs ("UIDropInteraction")]
		void PerformDrop (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:concludeDrop:"), EventArgs ("UIDropInteraction")]
		void ConcludeDrop (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:sessionDidEnd:"), EventArgs ("UIDropInteraction")]
		void SessionDidEnd (UIDropInteraction interaction, IUIDropSession session);

		[Export ("dropInteraction:previewForDroppingItem:withDefault:")]
		[return: NullAllowed]
		[DelegateName ("UIDropInteractionPreviewForItem"), NoDefaultValue]
		UITargetedDragPreview GetPreviewForDroppingItem (UIDropInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview);

		[Export ("dropInteraction:item:willAnimateDropWithAnimator:"), EventArgs ("UIDropInteractionAnimation")]
		void WillAnimateDrop (UIDropInteraction interaction, UIDragItem item, IUIDragAnimating animator);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDropProposal : NSCopying {
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIDropOperation operation);

		[Export ("operation")]
		UIDropOperation Operation { get; }

		[Export ("precise")]
		bool Precise { [Bind ("isPrecise")] get; set; }

		[Export ("prefersFullSizePreview")]
		bool PrefersFullSizePreview { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDropSession : UIDragDropSession, NSProgressReporting {
		[Abstract]
		[NullAllowed, Export ("localDragSession")]
		IUIDragSession LocalDragSession { get; }

		[Abstract]
		[Export ("progressIndicatorStyle", ArgumentSemantic.Assign)]
		UIDropSessionProgressIndicatorStyle ProgressIndicatorStyle { get; set; }

		[Abstract]
		[Export ("loadObjectsOfClass:completion:")]
		NSProgress LoadObjects (Class itemProviderReadingClass, Action<INSItemProviderReading []> completion);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UITargetedPreview))]
	[DisableDefaultCtor]
	interface UITargetedDragPreview : NSCopying {
		[Export ("initWithView:parameters:target:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView view, UIDragPreviewParameters parameters, UIDragPreviewTarget target);

		[Export ("initWithView:parameters:")]
		NativeHandle Constructor (UIView view, UIDragPreviewParameters parameters);

		[Export ("initWithView:")]
		NativeHandle Constructor (UIView view);

		[Export ("target")]
		UIDragPreviewTarget Target { get; }

		[Export ("view")]
		UIView View { get; }

		[Export ("parameters", ArgumentSemantic.Copy)]
		UIDragPreviewParameters Parameters { get; }

		[Export ("size")]
		CGSize Size { get; }

		[Export ("retargetedPreviewWithTarget:")]
		UITargetedDragPreview GetRetargetedPreview (UIDragPreviewTarget newTarget);

		// From URLPreviews (UITargetedDragPreview) category

		[Static]
		[Export ("previewForURL:target:")]
		UITargetedDragPreview GetPreview (NSUrl url, UIDragPreviewTarget target);

		[Static]
		[Export ("previewForURL:title:target:")]
		UITargetedDragPreview GetPreview (NSUrl url, [NullAllowed] string title, UIDragPreviewTarget target);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewDragDelegate {
		[Abstract]
		[Export ("collectionView:itemsForBeginningDragSession:atIndexPath:")]
		UIDragItem [] GetItemsForBeginningDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath);

		[Export ("collectionView:itemsForAddingToDragSession:atIndexPath:point:")]
		UIDragItem [] GetItemsForAddingToDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath, CGPoint point);

		[Export ("collectionView:dragPreviewParametersForItemAtIndexPath:")]
		[return: NullAllowed]
		UIDragPreviewParameters GetDragPreviewParameters (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:dragSessionWillBegin:")]
		void DragSessionWillBegin (UICollectionView collectionView, IUIDragSession session);

		[Export ("collectionView:dragSessionDidEnd:")]
		void DragSessionDidEnd (UICollectionView collectionView, IUIDragSession session);

		[Export ("collectionView:dragSessionAllowsMoveOperation:")]
		bool DragSessionAllowsMoveOperation (UICollectionView collectionView, IUIDragSession session);

		[Export ("collectionView:dragSessionIsRestrictedToDraggingApplication:")]
		bool DragSessionIsRestrictedToDraggingApplication (UICollectionView collectionView, IUIDragSession session);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewDropDelegate {
		[Abstract]
		[Export ("collectionView:performDropWithCoordinator:")]
		void PerformDrop (UICollectionView collectionView, IUICollectionViewDropCoordinator coordinator);

		[Export ("collectionView:canHandleDropSession:")]
		bool CanHandleDropSession (UICollectionView collectionView, IUIDropSession session);

		[Export ("collectionView:dropSessionDidEnter:")]
		void DropSessionDidEnter (UICollectionView collectionView, IUIDropSession session);

		[Export ("collectionView:dropSessionDidUpdate:withDestinationIndexPath:")]
		UICollectionViewDropProposal DropSessionDidUpdate (UICollectionView collectionView, IUIDropSession session, [NullAllowed] NSIndexPath destinationIndexPath);

		[Export ("collectionView:dropSessionDidExit:")]
		void DropSessionDidExit (UICollectionView collectionView, IUIDropSession session);

		[Export ("collectionView:dropSessionDidEnd:")]
		void DropSessionDidEnd (UICollectionView collectionView, IUIDropSession session);

		[Export ("collectionView:dropPreviewParametersForItemAtIndexPath:")]
		[return: NullAllowed]
		UIDragPreviewParameters GetDropPreviewParameters (UICollectionView collectionView, NSIndexPath indexPath);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDropProposal))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Not implemented
	interface UICollectionViewDropProposal {

		// inline from base type
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIDropOperation operation);

		[Export ("initWithDropOperation:intent:")]
		NativeHandle Constructor (UIDropOperation operation, UICollectionViewDropIntent intent);

		[Export ("intent")]
		UICollectionViewDropIntent Intent { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UICollectionViewDropCoordinator {
		[Abstract]
		[Export ("items")]
		IUICollectionViewDropItem [] Items { get; }

		[Abstract]
		[NullAllowed, Export ("destinationIndexPath")]
		NSIndexPath DestinationIndexPath { get; }

		[Abstract]
		[Export ("proposal")]
		UICollectionViewDropProposal Proposal { get; }

		[Abstract]
		[Export ("session")]
		IUIDropSession Session { get; }

		[Abstract]
		[Export ("dropItem:toPlaceholder:")]
		IUICollectionViewDropPlaceholderContext DropItemToPlaceholder (UIDragItem dragItem, UICollectionViewDropPlaceholder placeholder);

		[Abstract]
		[Export ("dropItem:toItemAtIndexPath:")]
		IUIDragAnimating DropItemToItem (UIDragItem dragItem, NSIndexPath itemIndexPath);

		[Abstract]
		[Export ("dropItem:intoItemAtIndexPath:rect:")]
		IUIDragAnimating DropItemIntoItem (UIDragItem dragItem, NSIndexPath itemIndexPath, CGRect rect);

		[Abstract]
		[Export ("dropItem:toTarget:")]
		IUIDragAnimating DropItemToTarget (UIDragItem dragItem, UIDragPreviewTarget target);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICollectionViewPlaceholder {
		[Export ("initWithInsertionIndexPath:reuseIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier);

		[NullAllowed, Export ("cellUpdateHandler", ArgumentSemantic.Copy)]
		Action<UICollectionViewCell> CellUpdateHandler { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewPlaceholder))]
	interface UICollectionViewDropPlaceholder {
		// inlined
		[Export ("initWithInsertionIndexPath:reuseIdentifier:")]
		NativeHandle Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier);

		[NullAllowed, Export ("previewParametersProvider", ArgumentSemantic.Copy)]
		Func<UICollectionViewCell, UIDragPreviewParameters> PreviewParametersProvider { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UICollectionViewDropItem {
		[Abstract]
		[Export ("dragItem")]
		UIDragItem DragItem { get; }

		[Abstract]
		[NullAllowed, Export ("sourceIndexPath")]
		NSIndexPath SourceIndexPath { get; }

		[Abstract]
		[Export ("previewSize")]
		CGSize PreviewSize { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UICollectionViewDropPlaceholderContext : UIDragAnimating {
		[Abstract]
		[Export ("dragItem")]
		UIDragItem DragItem { get; }

		[Abstract]
		[Export ("commitInsertionWithDataSourceUpdates:")]
		bool CommitInsertion (Action<NSIndexPath> dataSourceUpdates);

		[Abstract]
		[Export ("deletePlaceholder")]
		bool DeletePlaceholder ();

		[Abstract]
		[Export ("setNeedsCellUpdate")]
		void SetNeedsCellUpdate ();
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UITableViewDragDelegate {
		[Abstract]
		[Export ("tableView:itemsForBeginningDragSession:atIndexPath:")]
		UIDragItem [] GetItemsForBeginningDragSession (UITableView tableView, IUIDragSession session, NSIndexPath indexPath);

		[Export ("tableView:itemsForAddingToDragSession:atIndexPath:point:")]
		UIDragItem [] GetItemsForAddingToDragSession (UITableView tableView, IUIDragSession session, NSIndexPath indexPath, CGPoint point);

		[Export ("tableView:dragPreviewParametersForRowAtIndexPath:")]
		[return: NullAllowed]
		UIDragPreviewParameters GetDragPreviewParameters (UITableView tableView, NSIndexPath indexPath);

		[Export ("tableView:dragSessionWillBegin:")]
		void DragSessionWillBegin (UITableView tableView, IUIDragSession session);

		[Export ("tableView:dragSessionDidEnd:")]
		void DragSessionDidEnd (UITableView tableView, IUIDragSession session);

		[Export ("tableView:dragSessionAllowsMoveOperation:")]
		bool DragSessionAllowsMoveOperation (UITableView tableView, IUIDragSession session);

		[Export ("tableView:dragSessionIsRestrictedToDraggingApplication:")]
		bool DragSessionIsRestrictedToDraggingApplication (UITableView tableView, IUIDragSession session);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UITableViewDropDelegate {
		[Abstract]
		[Export ("tableView:performDropWithCoordinator:")]
		void PerformDrop (UITableView tableView, IUITableViewDropCoordinator coordinator);

		[Export ("tableView:canHandleDropSession:")]
		bool CanHandleDropSession (UITableView tableView, IUIDropSession session);

		[Export ("tableView:dropSessionDidEnter:")]
		void DropSessionDidEnter (UITableView tableView, IUIDropSession session);

		[Export ("tableView:dropSessionDidUpdate:withDestinationIndexPath:")]
		UITableViewDropProposal DropSessionDidUpdate (UITableView tableView, IUIDropSession session, [NullAllowed] NSIndexPath destinationIndexPath);

		[Export ("tableView:dropSessionDidExit:")]
		void DropSessionDidExit (UITableView tableView, IUIDropSession session);

		[Export ("tableView:dropSessionDidEnd:")]
		void DropSessionDidEnd (UITableView tableView, IUIDropSession session);

		[Export ("tableView:dropPreviewParametersForRowAtIndexPath:")]
		[return: NullAllowed]
		UIDragPreviewParameters GetDropPreviewParameters (UITableView tableView, NSIndexPath indexPath);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDropProposal))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Not implemented
	interface UITableViewDropProposal {

		// inline from base type
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIDropOperation operation);

		[Export ("initWithDropOperation:intent:")]
		NativeHandle Constructor (UIDropOperation operation, UITableViewDropIntent intent);

		[Export ("intent")]
		UITableViewDropIntent Intent { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITableViewDropCoordinator {
		[Abstract]
		[Export ("items")]
		IUITableViewDropItem [] Items { get; }

		[Abstract]
		[NullAllowed, Export ("destinationIndexPath")]
		NSIndexPath DestinationIndexPath { get; }

		[Abstract]
		[Export ("proposal")]
		UITableViewDropProposal Proposal { get; }

		[Abstract]
		[Export ("session")]
		IUIDropSession Session { get; }

		[Abstract]
		[Export ("dropItem:toPlaceholder:")]
		IUITableViewDropPlaceholderContext DropItemToPlaceholder (UIDragItem dragItem, UITableViewDropPlaceholder placeholder);

		[Abstract]
		[Export ("dropItem:toRowAtIndexPath:")]
		IUIDragAnimating DropItemToRow (UIDragItem dragItem, NSIndexPath indexPath);

		[Abstract]
		[Export ("dropItem:intoRowAtIndexPath:rect:")]
		IUIDragAnimating DropItemIntoRow (UIDragItem dragItem, NSIndexPath indexPath, CGRect rect);

		[Abstract]
		[Export ("dropItem:toTarget:")]
		IUIDragAnimating DropItemToTarget (UIDragItem dragItem, UIDragPreviewTarget target);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITableViewPlaceholder {
		[Export ("initWithInsertionIndexPath:reuseIdentifier:rowHeight:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier, nfloat rowHeight);

		[NullAllowed, Export ("cellUpdateHandler", ArgumentSemantic.Copy)]
		Action<UITableViewCell> CellUpdateHandler { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UITableViewPlaceholder))]
	interface UITableViewDropPlaceholder {
		// inlined
		[Export ("initWithInsertionIndexPath:reuseIdentifier:rowHeight:")]
		NativeHandle Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier, nfloat rowHeight);

		[NullAllowed, Export ("previewParametersProvider", ArgumentSemantic.Copy)]
		Func<UITableViewCell, UIDragPreviewParameters> PreviewParametersProvider { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITableViewDropItem {
		[Abstract]
		[Export ("dragItem")]
		UIDragItem DragItem { get; }

		[Abstract]
		[NullAllowed, Export ("sourceIndexPath")]
		NSIndexPath SourceIndexPath { get; }

		[Abstract]
		[Export ("previewSize")]
		CGSize PreviewSize { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITableViewDropPlaceholderContext : UIDragAnimating {
		[Abstract]
		[Export ("dragItem")]
		UIDragItem DragItem { get; }

		[Abstract]
		[Export ("commitInsertionWithDataSourceUpdates:")]
		bool CommitInsertion (Action<NSIndexPath> dataSourceUpdates);

		[Abstract]
		[Export ("deletePlaceholder")]
		bool DeletePlaceholder ();
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextDragPreviewRenderer {
		[Export ("initWithLayoutManager:range:")]
		NativeHandle Constructor (NSLayoutManager layoutManager, NSRange range);

		[Export ("initWithLayoutManager:range:unifyRects:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSLayoutManager layoutManager, NSRange range, bool unifyRects);

		[Export ("layoutManager")]
		NSLayoutManager LayoutManager { get; }

		[Export ("image")]
		UIImage Image { get; }

		[Export ("firstLineRect")]
		CGRect FirstLineRect { get; }

		[Export ("bodyRect")]
		CGRect BodyRect { get; }

		[Export ("lastLineRect")]
		CGRect LastLineRect { get; }

		[Export ("adjustFirstLineRect:bodyRect:lastLineRect:textOrigin:")]
		void Adjust (ref CGRect firstLineRect, ref CGRect bodyRect, ref CGRect lastLineRect, CGPoint origin);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextDraggable : UITextInput {
		[Abstract]
		[NullAllowed, Export ("textDragDelegate", ArgumentSemantic.Weak)]
		IUITextDragDelegate TextDragDelegate { get; set; }

		[Abstract]
		[NullAllowed, Export ("textDragInteraction")]
		UIDragInteraction TextDragInteraction { get; }

		[Abstract]
		[Export ("textDragActive")]
		bool TextDragActive { [Bind ("isTextDragActive")] get; }

		[Abstract]
		[Export ("textDragOptions", ArgumentSemantic.Assign)]
		UITextDragOptions TextDragOptions { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UITextDragDelegate {
		[Export ("textDraggableView:itemsForDrag:")]
		UIDragItem [] GetItemsForDrag (IUITextDraggable textDraggableView, IUITextDragRequest dragRequest);

		[Export ("textDraggableView:dragPreviewForLiftingItem:session:")]
		[return: NullAllowed]
		UITargetedDragPreview GetPreviewForLiftingItem (IUITextDraggable textDraggableView, UIDragItem item, IUIDragSession session);

		[Export ("textDraggableView:willAnimateLiftWithAnimator:session:")]
		void WillAnimateLift (IUITextDraggable textDraggableView, IUIDragAnimating animator, IUIDragSession session);

		[Export ("textDraggableView:dragSessionWillBegin:")]
		void DragSessionWillBegin (IUITextDraggable textDraggableView, IUIDragSession session);

		[Export ("textDraggableView:dragSessionDidEnd:withOperation:")]
		void DragSessionDidEnd (IUITextDraggable textDraggableView, IUIDragSession session, UIDropOperation operation);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextDragRequest {
		[Abstract]
		[Export ("dragRange")]
		UITextRange DragRange { get; }

		[Abstract]
		[Export ("suggestedItems")]
		UIDragItem [] SuggestedItems { get; }

		[Abstract]
		[Export ("existingItems")]
		UIDragItem [] ExistingItems { get; }

		[Abstract]
		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; }

		[Abstract]
		[Export ("dragSession")]
		IUIDragSession DragSession { get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIDropProposal))]
	[DisableDefaultCtor]
	interface UITextDropProposal : NSCopying {
		// inlined
		[Export ("initWithDropOperation:")]
		NativeHandle Constructor (UIDropOperation operation);

		[Export ("dropAction", ArgumentSemantic.Assign)]
		UITextDropAction DropAction { get; set; }

		[Export ("dropProgressMode", ArgumentSemantic.Assign)]
		UITextDropProgressMode DropProgressMode { get; set; }

		[Export ("useFastSameViewOperations")]
		bool UseFastSameViewOperations { get; set; }

		[Export ("dropPerformer", ArgumentSemantic.Assign)]
		UITextDropPerformer DropPerformer { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextDroppable : UITextInput, UITextPasteConfigurationSupporting {
		[Abstract]
		[NullAllowed, Export ("textDropDelegate", ArgumentSemantic.Weak)]
		IUITextDropDelegate TextDropDelegate { get; set; }

		[Abstract]
		[NullAllowed, Export ("textDropInteraction")]
		UIDropInteraction TextDropInteraction { get; }

		[Abstract]
		[Export ("textDropActive")]
		bool TextDropActive { [Bind ("isTextDropActive")] get; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UITextDropDelegate {
		[Export ("textDroppableView:willBecomeEditableForDrop:")]
		UITextDropEditability WillBecomeEditable (IUITextDroppable textDroppableView, IUITextDropRequest drop);

		[Export ("textDroppableView:proposalForDrop:")]
		UITextDropProposal GetProposalForDrop (IUITextDroppable textDroppableView, IUITextDropRequest drop);

		[Export ("textDroppableView:willPerformDrop:")]
		void WillPerformDrop (IUITextDroppable textDroppableView, IUITextDropRequest drop);

		[Export ("textDroppableView:previewForDroppingAllItemsWithDefault:")]
		[return: NullAllowed]
		UITargetedDragPreview GetPreviewForDroppingAllItems (IUITextDroppable textDroppableView, UITargetedDragPreview defaultPreview);

		[Export ("textDroppableView:dropSessionDidEnter:")]
		void DropSessionDidEnter (IUITextDroppable textDroppableView, IUIDropSession session);

		[Export ("textDroppableView:dropSessionDidUpdate:")]
		void DropSessionDidUpdate (IUITextDroppable textDroppableView, IUIDropSession session);

		[Export ("textDroppableView:dropSessionDidExit:")]
		void DropSessionDidExit (IUITextDroppable textDroppableView, IUIDropSession session);

		[Export ("textDroppableView:dropSessionDidEnd:")]
		void DropSessionDidEnd (IUITextDroppable textDroppableView, IUIDropSession session);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextDropRequest {
		[Abstract]
		[Export ("dropPosition")]
		UITextPosition DropPosition { get; }

		[Abstract]
		[Export ("suggestedProposal")]
		UITextDropProposal SuggestedProposal { get; }

		[Abstract]
		[Export ("sameView")]
		bool SameView { [Bind ("isSameView")] get; }

		[Abstract]
		[Export ("dropSession")]
		IUIDropSession DropSession { get; }
	}

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIDataSourceTranslating {
		[Abstract]
		[Export ("presentationSectionIndexForDataSourceSectionIndex:")]
		nint GetPresentationSectionIndex (nint dataSourceSectionIndex);

		[Abstract]
		[Export ("dataSourceSectionIndexForPresentationSectionIndex:")]
		nint GetDataSourceSectionIndex (nint presentationSectionIndex);

		[Abstract]
		[Export ("presentationIndexPathForDataSourceIndexPath:")]
		[return: NullAllowed]
		NSIndexPath GetPresentationIndexPath ([NullAllowed] NSIndexPath dataSourceIndexPath);

		[Abstract]
		[Export ("dataSourceIndexPathForPresentationIndexPath:")]
		[return: NullAllowed]
		NSIndexPath GetDataSourceIndexPath ([NullAllowed] NSIndexPath presentationIndexPath);

		[Abstract]
		[Export ("performUsingPresentationValues:")]
		void PerformUsingPresentationValues (Action actionsToTranslate);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISpringLoadedInteraction : UIInteraction {
		[Export ("initWithInteractionBehavior:interactionEffect:activationHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] IUISpringLoadedInteractionBehavior interactionBehavior, [NullAllowed] IUISpringLoadedInteractionEffect interactionEffect, Action<UISpringLoadedInteraction, IUISpringLoadedInteractionContext> handler);

		[Export ("initWithActivationHandler:")]
		NativeHandle Constructor (Action<UISpringLoadedInteraction, IUISpringLoadedInteractionContext> handler);

		[Export ("interactionBehavior", ArgumentSemantic.Strong)]
		IUISpringLoadedInteractionBehavior InteractionBehavior { get; }

		[Export ("interactionEffect", ArgumentSemantic.Strong)]
		IUISpringLoadedInteractionEffect InteractionEffect { get; }
	}

	interface IUISpringLoadedInteractionBehavior { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UISpringLoadedInteractionBehavior {
		[Abstract]
		[Export ("shouldAllowInteraction:withContext:")]
		bool ShouldAllowInteraction (UISpringLoadedInteraction interaction, IUISpringLoadedInteractionContext context);

		[Export ("interactionDidFinish:")]
		void InteractionDidFinish (UISpringLoadedInteraction interaction);
	}

	interface IUISpringLoadedInteractionEffect { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UISpringLoadedInteractionEffect {
		[Abstract]
		[Export ("interaction:didChangeWithContext:")]
		void DidChange (UISpringLoadedInteraction interaction, IUISpringLoadedInteractionContext context);
	}

	interface IUISpringLoadedInteractionContext { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UISpringLoadedInteractionContext {
		[Abstract]
		[Export ("state")]
		UISpringLoadedInteractionEffectState State { get; }

		[Abstract]
		[NullAllowed, Export ("targetView", ArgumentSemantic.Strong)]
		UIView TargetView { get; set; }

		[Abstract]
		[NullAllowed, Export ("targetItem", ArgumentSemantic.Strong)]
		NSObject TargetItem { get; set; }

		[Abstract]
		[Export ("locationInView:")]
		CGPoint LocationInView ([NullAllowed] UIView view);
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UISpringLoadedInteractionSupporting {
		[Abstract]
		[Export ("springLoaded")]
		bool SpringLoaded { [Bind ("isSpringLoaded")] get; set; }
	}

	// https://bugzilla.xamarin.com/show_bug.cgi?id=58282, we should be able to write one delegate with a 'Action<bool>'. See original signature:
	// typedef void (^UIContextualActionHandler)(UIContextualAction * _Nonnull, __kindof UIView * _Nonnull, void (^ _Nonnull)(BOOL));
	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	delegate void UIContextualActionHandler (UIContextualAction action, UIView sourceView, [BlockCallback] UIContextualActionCompletionHandler completionHandler);

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	delegate void UIContextualActionCompletionHandler (bool finished);

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIContextualAction {
		[Static]
		[Export ("contextualActionWithStyle:title:handler:")]
		UIContextualAction FromContextualActionStyle (UIContextualActionStyle style, [NullAllowed] string title, UIContextualActionHandler handler);

		[Export ("style")]
		UIContextualActionStyle Style { get; }

		[Export ("handler", ArgumentSemantic.Copy)]
		UIContextualActionHandler Handler { get; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; set; }
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISwipeActionsConfiguration {
		[Static]
		[Export ("configurationWithActions:")]
		UISwipeActionsConfiguration FromActions (UIContextualAction [] actions);

		[Export ("actions", ArgumentSemantic.Copy)]
		UIContextualAction [] Actions { get; }

		[Export ("performsFirstActionWithFullSwipe")]
		bool PerformsFirstActionWithFullSwipe { get; set; }
	}

	interface IUITextPasteConfigurationSupporting { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextPasteConfigurationSupporting : UIPasteConfigurationSupporting {
		[Abstract]
		[NullAllowed, Export ("pasteDelegate", ArgumentSemantic.Weak)]
		IUITextPasteDelegate PasteDelegate { get; set; }
	}

	interface IUITextPasteDelegate { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UITextPasteDelegate {
		[Export ("textPasteConfigurationSupporting:transformPasteItem:")]
		void TransformPasteItem (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, IUITextPasteItem item);

		[Export ("textPasteConfigurationSupporting:combineItemAttributedStrings:forRange:")]
		NSAttributedString CombineItemAttributedStrings (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString [] itemStrings, UITextRange textRange);

		[Export ("textPasteConfigurationSupporting:performPasteOfAttributedString:toRange:")]
		UITextRange PerformPaste (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString attributedString, UITextRange textRange);

		[Export ("textPasteConfigurationSupporting:shouldAnimatePasteOfAttributedString:toRange:")]
		bool ShouldAnimatePaste (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString attributedString, UITextRange textRange);
	}

	interface IUITextPasteItem { }

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UITextPasteItem {
		[Abstract]
		[Export ("itemProvider")]
		NSItemProvider ItemProvider { get; }

		[Abstract]
		[NullAllowed, Export ("localObject")]
		NSObject LocalObject { get; }

		[Abstract]
		[Export ("defaultAttributes")]
		NSDictionary<NSString, NSObject> DefaultAttributes { get; }

		[Abstract]
		[Export ("setStringResult:")]
		void SetStringResult (string @string);

		[Abstract]
		[Export ("setAttributedStringResult:")]
		void SetAttributedStringResult (NSAttributedString @string);

		[Abstract]
		[Export ("setAttachmentResult:")]
		void SetAttachmentResult (NSTextAttachment textAttachment);

		[Abstract]
		[Export ("setNoResult")]
		void SetNoResult ();

		[Abstract]
		[Export ("setDefaultResult")]
		void SetDefaultResult ();
	}

	[NoWatch, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIPasteConfiguration : NSSecureCoding, NSCopying {
		[Export ("acceptableTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] AcceptableTypeIdentifiers { get; set; }

		[Export ("initWithAcceptableTypeIdentifiers:")]
		NativeHandle Constructor (string [] acceptableTypeIdentifiers);

		[Export ("addAcceptableTypeIdentifiers:")]
		void AddAcceptableTypeIdentifiers (string [] acceptableTypeIdentifiers);

		[Export ("initWithTypeIdentifiersForAcceptingClass:")]
		NativeHandle Constructor (Class itemProviderReadingClass);

		[Wrap ("this (new Class (itemProviderReadingType))")]
		NativeHandle Constructor (Type itemProviderReadingType);

		[Export ("addTypeIdentifiersForAcceptingClass:")]
		void AddTypeIdentifiers (Class itemProviderReadingClass);

		[Wrap ("AddTypeIdentifiers (new Class (itemProviderReadingType))")]
		void AddTypeIdentifiers (Type itemProviderReadingType);
	}

	interface IUIPasteConfigurationSupporting { }

	[NoWatch, NoTV]
	[MacCatalyst (16, 0)]
	[Protocol]
	interface UIPasteConfigurationSupporting {
		[Abstract]
		[NullAllowed, Export ("pasteConfiguration", ArgumentSemantic.Copy)]
		UIPasteConfiguration PasteConfiguration { get; set; }

		[Export ("pasteItemProviders:")]
		void Paste (NSItemProvider [] itemProviders);

		[Export ("canPasteItemProviders:")]
		bool CanPaste (NSItemProvider [] itemProviders);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	interface UIDocumentBrowserViewController : NSCoding {
		[Deprecated (PlatformName.iOS, 14, 0)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("initForOpeningFilesWithContentTypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string [] allowedContentTypes);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initForOpeningContentTypes:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] UTType [] contentTypes);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUIDocumentBrowserViewControllerDelegate Delegate { get; set; }

		[Export ("allowsDocumentCreation")]
		bool AllowsDocumentCreation { get; set; }

		[Export ("allowsPickingMultipleItems")]
		bool AllowsPickingMultipleItems { get; set; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "No longer supported.")]
		[Export ("allowedContentTypes", ArgumentSemantic.Copy)]
		string [] AllowedContentTypes { get; }

		[Deprecated (PlatformName.iOS, 14, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 0)]
		[Export ("recentDocumentsContentTypes", ArgumentSemantic.Copy)]
		string [] RecentDocumentsContentTypes { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("contentTypesForRecentDocuments", ArgumentSemantic.Copy)]
		UTType [] ContentTypesForRecentDocuments { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("shouldShowFileExtensions")]
		bool ShouldShowFileExtensions { get; set; }

		[Export ("additionalLeadingNavigationBarButtonItems", ArgumentSemantic.Strong)]
		UIBarButtonItem [] AdditionalLeadingNavigationBarButtonItems { get; set; }

		[Export ("additionalTrailingNavigationBarButtonItems", ArgumentSemantic.Strong)]
		UIBarButtonItem [] AdditionalTrailingNavigationBarButtonItems { get; set; }

		[Async]
		[Export ("revealDocumentAtURL:importIfNeeded:completion:")]
		void RevealDocument (NSUrl url, bool importIfNeeded, [NullAllowed] Action<NSUrl, NSError> completion);

		[Async]
		[Export ("importDocumentAtURL:nextToDocumentAtURL:mode:completionHandler:")]
		void ImportDocument (NSUrl documentUrl, NSUrl neighbourUrl, UIDocumentBrowserImportMode importMode, Action<NSUrl, NSError> completion);

		[Internal] // got deprecated
		[Export ("transitionControllerForDocumentURL:")]
		UIDocumentBrowserTransitionController _DeprecatedGetTransitionController (NSUrl documentUrl);

		[Export ("customActions", ArgumentSemantic.Copy)]
		UIDocumentBrowserAction [] CustomActions { get; set; }

		[Export ("browserUserInterfaceStyle", ArgumentSemantic.Assign)]
		UIDocumentBrowserUserInterfaceStyle BrowserUserInterfaceStyle { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("localizedCreateDocumentActionTitle")]
		string LocalizedCreateDocumentActionTitle { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("defaultDocumentAspectRatio")]
		nfloat DefaultDocumentAspectRatio { get; set; }

		[Internal]
		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("transitionControllerForDocumentAtURL:")]
		UIDocumentBrowserTransitionController _NewGetTransitionController (NSUrl documentUrl);

		[Async]
		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("renameDocumentAtURL:proposedName:completionHandler:")]
		void RenameDocument (NSUrl documentUrl, string proposedName, Action<NSUrl, NSError> completionHandler);
	}

	interface IUIDocumentBrowserViewControllerDelegate { }

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIDocumentBrowserViewControllerDelegate {
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'DidPickDocumentsAtUrls (UIDocumentBrowserViewController, NSUrl[])' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'DidPickDocumentsAtUrls (UIDocumentBrowserViewController, NSUrl[])' instead.")]
		[Export ("documentBrowser:didPickDocumentURLs:")]
		void DidPickDocumentUrls (UIDocumentBrowserViewController controller, NSUrl [] documentUrls);

		[Export ("documentBrowser:didRequestDocumentCreationWithHandler:")]
		void DidRequestDocumentCreation (UIDocumentBrowserViewController controller, Action<NSUrl, UIDocumentBrowserImportMode> importHandler);

		[Export ("documentBrowser:didImportDocumentAtURL:toDestinationURL:")]
		void DidImportDocument (UIDocumentBrowserViewController controller, NSUrl sourceUrl, NSUrl destinationUrl);

		[Export ("documentBrowser:failedToImportDocumentAtURL:error:")]
		void FailedToImportDocument (UIDocumentBrowserViewController controller, NSUrl documentUrl, [NullAllowed] NSError error);

		[Export ("documentBrowser:applicationActivitiesForDocumentURLs:")]
		UIActivity [] GetApplicationActivities (UIDocumentBrowserViewController controller, NSUrl [] documentUrls);

		[Export ("documentBrowser:willPresentActivityViewController:")]
		void WillPresent (UIDocumentBrowserViewController controller, UIActivityViewController activityViewController);

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("documentBrowser:didPickDocumentsAtURLs:")]
		void DidPickDocumentsAtUrls (UIDocumentBrowserViewController controller, NSUrl [] documentUrls);
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDocumentBrowserTransitionController : UIViewControllerAnimatedTransitioning {
		[NullAllowed, Export ("loadingProgress", ArgumentSemantic.Strong)]
		NSProgress LoadingProgress { get; set; }

		[NullAllowed, Export ("targetView", ArgumentSemantic.Weak)]
		UIView TargetView { get; set; }
	}

	[NoTV, NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDocumentBrowserAction {
		[Export ("initWithIdentifier:localizedTitle:availability:handler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string localizedTitle, UIDocumentBrowserActionAvailability availability, Action<NSUrl []> handler);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; }

		[Export ("availability")]
		UIDocumentBrowserActionAvailability Availability { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Export ("supportedContentTypes", ArgumentSemantic.Copy)]
		string [] SupportedContentTypes { get; set; }

		[Export ("supportsMultipleItems")]
		bool SupportsMultipleItems { get; set; }
	}

	interface IUIFocusItemContainer { }
	[iOS (12, 0), TV (12, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusItemContainer {
		[Abstract]
		[Export ("coordinateSpace")]
		IUICoordinateSpace CoordinateSpace { get; }

		[Abstract]
		[Export ("focusItemsInRect:")]
		IUIFocusItem [] GetFocusItems (CGRect rect);
	}

	[iOS (12, 0), TV (12, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIFocusItemScrollableContainer : UIFocusItemContainer {
		[Abstract]
		[Export ("contentOffset", ArgumentSemantic.Assign)]
		CGPoint ContentOffset { get; set; }

		[Abstract]
		[Export ("contentSize")]
		CGSize ContentSize { get; }

		[Abstract]
		[Export ("visibleSize")]
		CGSize VisibleSize { get; }
	}

	[NoWatch] // it was added on 8,0, but was not binded and the method was added in 12,0
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIUserActivityRestoring {
		[Abstract]
		[TV (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("restoreUserActivityState:")]
		void RestoreUserActivityState (NSUserActivity activity);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIFontMetrics {
		[Static]
		[Export ("defaultMetrics", ArgumentSemantic.Strong)]
		UIFontMetrics DefaultMetrics { get; }

		[Static]
		[Export ("metricsForTextStyle:")]
		UIFontMetrics GetMetrics (string textStyle);

		[Export ("initForTextStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string textStyle);

		[Export ("scaledFontForFont:")]
		UIFont GetScaledFont (UIFont font);

		[Export ("scaledFontForFont:maximumPointSize:")]
		UIFont GetScaledFont (UIFont font, nfloat maximumPointSize);

		[Export ("scaledValueForValue:")]
		nfloat GetScaledValue (nfloat value);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("scaledFontForFont:compatibleWithTraitCollection:")]
		UIFont GetScaledFont (UIFont font, [NullAllowed] UITraitCollection traitCollection);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("scaledFontForFont:maximumPointSize:compatibleWithTraitCollection:")]
		UIFont GetScaledFont (UIFont font, nfloat maximumPointSize, [NullAllowed] UITraitCollection traitCollection);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("scaledValueForValue:compatibleWithTraitCollection:")]
		nfloat GetScaledValue (nfloat value, [NullAllowed] UITraitCollection traitCollection);
	}

	[iOS (12, 1)]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UIPencilPreferredAction : long {
		Ignore = 0,
		SwitchEraser,
		SwitchPrevious,
		ShowColorPalette,
		[iOS (16, 0), MacCatalyst (16, 0)]
		ShowInkAttributes,
	}

	[iOS (12, 1)]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIPencilInteraction : UIInteraction {
		[Static]
		[Export ("preferredTapAction")]
		UIPencilPreferredAction PreferredTapAction { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("prefersPencilOnlyDrawing")]
		bool PrefersPencilOnlyDrawing { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIPencilInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	interface IUIPencilInteractionDelegate { }

	[iOS (12, 1)]
	[NoWatch]
	[NoTV]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIPencilInteractionDelegate {
		[Export ("pencilInteractionDidTap:")]
		void DidTap (UIPencilInteraction interaction);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "UIOpenURLContext")]
	[DisableDefaultCtor]
	interface UIOpenUrlContext {

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("options", ArgumentSemantic.Strong)]
		UISceneOpenUrlOptions Options { get; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIResponder))]
	[DisableDefaultCtor]
	interface UIScene {

		[Export ("initWithSession:connectionOptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Export ("session")]
		UISceneSession Session { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUISceneDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Strong)]
		NSObject WeakDelegate { get; set; }

		[Export ("activationState")]
		UISceneActivationState ActivationState { get; }

		[Async]
		[Export ("openURL:options:completionHandler:")]
		void OpenUrl (NSUrl url, [NullAllowed] UISceneOpenExternalUrlOptions options, [NullAllowed] Action<bool> completion);

		[Export ("title")]
		string Title { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("subtitle")]
		string Subtitle { get; set; }

		[Export ("activationConditions", ArgumentSemantic.Strong)]
		UISceneActivationConditions ActivationConditions { get; set; }

		[Field ("UISceneWillConnectNotification")]
		[Notification]
		NSString WillConnectNotification { get; }

		[Field ("UISceneDidDisconnectNotification")]
		[Notification]
		NSString DidDisconnectNotification { get; }

		[Field ("UISceneDidActivateNotification")]
		[Notification]
		NSString DidActivateNotification { get; }

		[Field ("UISceneWillDeactivateNotification")]
		[Notification]
		NSString WillDeactivateNotification { get; }

		[Field ("UISceneWillEnterForegroundNotification")]
		[Notification]
		NSString WillEnterForegroundNotification { get; }

		[Field ("UISceneDidEnterBackgroundNotification")]
		[Notification]
		NSString DidEnterBackgroundNotification { get; }

		// UIScene (PointerLockState) category

		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("pointerLockState")]
		UIPointerLockState PointerLockState { get; }

		// UIScene (UISceneEnhancedStateRestoration) category

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("extendStateRestoration")]
		void ExtendStateRestoration ();

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("completeStateRestoration")]
		void CompleteStateRestoration ();
	}

	interface IUISceneDelegate { }

	[iOS (13, 0), TV (13, 0), NoWatch, NoMac]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UISceneDelegate {

		[Export ("scene:willConnectToSession:options:")]
		void WillConnect (UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Export ("sceneDidDisconnect:")]
		void DidDisconnect (UIScene scene);

		[Export ("sceneDidBecomeActive:")]
		void DidBecomeActive (UIScene scene);

		[Export ("sceneWillResignActive:")]
		void WillResignActive (UIScene scene);

		[Export ("sceneWillEnterForeground:")]
		void WillEnterForeground (UIScene scene);

		[Export ("sceneDidEnterBackground:")]
		void DidEnterBackground (UIScene scene);

		[Export ("scene:openURLContexts:")]
		void OpenUrlContexts (UIScene scene, NSSet<UIOpenUrlContext> urlContexts);

		[Export ("stateRestorationActivityForScene:")]
		[return: NullAllowed]
		NSUserActivity GetStateRestorationActivity (UIScene scene);

		[Export ("scene:restoreInteractionStateWithUserActivity:")]
		void RestoreInteractionState (UIScene scene, NSUserActivity stateRestorationActivity);

		[Export ("scene:willContinueUserActivityWithType:")]
		void WillContinueUserActivity (UIScene scene, string userActivityType);

		[Export ("scene:continueUserActivity:")]
		void ContinueUserActivity (UIScene scene, NSUserActivity userActivity);

		[Export ("scene:didFailToContinueUserActivityWithType:error:")]
		void DidFailToContinueUserActivity (UIScene scene, string userActivityType, NSError error);

		[Export ("scene:didUpdateUserActivity:")]
		void DidUpdateUserActivity (UIScene scene, NSUserActivity userActivity);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UISceneActivationConditions : NSSecureCoding {

		[Export ("canActivateForTargetContentIdentifierPredicate", ArgumentSemantic.Copy)]
		NSPredicate CanActivateForTargetContentIdentifierPredicate { get; set; }

		[Export ("prefersToActivateForTargetContentIdentifierPredicate", ArgumentSemantic.Copy)]
		NSPredicate PrefersToActivateForTargetContentIdentifierPredicate { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UISceneActivationRequestOptions : NSCopying {

		[NullAllowed, Export ("requestingScene", ArgumentSemantic.Strong)]
		UIScene RequestingScene { get; set; }

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoWatch, NoTV, NoiOS]
		[Export ("collectionJoinBehavior", ArgumentSemantic.Assign)]
		UISceneCollectionJoinBehavior CollectionJoinBehavior { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneConfiguration : NSCopying, NSSecureCoding {

		[Static]
		[Export ("configurationWithName:sessionRole:")]
		UISceneConfiguration Create ([NullAllowed] string name, [BindAs (typeof (UIWindowSceneSessionRole))] NSString sessionRole);

		[Export ("initWithName:sessionRole:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string name, [BindAs (typeof (UIWindowSceneSessionRole))] NSString sessionRole);

		[NullAllowed, Export ("name")]
		string Name { get; }

		[BindAs (typeof (UIWindowSceneSessionRole))]
		[Export ("role")]
		NSString Role { get; }

		[Advice ("You can use 'SceneType' with a 'Type' instead.")]
		[NullAllowed, Export ("sceneClass", ArgumentSemantic.Assign)]
		Class SceneClass { get; set; }

		Type SceneType {
			[Wrap ("Class.Lookup (SceneClass)!")]
			get;
			[Wrap ("SceneClass = value is null ? null : new Class (value)")]
			set;
		}

		[Advice ("You can use 'DelegateType' with a 'Type' instead.")]
		[NullAllowed, Export ("delegateClass", ArgumentSemantic.Assign)]
		Class DelegateClass { get; set; }

		Type DelegateType {
			[Wrap ("Class.Lookup (DelegateClass)!")]
			get;
			[Wrap ("DelegateClass = value is null ? null : new Class (value)")]
			set;
		}

		[NullAllowed, Export ("storyboard", ArgumentSemantic.Strong)]
		UIStoryboard Storyboard { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneConnectionOptions {

		[Export ("URLContexts", ArgumentSemantic.Copy)]
		NSSet<UIOpenUrlContext> UrlContexts { get; }

		[NullAllowed, Export ("sourceApplication")]
		string SourceApplication { get; }

		[NullAllowed, Export ("handoffUserActivityType")]
		string HandoffUserActivityType { get; }

		[Export ("userActivities", ArgumentSemantic.Copy)]
		NSSet<NSUserActivity> UserActivities { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("notificationResponse")]
		UNNotificationResponse NotificationResponse { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("shortcutItem")]
		UIApplicationShortcutItem ShortcutItem { get; }

		[NullAllowed, Export ("cloudKitShareMetadata")]
		CKShareMetadata CloudKitShareMetadata { get; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UISceneDestructionRequestOptions {

	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "UISceneOpenExternalURLOptions")]
	interface UISceneOpenExternalUrlOptions {

		[Export ("universalLinksOnly")]
		bool UniversalLinksOnly { get; set; }

		[NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed]
		[Export ("eventAttribution", ArgumentSemantic.Copy)]
		UIEventAttribution EventAttribution { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "UISceneOpenURLOptions")]
	[DisableDefaultCtor]
	interface UISceneOpenUrlOptions {

		[NullAllowed, Export ("sourceApplication")]
		string SourceApplication { get; }

		[NullAllowed, Export ("annotation")]
		NSObject Annotation { get; }

		[Export ("openInPlace")]
		bool OpenInPlace { get; }

		[NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed]
		[Export ("eventAttribution")]
		UIEventAttribution EventAttribution { get; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneSession : NSSecureCoding {

		[NullAllowed, Export ("scene")]
		UIScene Scene { get; }

		[BindAs (typeof (UIWindowSceneSessionRole))]
		[Export ("role")]
		NSString Role { get; }

		[Export ("configuration", ArgumentSemantic.Copy)]
		UISceneConfiguration Configuration { get; }

		[Export ("persistentIdentifier")]
		string PersistentIdentifier { get; }

		[NullAllowed, Export ("stateRestorationActivity", ArgumentSemantic.Strong)]
		NSUserActivity StateRestorationActivity { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> UserInfo { get; set; }
	}

	[iOS (13, 0), TV (13, 0)]
	[Watch (5, 2)]
	[MacCatalyst (13, 1)]
	/* NS_TYPED_ENUM */
	enum UIFontDescriptorSystemDesign {
		[DefaultEnumValue]
		[Field ("UIFontDescriptorSystemDesignDefault")]
		Default,
		[Field ("UIFontDescriptorSystemDesignRounded")]
		Rounded,
		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UIFontDescriptorSystemDesignSerif")]
		Serif,
		[Watch (7, 0)]
		[MacCatalyst (13, 1)]
		[Field ("UIFontDescriptorSystemDesignMonospaced")]
		Monospaced,
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIBarAppearance : NSCopying, NSSecureCoding {

		[Export ("initWithIdiom:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIUserInterfaceIdiom idiom);

		[Export ("idiom", ArgumentSemantic.Assign)]
		UIUserInterfaceIdiom Idiom { get; }

		[Export ("initWithBarAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIBarAppearance barAppearance);

		[Export ("configureWithDefaultBackground")]
		void ConfigureWithDefaultBackground ();

		[Export ("configureWithOpaqueBackground")]
		void ConfigureWithOpaqueBackground ();

		[Export ("configureWithTransparentBackground")]
		void ConfigureWithTransparentBackground ();

		[NullAllowed, Export ("backgroundEffect", ArgumentSemantic.Copy)]
		UIBlurEffect BackgroundEffect { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Strong)]
		UIImage BackgroundImage { get; set; }

		[Export ("backgroundImageContentMode", ArgumentSemantic.Assign)]
		UIViewContentMode BackgroundImageContentMode { get; set; }

		[NullAllowed, Export ("shadowColor", ArgumentSemantic.Copy)]
		UIColor ShadowColor { get; set; }

		[NullAllowed, Export ("shadowImage", ArgumentSemantic.Strong)]
		UIImage ShadowImage { get; set; }
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIBarButtonItemStateAppearance {

		[Export ("titleTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> TitleTextAttributes { get; set; }

		[Export ("titlePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset TitlePositionAdjustment { get; set; }

		[NullAllowed, Export ("backgroundImage", ArgumentSemantic.Strong)]
		UIImage BackgroundImage { get; set; }

		[Export ("backgroundImagePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset BackgroundImagePositionAdjustment { get; set; }
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIBarButtonItemAppearance : NSCopying, NSSecureCoding {

		[Export ("initWithStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIBarButtonItemStyle style);

		[Export ("configureWithDefaultForStyle:")]
		void ConfigureWithDefault (UIBarButtonItemStyle style);

		[Export ("normal", ArgumentSemantic.Strong)]
		UIBarButtonItemStateAppearance Normal { get; }

		[Export ("highlighted", ArgumentSemantic.Strong)]
		UIBarButtonItemStateAppearance Highlighted { get; }

		[Export ("disabled", ArgumentSemantic.Strong)]
		UIBarButtonItemStateAppearance Disabled { get; }

		[Export ("focused", ArgumentSemantic.Strong)]
		UIBarButtonItemStateAppearance Focused { get; }
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewCompositionalLayoutConfiguration : NSCopying {

		[Export ("scrollDirection", ArgumentSemantic.Assign)]
		UICollectionViewScrollDirection ScrollDirection { get; set; }

		[Export ("interSectionSpacing")]
		nfloat InterSectionSpacing { get; set; }

		[Export ("boundarySupplementaryItems", ArgumentSemantic.Copy)]
		NSCollectionLayoutBoundarySupplementaryItem [] BoundarySupplementaryItems { get; set; }

		[Watch (7, 0), TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("contentInsetsReference", ArgumentSemantic.Assign)]
		UIContentInsetsReference ContentInsetsReference { get; set; }
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate NSCollectionLayoutSection UICollectionViewCompositionalLayoutSectionProvider (nint section, INSCollectionLayoutEnvironment layoutEnvironment);

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UICollectionViewLayout))]
	[DisableDefaultCtor]
	interface UICollectionViewCompositionalLayout {

		[Export ("initWithSection:")]
		NativeHandle Constructor (NSCollectionLayoutSection section);

		[Export ("initWithSection:configuration:")]
		NativeHandle Constructor (NSCollectionLayoutSection section, UICollectionViewCompositionalLayoutConfiguration configuration);

		[Export ("initWithSectionProvider:")]
		NativeHandle Constructor (UICollectionViewCompositionalLayoutSectionProvider sectionProvider);

		[Export ("initWithSectionProvider:configuration:")]
		NativeHandle Constructor (UICollectionViewCompositionalLayoutSectionProvider sectionProvider, UICollectionViewCompositionalLayoutConfiguration configuration);

		[Export ("configuration", ArgumentSemantic.Copy)]
		UICollectionViewCompositionalLayoutConfiguration Configuration { get; set; }

		// UICollectionViewCompositionalLayout (UICollectionLayoutListSection) category

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("layoutWithListConfiguration:")]
		UICollectionViewCompositionalLayout GetLayout (UICollectionLayoutListConfiguration listConfiguration);
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICommandAlternate : NSCopying, NSSecureCoding {

		[Export ("title")]
		string Title { get; }

		[Export ("action")]
		Selector Action { get; }

		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[Static]
		[Export ("alternateWithTitle:action:modifierFlags:")]
		UICommandAlternate Create (string title, Selector action, UIKeyModifierFlags modifierFlags);
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIMenuElement))]
	[DisableDefaultCtor]
	interface UICommand : UIMenuLeaf {

		[Field ("UICommandTagShare")]
		NSString UICommandTagShare { get; }

		[Export ("title")]
		new string Title { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		new UIImage Image { get; set; }

		[NullAllowed, Export ("discoverabilityTitle")]
		new string DiscoverabilityTitle { get; set; }

		[Export ("action")]
		Selector Action { get; }

		[NullAllowed, Export ("propertyList")]
		NSObject PropertyList { get; }

		[Export ("attributes", ArgumentSemantic.Assign)]
		new UIMenuElementAttributes Attributes { get; set; }

		[Export ("state", ArgumentSemantic.Assign)]
		new UIMenuElementState State { get; set; }

		[Export ("alternates")]
		UICommandAlternate [] Alternates { get; }

		[Static]
		[Export ("commandWithTitle:image:action:propertyList:")]
		UICommand Create (string title, [NullAllowed] UIImage image, Selector action, [NullAllowed] NSObject propertyList);

		[Static]
		[Export ("commandWithTitle:image:action:propertyList:alternates:")]
		UICommand Create (string title, [NullAllowed] UIImage image, Selector action, [NullAllowed] NSObject propertyList, UICommandAlternate [] alternates);
	}

	interface IUIFontPickerViewControllerDelegate { }

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIFontPickerViewControllerDelegate {

		[Export ("fontPickerViewControllerDidCancel:")]
		void DidCancel (UIFontPickerViewController viewController);

		[Export ("fontPickerViewControllerDidPickFont:")]
		void DidPickFont (UIFontPickerViewController viewController);
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface UIFontPickerViewController {

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIFontPickerViewControllerConfiguration configuration);

		[Export ("configuration", ArgumentSemantic.Copy)]
		UIFontPickerViewControllerConfiguration Configuration { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIFontPickerViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("selectedFontDescriptor", ArgumentSemantic.Strong)]
		UIFontDescriptor SelectedFontDescriptor { get; set; }
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UIFontPickerViewControllerConfiguration : NSCopying {

		[Export ("includeFaces")]
		bool IncludeFaces { get; set; }

		[Export ("displayUsingSystemFont")]
		bool DisplayUsingSystemFont { get; set; }

		[Export ("filteredTraits", ArgumentSemantic.Assign)]
		UIFontDescriptorSymbolicTraits FilteredTraits { get; set; }

		[NullAllowed, Export ("filteredLanguagesPredicate", ArgumentSemantic.Copy)]
		NSPredicate FilteredLanguagesPredicate { get; set; }

		[Static]
		[Export ("filterPredicateForFilteredLanguages:")]
		[return: NullAllowed]
		NSPredicate FilterPredicate (string [] filteredLanguages);
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIGestureRecognizer))]
	[DisableDefaultCtor]
	interface UIHoverGestureRecognizer {

		[DesignatedInitializer]
		[Export ("initWithTarget:action:")]
		NativeHandle Constructor (NSObject target, Selector action);

		[DesignatedInitializer]
		[Wrap ("base (action)")]
		NativeHandle Constructor (Action action);

		[iOS (16, 1)]
		[MacCatalyst (16, 1)]
		[Export ("zOffset")]
		nfloat ZOffset { get; }

		[iOS (16, 4), NoMacCatalyst]
		[Export ("azimuthAngleInView:")]
		nfloat GetAzimuthAngle ([NullAllowed] UIView view);

		[iOS (16, 4), NoMacCatalyst]
		[Export ("azimuthUnitVectorInView:")]
		CGVector GetAzimuthUnitVector ([NullAllowed] UIView view);

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("altitudeAngle")]
		nfloat AltitudeAngle { get; }
	}

	interface IUILargeContentViewerItem { }

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UILargeContentViewerItem {

		[Abstract]
		[Export ("showsLargeContentViewer")]
		bool ShowsLargeContentViewer { get; }

		[Abstract]
		[NullAllowed, Export ("largeContentTitle")]
		string LargeContentTitle { get; }

		[Abstract]
		[NullAllowed, Export ("largeContentImage", ArgumentSemantic.Strong)]
		UIImage LargeContentImage { get; }

		[Abstract]
		[Export ("scalesLargeContentImage")]
		bool ScalesLargeContentImage { get; }

		[Abstract]
		[Export ("largeContentImageInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets LargeContentImageInsets { get; }
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UILargeContentViewerInteraction : UIInteraction {

		[Field ("UILargeContentViewerInteractionEnabledStatusDidChangeNotification")]
		[Notification]
		NSString InteractionEnabledStatusDidChangeNotification { get; }

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] IUILargeContentViewerInteractionDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUILargeContentViewerInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("gestureRecognizerForExclusionRelationship", ArgumentSemantic.Strong)]
		UIGestureRecognizer GestureRecognizerForExclusionRelationship { get; }

		[Static]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }
	}

	interface IUILargeContentViewerInteractionDelegate { }

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UILargeContentViewerInteractionDelegate {

		[Export ("largeContentViewerInteraction:didEndOnItem:atPoint:")]
		void DidEnd (UILargeContentViewerInteraction interaction, [NullAllowed] IUILargeContentViewerItem item, CGPoint point);

		[Export ("largeContentViewerInteraction:itemAtPoint:")]
		[return: NullAllowed]
		IUILargeContentViewerItem GetItem (UILargeContentViewerInteraction interaction, CGPoint point);

		[Export ("viewControllerForLargeContentViewerInteraction:")]
		UIViewController GetViewController (UILargeContentViewerInteraction interaction);
	}

	interface IUIMenuBuilder { }

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIMenuBuilder {

		[Abstract]
		[Export ("system")]
		UIMenuSystem System { get; }

		[Abstract]
		[Export ("menuForIdentifier:")]
		[return: NullAllowed]
		UIMenu GetMenu (string identifier);

		[Abstract]
		[Export ("actionForIdentifier:")]
		[return: NullAllowed]
		UIAction GetAction (string identifier);

		[Abstract]
		[Export ("commandForAction:propertyList:")]
		[return: NullAllowed]
		UICommand GetCommand (Selector action, [NullAllowed] NSObject propertyList);

		[Abstract]
		[Export ("replaceMenuForIdentifier:withMenu:")]
		void ReplaceMenu (string replacedIdentifier, UIMenu replacementMenu);

		[Abstract]
		[Export ("replaceChildrenOfMenuForIdentifier:fromChildrenBlock:")]
		void ReplaceChildrenOfMenu (string parentIdentifier, Func<UIMenuElement [], UIMenuElement []> childrenBlock);

		[Abstract]
		[Export ("insertSiblingMenu:beforeMenuForIdentifier:")]
		void InsertSiblingMenuBefore (UIMenu siblingMenu, string siblingIdentifier);

		[Abstract]
		[Export ("insertSiblingMenu:afterMenuForIdentifier:")]
		void InsertSiblingMenuAfter (UIMenu siblingMenu, string siblingIdentifier);

		[Abstract]
		[Export ("insertChildMenu:atStartOfMenuForIdentifier:")]
		void InsertChildMenuAtStart (UIMenu childMenu, string parentIdentifier);

		[Abstract]
		[Export ("insertChildMenu:atEndOfMenuForIdentifier:")]
		void InsertChildMenuAtEnd (UIMenu childMenu, string parentIdentifier);

		[Abstract]
		[Export ("removeMenuForIdentifier:")]
		void RemoveMenu (string removedIdentifier);
	}

	[NoWatch, iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIMenuSystem {

		[Static]
		[Export ("mainSystem")]
		UIMenuSystem MainSystem { get; }

		[Static]
		[Export ("contextSystem")]
		UIMenuSystem ContextSystem { get; }

		[Export ("setNeedsRebuild")]
		void SetNeedsRebuild ();

		[Export ("setNeedsRevalidate")]
		void SetNeedsRevalidate ();
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarAppearance))]
	interface UINavigationBarAppearance {

		[Export ("initWithIdiom:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIUserInterfaceIdiom idiom);

		[Export ("initWithBarAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIBarAppearance barAppearance);

		[Export ("titleTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakTitleTextAttributes { get; set; }

		[Wrap ("WeakTitleTextAttributes")]
		UIStringAttributes TitleTextAttributes { get; set; }

		[Export ("titlePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset TitlePositionAdjustment { get; set; }

		[Export ("largeTitleTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakLargeTitleTextAttributes { get; set; }

		[Wrap ("WeakLargeTitleTextAttributes")]
		UIStringAttributes LargeTitleTextAttributes { get; set; }

		[Export ("buttonAppearance", ArgumentSemantic.Copy)]
		UIBarButtonItemAppearance ButtonAppearance { get; set; }

		[Export ("doneButtonAppearance", ArgumentSemantic.Copy)]
		UIBarButtonItemAppearance DoneButtonAppearance { get; set; }

		[Export ("backButtonAppearance", ArgumentSemantic.Copy)]
		UIBarButtonItemAppearance BackButtonAppearance { get; set; }

		[Export ("backIndicatorImage", ArgumentSemantic.Strong)]
		UIImage BackIndicatorImage { get; }

		[Export ("backIndicatorTransitionMaskImage", ArgumentSemantic.Strong)]
		UIImage BackIndicatorTransitionMaskImage { get; }

		[Export ("setBackIndicatorImage:transitionMaskImage:")]
		void SetBackIndicatorImage ([NullAllowed] UIImage backIndicatorImage, [NullAllowed] UIImage backIndicatorTransitionMaskImage);
	}

	[NoWatch, iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	delegate NSDictionary UITextAttributesConversionHandler (NSDictionary textAttributes);

	[NoWatch, iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIScreenshotService {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIScreenshotServiceDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("windowScene", ArgumentSemantic.Weak)]
		UIWindowScene WindowScene { get; }
	}

	[NoWatch, iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
	delegate NSDictionary UIScreenshotServiceDelegatePdfHandler (NSData pdfData, nint indexOfCurrentPage, CGRect rectInCurrentPage);

	interface IUIScreenshotServiceDelegate { }

	[NoWatch, iOS (13, 0), TV (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIScreenshotServiceDelegate {

		[Export ("screenshotService:generatePDFRepresentationWithCompletion:")]
		void GeneratePdfRepresentation (UIScreenshotService screenshotService, UIScreenshotServiceDelegatePdfHandler completionHandler);
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UITextField))]
	interface UISearchTextField {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("tokens", ArgumentSemantic.Copy)]
		UISearchToken [] Tokens { get; set; }

		[Export ("insertToken:atIndex:")]
		void InsertToken (UISearchToken token, nint tokenIndex);

		[Export ("removeTokenAtIndex:")]
		void RemoveToken (nint tokenIndex);

		[Export ("positionOfTokenAtIndex:")]
		UITextPosition GetPositionOfToken (nint tokenIndex);

		[Export ("tokensInRange:")]
		UISearchToken [] GetTokens (UITextRange textRange);

		[Export ("textualRange")]
		UITextRange TextualRange { get; }

		[Export ("replaceTextualPortionOfRange:withToken:atIndex:")]
		void ReplaceTextualPortion (UITextRange textRange, UISearchToken token, nuint tokenIndex);

		[Export ("tokenBackgroundColor", ArgumentSemantic.Strong)]
		UIColor TokenBackgroundColor { get; set; }

		[Export ("allowsDeletingTokens")]
		bool AllowsDeletingTokens { get; set; }

		[Export ("allowsCopyingTokens")]
		bool AllowsCopyingTokens { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("searchSuggestions", ArgumentSemantic.Copy)]
		[NullAllowed]
		IUISearchSuggestion [] SearchSuggestions { get; set; }
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISearchToken {

		[Static]
		[Export ("tokenWithIcon:text:")]
		UISearchToken Create ([NullAllowed] UIImage icon, string text);

		[NullAllowed, Export ("representedObject", ArgumentSemantic.Strong)]
		NSObject RepresentedObject { get; set; }
	}

	interface IUISearchTextFieldDelegate { }

	[NoTV, iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UISearchTextFieldDelegate : UITextFieldDelegate {

		[Export ("searchTextField:itemProviderForCopyingToken:")]
		NSItemProvider GetItemProvider (UISearchTextField searchTextField, UISearchToken token);

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("searchTextField:didSelectSuggestion:")]
		void DidSelectSuggestion (UISearchTextField searchTextField, IUISearchSuggestion suggestion);
	}

	interface IUISearchTextFieldPasteItem { }

	[NoTV, iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UISearchTextFieldPasteItem : UITextPasteItem {

		[Abstract]
		[Export ("setSearchTokenResult:")]
		void SetSearchTokenResult (UISearchToken token);
	}

	[NoTV, iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIStatusBarManager {

		[Export ("statusBarStyle")]
		UIStatusBarStyle StatusBarStyle { get; }

		[Export ("statusBarHidden")]
		bool StatusBarHidden { [Bind ("isStatusBarHidden")] get; }

		[Export ("statusBarFrame")]
		CGRect StatusBarFrame { get; }
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITabBarItemStateAppearance {

		[Export ("titleTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakTitleTextAttributes { get; set; }

		[Wrap ("WeakTitleTextAttributes")]
		UIStringAttributes TitleTextAttributes { get; set; }

		[Export ("titlePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset TitlePositionAdjustment { get; set; }

		[NullAllowed, Export ("iconColor", ArgumentSemantic.Copy)]
		UIColor IconColor { get; set; }

		[Export ("badgePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset BadgePositionAdjustment { get; set; }

		[NullAllowed, Export ("badgeBackgroundColor", ArgumentSemantic.Copy)]
		UIColor BadgeBackgroundColor { get; set; }

		[Export ("badgeTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakBadgeTextAttributes { get; set; }

		[Wrap ("WeakBadgeTextAttributes")]
		UIStringAttributes BadgeTextAttributes { get; set; }

		[Export ("badgeTitlePositionAdjustment", ArgumentSemantic.Assign)]
		UIOffset BadgeTitlePositionAdjustment { get; set; }
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarAppearance))]
	interface UITabBarAppearance {

		[Export ("initWithIdiom:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIUserInterfaceIdiom idiom);

		[Export ("initWithBarAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIBarAppearance barAppearance);

		[Export ("stackedLayoutAppearance", ArgumentSemantic.Copy)]
		UITabBarItemAppearance StackedLayoutAppearance { get; set; }

		[Export ("inlineLayoutAppearance", ArgumentSemantic.Copy)]
		UITabBarItemAppearance InlineLayoutAppearance { get; set; }

		[Export ("compactInlineLayoutAppearance", ArgumentSemantic.Copy)]
		UITabBarItemAppearance CompactInlineLayoutAppearance { get; set; }

		[NullAllowed, Export ("selectionIndicatorTintColor", ArgumentSemantic.Copy)]
		UIColor SelectionIndicatorTintColor { get; set; }

		[NullAllowed, Export ("selectionIndicatorImage", ArgumentSemantic.Strong)]
		UIImage SelectionIndicatorImage { get; set; }

		[Export ("stackedItemPositioning", ArgumentSemantic.Assign)]
		UITabBarItemPositioning StackedItemPositioning { get; set; }

		[Export ("stackedItemWidth")]
		nfloat StackedItemWidth { get; set; }

		[Export ("stackedItemSpacing")]
		nfloat StackedItemSpacing { get; set; }
	}

	interface IUITextFormattingCoordinatorDelegate { }

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UITextFormattingCoordinatorDelegate {

		[Abstract]
		[Export ("updateTextAttributesWithConversionHandler:")]
		void UpdateTextAttributes (UITextAttributesConversionHandler conversionHandler);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextFormattingCoordinator
#if IOS
		: UIFontPickerViewControllerDelegate
#endif
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUITextFormattingCoordinatorDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Static]
		[Export ("fontPanelVisible")]
		bool FontPanelVisible { [Bind ("isFontPanelVisible")] get; }

		[Static]
		[Export ("textFormattingCoordinatorForWindowScene:")]
		UITextFormattingCoordinator GetTextFormattingCoordinator (UIWindowScene windowScene);

		[Export ("initWithWindowScene:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIWindowScene windowScene);

		[Export ("setSelectedAttributes:isMultiple:")]
		void SetSelectedAttributes (NSDictionary attributes, bool flag);

		[Static]
		[Export ("toggleFontPanel:")]
		void ToggleFontPanel (NSObject sender);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITextPlaceholder {

		[Export ("rects")]
		UITextSelectionRect [] Rects { get; }
	}

	interface IUITextInteractionDelegate { }

	[iOS (13, 0), NoTV, NoWatch]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UITextInteractionDelegate {

		[Export ("interactionShouldBegin:atPoint:")]
		bool ShouldBegin (UITextInteraction interaction, CGPoint point);

		[Export ("interactionWillBegin:")]
		void WillBegin (UITextInteraction interaction);

		[Export ("interactionDidEnd:")]
		void DidEnd (UITextInteraction interaction);
	}

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextInteraction : UIInteraction {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUITextInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("textInput", ArgumentSemantic.Weak)]
		IUITextInput TextInput { get; set; }

		[Export ("textInteractionMode")]
		UITextInteractionMode TextInteractionMode { get; }

		[Export ("gesturesForFailureRequirements")]
		UIGestureRecognizer [] GesturesForFailureRequirements { get; }

		[Static]
		[Export ("textInteractionForMode:")]
		UITextInteraction Create (UITextInteractionMode mode);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIBarAppearance))]
	interface UIToolbarAppearance {

		[Export ("initWithIdiom:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIUserInterfaceIdiom idiom);

		[Export ("initWithBarAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIBarAppearance barAppearance);

		[Export ("buttonAppearance", ArgumentSemantic.Copy)]
		UIBarButtonItemAppearance ButtonAppearance { get; set; }

		[Export ("doneButtonAppearance", ArgumentSemantic.Copy)]
		UIBarButtonItemAppearance DoneButtonAppearance { get; set; }
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIScene))]
	[DisableDefaultCtor]
	interface UIWindowScene {

		[Export ("initWithSession:connectionOptions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UISceneSession session, UISceneConnectionOptions connectionOptions);

		[Export ("screen")]
		UIScreen Screen { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("interfaceOrientation")]
		UIInterfaceOrientation InterfaceOrientation { get; }

		[Export ("coordinateSpace")]
		IUICoordinateSpace CoordinateSpace { get; }

		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }

		[NullAllowed, Export ("sizeRestrictions")]
		UISceneSizeRestrictions SizeRestrictions { get; }

		[Export ("windows")]
		UIWindow [] Windows { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("keyWindow", ArgumentSemantic.Strong)]
		UIWindow KeyWindow { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("activityItemsConfigurationSource", ArgumentSemantic.Weak)]
		IUIActivityItemsConfigurationProviding ActivityItemsConfigurationSource { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("statusBarManager")]
		UIStatusBarManager StatusBarManager { get; }

		// From UIWindowScene (UIScreenshotService)

		[NullAllowed, Export ("screenshotService")]
		UIScreenshotService ScreenshotService { get; }

		[NoWatch, TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("focusSystem")]
		UIFocusSystem FocusSystem { get; }

		[NoWatch]
		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[Export ("titlebar")]
		[NullAllowed]
		UITitlebar Titlebar { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("requestGeometryUpdateWithPreferences:errorHandler:")]
		void RequestGeometryUpdate (UIWindowSceneGeometryPreferences geometryPreferences, [NullAllowed] Action<NSError> errorHandler);

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("effectiveGeometry")]
		UIWindowSceneGeometry EffectiveGeometry { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("windowingBehaviors")]
		[NullAllowed]
		UISceneWindowingBehaviors WindowingBehaviors { get; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("fullScreen")]
		bool FullScreen { [Bind ("isFullScreen")] get; }

		[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("traitOverrides")]
		IUITraitOverrides TraitOverrides { get; }
	}

	interface IUIWindowSceneDelegate { }

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIWindowSceneDelegate : UISceneDelegate {

		[NullAllowed, Export ("window", ArgumentSemantic.Strong)]
		UIWindow Window { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("windowScene:didUpdateCoordinateSpace:interfaceOrientation:traitCollection:")]
		void DidUpdateCoordinateSpace (UIWindowScene windowScene, IUICoordinateSpace previousCoordinateSpace, UIInterfaceOrientation previousInterfaceOrientation, UITraitCollection previousTraitCollection);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("windowScene:performActionForShortcutItem:completionHandler:")]
		void PerformAction (UIWindowScene windowScene, UIApplicationShortcutItem shortcutItem, Action<bool> completionHandler);

		[Export ("windowScene:userDidAcceptCloudKitShareWithMetadata:")]
		void UserDidAcceptCloudKitShare (UIWindowScene windowScene, CKShareMetadata cloudKitShareMetadata);
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UISceneDestructionRequestOptions))]
	interface UIWindowSceneDestructionRequestOptions {

		[Export ("windowDismissalAnimation", ArgumentSemantic.Assign)]
		UIWindowSceneDismissalAnimation WindowDismissalAnimation { get; set; }
	}

	[TV (13, 0), iOS (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITabBarItemAppearance : NSCopying, NSSecureCoding {

		[Export ("initWithStyle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UITabBarItemAppearanceStyle style);

		[Export ("configureWithDefaultForStyle:")]
		void ConfigureWithDefault (UITabBarItemAppearanceStyle style);

		[Export ("normal", ArgumentSemantic.Strong)]
		UITabBarItemStateAppearance Normal { get; }

		[Export ("selected", ArgumentSemantic.Strong)]
		UITabBarItemStateAppearance Selected { get; }

		[Export ("disabled", ArgumentSemantic.Strong)]
		UITabBarItemStateAppearance Disabled { get; }

		[Export ("focused", ArgumentSemantic.Strong)]
		UITabBarItemStateAppearance Focused { get; }
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate UICollectionViewCell UICollectionViewDiffableDataSourceCellProvider (UICollectionView collectionView, NSIndexPath indexPath, NSObject itemIdentifier);

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate UICollectionReusableView UICollectionViewDiffableDataSourceSupplementaryViewProvider (UICollectionView collectionView, string elementKind, NSIndexPath indexPath);

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICollectionViewDiffableDataSource<SectionIdentifierType, ItemIdentifierType> : UICollectionViewDataSource
		where SectionIdentifierType : NSObject
		where ItemIdentifierType : NSObject {

		[Export ("initWithCollectionView:cellProvider:")]
		NativeHandle Constructor (UICollectionView collectionView, UICollectionViewDiffableDataSourceCellProvider cellProvider);

		[NullAllowed, Export ("supplementaryViewProvider", ArgumentSemantic.Copy)]
		UICollectionViewDiffableDataSourceSupplementaryViewProvider SupplementaryViewProvider { get; set; }

		[Export ("snapshot")]
		NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> Snapshot { get; }

		[Export ("applySnapshot:animatingDifferences:")]
		void ApplySnapshot (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, bool animatingDifferences);

		[Async]
		[Export ("applySnapshot:animatingDifferences:completion:")]
		void ApplySnapshot (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, bool animatingDifferences, [NullAllowed] Action completion);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("applySnapshotUsingReloadData:")]
		void ApplySnapshotUsingReloadData (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot);

		[Async]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("applySnapshotUsingReloadData:completion:")]
		void ApplySnapshotUsingReloadData (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, [NullAllowed] Action completion);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sectionIdentifierForIndex:")]
		[return: NullAllowed]
		SectionIdentifierType GetSectionIdentifier (nint index);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("indexForSectionIdentifier:")]
		nint GetIndex (SectionIdentifierType sectionIdentifier);

		[Export ("itemIdentifierForIndexPath:")]
		[return: NullAllowed]
		ItemIdentifierType GetItemIdentifier (NSIndexPath indexPath);

		[Export ("indexPathForItemIdentifier:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPath (ItemIdentifierType identifier);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("reorderingHandlers", ArgumentSemantic.Copy)]
		UICollectionViewDiffableDataSourceReorderingHandlers<SectionIdentifierType, ItemIdentifierType> ReorderingHandlers { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("applySnapshot:toSection:animatingDifferences:")]
		void ApplySnapshot (NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> snapshot, SectionIdentifierType sectionIdentifier, bool animatingDifferences);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("applySnapshot:toSection:animatingDifferences:completion:")]
		void ApplySnapshot (NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> snapshot, SectionIdentifierType sectionIdentifier, bool animatingDifferences, [NullAllowed] Action completion);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("snapshotForSection:")]
		NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> GetSnapshot (SectionIdentifierType section);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("sectionSnapshotHandlers", ArgumentSemantic.Copy)]
		UICollectionViewDiffableDataSourceSectionSnapshotHandlers<ItemIdentifierType> SectionSnapshotHandlers { get; set; }
	}

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate UITableViewCell UITableViewDiffableDataSourceCellProvider (UITableView tableView, NSIndexPath indexPath, NSObject obj);

	[NoWatch, TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITableViewDiffableDataSource<SectionIdentifierType, ItemIdentifierType> : UITableViewDataSource
		where SectionIdentifierType : NSObject
		where ItemIdentifierType : NSObject {

		[Export ("initWithTableView:cellProvider:")]
		NativeHandle Constructor (UITableView tableView, UITableViewDiffableDataSourceCellProvider cellProvider);

		[Export ("snapshot")]
		NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> Snapshot { get; }

		[Export ("applySnapshot:animatingDifferences:")]
		void ApplySnapshot (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, bool animatingDifferences);

		[Async]
		[Export ("applySnapshot:animatingDifferences:completion:")]
		void ApplySnapshot (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, bool animatingDifferences, [NullAllowed] Action completion);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("applySnapshotUsingReloadData:")]
		void ApplySnapshotUsingReloadData (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot);

		[Async]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("applySnapshotUsingReloadData:completion:")]
		void ApplySnapshotUsingReloadData (NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> snapshot, [NullAllowed] Action completion);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("sectionIdentifierForIndex:")]
		[return: NullAllowed]
		SectionIdentifierType GetSectionIdentifier (nint index);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("indexForSectionIdentifier:")]
		nint GetIndex (SectionIdentifierType sectionIdentifier);

		[Export ("itemIdentifierForIndexPath:")]
		[return: NullAllowed]
		ItemIdentifierType GetItemIdentifier (NSIndexPath indexPath);

		[Export ("indexPathForItemIdentifier:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPath (ItemIdentifierType identifier);

		[Export ("defaultRowAnimation", ArgumentSemantic.Assign)]
		UITableViewRowAnimation DefaultRowAnimation { get; set; }
	}

	[Static]
	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	interface UIActivityItemsConfigurationMetadataKey {
		[Field ("UIActivityItemsConfigurationMetadataKeyTitle")]
		NSString Title { get; }

		[Field ("UIActivityItemsConfigurationMetadataKeyMessageBody")]
		NSString MessageBody { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("UIActivityItemsConfigurationMetadataKeyLinkPresentationMetadata")]
		NSString LinkPresentationMetadata { get; }
	}

	delegate NSObject UIActivityItemsConfigurationMetadataProviderHandler (NSString activityItemsConfigurationMetadataKey);
	delegate NSObject UIActivityItemsConfigurationPerItemMetadataProviderHandler (nint index, NSString activityItemsConfigurationMetadataKey);
	delegate NSObject UIActivityItemsConfigurationPreviewProviderHandler (nint index, NSString activityItemsConfigurationPreviewIntent, CGSize suggestedSize);

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIActivityItemsConfiguration : UIActivityItemsConfigurationReading {

		[NullAllowed, Export ("localObject", ArgumentSemantic.Strong)]
		NSObject LocalObject { get; set; }

		[Export ("supportedInteractions", ArgumentSemantic.Copy)]
		NSString [] WeakSupportedInteractions { get; set; }

		[Advice ("Uses 'UIActivityItemsConfigurationMetadataKey' constants in the function handler.")]
		[NullAllowed, Export ("metadataProvider", ArgumentSemantic.Strong)]
		UIActivityItemsConfigurationMetadataProviderHandler MetadataProvider { get; set; }

		[Advice ("Uses 'UIActivityItemsConfigurationMetadataKey' constants in the function handler.")]
		[NullAllowed, Export ("perItemMetadataProvider", ArgumentSemantic.Strong)]
		UIActivityItemsConfigurationPerItemMetadataProviderHandler PerItemMetadataProvider { get; set; }

		[Advice ("Uses 'UIActivityItemsConfigurationPreviewIntent' enum constants in the function handler.")]
		[NullAllowed, Export ("previewProvider", ArgumentSemantic.Strong)]
		UIActivityItemsConfigurationPreviewProviderHandler PreviewProvider { get; set; }

		[NullAllowed, Export ("applicationActivitiesProvider", ArgumentSemantic.Strong)]
		Func<UIActivity []> ApplicationActivitiesProvider { get; set; }

		[Static]
		[Export ("activityItemsConfigurationWithObjects:")]
		UIActivityItemsConfiguration Create (INSItemProviderWriting [] objects);

		[Static]
		[Export ("activityItemsConfigurationWithItemProviders:")]
		UIActivityItemsConfiguration Create (NSItemProvider [] itemProviders);

		[Export ("initWithObjects:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INSItemProviderWriting [] objects);

		[Export ("initWithItemProviders:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSItemProvider [] itemProviders);
	}

	interface IUIActivityItemsConfigurationReading { }

	[NoWatch, NoTV, iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIActivityItemsConfigurationReading {

		[Abstract]
		[Export ("itemProvidersForActivityItemsConfiguration", ArgumentSemantic.Copy)]
		NSItemProvider [] ItemProvidersForActivityItemsConfiguration { get; }

		[Export ("activityItemsConfigurationSupportsInteraction:")]
		bool GetActivityItemsConfigurationSupportsInteraction (NSString activityItemsConfigurationInteraction);

		[Export ("activityItemsConfigurationMetadataForKey:")]
		[return: NullAllowed]
		NSObject GetActivityItemsConfigurationMetadata (NSString activityItemsConfigurationMetadataKey);

		[Export ("activityItemsConfigurationMetadataForItemAtIndex:key:")]
		[return: NullAllowed]
		NSObject GetActivityItemsConfigurationMetadata (nint index, NSString activityItemsConfigurationMetadataKey);

		[Export ("activityItemsConfigurationPreviewForItemAtIndex:intent:suggestedSize:")]
		[return: NullAllowed]
		NSItemProvider GetActivityItemsConfigurationPreview (nint index, NSString activityItemsConfigurationPreviewIntent, CGSize suggestedSize);

		[return: NullAllowed]
		[Export ("applicationActivitiesForActivityItemsConfiguration")]
		UIActivity [] GetApplicationActivitiesForActivityItemsConfiguration ();
	}

	[iOS (13, 0), TV (13, 0), NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneSizeRestrictions {

		[Export ("minimumSize", ArgumentSemantic.Assign)]
		CGSize MinimumSize { get; set; }

		[Export ("maximumSize", ArgumentSemantic.Assign)]
		CGSize MaximumSize { get; set; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("allowsFullScreen")]
		bool AllowsFullScreen { get; set; }
	}

	interface IUIContextMenuInteractionAnimating { }

	[NoWatch, TV (17, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIContextMenuInteractionAnimating {

		[Abstract]
		[NullAllowed, Export ("previewViewController")]
		UIViewController PreviewViewController { get; }

		[Abstract]
		[Export ("addAnimations:")]
		void AddAnimations (Action animations);

		[Abstract]
		[Export ("addCompletion:")]
		void AddCompletion (Action completion);
	}

	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[iOS (13, 4), NoWatch, TV (13, 4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIKey : NSCopying, NSCoding {

		[Export ("characters")]
		string Characters { get; }

		[Export ("charactersIgnoringModifiers")]
		string CharactersIgnoringModifiers { get; }

		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[Export ("keyCode")]
		UIKeyboardHidUsage KeyCode { get; }
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerInteraction : UIInteraction {

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUIPointerInteractionDelegate Delegate { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] IUIPointerInteractionDelegate @delegate);

		[Export ("invalidate")]
		void Invalidate ();
	}

	interface IUIPointerInteractionDelegate { }

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIPointerInteractionDelegate {

		[Export ("pointerInteraction:regionForRequest:defaultRegion:")]
		[return: NullAllowed]
		UIPointerRegion GetRegionForRequest (UIPointerInteraction interaction, UIPointerRegionRequest request, UIPointerRegion defaultRegion);

		[Export ("pointerInteraction:styleForRegion:")]
		[return: NullAllowed]
		UIPointerStyle GetStyleForRegion (UIPointerInteraction interaction, UIPointerRegion region);

		[Export ("pointerInteraction:willEnterRegion:animator:")]
		void WillEnterRegion (UIPointerInteraction interaction, UIPointerRegion region, IUIPointerInteractionAnimating animator);

		[Export ("pointerInteraction:willExitRegion:animator:")]
		void WillExitRegion (UIPointerInteraction interaction, UIPointerRegion region, IUIPointerInteractionAnimating animator);
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerRegionRequest {

		[Export ("location")]
		CGPoint Location { get; }

		[Export ("modifiers")]
		UIKeyModifierFlags Modifiers { get; }
	}

	interface IUIPointerInteractionAnimating { }

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface UIPointerInteractionAnimating {

		[Abstract]
		[Export ("addAnimations:")]
		void AddAnimations (Action animations);

		[Abstract]
		[Export ("addCompletion:")]
		void AddCompletion (Action<bool> completion);
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerRegion : NSCopying {

		[Export ("rect")]
		CGRect Rect { get; }

		[NullAllowed, Export ("identifier")]
		NSObject Identifier { get; }

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("latchingAxes", ArgumentSemantic.Assign)]
		UIAxis LatchingAxes { get; set; }

		[Static]
		[Export ("regionWithRect:identifier:")]
		UIPointerRegion Create (CGRect rect, [NullAllowed] NSObject identifier);
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIHoverStyle))]
	[DisableDefaultCtor]
	interface UIPointerStyle : NSCopying {

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("accessories", ArgumentSemantic.Copy)]
		UIPointerAccessory [] Accessories { get; set; }

		[Static]
		[Export ("styleWithEffect:shape:")]
		UIPointerStyle Create (UIPointerEffect effect, [NullAllowed] UIPointerShape shape);

		[Static]
		[Export ("styleWithShape:constrainedAxes:")]
		UIPointerStyle Create (UIPointerShape shape, UIAxis axes);

		[Static]
		[Export ("hiddenPointerStyle")]
		UIPointerStyle CreateHiddenPointerStyle ();

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("systemPointerStyle")]
		UIPointerStyle CreateSystemPointerStyle ();
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerEffect : NSCopying, UIHoverEffect {

		[Export ("preview", ArgumentSemantic.Copy)]
		UITargetedPreview Preview { get; }

		[Static]
		[Export ("effectWithPreview:")]
		UIPointerEffect Create (UITargetedPreview preview);
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPointerEffect))]
	interface UIPointerHighlightEffect {

	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPointerEffect))]
	interface UIPointerLiftEffect {

	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (UIPointerEffect))]
	interface UIPointerHoverEffect {

		[Export ("preferredTintMode", ArgumentSemantic.Assign)]
		UIPointerEffectTintMode PreferredTintMode { get; set; }

		[Export ("prefersShadow")]
		bool PrefersShadow { get; set; }

		[Export ("prefersScaledContent")]
		bool PrefersScaledContent { get; set; }
	}

	[NoWatch, NoTV, iOS (13, 4)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerShape : NSCopying {

		[Static]
		[Export ("shapeWithPath:")]
		UIPointerShape Create (UIBezierPath path);

		[Static]
		[Export ("shapeWithRoundedRect:")]
		UIPointerShape CreateRounded (CGRect rect);

		[Static]
		[Export ("shapeWithRoundedRect:cornerRadius:")]
		UIPointerShape CreateRounded (CGRect rect, nfloat cornerRadius);

		[Static]
		[Export ("beamWithPreferredLength:axis:")]
		UIPointerShape CreateBeam (nfloat preferredLength, UIAxis axis);
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> : NSCopying
		where ItemIdentifierType : NSObject {

		[Export ("appendItems:")]
		void AppendItems (ItemIdentifierType [] items);

		[Export ("appendItems:intoParentItem:")]
		void AppendItems (ItemIdentifierType [] items, [NullAllowed] ItemIdentifierType parentItem);

		[Export ("insertItems:beforeItem:")]
		void InsertItemsBefore (ItemIdentifierType [] items, ItemIdentifierType beforeIdentifier);

		[Export ("insertItems:afterItem:")]
		void InsertItemsAfter (ItemIdentifierType [] items, ItemIdentifierType afterIdentifier);

		[Export ("deleteItems:")]
		void DeleteItems (ItemIdentifierType [] items);

		[Export ("deleteAllItems")]
		void DeleteAllItems ();

		[Export ("expandItems:")]
		void ExpandItems (ItemIdentifierType [] items);

		[Export ("collapseItems:")]
		void CollapseItems (ItemIdentifierType [] items);

		[Export ("replaceChildrenOfParentItem:withSnapshot:")]
		void ReplaceChildren (ItemIdentifierType parentItem, NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> snapshot);

		[Export ("insertSnapshot:beforeItem:")]
		void InsertSnapshotBeforeItem (NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> snapshot, ItemIdentifierType item);

		[Export ("insertSnapshot:afterItem:")]
		ItemIdentifierType InsertSnapshotAfterItem (NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> snapshot, ItemIdentifierType item);

		[Export ("isExpanded:")]
		bool IsExpanded (ItemIdentifierType item);

		[Export ("isVisible:")]
		bool IsVisible (ItemIdentifierType item);

		[Export ("containsItem:")]
		bool ContainsItem (ItemIdentifierType item);

		[Export ("levelOfItem:")]
		nint GetLevel (ItemIdentifierType item);

		[Export ("indexOfItem:")]
		nint GetIndex (ItemIdentifierType item);

		[Export ("expandedItems")]
		ItemIdentifierType [] ExpandedItems { get; }

		[Export ("parentOfChildItem:")]
		[return: NullAllowed]
		ItemIdentifierType GetParent (ItemIdentifierType ofChildItem);

		[Export ("snapshotOfParentItem:")]
		NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> GetSnapshot (ItemIdentifierType parentItem);

		[Export ("snapshotOfParentItem:includingParentItem:")]
		NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> GetSnapshot (ItemIdentifierType parentItem, bool includingParentItem);

		[Export ("items", ArgumentSemantic.Strong)]
		ItemIdentifierType [] Items { get; }

		[Export ("rootItems", ArgumentSemantic.Strong)]
		ItemIdentifierType [] RootItems { get; }

		[Export ("visibleItems", ArgumentSemantic.Strong)]
		ItemIdentifierType [] VisibleItems { get; }

		[Export ("visualDescription")]
		string VisualDescription { get; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIBackgroundConfiguration : NSCopying, NSSecureCoding {

		[Static]
		[Export ("clearConfiguration")]
		UIBackgroundConfiguration ClearConfiguration { get; }

		[Static]
		[Export ("listPlainCellConfiguration")]
		UIBackgroundConfiguration ListPlainCellConfiguration { get; }

		[Static]
		[Export ("listPlainHeaderFooterConfiguration")]
		UIBackgroundConfiguration ListPlainHeaderFooterConfiguration { get; }

		[Static]
		[Export ("listGroupedCellConfiguration")]
		UIBackgroundConfiguration ListGroupedCellConfiguration { get; }

		[Static]
		[Export ("listGroupedHeaderFooterConfiguration")]
		UIBackgroundConfiguration ListGroupedHeaderFooterConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("listSidebarHeaderConfiguration")]
		UIBackgroundConfiguration ListSidebarHeaderConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("listSidebarCellConfiguration")]
		UIBackgroundConfiguration ListSidebarCellConfiguration { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("listAccompaniedSidebarCellConfiguration")]
		UIBackgroundConfiguration ListAccompaniedSidebarCellConfiguration { get; }

		[Export ("updatedConfigurationForState:")]
		UIBackgroundConfiguration GetUpdatedConfiguration (IUIConfigurationState state);

		[NullAllowed, Export ("customView", ArgumentSemantic.Strong)]
		UIView CustomView { get; set; }

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("backgroundInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets BackgroundInsets { get; set; }

		[Export ("edgesAddingLayoutMarginsToBackgroundInsets", ArgumentSemantic.Assign)]
		NSDirectionalRectEdge EdgesAddingLayoutMarginsToBackgroundInsets { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Strong)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed, Export ("backgroundColorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler BackgroundColorTransformer { get; set; }

		[Export ("resolvedBackgroundColorForTintColor:")]
		UIColor GetResolvedBackgroundColor (UIColor tintColor);

		[NullAllowed, Export ("visualEffect", ArgumentSemantic.Copy)]
		UIVisualEffect VisualEffect { get; set; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("imageContentMode", ArgumentSemantic.Assign)]
		UIViewContentMode ImageContentMode { get; set; }

		[NullAllowed, Export ("strokeColor", ArgumentSemantic.Strong)]
		UIColor StrokeColor { get; set; }

		[NullAllowed, Export ("strokeColorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler StrokeColorTransformer { get; set; }

		[Export ("resolvedStrokeColorForTintColor:")]
		UIColor GetResolvedStrokeColor (UIColor tintColor);

		[Export ("strokeWidth")]
		nfloat StrokeWidth { get; set; }

		[Export ("strokeOutset")]
		nfloat StrokeOutset { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UICellAccessory : NSCopying, NSSecureCoding {

		[Field ("UICellAccessoryStandardDimension")]
		nfloat StandardDimension { get; }

		[Export ("displayedState", ArgumentSemantic.Assign)]
		UICellAccessoryDisplayedState DisplayedState { get; set; }

		[Export ("hidden")]
		bool IsHidden { [Bind ("isHidden")] get; set; }

		[Export ("reservedLayoutWidth")]
		nfloat ReservedLayoutWidth { get; set; }

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Strong)]
		UIColor TintColor { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryDisclosureIndicator {

	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryCheckmark {

	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryDelete {

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Strong)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Copy)]
		Action ActionHandler { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryInsert {

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Strong)]
		UIColor BackgroundColor { get; set; }

		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Copy)]
		Action ActionHandler { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryReorder {

		[Export ("showsVerticalSeparator")]
		bool ShowsVerticalSeparator { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryMultiselect {

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Strong)]
		UIColor BackgroundColor { get; set; }
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	interface UICellAccessoryOutlineDisclosure {

		[Export ("style", ArgumentSemantic.Assign)]
		UICellAccessoryOutlineDisclosureStyle Style { get; set; }

		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Copy)]
		Action ActionHandler { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	[DisableDefaultCtor]
	interface UICellAccessoryLabel {

		[Export ("initWithText:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string text);

		[Export ("text")]
		string Text { get; }

		[Export ("font", ArgumentSemantic.Strong)]
		UIFont Font { get; set; }

		[Export ("adjustsFontForContentSizeCategory")]
		bool AdjustsFontForContentSizeCategory { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate nuint UICellAccessoryPosition (UICellAccessory [] accessories);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICellAccessory))]
	[DisableDefaultCtor]
	interface UICellAccessoryCustomView {

		[Export ("initWithCustomView:placement:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView customView, UICellAccessoryPlacement placement);

		[Export ("customView", ArgumentSemantic.Strong)]
		UIView CustomView { get; }

		[Export ("placement")]
		UICellAccessoryPlacement Placement { get; }

		[Export ("maintainsFixedSize")]
		bool MaintainsFixedSize { get; set; }

		[NullAllowed, Export ("position", ArgumentSemantic.Copy)]
		UICellAccessoryPosition Position { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewConfigurationState))]
	[DisableDefaultCtor]
	interface UICellConfigurationState {

		[Export ("initWithTraitCollection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UITraitCollection traitCollection);

		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }

		[Export ("expanded")]
		bool Expanded { [Bind ("isExpanded")] get; set; }

		[Export ("swiped")]
		bool Swiped { [Bind ("isSwiped")] get; set; }

		[Export ("reordering")]
		bool Reordering { [Bind ("isReordering")] get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cellDragState", ArgumentSemantic.Assign)]
		UICellConfigurationDragState CellDragState { get; set; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("cellDropState", ArgumentSemantic.Assign)]
		UICellConfigurationDropState CellDropState { get; set; }
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	delegate UISwipeActionsConfiguration UICollectionLayoutListSwipeActionsConfigurationProvider (NSIndexPath indexPath);

	[NoTV]
	[MacCatalyst (13, 1)]
	delegate UIListSeparatorConfiguration UICollectionLayoutListItemSeparatorHandler (NSIndexPath indexPath, UIListSeparatorConfiguration sectionSeparatorConfiguration);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICollectionLayoutListConfiguration : NSCopying {

		[Export ("initWithAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UICollectionLayoutListAppearance appearance);

		[Export ("appearance")]
		UICollectionLayoutListAppearance Appearance { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("showsSeparators")]
		bool ShowsSeparators { get; set; }

		[Watch (7, 4), NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("separatorConfiguration", ArgumentSemantic.Copy)]
		UIListSeparatorConfiguration SeparatorConfiguration { get; set; }

		[Watch (7, 4), NoTV, iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[NullAllowed]
		[Export ("itemSeparatorHandler", ArgumentSemantic.Copy)]
		UICollectionLayoutListItemSeparatorHandler ItemSeparatorHandler { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Assign)]
		UIColor BackgroundColor { get; set; }

		[Export ("headerMode", ArgumentSemantic.Assign)]
		UICollectionLayoutListHeaderMode HeaderMode { get; set; }

		[Export ("footerMode", ArgumentSemantic.Assign)]
		UICollectionLayoutListFooterMode FooterMode { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("headerTopPadding")]
		nfloat HeaderTopPadding { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("leadingSwipeActionsConfigurationProvider", ArgumentSemantic.Copy)]
		UICollectionLayoutListSwipeActionsConfigurationProvider LeadingSwipeActionsConfigurationProvider { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("trailingSwipeActionsConfigurationProvider", ArgumentSemantic.Copy)]
		UICollectionLayoutListSwipeActionsConfigurationProvider TrailingSwipeActionsConfigurationProvider { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void UICollectionViewCellRegistrationConfigurationHandler (UICollectionViewCell cell, NSIndexPath indexPath, NSObject item);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICollectionViewCellRegistration {

		[Static]
		[Export ("registrationWithCellClass:configurationHandler:")]
		UICollectionViewCellRegistration GetRegistration (Class cellClass, UICollectionViewCellRegistrationConfigurationHandler configurationHandler);

		[Static]
		[Wrap ("GetRegistration (new Class (cellType), configurationHandler)")]
		UICollectionViewCellRegistration GetRegistration (Type cellType, UICollectionViewCellRegistrationConfigurationHandler configurationHandler);

		[Static]
		[Export ("registrationWithCellNib:configurationHandler:")]
		UICollectionViewCellRegistration GetRegistration (UINib cellNib, UICollectionViewCellRegistrationConfigurationHandler configurationHandler);

		[NullAllowed, Export ("cellClass")]
		Class CellClass { get; }

		[NullAllowed, Wrap ("Class.Lookup (CellClass)")]
		Type CellType { get; }

		[NullAllowed, Export ("cellNib")]
		UINib CellNib { get; }

		[Export ("configurationHandler")]
		UICollectionViewCellRegistrationConfigurationHandler ConfigurationHandler { get; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void UICollectionViewSupplementaryRegistrationConfigurationHandler (UICollectionReusableView supplementaryView, string elementKind, NSIndexPath indexPath);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICollectionViewSupplementaryRegistration {

		[Static]
		[Export ("registrationWithSupplementaryClass:elementKind:configurationHandler:")]
		UICollectionViewSupplementaryRegistration GetRegistration (Class supplementaryClass, string elementKind, UICollectionViewSupplementaryRegistrationConfigurationHandler configurationHandler);

		[Static]
		[Wrap ("GetRegistration (new Class (supplementaryType), elementKind, configurationHandler)")]
		UICollectionViewSupplementaryRegistration GetRegistration (Type supplementaryType, string elementKind, UICollectionViewSupplementaryRegistrationConfigurationHandler configurationHandler);

		[Static]
		[Export ("registrationWithSupplementaryNib:elementKind:configurationHandler:")]
		UICollectionViewSupplementaryRegistration GetRegistration (UINib supplementaryNib, string elementKind, UICollectionViewSupplementaryRegistrationConfigurationHandler configurationHandler);

		[NullAllowed, Export ("supplementaryClass")]
		Class SupplementaryClass { get; }

		[NullAllowed, Wrap ("Class.Lookup (SupplementaryClass)")]
		Type SupplementaryType { get; }

		[NullAllowed, Export ("supplementaryNib")]
		UINib SupplementaryNib { get; }

		[Export ("elementKind")]
		string ElementKind { get; }

		[Export ("configurationHandler")]
		UICollectionViewSupplementaryRegistrationConfigurationHandler ConfigurationHandler { get; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UICollectionViewCell))]
	interface UICollectionViewListCell {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("defaultContentConfiguration")]
		UIListContentConfiguration DefaultContentConfiguration { get; }

		[Export ("indentationLevel")]
		nint IndentationLevel { get; set; }

		[Export ("indentationWidth")]
		nfloat IndentationWidth { get; set; }

		[Export ("indentsAccessories")]
		bool IndentsAccessories { get; set; }

		[Export ("accessories", ArgumentSemantic.Copy)]
		UICellAccessory [] Accessories { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("separatorLayoutGuide")]
		UILayoutGuide SeparatorLayoutGuide { get; }
	}

	interface IUIColorPickerViewControllerDelegate { }

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIColorPickerViewControllerDelegate {

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the 'DidSelectColor (UIColorPickerViewController, UIColor, bool)' overload instead.")]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the 'DidSelectColor (UIColorPickerViewController, UIColor, bool)' overload instead.")]
		[Export ("colorPickerViewControllerDidSelectColor:")]
		void DidSelectColor (UIColorPickerViewController viewController);

		[iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("colorPickerViewController:didSelectColor:continuously:")]
		void DidSelectColor (UIColorPickerViewController viewController, UIColor color, bool continuously);

		[Export ("colorPickerViewControllerDidFinish:")]
		void DidFinish (UIColorPickerViewController viewController);
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewController))]
	[DesignatedDefaultCtor]
	interface UIColorPickerViewController {
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIColorPickerViewControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("selectedColor", ArgumentSemantic.Strong)]
		UIColor SelectedColor { get; set; }

		[Export ("supportsAlpha")]
		bool SupportsAlpha { get; set; }
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIControl))]
	interface UIColorWell {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[Export ("supportsAlpha")]
		bool SupportsAlpha { get; set; }

		[NullAllowed, Export ("selectedColor", ArgumentSemantic.Strong)]
		UIColor SelectedColor { get; set; }
	}

	interface IUIConfigurationState { }

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface UIConfigurationState : NSCopying, NSSecureCoding {

		// Needs to be manually inlined in adopting classes
		// [Abstract]
		// [Export ("initWithTraitCollection:")]
		// NativeHandle Constructor (UITraitCollection traitCollection);

		[Abstract]
		[Export ("traitCollection", ArgumentSemantic.Strong)]
		UITraitCollection TraitCollection { get; set; }

		[Abstract]
		[Export ("customStateForKey:")]
		[return: NullAllowed]
		NSObject GetCustomState (string key);

		[Abstract]
		[Export ("setCustomState:forKey:")]
		void SetCustomState ([NullAllowed] NSObject customState, string key);

		[Abstract]
		[Export ("objectForKeyedSubscript:")]
		[return: NullAllowed]
		NSObject GetObject (string key);

		[Abstract]
		[Export ("setObject:forKeyedSubscript:")]
		void SetObject ([NullAllowed] NSObject obj, string key);
	}

	interface IUIContentConfiguration { }

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoMac]
	[Protocol]
	interface UIContentConfiguration : NSCopying {

		[Abstract]
		[Export ("makeContentView")]
		IUIContentView MakeContentView ();

		[Abstract]
		[Export ("updatedConfigurationForState:")]
		IUIContentConfiguration GetUpdatedConfiguration (IUIConfigurationState state);
	}

	interface IUIContentView { }

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[NoMac]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface UIContentView {

		[Abstract]
		[Export ("configuration", ArgumentSemantic.Copy)]
		IUIContentConfiguration Configuration { get; set; }

		[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("supportsConfiguration:")]
		bool SupportsConfiguration (IUIContentConfiguration configuration);
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void UIDeferredMenuElementCompletionHandler (UIMenuElement [] elements);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	delegate void UIDeferredMenuElementProviderHandler ([BlockCallback] UIDeferredMenuElementCompletionHandler completion);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIMenuElement))]
	[DisableDefaultCtor]
	interface UIDeferredMenuElement {

		[Static]
		[Export ("elementWithProvider:")]
		UIDeferredMenuElement Create (UIDeferredMenuElementProviderHandler elementProvider);

		[iOS (15, 0), MacCatalyst (15, 0), TV (15, 0)]
		[Static]
		[Export ("elementWithUncachedProvider:")]
		UIDeferredMenuElement CreateUncached (UIDeferredMenuElementProviderHandler elementProvider);
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface NSDiffableDataSourceSectionTransaction<SectionIdentifierType, ItemIdentifierType>
		where SectionIdentifierType : NSObject
		where ItemIdentifierType : NSObject {

		[Export ("sectionIdentifier")]
		SectionIdentifierType SectionIdentifier { get; }

		[Export ("initialSnapshot")]
		NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> InitialSnapshot { get; }

		[Export ("finalSnapshot")]
		NSDiffableDataSourceSectionSnapshot<ItemIdentifierType> FinalSnapshot { get; }

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Export ("difference")]
		NSOrderedCollectionDifference Difference { get; }
#endif
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface NSDiffableDataSourceTransaction<SectionIdentifierType, ItemIdentifierType>
		where SectionIdentifierType : NSObject
		where ItemIdentifierType : NSObject {

		[Export ("initialSnapshot")]
		NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> InitialSnapshot { get; }

		[Export ("finalSnapshot")]
		NSDiffableDataSourceSnapshot<SectionIdentifierType, ItemIdentifierType> FinalSnapshot { get; }

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Export ("difference")]
		NSOrderedCollectionDifference Difference { get; }
#endif

		[Export ("sectionTransactions")]
		NSDiffableDataSourceSectionTransaction<SectionIdentifierType, ItemIdentifierType> [] SectionTransactions { get; }
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIIndirectScribbleInteraction : UIInteraction {

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIIndirectScribbleInteractionDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIIndirectScribbleInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("handlingWriting")]
		bool HandlingWriting { [Bind ("isHandlingWriting")] get; }
	}

	interface IUIIndirectScribbleInteractionDelegate { }

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIIndirectScribbleInteractionDelegate {

		[Abstract]
		[Export ("indirectScribbleInteraction:requestElementsInRect:completion:")]
		void RequestElements (UIIndirectScribbleInteraction interaction, CGRect rect, Action<NSObject []> completion);

		[Abstract]
		[Export ("indirectScribbleInteraction:isElementFocused:")]
		bool IsElementFocused (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier);

		[Abstract]
		[Export ("indirectScribbleInteraction:frameForElement:")]
		CGRect GetFrameForElement (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier);

		[Abstract]
		[Export ("indirectScribbleInteraction:focusElementIfNeeded:referencePoint:completion:")]
		void FocusElementIfNeeded (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier, CGPoint focusReferencePoint, Action<IUITextInput> completion);

		[Export ("indirectScribbleInteraction:shouldDelayFocusForElement:")]
		bool ShouldDelayFocus (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier);

		[Export ("indirectScribbleInteraction:willBeginWritingInElement:")]
		void WillBeginWriting (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier);

		[Export ("indirectScribbleInteraction:didFinishWritingInElement:")]
		void DidFinishWriting (UIIndirectScribbleInteraction interaction, NSObject elementIdentifier);
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIListContentConfiguration : UIContentConfiguration, NSSecureCoding {

		[Static]
		[Export ("cellConfiguration")]
		UIListContentConfiguration CellConfiguration { get; }

		[Static]
		[Export ("subtitleCellConfiguration")]
		UIListContentConfiguration SubtitleCellConfiguration { get; }

		[Static]
		[Export ("valueCellConfiguration")]
		UIListContentConfiguration ValueCellConfiguration { get; }

		[Static]
		[Export ("plainHeaderConfiguration")]
		UIListContentConfiguration PlainHeaderConfiguration { get; }

		[Static]
		[Export ("plainFooterConfiguration")]
		UIListContentConfiguration PlainFooterConfiguration { get; }

		[Static]
		[Export ("groupedHeaderConfiguration")]
		UIListContentConfiguration GroupedHeaderConfiguration { get; }

		[Static]
		[Export ("groupedFooterConfiguration")]
		UIListContentConfiguration GroupedFooterConfiguration { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("prominentInsetGroupedHeaderConfiguration")]
		UIListContentConfiguration ProminentInsetGroupedHeaderConfiguration { get; }

		[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Static]
		[Export ("extraProminentInsetGroupedHeaderConfiguration")]
		UIListContentConfiguration ExtraProminentInsetGroupedHeaderConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sidebarCellConfiguration")]
		UIListContentConfiguration SidebarCellConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sidebarSubtitleCellConfiguration")]
		UIListContentConfiguration SidebarSubtitleCellConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("accompaniedSidebarCellConfiguration")]
		UIListContentConfiguration AccompaniedSidebarCellConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("accompaniedSidebarSubtitleCellConfiguration")]
		UIListContentConfiguration AccompaniedSidebarSubtitleCellConfiguration { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sidebarHeaderConfiguration")]
		UIListContentConfiguration SidebarHeaderConfiguration { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Export ("imageProperties")]
		UIListContentImageProperties ImageProperties { get; }

		[NullAllowed, Export ("text")]
		string Text { get; set; }

		[NullAllowed, Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }

		[Export ("textProperties")]
		UIListContentTextProperties TextProperties { get; }

		[NullAllowed, Export ("secondaryText")]
		string SecondaryText { get; set; }

		[NullAllowed, Export ("secondaryAttributedText", ArgumentSemantic.Copy)]
		NSAttributedString SecondaryAttributedText { get; set; }

		[Export ("secondaryTextProperties")]
		UIListContentTextProperties SecondaryTextProperties { get; }

		[Export ("axesPreservingSuperviewLayoutMargins", ArgumentSemantic.Assign)]
		UIAxis AxesPreservingSuperviewLayoutMargins { get; set; }

		[Export ("directionalLayoutMargins", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets DirectionalLayoutMargins { get; set; }

		[Export ("prefersSideBySideTextAndSecondaryText")]
		bool PrefersSideBySideTextAndSecondaryText { get; set; }

		[Export ("imageToTextPadding")]
		nfloat ImageToTextPadding { get; set; }

		[Export ("textToSecondaryTextHorizontalPadding")]
		nfloat TextToSecondaryTextHorizontalPadding { get; set; }

		[Export ("textToSecondaryTextVerticalPadding")]
		nfloat TextToSecondaryTextVerticalPadding { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface UIListContentView : UIContentView {

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIListContentConfiguration configuration);

		// UIContentView interface wants IUIContentConfiguration, covariant types can't come soon enough
		[Sealed, Export ("configuration", ArgumentSemantic.Copy)]
		UIListContentConfiguration ListContentConfiguration { get; set; }

		[NullAllowed, Export ("textLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide TextLayoutGuide { get; }

		[NullAllowed, Export ("secondaryTextLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide SecondaryTextLayoutGuide { get; }

		[NullAllowed, Export ("imageLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide ImageLayoutGuide { get; }
	}

	delegate UIColor UIConfigurationColorTransformerHandler (UIColor color);

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIListContentImageProperties : NSCopying, NSSecureCoding {

		[Field ("UIListContentImageStandardDimension")]
		nfloat StandardDimension { get; }

		[NullAllowed, Export ("preferredSymbolConfiguration", ArgumentSemantic.Copy)]
		UIImageSymbolConfiguration PreferredSymbolConfiguration { get; set; }

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Strong)]
		UIColor TintColor { get; set; }

		[NullAllowed, Export ("tintColorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler TintColorTransformer { get; set; }

		[Export ("resolvedTintColorForTintColor:")]
		UIColor GetResolvedTintColor (UIColor tintColor);

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("maximumSize", ArgumentSemantic.Assign)]
		CGSize MaximumSize { get; set; }

		[Export ("reservedLayoutSize", ArgumentSemantic.Assign)]
		CGSize ReservedLayoutSize { get; set; }

		[Export ("accessibilityIgnoresInvertColors")]
		bool AccessibilityIgnoresInvertColors { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIListContentTextProperties : NSCopying, NSSecureCoding {

		[Export ("font", ArgumentSemantic.Strong)]
		UIFont Font { get; set; }

		[Export ("color", ArgumentSemantic.Strong)]
		UIColor Color { get; set; }

		[NullAllowed, Export ("colorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler ColorTransformer { get; set; }

		[Export ("resolvedColor")]
		UIColor ResolvedColor { get; }

		[Export ("alignment", ArgumentSemantic.Assign)]
		UIListContentTextAlignment Alignment { get; set; }

		[Export ("lineBreakMode", ArgumentSemantic.Assign)]
		UILineBreakMode LineBreakMode { get; set; }

		[Export ("numberOfLines")]
		nint NumberOfLines { get; set; }

		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; set; }

		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set; }

		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[Export ("adjustsFontForContentSizeCategory")]
		bool AdjustsFontForContentSizeCategory { get; set; }

		[Export ("transform", ArgumentSemantic.Assign)]
		UIListContentTextTransform Transform { get; set; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("showsExpansionTextWhenTruncated")]
		bool ShowsExpansionTextWhenTruncated { get; set; }
	}

	interface UIPointerLockStateDidChangeEventArgs {
		[NoWatch, NoTV, iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed]
		[Export ("UIPointerLockStateSceneUserInfoKey")]
		UIScene Scene { get; }
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerLockState {

		[Field ("UIPointerLockStateDidChangeNotification")]
		[Notification (typeof (UIPointerLockStateDidChangeEventArgs))]
		NSString DidChangeNotification { get; }

		[Export ("locked")]
		bool Locked { [Bind ("isLocked")] get; }
	}

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIScribbleInteraction : UIInteraction {

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIScribbleInteractionDelegate @delegate);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIScribbleInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("handlingWriting")]
		bool HandlingWriting { [Bind ("isHandlingWriting")] get; }

		[Static]
		[Export ("pencilInputExpected")]
		bool PencilInputExpected { [Bind ("isPencilInputExpected")] get; }
	}

	interface IUIScribbleInteractionDelegate { }

	[NoWatch, NoTV, iOS (14, 0)]
	[MacCatalyst (14, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIScribbleInteractionDelegate {

		[Export ("scribbleInteraction:shouldBeginAtLocation:")]
		bool ShouldBegin (UIScribbleInteraction interaction, CGPoint location);

		[Export ("scribbleInteractionShouldDelayFocus:")]
		bool ShouldDelayFocus (UIScribbleInteraction interaction);

		[Export ("scribbleInteractionWillBeginWriting:")]
		void WillBeginWriting (UIScribbleInteraction interaction);

		[Export ("scribbleInteractionDidFinishWriting:")]
		void DidFinishWriting (UIScribbleInteraction interaction);
	}

	interface IUISearchSuggestion { }

	[TV (14, 0), NoWatch, iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[Protocol]
	interface UISearchSuggestion {

		[Abstract]
		[NullAllowed, Export ("localizedSuggestion")]
		NSString LocalizedSuggestion { get; }

		[return: NullAllowed]
		[Export ("localizedDescription")]
		NSString GetLocalizedDescription ();

		[return: NullAllowed]
		[Export ("iconImage")]
		UIImage GetIconImage ();

		[NoTV, iOS (16, 0)]
		[MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("localizedAttributedSuggestion")]
		NSAttributedString LocalizedAttributedSuggestion { get; }

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[NullAllowed, Export ("representedObject", ArgumentSemantic.Strong)]
		NSObject RepresentedObject { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Static, Partial]
	interface UIConfigurationColorTransformer {

		// Manually bound inside UIKit/UIConfigurationColorTransformer.cs
		[Internal]
		[Field ("UIConfigurationColorTransformerGrayscale")]
		IntPtr _Grayscale { get; }

#if XAMCORE_5_0
		[Internal]
#else
		[Obsolete ("Use the 'PreferredTint' property instead.")]
#endif
		[Field ("UIConfigurationColorTransformerPreferredTint")]
		IntPtr _PreferredTint { get; }

#if XAMCORE_5_0
		[Internal]
#else
		[Obsolete ("Use the 'MonochromeTint' property instead.")]
#endif
		[Field ("UIConfigurationColorTransformerMonochromeTint")]
		IntPtr _MonochromeTint { get; }
	}

	[TV (14, 0), NoWatch, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISearchSuggestionItem : UISearchSuggestion {

		[Static]
		[Export ("suggestionWithLocalizedSuggestion:")]
		UISearchSuggestionItem Create (NSString localizedSuggestion);

		[Static]
		[Export ("suggestionWithLocalizedSuggestion:descriptionString:")]
		UISearchSuggestionItem Create (NSString localizedSuggestion, [NullAllowed] string description);

		[Static]
		[Export ("suggestionWithLocalizedSuggestion:descriptionString:iconImage:")]
		UISearchSuggestionItem Create (NSString localizedSuggestion, [NullAllowed] string description, [NullAllowed] UIImage iconImage);

		[Export ("initWithLocalizedSuggestion:")]
		NativeHandle Constructor (NSString localizedSuggestion);

		[Export ("initWithLocalizedSuggestion:localizedDescription:")]
		NativeHandle Constructor (NSString localizedSuggestion, [NullAllowed] string description);

		[Export ("initWithLocalizedSuggestion:localizedDescription:iconImage:")]
		NativeHandle Constructor (NSString localizedSuggestion, [NullAllowed] string description, [NullAllowed] UIImage iconImage);

		// Inlined by the adopted protocol
		// [NullAllowed, Export ("localizedSuggestion")]
		// string LocalizedSuggestion { get; }

		// [NullAllowed, Export ("localizedDescription")]
		// string LocalizedDescription { get; }

		// [NullAllowed, Export ("iconImage")]
		// UIImage IconImage { get; }

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("suggestionWithLocalizedAttributedSuggestion:")]
		UISearchSuggestionItem Create (NSAttributedString suggestion);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("suggestionWithLocalizedAttributedSuggestion:descriptionString:")]
		UISearchSuggestionItem Create (NSAttributedString localizedSuggestion, [NullAllowed] string description);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("suggestionWithLocalizedAttributedSuggestion:descriptionString:iconImage:")]
		UISearchSuggestionItem Create (NSAttributedString localizedSuggestion, [NullAllowed] string description, [NullAllowed] UIImage iconImage);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithLocalizedAttributedSuggestion:")]
		NativeHandle Constructor (NSAttributedString localizedSuggestion);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithLocalizedAttributedSuggestion:localizedDescription:")]
		NativeHandle Constructor (NSAttributedString localizedSuggestion, [NullAllowed] string description);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithLocalizedAttributedSuggestion:localizedDescription:iconImage:")]
		NativeHandle Constructor (NSAttributedString localizedSuggestion, [NullAllowed] string description, [NullAllowed] UIImage iconImage);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("localizedAttributedSuggestion")]
		[NullAllowed]
		new NSAttributedString LocalizedAttributedSuggestion { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("representedObject", ArgumentSemantic.Strong)]
		new NSObject RepresentedObject { get; set; }
	}

	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIViewConfigurationState : UIConfigurationState {

		[Export ("initWithTraitCollection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UITraitCollection traitCollection);

		[Export ("traitCollection", ArgumentSemantic.Strong)]
		new UITraitCollection TraitCollection { get; set; }

		[Export ("disabled")]
		bool Disabled { [Bind ("isDisabled")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("focused")]
		bool Focused { [Bind ("isFocused")] get; set; }

		[Watch (8, 0), TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("pinned")]
		bool Pinned { [Bind ("isPinned")] get; set; }
	}

	// TODO: Our trampolines generator does not support generic delegate definitions
	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewDiffableDataSourceSectionSnapshotHandlers<ItemType> : NSCopying
		where ItemType : NSObject {

		// 	[NullAllowed, Export ("shouldExpandItemHandler", ArgumentSemantic.Copy)]
		// 	Func<ItemType, bool> ShouldExpandItemHandler { get; set; }

		// 	[NullAllowed, Export ("willExpandItemHandler", ArgumentSemantic.Copy)]
		// 	Action<ItemType> WillExpandItemHandler { get; set; }

		// 	[NullAllowed, Export ("shouldCollapseItemHandler", ArgumentSemantic.Copy)]
		// 	Func<ItemType, bool> ShouldCollapseItemHandler { get; set; }

		// 	[NullAllowed, Export ("willCollapseItemHandler", ArgumentSemantic.Copy)]
		// 	Action<ItemType> WillCollapseItemHandler { get; set; }

		// 	[NullAllowed, Export ("snapshotForExpandingParentItemHandler", ArgumentSemantic.Copy)]
		// 	Func<ItemType, NSDiffableDataSourceSectionSnapshot<ItemType>, NSDiffableDataSourceSectionSnapshot<ItemType>> SnapshotForExpandingParentItemHandler { get; set; }
	}

	// TODO: Our trampolines generator does not support generic delegate definitions
	[NoWatch, TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewDiffableDataSourceReorderingHandlers<SectionType, ItemType> : NSCopying
		where SectionType : NSObject
		where ItemType : NSObject {

		// [NullAllowed, Export ("canReorderItemHandler", ArgumentSemantic.Copy)]
		// Func<ItemType, bool> CanReorderItemHandler { get; set; }

		// [NullAllowed, Export ("willReorderHandler", ArgumentSemantic.Copy)]
		// Action<NSDiffableDataSourceTransaction<SectionType, ItemType>> WillReorderHandler { get; set; }

		// [NullAllowed, Export ("didReorderHandler", ArgumentSemantic.Copy)]
		// Action<NSDiffableDataSourceTransaction<SectionType, ItemType>> DidReorderHandler { get; set; }
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[NoWatch]
	[Native]
	public enum UIListContentTextTransform : long {
		None,
		Uppercase,
		Lowercase,
		Capitalized,
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIEventAttribution : NSCopying {

		[Export ("sourceIdentifier")]
		byte SourceIdentifier { get; }

		[Export ("destinationURL", ArgumentSemantic.Copy)]
		NSUrl DestinationUrl { get; }

		[NullAllowed, Export ("reportEndpoint", ArgumentSemantic.Copy)]
		NSUrl ReportEndpoint { get; }

		[Export ("sourceDescription")]
		string SourceDescription { get; }

		[Export ("purchaser")]
		string Purchaser { get; }

		[Export ("initWithSourceIdentifier:destinationURL:sourceDescription:purchaser:")]
		NativeHandle Constructor (byte sourceIdentifier, NSUrl destinationUrl, string sourceDescription, string purchaser);
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (UIView))]
	interface UIEventAttributionView {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	public enum UIListSeparatorVisibility : long {
		Automatic,
		Visible,
		Hidden,
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIListSeparatorConfiguration : NSCopying, NSSecureCoding {

		[Export ("initWithListAppearance:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UICollectionLayoutListAppearance listAppearance);

		[Export ("topSeparatorVisibility", ArgumentSemantic.Assign)]
		UIListSeparatorVisibility TopSeparatorVisibility { get; set; }

		[Export ("bottomSeparatorVisibility", ArgumentSemantic.Assign)]
		UIListSeparatorVisibility BottomSeparatorVisibility { get; set; }

		[Export ("topSeparatorInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets TopSeparatorInsets { get; set; }

		[Export ("bottomSeparatorInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets BottomSeparatorInsets { get; set; }

		[Export ("color", ArgumentSemantic.Strong)]
		UIColor Color { get; set; }

		[Export ("multipleSelectionColor", ArgumentSemantic.Strong)]
		UIColor MultipleSelectionColor { get; set; }

		[Watch (8, 0), NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("visualEffect", ArgumentSemantic.Copy)]
		UIVisualEffect VisualEffect { get; set; }
	}

	[NoWatch, NoTV, iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPrinterDestination : NSSecureCoding {

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; set; }

		[NullAllowed, Export ("txtRecord", ArgumentSemantic.Copy)]
		NSData TxtRecord { get; set; }
	}

	interface IUIActivityItemsConfigurationProviding { }

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface UIActivityItemsConfigurationProviding {

		[Abstract]
		[Export ("activityItemsConfiguration", ArgumentSemantic.Strong)]
		[NullAllowed]
		IUIActivityItemsConfigurationReading ActivityItemsConfiguration { get; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	delegate bool UIBandSelectionInteractionShouldBeginHandler (UIBandSelectionInteraction interaction, CGPoint point);

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIBandSelectionInteraction : UIInteraction {

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("state")]
		UIBandSelectionInteractionState State { get; }

		[Export ("selectionRect")]
		CGRect SelectionRect { get; }

		[Export ("initialModifierFlags")]
		UIKeyModifierFlags InitialModifierFlags { get; }

		[NullAllowed, Export ("shouldBeginHandler", ArgumentSemantic.Copy)]
		UIBandSelectionInteractionShouldBeginHandler ShouldBeginHandler { get; set; }

		[Export ("initWithSelectionHandler:")]
		NativeHandle Constructor (Action<UIBandSelectionInteraction> selectionHandler);
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	delegate NSDictionary UIConfigurationTextAttributesTransformerHandler (NSDictionary textAttributes);

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIButtonConfiguration : NSCopying, NSSecureCoding {

		[Static]
		[Export ("plainButtonConfiguration")]
		UIButtonConfiguration PlainButtonConfiguration { get; }

		[Static]
		[Export ("tintedButtonConfiguration")]
		UIButtonConfiguration TintedButtonConfiguration { get; }

		[Static]
		[Export ("grayButtonConfiguration")]
		UIButtonConfiguration GrayButtonConfiguration { get; }

		[Static]
		[Export ("filledButtonConfiguration")]
		UIButtonConfiguration FilledButtonConfiguration { get; }

		[Static]
		[Export ("borderlessButtonConfiguration")]
		UIButtonConfiguration BorderlessButtonConfiguration { get; }

		[Static]
		[Export ("borderedButtonConfiguration")]
		UIButtonConfiguration BorderedButtonConfiguration { get; }

		[Static]
		[Export ("borderedTintedButtonConfiguration")]
		UIButtonConfiguration BorderedTintedButtonConfiguration { get; }

		[Static]
		[Export ("borderedProminentButtonConfiguration")]
		UIButtonConfiguration BorderedProminentButtonConfiguration { get; }

		[Export ("updatedConfigurationForButton:")]
		UIButtonConfiguration GetUpdatedConfiguration (UIButton button);

		[Export ("background", ArgumentSemantic.Strong)]
		UIBackgroundConfiguration Background { get; set; }

		[Export ("cornerStyle", ArgumentSemantic.Assign)]
		UIButtonConfigurationCornerStyle CornerStyle { get; set; }

		[Export ("buttonSize", ArgumentSemantic.Assign)]
		UIButtonConfigurationSize ButtonSize { get; set; }

		[Export ("macIdiomStyle", ArgumentSemantic.Assign)]
		UIButtonConfigurationMacIdiomStyle MacIdiomStyle { get; set; }

		[NullAllowed, Export ("baseForegroundColor", ArgumentSemantic.Strong)]
		UIColor BaseForegroundColor { get; set; }

		[NullAllowed, Export ("baseBackgroundColor", ArgumentSemantic.Strong)]
		UIColor BaseBackgroundColor { get; set; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("imageColorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler ImageColorTransformer { get; set; }

		[NullAllowed, Export ("preferredSymbolConfigurationForImage", ArgumentSemantic.Copy)]
		UIImageSymbolConfiguration PreferredSymbolConfigurationForImage { get; set; }

		[Export ("showsActivityIndicator")]
		bool ShowsActivityIndicator { get; set; }

		[NullAllowed, Export ("activityIndicatorColorTransformer", ArgumentSemantic.Copy)]
		UIConfigurationColorTransformerHandler ActivityIndicatorColorTransformer { get; set; }

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[NullAllowed, Export ("titleTextAttributesTransformer", ArgumentSemantic.Copy)]
		UIConfigurationTextAttributesTransformerHandler TitleTextAttributesTransformer { get; set; }

		[TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed, Export ("titleLineBreakMode", ArgumentSemantic.Assign)]
		UILineBreakMode TitleLineBreakMode { get; set; }

		[NullAllowed, Export ("subtitle")]
		string Subtitle { get; set; }

		[NullAllowed, Export ("attributedSubtitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedSubtitle { get; set; }

		[NullAllowed, Export ("subtitleTextAttributesTransformer", ArgumentSemantic.Copy)]
		UIConfigurationTextAttributesTransformerHandler SubtitleTextAttributesTransformer { get; set; }

		[TV (16, 4), iOS (16, 4), MacCatalyst (16, 4)]
		[NullAllowed, Export ("subtitleLineBreakMode", ArgumentSemantic.Assign)]
		UILineBreakMode SubtitleLineBreakMode { get; set; }

		[Export ("contentInsets", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets ContentInsets { get; set; }

		[Export ("setDefaultContentInsets")]
		void SetDefaultContentInsets ();

		[Export ("imagePlacement", ArgumentSemantic.Assign)]
		NSDirectionalRectEdge ImagePlacement { get; set; }

		[Export ("imagePadding")]
		nfloat ImagePadding { get; set; }

		[Export ("titlePadding")]
		nfloat TitlePadding { get; set; }

		[Export ("titleAlignment", ArgumentSemantic.Assign)]
		UIButtonConfigurationTitleAlignment TitleAlignment { get; set; }

		[Export ("automaticallyUpdateForSelection")]
		bool AutomaticallyUpdateForSelection { get; set; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("indicator", ArgumentSemantic.Assign)]
		UIButtonConfigurationIndicator Indicator { get; set; }

		[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("indicatorColorTransformer", ArgumentSemantic.Copy)]
		[NullAllowed]
		UIConfigurationColorTransformerHandler IndicatorColorTransformer { get; set; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIFocusEffect : NSCopying {

		[Static]
		[Export ("effect")]
		UIFocusEffect Create ();
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIFocusEffect))]
	[DisableDefaultCtor]
	interface UIFocusHaloEffect {

		[Static]
		[Export ("effectWithRect:")]
		UIFocusHaloEffect Create (CGRect rect);

		[Static]
		[Export ("effectWithRoundedRect:cornerRadius:curve:")]
		UIFocusHaloEffect Create (CGRect rect, nfloat cornerRadius, string curve);

		[Static]
		[Export ("effectWithPath:")]
		UIFocusHaloEffect Create (UIBezierPath bezierPath);

		[NullAllowed, Export ("containerView", ArgumentSemantic.Weak)]
		UIView ContainerView { get; set; }

		[NullAllowed, Export ("referenceView", ArgumentSemantic.Weak)]
		UIView ReferenceView { get; set; }

		[Export ("position", ArgumentSemantic.Assign)]
		UIFocusHaloEffectPosition Position { get; set; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UITrackingLayoutGuide))]
	interface UIKeyboardLayoutGuide {

		[Export ("followsUndockedKeyboard")]
		bool FollowsUndockedKeyboard { get; set; }

		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("usesBottomSafeArea")]
		bool UsesBottomSafeArea { get; set; }

		[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("keyboardDismissPadding")]
		nfloat KeyboardDismissPadding { get; set; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISheetPresentationControllerDetent {

		[Field ("UISheetPresentationControllerAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Field ("UISheetPresentationControllerDetentInactive")]
		nfloat DetentInactive { get; }

		[Static]
		[Export ("mediumDetent")]
		UISheetPresentationControllerDetent CreateMediumDetent ();

		[Static]
		[Export ("largeDetent")]
		UISheetPresentationControllerDetent CreateLargeDetent ();


		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("identifier", ArgumentSemantic.Strong)]
		string Identifier { get; }

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Static]
		[Export ("customDetentWithIdentifier:resolver:")]
		UISheetPresentationControllerDetent Create ([NullAllowed] string identifier, Func<IUISheetPresentationControllerDetentResolutionContext, nfloat> resolver);

		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("resolvedValueInContext:")]
		nfloat GetResolvedValue (IUISheetPresentationControllerDetentResolutionContext context);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface UIToolTipInteraction : UIInteraction {

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIToolTipInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed, Export ("defaultToolTip")]
		string DefaultToolTip { get; set; }

		[Export ("initWithDefaultToolTip:")]
		NativeHandle Constructor (string defaultToolTip);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIToolTipConfiguration {

		[Export ("toolTip")]
		string ToolTip { get; }

		[Export ("sourceRect")]
		CGRect SourceRect { get; }

		[Static]
		[Export ("configurationWithToolTip:")]
		UIToolTipConfiguration Create (string toolTip);

		[Static]
		[Export ("configurationWithToolTip:inRect:")]
		UIToolTipConfiguration Create (string toolTip, CGRect sourceRect);
	}

	interface IUIToolTipInteractionDelegate { }

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIToolTipInteractionDelegate {

		[Export ("toolTipInteraction:configurationAtPoint:")]
		[return: NullAllowed]
		UIToolTipConfiguration GetConfiguration (UIToolTipInteraction interaction, CGPoint point);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UILayoutGuide))]
	interface UITrackingLayoutGuide {

		[Export ("setConstraints:activeWhenNearEdge:")]
		void SetConstraintsActiveWhenNear (NSLayoutConstraint [] trackingConstraints, NSDirectionalRectEdge edge);

		[Export ("constraintsActiveWhenNearEdge:")]
		NSLayoutConstraint [] GetConstraintsActiveWhenNear (NSDirectionalRectEdge edge);

		[Export ("setConstraints:activeWhenAwayFromEdge:")]
		void SetConstraintsActiveWhenAway (NSLayoutConstraint [] trackingConstraints, NSDirectionalRectEdge edge);

		[Export ("constraintsActiveWhenAwayFromEdge:")]
		NSLayoutConstraint [] GetConstraintsActiveWhenAway (NSDirectionalRectEdge edge);

		[Export ("removeAllTrackedConstraints")]
		void RemoveAllTrackedConstraints ();
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	delegate UIWindowSceneActivationConfiguration UIWindowSceneActivationActionConfigurationProvider (UIWindowSceneActivationAction action);

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UIAction))]
	[DisableDefaultCtor]
	interface UIWindowSceneActivationAction {

		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[Static]
		[Export ("actionWithIdentifier:alternateAction:configurationProvider:")]
		UIWindowSceneActivationAction Create ([NullAllowed][BindAs (typeof (UIActionIdentifier))] NSString identifier, [NullAllowed] UIAction alternateAction, UIWindowSceneActivationActionConfigurationProvider configurationProvider);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIWindowSceneActivationConfiguration {

		[Export ("userActivity", ArgumentSemantic.Strong)]
		NSUserActivity UserActivity { get; }

		[NullAllowed, Export ("options", ArgumentSemantic.Strong)]
		UIWindowSceneActivationRequestOptions Options { get; set; }

		[NullAllowed, Export ("preview", ArgumentSemantic.Strong)]
		UITargetedPreview Preview { get; set; }

		[Export ("initWithUserActivity:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUserActivity userActivity);
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	delegate UIWindowSceneActivationConfiguration UIWindowSceneActivationInteractionConfigurationProvider (UIWindowSceneActivationInteraction interaction, CGPoint location);

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIWindowSceneActivationInteraction : UIInteraction {

		[Export ("initWithConfigurationProvider:errorHandler:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIWindowSceneActivationInteractionConfigurationProvider configurationProvider, Action<NSError> errorHandler);
	}

	[TV (15, 0), NoWatch, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (UISceneActivationRequestOptions))]
	interface UIWindowSceneActivationRequestOptions {

		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use the property 'Placement' instead.")]
		[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use the property 'Placement' instead.")]
		[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use the property 'Placement' instead.")]
		[Export ("preferredPresentationStyle", ArgumentSemantic.Assign)]
		UIWindowScenePresentationStyle PreferredPresentationStyle { get; set; }

		[NullAllowed]
		[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("placement", ArgumentSemantic.Copy)]
		UIWindowScenePlacement Placement { get; set; }
	}

	[NoWatch, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIPointerAccessory : NSCopying {

		[Export ("shape", ArgumentSemantic.Copy)]
		UIPointerShape Shape { get; }

		[Export ("position")]
		UIPointerAccessoryPosition Position { get; }

		[Export ("orientationMatchesAngle")]
		bool OrientationMatchesAngle { get; set; }

		[Static]
		[Export ("accessoryWithShape:position:")]
		UIPointerAccessory Create (UIPointerShape shape, UIPointerAccessoryPosition position);

		[Static]
		[Export ("arrowAccessoryWithPosition:")]
		UIPointerAccessory CreateArrow (UIPointerAccessoryPosition position);
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Native]
	public enum UITitlebarTitleVisibility : long {
		Visible,
		Hidden,
	}

	[MacCatalyst (14, 0)]
	[NoiOS]
	[NoTV]
	[NoWatch]
	[Native]
	public enum UITitlebarToolbarStyle : long {
		Automatic,
		Expanded,
		Preference,
		Unified,
		UnifiedCompact,
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface UITitlebar {
		[Export ("titleVisibility", ArgumentSemantic.Assign)]
		UITitlebarTitleVisibility TitleVisibility { get; set; }

		[MacCatalyst (14, 0)]
		[Export ("toolbarStyle", ArgumentSemantic.Assign)]
		UITitlebarToolbarStyle ToolbarStyle { get; set; }

		[MacCatalyst (14, 0)]
		[Export ("separatorStyle", ArgumentSemantic.Assign)]
		UITitlebarSeparatorStyle SeparatorStyle { get; set; }

		[NullAllowed, Export ("toolbar", ArgumentSemantic.Strong)]
		NSToolbar Toolbar { get; set; }

		[Export ("autoHidesToolbarInFullScreen")]
		bool AutoHidesToolbarInFullScreen { get; set; }

		[NullAllowed, Export ("representedURL", ArgumentSemantic.Copy)]
		NSUrl RepresentedUrl { get; set; }
	}

	[NoWatch, TV (15, 4), iOS (15, 4), MacCatalyst (15, 4)]
	[BaseType (typeof (UICellAccessory))]
	[DesignatedDefaultCtor]
	interface UICellAccessoryDetail {

		[NullAllowed, Export ("actionHandler", ArgumentSemantic.Copy)]
		Action ActionHandler { get; set; }
	}

	interface IUICalendarSelectionMultiDateDelegate { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UICalendarSelectionMultiDateDelegate {
		[Abstract]
		[Export ("multiDateSelection:didSelectDate:")]
		void DidSelectDate (UICalendarSelectionMultiDate selection, NSDateComponents dateComponents);

		[Abstract]
		[Export ("multiDateSelection:didDeselectDate:")]
		void DidDeselectDate (UICalendarSelectionMultiDate selection, NSDateComponents dateComponents);

		[Export ("multiDateSelection:canSelectDate:")]
		bool CanSelectDate (UICalendarSelectionMultiDate selection, NSDateComponents dateComponents);

		[Export ("multiDateSelection:canDeselectDate:")]
		bool CanDeselectDate (UICalendarSelectionMultiDate selection, NSDateComponents dateComponents);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UICalendarSelection))]
	interface UICalendarSelectionMultiDate {
		[Export ("selectedDates", ArgumentSemantic.Copy)]
		NSDateComponents [] SelectedDates { get; set; }

		[Export ("setSelectedDates:animated:")]
		void SetSelectedDates (NSDateComponents [] selectedDates, bool animated);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUICalendarSelectionMultiDateDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("initWithDelegate:")]
		NativeHandle Constructor ([NullAllowed] IUICalendarSelectionMultiDateDelegate @delegate);
	}

	interface IUICalendarSelectionSingleDateDelegate { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UICalendarSelectionSingleDateDelegate {
		[Abstract]
		[Export ("dateSelection:didSelectDate:")]
		void DidSelectDate (UICalendarSelectionSingleDate selection, [NullAllowed] NSDateComponents dateComponents);

		[Export ("dateSelection:canSelectDate:")]
		bool CanSelectDate (UICalendarSelectionSingleDate selection, [NullAllowed] NSDateComponents dateComponents);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UICalendarSelection {
		[Export ("updateSelectableDates")]
		void UpdateSelectableDates ();
	}


	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UICalendarSelection))]
	interface UICalendarSelectionSingleDate {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUICalendarSelectionSingleDateDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[NullAllowed, Export ("selectedDate", ArgumentSemantic.Copy)]
		NSDateComponents SelectedDate { get; set; }

		[Export ("setSelectedDate:animated:")]
		void SetSelectedDate ([NullAllowed] NSDateComponents selectedDate, bool animated);

		[Export ("initWithDelegate:")]
		NativeHandle Constructor ([NullAllowed] IUICalendarSelectionSingleDateDelegate @delegate);
	}

	interface IUICalendarViewDelegate { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UICalendarViewDelegate {
		[Abstract]
		[Export ("calendarView:decorationForDateComponents:")]
		[return: NullAllowed]
		UICalendarViewDecoration GetDecoration (UICalendarView calendarView, NSDateComponents dateComponents);

		[iOS (16, 2), MacCatalyst (16, 0)]
		[Export ("calendarView:didChangeVisibleDateComponentsFrom:")]
		void DidChangeVisibleDateComponents (UICalendarView calendarView, NSDateComponents previousDateComponents);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface UICalendarViewDecoration {
		[Export ("initWithImage:color:size:")]
		NativeHandle Constructor ([NullAllowed] UIImage image, [NullAllowed] UIColor color, UICalendarViewDecorationSize size);

		[Export ("initWithCustomViewProvider:")]
		NativeHandle Constructor (Func<UIView> customViewProvider);

		[Static]
		[Export ("decorationWithColor:size:")]
		UICalendarViewDecoration Create ([NullAllowed] UIColor color, UICalendarViewDecorationSize size);

		[Static]
		[Export ("decorationWithImage:")]
		UICalendarViewDecoration Create ([NullAllowed] UIImage image);

		[Static]
		[Export ("decorationWithImage:color:size:")]
		UICalendarViewDecoration Create ([NullAllowed] UIImage image, [NullAllowed] UIColor color, UICalendarViewDecorationSize size);

		[Static]
		[Export ("decorationWithCustomViewProvider:")]
		UICalendarViewDecoration Create (Func<UIView> customViewProvider);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIView))]
	interface UICalendarView {

		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUICalendarViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("selectionBehavior", ArgumentSemantic.Strong)]
		UICalendarSelection SelectionBehavior { get; set; }

		[Export ("locale", ArgumentSemantic.Strong)]
		NSLocale Locale { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[NullAllowed, Export ("timeZone", ArgumentSemantic.Strong)]
		NSTimeZone TimeZone { get; set; }

		[Export ("fontDesign")]
		string FontDesign { get; set; }

		[Export ("availableDateRange", ArgumentSemantic.Copy)]
		NSDateInterval AvailableDateRange { get; set; }

		[Export ("visibleDateComponents", ArgumentSemantic.Copy)]
		NSDateComponents VisibleDateComponents { get; set; }

		[Export ("setVisibleDateComponents:animated:")]
		void SetVisibleDateComponents (NSDateComponents dateComponents, bool animated);

		[Export ("wantsDateDecorations")]
		bool WantsDateDecorations { get; set; }

		[Export ("reloadDecorationsForDateComponents:animated:")]
		void ReloadDecorations (NSDateComponents [] dates, bool animated);
	}


	interface IUIEditMenuInteractionAnimating { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UIEditMenuInteractionAnimating {
		[Abstract]
		[Export ("addAnimations:")]
		void AddAnimations (Action animations);

		[Abstract]
		[Export ("addCompletion:")]
		void AddCompletion (Action completion);
	}

	interface IUIEditMenuInteractionDelegate { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIEditMenuInteractionDelegate {
		[Export ("editMenuInteraction:menuForConfiguration:suggestedActions:")]
		[return: NullAllowed]
		UIMenu GetMenu (UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration, UIMenuElement [] suggestedActions);

		[Export ("editMenuInteraction:targetRectForConfiguration:")]
		CGRect GetTargetRect (UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration);

		[Export ("editMenuInteraction:willPresentMenuForConfiguration:animator:")]
		void WillPresentMenu (UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration, IUIEditMenuInteractionAnimating animator);

		[Export ("editMenuInteraction:willDismissMenuForConfiguration:animator:")]
		void WillDismissMenu (UIEditMenuInteraction interaction, UIEditMenuConfiguration configuration, IUIEditMenuInteractionAnimating animator);
	}

	interface IUILayoutGuideAspectFitting { }

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UILayoutGuideAspectFitting {
		[Abstract]
		[Export ("aspectRatio")]
		nfloat AspectRatio { get; set; }
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UIMenuLeaf {
		[Abstract]
		[Export ("title")]
		string Title { get; set; }

		[Abstract]
		[NullAllowed, Export ("image", ArgumentSemantic.Copy)]
		UIImage Image { get; set; }

		[Abstract]
		[NullAllowed, Export ("discoverabilityTitle")]
		string DiscoverabilityTitle { get; set; }

		[Abstract]
		[Export ("attributes", ArgumentSemantic.Assign)]
		UIMenuElementAttributes Attributes { get; set; }

		[Abstract]
		[Export ("state", ArgumentSemantic.Assign)]
		UIMenuElementState State { get; set; }

		[Abstract]
		[NullAllowed, Export ("sender")]
		NSObject Sender { get; }

		[NoTV, iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Abstract]
		[Export ("presentationSourceItem")]
		IUIPopoverPresentationControllerSourceItem PresentationSourceItem { get; }

		[Abstract]
		[Export ("performWithSender:target:")]
		void Target ([NullAllowed] NSObject sender, [NullAllowed] NSObject target);

		[iOS (17, 0)]
#if !XAMCORE_5_0
		[Abstract]
#endif
		[Export ("selectedImage", ArgumentSemantic.Copy)]
		UIImage SelectedImage { get; set; }
	}

	interface IUINavigationItemRenameDelegate { }

	interface IUIPopoverPresentationControllerSourceItem { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UIPopoverPresentationControllerSourceItem {

		[NoTV, iOS (17, 0)]
#if XAMCORE_5_0
		[Abstract]
#endif
		[Export ("frameInView:")]
		CGRect GetFrame (UIView referenceView);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UINavigationItemRenameDelegate {
		[Abstract]
		[Export ("navigationItem:didEndRenamingWithTitle:")]
		void DidEndRenaming (UINavigationItem navigationItem, string title);

		[Export ("navigationItemShouldBeginRenaming:")]
		bool ShouldBeginRenaming (UINavigationItem navigationItem);

		[Export ("navigationItem:willBeginRenamingWithSuggestedTitle:selectedRange:")]
		string WillBeginRenaming (UINavigationItem navigationItem, string title, NSRange selectedRange);

		[Export ("navigationItem:shouldEndRenamingWithTitle:")]
		bool ShouldEndRenaming (UINavigationItem navigationItem, string title);
	}

	interface IUISheetPresentationControllerDetentResolutionContext { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UISheetPresentationControllerDetentResolutionContext {
		[Abstract]
		[Export ("containerTraitCollection")]
		UITraitCollection ContainerTraitCollection { get; }

		[Abstract]
		[Export ("maximumDetentValue")]
		nfloat MaximumDetentValue { get; }
	}

	interface IUITextSearchAggregator { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UITextSearchAggregator {
		[Abstract]
		[Export ("allFoundRanges")]
		NSOrderedSet<UITextRange> AllFoundRanges { get; }

		[Abstract]
		[Export ("foundRange:forSearchString:inDocument:")]
		void GetFoundRange (UITextRange range, string @string, [NullAllowed] INSCopying document);

		[Abstract]
		[Export ("invalidateFoundRange:inDocument:")]
		void Invalidate (UITextRange foundRange, [NullAllowed] INSCopying document);

		[Abstract]
		[Export ("finishedSearching")]
		void FinishedSearching ();

		[Abstract]
		[Export ("invalidate")]
		void Invalidate ();
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface UITextSearchOptions {
		[Export ("wordMatchMethod")]
		UITextSearchMatchMethod WordMatchMethod { get; }

		[Export ("stringCompareOptions")]
		NSStringCompareOptions StringCompareOptions { get; }
	}

	interface IUITextSearching { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol]
	interface UITextSearching {
		[Abstract]
		[NullAllowed, Export ("selectedTextRange")]
		UITextRange SelectedTextRange { get; }

		[Abstract]
		[Export ("compareFoundRange:toRange:inDocument:")]
		NSComparisonResult Compare (UITextRange foundRange, UITextRange toRange, [NullAllowed] INSCopying document);

		[Abstract]
		[Export ("performTextSearchWithQueryString:usingOptions:resultAggregator:")]
		void PerformTextSearch (string @string, UITextSearchOptions options, IUITextSearchAggregator aggregator);

		[Abstract]
		[Export ("decorateFoundTextRange:inDocument:usingStyle:")]
		void Decorate (UITextRange foundRange, [NullAllowed] INSCopying document, UITextSearchFoundTextStyle style);

		[Abstract]
		[Export ("clearAllDecoratedFoundText")]
		void ClearAllDecoratedFoundText ();

		[Export ("supportsTextReplacement")]
		bool SupportsTextReplacement { get; }

		[Export ("shouldReplaceFoundTextInRange:inDocument:withText:")]
		bool ShouldReplaceFoundText (UITextRange range, [NullAllowed] INSCopying document, string replacementText);

		[Export ("replaceFoundTextInRange:inDocument:withText:")]
		void ReplaceFoundText (UITextRange range, [NullAllowed] INSCopying document, string replacementText);

		[Export ("replaceAllOccurrencesOfQueryString:usingOptions:withText:")]
		void ReplaceAllOccurrences (string queryString, UITextSearchOptions options, string replacementText);

		[Export ("willHighlightFoundTextRange:inDocument:")]
		void WillHighlight (UITextRange foundRange, [NullAllowed] INSCopying document);

		[Export ("scrollRangeToVisible:inDocument:")]
		void ScrollRangeToVisible (UITextRange range, [NullAllowed] INSCopying document);

		[NullAllowed, Export ("selectedTextSearchDocument")]
		INSCopying SelectedTextSearchDocument { get; }

		[Export ("compareOrderFromDocument:toDocument:")]
		NSComparisonResult CompareOrder (INSCopying fromDocument, INSCopying toDocument);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIEditMenuInteraction : UIInteraction {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIEditMenuInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("initWithDelegate:")]
		NativeHandle Constructor ([NullAllowed] IUIEditMenuInteractionDelegate @delegate);

		[Export ("presentEditMenuWithConfiguration:")]
		void PresentEditMenu (UIEditMenuConfiguration configuration);

		[Export ("dismissMenu")]
		void DismissMenu ();

		[Export ("reloadVisibleMenu")]
		void ReloadVisibleMenu ();

		[Export ("updateVisibleMenuPositionAnimated:")]
		void UpdateVisibleMenuPosition (bool animated);

		[Export ("locationInView:")]
		CGPoint GetLocation ([NullAllowed] UIView inView);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIEditMenuConfiguration {
		[Export ("identifier", ArgumentSemantic.Copy)]
		INSCopying Identifier { get; }

		[Export ("sourcePoint", ArgumentSemantic.Assign)]
		CGPoint SourcePoint { get; }

		[Export ("preferredArrowDirection", ArgumentSemantic.Assign)]
		UIEditMenuArrowDirection PreferredArrowDirection { get; set; }

		[Static]
		[Export ("configurationWithIdentifier:sourcePoint:")]
		UIEditMenuConfiguration Create ([NullAllowed] INSCopying identifier, CGPoint sourcePoint);
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UICellAccessory))]
	[DisableDefaultCtor]
	interface UICellAccessoryPopUpMenu {
		[Export ("initWithMenu:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIMenu menu);

		[Export ("menu", ArgumentSemantic.Copy)]
		UIMenu Menu { get; }

		[NullAllowed, Export ("selectedElementDidChangeHandler", ArgumentSemantic.Copy)]
		Action<UIMenu> SelectedElementDidChangeHandler { get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIDocumentProperties {
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithMetadata:")]
		NativeHandle Constructor (LPLinkMetadata metadata);

		[Export ("metadata", ArgumentSemantic.Copy)]
		LPLinkMetadata Metadata { get; set; }

		[NullAllowed, Export ("dragItemsProvider", ArgumentSemantic.Copy)]
		Func<IUIDragSession, NSArray<UIDragItem>> DragItemsProvider { get; set; }

		[NullAllowed, Export ("activityViewControllerProvider", ArgumentSemantic.Copy)]
		Func<UIActivityViewController> ActivityViewControllerProvider { get; set; }

		[Export ("wantsIconRepresentation")]
		bool WantsIconRepresentation { get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	delegate UIMenu OptionsMenuProviderHandler (UIMenuElement [] elements);

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIFindInteraction : UIInteraction {
		[Export ("findNavigatorVisible")]
		bool FindNavigatorVisible { [Bind ("isFindNavigatorVisible")] get; }

		[NullAllowed, Export ("activeFindSession")]
		UIFindSession ActiveFindSession { get; }

		[NullAllowed, Export ("searchText")]
		string SearchText { get; set; }

		[NullAllowed, Export ("replacementText")]
		string ReplacementText { get; set; }

		[NullAllowed, Export ("optionsMenuProvider", ArgumentSemantic.Copy)]
		OptionsMenuProviderHandler OptionsMenuProvider { get; set; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIFindInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("initWithSessionDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUIFindInteractionDelegate sessionDelegate);

		[Export ("presentFindNavigatorShowingReplace:")]
		void PresentFindNavigatorShowingReplace (bool showingReplace);

		[Export ("dismissFindNavigator")]
		void DismissFindNavigator ();

		[Export ("findNext")]
		void FindNext ();

		[Export ("findPrevious")]
		void FindPrevious ();

		[Export ("updateResultCount")]
		void UpdateResultCount ();
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface UIFindSession {
		[Export ("resultCount")]
		nint ResultCount { get; }

		[Export ("highlightedResultIndex")]
		nint HighlightedResultIndex { get; }

		[Export ("searchResultDisplayStyle", ArgumentSemantic.Assign)]
		UIFindSessionSearchResultDisplayStyle SearchResultDisplayStyle { get; set; }

		[Export ("performSearchWithQuery:options:")]
		void PerformSearch (string query, [NullAllowed] UITextSearchOptions options);

		[Export ("performSingleReplacementWithSearchQuery:replacementString:options:")]
		void PerformSingleReplacement (string searchQuery, string replacementString, [NullAllowed] UITextSearchOptions options);

		[Export ("replaceAllInstancesOfSearchQuery:withReplacementString:options:")]
		void ReplaceAllInstances (string searchQuery, string replacementString, [NullAllowed] UITextSearchOptions options);

		[Export ("highlightNextResultInDirection:")]
		void HighlightNextResult (UITextStorageDirection direction);

		[Export ("invalidateFoundResults")]
		void InvalidateFoundResults ();

		[Export ("supportsReplacement")]
		bool SupportsReplacement { get; }

		[Export ("allowsReplacementForCurrentlyHighlightedResult")]
		bool AllowsReplacementForCurrentlyHighlightedResult { get; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface UIPasteControlConfiguration : NSSecureCoding {
		[Export ("displayMode", ArgumentSemantic.Assign)]
		UIPasteControlDisplayMode DisplayMode { get; set; }

		[Export ("cornerStyle", ArgumentSemantic.Assign)]
		UIButtonConfigurationCornerStyle CornerStyle { get; set; }

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[NullAllowed, Export ("baseForegroundColor", ArgumentSemantic.Strong)]
		UIColor BaseForegroundColor { get; set; }

		[NullAllowed, Export ("baseBackgroundColor", ArgumentSemantic.Strong)]
		UIColor BaseBackgroundColor { get; set; }

	}

	interface IUIFindInteractionDelegate { }

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0), NoMac]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIFindInteractionDelegate {
		// This abstract method needs attributes since PDFKit.PDFView
		// implements this interface and has iOS 11 support. When inlining
		// this method, the attributes are not carried over and causes issues
		// since it is not supported until iOS 16
		[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0), NoMac]
		[Abstract]
		[Export ("findInteraction:sessionForView:")]
		[return: NullAllowed]
		UIFindSession GetSession (UIFindInteraction interaction, UIView view);

		[Export ("findInteraction:didBeginFindSession:")]
		void DidBeginFindSession (UIFindInteraction interaction, UIFindSession session);

		[Export ("findInteraction:didEndFindSession:")]
		void DidEndFindSession (UIFindInteraction interaction, UIFindSession session);
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneWindowingBehaviors {
		[Export ("closable")]
		bool Closable { [Bind ("isClosable")] get; set; }

		[Export ("miniaturizable")]
		bool Miniaturizable { [Bind ("isMiniaturizable")] get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIFindSession))]
	[DisableDefaultCtor]
	interface UITextSearchingFindSession {
		[NullAllowed, Export ("searchableObject", ArgumentSemantic.Weak)]
		IUITextSearching SearchableObject { get; }

		[Export ("initWithSearchableObject:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUITextSearching searchableObject);
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIWindowSceneGeometry : NSCopying {
		[Export ("systemFrame")]
		CGRect SystemFrame { get; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("interfaceOrientation")]
		UIInterfaceOrientation InterfaceOrientation { get; }
	}

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface UIWindowSceneGeometryPreferences { }

	[NoWatch, TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIWindowSceneGeometryPreferences))]
	[DisableDefaultCtor]
	interface UIWindowSceneGeometryPreferencesMac {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithSystemFrame:")]
		NativeHandle Constructor (CGRect systemFrame);

		[Export ("systemFrame", ArgumentSemantic.Assign)]
		CGRect SystemFrame { get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIWindowSceneGeometryPreferences))]
	[DisableDefaultCtor]
	interface UIWindowSceneGeometryPreferencesIOS {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithInterfaceOrientations:")]
		NativeHandle Constructor (UIInterfaceOrientationMask interfaceOrientations);

		[Export ("interfaceOrientations", ArgumentSemantic.Assign)]
		UIInterfaceOrientationMask InterfaceOrientations { get; set; }
	}

	[NoWatch, NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (UIControl))]
	interface UIPasteControl {
		[Export ("configuration")]
		UIPasteControlConfiguration Configuration { get; }

		[NullAllowed, Export ("target", ArgumentSemantic.Weak)]
		IUIPasteConfigurationSupporting Target { get; set; }

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIPasteControlConfiguration configuration);

		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frame);
	}

	[Static]
	[Internal]
	[Watch (9, 0), TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	interface UIFontWidthConstants {
		[Field ("UIFontWidthCondensed")]
		nfloat Condensed { get; }

		[Field ("UIFontWidthStandard")]
		nfloat Standard { get; }

		[Field ("UIFontWidthExpanded")]
		nfloat Expanded { get; }

		[Field ("UIFontWidthCompressed")]
		nfloat Compressed { get; }
	}

	[NoWatch, NoTV, iOS (16, 4), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextInputContext {

		[Export ("pencilInputExpected")]
		bool PencilInputExpected { [Bind ("isPencilInputExpected")] get; set; }

		[Export ("dictationInputExpected")]
		bool DictationInputExpected { [Bind ("isDictationInputExpected")] get; set; }

		[Export ("hardwareKeyboardInputExpected")]
		bool HardwareKeyboardInputExpected { [Bind ("isHardwareKeyboardInputExpected")] get; set; }

		[Static]
		[Export ("current")]
		UITextInputContext Current { get; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[Native]
	public enum UICollectionLayoutSectionOrthogonalScrollingBounce : long {
		Automatic,
		Always,
		Never,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UIContentUnavailableAlignment : long {
		Center,
		Natural,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UISceneCaptureState : long {
		Unspecified = -1,
		Inactive = 0,
		Active = 1,
	}

	[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Flags]
	[Native]
	public enum UIAccessibilityDirectTouchOptions : ulong {
		None = 0x0,
		SilentOnTouch = 1uL << 0,
		RequiresActivation = 1uL << 1,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UICornerCurve : long {
		Automatic,
		Circular,
		Continuous,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UIImageDynamicRange : long {
		Unspecified = -1,
		Standard = 0,
		ConstrainedHigh = 1,
		High = 2,
	}

	[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UILabelVibrancy : long {
		None,
		Automatic,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UILetterformAwareSizingRule : long {
		Typographic,
		Oversize,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UITextInlinePredictionType : long {
		Default,
		No,
		Yes,
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UITextItemContentType : long {
		Link = 0,
		TextAttachment = 1,
		Tag = 2,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum UITextViewBorderStyle : long {
		None,
		[NoTV, NoiOS, NoMacCatalyst]
		RoundedRect,
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionLayoutSectionOrthogonalScrollingProperties : NSCopying {
		[Export ("decelerationRate")]
		double DecelerationRate { get; set; }

		[Export ("bounce", ArgumentSemantic.Assign)]
		UICollectionLayoutSectionOrthogonalScrollingBounce Bounce { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIContentUnavailableButtonProperties : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("primaryAction", ArgumentSemantic.Copy)]
		UIAction PrimaryAction { get; set; }

		[NullAllowed, Export ("menu", ArgumentSemantic.Copy)]
		UIMenu Menu { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NoWatch]
		[Export ("role", ArgumentSemantic.Assign)]
		UIButtonRole Role { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIContentUnavailableConfiguration : UIContentConfiguration, NSSecureCoding {
		[Static]
		[Export ("emptyConfiguration")]
		UIContentUnavailableConfiguration CreateEmptyConfiguration ();

		[Static]
		[Export ("loadingConfiguration")]
		UIContentUnavailableConfiguration CreateLoadingConfiguration ();

		[Static]
		[Export ("searchConfiguration")]
		UIContentUnavailableConfiguration CreateSearchConfiguration ();

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Export ("imageProperties")]
		UIContentUnavailableImageProperties ImageProperties { get; }

		[NullAllowed, Export ("text")]
		string Text { get; set; }

		[NullAllowed, Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }

		[Export ("textProperties")]
		UIContentUnavailableTextProperties TextProperties { get; }

		[NullAllowed, Export ("secondaryText")]
		string SecondaryText { get; set; }

		[NullAllowed, Export ("secondaryAttributedText", ArgumentSemantic.Copy)]
		NSAttributedString SecondaryAttributedText { get; set; }

		[Export ("secondaryTextProperties")]
		UIContentUnavailableTextProperties SecondaryTextProperties { get; }

		[NoWatch]
		[Export ("button", ArgumentSemantic.Strong)]
		UIButtonConfiguration Button { get; set; }

		[Export ("buttonProperties")]
		UIContentUnavailableButtonProperties ButtonProperties { get; }

		[NoWatch]
		[Export ("secondaryButton", ArgumentSemantic.Strong)]
		UIButtonConfiguration SecondaryButton { get; set; }

		[Export ("secondaryButtonProperties")]
		UIContentUnavailableButtonProperties SecondaryButtonProperties { get; }

		[Export ("alignment", ArgumentSemantic.Assign)]
		UIContentUnavailableAlignment Alignment { get; set; }

		[NoWatch]
		[Export ("axesPreservingSuperviewLayoutMargins", ArgumentSemantic.Assign)]
		UIAxis AxesPreservingSuperviewLayoutMargins { get; set; }

		[Export ("directionalLayoutMargins", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets DirectionalLayoutMargins { get; set; }

		[Export ("imageToTextPadding")]
		nfloat ImageToTextPadding { get; set; }

		[Export ("textToSecondaryTextPadding")]
		nfloat TextToSecondaryTextPadding { get; set; }

		[Export ("textToButtonPadding")]
		nfloat TextToButtonPadding { get; set; }

		[Export ("buttonToSecondaryButtonPadding")]
		nfloat ButtonToSecondaryButtonPadding { get; set; }

		[NoWatch]
		[Export ("background", ArgumentSemantic.Strong)]
		UIBackgroundConfiguration Background { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIContentUnavailableImageProperties : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("preferredSymbolConfiguration", ArgumentSemantic.Copy)]
		UIImageSymbolConfiguration PreferredSymbolConfiguration { get; set; }

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Strong)]
		UIColor TintColor { get; set; }

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("maximumSize", ArgumentSemantic.Assign)]
		CGSize MaximumSize { get; set; }

		[Export ("accessibilityIgnoresInvertColors")]
		bool AccessibilityIgnoresInvertColors { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIContentUnavailableTextProperties : NSCopying, NSSecureCoding {
		[Export ("font", ArgumentSemantic.Strong)]
		UIFont Font { get; set; }

		[Export ("color", ArgumentSemantic.Strong)]
		UIColor Color { get; set; }

		[Export ("lineBreakMode", ArgumentSemantic.Assign)]
		UILineBreakMode LineBreakMode { get; set; }

		[Export ("numberOfLines")]
		nint NumberOfLines { get; set; }

		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; set; }

		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set; }

		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIContentUnavailableConfigurationState : UIConfigurationState {
		[Export ("initWithTraitCollection:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UITraitCollection traitCollection);

		[NullAllowed, Export ("searchText", ArgumentSemantic.Strong)]
		string SearchText { get; set; }
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (UIView))]
	[DisableDefaultCtor]
	interface UIContentUnavailableView : UIContentView {
		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIContentUnavailableConfiguration configuration);

		[Export ("scrollEnabled")]
		bool ScrollEnabled { [Bind ("isScrollEnabled")] get; set; }
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
#if XAMCORE_5_0
	[BaseType (typeof(UIViewController))]
	interface UIDocumentViewController
#else
	[BaseType (typeof (UIViewController), Name = "UIDocumentViewController")]
	interface UIDocViewController
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithDocument:")]
		NativeHandle Constructor ([NullAllowed] UIDocument document);

		[NullAllowed, Export ("document", ArgumentSemantic.Strong)]
		UIDocument Document { get; set; }

		[Export ("navigationItemDidUpdate")]
		void NavigationItemDidUpdate ();

		[Async]
		[Export ("openDocumentWithCompletionHandler:")]
		void OpenDocument (Action<bool> completionHandler);

		[Export ("documentDidOpen")]
		void DocumentDidOpen ();

		[Export ("undoRedoItemGroup")]
		UIBarButtonItemGroup UndoRedoItemGroup { get; }
	}

	interface IUIShapeProvider { }

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UIShapeProvider {
		[Abstract]
		[Export ("resolvedShapeInContext:")]
		UIResolvedShape CreateResolvedShape (UIShapeResolutionContext context);
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIShape : UIShapeProvider, NSCopying {
		[Static]
		[Export ("rectShape")]
		UIShape CreateRectShape ();

		[Static]
		[Export ("capsuleShape")]
		UIShape CreateCapsuleShape ();

		[Static]
		[Export ("circleShape")]
		UIShape CreateCircleShape ();

		[Static]
		[Export ("rectShapeWithCornerRadius:")]
		UIShape CreateRectShape (nfloat cornerRadius);

		[Static]
		[Export ("rectShapeWithCornerRadius:cornerCurve:")]
		UIShape CreateRectShape (nfloat cornerRadius, UICornerCurve cornerCurve);

		[Static]
		[Export ("rectShapeWithCornerRadius:cornerCurve:maskedCorners:")]
		UIShape CreateRectShape (nfloat cornerRadius, UICornerCurve cornerCurve, UIRectCorner maskedCorners);

		[Static]
		[Export ("fixedRectShapeWithRect:")]
		UIShape CreateFixedRectShape (CGRect rect);

		[Static]
		[Export ("fixedRectShapeWithRect:cornerRadius:")]
		UIShape CreateFixedRectShape (CGRect rect, nfloat cornerRadius);

		[Static]
		[Export ("fixedRectShapeWithRect:cornerRadius:cornerCurve:maskedCorners:")]
		UIShape CreateFixedRectShape (CGRect rect, nfloat cornerRadius, UICornerCurve cornerCurve, UIRectCorner maskedCorners);

		[Static]
		[Export ("shapeWithBezierPath:")]
		UIShape CreateShape (UIBezierPath path);

		[Static]
		[Export ("shapeWithProvider:")]
		UIShape CreateShape (IUIShapeProvider provider);

		[Export ("shapeByApplyingInsets:")]
		UIShape CreateShape (UIEdgeInsets insets);

		[Export ("shapeByApplyingInset:")]
		UIShape CreateShapeByApplyingInset (nfloat inset);
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIResolvedShape : NSCopying {
		[Export ("shape")]
		UIShape Shape { get; }

		[Export ("boundingRect")]
		CGRect BoundingRect { get; }

		[Export ("path")]
		UIBezierPath Path { get; }

		[Export ("shapeByApplyingInsets:")]
		UIResolvedShape CreateShape (UIEdgeInsets insets);

		[Export ("shapeByApplyingInset:")]
		UIResolvedShape CreateShape (nfloat inset);
	}

	interface IUIHoverEffect { }

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UIHoverEffect { }

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIHoverAutomaticEffect : UIHoverEffect, NSCopying {
		[Static]
		[Export ("effect")]
		UIHoverAutomaticEffect Create ();
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIHoverHighlightEffect : UIHoverEffect, NSCopying {
		[Static]
		[Export ("effect")]
		UIHoverHighlightEffect Create ();
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIHoverLiftEffect : UIHoverEffect, NSCopying {
		[Static]
		[Export ("effect")]
		UIHoverLiftEffect Create ();
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIHoverStyle : NSCopying {
		[Export ("effect", ArgumentSemantic.Strong)]
		IUIHoverEffect Effect { get; set; }

		[NullAllowed]
		[Export ("shape", ArgumentSemantic.Strong)]
		UIShape Shape { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Static]
		[Export ("styleWithEffect:shape:")]
		UIHoverStyle Create (IUIHoverEffect effect, [NullAllowed] UIShape shape);

		[Static]
		[Export ("styleWithShape:")]
		UIHoverStyle Create ([NullAllowed] UIShape shape);

		[Static]
		[Export ("automaticStyle")]
		UIHoverStyle CreateAutomatic ();
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UIShapeResolutionContext {
		[Export ("contentShape")]
		UIResolvedShape ContentShape { get; }
	}

	[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIWindowScenePlacement : NSCopying { }

	[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (UIWindowScenePlacement))]
	interface UIWindowSceneStandardPlacement {
		[Static]
		[Export ("standardPlacement")]
		UIWindowSceneStandardPlacement Create ();
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (UIWindowScenePlacement))]
	interface UIWindowSceneProminentPlacement {
		[Static]
		[Export ("prominentPlacement")]
		UIWindowSceneProminentPlacement Create ();
	}

	[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIImageReader {
		[Static]
		[Export ("defaultReader", ArgumentSemantic.Strong)]
		UIImageReader DefaultReader { get; }

		[Static]
		[Export ("readerWithConfiguration:")]
		UIImageReader CreateReader (UIImageReaderConfiguration configuration);

		[Export ("configuration", ArgumentSemantic.Copy)]
		UIImageReaderConfiguration Configuration { get; }
		[Export ("imageWithContentsOfFileURL:")]
		UIImage GetImage (NSUrl url);

		[Export ("imageWithData:")]
		UIImage GetImage (NSData data);

		[Async]
		[Export ("imageWithContentsOfFileURL:completion:")]
		void GetImage (NSUrl url, Action<UIImage> completion);

		[Async]
		[Export ("imageWithData:completion:")]
		void GetImage (NSData data, Action<UIImage> completion);
	}

	[Watch (10, 0), TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIImageReaderConfiguration : NSCopying {
		[Export ("prefersHighDynamicRange")]
		bool PrefersHighDynamicRange { get; set; }

		[Export ("preparesImagesForDisplay")]
		bool PreparesImagesForDisplay { get; set; }

		[Export ("preferredThumbnailSize", ArgumentSemantic.Assign)]
		CGSize PreferredThumbnailSize { get; set; }

		[Export ("pixelsPerInch")]
		nfloat PixelsPerInch { get; set; }
	}

	interface IUIPageControlProgressDelegate { }

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoWatch]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIPageControlProgressDelegate {
		[Export ("pageControlProgress:initialProgressForPage:")]
		float InitialProgressForPage (UIPageControlProgress progress, nint page);

		[Export ("pageControlProgressVisibilityDidChange:")]
		void VisibilityDidChange (UIPageControlProgress progress);
	}

	interface IUIPageControlTimerProgressDelegate { }

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoWatch]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIPageControlTimerProgressDelegate : UIPageControlProgressDelegate {
		[Export ("pageControlTimerProgressDidChange:")]
		void PageControlTimerProgressDidChange (UIPageControlTimerProgress progress);

		[Export ("pageControlTimerProgress:shouldAdvanceToPage:")]
		bool ShouldAdvanceToPage (UIPageControlTimerProgress progress, nint page);
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoWatch]
	[BaseType (typeof (NSObject))]
	interface UIPageControlProgress {
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIPageControlProgressDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("currentProgress")]
		float CurrentProgress { get; set; }

		[Export ("progressVisible")]
		bool ProgressVisible { [Bind ("isProgressVisible")] get; }
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0), NoWatch]
	[BaseType (typeof (UIPageControlProgress))]
	[DisableDefaultCtor]
	interface UIPageControlTimerProgress {
		[Export ("initWithPreferredDuration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double preferredDuration);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIPageControlTimerProgressDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("resetsToInitialPageAfterEnd")]
		bool ResetsToInitialPageAfterEnd { get; set; }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[Export ("resumeTimer")]
		void ResumeTimer ();

		[Export ("pauseTimer")]
		void PauseTimer ();

		[Export ("preferredDuration")]
		double PreferredDuration { get; set; }

		[Export ("setDuration:forPage:")]
		void SetDuration (double duration, nint page);

		[Export ("durationForPage:")]
		double GetDuration (nint page);
	}

	[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISceneSessionActivationRequest : NSCopying {
		[Export ("role")]
		string Role { get; }

		[NullAllowed, Export ("session")]
		UISceneSession Session { get; }

		[NullAllowed, Export ("userActivity", ArgumentSemantic.Strong)]
		NSUserActivity UserActivity { get; set; }

		[NullAllowed, Export ("options", ArgumentSemantic.Copy)]
		UISceneActivationRequestOptions Options { get; set; }

		[Static]
		[Export ("request")]
		UISceneSessionActivationRequest Create ();

		[Static]
		[Export ("requestWithRole:")]
		UISceneSessionActivationRequest Create (string role);

		[Static]
		[Export ("requestWithSession:")]
		UISceneSessionActivationRequest Create (UISceneSession session);
	}

	[NoWatch, TV (17, 0), iOS (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UISymbolEffectCompletionContext {
		[Export ("finished")]
		bool Finished { [Bind ("isFinished")] get; }

		[NullAllowed, Export ("sender", ArgumentSemantic.Weak)]
		NSObject Sender { get; }

		[NullAllowed, Export ("effect")]
		NSSymbolEffect Effect { get; }

		[NullAllowed, Export ("contentTransition")]
		NSSymbolContentTransition ContentTransition { get; }
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextItem {
		[Export ("contentType", ArgumentSemantic.Assign)]
		UITextItemContentType ContentType { get; }

		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; }

		[NullAllowed, Export ("link", ArgumentSemantic.Strong)]
		NSUrl Link { get; }

		[NullAllowed, Export ("textAttachment", ArgumentSemantic.Strong)]
		NSTextAttachment TextAttachment { get; }

		[NullAllowed, Export ("tagIdentifier", ArgumentSemantic.Strong)]
		string TagIdentifier { get; }
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UIWindowSceneDragInteraction : UIInteraction {
		[NoMacCatalyst]
		[Export ("gestureForFailureRelationships")]
		UIGestureRecognizer GestureForFailureRelationships { get; }
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextItemMenuPreview {
		[Static]
		[Export ("defaultPreview")]
		UITextItemMenuPreview DefaultPreview { get; }

		[Export ("initWithView:")]
		NativeHandle Constructor (UIView view);
	}

	[NoWatch, NoTV, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextItemMenuConfiguration {
		[Static]
		[Export ("configurationWithMenu:")]
		UITextItemMenuConfiguration Create (UIMenu menu);

		[Static]
		[Export ("configurationWithPreview:menu:")]
		UITextItemMenuConfiguration Create ([NullAllowed] UITextItemMenuPreview preview, UIMenu menu);
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITextLoupeSession {
		[Static]
		[Export ("beginLoupeSessionAtPoint:fromSelectionWidgetView:inView:")]
		[return: NullAllowed]
		UITextLoupeSession BeginLoupeSession (CGPoint point, [NullAllowed] UIView selectionWidget, UIView interactionView);

		[Export ("moveToPoint:withCaretRect:trackingCaret:")]
		void MoveToPoint (CGPoint point, CGRect caretRect, bool tracksCaret);

		[Export ("invalidate")]
		void Invalidate ();
	}

	interface IUITextCursorView { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITextCursorView : UICoordinateSpace {
		[Abstract]
		[Export ("blinking")]
		bool Blinking { [Bind ("isBlinking")] get; set; }

		[Abstract]
		[Export ("resetBlinkAnimation")]
		void ResetBlinkAnimation ();
	}

	interface IUITextSelectionHighlightView { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITextSelectionHighlightView : UICoordinateSpace {
		[Abstract]
		[Export ("selectionRects", ArgumentSemantic.Strong)]
		UITextSelectionRect [] SelectionRects { get; set; }
	}

	interface IUITextSelectionHandleView { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITextSelectionHandleView : UICoordinateSpace {
		[Abstract]
		[Export ("direction", ArgumentSemantic.Assign)]
		NSDirectionalRectEdge Direction { get; set; }

		[Abstract]
		[Export ("vertical")]
		bool Vertical { [Bind ("isVertical")] get; }

		[Abstract]
		[NullAllowed, Export ("customShape", ArgumentSemantic.Strong)]
		UIBezierPath CustomShape { get; set; }

		[Abstract]
		[Export ("preferredFrameForRect:")]
		CGRect GetPreferredFrame (CGRect rect);
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface UITextSelectionDisplayInteraction : UIInteraction {
		[Export ("activated")]
		bool Activated { [Bind ("isActivated")] get; set; }

		[NullAllowed, Export ("textInput", ArgumentSemantic.Weak)]
		IUITextInput TextInput { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUITextSelectionDisplayInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("cursorView", ArgumentSemantic.Strong)]
		IUITextCursorView CursorView { get; set; }

		[Export ("highlightView", ArgumentSemantic.Strong)]
		IUITextSelectionHighlightView HighlightView { get; set; }

		[Export ("handleViews", ArgumentSemantic.Strong)]
		IUITextSelectionHandleView [] HandleViews { get; set; }

		[Export ("initWithTextInput:delegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IUITextInput textInput, IUITextSelectionDisplayInteractionDelegate @delegate);

		[Export ("layoutManagedSubviews")]
		void LayoutManagedSubviews ();

		[Export ("setNeedsSelectionUpdate")]
		void SetNeedsSelectionUpdate ();
	}

	interface IUITextSelectionDisplayInteractionDelegate { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UITextSelectionDisplayInteractionDelegate {
		[Export ("selectionContainerViewBelowTextForSelectionDisplayInteraction:")]
		[return: NullAllowed]
		UIView GetSelectionContainerViewBellowText (UITextSelectionDisplayInteraction interaction);
	}

	interface IUITraitDefinition { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITraitDefinition {
		[Static]
		[Export ("identifier")]
		string Identifier { get; }

		[Static]
		[Export ("name")]
		string Name { get; }

		[Static]
		[Export ("affectsColorAppearance")]
		bool AffectsColorAppearance { get; }
	}

	interface IUICGFloatTraitDefinition { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UICGFloatTraitDefinition : UITraitDefinition {
		[Static, Abstract]
		[Export ("defaultValue")]
		nfloat DefaultValue { get; }
	}


	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UILetterformAwareAdjusting {
		[Abstract]
		[Export ("sizingRule", ArgumentSemantic.Assign)]
		UILetterformAwareSizingRule SizingRule { get; set; }
	}

	[NoWatch, TV (17, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UILookToDictateCapable {
		[NoTV]
		[Abstract]
		[Export ("lookToDictateEnabled")]
		bool LookToDictateEnabled { [Bind ("isLookToDictateEnabled")] get; set; }
	}

	interface IUINSIntegerTraitDefinition { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UINSIntegerTraitDefinition : UITraitDefinition {
		[Static, Abstract]
		[Export ("defaultValue")]
		nint DefaultValue { get; }
	}

	interface IUIObjectTraitDefinition { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIObjectTraitDefinition : UITraitDefinition {
		[Static, Abstract]
		[NullAllowed, Export ("defaultValue")]
		NSObject DefaultValue { get; }
	}

	interface IUIMutableTraits { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface UIMutableTraits {
		[Abstract]
		[Export ("setCGFloatValue:forTrait:")]
		void SetValue (nfloat value, IUICGFloatTraitDefinition trait);

		[Abstract]
		[Export ("valueForCGFloatTrait:")]
		nfloat GetValue (IUICGFloatTraitDefinition trait);

		[Abstract]
		[Export ("setNSIntegerValue:forTrait:")]
		void SetValue (nint value, IUINSIntegerTraitDefinition trait);

		[Abstract]
		[Export ("valueForNSIntegerTrait:")]
		nint GetValue (IUINSIntegerTraitDefinition trait);

		[Abstract]
		[Export ("setObject:forTrait:")]
		void SetObject ([NullAllowed] NSObject @object, IUIObjectTraitDefinition trait);

		[Abstract]
		[Export ("objectForTrait:")]
		[return: NullAllowed]
		NSObject GetObject (IUIObjectTraitDefinition trait);

		[Abstract]
		[Export ("userInterfaceIdiom", ArgumentSemantic.Assign)]
		UIUserInterfaceIdiom UserInterfaceIdiom { get; set; }

		[Abstract]
		[Export ("userInterfaceStyle", ArgumentSemantic.Assign)]
		UIUserInterfaceStyle UserInterfaceStyle { get; set; }

		[Abstract]
		[Export ("layoutDirection", ArgumentSemantic.Assign)]
		UITraitEnvironmentLayoutDirection LayoutDirection { get; set; }

		[Abstract]
		[Export ("displayScale")]
		nfloat DisplayScale { get; set; }

		[Abstract]
		[Export ("horizontalSizeClass", ArgumentSemantic.Assign)]
		UIUserInterfaceSizeClass HorizontalSizeClass { get; set; }

		[Abstract]
		[Export ("verticalSizeClass", ArgumentSemantic.Assign)]
		UIUserInterfaceSizeClass VerticalSizeClass { get; set; }

		[Abstract]
		[Export ("forceTouchCapability", ArgumentSemantic.Assign)]
		UIForceTouchCapability ForceTouchCapability { get; set; }

		[Abstract]
		[Export ("preferredContentSizeCategory")]
		string PreferredContentSizeCategory { get; set; }

		[Abstract]
		[Export ("displayGamut", ArgumentSemantic.Assign)]
		UIDisplayGamut DisplayGamut { get; set; }

		[NoWatch]
		[Abstract]
		[Export ("accessibilityContrast", ArgumentSemantic.Assign)]
		UIAccessibilityContrast AccessibilityContrast { get; set; }

		[NoWatch, NoTV]
		[Abstract]
		[Export ("userInterfaceLevel", ArgumentSemantic.Assign)]
		UIUserInterfaceLevel UserInterfaceLevel { get; set; }

		[Abstract]
		[Export ("legibilityWeight", ArgumentSemantic.Assign)]
		UILegibilityWeight LegibilityWeight { get; set; }

		[Abstract]
		[Export ("activeAppearance", ArgumentSemantic.Assign)]
		UIUserInterfaceActiveAppearance ActiveAppearance { get; set; }

		[Abstract]
		[Export ("toolbarItemPresentationSize", ArgumentSemantic.Assign)]
		UINSToolbarItemPresentationSize ToolbarItemPresentationSize { get; set; }

		[Abstract]
		[Export ("imageDynamicRange", ArgumentSemantic.Assign)]
		UIImageDynamicRange ImageDynamicRange { get; set; }

		[Abstract]
		[Export ("sceneCaptureState", ArgumentSemantic.Assign)]
		UISceneCaptureState SceneCaptureState { get; set; }

		[Abstract]
		[Export ("typesettingLanguage")]
		string TypesettingLanguage { get; set; }
	}


	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitUserInterfaceIdiom : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitUserInterfaceStyle : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitLayoutDirection : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitDisplayScale : UICGFloatTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitHorizontalSizeClass : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitVerticalSizeClass : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitForceTouchCapability : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitPreferredContentSizeCategory : UIObjectTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitDisplayGamut : UINSIntegerTraitDefinition {
	}

	[TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitAccessibilityContrast : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), NoWatch, iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitUserInterfaceLevel : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitLegibilityWeight : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitActiveAppearance : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitToolbarItemPresentationSize : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitImageDynamicRange : UINSIntegerTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitTypesettingLanguage : UIObjectTraitDefinition {
	}

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface UITraitSceneCaptureState : UINSIntegerTraitDefinition {
	}

	interface IUITraitOverrides { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITraitOverrides : UIMutableTraits {
		[Abstract]
		[Export ("containsTrait:")]
		bool ContainsTrait (IUITraitDefinition trait);

		[Abstract]
		[Export ("removeTrait:")]
		void RemoveTrait (IUITraitDefinition trait);
	}

	[iOS (17, 0), TV (17, 0), Watch (10, 0), MacCatalyst (17, 0)]
	public enum UIAccessibilityPriority {
		[Field ("UIAccessibilityPriorityHigh")]
		High,
		[Field ("UIAccessibilityPriorityDefault")]
		Default,
		[Field ("UIAccessibilityPriorityLow")]
		Low,
	}

	// cannot be an enum because the values are doubles
	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	public interface UICollectionLayoutSectionOrthogonalScrollingDecelerationRate {

		[Field ("UICollectionLayoutSectionOrthogonalScrollingDecelerationRateAutomatic")]
		double Automatic { get; }

		[Field ("UICollectionLayoutSectionOrthogonalScrollingDecelerationRateNormal")]
		double Normal { get; }

		[Field ("UICollectionLayoutSectionOrthogonalScrollingDecelerationRateFast")]
		double Fast { get; }
	}

	public enum UIAccessibilityTraits : long {
		[Field ("UIAccessibilityTraitNone")]
		None,

		[Field ("UIAccessibilityTraitButton")]
		Button,

		[Field ("UIAccessibilityTraitLink")]
		Link,

		[Field ("UIAccessibilityTraitHeader")]
		Header,

		[Field ("UIAccessibilityTraitSearchField")]
		SearchField,

		[Field ("UIAccessibilityTraitImage")]
		Image,

		[Field ("UIAccessibilityTraitSelected")]
		Selected,

		[Field ("UIAccessibilityTraitPlaysSound")]
		PlaysSound,

		[Field ("UIAccessibilityTraitKeyboardKey")]
		KeyboardKey,

		[Field ("UIAccessibilityTraitStaticText")]
		StaticText,

		[Field ("UIAccessibilityTraitSummaryElement")]
		SummaryElement,

		[Field ("UIAccessibilityTraitNotEnabled")]
		NotEnabled,

		[Field ("UIAccessibilityTraitUpdatesFrequently")]
		UpdatesFrequently,

		[Field ("UIAccessibilityTraitStartsMediaSession")]
		StartsMediaSession,

		[Field ("UIAccessibilityTraitAdjustable")]
		Adjustable,

		[Field ("UIAccessibilityTraitAllowsDirectInteraction")]
		AllowsDirectInteraction,

		[Field ("UIAccessibilityTraitCausesPageTurn")]
		CausesPageTurn,

		[MacCatalyst (13, 1)]
		[Field ("UIAccessibilityTraitTabBar")]
		TabBar,

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0), Watch (10, 0)]
		[Field ("UIAccessibilityTraitSupportsZoom")]
		SupportsZoom,

		[iOS (17, 0), MacCatalyst (17, 0), TV (17, 0), Watch (10, 0)]
		[Field ("UIAccessibilityTraitToggleButton")]
		ToggleButton,
	}

	interface IUITraitChangeRegistration { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITraitChangeRegistration : NSCopying { }

	[NoWatch, TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Protocol]
	interface UITraitChangeObservable {
		[Abstract]
		[Export ("registerForTraitChanges:withHandler:")]
		IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Action<IUITraitEnvironment, UITraitCollection> handler);

		[Abstract]
		[Export ("registerForTraitChanges:withTarget:action:")]
		IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, NSObject target, Selector action);

		[Abstract]
		[Export ("registerForTraitChanges:withAction:")]
		IUITraitChangeRegistration RegisterForTraitChanges (IUITraitDefinition [] traits, Selector action);

		[Abstract]
		[Export ("unregisterForTraitChanges:")]
		void UnregisterForTraitChanges (IUITraitChangeRegistration registration);
	}

}
