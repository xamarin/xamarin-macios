#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {
	[Register ("WKInterfaceObject", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKInterfaceObject : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceObject (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceObject (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual bool AccessibilityActivate ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetAlpha (nfloat alpha)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetHeight (nfloat height)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetHidden (bool hidden)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetWidth (nfloat width)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual CGPoint AccessibilityActivationPoint {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}

			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual NSAttributedString AccessibilityAttributedHint {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual NSAttributedString AccessibilityAttributedLabel {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual NSAttributedString AccessibilityAttributedValue {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual bool AccessibilityElementsHidden {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual CGRect AccessibilityFrame {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string AccessibilityHint {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}

			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string AccessibilityLabel {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string AccessibilityLanguage {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual UIAccessibilityNavigationStyle AccessibilityNavigationStyle {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual UIBezierPath AccessibilityPath {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual UIAccessibilityTrait AccessibilityTraits {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string AccessibilityValue {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual bool AccessibilityViewIsModal {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual string InterfaceProperty {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual bool IsAccessibilityElement {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public virtual bool ShouldGroupAccessibilityChildren {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			set {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public static NSString AnnouncementDidFinishNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int AnnouncementNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString AssistiveTechnologyKey {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString AssistiveTouchStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString BoldTextStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString ClosedCaptioningStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString DarkerSystemColorsStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString ElementFocusedNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString FocusedElementKey {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString GrayscaleStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString GuidedAccessStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString HearingDevicePairedEarDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString InvertColorsStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int LayoutChangedNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString MonoAudioStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString NotificationSwitchControlIdentifier {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString NotificationVoiceOverIdentifier {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int PageScrolledNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int PauseAssistiveTechnologyNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString ReduceMotionStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString ReduceTransparencyStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int ResumeAssistiveTechnologyNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static int ScreenChangedNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString ShakeToUndoDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeakScreenStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeakSelectionStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeechAttributeIpaNotation {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeechAttributeLanguage {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeechAttributePitch {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeechAttributePunctuation {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SpeechAttributeQueueAnnouncement {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString SwitchControlStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString TextAttributeCustom {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString TextAttributeHeadingLevel {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitAdjustable {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitAllowsDirectInteraction {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitButton {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitCausesPageTurn {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitHeader {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitImage {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitKeyboardKey {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitLink {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitNone {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitNotEnabled {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitPlaysSound {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitSearchField {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitSelected {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitStartsMediaSession {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitStaticText {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitSummaryElement {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitTabBar {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static long TraitUpdatesFrequently {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString UnfocusedElementKey {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString VoiceOverStatusChanged {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
		public static NSString VoiceOverStatusDidChangeNotification {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		//
		// Notifications
		//
		public static class Notifications {
			public static NSObject ObserveAnnouncementDidFinish (EventHandler<UIKit.UIAccessibilityAnnouncementFinishedEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveAnnouncementDidFinish (NSObject objectToObserve, EventHandler<UIKit.UIAccessibilityAnnouncementFinishedEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveAssistiveTechnologyKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveAssistiveTechnologyKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveAssistiveTouchStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveAssistiveTouchStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveBoldTextStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveBoldTextStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveClosedCaptioningStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveClosedCaptioningStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveDarkerSystemColorsStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveDarkerSystemColorsStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveElementFocused (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveElementFocused (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveFocusedElementKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveFocusedElementKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveGrayscaleStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveGrayscaleStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveGuidedAccessStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveGuidedAccessStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveHearingDevicePairedEarDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveHearingDevicePairedEarDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveInvertColorsStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveInvertColorsStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveMonoAudioStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveMonoAudioStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveReduceMotionStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveReduceMotionStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveReduceTransparencyStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveReduceTransparencyStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveShakeToUndoDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveShakeToUndoDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSpeakScreenStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSpeakScreenStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSpeakSelectionStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSpeakSelectionStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSwitchControlStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveSwitchControlStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveUnfocusedElementKey (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveUnfocusedElementKey (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveVoiceOverStatusDidChange (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
			public static NSObject ObserveVoiceOverStatusDidChange (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}
	} /* class WKInterfaceObject */
}
#endif // __IOS__ && !NET
