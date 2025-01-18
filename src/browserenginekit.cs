#if NET

using System;

using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UniformTypeIdentifiers;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif

#if MONOMAC
using BEDirectionalTextRange = Foundation.NSObject;
using IUIContextMenuInteractionDelegate = Foundation.NSObject;
using IUIDragSession = Foundation.NSObject;
using IUIEditMenuInteractionAnimating = Foundation.NSObject;
using UIColor = Foundation.NSObject;
using UIContextMenuConfiguration = Foundation.NSObject;
using UIContextMenuInteraction = Foundation.NSObject;
using UIDragInteraction = Foundation.NSObject;
using UIDragInteractionDelegate = Foundation.NSObject;
using UIDragItem = Foundation.NSObject;
using UIEditMenuArrowDirection = Foundation.NSObject;
using UIGestureRecognizerState = Foundation.NSObject;
using UIKey = Foundation.NSObject;
using UIScrollView = Foundation.NSObject;
using UIScrollViewDelegate = Foundation.NSObject;
using UITextGranularity = Foundation.NSObject;
using UITextLayoutDirection = Foundation.NSObject;
using UITextPlaceholder = Foundation.NSObject;
using UITextPosition = Foundation.NSObject;
using UITextRange = Foundation.NSObject;
using UITextSelectionDisplayInteraction = Foundation.NSObject;
using UITextSelectionRect = Foundation.NSObject;
using UITextStorageDirection = Foundation.NSObject;
using UIView = Foundation.NSObject;
#endif

#if MONOMAC
using IUIInteraction = Foundation.NSObject;
using UIInteraction = Foundation.NSObjectProtocol;
using UIKeyInput = Foundation.NSObjectProtocol;
using UIResponderStandardEditActions = Foundation.NSObjectProtocol;
using UITextInputTraits = Foundation.NSObjectProtocol;
#endif

using OS_xpc_object = Foundation.NSObject;

namespace BrowserEngineKit {
	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BELayerHierarchyHandle : NSSecureCoding
	{
		[Static]
		[Export ("handleWithXPCRepresentation:error:")]
		[return: NullAllowed]
		BELayerHierarchyHandle Create ([NullAllowed] OS_xpc_object xpcRepresentation, [NullAllowed] out NSError error);

		[Export ("createXPCRepresentation")]
		OS_xpc_object CreateXpcRepresentation ();
	}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BELayerHierarchy
	{
		[Static]
		[Export ("layerHierarchyWithError:")]
		[return: NullAllowed]
		BELayerHierarchy Create ([NullAllowed] out NSError error);

		[Export ("handle", ArgumentSemantic.Strong)]
		BELayerHierarchyHandle LayerHierarchyHandle { get; }

		[Export ("layer", ArgumentSemantic.Strong), NullAllowed]
		CALayer Layer { get; set; }

		[Export ("invalidate")]
		void Invalidate ();
	}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (UIView))]
	interface BELayerHierarchyHostingView
	{
		[Export ("handle", ArgumentSemantic.Strong), NullAllowed]
		BELayerHierarchyHandle LayerHierarchyHandle { get; set; }
	}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BELayerHierarchyHostingTransactionCoordinator : NSSecureCoding
	{
		[Static]
		[Export ("coordinatorWithError:")]
		[return: NullAllowed]
		BELayerHierarchyHostingTransactionCoordinator Create ([NullAllowed] out NSError error);

		[Static]
		[Export ("coordinatorWithXPCRepresentation:error:")]
		[return: NullAllowed]
		BELayerHierarchyHostingTransactionCoordinator Create ([NullAllowed] OS_xpc_object xpcRepresentation, [NullAllowed] out NSError error);

		// -(xpc_object_t _Nonnull)createXPCRepresentation __attribute__((swift_name("createXPCRepresentation()")));
		[Export ("createXPCRepresentation")]
		OS_xpc_object CreateXpcRepresentation ();

		[Export ("addLayerHierarchy:")]
		void Add (BELayerHierarchy layerHierarchy);

		[Export ("addLayerHierarchyHostingView:")]
		void Add (BELayerHierarchyHostingView hostingView);

		[Export ("commit")]
		void Commit ();
	}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(UIContextMenuConfiguration))]
	[DisableDefaultCtor]
	interface BEContextMenuConfiguration
	{
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("fulfillUsingConfiguration:")]
		bool Fulfill ([NullAllowed] UIContextMenuConfiguration configuration);
	}

	delegate bool BEDragInteractionDelegateGetDragItemsCallback (UIDragItem[] items);

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (UIDragInteractionDelegate))]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	interface BEDragInteractionDelegate
	{
		[Export ("dragInteraction:prepareDragSession:completion:")]
		void PrepareDragSession (BEDragInteraction dragInteraction, IUIDragSession session, Func<bool> completion);

		[Export ("dragInteraction:itemsForAddingToSession:forTouchAtPoint:completion:")]
		void GetDragItems (BEDragInteraction dragInteraction, IUIDragSession session, CGPoint point, BEDragInteractionDelegateGetDragItemsCallback completion);
	}

	interface IBEDragInteractionDelegate {}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (UIDragInteraction))]
	interface BEDragInteraction
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IBEDragInteractionDelegate Delegate { get; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; }

		[Export ("initWithDelegate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IBEDragInteractionDelegate @delegate);
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (UIScrollViewDelegate))]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	interface BEScrollViewDelegate
	{
		[Export ("scrollView:handleScrollUpdate:completion:")]
		void HandleScrollUpdate (BEScrollView scrollView, BEScrollViewScrollUpdate scrollUpdate, Action<bool> completion);

		[Export ("parentScrollViewForScrollView:")]
		[return: NullAllowed]
		BEScrollView GetParentScrollView (BEScrollView scrollView);
	}

	interface IBEScrollViewDelegate {}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(UIScrollView))]
	interface BEScrollView
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IBEScrollViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BEScrollViewScrollUpdatePhase : long
	{
		Began,
		Changed,
		Ended,
		Cancelled,
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEScrollViewScrollUpdate
	{
		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("phase")]
		BEScrollViewScrollUpdatePhase Phase { get; }

		[Export ("locationInView:")]
		CGPoint GetLocation ([NullAllowed] UIView view);

		[Export ("translationInView:")]
		CGPoint GetTranslation ([NullAllowed] UIView view);
	}

	delegate void BEWebContentProcessCreateCallback ([NullAllowed] BEWebContentProcess proces, [NullAllowed] NSError error);

	[NoTV, Mac (14,3), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEWebContentProcess
	{
		[Static]
		[Export ("webContentProcessWithInterruptionHandler:completion:")]
		[Async]
		void Create (Action interruptionHandler, BEWebContentProcessCreateCallback completion);

		[Static]
		[Export ("webContentProcessWithBundleID:interruptionHandler:completion:")]
		[Async]
		void Create (string bundleId, Action interruptionHandler, BEWebContentProcessCreateCallback completion);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("makeLibXPCConnectionError:")]
		[return: NullAllowed]
		OS_xpc_object MakeLibXpcConnection ([NullAllowed] out NSError error);

		[NoMac]
		[Export ("createVisibilityPropagationInteraction")]
		IUIInteraction CreateVisibilityPropagationInteraction ();

		// Inlined from the Capability (BEWebContentProcess) category
		[Export ("grantCapability:error:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error);

		// Inlined from the CapabilityInvalidationHandler (BEWebContentProcess) category
		[NoTV, NoMac, iOS (17,6), MacCatalyst (17, 6)]
		[Export ("grantCapability:error:invalidationHandler:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error, Action invalidationHandler);
	}

	delegate void BENetworkingProcessCreateCallback ([NullAllowed] BENetworkingProcess proces, [NullAllowed] NSError error);

	[NoTV, Mac (14,3), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BENetworkingProcess
	{
		[Static]
		[Export ("networkProcessWithInterruptionHandler:completion:")]
		[Async]
		void Create (Action interruptionHandler, BENetworkingProcessCreateCallback completion);

		[iOS (18, 2), Mac (15, 2), MacCatalyst (18, 2)]
		[Static]
		[Export ("networkProcessWithBundleID:interruptionHandler:completion:")]
		void Create (string bundleId, Action interruptionHandler, BENetworkingProcessCreateCallback completion);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("makeLibXPCConnectionError:")]
		[return: NullAllowed]
		OS_xpc_object MakeLibXpcConnection ([NullAllowed] out NSError error);


		// Inlined from the Capability (BENetworkingProcess) category
		[Export ("grantCapability:error:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error);

		// Inlined from the CapabilityInvalidationHandler (BENetworkingProcess) category
		[NoMac, NoTV, iOS (17, 6), MacCatalyst (17, 6)]
		[Export ("grantCapability:error:invalidationHandler:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error, Action invalidationHandler);
	}

	delegate void BERenderingProcessCreateCallback ([NullAllowed] BERenderingProcess proces, [NullAllowed] NSError error);

	[NoTV, Mac (14, 3), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BERenderingProcess
	{
		[Static]
		[Export ("renderingProcessWithInterruptionHandler:completion:")]
		[Async]
		void Create (Action interruptionHandler, BERenderingProcessCreateCallback completion);

		[Static]
		[Export ("renderingProcessWithBundleID:interruptionHandler:completion:")]
		[Async]
		void Create (string bundleId, Action interruptionHandler, BERenderingProcessCreateCallback completion);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("makeLibXPCConnectionError:")]
		[return: NullAllowed]
		OS_xpc_object MakeLibXpcConnection ([NullAllowed] out NSError error);

		[NoMac]
		[Export ("createVisibilityPropagationInteraction")]
		IUIInteraction CreateVisibilityPropagationInteraction ();

		// Inlined from the Capability (BERenderingProcess) category
		[Export ("grantCapability:error:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error);

		// Inlined from the CapabilityInvalidationHandler (BERenderingProcess) category
		[NoTV, NoMac, iOS (17,6), MacCatalyst (17, 6)]
		[Export ("grantCapability:error:invalidationHandler:")]
		[return: NullAllowed]
		IBEProcessCapabilityGrant GrantCapability (BEProcessCapability capability, [NullAllowed] out NSError error, Action invalidationHandler);
	}

	// Headers say this is available on macOS 14.3, but the BETextInput type isn't, so just remove this type from macOS as well.
	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	interface BETextInputDelegate
	{
		[Abstract]
		[Export ("shouldDeferEventHandlingToSystemForTextInput:context:")]
		bool ShouldDeferEventHandlingToSystem (IBETextInput textInput, BEKeyEntryContext keyEventContext);

		[Abstract]
		[Export ("textInput:setCandidateSuggestions:")]
		void SetCandidateSuggestions (IBETextInput textInput, [NullAllowed] BETextSuggestion[] suggestions);

		[Abstract]
		[Export ("selectionWillChangeForTextInput:")]
		void SelectionWillChange (IBETextInput textInput);

		[Abstract]
		[Export ("selectionDidChangeForTextInput:")]
		void SelectionDidChange (IBETextInput textInput);

		[Abstract]
		[Export ("textInput:deferReplaceTextActionToSystem:")]
		void DeferReplaceTextActionToSystem (IBETextInput textInput, NSObject sender);

		[Abstract]
		[Export ("invalidateTextEntryContextForTextInput:")]
		void InvalidateTextEntryContext (IBETextInput textInput);
	}

	interface IBETextInputDelegate {}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BEKeyPressState : long
	{
		Down = 1,
		Up = 2,
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEKeyEntry
	{
		[Export ("key")]
		UIKey Key { get; }

		[Export ("state")]
		BEKeyPressState State { get; }

		[Export ("keyRepeating")]
		bool KeyRepeating { [Bind ("isKeyRepeating")] get; }

		[Export ("timestamp")]
		double Timestamp { get; }
	}

	[TV (17, 4), Mac (14, 4), iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BEGestureType : long
	{
		Loupe = 0,
		OneFingerTap = 1,
		DoubleTapAndHold = 2,
		DoubleTap = 3,
		OneFingerDoubleTap = 8,
		OneFingerTripleTap = 9,
		TwoFingerSingleTap = 10,
		TwoFingerRangedSelectGesture = 11,
		IMPhraseBoundaryDrag = 14,
		ForceTouch = 15,
	}

	[TV (17, 4), Mac (14, 4), iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BESelectionTouchPhase : long
	{
		Started,
		Moved,
		Ended,
		EndedMovingForward,
		EndedMovingBackward,
		EndedNotMoving,
	}

	[Flags]
	[TV (17, 4), Mac (14, 4), iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BESelectionFlags : ulong
	{
		None = 0x0,
		WordIsNearTap = 1uL << 0,
		SelectionFlipped = 1uL << 1,
		PhraseBoundaryChanged = 1uL << 2,
	}

	[TV (17, 4), Mac (14, 4), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BETextAlternatives
	{
		[Export ("primaryString")]
		string PrimaryString { get; }

		[Export ("alternativeStrings")]
		string[] AlternativeStrings { get; }
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Flags]
	[Native]
	public enum BETextReplacementOptions : ulong
	{
		None = 0x0,
		AddUnderline = 1uL << 0,
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Flags]
	[Native]
	public enum BEKeyModifierFlags : long
	{
		None,
		Shift,
		CapsLock,
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BEResponderEditActions : UIResponderStandardEditActions
	{
		[Export ("share:")]
		void Share ([NullAllowed] NSObject sender);

		[Export ("addShortcut:")]
		void AddShortcut ([NullAllowed] NSObject sender);

		[Export ("lookup:")]
		void Lookup ([NullAllowed] NSObject sender);

		[Export ("findSelected:")]
		void FindSelected ([NullAllowed] NSObject sender);

		[Export ("promptForReplace:")]
		void PromptForReplace ([NullAllowed] NSObject sender);

		[Export ("replace:")]
		void Replace ([NullAllowed] NSObject sender);

		[Export ("translate:")]
		void Translate ([NullAllowed] NSObject sender);

		[Export ("transliterateChinese:")]
		void TransliterateChinese ([NullAllowed] NSObject sender);
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BETextSelectionDirectionNavigation
	{
		[Abstract]
		[Export ("moveInLayoutDirection:")]
		void MoveInLayoutDirection (UITextLayoutDirection direction);

		[Abstract]
		[Export ("extendInLayoutDirection:")]
		void ExtendInLayoutDirection (UITextLayoutDirection direction);

		[Abstract]
		[Export ("moveInStorageDirection:byGranularity:")]
		void MoveInStorageDirection (UITextStorageDirection direction, UITextGranularity granularity);

		[Abstract]
		[Export ("extendInStorageDirection:byGranularity:")]
		void ExtendInStorageDirection (UITextStorageDirection direction, UITextGranularity granularity);
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BEExtendedTextInputTraits : UITextInputTraits
	{
		[Export ("singleLineDocument")]
		bool SingleLineDocument { [Bind ("isSingleLineDocument")] get; }

		[Export ("typingAdaptationEnabled")]
		bool TypingAdaptationEnabled { [Bind ("isTypingAdaptationEnabled")] get; }

		[NullAllowed, Export ("insertionPointColor")]
		UIColor InsertionPointColor { get; }

		[NullAllowed, Export ("selectionHandleColor")]
		UIColor SelectionHandleColor { get; }

		[NullAllowed, Export ("selectionHighlightColor")]
		UIColor SelectionHighlightColor { get; }
	}

	interface IBEExtendedTextInputTraits {}

	delegate void BETextInputHandleKeyEntryCallback (BEKeyEntry entry, bool handled);
	delegate void BETextInputReplaceTextCallback (UITextSelectionRect[] rects);
	delegate void BETextInputRequestTextContextForAutocorrectionCallback (BETextDocumentContext context);
	delegate void BETextInputRequestTextRectsCallback (UITextSelectionRect[] rects);

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BETextInput : UIKeyInput, BETextSelectionDirectionNavigation, BEResponderEditActions
	{
		[Wrap ("WeakAsyncInputDelegate"), Abstract]
		[NullAllowed]
		IBETextInputDelegate AsyncInputDelegate { get; set; }

		[Abstract]
		[NullAllowed, Export ("asyncInputDelegate", ArgumentSemantic.Weak)]
		NSObject WeakAsyncInputDelegate { get; set; }

		[Abstract]
		[Export ("canPerformAction:withSender:")]
		bool CanPerformAction (Selector action, [NullAllowed] NSObject sender);

		[Abstract]
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; }

		[Abstract]
		[Export ("handleKeyEntry:withCompletionHandler:")]
		void HandleKeyEntry (BEKeyEntry entry, BETextInputHandleKeyEntryCallback completionHandler);

		[Abstract]
		[Export ("shiftKeyStateChangedFromState:toState:")]
		void ShiftKeyStateChanged (BEKeyModifierFlags oldState, BEKeyModifierFlags newState);

		[Abstract]
		[Export ("textInRange:")]
		[return: NullAllowed]
		string GetText (UITextRange range);

		[Abstract]
		[Export ("offsetFromPosition:toPosition:")]
		nint GetOffset (UITextPosition from, UITextPosition toPosition);

		[Abstract]
		[Export ("setBaseWritingDirection:forRange:")]
		void SetBaseWritingDirection (NSWritingDirection writingDirection, UITextRange range);

		[Abstract]
		[Export ("deleteInDirection:toGranularity:")]
		void Delete (UITextStorageDirection direction, UITextGranularity granularity);

		[Abstract]
		[Export ("transposeCharactersAroundSelection")]
		void TransposeCharactersAroundSelection ();

		[Abstract]
		[Export ("replaceText:withText:options:completionHandler:")]
		void ReplaceText (string originalText, string replacementText, BETextReplacementOptions options, BETextInputReplaceTextCallback completionHandler);

		[Abstract]
		[Export ("requestTextContextForAutocorrectionWithCompletionHandler:")]
		void RequestTextContextForAutocorrection (BETextInputRequestTextContextForAutocorrectionCallback completionHandler);

		[Abstract]
		[Export ("requestTextRectsForString:withCompletionHandler:")]
		void RequestTextRects (string input, BETextInputRequestTextRectsCallback completionHandler);

		[Abstract]
		[Export ("automaticallyPresentEditMenu")]
		bool AutomaticallyPresentEditMenu { get; }

		[NoTV]
		[Abstract]
		[Export ("requestPreferredArrowDirectionForEditMenuWithCompletionHandler:")]
		void RequestPreferredArrowDirectionForEditMenuWithCompletionHandler (Action<UIEditMenuArrowDirection> completionHandler);

		[NoTV]
		[Abstract]
		[Export ("systemWillPresentEditMenuWithAnimator:")]
		void SystemWillPresentEditMenu (IUIEditMenuInteractionAnimating animator);

		[NoTV]
		[Abstract]
		[Export ("systemWillDismissEditMenuWithAnimator:")]
		void SystemWillDismissEditMenu (IUIEditMenuInteractionAnimating animator);

		[Abstract]
		[NullAllowed, Export ("extendedTextInputTraits")]
		IBEExtendedTextInputTraits ExtendedTextInputTraits { get; }

		[Abstract]
		[Export ("textStylingAtPosition:inDirection:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> GetTextStyling (UITextPosition position, UITextStorageDirection direction);

		[Abstract]
		[Export ("replaceAllowed")]
		bool ReplaceAllowed { [Bind ("isReplaceAllowed")] get; }

		[Abstract]
		[Export ("replaceSelectedText:withText:")]
		void ReplaceSelectedText (string text, string replacementText);

		[Abstract]
		[Export ("updateCurrentSelectionTo:fromGesture:inState:")]
		void UpdateCurrentSelection (CGPoint point, BEGestureType gestureType, UIGestureRecognizerState state);

		[Abstract]
		[Export ("setSelectionFromPoint:toPoint:gesture:state:")]
		void SetSelection (CGPoint from, CGPoint to, BEGestureType gesture, UIGestureRecognizerState state);

		[Abstract]
		[Export ("adjustSelectionBoundaryToPoint:touchPhase:baseIsStart:flags:")]
		void AdjustSelectionBoundary (CGPoint point, BESelectionTouchPhase touch, bool boundaryIsStart, BESelectionFlags flags);

		[Abstract]
		[Export ("textInteractionGesture:shouldBeginAtPoint:")]
		bool ShouldTextInteractionGestureBeginAtPoint (BEGestureType gestureType, CGPoint point);

		[Abstract]
		[NullAllowed, Export ("selectedText")]
		string SelectedText { get; }

		[Abstract]
		[NullAllowed, Export ("selectedTextRange", ArgumentSemantic.Copy)]
		UITextRange SelectedTextRange { get; set; }

		[Abstract]
		[Export ("selectionAtDocumentStart")]
		bool SelectionAtDocumentStart { [Bind ("isSelectionAtDocumentStart")] get; }

		[Abstract]
		[Export ("caretRectForPosition:")]
		CGRect GetCaretRect (UITextPosition position);

		[Abstract]
		[Export ("selectionRectsForRange:")]
		UITextSelectionRect[] GetSelectionRects (UITextRange range);

		[Abstract]
		[Export ("selectWordForReplacement")]
		void SelectWordForReplacement ();

		[Abstract]
		[Export ("updateSelectionWithExtentPoint:boundary:completionHandler:")]
		void UpdateSelection (CGPoint extentPoint, UITextGranularity granularity, Action<bool> completionHandler);

		[Abstract]
		[Export ("selectTextInGranularity:atPoint:completionHandler:")]
		void SelectText (UITextGranularity granularity, CGPoint point, Action completionHandler);

		[Abstract]
		[Export ("selectPositionAtPoint:completionHandler:")]
		void SelectPosition (CGPoint point, Action completionHandler);

		[Abstract]
		[Export ("selectPositionAtPoint:withContextRequest:completionHandler:")]
		void SelectPosition (CGPoint point, BETextDocumentRequest request, Action<BETextDocumentContext> completionHandler);

		[Abstract]
		[Export ("adjustSelectionByRange:completionHandler:")]
		void AdjustSelection (BEDirectionalTextRange range, Action completionHandler);

		[Abstract]
		[Export ("moveByOffset:")]
		void Move (nint offset);

		[Abstract]
		[Export ("moveSelectionAtBoundary:inStorageDirection:completionHandler:")]
		void MoveSelectionAtBoundary (UITextGranularity granularity, UITextStorageDirection direction, Action completionHandler);

		[Abstract]
		[Export ("selectTextForEditMenuWithLocationInView:completionHandler:")]
		void SelectTextForEditMenu (CGPoint locationInView, Action<bool, NSString, NSRange> completionHandler);

		[Abstract]
		[NullAllowed, Export ("markedText")]
		string MarkedText { get; }

		[Abstract]
		[NullAllowed, Export ("attributedMarkedText")]
		NSAttributedString AttributedMarkedText { get; }

		[Abstract]
		[NullAllowed, Export ("markedTextRange")]
		UITextRange MarkedTextRange { get; }

		[Abstract]
		[Export ("hasMarkedText")]
		bool HasMarkedText { get; }

		[Abstract]
		[Export ("setMarkedText:selectedRange:")]
		void SetMarkedText ([NullAllowed] string markedText, NSRange selectedRange);

		[Abstract]
		[Export ("setAttributedMarkedText:selectedRange:")]
		void SetAttributedMarkedText ([NullAllowed] NSAttributedString markedText, NSRange selectedRange);

		[Abstract]
		[Export ("unmarkText")]
		void UnmarkText ();

		[Abstract]
		[Export ("isPointNearMarkedText:")]
		bool IsPointNearMarkedText (CGPoint point);

		[Abstract]
		[Export ("requestDocumentContext:completionHandler:")]
		void RequestDocumentContext (BETextDocumentRequest request, Action<BETextDocumentContext> completionHandler);

		[Abstract]
		[Export ("willInsertFinalDictationResult")]
		void WillInsertFinalDictationResult ();

		[Abstract]
		[Export ("replaceDictatedText:withText:")]
		void ReplaceDictatedText (string oldText, string newText);

		[Abstract]
		[Export ("didInsertFinalDictationResult")]
		void DidInsertFinalDictationResult ();

		[Abstract]
		[return: NullAllowed]
		[Export ("alternativesForSelectedText")]
		BETextAlternatives[] GetAlternativesForSelectedText ();

		[Abstract]
		[Export ("addTextAlternatives:")]
		void AddTextAlternatives (BETextAlternatives alternatives);

		[Abstract]
		[Export ("insertTextAlternatives:")]
		void InsertTextAlternatives (BETextAlternatives alternatives);

		[iOS (18,0), MacCatalyst (18, 0), TV (18, 0)]
		[Export ("removeTextAlternatives")]
		void RemoveTextAlternatives ();

		[Abstract]
		[Export ("insertTextPlaceholderWithSize:completionHandler:")]
		void InsertTextPlaceholder (CGSize size, Action<UITextPlaceholder> completionHandler);

		[Abstract]
		[Export ("removeTextPlaceholder:willInsertText:completionHandler:")]
		void RemoveTextPlaceholder (UITextPlaceholder placeholder, bool willInsertText, Action completionHandler);

		[Abstract]
		[Export ("insertTextSuggestion:")]
		void InsertTextSuggestion (BETextSuggestion textSuggestion);

		[Abstract]
		[Export ("textInputView")]
		UIView TextInputView { get; }

		[Abstract]
		[Export ("textFirstRect")]
		CGRect TextFirstRect { get; }

		[Abstract]
		[Export ("textLastRect")]
		CGRect TextLastRect { get; }

		[Abstract]
		[Export ("unobscuredContentRect")]
		CGRect UnobscuredContentRect { get; }

		[Abstract]
		[Export ("unscaledView")]
		UIView UnscaledView { get; }

		[Abstract]
		[Export ("selectionClipRect")]
		CGRect SelectionClipRect { get; }

		[Abstract]
		[Export ("autoscrollToPoint:")]
		void Autoscroll (CGPoint point);

		[Abstract]
		[Export ("cancelAutoscroll")]
		void CancelAutoscroll ();

		[iOS (18,0), MacCatalyst (18, 0), TV (18, 0)]
		[Export ("keyboardWillDismiss")]
		void KeyboardWillDismiss ();
	}

	interface IBETextInput {}

	[TV (17, 4), Mac (14, 4), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BETextSuggestion
	{
		[Export ("initWithInputText:")]
		NativeHandle Constructor (string inputText);

		[Export ("inputText")]
		string InputText { get; }
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(BETextSuggestion))]
	[DisableDefaultCtor]
	interface BEAutoFillTextSuggestion
	{
		[Export ("contents", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSString> Contents { get; }
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	interface BETextInteraction : UIInteraction
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IBETextInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("addShortcutForText:fromRect:")]
		void AddShortcut (string text, CGRect presentationRect);

		[Export ("shareText:fromRect:")]
		void Share (string text, CGRect presentationRect);

		[Export ("showDictionaryForTextInContext:definingTextInRange:fromRect:")]
		void ShowDictionary (string textWithContext, NSRange range, CGRect presentationRect);

		[Export ("translateText:fromRect:")]
		void Translate (string text, CGRect presentationRect);

		[Export ("showReplacementsForText:")]
		void ShowReplacements (string text);

		[Export ("transliterateChineseForText:")]
		void TransliterateChinese (string text);
		[Export ("presentEditMenuForSelection")]
		void PresentEditMenuForSelection ();

		[Export ("dismissEditMenuForSelection")]
		void DismissEditMenuForSelection ();

		[Export ("editabilityChanged")]
		void EditabilityChanged ();

		[Export ("refreshKeyboardUI")]
		void RefreshKeyboardUI ();

		[Export ("selectionChangedWithGestureAtPoint:gesture:state:flags:")]
		void SelectionChangedWithGesture (CGPoint point, BEGestureType gestureType, UIGestureRecognizerState gestureState, BESelectionFlags flags);

		[Export ("selectionBoundaryAdjustedToPoint:touchPhase:flags:")]
		void SelectionBoundaryAdjusted (CGPoint point, BESelectionTouchPhase touch, BESelectionFlags flags);

		[Export ("textSelectionDisplayInteraction")]
		UITextSelectionDisplayInteraction TextSelectionDisplayInteraction { get; }

		[NoTV]
		[Wrap ("WeakContextMenuInteractionDelegate")]
		[NullAllowed]
		IUIContextMenuInteractionDelegate ContextMenuInteractionDelegate { get; set; }

		[NoTV]
		[NullAllowed, Export ("contextMenuInteractionDelegate", ArgumentSemantic.Weak)]
		NSObject WeakContextMenuInteractionDelegate { get; set; }

		[NoTV]
		[Export ("contextMenuInteraction")]
		UIContextMenuInteraction ContextMenuInteraction { get; }
	}

	// Headers say this is available on macOS 14.3, but the BETextInteraction type isn't, so just remove this type from macOS as well.
	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof (NSObject))]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	interface BETextInteractionDelegate
	{
		[Abstract]
		[Export ("systemWillChangeSelectionForInteraction:")]
		void SystemWillChangeSelection (BETextInteraction textInteraction);

		[Abstract]
		[Export ("systemDidChangeSelectionForInteraction:")]
		void SystemDidChangeSelection (BETextInteraction textInteraction);
	}

	interface IBETextInteractionDelegate {}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEKeyEntryContext
	{
		[Export ("keyEntry", ArgumentSemantic.Strong)]
		BEKeyEntry KeyEntry { get; }

		[Export ("documentEditable")]
		bool DocumentEditable { [Bind ("isDocumentEditable")] get; set; }

		[Export ("shouldInsertCharacter")]
		bool ShouldInsertCharacter { get; set; }

		[Export ("shouldEvaluateForInputSystemHandling")]
		bool ShouldEvaluateForInputSystemHandling { get; set; }

		[Export ("initWithKeyEntry:")]
		[DesignatedInitializer]
		NativeHandle Constructor (BEKeyEntry keyEntry);
	}

	[Flags]
	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[Native]
	public enum BETextDocumentRequestOptions : long
	{
		None = 0x0,
		Text = 1L << 0,
		AttributedText = 1L << 1,
		TextRects = 1L << 2,
		MarkedTextRects = 1L << 5,
		AutocorrectedRanges = 1L << 7,
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BETextDocumentRequest
	{
		[Export ("options", ArgumentSemantic.Assign)]
		BETextDocumentRequestOptions Options { get; set; }

		[Export ("surroundingGranularity", ArgumentSemantic.Assign)]
		UITextGranularity SurroundingGranularity { get; set; }

		[Export ("granularityCount")]
		nint GranularityCount { get; set; }
	}

	[TV (17, 4), NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BETextDocumentContext
	{
		[Export ("initWithSelectedText:contextBefore:contextAfter:markedText:selectedRangeInMarkedText:")]
		NativeHandle Constructor ([NullAllowed] string selectedText, [NullAllowed] string contextBefore, [NullAllowed] string contextAfter, [NullAllowed] string markedText, NSRange selectedRangeInMarkedText);

		[Export ("initWithAttributedSelectedText:contextBefore:contextAfter:markedText:selectedRangeInMarkedText:")]
		NativeHandle Constructor ([NullAllowed] NSAttributedString selectedText, [NullAllowed] NSAttributedString contextBefore, [NullAllowed] NSAttributedString contextAfter, [NullAllowed] NSAttributedString markedText, NSRange selectedRangeInMarkedText);

		[Export ("addTextRect:forCharacterRange:")]
		void AddTextRect (CGRect rect, NSRange range);

		[Export ("autocorrectedRanges", ArgumentSemantic.Copy)]
		NSValue[] AutocorrectedRanges { get; set; }
	}

	[NoTV, Mac (14,3), iOS (17,4), MacCatalyst (17, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BEProcessCapabilityGrant
	{
		[Abstract]
		[Export ("invalidate")]
		bool Invalidate ();

		[Abstract]
		[Export ("valid")]
		bool Valid { [Bind ("isValid")] get; }
	}

	interface IBEProcessCapabilityGrant {}

	[NoTV, NoMac, iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEMediaEnvironment
	{
		[Export ("initWithWebPageURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithXPCRepresentation:error:")]
		NativeHandle Constructor (OS_xpc_object xpcRepresentation, [NullAllowed] out NSError error);

		[Export ("createXPCRepresentation")]
		OS_xpc_object CreateXpcRepresentation ();

		[Export ("activateWithError:")]
		bool Activate ([NullAllowed] out NSError error);

		[Export ("suspendWithError:")]
		bool Suspend ([NullAllowed] out NSError error);

		[Export ("makeCaptureSessionWithError:")]
		[return: NullAllowed]
		AVCaptureSession MakeCaptureSession ([NullAllowed] out NSError error);
	}

	[NoTV, Mac (14,3), iOS (17,4), MacCatalyst (17, 4)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEProcessCapability
	{
		[NoMac]
		[Static]
		[Export ("mediaPlaybackAndCaptureWithEnvironment:")]
		BEProcessCapability CreateMediaPlaybackAndCaptureProcess (BEMediaEnvironment environment);

		[Static]
		[Export ("background")]
		BEProcessCapability CreateBackground ();

		[Static]
		[Export ("foreground")]
		BEProcessCapability CreateForeground ();

		[Static]
		[Export ("suspended")]
		BEProcessCapability CreateSuspended ();

		[Export ("requestWithError:")]
		IBEProcessCapabilityGrant Request ([NullAllowed] out NSError error);
	}

	[TV (17, 5), Mac (14, 5), iOS (17,5), MacCatalyst (17, 5)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface BEWebAppManifest
	{
		[Export ("initWithJSONData:manifestURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData jsonData, NSUrl manifestUrl);

		[Export ("jsonData", ArgumentSemantic.Copy)]
		NSData JsonData { get; }

		[Export ("manifestURL", ArgumentSemantic.Copy)]
		NSUrl ManifestUrl { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum BEAccessibilityPressedState : long
	{
		Undefined = 0,
		False,
		True,
		Mixed,
	}

	[Flags]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Native]
	public enum BEAccessibilityContainerType : ulong
	{
		None = 0x0,
		Landmark = 1uL << 0,
		Table = 1uL << 1,
		List = 1uL << 2,
		Fieldset = 1uL << 3,
		Dialog = 1uL << 4,
		Tree = 1uL << 5,
		Frame = 1uL << 6,
		Article = 1uL << 7,
		SemanticGroup = 1uL << 8,
		ScrollArea = 1uL << 9,
		Alert = 1uL << 10,
		DescriptionList = 1uL << 11,
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[Category]
	[BaseType (typeof (NSObject))]
	interface NSObject_BEAccessibility
	{
		[Export ("browserAccessibilityCurrentStatus")]
		[return: NullAllowed]
		string GetBrowserAccessibilityCurrentStatus ();

		[Export ("setBrowserAccessibilityCurrentStatus:")]
		void SetBrowserAccessibilityCurrentStatus ([NullAllowed] string value);

		[Export ("browserAccessibilitySortDirection")]
		[return: NullAllowed]
		string GetBrowserAccessibilitySortDirection ();

		[Export ("setBrowserAccessibilitySortDirection:")]
		void SetBrowserAccessibilitySortDirection ([NullAllowed] string value);

		[return: NullAllowed]
		[Export ("browserAccessibilityRoleDescription")]
		string GetBrowserAccessibilityRoleDescription ();

		[Export ("setBrowserAccessibilityRoleDescription:")]
		void SetBrowserAccessibilityRoleDescription ([NullAllowed] string value);

		[Export ("browserAccessibilityIsRequired")]
		bool GetBrowserAccessibilityIsRequired ();

		[Export ("setBrowserAccessibilityIsRequired:")]
		void SetBrowserAccessibilityIsRequired (bool value);

		[Export ("browserAccessibilityPressedState")]
		BEAccessibilityPressedState GetBrowserAccessibilityPressedState ();

		[Export ("setBrowserAccessibilityPressedState:")]
		void SetBrowserAccessibilityPressedState (BEAccessibilityPressedState value);

		[Export ("browserAccessibilityHasDOMFocus")]
		bool GetBrowserAccessibilityHasDomFocus ();

		[Export ("setBrowserAccessibilityHasDOMFocus:")]
		void SetBrowserAccessibilityHasDomFocus (bool value);

		[Export ("browserAccessibilityContainerType")]
		BEAccessibilityContainerType GetBrowserAccessibilityContainerType ();

		[Export ("setBrowserAccessibilityContainerType:")]
		void SetBrowserAccessibilityContainerType (BEAccessibilityContainerType value);

		[Export ("browserAccessibilitySelectedTextRange")]
		NSRange GetBrowserAccessibilitySelectedTextRange ();

		[Export ("browserAccessibilitySetSelectedTextRange:")]
		void SetBrowserAccessibilitySelectedTextRange (NSRange range);

		[Export ("browserAccessibilityValueInRange:")]
		string GetBrowserAccessibilityValue (NSRange range);

		[Export ("browserAccessibilityAttributedValueInRange:")]
		NSAttributedString GetBrowserAccessibilityAttributedValue (NSRange range);

		[Export ("browserAccessibilityInsertTextAtCursor:")]
		void BrowserAccessibilityInsertTextAtCursor (string text);

		[Export ("browserAccessibilityDeleteTextAtCursor:")]
		void BrowserAccessibilityDeleteTextAtCursor (nint numberOfCharacters);

		[iOS (18, 2), TV (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("accessibilityLineEndPositionFromCurrentSelection")]
		nint GetAccessibilityLineEndPositionFromCurrentSelection ();

		[iOS (18, 2), TV (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("accessibilityLineStartPositionFromCurrentSelection")]
		nint GetAccessibilityLineStartPositionFromCurrentSelection ();

		[iOS (18, 2), TV (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("accessibilityLineRangeForPosition:")]
		NSRange GetAccessibilityLineRangeForPosition (nint position);
	}

	[BackingFieldType (typeof (ulong))]
	[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), NoMac]
	public enum BEAccessibilityTrait : long {
		[Field ("BEAccessibilityTraitMenuItem")]
		MenuItem,

		[Field ("BEAccessibilityTraitPopUpButton")]
		PopUpButton,

		[Field ("BEAccessibilityTraitRadioButton")]
		RadioButton,

		[Field ("BEAccessibilityTraitReadOnly")]
		ReadOnly,

		[Field ("BEAccessibilityTraitVisited")]
		Visited,
	}

	[BackingFieldType (typeof (uint))]
	[iOS (18, 0), TV (18, 0), MacCatalyst (18, 0), NoMac]
	public enum BEAccessibilityNotification : long {
		[Field ("BEAccessibilitySelectionChangedNotification")]
		SelectionChanged,

		[Field ("BEAccessibilityValueChangedNotification")]
		Changed,
	}

	[iOS (18, 2), TV (18, 2), Mac (15, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	interface BEAccessibilityTextMarker : NSCopying, NSSecureCoding
	{
	}

	[iOS (18, 2), TV (18, 2), Mac (15, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	interface BEAccessibilityTextMarkerRange : NSCopying, NSSecureCoding
	{
		[Export ("startMarker", ArgumentSemantic.Strong)]
		BEAccessibilityTextMarker StartMarker { get; set; }

		[Export ("endMarker", ArgumentSemantic.Strong)]
		BEAccessibilityTextMarker EndMarker { get; set; }
	}

	[iOS (18, 2), TV (18, 2), Mac (15, 2), MacCatalyst (18, 2)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface BEAccessibilityTextMarkerSupport
	{
		[Abstract]
		[Export ("accessibilityBoundsForTextMarkerRange:")]
		CGRect GetAccessibilityBounds (BEAccessibilityTextMarkerRange range);

		[Abstract]
		[Export ("accessibilityContentForTextMarkerRange:")]
		[return: NullAllowed]
		string GetAccessibilityContent (BEAccessibilityTextMarkerRange range);

		[Abstract]
		[return: NullAllowed]
		[Export ("accessibilityTextMarkerRangeForCurrentSelection")]
		BEAccessibilityTextMarkerRange GetAccessibilityTextMarkerRangeForCurrentSelection ();

		[Abstract]
		[Export ("accessibilityTextMarkerRange")]
		BEAccessibilityTextMarkerRange GetAccessibilityTextMarkerRange ();

		[Abstract]
		[Export ("accessibilityNextTextMarker:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityNextTextMarker (BEAccessibilityTextMarker marker);

		[Abstract]
		[Export ("accessibilityPreviousTextMarker:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityPreviousTextMarker (BEAccessibilityTextMarker marker);

		[Abstract]
		[Export ("accessibilityLineEndMarkerForMarker:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityLineEndMarker (BEAccessibilityTextMarker marker);

		[Abstract]
		[Export ("accessibilityLineStartMarkerForMarker:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityLineStartMarker (BEAccessibilityTextMarker marker);

		[Abstract]
		[Export ("accessibilityMarkerForPoint:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityMarker (CGPoint point);

		[Abstract]
		[Export ("accessibilityTextMarkerForPosition:")]
		[return: NullAllowed]
		BEAccessibilityTextMarker GetAccessibilityTextMarker (nint position);

		[Abstract]
		[Export ("accessibilityTextMarkerRangeForRange:")]
		[return: NullAllowed]
		BEAccessibilityTextMarkerRange GetAccessibilityTextMarker (NSRange range);

		[Abstract]
		[Export ("accessibilityRangeForTextMarkerRange:")]
		NSRange GetAccessibilityRange (BEAccessibilityTextMarkerRange range);
	}

	[NoTV, NoMac, iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BEDownloadMonitorLocation
	{
		[Export ("url")]
		NSUrl Url { get; }

		[Export ("bookmarkData")]
		NSData BookmarkData { get; }
	}

	delegate void BEDownloadMonitorUseDownloadsFolderCallback ([NullAllowed] BEDownloadMonitorLocation finalLocation);
	delegate void BEDownloadMonitorBeginMonitoringCallback ([NullAllowed] BEDownloadMonitorLocation placeholderLocation, [NullAllowed] NSError error);
	delegate void BEDownloadMonitorResumeMonitoringCallback ([NullAllowed] NSError error);

	[NoTV, NoMac, iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface BEDownloadMonitor
	{
		[Export ("initWithSourceURL:destinationURL:observedProgress:liveActivityAccessToken:")]
		NativeHandle Constructor (NSUrl sourceUrl, NSUrl destinationUrl, NSProgress observedProgress, NSData liveActivityAccessToken);

		[Export ("useDownloadsFolderWithPlaceholderType:finalFileCreatedHandler:")]
		void UseDownloadsFolder ([NullAllowed] UTType placehodlerType, BEDownloadMonitorUseDownloadsFolderCallback finalFileCreatedHandler);

		[Async]
		[Export ("beginMonitoring:")]
		void BeginMonitoring (BEDownloadMonitorBeginMonitoringCallback completion);

		[Async]
		[Export ("resumeMonitoring:completionHandler:")]
		void ResumeMonitoring (NSUrl url, BEDownloadMonitorResumeMonitoringCallback completionHandler);

		[Export ("identifier")]
		NSUuid Identifier { get; }

		[Export ("sourceURL")]
		NSUrl SourceUrl { get; }

		[Export ("destinationURL")]
		NSUrl DestinationUrl { get; }

		[Static]
		[return: NullAllowed]
		[Export ("createAccessToken")]
		NSData CreateAccessToken ();
	}
}
#endif
