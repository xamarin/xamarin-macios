//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009-2011, Novell, Inc.
// Copyrigh 2011-2013, Xamarin Inc.
//
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreLocation;
using UIKit;
using CloudKit;
#if !TVOS
using Contacts;
#endif
#if !WATCH
using MediaPlayer;
using CoreImage;
using CoreAnimation;
#endif
using CoreData;

#if XAMCORE_2_0
#if IOS
using FileProvider;
#endif // IOS
#if !TVOS
using Intents;
#endif // !TVOS
#endif // XAMCORE_2_0

using System;
using System.ComponentModel;

namespace UIKit {

	[NoWatch]
	[iOS (9,0)]
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
	}

	[Native] // NSInteger -> UIApplication.h
	[NoWatch]
	[TV (11,0)]
	public enum UIBackgroundRefreshStatus : long {
		Restricted, Denied, Available
	}

	[TV (10,0)][NoWatch]
	[Native] // NSUInteger -> UIApplication.h
	public enum UIBackgroundFetchResult : ulong {
		NewData, NoData, Failed
	}

	[NoTV][NoWatch]
	[iOS (9,0)]
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

	[NoWatch, NoTV, iOS (10,0)]
	[Native]
	public enum UIImpactFeedbackStyle : long {
		Light,
		Medium,
		Heavy
	}

	[NoWatch, NoTV, iOS (10,0)]
	[Native]
	public enum UINotificationFeedbackType : long {
		Success,
		Warning,
		Error
	}

#if WATCH
	// hacks to ease compilation
	interface CIColor {}
#else
	delegate void NSTextLayoutEnumerateLineFragments (CGRect rect, CGRect usedRectangle, NSTextContainer textContainer, NSRange glyphRange, ref bool stop);
	delegate void NSTextLayoutEnumerateEnclosingRects (CGRect rect, ref bool stop);
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
	[Category (allowStaticMembers: true)] // Classic isn't internal so we need this
	[BaseType (typeof (NSAttributedString))]
	interface NSAttributedStringAttachmentConveniences {
#if XAMCORE_2_0
		[Internal]
#else
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
#endif
		[Static, Export ("attributedStringWithAttachment:")]
		NSAttributedString FromTextAttachment (NSTextAttachment attachment);
	}
#endif

	[NoWatch, NoTV, iOS (10,0)]
	[DisableDefaultCtor]
	[Abstract] // abstract class that should not be used directly
	[BaseType (typeof (NSObject))]
	interface UIFeedbackGenerator {

		[Export ("prepare")]
		void Prepare ();
	}

	[NoWatch, NoTV, iOS (10,0)]
	[DisableDefaultCtor]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UIImpactFeedbackGenerator {

		[Export ("initWithStyle:")]
		IntPtr Constructor (UIImpactFeedbackStyle style);

		[Export ("impactOccurred")]
		void ImpactOccurred ();
	}

	[NoWatch, NoTV, iOS (10,0)]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UINotificationFeedbackGenerator {

		[Export ("notificationOccurred:")]
		void NotificationOccurred (UINotificationFeedbackType notificationType);
	}

	[NoWatch, NoTV, iOS (10,0)]
	[BaseType (typeof (UIFeedbackGenerator))]
	interface UISelectionFeedbackGenerator {

		[Export ("selectionChanged")]
		void SelectionChanged ();
	}

	interface IUICloudSharingControllerDelegate { }

	[iOS (10,0), NoTV, NoWatch]
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

	[iOS (10,0), NoTV, NoWatch]
	delegate void UICloudSharingControllerPreparationHandler (UICloudSharingController controller, [BlockCallback] UICloudSharingControllerPreparationCompletionHandler completion);

	[iOS (10,0), NoTV, NoWatch]
	delegate void UICloudSharingControllerPreparationCompletionHandler ([NullAllowed] CKShare share, [NullAllowed] CKContainer container, [NullAllowed] NSError error);

	[iOS (10,0), NoTV, NoWatch]
	[BaseType (typeof (UIViewController))]
	interface UICloudSharingController {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithPreparationHandler:")]
		IntPtr Constructor (UICloudSharingControllerPreparationHandler preparationHandler);

		[Export ("initWithShare:container:")]
		IntPtr Constructor (CKShare share, CKContainer container);

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
	[Category]
	[BaseType (typeof (NSAttributedString))]
	interface NSAttributedString_NSAttributedStringKitAdditions {
		[iOS (9,0)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachments (NSRange range);
	}

	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: -[NSDataAsset init]: unrecognized selector sent to instance 0x7f6c8cc0
	interface NSDataAsset : NSCopying
	{
		[Export ("initWithName:")]
		IntPtr Constructor (string name);

		[DesignatedInitializer]
		[Export ("initWithName:bundle:")]
		IntPtr Constructor (string name, NSBundle bundle);
	
		[Export ("name")]
		string Name { get; }
	
		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }
	
		[Export ("typeIdentifier")]
		NSString TypeIdentifier { get; }
	}

	delegate void NSFileProviderExtensionFetchThumbnailsHandler (NSString identifier, [NullAllowed] NSData imageData, [NullAllowed] NSError error);

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	partial interface NSFileProviderExtension {
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'NSFileProviderManager' instead.")]
	    [Static, Export ("writePlaceholderAtURL:withMetadata:error:")]
	    bool WritePlaceholder (NSUrl placeholderUrl, NSDictionary metadata, ref NSError error);
	
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FileProvider::NSFileProviderManager.GetPlaceholderUrl (NSUrl)' instead.")]
	    [Static, Export ("placeholderURLForURL:")]
	    NSUrl GetPlaceholderUrl (NSUrl url);
	
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FileProvider::NSFileProviderManager.ProviderIdentifier' instead.")]
	    [Export ("providerIdentifier")]
	    string ProviderIdentifier { get; }
	
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'FileProvider::NSFileProviderManager.DocumentStorageUrl' instead.")]
	    [Export ("documentStorageURL")]
	    NSUrl DocumentStorageUrl { get; }
	
	    [Export ("URLForItemWithPersistentIdentifier:")]
	    NSUrl GetUrlForItem (string persistentIdentifier);
	
	    [Export ("persistentIdentifierForItemAtURL:")]
	    string GetPersistentIdentifier (NSUrl itemUrl);
	
	    [Export ("providePlaceholderAtURL:completionHandler:")]
	    [Async]
	    void ProvidePlaceholderAtUrl (NSUrl url, [NullAllowed] Action<NSError> completionHandler);
	
	    [Export ("startProvidingItemAtURL:completionHandler:")]
	    [Async]
	    void StartProvidingItemAtUrl (NSUrl url, [NullAllowed] Action<NSError> completionHandler);
	
	    [Export ("itemChangedAtURL:")]
	    void ItemChangedAtUrl (NSUrl url);
	
	    [Export ("stopProvidingItemAtURL:")]
	    void StopProvidingItemAtUrl (NSUrl url);

#if XAMCORE_2_0 && IOS

		[iOS (11,0)]
		[Export ("itemForIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderItem GetItem (NSString identifier, out NSError error);

		// Inlining NSFileProviderExtension (NSFileProviderActions) so we get asyncs

		[iOS (11,0)]
		[Async]
		[Export ("importDocumentAtURL:toParentItemIdentifier:completionHandler:")]
		void ImportDocument (NSUrl fileUrl, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("createDirectoryWithName:inParentItemIdentifier:completionHandler:")]
		void CreateDirectory (string directoryName, string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("renameItemWithIdentifier:toName:completionHandler:")]
		void RenameItem (string itemIdentifier, string itemName, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("reparentItemWithIdentifier:toParentItemWithIdentifier:newName:completionHandler:")]
		void ReparentItem (string itemIdentifier, string parentItemIdentifier, [NullAllowed] string newName, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("trashItemWithIdentifier:completionHandler:")]
		void TrashItem (string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("untrashItemWithIdentifier:toParentItemIdentifier:completionHandler:")]
		void UntrashItem (string itemIdentifier, [NullAllowed] string parentItemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("deleteItemWithIdentifier:completionHandler:")]
		void DeleteItem (string itemIdentifier, Action<NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setLastUsedDate:forItemIdentifier:completionHandler:")]
		void SetLastUsedDate ([NullAllowed] NSDate lastUsedDate, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setTagData:forItemIdentifier:completionHandler:")]
		void SetTagData ([NullAllowed] NSData tagData, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Async]
		[Export ("setFavoriteRank:forItemIdentifier:completionHandler:")]
		void SetFavoriteRank ([NullAllowed] NSNumber favoriteRank, string itemIdentifier, Action<INSFileProviderItem, NSError> completionHandler);

		[iOS (11,0)]
		[Export ("enumeratorForContainerItemIdentifier:error:")]
		[return: NullAllowed]
		FileProvider.INSFileProviderEnumerator GetEnumerator (string containerItemIdentifier, out NSError error);

		// From NSFileProviderExtension (NSFileProviderThumbnailing)

		[iOS (11,0)]
		[Export ("fetchThumbnailsForItemIdentifiers:requestedSize:perThumbnailCompletionHandler:completionHandler:")]
		[Async]
		NSProgress FetchThumbnails (NSString [] itemIdentifiers, CGSize size, NSFileProviderExtensionFetchThumbnailsHandler perThumbnailCompletionHandler, Action<NSError> completionHandler);

		// From NSFileProviderExtension (NSFileProviderService)

		[iOS (11,0)]
		[Export ("supportedServiceSourcesForItemIdentifier:error:")]
		[return: NullAllowed]
		INSFileProviderServiceSource [] GetSupportedServiceSources (string itemIdentifier, out NSError error);

		// From NSFileProviderExtension (NSFileProviderDomain)

		[iOS (11,0)]
		[NullAllowed, Export ("domain")]
		NSFileProviderDomain Domain { get; }
#endif
	}
#endif // !WATCH

	[Category, BaseType (typeof (NSMutableAttributedString))]
	interface NSMutableAttributedStringKitAdditions {
		[iOS (7,0)] 
		[Export ("fixAttributesInRange:")]
		void FixAttributesInRange (NSRange range);
	}

#if !WATCH
#if XAMCORE_2_0 // NSLayoutAnchor is a generic type, which we only support in Unified (for now)
	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutAnchor<AnchorType>
	{
		[Export ("constraintEqualToAnchor:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutAnchor<AnchorType> anchor);
	
		[Export ("constraintGreaterThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor);
	
		[Export ("constraintLessThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor);
	
		[Export ("constraintEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);
	
		[Export ("constraintGreaterThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);
	
		[Export ("constraintLessThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutAnchor<AnchorType> anchor, nfloat constant);
	}

	[NoWatch]
	[iOS (9,0)]
	[TV (10,0)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutXAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutXAxisAnchor
	{
		[iOS (10,0)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension CreateAnchorWithOffset (NSLayoutXAxisAnchor otherAnchor);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintGreaterThanOrEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintLessThanOrEqualToSystemSpacingAfterAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToSystemSpacingAfterAnchor (NSLayoutXAxisAnchor anchor, nfloat multiplier);
	}

	[NoWatch]
	[iOS (9,0)]
	[TV (10,0)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutYAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutYAxisAnchor
	{
		[iOS (10,0)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension CreateAnchorWithOffset (NSLayoutYAxisAnchor otherAnchor);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintGreaterThanOrEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);

		[TV (11,0), iOS (11,0)]
		[Export ("constraintLessThanOrEqualToSystemSpacingBelowAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToSystemSpacingBelowAnchor (NSLayoutYAxisAnchor anchor, nfloat multiplier);
	}

	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutDimension>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutDimension
	{
		[Export ("constraintEqualToConstant:")]
		NSLayoutConstraint ConstraintEqualTo (nfloat constant);
	
		[Export ("constraintGreaterThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (nfloat constant);
	
		[Export ("constraintLessThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (nfloat constant);
	
		[Export ("constraintEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutDimension anchor, nfloat multiplier);
	
		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier);
	
		[Export ("constraintLessThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier);
	
		[Export ("constraintEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);
	
		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);
	
		[Export ("constraintLessThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualTo (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);
	}
#endif // XAMCORE_2_0

	[NoWatch]
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface NSLayoutConstraint {
		[Static]
		[Export ("constraintsWithVisualFormat:options:metrics:views:")]
		NSLayoutConstraint [] FromVisualFormat (string format, NSLayoutFormatOptions formatOptions, [NullAllowed] NSDictionary metrics, NSDictionary views);

		[Static]
		[Export ("constraintWithItem:attribute:relatedBy:toItem:attribute:multiplier:constant:")]
		NSLayoutConstraint Create (INativeObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, [NullAllowed] INativeObject view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant);

		[Export ("priority")]
		float Priority { get; set;  } // Returns a float, not nfloat.

		[Export ("shouldBeArchived")]
		bool ShouldBeArchived { get; set;  }

		[NullAllowed, Export ("firstItem", ArgumentSemantic.Assign)]
		NSObject FirstItem { get;  }

		[Export ("firstAttribute")]
		NSLayoutAttribute FirstAttribute { get;  }

		[Export ("relation")]
		NSLayoutRelation Relation { get;  }

		[Export ("secondItem", ArgumentSemantic.Assign)]
		NSObject SecondItem { get;  }

		[Export ("secondAttribute")]
		NSLayoutAttribute SecondAttribute { get;  }

		[Export ("multiplier")]
		nfloat Multiplier { get;  }

		[Export ("constant")]
		nfloat Constant { get; set;  }

		[iOS (8,0)]
		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[iOS (8,0)]
		[Static, Export ("activateConstraints:")]
		void ActivateConstraints (NSLayoutConstraint [] constraints);

		[iOS (8,0)]
		[Static, Export ("deactivateConstraints:")]
		void DeactivateConstraints (NSLayoutConstraint [] constraints);

#if XAMCORE_2_0
		[iOS (10,0), TV (10,0)]
		[Export ("firstAnchor", ArgumentSemantic.Copy)]
		[Internal]
		IntPtr _FirstAnchor<AnchorType> ();
	
		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("secondAnchor", ArgumentSemantic.Copy)]
		[Internal]
		IntPtr _SecondAnchor<AnchorType> ();
#endif
	}

	[NoWatch]
	[iOS (7,0)] // Yup, it is declared as appearing in 7.0, even if it shipped with 8.0
	[Category, BaseType(typeof(NSLayoutConstraint))]
	interface NSIdentifier {
		[Export ("identifier")]
		string GetIdentifier ();

		[Export ("setIdentifier:")]
		void SetIdentifier ([NullAllowed] string id);
	}
#endif // !WATCH

	[iOS (6,0)]
	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	interface NSParagraphStyle : NSSecureCoding, NSMutableCopying {
		[Export ("lineSpacing")]
		nfloat LineSpacing { get; [NotImplemented] set; }

		[Export ("paragraphSpacing")]
		nfloat ParagraphSpacing { get; [NotImplemented] set; }

		[Export ("alignment")]
		UITextAlignment Alignment { get; [NotImplemented] set; }

		[Export ("headIndent")]
		nfloat HeadIndent { get; [NotImplemented] set; }

		[Export ("tailIndent")]
		nfloat TailIndent { get; [NotImplemented] set; }

		[Export ("firstLineHeadIndent")]
		nfloat FirstLineHeadIndent { get; [NotImplemented] set; }

		[Export ("minimumLineHeight")]
		nfloat MinimumLineHeight { get; [NotImplemented] set; }

		[Export ("maximumLineHeight")]
		nfloat MaximumLineHeight { get; [NotImplemented] set; }

		[Export ("lineBreakMode")]
		UILineBreakMode LineBreakMode { get; [NotImplemented] set; }

		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; [NotImplemented] set; }

		[Export ("lineHeightMultiple")]
		nfloat LineHeightMultiple { get; [NotImplemented] set; }

		[Export ("paragraphSpacingBefore")]
		nfloat ParagraphSpacingBefore { get; [NotImplemented] set; }

		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; [NotImplemented] set; } // Returns a float, not nfloat.

		[Static]
		[Export ("defaultWritingDirectionForLanguage:")]
		NSWritingDirection GetDefaultWritingDirection (string languageName);

		[Static]
		[Export ("defaultParagraphStyle")]
		NSParagraphStyle Default { get; }

		[iOS (7,0)]
		[Export ("defaultTabInterval")]
		nfloat DefaultTabInterval { get; [NotImplemented] set; }

		[iOS (7,0)]
		[Export ("tabStops", ArgumentSemantic.Copy)]
		NSTextTab[] TabStops { get; [NotImplemented] set; }

		[iOS (9,0)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; [NotImplemented] set; }
	}

	[iOS (6,0)]
	[ThreadSafe]
	[BaseType (typeof (NSParagraphStyle))]
	interface NSMutableParagraphStyle {
		[Export ("lineSpacing")]
		[Override]
		nfloat LineSpacing { get; set; }

		[Export ("alignment")]
		[Override]
		UITextAlignment Alignment { get; set; }

		[Export ("headIndent")]
		[Override]
		nfloat HeadIndent { get; set; }

		[Export ("tailIndent")]
		[Override]
		nfloat TailIndent { get; set; }

		[Export ("firstLineHeadIndent")]
		[Override]
		nfloat FirstLineHeadIndent { get; set; }

		[Export ("minimumLineHeight")]
		[Override]
		nfloat MinimumLineHeight { get; set; }

		[Export ("maximumLineHeight")]
		[Override]
		nfloat MaximumLineHeight { get; set; }

		[Export ("lineBreakMode")]
		[Override]
		UILineBreakMode LineBreakMode { get; set; }

		[Export ("baseWritingDirection")]
		[Override]
		NSWritingDirection BaseWritingDirection { get; set; }

		[Export ("lineHeightMultiple")]
		[Override]
		nfloat LineHeightMultiple { get; set; }

		[Export ("paragraphSpacing")]
		[Override]
		nfloat ParagraphSpacing { get; set; }

		[Export ("paragraphSpacingBefore")]
		[Override]
		nfloat ParagraphSpacingBefore { get; set; }

		[Export ("hyphenationFactor")]
		[Override]
		float HyphenationFactor { get; set; } // Returns a float, not nfloat.

		[iOS (7,0)]
		[Export ("defaultTabInterval")]
		[Override]
		nfloat DefaultTabInterval { get; set; }

		[iOS (7,0)]
		[Export ("tabStops", ArgumentSemantic.Copy)]
		[Override]
		NSTextTab[] TabStops { get; set; }

		[iOS (9,0)]
		[Override]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[iOS (9,0)]
		[Export ("addTabStop:")]
		void AddTabStop (NSTextTab textTab);

		[iOS (9,0)]
		[Export ("removeTabStop:")]
		void RemoveTabStop (NSTextTab textTab);

		[iOS (9,0)]
		[Export ("setParagraphStyle:")]
		void SetParagraphStyle (NSParagraphStyle paragraphStyle);
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface NSTextTab : NSCoding, NSCopying, NSSecureCoding {

		[DesignatedInitializer]
		[Export ("initWithTextAlignment:location:options:")]
		[PostGet ("Options")]
		IntPtr Constructor (UITextAlignment alignment, nfloat location, [NullAllowed] NSDictionary options);

		[Export ("alignment")]
		UITextAlignment Alignment { get; }

		[Export ("location")]
		nfloat Location { get; }

		[Export ("options")]
		NSDictionary Options { get; }

		[Static]
		[Export ("columnTerminatorsForLocale:")]
		NSCharacterSet GetColumnTerminators ([NullAllowed] NSLocale locale);

		[Field ("NSTabColumnTerminatorsAttributeName")]
		NSString ColumnTerminatorsAttributeName { get; }
	}

#if !WATCH
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	[iOS (6,0)]
	interface NSShadow : NSSecureCoding, NSCopying {
		[Export ("shadowOffset", ArgumentSemantic.Assign)]
		CGSize ShadowOffset { get; set; }
		
		[Export ("shadowBlurRadius", ArgumentSemantic.Assign)]
		nfloat ShadowBlurRadius { get; set;  }

		[Export ("shadowColor", ArgumentSemantic.Retain), NullAllowed]
		UIColor ShadowColor { get; set;  }
	}

	[Model]
	[Protocol]
	[BaseType (typeof (NSObject))]
	partial interface NSTextAttachmentContainer {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("imageForBounds:textContainer:characterIndex:")]
		UIImage GetImageForBounds (CGRect bounds, NSTextContainer textContainer, nuint characterIndex);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("attachmentBoundsForTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetAttachmentBounds (NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint glyphPosition, nuint characterIndex);
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	partial interface NSTextAttachment : NSTextAttachmentContainer, NSCoding
#if !WATCH
	, UIAccessibilityContentSizeCategoryImageAdjusting
#endif // !WATCH
	{
		[DesignatedInitializer]
		[Export ("initWithData:ofType:")]
		[PostGet ("Contents")]
		IntPtr Constructor ([NullAllowed] NSData contentData, [NullAllowed] string uti);

		[NullAllowed] // by default this property is null
		[Export ("contents", ArgumentSemantic.Retain)]
		NSData Contents { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fileType", ArgumentSemantic.Retain)]
		string FileType { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("image", ArgumentSemantic.Retain)]
		UIImage Image { get; set; }

		[Export ("bounds")]
		CGRect Bounds { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fileWrapper", ArgumentSemantic.Retain)]
		NSFileWrapper FileWrapper { get; set; }
	}

	[Protocol]
	// no [Model] since it's not exposed in any API 
	// only NSTextContainer conforms to it but it's only queried by iOS itself
	interface NSTextLayoutOrientationProvider {

		[Abstract]
		[Export ("layoutOrientation")]
		NSTextLayoutOrientation LayoutOrientation {
			get;
#if !XAMCORE_3_0
			[NotImplemented] set;
#endif
		}
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	partial interface NSTextContainer : NSTextLayoutOrientationProvider, NSCoding {

		[DesignatedInitializer]
		[Export ("initWithSize:")]
		IntPtr Constructor (CGSize size);

		[NullAllowed] // by default this property is null
		[Export ("layoutManager", ArgumentSemantic.Assign)]
		NSLayoutManager LayoutManager { get; set; }

		[Export ("size")]
		CGSize Size { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("exclusionPaths", ArgumentSemantic.Copy)]
		UIBezierPath [] ExclusionPaths { get; set; }

		[Export ("lineBreakMode")]
		UILineBreakMode LineBreakMode { get; set; }

		[Export ("lineFragmentPadding")]
		nfloat LineFragmentPadding { get; set; }

		[Export ("maximumNumberOfLines")]
		nuint MaximumNumberOfLines { get; set; }

		[Export ("lineFragmentRectForProposedRect:atIndex:writingDirection:remainingRect:")]
		CGRect GetLineFragmentRect (CGRect proposedRect, nuint characterIndex, NSWritingDirection baseWritingDirection, out CGRect remainingRect);

		[Export ("widthTracksTextView")]
		bool WidthTracksTextView { get; set; }

		[Export ("heightTracksTextView")]
		bool HeightTracksTextView { get; set; }

		[iOS (9,0)]
		[Export ("replaceLayoutManager:")]
		void ReplaceLayoutManager (NSLayoutManager newLayoutManager);

		[iOS (9,0)]
		[Export ("simpleRectangularTextContainer")]
		bool IsSimpleRectangularTextContainer { [Bind ("isSimpleRectangularTextContainer")] get; }
		
	}

	[iOS (7,0)]
	[BaseType (typeof (NSMutableAttributedString), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTextStorageDelegate)})]
	partial interface NSTextStorage {
		[Export ("layoutManagers")]
		NSObject [] LayoutManagers { get; }

		[Export ("addLayoutManager:")]
		void AddLayoutManager (NSLayoutManager aLayoutManager);

		[Export ("removeLayoutManager:")]
		[PostGet ("LayoutManagers")]
		void RemoveLayoutManager (NSLayoutManager aLayoutManager);

		[Export ("editedMask")]
		NSTextStorageEditActions EditedMask { get; set; }

		[Export ("editedRange")]
		NSRange EditedRange { get;
#if !XAMCORE_3_0
			[NotImplemented] set;
#endif
		}

		[Export ("changeInLength")]
		nint ChangeInLength { get;
#if !XAMCORE_3_0
			[NotImplemented] set;
#endif
		}

		[NullAllowed] // by default this property is null
		[Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTextStorageDelegate Delegate { get; set; }

		[Export ("edited:range:changeInLength:")]
		void Edited (NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

		[Export ("processEditing")]
		void ProcessEditing ();

		[Export ("fixesAttributesLazily")]
		bool FixesAttributesLazily { get; }

		[Export ("invalidateAttributesInRange:")]
		void InvalidateAttributes (NSRange range);

		[Export ("ensureAttributesAreFixedInRange:")]
		void EnsureAttributesAreFixed (NSRange range);

		[iOS (7,0)]
		[Notification, Internal, Field ("NSTextStorageWillProcessEditingNotification")]
		NSString WillProcessEditingNotification { get; }

		[iOS (7,0)]
		[Notification, Internal, Field ("NSTextStorageDidProcessEditingNotification")]
		NSString DidProcessEditingNotification { get; }
	}

	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	partial interface NSTextStorageDelegate {
		[Export ("textStorage:willProcessEditing:range:changeInLength:")][EventArgs ("NSTextStorage")]
		void WillProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

		[Export ("textStorage:didProcessEditing:range:changeInLength:")][EventArgs ("NSTextStorage")]
		void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSLayoutManager : NSCoding {
		[NullAllowed] // by default this property is null
		[Export ("textStorage", ArgumentSemantic.Assign)]
		NSTextStorage TextStorage { get; set; }

		[Export ("textContainers")]
		NSTextContainer [] TextContainers { get; }

		[Export ("addTextContainer:")]
		[PostGet ("TextContainers")]
		void AddTextContainer (NSTextContainer container);

		[Export ("insertTextContainer:atIndex:")]
		[PostGet ("TextContainers")]
		void InsertTextContainer (NSTextContainer container, nint index);

		[Export ("removeTextContainerAtIndex:")]
		[PostGet ("TextContainers")]
		void RemoveTextContainer (nint index);

		[Export ("textContainerChangedGeometry:")]
		void TextContainerChangedGeometry (NSTextContainer container);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSLayoutManagerDelegate Delegate { get; set; }

		[Export ("showsInvisibleCharacters")]
		bool ShowsInvisibleCharacters { get; set; }
	
		[Export ("showsControlCharacters")]
		bool ShowsControlCharacters { get; set; }
	
		[Export ("hyphenationFactor")]
		nfloat HyphenationFactor { get; set; }
	
		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }
	
		[Export ("allowsNonContiguousLayout")]
		bool AllowsNonContiguousLayout { get; set; }
	
		[Export ("hasNonContiguousLayout")]
		bool HasNonContiguousLayout { get; }
	
		[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
		void InvalidateGlyphs (NSRange charRange, nint delta, out NSRange actualCharRange);

		[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
		void InvalidateLayout (NSRange charRange, out NSRange actualCharRange);
				

		[Export ("invalidateDisplayForCharacterRange:")]
		void InvalidateDisplayForCharacterRange (NSRange charRange);
		
		[Export ("invalidateDisplayForGlyphRange:")]
		void InvalidateDisplayForGlyphRange (NSRange glyphRange);

		[Export ("processEditingForTextStorage:edited:range:changeInLength:invalidatedRange:")]
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharRange, nint delta, NSRange invalidatedCharRange);

		[Export ("ensureGlyphsForCharacterRange:")]
		void EnsureGlyphsForCharacterRange (NSRange charRange);
		
		[Export ("ensureGlyphsForGlyphRange:")]
		void EnsureGlyphsForGlyphRange (NSRange glyphRange);
		
		[Export ("ensureLayoutForCharacterRange:")]
		void EnsureLayoutForCharacterRange (NSRange charRange);
		
		[Export ("ensureLayoutForGlyphRange:")]
		void EnsureLayoutForGlyphRange (NSRange glyphRange);
		
		[Export ("ensureLayoutForTextContainer:")]
		void EnsureLayoutForTextContainer (NSTextContainer container);
		
		[Export ("ensureLayoutForBoundingRect:inTextContainer:")]
		void EnsureLayoutForBoundingRect (CGRect bounds, NSTextContainer container);

		[Export ("setGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		void SetGlyphs (IntPtr glyphs, IntPtr props, IntPtr charIndexes, UIFont aFont, NSRange glyphRange);

		[Export ("numberOfGlyphs")]
		nuint NumberOfGlyphs { get; }

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'GetGlyph' instead.")]
		[Export ("glyphAtIndex:isValidIndex:")]
		ushort GlyphAtIndex (nuint glyphIndex, ref bool isValidIndex);

		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'GetGlyph' instead.")]
		[Export ("glyphAtIndex:")]
		ushort GlyphAtIndex (nuint glyphIndex);

		[Export ("isValidGlyphIndex:")]
		bool IsValidGlyphIndex (nuint glyphIndex);

		[Export ("propertyForGlyphAtIndex:")]
		NSGlyphProperty PropertyForGlyphAtIndex (nuint glyphIndex);

		[Export ("characterIndexForGlyphAtIndex:")]
		nuint CharacterIndexForGlyphAtIndex (nuint glyphIndex);

		[Export ("glyphIndexForCharacterAtIndex:")]
		nuint GlyphIndexForCharacterAtIndex (nuint charIndex);

		[Export ("getGlyphsInRange:glyphs:properties:characterIndexes:bidiLevels:")]
		[Internal]
		nuint GetGlyphsInternal (NSRange glyphRange, IntPtr glyphBuffer, IntPtr props, IntPtr charIndexBuffer, IntPtr bidiLevelBuffer);

		[Export ("setTextContainer:forGlyphRange:")]
		void SetTextContainer (NSTextContainer container, NSRange glyphRange);

		[Export ("setLineFragmentRect:forGlyphRange:usedRect:")]
		void SetLineFragmentRect (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect);

		[Export ("setExtraLineFragmentRect:usedRect:textContainer:")]
		void SetExtraLineFragmentRect (CGRect fragmentRect, CGRect usedRect, NSTextContainer container);

		[Export ("setLocation:forStartOfGlyphRange:")]
		void SetLocation (CGPoint location, NSRange glyphRange);

		[Export ("setNotShownAttribute:forGlyphAtIndex:")]
		void SetNotShownAttribute (bool flag, nuint glyphIndex);

		[Export ("setDrawsOutsideLineFragment:forGlyphAtIndex:")]
		void SetDrawsOutsideLineFragment (bool flag, nuint glyphIndex);

		[Export ("setAttachmentSize:forGlyphRange:")]
		void SetAttachmentSize (CGSize attachmentSize, NSRange glyphRange);

		[Export ("getFirstUnlaidCharacterIndex:glyphIndex:")]
		void GetFirstUnlaidCharacterIndex (ref nuint charIndex, ref nuint glyphIndex);

		[Export ("firstUnlaidCharacterIndex")]
		nuint FirstUnlaidCharacterIndex { get; }

		[Export ("firstUnlaidGlyphIndex")]
		nuint FirstUnlaidGlyphIndex { get; }

		[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		[Internal]
		NSTextContainer TextContainerForGlyphAtIndexInternal (nuint glyphIndex, IntPtr effectiveGlyphRange);

		[Export ("usedRectForTextContainer:")]
		CGRect GetUsedRectForTextContainer (NSTextContainer container);

		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		[Internal]
		CGRect LineFragmentRectForGlyphAtIndexInternal (nuint glyphIndex, IntPtr effectiveGlyphRange);

		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		[Internal]
		CGRect LineFragmentUsedRectForGlyphAtIndexInternal (nuint glyphIndex, IntPtr effectiveGlyphRange);

		[Export ("extraLineFragmentRect", ArgumentSemantic.Copy)]
		CGRect ExtraLineFragmentRect { get; }

		[Export ("extraLineFragmentUsedRect", ArgumentSemantic.Copy)]
		CGRect ExtraLineFragmentUsedRect { get; }

		[Export ("extraLineFragmentTextContainer", ArgumentSemantic.Copy)]
		NSTextContainer ExtraLineFragmentTextContainer { get; }
		
		[Export ("locationForGlyphAtIndex:")]
		CGPoint LocationForGlyphAtIndex (nuint glyphIndex);
		
		[Export ("notShownAttributeForGlyphAtIndex:")]
		bool NotShownAttributeForGlyphAtIndex (nuint glyphIndex);

		[Export ("drawsOutsideLineFragmentForGlyphAtIndex:")]
		bool DrawsOutsideLineFragmentForGlyphAtIndex (nuint glyphIndex);

		[Export ("attachmentSizeForGlyphAtIndex:")]
		CGSize AttachmentSizeForGlyphAtIndex (nuint glyphIndex);

		[Export ("truncatedGlyphRangeInLineFragmentForGlyphAtIndex:")]
		NSRange TruncatedGlyphRangeInLineFragmentForGlyphAtIndex (nuint glyphIndex);

		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		[Internal]
		NSRange GlyphRangeForCharacterRangeInternal (NSRange charRange, IntPtr actualCharRange);

		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
#if XAMCORE_2_0
		[Internal]
#endif
		NSRange CharacterRangeForGlyphRangeInternal (NSRange glyphRange, IntPtr actualGlyphRange);

		[Export ("glyphRangeForTextContainer:")]
		NSRange GetGlyphRange (NSTextContainer container);

		[Export ("rangeOfNominallySpacedGlyphsContainingIndex:")]
		NSRange RangeOfNominallySpacedGlyphsContainingIndex (nuint glyphIndex);

		[Export ("boundingRectForGlyphRange:inTextContainer:")]
		CGRect BoundingRectForGlyphRange (NSRange glyphRange, NSTextContainer container);

		[Export ("glyphRangeForBoundingRect:inTextContainer:")]
		NSRange GlyphRangeForBoundingRect (CGRect bounds, NSTextContainer container);

		[Export ("glyphRangeForBoundingRectWithoutAdditionalLayout:inTextContainer:")]
		NSRange GlyphRangeForBoundingRectWithoutAdditionalLayout (CGRect bounds, NSTextContainer container);

		[Export ("glyphIndexForPoint:inTextContainer:fractionOfDistanceThroughGlyph:")]
		nuint GlyphIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat partialFraction);

		[Export ("glyphIndexForPoint:inTextContainer:")]
		nuint GlyphIndexForPoint (CGPoint point, NSTextContainer container);

		[Export ("fractionOfDistanceThroughGlyphForPoint:inTextContainer:")]
		nfloat FractionOfDistanceThroughGlyphForPoint (CGPoint point, NSTextContainer container);

		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint CharacterIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat partialFraction);

		[Export ("getLineFragmentInsertionPointsForCharacterAtIndex:alternatePositions:inDisplayOrder:positions:characterIndexes:")]
#if XAMCORE_2_0
		[Internal]
#endif
		nuint GetLineFragmentInsertionPoints (nuint charIndex, bool alternatePosition, bool inDisplayOrder, IntPtr positions, IntPtr charIndexes);

		[Export ("enumerateLineFragmentsForGlyphRange:usingBlock:")]
		void EnumerateLineFragments (NSRange glyphRange, NSTextLayoutEnumerateLineFragments callback);

		[Export ("enumerateEnclosingRectsForGlyphRange:withinSelectedGlyphRange:inTextContainer:usingBlock:")]
		void EnumerateEnclosingRects (NSRange glyphRange, NSRange selectedRange, NSTextContainer textContainer, NSTextLayoutEnumerateEnclosingRects callback);

		[Export ("drawBackgroundForGlyphRange:atPoint:")]
		void DrawBackgroundForGlyphRange (NSRange glyphsToShow, CGPoint origin);

		[Export ("drawGlyphsForGlyphRange:atPoint:")]
		void DrawGlyphs (NSRange glyphsToShow, CGPoint origin);

		[Export ("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		[Internal]
		void ShowCGGlyphsInternal (IntPtr glyphs, IntPtr positions, nuint glyphCount, UIFont font, CGAffineTransform textMatrix, NSDictionary attributes, [NullAllowed] CGContext graphicsContext);
	
		// Can't make this internal and expose a manually written API, since you're supposed to override this method, not call it yourself.
		//[Export ("fillBackgroundRectArray:count:forCharacterRange:color:")]
		//void FillBackgroundRectArray (IntPtr rectArray, nuint rectCount, NSRange charRange, UIColor color);

		[Export ("drawUnderlineForGlyphRange:underlineType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void DrawUnderline (NSRange glyphRange, NSUnderlineStyle underlineVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[Export ("underlineGlyphRange:underlineType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void Underline (NSRange glyphRange, NSUnderlineStyle underlineVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[Export ("drawStrikethroughForGlyphRange:strikethroughType:baselineOffset:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void DrawStrikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, nfloat baselineOffset, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[Export ("strikethroughGlyphRange:strikethroughType:lineFragmentRect:lineFragmentGlyphRange:containerOrigin:")]
		void Strikethrough (NSRange glyphRange, NSUnderlineStyle strikethroughVal, CGRect lineRect, NSRange lineGlyphRange, CGPoint containerOrigin);

		[iOS (9,0)]
		[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		[return: NullAllowed]
		NSTextContainer GetTextContainer (nuint glyphIndex, out NSRange effectiveGlyphRange, bool withoutAdditionalLayout);
		
		[iOS (9,0)]
		[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentRect (nuint glyphIndex, out NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		[iOS (9,0)]
		[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		CGRect GetLineFragmentUsedRect (nuint glyphIndex, out NSRange effectiveGlyphRange, bool withoutAdditionalLayout);

		[iOS (9,0)] // documented as 7.0 but missing in 8.x
		[Export ("CGGlyphAtIndex:isValidIndex:")]
		unsafe ushort GetGlyph (nuint glyphIndex, ref bool isValidIndex);
	
		[iOS (9,0)] // documented as 7.0 but missing in 8.x
		[Export ("CGGlyphAtIndex:")]
		ushort GetGlyph (nuint glyphIndex);

		[NoWatch]
		[TV (12, 0), iOS (12, 0)]
		[Export ("limitsLayoutForSuspiciousContents")]
		bool LimitsLayoutForSuspiciousContents { get; set; }
		
	}
	
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	[iOS (7,0)]
	partial interface NSLayoutManagerDelegate {
		[Export ("layoutManager:shouldGenerateGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		nuint ShouldGenerateGlyphs (NSLayoutManager layoutManager, IntPtr glyphBuffer, IntPtr props, IntPtr charIndexes, UIFont aFont, NSRange glyphRange);
	
		[Export ("layoutManager:lineSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat LineSpacingAfterGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
	
		[Export ("layoutManager:paragraphSpacingBeforeGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat ParagraphSpacingBeforeGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
	
		[Export ("layoutManager:paragraphSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat ParagraphSpacingAfterGlyphAtIndex (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);
	
		[Export ("layoutManager:shouldUseAction:forControlCharacterAtIndex:")]
		NSControlCharacterAction ShouldUseAction (NSLayoutManager layoutManager, NSControlCharacterAction action, nuint charIndex);
	
		[Export ("layoutManager:shouldBreakLineByWordBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByWordBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);
	
		[Export ("layoutManager:shouldBreakLineByHyphenatingBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByHyphenatingBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);
	
		[Export ("layoutManager:boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect BoundingBoxForControlGlyph (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint charIndex);
	
		[Export ("layoutManagerDidInvalidateLayout:")]
		void DidInvalidatedLayout  (NSLayoutManager sender);
	
		[Export ("layoutManager:didCompleteLayoutForTextContainer:atEnd:")]
		void DidCompleteLayout (NSLayoutManager layoutManager, NSTextContainer textContainer, bool layoutFinishedFlag);
	
		[Export ("layoutManager:textContainer:didChangeGeometryFromSize:")]
		void DidChangeGeometry (NSLayoutManager layoutManager, NSTextContainer textContainer, CGSize oldSize);

		[iOS (9,0)]
		[Export ("layoutManager:shouldSetLineFragmentRect:lineFragmentUsedRect:baselineOffset:inTextContainer:forGlyphRange:")]
		bool ShouldSetLineFragmentRect (NSLayoutManager layoutManager, ref CGRect lineFragmentRect, ref CGRect lineFragmentUsedRect, ref nfloat baselineOffset, NSTextContainer textContainer, NSRange glyphRange);
	}
#endif // !WATCH

	[Category]
	[BaseType (typeof (NSCoder))]
	interface NSCoder_UIGeometryKeyedCoding {
		[Export ("encodeCGPoint:forKey:")]
		void Encode (CGPoint point, string forKey);
		
		[iOS(8,0)]
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

		[Watch (4,0), TV (11,0), iOS (11,0)]
		[Export ("encodeDirectionalEdgeInsets:forKey:")]
		void Encode (NSDirectionalEdgeInsets directionalEdgeInsets, string forKey);
		
		[Export ("encodeUIOffset:forKey:")]
		void Encode (UIOffset uiOffset, string forKey);
		
		[Export ("decodeCGPointForKey:")]
		CGPoint DecodeCGPoint (string key);

		[iOS(8,0)]
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

		[Watch (4,0), TV (11,0), iOS (11,0)]
		[Export ("decodeDirectionalEdgeInsetsForKey:")]
		NSDirectionalEdgeInsets DecodeDirectionalEdgeInsets (string key);
		
		[Export ("decodeUIOffsetForKey:")]
		UIOffset DecodeUIOffsetForKey (string key);
	}

#if !WATCH
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'CoreMotion' instead.")]
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

	[NoTV]
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIAccelerometerDelegate)})]
	[Availability (Deprecated = Platform.iOS_5_0, Message = "Use 'CoreMotion' instead.")]
	interface UIAccelerometer {
		[Static] [Export ("sharedAccelerometer")]
		UIAccelerometer SharedAccelerometer { get; }

		[Export ("updateInterval")]
		double UpdateInterval { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIAccelerometerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIAccelerometerDelegate {
#pragma warning disable 618
		[Export ("accelerometer:didAccelerate:"), EventArgs ("UIAccelerometer"), EventName ("Acceleration")]
		void DidAccelerate (UIAccelerometer accelerometer, UIAcceleration acceleration);
#pragma warning restore 618
	}
#endif // !WATCH

	interface UIAccessibility {
		[Export ("isAccessibilityElement")]
		bool IsAccessibilityElement { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityLabel", ArgumentSemantic.Copy)]
		string AccessibilityLabel { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[NullAllowed, Export ("accessibilityAttributedLabel", ArgumentSemantic.Copy)]
		NSAttributedString AccessibilityAttributedLabel { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityHint", ArgumentSemantic.Copy)]
		string AccessibilityHint { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[NullAllowed, Export ("accessibilityAttributedHint", ArgumentSemantic.Copy)]
		NSAttributedString AccessibilityAttributedHint { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("accessibilityValue", ArgumentSemantic.Copy)]
		string AccessibilityValue { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
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

		[iOS (6,0)]
		[Export ("shouldGroupAccessibilityChildren")]
		bool ShouldGroupAccessibilityChildren { get; set; }

		[iOS (8,0)]
		[Export ("accessibilityNavigationStyle")]
		UIAccessibilityNavigationStyle AccessibilityNavigationStyle { get; set; }

		[Field ("UIAccessibilityTraitNone")]
		long TraitNone { get; }

		[Field ("UIAccessibilityTraitButton")]
		long TraitButton { get; }

		[Field ("UIAccessibilityTraitLink")]
		long TraitLink { get; }

		[iOS (6,0)]
		[Field ("UIAccessibilityTraitHeader")]
		long TraitHeader { get; }

		[Field ("UIAccessibilityTraitSearchField")]
		long TraitSearchField { get; }

		[Field ("UIAccessibilityTraitImage")]
		long TraitImage { get; }

		[Field ("UIAccessibilityTraitSelected")]
		long TraitSelected { get; }

		[Field ("UIAccessibilityTraitPlaysSound")]
		long TraitPlaysSound { get; }

		[Field ("UIAccessibilityTraitKeyboardKey")]
		long TraitKeyboardKey { get; }

		[Field ("UIAccessibilityTraitStaticText")]
		long TraitStaticText { get; }

		[Field ("UIAccessibilityTraitSummaryElement")]
		long TraitSummaryElement { get; }

		[Field ("UIAccessibilityTraitNotEnabled")]
		long TraitNotEnabled { get; }

		[Field ("UIAccessibilityTraitUpdatesFrequently")]
		long TraitUpdatesFrequently { get; }

		[Field ("UIAccessibilityTraitStartsMediaSession")]
		long TraitStartsMediaSession { get; }

		[Field ("UIAccessibilityTraitAdjustable")]
		long TraitAdjustable { get; }

		[Field ("UIAccessibilityTraitAllowsDirectInteraction")]
		long TraitAllowsDirectInteraction { get; }

		[Field ("UIAccessibilityTraitCausesPageTurn")]
		long TraitCausesPageTurn { get; }

		[iOS (10,0), TV (10,0), Watch (3,0)]
		[Field ("UIAccessibilityTraitTabBar")]
		long TraitTabBar { get; }
	
		[iOS (6,0)]
		[Field ("UIAccessibilityAnnouncementDidFinishNotification")]
		[Notification (typeof (UIAccessibilityAnnouncementFinishedEventArgs))]
		NSString AnnouncementDidFinishNotification { get; }

		[NoWatch]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'VoiceOverStatusDidChangeNotification' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'VoiceOverStatusDidChangeNotification' instead.")]
		[Field ("UIAccessibilityVoiceOverStatusChanged")]
		NSString VoiceOverStatusChanged { get; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Field ("UIAccessibilityVoiceOverStatusDidChangeNotification")]
		[Notification]
		NSString VoiceOverStatusDidChangeNotification { get; }

		[NoWatch]
		[Field ("UIAccessibilityMonoAudioStatusDidChangeNotification")]
		[Notification]
		NSString MonoAudioStatusDidChangeNotification { get; }

		[NoWatch]
		[Field ("UIAccessibilityClosedCaptioningStatusDidChangeNotification")]
		[Notification]
		NSString ClosedCaptioningStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (6,0)]
		[Field ("UIAccessibilityInvertColorsStatusDidChangeNotification")]
		[Notification]
		NSString InvertColorsStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (6,0)]
		[Field ("UIAccessibilityGuidedAccessStatusDidChangeNotification")]
		[Notification]
		NSString GuidedAccessStatusDidChangeNotification { get; }

		[Field ("UIAccessibilityScreenChangedNotification")]
		int ScreenChangedNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityLayoutChangedNotification")]
		int LayoutChangedNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityAnnouncementNotification")]
		int AnnouncementNotification { get; } // This is int, not nint

		[Field ("UIAccessibilityPageScrolledNotification")]
		int PageScrolledNotification { get; } // This is int, not nint

		[iOS (7,0)]
		[NullAllowed] // by default this property is null
		[Export ("accessibilityPath", ArgumentSemantic.Copy)]
		UIBezierPath AccessibilityPath { get; set; }

		[iOS (7,0)]
		[Export ("accessibilityActivate")]
		bool AccessibilityActivate ();

		[iOS (7,0)]
		[Field ("UIAccessibilitySpeechAttributePunctuation")]
		NSString SpeechAttributePunctuation { get; }

		[iOS (7,0)]
		[Field ("UIAccessibilitySpeechAttributeLanguage")]
		NSString SpeechAttributeLanguage { get; }
		
		[iOS (7,0)]
		[Field ("UIAccessibilitySpeechAttributePitch")]
		NSString SpeechAttributePitch { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilityBoldTextStatusDidChangeNotification")]
		NSString BoldTextStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilityDarkerSystemColorsStatusDidChangeNotification")]
		NSString DarkerSystemColorsStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilityGrayscaleStatusDidChangeNotification")]
		NSString GrayscaleStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilityReduceMotionStatusDidChangeNotification")]
		NSString ReduceMotionStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilityReduceTransparencyStatusDidChangeNotification")]
		NSString ReduceTransparencyStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilitySwitchControlStatusDidChangeNotification")]
		NSString SwitchControlStatusDidChangeNotification { get; }

		[iOS (8,0)]
		[Field ("UIAccessibilityNotificationSwitchControlIdentifier")]
		NSString NotificationSwitchControlIdentifier { get; }


		// Chose int because this should be UIAccessibilityNotifications type
		// just like UIAccessibilityAnnouncementNotification field
		[iOS (8,0)]
		//[Notification] // int ScreenChangedNotification doesn't use this attr either
		[Field ("UIAccessibilityPauseAssistiveTechnologyNotification")]
		int PauseAssistiveTechnologyNotification { get; } // UIAccessibilityNotifications => uint32_t

		// Chose int because this should be UIAccessibilityNotifications type
		// just like UIAccessibilityAnnouncementNotification field
		[iOS (8,0)]
		//[Notification] // int ScreenChangedNotification doesn't use this attr either
		[Field ("UIAccessibilityResumeAssistiveTechnologyNotification")]
		int ResumeAssistiveTechnologyNotification { get; } // UIAccessibilityNotifications => uint32_t

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilitySpeakScreenStatusDidChangeNotification")]
		NSString SpeakScreenStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (8,0)]
		[Notification]
		[Field ("UIAccessibilitySpeakSelectionStatusDidChangeNotification")]
		NSString SpeakSelectionStatusDidChangeNotification { get; }

		[NoWatch]
		[iOS (9,0)]
		[Notification]
		[Field ("UIAccessibilityShakeToUndoDidChangeNotification")]
		NSString ShakeToUndoDidChangeNotification { get; }

		// FIXME: we only used this on a few types before, none of them available on tvOS
		// but a new member was added to the platform... 
		[TV (9,0), NoWatch, NoiOS]
		[NullAllowed, Export ("accessibilityHeaderElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityHeaderElements { get; set; }

		[iOS (9, 0)]
		[Notification]
		[Field ("UIAccessibilityElementFocusedNotification")]
		NSString ElementFocusedNotification { get; }

		[iOS (9, 0)]
		[Notification]
		[Field ("UIAccessibilityFocusedElementKey")]
		NSString FocusedElementKey { get; }

		[iOS (9, 0)]
		[Notification]
		[Field ("UIAccessibilityUnfocusedElementKey")]
		NSString UnfocusedElementKey { get; }

		[iOS (9, 0)]
		[Notification]
		[Field ("UIAccessibilityAssistiveTechnologyKey")]
		NSString AssistiveTechnologyKey { get; }

		[iOS (9, 0)]
		[Field ("UIAccessibilityNotificationVoiceOverIdentifier")]
		NSString NotificationVoiceOverIdentifier { get; }

		[NoWatch]
		[NoTV]
		[iOS (10,0)]
		[Notification]
		[Field ("UIAccessibilityHearingDevicePairedEarDidChangeNotification")]
		NSString HearingDevicePairedEarDidChangeNotification { get; }

		[NoWatch]
		[iOS (10,0), TV (10,0)]
		[Notification]
		[Field ("UIAccessibilityAssistiveTouchStatusDidChangeNotification")]
		NSString AssistiveTouchStatusDidChangeNotification { get; }

		[iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("UIAccessibilitySpeechAttributeQueueAnnouncement")]
		NSString SpeechAttributeQueueAnnouncement { get; }

		[iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("UIAccessibilitySpeechAttributeIPANotation")]
		NSString SpeechAttributeIpaNotation { get; }

		[iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("UIAccessibilityTextAttributeHeadingLevel")]
		NSString TextAttributeHeadingLevel { get; }

		[iOS (11,0), TV (11,0), Watch (4,0)]
		[Field ("UIAccessibilityTextAttributeCustom")]
		NSString TextAttributeCustom { get; }
	}

	interface UIAccessibilityAnnouncementFinishedEventArgs {
		[Export ("UIAccessibilityAnnouncementKeyStringValue")]
		string Announcement { get; }

		[Export ("UIAccessibilityAnnouncementKeyWasSuccessful")]
		bool WasSuccessful { get; }
	}

#if !WATCH
	[Protocol (IsInformal = true)]
	interface UIAccessibilityContainer {
		[Export ("accessibilityElementCount")]
		nint AccessibilityElementCount ();

		[Export ("accessibilityElementAtIndex:")]
		NSObject GetAccessibilityElementAt (nint index);

		[Export ("indexOfAccessibilityElement:")]
		nint GetIndexOfAccessibilityElement (NSObject element);

		[Export ("accessibilityElements")]
		[iOS (8,0)]
		NSObject GetAccessibilityElements ();

		[iOS (8,0)]
		[Export ("setAccessibilityElements:")]
		void SetAccessibilityElements ([NullAllowed] NSObject elements);

		[iOS (11,0), TV (11,0)]
		[Export ("accessibilityContainerType", ArgumentSemantic.Assign)]
		UIAccessibilityContainerType AccessibilityContainerType { get; set; }
	}

	interface IUIAccessibilityContainerDataTableCell {}

	[iOS (11,0), TV (11,0)]
	[Protocol]
	interface UIAccessibilityContainerDataTableCell {
		[Abstract]
		[Export ("accessibilityRowRange")]
		NSRange GetAccessibilityRowRange ();

		[Abstract]
		[Export ("accessibilityColumnRange")]
		NSRange GetAccessibilityColumnRange ();
	}

	[iOS (11,0), TV (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
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
		IUIAccessibilityContainerDataTableCell[] GetAccessibilityHeaderElementsForRow (nuint row);

		[Export ("accessibilityHeaderElementsForColumn:")]
		[return: NullAllowed]
		IUIAccessibilityContainerDataTableCell[] GetAccessibilityHeaderElementsForColumn (nuint column);
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Please use the designated initializer
	partial interface UIAccessibilityCustomAction {
	    [Export ("initWithName:target:selector:")]
	    IntPtr Constructor (string name, NSObject target, Selector selector);
	
		[TV (11,0), iOS (11,0)]
		[Export ("initWithAttributedName:target:selector:")]
		IntPtr Constructor (NSAttributedString attributedName, [NullAllowed] NSObject target, Selector selector);

		[NullAllowed] // by default this property is null
	    [Export ("name")]
	    string Name { get; set; }
	
		[TV (11,0), iOS (11,0)]
		[Export ("attributedName", ArgumentSemantic.Copy)]
		NSAttributedString AttributedName { get; set; }

		[NullAllowed] // by default this property is null
	    [Export ("target", ArgumentSemantic.Weak)]
	    NSObject Target { get; set; }
	
		[NullAllowed] // by default this property is null
	    [Export ("selector", ArgumentSemantic.UnsafeUnretained)]
	    Selector Selector { get; set; }
	}

	delegate UIAccessibilityCustomRotorItemResult UIAccessibilityCustomRotorSearch (UIAccessibilityCustomRotorSearchPredicate predicate);
	
	[iOS (10,0), TV (10,0)]
	[BaseType (typeof (NSObject))]
	interface UIAccessibilityCustomRotor {

		[Export ("initWithName:itemSearchBlock:")]
		IntPtr Constructor (string name, UIAccessibilityCustomRotorSearch itemSearchHandler);

		[iOS (11,0), TV (11,0)]
		[Export ("initWithAttributedName:itemSearchBlock:")]
		IntPtr Constructor (NSAttributedString attributedName, UIAccessibilityCustomRotorSearch itemSearchBlock);

		[iOS (11,0), TV (11,0)]
		[Export ("initWithSystemType:itemSearchBlock:")]
		IntPtr Constructor (UIAccessibilityCustomSystemRotorType type, UIAccessibilityCustomRotorSearch itemSearchBlock);

		[Export ("name")]
		string Name { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("attributedName", ArgumentSemantic.Copy)]
		NSAttributedString AttributedName { get; set; }

		[Export ("itemSearchBlock", ArgumentSemantic.Copy)]
		UIAccessibilityCustomRotorSearch ItemSearchHandler { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("systemRotorType")]
		UIAccessibilityCustomSystemRotorType SystemRotorType { get; }
	}

	[iOS (10,0), TV (10,0)]
	[Category]
	[BaseType (typeof (NSObject))]
	interface NSObject_UIAccessibilityCustomRotor {

		[Export ("accessibilityCustomRotors")]
		[return: NullAllowed]
		UIAccessibilityCustomRotor [] GetAccessibilityCustomRotors ();

		[Export ("setAccessibilityCustomRotors:")]
		void SetAccessibilityCustomRotors ([NullAllowed] UIAccessibilityCustomRotor [] customRotors);
	}
	
	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface UIAccessibilityCustomRotorItemResult {

		[Export ("initWithTargetElement:targetRange:")]
		IntPtr Constructor (NSObject targetElement, [NullAllowed] UITextRange targetRange);

		[NullAllowed, Export ("targetElement", ArgumentSemantic.Weak)]
		NSObject TargetElement { get; set; }

		[NullAllowed, Export ("targetRange", ArgumentSemantic.Retain)]
		UITextRange TargetRange { get; set; }
	}
	
	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface UIAccessibilityCustomRotorSearchPredicate {

		[Export ("currentItem", ArgumentSemantic.Retain)]
		UIAccessibilityCustomRotorItemResult CurrentItem { get; set; }

		[Export ("searchDirection", ArgumentSemantic.Assign)]
		UIAccessibilityCustomRotorDirection SearchDirection { get; set; }
	}
	
	[BaseType (typeof (NSObject))]
#if XAMCORE_2_0
	// only happens on the simulator (not devices) on iOS8 (still make sense)
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Use initWithAccessibilityContainer:
#endif
	interface UIAccessibilityElement : UIAccessibilityIdentification {
		[Export ("initWithAccessibilityContainer:")]
		IntPtr Constructor (NSObject container);

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

		[iOS (10,0), TV (10,0)]
		[Export ("accessibilityFrameInContainerSpace", ArgumentSemantic.Assign)]
		CGRect AccessibilityFrameInContainerSpace { get; set; }
	}

	interface UIAccessibilityFocus {
		[Export ("accessibilityElementDidBecomeFocused")]
		void AccessibilityElementDidBecomeFocused ();

		[Export ("accessibilityElementDidLoseFocus")]
		void AccessibilityElementDidLoseFocus ();

		[Export ("accessibilityElementIsFocused")]
		bool AccessibilityElementIsFocused ();

		[iOS (9,0)]
		[Export ("accessibilityAssistiveTechnologyFocusedIdentifiers")]
		NSSet<NSString> AccessibilityAssistiveTechnologyFocusedIdentifiers { get; }
	}

	interface UIAccessibilityAction {
#if !XAMCORE_2_0
		[New] // To avoid the warning that we are overwriting the method in NSObject.   
#endif
		[Export ("accessibilityIncrement")]
		void AccessibilityIncrement ();

#if !XAMCORE_2_0
		[New] // To avoid the warning that we are overwriting the method in NSObject
#endif
		[Export ("accessibilityDecrement")]
		void AccessibilityDecrement ();

#if !XAMCORE_2_0
		[New] // To avoid the warning that we are overwriting the method in NSObject
#endif
		[Export ("accessibilityScroll:")]
		bool AccessibilityScroll (UIAccessibilityScrollDirection direction);

		[Export ("accessibilityPerformEscape")]
		bool AccessibilityPerformEscape ();

		[iOS (6,0)]
		[Export ("accessibilityPerformMagicTap")]
		bool AccessibilityPerformMagicTap ();

		[iOS (8,0)]
		[Export ("accessibilityCustomActions"), NullAllowed]
		UIAccessibilityCustomAction [] AccessibilityCustomActions { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	// NSObject category inlined in UIResponder
	interface UIAccessibilityDragging {
		[NullAllowed, Export ("accessibilityDragSourceDescriptors", ArgumentSemantic.Copy)]
		UIAccessibilityLocationDescriptor[] AccessibilityDragSourceDescriptors { get; set; }

		[NullAllowed, Export ("accessibilityDropPointDescriptors", ArgumentSemantic.Copy)]
		UIAccessibilityLocationDescriptor[] AccessibilityDropPointDescriptors { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIAccessibilityLocationDescriptor {
		[Export ("initWithName:view:")]
		IntPtr Constructor (string name, UIView view);

		[Export ("initWithName:point:inView:")]
		IntPtr Constructor (string name, CGPoint point, UIView view);

		[Export ("initWithAttributedName:point:inView:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSAttributedString attributedName, CGPoint point, UIView view);

		[NullAllowed, Export ("view", ArgumentSemantic.Weak)]
		UIView View { get; }

		[Export ("point")]
		CGPoint Point { get; }

		[Export ("name", ArgumentSemantic.Strong)]
		string Name { get; }

		[Export ("attributedName", ArgumentSemantic.Strong)]
		NSAttributedString AttributedName { get; }
	}

	[NoWatch]
	[TV (11,0), iOS (11,0)]
	[Protocol]
	interface UIAccessibilityContentSizeCategoryImageAdjusting {
		[Abstract]
		[Export ("adjustsImageSizeForAccessibilityContentSizeCategory")]
		bool AdjustsImageSizeForAccessibilityContentSizeCategory { get; set; }
	}

	[NoTV]
	[BaseType (typeof (UIView), KeepRefUntil="Dismissed", Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIActionSheetDelegate)})]
	interface UIActionSheet {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Availability (Deprecated=Platform.iOS_8_0, Message="Use 'UIAlertController' instead.")]
		[Export ("initWithTitle:delegate:cancelButtonTitle:destructiveButtonTitle:otherButtonTitles:")][Internal][PostGet ("WeakDelegate")]
		IntPtr Constructor ([NullAllowed] string title, [NullAllowed] IUIActionSheetDelegate Delegate, [NullAllowed] string cancelTitle, [NullAllowed] string destroy, [NullAllowed] string other);

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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

	interface IUIActionSheetDelegate {}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
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

	[NoTV]
	[iOS (6,0)]
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

		[iOS (7,0)]
		[Export ("activityCategory"), Static]
		UIActivityCategory Category  { get; }
	}

	[NoTV]
	[iOS (6,0)]
	[Static]
	interface UIActivityType
	{
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

		[iOS (7,0)]
		[Field ("UIActivityTypeAddToReadingList")]
		NSString AddToReadingList { get; }
		
		[iOS (7,0)]
		[Field ("UIActivityTypePostToFlickr")]
		NSString PostToFlickr { get; }
		
		[iOS (7,0)]
		[Field ("UIActivityTypePostToVimeo")]
		NSString PostToVimeo { get; }
		
		[iOS (7,0)]
		[Field ("UIActivityTypePostToTencentWeibo")]
		NSString PostToTencentWeibo { get; }
		
		[iOS (7,0)]
		[Field ("UIActivityTypeAirDrop")]
		NSString AirDrop { get; }

		[iOS (9,0)]
		[Field ("UIActivityTypeOpenInIBooks")]
		NSString OpenInIBooks { get; }

		[iOS (11,0)]
		[Field ("UIActivityTypeMarkupAsPDF")]
		NSString MarkupAsPdf { get; }
	}

	//
	// You're supposed to implement this protocol in your UIView subclasses, not provide
	// a implementation for only this protocol, which is why there is no model to subclass.
	//
	[Protocol]
	interface UIInputViewAudioFeedback {
		[Export ("enableInputClicksWhenVisible")]
		[Abstract] // it's technically optional but there's no point in adopting the protocol if you do not provide the implemenation
		bool EnableInputClicksWhenVisible { get; }
	}

	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (NSOperation))]
	[ThreadSafe]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use initWithPlaceholderItem: to instantiate an instance of UIActivityItemProvider
	interface UIActivityItemProvider : UIActivityItemSource {
		[DesignatedInitializer]
		[Export ("initWithPlaceholderItem:")]
		[PostGet ("PlaceholderItem")]
		IntPtr Constructor (NSObject placeholderItem);
		
		[Export ("placeholderItem", ArgumentSemantic.Retain)]
		NSObject PlaceholderItem { get;  }

		[Export ("activityType")]
		NSString ActivityType { get;  }

		[Export ("item")]
		NSObject Item { get; }
		
	}

	interface IUIActivityItemSource { }

	[NoTV]
	[iOS (6,0)]
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

		[iOS (7,0)]
		[Export ("activityViewController:dataTypeIdentifierForActivityType:")]
		string GetDataTypeIdentifierForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType);

		[iOS (7,0)]
		[Export ("activityViewController:subjectForActivityType:")]
		string GetSubjectForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType);
		
		[iOS (7,0)]
		[Export ("activityViewController:thumbnailImageForActivityType:suggestedSize:")]
		UIImage GetThumbnailImageForActivity (UIActivityViewController activityViewController, [NullAllowed] NSString activityType, CGSize suggestedSize);
	}

	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Use initWithActivityItems:applicationActivities: to instantiate an instance of UIActivityViewController
	interface UIActivityViewController {
		[DesignatedInitializer]
		[Export ("initWithActivityItems:applicationActivities:")]
		IntPtr Constructor (NSObject [] activityItems, [NullAllowed] UIActivity [] applicationActivities);
		
		[NullAllowed] // by default this property is null
		[Export ("completionHandler", ArgumentSemantic.Copy)]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use the 'CompletionWithItemsHandler' property instead.")]
		Action<NSString,bool> CompletionHandler { get; set;  }

		[Export ("excludedActivityTypes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSString [] ExcludedActivityTypes { get; set;  }

		[iOS (8,0)]
		[NullAllowed, Export ("completionWithItemsHandler", ArgumentSemantic.Copy)]
		UIActivityViewControllerCompletion CompletionWithItemsHandler { get; set; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	partial interface UIAlertAction : NSCopying {
		[Export ("title")]
		string Title { get; }
		
		[Export ("style")]
		UIAlertActionStyle Style { get; }
	
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	
		[Static, Export ("actionWithTitle:style:handler:")]
		UIAlertAction Create (string title, UIAlertActionStyle style, [NullAllowed] Action<UIAlertAction> handler);
	}
	
	[iOS (8,0)]
	[BaseType (typeof (UIViewController))]
	partial interface UIAlertController
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

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

		[iOS(9,0)]
		[Export ("preferredAction")]
		[NullAllowed]
		UIAlertAction PreferredAction { get; set; }
	}

	interface IUIAlertViewDelegate {}

	[NoTV]
	[BaseType (typeof (UIView), KeepRefUntil="Dismissed", Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIAlertViewDelegate)})]
	[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'UIAlertController' with a 'UIAlertControllerStyle.Alert' type instead.")]
	interface UIAlertView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);
		
		[Sealed]
		[Export ("initWithTitle:message:delegate:cancelButtonTitle:otherButtonTitles:", IsVariadic = true)][Internal][PostGet ("WeakDelegate")]
		// The native function takes a variable number of arguments (otherButtonTitles), terminated with a nil value.
		// Unfortunately iOS/ARM64 (not the general ARM64 ABI as published by ARM) has a different calling convention for varargs methods
		// than regular methods: all variable arguments are passed on the stack, no matter how many normal arguments there are.
		// Normally 8 arguments are passed in registers, then the subsequent ones are passed on the stack, so what we do is to provide
		// 9 arguments, where the 9th is nil (this is the 'mustAlsoBeNull' argument). Remember that Objective-C always has two hidden
		// arguments (id, SEL), which means we only need 7 more. And 'mustAlsoBeNull' is that 7th argument.
		// So on ARM64 the 8th argument ('mustBeNull') is ignored, and iOS sees the 9th argument ('mustAlsoBeNull') as the 8th argument.
		[Availability (Deprecated=Platform.iOS_8_0, Message="Use 'UIAlertController' instead.")]
		IntPtr Constructor ([NullAllowed] string title, [NullAllowed] string message, [NullAllowed] IUIAlertViewDelegate viewDelegate, [NullAllowed] string cancelButtonTitle, IntPtr otherButtonTitles, IntPtr mustBeNull, IntPtr mustAlsoBeNull);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIAlertViewDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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
		UIAlertViewStyle AlertViewStyle { get; set;  }

		[Export ("textFieldAtIndex:")]
		UITextField GetTextField (nint textFieldIndex);
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
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
	[BaseType (typeof (NSObject))]
	[Model]
	[DisableDefaultCtor]
	[Protocol]
	interface UIAppearance {
	}

	[iOS (9,0)]
	[BaseType (typeof (UIView))]
	interface UIStackView {
		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithArrangedSubviews:")]
		IntPtr Constructor (UIView [] views);

		[Export ("arrangedSubviews")]
		UIView[] ArrangedSubviews { get; }

		[Export ("axis")]
		UILayoutConstraintAxis Axis { get; set;  }

		[Export ("distribution")]
		UIStackViewDistribution Distribution { get; set;  }

		[Export ("alignment")]
		UIStackViewAlignment Alignment { get; set;  }

		[Export ("spacing")]
		nfloat Spacing { get; set;  }

		[Export ("baselineRelativeArrangement")]
		bool BaselineRelativeArrangement { [Bind ("isBaselineRelativeArrangement")] get; set;  }

		[Export ("layoutMarginsRelativeArrangement")]
		bool LayoutMarginsRelativeArrangement { [Bind ("isLayoutMarginsRelativeArrangement")] get; set;  }

		[Export ("addArrangedSubview:")]
		void AddArrangedSubview (UIView view);

		[Export ("removeArrangedSubview:")]
		void RemoveArrangedSubview (UIView view);

		[Export ("insertArrangedSubview:atIndex:")]
		void InsertArrangedSubview (UIView view, nuint stackIndex);

		[iOS (11,0), TV (11,0)]
		[Export ("setCustomSpacing:afterView:")]
		void SetCustomSpacing (nfloat spacing, UIView arrangedSubview);

		[iOS (11,0), TV (11,0)]
		[Export ("customSpacingAfterView:")]
		nfloat GetCustomSpacing (UIView arrangedSubview);
	}
		
	[Static]
	[iOS (6,0)]
	interface UIStateRestoration {
		[Field ("UIStateRestorationViewControllerStoryboardKey")]
		NSString ViewControllerStoryboardKey { get; }

	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIStateRestoring {
		[Export ("restorationParent")]
		IUIStateRestoring RestorationParent { get;  }

		[Export ("objectRestorationClass")]
		Class ObjectRestorationClass { get;  }

		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[Export ("applicationFinishedRestoringState")]
		void ApplicationFinishedRestoringState ();
	}

	interface IUIStateRestoring {}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[iOS (7,0)]
	interface UIObjectRestoration {
#if false
		// a bit hard to support *static* as part of an interface / extension methods
		[Static][Export ("objectWithRestorationIdentifierPath:coder:")]
		IUIStateRestoring GetStateRestorationObjectFromPath (NSString [] identifierComponents, NSCoder coder);
#endif
	}

	interface IUIViewAnimating {}

	[iOS(10,0)]
	[Protocol]
	interface UIViewAnimating
	{
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

	interface IUIViewImplicitlyAnimating {}
	[iOS(10,0)]
	[Protocol]
	interface UIViewImplicitlyAnimating : UIViewAnimating
	{
		[Export ("addAnimations:delayFactor:")]
		void AddAnimations (Action animation, nfloat delayFactor);
	
		[Export ("addAnimations:")]
		void AddAnimations (Action animation);
	
		[Export ("addCompletion:")]
		void AddCompletion (Action<UIViewAnimatingPosition> completion);
	
		[Export ("continueAnimationWithTimingParameters:durationFactor:")]
		void ContinueAnimation ([NullAllowed] IUITimingCurveProvider parameters, nfloat durationFactor);
	}
		
	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface UIViewPropertyAnimator : UIViewImplicitlyAnimating, NSCopying
	{
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

		[iOS (11,0), TV (11,0)]
		[Export ("scrubsLinearly")]
		bool ScrubsLinearly { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("pausesOnCompletion")]
		bool PausesOnCompletion { get; set; }
	
		[Export ("initWithDuration:timingParameters:")]
		[DesignatedInitializer]
		IntPtr Constructor (double duration, IUITimingCurveProvider parameters);
	
		[Export ("initWithDuration:curve:animations:")]
		IntPtr Constructor (double duration, UIViewAnimationCurve curve, [NullAllowed] Action animations);
	
		[Export ("initWithDuration:controlPoint1:controlPoint2:animations:")]
		IntPtr Constructor (double duration, CGPoint point1, CGPoint point2, Action animations);
	
		[Export ("initWithDuration:dampingRatio:animations:")]
		IntPtr Constructor (double duration, nfloat ratio, [NullAllowed] Action animations);
	
		[Static]
		[Export ("runningPropertyAnimatorWithDuration:delay:options:animations:completion:")]
		UIViewPropertyAnimator CreateRunningPropertyAnimator (double duration, double delay, UIViewAnimationOptions options, [NullAllowed] Action animations, [NullAllowed] Action<UIViewAnimatingPosition> completion);
	}
	
	interface IUIViewControllerPreviewing {}
	[Protocol]
	[iOS (9,0)]
	interface UIViewControllerPreviewing {
		[Abstract]
		[Export ("previewingGestureRecognizerForFailureRelationship")]
		UIGestureRecognizer PreviewingGestureRecognizerForFailureRelationship { get; }

		[Abstract]
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate {
			get; // readonly
#if !XAMCORE_3_0
			[NotImplemented] set;
#endif
		}

		[Wrap ("WeakDelegate")]
		IUIViewControllerPreviewingDelegate Delegate { get; }

		[Abstract]
		[Export ("sourceView")]
		UIView SourceView { get; }

		[Abstract]
		[Export ("sourceRect")]
		CGRect SourceRect { get; set; }
	}

	interface IUIViewControllerPreviewingDelegate {}
	
	[Protocol]
	[Model]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface UIViewControllerPreviewingDelegate {
		[Abstract]
		[Export ("previewingContext:viewControllerForLocation:")]
		UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location);

		[Abstract]
		[Export ("previewingContext:commitViewController:")]
		void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit);
	}
	
	[Protocol]
	[iOS (6,0)]
	interface UIViewControllerRestoration {
#if false
		/* we don't generate anything for static members in protocols now, so just keep this out */
		[Static]
		[Export ("viewControllerWithRestorationIdentifierPath:coder:")]
		UIViewController GetStateRestorationViewController (NSString [] identifierComponents, NSCoder coder);
#endif
	}

	interface UIStatusBarFrameChangeEventArgs {
		[Export ("UIApplicationStatusBarFrameUserInfoKey")]
		CGRect StatusBarFrame { get; }
	}

	interface UIStatusBarOrientationChangeEventArgs {
		[NoTV]
		[Export ("UIApplicationStatusBarOrientationUserInfoKey")]
		UIInterfaceOrientation StatusBarOrientation { get; }
	}

	interface UIApplicationLaunchEventArgs {
		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsURLKey")]
		NSUrl Url { get; }

		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsSourceApplicationKey")]
		string SourceApplication { get; }

		[NoTV]
		[NullAllowed]
		[Export ("UIApplicationLaunchOptionsRemoteNotificationKey")]
		NSDictionary RemoteNotifications { get; }

		[ProbePresence]
		[Export ("UIApplicationLaunchOptionsLocationKey")]
		bool LocationLaunch { get; }
	}

	[StrongDictionary ("UIApplicationOpenUrlOptionKeys")]
	interface UIApplicationOpenUrlOptions {
		NSObject Annotation { get; set; }
		string SourceApplication { get; set; }
		bool OpenInPlace { get; set; }

		[iOS (10, 0)]
		bool UniversalLinksOnly { get; set; }
	}

	[iOS (9,0)]
	[Static]
	[Internal] // we'll make it public if there's a need for them (beside the strong dictionary we provide)
	interface UIApplicationOpenUrlOptionKeys {
		[Field ("UIApplicationOpenURLOptionsAnnotationKey")]
		NSString AnnotationKey { get; }

		[Field ("UIApplicationOpenURLOptionsSourceApplicationKey")]
		NSString SourceApplicationKey { get; }

		[Field ("UIApplicationOpenURLOptionsOpenInPlaceKey")]
		NSString OpenInPlaceKey { get; }

		[iOS (10,0), TV (10,0)]
		[Field ("UIApplicationOpenURLOptionUniversalLinksOnly")]
		NSString UniversalLinksOnlyKey { get; }
	}

	[NoWatch]
	[BaseType (typeof (UIResponder))]
	interface UIApplication {
		[Static, ThreadSafe]
		[Export ("sharedApplication")]
		UIApplication SharedApplication { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][ThreadSafe, NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIApplicationDelegate Delegate { get; set; }

		[Export ("beginIgnoringInteractionEvents")]
		void BeginIgnoringInteractionEvents ();
		
		[Export ("endIgnoringInteractionEvents")]
		void EndIgnoringInteractionEvents ();

		[Export ("isIgnoringInteractionEvents")]
		bool IsIgnoringInteractionEvents { get; }

		[Export ("idleTimerDisabled")]
		bool IdleTimerDisabled { [Bind ("isIdleTimerDisabled")] get; set; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Please use the overload instead.")]
		[Export ("openURL:")]
		bool OpenUrl (NSUrl url);

		[iOS (10,0), TV (10,0)]
		[Export ("openURL:options:completionHandler:")]
		void OpenUrl (NSUrl url, NSDictionary options, [NullAllowed] Action<bool> completion);

		[iOS (10,0), TV (10,0)]
		[Wrap ("OpenUrl (url, options?.Dictionary, completion)")]
		[Async]
		void OpenUrl (NSUrl url, UIApplicationOpenUrlOptions options, [NullAllowed] Action<bool> completion);

		[Export ("canOpenURL:")]
		bool CanOpenUrl ([NullAllowed] NSUrl url);
		
		[Export ("sendEvent:")]
		void SendEvent (UIEvent uievent);

		[Export ("keyWindow")]
		[Transient]
		UIWindow KeyWindow { get; }

		[Export ("windows")]
		[Transient]
		UIWindow [] Windows { get; } 

		[Export ("sendAction:to:from:forEvent:")]
		bool SendAction (Selector action, [NullAllowed] NSObject target, [NullAllowed] NSObject sender, [NullAllowed] UIEvent forEvent);

		[NoTV]
		[Export ("networkActivityIndicatorVisible"), ThreadSafe]
		bool NetworkActivityIndicatorVisible { [Bind ("isNetworkActivityIndicatorVisible")] get; set; }

		[NoTV]
		[Export ("statusBarStyle")]
		UIStatusBarStyle StatusBarStyle { get; set; }
		
		[NoTV]
		[Export ("setStatusBarStyle:animated:")]
		void SetStatusBarStyle (UIStatusBarStyle statusBarStyle, bool animated);

		[NoTV]
		[Export ("statusBarHidden")]
		bool StatusBarHidden { [Bind ("isStatusBarHidden")] get; set; }

		[NoTV]
		[Export ("setStatusBarHidden:withAnimation:")]
		void SetStatusBarHidden (bool state, UIStatusBarAnimation animation);

		[NoTV]
		[Export ("setStatusBarHidden:animated:")]
		[Availability (Deprecated = Platform.iOS_3_2, Message = "Use 'SetStatusBarHidden (bool, UIStatusBarAnimation)' instead.")]
		void SetStatusBarHidden (bool hidden, bool animated);

		[NoTV]
		[Export ("statusBarOrientation")]
		UIInterfaceOrientation StatusBarOrientation { get; set; }
		
		[NoTV]
		[Export ("setStatusBarOrientation:animated:")]
		void SetStatusBarOrientation (UIInterfaceOrientation orientation, bool animated);

		[NoTV]
		[Export ("statusBarOrientationAnimationDuration")]
		double StatusBarOrientationAnimationDuration { get; }

		[NoTV]
		[Export ("statusBarFrame")]
		CGRect StatusBarFrame { get; }
		
		[TV (10, 0)]
		[Export ("applicationIconBadgeNumber")]
		nint ApplicationIconBadgeNumber { get; set; }

		[NoTV]
		[Export ("applicationSupportsShakeToEdit")]
		bool ApplicationSupportsShakeToEdit { get; set; }

		// From @interface UIApplication (UIRemoteNotifications)
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'RegisterUserNotifications', 'RegisterForNotifications'  or 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[Export ("registerForRemoteNotificationTypes:")]
		void RegisterForRemoteNotificationTypes (UIRemoteNotificationType types);

		// From @interface UIApplication (UIRemoteNotifications)
		[Export ("unregisterForRemoteNotifications")]
		void UnregisterForRemoteNotifications ();

		// From @interface UIApplication (UIRemoteNotifications)
		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message = "Use 'CurrentUserNotificationSettings' or 'UNUserNotificationCenter.GetNotificationSettings' instead.")]
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
		NSString WillChangeStatusBarOrientationNotification { get; }

		[NoTV]
		[Field ("UIApplicationDidChangeStatusBarOrientationNotification")]
		[Notification (typeof (UIStatusBarOrientationChangeEventArgs))]
		NSString DidChangeStatusBarOrientationNotification { get; }

		[NoTV]
		[Field ("UIApplicationStatusBarOrientationUserInfoKey")]
		NSString StatusBarOrientationUserInfoKey { get; }

		[NoTV]
		[Field ("UIApplicationWillChangeStatusBarFrameNotification")]
		[Notification (typeof (UIStatusBarFrameChangeEventArgs))]
		NSString WillChangeStatusBarFrameNotification { get; }

		[NoTV]
		[Field ("UIApplicationDidChangeStatusBarFrameNotification")]
		[Notification (typeof (UIStatusBarFrameChangeEventArgs))]
		NSString DidChangeStatusBarFrameNotification { get; }

		[NoTV]
		[Field ("UIApplicationStatusBarFrameUserInfoKey")]
		NSString StatusBarFrameUserInfoKey { get; }
		
		[Field ("UIApplicationLaunchOptionsURLKey")]
		NSString LaunchOptionsUrlKey { get; }
		
		[Field ("UIApplicationLaunchOptionsSourceApplicationKey")]
		NSString LaunchOptionsSourceApplicationKey { get; }

		[NoTV]
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
		[Export ("setKeepAliveTimeout:handler:")]
		bool SetKeepAliveTimeout (double timeout, [NullAllowed] Action handler);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'PushKit' instead.")]
		[Export ("clearKeepAliveTimeout")]
		void ClearKeepAliveTimeout ();
		
		[Export ("protectedDataAvailable")]
		bool ProtectedDataAvailable { [Bind ("isProtectedDataAvailable")] get; }

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[Export ("presentLocalNotificationNow:")]
#if XAMCORE_2_0
		void PresentLocalNotificationNow (UILocalNotification notification);
#else
		void PresentLocationNotificationNow (UILocalNotification notification);
#endif

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.AddNotificationRequest' instead.")]
		[Export ("scheduleLocalNotification:")]
		void ScheduleLocalNotification (UILocalNotification notification);

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RemovePendingNotificationRequests' instead.")]
		[Export ("cancelLocalNotification:")]
		void CancelLocalNotification (UILocalNotification notification);

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RemoveAllPendingNotificationRequests' instead.")]
		[Export ("cancelAllLocalNotifications")]
		void CancelAllLocalNotifications ();

		// from @interface UIApplication (UILocalNotifications)
		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.GetPendingNotificationRequests' instead.")]
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
		[Field ("UIApplicationLaunchOptionsLocalNotificationKey")]
		NSString LaunchOptionsLocalNotificationKey { get; }

		[Export ("userInterfaceLayoutDirection")]
		UIUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; }

		// from @interface UIApplication (UINewsstand)
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Export ("setNewsstandIconImage:")]
		void SetNewsstandIconImage ([NullAllowed] UIImage image);

		[NoTV]
		[Field ("UIApplicationLaunchOptionsNewsstandDownloadsKey")]
		NSString LaunchOptionsNewsstandDownloadsKey { get; }

		[iOS (7,0)]
		[Field ("UIApplicationLaunchOptionsBluetoothCentralsKey")]
		NSString LaunchOptionsBluetoothCentralsKey { get; }

		[iOS (7,0)]
		[Field ("UIApplicationLaunchOptionsBluetoothPeripheralsKey")]
		NSString LaunchOptionsBluetoothPeripheralsKey { get; }

		[NoTV]
		[iOS (9,0)]
		[Field ("UIApplicationLaunchOptionsShortcutItemKey")]
		NSString LaunchOptionsShortcutItemKey { get; }

		//
		// 6.0
		//
		// from @interface UIApplication (UIStateRestoration)
		[iOS (6,0)]
		[Export ("extendStateRestoration")]
		void ExtendStateRestoration ();

		// from @interface UIApplication (UIStateRestoration)
		[iOS (6,0)]
		[Export ("completeStateRestoration")]
		void CompleteStateRestoration ();

		[NoTV]
		[iOS (6,0)]
		[Export ("supportedInterfaceOrientationsForWindow:")]
		UIInterfaceOrientationMask SupportedInterfaceOrientationsForWindow ([Transient] UIWindow window);

		[NoWatch]
		[Field ("UITrackingRunLoopMode")]
		NSString UITrackingRunLoopMode { get; }

		[iOS (6,0)]
		[Field ("UIApplicationStateRestorationBundleVersionKey")]
		NSString StateRestorationBundleVersionKey { get; }

		[iOS (6,0)]
		[Field ("UIApplicationStateRestorationUserInterfaceIdiomKey")]
		NSString StateRestorationUserInterfaceIdiomKey { get; }

		//
		// 7.0
		//
		[iOS (7,0)]
		[Field ("UIContentSizeCategoryDidChangeNotification")]
		[Notification (typeof (UIContentSizeCategoryChangedEventArgs))]
		NSString ContentSizeCategoryChangedNotification { get; }
		
		[ThreadSafe]
		[iOS (7,0)]
		[RequiresSuper]
		[Export ("beginBackgroundTaskWithName:expirationHandler:")]
		nint BeginBackgroundTask (string taskName, Action expirationHandler);

		[TV (11,0)]
		[iOS (7,0)]
		[Field ("UIApplicationBackgroundFetchIntervalMinimum")]
		double BackgroundFetchIntervalMinimum { get; }

		[TV (11,0)]
		[iOS (7,0)]
		[Field ("UIApplicationBackgroundFetchIntervalNever")]
		double BackgroundFetchIntervalNever { get; }

		[TV (11,0)]
		[iOS (7,0)]
		[Export ("setMinimumBackgroundFetchInterval:")]
		void SetMinimumBackgroundFetchInterval (double minimumBackgroundFetchInterval);

		[iOS (7,0)]
		[Export ("preferredContentSizeCategory")]
		NSString PreferredContentSizeCategory { get; }

		// from @interface UIApplication (UIStateRestoration)
		[iOS (7,0)]
		[Export ("ignoreSnapshotOnNextApplicationLaunch")]
		void IgnoreSnapshotOnNextApplicationLaunch ();

		// from @interface UIApplication (UIStateRestoration)
		[Export ("registerObjectForStateRestoration:restorationIdentifier:")]
		[iOS (7,0)]
		[Static]
		void RegisterObjectForStateRestoration (IUIStateRestoring uistateRestoringObject, string restorationIdentifier);

		[iOS (7,0)]
		[Field ("UIApplicationStateRestorationTimestampKey")]
		NSString StateRestorationTimestampKey { get; }

		[iOS (7,0)]
		[Field ("UIApplicationStateRestorationSystemVersionKey")]
		NSString StateRestorationSystemVersionKey { get; }

		[TV (11,0)]
		[iOS (7,0)]
		[Export ("backgroundRefreshStatus")]
		UIBackgroundRefreshStatus BackgroundRefreshStatus { get; }

		[TV (11,0)]
		[iOS (7,0)]
		[Notification]
		[Field ("UIApplicationBackgroundRefreshStatusDidChangeNotification")]
		NSString BackgroundRefreshStatusDidChangeNotification { get; }

		[iOS (7,0)]
		[Notification]
		[Field ("UIApplicationUserDidTakeScreenshotNotification")]
		NSString UserDidTakeScreenshotNotification { get; }

		// 
		// 8.0
		//
		[iOS (8,0)]
		[Field ("UIApplicationOpenSettingsURLString")]
		NSString OpenSettingsUrlString { get; }

		// from @interface UIApplication (UIUserNotificationSettings)
		[NoTV]
		[iOS (8,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.GetNotificationSettings' and 'UNUserNotificationCenter.GetNotificationCategories' instead.")]
		[Export ("currentUserNotificationSettings")]
		UIUserNotificationSettings CurrentUserNotificationSettings { get; }

		// from @interface UIApplication (UIRemoteNotifications)
		[iOS (8,0)]
		[Export ("isRegisteredForRemoteNotifications")]
		bool IsRegisteredForRemoteNotifications { get; }

		// from @interface UIApplication (UIRemoteNotifications)
		[iOS (8,0)]
		[Export ("registerForRemoteNotifications")]
		void RegisterForRemoteNotifications ();

		// from @interface UIApplication (UIUserNotificationSettings)
		[NoTV]
		[iOS (8,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RequestAuthorization' and 'UNUserNotificationCenter.SetNotificationCategories' instead.")]
		[Export ("registerUserNotificationSettings:")]
		void RegisterUserNotificationSettings (UIUserNotificationSettings notificationSettings);

		[iOS (8,0)]
		[Field ("UIApplicationLaunchOptionsUserActivityDictionaryKey")]
		NSString LaunchOptionsUserActivityDictionaryKey { get; }

		[iOS (8,0)]
		[Field ("UIApplicationLaunchOptionsUserActivityTypeKey")]
		NSString LaunchOptionsUserActivityTypeKey { get; }

		[iOS (10,0), NoTV, NoWatch]
		[Field ("UIApplicationLaunchOptionsCloudKitShareMetadataKey")]
		NSString LaunchOptionsCloudKitShareMetadataKey { get; }

		[NoTV]
		[iOS (9,0)]
		[NullAllowed, Export ("shortcutItems", ArgumentSemantic.Copy)]
		UIApplicationShortcutItem[] ShortcutItems { get; set; }

		//
		// 10.0
		//
		// from @interface UIApplication (UIAlternateApplicationIcons)

		[iOS (10,3)][TV (10,2)]
		[Export ("supportsAlternateIcons")]
		bool SupportsAlternateIcons { get; }

		[iOS (10,3)][TV (10,2)]
		[Async]
		[Export ("setAlternateIconName:completionHandler:")]
		void SetAlternateIconName ([NullAllowed] string alternateIconName, [NullAllowed] Action<NSError> completionHandler);

		[iOS (10,3)][TV (10,2)]
		[Export ("alternateIconName"), NullAllowed]
		string AlternateIconName { get; }
	}

	[NoTV]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UIApplicationShortcutIcon : NSCopying
	{
		[Static]
		[Export ("iconWithType:")]
		UIApplicationShortcutIcon FromType (UIApplicationShortcutIconType type);
	
		[Static]
		[Export ("iconWithTemplateImageName:")]
		UIApplicationShortcutIcon FromTemplateImageName (string templateImageName);

#if XAMCORE_2_0
#if IOS // This is inside ContactsUI.framework
		[Static, Export ("iconWithContact:")]
		UIApplicationShortcutIcon FromContact (CNContact contact);
#endif // IOS
#endif
	}

	[NoTV]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIApplicationShortcutItem : NSMutableCopying
	{
		[Export ("initWithType:localizedTitle:localizedSubtitle:icon:userInfo:")]
		[DesignatedInitializer]
		IntPtr Constructor (string type, string localizedTitle, [NullAllowed] string localizedSubtitle, [NullAllowed] UIApplicationShortcutIcon icon, [NullAllowed] NSDictionary<NSString,NSObject> userInfo);
	
		[Export ("initWithType:localizedTitle:")]
		IntPtr Constructor (string type, string localizedTitle);
	
		[Export ("type")]
		string Type { get; [NotImplemented] set; }
	
		[Export ("localizedTitle")]
		string LocalizedTitle { get; [NotImplemented] set; }
	
		[NullAllowed, Export ("localizedSubtitle")]
		string LocalizedSubtitle { get; [NotImplemented] set; }
	
		[NullAllowed, Export ("icon", ArgumentSemantic.Copy)]
		UIApplicationShortcutIcon Icon { get; [NotImplemented] set; }
	
		[NullAllowed, Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary<NSString,NSObject> UserInfo { get; [NotImplemented] set; }
	}

	[NoTV]
	[iOS (9,0)]
	[BaseType (typeof (UIApplicationShortcutItem))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIApplicationShortcutItem init].
	interface UIMutableApplicationShortcutItem
	{
		// inlined
		[Export ("initWithType:localizedTitle:localizedSubtitle:icon:userInfo:")]
		IntPtr Constructor (string type, string localizedTitle, [NullAllowed] string localizedSubtitle, [NullAllowed] UIApplicationShortcutIcon icon, [NullAllowed] NSDictionary<NSString,NSObject> userInfo);

		// inlined
		[Export ("initWithType:localizedTitle:")]
		IntPtr Constructor (string type, string localizedTitle);

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
		NSDictionary<NSString,NSObject> UserInfo { get; set; }
	}

	[iOS (7,0)]
	[BaseType (typeof (UIDynamicBehavior))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: init is undefined for objects of type UIAttachmentBehavior
	interface UIAttachmentBehavior {
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get;  }

		[Export ("attachedBehaviorType")]
		UIAttachmentBehaviorType AttachedBehaviorType { get;  }

		[Export ("anchorPoint")]
		CGPoint AnchorPoint { get; set;  }

		[Export ("length")]
		nfloat Length { get; set;  }

		[Export ("damping")]
		nfloat Damping { get; set;  }

		[Export ("frequency")]
		nfloat Frequency { get; set;  }

		[Export ("initWithItem:attachedToAnchor:")]
		IntPtr Constructor (IUIDynamicItem item, CGPoint anchorPoint);

		[DesignatedInitializer]
		[Export ("initWithItem:offsetFromCenter:attachedToAnchor:")]
		IntPtr Constructor (IUIDynamicItem item, UIOffset offset, CGPoint anchorPoint);

		[Export ("initWithItem:attachedToItem:")]
		IntPtr Constructor (IUIDynamicItem item, IUIDynamicItem attachedToItem);

		[DesignatedInitializer]
		[Export ("initWithItem:offsetFromCenter:attachedToItem:offsetFromCenter:")]
		IntPtr Constructor (IUIDynamicItem item, UIOffset offsetFromCenter, IUIDynamicItem attachedToItem, UIOffset attachOffsetFromCenter);

		[Static]
		[iOS (9,0)]
		[Export ("slidingAttachmentWithItem:attachedToItem:attachmentAnchor:axisOfTranslation:")]
		UIAttachmentBehavior CreateSlidingAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor, CGVector translationAxis);

		[Static]
		[iOS (9,0)]
		[Export ("slidingAttachmentWithItem:attachmentAnchor:axisOfTranslation:")]
		UIAttachmentBehavior CreateSlidingAttachment (IUIDynamicItem item, CGPoint attachmentAnchor, CGVector translationAxis);

// +(instancetype __nonnull)limitAttachmentWithItem:(id<UIDynamicItem> __nonnull)item1 offsetFromCenter:(UIOffset)offset1 attachedToItem:(id<UIDynamicItem> __nonnull)item2 offsetFromCenter:(UIOffset)offset2;
		[Static]
		[iOS (9,0)]
		[Export ("limitAttachmentWithItem:offsetFromCenter:attachedToItem:offsetFromCenter:")]
		UIAttachmentBehavior CreateLimitAttachment (IUIDynamicItem item1, UIOffset offsetFromCenter, IUIDynamicItem item2, UIOffset offsetFromCenter2);

		[Static]
		[iOS (9,0)]
		[Export ("fixedAttachmentWithItem:attachedToItem:attachmentAnchor:")]
		UIAttachmentBehavior CreateFixedAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor);

		[Static]
		[iOS (9,0)]
		[Export ("pinAttachmentWithItem:attachedToItem:attachmentAnchor:")]
		UIAttachmentBehavior CreatePinAttachment (IUIDynamicItem item1, IUIDynamicItem item2, CGPoint attachmentAnchor);

		[Export ("attachmentRange")]
		[iOS (9,0)]
		UIFloatRange AttachmentRange { get; set; }

		[Export ("frictionTorque")]
		[iOS (9,0)]
		nfloat FrictionTorque { get; set; }
	}

	[iOS (10,0), TV (10,0)]
	[Protocol]
	interface UIContentSizeCategoryAdjusting {
		[Abstract]
		[iOS (10,0), TV (10,0)] // Repeated because of generator bug
		[Export ("adjustsFontForContentSizeCategory")]
		bool AdjustsFontForContentSizeCategory { get; set; }
	}

	interface UIContentSizeCategoryChangedEventArgs {
		[Export ("UIContentSizeCategoryNewValueKey")]
		NSString WeakNewValue { get; }
	}

	[iOS (7,0)]
	[Static]
	[NoWatch]
	public enum UIContentSizeCategory {
		[iOS (10,0), TV (10,0)]
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

	interface IUICoordinateSpace {}
	
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	[Abstract]
	[iOS (8,0)]
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
		[Availability (Obsoleted = Platform.iOS_9_0, Message="Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
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
		void WillChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation newStatusBarOrientation, double duration);

		[NoTV]
		[Export ("application:didChangeStatusBarOrientation:")]
		void DidChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation oldStatusBarOrientation);

		[NoTV]
		[Export ("application:willChangeStatusBarFrame:")]
		void WillChangeStatusBarFrame (UIApplication application, CGRect newStatusBarFrame);

		[NoTV]
		[Export ("application:didChangeStatusBarFrame:")]
		void ChangedStatusBarFrame (UIApplication application, CGRect oldStatusBarFrame);

		[Export ("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
		void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken);

		[Export ("application:didFailToRegisterForRemoteNotificationsWithError:")]
		void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error);

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' for user visible notifications and 'ReceivedRemoteNotification' for silent remote notifications.")]
		[Export ("application:didReceiveRemoteNotification:")]
		void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo);

		[NoTV]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.WillPresentNotification/DidReceiveNotificationResponse' instead.")]
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
		[Availability (Obsoleted = Platform.iOS_9_0, Message="Override 'OpenUrl (UIApplication, NSUrl, NSDictionary)'. The later will be called if both are implemented.")]
		[Export ("application:openURL:sourceApplication:annotation:")]
		bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation);

		[iOS (9,0)]
		[Export ("application:openURL:options:")]
		bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options);

		[iOS (9,0)]
		[Wrap ("OpenUrl(app, url, options.Dictionary)")]
		bool OpenUrl (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions options);
		
		[Export ("window", ArgumentSemantic.Retain), NullAllowed]
		UIWindow Window { get; set; }

		//
		// 6.0
		//
		[iOS (6,0)]
		[Export ("application:willFinishLaunchingWithOptions:")]
		bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions);

		[NoTV]
		[iOS (6,0)]
		[Export ("application:supportedInterfaceOrientationsForWindow:")]
		UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, [Transient] UIWindow forWindow);

		[iOS (6,0)]
		[Export ("application:viewControllerWithRestorationIdentifierPath:coder:")]
		UIViewController GetViewController (UIApplication application, string [] restorationIdentifierComponents, NSCoder coder);

		[iOS (6,0)]
		[Export ("application:shouldSaveApplicationState:")]
		bool ShouldSaveApplicationState (UIApplication application, NSCoder coder);

		[iOS (6,0)]
		[Export ("application:shouldRestoreApplicationState:")]
		bool ShouldRestoreApplicationState (UIApplication application, NSCoder coder);

		[iOS (6,0)]
		[Export ("application:willEncodeRestorableStateWithCoder:")]
		void WillEncodeRestorableState (UIApplication application, NSCoder coder);

		[iOS (6,0)]
		[Export ("application:didDecodeRestorableStateWithCoder:")]
		void DidDecodeRestorableState (UIApplication application, NSCoder coder);		

		// special case from UIAccessibilityAction. we added it (completly) on UIResponser but magic tap is also:
		// "If you’d like the Magic Tap gesture to perform the same action from anywhere within your app, it is more 
		// appropriate to implement the accessibilityPerformMagicTap method in your app delegate."
		// ref: http://developer.apple.com/library/ios/#featuredarticles/ViewControllerPGforiPhoneOS/Accessibility/AccessibilityfromtheViewControllersPerspective.html
		[NoTV]
		[iOS (6,0)]
		[Export ("accessibilityPerformMagicTap")]
		bool AccessibilityPerformMagicTap ();

		[TV (10, 0)]
		[iOS (7,0)]
		[Export ("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
		void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler);

		[iOS (7,0)]
		[Export ("application:handleEventsForBackgroundURLSession:completionHandler:")]
		void HandleEventsForBackgroundUrl (UIApplication application, string sessionIdentifier, Action completionHandler);

		[TV (11,0)]
		[iOS (7,0)]
		[Export ("application:performFetchWithCompletionHandler:")]
		void PerformFetch (UIApplication application, Action<UIBackgroundFetchResult> completionHandler);

		// 
		// 8.0
		//
		[iOS (8,0)]
		[Export ("application:continueUserActivity:restorationHandler:")]
		bool ContinueUserActivity (UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler);

		[iOS (8,0)]
		[Export ("application:didFailToContinueUserActivityWithType:error:")]
#if XAMCORE_4_0
		void DidFailToContinueUserActivity (UIApplication application, string userActivityType, NSError error);
#else
		void DidFailToContinueUserActivitiy (UIApplication application, string userActivityType, NSError error);
#endif

		[NoTV]
		[iOS (8,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenter.RequestAuthorization' instead.")]
		[Export ("application:didRegisterUserNotificationSettings:")]
		void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings);

		[NoTV]
		[iOS (8,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forLocalNotification:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, UILocalNotification localNotification, Action completionHandler);

		[NoTV]
		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forLocalNotification:withResponseInfo:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, UILocalNotification localNotification, NSDictionary responseInfo, Action completionHandler);
		
		[NoTV]
		[iOS (8,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forRemoteNotification:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, Action completionHandler);

		[NoTV]
		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNUserNotificationCenterDelegate.DidReceiveNotificationResponse' instead.")]
		[Export ("application:handleActionWithIdentifier:forRemoteNotification:withResponseInfo:completionHandler:")]
		void HandleAction (UIApplication application, string actionIdentifier, NSDictionary remoteNotificationInfo, NSDictionary responseInfo, Action completionHandler);

		[NoTV]
		[iOS (9,0)]
		[Export ("application:performActionForShortcutItem:completionHandler:")]
		void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler);
		
		[iOS (8,0)]
		[Export ("application:willContinueUserActivityWithType:")]
		bool WillContinueUserActivity (UIApplication application, string userActivityType);

		[iOS (8,0)]
		[Export ("application:didUpdateUserActivity:")]
		void UserActivityUpdated (UIApplication application, NSUserActivity userActivity);

		[iOS (8,0)]
		[Export ("application:shouldAllowExtensionPointIdentifier:")]
		bool ShouldAllowExtensionPointIdentifier (UIApplication application, NSString extensionPointIdentifier);

		[iOS (8,2)]
		[Export ("application:handleWatchKitExtensionRequest:reply:")]
		void HandleWatchKitExtensionRequest (UIApplication application, NSDictionary userInfo, Action<NSDictionary> reply);

		[iOS (9,0)]
		[Export ("applicationShouldRequestHealthAuthorization:")]
		void ShouldRequestHealthAuthorization (UIApplication application);

		[iOS (10,0), TV (10,0), NoWatch]
		[Export ("application:userDidAcceptCloudKitShareWithMetadata:")]
		void UserDidAcceptCloudKitShare (UIApplication application, CKShareMetadata cloudKitShareMetadata);

#if XAMCORE_2_0 && !TVOS
		[NoTV]
		[iOS (11,0), Watch (4,0)]
		[Export ("application:handleIntent:completionHandler:")]
		void HandleIntent (UIApplication application, INIntent intent, Action<INIntentResponse> completionHandler);
#endif // XAMCORE_2_0 && !TVOS
	}

	[Static]
	interface UIExtensionPointIdentifier {
		[iOS (8,0)]
		[Field ("UIApplicationKeyboardExtensionPointIdentifier")]
		NSString Keyboard { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIBarItem : NSCoding, UIAppearance, UIAccessibility, UIAccessibilityIdentification {
		[Export ("enabled")][Abstract]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("title", ArgumentSemantic.Copy)][Abstract]
		string Title { get;set; }

		[Export ("image", ArgumentSemantic.Retain)][Abstract]
		UIImage Image { get; set; }

		[Export ("imageInsets")][Abstract]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")][Abstract]
		nint Tag { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("landscapeImagePhone", ArgumentSemantic.Retain)]
		UIImage LandscapeImagePhone { get; set;  }

		[NoTV]
		[Export ("landscapeImagePhoneInsets")]
		UIEdgeInsets LandscapeImagePhoneInsets { get; set;  }

		[Export ("setTitleTextAttributes:forState:"), Internal]
		[Appearance]
		void _SetTitleTextAttributes ([NullAllowed] NSDictionary attributes, UIControlState state);

		[Export ("titleTextAttributesForState:"), Internal]
		[Appearance]
		NSDictionary _GetTitleTextAttributes (UIControlState state);

		[NoWatch]
		[iOS (11,0), TV (11,0)]
		[NullAllowed, Export ("largeContentSizeImage", ArgumentSemantic.Strong)]
		UIImage LargeContentSizeImage { get; set; }

		[NoWatch]
		[iOS (11,0), TV (11,0)]
		[Export ("largeContentSizeImageInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets LargeContentSizeImageInsets { get; set; }
	}
	
	[BaseType (typeof (UIBarItem))]
	[DesignatedDefaultCtor]
	interface UIBarButtonItem : NSCoding
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	 {
		[Export ("initWithImage:style:target:action:")]
		[PostGet ("Image")]
		[PostGet ("Target")]
		IntPtr Constructor ([NullAllowed] UIImage image, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);
		
		[Export ("initWithTitle:style:target:action:")]
		[PostGet ("Target")]
		IntPtr Constructor ([NullAllowed] string title, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("initWithBarButtonSystemItem:target:action:")]
		[PostGet ("Target")]
		IntPtr Constructor (UIBarButtonSystemItem systemItem, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("initWithCustomView:")]
		[PostGet ("CustomView")]
		IntPtr Constructor (UIView customView);

		[Export ("style")]
		UIBarButtonItemStyle Style { get; set; }

		[Export ("width")]
		nfloat Width { get; set; }
		
		[NullAllowed] // by default this property is null
		[Export ("possibleTitles", ArgumentSemantic.Copy)]
		NSSet PossibleTitles { get; set; }
		
		[Export ("customView", ArgumentSemantic.Retain), NullAllowed]
		UIView CustomView { get; set; }
		
		[Export ("action")][NullAllowed]
		Selector Action { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("target", ArgumentSemantic.Assign)]
		NSObject Target { get; set; }

		[Export ("enabled")][Override]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed]
		[Export ("title", ArgumentSemantic.Copy)][Override]
		string Title { get;set; }

		[NullAllowed]
		[Export ("image", ArgumentSemantic.Retain)][Override]
		UIImage Image { get; set; }

		[Export ("imageInsets")][Override]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")][Override]
		nint Tag { get; set; }

		[Export ("tintColor", ArgumentSemantic.Retain), NullAllowed]
		[Appearance]
		UIColor TintColor { get; set;  }

		[Export ("initWithImage:landscapeImagePhone:style:target:action:"), PostGet ("Image")]
#if !TVOS
		[PostGet ("LandscapeImagePhone")]
#endif
		[PostGet ("Target")]
		IntPtr Constructor ([NullAllowed] UIImage image, [NullAllowed] UIImage landscapeImagePhone, UIBarButtonItemStyle style, [NullAllowed] NSObject target, [NullAllowed] Selector action);

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
		[Export ("setBackButtonBackgroundImage:forState:barMetrics:")]
		[Appearance]
		void SetBackButtonBackgroundImage ([NullAllowed] UIImage backgroundImage, UIControlState forState, UIBarMetrics barMetrics);

		[NoTV]
		[Export ("backButtonBackgroundImageForState:barMetrics:")]
		[Appearance]
		UIImage GetBackButtonBackgroundImage (UIControlState forState, UIBarMetrics barMetrics);

		[NoTV]
		[Export ("setBackButtonTitlePositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetBackButtonTitlePositionAdjustment (UIOffset adjustment, UIBarMetrics barMetrics);

		[NoTV]
		[Export ("backButtonTitlePositionAdjustmentForBarMetrics:")]
		[Appearance]
		UIOffset GetBackButtonTitlePositionAdjustment (UIBarMetrics barMetrics);

		[NoTV]
		[Export ("setBackButtonBackgroundVerticalPositionAdjustment:forBarMetrics:")]
		[Appearance]
		void SetBackButtonBackgroundVerticalPositionAdjustment (nfloat adjustment, UIBarMetrics barMetrics);

		[NoTV]
		[Export ("backButtonBackgroundVerticalPositionAdjustmentForBarMetrics:")]
		[Appearance]
		nfloat GetBackButtonBackgroundVerticalPositionAdjustment (UIBarMetrics barMetrics);

		[iOS (6,0)]
		[Appearance]
		[Export ("setBackgroundImage:forState:style:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] 	UIImage backgroundImage, UIControlState state, UIBarButtonItemStyle style, UIBarMetrics barMetrics);

		[iOS (6,0)]
		[Appearance]
		[Export ("backgroundImageForState:style:barMetrics:")]
		UIImage GetBackgroundImage (UIControlState state, UIBarButtonItemStyle style, UIBarMetrics barMetrics);

		[iOS (9,0)]
		[NullAllowed, Export ("buttonGroup", ArgumentSemantic.Weak)]
		UIBarButtonItemGroup ButtonGroup { get; }
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UIBarButtonItemGroup : NSCoding
	{
		[DesignatedInitializer]
		[Export ("initWithBarButtonItems:representativeItem:")]
		IntPtr Constructor (UIBarButtonItem[] barButtonItems, [NullAllowed] UIBarButtonItem representativeItem);
	
		[Export ("barButtonItems", ArgumentSemantic.Copy)]
		UIBarButtonItem[] BarButtonItems { get; set; }
	
		[NullAllowed, Export ("representativeItem", ArgumentSemantic.Strong)]
		UIBarButtonItem RepresentativeItem { get; set; }
	
		[Export ("displayingRepresentativeItem")]
		bool DisplayingRepresentativeItem { [Bind ("isDisplayingRepresentativeItem")] get; }
	}
	
	[iOS (6,0)]
	[BaseType (typeof (UIView))]
	interface UICollectionReusableView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);
		
		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
		NSString ReuseIdentifier { get;  }

		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Export ("applyLayoutAttributes:")]
		void ApplyLayoutAttributes ([NullAllowed] UICollectionViewLayoutAttributes layoutAttributes);

		[Export ("willTransitionFromLayout:toLayout:")]
		void WillTransition (UICollectionViewLayout oldLayout, UICollectionViewLayout newLayout);

		[Export ("didTransitionFromLayout:toLayout:")]
		void DidTransition (UICollectionViewLayout oldLayout, UICollectionViewLayout newLayout);

		[iOS (8,0)]
		[Export ("preferredLayoutAttributesFittingAttributes:")]
		UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes (UICollectionViewLayoutAttributes layoutAttributes);
	}

	[iOS (6,0)]
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
		IntPtr Constructor (CGRect frame, UICollectionViewLayout layout);

		[Export ("collectionViewLayout", ArgumentSemantic.Retain)]
		UICollectionViewLayout CollectionViewLayout { get; set;  }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UICollectionViewDelegate Delegate { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDataSource { get; set;  }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		UICollectionViewDataSource DataSource { get; set; }

		[Export ("backgroundView", ArgumentSemantic.Retain)]
		UIView BackgroundView { get; [NullAllowed] set; }

		[Export ("allowsSelection")]
		bool AllowsSelection { get; set;  }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set;  }

		[Export ("registerClass:forCellWithReuseIdentifier:"), Internal]
		void RegisterClassForCell (IntPtr /* Class */cellClass, NSString reuseIdentifier);

		[Export ("registerNib:forCellWithReuseIdentifier:")]
		void RegisterNibForCell (UINib nib, NSString reuseIdentifier);

		[Export ("registerClass:forSupplementaryViewOfKind:withReuseIdentifier:"), Protected]
		void RegisterClassForSupplementaryView (IntPtr /*Class*/ viewClass, NSString kind, NSString reuseIdentifier);

		[Export ("registerNib:forSupplementaryViewOfKind:withReuseIdentifier:")]
		void RegisterNibForSupplementaryView (UINib nib, NSString kind, NSString reuseIdentifier);

		[Export ("dequeueReusableCellWithReuseIdentifier:forIndexPath:")]
#if XAMCORE_2_0
		UICollectionReusableView
#else
		NSObject
#endif
		DequeueReusableCell (NSString reuseIdentifier, NSIndexPath indexPath);

		[Export ("dequeueReusableSupplementaryViewOfKind:withReuseIdentifier:forIndexPath:")]
#if XAMCORE_2_0
		UICollectionReusableView
#else
		NSObject
#endif
		DequeueReusableSupplementaryView (NSString kind, NSString identifier, NSIndexPath indexPath);

		[Export ("indexPathsForSelectedItems")]
		NSIndexPath [] GetIndexPathsForSelectedItems ();

		[Export ("selectItemAtIndexPath:animated:scrollPosition:")]
		void SelectItem (NSIndexPath indexPath, bool animated, UICollectionViewScrollPosition scrollPosition);

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
		UICollectionViewLayoutAttributes GetLayoutAttributesForItem (NSIndexPath indexPath);

		[Export ("layoutAttributesForSupplementaryElementOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForSupplementaryElement (NSString elementKind, NSIndexPath indexPath);

		[Export ("indexPathForItemAtPoint:")]
		NSIndexPath IndexPathForItemAtPoint (CGPoint point);

		[Export ("indexPathForCell:")]
		NSIndexPath IndexPathForCell (UICollectionViewCell cell);

		[Export ("cellForItemAtIndexPath:")]
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

		[Export ("moveItemAtIndexPath:toIndexPath:")]
		void MoveItem (NSIndexPath indexPath, NSIndexPath newIndexPath);

		[Export ("performBatchUpdates:completion:")]
		[Async]
		void PerformBatchUpdates (Action updates, [NullAllowed] UICompletionHandler completed);

		//
		// 7.0
		//
		[iOS (7,0)]
		[Export ("startInteractiveTransitionToCollectionViewLayout:completion:")]
		[Async (ResultTypeName="UICollectionViewTransitionResult")]
		UICollectionViewTransitionLayout StartInteractiveTransition (UICollectionViewLayout newCollectionViewLayout,
									     UICollectionViewLayoutInteractiveTransitionCompletion completion);

		[iOS (7,0)]
		[Export ("setCollectionViewLayout:animated:completion:")]
		[Async]
		void SetCollectionViewLayout (UICollectionViewLayout layout, bool animated, UICompletionHandler completion);
		
		[iOS (7,0)]
		[Export ("finishInteractiveTransition")]
		void FinishInteractiveTransition ();

		[iOS (7,0)]
		[Export ("cancelInteractiveTransition")]
		void CancelInteractiveTransition ();

		[iOS (9,0)]
		[Export ("beginInteractiveMovementForItemAtIndexPath:")]
		bool BeginInteractiveMovementForItem (NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("updateInteractiveMovementTargetPosition:")]
		void UpdateInteractiveMovement (CGPoint targetPosition);

		[iOS (9,0)]
		[Export ("endInteractiveMovement")]
		void EndInteractiveMovement ();

		[iOS (9,0)]
		[Export ("cancelInteractiveMovement")]
		void CancelInteractiveMovement ();


		[iOS (9,0)]
		[return : NullAllowed]
		[Export ("supplementaryViewForElementKind:atIndexPath:")]
		UICollectionReusableView GetSupplementaryView (NSString elementKind, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("visibleSupplementaryViewsOfKind:")]
		UICollectionReusableView[] GetVisibleSupplementaryViews (NSString elementKind);

		[iOS (9,0)]
		[Export ("indexPathsForVisibleSupplementaryElementsOfKind:")]
		NSIndexPath[] GetIndexPathsForVisibleSupplementaryElements (NSString elementKind);

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("remembersLastFocusedIndexPath")]
		bool RemembersLastFocusedIndexPath { get; set; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("prefetchDataSource", ArgumentSemantic.Weak)]
		IUICollectionViewDataSourcePrefetching PrefetchDataSource { get; set; }

		[iOS (10,0), TV (10,0)]
		[Export ("prefetchingEnabled")]
		bool PrefetchingEnabled { [Bind ("isPrefetchingEnabled")] get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("dragDelegate", ArgumentSemantic.Weak)]
		IUICollectionViewDragDelegate DragDelegate { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("dropDelegate", ArgumentSemantic.Weak)]
		IUICollectionViewDropDelegate DropDelegate { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("dragInteractionEnabled")]
		bool DragInteractionEnabled { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("reorderingCadence", ArgumentSemantic.Assign)]
		UICollectionViewReorderingCadence ReorderingCadence { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Export ("hasUncommittedUpdates")]
		bool HasUncommittedUpdates { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("hasActiveDrag")]
		bool HasActiveDrag { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("hasActiveDrop")]
		bool HasActiveDrop { get; }
	}

	interface IUICollectionViewDataSourcePrefetching {}
	
	[Protocol]
	[iOS (10, 0)]
	interface UICollectionViewDataSourcePrefetching {
		
		[Abstract]
		[Export ("collectionView:prefetchItemsAtIndexPaths:")]
		void PrefetchItems (UICollectionView collectionView, NSIndexPath[] indexPaths);
	
		[Export ("collectionView:cancelPrefetchingForItemsAtIndexPaths:")]
		void CancelPrefetching (UICollectionView collectionView, NSIndexPath[] indexPaths);
	}
		
	//
	// Combined version of UICollectionViewDataSource, UICollectionViewDelegate
	//
	[iOS (6,0)]
	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol (IsInformal = true)]
	interface UICollectionViewSource : UICollectionViewDataSource, UICollectionViewDelegate {
		
	}
	
	[iOS (6,0)]
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

		[iOS (9,0)]
		[Export ("collectionView:canMoveItemAtIndexPath:")]
		bool CanMoveItem (UICollectionView collectionView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("collectionView:moveItemAtIndexPath:toIndexPath:")]
		void MoveItem (UICollectionView collectionView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);

		[iOS (10,3), TV (10,2)]
		[return: NullAllowed]
		[Export ("indexTitlesForCollectionView:")]
		string [] GetIndexTitles (UICollectionView collectionView);

		[iOS (10,3), TV (10,2)]
		[return: NullAllowed]
		[Export ("collectionView:indexPathForIndexTitle:atIndex:")]
		NSIndexPath GetIndexPath (UICollectionView collectionView, string title, nint atIndex);
	}

	[iOS (6,0)]
	[Model]
	[Protocol]
#if XAMCORE_3_0
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

		[iOS (8,0)]
		[Export ("collectionView:willDisplayCell:forItemAtIndexPath:")]
		void WillDisplayCell (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath);

		[iOS (8,0)]
		[Export ("collectionView:willDisplaySupplementaryView:forElementKind:atIndexPath:")]
		void WillDisplaySupplementaryView (UICollectionView collectionView, UICollectionReusableView view, string elementKind, NSIndexPath indexPath);

		[Export ("collectionView:didEndDisplayingCell:forItemAtIndexPath:")]
		void CellDisplayingEnded (UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath);

		[Export ("collectionView:didEndDisplayingSupplementaryView:forElementOfKind:atIndexPath:")]
		void SupplementaryViewDisplayingEnded (UICollectionView collectionView, UICollectionReusableView view, NSString elementKind, NSIndexPath indexPath);

		[Export ("collectionView:shouldShowMenuForItemAtIndexPath:")]
		bool ShouldShowMenu (UICollectionView collectionView, NSIndexPath indexPath);

		[Export ("collectionView:canPerformAction:forItemAtIndexPath:withSender:")]
		bool CanPerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Export ("collectionView:performAction:forItemAtIndexPath:withSender:")]
		void PerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender);

		[iOS (7,0)]
		[Export ("collectionView:transitionLayoutForOldLayout:newLayout:")]
		UICollectionViewTransitionLayout TransitionLayout (UICollectionView collectionView, UICollectionViewLayout fromLayout, UICollectionViewLayout toLayout);
		[iOS (9,0)]
		[Export ("collectionView:targetIndexPathForMoveFromItemAtIndexPath:toProposedIndexPath:")]
		NSIndexPath GetTargetIndexPathForMove (UICollectionView collectionView, NSIndexPath originalIndexPath, NSIndexPath proposedIndexPath);

		[iOS (9,0)]
		[Export ("collectionView:targetContentOffsetForProposedContentOffset:")]
		CGPoint GetTargetContentOffset (UICollectionView collectionView, CGPoint proposedContentOffset);

		[iOS (9,0)]
		[Export ("collectionView:canFocusItemAtIndexPath:")]
		bool CanFocusItem (UICollectionView collectionView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("collectionView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UICollectionView collectionView, UICollectionViewFocusUpdateContext context);

		[iOS (9,0)]
		[Export ("collectionView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UICollectionView collectionView, UICollectionViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[iOS (9,0)]
		[Export ("indexPathForPreferredFocusedViewInCollectionView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UICollectionView collectionView);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("collectionView:shouldSpringLoadItemAtIndexPath:withContext:")]
		bool ShouldSpringLoadItem (UICollectionView collectionView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);
	}

	[iOS (6,0)]
	[BaseType (typeof (UICollectionReusableView))]
	interface UICollectionViewCell {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("contentView")]
		UIView ContentView { get;  }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set;  }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("backgroundView", ArgumentSemantic.Retain)]
		UIView BackgroundView { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("selectedBackgroundView", ArgumentSemantic.Retain)]
		UIView SelectedBackgroundView { get; set;  }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("dragStateDidChange:")]
		void DragStateDidChange (UICollectionViewCellDragState dragState);
	}

	[iOS (6,0)]
	[BaseType (typeof (UIViewController))]
	interface UICollectionViewController : UICollectionViewSource, NSCoding {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("collectionView", ArgumentSemantic.Retain)]
		UICollectionView CollectionView { get; set;  }

		[Export ("clearsSelectionOnViewWillAppear")]
		bool ClearsSelectionOnViewWillAppear { get; set;  }

		// The PostSnippet is there to ensure that "layout" is alive
		// note: we can't use [PostGet] since it would not work before iOS7 so the hack must remain...
		[DesignatedInitializer]
		[Export ("initWithCollectionViewLayout:")]
		IntPtr Constructor (UICollectionViewLayout layout);

		[iOS (7,0)]
		[Export ("collectionViewLayout")]
		UICollectionViewLayout Layout { get; }

		[iOS (7,0)]
		[Export ("useLayoutToLayoutNavigationTransitions", ArgumentSemantic.Assign)]
		bool UseLayoutToLayoutNavigationTransitions { get; set; }

		[iOS (9,0)]
		[Export ("installsStandardGestureForInteractiveMovement")]
		bool InstallsStandardGestureForInteractiveMovement { get; set; }		
	}
	
	[iOS (6,0)]
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

	[iOS (6,0)]
	[BaseType (typeof (UICollectionViewLayout))]
	interface UICollectionViewFlowLayout {
		[Export ("minimumLineSpacing")]
		nfloat MinimumLineSpacing { get; set;  }

		[Export ("minimumInteritemSpacing")]
		nfloat MinimumInteritemSpacing { get; set;  }

		[Export ("itemSize")]
		CGSize ItemSize { get; set;  }

		// Default value of this property is CGSize.Zero, setting to any other value causes each cell to be queried
		[iOS (8,0)] 
		[Export ("estimatedItemSize")]
		CGSize EstimatedItemSize { get; set; }

		[Export ("scrollDirection")]
		UICollectionViewScrollDirection ScrollDirection { get; set;  }

		[Export ("headerReferenceSize")]
		CGSize HeaderReferenceSize { get; set;  }

		[Export ("footerReferenceSize")]
		CGSize FooterReferenceSize { get; set;  }

		[Export ("sectionInset")]
		UIEdgeInsets SectionInset { get; set;  }

		[NoWatch]
		[iOS (11,0), TV (11,0)]
		[Export ("sectionInsetReference", ArgumentSemantic.Assign)]
		UICollectionViewFlowLayoutSectionInsetReference SectionInsetReference { get; set; }

		[iOS (9,0)]
		[Export ("sectionHeadersPinToVisibleBounds")]
		bool SectionHeadersPinToVisibleBounds { get; set; }

		[iOS (9,0)]
		[Export ("sectionFootersPinToVisibleBounds")]
		bool SectionFootersPinToVisibleBounds { get; set; }

		[iOS (10,0), TV (10,0)]
		[Field ("UICollectionViewFlowLayoutAutomaticSize")]
		CGSize AutomaticSize { get; }
	}

	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UICollectionViewLayout : NSCoding {
		[Export ("collectionView")]
		UICollectionView CollectionView { get;  }

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

		[iOS (8,0)]
		[Export ("invalidationContextForPreferredLayoutAttributes:withOriginalAttributes:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContext (UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes);

		[iOS (8,0)]
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

		[iOS (7,0)]
		[Static, Export ("invalidationContextClass")]
		Class InvalidationContextClass ();

		[iOS (7,0)]
		[Export ("invalidationContextForBoundsChange:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForBoundsChange (CGRect newBounds);

		[iOS (7,0)]
		[Export ("indexPathsToDeleteForSupplementaryViewOfKind:")]
		NSIndexPath [] GetIndexPathsToDeleteForSupplementaryView (NSString kind);

		[iOS (7,0)]
		[Export ("indexPathsToDeleteForDecorationViewOfKind:")]
		NSIndexPath [] GetIndexPathsToDeleteForDecorationViewOfKind (NSString kind);

		[iOS (7,0)]
		[Export ("indexPathsToInsertForSupplementaryViewOfKind:")]
		NSIndexPath [] GetIndexPathsToInsertForSupplementaryView (NSString kind);

		[iOS (7,0)]
		[Export ("indexPathsToInsertForDecorationViewOfKind:")]
		NSIndexPath [] GetIndexPathsToInsertForDecorationView (NSString kind);

		[iOS (7,0)]
		[Export ("invalidateLayoutWithContext:")]
		void InvalidateLayout (UICollectionViewLayoutInvalidationContext context);

		[iOS (7,0)]
		[Export ("finalizeLayoutTransition")]
		void FinalizeLayoutTransition ();

		[iOS (7,0)]
		[Export ("prepareForTransitionFromLayout:")]
		void PrepareForTransitionFromLayout (UICollectionViewLayout oldLayout);

		[iOS (7,0)]
		[Export ("prepareForTransitionToLayout:")]
		void PrepareForTransitionToLayout (UICollectionViewLayout newLayout);

		[iOS (7,0)]
		[Export ("targetContentOffsetForProposedContentOffset:")]
		CGPoint TargetContentOffsetForProposedContentOffset (CGPoint proposedContentOffset);

		[iOS (9,0)]
		[Export ("targetIndexPathForInteractivelyMovingItem:withPosition:")]
		NSIndexPath GetTargetIndexPathForInteractivelyMovingItem (NSIndexPath previousIndexPath, CGPoint position);

		[iOS (9,0)]
		[Export ("layoutAttributesForInteractivelyMovingItemAtIndexPath:withTargetPosition:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForInteractivelyMovingItem (NSIndexPath indexPath, CGPoint targetPosition);

		[iOS (9,0)]
		[Export ("invalidationContextForInteractivelyMovingItems:withTargetPosition:previousIndexPaths:previousPosition:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForInteractivelyMovingItems (NSIndexPath[] targetIndexPaths, CGPoint targetPosition, NSIndexPath[] previousIndexPaths, CGPoint previousPosition);

		[iOS (9,0)]
		[Export ("invalidationContextForEndingInteractiveMovementOfItemsToFinalIndexPaths:previousIndexPaths:movementCancelled:")]
		UICollectionViewLayoutInvalidationContext GetInvalidationContextForEndingInteractiveMovementOfItems (NSIndexPath[] finalIndexPaths, NSIndexPath[] previousIndexPaths, bool movementCancelled);
		
		[NoWatch]
		[iOS (11,0), TV (11,0)]
		[Export ("developmentLayoutDirection")]
		UIUserInterfaceLayoutDirection DevelopmentLayoutDirection { get; }

		[NoWatch]
		[iOS (11,0), TV (11,0)]
		[Export ("flipsHorizontallyInOppositeLayoutDirection")]
		bool FlipsHorizontallyInOppositeLayoutDirection { get; }
	}
	
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewLayoutAttributes : UIDynamicItem, NSCopying {
		[Export ("frame")]
		CGRect Frame { get; set;  }

		[Export ("center")]
		new CGPoint Center { get; set;  }

		[Export ("size")]
		CGSize Size { get; set;  }

		[Export ("transform3D")]
		CATransform3D Transform3D { get; set;  }

		[Export ("alpha")]
		nfloat Alpha { get; set;  }

		[Export ("zIndex")]
		nint ZIndex { get; set;  }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set;  }

		[Export ("indexPath", ArgumentSemantic.Retain)]
		NSIndexPath IndexPath { get; set;  }

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

		[iOS (7,0)]
		[Export ("bounds")]
		new CGRect Bounds { get; set; }

		[iOS (7,0)]
		[Export ("transform")]
		new CGAffineTransform Transform { get; set; }
		
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewLayoutInvalidationContext {
		[Export ("invalidateDataSourceCounts")]
		bool InvalidateDataSourceCounts { get; }

		[Export ("invalidateEverything")]
		bool InvalidateEverything { get; }

		[iOS (8,0)]
		[Export ("invalidatedItemIndexPaths")]
		NSIndexPath [] InvalidatedItemIndexPaths { get; }

		[iOS (8,0)]
		[Export ("invalidatedSupplementaryIndexPaths")]
		NSDictionary InvalidatedSupplementaryIndexPaths { get; }

		[iOS (8,0)]
		[Export ("invalidatedDecorationIndexPaths")]
		NSDictionary InvalidatedDecorationIndexPaths { get; }

		[iOS (8,0)]
		[Export ("contentOffsetAdjustment")]
		CGPoint ContentOffsetAdjustment { get; set; }

		[iOS (8,0)]
		[Export ("contentSizeAdjustment")]
		CGSize ContentSizeAdjustment { get; set; }

		[iOS (8,0)]
		[Export ("invalidateItemsAtIndexPaths:")]
		void InvalidateItems (NSIndexPath [] indexPaths);

		[iOS (8,0)]
		[Export ("invalidateSupplementaryElementsOfKind:atIndexPaths:")]
		void InvalidateSupplementaryElements (NSString elementKind, NSIndexPath [] indexPaths);

		[iOS (8,0)]
		[Export ("invalidateDecorationElementsOfKind:atIndexPaths:")]
		void InvalidateDecorationElements (NSString elementKind, NSIndexPath [] indexPaths);

		[iOS (9,0)]
		[NullAllowed, Export ("previousIndexPathsForInteractivelyMovingItems")]
		NSIndexPath[] PreviousIndexPathsForInteractivelyMovingItems { get; }

		[iOS (9,0)]
		[NullAllowed, Export ("targetIndexPathsForInteractivelyMovingItems")]
		NSIndexPath[] TargetIndexPathsForInteractivelyMovingItems { get; }

		[iOS (9,0)]
		[Export ("interactiveMovementTarget")]
		CGPoint InteractiveMovementTarget { get; }
	}
	
	[iOS (7,0)]
	[BaseType (typeof (UICollectionViewLayoutInvalidationContext))]
	partial interface UICollectionViewFlowLayoutInvalidationContext {
		[Export ("invalidateFlowLayoutDelegateMetrics")]
		bool InvalidateFlowLayoutDelegateMetrics { get; set; }
	
		[Export ("invalidateFlowLayoutAttributes")]
		bool InvalidateFlowLayoutAttributes { get; set; }
	}
	
	[iOS (7,0)]
	[BaseType (typeof (UICollectionViewLayout))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: -[UICollectionViewTransitionLayout init] is not a valid initializer - use -initWithCurrentLayout:nextLayout: instead
	interface UICollectionViewTransitionLayout : NSCoding {
		[Export ("currentLayout")]
		UICollectionViewLayout CurrentLayout { get;  }

		[Export ("nextLayout")]
		UICollectionViewLayout NextLayout { get;  }

		[DesignatedInitializer]
		[Export ("initWithCurrentLayout:nextLayout:")]
		[PostGet ("CurrentLayout")]
		[PostGet ("NextLayout")]
		IntPtr Constructor (UICollectionViewLayout currentLayout, UICollectionViewLayout newLayout);

		[Export ("updateValue:forAnimatedKey:")]
		void UpdateValue (nfloat value, string animatedKey);

		[Export ("valueForAnimatedKey:")]
		nfloat GetValueForAnimatedKey (string animatedKey);

		[Export ("transitionProgress", ArgumentSemantic.Assign)]
		nfloat TransitionProgress { get; set; }
	}
	
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface UICollectionViewUpdateItem {
		[NullAllowed]
		[Export ("indexPathBeforeUpdate")]
		NSIndexPath IndexPathBeforeUpdate { get;  }

		[NullAllowed]
		[Export ("indexPathAfterUpdate")]
		NSIndexPath IndexPathAfterUpdate { get;  }

		[Export ("updateAction")]
		UICollectionUpdateAction UpdateAction { get;  }
	}

	[iOS (6,0)]
	[Static]
	interface UICollectionElementKindSectionKey
	{
		[Field ("UICollectionElementKindSectionHeader")]
		NSString Header { get; }

		[Field ("UICollectionElementKindSectionFooter")]
		NSString Footer { get; }
	}
#endif // !WATCH
	
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
		[Export ("colorWithWhite:alpha:")][Static]
		UIColor FromWhiteAlpha (nfloat white, nfloat alpha);

		[Export ("colorWithHue:saturation:brightness:alpha:")][Static]
		UIColor FromHSBA (nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);
		
		[Export ("colorWithRed:green:blue:alpha:")][Static]
		UIColor FromRGBA (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("colorWithCGColor:")][Static]
		UIColor FromCGColor (CGColor color);

		[iOS (11,0), TV (11,0)]
		[Static]
		[Export ("colorNamed:")]
		[return: NullAllowed]
		UIColor FromName (string name);

#if !WATCH
		[iOS (11,0), TV (11,0)]
		[Static]
		[Export ("colorNamed:inBundle:compatibleWithTraitCollection:")]
		[return: NullAllowed]
		UIColor FromName (string name, [NullAllowed] NSBundle inBundle, [NullAllowed] UITraitCollection compatibleWithTraitCollection);
#endif
	
		[iOS (10,0), TV (10,0), Watch (3,0)]
		[Static]
		[Export ("colorWithDisplayP3Red:green:blue:alpha:")]
		UIColor FromDisplayP3 (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("colorWithPatternImage:")][Static]
		UIColor FromPatternImage (UIImage image);

		[Export ("initWithRed:green:blue:alpha:")]
		IntPtr Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Export ("initWithPatternImage:")]
		IntPtr Constructor (UIImage patternImage);

		[Export ("initWithWhite:alpha:")]
		IntPtr Constructor (nfloat white, nfloat alpha);

		// [Export ("initWithHue:saturation:brightness:alpha:")]
		// IntPtr Constructor (nfloat red, nfloat green, nfloat blue, nfloat alpha);
		// 
		// This method is not bound as a constructor because the binding already has a constructor that
		// takes 4 doubles (RGBA constructor) meaning that we would need to use an enum to diff between them making the API
		// uglier when it is not needed. The developer can use colorWithHue:saturation:brightness:alpha:
		// instead.
		
		[Export ("initWithCGColor:")]
		IntPtr Constructor (CGColor color);

		[Static] [Export ("clearColor")]
		UIColor Clear { get; }

		[Static] [Export ("blackColor")]
		UIColor Black { get; }

		[Static] [Export ("darkGrayColor")]
		UIColor DarkGray { get; }

		[Static] [Export ("lightGrayColor")]
		UIColor LightGray { get; }

		[Static] [Export ("whiteColor")]
		UIColor White { get; }

		[Static] [Export ("grayColor")]
		UIColor Gray { get; }

		[Static] [Export ("redColor")]
		UIColor Red { get; }

		[Static] [Export ("greenColor")]
		UIColor Green { get; }

		[Static] [Export ("blueColor")]
		UIColor Blue { get; }

		[Static] [Export ("cyanColor")]
		UIColor Cyan { get; }

		[Static] [Export ("yellowColor")]
		UIColor Yellow { get; }

		[Static] [Export ("magentaColor")]
		UIColor Magenta { get; }

		[Static] [Export ("orangeColor")]
		UIColor Orange { get; }

		[Static] [Export ("purpleColor")]
		UIColor Purple { get; }

		[Static] [Export ("brownColor")]
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
		[Export ("CIColor")]
		CIColor CIColor { get; }

		[NoWatch][NoTV]
		[Export ("lightTextColor")]
		[Static]
		UIColor LightTextColor { get; }

		[NoWatch][NoTV]
		[Export ("darkTextColor")]
		[Static]
		UIColor DarkTextColor { get; }

		[NoWatch][NoTV]
		[Export ("groupTableViewBackgroundColor")][Static]
		UIColor GroupTableViewBackgroundColor { get; }

		[Availability (Deprecated = Platform.iOS_7_0)]
		[NoWatch][NoTV]
		[Export ("viewFlipsideBackgroundColor")][Static]
		UIColor ViewFlipsideBackgroundColor { get; }

		[Availability (Deprecated = Platform.iOS_7_0)]
		[NoWatch][NoTV]
		[Export ("scrollViewTexturedBackgroundColor")][Static]
		UIColor ScrollViewTexturedBackgroundColor { get; }

		[Availability (Deprecated = Platform.iOS_7_0)]
		[NoWatch][NoTV]
		[Static, Export ("underPageBackgroundColor")]
		UIColor UnderPageBackgroundColor { get; }

		[NoWatch]
		[Static, Export ("colorWithCIColor:")]
		UIColor FromCIColor (CIColor color);

		[NoWatch]
		[Export ("initWithCIColor:")]
		IntPtr Constructor (CIColor ciColor);

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
		[iOS (11,0), NoWatch, NoTV]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string[] ReadableTypeIdentifiers { get; }

		// From the NSItemProviderReading protocol, a static method.
		[iOS (11,0), NoWatch, NoTV]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
#if !WATCH && !TVOS
		new
#endif
		UIColor GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol, a static method.
		// NSItemProviderWriting doesn't seem to be implemented for tvOS/watchOS, even though the headers say otherwise.
		[NoWatch, NoTV, iOS (11,0)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string[] WritableTypeIdentifiers { get; }
	}

#if !WATCH
	[iOS (7,0)]
	[BaseType (typeof (UIDynamicBehavior),
		   Delegates=new string [] { "CollisionDelegate" },
		   Events=new Type [] { typeof (UICollisionBehaviorDelegate)})]
	interface UICollisionBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		IntPtr Constructor ([Params] IUIDynamicItem [] items);
		
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get;  }

		[Export ("collisionMode")]
		UICollisionBehaviorMode CollisionMode { get; set;  }

		[Export ("translatesReferenceBoundsIntoBoundary")]
		bool TranslatesReferenceBoundsIntoBoundary { get; set;  }

		[Export ("boundaryIdentifiers", ArgumentSemantic.Copy)]
		NSObject [] BoundaryIdentifiers { get;  }

		[Export ("collisionDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakCollisionDelegate { get; set;  }

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
	
	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UICollisionBehaviorDelegate {
		[Export ("collisionBehavior:beganContactForItem:withItem:atPoint:")][EventArgs ("UICollisionBeganContact")]
		void BeganContact (UICollisionBehavior behavior, IUIDynamicItem firstItem, IUIDynamicItem secondItem, CGPoint atPoint);

		[Export ("collisionBehavior:endedContactForItem:withItem:")][EventArgs ("UICollisionEndedContact")]
		void EndedContact (UICollisionBehavior behavior, IUIDynamicItem firstItem, IUIDynamicItem secondItem);

		[Export ("collisionBehavior:beganContactForItem:withBoundaryIdentifier:atPoint:")][EventArgs ("UICollisionBeganBoundaryContact")]
		void BeganBoundaryContact (UICollisionBehavior behavior, IUIDynamicItem dynamicItem, [NullAllowed] NSObject boundaryIdentifier, CGPoint atPoint);

		[Export ("collisionBehavior:endedContactForItem:withBoundaryIdentifier:")][EventArgs ("UICollisionEndedBoundaryContact")]
		void EndedBoundaryContact (UICollisionBehavior behavior, IUIDynamicItem dynamicItem, [NullAllowed] NSObject boundaryIdentifier);
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: do not call -[UIDocument init] - the designated initializer is -[UIDocument initWithFileURL:
	[DisableDefaultCtor]
	[ThreadSafe]
	interface UIDocument : NSFilePresenter, NSProgressReporting, UIUserActivityRestoring {
		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get;  }

		[Export ("fileType", ArgumentSemantic.Copy)]
		string FileType { get;  }

		[Export ("fileModificationDate", ArgumentSemantic.Copy)]
		NSDate FileModificationDate { get; set;  }

		[Export ("documentState")]
		UIDocumentState DocumentState { get;  }

		[DesignatedInitializer]
		[Export ("initWithFileURL:")]
		[PostGet ("FileUrl")]
		IntPtr Constructor (NSUrl url);

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
		[iOS (8,0)]
		[Export ("userActivity", ArgumentSemantic.Retain)]
		NSUserActivity UserActivity { get; set; }

		[iOS (8,0)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity userActivity);

		[iOS (8,0)]
		[Field ("NSUserActivityDocumentURLKey")]
		NSString UserActivityDocumentUrlKey { get; }

	}

	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UIDynamicAnimatorDelegate {
#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("dynamicAnimatorWillResume:")]
		void WillResume (UIDynamicAnimator animator);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("dynamicAnimatorDidPause:")]
		void DidPause (UIDynamicAnimator animator);
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface UIDynamicAnimator {
		[DesignatedInitializer]
		[Export ("initWithReferenceView:")]
		IntPtr Constructor (UIView referenceView);
		
		[Export ("referenceView")]
		UIView ReferenceView { get;  }

		[Export ("behaviors", ArgumentSemantic.Copy)]
		UIDynamicBehavior [] Behaviors { get;  }

		[Export ("running")]
		bool Running { [Bind ("isRunning")] get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDynamicAnimatorDelegate Delegate { get; set;  }

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
		IntPtr Constructor (UICollectionViewLayout layout);
		
		[Export ("layoutAttributesForCellAtIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForCell (NSIndexPath cellIndexPath);

		[Export ("layoutAttributesForSupplementaryViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForSupplementaryView (NSString viewKind, NSIndexPath viewIndexPath);

		[Export ("layoutAttributesForDecorationViewOfKind:atIndexPath:")]
		UICollectionViewLayoutAttributes GetLayoutAttributesForDecorationView (NSString viewKind, NSIndexPath viewIndexPath);

	}

	[iOS (7,0)]
	[BaseType (typeof (UIDynamicBehavior))]
	interface UIDynamicItemBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		IntPtr Constructor ([Params] IUIDynamicItem [] items);
		
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get;  }

		[Export ("elasticity")]
		nfloat Elasticity { get; set;  }

		[Export ("friction")]
		nfloat Friction { get; set;  }

		[Export ("density")]
		nfloat Density { get; set;  }

		[Export ("resistance")]
		nfloat Resistance { get; set;  }

		[Export ("angularResistance")]
		nfloat AngularResistance { get; set;  }

		[Export ("allowsRotation")]
		bool AllowsRotation { get; set;  }

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

		[iOS (9,0)]
		[Export ("charge", ArgumentSemantic.Assign)]
		nfloat Charge { get; set; }

		[iOS (9,0)]
		[Export ("anchored")]
		bool Anchored { [Bind ("isAnchored")] get; set; }
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Protocol]
	[Model]
	interface UIDynamicItem {
		[Abstract]
		[Export ("center")]
		CGPoint Center { get; set;  }

		[Abstract]
		[Export ("bounds")]
		CGRect Bounds { get;  }

		[Abstract]
		[Export ("transform")]
		CGAffineTransform Transform { get; set;  }

		[iOS (9,0)]
		[Export ("collisionBoundsType")]
		UIDynamicItemCollisionBoundsType CollisionBoundsType { get; }

		[iOS (9,0)]
		[Export ("collisionBoundingPath")]
		UIBezierPath CollisionBoundingPath { get; }		
	}

	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UIDynamicItemGroup : UIDynamicItem
	{
		[Export ("initWithItems:")]
		IntPtr Constructor (IUIDynamicItem[] items);
	
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem[] Items { get; }
	}
	

	interface IUIDynamicItem {}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	interface UIDynamicBehavior {
		[Export ("childBehaviors", ArgumentSemantic.Copy)]
		UIDynamicBehavior [] ChildBehaviors { get;  }

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

	[iOS (9,0)]
	[BaseType (typeof(UIDynamicBehavior))]
	[DisableDefaultCtor]
	interface UIFieldBehavior
	{
		[Export ("addItem:")]
		void AddItem (IUIDynamicItem item);
	
		[Export ("removeItem:")]
		void RemoveItem (IUIDynamicItem item);
	
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem[] Items { get; }
	
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
#endif // !WATCH
	
	[Static][Internal]
	[iOS (8,2)]
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
		nfloat Semibold  { get; }
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
	interface UIFont : NSCopying {
		[Static] [Export ("systemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _SystemFontOfSize (nfloat size);

		[iOS (8,2)]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // we prefer to show the one using the enum
		[Internal] // bug 25511
		[Static][Export ("systemFontOfSize:weight:")]
		IntPtr _SystemFontOfSize (nfloat size, nfloat weight);

		[iOS (9,0)]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // we prefer to show the one using the enum
		[Internal] // bug 25511
		[Static][Export ("monospacedDigitSystemFontOfSize:weight:")]
		IntPtr _MonospacedDigitSystemFontOfSize (nfloat fontSize, nfloat weight);

		[Static] [Export ("boldSystemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _BoldSystemFontOfSize (nfloat size);

		[Static] [Export ("italicSystemFontOfSize:")]
		[Internal] // bug 25511
		IntPtr _ItalicSystemFontOfSize (nfloat size);

		[Static] [Export ("fontWithName:size:")]
		[Internal] // bug 25511
		IntPtr _FromName (string name, nfloat size);

		[NoWatch][NoTV]
		[Static] [Export ("labelFontSize")]
		nfloat LabelFontSize { get; }

		[NoWatch][NoTV]
		[Static] [Export ("buttonFontSize")]
		nfloat ButtonFontSize { get; }

		[NoWatch][NoTV]
		[Static] [Export ("smallSystemFontSize")]
		nfloat SmallSystemFontSize { get; }

		[NoWatch][NoTV]
		[Static] [Export ("systemFontSize")]
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
		nfloat xHeight { get; }

		[Export ("lineHeight")]
		nfloat LineHeight { get; }

		[Static] [Export ("familyNames")]
		string [] FamilyNames { get; }

		[Static] [Export ("fontNamesForFamilyName:")]
		string [] FontNamesForFamilyName (string familyName);

		[iOS (7,0)]
		[Export ("fontDescriptor")]
		UIFontDescriptor FontDescriptor { get; }

		[iOS (7,0)]
		[Static, Export ("fontWithDescriptor:size:")]
		[Internal] // bug 25511
		IntPtr _FromDescriptor (UIFontDescriptor descriptor, nfloat pointSize);

		[iOS (7,0)]
		[Static, Export ("preferredFontForTextStyle:")]
		[Internal] // bug 25511
		IntPtr _GetPreferredFontForTextStyle (NSString uiFontTextStyle);

		// FIXME [Watch (3,0)] the API is present but UITraitCollection is not exposed / rdar 27785753
#if !WATCH
		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("preferredFontForTextStyle:compatibleWithTraitCollection:")]
		[Internal]
		IntPtr _GetPreferredFontForTextStyle (NSString uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);
#endif
	}

	public enum UIFontTextStyle {
		[iOS (7,0)]
		[Field ("UIFontTextStyleHeadline")]
		Headline,

		[iOS (7,0)]
		[Field ("UIFontTextStyleBody")]
		Body,

		[iOS (7,0)]
		[Field ("UIFontTextStyleSubheadline")]
		Subheadline,

		[iOS (7,0)]
		[Field ("UIFontTextStyleFootnote")]
		Footnote,

		[iOS (7,0)]
		[Field ("UIFontTextStyleCaption1")]
		Caption1,

		[iOS (7,0)]
		[Field ("UIFontTextStyleCaption2")]
		Caption2,

		[iOS (9,0)]
		[Field ("UIFontTextStyleTitle1")]
		Title1,
		
		[iOS (9,0)]
		[Field ("UIFontTextStyleTitle2")]
		Title2,
		
		[iOS (9,0)]
		[Field ("UIFontTextStyleTitle3")]
		Title3,
		
		[iOS (9,0)]
		[Field ("UIFontTextStyleCallout")]
		Callout,

		[NoTV]
		[iOS (11,0), Watch (5,0)]
		[Field ("UIFontTextStyleLargeTitle")]
		LargeTitle,
	}

	[iOS (7,0)]
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

		[Static, Wrap ("FromAttributes (attributes == null ? null : attributes.Dictionary)")]
		UIFontDescriptor FromAttributes (UIFontAttributes attributes);
	
		[Static, Export ("fontDescriptorWithName:size:")]
		UIFontDescriptor FromName (string fontName, nfloat size);
	
		[Static, Export ("fontDescriptorWithName:matrix:")]
		UIFontDescriptor FromName (string fontName, CGAffineTransform matrix);
	
		[Static, Export ("preferredFontDescriptorWithTextStyle:")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (NSString uiFontTextStyle);

		[Static]
		[Wrap ("GetPreferredDescriptorForTextStyle (uiFontTextStyle.GetConstant ())")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (UIFontTextStyle uiFontTextStyle);

		// FIXME [Watch (3,0)] the API is present but UITraitCollection is not exposed / rdar #27785753
#if !WATCH
		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("preferredFontDescriptorWithTextStyle:compatibleWithTraitCollection:")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (NSString uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);

		[iOS (10,0), TV (10,0)]
		[Static]
		[Wrap ("GetPreferredDescriptorForTextStyle (uiFontTextStyle.GetConstant (), traitCollection)")]
		UIFontDescriptor GetPreferredDescriptorForTextStyle (UIFontTextStyle uiFontTextStyle, [NullAllowed] UITraitCollection traitCollection);
#endif
	
		[DesignatedInitializer]
		[Export ("initWithFontAttributes:")]
		IntPtr Constructor (NSDictionary attributes);
		
		[DesignatedInitializer]
		[Wrap ("this (attributes == null ? null : attributes.Dictionary)")]
		IntPtr Constructor (UIFontAttributes attributes);

		[Export ("fontDescriptorByAddingAttributes:")]
		UIFontDescriptor CreateWithAttributes (NSDictionary attributes);
		
		[Wrap ("CreateWithAttributes (attributes == null ? null : attributes.Dictionary)")]
		UIFontDescriptor CreateWithAttributes (UIFontAttributes attributes);

		[Export ("fontDescriptorWithSymbolicTraits:")]
		UIFontDescriptor CreateWithTraits (UIFontDescriptorSymbolicTraits symbolicTraits);
		
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

#if !XAMCORE_2_0
		// that got removed in iOS7 betas... so we remove it from unified
		// in case Apple starts to disallow it (and rejects apps that refers to it)
		[Internal, Field ("UIFontDescriptorVariationAttribute")]
		NSString VariationAttribute { get; }
#endif
		
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

#if !WATCH
	[BaseType (typeof(NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type[] {typeof (UIGestureRecognizerDelegate)})]
	interface UIGestureRecognizer {
		[DesignatedInitializer]
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("initWithTarget:action:")]
		[Sealed]
		[Internal]
		IntPtr Constructor (NSObject target, IntPtr /* SEL */ action);
		
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
		[Internal] [Sealed]
		void AddTarget (NSObject target, IntPtr action);

		[Export ("removeTarget:action:")]
		void RemoveTarget ([NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("removeTarget:action:")]
		[Internal] [Sealed]
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

		[TV (11,0), iOS (11,0)]
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		//
		// These come from the UIGestureRecognizerProtected category, and you should only call
		// these methods from a subclass of UIGestureRecognizer, never externally
		//

		[Export ("ignoreTouch:forEvent:")]
		void IgnoreTouch (UITouch touch, UIEvent forEvent);

		[iOS (9,0)]
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

		[iOS (7,0)]
		[Export ("shouldRequireFailureOfGestureRecognizer:")]
		bool ShouldRequireFailureOfGestureRecognizer (UIGestureRecognizer otherGestureRecognizer);

		[iOS (7,0)]
		[Export ("shouldBeRequiredToFailByGestureRecognizer:")]
		bool ShouldBeRequiredToFailByGestureRecognizer (UIGestureRecognizer otherGestureRecognizer);

		[iOS (9,1)]
		[Export ("touchesEstimatedPropertiesUpdated:")]
		void TouchesEstimatedPropertiesUpdated (NSSet touches);

		// FIXME: likely an array of UITouchType
		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("allowedTouchTypes", ArgumentSemantic.Copy)]
		NSNumber[] AllowedTouchTypes { get; set; }

		// FIXME: likely an array of UIPressType
		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("allowedPressTypes", ArgumentSemantic.Copy)]
		NSNumber[] AllowedPressTypes { get; set; }

		[iOS (9,2)]
		[TV (9,1)]
		[Export ("requiresExclusiveTouchType")]
		bool RequiresExclusiveTouchType { get; set; }

		[iOS (9,0)]
		[Export ("pressesBegan:withEvent:")]
		void PressesBegan (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesChanged:withEvent:")]
		void PressesChanged (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesEnded:withEvent:")]
		void PressesEnded (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesCancelled:withEvent:")]
		void PressesCancelled (NSSet<UIPress> presses, UIPressesEvent evt);
	}

	[NoWatch]
	[BaseType (typeof(NSObject))]
	[Model]
	[Protocol]
	interface UIGestureRecognizerDelegate {
		[Export ("gestureRecognizer:shouldReceiveTouch:"), DefaultValue (true), DelegateName ("UITouchEventArgs")]
		bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch);

		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[Export ("gestureRecognizerShouldBegin:"), DelegateName ("UIGestureProbe"), DefaultValue (true)]
		bool ShouldBegin (UIGestureRecognizer recognizer);

		[iOS (7,0)]
		[Export ("gestureRecognizer:shouldBeRequiredToFailByGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldBeRequiredToFailBy (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[iOS (7,0)]
		[Export ("gestureRecognizer:shouldRequireFailureOfGestureRecognizer:"), DelegateName ("UIGesturesProbe"), DefaultValue (false)]
		bool ShouldRequireFailureOf (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer);

		[iOS (9,0)]
		[Export ("gestureRecognizer:shouldReceivePress:"), DelegateName ("UIGesturesPress"), DefaultValue (false)]
		bool ShouldReceivePress (UIGestureRecognizer gestureRecognizer, UIPress press);
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface UIGraphicsRendererFormat : NSCopying
	{
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'PreferredFormat' instead.")]
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsRendererFormat DefaultFormat { get; }
	
		[Export ("bounds")]
		CGRect Bounds { get; }

		[TV (11,0), iOS (11,0)]
		[Static]
		[Export ("preferredFormat")]
		UIGraphicsRendererFormat PreferredFormat { get; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	interface UIGraphicsRendererContext
	{
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

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	[Abstract] // quote form headers "An abstract base class for creating graphics renderers. Do not use this class directly."
	interface UIGraphicsRenderer
	{
		[Export ("initWithBounds:")]
		IntPtr Constructor (CGRect bounds);
	
		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect bounds, UIGraphicsRendererFormat format);
	
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

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(UIGraphicsRendererFormat))]
	interface UIGraphicsImageRendererFormat
	{
		[Export ("scale")]
		nfloat Scale { get; set; }

		[Export ("opaque")]
		bool Opaque { get; set; }

		[Deprecated (PlatformName.iOS, 12, 0, message: "Use the 'PreferredRange' property instead.")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use the 'PreferredRange' property instead.")]
		[Export ("prefersExtendedRange")]
		bool PrefersExtendedRange { get; set; }

		[New] // kind of overloading a static member, make it return `instancetype`
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsImageRendererFormat DefaultFormat { get; }

		[iOS (11,0), TV (11,0)]
		[Static]
		[Export ("formatForTraitCollection:")]
		UIGraphicsImageRendererFormat GetFormat (UITraitCollection traitCollection);

		[TV (12, 0), iOS (12, 0)]
		[Export ("preferredRange", ArgumentSemantic.Assign)]
		UIGraphicsImageRendererFormatRange PreferredRange { get; set; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(UIGraphicsRendererContext))]
	interface UIGraphicsImageRendererContext
	{
		[Export ("currentImage")]
		UIImage CurrentImage { get; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(UIGraphicsRenderer))]
	interface UIGraphicsImageRenderer
	{
		[Export ("initWithSize:")]
		IntPtr Constructor (CGSize size);

		[Export ("initWithSize:format:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGSize size, UIGraphicsImageRendererFormat format);

		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect bounds, UIGraphicsImageRendererFormat format);

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

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof (UIGraphicsRendererFormat), Name="UIGraphicsPDFRendererFormat")]
	interface UIGraphicsPdfRendererFormat
	{
		[Export ("documentInfo", ArgumentSemantic.Copy)]
		// TODO: add strongly typed binding
		NSDictionary<NSString, NSObject> DocumentInfo { get; set; }

		[New] // kind of overloading a static member, make it return `instancetype`
		[Static]
		[Export ("defaultFormat")]
		UIGraphicsPdfRendererFormat DefaultFormat { get; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof (UIGraphicsRendererContext), Name="UIGraphicsPDFRendererContext")]
	interface UIGraphicsPdfRendererContext
	{
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


	[iOS (10,0), TV (10,0)]
	[BaseType (typeof (UIGraphicsRenderer), Name = "UIGraphicsPDFRenderer")]
	interface UIGraphicsPdfRenderer
	{
		[Export ("initWithBounds:format:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect bounds, UIGraphicsPdfRendererFormat format);

		[Export ("writePDFToURL:withActions:error:")]
		bool WritePdf (NSUrl url, Action<UIGraphicsPdfRendererContext> actions, out NSError error);

		[Export ("PDFDataWithActions:")]
		NSData CreatePdf (Action<UIGraphicsPdfRendererContext> actions);
	}

	[BaseType (typeof (UIDynamicBehavior))]
	[iOS (7,0)]
	interface UIGravityBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		IntPtr Constructor ([Params] IUIDynamicItem [] items);
		
		[Export ("items", ArgumentSemantic.Copy)]
		IUIDynamicItem [] Items { get;  }

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

	// HACK: those members are not *required* in ObjC but we made them
	// abstract to have them inlined in UITextField and UITextView
	// Even more confusing it that respondToSelecttor return NO on them
	// even if it works in _real_ life (compare unit and introspection tests)
	[Protocol]
	interface UITextInputTraits {
		[Abstract]
		[Export ("autocapitalizationType")]
		UITextAutocapitalizationType AutocapitalizationType { get; set; }
	
		[Abstract]
		[Export ("autocorrectionType")]
		UITextAutocorrectionType AutocorrectionType { get; set;  }
	
		[Abstract]
		[Export ("keyboardType")]
		UIKeyboardType KeyboardType { get; set;  }
	
		[Abstract]
		[Export ("keyboardAppearance")]
		UIKeyboardAppearance KeyboardAppearance { get; set;  }
	
		[Abstract]
		[Export ("returnKeyType")]
		UIReturnKeyType ReturnKeyType { get; set;  }
	
		[Abstract]
		[Export ("enablesReturnKeyAutomatically")]
		bool EnablesReturnKeyAutomatically { get; set;  }
	
		[Abstract]
		[Export ("secureTextEntry")]
		bool SecureTextEntry { [Bind ("isSecureTextEntry")] get; set;  }

		[Abstract]
		[Export ("spellCheckingType")]
		UITextSpellCheckingType SpellCheckingType { get; set; }

		[iOS (10, 0)] // Did not add abstract here breaking change, anyways this is optional in objc
		[Export ("textContentType")]
		NSString TextContentType { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("smartQuotesType", ArgumentSemantic.Assign)]
		UITextSmartQuotesType SmartQuotesType { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("smartDashesType", ArgumentSemantic.Assign)]
		UITextSmartDashesType SmartDashesType { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("smartInsertDeleteType", ArgumentSemantic.Assign)]
		UITextSmartInsertDeleteType SmartInsertDeleteType { get; set; }

		[iOS (12, 0)]
		[NullAllowed, Export ("passwordRules", ArgumentSemantic.Copy)]
		UITextInputPasswordRules PasswordRules { get; set; }
	}

	interface UIKeyboardEventArgs {
		[Export ("UIKeyboardFrameBeginUserInfoKey")]
		CGRect FrameBegin { get; }

		[NoTV]
		[Export ("UIKeyboardFrameEndUserInfoKey")]
		CGRect FrameEnd { get; }

		[NoTV]
		[Export ("UIKeyboardAnimationDurationUserInfoKey")]
		double AnimationDuration { get; }

		[NoTV]
		[Export ("UIKeyboardAnimationCurveUserInfoKey")]
		UIViewAnimationCurve AnimationCurve { get; }
	}
	
	[NoTV]
	[Static]
	interface UIKeyboard {
		[NoTV]
		[Field ("UIKeyboardWillShowNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillShowNotification { get; }

		[NoTV]
		[Field ("UIKeyboardDidShowNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidShowNotification { get; }

		[NoTV]
		[Field ("UIKeyboardWillHideNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillHideNotification { get; }

		[NoTV]
		[Field ("UIKeyboardDidHideNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidHideNotification { get; }

		[NoTV]
		[Field("UIKeyboardWillChangeFrameNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString WillChangeFrameNotification { get; }
		
		[NoTV]
		[Field("UIKeyboardDidChangeFrameNotification")]
		[Notification (typeof (UIKeyboardEventArgs))]
		NSString DidChangeFrameNotification { get; }

#if !XAMCORE_3_0
		//
		// Deprecated methods
		//

		[NoTV]
		[Availability (Deprecated = Platform.iOS_3_2)]
		[Field ("UIKeyboardCenterBeginUserInfoKey")]
		NSString CenterBeginUserInfoKey { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_3_2)]
		[Field ("UIKeyboardCenterEndUserInfoKey")]
		NSString CenterEndUserInfoKey { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_3_2)]
		[Field ("UIKeyboardBoundsUserInfoKey")]
		NSString BoundsUserInfoKey { get; }
#endif
		//
		// Keys
		//
		[NoTV]
		[Field ("UIKeyboardAnimationCurveUserInfoKey")]
		NSString AnimationCurveUserInfoKey { get; }

		[NoTV]
		[Field ("UIKeyboardAnimationDurationUserInfoKey")]
		NSString AnimationDurationUserInfoKey { get; }

		[NoTV]
		[Field ("UIKeyboardFrameEndUserInfoKey")]
		NSString FrameEndUserInfoKey { get; }

		[NoTV]
		[Field ("UIKeyboardFrameBeginUserInfoKey")]
		NSString FrameBeginUserInfoKey { get; }

		[NoTV]
		[iOS (9,0)]
		[Field ("UIKeyboardIsLocalUserInfoKey")]
		NSString IsLocalUserInfoKey { get; }
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIKeyCommand : NSCopying, NSSecureCoding {
		[NullAllowed, Export ("input")]
		NSString Input { get; }

		[Export ("modifierFlags")]
		UIKeyModifierFlags ModifierFlags { get; }

		[Static, Export ("keyCommandWithInput:modifierFlags:action:")]
		UIKeyCommand Create (NSString keyCommandInput, UIKeyModifierFlags modifierFlags, Selector action);

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

		[iOS (9,0)]
		[Static]
		[Export ("keyCommandWithInput:modifierFlags:action:discoverabilityTitle:")]
		UIKeyCommand Create (NSString keyCommandInput, UIKeyModifierFlags modifierFlags, Selector action, NSString discoverabilityTitle);

		[iOS (9,0)]
		[NullAllowed]
		[Export ("discoverabilityTitle")]
		NSString DiscoverabilityTitle { get; set; }
	}

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

	[BaseType (typeof (NSObject))]
	interface UITextPosition {
	}

	[BaseType (typeof (NSObject))]
	interface UITextRange {
		[Export ("isEmpty")]
		bool IsEmpty { get; }

#if XAMCORE_2_0
		[Export ("start")]
		UITextPosition Start { get;  }

		[Export ("end")]
		UITextPosition End { get;  }
#else
		[Export ("start"), Obsolete ("Use 'Start' instead.")]
		UITextPosition start { get;  }

		[Wrap ("start")]
		UITextPosition Start { get; }

		[Export ("end"), Obsolete ("Use 'End' instead.")]
		UITextPosition end { get;  }

		[Wrap ("end")]
		UITextPosition End { get; }
#endif
	}

	interface IUITextInput {}

	[Protocol]
	interface UITextInput : UIKeyInput {
		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("selectedTextRange", ArgumentSemantic.Copy)]
		UITextRange SelectedTextRange { get; set;  }

		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("markedTextStyle", ArgumentSemantic.Copy)]
		NSDictionary MarkedTextStyle { get; set;  }

		[Abstract]
		[Export ("beginningOfDocument")]
		UITextPosition BeginningOfDocument { get;  }

		[Abstract]
		[Export ("endOfDocument")]
		UITextPosition EndOfDocument { get;  }

		[Abstract]
		[Export ("inputDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakInputDelegate { get; set;  }

		[Wrap ("WeakInputDelegate")]
		[Protocolize]
		UITextInputDelegate InputDelegate { get; set; }

		[Abstract]
		[Export ("tokenizer")]
		NSObject WeakTokenizer { get;  }

		[Wrap ("WeakTokenizer")]
		[Protocolize]
		UITextInputTokenizer Tokenizer { get; }

		[Export ("textInputView")]
		UIView TextInputView { get;  }

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
		UITextWritingDirection GetBaseWritingDirection (UITextPosition forPosition, UITextStorageDirection direction);

		[Abstract]
		[Export ("setBaseWritingDirection:forRange:")]
		void SetBaseWritingDirectionforRange (UITextWritingDirection writingDirection, UITextRange range);

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

		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'NSAttributedString.BackgroundColorAttributeName'.")]
		[Field ("UITextInputTextBackgroundColorKey")]
		[NoTV]
		NSString TextBackgroundColorKey { get; }

		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'NSAttributedString.ForegroundColorAttributeName'.")]
		[Field ("UITextInputTextColorKey")]
		[NoTV]
		NSString TextColorKey { get; }

		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'NSAttributedString.FontAttributeName'.")]
		[Field ("UITextInputTextFontKey")]
		[NoTV]
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

		[iOS (6,0)]
		[Abstract]
		[Export ("selectionRectsForRange:")]
		UITextSelectionRect [] GetSelectionRects (UITextRange range);

		[iOS (6,0)]
		[Export ("shouldChangeTextInRange:replacementText:")]
		bool ShouldChangeTextInRange (UITextRange inRange, string replacementText);

		[iOS (6,0)]
		[Export ("frameForDictationResultPlaceholder:")]
		CGRect GetFrameForDictationResultPlaceholder (NSObject placeholder);

		[iOS (6,0)]
		[Export ("insertDictationResultPlaceholder")]
		NSObject InsertDictationResultPlaceholder ();

		[iOS (6,0)]
		[Export ("removeDictationResultPlaceholder:willInsertResult:")]
		void RemoveDictationResultPlaceholder (NSObject placeholder, bool willInsertResult);

		[iOS (9,0)]
		[Export ("beginFloatingCursorAtPoint:")]
		void BeginFloatingCursor (CGPoint point);

		[iOS (9,0)]
		[Export ("updateFloatingCursorAtPoint:")]
		void UpdateFloatingCursor (CGPoint point);

		[iOS (9,0)]
		[Export ("endFloatingCursor")]
		void EndFloatingCursor ();
	}

	[NoTV]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UITextInputAssistantItem
	{
		[Export ("allowsHidingShortcuts")]
		bool AllowsHidingShortcuts { get; set; }

		[Export ("leadingBarButtonGroups", ArgumentSemantic.Copy), NullAllowed]
		UIBarButtonItemGroup[] LeadingBarButtonGroups { get; set; }
	
		[Export ("trailingBarButtonGroups", ArgumentSemantic.Copy), NullAllowed]
		UIBarButtonItemGroup[] TrailingBarButtonGroups { get; set; }
	}
			
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

#if XAMCORE_2_0
	[BaseType (typeof (NSObject))]
	interface UITextInputStringTokenizer : UITextInputTokenizer{
#else
	[BaseType (typeof (UITextInputTokenizer))]
	interface UITextInputStringTokenizer {
#endif
		[Export ("initWithTextInput:")]
#if XAMCORE_2_0
		IntPtr Constructor (IUITextInput textInput);
#else
		IntPtr Constructor (NSObject /* UITextInput */ textInput);
#endif
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITextInputDelegate {
		[Abstract]
		[Export ("selectionWillChange:")]
#if XAMCORE_2_0
		void SelectionWillChange (IUITextInput uiTextInput);
#else
		void SelectionWillChange (NSObject /* UITextInput */ uiTextInput);
#endif

		[Abstract]
		[Export ("selectionDidChange:")]
#if XAMCORE_2_0
		void SelectionDidChange (IUITextInput uiTextInput);
#else
		void SelectionDidChange (NSObject /* UITextInput */ uiTextInput);
#endif

		[Abstract]
		[Export ("textWillChange:")]
#if XAMCORE_2_0
		void TextWillChange (IUITextInput textInput);
#else
		void TextWillChange (NSObject /* UITextInput */ textInput);
#endif

		[Abstract]
		[Export ("textDidChange:")]
#if XAMCORE_2_0
		void TextDidChange (IUITextInput textInput);
#else
		void TextDidChange (NSObject /* UITextInput */ textInput);
#endif
	}

	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface UITextSelectionRect {
		[Export ("rect")]
		CGRect Rect { get; }
		
		[Export ("writingDirection")]
		UITextWritingDirection WritingDirection { get;  }

		[Export ("containsStart")]
		bool ContainsStart { get;  }

		[Export ("containsEnd")]
		bool ContainsEnd { get;  }

		[Export ("isVertical")]
		bool IsVertical { get;  }
	}

	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	partial interface UILexicon : NSCopying {
	
	    [Export ("entries")]
	    UILexiconEntry [] Entries { get; }
	}
	
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	partial interface UILexiconEntry : NSCopying {
	
	    [Export ("documentText")]
	    string DocumentText { get; }
	
	    [Export ("userInput")]
	    string UserInput { get; }
	}

	[BaseType (typeof (NSObject))]
	interface UILocalizedIndexedCollation {
		[Export ("sectionTitles")]
		string [] SectionTitles { get;  }

		[Export ("sectionIndexTitles")]
		string [] SectionIndexTitles { get;  }

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
#endif // !WATCH

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'UserNotifications.UNNotificationRequest' instead.")]
	[DisableDefaultCtor] // designated
	interface UILocalNotification : NSCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("fireDate", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDate FireDate { get; set;  }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSTimeZone TimeZone { get; set;  }

		[Export ("repeatInterval")]
		NSCalendarUnit RepeatInterval { get; set;  }

		[Export ("repeatCalendar", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSCalendar RepeatCalendar { get; set;  }

		[Export ("alertBody", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertBody { get; set;  }

		[Export ("hasAction")]
		bool HasAction { get; set;  }

		[Export ("alertAction", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertAction { get; set;  }

		[Export ("alertLaunchImage", ArgumentSemantic.Copy)]
		[NullAllowed]
		string AlertLaunchImage { get; set;  }

		[Export ("soundName", ArgumentSemantic.Copy)]
		[NullAllowed]
		string SoundName { get; set;  }

		[Export ("applicationIconBadgeNumber")]
		nint ApplicationIconBadgeNumber { get; set;  }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDictionary UserInfo { get; set;  }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNNotificationSound.DefaultSound' instead.")]
		[Field ("UILocalNotificationDefaultSoundName")]
		NSString DefaultSoundName { get; }

		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("region", ArgumentSemantic.Copy)]
		CLRegion Region { get; set; }

		[iOS (8,0)]
		[Export ("regionTriggersOnce", ArgumentSemantic.UnsafeUnretained)]
		bool RegionTriggersOnce { get; set; }

		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("category")]
		string Category { get; set; }

		[iOS (8,2)]
		[NullAllowed]
		[Export ("alertTitle")]
		string AlertTitle { get; set; }
	}
	
#if !WATCH
	[BaseType (typeof(UIGestureRecognizer))]
	interface UILongPressGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[NoTV]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }

		[Export ("minimumPressDuration")]
		double MinimumPressDuration { get; set; }

		[Export ("allowableMovement")]
		nfloat AllowableMovement { get; set; }

		[Export ("numberOfTapsRequired")]
		nint NumberOfTapsRequired { get; set; }
	}

	[BaseType (typeof(UIGestureRecognizer))]
	interface UITapGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("numberOfTapsRequired")]
		nuint NumberOfTapsRequired { get; set; }

		[NoTV]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }
	}

	[BaseType (typeof(UIGestureRecognizer))]
	interface UIPanGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[NoTV]
		[Export ("minimumNumberOfTouches")]
		nuint MinimumNumberOfTouches { get; set; }

		[NoTV]
		[Export ("maximumNumberOfTouches")]
		nuint MaximumNumberOfTouches { get; set; }

		[Export ("setTranslation:inView:")]
		void SetTranslation (CGPoint translation, [NullAllowed] UIView view);

		[Export ("translationInView:")]
		CGPoint TranslationInView ([NullAllowed] UIView view);

		[Export ("velocityInView:")]
		CGPoint VelocityInView ([NullAllowed] UIView view);
	}

	[NoTV]
	[iOS (7,0)]
	[BaseType (typeof (UIPanGestureRecognizer))]
	interface UIScreenEdgePanGestureRecognizer {

		// inherit .ctor
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("edges", ArgumentSemantic.Assign)]
		UIRectEdge Edges { get; set; }
	}

	//
	// This class comes with an "init" constructor (which we autogenerate)
	// and does not require us to call this with initWithFrame:
	//
	[NoTV]
	[iOS (6,0)]
	[BaseType (typeof (UIControl))]
	interface UIRefreshControl : UIAppearance {
		[Export ("refreshing")]
		bool Refreshing { [Bind ("isRefreshing")] get;  }

		[NullAllowed] // by default this property is null
		[Export ("attributedTitle", ArgumentSemantic.Retain)]
		[Appearance]
		NSAttributedString AttributedTitle { get; set;  }

		[Export ("beginRefreshing")]
		void BeginRefreshing ();

		[Export ("endRefreshing")]
		void EndRefreshing ();
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface UIRegion : NSCopying, NSCoding {
		[Static]
		[Export ("infiniteRegion")]
		UIRegion Infinite { get; }

		[Export ("initWithRadius:")]
		IntPtr Constructor (nfloat radius);

		[Export ("initWithSize:")]
		IntPtr Constructor (CGSize size);

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

	[NoTV]
	[BaseType (typeof(UIGestureRecognizer))]
	interface UIRotationGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("rotation")]
		nfloat Rotation { get; set; }

		[Export ("velocity")]
		nfloat Velocity { get; }
	}

	[NoTV]
	[BaseType (typeof(UIGestureRecognizer))]
	interface UIPinchGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("scale")]
		nfloat Scale { get; set; }

		[Export ("velocity")]
		nfloat Velocity { get; }
	}

	[BaseType (typeof(UIGestureRecognizer))]
	interface UISwipeGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("direction")]
		UISwipeGestureRecognizerDirection Direction { get; set; }

		[NoTV]
		[Export ("numberOfTouchesRequired")]
		nuint NumberOfTouchesRequired { get; set; }
	}
	
	[BaseType (typeof(UIView))]
	interface UIActivityIndicatorView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithActivityIndicatorStyle:")]
		IntPtr Constructor (UIActivityIndicatorViewStyle style);

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
#endif // !WATCH

	[NoWatch, NoTV]
	[iOS (11,0)]
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
		IntPtr Constructor (string filename);

		[ThreadSafe]
		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);
		
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
		[Static] [Export ("imageNamed:")][Autorelease]
		UIImage FromBundle (string name);

#if !WATCH
		// Thread-safe in iOS 9 or later according to docs.
#if IOS
		// tvOS started with 9.0 code base (and watchOS 2.0 came later)
		[Advice ("This API is thread-safe only on 9.0 and later.")]
#endif
		[ThreadSafe]
		[iOS (8,0)]
		[Static, Export ("imageNamed:inBundle:compatibleWithTraitCollection:")]
		UIImage FromBundle (string name, [NullAllowed] NSBundle bundle, [NullAllowed] UITraitCollection traitCollection);
#endif // !WATCH

		[Static] [Export ("imageWithContentsOfFile:")][Autorelease]
		[ThreadSafe]
		UIImage FromFile (string filename);
		
		[Static] [Export ("imageWithData:")][Autorelease]
		[ThreadSafe]
		UIImage LoadFromData (NSData data);

		[Static] [Export ("imageWithCGImage:")][Autorelease]
		[ThreadSafe]
		UIImage FromImage (CGImage image);

		[Static][Export ("imageWithCGImage:scale:orientation:")][Autorelease]
		[ThreadSafe]			
		UIImage FromImage (CGImage image, nfloat scale, UIImageOrientation orientation);

#if !WATCH
		[Static][Export ("imageWithCIImage:")][Autorelease]
		[ThreadSafe]			
		UIImage FromImage (CIImage image);
#endif // !WATCH

		// From the NSItemProviderReading protocol, a static method.
		[Static]
		[iOS (11,0), NoWatch, NoTV]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string[] ReadableTypeIdentifiers { get; }
	
		// From the NSItemProviderReading protocol, a static method.
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[iOS (11,0), NoWatch, NoTV]
		[return: NullAllowed]
#if !WATCH && !TVOS
		new
#endif
		UIImage GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		[Export ("renderingMode")]
		[ThreadSafe]
		[iOS (7,0)]
		UIImageRenderingMode RenderingMode { get;  }

		[Export ("imageWithRenderingMode:")]
		[ThreadSafe]
		[iOS (7,0)]
		UIImage ImageWithRenderingMode (UIImageRenderingMode renderingMode);

		[Export ("CGImage")]
		[ThreadSafe]
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
		[Export ("stretchableImageWithLeftCapWidth:topCapHeight:")][Autorelease]
		[ThreadSafe]
		UIImage StretchableImage (nint leftCapWidth, nint topCapHeight);

		[NoTV]
		[Export ("leftCapWidth")]
		[ThreadSafe]
		nint LeftCapWidth { get; }

		[NoTV]
		[Export ("topCapHeight")]
		[ThreadSafe]
		nint TopCapHeight { get; }

		[Export ("scale")]
		[ThreadSafe]
		nfloat CurrentScale { get; }

		[Static, Export ("animatedImageNamed:duration:")][Autorelease]
		[ThreadSafe]
		UIImage CreateAnimatedImage (string name, double duration);

		[Static, Export ("animatedImageWithImages:duration:")][Autorelease]
		[ThreadSafe]
		UIImage CreateAnimatedImage (UIImage [] images, double duration);

		[Static, Export ("animatedResizableImageNamed:capInsets:duration:")][Autorelease]
		[ThreadSafe]
		UIImage CreateAnimatedImage (string name, UIEdgeInsets capInsets, double duration);

		[Export ("initWithCGImage:")]
		[ThreadSafe]
		IntPtr Constructor (CGImage cgImage);

#if !WATCH
		[Export ("initWithCIImage:")]
		[ThreadSafe]
		IntPtr Constructor (CIImage ciImage);
#endif // !WATCH

		[Export ("initWithCGImage:scale:orientation:")]
		[ThreadSafe]
		IntPtr Constructor (CGImage cgImage, nfloat scale,  UIImageOrientation orientation);

#if !WATCH
		[Export ("CIImage")]
		[ThreadSafe]
		CIImage CIImage { get; }
#endif // !WATCH
		
		[Export ("images")]
		[ThreadSafe]
		UIImage [] Images { get; }

		[Export ("duration")]
		[ThreadSafe]
		double Duration { get; }

		[Export ("resizableImageWithCapInsets:")][Autorelease]
		[ThreadSafe]
		UIImage CreateResizableImage (UIEdgeInsets capInsets);

		[Export ("capInsets")]
		[ThreadSafe]
		UIEdgeInsets CapInsets { get; }

		//
		// 6.0
		//
		[iOS (6,0)]
		[Export ("alignmentRectInsets")]
		[ThreadSafe]
		UIEdgeInsets AlignmentRectInsets { get;  }

		[iOS (6,0)]
		[Static]
		[Export ("imageWithData:scale:")]
		[ThreadSafe, Autorelease]
		UIImage LoadFromData (NSData data, nfloat scale);

#if !WATCH
		[iOS (6,0)]
		[Static]
		[Export ("imageWithCIImage:scale:orientation:")]
		[ThreadSafe, Autorelease]
		UIImage FromImage (CIImage ciImage, nfloat scale, UIImageOrientation orientation);
#endif // !WATCH

		[iOS (6,0)]
		[Export ("initWithData:scale:")]
		[ThreadSafe]
		IntPtr Constructor (NSData data, nfloat scale);

#if !WATCH
		[iOS (6,0)]
		[Export ("initWithCIImage:scale:orientation:")]
		[ThreadSafe]
		IntPtr Constructor (CIImage ciImage, nfloat scale, UIImageOrientation orientation);
#endif // !WATCH
	
		[iOS (6,0)]
		[Export ("resizableImageWithCapInsets:resizingMode:")]
		[ThreadSafe]
		UIImage CreateResizableImage (UIEdgeInsets capInsets, UIImageResizingMode resizingMode);

		[iOS (6,0)]
		[Static]
		[Export ("animatedResizableImageNamed:capInsets:resizingMode:duration:")]
		[ThreadSafe]
		UIImage CreateAnimatedImage (string name, UIEdgeInsets capInsets, UIImageResizingMode resizingMode, double duration);
		
		[iOS (6,0)]
		[Export ("imageWithAlignmentRectInsets:")]
		[ThreadSafe, Autorelease]
		UIImage ImageWithAlignmentRectInsets (UIEdgeInsets alignmentInsets);

		[iOS (6,0)]
		[Export ("resizingMode")]
		[ThreadSafe]
		UIImageResizingMode ResizingMode { get; }

#if !WATCH
		[iOS (8,0)]
		[Export ("traitCollection")]
		[ThreadSafe]
		UITraitCollection TraitCollection { get; }

		[iOS (8,0)]
		[Export ("imageAsset")]
		[ThreadSafe]
		UIImageAsset ImageAsset { get; }
#endif // !WATCH

		[iOS (9,0)]
		[Export ("imageFlippedForRightToLeftLayoutDirection")]
		UIImage GetImageFlippedForRightToLeftLayoutDirection ();

		[iOS (9,0)]
		[Export ("flipsForRightToLeftLayoutDirection")]
		bool FlipsForRightToLeftLayoutDirection { get; }

#if !WATCH
		[iOS (10,0), TV (10,0)]
		[Export ("imageRendererFormat")]
		UIGraphicsImageRendererFormat ImageRendererFormat { get; }
#endif

		[Watch (3,0)]
		[iOS (10,0), TV (10,0)]
		[Export ("imageWithHorizontallyFlippedOrientation")]
		UIImage GetImageWithHorizontallyFlippedOrientation ();

		// From the NSItemProviderWriting protocol, a static method.
		// NSItemProviderWriting doesn't seem to be implemented for tvOS/watchOS, even though the headers say otherwise.
		[NoWatch, NoTV, iOS (11,0)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if !WATCH && !TVOS
		new
#endif
		string[] WritableTypeIdentifiers { get; }
	}

#if !WATCH
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface UIImageAsset : NSSecureCoding {
		[Export ("imageWithTraitCollection:")]
		UIImage FromTraitCollection (UITraitCollection traitCollection);
		
		[Export ("registerImage:withTraitCollection:")]
		void RegisterImage (UIImage image, UITraitCollection traitCollection);

		[Export ("unregisterImageWithTraitCollection:")]
		void UnregisterImageWithTraitCollection (UITraitCollection traitCollection);
	}

	[BaseType (typeof (NSObject))]
	interface UIEvent {
		[Export ("type")]
		UIEventType Type { get; }

		[Export ("subtype")]
		UIEventSubtype Subtype { get; }
		
		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("allTouches")]
		NSSet AllTouches { get; }

		[Export ("touchesForView:")]
		NSSet TouchesForView (UIView view);

		[Export ("touchesForWindow:")]
		NSSet TouchesForWindow (UIWindow window);
		

		[Export ("touchesForGestureRecognizer:")]
		NSSet TouchesForGestureRecognizer (UIGestureRecognizer window);

		[iOS (9,0)]
		[Export ("coalescedTouchesForTouch:")]
		[return: NullAllowed]
		UITouch[] GetCoalescedTouches (UITouch touch);

		[iOS (9,0)]
		[Export ("predictedTouchesForTouch:")]
		[return: NullAllowed]
		UITouch[] GetPredictedTouches (UITouch touch);		
	}

	// that's one of the few enums based on CGFloat - we expose the [n]float directly in the API
	// but we need a way to give access to the constants to developers
	[Static]
	interface UIWindowLevel {
		[Field ("UIWindowLevelNormal")]
		nfloat Normal { get; }

		[Field ("UIWindowLevelAlert")]
		nfloat Alert { get; }

		[NoTV]
		[Field ("UIWindowLevelStatusBar")]
		nfloat StatusBar { get; }
	}
	
	[BaseType (typeof (UIView))]
	interface UIWindow {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

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
		UIScreen Screen { get; set; }

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
	}

	[BaseType (typeof (UIView))]
	interface UIControl {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("selected")]
		bool Selected { [Bind("isSelected")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("contentVerticalAlignment")]
		UIControlContentVerticalAlignment VerticalAlignment { get; set; }

		[Export ("contentHorizontalAlignment")]
		UIControlContentHorizontalAlignment HorizontalAlignment { get; set; }

		[NoWatch]
		[iOS (11,0), TV (11,0)]
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
		void AddTarget ([NullAllowed] NSObject target,  Selector sel, UIControlEvent events);

		[Sealed]
		[Internal]
		[Export ("addTarget:action:forControlEvents:")]
		void AddTarget ([NullAllowed] NSObject target,  IntPtr sel, UIControlEvent events);

		[Export ("removeTarget:action:forControlEvents:")]
		void RemoveTarget ([NullAllowed] NSObject target, [NullAllowed] Selector sel, UIControlEvent events);

		[Sealed]
		[Internal]
		[Export ("removeTarget:action:forControlEvents:")]
		void RemoveTarget ([NullAllowed] NSObject target, IntPtr sel, UIControlEvent events);

		[Export ("allTargets")]
		NSSet AllTargets { get; }
		
		[Export ("allControlEvents")]
		UIControlEvent AllControlEvents { get; }

		[Export ("actionsForTarget:forControlEvent:")]
		string [] GetActions ([NullAllowed] NSObject target, UIControlEvent events);
		
		[Export ("sendAction:to:forEvent:")]
		void SendAction (Selector action, [NullAllowed] NSObject target, [NullAllowed] UIEvent uievent);

		[Export ("sendActionsForControlEvents:")]
		void SendActionForControlEvents (UIControlEvent events);
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIBarPositioning {
#if XAMCORE_2_0
		[Abstract]
#endif
		[iOS (7,0)]
		[Export ("barPosition")]
		UIBarPosition BarPosition { get; }
	}

	interface IUIBarPositioning {}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIBarPositioningDelegate {
		[Export ("positionForBar:")][DelegateName ("Func<IUIBarPositioning,UIBarPosition>"), NoDefaultValue]
		UIBarPosition GetPositionForBar (IUIBarPositioning barPositioning);
	}
#endif // !WATCH
	
	[BaseType (typeof (NSObject))]
	[ThreadSafe]
	[DisableDefaultCtor] // designated
	interface UIBezierPath : NSSecureCoding, NSCopying {

		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

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
		CGPath CGPath { get; [NullAllowed] set; }

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

		[iOS (6,0)]
		[Export ("bezierPathByReversingPath")]
		UIBezierPath BezierPathByReversingPath ();
	}
	
#if !WATCH
	[BaseType (typeof (UIControl))]
	interface UIButton : UIAccessibilityContentSizeCategoryImageAdjusting
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("buttonWithType:")] [Static]
		UIButton FromType (UIButtonType type);

		[Export ("contentEdgeInsets")]
		UIEdgeInsets ContentEdgeInsets {get;set;}

		[Export ("titleEdgeInsets")]
		UIEdgeInsets TitleEdgeInsets { get; set; }

		[Export ("reversesTitleShadowWhenHighlighted")]
		bool ReverseTitleShadowWhenHighlighted { get; set; }

		[Export ("imageEdgeInsets")]
		UIEdgeInsets ImageEdgeInsets { get; set; }

		[Export ("adjustsImageWhenHighlighted")]
		bool AdjustsImageWhenHighlighted { get; set; }

		[Export ("adjustsImageWhenDisabled")]
		bool AdjustsImageWhenDisabled { get; set; }

		[NoTV]
		[Export ("showsTouchWhenHighlighted")]
		bool ShowsTouchWhenHighlighted { get; set; }

		[Export ("buttonType")]
		UIButtonType ButtonType { get; }

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

		[Export ("backgroundRectForBounds:")]
		CGRect BackgroundRectForBounds (CGRect rect);
		 
		[Export ("contentRectForBounds:")]
		CGRect ContentRectForBounds (CGRect rect);
		 
		[Export ("titleRectForContentRect:")]
		CGRect TitleRectForContentRect (CGRect rect);
		 
		[Export ("imageRectForContentRect:")]
		CGRect ImageRectForContentRect (CGRect rect);

#if !XAMCORE_3_0
		[Deprecated (PlatformName.iOS, 3, 0)]
		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; set; }

		[Deprecated (PlatformName.iOS, 3, 0)]
		[Export ("lineBreakMode")]
		UILineBreakMode LineBreakMode { get; set; }
		
		[Deprecated (PlatformName.iOS, 3, 0)]
		[Export ("titleShadowOffset")]
		CGSize TitleShadowOffset { get; set; }
#endif

		//
		// 6.0
		//
		[iOS (6,0)]
		[Export ("currentAttributedTitle", ArgumentSemantic.Retain)]
		NSAttributedString CurrentAttributedTitle { get;  }

		[iOS (6,0)]
		[Export ("setAttributedTitle:forState:")]
		void SetAttributedTitle (NSAttributedString title, UIControlState state);

		[iOS (6,0)]
		[Export ("attributedTitleForState:")]
		NSAttributedString GetAttributedTitle (UIControlState state);
	}
	
	[BaseType (typeof (UIView))]
	interface UILabel : UIContentSizeCategoryAdjusting {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)][NullAllowed]
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
		UIColor ShadowColor { get; set;}

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
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'MinimumScaleFactor' instead.")]
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
		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set;  }

		[NoTV]
		[iOS (6,0)]
		[Export ("adjustsLetterSpacingToFitWidth")]
		[Availability (Introduced = Platform.iOS_6_0, Deprecated = Platform.iOS_7_0, Message = "Use 'NSKernAttributeName' instead.")]
		bool AdjustsLetterSpacingToFitWidth { get; set;  }

		[iOS (6,0)]
		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set;  }

		[iOS (6,0)]
		[Export ("preferredMaxLayoutWidth")]
		nfloat PreferredMaxLayoutWidth { get; set;  }

		[iOS (9,0)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[TV (12, 0), NoWatch, NoiOS]
		[Export ("enablesMarqueeWhenAncestorFocused")]
		bool EnablesMarqueeWhenAncestorFocused { get; set; }
	}

	[BaseType (typeof (UIView))]
	interface UIImageView
#if !WATCH
	: UIAccessibilityContentSizeCategoryImageAdjusting
#endif // !WATCH
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithImage:")]
		[PostGet ("Image")]
		IntPtr Constructor ([NullAllowed] UIImage image);

		[Export ("initWithImage:highlightedImage:")]
		[PostGet ("Image")]
		[PostGet ("HighlightedImage")]
		IntPtr Constructor ([NullAllowed] UIImage image, [NullAllowed] UIImage highlightedImage);

		[Export ("image", ArgumentSemantic.Retain)][NullAllowed]
		UIImage Image { get; set; }

		[Export ("highlightedImage", ArgumentSemantic.Retain)][NullAllowed]
		UIImage HighlightedImage { get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("animationImages", ArgumentSemantic.Copy)][NullAllowed]
		UIImage [] AnimationImages { get; set; }

		[Export ("highlightedAnimationImages", ArgumentSemantic.Copy)][NullAllowed]
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

		[TV (9,0)] 
		[NoiOS] // UIKIT_AVAILABLE_TVOS_ONLY
		[Export ("adjustsImageWhenAncestorFocused")]
		bool AdjustsImageWhenAncestorFocused { get; set; }

		[TV (9,0)]
		[NoiOS] // UIKIT_AVAILABLE_TVOS_ONLY
		[Export ("focusedFrameGuide")]
		UILayoutGuide FocusedFrameGuide { get; }

		[TV (11, 0), NoWatch, NoiOS]
		[Export ("overlayContentView", ArgumentSemantic.Strong)]
		UIView OverlayContentView { get; }

		[TV (11,0), NoWatch, NoiOS]
		[Export ("masksFocusEffectToContents")]
		bool MasksFocusEffectToContents { get; set; }
	}

	[NoTV]
	[BaseType (typeof (UIControl))]
	interface UIDatePicker {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("datePickerMode")]
		UIDatePickerMode Mode { get; set; }

		[Export ("locale", ArgumentSemantic.Retain)]
		NSLocale Locale { get; [NullAllowed] set; }

		[Export ("timeZone", ArgumentSemantic.Retain)]
		NSTimeZone TimeZone { get; [NullAllowed] set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; [NullAllowed] set; }

		// not fully clear from docs but null is not allowed:
		// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Invalid parameter not satisfying: date
		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }
		
		[Export ("minimumDate", ArgumentSemantic.Retain)]
		NSDate MinimumDate { get; [NullAllowed] set; }
		
		[Export ("maximumDate", ArgumentSemantic.Retain)]
		NSDate MaximumDate { get; [NullAllowed] set; }

		[Export ("countDownDuration")]
		double CountDownDuration { get; set; }

		[Export ("minuteInterval")]
		nint MinuteInterval { get; set; }
		
		[Export ("setDate:animated:")]
		void SetDate (NSDate date, bool animated);
		
	}

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
		[Export ("orientation")]
		UIDeviceOrientation Orientation { get; }

#if false
		[Obsolete ("Deprecated in iOS 5.0")]
		[Export ("uniqueIdentifier", ArgumentSemantic.Retain)]
		string UniqueIdentifier { get; }
#endif

		[NoTV]
		[Export ("generatesDeviceOrientationNotifications")]
		bool GeneratesDeviceOrientationNotifications { [Bind ("isGeneratingDeviceOrientationNotifications")] get; }
				
		[NoTV]
		[Export ("beginGeneratingDeviceOrientationNotifications")]
		void BeginGeneratingDeviceOrientationNotifications ();

		[NoTV]
		[Export ("endGeneratingDeviceOrientationNotifications")]
		void EndGeneratingDeviceOrientationNotifications ();

		[NoTV]
		[Export ("batteryMonitoringEnabled")]
		bool BatteryMonitoringEnabled { [Bind ("isBatteryMonitoringEnabled")] get; set; }

		[NoTV]
		[Export ("batteryState")]
		UIDeviceBatteryState BatteryState { get; }

		[NoTV]
		[Export ("batteryLevel")]
		float BatteryLevel { get; } // This is float, not nfloat

		[Export ("proximityMonitoringEnabled")]
		bool ProximityMonitoringEnabled { [Bind ("isProximityMonitoringEnabled")] get; set; }

		[Export ("proximityState")]
		bool ProximityState { get; }

		[Export ("userInterfaceIdiom")]
		UIUserInterfaceIdiom UserInterfaceIdiom { get; }

		[NoTV]
		[Field ("UIDeviceOrientationDidChangeNotification")]
		[Notification]
		NSString OrientationDidChangeNotification { get; }

		[NoTV]
		[Field ("UIDeviceBatteryStateDidChangeNotification")]
		[Notification]
		NSString BatteryStateDidChangeNotification { get; }

		[NoTV]
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

		[iOS (6,0)]
		[Export ("identifierForVendor", ArgumentSemantic.Retain)]
		NSUuid IdentifierForVendor { get;  }
	}
	
	[BaseType (typeof (NSObject))]
	interface UIDictationPhrase {
		[Export ("alternativeInterpretations")]
		string[] AlternativeInterpretations { get; }
		
		[Export ("text")]
		string Text { get; }
	}

	[NoTV]
	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (UIDocumentInteractionControllerDelegate)})]
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
		UIImage[] Icons { get; }
		
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

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIDocumentInteractionControllerDelegate {
		[Availability (Deprecated = Platform.iOS_6_0)]
		[Export ("documentInteractionController:canPerformAction:"), DelegateName ("UIDocumentInteractionProbe"), DefaultValue (false)]
		bool CanPerformAction (UIDocumentInteractionController controller, [NullAllowed] Selector action);

		[Availability (Deprecated = Platform.iOS_6_0)]
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
		
		[Export ("documentInteractionControllerViewControllerForPreview:"), DelegateName ("UIDocumentViewController"), DefaultValue (null)]
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

	[NoTV]
	[BaseType (typeof (UINavigationController), Delegates=new string [] { "Delegate" }, Events=new Type [] {typeof(UIImagePickerControllerDelegate)})]
	interface UIImagePickerController {
		[Export ("isSourceTypeAvailable:")][Static]
		bool IsSourceTypeAvailable (UIImagePickerControllerSourceType sourceType);
		
		[Export ("availableMediaTypesForSourceType:"), Static]
		string [] AvailableMediaTypes (UIImagePickerControllerSourceType sourceType);
		
		// This is the foundation to implement both id <UINavigationControllerDelegate, UIImagePickerControllerDelegate>
		[Export("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject Delegate { get; set; }

		[Export ("sourceType")]
		UIImagePickerControllerSourceType SourceType { get; set; }

		[Export ("mediaTypes", ArgumentSemantic.Copy)]
		string [] MediaTypes { get; set; }

#if !XAMCORE_3_0
		[Export ("allowsImageEditing")]
		[Availability (Deprecated = Platform.iOS_3_1)]
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

		[Static][Export ("availableCaptureModesForCameraDevice:")]
		NSNumber [] AvailableCaptureModesForCameraDevice (UIImagePickerControllerCameraDevice cameraDevice);

		[Export ("cameraDevice")]
		UIImagePickerControllerCameraDevice CameraDevice { get; set; }

		[Export ("cameraFlashMode")]
		UIImagePickerControllerCameraFlashMode CameraFlashMode { get; set; }

		[Static, Export ("isCameraDeviceAvailable:")]
		bool IsCameraDeviceAvailable (UIImagePickerControllerCameraDevice cameraDevice);

		[Static, Export ("isFlashAvailableForCameraDevice:")]
		bool IsFlashAvailableForCameraDevice (UIImagePickerControllerCameraDevice cameraDevice);

		[iOS (11,0)]
		[Export ("imageExportPreset", ArgumentSemantic.Assign)]
		UIImagePickerControllerImageUrlExportPreset ImageExportPreset { get; set; }

		[iOS (11,0)]
		[Export ("videoExportPreset")]
		string VideoExportPreset { get; set; }

#if XAMCORE_2_0
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
#endif

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIImagePickerController.PHAsset' instead.")]
		[Field ("UIImagePickerControllerReferenceURL")]
		NSString ReferenceUrl { get; }

		[Field ("UIImagePickerControllerMediaMetadata")]
		NSString MediaMetadata { get; }

		[iOS (9,1)]
		[Field ("UIImagePickerControllerLivePhoto")]
		NSString LivePhoto { get; }

		[iOS (11,0)]
		[Field ("UIImagePickerControllerPHAsset")]
		NSString PHAsset { get; }

		[iOS (11,0)]
		[Field ("UIImagePickerControllerImageURL")]
		NSString ImageUrl { get; }
	}

	// UINavigationControllerDelegate, UIImagePickerControllerDelegate
	[BaseType (typeof (UINavigationControllerDelegate))]
	[NoTV]
	[Model]
	[Protocol]
	interface UIImagePickerControllerDelegate {
#if !XAMCORE_3_0
		[Availability (Obsoleted = Platform.iOS_3_0)]
		[Export ("imagePickerController:didFinishPickingImage:editingInfo:"), EventArgs ("UIImagePickerImagePicked")]
		void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo);
#endif

		[Export ("imagePickerController:didFinishPickingMediaWithInfo:"), EventArgs ("UIImagePickerMediaPicked")]
		void FinishedPickingMedia (UIImagePickerController picker, NSDictionary info);

		[Export ("imagePickerControllerDidCancel:"), EventArgs ("UIImagePickerController")]
		void Canceled (UIImagePickerController picker);
	}

	[NoTV]
	[BaseType (typeof (UIDocument))]
	// *** Assertion failure in -[UIManagedDocument init], /SourceCache/UIKit_Sim/UIKit-1914.84/UIDocument.m:258
	[DisableDefaultCtor]
	interface UIManagedDocument {
		// note: ctor are not inherited, but this is how the documentation tells you to create an UIManagedDocument
		// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/UIManagedDocument_Class/Reference/Reference.html
		[Export ("initWithFileURL:")]
		[PostGet ("FileUrl")]
		IntPtr Constructor (NSUrl url);

		[Export ("managedObjectContext", ArgumentSemantic.Retain)]
		NSManagedObjectContext ManagedObjectContext { get;  }

		[Export ("managedObjectModel", ArgumentSemantic.Retain)]
		NSManagedObjectModel ManagedObjectModel { get;  }

		[Export ("persistentStoreOptions", ArgumentSemantic.Copy)]
		NSDictionary PersistentStoreOptions { get; set;  }

		[Export ("modelConfiguration", ArgumentSemantic.Copy)]
		string ModelConfiguration { get; set;  }

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

	[NoTV]
	[BaseType (typeof (NSObject))]
	interface UIMenuController {
		[Static, Export ("sharedMenuController")]
		UIMenuController SharedMenuController { get; }

		[Export ("menuVisible")]
		bool MenuVisible { [Bind ("isMenuVisible")] get; set; }

		[Export ("setMenuVisible:animated:")]
		void SetMenuVisible (bool visible, bool animated);

		[Export ("setTargetRect:inView:")]
		void SetTargetRect (CGRect rect, UIView inView);

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

	[NoTV]
	[BaseType (typeof (NSObject))]
	interface UIMenuItem {
		[DesignatedInitializer] // TODO: Add an overload that takes an Action maybe?
		[Export ("initWithTitle:action:")]
		IntPtr Constructor (string title, Selector action);

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("action")]
		Selector Action { get; set; }
	}

	[BaseType (typeof (UIView))]
	interface UINavigationBar : UIBarPositioning, NSCoding {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NoTV]
		// [Appearance] rdar://22818366
		[Appearance]
		[Export ("barStyle", ArgumentSemantic.Assign)]
		UIBarStyle BarStyle { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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
#if XAMCORE_2_0
		UINavigationItem PopNavigationItem (bool animated);
#else
		UINavigationItem PopNavigationItemAnimated (bool animated);
#endif

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
		NSDictionary _TitleTextAttributes { get; set;  }

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
		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("shadowImage", ArgumentSemantic.Retain)]
		UIImage ShadowImage { get; set;  }

		//
		// 7.0
		//
		[iOS (7,0)]
		[Appearance]
		[NullAllowed]
		[Export ("barTintColor", ArgumentSemantic.Retain)]
		UIColor BarTintColor { get; set;  }

		[NoTV]
		[iOS (7,0)]
		[Appearance]
		[NullAllowed]
		[Export ("backIndicatorImage", ArgumentSemantic.Retain)]
		UIImage BackIndicatorImage { get; set;  }

		[NoTV]
		[iOS (7,0)]
		[Appearance]
		[NullAllowed]
		[Export ("backIndicatorTransitionMaskImage", ArgumentSemantic.Retain)]
		UIImage BackIndicatorTransitionMaskImage { get; set;  }

		[iOS (7,0)]
		[Appearance]
		[Export ("setBackgroundImage:forBarPosition:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarPosition barPosition, UIBarMetrics barMetrics);

		[iOS (7,0)]
		[Appearance]
		[Export ("backgroundImageForBarPosition:barMetrics:")]
		UIImage GetBackgroundImage (UIBarPosition barPosition, UIBarMetrics barMetrics);
		
		[NoTV]
		[iOS (11,0)]
		[Export ("prefersLargeTitles")]
		bool PrefersLargeTitles { get; set; }

		[NoTV]
		[iOS (11,0)]
		[Internal, NullAllowed, Export ("largeTitleTextAttributes", ArgumentSemantic.Copy)]
		[Appearance]
		NSDictionary _LargeTitleTextAttributes { get; set; }

		[NoTV]
		[iOS (11,0)]
		[Wrap ("_LargeTitleTextAttributes")]
		[Appearance]
		UIStringAttributes LargeTitleTextAttributes { get; set; }
	}

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
	}

	[BaseType (typeof (NSObject))]
	interface UINavigationItem : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithTitle:")]
		IntPtr Constructor (string title);

		[NullAllowed] // by default this property is null
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NoTV]
		[NullAllowed] // by default this property is null
		[Export ("backBarButtonItem", ArgumentSemantic.Retain)]
		UIBarButtonItem BackBarButtonItem { get; set; }

		[Export ("titleView", ArgumentSemantic.Retain), NullAllowed]
		UIView TitleView { get; [NullAllowed] set; }

		[NoTV]
		[Export ("prompt", ArgumentSemantic.Copy), NullAllowed]
		string Prompt { get; set; }

		[NoTV]
		[Export ("hidesBackButton", ArgumentSemantic.Assign)]
		bool HidesBackButton { get; set; }

		[NoTV]
		[Export ("setHidesBackButton:animated:")]
		void SetHidesBackButton (bool hides, bool animated);

		[Export ("leftBarButtonItem", ArgumentSemantic.Retain)][NullAllowed]
		UIBarButtonItem LeftBarButtonItem {
			get;
			// only on the setter to avoid endless recursion
			[PostGet ("LeftBarButtonItems")]
			set; 
		}

		[Export ("rightBarButtonItem", ArgumentSemantic.Retain)][NullAllowed]
		UIBarButtonItem RightBarButtonItem { 
			get; 
			// only on the setter to avoid endless recursion
			[PostGet ("RightBarButtonItems")]
			set; 
		}

		[Export ("setLeftBarButtonItem:animated:")][PostGet ("LeftBarButtonItem")]
		void SetLeftBarButtonItem ([NullAllowed] UIBarButtonItem item, bool animated);
		
		[Export ("setRightBarButtonItem:animated:")][PostGet ("RightBarButtonItem")]
		void SetRightBarButtonItem ([NullAllowed] UIBarButtonItem item, bool animated);

		[NullAllowed] // by default this property is null
		[Export ("leftBarButtonItems", ArgumentSemantic.Copy)]
		[PostGet ("LeftBarButtonItem")]
		UIBarButtonItem [] LeftBarButtonItems { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("rightBarButtonItems", ArgumentSemantic.Copy)]
		[PostGet ("RightBarButtonItem")]
		UIBarButtonItem [] RightBarButtonItems { get; set;  }

		[NoTV]
		[Export ("leftItemsSupplementBackButton")]
		bool LeftItemsSupplementBackButton { get; set;  }

		[Export ("setLeftBarButtonItems:animated:")][PostGet ("LeftBarButtonItems")]
		void SetLeftBarButtonItems (UIBarButtonItem [] items, bool animated);

		[Export ("setRightBarButtonItems:animated:")][PostGet ("RightBarButtonItems")]
		void SetRightBarButtonItems (UIBarButtonItem [] items, bool animated);

		[NoTV]
		[iOS (11,0)]
		[Export ("largeTitleDisplayMode", ArgumentSemantic.Assign)]
		UINavigationItemLargeTitleDisplayMode LargeTitleDisplayMode { get; set; }

		[NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("searchController", ArgumentSemantic.Retain)]
		UISearchController SearchController { get; set; }

		[NoTV]
		[iOS (11,0)]
		[Export ("hidesSearchBarWhenScrolling")]
		bool HidesSearchBarWhenScrolling { get; set; }
	}
	
	[BaseType (typeof (UIViewController))]
	interface UINavigationController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Internal, Export ("initWithNavigationBarClass:toolbarClass:")]
		IntPtr Constructor (IntPtr navigationBarClass, IntPtr toolbarClass);

		[Export ("initWithRootViewController:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		IntPtr Constructor (UIViewController rootViewController);

		[Export ("pushViewController:animated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
		void PushViewController (UIViewController viewController, bool animated);

		[Export ("popViewControllerAnimated:")]
		[PostGet ("ViewControllers")] // that will PostGet TopViewController and VisibleViewController too
#if XAMCORE_2_0
		UIViewController PopViewController (bool animated);
#else
		UIViewController PopViewControllerAnimated (bool animated);
#endif

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
		bool NavigationBarHidden { [Bind ("isNavigationBarHidden")] get ; set; } 

		[Export ("setNavigationBarHidden:animated:")]
		void SetNavigationBarHidden (bool hidden, bool animated);
		
		[Export ("navigationBar")]
		UINavigationBar NavigationBar { get; } 

		[NoTV]
		[Export ("toolbarHidden")]
		bool ToolbarHidden { [Bind ("isToolbarHidden")] get; set; }

		[NoTV]
		[Export ("setToolbarHidden:animated:")]
		void SetToolbarHidden (bool hidden, bool animated);

		[NoTV]
		[Export ("toolbar")]
		UIToolbar Toolbar { get; }
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UINavigationControllerDelegate Delegate { get; set; }

		[Field ("UINavigationControllerHideShowBarDuration")]
		nfloat HideShowBarDuration { get; }

		[NoTV]
		[iOS (7,0)]
		[Export ("interactivePopGestureRecognizer", ArgumentSemantic.Copy)]
		UIGestureRecognizer InteractivePopGestureRecognizer { get; }

		[NoTV]
		[iOS (8,0)]
		[Export ("hidesBarsWhenVerticallyCompact", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsWhenVerticallyCompact { get; set; }
		
		[NoTV]
		[iOS (8,0)]
		[Export ("hidesBarsOnTap", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsOnTap { get; set; }
		
		[iOS (8,0)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[NoTV]
		[iOS (8,0)]
		[Export ("hidesBarsWhenKeyboardAppears", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsWhenKeyboardAppears { get; set; }

		[NoTV]
		[iOS (8,0)]
		[Export ("hidesBarsOnSwipe", ArgumentSemantic.UnsafeUnretained)]
		bool HidesBarsOnSwipe { get; set; }
		
		[NoTV]
		[iOS (8,0)]
		[Export ("barHideOnSwipeGestureRecognizer", ArgumentSemantic.Retain)]
		UIPanGestureRecognizer BarHideOnSwipeGestureRecognizer { get; }

		[NoTV]
		[iOS (8,0)]
		[Export ("barHideOnTapGestureRecognizer", ArgumentSemantic.UnsafeUnretained)]
		UITapGestureRecognizer BarHideOnTapGestureRecognizer { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UINavigationControllerDelegate {

		[Export ("navigationController:willShowViewController:animated:"), EventArgs ("UINavigationController")]
		void WillShowViewController (UINavigationController navigationController, [Transient] UIViewController viewController, bool animated);

		[Export ("navigationController:didShowViewController:animated:"), EventArgs ("UINavigationController")]
		void DidShowViewController (UINavigationController navigationController, [Transient] UIViewController viewController, bool animated);

		[NoTV]
		[iOS (7,0)]
		[Export ("navigationControllerSupportedInterfaceOrientations:")]
		[NoDefaultValue]
		[DelegateName ("Func<UINavigationController,UIInterfaceOrientationMask>")]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UINavigationController navigationController);

		[NoTV]
		[iOS (7,0)]
		[Export ("navigationControllerPreferredInterfaceOrientationForPresentation:")]
		[DelegateName ("Func<UINavigationController,UIInterfaceOrientation>")]
		[NoDefaultValue]
		UIInterfaceOrientation GetPreferredInterfaceOrientation (UINavigationController navigationController);

		[iOS (7,0)]
		[Export ("navigationController:interactionControllerForAnimationController:")]
		[DelegateName ("Func<UINavigationController,IUIViewControllerAnimatedTransitioning,IUIViewControllerInteractiveTransitioning>")]
		[NoDefaultValue]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UINavigationController navigationController, IUIViewControllerAnimatedTransitioning animationController);

		[iOS (7,0)]
		[Export ("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
		[DelegateName ("Func<UINavigationController,UINavigationControllerOperation,UIViewController,UIViewController,IUIViewControllerAnimatedTransitioning>")]
		[NoDefaultValue]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation (UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController);
	}

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

	[BaseType (typeof (UIControl))]
	interface UIPageControl : UIAppearance {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("numberOfPages")]
		nint Pages { get; set; }

		[Export ("currentPage")]
		nint CurrentPage { get; set; }

		[Export ("hidesForSinglePage")]
		bool HidesForSinglePage { get; set; }

		[Export ("defersCurrentPageDisplay")]
		bool DefersCurrentPageDisplay { get; set; }

		[Export ("updateCurrentPageDisplay")]
		void UpdateCurrentPageDisplay ();

		[Export ("sizeForNumberOfPages:")]
		CGSize SizeForNumberOfPages (nint pageCount);

		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("pageIndicatorTintColor", ArgumentSemantic.Retain)]
		UIColor PageIndicatorTintColor { get; set;  }

		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("currentPageIndicatorTintColor", ArgumentSemantic.Retain)]
		UIColor CurrentPageIndicatorTintColor { get; set;  }
	}
	
	[BaseType (typeof (UIViewController),
		   Delegates = new string [] { "WeakDelegate", "WeakDataSource" },
		   Events = new Type [] { typeof (UIPageViewControllerDelegate), typeof (UIPageViewControllerDataSource)} )]
	interface UIPageViewController : NSCoding {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set;  }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPageViewControllerDelegate Delegate { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize]
		UIPageViewControllerDataSource DataSource { get; set;  }

		[Export ("transitionStyle")]
		UIPageViewControllerTransitionStyle TransitionStyle { get;  }

		[Export ("navigationOrientation")]
		UIPageViewControllerNavigationOrientation NavigationOrientation { get;  }

		[Export ("spineLocation")]
		UIPageViewControllerSpineLocation SpineLocation { get;  }

		[Export ("doubleSided")]
		bool DoubleSided { [Bind ("isDoubleSided")] get; set;  }

		[Export ("gestureRecognizers")]
		UIGestureRecognizer [] GestureRecognizers { get;  }

		[Export ("viewControllers")]
		UIViewController [] ViewControllers { get;  }

		[DesignatedInitializer]
		[Export ("initWithTransitionStyle:navigationOrientation:options:")]
		IntPtr Constructor (UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation navigationOrientation, NSDictionary options);

		[Export ("setViewControllers:direction:animated:completion:")]
		[PostGet ("ViewControllers")]
		[Async]
		void SetViewControllers (UIViewController [] viewControllers, UIPageViewControllerNavigationDirection direction, bool animated, [NullAllowed] UICompletionHandler completionHandler);

		[Field ("UIPageViewControllerOptionSpineLocationKey")]
		NSString OptionSpineLocationKey { get; }

		[iOS (6,0)]
		[Internal, Field ("UIPageViewControllerOptionInterPageSpacingKey")]
		NSString OptionInterPageSpacingKey { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPageViewControllerDelegate {
		[Export ("pageViewController:didFinishAnimating:previousViewControllers:transitionCompleted:"), EventArgs ("UIPageViewFinishedAnimation")]
		void DidFinishAnimating (UIPageViewController pageViewController, bool finished, UIViewController [] previousViewControllers, bool completed);

		[NoTV]
		[Export ("pageViewController:spineLocationForInterfaceOrientation:"), DelegateName ("UIPageViewSpineLocationCallback")]
		[DefaultValue(UIPageViewControllerSpineLocation.Mid)]
		UIPageViewControllerSpineLocation GetSpineLocation (UIPageViewController pageViewController, UIInterfaceOrientation orientation);

		[iOS (6,0)]
		[Export ("pageViewController:willTransitionToViewControllers:"), EventArgs ("UIPageViewControllerTransition")]
		void WillTransition (UIPageViewController pageViewController, UIViewController [] pendingViewControllers);

		[NoTV]
		[iOS (7,0)]
		[Export ("pageViewControllerSupportedInterfaceOrientations:")][DelegateName ("Func<UIPageViewController,UIInterfaceOrientationMask>")][DefaultValue (UIInterfaceOrientationMask.All)]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UIPageViewController pageViewController);

		[NoTV]
		[iOS (7,0)]
		[Export ("pageViewControllerPreferredInterfaceOrientationForPresentation:")][DelegateName ("Func<UIPageViewController,UIInterfaceOrientation>")][DefaultValue (UIInterfaceOrientation.Portrait)]
		UIInterfaceOrientation GetPreferredInterfaceOrientationForPresentation (UIPageViewController pageViewController);
	}

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

		[iOS (6,0)]
		[Export ("presentationCountForPageViewController:"), DelegateName ("UIPageViewGetNumber"), DefaultValue (1)]
		nint GetPresentationCount (UIPageViewController pageViewController);

		[iOS (6,0)]
		[Export ("presentationIndexForPageViewController:"), DelegateName ("UIPageViewGetNumber"), DefaultValue (1)]
		nint GetPresentationIndex (UIPageViewController pageViewController);
	}

	[NoTV]
	interface UIPasteboardChangeEventArgs {
		[Export ("UIPasteboardChangedTypesAddedKey")]
		string [] TypesAdded { get; }

		[Export ("UIPasteboardChangedTypesRemovedKey")]
		string [] TypesRemoved { get; }
	}
	
	[NoTV]
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: Calling -[UIPasteboard init] is not allowed.
	[DisableDefaultCtor]
	interface UIPasteboard {
		[Export ("generalPasteboard")][Static]
		UIPasteboard General { get; }

		[Export ("pasteboardWithName:create:")][Static]
		UIPasteboard FromName (string name, bool create);

		[Export ("pasteboardWithUniqueName")][Static]
		UIPasteboard GetUnique ();

		[Export ("name")]
		string Name { get; }

		[Export ("removePasteboardWithName:"), Static]
		void Remove (string name);
		
		[Export ("persistent")]
		bool Persistent { [Bind ("isPersistent")] get;
			[Deprecated (PlatformName.iOS, 10, 0)] set; }

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

#if XAMCORE_4_0
		[Export ("pasteboardTypesForItemSet:")]
		NSArray<NSString> [] GetPasteBoardTypes (NSIndexSet itemSet);
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

		[iOS (10,0), NoWatch]
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

		[iOS (10,0)]
		[Export ("setItems:options:")]
		void SetItems (NSDictionary<NSString, NSObject>[] items, NSDictionary options);
		
#if !TVOS
		[iOS (10,0)]
		[Wrap ("SetItems (items, pasteboardOptions?.Dictionary)")]
		void SetItems (NSDictionary<NSString, NSObject> [] items, UIPasteboardOptions pasteboardOptions);
#endif
		[NoWatch, NoTV, iOS (10, 0)]
		[Export ("hasStrings")]
		bool HasStrings { get; }

		[NoWatch, NoTV, iOS (10, 0)]
		[Export ("hasURLs")]
		bool HasUrls { get; }

		[NoWatch, NoTV, iOS (10, 0)]
		[Export ("hasImages")]
		bool HasImages { get; }
		
		[NoWatch, NoTV, iOS (10, 0)]
		[Export ("hasColors")]
		bool HasColors { get; }

		[NoWatch, NoTV, iOS (11,0)]
		[Export ("itemProviders", ArgumentSemantic.Copy)]
		NSItemProvider[] ItemProviders { get; set; }

		[NoWatch, NoTV, iOS (11,0)]
		[Export ("setItemProviders:localOnly:expirationDate:")]
		void SetItemProviders (NSItemProvider[] itemProviders, bool localOnly, [NullAllowed] NSDate expirationDate);

		[NoWatch, NoTV, iOS (11,0)]
		[Export ("setObjects:")]
		void SetObjects (INSItemProviderWriting[] objects);

		[NoWatch, NoTV, iOS (11,0)]
		[Export ("setObjects:localOnly:expirationDate:")]
		void SetObjects (INSItemProviderWriting[] objects, bool localOnly, [NullAllowed] NSDate expirationDate);
	}

	[NoTV]
	[Static]
	interface UIPasteboardNames {
		[Field ("UIPasteboardNameGeneral")]
		NSString General { get; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "The 'Find' pasteboard is no longer available.")]
		[Field ("UIPasteboardNameFind")]
		NSString Find { get; }
	}

#if !TVOS
	[NoWatch, NoTV, iOS (10, 0)]
	[StrongDictionary ("UIPasteboardOptionKeys")]
	interface UIPasteboardOptions {
		NSDate ExpirationDate { get; set; }
		bool LocalOnly { get; set; }
	}
#endif
	[NoWatch, NoTV, iOS (10,0)]
	[Static]
	interface UIPasteboardOptionKeys {
		[Field ("UIPasteboardOptionExpirationDate")]
		NSString ExpirationDateKey { get; }

		[Field ("UIPasteboardOptionLocalOnly")]
		NSString LocalOnlyKey { get; }
	}

	[NoTV]
	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" })]
	interface UIPickerView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NullAllowed] // by default this property is null
		[Export ("dataSource", ArgumentSemantic.Assign)]
#if XAMCORE_4_0
		IUIPickerViewDataSource DataSource { get; set; }
#else
		// should have been WeakDataSource
		NSObject DataSource { get; set; }
#endif

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPickerViewDelegate Delegate { get; set; }

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
#if XAMCORE_4_0
		nint RowsInSection (UITableView tableView, nint section);
#else
		nint RowsInSection (UITableView tableview, nint section);
#endif

		[Export ("tableView:cellForRowAtIndexPath:")]
		UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath);
	}

	[NoTV]
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
		UIView GetView (UIPickerView pickerView, nint row, nint component, UIView view);

		[Export ("pickerView:didSelectRow:inComponent:")]
		void Selected (UIPickerView pickerView, nint row, nint component);

		[iOS (6,0)]
		[Export ("pickerView:attributedTitleForRow:forComponent:")]
		NSAttributedString GetAttributedTitle (UIPickerView pickerView, nint row, nint component);
	}

	[NoTV]
	[Protocol, Model]
	[BaseType (typeof (UIPickerViewDelegate))]
	interface UIPickerViewAccessibilityDelegate {
		[Export ("pickerView:accessibilityLabelForComponent:")]
		string GetAccessibilityLabel (UIPickerView pickerView, nint acessibilityLabelForComponent);

		[Export ("pickerView:accessibilityHintForComponent:")]
		string GetAccessibilityHint (UIPickerView pickerView, nint component);

		[TV (11,0), iOS (11,0)]
		[Export ("pickerView:accessibilityAttributedLabelForComponent:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedLabel (UIPickerView pickerView, nint component);

		[TV (11,0), iOS (11,0)]
		[Export ("pickerView:accessibilityAttributedHintForComponent:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedHint (UIPickerView pickerView, nint component);
	}

	[NoTV]
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

	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (IsInformal = true)]
	interface UIPickerViewModel : UIPickerViewDataSource, UIPickerViewDelegate {
	}

	[iOS (8,0)]
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
		void ViewWillTransitionToSize (CGSize toSize, [NullAllowed] IUIViewControllerTransitionCoordinator coordinator);
		
		[Abstract]
		[Export ("willTransitionToTraitCollection:withTransitionCoordinator:")]
		void WillTransitionToTraitCollection (UITraitCollection traitCollection, [NullAllowed] IUIViewControllerTransitionCoordinator coordinator);
	}

	[iOS(8,0),Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIAppearanceContainer {
	}
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIPresentationController init].
	partial interface UIPresentationController : UIAppearanceContainer, UITraitEnvironment, UIContentContainer, UIFocusEnvironment {
		[Export ("initWithPresentedViewController:presentingViewController:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController);

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

		[Export ("overrideTraitCollection", ArgumentSemantic.Copy), NullAllowed]
		UITraitCollection OverrideTraitCollection { get; set; }

		[Export ("adaptivePresentationStyle")]
		UIModalPresentationStyle AdaptivePresentationStyle ();

		[iOS (8,3)]
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
	}

	delegate void UIPreviewHandler (UIPreviewAction action, UIViewController previewViewController);

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface UIPreviewAction : UIPreviewActionItem, NSCopying {
		[Static, Export ("actionWithTitle:style:handler:")]
		UIPreviewAction Create (string title, UIPreviewActionStyle style, UIPreviewHandler handler);

		[Export ("handler")]
		UIPreviewHandler Handler { get; }

		
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface UIPreviewActionGroup : UIPreviewActionItem, NSCopying {
		[Static, Export ("actionGroupWithTitle:style:actions:")]
		UIPreviewActionGroup Create (string title, UIPreviewActionStyle style, UIPreviewAction [] actions);
	}
	
	interface IUIPreviewActionItem {}
	
	[iOS (9,0)]
	[Protocol]
	interface UIPreviewActionItem {
		[Abstract]
		[Export ("title")]
		string Title { get; }
	}
	
	[BaseType (typeof (UIView))]
	interface UIProgressView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("initWithProgressViewStyle:")]
		IntPtr Constructor (UIProgressViewStyle style);

		[Export ("progressViewStyle")]
		UIProgressViewStyle Style { get; set; }

		[Export ("progress")]
		float Progress { get; set; } // This is float, not nfloat.

		[Export ("progressTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor ProgressTintColor { get; set;  }

		[Export ("trackTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor TrackTintColor { get; set;  }

		[Export ("progressImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ProgressImage { get; set;  }

		[Export ("trackImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage TrackImage { get; set;  }

		[Export ("setProgress:animated:")]
		void SetProgress (float progress /* this is float, not nfloat */, bool animated);

		[iOS (9,0)]
		[Export ("observedProgress")]
		[NullAllowed]
		NSProgress ObservedProgress { get; set; }
	}

	[iOS (7,0)]
	[BaseType (typeof (UIDynamicBehavior))]
	partial interface UIPushBehavior {
		[DesignatedInitializer]
		[Export ("initWithItems:mode:")]
		IntPtr Constructor (IUIDynamicItem [] items, UIPushBehaviorMode mode);
	
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

	[iOS (7,0)]
	[BaseType (typeof (UIDynamicBehavior))]
	[DisableDefaultCtor] // Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: init is undefined for objects of type UISnapBehavior
	partial interface UISnapBehavior {
		[DesignatedInitializer]
		[Export ("initWithItem:snapToPoint:")]
		IntPtr Constructor (IUIDynamicItem dynamicItem, CGPoint point);

		[Export ("damping", ArgumentSemantic.Assign)]
		nfloat Damping { get; set; }

		[iOS (9,0)]
		[Export ("snapPoint", ArgumentSemantic.Assign)]
		CGPoint SnapPoint { get; set; }
	}

	[NoTV]
	[BaseType (typeof (UIViewController))]
	// iOS6 returns the following (confusing) message with the default .ctor:
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[UIReferenceLibraryViewController initWithNibName:bundle:] is not a valid initializer. You must call -[UIReferenceLibraryViewController initWithTerm:].
	[DisableDefaultCtor]
	partial interface UIReferenceLibraryViewController : NSCoding {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("dictionaryHasDefinitionForTerm:"), Static]
		bool DictionaryHasDefinitionForTerm (string term);

		[DesignatedInitializer]
		[Export ("initWithTerm:")]
		IntPtr Constructor (string term);
	}

	[BaseType (typeof (NSObject))]
	interface UIResponder : UIAccessibilityAction, UIAccessibilityFocus, UIUserActivityRestoring
#if !TVOS
	, UIAccessibilityDragging
#endif // !TVOS
#if IOS
	, UIPasteConfigurationSupporting
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

		[Export ("undoManager")]
		NSUndoManager UndoManager { get; }

		// 3.2

		[Export ("inputAccessoryView")]
		UIView InputAccessoryView { get; }

		[Export ("inputView")]
		UIView InputView { get; }

		[Export ("reloadInputViews")]
		void ReloadInputViews ();

		[Export ("remoteControlReceivedWithEvent:")]
		void RemoteControlReceived  ([NullAllowed] UIEvent theEvent);

		// From the informal protocol ( = category on NSObject) UIResponderStandardEditActions

		[Export ("cut:")]
		void Cut ([NullAllowed] NSObject sender);
		
		[Export ("copy:")]
		void Copy ([NullAllowed] NSObject sender);
		
		[Export ("paste:")]
		void Paste ([NullAllowed] NSObject sender);
		
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

		//
		// 6.0
		//

		[iOS (6,0)]
		[Export ("toggleBoldface:")]
		void ToggleBoldface ([NullAllowed] NSObject sender);

		[iOS (6,0)]
		[Export ("toggleItalics:")]
		void ToggleItalics ([NullAllowed] NSObject sender);

		[iOS (6,0)]
		[Export ("toggleUnderline:")]
		void ToggleUnderline ([NullAllowed] NSObject sender);

		//
		// 7.0
		//
		
		[iOS (7,0)]
		[Export ("keyCommands")]
		UIKeyCommand [] KeyCommands { get; }
		
		[iOS (7,0)]
		[Static, Export ("clearTextInputContextIdentifier:")]
		void ClearTextInputContextIdentifier (NSString identifier);
		
		[iOS (7,0)]
		[Export ("targetForAction:withSender:")]
		NSObject GetTargetForAction (Selector action, [NullAllowed] NSObject sender);
		
		[iOS (7,0)]
		[Export ("textInputContextIdentifier")]
		NSString TextInputContextIdentifier { get; }
		
		[iOS (7,0)]
		[Export ("textInputMode")]
		UITextInputMode TextInputMode { get; }

		[iOS (8,0)]
		[Export ("inputViewController")]
		UIInputViewController InputViewController { get; }

		[iOS (8,0)]
		[Export ("inputAccessoryViewController")]
		UIInputViewController InputAccessoryViewController { get; }

		[iOS (8,0)]
		[Export ("userActivity"), NullAllowed]
		NSUserActivity UserActivity { get; set; }

		[iOS (8,0)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity activity);

		[iOS (9,0)]
		[Export ("pressesBegan:withEvent:")]
		void PressesBegan (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesChanged:withEvent:")]
		void PressesChanged (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesEnded:withEvent:")]
		void PressesEnded (NSSet<UIPress> presses, UIPressesEvent evt);

		[iOS (9,0)]
		[Export ("pressesCancelled:withEvent:")]
		void PressesCancelled (NSSet<UIPress> presses, UIPressesEvent evt);

		// from UIResponderInputViewAdditions (UIResponder) - other members already inlined

		[NoTV]
		[iOS (9,0)]
		[Export ("inputAssistantItem", ArgumentSemantic.Strong)]
		UITextInputAssistantItem InputAssistantItem { get; }

		[iOS (9,1)]
		[Export ("touchesEstimatedPropertiesUpdated:")]
		void TouchesEstimatedPropertiesUpdated (NSSet touches);
	}
	
	[Category, BaseType (typeof (UIResponder))]
	interface UIResponder_NSObjectExtension {
		[Export ("decreaseSize:")]
		void DecreaseSize ([NullAllowed] NSObject sender);

		[Export ("increaseSize:")]
		void IncreaseSize ([NullAllowed] NSObject sender);
	}
	
	[BaseType (typeof (NSObject))]
	interface UIScreen : UITraitEnvironment {
		[Export ("bounds")]
		CGRect Bounds { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use the 'Bounds' property.")]
		[Export ("applicationFrame")]
		CGRect ApplicationFrame { get; }

		[Export ("mainScreen")][Static]
		UIScreen MainScreen { get; }

		[NoTV] // Xcode 7.2
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
		[Export ("preferredMode", ArgumentSemantic.Retain)]
		UIScreenMode PreferredMode { get; }

		[Export ("mirroredScreen", ArgumentSemantic.Retain)]
		UIScreen MirroredScreen { get; }
			
		[Export ("screens")][Static]
		UIScreen [] Screens { get; }

		[Export ("scale")]
		nfloat Scale { get; }

		[Export ("displayLinkWithTarget:selector:")]
		CoreAnimation.CADisplayLink CreateDisplayLink (NSObject target, Selector sel);

		[iOS (10,3), TV (10,2)]
		[Export ("maximumFramesPerSecond")]
		nint MaximumFramesPerSecond { get; }

		[NoTV]
		[Export ("brightness")]
		nfloat Brightness { get; set; }

		[NoTV]
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

		[iOS (11,0), TV (11,0)]
		[Field ("UIScreenCapturedDidChangeNotification")]
		[Notification]
		NSString CapturedDidChangeNotification { get; }

		[iOS (7,0)]
		[return: NullAllowed]
		[Export ("snapshotViewAfterScreenUpdates:")]
		UIView SnapshotView (bool afterScreenUpdates);

		[iOS (8,0)]
		[Export ("nativeBounds")]
		CGRect NativeBounds { get; }

		[iOS (8,0)]
		[Export ("nativeScale")]
		nfloat NativeScale { get; }

		[iOS (8,0)]
		[Export ("coordinateSpace")]
		IUICoordinateSpace CoordinateSpace { get; }

		[iOS (8,0)]
		[Export ("fixedCoordinateSpace")]
		IUICoordinateSpace FixedCoordinateSpace { get; }

		[iOS (9,0)]
		[Export ("overscanCompensationInsets")]
		UIEdgeInsets OverscanCompensationInsets { get; }

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[NullAllowed, Export ("focusedView", ArgumentSemantic.Weak)]
		UIView FocusedView { get; }

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("supportsFocus")]
		bool SupportsFocus { get; }

		[iOS (10, 0)]
		[NullAllowed, Export ("focusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem FocusedItem { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("captured")]
		bool Captured { [Bind ("isCaptured")] get; }
	}

	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIScrollViewDelegate)})]
	interface UIScrollView : UIFocusItemScrollableContainer {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		// moved to UIFocusItemScrollableContainer in iOS 12 - but that makes the availability information incorrect (so `new` is used to avoid compiler warnings)
		[Export ("contentOffset")]
		new CGPoint ContentOffset { get; set; }

		[Export ("contentSize")]
		new CGSize ContentSize { get; set; }

		[Export ("contentInset")]
		UIEdgeInsets ContentInset { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("adjustedContentInset")]
		UIEdgeInsets AdjustedContentInset { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("adjustedContentInsetDidChange")]
		[RequiresSuper]
		void AdjustedContentInsetDidChange ();

		[iOS (11,0), TV (11,0)]
		[Export ("contentInsetAdjustmentBehavior", ArgumentSemantic.Assign)]
		UIScrollViewContentInsetAdjustmentBehavior ContentInsetAdjustmentBehavior { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("contentLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide ContentLayoutGuide { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("frameLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide FrameLayoutGuide { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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

		[Export ("scrollIndicatorInsets")]
		UIEdgeInsets ScrollIndicatorInsets { get; set; } 

		[Export ("indicatorStyle")]
		UIScrollViewIndicatorStyle IndicatorStyle { get; set; }

		[Export ("decelerationRate")]
		nfloat DecelerationRate { get; set; }

		[iOS (10,3), TV (10,2), NoWatch]
		[Export ("indexDisplayMode")]
		UIScrollViewIndexDisplayMode IndexDisplayMode { get; set; }

		[NoTV]
		[Export ("pagingEnabled")]
		bool PagingEnabled { [Bind ("isPagingEnabled")] get; set; }
		
		[Export ("directionalLockEnabled")]
		bool DirectionalLockEnabled { [Bind ("isDirectionalLockEnabled")] get; set; }
		
		[Export ("scrollEnabled")]
		bool ScrollEnabled { [Bind ("isScrollEnabled")] get; set; }
		
		[Export ("tracking")]
		bool Tracking { [Bind("isTracking")] get; } 
		
		[Export ("dragging")]
		bool Dragging { [Bind ("isDragging")] get; }

		[Export ("decelerating")]
		bool Decelerating { [Bind ("isDecelerating")] get; }

		[Export ("setContentOffset:animated:")]
		void SetContentOffset (CGPoint contentOffset, bool animated);

		[Export ("scrollRectToVisible:animated:")]
		void ScrollRectToVisible (CGRect rect, bool animated);

		[Export ("flashScrollIndicators")]
		void  FlashScrollIndicators ();

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
		[Export ("scrollsToTop")]
		bool ScrollsToTop { get; set; } 

		[Export ("panGestureRecognizer")]
		UIPanGestureRecognizer PanGestureRecognizer { get; }

		[NoTV]
		[Export ("pinchGestureRecognizer")]
		UIPinchGestureRecognizer PinchGestureRecognizer { get; }
		
		[Field ("UIScrollViewDecelerationRateNormal")]
		nfloat DecelerationRateNormal { get; }

		[Field ("UIScrollViewDecelerationRateFast")]
		nfloat DecelerationRateFast { get; }

		[iOS (7,0)]
		[Export ("keyboardDismissMode")]
		UIScrollViewKeyboardDismissMode KeyboardDismissMode { get; set; }

		[NoWatch]
		[iOS (11,0)]
		[TV (9,0)]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Configuring the 'PanGestureRecognizer' for indirect scrolling automatically supports directional presses now, so this property is no longer useful.")]
		[Export ("directionalPressGestureRecognizer")]
		UIGestureRecognizer DirectionalPressGestureRecognizer { get; }

		[NoTV][iOS (10,0)]
		[NullAllowed, Export ("refreshControl", ArgumentSemantic.Strong)]
		UIRefreshControl RefreshControl { get; set; }
	}

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

		[iOS (11,0), TV (11,0)]
		[Export ("scrollViewDidChangeAdjustedContentInset:")]
		void DidChangeAdjustedContentInset (UIScrollView scrollView);
	}

	[Protocol, Model]
	[BaseType (typeof (UIScrollViewDelegate))]
	interface UIScrollViewAccessibilityDelegate {
		[Export ("accessibilityScrollStatusForScrollView:")]
		string GetAccessibilityScrollStatus (UIScrollView scrollView);

		[TV (11,0), iOS (11,0)]
		[Export ("accessibilityAttributedScrollStatusForScrollView:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedScrollStatus (UIScrollView scrollView);
	}

	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UISearchBarDelegate)})]
#if TVOS
	[DisableDefaultCtor] // - (instancetype)init __TVOS_PROHIBITED;
#endif
	interface UISearchBar : UIBarPositioning, UITextInputTraits
#if !TVOS
		, NSCoding
#endif
	{
		[NoTV]
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[NoTV]
		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISearchBarDelegate Delegate { get; set; }

		[Export ("text", ArgumentSemantic.Copy)][NullAllowed]
		string Text { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("prompt", ArgumentSemantic.Copy)]
		string Prompt { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("placeholder", ArgumentSemantic.Copy)]
		string Placeholder { get; set; }

		[NoTV]
		[Export ("showsBookmarkButton")]
		bool ShowsBookmarkButton { get; set; }

		[NoTV]
		[Export ("showsCancelButton")]
		bool ShowsCancelButton { get; set; }

		[Export ("selectedScopeButtonIndex")]
		nint SelectedScopeButtonIndex { get; set; }

		[Export ("showsScopeBar")]
		bool ShowsScopeBar { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("scopeButtonTitles", ArgumentSemantic.Copy)]
		string [] ScopeButtonTitles { get; set; }

		[Export ("translucent", ArgumentSemantic.Assign)]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		[NoTV]
		[Export ("setShowsCancelButton:animated:")]
		void SetShowsCancelButton (bool showsCancelButton, bool animated);

		// 3.2
		[NoTV]
		[Export ("searchResultsButtonSelected")]
		bool SearchResultsButtonSelected { [Bind ("isSearchResultsButtonSelected")] get; set; }

		[NoTV]
		[Export ("showsSearchResultsButton")]
		bool ShowsSearchResultsButton { get; set; }

		// 5.0
		[Export ("backgroundImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage BackgroundImage { get; set;  }

		[Export ("scopeBarBackgroundImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ScopeBarBackgroundImage { get; set;  }

		[Export ("searchFieldBackgroundPositionAdjustment")]
		UIOffset SearchFieldBackgroundPositionAdjustment { get; set;  }

		[Export ("searchTextPositionAdjustment")]
		UIOffset SearchTextPositionAdjustment { get; set;  }

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

		[Export ("setPositionAdjustment:forSearchBarIcon:")]
		void SetPositionAdjustmentforSearchBarIcon (UIOffset adjustment, UISearchBarIcon icon);

		[Export ("positionAdjustmentForSearchBarIcon:")]
		UIOffset GetPositionAdjustmentForSearchBarIcon (UISearchBarIcon icon);

		[iOS (6,0)]
		[Export ("inputAccessoryView", ArgumentSemantic.Retain)][NullAllowed]
		UIView InputAccessoryView { get; set; }

		[iOS (7,0)]
		[Appearance]
		[Export ("setBackgroundImage:forBarPosition:barMetrics:")]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarPosition barPosition, UIBarMetrics barMetrics);
	
		[iOS (7,0)]
		[Export ("backgroundImageForBarPosition:barMetrics:")]
		[Appearance]
		UIImage BackgroundImageForBarPosition (UIBarPosition barPosition, UIBarMetrics barMetrics);
	
#if !XAMCORE_2_0
		// documented in https://developer.apple.com/library/prerelease/ios/releasenotes/General/iOS70APIDiffs/
		// (but non-clickable) and NOT in the header files (or in the UISearchBar web documentation)
		// that makes them private API

		[iOS (7,0)]
		[Export ("setBackgroundImage:forBarMetrics:")]
		[Appearance]
		void SetBackgroundImage ([NullAllowed] UIImage backgroundImage, UIBarMetrics barMetrics);
	
		// note that this one "work" on 7.x (i.e. as a private API) but does not on iOS8
		[iOS (7,0)]
		[Export ("backgroundImageForBarMetrics:")]
		[Appearance]
		UIImage BackgroundImageForBarMetrics (UIBarMetrics barMetrics);
#endif

		[iOS (7,0), Export ("barTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor BarTintColor { get; set; }

		[iOS (7,0)]
		[Export ("searchBarStyle")]
		UISearchBarStyle SearchBarStyle { get; set; }

		[NoTV]
		[iOS (9,0)]
		[Export ("inputAssistantItem", ArgumentSemantic.Strong)]
		UITextInputAssistantItem InputAssistantItem { get; }
	}

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
		[Export ("searchBarBookmarkButtonClicked:"), EventArgs ("UISearchBar")]
		void BookmarkButtonClicked (UISearchBar searchBar);

		[NoTV]
		[Export ("searchBarCancelButtonClicked:"), EventArgs ("UISearchBar")]
		void CancelButtonClicked (UISearchBar searchBar);

		[Export ("searchBar:selectedScopeButtonIndexDidChange:"), EventArgs ("UISearchBarButtonIndex")]
		void SelectedScopeButtonIndexChanged (UISearchBar searchBar, nint selectedScope);

		[NoTV]

		[Export ("searchBarResultsListButtonClicked:"), EventArgs ("UISearchBar")]
		void ListButtonClicked (UISearchBar searchBar);
	}

	[iOS (9,1)][TV (9,0)]
	[BaseType (typeof(UIViewController))]
	interface UISearchContainerViewController
	{
		// inlined
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("searchController", ArgumentSemantic.Strong)]
		UISearchController SearchController { get; }
	
		[Export ("initWithSearchController:")]
		IntPtr Constructor (UISearchController searchController);
	}
	
	[iOS (8,0)]
	[BaseType (typeof (UIViewController))]
	partial interface UISearchController : UIViewControllerTransitioningDelegate, UIViewControllerAnimatedTransitioning {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("initWithSearchResultsController:")]
		IntPtr Constructor ([NullAllowed] UIViewController searchResultsController);
		
		[NullAllowed] // by default this property is null
		[Export ("searchResultsUpdater", ArgumentSemantic.UnsafeUnretained)]
		NSObject WeakSearchResultsUpdater { get; set; }

		[Wrap ("WeakSearchResultsUpdater")][Protocolize]
		UISearchResultsUpdating SearchResultsUpdater { get; set; }
		
		[Export ("active", ArgumentSemantic.UnsafeUnretained)]
		bool Active { [Bind ("isActive")] get; set; }
		
		[Export ("delegate", ArgumentSemantic.UnsafeUnretained), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISearchControllerDelegate Delegate { get; set; }

		[NoTV]
		[Export ("dimsBackgroundDuringPresentation", ArgumentSemantic.UnsafeUnretained)]
		bool DimsBackgroundDuringPresentation { get; set; }
	
		[Export ("hidesNavigationBarDuringPresentation", ArgumentSemantic.UnsafeUnretained)]
		bool HidesNavigationBarDuringPresentation { get; set; }
		
		[Export ("searchResultsController", ArgumentSemantic.Retain)]
		UIViewController SearchResultsController { get; }
		
		[Export ("searchBar", ArgumentSemantic.Retain)]
		UISearchBar SearchBar { get; }

		[iOS (9,1)]
		[Export ("obscuresBackgroundDuringPresentation")]
		bool ObscuresBackgroundDuringPresentation { get; set; }
		
	}

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
	}
		
	[BaseType (typeof (NSObject))]
	[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController'.")]
	[NoTV]
	interface UISearchDisplayController {
		[Export ("initWithSearchBar:contentsController:")]
		[PostGet ("SearchBar")]
		[PostGet ("SearchContentsController")]
		IntPtr Constructor (UISearchBar searchBar, UIViewController viewController);

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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

		[Export ("searchResultsDataSource", ArgumentSemantic.Assign)][NullAllowed]
		NSObject SearchResultsWeakDataSource { get; set; }

		[Wrap ("SearchResultsWeakDataSource")][Protocolize]
		UITableViewDataSource SearchResultsDataSource { get; set; }

		[Export ("searchResultsDelegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject SearchResultsWeakDelegate { get; set; }

		[Wrap ("SearchResultsWeakDelegate")]
		[Protocolize]
		UITableViewDelegate SearchResultsDelegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("searchResultsTitle", ArgumentSemantic.Copy)]
		string SearchResultsTitle { get; set;  }

		[iOS (7,0)]
		[Export ("displaysSearchBarInNavigationBar", ArgumentSemantic.Assign)]
		bool DisplaysSearchBarInNavigationBar { get; set; }

		[iOS (7,0)]
		[Export ("navigationItem")]
		UINavigationItem NavigationItem { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[NoTV]
	interface UISearchDisplayDelegate {
		
		[Export ("searchDisplayControllerWillBeginSearch:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void WillBeginSearch (UISearchDisplayController controller);
		
		[Export ("searchDisplayControllerDidBeginSearch:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void DidBeginSearch (UISearchDisplayController controller);
		
		[Export ("searchDisplayControllerWillEndSearch:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void WillEndSearch (UISearchDisplayController controller);
		
		[Export ("searchDisplayControllerDidEndSearch:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void DidEndSearch (UISearchDisplayController controller);
		
		[Export ("searchDisplayController:didLoadSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void DidLoadSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willUnloadSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void WillUnloadSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willShowSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void WillShowSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:didShowSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void DidShowSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:willHideSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void WillHideSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:didHideSearchResultsTableView:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		void DidHideSearchResults (UISearchDisplayController controller, UITableView tableView);

		[Export ("searchDisplayController:shouldReloadTableForSearchString:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		bool ShouldReloadForSearchString (UISearchDisplayController controller, string forSearchString);
		
		[Export ("searchDisplayController:shouldReloadTableForSearchScope:")]
		[Availability (Deprecated = Platform.iOS_8_0)]
		bool ShouldReloadForSearchScope (UISearchDisplayController controller, nint forSearchOption);
	}
	
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UISearchResultsUpdating {
#if XAMCORE_2_0
		[Abstract]
#endif
	    [Export ("updateSearchResultsForSearchController:")]
	    void UpdateSearchResultsForSearchController (UISearchController searchController);
	}
	
	[BaseType (typeof(UIControl))]
	interface UISegmentedControl
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithItems:")]
		IntPtr Constructor (NSArray items);

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("segmentedControlStyle")]
		[NoTV][NoWatch]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "The 'SegmentedControlStyle' property no longer has any effect.")]
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
		UIImage DividerImageForLeftSegmentStaterightSegmentStatebarMetrics (UIControlState leftState, UIControlState rightState, UIBarMetrics barMetrics);

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

	[NoTV]
	[BaseType (typeof(UIControl))]
	interface UISlider {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

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
	}
#endif // !WATCH

	[iOS (6,0)]
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

		[iOS (7,0)]
		[Field ("NSTextEffectAttributeName")]
		NSString TextEffect { get; }
		
		[iOS (7,0)]
		[Field ("NSAttachmentAttributeName")]
		NSString Attachment { get; }
		
		[iOS (7,0)]
		[Field ("NSLinkAttributeName")]
		NSString Link { get; }
		
		[iOS (7,0)]
		[Field ("NSBaselineOffsetAttributeName")]
		NSString BaselineOffset { get; }
		
		[iOS (7,0)]
		[Field ("NSUnderlineColorAttributeName")]
		NSString UnderlineColor { get; }
		
		[iOS (7,0)]
		[Field ("NSStrikethroughColorAttributeName")]
		NSString StrikethroughColor { get; }
		
		[iOS (7,0)]
		[Field ("NSObliquenessAttributeName")]
		NSString Obliqueness { get; }
		
		[iOS (7,0)]
		[Field ("NSExpansionAttributeName")]
		NSString Expansion { get; }
		
		[iOS (7,0)]
		[Field ("NSWritingDirectionAttributeName")]
		NSString WritingDirection { get; }
		
//
// These are internal, if we choose to expose these, we should
// put them on a better named class
//
		[iOS (7,0)]
		[Internal, Field ("NSTextEffectLetterpressStyle")]
		NSString NSTextEffectLetterpressStyle { get; }

	//
	// Document Types
	//
		[iOS (7,0)]
		[Internal, Field ("NSDocumentTypeDocumentAttribute")]
		NSString NSDocumentTypeDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSPlainTextDocumentType")]
		NSString NSPlainTextDocumentType { get; }
		
		[iOS (7,0)]
		[Internal, Field ("NSRTFDTextDocumentType")]
		NSString NSRTFDTextDocumentType { get; }
		
		[iOS (7,0)]
		[Internal, Field ("NSRTFTextDocumentType")]
		NSString NSRTFTextDocumentType { get; }

		[iOS (7,0)]
		[Internal, Field ("NSHTMLTextDocumentType")]
		NSString NSHTMLTextDocumentType { get; }

	//
	//
	//
		[iOS (7,0)]
		[Internal, Field ("NSCharacterEncodingDocumentAttribute")]
		NSString NSCharacterEncodingDocumentAttribute { get; }
		
		[iOS (7,0)]
		[Internal, Field ("NSDefaultAttributesDocumentAttribute")]
		NSString NSDefaultAttributesDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSPaperSizeDocumentAttribute")]
		NSString NSPaperSizeDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSPaperMarginDocumentAttribute")]
		NSString NSPaperMarginDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSViewSizeDocumentAttribute")]
		NSString NSViewSizeDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSViewZoomDocumentAttribute")]
		NSString NSViewZoomDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSViewModeDocumentAttribute")]
		NSString NSViewModeDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSReadOnlyDocumentAttribute")]
		NSString NSReadOnlyDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSBackgroundColorDocumentAttribute")]
		NSString NSBackgroundColorDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSHyphenationFactorDocumentAttribute")]
		NSString NSHyphenationFactorDocumentAttribute { get; }

		[iOS (7,0)]
		[Internal, Field ("NSDefaultTabIntervalDocumentAttribute")]
		NSString NSDefaultTabIntervalDocumentAttribute { get; }
	}
	
#if !WATCH
	[NoTV]
	[BaseType (typeof(UIControl))]
	interface UISwitch : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("on")]
		bool On { [Bind ("isOn")] get; set; }

		[Export ("setOn:animated:")]
		void SetState (bool newState, bool animated);


		[Export ("onTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor OnTintColor { get; set; }

		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("thumbTintColor", ArgumentSemantic.Retain)]
		UIColor ThumbTintColor { get; set;  }

		[iOS (6,0)]
		[Appearance]
		[Export ("onImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIImage OnImage { get; set;  }

		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("offImage", ArgumentSemantic.Retain)]
		UIImage OffImage { get; set;  }		
	}

	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UITabBarDelegate)})]
	interface UITabBar
#if IOS
		: UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")][NullAllowed]
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
		[Export ("beginCustomizingItems:")]
		void BeginCustomizingItems ([NullAllowed] UITabBarItem [] items);

		[NoTV]
		[Export ("endCustomizingAnimated:")]
#if XAMCORE_2_0
		bool EndCustomizing (bool animated);
#else
		bool EndCustomizingAnimated (bool animated);
#endif

		[NoTV]
		[Export ("isCustomizing")]
		bool IsCustomizing { get; }

		[NoTV]
		[Export ("selectedImageTintColor", ArgumentSemantic.Retain)]
		[Availability (Deprecated = Platform.iOS_8_0)]
		[NullAllowed]
		[Appearance]
		UIColor SelectedImageTintColor { get; set;  }

		[Export ("backgroundImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		[Appearance]
		UIImage BackgroundImage { get; set;  }

		[Export ("selectionIndicatorImage", ArgumentSemantic.Retain)]
		[NullAllowed]
		[Appearance]
		UIImage SelectionIndicatorImage { get; set;  }

		[iOS (6,0)]
		[Export ("shadowImage", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIImage ShadowImage { get; set; }

		[iOS (7,0)]
		[Export ("barTintColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed]
		UIColor BarTintColor { get; set; }

		[NoTV]
		[iOS (7,0)]
		[Export ("itemPositioning")]
		UITabBarItemPositioning ItemPositioning { get; set; }

		[iOS (7,0)]
		[Export ("itemWidth")]
		nfloat ItemWidth { get; set; }

		[iOS (7,0)]
		[Export ("itemSpacing")]
		nfloat ItemSpacing { get; set; }

		[NoTV]
		[iOS (7,0)]
		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[iOS (7,0)]
		[Export ("translucent")]
		bool Translucent { [Bind ("isTranslucent")] get; set; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("unselectedItemTintColor", ArgumentSemantic.Copy)]
		UIColor UnselectedItemTintColor { get; set; }
	}

	[BaseType (typeof (UIViewController), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UITabBarControllerDelegate)})]
	interface UITabBarController : UITabBarDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

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
		[Export ("moreNavigationController")]
		UINavigationController MoreNavigationController { get; }

		[NoTV]
		[Export ("customizableViewControllers", ArgumentSemantic.Copy)]
		UIViewController [] CustomizableViewControllers { get; [NullAllowed]  set; }

		[Export ("tabBar")]
		UITabBar TabBar { get; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UITabBarControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITabBarDelegate {
		[Export ("tabBar:didSelectItem:"), EventArgs ("UITabBarItem")]
		void ItemSelected (UITabBar tabbar, UITabBarItem item);

		[NoTV]
		[Export ("tabBar:willBeginCustomizingItems:"), EventArgs ("UITabBarItems")]
		void WillBeginCustomizingItems (UITabBar tabbar, UITabBarItem [] items);

		[NoTV]
		[Export ("tabBar:didBeginCustomizingItems:"), EventArgs ("UITabBarItems")]
		void DidBeginCustomizingItems (UITabBar tabbar, UITabBarItem [] items);

		[NoTV]
		[Export ("tabBar:willEndCustomizingItems:changed:"), EventArgs ("UITabBarFinalItems")]
		void WillEndCustomizingItems (UITabBar tabbar, UITabBarItem [] items, bool changed);

		[NoTV]
		[Export ("tabBar:didEndCustomizingItems:changed:"), EventArgs ("UITabBarFinalItems")]
		void DidEndCustomizingItems (UITabBar tabbar, UITabBarItem [] items, bool changed);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UITabBarControllerDelegate {

		[Export ("tabBarController:shouldSelectViewController:"), DefaultValue (true), DelegateName ("UITabBarSelection")]
		bool ShouldSelectViewController (UITabBarController tabBarController, UIViewController viewController);
		
		[Export ("tabBarController:didSelectViewController:"), EventArgs ("UITabBarSelection")]
		void ViewControllerSelected (UITabBarController tabBarController, UIViewController viewController);

		[NoTV]
		[Export ("tabBarController:willBeginCustomizingViewControllers:"), EventArgs ("UITabBarCustomize")]
		void OnCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers);

		[NoTV]
		[Export ("tabBarController:willEndCustomizingViewControllers:changed:"), EventArgs ("UITabBarCustomizeChange")]
		void OnEndCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers, bool changed);

		[NoTV]
		[Export ("tabBarController:didEndCustomizingViewControllers:changed:"), EventArgs ("UITabBarCustomizeChange")]
		void FinishedCustomizingViewControllers (UITabBarController tabBarController, UIViewController [] viewControllers, bool changed);

		[NoTV]
		[iOS (7,0), Export ("tabBarControllerSupportedInterfaceOrientations:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIInterfaceOrientationMask>")]
		UIInterfaceOrientationMask SupportedInterfaceOrientations  (UITabBarController tabBarController);
		
		[NoTV]
		[iOS (7,0), Export ("tabBarControllerPreferredInterfaceOrientationForPresentation:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIInterfaceOrientation>")]
		UIInterfaceOrientation GetPreferredInterfaceOrientation  (UITabBarController tabBarController);
		
		[iOS (7,0), Export ("tabBarController:interactionControllerForAnimationController:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,IUIViewControllerAnimatedTransitioning,IUIViewControllerInteractiveTransitioning>")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController (UITabBarController tabBarController,
													 IUIViewControllerAnimatedTransitioning animationController);
		
		[iOS (7,0), Export ("tabBarController:animationControllerForTransitionFromViewController:toViewController:")]
		[NoDefaultValue]
		[DelegateName ("Func<UITabBarController,UIViewController,UIViewController,IUIViewControllerAnimatedTransitioning>")]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForTransition (UITabBarController tabBarController,
											   UIViewController fromViewController,
											   UIViewController toViewController);
	}
	
	[BaseType (typeof (UIBarItem))]
	[DesignatedDefaultCtor]
	interface UITabBarItem : NSCoding
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("enabled")][Override]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("title", ArgumentSemantic.Copy)][Override]
		[NullAllowed]
		string Title { get;set; }

		[Export ("image", ArgumentSemantic.Retain)][Override]
		[NullAllowed]
		UIImage Image { get; set; }

		[Export ("imageInsets")][Override]
		UIEdgeInsets ImageInsets { get; set; }

		[Export ("tag")][Override]
		nint Tag { get; set; }

		[Export ("initWithTitle:image:tag:")]
		[PostGet ("Image")]
		IntPtr Constructor ([NullAllowed] string title, [NullAllowed] UIImage image, nint tag);

		[Export ("initWithTabBarSystemItem:tag:")]
		IntPtr Constructor (UITabBarSystemItem systemItem, nint tag);

		[Export ("badgeValue", ArgumentSemantic.Copy)][NullAllowed]
		string BadgeValue { get; set; } 

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use the '(string, UIImage, UIImage)' constructor or the 'Image' and 'SelectedImage' properties along with 'RenderingMode = UIImageRenderingMode.AlwaysOriginal'.")]
		[Export ("setFinishedSelectedImage:withFinishedUnselectedImage:")]
		[PostGet ("FinishedSelectedImage")]
		[PostGet ("FinishedUnselectedImage")]
		void SetFinishedImages ([NullAllowed] UIImage selectedImage, [NullAllowed] UIImage unselectedImage);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("finishedSelectedImage")]
		UIImage FinishedSelectedImage { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("finishedUnselectedImage")]
		UIImage FinishedUnselectedImage { get; }

		[Export ("titlePositionAdjustment")]
		[Appearance]
		UIOffset TitlePositionAdjustment { get; set; }

		[iOS (7,0)]
		[Export ("initWithTitle:image:selectedImage:")]
		[PostGet ("Image")]
		[PostGet ("SelectedImage")]
		IntPtr Constructor ([NullAllowed] string title, [NullAllowed] UIImage image, [NullAllowed] UIImage selectedImage);

		[iOS (7,0)]
		[Export ("selectedImage", ArgumentSemantic.Retain)][NullAllowed]
		UIImage SelectedImage { get; set; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("badgeColor", ArgumentSemantic.Copy)]
		UIColor BadgeColor { get; set; }

		[iOS (10,0), TV (10,0)]
		[Export ("setBadgeTextAttributes:forState:")]
		[Internal]
		void SetBadgeTextAttributes ([NullAllowed] NSDictionary textAttributes, UIControlState state);

		[iOS (10,0), TV (10,0)]
		[Wrap ("SetBadgeTextAttributes (textAttributes == null ? null : textAttributes.Dictionary, state)")]
		void SetBadgeTextAttributes (UIStringAttributes textAttributes, UIControlState state);

		[iOS (10,0), TV (10,0)]
		[Export ("badgeTextAttributesForState:")]
		[Internal]
		NSDictionary<NSString, NSObject> _GetBadgeTextAttributes (UIControlState state);

		[iOS (10,0), TV (10,0)]
		[Wrap ("new UIStringAttributes (_GetBadgeTextAttributes(state))")]
		UIStringAttributes GetBadgeTextAttributes (UIControlState state);
	}
	
	[BaseType (typeof(UIScrollView))]
	interface UITableView : NSCoding, UIDataSourceTranslating
#if IOS
		, UISpringLoadedInteractionSupporting
#endif
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithFrame:style:")]
		IntPtr Constructor (CGRect frame, UITableViewStyle style);

		[Export ("style")]
		UITableViewStyle Style { get; }
		
		[Export ("dataSource", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")]
		[Protocolize] 
		UITableViewDataSource DataSource { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][New][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")][New]
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
		NSIndexPath IndexPathForRowAtPoint (CGPoint point);
		
		[Export ("indexPathForCell:")]
		NSIndexPath IndexPathForCell (UITableViewCell cell);

		[Export ("indexPathsForRowsInRect:")][Internal]
		IntPtr _IndexPathsForRowsInRect (CGRect rect);

		[Export ("cellForRowAtIndexPath:")]
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
		
		[Export ("editing")]
		bool Editing { [Bind ("isEditing")] get; set; }
		
		[Export ("setEditing:animated:")]
		void SetEditing (bool editing, bool animated);

		[Export ("allowsSelection")]
		bool AllowsSelection { get; set; }

		[Export ("allowsSelectionDuringEditing")]
		bool AllowsSelectionDuringEditing { get; set; }

		[Export ("indexPathForSelectedRow")]
		NSIndexPath IndexPathForSelectedRow { get; }

		[Export ("selectRowAtIndexPath:animated:scrollPosition:")]
		void SelectRow ([NullAllowed] NSIndexPath indexPath, bool animated,  UITableViewScrollPosition scrollPosition);

		[Export ("deselectRowAtIndexPath:animated:")]
		void DeselectRow ([NullAllowed] NSIndexPath indexPath, bool animated);

		[Export ("sectionIndexMinimumDisplayRowCount")]
		nint SectionIndexMinimumDisplayRowCount { get; set; }

		[NoTV][NoWatch]
		[Export ("separatorStyle")]
		UITableViewCellSeparatorStyle SeparatorStyle { get; set; }

		[NoTV]
		[Export ("separatorColor", ArgumentSemantic.Retain)]
		[Appearance]
		[NullAllowed] // nullable (and spotted by introspection on iOS9)
		UIColor SeparatorColor { get; set; }

		[Export ("tableHeaderView", ArgumentSemantic.Retain), NullAllowed]
		UIView TableHeaderView { get; set; }

		[Export ("tableFooterView", ArgumentSemantic.Retain), NullAllowed]
		UIView TableFooterView { get; set; }

		[Export ("dequeueReusableCellWithIdentifier:")]
		UITableViewCell DequeueReusableCell (string identifier);

		[Export ("dequeueReusableCellWithIdentifier:")][Sealed]
		UITableViewCell DequeueReusableCell (NSString identifier);

		// 3.2
		[Export ("backgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView BackgroundView { get; set; }

		[NoTV]
		[Field ("UITableViewIndexSearch")]
		NSString IndexSearch { get; }

		[Field ("UITableViewAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[Export ("allowsMultipleSelection")]
                bool AllowsMultipleSelection { get; set;  }

                [Export ("allowsMultipleSelectionDuringEditing")]
                bool AllowsMultipleSelectionDuringEditing { get; set;  }

                [Export ("moveSection:toSection:")]
                void MoveSection (nint fromSection, nint toSection);

                [Export ("moveRowAtIndexPath:toIndexPath:")]
                void MoveRow (NSIndexPath fromIndexPath, NSIndexPath toIndexPath);

                [Export ("indexPathsForSelectedRows")]
                NSIndexPath [] IndexPathsForSelectedRows { get; }

		[Export ("registerNib:forCellReuseIdentifier:")]
#if XAMCORE_2_0
		void RegisterNibForCellReuse ([NullAllowed] UINib nib, NSString reuseIdentifier);
#else
		void RegisterNibForCellReuse ([NullAllowed] UINib nib, string reuseIdentifier);
#endif

		[Field ("UITableViewSelectionDidChangeNotification")]
		[Notification]
		NSString SelectionDidChangeNotification { get; }

		//
		// 6.0
		//
		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("sectionIndexColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexColor { get; set;  }

		[iOS (6,0)]
		[Appearance]
		[NullAllowed]
		[Export ("sectionIndexTrackingBackgroundColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexTrackingBackgroundColor { get; set;  }

		[iOS (6,0)]
		[Export ("headerViewForSection:")]
		UITableViewHeaderFooterView GetHeaderView (nint section);

		[iOS (6,0)]
		[Export ("footerViewForSection:")]
		UITableViewHeaderFooterView GetFooterView (nint section);

		[iOS (6,0)]
		[Export ("dequeueReusableCellWithIdentifier:forIndexPath:")]
		UITableViewCell DequeueReusableCell (NSString reuseIdentifier, NSIndexPath indexPath);

		[iOS (6,0)]
		[Export ("dequeueReusableHeaderFooterViewWithIdentifier:")]
		UITableViewHeaderFooterView DequeueReusableHeaderFooterView (NSString reuseIdentifier);

		[iOS (6,0)]
		[Export ("registerClass:forCellReuseIdentifier:"), Internal]
		void RegisterClassForCellReuse (IntPtr /*Class*/ cellClass, NSString reuseIdentifier);

		[iOS (6,0)]
		[Export ("registerNib:forHeaderFooterViewReuseIdentifier:")]
#if XAMCORE_2_0
		void RegisterNibForHeaderFooterViewReuse (UINib nib, NSString reuseIdentifier);
#else
		void RegisterNibForHeaderFooterViewReuse (UINib nib, string reuseIdentifier);
#endif

		[iOS (6,0)]
		[Export ("registerClass:forHeaderFooterViewReuseIdentifier:"), Internal]
		void RegisterClassForHeaderFooterViewReuse (IntPtr /*Class*/ aClass, NSString reuseIdentifier);

		//
		// 7.0
		//
	        [iOS (7,0)]
		[Export ("estimatedRowHeight", ArgumentSemantic.Assign)]
	        nfloat EstimatedRowHeight { get; set; }
	
	        [iOS (7,0)]
		[Export ("estimatedSectionHeaderHeight", ArgumentSemantic.Assign)]
	        nfloat EstimatedSectionHeaderHeight { get; set; }
		
	        [iOS (7,0)]
		[Export ("estimatedSectionFooterHeight", ArgumentSemantic.Assign)]
	        nfloat EstimatedSectionFooterHeight { get; set; }
		
		[iOS (7,0)]
		[Appearance]
		[NullAllowed] // by default this property is null
		[Export ("sectionIndexBackgroundColor", ArgumentSemantic.Retain)]
		UIColor SectionIndexBackgroundColor { get; set; }

		[iOS (7,0)]
		[Appearance]
		[Export ("separatorInset")]
		UIEdgeInsets SeparatorInset { get; set; }

		[NoTV]
		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("separatorEffect", ArgumentSemantic.Copy)]
		[Appearance]
		UIVisualEffect SeparatorEffect { get; set; }

		[iOS (9,0)]
		[Export ("cellLayoutMarginsFollowReadableWidth")]
		bool CellLayoutMarginsFollowReadableWidth { get; set; }

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("remembersLastFocusedIndexPath")]
		bool RemembersLastFocusedIndexPath { get; set; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("prefetchDataSource", ArgumentSemantic.Weak)]
		IUITableViewDataSourcePrefetching PrefetchDataSource { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("dragDelegate", ArgumentSemantic.Weak)]
		IUITableViewDragDelegate DragDelegate { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("dropDelegate", ArgumentSemantic.Weak)]
		IUITableViewDropDelegate DropDelegate { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Export ("separatorInsetReference", ArgumentSemantic.Assign)]
		UITableViewSeparatorInsetReference SeparatorInsetReference { get; set; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Async]
		[Export ("performBatchUpdates:completion:")]
		void PerformBatchUpdates ([NullAllowed] Action updates, [NullAllowed] Action<bool> completion);
		
		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Export ("hasUncommittedUpdates")]
		bool HasUncommittedUpdates { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("dragInteractionEnabled")]
		bool DragInteractionEnabled { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("hasActiveDrag")]
		bool HasActiveDrag { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("hasActiveDrop")]
		bool HasActiveDrop { get; }

		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Export ("insetsContentViewsToSafeArea")]
		bool InsetsContentViewsToSafeArea { get; set; }
	}

	interface IUITableViewDataSourcePrefetching {}
	[iOS (10,0)]
	[Protocol]
	interface UITableViewDataSourcePrefetching
	{
		[Abstract]
		[Export ("tableView:prefetchRowsAtIndexPaths:")]
		void PrefetchRows (UITableView tableView, NSIndexPath[] indexPaths);
	
		[Export ("tableView:cancelPrefetchingForRowsAtIndexPaths:")]
		void CancelPrefetching (UITableView tableView, NSIndexPath[] indexPaths);
	}
		
	//
	// This mixed both the UITableViewDataSource and UITableViewDelegate in a single class
	//
	[Model]
	[BaseType (typeof (UIScrollViewDelegate))]
	[Synthetic]
	interface UITableViewSource {
		[Export ("tableView:numberOfRowsInSection:")]
		[Abstract]
#if XAMCORE_4_0
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

		[TV (10,2)]
		[Export ("sectionIndexTitlesForTableView:")]
		string [] SectionIndexTitles (UITableView tableView);

		[TV (10,2)] // <- Header removed __TVOS_PROHIBITED;
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
		[Availability (Deprecated = Platform.iOS_3_0)]
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
		[Export ("tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:")]
		string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath);
		
		[Export ("tableView:shouldIndentWhileEditingRowAtIndexPath:")]
		bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath);
		
		[NoTV]
		[Export ("tableView:willBeginEditingRowAtIndexPath:")]
		void WillBeginEditing (UITableView tableView, NSIndexPath indexPath);
		
		[NoTV]
		[Export ("tableView:didEndEditingRowAtIndexPath:")]
		void DidEndEditing (UITableView tableView, [NullAllowed] NSIndexPath indexPath);
		
		[Export ("tableView:targetIndexPathForMoveFromRowAtIndexPath:toProposedIndexPath:")]
		NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath);
		
		[Export ("tableView:indentationLevelForRowAtIndexPath:")]
		nint IndentationLevel (UITableView tableView, NSIndexPath indexPath);

		// Copy Paste support
		
                [Export ("tableView:shouldShowMenuForRowAtIndexPath:")]
                bool ShouldShowMenu (UITableView tableView, NSIndexPath rowAtindexPath);

                [Export ("tableView:canPerformAction:forRowAtIndexPath:withSender:")]
                bool CanPerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, [NullAllowed] NSObject sender);

                [Export ("tableView:performAction:forRowAtIndexPath:withSender:")]
                void PerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, [NullAllowed] NSObject sender);
		
		[iOS (6,0)]
		[Export ("tableView:willDisplayHeaderView:forSection:")]
		void WillDisplayHeaderView (UITableView tableView, UIView headerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:willDisplayFooterView:forSection:")]
		void WillDisplayFooterView (UITableView tableView, UIView footerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingCell:forRowAtIndexPath:")]
		void CellDisplayingEnded (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingHeaderView:forSection:")]
		void HeaderViewDisplayingEnded (UITableView tableView, UIView headerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingFooterView:forSection:")]
		void FooterViewDisplayingEnded (UITableView tableView, UIView footerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:shouldHighlightRowAtIndexPath:")]
		bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath);

		[iOS (6,0)]
		[Export ("tableView:didHighlightRowAtIndexPath:")]
		void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[iOS (6,0)]
		[Export ("tableView:didUnhighlightRowAtIndexPath:")]
		void RowUnhighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForRowAtIndexPath:")]
		nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath);
		
		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForHeaderInSection:")]
		nfloat EstimatedHeightForHeader (UITableView tableView, nint section);
		
		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForFooterInSection:")]
		nfloat EstimatedHeightForFooter (UITableView tableView, nint section);

		[NoTV]
		[iOS (8,0)]
		[Export ("tableView:editActionsForRowAtIndexPath:")]
		UITableViewRowAction [] EditActionsForRow (UITableView tableView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("tableView:canFocusRowAtIndexPath:")]
		bool CanFocusRow (UITableView tableView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("tableView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context);

		[iOS (9,0)]
		[Export ("tableView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[iOS (9,0)]
		[Export ("indexPathForPreferredFocusedViewInTableView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UITableView tableView);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:leadingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:trailingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:shouldSpringLoadRowAtIndexPath:withContext:")]
		bool ShouldSpringLoadRow (UITableView tableView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);

		// WARNING: If you add more methods here, add them to UITableViewController as well.
	}
	
	[BaseType (typeof (UIView))]
	interface UITableViewCell : NSCoding, UIGestureRecognizerDelegate {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[DesignatedInitializer]
		[Export ("initWithStyle:reuseIdentifier:")]
		IntPtr Constructor (UITableViewCellStyle style, [NullAllowed] NSString reuseIdentifier);

		[Export ("imageView", ArgumentSemantic.Retain)]
		UIImageView ImageView { get; } 

		[Export ("textLabel", ArgumentSemantic.Retain)]
		UILabel TextLabel { get; }

		[Export ("detailTextLabel", ArgumentSemantic.Retain)]
		UILabel DetailTextLabel { get; }

		[Export ("contentView", ArgumentSemantic.Retain)]
		UIView ContentView { get; }

		[Export ("backgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView BackgroundView { get; set; }

		[Export ("selectedBackgroundView", ArgumentSemantic.Retain), NullAllowed]
		UIView SelectedBackgroundView { get; set; }

		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
#if XAMCORE_2_0
		NSString ReuseIdentifier { get; }
#else
		string ReuseIdentifier { get; }
#endif

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
		bool ShouldIndentWhileEditing  { get; set; }
		
		[Export ("accessoryType")]
		UITableViewCellAccessory Accessory { get; set; }

		[Export ("accessoryView", ArgumentSemantic.Retain)][NullAllowed]
		UIView AccessoryView { get; [NullAllowed] set; }
		
		[Export ("editingAccessoryType")]
		UITableViewCellAccessory EditingAccessory { get; set; }
		
		[Export ("editingAccessoryView", ArgumentSemantic.Retain)][NullAllowed]
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

		[NoTV]
		[iOS (7,0)]
		[Export ("separatorInset")]
		UIEdgeInsets SeparatorInset { get; set; }

		[iOS (9,0)] // introduced in Xcode 7.1 SDK (iOS 9.1 but hidden in 9.0)
		[Export ("focusStyle", ArgumentSemantic.Assign)]
		UITableViewCellFocusStyle FocusStyle { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("dragStateDidChange:")]
		void DragStateDidChange (UITableViewCellDragState dragState);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("userInteractionEnabledWhileDragging")]
		bool UserInteractionEnabledWhileDragging { get; set; }
	}

	[BaseType (typeof (UIViewController))]
	interface UITableViewController : UITableViewDataSource, UITableViewDelegate {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[DesignatedInitializer]
		[Export ("initWithStyle:")]
		IntPtr Constructor (UITableViewStyle withStyle);

		[Export ("tableView", ArgumentSemantic.Retain)]
		UITableView TableView { get; set; }

		[Export ("clearsSelectionOnViewWillAppear")]
		bool ClearsSelectionOnViewWillAppear { get; set; }

		[NoTV][iOS (6,0)]
		[NullAllowed, Export ("refreshControl", ArgumentSemantic.Strong)]
		UIRefreshControl RefreshControl { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model (AutoGeneratedName = true)]
	[Protocol]
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

		[TV (10,2)]
		[Export ("sectionIndexTitlesForTableView:")]
		string [] SectionIndexTitles (UITableView tableView);

		[TV (10,2)]
		[Export ("tableView:sectionForSectionIndexTitle:atIndex:")]
		nint SectionFor (UITableView tableView, string title, nint atIndex);

		[Export ("tableView:commitEditingStyle:forRowAtIndexPath:")]
		void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath);

		[Export ("tableView:moveRowAtIndexPath:toIndexPath:")]
		void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);
	}

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
		[Availability (Deprecated = Platform.iOS_3_0)]
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
		[Export ("tableView:titleForDeleteConfirmationButtonForRowAtIndexPath:")]
		string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath);
		
		[Export ("tableView:shouldIndentWhileEditingRowAtIndexPath:")]
		bool ShouldIndentWhileEditing (UITableView tableView, NSIndexPath indexPath);

		[NoTV]
		[Export ("tableView:willBeginEditingRowAtIndexPath:")]
		void WillBeginEditing (UITableView tableView, NSIndexPath indexPath);
		
		[NoTV]
		[Export ("tableView:didEndEditingRowAtIndexPath:")]
		void DidEndEditing (UITableView tableView, NSIndexPath indexPath);
		
		[Export ("tableView:targetIndexPathForMoveFromRowAtIndexPath:toProposedIndexPath:")]
		NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath);
		
		[Export ("tableView:indentationLevelForRowAtIndexPath:")]
		nint IndentationLevel (UITableView tableView, NSIndexPath indexPath);

		// Copy Paste support
		[Export ("tableView:shouldShowMenuForRowAtIndexPath:")]
		bool ShouldShowMenu (UITableView tableView, NSIndexPath rowAtindexPath);

		[Export ("tableView:canPerformAction:forRowAtIndexPath:withSender:")]
		bool CanPerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender);

		[Export ("tableView:performAction:forRowAtIndexPath:withSender:")]
		void PerformAction (UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender);

		[iOS (6,0)]
		[Export ("tableView:willDisplayHeaderView:forSection:")]
		void WillDisplayHeaderView (UITableView tableView, UIView headerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:willDisplayFooterView:forSection:")]
		void WillDisplayFooterView (UITableView tableView, UIView footerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingCell:forRowAtIndexPath:")]
		void CellDisplayingEnded (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingHeaderView:forSection:")]
		void HeaderViewDisplayingEnded (UITableView tableView, UIView headerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:didEndDisplayingFooterView:forSection:")]
		void FooterViewDisplayingEnded (UITableView tableView, UIView footerView, nint section);

		[iOS (6,0)]
		[Export ("tableView:shouldHighlightRowAtIndexPath:")]
		bool ShouldHighlightRow (UITableView tableView, NSIndexPath rowIndexPath);

		[iOS (6,0)]
		[Export ("tableView:didHighlightRowAtIndexPath:")]
		void RowHighlighted (UITableView tableView, NSIndexPath rowIndexPath);

		[iOS (6,0)]
		[Export ("tableView:didUnhighlightRowAtIndexPath:")]
		void RowUnhighlighted (UITableView tableView, NSIndexPath rowIndexPath);		

		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForRowAtIndexPath:")]
		nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath);
		
		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForHeaderInSection:")]
		nfloat EstimatedHeightForHeader (UITableView tableView, nint section);
		
		[iOS (7,0)]
		[Export ("tableView:estimatedHeightForFooterInSection:")]
		nfloat EstimatedHeightForFooter (UITableView tableView, nint section);

		[NoTV]
		[iOS (8,0)]
		[Export ("tableView:editActionsForRowAtIndexPath:")]
		UITableViewRowAction [] EditActionsForRow (UITableView tableView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("tableView:canFocusRowAtIndexPath:")]
		bool CanFocusRow (UITableView tableView, NSIndexPath indexPath);

		[iOS (9,0)]
		[Export ("tableView:shouldUpdateFocusInContext:")]
		bool ShouldUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context);

		[iOS (9,0)]
		[Export ("tableView:didUpdateFocusInContext:withAnimationCoordinator:")]
		void DidUpdateFocus (UITableView tableView, UITableViewFocusUpdateContext context, UIFocusAnimationCoordinator coordinator);

		[iOS (9,0)]
		[Export ("indexPathForPreferredFocusedViewInTableView:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPathForPreferredFocusedView (UITableView tableView);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:leadingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:trailingSwipeActionsConfigurationForRowAtIndexPath:")]
		[return: NullAllowed]
		UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration (UITableView tableView, NSIndexPath indexPath);

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("tableView:shouldSpringLoadRowAtIndexPath:withContext:")]
		bool ShouldSpringLoadRow (UITableView tableView, NSIndexPath indexPath, IUISpringLoadedInteractionContext context);
	}

	[iOS (6,0)]
	[BaseType (typeof (UIView))]
	interface UITableViewHeaderFooterView : UIAppearance, NSCoding {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("textLabel", ArgumentSemantic.Retain)]
		UILabel TextLabel { get;  }

		[Export ("detailTextLabel", ArgumentSemantic.Retain)]
		UILabel DetailTextLabel { get;  }

		[Export ("contentView", ArgumentSemantic.Retain)]
		UIView ContentView { get;  }

		[NullAllowed] // by default this property is null
		[Export ("backgroundView", ArgumentSemantic.Retain)]
		UIView BackgroundView { get; set;  }

		[Export ("reuseIdentifier", ArgumentSemantic.Copy)]
		NSString ReuseIdentifier { get;  }

		[DesignatedInitializer]
		[Export ("initWithReuseIdentifier:")]
		IntPtr Constructor (NSString reuseIdentifier);

		[RequiresSuper]
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

	}

	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
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
	
	[BaseType (typeof (UIControl), Delegates=new string [] { "WeakDelegate" })]
	// , Events=new Type [] {typeof(UITextFieldDelegate)})] custom logic needed, see https://bugzilla.xamarin.com/show_bug.cgi?id=53174
	interface UITextField : UITextInput, UIContentSizeCategoryAdjusting
#if IOS
	, UITextDraggable, UITextDroppable, UITextPasteConfigurationSupporting
#endif // IOS
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)]
		string Text { get; [NullAllowed] set; }

		[Export ("textColor", ArgumentSemantic.Retain)]
		UIColor TextColor { get; [NullAllowed] set; }

		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; [NullAllowed] set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("borderStyle")]
		UITextBorderStyle BorderStyle { get; set; }

		[Export ("placeholder", ArgumentSemantic.Copy)]
		string Placeholder { get; [NullAllowed] set; }

		[Export ("clearsOnBeginEditing")]
		bool ClearsOnBeginEditing { get; set; }

		[Export ("adjustsFontSizeToFitWidth")]
		bool AdjustsFontSizeToFitWidth { get; set; }

		[Export ("minimumFontSize")]
		nfloat MinimumFontSize { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; [NullAllowed] set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UITextFieldDelegate Delegate { get; [NullAllowed] set; }

		[Export ("background", ArgumentSemantic.Retain)]
		UIImage Background { get; [NullAllowed] set; }

		[Export ("disabledBackground", ArgumentSemantic.Retain)]
		UIImage DisabledBackground { get; [NullAllowed] set; }

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

		[Export ("drawTextInRect:")]
		void DrawText (CGRect rect);

		[Export ("drawPlaceholderInRect:")]
		void DrawPlaceholder (CGRect rect);

		// 3.2
		[Export ("inputAccessoryView", ArgumentSemantic.Retain)][NullAllowed]
		UIView InputAccessoryView { get; set; }

		[Export ("inputView", ArgumentSemantic.Retain)][NullAllowed]
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

		[iOS (10,0), TV (10,0)]
		[Field ("UITextFieldDidEndEditingReasonKey")]
		NSString DidEndEditingReasonKey { get; }

		//
		// 6.0
		//

		[iOS (6,0)]
		[NullAllowed] // by default this property is null (on 6.0, not later)
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set;  }

		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("attributedPlaceholder", ArgumentSemantic.Copy)]
		NSAttributedString AttributedPlaceholder { get; set;  }

		[iOS (6,0)]
		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set;  }

		[iOS (6,0)]
		[Export ("clearsOnInsertion")]
		bool ClearsOnInsertion { get; set;  }

		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("typingAttributes", ArgumentSemantic.Copy)]
		NSDictionary TypingAttributes { get; set; }

		[iOS (7,0)]
		[Export ("defaultTextAttributes", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary WeakDefaultTextAttributes { get; set; }
	}

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

		[iOS (10, 0)]
		[Export ("textFieldDidEndEditing:reason:"), EventArgs ("UITextFieldEditingEnded"), EventName ("EndedWithReason")]
		void EditingEnded (UITextField textField, UITextFieldDidEndEditingReason reason);
		
		[Export ("textFieldShouldClear:"), DelegateName ("UITextFieldCondition"), DefaultValue ("true")]
		bool ShouldClear (UITextField textField);
		
		[Export ("textFieldShouldReturn:"), DelegateName ("UITextFieldCondition"), DefaultValue ("true")]
		bool ShouldReturn (UITextField textField);

		[Export ("textField:shouldChangeCharactersInRange:replacementString:"), DelegateName ("UITextFieldChange"), DefaultValue ("true")]
		bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString);
	}
	
	[BaseType (typeof (UIScrollView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UITextViewDelegate)})]
	interface UITextView : UITextInput, NSCoding, UIContentSizeCategoryAdjusting
#if IOS
	, UITextDraggable, UITextDroppable, UITextPasteConfigurationSupporting
#endif // IOS
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("text", ArgumentSemantic.Copy)][NullAllowed]
		string Text { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("font", ArgumentSemantic.Retain)]
		UIFont Font { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textColor", ArgumentSemantic.Retain)]
		UIColor TextColor { get; set; }

		[Export ("editable")]
		[NoTV]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Export ("textAlignment")]
		UITextAlignment TextAlignment { get; set; }

		[Export ("selectedRange")]
		NSRange SelectedRange { get; set; }

		[Export ("scrollRangeToVisible:")]
		void ScrollRangeToVisible (NSRange range);

		[Wrap ("WeakDelegate")][New]
		[Protocolize]
		UITextViewDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][New][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("dataDetectorTypes")]
		[NoTV]
		UIDataDetectorType DataDetectorTypes { get; set; }

		// 3.2

		[NullAllowed] // by default this property is null
		[Export ("inputAccessoryView", ArgumentSemantic.Retain)]
		UIView InputAccessoryView { get; set; }


		[Export ("inputView", ArgumentSemantic.Retain)][NullAllowed]
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

		[iOS (6,0)]
		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set;  }

		[iOS (6,0)]
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set;  }

		[iOS (6,0)]
		[Export ("clearsOnInsertion")]
		bool ClearsOnInsertion { get; set;  }		

		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("typingAttributes", ArgumentSemantic.Copy)]
		NSDictionary TypingAttributes {
			// this avoids a crash (see unit tests) and behave like UITextField does (return null)
			[PreSnippet ("if (SelectedRange.Length == 0) return null;")]
			get;
			set;
		}
		
		[iOS (7,0)]
		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")] get; set; }

		[DesignatedInitializer]
		[iOS (7,0)]
		[Export ("initWithFrame:textContainer:")]
		[PostGet ("TextContainer")]
		IntPtr Constructor (CGRect frame, NSTextContainer textContainer);
	
		[iOS (7,0)]
		[Export ("textContainer", ArgumentSemantic.Copy)]
		NSTextContainer TextContainer { get; }
	
		[iOS (7,0)]
		[Export ("textContainerInset", ArgumentSemantic.Assign)]
		UIEdgeInsets TextContainerInset { get; set; }

		[iOS (7,0)]
		[Export ("layoutManager", ArgumentSemantic.Copy)]
		NSLayoutManager LayoutManager { get; }
	
		[iOS (7,0)]
		[Export ("textStorage", ArgumentSemantic.Retain)]
		NSTextStorage TextStorage { get; }
	
		[iOS (7,0)]
		[Export ("linkTextAttributes", ArgumentSemantic.Copy)]
		NSDictionary WeakLinkTextAttributes { get; set; }
	}

	[BaseType (typeof(UIScrollViewDelegate))]
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

		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use the 'ShouldInteractWithUrl' overload that takes 'UITextItemInteraction' instead.")]
		[Export ("textView:shouldInteractWithURL:inRange:"), DelegateName ("Func<UITextView,NSUrl,NSRange,bool>"), DefaultValue ("true")]
		bool ShouldInteractWithUrl (UITextView textView, NSUrl URL, NSRange characterRange);

		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use the 'ShouldInteractWithTextAttachment' overload that takes 'UITextItemInteraction' instead.")]
		[Export ("textView:shouldInteractWithTextAttachment:inRange:"), DelegateName ("Func<UITextView,NSTextAttachment,NSRange,bool>"), DefaultValue ("true")]
		bool ShouldInteractWithTextAttachment (UITextView textView, NSTextAttachment textAttachment, NSRange characterRange);

		[iOS (10, 0)]
		[Export ("textView:shouldInteractWithURL:inRange:interaction:"), DelegateApiName ("AllowUrlInteraction"), DelegateName ("UITextViewDelegateShouldInteractUrlDelegate"), DefaultValue ("true")]
		bool ShouldInteractWithUrl (UITextView textView, NSUrl url, NSRange characterRange, UITextItemInteraction interaction);

		[iOS (10,0)]
		[Export ("textView:shouldInteractWithTextAttachment:inRange:interaction:"), DelegateApiName ("AllowTextAttachmentInteraction"), DelegateName ("UITextViewDelegateShouldInteractTextDelegate"), DefaultValue ("true")]
		bool ShouldInteractWithTextAttachment (UITextView textView, NSTextAttachment textAttachment, NSRange characterRange, UITextItemInteraction interaction);
	}
	
	[NoTV]
	[BaseType (typeof (UIView))]
	interface UIToolbar : UIBarPositioning {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("barStyle")]
		UIBarStyle BarStyle { get; set; }

		[Export ("items", ArgumentSemantic.Copy)][NullAllowed]
		UIBarButtonItem [] Items { get; set; }

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

		[iOS (6,0)]
		[Appearance]
		[Export ("setShadowImage:forToolbarPosition:")]
		void SetShadowImage ([NullAllowed] UIImage shadowImage, UIToolbarPosition topOrBottom);

		[iOS (6,0)]
		[Appearance]
		[Export ("shadowImageForToolbarPosition:")]
		UIImage GetShadowImage (UIToolbarPosition topOrBottom);

		[iOS (7,0)]
		[Appearance]
		[NullAllowed]
		[Export ("barTintColor", ArgumentSemantic.Retain)]
		UIColor BarTintColor { get; set; }

		[iOS (7,0)]
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIToolbarDelegate Delegate { get; set; }
	}

	interface IUITimingCurveProvider {}

	[iOS (10,0)]
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

	[NoTV]
	[BaseType (typeof (UIBarPositioningDelegate))]
	[Model]
	[Protocol]
	interface UIToolbarDelegate {
	}
	
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
		UIGestureRecognizer[] GestureRecognizers { get; }

		[iOS (8,0)]
		[Export ("majorRadius")]
		nfloat MajorRadius { get; }

		[iOS (8,0)]
		[Export ("majorRadiusTolerance")]
		nfloat MajorRadiusTolerance { get; }

		[iOS (9,0)]
		[Export ("force")]
		nfloat Force { get; }

		[iOS (9,0)]
		[Export ("maximumPossibleForce")]
		nfloat MaximumPossibleForce { get; }

		[iOS (9,0)]
		[Export ("type")]
		UITouchType Type { get; }

		[NoTV]
		[iOS (9,1)]
		[Export ("preciseLocationInView:")]
		CGPoint GetPreciseLocation ([NullAllowed] UIView view);

		[NoTV]
		[iOS (9,1)]
		[Export ("precisePreviousLocationInView:")]
		CGPoint GetPrecisePreviousLocation ([NullAllowed] UIView view);

		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[Export ("azimuthAngleInView:")]
		nfloat GetAzimuthAngle ([NullAllowed] UIView view);

		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[Export ("azimuthUnitVectorInView:")]
		CGVector GetAzimuthUnitVector ([NullAllowed] UIView view);
		
		[NoTV] // stylus only, header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[Export ("altitudeAngle")]
		nfloat AltitudeAngle { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[NullAllowed, Export ("estimationUpdateIndex")]
		NSNumber EstimationUpdateIndex { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[Export ("estimatedProperties")]
		UITouchProperties EstimatedProperties { get; }

		[NoTV] // header unclear but not part of web documentation for tvOS
		[iOS (9,1)]
		[Export ("estimatedPropertiesExpectingUpdates")]
		UITouchProperties EstimatedPropertiesExpectingUpdates { get; }
	}

	[NoTV]
	[BaseType (typeof (UINavigationController), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIVideoEditorControllerDelegate)})]
	interface UIVideoEditorController {
		[Export ("canEditVideoAtPath:")][Static]
		bool CanEditVideoAtPath (string path);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		// id<UINavigationControllerDelegate, UIVideoEditorControllerDelegate>
		UIVideoEditorControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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
	[NoTV]
	[Model]
	[Protocol]
	interface UIVideoEditorControllerDelegate {
		[Export ("videoEditorController:didSaveEditedVideoToPath:"), EventArgs ("UIPath"), EventName ("Saved")]
		void VideoSaved (UIVideoEditorController editor, [EventName ("path")] string editedVideoPath);
	
		[Export ("videoEditorController:didFailWithError:"), EventArgs ("NSError", true)]
		void Failed (UIVideoEditorController editor, NSError  error);
	
		[Export ("videoEditorControllerDidCancel:")]
		void UserCancelled (UIVideoEditorController editor);
	}
		
	[BaseType (typeof (UIResponder))]
	interface UIView : UIAppearance, UIAppearanceContainer, UIAccessibility, UIDynamicItem, NSCoding, UIAccessibilityIdentification, UITraitEnvironment, UICoordinateSpace, UIFocusItem, CALayerDelegate, UIFocusItemContainer {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);
		
		[Export ("addSubview:")][PostGet ("Subviews")]
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

#if !XAMCORE_2_0
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawAtPoint:withFont:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGPoint, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGPoint point, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawAtPoint:forWidth:withFont:lineBreakMode:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawAtPoint:forWidth:withFont:fontSize:lineBreakMode:baselineAdjustment:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);
		
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawAtPoint:forWidth:withFont:minFontSize:actualFontSize:lineBreakMode:baselineAdjustment:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);
		
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawInRect:withFont:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGRect rect, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawInRect:withFont:lineBreakMode:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGRect rect, UIFont font, UILineBreakMode mode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("drawInRect:withFont:lineBreakMode:alignment:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.DrawString(CGRect, UIStringAttributes) instead")]
		[ThreadSafe]
		CGSize DrawString ([Target] string str, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment);

		[Bind ("endEditing:")]
		bool EndEditing (bool force);
		
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("sizeWithFont:")]
			//[Obsolete ("Deprecated in iOS7.  Use NSString.GetSizeUsingAttributes(UIStringAttributes) instead.")]
		[ThreadSafe]
		CGSize StringSize ([Target] string str, UIFont font);
		
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("sizeWithFont:forWidth:lineBreakMode:")]
		[ThreadSafe]
			//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		CGSize StringSize ([Target] string str, UIFont font, nfloat forWidth, UILineBreakMode breakMode);
					
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("sizeWithFont:constrainedToSize:")]
			//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		[ThreadSafe]
		CGSize StringSize ([Target] string str, UIFont font, CGSize constrainedToSize);
		
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("sizeWithFont:constrainedToSize:lineBreakMode:")]
			//[Obsolete ("Deprecated in iOS7.   Use NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext) instead.")]
		[ThreadSafe]
		CGSize StringSize ([Target] string str, UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2
		[Bind ("sizeWithFont:minFontSize:actualFontSize:forWidth:lineBreakMode:")]
		// Wait for guidance here: https://devforums.apple.com/thread/203655
			//[Obsolete ("Deprecated on iOS7.   No guidance.")]
		[ThreadSafe]
		CGSize StringSize ([Target] string str, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode);
#endif // !XAMCORE_2_0

		[Export ("userInteractionEnabled")]
		bool UserInteractionEnabled { [Bind ("isUserInteractionEnabled")]get; set; }

		[Export ("tag")]
		nint Tag { get;set; }

		[ThreadSafe]
		[Export ("layer", ArgumentSemantic.Retain)]
		CoreAnimation.CALayer Layer { get; }

		[Export ("frame")]
		new CGRect Frame { get; set; }
		
		[Export ("center")]
		new CGPoint Center { get; set; }
		
		[Export ("transform")]
		new CGAffineTransform Transform { get; set; }

		[NoTV]
		[Export ("multipleTouchEnabled")]
		bool MultipleTouchEnabled { [Bind ("isMultipleTouchEnabled")] get; set; }

		[NoTV]
		[Export ("exclusiveTouch")]
		bool ExclusiveTouch { [Bind ("isExclusiveTouch")] get; set; }

		[Export ("hitTest:withEvent:")]
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
		
		[Export ("insertSubview:atIndex:")][PostGet ("Subviews")]
		void InsertSubview (UIView view, nint atIndex);

		[Export ("exchangeSubviewAtIndex:withSubviewAtIndex:")]
		void ExchangeSubview (nint atIndex, nint withSubviewAtIndex);

		[Export ("insertSubview:belowSubview:")][PostGet ("Subviews")]
		void InsertSubviewBelow (UIView view, UIView siblingSubview);

		[Export ("insertSubview:aboveSubview:")][PostGet ("Subviews")]
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
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'CreateResizableImage' instead.")]
		CGRect ContentStretch { get; set; }

		[Static] [Export ("beginAnimations:context:")]
		void BeginAnimations ([NullAllowed] string animationID, IntPtr context);

		[Static] [Export ("commitAnimations")]
		void CommitAnimations ();

		[Static] [Export ("setAnimationDelegate:")]
		void SetAnimationDelegate (NSObject del);

		[Static] [Export ("setAnimationWillStartSelector:")]
		void SetAnimationWillStartSelector (Selector sel);
		
		[Static] [Export ("setAnimationDidStopSelector:")]
		void SetAnimationDidStopSelector (Selector sel);
		
		[Static] [Export ("setAnimationDuration:")]
		void SetAnimationDuration (double duration);

		[Static] [Export ("setAnimationDelay:")]
		void SetAnimationDelay (double delay);
		
		[Static] [Export ("setAnimationStartDate:")]
		void SetAnimationStartDate (NSDate startDate);
		
		[Static] [Export ("setAnimationCurve:")]
		void SetAnimationCurve (UIViewAnimationCurve curve);
		
		[Static] [Export ("setAnimationRepeatCount:")]
		void SetAnimationRepeatCount (float repeatCount /* This is float, not nfloat */);
		
		[Static] [Export ("setAnimationRepeatAutoreverses:")]
		void SetAnimationRepeatAutoreverses (bool repeatAutoreverses);
		
		[Static] [Export ("setAnimationBeginsFromCurrentState:")]
		void SetAnimationBeginsFromCurrentState (bool fromCurrentState);

		[Static] [Export ("setAnimationTransition:forView:cache:")]
		void SetAnimationTransition (UIViewAnimationTransition transition, UIView forView, bool cache);

		[Static] [Export ("areAnimationsEnabled")]
		bool AnimationsEnabled { [Bind ("areAnimationsEnabled")] get; [Bind ("setAnimationsEnabled:")] set; }

		// 3.2:
		[Export ("addGestureRecognizer:"), PostGet ("GestureRecognizers")]
		void AddGestureRecognizer (UIGestureRecognizer gestureRecognizer);

		[Export ("removeGestureRecognizer:"), PostGet ("GestureRecognizers")]
		void RemoveGestureRecognizer (UIGestureRecognizer gestureRecognizer);

		[NullAllowed] // by default this property is null
		[Export ("gestureRecognizers", ArgumentSemantic.Copy)]
		UIGestureRecognizer[] GestureRecognizers { get; set; }

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
		[Export ("viewPrintFormatter")]
		UIViewPrintFormatter ViewPrintFormatter { get; }

		[NoTV]
		[Export ("drawRect:forViewPrintFormatter:")]
		void DrawRect (CGRect area, UIViewPrintFormatter formatter);

		//
		// 6.0
		//
		
		[iOS (6,0)]
		[Export ("constraints")]
		NSLayoutConstraint [] Constraints { get; }

		[iOS (6,0)]
		[Export ("addConstraint:")]
		void AddConstraint (NSLayoutConstraint constraint);

		[iOS (6,0)]
		[Export ("addConstraints:")]
		void AddConstraints (NSLayoutConstraint [] constraints);

		[iOS (6,0)]
		[Export ("removeConstraint:")]
		void RemoveConstraint (NSLayoutConstraint constraint);

		[iOS (6,0)]
		[Export ("removeConstraints:")]
		void RemoveConstraints (NSLayoutConstraint [] constraints);

		[iOS (6,0)]
		[Export ("needsUpdateConstraints")]
		bool NeedsUpdateConstraints ();

		[iOS (6,0)]
		[Export ("setNeedsUpdateConstraints")]
		void SetNeedsUpdateConstraints ();

		[iOS (6,0)]
		[Static]
		[Export ("requiresConstraintBasedLayout")]
		bool RequiresConstraintBasedLayout ();

		[iOS (6,0)]
		[Export ("alignmentRectForFrame:")]
		CGRect AlignmentRectForFrame (CGRect frame);

		[iOS (6,0)]
		[Export ("frameForAlignmentRect:")]
		CGRect FrameForAlignmentRect (CGRect alignmentRect);

		[iOS (6,0)]
		[Export ("alignmentRectInsets")]
		UIEdgeInsets AlignmentRectInsets { get; }

		[NoTV]
		[iOS (6,0)]
		[Export ("viewForBaselineLayout")]
		[Availability (Deprecated = Platform.iOS_9_0, Message="Override 'ViewForFirstBaselineLayout' or 'ViewForLastBaselineLayout'.")]
		UIView ViewForBaselineLayout { get; }

		[iOS (9,0)]
		[Export ("viewForFirstBaselineLayout")]
		UIView ViewForFirstBaselineLayout { get; }

		[iOS (9,0)]
		[Export ("viewForLastBaselineLayout")]
		UIView ViewForLastBaselineLayout { get; }
			

		[iOS (6,0)]
		[Export ("intrinsicContentSize")]
		CGSize IntrinsicContentSize { get; }

		[iOS (6,0)]
		[Export ("invalidateIntrinsicContentSize")]
		void InvalidateIntrinsicContentSize ();

		[iOS (6,0)]
		[Export ("contentHuggingPriorityForAxis:")]
		float ContentHuggingPriority (UILayoutConstraintAxis axis); // This returns a float, not nfloat.

		[iOS (6,0)]
		[Export ("setContentHuggingPriority:forAxis:")]
		void SetContentHuggingPriority (float priority /* this is a float, not nfloat */, UILayoutConstraintAxis axis);

		[iOS (6,0)]
		[Export ("contentCompressionResistancePriorityForAxis:")]
		float ContentCompressionResistancePriority (UILayoutConstraintAxis axis); // This returns a float, not nfloat.

		[iOS (6,0)]
		[Export ("setContentCompressionResistancePriority:forAxis:")]
		void SetContentCompressionResistancePriority (float priority /* this is a float, not nfloat */, UILayoutConstraintAxis axis);

		[iOS (6,0)]
		[Export ("constraintsAffectingLayoutForAxis:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (UILayoutConstraintAxis axis);

		[iOS (6,0)]
		[Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[iOS (6,0)]
		[Export ("exerciseAmbiguityInLayout")]
		void ExerciseAmbiguityInLayout ();

		[iOS (6,0)]
		[Export ("systemLayoutSizeFittingSize:")]
		CGSize SystemLayoutSizeFittingSize (CGSize size);

		[iOS (6,0)]
		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[iOS (6,0)]
		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("restorationIdentifier", ArgumentSemantic.Copy)]
		string RestorationIdentifier { get; set; }
		
		[iOS (6,0)]
		[Export ("gestureRecognizerShouldBegin:")]
		bool GestureRecognizerShouldBegin (UIGestureRecognizer gestureRecognizer);

		[iOS (6,0)]
		[Export ("translatesAutoresizingMaskIntoConstraints")]
		bool TranslatesAutoresizingMaskIntoConstraints { get; set; }

		[iOS (6,0)]
		[Export ("updateConstraintsIfNeeded")]
		void UpdateConstraintsIfNeeded ();
		
		[iOS (6,0)]
		[Export ("updateConstraints")]
		[RequiresSuper]
		void UpdateConstraints ();

		[iOS (6,0)]
		[Field ("UIViewNoIntrinsicMetric")]
		nfloat NoIntrinsicMetric { get; }
		
		[iOS (6,0)]
		[Field ("UILayoutFittingCompressedSize")]
		CGSize UILayoutFittingCompressedSize { get; }

		[iOS (6,0)]
		[Field ("UILayoutFittingExpandedSize")]
		CGSize UILayoutFittingExpandedSize { get; }

		[NullAllowed]
		[Export ("tintColor")]
		[Appearance]
		[iOS (7,0)]
		UIColor TintColor { get; set; }

		[iOS (7,0)]
		[Export ("tintAdjustmentMode")]
		UIViewTintAdjustmentMode TintAdjustmentMode { get; set; }

		[iOS (7,0)]
		[Export ("tintColorDidChange")]
		void TintColorDidChange ();

		[iOS (7,0)]
		[Static, Export ("performWithoutAnimation:")]
		void PerformWithoutAnimation (Action actionsWithoutAnimation);

		[iOS (7,0)]
		[Static, Export ("performSystemAnimation:onViews:options:animations:completion:")]
		[Async]
		void PerformSystemAnimation (UISystemAnimation animation, UIView [] views, UIViewAnimationOptions options, Action parallelAnimations, UICompletionHandler completion);

		[iOS (7,0)]
		[Static, Export ("animateKeyframesWithDuration:delay:options:animations:completion:")]
		[Async]
		void AnimateKeyframes (double duration, double delay, UIViewKeyframeAnimationOptions options, Action animations, UICompletionHandler completion);

		[iOS (7,0)]
		[Static, Export ("addKeyframeWithRelativeStartTime:relativeDuration:animations:")]
		void AddKeyframeWithRelativeStartTime (double frameStartTime, double frameDuration, Action animations);

		[iOS (7,0)]
		[Export ("addMotionEffect:")]
		[PostGet ("MotionEffects")]
		void AddMotionEffect (UIMotionEffect effect);

		[iOS (7,0)]
		[Export ("removeMotionEffect:")]
		[PostGet ("MotionEffects")]
		void RemoveMotionEffect (UIMotionEffect effect);

		[iOS (7,0)]
		[NullAllowed] // by default this property is null
		[Export ("motionEffects", ArgumentSemantic.Copy)]
		UIMotionEffect [] MotionEffects { get; set; }

		[iOS (7,0)]
		[Export ("snapshotViewAfterScreenUpdates:")]
		UIView SnapshotView (bool afterScreenUpdates);

		[iOS (7,0)]
		[Export ("resizableSnapshotViewFromRect:afterScreenUpdates:withCapInsets:")]
		[return: NullAllowed]
		UIView ResizableSnapshotView (CGRect rect, bool afterScreenUpdates, UIEdgeInsets capInsets);

		[iOS (7,0)]
		[Export ("drawViewHierarchyInRect:afterScreenUpdates:")]
		bool DrawViewHierarchy (CGRect rect, bool afterScreenUpdates);

		[iOS (7,0)]
		[Static]
		[Export ("animateWithDuration:delay:usingSpringWithDamping:initialSpringVelocity:options:animations:completion:")]
		[Async]
		void AnimateNotify (double duration, double delay, nfloat springWithDampingRatio, nfloat initialSpringVelocity, UIViewAnimationOptions options, Action animations, [NullAllowed] UICompletionHandler completion);


		[iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("maskView", ArgumentSemantic.Retain)]
		UIView MaskView { get; set; }

		[iOS(8,0)]
		[Export ("systemLayoutSizeFittingSize:withHorizontalFittingPriority:verticalFittingPriority:")]
		// float, not CGFloat / nfloat, but we can't use an enum in the signature
		CGSize SystemLayoutSizeFittingSize (CGSize targetSize, /* UILayoutPriority */ float horizontalFittingPriority, /* UILayoutPriority */ float verticalFittingPriority);

		[iOS(8,0)]
		[Export ("layoutMargins")]
		UIEdgeInsets LayoutMargins { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("directionalLayoutMargins", ArgumentSemantic.Assign)]
		NSDirectionalEdgeInsets DirectionalLayoutMargins { get; set; }

		[iOS(8,0)]
		[Export ("preservesSuperviewLayoutMargins")]
		bool PreservesSuperviewLayoutMargins { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("insetsLayoutMarginsFromSafeArea")]
		bool InsetsLayoutMarginsFromSafeArea { get; set; }

		[iOS(8,0)]
		[Export ("layoutMarginsDidChange")]
		void LayoutMarginsDidChange ();

		[iOS (11,0), TV (11,0)]
		[Export ("safeAreaInsets")]
		UIEdgeInsets SafeAreaInsets { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("safeAreaInsetsDidChange")]
		void SafeAreaInsetsDidChange ();

		[iOS (9,0)]
		[Static]
		[Export ("userInterfaceLayoutDirectionForSemanticContentAttribute:")]
		UIUserInterfaceLayoutDirection GetUserInterfaceLayoutDirection (UISemanticContentAttribute attribute);

		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("userInterfaceLayoutDirectionForSemanticContentAttribute:relativeToLayoutDirection:")]
		UIUserInterfaceLayoutDirection GetUserInterfaceLayoutDirection (UISemanticContentAttribute semanticContentAttribute, UIUserInterfaceLayoutDirection layoutDirection);

		[iOS (10,0), TV (10,0)]
		[Export ("effectiveUserInterfaceLayoutDirection")]
		UIUserInterfaceLayoutDirection EffectiveUserInterfaceLayoutDirection { get; }

		[iOS (9,0)]
		[Export ("semanticContentAttribute", ArgumentSemantic.Assign)]
		UISemanticContentAttribute SemanticContentAttribute { get; set; }

		[iOS (9,0)]
		[Export ("layoutMarginsGuide", ArgumentSemantic.Strong)]
		UILayoutGuide LayoutMarginsGuide { get; }

		[iOS (9,0)]
		[Export ("readableContentGuide", ArgumentSemantic.Strong)]
		UILayoutGuide ReadableContentGuide { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("safeAreaLayoutGuide", ArgumentSemantic.Strong)]
		UILayoutGuide SafeAreaLayoutGuide { get; }

		[iOS (9,0)]
		[Export ("inheritedAnimationDuration")]
		[Static]
		double InheritedAnimationDuration { get; }

#if XAMCORE_2_0 // NSLayoutXAxisAnchor is a generic type, only supported in Unified (for now)
		[iOS (9,0)]
		[Export ("leadingAnchor")]
		NSLayoutXAxisAnchor LeadingAnchor { get; }
		
		[iOS (9,0)]
		[Export ("trailingAnchor")]
		NSLayoutXAxisAnchor TrailingAnchor { get; }
		
		[iOS (9,0)]
		[Export ("leftAnchor")]
		NSLayoutXAxisAnchor LeftAnchor { get; }
		
		[iOS (9,0)]
		[Export ("rightAnchor")]
		NSLayoutXAxisAnchor RightAnchor { get; }
		
		[iOS (9,0)]
		[Export ("topAnchor")]
		NSLayoutYAxisAnchor TopAnchor { get; }
		
		[iOS (9,0)]
		[Export ("bottomAnchor")]
		NSLayoutYAxisAnchor BottomAnchor { get; }
		
		[iOS (9,0)]
		[Export ("widthAnchor")]
		NSLayoutDimension WidthAnchor { get; }
		
		[iOS (9,0)]
		[Export ("heightAnchor")]
		NSLayoutDimension HeightAnchor { get; }
		
		[iOS (9,0)]
		[Export ("centerXAnchor")]
		NSLayoutXAxisAnchor CenterXAnchor { get; }
		
		[iOS (9,0)]
		[Export ("centerYAnchor")]
		NSLayoutYAxisAnchor CenterYAnchor { get; }
		
		[iOS (9,0)]
		[Export ("firstBaselineAnchor")]
		NSLayoutYAxisAnchor FirstBaselineAnchor { get; }
		
		[iOS (9,0)]
		[Export ("lastBaselineAnchor")]
		NSLayoutYAxisAnchor LastBaselineAnchor { get; }
#endif // XAMCORE_2_0

		[iOS (9,0)]
		[Export ("layoutGuides")]
		UILayoutGuide [] LayoutGuides { get; }
		
		[iOS (9,0)]
		[Export ("addLayoutGuide:")]
		void AddLayoutGuide (UILayoutGuide guide);
		
		[iOS (9,0)]
		[Export ("removeLayoutGuide:")]
		void RemoveLayoutGuide (UILayoutGuide guide);

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("focused")]
		bool Focused { [Bind ("isFocused")] get; }

		[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
		[Export ("canBecomeFocused")]
		new bool CanBecomeFocused { get; }

		[NoWatch, NoTV, iOS (11,0)]
		[Export ("addInteraction:")]
		void AddInteraction (IUIInteraction interaction);
	
		[NoWatch, NoTV, iOS (11,0)]
		[Export ("removeInteraction:")]
		void RemoveInteraction (IUIInteraction interaction);
	
		[NoWatch, NoTV, iOS (11, 0)]
		[Export ("interactions", ArgumentSemantic.Copy)]
		IUIInteraction[] Interactions { get; set; }

		// UIAccessibilityInvertColors category
		[NoWatch]
		[TV (11,0), iOS (11,0)]
		[Export ("accessibilityIgnoresInvertColors")]
		bool AccessibilityIgnoresInvertColors { get; set; }
	}

	[Category, BaseType (typeof (UIView))]
	interface UIView_UITextField {
		[Export ("endEditing:")]
		bool EndEditing (bool force);
	}

	[iOS (10,0), TV (10,0)]
	[Category]
	[BaseType (typeof (UILayoutGuide))]
	interface UILayoutGuide_UIConstraintBasedLayoutDebugging {

		[Export ("constraintsAffectingLayoutForAxis:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (UILayoutConstraintAxis axis);

		[Export ("hasAmbiguousLayout")]
		bool GetHasAmbiguousLayout ();
	}

	interface IUIContentContainer {}
	
	[BaseType (typeof (UIResponder))]
	interface UIViewController : NSCoding, UIAppearanceContainer, UIContentContainer, UITraitEnvironment, UIFocusEnvironment, NSExtensionRequestHandling {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);
		
		[Export ("view", ArgumentSemantic.Retain)]
		[NullAllowed]
		UIView View { get; set; }

		[Export ("loadView")]
		void LoadView ();

		[Export ("viewDidLoad")]
		void ViewDidLoad ();

		[NoTV]
		[Export ("viewDidUnload")]
		[Availability (Deprecated = Platform.iOS_6_0)]
		void ViewDidUnload ();

		[Export ("isViewLoaded")]
		bool IsViewLoaded { get; }
		
		[Export ("nibName", ArgumentSemantic.Copy)]
		string NibName { get; }

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
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'PresentViewController (UIViewController, bool, NSAction)' instead and set the 'ModalViewController' property to true.")]
		void PresentModalViewController (UIViewController modalViewController, bool animated);

		[NoTV]
		[Export ("dismissModalViewControllerAnimated:")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'DismissViewController (bool, NSAction)' instead.")]
#if XAMCORE_2_0
		void DismissModalViewController (bool animated);
#else
		void DismissModalViewControllerAnimated (bool animated);
#endif
		
		[NoTV]
		[Export ("modalViewController")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use 'PresentedViewController' instead.")]
		UIViewController ModalViewController { get; }

		[Export ("modalTransitionStyle", ArgumentSemantic.Assign)]
		UIModalTransitionStyle ModalTransitionStyle { get; set; }

		[NoTV]
		[Export ("wantsFullScreenLayout", ArgumentSemantic.Assign)]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'EdgesForExtendedLayout', 'ExtendedLayoutIncludesOpaqueBars' and 'AutomaticallyAdjustsScrollViewInsets' instead.")]
		bool WantsFullScreenLayout { get; set; }

		[Export ("parentViewController")][NullAllowed]
		UIViewController ParentViewController { get; }

		[Export ("tabBarItem", ArgumentSemantic.Retain)]
		UITabBarItem TabBarItem { get; set; }

		// UIViewControllerRotation category

		[NoTV]
		[Export ("shouldAutorotateToInterfaceOrientation:")]
		[Availability (Deprecated = Platform.iOS_6_0, Message = "Use both 'GetSupportedInterfaceOrientations' and 'PreferredInterfaceOrientationForPresentation' instead.")]
		bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation);

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		[Export ("rotatingHeaderView")]
		UIView RotatingHeaderView { get; }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		[Export ("rotatingFooterView")]
		UIView RotatingFooterView { get; }

		[NoTV]
		[Export ("interfaceOrientation")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		UIInterfaceOrientation InterfaceOrientation { get; }

		[NoTV]
		[Export ("willRotateToInterfaceOrientation:duration:")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration);
				 
		[NoTV]
		[Export ("didRotateFromInterfaceOrientation:")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		void DidRotate (UIInterfaceOrientation fromInterfaceOrientation);
		
		[NoTV]
		[Export ("willAnimateRotationToInterfaceOrientation:duration:")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration);

		[NoTV]
		[Export ("willAnimateFirstHalfOfRotationToInterfaceOrientation:duration:")]
		[Availability (Deprecated = Platform.iOS_5_0)]
		void WillAnimateFirstHalfOfRotation (UIInterfaceOrientation toInterfaceOrientation, double duration);

		[NoTV]
		[Export ("didAnimateFirstHalfOfRotationToInterfaceOrientation:")]
		[Availability (Deprecated = Platform.iOS_5_0)]
		void DidAnimateFirstHalfOfRotation (UIInterfaceOrientation toInterfaceOrientation);
		
		[NoTV]
		[Export ("willAnimateSecondHalfOfRotationFromInterfaceOrientation:duration:")]
		[Availability (Deprecated = Platform.iOS_5_0)]
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
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController' instead.")]
		[Export ("searchDisplayController", ArgumentSemantic.Retain)]
		UISearchDisplayController SearchDisplayController { get; }
		
		// These come from @interface UIViewController (UINavigationControllerItem)
		[Export ("navigationItem", ArgumentSemantic.Retain)]
		UINavigationItem NavigationItem  {get; }

		[NoTV]
		[Export ("hidesBottomBarWhenPushed")]
		bool HidesBottomBarWhenPushed { get; set; }

		[Export ("splitViewController", ArgumentSemantic.Retain)]
		UISplitViewController SplitViewController { get; }

		[Export ("tabBarController", ArgumentSemantic.Retain)]
		UITabBarController TabBarController { get; }

		[Export ("navigationController", ArgumentSemantic.Retain)]
		UINavigationController NavigationController { get; }

		// These come from @interface UIViewController (UINavigationControllerContextualToolbarItems)
		[Export ("toolbarItems", ArgumentSemantic.Retain)]
		UIBarButtonItem [] ToolbarItems { get; [NullAllowed] set; }

		[NoTV]
		[Export ("setToolbarItems:animated:")][PostGet ("ToolbarItems")]
		void SetToolbarItems ([NullAllowed] UIBarButtonItem [] items, bool animated);

		// These come in 3.2

		[Export ("modalPresentationStyle", ArgumentSemantic.Assign)]
		UIModalPresentationStyle ModalPresentationStyle { get; set; }

		// 3.2 extensions from MoviePlayer
		[NoMac]
		[NoTV]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Export ("presentMoviePlayerViewControllerAnimated:")]
		void PresentMoviePlayerViewController (MPMoviePlayerViewController moviePlayerViewController);

		[NoMac]
		[NoTV]
		[Availability (Deprecated = Platform.iOS_9_0, Message = "Use 'AVPlayerViewController' (AVKit) instead.")]
		[Export ("dismissMoviePlayerViewControllerAnimated")]
		void DismissMoviePlayerViewController ();


		// This is defined in a category in UIPopoverSupport.h: UIViewController (UIPopoverController)
		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'PreferredContentSize' instead.")]
		[Export ("contentSizeForViewInPopover")]
		CGSize ContentSizeForViewInPopover { get; set; }

		// This is defined in a category in UIPopoverSupport.h: UIViewController (UIPopoverController)
		[Export ("modalInPopover")]
		bool ModalInPopover { [Bind ("isModalInPopover")] get; set; }

		// It seems apple added a setter now but seems it is a mistake on new property radar:27929872
		[Export ("disablesAutomaticKeyboardDismissal")]
		bool DisablesAutomaticKeyboardDismissal { get; }

		[Export ("storyboard", ArgumentSemantic.Retain)]
		UIStoryboard Storyboard { get;  }

		[Export ("presentedViewController")]
		UIViewController PresentedViewController { get;  }

		[Export ("presentingViewController")]
		UIViewController PresentingViewController { get;  }

		[Export ("definesPresentationContext", ArgumentSemantic.Assign)]
		bool DefinesPresentationContext { get; set;  }

		[Export ("providesPresentationContextTransitionStyle", ArgumentSemantic.Assign)]
		bool ProvidesPresentationContextTransitionStyle { get; set;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0)]
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
		[Static]
		[Export ("attemptRotationToDeviceOrientation")]
		void AttemptRotationToDeviceOrientation ();

		[NoTV]
		[Availability (Deprecated = Platform.iOS_6_0)]
		[Export ("automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers")]
		/*PROTECTED*/ bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers { get; }

		[Export ("childViewControllers")]
		/*PROTECTED, MUSTCALLBASE*/ UIViewController [] ChildViewControllers { get;  }

		[Export ("addChildViewController:")]
		[PostGet ("ChildViewControllers")]
		/*PROTECTED, MUSTCALLBASE*/ void AddChildViewController (UIViewController childController);

		[Export ("removeFromParentViewController")]
		/*PROTECTED, MUSTCALLBASE*/ void RemoveFromParentViewController ();

		[Export ("transitionFromViewController:toViewController:duration:options:animations:completion:")]
		[Async]
		/*PROTECTED, MUSTCALLBASE*/ void Transition (UIViewController fromViewController, UIViewController toViewController, double duration, UIViewAnimationOptions options, /* non null */ Action animations, UICompletionHandler completionHandler);

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
		[iOS (6,0)]
		[Export ("shouldPerformSegueWithIdentifier:sender:")]
		bool ShouldPerformSegue (string segueIdentifier, NSObject sender);

		[iOS (6,0)]
		[Export ("canPerformUnwindSegueAction:fromViewController:withSender:")]
		bool CanPerformUnwind (Selector segueAction, UIViewController fromViewController, NSObject sender);

		[iOS (6,0)]
		[Export ("viewControllerForUnwindSegueAction:fromViewController:withSender:")]
		UIViewController GetViewControllerForUnwind (Selector segueAction, UIViewController fromViewController, NSObject sender);

		[iOS (6,0)]
		[Export ("segueForUnwindingToViewController:fromViewController:identifier:")]
		UIStoryboardSegue GetSegueForUnwinding (UIViewController toViewController, UIViewController fromViewController, string identifier);

		[NoTV]
		[iOS (6,0)]
		[Export ("supportedInterfaceOrientations")]
		UIInterfaceOrientationMask GetSupportedInterfaceOrientations ();

		[NoTV]
		[iOS (6,0)]
		[Export ("preferredInterfaceOrientationForPresentation")]
		UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ();

		[NoTV]
		[iOS (6,0)]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use Adaptive View Controllers instead.")]
		[Export ("shouldAutomaticallyForwardRotationMethods")]
		bool ShouldAutomaticallyForwardRotationMethods { get; }

		[iOS (6,0)]
		[Export ("shouldAutomaticallyForwardAppearanceMethods")]
		bool ShouldAutomaticallyForwardAppearanceMethods { get; }

		[iOS (6,0)]
		[NullAllowed] // by default this property is null
		[Export ("restorationIdentifier", ArgumentSemantic.Copy)]
		string RestorationIdentifier { get; set; }

		[NullAllowed]
		[iOS (6,0)]
		[Export ("restorationClass")]
		Class RestorationClass { get; set; }

		[iOS (6,0)]
		[Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[iOS (6,0)]
		[Export ("decodeRestorableStateWithCoder:")]
		void DecodeRestorableState (NSCoder coder);

		[iOS (6,0)]
		[Export ("updateViewConstraints")]
		void UpdateViewConstraints ();

		[NoTV]
		[iOS (6,0)]
		[Export ("shouldAutorotate")]
		bool ShouldAutorotate ();

		[iOS (7,0)]
		[Export ("edgesForExtendedLayout", ArgumentSemantic.Assign)]
		UIRectEdge EdgesForExtendedLayout { get; set; }
	
		[iOS (7,0)]
		[Export ("extendedLayoutIncludesOpaqueBars", ArgumentSemantic.Assign)]
		bool ExtendedLayoutIncludesOpaqueBars { get; set; }
	
		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIScrollView.ContentInsetAdjustmentBehavior' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIScrollView.ContentInsetAdjustmentBehavior' instead.")]
		[Export ("automaticallyAdjustsScrollViewInsets", ArgumentSemantic.Assign)]
		bool AutomaticallyAdjustsScrollViewInsets { get; set; }

		[iOS (7,0)]
		[Export ("preferredContentSize", ArgumentSemantic.Copy)]
		new CGSize PreferredContentSize { get; set; }

		[NoTV][NoWatch]
		[iOS (7,0)]
		[Export ("preferredStatusBarStyle")]
		UIStatusBarStyle PreferredStatusBarStyle ();

		[NoTV]
		[iOS (7,0)]
		[Export ("prefersStatusBarHidden")]
		bool PrefersStatusBarHidden ();

		[NoTV]
		[iOS (7,0)]
		[Export ("setNeedsStatusBarAppearanceUpdate")]
		void SetNeedsStatusBarAppearanceUpdate ();

		[iOS (7,0)]
		[Export ("applicationFinishedRestoringState")]
		void ApplicationFinishedRestoringState ();

		[iOS (7,0)]
		[Export ("transitioningDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakTransitioningDelegate { get; set; }

		[iOS (7,0)]
		[Wrap ("WeakTransitioningDelegate")]
		[Protocolize]
		UIViewControllerTransitioningDelegate TransitioningDelegate { get; set; }

		[NoTV]
		[iOS (7,0)]
		[Export ("childViewControllerForStatusBarStyle")]
		UIViewController ChildViewControllerForStatusBarStyle ();

		[NoTV]
		[iOS (7,0)]
		[Export ("childViewControllerForStatusBarHidden")]
		UIViewController ChildViewControllerForStatusBarHidden ();

		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.TopAnchor' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.TopAnchor' instead.")]
		[Export ("topLayoutGuide")]
		IUILayoutSupport TopLayoutGuide { get; }

		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.BottomAnchor' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'UIView.SafeAreaLayoutGuide.BottomAnchor' instead.")]
		[Export ("bottomLayoutGuide")]
		IUILayoutSupport BottomLayoutGuide { get; }
		
		[NoTV][NoWatch]
		[iOS (7,0)]
		[Export ("preferredStatusBarUpdateAnimation")]
		UIStatusBarAnimation PreferredStatusBarUpdateAnimation { get; }

		[NoTV]
		[iOS (7,0)]
		[Export ("modalPresentationCapturesStatusBarAppearance", ArgumentSemantic.Assign)]
		bool ModalPresentationCapturesStatusBarAppearance { get; set; }

		//
		// iOS 8
		//

		[iOS (8,0)]
		[Export ("targetViewControllerForAction:sender:")]
		UIViewController GetTargetViewControllerForAction (Selector action,  [NullAllowed] NSObject sender);
	
		[iOS (8,0)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc,  [NullAllowed] NSObject sender);
	
		[iOS (8,0)]
		[Export ("showDetailViewController:sender:")]
		void ShowDetailViewController (UIViewController vc, [NullAllowed] NSObject sender);

		[iOS (8,0)]
		[Export ("setOverrideTraitCollection:forChildViewController:")]
		void SetOverrideTraitCollection (UITraitCollection collection, UIViewController childViewController);
		
		[iOS (8,0)]
		[Export ("overrideTraitCollectionForChildViewController:")]
		UITraitCollection GetOverrideTraitCollectionForChildViewController (UIViewController childViewController);

		[iOS (8,0)]
		[Export ("extensionContext")]
		NSExtensionContext ExtensionContext { get; }

		[iOS (8,0)]
		[Export ("presentationController")]
		UIPresentationController PresentationController { get; }

		[NoTV]
		[iOS (8,0)]
		[Export ("popoverPresentationController")]
		UIPopoverPresentationController PopoverPresentationController { get; }

		[iOS (8,0)]
		[Field ("UIViewControllerShowDetailTargetDidChangeNotification")]
		[Notification]
		NSString ShowDetailTargetDidChangeNotification { get; }

		[iOS (9,0)]
		[Export ("loadViewIfNeeded")]
		void LoadViewIfNeeded ();

		[iOS (9,0)]
		[Export ("viewIfLoaded", ArgumentSemantic.Strong), NullAllowed]
		UIView ViewIfLoaded { get; }

		[iOS (9,0)]
		[Export ("allowedChildViewControllersForUnwindingFromSource:")]
		UIViewController[] GetAllowedChildViewControllersForUnwinding (UIStoryboardUnwindSegueSource segueSource);

		[iOS (9,0)]
		[Export ("childViewControllerContainingSegueSource:")]
		[return: NullAllowed]
		UIViewController GetChildViewControllerContainingSegueSource (UIStoryboardUnwindSegueSource segueSource);

		[iOS (9,0)]
		[Export ("unwindForSegue:towardsViewController:")]
		void Unwind (UIStoryboardSegue unwindSegue, UIViewController subsequentVC);
		
		[iOS (9,0)]
		[Export ("addKeyCommand:")]
		void AddKeyCommand (UIKeyCommand command);
		
		[iOS (9,0)]
		[Export ("removeKeyCommand:")]
		void RemoveKeyCommand (UIKeyCommand command);

		[iOS (9,0)]
		[Export ("registerForPreviewingWithDelegate:sourceView:")]
		IUIViewControllerPreviewing RegisterForPreviewingWithDelegate (IUIViewControllerPreviewingDelegate previewingDelegate, UIView sourceView);

		[iOS (9,0)]
		[Export ("unregisterForPreviewingWithContext:")]
		void UnregisterForPreviewingWithContext (IUIViewControllerPreviewing previewing);

		[iOS (9,0)]
		[Export ("previewActionItems")]
		IUIPreviewActionItem[] PreviewActionItems { get; }

		[Field ("UIViewControllerHierarchyInconsistencyException")]
		NSString HierarchyInconsistencyException { get; }

		[iOS (10,0), TV (10,0)]
		[Export ("restoresFocusAfterTransition")]
		bool RestoresFocusAfterTransition { get; set; }

		[NoWatch, NoiOS]
		[TV (11,0)]
		[Export ("preferredUserInterfaceStyle")]
		UIUserInterfaceStyle PreferredUserInterfaceStyle { get; }

		[NoWatch, NoiOS]
		[TV (11,0)]
		[Export ("setNeedsUserInterfaceAppearanceUpdate")]
		void SetNeedsUserInterfaceAppearanceUpdate ();

		[NoWatch, NoiOS]
		[TV (11, 0)]
		[NullAllowed, Export ("childViewControllerForUserInterfaceStyle")]
		UIViewController ChildViewControllerForUserInterfaceStyle { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("additionalSafeAreaInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets AdditionalSafeAreaInsets { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("systemMinimumLayoutMargins")]
		NSDirectionalEdgeInsets SystemMinimumLayoutMargins { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("viewRespectsSystemMinimumLayoutMargins")]
		bool ViewRespectsSystemMinimumLayoutMargins { get; set; }

		[iOS (11,0), TV (11,0)]
		[Export ("viewLayoutMarginsDidChange")]
		[RequiresSuper]
		void ViewLayoutMarginsDidChange ();

		[iOS (11,0), TV (11,0)]
		[Export ("viewSafeAreaInsetsDidChange")]
		[RequiresSuper]
		void ViewSafeAreaInsetsDidChange ();

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("childViewControllerForScreenEdgesDeferringSystemGestures")]
		UIViewController ChildViewControllerForScreenEdgesDeferringSystemGestures { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("preferredScreenEdgesDeferringSystemGestures")]
		UIRectEdge PreferredScreenEdgesDeferringSystemGestures { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("setNeedsUpdateOfScreenEdgesDeferringSystemGestures")]
		void SetNeedsUpdateOfScreenEdgesDeferringSystemGestures ();

		// UIHomeIndicatorAutoHidden (UIViewController) category

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("childViewControllerForHomeIndicatorAutoHidden")]
		UIViewController ChildViewControllerForHomeIndicatorAutoHidden { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("prefersHomeIndicatorAutoHidden")]
		bool PrefersHomeIndicatorAutoHidden { get; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("setNeedsUpdateOfHomeIndicatorAutoHidden")]
		void SetNeedsUpdateOfHomeIndicatorAutoHidden ();
	}

	[iOS (7,0)]
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

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("viewForKey:")]
		UIView GetViewFor (NSString uiTransitionContextToOrFromKey);

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("targetTransform")]
		CGAffineTransform TargetTransform { get; }


#if XAMCORE_4_0 // Can't break the world right now
		[Abstract]
#endif
		[iOS (10,0), TV (10,0)]
		[Export ("pauseInteractiveTransition")]
		void PauseInteractiveTransition ();
	}

	interface IUIViewControllerContextTransitioning {
	}

	interface IUITraitEnvironment {}
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[iOS (8,0)]
	partial interface UITraitEnvironment {
#if XAMCORE_2_0
		[Abstract]
#endif
		[iOS (8,0)]
		[Export ("traitCollection")]
		UITraitCollection TraitCollection { get; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[iOS (8,0)]
		[Export ("traitCollectionDidChange:")]
		void TraitCollectionDidChange ([NullAllowed] UITraitCollection previousTraitCollection);
	}
	
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface UITraitCollection : NSCopying, NSSecureCoding {
		[Export ("userInterfaceIdiom")]
		UIUserInterfaceIdiom UserInterfaceIdiom { get; }

		[TV (10, 0), NoWatch, iOS (12,0)]
		[Export ("userInterfaceStyle")]
		UIUserInterfaceStyle UserInterfaceStyle { get; }

		[Export ("displayScale")]
		nfloat DisplayScale { get; }

		[Export ("horizontalSizeClass")]
		UIUserInterfaceSizeClass HorizontalSizeClass { get; }

		[Export ("verticalSizeClass")]
		UIUserInterfaceSizeClass VerticalSizeClass { get; }

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

		[iOS (9,0)]
		[Static, Export ("traitCollectionWithForceTouchCapability:")]
		UITraitCollection FromForceTouchCapability (UIForceTouchCapability capability);

		[TV (10, 0), NoWatch, iOS (12,0)]
		[Static]
		[Export ("traitCollectionWithUserInterfaceStyle:")]
		UITraitCollection FromUserInterfaceStyle (UIUserInterfaceStyle userInterfaceStyle);

		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("traitCollectionWithDisplayGamut:")]
		UITraitCollection FromDisplayGamut (UIDisplayGamut displayGamut);
		
		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("traitCollectionWithLayoutDirection:")]
		UITraitCollection FromLayoutDirection (UITraitEnvironmentLayoutDirection layoutDirection);

		[iOS (10,0), TV (10,0)]
		[Static]
		[Export ("traitCollectionWithPreferredContentSizeCategory:")]
		[Internal]
		UITraitCollection FromPreferredContentSizeCategory (NSString preferredContentSizeCategory);
		
		[iOS (9,0)]
		[Export ("forceTouchCapability")]
		UIForceTouchCapability ForceTouchCapability { get; }

		[iOS (10,0), TV (10,0)]
		[Export ("displayGamut")]
		UIDisplayGamut DisplayGamut { get; }

		[iOS (10,0), TV (10,0)]
		[Export ("preferredContentSizeCategory")]
		string PreferredContentSizeCategory { get; }

		[iOS (10,0), TV (10,0)]
		[Export ("layoutDirection")]
		UITraitEnvironmentLayoutDirection LayoutDirection { get; }
	}
	
	[iOS (7,0)]
	[Static]
	partial interface UITransitionContext {
		[Field ("UITransitionContextFromViewControllerKey")]
		NSString FromViewControllerKey { get; }

		[Field ("UITransitionContextToViewControllerKey")]
		NSString ToViewControllerKey { get; }

		[iOS(8,0)]
		[Field ("UITransitionContextFromViewKey")]
		NSString FromViewKey { get; }

		[iOS (8,0)]
		[Field ("UITransitionContextToViewKey")]
		NSString ToViewKey { get; }
	}

	[iOS (7,0)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	partial interface UIViewControllerAnimatedTransitioning {
		[Abstract]
		[Export ("transitionDuration:")]
		double TransitionDuration (IUIViewControllerContextTransitioning transitionContext);

		[Abstract]
		[Export ("animateTransition:")]
		void AnimateTransition (IUIViewControllerContextTransitioning transitionContext);
		
		[iOS (10, 0)]
		[Export ("interruptibleAnimatorForTransition:")]
		IUIViewImplicitlyAnimating GetInterruptibleAnimator (IUIViewControllerContextTransitioning transitionContext);

		[Export ("animationEnded:")]
		void AnimationEnded (bool transitionCompleted);
	}
	interface IUIViewControllerAnimatedTransitioning {}

	[iOS (7,0)]
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

		[iOS (10,0), TV (10,0)]
		[Export ("wantsInteractiveStart")]
		bool WantsInteractiveStart { get; }
	}
	interface IUIViewControllerInteractiveTransitioning {}
			
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	partial interface UIViewControllerTransitioningDelegate {
		[Export ("animationControllerForPresentedController:presentingController:sourceController:")]
#if XAMCORE_2_0
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController (UIViewController presented, UIViewController presenting, UIViewController source);
#else
		IUIViewControllerAnimatedTransitioning PresentingController (UIViewController presented, UIViewController presenting, UIViewController source);
#endif
	
		[Export ("animationControllerForDismissedController:")]
		IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController (UIViewController dismissed);
	
		[Export ("interactionControllerForPresentation:")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation (IUIViewControllerAnimatedTransitioning animator);
	
		[Export ("interactionControllerForDismissal:")]
		IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal (IUIViewControllerAnimatedTransitioning animator);

		[iOS (8,0)]
		[Export ("presentationControllerForPresentedViewController:presentingViewController:sourceViewController:")]
		UIPresentationController GetPresentationControllerForPresentedViewController (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController, UIViewController sourceViewController);
	}
	
	[iOS (7,0)]
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

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("timingCurve", ArgumentSemantic.Strong)]
		IUITimingCurveProvider TimingCurve { get; set; }

		// getter comes from UIViewControllerInteractiveTransitioning but
		// headers declares a setter here
		[iOS (10,0), TV (10,0)]
		[Export ("wantsInteractiveStart")]
		new bool WantsInteractiveStart { get; set; }

		[iOS (10,0), TV (10,0)]
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
	[iOS (7,0)]
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

#if XAMCORE_2_0
		[Abstract]
#endif
		[iOS (8,0)]
		[Export ("targetTransform")]
		CGAffineTransform TargetTransform ();

#if XAMCORE_2_0
		[Abstract]
#endif
		[iOS (8,0)]
		[Export ("viewForKey:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)] // this is not the one we want to be seen (compat only)
		UIView GetTransitionViewControllerForKey (NSString key);

#if XAMCORE_4_0 // This is abstract in headers but is a breaking change
		[Abstract]
#endif
		[iOS (10, 0)]
		[Export ("isInterruptible")]
		bool IsInterruptible { get; }
	}
	interface IUIViewControllerTransitionCoordinatorContext {}

	//
	// This protocol is only for consumption (there is no API to set a transition coordinator,
	// only get an existing one), so we do not provide a model to subclass.
	//
	[iOS (7,0)]
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
		[Export ("notifyWhenInteractionEndsUsingBlock:")]
		void NotifyWhenInteractionEndsUsingBlock (Action<IUIViewControllerTransitionCoordinatorContext> handler);

#if XAMCORE_4_0 // This is abstract in headers but is a breaking change
		[Abstract]
#endif
		[iOS (10,0)]
		[Export ("notifyWhenInteractionChangesUsingBlock:")]
		void NotifyWhenInteractionChanges (Action<IUIViewControllerTransitionCoordinatorContext> handler);
	}
	interface IUIViewControllerTransitionCoordinator {}

	[Category, BaseType (typeof (UIViewController))]
	partial interface TransitionCoordinator_UIViewController {
		[Export ("transitionCoordinator")]
		IUIViewControllerTransitionCoordinator GetTransitionCoordinator ();
	}

	[NoTV]
	[Deprecated (PlatformName.iOS, 12, 0, message: "No longer supported; please adopt 'WKWebView'.")]
	[BaseType (typeof (UIView), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIWebViewDelegate)})]
	interface UIWebView : UIScrollViewDelegate {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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

		[iOS (6,0)]
		[Export ("suppressesIncrementalRendering")]
		bool SuppressesIncrementalRendering { get; set; }

		[iOS (6,0)]
		[Export ("keyboardDisplayRequiresUserAction")]
		bool KeyboardDisplayRequiresUserAction { get; set; }

		[iOS (7,0)]
		[Export ("paginationMode")]
		UIWebPaginationMode PaginationMode { get; set; }
	
		[iOS (7,0)]
		[Export ("paginationBreakingMode")]
		UIWebPaginationBreakingMode PaginationBreakingMode { get; set; }
	
		[iOS (7,0)]
		[Export ("pageLength")]
		nfloat PageLength { get; set; }
	
		[iOS (7,0)]
		[Export ("gapBetweenPages")]
		nfloat GapBetweenPages { get; set; }
	
		[iOS (7,0)]
		[Export ("pageCount")]
		nint PageCount { get; }

		[iOS (9,0)]
		[Export ("allowsPictureInPictureMediaPlayback")]
		bool AllowsPictureInPictureMediaPlayback { get; set; }

		[iOS (9,0), Mac(10,11)]
		[Export ("allowsLinkPreview")]
		bool AllowsLinkPreview { get; set; }
	}

	[NoTV]
	[Deprecated (PlatformName.iOS, 12, 0, message: "No longer supported; please adopt 'WKWebView' APIs.")]
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
	[iOS (10,0), TV (10,0)]
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

		[iOS (11,0), TV (11,0)]
		[Field ("UITextContentTypeUsername")]
		NSString Username { get; }

		[iOS (11,0), TV (11,0)]
		[Field ("UITextContentTypePassword")]
		NSString Password { get; }

		[TV (12, 0), iOS (12, 0)]
		[Field ("UITextContentTypeNewPassword")]
		NSString NewPassword { get; }

		[TV (12, 0), iOS (12, 0)]
		[Field ("UITextContentTypeOneTimeCode")]
		NSString OneTimeCode { get; }
	}
	
	[BaseType (typeof (UIViewController), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UISplitViewControllerDelegate)})]
	interface UISplitViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("viewControllers", ArgumentSemantic.Copy)]
		[PostGet ("ChildViewControllers")]
		UIViewController [] ViewControllers { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UISplitViewControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Export ("presentsWithGesture")]
		bool PresentsWithGesture { get; set; }

		//
		// iOS 8
		//
		[iOS (8,0)]
		[Export ("collapsed")]
		bool Collapsed { [Bind ("isCollapsed")] get; }
		
		[iOS (8,0)]
		[Export ("preferredDisplayMode")]
		UISplitViewControllerDisplayMode PreferredDisplayMode { get; set; }
		
		[iOS (8,0)]
		[Export ("displayMode")]
		UISplitViewControllerDisplayMode DisplayMode { get; }
		
		[iOS (8,0)]
		[Export ("preferredPrimaryColumnWidthFraction", ArgumentSemantic.UnsafeUnretained)]
		nfloat PreferredPrimaryColumnWidthFraction { get; set; }
		
		[iOS (8,0)]
		[Export ("minimumPrimaryColumnWidth", ArgumentSemantic.UnsafeUnretained)]
		nfloat MinimumPrimaryColumnWidth { get; set; }
		
		[iOS (8,0)]
		[Export ("maximumPrimaryColumnWidth", ArgumentSemantic.UnsafeUnretained)]
		nfloat MaximumPrimaryColumnWidth { get; set; }
		
		[iOS (8,0)]
		[Export ("primaryColumnWidth")]
		nfloat PrimaryColumnWidth { get; }
		
		[iOS (8,0)]
		[Export ("displayModeButtonItem")]
		UIBarButtonItem DisplayModeButtonItem { get; }
		
		[iOS (8,0)]
		[Export ("showViewController:sender:")]
		void ShowViewController (UIViewController vc,  [NullAllowed] NSObject sender);
		
		[iOS (8,0)]
		[Export ("showDetailViewController:sender:")]
		void ShowDetailViewController (UIViewController vc,  [NullAllowed] NSObject sender);

		[iOS (8,0)]
		[Field ("UISplitViewControllerAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("primaryEdge", ArgumentSemantic.Assign)]
		UISplitViewControllerPrimaryEdge PrimaryEdge { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UISplitViewControllerDelegate {
		[NoTV]
		[iOS (7,0)]  // While introduced in 7.0, it was not made public, it was only publicized in iOS 8 and made retroactively supported
		[Export ("splitViewControllerSupportedInterfaceOrientations:"), DelegateName("Func<UISplitViewController,UIInterfaceOrientationMask>"), DefaultValue(UIInterfaceOrientationMask.All)]
		UIInterfaceOrientationMask SupportedInterfaceOrientations (UISplitViewController splitViewController);
		
		[NoTV]
		[iOS (7,0)]  // While introduced in 7.0, it was not made public, it was only publicized in iOS 8 and made retroactively supported
		[Export ("splitViewControllerPreferredInterfaceOrientationForPresentation:"), DelegateName("Func<UISplitViewController,UIInterfaceOrientation>"), DefaultValue (UIInterfaceOrientation.Unknown)]
		UIInterfaceOrientation GetPreferredInterfaceOrientationForPresentation (UISplitViewController splitViewController);

		[NoTV]
		[Export ("splitViewController:popoverController:willPresentViewController:"), EventArgs ("UISplitViewPresent")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController' instead.")]
		void WillPresentViewController (UISplitViewController svc, UIPopoverController pc, UIViewController aViewController);

		[NoTV]
		[Export ("splitViewController:willHideViewController:withBarButtonItem:forPopoverController:"), EventArgs ("UISplitViewHide")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController' instead.")]
		void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc);

		[NoTV]
		[Export ("splitViewController:willShowViewController:invalidatingBarButtonItem:"), EventArgs ("UISplitViewShow")]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController' instead.")]
		void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button);

		[NoTV]
		[Export ("splitViewController:shouldHideViewController:inOrientation:"), DelegateName ("UISplitViewControllerHidePredicate"), DefaultValue (true)]
		[Availability (Deprecated = Platform.iOS_8_0, Message="Use 'UISearchController' instead.")]
		bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation);
	
		[iOS (8,0)]
		[Export ("splitViewController:willChangeToDisplayMode:"), EventArgs ("UISplitViewControllerDisplayMode")]
		void WillChangeDisplayMode (UISplitViewController svc, UISplitViewControllerDisplayMode displayMode);
		
		[iOS (8,0)]
		[Export ("targetDisplayModeForActionInSplitViewController:"), DelegateName("UISplitViewControllerFetchTargetForActionHandler"), DefaultValue(UISplitViewControllerDisplayMode.Automatic)]
		UISplitViewControllerDisplayMode GetTargetDisplayModeForAction (UISplitViewController svc);
		
		[iOS (8,0)]
		[Export ("splitViewController:showViewController:sender:"), DelegateName("UISplitViewControllerDisplayEvent"), DefaultValue(false)]
		bool EventShowViewController (UISplitViewController splitViewController, UIViewController vc, NSObject sender);
		
		[iOS (8,0)]
		[Export ("splitViewController:showDetailViewController:sender:"), DelegateName("UISplitViewControllerDisplayEvent"),DefaultValue(false)]
		bool EventShowDetailViewController (UISplitViewController splitViewController, UIViewController vc, NSObject sender);
		
		[iOS (8,0)]
		[Export ("primaryViewControllerForCollapsingSplitViewController:"), DelegateName("UISplitViewControllerGetViewController"), DefaultValue(null)]
		UIViewController GetPrimaryViewControllerForCollapsingSplitViewController (UISplitViewController splitViewController);
		
		[iOS (8,0)]
		[Export ("primaryViewControllerForExpandingSplitViewController:"), DelegateName("UISplitViewControllerGetViewController"), DefaultValue(null)]
		UIViewController GetPrimaryViewControllerForExpandingSplitViewController (UISplitViewController splitViewController);
		
		[iOS (8,0)]
		[Export ("splitViewController:collapseSecondaryViewController:ontoPrimaryViewController:"),DelegateName ("UISplitViewControllerCanCollapsePredicate"), DefaultValue (true)]
		bool CollapseSecondViewController (UISplitViewController splitViewController, UIViewController secondaryViewController, UIViewController primaryViewController);
		
		[iOS (8,0)]
		[Export ("splitViewController:separateSecondaryViewControllerFromPrimaryViewController:"), DelegateName("UISplitViewControllerGetSecondaryViewController"), DefaultValue(null)]
		UIViewController SeparateSecondaryViewController (UISplitViewController splitViewController, UIViewController primaryViewController);
	}

	[Category]
	[BaseType (typeof (UIViewController))]
	partial interface UISplitViewController_UIViewController {
		[iOS (8,0)]
		[Export ("splitViewController", ArgumentSemantic.Retain)]
		UISplitViewController GetSplitViewController ();
		
		[iOS (8,0)]
		[Export ("collapseSecondaryViewController:forSplitViewController:")]
		void CollapseSecondaryViewController (UIViewController secondaryViewController, UISplitViewController splitViewController);
		
		[iOS (8,0)]
		[Export ("separateSecondaryViewControllerForSplitViewController:")]
		UIViewController SeparateSecondaryViewControllerForSplitViewController (UISplitViewController splitViewController);
	}

	[NoTV]
	[BaseType (typeof (UIControl))]
	interface UIStepper {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")] get; set;  }

		[Export ("autorepeat")]
		bool AutoRepeat { get; set;  }

		[Export ("wraps")]
		bool Wraps { get; set;  }

		[Export ("value")]
		double Value { get; set;  }

		[Export ("minimumValue")]
		double MinimumValue { get; set;  }

		[Export ("maximumValue")]
		double MaximumValue { get; set;  }

		[Export ("stepValue")]
		double StepValue { get; set;  }

		//
		// 6.0
		//

		[iOS (6,0)]
		[Appearance]
		[Export ("setBackgroundImage:forState:")]
		void SetBackgroundImage ([NullAllowed] UIImage image, UIControlState state);

		[iOS (6,0)]
		[Appearance]
		[Export ("backgroundImageForState:")]
		UIImage BackgroundImage (UIControlState state);

		[iOS (6,0)]
		[Appearance]
		[Export ("setDividerImage:forLeftSegmentState:rightSegmentState:")]
		void SetDividerImage ([NullAllowed] UIImage image, UIControlState leftState, UIControlState rightState);

		[iOS (6,0)]
		[Appearance]
		[Export ("dividerImageForLeftSegmentState:rightSegmentState:")]
		UIImage GetDividerImage (UIControlState leftState, UIControlState rightState);

		[iOS (6,0)]
		[Appearance]
		[Export ("setIncrementImage:forState:")]
		void SetIncrementImage ([NullAllowed] UIImage image, UIControlState state);

		[iOS (6,0)]
		[Appearance]
		[Export ("incrementImageForState:")]
		UIImage GetIncrementImage (UIControlState state);

		[iOS (6,0)]
		[Appearance]
		[Export ("setDecrementImage:forState:")]
		void SetDecrementImage ([NullAllowed] UIImage image, UIControlState state);

		[iOS (6,0)]
		[Appearance]
		[Export ("decrementImageForState:")]
		UIImage GetDecrementImage (UIControlState state);
	}

	[BaseType (typeof (NSObject))]
	interface UIStoryboard {
		[Static]
		[Export ("storyboardWithName:bundle:")]
		UIStoryboard FromName (string name, [NullAllowed] NSBundle storyboardBundleOrNil);

		[Export ("instantiateInitialViewController")]
#if XAMCORE_2_0
		UIViewController InstantiateInitialViewController ();
#else
		NSObject InstantiateInitialViewController ();
#endif

		[Export ("instantiateViewControllerWithIdentifier:")]
#if XAMCORE_2_0
		UIViewController InstantiateViewController (string identifier);
#else
		NSObject InstantiateViewController (string identifier);
#endif
	}

	[Availability (Deprecated = Platform.iOS_9_0)]
	[DisableDefaultCtor] // as it subclass UIStoryboardSegue we end up with the same error
	[BaseType (typeof (UIStoryboardSegue))]
	interface UIStoryboardPopoverSegue {
		[Export ("initWithIdentifier:source:destination:"), PostGet ("SourceViewController"), PostGet ("DestinationViewController")]
		IntPtr Constructor ([NullAllowed] string identifier, UIViewController source, UIViewController destination);

		[Export ("popoverController", ArgumentSemantic.Retain)]
		UIPopoverController PopoverController { get;  }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: Don't call -[UIStoryboardSegue init]
	interface UIStoryboardSegue {
		[DesignatedInitializer]
		[Export ("initWithIdentifier:source:destination:"), PostGet ("SourceViewController"), PostGet ("DestinationViewController")]
		IntPtr Constructor ([NullAllowed] string identifier, UIViewController source, UIViewController destination);
		
		[Export ("identifier")]
		[NullAllowed]
		string Identifier { get;  }

		[Export ("sourceViewController")]
		UIViewController SourceViewController { get;  }

		[Export ("destinationViewController")]
		UIViewController DestinationViewController { get;  }

		[Export ("perform")]
		void Perform ();

		[iOS (6,0)]
		[Static]
		[Export ("segueWithIdentifier:source:destination:performHandler:")]
		UIStoryboardSegue Create ([NullAllowed] string identifier, UIViewController source, UIViewController destination, Action performHandler);
	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] 
	interface UIStoryboardUnwindSegueSource {
		[Export ("sourceViewController")]
		UIViewController SourceViewController { get; }

		// ideally we would not expose a `Selector` but this is created by iOS (not user code) and it's a getter only property
		[Export ("unwindAction")]
		Selector UnwindAction { get; }

		[Export ("sender")]
		NSObject Sender { get; }
	}
	
	[iOS (8,0)]
	[Protocol]
	interface UIPopoverBackgroundViewMethods {
		//
		// These must be overwritten by users, using the [Export ("...")] on the
		// static method
		//
		[Static, Export ("arrowHeight")]
		nfloat GetArrowHeight ();

		[Static, Export ("arrowBase")]
		nfloat GetArrowBase ();

		[Static, Export ("contentViewInsets")]
		UIEdgeInsets GetContentViewInsets ();
	}
	
	[BaseType (typeof (UIView))]
	interface UIPopoverBackgroundView : UIPopoverBackgroundViewMethods {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("arrowOffset")]
		nfloat ArrowOffset { get; set; }

#pragma warning disable 618
		[Export ("arrowDirection")]
		UIPopoverArrowDirection ArrowDirection { get; set; }
#pragma warning restore 618

		[iOS (6,0)]
		[Static, Export ("wantsDefaultContentAppearance")]
		bool WantsDefaultContentAppearance { get; }
	}
		
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIPopoverControllerDelegate)})]
	[DisableDefaultCtor] // bug #1786
	interface UIPopoverController : UIAppearanceContainer {
		[Export ("initWithContentViewController:")][PostGet ("ContentViewController")]
		IntPtr Constructor (UIViewController viewController);

		[Export ("contentViewController", ArgumentSemantic.Retain)]
		UIViewController ContentViewController { get; set; }

		[Export ("setContentViewController:animated:")][PostGet ("ContentViewController")]
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

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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
[Internal][Export ("popoverBackgroundViewClass", ArgumentSemantic.Retain)]
		IntPtr PopoverBackgroundViewClass { get; set; }

		[iOS (7,0)]
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface UIPopoverControllerDelegate {
		[Export ("popoverControllerDidDismissPopover:"), EventArgs ("UIPopoverController")]
		void DidDismiss (UIPopoverController popoverController);

		[Export ("popoverControllerShouldDismissPopover:"), DelegateName ("UIPopoverControllerCondition"), DefaultValue ("true")]
		bool ShouldDismiss (UIPopoverController popoverController);
		
		[iOS (7,0), Export ("popoverController:willRepositionPopoverToRect:inView:"), EventArgs ("UIPopoverControllerReposition")]
		void WillReposition (UIPopoverController popoverController, ref CGRect rect, ref UIView view);
	}

	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (UIPresentationController),
		Delegates=new string [] {"WeakDelegate"},
		Events=new Type [] { typeof (UIPopoverPresentationControllerDelegate) })]
	[DisableDefaultCtor] // NSGenericException Reason: -[UIPopoverController init] is not a valid initializer. You must call -[UIPopoverController initWithContentViewController:]
	partial interface UIPopoverPresentationController {
		// re-exposed from base class
		[Export ("initWithPresentedViewController:presentingViewController:")]
		IntPtr Constructor (UIViewController presentedViewController, [NullAllowed] UIViewController presentingViewController);

		[Export ("delegate", ArgumentSemantic.UnsafeUnretained)]
		NSObject WeakDelegate { get; set; }

#if !XAMCORE_2_0
		[Wrap ("WeakDelegate")]
		[Obsolete ("Use the Delegate property")]
		UIPopoverPresentationControllerDelegate Delegat { get; set; }
#endif
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPopoverPresentationControllerDelegate Delegate { get; set; }
	
		[Export ("permittedArrowDirections", ArgumentSemantic.UnsafeUnretained)]
		UIPopoverArrowDirection PermittedArrowDirections { get; set; }
	
		[Export ("sourceView", ArgumentSemantic.Retain)]
		UIView SourceView { get; set; }
	
		[Export ("sourceRect", ArgumentSemantic.UnsafeUnretained)]
		CGRect SourceRect { get; set; }

		[iOS (9,0)]
		[Export ("canOverlapSourceViewRect")]
		bool CanOverlapSourceViewRect { get; set; }
		
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
	}

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIAdaptivePresentationControllerDelegate {
		[IgnoredInDelegate]
		[Export ("adaptivePresentationStyleForPresentationController:")]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController forPresentationController);
	
		[Export ("presentationController:viewControllerForAdaptivePresentationStyle:"), 
			DelegateName ("UIAdaptivePresentationWithStyleRequested"), DefaultValue (null)]
		UIViewController GetViewControllerForAdaptivePresentation (UIPresentationController controller, UIModalPresentationStyle style);

		[iOS (8,3)]
		[Export ("adaptivePresentationStyleForPresentationController:traitCollection:"),
			DelegateName ("UIAdaptivePresentationStyleWithTraitsRequested"), DefaultValue (UIModalPresentationStyle.None)]
		UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController controller, UITraitCollection traitCollection);

		[iOS (8,3)]
		[Export ("presentationController:willPresentWithAdaptiveStyle:transitionCoordinator:"),
			EventName ("WillPresentController"), EventArgs ("UIWillPresentAdaptiveStyle")]
		void WillPresent (UIPresentationController presentationController, UIModalPresentationStyle style, IUIViewControllerTransitionCoordinator transitionCoordinator);
	}

	[NoTV]
	[Protocol, Model]
	[BaseType (typeof (UIAdaptivePresentationControllerDelegate))]
	partial interface UIPopoverPresentationControllerDelegate {
		[Export ("prepareForPopoverPresentation:"), EventName ("PrepareForPresentation")]
		void PrepareForPopoverPresentation (UIPopoverPresentationController popoverPresentationController);
		
		[Export ("popoverPresentationControllerShouldDismissPopover:"), DelegateName ("ShouldDismiss"), DefaultValue (true)]
		bool ShouldDismissPopover (UIPopoverPresentationController popoverPresentationController);

		[Export ("popoverPresentationControllerDidDismissPopover:"), EventName ("DidDismiss")]
		void DidDismissPopover (UIPopoverPresentationController popoverPresentationController);
		
		[Export ("popoverPresentationController:willRepositionPopoverToRect:inView:"),
			EventName ("WillReposition"), EventArgs ("UIPopoverPresentationControllerReposition")]
		void WillRepositionPopover (UIPopoverPresentationController popoverPresentationController, ref CGRect targetRect, ref UIView inView);
	}
	
	[BaseType (typeof (NSObject))]
	interface UIScreenMode {
		[Export ("pixelAspectRatio")]
		nfloat PixelAspectRatio { get; }

		[Export ("size")]
		CGSize Size { get; }
	}

	[BaseType (typeof (NSObject))]
	interface UITextInputMode : NSSecureCoding {
		[Export ("currentInputMode"), NullAllowed][Static]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[NoTV]
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

	[NoTV]
	[iOS (8,0)]
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

	[NoTV]
	[iOS (8,0)]
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
		bool Present (bool animated, UIPrinterPickerCompletionHandler completion);
	
		[Async (ResultTypeName = "UIPrinterPickerCompletionResult")]
		[Export ("presentFromRect:inView:animated:completionHandler:")]
		bool PresentFromRect (CGRect rect, UIView view, bool animated, UIPrinterPickerCompletionHandler completion);
	
		[Async (ResultTypeName = "UIPrinterPickerCompletionResult")]
		[Export ("presentFromBarButtonItem:animated:completionHandler:")]
		bool PresentFromBarButtonItem (UIBarButtonItem item, bool animated, UIPrinterPickerCompletionHandler completion);
	
		[Export ("dismissAnimated:")]
		void Dismiss (bool animated);
	}

	[NoTV]
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

	[NoTV]
	[BaseType (typeof (NSObject))]
	interface UIPrintPaper {
		[Export ("bestPaperForPageSize:withPapersFromArray:")][Static]
		UIPrintPaper ForPageSize (CGSize pageSize, UIPrintPaper [] paperList);

		[Export ("paperSize")]
		CGSize PaperSize { get; }

		[Export ("printableRect")]
		CGRect PrintableRect { get; }
	}

	[NoTV]
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

	[NoTV]
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

		[iOS (7,0), Export ("printInteractionController:cutLengthForPaper:")]
		[NoDefaultValue]
#if XAMCORE_2_0
		[DelegateName ("Func<UIPrintInteractionController,UIPrintPaper,nfloat>")]
#else
		[DelegateName ("Func<UIPrintInteractionController,UIPrintPaper,float>")]
#endif
		nfloat CutLengthForPaper (UIPrintInteractionController printInteractionController, UIPrintPaper paper);

		[iOS (9, 0)]
		[Export ("printInteractionController:chooseCutterBehavior:"), DefaultValue ("UIPrinterCutterBehavior.NoCut"), DelegateName ("UIPrintInteractionCutterBehavior")]
		UIPrinterCutterBehavior ChooseCutterBehavior (UIPrintInteractionController printInteractionController, NSNumber [] availableBehaviors);
	}

	[NoTV]
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] {typeof(UIPrintInteractionControllerDelegate)})]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[UIPrintInteractionController init] not allowed
	[DisableDefaultCtor]
	interface UIPrintInteractionController {
		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIPrintInteractionControllerDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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
		[Export ("showsPageRange")]
		bool ShowsPageRange { get; set; }

		[Export ("canPrintData:")][Static]
		bool CanPrint (NSData data);

		[Export ("canPrintURL:")][Static]
		bool CanPrint (NSUrl url);

		[Export ("printingAvailable")][Static]
		bool PrintingAvailable { [Bind ("isPrintingAvailable")] get; }

		[Export ("printableUTIs")][Static]
		NSSet PrintableUTIs { get; }

		[Export ("sharedPrintController")][Static]
		UIPrintInteractionController SharedPrintController { get; }

		[Export ("dismissAnimated:")]
		void Dismiss (bool animated);

		[Export ("presentAnimated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		// documentation (and header) mistake that Apple corrected (IIRC I filled that issue)
#if XAMCORE_2_0
		bool
#else
		void
#endif
		Present (bool animated, UIPrintInteractionCompletionHandler completion);

		[Export ("presentFromBarButtonItem:animated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		// documentation (and header) mistake that Apple corrected (IIRC I filled that issue)
#if XAMCORE_2_0
		bool
#else
		void
#endif
		PresentFromBarButtonItem (UIBarButtonItem item, bool animated, UIPrintInteractionCompletionHandler completion);

		[Export ("presentFromRect:inView:animated:completionHandler:")]
		[Async (ResultTypeName = "UIPrintInteractionResult")]
		// documentation (and header) mistake that Apple corrected (IIRC I filled that issue)
#if XAMCORE_2_0
		bool
#else
		void
#endif
		PresentFromRectInView (CGRect rect, UIView view, bool animated, UIPrintInteractionCompletionHandler completion);

		[iOS (7,0), Export ("showsNumberOfCopies")]
		bool ShowsNumberOfCopies { get; set; }


		[iOS (8,0)]
		[Export ("showsPaperSelectionForLoadedPapers")]
		bool ShowsPaperSelectionForLoadedPapers { get; set; }

		[iOS (8,0)]
		[Async (ResultTypeName = "UIPrintInteractionCompletionResult")]
		[Export ("printToPrinter:completionHandler:")]
		bool PrintToPrinter (UIPrinter printer, UIPrintInteractionCompletionHandler completion);
	}

	[NoTV]
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

		[Export ("printInfo")][Static]
		UIPrintInfo PrintInfo { get; }

		[Export ("printInfoWithDictionary:")][Static]
		UIPrintInfo FromDictionary (NSDictionary dictionary);

		[Export ("dictionaryRepresentation")]
		NSDictionary ToDictionary { get; }
	}

	[NoTV]
	[BaseType (typeof (UIPrintFormatter))]
	interface UIViewPrintFormatter {
		[Export ("view")]
		UIView View { get; }
	}

	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	partial interface UIVisualEffect : NSCopying, NSSecureCoding {
	}

	[iOS (8,0)]
	[BaseType (typeof (UIVisualEffect))]
	partial interface UIBlurEffect {
	    [Static, Export ("effectWithStyle:")]
	    UIBlurEffect FromStyle (UIBlurEffectStyle style);
	}

	[iOS (8,0)]
	[BaseType (typeof (UIVisualEffect))]
	partial interface UIVibrancyEffect {
	    [Static, Export ("effectForBlurEffect:")]
	    UIVibrancyEffect FromBlurEffect (UIBlurEffect blurEffect);
	}
		
	[iOS (8,0)]
	[BaseType (typeof (UIView))]
	partial interface UIVisualEffectView : NSSecureCoding {
	
		[DesignatedInitializer]
		[Export ("initWithEffect:")]
		IntPtr Constructor ([NullAllowed] UIVisualEffect effect);
	
	    [Export ("contentView", ArgumentSemantic.Retain)]
	    UIView ContentView { get; }
	
		[NullAllowed]
		[Export ("effect", ArgumentSemantic.Copy)]
		UIVisualEffect Effect { get; set; }
	}

	[NoTV]
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
		IntPtr Constructor ([NullAllowed] string text);

		[iOS (7,0)]
		[Export ("initWithAttributedText:")]
		IntPtr Constructor ([NullAllowed] NSAttributedString text);

		[iOS (7,0)]
		[NullAllowed]
		[Export ("attributedText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedText { get; set; }
	}

	[NoTV]
	[BaseType (typeof (NSObject))]
	interface UIPrintFormatter : NSCopying {

		[Deprecated (PlatformName.iOS, 10, 0, message:"Use 'PerPageContentInsets' instead.")]
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

		[iOS (8,0)]
		[Export ("perPageContentInsets")]
		UIEdgeInsets PerPageContentInsets { get; set; }
	}

	[NoTV]
	[BaseType (typeof (UIPrintFormatter))]
#if XAMCORE_4_0
	[DisableDefaultCtor] // nonfunctional (and it doesn't show up in the header anyway)
#endif
	interface UIMarkupTextPrintFormatter {
		[NullAllowed] // by default this property is null
		[Export ("markupText", ArgumentSemantic.Copy)]
		string MarkupText { get; set; }

		[Export ("initWithMarkupText:")]
		IntPtr Constructor ([NullAllowed] string text);
	}

	[iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface UIMotionEffect : NSCoding, NSCopying {
		[Export ("keyPathsAndRelativeValuesForViewerOffset:")]
		NSDictionary ComputeKeyPathsAndRelativeValues (UIOffset viewerOffset);
	}

	[iOS (7,0)]
	[BaseType (typeof (UIMotionEffect))]
	interface UIInterpolatingMotionEffect : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithKeyPath:type:")]
		IntPtr Constructor (string keyPath, UIInterpolatingMotionEffectType type);
		
		[Export ("keyPath")]
		string KeyPath { get;  }

		[Export ("type")]
		UIInterpolatingMotionEffectType Type { get;  }

		[NullAllowed] // by default this property is null
		[Export ("minimumRelativeValue", ArgumentSemantic.Retain)]
		NSObject MinimumRelativeValue { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("maximumRelativeValue", ArgumentSemantic.Retain)]
		NSObject MaximumRelativeValue { get; set;  }
	}

	[iOS (7,0)]
	[BaseType (typeof (UIMotionEffect))]
	interface UIMotionEffectGroup {
		[NullAllowed] // by default this property is null
		[Export ("motionEffects", ArgumentSemantic.Copy)]
		UIMotionEffect [] MotionEffects { get; set; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // designated
	interface UISpringTimingParameters : UITimingCurveProvider
	{
		[DesignatedInitializer]
		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initialVelocity")]
		CGVector InitialVelocity { get; }

		[Export ("initWithDampingRatio:initialVelocity:")]
		[DesignatedInitializer]
		IntPtr Constructor (nfloat ratio, CGVector velocity);

		[Export ("initWithMass:stiffness:damping:initialVelocity:")]
		[DesignatedInitializer]
		IntPtr Constructor (nfloat mass, nfloat stiffness, nfloat damping, CGVector velocity);

		[Export ("initWithDampingRatio:")]
		IntPtr Constructor (nfloat ratio);
	}
		
	[NoTV]
	[Category, BaseType (typeof (NSString))]
	interface UIStringDrawing {
		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGPoint, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:withFont:")]
		CGSize DrawString (CGPoint point, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:lineBreakMode:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:fontSize:lineBreakMode:baselineAdjustment:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawAtPoint:forWidth:withFont:minFontSize:actualFontSize:lineBreakMode:baselineAdjustment:")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:")]
		CGSize DrawString (CGRect rect, UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:lineBreakMode:")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.DrawString (CGRect, UIStringAttributes)' instead.")]
		[Export ("drawInRect:withFont:lineBreakMode:alignment:")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.GetSizeUsingAttributes (UIStringAttributes)' instead.")]
		[Export ("sizeWithFont:")]
		CGSize StringSize (UIFont font);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:forWidth:lineBreakMode:")]
		CGSize StringSize (UIFont font, nfloat forWidth, UILineBreakMode breakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:constrainedToSize:")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0, Message = "Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes, NSStringDrawingContext)' instead.")]
		[Export ("sizeWithFont:constrainedToSize:lineBreakMode:")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode);

		// note: duplicate from maccore's foundation.cs where it's binded on NSString2 (for Classic)
		[ThreadSafe]
		[Availability (Deprecated = Platform.iOS_7_0)]
		// Wait for guidance here: https://devforums.apple.com/thread/203655
		//[Obsolete ("Deprecated on iOS7.   No guidance.")]
		[Export ("sizeWithFont:minFontSize:actualFontSize:forWidth:lineBreakMode:")]
		CGSize StringSize (UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode);
	}
#endif // !WATCH

	[Category, BaseType (typeof (NSString))]
	interface NSStringDrawing {
		[iOS (7,0)]
		[Export ("sizeWithAttributes:")]
		CGSize WeakGetSizeUsingAttributes (NSDictionary attributes);

		[iOS (7,0)]
		[Wrap ("WeakGetSizeUsingAttributes (This, attributes.Dictionary)")]
		CGSize GetSizeUsingAttributes (UIStringAttributes attributes);

		[iOS (7,0)]
		[Export ("drawAtPoint:withAttributes:")]
		void WeakDrawString (CGPoint point, NSDictionary attributes);

		[iOS (7,0)]
		[Wrap ("WeakDrawString (This, point, attributes.Dictionary)")]
		void DrawString (CGPoint point, UIStringAttributes attributes);

		[iOS (7,0)]
		[Export ("drawInRect:withAttributes:")]
		void WeakDrawString (CGRect rect, NSDictionary attributes);

		[iOS (7,0)]
		[Wrap ("WeakDrawString (This, rect, attributes.Dictionary)")]
		void DrawString (CGRect rect, UIStringAttributes attributes);
	}

	[Category, BaseType (typeof (NSString))]
	interface NSExtendedStringDrawing {
		[iOS (7,0)]
		[Export ("drawWithRect:options:attributes:context:")]
		void WeakDrawString (CGRect rect, NSStringDrawingOptions options, NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		[iOS (7,0)]
		[Wrap ("WeakDrawString (This, rect, options, attributes == null ? null : attributes.Dictionary, context)")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, UIStringAttributes attributes, [NullAllowed] NSStringDrawingContext context);
		
		[iOS (7,0)]
		[Export ("boundingRectWithSize:options:attributes:context:")]
		CGRect WeakGetBoundingRect (CGSize size, NSStringDrawingOptions options, NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		[iOS (7,0)]
		[Wrap ("WeakGetBoundingRect (This, size, options, attributes == null ? null : attributes.Dictionary, context)")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, UIStringAttributes attributes, [NullAllowed] NSStringDrawingContext context);
	}

#if !WATCH
	[NoWatch]
	[iOS (7,0)]
	[BaseType (typeof (UIView))]
	interface UIInputView : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithFrame:inputViewStyle:")]
		IntPtr Constructor (CGRect frame, UIInputViewStyle inputViewStyle);

		[Export ("inputViewStyle")]
		UIInputViewStyle InputViewStyle { get; }

		[iOS (9,0)]
		[Export ("allowsSelfSizing")]
		bool AllowsSelfSizing { get; set; }
	}

	interface IUITextInputDelegate {
	}

	interface IUITextDocumentProxy {}
	
	[NoWatch]
	[iOS (8,0)]
	[BaseType (typeof (UIViewController))]
	partial interface UIInputViewController : UITextInputDelegate {

		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("inputView", ArgumentSemantic.Retain), NullAllowed]
		UIInputView InputView { get; set; }
		
		[Export ("textDocumentProxy"), NullAllowed]
		IUITextDocumentProxy TextDocumentProxy { get; }
		
		[Export ("dismissKeyboard")]
		void DismissKeyboard ();
		
		[Export ("advanceToNextInputMode")]
		void AdvanceToNextInputMode ();

		[NoTV]
		[Export ("requestSupplementaryLexiconWithCompletion:")]
		[Async]
		void RequestSupplementaryLexicon (Action<UILexicon> completionHandler);

		[NullAllowed] // by default this property is null
		[Export ("primaryLanguage")]
		string PrimaryLanguage { get; set; }

		[iOS (11,1), TV (11,1)]
		[Export ("hasDictationKey")]
		bool HasDictationKey { get; set; }

		[iOS (10,0), TV (10,0)]
		[Export ("handleInputModeListFromView:withEvent:")]
		void HandleInputModeList (UIView fromView, UIEvent withEvent);

		[iOS (11,0), TV (11,0)]
		[Export ("hasFullAccess")]
		bool HasFullAccess { get; }

		[iOS (11,0), TV (11,0)]
		[Export ("needsInputModeSwitchKey")]
		bool NeedsInputModeSwitchKey { get; }
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Protocol]
	interface UIInteraction
	{
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
	[iOS (8,0)]
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

		// Another abstract that was introduced on this released, breaking ABI
		// Radar: 26867207
#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (10, 0)]
		[NullAllowed, Export ("documentInputMode")]
		UITextInputMode DocumentInputMode { get; }

		// New abstract, breaks ABI
		// Radar: 33685383
#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (11,0)]
		[NullAllowed, Export ("selectedText")]
		string SelectedText { get; }

		// New abstract, breaks ABI
		// Radar: 33685383
#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (11,0)]
		[Export ("documentIdentifier", ArgumentSemantic.Copy)]
		NSUuid DocumentIdentifier { get; }
	}

	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UILayoutGuide : NSCoding
	{
		[Export ("layoutFrame")]
		CGRect LayoutFrame { get; }
	
		[NullAllowed, Export ("owningView", ArgumentSemantic.Weak)]
		UIView OwningView { get; set; }
	
		[Export ("identifier")]
		string Identifier { get; set; }
	
#if XAMCORE_2_0 // NSLayoutXAxisAnchor is a generic type, only supported in Unified (for now)
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
#endif // XAMCORE_2_0
	}
		
	[NoWatch]
	[iOS (7,0)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface UILayoutSupport {
		[Export ("length")]
		[Abstract]
		nfloat Length { get; }

#if XAMCORE_2_0 // NSLayoutYAxisAnchor is generic, which is only supported in Unified (for now at least)
		[iOS (9,0)]
		[Export ("topAnchor", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutYAxisAnchor TopAnchor { get; }

		[iOS (9,0)]
		[Export ("bottomAnchor", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutYAxisAnchor BottomAnchor { get; }

		[iOS (9,0)]
		[Export ("heightAnchor", ArgumentSemantic.Strong)]
#if XAMCORE_4_0
		// Apple added a new required member in iOS 9, but that breaks our binary compat, so we can't do that in our existing code.
		[Abstract]
#endif
		NSLayoutDimension HeightAnchor { get; }
#endif // XAMCORE_2_0
	}

	interface IUILayoutSupport {}

	// This protocol is supposed to be an aggregate to existing classes,
	// at the moment there is no API that require a specific UIAccessibilityIdentification
	// implementation, so we don't provide a Model class (for now at least).
	[NoWatch]
	[Protocol]
	interface UIAccessibilityIdentification {
		[Abstract]
		[NullAllowed] // by default this property is null
		[Export ("accessibilityIdentifier", ArgumentSemantic.Copy)]
		string AccessibilityIdentifier { get; set;  }
	}

	interface IUIAccessibilityIdentification {}

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationSettings' instead.")]
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
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
	[iOS (8,0)]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationCategory' instead.")]
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
	[iOS (8,0)]
	[BaseType (typeof (UIUserNotificationCategory))]
	partial interface UIMutableUserNotificationCategory {

		[NullAllowed] // by default this property is null
		[Export ("identifier")]
		string Identifier { get; set; }

		[Export ("setActions:forContext:")]
		void SetActions (UIUserNotificationAction [] actions, UIUserNotificationActionContext context);
	}

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface UIUserNotificationAction : NSCopying, NSMutableCopying, NSSecureCoding {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("activationMode", ArgumentSemantic.Assign)]
		UIUserNotificationActivationMode ActivationMode { get; }

		[Export ("authenticationRequired", ArgumentSemantic.Assign)]
		bool AuthenticationRequired { [Bind ("isAuthenticationRequired")]get; }

		[Export ("destructive", ArgumentSemantic.Assign)]
		bool Destructive { [Bind ("isDestructive")]get; }

		[iOS (9,0)]
		[Export ("parameters", ArgumentSemantic.Copy)]
		NSDictionary Parameters { get; [NotImplemented] set; }

		[iOS (9,0)]
		[Export ("behavior", ArgumentSemantic.Assign)]
		UIUserNotificationActionBehavior Behavior { get; [NotImplemented] set;}

		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNTextInputNotificationAction.TextInputButtonTitle' instead.")]
		[Field ("UIUserNotificationTextInputActionButtonTitleKey")]
		NSString TextInputActionButtonTitleKey { get; }

		// note: defined twice, where watchOS is defined it says it's not in iOS, the other one (for iOS 9) says it's not in tvOS
		[iOS (9,0)]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UNTextInputNotificationResponse.UserText' instead.")]
		[Field ("UIUserNotificationActionResponseTypedTextKey")]
		NSString ResponseTypedTextKey { get; }
	}
#else
	[Watch (2,0)]
	[Static]
	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationAction' or 'UserNotifications.UNTextInputNotificationAction' instead.")]
	interface UIUserNotificationAction {
		// note: defined twice, where watchOS is defined it says it's not in iOS, the other one (for iOS 9) says it's not in tvOS
		[Field ("UIUserNotificationActionResponseTypedTextKey")]
		NSString ResponseTypedTextKey { get; }
	}
#endif

	[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'UserNotifications.UNNotificationAction' instead.")]
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
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
		bool AuthenticationRequired { [Bind ("isAuthenticationRequired")]get; set; }

		[Export ("destructive", ArgumentSemantic.Assign)]
		bool Destructive { [Bind ("isDestructive")]get; set; }

		[iOS (9,0)]
		[Export ("behavior", ArgumentSemantic.Assign)]
		UIUserNotificationActionBehavior Behavior { get; set; }

		[iOS (9,0)]
		[Export ("parameters", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary Parameters { get; set; }		
	}

#if !WATCH
	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'UIDocumentPickerViewController' instead.")]
	[BaseType (typeof (UIViewController), Delegates=new string [] {"Delegate"}, Events=new Type [] {typeof (UIDocumentMenuDelegate)})]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You cannot initialize a UIDocumentMenuViewController except by the initWithDocumentTypes:inMode: and initWithURL:inMode: initializers.
	partial interface UIDocumentMenuViewController : NSCoding {

		[DesignatedInitializer]
		[Export ("initWithDocumentTypes:inMode:")]
		IntPtr Constructor (string [] allowedUTIs, UIDocumentPickerMode mode);

		[DesignatedInitializer]
		[Export ("initWithURL:inMode:")]
		IntPtr Constructor (NSUrl url, UIDocumentPickerMode mode);

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
	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIDocumentMenuDelegate {
		[Abstract]
		[Export ("documentMenu:didPickDocumentPicker:"), EventArgs ("UIDocumentMenuDocumentPicked")]
		void DidPickDocumentPicker (UIDocumentMenuViewController documentMenu, UIDocumentPickerViewController documentPicker);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("documentMenuWasCancelled:")]
		void WasCancelled (UIDocumentMenuViewController documentMenu);
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (UIViewController), Delegates=new string [] {"Delegate"}, Events=new Type [] {typeof (UIDocumentPickerDelegate)})]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You cannot initialize a UIDocumentPickerViewController except by the initWithDocumentTypes:inMode: and initWithURL:inMode: initializers
	partial interface UIDocumentPickerViewController : NSCoding {

		[DesignatedInitializer]
		[Export ("initWithDocumentTypes:inMode:")]
		IntPtr Constructor (string [] allowedUTIs, UIDocumentPickerMode mode);

		[Advice ("This method will be deprecated in a future release and should be avoided. Instead, use 'UIDocumentPickerViewController (NSUrl[], UIDocumentPickerMode)'.")]
		[DesignatedInitializer]
		[Export ("initWithURL:inMode:")]
		IntPtr Constructor (NSUrl url, UIDocumentPickerMode mode);

		[iOS (11,0)]
		[Export ("initWithURLs:inMode:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl[] urls, UIDocumentPickerMode mode);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		UIDocumentPickerDelegate Delegate { get; set; }

		[Export ("documentPickerMode", ArgumentSemantic.Assign)]
		UIDocumentPickerMode DocumentPickerMode { get; }

		[iOS (11,0)]
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface UIDocumentPickerDelegate {
		[Deprecated (PlatformName.iOS, 11, 0, message: "Implement 'DidPickDocument (UIDocumentPickerViewController, NSUrl[])' instead.")]
#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("documentPicker:didPickDocumentAtURL:"), EventArgs ("UIDocumentPicked")]
		void DidPickDocument (UIDocumentPickerViewController controller, NSUrl url);

		[iOS (11,0)]
		[Export ("documentPicker:didPickDocumentsAtURLs:"), EventArgs ("UIDocumentPickedAtUrls"), EventName ("DidPickDocumentAtUrls")]
		void DidPickDocument (UIDocumentPickerViewController controller, NSUrl[] urls);

		[Export ("documentPickerWasCancelled:")]
		void WasCancelled (UIDocumentPickerViewController controller);
	}

	[NoWatch]
	[NoTV]
	[iOS (8,0)]
	[BaseType (typeof (UIViewController))]
	partial interface UIDocumentPickerExtensionViewController {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		IntPtr Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

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
	[iOS (6,0)]
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

		[TV (11,0), iOS (11,0)]
		[Export ("accessibilityAttributedContentForLineNumber:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedContent (nint lineNumber);

		[TV (11,0), iOS (11,0)]
		[NullAllowed, Export ("accessibilityAttributedPageContent")]
		NSAttributedString GetAccessibilityAttributedPageContent ();
	}

	[NoWatch]
	[iOS (7,0)]
	[Protocol]
	interface UIGuidedAccessRestrictionDelegate {
		[Abstract]
		[Export ("guidedAccessRestrictionIdentifiers")]
		string [] GetGuidedAccessRestrictionIdentifiers { get; }

		[Abstract]
		[Export ("guidedAccessRestrictionWithIdentifier:didChangeState:")][EventArgs ("UIGuidedAccessRestriction")]
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
	[iOS (9,0)] // added in Xcode 7.1 / iOS 9.1 SDK
	[BaseType (typeof (UIFocusUpdateContext))]
	interface UICollectionViewFocusUpdateContext {
		
		[Export ("previouslyFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath PreviouslyFocusedIndexPath { [return: NullAllowed] get; }

		[Export ("nextFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath NextFocusedIndexPath { [return: NullAllowed] get; }
	}

	[iOS (10,0), TV (10,0)]
	[BaseType (typeof(NSObject))]
	[DesignatedDefaultCtor]
	interface UICubicTimingParameters : UITimingCurveProvider
	{
		[Export ("animationCurve")]
		UIViewAnimationCurve AnimationCurve { get; }

		[Export ("controlPoint1")]
		CGPoint ControlPoint1 { get; }

		[Export ("controlPoint2")]
		CGPoint ControlPoint2 { get; }

		[Export ("initWithAnimationCurve:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIViewAnimationCurve curve);

		[Export ("initWithControlPoint1:controlPoint2:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGPoint point1, CGPoint point2);
	}

	interface IUIFocusAnimationContext {}

	[iOS (11,0)]
	[Protocol]
	interface UIFocusAnimationContext {
		[Abstract]
		[Export ("duration")]
		double Duration { get; }
	}
		
	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface UIFocusAnimationCoordinator {
		[Export ("addCoordinatedAnimations:completion:")]
		[Async]
		void AddCoordinatedAnimations ([NullAllowed] Action animations, [NullAllowed] Action completion);

		[Async]
		[TV (11,0), iOS (11,0)]
		[Export ("addCoordinatedFocusingAnimations:completion:")]
		void AddCoordinatedFocusingAnimations ([NullAllowed] Action<IUIFocusAnimationContext> animations, [NullAllowed] Action completion);

		[Async]
		[TV (11,0), iOS (11,0)]
		[Export ("addCoordinatedUnfocusingAnimations:completion:")]
		void AddCoordinatedUnfocusingAnimations ([NullAllowed] Action<IUIFocusAnimationContext> animations, [NullAllowed] Action completion);
	}

	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof (UILayoutGuide))]
	interface UIFocusGuide {
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'PreferredFocusEnvironments' instead.")]
		[NullAllowed, Export ("preferredFocusedView", ArgumentSemantic.Weak)]
		UIView PreferredFocusedView { get; set; }

		[iOS (10,0), TV (10,0)]
		[Export ("preferredFocusEnvironments", ArgumentSemantic.Copy), NullAllowed] // null_resettable
		IUIFocusEnvironment[] PreferredFocusEnvironments { get; set; }
	}

	[TV (12,0), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIFocusMovementHint : NSCopying
	{
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

	interface IUIFocusItem {}
	[iOS (10,0)]
	[Protocol]
	interface UIFocusItem : UIFocusEnvironment
	{
		[Abstract]
		[Export ("canBecomeFocused")]
		bool CanBecomeFocused { get; }

		// FIXME: declared as a @required, but this breaks compatibility
		// Radar: 41121416
		[TV (12, 0), iOS (12, 0), NoWatch]
#if XAMCORE_4_0 
		[Abstract]
#endif
		[Export ("frame")]
		CGRect Frame { get; }

		[TV (12, 0), iOS (12, 0), NoWatch]
		[Export ("didHintFocusMovement:")]
		void DidHintFocusMovement (UIFocusMovementHint hint);
	}
		
	[DisableDefaultCtor] // [Assert] -init is not a useful initializer for this class. Use one of the designated initializers instead
	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	interface UIFocusUpdateContext {
		[NullAllowed, Export ("previouslyFocusedView", ArgumentSemantic.Weak)]
		UIView PreviouslyFocusedView { get; }

		[NullAllowed, Export ("nextFocusedView", ArgumentSemantic.Weak)]
		UIView NextFocusedView { get; }

		[Export ("focusHeading", ArgumentSemantic.Assign)]
		UIFocusHeading FocusHeading { get; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("previouslyFocusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem PreviouslyFocusedItem { get; }

		[iOS (10,0), TV (10,0)]
		[NullAllowed, Export ("nextFocusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem NextFocusedItem { get; }

		[iOS (11,0), TV (11,0)]
		[Notification]
		[Field ("UIFocusDidUpdateNotification")]
		NSString DidUpdateNotification { get; }

		[iOS (11,0), TV (11,0)]
		[Notification]
		[Field ("UIFocusMovementDidFailNotification")]
		NSString MovementDidFailNotification { get; }

		[iOS (11,0), TV (11,0)]
		[Field ("UIFocusUpdateContextKey")]
		NSString Key { get; }

		[iOS (11,0), TV (11,0)]
		[Field ("UIFocusUpdateAnimationCoordinatorKey")]
		NSString AnimationCoordinatorKey { get; }
	}

	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIFocusSystem {
		[Static]
		[Export ("environment:containsEnvironment:")]
		bool Contains (IUIFocusEnvironment environment, IUIFocusEnvironment otherEnvironment);

		[NoiOS]
		[Static]
		[Export ("registerURL:forSoundIdentifier:")]
		void RegisterUrl (NSUrl soundFileUrl, NSString identifier);

		// The 2 values associated with the 'UIFocusSoundIdentifier' smart enum cannot be used.
		// See https://developer.apple.com/documentation/uikit/uifocussystem/2887479-register
		// Do not specify one of the UIKit sound identifiers (such as default); doing so will cause an immediate assertion failure and crash your app.
		
		[TV (12, 0), iOS (12, 0)]
		[NullAllowed, Export ("focusedItem", ArgumentSemantic.Weak)]
		IUIFocusItem FocusedItem { get; }

		[TV (12,0), iOS (12,0)]
		[Static]
		[Export ("focusSystemForEnvironment:")]
		[return: NullAllowed]
		UIFocusSystem Create (IUIFocusEnvironment environment);

		[TV (12,0), iOS (12,0)]
		[Export ("requestFocusUpdateToEnvironment:")]
		void RequestFocusUpdate (IUIFocusEnvironment environment);

		[TV (12,0), iOS (12,0)]
		[Export ("updateFocusIfNeeded")]
		void UpdateFocusIfNeeded ();
	}

	interface IUIFocusDebuggerOutput {}

	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[Protocol]
	interface UIFocusDebuggerOutput {}

	[NoWatch]
	[iOS (11,0), TV (11,0)]
	[BaseType (typeof(NSObject))]
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
	}

	[NoWatch]
	[iOS (9,0)]
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
		UIGestureRecognizer[] GestureRecognizers { get; }

		[Export ("force")]
		nfloat Force { get; }
	}

	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof (UIEvent))]
	interface UIPressesEvent {
		[Export ("allPresses")]
		NSSet<UIPress> AllPresses { get; }

		[Export ("pressesForGestureRecognizer:")]
		NSSet<UIPress> GetPresses (UIGestureRecognizer gesture);
	}

	[NoWatch, NoTV, iOS (10,0)]
	[BaseType (typeof(NSObject), Delegates=new string [] {"Delegate"}, Events=new Type [] { typeof (UIPreviewInteractionDelegate)})]
	[DisableDefaultCtor]
	interface UIPreviewInteraction {

		[Export ("initWithView:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIView view);

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

	[NoWatch, NoTV, iOS (10, 0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UIPreviewInteractionDelegate {

		[Abstract]
		[Export ("previewInteraction:didUpdatePreviewTransition:ended:")]
		[EventArgs ("NSPreviewInteractionPreviewUpdate")]
		void DidUpdatePreviewTransition (UIPreviewInteraction previewInteraction, nfloat transitionProgress, bool ended);

		[Abstract]
		[Export ("previewInteractionDidCancel:")]
		void DidCancel (UIPreviewInteraction previewInteraction);

		[Export ("previewInteractionShouldBegin:")]
		[DelegateName ("Func<UIPreviewInteraction,bool>"), DefaultValue(true)]
		bool ShouldBegin (UIPreviewInteraction previewInteraction);

		[Export ("previewInteraction:didUpdateCommitTransition:ended:")]
		[EventArgs ("NSPreviewInteractionPreviewUpdate")]
		void DidUpdateCommit (UIPreviewInteraction previewInteraction, nfloat transitionProgress, bool ended);
	}
	
	[NoWatch]
	[iOS (9,0)]
	[BaseType (typeof (UIFocusUpdateContext))]
	interface UITableViewFocusUpdateContext {
		
		[Export ("previouslyFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath PreviouslyFocusedIndexPath { [return: NullAllowed] get; }

		[Export ("nextFocusedIndexPath", ArgumentSemantic.Strong)]
		NSIndexPath NextFocusedIndexPath { [return: NullAllowed] get; }
	}

	[NoWatch, NoiOS]
	[TV (11,0)]
	public enum UIFocusSoundIdentifier {

		[Field ("UIFocusSoundIdentifierNone")]
		None,

		[Field ("UIFocusSoundIdentifierDefault")]
		Default,
	}

	interface IUIFocusEnvironment {}
	[NoWatch]
	[iOS (9,0)]
	[Protocol]
	interface UIFocusEnvironment {
		// Apple moved this member to @optional since they deprecated it
		// but that's a breaking change for us, so it remains [Abstract]
		// and we need to teach the intro and xtro tests about it
		[Abstract]
		[NullAllowed, Export ("preferredFocusedView", ArgumentSemantic.Weak)]
		[iOS (9,0)] // duplicated so it's inlined properly
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'PreferredFocusEnvironments' instead.")]
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
#if XAMCORE_4_0
		[Abstract]
#endif
		[iOS (10, 0)]
		[Export ("preferredFocusEnvironments", ArgumentSemantic.Copy)]
		IUIFocusEnvironment[] PreferredFocusEnvironments { get; }

		[NoiOS]
		[TV (11,0)]
		[Export ("soundIdentifierForFocusUpdateInContext:")]
		[return: NullAllowed]
		NSString GetSoundIdentifier (UIFocusUpdateContext context);

		// FIXME: declared as a @required, but this breaks compatibility
		// Radar: 41121293
		[TV (12, 0), iOS (12, 0)]
#if XAMCORE_4_0 
		[Abstract]
#endif
		[NullAllowed, Export ("parentFocusEnvironment", ArgumentSemantic.Weak)]
		IUIFocusEnvironment ParentFocusEnvironment { get; }

		[TV (12, 0), iOS (12, 0)]
#if XAMCORE_4_0 
		[Abstract]
#endif
		[NullAllowed, Export ("focusItemContainer")]
		IUIFocusItemContainer FocusItemContainer { get; }
	}

	[TV (12,0), iOS (12,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UITextInputPasswordRules : NSSecureCoding, NSCopying
	{
		[Export ("passwordRulesDescriptor")]
		string PasswordRulesDescriptor { get; }

		[Static]
		[Export ("passwordRulesWithDescriptor:")]
		UITextInputPasswordRules Create (string passwordRulesDescriptor);
	}
#endif // !WATCH

	[NoWatch][NoTV]
	[Static][Internal]
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

#if !WATCH
#region Drag and Drop
	interface IUIInteraction {}
	interface IUIDropSession {}
	interface IUIDragDropSession {}
	interface IUIDragAnimating {}
	interface IUIDragSession {}
	interface IUIDragInteractionDelegate {}
	interface IUIDropInteractionDelegate {}
	interface IUICollectionViewDragDelegate {}
	interface IUICollectionViewDropDelegate {}
	interface IUICollectionViewDropCoordinator {}
	interface IUICollectionViewDropItem {}
	interface IUICollectionViewDropPlaceholderContext {}
	interface IUITableViewDragDelegate {}
	interface IUITableViewDropDelegate {}
	interface IUITableViewDropCoordinator {}
	interface IUITableViewDropItem {}
	interface IUITableViewDropPlaceholderContext {}
	interface IUITextDragDelegate {}
	interface IUITextDraggable {}
	interface IUITextDragRequest {}
	interface IUITextDroppable {}
	interface IUITextDropDelegate {}
	interface IUITextDropRequest {}

	[NoWatch, NoTV, iOS (11,0)]
	[Protocol]
	interface UIDragAnimating
	{
		[Abstract]
		[Export ("addAnimations:")]
		void AddAnimations (Action animations);
	
		[Abstract]
		[Export ("addCompletion:")]
		void AddCompletion (Action<UIViewAnimatingPosition> completion);
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Protocol]
	interface UIDragDropSession
	{
		[Abstract]
		[Export ("items")]
		UIDragItem[] Items { get; }
	
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
		bool HasConformingItems (string[] typeIdentifiers);
	
		[Abstract]
		[Export ("canLoadObjectsOfClass:")]
		bool CanLoadObjects (Class itemProviderReadingClass);
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDragItem
	{
		[Export ("initWithItemProvider:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSItemProvider itemProvider);
	
		[Export ("itemProvider")]
		NSItemProvider ItemProvider { get; }
	
		[NullAllowed, Export ("localObject", ArgumentSemantic.Strong)]
		NSObject LocalObject { get; set; }
	
		[NullAllowed, Export ("previewProvider", ArgumentSemantic.Copy)]
		Func<UIDragPreview> PreviewProvider { get; set; }
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDragPreview : NSCopying
	{
		[Export ("initWithView:parameters:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIView view, UIDragPreviewParameters parameters);
	
		[Export ("initWithView:")]
		IntPtr Constructor (UIView view);
	
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
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DesignatedDefaultCtor]
	interface UIDragPreviewParameters : NSCopying
	{
		[Export ("initWithTextLineRects:")]
		IntPtr Constructor (NSValue[] textLineRects);
	
		[NullAllowed, Export ("visiblePath", ArgumentSemantic.Copy)]
		UIBezierPath VisiblePath { get; set; }
	
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		UIColor BackgroundColor { get; set; }
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDragPreviewTarget : NSCopying
	{
		[Export ("initWithContainer:center:transform:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIView container, CGPoint center, CGAffineTransform transform);
	
		[Export ("initWithContainer:center:")]
		IntPtr Constructor (UIView container, CGPoint center);
	
		[Export ("container")]
		UIView Container { get; }
	
		[Export ("center")]
		CGPoint Center { get; }
	
		[Export ("transform")]
		CGAffineTransform Transform { get; }
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Protocol]
	interface UIDragSession : UIDragDropSession
	{
		[Abstract]
		[NullAllowed, Export ("localContext", ArgumentSemantic.Strong)]
		NSObject LocalContext { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDragInteraction : UIInteraction {
		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		IntPtr Constructor (IUIDragInteractionDelegate @delegate);

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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UIDragInteractionDelegate {
		[Abstract]
		[Export ("dragInteraction:itemsForBeginningSession:")]
		UIDragItem[] GetItemsForBeginningSession (UIDragInteraction interaction, IUIDragSession session);

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
		UIDragItem[] GetItemsForAddingToSession (UIDragInteraction interaction, IUIDragSession session, CGPoint point);

		[Export ("dragInteraction:sessionForAddingItems:withTouchAtPoint:")]
		[return: NullAllowed]
		IUIDragSession GetSessionForAddingItems (UIDragInteraction interaction, IUIDragSession[] sessions, CGPoint point);

		[Export ("dragInteraction:session:willAddItems:forInteraction:")]
		void WillAddItems (UIDragInteraction interaction, IUIDragSession session, UIDragItem[] items, UIDragInteraction addingInteraction);

		[Export ("dragInteraction:previewForCancellingItem:withDefault:")]
		[return: NullAllowed]
		UITargetedDragPreview GetPreviewForCancellingItem (UIDragInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview);

		[Export ("dragInteraction:item:willAnimateCancelWithAnimator:")]
		void WillAnimateCancel (UIDragInteraction interaction, UIDragItem item, IUIDragAnimating animator);
	}

	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))] // If Apple adds a delegate setter: Delegates=new string [] {"Delegate"}, Events=new Type [] { typeof (UIDropInteractionDelegate)})]
	[DisableDefaultCtor]
	interface UIDropInteraction : UIInteraction
	{
		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		IntPtr Constructor (IUIDropInteractionDelegate @delegate);
	
		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		IUIDropInteractionDelegate Delegate { get; }
	
		[Export ("allowsSimultaneousDropSessions")]
		bool AllowsSimultaneousDropSessions { get; set; }
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UIDropInteractionDelegate
	{
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
		[return: NullAllowed][DelegateName("UIDropInteractionPreviewForItem"), NoDefaultValue]
		UITargetedDragPreview GetPreviewForDroppingItem (UIDropInteraction interaction, UIDragItem item, UITargetedDragPreview defaultPreview);
	
		[Export ("dropInteraction:item:willAnimateDropWithAnimator:"), EventArgs("UIDropInteractionAnimation")]
		void WillAnimateDrop (UIDropInteraction interaction, UIDragItem item, IUIDragAnimating animator);
	}
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDropProposal : NSCopying
	{
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIDropOperation operation);
	
		[Export ("operation")]
		UIDropOperation Operation { get; }
	
		[Export ("precise")]
		bool Precise { [Bind ("isPrecise")] get; set; }
	
		[Export ("prefersFullSizePreview")]
		bool PrefersFullSizePreview { get; set; }
	}

	[NoWatch, NoTV, iOS (11,0)]
	[Protocol]
	interface UIDropSession : UIDragDropSession, NSProgressReporting
	{
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
	
	[NoWatch, NoTV, iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UITargetedDragPreview : NSCopying
	{
		[Export ("initWithView:parameters:target:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIView view, UIDragPreviewParameters parameters, UIDragPreviewTarget target);
	
		[Export ("initWithView:parameters:")]
		IntPtr Constructor (UIView view, UIDragPreviewParameters parameters);
	
		[Export ("initWithView:")]
		IntPtr Constructor (UIView view);
	
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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UICollectionViewDragDelegate {
		[Abstract]
		[Export ("collectionView:itemsForBeginningDragSession:atIndexPath:")]
		UIDragItem[] GetItemsForBeginningDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath);

		[Export ("collectionView:itemsForAddingToDragSession:atIndexPath:point:")]
		UIDragItem[] GetItemsForAddingToDragSession (UICollectionView collectionView, IUIDragSession session, NSIndexPath indexPath, CGPoint point);

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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
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
	[iOS (11,0)]
	[BaseType (typeof(UIDropProposal))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Not implemented
	interface UICollectionViewDropProposal {

		// inline from base type
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIDropOperation operation);

		[Export ("initWithDropOperation:intent:")]
		IntPtr Constructor (UIDropOperation operation, UICollectionViewDropIntent intent);

		[Export ("intent")]
		UICollectionViewDropIntent Intent { get; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UICollectionViewDropCoordinator {
		[Abstract]
		[Export ("items")]
		IUICollectionViewDropItem[] Items { get; }

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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UICollectionViewPlaceholder {
		[Export ("initWithInsertionIndexPath:reuseIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier);

		[NullAllowed, Export ("cellUpdateHandler", ArgumentSemantic.Copy)]
		Action<UICollectionViewCell> CellUpdateHandler { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(UICollectionViewPlaceholder))]
	interface UICollectionViewDropPlaceholder {
		// inlined
		[Export ("initWithInsertionIndexPath:reuseIdentifier:")]
		IntPtr Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier);

		[NullAllowed, Export ("previewParametersProvider", ArgumentSemantic.Copy)]
		Func<UICollectionViewCell, UIDragPreviewParameters> PreviewParametersProvider { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
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
	[iOS (11,0)]
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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UITableViewDragDelegate {
		[Abstract]
		[Export ("tableView:itemsForBeginningDragSession:atIndexPath:")]
		UIDragItem[] GetItemsForBeginningDragSession (UITableView tableView, IUIDragSession session, NSIndexPath indexPath);

		[Export ("tableView:itemsForAddingToDragSession:atIndexPath:point:")]
		UIDragItem[] GetItemsForAddingToDragSession (UITableView tableView, IUIDragSession session, NSIndexPath indexPath, CGPoint point);

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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
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
	[iOS (11,0)]
	[BaseType (typeof(UIDropProposal))]
	[DisableDefaultCtor] // NSInternalInconsistencyException Reason: Not implemented
	interface UITableViewDropProposal {

		// inline from base type
		[Export ("initWithDropOperation:")]
		[DesignatedInitializer]
		IntPtr Constructor (UIDropOperation operation);

		[Export ("initWithDropOperation:intent:")]
		IntPtr Constructor (UIDropOperation operation, UITableViewDropIntent intent);

		[Export ("intent")]
		UITableViewDropIntent Intent { get; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UITableViewDropCoordinator {
		[Abstract]
		[Export ("items")]
		IUITableViewDropItem[] Items { get; }

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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UITableViewPlaceholder {
		[Export ("initWithInsertionIndexPath:reuseIdentifier:rowHeight:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier, nfloat rowHeight);

		[NullAllowed, Export ("cellUpdateHandler", ArgumentSemantic.Copy)]
		Action<UITableViewCell> CellUpdateHandler { get; set; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(UITableViewPlaceholder))]
	interface UITableViewDropPlaceholder {
		// inlined
		[Export ("initWithInsertionIndexPath:reuseIdentifier:rowHeight:")]
		IntPtr Constructor (NSIndexPath insertionIndexPath, string reuseIdentifier, nfloat rowHeight);

		[NullAllowed, Export ("previewParametersProvider", ArgumentSemantic.Copy)]
		Func<UITableViewCell, UIDragPreviewParameters> PreviewParametersProvider { get; set; }
	}

	[NoWatch, NoTV, iOS (11,0)]
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

	[NoWatch, NoTV, iOS (11,0)]
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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UITextDragPreviewRenderer {
		[Export ("initWithLayoutManager:range:")]
		IntPtr Constructor (NSLayoutManager layoutManager, NSRange range);

		[Export ("initWithLayoutManager:range:unifyRects:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSLayoutManager layoutManager, NSRange range, bool unifyRects);

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
	[iOS (11,0)]
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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UITextDragDelegate {
		[Export ("textDraggableView:itemsForDrag:")]
		UIDragItem[] GetItemsForDrag (IUITextDraggable textDraggableView, IUITextDragRequest dragRequest);

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
	[iOS (11,0)]
	[Protocol]
	interface UITextDragRequest {
		[Abstract]
		[Export ("dragRange")]
		UITextRange DragRange { get; }

		[Abstract]
		[Export ("suggestedItems")]
		UIDragItem[] SuggestedItems { get; }

		[Abstract]
		[Export ("existingItems")]
		UIDragItem[] ExistingItems { get; }

		[Abstract]
		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; }

		[Abstract]
		[Export ("dragSession")]
		IUIDragSession DragSession { get; }
	}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(UIDropProposal))]
	[DisableDefaultCtor]
	interface UITextDropProposal : NSCopying {
		// inlined
		[Export ("initWithDropOperation:")]
		IntPtr Constructor (UIDropOperation operation);

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
	[iOS (11,0)]
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
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
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
	[iOS (11,0)]
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

#endregion

	[TV (11,0), iOS (11,0)]
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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UISpringLoadedInteraction : UIInteraction {
		[Export ("initWithInteractionBehavior:interactionEffect:activationHandler:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] IUISpringLoadedInteractionBehavior interactionBehavior, [NullAllowed] IUISpringLoadedInteractionEffect interactionEffect, Action<UISpringLoadedInteraction, IUISpringLoadedInteractionContext> handler);

		[Export ("initWithActivationHandler:")]
		IntPtr Constructor (Action<UISpringLoadedInteraction, IUISpringLoadedInteractionContext> handler);

		[Export ("interactionBehavior", ArgumentSemantic.Strong)]
		IUISpringLoadedInteractionBehavior InteractionBehavior { get; }

		[Export ("interactionEffect", ArgumentSemantic.Strong)]
		IUISpringLoadedInteractionEffect InteractionEffect { get; }
	}

	interface IUISpringLoadedInteractionBehavior {}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UISpringLoadedInteractionBehavior {
		[Abstract]
		[Export ("shouldAllowInteraction:withContext:")]
		bool ShouldAllowInteraction (UISpringLoadedInteraction interaction, IUISpringLoadedInteractionContext context);

		[Export ("interactionDidFinish:")]
		void InteractionDidFinish (UISpringLoadedInteraction interaction);
	}

	interface IUISpringLoadedInteractionEffect {}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UISpringLoadedInteractionEffect {
		[Abstract]
		[Export ("interaction:didChangeWithContext:")]
		void DidChange (UISpringLoadedInteraction interaction, IUISpringLoadedInteractionContext context);
	}

	interface IUISpringLoadedInteractionContext {}

	[NoWatch, NoTV]
	[iOS (11,0)]
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
	[iOS (11,0)]
	[Protocol]
	interface UISpringLoadedInteractionSupporting {
		[Abstract]
		[Export ("springLoaded")]
		bool SpringLoaded { [Bind ("isSpringLoaded")] get; set; }
	}

	// https://bugzilla.xamarin.com/show_bug.cgi?id=58282, we should be able to write one delegate with a 'Action<bool>'. See original signature:
	// typedef void (^UIContextualActionHandler)(UIContextualAction * _Nonnull, __kindof UIView * _Nonnull, void (^ _Nonnull)(BOOL));
	[NoWatch, NoTV]
	[iOS (11,0)]
	delegate void UIContextualActionHandler (UIContextualAction action, UIView sourceView, [BlockCallback] UIContextualActionCompletionHandler completionHandler);

	[NoWatch, NoTV]
	[iOS (11,0)]
	delegate void UIContextualActionCompletionHandler (bool finished);

	[NoWatch, NoTV]
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UISwipeActionsConfiguration {
		[Static]
		[Export ("configurationWithActions:")]
		UISwipeActionsConfiguration FromActions (UIContextualAction[] actions);

		[Export ("actions", ArgumentSemantic.Copy)]
		UIContextualAction[] Actions { get; }

		[Export ("performsFirstActionWithFullSwipe")]
		bool PerformsFirstActionWithFullSwipe { get; set; }
	}

	interface IUITextPasteConfigurationSupporting {}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UITextPasteConfigurationSupporting : UIPasteConfigurationSupporting {
		[Abstract]
		[NullAllowed, Export ("pasteDelegate", ArgumentSemantic.Weak)]
		IUITextPasteDelegate PasteDelegate { get; set; }
	}

	interface IUITextPasteDelegate {}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UITextPasteDelegate {
		[Export ("textPasteConfigurationSupporting:transformPasteItem:")]
		void TransformPasteItem (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, IUITextPasteItem item);

		[Export ("textPasteConfigurationSupporting:combineItemAttributedStrings:forRange:")]
		NSAttributedString CombineItemAttributedStrings (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString[] itemStrings, UITextRange textRange);

		[Export ("textPasteConfigurationSupporting:performPasteOfAttributedString:toRange:")]
		UITextRange PerformPaste (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString attributedString, UITextRange textRange);

		[Export ("textPasteConfigurationSupporting:shouldAnimatePasteOfAttributedString:toRange:")]
		bool ShouldAnimatePaste (IUITextPasteConfigurationSupporting textPasteConfigurationSupporting, NSAttributedString attributedString, UITextRange textRange);
	}

	interface IUITextPasteItem {}

	[NoWatch, NoTV]
	[iOS (11,0)]
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
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DesignatedDefaultCtor]
	interface UIPasteConfiguration : NSSecureCoding, NSCopying {
		[Export ("acceptableTypeIdentifiers", ArgumentSemantic.Copy)]
		string[] AcceptableTypeIdentifiers { get; set; }

		[Export ("initWithAcceptableTypeIdentifiers:")]
		IntPtr Constructor (string[] acceptableTypeIdentifiers);

		[Export ("addAcceptableTypeIdentifiers:")]
		void AddAcceptableTypeIdentifiers (string[] acceptableTypeIdentifiers);

		[Export ("initWithTypeIdentifiersForAcceptingClass:")]
		IntPtr Constructor (Class itemProviderReadingClass);

		[Wrap ("this (new Class (itemProviderReadingType))")]
		IntPtr Constructor (Type itemProviderReadingType);

		[Export ("addTypeIdentifiersForAcceptingClass:")]
		void AddTypeIdentifiers (Class itemProviderReadingClass);

		[Wrap ("AddTypeIdentifiers (new Class (itemProviderReadingType))")]
		void AddTypeIdentifiers (Type itemProviderReadingType);
	}

	interface IUIPasteConfigurationSupporting {}

	[NoWatch, NoTV]
	[iOS (11,0)]
	[Protocol]
	interface UIPasteConfigurationSupporting {
		[Abstract]
		[NullAllowed, Export ("pasteConfiguration", ArgumentSemantic.Copy)]
		UIPasteConfiguration PasteConfiguration { get; set; }

		[Export ("pasteItemProviders:")]
		void Paste (NSItemProvider[] itemProviders);

		[Export ("canPasteItemProviders:")]
		bool CanPaste (NSItemProvider[] itemProviders);
	}

	[NoTV, NoWatch]
	[iOS (11,0)]
	[BaseType (typeof(UIViewController))]
	interface UIDocumentBrowserViewController : NSCoding {
		[Export ("initForOpeningFilesWithContentTypes:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] string[] allowedContentTypes);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		IUIDocumentBrowserViewControllerDelegate Delegate { get; set; }

		[Export ("allowsDocumentCreation")]
		bool AllowsDocumentCreation { get; set; }

		[Export ("allowsPickingMultipleItems")]
		bool AllowsPickingMultipleItems { get; set; }

		[Export ("allowedContentTypes", ArgumentSemantic.Copy)]
		string[] AllowedContentTypes { get; }

		[Export ("additionalLeadingNavigationBarButtonItems", ArgumentSemantic.Strong)]
		UIBarButtonItem[] AdditionalLeadingNavigationBarButtonItems { get; set; }

		[Export ("additionalTrailingNavigationBarButtonItems", ArgumentSemantic.Strong)]
		UIBarButtonItem[] AdditionalTrailingNavigationBarButtonItems { get; set; }

		[Async]
		[Export ("revealDocumentAtURL:importIfNeeded:completion:")]
		void RevealDocument (NSUrl url, bool importIfNeeded, [NullAllowed] Action<NSUrl, NSError> completion);

		[Async]
		[Export ("importDocumentAtURL:nextToDocumentAtURL:mode:completionHandler:")]
		void ImportDocument (NSUrl documentUrl, NSUrl neighbourUrl, UIDocumentBrowserImportMode importMode, Action<NSUrl, NSError> completion);

		[Internal] // got deprecated
		[Export ("transitionControllerForDocumentURL:")]
		UIDocumentBrowserTransitionController _DeprecatedGetTransitionController (NSUrl documentUrl);

		[Export ("customActions", ArgumentSemantic.Strong)]
		UIDocumentBrowserAction[] CustomActions { get; set; }

		[Export ("browserUserInterfaceStyle", ArgumentSemantic.Assign)]
		UIDocumentBrowserUserInterfaceStyle BrowserUserInterfaceStyle { get; set; }

		[Internal]
		[iOS (12,0)]
		[Export ("transitionControllerForDocumentAtURL:")]
		UIDocumentBrowserTransitionController _NewGetTransitionController (NSUrl documentUrl);
	}

	interface IUIDocumentBrowserViewControllerDelegate {}

	[NoTV, NoWatch]
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface UIDocumentBrowserViewControllerDelegate {
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'DidPickDocumentsAtUrls (UIDocumentBrowserViewController, NSUrl[])' instead.")]
		[Export ("documentBrowser:didPickDocumentURLs:")]
		void DidPickDocumentUrls (UIDocumentBrowserViewController controller, NSUrl[] documentUrls);

		[Export ("documentBrowser:didRequestDocumentCreationWithHandler:")]
		void DidRequestDocumentCreation (UIDocumentBrowserViewController controller, Action<NSUrl, UIDocumentBrowserImportMode> importHandler);

		[Export ("documentBrowser:didImportDocumentAtURL:toDestinationURL:")]
		void DidImportDocument (UIDocumentBrowserViewController controller, NSUrl sourceUrl, NSUrl destinationUrl);

		[Export ("documentBrowser:failedToImportDocumentAtURL:error:")]
		void FailedToImportDocument (UIDocumentBrowserViewController controller, NSUrl documentUrl, [NullAllowed] NSError error);

		[Export ("documentBrowser:applicationActivitiesForDocumentURLs:")]
		UIActivity[] GetApplicationActivities (UIDocumentBrowserViewController controller, NSUrl[] documentUrls);

		[Export ("documentBrowser:willPresentActivityViewController:")]
		void WillPresent (UIDocumentBrowserViewController controller, UIActivityViewController activityViewController);

		[iOS (12,0)]
		[Export ("documentBrowser:didPickDocumentsAtURLs:")]
		void DidPickDocumentsAtUrls (UIDocumentBrowserViewController controller, NSUrl[] documentUrls);
	}

	[NoTV, NoWatch]
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDocumentBrowserTransitionController : UIViewControllerAnimatedTransitioning {
		[NullAllowed, Export ("loadingProgress", ArgumentSemantic.Strong)]
		NSProgress LoadingProgress { get; set; }

		[NullAllowed, Export ("targetView", ArgumentSemantic.Weak)]
		UIView TargetView { get; set; }
	}

	[NoTV, NoWatch]
	[iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface UIDocumentBrowserAction {
		[Export ("initWithIdentifier:localizedTitle:availability:handler:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string localizedTitle, UIDocumentBrowserActionAvailability availability, Action<NSUrl[]> handler);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("localizedTitle")]
		string LocalizedTitle { get; }

		[Export ("availability")]
		UIDocumentBrowserActionAvailability Availability { get; }

		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		UIImage Image { get; set; }

		[Export ("supportedContentTypes", ArgumentSemantic.Copy)]
		string[] SupportedContentTypes { get; set; }

		[Export ("supportsMultipleItems")]
		bool SupportsMultipleItems { get; set; }
	}

	interface IUIFocusItemContainer {}
	[iOS (12,0), TV (12,0), NoWatch]
	[Protocol]
	interface UIFocusItemContainer
	{
		[Abstract]
		[Export ("coordinateSpace")]
		IUICoordinateSpace CoordinateSpace { get; }

		[Abstract]
		[Export ("focusItemsInRect:")]
		IUIFocusItem[] GetFocusItems (CGRect rect);
	}

	[iOS (12,0), TV(12,0), NoWatch]
	[Protocol]
	interface UIFocusItemScrollableContainer : UIFocusItemContainer
	{
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

	[iOS (8,0), NoWatch] // it was added on 8,0, but was not binded and the method was added in 12,0
	[Protocol]
	interface UIUserActivityRestoring
	{
		[Abstract]
		[iOS (8,0), TV(12,0)]
		[Export ("restoreUserActivityState:")]
		void RestoreUserActivityState (NSUserActivity activity);
	}

#endif // !WATCH

	[Watch (4,0), TV (11,0), iOS (11,0)]
	[BaseType (typeof(NSObject))]
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
		IntPtr Constructor (string textStyle);

		[Export ("scaledFontForFont:")]
		UIFont GetScaledFont (UIFont font);

		[Export ("scaledFontForFont:maximumPointSize:")]
		UIFont GetScaledFont (UIFont font, nfloat maximumPointSize);

		[Export ("scaledValueForValue:")]
		nfloat GetScaledValue (nfloat value);

#if !WATCH
		[NoWatch]
		[Export ("scaledFontForFont:compatibleWithTraitCollection:")]
		UIFont GetScaledFont (UIFont font, [NullAllowed] UITraitCollection traitCollection);

		[NoWatch]
		[Export ("scaledFontForFont:maximumPointSize:compatibleWithTraitCollection:")]
		UIFont GetScaledFont (UIFont font, nfloat maximumPointSize, [NullAllowed] UITraitCollection traitCollection);

		[NoWatch]
		[Export ("scaledValueForValue:compatibleWithTraitCollection:")]
		nfloat GetScaledValue (nfloat value, [NullAllowed] UITraitCollection traitCollection);
#endif // !WATCH
	}

#if !WATCH
	[iOS (12,1)]
	[NoWatch][NoTV]
	[Native]
	public enum UIPencilPreferredAction : long {
		Ignore = 0,
		SwitchEraser,
		SwitchPrevious,
		ShowColorPalette,
	}

	[iOS (12,1)]
	[NoWatch][NoTV]
	[BaseType (typeof (NSObject))]
	interface UIPencilInteraction : UIInteraction {
		[Static]
		[Export ("preferredTapAction")]
		UIPencilPreferredAction PreferredTapAction { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IUIPencilInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	}

	interface IUIPencilInteractionDelegate {}

	[iOS (12,1)]
	[NoWatch][NoTV]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface UIPencilInteractionDelegate {
		[Export ("pencilInteractionDidTap:")]
		void DidTap (UIPencilInteraction interaction);
	}
#endif // !WATCH
}
