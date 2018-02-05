using System;

using ObjCRuntime;

namespace WebKit {

	public enum DomCssRuleType : ushort {
		Unknown = 0,
		Style = 1,
		Charset = 2,
		Import = 3,
		Media = 4,
		FontFace = 5,
		Page = 6,
		Variables = 7,
		WebKitKeyFrames = 8,
		WebKitKeyFrame = 9,
		NamespaceRule = 10,
	}

	public enum DomCssValueType : ushort {
		Inherit = 0,
		PrimitiveValue = 1,
		ValueList = 2,
		Custom = 3
	}

	[Flags]
	public enum DomDocumentPosition : ushort {
		Disconnected = 0x01,
		Preceeding = 0x02,
		Following = 0x04,
		Contains = 0x08,
		ContainedBy = 0x10,
		ImplementationSpecific = 0x20
	}

	public enum DomNodeType : ushort {
		Element = 1,
		Attribute = 2,
		Text = 3,
		CData = 4,
		EntityReference = 5,
		Entity = 6,
		ProcessingInstruction = 7,
		Comment = 8,
		Document = 9,
		DocumentType = 10,
		DocumentFragment = 11,
		Notation = 12
	}

	public enum DomRangeCompareHow : ushort {
		StartToStart = 0, 
		StartToEnd = 1, 
		EndToEnd = 2, 
		EndToStart = 3
	}

	[Native]
	public enum WebCacheModel : ulong {
		DocumentViewer, DocumentBrowser, PrimaryWebBrowser
	}

	public enum DomEventPhase : ushort {
		Capturing = 1, AtTarget, Bubbling
	}

	[Flags]
	public enum WebDragSourceAction : ulong {
		None = 0,
		DHTML = 1,
		Image = 2, 
		Link = 4,
		Selection = 8,
#if XAMCORE_2_0
		Any = UInt64.MaxValue
#else
		Any = UInt32.MaxValue
#endif
	}

	[Flags]
	public enum WebDragDestinationAction : ulong {
		None = 0,
		DHTML = 1,
		Image = 2, 
		Link = 4,
		Selection = 8,
#if XAMCORE_2_0
		Any = UInt64.MaxValue
#else
		Any = UInt32.MaxValue
#endif
	}

#if !XAMCORE_4_0
	public enum WebNavigationType : uint {
#else
	[Native]
	public enum WebNavigationType : long {
#endif
		LinkClicked, FormSubmitted, BackForward, Reload, FormResubmitted, Other
	}

	// Used as an 'unsigned int' parameter 
	public enum DomKeyLocation : uint {
		Standard = 0,
		Left = 1,
		Right = 2,
		NumberPad = 3
	}

	// Used as an 'int' parameter 
	public enum DomDelta : int {
		Pixel = 0,
		Line = 1,
		Page = 2
	}
}
