using System;
using System.Runtime.Versioning;

using ObjCRuntime;

namespace WebKit {

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum DomCssValueType : ushort {
		Inherit = 0,
		PrimitiveValue = 1,
		ValueList = 2,
		Custom = 3
	}

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	[Flags]
	public enum DomDocumentPosition : ushort {
		Disconnected = 0x01,
		Preceeding = 0x02,
		Following = 0x04,
		Contains = 0x08,
		ContainedBy = 0x10,
		ImplementationSpecific = 0x20
	}

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
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

#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum DomEventPhase : ushort {
		Capturing = 1, AtTarget, Bubbling
	}

	[Flags]
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum WebDragSourceAction : ulong {
		None = 0,
		DHTML = 1,
		Image = 2, 
		Link = 4,
		Selection = 8,
		Any = UInt64.MaxValue
	}

	[Flags]
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum WebDragDestinationAction : ulong {
		None = 0,
		DHTML = 1,
		Image = 2, 
		Link = 4,
#if !NET
		[Obsolete ("This API is not available on this platform.")]
#else
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[Obsolete ("This API is not available on this platform.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		Selection = 8,
		Any = UInt64.MaxValue
	}

#if !NET
	public enum WebNavigationType : uint {
#else
	[Native]
	public enum WebNavigationType : long {
#endif
		LinkClicked, FormSubmitted, BackForward, Reload, FormResubmitted, Other
	}

	// Used as an 'unsigned int' parameter 
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum DomKeyLocation : uint {
		Standard = 0,
		Left = 1,
		Right = 2,
		NumberPad = 3
	}

	// Used as an 'int' parameter 
#if !NET
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#else
	[UnsupportedOSPlatform ("macos10.14")]
#if MONOMAC
	[Obsolete ("Starting with macos10.14 no longer supported.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#endif
	public enum DomDelta : int {
		Pixel = 0,
		Line = 1,
		Page = 2
	}
}
