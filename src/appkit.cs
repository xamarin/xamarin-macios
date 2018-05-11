 // Copyright 2011-2012 Xamarin, Inc.
// Copyright 2010, 2011, Novell, Inc.
// Copyright 2010, Kenneth Pouncey
// Coprightt 2010, James Clancey
// Copyright 2011, Curtis Wensley
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//
// appkit.cs: Definitions for AppKit
//

// TODO: turn NSAnimatablePropertyCOntainer into a system similar to UIAppearance

using System;
using System.Diagnostics;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreImage;
using CoreAnimation;
using CoreData;
using OpenGL;
using CoreVideo;
using CloudKit;

using CGGlyph = System.UInt16;

namespace AppKit {
	//[BaseType (typeof (NSObject))]
	//interface CIImage {
	//	[Export ("drawInRect:fromRect:operation:fraction:")]
	//	void Draw (CGRect inRect, CGRect fromRect, NSCompositingOperation operation, float fractionDelta);
	//
	//	[Export ("drawAtPoint:fromRect:operation:fraction:")]
	//	void DrawAtPoint (CGPoint atPoint, CGRect fromRect, NSCompositingOperation operation, float fractionDelta);
	//}
	
	[BaseType (typeof (NSCell))]
	interface NSActionCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);
	
		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target  { get; set; }
	
		[Export ("action"), NullAllowed]
		Selector Action  { get; set; }
	
		[Export ("tag")]
		nint Tag  { get; set; }
	
	}

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSAlignmentFeedbackToken
	{
	}

	interface INSAlignmentFeedbackToken { }

	// @interface NSAlignmentFeedbackFilter : NSObject
	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSAlignmentFeedbackFilter
	{
		[Static]
		[Export ("inputEventMask")]
		NSEventMask InputEventMask { get; }

		[Export ("updateWithEvent:")]
		void Update (NSEvent theEvent);

		[Export ("updateWithPanRecognizer:")]
		void Update (NSPanGestureRecognizer panRecognizer);

		[Export ("alignmentFeedbackTokenForMovementInView:previousPoint:alignedPoint:defaultPoint:")]
		[return: NullAllowed]
		INSAlignmentFeedbackToken GetTokenForMovement ([NullAllowed] NSView view, CGPoint previousPoint, CGPoint alignedPoint, CGPoint defaultPoint);

		[Export ("alignmentFeedbackTokenForHorizontalMovementInView:previousX:alignedX:defaultX:")]
		[return: NullAllowed]
		INSAlignmentFeedbackToken GetTokenForHorizontalMovement ([NullAllowed] NSView view, nfloat previousX, nfloat alignedX, nfloat defaultX);

		[Export ("alignmentFeedbackTokenForVerticalMovementInView:previousY:alignedY:defaultY:")]
		[return: NullAllowed]
		INSAlignmentFeedbackToken GetTokenForVerticalMovement ([NullAllowed] NSView view, nfloat previousY, nfloat alignedY, nfloat defaultY);

		[Export ("performFeedback:performanceTime:")]
		void PerformFeedback (INSAlignmentFeedbackToken[] tokens, NSHapticFeedbackPerformanceTime performanceTime);
	}

	//
	// Inlined, not really an object implementation
	//
	interface NSAnimatablePropertyContainer {
		[Export ("animator")]
		NSObject Animator { [return: Proxy] get; }
	
		[Export ("animations")]
		NSDictionary Animations { get; set; }
	
		[Export ("animationForKey:")]
		NSObject AnimationFor (NSString key);
	
		[Static, Export ("defaultAnimationForKey:")]
		NSObject DefaultAnimationFor (NSString key);
	}
	
	interface NSAnimationProgressMarkEventArgs {
		[Export ("NSAnimationProgressMark")]
		float Progress { get; } /* float, not CGFloat */
	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSAnimationDelegate)})]
	interface NSAnimation : NSCoding, NSCopying {
		[Export ("initWithDuration:animationCurve:")]
		[Sealed] // Just to avoid the duplicate selector error
		IntPtr Constructor (double duration, NSAnimationCurve animationCurve);

#if !XAMCORE_4_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initWithDuration:animationCurve:")]
		IntPtr Constant (double duration, NSAnimationCurve animationCurve);
#endif
	
		[Export ("startAnimation")]
		void StartAnimation ();
	
		[Export ("stopAnimation")]
		void StopAnimation ();
	
		[Export ("isAnimating")]
		bool IsAnimating ();
	
		[Export ("currentProgress")]
		float CurrentProgress { get; set; } /* NSAnimationProgress = float */
	
		[Export ("duration")]
		double Duration  { get; set; }
	
		[Export ("animationBlockingMode")]
		NSAnimationBlockingMode AnimationBlockingMode  { get; set; }
	
		[Export ("frameRate")]
		float FrameRate  { get; set; } /* float, not CGFloat */
	
		[Export ("animationCurve")]
		NSAnimationCurve AnimationCurve  { get; set; }
	
		[Export ("currentValue")]
		float CurrentValue { get; } /* float, not CGFloat */
	
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSAnimationDelegate Delegate { get; set; }

		[Export ("progressMarks", ArgumentSemantic.Copy)]
		NSNumber [] ProgressMarks  { get; set; }
	
		[Export ("addProgressMark:")]
		void AddProgressMark (float /* NSAnimationProgress = float */ progressMark);
	
		[Export ("removeProgressMark:")]
		void RemoveProgressMark (float /* NSAnimationProgress = float */ progressMark);
	
		[Export ("startWhenAnimation:reachesProgress:")]
		void StartWhenAnimationReaches (NSAnimation animation, float /* NSAnimationProgress = float */ startProgress);
	
		[Export ("stopWhenAnimation:reachesProgress:")]
		void StopWhenAnimationReaches (NSAnimation animation, float /* NSAnimationProgress = float */ stopProgress);
	
		[Export ("clearStartAnimation")]
		void ClearStartAnimation ();
	
		[Export ("clearStopAnimation")]
		void ClearStopAnimation ();

		[Export ("runLoopModesForAnimating")]
		NSString [] RunLoopModesForAnimating { get; }

		[Notification (typeof (NSAnimationProgressMarkEventArgs)), Field ("NSAnimationProgressMarkNotification")]
		NSString ProgressMarkNotification { get; }

		[Field ("NSAnimationProgressMark")]
		NSString ProgressMark { get; }

		[Field ("NSAnimationTriggerOrderIn")]
		NSString TriggerOrderIn { get; }

		[Field ("NSAnimationTriggerOrderOut")]
		NSString TriggerOrderOut { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSAnimationDelegate {
		[Export ("animationShouldStart:"), DelegateName ("NSAnimationPredicate"), DefaultValue (true)]
		bool AnimationShouldStart (NSAnimation animation);
	
		[Export ("animationDidStop:"), EventArgs ("NSAnimation")]
		void AnimationDidStop (NSAnimation animation);
	
		[Export ("animationDidEnd:"), EventArgs ("NSAnimation")]
		void AnimationDidEnd (NSAnimation animation);
	
		[Export ("animation:valueForProgress:"), DelegateName ("NSAnimationProgress"), DefaultValueFromArgumentAttribute ("progress")]
		float /* float, not CGFloat */ ComputeAnimationCurve (NSAnimation animation, float /* NSAnimationProgress = float */ progress);
	
		[Export ("animation:didReachProgressMark:"), EventArgs ("NSAnimation")]
		void AnimationDidReachProgressMark (NSAnimation animation, float /* NSAnimationProgress = float */ progress);
	}

	[BaseType (typeof (NSObject))]
	partial interface NSAnimationContext {
		[Static]
		[Export ("beginGrouping")]
		void BeginGrouping ();

		[Static]
		[Export ("endGrouping")]
		void EndGrouping ();

		[Static]
		[Export ("currentContext")]
		NSAnimationContext CurrentContext { get; }

		//Detected properties
		[Export ("duration")]
		double Duration { get; set; }

		[Mac (10, 7), Export ("completionHandler", ArgumentSemantic.Copy)]
		Action CompletionHandler { get; set; }

		[Static]
		[Mac (10, 7), Export ("runAnimationGroup:completionHandler:")]
		void RunAnimation (Action<NSAnimationContext> changes, [NullAllowed] Action completionHandler);

		[Mac (10, 7), Export ("timingFunction", ArgumentSemantic.Strong)]
		CAMediaTimingFunction TimingFunction { get; set; }

		[Mac (10, 8), Export ("allowsImplicitAnimation")]
		bool AllowsImplicitAnimation { get; set; }
	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSAlertDelegate)})]
	interface NSAlert {
		[Static, Export ("alertWithError:")]
		NSAlert WithError (NSError  error);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use constructor instead.")]
		[Static, Export ("alertWithMessageText:defaultButton:alternateButton:otherButton:informativeTextWithFormat:")]
		NSAlert WithMessage([NullAllowed] string message, [NullAllowed] string defaultButton, [NullAllowed] string alternateButton, [NullAllowed]  string otherButton, string full);
	
		[Export ("messageText")]
		string MessageText { get; set; }
	
		[Export ("informativeText")]
		string InformativeText { get; set; }
	
		[Export ("icon", ArgumentSemantic.Retain)]
		NSImage Icon { get; set; }
	
		[Export ("addButtonWithTitle:")]
		NSButton AddButton (string title);
	
		[Export ("buttons")]
		NSButton [] Buttons { get; }
	
		[Export ("showsHelp")]
		bool ShowsHelp { get; set; }
	
		[Export ("helpAnchor")]
		string HelpAnchor { get; set; }
	
		[Export ("alertStyle")]
		NSAlertStyle AlertStyle { get; set; }
	
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSAlertDelegate Delegate { get; set; }
	
		[Export ("showsSuppressionButton")]
		bool ShowsSuppressionButton { get; set; } 
	
		[Export ("suppressionButton")]
		NSButton SuppressionButton { get; } 
	
		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; } 
	
		[Export ("layout")]
		void Layout ();
	
		[Export ("runModal")]
		nint RunModal ();
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use BeginSheetModalForWindow (NSWindow sheetWindow, Action<nint> handler) instead.")]
		[Export ("beginSheetModalForWindow:modalDelegate:didEndSelector:contextInfo:")]
		void BeginSheet ([NullAllowed] NSWindow  window, [NullAllowed] NSObject modalDelegate, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);

#if XAMCORE_2_0	// This conflicts with a BeginSheet override in src/AppKit/NSAlert.cs and requires a cast if you pass in delegate {}.
		[Mac (10,9)]
		[Export ("beginSheetModalForWindow:completionHandler:")]
		[Async]
		void BeginSheet ([NullAllowed]NSWindow Window, [NullAllowed] Action<NSModalResponse> handler);
#endif

		[Export ("window")]
		NSPanel Window  { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSAlertDelegate {
		[Export ("alertShowHelp:"), DelegateName ("NSAlertPredicate"), DefaultValue (false)]
		bool ShowHelp (NSAlert  alert);
	}

	interface NSApplicationDidFinishLaunchingEventArgs {
		[Export ("NSApplicationLaunchIsDefaultLaunchKey")]
		bool IsLaunchDefault { get; }

		[ProbePresence, Export ("NSApplicationLaunchUserNotificationKey")]
		bool IsLaunchFromUserNotification { get; }
	}

	[Mac (10,9)]
	[BaseType (typeof (NSObject))]
	interface NSAppearance : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithAppearanceNamed:bundle:")]
		IntPtr Constructor (string name, [NullAllowed] NSBundle bundle);

		[Mac (10,9)]
		[Export ("name")]
		string Name { get; }

		[Mac (10,10)]
		[Export ("allowsVibrancy")]
		bool AllowsVibrancy { get; }

		[Static, Export ("currentAppearance")]
		NSAppearance CurrentAppearance { get; [Bind("setCurrentAppearance:")] set; }

		[Static, Export ("appearanceNamed:")]
		NSAppearance GetAppearance (NSString name);

		[Mac (10,9)]
		[Field ("NSAppearanceNameAqua")]
		NSString NameAqua { get; }

		[Availability (Introduced = Platform.Mac_10_9, Deprecated = Platform.Mac_10_10)]
		[Field ("NSAppearanceNameLightContent")]
		NSString NameLightContent { get; }

		[Mac (10,10)]
		[Field ("NSAppearanceNameVibrantDark")]
		NSString NameVibrantDark { get; }

		[Mac (10,10)]
		[Field ("NSAppearanceNameVibrantLight")]
		NSString NameVibrantLight { get; }
	}

	[Mac (10,9)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSAppearanceCustomization {

		[Mac (10,9)]
		[Export ("appearance", ArgumentSemantic.Strong)]
		NSAppearance Appearance { get; set; }

		[Mac (10,9)]
		[Export ("effectiveAppearance", ArgumentSemantic.Strong)]
		NSAppearance EffectiveAppearance { get; }
	}


	[BaseType (typeof (NSResponder), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSApplicationDelegate) })]
	[DisableDefaultCtor] // An uncaught exception was raised: Creating more than one Application
	interface NSApplication : NSAccessibilityElementProtocol, NSAccessibility {
		[Export ("sharedApplication"), Static, ThreadSafe]
		NSApplication SharedApplication { get; }
	
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSApplicationDelegate Delegate { get; set; }
	
		[Export ("context")]
		[Availability (Deprecated = Platform.Mac_10_12, Message = "This method always returns null. If you need access to the current drawing context, use NSGraphicsContext.CurrentContext inside of a draw operation.")]
		NSGraphicsContext Context { get; }
	
		[Export ("hide:")]
		void Hide (NSObject sender);
	
		[Export ("unhide:")]
		void Unhide (NSObject sender);
	
		[Export ("unhideWithoutActivation")]
		void UnhideWithoutActivation ();
	
		[Export ("windowWithWindowNumber:")]
		NSWindow WindowWithWindowNumber (nint windowNum);
	
		[Export ("mainWindow")]
		NSWindow MainWindow { get; }
	
		[Export ("keyWindow")]
		NSWindow KeyWindow { get; }
	
		[Export ("isActive")]
		bool Active { get; }
	
		[Export ("isHidden")]
		bool Hidden { get; }
	
		[Export ("isRunning")]
		bool Running { get; }
	
		[Export ("deactivate")]
		void Deactivate ();
	
		[Export ("activateIgnoringOtherApps:")]
		void ActivateIgnoringOtherApps (bool flag);
	
		[Export ("hideOtherApplications:")]
		void HideOtherApplications (NSObject sender);
	
		[Export ("unhideAllApplications:")]
		void UnhideAllApplications (NSObject sender);
	
		[Export ("finishLaunching")]
		void FinishLaunching ();
	
		[Export ("run")]
		void Run ();
	
		[Export ("runModalForWindow:")]
		nint RunModalForWindow (NSWindow theWindow);
	
		[Export ("stop:")]
		void Stop (NSObject sender);
	
		[Export ("stopModal")]
		void StopModal ();
	
		[Export ("stopModalWithCode:")]
		void StopModalWithCode (nint returnCode);
	
		[Export ("abortModal"), ThreadSafe]
		void AbortModal ();
	
		[Export ("modalWindow")]
		NSWindow ModalWindow { get; }
	
		[Export ("beginModalSessionForWindow:")]
		IntPtr BeginModalSession (NSWindow theWindow);
	
		[Export ("runModalSession:")]
		nint RunModalSession (IntPtr session);
	
		[Export ("endModalSession:")]
		void EndModalSession (IntPtr session);
	
		[Export ("terminate:")]
		void Terminate ([NullAllowed] NSObject sender);
	
		[Export ("requestUserAttention:")]
		nint RequestUserAttention (NSRequestUserAttentionType requestType);
	
		[Export ("cancelUserAttentionRequest:")]
		void CancelUserAttentionRequest (nint request);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use NSWindow.BeginSheet instead.")]
		[Export ("beginSheet:modalForWindow:modalDelegate:didEndSelector:contextInfo:")]
		void BeginSheet (NSWindow sheet, NSWindow docWindow, [NullAllowed] NSObject modalDelegate, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use NSWindow.EndSheet instead.")]
		[Export ("endSheet:")]
		void EndSheet (NSWindow sheet);
	
		[Availability (Deprecated = Platform.Mac_10_9)]
		[Export ("endSheet:returnCode:")]
		void EndSheet (NSWindow  sheet, nint returnCode);
	
		[Export ("nextEventMatchingMask:untilDate:inMode:dequeue:"), Protected]
		NSEvent NextEvent (NSEventMask mask, [NullAllowed] NSDate expiration, NSString runLoopMode, bool deqFlag);

#if !XAMCORE_4_0
		[Obsolete ("Use the 'NextEvent (NSEventMask, NSDate, [NSRunLoopMode|NSString], bool)' overloads instead.")]
		[Wrap ("NextEvent ((NSEventMask) (ulong) mask, expiration, (NSString) mode, deqFlag)", IsVirtual = true), Protected]
		NSEvent NextEvent (nuint mask, NSDate expiration, string mode, bool deqFlag);

		// NSEventMask must be casted to nuint to preserve the NSEventMask.Any special value on 64 bit systems. NSEventMask is not [Native].
		[Obsolete ("Use the 'NextEvent (NSEventMask, NSDate, [NSRunLoopMode|NSString], bool)' overloads instead.")]
		[Wrap ("NextEvent (mask, expiration, (NSString) mode, deqFlag)")]
		NSEvent NextEvent (NSEventMask mask, NSDate expiration, string mode, bool deqFlag);
#endif

		// NSEventMask must be casted to nuint to preserve the NSEventMask.Any special value on 64 bit systems. NSEventMask is not [Native].
		[Wrap ("NextEvent (mask, expiration, runLoopMode.GetConstant (), deqFlag)")]
		NSEvent NextEvent (NSEventMask mask, NSDate expiration, NSRunLoopMode runLoopMode, bool deqFlag);

		[Export ("discardEventsMatchingMask:beforeEvent:"), Protected]
		void DiscardEvents (nuint mask, NSEvent lastEvent);
	
                [ThreadSafe]
		[Export ("postEvent:atStart:")]
		void PostEvent (NSEvent theEvent, bool atStart);
	
		[Export ("currentEvent")]
		NSEvent CurrentEvent { get; }
	
		[Export ("sendEvent:")]
		void SendEvent (NSEvent theEvent);
	
		[Export ("preventWindowOrdering")]
		void PreventWindowOrdering ();
	
		[Export ("makeWindowsPerform:inOrder:")]
		[Availability (Deprecated = Platform.Mac_10_12, Message = "Use EnumerateWindows instead.")]
		NSWindow MakeWindowsPerform (Selector aSelector, bool inOrder);
	
		[Export ("windows")]
		NSWindow [] Windows { get; }
	
		[Export ("setWindowsNeedUpdate:")]
		void SetWindowsNeedUpdate (bool needUpdate);
	
		[Export ("updateWindows")]
		void UpdateWindows ();
	
#if !XAMCORE_2_0
		[Export ("setMainMenu:")]
		[Obsolete ("Use 'MainMenu' property.")]
		[Sealed]
		void SetMainMenu (NSMenu  aMenu);
#endif
	
		[Export ("mainMenu", ArgumentSemantic.Retain)]
		NSMenu MainMenu { get; set; }
	
		[Export ("helpMenu", ArgumentSemantic.Retain)]
		NSMenu HelpMenu { get; [NullAllowed] set; }
	
		[Export ("applicationIconImage", ArgumentSemantic.Retain)]
		NSImage ApplicationIconImage { get; set; }
	
		[Export ("activationPolicy"), Protected]
		NSApplicationActivationPolicy GetActivationPolicy ();

		[Export ("setActivationPolicy:"), Protected]
		bool SetActivationPolicy (NSApplicationActivationPolicy activationPolicy);

		[Export ("dockTile")]
		NSDockTile DockTile { get; }
	
		[Export ("sendAction:to:from:")]
		bool SendAction (Selector theAction, [NullAllowed] NSObject theTarget, [NullAllowed] NSObject sender);
	
		[Export ("targetForAction:")]
		NSObject TargetForAction (Selector theAction);
	
		[Export ("targetForAction:to:from:")]
		NSObject TargetForAction (Selector theAction, [NullAllowed] NSObject theTarget, [NullAllowed] NSObject sender);
	
		[Export ("tryToPerform:with:")]
		bool TryToPerform (Selector anAction, [NullAllowed] NSObject target);
	
		[Export ("validRequestorForSendType:returnType:")]
		[return: NullAllowed]
		NSObject ValidRequestor (string sendType, string returnType);
	
		[Export ("reportException:")]
		void ReportException (NSException theException);
	
		[Static]
		[Export ("detachDrawingThread:toTarget:withObject:")]
		void DetachDrawingThread (Selector selector, NSObject target, NSObject argument);
	
		[Export ("replyToApplicationShouldTerminate:")]
		void ReplyToApplicationShouldTerminate (bool shouldTerminate);
	
		[Export ("replyToOpenOrPrint:")]
		void ReplyToOpenOrPrint (NSApplicationDelegateReply reply);
	
		[Export ("orderFrontCharacterPalette:")]
		void OrderFrontCharacterPalette (NSObject sender);
	
		[Export ("presentationOptions")]
		NSApplicationPresentationOptions PresentationOptions { get; set; }
	
		[Export ("currentSystemPresentationOptions")]
		NSApplicationPresentationOptions CurrentSystemPresentationOptions { get; }
	
		[Export ("windowsMenu")]
		NSMenu WindowsMenu { get; set; }
	
		[Export ("arrangeInFront:")]
		void ArrangeInFront (NSObject sender);
	
		[Export ("removeWindowsItem:")]
		void RemoveWindowsItem (NSWindow  win);
	
		[Export ("addWindowsItem:title:filename:")]
		void AddWindowsItem (NSWindow  win, string title, bool isFilename);
	
		[Export ("changeWindowsItem:title:filename:")]
		void ChangeWindowsItem (NSWindow  win, string title, bool isFilename);
	
		[Export ("updateWindowsItem:")]
		void UpdateWindowsItem (NSWindow  win);
	
		[Export ("miniaturizeAll:")]
		void MiniaturizeAll (NSObject sender);
	
		[Export ("isFullKeyboardAccessEnabled")]
		bool FullKeyboardAccessEnabled { get; }

		[Export ("servicesProvider")]
		NSObject ServicesProvider { get; set; }
	
		[Export ("userInterfaceLayoutDirection")]
#if !XAMCORE_4_0
		NSApplicationLayoutDirection UserInterfaceLayoutDirection { get; }
#else
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; }
#endif

		[Export ("servicesMenu")]
		NSMenu ServicesMenu { get; set; }

		// From NSColorPanel
		[Export ("orderFrontColorPanel:")]
		void OrderFrontColorPanel (NSObject sender);

		[Mac (10, 7), Export ("disableRelaunchOnLogin"), ThreadSafe]
		void DisableRelaunchOnLogin ();

		[Mac (10, 7), Export ("enableRelaunchOnLogin"), ThreadSafe]
		void EnableRelaunchOnLogin ();

		[Mac (10, 7), Export ("enabledRemoteNotificationTypes")]
		NSRemoteNotificationType EnabledRemoteNotificationTypes ();

		[Mac (10, 7), Export ("registerForRemoteNotificationTypes:")]
		void RegisterForRemoteNotificationTypes (NSRemoteNotificationType types);

		[Mac (10, 7), Export ("unregisterForRemoteNotifications")]
		void UnregisterForRemoteNotifications ();

		[Notification, Field ("NSApplicationDidBecomeActiveNotification")]
		NSString DidBecomeActiveNotification { get; }

		[Notification, Field ("NSApplicationDidHideNotification")]
		NSString DidHideNotification { get; }

		[Notification (typeof (NSApplicationDidFinishLaunchingEventArgs)), Field ("NSApplicationDidFinishLaunchingNotification")]
		NSString DidFinishLaunchingNotification { get; }

		[Notification, Field ("NSApplicationDidResignActiveNotification")]
		NSString DidResignActiveNotification { get; }

		[Notification, Field ("NSApplicationDidUnhideNotification")]
		NSString DidUnhideNotification { get; }

		[Notification, Field ("NSApplicationDidUpdateNotification")]
		NSString DidUpdateNotification { get; }

		[Notification, Field ("NSApplicationWillBecomeActiveNotification")]
		NSString WillBecomeActiveNotification { get; }

		[Notification, Field ("NSApplicationWillHideNotification")]
		NSString WillHideNotification { get; }

		[Notification, Field ("NSApplicationWillFinishLaunchingNotification")]
		NSString WillFinishLaunchingNotification { get; }

		[Notification, Field ("NSApplicationWillResignActiveNotification")]
		NSString WillResignActiveNotification { get; }

		[Notification, Field ("NSApplicationWillUnhideNotification")]
		NSString WillUnhideNotification { get; }

		[Notification, Field ("NSApplicationWillUpdateNotification")]
		NSString WillUpdateNotification { get; }

		[Notification, Field ("NSApplicationWillTerminateNotification")]
		NSString WillTerminateNotification { get; }

		[Notification, Field ("NSApplicationDidChangeScreenParametersNotification")]
		NSString DidChangeScreenParametersNotification { get; }

		[Mac (10, 7), Field ("NSApplicationLaunchIsDefaultLaunchKey")]
		NSString LaunchIsDefaultLaunchKey  { get; }

		[Mac (10, 7), Field ("NSApplicationLaunchRemoteNotificationKey")]
		NSString LaunchRemoteNotificationKey { get; }

		[Mac (10, 7), Field ("NSApplicationLaunchUserNotificationKey")]
		NSString LaunchUserNotificationKey { get; }

		[Notification, Field ("NSApplicationDidFinishRestoringWindowsNotification")]
		NSString DidFinishRestoringWindowsNotification { get; }

		[Export ("occlusionState")]
		[Mac (10,9)]
		NSApplicationOcclusionState OcclusionState { get; }

		// This comes from the NSWindowRestoration category (defined on NSApplication: '@interface NSApplication (NSWindowRestoration)')
		// Also can't call it 'RestoreWindow', because that's already in use.
		[Export ("restoreWindowWithIdentifier:state:completionHandler:")]
		bool RestoreWindowWithIdentifier (string identifier, NSCoder state, NSWindowCompletionHandler onCompletion);

		// This one comes from the NSRestorableStateExtension category ('@interface NSApplication (NSRestorableStateExtension)')
		[Mac (10, 7)]
		[Export ("extendStateRestoration")]
		void ExtendStateRestoration ();

		// This one comes from the NSRestorableStateExtension category ('@interface NSApplication (NSRestorableStateExtension)')
		[Mac (10, 7)]
		[Export ("completeStateRestoration")]
		void CompleteStateRestoration ();

#if XAMCORE_4_0
		[Export ("registerServicesMenuSendTypes:returnTypes:"), EventArgs ("NSApplicationRegister")]
		void RegisterServicesMenu (string [] sendTypes, string [] returnTypes);

		[Export ("orderFrontStandardAboutPanel:")]
		void OrderFrontStandardAboutPanel (NSObject sender);

		[Export ("orderFrontStandardAboutPanelWithOptions:")]
		void OrderFrontStandardAboutPanelWithOptions (NSDictionary optionsDictionary);
#else
		[Export ("registerServicesMenuSendTypes:returnTypes:"), EventArgs ("NSApplicationRegister")]
		void RegisterServicesMenu2 (string [] sendTypes, string [] returnTypes);

		[Export ("orderFrontStandardAboutPanel:"), EventArgs ("NSObject")]
		void OrderFrontStandardAboutPanel2 (NSObject sender);

		[Export ("orderFrontStandardAboutPanelWithOptions:"), EventArgs ("NSDictionary")]
		void OrderFrontStandardAboutPanelWithOptions2 (NSDictionary optionsDictionary);
#endif

		[Mac (10,12)]
		[Export ("enumerateWindowsWithOptions:usingBlock:")]
		void EnumerateWindows (NSWindowListOptions options, NSApplicationEnumerateWindowsHandler block);
	}

	[Static]
	interface NSAboutPanelOption {
		[Mac (10, 13)]
		[Field ("NSAboutPanelOptionCredits")]
		NSString Credits { get; }

		[Mac (10, 13)]
		[Field ("NSAboutPanelOptionApplicationName")]
		NSString ApplicationName { get; }

		[Mac (10, 13)]
		[Field ("NSAboutPanelOptionApplicationIcon")]
		NSString ApplicationIcon { get; }

		[Mac (10, 13)]
		[Field ("NSAboutPanelOptionVersion")]
		NSString Version { get; }

		[Mac (10, 13)]
		[Field ("NSAboutPanelOptionApplicationVersion")]
		NSString ApplicationVersion { get; }
	}

	delegate void NSApplicationEnumerateWindowsHandler (NSWindow window, ref bool stop);
	delegate void ContinueUserActivityRestorationHandler (NSObject [] restorableObjects);
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSApplicationDelegate {
		[Export ("applicationShouldTerminate:"), DelegateName ("NSApplicationTermination"), DefaultValue (NSApplicationTerminateReply.Now)]
		NSApplicationTerminateReply ApplicationShouldTerminate (NSApplication  sender);
	
		[Export ("application:openFile:"), DelegateName ("NSApplicationFile"), DefaultValue (false)]
		bool OpenFile (NSApplication sender, string  filename);
	
		[Export ("application:openFiles:"), EventArgs ("NSApplicationFiles")]
		void OpenFiles (NSApplication sender, string [] filenames);
	
		[Export ("application:openTempFile:"), DelegateName ("NSApplicationFile"), DefaultValue (false)]
		bool OpenTempFile (NSApplication sender, string  filename);
	
		[Export ("applicationShouldOpenUntitledFile:"), DelegateName ("NSApplicationPredicate"), DefaultValue (false)]
		bool ApplicationShouldOpenUntitledFile (NSApplication  sender);
	
		[Export ("applicationOpenUntitledFile:"), DelegateName ("NSApplicationPredicate"), DefaultValue (false)]
		bool ApplicationOpenUntitledFile (NSApplication sender);
	
		[Export ("application:openFileWithoutUI:"), DelegateName ("NSApplicationFileCommand"), DefaultValue (false)]
		bool OpenFileWithoutUI (NSObject sender, string filename);
	
		[Export ("application:printFile:"), DelegateName ("NSApplicationFile"), DefaultValue (false)]
		bool PrintFile (NSApplication sender, string filename);
	
		[Export ("application:printFiles:withSettings:showPrintPanels:"), DelegateName ("NSApplicationPrint"), DefaultValue (NSApplicationPrintReply.Failure)]
		NSApplicationPrintReply PrintFiles (NSApplication application, string [] fileNames, NSDictionary printSettings, bool showPrintPanels);
	
		[Export ("applicationShouldTerminateAfterLastWindowClosed:"), DelegateName ("NSApplicationPredicate"), DefaultValue (false)]
		bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender);
	
		[Export ("applicationShouldHandleReopen:hasVisibleWindows:"), DelegateName ("NSApplicationReopen"), DefaultValue (false)]
		bool ApplicationShouldHandleReopen (NSApplication sender, bool hasVisibleWindows);
	
		[Export ("applicationDockMenu:"), DelegateName ("NSApplicationMenu"), DefaultValue (null)]
		NSMenu ApplicationDockMenu (NSApplication sender);
	
		[Export ("application:willPresentError:"), DelegateName ("NSApplicationError"), DefaultValue (null)]
		NSError WillPresentError (NSApplication application, NSError error);
	
		[Export ("applicationWillFinishLaunching:"), EventArgs ("NSNotification")]
		void WillFinishLaunching (NSNotification notification);
	
		[Export ("applicationDidFinishLaunching:"), EventArgs ("NSNotification")]
		void DidFinishLaunching (NSNotification notification);
	
		[Export ("applicationWillHide:"), EventArgs ("NSNotification")]
		void WillHide (NSNotification notification);
	
		[Export ("applicationDidHide:"), EventArgs ("NSNotification")]
		void DidHide (NSNotification notification);
	
		[Export ("applicationWillUnhide:"), EventArgs ("NSNotification")]
		void WillUnhide (NSNotification notification);
	
		[Export ("applicationDidUnhide:"), EventArgs ("NSNotification")]
		void DidUnhide (NSNotification notification);
	
		[Export ("applicationWillBecomeActive:"), EventArgs ("NSNotification")]
		void WillBecomeActive (NSNotification notification);
	
		[Export ("applicationDidBecomeActive:"), EventArgs ("NSNotification")]
		void DidBecomeActive (NSNotification notification);
	
		[Export ("applicationWillResignActive:"), EventArgs ("NSNotification")]
		void WillResignActive (NSNotification notification);
	
		[Export ("applicationDidResignActive:"), EventArgs ("NSNotification")]
		void DidResignActive (NSNotification notification);
	
		[Export ("applicationWillUpdate:"), EventArgs ("NSNotification")]
		void WillUpdate (NSNotification notification);
	
		[Export ("applicationDidUpdate:"), EventArgs ("NSNotification")]
		void DidUpdate (NSNotification notification);
	
		[Export ("applicationWillTerminate:"), EventArgs ("NSNotification")]
		void WillTerminate (NSNotification notification);
	
		[Export ("applicationDidChangeScreenParameters:"), EventArgs ("NSNotification")]
		void ScreenParametersChanged (NSNotification notification);

#if !XAMCORE_4_0 // Needs to move from delegate in next API break
		[Obsolete ("Use the 'RegisterServicesMenu2' on NSApplication.")]
		[Export ("registerServicesMenuSendTypes:returnTypes:"), EventArgs ("NSApplicationRegister")]
		void RegisterServicesMenu (string [] sendTypes, string [] returnTypes);
	
		[Obsolete ("Use the 'INSServicesMenuRequestor' protocol.")]
		[Export ("writeSelectionToPasteboard:types:"), DelegateName ("NSApplicationSelection"), DefaultValue (false)]
		bool WriteSelectionToPasteboard (NSPasteboard board, string [] types);
	
		[Obsolete ("Use the 'INSServicesMenuRequestor' protocol.")]
		[Export ("readSelectionFromPasteboard:"), DelegateName ("NSPasteboardPredicate"), DefaultValue (false)]
		bool ReadSelectionFromPasteboard (NSPasteboard pboard);
	
		[Obsolete ("Use the 'OrderFrontStandardAboutPanel2' on NSApplication.")]
		[Export ("orderFrontStandardAboutPanel:"), EventArgs ("NSObject")]
		void OrderFrontStandardAboutPanel (NSObject sender);
	
		[Obsolete ("Use the 'OrderFrontStandardAboutPanelWithOptions2' on NSApplication.")]
		[Export ("orderFrontStandardAboutPanelWithOptions:"), EventArgs ("NSDictionary")]
		void OrderFrontStandardAboutPanelWithOptions (NSDictionary optionsDictionary);
#endif

		[Mac (10, 7), Export ("application:didRegisterForRemoteNotificationsWithDeviceToken:"), EventArgs ("NSData")]
		void RegisteredForRemoteNotifications (NSApplication application, NSData deviceToken);

		[Mac (10, 7), Export ("application:didFailToRegisterForRemoteNotificationsWithError:"), EventArgs ("NSError", true)]
		void FailedToRegisterForRemoteNotifications (NSApplication application, NSError error);

		[Mac (10, 7), Export ("application:didReceiveRemoteNotification:"), EventArgs ("NSDictionary")]
		void ReceivedRemoteNotification (NSApplication application, NSDictionary userInfo);

		[Mac (10, 7), Export ("application:willEncodeRestorableState:"), EventArgs ("NSCoder")]
		void WillEncodeRestorableState (NSApplication app, NSCoder encoder);

		[Mac (10, 7), Export ("application:didDecodeRestorableState:"), EventArgs ("NSCoder")]
		void DecodedRestorableState (NSApplication app, NSCoder state);

#if XAMCORE_2_0
		[Mac (10,10, onlyOn64 : true)]
		[Export ("application:willContinueUserActivityWithType:"), DelegateName ("NSApplicationUserActivityType"), DefaultValue (false)]
		bool WillContinueUserActivity (NSApplication application, string userActivityType);

		[Mac (10,10, onlyOn64 : true)]
		[Export ("application:continueUserActivity:restorationHandler:"), DelegateName ("NSApplicationContinueUserActivity"), DefaultValue (false)]
		bool ContinueUserActivity (NSApplication application, NSUserActivity userActivity, ContinueUserActivityRestorationHandler restorationHandler);
#endif

		[Mac (10,10, onlyOn64 : true)]
		[Export ("application:didFailToContinueUserActivityWithType:error:"), EventArgs ("NSApplicationFailed"), DefaultValue (false)]
		void FailedToContinueUserActivity (NSApplication application, string userActivityType, NSError error);

#if XAMCORE_2_0
		[Mac (10,10, onlyOn64 : true)]
		[Export ("application:didUpdateUserActivity:"), EventArgs ("NSApplicationUpdatedUserActivity"), DefaultValue (false)]
		void UpdatedUserActivity (NSApplication application, NSUserActivity userActivity);

		[Mac (10,12, onlyOn64 : true)]
		[Export ("application:userDidAcceptCloudKitShareWithMetadata:"), EventArgs ("NSApplicationUserAcceptedCloudKitShare")]
		void UserDidAcceptCloudKitShare (NSApplication application, CKShareMetadata metadata);
#endif

		[Mac (10,13), EventArgs ("NSApplicationOpenUrls")]
		[Export ("application:openURLs:")]
		void OpenUrls (NSApplication application, NSUrl[] urls);
	}

	[Protocol]
	interface NSServicesMenuRequestor
	{
		[Export ("writeSelectionToPasteboard:types:")]
		bool WriteSelectionToPasteboard (NSPasteboard pboard, string[] types);

		[Export ("readSelectionFromPasteboard:")]
		bool ReadSelectionFromPasteboard (NSPasteboard pboard);
	}

	[Mac (10, 12, 2)]
	[Category]
	[BaseType (typeof(NSApplication))]
	interface NSApplication_NSTouchBarCustomization
	{
		[Export ("isAutomaticCustomizeTouchBarMenuItemEnabled")]
		bool GetAutomaticCustomizeTouchBarMenuItemEnabled ();

		[Export ("setAutomaticCustomizeTouchBarMenuItemEnabled:")]
		void SetAutomaticCustomizeTouchBarMenuItemEnabled (bool enabled);

		[Export ("toggleTouchBarCustomizationPalette:")]
		void ToggleTouchBarCustomizationPalette ([NullAllowed] NSObject sender);
	}

		
	[BaseType (typeof (NSObjectController))]
	interface NSArrayController {
		[Export ("rearrangeObjects")]
		void RearrangeObjects ();

		[Export ("automaticRearrangementKeyPaths")]
		NSObject [] AutomaticRearrangementKeyPaths ();

		[Export ("didChangeArrangementCriteria")]
		void DidChangeArrangementCriteria ();

		[Export ("arrangeObjects:")]
		NSObject [] ArrangeObjects (NSObject [] objects);

		[Export ("arrangedObjects")]
		NSObject [] ArrangedObjects ();

		[Export ("addSelectionIndexes:")]
		bool AddSelectionIndexes (NSIndexSet indexes);

		[Export ("removeSelectionIndexes:")]
		bool RemoveSelectionIndexes (NSIndexSet indexes);

		[Export ("addSelectedObjects:")]
		bool AddSelectedObjects (NSObject [] objects);

		[Export ("removeSelectedObjects:")]
		bool RemoveSelectedObjects (NSObject [] objects);

		[Export ("add:")]
		void Add (NSObject sender);

		[Export ("remove:")]
		void RemoveOp (NSObject sender);

		[Export ("insert:")]
		void Insert (NSObject sender);

		[Export ("canInsert")]
		bool CanInsert ();

		[Export ("selectNext:")]
		void SelectNext (NSObject sender);

		[Export ("selectPrevious:")]
		void SelectPrevious (NSObject sender);

		[Export ("canSelectNext")]
		bool CanSelectNext ();

		[Export ("canSelectPrevious")]
		bool CanSelectPrevious ();

		[Export ("addObject:")]
		void AddObject (NSObject aObject);

		[Export ("addObjects:")]
		void AddObjects (NSArray objects);

		[Export ("insertObject:atArrangedObjectIndex:")]
		void Insert (NSObject aObject, nint index);

		[Export ("insertObjects:atArrangedObjectIndexes:")]
		void Insert (NSObject [] objects, NSIndexSet indexes);

		[Export ("removeObjectAtArrangedObjectIndex:")]
		void RemoveAt (nint index);

		[Export ("removeObjectsAtArrangedObjectIndexes:")]
		void Remove (NSIndexSet indexes);

		[Export ("removeObject:")]
		void Remove (NSObject aObject);

		[Export ("removeObjects:")]
		void Remove (NSObject [] objects);

		//Detected properties
		[Export ("automaticallyRearrangesObjects")]
		bool AutomaticallyRearrangesObjects { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSObject [] SortDescriptors { get; set; }

		[Export ("filterPredicate", ArgumentSemantic.Retain)]
		NSPredicate FilterPredicate { get; [NullAllowed] set; }

		[Export ("clearsFilterPredicateOnInsertion")]
		bool ClearsFilterPredicateOnInsertion { get; set; }

		[Export ("avoidsEmptySelection")]
		bool AvoidsEmptySelection { get; set; }

		[Export ("preservesSelection")]
		bool PreservesSelection { get; set; }

		[Export ("selectsInsertedObjects")]
		bool SelectsInsertedObjects { get; set; }

		[Export ("alwaysUsesMultipleValuesMarker")]
		bool AlwaysUsesMultipleValuesMarker { get; set; }

		[Export ("selectionIndexes"), Protected]
		NSIndexSet GetSelectionIndexes ();

		[Export ("setSelectionIndexes:"), Protected]
		bool SetSelectionIndexes (NSIndexSet indexes);

		[Export ("selectionIndex"), Protected]
		nuint GetSelectionIndex ();

		[Export ("setSelectionIndex:"), Protected]
		bool SetSelectionIndex (nuint index);

		[Export ("selectedObjects"), Protected]
		NSObject [] GetSelectedObjects ();

		[Export ("setSelectedObjects:"), Protected]
		bool SetSelectedObjects (NSObject [] objects);
	}
	
	[BaseType (typeof (NSObject))]
	interface NSBezierPath : NSCoding, NSCopying {

		[Static]
		[Export ("bezierPathWithRect:")]
		NSBezierPath FromRect (CGRect rect);

		[Static]
		[Export ("bezierPathWithOvalInRect:")]
		NSBezierPath FromOvalInRect (CGRect rect);

		[Static]
		[Export ("bezierPathWithRoundedRect:xRadius:yRadius:")]
		NSBezierPath FromRoundedRect (CGRect rect, nfloat xRadius, nfloat yRadius);

		[Static]
		[Export ("fillRect:")]
		void FillRect (CGRect rect);

		[Static]
		[Export ("strokeRect:")]
		void StrokeRect (CGRect rect);

		[Static]
		[Export ("clipRect:")]
		void ClipRect (CGRect rect);

		[Static]
		[Export ("strokeLineFromPoint:toPoint:")]
		void StrokeLine (CGPoint point1, CGPoint point2);

		//IntPtr is exposed because the packedGlyphs should be treated as a "black box"
		[Static]
		[Export ("drawPackedGlyphs:atPoint:")]
		void DrawPackedGlyphsAtPoint (IntPtr packedGlyphs, CGPoint point);

		[Export ("moveToPoint:")]
		void MoveTo (CGPoint point);

		[Export ("lineToPoint:")]
		void LineTo (CGPoint point);

		[Export ("curveToPoint:controlPoint1:controlPoint2:")]
		void CurveTo (CGPoint endPoint, CGPoint controlPoint1, CGPoint controlPoint2);

		[Export ("closePath")]
		void ClosePath ();

		[Export ("removeAllPoints")]
		void RemoveAllPoints ();

		[Export ("relativeMoveToPoint:")]
		void RelativeMoveTo (CGPoint point);

		[Export ("relativeLineToPoint:")]
		void RelativeLineTo (CGPoint point);

		[Export ("relativeCurveToPoint:controlPoint1:controlPoint2:")]
		void RelativeCurveTo (CGPoint endPoint, CGPoint controlPoint1, CGPoint controlPoint2);

		[Export ("getLineDash:count:phase:"), Internal]
		void _GetLineDash (IntPtr pattern, out nint count, out nfloat phase);

		[Export ("setLineDash:count:phase:"), Internal]
		void _SetLineDash (IntPtr pattern, nint count, nfloat phase);

		[Export ("stroke")]
		void Stroke ();

		[Export ("fill")]
		void Fill ();

		[Export ("addClip")]
		void AddClip ();

		[Export ("setClip")]
		void SetClip ();

		[Export ("bezierPathByFlatteningPath")]
		NSBezierPath BezierPathByFlatteningPath ();

		[Export ("bezierPathByReversingPath")]
		NSBezierPath BezierPathByReversingPath ();

		[Export ("transformUsingAffineTransform:")]
		void TransformUsingAffineTransform (NSAffineTransform transform);

		[Export ("isEmpty")]
		bool IsEmpty { get; }

		[Export ("currentPoint")]
		CGPoint CurrentPoint { get; }

		[Export ("controlPointBounds")]
		CGRect ControlPointBounds { get; }

		[Export ("bounds")]
		CGRect Bounds { get; }

		[Export ("elementCount")]
		nint ElementCount { get; }

		[Export ("elementAtIndex:associatedPoints:"), Internal]
		NSBezierPathElement _ElementAt (nint index, IntPtr points);

		[Export ("elementAtIndex:")]
		NSBezierPathElement ElementAt (nint index);

		[Export ("setAssociatedPoints:atIndex:"), Internal]
		void _SetAssociatedPointsAtIndex (IntPtr points, nint index);

		[Export ("appendBezierPath:")]
		void AppendPath (NSBezierPath path);

		[Export ("appendBezierPathWithRect:")]
		void AppendPathWithRect (CGRect rect);

		[Export ("appendBezierPathWithPoints:count:"), Internal]
		void _AppendPathWithPoints (IntPtr points, nint count);

		[Export ("appendBezierPathWithOvalInRect:")]
		void AppendPathWithOvalInRect (CGRect rect);

		[Export ("appendBezierPathWithArcWithCenter:radius:startAngle:endAngle:clockwise:")]
		void AppendPathWithArc (CGPoint center, nfloat radius, nfloat startAngle, nfloat endAngle, bool clockwise);

		[Export ("appendBezierPathWithArcWithCenter:radius:startAngle:endAngle:")]
		void AppendPathWithArc (CGPoint center, nfloat radius, nfloat startAngle, nfloat endAngle);

		[Export ("appendBezierPathWithArcFromPoint:toPoint:radius:")]
		void AppendPathWithArc (CGPoint point1, CGPoint point2, nfloat radius);

		[Availability (Obsoleted = Platform.Mac_10_13, Message = "Use 'AppendPathWithCGGlyph (CGGlyph, NSFont)' instead.")]
		[Export ("appendBezierPathWithGlyph:inFont:")]
		void AppendPathWithGlyph (uint /* NSGlyph = unsigned int */ glyph, NSFont font);

		[Export ("appendBezierPathWithGlyphs:count:inFont:"), Internal]
		void _AppendPathWithGlyphs (IntPtr glyphs, nint count, NSFont font);

		//IntPtr is exposed because the packedGlyphs should be treated as a "black box"
		[Availability (Obsoleted = Platform.Mac_10_13, Message = "Use 'Append (uint[], NSFont)' instead.")]
		[Export ("appendBezierPathWithPackedGlyphs:")]
		void AppendPathWithPackedGlyphs (IntPtr packedGlyphs);

		[Export ("appendBezierPathWithRoundedRect:xRadius:yRadius:")]
		void AppendPathWithRoundedRect (CGRect rect, nfloat xRadius, nfloat yRadius);

		[Export ("containsPoint:")]
		bool Contains (CGPoint point);

		//Detected properties
		[Static]
		[Export ("defaultMiterLimit")]
		nfloat DefaultMiterLimit { get; set; }

		[Static]
		[Export ("defaultFlatness")]
		nfloat DefaultFlatness { get; set; }

		[Static]
		[Export ("defaultWindingRule")]
		NSWindingRule DefaultWindingRule { get; set; }

		[Static]
		[Export ("defaultLineCapStyle")]
		NSLineCapStyle DefaultLineCapStyle { get; set; }

		[Static]
		[Export ("defaultLineJoinStyle")]
		NSLineJoinStyle DefaultLineJoinStyle { get; set; }

		[Static]
		[Export ("defaultLineWidth")]
		nfloat DefaultLineWidth { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("lineCapStyle")]
		NSLineCapStyle LineCapStyle { get; set; }

		[Export ("lineJoinStyle")]
		NSLineJoinStyle LineJoinStyle { get; set; }

		[Export ("windingRule")]
		NSWindingRule WindingRule { get; set; }

		[Export ("miterLimit")]
		nfloat MiterLimit { get; set; }

		[Export ("flatness")]
		nfloat Flatness { get; set; }

		[Mac (10,13)]
		[Export ("appendBezierPathWithCGGlyph:inFont:")]
		void AppendPathWithCGGlyph (CGGlyph glyph, NSFont font);

		[Mac (10,13)]
		[Export ("appendBezierPathWithCGGlyphs:count:inFont:")]
		[Internal]
		void _AppendBezierPathWithCGGlyphs (IntPtr glyphs, nint count, NSFont font);

		[Wrap ("AppendPath (path)")]
		void Append (NSBezierPath path);
	}

	[BaseType (typeof (NSImageRep))]
	[DisableDefaultCtor] // An uncaught exception was raised: -[NSBitmapImageRep init]: unrecognized selector sent to instance 0x686880
	partial interface NSBitmapImageRep : NSSecureCoding {
		[Export ("initWithFocusedViewRect:")]
		IntPtr Constructor (CGRect rect);

		[Export ("initWithBitmapDataPlanes:pixelsWide:pixelsHigh:bitsPerSample:samplesPerPixel:hasAlpha:isPlanar:colorSpaceName:bytesPerRow:bitsPerPixel:")]
		IntPtr Constructor (IntPtr planes, nint width, nint height, nint bps, nint spp, bool alpha, bool isPlanar,
				    string colorSpaceName, nint rBytes, nint pBits);

		[Export ("initWithBitmapDataPlanes:pixelsWide:pixelsHigh:bitsPerSample:samplesPerPixel:hasAlpha:isPlanar:colorSpaceName:bitmapFormat:bytesPerRow:bitsPerPixel:")]
		IntPtr Constructor (IntPtr planes, nint width, nint height, nint bps, nint spp, bool alpha, bool isPlanar, string colorSpaceName,
				    NSBitmapFormat bitmapFormat, nint rBytes, nint pBits);

		[Export ("initWithCGImage:")]
		IntPtr Constructor (CGImage cgImage);

		[Export ("initWithCIImage:")]
		IntPtr Constructor (CIImage ciImage);

		[Static]
		[Export ("imageRepsWithData:")]
		NSImageRep [] ImageRepsWithData (NSData data);

		[Static]
		[Export ("imageRepWithData:")]
		NSImageRep ImageRepFromData (NSData data);

		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[Export ("bitmapData")]
		IntPtr BitmapData { get; }

		[Export ("getBitmapDataPlanes:")]
		void GetBitmapDataPlanes (IntPtr data);

		[Export ("isPlanar")]
		bool IsPlanar { get; }

		[Export ("samplesPerPixel")]
		nint SamplesPerPixel { get; }

		[Export ("bitsPerPixel")]
		nint BitsPerPixel { get; }

		[Export ("bytesPerRow")]
		nint BytesPerRow { get; }

		[Export ("bytesPerPlane")]
		nint BytesPerPlane { get; }

		[Export ("numberOfPlanes")]
		nint Planes { get; }

		[Export ("bitmapFormat")]
		NSBitmapFormat BitmapFormat { get; }

		[Export ("getCompression:factor:")]
		void GetCompressionFactor (out NSTiffCompression compression, out float /* float, not CGFloat */ factor);

		[Export ("setCompression:factor:")]
		void SetCompressionFactor (NSTiffCompression compression, float /* float, not CGFloat */ factor);

		[Export ("TIFFRepresentation")]
		NSData TiffRepresentation { get; }

		[Export ("TIFFRepresentationUsingCompression:factor:")]
		NSData TiffRepresentationUsingCompressionFactor (NSTiffCompression comp, float /* float, not CGFloat */ factor);

		[Static]
		[Export ("TIFFRepresentationOfImageRepsInArray:")]
		NSData ImagesAsTiff (NSImageRep [] imageReps);

		[Static]
		[Export ("TIFFRepresentationOfImageRepsInArray:usingCompression:factor:")]
		NSData ImagesAsTiff (NSImageRep [] imageReps, NSTiffCompression comp, float /* float, not CGFloat */ factor);

		// FIXME: binding
		//[Static]
		//[Export ("getTIFFCompressionTypes:count:")]
		//void GetTiffCompressionTypes (const NSTIFFCompression list, int numTypes);

		[Static]
		[Export ("localizedNameForTIFFCompressionType:")]
		string LocalizedNameForTiffCompressionType (NSTiffCompression compression);

		[Export ("canBeCompressedUsing:")]
		bool CanBeCompressedUsing (NSTiffCompression compression);

		[Export ("colorizeByMappingGray:toColor:blackMapping:whiteMapping:")]
		void Colorize (nfloat midPoint, NSColor midPointColor, NSColor shadowColor, NSColor lightColor);

		[Export ("incrementalLoadFromData:complete:")]
		nint IncrementalLoad (NSData data, bool complete);

		[Export ("setColor:atX:y:")]
		void SetColorAt (NSColor color, nint x, nint y);

		[Export ("colorAtX:y:")]
		NSColor ColorAt (nint x, nint y);

		// FIXME: BINDING
		//[Export ("getPixel:atX:y:")]
		//void GetPixel (int[] p, int x, int y);
		//[Export ("setPixel:atX:y:")]
		//void SetPixel (int[] p, int x, int y);

		[Export ("CGImage")]
		CGImage CGImage { get; }

		[Export ("colorSpace")]
		NSColorSpace ColorSpace { get; }

		[Export ("bitmapImageRepByConvertingToColorSpace:renderingIntent:")]
		NSBitmapImageRep ConvertingToColorSpace (NSColorSpace targetSpace, NSColorRenderingIntent renderingIntent);

		[Export ("bitmapImageRepByRetaggingWithColorSpace:")]
		NSBitmapImageRep RetaggedWithColorSpace (NSColorSpace newSpace);
		
		[Export ("representationUsingType:properties:")]
		NSData RepresentationUsingTypeProperties(NSBitmapImageFileType storageType, [NullAllowed] NSDictionary properties);

		[Field ("NSImageCompressionMethod")]
		NSString CompressionMethod { get; }

		[Field ("NSImageCompressionFactor")]
		NSString CompressionFactor { get; }

		[Field ("NSImageDitherTransparency")]
		NSString DitherTransparency { get; }

		[Field ("NSImageRGBColorTable")]
		NSString RGBColorTable { get; }

		[Field ("NSImageInterlaced")]
		NSString Interlaced { get; }

		[Field ("NSImageColorSyncProfileData")]
		NSString ColorSyncProfileData { get; }

		[Field ("NSImageFrameCount")]
		NSString FrameCount { get; }

		[Field ("NSImageCurrentFrame")]
		NSString CurrentFrame { get; }

		[Field ("NSImageCurrentFrameDuration")]
		NSString CurrentFrameDuration { get; }

		[Field ("NSImageLoopCount")]
		NSString LoopCount { get; }

		[Field ("NSImageGamma")]
		NSString Gamma { get; }

		[Field ("NSImageProgressive")]
		NSString Progressive { get; }

		[Field ("NSImageEXIFData")]
		NSString EXIFData { get; }

		[Field ("NSImageFallbackBackgroundColor")]
		NSString FallbackBackgroundColor { get; }
	}

	[BaseType (typeof (NSView))]
	interface NSBox {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("borderType")]
		NSBorderType BorderType { get; set; }
	
		[Export ("titlePosition")]
		NSTitlePosition TitlePosition { get; set; }
	
		[Export ("boxType")]
		NSBoxType BoxType { get; set; }
	
		[Export ("title")]
		string Title { get; set; }
	
		[Export ("titleFont", ArgumentSemantic.Retain)]
		NSFont TitleFont { get; set; }
	
		[Export ("borderRect")]
		CGRect BorderRect { get; } 
	
		[Export ("titleRect")]
		CGRect TitleRect { get; }
	
		[Export ("titleCell")]
		NSObject TitleCell { get; }
	
		[Export ("sizeToFit")]
		void SizeToFit ();
	
		[Export ("contentViewMargins")]
		CGSize ContentViewMargins { get; set; } 
	
		[Export ("setFrameFromContentFrame:")]
		void SetFrameFromContentFrame (CGRect contentFrame);
	
		[Export ("contentView")]
		NSObject ContentView { get; set; }
	
		[Export ("transparent")]
		bool Transparent { [Bind ("isTransparent")] get; set; }

		[Export ("setTitleWithMnemonic:")]
		[Availability (Deprecated = Platform.Mac_10_8, Message = "For compatability, this method still sets the Title with the ampersand stripped from it.")]
		void SetTitleWithMnemonic (string stringWithMnemonic);

		[Export ("borderWidth")]
		nfloat BorderWidth { get; set; }
	
		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }
	
		[ThreadSafe] // Bug 22909 - This can be called from a non-ui thread <= OS X 10.9
		[Export ("borderColor", ArgumentSemantic.Copy)]
		NSColor BorderColor { get; set; }
	
		[ThreadSafe] // Bug 22909 - This can be called from a non-ui thread <= OS X 10.9
		[Export ("fillColor", ArgumentSemantic.Copy)]
		NSColor FillColor { get; set; }
	}
		
	[BaseType (typeof (NSControl))]
		// , Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSBrowserDelegate)})]
	partial interface NSBrowser {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("loadColumnZero")]
		void LoadColumnZero ();

		[Export ("isLoaded")]
		bool Loaded { get; }

		[Export ("autohidesScroller")]
		bool AutohidesScroller  { get; set; }

		[Export ("itemAtIndexPath:")]
		NSObject ItemAtIndexPath (NSIndexPath indexPath);

		[Export ("itemAtRow:inColumn:")]
		NSObject GetItem (nint row, nint column);

		[Export ("indexPathForColumn:")]
		NSIndexPath IndexPathForColumn (nint column);

		[Export ("isLeafItem:")]
		bool IsLeafItem (NSObject item);

		[Export ("reloadDataForRowIndexes:inColumn:")]
		void ReloadData (NSIndexSet rowIndexes, nint column);

		[Export ("parentForItemsInColumn:")]
		NSObject ParentForItems (nint column);

		[Export ("scrollRowToVisible:inColumn:")]
		void ScrollRowToVisible (nint row, nint column);

		[Export ("setTitle:ofColumn:")]
		void SetTitle (string aString, nint column);

		[Export ("titleOfColumn:")]
		string ColumnTitle (nint column);

		[Export ("pathToColumn:")]
		string ColumnPath (nint column);

		[Export ("clickedColumn")]
		nint ClickedColumn ();

		[Export ("clickedRow")]
		nint ClickedRow ();

		[Export ("selectedColumn")]
		nint SelectedColumn ();

		[Export ("selectedCell")]
		NSObject SelectedCell ();

		[Export ("selectedCellInColumn:")]
		NSObject SelectedCellInColumn (nint column);

		[Export ("selectedCells")]
		NSCell [] SelectedCells ();

		[Export ("selectRow:inColumn:")]
		void Select (nint row, nint column);

		[Export ("selectedRowInColumn:")]
		nint SelectedRow (nint column);

		[Export ("selectionIndexPath", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSIndexPath SelectionIndexPath { get; set; }

		[Export ("selectionIndexPaths", ArgumentSemantic.Copy)]
		NSIndexPath [] SelectionIndexPaths  { get; set; }

		[Export ("selectRowIndexes:inColumn:")]
		void SelectRowIndexes (NSIndexSet indexes, nint column);

		[Export ("selectedRowIndexesInColumn:")]
		NSIndexSet SelectedRowIndexes (nint column);

		[Export ("reloadColumn:")]
		void ReloadColumn (nint column);

		[Export ("validateVisibleColumns")]
		void ValidateVisibleColumns ();

		[Export ("scrollColumnsRightBy:")]
		void ScrollColumnsRightBy (nint shiftAmount);

		[Export ("scrollColumnsLeftBy:")]
		void ScrollColumnsLeftBy (nint shiftAmount);

		[Export ("scrollColumnToVisible:")]
		void ScrollColumnToVisible (nint column);

		[Export ("addColumn")]
		void AddColumn ();

		[Export ("numberOfVisibleColumns")]
		nint VisibleColumns { get; }

		[Export ("firstVisibleColumn")]
		nint FirstVisibleColumn { get; }

		[Export ("lastVisibleColumn")]
		nint LastVisibleColumn { get; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use the item based NSBrowser instead.")]
		[Export ("columnOfMatrix:")]
		nint ColumnOfMatrix (NSMatrix matrix);

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use the item based NSBrowser instead.")]
		[Export ("matrixInColumn:")]
		NSMatrix MatrixInColumn (nint column);

		[Export ("loadedCellAtRow:column:")]
		NSCell LoadedCell (nint row, nint col);

		[Export ("selectAll:")]
		void SelectAll (NSObject sender);

		[Export ("tile")]
		void Tile ();

		[Export ("doClick:")]
		void DoClick (NSObject sender);

		[Export ("doDoubleClick:")]
		void DoDoubleClick (NSObject sender);

		[Export ("sendAction")]
		bool SendAction ();

		[Export ("titleFrameOfColumn:")]
		CGRect TitleFrameOfColumn (nint column);

		[Export ("drawTitleOfColumn:inRect:")]
		void DrawTitle (nint column, CGRect aRect);

		[Export ("titleHeight")]
		nfloat TitleHeight { get; }

		[Export ("frameOfColumn:")]
		CGRect ColumnFrame (nint column);

		[Export ("frameOfInsideOfColumn:")]
		CGRect ColumnInsideFrame (nint column);

		[Export ("frameOfRow:inColumn:")]
		CGRect RowFrame (nint row, nint column);

		[Export ("getRow:column:forPoint:")]
		bool GetRowColumnForPoint (out nint row, out nint column, CGPoint point);

		[Export ("columnWidthForColumnContentWidth:")]
		nfloat ColumnWidthForColumnContentWidth (nfloat columnContentWidth);

		[Export ("columnContentWidthForColumnWidth:")]
		nfloat ColumnContentWidthForColumnWidth (nfloat columnWidth);

#if !XAMCORE_2_0
		[Export ("setColumnResizingType:")]
		[Obsolete ("Use the 'ColumnResizingType' property instead.")]
		[Sealed]
		void SetColumnResizingType (NSBrowserColumnResizingType columnResizingType);
#endif

		[Export ("columnResizingType")]
		NSBrowserColumnResizingType ColumnResizingType { get; set; }

		[Export ("prefersAllColumnUserResizing")]
		bool PrefersAllColumnUserResizing { get; set; }

		[Export ("setWidth:ofColumn:")]
		void SetColumnWidth (nfloat columnWidth, nint columnIndex);

		[Export ("widthOfColumn:")]
		nfloat GetColumnWidth (nint column);

		[Export ("rowHeight")]
		nfloat RowHeight { get; set; }

		[Export ("noteHeightOfRowsWithIndexesChanged:inColumn:")]
		void NoteHeightOfRows (NSIndexSet indexSet, nint columnIndex);

		[Export ("defaultColumnWidth")]
		nfloat DefaultColumnWidth { get; set; }

		[Export ("columnsAutosaveName")]
		string ColumnsAutosaveName  { get; set; }

		[Static]
		[Export ("removeSavedColumnsWithAutosaveName:")]
		void RemoveSavedColumnsWithAutosaveName (string name);

		[Export ("canDragRowsWithIndexes:inColumn:withEvent:")]
		bool CanDragRowsWithIndexes (NSIndexSet rowIndexes, nint column, NSEvent theEvent);

		// FIXME: binding, NSPointPointer
		//[Export ("draggingImageForRowsWithIndexes:inColumn:withEvent:offset:")]
		//NSImage DraggingImageForRowsWithIndexes (NSIndexSet rowIndexes, int column, NSEvent theEvent, NSPointPointer dragImageOffset);

		[Export ("setDraggingSourceOperationMask:forLocal:")]
		void SetDraggingSourceOperationMask (NSDragOperation mask, bool isLocal);

		[Export ("allowsTypeSelect")]
		bool AllowsTypeSelect  { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Retain)]
		NSColor BackgroundColor  { get; set; }

		[Export ("editItemAtIndexPath:withEvent:select:")]
		void EditItemAtIndexPath (NSIndexPath indexPath, NSEvent theEvent, bool select);

		//Detected properties
		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use the item based NSBrowser instead.")]
		[Export ("matrixClass")]
		Class MatrixClass { get; [Bind ("setMatrixClass:")] set; }

		[Static]
		[Export ("cellClass")]
		Class CellClass { get; }

		[Export ("setCellClass:")]
		void SetCellClass (Class factoryId);

		[Export ("cellPrototype", ArgumentSemantic.Retain)]
		NSObject CellPrototype { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSBrowserDelegate Delegate { get; set; }

		[Export ("reusesColumns")]
		bool ReusesColumns { get; set; }

		[Export ("hasHorizontalScroller")]
		bool HasHorizontalScroller { get; set; }

		[Export ("separatesColumns")]
		bool SeparatesColumns { get; set; }

		[Export ("titled")]
		bool Titled { [Bind ("isTitled")]get; set; }

		[Export ("minColumnWidth")]
		nfloat MinColumnWidth { get; set; }

		[Export ("maxVisibleColumns")]
		nint MaxVisibleColumns { get; set; }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("allowsBranchSelection")]
		bool AllowsBranchSelection { get; set; }

		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get; set; }

		[Export ("takesTitleFromPreviousColumn")]
		bool TakesTitleFromPreviousColumn { get; set; }

		[Export ("sendsActionOnArrowKeys")]
		bool SendsActionOnArrowKeys { get; set; }

		[Export ("pathSeparator")]
		string PathSeparator { get; set; }

		[Export ("path"), Protected]
		string GetPath ();

		[Export ("setPath:"), Protected]
		bool SetPath (string path);

		[Export ("lastColumn")]
		nint LastColumn { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSBrowserDelegate {
		[Export ("browser:numberOfRowsInColumn:"), EventArgs ("NSBrowserColumn")]
		nint RowsInColumn (NSBrowser sender, nint column);

		[Export ("browser:createRowsForColumn:inMatrix:")]
		void CreateRowsForColumn (NSBrowser sender, nint column, NSMatrix matrix);

		[Export ("browser:numberOfChildrenOfItem:")]
		nint CountChildren (NSBrowser browser, NSObject item);

		[Export ("browser:child:ofItem:")]
		NSObject GetChild (NSBrowser browser, nint index, NSObject item);

		[Export ("browser:isLeafItem:")]
		bool IsLeafItem (NSBrowser browser, NSObject item);

		[Export ("browser:objectValueForItem:")]
		NSObject ObjectValueForItem (NSBrowser browser, NSObject item);

		[Export ("browser:heightOfRow:inColumn:")]
		nfloat RowHeight (NSBrowser browser, nint row, nint columnIndex);

		[Export ("rootItemForBrowser:")]
		NSObject RootItemForBrowser (NSBrowser browser);

		[Export ("browser:setObjectValue:forItem:")]
		void SetObjectValue (NSBrowser browser, NSObject obj, NSObject item);

		[Export ("browser:shouldEditItem:")]
		bool ShouldEditItem (NSBrowser browser, NSObject item);

		[Export ("browser:willDisplayCell:atRow:column:")]
		void WillDisplayCell (NSBrowser sender, NSObject cell, nint row, nint column);

		[Export ("browser:titleOfColumn:")]
		string ColumnTitle (NSBrowser sender, nint column);

		[Export ("browser:selectCellWithString:inColumn:")]
		bool SelectCellWithString (NSBrowser sender, string title, nint column);

		[Export ("browser:selectRow:inColumn:")]
		bool SelectRowInColumn (NSBrowser sender, nint row, nint column);

		[Export ("browser:isColumnValid:")]
		bool IsColumnValid (NSBrowser sender, nint column);

		[Export ("browserWillScroll:")]
		void WillScroll (NSBrowser sender);

		[Export ("browserDidScroll:")]
		void DidScroll (NSBrowser sender);

		[Export ("browser:shouldSizeColumn:forUserResize:toWidth:")]
		nfloat ShouldSizeColumn (NSBrowser browser, nint columnIndex, bool userResize, nfloat suggestedWidth);

		[Export ("browser:sizeToFitWidthOfColumn:")]
		nfloat SizeToFitWidth (NSBrowser browser, nint columnIndex);

		[Export ("browserColumnConfigurationDidChange:")]
		void ColumnConfigurationDidChange (NSNotification notification);

		[Export ("browser:shouldShowCellExpansionForRow:column:")]
		bool ShouldShowCellExpansion (NSBrowser browser, nint row, nint column);

		[Export ("browser:writeRowsWithIndexes:inColumn:toPasteboard:")]
		bool WriteRowsWithIndexesToPasteboard (NSBrowser browser, NSIndexSet rowIndexes, nint column, NSPasteboard pasteboard);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSFilePromiseReceiver' objects instead.")]
		[Export ("browser:namesOfPromisedFilesDroppedAtDestination:forDraggedRowsWithIndexes:inColumn:")]
		string [] PromisedFilesDroppedAtDestination (NSBrowser browser, NSUrl dropDestination, NSIndexSet rowIndexes, nint column);

		[Export ("browser:canDragRowsWithIndexes:inColumn:withEvent:")]
		bool CanDragRowsWithIndexes (NSBrowser browser, NSIndexSet rowIndexes, nint column, NSEvent theEvent);

		// FIXME: NSPOintPointer is a pointer to a CGPoint, so we need to support refs
		//[Export ("browser:draggingImageForRowsWithIndexes:inColumn:withEvent:offset:")]
		//NSImage DraggingImageForRowsWithIndexes (NSBrowser browser, NSIndexSet rowIndexes, int column, NSEvent theEvent, NSPointPointer dragImageOffset);

		[Export ("browser:validateDrop:proposedRow:column:dropOperation:")]
#if !XAMCORE_2_0
		NSDragOperation ValidateDrop (NSBrowser browser, [Protocolize (4)] NSDraggingInfo info, ref nint row, ref nint column, NSBrowserDropOperation dropOperation);
#else
		NSDragOperation ValidateDrop (NSBrowser browser, [Protocolize (4)] NSDraggingInfo info, ref nint row, ref nint column, ref NSBrowserDropOperation dropOperation);
#endif

		[Export ("browser:acceptDrop:atRow:column:dropOperation:")]
		bool AcceptDrop (NSBrowser browser, [Protocolize (4)] NSDraggingInfo info, nint row, nint column, NSBrowserDropOperation dropOperation);

		[return: NullAllowed]
		[Export ("browser:typeSelectStringForRow:inColumn:")]
		string TypeSelectString (NSBrowser browser, nint row, nint column);

		[Export ("browser:shouldTypeSelectForEvent:withCurrentSearchString:")]
		bool ShouldTypeSelectForEvent (NSBrowser browser, NSEvent theEvent, string currentSearchString);

		[Export ("browser:nextTypeSelectMatchFromRow:toRow:inColumn:forString:")]
		nint NextTypeSelectMatch (NSBrowser browser, nint startRow, nint endRow, nint column, string searchString);

		[Export ("browser:previewViewControllerForLeafItem:")]
		NSViewController PreviewViewControllerForLeafItem (NSBrowser browser, NSObject item);

		[Export ("browser:headerViewControllerForItem:")]
		NSViewController HeaderViewControllerForItem (NSBrowser browser, NSObject item);

		[Export ("browser:didChangeLastColumn:toColumn:")]
		void DidChangeLastColumn (NSBrowser browser, nint oldLastColumn, nint toColumn);

		[Export ("browser:selectionIndexesForProposedSelection:inColumn:")]
		NSIndexSet SelectionIndexesForProposedSelection (NSBrowser browser, NSIndexSet proposedSelectionIndexes, nint inColumn);

	}

	[BaseType (typeof (NSCell))]
	interface NSBrowserCell {
		[Mac (10,12)]
		[Export ("initTextCell:")]
		[DesignatedInitializer]
		IntPtr Constructor (string str);

		[Mac (10,12)]
		[Export ("initImageCell:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSImage image);

		[Static]
		[Export ("branchImage")]
		NSImage BranchImage { get; }

		[Static]
		[Export ("highlightedBranchImage")]
		NSImage HighlightedBranchImage { get; }

		[Export ("highlightColorInView:")]
		NSColor HighlightColorInView (NSView controlView);

		[Export ("reset")]
		void Reset ();

		[Export ("set")]
		void Set ();

		//Detected properties
		[Export ("leaf")]
		bool Leaf { [Bind ("isLeaf")]get; set; }

		[Export ("loaded")]
		bool Loaded { [Bind ("isLoaded")]get; set; }

		[Export ("image", ArgumentSemantic.Retain)]
		NSImage Image { get; set; }

		[Export ("alternateImage", ArgumentSemantic.Retain)]
		NSImage AlternateImage { get; set; }

	}

	[BaseType (typeof (NSActionCell))]
	interface NSButtonCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[DesignatedInitializer]
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage image);

		[Export ("title")]
		string Title { get; set; }
	
		[Export ("alternateTitle")]
		string AlternateTitle { get; set; }
	
		[Export ("alternateImage", ArgumentSemantic.Retain)]
		NSImage AlternateImage { get; set; }
	
		[Export ("imagePosition")]
		NSCellImagePosition ImagePosition { get; set; }
	
		[Export ("imageScaling")]
		NSImageScale ImageScale { get; set; }
	
		[Export ("highlightsBy")]
		nint HighlightsBy { get; set; }
	
		[Export ("showsStateBy")]
		nint ShowsStateBy { get; set; }
	
#if !XAMCORE_2_0
		[Export ("setShowsStateBy:")]
		[Obsolete ("Use the 'ShowsStateBy' property instead.")]
		[Sealed]
		void SetShowsStateBy (nint aType);
#endif
	
		[Export ("setButtonType:")]
		void SetButtonType (NSButtonType aType);
	
		[Export ("isOpaque")]
		bool IsOpaque { get; }
	
		[Export ("setFont:")]
		void SetFont (NSFont  fontObj);
	
		[Export ("transparent")]
		bool Transparent { [Bind ("isTransparent")] get; set; }
	
		[Export ("setPeriodicDelay:interval:")]
		void SetPeriodicDelay (float /* float, not CGFloat */ delay, float /* float, not CGFloat */ interval);
	
		[Export ("getPeriodicDelay:interval:")]
		void GetPeriodicDelay (out float /* float, not CGFloat */ delay, out float /* float, not CGFloat */ interval);
	
		[Export ("keyEquivalent")]
		string KeyEquivalent { get; set; }
	
		[Export ("keyEquivalentModifierMask")]
		NSEventModifierMask KeyEquivalentModifierMask { get; set; }
	
		[Export ("keyEquivalentFont", ArgumentSemantic.Retain)]
		NSFont KeyEquivalentFont { get; set; }
	
		[Export ("setKeyEquivalentFont:size:")]
		void SetKeyEquivalentFont (string  fontName, nfloat fontSize);
	
		[Export ("performClick:")]
		void PerformClick (NSObject sender);
	
		[Export ("drawImage:withFrame:inView:")]
		void DrawImage (NSImage image, CGRect frame, NSView controlView);
	
		[Export ("drawTitle:withFrame:inView:")]
		CGRect DrawTitle (NSAttributedString title, CGRect frame, NSView controlView);
	
		[Export ("drawBezelWithFrame:inView:")]
		void DrawBezelWithFrame (CGRect frame, NSView controlView);

		[Availability (Deprecated = Platform.Mac_10_8, Message = "This method no longer does anything and should not be called.")]
		[Export ("alternateMnemonicLocation")]
		nint AlternateMnemonicLocation { get; set; }
	
		[Export ("alternateMnemonic")]
		[Availability (Deprecated = Platform.Mac_10_8, Message = "This method still will set Title with the ampersand stripped from the value, but does nothing else. Set the Title directly.")]
		string AlternateMnemonic { get; [Bind ("setAlternateTitleWithMnemonic:")] set; }
	
		[Export ("setGradientType:")]
		[Availability (Deprecated = Platform.Mac_10_12, Message = "The GradientType property is unused, and setting it has no effect.")]
		void SetGradientType (NSGradientType type);
	
		[Export ("imageDimsWhenDisabled")]
		bool ImageDimsWhenDisabled { get; set; }
	
		[Export ("showsBorderOnlyWhileMouseInside")]
		bool ShowsBorderOnlyWhileMouseInside { get; set; }
	
		[Export ("mouseEntered:")]
		void MouseEntered (NSEvent theEvent);
	
		[Export ("mouseExited:")]
		void MouseExited (NSEvent theEvent);
	
		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Export ("attributedTitle")]
		NSAttributedString AttributedTitle { get; set; }
	
		[Export ("attributedAlternateTitle")]
		NSAttributedString AttributedAlternateTitle { get; set; }
	
		[Export ("bezelStyle")]
		NSBezelStyle BezelStyle { get; set; }

		[Export ("sound")]
		NSSound Sound { get; set; }
	
	}
	
	[BaseType (typeof (NSControl))]
	interface NSButton : NSAccessibilityButton, NSUserInterfaceCompression {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("buttonWithTitle:image:target:action:")]
		NSButton _CreateButton (string title, NSImage image, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("buttonWithTitle:target:action:")]
		NSButton _CreateButton (string title, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("buttonWithImage:target:action:")]
		NSButton _CreateButton (NSImage image, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("checkboxWithTitle:target:action:")]
		NSButton _CreateCheckbox (string title, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("radioButtonWithTitle:target:action:")]
		NSButton _CreateRadioButton (string title, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("title")]
		string Title { get; set; } 
	
		[Export ("alternateTitle")]
		string AlternateTitle { get; set; }
	
		[Export ("image", ArgumentSemantic.Retain), NullAllowed]
		NSImage Image { get; set; }
	
		[Export ("alternateImage", ArgumentSemantic.Retain), NullAllowed]
		NSImage AlternateImage  { get; set; }
	
		[Export ("imagePosition")]
		NSCellImagePosition ImagePosition  { get; set; }
	
		[Export ("setButtonType:")]
		void SetButtonType (NSButtonType aType);
	
		[Export ("state")]
		NSCellStateValue State { get; set; }
	
		[Export ("bordered")]
		bool Bordered  { [Bind ("isBordered")] get; set; }
	
		[Export ("transparent")]
		bool Transparent  { [Bind ("isTransparent")] get; set; }
	
		[Export ("setPeriodicDelay:interval:")]
		void SetPeriodicDelay (float /* float, not CGFloat */ delay, /* float, not CGFloat */ float interval);
	
		[Export ("getPeriodicDelay:interval:")]
		void GetPeriodicDelay (ref float /* float, not CGFloat */ delay, ref float /* float, not CGFloat */ interval);
	
		[Export ("keyEquivalent")]
		string KeyEquivalent  { get; set; }
	
		[Export ("keyEquivalentModifierMask")]
		NSEventModifierMask KeyEquivalentModifierMask  { get; set; }
	
		[Export ("highlight:")]
		void Highlight (bool flag);
	
		[Export ("performKeyEquivalent:")]
		bool PerformKeyEquivalent (NSEvent  key);

		[Availability (Deprecated = Platform.Mac_10_8, Message = "On 10.8, this method still will set the Title with the ampersand stripped from stringWithAmpersand, but does nothing else. Set the Title directly.")]
		[Export ("setTitleWithMnemonic:")]
		void SetTitleWithMnemonic (string mnemonic);

		[Export ("attributedTitle")]
		NSAttributedString AttributedTitle { get; set; }

		[Export ("attributedAlternateTitle")]
		NSAttributedString AttributedAlternateTitle  { get; set; }

		[Export ("bezelStyle")]
		NSBezelStyle BezelStyle { get; set; }

		[Export ("allowsMixedState")]
		bool AllowsMixedState { get; set;}
		
		[Export ("setNextState")]
		void SetNextState ();

		[Export ("showsBorderOnlyWhileMouseInside")]
#if XAMCORE_2_0
		bool ShowsBorderOnlyWhileMouseInside { get; set; }
#else
		bool ShowsBorderOnlyWhileMouseInside ();

		[Export ("setShowsBorderOnlyWhileMouseInside:")]
		void SetShowsBorderOnlyWhileMouseInside (bool showsBorder);
#endif

		[Export ("sound")]
		NSSound Sound { get; set; }

		[Mac (10,10,3)]
		[Export ("springLoaded")]
		bool IsSpringLoaded { [Bind ("isSpringLoaded")] get; set; }

		[Mac (10,10,3)]
		[Export ("maxAcceleratorLevel")]
		nint MaxAcceleratorLevel { get; set; }

		[Mac (10,12)]
		[Export ("imageHugsTitle")]
		bool ImageHugsTitle { get; set; }

		[Export ("imageScaling")]
		NSImageScale ImageScaling { get; set; }

		[Mac (10, 12, 2)]
		[NullAllowed, Export ("bezelColor", ArgumentSemantic.Copy)]
		NSColor BezelColor { get; set; }
	}
	
	[BaseType (typeof (NSImageRep))]
	[DisableDefaultCtor] // An uncaught exception was raised: -[NSCachedImageRep init]: unrecognized selector sent to instance 0x14890e0
	[Availability (Deprecated = Platform.Mac_10_6)]
	interface NSCachedImageRep {
		[Availability (Deprecated = Platform.Mac_10_6)]
		[Export ("initWithWindow:rect:")]
		IntPtr Constructor (NSWindow win, CGRect rect);

		[Availability (Deprecated = Platform.Mac_10_6)]
		[Export ("initWithSize:depth:separate:alpha:")]
		IntPtr Constructor (CGSize size, NSWindowDepth depth, bool separate, bool alpha);

		[Availability (Deprecated = Platform.Mac_10_6)]
		[Export ("window")]
		NSWindow Window { get; }

		[Availability (Deprecated = Platform.Mac_10_6)]
		[Export ("rect")]
		CGRect Rectangle { get; }
	}
	
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSCell : NSUserInterfaceItemIdentification, NSCoding, NSCopying, NSAccessibilityElementProtocol, NSAccessibility, NSObjectAccessibilityExtensions {
		[Static, Export ("prefersTrackingUntilMouseUp")]
		bool PrefersTrackingUntilMouseUp { get; }
	
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[DesignatedInitializer]
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);
	
		[Export ("controlView")]
		NSView ControlView { get; set; }
	
		[Export ("type")]
		NSCellType CellType { get; set; }
	
		[Export ("state")]
		NSCellStateValue State { get; set; }
	
		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }
	
		[Export ("action"), NullAllowed]
		Selector Action { get; set; }
	
		[Export ("tag")]
		nint Tag { get; set; }
	
		[Export ("title")]
		string Title { get; set; }
	
		[Export ("isOpaque")]
		bool IsOpaque { get; } 
	
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }
	
		[Export ("sendActionOn:")]
		nint SendActionOn (NSEventType mask);
	
		[Export ("continuous")]
		bool IsContinuous { [Bind ("isContinuous")] get; set; }
	
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }
	
		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")] get; set; }
	
		[Export ("bordered")]
		bool Bordered { [Bind ("isBordered")] get; set; }
	
		[Export ("bezeled")]
		bool Bezeled { [Bind ("isBezeled")] get; set; }
	
		[Export ("scrollable")]
		bool Scrollable { [Bind ("isScrollable")] get; set; }
	
		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }
	
		[Export ("alignment")]
		NSTextAlignment Alignment { get; set; }
	
		[Export ("wraps")]
		bool Wraps { get; set; }
	
		[Export ("font", ArgumentSemantic.Retain)]
		NSFont Font { get; set; }
	
		[Export ("isEntryAcceptable:")]
		bool IsEntryAcceptable (string  aString);
	
		[Export ("keyEquivalent")]
		string KeyEquivalent { get; }
	
		[Export ("formatter", ArgumentSemantic.Retain)]
		NSFormatter Formatter { get; set; }
	
		[Export ("objectValue", ArgumentSemantic.Copy), NullAllowed]
		NSObject ObjectValue { get; set; }
	
		[Export ("hasValidObjectValue")]
		bool HasValidObjectValue { get; }
	
		[Export ("stringValue")]
		string StringValue { get; set; }
	
		[Export ("compare:")]
		NSComparisonResult Compare (NSObject otherCell);
	
		[Export ("intValue")]
		int IntValue { get; set; } /* int, not NSInteger */ 
	
		[Export ("floatValue")]
		float FloatValue { get; set; } /* float, not CGFloat */
	
		[Export ("doubleValue")]
		double DoubleValue { get; set; }
	
		[Export ("takeIntValueFrom:")]
		void TakeIntValueFrom (NSObject sender);
	
		[Export ("takeFloatValueFrom:")]
		void TakeFloatValueFrom (NSObject sender);
	
		[Export ("takeDoubleValueFrom:")]
		void TakeDoubleValueFrom (NSObject sender);
	
		[Export ("takeStringValueFrom:")]
		void TakeStringValueFrom (NSObject sender);
	
		[Export ("takeObjectValueFrom:")]
		void TakeObjectValueFrom (NSObject sender);
	
		[Export ("image", ArgumentSemantic.Retain)]
		NSImage Image  { get; set; }
	
		[Export ("controlTint")]
		NSControlTint ControlTint { get; set; }

		[Notification, Field ("NSControlTintDidChangeNotification")]
		NSString ControlTintChangedNotification { get; }

		[Export ("controlSize")]
		NSControlSize ControlSize { get; set; }
	
		[Export ("representedObject", ArgumentSemantic.Retain)]
		NSObject RepresentedObject { get; set; }
	
		[Export ("cellAttribute:")]
		nint CellAttribute (NSCellAttribute aParameter);
	
		[Export ("setCellAttribute:to:")]
		void SetCellAttribute (NSCellAttribute aParameter, nint value);
	
		[Export ("imageRectForBounds:")]
		CGRect ImageRectForBounds (CGRect theRect);
	
		[Export ("titleRectForBounds:")]
		CGRect TitleRectForBounds (CGRect theRect);
	
		[Export ("drawingRectForBounds:")]
		CGRect DrawingRectForBounds (CGRect theRect);
	
		[Export ("cellSize")]
		CGSize CellSize { get; }
	
		[Export ("cellSizeForBounds:")]
		CGSize CellSizeForBounds (CGRect bounds);
	
		[Export ("highlightColorWithFrame:inView:")]
		[return: NullAllowed]
		NSColor HighlightColor (CGRect cellFrame, NSView controlView);
	
		[Export ("calcDrawInfo:")]
		void CalcDrawInfo (CGRect aRect);
	
		[Export ("setUpFieldEditorAttributes:")]
		NSText SetUpFieldEditorAttributes (NSText textObj);
	
		[Export ("drawInteriorWithFrame:inView:")]
		void DrawInteriorWithFrame (CGRect cellFrame, NSView  inView);
	
		[Export ("drawWithFrame:inView:")]
		void DrawWithFrame (CGRect cellFrame, NSView inView);
	
		[Export ("highlight:withFrame:inView:")]
		void Highlight (bool highlight, CGRect withFrame, NSView  inView);
	
		[Export ("mouseDownFlags")]
		nint MouseDownFlags { get; }
	
		[Export ("getPeriodicDelay:interval:")]
		void GetPeriodicDelay (ref float /* float, not CGFloat */ delay, ref float /* float, not CGFloat */ interval);
	
		[Export ("startTrackingAt:inView:")]
		bool StartTracking (CGPoint startPoint, NSView inView);
	
		[Export ("continueTracking:at:inView:")]
		bool ContinueTracking (CGPoint lastPoint, CGPoint currentPoint, NSView inView);
	
		[Export ("stopTracking:at:inView:mouseIsUp:")]
		void StopTracking (CGPoint lastPoint, CGPoint stopPoint, NSView inView, bool mouseIsUp);
	
		[Export ("trackMouse:inRect:ofView:untilMouseUp:")]
		bool TrackMouse (NSEvent  theEvent, CGRect cellFrame, NSView  controlView, bool untilMouseUp);
	
		[Export ("editWithFrame:inView:editor:delegate:event:")]
		void EditWithFrame (CGRect aRect, [NullAllowed] NSView  inView, [NullAllowed] NSText editor, [NullAllowed] NSObject delegateObject, NSEvent theEvent);
	
		[Export ("selectWithFrame:inView:editor:delegate:start:length:")]
		void SelectWithFrame (CGRect aRect, [NullAllowed] NSView inView, [NullAllowed] NSText editor, [NullAllowed] NSObject delegateObject, nint selStart, nint selLength);
	
		[Export ("endEditing:")]
		void EndEditing ([NullAllowed] NSText textObj);
	
		[Export ("resetCursorRect:inView:")]
		void ResetCursorRect (CGRect cellFrame, NSView  inView);
	
		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSMenu Menu { get; set; }
	
		[Export ("menuForEvent:inRect:ofView:")]
		NSMenu MenuForEvent (NSEvent theEvent, CGRect cellFrame, NSView  view);
	
		[Static]
		[Export ("defaultMenu")]
		[NullAllowed]
		NSMenu DefaultMenu { get; }
	
		[Export ("setSendsActionOnEndEditing:")]
		void SetSendsActionOnEndEditing (bool flag);
	
		[Export ("sendsActionOnEndEditing")]
		bool SendsActionOnEndEditing ();
	
		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; set; }
       
		[Export ("lineBreakMode")]
		NSLineBreakMode LineBreakMode { get; set; }
	
		[Export ("allowsUndo")]
		bool AllowsUndo { get; set; }
	
		[Export ("integerValue")]
		nint IntegerValue { get; set; }
	
		[Export ("takeIntegerValueFrom:")]
		void TakeIntegerValueFrom (NSObject sender);
	
		[Export ("truncatesLastVisibleLine")]
		bool TruncatesLastVisibleLine { get; set; }
	
		[Export ("userInterfaceLayoutDirection")]
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; set; }
	
		[Export ("fieldEditorForView:")]
		NSTextView FieldEditorForView (NSView  aControlView);
	
		[Export ("usesSingleLineMode")]
		bool UsesSingleLineMode { get; set; }

		//  NSCell(NSCellAttributedStringMethods)
		[Export ("refusesFirstResponder")]
		bool RefusesFirstResponder ();
	
		[Export ("acceptsFirstResponder")]
		bool AcceptsFirstResponder ();
	
		[Export ("showsFirstResponder")]
		bool ShowsFirstResponder { get; set; }

		[Availability (Deprecated = Platform.Mac_10_8, Message = "Mnemonic methods have typically not been used.")]
		[Export ("mnemonicLocation")]
		nint MnemonicLocation { get; set; }
	
		[Availability (Deprecated = Platform.Mac_10_8, Message = "Mnemonic methods have typically not been used.")]
		[Export ("mnemonic")]
		string Mnemonic { get; }
	
		[Availability (Deprecated = Platform.Mac_10_8, Message = "Mnemonic methods have typically not been used.")]
		[Export ("setTitleWithMnemonic:")]
		void SetTitleWithMnemonic (string  stringWithAmpersand);
	
		[Export ("performClick:")]
		void PerformClick (NSObject sender);
	
		[Export ("focusRingType")]
		NSFocusRingType FocusRingType { get; set; }
	
		[Static, Export ("defaultFocusRingType")]
		NSFocusRingType DefaultFocusRingType { get; }
	
		[Export ("wantsNotificationForMarkedText")]
		bool WantsNotificationForMarkedText { get; [NotImplemented] set; }
	
		// NSCell(NSCellAttributedStringMethods)
		[Export ("attributedStringValue")]
		NSAttributedString AttributedStringValue { get; set; }
	
		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set; }
	
		[Export ("importsGraphics")]
		bool ImportsGraphics { get; set; }
       
		// NSCell(NSCellMixedState) {
		[Export ("allowsMixedState")]
		bool AllowsMixedState { get; set; }
	
		[Export ("nextState")]
		nint NextState { get; }
	
		[Export ("setNextState")]
		void SetNextState ();
	
		[Export ("hitTestForEvent:inRect:ofView:")]
		NSCellHit HitTest (NSEvent forEvent, CGRect inRect, NSView  ofView);
	
		// NSCell(NSCellExpansion) 
		[Export ("expansionFrameWithFrame:inView:")]
		CGRect ExpansionFrame (CGRect withFrame, NSView inView);
	
		[Export ("drawWithExpansionFrame:inView:")]
		void DrawWithExpansionFrame (CGRect cellFrame, NSView inView);
	
		[Export ("backgroundStyle")]
		NSBackgroundStyle BackgroundStyle { get; set; }
	
		[Export ("interiorBackgroundStyle")]
		NSBackgroundStyle InteriorBackgroundStyle { get; }
	
		[Mac (10, 7), Export ("draggingImageComponentsWithFrame:inView:")]
		NSDraggingImageComponent [] GenerateDraggingImageComponents (CGRect frame, NSView view);

		[Mac (10, 7), Export ("drawFocusRingMaskWithFrame:inView:")]
		void DrawFocusRing (CGRect cellFrameMask, NSView inControlView);

		[Mac (10, 7), Export ("focusRingMaskBoundsForFrame:inView:")]
		CGRect GetFocusRingMaskBounds (CGRect cellFrame, NSView controlView);
	}

	[BaseType (typeof (NSImageRep))]
	[DisableDefaultCtor] // An uncaught exception was raised: -[NSCIImageRep init]: unrecognized selector sent to instance 0x1b682a0
	interface NSCIImageRep {
		[Static]
		[Export ("imageRepWithCIImage:")]
		NSCIImageRep FromCIImage (CIImage image);

		[Export ("initWithCIImage:")]
		IntPtr Constructor (CIImage image);

		[Export ("CIImage")]
		CIImage CIImage { get; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSGestureRecognizer))]
	interface NSClickGestureRecognizer : NSCoding {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("buttonMask")]
		nuint ButtonMask { get; set; }

		[Export ("numberOfClicksRequired")]
		nint NumberOfClicksRequired { get; set; }

		[Mac (10, 12, 2)]
		[Export ("numberOfTouchesRequired")]
		nint NumberOfTouchesRequired { get; set; }
	}
	
	[BaseType (typeof (NSView))]
	interface NSClipView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	
		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }
	
		[Export ("documentView")]
		NSView DocumentView { get; set; }
	
		[Export ("documentRect")]
		CGRect DocumentRect { get; }
	
		[Export ("documentCursor", ArgumentSemantic.Retain)]
		NSCursor DocumentCursor { get; set; }
	
		[Export ("documentVisibleRect")]
		CGRect DocumentVisibleRect ();
	
		[Export ("viewFrameChanged:")]
		void ViewFrameChanged (NSNotification  notification);
	
		[Export ("viewBoundsChanged:")]
		void ViewBoundsChanged (NSNotification  notification);
	
		[Export ("copiesOnScroll")]
		bool CopiesOnScroll { get; set; }
	
		[Export ("autoscroll:")]
		bool Autoscroll (NSEvent  theEvent);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use ConstrainBoundsRect instead.")]
		[Export ("constrainScrollPoint:")]
		CGPoint ConstrainScrollPoint (CGPoint newOrigin);

		[Mac (10,9)]
		[Export ("constrainBoundsRect:")]
		CGRect ConstrainBoundsRect (CGRect proposedBounds);
	
		[Export ("scrollToPoint:")]
		void ScrollToPoint (CGPoint newOrigin);

		[Export ("scrollClipView:toPoint:")]
		void ScrollClipView (NSClipView  aClipView, CGPoint aPoint);

		[Mac (10,10)]
		[Export ("contentInsets")]
		NSEdgeInsets ContentInsets { get; set; }

		[Mac (10,10)]
		[Export ("automaticallyAdjustsContentInsets")]
		bool AutomaticallyAdjustsContentInsets { get; set; }
	}

	[Category, BaseType (typeof (NSCoder))]
	partial interface NSCoderAppKitAddons {
		[Availability (Deprecated = Platform.Mac_10_9)]
		[Export ("decodeNXColor")]
		NSColor DecodeNXColor ();
	}

	[BaseType (typeof (NSViewController))]
	interface NSCollectionViewItem : NSCopying {
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("collectionView")]
		[NullAllowed]
		NSCollectionView CollectionView { get; }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")]get; set; }

		[Export ("imageView", ArgumentSemantic.Assign)]
		[Mac (10, 7)]
		NSImageView ImageView { get; set;  }

		[Export ("textField", ArgumentSemantic.Assign)]
		NSTextField TextField { get; set;  }

		[Export ("draggingImageComponents")]
		NSDraggingImageComponent [] DraggingImageComponents { get;  }

		[Mac (10,11)]
		[Export ("highlightState", ArgumentSemantic.Assign)]
		NSCollectionViewItemHighlightState HighlightState { get; set; }
	}

	[BaseType (typeof (NSView))]
	interface NSCollectionView : NSDraggingSource, NSDraggingDestination {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("isFirstResponder")]
		bool IsFirstResponder { get; } 

		[Export ("newItemForRepresentedObject:")]
		[return: Release ()]
		NSCollectionViewItem NewItemForRepresentedObject (NSObject obj);

		[Export ("itemAtIndex:")]
		NSCollectionViewItem ItemAtIndex (nint index);

		[Export ("frameForItemAtIndex:")]
		CGRect FrameForItemAtIndex (nint index);

		[Export ("setDraggingSourceOperationMask:forLocal:")]
		void SetDraggingSource (NSDragOperation dragOperationMask, bool localDestination);

		//[Export ("draggingImageForItemsAtIndexes:withEvent:offset:")]
		//NSImage DraggingImage (NSIndexSet itemIndexes, NSEvent evt, NSPointPointer dragImageOffset);

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSCollectionViewDelegate Delegate { get; set; }

		[Export ("content", ArgumentSemantic.Copy)]
		NSObject [] Content { get; set; }

		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")]get; set; }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("selectionIndexes", ArgumentSemantic.Copy)]
		NSIndexSet SelectionIndexes { get; set; }

		[Export ("itemPrototype", ArgumentSemantic.Retain)]
		NSCollectionViewItem ItemPrototype { get; set; }

		[Export ("maxNumberOfRows")]
		nint MaxNumberOfRows { get; set; }

		[Export ("maxNumberOfColumns")]
		nint MaxNumberOfColumns { get; set; }

		[Export ("minItemSize")]
		CGSize MinItemSize { get; set; }

		[Export ("maxItemSize")]
		CGSize MaxItemSize { get; set; }

		[Export ("backgroundColors", ArgumentSemantic.Copy), NullAllowed]
		NSColor [] BackgroundColors { get; set; }

		[Mac (10, 7)]
		[Export ("frameForItemAtIndex:withNumberOfItems:")]
		CGRect FrameForItemAtIndex (nint index, nint numberOfItems);

		[Mac (10,11)]
		[Protocolize]
		[NullAllowed, Export ("dataSource", ArgumentSemantic.Weak)]
		NSCollectionViewDataSource DataSource { get; set; }

		[Mac (10,11)]
		[Export ("reloadData")]
		void ReloadData ();

		[Mac (10,11)]
		[NullAllowed, Export ("backgroundView", ArgumentSemantic.Strong)]
		NSView BackgroundView { get; set; }

		[Mac (10,11)]
		[NullAllowed, Export ("collectionViewLayout", ArgumentSemantic.Strong)]
		NSCollectionViewLayout CollectionViewLayout { get; set; }

		[Mac (10,11)]
		[Export ("layoutAttributesForItemAtIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributes (NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("layoutAttributesForSupplementaryElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributes (string kind, NSIndexPath indexPath);

		// -(NSInteger)numberOfSections __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)]
		[Export ("numberOfSections")]
		// [Verify (MethodToProperty)]
		nint NumberOfSections { get; }

		[Mac (10,11)]
		[Export ("numberOfItemsInSection:")]
		nint GetNumberOfItems (nint section);

		[Mac (10,11)]
		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get; set; }

		[Mac (10,11)]
		[Export ("selectionIndexPaths", ArgumentSemantic.Copy)]
		NSSet SelectionIndexPaths { get; set; }

		[Mac (10,11)]
		[Export ("selectItemsAtIndexPaths:scrollPosition:")]
		void SelectItems (NSSet indexPaths, NSCollectionViewScrollPosition scrollPosition);

		[Mac (10,11)]
		[Export ("deselectItemsAtIndexPaths:")]
		void DeselectItems (NSSet indexPaths);

		[Mac (10,11)]
		[Export ("registerClass:forItemWithIdentifier:"), Internal]
		void _RegisterClassForItem ([NullAllowed] IntPtr itemClass, string identifier);

		[Mac (10,11)]
		[Export ("registerNib:forItemWithIdentifier:")]
		void RegisterNib ([NullAllowed] NSNib nib, string identifier);

		[Mac (10,11)]
		[Export ("registerClass:forSupplementaryViewOfKind:withIdentifier:"), Internal]
		void _RegisterClassForSupplementaryView ([NullAllowed] IntPtr viewClass, NSString kind, string identifier);

		[Mac (10,11)]
		[Export ("registerNib:forSupplementaryViewOfKind:withIdentifier:")]
		void RegisterNib ([NullAllowed] NSNib nib, NSString kind, string identifier);

		[Mac (10,11)]
		[Export ("makeItemWithIdentifier:forIndexPath:")]
		NSCollectionViewItem MakeItem (string identifier, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("makeSupplementaryViewOfKind:withIdentifier:forIndexPath:")]
		NSView MakeSupplementaryView (NSString elementKind, string identifier, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("itemAtIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewItem GetItem (NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("visibleItems")]
		// [Verify (MethodToProperty)]
		NSCollectionViewItem[] VisibleItems { get; }

		[Mac (10,11)]
		[Export ("indexPathsForVisibleItems")]
		// [Verify (MethodToProperty)]
		NSSet<NSIndexPath> IndexPathsForVisibleItems { get; }

		[Mac (10,11)]
		[Export ("indexPathForItem:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPath (NSCollectionViewItem item);

		[Mac (10,11)]
		[Export ("indexPathForItemAtPoint:")]
		[return: NullAllowed]
		NSIndexPath GetIndexPath (CGPoint point);

		// -(NSView<NSCollectionViewElement> * __nullable)supplementaryViewForElementKind:(NSString * __nonnull)elementKind atIndexPath:(NSIndexPath * __nonnull)indexPath __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)]
		[Export ("supplementaryViewForElementKind:atIndexPath:")]
		[return: NullAllowed]
		INSCollectionViewElement GetSupplementaryView (NSString elementKind, NSIndexPath indexPath);

		// -(NSArray<NSView<NSCollectionViewElement> * __nonnull> * __nonnull)visibleSupplementaryViewsOfKind:(NSString * __nonnull)elementKind __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)]
		[Export ("visibleSupplementaryViewsOfKind:")]
		INSCollectionViewElement[] GetVisibleSupplementaryViews (NSString elementKind);

		[Mac (10,11)]
		[Export ("indexPathsForVisibleSupplementaryElementsOfKind:")]
		NSSet GetIndexPaths (string elementKind);

		[Mac (10,11)]
		[Export ("insertSections:")]
		void InsertSections (NSIndexSet sections);

		[Mac (10,11)]
		[Export ("deleteSections:")]
		void DeleteSections (NSIndexSet sections);

		[Mac (10,11)]
		[Export ("reloadSections:")]
		void ReloadSections (NSIndexSet sections);

		[Mac (10,11)]
		[Export ("moveSection:toSection:")]
		void MoveSection (nint section, nint newSection);

		[Mac (10,11)]
		[Export ("insertItemsAtIndexPaths:")]
		void InsertItems (NSSet<NSIndexPath> indexPaths);

		[Mac (10,11)]
		[Export ("deleteItemsAtIndexPaths:")]
		void DeleteItems (NSSet<NSIndexPath> indexPaths);

		[Mac (10,11)]
		[Export ("reloadItemsAtIndexPaths:")]
		void ReloadItems (NSSet<NSIndexPath> indexPaths);

		[Mac (10,11)]
		[Export ("moveItemAtIndexPath:toIndexPath:")]
		void MoveItem (NSIndexPath indexPath, NSIndexPath newIndexPath);

		[Mac (10,11)]
		[Export ("performBatchUpdates:completionHandler:")]
		void PerformBatchUpdates (Action updates, Action<bool> completionHandler);

		[Mac (10,11)]
		[Export ("scrollToItemsAtIndexPaths:scrollPosition:")]
		void ScrollToItems (NSSet<NSIndexPath> indexPaths, NSCollectionViewScrollPosition scrollPosition);

		[Mac (10,11)]
		[Export ("draggingImageForItemsAtIndexPaths:withEvent:offset:")]
		NSImage GetDraggingImage (NSSet<NSIndexPath> indexPaths, NSEvent theEvent, ref CGPoint dragImageOffset);

		[Mac (10,11)] // Not marked as 10.11 in the header files, but didn't exist previously
		[Export ("selectAll:")]
		void SelectAll ([NullAllowed] NSObject sender);

		[Mac (10,11)] // Not marked as 10.11 in the header files, but didn't exist previously
		[Export ("deselectAll:")]
		void DeselectAll ([NullAllowed] NSObject sender);

		[Mac (10, 12)]
		[Export ("backgroundViewScrollsWithContent")]
		bool BackgroundViewScrollsWithContent { get; set; }

		[Mac (10,12)]
		[Export ("toggleSectionCollapse:")]
		void ToggleSectionCollapse (NSObject sender);

		[Mac (10, 13)]
		[NullAllowed, Export ("prefetchDataSource", ArgumentSemantic.Weak)]
		INSCollectionViewPrefetching PrefetchDataSource { get; set; }
	}

	// @protocol NSCollectionViewDataSource <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewDataSource
	{
		[Mac (10,11)]
		[Abstract]
		[Export ("collectionView:numberOfItemsInSection:")]
		nint GetNumberofItems (NSCollectionView collectionView, nint section);

		[Mac (10,11)]
		[Abstract]
		[Export ("collectionView:itemForRepresentedObjectAtIndexPath:")]
		NSCollectionViewItem GetItem (NSCollectionView collectionView, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("numberOfSectionsInCollectionView:")]
		nint GetNumberOfSections (NSCollectionView collectionView);

		[Export ("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
		NSView GetView (NSCollectionView collectionView, NSString kind, NSIndexPath indexPath);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSCollectionViewDelegate {
		[Export ("collectionView:canDragItemsAtIndexes:withEvent:")]
		bool CanDragItems (NSCollectionView collectionView, NSIndexSet indexes, NSEvent evt);

		[Export ("collectionView:writeItemsAtIndexes:toPasteboard:")]
		bool WriteItems (NSCollectionView collectionView, NSIndexSet indexes, NSPasteboard toPasteboard);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSFilePromiseReceiver' objects instead.")]
		[Export ("collectionView:namesOfPromisedFilesDroppedAtDestination:forDraggedItemsAtIndexes:")]
		string [] NamesOfPromisedFilesDroppedAtDestination (NSCollectionView collectionView, NSUrl dropUrl, NSIndexSet indexes);

		//[Export ("collectionView:draggingImageForItemsAtIndexes:withEvent:offset:")]
		//NSImage DraggingImageForItems (NSCollectionView collectionView, NSIndexSet indexes, NSEvent evg, NSPointPointer dragImageOffset);

		[Export ("collectionView:validateDrop:proposedIndex:dropOperation:")]
#if !XAMCORE_2_0
		NSDragOperation ValidateDrop (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo, ref nint dropIndex, NSCollectionViewDropOperation dropOperation);
#else
		NSDragOperation ValidateDrop (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo, ref nint dropIndex, ref NSCollectionViewDropOperation dropOperation);
#endif

		[Export ("collectionView:acceptDrop:index:dropOperation:")]
		bool AcceptDrop (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo, nint index, NSCollectionViewDropOperation dropOperation);

		[Mac (10,11)]
		[Export ("collectionView:canDragItemsAtIndexPaths:withEvent:")]
		bool CanDragItems (NSCollectionView collectionView, NSSet indexPaths, NSEvent theEvent);

		[Mac (10,11)]
		[Export ("collectionView:writeItemsAtIndexPaths:toPasteboard:")]
		bool WriteItems (NSCollectionView collectionView, NSSet indexPaths, NSPasteboard pasteboard);

		[Mac (10,11)]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSFilePromiseReceiver' objects instead.")]
		[Export ("collectionView:namesOfPromisedFilesDroppedAtDestination:forDraggedItemsAtIndexPaths:")]
		string[] GetNamesOfPromisedFiles (NSCollectionView collectionView, NSUrl dropURL, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:draggingImageForItemsAtIndexPaths:withEvent:offset:")]
		NSImage GetDraggingImage (NSCollectionView collectionView, NSSet indexPaths, NSEvent theEvent, ref CGPoint dragImageOffset);

#if !XAMCORE_4_0
		[Mac (10,11)]
		[Export ("collectionView:validateDrop:proposedIndexPath:dropOperation:")]
		NSDragOperation ValidateDropOperation (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation);
#else
		[Mac (10,11)]
		[Export ("collectionView:validateDrop:proposedIndexPath:dropOperation:")]
		NSDragOperation ValidateDrop (NSCollectionView collectionView, INSDraggingInfo draggingInfo, ref NSIndexPath proposedDropIndexPath, ref NSCollectionViewDropOperation proposedDropOperation);
#endif

		[Mac (10,11)]
		[Export ("collectionView:acceptDrop:indexPath:dropOperation:")]
		bool AcceptDrop (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo, NSIndexPath indexPath, NSCollectionViewDropOperation dropOperation);

		[Mac (10,11)]
		[Export ("collectionView:pasteboardWriterForItemAtIndexPath:")]
		[return: NullAllowed]
		INSPasteboardWriting GetPasteboardWriter (NSCollectionView collectionView, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("collectionView:draggingSession:willBeginAtPoint:forItemsAtIndexPaths:")]
		void DraggingSessionWillBegin (NSCollectionView collectionView, NSDraggingSession session, CGPoint screenPoint, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:shouldChangeItemsAtIndexPaths:toHighlightState:")]
		NSSet ShouldChangeItems (NSCollectionView collectionView, NSSet indexPaths, NSCollectionViewItemHighlightState highlightState);

		[Mac (10,11)]
		[Export ("collectionView:didChangeItemsAtIndexPaths:toHighlightState:")]
		void ItemsChanged (NSCollectionView collectionView, NSSet indexPaths, NSCollectionViewItemHighlightState highlightState);

		[Mac (10,11)]
		[Export ("collectionView:shouldSelectItemsAtIndexPaths:")]
		NSSet ShouldSelectItems (NSCollectionView collectionView, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:shouldDeselectItemsAtIndexPaths:")]
		NSSet ShouldDeselectItems (NSCollectionView collectionView, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:didSelectItemsAtIndexPaths:")]
		void ItemsSelected (NSCollectionView collectionView, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:didDeselectItemsAtIndexPaths:")]
		void ItemsDeselected (NSCollectionView collectionView, NSSet indexPaths);

		[Mac (10,11)]
		[Export ("collectionView:willDisplayItem:forRepresentedObjectAtIndexPath:")]
		void WillDisplayItem (NSCollectionView collectionView, NSCollectionViewItem item, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("collectionView:willDisplaySupplementaryView:forElementKind:atIndexPath:")]
		void WillDisplaySupplementaryView (NSCollectionView collectionView, NSView view, NSString elementKind, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("collectionView:didEndDisplayingItem:forRepresentedObjectAtIndexPath:")]
		void DisplayingItemEnded (NSCollectionView collectionView, NSCollectionViewItem item, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("collectionView:didEndDisplayingSupplementaryView:forElementOfKind:atIndexPath:")]
		void DisplayingSupplementaryViewEnded (NSCollectionView collectionView, NSView view, string elementKind, NSIndexPath indexPath);

		[Mac (10,11)]
		[Export ("collectionView:transitionLayoutForOldLayout:newLayout:")]
		NSCollectionViewTransitionLayout TransitionLayout (NSCollectionView collectionView, NSCollectionViewLayout fromLayout, NSCollectionViewLayout toLayout);
	}

	interface INSCollectionViewElement {}

	[Mac (10,11)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewElement : NSUserInterfaceItemIdentification
	{
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Export ("applyLayoutAttributes:")]
		void ApplyLayoutAttributes (NSCollectionViewLayoutAttributes layoutAttributes);

		[Export ("willTransitionFromLayout:toLayout:")]
		void WillTransition (NSCollectionViewLayout oldLayout, NSCollectionViewLayout newLayout);

		[Export ("didTransitionFromLayout:toLayout:")]
		void DidTransition (NSCollectionViewLayout oldLayout, NSCollectionViewLayout newLayout);

		[Export ("preferredLayoutAttributesFittingAttributes:")]
		NSCollectionViewLayoutAttributes GetPreferredLayoutAttributes (NSCollectionViewLayoutAttributes layoutAttributes);
	}

	[Static]
	interface NSCollectionElementKind
	{
		[Mac (10,11)]
		[Field ("NSCollectionElementKindInterItemGapIndicator")]
		NSString InterItemGapIndicator { get; }

		[Mac (10,11)]
		[Field ("NSCollectionElementKindSectionHeader")]
		NSString SectionHeader { get; }

		[Mac (10,11)]
		[Field ("NSCollectionElementKindSectionFooter")]
		NSString SectionFooter { get; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewLayoutAttributes : NSCopying
	{
		[Export ("frame", ArgumentSemantic.Assign)]
		CGRect Frame { get; set; }

		[Export ("size", ArgumentSemantic.Assign)]
		CGSize Size { get; set; }

		[Export ("alpha", ArgumentSemantic.Assign)]
		nfloat Alpha { get; set; }

		[Export ("zIndex", ArgumentSemantic.Assign)]
		nint ZIndex { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[NullAllowed, Export ("indexPath", ArgumentSemantic.Strong)]
		NSIndexPath IndexPath { get; set; }

		[Export ("representedElementCategory")]
		NSCollectionElementCategory RepresentedElementCategory { get; }

		[NullAllowed, Export ("representedElementKind")]
		string RepresentedElementKind { get; }

		[Static]
		[Export ("layoutAttributesForItemWithIndexPath:")]
		NSCollectionViewLayoutAttributes CreateForItem (NSIndexPath indexPath);

		[Static]
		[Export ("layoutAttributesForInterItemGapBeforeIndexPath:")]
		NSCollectionViewLayoutAttributes CreateForInterItemGap (NSIndexPath indexPath);

		[Static]
		[Export ("layoutAttributesForSupplementaryViewOfKind:withIndexPath:")]
		NSCollectionViewLayoutAttributes CreateForSupplementaryView (NSString elementKind, NSIndexPath indexPath);

		[Static]
		[Export ("layoutAttributesForDecorationViewOfKind:withIndexPath:")]
		NSCollectionViewLayoutAttributes CreateForDecorationView (NSString decorationViewKind, NSIndexPath indexPath);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewUpdateItem
	{
		[NullAllowed, Export ("indexPathBeforeUpdate")]
		NSIndexPath IndexPathBeforeUpdate { get; }

		[NullAllowed, Export ("indexPathAfterUpdate")]
		NSIndexPath IndexPathAfterUpdate { get; }

		[Export ("updateAction")]
		NSCollectionUpdateAction UpdateAction { get; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewLayoutInvalidationContext
	{
		[Export ("invalidateEverything")]
		bool InvalidateEverything { get; }

		[Export ("invalidateDataSourceCounts")]
		bool InvalidateDataSourceCounts { get; }

		[Export ("invalidateItemsAtIndexPaths:")]
		void InvalidateItems (NSSet indexPaths);

		[Export ("invalidateSupplementaryElementsOfKind:atIndexPaths:")]
		void InvalidateSupplementaryElements (NSString elementKind, NSSet indexPaths);

		[Export ("invalidateDecorationElementsOfKind:atIndexPaths:")]
		void InvalidateDecorationElements (NSString elementKind, NSSet indexPaths);

		[Export ("invalidatedItemIndexPaths")]
		NSSet InvalidatedItemIndexPaths { get; }

		[Export ("invalidatedSupplementaryIndexPaths")]
		NSDictionary InvalidatedSupplementaryIndexPaths { get; }

		[Export ("invalidatedDecorationIndexPaths")]
		NSDictionary InvalidatedDecorationIndexPaths { get; }

		[Export ("contentOffsetAdjustment", ArgumentSemantic.Assign)]
		CGPoint ContentOffsetAdjustment { get; set; }

		[Export ("contentSizeAdjustment", ArgumentSemantic.Assign)]
		CGSize ContentSizeAdjustment { get; set; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSCollectionViewLayout : NSCoding
	{
		[NullAllowed, Export ("collectionView", ArgumentSemantic.Weak)]
		NSCollectionView CollectionView { get; }

		[Export ("invalidateLayout")]
		void InvalidateLayout ();

		[Export ("invalidateLayoutWithContext:")]
		void InvalidateLayout (NSCollectionViewLayoutInvalidationContext context);

		[Export ("registerClass:forDecorationViewOfKind:"), Internal]
		void _RegisterClassForDecorationView ([NullAllowed] IntPtr viewClass, NSString elementKind);

		[Export ("registerNib:forDecorationViewOfKind:")]
		void RegisterNib ([NullAllowed] NSNib nib, NSString elementKind);

		//
		// NSSubclassingHooks
		//

		// +(__nonnull Class)layoutAttributesClass;
		[Static]
		[Export ("layoutAttributesClass")]
		// [Verify (MethodToProperty)]
		Class LayoutAttributesClass { get; }

		// +(__nonnull Class)invalidationContextClass;
		[Static]
		[Export ("invalidationContextClass")]
		// [Verify (MethodToProperty)]
		Class InvalidationContextClass { get; }

		[Export ("prepareLayout")]
		void PrepareLayout ();

		// -(__nonnull NSArray *)layoutAttributesForElementsInRect:(NSRect)rect;
		[Export ("layoutAttributesForElementsInRect:")]
		// [Verify (StronglyTypedNSArray)]
		NSCollectionViewLayoutAttributes[] GetLayoutAttributesForElements (CGRect rect);

		[Export ("layoutAttributesForItemAtIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributesForItem (NSIndexPath indexPath);

		[Export ("layoutAttributesForSupplementaryViewOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributesForSupplementaryView (NSString elementKind, NSIndexPath indexPath);

		[Export ("layoutAttributesForDecorationViewOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributesForDecorationView (NSString elementKind, NSIndexPath indexPath);

		[Export ("layoutAttributesForDropTargetAtPoint:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributesForDropTarget (CGPoint pointInCollectionView);

		[Export ("layoutAttributesForInterItemGapBeforeIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetLayoutAttributesForInterItemGap (NSIndexPath indexPath);

		[Export ("shouldInvalidateLayoutForBoundsChange:")]
		bool ShouldInvalidateLayout (CGRect newBounds);

		[Export ("invalidationContextForBoundsChange:")]
		NSCollectionViewLayoutInvalidationContext GetInvalidationContext (CGRect newBounds);

		[Export ("shouldInvalidateLayoutForPreferredLayoutAttributes:withOriginalAttributes:")]
		bool ShouldInvalidateLayout (NSCollectionViewLayoutAttributes preferredAttributes, NSCollectionViewLayoutAttributes originalAttributes);

		[Export ("invalidationContextForPreferredLayoutAttributes:withOriginalAttributes:")]
		NSCollectionViewLayoutInvalidationContext GetInvalidationContext (NSCollectionViewLayoutAttributes preferredAttributes, NSCollectionViewLayoutAttributes originalAttributes);

		[Export ("targetContentOffsetForProposedContentOffset:withScrollingVelocity:")]
		CGPoint GetTargetContentOffset (CGPoint proposedContentOffset, CGPoint velocity);

		[Export ("targetContentOffsetForProposedContentOffset:")]
		CGPoint GetTargetContentOffset (CGPoint proposedContentOffset);

		[Export ("collectionViewContentSize")]
		// [Verify (MethodToProperty)]
		CGSize CollectionViewContentSize { get; }

		//
		// NSUpdateSupportHooks
		//

		[Export ("prepareForCollectionViewUpdates:")]
		void PrepareForCollectionViewUpdates (NSCollectionViewUpdateItem[] updateItems);

		[Export ("finalizeCollectionViewUpdates")]
		void FinalizeCollectionViewUpdates ();

		[Export ("prepareForAnimatedBoundsChange:")]
		void PrepareForAnimatedBoundsChange (CGRect oldBounds);

		[Export ("finalizeAnimatedBoundsChange")]
		void FinalizeAnimatedBoundsChange ();

		[Export ("prepareForTransitionToLayout:")]
		void PrepareForTransitionToLayout (NSCollectionViewLayout newLayout);

		[Export ("prepareForTransitionFromLayout:")]
		void PrepareForTransitionFromLayout (NSCollectionViewLayout oldLayout);

		[Export ("finalizeLayoutTransition")]
		void FinalizeLayoutTransition ();

		[Export ("initialLayoutAttributesForAppearingItemAtIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetInitialLayoutAttributesForAppearingItem (NSIndexPath itemIndexPath);

		[Export ("finalLayoutAttributesForDisappearingItemAtIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetFinalLayoutAttributesForDisappearingItem (NSIndexPath itemIndexPath);

		[Export ("initialLayoutAttributesForAppearingSupplementaryElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetInitialLayoutAttributesForAppearingSupplementaryElement (NSString elementKind, NSIndexPath elementIndexPath);

		[Export ("finalLayoutAttributesForDisappearingSupplementaryElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetFinalLayoutAttributesForDisappearingSupplementaryElement (NSString elementKind, NSIndexPath elementIndexPath);

		[Export ("initialLayoutAttributesForAppearingDecorationElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetInitialLayoutAttributesForAppearingDecorationElement (NSString elementKind, NSIndexPath decorationIndexPath);

		[Export ("finalLayoutAttributesForDisappearingDecorationElementOfKind:atIndexPath:")]
		[return: NullAllowed]
		NSCollectionViewLayoutAttributes GetFinalLayoutAttributesForDisappearingDecorationElement (NSString elementKind, NSIndexPath decorationIndexPath);

		[Export ("indexPathsToDeleteForSupplementaryViewOfKind:")]
		NSSet GetIndexPathsToDeleteForSupplementaryView (NSString elementKind);

		[Export ("indexPathsToDeleteForDecorationViewOfKind:")]
		NSSet GetIndexPathsToDeleteForDecorationView (NSString elementKind);

		[Export ("indexPathsToInsertForSupplementaryViewOfKind:")]
		NSSet GetIndexPathsToInsertForSupplementaryView (NSString elementKind);

		[Export ("indexPathsToInsertForDecorationViewOfKind:")]
		NSSet GetIndexPathsToInsertForDecorationView (NSString elementKind);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSCollectionViewLayoutInvalidationContext))]
	interface NSCollectionViewFlowLayoutInvalidationContext
	{
		[Export ("invalidateFlowLayoutDelegateMetrics")]
		bool InvalidateFlowLayoutDelegateMetrics { get; set; }

		[Export ("invalidateFlowLayoutAttributes")]
		bool InvalidateFlowLayoutAttributes { get; set; }
	}

	[Mac (10,11)]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface NSCollectionViewDelegateFlowLayout: NSCollectionViewDelegate
	{
		[Export ("collectionView:layout:sizeForItemAtIndexPath:")]
		CGSize SizeForItem (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, NSIndexPath indexPath);

		[Export ("collectionView:layout:insetForSectionAtIndex:")]
		NSEdgeInsets InsetForSection (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, nint section);

		[Export ("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
		nfloat MinimumLineSpacing (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, nint section);

		[Export ("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
		nfloat MinimumInteritemSpacingForSection (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, nint section);

		[Export ("collectionView:layout:referenceSizeForHeaderInSection:")]
		CGSize ReferenceSizeForHeader (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, nint section);

		[Export ("collectionView:layout:referenceSizeForFooterInSection:")]
		CGSize ReferenceSizeForFooter (NSCollectionView collectionView, NSCollectionViewLayout collectionViewLayout, nint section);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSCollectionViewLayout))]
	interface NSCollectionViewFlowLayout
	{
		[Export ("minimumLineSpacing", ArgumentSemantic.Assign)]
		nfloat MinimumLineSpacing { get; set; }

		[Export ("minimumInteritemSpacing", ArgumentSemantic.Assign)]
		nfloat MinimumInteritemSpacing { get; set; }

		[Export ("itemSize", ArgumentSemantic.Assign)]
		CGSize ItemSize { get; set; }

		[Export ("estimatedItemSize", ArgumentSemantic.Assign)]
		CGSize EstimatedItemSize { get; set; }

		[Export ("scrollDirection", ArgumentSemantic.Assign)]
		NSCollectionViewScrollDirection ScrollDirection { get; set; }

		[Export ("headerReferenceSize", ArgumentSemantic.Assign)]
		CGSize HeaderReferenceSize { get; set; }

		[Export ("footerReferenceSize", ArgumentSemantic.Assign)]
		CGSize FooterReferenceSize { get; set; }

		[Export ("sectionInset", ArgumentSemantic.Assign)]
		NSEdgeInsets SectionInset { get; set; }

		[Mac (10, 12)]
		[Export ("sectionHeadersPinToVisibleBounds")]
		bool SectionHeadersPinToVisibleBounds { get; set; }

		[Mac (10, 12)]
		[Export ("sectionFootersPinToVisibleBounds")]
		bool SectionFootersPinToVisibleBounds { get; set; }

		[Mac (10,12)]
		[Export ("sectionAtIndexIsCollapsed:")]
		bool SectionAtIndexIsCollapsed (nuint sectionIndex);

		[Mac (10,12)]
		[Export ("collapseSectionAtIndex:")]
		void CollapseSectionAtIndex (nuint sectionIndex);

		[Mac (10,12)]
		[Export ("expandSectionAtIndex:")]
		void ExpandSectionAtIndex (nuint sectionIndex);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSCollectionViewLayout))]
	interface NSCollectionViewGridLayout
	{
		[Export ("margins", ArgumentSemantic.Assign)]
		NSEdgeInsets Margins { get; set; }

		[Export ("minimumInteritemSpacing", ArgumentSemantic.Assign)]
		nfloat MinimumInteritemSpacing { get; set; }

		[Export ("minimumLineSpacing", ArgumentSemantic.Assign)]
		nfloat MinimumLineSpacing { get; set; }

		[Export ("maximumNumberOfRows", ArgumentSemantic.Assign)]
		nuint MaximumNumberOfRows { get; set; }

		[Export ("maximumNumberOfColumns", ArgumentSemantic.Assign)]
		nuint MaximumNumberOfColumns { get; set; }

		[Export ("minimumItemSize", ArgumentSemantic.Assign)]
		CGSize MinimumItemSize { get; set; }

		[Export ("maximumItemSize", ArgumentSemantic.Assign)]
		CGSize MaximumItemSize { get; set; }

		[Export ("backgroundColors", ArgumentSemantic.Copy)]
		NSColor[] BackgroundColors { get; set; }
	}

	[Mac (10,11)]
	[DisableDefaultCtor]
	[BaseType (typeof(NSCollectionViewLayout))]
	interface NSCollectionViewTransitionLayout
	{
#if !XAMCORE_4_0
		[Obsolete ("Use the constructor that allows you to set currentLayout and newLayout.")]
		[Export ("init")]
		IntPtr Constructor ();
#endif

		[Export ("transitionProgress", ArgumentSemantic.Assign)]
		nfloat TransitionProgress { get; set; }

		[Export ("currentLayout")]
		NSCollectionViewLayout CurrentLayout { get; }

		[Export ("nextLayout")]
		NSCollectionViewLayout NextLayout { get; }

		[Export ("initWithCurrentLayout:nextLayout:")]
		IntPtr Constructor (NSCollectionViewLayout currentLayout, NSCollectionViewLayout newLayout);

		[Export ("updateValue:forAnimatedKey:")]
		void UpdateValue (nfloat value, string key);

		[Export ("valueForAnimatedKey:")]
		nfloat GetValue (string key);
	}

	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // -colorSpaceName not valid for the NSColor <NSColor: 0x1b94780>; need to first convert colorspace.
	partial interface NSColor : NSCoding, NSCopying, NSSecureCoding, NSPasteboardReading, NSPasteboardWriting
	{
		[Static]
		[Export ("colorWithCalibratedWhite:alpha:")]
		NSColor FromCalibratedWhite (nfloat white, nfloat alpha);

		[Static]
		[Export ("colorWithCalibratedHue:saturation:brightness:alpha:")]
		NSColor FromCalibratedHsba (nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);

		[Static]
		[Export ("colorWithCalibratedRed:green:blue:alpha:")]
		NSColor FromCalibratedRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Static]
		[Export ("colorWithDeviceWhite:alpha:")]
		NSColor FromDeviceWhite (nfloat white, nfloat alpha);

		[Static]
		[Export ("colorWithDeviceHue:saturation:brightness:alpha:")]
		NSColor FromDeviceHsba (nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);

		[Static]
		[Export ("colorWithDeviceRed:green:blue:alpha:")]
		NSColor FromDeviceRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Static]
		[Export ("colorWithDeviceCyan:magenta:yellow:black:alpha:")]
		NSColor FromDeviceCymka (nfloat cyan, nfloat magenta, nfloat yellow, nfloat black, nfloat alpha);

		[Static]
		[Export ("colorWithCatalogName:colorName:")]
		NSColor FromCatalogName (string listName, string colorName);

		[Static]
		[Export ("colorWithColorSpace:components:count:"), Internal]
		NSColor _FromColorSpace (NSColorSpace space, IntPtr components, nint numberOfComponents);

		[Mac (10,9)]
		[Static, Export ("colorWithWhite:alpha:")]
		NSColor FromWhite (nfloat white, nfloat alpha);

		[Mac (10,9)]
		[Static, Export ("colorWithRed:green:blue:alpha:")]
		NSColor FromRgba (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Mac (10,9)]
		[Static, Export ("colorWithHue:saturation:brightness:alpha:")]
		NSColor FromHsba (nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);
		
		[Static]
		[Export ("blackColor")]
		NSColor Black { get; }

		[Static]
		[Export ("darkGrayColor")]
		NSColor DarkGray { get; } 

		[Static]
		[Export ("lightGrayColor")]
		NSColor LightGray { get; }

		[Static]
		[Export ("whiteColor")]
		NSColor White { get; }

		[Static]
		[Export ("grayColor")]
		NSColor Gray { get; }

		[Static]
		[Export ("redColor")]
		NSColor Red { get; }

		[Static]
		[Export ("greenColor")]
		NSColor Green { get; }

		[Static]
		[Export ("blueColor")]
		NSColor Blue { get; }

		[Static]
		[Export ("cyanColor")]
		NSColor Cyan { get; }

		[Static]
		[Export ("yellowColor")]
		NSColor Yellow { get; }

		[Static]
		[Export ("magentaColor")]
		NSColor Magenta { get; }

		[Static]
		[Export ("orangeColor")]
		NSColor Orange { get; }

		[Static]
		[Export ("purpleColor")]
		NSColor Purple { get; }

		[Static]
		[Export ("brownColor")]
		NSColor Brown { get; }

		[Static]
		[Export ("clearColor")]
		NSColor Clear { get; }

		[Static]
		[Export ("controlShadowColor")]
		NSColor ControlShadow { get; }

		[Static]
		[Export ("controlDarkShadowColor")]
		NSColor ControlDarkShadow { get; }

		[Static]
		[Export ("controlColor")]
		NSColor Control { get; }

		[Static]
		[Export ("controlHighlightColor")]
		NSColor ControlHighlight { get; }

		[Static]
		[Export ("controlLightHighlightColor")]
		NSColor ControlLightHighlight { get; }

		[Static]
		[Export ("controlTextColor")]
		NSColor ControlText { get; }

		[Static]
		[Export ("controlBackgroundColor")]
		NSColor ControlBackground { get; }

		[Static]
		[Export ("selectedControlColor")]
		NSColor SelectedControl { get; }

		[Static]
		[Export ("secondarySelectedControlColor")]
		NSColor SecondarySelectedControl { get; }

		[Static]
		[Export ("selectedControlTextColor")]
		NSColor SelectedControlText { get; }

		[Static]
		[Export ("disabledControlTextColor")]
		NSColor DisabledControlText { get; }

		[Static]
		[Export ("textColor")]
		NSColor Text { get; }

		[Static]
		[Export ("textBackgroundColor")]
		NSColor TextBackground { get; }

		[Static]
		[Export ("selectedTextColor")]
		NSColor SelectedText { get; }

		[Static]
		[Export ("selectedTextBackgroundColor")]
		NSColor SelectedTextBackground { get; }

		[Static]
		[Export ("gridColor")]
		NSColor Grid { get; }

		[Static]
		[Export ("keyboardFocusIndicatorColor")]
		NSColor KeyboardFocusIndicator { get; }

		[Static]
		[Export ("windowBackgroundColor")]
		NSColor WindowBackground { get; }

		[Static]
		[Export ("scrollBarColor")]
		NSColor ScrollBar { get; }

		[Static]
		[Export ("knobColor")]
		NSColor Knob { get; }

		[Static]
		[Export ("selectedKnobColor")]
		NSColor SelectedKnob { get; }

		[Static]
		[Export ("windowFrameColor")]
		NSColor WindowFrame { get; }

		[Static]
		[Export ("windowFrameTextColor")]
		NSColor WindowFrameText { get; }

		[Static]
		[Export ("selectedMenuItemColor")]
		NSColor SelectedMenuItem { get; }

		[Static]
		[Export ("selectedMenuItemTextColor")]
		NSColor SelectedMenuItemText { get; }

		[Static]
		[Export ("highlightColor")]
		NSColor Highlight { get; }

		[Static]
		[Export ("shadowColor")]
		NSColor Shadow { get; }

		[Static]
		[Export ("headerColor")]
		NSColor Header { get; }

		[Static]
		[Export ("headerTextColor")]
		NSColor HeaderText { get; }

		[Static]
		[Export ("alternateSelectedControlColor")]
		NSColor AlternateSelectedControl { get; }

		[Static]
		[Export ("alternateSelectedControlTextColor")]
		NSColor AlternateSelectedControlText { get; }

		[Static]
		[Export ("controlAlternatingRowBackgroundColors")]
		NSColor [] ControlAlternatingRowBackgroundColors ();

		[Export ("highlightWithLevel:")]
		NSColor HighlightWithLevel (nfloat highlightLevel);

		[Export ("shadowWithLevel:")]
		NSColor ShadowWithLevel (nfloat shadowLevel);

		[Static]
		[Export ("colorForControlTint:")]
		NSColor FromControlTint (NSControlTint controlTint);

		[Static]
		[Export ("currentControlTint")]
		NSControlTint CurrentControlTint { get; }

		[Export ("set")]
		void Set ();

		[Export ("setFill")]
		void SetFill ();

		[Export ("setStroke")]
		void SetStroke ();

		[Export ("colorSpaceName")]
		string ColorSpaceName { get; }

		[Export ("colorUsingColorSpaceName:")]
		NSColor UsingColorSpace ([NullAllowed] string colorSpaceName);

		[Export ("colorUsingColorSpaceName:device:")]
		NSColor UsingColorSpace ([NullAllowed] string colorSpaceName, [NullAllowed] NSDictionary deviceDescription);

		[Export ("colorUsingColorSpace:")]
		NSColor UsingColorSpace (NSColorSpace colorSpace);

		[Export ("blendedColorWithFraction:ofColor:")]
		NSColor BlendedColor (nfloat fraction, NSColor color);

		[Export ("colorWithAlphaComponent:")]
		NSColor ColorWithAlphaComponent (nfloat alpha);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("catalogNameComponent")]
		string CatalogNameComponent { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("colorNameComponent")]
		string ColorNameComponent { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("localizedCatalogNameComponent")]
		string LocalizedCatalogNameComponent { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("localizedColorNameComponent")]
		string LocalizedColorNameComponent { get; }

		[Export ("redComponent")]
		nfloat RedComponent { [MarshalNativeExceptions] get; }

		[Export ("greenComponent")]
		nfloat GreenComponent { [MarshalNativeExceptions] get; }

		[Export ("blueComponent")]
		nfloat BlueComponent { [MarshalNativeExceptions] get; }

		[Export ("getRed:green:blue:alpha:")]
		void GetRgba (out nfloat red, out nfloat green, out nfloat blue, out nfloat alpha);

		[Export ("hueComponent")]
		nfloat HueComponent { [MarshalNativeExceptions] get; }

		[Export ("saturationComponent")]
		nfloat SaturationComponent { [MarshalNativeExceptions] get; }

		[Export ("brightnessComponent")]
		nfloat BrightnessComponent { [MarshalNativeExceptions] get; }

		[Export ("getHue:saturation:brightness:alpha:")]
		void GetHsba (out nfloat hue, out nfloat saturation, out nfloat brightness, out nfloat alpha);

		[Export ("whiteComponent")]
		nfloat WhiteComponent { [MarshalNativeExceptions] get; }

		[Export ("getWhite:alpha:")]
		void GetWhiteAlpha (out nfloat white, out nfloat alpha);

		[Export ("cyanComponent")]
		nfloat CyanComponent { [MarshalNativeExceptions] get; }

		[Export ("magentaComponent")]
		nfloat MagentaComponent { [MarshalNativeExceptions] get; }

		[Export ("yellowComponent")]
		nfloat YellowComponent { [MarshalNativeExceptions] get; }

		[Export ("blackComponent")]
		nfloat BlackComponent { [MarshalNativeExceptions] get; }

		[Export ("getCyan:magenta:yellow:black:alpha:")]
		void GetCmyka (out nfloat cyan, out nfloat magenta, out nfloat yellow, out nfloat black, out nfloat alpha);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("colorSpace")]
		NSColorSpace ColorSpace { get; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("numberOfComponents")]
		nint ComponentCount { get; }

		[Export ("getComponents:"), Internal]
		void _GetComponents (IntPtr components);

		[Export ("alphaComponent")]
		nfloat AlphaComponent { [MarshalNativeExceptions] get; }

		[Static]
		[Export ("colorFromPasteboard:")]
		NSColor FromPasteboard (NSPasteboard pasteBoard);

		[Export ("writeToPasteboard:")]
		void WriteToPasteboard (NSPasteboard pasteBoard);

		[Static]
		[Export ("colorWithPatternImage:")]
		NSColor FromPatternImage (NSImage image);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("patternImage")]
		NSImage PatternImage { get; }

		[Mac (10, 8)]
		[Export ("CGColor")]
		CGColor CGColor { get; }

		[Export ("drawSwatchInRect:")]
		void DrawSwatchInRect (CGRect rect);

		[Static]
		[Export ("ignoresAlpha")]
		bool IgnoresAlpha { get; set; }

		[Static]
		[Export ("colorWithCIColor:")]
		NSColor FromCIColor (CIColor color);

		[Mac (10,10)]
		[Static, Export ("labelColor")]
		NSColor LabelColor { get; }

		[Mac (10,10)]
		[Static, Export ("secondaryLabelColor")]
		NSColor SecondaryLabelColor { get; }

		[Mac (10,10)]
		[Static, Export ("tertiaryLabelColor")]
		NSColor TertiaryLabelColor { get; } 

		[Mac (10,10)]
		[Static, Export ("quaternaryLabelColor")]
		NSColor QuaternaryLabelColor { get; }
		
		[Mac (10,12)]
		[Static]
		[Export ("colorWithDisplayP3Red:green:blue:alpha:")]
		NSColor FromDisplayP3 (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Mac (10,12)]
		[Static]
		[Export ("colorWithColorSpace:hue:saturation:brightness:alpha:")]
		NSColor FromColor (NSColorSpace space, nfloat hue, nfloat saturation, nfloat brightness, nfloat alpha);

		[Mac (10, 12, 2)]
		[Static]
		[Export ("scrubberTexturedBackgroundColor", ArgumentSemantic.Strong)]
		NSColor ScrubberTexturedBackgroundColor { get; }

		[Mac (10,13)]
		[Static]
		[Export ("colorNamed:bundle:")]
		[return: NullAllowed]
		NSColor FromName (string name, [NullAllowed] NSBundle bundle);

		[Mac (10,13)]
		[Static]
		[Export ("colorNamed:")]
		[return: NullAllowed]
		NSColor FromName (string name);

		[Mac (10, 13)]
		[Export ("type")]
		NSColorType Type { get; }

		[Mac (10,13)]
		[Export ("colorUsingType:")]
		[return: NullAllowed]
		NSColor GetColor (NSColorType type);

		[Mac (10, 10)]
		[Static]
		[Export ("systemRedColor", ArgumentSemantic.Strong)]
		NSColor SystemRedColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemGreenColor", ArgumentSemantic.Strong)]
		NSColor SystemGreenColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemBlueColor", ArgumentSemantic.Strong)]
		NSColor SystemBlueColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemOrangeColor", ArgumentSemantic.Strong)]
		NSColor SystemOrangeColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemYellowColor", ArgumentSemantic.Strong)]
		NSColor SystemYellowColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemBrownColor", ArgumentSemantic.Strong)]
		NSColor SystemBrownColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemPinkColor", ArgumentSemantic.Strong)]
		NSColor SystemPinkColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemPurpleColor", ArgumentSemantic.Strong)]
		NSColor SystemPurpleColor { get; }

		[Mac (10, 10)]
		[Static]
		[Export ("systemGrayColor", ArgumentSemantic.Strong)]
		NSColor SystemGrayColor { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSColorList : NSSecureCoding {
		[Static]
		[Export ("availableColorLists")]
		NSColorList [] AvailableColorLists { get; }

		[Static]
		[Export ("colorListNamed:")]
		NSColorList ColorListNamed (string name);

		[Export ("initWithName:")]
		IntPtr Constructor (string name);

		[Export ("initWithName:fromFile:")]
		IntPtr Constructor (string name, [NullAllowed] string path);

		[Export ("name")]
		string Name { get; }

		[Export ("setColor:forKey:")]
		void SetColorForKey (NSColor color, string key);

		[Export ("insertColor:key:atIndex:")]
		void InsertColor (NSColor color, string key, nint indexPos);

		[Export ("removeColorWithKey:")]
		void RemoveColor (string key);

		[Export ("colorWithKey:")]
		NSColor ColorWithKey (string key);

		[Export ("allKeys")]
		string [] AllKeys ();

		[Export ("isEditable")]
		bool IsEditable { get; }

		[Export ("writeToFile:")]
		bool WriteToFile ([NullAllowed] string path);

		[Export ("removeFile")]
		void RemoveFile ();

		[Mac (10,11)]
		[Export ("writeToURL:error:")]
		bool WriteToUrl ([NullAllowed] NSUrl url, [NullAllowed] out NSError error);
	}

	[BaseType (typeof (NSPanel))]
	partial interface NSColorPanel {
		[Static, Export ("sharedColorPanel")]
		NSColorPanel SharedColorPanel { get; }

		[Static]
		[Export ("sharedColorPanelExists")]
		bool SharedColorPanelExists { get; }

		[Static]
		[Export ("dragColor:withEvent:fromView:")]
		bool DragColor (NSColor color, NSEvent theEvent, NSView sourceView);

		[Static]
		[Export ("setPickerMask:")]
		void SetPickerStyle (NSColorPanelFlags mask);

		[Static]
		[Export ("setPickerMode:")]
		void SetPickerMode (NSColorPanelMode mode);

		[Export ("alpha")]
		nfloat Alpha { get; }

		[Export ("setAction:")]
		void SetAction ([NullAllowed] Selector aSelector);

		[Export ("setTarget:")]
		void SetTarget ([NullAllowed] NSObject anObject);

		[Export ("attachColorList:")]
		void AttachColorList (NSColorList colorList);

		[Export ("detachColorList:")]
		void DetachColorList (NSColorList colorList);

		//Detected properties
		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; }

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")]get; set; }

		[Export ("showsAlpha")]
		bool ShowsAlpha { get; set; }

		[Export ("mode")]
		NSColorPanelMode Mode { get; set; }

		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }

	}

	[BaseType (typeof (NSObject))]
	interface NSColorPicker {
		[Export ("initWithPickerMask:colorPanel:")]
		IntPtr Constructor (NSColorPanelFlags mask, NSColorPanel owningColorPanel);

		[Export ("colorPanel")]
		NSColorPanel ColorPanel { get; }

		[Export ("provideNewButtonImage")]
		NSImage ProvideNewButtonImage ();

		[Export ("insertNewButtonImage:in:")]
		void InsertNewButtonImage (NSImage newButtonImage, NSButtonCell buttonCell);

		[Export ("viewSizeChanged:")]
		void ViewSizeChanged (NSObject sender);

		[Export ("attachColorList:")]
		void AttachColorList (NSColorList colorList);

		[Export ("detachColorList:")]
		void DetachColorList (NSColorList colorList);

		[Export ("setMode:")]
		void SetMode (NSColorPanelMode mode);

		[Export ("buttonToolTip")]
		string ButtonToolTip { get; }

		[Export ("minContentSize")]
		CGSize MinContentSize { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSColorSpace : NSCoding, NSSecureCoding {
		[Export ("initWithICCProfileData:")]
		IntPtr Constructor (NSData iccData);

		[Export ("ICCProfileData")]
		NSData ICCProfileData { get; }

		// Conflicts with the built-in handle intptr
		//[Export ("initWithColorSyncProfile:")]
		//IntPtr Constructor (IntPtr colorSyncProfile);

		[Export ("colorSyncProfile")]
		IntPtr ColorSyncProfile { get; }

		[Export ("initWithCGColorSpace:")]
		IntPtr Constructor (CGColorSpace cgColorSpace);

		[Export ("CGColorSpace")]
		CGColorSpace ColorSpace { get; }

		[Export ("numberOfColorComponents")]
		nint ColorComponents { get; }

		[Export ("colorSpaceModel")]
		NSColorSpaceModel ColorSpaceModel { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }

		[Static]
		[Export ("genericRGBColorSpace")]
		NSColorSpace GenericRGBColorSpace { get; }

		[Static]
		[Export ("genericGrayColorSpace")]
		NSColorSpace GenericGrayColorSpace { get; }

		[Static]
		[Export ("genericCMYKColorSpace")]
		NSColorSpace GenericCMYKColorSpace { get; }

		[Static]
		[Export ("deviceRGBColorSpace")]
		NSColorSpace DeviceRGBColorSpace { get; }

		[Static]
		[Export ("deviceGrayColorSpace")]
		NSColorSpace DeviceGrayColorSpace { get; }

		[Static]
		[Export ("deviceCMYKColorSpace")]
		NSColorSpace DeviceCMYKColorSpace { get; }

		[Static]
		[Export ("sRGBColorSpace")]
		NSColorSpace SRGBColorSpace { get; }

		[Static]
		[Export ("genericGamma22GrayColorSpace")]
		NSColorSpace GenericGamma22GrayColorSpace { get; }

		[Static]
		[Export ("adobeRGB1998ColorSpace")]
		NSColorSpace AdobeRGB1998ColorSpace { get; }

		[Static]
		[Export ("availableColorSpacesWithModel:")]
		NSColorSpace [] AvailableColorSpacesWithModel (NSColorSpaceModel model);

		[Mac (10, 12)]
		[Static]
		[Export ("extendedSRGBColorSpace")]
		NSColorSpace ExtendedSRgbColorSpace { get; }

		[Mac (10, 12)]
		[Static]
		[Export ("extendedGenericGamma22GrayColorSpace")]
		NSColorSpace ExtendedGenericGamma22GrayColorSpace { get; }

		[Mac (10, 12)]
		[Static]
		[Export ("displayP3ColorSpace")]
		NSColorSpace DisplayP3ColorSpace { get; }

		[Field ("NSCalibratedWhiteColorSpace")]
		NSString CalibratedWhite { get; }

		[Field ("NSCalibratedBlackColorSpace")]
		NSString CalibratedBlack { get; }
		
		[Field ("NSCalibratedRGBColorSpace")]
		NSString CalibratedRGB { get; }

		[Field ("NSDeviceWhiteColorSpace")]
		NSString DeviceWhite { get; }

		[Field ("NSDeviceBlackColorSpace")]
		NSString DeviceBlack { get; }

		[Field ("NSDeviceRGBColorSpace")]
		NSString DeviceRGB { get; }

		[Field ("NSDeviceCMYKColorSpace")]
		NSString DeviceCMYK { get; }

		[Field ("NSNamedColorSpace")]
		NSString Named { get; }

		[Field ("NSPatternColorSpace")]
		NSString Pattern { get; }

		[Field ("NSCustomColorSpace")]
		NSString Custom { get; }
	}

	[BaseType (typeof (NSControl))]
	interface NSColorWell {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("deactivate")]
		void Deactivate ();

		[Export ("activate:")]
		void Activate (bool exclusive);

		[Export ("isActive")]
		bool IsActive { get; }

		[Export ("drawWellInside:")]
		void DrawWellInside (CGRect insideRect);

		[Export ("takeColorFrom:")]
		void TakeColorFrom (NSObject sender);

		//Detected properties
		[Export ("bordered")]
		bool Bordered { [Bind ("isBordered")]get; set; }

		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }

	}

	[BaseType (typeof (NSTextField),
		Delegates = new [] { "Delegate" },
		Events = new [] { typeof (NSComboBoxDelegate) }
	)]
	partial interface NSComboBox {

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSComboBoxDelegate Delegate { get; set; }

		[Export ("hasVerticalScroller")]
		bool HasVerticalScroller { get; set; }

		[Export ("intercellSpacing")]
		CGSize IntercellSpacing { get; set; }

		[Export ("itemHeight")]
		nfloat ItemHeight { get; set; }

		[Export ("numberOfVisibleItems")]
		nint VisibleItems { get; set; }

		[Export ("buttonBordered")]
		bool ButtonBordered { [Bind ("isButtonBordered")] get; set; }

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("noteNumberOfItemsChanged")]
		void NoteNumberOfItemsChanged ();

		[Export ("usesDataSource")]
		bool UsesDataSource { get; set; }

		[Export ("scrollItemAtIndexToTop:")]
		void ScrollItemAtIndexToTop (nint scrollItemIndex);

		[Export ("scrollItemAtIndexToVisible:")]
		void ScrollItemAtIndexToVisible (nint scrollItemIndex);

		[Export ("selectItemAtIndex:")]
		void SelectItem (nint itemIndex);

		[Export ("deselectItemAtIndex:")]
		void DeselectItem (nint itemIndex);

		[Export ("indexOfSelectedItem")]
		nint SelectedIndex { get; }

		[Export ("numberOfItems")]
		nint Count { get; }

		[Export ("completes")]
		bool Completes { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign)][NullAllowed]
		[Protocolize]
		NSComboBoxDataSource DataSource { get; set; }

		[Export ("addItemWithObjectValue:")]
		void Add (NSObject object1);

		[Export ("addItemsWithObjectValues:")]
		[PostGet ("Values")]
		void Add (NSObject [] items);

		[Export ("insertItemWithObjectValue:atIndex:")]
		[PostGet ("Values")]
		void Insert (NSObject object1, nint index);

		[Export ("removeItemWithObjectValue:")]
		[PostGet ("Values")]
		void Remove (NSObject object1);

		[Export ("removeItemAtIndex:")]
		[PostGet ("Values")]
		void RemoveAt (nint index);

		[Export ("removeAllItems")]
		[PostGet ("Values")]
		void RemoveAll ();

		[Export ("selectItemWithObjectValue:")]
		void Select (NSObject object1);

		[Export ("itemObjectValueAtIndex:")]
		NSObject GetItemObject (nint index);

		[Export ("objectValueOfSelectedItem")]
		NSObject SelectedValue { get; }

		[Export ("indexOfItemWithObjectValue:")]
		nint IndexOf (NSObject object1);

		[Export ("objectValues")]
		NSObject [] Values { get; }

		[Notification, Field ("NSComboBoxSelectionDidChangeNotification")]
		NSString SelectionDidChangeNotification { get; }

		[Notification, Field ("NSComboBoxSelectionIsChangingNotification")]
		NSString SelectionIsChangingNotification { get; }

		[Notification, Field ("NSComboBoxWillDismissNotification")]
		NSString WillDismissNotification { get; }

		[Notification, Field ("NSComboBoxWillPopUpNotification")]
		NSString WillPopUpNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSComboBoxDataSource {
		[Export ("comboBox:objectValueForItemAtIndex:")]
		NSObject ObjectValueForItem (NSComboBox comboBox, nint index);
		
		[Export ("numberOfItemsInComboBox:")]
		nint ItemCount (NSComboBox comboBox);
		
		[Export ("comboBox:completedString:")]
		string CompletedString (NSComboBox comboBox, string uncompletedString);
		
		[Export ("comboBox:indexOfItemWithStringValue:")]
		nint IndexOfItem (NSComboBox comboBox, string value);
	}

	[BaseType (typeof (NSTextFieldCell))]
	partial interface NSComboBoxCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("hasVerticalScroller")]
		bool HasVerticalScroller { get; set; }

		[Export ("intercellSpacing")]
		CGSize IntercellSpacing { get; set; }

		[Export ("itemHeight")]
		nfloat ItemHeight { get; set; }

		[Export ("numberOfVisibleItems")]
		nint VisibleItems { get; set; }

		[Export ("buttonBordered")]
		bool ButtonBordered { [Bind ("isButtonBordered")] get; set; }

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("noteNumberOfItemsChanged")]
		void NoteNumberOfItemsChanged ();

		[Export ("usesDataSource")]
		bool UsesDataSource { get; set; }

		[Export ("scrollItemAtIndexToTop:")]
		void ScrollItemAtIndexToTop (nint scrollItemIndex);

		[Export ("scrollItemAtIndexToVisible:")]
		void ScrollItemAtIndexToVisible (nint scrollItemIndex);

		[Export ("selectItemAtIndex:")]
		void SelectItem (nint itemIndex);

		[Export ("deselectItemAtIndex:")]
		void DeselectItem (nint itemIndex);

		[Export ("indexOfSelectedItem")]
		nint SelectedIndex { get; }

		[Export ("numberOfItems")]
		nint Count { get; }

		[Export ("completes")]
		bool Completes { get; set; }

		[Export ("dataSource", ArgumentSemantic.Assign)][NullAllowed]
		[Protocolize]
		NSComboBoxCellDataSource DataSource { get; set; }

		[Export ("addItemWithObjectValue:")]
		void Add (NSObject object1);

		[Export ("addItemsWithObjectValues:")]
		[PostGet ("Values")]
		void Add (NSObject [] items);

		[Export ("insertItemWithObjectValue:atIndex:")]
		[PostGet ("Values")]
		void Insert (NSObject object1, nint index);

		[Export ("removeItemWithObjectValue:")]
		[PostGet ("Values")]
		void Remove (NSObject object1);

		[Export ("removeItemAtIndex:")]
		[PostGet ("Values")]
		void RemoveAt (nint index);

		[Export ("removeAllItems")]
		[PostGet ("Values")]
		void RemoveAll ();

		[Export ("selectItemWithObjectValue:")]
		void Select (NSObject object1);

		[Export ("itemObjectValueAtIndex:")]
		NSComboBox GetItem (nint index);

		[Export ("objectValueOfSelectedItem")]
		NSObject SelectedValue { get; }

		[Export ("indexOfItemWithObjectValue:")]
		nint IndexOf (NSObject object1);

		[Export ("objectValues")]
		NSObject [] Values { get; }

		[Export ("completedString:")]
		string CompletedString (string substring);
		
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSComboBoxCellDataSource {
		[Export ("comboBoxCell:objectValueForItemAtIndex:")]
		NSObject ObjectValueForItem (NSComboBoxCell comboBox, nint index);

		[Export ("numberOfItemsInComboBoxCell:")]
		nint ItemCount (NSComboBoxCell comboBox);

		[Export ("comboBoxCell:completedString:")]
		string CompletedString (NSComboBoxCell comboBox, string uncompletedString);

		[Export ("comboBoxCell:indexOfItemWithStringValue:")]
		nuint IndexOfItem (NSComboBoxCell comboBox, string value);
	}

	[BaseType (typeof (NSView))]
	partial interface NSControl {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("sizeToFit")]
		void SizeToFit ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("calcSize")]
		void CalcSize ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("selectedCell")]
		NSCell SelectedCell { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("selectedTag")]
		nint SelectedTag { get; }

		[Export ("sendActionOn:")]
		nint SendActionOn (NSEventType mask);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("setNeedsDisplay")]
		void SetNeedsDisplay ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("updateCell:")]
		void UpdateCell (NSCell aCell);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("updateCellInside:")]
		void UpdateCellInside (NSCell aCell);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("drawCellInside:")]
		void DrawCellInside (NSCell aCell);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("drawCell:")]
		void DrawCell (NSCell aCell);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("selectCell:")]
		void SelectCell (NSCell aCell);

		[Export ("sendAction:to:")]
		bool SendAction (Selector theAction, NSObject theTarget);

		[Export ("takeIntValueFrom:")]
		void TakeIntValueFrom (NSObject sender);

		[Export ("takeFloatValueFrom:")]
		void TakeFloatValueFrom (NSObject sender);

		[Export ("takeDoubleValueFrom:")]
		void TakeDoubleValueFrom (NSObject sender);

		[Export ("takeStringValueFrom:")]
		void TakeStringValueFrom (NSObject sender);

		[Export ("takeObjectValueFrom:")]
		void TakeObjectValueFrom (NSObject sender);

		[Export ("currentEditor")]
		NSText CurrentEditor { get; }

		[Export ("abortEditing")]
		bool AbortEditing ();

		[Export ("validateEditing")]
		void ValidateEditing ();

		[Export ("mouseDown:")]
		void MouseDown (NSEvent theEvent);

		[Export ("takeIntegerValueFrom:")]
		void TakeIntegerValueFrom (NSObject sender);

		[Export ("invalidateIntrinsicContentSizeForCell:"), Mac (10, 7)]
		void InvalidateIntrinsicContentSizeForCell (NSCell cell);

		//Detected properties
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("cellClass")]
		Class CellClass { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("cell")]
		NSCell Cell { get; set; }

		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }

		[Export ("action"), NullAllowed]
		Selector Action { get; set; }

		[Export ("tag")]
		nint Tag { get; set; }

		[Export ("ignoresMultiClick")]
		bool IgnoresMultiClick { get; set; }

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")]get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Export ("alignment")]
		NSTextAlignment Alignment { get; set; }

		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("formatter", ArgumentSemantic.Retain)]
		NSObject Formatter { get; set; }

		[Export ("objectValue", ArgumentSemantic.Copy)]
		NSObject ObjectValue { get; set; }

		[Export ("stringValue")]
		string StringValue { get; set; }

		[Export ("attributedStringValue", ArgumentSemantic.Copy)]
		NSAttributedString AttributedStringValue { get; set; }

		[Export ("intValue")]
		int IntValue { get; set; } /* int, not NSInteger */

		[Export ("floatValue")]
		float FloatValue { get; set; } /* float, not CGFloat */ 

		[Export ("doubleValue")]
		double DoubleValue { get; set; }

		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; set; }

#if XAMCORE_2_0
		[Export ("integerValue")]
		nint NIntValue { get; set; }
#else
		[Export ("integerValue")]
		nint IntegerValue { get; set; }
#endif
		[Export ("performClick:")]
		void PerformClick (NSObject sender);

		[Export ("refusesFirstResponder")]
		bool RefusesFirstResponder { get; set; }

		[Mac (10,10)]
		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; [Bind ("setHighlighted:")] set; }

		[Mac (10,10)]
		[Export ("controlSize")]
		NSControlSize ControlSize { get; set; }

		[Mac (10,10)]
		[Export ("sizeThatFits:")]
		CGSize SizeThatFits (CGSize size);

		[Mac (10,10)]
		[Export ("lineBreakMode")]
		NSLineBreakMode LineBreakMode { get; set; }

		[Mac (10,10)]
		[Export ("usesSingleLineMode")]
		bool UsesSingleLineMode { get; set; }

		[Mac (10,10)]
		[Export ("drawWithExpansionFrame:inView:")]
		void DrawWithExpansionFrame (CGRect cellFrame, NSView view);

		[Mac (10,10)]
		[Export ("editWithFrame:editor:delegate:event:")]
		void EditWithFrame (CGRect aRect, [NullAllowed] NSText textObj, [NullAllowed] NSObject anObject, NSEvent theEvent);

		[Mac (10,10)]
		[Export ("selectWithFrame:editor:delegate:start:length:")]
		void SelectWithFrame (CGRect aRect, [NullAllowed] NSText textObj, [NullAllowed] NSObject anObject, nint selStart, nint selLength);

		[Mac (10,10)]
		[Export ("endEditing:")]
		void EndEditing ([NullAllowed] NSText textObj);
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSController : NSCoding {
		[Export ("objectDidBeginEditing:")]
		void ObjectDidBeginEditing (NSObject editor);

		[Export ("objectDidEndEditing:")]
		void ObjectDidEndEditing (NSObject editor);

		[Export ("discardEditing")]
		void DiscardEditing ();

		[Export ("commitEditing")]
		bool CommitEditing { get; }

		[Export ("commitEditingWithDelegate:didCommitSelector:contextInfo:")]
		void CommitEditingWithDelegate (NSObject delegate1, Selector didCommitSelector, IntPtr contextInfo);

		[Export ("isEditing")]
		bool IsEditing { get; }

	}

	[BaseType (typeof (NSObject))]
	interface NSCursor : NSCoding {
		[Static]
		[Export ("currentCursor")]
		NSCursor CurrentCursor { get; }

		[Static]
		[Export ("currentSystemCursor")]
		[NullAllowed]
		NSCursor CurrentSystemCursor { get; }

		[Static]
		[Export ("arrowCursor")]
		NSCursor ArrowCursor { get; }

		[Static]
		[Export ("IBeamCursor")]
		NSCursor IBeamCursor { get; }

		[Static]
		[Export ("pointingHandCursor")]
		NSCursor PointingHandCursor { get; }

		[Static]
		[Export ("closedHandCursor")]
		NSCursor ClosedHandCursor { get; }

		[Static]
		[Export ("openHandCursor")]
		NSCursor OpenHandCursor { get; }

		[Static]
		[Export ("resizeLeftCursor")]
		NSCursor ResizeLeftCursor { get; }

		[Static]
		[Export ("resizeRightCursor")]
		NSCursor ResizeRightCursor { get; }

		[Static]
		[Export ("resizeLeftRightCursor")]
		NSCursor ResizeLeftRightCursor { get; }

		[Static]
		[Export ("resizeUpCursor")]
		NSCursor ResizeUpCursor { get; }

		[Static]
		[Export ("resizeDownCursor")]
		NSCursor ResizeDownCursor { get; }

		[Static]
		[Export ("resizeUpDownCursor")]
		NSCursor ResizeUpDownCursor { get; }

		[Static]
		[Export ("crosshairCursor")]
		NSCursor CrosshairCursor { get; }

		[Static]
		[Export ("disappearingItemCursor")]
		NSCursor DisappearingItemCursor { get; }

		[Static]
		[Export ("operationNotAllowedCursor")]
		NSCursor OperationNotAllowedCursor { get; }

		[Static]
		[Export ("dragLinkCursor")]
		NSCursor DragLinkCursor { get; }

		[Static]
		[Export ("dragCopyCursor")]
		NSCursor DragCopyCursor { get; }

		[Static]
		[Export ("contextualMenuCursor")]
		NSCursor ContextualMenuCursor { get; }

		[Mac (10, 7)]
		[Static]
		[Export ("IBeamCursorForVerticalLayout")]
		NSCursor IBeamCursorForVerticalLayout { get; }
		
		[DesignatedInitializer]
		[Export ("initWithImage:hotSpot:")]
		IntPtr Constructor (NSImage newImage, CGPoint aPoint);

		[Availability (Deprecated = Platform.Mac_10_12, Message = "Color hints are ignored. Use NSCursor (NSImage newImage, CGPoint aPoint) instead.")]
		[Export ("initWithImage:foregroundColorHint:backgroundColorHint:hotSpot:")]
		IntPtr Constructor (NSImage newImage, NSColor fg, NSColor bg, CGPoint hotSpot);

		[Static]
		[Export ("hide")]
		void Hide ();

		[Static]
		[Export ("unhide")]
		void Unhide ();

		[Static]
		[Export ("setHiddenUntilMouseMoves:")]
		void SetHiddenUntilMouseMoves (bool flag);

		//[Static]
		//[Export ("pop")]
		//void Pop ();

		[Export ("image")]
		NSImage Image { get; }

		[Export ("hotSpot")]
		CGPoint HotSpot { get; }

		[Export ("push")]
		void Push ();

		[Export ("pop")]
		void Pop ();

		[Export ("set")]
		void Set ();

		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Export ("setOnMouseExited:")]
		void SetOnMouseExited (bool flag);

		[Export ("setOnMouseEntered:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void SetOnMouseEntered (bool flag);

		[Export ("isSetOnMouseExited")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		bool IsSetOnMouseExited ();

		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[Export ("isSetOnMouseEntered")]
		bool IsSetOnMouseEntered ();

		[Export ("mouseEntered:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void MouseEntered (NSEvent theEvent);

		[Export ("mouseExited:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void MouseExited (NSEvent theEvent);
	}

	[BaseType (typeof (NSImageRep))]
	[DisableDefaultCtor] // An uncaught exception was raised: -[NSCustomImageRep init]: unrecognized selector sent to instance 0x54a870
	partial interface NSCustomImageRep {
		[Export ("initWithDrawSelector:delegate:")]
		IntPtr Constructor (Selector drawSelectorMethod, NSObject delegateObject);

		[Export ("drawSelector")]
		Selector DrawSelector { get; }
		
		[Export ("delegate", ArgumentSemantic.Assign)]  
		NSObject Delegate { get; }  
	}	

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // - (instancetype)init NS_UNAVAILABLE;
	interface NSDataAsset : NSCopying
	{
		[Export ("initWithName:")]
		IntPtr Constructor (string name);

		[Export ("initWithName:bundle:")]
		[DesignatedInitializer]
		IntPtr Constructor (string name, NSBundle bundle);

		[Export ("name")]
		string Name { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[Export ("typeIdentifier")] // Uniform Type Identifier
		NSString TypeIdentifier { get; }
	}

	[BaseType (typeof (NSControl), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (NSDatePickerCellDelegate)})]
	interface NSDatePicker {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		//Detected properties
		[Export ("datePickerStyle")]
		NSDatePickerStyle DatePickerStyle { get; set; }

		[Export ("bezeled")]
		bool Bezeled { [Bind ("isBezeled")]get; set; }

		[Export ("bordered")]
		bool Bordered { [Bind ("isBordered")]get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("cell")]
		NSDatePickerCell Cell { get; set; }

		[Export ("textColor", ArgumentSemantic.Copy)]
		NSColor TextColor { get; set; }

		[Export ("datePickerMode")]
		NSDatePickerMode DatePickerMode { get; set; }

		[Export ("datePickerElements")]
		NSDatePickerElementFlags DatePickerElements { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("dateValue", ArgumentSemantic.Copy)]
		NSDate DateValue { get; set; }

		[Export ("timeInterval")]
		double TimeInterval { get; set; }

		[Export ("minDate", ArgumentSemantic.Copy)]
		NSDate MinDate { get; set; }

		[Export ("maxDate", ArgumentSemantic.Copy)]
		NSDate MaxDate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSDatePickerCellDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSActionCell), Delegates=new string [] {"WeakDelegate"}, Events=new Type [] {typeof (NSDatePickerCellDelegate)})]
	interface NSDatePickerCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		//Detected properties
		[Export ("datePickerStyle")]
		NSDatePickerStyle DatePickerStyle { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("textColor", ArgumentSemantic.Copy)]
		NSColor TextColor { get; set; }

		[Export ("datePickerMode")]
		NSDatePickerMode DatePickerMode { get; set; }

		[Export ("datePickerElements")]
		NSDatePickerElementFlags DatePickerElements { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("dateValue", ArgumentSemantic.Copy)]
		NSDate DateValue { get; set; }

		[Export ("timeInterval")]
		double TimeInterval { get; set; }

		[Export ("minDate", ArgumentSemantic.Copy)]
		NSDate MinDate { get; set; }

		[Export ("maxDate", ArgumentSemantic.Copy)]
		NSDate MaxDate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSDatePickerCellDelegate Delegate { get; set; }

	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSDatePickerCellDelegate {
		[Export ("datePickerCell:validateProposedDateValue:timeInterval:"), EventArgs ("NSDatePickerValidator")]
		void ValidateProposedDateValue (NSDatePickerCell aDatePickerCell, ref NSDate proposedDateValue, double proposedTimeInterval);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSDictionaryControllerKeyValuePair
	{
		[NullAllowed, Export ("key")]
		string Key { get; set; }

		[NullAllowed, Export ("value", ArgumentSemantic.Strong)]
		NSObject Value { get; set; }

		[NullAllowed, Export ("localizedKey")]
		string LocalizedKey { get; set; }

		[Export ("explicitlyIncluded")]
		bool ExplicitlyIncluded { [Bind ("isExplicitlyIncluded")] get; }
	}

	[BaseType (typeof(NSArrayController))]
	interface NSDictionaryController
	{
		// -(NSDictionaryControllerKeyValuePair * __nonnull)newObject;
		[Export ("newObject")]
		// [Verify (MethodToProperty)]
		NSDictionaryControllerKeyValuePair NewObject { get; }

		[Export ("initialKey")]
		string InitialKey { get; set; }

		[Export ("initialValue", ArgumentSemantic.Strong)]
		NSObject InitialValue { get; set; }

		[Export ("includedKeys", ArgumentSemantic.Copy)]
		string[] IncludedKeys { get; set; }

		[Export ("excludedKeys", ArgumentSemantic.Copy)]
		string[] ExcludedKeys { get; set; }

		[Export ("localizedKeyDictionary", ArgumentSemantic.Copy)]
		NSDictionary LocalizedKeyDictionary { get; set; }

		[NullAllowed, Export ("localizedKeyTable")]
		string LocalizedKeyTable { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSDockTile {
		[Export ("size")]
		CGSize Size { get; }

		[Export ("display")]
		void Display ();

		[Export ("owner")]
		NSObject Owner { get; }

		//Detected properties
		[Export ("contentView", ArgumentSemantic.Retain)]
		NSView ContentView { get; set; }

		[Export ("showsApplicationBadge")]
		bool ShowsApplicationBadge { get; set; }

		[Export ("badgeLabel"), NullAllowed]
		string BadgeLabel { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSDockTilePlugIn {
		[Abstract]
		[Export ("setDockTile:")]
		void SetDockTile (NSDockTile dockTile);

		[Abstract]
		[Export ("dockMenu")]
		NSMenu DockMenu ();
	}

	delegate void NSDocumentCompletionHandler (IntPtr nsErrorPointerOrZero);
	
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSDocument {
		[Export ("initWithType:error:")]
		IntPtr Constructor (string typeName, out NSError outError);

		[Static]
		[Export ("canConcurrentlyReadDocumentsOfType:")]
		bool CanConcurrentlyReadDocumentsOfType (string typeName);

		[Export ("initWithContentsOfURL:ofType:error:")]
		IntPtr Constructor (NSUrl url, string typeName, out NSError outError);

		[Export ("initForURL:withContentsOfURL:ofType:error:")]
		IntPtr Constructor ([NullAllowed] NSUrl documentUrl, NSUrl documentContentsUrl, string typeName, out NSError outError);

		 [Export ("revertDocumentToSaved:")]
		 void RevertDocumentToSaved (NSObject sender);

		 [Export ("revertToContentsOfURL:ofType:error:")]
		 bool RevertToContentsOfUrl (NSUrl url, string typeName, out NSError outError);

		[Export ("readFromURL:ofType:error:")]
		bool ReadFromUrl (NSUrl url, string typeName, out NSError outError);

		[Export ("readFromFileWrapper:ofType:error:")]
		bool ReadFromFileWrapper (NSFileWrapper fileWrapper, string typeName, out NSError outError);

		[Export ("readFromData:ofType:error:")]
		bool ReadFromData (NSData data, string typeName, out NSError outError);

		[Export ("writeToURL:ofType:error:")]
		bool WriteToUrl (NSUrl url, string typeName, out NSError outError);

		[Export ("fileWrapperOfType:error:")]
		NSFileWrapper GetAsFileWrapper (string typeName, out NSError outError);

		[Export ("dataOfType:error:")]
		NSData GetAsData (string typeName, out NSError outError);

		[Export ("writeSafelyToURL:ofType:forSaveOperation:error:")]
		bool WriteSafelyToUrl (NSUrl url, string typeName, NSSaveOperationType saveOperation, out NSError outError);

		[Export ("writeToURL:ofType:forSaveOperation:originalContentsURL:error:")]
		bool WriteToUrl (NSUrl url, string typeName, NSSaveOperationType saveOperation, NSUrl absoluteOriginalContentsUrl, out NSError outError);

		[Export ("fileAttributesToWriteToURL:ofType:forSaveOperation:originalContentsURL:error:")]
		NSDictionary FileAttributesToWrite (NSUrl toUrl, string typeName, NSSaveOperationType saveOperation, NSUrl absoluteOriginalContentsUrl, out NSError outError);

		[Export ("keepBackupFile")]
		bool KeepBackupFile ();

		[Export ("saveDocument:")]
		void SaveDocument (NSObject sender);

		[Export ("saveDocumentAs:")]
		void SaveDocumentAs (NSObject sender);

		[Export ("saveDocumentTo:")]
		void SaveDocumentTo (NSObject sender);

		[Export ("saveDocumentWithDelegate:didSaveSelector:contextInfo:")]
		void SaveDocument (NSObject delegateObject, Selector didSaveSelector, IntPtr contextInfo);

		[Mac (10,9)]
		[Export ("saveDocumentToPDF:")]
		void SaveDocumentAsPdf (NSObject sender);

		[Mac (10,9)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("PDFPrintOperation", ArgumentSemantic.Retain)]
		NSPrintOperation PDFPrintOperation { get; }

		[Export ("runModalSavePanelForSaveOperation:delegate:didSaveSelector:contextInfo:")]
		void RunModalSavePanelForSaveOperation (NSSaveOperationType saveOperation, NSObject delegateObject, Selector didSaveSelector, IntPtr contextInfo);

		[Export ("shouldRunSavePanelWithAccessoryView")]
		bool ShouldRunSavePanelWithAccessoryView { get; }

		[Export ("prepareSavePanel:")]
		bool PrepareSavePanel (NSSavePanel savePanel);

		[Export ("fileNameExtensionWasHiddenInLastRunSavePanel")]
		bool FileNameExtensionWasHiddenInLastRunSavePanel { get; }

		[Export ("fileTypeFromLastRunSavePanel")]
		string FileTypeFromLastRunSavePanel { get; }

		[Export ("saveToURL:ofType:forSaveOperation:delegate:didSaveSelector:contextInfo:")]
		void SaveToUrl (NSUrl url, string typeName, NSSaveOperationType saveOperation, NSObject delegateObject, Selector didSaveSelector, IntPtr contextInfo);

		[Export ("saveToURL:ofType:forSaveOperation:error:")]
		bool SaveToUrl (NSUrl url, string typeName, NSSaveOperationType saveOperation, out NSError outError);

		[Export ("hasUnautosavedChanges")]
		bool HasUnautosavedChanges { get; }

		[Export ("autosaveDocumentWithDelegate:didAutosaveSelector:contextInfo:")]
		void AutosaveDocument (NSObject delegateObject, Selector didAutosaveSelector, IntPtr contextInfo);

		[Export ("autosavingFileType")]
		string AutosavingFileType { get; }

		[Export ("canCloseDocumentWithDelegate:shouldCloseSelector:contextInfo:")]
		void CanCloseDocument (NSObject delegateObject, Selector shouldCloseSelector, IntPtr contextInfo);

		[Export ("close")]
		void Close ();

		[Export ("runPageLayout:")]
		void RunPageLayout (NSObject sender);

		[Export ("runModalPageLayoutWithPrintInfo:delegate:didRunSelector:contextInfo:")]
		void RunModalPageLayout (NSPrintInfo printInfo, NSObject delegateObject, Selector didRunSelector, IntPtr contextInfo);

		[Export ("preparePageLayout:")]
		bool PreparePageLayout (NSPageLayout pageLayout);

		[Export ("shouldChangePrintInfo:")]
		bool ShouldChangePrintInfo (NSPrintInfo newPrintInfo);

		[Export ("printDocument:")]
		void PrintDocument (NSObject sender);

		[Export ("printDocumentWithSettings:showPrintPanel:delegate:didPrintSelector:contextInfo:")]
		void PrintDocument (NSDictionary printSettings, bool showPrintPanel, NSObject delegateObject, Selector didPrintSelector, IntPtr contextInfo);

		[Export ("printOperationWithSettings:error:")]
		NSPrintOperation PrintOperation (NSDictionary printSettings, out NSError outError);

		[Export ("runModalPrintOperation:delegate:didRunSelector:contextInfo:")]
		void RunModalPrintOperation (NSPrintOperation printOperation, NSObject delegateObject, Selector didRunSelector, IntPtr contextInfo);

		[Export ("isDocumentEdited")]
		bool IsDocumentEdited { get; }

		[Export ("updateChangeCount:")]
		void UpdateChangeCount (NSDocumentChangeType change);

		[Export ("presentError:modalForWindow:delegate:didPresentSelector:contextInfo:")]
		void PresentError (NSError error, NSWindow window, [NullAllowed] NSObject delegateObject, [NullAllowed] Selector didPresentSelector, IntPtr contextInfo);

		[Export ("presentError:")]
		bool PresentError (NSError error);

		[Export ("willPresentError:")]
		NSError WillPresentError (NSError error);

		[Export ("makeWindowControllers")]
		void MakeWindowControllers ();

		[Export ("windowNibName")]
		string WindowNibName { get; }

		[Export ("windowControllerWillLoadNib:")]
		void WindowControllerWillLoadNib (NSWindowController windowController);

		[Export ("windowControllerDidLoadNib:")]
		void WindowControllerDidLoadNib (NSWindowController windowController);

		[Export ("setWindow:")]
		void SetWindow (NSWindow window);

		[Export ("addWindowController:")]
		[PostGet ("WindowControllers")]
		void AddWindowController (NSWindowController windowController);

		[Export ("removeWindowController:")]
		[PostGet ("WindowControllers")]
		void RemoveWindowController (NSWindowController windowController);

		[Export ("showWindows")]
		void ShowWindows ();

		[Export ("windowControllers")]
		NSWindowController [] WindowControllers { get; }

		[Export ("shouldCloseWindowController:delegate:shouldCloseSelector:contextInfo:")]
		void ShouldCloseWindowController (NSWindowController windowController, NSObject delegateObject, Selector shouldCloseSelector, IntPtr contextInfo);

		[Export ("displayName")]
		string DisplayName { get; [Mac (10, 7)][NullAllowed] set; }

		[Export ("windowForSheet")]
		NSWindow WindowForSheet { get; }

		[Static, Export ("readableTypes", ArgumentSemantic.Copy)]
		string [] ReadableTypes { get; }

		[Static]
		[Export ("writableTypes", ArgumentSemantic.Copy)]
		string [] WritableTypes ();

		[Static]
		[Export ("isNativeType:")]
		bool IsNativeType (string type);

		[Export ("writableTypesForSaveOperation:")]
		string [] WritableTypesForSaveOperation (NSSaveOperationType saveOperation);

		[Export ("fileNameExtensionForType:saveOperation:")]
		string FileNameExtensionForSaveOperation (string typeName, NSSaveOperationType saveOperation);

		[Export ("validateUserInterfaceItem:")]
		bool ValidateUserInterfaceItem (NSObject /* Must implement NSValidatedUserInterfaceItem */ anItem);

		//Detected properties
		[Export ("fileType")]
		string FileType { get; set; }

		[Export ("fileURL", ArgumentSemantic.Copy), NullAllowed]
		NSUrl FileUrl { get; set; }

		[Export ("fileModificationDate", ArgumentSemantic.Copy)]
		NSDate FileModificationDate { get; set; }

		[Export ("autosavedContentsFileURL", ArgumentSemantic.Copy)]
		NSUrl AutosavedContentsFileUrl { get; set; }

		[Export ("printInfo", ArgumentSemantic.Copy)]
		NSPrintInfo PrintInfo { get; set; }

		[Export ("undoManager", ArgumentSemantic.Retain)]
		NSUndoManager UndoManager { get; set; }

		[Export ("hasUndoManager")]
		bool HasUndoManager { get; set; }

		[Mac (10, 7), Export ("performActivityWithSynchronousWaiting:usingBlock:")]
		void PerformActivity (bool waitSynchronously, Action activityCompletionHandler);

		[Mac (10, 7), Export ("continueActivityUsingBlock:")]
		void ContinueActivity (Action resume);

		[Mac (10, 7), Export ("continueAsynchronousWorkOnMainThreadUsingBlock:")]
		void ContinueAsynchronousWorkOnMainThread (Action work);

		[Mac (10, 7), Export ("performSynchronousFileAccessUsingBlock:")]
		void PerformSynchronousFileAccess (Action fileAccessCallback);

		[Mac (10, 7), Export ("performAsynchronousFileAccessUsingBlock:")]
		void PerformAsynchronousFileAccess (Action ioCode);

		[Mac (10, 7), Export ("isEntireFileLoaded")]
		bool IsEntireFileLoaded { get; }

		[Mac (10, 7), Export ("unblockUserInteraction")]
		void UnblockUserInteraction ();

		[Mac (10, 7), Export ("autosavingIsImplicitlyCancellable")]
		bool AutosavingIsImplicitlyCancellable { get; }

		[Mac (10, 7), Export ("saveToURL:ofType:forSaveOperation:completionHandler:")]
		void SaveTo (NSUrl url, string typeName, NSSaveOperationType saveOperation, NSDocumentCompletionHandler completionHandler);

		[Mac (10, 7), Export ("canAsynchronouslyWriteToURL:ofType:forSaveOperation:")]
		bool CanWriteAsynchronously (NSUrl toUrl, string typeName, NSSaveOperationType saveOperation);

		[Mac (10, 7), Export ("checkAutosavingSafetyAndReturnError:")]
		bool CheckAutosavingSafety (out NSError outError);

		[Mac (10, 7), Export ("scheduleAutosaving")]
		void ScheduleAutosaving ();

		[Mac (10, 7), Export ("autosaveWithImplicitCancellability:completionHandler:")]
		void Autosave (bool autosavingIsImplicitlyCancellable, NSDocumentCompletionHandler completionHandler);

		[Static]
		[Mac (10, 7), Export ("autosavesInPlace")]
		bool AutosavesInPlace ();

		[Static]
		[Mac (10, 7), Export ("preservesVersions")]
		bool PreservesVersions ();

		[Mac (10, 7), Export ("duplicateDocument:")]
		void DuplicateDocument (NSObject sender);

		[Mac (10, 7), Export ("duplicateDocumentWithDelegate:didDuplicateSelector:contextInfo:"), Internal]
		void _DuplicateDocument ([NullAllowed] NSObject cbackobject, [NullAllowed] Selector didDuplicateSelector, IntPtr contextInfo);

		[Mac (10, 7), Export ("duplicateAndReturnError:")]
		NSDocument Duplicate (out NSError outError);

		[Mac (10, 7), Export ("isInViewingMode")]
		bool IsInViewingMode { get; }

		[Mac (10, 7), Export ("changeCountTokenForSaveOperation:")]
		NSObject ChangeCountToken (NSSaveOperationType saveOperation);

		[Mac (10, 7), Export ("updateChangeCountWithToken:forSaveOperation:")]
		void UpdateChangeCount (NSObject changeCountToken, NSSaveOperationType saveOperation);

		[Mac (10, 7), Export ("willNotPresentError:")]
		void WillNotPresentError (NSError error);

#if !XAMCORE_2_0
		[Mac (10, 7), Export ("setDisplayName:")]
		[Obsolete ("Use the 'DisplayName' property instead.")]
		[Sealed]
		void SetDisplayName ([NullAllowed] string displayNameOrNull);
#endif

		[Mac (10, 7), Export ("restoreDocumentWindowWithIdentifier:state:completionHandler:")]
		void RestoreDocumentWindow (string identifier, NSCoder state, NSWindowCompletionHandler completionHandler);

		// This one comes from the NSRestorableState category ('@interface NSResponder (NSRestorableState)')
		[Mac (10, 7), Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		// This one comes from the NSRestorableState category ('@interface NSResponder (NSRestorableState)')
		[Export ("restoreStateWithCoder:")]
		void RestoreState (NSCoder coder);

		// This one comes from the NSRestorableState category ('@interface NSResponder (NSRestorableState)')
		[Export ("invalidateRestorableState")]
		void InvalidateRestorableState ();

		// This one comes from the NSRestorableState category ('@interface NSResponder (NSRestorableState)')
		[Static]
		[Export ("restorableStateKeyPaths", ArgumentSemantic.Copy)]
		string [] RestorableStateKeyPaths ();

#if XAMCORE_2_0
		[Mac (10,10, onlyOn64 : true)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("userActivity", ArgumentSemantic.Strong)]
		NSUserActivity UserActivity { get; set; }

		[Mac (10,10, onlyOn64 : true)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity userActivity);

		[Mac (10,10, onlyOn64 : true)]
		[Export ("restoreUserActivityState:")]
		void RestoreUserActivityState (NSUserActivity userActivity);
#endif

		[Mac (10,12)]
		[Export ("isBrowsingVersions")] 
		bool IsBrowsingVersions { get; }


		[Mac (10,12)]
		[Export ("stopBrowsingVersionsWithCompletionHandler:")]
		[Async]
		void StopBrowsingVersions (Action completionHandler);

		[Mac (10, 13)]
		[Export ("allowsDocumentSharing")]
		bool AllowsDocumentSharing { get; }

		[Mac (10,13)]
		[Export ("shareDocumentWithSharingService:completionHandler:")]
		[Async]
		void ShareDocument (NSSharingService sharingService, [NullAllowed] Action<bool> completionHandler);

		[Mac (10,13)]
		[Export ("prepareSharingServicePicker:")]
		void Prepare (NSSharingServicePicker sharingServicePicker);
	}

	delegate void OpenDocumentCompletionHandler (NSDocument document, bool documentWasAlreadyOpen, NSError error);

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSDocumentController : NSWindowRestoration, NSCoding {
		[Static, Export ("sharedDocumentController")]
		NSDocumentController SharedDocumentController { get; }

		[Export ("documents")]
		NSDocument [] Documents { get; }

		[Export ("currentDocument")]
		NSDocument CurrentDocument { get; }

		[Export ("currentDirectory")]
		string CurrentDirectory { get; }

		[Export ("documentForURL:")]
		NSDocument DocumentForUrl (NSUrl url);

		[Export ("documentForWindow:")]
		NSDocument DocumentForWindow (NSWindow window);

		[Export ("addDocument:")]
		[PostGet ("Documents")]
		void AddDocument (NSDocument document);

		[Export ("removeDocument:")]
		[PostGet ("Documents")]
		void RemoveDocument (NSDocument document);

		[Export ("newDocument:")]
		void NewDocument ([NullAllowed] NSObject sender);

		[Export ("openUntitledDocumentAndDisplay:error:")]
		NSObject OpenUntitledDocument (bool displayDocument, out NSError outError);

		[Export ("makeUntitledDocumentOfType:error:")]
		NSObject MakeUntitledDocument (string typeName, out NSError error);

		[Export ("openDocument:")]
		void OpenDocument ([NullAllowed] NSObject sender);

		[Export ("URLsFromRunningOpenPanel")]
		NSUrl [] UrlsFromRunningOpenPanel ();

		[Export ("runModalOpenPanel:forTypes:")]
		nint RunModalOpenPanel (NSOpenPanel openPanel, string [] types);

		[Export ("openDocumentWithContentsOfURL:display:error:")]
		NSObject OpenDocument (NSUrl url, bool displayDocument, out NSError outError);

		[Mac (10, 7)]
		[Export ("openDocumentWithContentsOfURL:display:completionHandler:")]
		void OpenDocument (NSUrl url, bool display, OpenDocumentCompletionHandler completionHandler);

		[Export ("makeDocumentWithContentsOfURL:ofType:error:")]
		NSObject MakeDocument (NSUrl url, string typeName, out NSError outError);

		[Export ("reopenDocumentForURL:withContentsOfURL:error:")]
		bool ReopenDocument (NSUrl url, NSUrl contentsUrl, out NSError outError);

		[Export ("makeDocumentForURL:withContentsOfURL:ofType:error:")]
		NSObject MakeDocument ([NullAllowed] NSUrl urlOrNil, NSUrl contentsUrl, string typeName, out NSError outError);

		[Export ("saveAllDocuments:")]
		void SaveAllDocuments ([NullAllowed] NSObject sender);

		[Export ("hasEditedDocuments")]
		bool HasEditedDocuments { get; }

		[Export ("reviewUnsavedDocumentsWithAlertTitle:cancellable:delegate:didReviewAllSelector:contextInfo:")]
		void ReviewUnsavedDocuments (string title, bool cancellable, NSObject delegateObject, Selector didReviewAllSelector, IntPtr contextInfo);

		[Export ("closeAllDocumentsWithDelegate:didCloseAllSelector:contextInfo:")]
		void CloseAllDocuments (NSObject delegateObject, Selector didCloseAllSelector, IntPtr contextInfo);

		[Export ("presentError:modalForWindow:delegate:didPresentSelector:contextInfo:")]
		void PresentError (NSError error, NSWindow window, [NullAllowed] NSObject delegateObject, [NullAllowed] Selector didPresentSelector, IntPtr contextInfo);

		[Export ("presentError:")]
		bool PresentError (NSError error);

		[Export ("willPresentError:")]
		NSError WillPresentError (NSError error);

		[Export ("maximumRecentDocumentCount"), ThreadSafe]
		nint MaximumRecentDocumentCount { get; }

		[Export ("clearRecentDocuments:")]
		void ClearRecentDocuments ([NullAllowed] NSObject sender);

		[Export ("noteNewRecentDocument:")]
		void NoteNewRecentDocument (NSDocument document);

		[Export ("noteNewRecentDocumentURL:")]
		void NoteNewRecentDocumentURL (NSUrl url);

		[Export ("recentDocumentURLs")]
		NSUrl [] RecentDocumentUrls { get; }

		[Export ("defaultType")]
		string DefaultType { get; }

		[Export ("typeForContentsOfURL:error:")]
		string TypeForUrl (NSUrl url, out NSError outError);

		[Export ("documentClassNames")]
		string [] DocumentClassNames  {get; }

		[Export ("documentClassForType:")]
		Class DocumentClassForType (string typeName);

		[Export ("displayNameForType:")]
		string DisplayNameForType (string typeName);

		[Export ("validateUserInterfaceItem:")]
		bool ValidateUserInterfaceItem (NSObject /* must implement NSValidatedUserInterfaceItem */ anItem);

		//Detected properties
		[Export ("autosavingDelay")]
		double AutosavingDelay { get; set; }
	}

	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	interface NSDraggingImageComponent {
		[Export ("key", ArgumentSemantic.Copy)]
		string Key { get; set;  }

		[Export ("contents", ArgumentSemantic.Strong)]
		NSObject Contents { get; set;  }

		[Export ("frame")]
		CGRect Frame { get; set;  }

		[Static]
		[Export ("draggingImageComponentWithKey:")]
		NSDraggingImageComponent FromKey (string key);

		[Export ("initWithKey:")]
		[DesignatedInitializer]
		IntPtr Constructor (string key);

		[Field ("NSDraggingImageComponentIconKey")]
		NSString IconKey { get; }

		[Field ("NSDraggingImageComponentLabelKey")]
		NSString LabelKey { get; }
	}

	delegate NSDraggingImageComponent [] NSDraggingItemImagesContentProvider ();
	
	[BaseType (typeof (NSObject))]
	interface NSDraggingItem {
		[Export ("item", ArgumentSemantic.Strong)]
		NSObject Item { get;  }

		[Export ("draggingFrame")]
		CGRect DraggingFrame { get; set;  }

		[Export ("imageComponents", ArgumentSemantic.Copy)]
		NSDraggingImageComponent [] ImageComponents { get;  }

		[Export ("initWithPasteboardWriter:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSPasteboardWriting pasteboardWriter);

		[Export ("setImageComponentsProvider:")]
		void SetImagesContentProvider ([NullAllowed] NSDraggingItemImagesContentProvider provider);

		[Export ("setDraggingFrame:contents:")]
		void SetDraggingFrame (CGRect frame, NSObject contents);

	}
	
#if !XAMCORE_4_0
	[BaseType (typeof (NSObject))]
#endif
	[Protocol] // Apple docs say: "you never need to create a class that implements the NSDraggingInfo protocol.", so don't add [Model]
	interface NSDraggingInfo  {
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingDestinationWindow")]
		NSWindow DraggingDestinationWindow { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingSourceOperationMask")]
		NSDragOperation DraggingSourceOperationMask { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingLocation")]
		CGPoint DraggingLocation { get; }
	
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggedImageLocation")]
		CGPoint DraggedImageLocation { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggedImage")]
		NSImage DraggedImage { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingPasteboard")]
		NSPasteboard DraggingPasteboard { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingSource")]
		NSObject DraggingSource { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("draggingSequenceNumber")]
		nint DraggingSequenceNumber { get; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("slideDraggedImageTo:")]
		void SlideDraggedImageTo (CGPoint screenPoint);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use NSFilePromiseProvider objects instead.")]
		[Export ("namesOfPromisedFilesDroppedAtDestination:")]
		string [] PromisedFilesDroppedAtDestination (NSUrl dropDestination);

#if XAMCORE_4_0
		[Abstract]
#endif
		[Mac (10, 7)]
		[Export ("animatesToDestination")]
		bool AnimatesToDestination { get; set; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Mac (10, 7)]
		[Export ("numberOfValidItemsForDrop")]
		nint NumberOfValidItemsForDrop { get; set; }

#if XAMCORE_4_0
		[Abstract]
#endif
		[Mac (10, 7)]
		[Export ("draggingFormation")]
		NSDraggingFormation DraggingFormation { get; set; } 

		[Mac (10, 7)]
		[Internal]
		[Export ("enumerateDraggingItemsWithOptions:forView:classes:searchOptions:usingBlock:")]
		void EnumerateDraggingItems (NSDraggingItemEnumerationOptions enumOpts, NSView view, IntPtr classArray,
					     NSDictionary searchOptions, NSDraggingEnumerator enumerator);

		[Mac (10,11)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("springLoadingHighlight")]
		NSSpringLoadingHighlight SpringLoadingHighlight { get; }

		[Mac (10,11)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("resetSpringLoading")]
		void ResetSpringLoading ();
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSDraggingDestination {
		[Export ("draggingEntered:"), DefaultValue (NSDragOperation.None)]
		NSDragOperation DraggingEntered ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("draggingUpdated:"), DefaultValue (NSDragOperation.None)]
		NSDragOperation DraggingUpdated ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("draggingExited:")]
		void DraggingExited ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("prepareForDragOperation:"), DefaultValue (false)]
		bool PrepareForDragOperation ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("performDragOperation:"), DefaultValue (false)]
		bool PerformDragOperation ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("concludeDragOperation:")]
		void ConcludeDragOperation ([Protocolize (4)] NSDraggingInfo sender);

		[Export ("draggingEnded:")]
		void DraggingEnded ([Protocolize (4)] NSDraggingInfo sender);

		[DebuggerBrowsableAttribute (DebuggerBrowsableState.Never)]
		[Export ("wantsPeriodicDraggingUpdates"), DefaultValue (true)]
		bool WantsPeriodicDraggingUpdates { get; }
	}

	delegate void NSDraggingEnumerator (NSDraggingItem draggingItem, nint idx, ref bool stop);

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // warning on dispose - created using NSView.BeginDraggingSession
	interface NSDraggingSession {
		[Export ("draggingFormation")]
		NSDraggingFormation DraggingFormation { get; set;  }

		[Export ("animatesToStartingPositionsOnCancelOrFail")]
		bool AnimatesToStartingPositionsOnCancelOrFail { get; set;  }

		[Export ("draggingLeaderIndex")]
		nint DraggingLeaderIndex { get; set;  }

		[Export ("draggingPasteboard")]
		NSPasteboard DraggingPasteboard { get;  }

		[Export ("draggingSequenceNumber")]
		nint DraggingSequenceNumber { get;  }

		[Export ("draggingLocation")]
		CGPoint DraggingLocation { get;  }

		[Internal]
		[Export ("enumerateDraggingItemsWithOptions:forView:classes:searchOptions:usingBlock:")]
		void EnumerateDraggingItems (NSDraggingItemEnumerationOptions enumOpts, NSView view, IntPtr classArray, [NullAllowed] NSDictionary searchOptions, NSDraggingEnumerator enumerator);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSDraggingSource {
		[Export ("draggingSourceOperationMaskForLocal:"), DefaultValue (NSDragOperation.None)]
		NSDragOperation DraggingSourceOperationMaskForLocal (bool flag);

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use NSFilePromiseProvider objects instead.")]
		[Export ("namesOfPromisedFilesDroppedAtDestination:"), DefaultValue (new string[0])]
		string [] NamesOfPromisedFilesDroppedAtDestination (NSUrl dropDestination);

		[Export ("draggedImage:beganAt:")]
		void DraggedImageBeganAt (NSImage image, CGPoint screenPoint);

		[Export ("draggedImage:endedAt:operation:")]
		void DraggedImageEndedAtOperation (NSImage image, CGPoint screenPoint, NSDragOperation operation);

		[Export ("draggedImage:movedTo:")]
		void DraggedImageMovedTo (NSImage image, CGPoint screenPoint);

		[Export ("ignoreModifierKeysWhileDragging"), DefaultValue (false)]
		bool IgnoreModifierKeysWhileDragging { get; }

		[Availability (Deprecated = Platform.Mac_10_1, Message = "Use DraggedImageEndedAtOperation instead.")]
		[Export ("draggedImage:endedAt:deposited:")]
		void DraggedImageEndedAtDeposited (NSImage image, CGPoint screenPoint, bool deposited);
	}
	
	[BaseType (typeof (NSResponder), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSDrawerDelegate)})]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
	partial interface NSDrawer : NSAccessibilityElementProtocol, NSAccessibility {
		[Export ("initWithContentSize:preferredEdge:")]
		IntPtr Constructor (CGSize contentSize, NSRectEdge edge);

		[Export ("parentWindow")]
		NSWindow ParentWindow { get; set; }

		[Export ("contentView", ArgumentSemantic.Retain)]
		NSView ContentView { get; set; }

		[Export ("preferredEdge")]
		NSRectEdge PreferredEdge { get; set; }
		
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSDrawerDelegate Delegate { get; set; }

		[Export ("open")]
		void Open ();

		[Export ("openOnEdge:")]
		void OpenOnEdge (NSRectEdge edge);

		[Export ("close")]
		void Close ();

		[Export ("open:")]
		void Open (NSObject sender);

		[Export ("close:")]
		void Close (NSObject sender);

		[Export ("toggle:")]
		void Toggle (NSObject sender);

		[Export ("state")]
		NSDrawerState State { get; }

		[Export ("edge")]
		NSRectEdge Edge { get; }

		[Export ("contentSize")]
		CGSize ContentSize { get; set; }

		[Export ("minContentSize")]
		CGSize MinContentSize { get; set; }

		[Export ("maxContentSize")]
		CGSize MaxContentSize { get; set; }

		[Export ("leadingOffset")]
		nfloat LeadingOffset { get; set; }

		[Export ("trailingOffset")]
		nfloat TrailingOffset { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
	interface NSDrawerDelegate {
		[Export ("drawerDidClose:"), EventArgs ("NSNotification")]
		void DrawerDidClose (NSNotification notification);
		
		[Export ("drawerDidOpen:"), EventArgs ("NSNotification")]
		void DrawerDidOpen (NSNotification notification);

		[Export ("drawerShouldClose:"), DelegateName ("DrawerShouldCloseDelegate"), DefaultValue (true)]
		bool DrawerShouldClose (NSDrawer sender);

		[Export ("drawerShouldOpen:"), DelegateName ("DrawerShouldOpenDelegate"), DefaultValue (true)]
		bool DrawerShouldOpen (NSDrawer sender);
	
		[Export ("drawerWillClose:"), EventArgs ("NSNotification")]
		void DrawerWillClose (NSNotification notification);
	
		[Export ("drawerWillOpen:"), EventArgs ("NSNotification")]
		void DrawerWillOpen (NSNotification notification);

		[Export ("drawerWillResizeContents:toSize:"), DelegateName ("DrawerWillResizeContentsDelegate"), DefaultValue (null)]
		CGSize DrawerWillResizeContents (NSDrawer sender, CGSize toSize);

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash at runtime (e.g. description). Documentation state: "You don’t create NSFont objects using the alloc and init methods."
	partial interface NSFont : NSSecureCoding, NSCopying {
		[Static]
		[Export ("fontWithName:size:")]
		NSFont FromFontName (string fontName, nfloat fontSize);

		//[Static]
		//[Export ("fontWithName:matrix:")]
		//NSFont FromFontName (string fontName, float [] fontMatrix);

		[Static]
		[Export ("fontWithDescriptor:size:")]
		NSFont FromDescription (NSFontDescriptor fontDescriptor, nfloat fontSize);

		[Static]
		[Export ("fontWithDescriptor:textTransform:")]
		NSFont FromDescription (NSFontDescriptor fontDescriptor, [NullAllowed] NSAffineTransform textTransform);

		[Static]
		[Export ("userFontOfSize:")]
		NSFont UserFontOfSize (nfloat fontSize);

		[Static]
		[Export ("userFixedPitchFontOfSize:")]
		NSFont UserFixedPitchFontOfSize (nfloat fontSize);

		[Static]
		[Export ("setUserFont:")]
		void SetUserFont ([NullAllowed] NSFont aFont);

		[Static]
		[Export ("setUserFixedPitchFont:")]
		void SetUserFixedPitchFont ([NullAllowed] NSFont aFont);

		[Static]
		[Export ("systemFontOfSize:")]
		NSFont SystemFontOfSize (nfloat fontSize);

		[Static]
		[Export ("boldSystemFontOfSize:")]
		NSFont BoldSystemFontOfSize (nfloat fontSize);

		[Static]
		[Export ("labelFontOfSize:")]
		NSFont LabelFontOfSize (nfloat fontSize);

		[Static]
		[Export ("titleBarFontOfSize:")]
		NSFont TitleBarFontOfSize (nfloat fontSize);

		[Static]
		[Export ("menuFontOfSize:")]
		NSFont MenuFontOfSize (nfloat fontSize);

		[Static]
		[Export("menuBarFontOfSize:")]
		NSFont MenuBarFontOfSize (nfloat fontSize);

		[Static]
		[Export("messageFontOfSize:")]
		NSFont MessageFontOfSize (nfloat fontSize);

		[Static]
		[Export ("paletteFontOfSize:")]
		NSFont PaletteFontOfSize (nfloat fontSize);

		[Static]
		[Export ("toolTipsFontOfSize:")]
		NSFont ToolTipsFontOfSize (nfloat fontSize);

		[Static]
		[Export ("controlContentFontOfSize:")]
		NSFont ControlContentFontOfSize (nfloat fontSize);

		[Static]
		[Export ("systemFontSize")]
		nfloat SystemFontSize { get; }

		[Static]
		[Export ("smallSystemFontSize")]
		nfloat SmallSystemFontSize { get; }

		[Static]
		[Export ("labelFontSize")]
		nfloat LabelFontSize { get; }

		[Static]
		[Export ("systemFontSizeForControlSize:")]
		nfloat SystemFontSizeForControlSize (NSControlSize controlSize);

		[Export ("fontName")]
		string FontName { get; }

		[Export ("pointSize")]
		nfloat PointSize { get; }

		//[Export ("matrix")]
		//  FIXME
		//IntPtr *float Matrix { get; }

		[Export ("familyName")]
		string FamilyName { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("fontDescriptor")]
		NSFontDescriptor FontDescriptor { get; }

		[Export ("textTransform")]
		NSAffineTransform TextTransform { get; }

		[Export ("numberOfGlyphs")]
		nint GlyphCount { get; }

		[Export ("mostCompatibleStringEncoding")]
		NSStringEncoding MostCompatibleStringEncoding { get; }

		[Export ("glyphWithName:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use the 'CGGlyph' APIs instead.")]
		uint GlyphWithName (string aName); /* NSGlyph = unsigned int */

		[Export ("coveredCharacterSet")]
		NSCharacterSet CoveredCharacterSet { get; }

		[Export ("boundingRectForFont")]
		CGRect BoundingRectForFont { get; }

		[Export ("maximumAdvancement")]
		CGSize MaximumAdvancement { get; }

		[Export ("ascender")]
		nfloat Ascender { get; }

		[Export ("descender")]
		nfloat Descender { get; }

		[Export ("leading")]
		nfloat Leading { get; }

		[Export ("underlinePosition")]
		nfloat UnderlinePosition { get; }

		[Export ("underlineThickness")]
		nfloat UnderlineThickness { get; }

		[Export ("italicAngle")]
		nfloat ItalicAngle { get; }

		[Export ("capHeight")]
		nfloat CapHeight { get; }

		[Export ("xHeight")]
		nfloat XHeight { get; }

		[Export ("isFixedPitch")]
		bool IsFixedPitch { get; }

		[Export ("boundingRectForGlyph:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use the 'CGGlyph' APIs instead.")]
		CGRect BoundingRectForGlyph (uint /* NSGlyph = unsigned int */ aGlyph);

		[Export ("advancementForGlyph:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use the 'CGGlyph' APIs instead.")]
		CGSize AdvancementForGlyph (uint /* NSGlyph = unsigned int */ aGlyph);

		[Export ("set")]
		void Set ();

		[Export ("setInContext:")]
		void SetInContext (NSGraphicsContext graphicsContext);

		[Export ("printerFont")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		NSFont PrinterFont { get; }

		[Export ("screenFont")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		NSFont ScreenFont { get; }

		[Export ("screenFontWithRenderingMode:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		NSFont ScreenFontWithRenderingMode (NSFontRenderingMode renderingMode);

		[Export ("renderingMode")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		NSFontRenderingMode RenderingMode { get; }

		[Export ("isVertical")]
		bool IsVertical { get; }

		//
		// Not a property because this causes the creation of a new font on request in the specified configuration.
		//
		[Export ("verticalFont")]
		NSFont GetVerticalFont ();

		[Field ("NSFontFamilyAttribute")]
		NSString FamilyAttribute { get; }

		[Field ("NSFontNameAttribute")]
		NSString NameAttribute { get; }

		[Field ("NSFontFaceAttribute")]
		NSString FaceAttribute { get; }

		[Field ("NSFontSizeAttribute")]
		NSString SizeAttribute { get; }

		[Field ("NSFontVisibleNameAttribute")]
		NSString VisibleNameAttribute { get; }

		[Field ("NSFontMatrixAttribute")]
		NSString MatrixAttribute { get; }

		[Field ("NSFontVariationAttribute")]
		NSString VariationAttribute { get; }

		[Field ("NSFontCharacterSetAttribute")]
		NSString CharacterSetAttribute { get; }

		[Field ("NSFontCascadeListAttribute")]
		NSString CascadeListAttribute { get; }

		[Field ("NSFontTraitsAttribute")]
		NSString TraitsAttribute { get; }

		[Field ("NSFontFixedAdvanceAttribute")]
		NSString FixedAdvanceAttribute { get; }

		[Field ("NSFontFeatureSettingsAttribute")]
		NSString FeatureSettingsAttribute { get; }

		[Field ("NSFontSymbolicTrait")]
		NSString SymbolicTrait { get; }

		[Field ("NSFontWeightTrait")]
		NSString WeightTrait { get; }

		[Field ("NSFontWidthTrait")]
		NSString WidthTrait { get; }

		[Field ("NSFontSlantTrait")]
		NSString SlantTrait { get; }

		[Field ("NSFontVariationAxisIdentifierKey")]
		NSString VariationAxisIdentifierKey { get; }

		[Field ("NSFontVariationAxisMinimumValueKey")]
		NSString VariationAxisMinimumValueKey { get; }

		[Field ("NSFontVariationAxisMaximumValueKey")]
		NSString VariationAxisMaximumValueKey { get; }

		[Field ("NSFontVariationAxisDefaultValueKey")]
		NSString VariationAxisDefaultValueKey { get; }

		[Field ("NSFontVariationAxisNameKey")]
		NSString VariationAxisNameKey { get; }

		[Field ("NSFontFeatureTypeIdentifierKey")]
		NSString FeatureTypeIdentifierKey { get; }

		[Field ("NSFontFeatureSelectorIdentifierKey")]
		NSString FeatureSelectorIdentifierKey { get; }

		[Mac (10,11)]
		[Static]
		[Export ("systemFontOfSize:weight:")]
		NSFont SystemFontOfSize (nfloat fontSize, nfloat weight);

		[Mac (10,11)]
		[Static]
		[Export ("monospacedDigitSystemFontOfSize:weight:")]
		NSFont MonospacedDigitSystemFontOfSize (nfloat fontSize, nfloat weight);

		[Mac (10,13)]
		[Export ("boundingRectForCGGlyph:")]
		CGRect GetBoundingRect (CGGlyph glyph);

		[Mac (10,13)]
		[Export ("advancementForCGGlyph:")]
		CGSize GetAdvancement (CGGlyph glyph);

		[Mac (10,13)]
		[Internal]
		[Export ("getBoundingRects:forCGGlyphs:count:")]
		void _GetBoundingRects (IntPtr bounds, IntPtr glyphs, nuint glyphCount);

		[Mac (10,13)]
		[Internal]
		[Export ("getAdvancements:forCGGlyphs:count:")]
		void _GetAdvancements (IntPtr advancements, IntPtr glyphs, nuint glyphCount);
	}

	[Mac (10, 7)]
	interface NSFontCollectionChangedEventArgs {
		[Internal, Export ("NSFontCollectionActionKey")]
		NSString _Action { get; }

		[Export ("NSFontCollectionNameKey")]
		string Name { get; }

		[Export ("NSFontCollectionOldNameKey")]
		string OldName { get; }

		[Internal, Export ("NSFontCollectionVisibilityKey")]
		NSNumber _Visibility { get; }
	}

	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	interface NSFontCollection : NSSecureCoding, NSMutableCopying {
		[Static]
		[Export ("fontCollectionWithDescriptors:")]
		NSFontCollection FromDescriptors (NSFontDescriptor [] queryDescriptors);

		[Static]
		[Export ("fontCollectionWithAllAvailableDescriptors", ArgumentSemantic.Copy)]
		NSFontCollection GetAllAvailableFonts ();

		[Static]
		[Export ("fontCollectionWithLocale:")]
		NSFontCollection FromLocale (NSLocale locale);

		[Static]
		[Export ("showFontCollection:withName:visibility:error:")]
		bool ShowFontCollection (NSFontCollection fontCollection, string name, NSFontCollectionVisibility visibility, out NSError error);

		[Static]
		[Export ("hideFontCollectionWithName:visibility:error:")]
		bool HideFontCollection (string name, NSFontCollectionVisibility visibility, out NSError error);

		[Static]
		[Export ("renameFontCollectionWithName:visibility:toName:error:")]
		bool RenameFontCollection (string fromName, NSFontCollectionVisibility visibility, string toName, out NSError error);

		[Static]
		[Export ("allFontCollectionNames", ArgumentSemantic.Copy)]
		string [] AllFontCollectionNames { get; }

		[Static]
		[Export ("fontCollectionWithName:")]
		NSFontCollection FromName (string name);

		[Static]
		[Export ("fontCollectionWithName:visibility:")]
		NSFontCollection FromName (string name, NSFontCollectionVisibility visibility);

		[Export ("queryDescriptors")]
		NSFontDescriptor [] GetQueryDescriptors ();

		[Export ("exclusionDescriptors")]
		NSFontDescriptor [] GetExclusionDescriptors ();

		[Export ("matchingDescriptors")]
		NSFontDescriptor [] GetMatchingDescriptors ();

		[Export ("matchingDescriptorsWithOptions:")]
		NSFontDescriptor [] GetMatchingDescriptors (NSDictionary options);

		[Export ("matchingDescriptorsForFamily:")]
		NSFontDescriptor [] GetMatchingDescriptors (string family);

		[Export ("matchingDescriptorsForFamily:options:")]
		NSFontDescriptor [] GetMatchingDescriptors (string family, NSDictionary options);

		[Field ("NSFontCollectionIncludeDisabledFontsOption")]
		NSString IncludeDisabledFontsOption { get; }
		
		[Field ("NSFontCollectionRemoveDuplicatesOption")]
		NSString RemoveDuplicatesOption { get; }
		
		[Field ("NSFontCollectionDisallowAutoActivationOption")]
		NSString DisallowAutoActivationOption { get; }
		
		[Notification (typeof (NSFontCollectionChangedEventArgs)), Field ("NSFontCollectionDidChangeNotification")]
		NSString ChangedNotification { get; }
		
		[Field ("NSFontCollectionActionKey")]
		NSString ActionKey { get; }
		
		[Field ("NSFontCollectionNameKey")]
		NSString NameKey { get; }
		
		[Field ("NSFontCollectionOldNameKey")]
		NSString OldNameKey { get; }
		
		[Field ("NSFontCollectionVisibilityKey")]
		NSString VisibilityKey { get; }
		
		[Field ("NSFontCollectionWasShown")]
		NSString ActionWasShown { get; }
		
		[Field ("NSFontCollectionWasHidden")]
		NSString ActionWasHidden { get; }
		
		[Field ("NSFontCollectionWasRenamed")]
		NSString ActionWasRenamed { get; }
		
		[Field ("NSFontCollectionAllFonts")]
		NSString NameAllFonts { get; }
		
		[Field ("NSFontCollectionUser")]
		NSString NameUser { get; }
		
		[Field ("NSFontCollectionFavorites")]
		NSString NameFavorites { get; }
		
		[Field ("NSFontCollectionRecentlyUsed")]
		NSString NameRecentlyUsed { get; }
		
	}

	[Mac (10, 7)]
	[BaseType (typeof (NSFontCollection))]
	[DisableDefaultCtor]
	interface NSMutableFontCollection {
		[Export ("setQueryDescriptors:")]
		void SetQueryDescriptors (NSFontDescriptor [] descriptors);

		[Export ("setExclusionDescriptors:")]
		void SetExclusionDescriptors (NSFontDescriptor [] descriptors);

		[Export ("addQueryForDescriptors:")]
		void AddQueryForDescriptors (NSFontDescriptor [] descriptors);

		[Export ("removeQueryForDescriptors:")]
		void RemoveQueryForDescriptors (NSFontDescriptor [] descriptors);

		[Mac(10,10)]
		[Static]
		[Export ("fontCollectionWithDescriptors:")]
		NSMutableFontCollection FromDescriptors (NSFontDescriptor [] queryDescriptors);

		[Mac(10,10)]
		[Static]
		[Export ("fontCollectionWithAllAvailableDescriptors", ArgumentSemantic.Copy)]
		NSMutableFontCollection GetAllAvailableFonts ();

		[Mac(10,10)]
		[Static]
		[Export ("fontCollectionWithLocale:")]
		NSMutableFontCollection FromLocale (NSLocale locale);

		[Mac(10,10)]
		[Static]
		[Export ("fontCollectionWithName:")]
		NSMutableFontCollection FromName (string name);

		[Mac(10,10)]
		[Static]
		[Export ("fontCollectionWithName:visibility:")]
		NSMutableFontCollection FromName (string name, NSFontCollectionVisibility visibility);
	}	

	[BaseType (typeof (NSObject))]
	interface NSFontDescriptor : NSSecureCoding, NSCopying {
		[Export ("postscriptName")]
		string PostscriptName { get; }

		[Export ("pointSize")]
		nfloat PointSize { get; }

		[Export ("matrix")]
		NSAffineTransform Matrix { get; }

		[Export ("symbolicTraits")]
		NSFontSymbolicTraits SymbolicTraits { get; }

		[Export ("objectForKey:")]
		NSObject ObjectForKey (string key);

		[Export ("fontAttributes")]
		NSDictionary FontAttributes { get; }

		[Static]
		[Export ("fontDescriptorWithFontAttributes:")]
		NSFontDescriptor FromAttributes (NSDictionary attributes);

		[Static]
		[Export ("fontDescriptorWithName:size:")]
		NSFontDescriptor FromNameSize (string fontName, nfloat size);

		[Static]
		[Export ("fontDescriptorWithName:matrix:")]
		NSFontDescriptor FromNameMatrix (string fontName, NSAffineTransform matrix);

		[Export ("initWithFontAttributes:")]
		IntPtr Constructor ([NullAllowed] NSDictionary attributes);

		[Export ("matchingFontDescriptorsWithMandatoryKeys:")]
		NSFontDescriptor [] MatchingFontDescriptors (NSSet mandatoryKeys);

		[Export ("matchingFontDescriptorWithMandatoryKeys:")]
		NSFontDescriptor MatchingFontDescriptorWithMandatoryKeys (NSSet mandatoryKeys);

		[Export ("fontDescriptorByAddingAttributes:")]
		NSFontDescriptor FontDescriptorByAddingAttributes (NSDictionary attributes);

		[Export ("fontDescriptorWithSymbolicTraits:")]
		NSFontDescriptor FontDescriptorWithSymbolicTraits (NSFontSymbolicTraits symbolicTraits);

		[Export ("fontDescriptorWithSize:")]
		NSFontDescriptor FontDescriptorWithSize (nfloat newPointSize);

		[Export ("fontDescriptorWithMatrix:")]
		NSFontDescriptor FontDescriptorWithMatrix (NSAffineTransform matrix);

		[Export ("fontDescriptorWithFace:")]
		NSFontDescriptor FontDescriptorWithFace (string newFace);

		[Export ("fontDescriptorWithFamily:")]
		NSFontDescriptor FontDescriptorWithFamily (string newFamily);

		[Mac (10, 13)]
		[Export ("requiresFontAssetRequest")]
		bool RequiresFontAssetRequest { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSFontManager {
		[Static, Export ("setFontPanelFactory:")]
		void SetFontPanelFactory (Class factoryId);

		[Static, Export ("setFontManagerFactory:")]
		void SetFontManagerFactory (Class factoryId);

		[Static, Export ("sharedFontManager")]
		NSFontManager SharedFontManager { get; }

		[Export ("isMultiple")]
		bool IsMultiple { get; }

		[Export ("selectedFont")]
		NSFont SelectedFont { get; }

		[Export ("setSelectedFont:isMultiple:")]
		void SetSelectedFont (NSFont fontObj, bool isMultiple);

		[Export ("setFontMenu:")]
		void SetFontMenu (NSMenu newMenu);

		[Export ("fontMenu:")]
		NSMenu FontMenu (bool create);

		[Export ("fontPanel:")]
		NSFontPanel FontPanel (bool create);

		[Export ("fontWithFamily:traits:weight:size:")]
		NSFont FontWithFamily (string family, NSFontTraitMask traits, nint weight, nfloat size);

		[Export ("traitsOfFont:")]
		NSFontTraitMask TraitsOfFont (NSFont fontObj);

		[Export ("weightOfFont:")]
		nint WeightOfFont (NSFont fontObj);

		[Export ("availableFonts")]
		string [] AvailableFonts { get; }

		[Export ("availableFontFamilies")]
		string [] AvailableFontFamilies { get; }

		[Export ("availableMembersOfFontFamily:")]
		NSArray [] AvailableMembersOfFontFamily (string fam);

		[Export ("convertFont:")]
		NSFont ConvertFont (NSFont fontObj);

		[Export ("convertFont:toSize:")]
		NSFont ConvertFont (NSFont fontObj, nfloat size);

		[Export ("convertFont:toFace:")]
		NSFont ConvertFont (NSFont fontObj, string typeface);

		[Export ("convertFont:toFamily:")]
		NSFont ConvertFontToFamily (NSFont fontObj, string family);

		[Export ("convertFont:toHaveTrait:")]
		NSFont ConvertFont (NSFont fontObj, NSFontTraitMask trait);

		[Export ("convertFont:toNotHaveTrait:")]
		NSFont ConvertFontToNotHaveTrait (NSFont fontObj, NSFontTraitMask trait);

		[Export ("convertWeight:ofFont:")]
		NSFont ConvertWeight (bool increaseWeight, NSFont fontObj);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("action"), NullAllowed]
		Selector Action { get; set; }

		[Export ("sendAction")]
		bool SendAction { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; } 

		[Export ("localizedNameForFamily:face:")]
		string LocalizedNameForFamily (string family, string faceKey);

		[Export ("setSelectedAttributes:isMultiple:")]
		void SetSelectedAttributes (NSDictionary attributes, bool isMultiple);

		[Export ("convertAttributes:")]
		NSDictionary ConvertAttributes (NSDictionary attributes);

		[Export ("availableFontNamesMatchingFontDescriptor:")]
		string [] AvailableFontNamesMatchingFontDescriptor (NSFontDescriptor descriptor);

		[Export ("collectionNames")]
		string [] CollectionNames { get; }

		[Export ("fontDescriptorsInCollection:")]
		NSArray FontDescriptorsInCollection (string collectionNames);

		[Export ("addCollection:options:")]
		bool AddCollection (string collectionName, NSFontCollectionOptions collectionOptions);

		[Export ("removeCollection:")]
		bool RemoveCollection (string collectionName);

		[Export ("addFontDescriptors:toCollection:")]
		void AddFontDescriptors (NSFontDescriptor [] descriptors, string collectionName);

		[Export ("removeFontDescriptor:fromCollection:")]
		void RemoveFontDescriptor (NSFontDescriptor descriptor, string collection);

		[Export ("currentFontAction")]
		nint CurrentFontAction { get; }

		[Export ("convertFontTraits:")]
		NSFontTraitMask ConvertFontTraits (NSFontTraitMask traits);

		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }

		[Export ("fontNamed:hasTraits:")]
		bool FontNamedHasTraits (string fName, NSFontTraitMask someTraits);

		[Export ("availableFontNamesWithTraits:")]
		string [] AvailableFontNamesWithTraits (NSFontTraitMask someTraits);

		[Export ("addFontTrait:")]
		void AddFontTrait (NSObject sender);

		[Export ("removeFontTrait:")]
		void RemoveFontTrait (NSObject sender);

		[Export ("modifyFontViaPanel:")]
		void ModifyFontViaPanel (NSObject sender);

		[Export ("modifyFont:")]
		void ModifyFont (NSObject sender);

		[Export ("orderFrontFontPanel:")]
		void OrderFrontFontPanel (NSObject sender);

		[Export ("orderFrontStylesPanel:")]
		void OrderFrontStylesPanel (NSObject sender);
	}

	[BaseType (typeof (NSPanel))]
	interface NSFontPanel {
		[Static]
		[Export ("sharedFontPanel")]
		NSFontPanel SharedFontPanel { get; }

		[Static]
		[Export ("sharedFontPanelExists")]
		bool SharedFontPanelExists { get; }

		[Export ("setPanelFont:isMultiple:")]
		void SetPanelFont (NSFont fontObj, bool isMultiple);

		[Export ("panelConvertFont:")]
		NSFont PanelConvertFont (NSFont fontObj);

		[Export ("worksWhenModal")]
		bool WorksWhenModal { get; }

		[Export ("reloadDefaultFontFamilies")]
		void ReloadDefaultFontFamilies ();

		//Detected properties
		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }
	}

	[Mac (10,11)]
	[Static]
	interface NSFontWeight {
		[Field ("NSFontWeightUltraLight")]
		nfloat UltraLight { get; }

		[Field ("NSFontWeightThin")]
		nfloat Thin { get; }

		[Field ("NSFontWeightLight")]
		nfloat Light { get; }

		[Field ("NSFontWeightRegular")]
		nfloat Regular { get; }

		[Field ("NSFontWeightMedium")]
		nfloat Medium { get; }

		[Field ("NSFontWeightSemibold")]
		nfloat Semibold { get; }

		[Field ("NSFontWeightBold")]
		nfloat Bold { get; }

		[Field ("NSFontWeightHeavy")]
		nfloat Heavy { get; }

		[Field ("NSFontWeightBlack")]
		nfloat Black { get; }
	}

	[Availability (Deprecated = Platform.Mac_10_10)]
	[BaseType (typeof (NSMatrix))]
	partial interface NSForm  {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("initWithFrame:mode:prototype:numberOfRows:numberOfColumns:")]
		IntPtr Constructor (CGRect frameRect, NSMatrixMode aMode, NSCell aCell, nint rowsHigh, nint colsWide);

		[Export ("initWithFrame:mode:cellClass:numberOfRows:numberOfColumns:")]
		IntPtr Constructor (CGRect frameRect, NSMatrixMode aMode, Class factoryId, nint rowsHigh, nint colsWide);

		[Export ("indexOfSelectedItem")]
		nint SelectedItemIndex { get; }

		[Export ("setEntryWidth:")]
		void SetEntryWidth (nfloat width);

		[Export ("setInterlineSpacing:")]
		void SetInterlineSpacing (nfloat spacing);

		[Export ("setBordered:")]
		void SetBordered (bool bordered);

		[Export ("setBezeled:")]
		void SetBezeled (bool bezeled);

		[Export ("setTitleAlignment:")]
		void SetTitleAlignment (NSTextAlignment mode);

		[Export ("setTextAlignment:")]
		void SetTextAlignment (NSTextAlignment mode);

		[Export ("setTitleFont:")]
		void SetTitleFont (NSFont fontObj);

		[Export ("setTextFont:")]
		void SetTextFont (NSFont fontObj);

		[Export ("cellAtIndex:")]
		NSObject CellAtIndex (nint index);

		[Export ("drawCellAtIndex:")]
		void DrawCellAtIndex (nint index);

		[Export ("addEntry:")]
		NSFormCell AddEntry (string title);

		[Export ("insertEntry:atIndex:")]
		NSFormCell InsertEntryatIndex (string title, nint index);

		[Export ("removeEntryAtIndex:")]
		void RemoveEntryAtIndex (nint index);

		[Export ("indexOfCellWithTag:")]
		nint IndexOfCellWithTag (nint aTag);

		[Export ("selectTextAtIndex:")]
		void SelectTextAtIndex (nint index);

		[Export ("setFrameSize:")]
		void SetFrameSize (CGSize newSize);

		[Export ("setTitleBaseWritingDirection:")]
		void SetTitleBaseWritingDirection (NSWritingDirection writingDirection);

		[Export ("setTextBaseWritingDirection:")]
		void SetTextBaseWritingDirection (NSWritingDirection writingDirection);
	}
	
	[BaseType (typeof (NSActionCell))]
	partial interface NSFormCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("isOpaque")]
		bool IsOpaque { get; }

		//Detected properties
		[Export ("titleWidth")]
		nfloat TitleWidth { get; set; }

		[Export ("titleWidth:")]
		nfloat TitleWidthConstraintedToSize (CGSize aSize);		

		[Export ("title")]
		string Title { get; set; }

		[Export ("titleFont", ArgumentSemantic.Retain)]
		NSFont TitleFont { get; set; }

		[Export ("titleAlignment")]
		NSTextAlignment TitleAlignment { get; set; }

		[Export ("placeholderString")]
		string PlaceholderString { get; set; }

		[Export ("placeholderAttributedString", ArgumentSemantic.Copy)]
		NSAttributedString PlaceholderAttributedString { get; set; }

		[Export ("titleBaseWritingDirection")]
		NSWritingDirection TitleBaseWritingDirection { get; set; }

		[Availability (Deprecated = Platform.Mac_10_8, Message = "Set Title instead.")]
		[Export ("setTitleWithMnemonic:")]
		void SetTitleWithMnemonic (string  stringWithAmpersand);
		
		[Export ("attributedTitle")]
		NSAttributedString AttributedTitle { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSGlyphGenerator {
		[Export ("generateGlyphsForGlyphStorage:desiredNumberOfCharacters:glyphIndex:characterIndex:")]
		void GenerateGlyphs (NSObject nsGlyphStorageOrNSLayoutManager, nuint nchars, ref nuint glyphIndex, ref nuint charIndex);

		[Static, Export ("sharedGlyphGenerator")]
		NSGlyphGenerator SharedGlyphGenerator { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface NSGradient : NSCoding, NSCopying {
		[Export ("initWithStartingColor:endingColor:")]
		IntPtr Constructor  (NSColor startingColor, NSColor endingColor);

		[Export ("initWithColors:")]
		IntPtr Constructor  (NSColor[] colorArray);

		// See AppKit/NSGradiant.cs
		//[Export ("initWithColorsAndLocations:")]
		//[Export ("initWithColors:atLocations:colorSpace:")]

		[Export ("drawFromPoint:toPoint:options:")]
		void DrawFromPoint (CGPoint startingPoint, CGPoint endingPoint, NSGradientDrawingOptions options);

		[Export ("drawInRect:angle:")]
		void DrawInRect (CGRect rect, nfloat angle);

		[Export ("drawInBezierPath:angle:")]
		void DrawInBezierPath (NSBezierPath path, nfloat angle);

		[Export ("drawFromCenter:radius:toCenter:radius:options:")]
		void DrawFromCenterRadius (CGPoint startCenter, nfloat startRadius, CGPoint endCenter, nfloat endRadius, NSGradientDrawingOptions options);

		[Export ("drawInRect:relativeCenterPosition:")]
		void DrawInRect (CGRect rect, CGPoint relativeCenterPosition);

		[Export ("drawInBezierPath:relativeCenterPosition:")]
		void DrawInBezierPath (NSBezierPath path, CGPoint relativeCenterPosition);

		[Export ("colorSpace")]
		NSColorSpace ColorSpace { get; }

		[Export ("numberOfColorStops")]
		nint ColorStopsCount { get; }

		[Export ("getColor:location:atIndex:")]
		void GetColor (out NSColor color, out nfloat location, nint index);

		[Export ("interpolatedColorAtLocation:")]
		NSColor GetInterpolatedColor(nfloat location);
	}

	[ThreadSafe] // CurrentContext returns a context that can be used from the current thread
	[BaseType (typeof (NSObject))]
	interface NSGraphicsContext {
		[Static, Export ("graphicsContextWithAttributes:")]
		NSGraphicsContext FromAttributes (NSDictionary attributes);
	
		[Static, Export ("graphicsContextWithWindow:")]
		NSGraphicsContext FromWindow (NSWindow window);
	
		[Static, Export ("graphicsContextWithBitmapImageRep:")]
		NSGraphicsContext FromBitmap (NSBitmapImageRep bitmapRep);
	
		[Static, Export ("graphicsContextWithGraphicsPort:flipped:")]
		NSGraphicsContext FromGraphicsPort (IntPtr graphicsPort, bool initialFlippedState);
	
		[Static, Export ("currentContext")]
		NSGraphicsContext CurrentContext { get; set; }
	
		[Static, Export ("currentContextDrawingToScreen")]
		bool IsCurrentContextDrawingToScreen { get; }
	
		[Static, Export ("saveGraphicsState")]
		void GlobalSaveGraphicsState ();
	
		[Static, Export ("restoreGraphicsState")]
		void GlobalRestoreGraphicsState ();
	
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static, Export ("setGraphicsState:")]
		void SetGraphicsState (nint gState);
	
		[Export ("attributes")]
		NSDictionary Attributes { get; } 
	
		[Export ("isDrawingToScreen")]
		bool IsDrawingToScreen { get; }
	
		[Export ("saveGraphicsState")]
		void SaveGraphicsState ();
	
		[Export ("restoreGraphicsState")]
		void RestoreGraphicsState ();
	
		[Export ("flushGraphics")]
		void FlushGraphics ();

		// keep signature in sync with 'graphicsContextWithGraphicsPort:flipped:'
		[Export ("graphicsPort")]
		IntPtr GraphicsPortHandle {get; }
	
		[Export ("isFlipped")]
		bool IsFlipped { get; }
	
		[Export ("shouldAntialias")]
		bool ShouldAntialias { get; set; }
	
		[Export ("imageInterpolation")]
		NSImageInterpolation ImageInterpolation { get; set; }
	
		[Export ("patternPhase")]
		CGPoint PatternPhase { get; set; }
	
		[Export ("compositingOperation")]
		NSComposite CompositingOperation { get; set; }
	
		[Export ("colorRenderingIntent")]
		NSColorRenderingIntent ColorRenderingIntent { get; set; }

		[Export ("CIContext")]
		CoreImage.CIContext CIContext { get; } 

		[Mac (10,10)]
		[Export ("CGContext")]
		CGContext CGContext { get; }

		[Mac (10,10)]
		[Static, Export ("graphicsContextWithCGContext:flipped:")]
		NSGraphicsContext FromCGContext (CGContext graphicsPort, bool initialFlippedState);

	}

	[Mac (10,12)]
	[BaseType (typeof(NSView))]
	interface NSGridView
	{
		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect frameRect);

		[Static]
		[Export ("gridViewWithNumberOfColumns:rows:")]
		NSGridView Create (nint columnCount, nint rowCount);

		[Static]
		[Export ("gridViewWithViews:")]
		NSGridView Create (NSView [] rows);

		[Export ("numberOfRows")]
		nint RowCount { get; }

		[Export ("numberOfColumns")]
		nint ColumnCount { get; }

		[Export ("rowAtIndex:")]
		NSGridRow GetRow (nint index);

		[Export ("indexOfRow:")]
		nint GetIndex (NSGridRow row);

		[Export ("columnAtIndex:")]
		NSGridColumn GetColumn (nint index);

		[Export ("indexOfColumn:")]
		nint GetIndex (NSGridColumn column);

		[Export ("cellAtColumnIndex:rowIndex:")]
		NSGridCell GetCell (nint columnIndex, nint rowIndex);

		[Export ("cellForView:")]
		[return: NullAllowed]
		NSGridCell GetCell (NSView view);

		[Export ("addRowWithViews:")]
		NSGridRow AddRow (NSView[] views);

		[Export ("insertRowAtIndex:withViews:")]
		NSGridRow InsertRow (nint index, NSView[] views);

		[Export ("moveRowAtIndex:toIndex:")]
		void MoveRow (nint fromIndex, nint toIndex);

		[Export ("removeRowAtIndex:")]
		void RemoveRow (nint index);

		[Export ("addColumnWithViews:")]
		NSGridColumn AddColumn (NSView[] views);

		[Export ("insertColumnAtIndex:withViews:")]
		NSGridColumn InsertColumn (nint index, NSView[] views);

		[Export ("moveColumnAtIndex:toIndex:")]
		void MoveColumn (nint fromIndex, nint toIndex);

		[Export ("removeColumnAtIndex:")]
		void RemoveColumn (nint index);

		[Export ("xPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement X { get; set; }

		[Export ("yPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement Y { get; set; }

		[Export ("rowAlignment", ArgumentSemantic.Assign)]
		NSGridRowAlignment RowAlignment { get; set; }

		[Export ("rowSpacing")]
		nfloat RowSpacing { get; set; }

		[Export ("columnSpacing")]
		nfloat ColumnSpacing { get; set; }

		[Export ("mergeCellsInHorizontalRange:verticalRange:")]
		void MergeCells (NSRange hRange, NSRange vRange);

		[Mac (10, 12)]
		[Field ("NSGridViewSizeForContent")]
		nfloat SizeForContent { get; }
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSGridRow : NSCoding
	{
		[NullAllowed, Export ("gridView", ArgumentSemantic.Weak)]
		NSGridView GridView { get; }

		[Export ("numberOfCells")]
		nint CellCount { get; }

		[Export ("cellAtIndex:")]
		NSGridCell GetCell (nint index);

		[Export ("yPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement Y { get; set; }

		[Export ("rowAlignment", ArgumentSemantic.Assign)]
		NSGridRowAlignment RowAlignment { get; set; }

		[Export ("height")]
		nfloat Height { get; set; }

		[Export ("topPadding")]
		nfloat TopPadding { get; set; }

		[Export ("bottomPadding")]
		nfloat BottomPadding { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("mergeCellsInRange:")]
		void MergeCells (NSRange range);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSGridColumn : NSCoding
	{
		[NullAllowed, Export ("gridView", ArgumentSemantic.Weak)]
		NSGridView GridView { get; }

		[Export ("numberOfCells")]
		nint CellCount { get; }

		[Export ("cellAtIndex:")]
		NSGridCell GetCell (nint index);

		[Export ("xPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement X { get; set; }

		[Export ("width")]
		nfloat Width { get; set; }

		[Export ("leadingPadding")]
		nfloat LeadingPadding { get; set; }

		[Export ("trailingPadding")]
		nfloat TrailingPadding { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("mergeCellsInRange:")]
		void MergeCells (NSRange range);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSGridCell : NSCoding
	{
		[Export ("contentView", ArgumentSemantic.Strong), NullAllowed]
		NSView ContentView { get; set; }

		[Export ("emptyContentView", ArgumentSemantic.Strong)]
		[Static]
		NSView EmptyContentView { get; }

		[NullAllowed, Export ("row", ArgumentSemantic.Weak)]
		NSGridRow Row { get; }

		[NullAllowed, Export ("column", ArgumentSemantic.Weak)]
		NSGridColumn Column { get; }

		[Export ("xPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement X { get; set; }

		[Export ("yPlacement", ArgumentSemantic.Assign)]
		NSGridCellPlacement Y { get; set; }

		[Export ("rowAlignment", ArgumentSemantic.Assign)]
		NSGridRowAlignment RowAlignment { get; set; }

		[Export ("customPlacementConstraints", ArgumentSemantic.Copy)]
		NSLayoutConstraint[] CustomPlacementConstraints { get; set; }
	}

	[BaseType (typeof (NSGraphicsContext))]
	[DisableDefaultCtor]
	interface NSPrintPreviewGraphicsContext {
	}

	[BaseType (typeof (NSImageRep))]
	[DisableDefaultCtor] // An uncaught exception was raised: -[NSEPSImageRep init]: unrecognized selector sent to instance 0x1db2d90
	interface NSEPSImageRep {
		[Static]
		[Export ("imageRepWithData:")]
		NSObject FromData (NSData epsData);

		[Export ("initWithData:")]
		IntPtr Constructor (NSData epsData);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("prepareGState")]
		void PrepareGState ();

		[Export ("EPSRepresentation")]
		NSData EPSRepresentation { get; }

		[Export ("boundingBox")]
		CGRect BoundingBox { get; }
	}

	delegate void GlobalEventHandler (NSEvent theEvent);
	delegate NSEvent LocalEventHandler (NSEvent theEvent);
	delegate void NSEventTrackHandler (nfloat gestureAmount, NSEventPhase eventPhase, bool isComplete, ref bool stop);

	[BaseType (typeof (NSObject))]
	interface NSEvent : NSCoding, NSCopying {
		[Export ("type")]
		NSEventType Type { get; }

		[Export ("modifierFlags")]
		NSEventModifierMask ModifierFlags { get; }

		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("window")]
		NSWindow Window { get; }

		[Export ("windowNumber")]
		nint WindowNumber { get; }

		[Availability (Deprecated = Platform.Mac_10_12, Message = "This method always returns null. If you need access to the current drawing context, use NSGraphicsContext.CurrentContext inside of a draw operation.")]
		[Export ("context")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSGraphicsContext Context { get; }

		[Export ("clickCount")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint ClickCount { get; }

		[Export ("buttonNumber")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint ButtonNumber { get; }

		[Export ("eventNumber")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint EventNumber { get; }

		[Export ("pressure")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		float Pressure { get; } /* float, not CGFloat */

		[Export ("locationInWindow")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		CGPoint LocationInWindow { get; }

		[Export ("deltaX")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat DeltaX { get; }

		[Export ("deltaY")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat DeltaY { get; }

		[Export ("deltaZ")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat DeltaZ { get; }

		[Export ("characters")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		string Characters { get; }

		[Export ("charactersIgnoringModifiers")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		string CharactersIgnoringModifiers { get; }

		[Export ("isARepeat")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		bool IsARepeat { get; }

		[Export ("keyCode")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		ushort KeyCode { get; }

		[Export ("trackingNumber")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint TrackingNumber { get; }

		[Export ("userData")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		IntPtr UserData { get; }

		[Export ("trackingArea")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSTrackingArea TrackingArea { get; }

		[Export ("subtype")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		short Subtype { get; }

		[Export ("data1")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint Data1 { get; }

		[Export ("data2")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint Data2 { get; }

		[Export ("eventRef")]
		IntPtr EventRef { get; }

		[Static]
		[Export ("eventWithEventRef:")]
		NSEvent EventWithEventRef (IntPtr cgEventRef);

		[Export ("CGEvent")]
		IntPtr CGEvent { get; }

		[Static]
		[Export ("eventWithCGEvent:")]
		NSEvent EventWithCGEvent (IntPtr cgEventPtr);

		[Export ("magnification")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat Magnification { get; }

		[Export ("deviceID")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint DeviceID { get; }

		[Export ("rotation")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		float Rotation { get; } /* float, not CGFloat */

		[Export ("absoluteX")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint AbsoluteX { get; }

		[Export ("absoluteY")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint AbsoluteY { get; }

		[Export ("absoluteZ")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nint AbsoluteZ { get; }

		// TODO: What is the type?
		[Export ("buttonMask")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint ButtonMask { get; }

		[Export ("tilt")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		CGPoint Tilt { get; }

		[Export ("tangentialPressure")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		float TangentialPressure { get; } /* float, not CGFloat */

		[Export ("vendorDefined")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSObject VendorDefined { get; }

		[Export ("vendorID")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint VendorID { get; }

		[Export ("tabletID")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint TabletID { get; }

		[Export ("pointingDeviceID")]
		nuint PointingDeviceID ();

		[Export ("systemTabletID")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint SystemTabletID { get; }

		[Export ("vendorPointingDeviceType")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint VendorPointingDeviceType { get; }

		[Export ("pointingDeviceSerialNumber")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint PointingDeviceSerialNumber { get; }

		[Export ("uniqueID")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		long UniqueID { get; }

		[Export ("capabilityMask")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nuint CapabilityMask { get; }

		[Export ("pointingDeviceType")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSPointingDeviceType PointingDeviceType { get; }

		[Export ("isEnteringProximity")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		bool IsEnteringProximity { get; }

		[Export ("touchesMatchingPhase:inView:")]
		NSSet TouchesMatchingPhase (NSTouchPhase phase, NSView view);

		[Static]
		[Export ("startPeriodicEventsAfterDelay:withPeriod:")]
		void StartPeriodicEventsAfterDelay (double delay, double period);

		[Static]
		[Export ("stopPeriodicEvents")]
		void StopPeriodicEvents ();

		[Static]
		[Export ("mouseEventWithType:location:modifierFlags:timestamp:windowNumber:context:eventNumber:clickCount:pressure:")]
		NSEvent MouseEvent (NSEventType type, CGPoint location, NSEventModifierMask flags, double time, nint wNum, [NullAllowed] NSGraphicsContext context, nint eNum, nint cNum, float /* float, not CGFloat */ pressure);

		[Static]
		[Export ("keyEventWithType:location:modifierFlags:timestamp:windowNumber:context:characters:charactersIgnoringModifiers:isARepeat:keyCode:")]
		NSEvent KeyEvent (NSEventType type, CGPoint location, NSEventModifierMask flags, double time, nint wNum, [NullAllowed] NSGraphicsContext context, string keys, string ukeys, bool isARepeat, ushort code);

		[Static]
		[Export ("enterExitEventWithType:location:modifierFlags:timestamp:windowNumber:context:eventNumber:trackingNumber:userData:")]
		NSEvent EnterExitEvent (NSEventType type, CGPoint location, NSEventModifierMask flags, double time, nint wNum, [NullAllowed] NSGraphicsContext context, nint eNum, nint tNum, IntPtr data);

		[Static]
		[Export ("otherEventWithType:location:modifierFlags:timestamp:windowNumber:context:subtype:data1:data2:")]
		NSEvent OtherEvent (NSEventType type, CGPoint location, NSEventModifierMask flags, double time, nint wNum, [NullAllowed] NSGraphicsContext context, short subtype, nint d1, nint d2);

		[Static]
		[Export ("mouseLocation")]
		CGPoint CurrentMouseLocation { get; }

		[Static]
		[Export ("modifierFlags")]
		NSEventModifierMask CurrentModifierFlags { get; }

		[Static]
		[Export ("pressedMouseButtons")]
		nuint CurrentPressedMouseButtons { get; }

		[Static]
		[Export ("doubleClickInterval")]
		double DoubleClickInterval { get; }

		[Static]
		[Export ("keyRepeatDelay")]
		double KeyRepeatDelay { get; }

		[Static]
		[Export ("keyRepeatInterval")]
		double KeyRepeatInterval { get; }

		[Static]
		[Export ("addGlobalMonitorForEventsMatchingMask:handler:")]
		NSObject AddGlobalMonitorForEventsMatchingMask (NSEventMask mask, GlobalEventHandler handler);
		
		[Static]
		[Export ("addLocalMonitorForEventsMatchingMask:handler:")]
		NSObject AddLocalMonitorForEventsMatchingMask (NSEventMask mask, LocalEventHandler handler);
		
		[Static]
		[Export ("removeMonitor:")]
		void RemoveMonitor (NSObject eventMonitor);

		//Detected properties
		[Static]
		[Export ("mouseCoalescingEnabled")]
		bool MouseCoalescingEnabled { [Bind ("isMouseCoalescingEnabled")]get; set; }

		[Mac (10, 7)]
		[Export ("hasPreciseScrollingDeltas")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		bool HasPreciseScrollingDeltas { get; }

		[Mac (10, 7)]
		[Export ("scrollingDeltaX")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat ScrollingDeltaX { get; }

		[Mac (10, 7)]
		[Export ("scrollingDeltaY")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		nfloat ScrollingDeltaY { get; }

		[Mac (10, 7)]
		[Export ("momentumPhase")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSEventPhase MomentumPhase { get; }

		[Mac (10, 7)]
		[Export ("isDirectionInvertedFromDevice")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		bool IsDirectionInvertedFromDevice { get; }

		[Mac (10, 7)]
		[Export ("phase")]
		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		NSEventPhase Phase { get; }

		[Mac (10, 7)]
		[Static]
		[Export ("isSwipeTrackingFromScrollEventsEnabled")]
		bool IsSwipeTrackingFromScrollEventsEnabled { get; }

		[Mac (10, 7)]
		[Export ("trackSwipeEventWithOptions:dampenAmountThresholdMin:max:usingHandler:")]
		void TrackSwipeEvent (NSEventSwipeTrackingOptions options, nfloat minDampenThreshold, nfloat maxDampenThreshold, NSEventTrackHandler trackingHandler);

		[Mac (10,10,3)]
		[Export ("stage")]
		nint Stage { get; }

		[Mac (10,10,3)]
		[Export ("stageTransition")]
		nfloat StageTransition { get; }

		[Mac (10,10,3)]
		[Export ("associatedEventsMask")]
		NSEventMask AssociatedEventsMask { get; }

		[Mac (10, 12)]
		[Export ("allTouches")]
		NSSet<NSTouch> AllTouches { get; }

		[Mac (10,12)]
		[Export ("touchesForView:")]
		NSSet<NSTouch> GetTouches (NSView view);

		[Mac (10,12,2)]
		[Export ("coalescedTouchesForTouch:")]
		NSTouch[] GetCoalescedTouches (NSTouch touch);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject), Delegates=new string [] {"WeakDelegate"}, Events=new Type[] {typeof (NSGestureRecognizerDelegate)})]
	interface NSGestureRecognizer : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithTarget:action:")]
		IntPtr Constructor ([NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }

		[Export ("action")]
		Selector Action { get; set; }

		[Export ("state")]
		NSGestureRecognizerState State { get; [Advice ("Only subclasses of 'NSGestureRecognizer' can set this property.")] set; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSGestureRecognizerDelegate Delegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("view")]
		NSView View { get; }

		[Export ("delaysPrimaryMouseButtonEvents")]
		bool DelaysPrimaryMouseButtonEvents { get; set; }

		[Export ("delaysSecondaryMouseButtonEvents")]
		bool DelaysSecondaryMouseButtonEvents { get; set; }

		[Export ("delaysOtherMouseButtonEvents")]
		bool DelaysOtherMouseButtonEvents { get; set; }

		[Export ("delaysKeyEvents")]
		bool DelaysKeyEvents { get; set; }

		[Export ("delaysMagnificationEvents")]
		bool DelaysMagnificationEvents { get; set; }

		[Export ("delaysRotationEvents")]
		bool DelaysRotationEvents { get; set; }

		[Export ("locationInView:")]
		CGPoint LocationInView ([NullAllowed] NSView view);

		[Export ("reset")]
		void Reset ();

		[Export ("canPreventGestureRecognizer:")]
		bool CanPrevent (NSGestureRecognizer preventedGestureRecognizer);

		[Export ("canBePreventedByGestureRecognizer:")]
		bool CanBePrevented (NSGestureRecognizer preventingGestureRecognizer);

		[Export ("shouldRequireFailureOfGestureRecognizer:")]
		bool ShouldRequireFailureOfGestureRecognizer (NSGestureRecognizer otherGestureRecognizer);

		[Export ("shouldBeRequiredToFailByGestureRecognizer:")]
		bool ShouldBeRequiredToFailByGestureRecognizer (NSGestureRecognizer otherGestureRecognizer);

		[Export ("mouseDown:")]
		void MouseDown (NSEvent mouseEvent);

		[Export ("rightMouseDown:")]
		void RightMouseDown (NSEvent mouseEvent);

		[Export ("otherMouseDown:")]
		void OtherMouseDown (NSEvent mouseEvent);

		[Export ("mouseUp:")]
		void MouseUp (NSEvent mouseEvent);

		[Export ("rightMouseUp:")]
		void RightMouseUp (NSEvent mouseEvent);

		[Export ("otherMouseUp:")]
		void OtherMouseUp (NSEvent mouseEvent);

		[Export ("mouseDragged:")]
		void MouseDragged (NSEvent mouseEvent);

		[Export ("rightMouseDragged:")]
		void RightMouseDragged (NSEvent mouseEvent);

		[Export ("otherMouseDragged:")]
		void OtherMouseDragged (NSEvent mouseEvent);

		[Export ("keyDown:")]
		void KeyDown (NSEvent keyEvent);

		[Export ("keyUp:")]
		void KeyUp (NSEvent keyEvent);

		[Export ("flagsChanged:")]
		void FlagsChanged (NSEvent flagEvent);

		[Export ("tabletPoint:")]
		void TabletPoint (NSEvent tabletEvent);

		[Export ("magnifyWithEvent:")]
		void Magnify (NSEvent magnifyEvent);

		[Export ("rotateWithEvent:")]
		void Rotate (NSEvent rotateEvent);

#if XAMCORE_2_0
		[Mac (10,10,3, onlyOn64 : true)]
		[Export ("pressureChangeWithEvent:")]
		void PressureChange (NSEvent pressureChangeEvent);
#endif

		[Mac (10,11)]
		[Export ("pressureConfiguration", ArgumentSemantic.Strong)]
		NSPressureConfiguration PressureConfiguration { get; set; }

		[Mac (10,12,2)]
		[Export ("touchesBeganWithEvent:")]
		void TouchesBegan (NSEvent touchEvent);

		[Mac (10,12,2)]
		[Export ("touchesMovedWithEvent:")]
		void TouchesMoved (NSEvent touchEvent);

		[Mac (10,12,2)]
		[Export ("touchesEndedWithEvent:")]
		void TouchesEnded (NSEvent touchEvent);

		[Mac (10,12,2)]
		[Export ("touchesCancelledWithEvent:")]
		void TouchesCancelled (NSEvent touchEvent);
	}

	[Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSGestureRecognizerDelegate {
		[Export ("gestureRecognizerShouldBegin:"), DelegateName ("NSGestureProbe"), DefaultValue (true)]
		bool ShouldBegin (NSGestureRecognizer gestureRecognizer);

		[Export ("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:"), DelegateName ("NSGesturesProbe"), DefaultValue (false)]
		bool ShouldRecognizeSimultaneously (NSGestureRecognizer gestureRecognizer, NSGestureRecognizer otherGestureRecognizer);

		[Export ("gestureRecognizer:shouldRequireFailureOfGestureRecognizer:"), DelegateName ("NSGesturesProbe"), DefaultValue (false)]
		bool ShouldRequireFailure (NSGestureRecognizer gestureRecognizer, NSGestureRecognizer otherGestureRecognizer);

		[Export ("gestureRecognizer:shouldBeRequiredToFailByGestureRecognizer:"), DelegateName ("NSGesturesProbe"), DefaultValue (false)]
		bool ShouldBeRequiredToFail (NSGestureRecognizer gestureRecognizer, NSGestureRecognizer otherGestureRecognizer);

#if !XAMCORE_4_0
		[Export ("xamarinselector:removed:"), DelegateName ("NSGestureEvent"), DefaultValue (true)]
		[Obsolete ("It will never be called.")]
		bool ShouldReceiveEvent (NSGestureRecognizer gestureRecognizer, NSEvent gestureEvent);
#endif

		[Mac (10,11)]
		[Export ("gestureRecognizer:shouldAttemptToRecognizeWithEvent:"), DelegateName ("NSGestureEvent"), DefaultValue (true)]
		bool ShouldAttemptToRecognize (NSGestureRecognizer gestureRecognizer, NSEvent theEvent);

		[Mac (10,12,2)]
		[Export ("gestureRecognizer:shouldReceiveTouch:"), DelegateName ("NSTouchEvent"), DefaultValue (true)]
		bool ShouldReceiveTouch (NSGestureRecognizer gestureRecognizer, NSTouch touch);
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe] // Not documented anywhere, but their Finder extension sample uses it on non-ui thread
	[Dispose ("__mt_items_var = null;")]
	partial interface NSMenu : NSCoding, NSCopying, NSAccessibility, NSAccessibilityElement, NSUserInterfaceItemIdentification  {
		[DesignatedInitializer]
		[Export ("initWithTitle:")]
		IntPtr Constructor (string aTitle);

		[Static]
		[Export ("popUpContextMenu:withEvent:forView:")]
		void PopUpContextMenu (NSMenu menu, NSEvent theEvent, NSView view);

		[Static]
		[Export ("popUpContextMenu:withEvent:forView:withFont:")]
		void PopUpContextMenu (NSMenu menu, NSEvent theEvent, NSView view, [NullAllowed] NSFont font);

		[Export ("popUpMenuPositioningItem:atLocation:inView:")]
		bool PopUpMenu ([NullAllowed] NSMenuItem item, CGPoint location, [NullAllowed] NSView view);

		[Export ("insertItem:atIndex:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		void InsertItem (NSMenuItem newItem, nint index);

		[Export ("addItem:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		void AddItem (NSMenuItem newItem);

		[Export ("insertItemWithTitle:action:keyEquivalent:atIndex:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		NSMenuItem InsertItem (string title, [NullAllowed] Selector action, string charCode, nint index);

		[Export ("addItemWithTitle:action:keyEquivalent:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		NSMenuItem AddItem (string title, [NullAllowed] Selector action, string charCode);

		[Export ("removeItemAtIndex:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		void RemoveItemAt (nint index);

		[Export ("removeItem:")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		void RemoveItem (NSMenuItem item);

		[Export ("setSubmenu:forItem:")]
		void SetSubmenu ([NullAllowed] NSMenu aMenu, NSMenuItem anItem);

		[Export ("removeAllItems")]
		[PostSnippet ("__mt_items_var = ItemArray();")]
		void RemoveAllItems ();

		[Export ("itemArray")]
		NSMenuItem [] ItemArray ();

		[Export ("numberOfItems")]
		nint Count { get; }

		[Export ("itemAtIndex:")]
		[return: NullAllowed]
		NSMenuItem ItemAt (nint index);

		[Export ("indexOfItem:")]
		nint IndexOf (NSMenuItem index);

		[Export ("indexOfItemWithTitle:")]
		nint IndexOf (string aTitle);

		[Export ("indexOfItemWithTag:")]
		nint IndexOf (nint itemTag);

		[Export ("indexOfItemWithRepresentedObject:")]
		nint IndexOfItem ([NullAllowed] NSObject obj);

		[Export ("indexOfItemWithSubmenu:")]
		nint IndexOfItem ([NullAllowed] NSMenu submenu);

		[Export ("indexOfItemWithTarget:andAction:")]
		nint IndexOfItem ([NullAllowed] NSObject target, [NullAllowed] Selector actionSelector);

		[Export ("itemWithTitle:")]
		[return: NullAllowed]
		NSMenuItem ItemWithTitle (string title);

		[Export ("itemWithTag:")]
		[return: NullAllowed]
		NSMenuItem ItemWithTag (nint tag);

		[Export ("update")]
		void Update ();

		[Export ("performKeyEquivalent:")]
		bool PerformKeyEquivalent (NSEvent theEvent);

		[Export ("itemChanged:")]
		void ItemChanged (NSMenuItem item);

		[Export ("performActionForItemAtIndex:")]
		void PerformActionForItem (nint index);

		[Export ("menuBarHeight")]
		nfloat MenuBarHeight { get; }

		[Export ("cancelTracking")]
		void CancelTracking ();

		[Export ("cancelTrackingWithoutAnimation")]
		void CancelTrackingWithoutAnimation ();

		[Export ("highlightedItem")]
		[NullAllowed]
		NSMenuItem HighlightedItem { get; }

		[Export ("size")]
		CGSize Size { get; }

		[Static]
		[Export ("menuZone")]
		NSZone MenuZone { get; }

		[Export ("helpRequested:")]
		void HelpRequested (NSEvent eventPtr);

		[Export ("isTornOff")]
		bool IsTornOff { get; }

		//Detected properties
		[Export ("title")]
		string Title { get; set; }

		[Static]
		[Export ("menuBarVisible")]
		bool MenuBarVisible { get; set; }

		[NullAllowed, Export ("supermenu", ArgumentSemantic.Assign)]
		NSMenu Supermenu { get; set; }

		[Export ("autoenablesItems")]
		bool AutoEnablesItems { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		[NullAllowed]
		NSMenuDelegate Delegate { get; set; }
		
		[Export ("minimumWidth")]
		nfloat MinimumWidth { get; set; }

		[Export ("font", ArgumentSemantic.Retain)]
		NSFont Font { get; set; }

		[Export ("allowsContextMenuPlugIns")]
		bool AllowsContextMenuPlugIns { get; set; }

		[Export ("showsStateColumn")]
		bool ShowsStateColumn { get; set; }

		[Export ("menuChangedMessagesEnabled")]
		bool MenuChangedMessagesEnabled { get; set; }

		[Export ("propertiesToUpdate")]
		NSMenuProperty PropertiesToUpdate ();

		[Mac (10,11)]
		[Export ("userInterfaceLayoutDirection", ArgumentSemantic.Assign)]
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; set; }
	}

	interface INSMenuDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMenuDelegate {
		[Export ("menuNeedsUpdate:")]
		void NeedsUpdate (NSMenu menu);

		[Export ("numberOfItemsInMenu:")]
		nint MenuItemCount (NSMenu menu);

		[Export ("menu:updateItem:atIndex:shouldCancel:")]
		bool UpdateItem (NSMenu menu, NSMenuItem item, nint atIndex, bool shouldCancel);

		[Export ("menuHasKeyEquivalent:forEvent:target:action:")]
		bool HasKeyEquivalentForEvent (NSMenu menu, NSEvent theEvent, NSObject target, Selector action);

		[Export ("menuWillOpen:")]
		void MenuWillOpen (NSMenu menu);

		[Export ("menuDidClose:")]
		void MenuDidClose (NSMenu menu);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("menu:willHighlightItem:")]
		void MenuWillHighlightItem (NSMenu menu, NSMenuItem item);

		[Export ("confinementRectForMenu:onScreen:")]
		CGRect ConfinementRectForMenu (NSMenu menu, NSScreen screen);
	}

	[BaseType (typeof (NSObject))]
	[ThreadSafe] // Not documented anywhere, but their Finder extension sample uses it on non-ui thread
	interface NSMenuItem : NSCoding, NSCopying, NSAccessibility, NSAccessibilityElement, NSUserInterfaceItemIdentification {
		[Static]
		[Export ("separatorItem")]
		NSMenuItem SeparatorItem { get; }

		[DesignatedInitializer]
		[Export ("initWithTitle:action:keyEquivalent:")]
		IntPtr Constructor (string title, [NullAllowed] Selector selectorAction, string charCode);

		[Export ("hasSubmenu")]
		bool HasSubmenu { get; }

		[Export ("parentItem")]
		NSMenuItem ParentItem { get; }

		[Export ("isSeparatorItem")]
		bool IsSeparatorItem { get; }

		[Export ("userKeyEquivalent")]
		string UserKeyEquivalent { get; }

		[Export ("setTitleWithMnemonic:")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'Title' instead.")]
		void SetTitleWithMnemonic (string stringWithAmpersand);

		[Export ("isHighlighted")]
		bool Highlighted { get; }

		[Export ("isHiddenOrHasHiddenAncestor")]
		bool IsHiddenOrHasHiddenAncestor { get; }

		//Detected properties
		[Static]
		[Export ("usesUserKeyEquivalents")]
		bool UsesUserKeyEquivalents { get; set; }

		[Export ("menu")]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Export ("submenu", ArgumentSemantic.Retain)]
		NSMenu Submenu { get; [NullAllowed] set; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[Export ("keyEquivalent")]
		string KeyEquivalent { get; set; }

		[Export ("keyEquivalentModifierMask")]
		NSEventModifierMask KeyEquivalentModifierMask { get; set; }

		[Export ("image", ArgumentSemantic.Retain), NullAllowed]
		NSImage Image { get; set; }

		[Export ("state")]
		NSCellStateValue State { get; set; }

		[Export ("onStateImage", ArgumentSemantic.Retain)]
		NSImage OnStateImage { get; set; }

		[Export ("offStateImage", ArgumentSemantic.Retain)]
		NSImage OffStateImage { get; set; }

		[Export ("mixedStateImage", ArgumentSemantic.Retain)]
		NSImage MixedStateImage { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Export ("alternate")]
		bool Alternate { [Bind ("isAlternate")]get; set; }

		[Export ("indentationLevel")]
		nint IndentationLevel { get; set; }

		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }

		[Export ("action"), NullAllowed]
		Selector Action { get; set; }

		[Export ("tag")]
		nint Tag { get; set; }

		[Export ("representedObject", ArgumentSemantic.Retain)]
		NSObject RepresentedObject { get; set; }

		[Export ("view", ArgumentSemantic.Retain)]
		NSView View { get; set; }

		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("toolTip")]
		string ToolTip { get; set; }

		[Mac (10, 13)]
		[Export ("allowsKeyEquivalentWhenHidden")]
		bool AllowsKeyEquivalentWhenHidden { get; set; }
	}

	[BaseType (typeof (NSButtonCell))]
	interface NSMenuItemCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("calcSize")]
		void CalcSize ();

		[Export ("stateImageWidth")]
		nfloat StateImageWidth ();

		[Export ("imageWidth")]
		nfloat ImageWidth { get; }

		[Export ("titleWidth")]
		nfloat TitleWidth { get; }

		[Export ("keyEquivalentWidth")]
		nfloat KeyEquivalentWidth { get; }

		[Export ("stateImageRectForBounds:")]
		CGRect StateImageRectForBounds (CGRect cellFrame);

		[Export ("titleRectForBounds:")]
		CGRect TitleRectForBounds (CGRect cellFrame);

		[Export ("keyEquivalentRectForBounds:")]
		CGRect KeyEquivalentRectForBounds (CGRect cellFrame);

		[Export ("drawSeparatorItemWithFrame:inView:")]
		void DrawSeparatorItem (CGRect cellFrame, NSView controlView);

		[Export ("drawStateImageWithFrame:inView:")]
		void DrawStateImage (CGRect cellFrame, NSView controlView);

		[Export ("drawImageWithFrame:inView:")]
		void DrawImage (CGRect cellFrame, NSView controlView);

		[Export ("drawTitleWithFrame:inView:")]
		void DrawTitle (CGRect cellFrame, NSView controlView);

		[Export ("drawKeyEquivalentWithFrame:inView:")]
		void DrawKeyEquivalent (CGRect cellFrame, NSView controlView);

		[Export ("drawBorderAndBackgroundWithFrame:inView:")]
		void DrawBorderAndBackground (CGRect cellFrame, NSView controlView);

		[Export ("tag")]
		nint Tag { get; }

		//Detected properties
		[Export ("menuItem", ArgumentSemantic.Retain)]
		NSMenuItem MenuItem { get; set; }

		[Export ("menuView")]
		NSMenuView MenuView { get; set; }

		[Export ("needsSizing")]
		bool NeedsSizing { get; set; }

		[Export ("needsDisplay")]
		bool NeedsDisplay { get; set; }

	}

	[Mac (10, 0, 0, PlatformArchitecture.Arch32)] // kept for the arch limitation
	[BaseType (typeof (NSView))]
	interface NSMenuView {
		[Static]
		[Export ("menuBarHeight")]
		nfloat MenuBarHeight { get; }

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		// <quote>Deprecated. Tear-off menus are not supported in OS X.</quote>
		//[Export ("initAsTearOff")]
		//IntPtr Constructor (int tokenInitAsTearOff);

		[Export ("itemChanged:")]
		void ItemChanged (NSNotification notification);

		[Export ("itemAdded:")]
		void ItemAdded (NSNotification notification);

		[Export ("itemRemoved:")]
		void ItemRemoved (NSNotification notification);

		[Export ("update")]
		void Update ();

		[Export ("innerRect")]
		CGRect InnerRect { get; }

		[Export ("rectOfItemAtIndex:")]
		CGRect RectOfItemAtIndex (nint index);

		[Export ("indexOfItemAtPoint:")]
		nint IndexOfItemAtPoint (CGPoint point);

		[Export ("setNeedsDisplayForItemAtIndex:")]
		void SetNeedsDisplay (nint itemAtIndex);

		[Export ("stateImageOffset")]
		nfloat StateImageOffset { get; }

		[Export ("stateImageWidth")]
		nfloat StateImageWidth { get; }

		[Export ("imageAndTitleOffset")]
		nfloat ImageAndTitleOffset { get; }

		[Export ("imageAndTitleWidth")]
		nfloat ImageAndTitleWidth { get; }

		[Export ("keyEquivalentOffset")]
		nfloat KeyEquivalentOffset { get; }

		[Export ("keyEquivalentWidth")]
		nfloat KeyEquivalentWidth { get; }

		[Export ("setMenuItemCell:forItemAtIndex:")]
		void SetMenuItemCell (NSMenuItemCell cell, nint itemAtIndex);

		[Export ("menuItemCellForItemAtIndex:")]
		NSMenuItemCell GetMenuItemCell (nint itemAtIndex);

		[Export ("attachedMenuView")]
		NSMenuView AttachedMenuView { get; }

		[Export ("sizeToFit")]
		void SizeToFit ();

		[Export ("attachedMenu")]
		NSMenu AttachedMenu { get; }

		[Export ("isAttached")]
		bool IsAttached { get; }

		[Export ("isTornOff")]
		bool IsTornOff { get; }

		[Export ("locationForSubmenu:")]
		CGPoint LocationForSubmenu (NSMenu aSubmenu);

		[Export ("setWindowFrameForAttachingToRect:onScreen:preferredEdge:popUpSelectedItem:")]
		void SetWindowFrameForAttachingToRect (CGRect screenRect, NSScreen onScreen, NSRectEdge preferredEdge, nint popupSelectedItem);

		[Export ("detachSubmenu")]
		void DetachSubmenu ();

		[Export ("attachSubmenuForItemAtIndex:")]
		void AttachSubmenuForItemAtIndex (nint index);

		[Export ("performActionWithHighlightingForItemAtIndex:")]
		void PerformActionWithHighlighting (nint forItemAtIndex);

		[Export ("trackWithEvent:")]
		bool TrackWithEvent (NSEvent theEvent);

		//Detected properties
		[Export ("menu")]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Export ("horizontal")]
		bool Horizontal { [Bind ("isHorizontal")]get; set; }

		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("highlightedItemIndex")]
		nint HighlightedItemIndex { get; set; }

		[Export ("needsSizing")]
		bool NeedsSizing { get; set; }

		[Export ("horizontalEdgePadding")]
		nfloat HorizontalEdgePadding { get; set; }
	}

	[BaseType (typeof (NSObject))]
	partial interface NSNib : NSCoding {
		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl nibFileUrl);

		[Export ("initWithNibNamed:bundle:")]
		IntPtr Constructor (string nibName, [NullAllowed] NSBundle bundle);

		[Export ("instantiateNibWithExternalNameTable:")]
		bool InstantiateNib (NSDictionary externalNameTable);

		[Mac (10,8)]
		[Export ("instantiateWithOwner:topLevelObjects:")]
		bool InstantiateNibWithOwner ([NullAllowed] NSObject owner, out NSArray topLevelObjects);

		// This requires an "out NSArray"
		//[Export ("instantiateNibWithOwner:topLevelObjects:")]
		//bool InstantiateNib (NSObject owner, NSArray topLevelObjects);
	}	

	[BaseType (typeof (NSController))]
	interface NSObjectController {
		[DesignatedInitializer]
		[Export ("initWithContent:")]
		IntPtr Constructor (NSObject content);

		[Export ("content", ArgumentSemantic.Retain)]
		NSObject Content { get; set; }

		[Export ("selection")]
		NSObjectController Selection { get; }

		[Export ("selectedObjects")]
		NSObject [] SelectedObjects { get; [NotImplemented] set; }

		[Export ("automaticallyPreparesContent")]
		bool AutomaticallyPreparesContent { get; set; }

		[Export ("prepareContent")]
		void PrepareContent ();

		[Export ("objectClass")]
		Class ObjectClass { get; set; }

		[Export ("newObject")]
		NSObject NewObject { get; }

		[Export ("addObject:")]
		void AddObject (NSObject object1);

		[Export ("removeObject:")]
		void RemoveObject (NSObject object1);

#if !XAMCORE_2_0
		[Export ("setEditable:")]
		[Obsolete ("Use the 'Editable' property instead.")]
		[Sealed]
		void SetEditable (bool editable);
#endif

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Export ("add:")]
		void Add (NSObject sender);

		[Export ("canAdd")]
		bool CanAdd { get; }

		[Export ("remove:")]
		void Remove (NSObject sender);

		[Export ("canRemove")]
		bool CanRemove { get; }

		[Export ("validateUserInterfaceItem:")]
		bool ValidateUserInterfaceItem (NSObject item);

		//[Export ("managedObjectContext")]
		//NSManagedObjectContext ManagedObjectContext { get; set; }

		[Export ("entityName")]
		string EntityName { get; set; }

		[Export ("fetchPredicate")]
		NSPredicate FetchPredicate { get; set; }

		//[Export ("fetchWithRequest:merge:error:")]
		//bool FetchWithRequestMerge (NSFetchRequest fetchRequest, bool merge, NSError error);

		[Export ("fetch:")]
		void Fetch (NSObject sender);

		[Export ("usesLazyFetching")]
		bool UsesLazyFetching { get; set; }

		//[Export ("defaultFetchRequest")]
		//NSFetchRequest DefaultFetchRequest { get; }
	}

	[ThreadSafe]
	[BaseType (typeof (NSObject))]
	interface NSOpenGLPixelFormat : NSCoding {
		[Export ("initWithData:")]
		IntPtr Constructor (NSData attribs);

		// TODO: wrap the CLContext and take a CLContext here instead.
		//[Export ("initWithCGLPixelFormatObj:")]
		//IntPtr Constructor (IntPtr cglContextHandle);

		[Export ("getValues:forAttribute:forVirtualScreen:")]
		void GetValue (ref int /* GLint = int32_t */ vals, NSOpenGLPixelFormatAttribute attrib, int /* GLint = int32_t */ screen);

		[Export ("numberOfVirtualScreens")]
		int NumberOfVirtualScreens { get; } /* GLint = int32_t */

		[Export ("CGLPixelFormatObj")]
		CGLPixelFormat CGLPixelFormat { get; }
	}

	[ThreadSafe]
	[Availability (Deprecated = Platform.Mac_10_7)]
	[BaseType (typeof (NSObject))]
	interface NSOpenGLPixelBuffer {
		[Export ("initWithTextureTarget:textureInternalFormat:textureMaxMipMapLevel:pixelsWide:pixelsHigh:")]
		IntPtr Constructor (NSGLTextureTarget targetGlEnum, NSGLFormat format, int /* GLint = int32_t */ maxLevel, int /* GLsizei = int32_t */ pixelsWide, int /* GLsizei = int32_t */ pixelsHigh);

		// FIXME: This conflicts with our internal ctor
		// [Export ("initWithCGLPBufferObj:")]
		// IntPtr Constructor (IntPtr pbuffer);

		[Export ("CGLPBufferObj")]
		IntPtr CGLPBuffer { get; }

		[Export ("pixelsWide")]
		int PixelsWide { get; } /* GLsizei = int32_t */

		[Export ("pixelsHigh")]
		int PixelsHigh { get; } /* GLsizei = int32_t */

		[Export ("textureTarget")]
		NSGLTextureTarget TextureTarget { get; }

		[Export ("textureInternalFormat")]
		NSGLFormat TextureInternalFormat { get; }

		[Export ("textureMaxMipMapLevel")]
		int TextureMaxMipMapLevel { get; } /* GLint = int32_t */
	}

	[ThreadSafe] // single thread - but not restricted to the main thread
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // warns with "invalid context" at runtime
	interface NSOpenGLContext {
		[Export ("initWithFormat:shareContext:")]
		IntPtr Constructor (NSOpenGLPixelFormat format, [NullAllowed] NSOpenGLContext shareContext);

		// FIXME: This conflicts with our internal ctor
		// [Export ("initWithCGLContextObj:")]
		// IntPtr Constructor (IntPtr cglContext);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("setFullScreen")]
		void SetFullScreen ();

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("setOffScreen:width:height:rowbytes:")]
		void SetOffScreen (IntPtr baseaddr, int /* GLsizei = int32_t */ width, int /* GLsizei = int32_t */ height, int /* GLint = int32_t */ rowbytes);

		[Export ("clearDrawable")]
		void ClearDrawable ();

		[Export ("update")]
		void Update ();

		[Export ("flushBuffer")]
		void FlushBuffer ();

                [ThreadSafe]
		[Export ("makeCurrentContext")]
		void MakeCurrentContext ();

		[Static]
		[Export ("clearCurrentContext")]
		void ClearCurrentContext ();

		[Static]
		[Export ("currentContext")]
		NSOpenGLContext CurrentContext { get; }

		[Availability (Deprecated = Platform.Mac_10_8)]
		[Export ("copyAttributesFromContext:withMask:")]
		void CopyAttributes (NSOpenGLContext context, uint /* GLbitfield = uint32_t */ mask);

		[Export ("setValues:forParameter:")]
		void SetValues (IntPtr vals, NSOpenGLContextParameter param);

		[Export ("getValues:forParameter:")]
		void GetValues (IntPtr vals, NSOpenGLContextParameter param);

		[Availability (Deprecated = Platform.Mac_10_8)]
		[Export ("createTexture:fromView:internalFormat:")]
		void CreateTexture (int /* GLenum = uint32_t */ targetIdentifier, NSView view, int /* GLenum = uint32_t */ format);

		[Export ("CGLContextObj")]
		CGLContext CGLContext { get; }

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("setPixelBuffer:cubeMapFace:mipMapLevel:currentVirtualScreen:")]
		void SetPixelBuffer (NSOpenGLPixelBuffer pixelBuffer, NSGLTextureCubeMap face, int /* GLint = int32_t */ level, int /* GLint = int32_t */ screen);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("pixelBuffer")]
		NSOpenGLPixelBuffer PixelBuffer { get; }

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("pixelBufferCubeMapFace")]
		int PixelBufferCubeMapFace { get; } /* GLenum = uint32_t */

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("pixelBufferMipMapLevel")]
		int PixelBufferMipMapLevel { get; } /* GLint = int32_t */

		// TODO: fixme enumerations
		// GL_FRONT, GL_BACK, GL_AUX0
		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("setTextureImageToPixelBuffer:colorBuffer:")]
		void SetTextureImage (NSOpenGLPixelBuffer pixelBuffer, NSGLColorBuffer source);

		//Detected properties
		[Export ("view")]
		NSView View { get; set; }

		[Export ("currentVirtualScreen")]
		int CurrentVirtualScreen { get; set; } /* GLint = int32_t */

		[Mac (10,10)]
		[Export ("pixelFormat", ArgumentSemantic.Retain)]
		NSOpenGLPixelFormat PixelFormat { get; }
	}

	[BaseType (typeof (NSView))]
	partial interface NSOpenGLView {
		[Static]
		[Export ("defaultPixelFormat")]
		NSOpenGLPixelFormat DefaultPixelFormat { get; }

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("initWithFrame:pixelFormat:")]
		IntPtr Constructor (CGRect frameRect, NSOpenGLPixelFormat format);

		[Export ("clearGLContext")]
		void ClearGLContext ();

		[Export ("update")]
		void Update ();

		[Export ("reshape")]
		void Reshape ();

		[Export ("prepareOpenGL")]
		void PrepareOpenGL ();

		//Detected properties
		[Export ("openGLContext", ArgumentSemantic.Retain)]
		NSOpenGLContext OpenGLContext { get; set; }

		[Export ("pixelFormat", ArgumentSemantic.Retain)]
		NSOpenGLPixelFormat PixelFormat { get; set; }
	}

	[BaseType (typeof (NSSavePanel))]
	interface NSOpenPanel {
		[Export ("URLs")]
		NSUrl [] Urls { get; }

		//Detected properties
		[Export ("resolvesAliases")]
		bool ResolvesAliases { get; set; }

		[Export ("canChooseDirectories")]
		bool CanChooseDirectories { get; set; }

		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("canChooseFiles")]
		bool CanChooseFiles { get; set; }

		// Deprecated methods, but needed to run on pre 10.6 systems
		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use Urls instead.")]
		[Export ("filenames")]
		string [] Filenames { get; }

		//runModalForWindows:Completeion
		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use NSApplication.RunModal instead.")]
		[Export ("beginSheetForDirectory:file:types:modalForWindow:modalDelegate:didEndSelector:contextInfo:")]
		void BeginSheet ([NullAllowed] string directory, [NullAllowed] string fileName, [NullAllowed] string [] fileTypes, [NullAllowed] NSWindow modalForWindow, [NullAllowed] NSObject modalDelegate, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);

		[Availability (Deprecated = Platform.Mac_10_6)]
		[Export ("beginForDirectory:file:types:modelessDelegate:didEndSelector:contextInfo:")]
		void Begin ([NullAllowed] string directory, [NullAllowed] string fileName, [NullAllowed] string [] fileTypes, [NullAllowed] NSObject modelessDelegate, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use NSApplication.RunModal instead.")]
		[Export ("runModalForDirectory:file:types:")]
		nint RunModal ([NullAllowed] string directory, [NullAllowed] string fileName, [NullAllowed] string [] types);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use NSApplication.RunModal instead.")]
		[Export ("runModalForTypes:")]
		nint RunModal (string [] types);
	}

#if !XAMCORE_4_0
	// This class doesn't show up in any documentation
	[BaseType (typeof (NSOpenPanel))]
	[DisableDefaultCtor] // should not be created by (only returned to) user code
	interface NSRemoteOpenPanel {}
#endif

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSOpenSavePanelDelegate {
		[Export ("panel:shouldEnableURL:"), DelegateName ("NSOpenSavePanelUrl"), DefaultValue (true)]
		bool ShouldEnableUrl (NSSavePanel panel, NSUrl url);

		[Export ("panel:validateURL:error:"), DelegateName ("NSOpenSavePanelValidate"), DefaultValue (true)]
		bool ValidateUrl (NSSavePanel panel, NSUrl url, out NSError outError);

		[Export ("panel:didChangeToDirectoryURL:"), EventArgs ("NSOpenSavePanelUrl")]
		void DidChangeToDirectory (NSSavePanel panel, NSUrl newDirectoryUrl);

		[Export ("panel:userEnteredFilename:confirmed:"), DelegateName ("NSOpenSaveFilenameConfirmation"), DefaultValueFromArgument ("filename")]
		string UserEnteredFilename (NSSavePanel panel, string filename, bool confirmed);

		[Export ("panel:willExpand:"), EventArgs ("NSOpenSaveExpanding")]
		void WillExpand (NSSavePanel panel, bool expanding);

		[Export ("panelSelectionDidChange:"), EventArgs ("NSOpenSaveSelectionChanged")]
		void SelectionDidChange (NSSavePanel panel);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use ValidateUrl instead.")]
		[Export ("panel:isValidFilename:"), DelegateName ("NSOpenSaveFilename"), DefaultValue (true)]
		bool IsValidFilename (NSSavePanel panel, string fileName);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use DidChangeToDirectory instead.")]
		[Export ("panel:directoryDidChange:"), EventArgs ("NSOpenSaveFilename")]
		void DirectoryDidChange (NSSavePanel panel, string path);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "This method does not control sorting order.")]
		[Export ("panel:compareFilename:with:caseSensitive:"), DelegateName ("NSOpenSaveCompare"), DefaultValue (NSComparisonResult.Same)]
		NSComparisonResult CompareFilenames (NSSavePanel panel, string name1, string name2, bool caseSensitive);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use ShouldEnableUrl instead.")]
		[Export ("panel:shouldShowFilename:"), DelegateName ("NSOpenSaveFilename"), DefaultValue (true)]
		bool ShouldShowFilename (NSSavePanel panel, string filename);
	}

	[BaseType (typeof (NSTableView))]
	partial interface NSOutlineView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("outlineTableColumn"), NullAllowed]
		NSTableColumn OutlineTableColumn { get; set; }

		[Export ("isExpandable:")]
		bool IsExpandable ([NullAllowed] NSObject item);

		[Export ("expandItem:expandChildren:")]
		void ExpandItem ([NullAllowed] NSObject item, bool expandChildren);

		[Export ("expandItem:")]
		void ExpandItem ([NullAllowed] NSObject item);

		[Export ("collapseItem:collapseChildren:")]
		void CollapseItem ([NullAllowed] NSObject item, bool collapseChildren);

		[Export ("collapseItem:")]
		void CollapseItem ([NullAllowed] NSObject item);

		[Export ("reloadItem:reloadChildren:")]
		void ReloadItem ([NullAllowed] NSObject item, bool reloadChildren);

		[Export ("reloadItem:")]
		void ReloadItem ([NullAllowed] NSObject item);

		[Export ("parentForItem:")]
		NSObject GetParent ([NullAllowed] NSObject item);

		[Export ("itemAtRow:")]
		NSObject ItemAtRow (nint row);

		[Export ("rowForItem:")]
		nint RowForItem ([NullAllowed] NSObject item);

		[Export ("levelForItem:")]
		nint LevelForItem ([NullAllowed] NSObject item);

		[Export ("levelForRow:")]
		nint LevelForRow (nint row);

		[Export ("isItemExpanded:")]
		bool IsItemExpanded (NSObject item);

		[Export ("indentationPerLevel")]
		nfloat IndentationPerLevel { get; set; }

		[Export ("indentationMarkerFollowsCell")]
		bool IndentationMarkerFollowsCell { get; set; }

		[Export ("autoresizesOutlineColumn")]
		bool AutoresizesOutlineColumn { get; set; }

		[Export ("frameOfOutlineCellAtRow:")]
		CGRect FrameOfOutlineCellAtRow (nint row);

		[Export ("setDropItem:dropChildIndex:")]
		void SetDropItem ([NullAllowed] NSObject item, nint index);

		[Export ("shouldCollapseAutoExpandedItemsForDeposited:")]
		bool ShouldCollapseAutoExpandedItems (bool forDeposited);

		[Export ("autosaveExpandedItems")]
		bool AutosaveExpandedItems { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate  { get; set; }

		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSOutlineViewDelegate Delegate  { get; set; }

		[Export ("dataSource", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDataSource  { get; set; }

		[Wrap ("WeakDataSource")][NullAllowed]
		[Protocolize]
		NSOutlineViewDataSource DataSource  { get; set; }

		[Mac (10,10)]
		[Export ("numberOfChildrenOfItem:")]
		nint NumberOfChildren ([NullAllowed] NSObject item);

		[Mac (10,10)]
		[Export ("child:ofItem:")]
		NSObject GetChild (nint index, [NullAllowed] NSObject parentItem);

		[Mac (10,7)]
		[Export ("userInterfaceLayoutDirection")]
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; set; }

		[Mac (10,11)]
		[Export ("childIndexForItem:")]
		nint GetChildIndex (NSObject item);

		[Mac (10, 12)]
		[Export ("stronglyReferencesItems")]
		bool StronglyReferencesItems { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSOutlineViewDelegate {
		[Export ("outlineView:willDisplayCell:forTableColumn:item:")]
		void WillDisplayCell (NSOutlineView outlineView, NSObject cell, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:shouldEditTableColumn:item:")] [DefaultValue (false)]
		bool ShouldEditTableColumn (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("selectionShouldChangeInOutlineView:")] [DefaultValue (false)]
		bool SelectionShouldChange (NSOutlineView outlineView);
	
		[Export ("outlineView:shouldSelectItem:")] [DefaultValue (true)]
		bool ShouldSelectItem (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:selectionIndexesForProposedSelection:")]
		NSIndexSet GetSelectionIndexes (NSOutlineView outlineView, NSIndexSet proposedSelectionIndexes);
	
		[Export ("outlineView:shouldSelectTableColumn:")]
		bool ShouldSelectTableColumn (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn);
	
		[Export ("outlineView:mouseDownInHeaderOfTableColumn:")]
		void MouseDown (NSOutlineView outlineView, NSTableColumn tableColumn);
	
		[Export ("outlineView:didClickTableColumn:")]
		void DidClickTableColumn (NSOutlineView outlineView, NSTableColumn tableColumn);
	
		[Export ("outlineView:didDragTableColumn:")]
		void DidDragTableColumn (NSOutlineView outlineView, NSTableColumn tableColumn);
		
		[Export ("outlineView:toolTipForCell:rect:tableColumn:item:mouseLocation:")]
		string ToolTipForCell (NSOutlineView outlineView, NSCell cell, ref CGRect rect, NSTableColumn tableColumn, NSObject item, CGPoint mouseLocation);
	
		[Export ("outlineView:heightOfRowByItem:"), NoDefaultValue]
		nfloat GetRowHeight (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:typeSelectStringForTableColumn:item:")]
		string GetSelectString (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:nextTypeSelectMatchFromItem:toItem:forString:")]
		NSObject GetNextTypeSelectMatch (NSOutlineView outlineView, NSObject startItem, NSObject endItem, string searchString);
	
		[Export ("outlineView:shouldTypeSelectForEvent:withCurrentSearchString:")]
		bool ShouldTypeSelect (NSOutlineView outlineView, NSEvent theEvent, [NullAllowed] string searchString);
	
		[Export ("outlineView:shouldShowCellExpansionForTableColumn:item:")]
		bool ShouldShowCellExpansion (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:shouldTrackCell:forTableColumn:item:")]
		bool ShouldTrackCell (NSOutlineView outlineView, NSCell cell, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:dataCellForTableColumn:item:"), NoDefaultValue]
		NSCell GetCell (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item);

		[Mac (10, 7)]
		[Export ("outlineView:viewForTableColumn:item:"), NoDefaultValue]
		NSView GetView (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:isGroupItem:")]
		bool IsGroupItem (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:shouldExpandItem:")]
		bool ShouldExpandItem (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:shouldCollapseItem:")]
		bool ShouldCollapseItem (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:willDisplayOutlineCell:forTableColumn:item:")]
		void WillDisplayOutlineCell (NSOutlineView outlineView, NSObject cell, [NullAllowed] NSTableColumn tableColumn, NSObject item);
	
		[Export ("outlineView:sizeToFitWidthOfColumn:"), NoDefaultValue]
		nfloat GetSizeToFitColumnWidth (NSOutlineView outlineView, nint column);
	
		[Export ("outlineView:shouldReorderColumn:toColumn:")]
		bool ShouldReorder (NSOutlineView outlineView, nint columnIndex, nint newColumnIndex);
	
		[Export ("outlineView:shouldShowOutlineCellForItem:")]
		bool ShouldShowOutlineCell (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineViewColumnDidMove:")]
		void ColumnDidMove (NSNotification notification);
	
		[Export ("outlineViewColumnDidResize:")]
		void ColumnDidResize (NSNotification notification);
	
		[Export ("outlineViewSelectionIsChanging:")]
		void SelectionIsChanging (NSNotification notification);
	
		[Export ("outlineViewItemWillExpand:")]
		void ItemWillExpand (NSNotification notification);
	
		[Export ("outlineViewItemDidExpand:")]
		void ItemDidExpand (NSNotification notification);
	
		[Export ("outlineViewItemWillCollapse:")]
		void ItemWillCollapse (NSNotification notification);
	
		[Export ("outlineViewItemDidCollapse:")]
		void ItemDidCollapse (NSNotification notification);

		[Export ("outlineViewSelectionDidChange:")]
		void SelectionDidChange (NSNotification notification);

		[Mac (10, 7), Export ("outlineView:rowViewForItem:")]
		NSTableRowView RowViewForItem (NSOutlineView outlineView, NSObject item);

		[Mac (10, 7), Export ("outlineView:didAddRowView:forRow:")]
		void DidAddRowView (NSOutlineView outlineView, NSTableRowView rowView, nint row);

		[Mac (10, 7), Export ("outlineView:didRemoveRowView:forRow:")]
		void DidRemoveRowView (NSOutlineView outlineView, NSTableRowView rowView, nint row);
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSOutlineViewDataSource {
		[Export ("outlineView:child:ofItem:")]
		NSObject GetChild (NSOutlineView outlineView, nint childIndex, [NullAllowed] NSObject item);
	
		[Export ("outlineView:isItemExpandable:")]
		bool ItemExpandable (NSOutlineView outlineView, NSObject item);
	
		[Export ("outlineView:numberOfChildrenOfItem:")]
		nint GetChildrenCount (NSOutlineView outlineView, [NullAllowed] NSObject item);
	
		[Export ("outlineView:objectValueForTableColumn:byItem:")]
		NSObject GetObjectValue (NSOutlineView outlineView, [NullAllowed] NSTableColumn tableColumn, [NullAllowed] NSObject item);
	
		[Export ("outlineView:setObjectValue:forTableColumn:byItem:")]
		void SetObjectValue (NSOutlineView outlineView, [NullAllowed] NSObject theObject, [NullAllowed] NSTableColumn tableColumn, [NullAllowed] NSObject item);
	
		[Export ("outlineView:itemForPersistentObject:")]
		NSObject ItemForPersistentObject (NSOutlineView outlineView, NSObject theObject);
	
		[Export ("outlineView:persistentObjectForItem:")]
		NSObject PersistentObjectForItem (NSOutlineView outlineView, [NullAllowed] NSObject item);
	
		[Export ("outlineView:sortDescriptorsDidChange:")]
		void SortDescriptorsChanged (NSOutlineView outlineView, NSSortDescriptor [] oldDescriptors);
	
		[Export ("outlineView:writeItems:toPasteboard:")]
		bool OutlineViewwriteItemstoPasteboard (NSOutlineView outlineView, NSArray items, NSPasteboard pboard);
	
		[Export ("outlineView:validateDrop:proposedItem:proposedChildIndex:")]
		NSDragOperation ValidateDrop (NSOutlineView outlineView, [Protocolize (4)] NSDraggingInfo info, [NullAllowed] NSObject item, nint index);
	
		[Export ("outlineView:acceptDrop:item:childIndex:")]
		bool AcceptDrop (NSOutlineView outlineView, [Protocolize (4)] NSDraggingInfo info, [NullAllowed] NSObject item, nint index);
	
		[Export ("outlineView:namesOfPromisedFilesDroppedAtDestination:forDraggedItems:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSFilePromiseReceiver' objects instead.")]
		string [] FilesDropped (NSOutlineView outlineView, NSUrl dropDestination, NSArray items);
	}

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSHapticFeedbackPerformer
	{
		[Abstract]
		[Export ("performFeedbackPattern:performanceTime:")]
		void PerformFeedback (NSHapticFeedbackPattern pattern, NSHapticFeedbackPerformanceTime performanceTime);
	}

	interface INSHapticFeedbackPerformer { }

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSHapticFeedbackManager
	{
		[Static]
		[Export ("defaultPerformer")]
		INSHapticFeedbackPerformer DefaultPerformer { get; }
	}

	[BaseType (typeof (NSObject))]
	partial interface NSHelpManager {
		[Static]
		[Export ("sharedHelpManager")]
		NSHelpManager SharedHelpManager ();

		[Export ("setContextHelp:forObject:")]
		void SetContext (NSAttributedString attrString, NSObject theObject);

		[Export ("removeContextHelpForObject:")]
		void RemoveContext (NSObject theObject);

		[Export ("contextHelpForObject:")]
		NSAttributedString Context (NSObject theObject);

		[Export ("showContextHelpForObject:locationHint:")]
		bool ShowContext (NSObject theObject, CGPoint pt);

		[Export ("openHelpAnchor:inBook:")]
		void OpenHelpAnchor (string anchor, string book);

		[Export ("findString:inBook:")]
		void FindString (string query, string book);

		[Export ("registerBooksInBundle:")]
		bool RegisterBooks (NSBundle bundle );

		//Detected properties
		[Static]
		[Export ("contextHelpModeActive")]
		bool ContextHelpModeActive { [Bind ("isContextHelpModeActive")]get; set; }
	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSImageDelegate)})]
	[Dispose ("__mt_reps_var = null;")]
	[ThreadSafe]
	partial interface NSImage : NSCoding, NSCopying, NSSecureCoding, NSPasteboardReading, NSPasteboardWriting {
		[Static]
		[Export ("imageNamed:")]
		NSImage ImageNamed (string name);

		[DesignatedInitializer]
		[Export ("initWithSize:")]
		IntPtr Constructor (CGSize aSize);

		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[Export ("initWithContentsOfFile:")]
		IntPtr Constructor (string fileName);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		//[Export ("initByReferencingURL:")]
		//IntPtr Constructor (NSUrl url);

		// FIXME: need IconRec
		//[Export ("initWithIconRef:")]
		//IntPtr Constructor (IconRef iconRef);

		[Sealed, Export ("initWithContentsOfFile:"), Internal]
		IntPtr InitWithContentsOfFile (string fileName);

		[Export ("initByReferencingFile:"), Internal]
		IntPtr InitByReferencingFile (string name);

		[Export ("initWithPasteboard:")]
		IntPtr Constructor (NSPasteboard pasteboard);

		[Export ("initWithData:"), Internal]
		[Sealed]
		IntPtr InitWithData (NSData data);

		[Export ("initWithDataIgnoringOrientation:"), Internal]
		IntPtr InitWithDataIgnoringOrientation (NSData data);

		[Export ("drawAtPoint:fromRect:operation:fraction:")]
		void Draw (CGPoint point, CGRect fromRect, NSCompositingOperation op, nfloat delta);

		[Export ("drawInRect:fromRect:operation:fraction:")]
		void Draw (CGRect rect, CGRect fromRect, NSCompositingOperation op, nfloat delta);

		[Export ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:")]
		void Draw (CGRect dstSpacePortionRect, CGRect srcSpacePortionRect, NSCompositingOperation op, nfloat requestedAlpha, bool respectContextIsFlipped, [NullAllowed] NSDictionary hints);

		[Mac (10,9)]
		[Export ("drawInRect:")]
		void Draw (CGRect rect);

		[Export ("drawRepresentation:inRect:")]
		bool Draw (NSImageRep imageRep, CGRect rect);

		[Export ("recache")]
		void Recache ();

		[Export ("TIFFRepresentation")]
		NSData AsTiff ();

		[Export ("TIFFRepresentationUsingCompression:factor:")]
		NSData AsTiff (NSTiffCompression comp, float /* float, not CGFloat */ aFloat);

		[Export ("representations")]
		NSImageRep [] Representations ();

		[Export ("addRepresentations:")]
		[PostSnippet ("__mt_reps_var = Representations();")]
		void AddRepresentations (NSImageRep [] imageReps);

		[Export ("addRepresentation:")]
		[PostSnippet ("__mt_reps_var = Representations();")]
		void AddRepresentation (NSImageRep imageRep);

		[Export ("removeRepresentation:")]
		[PostSnippet ("__mt_reps_var = Representations();")]
		void RemoveRepresentation (NSImageRep imageRep);

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("lockFocus")]
		void LockFocus ();

		[Export ("lockFocusFlipped:")]
		void LockFocusFlipped (bool flipped);

		[Export ("unlockFocus")]
		void UnlockFocus ();

		[Export ("bestRepresentationForDevice:")]
		NSImageRep BestRepresentationForDevice ([NullAllowed] NSDictionary deviceDescription);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageUnfilteredFileTypes")]
		NSObject [] ImageUnfilteredFileTypes ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageUnfilteredPasteboardTypes")]
		string [] ImageUnfilteredPasteboardTypes ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageFileTypes")]
		string [] ImageFileTypes { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imagePasteboardTypes")]
		string [] ImagePasteboardTypes { get; }
		
		[Static]
		[Export ("imageTypes", ArgumentSemantic.Copy)]
		string [] ImageTypes { get; }

		[Static]
		[Export ("imageUnfilteredTypes", ArgumentSemantic.Copy)]
		string [] ImageUnfilteredTypes { get; }
		
		[Static]
		[Export ("canInitWithPasteboard:")]
		bool CanInitWithPasteboard (NSPasteboard pasteboard);

		[Export ("cancelIncrementalLoad")]
		void CancelIncrementalLoad ();

		[Export ("accessibilityDescription")]
		string AccessibilityDescription	 { get; set; }

		[Export ("initWithCGImage:size:")]
		IntPtr Constructor (CGImage cgImage, CGSize size);

		[Export ("CGImageForProposedRect:context:hints:")]
		CGImage AsCGImage (ref CGRect proposedDestRect, [NullAllowed] NSGraphicsContext referenceContext, [NullAllowed] NSDictionary hints);

		[Export ("bestRepresentationForRect:context:hints:")]
		NSImageRep BestRepresentation (CGRect rect, [NullAllowed] NSGraphicsContext referenceContext, [NullAllowed] NSDictionary hints);

		[Export ("hitTestRect:withImageDestinationRect:context:hints:flipped:")]
		bool HitTestRect (CGRect testRectDestSpace, CGRect imageRectDestSpace, NSGraphicsContext context, NSDictionary hints, bool flipped);

		//Detected properties
		[Export ("size")]
		CGSize Size { get; set; }

		[Export ("name"), Internal]
		string GetName ();

		[Export ("setName:"), Internal]
		bool SetName (string aString);

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("usesEPSOnResolutionMismatch")]
		bool UsesEpsOnResolutionMismatch { get; set; }

		[Export ("prefersColorMatch")]
		bool PrefersColorMatch { get; set; }

		[Export ("matchesOnMultipleResolution")]
		bool MatchesOnMultipleResolution { get; set; }

		[Mac (10,7)]
		[Export ("matchesOnlyOnBestFittingAxis")]
		bool MatchesOnlyOnBestFittingAxis { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSImageDelegate Delegate { get; set; }

		[Export ("cacheMode")]
		NSImageCacheMode CacheMode { get; set; }

		[Export ("alignmentRect")]
		CGRect AlignmentRect { get; set; }

		[Export ("template")]
		bool Template { [Bind ("isTemplate")]get; set; }

#if !XAMCORE_2_0
		[Bind ("sizeWithAttributes:")]
		CGSize StringSize ([Target] string str, NSDictionary attributes);

		[Bind ("drawInRect:withAttributes:")]
		void DrawInRect ([Target] string str, CGRect rect, NSDictionary attributes);
#endif

		[Export ("drawInRect:fromRect:operation:fraction:")]
		[Sealed]
		void DrawInRect (CGRect dstRect, CGRect srcRect, NSCompositingOperation operation, nfloat delta);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use DrawInRect with respectContextIsFlipped instead.")]
		[Export ("flipped")]
		bool Flipped { [Bind ("isFlipped")] get; set; }

		[Mac (10,10)]
		[Export ("capInsets")]
		NSEdgeInsets CapInsets { get; set; }

		[Mac (10,10)]
		[Export ("resizingMode")]
		NSImageResizingMode ResizingMode { get; set; }

		[Mac (10,7)]
		[Export ("recommendedLayerContentsScale:")]
		nfloat GetRecommendedLayerContentsScale (nfloat preferredContentsScale);

		[Mac (10,7)]
		[Export ("layerContentsForContentsScale:")]
		NSObject GetLayerContentsForContentsScale (nfloat layerContentsScale);
	}

	public enum NSImageName 
	{
		[Field ("NSImageNameQuickLookTemplate")]
		QuickLookTemplate,

		[Field ("NSImageNameBluetoothTemplate")]
		BluetoothTemplate,

		[Field ("NSImageNameIChatTheaterTemplate")]
		IChatTheaterTemplate,

		[Field ("NSImageNameSlideshowTemplate")]
		SlideshowTemplate,

		[Field ("NSImageNameActionTemplate")]
		ActionTemplate,

		[Field ("NSImageNameSmartBadgeTemplate")]
		SmartBadgeTemplate,

		[Field ("NSImageNamePathTemplate")]
		PathTemplate,

		[Field ("NSImageNameInvalidDataFreestandingTemplate")]
		InvalidDataFreestandingTemplate,

		[Field ("NSImageNameLockLockedTemplate")]
		LockLockedTemplate,

		[Field ("NSImageNameLockUnlockedTemplate")]
		LockUnlockedTemplate,

		[Field ("NSImageNameGoRightTemplate")]
		GoRightTemplate,

		[Field ("NSImageNameGoLeftTemplate")]
		GoLeftTemplate,

		[Field ("NSImageNameRightFacingTriangleTemplate")]
		RightFacingTriangleTemplate,

		[Field ("NSImageNameLeftFacingTriangleTemplate")]
		LeftFacingTriangleTemplate,

		[Field ("NSImageNameAddTemplate")]
		AddTemplate,

		[Field ("NSImageNameRemoveTemplate")]
		RemoveTemplate,

		[Field ("NSImageNameRevealFreestandingTemplate")]
		RevealFreestandingTemplate,

		[Field ("NSImageNameFollowLinkFreestandingTemplate")]
		FollowLinkFreestandingTemplate,

		[Field ("NSImageNameEnterFullScreenTemplate")]
		EnterFullScreenTemplate,

		[Field ("NSImageNameExitFullScreenTemplate")]
		ExitFullScreenTemplate,

		[Field ("NSImageNameStopProgressTemplate")]
		StopProgressTemplate,

		[Field ("NSImageNameStopProgressFreestandingTemplate")]
		StopProgressFreestandingTemplate,

		[Field ("NSImageNameRefreshTemplate")]
		RefreshTemplate,

		[Field ("NSImageNameRefreshFreestandingTemplate")]
		RefreshFreestandingTemplate,

		[Field ("NSImageNameFolder")]
		Folder,

		[Field ("NSImageNameTrashEmpty")]
		TrashEmpty,

		[Field ("NSImageNameTrashFull")]
		TrashFull,

		[Field ("NSImageNameHomeTemplate")]
		HomeTemplate,

		[Field ("NSImageNameBookmarksTemplate")]
		BookmarksTemplate,

		[Field ("NSImageNameCaution")]
		Caution,

		[Field ("NSImageNameStatusAvailable")]
		StatusAvailable,

		[Field ("NSImageNameStatusPartiallyAvailable")]
		StatusPartiallyAvailable,

		[Field ("NSImageNameStatusUnavailable")]
		StatusUnavailable,

		[Field ("NSImageNameStatusNone")]
		StatusNone,

		[Field ("NSImageNameApplicationIcon")]
		ApplicationIcon,

		[Field ("NSImageNameMenuOnStateTemplate")]
		MenuOnStateTemplate,

		[Field ("NSImageNameMenuMixedStateTemplate")]
		MenuMixedStateTemplate,

		[Field ("NSImageNameUserGuest")]
		UserGuest,

		[Field ("NSImageNameMobileMe")]
		MobileMe,

		[Mac (10, 8)]
		[Field ("NSImageNameShareTemplate")]
		ShareTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAddDetailTemplate")]
		TouchBarAddDetailTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAddTemplate")]
		TouchBarAddTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAlarmTemplate")]
		TouchBarAlarmTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioInputMuteTemplate")]
		TouchBarAudioInputMuteTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioInputTemplate")]
		TouchBarAudioInputTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioOutputMuteTemplate")]
		TouchBarAudioOutputMuteTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioOutputVolumeHighTemplate")]
		TouchBarAudioOutputVolumeHighTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioOutputVolumeLowTemplate")]
		TouchBarAudioOutputVolumeLowTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioOutputVolumeMediumTemplate")]
		TouchBarAudioOutputVolumeMediumTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarAudioOutputVolumeOffTemplate")]
		TouchBarAudioOutputVolumeOffTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarBookmarksTemplate")]
		TouchBarBookmarksTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarColorPickerFill")]
		TouchBarColorPickerFill,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarColorPickerFont")]
		TouchBarColorPickerFont,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarColorPickerStroke")]
		TouchBarColorPickerStroke,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarCommunicationAudioTemplate")]
		TouchBarCommunicationAudioTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarCommunicationVideoTemplate")]
		TouchBarCommunicationVideoTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarComposeTemplate")]
		TouchBarComposeTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarDeleteTemplate")]
		TouchBarDeleteTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarDownloadTemplate")]
		TouchBarDownloadTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarEnterFullScreenTemplate")]
		TouchBarEnterFullScreenTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarExitFullScreenTemplate")]
		TouchBarExitFullScreenTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarFastForwardTemplate")]
		TouchBarFastForwardTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarFolderCopyToTemplate")]
		TouchBarFolderCopyToTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarFolderMoveToTemplate")]
		TouchBarFolderMoveToTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarFolderTemplate")]
		TouchBarFolderTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarGetInfoTemplate")]
		TouchBarGetInfoTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarGoBackTemplate")]
		TouchBarGoBackTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarGoDownTemplate")]
		TouchBarGoDownTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarGoForwardTemplate")]
		TouchBarGoForwardTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarGoUpTemplate")]
		TouchBarGoUpTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarHistoryTemplate")]
		TouchBarHistoryTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarIconViewTemplate")]
		TouchBarIconViewTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarListViewTemplate")]
		TouchBarListViewTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarMailTemplate")]
		TouchBarMailTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarNewFolderTemplate")]
		TouchBarNewFolderTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarNewMessageTemplate")]
		TouchBarNewMessageTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarOpenInBrowserTemplate")]
		TouchBarOpenInBrowserTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarPauseTemplate")]
		TouchBarPauseTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarPlayheadTemplate")]
		TouchBarPlayheadTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarPlayPauseTemplate")]
		TouchBarPlayPauseTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarPlayTemplate")]
		TouchBarPlayTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarQuickLookTemplate")]
		TouchBarQuickLookTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRecordStartTemplate")]
		TouchBarRecordStartTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRecordStopTemplate")]
		TouchBarRecordStopTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRefreshTemplate")]
		TouchBarRefreshTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRewindTemplate")]
		TouchBarRewindTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRotateLeftTemplate")]
		TouchBarRotateLeftTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarRotateRightTemplate")]
		TouchBarRotateRightTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSearchTemplate")]
		TouchBarSearchTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarShareTemplate")]
		TouchBarShareTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSidebarTemplate")]
		TouchBarSidebarTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipAhead15SecondsTemplate")]
		TouchBarSkipAhead15SecondsTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipAhead30SecondsTemplate")]
		TouchBarSkipAhead30SecondsTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipAheadTemplate")]
		TouchBarSkipAheadTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipBack15SecondsTemplate")]
		TouchBarSkipBack15SecondsTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipBack30SecondsTemplate")]
		TouchBarSkipBack30SecondsTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipBackTemplate")]
		TouchBarSkipBackTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipToEndTemplate")]
		TouchBarSkipToEndTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSkipToStartTemplate")]
		TouchBarSkipToStartTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarSlideshowTemplate")]
		TouchBarSlideshowTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTagIconTemplate")]
		TouchBarTagIconTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextBoldTemplate")]
		TouchBarTextBoldTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextBoxTemplate")]
		TouchBarTextBoxTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextCenterAlignTemplate")]
		TouchBarTextCenterAlignTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextItalicTemplate")]
		TouchBarTextItalicTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextJustifiedAlignTemplate")]
		TouchBarTextJustifiedAlignTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextLeftAlignTemplate")]
		TouchBarTextLeftAlignTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextListTemplate")]
		TouchBarTextListTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextRightAlignTemplate")]
		TouchBarTextRightAlignTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextStrikethroughTemplate")]
		TouchBarTextStrikethroughTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarTextUnderlineTemplate")]
		TouchBarTextUnderlineTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarUserAddTemplate")]
		TouchBarUserAddTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarUserGroupTemplate")]
		TouchBarUserGroupTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarUserTemplate")]
		TouchBarUserTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarVolumeDownTemplate")]
		TouchBarVolumeDownTemplate,

		[Mac (10, 12, 2)]
		[Field ("NSImageNameTouchBarVolumeUpTemplate")]
		TouchBarVolumeUpTemplate,

		[Mac (10, 13)]
		[Field ("NSImageNameTouchBarRemoveTemplate")]
		TouchBarRemoveTemplate,
	}

	interface NSStringAttributes {

	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSStringDrawingContext
	{
		[Export ("minimumScaleFactor", ArgumentSemantic.Assign)]
		nfloat MinimumScaleFactor { get; set; }

		[Export ("actualScaleFactor")]
		nfloat ActualScaleFactor { get; }

		[Export ("totalBounds")]
		CGRect TotalBounds { get; }
	}

	[Category, BaseType (typeof (NSString))]
	interface NSStringDrawing_NSString {
		[Export ("sizeWithAttributes:")]
		CGSize StringSize (NSDictionary attributes);

		[Wrap ("This.StringSize (attributes == null ? null : attributes.Dictionary)")]
		CGSize StringSize (AppKit.NSStringAttributes attributes);

		[Export ("drawAtPoint:withAttributes:")]
		void DrawAtPoint (CGPoint point, NSDictionary attributes);

		[Wrap ("This.DrawAtPoint (point, attributes == null ? null : attributes.Dictionary)")]
		void DrawAtPoint (CGPoint point, AppKit.NSStringAttributes attributes);

		[Export ("drawInRect:withAttributes:")]
		void DrawInRect (CGRect rect, NSDictionary attributes);

		[Wrap ("This.DrawInRect (rect, attributes == null ? null : attributes.Dictionary)")]
		void DrawInRect (CGRect rect, AppKit.NSStringAttributes attributes);
	}

	[Category, BaseType (typeof (NSAttributedString))]
	interface NSStringDrawing_NSAttributedString {
		[Export ("size")]
		CGSize GetSize ();

		[Export ("drawAtPoint:")]
		void DrawAtPoint (CGPoint point);

		[Export ("drawInRect:")]
		void DrawInRect (CGRect rect);
	}
		
	[Category, BaseType (typeof (NSString))]
	interface NSExtendedStringDrawing {
		[Mac (10,11)]
		[Export ("drawWithRect:options:attributes:context:")]
		void WeakDrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		[Mac (10,11)]
		[Wrap ("WeakDrawString (This, rect, options, attributes == null ? null : attributes.Dictionary, context)")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringAttributes attributes, [NullAllowed] NSStringDrawingContext context);

		[Mac (10,11)]
		[Export ("boundingRectWithSize:options:attributes:context:")]
		CGRect WeakGetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSDictionary attributes, [NullAllowed] NSStringDrawingContext context);

		[Mac (10,11)]
		[Wrap ("WeakGetBoundingRect (This, size, options, attributes == null ? null : attributes.Dictionary, context)")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringAttributes attributes, [NullAllowed] NSStringDrawingContext context);
	}

	// @interface NSExtendedStringDrawing (NSAttributedString)
	[Category]
	[BaseType (typeof(NSAttributedString))]
	interface NSAttributedString_NSExtendedStringDrawing
	{
		[Mac (10,11)]
		[Export ("drawWithRect:options:context:")]
		void DrawWithRect (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		[Mac (10,11)]
		[Export ("boundingRectWithSize:options:context:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);
	}

	// Pending: @interface NSAttributedString (NSExtendedStringDrawing)

	[Category, BaseType (typeof (NSMutableAttributedString))]
	interface NSMutableAttributedStringAppKitAddons {
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromURL (NSUrl url, NSDictionary options, out NSDictionary returnOptions, out NSError error);

		[Wrap ("This.ReadFromURL (url, options == null ? null : options.Dictionary, out returnOptions, out error)")]
		bool ReadFromURL (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary returnOptions, out NSError error);

		[Export ("readFromURL:options:documentAttributes:")]
		bool ReadFromURL (NSUrl url, NSDictionary options, out NSDictionary returnOptions);

		[Wrap ("This.ReadFromURL (url, options == null ? null : options.Dictionary, out returnOptions)")]
		bool ReadFromURL (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary returnOptions);

		[Export ("readFromData:options:documentAttributes:error:")]
		bool ReadFromData (NSData data, NSDictionary options, out NSDictionary returnOptions, out NSError error);

		[Wrap ("This.ReadFromData (data, options == null ? null : options.Dictionary, out returnOptions, out error)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary returnOptions, out NSError error);

		[Export ("readFromData:options:documentAttributes:")]
		bool ReadFromData (NSData data, NSDictionary options, out NSDictionary dict);

		[Wrap ("This.ReadFromData (data, options == null ? null : options.Dictionary, out returnOptions)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary returnOptions);

		[Export ("superscriptRange:")]
		void SuperscriptRange (NSRange range);

		[Export ("subscriptRange:")]
		void SubscriptRange (NSRange range);

		[Export ("unscriptRange:")]
		void UnscriptRange (NSRange range);

		[Export ("applyFontTraits:range:")]
		void ApplyFontTraits (NSFontTraitMask traitMask, NSRange range);

		[Export ("setAlignment:range:")]
		void SetAlignment (NSTextAlignment alignment, NSRange range);

		[Export ("setBaseWritingDirection:range:")]
		void SetBaseWritingDirection (NSWritingDirection writingDirection, NSRange range);

		[Export ("fixFontAttributeInRange:")]
		void FixFontAttributeInRange (NSRange range);
	
		[Export ("fixParagraphStyleAttributeInRange:")]
		void FixParagraphStyleAttributeInRange (NSRange range);

		[Export ("fixAttachmentAttributeInRange:")]
		void FixAttachmentAttributeInRange (NSRange range);

		[Export ("updateAttachmentsFromPath:")]
		void UpdateAttachmentsFromPath (string path);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSImageDelegate {
		[Export ("imageDidNotDraw:inRect:"), DelegateName ("NSImageRect"), DefaultValue (null)]
		NSImage ImageDidNotDraw (NSObject sender, CGRect aRect);

		[Export ("image:willLoadRepresentation:"), EventArgs ("NSImageLoad")]
		void WillLoadRepresentation (NSImage image, NSImageRep rep);

		[Export ("image:didLoadRepresentationHeader:"), EventArgs ("NSImageLoad")]
		void DidLoadRepresentationHeader (NSImage image, NSImageRep rep);

		[Export ("image:didLoadPartOfRepresentation:withValidRows:"), EventArgs ("NSImagePartial")]
		void DidLoadPartOfRepresentation (NSImage image, NSImageRep rep, nint rows);

		[Export ("image:didLoadRepresentation:withStatus:"), EventArgs ("NSImageLoadRepresentation")]
		void DidLoadRepresentation (NSImage image, NSImageRep rep, NSImageLoadStatus status);
	}

	[BaseType (typeof (NSCell))]
	interface NSImageCell {
		//Detected properties
		[Export ("imageAlignment")]
		NSImageAlignment ImageAlignment { get; set; }

		[Export ("imageScaling")]
		NSImageScale ImageScaling { get; set; }

		[Export ("imageFrameStyle")]
		NSImageFrameStyle ImageFrameStyle { get; set; }

		// Inlined from parent
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);
	}

	[Static]
	partial interface NSImageHint {
		[Field ("NSImageHintCTM")]
		NSString Ctm { get; }

		[Field ("NSImageHintInterpolation")]
		NSString Interpolation { get; }

		[Mac (10, 12)]
		[Field ("NSImageHintUserInterfaceLayoutDirection")]
		NSString UserInterfaceLayoutDirection { get; }
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSImageRep : NSCoding, NSCopying {
		[Export ("draw")]
		bool Draw ();

		[Export ("drawAtPoint:")]
		bool DrawAtPoint (CGPoint point);

		[Export ("drawInRect:")]
		bool DrawInRect (CGRect rect);

		[Export ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:")]
		bool DrawInRect (CGRect dstSpacePortionRect, CGRect srcSpacePortionRect, NSCompositingOperation op, nfloat requestedAlpha, bool respectContextIsFlipped, NSDictionary hints);

		[Export ("setAlpha:")]
		void SetAlpha (bool alpha);

		[Export ("hasAlpha")]
		bool HasAlpha { get; }

		[Static]
		[Export ("registerImageRepClass:")]
		void RegisterImageRepClass (Class imageRepClass);

		[Static]
		[Export ("unregisterImageRepClass:")]
		void UnregisterImageRepClass (Class imageRepClass);

		//[Static]
		//[Export ("registeredImageRepClasses")]
		//Class [] RegisteredImageRepClasses ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageRepClassForFileType:")]
		Class ImageRepClassForFileType (string type);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageRepClassForPasteboardType:")]
		Class ImageRepClassForPasteboardType (string type);

		[Static]
		[Export ("imageRepClassForType:")]
		Class ImageRepClassForType (string type);

		[Static]
		[Export ("imageRepClassForData:")]
		Class ImageRepClassForData (NSData data);

		[Static]
		[Export ("canInitWithData:")]
		bool CanInitWithData (NSData data);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageUnfilteredFileTypes")]
		string [] ImageUnfilteredFileTypes { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageUnfilteredPasteboardTypes")]
		string [] ImageUnfilteredPasteboardTypes { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imageFileTypes")]
		string [] ImageFileTypes { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Static]
		[Export ("imagePasteboardTypes")]
		string [] ImagePasteboardTypes { get; }

		[Static]
		[Export ("imageUnfilteredTypes", ArgumentSemantic.Copy)]
		string []ImageUnfilteredTypes { get; }

		[Static]
		[Export ("imageTypes", ArgumentSemantic.Copy)]
		string [] ImageTypes { get; }

		[Static]
		[Export ("canInitWithPasteboard:")]
		bool CanInitWithPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("imageRepsWithContentsOfFile:")]
		NSImageRep [] ImageRepsFromFile (string filename);

		[Static]
		[Export ("imageRepWithContentsOfFile:")]
		NSImageRep ImageRepFromFile (string filename);

		[Static]
		[Export ("imageRepsWithContentsOfURL:")]
		NSImageRep [] ImageRepsFromUrl (NSUrl url);

		[Static]
		[Export ("imageRepWithContentsOfURL:")]
		NSImageRep ImageRepFromUrl (NSUrl url);

		[Static]
		[Export ("imageRepsWithPasteboard:")]
		NSImageRep [] ImageRepsFromPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("imageRepWithPasteboard:")]
		NSImageRep ImageRepFromPasteboard (NSPasteboard pasteboard);

		[Export ("CGImageForProposedRect:context:hints:")]
		CGImage AsCGImage (ref CGRect proposedDestRect, [NullAllowed] NSGraphicsContext context, [NullAllowed] NSDictionary hints);

		//Detected properties
		[Export ("size")]
		CGSize Size { get; set; }

		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")]get; set; }

		[Export ("colorSpaceName")]
		string ColorSpaceName { get; set; }

		[Export ("bitsPerSample")]
		nint BitsPerSample { get; set; }

		[Export ("pixelsWide")]
		nint PixelsWide { get; set; }

		[Export ("pixelsHigh")]
		nint PixelsHigh { get; set; }

		[Mac (10, 12)]
		[Export ("layoutDirection", ArgumentSemantic.Assign)]
		NSImageLayoutDirection LayoutDirection { get; set; }
	}

	[BaseType (typeof (NSControl))]
	interface NSImageView : NSAccessibilityImage {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		//Detected properties
		[Export ("image", ArgumentSemantic.Retain)]
		NSImage Image { get; [NullAllowed] set; }

		[Export ("imageAlignment")]
		NSImageAlignment ImageAlignment { get; set; }

		[Export ("imageScaling")]
		NSImageScale ImageScaling { get; set; }

		[Export ("imageFrameStyle")]
		NSImageFrameStyle ImageFrameStyle { get; set; }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")]get; set; }

		[Export ("animates")]
		bool Animates { get; set; }

		[Export ("allowsCutCopyPaste")]
		bool AllowsCutCopyPaste { get; set; }

		[Mac (10,12)]
		[Static]
		[Export ("imageViewWithImage:")]
		NSImageView FromImage (NSImage image);
	}

	[BaseType (typeof (NSControl), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSMatrixDelegate)})]
	partial interface NSMatrix {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("initWithFrame:mode:prototype:numberOfRows:numberOfColumns:")]
		IntPtr Constructor (CGRect frameRect, NSMatrixMode aMode, NSCell aCell, nint rowsHigh, nint colsWide);

		[Export ("initWithFrame:mode:cellClass:numberOfRows:numberOfColumns:")]
		IntPtr Constructor (CGRect frameRect, NSMatrixMode aMode, Class factoryId, nint rowsHigh, nint colsWide);

		[Export ("makeCellAtRow:column:")]
		NSCell MakeCell (nint row, nint col);

		[Export ("sendAction:to:forAllCells:")]
		void SendAction (Selector aSelector, NSObject anObject, bool forAllCells);

		[Export ("cells")]
		NSCell [] Cells { get; }

		[Export ("sortUsingSelector:")]
		void Sort (Selector comparator);

		//[Export ("sortUsingFunction:context:")][Internal]
		// We need to define NSCompareFunc as:
		// (NSInteger (*)(id, id, void *))
		//void Sort (NSCompareFunc func, IntPtr context);

		[Export ("selectedCell")]
		NSCell SelectedCell { get; }

		[Export ("selectedCells")]
		NSCell [] SelectedCells { get; }

		[Export ("selectedRow")]
		nint SelectedRow { get; }

		[Export ("selectedColumn")]
		nint SelectedColumn { get; }

		[Export ("setSelectionFrom:to:anchor:highlight:")]
		void SetSelection (nint startPos, nint endPos, nint anchorPos, bool highlight);

		[Export ("deselectSelectedCell")]
		void DeselectSelectedCell ();

		[Export ("deselectAllCells")]
		void DeselectAllCells ();

		[Export ("selectCellAtRow:column:")]
		void SelectCell (nint row, nint column);

		[Export ("selectAll:")]
		void SelectAll (NSObject sender);

		[Export ("selectCellWithTag:")]
		bool SelectCellWithTag (nint tag);

		[Export ("setScrollable:")]
		void SetScrollable (bool flag);

		[Export ("setState:atRow:column:")]
		void SetState (nint state, nint row, nint column);

		[Export ("getNumberOfRows:columns:")]
		void GetRowsAndColumnsCount (out nint rowCount, out nint colCount);

		[Export ("numberOfRows")]
		nint Rows { get; }

		[Export ("numberOfColumns")]
		nint Columns { get; }

		[Export ("cellAtRow:column:")][Internal]
		NSCell CellAtRowColumn (nint row, nint column);

		[Export ("cellFrameAtRow:column:")]
		CGRect CellFrameAtRowColumn (nint row, nint column);

		[Export ("getRow:column:ofCell:")]
		bool GetRowColumn (out nint row, out nint column, NSCell aCell);

		[Export ("getRow:column:forPoint:")]
		bool GetRowColumnForPoint (out nint row, out nint column, CGPoint aPoint);

		[Export ("renewRows:columns:")]
		void RenewRowsColumns (nint newRows, nint newCols);

		[Export ("putCell:atRow:column:")]
		void PutCell (NSCell newCell, nint row, nint column);

		[Export ("addRow")]
		void AddRow ();

		[Export ("addRowWithCells:")]
		void AddRowWithCells (NSCell [] newCells);

		[Export ("insertRow:")]
		void InsertRow (nint row);

		[Export ("insertRow:withCells:")]
		void InsertRow (nint row, NSCell [] newCells);

		[Export ("removeRow:")]
		void RemoveRow (nint row);

		[Export ("addColumn")]
		void AddColumn ();

		[Export ("addColumnWithCells:")]
		void AddColumnWithCells (NSCell [] newCells);

		[Export ("insertColumn:")]
		void InsertColumn (nint column);

		[Export ("insertColumn:withCells:")]
		void InsertColumn (nint column, NSCell [] newCells);

		[Export ("removeColumn:")]
		void RemoveColumn (nint col);

		[Export ("cellWithTag:")]
		NSCell CellWithTag (nint anInt);

		[Export ("sizeToCells")]
		void SizeToCells ();
									       
		[Export ("setValidateSize:")]
		void SetValidateSize (bool flag);

		[Export ("drawCellAtRow:column:")]
		void DrawCellAtRowColumn (nint row, nint column);

		[Export ("highlightCell:atRow:column:")]
		void HighlightCell (bool highlight, nint row, nint column);

		[Export ("scrollCellToVisibleAtRow:column:")]
		void ScrollCellToVisible (nint row, nint column);

		[Export ("mouseDownFlags")]
		nint MouseDownFlags ();

		[Export ("mouseDown:")]
		void MouseDown (NSEvent theEvent);

		[Export ("performKeyEquivalent:")]
		bool PerformKeyEquivalent (NSEvent theEvent);

		[Export ("sendAction")]
		bool SendAction ();

		[Export ("sendDoubleAction")]
		void SendDoubleAction ();

		[Export ("textShouldBeginEditing:")]
		bool ShouldBeginEditing (NSText textObject);

		[Export ("textShouldEndEditing:")]
		bool ShouldEndEditing (NSText textObject);

		[Export ("textDidBeginEditing:")]
		void DidBeginEditing (NSNotification notification);

		[Export ("textDidEndEditing:")]
		void DidEndEditing (NSNotification notification);

		[Export ("textDidChange:")]
		void Changed (NSNotification notification);

		[Export ("selectText:")]
		void SelectText (NSObject sender);

		[Export ("selectTextAtRow:column:")]
		NSObject SelectTextAtRowColumn (nint row, nint column);

		[Export ("acceptsFirstMouse:")]
		bool AcceptsFirstMouse (NSEvent theEvent);

		[Export ("resetCursorRects")]
		void ResetCursorRects ();

		[Export ("setToolTip:forCell:")]
		void SetToolTipForCell (string toolTipString, NSCell cell);

		[Export ("toolTipForCell:")]
		string ToolTipForCell (NSCell cell);

		//Detected properties
		[Export ("cellClass")]
		Class CellClass { get; set; }

		[Export ("prototype", ArgumentSemantic.Copy)]
		NSCell Prototype { get; set; }

		[Export ("mode")]
		NSMatrixMode Mode { get; set; }

		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get; set; }

		[Export ("selectionByRect")]
		bool SelectionByRect { [Bind ("isSelectionByRect")]get; set; }

		[Export ("cellSize")]
		CGSize CellSize { get; set; }

		[Export ("intercellSpacing")]
		CGSize IntercellSpacing { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("cellBackgroundColor", ArgumentSemantic.Copy)]
		NSColor CellBackgroundColor { get; set; }

		[Export ("drawsCellBackground")]
		bool DrawsCellBackground { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }

		[Export ("autosizesCells")]
		bool AutosizesCells { get; set; }

		[Export ("autoscroll")]
		bool Autoscroll { [Bind ("isAutoscroll")]get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSMatrixDelegate Delegate { get; set; }

		//Detected properties
		[Export ("tabKeyTraversesCells")]
		bool TabKeyTraversesCells { get; set; }

		[Export ("keyCell")]
		NSObject KeyCell { get; set; }
	}

	[BaseType (typeof (NSControl))]
	interface NSLevelIndicator {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("warningValue")]
		double WarningValue { get; set; }

		[Export ("criticalValue")]
		double CriticalValue { get; set; }

		[Export ("tickMarkPosition")]
		NSTickMarkPosition TickMarkPosition { get; set; }

		[Export ("numberOfTickMarks")]
		nint TickMarkCount { get; set; }

		[Export ("numberOfMajorTickMarks")]
		nint MajorTickMarkCount { get; set; }

		[Export ("tickMarkValueAtIndex:")]
		double TickMarkValueAt (nint index);

		[Export ("rectOfTickMarkAtIndex:")]
		CGRect RectOfTickMark (nint index);

		[Mac (10,10)]
		[Export ("levelIndicatorStyle")]
		NSLevelIndicatorStyle LevelIndicatorStyle { get; set; }

		[Mac (10, 13)]
		[Export ("fillColor", ArgumentSemantic.Copy)]
		NSColor FillColor { get; set; }

		[Mac (10, 13)]
		[Export ("warningFillColor", ArgumentSemantic.Copy)]
		NSColor WarningFillColor { get; set; }

		[Mac (10, 13)]
		[Export ("criticalFillColor", ArgumentSemantic.Copy)]
		NSColor CriticalFillColor { get; set; }

		[Mac (10, 13)]
		[Export ("drawsTieredCapacityLevels")]
		bool DrawsTieredCapacityLevels { get; set; }

		[Mac (10, 13)]
		[Export ("placeholderVisibility", ArgumentSemantic.Assign)]
		NSLevelIndicatorPlaceholderVisibility PlaceholderVisibility { get; set; }

		[Mac (10, 13)]
		[NullAllowed, Export ("ratingImage", ArgumentSemantic.Strong)]
		NSImage RatingImage { get; set; }

		[Mac (10, 13)]
		[NullAllowed, Export ("ratingPlaceholderImage", ArgumentSemantic.Strong)]
		NSImage RatingPlaceholderImage { get; set; }

		[Mac (10, 13)]
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }
	}

	[BaseType (typeof (NSActionCell))]
	interface NSLevelIndicatorCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("initWithLevelIndicatorStyle:")]
		IntPtr Constructor (NSLevelIndicatorStyle levelIndicatorStyle);

		[Export ("levelIndicatorStyle")]
		NSLevelIndicatorStyle LevelIndicatorStyle { get; set; }

		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("warningValue")]
		double WarningValue { get; set; }

		[Export ("criticalValue")]
		double CriticalValue { get; set; }

		[Export ("tickMarkPosition")]
		NSTickMarkPosition TickMarkPosition { get; set; }

		[Export ("numberOfTickMarks")]
		nint TickMarkCount { get; set; }

		[Export ("numberOfMajorTickMarks")]
		nint MajorTickMarkCount { get; set; }

		[Export ("rectOfTickMarkAtIndex:")]
		CGRect RectOfTickMarkAt (nint index);

		[Export ("tickMarkValueAtIndex:")]
		double TickMarkValueAt (nint index);

		[Export ("setImage:")]
		void SetImage (NSImage image);
	}
	
#if MONOMAC
	[Protocol (IsInformal = true)]
	[Mac (10, 7)]
	interface NSLayerDelegateContentsScaleUpdating {
		[Export ("layer:shouldInheritContentsScale:fromWindow:")]
		bool ShouldInheritContentsScale (CALayer layer, nfloat newScale, NSWindow fromWindow);
	}
#endif

#if XAMCORE_2_0 // NSLayoutAnchor is a generic type, which we only support in Unified (for now)
	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutAnchor<AnchorType> : NSCoding, NSCopying
	{
		[Export ("constraintEqualToAnchor:")]
		NSLayoutConstraint ConstraintEqualToAnchor (NSLayoutAnchor<AnchorType> anchor);

		[Export ("constraintGreaterThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToAnchor (NSLayoutAnchor<AnchorType> anchor);

		[Export ("constraintLessThanOrEqualToAnchor:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToAnchor (NSLayoutAnchor<AnchorType> anchor);

		// -(NSLayoutConstraint *)constraintEqualToAnchor:(NSLayoutAnchor *)anchor constant:(CGFloat)c;
		[Export ("constraintEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintEqualToAnchor (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		[Export ("constraintGreaterThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToAnchor (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		[Export ("constraintLessThanOrEqualToAnchor:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToAnchor (NSLayoutAnchor<AnchorType> anchor, nfloat constant);

		[Mac (10, 12)]
		[Export ("name")]
		string Name { get; }

		[Mac (10, 12)]
		[NullAllowed, Export ("item", ArgumentSemantic.Weak)]
		NSObject Item { get; }

		[Mac (10, 12)]
		[Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[Mac (10, 12)]
		[Export ("constraintsAffectingLayout")]
		NSLayoutConstraint[] ConstraintsAffectingLayout { get; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutXAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutXAxisAnchor
	{
		[Mac (10,12)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension GetAnchorWithOffset (NSLayoutXAxisAnchor otherAnchor);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutYAxisAnchor>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutYAxisAnchor
	{
		[Mac (10,12)]
		[Export ("anchorWithOffsetToAnchor:")]
		NSLayoutDimension GetAnchorWithOffset (NSLayoutYAxisAnchor otherAnchor);
	}

	[Mac (10,11)]
	[BaseType (typeof(NSLayoutAnchor<NSLayoutDimension>))]
	[DisableDefaultCtor] // Handle is nil
	interface NSLayoutDimension
	{
		[Export ("constraintEqualToConstant:")]
		NSLayoutConstraint ConstraintEqualToConstant (nfloat constant);

		[Export ("constraintGreaterThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToConstant (nfloat constant);

		[Export ("constraintLessThanOrEqualToConstant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToConstant (nfloat constant);

		[Export ("constraintEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier);

		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier);

		[Export ("constraintLessThanOrEqualToAnchor:multiplier:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier);

		[Export ("constraintEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);

		[Export ("constraintGreaterThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintGreaterThanOrEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);

		[Export ("constraintLessThanOrEqualToAnchor:multiplier:constant:")]
		NSLayoutConstraint ConstraintLessThanOrEqualToAnchor (NSLayoutDimension anchor, nfloat multiplier, nfloat constant);
	}
#endif // XAMCORE_2_0

	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	interface NSLayoutConstraint : NSAnimatablePropertyContainer {
		[Static]
		[Export ("constraintsWithVisualFormat:options:metrics:views:")]
		NSLayoutConstraint [] FromVisualFormat (string format, NSLayoutFormatOptions formatOptions, [NullAllowed] NSDictionary metrics, NSDictionary views);

		[Static]
		[Export ("constraintWithItem:attribute:relatedBy:toItem:attribute:multiplier:constant:")]
		NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, [NullAllowed] NSObject view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant);
		
		[Export ("priority")]
		float Priority { get; set;  } /* NSLayoutPriority = float */

		[Export ("shouldBeArchived")]
		bool ShouldBeArchived { get; set;  }

		[Export ("firstItem", ArgumentSemantic.Assign)]
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

		[Mac (10,10)]
		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Mac (10,10)]
		[Static, Export ("activateConstraints:")]
		void ActivateConstraints (NSLayoutConstraint [] constraints);

		[Mac (10,10)]
		[Static, Export ("deactivateConstraints:")]
		void DeactivateConstraints (NSLayoutConstraint [] constraints);

#if XAMCORE_2_0
		[Mac (10, 12)]
		[Export ("firstAnchor", ArgumentSemantic.Copy)]
		NSLayoutAnchor<NSObject> FirstAnchor { get; }

		[Mac (10, 12)]
		[NullAllowed, Export ("secondAnchor", ArgumentSemantic.Copy)]
		NSLayoutAnchor<NSObject> SecondAnchor { get; }
#endif

		[NullAllowed, Export ("identifier")]
		string Identifier { get; set; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSLayoutGuide : NSCoding, NSUserInterfaceItemIdentification
	{
		[Export ("frame")]
		CGRect Frame { get; }

		[NullAllowed, Export ("owningView", ArgumentSemantic.Weak)]
		NSView OwningView { get; set; }

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

		[Mac (10, 12)]
		[Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[Mac (10,12)]
		[Export ("constraintsAffectingLayoutForOrientation:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (NSLayoutConstraintOrientation orientation);
	}

	delegate void NSTextLayoutEnumerateLineFragments (CGRect rect, CGRect usedRectangle, NSTextContainer textContainer, NSRange glyphRange, out bool stop);
	delegate void NSTextLayoutEnumerateEnclosingRects (CGRect rect, out bool stop);
	
	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSLayoutManager : NSCoding {
		[Export ("attributedString")]
		NSAttributedString AttributedString { get; }

		[Export ("replaceTextStorage:")]
		void ReplaceTextStorage (NSTextStorage newTextStorage);

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

		[Export ("textContainerChangedTextView:")]
		void TextContainerChangedTextView (NSTextContainer container);

		[Export ("layoutOptions")]
		NSGlyphStorageOptions LayoutOptions { get; }

		[Export ("hasNonContiguousLayout")]
		bool HasNonContiguousLayout { get; }

		//[Export ("invalidateGlyphsForCharacterRange:changeInLength:actualCharacterRange:")]
		//void InvalidateGlyphs (NSRange charRange, int changeInLength, NSRangePointer actualCharRange);

		//[Export ("invalidateLayoutForCharacterRange:actualCharacterRange:")]
		//void InvalidateLayout (NSRange charRange, NSRangePointer actualCharRange);

		//[Export ("invalidateLayoutForCharacterRange:isSoft:actualCharacterRange:")]
		//void InvalidateLayout (NSRange charRange, bool isSoft, NSRangePointer actualCharRange);

		[Export ("invalidateDisplayForCharacterRange:")]
		void InvalidateDisplayForCharacterRange (NSRange charRange);

		[Export ("invalidateDisplayForGlyphRange:")]
		void InvalidateDisplayForGlyphRange (NSRange glyphRange);

#if !XAMCORE_4_0
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharRange, nint delta, NSRange invalidatedCharRange) instead).")]
		[Export ("textStorage:edited:range:changeInLength:invalidatedRange:")]
		void TextStorageEdited (NSTextStorage str, NSTextStorageEditedFlags editedMask, NSRange newCharRange, nint changeInLength, NSRange invalidatedCharRange);
#endif

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

		//[Export ("insertGlyphs:length:forStartingGlyphAtIndex:characterIndex:")]
		//void InsertGlyphs (uint [] glyphs, int length, int glyphIndex, int charIndex);

		[Export ("insertGlyph:atGlyphIndex:characterIndex:")]
		void InsertGlyph (uint /* NSGlyph = unsigned int */ glyph, nint glyphIndex, nint charIndex);

		[Export ("replaceGlyphAtIndex:withGlyph:")]
		void ReplaceGlyphAtIndex (nint glyphIndex, uint /* NSGlyph = unsigned int */ newGlyph);

		[Export ("deleteGlyphsInRange:")]
		void DeleteGlyphs (NSRange glyphRange);

		[Export ("setCharacterIndex:forGlyphAtIndex:")]
		void SetCharacterIndex (nint charIndex, nint glyphIndex);

		[Export ("setIntAttribute:value:forGlyphAtIndex:")]
		void SetIntAttribute (nint attributeTag, nint value, nint glyphIndex);

		[Export ("invalidateGlyphsOnLayoutInvalidationForGlyphRange:")]
		void InvalidateGlyphsOnLayoutInvalidation (NSRange glyphRange);

		[Export ("numberOfGlyphs")]
		nint NumberOfGlyphs { get; }

		[Export ("glyphAtIndex:isValidIndex:")]
#if XAMCORE_2_0
		uint /* NSGlyph = unsigned int */ GlyphAtIndex (nint glyphIndex, ref bool isValidIndex);
#else
		[Obsolete ("Use 'GlyphAtIndex' instead.")]
		uint /* NSGlyph = unsigned int */ GlyphAtIndexisValidIndex (nuint glyphIndex, ref bool isValidIndex);
#endif

		[Export ("glyphAtIndex:")]
#if XAMCORE_2_0
		uint /* NSGlyph = unsigned int */ GlyphAtIndex (nint glyphIndex);
#else
		[Obsolete ("Use 'GlyphAtIndex' instead.")]
		uint /* NSGlyph = unsigned int */ GlyphCount (nint glyphIndex);
#endif

		[Export ("isValidGlyphIndex:")]
		bool IsValidGlyphIndex (nint glyphIndex);

		[Export ("characterIndexForGlyphAtIndex:")]
		nuint CharacterIndexForGlyphAtIndex (nint glyphIndex);

		[Export ("glyphIndexForCharacterAtIndex:")]
		nuint GlyphIndexForCharacterAtIndex (nint charIndex);

		[Export ("intAttribute:forGlyphAtIndex:")]
		nint GetIntAttribute (nint attributeTag, nint glyphIndex);

		[Export ("setTextContainer:forGlyphRange:")]
		void SetTextContainerForRange (NSTextContainer container, NSRange glyphRange);

		[Export ("setLineFragmentRect:forGlyphRange:usedRect:")]
		void SetLineFragmentRect (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect);

		[Export ("setExtraLineFragmentRect:usedRect:textContainer:")]
		void SetExtraLineFragmentRect (CGRect fragmentRect, CGRect usedRect, NSTextContainer container);

		[Export ("setLocation:forStartOfGlyphRange:")]
		void SetLocation (CGPoint location, NSRange forStartOfGlyphRange);

		//[Export ("setLocations:startingGlyphIndexes:count:forGlyphRange:")]
		//void SetLocations (NSPointArray locations, int glyphIndexes, uint count, NSRange glyphRange);

		[Export ("setNotShownAttribute:forGlyphAtIndex:")]
		void SetNotShownAttribute (bool flag, nint glyphIndex);

		[Export ("setDrawsOutsideLineFragment:forGlyphAtIndex:")]
		void SetDrawsOutsideLineFragment (bool flag, nint glyphIndex);

		[Export ("setAttachmentSize:forGlyphRange:")]
		void SetAttachmentSize (CGSize attachmentSize, NSRange glyphRange);

		[Export ("getFirstUnlaidCharacterIndex:glyphIndex:")]
		void GetFirstUnlaidCharacterIndex (ref nuint charIndex, ref nuint glyphIndex);

		[Export ("firstUnlaidCharacterIndex")]
		nint FirstUnlaidCharacterIndex { get; }

		[Export ("firstUnlaidGlyphIndex")]
		nint FirstUnlaidGlyphIndex { get; }

		//[Export ("textContainerForGlyphAtIndex:effectiveRange:")]
		//NSTextContainer TextContainerForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange);

		[Export ("usedRectForTextContainer:")]
		CGRect GetUsedRectForTextContainer (NSTextContainer container);

		//[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:")]
		//CGRect LineFragmentRectForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange);

		//[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:")]
		//CGRect LineFragmentUsedRectForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange);

		//[Export ("lineFragmentRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		//CGRect LineFragmentRectForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange, bool flag);

		//[Export ("lineFragmentUsedRectForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		//CGRect LineFragmentUsedRectForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange, bool flag);

		//[Export ("textContainerForGlyphAtIndex:effectiveRange:withoutAdditionalLayout:")]
		//NSTextContainer TextContainerForGlyphAt (int glyphIndex, NSRangePointer effectiveGlyphRange, bool flag);

		[Export ("extraLineFragmentRect")]
		CGRect ExtraLineFragmentRect { get; }

		[Export ("extraLineFragmentUsedRect")]
		CGRect ExtraLineFragmentUsedRect { get; }

		[Export ("extraLineFragmentTextContainer")]
		NSTextContainer ExtraLineFragmentTextContainer { get; }

		[Export ("locationForGlyphAtIndex:")]
		CGPoint LocationForGlyphAtIndex (nint glyphIndex);

		[Export ("notShownAttributeForGlyphAtIndex:")]
		bool NotShownAttributeForGlyphAtIndex (nint glyphIndex);

		[Export ("drawsOutsideLineFragmentForGlyphAtIndex:")]
		bool DrawsOutsideLineFragmentForGlyphAt (nint glyphIndex);

		[Export ("attachmentSizeForGlyphAtIndex:")]
		CGSize AttachmentSizeForGlyphAt (nint glyphIndex);

		[Export ("setLayoutRect:forTextBlock:glyphRange:")]
		void SetLayoutRect (CGRect layoutRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[Export ("setBoundsRect:forTextBlock:glyphRange:")]
		void SetBoundsRect (CGRect boundsRect, NSTextBlock forTextBlock, NSRange glyphRange);

		[Export ("layoutRectForTextBlock:glyphRange:")]
		CGRect LayoutRect (NSTextBlock block, NSRange glyphRange);

		[Export ("boundsRectForTextBlock:glyphRange:")]
		CGRect BoundsRect (NSTextBlock block, NSRange glyphRange);

		//[Export ("layoutRectForTextBlock:atIndex:effectiveRange:")]
		//CGRect LayoutRect (NSTextBlock block, int glyphIndex, NSRangePointer effectiveGlyphRange);

		//[Export ("boundsRectForTextBlock:atIndex:effectiveRange:")]
		//CGRect BoundsRect (NSTextBlock block, int glyphIndex, NSRangePointer effectiveGlyphRange);

		//[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		//NSRange GetGlyphRange (NSRange charRange, NSRangePointer actualCharRange);

		//[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		//NSRange GetCharacterRange (NSRange glyphRange, NSRangePointer actualGlyphRange);

		[Export ("glyphRangeForTextContainer:")]
		NSRange GetGlyphRange (NSTextContainer container);

		[Export ("rangeOfNominallySpacedGlyphsContainingIndex:")]
		NSRange RangeOfNominallySpacedGlyphsContainingIndex (nint glyphIndex);

		//[Export ("rectArrayForCharacterRange:withinSelectedCharacterRange:inTextContainer:rectCount:")]
		//NSRectArray RectArrayForCharacterRangewithinSelectedCharacterRangeinTextContainerrectCount (NSRange charRange, NSRange selCharRange, NSTextContainer container, uint rectCount);

		[Internal]
		[Export ("rectArrayForGlyphRange:withinSelectedGlyphRange:inTextContainer:rectCount:")]
		[Availability (Deprecated = Platform.Mac_10_11)]
		IntPtr GetRectArray (NSRange glyphRange, NSRange selectedGlyphRange, IntPtr textContainerHandle, out nuint rectCount);

		[Export ("boundingRectForGlyphRange:inTextContainer:")]
		CGRect BoundingRectForGlyphRange (NSRange glyphRange, NSTextContainer container);

		[Export ("glyphRangeForBoundingRect:inTextContainer:")]
		NSRange GlyphRangeForBoundingRect (CGRect bounds, NSTextContainer container);

		[Export ("glyphRangeForBoundingRectWithoutAdditionalLayout:inTextContainer:")]
		NSRange GlyphRangeForBoundingRectWithoutAdditionalLayout (CGRect bounds, NSTextContainer container);

		[Export ("glyphIndexForPoint:inTextContainer:fractionOfDistanceThroughGlyph:")]
		nuint GlyphIndexForPointInTextContainer (CGPoint point, NSTextContainer container, ref nfloat fractionOfDistanceThroughGlyph);

		[Export ("glyphIndexForPoint:inTextContainer:")]
		nuint GlyphIndexForPoint (CGPoint point, NSTextContainer container);

		[Export ("fractionOfDistanceThroughGlyphForPoint:inTextContainer:")]
		nfloat FractionOfDistanceThroughGlyphForPoint (CGPoint point, NSTextContainer container);

		[Export ("characterIndexForPoint:inTextContainer:fractionOfDistanceBetweenInsertionPoints:")]
		nuint CharacterIndexForPoint (CGPoint point, NSTextContainer container, ref nfloat fractionOfDistanceBetweenInsertionPoints);

		[Export ("getLineFragmentInsertionPointsForCharacterAtIndex:alternatePositions:inDisplayOrder:positions:characterIndexes:")]
		nuint GetLineFragmentInsertionPoints (nuint charIndex, bool aFlag, bool dFlag, IntPtr positions, IntPtr charIndexes);

		//[Export ("temporaryAttributesAtCharacterIndex:effectiveRange:")]
		//NSDictionary GetTemporaryAttributes (int charIndex, NSRangePointer effectiveCharRange);

		[Export ("setTemporaryAttributes:forCharacterRange:")]
		void SetTemporaryAttributes (NSDictionary attrs, NSRange charRange);

		[Export ("addTemporaryAttributes:forCharacterRange:")]
		void AddTemporaryAttributes (NSDictionary attrs, NSRange charRange);

		[Export ("removeTemporaryAttribute:forCharacterRange:")]
		void RemoveTemporaryAttribute (string attrName, NSRange charRange);

		//[Export ("temporaryAttribute:atCharacterIndex:effectiveRange:")]
		//NSObject GetTemporaryAttribute (string attrName, uint location, NSRangePointer range);

		//[Export ("temporaryAttribute:atCharacterIndex:longestEffectiveRange:inRange:")]
		//NSObject GetTemporaryAttribute (string attrName, uint location, NSRangePointer range, NSRange rangeLimit);

		//[Export ("temporaryAttributesAtCharacterIndex:longestEffectiveRange:inRange:")]
		//NSDictionary GetTemporaryAttributes (int characterIndex, NSRangePointer longestEffectiveRange, NSRange rangeLimit);

		[Export ("addTemporaryAttribute:value:forCharacterRange:")]
		void AddTemporaryAttribute (string attrName, NSObject value, NSRange charRange);

		[Export ("substituteFontForFont:")]
		NSFont SubstituteFontForFont (NSFont originalFont);

		[Export ("defaultLineHeightForFont:")]
		nfloat DefaultLineHeightForFont (NSFont theFont);

		[Export ("defaultBaselineOffsetForFont:")]
		nfloat DefaultBaselineOffsetForFont (NSFont theFont);

		//Detected properties
		[Export ("textStorage")]
		NSTextStorage TextStorage { get; set; }

		[Export ("glyphGenerator", ArgumentSemantic.Retain)]
		NSGlyphGenerator GlyphGenerator { get; set; }

		[Export ("typesetter", ArgumentSemantic.Retain)]
		NSTypesetter Typesetter { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSLayoutManagerDelegate Delegate { get; set; }

		[Export ("backgroundLayoutEnabled")]
		bool BackgroundLayoutEnabled { get; set; }

		[Export ("usesScreenFonts")]
		bool UsesScreenFonts { get; set; }

		[Export ("showsInvisibleCharacters")]
		bool ShowsInvisibleCharacters { get; set; }

		[Export ("showsControlCharacters")]
		bool ShowsControlCharacters { get; set; }

		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; set; } /* float, not CGFloat */

		[Export ("defaultAttachmentScaling")]
		NSImageScaling DefaultAttachmentScaling { get; set; }

		[Export ("typesetterBehavior")]
		NSTypesetterBehavior TypesetterBehavior { get; set; }

		[Export ("allowsNonContiguousLayout")]
		bool AllowsNonContiguousLayout { get; set; }

		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }

		[Export ("drawBackgroundForGlyphRange:atPoint:")]
		void DrawBackgroundForGlyphRange (NSRange glyphsToShow, CGPoint origin);

		[Export ("drawGlyphsForGlyphRange:atPoint:")]
		void DrawGlyphsForGlyphRange (NSRange glyphsToShow, CGPoint origin);

		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange CharacterRangeForGlyphRange (NSRange glyphRange, out NSRange actualGlyphRange);

		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GlyphRangeForCharacterRange (NSRange charRange, out NSRange actualCharRange);

		[Mac (10,10)]
		[Export ("getGlyphsInRange:glyphs:properties:characterIndexes:bidiLevels:")]
		[Internal]
		nuint GetGlyphsInternal (NSRange glyphRange, IntPtr glyphBuffer, IntPtr props, IntPtr charIndexBuffer, IntPtr bidiLevelBuffer);

		[Mac (10,10)]
		[Export ("propertyForGlyphAtIndex:")]
		NSGlyphProperty GetProperty (nuint glyphIndex);

		[Mac (10,11)]
		[Export ("CGGlyphAtIndex:isValidIndex:")]
		ushort GetCGGlyph (nuint glyphIndex, out bool isValidIndex);

		[Mac (10,11)]
		[Export ("CGGlyphAtIndex:")]
		ushort GetCGGlyph (nuint glyphIndex);

		[Mac (10,11)]
		[Export ("processEditingForTextStorage:edited:range:changeInLength:invalidatedRange:")]
		void ProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editMask, NSRange newCharRange, nint delta, NSRange invalidatedCharRange);

		[Mac (10,11)]
		[Export ("setGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		void SetGlyphs (IntPtr glyphs, IntPtr props, IntPtr charIndexes, NSFont aFont, NSRange glyphRange);

		[Mac (10,11)]
		[Export ("truncatedGlyphRangeInLineFragmentForGlyphAtIndex:")]
		NSRange GetTruncatedGlyphRangeInLineFragment (nuint glyphIndex);

		[Mac (10,11)]
		[Export ("enumerateLineFragmentsForGlyphRange:usingBlock:")]
		void EnumerateLineFragments (NSRange glyphRange, NSTextLayoutEnumerateLineFragments callback);

		[Mac (10,11)]
		[Export ("enumerateEnclosingRectsForGlyphRange:withinSelectedGlyphRange:inTextContainer:usingBlock:")]
		void EnumerateEnclosingRects (NSRange glyphRange, NSRange selectedRange, NSTextContainer textContainer, NSTextLayoutEnumerateEnclosingRects callback);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSLayoutManagerDelegate {
		[Export ("layoutManagerDidInvalidateLayout:")]
		void LayoutInvalidated (NSLayoutManager sender);

		[Export ("layoutManager:didCompleteLayoutForTextContainer:atEnd:")]
		void LayoutCompleted (NSLayoutManager layoutManager, NSTextContainer textContainer, bool layoutFinishedFlag);

		[Export ("layoutManager:shouldUseTemporaryAttributes:forDrawingToScreen:atCharacterIndex:effectiveRange:")]
		NSDictionary ShouldUseTemporaryAttributes (NSLayoutManager layoutManager, NSDictionary temporaryAttributes, bool drawingToScreen, nint charIndex, IntPtr effectiveCharRange);

		[Mac (10,11)]
		[Export ("layoutManager:shouldGenerateGlyphs:properties:characterIndexes:font:forGlyphRange:")]
		nuint ShouldGenerateGlyphs (NSLayoutManager layoutManager, IntPtr glyphBuffer, IntPtr props, IntPtr charIndexes, NSFont aFont, NSRange glyphRange);

		[Mac (10,11)]
		[Export ("layoutManager:lineSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetLineSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		[Mac (10,11)]
		[Export ("layoutManager:paragraphSpacingBeforeGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingBeforeGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		[Mac (10,11)]
		[Export ("layoutManager:paragraphSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingAfterGlyph (NSLayoutManager layoutManager, nuint glyphIndex, CGRect rect);

		[Mac (10,11)]
		[Export ("layoutManager:shouldUseAction:forControlCharacterAtIndex:")]
		NSControlCharacterAction ShouldUseAction (NSLayoutManager layoutManager, NSControlCharacterAction action, nuint charIndex);

		[Mac (10,11)]
		[Export ("layoutManager:shouldBreakLineByWordBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByWordBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);

		[Mac (10,11)]
		[Export ("layoutManager:shouldBreakLineByHyphenatingBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByHyphenatingBeforeCharacter (NSLayoutManager layoutManager, nuint charIndex);

		[Mac (10,11)]
		[Export ("layoutManager:boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetBoundingBox (NSLayoutManager layoutManager, nuint glyphIndex, NSTextContainer textContainer, CGRect proposedRect, CGPoint glyphPosition, nuint charIndex);

		[Mac (10,11)]
		[Export ("layoutManager:textContainer:didChangeGeometryFromSize:")]
		void DidChangeGeometry (NSLayoutManager layoutManager, NSTextContainer textContainer, CGSize oldSize);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSGestureRecognizer))]
	interface NSMagnificationGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("magnification")]
		nfloat Magnification { get; set; }
	}

	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
#if XAMCORE_2_0
	interface NSMatrixDelegate : NSControlTextEditingDelegate {
#else
	interface NSMatrixDelegate {
		[Export ("control:textShouldBeginEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldBeginEditing (NSControl control, NSText fieldEditor);

		[Export ("control:textShouldEndEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldEndEditing (NSControl control, NSText fieldEditor);

		[Export ("control:didFailToFormatString:errorDescription:"), DelegateName ("NSControlTextError"), DefaultValue (true)]
		bool DidFailToFormatString (NSControl control, string str, string error);
		
		[Export ("control:didFailToValidatePartialString:errorDescription:"), EventArgs ("NSControlTextError")]
		void DidFailToValidatePartialString (NSControl control, string str, string error);
		
		[Export ("control:isValidObject:"), DelegateName ("NSControlTextValidation"), DefaultValue (true)]
		bool IsValidObject (NSControl control, NSObject objectToValidate);

		[Export ("control:textView:doCommandBySelector:"), DelegateName ("NSControlCommand"), DefaultValue (false)]
		bool DoCommandBySelector (NSControl control, NSTextView textView, Selector commandSelector);

		[Export ("control:textView:completions:forPartialWordRange:indexOfSelectedItem:"), DelegateName ("NSControlTextCompletion"), DefaultValue (null)]
		string [] GetCompletions (NSControl control, NSTextView textView, string [] words, NSRange charRange, ref nint index);
#endif
	}

	[Model]
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface NSControlTextEditingDelegate {
		[Export ("control:textShouldBeginEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldBeginEditing (NSControl control, NSText fieldEditor);

		[Export ("control:textShouldEndEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldEndEditing (NSControl control, NSText fieldEditor);

		[Export ("control:didFailToFormatString:errorDescription:"), DelegateName ("NSControlTextError"), DefaultValue (true)]
		bool DidFailToFormatString (NSControl control, string str, string error);

		[Export ("control:didFailToValidatePartialString:errorDescription:"), EventArgs ("NSControlTextError")]
		void DidFailToValidatePartialString (NSControl control, string str, string error);

		[Export ("control:isValidObject:"), DelegateName ("NSControlTextValidation"), DefaultValue (true)]
		bool IsValidObject (NSControl control, NSObject objectToValidate);

		[Export ("control:textView:doCommandBySelector:"), DelegateName ("NSControlCommand"), DefaultValue (false)]
		bool DoCommandBySelector (NSControl control, NSTextView textView, Selector commandSelector);

		[Export ("control:textView:completions:forPartialWordRange:indexOfSelectedItem:"), DelegateName ("NSControlTextCompletion"), DefaultValue (null)]
		string [] GetCompletions (NSControl control, NSTextView textView, string [] words, NSRange charRange, ref nint index);
	}

	[BaseType (typeof (NSObject))]
	[Dispose ("__mt_accessory_var = null;")]
	interface NSPageLayout {
		[Static]
		[Export ("pageLayout")]
		NSPageLayout PageLayout { get; }

		[Export ("addAccessoryController:")]
		[PostSnippet ("__mt_accessory_var = AccessoryControllers();")]
		void AddAccessoryController (NSViewController accessoryController);

		[Export ("removeAccessoryController:")]
		[PostSnippet ("__mt_accessory_var = AccessoryControllers();")]
		void RemoveAccessoryController (NSViewController accessoryController);

		[Export ("accessoryControllers")]
		NSViewController [] AccessoryControllers ();

		[Export ("beginSheetWithPrintInfo:modalForWindow:delegate:didEndSelector:contextInfo:")]
		void BeginSheet (NSPrintInfo printInfo, NSWindow docWindow, [NullAllowed] NSObject del, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);

		[Export ("runModalWithPrintInfo:")]
		nint RunModalWithPrintInfo (NSPrintInfo printInfo);

		[Export ("runModal")]
		nint RunModal ();

		[Export ("printInfo")]
		NSPrintInfo PrintInfo { get; }
	}

	[BaseType (typeof (NSWindow))]
	interface NSPanel {
		//Detected properties
		[Export ("floatingPanel")]
		bool FloatingPanel { [Bind ("isFloatingPanel")]get; set; }

		[Export ("becomesKeyOnlyIfNeeded")]
		bool BecomesKeyOnlyIfNeeded { get; set; }

		[Export ("worksWhenModal")]
		bool WorksWhenModal { get; set; }

		[Export ("initWithContentRect:styleMask:backing:defer:")]
		IntPtr Constructor (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation);
	}

	[BaseType (typeof (NSObject))]
	interface NSParagraphStyle : NSSecureCoding, NSMutableCopying {
		[Static]
		[Export ("defaultParagraphStyle", ArgumentSemantic.Copy)]
		NSParagraphStyle DefaultParagraphStyle { get; [NotImplemented] set; }

		[Static]
		[Export ("defaultWritingDirectionForLanguage:")]
		NSWritingDirection DefaultWritingDirection (string languageName);

		[Export ("lineSpacing")]
		nfloat LineSpacing { get; [NotImplemented] set; }

		[Export ("paragraphSpacing")]
		nfloat ParagraphSpacing { get; [NotImplemented] set; }

		[Export ("alignment")]
		NSTextAlignment Alignment { get; [NotImplemented] set; }

		[Export ("headIndent")]
		nfloat HeadIndent { get; [NotImplemented] set; }

		[Export ("tailIndent")]
		nfloat TailIndent { get; [NotImplemented] set; }

		[Export ("firstLineHeadIndent")]
		nfloat FirstLineHeadIndent { get; [NotImplemented] set; }

		[Export ("tabStops")]
		NSTextTab [] TabStops { get; [NotImplemented] set; }

		[Export ("minimumLineHeight")]
		nfloat MinimumLineHeight { get; [NotImplemented] set; }

		[Export ("maximumLineHeight")]
		nfloat MaximumLineHeight { get; [NotImplemented] set; }

		[Export ("lineBreakMode")]
		NSLineBreakMode LineBreakMode { get; [NotImplemented] set; }

		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; [NotImplemented] set; }

		[Export ("lineHeightMultiple")]
		nfloat LineHeightMultiple { get; [NotImplemented] set; }

		[Export ("paragraphSpacingBefore")]
		nfloat ParagraphSpacingBefore { get; [NotImplemented] set; }

		[Export ("defaultTabInterval")]
		nfloat DefaultTabInterval { get; [NotImplemented] set; }

		[Export ("textBlocks")]
		NSTextTableBlock [] TextBlocks { get; [NotImplemented] set; }

		[Export ("textLists")]
		NSTextList[] TextLists { get; [NotImplemented] set; }

		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; [NotImplemented] set; } /* float, not CGFloat */

		[Export ("tighteningFactorForTruncation")]
		float TighteningFactorForTruncation { get; [NotImplemented] set; } /* float, not CGFloat */

		[Export ("headerLevel")]
		nint HeaderLevel { get; [NotImplemented] set; }

		// @property (readonly) BOOL allowsDefaultTighteningForTruncation __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; }
	}

	[BaseType (typeof (NSParagraphStyle))]
	interface NSMutableParagraphStyle {

		[Export ("addTabStop:")]
		[PostGet ("TabStops")]
		void AddTabStop (NSTextTab anObject);

		[Export ("removeTabStop:")]
		[PostGet ("TabStops")]
		void RemoveTabStop (NSTextTab anObject);

		[Export ("tabStops", ArgumentSemantic.Copy)]
		[Override]
		NSTextTab [] TabStops { get; set; }

		[Export ("setParagraphStyle:")]
		void SetParagraphStyle (NSParagraphStyle obj);

		[Export ("defaultTabInterval")]
		[Override]
		nfloat DefaultTabInterval { get; set; }

		[Export ("setTextBlocks:")]
		void SetTextBlocks (NSTextBlock [] array);

		[Export ("setTextLists:")]
		void SetTextLists (NSTextList [] array);

		[Export ("tighteningFactorForTruncation")]
		[Override]
		float TighteningFactorForTruncation { get; set; } /* float, not CGFloat */

		[Export ("headerLevel")]
		[Override]
		nint HeaderLevel { get; set; }

		[Export ("lineSpacing")]
		[Override]
		nfloat LineSpacing { get; set; }

		[Export ("alignment")]
		[Override]
		NSTextAlignment Alignment { get; set; }

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
		NSLineBreakMode LineBreakMode { get; set; }

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
		float HyphenationFactor { get; set; } /* float, not CGFloat */

		[Mac (10,11)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

	}

	[Mac (10,10)]
	[BaseType (typeof (NSGestureRecognizer))]
	interface NSPanGestureRecognizer : NSCoding {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("buttonMask")]
		nuint ButtonMask { get; set; }

		[Export ("translationInView:")]
		CGPoint TranslationInView (NSView view);

		[Export ("setTranslation:inView:")]
		void SetTranslation (CGPoint translation, NSView view);

		[Export ("velocityInView:")]
		CGPoint VelocityInView (NSView view);

		[Mac (10, 12, 2)]
		[Export ("numberOfTouchesRequired")]
		nint NumberOfTouchesRequired { get; set; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSGestureRecognizer))]
	interface NSPressGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("buttonMask")]
		nuint ButtonMask { get; set; }

		[Export ("minimumPressDuration")]
		double MinimumPressDuration { get; set; }

		[Export ("allowableMovement")]
		nfloat AllowableMovement { get; set; }

		[Mac (10, 12, 2)]
		[Export ("numberOfTouchesRequired")]
		nint NumberOfTouchesRequired { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: +[NSPasteboard alloc]: unrecognized selector sent to class 0xac3dcbf0
	partial interface NSPasteboard // NSPasteboard does _not_ implement NSPasteboardReading/NSPasteboardWriting
	{
		[Static]
		[Export ("generalPasteboard")]
		NSPasteboard GeneralPasteboard { get; }

		[Static]
		[Export ("pasteboardWithName:")]
		NSPasteboard FromName (string name);

		[Static]
		[Export ("pasteboardWithUniqueName")]
		NSPasteboard CreateWithUniqueName ();

		[Export ("name")]
		string Name { get; }

		[Export ("changeCount")]
		nint ChangeCount { get; }

		[Export ("releaseGlobally")]
		void ReleaseGlobally ();

		[Export ("clearContents")]
		nint ClearContents ();

		// We have to support the backwards WriteObjects (NSPasteboardReading [] objects) so we just pass the handle.
		[Export ("writeObjects:")]
		[Internal]
		bool WriteObjects (IntPtr objects);

		[Export ("readObjectsForClasses:options:")]
		NSObject [] ReadObjectsForClasses (Class [] classArray, [NullAllowed] NSDictionary options);

		[Export ("pasteboardItems")]
		NSPasteboardItem [] PasteboardItems { get; }

		[Export ("indexOfPasteboardItem:")]
		nint IndexOf (NSPasteboardItem pasteboardItem);

		[Export ("canReadItemWithDataConformingToTypes:")]
		bool CanReadItemWithDataConformingToTypes (string [] utiTypes);

		[Export ("canReadObjectForClasses:options:")]
		bool CanReadObjectForClasses (Class [] classArray, [NullAllowed] NSDictionary options);

		[Export ("declareTypes:owner:")]
		nint DeclareTypes (string [] newTypes, [NullAllowed] NSObject newOwner);

		[Export ("addTypes:owner:")]
		nint AddTypes (string [] newTypes, [NullAllowed] NSObject newOwner);

		[Export ("types")]
		string [] Types { get; }

		[Export ("availableTypeFromArray:")]
		string GetAvailableTypeFromArray (string [] types);

		[Export ("setData:forType:")]
		bool SetDataForType (NSData data, string dataType);

		[Export ("setPropertyList:forType:")]
		bool SetPropertyListForType (NSObject plist, string dataType);

		[Export ("setString:forType:")]
		bool SetStringForType (string str, string dataType);

		[Export ("dataForType:")]
		NSData GetDataForType (string dataType);

		[Export ("propertyListForType:")]
		NSObject GetPropertyListForType (string dataType);

		[Export ("stringForType:")]
		string GetStringForType (string dataType);

		// Pasteboard data types

		[Field ("NSStringPboardType")]
		NSString NSStringType{ get; }
		
		[Field ("NSFilenamesPboardType")]
		NSString NSFilenamesType{ get; }
		
		[Field ("NSPostScriptPboardType")]
		NSString NSPostScriptType{ get; }

		[Field ("NSTIFFPboardType")]
		NSString NSTiffType{ get; }
		
		[Field ("NSRTFPboardType")]
		NSString NSRtfType{ get; }
		
		[Field ("NSTabularTextPboardType")]
		NSString NSTabularTextType{ get; }
		
		[Field ("NSFontPboardType")]
		NSString NSFontType{ get; }
		
		[Field ("NSRulerPboardType")]
		NSString NSRulerType{ get; }
		
		[Field ("NSFileContentsPboardType")]
		NSString NSFileContentsType{ get; }
		
		[Field ("NSColorPboardType")]
		NSString NSColorType{ get; }
		
		[Field ("NSRTFDPboardType")]
		NSString NSRtfdType{ get; }
		
		[Field ("NSHTMLPboardType")]
		NSString NSHtmlType{ get; }
		
		[Field ("NSPICTPboardType")]
		NSString NSPictType{ get; }
		
		[Field ("NSURLPboardType")]
		NSString NSUrlType{ get; }
		
		[Field ("NSPDFPboardType")]
		NSString NSPdfType{ get; }
		
		[Field ("NSVCardPboardType")]
		NSString NSVCardType{ get; }
		
		[Field ("NSFilesPromisePboardType")]
		NSString NSFilesPromiseType{ get; }
		
		[Field ("NSMultipleTextSelectionPboardType")]
		NSString NSMultipleTextSelectionType{ get; }

		// Pasteboard names: for NSPasteboard.FromName()

		[Field ("NSGeneralPboard")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSPasteboardNameGeneral' instead.")]
		NSString NSGeneralPasteboardName { get; }

		[Field ("NSFontPboard")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSPasteboardNameFont' instead.")]
		NSString NSFontPasteboardName { get; }

		[Field ("NSRulerPboard")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSPasteboardNameRuler' instead.")]
		NSString NSRulerPasteboardName { get; }

		[Field ("NSFindPboard")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSPasteboardNameFind' instead.")]
		NSString NSFindPasteboardName { get; }

		[Field ("NSDragPboard")]
		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSPasteboardNameDrag' instead.")]
		NSString NSDragPasteboardName { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardNameGeneral")]
		NSString NSPasteboardNameGeneral { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardNameFont")]
		NSString NSPasteboardNameFont { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardNameRuler")]
		NSString NSPasteboardNameRuler { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardNameFind")]
		NSString NSPasteboardNameFind { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardNameDrag")]
		NSString NSPasteboardNameDrag { get; }

		[Field ("NSPasteboardTypeString")]
		NSString NSPasteboardTypeString { get; }

		[Field ("NSPasteboardTypePDF")]
		NSString NSPasteboardTypePDF { get; }

		[Field ("NSPasteboardTypeTIFF")]
		NSString NSPasteboardTypeTIFF { get; }

		[Field ("NSPasteboardTypePNG")]
		NSString NSPasteboardTypePNG { get; }

		[Field ("NSPasteboardTypeRTF")]
		NSString NSPasteboardTypeRTF { get; }

		[Field ("NSPasteboardTypeRTFD")]
		NSString NSPasteboardTypeRTFD { get; }

		[Field ("NSPasteboardTypeHTML")]
		NSString NSPasteboardTypeHTML { get; }

		[Field ("NSPasteboardTypeTabularText")]
		NSString NSPasteboardTypeTabularText { get; }

		[Field ("NSPasteboardTypeFont")]
		NSString NSPasteboardTypeFont { get; }

		[Field ("NSPasteboardTypeRuler")]
		NSString NSPasteboardTypeRuler { get; }

		[Field ("NSPasteboardTypeColor")]
		NSString NSPasteboardTypeColor { get; }

		[Field ("NSPasteboardTypeSound")]
		NSString NSPasteboardTypeSound { get; }

		[Field ("NSPasteboardTypeMultipleTextSelection")]
		NSString NSPasteboardTypeMultipleTextSelection { get; }

		[Field ("NSPasteboardTypeFindPanelSearchOptions")]
		NSString NSPasteboardTypeFindPanelSearchOptions { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardTypeURL")]
		NSString NSPasteboardTypeUrl { get; }

		[Mac (10, 13)]
		[Field ("NSPasteboardTypeFileURL")]
		NSString NSPasteboardTypeFileUrl { get; }

		[Mac (10,12)]
		[Export ("prepareForNewContentsWithOptions:")]
		nint PrepareForNewContents (NSPasteboardContentsOptions options);
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPasteboardWriting {
		[Export ("writableTypesForPasteboard:")]
		string [] GetWritableTypesForPasteboard (NSPasteboard pasteboard);

		[Export ("writingOptionsForType:pasteboard:")]
		NSPasteboardWritingOptions GetWritingOptionsForType (string type, NSPasteboard pasteboard);

		[Export ("pasteboardPropertyListForType:")]
		NSObject GetPasteboardPropertyListForType (string type);
	}

	[BaseType (typeof (NSObject))]
	interface NSPasteboardItem : NSPasteboardWriting, NSPasteboardReading {
		[Export ("types")]
		string [] Types { get; }

		[Export ("availableTypeFromArray:")]
		string GetAvailableTypeFromArray (string [] types);

		[Export ("setDataProvider:forTypes:")]
		bool SetDataProviderForTypes ([Protocolize] NSPasteboardItemDataProvider dataProvider, string [] types);

		[Export ("setData:forType:")]
		bool SetDataForType (NSData data, string type);

		[Export ("setString:forType:")]
		bool SetStringForType (string str, string type);

		[Export ("setPropertyList:forType:")]
		bool SetPropertyListForType (NSObject propertyList, string type);

		[Export ("dataForType:")]
		NSData GetDataForType (string type);

		[Export ("stringForType:")]
		string GetStringForType (string type);

		[Export ("propertyListForType:")]
		NSObject GetPropertyListForType (string type);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPasteboardItemDataProvider {
		[Abstract]
		[Export ("pasteboard:item:provideDataForType:")]
		void ProvideDataForType (NSPasteboard pasteboard, NSPasteboardItem item, string type);

		[Abstract]
		[Export ("pasteboardFinishedWithDataProvider:")]
		void FinishedWithDataProvider (NSPasteboard pasteboard);
	}

	interface INSPasteboardReading {}
	interface INSPasteboardWriting {}

	[BaseType (typeof (NSObject))]
#if !XAMCORE_4_0
	// A class that implements only NSPasteboardReading does not make sense, it's
	// used to add pasteboard support to existing classes.
	[Model]
#endif
	[Protocol]
	interface NSPasteboardReading {
		[Static]
		[Export ("readableTypesForPasteboard:")]
		string [] GetReadableTypesForPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("readingOptionsForType:pasteboard:")]
		NSPasteboardReadingOptions GetReadingOptionsForType (string type, NSPasteboard pasteboard);

#if !XAMCORE_4_0
		// This binding is just broken, it's an ObjC ctor (init*) bound as a normal method.
		[Abstract]
		[Export ("xamarinselector:removed:")]
		[Obsolete ("It will never be called.")]
		NSObject InitWithPasteboardPropertyList (NSObject propertyList, string type);
#else
		FIXME: (compiler error to not forget)
		FIXME: figure out how to bind constructors in protocols.
#endif
	}
	
	[BaseType (typeof (NSActionCell), Events=new Type [] { typeof (NSPathCellDelegate) }, Delegates=new string [] { "WeakDelegate" })]
	interface NSPathCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("pathStyle")]
		NSPathStyle PathStyle { get; set; }

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Export ("setObjectValue:")]
		void SetObjectValue (NSObject obj);

		[Export ("allowedTypes")]
		string [] AllowedTypes { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSPathCellDelegate Delegate { get; set; }

		[Static, Export ("pathComponentCellClass")]
		Class PathComponentCellClass { get; }

		[Export ("pathComponentCells", ArgumentSemantic.Copy)]
		NSPathComponentCell [] PathComponentCells { get; set; }

		[Export ("rectOfPathComponentCell:withFrame:inView:")]
		CGRect GetRect (NSPathComponentCell componentCell, CGRect withFrame, NSView inView);

		[Export ("pathComponentCellAtPoint:withFrame:inView:")]
		NSPathComponentCell GetPathComponent (CGPoint point, CGRect frame, NSView view);

		[Export ("clickedPathComponentCell")]
		NSPathComponentCell ClickedPathComponentCell { get; }

		[Export ("mouseEntered:withFrame:inView:")]
		void MouseEntered (NSEvent evt, CGRect frame, NSView view);

		[Export ("mouseExited:withFrame:inView:")]
		void MouseExited (NSEvent evt, CGRect frame, NSView view);

		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("placeholderString")]
		string PlaceholderString { get; set; }

		[Export ("placeholderAttributedString", ArgumentSemantic.Copy)]
		NSAttributedString PlaceholderAttributedString { get; set; }

		[Export ("setControlSize:")]
		void SetControlSize (NSControlSize size);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPathCellDelegate {
		[Export ("pathCell:willDisplayOpenPanel:"), EventArgs ("NSPathCellDisplayPanel")]
		void WillDisplayOpenPanel (NSPathCell pathCell, NSOpenPanel openPanel);

		[Export ("pathCell:willPopUpMenu:"), EventArgs ("NSPathCellMenu")]
		void WillPopupMenu (NSPathCell pathCell, NSMenu menu);
	}

	[BaseType (typeof (NSTextFieldCell))]
	interface NSPathComponentCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("image", ArgumentSemantic.Copy)]
		NSImage Image { get; set; }

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }
	}


	[BaseType (typeof (NSControl))]
	interface NSPathControl {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Please use ClickedPathItem instead.")]
		[Export ("clickedPathComponentCell")]
		NSPathComponentCell ClickedPathComponentCell { get; }

		[Export ("setDraggingSourceOperationMask:forLocal:")]
		void SetDraggingSource (NSDragOperation operationMask, bool isLocal);

		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }

		[Export ("pathStyle")]
		NSPathStyle PathStyle { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Please use PathItems instead.")]
		[Export ("pathComponentCells")]
		NSPathComponentCell [] PathComponentCells { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy), NullAllowed]
		NSColor BackgroundColor { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSPathControlDelegate Delegate { get; set; }

		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Mac (10,10)]
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Mac (10,10)]
		[Export ("allowedTypes", ArgumentSemantic.Copy)]
		NSString [] AllowedTypes { get; set; }

		[Mac (10,10)]
		[Export ("placeholderString")]
		string PlaceholderString { get; set; }

		[Mac (10,10)]
		[Export ("placeholderAttributedString", ArgumentSemantic.Copy)]
		NSAttributedString PlaceholderAttributedString { get; set; }

		[Mac (10,10)]
		[Export ("clickedPathItem")]
		NSPathControlItem ClickedPathItem { get; }

		[Mac (10,10)]
		[Export ("pathItems", ArgumentSemantic.Copy)]
		NSPathControlItem [] PathItems { get; set; }

	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPathControlDelegate {
		#if !XAMCORE_2_0
		[Abstract]
		#endif
		[Export ("pathControl:shouldDragPathComponentCell:withPasteboard:")]
		bool ShouldDragPathComponentCell (NSPathControl pathControl, NSPathComponentCell pathComponentCell, NSPasteboard pasteboard);

		#if !XAMCORE_2_0
		[Abstract]
		#endif
		[Export ("pathControl:validateDrop:")]
		NSDragOperation ValidateDrop (NSPathControl pathControl, [Protocolize (4)] NSDraggingInfo info);

		#if !XAMCORE_2_0
		[Abstract]
		#endif
		[Export ("pathControl:acceptDrop:")]
		bool AcceptDrop (NSPathControl pathControl, [Protocolize (4)] NSDraggingInfo info);

#if !XAMCORE_2_0
		[Abstract]
#endif
		[Export ("pathControl:willDisplayOpenPanel:")]
		void WillDisplayOpenPanel (NSPathControl pathControl, NSOpenPanel openPanel);

#if !XAMCORE_2_0
		[Abstract]
#endif
		[Export ("pathControl:willPopUpMenu:")]
		void WillPopUpMenu (NSPathControl pathControl, NSMenu menu);

		[Mac (10,10)]
		[Export ("pathControl:shouldDragItem:withPasteboard:")]
		bool ShouldDragItem (NSPathControl pathControl, NSPathControlItem pathItem, NSPasteboard pasteboard);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface NSPathControlItem 
	{
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[Export ("image", ArgumentSemantic.Strong)]
		NSImage Image { get; set; }

		[Export ("URL")]
		NSUrl Url { get; }
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSResponder))]
	interface NSPopover : NSAppearanceCustomization, NSAccessibilityElementProtocol, NSAccessibility {
		[Obsolete ("Use 'GetAppearance' and 'SetAppearance' methods instead.")]
		[Export ("appearance", ArgumentSemantic.Retain)]
		new NSPopoverAppearance Appearance { get; set;  }

		[Export ("behavior")]
		NSPopoverBehavior Behavior { get; set;  }

		[Export ("animates")]
		bool Animates { get; set;  }

		[Export ("contentViewController", ArgumentSemantic.Retain)]
		NSViewController ContentViewController { get; set;  }

		[Export ("contentSize")]
		CGSize ContentSize { get; set;  }

		[Export ("shown")]
		bool Shown { [Bind ("isShown")] get;  }

		[Export ("positioningRect")]
		CGRect PositioningRect { get; set;  }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSPopoverDelegate Delegate { set; get; }
		
		[Export ("showRelativeToRect:ofView:preferredEdge:")]
		void Show (CGRect relativePositioningRect, NSView positioningView, NSRectEdge preferredEdge);

		[Export ("performClose:")]
		void PerformClose (NSObject sender);

		[Export ("close")]
		void Close ();

		[Field ("NSPopoverCloseReasonKey")]
		NSString CloseReasonKey { get; }
		
		[Field ("NSPopoverCloseReasonStandard")]
		NSString CloseReasonStandard { get; }
		
		[Field ("NSPopoverCloseReasonDetachToWindow")]
		NSString CloseReasonDetachToWindow { get; }
		
		[Notification, Field ("NSPopoverWillShowNotification")]
		NSString WillShowNotification { get; }
		
		[Notification, Field ("NSPopoverDidShowNotification")]
		NSString DidShowNotification { get; }
		
		[Notification (typeof (NSPopoverCloseEventArgs)), Field ("NSPopoverWillCloseNotification")]
		NSString WillCloseNotification { get; }
		
		[Notification (typeof (NSPopoverCloseEventArgs)), Field ("NSPopoverDidCloseNotification")]
		NSString DidCloseNotification { get; }

		[Mac (10,10)]
		[Export ("detached")]
		bool Detached { [Bind ("isDetached")] get; }
	}

	partial interface NSPopoverCloseEventArgs {
		[Internal, Export ("NSPopoverCloseReasonKey")]
		NSString _Reason { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPopoverDelegate {
		[Export ("popoverShouldClose:")]
		bool ShouldClose (NSPopover popover);

		[Export ("detachableWindowForPopover:")]
		NSWindow GetDetachableWindowForPopover (NSPopover popover);

		[Export ("popoverWillShow:")]
		void WillShow (NSNotification notification);

		[Export ("popoverDidShow:")]
		void DidShow (NSNotification notification);

		[Export ("popoverWillClose:")]
		void WillClose (NSNotification notification);

		[Export ("popoverDidClose:")]
		void DidClose (NSNotification notification);

		[Mac (10,10)]
		[Export ("popoverDidDetach:")]
		void DidDetach (NSPopover popover);
	}

	[BaseType (typeof (NSButton))]
	partial interface NSPopUpButton {
		[Export ("initWithFrame:pullsDown:")]
		IntPtr Constructor (CGRect buttonFrame, bool pullsDown);

		[Export ("addItemWithTitle:")]
		void AddItem (string title);

		[Export ("addItemsWithTitles:")]
		void AddItems (string [] itemTitles);

		[Export ("insertItemWithTitle:atIndex:")]
		void InsertItem (string title, nint index);

		[Export ("removeItemWithTitle:")]
		void RemoveItem (string title);

		[Export ("removeItemAtIndex:")]
		void RemoveItem (nint index);

		[Export ("removeAllItems")]
		void RemoveAllItems ();

		[Export ("itemArray")]
		NSMenuItem [] Items ();

		[Export ("numberOfItems")]
		nint ItemCount { get; }

		[Export ("indexOfItem:")]
		nint IndexOfItem (NSMenuItem item);

		[Export ("indexOfItemWithTitle:")]
		nint IndexOfItem (string title);

		[Export ("indexOfItemWithTag:")]
		nint IndexOfItem (nint tag);

		[Export ("indexOfItemWithRepresentedObject:")]
		nint IndexOfItem (NSObject obj);

		[Export ("indexOfItemWithTarget:andAction:")]
		nint IndexOfItem (NSObject target, Selector actionSelector);

		[Export ("itemAtIndex:")]
		NSMenuItem ItemAtIndex (nint index);

		[Export ("itemWithTitle:")]
		NSMenuItem ItemWithTitle (string title);

		[Export ("lastItem")]
		NSMenuItem LastItem { get; }

		[Export ("selectItem:")]
		void SelectItem ([NullAllowed] NSMenuItem item);

		[Export ("selectItemAtIndex:")]
		void SelectItem (nint index);

		[Export ("selectItemWithTitle:")]
		void SelectItem (string title);

		[Export ("selectItemWithTag:")]
		bool SelectItemWithTag (nint tag);

		[Export ("setTitle:")]
		void SetTitle (string aString);

		[Export ("selectedItem")]
		NSMenuItem SelectedItem { get; }

		[Export ("indexOfSelectedItem")]
		nint IndexOfSelectedItem { get; }

		[Export ("synchronizeTitleAndSelectedItem")]
		void SynchronizeTitleAndSelectedItem ();

		[Export ("itemTitleAtIndex:")]
		string ItemTitle (nint index);

		[Export ("itemTitles")]
		string [] ItemTitles ();

		[Export ("titleOfSelectedItem")]
		string TitleOfSelectedItem { get; }

		//Detected properties
		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Export ("pullsDown")]
		bool PullsDown { get; set; }

		[Export ("autoenablesItems")]
		bool AutoEnablesItems { get; set; }

		[Export ("preferredEdge")]
		NSRectEdge PreferredEdge { get; set; }

		[Mac (10,10)]
		[Export ("selectedTag")]
		nint SelectedTag { get; }
	}


	[BaseType (typeof (NSMenuItemCell))]
	partial interface NSPopUpButtonCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[DesignatedInitializer]
		[Export ("initTextCell:pullsDown:")]
		IntPtr Constructor (string stringValue, bool pullDown);

		[Export ("addItemWithTitle:")]
		void AddItem (string title);

		[Export ("addItemsWithTitles:")]
		void AddItems (string [] itemTitles);

		[Export ("insertItemWithTitle:atIndex:")]
		void InsertItem (string title, nint index);

		[Export ("removeItemWithTitle:")]
		void RemoveItem (string title);

		[Export ("removeItemAtIndex:")]
		void RemoveItemAt (nint index);

		[Export ("removeAllItems")]
		void RemoveAllItems ();

		[Export ("itemArray")]
		NSMenuItem [] Items { get; }

		[Export ("numberOfItems")]
		nint Count { get; }

		[Export ("indexOfItem:")]
		nint IndexOf (NSMenuItem item);

		[Export ("indexOfItemWithTitle:")]
		nint IndexOfItemWithTitle (string title);

		[Export ("indexOfItemWithTag:")]
		nint IndexOfItemWithTag (nint tag);

		[Export ("indexOfItemWithRepresentedObject:")]
		nint IndexOfItemWithRepresentedObject (NSObject obj);

		[Export ("indexOfItemWithTarget:andAction:")]
		nint IndexOfItemWithTargetandAction (NSObject target, Selector actionSelector);

		[Export ("itemAtIndex:")]
		NSMenuItem ItemAt (nint index);

		[Export ("itemWithTitle:")]
		NSMenuItem ItemWithTitle (string title);

		[Export ("lastItem")]
		NSMenuItem LastItem { get; }

		[Export ("selectItem:")]
		void SelectItem (NSMenuItem item);

		[Export ("selectItemAtIndex:")]
		void SelectItemAt (nint index);

		[Export ("selectItemWithTitle:")]
		void SelectItemWithTitle (string title);

		[Export ("selectItemWithTag:")]
		bool SelectItemWithTag (nint tag);

		[Export ("setTitle:")]
		void SetTitle (string aString);

		[Export ("selectedItem")]
		NSMenuItem SelectedItem { get; }

		[Export ("indexOfSelectedItem")]
		nint SelectedItemIndex { get; }

		[Export ("synchronizeTitleAndSelectedItem")]
		void SynchronizeTitleAndSelectedItem ();

		[Export ("itemTitleAtIndex:")]
		string GetItemTitle (nint index);

		[Export ("itemTitles")]
		string [] ItemTitles { get; }

		[Export ("titleOfSelectedItem")]
		string TitleOfSelectedItem { get; }

		[Export ("attachPopUpWithFrame:inView:")]
		void AttachPopUp (CGRect cellFrame, NSView inView);

		[Export ("dismissPopUp")]
		void DismissPopUp ();

		[Export ("performClickWithFrame:inView:")]
		void PerformClick (CGRect withFrame, NSView controlView);

		//Detected properties
		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Export ("pullsDown")]
		bool PullsDown { get; set; }

		[Export ("autoenablesItems")]
		bool AutoenablesItems { get; set; }

		[Export ("preferredEdge")]
		NSRectEdge PreferredEdge { get; set; }

		[Export ("usesItemFromMenu")]
		bool UsesItemFromMenu { get; set; }

		[Export ("altersStateOfSelectedItem")]
		bool AltersStateOfSelectedItem { get; set; }

		[Export ("arrowPosition")]
		NSPopUpArrowPosition ArrowPosition { get; set; }

		[Export ("objectValue")]
		NSObject ObjectValue { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPrinter : NSCoding, NSCopying {
		[Static]
		[Export ("printerNames", ArgumentSemantic.Copy)]
		string [] PrinterNames{ get; }

		[Static]
		[Export ("printerTypes", ArgumentSemantic.Copy)]
		string [] PrinterTypes { get; }

		[Static]
		[Export ("printerWithName:")]
		NSPrinter PrinterWithName (string name);

		[Static]
		[Export ("printerWithType:")]
		NSPrinter PrinterWithType (string type);

		[Export ("name")]
		string Name { get; }

		[Export ("type")]
		string Type { get; }

		[Export ("languageLevel")]
		nint LanguageLevel { get; }

		[Export ("pageSizeForPaper:")]
		CGSize PageSizeForPaper (string paperName); 

		[Export ("statusForTable:")]
		NSPrinterTableStatus StatusForTable (string tableName);

		[Export ("isKey:inTable:")]
		bool IsKeyInTable (string key, string table);

		[Export ("booleanForKey:inTable:")]
		bool BooleanForKey (string key, string table);

		[Export ("floatForKey:inTable:")]
		float /* float, not CGFloat */ FloatForKey (string key, string table);

		[Export ("intForKey:inTable:")]
		int /* int, not NSInteger */ IntForKey (string key, string table);

		[Export ("rectForKey:inTable:")]
		CGRect RectForKey (string key, string table);

		[Export ("sizeForKey:inTable:")]
		CGSize SizeForKey (string key, string table);

		[Export ("stringForKey:inTable:")]
		string StringForKey (string key, string table);

		[Export ("stringListForKey:inTable:")]
		string [] StringListForKey (string key, string table);

		[Export ("deviceDescription")]
		NSDictionary DeviceDescription { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSPrintInfo : NSCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithDictionary:")]
		IntPtr Constructor (NSDictionary attributes);

		[Export ("dictionary")]
		NSMutableDictionary Dictionary { get; }

		[Export ("setUpPrintOperationDefaultValues")]
		void SetUpPrintOperationDefaultValues ();

		[Export ("imageablePageBounds")]
		CGRect ImageablePageBounds { get; }

		[Export ("localizedPaperName")]
		string LocalizedPaperName { get; }

		[Static]
		[Export ("defaultPrinter")]
		NSPrinter DefaultPrinter { get; }

		[Export ("printSettings")]
		NSMutableDictionary PrintSettings { get; }

#if XAMCORE_4_0
		[Internal]
#endif
		[Export ("PMPrintSession")]
		IntPtr GetPMPrintSession ();

#if XAMCORE_4_0
		[Internal]
#endif
		[Export ("PMPageFormat")]
		IntPtr GetPMPageFormat ();

#if XAMCORE_4_0
		[Internal]
#endif
		[Export ("PMPrintSettings")]
		IntPtr GetPMPrintSettings ();

		[Export ("updateFromPMPageFormat")]
		void UpdateFromPMPageFormat ();

		[Export ("updateFromPMPrintSettings")]
		void UpdateFromPMPrintSettings ();

		//Detected properties
		[Static]
		[Export ("sharedPrintInfo")]
		NSPrintInfo SharedPrintInfo { get; set; }

		[Export ("paperName")]
		string PaperName { get; set; }

		[Export ("paperSize")]
		CGSize PaperSize { get; set; }

		[Export ("orientation")]
		NSPrintingOrientation Orientation { get; set; }

		[Export ("scalingFactor")]
		nfloat ScalingFactor { get; set; }

		[Export ("leftMargin")]
		nfloat LeftMargin { get; set; }

		[Export ("rightMargin")]
		nfloat RightMargin { get; set; }

		[Export ("topMargin")]
		nfloat TopMargin { get; set; }

		[Export ("bottomMargin")]
		nfloat BottomMargin { get; set; }

		[Export ("horizontallyCentered")]
		bool HorizontallyCentered { [Bind ("isHorizontallyCentered")]get; set; }

		[Export ("verticallyCentered")]
		bool VerticallyCentered { [Bind ("isVerticallyCentered")]get; set; }

		[Export ("horizontalPagination")]
		NSPrintingPaginationMode HorizontalPagination { get; set; }

		[Export ("verticalPagination")]
		NSPrintingPaginationMode VerticalPagination { get; set; }

		[Export ("jobDisposition")]
		string JobDisposition { get; set; }

		[Export ("printer", ArgumentSemantic.Copy)]
		NSPrinter Printer { get; set; }

		[Export ("selectionOnly")]
		bool SelectionOnly { [Bind ("isSelectionOnly")]get; set; }

	}


	[BaseType (typeof (NSObject))]
	partial interface NSPrintOperation {
		[Static]
		[Export ("printOperationWithView:printInfo:")]
		NSPrintOperation FromView (NSView view, NSPrintInfo printInfo);

		[Static]
		[Export ("PDFOperationWithView:insideRect:toData:printInfo:")]
		NSPrintOperation PdfFromView (NSView view, CGRect rect, NSMutableData data, NSPrintInfo printInfo);

		[Static]
		[Export ("PDFOperationWithView:insideRect:toPath:printInfo:")]
		NSPrintOperation PdfFromView (NSView view, CGRect rect, string path, NSPrintInfo printInfo);

		[Static]
		[Export ("EPSOperationWithView:insideRect:toData:printInfo:")]
		NSPrintOperation EpsFromView (NSView view, CGRect rect, NSMutableData data, NSPrintInfo printInfo);

		[Static]
		[Export ("EPSOperationWithView:insideRect:toPath:printInfo:")]
		NSPrintOperation EpsFromView (NSView view, CGRect rect, string path, NSPrintInfo printInfo);

		[Static]
		[Export ("printOperationWithView:")]
		NSPrintOperation FromView (NSView view);

		[Static]
		[Export ("PDFOperationWithView:insideRect:toData:")]
		NSPrintOperation PdfFromView (NSView view, CGRect rect, NSMutableData data);

		[Static]
		[Export ("EPSOperationWithView:insideRect:toData:")]
		NSPrintOperation EpsFromView (NSView view, CGRect rect, NSMutableData data);

		[Export ("isCopyingOperation")]
		bool IsCopyingOperation { get; }

		[Export ("runOperationModalForWindow:delegate:didRunSelector:contextInfo:")]
		void RunOperationModal (NSWindow docWindow, NSObject del, Selector didRunSelector, IntPtr contextInfo);

		[Export ("runOperation")]
		bool RunOperation ();

		[Export ("view")]
		NSView View { get; }

		[Export ("context")]
		NSGraphicsContext Context { get; }

		[Export ("pageRange")]
		NSRange PageRange { get; }

		[Export ("currentPage")]
		nint CurrentPage { get; }

		[Export ("createContext")]
		NSGraphicsContext CreateContext ();

		[Export ("destroyContext")]
		void DestroyContext ();

		[Export ("deliverResult")]
		bool DeliverResult ();

		[Export ("cleanUpOperation")]
		void CleanUpOperation ();

		//Detected properties
		[Static]
		[Export ("currentOperation")]
		NSPrintOperation CurrentOperation { get; set; }

		[Export ("jobTitle")]
		string JobTitle { get; set; }

		[Export ("showsPrintPanel")]
		bool ShowsPrintPanel { get; set; }

		[Export ("showsProgressPanel")]
		bool ShowsProgressPanel { get; set; }

		[Export ("printPanel", ArgumentSemantic.Retain)]
		NSPrintPanel PrintPanel { get; set; }

		[Export ("canSpawnSeparateThread")]
		bool CanSpawnSeparateThread { get; set; }

		[Export ("pageOrder")]
		NSPrintingPageOrder PageOrder { get; set; }

		[Export ("printInfo", ArgumentSemantic.Copy)]
		NSPrintInfo PrintInfo { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSPrintPanelAccessorizing {
		[Abstract]
		[Export ("localizedSummaryItems")]
		NSDictionary [] LocalizedSummaryItems ();

		[Abstract]
		[Export ("keyPathsForValuesAffectingPreview")]
		NSSet KeyPathsForValuesAffectingPreview ();
	}

	[BaseType (typeof (NSObject))]
	[Dispose ("__mt_accessory_var = null;")] 
	interface NSPrintPanel {
		[Static]
		[Export ("printPanel")]
		NSPrintPanel PrintPanel { get; }

		[Export ("addAccessoryController:")]
		[PostSnippet ("__mt_accessory_var = AccessoryControllers();")]
		void AddAccessoryController (NSViewController accessoryController);

		[Export ("removeAccessoryController:")]
		[PostSnippet ("__mt_accessory_var = AccessoryControllers();")]
		void RemoveAccessoryController (NSViewController accessoryController);

		[Export ("accessoryControllers")]
		NSViewController [] AccessoryControllers ();

		[Export ("beginSheetWithPrintInfo:modalForWindow:delegate:didEndSelector:contextInfo:")]
		void BeginSheet (NSPrintInfo printInfo, NSWindow docWindow, [NullAllowed] NSObject del, [NullAllowed] Selector didEndSelector, IntPtr contextInfo);

		[Export ("runModalWithPrintInfo:")]
		nint RunModalWithPrintInfo (NSPrintInfo printInfo);

		[Export ("runModal")]
		nint RunModal ();

		[Export ("printInfo")]
		NSPrintInfo PrintInfo { get; }

		//Detected properties
		[Export ("options")]
		NSPrintPanelOptions Options { get; set; }

		[Export ("defaultButtonTitle")]
		string DefaultButtonTitle { get; set; }

		[Export ("helpAnchor")]
		string HelpAnchor { get; set; }

		[Export ("jobStyleHint")]
		string JobStyleHint { get; set; }
	}

	[BaseType (typeof (NSView))]
	interface NSProgressIndicator : NSAccessibilityProgressIndicator {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("incrementBy:")]
		void IncrementBy (double delta);

		[Export ("startAnimation:")]
		void StartAnimation ([NullAllowed] NSObject sender);

		[Export ("stopAnimation:")]
		void StopAnimation ([NullAllowed] NSObject sender);

		[Export ("style")]
		NSProgressIndicatorStyle Style { get; set; }

		[Export ("sizeToFit")]
		void SizeToFit ();

		[Export ("displayedWhenStopped")]
		bool IsDisplayedWhenStopped { [Bind ("isDisplayedWhenStopped")] get; set; }

		//Detected properties
		[Export ("indeterminate")]
		bool Indeterminate { [Bind ("isIndeterminate")]get; set; }

		[Export ("bezeled")]
		bool Bezeled { [Bind ("isBezeled")]get; set; }

		[Export ("controlTint")]
		NSControlTint ControlTint { get; set; }

		[Export ("controlSize")]
		NSControlSize ControlSize { get; set; }

		[Export ("doubleValue")]
		double DoubleValue { get; set; }

		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("usesThreadedAnimation")]
		bool UsesThreadedAnimation { get; set; }
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSResponder : NSCoding, NSTouchBarProvider {
		[Export ("tryToPerform:with:")]
		bool TryToPerformwith (Selector anAction, [NullAllowed] NSObject anObject);

		[Export ("performKeyEquivalent:")]
		bool PerformKeyEquivalent (NSEvent theEvent);

		[Export ("validRequestorForSendType:returnType:")]
#if XAMCORE_2_0
		NSObject ValidRequestorForSendType ([NullAllowed] string sendType, [NullAllowed] string returnType);
#else
		[Obsolete ("Use 'ValidRequestorForSendType' instead.")]
		NSObject ValidRequestorForSendTypereturnType (string sendType, string returnType);
#endif

		[Export ("mouseDown:")]
		void MouseDown (NSEvent theEvent);

		[Export ("rightMouseDown:")]
		void RightMouseDown (NSEvent theEvent);

		[Export ("otherMouseDown:")]
		void OtherMouseDown (NSEvent theEvent);

		[Export ("mouseUp:")]
		void MouseUp (NSEvent theEvent);

		[Export ("rightMouseUp:")]
		void RightMouseUp (NSEvent theEvent);

		[Export ("otherMouseUp:")]
		void OtherMouseUp (NSEvent theEvent);

		[Export ("mouseMoved:")]
		void MouseMoved (NSEvent theEvent);

		[Export ("mouseDragged:")]
		void MouseDragged (NSEvent theEvent);

		[Export ("scrollWheel:")]
		void ScrollWheel (NSEvent theEvent);

		[Export ("rightMouseDragged:")]
		void RightMouseDragged (NSEvent theEvent);

		[Export ("otherMouseDragged:")]
		void OtherMouseDragged (NSEvent theEvent);

		[Export ("mouseEntered:")]
		void MouseEntered (NSEvent theEvent);

		[Export ("mouseExited:")]
		void MouseExited (NSEvent theEvent);

		[Export ("keyDown:")]
		void KeyDown (NSEvent theEvent);

		[Export ("keyUp:")]
		void KeyUp (NSEvent theEvent);

		[Export ("flagsChanged:")]
		void FlagsChanged (NSEvent theEvent);

		[Export ("tabletPoint:")]
		void TabletPoint (NSEvent theEvent);

		[Export ("tabletProximity:")]
		void TabletProximity (NSEvent theEvent);

		[Export ("cursorUpdate:")]
		void CursorUpdate (NSEvent theEvent);

		[Export ("magnifyWithEvent:")]
		void MagnifyWithEvent (NSEvent theEvent);

		[Export ("rotateWithEvent:")]
		void RotateWithEvent (NSEvent theEvent);

		[Export ("swipeWithEvent:")]
		void SwipeWithEvent (NSEvent theEvent);

		[Export ("beginGestureWithEvent:")]
		void BeginGestureWithEvent (NSEvent theEvent);

		[Export ("endGestureWithEvent:")]
		void EndGestureWithEvent (NSEvent theEvent);

		[Export ("touchesBeganWithEvent:")]
		void TouchesBeganWithEvent (NSEvent theEvent);

		[Export ("touchesMovedWithEvent:")]
		void TouchesMovedWithEvent (NSEvent theEvent);

		[Export ("touchesEndedWithEvent:")]
		void TouchesEndedWithEvent (NSEvent theEvent);

		[Export ("touchesCancelledWithEvent:")]
		void TouchesCancelledWithEvent (NSEvent theEvent);

		[Export ("noResponderFor:")]
		void NoResponderFor (Selector eventSelector);

		[Export ("acceptsFirstResponder")]
		bool AcceptsFirstResponder ();

		[Export ("becomeFirstResponder")]
		bool BecomeFirstResponder ();

		[Export ("resignFirstResponder")]
		bool ResignFirstResponder ();

		[Export ("interpretKeyEvents:")]
		void InterpretKeyEvents (NSEvent [] eventArray);

		[Export ("flushBufferedKeyEvents")]
		void FlushBufferedKeyEvents ();

		[Export ("showContextHelp:")]
		void ShowContextHelp (NSObject sender);

		[Export ("helpRequested:")]
		void HelpRequested (NSEvent theEventPtr);

		[Export ("shouldBeTreatedAsInkEvent:")]
		bool ShouldBeTreatedAsInkEvent (NSEvent theEvent);

		//Detected properties
		[Export ("nextResponder")][NullAllowed]
		NSResponder NextResponder { get; set; }

		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		NSMenu Menu { get; set; }

		[Mac (10, 7), Export ("encodeRestorableStateWithCoder:")]
		void EncodeRestorableState (NSCoder coder);

		[Mac (10, 7), Export ("restoreStateWithCoder:")]
		void RestoreState (NSCoder coder);

		[Mac (10, 7), Export ("invalidateRestorableState")]
		void InvalidateRestorableState ();

		[Static]
		[Mac (10, 7), Export ("restorableStateKeyPaths", ArgumentSemantic.Copy)]
		string [] RestorableStateKeyPaths ();

		[Mac (10, 7)]
		[Export ("wantsForwardedScrollEventsForAxis:")]
		bool WantsForwardedScrollEventsForAxis (NSEventGestureAxis axis);

#if XAMCORE_2_0
		[Mac (10,10, onlyOn64 : true)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("userActivity", ArgumentSemantic.Strong)]
		NSUserActivity UserActivity { get; set; }

		[Mac (10,10, onlyOn64 : true)]
		[Export ("updateUserActivityState:")]
		void UpdateUserActivityState (NSUserActivity userActivity);

		[Mac (10,10, onlyOn64 : true)]
		[Export ("restoreUserActivityState:")]
		void RestoreUserActivityState (NSUserActivity userActivity);

		[Mac (10,10,3, onlyOn64 : true)]
		[Export ("pressureChangeWithEvent:")]
		void PressureChange (NSEvent pressureChangeEvent);
#endif

		[Mac (10,12)]
		[Export ("newWindowForTab:")]
		void GetNewWindowForTab ([NullAllowed] NSObject sender);

		[Export ("presentError:")]
		bool PresentError (NSError error);

		[Export ("willPresentError:")]
		NSError WillPresentError (NSError error);

		[Sealed]
		[Export ("presentError:modalForWindow:delegate:didPresentSelector:contextInfo:")]
		void PresentError (NSError error, NSWindow window, [NullAllowed] NSObject @delegate, [NullAllowed] Selector didPresentSelector, IntPtr contextInfo);

		[Mac (10,13)]
		[Export ("encodeRestorableStateWithCoder:backgroundQueue:")]
		void EncodeRestorableState (NSCoder coder, NSOperationQueue queue);
	}

	[Category]
	[BaseType (typeof(NSResponder))]
	interface NSResponder_NSTouchBarProvider : INSTouchBarProvider
	{
		[Mac (10, 12, 2)]
		[NullAllowed, Export ("touchBar")]
		NSTouchBar GetTouchBar ();

		[Mac (10, 12, 2)]
		[Export ("setTouchBar:")]
		void SetTouchBar ([NullAllowed]NSTouchBar bar);

		[Mac (10, 12, 2)]
		[NullAllowed, Export ("makeTouchBar")]
		NSTouchBar MakeTouchBar ();
	}

	[Mac (10,10)]
	[BaseType (typeof (NSGestureRecognizer))]
	interface NSRotationGestureRecognizer {
		[Export ("initWithTarget:action:")]
		IntPtr Constructor (NSObject target, Selector action);

		[Export ("rotation")]
		nfloat Rotation { get; set; }

		[Export ("rotationInDegrees")]
		nfloat RotationInDegrees { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSRulerMarker : NSCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithRulerView:markerLocation:image:imageOrigin:")]
		IntPtr Constructor (NSRulerView ruler, nfloat location, NSImage image, CGPoint imageOrigin);

		[Export ("ruler")]
		NSRulerView Ruler { get; }

		[Export ("isDragging")]
		bool IsDragging { get; }

		[Export ("imageRectInRuler")]
		CGRect ImageRectInRuler { get; }

		[Export ("thicknessRequiredInRuler")]
		nfloat ThicknessRequiredInRuler { get; }

		[Export ("drawRect:")]
		void DrawRect (CGRect rect);

		[Export ("trackMouse:adding:")]
		bool TrackMouse (NSEvent mouseDownEvent, bool isAdding);

		//Detected properties
		[Export ("markerLocation")]
		nfloat MarkerLocation { get; set; }

		[Export ("image", ArgumentSemantic.Retain)]
		NSImage Image { get; set; }

		[Export ("imageOrigin")]
		CGPoint ImageOrigin { get; set; }

		[Export ("movable")]
		bool Movable { [Bind ("isMovable")]get; set; }

		[Export ("removable")]
		bool Removable { [Bind ("isRemovable")]get; set; }

		[Export ("representedObject", ArgumentSemantic.Retain)]
		NSObject RepresentedObject { get; set; }
	}

	[BaseType (typeof (NSView))]
	partial interface NSRulerView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Static]
		[Export ("registerUnitWithName:abbreviation:unitToPointsConversionFactor:stepUpCycle:stepDownCycle:")]
		void RegisterUnit (string unitName, string abbreviation, nfloat conversionFactor, NSNumber [] stepUpCycle, NSNumber [] stepDownCycle);

		[DesignatedInitializer]
		[Export ("initWithScrollView:orientation:")]
		IntPtr Constructor (NSScrollView scrollView, NSRulerOrientation orientation);

		[Export ("baselineLocation")]
		nfloat BaselineLocation { get; }

		[Export ("requiredThickness")]
		nfloat RequiredThickness { get; }

		[Export ("addMarker:")]
		[PostGet ("Markers")]
		void AddMarker (NSRulerMarker marker);

		[Export ("removeMarker:")]
		[PostGet ("Markers")]
		void RemoveMarker (NSRulerMarker marker);

		[Export ("trackMarker:withMouseEvent:")]
		bool TrackMarker (NSRulerMarker marker, NSEvent theEvent);

		[Export ("moveRulerlineFromLocation:toLocation:")]
		void MoveRulerline (nfloat oldLocation, nfloat newLocation);

		[Export ("invalidateHashMarks")]
		void InvalidateHashMarks ();

		[Export ("drawHashMarksAndLabelsInRect:")]
		void DrawHashMarksAndLabels (CGRect rect);

		[Export ("drawMarkersInRect:")]
		void DrawMarkers (CGRect rect);

		[Export ("isFlipped")]
		bool IsFlipped { get; }

		//Detected properties
		[Export ("scrollView", ArgumentSemantic.Weak)]
		NSScrollView ScrollView { get; set; }

		[Export ("orientation")]
		NSRulerOrientation Orientation { get; set; }

		[Export ("ruleThickness")]
		nfloat RuleThickness { get; set; }

		[Export ("reservedThicknessForMarkers")]
		nfloat ReservedThicknessForMarkers { get; set; }

		[Export ("reservedThicknessForAccessoryView")]
		nfloat ReservedThicknessForAccessoryView { get; set; }

		[Export ("measurementUnits")]
#if XAMCORE_4_0
		[BindAs (typeof (NSRulerViewUnits))] 
#endif
		string MeasurementUnits { get; set; }

		[Export ("originOffset")]
		nfloat OriginOffset { get; set; }

		[Export ("clientView", ArgumentSemantic.Weak)]
		NSView ClientView { get; set; }

		[Export ("markers", ArgumentSemantic.Copy), NullAllowed]
		NSRulerMarker [] Markers { get; set; }

		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; }
	}

	[Mac (10, 13)]
	enum NSRulerViewUnits
	{
		[Field ("NSRulerViewUnitInches")]
		Inches,

		[Field ("NSRulerViewUnitCentimeters")]
		Centimeters,

		[Field ("NSRulerViewUnitPoints")]
		Points,

		[Field ("NSRulerViewUnitPicas")]
		Picas,
	}

	delegate void NSSavePanelComplete (nint result);
	
	[BaseType (typeof (NSPanel), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSOpenSavePanelDelegate)})]
	interface NSSavePanel {
		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("isExpanded")]
		bool IsExpanded { get; }

		[Export ("validateVisibleColumns")]
		void ValidateVisibleColumns ();

		[Export ("ok:")]
		void Ok (NSObject sender);

		[Export ("cancel:")]
		void Cancel (NSObject sender);

		[Export ("beginSheetModalForWindow:completionHandler:")]
		void BeginSheet (NSWindow window, NSSavePanelComplete onComplete);

		[Export ("beginWithCompletionHandler:")]
		void Begin (NSSavePanelComplete onComplete);

		[Export ("runModal")]
		nint RunModal ();

		//Detected properties
		[Export ("directoryURL", ArgumentSemantic.Copy)]
		NSUrl DirectoryUrl { get; set; }

		[Export ("allowedFileTypes")]
		string [] AllowedFileTypes { get; set; }

		[Export ("allowsOtherFileTypes")]
		bool AllowsOtherFileTypes { get; set; }

		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSOpenSavePanelDelegate Delegate { get; set; }

		[Export ("canCreateDirectories")]
		bool CanCreateDirectories { get; set; }

		[Export ("canSelectHiddenExtension")]
		bool CanSelectHiddenExtension { get; set; }

		[Export ("extensionHidden")]
		bool ExtensionHidden { [Bind ("isExtensionHidden")]get; set; }

		[Export ("treatsFilePackagesAsDirectories")]
		bool TreatsFilePackagesAsDirectories { get; set; }

		[Export ("prompt")]
		string Prompt { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("nameFieldLabel")]
		string NameFieldLabel { get; set; }

		[Export ("nameFieldStringValue")]
		string NameFieldStringValue { get; set; }

		[Export ("message")]
		string Message { get; set; }

		[Export ("showsHiddenFiles")]
		bool ShowsHiddenFiles { get; set; }

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use Url instead.")]
		[Export ("filename")]
		string Filename { get; }

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use DirectoryUrl instead.")]
		[Export ("directory")]
		string Directory { get; set; }

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use AllowedFileTypes instead.")]
		[Export ("requiredFileType")]
		string RequiredFileType { get; set; }

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use Begin with the callback instead.")]
		[Export ("beginSheetForDirectory:file:modalForWindow:modalDelegate:didEndSelector:contextInfo:")]
		void Begin (string directory, string filename, NSWindow docWindow, NSObject modalDelegate, Selector selector, IntPtr context);

		[Availability (Deprecated = Platform.Mac_10_6, Message = "Use RunModal without parameters instead.")]
		[Export ("runModalForDirectory:file:")]
		nint RunModal ([NullAllowed] string directory, [NullAllowed]  string filename);

[Mac (10,9)]
		[Export ("showsTagField")]
		bool ShowsTagField { get; set; }

		[Mac (10,9)]
		[Export ("tagNames", ArgumentSemantic.Copy)]
		string [] TagNames { get; set; }
		
	}

#if !XAMCORE_4_0
	// This class doesn't show up in any documentation.
	[BaseType (typeof (NSSavePanel))]
	[DisableDefaultCtor] // should not be created by (only returned to) user code
	interface NSRemoteSavePanel {}
#endif

	[BaseType (typeof (NSObject))]
	partial interface NSScreen {
		[Static]
		[Export ("screens", ArgumentSemantic.Copy)]
		NSScreen [] Screens { get; }

		[Static]
		[Export ("mainScreen")]
		NSScreen MainScreen { get; }

		[Static]
		[Export ("deepestScreen")]
		NSScreen DeepestScreen { get; }

		[Export ("depth")]
		NSWindowDepth Depth { get; }

		[Export ("frame")]
		CGRect Frame { get; }

		[Export ("visibleFrame")]
		CGRect VisibleFrame { get; }

		[Export ("deviceDescription")]
		NSDictionary DeviceDescription { get; }

		[Export ("colorSpace")]
		NSColorSpace ColorSpace { get; }

		[Export ("supportedWindowDepths"), Internal]
		IntPtr GetSupportedWindowDepths ();

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("userSpaceScaleFactor")]
		nfloat UserSpaceScaleFactor { get; }

		[Mac (10, 7), Export ("convertRectToBacking:")]
		CGRect ConvertRectToBacking (CGRect aRect);

		[Mac (10, 7), Export ("convertRectFromBacking:")]
		CGRect ConvertRectfromBacking (CGRect aRect);

		[Mac (10, 7), Export ("backingAlignedRect:options:")]
		CGRect GetBackingAlignedRect (CGRect globalScreenCoordRect, NSAlignmentOptions options);

		[Mac (10, 7), Export ("backingScaleFactor")]
		nfloat BackingScaleFactor { get; }

		[Mac (10,9)]
		[Static, Export ("screensHaveSeparateSpaces")]
		bool ScreensHaveSeparateSpaces ();

		[Mac (10,12)]
		[Export ("canRepresentDisplayGamut:")]
		bool CanRepresentDisplayGamut (NSDisplayGamut displayGamut);
	}

	[BaseType (typeof (NSControl))]
	interface NSScroller {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Availability (Deprecated = Platform.Mac_10_7, Message = "Use GetScrollerWidth instead.")]
		[Static]
		[Export ("scrollerWidth")]
		nfloat ScrollerWidth { get; }

		[Availability (Deprecated = Platform.Mac_10_7, Message = "Use GetScrollerWidth instead.")]
		[Static]
		[Export ("scrollerWidthForControlSize:")]
		nfloat ScrollerWidthForControlSize (NSControlSize controlSize);

		[Export ("drawParts")]
		[Availability (Deprecated = Platform.Mac_10_7)]
		void DrawParts ();

		[Export ("rectForPart:")]
		CGRect RectForPart (NSScrollerPart partCode);

		[Export ("checkSpaceForParts")]
		void CheckSpaceForParts ();

		[Export ("usableParts")]
		NSUsableScrollerParts UsableParts { get; }

		[Export ("drawArrow:highlight:")]
		void DrawArrow (NSScrollerArrow whichArrow, bool highlight);

		[Export ("drawKnob")]
		void DrawKnob ();

		[Export ("drawKnobSlotInRect:highlight:")]
		void DrawKnobSlot (CGRect slotRect, bool highlight);

		[Export ("highlight:")]
		void Highlight (bool flag);

		[Export ("testPart:")]
		NSScrollerPart TestPart (CGPoint thePoint);

		[Export ("trackKnob:")]
		void TrackKnob (NSEvent theEvent);

		[Export ("trackScrollButtons:")]
		void TrackScrollButtons (NSEvent theEvent);

		[Export ("hitPart")]
		NSScrollerPart HitPart { get; }

		//Detected properties
		[Export ("arrowsPosition")]
		NSScrollArrowPosition ArrowsPosition { get; set; }

		[Export ("controlTint")]
		NSControlTint ControlTint { get; set; }

		[Export ("controlSize")]
		NSControlSize ControlSize { get; set; }

		[Export ("knobProportion")]
		nfloat KnobProportion { get; set; }
		
		[Static]
		[Mac (10, 7), Export ("isCompatibleWithOverlayScrollers")]
		bool CompatibleWithOverlayScrollers { get; }
		
		[Mac (10, 7), Export ("knobStyle")]
		NSScrollerKnobStyle KnobStyle { get; set; }
		
		[Static]
		[Mac (10, 7), Export ("preferredScrollerStyle")]
		NSScrollerStyle PreferredScrollerStyle { get; }
		
		[Export ("scrollerStyle")]
		NSScrollerStyle ScrollerStyle { get; set; }
		
		[Static]
		[Mac (10, 7), Export ("scrollerWidthForControlSize:scrollerStyle:")]
		nfloat GetScrollerWidth (NSControlSize forControlSize, NSScrollerStyle scrollerStyle);
		
		[Notification, Field ("NSPreferredScrollerStyleDidChangeNotification")]
		NSString PreferredStyleChangedNotification { get; }

	}

	[BaseType (typeof (NSView))]
	partial interface NSScrollView : NSTextFinderBarContainer {
		[Availability (Deprecated = Platform.Mac_10_7)]
		[Static]
		[Export ("frameSizeForContentSize:hasHorizontalScroller:hasVerticalScroller:borderType:")]
		CGSize FrameSizeForContentSize (CGSize cSize, bool hFlag, bool vFlag, NSBorderType aType);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Static]
		[Export ("contentSizeForFrameSize:hasHorizontalScroller:hasVerticalScroller:borderType:")]
		CGSize ContentSizeForFrame (CGSize fSize, bool hFlag, bool vFlag, NSBorderType aType);

		[Export ("documentVisibleRect")]
		CGRect DocumentVisibleRect { get; }

		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("contentSize")]
		CGSize ContentSize { get; }

		[Export ("tile")]
		void Tile ();

		[Export ("reflectScrolledClipView:")]
		void ReflectScrolledClipView (NSClipView cView);

		[Export ("scrollWheel:")]
		void ScrollWheel (NSEvent theEvent);

		//Detected properties
		[Export ("documentView")]
		NSObject DocumentView { get; set; }

		[Export ("contentView", ArgumentSemantic.Retain)]
		new NSClipView ContentView { get; set; }

		[Export ("documentCursor", ArgumentSemantic.Retain)]
		NSCursor DocumentCursor { get; set; }

		[Export ("borderType")]
		NSBorderType BorderType { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("hasVerticalScroller")]
		bool HasVerticalScroller { get; set; }

		[Export ("hasHorizontalScroller")]
		bool HasHorizontalScroller { get; set; }
		
		[Export ("verticalScroller", ArgumentSemantic.Retain)]
		NSScroller VerticalScroller { get; set; }

		[Export ("horizontalScroller", ArgumentSemantic.Retain)]
		NSScroller HorizontalScroller { get; set; }

		[Export ("autohidesScrollers")]
		bool AutohidesScrollers { get; set; }

		[Export ("horizontalLineScroll")]
		nfloat HorizontalLineScroll { get; set; }

		[Export ("verticalLineScroll")]
		nfloat VerticalLineScroll { get; set; }

		[Export ("lineScroll")]
		nfloat LineScroll { get; set; }

		[Export ("horizontalPageScroll")]
		nfloat HorizontalPageScroll { get; set; }

		[Export ("verticalPageScroll")]
		nfloat VerticalPageScroll { get; set; }

		[Export ("pageScroll")]
		nfloat PageScroll { get; set; }

		[Export ("scrollsDynamically")]
		bool ScrollsDynamically { get; set; }
		
		[Export ("hasVerticalRuler")]
		bool HasVerticalRuler { get; set; }

		[Export ("hasHorizontalRuler")]
		bool HasHorizontalRuler { get; set; }
		
		[Export ("rulersVisible")]
		bool RulersVisible { get; set; }
		
		[Export ("horizontalRulerView")]
		NSRulerView HorizontalRulerView { get; set; }
		
		[Export ("verticalRulerView")]
		NSRulerView VerticalRulerView { get; set; }

		[Static]
		[Mac (10, 7), Export ("contentSizeForFrameSize:horizontalScrollerClass:verticalScrollerClass:borderType:controlSize:scrollerStyle:")]
		CGSize GetContentSizeForFrame (CGSize forFrameSize, Class horizontalScrollerClass, Class verticalScrollerClass, NSBorderType borderType, NSControlSize controlSize, NSScrollerStyle scrollerStyle);
        
        	[Mac (10, 7), Export ("findBarPosition")]
        	NSScrollViewFindBarPosition FindBarPosition { get; set; }

        	[Mac (10, 7), Export ("flashScrollers")]
        	void FlashScrollers ();
        
		[Static]
		[Mac (10, 7), Export ("frameSizeForContentSize:horizontalScrollerClass:verticalScrollerClass:borderType:controlSize:scrollerStyle:")]
		CGSize GetFrameSizeForContent (CGSize contentSize, Class horizontalScrollerClass, Class verticalScrollerClass, NSBorderType borderType, NSControlSize controlSize, NSScrollerStyle scrollerStyle);
		
		[Mac (10, 7), Export ("horizontalScrollElasticity")]
		NSScrollElasticity HorizontalScrollElasticity { get; set; }
        
        	[Mac (10, 7), Export ("scrollerKnobStyle")]
        	NSScrollerKnobStyle ScrollerKnobStyle { get; set; }
        
        	[Mac (10, 7), Export ("scrollerStyle")]
        	NSScrollerStyle ScrollerStyle { get; set; }
        
		[Mac (10, 7), Export ("usesPredominantAxisScrolling")]
        	bool UsesPredominantAxisScrolling { get; set; }

		[Mac (10, 7), Export ("verticalScrollElasticity")]
		NSScrollElasticity VerticalScrollElasticity { get; set; }

		[Mac (10, 8), Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[Mac (10, 8), Export ("magnification")]
		nfloat Magnification { get; set; }

		[Mac (10, 8), Export ("maxMagnification")]
		nfloat MaxMagnification { get; set; }

		[Mac (10, 8), Export ("minMagnification")]
		nfloat MinMagnification { get; set; }

		[Mac (10, 8), Export ("magnifyToFitRect:")]
		void MagnifyToFitRect (CGRect rect);

		[Mac (10, 8), Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centeredAtPoint);

		[Mac (10, 8), Notification, Field ("NSScrollViewWillStartLiveMagnifyNotification")]
		NSString WillStartLiveMagnifyNotification { get; }

		[Mac (10, 8), Notification, Field ("NSScrollViewDidEndLiveMagnifyNotification")]
		NSString DidEndLiveMagnifyNotification { get; }

		[Mac (10,9), Notification, Field ("NSScrollViewDidLiveScrollNotification")]
		NSString DidLiveScrollNotification { get; }

		[Mac (10,9), Notification, Field ("NSScrollViewDidEndLiveScrollNotification")]
		NSString DidEndLiveScrollNotification { get; }

		[Mac (10,10)]
		[Export ("automaticallyAdjustsContentInsets")]
		bool AutomaticallyAdjustsContentInsets { get; set; }

		// @property NSEdgeInsets contentInsets __attribute__((availability(macosx, introduced=10_10)));
		[Mac (10, 10)]
		[Export ("contentInsets", ArgumentSemantic.Assign)]
		NSEdgeInsets ContentInsets { get; set; }

		// @property NSEdgeInsets scrollerInsets __attribute__((availability(macosx, introduced=10_10)));
		[Mac (10, 10)]
		[Export ("scrollerInsets", ArgumentSemantic.Assign)]
		NSEdgeInsets ScrollerInsets { get; set; }
	}

	[BaseType (typeof (NSTextField), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSSearchFieldDelegate)})]
	interface NSSearchField {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("recentSearches")]
		string [] RecentSearches { get; set; }

		[Export ("recentsAutosaveName")]
		string RecentsAutosaveName { get; set; }

		[New, Export ("cell")]
		NSSearchFieldCell Cell { get; set; }

		[Mac (10,10)]
		[Export ("searchMenuTemplate", ArgumentSemantic.Retain)]
		NSMenu SearchMenuTemplate { get; set; }

		[Mac (10,10)]
		[Export ("sendsWholeSearchString")]
		bool SendsWholeSearchString { get; set; }

		[Mac (10,10)]
		[Export ("maximumRecents")]
		nint MaximumRecents { get; set; }

		[Mac (10,10)]
		[Export ("sendsSearchStringImmediately")]
		bool SendsSearchStringImmediately { get; set; }

		[Mac (10,11)]
		[Export ("rectForSearchTextWhenCentered:")]
		CGRect GetRectForSearchText (bool isCentered);

		[Mac (10,11)]
		[Export ("rectForSearchButtonWhenCentered:")]
		CGRect GetRectForSearchButton (bool isCentered);

		[Mac (10,11)]
		[Export ("rectForCancelButtonWhenCentered:")]
		CGRect GetRectForCancelButton (bool isCentered);

		[Wrap ("WeakDelegate")]
		[NullAllowed][Protocolize]
		NSSearchFieldDelegate Delegate { get; set; }

		[Mac (10,11)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Assign)]
		NSObject WeakDelegate { get; set; }

		[Mac (10,11)]
		[Export ("centersPlaceholder")]
		bool CentersPlaceholder { get; set; }
	}
		
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface NSSearchFieldDelegate : NSTextFieldDelegate
	{
		[Mac (10,11)]
		[Export ("searchFieldDidStartSearching:")]
		void SearchingStarted (NSSearchField sender);

		[Mac (10,11)]
		[Export ("searchFieldDidEndSearching:")]
		void SearchingEnded (NSSearchField sender);
	}

	[BaseType (typeof (NSTextFieldCell))]
	interface NSSearchFieldCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("searchButtonCell", ArgumentSemantic.Retain)]
		NSButtonCell SearchButtonCell { get; set; }

		[Export ("cancelButtonCell", ArgumentSemantic.Retain)]
		NSButtonCell CancelButtonCell { get; set; }

		[Export ("resetSearchButtonCell")]
		void ResetSearchButtonCell ();

		[Export ("resetCancelButtonCell")]
		void ResetCancelButtonCell ();

		[Export ("searchTextRectForBounds:")]
		CGRect SearchTextRectForBounds (CGRect rect);

		[Export ("searchButtonRectForBounds:")]
		CGRect SearchButtonRectForBounds (CGRect rect);

		[Export ("cancelButtonRectForBounds:")]
		CGRect CancelButtonRectForBounds (CGRect rect);

		[Export ("searchMenuTemplate", ArgumentSemantic.Retain)]
		NSMenu SearchMenuTemplate { get; set; }

		[Export ("sendsWholeSearchString")]
		bool SendsWholeSearchString { get; set; }

		[Export ("maximumRecents")]
		nint MaximumRecents { get; set; }

		[Export ("recentSearches")]
		string [] RecentSearches { get; set; }

		[Export ("recentsAutosaveName")]
		string RecentsAutosaveName { get; set; }

		[Export ("sendsSearchStringImmediately")]
		bool SendsSearchStringImmediately { get; set; }
	}
	
	[BaseType (typeof (NSControl))]
	interface NSSegmentedControl : NSUserInterfaceCompression {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("selectSegmentWithTag:")]
		bool SelectSegment (nint tag);

		[Export ("setWidth:forSegment:")]
		void SetWidth (nfloat width, nint segment);

		[Export ("widthForSegment:")]
		nfloat GetWidth (nint segment);

		[Export ("setImage:forSegment:")]
		void SetImage ([NullAllowed] NSImage image, nint segment);

		[Export ("imageForSegment:")]
		NSImage GetImage (nint segment);

		[Export ("setImageScaling:forSegment:")]
		void SetImageScaling (NSImageScaling scaling, nint segment);

		[Export ("imageScalingForSegment:")]
		NSImageScaling GetImageScaling (nint segment);

		[Export ("setLabel:forSegment:")]
		void SetLabel (string label, nint segment);

		[Export ("labelForSegment:")]
		string GetLabel (nint segment);

		[Export ("setMenu:forSegment:")]
		void SetMenu ([NullAllowed] NSMenu menu, nint segment);

		[Export ("menuForSegment:")]
		NSMenu GetMenu (nint segment);

		[Export ("setSelected:forSegment:")]
		void SetSelected (bool selected, nint segment);

		[Export ("isSelectedForSegment:")]
		bool IsSelectedForSegment (nint segment);

		[Export ("setEnabled:forSegment:")]
		void SetEnabled (bool enabled, nint segment);

		[Export ("isEnabledForSegment:")]
		bool IsEnabled (nint segment);

		//Detected properties
		[Export ("segmentCount")]
		nint SegmentCount { get; set; }

		[Export ("selectedSegment")]
		nint SelectedSegment { get; set; }

		[Export ("segmentStyle")]
		NSSegmentStyle SegmentStyle { get; set; }

		[Mac (10,10,3)]
		[Export ("springLoaded")]
		bool IsSpringLoaded { [Bind ("isSpringLoaded")] get; set; }

		[Mac (10,10,3)]
		[Export ("trackingMode")]
		NSSegmentSwitchTracking TrackingMode { get; set; }

		[Mac (10,10,3)]
		[Export ("doubleValueForSelectedSegment")]
		double GetValueForSelectedSegment (); // actually returns double

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("segmentedControlWithLabels:trackingMode:target:action:")]
		NSSegmentedControl _FromLabels (string[] labels, NSSegmentSwitchTracking trackingMode, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("segmentedControlWithImages:trackingMode:target:action:")]
		NSSegmentedControl _FromImages (NSImage[] images, NSSegmentSwitchTracking trackingMode, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10, 12, 2)]
		[NullAllowed, Export ("selectedSegmentBezelColor", ArgumentSemantic.Copy)]
		NSColor SelectedSegmentBezelColor { get; set; }

		[Mac (10,13)]
		[Export ("setToolTip:forSegment:")]
		void SetToolTip ([NullAllowed] string toolTip, nint segment);

		[Mac (10,13)]
		[Export ("toolTipForSegment:")]
		[return: NullAllowed]
		string GetToolTip (nint forSegment);

		[Mac (10,13)]
		[Export ("setTag:forSegment:")]
		void SetTag (nint tag, nint segment);

		[Mac (10,13)]
		[Export ("tagForSegment:")]
		nint GetTag (nint segment);

		[Mac (10,13)]
		[Export ("setShowsMenuIndicator:forSegment:")]
		void SetShowsMenuIndicator (bool showsMenuIndicator, nint segment);

		[Mac (10,13)]
		[Export ("showsMenuIndicatorForSegment:")]
		bool ShowsMenuIndicator (nint segment);

		[Mac (10,13)]
		[Export ("setAlignment:forSegment:")]
		void SetAlignment (NSTextAlignment alignment, nint segment);

		[Mac (10,13)]
		[Export ("alignmentForSegment:")]
		NSTextAlignment GetAlignment (nint segment);

		[Mac (10, 13)]
		[Export ("segmentDistribution", ArgumentSemantic.Assign)]
		NSSegmentDistribution SegmentDistribution { get; set; }
	}
	
	[BaseType (typeof (NSActionCell))]
	interface NSSegmentedCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("selectSegmentWithTag:")]
		bool SelectSegment (nint tag);

		[Export ("makeNextSegmentKey")]
		void InsertSegmentAfterSelection ();

		[Export ("makePreviousSegmentKey")]
		void InsertSegmentBeforeSelection ();

		[Export ("setWidth:forSegment:")]
		void SetWidth (nfloat width, nint forSegment);

		[Export ("widthForSegment:")]
		nfloat GetWidth (nint forSegment);

		[Export ("setImage:forSegment:")]
		void SetImage (NSImage image, nint forSegment);

		[Export ("imageForSegment:")]
		NSImage GetImageForSegment (nint forSegment);

		[Export ("setImageScaling:forSegment:")]
		void SetImageScaling (NSImageScaling scaling, nint forSegment);

		[Export ("imageScalingForSegment:")]
		NSImageScaling GetImageScaling (nint forSegment);

		[Export ("setLabel:forSegment:")]
		void SetLabel (string label, nint forSegment);

		[Export ("labelForSegment:")]
		string GetLabel (nint forSegment);

		[Export ("setSelected:forSegment:")]
		void SetSelected (bool selected, nint forSegment);

		[Export ("isSelectedForSegment:")]
		bool IsSelected (nint forSegment);

		[Export ("setEnabled:forSegment:")]
		void SetEnabled (bool enabled, nint forSegment);

		[Export ("isEnabledForSegment:")]
		bool IsEnabled (nint forSegment);

		[Export ("setMenu:forSegment:")]
		void SetMenu (NSMenu menu, nint forSegment);

		[Export ("menuForSegment:")]
		NSMenu GetMenu (nint forSegment);

		[Export ("setToolTip:forSegment:")]
		void SetToolTip (string toolTip, nint forSegment);

		[Export ("toolTipForSegment:")]
		string GetToolTip (nint forSegment);

		[Export ("setTag:forSegment:")]
		void SetTag (nint tag, nint forSegment);

		[Export ("tagForSegment:")]
		nint GetTag (nint forSegment);

		[Export ("drawSegment:inFrame:withView:")]
		void DrawSegment (nint segment, CGRect frame, NSView controlView);

		//Detected properties
		[Export ("segmentCount")]
		nint SegmentCount { get; set; }

		[Export ("selectedSegment")]
		nint SelectedSegment { get; set; }

		[Export ("trackingMode")]
		NSSegmentSwitchTracking TrackingMode { get; set; }

		[Export ("segmentStyle")]
		NSSegmentStyle SegmentStyle { get; set; }

	}

	[BaseType (typeof (NSControl))]
	interface NSSlider : NSAccessibilitySlider {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("vertical")]
		// Radar 27222357
#if XAMCORE_4_0
		bool
#else
		nint
#endif
		IsVertical { [Bind ("isVertical")] get; [Mac (10, 12)] set; }

		[Export ("acceptsFirstMouse:")]
		bool AcceptsFirstMouse (NSEvent theEvent);

		//Detected properties
		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("altIncrementValue")]
		double AltIncrementValue { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("titleCell")]
		NSObject TitleCell { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("titleColor")]
		NSColor TitleColor { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("titleFont")]
		NSFont TitleFont { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("title")]
		string Title { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("knobThickness")]
		nfloat KnobThickness { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("image")]
		NSImage Image { get; set; }
	
		[Export ("tickMarkValueAtIndex:")]
		double TickMarkValue (nint index);

		[Export ("rectOfTickMarkAtIndex:")]
		CGRect RectOfTick (nint index);

		[Export ("indexOfTickMarkAtPoint:")]
		nint IndexOfTickMark (CGPoint point);

		[Export ("closestTickMarkValueToValue:")]
		double ClosestTickMarkValue (double value);

		//Detected properties
		[Export ("numberOfTickMarks")]
		nint TickMarksCount { get; set; }

		[Export ("tickMarkPosition")]
		NSTickMarkPosition TickMarkPosition { get; set; }

		[Export ("allowsTickMarkValuesOnly")]
		bool AllowsTickMarkValuesOnly { get; set; }

		[Mac (10,10)]
		[Export ("sliderType")]
		NSSliderType SliderType { get; set; }

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("sliderWithTarget:action:")]
		NSSlider _FromTarget ([NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10,12)]
		[Static]
		[Internal]
		[Export ("sliderWithValue:minValue:maxValue:target:action:")]
		NSSlider _FromValue (double value, double minValue, double maxValue, [NullAllowed] NSObject target, [NullAllowed] Selector action);

		[Mac (10, 12, 2)]
		[NullAllowed, Export ("trackFillColor", ArgumentSemantic.Copy)]
		NSColor TrackFillColor { get; set; }
	}
	
	[BaseType (typeof (NSActionCell))]
	interface NSSliderCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Static]
		[Export ("prefersTrackingUntilMouseUp")]
		bool PrefersTrackingUntilMouseUp ();

		[Export ("vertical")]
		// Radar 27222357
#if XAMCORE_4_0
		bool
#else
		nint
#endif
		IsVertical { [Bind ("isVertical")] get; [Mac (10, 12)] set; }

		[Export ("knobRectFlipped:")]
		CGRect KnobRectFlipped (bool flipped);

		[Export ("drawKnob:")]
		void DrawKnob (CGRect knobRect);

		[Export ("drawKnob")]
		void DrawKnob ();

		[Export ("drawBarInside:flipped:")]
		void DrawBar (CGRect aRect, bool flipped);

		[Export ("trackRect")]
		CGRect TrackRect{ get; }

		//Detected properties
		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("altIncrementValue")]
		double AltIncrementValue { get; set; }

		[Export ("titleColor")]
		NSColor TitleColor { get; set; }

		[Export ("titleFont")]
		NSFont TitleFont { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("titleCell")]
		NSObject TitleCell { get; set; }

		[Export ("knobThickness")]
		nfloat KnobThickness { get; set; }

		[Export ("sliderType")]
		NSSliderType SliderType { get; set; }
	
		[Export ("tickMarkValueAtIndex:")]
		double TickMarkValue (nint index);

		[Export ("rectOfTickMarkAtIndex:")]
		CGRect RectOfTickMark (nint index);

		[Export ("indexOfTickMarkAtPoint:")]
		nint IndexOfTickMark (CGPoint point);

		[Export ("closestTickMarkValueToValue:")]
		double ClosestTickMarkValue (double value);

		//Detected properties
		[Export ("numberOfTickMarks")]
		nint TickMarks { get; set; }

		[Export ("tickMarkPosition")]
		NSTickMarkPosition TickMarkPosition { get; set; }

		[Export ("allowsTickMarkValuesOnly")]
		bool AllowsTickMarkValuesOnly { get; set; }

		[Mac (10,9)]
		[Export ("barRectFlipped:")]
		CGRect BarRectFlipped (bool flipped);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSSliderTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Export ("slider", ArgumentSemantic.Strong)]
		NSSlider Slider { get; set; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("minimumValueAccessory", ArgumentSemantic.Strong)]
		NSSliderAccessory MinimumValueAccessory { get; set; }

		[NullAllowed, Export ("maximumValueAccessory", ArgumentSemantic.Strong)]
		NSSliderAccessory MaximumValueAccessory { get; set; }

		[Export ("valueAccessoryWidth")]
		nfloat ValueAccessoryWidth { get; set; }

		[NullAllowed, Export ("target", ArgumentSemantic.Weak)]
		NSObject Target { get; set; }

		[NullAllowed, Export ("action", ArgumentSemantic.Assign)]
		Selector Action { get; set; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }

		[Mac (10,13)]
		[Export ("view")]
		INSUserInterfaceCompression View { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface NSSpeechRecognizer {
		[Export ("startListening")]
		void StartListening ();

		[Export ("stopListening")]
		void StopListening ();

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSSpeechRecognizerDelegate Delegate { get; set; }

		[Export ("commands")]
		string [] Commands { get; set; }

		[Export ("displayedCommandsTitle")]
		string DisplayedCommandsTitle { get; set; }

		[Export ("listensInForegroundOnly")]
		bool ListensInForegroundOnly { get; set; }

		[Export ("blocksOtherRecognizers")]
		bool BlocksOtherRecognizers { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSSpeechRecognizerDelegate {
		[Export ("speechRecognizer:didRecognizeCommand:")]
		void DidRecognizeCommand (NSSpeechRecognizer sender, string command);
	}

	[BaseType (typeof (NSObject))]
	interface NSSpeechSynthesizer {
		[Export ("initWithVoice:")]
		IntPtr Constructor (string voice);

		[Export ("startSpeakingString:")]
		bool StartSpeakingString (string theString);

		[Export ("startSpeakingString:toURL:")]
		bool StartSpeakingStringtoURL (string theString, NSUrl url);

		[Export ("isSpeaking")]
		bool IsSpeaking { get; }

		[Export ("stopSpeaking")]
		void StopSpeaking ();

		[Export ("stopSpeakingAtBoundary:")]
		void StopSpeaking (NSSpeechBoundary boundary);

		[Export ("pauseSpeakingAtBoundary:")]
		void PauseSpeaking (NSSpeechBoundary boundary);

		[Export ("continueSpeaking")]
		void ContinueSpeaking ();

		[Export ("addSpeechDictionary:")]
		void AddSpeechDictionary (NSDictionary speechDictionary);

		[Export ("phonemesFromText:")]
		string PhonemesFromText (string text);

		[Export ("objectForProperty:error:")]
		NSObject ObjectForProperty (string property, out NSError outError);

		[Export ("setObject:forProperty:error:")]
		bool SetObjectforProperty (NSObject theObject, string property, out NSError outError);

		[Static]
		[Export ("isAnyApplicationSpeaking")]
		bool IsAnyApplicationSpeaking { get; }

		[Static]
		[Export ("defaultVoice")]
		string DefaultVoice { get; }

		[Static]
		[Export ("availableVoices")]
		string [] AvailableVoices { get; }

		[Static]
		[Export ("attributesForVoice:")]
		NSDictionary AttributesForVoice (string voice);

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSSpeechSynthesizerDelegate Delegate { get; set; }

		[Export ("voice"), Protected]
		string GetVoice ();

		[Export ("setVoice:"), Protected]
		bool SetVoice (string voice);

		[Export ("rate")]
		float Rate { get; set; } /* float, not CGFloat */

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */

		[Export ("usesFeedbackWindow")]
		bool UsesFeedbackWindow { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSSpeechSynthesizerDelegate {
		[Export ("speechSynthesizer:didFinishSpeaking:")]
		void DidFinishSpeaking (NSSpeechSynthesizer sender, bool finishedSpeaking);

		[Export ("speechSynthesizer:willSpeakWord:ofString:")]
		void WillSpeakWord (NSSpeechSynthesizer sender, NSRange wordCharacterRange, string ofString);

		[Export ("speechSynthesizer:willSpeakPhoneme:")]
		void WillSpeakPhoneme (NSSpeechSynthesizer sender, short phonemeOpcode);

		[Export ("speechSynthesizer:didEncounterErrorAtIndex:ofString:message:")]
		void DidEncounterError (NSSpeechSynthesizer sender, nuint characterIndex, string theString, string message);

		[Export ("speechSynthesizer:didEncounterSyncMessage:")]
		void DidEncounterSyncMessage (NSSpeechSynthesizer sender, string message);
	}

	[StrongDictionary ("NSTextCheckingKey")]
	interface NSTextCheckingOptions {
		NSOrthography Orthography { get; set; }
		string [] Quotes { get; set; }
		NSDictionary Replacements { get; set; }
		NSDate ReferenceDate { get; set; }
		NSTimeZone ReferenceTimeZone { get; set; }
		NSUrl DocumentUrl { get; set; }
		string DocumentTitle { get; set; }
		string DocumentAuthor { get; set; }
	}

	[Internal, Static]
	interface NSTextCheckingKey {
		[Field ("NSTextCheckingOrthographyKey")]
		NSString OrthographyKey { get; }
		[Field ("NSTextCheckingQuotesKey")]
		NSString QuotesKey { get; }
		[Field ("NSTextCheckingReplacementsKey")]
		NSString ReplacementsKey { get; }
		[Field ("NSTextCheckingReferenceDateKey")]
		NSString ReferenceDateKey { get; }
		[Field ("NSTextCheckingReferenceTimeZoneKey")]
		NSString ReferenceTimeZoneKey { get; }
		[Field ("NSTextCheckingDocumentURLKey")]
		NSString DocumentUrlKey { get; }
		[Field ("NSTextCheckingDocumentTitleKey")]
		NSString DocumentTitleKey { get; }
		[Field ("NSTextCheckingDocumentAuthorKey")]
		NSString DocumentAuthorKey { get; }
	}
	
	[BaseType (typeof (NSObject))]
	partial interface NSSpellChecker {
		[Static]
		[Export ("sharedSpellChecker")]
		NSSpellChecker SharedSpellChecker { get; }

		[Static]
		[Export ("sharedSpellCheckerExists")]
		bool SharedSpellCheckerExists { get; }

		[Static]
		[Export ("uniqueSpellDocumentTag")]
		nint UniqueSpellDocumentTag { get; }

		[Export ("checkSpellingOfString:startingAt:language:wrap:inSpellDocumentWithTag:wordCount:")]
		NSRange CheckSpelling (string stringToCheck, nint startingOffset, string language, bool wrapFlag, nint documentTag, out nint wordCount);

		[Export ("checkSpellingOfString:startingAt:")]
		NSRange CheckSpelling (string stringToCheck, nint startingOffset);

		[Export ("countWordsInString:language:")]
		nint CountWords (string stringToCount, string language);

		[Export ("checkGrammarOfString:startingAt:language:wrap:inSpellDocumentWithTag:details:")]
		NSRange CheckGrammar (string stringToCheck, nint startingOffset, string language, bool wrapFlag, nint documentTag, NSDictionary[] details );

		[Export ("checkString:range:types:options:inSpellDocumentWithTag:orthography:wordCount:")]
		NSTextCheckingResult [] CheckString (string stringToCheck, NSRange range, NSTextCheckingTypes checkingTypes, [NullAllowed] NSDictionary options, nint tag, out NSOrthography orthography, out nint wordCount);

		[Wrap ("CheckString (stringToCheck, range, checkingTypes, options == null ? null : options.Dictionary, tag, out orthography, out wordCount)")]
		NSTextCheckingResult [] CheckString (string stringToCheck, NSRange range, NSTextCheckingTypes checkingTypes, NSTextCheckingOptions options, nint tag, out NSOrthography orthography, out nint wordCount);

		[Export ("requestCheckingOfString:range:types:options:inSpellDocumentWithTag:completionHandler:")]
		nint RequestChecking (string stringToCheck, NSRange range, NSTextCheckingTypes checkingTypes, [NullAllowed] NSDictionary options, nint tag, Action<nint, NSTextCheckingResult [], NSOrthography, nint> completionHandler);

		[Wrap ("RequestChecking (stringToCheck, range, checkingTypes, options == null ? null : options.Dictionary, tag, completionHandler)")]
		nint RequestChecking (string stringToCheck, NSRange range, NSTextCheckingTypes checkingTypes, NSTextCheckingOptions options, nint tag, Action<nint, NSTextCheckingResult [], NSOrthography, nint> completionHandler);
		
		[Export ("menuForResult:string:options:atLocation:inView:")]
		NSMenu MenuForResults (NSTextCheckingResult result, string checkedString, NSDictionary options, CGPoint location, NSView view);

		[Wrap ("MenuForResults (result, checkedString, options == null ? null : options.Dictionary, location, view)")]
		NSMenu MenuForResults (NSTextCheckingResult result, string checkedString, NSTextCheckingOptions options, CGPoint location, NSView view);

		[Export ("userQuotesArrayForLanguage:")]
		string [] UserQuotesArrayForLanguage (string language);

		[Export ("userReplacementsDictionary")]
		NSDictionary UserReplacementsDictionary { get; }

		[Export ("updateSpellingPanelWithMisspelledWord:")]
		void UpdateSpellingPanelWithMisspelledWord (string word);

		[Export ("updateSpellingPanelWithGrammarString:detail:")]
		void UpdateSpellingPanelWithGrammarl (string theString, NSDictionary detail);

		[Export ("spellingPanel")]
		NSPanel SpellingPanel { get; }

		[Export ("substitutionsPanel")]
		NSPanel SubstitutionsPanel { get; }

		[Export ("updatePanels")]
		void UpdatePanels ();

		[Export ("ignoreWord:inSpellDocumentWithTag:")]
		void IgnoreWord (string wordToIgnore, nint documentTag);

		[Export ("ignoredWordsInSpellDocumentWithTag:")]
		string [] IgnoredWords (nint documentTag);

		[Export ("setIgnoredWords:inSpellDocumentWithTag:")]
		void SetIgnoredWords (string [] words, nint documentTag);

		[Export ("guessesForWordRange:inString:language:inSpellDocumentWithTag:")]
		string [] GuessesForWordRange (NSRange range, string theString, string language, nint documentTag);

		[Export ("completionsForPartialWordRange:inString:language:inSpellDocumentWithTag:")]
		string [] CompletionsForPartialWordRange (NSRange range, string theString, string language, nint documentTag);

		[Export ("closeSpellDocumentWithTag:")]
		void CloseSpellDocument (nint documentTag);

		[Export ("availableLanguages")]
		string [] AvailableLanguages { get; }

		[Export ("userPreferredLanguages")]
		string [] UserPreferredLanguages { get; }

		[Export ("setWordFieldStringValue:")]
		void SetWordFieldStringValue (string aString);

		[Export ("learnWord:")]
		void LearnWord (string word);

		[Export ("hasLearnedWord:")]
		bool HasLearnedWord (string word);

		[Export ("unlearnWord:")]
		void UnlearnWord (string word);

		//Detected properties
		[Export ("accessoryView", ArgumentSemantic.Retain), NullAllowed]
		NSView AccessoryView { get; set; }

		[Export ("substitutionsPanelAccessoryViewController", ArgumentSemantic.Retain)]
		NSViewController SubstitutionsPanelAccessoryViewController { get; set; }

		[Export ("automaticallyIdentifiesLanguages")]
		bool AutomaticallyIdentifiesLanguages { get; set; }

		[Export ("language"), Protected]
		string GetLanguage ();

		[Export ("setLanguage:"), Protected]
		bool SetLanguage (string language);

		[Mac (10,9)]
		[Static, Export ("isAutomaticQuoteSubstitutionEnabled")]
		bool IsAutomaticQuoteSubstitutionEnabled ();

		[Mac (10,9)]
		[Static, Export ("isAutomaticDashSubstitutionEnabled")]
		bool IsAutomaticDashSubstitutionEnabled ();
		
		[Mac (10, 12)]
		[Static]
		[Export ("isAutomaticCapitalizationEnabled")]
		bool IsAutomaticCapitalizationEnabled { get; }

		[Mac (10, 12)]
		[Static]
		[Export ("isAutomaticPeriodSubstitutionEnabled")]
		bool IsAutomaticPeriodSubstitutionEnabled { get; }

		[Mac (10,12)]
		[Export ("preventsAutocorrectionBeforeString:language:")]
		bool PreventsAutocorrectionBefore (string aString, [NullAllowed] string language);

		[Mac (10, 12, 2)]
		[Static]
		[Export ("isAutomaticTextCompletionEnabled")]
		bool IsAutomaticTextCompletionEnabled { get; }

		[Mac (10,12,2)]
#if XAMCORE_4_0
		[Async (ResultTypeName="NSSpellCheckerCandidates")]
#else
		[Async (ResultTypeName="NSSpellCheckerCanidates")]
#endif
		[Export ("requestCandidatesForSelectedRange:inString:types:options:inSpellDocumentWithTag:completionHandler:")]
		nint RequestCandidates (NSRange selectedRange, string stringToCheck, ulong checkingTypes, [NullAllowed] NSDictionary<NSString, NSObject> options, nint tag, [NullAllowed] Action<nint, NSTextCheckingResult []> completionHandler);

		[Mac (10,12,2)]
		[Export ("deletesAutospaceBetweenString:andString:language:")]
		bool DeletesAutospace (string precedingString, string followingString, [NullAllowed] string language);

	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSSoundDelegate) })]
	[DisableDefaultCtor] // no valid handle is returned
	partial interface NSSound : NSCoding, NSCopying, NSPasteboardReading, NSPasteboardWriting
	{
		[Static]
		[Export ("soundNamed:")]
		NSSound FromName (string name);

		[Export ("initWithContentsOfURL:byReference:")]
		IntPtr Constructor (NSUrl url, bool byRef);

		[Export ("initWithContentsOfFile:byReference:")]
		IntPtr Constructor (string path, bool byRef);

		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[Static]
		[Export ("canInitWithPasteboard:")]
		bool CanCreateFromPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("soundUnfilteredTypes", ArgumentSemantic.Copy)]
		string [] SoundUnfilteredTypes ();

		[Export ("initWithPasteboard:")]
		IntPtr Constructor (NSPasteboard pasteboard);

		[Export ("writeToPasteboard:")]
		void WriteToPasteboard (NSPasteboard pasteboard);

		[Export ("play")]
		bool Play ();

		[Export ("pause")]
		bool Pause ();

		[Export ("resume")]
		bool Resume ();

		[Export ("stop")]
		bool Stop ();

		[Export ("isPlaying")]
		bool IsPlaying ();

		[Export ("duration")]
		double Duration ();

		//Detected properties
		[Export ("name"), Protected]
		string GetName ();

		[Export ("setName:"), Protected]
		bool SetName (string name);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSSoundDelegate Delegate { get; set; }

		[Export ("volume")]
		float Volume { get; set; } /* float, not CGFloat */

		[Export ("currentTime")]
		double CurrentTime { get; set; }

		[Export ("loops")]
		bool Loops { get; set; }

		[Export ("playbackDeviceIdentifier")]
		string PlaybackDeviceID { get; set; }

		// FIXME: Poor docs, no type defined for the array elements
		[Export ("channelMapping")]
		NSObject ChannelMapping { get; set; }
	}

	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSSoundDelegate {
		[Export ("sound:didFinishPlaying:"), EventArgs ("NSSoundFinished")]
		void DidFinishPlaying (NSSound sound, bool finished);
	}

	[BaseType (typeof (NSView))]
	partial interface NSSplitView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("drawDividerInRect:")]
		void DrawDivider (CGRect rect);

		[Export ("dividerColor")]
		NSColor DividerColor { get; }

		[Export ("dividerThickness")]
		nfloat DividerThickness { get; }

		[Export ("adjustSubviews")]
		void AdjustSubviews ();

		[Export ("isSubviewCollapsed:")]
		bool IsSubviewCollapsed (NSView subview);

		[Export ("minPossiblePositionOfDividerAtIndex:")]
		nfloat MinPositionOfDivider (nint dividerIndex);

		[Export ("maxPossiblePositionOfDividerAtIndex:")]
		nfloat MaxPositionOfDivider (nint dividerIndex);

		[Export ("setPosition:ofDividerAtIndex:")]
		void SetPositionOfDivider (nfloat position, nint dividerIndex);

		//Detected properties
		[Export ("vertical")]
		bool IsVertical { [Bind ("isVertical")]get; set; }

		[Export ("dividerStyle")]
		NSSplitViewDividerStyle DividerStyle { get; set; }

		[Export ("autosaveName")]
		string AutosaveName { get; set; }
		
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSSplitViewDelegate Delegate { get; set; }

		[Mac (10,11)]
		[Export ("arrangesAllSubviews")]
		bool ArrangesAllSubviews { get; set; }

		[Mac (10,11)]
		[Export ("arrangedSubviews", ArgumentSemantic.Copy)]
		NSView[] ArrangedSubviews { get; }

		[Mac (10,11)]
		[Export ("addArrangedSubview:")]
		void AddArrangedSubview (NSView view);

		[Mac (10,11)]
		[Export ("insertArrangedSubview:atIndex:")]
		void InsertArrangedSubview (NSView view, nint index);

		[Mac (10,11)]
		[Export ("removeArrangedSubview:")]
		void RemoveArrangedSubview (NSView view);

		[Mac (10, 8), Export ("holdingPriorityForSubviewAtIndex:")]
		#if XAMCORE_2_0
				float /*NSLayoutPriority*/ HoldingPriorityForSubview (nint subviewIndex);
#else
		float /*NSLayoutPriority*/ HoldingPriorityForSubviewAtIndex (nint subviewIndex);
		#endif

		[Mac (10, 8), Export ("setHoldingPriority:forSubviewAtIndex:")]
		void SetHoldingPriority (float /*NSLayoutPriority*/ priority, nint subviewIndex);

		[Notification (typeof (NSSplitViewDividerIndexEventArgs))]
		[Field ("NSSplitViewWillResizeSubviewsNotification")]
		NSString NSSplitViewWillResizeSubviewsNotification { get; }

		[Notification (typeof (NSSplitViewDividerIndexEventArgs))]
		[Field ("NSSplitViewDidResizeSubviewsNotification")]
		NSString NSSplitViewDidResizeSubviewsNotification { get; }
	}

	interface INSSplitViewDelegate {}

	[Mac (10,10)]
	[BaseType (typeof (NSViewController))]
	interface NSSplitViewController : NSSplitViewDelegate, NSUserInterfaceValidations {
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("splitView", ArgumentSemantic.Strong)]
		NSSplitView SplitView { get; set; }

		[Export ("splitViewItems", ArgumentSemantic.Copy)]
		NSSplitViewItem [] SplitViewItems { get; set; }

		[Export ("addSplitViewItem:")][PostGet ("SplitViewItems")]
		void AddSplitViewItem (NSSplitViewItem splitViewItem);

		[Export ("insertSplitViewItem:atIndex:")][PostGet ("SplitViewItems")]
		void InsertSplitViewItem (NSSplitViewItem splitViewItem, nint index);

		[Export ("removeSplitViewItem:")][PostGet ("SplitViewItems")]
		void RemoveSplitViewItem (NSSplitViewItem splitViewItem);

		[Export ("splitViewItemForViewController:")][PostGet ("SplitViewItems")]
		NSSplitViewItem GetSplitViewItem (NSViewController viewController);

		[Mac (10,11)]
		[Export ("minimumThicknessForInlineSidebars", ArgumentSemantic.Assign)]
		nfloat MinimumThicknessForInlineSidebars { get; set; }

		[Mac (10,11)]
		[Export ("toggleSidebar:")]
		void ToggleSidebar ([NullAllowed] NSObject sender);

		[Mac (10,11)]
		[Field ("NSSplitViewControllerAutomaticDimension")]
		nfloat AutomaticDimension { get; }

		// 'new' since it's inlined from NSSplitViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("splitView:additionalEffectiveRectOfDividerAtIndex:")]
		new CGRect GetAdditionalEffectiveRect (NSSplitView splitView, nint dividerIndex);

		// 'new' since it's inlined from NSSplitViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("splitView:canCollapseSubview:")]
		new bool CanCollapse (NSSplitView splitView, NSView subview);

		// 'new' since it's inlined from NSSplitViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("splitView:effectiveRect:forDrawnRect:ofDividerAtIndex:")]
		new CGRect GetEffectiveRect (NSSplitView splitView, CGRect proposedEffectiveRect, CGRect drawnRect, nint dividerIndex);

		// 'new' since it's inlined from NSSplitViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("splitView:shouldHideDividerAtIndex:")]
		new bool ShouldHideDivider (NSSplitView splitView, nint dividerIndex);

		// 'new' since it's inlined from NSSplitViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("splitView:shouldCollapseSubview:forDoubleClickOnDividerAtIndex:")]
		new bool ShouldCollapseForDoubleClick (NSSplitView splitView, NSView subview, nint doubleClickAtDividerIndex);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface NSSplitViewItem : NSAnimatablePropertyContainer, NSCoding {
		[Export ("viewController", ArgumentSemantic.Strong)]
		NSViewController ViewController { get; set; }

		[Export ("collapsed")]
		bool Collapsed { [Bind ("isCollapsed")] get; set; }

		[Export ("canCollapse")]
		bool CanCollapse { get; set; }

		[Export ("holdingPriority")]
		float HoldingPriority { get; set; } /* NSLayoutPriority = float */

		[Static, Export ("splitViewItemWithViewController:")]
		NSSplitViewItem FromViewController (NSViewController viewController);

		[Mac (10,11)]
		[Static]
		[Export ("sidebarWithViewController:")]
		NSSplitViewItem CreateSidebar (NSViewController viewController);

		[Mac (10,11)]
		[Static]
		[Export ("contentListWithViewController:")]
		NSSplitViewItem CreateContentList (NSViewController viewController);

		[Mac (10,11)]
		[Export ("behavior")]
		NSSplitViewItemBehavior Behavior { get; }

		[Mac (10,11)]
		[Export ("minimumThickness", ArgumentSemantic.Assign)]
		nfloat MinimumThickness { get; set; }

		[Mac (10,11)]
		[Export ("maximumThickness", ArgumentSemantic.Assign)]
		nfloat MaximumThickness { get; set; }

		[Mac (10,11)]
		[Export ("preferredThicknessFraction", ArgumentSemantic.Assign)]
		nfloat PreferredThicknessFraction { get; set; }

		[Mac (10,11)]
		[Export ("automaticMaximumThickness", ArgumentSemantic.Assign)]
		nfloat AutomaticMaximumThickness { get; set; }

		[Mac (10,11)]
		[Export ("springLoaded")]
		bool SpringLoaded { [Bind ("isSpringLoaded")] get; set; }

		[Mac (10,11)]
		[Field ("NSSplitViewItemUnspecifiedDimension")]
		nfloat UnspecifiedDimension { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface NSSplitViewDelegate {
		[Export ("splitView:canCollapseSubview:") ] [DefaultValue (true)]
		bool CanCollapse (NSSplitView splitView, NSView subview);

		[Export ("splitView:shouldCollapseSubview:forDoubleClickOnDividerAtIndex:")] [DefaultValue (true)]
		bool ShouldCollapseForDoubleClick (NSSplitView splitView, NSView subview, nint doubleClickAtDividerIndex);

		[Export ("splitView:constrainMinCoordinate:ofSubviewAt:")]
		nfloat SetMinCoordinateOfSubview (NSSplitView splitView, nfloat proposedMinimumPosition, nint subviewDividerIndex);

		[Export ("splitView:constrainMaxCoordinate:ofSubviewAt:")]
		nfloat SetMaxCoordinateOfSubview (NSSplitView splitView, nfloat proposedMaximumPosition, nint subviewDividerIndex);

		[Export ("splitView:constrainSplitPosition:ofSubviewAt:")]
		nfloat ConstrainSplitPosition (NSSplitView splitView, nfloat proposedPosition, nint subviewDividerIndex);

		[Export ("splitView:resizeSubviewsWithOldSize:")]
		void Resize (NSSplitView splitView, CGSize oldSize);

		[Export ("splitView:shouldAdjustSizeOfSubview:")][DefaultValue (true)]
		bool ShouldAdjustSize (NSSplitView splitView, NSView view);

		[Export ("splitView:shouldHideDividerAtIndex:")] [DefaultValue (false)]
		bool ShouldHideDivider (NSSplitView splitView, nint dividerIndex);

		[Export ("splitView:effectiveRect:forDrawnRect:ofDividerAtIndex:")]
		CGRect GetEffectiveRect (NSSplitView splitView, CGRect proposedEffectiveRect, CGRect drawnRect, nint dividerIndex);

		[Export ("splitView:additionalEffectiveRectOfDividerAtIndex:")]
		CGRect GetAdditionalEffectiveRect (NSSplitView splitView, nint dividerIndex);

		[Export ("splitViewWillResizeSubviews:")]
		void SplitViewWillResizeSubviews (NSNotification notification);

		[Export ("splitViewDidResizeSubviews:")]
		void DidResizeSubviews (NSNotification notification);
	}

	[Mac (10,11)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSSpringLoadingDestination
	{
		[Abstract]
		[Export ("springLoadingActivated:draggingInfo:")]
		void Activated (bool activated, [Protocolize (4)] NSDraggingInfo draggingInfo);

		[Abstract]
		[Export ("springLoadingHighlightChanged:")]
		void HighlightChanged ([Protocolize (4)] NSDraggingInfo draggingInfo);

		[Export ("springLoadingEntered:")]
		NSSpringLoadingOptions Entered ([Protocolize (4)] NSDraggingInfo draggingInfo);

		[Export ("springLoadingUpdated:")]
		NSSpringLoadingOptions Updated ([Protocolize (4)] NSDraggingInfo draggingInfo);

		[Export ("springLoadingExited:")]
		void Exited ([Protocolize (4)] NSDraggingInfo draggingInfo);

		[Export ("draggingEnded:")]
		void DraggingEnded ([Protocolize (4)] NSDraggingInfo draggingInfo);
	}

	[Mac (10,9)]
	[BaseType (typeof (NSView))]
	interface NSStackView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSStackViewDelegate Delegate { get; set; }

		[Export ("orientation")]
		NSUserInterfaceLayoutOrientation Orientation { get; set; }

		[Export ("alignment")]
		NSLayoutAttribute Alignment { get; set; }

		[Export ("edgeInsets")]
		NSEdgeInsets EdgeInsets { get; set; }

		[Export ("views", ArgumentSemantic.Copy)]
		NSView [] Views { get; }

		[Export ("detachedViews", ArgumentSemantic.Copy)]
		NSView [] DetachedViews { get; }

		[Export ("spacing")]
		nfloat Spacing { get; set; }

		[Availability (Introduced = Platform.Mac_10_9, Deprecated = Platform.Mac_10_11, Message = "Set Distribution to NSStackViewDistribution.EqualSpacing instead.")]
		[Export ("hasEqualSpacing")]
		bool HasEqualSpacing { get; set; }

		[Static, Export ("stackViewWithViews:")]
		NSStackView FromViews (NSView [] views);

		[Export ("addView:inGravity:")][PostGet ("Views")]
		void AddView (NSView aView, NSStackViewGravity gravity);

		[Export ("insertView:atIndex:inGravity:")][PostGet ("Views")]
		void InsertView (NSView aView, nuint index, NSStackViewGravity gravity);

		[Export ("removeView:")][PostGet ("Views")]
		void RemoveView (NSView aView);

		[Export ("viewsInGravity:")]
		NSView [] ViewsInGravity (NSStackViewGravity gravity);

		[Export ("setViews:inGravity:")][PostGet ("Views")] 
		void SetViews (NSView [] views, NSStackViewGravity gravity);

		[Export ("setVisibilityPriority:forView:")]
		void SetVisibilityPriority (float /* NSStackViewVisibilityPriority = float */ priority, NSView aView);

		[Export ("visibilityPriorityForView:")]
		float /* NSStackViewVisibilityPriority = float */ VisibilityPriority (NSView aView);

		[Export ("setCustomSpacing:afterView:")]
		void SetCustomSpacing (nfloat spacing, NSView aView);

		[Export ("customSpacingAfterView:")]
		nfloat CustomSpacingAfterView (NSView aView);

		[Export ("clippingResistancePriorityForOrientation:")]
		float /* NSLayoutPriority = float */ ClippingResistancePriorityForOrientation (NSLayoutConstraintOrientation orientation);

		[Export ("setClippingResistancePriority:forOrientation:")]
		void SetClippingResistancePriority (float /* NSLayoutPriority = float */ clippingResistancePriority, NSLayoutConstraintOrientation orientation);

		[Export ("huggingPriorityForOrientation:")]
		float /* NSLayoutPriority = float */ HuggingPriority (NSLayoutConstraintOrientation orientation);

		[Export ("setHuggingPriority:forOrientation:")]
		void SetHuggingPriority (float /* NSLayoutPriority = float */ huggingPriority, NSLayoutConstraintOrientation orientation);

		[Mac (10,11)]
		[Export ("detachesHiddenViews")]
		bool DetachesHiddenViews { get; set; }

		[Mac (10,11)]
		[Export ("distribution", ArgumentSemantic.Assign)]
		NSStackViewDistribution Distribution { get; set; }

		[Mac (10,11)]
		[Export ("arrangedSubviews", ArgumentSemantic.Copy)]
//		[Verify (StronglyTypedNSArray)]
		NSView[] ArrangedSubviews { get; }

		[Mac (10,11)]
		[Export ("addArrangedSubview:")]
		void AddArrangedSubview (NSView view);

		[Mac (10,11)]
		[Export ("insertArrangedSubview:atIndex:")]
		void InsertArrangedSubview (NSView view, nint index);

		[Mac (10,11)]
		[Export ("removeArrangedSubview:")]
		void RemoveArrangedSubview (NSView view);
	}

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSStackViewDelegate {
		[Export ("stackView:willDetachViews:"), DelegateName ("NSStackViewEvent")]
		void WillDetachViews (NSStackView stackView, NSView [] views);

		[Export ("stackView:didReattachViews:"), DelegateName ("NSStackViewEvent")]
		void DidReattachViews (NSStackView stackView, NSView [] views);
	}

	[BaseType (typeof (NSObject))]
	partial interface NSStatusBar {
		[Static, Export ("systemStatusBar")]
		NSStatusBar SystemStatusBar { get; }

		[Export ("statusItemWithLength:")]
		NSStatusItem CreateStatusItem (nfloat length);

		[Export ("removeStatusItem:")]
		void RemoveStatusItem (NSStatusItem item);

		[Export ("isVertical")]
		bool IsVertical { get; }

		[Export ("thickness")]
		nfloat Thickness { get; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSButton))]
	interface NSStatusBarButton {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("appearsDisabled")]
		bool AppearsDisabled { get; set; }
	}
	
	[BaseType (typeof (NSObject))]
	[PrivateDefaultCtor]
	partial interface NSStatusItem {
		[Export ("statusBar")]
		NSStatusBar StatusBar { get; }

		[Export ("length")]
		nfloat Length { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("action"), NullAllowed]
		Selector Action { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("sendActionOn:")]
		nint SendActionOn (NSTouchPhase mask);

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("popUpStatusItemMenu:")]
		void PopUpStatusItemMenu (NSMenu menu);

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("drawStatusBarBackgroundInRect:withHighlight:")]
		void DrawStatusBarBackground (CGRect rect, bool highlight);

		//Detected properties
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("target", ArgumentSemantic.Assign), NullAllowed]
		NSObject Target { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("title")]
		string Title { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("attributedTitle")]
		NSAttributedString AttributedTitle { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("image")]
		NSImage Image { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("alternateImage")]
		NSImage AlternateImage { get; set; }

		[Export ("menu", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSMenu Menu { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("toolTip")]
		string ToolTip { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("highlightMode")]
		bool HighlightMode { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10, Message = "Soft-deprecation, forwards message to button, but will be gone in the future.")]
		[Export ("view")]
		NSView View { get; [NullAllowed] set; }

		[Mac (10,10)]
		[Export ("button", ArgumentSemantic.Retain)]
		NSStatusBarButton Button { get; }

		[Mac (10, 12)]
		[Export ("behavior", ArgumentSemantic.Assign)]
		NSStatusItemBehavior Behavior { get; set; }

		[Mac (10, 12)]
		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; set; }

		[Mac (10, 12)]
		[Export ("autosaveName")]
		string AutosaveName { get; set; }
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSShadow : NSCoding, NSCopying {
		[Export ("set")]
		void Set ();

		//Detected properties
		[Export ("shadowOffset")]
		CGSize ShadowOffset { get; set; }

		[Export ("shadowBlurRadius")]
		nfloat ShadowBlurRadius { get; set; }

		[Export ("shadowColor", ArgumentSemantic.Copy)]
		NSColor ShadowColor { get; set; }

	}

	[Static]
	interface NSStringAttributeKey {
		[Field ("NSFontAttributeName")]
		NSString Font { get; }

		[Field ("NSParagraphStyleAttributeName")]
		NSString ParagraphStyle { get; }

		[Field ("NSForegroundColorAttributeName")]
		NSString ForegroundColor { get; }

		[Field ("NSUnderlineStyleAttributeName")]
		NSString UnderlineStyle { get; }

		[Field ("NSSuperscriptAttributeName")]
		NSString Superscript { get; }

		[Field ("NSBackgroundColorAttributeName")]
		NSString BackgroundColor { get; }

		[Field ("NSAttachmentAttributeName")]
		NSString Attachment { get; }

		[Field ("NSLigatureAttributeName")]
		NSString Ligature { get; }

		[Field ("NSBaselineOffsetAttributeName")]
		NSString BaselineOffset { get; }

		[Field ("NSKernAttributeName")]
		NSString KerningAdjustment { get; }

		[Field ("NSLinkAttributeName")]
		NSString Link { get; }

		[Field ("NSStrokeWidthAttributeName")]
		NSString StrokeWidth { get; }

		[Field ("NSStrokeColorAttributeName")]
		NSString StrokeColor { get; }

		[Field ("NSUnderlineColorAttributeName")]
		NSString UnderlineColor { get; }

		[Field ("NSStrikethroughStyleAttributeName")]
		NSString StrikethroughStyle { get; }

		[Field ("NSStrikethroughColorAttributeName")]
		NSString StrikethroughColor { get; }

		[Field ("NSShadowAttributeName")]
		NSString Shadow { get; }

		[Field ("NSObliquenessAttributeName")]
		NSString Obliqueness { get; }

		[Field ("NSExpansionAttributeName")]
		NSString Expansion { get; }

		[Field ("NSCursorAttributeName")]
		NSString Cursor { get; }

		[Field ("NSToolTipAttributeName")]
		NSString ToolTip { get; }

		[Field ("NSCharacterShapeAttributeName")]
		NSString CharacterShape { get; }

		[Field ("NSGlyphInfoAttributeName")]
		NSString GlyphInfo { get; }

		[Field ("NSWritingDirectionAttributeName")]
		NSString WritingDirection { get; }

		[Field ("NSMarkedClauseSegmentAttributeName")]
		NSString MarkedClauseSegment { get; }

		[Field ("NSSpellingStateAttributeName")]
		NSString SpellingState { get; }

		[Mac (10,7)]
		[Field ("NSVerticalGlyphFormAttributeName")]
		NSString VerticalGlyphForm { get; }

		[Mac (10,8)]
		[Field ("NSTextAlternativesAttributeName")]
		NSString TextAlternatives { get; }

		[Mac (10,10)]
		[Field ("NSTextEffectAttributeName")]
		NSString TextEffect { get; }

		// Internal
		[Internal, Field ("NSDocumentTypeDocumentOption")]
		NSString NSDocumentTypeDocumentOption { get; }

		[Internal, Field ("NSDefaultAttributesDocumentOption")]
		NSString NSDefaultAttributesDocumentOption { get; }

		[Internal, Field ("NSCharacterEncodingDocumentOption")]
		NSString NSCharacterEncodingDocumentOption { get; }

		[Internal, Field ("NSTextEncodingNameDocumentOption")]
		NSString NSTextEncodingNameDocumentOption { get; }

		[Internal, Field ("NSBaseURLDocumentOption")]
		NSString NSBaseURLDocumentOption { get; }

		[Internal, Field ("NSTimeoutDocumentOption")]
		NSString NSTimeoutDocumentOption { get; }

		[Internal, Field ("NSWebPreferencesDocumentOption")]
		NSString NSWebPreferencesDocumentOption { get; }

		[Internal, Field ("NSWebResourceLoadDelegateDocumentOption")]
		NSString NSWebResourceLoadDelegateDocumentOption { get; }

		[Internal, Field ("NSTextSizeMultiplierDocumentOption")]
		NSString NSTextSizeMultiplierDocumentOption { get; }

		[Internal, Field ("NSFileTypeDocumentOption")]
		NSString NSFileTypeDocumentOption { get; }

		[Internal, Field ("NSPlainTextDocumentType")]
		NSString NSPlainTextDocumentType { get; }

		[Internal, Field ("NSRTFTextDocumentType")]
		NSString NSRtfTextDocumentType { get; }

		[Internal, Field ("NSRTFDTextDocumentType")]
		NSString NSRtfdTextDocumentType { get; }

		[Internal, Field ("NSMacSimpleTextDocumentType")]
		NSString NSMacSimpleTextDocumentType { get; }

		[Internal, Field ("NSHTMLTextDocumentType")]
		NSString NSHTMLTextDocumentType { get; }

		[Internal, Field ("NSDocFormatTextDocumentType")]
		NSString NSDocFormatTextDocumentType { get; }

		[Internal, Field ("NSWordMLTextDocumentType")]
		NSString NSWordMLTextDocumentType { get; }

		[Internal, Field ("NSWebArchiveTextDocumentType")]
		NSString NSWebArchiveTextDocumentType { get; }

		[Internal, Field ("NSOfficeOpenXMLTextDocumentType")]
		NSString NSOfficeOpenXMLTextDocumentType { get; }

		[Internal, Field ("NSOpenDocumentTextDocumentType")]
		NSString NSOpenDocumentTextDocumentType { get; }

		[Mac (10,11)]
		[Internal, Field ("NSDefaultAttributesDocumentAttribute")]
		NSString NSDefaultAttributesDocumentAttribute { get; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface NSStoryboard {
		[Static, Export ("storyboardWithName:bundle:")]
		NSStoryboard FromName (string name, [NullAllowed] NSBundle storyboardBundleOrNil);

		[Export ("instantiateInitialController")]
		NSObject InstantiateInitialController ();

		[Export ("instantiateControllerWithIdentifier:")]
		NSObject InstantiateControllerWithIdentifier (string identifier);

		[Mac (10, 13)]
		[Static]
		[NullAllowed, Export ("mainStoryboard", ArgumentSemantic.Strong)]
		NSStoryboard MainStoryboard { get; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSObject))]
	interface NSStoryboardSegue {
		[DesignatedInitializer]
		[Export ("initWithIdentifier:source:destination:")]
		IntPtr Constructor (string identifier, NSObject sourceController, NSObject destinationController);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("sourceController", ArgumentSemantic.Strong)]
		NSObject SourceController { get; }

		[Export ("destinationController", ArgumentSemantic.Strong)]
		NSObject DestinationController { get; }

		[Static, Export ("segueWithIdentifier:source:destination:performHandler:")]
		NSStoryboardSegue FromIdentifier (string identifier, NSObject sourceController, NSObject destinationController, Action performHandler);

		[Export ("perform")]
		void Perform ();
	}

	[Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSSeguePerforming {
		[Export ("prepareForSegue:sender:")]
		void PrepareForSegue (NSStoryboardSegue segue, NSObject sender);

		[Export ("performSegueWithIdentifier:sender:")]
		void PerformSegue (string identifier, NSObject sender);

		[Export ("shouldPerformSegueWithIdentifier:sender:")]
		bool ShouldPerformSegue (string identifier, NSObject sender);
	}

	[BaseType (typeof (NSController))]
	interface NSUserDefaultsController {
		[DesignatedInitializer]
		[Export ("initWithDefaults:initialValues:")]
		IntPtr Constructor ([NullAllowed] NSUserDefaults defaults, [NullAllowed] NSDictionary initialValues);
//
//		[Export ("initWithCoder:")]
//		IntPtr Constructor (NSCoder coder);
//
		[Export ("defaults", ArgumentSemantic.Strong)]
		NSUserDefaults Defaults { get; }

		[Export ("initialValues", ArgumentSemantic.Copy)]
		NSDictionary InitialValues { get; set; }

		[Export ("appliesImmediately")]
		bool AppliesImmediately { get; set; }

		[Export ("hasUnappliedChanges")]
		bool HasUnappliedChanges { get; }

		[Export ("values", ArgumentSemantic.Strong)]
		NSObject Values { get; }

		[Static, Export ("sharedUserDefaultsController")]
		NSUserDefaultsController SharedUserDefaultsController { get; }

		[Export ("revert:")]
		void Revert (NSObject sender);

		[Export ("save:")]
		void Save (NSObject sender);

		[Export ("revertToInitialValues:")]
		void RevertToInitialValues (NSObject sender);
	}

	interface INSUserInterfaceItemIdentification {}

	[Protocol]
	interface NSUserInterfaceItemIdentification {
		[Mac (10, 7), Export ("identifier", ArgumentSemantic.Copy)]
		string Identifier { get; set; }
	}

	[Mac (10, 7)]
	[Protocol]
#if !XAMCORE_4_0
	[Model]
	[BaseType (typeof (NSObject))]
#endif
	partial interface NSTextFinderClient {
#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("string", ArgumentSemantic.Copy)]
		string String { get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("firstSelectedRange")]
		NSRange FirstSelectedRange { get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("selectedRanges", ArgumentSemantic.Copy)]
		NSArray SelectedRanges { get; set;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("visibleCharacterRanges", ArgumentSemantic.Copy)]
		NSArray VisibleCharacterRanges { get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")] get;  }

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("stringAtIndex:effectiveRange:endsWithSearchBoundary:")]
		string StringAtIndexeffectiveRangeendsWithSearchBoundary (nuint characterIndex, ref NSRange outRange, bool outFlag);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("stringLength")]
		nuint StringLength ();

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("scrollRangeToVisible:")]
		void ScrollRangeToVisible (NSRange range);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("shouldReplaceCharactersInRanges:withStrings:")]
		bool ShouldReplaceCharactersInRangeswithStrings (NSArray ranges, NSArray strings);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceCharactersInRangewithString (NSRange range, string str);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("didReplaceCharacters")]
		void DidReplaceCharacters ();

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("contentViewAtIndex:effectiveCharacterRange:")]
		NSView ContentViewAtIndexeffectiveCharacterRange (nuint index, ref NSRange outRange);

#if !XAMCORE_4_0
		[Abstract]
#endif
		[Export ("rectsForCharacterRange:")]
		NSArray RectsForCharacterRange (NSRange range);

#if !XAMCORE_4_0
		[Abstract]
		[Export ("drawCharactersInRange:forContentView:")]
		void DrawCharactersInRangeforContentView (NSRange range, NSView view);
#else
		[Export ("drawCharactersInRange:forContentView:")]
		void DrawCharacters (NSRange range, NSView view);
#endif

	}

	[BaseType (typeof (NSObject)), Model, Protocol]
	partial interface NSTextFinderBarContainer {
		[Abstract, Export ("findBarVisible"), Mac (10, 7)]
		bool FindBarVisible { [Bind ("isFindBarVisible")] get; set;  }

		[Abstract, Export ("findBarView", ArgumentSemantic.Retain), Mac (10, 7)]
		NSView FindBarView { get; set; }

		[Abstract, Export ("findBarViewDidChangeHeight"), Mac (10, 7)]
		void FindBarViewDidChangeHeight ();

		[Export ("contentView")]
		NSView ContentView { get; }
	}

	[DesignatedDefaultCtor]
	[Mac (10, 7)]
	[BaseType (typeof (NSObject))]
	partial interface NSTextFinder : NSCoding {
		[Export ("client", ArgumentSemantic.Assign)]
		[Protocolize]
		NSTextFinderClient Client { set; }

		[Export ("findBarContainer", ArgumentSemantic.Assign)]
		[Protocolize]
		NSTextFinderBarContainer FindBarContainer { set; }

		[Export ("findIndicatorNeedsUpdate")]
		bool FindIndicatorNeedsUpdate { get; set; }

		[Export ("incrementalSearchingEnabled")]
		bool IncrementalSearchingEnabled { [Bind ("isIncrementalSearchingEnabled")] get; set;  }

		[Export ("incrementalMatchRanges", ArgumentSemantic.Copy)]
		NSArray IncrementalMatchRanges { get;  }

		[Export ("performAction:")]
		void PerformAction (NSTextFinderAction op);

		[Export ("validateAction:")]
		bool ValidateAction (NSTextFinderAction op);

		[Export ("cancelFindIndicator")]
		void CancelFindIndicator ();

		[Static]
		[Export ("drawIncrementalMatchHighlightInRect:")]
		void DrawIncrementalMatchHighlightInRect (CGRect rect);

		[Export ("noteClientStringWillChange")]
		void NoteClientStringWillChange ();
	}

	[BaseType (typeof (NSResponder))]
	[Dispose ("__mt_tracking_var = null;")]
	partial interface NSView : NSDraggingDestination, NSAnimatablePropertyContainer, NSUserInterfaceItemIdentification, NSAppearanceCustomization, NSAccessibilityElementProtocol, NSAccessibility, NSObjectAccessibilityExtensions {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("window")]
		NSWindow Window { get; }

		[Export ("superview")]
		NSView Superview { get; }

		[Export ("isDescendantOf:")]
		bool IsDescendantOf (NSView aView);

		[Export ("ancestorSharedWithView:")]
		NSView AncestorSharedWithView (NSView aView);

		[Export ("opaqueAncestor")]
		NSView OpaqueAncestor { get; }

		[Export ("isHiddenOrHasHiddenAncestor")]
		bool IsHiddenOrHasHiddenAncestor { get; }

		//[Export ("getRectsBeingDrawn:count:")]
		// void GetRectsBeingDrawn

		[Export ("needsToDrawRect:")]
		bool NeedsToDraw (CGRect aRect);

		[Export ("wantsDefaultClipping")]
		bool WantsDefaultClipping { get; }

		[Export ("viewDidHide")]
		void ViewDidHide ();

		[Export ("viewDidUnhide")]
		void ViewDidUnhide ();

		[Export ("addSubview:")][PostGet ("Subviews")]
		void AddSubview (NSView aView);

		[Export ("addSubview:positioned:relativeTo:")][PostGet ("Subviews")]
		void AddSubview (NSView aView, NSWindowOrderingMode place, [NullAllowed] NSView otherView);

		[Export ("viewWillMoveToWindow:")]
		void ViewWillMoveToWindow ([NullAllowed] NSWindow newWindow);

		[Export ("viewDidMoveToWindow")]
		void ViewDidMoveToWindow ();

		[Export ("viewWillMoveToSuperview:")]
		void ViewWillMoveToSuperview ([NullAllowed] NSView newSuperview);

		[Export ("viewDidMoveToSuperview")]
		void ViewDidMoveToSuperview ();

		[Export ("didAddSubview:")]
		void DidAddSubview ([NullAllowed] NSView subview);

		[Export ("willRemoveSubview:")]
		void WillRemoveSubview ([NullAllowed] NSView subview);

		[Export ("removeFromSuperview")]
		[PreSnippet ("var mySuper = Superview;")]
		[PostSnippet ("if (mySuper != null) {\n\t#pragma warning disable 168\n\tvar flush = mySuper.Subviews;\n#pragma warning restore 168\n\t}")]
		void RemoveFromSuperview ();

		[Export ("replaceSubview:with:")][PostGet ("Subviews")]
		void ReplaceSubviewWith (NSView oldView, NSView newView);

		[Export ("removeFromSuperviewWithoutNeedingDisplay")]
		[PreSnippet ("var mySuper = Superview;")]
		[PostSnippet ("if (mySuper != null) {\n\t#pragma warning disable 168\n\tvar flush = mySuper.Subviews;\n#pragma warning restore 168\n\t}")]
		void RemoveFromSuperviewWithoutNeedingDisplay ();

		[Export ("resizeSubviewsWithOldSize:")]
		void ResizeSubviewsWithOldSize (CGSize oldSize);

		[Export ("resizeWithOldSuperviewSize:")]
		void ResizeWithOldSuperviewSize (CGSize oldSize);

		[Export ("setFrameOrigin:")]
		void SetFrameOrigin (CGPoint newOrigin);

		[Export ("setFrameSize:")]
		void SetFrameSize (CGSize newSize);

		[Export ("setBoundsOrigin:")]
		void SetBoundsOrigin (CGPoint newOrigin);

		[Export ("setBoundsSize:")]
		void SetBoundsSize (CGSize newSize);

		[Export ("translateOriginToPoint:")]
		void TranslateOriginToPoint (CGPoint translation);

		[Export ("scaleUnitSquareToSize:")]
		void ScaleUnitSquareToSize (CGSize newUnitSize);

		[Export ("rotateByAngle:")]
		void RotateByAngle (nfloat angle);

		[Export ("isFlipped")]
		bool IsFlipped { get; }

		[Export ("isRotatedFromBase")]
		bool IsRotatedFromBase { get; }

		[Export ("isRotatedOrScaledFromBase")]
		bool IsRotatedOrScaledFromBase { get; }

		[Export ("isOpaque")]
		bool IsOpaque { get; }

		[Export ("convertPoint:fromView:")]
		CGPoint ConvertPointFromView (CGPoint aPoint, [NullAllowed] NSView aView);

		[Export ("convertPoint:toView:")]
		CGPoint ConvertPointToView (CGPoint aPoint, [NullAllowed] NSView aView);

		[Export ("convertSize:fromView:")]
		CGSize ConvertSizeFromView (CGSize aSize, [NullAllowed] NSView aView);

		[Export ("convertSize:toView:")]
		CGSize ConvertSizeToView (CGSize aSize, [NullAllowed] NSView aView);

		[Export ("convertRect:fromView:")]
		CGRect ConvertRectFromView (CGRect aRect, [NullAllowed] NSView aView);

		[Export ("convertRect:toView:")]
		CGRect ConvertRectToView (CGRect aRect, [NullAllowed] NSView aView);

		[Export ("centerScanRect:")]
		CGRect CenterScanRect (CGRect aRect);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertPointToBase:")]
		CGPoint ConvertPointToBase (CGPoint aPoint);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertPointFromBase:")]
		CGPoint ConvertPointFromBase (CGPoint aPoint);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertSizeToBase:")]
		CGSize ConvertSizeToBase (CGSize aSize);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertSizeFromBase:")]
		CGSize ConvertSizeFromBase (CGSize aSize);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertRectToBase:")]
		CGRect ConvertRectToBase (CGRect aRect);

		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("convertRectFromBase:")]
		CGRect ConvertRectFromBase (CGRect aRect);

		[Export ("canDraw")]
		bool CanDraw ();

		[Export ("setNeedsDisplayInRect:")]
		void SetNeedsDisplayInRect (CGRect invalidRect);

		//[Export ("setNeedsDisplay:")]
		//void SetNeedsDisplay (bool flag);
		
		[Export ("lockFocus")]
		void LockFocus ();

		[Export ("unlockFocus")][ThreadSafe]
		void UnlockFocus ();

		[Export ("lockFocusIfCanDraw")][ThreadSafe]
		bool LockFocusIfCanDraw ();

		[Export ("lockFocusIfCanDrawInContext:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSView.DisplayRectIgnoringOpacity (CGRect, NSGraphicsContext)' to draw a view subtree into a graphics context.")]
		bool LockFocusIfCanDrawInContext (NSGraphicsContext context);

		[Export ("focusView")][Static]
		[return: NullAllowed]
		NSView FocusView ();

		[Export ("visibleRect")]
		CGRect VisibleRect ();

		[Export ("display")]
		void Display ();

		[Export ("displayIfNeeded")]
		void DisplayIfNeeded ();

		[Export ("displayIfNeededIgnoringOpacity")]
		void DisplayIfNeededIgnoringOpacity ();

		[Export ("displayRect:")]
		void DisplayRect (CGRect rect);

		[Export ("displayIfNeededInRect:")]
		void DisplayIfNeededInRect (CGRect rect);

		[Export ("displayRectIgnoringOpacity:")]
		void DisplayRectIgnoringOpacity (CGRect rect);

		[Export ("displayIfNeededInRectIgnoringOpacity:")]
		void DisplayIfNeededInRectIgnoringOpacity (CGRect rect);

		[Export ("drawRect:")]
		[ThreadSafe] // Bug 22909 - This can be called from a non-ui thread <= OS X 10.9
		void DrawRect (CGRect dirtyRect);

		[Export ("displayRectIgnoringOpacity:inContext:")]
		void DisplayRectIgnoringOpacity (CGRect aRect, NSGraphicsContext context);

		[Export ("bitmapImageRepForCachingDisplayInRect:")]
		NSBitmapImageRep BitmapImageRepForCachingDisplayInRect (CGRect rect);

		[Export ("cacheDisplayInRect:toBitmapImageRep:")]
		void CacheDisplay (CGRect rect, NSBitmapImageRep bitmapImageRep);

		[Export ("viewWillDraw")]
		void ViewWillDraw ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("gState")]
		nint GState ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("allocateGState")]
		void AllocateGState ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("releaseGState")]
		void ReleaseGState ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("setUpGState")]
		void SetUpGState ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("renewGState")]
		void RenewGState ();

		[Export ("scrollPoint:")]
		void ScrollPoint (CGPoint aPoint);

		[Export ("scrollRectToVisible:")]
		bool ScrollRectToVisible (CGRect aRect);

		[Export ("autoscroll:")]
		bool Autoscroll (NSEvent theEvent);

		[Export ("adjustScroll:")]
		CGRect AdjustScroll (CGRect newVisible);

		[Export ("scrollRect:by:")]
		void ScrollRect (CGRect aRect, CGSize delta);

		[Export ("translateRectsNeedingDisplayInRect:by:")]
		void TranslateRectsNeedingDisplay (CGRect clipRect, CGSize delta);

		[Export ("hitTest:")]
		NSView HitTest (CGPoint aPoint);

		[Export ("mouse:inRect:")]
		bool IsMouseInRect (CGPoint aPoint, CGRect aRect);

		[Export ("viewWithTag:")]
		NSObject ViewWithTag (nint aTag);

		[Export ("tag")]
		nint Tag { get; }

		[Export ("performKeyEquivalent:")]
		bool PerformKeyEquivalent (NSEvent theEvent);

		[Export ("acceptsFirstMouse:")]
		bool AcceptsFirstMouse (NSEvent theEvent);

		[Export ("shouldDelayWindowOrderingForEvent:")]
		bool ShouldDelayWindowOrderingForEvent (NSEvent theEvent);

		[Export ("needsPanelToBecomeKey")]
		bool NeedsPanelToBecomeKey { get; }

		[Export ("mouseDownCanMoveWindow")]
		bool MouseDownCanMoveWindow { get; }

		[Export ("addCursorRect:cursor:")]
		void AddCursorRect (CGRect aRect, NSCursor cursor);

		[Export ("removeCursorRect:cursor:")]
		void RemoveCursorRect (CGRect aRect, NSCursor cursor);

		[Export ("discardCursorRects")]
		void DiscardCursorRects ();

		[Export ("resetCursorRects")]
		void ResetCursorRects ();

		[Export ("addTrackingRect:owner:userData:assumeInside:")]
		nint AddTrackingRect (CGRect aRect, NSObject anObject, IntPtr data, bool assumeInside);

		[Export ("removeTrackingRect:")]
		void RemoveTrackingRect (nint tag);

		[Export ("makeBackingLayer")]
		CALayer MakeBackingLayer ();

		[Export ("addTrackingArea:")][PostSnippet ("__mt_tracking_var = TrackingAreas ();")]
		void AddTrackingArea (NSTrackingArea trackingArea);

		[Export ("removeTrackingArea:")][PostSnippet ("__mt_tracking_var = TrackingAreas ();")]
		void RemoveTrackingArea (NSTrackingArea trackingArea);

		[Export ("trackingAreas")]
		NSTrackingArea [] TrackingAreas ();

		[Export ("updateTrackingAreas")]
		void UpdateTrackingAreas ();

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("shouldDrawColor")]
		bool ShouldDrawColor { get; }

		[Export ("enclosingScrollView")]
		NSScrollView EnclosingScrollView { get; }

		[Export ("menuForEvent:")]
		NSMenu MenuForEvent (NSEvent theEvent);

		[Static]
		[Export ("defaultMenu")]
		NSMenu DefaultMenu ();

		[Export ("addToolTipRect:owner:userData:")]
#if !XAMCORE_4_0
		nint AddToolTip (CGRect aRect, NSObject anObject, IntPtr data);
#else
		[Internal]
		nint _AddToolTip (CGRect aRect, NSObject anObject, IntPtr data);
#endif

		[Export ("removeToolTip:")]
		void RemoveToolTip (nint tag);

		[Export ("removeAllToolTips")]
		void RemoveAllToolTips ();

		[Export ("viewWillStartLiveResize")]
		void ViewWillStartLiveResize ();

		[Export ("viewDidEndLiveResize")]
		void ViewDidEndLiveResize ();

		[Export ("inLiveResize")]
		bool InLiveResize { get; }

		[Export ("preservesContentDuringLiveResize")]
		bool PreservesContentDuringLiveResize { get; }

		[Export ("rectPreservedDuringLiveResize")]
		CGRect RectPreservedDuringLiveResize { get; }

		//[Export ("getRectsExposedDuringLiveResize:count:")]
		// void GetRectsExposedDuringLiveResizecount

		[Export ("inputContext")]
		NSTextInputContext InputContext { get; }

		//Detected properties
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")]get; set; }

		[Export ("subviews", ArgumentSemantic.Copy)]
		NSView [] Subviews { get; set; }

		[Export ("postsFrameChangedNotifications")]
		bool PostsFrameChangedNotifications { get; set; }

		[Export ("autoresizesSubviews")]
		bool AutoresizesSubviews { get; set; }

		[Export ("autoresizingMask")]
		NSViewResizingMask AutoresizingMask { get; set; }

		[Export ("frame")]
		CGRect Frame { get; set; }

		[Export ("frameRotation")]
		nfloat FrameRotation { get; set; }

		[Export ("frameCenterRotation")]
		nfloat FrameCenterRotation { get; set; }

		[Export ("boundsRotation")]
		nfloat BoundsRotation { get; set; }

		[Export ("bounds")]
		CGRect Bounds { get; set; }

		[Export ("canDrawConcurrently")]
		bool CanDrawConcurrently { get; set; }

		[Export ("needsDisplay")]
		bool NeedsDisplay { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 12, 2)]
		[Export ("acceptsTouchEvents")]
		bool AcceptsTouchEvents { get; set; }

		[Export ("wantsRestingTouches")]
		bool WantsRestingTouches { get; set; }

		[Export ("layerContentsRedrawPolicy")]
		NSViewLayerContentsRedrawPolicy LayerContentsRedrawPolicy { get; set; }

		[Export ("layerContentsPlacement")]
		NSViewLayerContentsPlacement LayerContentsPlacement { get; set; }

		[Export ("wantsLayer")]
		bool WantsLayer { get; set; }

		[Export ("layer", ArgumentSemantic.Retain), NullAllowed]
		CALayer Layer { get; set; }

		[Export ("alphaValue")]
		nfloat AlphaValue { get; set; }

		[Export ("backgroundFilters", ArgumentSemantic.Copy), NullAllowed]
		CIFilter [] BackgroundFilters { get; set; }

		[Export ("compositingFilter", ArgumentSemantic.Retain), NullAllowed]
		CIFilter CompositingFilter { get; set; }

		[Export ("contentFilters", ArgumentSemantic.Copy), NullAllowed]
		CIFilter [] ContentFilters { get; set; }

		[Export ("shadow", ArgumentSemantic.Copy)]
		NSShadow Shadow { get; set; }

		[Export ("postsBoundsChangedNotifications")]
		bool PostsBoundsChangedNotifications { get; set; }

		[Export ("toolTip")]
		string ToolTip { get; set; }
				
		[Export ("registerForDraggedTypes:")]
		void RegisterForDraggedTypes (string [] newTypes);

		[Export ("unregisterDraggedTypes")]
		void UnregisterDraggedTypes ();
		
		[Export ("registeredDraggedTypes")]
		string[] RegisteredDragTypes();

		[Export ("beginDraggingSessionWithItems:event:source:")]
		NSDraggingSession BeginDraggingSession (NSDraggingItem [] items, NSEvent evnt, [Protocolize] NSDraggingSource source);

		[Availability (Deprecated = Platform.Mac_10_7, Message = "Use BeginDraggingSession instead.")]
		[Export ("dragImage:at:offset:event:pasteboard:source:slideBack:")]
		void DragImage (NSImage anImage, CGPoint viewLocation, CGSize initialOffset, NSEvent theEvent, NSPasteboard pboard, NSObject sourceObj, bool slideFlag);

		[Export ("dragFile:fromRect:slideBack:event:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'BeginDraggingSession (NSDraggingItem [], NSEvent, NSDraggingSource)' instead.")]
		bool DragFile (string filename, CGRect aRect, bool slideBack, NSEvent theEvent);
		
		[Export ("dragPromisedFilesOfTypes:fromRect:source:slideBack:event:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'BeginDraggingSession (NSDraggingItem [], NSEvent, NSDraggingSource)' instead.")]
		bool DragPromisedFilesOfTypes (string[] typeArray, CGRect aRect, NSObject sourceObject, bool slideBack, NSEvent theEvent);
		
		[Export ("exitFullScreenModeWithOptions:")]
		void ExitFullscreenModeWithOptions(NSDictionary options);
		
		[Export ("enterFullScreenMode:withOptions:")]
		bool EnterFullscreenModeWithOptions(NSScreen screen, NSDictionary options);
		
		[Export ("isInFullScreenMode")]
		bool IsInFullscreenMode { get; }
		
		[Field ("NSFullScreenModeApplicationPresentationOptions")]   
		NSString NSFullScreenModeApplicationPresentationOptions { get; }
		
		// Fields
		[Field ("NSFullScreenModeAllScreens")]
		NSString NSFullScreenModeAllScreens { get; }
		
		[Field ("NSFullScreenModeSetting")]
		NSString NSFullScreenModeSetting { get; }
		
		[Field ("NSFullScreenModeWindowLevel")]
		NSString NSFullScreenModeWindowLevel { get; }

		[Notification, Field ("NSViewFrameDidChangeNotification")]
		NSString FrameChangedNotification { get; }
 
		[Notification, Field ("NSViewFocusDidChangeNotification")]
		[Deprecated (PlatformName.MacOSX, 10, 4)]
		NSString FocusChangedNotification { get; }

		[Notification, Field ("NSViewBoundsDidChangeNotification")]
		NSString BoundsChangedNotification { get; }

		[Notification, Field ("NSViewGlobalFrameDidChangeNotification")]
		NSString GlobalFrameChangedNotification { get; }

		[Notification, Field ("NSViewDidUpdateTrackingAreasNotification")]
		NSString UpdatedTrackingAreasNotification { get; }

		[Mac (10, 7), Export ("constraints")]
		NSLayoutConstraint [] Constraints { get; }
		
		[Mac (10, 7), Export ("addConstraint:")][PostGet ("Constraints")]
		void AddConstraint (NSLayoutConstraint constraint);

		[Mac (10, 7), Export ("addConstraints:")][PostGet ("Constraints")]
		void AddConstraints (NSLayoutConstraint [] constraints);

		[Mac (10, 7), Export ("removeConstraint:")][PostGet ("Constraints")]
		void RemoveConstraint (NSLayoutConstraint constraint);

		[Mac (10, 7), Export ("removeConstraints:")][PostGet ("Constraints")]
		void RemoveConstraints (NSLayoutConstraint [] constraints);

		[Mac (10, 7), Export ("layoutSubtreeIfNeeded")]
		void LayoutSubtreeIfNeeded ();

		[Mac (10, 7), Export ("layout")]
		void Layout ();

		[Mac (10, 7), Export ("needsUpdateConstraints")]
		bool NeedsUpdateConstraints { get; set; }

		[Mac (10, 7), Export ("needsLayout")]
		bool NeedsLayout { get; set; }

		[Mac (10, 7), Export ("updateConstraints")]
		[RequiresSuper]
		void UpdateConstraints ();

		[Mac (10, 7), Export ("updateConstraintsForSubtreeIfNeeded")]
		void UpdateConstraintsForSubtreeIfNeeded ();

		[Static]
		[Mac (10, 7), Export ("requiresConstraintBasedLayout")]
		bool RequiresConstraintBasedLayout ();

		//Detected properties
		[Mac (10, 7), Export ("translatesAutoresizingMaskIntoConstraints")]
		bool TranslatesAutoresizingMaskIntoConstraints { get; set; }

		[Mac (10, 7), Export ("alignmentRectForFrame:")]
		CGRect GetAlignmentRectForFrame (CGRect frame);

		[Mac (10, 7), Export ("frameForAlignmentRect:")]
		CGRect GetFrameForAlignmentRect (CGRect alignmentRect);

		[Mac (10, 7), Export ("alignmentRectInsets")]
		NSEdgeInsets AlignmentRectInsets { get; }

		[Mac (10, 7), Export ("baselineOffsetFromBottom")]
		nfloat BaselineOffsetFromBottom { get; }

		[Mac (10, 7), Export ("intrinsicContentSize")]
		CGSize IntrinsicContentSize { get; }

		[Mac (10, 7), Export ("invalidateIntrinsicContentSize")]
		void InvalidateIntrinsicContentSize ();

		[Mac (10, 7), Export ("contentHuggingPriorityForOrientation:")]
		float /* NSLayoutPriority = float */ GetContentHuggingPriorityForOrientation (NSLayoutConstraintOrientation orientation);

		[Mac (10, 7), Export ("setContentHuggingPriority:forOrientation:")]
		void SetContentHuggingPriorityForOrientation (float /* NSLayoutPriority = float */ priority, NSLayoutConstraintOrientation orientation);

		[Mac (10, 7), Export ("contentCompressionResistancePriorityForOrientation:")]
		float /* NSLayoutPriority = float */ GetContentCompressionResistancePriority (NSLayoutConstraintOrientation orientation);

		[Mac (10, 7), Export ("setContentCompressionResistancePriority:forOrientation:")]
		void SetContentCompressionResistancePriority (float /* NSLayoutPriority = float */ priority, NSLayoutConstraintOrientation orientation);

		[Mac (10, 7), Export ("fittingSize")]
		CGSize FittingSize { get; }

		[Mac (10, 7), Export ("constraintsAffectingLayoutForOrientation:")]
		NSLayoutConstraint [] GetConstraintsAffectingLayout (NSLayoutConstraintOrientation orientation);

		[Mac (10, 7), Export ("hasAmbiguousLayout")]
		bool HasAmbiguousLayout { get; }

		[Mac (10, 7), Export ("exerciseAmbiguityInLayout")]
		void ExerciseAmbiguityInLayout ();

		[Availability (Deprecated = Platform.Mac_10_8)]
		[Export ("performMnemonic:")]
		bool PerformMnemonic (string mnemonic);

		[Export ("nextKeyView")]
		NSView NextKeyView { get; set; }

		[Export ("previousKeyView")]
		NSView PreviousKeyView { get; }

		[Export ("nextValidKeyView")]
		NSView NextValidKeyView { get; }

		[Export ("previousValidKeyView")]
		NSView PreviousValidKeyView { get; }

		[Export ("canBecomeKeyView")]
		bool CanBecomeKeyView { get; }

		[Export ("setKeyboardFocusRingNeedsDisplayInRect:")]
		void SetKeyboardFocusRingNeedsDisplay (CGRect rect);

		[Export ("focusRingType")]
		NSFocusRingType FocusRingType { get; set; }

		[Static, Export ("defaultFocusRingType")]
		NSFocusRingType DefaultFocusRingType { get; }

		[Export ("drawFocusRingMask")]
		void DrawFocusRingMask ();

		[Export ("focusRingMaskBounds")]
		CGRect FocusRingMaskBounds { get; }

		[Export ("noteFocusRingMaskChanged")]
		void NoteFocusRingMaskChanged ();

		[Mac (10, 7), Export ("isDrawingFindIndicator")]
		bool IsDrawingFindIndicator { get; }
		
		[Export ("dataWithEPSInsideRect:")]
		NSData DataWithEpsInsideRect (CGRect rect);
	
		[Export ("dataWithPDFInsideRect:")]
		NSData DataWithPdfInsideRect (CGRect rect);
	
		[Export ("print:")]
		void Print (NSObject sender);
		
		[Export ("printJobTitle")]
		string PrintJobTitle { get; }

		[Export ("beginDocument")]
		void BeginDocument ();
		
		[Export ("endDocument")]
		void EndDocument ();

		[Export ("beginPageInRect:atPlacement:")]
		void BeginPage (CGRect rect, CGPoint placement);

		[Export ("endPage")]
		void EndPage ();
		
		[Export ("pageHeader")]
		NSAttributedString PageHeader { get; }

		[Export ("pageFooter")]
		NSAttributedString PageFooter { get; }

		[Export ("writeEPSInsideRect:toPasteboard:")]
		void WriteEpsInsideRect (CGRect rect, NSPasteboard pboard);

		[Export ("writePDFInsideRect:toPasteboard:")]
		void WritePdfInsideRect (CGRect rect, NSPasteboard pboard);
		
		[Export ("drawPageBorderWithSize:")]
		void DrawPageBorder (CGSize borderSize);
		
		[Export ("drawSheetBorderWithSize:")]
		void DrawSheetBorder (CGSize borderSize);
		
		[Export ("heightAdjustLimit")]
		nfloat HeightAdjustLimit { get; }
		
		[Export ("widthAdjustLimit")]
		nfloat WidthAdjustLimit { get; }
		
		[Export ("adjustPageWidthNew:left:right:limit:")]
		void AdjustPageWidthNew (ref nfloat newRight, nfloat left, nfloat proposedRight, nfloat rightLimit);
		
		[Export ("adjustPageHeightNew:top:bottom:limit:")]
		void AdjustPageHeightNew (ref nfloat newBottom, nfloat top, nfloat proposedBottom, nfloat bottomLimit);
		
		[Export ("knowsPageRange:")]
		bool KnowsPageRange (ref NSRange aRange);
		
		[Export ("rectForPage:")]
		CGRect RectForPage (nint pageNumber);
		
		[Export ("locationOfPrintRect:")]
		CGPoint LocationOfPrintRect (CGRect aRect);

		[Mac (10, 7), Export ("wantsBestResolutionOpenGLSurface")]
		bool WantsBestResolutionOpenGLSurface { get; set; }

		[Mac (10, 7), Export ("backingAlignedRect:options:")]
		CGRect BackingAlignedRect (CGRect aRect, NSAlignmentOptions options);

		[Mac (10, 7), Export ("convertRectFromBacking:")]
		CGRect ConvertRectFromBacking (CGRect aRect);

		[Mac (10, 7), Export ("convertRectToBacking:")]
		CGRect ConvertRectToBacking (CGRect aRect);

		[Mac (10, 7), Export ("convertRectFromLayer:")]
		CGRect ConvertRectFromLayer (CGRect aRect);

		[Mac (10, 7), Export ("convertRectToLayer:")]
		CGRect ConvertRectToLayer (CGRect aRect);

		[Mac (10, 7), Export ("convertPointFromBacking:")]
		CGPoint ConvertPointFromBacking (CGPoint aPoint);

		[Mac (10, 7), Export ("convertPointToBacking:")]
		CGPoint ConvertPointToBacking (CGPoint aPoint);

		[Mac (10, 7), Export ("convertPointFromLayer:")]
		CGPoint ConvertPointFromLayer (CGPoint aPoint);

		[Mac (10, 7), Export ("convertPointToLayer:")]
		CGPoint ConvertPointToLayer (CGPoint aPoint);

		[Mac (10, 7), Export ("convertSizeFromBacking:")]
		CGSize ConvertSizeFromBacking (CGSize aSize);

		[Mac (10, 7), Export ("convertSizeToBacking:")]
		CGSize ConvertSizeToBacking (CGSize aSize);

		[Mac (10, 7), Export ("convertSizeFromLayer:")]
		CGSize ConvertSizeFromLayer (CGSize aSize);

		[Mac (10, 7), Export ("convertSizeToLayer:")]
		CGSize ConvertSizeToLayer (CGSize aSize);

		[Availability (Introduced = Platform.Mac_10_7)]
		[Export ("viewDidChangeBackingProperties")]
		void DidChangeBackingProperties ();

		[Mac (10,10)]
		[Export ("allowsVibrancy")]
		bool AllowsVibrancy { get; }

		[Mac (10,10)]
		[Export ("gestureRecognizers", ArgumentSemantic.Copy)]
		NSGestureRecognizer [] GestureRecognizers { get; set; }

		[Mac (10,10)]
		[Export ("addGestureRecognizer:")][PostGet("GestureRecognizers")]
		void AddGestureRecognizer (NSGestureRecognizer gestureRecognizer);

		[Mac (10,10)]
		[Export ("removeGestureRecognizer:")][PostGet("GestureRecognizers")]
		void RemoveGestureRecognizer (NSGestureRecognizer gestureRecognizer);

		[Mac (10,7)]
		[Export ("prepareForReuse")]
		void PrepareForReuse ();

		[Mac (10,9)]
		[Static, Export ("isCompatibleWithResponsiveScrolling")]
		bool IsCompatibleWithResponsiveScrolling { get; }

		[Mac (10,9)]
		[Export ("prepareContentInRect:")]
		void PrepareContentInRect (CGRect rect);
		
		[Mac (10,9)]
		[Export ("canDrawSubviewsIntoLayer")]
		bool CanDrawSubviewsIntoLayer { get; set; }

		[Mac (10,9)]
		[Export ("layerUsesCoreImageFilters")]
		bool LayerUsesCoreImageFilters { get; set; }

		[Mac (10,9)] // NS_AVAILABLE_MAC(10,8); but the 10.8 headers do not contain this member, nor can the dontlink tests find it in 10.8.
		[Export ("userInterfaceLayoutDirection")]
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; set; }

		[Mac (10,9)]
		[Export ("preparedContentRect")]
		CGRect PreparedContentRect { get; set; }

		[Mac (10,11)]
		[Export ("pressureConfiguration", ArgumentSemantic.Strong)]
		NSPressureConfiguration PressureConfiguration { get; set; }

		[Mac (10,11)]
		[Export ("willOpenMenu:withEvent:")]
		void WillOpenMenu (NSMenu menu, NSEvent theEvent);

		[Mac (10,11)]
		[Export ("didCloseMenu:withEvent:")]
		void DidCloseMenu (NSMenu menu, [NullAllowed] NSEvent theEvent);

		// NSConstraintBasedLayoutCoreMethods

#if XAMCORE_2_0 // NSLayoutXAxisAnchor is a generic type, only supported in Unified (for now)
		[Mac (10,11)]
		[Export ("leadingAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor LeadingAnchor { get; }

		[Mac (10,11)]
		[Export ("trailingAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor TrailingAnchor { get; }

		[Mac (10,11)]
		[Export ("leftAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor LeftAnchor { get; }

		[Mac (10,11)]
		[Export ("rightAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor RightAnchor { get; }

		[Mac (10,11)]
		[Export ("topAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor TopAnchor { get; }

		[Mac (10,11)]
		[Export ("bottomAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor BottomAnchor { get; }

		[Mac (10,11)]
		[Export ("widthAnchor", ArgumentSemantic.Strong)]
		NSLayoutDimension WidthAnchor { get; }

		[Mac (10,11)]
		[Export ("heightAnchor", ArgumentSemantic.Strong)]
		NSLayoutDimension HeightAnchor { get; }

		[Mac (10,11)]
		[Export ("centerXAnchor", ArgumentSemantic.Strong)]
		NSLayoutXAxisAnchor CenterXAnchor { get; }

		[Mac (10,11)]
		[Export ("centerYAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor CenterYAnchor { get; }

		[Mac (10,11)]
		[Export ("firstBaselineAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor FirstBaselineAnchor { get; }

		[Mac (10,11)]
		[Export ("lastBaselineAnchor", ArgumentSemantic.Strong)]
		NSLayoutYAxisAnchor LastBaselineAnchor { get; }
#endif // XAMCORE_2_0

		[Mac (10,11)]
		[Export ("firstBaselineOffsetFromTop")]
		nfloat FirstBaselineOffsetFromTop { get; }

		[Mac (10,11)]
		[Export ("lastBaselineOffsetFromBottom")]
		nfloat LastBaselineOffsetFromBottom { get; }

		[Mac (10,11)]
		[Field ("NSViewNoIntrinsicMetric")]
		nfloat NoIntrinsicMetric { get; }

		[Mac (10,11)]
		[Export ("addLayoutGuide:")]
		void AddLayoutGuide (NSLayoutGuide guide);

		[Mac (10,11)]
		[Export ("removeLayoutGuide:")]
		void RemoveLayoutGuide (NSLayoutGuide guide);

		[Mac (10,11)]
		[Export ("layoutGuides", ArgumentSemantic.Copy)]
		NSLayoutGuide[] LayoutGuides { get; }
	}

	[BaseType (typeof (NSAnimation))]
	interface NSViewAnimation { 
		[Export ("initWithViewAnimations:")]
		IntPtr Constructor (NSDictionary [] viewAnimations);
	
		[Export ("viewAnimations", ArgumentSemantic.Copy)]
		NSDictionary [] ViewAnimations { get; set; }
	
		[Export ("animator")]
		NSObject Animator { [return: Proxy] get; }
	
		[Export ("animations")]
		NSDictionary Animations  { get; set; }
	
		[Export ("animationForKey:")]
		NSObject AnimationForKey (string  key);
	
		[Static]
		[Export ("defaultAnimationForKey:")]
		NSObject DefaultAnimationForKey (string  key);
	
		[Field ("NSViewAnimationTargetKey")]
		NSString TargetKey { get; }
		
		[Field ("NSViewAnimationStartFrameKey")]
		NSString StartFrameKey { get; }
		
		[Field ("NSViewAnimationEndFrameKey")]
		NSString EndFrameKey { get; }
		
		[Field ("NSViewAnimationEffectKey")]
		NSString EffectKey { get; }
		
		[Field ("NSViewAnimationFadeInEffect")]
		NSString FadeInEffect { get; }
		
		[Field ("NSViewAnimationFadeOutEffect")]
		NSString FadeOutEffect { get; }
	}

	[Category]
	[BaseType (typeof(NSView))]
	interface NSView_NSTouchBar
	{
		[Mac (10, 12, 2)]
		[Export ("allowedTouchTypes")]
		NSTouchTypeMask GetAllowedTouchTypes ();

		[Export ("setAllowedTouchTypes:")]
		void SetAllowedTouchTypes (NSTouchTypeMask touchTypes);
	}

	[BaseType (typeof (NSResponder))]
	interface NSViewController : NSUserInterfaceItemIdentification, NSCoding, NSSeguePerforming
#if XAMCORE_2_0
	, NSExtensionRequestHandling 
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("loadView")]
		void LoadView ();

		[Export ("nibName", ArgumentSemantic.Copy)]
		string NibName { get; }

		[Export ("nibBundle", ArgumentSemantic.Strong)]
		NSBundle NibBundle { get; }

		[Export ("commitEditingWithDelegate:didCommitSelector:contextInfo:")]
		void CommitEditing (NSObject delegateObject, Selector didCommitSelector, IntPtr contextInfo);

		[Export ("commitEditing")]
		bool CommitEditing ();

		[Export ("discardEditing")]
		void DiscardEditing ();

		//Detected properties
		[Export ("representedObject", ArgumentSemantic.Strong)]
		NSObject RepresentedObject { get; set; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("view", ArgumentSemantic.Strong)]
		NSView View { get; set; }

		[Mac (10,10)]
		[Export ("viewLoaded")]
		bool ViewLoaded { [Bind ("isViewLoaded")] get; }

		[Mac (10,10)]
		[Export ("preferredContentSize")]
		CGSize PreferredContentSize { get; set; }

		[Mac (10,10)]
		[Export ("viewDidLoad")]
		void ViewDidLoad ();

		[Mac (10,10)]
		[Export ("viewWillAppear")]
		void ViewWillAppear ();

		[Mac (10,10)]
		[Export ("viewDidAppear")]
		void ViewDidAppear ();

		[Mac (10,10)]
		[Export ("viewWillDisappear")]
		void ViewWillDisappear ();

		[Mac (10,10)]
		[Export ("viewDidDisappear")]
		void ViewDidDisappear ();

		[Mac (10,10)]
		[Export ("updateViewConstraints")]
		void UpdateViewConstraints ();

		[Mac (10,10)]
		[Export ("viewWillLayout")]
		void ViewWillLayout ();

		[Mac (10,10)]
		[Export ("viewDidLayout")]
		void ViewDidLayout ();

		[Mac (10,10)]
		[Export ("presentedViewControllers", ArgumentSemantic.Assign)]
		NSViewController [] PresentedViewControllers { get; }

		[Mac (10,10)]
		[Export ("presentingViewController", ArgumentSemantic.UnsafeUnretained)]
		NSViewController PresentingViewController { get; }

		[Mac (10,10)]
		[Export ("presentViewController:animator:")]
		void PresentViewController (NSViewController viewController, [Protocolize] NSViewControllerPresentationAnimator animator);

		[Mac (10,10)]
		[Export ("dismissViewController:")]
		void DismissViewController (NSViewController viewController);

		[Mac (10,10)]
		[Export ("dismissController:")]
		void DismissController (NSObject sender);

		[Mac (10,10)]
		[Export ("presentViewControllerAsSheet:")]
		void PresentViewControllerAsSheet (NSViewController viewController);

		[Mac (10,10)]
		[Export ("presentViewControllerAsModalWindow:")]
		void PresentViewControllerAsModalWindow (NSViewController viewController);

		[Mac (10,10)]
		[Export ("presentViewController:asPopoverRelativeToRect:ofView:preferredEdge:behavior:")]
		void PresentViewController (NSViewController viewController, CGRect positioningRect, NSView positioningView, nuint preferredEdge, NSPopoverBehavior behavior);

		[Mac (10,10)]
		[Export ("transitionFromViewController:toViewController:options:completionHandler:")]
		void TransitionFromViewController (NSViewController fromViewController, NSViewController toViewController, NSViewControllerTransitionOptions options, Action completion);

		[Mac (10,10)]
		[Export ("parentViewController")]
		NSViewController ParentViewController { get; }

		[Mac (10,10)]
		[Export ("childViewControllers", ArgumentSemantic.Copy)]
		NSViewController [] ChildViewControllers { get; set; }

		[Mac (10,10)]
		[Export ("addChildViewController:")][PostGet("ChildViewControllers")]
		void AddChildViewController (NSViewController childViewController);

		[Mac (10,10)]
		[Export ("removeFromParentViewController")]
		void RemoveFromParentViewController ();

		[Mac (10,10)]
		[Export ("insertChildViewController:atIndex:")][PostGet("ChildViewControllers")]
		void InsertChildViewController (NSViewController childViewController, nint index);

		[Mac (10,10)]
		[Export ("removeChildViewControllerAtIndex:")][PostGet("ChildViewControllers")]
		void RemoveChildViewController (nint index);

		[Mac (10,10)]
		[Export ("preferredContentSizeDidChangeForViewController:")]
		void PreferredContentSizeDidChange (NSViewController viewController);

		[Mac (10,10)]
		[Export ("viewWillTransitionToSize:")]
		void ViewWillTransition (CGSize newSize);

		[Mac (10,10)]
		[Export ("storyboard", ArgumentSemantic.Strong)]
		NSStoryboard Storyboard { get; }

#if XAMCORE_2_0
		[Mac (10,10, onlyOn64 : true)]
		[Export ("presentViewControllerInWidget:")]
		void PresentViewControllerInWidget (NSViewController viewController);

		[Mac (10, 10, onlyOn64: true)]
		[NullAllowed, Export ("extensionContext", ArgumentSemantic.Retain)]
		NSExtensionContext ExtensionContext { get; }

		[Mac (10, 10, onlyOn64: true)]
		[NullAllowed, Export ("sourceItemView", ArgumentSemantic.Strong)]
		NSView SourceItemView { get; set; }

		[Mac (10, 10, onlyOn64: true)]
		[Export ("preferredScreenOrigin", ArgumentSemantic.Assign)]
		CGPoint PreferredScreenOrigin { get; set; }

		[Mac (10, 10, onlyOn64: true)]
		[Export ("preferredMinimumSize")]
		CGSize PreferredMinimumSize { get; }

		[Mac (10, 10, onlyOn64: true)]
		[Export ("preferredMaximumSize")]
		CGSize PreferredMaximumSize { get; }
#endif
	}

	[Mac (10,10)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSViewControllerPresentationAnimator {
		[Export ("animatePresentationOfViewController:fromViewController:")]
		[Abstract]
		void AnimatePresentation (NSViewController viewController, NSViewController fromViewController);

		[Export ("animateDismissalOfViewController:fromViewController:")]
		[Abstract]
		void AnimateDismissal (NSViewController viewController, NSViewController fromViewController);
	}

	interface INSViewControllerPresentationAnimator {}

	[Mac (10, 8)]
	[BaseType (typeof (NSViewController),
		Delegates = new [] { "WeakDelegate" },
		Events = new [] { typeof (NSPageControllerDelegate) })]
	partial interface NSPageController : NSAnimatablePropertyContainer {

		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		NSPageControllerDelegate Delegate { get; set; }

		[Export ("arrangedObjects", ArgumentSemantic.Copy)]
		NSObject [] ArrangedObjects { get; set; }

		[Export ("selectedViewController", ArgumentSemantic.Strong)]
		NSViewController SelectedViewController { get; }

		[Export ("transitionStyle")]
		NSPageControllerTransitionStyle TransitionStyle { get; set; }

		[Export ("completeTransition")]
		void CompleteTransition ();

		[Export ("selectedIndex")]
		nint SelectedIndex { get; set; }

		[Export ("takeSelectedIndexFrom:")]
		void NavigateTo (NSObject sender);

		[Export ("navigateForwardToObject:")]
		void NavigateForwardTo (NSObject target);

		[Export ("navigateBack:")]
		void NavigateBack (NSObject sender);

		[Export ("navigateForward:")]
		void NavigateForward (NSObject sender);
	}

	[BaseType (typeof (NSObject)), Model, Protocol]
	partial interface NSPageControllerDelegate {

		[Export ("pageController:identifierForObject:"), DelegateName ("NSPageControllerGetIdentifier"), DefaultValue ("String.Empty")]
		string GetIdentifier (NSPageController pageController, NSObject targetObject);

		[Export ("pageController:viewControllerForIdentifier:"), DelegateName ("NSPageControllerGetViewController"), DefaultValue (null)]
		NSViewController GetViewController (NSPageController pageController, string identifier);

		[Export ("pageController:frameForObject:"), DelegateName ("NSPageControllerGetFrame"), NoDefaultValue]
		CGRect GetFrame (NSPageController pageController, NSObject targetObject);

		[Export ("pageController:prepareViewController:withObject:"), EventArgs ("NSPageControllerPrepareViewController")]
		void PrepareViewController (NSPageController pageController, NSViewController viewController, NSObject targetObject);

		[Export ("pageController:didTransitionToObject:"), EventArgs ("NSPageControllerTransition")]
		void DidTransition (NSPageController pageController, NSObject targetObject);

		[Export ("pageControllerWillStartLiveTransition:")]
		void WillStartLiveTransition (NSPageController pageController);

		[Export ("pageControllerDidEndLiveTransition:")]
		void DidEndLiveTransition (NSPageController pageController);
	}

	[BaseType (typeof (NSImageRep), Name="NSPDFImageRep")]
	[DisableDefaultCtor] // -[NSPDFImageRep init]: unrecognized selector sent to instance 0x2652460
	interface NSPdfImageRep {
		[Export ("initWithData:")]
		IntPtr Constructor (NSData pdfData);

		[Export ("PDFRepresentation", ArgumentSemantic.Retain)]
		NSData PdfRepresentation { get; }

		[Export ("bounds")]
		CGRect Bounds { get; }

		[Export ("currentPage")]
		nint CurrentPage { get; set; }

		[Export ("pageCount")]
		nint PageCount { get; }
	}

	[BaseType (typeof (NSObject))]
	partial interface NSTableColumn : NSUserInterfaceItemIdentification, NSCoding {
		[Mac (10, 7), Export ("initWithIdentifier:")]
		[Sealed]
		IntPtr Constructor (string identifier);

		[DesignatedInitializer]
		[Mac (10, 7), Export ("initWithIdentifier:")]
		IntPtr Constructor (NSString identifier);

#if !XAMCORE_2_0
		[Obsolete, Export ("initWithIdentifier:")]
		[Sealed]
		IntPtr Constructor (NSObject identifier);
#endif
	
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dataCellForRow:")]
		NSCell DataCellForRow (nint row);
		
		[Export ("sizeToFit")]
		void SizeToFit ();
		
		[Export ("tableView")]
		NSTableView TableView { get; set; }
		
		[Export ("width")]
		nfloat Width { get; set; }
		
		[Export ("minWidth")]
		nfloat MinWidth { get; set; }
		
		[Export ("maxWidth")]
		nfloat MaxWidth { get; set; }
	
		[Export ("headerCell", ArgumentSemantic.Retain)]
		NSCell HeaderCell { get; set; }

		[Export ("dataCell")]
		NSCell DataCell { get; set; }
	
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")]get; set; }
	
		[Export ("sortDescriptorPrototype", ArgumentSemantic.Copy), NullAllowed]
		NSSortDescriptor SortDescriptorPrototype { get; set; }
	
		[Export ("resizingMask")]
		NSTableColumnResizing ResizingMask { get; set; }
	
		[Export ("headerToolTip")]
		string HeaderToolTip { get; set; }
	
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")]get; set; }

		[Mac (10,10)]
		[Export ("title")]
		string Title { get; set; }
	}

	[Mac (10, 7)]
	[BaseType (typeof (NSView))]
	interface NSTableRowView : NSAccessibilityRow {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("selectionHighlightStyle")]
		NSTableViewSelectionHighlightStyle SelectionHighlightStyle { get; set;  }

		[Export ("emphasized")]
		bool Emphasized { [Bind ("isEmphasized")] get; set;  }

		[Export ("groupRowStyle")]
		bool GroupRowStyle { [Bind ("isGroupRowStyle")] get; set;  }

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set;  }

		[Export ("floating")]
		bool Floating { [Bind ("isFloating")] get; set;  }

		[Export ("draggingDestinationFeedbackStyle")]
		NSTableViewDraggingDestinationFeedbackStyle DraggingDestinationFeedbackStyle { get; set;  }

		[Export ("indentationForDropOperation")]
		nfloat IndentationForDropOperation { get; set;  }

		[Export ("interiorBackgroundStyle")]
		NSBackgroundStyle InteriorBackgroundStyle { get;  }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set;  }

		[Export ("numberOfColumns")]
		nint NumberOfColumns { get;  }

		[Export ("targetForDropOperation")]
		bool TargetForDropOperation { [Bind ("isTargetForDropOperation")] get; set; }

		[Export ("drawBackgroundInRect:")]
		void DrawBackground (CGRect dirtyRect);

		[Export ("drawSelectionInRect:")]
		void DrawSelection (CGRect dirtyRect);

		[Export ("drawSeparatorInRect:")]
		void DrawSeparator (CGRect dirtyRect);

		[Export ("drawDraggingDestinationFeedbackInRect:")]
		void DrawDraggingDestinationFeedback (CGRect dirtyRect);

		[Export ("viewAtColumn:")]
		NSView ViewAtColumn (nint column);

		[Mac (10,10)]
		[Export ("previousRowSelected")]
		bool PreviousRowSelected { [Bind ("isPreviousRowSelected")] get; set; }

		[Mac (10,10)]
		[Export ("nextRowSelected")]
		bool NextRowSelected { [Bind ("isNextRowSelected")] get; set; }
	}

	[Mac (10, 7)]
	[BaseType (typeof (NSView))]
	partial interface NSTableCellView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("backgroundStyle")]
		NSBackgroundStyle BackgroundStyle {
			get; set;
		}

		[Export ("imageView", ArgumentSemantic.Assign)]
		NSImageView ImageView {
			get; set;
		}

		[Export ("objectValue", ArgumentSemantic.Retain), NullAllowed]
		NSObject ObjectValue {
			get; set;
		}

		[Export ("rowSizeStyle")]
		NSTableViewRowSizeStyle RowSizeStyle {
			get; set;
		}

		[Export ("textField", ArgumentSemantic.Assign)]
		NSTextField TextField {
			get; set;
		}

		[Export ("draggingImageComponents", ArgumentSemantic.Retain)]
		NSArray DraggingImageComponents {
			get;
		}
	}

	delegate void NSTableViewRowHandler (NSTableRowView rowView, nint row);
	
	[BaseType (typeof (NSControl), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTableViewDelegate)})]
	partial interface NSTableView : NSDraggingSource, NSAccessibilityTable {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("noteHeightOfRowsWithIndexesChanged:")]
		void NoteHeightOfRowsWithIndexesChanged (NSIndexSet indexSet );
	
		[Export ("tableColumns")]
		NSTableColumn[] TableColumns ();
	
		[Export ("numberOfColumns")]
		nint ColumnCount { get; }
	
		[Export ("numberOfRows")]
		nint RowCount { get; }
	
		[Export ("addTableColumn:")]
		void AddColumn (NSTableColumn tableColumn);
	
		[Export ("removeTableColumn:")]
		void RemoveColumn (NSTableColumn tableColumn);
	
		[Export ("moveColumn:toColumn:")]
		void MoveColumn (nint oldIndex, nint newIndex);
	
		[Export ("columnWithIdentifier:")]
		nint FindColumn (NSString identifier);
	
		[Export ("tableColumnWithIdentifier:")]
		NSTableColumn FindTableColumn (NSString identifier);
	
		[Export ("tile")]
		void Tile ();
	
		[Export ("sizeToFit")]
		void SizeToFit ();
	
		[Export ("sizeLastColumnToFit")]
		void SizeLastColumnToFit ();
	
		[Export ("scrollRowToVisible:")]
		void ScrollRowToVisible (nint row);
	
		[Export ("scrollColumnToVisible:")]
		void ScrollColumnToVisible (nint column);
	
		[Export ("reloadData")]
		void ReloadData ();
	
		[Export ("noteNumberOfRowsChanged")]
		void NoteNumberOfRowsChanged ();
	
		[Export ("reloadDataForRowIndexes:columnIndexes:")]
		void ReloadData (NSIndexSet rowIndexes, NSIndexSet columnIndexes );
	
		[Export ("editedColumn")]
		nint EditedColumn { get; }
	
		[Export ("editedRow")]
		nint EditedRow { get; }
	
		[Export ("clickedColumn")]
		nint ClickedColumn { get; }
	
		[Export ("clickedRow")]
		nint ClickedRow { get; }
	
		[Export ("setIndicatorImage:inTableColumn:")]
		void SetIndicatorImage ([NullAllowed] NSImage anImage, NSTableColumn tableColumn);
	
		[Export ("indicatorImageInTableColumn:")]
		NSImage GetIndicatorImage (NSTableColumn tableColumn);
	
		[Export ("canDragRowsWithIndexes:atPoint:")]
		bool CanDragRows (NSIndexSet rowIndexes, CGPoint mouseDownPoint );
	
		[Export ("dragImageForRowsWithIndexes:tableColumns:event:offset:")]
		NSImage DragImageForRowsWithIndexestableColumnseventoffset (NSIndexSet dragRows, NSTableColumn [] tableColumns, NSEvent dragEvent, ref CGPoint dragImageOffset);
	
		[Export ("setDraggingSourceOperationMask:forLocal:")]
		void SetDraggingSourceOperationMask (NSDragOperation mask, bool isLocal);
	
		[Export ("setDropRow:dropOperation:")]
		void SetDropRowDropOperation (nint row, NSTableViewDropOperation dropOperation);
	
		[Export ("selectAll:")]
		void SelectAll ([NullAllowed] NSObject sender);
	
		[Export ("deselectAll:")]
		void DeselectAll ([NullAllowed] NSObject sender);
	
		[Export ("selectColumnIndexes:byExtendingSelection:")]
		void SelectColumns (NSIndexSet indexes, bool byExtendingSelection);
	
		[Export ("selectRowIndexes:byExtendingSelection:")]
		void SelectRows (NSIndexSet indexes, bool byExtendingSelection);
	
		[Export ("selectedColumnIndexes")]
		NSIndexSet SelectedColumns { get; }
	
		[Export ("selectedRowIndexes")]
		NSIndexSet SelectedRows { get; }
	
		[Export ("deselectColumn:")]
		void DeselectColumn (nint column);
	
		[Export ("deselectRow:")]
		void DeselectRow (nint row);
	
		[Export ("selectedColumn")]
		nint SelectedColumn { get; }
	
		[Export ("selectedRow")]
		nint SelectedRow { get; }
	
		[Export ("isColumnSelected:")]
		bool IsColumnSelected (nint column);
	
		[Export ("isRowSelected:")]
		bool IsRowSelected (nint row);
	
		[Export ("numberOfSelectedColumns")]
		nint SelectedColumnsCount { get; }
	
		[Export ("numberOfSelectedRows")]
		nint SelectedRowCount { get; }
	
		[Export ("rectOfColumn:")]
		CGRect RectForColumn (nint column);
	
		[Export ("rectOfRow:")]
		CGRect RectForRow (nint row);
	
		[Export ("columnIndexesInRect:")]
		NSIndexSet GetColumnIndexesInRect (CGRect rect);
	
		[Export ("rowsInRect:")]
		NSRange RowsInRect (CGRect rect);
	
		[Export ("columnAtPoint:")]
		nint GetColumn (CGPoint point);
	
		[Export ("rowAtPoint:")]
		nint GetRow (CGPoint point);
	
		[Export ("frameOfCellAtColumn:row:")]
		CGRect GetCellFrame (nint column, nint row);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use View Based TableView and GetView.")]
		[Export ("preparedCellAtColumn:row:")]
		NSCell GetCell (nint column, nint row );
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView with an NSTextField.")]
		[Export ("textShouldBeginEditing:")]
		bool TextShouldBeginEditing (NSText textObject);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView with an NSTextField.")]
		[Export ("textShouldEndEditing:")]
		bool TextShouldEndEditing (NSText textObject);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView with an NSTextField.")]
		[Export ("textDidBeginEditing:")]
		void TextDidBeginEditing (NSNotification notification);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView with an NSTextField.")]
		[Export ("textDidEndEditing:")]
		void TextDidEndEditing (NSNotification notification);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView with an NSTextField.")]
		[Export ("textDidChange:")]
		void TextDidChange (NSNotification notification);
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView; observe the window’s firstResponder for focus change notifications.")]
		[Export ("shouldFocusCell:atColumn:row:")]
		bool ShouldFocusCell (NSCell cell, nint column, nint row );
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView; directly interact with a particular view as required and call PerformClick on it, if necessary.")]
		[Export ("performClickOnCellAtColumn:row:")]
		void PerformClick (nint column, nint row );
	
		[Export ("editColumn:row:withEvent:select:")]
		void EditColumn (nint column, nint row, [NullAllowed] NSEvent theEvent, bool select);
	
		[Export ("drawRow:clipRect:")]
		void DrawRow (nint row, CGRect clipRect);
	
		[Export ("highlightSelectionInClipRect:")]
		void HighlightSelection (CGRect clipRect);
	
		[Export ("drawGridInClipRect:")]
		void DrawGrid (CGRect clipRect);
	
		[Export ("drawBackgroundInClipRect:")]
		void DrawBackground (CGRect clipRect );
	
		//Detected properties
		[Export ("dataSource", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDataSource { get; set; }

		[Wrap ("WeakDataSource")][NullAllowed]
		[Protocolize]
		NSTableViewDataSource DataSource { get; set; }
	
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
	
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSTableViewDelegate Delegate { get; set; }
	
		[Export ("headerView", ArgumentSemantic.Retain), NullAllowed]
		NSTableHeaderView HeaderView { get; set; }
	
		[Export ("cornerView", ArgumentSemantic.Retain)]
		NSView CornerView { get; set; }
	
		[Export ("allowsColumnReordering")]
		bool AllowsColumnReordering { get; set; }
	
		[Export ("allowsColumnResizing")]
		bool AllowsColumnResizing { get; set; }
	
		[Export ("columnAutoresizingStyle")]
		NSTableViewColumnAutoresizingStyle ColumnAutoresizingStyle { get; set; }
	
		[Export ("gridStyleMask")]
		NSTableViewGridStyle GridStyleMask { get; set; }
	
		[Export ("intercellSpacing")]
		CGSize IntercellSpacing { get; set; }
	
		[Export ("usesAlternatingRowBackgroundColors")]
		bool UsesAlternatingRowBackgroundColors { get; set; }
	
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	
		[Export ("gridColor", ArgumentSemantic.Copy)]
		NSColor GridColor { get; set; }
	
		[Export ("rowHeight")]
		nfloat RowHeight { get; set; }
	
		[Export ("doubleAction")]
		Selector DoubleAction { get; set; }
	
		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor[] SortDescriptors { get; set; }
	
		[Export ("highlightedTableColumn")]
		NSTableColumn HighlightedTableColumn { get; set; }
	
		[Export ("verticalMotionCanBeginDrag")]
		bool VerticalMotionCanBeginDrag { get; set; }
	
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }
	
		[Export ("allowsEmptySelection")]
		bool AllowsEmptySelection { get; set; }
	
		[Export ("allowsColumnSelection")]
		bool AllowsColumnSelection { get; set; }
	
		[Export ("allowsTypeSelect")]
		bool AllowsTypeSelect { get; set; }
	
		[Export ("selectionHighlightStyle")]
		NSTableViewSelectionHighlightStyle SelectionHighlightStyle { get; set; }
	
		[Export ("draggingDestinationFeedbackStyle")]
		NSTableViewDraggingDestinationFeedbackStyle DraggingDestinationFeedbackStyle { get; set; }
	
		[Export ("autosaveName")]
		string AutosaveName { get; set; }
	
		[Export ("autosaveTableColumns")]
		bool AutosaveTableColumns { get; set; }
	
		[Availability (Deprecated = Platform.Mac_10_10, Message = "Use a View Based TableView; observe the window’s firstResponder.")]
		[Export ("focusedColumn")]
		nint FocusedColumn { get; set; }

		[Mac (10, 7)]
		[Export ("effectiveRowSizeStyle")]
		NSTableViewRowSizeStyle EffectiveRowSizeStyle { get; }

		[Mac (10, 7)]
		[Export ("viewAtColumn:row:makeIfNecessary:")]
		NSView GetView (nint column, nint row, bool makeIfNecessary);

		[Mac (10, 7)]
		[Export ("rowViewAtRow:makeIfNecessary:")]
		NSTableRowView GetRowView (nint row, bool makeIfNecessary);

		[Mac (10, 7)]
		[Export ("rowForView:")]
		nint RowForView (NSView view);

		[Mac (10, 7)]
		[Export ("columnForView:")]
		nint ColumnForView (NSView view);

		// According to the header identifier should be non-null but example in 
		// https://bugzilla.xamarin.com/show_bug.cgi?id=36496 shows actual behavior differs
		[Mac (10, 7)]
		[Export ("makeViewWithIdentifier:owner:")]
		NSView MakeView ([NullAllowed]string identifier, [NullAllowed]NSObject owner);

		[Mac (10, 7)]
		[Export ("enumerateAvailableRowViewsUsingBlock:")]
		void EnumerateAvailableRowViews (NSTableViewRowHandler callback);

		[Mac (10, 7)]
		[Export ("beginUpdates")]
		void BeginUpdates ();

		[Mac (10, 7)]
		[Export ("endUpdates")]
		void EndUpdates ();

		[Mac (10, 7)]
		[Export ("insertRowsAtIndexes:withAnimation:")]
		void InsertRows (NSIndexSet indexes, NSTableViewAnimation animationOptions);

		[Mac (10, 7)]
		[Export ("removeRowsAtIndexes:withAnimation:")]
		void RemoveRows (NSIndexSet indexes, NSTableViewAnimation animationOptions);

		[Mac (10, 7)]
		[Export ("moveRowAtIndex:toIndex:")]
		void MoveRow (nint oldIndex, nint newIndex);

		[Mac (10, 7)]
		[Export ("rowSizeStyle")]
		NSTableViewRowSizeStyle RowSizeStyle { get; set; }

		[Mac (10, 7)]
		[Export ("floatsGroupRows")]
		bool FloatsGroupRows { get; set; }

		[Field ("NSTableViewRowViewKey")]
		NSString RowViewKey { get; }

		[Mac (10,8)]
		[Export ("registerNib:forIdentifier:")]
		void RegisterNib ([NullAllowed] NSNib nib, string identifier);

		[Mac (10,7)]
		[Export ("didAddRowView:forRow:")]
		void RowViewAdded (NSTableRowView rowView, nint row);

		[Mac (10,7)]
		[Export ("didRemoveRowView:forRow:")]
		void RowViewRemoved (NSTableRowView rowView, nint row);

		[Mac (10,8)]
		[Export ("registeredNibsByIdentifier", ArgumentSemantic.Copy)]
		NSDictionary RegisteredNibsByIdentifier { get; }

		[Mac (10,10)]
		[Export ("usesStaticContents")]
		bool UsesStaticContents { get; set; }		

		[Mac (10,11)]
		[Export ("hideRowsAtIndexes:withAnimation:")]
		void HideRows (NSIndexSet indexes, NSTableViewAnimation rowAnimation);

		[Mac (10,11)]
		[Export ("unhideRowsAtIndexes:withAnimation:")]
		void UnhideRows (NSIndexSet indexes, NSTableViewAnimation rowAnimation);

		[Mac (10,11)]
		[Export ("hiddenRowIndexes", ArgumentSemantic.Copy)]
		NSIndexSet HiddenRowIndexes { get; }

		[Mac (10,11)]
		[Export ("rowActionsVisible")]
		bool RowActionsVisible { get; set; }

		[Mac (10,12)]
		[Export ("userInterfaceLayoutDirection")]
		NSUserInterfaceLayoutDirection UserInterfaceLayoutDirection { get; set; }

		[Mac (10, 13)]
		[Export ("usesAutomaticRowHeights")]
		bool UsesAutomaticRowHeights { get; set; }
	} 
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSTableViewDelegate {
		[Export ("tableView:willDisplayCell:forTableColumn:row:"), EventArgs ("NSTableViewCell")]
		void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:shouldEditTableColumn:row:"), DelegateName ("NSTableViewColumnRowPredicate"), DefaultValue (false)]
		bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, nint row);
	
		[Export ("selectionShouldChangeInTableView:"), DelegateName ("NSTableViewPredicate"), DefaultValue (true)]
		bool SelectionShouldChange (NSTableView tableView);
	
		[Export ("tableView:shouldSelectRow:"), DelegateName ("NSTableViewRowPredicate")] [DefaultValue (true)]
		bool ShouldSelectRow (NSTableView tableView, nint row);
	
		[Export ("tableView:selectionIndexesForProposedSelection:"), DelegateName ("NSTableViewIndexFilter"), DefaultValueFromArgument ("proposedSelectionIndexes")]
		NSIndexSet GetSelectionIndexes (NSTableView tableView, NSIndexSet proposedSelectionIndexes);
	
		[Export ("tableView:shouldSelectTableColumn:"), DelegateName ("NSTableViewColumnPredicate"), DefaultValue (true)]
		bool ShouldSelectTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:mouseDownInHeaderOfTableColumn:"), EventArgs ("NSTableViewTable")]
		void MouseDownInHeaderOfTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:didClickTableColumn:"), EventArgs ("NSTableViewTable")]
		void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:didDragTableColumn:"), EventArgs ("NSTableViewTable")]
		void DidDragTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:heightOfRow:"), DelegateName ("NSTableViewRowHeight"), NoDefaultValue]
		nfloat GetRowHeight (NSTableView tableView, nint row );
	
		[Export ("tableView:typeSelectStringForTableColumn:row:"), DelegateName ("NSTableViewColumnRowString"), DefaultValue ("String.Empty")]
		string GetSelectString (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:nextTypeSelectMatchFromRow:toRow:forString:"), DelegateName ("NSTableViewSearchString"), DefaultValue (-1)]
		nint GetNextTypeSelectMatch (NSTableView tableView, nint startRow, nint endRow, string searchString);
	
		[Export ("tableView:shouldTypeSelectForEvent:withCurrentSearchString:"), DelegateName ("NSTableViewEventString"), DefaultValue (false)]
		bool ShouldTypeSelect (NSTableView tableView, NSEvent theEvent, string searchString );
	
		[Export ("tableView:shouldShowCellExpansionForTableColumn:row:"), DelegateName ("NSTableViewColumnRowPredicate"), DefaultValue (false)]
		bool ShouldShowCellExpansion (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:shouldTrackCell:forTableColumn:row:"), DelegateName ("NSTableViewCell"), DefaultValue (false)]
		bool ShouldTrackCell (NSTableView tableView, NSCell cell, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:dataCellForTableColumn:row:"), DelegateName ("NSTableViewCellGetter"), NoDefaultValue]
		NSCell GetDataCell (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:isGroupRow:"), DelegateName ("NSTableViewRowPredicate"), DefaultValue (false)]
		bool IsGroupRow (NSTableView tableView, nint row );
	
		[Export ("tableView:sizeToFitWidthOfColumn:"), DelegateName ("NSTableViewColumnWidth"), DefaultValue (80)]
		nfloat GetSizeToFitColumnWidth (NSTableView tableView, nint column );
	
		[Export ("tableView:shouldReorderColumn:toColumn:"), DelegateName ("NSTableReorder"), DefaultValue (false)]
		bool ShouldReorder (NSTableView tableView, nint columnIndex, nint newColumnIndex );
	
		[Export ("tableViewSelectionDidChange:"), EventArgs ("NSNotification")]
		void SelectionDidChange (NSNotification notification);
	
		[Export ("tableViewColumnDidMove:"), EventArgs ("NSNotification")]
		void ColumnDidMove (NSNotification notification);
	
		[Export ("tableViewColumnDidResize:"), EventArgs ("NSNotification")]
		void ColumnDidResize (NSNotification notification);
	
		[Export ("tableViewSelectionIsChanging:"), EventArgs ("NSNotification")]
		void SelectionIsChanging (NSNotification notification);

		[Mac (10, 7)]
                [Export ("tableView:viewForTableColumn:row:"), DelegateName ("NSTableViewViewGetter"), NoDefaultValue]
                NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row);

		[Mac (10, 7)]
                [Export ("tableView:rowViewForRow:"), DelegateName ("NSTableViewRowGetter"), DefaultValue (null)]
                NSTableRowView CoreGetRowView (NSTableView tableView, nint row);

		[Mac (10, 7)]
                [Export ("tableView:didAddRowView:forRow:"), EventArgs ("NSTableViewRow")]
                void DidAddRowView (NSTableView tableView, NSTableRowView rowView, nint row);

		[Mac (10, 7)]
                [Export ("tableView:didRemoveRowView:forRow:"), EventArgs ("NSTableViewRow")]
                void DidRemoveRowView (NSTableView tableView, NSTableRowView rowView, nint row);

		[Mac (10,11)]
		[Export ("tableView:rowActionsForRow:edge:"), DelegateName ("NSTableViewRowActionsGetter"), NoDefaultValue]
//		[Verify (StronglyTypedNSArray)]
		NSTableViewRowAction[] RowActions (NSTableView tableView, nint row, NSTableRowActionEdge edge);
	}
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSTableViewDataSource {
		[Export ("numberOfRowsInTableView:")]
		nint GetRowCount (NSTableView tableView);
	
		[Export ("tableView:objectValueForTableColumn:row:")]
		NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:setObjectValue:forTableColumn:row:")]
		void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:sortDescriptorsDidChange:")]
		void SortDescriptorsChanged (NSTableView tableView, NSSortDescriptor [] oldDescriptors);
	
		[Export ("tableView:writeRowsWithIndexes:toPasteboard:")]
		bool WriteRows (NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard );
	
		[Export ("tableView:validateDrop:proposedRow:proposedDropOperation:")]
		NSDragOperation ValidateDrop (NSTableView tableView, [Protocolize (4)] NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation);
	
		[Export ("tableView:acceptDrop:row:dropOperation:")]
		bool AcceptDrop (NSTableView tableView, [Protocolize (4)] NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation);
	
		[Export ("tableView:namesOfPromisedFilesDroppedAtDestination:forDraggedRowsWithIndexes:")]
		string [] FilesDropped (NSTableView tableView, NSUrl dropDestination, NSIndexSet indexSet );

		[Mac (10, 7)]
		[Export ("tableView:pasteboardWriterForRow:")]
#if XAMCORE_2_0
		INSPasteboardWriting GetPasteboardWriterForRow (NSTableView tableView, nint row);
#else
		NSPasteboardWriting GetPasteboardWriterForRow (NSTableView tableView, nint row);
#endif

		[Mac (10, 7)]
		[Export ("tableView:draggingSession:willBeginAtPoint:forRowIndexes:")]
		void DraggingSessionWillBegin (NSTableView tableView, NSDraggingSession draggingSession, CGPoint willBeginAtScreenPoint, NSIndexSet rowIndexes);

		[Mac (10, 7)]
		[Export ("tableView:draggingSession:endedAtPoint:operation:")]
		void DraggingSessionEnded (NSTableView tableView, NSDraggingSession draggingSession, CGPoint endedAtScreenPoint, NSDragOperation operation);

		[Mac (10, 7)]
		[Export ("tableView:updateDraggingItemsForDrag:")]
		void UpdateDraggingItems (NSTableView tableView, [Protocolize (4)] NSDraggingInfo draggingInfo);
	}

	//
	// This is the mixed NSTableViewDataSource and NSTableViewDelegate
	//
	[Model]
	[Synthetic]
	[BaseType (typeof (NSObject))]
	interface NSTableViewSource {
		//
		// These come from NSTableViewDataSource
		//
		[Export ("tableView:willDisplayCell:forTableColumn:row:")]
		void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:shouldEditTableColumn:row:")] [DefaultValue (false)]
		bool ShouldEditTableColumn (NSTableView tableView, NSTableColumn tableColumn, nint row);
	
		[Export ("selectionShouldChangeInTableView:")] [DefaultValue (false)]
		bool SelectionShouldChange (NSTableView tableView);
	
		[Export ("tableView:shouldSelectRow:")] [DefaultValue (true)]
		bool ShouldSelectRow (NSTableView tableView, nint row);
	
		[Export ("tableView:selectionIndexesForProposedSelection:")]
		NSIndexSet GetSelectionIndexes (NSTableView tableView, NSIndexSet proposedSelectionIndexes);
	
		[Export ("tableView:shouldSelectTableColumn:")] [DefaultValue (true)]
		bool ShouldSelectTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:mouseDownInHeaderOfTableColumn:")]
		void MouseDown (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:didClickTableColumn:")]
		void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		[Export ("tableView:didDragTableColumn:")]
		void DidDragTableColumn (NSTableView tableView, NSTableColumn tableColumn);
	
		//FIXME: Binding NSRectPointer
		//[Export ("tableView:toolTipForCell:rect:tableColumn:row:mouseLocation:")]
		//string TableViewtoolTipForCellrecttableColumnrowmouseLocation (NSTableView tableView, NSCell cell, NSRectPointer rect, NSTableColumn tableColumn, int row, CGPoint mouseLocation);
	
		[Export ("tableView:heightOfRow:")]
		nfloat GetRowHeight (NSTableView tableView, nint row );
	
		[Export ("tableView:typeSelectStringForTableColumn:row:")]
		string GetSelectString (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:nextTypeSelectMatchFromRow:toRow:forString:")]
		nint GetNextTypeSelectMatch (NSTableView tableView, nint startRow, nint endRow, string searchString );
	
		[Export ("tableView:shouldTypeSelectForEvent:withCurrentSearchString:")]
		bool ShouldTypeSelect (NSTableView tableView, NSEvent theEvent, string searchString );
	
		[Export ("tableView:shouldShowCellExpansionForTableColumn:row:")]
		bool ShouldShowCellExpansion (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:shouldTrackCell:forTableColumn:row:")]
		bool ShouldTrackCell (NSTableView tableView, NSCell cell, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:dataCellForTableColumn:row:")]
		NSCell GetCell (NSTableView tableView, NSTableColumn tableColumn, nint row );
	
		[Export ("tableView:isGroupRow:"), DefaultValue (false)]
		bool IsGroupRow (NSTableView tableView, nint row );
	
		[Export ("tableView:sizeToFitWidthOfColumn:")]
		nfloat GetSizeToFitColumnWidth (NSTableView tableView, nint column );
	
		[Export ("tableView:shouldReorderColumn:toColumn:")]
		bool ShouldReorder (NSTableView tableView, nint columnIndex, nint newColumnIndex );
	
		[Export ("tableViewSelectionDidChange:")]
		void SelectionDidChange (NSNotification notification);
	
		[Export ("tableViewColumnDidMove:")]
		void ColumnDidMove (NSNotification notification);
	
		[Export ("tableViewColumnDidResize:")]
		void ColumnDidResize (NSNotification notification);
	
		[Export ("tableViewSelectionIsChanging:")]
		void SelectionIsChanging (NSNotification notification);

		// NSTableViewDataSource
		[Export ("numberOfRowsInTableView:")]
		nint GetRowCount (NSTableView tableView);
	
		[Export ("tableView:objectValueForTableColumn:row:")]
		NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:setObjectValue:forTableColumn:row:")]
		void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint row);
	
		[Export ("tableView:sortDescriptorsDidChange:")]
		void SortDescriptorsChanged (NSTableView tableView, NSSortDescriptor [] oldDescriptors);
	
		[Export ("tableView:writeRowsWithIndexes:toPasteboard:")]
		bool WriteRows (NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard );
	
		[Export ("tableView:validateDrop:proposedRow:proposedDropOperation:")]
		NSDragOperation ValidateDrop (NSTableView tableView, [Protocolize (4)] NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation);
	
		[Export ("tableView:acceptDrop:row:dropOperation:")]
		bool AcceptDrop (NSTableView tableView, [Protocolize (4)] NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation);
	
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSFilePromiseReceiver' objects instead.")]
		[Export ("tableView:namesOfPromisedFilesDroppedAtDestination:forDraggedRowsWithIndexes:")]
		string [] FilesDropped (NSTableView tableView, NSUrl dropDestination, NSIndexSet indexSet );
		
		[Mac (10, 7)]
		[Export ("tableView:viewForTableColumn:row:")]
		NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row);

		[Mac (10, 7)]
		[Export ("tableView:rowViewForRow:")]
		NSTableRowView GetRowView (NSTableView tableView, nint row);

		[Mac (10, 7)]
		[Export ("tableView:didAddRowView:forRow:")]
		void DidAddRowView (NSTableView tableView, NSTableRowView rowView, nint row);

		[Mac (10, 7)]
		[Export ("tableView:didRemoveRowView:forRow:")]
		void DidRemoveRowView (NSTableView tableView, NSTableRowView rowView, nint row);

		[Mac (10, 7)]
		[Export ("tableView:pasteboardWriterForRow:")]
#if XAMCORE_2_0
		INSPasteboardWriting GetPasteboardWriterForRow (NSTableView tableView, nint row);
#else
		NSPasteboardWriting GetPasteboardWriterForRow (NSTableView tableView, nint row);
#endif

		[Mac (10, 7)]
		[Export ("tableView:draggingSession:willBeginAtPoint:forRowIndexes:")]
		void DraggingSessionWillBegin (NSTableView tableView, NSDraggingSession draggingSession, CGPoint willBeginAtScreenPoint, NSIndexSet rowIndexes);

		[Mac (10, 7)]
		[Export ("tableView:draggingSession:endedAtPoint:operation:")]
		void DraggingSessionEnded (NSTableView tableView, NSDraggingSession draggingSession, CGPoint endedAtScreenPoint, NSDragOperation operation);

		[Mac (10, 7)]
		[Export ("tableView:updateDraggingItemsForDrag:")]
		void UpdateDraggingItems (NSTableView tableView, [Protocolize (4)] NSDraggingInfo draggingInfo);
	}
	
	[BaseType (typeof (NSTextFieldCell))]
	interface NSTableHeaderCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("drawSortIndicatorWithFrame:inView:ascending:priority:")]
		void DrawSortIndicator (CGRect cellFrame, NSView controlView, bool ascending, nint priority );
	
		[Export ("sortIndicatorRectForBounds:")]
		CGRect GetSortIndicatorRect (CGRect theRect );
	}
	
	[BaseType (typeof (NSView))]
	interface NSTableHeaderView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("draggedColumn")]
		nint DraggedColumn { get; }
	
		[Export ("draggedDistance")]
		nfloat DraggedDistance { get; }
	
		[Export ("resizedColumn")]
		nint ResizedColumn { get; }
	
		[Export ("headerRectOfColumn:")]
		CGRect GetHeaderRect (nint column);
	
		[Export ("columnAtPoint:")]
		nint GetColumn (CGPoint point);
	
		//Detected properties
		[Export ("tableView")]
		NSTableView TableView { get; set; }
	}

	[Mac (10,11)]
	[BaseType (typeof(NSObject))]
	interface NSTableViewRowAction : NSCopying
	{
		[Static]
		[Export ("rowActionWithStyle:title:handler:")]
		NSTableViewRowAction FromStyle (NSTableViewRowActionStyle style, string title, Action<NSTableViewRowAction, nint> handler);

		[Export ("style")]
		NSTableViewRowActionStyle Style { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Mac (10,12)]
		[Export ("image", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSImage Image { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	}
		
	[BaseType (typeof (NSView), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTabViewDelegate)})]
	partial interface NSTabView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("selectTabViewItem:")]
		void Select (NSTabViewItem tabViewItem);

		[Export ("selectTabViewItemAtIndex:")]
		void SelectAt (nint index);

		[Export ("selectTabViewItemWithIdentifier:")]
		void Select (NSObject identifier);

		[Export ("takeSelectedTabViewItemFromSender:")]
		void TakeSelectedTabViewItemFrom (NSObject sender);

		[Export ("selectFirstTabViewItem:")]
		void SelectFirst (NSObject sender);

		[Export ("selectLastTabViewItem:")]
		void SelectLast (NSObject sender);

		[Export ("selectNextTabViewItem:")]
		void SelectNext (NSObject sender);

		[Export ("selectPreviousTabViewItem:")]
		void SelectPrevious (NSObject sender);

		[Export ("selectedTabViewItem")]
		NSTabViewItem Selected { get; }

		[Export ("font", ArgumentSemantic.Retain)]
		NSFont Font { get; set; }

		[Export ("tabViewType")]
		NSTabViewType TabViewType { get; set; }

		[Export ("tabViewItems")]
		NSTabViewItem [] Items { get; }

		[Export ("allowsTruncatedLabels")]
		bool AllowsTruncatedLabels { get; set; }

		[Export ("minimumSize")]
		CGSize MinimumSize { get; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("controlTint")]
		NSControlTint ControlTint { get; set; }

		[Export ("controlSize")]
		NSControlSize ControlSize { get; set; }

		[Export ("addTabViewItem:")][PostGet ("Items")]
		void Add (NSTabViewItem tabViewItem);

		[Export ("insertTabViewItem:atIndex:")][PostGet ("Items")]
		void Insert (NSTabViewItem tabViewItem, nint index);

		[Export ("removeTabViewItem:")][PostGet ("Items")]
		void Remove (NSTabViewItem tabViewItem);

		[Export ("delegate",  ArgumentSemantic.Assign), NullAllowed]
		[Protocolize]
		NSTabViewDelegate Delegate { get; set; }

		[Export ("tabViewItemAtPoint:")]
		NSTabViewItem TabViewItemAtPoint (CGPoint point);

		[Export ("contentRect")]
		CGRect ContentRect { get; }

		[Export ("numberOfTabViewItems")]
		nint Count { get; }

		[Export ("indexOfTabViewItem:")]
		nint IndexOf (NSTabViewItem tabViewItem);

		[Export ("tabViewItemAtIndex:")]
		NSTabViewItem Item (nint index);

		[Export ("indexOfTabViewItemWithIdentifier:")]
		nint IndexOf (NSObject identifier);

		[Mac (10, 12)]
		[Export ("tabPosition")]
		NSTabPosition TabPosition { get; set; }

		[Mac (10, 12)]
		[Export ("tabViewBorderType")]
		NSTabViewBorderType BorderType { get; set; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSViewController))]
	interface NSTabViewController : NSTabViewDelegate, NSToolbarDelegate {
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("tabStyle")]
		NSTabViewControllerTabStyle TabStyle { get; set; }

		[Export ("tabView", ArgumentSemantic.Strong)]
		NSTabView TabView { get; set; }

//		[FAIL] Selector not found for AppKit.NSTabViewController : segmentedControl
//		[FAIL] Selector not found for AppKit.NSTabViewController : setSegmentedControl:
		[Availability (Obsoleted = Platform.Mac_10_11)] // Test failures on El Capitan
		[Export ("segmentedControl", ArgumentSemantic.Strong)]
		NSSegmentedControl SegmentedControl { get; set; }

		[Export ("transitionOptions")]
		NSViewControllerTransitionOptions TransitionOptions { get; set; }

		[Export ("canPropagateSelectedChildViewControllerTitle")]
		bool CanPropagateSelectedChildViewControllerTitle { get; set; }

		[Export ("tabViewItems", ArgumentSemantic.Copy)]
		NSTabViewItem [] TabViewItems { get; set; }

		[Export ("selectedTabViewItemIndex")]
		nint SelectedTabViewItemIndex { get; set; }

		[Export ("addTabViewItem:")][PostGet("TabViewItems")]
		void AddTabViewItem (NSTabViewItem tabViewItem);

		[Export ("insertTabViewItem:atIndex:")][PostGet("TabViewItems")]
		void InsertTabViewItem (NSTabViewItem tabViewItem, nint index);

		[Export ("removeTabViewItem:")][PostGet("TabViewItems")]
		void RemoveTabViewItem (NSTabViewItem tabViewItem);

		[Export ("tabViewItemForViewController:")]
		NSTabViewItem GetTabViewItem (NSViewController viewController);

		// 'new' since it's inlined from NSTabViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("tabView:shouldSelectTabViewItem:"), DelegateName ("NSTabViewPredicate")]
		new bool ShouldSelectTabViewItem (NSTabView tabView, NSTabViewItem item);

		// 'new' since it's inlined from NSTabViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("tabView:willSelectTabViewItem:"), EventArgs ("NSTabViewItem")]
		new void WillSelect (NSTabView tabView, NSTabViewItem item);

		// 'new' since it's inlined from NSTabViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("tabView:didSelectTabViewItem:"), EventArgs ("NSTabViewItem")]
		new void DidSelect (NSTabView tabView, NSTabViewItem item);

		// 'new' since it's inlined from NSToolbarViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("toolbar:itemForItemIdentifier:willBeInsertedIntoToolbar:"), DelegateName ("NSToolbarWillInsert")]
		new NSToolbarItem WillInsertItem (NSToolbar toolbar, string itemIdentifier, bool willBeInserted);

		// 'new' since it's inlined from NSToolbarViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("toolbarDefaultItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers")]
		new string [] DefaultItemIdentifiers (NSToolbar toolbar);

		// 'new' since it's inlined from NSToolbarViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("toolbarAllowedItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers")]
		new string [] AllowedItemIdentifiers (NSToolbar toolbar);

		// 'new' since it's inlined from NSToolbarViewDelegate as this instance needs [RequiresSuper]
		[RequiresSuper]
		[Export ("toolbarSelectableItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers")]
		new string [] SelectableItemIdentifiers (NSToolbar toolbar);
	}

	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface NSTabViewDelegate {
		[Export ("tabView:shouldSelectTabViewItem:"), DelegateName ("NSTabViewPredicate"), DefaultValue (true)]
		bool ShouldSelectTabViewItem (NSTabView tabView, NSTabViewItem item);
		
		[Export ("tabView:willSelectTabViewItem:"), EventArgs ("NSTabViewItem")]
		void WillSelect (NSTabView tabView, NSTabViewItem item);

		[Export ("tabView:didSelectTabViewItem:"), EventArgs ("NSTabViewItem")]
		void DidSelect (NSTabView tabView, NSTabViewItem item);
	 
		[Export ("tabViewDidChangeNumberOfTabViewItems:")]
		void NumberOfItemsChanged (NSTabView tabView);
	}

	[BaseType (typeof (NSObject))]
	interface NSTabViewItem : NSCoding {
		[Export ("initWithIdentifier:")]
		IntPtr Constructor (NSObject identifier);

		[Export ("identifier", ArgumentSemantic.Retain)]
		NSObject Identifier { get; set; }

		[Export ("view", ArgumentSemantic.Retain)]
		NSView View { get; set; }

		[Export ("initialFirstResponder")]
		NSObject InitialFirstResponder { get; set; }

		[Export ("label")]
		string Label { get; set; }

		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }

		[Export ("tabState")]
		NSTabState TabState { get; }

		[Export ("tabView")]
		NSTabView TabView { get; }

		[Export ("drawLabel:inRect:")]
		void DrawLabel (bool shouldTruncateLabel, CGRect labelRect);

		[Export ("sizeOfLabel:")]
		CGSize SizeOfLabel (bool computeMin);

		[Export ("toolTip")]
		string ToolTip { get; set; }

		[Mac (10,10)]
		[Export ("image", ArgumentSemantic.Strong)]
		NSImage Image { get; set; }

		[Mac (10,10)]
		[Export ("viewController", ArgumentSemantic.Strong)]
		NSViewController ViewController { get; set; }

		[Mac (10,10)]
		[Static, Export ("tabViewItemWithViewController:")]
		NSTabViewItem GetTabViewItem (NSViewController viewController);
	}
	
	[BaseType (typeof (NSView), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTextDelegate)})]
	partial interface NSText {
		[DesignatedInitializer]
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("replaceCharactersInRange:withString:")]
		void Replace (NSRange range, string aString);

		[Export ("replaceCharactersInRange:withRTF:")]
		void ReplaceWithRtf (NSRange range, NSData rtfData);

		[Export ("replaceCharactersInRange:withRTFD:")]
		void ReplaceWithRtfd (NSRange range, NSData rtfdData);

		[Export ("RTFFromRange:")]
		NSData RtfFromRange (NSRange range);

		[Export ("RTFDFromRange:")]
		NSData RtfdFromRange (NSRange range);

		[Export ("writeRTFDToFile:atomically:")]
		bool WriteRtfd (string path, bool atomically);

		[Export ("readRTFDFromFile:")]
		bool FromRtfdFile (string path);

		[Export ("isRulerVisible")]
		bool IsRulerVisible { get; }

		[Export ("scrollRangeToVisible:")]
		void ScrollRangeToVisible (NSRange range);

		[Export ("setTextColor:range:")]
		void SetTextColor (NSColor color, NSRange range);

		[Export ("setFont:range:")]
		void SetFont (NSFont font, NSRange range);

		[Export ("sizeToFit")]
		void SizeToFit ();

		[Export ("copy:")]
		void Copy (NSObject sender);

		[Export ("copyFont:")]
		void CopyFont (NSObject sender);

		[Export ("copyRuler:")]
		void CopyRuler (NSObject sender);

		[Export ("cut:")]
		void Cut (NSObject sender);

		[Export ("delete:")]
		void Delete (NSObject sender);

		[Export ("paste:")]
		void Paste (NSObject sender);

		[Export ("pasteFont:")]
		void PasteFont (NSObject sender);

		[Export ("pasteRuler:")]
		void PasteRuler (NSObject sender);

		[Export ("selectAll:")]
		void SelectAll (NSObject sender);

		[Export ("changeFont:")]
		void ChangeFont (NSObject sender);

		[Export ("alignLeft:")]
		void AlignLeft (NSObject sender);

		[Export ("alignRight:")]
		void AlignRight (NSObject sender);

		[Export ("alignCenter:")]
		void AlignCenter (NSObject sender);

		[Export ("subscript:")]
		void Subscript (NSObject sender);

		[Export ("superscript:")]
		void Superscript (NSObject sender);

		[Export ("underline:")]
		void Underline (NSObject sender);

		[Export ("unscript:")]
		void Unscript (NSObject sender);

		[Export ("showGuessPanel:")]
		void ShowGuessPanel (NSObject sender);

		[Export ("checkSpelling:")]
		void CheckSpelling (NSObject sender);

		[Export ("toggleRuler:")]
		void ToggleRuler (NSObject sender);

		//Detected properties
		[Export ("string")]
		string Value { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTextDelegate Delegate { get; set; }
		
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")]get; set; }

		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")]get; set; }

		[Export ("richText")]
		bool RichText { [Bind ("isRichText")]get; set; }

		[Export ("importsGraphics")]
		bool ImportsGraphics { get; set; }

		[Export ("fieldEditor")]
		bool FieldEditor { [Bind ("isFieldEditor")]get; set; }

		[Export ("usesFontPanel")]
		bool UsesFontPanel { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("selectedRange")]
		NSRange SelectedRange { get; set; }

		[Export ("font", ArgumentSemantic.Retain)]
		NSFont Font { get; set; }

		[Export ("textColor", ArgumentSemantic.Copy)]
		NSColor TextColor { get; set; }

		[Export ("alignment")]
		NSTextAlignment Alignment { get; set; }

		[Export ("baseWritingDirection")]
		NSWritingDirection BaseWritingDirection { get; set; }

		[Export ("maxSize")]
		CGSize MaxSize { get; set; }

		[Export ("minSize")]
		CGSize MinSize { get; set; }

		[Export ("horizontallyResizable")]
		bool HorizontallyResizable { [Bind ("isHorizontallyResizable")]get; set; }

		[Export ("verticallyResizable")]
		bool VerticallyResizable { [Bind ("isVerticallyResizable")]get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSTextDelegate {
		[Export ("textShouldBeginEditing:"), DelegateName ("NSTextPredicate"), DefaultValue (true)]
		bool TextShouldBeginEditing (NSText textObject);

		[Export ("textShouldEndEditing:"), DelegateName ("NSTextPredicate"), DefaultValue (true)]
		bool TextShouldEndEditing (NSText textObject);

		[Export ("textDidBeginEditing:"), EventArgs ("NSNotification")]
		void TextDidBeginEditing (NSNotification notification);

		[Export ("textDidEndEditing:"), EventArgs ("NSNotification")]
		void TextDidEndEditing (NSNotification notification);

		[Export ("textDidChange:"), EventArgs ("NSNotification")]
		void TextDidChange (NSNotification notification);
	}

	[BaseType (typeof (NSCell))]
	interface NSTextAttachmentCell {
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("wantsToTrackMouse")]
		bool WantsToTrackMouse ();

		[Export ("highlight:withFrame:inView:")]
		void Highlight (bool highlight, CGRect cellFrame, NSView controlView);

		[Export ("trackMouse:inRect:ofView:untilMouseUp:")]
		bool TrackMouse (NSEvent theEvent, CGRect cellFrame, NSView controlView, bool untilMouseUp);

		[Export ("cellSize")]
		CGSize CellSize { get; }

		[Export ("cellBaselineOffset")]
		CGPoint CellBaselineOffset { get; }

		[Export ("drawWithFrame:inView:characterIndex:")]
		void DrawWithFrame (CGRect cellFrame, NSView controlView, nuint charIndex);

		[Export ("drawWithFrame:inView:characterIndex:layoutManager:")]
		void DrawWithFrame (CGRect cellFrame, NSView controlView, nuint charIndex, NSLayoutManager layoutManager);

		[Export ("wantsToTrackMouseForEvent:inRect:ofView:atCharacterIndex:")]
		bool WantsToTrackMouse (NSEvent theEvent, CGRect cellFrame, NSView controlView, nuint charIndex);

		[Export ("trackMouse:inRect:ofView:atCharacterIndex:untilMouseUp:")]
		bool TrackMouse (NSEvent theEvent, CGRect cellFrame, NSView controlView, nuint charIndex, bool untilMouseUp);

		[Export ("cellFrameForTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect CellFrameForTextContainer (NSTextContainer textContainer, CGRect lineFrag, CGPoint position, nuint charIndex);

		//Detected properties
		[Export ("attachment")][NullAllowed]
		NSTextAttachment Attachment { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSTextAttachment : NSCoding {
		[Export ("initWithFileWrapper:")]
		IntPtr Constructor (NSFileWrapper fileWrapper);

		[Mac (10,11)]
		[Export ("initWithData:ofType:")]
		[DesignatedInitializer]
		IntPtr Constructor ([NullAllowed] NSData contentData, [NullAllowed] string uti);

		//Detected properties
		[Export ("fileWrapper", ArgumentSemantic.Retain)]
		NSFileWrapper FileWrapper { get; set; }

		[Export ("attachmentCell", ArgumentSemantic.Retain)]
		NSTextAttachmentCell AttachmentCell { get; set; }

		[Mac (10,11, onlyOn64: true)] // 32-bit gives: [FAIL] Selector not found for AppKit.NSTextAttachment : contents
		[NullAllowed, Export ("contents", ArgumentSemantic.Copy)]
		NSData Contents { get; set; }

		[Mac (10,11, onlyOn64: true)]
		[NullAllowed, Export ("fileType")]
		string FileType { get; set; }

		[Mac (10,11, onlyOn64: true)] // 32-bit gives: [FAIL] Selector not found for AppKit.NSTextAttachment : image
		[NullAllowed, Export ("image", ArgumentSemantic.Strong)]
		NSImage Image { get; set; }

		[Mac (10,11, onlyOn64: true)] // 32-bit gives: [FAIL] Selector not found for AppKit.NSTextAttachment : bounds
		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; set; }
	}

	[Mac (10,11)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSTextAttachmentContainer
	{
		[Abstract]
		[Export ("imageForBounds:textContainer:characterIndex:")]
		[return: NullAllowed]
		NSImage GetImage (CGRect imageBounds, [NullAllowed] NSTextContainer textContainer, nuint charIndex);

		[Abstract]
		[Export ("attachmentBoundsForTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetAttachmentBounds ([NullAllowed] NSTextContainer textContainer, CGRect lineFrag, CGPoint position, nuint charIndex);
	}

	[DesignatedDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSTextBlock : NSCoding, NSCopying {
		[Export ("setValue:type:forDimension:")]
		void SetValue (nfloat val, NSTextBlockValueType type, NSTextBlockDimension dimension);

		[Export ("valueForDimension:")]
		nfloat GetValue (NSTextBlockDimension dimension);

		[Export ("valueTypeForDimension:")]
		NSTextBlockValueType GetValueType (NSTextBlockDimension dimension);

		[Export ("setContentWidth:type:")]
		void SetContentWidth (nfloat val, NSTextBlockValueType type);

		[Export ("contentWidth")]
		nfloat ContentWidth { get; }

		[Export ("contentWidthValueType")]
		NSTextBlockValueType ContentWidthValueType { get; }

		[Export ("setWidth:type:forLayer:edge:")]
		void SetWidth (nfloat val, NSTextBlockValueType type, NSTextBlockLayer layer, NSRectEdge edge);

		[Export ("setWidth:type:forLayer:")]
		void SetWidth (nfloat val, NSTextBlockValueType type, NSTextBlockLayer layer);

		[Export ("widthForLayer:edge:")]
		nfloat GetWidth (NSTextBlockLayer layer, NSRectEdge edge);

		[Export ("widthValueTypeForLayer:edge:")]
		NSTextBlockValueType WidthValueTypeForLayer (NSTextBlockLayer layer, NSRectEdge edge);

		[Export ("setBorderColor:forEdge:")]
		void SetBorderColor (NSColor color, NSRectEdge edge);

		[Export ("setBorderColor:")]
		void SetBorderColor (NSColor color);

		[Export ("borderColorForEdge:")]
		NSColor GetBorderColor (NSRectEdge edge);

		[Export ("rectForLayoutAtPoint:inRect:textContainer:characterRange:")]
		CGRect GetRectForLayout (CGPoint startingPoint, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[Export ("boundsRectForContentRect:inRect:textContainer:characterRange:")]
		CGRect GetBoundsRect (CGRect contentRect, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[Export ("drawBackgroundWithFrame:inView:characterRange:layoutManager:")]
		void DrawBackground (CGRect frameRect, NSView controlView, NSRange charRange, NSLayoutManager layoutManager);

		//Detected properties
		[Export ("verticalAlignment")]
		NSTextBlockVerticalAlignment VerticalAlignment { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

	}

	[BaseType (typeof (NSControl), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTextFieldDelegate)})]
	partial interface NSTextField : NSAccessibilityNavigableStaticText {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);
		
		[Export ("selectText:")]
		void SelectText (NSObject sender);

		[Export ("textShouldBeginEditing:")]
		bool ShouldBeginEditing (NSText textObject);

		[Export ("textShouldEndEditing:")]
		bool ShouldEndEditing (NSText textObject);

		[Export ("textDidBeginEditing:")]
		void DidBeginEditing (NSNotification notification);

		[Export ("textDidEndEditing:")]
		void DidEndEditing (NSNotification notification);

		[Export ("textDidChange:")]
		void DidChange (NSNotification notification);

		[Export ("acceptsFirstResponder")]
		bool AcceptsFirstResponder ();

		//Detected properties
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("textColor", ArgumentSemantic.Copy)]
		NSColor TextColor { get; set; }

		[Export ("bordered")]
		bool Bordered { [Bind ("isBordered")]get; set; }

		[Export ("bezeled")]
		bool Bezeled { [Bind ("isBezeled")]get; set; }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")]get; set; }

		[Export ("selectable")]
		bool Selectable { [Bind ("isSelectable")]get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTextFieldDelegate Delegate { get; set; }

		[Export ("bezelStyle")]
		NSTextFieldBezelStyle BezelStyle { get; set; }

		[Export ("allowsEditingTextAttributes")]
		bool AllowsEditingTextAttributes { get; set; }

		[Export ("importsGraphics")]
		bool ImportsGraphics { get; set; }

		[Mac (10, 8), Export ("preferredMaxLayoutWidth")]
		nfloat PreferredMaxLayoutWidth { get; set; }

		[Mac (10,10)]
		[Export ("placeholderString", ArgumentSemantic.Copy)]
		string PlaceholderString { get; set; }

		[Mac (10,10)]
		[Export ("placeholderAttributedString", ArgumentSemantic.Copy)]
		NSAttributedString PlaceholderAttributedString { get; set; }

		[Mac (10,11)]
		[Export ("maximumNumberOfLines", ArgumentSemantic.Assign)]
		nint MaximumNumberOfLines { get; set; }

		[Mac (10,11)]
		[Export ("allowsDefaultTighteningForTruncation")]
		bool AllowsDefaultTighteningForTruncation { get; set; }

		[Mac (10,12)]
		[Static]
		[Export ("labelWithString:")]
		NSTextField CreateLabel(string stringValue);

		[Mac (10,12)]
		[Static]
		[Export ("wrappingLabelWithString:")]
		NSTextField CreateWrappingLabel (string stringValue);

		[Mac (10,12)]
		[Static]
		[Export ("labelWithAttributedString:")]
		NSTextField CreateLabel (NSAttributedString attributedStringValue);

		[Mac (10,12)]
		[Static]
		[Export ("textFieldWithString:")]
		NSTextField CreateTextField ([NullAllowed] string stringValue);
	}

	[Mac (10, 12, 1)]
	[Category]
	[BaseType (typeof(NSTextField))]
	interface NSTextField_NSTouchBar
	{
		[Export ("isAutomaticTextCompletionEnabled")]
		bool GetAutomaticTextCompletionEnabled ();

		[Export ("automaticTextCompletionEnabled:")]
		void SetAutomaticTextCompletionEnabled (bool enabled);

		[Mac (10, 12, 2)]
		[Export ("allowsCharacterPickerTouchBarItem")]
		bool GetAllowsCharacterPickerTouchBarItem ();

		[Export ("setAllowsCharacterPickerTouchBarItem:")]
		void SetAllowsCharacterPickerTouchBarItem (bool allows);
	}


	[BaseType (typeof (NSTextField))]
	interface NSSecureTextField {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);
	}

	interface INSTextFieldDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSTextFieldDelegate {
		[Export ("control:textShouldBeginEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldBeginEditing (NSControl control, NSText fieldEditor);

		[Export ("control:textShouldEndEditing:"), DelegateName ("NSControlText"), DefaultValue (true)]
		bool TextShouldEndEditing (NSControl control, NSText fieldEditor);

		[Export ("control:didFailToFormatString:errorDescription:"), DelegateName ("NSControlTextError"), DefaultValue (true)]
		bool DidFailToFormatString (NSControl control, string str, string error);
		
		[Export ("control:didFailToValidatePartialString:errorDescription:"), EventArgs ("NSControlTextError")]
		void DidFailToValidatePartialString (NSControl control, string str, string error);
		
		[Export ("control:isValidObject:"), DelegateName ("NSControlTextValidation"), DefaultValue (true)]
		bool IsValidObject (NSControl control, NSObject objectToValidate);

		[Export ("control:textView:doCommandBySelector:"), DelegateName ("NSControlCommand"), DefaultValue (false)]
		bool DoCommandBySelector (NSControl control, NSTextView textView, Selector commandSelector);

		[Export ("control:textView:completions:forPartialWordRange:indexOfSelectedItem:"), DelegateName ("NSControlTextFilter"), DefaultValue ("new string[0]")]
		string [] GetCompletions (NSControl control, NSTextView textView, string [] words, NSRange charRange, ref nint index);

		[Export ("controlTextDidEndEditing:"), EventArgs ("NSNotification")]
		void EditingEnded (NSNotification notification);

		[Export ("controlTextDidChange:"), EventArgs ("NSNotification")]
		void Changed (NSNotification notification);

		[Export ("controlTextDidBeginEditing:"), EventArgs ("NSNotification")]
		void EditingBegan (NSNotification notification);

		[Mac (10,12,2)]
		[Export ("textField:textView:candidatesForSelectedRange:"), DelegateName ("NSTextFieldGetCandidates"), DefaultValue (null)]
		[return: NullAllowed]
		NSObject[] GetCandidates (NSTextField textField, NSTextView textView, NSRange selectedRange);

		[Mac (10,12,2)]
		[Export ("textField:textView:candidates:forSelectedRange:"), DelegateName ("NSTextFieldTextCheckingResults"), DefaultValue (null)]
		NSTextCheckingResult[] GetTextCheckingResults (NSTextField textField, NSTextView textView, NSTextCheckingResult[] candidates, NSRange selectedRange);

		[Mac (10,12,2)]
		[Export ("textField:textView:shouldSelectCandidateAtIndex:"), DelegateName ("NSTextFieldSelectCandidate"), DefaultValue (false)]
		bool ShouldSelectCandidate (NSTextField textField, NSTextView textView, nuint index);
	}
	
	[BaseType (typeof (NSTextFieldDelegate))]
	[Model]
	[Protocol]
	interface NSComboBoxDelegate {
		[Export ("comboBoxWillPopUp:")]
		void WillPopUp (NSNotification notification);

		[Export ("comboBoxWillDismiss:")]
		void WillDismiss (NSNotification notification);

		[Export ("comboBoxSelectionDidChange:")]
		void SelectionChanged (NSNotification notification);

		[Export ("comboBoxSelectionIsChanging:")]
		void SelectionIsChanging (NSNotification notification);
	}

	[BaseType (typeof(NSObject))]
	[Model]
	[Protocol]
	interface NSTokenFieldCellDelegate {
		[Export ("tokenFieldCell:completionsForSubstring:indexOfToken:indexOfSelectedItem:")]
		NSArray GetCompletionStrings (NSTokenFieldCell tokenFieldCell, string substring, nint tokenIndex, ref nint selectedIndex);

		[Export ("tokenFieldCell:shouldAddObjects:atIndex:")]
		NSArray ShouldAddObjects (NSTokenFieldCell tokenFieldCell, NSObject[] tokens, nuint index);

		[Export ("tokenFieldCell:displayStringForRepresentedObject:")]
		string GetDisplayString (NSTokenFieldCell tokenFieldCell, NSObject representedObject);

		[Export ("tokenFieldCell:editingStringForRepresentedObject:")]
		string GetEditingString (NSTokenFieldCell tokenFieldCell, NSObject representedObject);

		[Export ("tokenFieldCell:representedObjectForEditingString:")]
		[return: NullAllowed]
		NSObject GetRepresentedObject (NSTokenFieldCell tokenFieldCell, string editingString);

		[Export ("tokenFieldCell:writeRepresentedObjects:toPasteboard:")]
		bool WriteRepresentedObjects (NSTokenFieldCell tokenFieldCell, NSObject [] objects, NSPasteboard pboard);

		[Export ("tokenFieldCell:readFromPasteboard:")]
		NSObject [] Read (NSTokenFieldCell tokenFieldCell, NSPasteboard pboard);

		[Export ("tokenFieldCell:menuForRepresentedObject:")]
		NSMenu GetMenu (NSTokenFieldCell tokenFieldCell, NSObject representedObject);

		[Export ("tokenFieldCell:hasMenuForRepresentedObject:")]
		bool HasMenu (NSTokenFieldCell tokenFieldCell, NSObject representedObject);

		[Export ("tokenFieldCell:styleForRepresentedObject:")]
		NSTokenStyle GetStyle (NSTokenFieldCell tokenFieldCell, NSObject representedObject);
	}

	[BaseType (typeof (NSActionCell))]
	interface NSTextFieldCell {
		[DesignatedInitializer]
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);
	
		[Export ("initImageCell:")]
		IntPtr Constructor (NSImage  image);

		[Export ("setUpFieldEditorAttributes:")]
		NSText SetUpFieldEditorAttributes (NSText textObj);
	
		//Detected properties
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	
		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }
	
		[Export ("textColor", ArgumentSemantic.Copy)]
		NSColor TextColor { get; set; }
	
		[Export ("bezelStyle")]
		NSTextFieldBezelStyle BezelStyle { get; set; }
	
		[Export ("placeholderString")]
		string PlaceholderString { get; set; }
	
		[Export ("placeholderAttributedString", ArgumentSemantic.Copy)]
		NSAttributedString PlaceholderAttributedString { get; set; }
	
		[Export ("allowedInputSourceLocales")]
		string [] AllowedInputSourceLocales { get; set; }

		[Export ("wantsNotificationForMarkedText")]
		[Override]
		bool WantsNotificationForMarkedText { get; set; }
	}

	[BaseType (typeof(NSTextFieldCell))]
	[DisableDefaultCtor]
	interface NSTokenFieldCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("tokenStyle")]
		NSTokenStyle TokenStyle { get; set; }

		[Export ("completionDelay")]
		double CompletionDelay { get; set; }

		[Static]
		[Export ("defaultCompletionDelay")]
		double DefaultCompletionDelay { get; }

		[Export ("tokenizingCharacterSet", ArgumentSemantic.Copy), NullAllowed] 
		NSCharacterSet CharacterSet { get; set; }

		[Static]
		[Export ("defaultTokenizingCharacterSet")] 
		NSCharacterSet DefaultCharacterSet { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed] 
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTokenFieldCellDelegate Delegate { get; set; }
	}

	[BaseType (typeof (NSTextFieldCell))]
	interface NSSecureTextFieldCell {
		[Export ("initTextCell:")]
		IntPtr Constructor (string aString);

		[Export ("echosBullets")]
		bool EchosBullets { get; set; }
	}  

	interface INSTextInputClient {}

	[BaseType (typeof (NSObject))]
	partial interface NSTextInputContext {
		[Export ("initWithClient:")]
		[DesignatedInitializer]
		IntPtr Constructor ([Protocolize]NSTextInputClient client);

		[Static]
		[Export ("currentInputContext")]
		NSTextInputContext CurrentInputContext { get; }

		[Export ("client")]
		INSTextInputClient Client { get; }

		[Export ("acceptsGlyphInfo")]
		bool AcceptsGlyphInfo { get; set; }

		[NullAllowed, Export ("keyboardInputSources")]
		string[] KeyboardInputSources { get; }

		[NullAllowed, Export ("selectedKeyboardInputSource")]
		string SelectedKeyboardInputSource { get; set; }

		[NullAllowed, Export ("allowedInputSourceLocales", ArgumentSemantic.Copy)]
		string[] AllowedInputSourceLocales { get; set; }

		[Export ("activate")]
		void Activate ();

		[Export ("deactivate")]
		void Deactivate ();

		[Export ("handleEvent:")]
		bool HandleEvent (NSEvent theEvent);

		[Export ("discardMarkedText")]
		void DiscardMarkedText ();

		[Export ("invalidateCharacterCoordinates")]
		void InvalidateCharacterCoordinates ();

		[Static]
		[Export ("localizedNameForInputSource:")]
		string LocalizedNameForInputSource (string inputSourceIdentifier);
	}

	[BaseType (typeof (NSObject))]
	interface NSTextList : NSCoding, NSCopying {
		[Export ("initWithMarkerFormat:options:")]
		IntPtr Constructor (
#if XAMCORE_4_0
		[BindAs (typeof (NSTextListMarkerFormats))] 
#endif
		string format, NSTextListOptions mask);

		[Wrap ("this (format.GetConstant(), mask)")]
		IntPtr Constructor (NSTextListMarkerFormats format, NSTextListOptions mask);

#if XAMCORE_4_0
		[BindAs (typeof (NSTextListMarkerFormats))] 
#endif
		[Export ("markerFormat")]
		string MarkerFormat { get; }

		[Export ("listOptions")]
		NSTextListOptions ListOptions { get; }

		[Export ("markerForItemNumber:")]
		string GetMarker (nint itemNum);

		//Detected properties
		[Export ("startingItemNumber")]
		nint StartingItemNumber { get; set; }

	}

	enum NSTextListMarkerFormats
	{
		[Mac (10, 13)]
		[Field ("NSTextListMarkerBox")]
		Box,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerCheck")]
		Check,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerCircle")]
		Circle,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerDiamond")]
		Diamond,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerDisc")]
		Disc,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerHyphen")]
		Hyphen,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerSquare")]
		Square,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerLowercaseHexadecimal")]
		LowercaseHexadecimal,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerUppercaseHexadecimal")]
		UppercaseHexadecimal,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerOctal")]
		Octal,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerLowercaseAlpha")]
		LowercaseAlpha,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerUppercaseAlpha")]
		UppercaseAlpha,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerLowercaseLatin")]
		LowercaseLatin,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerUppercaseLatin")]
		UppercaseLatin,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerLowercaseRoman")]
		LowercaseRoman,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerUppercaseRoman")]
		UppercaseRoman,

		[Mac (10, 13)]
		[Field ("NSTextListMarkerDecimal")]
		Decimal,
	}
	
	[BaseType (typeof (NSTextBlock))]
	[DisableDefaultCtor]
	interface NSTextTableBlock {
		[DesignatedInitializer]
		[Export ("initWithTable:startingRow:rowSpan:startingColumn:columnSpan:")]
		IntPtr Constructor (NSTextTable table, nint row, nint rowSpan, nint col, nint colSpan);

		[Export ("table")]
		NSTextTable Table { get; }

		[Export ("startingRow")]
		nint StartingRow { get; }

		[Export ("rowSpan")]
		nint RowSpan { get; }

		[Export ("startingColumn")]
		nint StartingColumn { get; }

		[Export ("columnSpan")]
		nint ColumnSpan { get; }
	}

	[BaseType (typeof (NSTextBlock))]
	interface NSTextTable {
		[Export ("rectForBlock:layoutAtPoint:inRect:textContainer:characterRange:")]
		CGRect GetRectForBlock (NSTextTableBlock block, CGPoint startingPoint, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[Export ("boundsRectForBlock:contentRect:inRect:textContainer:characterRange:")]
		CGRect GetBoundsRect (NSTextTableBlock block, CGRect contentRect, CGRect rect, NSTextContainer textContainer, NSRange charRange);

		[Export ("drawBackgroundForBlock:withFrame:inView:characterRange:layoutManager:")]
		void DrawBackground (NSTextTableBlock block, CGRect frameRect, NSView controlView, NSRange charRange, NSLayoutManager layoutManager);

		//Detected properties
		[Export ("numberOfColumns")]
		nint Columns { get; set; }

		[Export ("layoutAlgorithm")]
		NSTextTableLayoutAlgorithm LayoutAlgorithm { get; set; }

		[Export ("collapsesBorders")]
		bool CollapsesBorders { get; set; }

		[Export ("hidesEmptyCells")]
		bool HidesEmptyCells { get; set; }
	}

	[BaseType (typeof (NSObject))]
	partial interface NSTextContainer : NSCoding {
		[Export ("initWithContainerSize:"), Internal]
		[Sealed]
		IntPtr InitWithContainerSize (CGSize size);

		[Mac (10,11)]
		[Export ("initWithSize:"), Internal]
		[Sealed]
		IntPtr InitWithSize (CGSize size);

		[Export ("replaceLayoutManager:")]
		void ReplaceLayoutManager (NSLayoutManager newLayoutManager);

		// FIXME: Binding
		//[Export ("lineFragmentRectForProposedRect:sweepDirection:movementDirection:remainingRect:")]
		//CGRect LineFragmentRect (CGRect proposedRect, NSLineSweepDirection sweepDirection, NSLineMovementDirection movementDirection, NSRectPointer remainingRect);

		[Export ("isSimpleRectangularTextContainer")]
		bool IsSimpleRectangularTextContainer { get; }

		[Export ("containsPoint:")]
		bool ContainsPoint (CGPoint point);

		//Detected properties
		[Export ("layoutManager")]
		NSLayoutManager LayoutManager { get; set; }

		[Export ("textView", ArgumentSemantic.Weak)]
		NSTextView TextView { get; set; }

		[Export ("widthTracksTextView")]
		bool WidthTracksTextView { get; set; }

		[Export ("heightTracksTextView")]
		bool HeightTracksTextView { get; set; }

		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use Size instead.")]
		[Export ("containerSize")]
		CGSize ContainerSize { get; set; }

		[Export ("lineFragmentPadding")]
		nfloat LineFragmentPadding { get; set; }

		[Mac (10,11)]
		[Export ("size", ArgumentSemantic.Assign)]
		CGSize Size { get; set; }

		[Mac (10,11)]
		[Export ("exclusionPaths", ArgumentSemantic.Copy)]
		// [Verify (StronglyTypedNSArray)]
		NSBezierPath[] ExclusionPaths { get; set; }

		[Mac (10,11)]
		[Export ("lineBreakMode", ArgumentSemantic.Assign)]
		NSLineBreakMode LineBreakMode { get; set; }

		[Mac (10,11)]
		[Export ("maximumNumberOfLines", ArgumentSemantic.Assign)]
		nuint MaximumNumberOfLines { get; set; }

		[Mac (10,11)]
		[Export ("lineFragmentRectForProposedRect:atIndex:writingDirection:remainingRect:")]
		CGRect GetLineFragmentRect (CGRect proposedRect, nuint characterIndex, NSWritingDirection baseWritingDirection, ref CGRect remainingRect);
	}

	[BaseType (typeof (NSMutableAttributedString), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTextStorageDelegate)})]
	partial interface NSTextStorage {
		[Export ("initWithString:")]
		IntPtr Constructor (string str);

		[Export ("addLayoutManager:")][PostGet ("LayoutManagers")]
		void AddLayoutManager (NSLayoutManager obj);

		[Export ("removeLayoutManager:")][PostGet ("LayoutManagers")]
		void RemoveLayoutManager (NSLayoutManager obj);

		[Export ("layoutManagers")]
		NSLayoutManager [] LayoutManagers { get; }

		[Export ("edited:range:changeInLength:")]
		void Edited (nuint editedMask, NSRange range, nint delta);

		[Export ("processEditing")]
		void ProcessEditing ();

		[Export ("invalidateAttributesInRange:")]
		void InvalidateAttributes (NSRange range);

		[Export ("ensureAttributesAreFixedInRange:")]
		void EnsureAttributesAreFixed (NSRange range);

		[Export ("fixesAttributesLazily")]
		bool FixesAttributesLazily { get; }

		[Export ("editedMask")]
#if !XAMCORE_4_0
		NSTextStorageEditedFlags EditedMask { get; }
#else
		NSTextStorageEditActions EditedMask { get; }
#endif

		[Export ("editedRange")]
		NSRange EditedRange { get; }

		[Export ("changeInLength")]
		nint ChangeInLength { get; }

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTextStorageDelegate Delegate { get; set; }

	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSTextStorageDelegate {
		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use WillProcessEditing instead.")]
		[Export ("textStorageWillProcessEditing:")]
		void TextStorageWillProcessEditing (NSNotification notification);

		[Availability (Deprecated = Platform.Mac_10_11, Message = "Use DidProcessEditing instead.")]
		[Export ("textStorageDidProcessEditing:")]
		void TextStorageDidProcessEditing (NSNotification notification);

		[Mac (10,11)]
		[Export ("textStorage:willProcessEditing:range:changeInLength:"), EventArgs ("NSTextStorage")]
		void WillProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);

		[Mac (10,11)]
		[Export ("textStorage:didProcessEditing:range:changeInLength:"), EventArgs ("NSTextStorage")]
		void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta);
	}

	[BaseType (typeof (NSObject))]
	interface NSTextTab : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithTextAlignment:location:options:")]
		IntPtr Constructor (NSTextAlignment alignment, nfloat loc, NSDictionary options);

		[Export ("alignment")]
		NSTextAlignment Alignment { get; }

		[Export ("options")]
		NSDictionary Options { get; }

		[Export ("initWithType:location:")]
		IntPtr Constructor (NSTextTabType type, nfloat loc);

		[Export ("location")]
		nfloat Location { get; }

		[Export ("tabStopType")]
		NSTextTabType TabStopType { get; }

		[Mac (10,11)]
		[Static]
		[Export ("columnTerminatorsForLocale:")]
		NSCharacterSet GetColumnTerminators ([NullAllowed] NSLocale locale);
	}

	[BaseType (typeof (NSText), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTextViewDelegate)})]
	partial interface NSTextView : NSTextInputClient, NSDraggingSource, NSTextFinderClient, NSAccessibilityNavigableStaticText, NSCandidateListTouchBarItemDelegate, NSTouchBarDelegate {
		[DesignatedInitializer]
		[Export ("initWithFrame:textContainer:")]
		IntPtr Constructor (CGRect frameRect, NSTextContainer container);

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("replaceTextContainer:")]
		void ReplaceTextContainer (NSTextContainer newContainer);

		[Export ("textContainerOrigin")]
		CGPoint TextContainerOrigin { get; }

		[Export ("invalidateTextContainerOrigin")]
		void InvalidateTextContainerOrigin ();

		[Export ("layoutManager")]
		NSLayoutManager LayoutManager { get; }

		[Export ("textStorage")]
		NSTextStorage TextStorage { get; }

		[Export ("insertText:")]
		void InsertText (NSObject insertString);

		[Export ("setConstrainedFrameSize:")]
		void SetConstrainedFrameSize (CGSize desiredSize);

		[Export ("setAlignment:range:")]
		void SetAlignmentRange (NSTextAlignment alignment, NSRange range);

		[Export ("setBaseWritingDirection:range:")]
		void SetBaseWritingDirection (NSWritingDirection writingDirection, NSRange range);

		[Export ("turnOffKerning:")]
		void TurnOffKerning (NSObject sender);

		[Export ("tightenKerning:")]
		void TightenKerning (NSObject sender);

		[Export ("loosenKerning:")]
		void LoosenKerning (NSObject sender);

		[Export ("useStandardKerning:")]
		void UseStandardKerning (NSObject sender);

		[Export ("turnOffLigatures:")]
		void TurnOffLigatures (NSObject sender);

		[Export ("useStandardLigatures:")]
		void UseStandardLigatures (NSObject sender);

		[Export ("useAllLigatures:")]
		void UseAllLigatures (NSObject sender);

		[Export ("raiseBaseline:")]
		void RaiseBaseline (NSObject sender);

		[Export ("lowerBaseline:")]
		void LowerBaseline (NSObject sender);

		[Export ("toggleTraditionalCharacterShape:")]
		void ToggleTraditionalCharacterShape (NSObject sender);

		[Export ("outline:")]
		void Outline (NSObject sender);

		[Export ("performFindPanelAction:")]
		void PerformFindPanelAction (NSObject sender);

		[Export ("alignJustified:")]
		void AlignJustified (NSObject sender);

		[Export ("changeColor:")]
		void ChangeColor (NSObject sender);

		[Export ("changeAttributes:")]
		void ChangeAttributes (NSObject sender);

		[Export ("changeDocumentBackgroundColor:")]
		void ChangeDocumentBackgroundColor (NSObject sender);

		[Export ("orderFrontSpacingPanel:")]
		void OrderFrontSpacingPanel (NSObject sender);

		[Export ("orderFrontLinkPanel:")]
		void OrderFrontLinkPanel (NSObject sender);

		[Export ("orderFrontListPanel:")]
		void OrderFrontListPanel (NSObject sender);

		[Export ("orderFrontTablePanel:")]
		void OrderFrontTablePanel (NSObject sender);

		[Export ("rulerView:didMoveMarker:")]
		void RulerViewDidMoveMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:didRemoveMarker:")]
		void RulerViewDidRemoveMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:didAddMarker:")]
		void RulerViewDidAddMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:shouldMoveMarker:")]
		bool RulerViewShouldMoveMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:shouldAddMarker:")]
		bool RulerViewShouldAddMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:willMoveMarker:toLocation:")]
		nfloat RulerViewWillMoveMarker (NSRulerView ruler, NSRulerMarker marker, nfloat location);

		[Export ("rulerView:shouldRemoveMarker:")]
		bool RulerViewShouldRemoveMarker (NSRulerView ruler, NSRulerMarker marker);

		[Export ("rulerView:willAddMarker:atLocation:")]
		nfloat RulerViewWillAddMarker (NSRulerView ruler, NSRulerMarker marker, nfloat location);

		[Export ("rulerView:handleMouseDown:")]
		void RulerViewHandleMouseDown (NSRulerView ruler, NSEvent theEvent);

		[Export ("setNeedsDisplayInRect:avoidAdditionalLayout:")]
		void SetNeedsDisplay (CGRect rect, bool avoidAdditionalLayout);

		[Export ("shouldDrawInsertionPoint")]
		bool ShouldDrawInsertionPoint { get; }

		[Export ("drawInsertionPointInRect:color:turnedOn:")]
		void DrawInsertionPoint (CGRect rect, NSColor color, bool turnedOn);

		[Export ("drawViewBackgroundInRect:")]
		void DrawViewBackgroundInRect (CGRect rect);

		[Export ("updateRuler")]
		void UpdateRuler ();

		[Export ("updateFontPanel")]
		void UpdateFontPanel ();

		[Export ("updateDragTypeRegistration")]
		void UpdateDragTypeRegistration ();

		[Export ("selectionRangeForProposedRange:granularity:")]
		NSRange SelectionRange (NSRange proposedCharRange, NSSelectionGranularity granularity);

		[Export ("clickedOnLink:atIndex:")]
		void ClickedOnLink (NSObject link, nuint charIndex);

		[Export ("startSpeaking:")]
		void StartSpeaking (NSObject sender);

		[Export ("stopSpeaking:")]
		void StopSpeaking (NSObject sender);

		[Export ("characterIndexForInsertionAtPoint:")]
		nuint CharacterIndex (CGPoint point);

		//Detected properties
		[Export ("textContainer")]
		NSTextContainer TextContainer { get; set; }

		[Export ("textContainerInset")]
		CGSize TextContainerInset { get; set; }

		//
		// Completion support
		//
		[Export ("complete:")]
		void Complete ([NullAllowed] NSObject sender);

		[Export ("rangeForUserCompletion")]
		NSRange RangeForUserCompletion ();

		[Export ("completionsForPartialWordRange:indexOfSelectedItem:")]
		string [] CompletionsForPartialWord (NSRange charRange, out nint index);

		[Export ("insertCompletion:forPartialWordRange:movement:isFinal:")]
		void InsertCompletion (string completion, NSRange partialWordCharRange, nint movement, bool isFinal);

		// Pasteboard
		[Export ("writablePasteboardTypes")]
		string [] WritablePasteboardTypes ();

		[Export ("writeSelectionToPasteboard:type:")]
		bool WriteSelectionToPasteboard (NSPasteboard pboard, string type);

		[Export ("writeSelectionToPasteboard:types:")]
		bool WriteSelectionToPasteboard (NSPasteboard pboard, string [] types);

		[Export ("readablePasteboardTypes")]
		string [] ReadablePasteboardTypes ();

		[Export ("preferredPasteboardTypeFromArray:restrictedToTypesFromArray:")]
		string GetPreferredPasteboardType (string [] availableTypes, string [] allowedTypes);

		[Export ("readSelectionFromPasteboard:type:")]
		bool ReadSelectionFromPasteboard (NSPasteboard pboard, string type);

		[Export ("readSelectionFromPasteboard:")]
		bool ReadSelectionFromPasteboard (NSPasteboard pboard);

		[Static]
		[Export ("registerForServices")]
		void RegisterForServices ();

		[Export ("validRequestorForSendType:returnType:")]
		NSObject ValidRequestorForSendType (string sendType, string returnType);

		[Export ("pasteAsPlainText:")]
		void PasteAsPlainText (NSObject sender);

		[Export ("pasteAsRichText:")]
		void PasteAsRichText (NSObject sender);

		//
		// Dragging support
		//

		// FIXME: Binding
		//[Export ("dragImageForSelectionWithEvent:origin:")]
		//NSImage DragImageForSelection (NSEvent theEvent, NSPointPointer origin);

		[Export ("acceptableDragTypes")]
		string [] AcceptableDragTypes ();

		[Export ("dragOperationForDraggingInfo:type:")]
		NSDragOperation DragOperationForDraggingInfo ([Protocolize (4)] NSDraggingInfo dragInfo, string type);

		[Export ("cleanUpAfterDragOperation")]
		void CleanUpAfterDragOperation ();

		[Export ("setSelectedRanges:affinity:stillSelecting:")]
		void SetSelectedRanges (NSArray /*NSRange []*/ ranges, NSSelectionAffinity affinity, bool stillSelectingFlag);

		[Export ("setSelectedRange:affinity:stillSelecting:")]
		void SetSelectedRange (NSRange charRange, NSSelectionAffinity affinity, bool stillSelectingFlag);

		[Export ("selectionAffinity")]
		NSSelectionAffinity SelectionAffinity ();

		[Export ("updateInsertionPointStateAndRestartTimer:")]
		void UpdateInsertionPointStateAndRestartTimer (bool restartFlag);

		[Export ("toggleContinuousSpellChecking:")]
		void ToggleContinuousSpellChecking (NSObject sender);

		[Export ("spellCheckerDocumentTag")]
		nint SpellCheckerDocumentTag ();

		[Export ("toggleGrammarChecking:")]
		void ToggleGrammarChecking (NSObject sender);

		[Export ("setSpellingState:range:")]
		void SetSpellingState (nint value, NSRange charRange);

		[Export ("shouldChangeTextInRanges:replacementStrings:")]
		bool ShouldChangeText (NSArray /* NSRange [] */ affectedRanges, string [] replacementStrings);

		[Export ("rangesForUserTextChange")]
		NSArray /* NSRange [] */ RangesForUserTextChange ();

		[Export ("rangesForUserCharacterAttributeChange")]
		NSArray /* NSRange [] */ RangesForUserCharacterAttributeChange ();

		[Export ("rangesForUserParagraphAttributeChange")]
		NSArray /* NSRange [] */ RangesForUserParagraphAttributeChange ();

		//[Export ("shouldChangeTextInRange:replacementString:")]
		//bool ShouldChangeText (NSRange affectedCharRange, string replacementString);

		[Export ("rangeForUserTextChange")]
		NSRange RangeForUserTextChange ();

		[Export ("rangeForUserCharacterAttributeChange")]
		NSRange RangeForUserCharacterAttributeChange ();

		[Export ("rangeForUserParagraphAttributeChange")]
		NSRange RangeForUserParagraphAttributeChange ();

		[Export ("breakUndoCoalescing")]
		void BreakUndoCoalescing ();

		[Export ("isCoalescingUndo")]
		bool IsCoalescingUndo ();

		[Export ("showFindIndicatorForRange:")]
		void ShowFindIndicatorForRange (NSRange charRange);

		[Export ("setSelectedRange:")]
		void SetSelectedRange (NSRange charRange);

		[Export ("selectionGranularity")]
		NSSelectionGranularity SelectionGranularity { get; set; }

		[Export ("selectedTextAttributes")]
		NSDictionary SelectedTextAttributes { get; set; }

		[Export ("insertionPointColor")]
		NSColor InsertionPointColor { get; set; }

		[Export ("markedTextAttributes")]
		NSDictionary MarkedTextAttributes { get; set; }

		[Export ("linkTextAttributes")]
		NSDictionary LinkTextAttributes { get; set; }

		[Export ("displaysLinkToolTips")]
		bool DisplaysLinkToolTips { get; set; }

		[Export ("acceptsGlyphInfo")]
		bool AcceptsGlyphInfo { get; set; }

		[Export ("rulerVisible")]
		bool RulerVisible { [Bind ("isRulerVisible")]get; set; }

		[Export ("usesRuler")]
		bool UsesRuler { get; set; }

		[Export ("continuousSpellCheckingEnabled")]
		bool ContinuousSpellCheckingEnabled { [Bind ("isContinuousSpellCheckingEnabled")]get; set; }

		[Export ("grammarCheckingEnabled")]
		bool GrammarCheckingEnabled { [Bind ("isGrammarCheckingEnabled")]get; set; }

		[Export ("typingAttributes")]
		NSDictionary TypingAttributes { get; set; }

		[Export ("usesFindPanel")]
		bool UsesFindPanel { get; set; }

		[Export ("allowsDocumentBackgroundColorChange")]
		bool AllowsDocumentBackgroundColorChange { get; set; }

		[Export ("defaultParagraphStyle")]
		NSParagraphStyle DefaultParagraphStyle { get; set; }

		[Export ("allowsUndo")]
		bool AllowsUndo { get; set; }

		[Export ("allowsImageEditing")]
		bool AllowsImageEditing { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTextViewDelegate Delegate { get; set; }

		[Export ("editable")]
		new bool Editable { [Bind ("isEditable")]get; set; }

		[Export ("selectable")]
		new bool Selectable { [Bind ("isSelectable")]get; set; }

		[Export ("richText")]
		bool RichText { [Bind ("isRichText")]get; set; }

		[Export ("importsGraphics")]
		bool ImportsGraphics { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Export ("fieldEditor")]
		bool FieldEditor { [Bind ("isFieldEditor")]get; set; }

		[Export ("usesFontPanel")]
		bool UsesFontPanel { get; set; }

		[Export ("allowedInputSourceLocales")]
		string [] AllowedInputSourceLocales { get; set; }

		// FIXME: binding
		//[Export ("shouldChangeTextInRanges:replacementStrings:")]
		//bool ShouldChangeTextInRanges (NSArray affectedRanges, NSArray replacementStrings);

		// FIXME: binding
		//[Export ("rangesForUserTextChange")]
		//NSArray RangesForUserTextChange ();

		// FIXME: binding
		//[Export ("rangesForUserCharacterAttributeChange")]
		//NSArray RangesForUserCharacterAttributeChange ();

		// FIXME: binding
		//[Export ("rangesForUserParagraphAttributeChange")]
		//NSArray RangesForUserParagraphAttributeChange ();

		[Export ("shouldChangeTextInRange:replacementString:")]
		bool ShouldChangeText (NSRange affectedCharRange, string replacementString);

		[Export ("didChangeText")]
		void DidChangeText ();

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		//
		// Smart copy/paset support
		//
		[Export ("smartDeleteRangeForProposedRange:")]
		NSRange SmartDeleteRangeForProposedRange (NSRange proposedCharRange);

		[Export ("toggleSmartInsertDelete:")]
		void ToggleSmartInsertDelete (NSObject sender);

		[Export ("smartInsertForString:replacingRange:beforeString:afterString:")]
		void SmartInsert (string pasteString, NSRange charRangeToReplace, string beforeString, string afterString);

		[Export ("smartInsertBeforeStringForString:replacingRange:")]
		string SmartInsertBefore (string pasteString, NSRange charRangeToReplace);

		[Export ("smartInsertAfterStringForString:replacingRange:")]
		string SmartInsertAfter (string pasteString, NSRange charRangeToReplace);

		[Export ("toggleAutomaticQuoteSubstitution:")]
		void ToggleAutomaticQuoteSubstitution (NSObject sender);

		[Export ("toggleAutomaticLinkDetection:")]
		void ToggleAutomaticLinkDetection (NSObject sender);

		[Export ("toggleAutomaticDataDetection:")]
		void ToggleAutomaticDataDetection (NSObject sender);

		[Export ("toggleAutomaticDashSubstitution:")]
		void ToggleAutomaticDashSubstitution (NSObject sender);

		[Export ("toggleAutomaticTextReplacement:")]
		void ToggleAutomaticTextReplacement (NSObject sender);

		[Export ("toggleAutomaticSpellingCorrection:")]
		void ToggleAutomaticSpellingCorrection (NSObject sender);

		[Export ("checkTextInRange:types:options:")]
		void CheckText (NSRange range, NSTextCheckingTypes checkingTypes, NSDictionary options);

		[Export ("handleTextCheckingResults:forRange:types:options:orthography:wordCount:")]
		void HandleTextChecking (NSTextCheckingResult [] results, NSRange range, NSTextCheckingTypes checkingTypes, NSDictionary options, NSOrthography orthography, nint wordCount);

		[Export ("orderFrontSubstitutionsPanel:")]
		void OrderFrontSubstitutionsPanel (NSObject sender);

		[Export ("checkTextInSelection:")]
		void CheckTextInSelection (NSObject sender);

		[Export ("checkTextInDocument:")]
		void CheckTextInDocument (NSObject sender);

		//Detected properties
		[Export ("smartInsertDeleteEnabled")]
		bool SmartInsertDeleteEnabled { get; set; }

		[Export ("automaticQuoteSubstitutionEnabled")]
		bool AutomaticQuoteSubstitutionEnabled { [Bind ("isAutomaticQuoteSubstitutionEnabled")]get; set; }

		[Export ("automaticLinkDetectionEnabled")]
		bool AutomaticLinkDetectionEnabled { [Bind ("isAutomaticLinkDetectionEnabled")]get; set; }

		[Export ("automaticDataDetectionEnabled")]
		bool AutomaticDataDetectionEnabled { [Bind ("isAutomaticDataDetectionEnabled")]get; set; }

		[Export ("automaticDashSubstitutionEnabled")]
		bool AutomaticDashSubstitutionEnabled { [Bind ("isAutomaticDashSubstitutionEnabled")]get; set; }

		[Export ("automaticTextReplacementEnabled")]
		bool AutomaticTextReplacementEnabled { [Bind ("isAutomaticTextReplacementEnabled")]get; set; }

		[Export ("automaticSpellingCorrectionEnabled")]
		bool AutomaticSpellingCorrectionEnabled { [Bind ("isAutomaticSpellingCorrectionEnabled")]get; set; }

		[Export ("enabledTextCheckingTypes")]
		NSTextCheckingTypes EnabledTextCheckingTypes { get; set; }

		[Mac (10,10)]
		[Export ("usesRolloverButtonForSelection")]
		bool UsesRolloverButtonForSelection { get; set; }

		[Mac (10,7)]
		[Export ("toggleQuickLookPreviewPanel:")]
		void ToggleQuickLookPreviewPanel (NSObject sender);

		[Mac (10, 12)]
		[Static]
		[Export ("stronglyReferencesTextStorage")]
		bool StronglyReferencesTextStorage { get; }

		[Mac (10, 12, 1)]
		[Export ("automaticTextCompletionEnabled")]
		bool AutomaticTextCompletionEnabled { [Bind ("isAutomaticTextCompletionEnabled")] get; set; }

		[Mac (10, 12, 2)]
		[Export ("toggleAutomaticTextCompletion:")]
		void ToggleAutomaticTextCompletion ([NullAllowed] NSObject sender);

		[Mac (10, 12, 2)]
		[Export ("allowsCharacterPickerTouchBarItem")]
		bool AllowsCharacterPickerTouchBarItem { get; set; }

		[Mac (10, 12, 2)]
		[Export ("updateTouchBarItemIdentifiers")]
		void UpdateTouchBarItemIdentifiers ();

		[Mac (10, 12, 2)]
		[Export ("updateTextTouchBarItems")]
		void UpdateTextTouchBarItems ();

		[Mac (10, 12, 2)]
		[Export ("updateCandidates")]
		void UpdateCandidates ();

		[Mac (10, 12, 2)]
		[NullAllowed, Export ("candidateListTouchBarItem", ArgumentSemantic.Strong)]
		NSCandidateListTouchBarItem CandidateListTouchBarItem { get; }
	}

	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface NSTextInputClient
	{
		[Export ("insertText:replacementRange:")]
		void InsertText (NSObject text, NSRange replacementRange);

		[Export ("setMarkedText:selectedRange:replacementRange:")]
		void SetMarkedText (NSObject text, NSRange selectedRange, NSRange replacementRange);

		[Export ("unmarkText")]
		void UnmarkText ();

		[Export ("selectedRange")]
		NSRange SelectedRange { get; }

		[Export ("markedRange")]
		NSRange MarkedRange { get; }

		[Export ("hasMarkedText")]
		bool HasMarkedText { get; }

		[Export ("attributedSubstringForProposedRange:actualRange:")]
		NSAttributedString GetAttributedSubstring (NSRange proposedRange, out NSRange actualRange);

		[Export ("validAttributesForMarkedText")]
		NSString [] ValidAttributesForMarkedText { get; }

		[Export ("firstRectForCharacterRange:actualRange:")]
		CGRect GetFirstRect (NSRange characterRange, out NSRange actualRange);

		[Export ("characterIndexForPoint:")]
		nuint GetCharacterIndex (CGPoint point);

		[Export ("attributedString")]
		NSAttributedString AttributedString { get; }

		[Export ("fractionOfDistanceThroughGlyphForPoint:")]
		nfloat GetFractionOfDistanceThroughGlyph (CGPoint point);

		[Export ("baselineDeltaForCharacterAtIndex:")]
		nfloat GetBaselineDelta (nuint charIndex);

		[Export ("windowLevel")]
		NSWindowLevel WindowLevel { get; }

		[Export ("drawsVerticallyForCharacterAtIndex:")]
		bool DrawsVertically (nuint charIndex);
	}

	[BaseType (typeof (NSTextDelegate))]
	[Model]
	[Protocol]
	partial interface NSTextViewDelegate {
		[Export ("textView:clickedOnLink:atIndex:"), DelegateName ("NSTextViewLink"), DefaultValue (false)]
		bool LinkClicked (NSTextView textView, NSObject link, nuint charIndex);

		[Export ("textView:clickedOnCell:inRect:atIndex:"), EventArgs ("NSTextViewClicked")]
		void CellClicked (NSTextView textView, NSTextAttachmentCell cell, CGRect cellFrame, nuint charIndex);

		[Export ("textView:doubleClickedOnCell:inRect:atIndex:"), EventArgs ("NSTextViewDoubleClick")]
		void CellDoubleClicked (NSTextView textView, NSTextAttachmentCell cell, CGRect cellFrame, nuint charIndex);

		// 
		[Export ("textView:writablePasteboardTypesForCell:atIndex:"), DelegateName ("NSTextViewCellPosition"),DefaultValue (null)]
		string [] GetWritablePasteboardTypes (NSTextView view, NSTextAttachmentCell forCell, nuint charIndex);

		[Export ("textView:writeCell:atIndex:toPasteboard:type:"), DelegateName ("NSTextViewCellPasteboard"), DefaultValue (true)]
		bool WriteCell (NSTextView view, NSTextAttachmentCell cell, nuint charIndex, NSPasteboard pboard, string type);

		[Export ("textView:willChangeSelectionFromCharacterRange:toCharacterRange:"), DelegateName ("NSTextViewSelectionChange"), DefaultValueFromArgument ("newSelectedCharRange")]
		NSRange WillChangeSelection (NSTextView textView, NSRange oldSelectedCharRange, NSRange newSelectedCharRange);

		[Export ("textView:willChangeSelectionFromCharacterRanges:toCharacterRanges:"), DelegateName ("NSTextViewSelectionWillChange"), DefaultValueFromArgument ("newSelectedCharRanges")]
		NSValue [] WillChangeSelectionFromRanges (NSTextView textView, NSValue [] oldSelectedCharRanges, NSValue [] newSelectedCharRanges);

		[Export ("textView:shouldChangeTextInRanges:replacementStrings:"), DelegateName ("NSTextViewSelectionShouldChange"), DefaultValue (true)]
		bool ShouldChangeTextInRanges (NSTextView textView, NSValue [] affectedRanges, string [] replacementStrings);

		[Export ("textView:shouldChangeTypingAttributes:toAttributes:"), DelegateName ("NSTextViewTypeAttribute"), DefaultValueFromArgument ("newTypingAttributes")]
		NSDictionary ShouldChangeTypingAttributes (NSTextView textView, NSDictionary oldTypingAttributes, NSDictionary newTypingAttributes);

		[Export ("textViewDidChangeSelection:"), EventArgs ("NSTextViewNotification")]
		void DidChangeSelection (NSNotification notification);

		[Export ("textViewDidChangeTypingAttributes:"), EventArgs ("NSTextViewNotification")]
		void DidChangeTypingAttributes (NSNotification notification);

		[Export ("textView:willDisplayToolTip:forCharacterAtIndex:"), DelegateName ("NSTextViewTooltip"), DefaultValueFromArgument ("tooltip")]
		string WillDisplayToolTip (NSTextView textView, string tooltip, nuint characterIndex);

		[Export ("textView:completions:forPartialWordRange:indexOfSelectedItem:"), DelegateName ("NSTextViewCompletion"), DefaultValue (null)]
		string [] GetCompletions (NSTextView textView, string [] words, NSRange charRange, ref nint index);

		[Export ("textView:shouldChangeTextInRange:replacementString:"), DelegateName ("NSTextViewChangeText"), DefaultValue (true)]
		bool ShouldChangeTextInRange (NSTextView textView, NSRange affectedCharRange, string replacementString);

		[Export ("textView:doCommandBySelector:"), DelegateName ("NSTextViewSelectorCommand"), DefaultValue (false)]
		bool DoCommandBySelector (NSTextView textView, Selector commandSelector);

		[Export ("textView:shouldSetSpellingState:range:"), DelegateName ("NSTextViewSpellingQuery"), DefaultValue (0)]
		nint ShouldSetSpellingState (NSTextView textView, nint value, NSRange affectedCharRange);

		[Export ("textView:menu:forEvent:atIndex:"), DelegateName ("NSTextViewEventMenu"), DefaultValueFromArgument ("menu")]
		NSMenu MenuForEvent (NSTextView view, NSMenu menu, NSEvent theEvent, nuint charIndex);

		[Export ("textView:willCheckTextInRange:options:types:"), DelegateName ("NSTextViewOnTextCheck"), DefaultValueFromArgument ("options")]
		NSDictionary WillCheckText (NSTextView view, NSRange range, NSDictionary options, NSTextCheckingTypes checkingTypes);

		[Export ("textView:didCheckTextInRange:types:options:results:orthography:wordCount:"), DelegateName ("NSTextViewTextChecked"), DefaultValueFromArgument ("results")]
		NSTextCheckingResult [] DidCheckText (NSTextView view, NSRange range, NSTextCheckingTypes checkingTypes, NSDictionary options, NSTextCheckingResult [] results, NSOrthography orthography, nint wordCount);

#if !XAMCORE_4_0
		[Export ("textView:draggedCell:inRect:event:"), EventArgs ("NSTextViewDraggedCell")]
		void DraggedCell (NSTextView view, NSTextAttachmentCell cell, CGRect rect, NSEvent theevent);
#else
		[Export ("textView:draggedCell:inRect:event:"), EventArgs ("NSTextViewDraggedCell")]
		void DraggedCell (NSTextView view, NSTextAttachmentCell cell, CGRect rect, NSEvent theEvent);
#endif

		[Export ("undoManagerForTextView:"), DelegateName ("NSTextViewGetUndoManager"), DefaultValue (null)]
		NSUndoManager GetUndoManager (NSTextView view);

		[Mac (10,12,2)]
		[Export ("textView:shouldUpdateTouchBarItemIdentifiers:"), DelegateName ("NSTextViewUpdateTouchBarItemIdentifiers"), NoDefaultValue]
		string[] ShouldUpdateTouchBarItemIdentifiers (NSTextView textView, string[] identifiers);

		[Mac (10,12,2)]
		[Export ("textView:candidatesForSelectedRange:"), DelegateName ("NSTextViewGetCandidates"), NoDefaultValue]
		[return: NullAllowed]
		NSObject[] GetCandidates (NSTextView textView, NSRange selectedRange);

		[Mac (10,12,2)]
		[Export ("textView:candidates:forSelectedRange:"), DelegateName ("NSTextViewTextCheckingResults"), NoDefaultValue]
		NSTextCheckingResult[] GetTextCheckingCandidates (NSTextView textView, NSTextCheckingResult[] candidates, NSRange selectedRange);

		[Mac (10,12,2)]
		[Export ("textView:shouldSelectCandidateAtIndex:"), DelegateName ("NSTextViewSelectCandidate"), NoDefaultValue]
		bool ShouldSelectCandidates (NSTextView textView, nuint index);

	}
	
	
	[BaseType (typeof (NSTextField))]
	interface NSTokenField {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("tokenStyle")]
		NSTokenStyle TokenStyle { get; set; }

		[Export ("completionDelay")]
		double CompletionDelay { get; set; }

		[Static]
		[Export ("defaultCompletionDelay")]
		double DefaultCompletionDelay { get; }

		[Static]
		[Export ("defaultTokenizingCharacterSet")]
		NSCharacterSet DefaultCharacterSet { get; }

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSTokenFieldDelegate Delegate { get; set; }

		[Export ("tokenizingCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet CharacterSet { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSTokenFieldDelegate {
		[Export ("tokenField:completionsForSubstring:indexOfToken:indexOfSelectedItem:")]
		string [] GetCompletionStrings (NSTokenField tokenField, string substring, nint tokenIndex, nint selectedIndex);

		[Export ("tokenField:shouldAddObjects:atIndex:")]
		NSArray ShouldAddObjects (NSTokenField tokenField, NSArray tokens, nuint index);

		[Export ("tokenField:displayStringForRepresentedObject:")]
		string GetDisplayString (NSTokenField tokenField, NSObject representedObject);

		[Export ("tokenField:editingStringForRepresentedObject:")]
		string GetEditingString (NSTokenField tokenField, NSObject representedObject);

		[Export ("tokenField:representedObjectForEditingString:")]
		[return: NullAllowed]
		NSObject GetRepresentedObject (NSTokenField tokenField, string editingString);

		[Export ("tokenField:writeRepresentedObjects:toPasteboard:")]
		bool WriteRepresented (NSTokenField tokenField, NSArray objects, NSPasteboard pboard);

		[Export ("tokenField:readFromPasteboard:")]
		NSObject [] Read (NSTokenField tokenField, NSPasteboard pboard);

		[Export ("tokenField:menuForRepresentedObject:")]
		NSMenu GetMenu (NSTokenField tokenField, NSObject representedObject);

		[Export ("tokenField:hasMenuForRepresentedObject:")]
		bool HasMenu (NSTokenField tokenField, NSObject representedObject);

		[Export ("tokenField:styleForRepresentedObject:")]
		NSTokenStyle GetStyle (NSTokenField tokenField, NSObject representedObject);

	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSToolbarDelegate)})]
#if XAMCORE_2_0
	[DisableDefaultCtor] // init was added in 10.13
#endif
	partial interface NSToolbar {
#if XAMCORE_2_0
		[Mac (10, 13)]
		[Export ("init")]
		IntPtr Constructor ();
#endif
		[DesignatedInitializer]
		[Export ("initWithIdentifier:")]
		IntPtr Constructor (string identifier);

		[Export ("insertItemWithItemIdentifier:atIndex:")]
		void InsertItem (string itemIdentifier, nint index);

		[Export ("removeItemAtIndex:")]
		void RemoveItem (nint index);

		[Export ("runCustomizationPalette:")]
		void RunCustomizationPalette (NSObject sender);

		[Export ("customizationPaletteIsRunning")]
		bool IsCustomizationPaletteRunning { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("items")]
		NSToolbarItem [] Items { get; }

		[Export ("visibleItems")]
		NSToolbarItem [] VisibleItems { get; }

		[Export ("setConfigurationFromDictionary:")]
		void SetConfigurationFromDictionary (NSDictionary configDict);

		[Export ("configurationDictionary")]
		NSDictionary ConfigurationDictionary { get; }

		[Export ("validateVisibleItems")]
		void ValidateVisibleItems ();

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSToolbarDelegate Delegate { get; set; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")]get; set; }

		[Export ("displayMode")]
		NSToolbarDisplayMode DisplayMode { get; set; }

		[Export ("selectedItemIdentifier"), NullAllowed]
		string SelectedItemIdentifier { get; set; }

		[Export ("sizeMode")]
		NSToolbarSizeMode SizeMode { get; set; }

		[Export ("showsBaselineSeparator")]
		bool ShowsBaselineSeparator { get; set; }

		[Export ("allowsUserCustomization")]
		bool AllowsUserCustomization { get; set; }

		[Export ("autosavesConfiguration")]
		bool AutosavesConfiguration { get; set; }

		[Field ("NSToolbarSeparatorItemIdentifier")]
		NSString NSToolbarSeparatorItemIdentifier { get; }
		
		[Field ("NSToolbarSpaceItemIdentifier")]
		NSString NSToolbarSpaceItemIdentifier { get; }
		
		[Field ("NSToolbarFlexibleSpaceItemIdentifier")]
		NSString NSToolbarFlexibleSpaceItemIdentifier { get; }
		
		[Field ("NSToolbarShowColorsItemIdentifier")]
		NSString NSToolbarShowColorsItemIdentifier { get; }
		
		[Field ("NSToolbarShowFontsItemIdentifier")]
		NSString NSToolbarShowFontsItemIdentifier { get; }
		
		[Field ("NSToolbarCustomizeToolbarItemIdentifier")]
		NSString NSToolbarCustomizeToolbarItemIdentifier { get; }
		
		[Field ("NSToolbarPrintItemIdentifier")]
		NSString NSToolbarPrintItemIdentifier { get; }

		[Mac (10,10)]
		[Export ("allowsExtensionItems")]
		bool AllowsExtensionItems { get; set; }

		[Mac (10,11)]
		[Field ("NSToolbarToggleSidebarItemIdentifier")]
		NSString NSToolbarToggleSidebarItemIdentifier { get; }

		[Mac (10,12)]
		[Field ("NSToolbarCloudSharingItemIdentifier")]
		NSString NSToolbarCloudSharingItemIdentifier { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model, Protocol]
	interface NSToolbarDelegate {
		[Export ("toolbar:itemForItemIdentifier:willBeInsertedIntoToolbar:"), DelegateName ("NSToolbarWillInsert"), DefaultValue (null)]
		NSToolbarItem WillInsertItem (NSToolbar toolbar, string itemIdentifier, bool willBeInserted);

		[Export ("toolbarDefaultItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers"), DefaultValue (null)]
		string [] DefaultItemIdentifiers (NSToolbar toolbar);

		[Export ("toolbarAllowedItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers"), DefaultValue (null)]
		string [] AllowedItemIdentifiers (NSToolbar toolbar);

		[Export ("toolbarSelectableItemIdentifiers:"), DelegateName ("NSToolbarIdentifiers"), DefaultValue (null)]
		string [] SelectableItemIdentifiers (NSToolbar toolbar);

		[Export ("toolbarWillAddItem:"), EventArgs ("NSNotification")]
		void WillAddItem (NSNotification notification);

		[Export ("toolbarDidRemoveItem:"), EventArgs ("NSNotification")]
		void DidRemoveItem (NSNotification notification);
	}

	[BaseType (typeof (NSObject))]
	interface NSToolbarItem : NSCopying {
		[DesignatedInitializer]
		[Export ("initWithItemIdentifier:")]
		IntPtr Constructor (string itemIdentifier);

		[Export ("itemIdentifier")]
		string Identifier { get; }

		[Export ("toolbar")]
		NSToolbar Toolbar { get; }

		[Export ("validate")]
		void Validate ();

		[Export ("allowsDuplicatesInToolbar")]
		bool AllowsDuplicatesInToolbar { get; }

		//Detected properties
		[Export ("label")]
		string Label { get; set; }

		[Export ("paletteLabel")]
		string PaletteLabel { get; set; }

		[Export ("toolTip")]
		string ToolTip { get; set; }

		[Export ("menuFormRepresentation", ArgumentSemantic.Retain)]
		NSMenuItem MenuFormRepresentation { get; set; }

		[Export ("tag")]
		nint Tag { get; set; }

		[Export ("target", ArgumentSemantic.Weak), NullAllowed]
		NSObject Target { get; set; }

		[Export ("action"), NullAllowed]
		Selector Action { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")]get; set; }

		[Export ("image", ArgumentSemantic.Retain), NullAllowed]
		NSImage Image { get; set; }

		[Export ("view", ArgumentSemantic.Retain)]
		NSView View { get; set; }

		[Export ("minSize")]
		CGSize MinSize { get; set; }

		[Export ("maxSize")]
		CGSize MaxSize { get; set; }

		[Export ("visibilityPriority")]
		nint VisibilityPriority { get; set; }

		[Export ("autovalidates")]
		bool Autovalidates { get; set; }
	}

	[BaseType (typeof (NSToolbarItem))]
	interface NSToolbarItemGroup
	{
		[Export ("initWithItemIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string itemIdentifier);

		[Export ("subitems", ArgumentSemantic.Copy)]
		NSToolbarItem[] Subitems { get; set; }
	}

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface NSTouch : NSCopying {
		[Export ("identity", ArgumentSemantic.Retain)]
		NSObject Identity { get; }

		[Export ("phase")]
		NSTouchPhase Phase { get; }

		[Export ("normalizedPosition")]
		CGPoint NormalizedPosition { get; }

		[Export ("isResting")]
		bool IsResting { get; }

		[Export ("device", ArgumentSemantic.Retain)]
		NSObject Device { get; }

		[Export ("deviceSize")]
		CGSize DeviceSize { get; }
	}

	[Mac (10, 12, 2)]
	[Category]
	[BaseType (typeof(NSTouch))]
	interface NSTouch_NSTouchBar
	{
		[Export ("type")]
		NSTouchType GetTouchType ();

		[Export ("locationInView:")]
		CGPoint GetLocation ([NullAllowed] NSView view);

		[Export ("previousLocationInView:")]
		CGPoint GetPreviousLocation ([NullAllowed] NSView view);
	}

	[DesignatedDefaultCtor]
	[Mac (10,12,2)]
	[BaseType (typeof(NSObject), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSTouchBarDelegate)})]
	interface NSTouchBar : NSCoding
	{
		[NullAllowed, Export ("customizationIdentifier")]
		string CustomizationIdentifier { get; set; }

		[Export ("customizationAllowedItemIdentifiers", ArgumentSemantic.Copy)]
		string[] CustomizationAllowedItemIdentifiers { get; set; }

		[Export ("customizationRequiredItemIdentifiers", ArgumentSemantic.Copy)]
		string[] CustomizationRequiredItemIdentifiers { get; set; }

		[Export ("defaultItemIdentifiers", ArgumentSemantic.Copy)]
		string[] DefaultItemIdentifiers { get; set; }

		[NullAllowed, Export ("principalItemIdentifier")]
		string PrincipalItemIdentifier { get; set; }

		[Export ("templateItems", ArgumentSemantic.Copy)]
		NSSet<NSTouchBarItem> TemplateItems { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INSTouchBarDelegate Delegate { get; set; }

		[Export ("itemForIdentifier:")]
		NSTouchBarItem GetItemForIdentifier (string identifier);

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }

		[NullAllowed, Export ("escapeKeyReplacementItemIdentifier")]
		string EscapeKeyReplacementItemIdentifier { get; set; }
	}

	interface INSTouchBarDelegate { }

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSTouchBarDelegate
	{
		[Export ("touchBar:makeItemForIdentifier:"), DelegateName ("NSTouchBarMakeItem"), DefaultValue (null)]
		[return: NullAllowed]
		NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSTouchBarItem : NSCoding
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Wrap ("this (identifier.GetConstant ())")]
		IntPtr Constructor (NSTouchBarItemIdentifier identifier);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("visibilityPriority")]
		float VisibilityPriority { get; set; }

		[NullAllowed, Export ("view")]
		NSView View { get; }

		[NullAllowed, Export ("viewController")]
		NSViewController ViewController { get; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; }

		[Export ("visible")]
		bool Visible { [Bind ("isVisible")] get; }
	}

	[Mac (10,12,2)]
	public enum NSTouchBarItemIdentifier
	{
		[Field ("NSTouchBarItemIdentifierFixedSpaceSmall")]
		FixedSpaceSmall,

		[Field ("NSTouchBarItemIdentifierFixedSpaceLarge")]
		FixedSpaceLarge,

		[Field ("NSTouchBarItemIdentifierFlexibleSpace")]
		FlexibleSpace,

		[Field ("NSTouchBarItemIdentifierOtherItemsProxy")]
		OtherItemsProxy,

		[Field ("NSTouchBarItemIdentifierCharacterPicker")]
		CharacterPicker,

		[Field ("NSTouchBarItemIdentifierTextColorPicker")]
		TextColorPicker,

		[Field ("NSTouchBarItemIdentifierTextStyle")]
		TextStyle,

		[Field ("NSTouchBarItemIdentifierTextAlignment")]
		TextAlignment,

		[Field ("NSTouchBarItemIdentifierTextList")]
		TextList,

		[Field ("NSTouchBarItemIdentifierTextFormat")]
		TextFormat,

		[Field ("NSTouchBarItemIdentifierCandidateList")]
		CandidateList
	}

	[Mac (10, 12, 2)]
	[Protocol]
	interface NSTouchBarProvider
	{
		[Abstract]
		[NullAllowed, Export ("touchBar", ArgumentSemantic.Strong)]
		NSTouchBar TouchBar { get; }
	}

	interface INSTouchBarProvider { }

	[BaseType (typeof (NSObject))]
	interface NSTrackingArea : NSCoding, NSCopying {
		[Export ("initWithRect:options:owner:userInfo:")]
		IntPtr Constructor (CGRect rect, NSTrackingAreaOptions options, NSObject owner, [NullAllowed] NSDictionary userInfo);
		
		[Export ("rect")]
		CGRect Rect { get; }

		[Export ("options")]
		NSTrackingAreaOptions Options { get; }

		[Export ("owner")]
		NSObject Owner { get; }

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface NSTreeNode {
		[Static, Export ("treeNodeWithRepresentedObject:")]
		NSTreeNode FromRepresentedObject (NSObject modelObject);

		[Export ("initWithRepresentedObject:")]
		IntPtr Constructor (NSObject modelObject);

		[Export ("representedObject")]
		NSObject RepresentedObject { get; }

		[Export ("indexPath")]
		NSIndexPath IndexPath { get; }

		[Export ("isLeaf")]
		bool IsLeaf { get; }

		[Export ("childNodes")]
		NSTreeNode [] Children { get; }

		//[Export ("mutableChildNodes")]
		//NSMutableArray MutableChildren { get; }

		[Export ("descendantNodeAtIndexPath:")]
		NSTreeNode DescendantNode (NSIndexPath atIndexPath);

		[Export ("parentNode")]
		NSTreeNode ParentNode { get; }

		[Export ("sortWithSortDescriptors:recursively:")]
		void SortWithSortDescriptors (NSSortDescriptor [] sortDescriptors, bool recursively);

	}

	[BaseType (typeof (NSObjectController))]
	interface NSTreeController {
		[Export ("rearrangeObjects")]
		void RearrangeObjects ();

		[Export ("arrangedObjects")]
#if XAMCORE_4_0
		NSTreeNode ArrangedObjects { get; }
#else
		NSObject ArrangedObjects { get; }
#endif

		[Export ("childrenKeyPath")]
		string ChildrenKeyPath { get; set; }

		[Export ("countKeyPath")]
		string CountKeyPath { get; set; }

		[Export ("leafKeyPath")]
		string LeafKeyPath { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; set; }

		[Export ("content", ArgumentSemantic.Retain)]
		NSObject Content { get; set; }

		[Export ("add:")]
		void Add (NSObject sender);

		[Export ("remove:")]
		void Remove (NSObject sender);

		[Export ("addChild:")]
		void AddChild (NSObject sender);

		[Export ("insert:")]
		void Insert (NSObject sender);

		[Export ("insertChild:")]
		void InsertChild (NSObject sender);

		[Export ("canInsert")]
		bool CanInsert { get; }

		[Export ("canInsertChild")]
		bool CanInsertChild { get; }

		[Export ("canAddChild")]
		bool CanAddChild { get; }

		[Export ("insertObject:atArrangedObjectIndexPath:")]
		void InsertObject (NSObject object1, NSIndexPath indexPath);

		[Export ("insertObjects:atArrangedObjectIndexPaths:")]
		void InsertObjects (NSObject [] objects, NSArray indexPaths);

		[Export ("removeObjectAtArrangedObjectIndexPath:")]
		void RemoveObjectAtArrangedObjectIndexPath (NSIndexPath indexPath);

		[Export ("removeObjectsAtArrangedObjectIndexPaths:")]
		void RemoveObjectsAtArrangedObjectIndexPaths (NSIndexPath [] indexPaths);

		[Export ("avoidsEmptySelection")]
		bool AvoidsEmptySelection { get; set; }

		[Export ("preservesSelection")]
		bool PreservesSelection { get; set; }

		[Export ("selectsInsertedObjects")]
		bool SelectsInsertedObjects { get; set; }

		[Export ("alwaysUsesMultipleValuesMarker")]
		bool AlwaysUsesMultipleValuesMarker { get; set; }

		[Export ("selectedObjects")]
		NSObject [] SelectedObjects { get; }

		[Export ("selectionIndexPaths"), Protected]
		NSIndexPath [] GetSelectionIndexPaths ();

		[Export ("setSelectionIndexPaths:"), Protected]
		bool SetSelectionIndexPaths (NSIndexPath [] indexPaths);

		[Export ("selectionIndexPath"), Protected]
		NSIndexPath GetSelectionIndexPath ();

		[Export ("setSelectionIndexPath:"), Protected]
		bool SetSelectionIndexPath (NSIndexPath index);

		[Export ("addSelectionIndexPaths:")]
		bool AddSelectionIndexPaths (NSIndexPath [] indexPaths);

		[Export ("removeSelectionIndexPaths:")]
		bool RemoveSelectionIndexPaths (NSIndexPath [] indexPaths);

		[Export ("selectedNodes")]
		NSTreeNode [] SelectedNodes { get; }

		[Export ("moveNode:toIndexPath:")]
		void MoveNode (NSTreeNode node, NSIndexPath indexPath);

		[Export ("moveNodes:toIndexPath:")]
		void MoveNodes (NSTreeNode [] nodes, NSIndexPath startingIndexPath);

		[Export ("childrenKeyPathForNode:")]
		string ChildrenKeyPathForNode (NSTreeNode node);

		[Export ("countKeyPathForNode:")]
		string CountKeyPathForNode (NSTreeNode node);

		[Export ("leafKeyPathForNode:")]
		string LeafKeyPathForNode (NSTreeNode node);
	}

	[BaseType (typeof (NSObject))]
	partial interface NSTypesetter {

	}

	delegate void NSWindowTrackEventsMatchingCompletionHandler (NSEvent evt, ref bool stop);
	
	[BaseType (typeof (NSResponder), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSWindowDelegate)})]
	[DisableDefaultCtor]
	partial interface NSWindow : NSAnimatablePropertyContainer, NSUserInterfaceItemIdentification, NSAppearanceCustomization, NSAccessibilityElementProtocol, NSAccessibility {
		[Static, Export ("frameRectForContentRect:styleMask:")]
		CGRect FrameRectFor (CGRect contectRect, NSWindowStyle styleMask);
	
		[Static]
		[Export ("contentRectForFrameRect:styleMask:")]
		CGRect ContentRectFor (CGRect forFrameRect, NSWindowStyle styleMask);
	
		[Static]
		[Export ("minFrameWidthWithTitle:styleMask:")]
		nfloat MinFrameWidthWithTitle (string aTitle, NSWindowStyle aStyle);
	
		[Static]
		[Export ("defaultDepthLimit")]
		NSWindowDepth DefaultDepthLimit { get; }
	
		[Export ("frameRectForContentRect:")]
		CGRect FrameRectFor (CGRect contentRect);
	
		[Export ("contentRectForFrameRect:")]
		CGRect ContentRectFor (CGRect frameRect);

		[Export ("init")]
		[PostSnippet ("if (!DisableReleasedWhenClosedInConstructor) { ReleasedWhenClosed = false; }")]
		IntPtr Constructor ();

		[DesignatedInitializer]
		[Export ("initWithContentRect:styleMask:backing:defer:")]
		[PostSnippet ("if (!DisableReleasedWhenClosedInConstructor) { ReleasedWhenClosed = false; }")]
		IntPtr Constructor (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation);
	
		[Export ("initWithContentRect:styleMask:backing:defer:screen:")]
		[PostSnippet ("if (!DisableReleasedWhenClosedInConstructor) { ReleasedWhenClosed = false; }")]
		IntPtr Constructor (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation, NSScreen  screen);
	
		[Export ("title")]
		string Title  { get; set; }
	
		[Export ("representedURL", ArgumentSemantic.Copy)]
		NSUrl RepresentedUrl { get; set; }
	
		[Export ("representedFilename")]
		string RepresentedFilename  { get; set; }
	
		[Export ("setTitleWithRepresentedFilename:")]
		void SetTitleWithRepresentedFilename (string  filename);
	
		[Export ("setExcludedFromWindowsMenu:")]
		void SetExcludedFromWindowsMenu (bool flag);
	
		[Export ("isExcludedFromWindowsMenu")]
		bool ExcludedFromWindowsMenu { get; } 
	
		[Export ("contentView", ArgumentSemantic.Retain)]
		NSView ContentView  { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
	
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSWindowDelegate Delegate { get; set; }
	
		[Export ("windowNumber")]
		nint WindowNumber { get; }
	
		[Export ("styleMask")]
		NSWindowStyle StyleMask { get; set; }
	
		[Export ("fieldEditor:forObject:")]
		NSText FieldEditor (bool createFlag, [NullAllowed] NSObject forObject);
	
		[Export ("endEditingFor:")]
		void EndEditingFor ([NullAllowed] NSObject anObject);
	
		[Export ("constrainFrameRect:toScreen:")]
		CGRect ConstrainFrameRect (CGRect frameRect, [NullAllowed] NSScreen screen);
	
		[Export ("setFrame:display:")]
		void SetFrame (CGRect frameRect, bool display);
	
		[Export ("setContentSize:")]
		void SetContentSize (CGSize aSize);
	
		[Export ("setFrameOrigin:")]
		void SetFrameOrigin (CGPoint aPoint);
	
		[Export ("setFrameTopLeftPoint:")]
		void SetFrameTopLeftPoint (CGPoint aPoint);
	
		[Export ("cascadeTopLeftFromPoint:")]
		CGPoint CascadeTopLeftFromPoint (CGPoint topLeftPoint);
	
		[Export ("frame")]
		[ThreadSafe] // Bug 22909 - This can be called from a non-ui thread <= OS X 10.9
		CGRect Frame { get; }
	
		[Export ("animationResizeTime:")]
		double AnimationResizeTime (CGRect newFrame);
	
		[Export ("setFrame:display:animate:")]
		void SetFrame (CGRect frameRect, bool display, bool animate);
	
		[Export ("inLiveResize")]
		bool InLiveResize { get; } 
	
		[Export ("showsResizeIndicator")]
		bool ShowsResizeIndicator { get; set; }
	
		[Export ("resizeIncrements")]
		CGSize ResizeIncrements  { get; set; }
	
		[Export ("aspectRatio")]
		CGSize AspectRatio  { get; set; }
	
		[Export ("contentResizeIncrements")]
		CGSize ContentResizeIncrements  { get; set; }
	
		[Export ("contentAspectRatio")]
		CGSize ContentAspectRatio  { get; set; }
	
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("useOptimizedDrawing:")]
		void UseOptimizedDrawing (bool flag);
	
		[Export ("disableFlushWindow")]
		void DisableFlushWindow ();
	
		[Export ("enableFlushWindow")]
		void EnableFlushWindow ();
	
		[Export ("isFlushWindowDisabled")]
		bool FlushWindowDisabled { get; }
	
		[Export ("flushWindow")]
		void FlushWindow ();
	
		[Export ("flushWindowIfNeeded")]
		void FlushWindowIfNeeded ();
	
		[Export ("viewsNeedDisplay")]
		bool ViewsNeedDisplay  { get; set; }
	
		[Export ("displayIfNeeded")]
		void DisplayIfNeeded ();
	
		[Export ("display")]
		void Display ();
	
		[Export ("autodisplay")]
		bool Autodisplay  { [Bind ("isAutodisplay")] get; set; }
	
		[Export ("preservesContentDuringLiveResize")]
		bool PreservesContentDuringLiveResize  { get; set; }
	
		[Export ("update")]
		void Update ();
	
		[Export ("makeFirstResponder:")]
		bool MakeFirstResponder ([NullAllowed] NSResponder  aResponder);
	
		[Export ("firstResponder")]
		NSResponder FirstResponder { get; }
	
		[Export ("resizeFlags")]
		nint ResizeFlags { get; }
	
		[Export ("keyDown:")]
		void KeyDown (NSEvent  theEvent);
	
		/* NSWindow.Close by default calls [window release]
		 * This will cause a double free in our code since we're not aware of this
		 * and we end up GCing the proxy eventually and sending our own release
		 */
		[Internal, Export ("close")]
		void _Close ();
	
		[Export ("releasedWhenClosed")]
		bool ReleasedWhenClosed  { [Bind ("isReleasedWhenClosed")] get; set; }
	
		[Export ("miniaturize:")]
		void Miniaturize ([NullAllowed] NSObject sender);
	
		[Export ("deminiaturize:")]
		void Deminiaturize ([NullAllowed] NSObject sender);
	
		[Export ("isZoomed")]
		bool IsZoomed { get; set; }
	
		[Export ("zoom:")]
		void Zoom ([NullAllowed] NSObject sender);
	
		[Export ("isMiniaturized")]
		bool IsMiniaturized { get; set; }
	
		[Export ("tryToPerform:with:")]
		bool TryToPerform (Selector anAction, NSObject anObject);
		
		[Export ("validRequestorForSendType:returnType:")]
		NSObject ValidRequestorForSendType (string sendType, string returnType);
	
		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor  { get; set; }
	
		[Export ("setContentBorderThickness:forEdge:")]
		void SetContentBorderThickness (nfloat thickness, NSRectEdge edge);
	
		[Export ("contentBorderThicknessForEdge:")]
		nfloat ContentBorderThicknessForEdge (NSRectEdge edge);
	
		[Export ("setAutorecalculatesContentBorderThickness:forEdge:")]
		void SetAutorecalculatesContentBorderThickness (bool flag, NSRectEdge forEdge);
	
		[Export ("autorecalculatesContentBorderThicknessForEdge:")]
		bool AutorecalculatesContentBorderThickness (NSRectEdge forEdgeedge);
	
		[Export ("movable")]
		bool IsMovable  { [Bind ("isMovable")] get; set; }
	
		[Export ("movableByWindowBackground")]
		bool MovableByWindowBackground  { [Bind ("isMovableByWindowBackground")] get; set; }
	
		[Export ("hidesOnDeactivate")]
		bool HidesOnDeactivate  { get; set; }
	
		[Export ("canHide")]
		bool CanHide  { get; set; }
	
		[Export ("center")]
		void Center ();
	
		[Export ("makeKeyAndOrderFront:")]
		void MakeKeyAndOrderFront ([NullAllowed] NSObject sender);
	
		[Export ("orderFront:")]
		void OrderFront ([NullAllowed] NSObject sender);
		
		[Export ("orderBack:")]
		void OrderBack ([NullAllowed] NSObject sender);
	
		[Export ("orderOut:")]
		void OrderOut ([NullAllowed] NSObject sender);
	
		[Export ("orderWindow:relativeTo:")]
		void OrderWindow (NSWindowOrderingMode place, nint relativeTo);
	
		[Export ("orderFrontRegardless")]
		void OrderFrontRegardless ();
	
		[Export ("miniwindowImage", ArgumentSemantic.Retain)]
		NSImage MiniWindowImage { get; set; }
	
		[Export ("miniwindowTitle")]
		string MiniWindowTitle  { get; set; }
	
		[Export ("dockTile")]
		NSDockTile DockTile { get; } 
	
		[Export ("documentEdited")]
		bool DocumentEdited  { [Bind ("isDocumentEdited")] get; set; }
	
		[Export ("isVisible")]
		bool IsVisible  { get; set; }
	
		[Export ("isKeyWindow")]
		bool IsKeyWindow { get; }
	
		[Export ("isMainWindow")]
		bool IsMainWindow { get; }
		
		[Export ("canBecomeKeyWindow")]
		bool CanBecomeKeyWindow { get; }
		
		[Export ("canBecomeMainWindow")]
		bool CanBecomeMainWindow { get; }
	
		[Export ("makeKeyWindow")]
		void MakeKeyWindow ();
	
		[Export ("makeMainWindow")]
		void MakeMainWindow ();
	
		[Export ("becomeKeyWindow")]
		void BecomeKeyWindow ();
		
		[Export ("resignKeyWindow")]
		void ResignKeyWindow ();
		
		[Export ("becomeMainWindow")]
		void BecomeMainWindow ();
	
		[Export ("resignMainWindow")]
		void ResignMainWindow ();
		
		[Export ("worksWhenModal")]
		bool WorksWhenModal ();
		
		[Export ("preventsApplicationTerminationWhenModal")]
		bool PreventsApplicationTerminationWhenModal  { get; set; }
	
		[Availability (Deprecated = Platform.Mac_10_7, Message = "Use ConvertRectToScreen instead.")]
		[Export ("convertBaseToScreen:")]
		CGPoint ConvertBaseToScreen (CGPoint aPoint);
	
		[Availability (Deprecated = Platform.Mac_10_7, Message = "Use ConvertRectFromScreen instead.")]
		[Export ("convertScreenToBase:")]
		CGPoint ConvertScreenToBase (CGPoint aPoint);
	
		[Export ("performClose:")]
		void PerformClose ([NullAllowed] NSObject sender);
		
		[Export ("performMiniaturize:")]
		void PerformMiniaturize ([NullAllowed] NSObject sender);
	
		[Export ("performZoom:")]
		void PerformZoom ([NullAllowed] NSObject sender);
	
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("gState")]
		nint GState ();
	
		[Export ("setOneShot:")]
		void SetOneShot (bool flag);
	
		[Export ("isOneShot")]
		bool IsOneShot { get; }
	
		[Export ("dataWithEPSInsideRect:")]
		NSData DataWithEpsInsideRect (CGRect rect);
	
		[Export ("dataWithPDFInsideRect:")]
		NSData DataWithPdfInsideRect (CGRect rect);
	
		[Export ("print:")]
		void Print ([NullAllowed] NSObject sender);
	
		[Export ("disableCursorRects")]
		void DisableCursorRects ();
	
		[Export ("enableCursorRects")]
		void EnableCursorRects ();
	
		[Export ("discardCursorRects")]
		void DiscardCursorRects ();
	
		[Export ("areCursorRectsEnabled")]
		bool AreCursorRectsEnabled { get; }
	
		[Export ("invalidateCursorRectsForView:")]
		void InvalidateCursorRectsForView (NSView  aView);
	
		[Export ("resetCursorRects")]
		void ResetCursorRects ();
	
		[Export ("allowsToolTipsWhenApplicationIsInactive")]
		bool AllowsToolTipsWhenApplicationIsInactive  { get; set; }
	
		[Export ("backingType")]
		NSBackingStore BackingType  { get; set; }
	
		[Export ("level")]
		NSWindowLevel Level  { get; set; }
	
		[Export ("depthLimit")]
		NSWindowDepth DepthLimit  { get; set; }
	
		[Export ("dynamicDepthLimit")]
		bool HasDynamicDepthLimit { [Bind ("hasDynamicDepthLimit")] get; set; }
	
		[Export ("screen")]
		NSScreen Screen { get; }
	
		[Export ("deepestScreen")]
		NSScreen DeepestScreen { get; }
	
		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("canStoreColor")]
		bool CanStoreColor { get; }
	
		[Export ("hasShadow")]
		bool HasShadow  { get; set; }
	
		[Export ("invalidateShadow")]
		void InvalidateShadow ();
	
		[Export ("alphaValue")]
		nfloat AlphaValue  { get; set; }
	
		[Export ("opaque")]
		bool IsOpaque  { [Bind ("isOpaque")]get; set; }
	
		[Export ("sharingType")]
		NSWindowSharingType SharingType  { get; set; }
	
		[Export ("preferredBackingLocation")]
		NSWindowBackingLocation PreferredBackingLocation  { get; set; }
	
		[Export ("backingLocation")]
		NSWindowBackingLocation BackingLocation { get; }
	
		[Export ("allowsConcurrentViewDrawing")]
		bool AllowsConcurrentViewDrawing  { get; set; }
	
		[Export ("displaysWhenScreenProfileChanges")]
		bool DisplaysWhenScreenProfileChanges  { get; set; }
	
		[Export ("disableScreenUpdatesUntilFlush")]
		void DisableScreenUpdatesUntilFlush ();
	
		[Export ("canBecomeVisibleWithoutLogin")]
		bool CanBecomeVisibleWithoutLogin { get; set; }
	
		[Export ("collectionBehavior")]
		NSWindowCollectionBehavior CollectionBehavior  { get; set; }
	
		[Export ("isOnActiveSpace")]
		bool IsOnActiveSpace { get; }
	
		[Export ("stringWithSavedFrame")]
		string StringWithSavedFrame ();
	
		[Export ("setFrameFromString:")]
		void SetFrameFrom (string str);
	
		[Export ("saveFrameUsingName:")]
		void SaveFrameUsingName (string  name);
	
		[Export ("setFrameUsingName:force:")]
		bool SetFrameUsingName (string  name, bool force);
	
		[Export ("setFrameUsingName:")]
		bool SetFrameUsingName (string  name);
	
		[Export ("frameAutosaveName"), Protected]
		string GetFrameAutosaveName ();

		[Export ("setFrameAutosaveName:"), Protected]
		bool SetFrameAutosaveName (string frameName);

		[Static]
		[Export ("removeFrameUsingName:")]
		void RemoveFrameUsingName (string  name);
	
		[Export ("cacheImageInRect:")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "This method shouldn’t be used as it doesn’t work in all drawing situations; instead, a subview should be used that implements the desired drawing behavior.")]
		void CacheImageInRect (CGRect aRect);
	
		[Export ("restoreCachedImage")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "This method shouldn’t be used as it doesn’t work in all drawing situations; instead, a subview should be used that implements the desired drawing behavior.")]
		void RestoreCachedImage ();
	
		[Export ("discardCachedImage")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "This method shouldn’t be used as it doesn’t work in all drawing situations; instead, a subview should be used that implements the desired drawing behavior.")]
		void DiscardCachedImage ();
	
		[Export ("minSize")]
		CGSize MinSize  { get; set; }
	
		[Export ("maxSize")]
		CGSize MaxSize  { get; set; }
	
		[Export ("contentMinSize")]
		CGSize ContentMinSize  { get; set; }
	
		[Export ("contentMaxSize")]
		CGSize ContentMaxSize  { get; set; }
	
		[Export ("nextEventMatchingMask:"), Protected]
		NSEvent NextEventMatchingMask (nuint mask);

		[Export ("nextEventMatchingMask:untilDate:inMode:dequeue:"), Protected]
		NSEvent NextEventMatchingMask (nuint mask, NSDate  expiration, string  mode, bool deqFlag);
	
		[Export ("discardEventsMatchingMask:beforeEvent:"), Protected]
		void DiscardEventsMatchingMask (nuint mask, NSEvent beforeLastEvent);

		[Export ("postEvent:atStart:")]
		void PostEvent (NSEvent theEvent, bool atStart);
	
		[Export ("currentEvent")]
		NSEvent CurrentEvent ();
	
		[Export ("acceptsMouseMovedEvents")]
		bool AcceptsMouseMovedEvents  { get; set; }
	
		[Export ("ignoresMouseEvents")]
		bool IgnoresMouseEvents  { get; set; }
	
		[Export ("deviceDescription")]
		NSDictionary DeviceDescription { get; }
	
		[Export ("sendEvent:")]
		void SendEvent (NSEvent theEvent);
	
		[Export ("mouseLocationOutsideOfEventStream")]
		CGPoint MouseLocationOutsideOfEventStream { get; }
	
		[Availability (Deprecated = Platform.Mac_10_11, Message = "This method does not do anything and should not be called.")]
		[Static]
		[Export ("menuChanged:")]
		void MenuChanged (NSMenu  menu);
	
		[Export ("windowController")]
		NSObject WindowController { get; [NullAllowed] set; }
	
		[Export ("isSheet")]
		bool IsSheet { get; }
	
		[Export ("attachedSheet")]
		NSWindow AttachedSheet { get; }

		[Static]
		[Export ("standardWindowButton:forStyleMask:")]
		NSButton StandardWindowButton (NSWindowButton b, NSWindowStyle styleMask);
	
		[Export ("standardWindowButton:")]
		NSButton StandardWindowButton (NSWindowButton b);
	
		[Export ("addChildWindow:ordered:")][PostGet ("ChildWindows")]
		void AddChildWindow (NSWindow  childWin, NSWindowOrderingMode place);
	
		[Export ("removeChildWindow:")][PostGet ("ChildWindows")]
		void RemoveChildWindow (NSWindow  childWin);
	
		[Export ("childWindows")]
		NSWindow [] ChildWindows { get; }
	
		[Export ("parentWindow")]
		NSWindow ParentWindow { get; set; }
	
		[Export ("graphicsContext")]
		NSGraphicsContext GraphicsContext { get; }
	
		[Availability (Deprecated = Platform.Mac_10_7)]
		[Export ("userSpaceScaleFactor")]
		nfloat UserSpaceScaleFactor { get; }
	
		[Export ("colorSpace", ArgumentSemantic.Retain)]
		NSColorSpace ColorSpace  { get; set; }
	
		[Static]
		[Export ("windowNumbersWithOptions:")]
		NSArray WindowNumbersWithOptions (NSWindowNumberListOptions options);
	
		[Static]
		[Export ("windowNumberAtPoint:belowWindowWithWindowNumber:")]
		nint WindowNumberAtPoint (CGPoint point, nint windowNumber);
	
		[Export ("initialFirstResponder")]
		NSView InitialFirstResponder { get; set; }
	
		[Export ("selectNextKeyView:")]
		void SelectNextKeyView ([NullAllowed] NSObject sender);
	
		[Export ("selectPreviousKeyView:")]
		void SelectPreviousKeyView ([NullAllowed] NSObject sender);
	
		[Export ("selectKeyViewFollowingView:")]
		void SelectKeyViewFollowingView (NSView aView);
	
		[Export ("selectKeyViewPrecedingView:")]
		void SelectKeyViewPrecedingView (NSView aView);
	
		[Export ("keyViewSelectionDirection")]
		NSSelectionDirection KeyViewSelectionDirection ();
	
		[Export ("defaultButtonCell")]
		NSButtonCell DefaultButtonCell { get; [NullAllowed] set; }
	
		[Export ("disableKeyEquivalentForDefaultButtonCell")]
		void DisableKeyEquivalentForDefaultButtonCell ();
	
		[Export ("enableKeyEquivalentForDefaultButtonCell")]
		void EnableKeyEquivalentForDefaultButtonCell ();
	
		[Export ("autorecalculatesKeyViewLoop")]
		bool AutorecalculatesKeyViewLoop  { get; set; }
	
		[Export ("recalculateKeyViewLoop")]
		void RecalculateKeyViewLoop ();
	
		[Export ("toolbar")]
		[NullAllowed]
		NSToolbar Toolbar { get; set; }
	
		[Export ("toggleToolbarShown:")]
		void ToggleToolbarShown (NSObject sender);
	
		[Export ("runToolbarCustomizationPalette:")]
		void RunToolbarCustomizationPalette (NSObject sender);
	
		[Export ("showsToolbarButton")]
		bool ShowsToolbarButton { get; set; }

		[Export ("registerForDraggedTypes:")]
		void RegisterForDraggedTypes (string [] newTypes);
	
		[Export ("unregisterDraggedTypes")]
		void UnregisterDraggedTypes ();
	
		[Export ("windowRef")]
		IntPtr WindowRef { get; }

		// This one comes from the NSUserInterfaceRestoration category ('@interface NSWindow (NSUserInterfaceRestoration)')
		[Mac (10, 7), Export ("disableSnapshotRestoration")]
		void DisableSnapshotRestoration ();

		// This one comes from the NSUserInterfaceRestoration category ('@interface NSWindow (NSUserInterfaceRestoration)')
		[Mac (10, 7), Export ("enableSnapshotRestoration")]
		void EnableSnapshotRestoration ();

		// This one comes from the NSUserInterfaceRestoration category ('@interface NSWindow (NSUserInterfaceRestoration)')
		[Mac (10, 7), Export ("restorable")]
		bool Restorable { [Bind ("isRestorable")]get; set; }

		// This one comes from the NSUserInterfaceRestoration category ('@interface NSWindow (NSUserInterfaceRestoration)')
		[Mac (10, 7), Export ("restorationClass")]
		Class RestorationClass { get; set; }

		//Detected properties
		[Mac (10, 7), Export ("updateConstraintsIfNeeded")]
		void UpdateConstraintsIfNeeded ();

		[Mac (10, 7), Export ("layoutIfNeeded")]
		void LayoutIfNeeded ();

		[Mac (10, 7), Export ("setAnchorAttribute:forOrientation:")]
		void SetAnchorAttribute (NSLayoutAttribute layoutAttribute, NSLayoutConstraintOrientation forOrientation);

		[Mac (10, 7), Export ("visualizeConstraints:")]
		void VisualizeConstraints ([NullAllowed] NSLayoutConstraint [] constraints);

		[Mac (10, 7), Export ("convertRectToScreen:")]
		CGRect ConvertRectToScreen (CGRect aRect);

		[Mac (10, 7), Export ("convertRectFromScreen:")]
		CGRect ConvertRectFromScreen (CGRect aRect);

		[Mac (10, 7), Export ("convertRectToBacking:")]
		CGRect ConvertRectToBacking (CGRect aRect);

		[Mac (10, 7), Export ("convertRectFromBacking:")]
		CGRect ConvertRectFromBacking (CGRect aRect);

		[Mac (10, 7), Export ("backingAlignedRect:options:")]
		CGRect BackingAlignedRect (CGRect aRect, NSAlignmentOptions options);

		[Mac (10, 7), Export ("backingScaleFactor")]
		nfloat BackingScaleFactor { get; }

		[Mac (10, 7), Export ("toggleFullScreen:")]
		void ToggleFullScreen ([NullAllowed] NSObject sender);

		//Detected properties
		[Export ("animationBehavior")]
		NSWindowAnimationBehavior AnimationBehavior { get; set; }

#if !XAMARIN_MAC
		//
		// Fields
		//
		[Field ("NSWindowDidBecomeKeyNotification")]
		NSString DidBecomeKeyNotification { get; }

		[Field ("NSWindowDidBecomeMainNotification")]
		NSString DidBecomeMainNotification { get; }

		[Field ("NSWindowDidChangeScreenNotification")]
		NSString DidChangeScreenNotification { get; }

		[Field ("NSWindowDidDeminiaturizeNotification")]
		NSString DidDeminiaturizeNotification { get; }

		[Field ("NSWindowDidExposeNotification")]
		NSString DidExposeNotification { get; }

		[Field ("NSWindowDidMiniaturizeNotification")]
		NSString DidMiniaturizeNotification { get; }

		[Field ("NSWindowDidMoveNotification")]
		NSString DidMoveNotification { get; }

		[Field ("NSWindowDidResignKeyNotification")]
		NSString DidResignKeyNotification { get; }

		[Field ("NSWindowDidResignMainNotification")]
		NSString DidResignMainNotification { get; }

		[Field ("NSWindowDidResizeNotification")]
		NSString DidResizeNotification { get; }

		[Field ("NSWindowDidUpdateNotification")]
		NSString DidUpdateNotification { get; }

		[Field ("NSWindowWillCloseNotification")]
		NSString WillCloseNotification { get; }

		[Field ("NSWindowWillMiniaturizeNotification")]
		NSString WillMiniaturizeNotification { get; }

		[Field ("NSWindowWillMoveNotification")]
		NSString WillMoveNotification { get; }

		[Field ("NSWindowWillBeginSheetNotification")]
		NSString WillBeginSheetNotification { get; }

		[Field ("NSWindowDidEndSheetNotification")]
		NSString DidEndSheetNotification { get; }

		[Field ("NSWindowDidChangeScreenProfileNotification")]
		NSString DidChangeScreenProfileNotification { get; }

		[Field ("NSWindowWillStartLiveResizeNotification")]
		NSString WillStartLiveResizeNotification { get; }

		[Field ("NSWindowDidEndLiveResizeNotification")]
		NSString DidEndLiveResizeNotification { get; }

		[Field ("NSWindowWillEnterFullScreenNotification")]
		NSString WillEnterFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidEnterFullScreenNotification")]
		NSString DidEnterFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillExitFullScreenNotification")]
		NSString WillExitFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidExitFullScreenNotification")]
		NSString DidExitFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillEnterVersionBrowserNotification")]
		NSString WillEnterVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidEnterVersionBrowserNotification")]
		NSString DidEnterVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillExitVersionBrowserNotification")]
		NSString WillExitVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidExitVersionBrowserNotification")]
		NSString DidExitVersionBrowserNotification { get; }
#endif

		// 10.10
		[Mac (10,10)]
		[Export ("titleVisibility")]
		NSWindowTitleVisibility TitleVisibility { get; set; }

		[Mac (10,10)]
		[Export ("titlebarAppearsTransparent")]
		bool TitlebarAppearsTransparent { get; set; }

		[Mac (10,10)]
		[Export ("contentLayoutRect")]
		CGRect ContentLayoutRect { get; }

		[Mac (10,10)]
		[Export ("contentLayoutGuide")]
		NSObject ContentLayoutGuide { get; }

		[Mac (10,10)]
		[Export ("titlebarAccessoryViewControllers", ArgumentSemantic.Copy)]
		// Header says this is a r/w property, but it fails at runtime.
		//  -[NSWindow setTitlebarAccessoryViewControllers:]: unrecognized selector sent to instance 0x6180001e0f00
		NSTitlebarAccessoryViewController [] TitlebarAccessoryViewControllers { get; }

		[Mac (10,10)]
		[Export ("addTitlebarAccessoryViewController:")]
		void AddTitlebarAccessoryViewController (NSTitlebarAccessoryViewController childViewController);

		[Mac (10,10)]
		[Export ("insertTitlebarAccessoryViewController:atIndex:")]
		void InsertTitlebarAccessoryViewController (NSTitlebarAccessoryViewController childViewController, nint index);

		[Mac (10,10)]
		[Export ("removeTitlebarAccessoryViewControllerAtIndex:")]
		void RemoveTitlebarAccessoryViewControllerAtIndex (nint index);

		[Mac (10,10)]
		[Static, Export ("windowWithContentViewController:")]
		NSWindow GetWindowWithContentViewController (NSViewController contentViewController);

		[Mac (10,10)]
		[Export ("contentViewController", ArgumentSemantic.Strong)]
		NSViewController ContentViewController { get; set; }

		[Mac (10,10)]
		[Export ("trackEventsMatchingMask:timeout:mode:handler:")]
		void TrackEventsMatching (NSEventMask mask, double timeout, string mode, NSWindowTrackEventsMatchingCompletionHandler trackingHandler);

		[Mac (10,9)]
		[Export ("sheets", ArgumentSemantic.Copy)]
		NSWindow [] Sheets { get; }

		[Mac (10,9)]
		[Export ("sheetParent", ArgumentSemantic.Retain)]
		NSWindow SheetParent { get; }

		[Mac (10,9)]
		[Export ("occlusionState")]
		NSWindowOcclusionState OcclusionState { get; }

		[Mac (10,9)]
		[Export ("beginSheet:completionHandler:")]
		void BeginSheet (NSWindow sheetWindow, Action<nint> completionHandler);

		[Mac (10,9)]
		[Export ("beginCriticalSheet:completionHandler:")]
		void BeginCriticalSheet (NSWindow sheetWindow, Action<nint> completionHandler);

		[Mac (10,9)]
		[Export ("endSheet:")]
		void EndSheet (NSWindow sheetWindow);

		[Mac (10,9)]
		[Export ("endSheet:returnCode:")]
		void EndSheet (NSWindow sheetWindow, NSModalResponse returnCode);
#if !XAMCORE_4_0
		[Obsolete ("Use the EndSheet(NSWindow,NSModalResponse) overload.")]
		[Mac (10,9)]
		[Wrap ("EndSheet (sheetWindow, (NSModalResponse)(long)returnCode)", IsVirtual = true)]
		void EndSheet (NSWindow sheetWindow, nint returnCode);
#endif
		
		[Mac (10,11)]
		[Export ("minFullScreenContentSize", ArgumentSemantic.Assign)]
		CGSize MinFullScreenContentSize { get; set; }

		[Mac (10,11)]
		[Export ("maxFullScreenContentSize", ArgumentSemantic.Assign)]
		CGSize MaxFullScreenContentSize { get; set; }

		[Mac (10,11)]
		[Export ("performWindowDragWithEvent:")]
		void PerformWindowDrag(NSEvent theEvent);

		[Mac (10,12)]
		[Export ("canRepresentDisplayGamut:")]
		bool CanRepresentDisplayGamut (NSDisplayGamut displayGamut);
	}

	[Mac (10,10)]
	[BaseType (typeof (NSViewController))]
	interface NSTitlebarAccessoryViewController : NSAnimationDelegate, NSAnimatablePropertyContainer {
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("layoutAttribute")]
		NSLayoutAttribute LayoutAttribute { get; set; }

		[Export ("fullScreenMinHeight")]
		nfloat FullScreenMinHeight { get; set; }

		[RequiresSuper]
		[Export ("viewWillAppear")]
		void ViewWillAppear ();

		[RequiresSuper]
		[Export ("viewDidAppear")]
		void ViewDidAppear ();

		[RequiresSuper]
		[Export ("viewDidDisappear")]
		void ViewDidDisappear ();

		[Mac (10,12)]
		[Export ("hidden")]
		bool IsHidden { [Bind ("isHidden")] get; set; }
	}

	[Mac (10,10)]
	[BaseType (typeof (NSView))]
	interface NSVisualEffectView {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("material")]
		NSVisualEffectMaterial Material { get; set; }

		[Export ("interiorBackgroundStyle")]
		NSBackgroundStyle InteriorBackgroundStyle { get; }

		[Export ("blendingMode")]
		NSVisualEffectBlendingMode BlendingMode { get; set; }

		[Export ("state")]
		NSVisualEffectState State { get; set; }

		[Export ("maskImage", ArgumentSemantic.Retain)]
		NSImage MaskImage { get; set; }

		[RequiresSuper]
		[Export ("viewDidMoveToWindow")]
		void ViewDidMove ();

		[RequiresSuper]
		[Export ("viewWillMoveToWindow:")]
		void ViewWillMove (NSWindow newWindow);

		[Mac (10, 12)]
		[Export ("emphasized")]
		bool Emphasized { [Bind ("isEmphasized")] get; set; }
	}
	
	delegate void NSWindowCompletionHandler (NSWindow window, NSError error);
	
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Mac (10, 7)]
	partial interface NSWindowRestoration {
		[Static]
		[Export ("restoreWindowWithIdentifier:state:completionHandler:")]
		void RestoreWindow (string identifier, NSCoder state, NSWindowCompletionHandler onCompletion);

	}

	[BaseType (typeof (NSResponder))]
	interface NSWindowController : NSCoding, NSSeguePerforming {
		[DesignatedInitializer]
		[Export ("initWithWindow:")]
		IntPtr Constructor (NSWindow  window);
	
		[Export ("initWithWindowNibName:")]
		IntPtr Constructor (string  windowNibName);
	
		[Export ("initWithWindowNibName:owner:")]
		IntPtr Constructor (string  windowNibName, NSObject owner);
	
		[Export ("windowNibName")]
		string WindowNibName { get; }
	
		[Export ("windowNibPath")]
		string WindowNibPath { get; }
	
		[Export ("owner")]
		NSObject Owner { get; }
	
		[Export ("windowFrameAutosaveName")]
		string WindowFrameAutosaveName { get; set; }
	
		[Export ("shouldCascadeWindows")]
		bool ShouldCascadeWindows  { get; set; }
	
		[Export ("document")]
		[NullAllowed]
		NSDocument Document { get; set; }
	
		[Export ("setDocumentEdited:")]
		void SetDocumentEdited (bool dirtyFlag);
	
		[Export ("shouldCloseDocument")]
		bool ShouldCloseDocument  { get; set; }
	
		[Export ("window", ArgumentSemantic.Retain)]
		NSWindow Window { get; set; }
	
		[Export ("synchronizeWindowTitleWithDocumentName")]
		void SynchronizeWindowTitleWithDocumentName ();
	
		[Export ("windowTitleForDocumentDisplayName:")]
		string WindowTitleForDocumentDisplayName (string  displayName);
	
		[Export ("close")]
		void Close ();
	
		[Export ("showWindow:")]
		void ShowWindow ([NullAllowed] NSObject sender);
	
		[Export ("isWindowLoaded")]
		bool IsWindowLoaded  { get; }
	
		[Export ("windowWillLoad")]
		void WindowWillLoad ();
	
		[Export ("windowDidLoad")]
		void WindowDidLoad ();
	
		[Export ("loadWindow")]
		void LoadWindow ();

		[Mac (10,10)]
		[Export ("contentViewController", ArgumentSemantic.Retain)]
		NSViewController ContentViewController { get; set; }

		[Mac (10,10)]
		[Export ("storyboard", ArgumentSemantic.Strong)]
		NSStoryboard Storyboard { get; }

		[Mac (10,10)]
		[Export ("dismissController:")]
		void DismissController (NSObject sender);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSWindowDelegate {
		[Export ("windowShouldClose:"), DelegateName ("NSObjectPredicate"), DefaultValue (true)]
		bool WindowShouldClose (NSObject sender);
	
		[Export ("windowWillReturnFieldEditor:toObject:"), DelegateName ("NSWindowClient"), DefaultValue (null)]
		NSObject WillReturnFieldEditor (NSWindow  sender, NSObject client);
	
		[Export ("windowWillResize:toSize:"), DelegateName ("NSWindowResize"), DefaultValueFromArgument ("toFrameSize")]
		CGSize WillResize (NSWindow sender, CGSize toFrameSize);
	
		[Export ("windowWillUseStandardFrame:defaultFrame:"), DelegateName ("NSWindowFrame"), DefaultValueFromArgument ("newFrame")]
		CGRect WillUseStandardFrame (NSWindow window, CGRect newFrame);
	
		[Export ("windowShouldZoom:toFrame:"), DelegateName ("NSWindowFramePredicate"), DefaultValue (true)]
		bool ShouldZoom (NSWindow  window, CGRect newFrame);
	
		[Export ("windowWillReturnUndoManager:"), DelegateName ("NSWindowUndoManager"), DefaultValue (null)]
		NSUndoManager WillReturnUndoManager (NSWindow  window);
	
		[Export ("window:willPositionSheet:usingRect:"), DelegateName ("NSWindowSheetRect"), DefaultValueFromArgument ("usingRect")]
		CGRect WillPositionSheet (NSWindow  window, NSWindow  sheet, CGRect usingRect);
	
		[Export ("window:shouldPopUpDocumentPathMenu:"), DelegateName ("NSWindowMenu"), DefaultValue (true)]
		bool ShouldPopUpDocumentPathMenu (NSWindow  window, NSMenu  menu);
	
		[Export ("window:shouldDragDocumentWithEvent:from:withPasteboard:"), DelegateName ("NSWindowDocumentDrag"), DefaultValue (true)]
		bool ShouldDragDocumentWithEvent (NSWindow  window, NSEvent theEvent, CGPoint dragImageLocation, NSPasteboard  withPasteboard);
	
		[Export ("windowDidResize:"), EventArgs ("NSNotification")]
		void DidResize (NSNotification  notification);
	
		[Export ("windowDidExpose:"), EventArgs ("NSNotification")]
		void DidExpose (NSNotification  notification);
	
		[Export ("windowWillMove:"), EventArgs ("NSNotification")]
		void WillMove (NSNotification  notification);
	
		[Export ("windowDidMove:"), EventArgs ("NSNotification")]
#if XAMCORE_2_0
		void DidMove (NSNotification  notification);
#else
		void DidMoved (NSNotification  notification);
#endif
	
		[Export ("windowDidBecomeKey:"), EventArgs ("NSNotification")]
		void DidBecomeKey (NSNotification  notification);
	
		[Export ("windowDidResignKey:"), EventArgs ("NSNotification")]
		void DidResignKey (NSNotification  notification);
	
		[Export ("windowDidBecomeMain:"), EventArgs ("NSNotification")]
		void DidBecomeMain (NSNotification  notification);
	
		[Export ("windowDidResignMain:"), EventArgs ("NSNotification")]
		void DidResignMain (NSNotification  notification);
	
		[Export ("windowWillClose:"), EventArgs ("NSNotification")]
		void WillClose (NSNotification  notification);
	
		[Export ("windowWillMiniaturize:"), EventArgs ("NSNotification")]
		void WillMiniaturize (NSNotification  notification);
	
		[Export ("windowDidMiniaturize:"), EventArgs ("NSNotification")]
		void DidMiniaturize (NSNotification  notification);
	
		[Export ("windowDidDeminiaturize:"), EventArgs ("NSNotification")]
		void DidDeminiaturize (NSNotification  notification);
	
		[Export ("windowDidUpdate:"), EventArgs ("NSNotification")]
		void DidUpdate (NSNotification  notification);
	
		[Export ("windowDidChangeScreen:"), EventArgs ("NSNotification")]
		void DidChangeScreen (NSNotification  notification);
	
		[Export ("windowDidChangeScreenProfile:"), EventArgs ("NSNotification")]
		void DidChangeScreenProfile (NSNotification notification);
	
		[Export ("windowWillBeginSheet:"), EventArgs ("NSNotification")]
		void WillBeginSheet (NSNotification notification);
	
		[Export ("windowDidEndSheet:"), EventArgs ("NSNotification")]
		void DidEndSheet (NSNotification notification);
	
		[Export ("windowWillStartLiveResize:"), EventArgs ("NSNotification")]
		void WillStartLiveResize (NSNotification notification);
	
		[Export ("windowDidEndLiveResize:"), EventArgs ("NSNotification")]
		void DidEndLiveResize (NSNotification notification);

		[Mac (10, 7), Export ("windowWillEnterFullScreen:"), EventArgs ("NSNotification")]
		void WillEnterFullScreen (NSNotification notification);

		[Mac (10, 7), Export ("windowDidEnterFullScreen:"), EventArgs ("NSNotification")]
		void DidEnterFullScreen (NSNotification notification);

		[Mac (10, 7), Export ("windowWillExitFullScreen:"), EventArgs ("NSNotification")]
		void WillExitFullScreen (NSNotification  notification);
		
		[Mac (10, 7), Export ("windowDidExitFullScreen:"), EventArgs ("NSNotification")]
		void DidExitFullScreen (NSNotification notification);

		[Mac (10, 7), Export ("windowDidFailToEnterFullScreen:"), EventArgs ("NSWindow")]
		void DidFailToEnterFullScreen (NSWindow window);

		[Mac (10, 7), Export ("windowDidFailToExitFullScreen:"), EventArgs ("NSWindow")]
		void DidFailToExitFullScreen (NSWindow window);
		
		[Mac (10, 7), Export ("window:willUseFullScreenContentSize:"), DelegateName ("NSWindowSize"), DefaultValueFromArgument ("proposedSize")]
		CGSize WillUseFullScreenContentSize (NSWindow  window, CGSize proposedSize);
		
		[Mac (10, 7), Export ("window:willUseFullScreenPresentationOptions:"), DelegateName ("NSWindowApplicationPresentationOptions"), DefaultValueFromArgument ("proposedOptions")]
		NSApplicationPresentationOptions WillUseFullScreenPresentationOptions (NSWindow  window, NSApplicationPresentationOptions proposedOptions);
		
		[Mac (10, 7), Export ("customWindowsToEnterFullScreenForWindow:"), DelegateName ("NSWindowWindows"), DefaultValue (null)]
		NSWindow[] CustomWindowsToEnterFullScreen (NSWindow  window);

		[Mac (10, 7), Export ("customWindowsToExitFullScreenForWindow:"), DelegateName ("NSWindowWindows"), DefaultValue (null)]
		NSWindow[] CustomWindowsToExitFullScreen (NSWindow  window);

		[Mac (10, 7), Export ("window:startCustomAnimationToEnterFullScreenWithDuration:"), EventArgs("NSWindowDuration")]
		void StartCustomAnimationToEnterFullScreen (NSWindow  window, double duration);

		[Mac (10, 7), Export ("window:startCustomAnimationToExitFullScreenWithDuration:"), EventArgs("NSWindowDuration")]
		void StartCustomAnimationToExitFullScreen (NSWindow  window, double duration);

		[Mac (10, 7), Export ("window:willEncodeRestorableState:"), EventArgs ("NSWindowCoder")]
		void WillEncodeRestorableState(NSWindow window, NSCoder coder);
		
		[Mac (10, 7), Export ("window:didDecodeRestorableState:"), EventArgs ("NSWindowCoder")]
		void DidDecodeRestorableState(NSWindow window, NSCoder coder);
		
		[Mac (10, 7), Export ("window:willResizeForVersionBrowserWithMaxPreferredSize:maxAllowedSize:"), DelegateName ("NSWindowSizeSize"), DefaultValueFromArgument ("maxPreferredSize")]
		CGSize WillResizeForVersionBrowser(NSWindow window, CGSize maxPreferredSize, CGSize maxAllowedSize);
		
		[Mac (10, 7), Export ("windowWillEnterVersionBrowser:"), EventArgs ("NSNotification")]
		void WillEnterVersionBrowser (NSNotification notification);
		
		[Mac (10, 7), Export ("windowDidEnterVersionBrowser:"), EventArgs ("NSNotification")]
		void DidEnterVersionBrowser (NSNotification notification);
		
		[Mac (10, 7), Export ("windowWillExitVersionBrowser:"), EventArgs ("NSNotification")]
		void WillExitVersionBrowser (NSNotification notification);
		
		[Mac (10, 7), Export ("windowDidExitVersionBrowser:"), EventArgs ("NSNotification")]
		void DidExitVersionBrowser (NSNotification notification);

		[Availability (Introduced = Platform.Mac_10_7)]
		[Export ("windowDidChangeBackingProperties:"), EventArgs ("NSNotification")]
		void DidChangeBackingProperties (NSNotification notification);
	}

	interface NSWorkspaceRenamedEventArgs {
		[Export ("NSWorkspaceVolumeLocalizedNameKey")]
		string VolumeLocalizedName { get; }
		
		[Export ("NSWorkspaceVolumeURLKey")]
		NSUrl VolumeUrl { get; }

		[Export ("NSWorkspaceVolumeOldLocalizedNameKey")]
		string OldVolumeLocalizedName { get; }

		[Export ("NSWorkspaceVolumeOldURLKey")]
		NSUrl OldVolumeUrl { get; }
	}

	interface NSWorkspaceMountEventArgs {
		[Export ("NSWorkspaceVolumeLocalizedNameKey")]
		string VolumeLocalizedName { get; }
		
		[Export ("NSWorkspaceVolumeURLKey")]
		NSUrl VolumeUrl { get; }
	}

	interface NSWorkspaceApplicationEventArgs {
		[Export ("NSWorkspaceApplicationKey")]
		NSRunningApplication Application { get; }
	}

	interface NSWorkspaceFileOperationEventArgs {
		[Export ("NSOperationNumber")]
		nint FileType { get; }
	}
	
	delegate void NSWorkspaceUrlHandler (NSDictionary newUrls, NSError error);
	
	[BaseType (typeof (NSObject))]
	interface NSWorkspace : NSWorkspaceAccessibilityExtensions {
		[Static]
		[Export ("sharedWorkspace"), ThreadSafe]
		NSWorkspace SharedWorkspace { get; }
		
		[Export ("notificationCenter"), ThreadSafe]
		NSNotificationCenter NotificationCenter { get; }
		
		[Export ("openFile:"), ThreadSafe]
		bool OpenFile (string fullPath);
		
		[Export ("openFile:withApplication:"), ThreadSafe]
		bool OpenFile (string fullPath, [NullAllowed] string appName);
		
		[Export ("openFile:withApplication:andDeactivate:"), ThreadSafe]
		bool OpenFile (string fullPath, [NullAllowed] string appName, bool deactivate);
		
		[Export ("openFile:fromImage:at:inView:"), ThreadSafe]
		bool OpenFile (string fullPath, NSImage anImage, CGPoint point, NSView aView);
		
		[Export ("openURL:"), ThreadSafe]
		bool OpenUrl (NSUrl url);
		
		[Export ("launchApplication:"), ThreadSafe]
		bool LaunchApplication (string appName);
		
		[Export ("launchApplicationAtURL:options:configuration:error:"), ThreadSafe]
		NSRunningApplication LaunchApplication (NSUrl url, NSWorkspaceLaunchOptions options, NSDictionary configuration, out NSError error);
		
		[Export ("launchApplication:showIcon:autolaunch:"), ThreadSafe]
		bool LaunchApplication (string appName, bool showIcon, bool autolaunch);
		
		[Export ("fullPathForApplication:"), ThreadSafe]
		string FullPathForApplication (string appName);
		
		[Export ("selectFile:inFileViewerRootedAtPath:"), ThreadSafe]
		bool SelectFile (string fullPath, string rootFullPath);
		
		[Export ("activateFileViewerSelectingURLs:"), ThreadSafe]
		void ActivateFileViewer (NSUrl[] fileUrls);
		
		[Export ("showSearchResultsForQueryString:"), ThreadSafe]
		bool ShowSearchResults (string queryString );
		
		[Export ("noteFileSystemChanged:")]
		void NoteFileSystemChanged (string path);
		
		[Export ("getInfoForFile:application:type:"), ThreadSafe]
		bool GetInfo (string fullPath, out string appName, out string fileType);
		
		[Export ("isFilePackageAtPath:"), ThreadSafe]
		bool IsFilePackage (string fullPath);
		
		[Export ("iconForFile:"), ThreadSafe]
		NSImage IconForFile (string fullPath);
		
		[Export ("iconForFiles:"), ThreadSafe]
		NSImage IconForFiles (string[] fullPaths);
		
		[Export ("iconForFileType:"), ThreadSafe]
		NSImage IconForFileType (string fileType);
		
		[Export ("setIcon:forFile:options:"), ThreadSafe]
		bool SetIconforFile (NSImage image, string fullPath, NSWorkspaceIconCreationOptions options);
		
		[Export ("fileLabels"), ThreadSafe]
		string[] FileLabels { get ; }
		
		[Export ("fileLabelColors"), ThreadSafe]
		NSColor[] FileLabelColors { get; }
		
		[Export ("recycleURLs:completionHandler:"), ThreadSafe]
		void RecycleUrls (NSArray urls, NSWorkspaceUrlHandler completionHandler);
		
		[Export ("duplicateURLs:completionHandler:"), ThreadSafe]
		void DuplicateUrls (NSArray urls, NSWorkspaceUrlHandler completionHandler);
		
		[Export ("getFileSystemInfoForPath:isRemovable:isWritable:isUnmountable:description:type:"), ThreadSafe]
		bool GetFileSystemInfo (string fullPath, out bool removableFlag, out bool writableFlag, out bool unmountableFlag, out string description, out string fileSystemType);
		
		[Export ("performFileOperation:source:destination:files:tag:"), ThreadSafe]
		bool PerformFileOperation (NSString workspaceOperation, string source, string destination, string[] files, out nint tag);
		
		[Export ("unmountAndEjectDeviceAtPath:"), ThreadSafe]
		bool UnmountAndEjectDevice(string path);

		[Export ("unmountAndEjectDeviceAtURL:error:"), ThreadSafe]
		bool UnmountAndEjectDevice (NSUrl url, out NSError error);
		
		[Export ("extendPowerOffBy:")]
		nint ExtendPowerOffBy (nint requested);
		
		[Export ("hideOtherApplications")]
		void HideOtherApplications ();
		
		[Export ("mountedLocalVolumePaths")]
		string[] MountedLocalVolumePaths { get; }
		
		[Export ("mountedRemovableMedia")]
		string[] MountedRemovableMedia {  get; }
		
		[Export ("URLForApplicationWithBundleIdentifier:"), ThreadSafe]
		NSUrl UrlForApplication (string bundleIdentifier );
		
		[Export ("URLForApplicationToOpenURL:"), ThreadSafe]
		NSUrl UrlForApplication (NSUrl url );
		
		[Export ("absolutePathForAppBundleWithIdentifier:"), ThreadSafe]
		string AbsolutePathForAppBundle (string bundleIdentifier);
		
		[Export ("launchAppWithBundleIdentifier:options:additionalEventParamDescriptor:launchIdentifier:"), ThreadSafe]
		bool LaunchApp (string bundleIdentifier, NSWorkspaceLaunchOptions options, NSAppleEventDescriptor descriptor, IntPtr identifier);
		
		[Internal]
		[Export ("openURLs:withAppBundleIdentifier:options:additionalEventParamDescriptor:launchIdentifiers:"), ThreadSafe]
		bool _OpenUrls (NSUrl[] urls, string bundleIdentifier, NSWorkspaceLaunchOptions options, NSAppleEventDescriptor descriptor, [NullAllowed] string[] identifiers);
		
		[Export ("launchedApplications")]
		NSDictionary [] LaunchedApplications { get; }
		
		[Export ("activeApplication")]
		NSDictionary ActiveApplication { get; }
		
		[Export ("typeOfFile:error:"), ThreadSafe]
		string TypeOfFile (string absoluteFilePath, out NSError outError);
		
		[Export ("localizedDescriptionForType:"), ThreadSafe]
		string LocalizedDescription (string typeName);
		
		[Export ("preferredFilenameExtensionForType:"), ThreadSafe]
		string PreferredFilenameExtension (string typeName);
		
		[Export ("filenameExtension:isValidForType:"), ThreadSafe]
		bool IsFilenameExtensionValid (string filenameExtension, string typeName);
		
		[Export ("type:conformsToType:"), ThreadSafe]
		bool TypeConformsTo (string firstTypeName, string secondTypeName);
		
		[Export ("setDesktopImageURL:forScreen:options:error:")]
		bool SetDesktopImageUrl (NSUrl url, NSScreen screen, NSDictionary options, NSError error );
		
		[Export ("desktopImageURLForScreen:")]
		NSUrl DesktopImageUrl (NSScreen screen );
		
		[Export ("desktopImageOptionsForScreen:")]
		NSDictionary DesktopImageOptions (NSScreen screen);		
		
		[Export ("runningApplications"), ThreadSafe]
		NSRunningApplication [] RunningApplications { get; }

		[Mac (10, 7)]
		[Export ("frontmostApplication")]
		NSRunningApplication FrontmostApplication { get; }

		[Mac (10, 7)]
		[Export ("menuBarOwningApplication")]
		NSRunningApplication MenuBarOwningApplication { get; }

		[Field ("NSWorkspaceWillPowerOffNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString WillPowerOffNotification { get; }

		[Field ("NSWorkspaceWillSleepNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString WillSleepNotification { get; }
		
		[Field ("NSWorkspaceDidWakeNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString DidWakeNotification { get; }
		
		[Field ("NSWorkspaceScreensDidSleepNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString ScreensDidSleepNotification { get; }
		
		[Field ("NSWorkspaceScreensDidWakeNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString ScreensDidWakeNotification { get; }
		
		[Field ("NSWorkspaceSessionDidBecomeActiveNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString SessionDidBecomeActiveNotification { get; }
		
		[Field ("NSWorkspaceSessionDidResignActiveNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString SessionDidResignActiveNotification { get; }

		[Field ("NSWorkspaceDidRenameVolumeNotification")]
		[Notification (typeof (NSWorkspaceRenamedEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidRenameVolumeNotification { get; }

		[Field ("NSWorkspaceDidMountNotification")]
		[Notification (typeof (NSWorkspaceMountEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidMountNotification { get; }
		
		[Field ("NSWorkspaceDidUnmountNotification")]
		[Notification (typeof (NSWorkspaceMountEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidUnmountNotification { get; }
		
		[Field ("NSWorkspaceWillUnmountNotification")]
		[Notification (typeof (NSWorkspaceMountEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString WillUnmountNotification { get; }

		[Field ("NSWorkspaceWillLaunchApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString WillLaunchApplication { get; }

		[Field ("NSWorkspaceDidLaunchApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidLaunchApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidTerminateApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidTerminateApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidHideApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidHideApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidUnhideApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidUnhideApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidActivateApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidActivateApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidDeactivateApplicationNotification")]
		[Notification (typeof (NSWorkspaceApplicationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidDeactivateApplicationNotification { get; }
		
		[Field ("NSWorkspaceDidPerformFileOperationNotification")]
		[Notification (typeof (NSWorkspaceFileOperationEventArgs), "SharedWorkspace.NotificationCenter")]
		NSString DidPerformFileOperationNotification { get; }
		
		[Field ("NSWorkspaceDidChangeFileLabelsNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString DidChangeFileLabelsNotification { get; }

		[Field ("NSWorkspaceActiveSpaceDidChangeNotification")]
		[Notification ("SharedWorkspace.NotificationCenter")]
		NSString ActiveSpaceDidChangeNotification { get; }

		[Field ("NSWorkspaceLaunchConfigurationAppleEvent")]
		NSString LaunchConfigurationAppleEvent { get; }

		[Field ("NSWorkspaceLaunchConfigurationArguments")]
		NSString LaunchConfigurationArguments { get; }

		[Field ("NSWorkspaceLaunchConfigurationEnvironment")]
		NSString LaunchConfigurationEnvironment { get; }

		[Field ("NSWorkspaceLaunchConfigurationArchitecture")]
		NSString LaunchConfigurationArchitecture { get; }
		
		//
		// File operations
		//
		// Those not listed are not here, because they are documented as returing an error
		//
		[Field ("NSWorkspaceRecycleOperation")]
		NSString OperationRecycle { get; }

		[Field ("NSWorkspaceDuplicateOperation")]
		NSString OperationDuplicate { get; }

		[Field ("NSWorkspaceMoveOperation")]
		NSString OperationMove { get; }
		
		[Field ("NSWorkspaceCopyOperation")]
		NSString OperationCopy { get; }
		
		[Field ("NSWorkspaceLinkOperation")]
		NSString OperationLink { get; }
		
		[Field ("NSWorkspaceDestroyOperation")]
		NSString OperationDestroy { get; }

		[Mac (10,10)]
		[Export ("openURL:options:configuration:error:")]
		NSRunningApplication OpenURL (NSUrl url, NSWorkspaceLaunchOptions options, NSDictionary configuration, out NSError error);

		[Mac (10,10)]
		[Export ("openURLs:withApplicationAtURL:options:configuration:error:")]
		NSRunningApplication OpenURLs (NSUrl [] urls, NSUrl applicationURL, NSWorkspaceLaunchOptions options, NSDictionary configuration, out NSError error);

		[Mac (10, 10)]
		[Field ("NSWorkspaceAccessibilityDisplayOptionsDidChangeNotification")]
		[Notification]
		NSString DisplayOptionsDidChangeNotification { get; }
	}
	
	[BaseType (typeof (NSObject))]
	[ThreadSafe] // NSRunningApplication is documented to be thread-safe.
	partial interface NSRunningApplication {
		[Export ("terminated")]
		bool Terminated { [Bind ("isTerminated")] get;  }
		
		[Export ("finishedLaunching")]
		bool FinishedLaunching { [Bind ("isFinishedLaunching")] get;  }
		
		[Export ("hidden")]
		bool Hidden { [Bind ("isHidden")] get;  }
		
		[Export ("active")]
		bool Active { [Bind ("isActive")] get;  }
		
		[Export ("activationPolicy")]
		NSApplicationActivationPolicy ActivationPolicy { get;  }
		
		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get;  }
		
		[Export ("bundleIdentifier", ArgumentSemantic.Copy)]
		string BundleIdentifier { get;  }
		
		[Export ("bundleURL", ArgumentSemantic.Copy)]
		NSUrl BundleUrl { get;  }
		
		[Export ("executableURL", ArgumentSemantic.Copy)]
		NSUrl ExecutableUrl { get;  }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get;  } /* pid_t = int */
		
		[Export ("launchDate", ArgumentSemantic.Copy)]
		NSDate LaunchDate { get;  }
		
		[Export ("icon", ArgumentSemantic.Strong)]
		NSImage Icon { get;  }
		
		[Export ("executableArchitecture")]
		nint ExecutableArchitecture { get;  }
		
		[Export ("hide")]
		bool Hide ();
		
		[Export ("unhide")]
		bool Unhide ();
		
		[Export ("activateWithOptions:")]
		bool Activate (NSApplicationActivationOptions options);
		
		[Export ("terminate")]
		bool Terminate ();
		
		[Export ("forceTerminate")]
		bool ForceTerminate ();
		
		[Static]
		[Export ("runningApplicationsWithBundleIdentifier:")]
		NSRunningApplication[] GetRunningApplications (string bundleIdentifier);
		
		[Static]
		[Export ("runningApplicationWithProcessIdentifier:")]
		NSRunningApplication GetRunningApplication (int /* pid_t = int */ pid);
		
		[Static][ThreadSafe]
		[Export ("currentApplication")]
		NSRunningApplication CurrentApplication { get ; }

		[Export ("ownsMenuBar")]
		bool OwnsMenuBar { get; }
	}	

	[BaseType (typeof (NSControl))]
	interface NSStepper : NSAccessibilityStepper {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		//Detected properties
		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("increment")]
		double Increment { get; set; }

		[Export ("valueWraps")]
		bool ValueWraps { get; set; }

		[Export ("autorepeat")]
		bool Autorepeat { get; set; }

	}

	[BaseType (typeof(NSActionCell))]
	interface NSStepperCell
	{
		[Export ("minValue")]
		double MinValue { get; set; }

		[Export ("maxValue")]
		double MaxValue { get; set; }

		[Export ("increment")]
		double Increment { get; set; }

		[Export ("valueWraps")]
		bool ValueWraps { get; set; }

		[Export ("autorepeat")]
		bool Autorepeat { get; set; }
	}

	
	[BaseType (typeof (NSObject))]
	interface NSPredicateEditorRowTemplate : NSCoding, NSCopying {
		[Export ("matchForPredicate:")]
		double MatchForPredicate (NSPredicate predicate);

		[Export ("templateViews")]
		NSObject[] TemplateViews { get; }

		[Export ("setPredicate:")]
		void SetPredicate (NSPredicate predicate);

		[Export ("predicateWithSubpredicates:")]
		NSPredicate PredicateWithSubpredicates (NSPredicate[] subpredicates);
		
		[Export ("displayableSubpredicatesOfPredicate:")]
		NSPredicate[] DisplayableSubpredicatesOfPredicate (NSPredicate predicate);

		[Export ("initWithLeftExpressions:rightExpressions:modifier:operators:options:")]
		//NSObject InitWithLeftExpressionsrightExpressionsmodifieroperatorsoptions (NSArray leftExpressions, NSArray rightExpressions, NSComparisonPredicateModifier modifier, NSArray operators, uint options);
		IntPtr Constructor (NSExpression[] leftExpressions, NSExpression[] rightExpressions, NSComparisonPredicateModifier modifier, NSObject[] operators, NSComparisonPredicateOptions options);

		[Export ("initWithLeftExpressions:rightExpressionAttributeType:modifier:operators:options:")]
		//NSObject InitWithLeftExpressionsrightExpressionAttributeTypemodifieroperatorsoptions (NSArray leftExpressions, NSAttributeType attributeType, NSComparisonPredicateModifier modifier, NSArray operators, uint options);
		IntPtr Constructor (NSExpression[] leftExpressions, NSAttributeType attributeType, NSComparisonPredicateModifier modifier, NSObject[] operators, NSComparisonPredicateOptions options);

		[Export ("initWithCompoundTypes:")]
		IntPtr Constructor (NSNumber[] compoundTypes);

		[Export ("leftExpressions")]
		NSExpression[] LeftExpressions { get; }

		[Export ("rightExpressions")]
		NSExpression[] RightExpressions { get; }

		[Export ("rightExpressionAttributeType")]
		NSAttributeType RightExpressionAttributeType { get; }

		[Export ("modifier")]
		NSComparisonPredicateModifier Modifier { get; }

		[Export ("operators")]
		NSObject[] Operators { get; }

		[Export ("options")]
		NSComparisonPredicateOptions Options { get; }

		[Export ("compoundTypes")]
		NSNumber[] CompoundTypes { get; }

		[Static]
		[Export ("templatesWithAttributeKeyPaths:inEntityDescription:")]
		//NSArray TemplatesWithAttributeKeyPathsinEntityDescription (NSArray keyPaths, NSEntityDescription entityDescription);
		NSPredicateEditorRowTemplate[] GetTemplates (string[] keyPaths, NSEntityDescription entityDescription);

	}

	[Mac(10,10,3)]
	[BaseType (typeof (NSObject))]
	interface NSPressureConfiguration
	{
		[Export ("pressureBehavior")]
		NSPressureBehavior PressureBehavior { get; }

		[DesignatedInitializer]
		[Export ("initWithPressureBehavior:")]
		IntPtr Constructor (NSPressureBehavior pressureBehavior);

		[Export ("set")]
		void Set ();
	}
   
	[BaseType (typeof (NSControl), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSRuleEditorDelegate)})]
	partial interface NSRuleEditor {
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("reloadCriteria")]
		void ReloadCriteria ();

		[Export ("predicate")]
		NSPredicate Predicate { get; }

		[Export ("reloadPredicate")]
		void ReloadPredicate ();

		[Export ("predicateForRow:")]
		NSPredicate GetPredicate (nint row);

		[Export ("numberOfRows")]
		nint NumberOfRows { get; }

		[Export ("subrowIndexesForRow:")]
		NSIndexSet SubrowIndexes (nint rowIndex);

		[Export ("criteriaForRow:")]
		NSArray Criteria (nint row);

		[Export ("displayValuesForRow:")]
		NSObject[] DisplayValues (nint row);

		[Export ("rowForDisplayValue:")]
		nint Row (NSObject displayValue);

		[Export ("rowTypeForRow:")]
		NSRuleEditorRowType RowType (nint rowIndex);

		[Export ("parentRowForRow:")]
		nint ParentRow (nint rowIndex);

		[Export ("addRow:")]
		void AddRow (NSObject sender);

		[Export ("insertRowAtIndex:withType:asSubrowOfRow:animate:")]
		void InsertRowAtIndex (nint rowIndex, NSRuleEditorRowType rowType, nint parentRow, bool shouldAnimate);

		[Export ("setCriteria:andDisplayValues:forRowAtIndex:")]
		void SetCriteria (NSArray criteria, NSArray values, nint rowIndex);

		[Export ("removeRowAtIndex:")]
		void RemoveRowAtIndex (nint rowIndex);

		[Export ("removeRowsAtIndexes:includeSubrows:")]
		void RemoveRowsAtIndexes (NSIndexSet rowIndexes, bool includeSubrows);

		[Export ("selectedRowIndexes")]
		NSIndexSet SelectedRows { get; }

		[Export ("selectRowIndexes:byExtendingSelection:")]
		void SelectRows (NSIndexSet indexes, bool extend);

		//Detected properties
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
#if XAMCORE_2_0
		NSObject  WeakDelegate { get; set; }
#else
		NSRuleEditorDelegate WeakDelegate { get; set; }
#endif

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSRuleEditorDelegate Delegate { get; set; }
       
		[Export ("formattingStringsFilename")]
		string FormattingStringsFilename { get; set; }

		[Export ("formattingDictionary", ArgumentSemantic.Copy)]
		NSDictionary FormattingDictionary { get; set; }

		[Export ("nestingMode")]
		NSRuleEditorNestingMode NestingMode { get; set; }

		[Export ("rowHeight")]
		nfloat RowHeight { get; set; }

		[Export ("editable")]
		bool Editable { [Bind ("isEditable")]get; set; }

		[Export ("canRemoveAllRows")]
		bool CanRemoveAllRows { get; set; }

		[Export ("rowClass")]
		Class RowClass { get; set; }

		[Export ("rowTypeKeyPath")]
		string RowTypeKeyPath { get; set; }

		[Export ("subrowsKeyPath")]
		string SubrowsKeyPath { get; set; }

		[Export ("criteriaKeyPath")]
		string CriteriaKeyPath { get; set; }

		[Export ("displayValuesKeyPath")]
		string DisplayValuesKeyPath { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSRuleEditorDelegate {
		[Abstract]
		[Export ("ruleEditor:numberOfChildrenForCriterion:withRowType:"), DelegateName ("NSRuleEditorNumberOfChildren"), DefaultValue(0)]
		nint NumberOfChildren (NSRuleEditor editor, NSObject criterion, NSRuleEditorRowType rowType);

		[Abstract]
		[Export ("ruleEditor:child:forCriterion:withRowType:"), DelegateName ("NSRulerEditorChildCriterion"), DefaultValue(null)]
		NSObject ChildForCriterion (NSRuleEditor editor, nint index, NSObject criterion, NSRuleEditorRowType rowType);

		[Abstract]
		[Export ("ruleEditor:displayValueForCriterion:inRow:"), DelegateName ("NSRulerEditorDisplayValue"), DefaultValue(null)]
		NSObject DisplayValue (NSRuleEditor editor, NSObject criterion, nint row);

		[Abstract]
		[Export ("ruleEditor:predicatePartsForCriterion:withDisplayValue:inRow:"), DelegateName ("NSRulerEditorPredicateParts"), DefaultValue(null)]
		NSDictionary PredicateParts (NSRuleEditor editor, NSObject criterion, NSObject value, nint row);

		[Abstract]
		[Export ("ruleEditorRowsDidChange:"), EventArgs ("NSNotification")]
		void RowsDidChange (NSNotification notification);
		
		[Export ("controlTextDidEndEditing:"), EventArgs ("NSNotification")]
		void EditingEnded (NSNotification notification);

		[Export ("controlTextDidChange:"), EventArgs ("NSNotification")]
		void Changed (NSNotification notification);

		[Export ("controlTextDidBeginEditing:"), EventArgs ("NSNotification")]
		void EditingBegan (NSNotification notification);			

	}
   
	[BaseType (typeof (NSRuleEditor))]
	interface NSPredicateEditor {
		//Detected properties
		[Export ("rowTemplates", ArgumentSemantic.Copy)]
		NSPredicateEditorRowTemplate[] RowTemplates { get; set; }

	} 	

	// Start of NSSharingService.h
	
	[Mac (10, 8)]
	delegate void NSSharingServiceHandler ();
	
	[Mac (10, 8)]
	[BaseType (typeof (NSObject),
	           Delegates=new string [] {"WeakDelegate"},
	Events=new Type [] { typeof (NSSharingServiceDelegate) })]
	interface NSSharingService 
	{
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSSharingServiceDelegate Delegate { get; set; }
		
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; }
		
		[Export ("image", ArgumentSemantic.Strong)]
		NSImage Image { get; }
		
		[Export ("alternateImage", ArgumentSemantic.Strong)]
		NSImage AlternateImage { get; }
		
		[Export ("sharingServicesForItems:")][Static]
		NSSharingService [] SharingServicesForItems (NSObject [] items);
		
		[Export ("sharingServiceNamed:")][Static]
		NSSharingService GetSharingService (NSString serviceName);
		
		[DesignatedInitializer]
		[Export ("initWithTitle:image:alternateImage:handler:")]
		IntPtr Constructor (string title, NSImage image, NSImage alternateImage, NSSharingServiceHandler handler);
		
		[Export ("canPerformWithItems:")]
		bool CanPerformWithItems ([NullAllowed] NSObject [] items);

		[Export ("performWithItems:")]
		void PerformWithItems (NSObject [] items);

		[Mac (10,9)]
		[Export ("menuItemTitle")]
		string MenuItemTitle { get; set; }

		[Mac (10,9)]
		[Export ("recipients", ArgumentSemantic.Copy)]
		NSObject [] Recipients { get; set; }

		[Mac (10,9)]
		[Export ("subject")]
		string Subject { get; set; }

		[Mac (10,9)]
		[Export ("messageBody")]
		string MessageBody { get; }

		[Mac (10,9)]
		[Export ("permanentLink", ArgumentSemantic.Copy)]
		NSUrl PermanentLink { get; }

		[Mac (10,9)]
		[Export ("accountName")]
		string AccountName { get; }

		[Mac (10,9)]
		[Export ("attachmentFileURLs", ArgumentSemantic.Copy)]
		NSUrl [] AttachmentFileUrls { get; }		
		
		// Constants

		[Field ("NSSharingServiceNamePostOnFacebook")][Internal]
		NSString NSSharingServiceNamePostOnFacebook { get; }
		
		[Field ("NSSharingServiceNamePostOnTwitter")][Internal]
		NSString NSSharingServiceNamePostOnTwitter { get; }
		
		[Field ("NSSharingServiceNamePostOnSinaWeibo")][Internal]
		NSString NSSharingServiceNamePostOnSinaWeibo { get; }
		
		[Field ("NSSharingServiceNameComposeEmail")][Internal]
		NSString NSSharingServiceNameComposeEmail { get; }
		
		[Field ("NSSharingServiceNameComposeMessage")][Internal]
		NSString NSSharingServiceNameComposeMessage { get; }
		
		[Field ("NSSharingServiceNameSendViaAirDrop")][Internal]
		NSString NSSharingServiceNameSendViaAirDrop { get; }
		
		[Field ("NSSharingServiceNameAddToSafariReadingList")][Internal]
		NSString NSSharingServiceNameAddToSafariReadingList { get; }
		
		[Field ("NSSharingServiceNameAddToIPhoto")][Internal]
		NSString NSSharingServiceNameAddToIPhoto { get; }
		
		[Field ("NSSharingServiceNameAddToAperture")][Internal]
		NSString NSSharingServiceNameAddToAperture { get; }
		
		[Field ("NSSharingServiceNameUseAsTwitterProfileImage")][Internal]
		NSString NSSharingServiceNameUseAsTwitterProfileImage { get; }
		
		[Field ("NSSharingServiceNameUseAsDesktopPicture")][Internal]
		NSString NSSharingServiceNameUseAsDesktopPicture { get; }
		
		[Field ("NSSharingServiceNamePostImageOnFlickr")][Internal]
		NSString NSSharingServiceNamePostImageOnFlickr { get; }
		
		[Field ("NSSharingServiceNamePostVideoOnVimeo")][Internal]
		NSString NSSharingServiceNamePostVideoOnVimeo { get; }
		
		[Field ("NSSharingServiceNamePostVideoOnYouku")][Internal]
		NSString NSSharingServiceNamePostVideoOnYouku { get; }
		
		[Field ("NSSharingServiceNamePostVideoOnTudou")][Internal]
		NSString NSSharingServiceNamePostVideoOnTudou { get; }

		[Mac (10, 12)]
		[Field ("NSSharingServiceNameCloudSharing")][Internal]
		NSString NSSharingServiceNameCloudSharing { get; }
	}
	
	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSSharingServiceDelegate 
	{
		[Export ("sharingService:willShareItems:"), EventArgs ("NSSharingServiceItems")]
		void WillShareItems (NSSharingService sharingService, NSObject [] items);
		
		[Export ("sharingService:didFailToShareItems:error:"), EventArgs ("NSSharingServiceDidFailToShareItems")]
		void DidFailToShareItems (NSSharingService sharingService, NSObject [] items, NSError error);
		
		[Export ("sharingService:didShareItems:"), EventArgs ("NSSharingServiceItems")]
		void DidShareItems (NSSharingService sharingService, NSObject [] items);
		
		[Export ("sharingService:sourceFrameOnScreenForShareItem:"), DelegateName ("NSSharingServiceSourceFrameOnScreenForShareItem"), DefaultValue (null)]
		CGRect SourceFrameOnScreenForShareItem (NSSharingService sharingService, INSPasteboardWriting item);
		
		[Export ("sharingService:transitionImageForShareItem:contentRect:"), DelegateName ("NSSharingServiceTransitionImageForShareItem"), DefaultValue (null)]
		NSImage TransitionImageForShareItem (NSSharingService sharingService, INSPasteboardWriting item, CGRect contentRect);
		
		[Export ("sharingService:sourceWindowForShareItems:sharingContentScope:"), DelegateName ("NSSharingServiceSourceWindowForShareItems"), DefaultValue (null)]
		NSWindow SourceWindowForShareItems (NSSharingService sharingService, NSObject [] items, NSSharingContentScope sharingContentScope);
	
		[Export ("anchoringViewForSharingService:showRelativeToRect:preferredEdge:"), DelegateName ("NSSharingServiceAnchoringViewForSharingService"), DefaultValue (null)]
		[return: NullAllowed]
		NSView CreateAnchoringView (NSSharingService sharingService, ref CGRect positioningRect, ref NSRectEdge preferredEdge);
	}

	interface INSSharingServiceDelegate {}

	[Protocol, Model]
	[Mac (10, 12, onlyOn64: true)]
	[BaseType (typeof (NSSharingServiceDelegate))]
	interface NSCloudSharingServiceDelegate
	{
		[Export ("sharingService:didCompleteForItems:error:")]
		void Completed (NSSharingService sharingService, NSObject[] items, [NullAllowed] NSError error);

		[Export ("optionsForSharingService:shareProvider:")]
		NSCloudKitSharingServiceOptions Options (NSSharingService cloudKitSharingService, NSItemProvider provider);

		[Export ("sharingService:didSaveShare:")]
		void Saved (NSSharingService sharingService, CKShare share);

		[Export ("sharingService:didStopSharing:")]
		void Stopped (NSSharingService sharingService, CKShare share);
	}

	[Mac (10, 8)]
	[BaseType (typeof (NSObject),
	           Delegates=new string [] {"WeakDelegate"},
	Events=new Type [] { typeof (NSSharingServicePickerDelegate) })]
	interface NSSharingServicePicker 
	{
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSSharingServicePickerDelegate Delegate { get; set; }
		
		[DesignatedInitializer]
		[Export ("initWithItems:")]
		IntPtr Constructor (NSObject [] items);
		
		[Export ("showRelativeToRect:ofView:preferredEdge:")]
		void ShowRelativeToRect (CGRect rect, NSView view, NSRectEdge preferredEdge);
	}

	interface INSSharingServicePickerDelegate {}

	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSSharingServicePickerDelegate 
	{
		[Export ("sharingServicePicker:sharingServicesForItems:proposedSharingServices:"), DelegateName ("NSSharingServicePickerSharingServicesForItems"), DefaultValueFromArgument ("proposedServices")]
		NSSharingService [] SharingServicesForItems (NSSharingServicePicker sharingServicePicker, NSObject [] items, NSSharingService [] proposedServices);
		
		[Export ("sharingServicePicker:delegateForSharingService:"), DelegateName ("NSSharingServicePickerDelegateForSharingService"), DefaultValue (null)]
#if XAMCORE_2_0
		INSSharingServiceDelegate DelegateForSharingService (NSSharingServicePicker sharingServicePicker, NSSharingService sharingService);
#else
		NSSharingServiceDelegate DelegateForSharingService (NSSharingServicePicker sharingServicePicker, NSSharingService sharingService);
#endif
		
		[Export ("sharingServicePicker:didChooseSharingService:"), EventArgs ("NSSharingServicePickerDidChooseSharingService")]
		void DidChooseSharingService (NSSharingServicePicker sharingServicePicker, NSSharingService service);
	}
	
	[BaseType (typeof (NSTypesetter))]
	interface NSATSTypesetter {
		[Static]
		[Export ("sharedTypesetter")]
		NSATSTypesetter SharedTypesetter { get; }
	}

	partial interface NSTypesetter {
		[Export ("substituteFontForFont:")]
		NSFont GetSubstituteFont (NSFont originalFont);

		[Export ("textTabForGlyphLocation:writingDirection:maxLocation:")]
		NSTextTab GetTextTab (nfloat glyphLocation, NSWritingDirection direction, nfloat maxLocation);

		[Export ("setParagraphGlyphRange:separatorGlyphRange:")]
		void SetParagraphGlyphRange (NSRange paragraphRange, NSRange paragraphSeparatorRange);

		[Export ("paragraphGlyphRange")]
		NSRange ParagraphGlyphRange { get; }

		[Export ("paragraphSeparatorGlyphRange")]
		NSRange ParagraphSeparatorGlyphRange { get; }

		[Export ("paragraphCharacterRange")]
		NSRange ParagraphCharacterRange { get; }

		[Export ("paragraphSeparatorCharacterRange")]
		NSRange ParagraphSeparatorCharacterRange { get; }

		[Export ("layoutParagraphAtPoint:")]
		nuint LayoutParagraphAtPoint (ref CGPoint lineFragmentOrigin);

		[Export ("beginParagraph")]
		void BeginParagraph ();

		[Export ("endParagraph")]
		void EndParagraph ();

		[Export ("beginLineWithGlyphAtIndex:")]
		void BeginLine (nuint glyphIndex);

		[Export ("endLineWithGlyphRange:")]
		void EndLine (NSRange lineGlyphRange);

		[Export ("lineSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetLineSpacingAfterGlyph (nuint glyphIndex, CGRect proposedLineFragmentRect);

		[Export ("paragraphSpacingBeforeGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingBeforeGlyph (nuint glyphIndex, CGRect proposedLineFragmentRect);

		[Export ("paragraphSpacingAfterGlyphAtIndex:withProposedLineFragmentRect:")]
		nfloat GetParagraphSpacingAfterGlyph (nuint glyphIndex, CGRect proposedLineFragmentRect);

		[Export ("getLineFragmentRect:usedRect:forParagraphSeparatorGlyphRange:atProposedOrigin:")]
		void GetLineFragment (out CGRect lineFragmentRect, out CGRect lineFragmentUsedRect, NSRange paragraphSeparatorGlyphRange, CGPoint proposedOrigin);

		[Export ("attributesForExtraLineFragment")]
		NSDictionary AttributesForExtraLineFragment ();

		[Export ("actionForControlCharacterAtIndex:")]
		NSTypesetterControlCharacterAction GetActionForControlCharacter (nuint charIndex);

		[Export ("layoutManager")]
		NSLayoutManager LayoutManager { get; }

		[Export ("textContainers")]
		NSTextContainer [] TextContainers { get; }

		[Export ("currentTextContainer")]
		NSTextContainer CurrentTextContainer { get; }

		[Export ("currentParagraphStyle")]
		NSParagraphStyle CurrentParagraphStyle { get; }

		[Export ("setHardInvalidation:forGlyphRange:")]
		void SetHardInvalidation (bool value, NSRange glyphRange);

		[Export ("layoutGlyphsInLayoutManager:startingAtGlyphIndex:maxNumberOfLineFragments:nextGlyphIndex:")]
		void LayoutGlyphs (NSLayoutManager layoutManager, nuint startGlyphIndex, nuint maxLineFragments, out nuint nextGlyph);

		[Export ("layoutCharactersInRange:forLayoutManager:maximumNumberOfLineFragments:")]
		NSRange LayoutCharacters (NSRange characterRange, NSLayoutManager layoutManager, nuint maxLineFragments);

		// TODO: provide a higher level C# API for this too
		[Static]
		[Export ("printingAdjustmentInLayoutManager:forNominallySpacedGlyphRange:packedGlyphs:count:")]
		CGSize GetInterGlyphSpacing (NSLayoutManager layoutManager, NSRange nominallySpacedGlyphsRange, IntPtr packedGlyphs, nuint packedGlyphsCount);

		[Export ("baselineOffsetInLayoutManager:glyphIndex:")]
		nfloat GetBaselineOffset (NSLayoutManager layoutManager, nuint glyphIndex);

		[Static]
		[Export ("sharedSystemTypesetter")]
		NSTypesetter SharedSystemTypesetter { get; }

		[Static]
		[Export ("sharedSystemTypesetterForBehavior:")]
		NSTypesetter GetSharedSystemTypesetter (NSTypesetterBehavior forBehavior);

		[Static]
		[Export ("defaultTypesetterBehavior")]
		NSTypesetterBehavior DefaultTypesetterBehavior { get; }

		//
		// Detected properties
		[Export ("usesFontLeading")]
		bool UsesFontLeading { get; set; }

		[Export ("typesetterBehavior")]
		NSTypesetterBehavior TypesetterBehavior { get; set; }

		[Export ("hyphenationFactor")]
		float HyphenationFactor { get; set; } /* float, not CGFloat */

		[Export ("lineFragmentPadding")]
		nfloat LineFragmentPadding { get; set; }

		[Export ("bidiProcessingEnabled")]
		bool BidiProcessingEnabled { get; set; }

		[Export ("attributedString")]
		NSAttributedString AttributedString { get; set; }


		///
		/// NSLayoutPhaseInterface
		///

		[Export ("willSetLineFragmentRect:forGlyphRange:usedRect:baselineOffset:")]
		void WillSetLineFragment (ref CGRect lineRect, NSRange glyphRange, ref CGRect usedRect, ref nfloat baselineOffset);

		[Export ("shouldBreakLineByWordBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByWordBeforeCharacter (nuint charIndex);

		[Export ("shouldBreakLineByHyphenatingBeforeCharacterAtIndex:")]
		bool ShouldBreakLineByHyphenatingBeforeCharacter (nuint charIndex);

		[Export ("hyphenationFactorForGlyphAtIndex:")]
		float /* float, not CGFloat */ HyphenationFactorForGlyph (nuint glyphIndex);

		[Export ("hyphenCharacterForGlyphAtIndex:")]
		uint /* UTF32Char */ HyphenCharacterForGlyph (nuint glyphIndex);

		[Export ("boundingBoxForControlGlyphAtIndex:forTextContainer:proposedLineFragment:glyphPosition:characterIndex:")]
		CGRect GetBoundingBoxForControlGlyph (nuint glyphIndex, NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint glyphPosition, nuint charIndex);

		//
		// NSGlyphStorageInterface
		//
		[Export ("characterRangeForGlyphRange:actualGlyphRange:")]
		NSRange GetCharacterRangeForGlyphRange (NSRange glyphRange, out NSRange actualGlyphRange);

		[Export ("glyphRangeForCharacterRange:actualCharacterRange:")]
		NSRange GlyphRangeForCharacterRange (NSRange charRange, out NSRange actualCharRange);

		// TODO: could use a higher level API
		[Export ("getGlyphsInRange:glyphs:characterIndexes:glyphInscriptions:elasticBits:bidiLevels:")]
		nuint GetGlyphsInRange (NSRange glyphsRange, IntPtr glyphBuffer, IntPtr charIndexBuffer, IntPtr inscribeBuffer, IntPtr elasticBuffer, IntPtr bidiLevelBuffer);

		[Export ("getLineFragmentRect:usedRect:remainingRect:forStartingGlyphAtIndex:proposedRect:lineSpacing:paragraphSpacingBefore:paragraphSpacingAfter:")]
		void GetLineFragment (out CGRect lineFragment, out CGRect lineFragmentUsed, out CGRect remaining, nuint startingGlyphIndex, CGRect proposedRect, nfloat lineSpacing, nfloat paragraphSpacingBefore, nfloat paragraphSpacingAfter);

		[Export ("setLineFragmentRect:forGlyphRange:usedRect:baselineOffset:")]
		void SetLineFragment (CGRect fragmentRect, NSRange glyphRange, CGRect usedRect, nfloat baselineOffset);

		[Export ("substituteGlyphsInRange:withGlyphs:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void SubstituteGlyphs (NSRange glyphRange, IntPtr glyphs);

		[Export ("insertGlyph:atGlyphIndex:characterIndex:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void InsertGlyph (uint glyph, nuint glyphIndex, nuint characterIndex); // glyph is NSGlyph - typedef unsigned int NSGlyph;

		[Export ("deleteGlyphsInRange:")]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		void DeleteGlyphs (NSRange glyphRange);

		[Export ("setNotShownAttribute:forGlyphRange:")]
		void SetNotShownAttribute (bool flag, NSRange glyphRange);

		[Export ("setDrawsOutsideLineFragment:forGlyphRange:")]
		void SetDrawsOutsideLineFragment (bool flag, NSRange glyphRange);

		// TODO: high level C# binding
		[Export ("setLocation:withAdvancements:forStartOfGlyphRange:")]
		void SetLocation (CGPoint location, IntPtr advancements, NSRange glyphRange);

		[Export ("setAttachmentSize:forGlyphRange:")]
		void SetAttachmentSize (CGSize attachmentSize, NSRange glyphRange);

		// TODO: high level C# binding
		[Export ("setBidiLevels:forGlyphRange:")]
		void SetBidiLevels (IntPtr levels, NSRange glyphRange);
	}

	partial interface NSCollectionViewDelegate {
		[Mac (10, 7), Export ("collectionView:pasteboardWriterForItemAtIndex:")]
#if XAMCORE_2_0
		INSPasteboardWriting PasteboardWriterForItem (NSCollectionView collectionView, nuint index);
#else
		NSPasteboardWriting PasteboardWriterForItemAtIndex (NSCollectionView collectionView, nuint index);
#endif

		[Mac (10, 7), Export ("collectionView:updateDraggingItemsForDrag:")]
		void UpdateDraggingItemsForDrag (NSCollectionView collectionView, [Protocolize (4)] NSDraggingInfo draggingInfo);

		[Mac (10, 7), Export ("collectionView:draggingSession:willBeginAtPoint:forItemsAtIndexes:")]
		void DraggingSessionWillBegin (NSCollectionView collectionView, NSDraggingSession draggingSession,
			CGPoint screenPoint, NSIndexSet indexes);

		[Mac (10, 7), Export ("collectionView:draggingSession:endedAtPoint:dragOperation:")]
		void DraggingSessionEnded (NSCollectionView collectionView, NSDraggingSession draggingSession,
			CGPoint screenPoint, NSDragOperation dragOperation);
	}

	partial interface NSColor {
		[Mac (10, 7), Static, Export ("colorWithGenericGamma22White:alpha:")]
		NSColor FromGamma22White (nfloat white, nfloat alpha);

		[Mac (10, 7), Static, Export ("colorWithSRGBRed:green:blue:alpha:")]
		NSColor FromSrgb (nfloat red, nfloat green, nfloat blue, nfloat alpha);

		[Notification, Field ("NSSystemColorsDidChangeNotification")]
		NSString SystemColorsChanged { get; }
	}

	partial interface NSDocumentController {
		[Mac (10, 7), Export ("duplicateDocumentWithContentsOfURL:copying:displayName:error:")]
		NSDocument DuplicateDocumentWithContentsOfUrl (NSUrl url, bool duplicateByCopying,
			[NullAllowed] string displayName, out NSError error);

#if !XAMCORE_2_0
		[Mac (10, 7), Export ("openDocumentWithContentsOfURL:display:completionHandler:")]
		[Obsolete ("Use 'OpenDocument' instead.")]
		[Sealed]
		void OpenDocumentWithContentsOfUrl (NSUrl url, bool displayDocument,
			OpenDocumentCompletionHandler completionHandler);
#endif

		[Mac (10, 7), Export ("reopenDocumentForURL:withContentsOfURL:display:completionHandler:")]
		void ReopenDocumentForUrl ([NullAllowed] NSUrl url, NSUrl contentsUrl,
			bool displayDocument, OpenDocumentCompletionHandler completionHandler);
	}

	[Mac (10, 7), Model]
	interface NSTextLayoutOrientationProvider {
		[Export ("layoutOrientation")]
		NSTextLayoutOrientation LayoutOrientation { get; }
	}

	partial interface NSLayoutManager {
		// FIXME: This may need some generator work, or use IntPtr for glyphs?
		//
		//   ./AppKit/NSLayoutManager.g.cs(1015,44): error CS1503: Argument `#1'
		//   cannot convert `ushort[]' expression to type `MonoMac.Foundation.NSObject[]'
		//
		// [Mac (10, 7), Export ("showCGGlyphs:positions:count:font:matrix:attributes:inContext:")]
		// void ShowCGGlyphs (CGGlyph [] glyphs, CGPoint [] positions, uint glyphCount, NSFont font,
		// 	NSAffineTransform textMatrix, NSDictionary attributes, NSGraphicsContext graphicsContext);
	}

	partial interface NSViewColumnMoveEventArgs {
		[Export ("NSOldColumn")]
		nint OldColumn { get; }

		[Export ("NSNewColumn")]
		nint NewColumn { get; }
	}

	partial interface NSViewColumnResizeEventArgs {
		[Export ("NSTableColumn")]
		NSTableColumn Column { get; }

		[Export ("NSOldWidth")]
		nint OldWidth { get; }
	}

	partial interface NSOutlineViewItemEventArgs {
		[Export ("NSObject")]
		NSObject Item { get; }
	}

	partial interface NSOutlineView : NSAccessibilityOutline {

		[Notification, Field ("NSOutlineViewSelectionDidChangeNotification")]
		NSString SelectionDidChangeNotification { get; }

		[Notification, Field ("NSOutlineViewSelectionIsChangingNotification")]
		NSString SelectionIsChangingNotification { get; }

		[Notification (typeof (NSViewColumnMoveEventArgs))]
		[Field ("NSOutlineViewColumnDidMoveNotification")]
		NSString ColumnDidMoveNotification { get; }

		[Notification (typeof (NSViewColumnResizeEventArgs))]
		[Field ("NSOutlineViewColumnDidResizeNotification")]
		NSString ColumnDidResizeNotification { get; }

		[Notification (typeof (NSOutlineViewItemEventArgs))]
		[Field ("NSOutlineViewItemWillExpandNotification")]
		NSString ItemWillExpandNotification { get; }

		[Notification (typeof (NSOutlineViewItemEventArgs))]
		[Field ("NSOutlineViewItemDidExpandNotification")]
		NSString ItemDidExpandNotification { get; }

		[Notification (typeof (NSOutlineViewItemEventArgs))]
		[Field ("NSOutlineViewItemWillCollapseNotification")]
		NSString ItemWillCollapseNotification { get; }

		[Notification (typeof (NSOutlineViewItemEventArgs))]
		[Field ("NSOutlineViewItemDidCollapseNotification")]
		NSString ItemDidCollapseNotification { get; }

		// - (void)moveItemAtIndex:(NSInteger)fromIndex inParent:(id)oldParent toIndex:(NSInteger)toIndex inParent:(id)newParent NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("moveItemAtIndex:inParent:toIndex:inParent:")]
		void MoveItem (nint fromIndex, [NullAllowed] NSObject oldParent, nint toIndex, [NullAllowed] NSObject newParent);

#if !XAMCORE_2_0
		// - (void)insertItemsAtIndexes:(NSIndexSet *)indexes inParent:(id)parent withAnimation:(NSTableViewAnimationOptions)animationOptions NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("insertItemsAtIndexes:inParent:withAnimation:")]
		void InsertItems (NSIndexSet indexes, [NullAllowed] NSObject parent, NSTableViewAnimationOptions animationOptions);

		// - (void)removeItemsAtIndexes:(NSIndexSet *)indexes inParent:(id)parent withAnimation:(NSTableViewAnimationOptions)animationOptions NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("removeItemsAtIndexes:inParent:withAnimation:")]
		void RemoveItems (NSIndexSet indexes, [NullAllowed] NSObject parent, NSTableViewAnimationOptions animationOptions);

		// - (void)insertRowsAtIndexes:(NSIndexSet *)indexes withAnimation:(NSTableViewAnimationOptions)animationOptions UNAVAILABLE_ATTRIBUTE;
		[Export ("insertRowsAtIndexes:withAnimation:")]
		void InsertRows (NSIndexSet indexes, NSTableViewAnimationOptions animationOptions);

		// - (void)removeRowsAtIndexes:(NSIndexSet *)indexes withAnimation:(NSTableViewAnimationOptions)animationOptions UNAVAILABLE_ATTRIBUTE;
		[Export ("removeRowsAtIndexes:withAnimation:")]
		void RemoveRows (NSIndexSet indexes, NSTableViewAnimationOptions animationOptions);
#else
		[Mac (10, 7), Export ("insertItemsAtIndexes:inParent:withAnimation:")]
		void InsertItems (NSIndexSet indexes, [NullAllowed] NSObject parent, NSTableViewAnimation animationOptions);

		[Mac (10, 7), Export ("removeItemsAtIndexes:inParent:withAnimation:")]
		void RemoveItems (NSIndexSet indexes, [NullAllowed] NSObject parent, NSTableViewAnimation animationOptions);

		[Export ("insertRowsAtIndexes:withAnimation:")]
		void InsertRows (NSIndexSet indexes, NSTableViewAnimation animationOptions);

		[Export ("removeRowsAtIndexes:withAnimation:")]
		void RemoveRows (NSIndexSet indexes, NSTableViewAnimation animationOptions);
#endif

		// - (void)moveRowAtIndex:(NSInteger)oldIndex toIndex:(NSInteger)newIndex UNAVAILABLE_ATTRIBUTE;
		[Export ("moveRowAtIndex:toIndex:")]
		void MoveRow (nint oldIndex, nint newIndex);
	}

	partial interface NSOutlineViewDataSource {
		// - (id <NSPasteboardWriting>)outlineView:(NSOutlineView *)outlineView pasteboardWriterForItem:(id)item NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("outlineView:pasteboardWriterForItem:")]
#if XAMCORE_2_0
		INSPasteboardWriting PasteboardWriterForItem (NSOutlineView outlineView, NSObject item);
#else
		NSPasteboardWriting PasteboardWriterForItem (NSOutlineView outlineView, NSObject item);
#endif

		// - (void)outlineView:(NSOutlineView *)outlineView draggingSession:(NSDraggingSession *)session willBeginAtPoint:(NSPoint)screenPoint forItems:(NSArray *)draggedItems NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("outlineView:draggingSession:willBeginAtPoint:forItems:")]
		void DraggingSessionWillBegin (NSOutlineView outlineView, NSDraggingSession session, CGPoint screenPoint, NSArray draggedItems);

		// - (void)outlineView:(NSOutlineView *)outlineView draggingSession:(NSDraggingSession *)session endedAtPoint:(NSPoint)screenPoint operation:(NSDragOperation)operation NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("outlineView:draggingSession:endedAtPoint:operation:")]
		void DraggingSessionEnded (NSOutlineView outlineView, NSDraggingSession session, CGPoint screenPoint, NSDragOperation operation);

		// - (void)outlineView:(NSOutlineView *)outlineView updateDraggingItemsForDrag:(id <NSDraggingInfo>)draggingInfo NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("outlineView:updateDraggingItemsForDrag:")]
		void UpdateDraggingItemsForDrag (NSOutlineView outlineView, [Protocolize (4)] NSDraggingInfo draggingInfo);
	}

	interface NSWindowExposeEventArgs {
		[Export ("NSExposedRect", ArgumentSemantic.Copy)]
		CGRect ExposedRect { get; }
	}

	interface NSWindowBackingPropertiesEventArgs {
		[Export ("NSBackingPropertyOldScaleFactorKey")]
		nint OldScaleFactor { get; }

		[Export ("NSBackingPropertyOldColorSpaceKey")]
		NSColorSpace OldColorSpace { get; }
	}

	partial interface NSWindow {
		//
		// Fields + Notifications
		//
		[Field ("NSWindowDidBecomeKeyNotification")]
		[Notification]
		NSString DidBecomeKeyNotification { get; }

		[Field ("NSWindowDidBecomeMainNotification")]
		[Notification]
		NSString DidBecomeMainNotification { get; }

		[Field ("NSWindowDidChangeScreenNotification")]
		[Notification]
		NSString DidChangeScreenNotification { get; }

		[Field ("NSWindowDidDeminiaturizeNotification")]
		[Notification]
		NSString DidDeminiaturizeNotification { get; }

		[Field ("NSWindowDidExposeNotification")]
		[Notification (typeof (NSWindowExposeEventArgs))]
		NSString DidExposeNotification { get; }

		[Field ("NSWindowDidMiniaturizeNotification")]
		[Notification]
		NSString DidMiniaturizeNotification { get; }

		[Field ("NSWindowDidMoveNotification")]
		[Notification]
		NSString DidMoveNotification { get; }

		[Field ("NSWindowDidResignKeyNotification")]
		[Notification]
		NSString DidResignKeyNotification { get; }

		[Field ("NSWindowDidResignMainNotification")]
		[Notification]
		NSString DidResignMainNotification { get; }

		[Field ("NSWindowDidResizeNotification")]
		[Notification]
		NSString DidResizeNotification { get; }

		[Field ("NSWindowDidUpdateNotification")]
		[Notification]
		NSString DidUpdateNotification { get; }

		[Field ("NSWindowWillCloseNotification")]
		[Notification]
		NSString WillCloseNotification { get; }

		[Field ("NSWindowWillMiniaturizeNotification")]
		[Notification]
		NSString WillMiniaturizeNotification { get; }

		[Field ("NSWindowWillMoveNotification")]
		[Notification]
		NSString WillMoveNotification { get; }

		[Field ("NSWindowWillBeginSheetNotification")]
		[Notification]
		NSString WillBeginSheetNotification { get; }

		[Field ("NSWindowDidEndSheetNotification")]
		[Notification]
		NSString DidEndSheetNotification { get; }

		[Field ("NSWindowDidChangeScreenProfileNotification")]
		[Notification]
		NSString DidChangeScreenProfileNotification { get; }

		[Field ("NSWindowWillStartLiveResizeNotification")]
		[Notification]
		NSString WillStartLiveResizeNotification { get; }

		[Field ("NSWindowDidEndLiveResizeNotification")]
		[Notification]
		NSString DidEndLiveResizeNotification { get; }

		[Field ("NSWindowWillEnterFullScreenNotification")]
		[Notification]
		NSString WillEnterFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidEnterFullScreenNotification")]
		[Notification]
		NSString DidEnterFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillExitFullScreenNotification")]
		[Notification]
		NSString WillExitFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidExitFullScreenNotification")]
		[Notification]
		NSString DidExitFullScreenNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillEnterVersionBrowserNotification")]
		[Notification]
		NSString WillEnterVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidEnterVersionBrowserNotification")]
		[Notification]
		NSString DidEnterVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowWillExitVersionBrowserNotification")]
		[Notification]
		NSString WillExitVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidExitVersionBrowserNotification")]
		[Notification]
		NSString DidExitVersionBrowserNotification { get; }

		[Mac (10, 7), Field ("NSWindowDidChangeBackingPropertiesNotification")]
		[Notification (typeof (NSWindowBackingPropertiesEventArgs))]
		NSString DidChangeBackingPropertiesNotification { get; }

		[Mac (10, 12)]
		[Static]
		[Export ("allowsAutomaticWindowTabbing")]
		bool AllowsAutomaticWindowTabbing { get; set; }

		[Mac (10, 12)]
		[Static]
		[Export ("userTabbingPreference")]
		NSWindowUserTabbingPreference UserTabbingPreference { get; }

		[Mac (10, 12)]
		[Export ("tabbingMode", ArgumentSemantic.Assign)]
		NSWindowTabbingMode TabbingMode { get; set; }

		[Mac (10, 12)]
		[Export ("tabbingIdentifier")]
		string TabbingIdentifier { get; set; }

		[Mac (10,12)]
		[Export ("selectNextTab:")]
		void SelectNextTab ([NullAllowed] NSObject sender);

		[Mac (10,12)]
		[Export ("selectPreviousTab:")]
		void SelectPreviousTab ([NullAllowed] NSObject sender);

		[Mac (10,12)]
		[Export ("moveTabToNewWindow:")]
		void MoveTabToNewWindow ([NullAllowed] NSObject sender);

		[Mac (10,12)]
		[Export ("mergeAllWindows:")]
		void MergeAllWindows ([NullAllowed] NSObject sender);

		[Mac (10,12)]
		[Export ("toggleTabBar:")]
		void ToggleTabBar ([NullAllowed] NSObject sender);

		[Mac (10, 12)]
		[NullAllowed, Export ("tabbedWindows", ArgumentSemantic.Copy)]
		NSWindow[] TabbedWindows { get; }

		[Mac (10,12)]
		[Export ("addTabbedWindow:ordered:")]
		void AddTabbedWindow (NSWindow window, NSWindowOrderingMode ordered);

		[Mac (10, 12)]
		[Export ("windowTitlebarLayoutDirection")]
		NSUserInterfaceLayoutDirection WindowTitlebarLayoutDirection { get; }

		[Mac (10,13)]
		[Export ("toggleTabOverview:")]
		void ToggleTabOverview ([NullAllowed] NSObject sender);

		[Mac (10, 13)]
		[Export ("tab", ArgumentSemantic.Strong)]
		NSWindowTab Tab { get; }

		[Mac (10, 13)]
		[NullAllowed, Export ("tabGroup", ArgumentSemantic.Weak)]
		NSWindowTabGroup TabGroup { get; }
	}

	partial interface NSPrintOperation {
		[Mac (10, 7), Export ("preferredRenderingQuality")]
		NSPrintRenderingQuality PreferredRenderingQuality { get; }
	}

	[Category, BaseType (typeof (NSResponder))]
	partial interface NSControlEditingSupport {
		[Mac (10, 7), Export ("validateProposedFirstResponder:forEvent:")]
		bool ValidateProposedFirstResponder (NSResponder responder, [NullAllowed] NSEvent forEvent);
	}

	partial interface NSResponder {
		[Mac (10, 7), Export ("wantsScrollEventsForSwipeTrackingOnAxis:")]
		bool WantsScrollEventsForSwipeTrackingOnAxis (NSEventGestureAxis axis);

		[Mac (10, 7), Export ("supplementalTargetForAction:sender:")]
		NSObject SupplementalTargetForAction (Selector action, [NullAllowed] NSObject sender);

		[Mac (10, 8), Export ("smartMagnifyWithEvent:")]
		void SmartMagnify (NSEvent withEvent);

		[Mac (10, 8), Export ("quickLookWithEvent:")]
		void QuickLook (NSEvent withEvent);
	}

	[Category, BaseType (typeof (NSResponder))]
	partial interface NSStandardKeyBindingMethods {
		[Mac (10, 8), Export ("quickLookPreviewItems:")]
		void QuickLookPreviewItems (NSObject sender);
	}

	[Category, BaseType (typeof (NSView))]
	partial interface NSRulerMarkerClientViewDelegation {
		[Mac (10, 7), Export ("rulerView:locationForPoint:")]
		nfloat RulerViewLocation (NSRulerView ruler, CGPoint locationForPoint);

		[Mac (10, 7), Export ("rulerView:pointForLocation:")]
		CGPoint RulerViewPoint (NSRulerView ruler, nfloat pointForLocation);
	}

	[Category, BaseType (typeof (NSResponder))]
	partial interface NSTextFinderSupport {
		[Mac (10, 7), Export ("performTextFinderAction:")]
		void PerformTextFinderAction ([NullAllowed] NSObject sender);
	}

	partial interface NSRunningApplication {
		[Mac (10, 7), Static, Export ("terminateAutomaticallyTerminableApplications")]
		void TerminateAutomaticallyTerminableApplications ();
	}

	partial interface NSPasteboard {
		[Mac (10, 7), Field ("NSPasteboardTypeTextFinderOptions")]
		NSString PasteboardTypeTextFinderOptions { get; }
	}

	delegate void NSSpellCheckerShowCorrectionIndicatorOfTypeHandler (string acceptedString);

	partial interface NSSpellChecker {
		[Mac (10, 7), Export ("correctionForWordRange:inString:language:inSpellDocumentWithTag:")]
		string GetCorrection (NSRange forWordRange, string inString, string language, nint inSpellDocumentWithTag);

		[Mac (10, 7), Export ("languageForWordRange:inString:orthography:")]
		string GetLanguage (NSRange forWordRange, string inString, NSOrthography orthography);

		[Mac (10, 7), Export ("recordResponse:toCorrection:forWord:language:inSpellDocumentWithTag:")]
		void RecordResponse (NSCorrectionResponse response, string toCorrection, string forWord, string language, nint inSpellDocumentWithTag);

		[Mac (10, 7), Export ("dismissCorrectionIndicatorForView:")]
		void DismissCorrectionIndicator (NSView forView);

		[Mac (10, 7), Export ("showCorrectionIndicatorOfType:primaryString:alternativeStrings:forStringInRect:view:completionHandler:")]
		void ShowCorrectionIndicatorOfType (NSCorrectionIndicatorType type, string primaryString, string [] alternativeStrings,
			CGRect forStringInRect, NSRulerView view, NSSpellCheckerShowCorrectionIndicatorOfTypeHandler completionHandler);

		[Mac (10, 7), Static, Export ("isAutomaticTextReplacementEnabled")]
		bool IsAutomaticTextReplacementEnabled { get; }

		[Mac (10, 7), Static, Export ("isAutomaticSpellingCorrectionEnabled")]
		bool IsAutomaticSpellingCorrectionEnabled { get; }

		[Field ("NSTextCheckingOrthographyKey")]
		NSString TextCheckingOrthographyKey { get; }

		[Field ("NSTextCheckingQuotesKey")]
		NSString TextCheckingQuotesKey { get; }

		[Field ("NSTextCheckingReplacementsKey")]
		NSString TextCheckingReplacementsKey { get; }

		[Field ("NSTextCheckingReferenceDateKey")]
		NSString TextCheckingReferenceDateKey { get; }

		[Field ("NSTextCheckingReferenceTimeZoneKey")]
		NSString TextCheckingReferenceTimeZoneKey { get; }

		[Field ("NSTextCheckingDocumentURLKey")]
		NSString TextCheckingDocumentURLKey { get; }

		[Field ("NSTextCheckingDocumentTitleKey")]
		NSString TextCheckingDocumentTitleKey { get; }

		[Field ("NSTextCheckingDocumentAuthorKey")]
		NSString TextCheckingDocumentAuthorKey { get; }

		[Mac (10, 7), Field ("NSTextCheckingRegularExpressionsKey")]
		NSString TextCheckingRegularExpressionsKey { get; }

		[Mac (10, 7), Notification, Field ("NSSpellCheckerDidChangeAutomaticSpellingCorrectionNotification")]
		NSString DidChangeAutomaticSpellingCorrectionNotification { get; }

		[Mac (10, 7), Notification, Field ("NSSpellCheckerDidChangeAutomaticTextReplacementNotification")]
		NSString DidChangeAutomaticTextReplacementNotification { get; }

		[Mac (10, 12)]
		[Field ("NSTextCheckingSelectedRangeKey")]
		NSString TextCheckingSelectedRangeKey { get; }

		[Mac (10, 12)]
		[Field ("NSSpellCheckerDidChangeAutomaticCapitalizationNotification")]
		[Notification]
		NSString DidChangeAutomaticCapitalizationNotification { get; }

		[Mac (10, 12)]
		[Field ("NSSpellCheckerDidChangeAutomaticPeriodSubstitutionNotification")]
		[Notification]
		NSString DidChangeAutomaticPeriodSubstitutionNotification { get; }

		[Mac (10, 12, 2)]
		[Field ("NSSpellCheckerDidChangeAutomaticTextCompletionNotification")]
		[Notification]
		NSString DidChangeAutomaticTextCompletionNotification { get; }
	}

	partial interface NSTextViewDidChangeSelectionEventArgs {
		// FIXME: verify property type "NSValue object containing an NSRange structure"
		[Export ("NSOldSelectedCharacterRange")]
		NSValue OldSelectedCharacterRange { get; }
	}

	partial interface NSTextViewWillChangeNotifyingTextViewEventArgs {
		[Export ("NSOldNotifyingTextView")]
		NSTextView OldView { get; }

		[Export ("NSNewNotifyingTextView")]
		NSTextView NewView { get; }
	}

	partial interface NSTextView : NSTextLayoutOrientationProvider {
		[Mac (10, 7), Export ("setLayoutOrientation:")]
		void SetLayoutOrientation (NSTextLayoutOrientation theOrientation);

		[Mac (10, 7), Export ("changeLayoutOrientation:")]
		void ChangeLayoutOrientation (NSObject sender);

		[Mac (10, 7), Export ("usesInspectorBar")]
		bool UsesInspectorBar { get; set; }

		[Mac (10, 7), Export ("usesFindBar")]
		bool UsesFindBar { get; set; }

		[Mac (10, 7), Export ("incrementalSearchingEnabled")]
		bool IsIncrementalSearchingEnabled {[Bind ("isIncrementalSearchingEnabled")]get; set; }

		[Mac (10, 7), Export ("quickLookPreviewableItemsInRanges:")]
		NSArray QuickLookPreviewableItemsInRanges (NSArray ranges);

		[Mac (10, 7), Export ("updateQuickLookPreviewPanel")]
		void UpdateQuickLookPreviewPanel ();

		[Notification (typeof (NSTextViewWillChangeNotifyingTextViewEventArgs))]
		[Field ("NSTextViewWillChangeNotifyingTextViewNotification")]
		NSString WillChangeNotifyingTextViewNotification { get; }

		[Notification (typeof (NSTextViewDidChangeSelectionEventArgs))]
		[Field ("NSTextViewDidChangeSelectionNotification")]
		NSString DidChangeSelectionNotification { get; }

		[Notification, Field ("NSTextViewDidChangeTypingAttributesNotification")]
		NSString DidChangeTypingAttributesNotification { get; }
	}

	partial interface NSView {

		[Mac (10, 8), Export ("wantsUpdateLayer")]
		bool WantsUpdateLayer { get; }

		[Mac (10, 8), Export ("updateLayer")]
		void UpdateLayer ();

		[Mac (10, 8), Export ("rectForSmartMagnificationAtPoint:inRect:")]
		CGRect RectForSmartMagnificationAtPoint (CGPoint atPoint, CGRect inRect);
	}

#if !XAMCORE_4_0
	[Category, BaseType (typeof (NSApplication))]
	partial interface NSRemoteNotifications_NSApplication {

		[Mac (10, 8), Field ("NSApplicationLaunchUserNotificationKey", "AppKit")]
		NSString NSApplicationLaunchUserNotificationKey { get; }
	}
#endif

	partial interface NSControlTextEditingEventArgs {
		[Export ("NSFieldEditor")]
		NSTextView FieldEditor { get; }
	}

	partial interface NSControl {

		[Notification (typeof (NSControlTextEditingEventArgs))]
		[Field ("NSControlTextDidBeginEditingNotification")]
		NSString TextDidBeginEditingNotification { get; }

		[Notification (typeof (NSControlTextEditingEventArgs))]
		[Field ("NSControlTextDidEndEditingNotification")]
		NSString TextDidEndEditingNotification { get; }

		[Notification (typeof (NSControlTextEditingEventArgs))]
		[Field ("NSControlTextDidChangeNotification")]
		NSString TextDidChangeNotification { get; }

		[Mac (10, 8), Export ("allowsExpansionToolTips")]
		bool AllowsExpansionToolTips { get; set; }
	}

	partial interface NSMatrix {

		[Mac (10, 8), Export ("autorecalculatesCellSize")]
		bool AutoRecalculatesCellSize { get; set; }
	}

	partial interface NSForm {

		[Mac (10, 8), Export ("preferredTextFieldWidth")]
		nfloat PreferredTextFieldWidth { get; set; }
	}

	partial interface NSFormCell {

		[Mac (10, 8), Export ("preferredTextFieldWidth")]
		nfloat PreferredTextFieldWidth { get; set; }
	}

	partial interface NSColor {

		[Mac (10, 8), Static, Export ("underPageBackgroundColor")]
		NSColor UnderPageBackgroundColor { get; }

		[Mac (10, 8), Static, Export ("colorWithCGColor:")]
		NSColor FromCGColor (CGColor cgColor);
	}

	delegate bool NSCustomImageRepDrawingHandler (CGRect dstRect);

	partial interface NSCustomImageRep {

		[Mac (10, 8), Export ("initWithSize:flipped:drawingHandler:")]
		IntPtr Constructor (CGSize size, bool flipped, NSCustomImageRepDrawingHandler drawingHandler);

		[Mac (10, 8), Export ("drawingHandler")]
		NSCustomImageRepDrawingHandler DrawingHandler { get; }
	}

	delegate void NSDocumentMoveCompletionHandler (bool didMove);
	delegate void NSDocumentMoveToUrlCompletionHandler (NSError error);
	delegate void NSDocumentLockDocumentCompletionHandler (bool didLock);
	delegate void NSDocumentUnlockDocumentCompletionHandler (bool didUnlock);
	delegate void NSDocumentLockCompletionHandler (NSError error);
	delegate void NSDocumentUnlockCompletionHandler (NSError error);

	partial interface NSDocument {

		[Mac (10, 8), Export ("draft")]
		bool IsDraft { [Bind ("isDraft")] get; set; }

		[Mac (10, 8), Export ("backupFileURL")]
		NSUrl BackupFileUrl { get; }

		[Mac (10, 8), Export ("browseDocumentVersions:")]
		void BrowseDocumentVersions (NSObject sender);

		[Mac (10, 8), Static, Export ("autosavesDrafts")]
		bool AutoSavesDrafts { get; }

		[Mac (10, 8), Export ("renameDocument:")]
		void RenameDocument (NSObject sender);

		[Mac (10, 8), Export ("moveDocumentToUbiquityContainer:")]
		void MoveDocumentToUbiquityContainer (NSObject sender);

		[Mac (10, 8), Export ("moveDocument:")]
		void MoveDocument (NSObject sender);

		[Mac (10, 8), Export ("moveDocumentWithCompletionHandler:")]
		void MoveDocumentWithCompletionHandler (NSDocumentMoveCompletionHandler completionHandler);

		[Mac (10, 8), Export ("moveToURL:completionHandler:")]
		void MoveToUrl (NSUrl url, NSDocumentMoveToUrlCompletionHandler completionHandler);

		[Mac (10, 8), Export ("lockDocument:")]
		void LockDocument (NSObject sender);

		[Mac (10, 8), Export ("unlockDocument:")]
		void UnlockDocument (NSObject sender);

		[Mac (10, 8), Export ("lockDocumentWithCompletionHandler:")]
		void LockDocumentWithCompletionHandler (NSDocumentLockDocumentCompletionHandler completionHandler);

		[Mac (10, 8), Export ("lockWithCompletionHandler:")]
		void LockWithCompletionHandler (NSDocumentLockCompletionHandler completionHandler);

		[Mac (10, 8), Export ("unlockDocumentWithCompletionHandler:")]
		void UnlockDocumentWithCompletionHandler (NSDocumentUnlockDocumentCompletionHandler completionHandler);

		[Mac (10, 8), Export ("unlockWithCompletionHandler:")]
		void UnlockWithCompletionHandler (NSDocumentUnlockCompletionHandler completionHandler);

		[Mac (10, 8), Export ("isLocked")]
		bool IsLocked { get; }

		[Mac (10, 8), Export ("defaultDraftName")]
		string DefaultDraftName { get; }

		[Mac (10, 8), Static, Export ("usesUbiquitousStorage")]
		bool UsesUbiquitousStorage { get; }

		[Mac (10,13)]
		[Export ("encodeRestorableStateWithCoder:backgroundQueue:")]
		void EncodeRestorableState (NSCoder coder, NSOperationQueue queue);
	}

	delegate void NSDocumentControllerOpenPanelWithCompletionHandler (NSArray urlsToOpen);
	delegate void NSDocumentControllerOpenPanelResultHandler (nint result);

	partial interface NSDocumentController {

		[Mac (10, 8), Export ("beginOpenPanelWithCompletionHandler:")]
		void BeginOpenPanelWithCompletionHandler (NSDocumentControllerOpenPanelWithCompletionHandler completionHandler);

		[Mac (10, 8), Export ("beginOpenPanel:forTypes:completionHandler:")]
		void BeginOpenPanel (NSOpenPanel openPanel, NSArray inTypes, NSDocumentControllerOpenPanelResultHandler completionHandler);

		[Mac (10, 13)]
		[Export ("allowsAutomaticShareMenu")]
		bool AllowsAutomaticShareMenu { get; }

		[Mac (10, 13)]
		[Export ("standardShareMenuItem")]
		NSMenuItem StandardShareMenuItem { get; }
	}

	partial interface NSImage {

		[Mac (10, 8), Static, Export ("imageWithSize:flipped:drawingHandler:")]
#if XAMCORE_2_0
		NSImage ImageWithSize (CGSize size, bool flipped, NSCustomImageRepDrawingHandler drawingHandler);
#else
		NSObject ImageWithSize (CGSize size, bool flipped, NSCustomImageRepDrawingHandler drawingHandler);
#endif
	}

	partial interface NSNib {

		[Mac (10, 8), Export ("initWithNibData:bundle:")]
		IntPtr Constructor (NSData nibData, NSBundle bundle);
	}

	partial interface NSSplitViewDividerIndexEventArgs {
		// FIXME: The generator can't handle Nullable<int>, and
		// the key may or may not exist; if it doesn't exist, then
		// the generator will have this property always return 0,
		// which may actually be a valid index value. Either the
		// generator needs to support nullable, or ProbePresence
		// on non-boolean property types.
		//
		// [Export ("NSSplitViewDividerIndex")]
		// int? DividerIndex { get; }
	}

	[Category, BaseType (typeof (NSSegmentedCell))]
	partial interface NSSegmentBackgroundStyle_NSSegmentedCell {

		[Mac (10, 8), Field ("NSSharingServiceNamePostOnFacebook")]
		NSString SharingServiceNamePostOnFacebook { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostOnTwitter")]
		NSString SharingServiceNamePostOnTwitter { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostOnSinaWeibo")]
		NSString SharingServiceNamePostOnSinaWeibo { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameComposeEmail")]
		NSString SharingServiceNameComposeEmail { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameComposeMessage")]
		NSString SharingServiceNameComposeMessage { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameSendViaAirDrop")]
		NSString SharingServiceNameSendViaAirDrop { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameAddToSafariReadingList")]
		NSString SharingServiceNameAddToSafariReadingList { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameAddToIPhoto")]
		NSString SharingServiceNameAddToIPhoto { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameAddToAperture")]
		NSString SharingServiceNameAddToAperture { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameUseAsTwitterProfileImage")]
		NSString SharingServiceNameUseAsTwitterProfileImage { get; }

		[Mac (10, 8), Field ("NSSharingServiceNameUseAsDesktopPicture")]
		NSString SharingServiceNameUseAsDesktopPicture { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostImageOnFlickr")]
		NSString SharingServiceNamePostImageOnFlickr { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostVideoOnVimeo")]
		NSString SharingServiceNamePostVideoOnVimeo { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostVideoOnYouku")]
		NSString SharingServiceNamePostVideoOnYouku { get; }

		[Mac (10, 8), Field ("NSSharingServiceNamePostVideoOnTudou")]
		NSString SharingServiceNamePostVideoOnTudou { get; }
	}

	[Category, BaseType (typeof (NSTextView))]
	partial interface NSTextView_SharingService {

		[Mac (10, 8), Export ("orderFrontSharingServicePicker:")]
		void OrderFrontSharingServicePicker (NSObject sender);
	}

	/*partial interface NSTextViewDelegate {

		[Mac (10, 8), Export ("textView:willShowSharingServicePicker:forItems:"), DelegateName (...)]
		NSSharingServicePicker WillShowSharingService (NSTextView textView,
			NSSharingServicePicker servicePicker, NSArray forItems);
	}*/

	interface NSTextAlternativesSelectedAlternativeStringEventArgs {
		[Export ("NSAlternativeString")]
		string AlternativeString { get; }
	}

	[Mac (10, 8), BaseType (typeof (NSObject))]
	partial interface NSTextAlternatives {

		[Export ("initWithPrimaryString:alternativeStrings:")]
		IntPtr Constructor (string primaryString, NSArray alternativeStrings);

		[Export ("primaryString", ArgumentSemantic.Copy)]
		string PrimaryString { get; }

		[Export ("alternativeStrings", ArgumentSemantic.Copy)]
		NSArray AlternativeStrings { get; }

		[Export ("noteSelectedAlternativeString:")]
		void NoteSelectedAlternativeString (string alternativeString);

		[Mac (10, 8), Notification (typeof (NSTextAlternativesSelectedAlternativeStringEventArgs)),
			Field ("NSTextAlternativesSelectedAlternativeStringNotification")]
		NSString SelectedAlternativeStringNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	partial interface NSGlyphInfo : NSCoding, NSCopying, NSSecureCoding {

		[Static, Export ("glyphInfoWithGlyphName:forFont:baseString:")]
		NSGlyphInfo Get (string glyphName, NSFont forFont, string baseString);

		[Static, Export ("glyphInfoWithGlyph:forFont:baseString:")]
		NSGlyphInfo Get (uint /* NSGlyph = unsigned int */ glyph, NSFont forFont, string baseString);

		[Static, Export ("glyphInfoWithCharacterIdentifier:collection:baseString:")]
		NSGlyphInfo Get (nuint characterIdentifier, NSCharacterCollection characterCollection, string baseString);

		[Export ("glyphName")]
		string GlyphName { get; }

		[Export ("characterIdentifier")]
		nuint CharacterIdentifier { get; }

		[Export ("characterCollection")]
		NSCharacterCollection CharacterCollection { get; }

		[Mac (10,13)]
		[Static]
		[Export ("glyphInfoWithCGGlyph:forFont:baseString:")]
		[return: NullAllowed]
		NSGlyphInfo GetGlyphInfo (ushort glyph, NSFont font, string @string);

		[Mac (10, 13)]
		[Export ("glyphID")]
		ushort GlyphId { get; }

		[Mac (10, 13)]
		[Export ("baseString")]
		string BaseString { get; }
	}

	partial interface NSTableViewDelegate {

		[Export ("tableView:toolTipForCell:rect:tableColumn:row:mouseLocation:"), DelegateName ("NSTableViewToolTip"), DefaultValue ("null")]
		NSString GetToolTip (NSTableView tableView, NSCell cell, ref CGRect rect, NSTableColumn tableColumn, nint row, CGPoint mouseLocation);
	}

	partial interface NSBrowser {
		[Notification, Field ("NSBrowserColumnConfigurationDidChangeNotification")]
		NSString ColumnConfigurationChangedNotification { get; }
	}

	partial interface NSColorPanel {
		[Notification, Field ("NSColorPanelColorDidChangeNotification")]
		NSString ColorChangedNotification { get; }
	}

	partial interface NSFont {
		[Notification, Field ("NSAntialiasThresholdChangedNotification")]
		NSString AntialiasThresholdChangedNotification { get; }

		[Notification, Field ("NSFontSetChangedNotification")]
		NSString FontSetChangedNotification { get; }
	}

	partial interface NSHelpManager {
		[Notification, Field ("NSContextHelpModeDidActivateNotification")]
		NSString ContextHelpModeDidActivateNotification { get; }

		[Notification, Field ("NSContextHelpModeDidDeactivateNotification")]
		NSString ContextHelpModeDidDeactivateNotification { get; }
	}

	partial interface NSDrawer {
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
		[Notification, Field ("NSDrawerWillOpenNotification")]
		NSString WillOpenNotification { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
		[Notification, Field ("NSDrawerDidOpenNotification")]
		NSString DidOpenNotification { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
		[Notification, Field ("NSDrawerWillCloseNotification")]
		NSString WillCloseNotification { get; }

		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSSplitViewController' instead.")]
		[Notification, Field ("NSDrawerDidCloseNotification")]
		NSString DidCloseNotification { get; }
	}

	partial interface NSMenuItemIndexEventArgs {
		[Export ("NSMenuItemIndex")]
		nint MenuItemIndex { get; }
	}

	partial interface NSMenuItemEventArgs {
		[Export ("MenuItem")]
		NSMenu MenuItem { get; }
	}

	partial interface NSMenu {
		[Notification (typeof (NSMenuItemEventArgs))]
		[Field ("NSMenuWillSendActionNotification")]
		NSString WillSendActionNotification { get; }

		[Notification (typeof (NSMenuItemEventArgs))]
		[Field ("NSMenuDidSendActionNotification")]
		NSString DidSendActionNotification { get; }

		[Notification (typeof (NSMenuItemIndexEventArgs))]
		[Field ("NSMenuDidAddItemNotification")]
		NSString DidAddItemNotification { get; }

		[Notification (typeof (NSMenuItemIndexEventArgs))]
		[Field ("NSMenuDidRemoveItemNotification")]
		NSString DidRemoveItemNotification { get; }

		[Notification (typeof (NSMenuItemIndexEventArgs))]
		[Field ("NSMenuDidChangeItemNotification")]
		NSString DidChangeItemNotification { get; }

		[Notification, Field ("NSMenuDidBeginTrackingNotification")]
		NSString DidBeginTrackingNotification { get; }

		[Notification, Field ("NSMenuDidEndTrackingNotification")]
		NSString DidEndTrackingNotification { get; }
	}

	partial interface NSPopUpButtonCell {
		[Notification, Field ("NSPopUpButtonCellWillPopUpNotification")]
		NSString WillPopUpNotification { get; }
	}

	partial interface NSPopUpButton {
		[Notification, Field ("NSPopUpButtonWillPopUpNotification")]
		NSString WillPopUpNotification { get; }
	}

	partial interface NSRuleEditor {
		[Notification, Field ("NSRuleEditorRowsDidChangeNotification")]
		NSString RowsDidChangeNotification { get; }
	}

	partial interface NSScreen {
		[Notification, Field ("NSScreenColorSpaceDidChangeNotification")]
		NSString ColorSpaceDidChangeNotification { get; }
	}

	partial interface NSTableView {
		[Notification, Field ("NSTableViewSelectionDidChangeNotification")]
		NSString SelectionDidChangeNotification { get; }

		[Notification, Field ("NSTableViewSelectionIsChangingNotification")]
		NSString SelectionIsChangingNotification { get; }

		[Notification (typeof (NSViewColumnMoveEventArgs))]
		[Field ("NSTableViewColumnDidMoveNotification")]
		NSString ColumnDidMoveNotification { get; }

		[Notification (typeof (NSViewColumnResizeEventArgs))]
		[Field ("NSTableViewColumnDidResizeNotification")]
		NSString ColumnDidResizeNotification { get; }
	}

	partial interface NSTextDidEndEditingEventArgs {
		// FIXME: I think this is essentially a flags value
		// of movements and characters. The docs are a bit
		// confusing.
		[Export ("NSTextMovement")]
		nint Movement { get; }
	}

	partial interface NSText {
		[Notification, Field ("NSTextDidBeginEditingNotification")]
		NSString DidBeginEditingNotification { get; }

		[Notification (typeof (NSTextDidEndEditingEventArgs))]
		[Field ("NSTextDidEndEditingNotification")]
		NSString DidEndEditingNotification { get; }

		[Notification, Field ("NSTextDidChangeNotification")]
		NSString DidChangeNotification { get; }

		[Mac (10, 13)]
		[Field ("NSTextMovementUserInfoKey")]
		NSString MovementUserInfoKey { get; }
	}

	partial interface NSTextInputContext {
		[Notification, Field ("NSTextInputContextKeyboardSelectionDidChangeNotification")]
		NSString KeyboardSelectionDidChangeNotification { get; }
	}

	partial interface NSTextStorage {
		[Notification, Field ("NSTextStorageWillProcessEditingNotification")]
		NSString WillProcessEditingNotification { get; }

		[Notification, Field ("NSTextStorageDidProcessEditingNotification")]
		NSString DidProcessEditingNotification { get; }
	}

	partial interface NSToolbarItemEventArgs {
		[Export ("item")]
		NSToolbarItem Item { get; }
	}

	partial interface NSToolbar {
		[Notification (typeof (NSToolbarItemEventArgs))]
		[Field ("NSToolbarWillAddItemNotification")]
		NSString NSToolbarWillAddItemNotification { get; }

		[Notification (typeof (NSToolbarItemEventArgs))]
		[Field ("NSToolbarDidRemoveItemNotification")]
		NSString NSToolbarDidRemoveItemNotification { get; }
	}

	partial interface NSImageRep {
		[Notification, Field ("NSImageRepRegistryDidChangeNotification")]
		NSString RegistryDidChangeNotification { get; }
	}

	interface INSAccessibility {};
	interface INSAccessibilityElement {};

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibility
	{
		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityFrame", ArgumentSemantic.Assign)]
		CGRect AccessibilityFrame { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityIdentifier")]
		string AccessibilityIdentifier { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityParent", ArgumentSemantic.Weak)]
		NSObject AccessibilityParent { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityFocused")]
		bool AccessibilityFocused { [Bind ("isAccessibilityFocused")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityElement")]
		bool AccessibilityElement { [Bind ("isAccessibilityElement")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityActivationPoint", ArgumentSemantic.Assign)]
		CGPoint AccessibilityActivationPoint { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityTopLevelUIElement", ArgumentSemantic.Weak)]
		NSObject AccessibilityTopLevelUIElement { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityURL", ArgumentSemantic.Copy)]
		NSUrl AccessibilityUrl { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityValue", ArgumentSemantic.Strong)]
		NSObject AccessibilityValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityValueDescription")]
		string AccessibilityValueDescription { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVisibleChildren", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityVisibleChildren { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySubrole")]
		string AccessibilitySubrole { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityTitle")]
		string AccessibilityTitle { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityTitleUIElement", ArgumentSemantic.Weak)]
		NSObject AccessibilityTitleUIElement { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityNextContents", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityNextContents { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityOrientation", ArgumentSemantic.Assign)]
		NSAccessibilityOrientation AccessibilityOrientation { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityOverflowButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityOverflowButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityPlaceholderValue")]
		string AccessibilityPlaceholderValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityPreviousContents", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityPreviousContents { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityRole")]
		string AccessibilityRole { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityRoleDescription")]
		string AccessibilityRoleDescription { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySearchButton", ArgumentSemantic.Strong)]
		NSObject AccessibilitySearchButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySearchMenu", ArgumentSemantic.Strong)]
		NSObject AccessibilitySearchMenu { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilitySelected")]
		bool AccessibilitySelected { [Bind ("isAccessibilitySelected")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedChildren", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySelectedChildren { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityServesAsTitleForUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityServesAsTitleForUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityShownMenu", ArgumentSemantic.Strong)]
		NSObject AccessibilityShownMenu { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMinValue", ArgumentSemantic.Strong)]
		NSObject AccessibilityMinValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMaxValue", ArgumentSemantic.Strong)]
		NSObject AccessibilityMaxValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityLinkedUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityLinkedUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityWindow", ArgumentSemantic.Weak)]
		NSObject AccessibilityWindow { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityHelp")]
		string AccessibilityHelp { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityFilename")]
		string AccessibilityFilename { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityExpanded")]
		bool AccessibilityExpanded { [Bind ("isAccessibilityExpanded")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityEdited")]
		bool AccessibilityEdited { [Bind ("isAccessibilityEdited")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityEnabled")]
		bool AccessibilityEnabled { [Bind ("isAccessibilityEnabled")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityChildren", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityChildren { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityClearButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityClearButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityCancelButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityCancelButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityProtectedContent")]
		bool AccessibilityProtectedContent { [Bind ("isAccessibilityProtectedContent")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityContents", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityContents { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityAlternateUIVisible")]
		bool AccessibilityAlternateUIVisible { [Bind ("isAccessibilityAlternateUIVisible")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySharedFocusElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySharedFocusElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityApplicationFocusedUIElement", ArgumentSemantic.Strong)]
		NSObject AccessibilityApplicationFocusedUIElement { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMainWindow", ArgumentSemantic.Strong)]
		NSObject AccessibilityMainWindow { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityHidden")]
		bool AccessibilityHidden { [Bind ("isAccessibilityHidden")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityFrontmost")]
		bool AccessibilityFrontmost { [Bind ("isAccessibilityFrontmost")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityFocusedWindow", ArgumentSemantic.Strong)]
		NSObject AccessibilityFocusedWindow { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityWindows", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityWindows { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityExtrasMenuBar", ArgumentSemantic.Weak)]
		NSObject AccessibilityExtrasMenuBar { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMenuBar", ArgumentSemantic.Weak)]
		NSObject AccessibilityMenuBar { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityColumnTitles", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityColumnTitles { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityOrderedByRow")]
		bool AccessibilityOrderedByRow { [Bind ("isAccessibilityOrderedByRow")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityHorizontalUnits", ArgumentSemantic.Assign)]
		NSAccessibilityUnits AccessibilityHorizontalUnits { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityVerticalUnits", ArgumentSemantic.Assign)]
		NSAccessibilityUnits AccessibilityVerticalUnits { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityHorizontalUnitDescription")]
		string AccessibilityHorizontalUnitDescription { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVerticalUnitDescription")]
		string AccessibilityVerticalUnitDescription { get; set; }

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityLayoutPointForScreenPoint:")]
		CGPoint GetAccessibilityLayoutForScreen (CGPoint point);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityLayoutSizeForScreenSize:")]
		CGSize GetAccessibilityLayoutForScreen (CGSize size);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityScreenPointForLayoutPoint:")]
		CGPoint GetAccessibilityScreenForLayout (CGPoint point);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityScreenSizeForLayoutSize:")]
		CGSize GetAccessibilityScreenForLayout (CGSize size);

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityHandles", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityHandles { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityWarningValue", ArgumentSemantic.Strong)]
		NSObject AccessibilityWarningValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityCriticalValue", ArgumentSemantic.Strong)]
		NSObject AccessibilityCriticalValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityDisclosed")]
		bool AccessibilityDisclosed { [Bind ("isAccessibilityDisclosed")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityDisclosedByRow", ArgumentSemantic.Weak)]
		NSObject AccessibilityDisclosedByRow { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityDisclosedRows", ArgumentSemantic.Strong)]
		NSObject AccessibilityDisclosedRows { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityDisclosureLevel")]
		nint AccessibilityDisclosureLevel { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMarkerUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityMarkerUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMarkerValues", ArgumentSemantic.Strong)]
		NSObject AccessibilityMarkerValues { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMarkerGroupUIElement", ArgumentSemantic.Strong)]
		NSObject AccessibilityMarkerGroupUIElement { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityUnits", ArgumentSemantic.Assign)]
		NSAccessibilityUnits AccessibilityUnits { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityUnitDescription")]
		string AccessibilityUnitDescription { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityRulerMarkerType", ArgumentSemantic.Assign)]
		NSAccessibilityRulerMarkerType AccessibilityRulerMarkerType { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMarkerTypeDescription")]
		string AccessibilityMarkerTypeDescription { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityHorizontalScrollBar", ArgumentSemantic.Strong)]
		NSObject AccessibilityHorizontalScrollBar { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVerticalScrollBar", ArgumentSemantic.Strong)]
		NSObject AccessibilityVerticalScrollBar { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityAllowedValues", ArgumentSemantic.Copy)]
		NSNumber[] AccessibilityAllowedValues { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityLabelUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityLabelUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityLabelValue")]
		float AccessibilityLabelValue { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySplitters", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySplitters { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityDecrementButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityDecrementButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityIncrementButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityIncrementButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityTabs", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityTabs { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityHeader", ArgumentSemantic.Strong)]
		NSObject AccessibilityHeader { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityColumnCount")]
		nint AccessibilityColumnCount { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityRowCount")]
		nint AccessibilityRowCount { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityIndex")]
		nint AccessibilityIndex { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityColumns", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityColumns { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityRows", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityRows { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVisibleRows", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityVisibleRows { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedRows", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySelectedRows { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVisibleColumns", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityVisibleColumns { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedColumns", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySelectedColumns { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilitySortDirection", ArgumentSemantic.Assign)]
		NSAccessibilitySortDirection AccessibilitySortDirection { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityRowHeaderUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityRowHeaderUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedCells", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySelectedCells { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityVisibleCells", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityVisibleCells { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityColumnHeaderUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilityColumnHeaderUIElements { get; set; }

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityCellForColumn:row:")]
		[return: NullAllowed]
		NSObject GetAccessibilityCellForColumn (nint column, nint row);

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityRowIndexRange", ArgumentSemantic.Assign)]
		NSRange AccessibilityRowIndexRange { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityColumnIndexRange", ArgumentSemantic.Assign)]
		NSRange AccessibilityColumnIndexRange { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityInsertionPointLineNumber")]
		nint AccessibilityInsertionPointLineNumber { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilitySharedCharacterRange", ArgumentSemantic.Assign)]
		NSRange AccessibilitySharedCharacterRange { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySharedTextUIElements", ArgumentSemantic.Copy)]
		NSObject[] AccessibilitySharedTextUIElements { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityVisibleCharacterRange", ArgumentSemantic.Assign)]
		NSRange AccessibilityVisibleCharacterRange { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityNumberOfCharacters")]
		nint AccessibilityNumberOfCharacters { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedText")]
		string AccessibilitySelectedText { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilitySelectedTextRange", ArgumentSemantic.Assign)]
		NSRange AccessibilitySelectedTextRange { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedTextRanges", ArgumentSemantic.Copy)]
		NSValue[] AccessibilitySelectedTextRanges { get; set; }

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityAttributedStringForRange:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedString (NSRange range);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityRangeForLine:")]
		NSRange GetAccessibilityRangeForLine (nint line);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityStringForRange:")]
		[return: NullAllowed]
		string GetAccessibilityString (NSRange range);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityRangeForPosition:")]
		NSRange GetAccessibilityRange (CGPoint point);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityRangeForIndex:")]
		NSRange GetAccessibilityRange (nint index);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityFrameForRange:")]
		CGRect GetAccessibilityFrame (NSRange range);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityRTFForRange:")]
		[return: NullAllowed]
		NSData GetAccessibilityRtf (NSRange range);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityStyleRangeForIndex:")]
		NSRange GetAccessibilityStyleRange (nint index);

		[Mac (10,10)]
		[Abstract]
		[Export ("accessibilityLineForIndex:")]
		nint GetAccessibilityLine (nint index);

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityToolbarButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityToolbarButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityModal")]
		bool AccessibilityModal { [Bind ("isAccessibilityModal")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityProxy", ArgumentSemantic.Strong)]
		NSObject AccessibilityProxy { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityMain")]
		bool AccessibilityMain { [Bind ("isAccessibilityMain")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityFullScreenButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityFullScreenButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityGrowArea", ArgumentSemantic.Strong)]
		NSObject AccessibilityGrowArea { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityDocument")]
		string AccessibilityDocument { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityDefaultButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityDefaultButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityCloseButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityCloseButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityZoomButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityZoomButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[NullAllowed, Export ("accessibilityMinimizeButton", ArgumentSemantic.Strong)]
		NSObject AccessibilityMinimizeButton { get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityMinimized")]
		bool AccessibilityMinimized { [Bind ("isAccessibilityMinimized")] get; set; }

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformCancel")]
		bool AccessibilityPerformCancel ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformConfirm")]
		bool AccessibilityPerformConfirm ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformDecrement")]
		bool AccessibilityPerformDecrement ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformDelete")]
		bool AccessibilityPerformDelete ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformIncrement")]
		bool AccessibilityPerformIncrement ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformPick")]
		bool AccessibilityPerformPick ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformPress")]
		bool AccessibilityPerformPress ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformRaise")]
		bool AccessibilityPerformRaise ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformShowAlternateUI")]
		bool AccessibilityPerformShowAlternateUI ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformShowDefaultUI")]
		bool AccessibilityPerformShowDefaultUI ();

		[Mac (10, 10)]
		[Abstract]
		[Export ("accessibilityPerformShowMenu")]
		bool AccessibilityPerformShowMenu ();

		[Mac (10,10)]
		[Abstract]
		[Export ("isAccessibilitySelectorAllowed:")]
		bool IsAccessibilitySelectorAllowed (Selector selector);

		[Mac (10, 12)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("accessibilityRequired")]
		bool AccessibilityRequired { [Bind ("isAccessibilityRequired")] get; set; }

		[Notification]
		[Field ("NSAccessibilityMainWindowChangedNotification")]
		NSString MainWindowChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityFocusedWindowChangedNotification")]
		NSString FocusedWindowChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityFocusedUIElementChangedNotification")]
		NSString UIElementFocusedChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityApplicationActivatedNotification")]
		NSString ApplicationActivatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityApplicationDeactivatedNotification")]
		NSString ApplicationDeactivatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityApplicationHiddenNotification")]
		NSString ApplicationHiddenNotification { get; }

		[Notification]
		[Field ("NSAccessibilityApplicationShownNotification")]
		NSString ApplicationShownNotification { get; }

		[Notification]
		[Field ("NSAccessibilityWindowCreatedNotification")]
		NSString WindowCreatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityWindowMovedNotification")]
		NSString WindowMovedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityWindowResizedNotification")]
		NSString WindowResizedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityWindowMiniaturizedNotification")]
		NSString WindowMiniaturizedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityWindowDeminiaturizedNotification")]
		NSString WindowDeminiaturizedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityDrawerCreatedNotification")]
		NSString DrawerCreatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySheetCreatedNotification")]
		NSString SheetCreatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityUIElementDestroyedNotification")]
		NSString UIElementDestroyedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityValueChangedNotification")]
		NSString ValueChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityTitleChangedNotification")]
		NSString TitleChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityResizedNotification")]
		NSString ResizedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityMovedNotification")]
		NSString MovedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityCreatedNotification")]
		NSString CreatedNotification { get; }

		[Mac (10, 9)]
		[Notification]
		[Field ("NSAccessibilityLayoutChangedNotification")]
		NSString LayoutChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityHelpTagCreatedNotification")]
		NSString HelpTagCreatedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedTextChangedNotification")]
		NSString SelectedTextChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityRowCountChangedNotification")]
		NSString RowCountChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedChildrenChangedNotification")]
		NSString SelectedChildrenChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedRowsChangedNotification")]
		NSString SelectedRowsChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedColumnsChangedNotification")]
		NSString SelectedColumnsChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityRowExpandedNotification")]
		NSString RowExpandedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityRowCollapsedNotification")]
		NSString RowCollapsedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedCellsChangedNotification")]
		NSString SelectedCellsChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilityUnitsChangedNotification")]
		NSString UnitsChangedNotification { get; }

		[Notification]
		[Field ("NSAccessibilitySelectedChildrenMovedNotification")]
		NSString SelectedChildrenMovedNotification { get; }

		[Mac (10, 7)]
		[Notification]
		[Field ("NSAccessibilityAnnouncementRequestedNotification")]
		NSString AnnouncementRequestedNotification { get; }

		[Mac (10, 13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("accessibilityChildrenInNavigationOrder", ArgumentSemantic.Copy)]
		NSAccessibilityElement[] AccessibilityChildrenInNavigationOrder { get; set; }

		[Mac (10, 13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[Export ("accessibilityCustomRotors", ArgumentSemantic.Copy)]
		NSAccessibilityCustomRotor[] AccessibilityCustomRotors { get; set; }

		[Mac (10, 13)]
#if XAMCORE_4_0
		[Abstract]
#endif
		[NullAllowed, Export ("accessibilityCustomActions", ArgumentSemantic.Copy)]
		NSAccessibilityCustomAction[] AccessibilityCustomActions { get; set; }
	}

	[Protocol]
	interface NSCollectionViewSectionHeaderView : NSCollectionViewElement
	{
		[Mac (10, 12)]
		[NullAllowed, Export ("sectionCollapseButton", ArgumentSemantic.Assign)]
		NSButton SectionCollapseButton { get; set; }
	}

	[Mac (10, 10)]
	[BaseType (typeof (NSObject))]
	interface NSAccessibilityElement : NSAccessibility {
		[Export ("accessibilityAddChildElement:")]
		void AccessibilityAddChildElement (NSAccessibilityElement childElement);

		[Static, Export ("accessibilityElementWithRole:frame:label:parent:")]
		NSObject CreateElement (NSString role, CGRect frame, NSString label, NSObject parent);

		[Export ("accessibilityFrameInParentSpace")]
		CGRect AccessibilityFrameInParentSpace { get; set; }
	}

	[Static]
	partial interface NSAccessibilityAttributes {
		[Mac (10, 10)]
		[Field ("NSAccessibilitySharedFocusElementsAttribute")]
		NSString SharedFocusElementsAttribute { get; }

		[Mac (10, 10)]
		[Field ("NSAccessibilityAlternateUIVisibleAttribute")]
		NSString AlternateUIVisibleAttribute { get; }

		[Mac (10, 11)]
		[Field ("NSAccessibilityListItemPrefixTextAttribute")]
		NSString ListItemPrefixTextAttribute { get; }

		[Mac (10, 11)]
		[Field ("NSAccessibilityListItemIndexTextAttribute")]
		NSString ListItemIndexTextAttribute { get; }

		[Mac (10, 11)]
		[Field ("NSAccessibilityListItemLevelTextAttribute")]
		NSString ListItemLevelTextAttribute { get; }

		[Field ("NSAccessibilityRoleAttribute")]
		NSString RoleAttribute { get; }

		[Field ("NSAccessibilityRoleDescriptionAttribute")]
		NSString RoleDescriptionAttribute { get; }

		[Field ("NSAccessibilitySubroleAttribute")]
		NSString SubroleAttribute { get; }

		[Field ("NSAccessibilityHelpAttribute")]
		NSString HelpAttribute { get; }

		[Field ("NSAccessibilityValueAttribute")]
		NSString ValueAttribute { get; }

		[Field ("NSAccessibilityMinValueAttribute")]
		NSString MinValueAttribute { get; }

		[Field ("NSAccessibilityMaxValueAttribute")]
		NSString MaxValueAttribute { get; }

		[Field ("NSAccessibilityEnabledAttribute")]
		NSString EnabledAttribute { get; }

		[Field ("NSAccessibilityFocusedAttribute")]
		NSString FocusedAttribute { get; }

		[Field ("NSAccessibilityParentAttribute")]
		NSString ParentAttribute { get; }

		[Field ("NSAccessibilityChildrenAttribute")]
		NSString ChildrenAttribute { get; }

		[Field ("NSAccessibilityWindowAttribute")]
		NSString WindowAttribute { get; }

#if !XAMCORE_4_0
		[Obsolete ("Use 'TopLevelUIElementAttribute' instead.")]
		[Field ("NSAccessibilityTopLevelUIElementAttribute")]
		NSString ToplevelUIElementAttribute { get; }
#endif

		[Field ("NSAccessibilityTopLevelUIElementAttribute")]
		NSString TopLevelUIElementAttribute { get; }

		[Field ("NSAccessibilitySelectedChildrenAttribute")]
		NSString SelectedChildrenAttribute { get; }

		[Field ("NSAccessibilityVisibleChildrenAttribute")]
		NSString VisibleChildrenAttribute { get; }

		[Field ("NSAccessibilityPositionAttribute")]
		NSString PositionAttribute { get; }

		[Field ("NSAccessibilitySizeAttribute")]
		NSString SizeAttribute { get; }

		[Field ("NSAccessibilityContentsAttribute")]
		NSString ContentsAttribute { get; }

		[Field ("NSAccessibilityTitleAttribute")]
		NSString TitleAttribute { get; }

		[Field ("NSAccessibilityDescriptionAttribute")]
		NSString DescriptionAttribute { get; }

		[Field ("NSAccessibilityShownMenuAttribute")]
		NSString ShownMenuAttribute { get; }

		[Field ("NSAccessibilityValueDescriptionAttribute")]
		NSString ValueDescriptionAttribute { get; }

		[Field ("NSAccessibilityPreviousContentsAttribute")]
		NSString PreviousContentsAttribute { get; }

		[Field ("NSAccessibilityNextContentsAttribute")]
		NSString NextContentsAttribute { get; }

		[Field ("NSAccessibilityHeaderAttribute")]
		NSString HeaderAttribute { get; }

		[Field ("NSAccessibilityEditedAttribute")]
		NSString EditedAttribute { get; }

		[Field ("NSAccessibilityTabsAttribute")]
		NSString TabsAttribute { get; }

		[Field ("NSAccessibilityHorizontalScrollBarAttribute")]
		NSString HorizontalScrollBarAttribute { get; }

		[Field ("NSAccessibilityVerticalScrollBarAttribute")]
		NSString VerticalScrollBarAttribute { get; }

		[Field ("NSAccessibilityOverflowButtonAttribute")]
		NSString OverflowButtonAttribute { get; }

		[Field ("NSAccessibilityIncrementButtonAttribute")]
		NSString IncrementButtonAttribute { get; }

		[Field ("NSAccessibilityDecrementButtonAttribute")]
		NSString DecrementButtonAttribute { get; }

		[Field ("NSAccessibilityFilenameAttribute")]
		NSString FilenameAttribute { get; }

		[Field ("NSAccessibilityExpandedAttribute")]
		NSString ExpandedAttribute { get; }

		[Field ("NSAccessibilitySelectedAttribute")]
		NSString SelectedAttribute { get; }

		[Field ("NSAccessibilitySplittersAttribute")]
		NSString SplittersAttribute { get; }

		[Field ("NSAccessibilityDocumentAttribute")]
		NSString DocumentAttribute { get; }

		[Mac (10, 10)]
		[Field ("NSAccessibilityActivationPointAttribute")]
		NSString ActivationPointAttribute { get; }

		[Field ("NSAccessibilityURLAttribute")]
		NSString URLAttribute { get; }

		[Field ("NSAccessibilityIndexAttribute")]
		NSString IndexAttribute { get; }

		[Field ("NSAccessibilityRowCountAttribute")]
		NSString RowCountAttribute { get; }

		[Field ("NSAccessibilityColumnCountAttribute")]
		NSString ColumnCountAttribute { get; }

		[Field ("NSAccessibilityOrderedByRowAttribute")]
		NSString OrderedByRowAttribute { get; }

		[Field ("NSAccessibilityWarningValueAttribute")]
		NSString WarningValueAttribute { get; }

		[Field ("NSAccessibilityCriticalValueAttribute")]
		NSString CriticalValueAttribute { get; }

		[Field ("NSAccessibilityPlaceholderValueAttribute")]
		NSString PlaceholderValueAttribute { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilityContainsProtectedContentAttribute")]
		NSString ContainsProtectedContentAttribute { get; }

		[Field ("NSAccessibilityTitleUIElementAttribute")]
		NSString TitleUIAttribute { get; }

		[Field ("NSAccessibilityServesAsTitleForUIElementsAttribute")]
		NSString ServesAsTitleForUIElementsAttribute { get; }

		[Field ("NSAccessibilityLinkedUIElementsAttribute")]
		NSString LinkedUIElementsAttribute { get; }

		[Field ("NSAccessibilitySelectedTextAttribute")]
		NSString SelectedTextAttribute { get; }

		[Field ("NSAccessibilitySelectedTextRangeAttribute")]
		NSString SelectedTextRangeAttribute { get; }

		[Field ("NSAccessibilityNumberOfCharactersAttribute")]
		NSString NumberOfCharactersAttribute { get; }

		[Field ("NSAccessibilityVisibleCharacterRangeAttribute")]
		NSString VisibleCharacterRangeAttribute { get; }

		[Field ("NSAccessibilitySharedTextUIElementsAttribute")]
		NSString SharedTextUIElementsAttribute { get; }

		[Field ("NSAccessibilitySharedCharacterRangeAttribute")]
		NSString SharedCharacterRangeAttribute { get; }

		[Field ("NSAccessibilityInsertionPointLineNumberAttribute")]
		NSString InsertionPointLineNumberAttribute { get; }

		[Field ("NSAccessibilitySelectedTextRangesAttribute")]
		NSString SelectedTextRangesAttribute { get; }

		[Field ("NSAccessibilityLineForIndexParameterizedAttribute")]
		NSString LineForIndexParameterizedAttribute { get; }

		[Field ("NSAccessibilityRangeForLineParameterizedAttribute")]
		NSString RangeForLineParameterizedAttribute { get; }

		[Field ("NSAccessibilityStringForRangeParameterizedAttribute")]
		NSString StringForRangeParameterizeAttribute { get; }

		[Field ("NSAccessibilityRangeForPositionParameterizedAttribute")]
		NSString RangeForPositionParameterizedAttribute { get; }

		[Field ("NSAccessibilityRangeForIndexParameterizedAttribute")]
		NSString RangeForIndexParameterizedAttribute { get; }

		[Field ("NSAccessibilityBoundsForRangeParameterizedAttribute")]
		NSString BoundsForRangeParameterizedAttribute { get; }

		[Field ("NSAccessibilityRTFForRangeParameterizedAttribute")]
		NSString RTFForRangeParameterizedAttribute { get; }

		[Field ("NSAccessibilityStyleRangeForIndexParameterizedAttribute")]
		NSString StyleRangeForIndexParameterizedAttribute { get; }

		[Field ("NSAccessibilityAttributedStringForRangeParameterizedAttribute")]
		NSString AttributedStringForRangeParameterizedAttribute { get; }

		[Field ("NSAccessibilityFontTextAttribute")]
		NSString FontTextAttribute { get; }

		[Field ("NSAccessibilityForegroundColorTextAttribute")]
		NSString ForegroundColorTextAttribute { get; }

		[Field ("NSAccessibilityBackgroundColorTextAttribute")]
		NSString BackgroundColorTextAttribute { get; }

		[Field ("NSAccessibilityUnderlineColorTextAttribute")]
		NSString UnderlineColorTextAttribute { get; }

		[Field ("NSAccessibilityStrikethroughColorTextAttribute")]
		NSString StrikethroughColorTextAttribute { get; }

		[Field ("NSAccessibilityUnderlineTextAttribute")]
		NSString UnderlineTextAttribute { get; }

		[Field ("NSAccessibilitySuperscriptTextAttribute")]
		NSString SuperscriptTextAttribute { get; }

		[Field ("NSAccessibilityStrikethroughTextAttribute")]
		NSString StrikethroughTextAttribute { get; }

		[Field ("NSAccessibilityShadowTextAttribute")]
		NSString ShadowTextAttribute { get; }

		[Field ("NSAccessibilityAttachmentTextAttribute")]
		NSString AttachmentTextAttribute { get; }

		[Field ("NSAccessibilityLinkTextAttribute")]
		NSString LinkTextAttribute { get; }

		[Mac (10, 7)]
		[Field ("NSAccessibilityAutocorrectedTextAttribute")]
		NSString AutocorrectedAttribute { get; }

		[Field ("NSAccessibilityMisspelledTextAttribute")]
		NSString MisspelledTextAttribute { get; }

		[Field ("NSAccessibilityMarkedMisspelledTextAttribute")]
		NSString MarkedMisspelledTextAttribute { get; }	

		[Field ("NSAccessibilityMainAttribute")]
		NSString MainAttribute { get; }

		[Field ("NSAccessibilityMinimizedAttribute")]
		NSString MinimizedAttribute { get; }

		[Field ("NSAccessibilityCloseButtonAttribute")]
		NSString CloseButtonAttribute { get; }

		[Field ("NSAccessibilityZoomButtonAttribute")]
		NSString ZoomButtonAttribute { get; }

		[Field ("NSAccessibilityMinimizeButtonAttribute")]
		NSString MinimizeButtonAttribute { get; }

		[Field ("NSAccessibilityToolbarButtonAttribute")]
		NSString ToolbarButtonAttribute { get; }

		[Field ("NSAccessibilityProxyAttribute")]
		NSString ProxyAttribute { get; }

		[Field ("NSAccessibilityGrowAreaAttribute")]
		NSString GrowAreaAttribute { get; }

		[Field ("NSAccessibilityModalAttribute")]
		NSString ModalAttribute { get; }

		[Field ("NSAccessibilityDefaultButtonAttribute")]
		NSString DefaultButtonAttribute { get; }

		[Field ("NSAccessibilityCancelButtonAttribute")]
		NSString CancelButtonAttribute { get; }

		[Mac (10,7)]
		[Field ("NSAccessibilityFullScreenButtonAttribute")]
		NSString FullScreenButtonAttribute { get; }

		[Field ("NSAccessibilityMenuBarAttribute")]
		NSString MenuBarAttribute { get; }

		[Field ("NSAccessibilityWindowsAttribute")]
		NSString WindowsAttribute { get; }

		[Field ("NSAccessibilityFrontmostAttribute")]
		NSString FrontmostAttribute { get; }

		[Field ("NSAccessibilityHiddenAttribute")]
		NSString HiddenAttribute { get; }

		[Field ("NSAccessibilityMainWindowAttribute")]
		NSString MainWindowAttribute { get; }

		[Field ("NSAccessibilityFocusedWindowAttribute")]
		NSString FocusedWindowAttribute { get; }

		[Field ("NSAccessibilityFocusedUIElementAttribute")]
		NSString FocusedUIElementAttribute { get; }

		[Mac (10, 8)]
		[Field ("NSAccessibilityExtrasMenuBarAttribute")]
		NSString ExtrasMenuBarAttribute { get; }

		[Field ("NSAccessibilityColumnTitlesAttribute")]
		NSString ColumnTitlesAttribute { get; }

		[Field ("NSAccessibilitySearchButtonAttribute")]
		NSString SearchButtonAttribute { get; }

		[Field ("NSAccessibilitySearchMenuAttribute")]
		NSString SearchMenuAttribute { get; }

		[Field ("NSAccessibilityClearButtonAttribute")]
		NSString ClearButtonAttribute { get; }

		[Field ("NSAccessibilityRowsAttribute")]
		NSString RowsAttribute { get; }

		[Field ("NSAccessibilityVisibleRowsAttribute")]
		NSString VisibleRowsAttribute { get; }

		[Field ("NSAccessibilitySelectedRowsAttribute")]
		NSString SelectedRowsAttribute { get; }

		[Field ("NSAccessibilityColumnsAttribute")]
		NSString ColumnsAttribute { get; }

		[Field ("NSAccessibilityVisibleColumnsAttribute")]
		NSString VisibleColumnsAttribute { get; }

		[Field ("NSAccessibilitySelectedColumnsAttribute")]
		NSString SelectedColumnsAttribute { get; }

		[Field ("NSAccessibilitySortDirectionAttribute")]
		NSString SortDirectionAttribute { get; }

		[Field ("NSAccessibilitySelectedCellsAttribute")]
		NSString SelectedCellsAttribute { get; }

		[Field ("NSAccessibilityVisibleCellsAttribute")]
		NSString VisibleCellsAttribute { get; }

		[Field ("NSAccessibilityRowHeaderUIElementsAttribute")]
		NSString RowHeaderUIElementsAttribute { get; }

		[Field ("NSAccessibilityColumnHeaderUIElementsAttribute")]
		NSString ColumnHeaderUIElementsAttribute { get; }

		[Field ("NSAccessibilityCellForColumnAndRowParameterizedAttribute")]
		NSString CellForColumnAndRowParameterizedAttribute { get; }

		[Field ("NSAccessibilityRowIndexRangeAttribute")]
		NSString RowIndexRangeAttribute { get; }

		[Field ("NSAccessibilityColumnIndexRangeAttribute")]
		NSString ColumnIndexRangeAttribute { get; }

		[Field ("NSAccessibilityHorizontalUnitsAttribute")]
		NSString HorizontalUnitsAttribute { get; }

		[Field ("NSAccessibilityVerticalUnitsAttribute")]
		NSString VerticalUnitsAttribute { get; }

		[Field ("NSAccessibilityHorizontalUnitDescriptionAttribute")]
		NSString HorizontalUnitDescriptionAttribute { get; }

		[Field ("NSAccessibilityVerticalUnitDescriptionAttribute")]
		NSString VerticalUnitDescriptionAttribute { get; }

		[Field ("NSAccessibilityLayoutPointForScreenPointParameterizedAttribute")]
		NSString LayoutPointForScreenPointParameterizedAttribute { get; }

		[Field ("NSAccessibilityLayoutSizeForScreenSizeParameterizedAttribute")]
		NSString LayoutSizeForScreenSizeParameterizedAttribute { get; }

		[Field ("NSAccessibilityScreenPointForLayoutPointParameterizedAttribute")]
		NSString ScreenPointForLayoutPointParameterizedAttribute { get; }

		[Field ("NSAccessibilityScreenSizeForLayoutSizeParameterizedAttribute")]
		NSString ScreenSizeForLayoutSizeParameterizedAttribute { get; }

		[Field ("NSAccessibilityHandlesAttribute")]
		NSString HandlesAttribute { get; }

		[Field ("NSAccessibilityDisclosingAttribute")]
		NSString DisclosingAttribute { get; }

		[Field ("NSAccessibilityDisclosedRowsAttribute")]
		NSString DisclosedRowsAttribute { get; }

		[Field ("NSAccessibilityDisclosedByRowAttribute")]
		NSString DisclosedByRowAttribute { get; }

		[Field ("NSAccessibilityDisclosureLevelAttribute")]
		NSString DisclosureLevelAttribute { get; }

		[Field ("NSAccessibilityAllowedValuesAttribute")]
		NSString AllowedValuesAttribute { get; }

		[Field ("NSAccessibilityLabelUIElementsAttribute")]
		NSString LabelUIElementsAttribute { get; }

		[Field ("NSAccessibilityLabelValueAttribute")]
		NSString LabelValueAttribute { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'NSAccessibility' methods instead.")]
		[Field ("NSAccessibilityMatteHoleAttribute")]
		NSString MatteHoleAttribute { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'NSAccessibility' methods instead.")]
		[Field ("NSAccessibilityMatteContentUIElementAttribute")]
		NSString MatteContentUIElementAttribute { get; }

		[Field ("NSAccessibilityMarkerUIElementsAttribute")]
		NSString MarkerUIElementsAttribute { get; }

		[Field ("NSAccessibilityMarkerValuesAttribute")]
		NSString MarkerValuesAttribute { get; }

		[Field ("NSAccessibilityMarkerGroupUIElementAttribute")]
		NSString MarkerGroupUIElementAttribute { get; }

		[Field ("NSAccessibilityUnitsAttribute")]
		NSString UnitsAttribute { get; }

		[Field ("NSAccessibilityUnitDescriptionAttribute")]
		NSString UnitDescriptionAttribute { get; }

		[Field ("NSAccessibilityMarkerTypeAttribute")]
		NSString MarkerTypeAttribute { get; }

		[Field ("NSAccessibilityMarkerTypeDescriptionAttribute")]
		NSString MarkerTypeDescriptionAttribute { get; }

		[Mac (10, 7)]
		[Field ("NSAccessibilityIdentifierAttribute")]
		NSString IdentifierAttribute { get; }

		[Mac (10, 12)]
		[Field ("NSAccessibilityRequiredAttribute")]
		NSString RequiredAttribute { get; }

		[Mac (10, 12)]
		[Field ("NSAccessibilityTextAlignmentAttribute")]
		NSString TextAlignmentAttribute { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityLanguageTextAttribute")]
		NSString LanguageTextAttribute { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityCustomTextAttribute")]
		NSString CustomTextAttribute { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityAnnotationTextAttribute")]
		NSString AnnotationTextAttribute { get; }
	}

	[Static]
	[Mac (10, 13)]
	partial interface NSAccessibilityAnnotationAttributeKey {
		[Field ("NSAccessibilityAnnotationLabel")]
		NSString AnnotationLabel { get; }

		[Field ("NSAccessibilityAnnotationElement")]
		NSString AnnotationElement { get; }

		[Field ("NSAccessibilityAnnotationLocation")]
		NSString AnnotationLocation { get; }
	}

	[Static]
	interface NSAccessibilityFontKeys {
		[Field ("NSAccessibilityFontNameKey")]
		NSString FontNameKey { get; }

		[Field ("NSAccessibilityFontFamilyKey")]
		NSString FontFamilyKey { get; }

		[Field ("NSAccessibilityVisibleNameKey")]
		NSString VisibleNameKey { get; }

		[Field ("NSAccessibilityFontSizeKey")]
		NSString FontSizeKey { get; }
	}

	[Static]
	interface NSAccessibilityRoles {
		[Field ("NSAccessibilityUnknownRole")]
		NSString UnknownRole { get; }

		[Field ("NSAccessibilityButtonRole")]
		NSString ButtonRole { get; }

		[Field ("NSAccessibilityRadioButtonRole")]
		NSString RadioButtonRole { get; }

		[Field ("NSAccessibilityCheckBoxRole")]
		NSString CheckBoxRole { get; }

		[Field ("NSAccessibilitySliderRole")]
		NSString SliderRole { get; }

		[Field ("NSAccessibilityTabGroupRole")]
		NSString TabGroupRole { get; }

		[Field ("NSAccessibilityTextFieldRole")]
		NSString TextFieldRole { get; }

		[Field ("NSAccessibilityStaticTextRole")]
		NSString StaticTextRole { get; }

		[Field ("NSAccessibilityTextAreaRole")]
		NSString TextAreaRole { get; }

		[Field ("NSAccessibilityScrollAreaRole")]
		NSString ScrollAreaRole { get; }

		[Field ("NSAccessibilityPopUpButtonRole")]
		NSString PopUpButtonRole { get; }

		[Field ("NSAccessibilityMenuButtonRole")]
		NSString MenuButtonRole { get; }

		[Field ("NSAccessibilityTableRole")]
		NSString TableRole { get; }

		[Field ("NSAccessibilityApplicationRole")]
		NSString ApplicationRole { get; }

		[Field ("NSAccessibilityGroupRole")]
		NSString GroupRole { get; }

		[Field ("NSAccessibilityRadioGroupRole")]
		NSString RadioGroupRole { get; }

		[Field ("NSAccessibilityListRole")]
		NSString ListRole { get; }

		[Field ("NSAccessibilityScrollBarRole")]
		NSString ScrollBarRole { get; }

		[Field ("NSAccessibilityValueIndicatorRole")]
		NSString ValueIndicatorRole { get; }

		[Field ("NSAccessibilityImageRole")]
		NSString ImageRole { get; }

		[Field ("NSAccessibilityMenuBarRole")]
		NSString MenuRole { get; }

		[Field ("NSAccessibilityMenuItemRole")]
		NSString MenuItemRole { get; }

		[Field ("NSAccessibilityColumnRole")]
		NSString ColumnRole { get; }

		[Field ("NSAccessibilityRowRole")]
		NSString RowRole { get; }

		[Field ("NSAccessibilityToolbarRole")]
		NSString ToolbarRole { get; }

		[Field ("NSAccessibilityBusyIndicatorRole")]
		NSString BusyIndicatorRole { get; }

		[Field ("NSAccessibilityProgressIndicatorRole")]
		NSString ProgressIndicatorRole { get; }

		[Field ("NSAccessibilityWindowRole")]
		NSString WindowRole { get; }

		[Field ("NSAccessibilityDrawerRole")]
		NSString DrawerRole { get; }

		[Field ("NSAccessibilitySystemWideRole")]
		NSString SystemWideRole { get; }

		[Field ("NSAccessibilityOutlineRole")]
		NSString OutlineRole { get; }

		[Field ("NSAccessibilityIncrementorRole")]
		NSString IncrementorRole { get; }

		[Field ("NSAccessibilityBrowserRole")]
		NSString BrowserRole { get; }

		[Field ("NSAccessibilityComboBoxRole")]
		NSString ComboBoxRole { get; }

		[Field ("NSAccessibilitySplitGroupRole")]
		NSString SplitGroupRole { get; }

		[Field ("NSAccessibilitySplitterRole")]
		NSString SplitterRole { get; }

		[Field ("NSAccessibilityColorWellRole")]
		NSString ColorWellRole { get; }

		[Field ("NSAccessibilityGrowAreaRole")]
		NSString GrowAreaRole { get; }

		[Field ("NSAccessibilitySheetRole")]
		NSString SheetRole { get; }

		[Field ("NSAccessibilityHelpTagRole")]
		NSString HelpTagRole { get; }

		[Field ("NSAccessibilityMatteRole")]
		NSString MatteRole { get; }

		[Field ("NSAccessibilityRulerRole")]
		NSString RulerRole { get; }

		[Field ("NSAccessibilityRulerMarkerRole")]
		NSString RulerMarkerRole { get; }

		[Field ("NSAccessibilityLinkRole")]
		NSString LinkRole { get; }

		[Field ("NSAccessibilityDisclosureTriangleRole")]
		NSString DisclosureTriangleRole { get; }

		[Field ("NSAccessibilityGridRole")]
		NSString GridRole { get; }

		[Field ("NSAccessibilityRelevanceIndicatorRole")]
		NSString RelevanceIndicatorRole { get; }

		[Field ("NSAccessibilityLevelIndicatorRole")]
		NSString LevelIndicatorRole { get; }

		[Field ("NSAccessibilityCellRole")]
		NSString CellRole { get; }

		[Mac (10, 7)]
		[Field ("NSAccessibilityPopoverRole")]
		NSString PopoverRole { get; }

		[Field ("NSAccessibilityLayoutAreaRole")]
		NSString LayoutAreaRole { get; }

		[Field ("NSAccessibilityLayoutItemRole")]
		NSString LayoutItemRole { get; }

		[Field ("NSAccessibilityHandleRole")]
		NSString HandleRole { get; }

		[Mac (10, 12)]
		[Field ("NSAccessibilityMenuBarItemRole")]
		NSString MenuBarItemRole { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityPageRole")]
		NSString PageRole { get; }
	}

	[Static]
	interface NSAccessibilitySubroles {
		[Field ("NSAccessibilityUnknownSubrole")]
		NSString UnknownSubrole { get; }

		[Field ("NSAccessibilityCloseButtonSubrole")]
		NSString CloseButtonSubrole { get; }

		[Field ("NSAccessibilityZoomButtonSubrole")]
		NSString ZoomButtonSubrole { get; }

		[Field ("NSAccessibilityMinimizeButtonSubrole")]
		NSString MinimizeButtonSubrole { get; }

		[Field ("NSAccessibilityToolbarButtonSubrole")]
		NSString ToolbarButtonSubrole { get; }

		[Field ("NSAccessibilityTableRowSubrole")]
		NSString TableRowSubrole { get; }

		[Field ("NSAccessibilityOutlineRowSubrole")]
		NSString OutlineRowSubrole { get; }

		[Field ("NSAccessibilitySecureTextFieldSubrole")]
		NSString SecureTextFieldSubrole { get; }

		[Field ("NSAccessibilityStandardWindowSubrole")]
		NSString StandardWindowSubrole { get; }

		[Field ("NSAccessibilityDialogSubrole")]
		NSString DialogSubrole { get; }

		[Field ("NSAccessibilitySystemDialogSubrole")]
		NSString SystemDialogSubrole { get; }

		[Field ("NSAccessibilityFloatingWindowSubrole")]
		NSString FloatingWindowSubrole { get; }

		[Field ("NSAccessibilitySystemFloatingWindowSubrole")]
		NSString SystemFloatingWindowSubrole { get; }

		[Field ("NSAccessibilityIncrementArrowSubrole")]
		NSString IncrementArrowSubrole { get; }

		[Field ("NSAccessibilityDecrementArrowSubrole")]
		NSString DecrementArrowSubrole { get; }

		[Field ("NSAccessibilityIncrementPageSubrole")]
		NSString IncrementPageSubrole { get; }

		[Field ("NSAccessibilityDecrementPageSubrole")]
		NSString DecrementPageSubrole { get; }

		[Field ("NSAccessibilitySearchFieldSubrole")]
		NSString SearchFieldSubrole { get; }

		[Field ("NSAccessibilityTextAttachmentSubrole")]
		NSString TextAttachmentSubrole { get; }

		[Field ("NSAccessibilityTextLinkSubrole")]
		NSString TextLinkSubrole { get; }

		[Field ("NSAccessibilityTimelineSubrole")]
		NSString TimelineSubrole { get; }

		[Field ("NSAccessibilitySortButtonSubrole")]
		NSString SortButtonSubrole { get; }

		[Field ("NSAccessibilityRatingIndicatorSubrole")]
		NSString RatingIndicatorSubrole { get; }

		[Field ("NSAccessibilityContentListSubrole")]
		NSString ContentListSubrole { get; }

		[Field ("NSAccessibilityDefinitionListSubrole")]
		NSString DefinitionListSubrole { get; }

		[Mac (10, 7)]
		[Field ("NSAccessibilityFullScreenButtonSubrole")]
		NSString FullScreenButtonSubrole { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilityToggleSubrole")]
		NSString ToggleSubrole { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilitySwitchSubrole")]
		NSString SwitchSubrole { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilityDescriptionListSubrole")]
		NSString DescriptionListSubrole { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityTabButtonSubrole")]
		NSString TabButtonSubrole { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilityCollectionListSubrole")]
		NSString CollectionListSubrole { get; }

		[Mac (10, 13)]
		[Field ("NSAccessibilitySectionListSubrole")]
		NSString SectionListSubrole { get; }
	}

#if !XAMCORE_4_0
	[Static]
	interface NSAccessibilityNotifications {
		[Field ("NSAccessibilityMainWindowChangedNotification")]
		NSString MainWindowChangedNotification { get; }

		[Field ("NSAccessibilityFocusedWindowChangedNotification")]
		NSString FocusedWindowChangedNotification { get; }

		[Field ("NSAccessibilityFocusedUIElementChangedNotification")]
		NSString UIElementFocusedChangedNotification { get; }

		[Field ("NSAccessibilityApplicationActivatedNotification")]
		NSString ApplicationActivatedNotification { get; }

		[Field ("NSAccessibilityApplicationDeactivatedNotification")]
		NSString ApplicationDeactivatedNotification { get; }

		[Field ("NSAccessibilityApplicationHiddenNotification")]
		NSString ApplicationHiddenNotification { get; }

		[Field ("NSAccessibilityApplicationShownNotification")]
		NSString ApplicationShownNotification { get; }

		[Field ("NSAccessibilityWindowCreatedNotification")]
		NSString WindowCreatedNotification { get; }

		[Field ("NSAccessibilityWindowMovedNotification")]
		NSString WindowMovedNotification { get; }

		[Field ("NSAccessibilityWindowResizedNotification")]
		NSString WindowResizedNotification { get; }

		[Field ("NSAccessibilityWindowMiniaturizedNotification")]
		NSString WindowMiniaturizedNotification { get; }

		[Field ("NSAccessibilityWindowDeminiaturizedNotification")]
		NSString WindowDeminiaturizedNotification { get; }

		[Field ("NSAccessibilityDrawerCreatedNotification")]
		NSString DrawerCreatedNotification { get; }

		[Field ("NSAccessibilitySheetCreatedNotification")]
		NSString SheetCreatedNotification { get; }

		[Field ("NSAccessibilityUIElementDestroyedNotification")]
		NSString UIElementDestroyedNotification { get; }

		[Field ("NSAccessibilityValueChangedNotification")]
		NSString ValueChangedNotification { get; }

		[Field ("NSAccessibilityTitleChangedNotification")]
		NSString TitleChangedNotification { get; }

		[Field ("NSAccessibilityResizedNotification")]
		NSString ResizedNotification { get; }

		[Field ("NSAccessibilityMovedNotification")]
		NSString MovedNotification { get; }

		[Field ("NSAccessibilityCreatedNotification")]
		NSString CreatedNotification { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilityLayoutChangedNotification")]
		NSString LayoutChangedNotification { get; }

		[Field ("NSAccessibilityHelpTagCreatedNotification")]
		NSString HelpTagCreatedNotification { get; }

		[Field ("NSAccessibilitySelectedTextChangedNotification")]
		NSString SelectedTextChangedNotification { get; }

		[Field ("NSAccessibilityRowCountChangedNotification")]
		NSString RowCountChangedNotification { get; }

		[Field ("NSAccessibilitySelectedChildrenChangedNotification")]
		NSString SelectedChildrenChangedNotification { get; }

		[Field ("NSAccessibilitySelectedRowsChangedNotification")]
		NSString SelectedRowsChangedNotification { get; }

		[Field ("NSAccessibilitySelectedColumnsChangedNotification")]
		NSString SelectedColumnsChangedNotification { get; }

		[Field ("NSAccessibilityRowExpandedNotification")]
		NSString RowExpandedNotification { get; }

		[Field ("NSAccessibilityRowCollapsedNotification")]
		NSString RowCollapsedNotification { get; }

		[Field ("NSAccessibilitySelectedCellsChangedNotification")]
		NSString SelectedCellsChangedNotification { get; }

		[Field ("NSAccessibilityUnitsChangedNotification")]
		NSString UnitsChangedNotification { get; }

		[Field ("NSAccessibilitySelectedChildrenMovedNotification")]
		NSString SelectedChildrenMovedNotification { get; }

		[Mac (10, 7)]
		[Field ("NSAccessibilityAnnouncementRequestedNotification")]
		NSString AnnouncementRequestedNotification { get; }
	}

	[Static]
	interface NSWorkspaceAccessibilityNotifications {
		[Mac (10, 10)]
		[Field ("NSWorkspaceAccessibilityDisplayOptionsDidChangeNotification")]
		NSString DisplayOptionsDidChangeNotification { get; }
	}
#endif

	[Static]
	interface NSAccessibilityNotificationUserInfoKeys {
		[Mac (10, 9)]
		[Field ("NSAccessibilityUIElementsKey")]
		NSString UIElementsKey { get; }

		[Mac (10, 9)]
		[Field ("NSAccessibilityPriorityKey")]
		NSString PriorityKey { get; }

		[Field ("NSAccessibilityAnnouncementKey")]
		NSString AnnouncementKey { get; }
	}

	[Static]
	interface NSAccessibilityActions {
		[Field ("NSAccessibilityPressAction")]
		NSString PressAction { get; }

		[Field ("NSAccessibilityIncrementAction")]
		NSString IncrementAction { get; }

		[Field ("NSAccessibilityDecrementAction")]
		NSString DecrementAction { get; }

		[Field ("NSAccessibilityConfirmAction")]
		NSString ConfirmAction { get; }

		[Field ("NSAccessibilityPickAction")]
		NSString PickAction { get; }

		[Field ("NSAccessibilityCancelAction")]
		NSString CancelAction { get; }

		[Field ("NSAccessibilityRaiseAction")]
		NSString RaiseAction { get; }

		[Field ("NSAccessibilityShowMenuAction")]
		NSString ShowMenu { get; }

		[Field ("NSAccessibilityDeleteAction")]
		NSString DeleteAction { get; }

		[Mac (10,9)]
		[Field ("NSAccessibilityShowAlternateUIAction")]
		NSString ShowAlternateUIAction { get; }

		[Mac (10,9)]
		[Field ("NSAccessibilityShowDefaultUIAction")]
		NSString ShowDefaultUIAction { get; }
	}

	[Mac (10,10)]
	[Protocol (Name = "NSAccessibilityElement")] // exists both as a type and a protocol in ObjC, Swift uses NSAccessibilityElementProtocol
	interface NSAccessibilityElementProtocol {
		[Abstract]
		[Export ("accessibilityFrame")]
		CGRect AccessibilityFrame { get; }

		[Abstract]
		[NullAllowed, Export ("accessibilityParent")]
		NSObject AccessibilityParent { get; }

		[Export ("isAccessibilityFocused")]
		bool AccessibilityFocused { get; }

		[Export ("accessibilityIdentifier")]
		string AccessibilityIdentifier { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityGroup : NSAccessibilityElementProtocol {
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityButton : NSAccessibilityElementProtocol {
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }

		[Abstract]
		[Export ("accessibilityPerformPress")]
		bool AccessibilityPerformPress ();
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilitySwitch : NSAccessibilityButton {
		[Abstract]
		[NullAllowed, Export ("accessibilityValue")]
		string AccessibilityValue { get; }

		[Export ("accessibilityPerformIncrement")]
		bool AccessibilityPerformIncrement ();

		[Export ("accessibilityPerformDecrement")]
		bool AccessibilityPerformDecrement ();
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityRadioButton : NSAccessibilityButton {
		[Abstract]
		[NullAllowed, Export ("accessibilityValue")]
		NSNumber AccessibilityValue { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityStaticText : NSAccessibilityElementProtocol {
		[Abstract]
		[NullAllowed, Export ("accessibilityValue")]
		string AccessibilityValue { get; }

		[Export ("accessibilityAttributedStringForRange:")]
		[return: NullAllowed]
		NSAttributedString GetAccessibilityAttributedString (NSRange range);

		[Export ("accessibilityVisibleCharacterRange")]
		NSRange AccessibilityVisibleCharacterRange { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityNavigableStaticText : NSAccessibilityStaticText {
		[Abstract]
		[Export ("accessibilityStringForRange:")]
		[return: NullAllowed]
		string GetAccessibilityString (NSRange range);

		[Abstract]
		[Export ("accessibilityLineForIndex:")]
		nint GetAccessibilityLine (nint index);

		[Abstract]
		[Export ("accessibilityRangeForLine:")]
		NSRange GetAccessibilityRangeForLine (nint lineNumber);

		[Abstract]
		[Export ("accessibilityFrameForRange:")]
		CGRect GetAccessibilityFrame (NSRange range);
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityProgressIndicator : NSAccessibilityGroup {
		[Abstract]
		[NullAllowed, Export ("accessibilityValue")]
		NSNumber AccessibilityValue { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityStepper : NSAccessibilityElementProtocol {
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }

		[Abstract]
		[Export ("accessibilityPerformIncrement")]
		bool AccessibilityPerformIncrement ();

		[Abstract]
		[Export ("accessibilityPerformDecrement")]
		bool AccessibilityPerformDecrement ();

		[NullAllowed, Export ("accessibilityValue")]
		NSObject AccessibilityValue { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilitySlider : NSAccessibilityElementProtocol {
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }

		[Abstract]
		[NullAllowed, Export ("accessibilityValue")]
		NSObject AccessibilityValue { get; }

		[Abstract]
		[Export ("accessibilityPerformIncrement")]
		bool AccessibilityPerformIncrement ();

		[Abstract]
		[Export ("accessibilityPerformDecrement")]
		bool AccessibilityPerformDecrement ();
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityImage : NSAccessibilityElementProtocol {
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityContainsTransientUI : NSAccessibilityElementProtocol {
		[Abstract]
		[Export ("accessibilityPerformShowAlternateUI")]
		bool AccessibilityPerformShowAlternateUI ();

		[Abstract]
		[Export ("accessibilityPerformShowDefaultUI")]
		bool AccessibilityPerformShowDefaultUI ();

		[Abstract]
		[Export ("isAccessibilityAlternateUIVisible")]
		bool IsAccessibilityAlternateUIVisible { get; }
	}

	interface INSAccessibilityRow {}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityTable : NSAccessibilityGroup {
		[Abstract]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }

		[Abstract]
		[NullAllowed, Export ("accessibilityRows")]
		INSAccessibilityRow[] AccessibilityRows { get; }

		[NullAllowed, Export ("accessibilitySelectedRows")]
		INSAccessibilityRow[] AccessibilitySelectedRows { get; set; }

		[Export ("accessibilityVisibleRows")]
		INSAccessibilityRow[] AccessibilityVisibleRows { get; }

		[Export ("accessibilityColumns")]
		NSObject[] AccessibilityColumns { get; } 

		[Export ("accessibilityVisibleColumns")]
		NSObject[] AccessibilityVisibleColumns { get; } 

		[Export ("accessibilitySelectedColumns")]
		NSObject[] AccessibilitySelectedColumns { get; } 

		[Export ("accessibilityHeaderGroup")]
		string AccessibilityHeaderGroup { get; } 

		[Export ("accessibilitySelectedCells")]
		NSObject[] AccessibilitySelectedCells { get; } 

		[Export ("accessibilityVisibleCells")]
		NSObject[] AccessibilityVisibleCells { get; } 

		[Export ("accessibilityRowHeaderUIElements")]
		NSObject[] AccessibilityRowHeaderUIElements { get; } 

		[Export ("accessibilityColumnHeaderUIElements")]
		NSObject[] AccessibilityColumnHeaderUIElements { get; } 
	}

	[Mac (10,10)]
	interface NSAccessibilityOutline : NSAccessibilityTable {
	}

	[Mac (10,10)]
	interface NSAccessibilityList : NSAccessibilityTable {
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityRow : NSAccessibilityGroup {
		[Abstract]
		[Export ("accessibilityIndex")]
		nint AccessibilityIndex { get; }

		[Export ("accessibilityDisclosureLevel")]
		nint AccessibilityDisclosureLevel { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityLayoutArea : NSAccessibilityGroup {
		[Abstract]
		[Export ("accessibilityLabel")]
		string AccessibilityLabel { get; }

		[Abstract]
		[NullAllowed, Export ("accessibilityChildren")]
		NSObject[] AccessibilityChildren { get; }

		[Abstract]
		[NullAllowed, Export ("accessibilitySelectedChildren")]
		NSObject[] AccessibilitySelectedChildren { get; }

		[Abstract]
		[Export ("accessibilityFocusedUIElement")]
		NSObject AccessibilityFocusedUIElement { get; }
	}

	[Mac (10,10)]
	[Protocol]
	interface NSAccessibilityLayoutItem : NSAccessibilityGroup {
		[Export ("setAccessibilityFrame:")]
		void SetAccessibilityFrame (CGRect frame);
	}

	interface NSObjectAccessibilityExtensions {
		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityAttributeNames")]
		NSArray AccessibilityAttributeNames { get; }

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityAttributeValue:")]
		NSObject GetAccessibilityValue (NSString attribute);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityIsAttributeSettable:")]
		bool IsAccessibilityAttributeSettable (NSString attribute);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilitySetValue:forAttribute:")]
		void SetAccessibilityValue (NSString attribute, NSObject value);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityParameterizedAttributeNames")]
		NSArray AccessibilityParameterizedAttributeNames { get; }

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityAttributeValue:forParameter:")]
		NSObject GetAccessibilityValue (NSString attribute, NSObject parameter);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityActionNames")]
		NSArray AccessibilityActionNames { get; }

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityActionDescription:")]
		NSString GetAccessibilityActionDescription (NSString action);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityPerformAction:")]
		void AccessibilityPerformAction (NSString action);

		[Availability (Obsoleted = Platform.Mac_10_10, Message = "Use the NSAccessibility protocol methods instead.")]
		[Export ("accessibilityIsIgnored")]
		bool AccessibilityIsIgnored { get; }

		[Export ("accessibilityHitTest:")]
		NSObject GetAccessibilityHitTest (CGPoint point);

		[Export ("accessibilityFocusedUIElement")]
		NSObject GetAccessibilityFocusedUIElement ();

		[Export ("accessibilityIndexOfChild:")]
		nuint GetAccessibilityIndexOfChild (NSObject child);

		[Export ("accessibilityArrayAttributeCount:")]
		nuint GetAccessibilityArrayAttributeCount (NSString attribute);

		[Export ("accessibilityArrayAttributeValues:index:maxCount:")]
		NSObject[] GetAccessibilityArrayAttributeValues (NSString attribute, nuint index, nuint maxCount);

		[Mac (10,9)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Export ("accessibilityNotifiesWhenDestroyed")]
		bool AccessibilityNotifiesWhenDestroyed { get; }
	}

	[Mac (10, 10)]
	interface NSWorkspaceAccessibilityExtensions {
		[Export ("accessibilityDisplayShouldIncreaseContrast")]
		bool AccessibilityDisplayShouldIncreaseContrast { get; }

		[Export ("accessibilityDisplayShouldDifferentiateWithoutColor")]
		bool AccessibilityDisplayShouldDifferentiateWithoutColor { get; }

		[Export ("accessibilityDisplayShouldReduceTransparency")]
		bool AccessibilityDisplayShouldReduceTransparency { get; }

		[Mac (10, 12)]
		[Export ("accessibilityDisplayShouldInvertColors")]
		bool AccessibilityDisplayShouldInvertColors { get; }

		[Mac (10, 12)]
		[Export ("accessibilityDisplayShouldReduceMotion")]
		bool AccessibilityDisplayShouldReduceMotion { get; }

		[Mac (10, 13)]
		[Export ("voiceOverEnabled")]
		bool VoiceOverEnabled { [Bind ("isVoiceOverEnabled")] get; }

		[Mac (10, 13)]
		[Export ("switchControlEnabled")]
		bool SwitchControlEnabled { [Bind ("isSwitchControlEnabled")] get; }
	}
	
	interface INSFilePromiseProviderDelegate {}

	[DesignatedDefaultCtor]
	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface NSFilePromiseProvider : NSPasteboardWriting
	{
		[Export ("fileType")]
		string FileType { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INSFilePromiseProviderDelegate Delegate { get; set; }

		[NullAllowed, Export ("userInfo", ArgumentSemantic.Strong)]
		NSObject UserInfo { get; set; }

		[Export ("initWithFileType:delegate:")]
		IntPtr Constructor (string fileType, INSFilePromiseProviderDelegate @delegate);
	}

	[Mac (10,12)]
	[Protocol]
	interface NSFilePromiseProviderDelegate
	{
		[Abstract]
		[Export ("filePromiseProvider:fileNameForType:")]
		string GetFileNameForDestination (NSFilePromiseProvider filePromiseProvider, string fileType);

		[Export ("filePromiseProvider:writePromiseToURL:completionHandler:")]
		void WritePromiseToUrl (NSFilePromiseProvider filePromiseProvider, NSUrl url, [NullAllowed] Action<NSError> completionHandler);

		[Export ("operationQueueForFilePromiseProvider:")]
		NSOperationQueue GetOperationQueue (NSFilePromiseProvider filePromiseProvider);
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface NSFilePromiseReceiver : NSPasteboardReading
	{
		[Static]
		[Export ("readableDraggedTypes", ArgumentSemantic.Copy)]
		string[] ReadableDraggedTypes { get; }

		[Export ("fileTypes", ArgumentSemantic.Copy)]
		string[] FileTypes { get; }

		[Export ("fileNames", ArgumentSemantic.Copy)]
		string[] FileNames { get; }

		[Export ("receivePromisedFilesAtDestination:options:operationQueue:reader:")]
		void ReceivePromisedFiles (NSUrl destinationDir, NSDictionary options, NSOperationQueue operationQueue, Action<NSUrl, NSError> reader);
	}

	interface INSValidatedUserInterfaceItem { }

	[Protocol]
	interface NSValidatedUserInterfaceItem
	{
		[Abstract]
		[NullAllowed, Export ("action")]
		Selector Action { get; }

		[Abstract]
		[Export ("tag")]
		nint Tag { get; }
	}

#if XAMCORE_2_0
	[Protocol]
	[Mac (10,12, onlyOn64 : true)]
	interface NSCloudSharingValidation
	{
		[Abstract]
		[Export ("cloudShareForUserInterfaceItem:")]
		[return: NullAllowed]
		CKShare GetCloudShare (INSValidatedUserInterfaceItem item);
	}
#endif

	[BaseType (typeof(CAOpenGLLayer))]
	interface NSOpenGLLayer
	{
		[NullAllowed, Export ("view", ArgumentSemantic.Assign)]
		NSView View { get; set; }

		[NullAllowed, Export ("openGLPixelFormat", ArgumentSemantic.Strong)]
		NSOpenGLPixelFormat OpenGLPixelFormat { get; set; }

		[NullAllowed, Export ("openGLContext", ArgumentSemantic.Strong)]
		NSOpenGLContext OpenGLContext { get; set; }

		[Export ("openGLPixelFormatForDisplayMask:")]
		NSOpenGLPixelFormat GetOpenGLPixelFormat (uint mask);

		[Export ("openGLContextForPixelFormat:")]
		NSOpenGLContext GetOpenGLContext (NSOpenGLPixelFormat pixelFormat);

		[Export ("canDrawInOpenGLContext:pixelFormat:forLayerTime:displayTime:")]
		bool CanDraw (NSOpenGLContext context, NSOpenGLPixelFormat pixelFormat, double t, ref CVTimeStamp ts);

		[Export ("drawInOpenGLContext:pixelFormat:forLayerTime:displayTime:")]
		void Draw (NSOpenGLContext context, NSOpenGLPixelFormat pixelFormat, double t, ref CVTimeStamp ts);
	}

	[Protocol (IsInformal=true)]
	interface NSToolTipOwner
	{
		[Abstract]
		[Export ("view:stringForToolTip:point:userData:")]
		string GetStringForToolTip (NSView view, nint tag, CGPoint point, IntPtr data);
	}

	interface INSUserInterfaceValidations {}

	[Protocol]
	interface NSUserInterfaceValidations
	{
		[Abstract]
		[Export ("validateUserInterfaceItem:")]
		bool ValidateUserInterfaceItem (INSValidatedUserInterfaceItem item);
	}

	[Protocol (IsInformal=true)]
	interface NSMenuValidation
	{
		[Abstract]
		[Export ("validateMenuItem:")]
		bool ValidateMenuItem (NSMenuItem menuItem);
	}

	public interface INSCandidateListTouchBarItemDelegate {}

	delegate NSAttributedString AttributedStringForCandidateHandler (NSObject candidate, nint index);

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSCandidateListTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[NullAllowed, Export ("client", ArgumentSemantic.Weak)]
		INSTextInputClient Client { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INSCandidateListTouchBarItemDelegate Delegate { get; set; }

		[Export ("collapsed")]
		bool Collapsed { [Bind ("isCollapsed")] get; set; }

		[Export ("allowsCollapsing")]
		bool AllowsCollapsing { get; set; }

		[Export ("candidateListVisible")]
		bool CandidateListVisible { [Bind ("isCandidateListVisible")] get; }

		[Export ("updateWithInsertionPointVisibility:")]
		void UpdateWithInsertionPointVisibility (bool isVisible);

		[Export ("allowsTextInputContextCandidates")]
		bool AllowsTextInputContextCandidates { get; set; }

		[NullAllowed, Export ("attributedStringForCandidate", ArgumentSemantic.Copy)]
		AttributedStringForCandidateHandler AttributedStringForCandidate { get; set; }

		[Export ("candidates", ArgumentSemantic.Copy)]
		NSObject[] Candidates { get; }

		[Export ("setCandidates:forSelectedRange:inString:")]
		void SetCandidates (NSObject[] candidates, NSRange selectedRange, [NullAllowed] string originalString);

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }
	}

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSCandidateListTouchBarItemDelegate
	{
		[Mac (10,12,2)]
		[Export ("candidateListTouchBarItem:beginSelectingCandidateAtIndex:")]
		void BeginSelectingCandidate (NSCandidateListTouchBarItem anItem, nint index);

		[Mac (10,12,2)]
		[Export ("candidateListTouchBarItem:changeSelectionFromCandidateAtIndex:toIndex:")]
		void ChangeSelectionFromCandidate (NSCandidateListTouchBarItem anItem, nint previousIndex, nint index);

		[Mac (10,12,2)]
		[Export ("candidateListTouchBarItem:endSelectingCandidateAtIndex:")]
		void EndSelectingCandidate (NSCandidateListTouchBarItem anItem, nint index);

		[Mac (10,12,2)]
		[Export ("candidateListTouchBarItem:changedCandidateListVisibility:")]
		void ChangedCandidateListVisibility (NSCandidateListTouchBarItem anItem, bool isVisible);
	}

	[Category]
	[BaseType (typeof(NSView))]
	interface NSView_NSCandidateListTouchBarItem
	{
		[Mac (10, 12, 2)]
		[NullAllowed, Export ("candidateListTouchBarItem")]
		NSCandidateListTouchBarItem GetCandidateListTouchBarItem (); 
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSColorPickerTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Static]
		[Export ("colorPickerWithIdentifier:")]
		NSColorPickerTouchBarItem CreateColorPicker(string identifier);

		[Static]
		[Export ("textColorPickerWithIdentifier:")]
		NSColorPickerTouchBarItem CreateTextColorPicker (string identifier);

		[Static]
		[Export ("strokeColorPickerWithIdentifier:")]
		NSColorPickerTouchBarItem CreateStrokeColorPicker(string identifier);

		[Static]
		[Export ("colorPickerWithIdentifier:buttonImage:")]
		NSColorPickerTouchBarItem CreateColorPicker (string identifier, NSImage image);

		[Export ("color", ArgumentSemantic.Copy)]
		NSColor Color { get; set; }

		[Export ("showsAlpha")]
		bool ShowsAlpha { get; set; }

		[Export ("colorList", ArgumentSemantic.Strong)]
		NSColorList ColorList { get; set; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }

		[NullAllowed, Export ("target", ArgumentSemantic.Weak)]
		NSObject Target { get; set; }

		[NullAllowed, Export ("action", ArgumentSemantic.Assign)]
		Selector Action { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Mac (10, 13)]
		[NullAllowed, Export ("allowedColorSpaces", ArgumentSemantic.Copy)]
		NSColorSpace[] AllowedColorSpaces { get; set; }
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSCustomTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Export ("view", ArgumentSemantic.Strong)]
		NSView View { get; set; }

		[Export ("viewController", ArgumentSemantic.Strong)]
		NSViewController ViewController { get; set; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }
	}

	[Category]
	[BaseType (typeof(NSGestureRecognizer))]
	interface NSGestureRecognizer_NSTouchBar
	{
		[Mac (10, 12, 1)]
		[Export ("allowedTouchTypes", ArgumentSemantic.Assign)]
		NSTouchTypeMask GetAllowedTouchTypes ();

		[Mac (10, 12, 1)]
		[Export ("setAllowedTouchTypes:", ArgumentSemantic.Assign)]
		void SetAllowedTouchTypes (NSTouchTypeMask types);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSGroupTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Static]
		[Export ("groupItemWithIdentifier:items:")]
		NSGroupTouchBarItem CreateGroupItem (string identifier, NSTouchBarItem[] items);

		[Export ("groupTouchBar", ArgumentSemantic.Strong)]
		NSTouchBar GroupTouchBar { get; set; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }

		[Mac (10,13)]
		[Static]
		[Export ("groupItemWithIdentifier:items:allowedCompressionOptions:")]
		NSGroupTouchBarItem CreateGroupItem (string identifier, NSTouchBarItem[] items, NSUserInterfaceCompressionOptions allowedCompressionOptions);

		[Mac (10,13)]
		[Static]
		[Export ("alertStyleGroupItemWithIdentifier:")]
		NSGroupTouchBarItem CreateAlertStyleGroupItem (string identifier);

		[Mac (10, 13)]
		[Export ("groupUserInterfaceLayoutDirection", ArgumentSemantic.Assign)]
		NSUserInterfaceLayoutDirection GroupUserInterfaceLayoutDirection { get; set; }

		[Mac (10, 13)]
		[Export ("prefersEqualWidths")]
		bool PrefersEqualWidths { get; set; }

		[Mac (10, 13)]
		[Export ("preferredItemWidth")]
		nfloat PreferredItemWidth { get; set; }

		[Mac (10, 13)]
		[Export ("effectiveCompressionOptions")]
		NSUserInterfaceCompressionOptions EffectiveCompressionOptions { get; }

		[Mac (10, 13)]
		[Export ("prioritizedCompressionOptions", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions[] PrioritizedCompressionOptions { get; set; }
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSPopoverTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Export ("popoverTouchBar", ArgumentSemantic.Strong)]
		NSTouchBar PopoverTouchBar { get; set; }

		[Export ("customizationLabel")]
		string CustomizationLabel { get; set; }

		[Export ("collapsedRepresentation", ArgumentSemantic.Strong)]
		NSView CollapsedRepresentation { get; set; }

		[NullAllowed, Export ("collapsedRepresentationImage", ArgumentSemantic.Strong)]
		NSImage CollapsedRepresentationImage { get; set; }

		[Export ("collapsedRepresentationLabel", ArgumentSemantic.Strong)]
		string CollapsedRepresentationLabel { get; set; }

		[NullAllowed, Export ("pressAndHoldTouchBar", ArgumentSemantic.Strong)]
		NSTouchBar PressAndHoldTouchBar { get; set; }

		[Export ("showsCloseButton")]
		bool ShowsCloseButton { get; set; }

		[Export ("showPopover:")]
		void ShowPopover ([NullAllowed] NSObject sender);

		[Export ("dismissPopover:")]
		void DismissPopover ([NullAllowed] NSObject sender);

		[Export ("makeStandardActivatePopoverGestureRecognizer")]
		NSGestureRecognizer MakeStandardActivatePopoverGestureRecognizer ();
	}
	
	interface INSScrubberDataSource {}
	interface INSScrubberDelegate {}

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSScrubberDataSource
	{
		[Mac (10,12,2)]
		[Abstract]
		[Export ("numberOfItemsForScrubber:")]
		nint GetNumberOfItems (NSScrubber scrubber);

		[Mac (10,12,2)]
		[Abstract]
		[Export ("scrubber:viewForItemAtIndex:")]
		NSScrubberItemView GetViewForItem (NSScrubber scrubber, nint index);
	}

	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSScrubberDelegate
	{
		[Mac (10,12,2)]
		[Export ("scrubber:didSelectItemAtIndex:")]
		void DidSelectItem (NSScrubber scrubber, nint selectedIndex);

		[Mac (10,12,2)]
		[Export ("scrubber:didHighlightItemAtIndex:")]
		void DidHighlightItem (NSScrubber scrubber, nint highlightedIndex);

		[Mac (10,12,2)]
		[Export ("scrubber:didChangeVisibleRange:")]
		void DidChangeVisible (NSScrubber scrubber, NSRange visibleRange);

		[Mac (10,12,2)]
		[Export ("didBeginInteractingWithScrubber:")]
		void DidBeginInteracting (NSScrubber scrubber);

		[Mac (10,12,2)]
		[Export ("didFinishInteractingWithScrubber:")]
		void DidFinishInteracting (NSScrubber scrubber);

		[Mac (10,12,2)]
		[Export ("didCancelInteractingWithScrubber:")]
		void DidCancelInteracting (NSScrubber scrubber);
	}

	[DesignatedDefaultCtor]
	[Mac (10,12,2)]
	[BaseType (typeof(NSObject))]
	interface NSScrubberSelectionStyle : NSCoding
	{
		[Static]
		[Export ("outlineOverlayStyle", ArgumentSemantic.Strong)]
		NSScrubberSelectionStyle OutlineOverlayStyle { get; }

		[Static]
		[Export ("roundedBackgroundStyle", ArgumentSemantic.Strong)]
		NSScrubberSelectionStyle RoundedBackgroundStyle { get; }

		[Export ("makeSelectionView")]
		NSScrubberSelectionView MakeSelectionView ();
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSView))]
	interface NSScrubber
	{
		[NullAllowed, Export ("dataSource", ArgumentSemantic.Weak)]
		INSScrubberDataSource DataSource { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INSScrubberDelegate Delegate { get; set; }

		[Export ("scrubberLayout", ArgumentSemantic.Strong)]
		NSScrubberLayout ScrubberLayout { get; set; }

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		[Export ("highlightedIndex")]
		nint HighlightedIndex { get; }

		[Export ("selectedIndex")]
		nint SelectedIndex { get; set; }

		[Export ("mode", ArgumentSemantic.Assign)]
		NSScrubberMode Mode { get; set; }

		[Export ("itemAlignment", ArgumentSemantic.Assign)]
		NSScrubberAlignment ItemAlignment { get; set; }

		[Export ("continuous")]
		bool Continuous { [Bind ("isContinuous")] get; set; }

		[Export ("floatsSelectionViews")]
		bool FloatsSelectionViews { get; set; }

		[NullAllowed, Export ("selectionBackgroundStyle", ArgumentSemantic.Strong)]
		NSScrubberSelectionStyle SelectionBackgroundStyle { get; set; }

		[NullAllowed, Export ("selectionOverlayStyle", ArgumentSemantic.Strong)]
		NSScrubberSelectionStyle SelectionOverlayStyle { get; set; }

		[Export ("showsArrowButtons")]
		bool ShowsArrowButtons { get; set; }

		[Export ("showsAdditionalContentIndicators")]
		bool ShowsAdditionalContentIndicators { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[NullAllowed, Export ("backgroundView", ArgumentSemantic.Strong)]
		NSView BackgroundView { get; set; }

		[Export ("initWithFrame:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect frameRect);

		[Export ("reloadData")]
		void ReloadData ();

		[Export ("performSequentialBatchUpdates:")]
		void PerformSequentialBatchUpdates (Action updateHandler);

		[Export ("insertItemsAtIndexes:")]
		void InsertItems (NSIndexSet indexes);

		[Export ("removeItemsAtIndexes:")]
		void RemoveItems (NSIndexSet indexes);

		[Export ("reloadItemsAtIndexes:")]
		void ReloadItems (NSIndexSet indexes);

		[Export ("moveItemAtIndex:toIndex:")]
		void MoveItem (nint oldIndex, nint newIndex);

		[Export ("scrollItemAtIndex:toAlignment:")]
		void ScrollItem (nint index, NSScrubberAlignment alignment);

		[Export ("itemViewForItemAtIndex:")]
		NSScrubberItemView GetItemViewForItem(nint index);

		[Export ("registerClass:forItemIdentifier:")]
		void RegisterClass ([NullAllowed] Class itemViewClass, string itemIdentifier);

		[Export ("registerNib:forItemIdentifier:")]
		void RegisterNib ([NullAllowed] NSNib nib, string itemIdentifier);

		[Export ("makeItemWithIdentifier:owner:")]
		NSScrubberItemView MakeItem (string itemIdentifier, [NullAllowed] NSObject owner);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSView))]
	interface NSScrubberArrangedView
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frameRect);

		[Export ("selected")]
		bool Selected { [Bind ("isSelected")] get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[RequiresSuper]
		[Export ("applyLayoutAttributes:")]
		void ApplyLayoutAttributes (NSScrubberLayoutAttributes layoutAttributes);
	}

	// These are empty but types used in other bindings
	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberArrangedView))]
	interface NSScrubberItemView
	{
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberArrangedView))]
	interface NSScrubberSelectionView
	{
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberItemView))]
	interface NSScrubberTextItemView
	{
		[Export ("textField", ArgumentSemantic.Strong)]
		NSTextField TextField { get; }

		[Export ("title")]
		string Title { get; set; }
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberItemView))]
	interface NSScrubberImageItemView
	{
		[Export ("imageView", ArgumentSemantic.Strong)]
		NSImageView ImageView { get; }

		[Export ("image", ArgumentSemantic.Copy)]
		NSImage Image { get; set; }

		[Export ("imageAlignment", ArgumentSemantic.Assign)]
		NSImageAlignment ImageAlignment { get; set; }
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSObject))]
	interface NSScrubberLayoutAttributes : NSCopying
	{
		[Export ("itemIndex")]
		nint ItemIndex { get; set; }

		[Export ("frame", ArgumentSemantic.Assign)]
		CGRect Frame { get; set; }

		[Export ("alpha")]
		nfloat Alpha { get; set; }

		[Static]
		[Export ("layoutAttributesForItemAtIndex:")]
		NSScrubberLayoutAttributes CreateLayoutAttributes (nint index);
	}

	[DesignatedDefaultCtor]
	[Mac (10,12,2)]
	[BaseType (typeof(NSObject))]
	interface NSScrubberLayout : NSCoding
	{
		[Static]
		[Export ("layoutAttributesClass")]
		Class LayoutAttributesClass { get; }

		[NullAllowed, Export ("scrubber", ArgumentSemantic.Weak)]
		NSScrubber Scrubber { get; }

		[Export ("visibleRect")]
		CGRect VisibleRect { get; }

		[RequiresSuper]
		[Export ("invalidateLayout")]
		void InvalidateLayout ();

		[Export ("prepareLayout")]
		void PrepareLayout ();

		[Export ("scrubberContentSize")]
		CGSize ScrubberContentSize { get; }

		[Export ("layoutAttributesForItemAtIndex:")]
		NSScrubberLayoutAttributes LayoutAttributesForItem (nint index);

		[Export ("layoutAttributesForItemsInRect:")]
		NSSet<NSScrubberLayoutAttributes> LayoutAttributesForItems (CGRect rect);

		[Export ("shouldInvalidateLayoutForSelectionChange")]
		bool ShouldInvalidateLayoutForSelectionChange ();

		[Export ("shouldInvalidateLayoutForHighlightChange")]
		bool ShouldInvalidateLayoutForHighlightChange ();

		[Export ("shouldInvalidateLayoutForChangeFromVisibleRect:toVisibleRect:")]
		bool ShouldInvalidateLayoutForChangeFromVisibleRect (CGRect fromVisibleRect, CGRect toVisibleRect);

		[Export ("automaticallyMirrorsInRightToLeftLayout")]
		bool AutomaticallyMirrorsInRightToLeftLayout { get; }
	}

	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface NSScrubberFlowLayoutDelegate : NSScrubberDelegate
	{
		[Export ("scrubber:layout:sizeForItemAtIndex:")]
		CGSize Layout (NSScrubber scrubber, NSScrubberFlowLayout layout, nint itemIndex);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberLayout))]
	interface NSScrubberFlowLayout
	{
		[Export ("itemSpacing")]
		nfloat ItemSpacing { get; set; }

		[Export ("itemSize", ArgumentSemantic.Assign)]
		CGSize ItemSize { get; set; }

		[Export ("invalidateLayoutForItemsAtIndexes:")]
		void InvalidateLayoutForItems (NSIndexSet invalidItemIndexes);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSScrubberLayout))]
	interface NSScrubberProportionalLayout
	{
		[Export ("numberOfVisibleItems")]
		nint NumberOfVisibleItems { get; set; }

		[Export ("initWithNumberOfVisibleItems:")]
		[DesignatedInitializer]
		IntPtr Constructor (nint numberOfVisibleItems);
	}

	public interface INSSharingServicePickerTouchBarItemDelegate {}

	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	interface NSSharingServicePickerTouchBarItemDelegate : NSSharingServicePickerDelegate
	{
		[Abstract]
		[Export ("itemsForSharingServicePickerTouchBarItem:")]
		INSPasteboardWriting [] ItemsForSharingServicePickerTouchBarItem (NSSharingServicePickerTouchBarItem pickerTouchBarItem);
	}

	[Mac (10,12,2)]
	[BaseType (typeof(NSTouchBarItem))]
	[DisableDefaultCtor]
	interface NSSharingServicePickerTouchBarItem
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		INSSharingServicePickerTouchBarItemDelegate Delegate { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("buttonTitle")]
		string ButtonTitle { get; set; }

		[NullAllowed, Export ("buttonImage", ArgumentSemantic.Retain)]
		NSImage ButtonImage { get; set; }
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSSliderAccessory : NSCoding, NSAccessibility, NSAccessibilityElementProtocol
	{
		[Static]
		[Export ("accessoryWithImage:")]
		NSSliderAccessory CreateAccessory (NSImage image);

		[Export ("behavior", ArgumentSemantic.Copy)]
		NSSliderAccessoryBehavior Behavior { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Mac (10, 12, 2)]
		[Field ("NSSliderAccessoryWidthDefault")]
		double DefaultWidth { get; }

		[Mac (10, 12, 2)]
		[Field ("NSSliderAccessoryWidthWide")]
		double WidthWide { get; }
	}

	[Mac (10,12)]
	[BaseType (typeof(NSObject))]
	interface NSSliderAccessoryBehavior : NSCoding, NSCopying
	{
		[Static]
		[Export ("automaticBehavior", ArgumentSemantic.Copy)]
		NSSliderAccessoryBehavior AutomaticBehavior { get; }

		[Static]
		[Export ("valueStepBehavior", ArgumentSemantic.Copy)]
		NSSliderAccessoryBehavior ValueStepBehavior { get; }

		[Static]
		[Export ("valueResetBehavior", ArgumentSemantic.Copy)]
		NSSliderAccessoryBehavior ValueResetBehavior { get; }

		[Static]
		[Export ("behaviorWithTarget:action:")]
		NSSliderAccessoryBehavior CreateBehavior ([NullAllowed] NSObject target, Selector action);

		[Static]
		[Export ("behaviorWithHandler:")]
		NSSliderAccessoryBehavior CreateBehavior (Action<NSSliderAccessory> handler);

		[Export ("handleAction:")]
		void HandleAction (NSSliderAccessory sender);
	}

	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	interface NSAccessibilityCustomAction
	{
		[Export ("initWithName:handler:")]
		IntPtr Constructor (string name, [NullAllowed] Func<bool> handler);

		[Export ("initWithName:target:selector:")]
		IntPtr Constructor (string name, NSObject target, Selector selector);

		[Export ("name")]
		string Name { get; set; }

		[NullAllowed, Export ("handler", ArgumentSemantic.Copy)]
		Func<bool> Handler { get; set; }

		[NullAllowed, Export ("target", ArgumentSemantic.Weak)]
		NSObject Target { get; set; }

		[Advice (@"It must conform to one of the following signatures: 'bool ActionMethod ()' or 'bool ActionMethod (NSAccessibilityCustomAction)' and be decorated with a corresponding [Export].")]
		[NullAllowed, Export ("selector", ArgumentSemantic.Assign)]
		Selector Selector { get; set; }
	}

	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	interface NSAccessibilityCustomRotor
	{
		[Export ("initWithLabel:itemSearchDelegate:")]
		IntPtr Constructor (string label, INSAccessibilityCustomRotorItemSearchDelegate itemSearchDelegate);

		[Export ("initWithRotorType:itemSearchDelegate:")]
		IntPtr Constructor (NSAccessibilityCustomRotorType rotorType, INSAccessibilityCustomRotorItemSearchDelegate itemSearchDelegate);

		[Export ("type", ArgumentSemantic.Assign)]
		NSAccessibilityCustomRotorType Type { get; set; }

		[Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("itemSearchDelegate", ArgumentSemantic.Weak)]
		INSAccessibilityCustomRotorItemSearchDelegate ItemSearchDelegate { get; set; }

		[NullAllowed, Export ("itemLoadingDelegate", ArgumentSemantic.Weak)]
		INSAccessibilityElementLoading ItemLoadingDelegate { get; set; }
	}
	
	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	interface NSAccessibilityCustomRotorSearchParameters
	{
		[NullAllowed, Export ("currentItem", ArgumentSemantic.Strong)]
		NSAccessibilityCustomRotorItemResult CurrentItem { get; set; }

		[Export ("searchDirection", ArgumentSemantic.Assign)]
		NSAccessibilityCustomRotorSearchDirection SearchDirection { get; set; }

		[Export ("filterString")]
		string FilterString { get; set; }
	}
	
	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSAccessibilityCustomRotorItemResult
	{
		[Export ("initWithTargetElement:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSAccessibilityElement targetElement);

		[Export ("initWithItemLoadingToken:customLabel:")]
		[DesignatedInitializer]
		IntPtr Constructor (INSSecureCoding itemLoadingToken, string customLabel);

		[NullAllowed, Export ("targetElement", ArgumentSemantic.Weak)]
		NSAccessibilityElement TargetElement { get; }

		[NullAllowed, Export ("itemLoadingToken", ArgumentSemantic.Strong)]
		INSSecureCoding ItemLoadingToken { get; }

		[Export ("targetRange", ArgumentSemantic.Assign)]
		NSRange TargetRange { get; set; }

		[NullAllowed, Export ("customLabel")]
		string CustomLabel { get; set; }
	}

	interface INSAccessibilityCustomRotorItemSearchDelegate {}

	[Mac (10,13)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface NSAccessibilityCustomRotorItemSearchDelegate
	{
		[Abstract]
		[Export ("rotor:resultForSearchParameters:")]
		[return: NullAllowed]
		NSAccessibilityCustomRotorItemResult GetResult (NSAccessibilityCustomRotor rotor, NSAccessibilityCustomRotorSearchParameters searchParameters);
	}

	interface INSAccessibilityElementLoading {}

	[Mac (10,13)]
	[Protocol]
	interface NSAccessibilityElementLoading
	{
		[Abstract]
		[Export ("accessibilityElementWithToken:")]
		[return: NullAllowed]
		NSAccessibilityElement GetAccessibilityElement (INSSecureCoding token);

		[Export ("accessibilityRangeInTargetElementWithToken:")]
		NSRange GetAccessibilityRangeInTargetElement (INSSecureCoding token);
	}

	interface INSCollectionViewPrefetching { }

	[Mac (10,13)]
	[Protocol]
	interface NSCollectionViewPrefetching
	{
		[Abstract]
		[Export ("collectionView:prefetchItemsAtIndexPaths:")]
		void PrefetchItems (NSCollectionView collectionView, NSIndexPath[] indexPaths);

		[Export ("collectionView:cancelPrefetchingForItemsAtIndexPaths:")]
		void CancelPrefetching (NSCollectionView collectionView, NSIndexPath[] indexPaths);
	}

	delegate bool DownloadFontAssetsRequestCompletionHandler (NSError error);

	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSFontAssetRequest : INSProgressReporting
	{
		[Export ("initWithFontDescriptors:options:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSFontDescriptor[] fontDescriptors, NSFontAssetRequestOptions options);

		[Export ("downloadedFontDescriptors", ArgumentSemantic.Copy)]
		NSFontDescriptor[] DownloadedFontDescriptors { get; }

		[Export ("progress", ArgumentSemantic.Strong)]
		NSProgress Progress { get; }

		[Export ("downloadFontAssetsWithCompletionHandler:")]
		void DownloadFontAssets (DownloadFontAssetsRequestCompletionHandler completionHandler);
	}

	[Category]
	[BaseType (typeof(NSObject))]
	interface NSObject_NSFontPanelValidationAdditions
	{
		[Export ("validModesForFontPanel:")]
		NSFontPanelModeMask GetValidModes (NSFontPanel fontPanel);
	}

	[DesignatedDefaultCtor]
	[Mac (10, 13)]
	[BaseType (typeof(NSObject))]
	interface NSUserInterfaceCompressionOptions : NSCopying, NSCoding
	{
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier);

		[Export ("initWithCompressionOptions:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSSet<NSUserInterfaceCompressionOptions> options);

		[Export ("containsOptions:")]
		bool Contains (NSUserInterfaceCompressionOptions options);

		[Export ("intersectsOptions:")]
		bool Intersects (NSUserInterfaceCompressionOptions options);

		[Export ("empty")]
		bool Empty { [Bind ("isEmpty")] get; }

		[Export ("optionsByAddingOptions:")]
		NSUserInterfaceCompressionOptions GetOptionsByAdding (NSUserInterfaceCompressionOptions options);

		[Export ("optionsByRemovingOptions:")]
		NSUserInterfaceCompressionOptions GetOptionsByRemoving (NSUserInterfaceCompressionOptions options);

		[Static]
		[Export ("hideImagesOption", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions HideImagesOption { get; }

		[Static]
		[Export ("hideTextOption", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions HideTextOption { get; }

		[Static]
		[Export ("reduceMetricsOption", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions ReduceMetricsOption { get; }

		[Static]
		[Export ("breakEqualWidthsOption", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions BreakEqualWidthsOption { get; }

		[Static]
		[Export ("standardOptions", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions StandardOptions { get; }
	}

	interface INSUserInterfaceCompression { }

	[Mac (10, 13)]
	[Protocol]
	interface NSUserInterfaceCompression
	{
		[Abstract]
		[Export ("compressWithPrioritizedCompressionOptions:")]
		void Compress (NSUserInterfaceCompressionOptions[] prioritizedOptions);

		[Abstract]
		[Export ("minimumSizeWithPrioritizedCompressionOptions:")]
		CGSize GetMinimumSize (NSUserInterfaceCompressionOptions[] prioritizedOptions);

		[Abstract]
		[Export ("activeCompressionOptions", ArgumentSemantic.Copy)]
		NSUserInterfaceCompressionOptions ActiveCompressionOptions { get; }
	}

	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	interface NSWindowTab
	{
		[Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[Export ("toolTip")]
		string ToolTip { get; set; }

		[NullAllowed, Export ("accessoryView", ArgumentSemantic.Strong)]
		NSView AccessoryView { get; set; }
	}

	[Mac (10,13)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSWindowTabGroup
	{
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("windows", ArgumentSemantic.Copy)]
		NSWindow[] Windows { get; }

		[Export ("overviewVisible")]
		bool OverviewVisible { [Bind ("isOverviewVisible")] get; set; }

		[Export ("tabBarVisible")]
		bool TabBarVisible { [Bind ("isTabBarVisible")] get; }

		[NullAllowed, Export ("selectedWindow", ArgumentSemantic.Weak)]
		NSWindow SelectedWindow { get; set; }

		[Export ("addWindow:")]
		void Add (NSWindow window);

		[Export ("insertWindow:atIndex:")]
		void Insert (NSWindow window, nint index);

		[Export ("removeWindow:")]
		void Remove (NSWindow window);
	}
}
