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

using XamCore.ObjCRuntime;
using XamCore.Foundation;
using XamCore.AppKit;

namespace XamCore.InputMethodKit {

	public partial interface IMKGlobal {
		[Field ("kIMKCommandMenuItemName")]
		NSString CommandMenuItemName { get; }

		[Field ("kIMKCommandClientName")]
		NSString CommandClientName { get; }
	}

	[Model]
	public partial interface IMKServerProxy {
		[Field ("IMKModeDictionary")]
		NSString ModeDictionary { get; }

		[Field ("IMKControllerClass")]
		NSString ControllerClass { get; }

		[Field ("IMKDelegateClass")]
		NSString DelegateClass { get; }
	}

	[BaseType (typeof (NSObject))]
	public partial interface IMKServer : IMKServerProxy {

		[Export ("initWithName:bundleIdentifier:")]
		IntPtr Constructor (string name, string bundleIdentifier);

		[Export ("initWithName:controllerClass:delegateClass:")]
		IntPtr Constructor (string name, Class controllerClassId, Class delegateClassId);

		[Export ("bundle")]
		NSBundle Bundle { get; }

		[Lion, Export ("paletteWillTerminate")]
		bool PaletteWillTerminate { get; }

		[Lion, Export ("lastKeyEventWasDeadKey")]
		bool LastKeyEventWasDeadKey { get; }
	}

	[Category, BaseType (typeof (NSObject))]
	public partial interface IMKServerInput_NSObject {

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
	public partial interface IMKStateSetting {
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
	public partial interface IMKMouseHandling {

		[Export ("mouseDownOnCharacterIndex:coordinate:withModifier:continueTracking:client:")]
		bool MouseDown (nuint index, NSPoint point, nuint flags, out bool keepTracking, NSObject sender);

		[Export ("mouseUpOnCharacterIndex:coordinate:withModifier:client:")]
		bool MouseUp (nuint index, NSPoint point, nuint flags, NSObject sender);

		[Export ("mouseMovedOnCharacterIndex:coordinate:withModifier:client:")]
		bool MouseMoved (nuint index, NSPoint point, nuint flags, NSObject sender);
	}

	[BaseType (typeof (NSObject))]
	public partial interface IMKInputController : IMKStateSetting, IMKMouseHandling {

		[Export ("initWithServer:delegate:client:")]
		IntPtr Constructor (IMKServer server, NSObject delegateObject, NSObject inputClient);

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

		[Lion, Export ("inputControllerWillClose")]
		void InputControllerWillClose ();

		[Export ("annotationSelected:forCandidate:")]
		void AnnotationSelected (NSAttributedString annotationString, NSAttributedString candidateString);

		[Export ("candidateSelectionChanged:")]
		void CandidateSelectionChanged (NSAttributedString candidateString);

		[Export ("candidateSelected:")]
		void CandidateSelected (NSAttributedString candidateString);
	}

	[BaseType (typeof (NSResponder))]
	public partial interface IMKCandidates {

		[Export ("initWithServer:panelType:")]
		IntPtr Constructor (IMKServer server, IMKCandidatePanelType panelType);

		[Export ("initWithServer:panelType:styleType:")]
		IntPtr Constructor (IMKServer server, IMKCandidatePanelType panelType, IMKStyleType style);

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

		[Lion, Export ("selectedCandidateString")]
		NSAttributedString SelectedCandidateString { get; }

		[Lion, Export ("candidateFrameTopLeft")]
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

		[Lion, Export ("selectedCandidate")]
		nint SelectedCandidate { get; }

		[Lion, Export ("showChild")]
		void ShowChild ();

		[Lion, Export ("hideChild")]
		void HideChild ();

		[Lion, Export ("attachChild:toCandidate:type:")]
		void AttachChild (IMKCandidates child, nint candidateIdentifier, IMKStyleType theType);

		[Lion, Export ("detachChild:")]
		void DetachChild (nint candidateIdentifier);

		[Lion, Export ("candidateData")]
		NSObject [] CandidateData { set; }

		[Lion, Export ("selectCandidateWithIdentifier:")]
		bool SelectCandidateWithIdentifier (nint candidateIdentifier);

		[Export ("selectCandidate:")]
		void SelectCandidate (nint candidateIdentifier);

		[Export ("showCandidates")]
		void ShowCandidates ();

		[Lion, Export ("candidateStringIdentifier:")]
		nint GetCandidateIdentifier (NSObject candidateString);

		[Lion, Export ("candidateIdentifierAtLineNumber:")]
		nint GetCandidateIdentifier (nint lineNumber);

		[Lion, Export ("lineNumberForCandidateWithIdentifier:")]
		nint GetLineNumberForCandidate (nint candidateIdentifier);

		[Export ("clearSelection")]
		void ClearSelection ();
	}
}
