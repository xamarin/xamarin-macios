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
using Foundation;
using AppKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace InputMethodKit {

	partial interface IMKGlobal {
		[Field ("kIMKCommandMenuItemName")]
		NSString CommandMenuItemName { get; }

		[Field ("kIMKCommandClientName")]
		NSString CommandClientName { get; }
	}

	[Model]
	partial interface IMKServerProxy {
		[Field ("IMKModeDictionary")]
		NSString ModeDictionary { get; }

		[Field ("IMKControllerClass")]
		NSString ControllerClass { get; }

		[Field ("IMKDelegateClass")]
		NSString DelegateClass { get; }
	}

	[BaseType (typeof (NSObject))]
	partial interface IMKServer : IMKServerProxy {

		[Export ("initWithName:bundleIdentifier:")]
		NativeHandle Constructor (string name, string bundleIdentifier);

		[Export ("initWithName:controllerClass:delegateClass:")]
		NativeHandle Constructor (string name, Class controllerClassId, Class delegateClassId);

		[Export ("bundle")]
		NSBundle Bundle { get; }

		[Export ("paletteWillTerminate")]
		bool PaletteWillTerminate { get; }

		[Export ("lastKeyEventWasDeadKey")]
		bool LastKeyEventWasDeadKey { get; }
	}

	[Category, BaseType (typeof (NSObject))]
	partial interface IMKServerInput_NSObject {

		[Export ("inputText:key:modifiers:client:")]
		bool InputText (string str, nint keyCode, nuint flags, NSObject sender);

		[Export ("inputText:client:")]
		bool InputText (string str, NSObject sender);

		[Export ("handleEvent:client:")]
		bool HandleEvent (NSEvent evnt, NSObject sender);

		[Export ("didCommandBySelector:client:")]
		bool DidCommandBySelector (Selector aSelector, NSObject sender);

		[Export ("composedString:")]
		NSObject ComposedString (NSObject sender);

		[Export ("originalString:")]
		NSAttributedString OriginalString (NSObject sender);

		[Export ("commitComposition:")]
		void CommitComposition (NSObject sender);

		[Export ("candidates:")]
		NSObject [] Candidates (NSObject sender);
	}

	[Protocol]
	partial interface IMKStateSetting {
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

	[Model]
	partial interface IMKMouseHandling {

		[Export ("mouseDownOnCharacterIndex:coordinate:withModifier:continueTracking:client:")]
		bool MouseDown (nuint index, NSPoint point, nuint flags, out bool keepTracking, NSObject sender);

		[Export ("mouseUpOnCharacterIndex:coordinate:withModifier:client:")]
		bool MouseUp (nuint index, NSPoint point, nuint flags, NSObject sender);

		[Export ("mouseMovedOnCharacterIndex:coordinate:withModifier:client:")]
		bool MouseMoved (nuint index, NSPoint point, nuint flags, NSObject sender);
	}

	[BaseType (typeof (NSObject))]
	partial interface IMKInputController : IMKStateSetting, IMKMouseHandling {

		[Export ("initWithServer:delegate:client:")]
		NativeHandle Constructor (IMKServer server, NSObject delegateObject, NSObject inputClient);

		[Export ("updateComposition")]
		void UpdateComposition ();

		[Export ("cancelComposition")]
		void CancelComposition ();

		[Export ("compositionAttributesAtRange:")]
		NSMutableDictionary CompositionAttributesAtRange (NSRange range);

		[Export ("selectionRange")]
		NSRange SelectionRange { get; }

		[Export ("replacementRange")]
		NSRange ReplacementRange { get; }

		[Export ("markForStyle:atRange:")]
		NSDictionary Mark (nint style, NSRange range);

		[Export ("doCommandBySelector:commandDictionary:")]
		void DoCommand (Selector aSelector, NSDictionary infoDictionary);

		[Export ("hidePalettes")]
		void HidePalettes ();

		[Export ("menu")]
		NSMenu Menu { get; }

		[Export ("delegate")]
		NSObject Delegate { get; set; }

		[Export ("server")]
		IMKServer Server { get; }

		[Export ("client")]
		NSObject Client { get; }

		[Export ("inputControllerWillClose")]
		void InputControllerWillClose ();

		[Export ("annotationSelected:forCandidate:")]
		void AnnotationSelected (NSAttributedString annotationString, NSAttributedString candidateString);

		[Export ("candidateSelectionChanged:")]
		void CandidateSelectionChanged (NSAttributedString candidateString);

		[Export ("candidateSelected:")]
		void CandidateSelected (NSAttributedString candidateString);
	}

	[BaseType (typeof (NSResponder))]
	partial interface IMKCandidates {

		[Export ("initWithServer:panelType:")]
		NativeHandle Constructor (IMKServer server, IMKCandidatePanelType panelType);

		[Export ("initWithServer:panelType:styleType:")]
		NativeHandle Constructor (IMKServer server, IMKCandidatePanelType panelType, IMKStyleType style);

		[Export ("panelType")]
		IMKCandidatePanelType PanelType { get; set; }

		[Export ("show:")]
		void Show (IMKCandidatesLocationHint locationHint);

		[Export ("hide")]
		void Hide ();

		[Export ("isVisible")]
		bool IsVisible { get; }

		[Export ("updateCandidates")]
		void UpdateCandidates ();

		[Export ("showAnnotation:")]
		void ShowAnnotation (NSAttributedString annotationString);

		[Export ("showSublist:subListDelegate:")]
		void ShowSublist (NSObject [] candidates, NSObject delegateObject);

		[Export ("selectedCandidateString")]
		NSAttributedString SelectedCandidateString { get; }

		[Export ("candidateFrameTopLeft")]
		NSPoint CandidateFrameTopLeft { set; }

		[Export ("candidateFrame")]
		NSRect CandidateFrame { get; }

		[Export ("selectionKeys")]
		NSObject [] SelectionKeys { get; set; }

		/* TISInputSourceRef comes from Carbon
		[Export ("selectionKeysKeylayout")]
		TISInputSourceRef SelectionKeysKeylayout { get; set; }*/

		[Export ("attributes")]
		NSDictionary Attributes { get; set; }

		[Export ("dismissesAutomatically")]
		bool DismissesAutomatically { get; set; }

		[Export ("selectedCandidate")]
		nint SelectedCandidate { get; }

		[Export ("showChild")]
		void ShowChild ();

		[Export ("hideChild")]
		void HideChild ();

		[Export ("attachChild:toCandidate:type:")]
		void AttachChild (IMKCandidates child, nint candidateIdentifier, IMKStyleType theType);

		[Export ("detachChild:")]
		void DetachChild (nint candidateIdentifier);

		[Export ("candidateData")]
		NSObject [] CandidateData { set; }

		[Export ("selectCandidateWithIdentifier:")]
		bool SelectCandidateWithIdentifier (nint candidateIdentifier);

		[Export ("selectCandidate:")]
		void SelectCandidate (nint candidateIdentifier);

		[Export ("showCandidates")]
		void ShowCandidates ();

		[Export ("candidateStringIdentifier:")]
		nint GetCandidateIdentifier (NSObject candidateString);

		[Export ("candidateIdentifierAtLineNumber:")]
		nint GetCandidateIdentifier (nint lineNumber);

		[Export ("lineNumberForCandidateWithIdentifier:")]
		nint GetLineNumberForCandidate (nint candidateIdentifier);

		[Export ("clearSelection")]
		void ClearSelection ();
	}
}
