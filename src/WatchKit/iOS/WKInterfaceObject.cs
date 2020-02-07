#if __IOS__
using System;
using System.ComponentModel;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {
	[Register ("WKInterfaceObject", SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceObject : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		protected WKInterfaceObject (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKInterfaceObject (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual bool AccessibilityActivate ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetAlpha (nfloat alpha)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetHeight (nfloat height)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetHidden (bool hidden)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetWidth (nfloat width)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual CGPoint AccessibilityActivationPoint {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}

			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual NSAttributedString AccessibilityAttributedHint {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual NSAttributedString AccessibilityAttributedLabel {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual NSAttributedString AccessibilityAttributedValue {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual bool AccessibilityElementsHidden {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual CGRect AccessibilityFrame {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string AccessibilityHint {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}

			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string AccessibilityLabel {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string AccessibilityLanguage {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual UIAccessibilityNavigationStyle AccessibilityNavigationStyle {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual UIBezierPath AccessibilityPath {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual UIAccessibilityTrait AccessibilityTraits {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string AccessibilityValue {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual bool AccessibilityViewIsModal {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual string InterfaceProperty {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual bool IsAccessibilityElement {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public virtual bool ShouldGroupAccessibilityChildren {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			set {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public static NSString AnnouncementDidFinishNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int AnnouncementNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString AssistiveTechnologyKey {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString AssistiveTouchStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString BoldTextStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString ClosedCaptioningStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString DarkerSystemColorsStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString ElementFocusedNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString FocusedElementKey {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString GrayscaleStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString GuidedAccessStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString HearingDevicePairedEarDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString InvertColorsStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int LayoutChangedNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString MonoAudioStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString NotificationSwitchControlIdentifier {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString NotificationVoiceOverIdentifier {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int PageScrolledNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int PauseAssistiveTechnologyNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString ReduceMotionStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString ReduceTransparencyStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int ResumeAssistiveTechnologyNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static int ScreenChangedNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString ShakeToUndoDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeakScreenStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeakSelectionStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeechAttributeIpaNotation {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeechAttributeLanguage {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeechAttributePitch {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeechAttributePunctuation {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SpeechAttributeQueueAnnouncement {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString SwitchControlStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString TextAttributeCustom {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString TextAttributeHeadingLevel {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitAdjustable {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitAllowsDirectInteraction {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitButton {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitCausesPageTurn {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitHeader {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitImage {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitKeyboardKey {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitLink {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitNone {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitNotEnabled {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitPlaysSound {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitSearchField {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitSelected {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitStartsMediaSession {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitStaticText {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitSummaryElement {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitTabBar {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static long TraitUpdatesFrequently {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString UnfocusedElementKey {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString VoiceOverStatusChanged {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
		public static NSString VoiceOverStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		//
		// Notifications
		//
		public static class Notifications {
			public static NSObject ObserveAnnouncementDidFinish (EventHandler<UIKit.UIAccessibilityAnnouncementFinishedEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveAnnouncementDidFinish (NSObject objectToObserve, EventHandler<UIKit.UIAccessibilityAnnouncementFinishedEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveAssistiveTechnologyKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveAssistiveTechnologyKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveAssistiveTouchStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveAssistiveTouchStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveBoldTextStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveBoldTextStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveClosedCaptioningStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveClosedCaptioningStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveDarkerSystemColorsStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveDarkerSystemColorsStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveElementFocused (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveElementFocused (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveFocusedElementKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveFocusedElementKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveGrayscaleStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveGrayscaleStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveGuidedAccessStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveGuidedAccessStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveHearingDevicePairedEarDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveHearingDevicePairedEarDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveInvertColorsStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveInvertColorsStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveMonoAudioStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveMonoAudioStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveReduceMotionStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveReduceMotionStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveReduceTransparencyStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveReduceTransparencyStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveShakeToUndoDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveShakeToUndoDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSpeakScreenStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSpeakScreenStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSpeakSelectionStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSpeakSelectionStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSwitchControlStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveSwitchControlStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveUnfocusedElementKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveUnfocusedElementKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveVoiceOverStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
			public static NSObject ObserveVoiceOverStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}
	} /* class WKInterfaceObject */
}
#endif // __IOS__
