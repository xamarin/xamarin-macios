using MonoMac.AVFoundation;
using MonoMac.AddressBook;
using MonoMac.AppKit;
using MonoMac.AudioToolbox;
using MonoMac.AudioUnit;
using MonoMac.AudioUnitWrapper;
using MonoMac.Constants;
using MonoMac.CoreAnimation;
using MonoMac.CoreBluetooth;
using MonoMac.CoreData;
using MonoMac.CoreFoundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.CoreLocation;
using MonoMac.CoreMedia;
using MonoMac.CoreMidi;
using MonoMac.CoreServices;
using MonoMac.CoreText;
using MonoMac.CoreVideo;
using MonoMac.CoreWlan;
using MonoMac.Darwin;
using MonoMac.Foundation;
using MonoMac.GameKit;
using MonoMac.Growl;
using MonoMac.ImageIO;
using MonoMac.ImageKit;
using MonoMac.ObjCRuntime;
using MonoMac.OpenAL;
using MonoMac.OpenGL;
using MonoMac.PdfKit;
using MonoMac.QTKit;
using MonoMac.QuartzComposer;
using MonoMac.QuickLook;
using MonoMac.SceneKit;
using MonoMac.ScriptingBridge;
using MonoMac.Security;
using MonoMac.StoreKit;
using MonoMac.WebKit;
using MonoTouch;
using MonoTouch.AVFoundation;
using MonoTouch.Accelerate;
using MonoTouch.Accounts;
using MonoTouch.AdSupport;
using MonoTouch.AddressBook;
using MonoTouch.AddressBookUI;
using MonoTouch.AssetsLibrary;
using MonoTouch.AudioToolbox;
using MonoTouch.AudioUnit;
using MonoTouch.AudioUnitWrapper;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreBluetooth;
using MonoTouch.CoreData;
using MonoTouch.CoreFoundation;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.CoreLocation;
using MonoTouch.CoreMedia;
using MonoTouch.CoreMidi;
using MonoTouch.CoreMotion;
using MonoTouch.CoreServices;
using MonoTouch.CoreTelephony;
using MonoTouch.CoreText;
using MonoTouch.CoreVideo;
using MonoTouch.EventKit;
using MonoTouch.EventKitUI;
using MonoTouch.ExternalAccessory;
using MonoTouch.Foundation;
using MonoTouch.GLKit;
using MonoTouch.GameController;
using MonoTouch.GameKit;
using MonoTouch.ImageIO;
using MonoTouch.JavaScriptCore;
using MonoTouch.MapKit;
using MonoTouch.MediaAccessibility;
using MonoTouch.MediaPlayer;
using MonoTouch.MediaToolbox;
using MonoTouch.MessageUI;
using MonoTouch.MobileCoreServices;
using MonoTouch.MultipeerConnectivity;
using MonoTouch.NewsstandKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.PassKit;
using MonoTouch.QuickLook;
using MonoTouch.Registrar;
using MonoTouch.SafariServices;
using MonoTouch.Security;
using MonoTouch.Social;
using MonoTouch.SpriteKit;
using MonoTouch.StoreKit;
using MonoTouch.SystemConfiguration;
using MonoTouch.Twitter;
using MonoTouch.UIKit;
using MonoTouch.iAd;
using OpenTK;
using System.Drawing;
using System.Reflection.Emit;
namespace Test {
	[Protocol] interface C1 : ICFType {}
	[Protocol] interface C2 : INativeObject {}
	[Protocol] interface C3 : IUITextInputTraits {}
	[Protocol] interface C4 : IUIGuidedAccessRestrictionDelegate {}
	[Protocol] interface C5 : IGLKNamedEffect {}
	[Protocol] interface C6 : IGLKViewDelegate {}
	[Protocol] interface C7 : IGLKViewControllerDelegate {}
	[Protocol] interface C8 : IMPMediaPickerControllerDelegate {}
	[Protocol] interface C9 : IMFMailComposeViewControllerDelegate {}
	[Protocol] interface C10 : IMFMessageComposeViewControllerDelegate {}
	[Protocol] interface C11 : INSTextAttachmentContainer {}
	[Protocol] interface C12 : INSTextStorageDelegate {}
	[Protocol] interface C13 : INSLayoutManagerDelegate {}
	[Protocol] interface C14 : IUIAccelerometerDelegate {}
	[Protocol] interface C15 : IUIActionSheetDelegate {}
	[Protocol] interface C16 : IUIActivityItemSource {}
	[Protocol] interface C17 : IUIAlertViewDelegate {}
	[Protocol] interface C18 : IUIAppearance {}
	[Protocol] interface C19 : IUIStateRestoring {}
	[Protocol] interface C20 : IUIApplicationDelegate {}
	[Protocol] interface C21 : IUICollectionViewSource {}
	[Protocol] interface C22 : IUICollectionViewDataSource {}
	[Protocol] interface C23 : IUICollectionViewDelegate {}
	[Protocol] interface C24 : IUICollectionViewDelegateFlowLayout {}
	[Protocol] interface C25 : IUICollisionBehaviorDelegate {}
	[Protocol] interface C26 : IUIDynamicAnimatorDelegate {}
	[Protocol] interface C27 : IUIDynamicItem {}
	[Protocol] interface C28 : IUIGestureRecognizerDelegate {}
	[Protocol] interface C29 : IUITextInputTokenizer {}
	[Protocol] interface C30 : IUITextInputDelegate {}
	[Protocol] interface C31 : IUIBarPositioning {}
	[Protocol] interface C32 : IUIBarPositioningDelegate {}
	[Protocol] interface C33 : IUIDocumentInteractionControllerDelegate {}
	[Protocol] interface C34 : IUIImagePickerControllerDelegate {}
	[Protocol] interface C35 : IUINavigationBarDelegate {}
	[Protocol] interface C36 : IUINavigationControllerDelegate {}
	[Protocol] interface C37 : IUIPageViewControllerDelegate {}
	[Protocol] interface C38 : IUIPageViewControllerDataSource {}
	[Protocol] interface C39 : IUIPickerViewDelegate {}
	[Protocol] interface C40 : IUIPickerViewDataSource {}
	[Protocol] interface C41 : IUIScrollViewDelegate {}
	[Protocol] interface C42 : IUISearchBarDelegate {}
	[Protocol] interface C43 : IUISearchDisplayDelegate {}
	[Protocol] interface C44 : IUITabBarDelegate {}
	[Protocol] interface C45 : IUITabBarControllerDelegate {}
	[Protocol] interface C46 : IUITableViewDataSource {}
	[Protocol] interface C47 : IUITableViewDelegate {}
	[Protocol] interface C48 : IUITextFieldDelegate {}
	[Protocol] interface C49 : IUITextViewDelegate {}
	[Protocol] interface C50 : IUIToolbarDelegate {}
	[Protocol] interface C51 : IUIVideoEditorControllerDelegate {}
	[Protocol] interface C52 : IUIViewControllerContextTransitioning {}
	[Protocol] interface C53 : IUIViewControllerAnimatedTransitioning {}
	[Protocol] interface C54 : IUIViewControllerInteractiveTransitioning {}
	[Protocol] interface C55 : IUIViewControllerTransitioningDelegate {}
	[Protocol] interface C57 : IUIViewControllerTransitionCoordinatorContext {}
	[Protocol] interface C58 : IUIViewControllerTransitionCoordinator {}
	[Protocol] interface C59 : IUIWebViewDelegate {}
	[Protocol] interface C60 : IUISplitViewControllerDelegate {}
	[Protocol] interface C61 : IUIPopoverControllerDelegate {}
	[Protocol] interface C62 : IUIPrintInteractionControllerDelegate {}
	[Protocol] interface C63 : IUILayoutSupport {}
	[Protocol] interface C64 : IMKAnnotation {}
	[Protocol] interface C65 : IMKOverlay {}
	[Protocol] interface C66 : IMKMapViewDelegate {}
	[Protocol] interface C67 : IMKReverseGeocoderDelegate {}
	[Protocol] interface C68 : IMKLocalSearch {}
	[Protocol] interface C69 : IMKLocalSearchRequest {}
	[Protocol] interface C70 : IMKLocalSearchResponse {}
	[Protocol] interface C71 : IABNewPersonViewControllerDelegate {}
	[Protocol] interface C72 : IABPeoplePickerNavigationControllerDelegate {}
	[Protocol] interface C73 : IABPersonViewControllerDelegate {}
	[Protocol] interface C74 : IABUnknownPersonViewControllerDelegate {}
	[Protocol] interface C75 : IEAAccessoryDelegate {}
	[Protocol] interface C76 : IADBannerViewDelegate {}
	[Protocol] interface C77 : IADInterstitialAdDelegate {}
	[Protocol] interface C78 : IEKEventViewDelegate {}
	[Protocol] interface C79 : IEKEventEditViewDelegate {}
	[Protocol] interface C80 : IEKCalendarChooserDelegate {}
	[Protocol] interface C81 : IPKAddPassesViewControllerDelegate {}
	[Protocol] interface C82 : ISKPhysicsContactDelegate {}
	[Protocol] interface C83 : IMCSessionDelegate {}
	[Protocol] interface C84 : IMCNearbyServiceAdvertiserDelegate {}
	[Protocol] interface C85 : IMCNearbyServiceBrowserDelegate {}
	[Protocol] interface C86 : IMCBrowserViewControllerDelegate {}
	[Protocol] interface C87 : IMCAdvertiserAssistantDelegate {}
	[Protocol] interface C88 : IAVAudioPlayerDelegate {}
	[Protocol] interface C89 : IAVAudioRecorderDelegate {}
	[Protocol] interface C90 : IAVAudioSessionDelegate {}
	[Protocol] interface C91 : IAVAssetResourceLoaderDelegate {}
	[Protocol] interface C92 : IAVVideoCompositing {}
	[Protocol] interface C93 : IAVVideoCompositionValidationHandling {}
	[Protocol] interface C94 : IAVCaptureVideoDataOutputSampleBufferDelegate {}
	[Protocol] interface C95 : IAVCaptureAudioDataOutputSampleBufferDelegate {}
	[Protocol] interface C96 : IAVCaptureFileOutputRecordingDelegate {}
	[Protocol] interface C97 : IAVCaptureMetadataOutputObjectsDelegate {}
	[Protocol] interface C98 : IAVPlayerItemOutputPullDelegate {}
	[Protocol] interface C99 : IAVPlayerItemOutputPushDelegate {}
	[Protocol] interface C100 : IAVPlayerItemLegibleOutputPushDelegate {}
	[Protocol] interface C101 : IAVAsynchronousKeyValueLoading {}
	[Protocol] interface C102 : IAVSpeechSynthesizerDelegate {}
	[Protocol] interface C103 : ICAMediaTiming {}
	[Protocol] interface C104 : ICALayerDelegate {}
	[Protocol] interface C105 : ICAAction {}
	[Protocol] interface C106 : ICBCentralManagerDelegate {}
	[Protocol] interface C107 : ICBPeripheralDelegate {}
	[Protocol] interface C108 : ICBPeripheralManagerDelegate {}
	[Protocol] interface C109 : INSFetchedResultsControllerDelegate {}
	[Protocol] interface C110 : INSFetchedResultsSectionInfo {}
	[Protocol] interface C111 : INSCacheDelegate {}
	[Protocol] interface C112 : INSCoding {}
	[Protocol] interface C113 : INSSecureCoding {}
	[Protocol] interface C114 : INSCopying {}
	[Protocol] interface C115 : INSMutableCopying {}
	[Protocol] interface C116 : INSKeyedArchiverDelegate {}
	[Protocol] interface C117 : INSKeyedUnarchiverDelegate {}
	[Protocol] interface C118 : INSMetadataQueryDelegate {}
	[Protocol] interface C119 : INSUrlConnectionDelegate {}
	[Protocol] interface C120 : INSUrlConnectionDownloadDelegate {}
	[Protocol] interface C121 : INSUrlSessionDelegate {}
	[Protocol] interface C122 : INSUrlSessionTaskDelegate {}
	[Protocol] interface C123 : INSUrlSessionDataDelegate {}
	[Protocol] interface C124 : INSUrlSessionDownloadDelegate {}
	[Protocol] interface C125 : INSStreamDelegate {}
	[Protocol] interface C126 : INSNetServiceDelegate {}
	[Protocol] interface C127 : INSNetServiceBrowserDelegate {}
	[Protocol] interface C128 : INSPortDelegate {}
	[Protocol] interface C129 : INSMachPortDelegate {}
	[Protocol] interface C130 : INSFileManagerDelegate {}
	[Protocol] interface C131 : INSFilePresenter {}
	[Protocol] interface C132 : IGKPeerPickerControllerDelegate {}
	[Protocol] interface C133 : IGKVoiceChatClient {}
	[Protocol] interface C134 : IGKSessionDelegate {}
	[Protocol] interface C135 : IGKLeaderboardViewControllerDelegate {}
	[Protocol] interface C136 : IGKMatchDelegate {}
	[Protocol] interface C137 : IGKMatchmakerViewControllerDelegate {}
	[Protocol] interface C138 : IGKAchievementViewControllerDelegate {}
	[Protocol] interface C139 : IGKFriendRequestComposeViewControllerDelegate {}
	[Protocol] interface C140 : IGKTurnBasedEventHandlerDelegate {}
	[Protocol] interface C141 : IGKTurnBasedMatchmakerViewControllerDelegate {}
	[Protocol] interface C142 : IGKGameCenterControllerDelegate {}
	[Protocol] interface C143 : IGKChallengeEventHandlerDelegate {}
	[Protocol] interface C144 : IGKLocalPlayerListener {}
	[Protocol] interface C145 : IGKChallengeListener {}
	[Protocol] interface C146 : IGKInviteEventListener {}
	[Protocol] interface C147 : IGKTurnBasedEventListener {}
	[Protocol] interface C148 : IQLPreviewControllerDataSource {}
	[Protocol] interface C149 : IQLPreviewControllerDelegate {}
	[Protocol] interface C150 : IQLPreviewItem {}
	[Protocol] interface C151 : ISKPaymentTransactionObserver {}
	[Protocol] interface C152 : ISKRequestDelegate {}
	[Protocol] interface C153 : ISKProductsRequestDelegate {}
	[Protocol] interface C154 : ISKStoreProductViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M1 : ICFType {}
	[Protocol] [BaseType (typeof (NSObject))] interface M2 : INativeObject {}
	[Protocol] [BaseType (typeof (NSObject))] interface M3 : IUITextInputTraits {}
	[Protocol] [BaseType (typeof (NSObject))] interface M4 : IUIGuidedAccessRestrictionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M5 : IGLKNamedEffect {}
	[Protocol] [BaseType (typeof (NSObject))] interface M6 : IGLKViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M7 : IGLKViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M8 : IMPMediaPickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M9 : IMFMailComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M10 : IMFMessageComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M11 : INSTextAttachmentContainer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M12 : INSTextStorageDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M13 : INSLayoutManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M14 : IUIAccelerometerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M15 : IUIActionSheetDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M16 : IUIActivityItemSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M17 : IUIAlertViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M18 : IUIAppearance {}
	[Protocol] [BaseType (typeof (NSObject))] interface M19 : IUIStateRestoring {}
	[Protocol] [BaseType (typeof (NSObject))] interface M20 : IUIApplicationDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M21 : IUICollectionViewSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M22 : IUICollectionViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M23 : IUICollectionViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M24 : IUICollectionViewDelegateFlowLayout {}
	[Protocol] [BaseType (typeof (NSObject))] interface M25 : IUICollisionBehaviorDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M26 : IUIDynamicAnimatorDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M27 : IUIDynamicItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M28 : IUIGestureRecognizerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M29 : IUITextInputTokenizer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M30 : IUITextInputDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M31 : IUIBarPositioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M32 : IUIBarPositioningDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M33 : IUIDocumentInteractionControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M34 : IUIImagePickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M35 : IUINavigationBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M36 : IUINavigationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M37 : IUIPageViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M38 : IUIPageViewControllerDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M39 : IUIPickerViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M40 : IUIPickerViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M41 : IUIScrollViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M42 : IUISearchBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M43 : IUISearchDisplayDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M44 : IUITabBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M45 : IUITabBarControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M46 : IUITableViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M47 : IUITableViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M48 : IUITextFieldDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M49 : IUITextViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M50 : IUIToolbarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M51 : IUIVideoEditorControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M52 : IUIViewControllerContextTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M53 : IUIViewControllerAnimatedTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M54 : IUIViewControllerInteractiveTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M55 : IUIViewControllerTransitioningDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M57 : IUIViewControllerTransitionCoordinatorContext {}
	[Protocol] [BaseType (typeof (NSObject))] interface M58 : IUIViewControllerTransitionCoordinator {}
	[Protocol] [BaseType (typeof (NSObject))] interface M59 : IUIWebViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M60 : IUISplitViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M61 : IUIPopoverControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M62 : IUIPrintInteractionControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M63 : IUILayoutSupport {}
	[Protocol] [BaseType (typeof (NSObject))] interface M64 : IMKAnnotation {}
	[Protocol] [BaseType (typeof (NSObject))] interface M65 : IMKOverlay {}
	[Protocol] [BaseType (typeof (NSObject))] interface M66 : IMKMapViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M67 : IMKReverseGeocoderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M68 : IMKLocalSearch {}
	[Protocol] [BaseType (typeof (NSObject))] interface M69 : IMKLocalSearchRequest {}
	[Protocol] [BaseType (typeof (NSObject))] interface M70 : IMKLocalSearchResponse {}
	[Protocol] [BaseType (typeof (NSObject))] interface M71 : IABNewPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M72 : IABPeoplePickerNavigationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M73 : IABPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M74 : IABUnknownPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M75 : IEAAccessoryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M76 : IADBannerViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M77 : IADInterstitialAdDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M78 : IEKEventViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M79 : IEKEventEditViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M80 : IEKCalendarChooserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M81 : IPKAddPassesViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M82 : ISKPhysicsContactDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M83 : IMCSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M84 : IMCNearbyServiceAdvertiserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M85 : IMCNearbyServiceBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M86 : IMCBrowserViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M87 : IMCAdvertiserAssistantDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M88 : IAVAudioPlayerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M89 : IAVAudioRecorderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M90 : IAVAudioSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M91 : IAVAssetResourceLoaderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M92 : IAVVideoCompositing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M93 : IAVVideoCompositionValidationHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M94 : IAVCaptureVideoDataOutputSampleBufferDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M95 : IAVCaptureAudioDataOutputSampleBufferDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M96 : IAVCaptureFileOutputRecordingDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M97 : IAVCaptureMetadataOutputObjectsDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M98 : IAVPlayerItemOutputPullDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M99 : IAVPlayerItemOutputPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M100 : IAVPlayerItemLegibleOutputPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M101 : IAVAsynchronousKeyValueLoading {}
	[Protocol] [BaseType (typeof (NSObject))] interface M102 : IAVSpeechSynthesizerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M103 : ICAMediaTiming {}
	[Protocol] [BaseType (typeof (NSObject))] interface M104 : ICALayerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M105 : ICAAction {}
	[Protocol] [BaseType (typeof (NSObject))] interface M106 : ICBCentralManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M107 : ICBPeripheralDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M108 : ICBPeripheralManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M109 : INSFetchedResultsControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M110 : INSFetchedResultsSectionInfo {}
	[Protocol] [BaseType (typeof (NSObject))] interface M111 : INSCacheDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M112 : INSCoding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M113 : INSSecureCoding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M114 : INSCopying {}
	[Protocol] [BaseType (typeof (NSObject))] interface M115 : INSMutableCopying {}
	[Protocol] [BaseType (typeof (NSObject))] interface M116 : INSKeyedArchiverDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M117 : INSKeyedUnarchiverDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M118 : INSMetadataQueryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M119 : INSUrlConnectionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M120 : INSUrlConnectionDownloadDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M121 : INSUrlSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M122 : INSUrlSessionTaskDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M123 : INSUrlSessionDataDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M124 : INSUrlSessionDownloadDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M125 : INSStreamDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M126 : INSNetServiceDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M127 : INSNetServiceBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M128 : INSPortDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M129 : INSMachPortDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M130 : INSFileManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M131 : INSFilePresenter {}
	[Protocol] [BaseType (typeof (NSObject))] interface M132 : IGKPeerPickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M133 : IGKVoiceChatClient {}
	[Protocol] [BaseType (typeof (NSObject))] interface M134 : IGKSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M135 : IGKLeaderboardViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M136 : IGKMatchDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M137 : IGKMatchmakerViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M138 : IGKAchievementViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M139 : IGKFriendRequestComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M140 : IGKTurnBasedEventHandlerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M141 : IGKTurnBasedMatchmakerViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M142 : IGKGameCenterControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M143 : IGKChallengeEventHandlerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M144 : IGKLocalPlayerListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M145 : IGKChallengeListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M146 : IGKInviteEventListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M147 : IGKTurnBasedEventListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M148 : IQLPreviewControllerDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M149 : IQLPreviewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M150 : IQLPreviewItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M151 : ISKPaymentTransactionObserver {}
	[Protocol] [BaseType (typeof (NSObject))] interface M152 : ISKRequestDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M153 : ISKProductsRequestDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M154 : ISKStoreProductViewControllerDelegate {}
}
