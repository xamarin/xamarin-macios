//
// inputmethodkit.cs: bindings for InputMethodKit
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin, Inc.
//
// 64-bit-audit: abock
//

using System;

using ObjCRuntime;

using AppKit;
using Foundation;
using CoreGraphics;

namespace InputMethodKit {

	[Protocol]
	interface IMKServerProxy {
		[Field ("IMKModeDictionary")]
		NSString ModeDictionary { get; }

		[Field ("IMKControllerClass")]
		NSString ControllerClass { get; }

		[Field ("IMKDelegateClass")]
		NSString DelegateClass { get; }
	}
	
	interface IIMKServerProxy {}

	[BaseType (typeof(NSObject))]
	interface IMKServer : IIMKServerProxy
	{
		[Export ("initWithName:bundleIdentifier:")]
		IntPtr Constructor (string name, string bundleIdentifier);

		[Export ("initWithName:controllerClass:delegateClass:")]
		IntPtr Constructor (string name, Class controllerClassId, Class delegateClassId);

		[Export ("bundle")]
		NSBundle Bundle { get; }

		[Mac (10, 9)]
		[Export ("paletteWillTerminate")]
		bool PaletteWillTerminate { get; }

		[Mac (10, 9)]
		[Export ("lastKeyEventWasDeadKey")]
		bool LastKeyEventWasDeadKey { get; }
	}

	[Protocol (IsInformal = true)]
	interface IMKServerInput
	{
		[Export ("inputText:key:modifiers:client:")]
		bool GetInputText (string data, nint keyCode, nuint flags, NSObject sender);

		[Export ("inputText:client:")]
		bool GetInputText (string data, NSObject sender);

		[Export ("handleEvent:client:")]
		bool OnEvent (NSEvent mouseEvent, NSObject sender);

		[Export ("didCommandBySelector:client:")]
		bool DidCommandBySelector (Selector aSelector, NSObject sender);

		[Export ("composedString:")]
		NSObject GetComposedString (NSObject sender);

		[Export ("originalString:")]
		NSAttributedString GetOriginalString (NSObject sender);

		[Export ("commitComposition:")]
		void CommitComposition (NSObject sender);

		[Export ("candidates:")]
		NSObject[] GetCandidates (NSObject sender);
	}

	[Protocol]
	interface IMKStateSetting
	{
		[Abstract]
		[Export ("activateServer:")]
		void ActivateServer (NSObject sender);

		[Abstract]
		[Export ("deactivateServer:")]
		void DeactivateServer (NSObject sender);

		[Abstract]
		[Export ("valueForTag:client:")]
		NSObject GetValue (nint tag, NSObject sender);

		[Abstract]
		[Export ("setValue:forTag:client:")]
		void SetValue (NSObject value, nint tag, NSObject sender);

		[Abstract]
		[Export ("modes:")]
		NSDictionary GetModes (NSObject sender);

		[Abstract]
		[Export ("recognizedEvents:")]
		nuint GetRecognizedEvents (NSObject sender);

		[Abstract]
		[Export ("showPreferences:")]
		void ShowPreferences (NSObject sender);
	}

	interface IIMKMouseHandling {}

	[Protocol]
	interface IMKMouseHandling
	{
		[Abstract]
		[Export ("mouseDownOnCharacterIndex:coordinate:withModifier:continueTracking:client:")]
		bool OnMouseDownOn (nuint index, CGPoint point, nuint flags, out bool keepTracking, NSObject sender);

		[Abstract]
		[Export ("mouseUpOnCharacterIndex:coordinate:withModifier:client:")]
		bool OnMouseUpOn (nuint index, CGPoint point, nuint flags, NSObject sender);

		[Abstract]
		[Export ("mouseMovedOnCharacterIndex:coordinate:withModifier:client:")]
		bool OnMouseMoved (nuint index, CGPoint point, nuint flags, NSObject sender);
	}

	interface IIMKStateSetting {}
	interface IIMKTextInput {}

	// empty because header is not public
	[Protocol]
	interface IMKTextInput {}

	[BaseType (typeof(NSObject))]
	interface IMKInputController : IIMKStateSetting, IIMKMouseHandling
	{
		[Export ("initWithServer:delegate:client:")]
		IntPtr Constructor (IMKServer server, NSObject @delegate, NSObject inputClient);

		[Export ("updateComposition")]
		void UpdateComposition ();

		[Export ("cancelComposition")]
		void CancelComposition ();

		[Export ("compositionAttributesAtRange:")]
		NSMutableDictionary GetCompositionAttributes (NSRange range);

		[Export ("selectionRange")]
		NSRange SelectionRange { get; }

		[Export ("replacementRange")]
		NSRange ReplacementRange { get; }

		[Export ("markForStyle:atRange:")]
		NSDictionary GetMarkForStyle (nint style, NSRange range);

		[Export ("doCommandBySelector:commandDictionary:")]
		void DoCommand (Selector aSelector, NSDictionary infoDictionary);

		[Export ("hidePalettes")]
		void HidePalettes ();

		[Export ("menu")]
		NSMenu Menu { get; }

		[Export ("delegate")]
		NSObject Delegate { get; set; }

		[Export ("server")]
		IMKServer Server ();

		[Export ("client")]
		IIMKTextInput Client ();

		[Mac (10,9)]
		[Export ("inputControllerWillClose")]
		void OnInputControllerWillClose ();

		[Export ("annotationSelected:forCandidate:")]
		void OnAnnotationSelected (NSAttributedString annotationString, NSAttributedString candidateString);

		[Export ("candidateSelectionChanged:")]
		void OnCandidateSelectionChanged (NSAttributedString candidateString);

		[Export ("candidateSelected:")]
		void OnCandidateSelected (NSAttributedString candidateString);
	}

	public enum IMKCandidatePanelType : uint {
 		SingleColumnScrolling = 1,
 		ScrollingGrid = 2,
 		SingleRowStepping = 3,
 	}

	public enum IMKStyleType : uint {
		Main = 0,
		Annotation = 1,
		SubList = 2,
	}

	public enum IMKLocateCandidates : uint {
		AboveHint = 1,
		BelowHint = 2,
		LeftHint = 3,
		RightHint = 4,
	}

	public enum IMKCommandName {
		[Field ("kIMKCommandMenuItemName")]
		MenuItem,

		[Field ("kIMKCommandClientName")]
		Client,
	}

	[Static]
	[Internal]
	interface IMKCandidatesAttributesKeys {
		[Field ("NSFontAttributeName")]
		NSString FontKey { get; }
		[Field ("IMKCandidatesOpacityAttributeName")]
		NSString OpacityKey { get; }
		[Field ("NSForegroundColorAttributeName")]
		NSString ColorKey { get; }
		[Field ("NSBackgroundColorDocumentAttribute")]
		NSString DocumentBackgroundColorKey { get; }
		[Field ("IMKCandidatesSendServerKeyEventFirst")]
		NSString SendServerKeyEventFirstKey { get; }
	}

	[StrongDictionary ("IMKCandidatesAttributesKeys")]
	interface IMKCandidatesAttributes {
		NSFont Font { get; set;}
		float Opacity { get; set; }
		NSColor Color { get; set; }
		NSColor DocumentBackgroundColor { get; set; }
		bool SendServerKeyEventFirst { get; set; }
	}

	[BaseType (typeof(NSResponder))]
	interface IMKCandidates {
		[Export ("initWithServer:panelType:")]
		IntPtr Constructor (IMKServer server, IMKCandidatePanelType panelType);

		[Export ("initWithServer:panelType:styleType:")]
		IntPtr Constructor (IMKServer server, IMKCandidatePanelType panelType, IMKStyleType style);

		[Export ("panelType")]
		IMKCandidatePanelType PanelType { get; set; }

		[Export ("show:")]
		void Show (IMKLocateCandidates locationHint);

		[Export ("hide")]
		void Hide ();

		[Export ("isVisible")]
		bool IsVisible ();

		[Export ("updateCandidates")]
		void UpdateCandidates ();

		[Export ("showAnnotation:")]
		void ShowAnnotation (NSAttributedString annotationString);

		[Export ("showSublist:subListDelegate:")]
		void ShowSublist (NSObject[] candidates, NSObject @delegate);

		[Export ("candidateFrame")]
		CGRect CandidateFrame { get; }

		// from Events.h
		// enum {
		// kVK_ANSI_A                    = 0x00,
		// kVK_ANSI_S                    = 0x01,
		// kVK_ANSI_D                    = 0x02,
		// kVK_ANSI_F             
		// So the array can be bound as ints 
		[BindAs (typeof (nint []))]
		[Export ("selectionKeys")]
		NSNumber[] SelectionKeys { get;  [Export ("setSelectionKeys:")] set; } 

// this are opacque pointers from CF, need to think about them
//		[Export ("setSelectionKeysKeylayout:")]
//		void SetSelectionKeys(TISInputSource layout);

		// -(TISInputSourceRef)selectionKeysKeylayout;
//		[Export ("selectionKeysKeylayout")]
//		unsafe TISInputSourceRef* SelectionKeysKeylayout ();

		IMKCandidatesAttributes StrongAttributes {
			[Wrap ("new IMKCandidatesAttributes (Attributes)")]
			get;
			[Wrap ("Attributes = value?.Dictionary")]
			set;
		} 

		[Export ("attributes")]
		NSDictionary Attributes { get; [Export ("setAttributes:")] set; }

		[Export ("dismissesAutomatically")]
		bool DismissesAutomatically { get; [Export ("setDismissesAutomatically:")] set; }

		[Mac (10,9)]
		[Export ("selectedCandidate")]
		nint GetSelectedCandidate ();

		[Mac (10,9)]
		[Export ("setCandidateFrameTopLeft:")]
		void SetCandidateFrameTopLeft (CGPoint point);

		[Mac (10,9)]
		[Export ("showChild")]
		void ShowChild ();

		[Mac (10,9)]
		[Export ("hideChild")]
		void HideChild ();

		[Mac (10,9)]
		[Export ("attachChild:toCandidate:type:")]
		void AttachChild (IMKCandidates child, nint candidateIdentifier, IMKStyleType theType);

		[Mac (10,9)]
		[Export ("detachChild:")]
		void DetachChild (nint candidateIdentifier);

		// the candidates data can be strings or attributed strings.
		[Mac (10,9)]
		[Export ("setCandidateData:")]
		void SetCandidateData (NSString[] candidatesData);

		[Mac (10,9)]
		[Export ("selectCandidateWithIdentifier:")]
		bool SelectCandidate (nint candidateIdentifier);

		[Mac (10,9)]
		[Export ("showCandidates")]
		void ShowCandidates ();

		[Mac (10,9)]
		[Export ("candidateStringIdentifier:")]
		nint MapCandidateStringIdentifier (NSObject candidateString);

		[Mac (10,9)]
		[Export ("selectedCandidateString")]
		NSAttributedString GetSelectedCandidateString ();

		[Mac (10,9)]
		[Export ("candidateIdentifierAtLineNumber:")]
		nint GetCandidateIdentifier (nint lineNumber);

		[Mac (10,9)]
		[Export ("lineNumberForCandidateWithIdentifier:")]
		nint GetLineNumberForCandidate (nint candidateIdentifier);

		[Mac (10,9)]
		[Export ("clearSelection")]
		void ClearSelection ();
	}
}
