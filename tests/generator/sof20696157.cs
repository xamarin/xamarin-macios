using AVFoundation;
using AVKit;
using Accelerate;
using Accounts;
using AdSupport;
using AddressBook;
using AddressBookUI;
using AssetsLibrary;
using AudioToolbox;
using AudioUnit;
using CallKit;
using CloudKit;
using Contacts;
using ContactsUI;
using CoreAnimation;
using CoreAudioKit;
using CoreBluetooth;
using CoreData;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreLocation;
using CoreMedia;
using CoreMidi;
using CoreMotion;
using CoreServices;
using CoreSpotlight;
using CoreTelephony;
using CoreText;
using CoreVideo;
using EventKit;
using EventKitUI;
using ExternalAccessory;
using Foundation;
using GLKit;
using GameController;
using GameKit;
using GameplayKit;
using HealthKit;
using HealthKitUI;
using HomeKit;
using ImageIO;
using Intents;
using IntentsUI;
using JavaScriptCore;
using LocalAuthentication;
using MapKit;
using MediaAccessibility;
using MediaPlayer;
using MediaToolbox;
using MessageUI;
using Messages;
using Metal;
using MetalKit;
using MetalPerformanceShaders;
using MobileCoreServices;
using ModelIO;
using MultipeerConnectivity;
using NetworkExtension;
using NewsstandKit;
using NotificationCenter;
using ObjCRuntime;
using OpenGLES;
using OpenTK;
using PassKit;
using Photos;
using PhotosUI;
using PushKit;
using QuickLook;
using Registrar;
using ReplayKit;
using SafariServices;
using SceneKit;
using Security;
using Social;
using Speech;
using SpriteKit;
using StoreKit;
using System;
using System.Drawing;
using SystemConfiguration;
using Twitter;
using UIKit;
using UserNotifications;
using UserNotificationsUI;
using VideoSubscriberAccount;
using VideoToolbox;
using WatchConnectivity;
using WatchKit;
using WebKit;
using iAd;
namespace Test {
	[Protocol] interface C1 : IMFMailComposeViewControllerDelegate {}
	[Protocol] interface C2 : IGKTurnBasedEventListener {}
	[Protocol] interface C3 : IINIntentHandlerProviding {}
	[Protocol] interface C4 : IMFMessageComposeViewControllerDelegate {}
	[Protocol] interface C5 : IUIPopoverControllerDelegate {}
	[Protocol] interface C6 : IUIPopoverPresentationControllerDelegate {}
	[Protocol] interface C7 : IGKTurnBasedMatchmakerViewControllerDelegate {}
	[Protocol] interface C8 : IINListRideOptionsIntentHandling {}
	[Protocol] interface C9 : IUIPreviewActionItem {}
	[Protocol] interface C10 : IMTLBlitCommandEncoder {}
	[Protocol] interface C11 : IGKVoiceChatClient {}
	[Protocol] interface C12 : IMTLBuffer {}
	[Protocol] interface C13 : ISKViewDelegate {}
	[Protocol] interface C14 : IUIPreviewInteractionDelegate {}
	[Protocol] interface C15 : ISKWarpable {}
	[Protocol] interface C16 : IINMessagesDomainHandling {}
	[Protocol] interface C17 : IINPauseWorkoutIntentHandling {}
	[Protocol] interface C18 : IMTLCommandBuffer {}
	[Protocol] interface C19 : IAVPlayerItemLegibleOutputPushDelegate {}
	[Protocol] interface C20 : IUIPrinterPickerControllerDelegate {}
	[Protocol] interface C21 : IGKAgentDelegate {}
	[Protocol] interface C22 : IMTLCommandEncoder {}
	[Protocol] interface C23 : IMTLCommandQueue {}
	[Protocol] interface C24 : IAVPlayerItemMetadataCollectorPushDelegate {}
	[Protocol] interface C25 : IINPaymentsDomainHandling {}
	[Protocol] interface C26 : ICFType {}
	[Protocol] interface C27 : IEKCalendarChooserDelegate {}
	[Protocol] interface C28 : IAVPlayerItemMetadataOutputPushDelegate {}
	[Protocol] interface C29 : ISKCloudServiceSetupViewControllerDelegate {}
	[Protocol] interface C30 : IMTLComputeCommandEncoder {}
	[Protocol] interface C31 : IEKEventEditViewDelegate {}
	[Protocol] interface C32 : IAVPlayerItemOutputPullDelegate {}
	[Protocol] interface C33 : IAVPlayerItemOutputPushDelegate {}
	[Protocol] interface C34 : IEKEventViewDelegate {}
	[Protocol] interface C35 : IMTLComputePipelineState {}
	[Protocol] interface C36 : IUIPrintInteractionControllerDelegate {}
	[Protocol] interface C37 : IEAAccessoryDelegate {}
	[Protocol] interface C38 : ISKPaymentTransactionObserver {}
	[Protocol] interface C39 : IINPhotosDomainHandling {}
	[Protocol] interface C40 : IMTLDepthStencilState {}
	[Protocol] interface C41 : IGKGameModel {}
	[Protocol] interface C42 : IMTLDevice {}
	[Protocol] interface C43 : IGKGameModelPlayer {}
	[Protocol] interface C44 : ISKProductsRequestDelegate {}
	[Protocol] interface C45 : IGKGameModelUpdate {}
	[Protocol] interface C46 : IMTLDrawable {}
	[Protocol] interface C47 : IEAWiFiUnconfiguredAccessoryBrowserDelegate {}
	[Protocol] interface C48 : IINRadioDomainHandling {}
	[Protocol] interface C49 : IMTLFence {}
	[Protocol] interface C50 : INativeObject {}
	[Protocol] interface C51 : IMTLFunction {}
	[Protocol] interface C52 : ISKRequestDelegate {}
	[Protocol] interface C53 : IEAGLDrawable {}
	[Protocol] interface C54 : ISKStoreProductViewControllerDelegate {}
	[Protocol] interface C55 : IMTLHeap {}
	[Protocol] interface C56 : IINRequestPaymentIntentHandling {}
	[Protocol] interface C57 : IAVSpeechSynthesizerDelegate {}
	[Protocol] interface C58 : IMTLLibrary {}
	[Protocol] interface C59 : IINRequestRideIntentHandling {}
	[Protocol] interface C60 : IPKAddPassesViewControllerDelegate {}
	[Protocol] interface C61 : IMTLParallelRenderCommandEncoder {}
	[Protocol] interface C62 : INSCacheDelegate {}
	[Protocol] interface C63 : IPKAddPaymentPassViewControllerDelegate {}
	[Protocol] interface C64 : IUIScrollViewAccessibilityDelegate {}
	[Protocol] interface C65 : IUIScrollViewDelegate {}
	[Protocol] interface C66 : IAVVideoCompositing {}
	[Protocol] interface C67 : IMTLRenderCommandEncoder {}
	[Protocol] interface C68 : INSCoding {}
	[Protocol] interface C69 : IINResumeWorkoutIntentHandling {}
	[Protocol] interface C70 : IAVVideoCompositionValidationHandling {}
	[Protocol] interface C71 : IUISearchBarDelegate {}
	[Protocol] interface C72 : INSCopying {}
	[Protocol] interface C73 : IGKRandom {}
	[Protocol] interface C74 : INSLayoutManagerDelegate {}
	[Protocol] interface C75 : IUISearchControllerDelegate {}
	[Protocol] interface C76 : IMTLRenderPipelineState {}
	[Protocol] interface C77 : IINRidesharingDomainHandling {}
	[Protocol] interface C78 : IPKPaymentAuthorizationControllerDelegate {}
	[Protocol] interface C79 : IMTLResource {}
	[Protocol] interface C80 : IUISearchDisplayDelegate {}
	[Protocol] interface C81 : IUISearchResultsUpdating {}
	[Protocol] interface C82 : IGKSceneRootNodeType {}
	[Protocol] interface C83 : IINSaveProfileInCarIntentHandling {}
	[Protocol] interface C84 : IPKPaymentAuthorizationViewControllerDelegate {}
	[Protocol] interface C85 : INSTextAttachmentContainer {}
	[Protocol] interface C86 : IMTLSamplerState {}
	[Protocol] interface C87 : IINSearchCallHistoryIntentHandling {}
	[Protocol] interface C88 : IAVPictureInPictureControllerDelegate {}
	[Protocol] interface C89 : INSTextLayoutOrientationProvider {}
	[Protocol] interface C90 : IGKStrategist {}
	[Protocol] interface C91 : INSDiscardableContent {}
	[Protocol] interface C92 : IAVPlayerViewControllerDelegate {}
	[Protocol] interface C93 : INSTextStorageDelegate {}
	[Protocol] interface C94 : IINSearchForMessagesIntentHandling {}
	[Protocol] interface C95 : IUISplitViewControllerDelegate {}
	[Protocol] interface C96 : IINSearchForPhotosIntentHandling {}
	[Protocol] interface C97 : IUIAccelerometerDelegate {}
	[Protocol] interface C98 : IMTLTexture {}
	[Protocol] interface C99 : ICXCallDirectoryExtensionContextDelegate {}
	[Protocol] interface C100 : IUIAccessibilityContainer {}
	[Protocol] interface C101 : IINSendMessageIntentHandling {}
	[Protocol] interface C102 : INSExtensionRequestHandling {}
	[Protocol] interface C103 : IUIStateRestoring {}
	[Protocol] interface C104 : IINSendPaymentIntentHandling {}
	[Protocol] interface C105 : ICXCallObserverDelegate {}
	[Protocol] interface C106 : IGLKNamedEffect {}
	[Protocol] interface C107 : IUIAccessibilityIdentification {}
	[Protocol] interface C108 : IINSetAudioSourceInCarIntentHandling {}
	[Protocol] interface C109 : IUIAccessibilityReadingContent {}
	[Protocol] interface C110 : IABNewPersonViewControllerDelegate {}
	[Protocol] interface C111 : INSFileManagerDelegate {}
	[Protocol] interface C112 : IINSetClimateSettingsInCarIntentHandling {}
	[Protocol] interface C113 : ICIFilterConstructor {}
	[Protocol] interface C114 : IABPeoplePickerNavigationControllerDelegate {}
	[Protocol] interface C115 : IUIActionSheetDelegate {}
	[Protocol] interface C116 : INSFilePresenter {}
	[Protocol] interface C117 : IGLKViewControllerDelegate {}
	[Protocol] interface C118 : IGLKViewDelegate {}
	[Protocol] interface C119 : IINSetDefrosterSettingsInCarIntentHandling {}
	[Protocol] interface C120 : ICXProviderDelegate {}
	[Protocol] interface C121 : IABPersonViewControllerDelegate {}
	[Protocol] interface C122 : IABUnknownPersonViewControllerDelegate {}
	[Protocol] interface C123 : IINSetMessageAttributeIntentHandling {}
	[Protocol] interface C124 : IUIActivityItemSource {}
	[Protocol] interface C125 : IUITabBarControllerDelegate {}
	[Protocol] interface C126 : IINSetProfileInCarIntentHandling {}
	[Protocol] interface C127 : IUIAdaptivePresentationControllerDelegate {}
	[Protocol] interface C128 : IUITabBarDelegate {}
	[Protocol] interface C129 : IMTKViewDelegate {}
	[Protocol] interface C130 : IINSetRadioStationIntentHandling {}
	[Protocol] interface C131 : IUIAlertViewDelegate {}
	[Protocol] interface C132 : IINSetSeatSettingsInCarIntentHandling {}
	[Protocol] interface C133 : IUIAppearance {}
	[Protocol] interface C134 : IUIAppearanceContainer {}
	[Protocol] interface C135 : INSKeyedArchiverDelegate {}
	[Protocol] interface C136 : IINSpeakable {}
	[Protocol] interface C137 : IAUAudioUnitFactory {}
	[Protocol] interface C138 : INSKeyedUnarchiverDelegate {}
	[Protocol] interface C139 : IINStartAudioCallIntentHandling {}
	[Protocol] interface C140 : ICIImageProcessorInput {}
	[Protocol] interface C141 : ICIImageProcessorOutput {}
	[Protocol] interface C142 : IPHLivePhotoFrame {}
	[Protocol] interface C143 : ICIImageProvider {}
	[Protocol] interface C144 : IUIApplicationDelegate {}
	[Protocol] interface C145 : IINStartPhotoPlaybackIntentHandling {}
	[Protocol] interface C146 : IUITableViewDataSource {}
	[Protocol] interface C147 : IUITableViewDataSourcePrefetching {}
	[Protocol] interface C148 : INSLocking {}
	[Protocol] interface C149 : IAVAssetDownloadDelegate {}
	[Protocol] interface C150 : IINStartVideoCallIntentHandling {}
	[Protocol] interface C151 : INSMachPortDelegate {}
	[Protocol] interface C152 : IUITableViewDelegate {}
	[Protocol] interface C153 : IPHPhotoLibraryChangeObserver {}
	[Protocol] interface C154 : IINStartWorkoutIntentHandling {}
	[Protocol] interface C155 : IPHContentEditingController {}
	[Protocol] interface C156 : IPHLivePhotoViewDelegate {}
	[Protocol] interface C157 : INSMetadataQueryDelegate {}
	[Protocol] interface C158 : IPKPushRegistryDelegate {}
	[Protocol] interface C159 : IQLPreviewControllerDataSource {}
	[Protocol] interface C160 : IQLPreviewControllerDelegate {}
	[Protocol] interface C161 : IINWorkoutsDomainHandling {}
	[Protocol] interface C162 : IUIBarPositioning {}
	[Protocol] interface C163 : IQLPreviewItem {}
	[Protocol] interface C164 : IUIBarPositioningDelegate {}
	[Protocol] interface C165 : INSMutableCopying {}
	[Protocol] interface C166 : IINUIHostedViewControlling {}
	[Protocol] interface C167 : IINUIHostedViewSiriProviding {}
	[Protocol] interface C168 : IAVAssetResourceLoaderDelegate {}
	[Protocol] interface C169 : IUITextDocumentProxy {}
	[Protocol] interface C170 : IRPBroadcastActivityViewControllerDelegate {}
	[Protocol] interface C171 : IJSExport {}
	[Protocol] interface C172 : IRPBroadcastControllerDelegate {}
	[Protocol] interface C173 : IUITextFieldDelegate {}
	[Protocol] interface C174 : IRPPreviewViewControllerDelegate {}
	[Protocol] interface C175 : IUICloudSharingControllerDelegate {}
	[Protocol] interface C176 : ICKRecordValue {}
	[Protocol] interface C177 : INSNetServiceBrowserDelegate {}
	[Protocol] interface C178 : IUITextInput {}
	[Protocol] interface C179 : INSNetServiceDelegate {}
	[Protocol] interface C180 : IRPScreenRecorderDelegate {}
	[Protocol] interface C181 : IMKAnnotation {}
	[Protocol] interface C182 : IHMAccessoryBrowserDelegate {}
	[Protocol] interface C183 : IUITextInputDelegate {}
	[Protocol] interface C184 : IHMAccessoryDelegate {}
	[Protocol] interface C185 : IUITextInputTokenizer {}
	[Protocol] interface C186 : ISFSafariViewControllerDelegate {}
	[Protocol] interface C187 : ICMAttachmentBearer {}
	[Protocol] interface C188 : IUITextInputTraits {}
	[Protocol] interface C189 : IAVAsynchronousKeyValueLoading {}
	[Protocol] interface C190 : IMDLComponent {}
	[Protocol] interface C191 : IUICollectionViewDataSource {}
	[Protocol] interface C192 : IAVAudio3DMixing {}
	[Protocol] interface C193 : IUICollectionViewDataSourcePrefetching {}
	[Protocol] interface C194 : ISCNActionable {}
	[Protocol] interface C195 : IMDLLightProbeIrradianceDataSource {}
	[Protocol] interface C196 : IUICollectionViewDelegate {}
	[Protocol] interface C197 : INSObjectProtocol {}
	[Protocol] interface C198 : IUICollectionViewDelegateFlowLayout {}
	[Protocol] interface C199 : ISCNAnimatable {}
	[Protocol] interface C200 : IHMCameraSnapshotControlDelegate {}
	[Protocol] interface C201 : IAVAudioRecorderDelegate {}
	[Protocol] interface C202 : IMKLocalSearchCompleterDelegate {}
	[Protocol] interface C203 : IUITextViewDelegate {}
	[Protocol] interface C204 : IAVAudioMixing {}
	[Protocol] interface C205 : IHMCameraStreamControlDelegate {}
	[Protocol] interface C206 : IUITimingCurveProvider {}
	[Protocol] interface C207 : ISCNBoundingVolume {}
	[Protocol] interface C208 : IAVAudioStereoMixing {}
	[Protocol] interface C209 : IMDLMeshBuffer {}
	[Protocol] interface C210 : IMDLMeshBufferAllocator {}
	[Protocol] interface C211 : ISCNBufferStream {}
	[Protocol] interface C212 : IUIToolbarDelegate {}
	[Protocol] interface C213 : INSPortDelegate {}
	[Protocol] interface C214 : IUICollectionViewSource {}
	[Protocol] interface C215 : IAVAudioSessionDelegate {}
	[Protocol] interface C216 : IAVAudioPlayerDelegate {}
	[Protocol] interface C217 : IMDLMeshBufferZone {}
	[Protocol] interface C218 : IMDLNamed {}
	[Protocol] interface C219 : IUICollisionBehaviorDelegate {}
	[Protocol] interface C220 : IAVCaptureAudioDataOutputSampleBufferDelegate {}
	[Protocol] interface C221 : IAVCaptureFileOutputRecordingDelegate {}
	[Protocol] interface C222 : IUITraitEnvironment {}
	[Protocol] interface C223 : IUIContentContainer {}
	[Protocol] interface C224 : IMDLObjectContainerComponent {}
	[Protocol] interface C225 : IMKMapViewDelegate {}
	[Protocol] interface C226 : IUIContentSizeCategoryAdjusting {}
	[Protocol] interface C227 : INSProgressReporting {}
	[Protocol] interface C228 : IMKOverlay {}
	[Protocol] interface C229 : IAVCaptureVideoDataOutputSampleBufferDelegate {}
	[Protocol] interface C230 : IAVCaptureMetadataOutputObjectsDelegate {}
	[Protocol] interface C231 : IUICoordinateSpace {}
	[Protocol] interface C232 : INSSecureCoding {}
	[Protocol] interface C233 : IAVCapturePhotoCaptureDelegate {}
	[Protocol] interface C234 : ICNKeyDescriptor {}
	[Protocol] interface C235 : IUIDataSourceModelAssociation {}
	[Protocol] interface C236 : IUIVideoEditorControllerDelegate {}
	[Protocol] interface C237 : INSStreamDelegate {}
	[Protocol] interface C238 : IMDLTransformComponent {}
	[Protocol] interface C239 : ICNContactPickerDelegate {}
	[Protocol] interface C240 : ICNContactViewControllerDelegate {}
	[Protocol] interface C241 : IMKReverseGeocoderDelegate {}
	[Protocol] interface C242 : ICAAction {}
	[Protocol] interface C243 : IUIViewAnimating {}
	[Protocol] interface C244 : IUIDocumentInteractionControllerDelegate {}
	[Protocol] interface C245 : ICBPeripheralManagerDelegate {}
	[Protocol] interface C246 : IUIDocumentMenuDelegate {}
	[Protocol] interface C247 : IMCAdvertiserAssistantDelegate {}
	[Protocol] interface C248 : ICALayerDelegate {}
	[Protocol] interface C249 : IMCBrowserViewControllerDelegate {}
	[Protocol] interface C250 : ICAMediaTiming {}
	[Protocol] interface C251 : IUIDocumentPickerDelegate {}
	[Protocol] interface C252 : ISCNNodeRendererDelegate {}
	[Protocol] interface C253 : ICLLocationManagerDelegate {}
	[Protocol] interface C254 : ICAMetalDrawable {}
	[Protocol] interface C255 : IMCNearbyServiceAdvertiserDelegate {}
	[Protocol] interface C256 : IMCNearbyServiceBrowserDelegate {}
	[Protocol] interface C257 : IUIDynamicAnimatorDelegate {}
	[Protocol] interface C258 : ICBPeripheralDelegate {}
	[Protocol] interface C259 : IUIViewControllerAnimatedTransitioning {}
	[Protocol] interface C260 : IUIDynamicItem {}
	[Protocol] interface C261 : ICBCentralManagerDelegate {}
	[Protocol] interface C262 : IMCSessionDelegate {}
	[Protocol] interface C263 : IHMHomeDelegate {}
	[Protocol] interface C264 : IUIViewControllerContextTransitioning {}
	[Protocol] interface C265 : INSFetchedResultsControllerDelegate {}
	[Protocol] interface C266 : IUIViewControllerInteractiveTransitioning {}
	[Protocol] interface C267 : INSFetchedResultsSectionInfo {}
	[Protocol] interface C268 : IUIViewControllerPreviewing {}
	[Protocol] interface C269 : IHMHomeManagerDelegate {}
	[Protocol] interface C270 : IUIViewControllerPreviewingDelegate {}
	[Protocol] interface C271 : IUIViewControllerRestoration {}
	[Protocol] interface C272 : INSFetchRequestResult {}
	[Protocol] interface C273 : ICSSearchableIndexDelegate {}
	[Protocol] interface C274 : IUIViewControllerTransitionCoordinator {}
	[Protocol] interface C275 : INSURLAuthenticationChallengeSender {}
	[Protocol] interface C276 : IUIViewControllerTransitionCoordinatorContext {}
	[Protocol] interface C277 : IUIViewControllerTransitioningDelegate {}
	[Protocol] interface C278 : IUIFocusEnvironment {}
	[Protocol] interface C279 : IUIViewImplicitlyAnimating {}
	[Protocol] interface C280 : INSUrlConnectionDataDelegate {}
	[Protocol] interface C281 : INSUserActivityDelegate {}
	[Protocol] interface C282 : IUIFocusItem {}
	[Protocol] interface C283 : INSUrlSessionDataDelegate {}
	[Protocol] interface C284 : ISCNPhysicsContactDelegate {}
	[Protocol] interface C285 : INSUrlConnectionDelegate {}
	[Protocol] interface C286 : INSUrlConnectionDownloadDelegate {}
	[Protocol] interface C287 : INSUrlSessionDelegate {}
	[Protocol] interface C288 : IMPMediaPickerControllerDelegate {}
	[Protocol] interface C289 : INSUrlSessionDownloadDelegate {}
	[Protocol] interface C290 : IMPMediaPlayback {}
	[Protocol] interface C291 : IADBannerViewDelegate {}
	[Protocol] interface C292 : INSUrlSessionStreamDelegate {}
	[Protocol] interface C293 : IGKFriendRequestComposeViewControllerDelegate {}
	[Protocol] interface C294 : IGKGameCenterControllerDelegate {}
	[Protocol] interface C295 : INSUrlProtocolClient {}
	[Protocol] interface C296 : IADInterstitialAdDelegate {}
	[Protocol] interface C297 : INSUrlSessionTaskDelegate {}
	[Protocol] interface C298 : IGKAchievementViewControllerDelegate {}
	[Protocol] interface C299 : IGKTurnBasedEventHandlerDelegate {}
	[Protocol] interface C300 : IUIGestureRecognizerDelegate {}
	[Protocol] interface C301 : IGKPeerPickerControllerDelegate {}
	[Protocol] interface C302 : IGKGameSessionEventListener {}
	[Protocol] interface C303 : IGKChallengeEventHandlerDelegate {}
	[Protocol] interface C304 : IGKChallengeListener {}
	[Protocol] interface C305 : IINCallsDomainHandling {}
	[Protocol] interface C306 : IINGetAvailableRestaurantReservationBookingsIntentHandling {}
	[Protocol] interface C307 : IGKInviteEventListener {}
	[Protocol] interface C308 : IINCancelWorkoutIntentHandling {}
	[Protocol] interface C309 : IGKSavedGameListener {}
	[Protocol] interface C310 : IINGetRestaurantGuestIntentHandling {}
	[Protocol] interface C311 : IMPPlayableContentDelegate {}
	[Protocol] interface C312 : IINEndWorkoutIntentHandling {}
	[Protocol] interface C313 : IINGetRideStatusIntentHandling {}
	[Protocol] interface C314 : IINGetRideStatusIntentResponseObserver {}
	[Protocol] interface C315 : IINGetAvailableRestaurantReservationBookingDefaultsIntentHandling {}
	[Protocol] interface C316 : IINGetUserCurrentRestaurantReservationBookingsIntentHandling {}
	[Protocol] interface C317 : IINCarPlayDomainHandling {}
	[Protocol] interface C318 : IUIGuidedAccessRestrictionDelegate {}
	[Protocol] interface C319 : IGKSessionDelegate {}
	[Protocol] interface C320 : ISCNProgramDelegate {}
	[Protocol] interface C321 : INWTcpConnectionAuthenticationDelegate {}
	[Protocol] interface C322 : IGKLeaderboardViewControllerDelegate {}
	[Protocol] interface C323 : IGKLocalPlayerListener {}
	[Protocol] interface C324 : ISCNShadable {}
	[Protocol] interface C325 : IUIWebViewDelegate {}
	[Protocol] interface C326 : IGKMatchDelegate {}
	[Protocol] interface C327 : IMPPlayableContentDataSource {}
	[Protocol] interface C328 : ISCNTechniqueSupport {}
	[Protocol] interface C329 : IGKMatchmakerViewControllerDelegate {}
	[Protocol] interface C330 : ISKPhysicsContactDelegate {}
	[Protocol] interface C331 : INCWidgetProviding {}
	[Protocol] interface C332 : IUIImagePickerControllerDelegate {}
	[Protocol] interface C333 : ISFSpeechRecognitionTaskDelegate {}
	[Protocol] interface C334 : ISCNSceneExportDelegate {}
	[Protocol] interface C335 : IUIKeyInput {}
	[Protocol] interface C336 : ISFSpeechRecognizerDelegate {}
	[Protocol] interface C337 : IUIInputViewAudioFeedback {}
	[Protocol] interface C338 : ISCNSceneRenderer {}
	[Protocol] interface C339 : ISCNSceneRendererDelegate {}
	[Protocol] interface C340 : IUINavigationBarDelegate {}
	[Protocol] interface C341 : IUNUserNotificationCenterDelegate {}
	[Protocol] interface C342 : ISKSceneDelegate {}
	[Protocol] interface C343 : IUILayoutSupport {}
	[Protocol] interface C344 : IUIPopoverBackgroundViewMethods {}
	[Protocol] interface C345 : IMSStickerBrowserViewDataSource {}
	[Protocol] interface C346 : IINBookRestaurantReservationIntentHandling {}
	[Protocol] interface C347 : IUIPickerViewAccessibilityDelegate {}
	[Protocol] interface C348 : IWCSessionDelegate {}
	[Protocol] interface C349 : IUNNotificationContentExtension {}
	[Protocol] interface C350 : IWKUIDelegate {}
	[Protocol] interface C351 : IUINavigationControllerDelegate {}
	[Protocol] interface C352 : IWKImageAnimatable {}
	[Protocol] interface C353 : IUIObjectRestoration {}
	[Protocol] interface C354 : IUIPickerViewDataSource {}
	[Protocol] interface C355 : IVSAccountManagerDelegate {}
	[Protocol] interface C356 : IUIPickerViewDelegate {}
	[Protocol] interface C357 : IWKNavigationDelegate {}
	[Protocol] interface C358 : IUIPickerViewModel {}
	[Protocol] interface C359 : IUIPageViewControllerDataSource {}
	[Protocol] interface C360 : IWKPreviewActionItem {}
	[Protocol] interface C361 : IUIPageViewControllerDelegate {}
	[Protocol] interface C362 : IWKScriptMessageHandler {}
	[Protocol] [BaseType (typeof (NSObject))] interface M1 : IMFMailComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M2 : IGKTurnBasedEventListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M3 : IINIntentHandlerProviding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M4 : IMFMessageComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M5 : IUIPopoverControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M6 : IUIPopoverPresentationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M7 : IGKTurnBasedMatchmakerViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M8 : IINListRideOptionsIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M9 : IUIPreviewActionItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M10 : IMTLBlitCommandEncoder {}
	[Protocol] [BaseType (typeof (NSObject))] interface M11 : IGKVoiceChatClient {}
	[Protocol] [BaseType (typeof (NSObject))] interface M12 : IMTLBuffer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M13 : ISKViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M14 : IUIPreviewInteractionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M15 : ISKWarpable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M16 : IINMessagesDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M17 : IINPauseWorkoutIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M18 : IMTLCommandBuffer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M19 : IAVPlayerItemLegibleOutputPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M20 : IUIPrinterPickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M21 : IGKAgentDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M22 : IMTLCommandEncoder {}
	[Protocol] [BaseType (typeof (NSObject))] interface M23 : IMTLCommandQueue {}
	[Protocol] [BaseType (typeof (NSObject))] interface M24 : IAVPlayerItemMetadataCollectorPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M25 : IINPaymentsDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M26 : ICFType {}
	[Protocol] [BaseType (typeof (NSObject))] interface M27 : IEKCalendarChooserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M28 : IAVPlayerItemMetadataOutputPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M29 : ISKCloudServiceSetupViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M30 : IMTLComputeCommandEncoder {}
	[Protocol] [BaseType (typeof (NSObject))] interface M31 : IEKEventEditViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M32 : IAVPlayerItemOutputPullDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M33 : IAVPlayerItemOutputPushDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M34 : IEKEventViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M35 : IMTLComputePipelineState {}
	[Protocol] [BaseType (typeof (NSObject))] interface M36 : IUIPrintInteractionControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M37 : IEAAccessoryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M38 : ISKPaymentTransactionObserver {}
	[Protocol] [BaseType (typeof (NSObject))] interface M39 : IINPhotosDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M40 : IMTLDepthStencilState {}
	[Protocol] [BaseType (typeof (NSObject))] interface M41 : IGKGameModel {}
	[Protocol] [BaseType (typeof (NSObject))] interface M42 : IMTLDevice {}
	[Protocol] [BaseType (typeof (NSObject))] interface M43 : IGKGameModelPlayer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M44 : ISKProductsRequestDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M45 : IGKGameModelUpdate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M46 : IMTLDrawable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M47 : IEAWiFiUnconfiguredAccessoryBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M48 : IINRadioDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M49 : IMTLFence {}
	[Protocol] [BaseType (typeof (NSObject))] interface M50 : INativeObject {}
	[Protocol] [BaseType (typeof (NSObject))] interface M51 : IMTLFunction {}
	[Protocol] [BaseType (typeof (NSObject))] interface M52 : ISKRequestDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M53 : IEAGLDrawable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M54 : ISKStoreProductViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M55 : IMTLHeap {}
	[Protocol] [BaseType (typeof (NSObject))] interface M56 : IINRequestPaymentIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M57 : IAVSpeechSynthesizerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M58 : IMTLLibrary {}
	[Protocol] [BaseType (typeof (NSObject))] interface M59 : IINRequestRideIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M60 : IPKAddPassesViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M61 : IMTLParallelRenderCommandEncoder {}
	[Protocol] [BaseType (typeof (NSObject))] interface M62 : INSCacheDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M63 : IPKAddPaymentPassViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M64 : IUIScrollViewAccessibilityDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M65 : IUIScrollViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M66 : IAVVideoCompositing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M67 : IMTLRenderCommandEncoder {}
	[Protocol] [BaseType (typeof (NSObject))] interface M68 : INSCoding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M69 : IINResumeWorkoutIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M70 : IAVVideoCompositionValidationHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M71 : IUISearchBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M72 : INSCopying {}
	[Protocol] [BaseType (typeof (NSObject))] interface M73 : IGKRandom {}
	[Protocol] [BaseType (typeof (NSObject))] interface M74 : INSLayoutManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M75 : IUISearchControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M76 : IMTLRenderPipelineState {}
	[Protocol] [BaseType (typeof (NSObject))] interface M77 : IINRidesharingDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M78 : IPKPaymentAuthorizationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M79 : IMTLResource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M80 : IUISearchDisplayDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M81 : IUISearchResultsUpdating {}
	[Protocol] [BaseType (typeof (NSObject))] interface M82 : IGKSceneRootNodeType {}
	[Protocol] [BaseType (typeof (NSObject))] interface M83 : IINSaveProfileInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M84 : IPKPaymentAuthorizationViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M85 : INSTextAttachmentContainer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M86 : IMTLSamplerState {}
	[Protocol] [BaseType (typeof (NSObject))] interface M87 : IINSearchCallHistoryIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M88 : IAVPictureInPictureControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M89 : INSTextLayoutOrientationProvider {}
	[Protocol] [BaseType (typeof (NSObject))] interface M90 : IGKStrategist {}
	[Protocol] [BaseType (typeof (NSObject))] interface M91 : INSDiscardableContent {}
	[Protocol] [BaseType (typeof (NSObject))] interface M92 : IAVPlayerViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M93 : INSTextStorageDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M94 : IINSearchForMessagesIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M95 : IUISplitViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M96 : IINSearchForPhotosIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M97 : IUIAccelerometerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M98 : IMTLTexture {}
	[Protocol] [BaseType (typeof (NSObject))] interface M99 : ICXCallDirectoryExtensionContextDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M100 : IUIAccessibilityContainer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M101 : IINSendMessageIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M102 : INSExtensionRequestHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M103 : IUIStateRestoring {}
	[Protocol] [BaseType (typeof (NSObject))] interface M104 : IINSendPaymentIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M105 : ICXCallObserverDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M106 : IGLKNamedEffect {}
	[Protocol] [BaseType (typeof (NSObject))] interface M107 : IUIAccessibilityIdentification {}
	[Protocol] [BaseType (typeof (NSObject))] interface M108 : IINSetAudioSourceInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M109 : IUIAccessibilityReadingContent {}
	[Protocol] [BaseType (typeof (NSObject))] interface M110 : IABNewPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M111 : INSFileManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M112 : IINSetClimateSettingsInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M113 : ICIFilterConstructor {}
	[Protocol] [BaseType (typeof (NSObject))] interface M114 : IABPeoplePickerNavigationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M115 : IUIActionSheetDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M116 : INSFilePresenter {}
	[Protocol] [BaseType (typeof (NSObject))] interface M117 : IGLKViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M118 : IGLKViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M119 : IINSetDefrosterSettingsInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M120 : ICXProviderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M121 : IABPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M122 : IABUnknownPersonViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M123 : IINSetMessageAttributeIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M124 : IUIActivityItemSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M125 : IUITabBarControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M126 : IINSetProfileInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M127 : IUIAdaptivePresentationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M128 : IUITabBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M129 : IMTKViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M130 : IINSetRadioStationIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M131 : IUIAlertViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M132 : IINSetSeatSettingsInCarIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M133 : IUIAppearance {}
	[Protocol] [BaseType (typeof (NSObject))] interface M134 : IUIAppearanceContainer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M135 : INSKeyedArchiverDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M136 : IINSpeakable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M137 : IAUAudioUnitFactory {}
	[Protocol] [BaseType (typeof (NSObject))] interface M138 : INSKeyedUnarchiverDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M139 : IINStartAudioCallIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M140 : ICIImageProcessorInput {}
	[Protocol] [BaseType (typeof (NSObject))] interface M141 : ICIImageProcessorOutput {}
	[Protocol] [BaseType (typeof (NSObject))] interface M142 : IPHLivePhotoFrame {}
	[Protocol] [BaseType (typeof (NSObject))] interface M143 : ICIImageProvider {}
	[Protocol] [BaseType (typeof (NSObject))] interface M144 : IUIApplicationDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M145 : IINStartPhotoPlaybackIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M146 : IUITableViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M147 : IUITableViewDataSourcePrefetching {}
	[Protocol] [BaseType (typeof (NSObject))] interface M148 : INSLocking {}
	[Protocol] [BaseType (typeof (NSObject))] interface M149 : IAVAssetDownloadDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M150 : IINStartVideoCallIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M151 : INSMachPortDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M152 : IUITableViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M153 : IPHPhotoLibraryChangeObserver {}
	[Protocol] [BaseType (typeof (NSObject))] interface M154 : IINStartWorkoutIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M155 : IPHContentEditingController {}
	[Protocol] [BaseType (typeof (NSObject))] interface M156 : IPHLivePhotoViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M157 : INSMetadataQueryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M158 : IPKPushRegistryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M159 : IQLPreviewControllerDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M160 : IQLPreviewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M161 : IINWorkoutsDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M162 : IUIBarPositioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M163 : IQLPreviewItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M164 : IUIBarPositioningDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M165 : INSMutableCopying {}
	[Protocol] [BaseType (typeof (NSObject))] interface M166 : IINUIHostedViewControlling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M167 : IINUIHostedViewSiriProviding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M168 : IAVAssetResourceLoaderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M169 : IUITextDocumentProxy {}
	[Protocol] [BaseType (typeof (NSObject))] interface M170 : IRPBroadcastActivityViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M171 : IJSExport {}
	[Protocol] [BaseType (typeof (NSObject))] interface M172 : IRPBroadcastControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M173 : IUITextFieldDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M174 : IRPPreviewViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M175 : IUICloudSharingControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M176 : ICKRecordValue {}
	[Protocol] [BaseType (typeof (NSObject))] interface M177 : INSNetServiceBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M178 : IUITextInput {}
	[Protocol] [BaseType (typeof (NSObject))] interface M179 : INSNetServiceDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M180 : IRPScreenRecorderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M181 : IMKAnnotation {}
	[Protocol] [BaseType (typeof (NSObject))] interface M182 : IHMAccessoryBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M183 : IUITextInputDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M184 : IHMAccessoryDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M185 : IUITextInputTokenizer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M186 : ISFSafariViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M187 : ICMAttachmentBearer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M188 : IUITextInputTraits {}
	[Protocol] [BaseType (typeof (NSObject))] interface M189 : IAVAsynchronousKeyValueLoading {}
	[Protocol] [BaseType (typeof (NSObject))] interface M190 : IMDLComponent {}
	[Protocol] [BaseType (typeof (NSObject))] interface M191 : IUICollectionViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M192 : IAVAudio3DMixing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M193 : IUICollectionViewDataSourcePrefetching {}
	[Protocol] [BaseType (typeof (NSObject))] interface M194 : ISCNActionable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M195 : IMDLLightProbeIrradianceDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M196 : IUICollectionViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M197 : INSObjectProtocol {}
	[Protocol] [BaseType (typeof (NSObject))] interface M198 : IUICollectionViewDelegateFlowLayout {}
	[Protocol] [BaseType (typeof (NSObject))] interface M199 : ISCNAnimatable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M200 : IHMCameraSnapshotControlDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M201 : IAVAudioRecorderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M202 : IMKLocalSearchCompleterDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M203 : IUITextViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M204 : IAVAudioMixing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M205 : IHMCameraStreamControlDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M206 : IUITimingCurveProvider {}
	[Protocol] [BaseType (typeof (NSObject))] interface M207 : ISCNBoundingVolume {}
	[Protocol] [BaseType (typeof (NSObject))] interface M208 : IAVAudioStereoMixing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M209 : IMDLMeshBuffer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M210 : IMDLMeshBufferAllocator {}
	[Protocol] [BaseType (typeof (NSObject))] interface M211 : ISCNBufferStream {}
	[Protocol] [BaseType (typeof (NSObject))] interface M212 : IUIToolbarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M213 : INSPortDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M214 : IUICollectionViewSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M215 : IAVAudioSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M216 : IAVAudioPlayerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M217 : IMDLMeshBufferZone {}
	[Protocol] [BaseType (typeof (NSObject))] interface M218 : IMDLNamed {}
	[Protocol] [BaseType (typeof (NSObject))] interface M219 : IUICollisionBehaviorDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M220 : IAVCaptureAudioDataOutputSampleBufferDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M221 : IAVCaptureFileOutputRecordingDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M222 : IUITraitEnvironment {}
	[Protocol] [BaseType (typeof (NSObject))] interface M223 : IUIContentContainer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M224 : IMDLObjectContainerComponent {}
	[Protocol] [BaseType (typeof (NSObject))] interface M225 : IMKMapViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M226 : IUIContentSizeCategoryAdjusting {}
	[Protocol] [BaseType (typeof (NSObject))] interface M227 : INSProgressReporting {}
	[Protocol] [BaseType (typeof (NSObject))] interface M228 : IMKOverlay {}
	[Protocol] [BaseType (typeof (NSObject))] interface M229 : IAVCaptureVideoDataOutputSampleBufferDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M230 : IAVCaptureMetadataOutputObjectsDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M231 : IUICoordinateSpace {}
	[Protocol] [BaseType (typeof (NSObject))] interface M232 : INSSecureCoding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M233 : IAVCapturePhotoCaptureDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M234 : ICNKeyDescriptor {}
	[Protocol] [BaseType (typeof (NSObject))] interface M235 : IUIDataSourceModelAssociation {}
	[Protocol] [BaseType (typeof (NSObject))] interface M236 : IUIVideoEditorControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M237 : INSStreamDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M238 : IMDLTransformComponent {}
	[Protocol] [BaseType (typeof (NSObject))] interface M239 : ICNContactPickerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M240 : ICNContactViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M241 : IMKReverseGeocoderDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M242 : ICAAction {}
	[Protocol] [BaseType (typeof (NSObject))] interface M243 : IUIViewAnimating {}
	[Protocol] [BaseType (typeof (NSObject))] interface M244 : IUIDocumentInteractionControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M245 : ICBPeripheralManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M246 : IUIDocumentMenuDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M247 : IMCAdvertiserAssistantDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M248 : ICALayerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M249 : IMCBrowserViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M250 : ICAMediaTiming {}
	[Protocol] [BaseType (typeof (NSObject))] interface M251 : IUIDocumentPickerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M252 : ISCNNodeRendererDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M253 : ICLLocationManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M254 : ICAMetalDrawable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M255 : IMCNearbyServiceAdvertiserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M256 : IMCNearbyServiceBrowserDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M257 : IUIDynamicAnimatorDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M258 : ICBPeripheralDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M259 : IUIViewControllerAnimatedTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M260 : IUIDynamicItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M261 : ICBCentralManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M262 : IMCSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M263 : IHMHomeDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M264 : IUIViewControllerContextTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M265 : INSFetchedResultsControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M266 : IUIViewControllerInteractiveTransitioning {}
	[Protocol] [BaseType (typeof (NSObject))] interface M267 : INSFetchedResultsSectionInfo {}
	[Protocol] [BaseType (typeof (NSObject))] interface M268 : IUIViewControllerPreviewing {}
	[Protocol] [BaseType (typeof (NSObject))] interface M269 : IHMHomeManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M270 : IUIViewControllerPreviewingDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M271 : IUIViewControllerRestoration {}
	[Protocol] [BaseType (typeof (NSObject))] interface M272 : INSFetchRequestResult {}
	[Protocol] [BaseType (typeof (NSObject))] interface M273 : ICSSearchableIndexDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M274 : IUIViewControllerTransitionCoordinator {}
	[Protocol] [BaseType (typeof (NSObject))] interface M275 : INSURLAuthenticationChallengeSender {}
	[Protocol] [BaseType (typeof (NSObject))] interface M276 : IUIViewControllerTransitionCoordinatorContext {}
	[Protocol] [BaseType (typeof (NSObject))] interface M277 : IUIViewControllerTransitioningDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M278 : IUIFocusEnvironment {}
	[Protocol] [BaseType (typeof (NSObject))] interface M279 : IUIViewImplicitlyAnimating {}
	[Protocol] [BaseType (typeof (NSObject))] interface M280 : INSUrlConnectionDataDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M281 : INSUserActivityDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M282 : IUIFocusItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M283 : INSUrlSessionDataDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M284 : ISCNPhysicsContactDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M285 : INSUrlConnectionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M286 : INSUrlConnectionDownloadDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M287 : INSUrlSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M288 : IMPMediaPickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M289 : INSUrlSessionDownloadDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M290 : IMPMediaPlayback {}
	[Protocol] [BaseType (typeof (NSObject))] interface M291 : IADBannerViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M292 : INSUrlSessionStreamDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M293 : IGKFriendRequestComposeViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M294 : IGKGameCenterControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M295 : INSUrlProtocolClient {}
	[Protocol] [BaseType (typeof (NSObject))] interface M296 : IADInterstitialAdDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M297 : INSUrlSessionTaskDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M298 : IGKAchievementViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M299 : IGKTurnBasedEventHandlerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M300 : IUIGestureRecognizerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M301 : IGKPeerPickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M302 : IGKGameSessionEventListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M303 : IGKChallengeEventHandlerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M304 : IGKChallengeListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M305 : IINCallsDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M306 : IINGetAvailableRestaurantReservationBookingsIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M307 : IGKInviteEventListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M308 : IINCancelWorkoutIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M309 : IGKSavedGameListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M310 : IINGetRestaurantGuestIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M311 : IMPPlayableContentDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M312 : IINEndWorkoutIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M313 : IINGetRideStatusIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M314 : IINGetRideStatusIntentResponseObserver {}
	[Protocol] [BaseType (typeof (NSObject))] interface M315 : IINGetAvailableRestaurantReservationBookingDefaultsIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M316 : IINGetUserCurrentRestaurantReservationBookingsIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M317 : IINCarPlayDomainHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M318 : IUIGuidedAccessRestrictionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M319 : IGKSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M320 : ISCNProgramDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M321 : INWTcpConnectionAuthenticationDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M322 : IGKLeaderboardViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M323 : IGKLocalPlayerListener {}
	[Protocol] [BaseType (typeof (NSObject))] interface M324 : ISCNShadable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M325 : IUIWebViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M326 : IGKMatchDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M327 : IMPPlayableContentDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M328 : ISCNTechniqueSupport {}
	[Protocol] [BaseType (typeof (NSObject))] interface M329 : IGKMatchmakerViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M330 : ISKPhysicsContactDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M331 : INCWidgetProviding {}
	[Protocol] [BaseType (typeof (NSObject))] interface M332 : IUIImagePickerControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M333 : ISFSpeechRecognitionTaskDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M334 : ISCNSceneExportDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M335 : IUIKeyInput {}
	[Protocol] [BaseType (typeof (NSObject))] interface M336 : ISFSpeechRecognizerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M337 : IUIInputViewAudioFeedback {}
	[Protocol] [BaseType (typeof (NSObject))] interface M338 : ISCNSceneRenderer {}
	[Protocol] [BaseType (typeof (NSObject))] interface M339 : ISCNSceneRendererDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M340 : IUINavigationBarDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M341 : IUNUserNotificationCenterDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M342 : ISKSceneDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M343 : IUILayoutSupport {}
	[Protocol] [BaseType (typeof (NSObject))] interface M344 : IUIPopoverBackgroundViewMethods {}
	[Protocol] [BaseType (typeof (NSObject))] interface M345 : IMSStickerBrowserViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M346 : IINBookRestaurantReservationIntentHandling {}
	[Protocol] [BaseType (typeof (NSObject))] interface M347 : IUIPickerViewAccessibilityDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M348 : IWCSessionDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M349 : IUNNotificationContentExtension {}
	[Protocol] [BaseType (typeof (NSObject))] interface M350 : IWKUIDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M351 : IUINavigationControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M352 : IWKImageAnimatable {}
	[Protocol] [BaseType (typeof (NSObject))] interface M353 : IUIObjectRestoration {}
	[Protocol] [BaseType (typeof (NSObject))] interface M354 : IUIPickerViewDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M355 : IVSAccountManagerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M356 : IUIPickerViewDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M357 : IWKNavigationDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M358 : IUIPickerViewModel {}
	[Protocol] [BaseType (typeof (NSObject))] interface M359 : IUIPageViewControllerDataSource {}
	[Protocol] [BaseType (typeof (NSObject))] interface M360 : IWKPreviewActionItem {}
	[Protocol] [BaseType (typeof (NSObject))] interface M361 : IUIPageViewControllerDelegate {}
	[Protocol] [BaseType (typeof (NSObject))] interface M362 : IWKScriptMessageHandler {}
}
