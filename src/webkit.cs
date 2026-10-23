//
// Copyright 2010, Novell, Inc.
// Copyright 2010, Alexander Shulgin
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

using CoreGraphics;
using JavaScriptCore;
using Network;
using Security;

#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
using UIScrollView = AppKit.NSScrollView;
using UIImage = AppKit.NSImage;
using IUIContextMenuInteractionCommitAnimating = Foundation.NSObject;
using UIContextMenuConfiguration = Foundation.NSObject;
using UIEdgeInsets = AppKit.NSEdgeInsets;
using UIFindInteraction = Foundation.NSObject;
using UIViewController = AppKit.NSViewController;
using IUIEditMenuInteractionAnimating = Foundation.NSObject;
using UIConversationContext = Foundation.NSObject;
using UIInputSuggestion = Foundation.NSObject;
#else
#if __MACCATALYST__
using AppKit;
using UIConversationContext = Foundation.NSObject;
using UIInputSuggestion = Foundation.NSObject;
#else
using NSDraggingInfo = Foundation.NSObject;
using INSDraggingInfo = Foundation.NSObject;
#endif
using UIKit;
using NSEventModifierMask = System.Object;
using NSImage = UIKit.UIImage;
using NSMenuItem = Foundation.NSObject;
using NSPasteboard = Foundation.NSObject;
using NSPrintInfo = Foundation.NSObject;
using NSPrintOperation = Foundation.NSObject;
using NSRefreshController = Foundation.NSObject;
using NSResponder = UIKit.UIResponder;
using NSSelectionAffinity = Foundation.NSObject;
using NSUserInterfaceValidations = Foundation.NSObjectProtocol;
using NSView = UIKit.UIView;
using NSWindow = UIKit.UIWindow;
#endif

namespace WebKit {

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (WebScriptObject), Name = "DOMObject")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMObject init]: should never be used
	partial interface DomObject : NSCopying {
	}

	/////////////////////////
	// DomObject subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMAbstractView")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMAbstractView init]: should never be used
	partial interface DomAbstractView {
		[Export ("document", ArgumentSemantic.Retain)]
		DomDocument Document { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSRule")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSRule init]: should never be used
	partial interface DomCssRule {
		[Export ("type")]
		DomCssRuleType Type { get; }

		[Export ("cssText", ArgumentSemantic.Copy)]
		string CssText { get; set; }

		[Export ("parentStyleSheet", ArgumentSemantic.Retain)]
		DomCssStyleSheet ParentStyleSheet { get; }

		[Export ("parentRule", ArgumentSemantic.Retain)]
		DomCssRule ParentRule { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSCharsetRule")]
	[DisableDefaultCtor]
	partial interface DomCssCharsetRule {
		[Export ("encoding")]
		string Encoding { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSFontFaceRule")]
	[DisableDefaultCtor]
	partial interface DomCssFontFaceRule {
		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSImportRule")]
	[DisableDefaultCtor]
	partial interface DomImportCssRule {
		[Export ("href")]
		string Href { get; }

		[Export ("media", ArgumentSemantic.Strong)]
		DomMediaList Media { get; }

		[Export ("styleSheet", ArgumentSemantic.Strong)]
		DomCssStyleSheet StyleSheet { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSMediaRule")]
	[DisableDefaultCtor]
	partial interface DomCssMediaRule {
		[Export ("media", ArgumentSemantic.Strong)]
		DomMediaList Media { get; }

		[Export ("cssRules", ArgumentSemantic.Strong)]
		DomCssRuleList CssRules { get; }

		[Export ("insertRule:index:")]
		uint InsertRule (string rule, uint index);

		[Export ("deleteRule:")]
		void DeleteRule (uint index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSPageRule")]
	[DisableDefaultCtor]
	partial interface DomCssPageRule {
		[Export ("selectorText")]
		string SelectorText { get; }

		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSStyleRule")]
	[DisableDefaultCtor]
	partial interface DomCssStyleRule {
		[Export ("selectorText")]
		string SelectorText { get; }

		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSUnknownRule")]
	[DisableDefaultCtor]
	partial interface DomCssUnknownRule {
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSRuleList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSRuleList init]: should never be used
	partial interface DomCssRuleList {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomCssRule GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSStyleDeclaration")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSStyleDeclaration init]: should never be used
	partial interface DomCssStyleDeclaration {
		[Export ("cssText", ArgumentSemantic.Copy)]
		string CssText { get; set; }

		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("parentRule", ArgumentSemantic.Retain)]
		DomCssRule ParentRule { get; }

		[Export ("getPropertyValue:")]
		string GetPropertyValue (string propertyName);

		[Export ("getPropertyCSSValue:")]
		DomCssValue GetPropertyCssValue (string propertyName);

		[Export ("removeProperty:")]
		string RemoveProperty (string propertyName);

		[Export ("getPropertyPriority:")]
		string GetPropertyPriority (string propertyName);

		[Export ("setProperty:value:priority:")]
		void SetProperty (string propertyName, string value, string priority);

		[Export ("item:")]
		string GetItem (int /* unsigned int */ index);

		[Export ("getPropertyShorthand:")]
		string GetPropertyShorthand (string propertyName);

		[Export ("isPropertyImplicit:")]
		bool IsPropertyImplicit (string propertyName);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomStyleSheet), Name = "DOMCSSStyleSheet")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSStyleSheet init]: should never be used
	partial interface DomCssStyleSheet {
		[Export ("ownerRule", ArgumentSemantic.Retain)]
		DomCssRule OwnerRule { get; }

		[Export ("cssRules", ArgumentSemantic.Retain)]
		DomCssRuleList CssRules { get; }

		[Export ("rules", ArgumentSemantic.Retain)]
		DomCssRuleList Rules { get; }

		[Export ("insertRule:index:")]
		uint /* unsigned int */ InsertRule (string rule, uint /* unsigned int */ index);

		[Export ("deleteRule:")]
		void DeleteRule (uint /* unsigned int */ index);

		[Export ("addRule:style:index:")]
		int /* int, not NSInteger */ AddRule (string selector, string style, uint /* unsigned int */ index);

		[Export ("removeRule:")]
		void RemoveRule (uint /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSValue")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSValue init]: should never be used
	partial interface DomCssValue {
		[Export ("cssText", ArgumentSemantic.Copy)]
		string CssText { get; set; }

		[Export ("cssValueType")]
		DomCssValueType Type { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMHTMLCollection")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMHTMLCollection init]: should never be used
	partial interface DomHtmlCollection {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomNode GetItem (int /* unsigned int */ index);

		[Export ("namedItem:")]
		DomNode GetNamedItem (string name);

		[Export ("tags:")]
		DomNodeList GetTags (string name);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMImplementation")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMImplementation init]: should never be used
	partial interface DomImplementation {
		[Export ("hasFeature:version:")]
		bool HasFeature (string feature, string version);

		[Export ("createDocumentType:publicId:systemId:")]
		DomDocumentType CreateDocumentType (string qualifiedName, string publicId, string systemId);

		[Export ("createDocument:qualifiedName:doctype:")]
		DomDocument CreateDocument (string namespaceUri, string qualifiedName, DomDocumentType doctype);

		[Export ("createCSSStyleSheet:media:")]
		DomCssStyleSheet CreateCssStyleSheet (string title, string media);

		[Export ("createHTMLDocument:")]
		DomHtmlDocument CreateHtmlDocument (string title);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMMediaList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMMediaList init]: should never be used
	partial interface DomMediaList {
		[Export ("mediaText", ArgumentSemantic.Copy)]
		string MediaText { get; set; }

		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		string GetItem (int /* unsigned int */ index);

		[Export ("deleteMedium:")]
		void DeleteMedium (string oldMedium);

		[Export ("appendMedium:")]
		void AppendMedium (string newMedium);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMNamedNodeMap")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMNamedNodeMap init]: should never be used
	partial interface DomNamedNodeMap {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("getNamedItem:")]
		DomNode GetNamedItem (string name);

		[Export ("setNamedItem:")]
		DomNode SetNamedItem (DomNode node);

		[Export ("removeNamedItem:")]
		DomNode RemoveNamedItem (string name);

		[Export ("item:")]
		DomNode GetItem (int /* unsigned int */ index);

		[Export ("getNamedItemNS:localName:")]
		DomNode GetNamedItemNS (string namespaceUri, string localName);

		[Export ("setNamedItemNS:")]
		DomNode SetNamedItemNS (DomNode node);

		[Export ("removeNamedItemNS:localName:")]
		DomNode RemoveNamedItemNS (string namespaceURI, string localName);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMNode")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMNode init]: should never be used
	partial interface DomNode : DomEventTarget {
		[Export ("nodeName", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("nodeValue", ArgumentSemantic.Copy)]
		string NodeValue { get; set; }

		[Export ("nodeType")]
		DomNodeType NodeType { get; }

		[Export ("parentNode", ArgumentSemantic.Retain)]
		DomNode ParentNode { get; }

		[Export ("childNodes", ArgumentSemantic.Retain)]
		DomNodeList ChildNodes { get; }

		[Export ("firstChild", ArgumentSemantic.Retain)]
		DomNode FirstChild { get; }

		[Export ("lastChild", ArgumentSemantic.Retain)]
		DomNode LastChild { get; }

		[Export ("previousSibling", ArgumentSemantic.Retain)]
		DomNode PreviousSibling { get; }

		[Export ("nextSibling", ArgumentSemantic.Retain)]
		DomNode NextSibling { get; }

		[Export ("attributes", ArgumentSemantic.Retain)]
		DomNamedNodeMap Attributes { get; }

		[Export ("ownerDocument", ArgumentSemantic.Retain)]
		DomDocument OwnerDocument { get; }

		[Export ("namespaceURI", ArgumentSemantic.Copy)]
		string NamespaceURI { get; }

		[Export ("prefix", ArgumentSemantic.Copy)]
		string Prefix { get; set; }

		[Export ("localName", ArgumentSemantic.Copy)]
		string LocalName { get; }

		[Export ("baseURI", ArgumentSemantic.Copy)]
		string BaseURI { get; }

		[Export ("textContent", ArgumentSemantic.Copy)]
		string TextContent { get; set; }

		[Export ("parentElement", ArgumentSemantic.Retain)]
		DomElement ParentElement { get; }

		[Export ("isContentEditable")]
		bool IsContentEditable { get; }

		[Export ("insertBefore:refChild:")]
		DomNode InsertBefore (DomNode newChild, [NullAllowed] DomNode refChild);

		[Export ("replaceChild:oldChild:")]
		DomNode ReplaceChild (DomNode newChild, DomNode oldChild);

		[Export ("removeChild:")]
		DomNode RemoveChild (DomNode oldChild);

		[Export ("appendChild:")]
		DomNode AppendChild (DomNode newChild);

		[Export ("hasChildNodes")]
		bool HasChildNodes ();

		[Export ("cloneNode:")]
		DomNode CloneNode (bool deep);

		[Export ("normalize")]
		void Normalize ();

		[Export ("isSupported:version:")]
		bool IsSupported (string feature, string version);

		[Export ("hasAttributes")]
		bool HasAttributes ();

		[Export ("isSameNode:")]
		bool IsSameNode ([NullAllowed] DomNode other);

		[Export ("isEqualNode:")]
		bool IsEqualNode ([NullAllowed] DomNode other);

		[Export ("lookupPrefix:")]
		string LookupPrefix (string namespaceURI);

		[Export ("isDefaultNamespace:")]
		bool IsDefaultNamespace (string namespaceURI);

		[Export ("lookupNamespaceURI:")]
		string LookupNamespace (string prefix);

		[Export ("compareDocumentPosition:")]
		DomDocumentPosition CompareDocumentPosition (DomNode other);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	interface IDomNodeFilter { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[Protocol, Model]
	[BaseType (typeof (NSObject), Name = "DOMNodeFilter")]
	interface DomNodeFilter {
		/// <param name="n">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("acceptNode:")]
		[Abstract]
		short AcceptNode (DomNode n);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMNodeIterator")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomNodeIterator {
		[Export ("root", ArgumentSemantic.Retain)]
		DomNode Root { get; }

		[Export ("whatToShow")]
		uint WhatToShow { get; } /* unsigned int */

		[Export ("filter", ArgumentSemantic.Retain)]
		IDomNodeFilter Filter { get; }

		[Export ("expandEntityReferences")]
		bool ExpandEntityReferences { get; }

		[Export ("referenceNode", ArgumentSemantic.Retain)]
		DomNode ReferenceNode { get; }

		[Export ("pointerBeforeReferenceNode")]
		bool PointerBeforeReferenceNode { get; }

		[Export ("nextNode")]
		DomNode NextNode { get; }

		[Export ("previousNode")]
		DomNode PreviousNode { get; }

		[Export ("detach")]
		void Detach ();
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMNodeList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMNodeList init]: should never be used
	partial interface DomNodeList {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomNode GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMRange")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMRange init]: should never be used
	partial interface DomRange {
		[Export ("startContainer", ArgumentSemantic.Retain)]
		DomNode StartContainer { get; }

		[Export ("startOffset")]
		int StartOffset { get; } /* int, not NSInteger */

		[Export ("endContainer", ArgumentSemantic.Retain)]
		DomNode EndContainer { get; }

		[Export ("endOffset")]
		int EndOffset { get; } /* int, not NSInteger */

		[Export ("collapsed")]
		bool Collapsed { get; }

		[Export ("commonAncestorContainer", ArgumentSemantic.Retain)]
		DomNode CommonAncestorContainer { get; }

		[Export ("text", ArgumentSemantic.Copy)]
		string Text { get; }

		[Export ("setStart:offset:")]
		void SetStart (DomNode refNode, int /* int, not NSInteger */ offset);

		[Export ("setEnd:offset:")]
		void SetEnd (DomNode refNode, int /* int, not NSInteger */ offset);

		[Export ("setStartBefore:")]
		void SetStartBefore (DomNode refNode);

		[Export ("setStartAfter:")]
		void SetStartAfter (DomNode refNode);

		[Export ("setEndBefore:")]
		void SetEndBefore (DomNode refNode);

		[Export ("setEndAfter:")]
		void SetEndAfter (DomNode refNode);

		[Export ("collapse:")]
		void Collapse (bool toStart);

		[Export ("selectNode:")]
		void SelectNode (DomNode refNode);

		[Export ("selectNodeContents:")]
		void SelectNodeContents (DomNode refNode);

		[Export ("compareBoundaryPoints:sourceRange:")]
		short CompareBoundaryPoints (DomRangeCompareHow how, DomRange sourceRange);

		[Export ("deleteContents")]
		void DeleteContents ();

		[Export ("extractContents")]
		DomDocumentFragment ExtractContents ();

		[Export ("cloneContents")]
		DomDocumentFragment CloneContents ();

		[Export ("insertNode:")]
		void InsertNode (DomNode newNode);

		[Export ("surroundContents:")]
		void SurroundContents (DomNode newParent);

		[Export ("cloneRange")]
		DomRange CloneRange ();

		[Export ("toString")]
		string ToString ();

		[Export ("detach")]
		void Detach ();

		[Export ("createContextualFragment:")]
		DomDocumentFragment CreateContextualFragment (string html);

		[Export ("intersectsNode:")]
		bool IntersectsNode (DomNode refNode);

		[Export ("compareNode:")]
		short CompareNode (DomNode refNode);

		[Export ("comparePoint:offset:")]
		short ComparePoint (DomNode refNode, int /* int, not NSInteger */ offset);

		[Export ("isPointInRange:offset:")]
		bool IsPointInRange (DomNode refNode, int /* int, not NSInteger */ offset);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMStyleSheet")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMStyleSheet init]: should never be used
	partial interface DomStyleSheet {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("ownerNode", ArgumentSemantic.Retain)]
		DomNode OwnerNode { get; }

		[Export ("parentStyleSheet", ArgumentSemantic.Retain)]
		DomStyleSheet ParentStyleSheet { get; }

		[Export ("href", ArgumentSemantic.Copy)]
		string Href { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; }

		[Export ("media", ArgumentSemantic.Retain)]
		DomMediaList Media { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMStyleSheetList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMStyleSheetList init]: should never be used
	partial interface DomStyleSheetList {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomStyleSheet GetItem (int /* unsigned int */ index);
	}

	///////////////////////
	// DomNode subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMAttr")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMAttr init]: should never be used
	partial interface DomAttr {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("specified")]
		bool Specified { get; }

		[Export ("value", ArgumentSemantic.Copy)]
		string Value { get; set; }

		[Export ("ownerElement", ArgumentSemantic.Retain)]
		DomElement OwnerElement { get; }

		[Export ("style", ArgumentSemantic.Retain)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMCharacterData")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCharacterData init]: should never be used
	partial interface DomCharacterData {
		[Export ("data", ArgumentSemantic.Copy)]
		string Data { get; set; }

		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("substringData:length:")]
		string SubstringData (uint /* unsigned int */ offset, uint /* unsigned int */ length);

		[Export ("appendData:")]
		void AppendData (string data);

		[Export ("insertData:data:")]
		void InsertData (uint /* unsigned int */ offset, string data);

		[Export ("deleteData:length:")]
		void DeleteData (uint /* unsigned int */ offset, uint /* unsigned int */ length);

		[Export ("replaceData:length:data:")]
		void ReplaceData (uint /* unsigned int */ offset, uint /* unsigned int */ length, string data);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMDocument")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMDocument init]: should never be used
	partial interface DomDocument {
		[Export ("doctype", ArgumentSemantic.Retain)]
		DomDocumentType DocumentType { get; }

		[Export ("implementation", ArgumentSemantic.Retain)]
		DomImplementation Implementation { get; }

		[Export ("documentElement", ArgumentSemantic.Retain)]
		DomElement DocumentElement { get; }

		[Export ("inputEncoding", ArgumentSemantic.Copy)]
		string InputEncoding { get; }

		[Export ("xmlEncoding", ArgumentSemantic.Copy)]
		string XmlEncoding { get; }

		[Export ("xmlVersion", ArgumentSemantic.Copy)]
		string XmlVersion { get; set; }

		[Export ("xmlStandalone")]
		bool XmlStandalone { get; set; }

		[Export ("documentURI", ArgumentSemantic.Copy)]
		string DocumentURI { get; set; }

		[Export ("defaultView", ArgumentSemantic.Retain)]
		DomAbstractView DefaultView { get; }

		[Export ("styleSheets", ArgumentSemantic.Retain)]
		DomStyleSheetList StyleSheets { get; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("referrer", ArgumentSemantic.Copy)]
		string Referrer { get; }

		[Export ("domain", ArgumentSemantic.Copy)]
		string Domain { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		string Url { get; }

		[Export ("cookie", ArgumentSemantic.Copy)]
		string Cookie { get; set; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Body' property instead.")]
		[Wrap ("Body", IsVirtual = true)]
		DomHtmlElement body { get; set; }
#endif

		[Export ("body", ArgumentSemantic.Retain)]
		DomHtmlElement Body { get; set; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Images' property instead.")]
		[Wrap ("Images", IsVirtual = true)]
		DomHtmlCollection images { get; }
#endif

		[Export ("images", ArgumentSemantic.Retain)]
		DomHtmlCollection Images { get; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Applets' property instead.")]
		[Wrap ("Applets", IsVirtual = true)]
		DomHtmlCollection applets { get; }
#endif

		[Export ("applets", ArgumentSemantic.Retain)]
		DomHtmlCollection Applets { get; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Links' property instead.")]
		[Wrap ("Links", IsVirtual = true)]
		DomHtmlCollection links { get; }
#endif

		[Export ("links", ArgumentSemantic.Retain)]
		DomHtmlCollection Links { get; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Forms' property instead.")]
		[Wrap ("Forms", IsVirtual = true)]
		DomHtmlCollection forms { get; }
#endif

		[Export ("forms", ArgumentSemantic.Retain)]
		DomHtmlCollection Forms { get; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Anchors' property instead.")]
		[Wrap ("Anchors", IsVirtual = true)]
		DomHtmlCollection anchors { get; }
#endif

		[Export ("anchors", ArgumentSemantic.Retain)]
		DomHtmlCollection Anchors { get; }

		[Export ("lastModified", ArgumentSemantic.Copy)]
		string LastModified { get; }

		[Export ("charset", ArgumentSemantic.Copy)]
		string Charset { get; set; }

		[Export ("defaultCharset", ArgumentSemantic.Copy)]
		string DefaultCharset { get; }

		[Export ("readyState", ArgumentSemantic.Copy)]
		string ReadyState { get; }

		[Export ("characterSet", ArgumentSemantic.Copy)]
		string CharacterSet { get; }

		[Export ("preferredStylesheetSet", ArgumentSemantic.Copy)]
		string PreferredStylesheetSet { get; }

		[Export ("selectedStylesheetSet", ArgumentSemantic.Copy)]
		string SelectedStylesheetSet { get; set; }

		[Export ("createElement:")]
		DomElement CreateElement (string tagName);

		[Export ("createDocumentFragment")]
		DomDocumentFragment CreateDocumentFragment ();

		[Export ("createTextNode:")]
		DomText CreateTextNode (string data);

		[Export ("createComment:")]
		DomComment CreateComment (string data);

		[Export ("createCDATASection:")]
		DomCDataSection CreateCDataSection (string data);

		[Export ("createProcessingInstruction:data:")]
		DomProcessingInstruction CreateProcessingInstruction (string target, string data);

		[Export ("createAttribute:")]
		DomAttr CreateAttribute (string name);

		[Export ("createEntityReference:")]
		DomEntityReference CreateEntityReference (string name);

		[Export ("getElementsByTagName:")]
		DomNodeList GetElementsByTagName (string tagname);

		[Export ("importNode:deep:")]
		DomNode ImportNode (DomNode importedNode, bool deep);

		[Export ("createElementNS:qualifiedName:")]
		DomElement CreateElementNS (string namespaceURI, string qualifiedName);

		[Export ("createAttributeNS:qualifiedName:")]
		DomAttr CreateAttributeNS (string namespaceURI, string qualifiedName);

		[Export ("getElementsByTagNameNS:localName:")]
		DomNodeList GetElementsByTagNameNS (string namespaceURI, string localName);

		[Export ("getElementById:")]
		DomElement GetElementById (string elementId);

		[Export ("adoptNode:")]
		DomNode AdoptNode (DomNode source);

		[Export ("createEvent:")]
		DomEvent CreateEvent (string eventType);

		[Export ("createRange")]
		DomRange CreateRange ();

		[Export ("createNodeIterator:whatToShow:filter:expandEntityReferences:")]
		DomNodeIterator CreateNodeIterator (DomNode root, uint /* unsigned int */ whatToShow, IDomNodeFilter filter, bool expandEntityReferences);

		//[Export ("createTreeWalker:whatToShow:filter:expandEntityReferences:")]
		//DomTreeWalker CreateTreeWalker (DomNode root, unsigned whatToShow, id <DomNodeFilter> filter, bool expandEntityReferences);

		[Export ("getOverrideStyle:pseudoElement:")]
		DomCssStyleDeclaration GetOverrideStyle (DomElement element, string pseudoElement);

		//[Export ("createExpression:resolver:")]
		//DomXPathExpression CreateExpression (string expression, id <DomXPathNSResolver> resolver);

		//[Export ("createNSResolver:")]
		//id <DomXPathNSResolver> CreateNSResolver (DomNode nodeResolver);

		//[Export ("evaluate:contextNode:resolver:type:inResult:")]
		//DomXPathResult Evaluate (string expression, DomNode contextNode, id <DomXPathNSResolver> resolver, unsigned short type, DomXPathResult inResult);

		[Export ("execCommand:userInterface:value:")]
		bool ExecCommand (string command, bool userInterface, string value);

		[Export ("execCommand:userInterface:")]
		bool ExecCommand (string command, bool userInterface);

		[Export ("execCommand:")]
		bool ExecCommand (string command);

		[Export ("queryCommandEnabled:")]
		bool QueryCommandEnabled (string command);

		[Export ("queryCommandIndeterm:")]
		bool QueryCommandIndeterm (string command);

		[Export ("queryCommandState:")]
		bool QueryCommandState (string command);

		[Export ("queryCommandSupported:")]
		bool QueryCommandSupported (string command);

		[Export ("queryCommandValue:")]
		string QueryCommandValue (string command);

		[Export ("getElementsByName:")]
		DomNodeList GetElementsByName (string elementName);

		[Export ("elementFromPoint:y:")]
		DomElement ElementFromPoint (int /* int, not NSInteger */ x, int /* int, not NSInteger */ y);

		[Export ("createCSSStyleDeclaration")]
		DomCssStyleDeclaration CreateCssStyleDeclaration ();

		[Export ("getComputedStyle:pseudoElement:")]
		DomCssStyleDeclaration GetComputedStyle (DomElement element, string pseudoElement);

		[Export ("getMatchedCSSRules:pseudoElement:")]
		DomCssRuleList GetMatchedCSSRules (DomElement element, string pseudoElement);

		[Export ("getMatchedCSSRules:pseudoElement:authorOnly:")]
		DomCssRuleList GetMatchedCSSRules (DomElement element, string pseudoElement, bool authorOnly);

		[Export ("getElementsByClassName:")]
		DomNodeList GetElementsByClassName (string tagname);

		[Export ("querySelector:")]
		DomElement QuerySelector (string selectors);

		[Export ("querySelectorAll:")]
		DomNodeList QuerySelectorAll (string selectors);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMDocumentFragment")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMDocumentFragment init]: should never be used
	partial interface DomDocumentFragment {
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMDocumentType")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMDocumentType init]: should never be used
	partial interface DomDocumentType {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("entities", ArgumentSemantic.Retain)]
		DomNamedNodeMap Entities { get; }

		[Export ("notations", ArgumentSemantic.Retain)]
		DomNamedNodeMap Notations { get; }

		[Export ("publicId", ArgumentSemantic.Copy)]
		string PublicId { get; }

		[Export ("systemId", ArgumentSemantic.Copy)]
		string SystemId { get; }

		[Export ("internalSubset", ArgumentSemantic.Copy)]
		string InternalSubset { get; }

	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMElement")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMElement init]: should never be used
	partial interface DomElement {
		[Export ("tagName", ArgumentSemantic.Copy)]
		string TagName { get; }

		[Export ("style", ArgumentSemantic.Retain)]
		DomCssStyleDeclaration Style { get; }

		[Export ("offsetLeft")]
		int OffsetLeft { get; } /* int, not NSInteger */

		[Export ("offsetTop")]
		int OffsetTop { get; } /* int, not NSInteger */

		[Export ("offsetWidth")]
		int OffsetWidth { get; } /* int, not NSInteger */

		[Export ("offsetHeight")]
		int OffsetHeight { get; } /* int, not NSInteger */

		[Export ("offsetParent", ArgumentSemantic.Retain)]
		DomElement OffsetParent { get; }

		[Export ("clientLeft")]
		int ClientLeft { get; } /* int, not NSInteger */

		[Export ("clientTop")]
		int ClientTop { get; } /* int, not NSInteger */

		[Export ("clientWidth")]
		int ClientWidth { get; } /* int, not NSInteger */

		[Export ("clientHeight")]
		int ClientHeight { get; } /* int, not NSInteger */

		[Export ("scrollLeft")]
		int ScrollLeft { get; set; } /* int, not NSInteger */

		[Export ("scrollTop")]
		int ScrollTop { get; set; } /* int, not NSInteger */

		[Export ("scrollWidth")]
		int ScrollWidth { get; } /* int, not NSInteger */

		[Export ("scrollHeight")]
		int ScrollHeight { get; } /* int, not NSInteger */

		[Export ("className", ArgumentSemantic.Copy)]
		string ClassName { get; set; }

		[Export ("firstElementChild", ArgumentSemantic.Retain)]
		DomElement FirstElementChild { get; }

		[Export ("lastElementChild", ArgumentSemantic.Retain)]
		DomElement LastElementChild { get; }

		[Export ("previousElementSibling", ArgumentSemantic.Retain)]
		DomElement PreviousElementSibling { get; }

		[Export ("nextElementSibling", ArgumentSemantic.Retain)]
		DomElement NextElementSibling { get; }

		[Export ("childElementCount")]
		uint ChildElementCount { get; } /* unsigned int */

		[Export ("innerText", ArgumentSemantic.Copy)]
		string InnerText { get; }

		[Export ("getAttribute:")]
		string GetAttribute (string name);

		[Export ("setAttribute:value:")]
		void SetAttribute (string name, string value);

		[Export ("removeAttribute:")]
		void RemoveAttribute (string name);

		[Export ("getAttributeNode:")]
		DomAttr GetAttributeNode (string name);

		[Export ("setAttributeNode:")]
		DomAttr SetAttributeNode (DomAttr newAttr);

		[Export ("removeAttributeNode:")]
		DomAttr RemoveAttributeNode (DomAttr oldAttr);

		[Export ("getElementsByTagName:")]
		DomNodeList GetElementsByTagName (string name);

		[Export ("getAttributeNS:localName:")]
		string GetAttributeNS (string namespaceURI, string localName);

		[Export ("setAttributeNS:qualifiedName:value:")]
		void SetAttributeNS (string namespaceURI, string qualifiedName, string value);

		[Export ("removeAttributeNS:localName:")]
		void RemoveAttributeNS (string namespaceURI, string localName);

		[Export ("getElementsByTagNameNS:localName:")]
		DomNodeList GetElementsByTagNameNS (string namespaceURI, string localName);

		[Export ("getAttributeNodeNS:localName:")]
		DomAttr GetAttributeNodeNS (string namespaceURI, string localName);

		[Export ("setAttributeNodeNS:")]
		DomAttr SetAttributeNodeNS (DomAttr newAttr);

		[Export ("hasAttribute:")]
		bool HasAttribute (string name);

		[Export ("hasAttributeNS:localName:")]
		bool HasAttributeNS (string namespaceURI, string localName);

		[Export ("focus")]
		void Focus ();

		[Export ("blur")]
		void Blur ();

		[Export ("scrollIntoView:")]
		void ScrollIntoView (bool alignWithTop);

		[Export ("contains:")]
		bool Contains (DomElement element);

		[Export ("scrollIntoViewIfNeeded:")]
		void ScrollIntoViewIfNeeded (bool centerIfNeeded);

		[Export ("scrollByLines:")]
		void ScrollByLines (int /* int, not NSInteger */ lines);

		[Export ("scrollByPages:")]
		void ScrollByPages (int /* int, not NSInteger */ pages);

		[Export ("getElementsByClassName:")]
		DomNodeList GetElementsByClassName (string name);

		[Export ("querySelector:")]
		DomElement QuerySelector (string selectors);

		[Export ("querySelectorAll:")]
		DomNodeList QuerySelectorAll (string selectors);

		[Export ("webkitRequestFullScreen:")]
		void WebKitRequestFullScreen (ushort flags);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMEntityReference")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMEntityReference init]: should never be used
	partial interface DomEntityReference {
	}

	interface IDomEventTarget { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject), Name = "DOMEventTarget")]
	[Protocol]
	[Model]
	partial interface DomEventTarget : NSCopying {
		/// <param name="type">To be added.</param>
		/// <param name="listener">To be added.</param>
		/// <param name="useCapture">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("addEventListener:listener:useCapture:")]
		[Abstract]
		void AddEventListener (string type, IDomEventListener listener, bool useCapture);

		/// <param name="type">To be added.</param>
		/// <param name="listener">To be added.</param>
		/// <param name="useCapture">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("removeEventListener:listener:useCapture:")]
		[Abstract]
		void RemoveEventListener (string type, IDomEventListener listener, bool useCapture);

		/// <param name="evt">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dispatchEvent:")]
		[Abstract]
		bool DispatchEvent (DomEvent evt);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMEvent")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMEvent init]: should never be used
	partial interface DomEvent {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("target", ArgumentSemantic.Retain)]
		IDomEventTarget Target { get; }

		[Export ("currentTarget", ArgumentSemantic.Retain)]
		IDomEventTarget CurrentTarget { get; }

		[Export ("eventPhase")]
		DomEventPhase EventPhase { get; }

		[Export ("bubbles")]
		bool Bubbles { get; }

		[Export ("cancelable")]
		bool Cancelable { get; }

		[Export ("timeStamp")]
		UInt64 TimeStamp { get; }

		[Export ("srcElement", ArgumentSemantic.Retain)]
		IDomEventTarget SourceElement { get; }

		[Export ("returnValue")]
		bool ReturnValue { get; set; }

		[Export ("cancelBubble")]
		bool CancelBubble { get; set; }

		[Export ("stopPropagation")]
		void StopPropagation ();

		[Export ("preventDefault")]
		void PreventDefault ();

		[Export ("initEvent:canBubbleArg:cancelableArg:")]
		NativeHandle Constructor (string eventTypeArg, bool canBubbleArg, bool cancelableArg);
	}

	// Note: DOMMutationEvent is not bound since it is deprecated
	// by the W3C to be replaced with Mutation Observers

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomEvent), Name = "DOMOverflowEvent")]
	[DisableDefaultCtor]
	partial interface DomOverflowEvent {
		[Export ("initOverflowEvent:horizontalOverflow:verticalOverflow:")]
		NativeHandle Constructor (ushort orient, bool hasHorizontalOverflow, bool hasVerticalOverflow);

		[Export ("orient")]
		ushort Orient { get; }

		[Export ("horizontalOverflow")]
		bool HasHorizontalOverflow { get; }

		[Export ("verticalOverflow")]
		bool HasVerticalOverflow { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomEvent), Name = "DOMProgressEvent")]
	[DisableDefaultCtor]
	partial interface DomProgressEvent {
		[Export ("lengthComputable")]
		bool IsLengthComputable { get; }

		[Export ("loaded")]
		ulong Loaded { get; }

		[Export ("total")]
		ulong Total { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomEvent), Name = "DOMUIEvent")]
	[DisableDefaultCtor]
	partial interface DomUIEvent {
		[Export ("initUIEvent:canBubble:cancelable:view:detail:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, int /* int, not NSInteger */ detail);

		[Export ("view", ArgumentSemantic.Retain)]
		DomAbstractView View { get; }

		[Export ("detail")]
		int Detail { get; } /* int, not NSInteger */

		[Export ("keyCode")]
		int KeyCode { get; } /* int, not NSInteger */

		[Export ("charCode")]
		int CharCode { get; } /* int, not NSInteger */

		[Export ("layerX")]
		int LayerX { get; } /* int, not NSInteger */

		[Export ("layerY")]
		int LayerY { get; } /* int, not NSInteger */

		[Export ("pageX")]
		int PageX { get; } /* int, not NSInteger */

		[Export ("which")]
		int Which { get; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomUIEvent), Name = "DOMKeyboardEvent")]
	[DisableDefaultCtor]
	partial interface DomKeyboardEvent {
		[Export ("initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:altGraphKey:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, string keyIdentifier, DomKeyLocation keyLocation, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, bool altGraphKey);

		[Export ("initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, string keyIdentifier, DomKeyLocation keyLocation, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey);

		[Export ("getModifierState:")]
		bool GetModifierState (string keyIdentifier);

		[Export ("keyIdentifier", ArgumentSemantic.Copy)]
		string KeyIdentifier { get; }

		[Export ("keyLocation")]
		DomKeyLocation KeyLocation { get; }

		[Export ("ctrlKey")]
		bool CtrlKey { get; }

		[Export ("shiftKey")]
		bool ShiftKey { get; }

		[Export ("altKey")]
		bool AltKey { get; }

		[Export ("metaKey")]
		bool MetaKey { get; }

		[Export ("altGraphKey")]
		bool AltGraphKey { get; }

		[Export ("keyCode")]
		int KeyCode { get; } /* int, not NSInteger */

		[Export ("charCode")]
		int CharCode { get; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomUIEvent), Name = "DOMMouseEvent")]
	[DisableDefaultCtor]
	partial interface DomMouseEvent {
		[Export ("initMouseEvent:canBubble:cancelable:view:detail:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:button:relatedTarget:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, int /* int, not NSInteger */ detail, int /* int, not NSInteger */ screenX, int /* int, not NSInteger */ screenY, int /* int, not NSInteger */ clientX, int /* int, not NSInteger */ clientY, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, ushort button, IDomEventTarget relatedTarget);

		[Export ("screenX")]
		int ScreenX { get; } /* int, not NSInteger */

		[Export ("screenY")]
		int ScreenY { get; } /* int, not NSInteger */

		[Export ("clientX")]
		int ClientX { get; } /* int, not NSInteger */

		[Export ("clientY")]
		int ClientY { get; } /* int, not NSInteger */

		[Export ("ctrlKey")]
		bool CtrlKey { get; }

		[Export ("shiftKey")]
		bool ShiftKey { get; }

		[Export ("altKey")]
		bool AltKey { get; }

		[Export ("metaKey")]
		bool MetaKey { get; }

		[Export ("button")]
		ushort Button { get; }

		[Export ("relatedTarget", ArgumentSemantic.Retain)]
		IDomEventTarget RelatedTarget { get; }

		[Export ("offsetX")]
		int OffsetX { get; } /* int, not NSInteger */

		[Export ("offsetY")]
		int OffsetY { get; } /* int, not NSInteger */

		[Export ("x")]
		int X { get; } /* int, not NSInteger */

		[Export ("y")]
		int Y { get; } /* int, not NSInteger */

		[Export ("fromElement", ArgumentSemantic.Retain)]
		DomNode FromElement { get; }

		[Export ("toElement", ArgumentSemantic.Retain)]
		DomNode ToElement { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomMouseEvent), Name = "DOMWheelEvent")]
	[DisableDefaultCtor]
	partial interface DomWheelEvent {
		[Export ("initWheelEvent:wheelDeltaY:view:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:")]
		NativeHandle Constructor (int /* int, not NSInteger */ wheelDeltaX, int /* int, not NSInteger */ wheelDeltaY, DomAbstractView view, int /* int, not NSInteger */ screenX, int /* int, not NSInteger */ screnY, int /* int, not NSInteger */ clientX, int /* int, not NSInteger */ clientY, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey);

		[Export ("wheelDeltaX")]
		int WheelDeltaX { get; } /* int, not NSInteger */

		[Export ("wheelDeltaY")]
		int WheelDeltaY { get; } /* int, not NSInteger */

		[Export ("wheelDelta")]
		DomDelta /* int */ WheelDelta { get; }

		[Export ("isHorizontal")]
		bool IsHorizontal { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject), Name = "DOMEventListener")]
	[Model]
	[Protocol]
	partial interface DomEventListener {
		/// <param name="evt">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("handleEvent:")]
		void HandleEvent (DomEvent evt);
	}

	interface IDomEventListener { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCharacterData), Name = "DOMProcessingInstruction")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMProcessingInstruction init]: should never be used
	partial interface DomProcessingInstruction {
		[Export ("target", ArgumentSemantic.Copy)]
		string Target { get; }

		[Export ("data", ArgumentSemantic.Copy)]
		string Data { get; set; }

		[Export ("sheet", ArgumentSemantic.Retain)]
		DomStyleSheet Sheet { get; }
	}

	////////////////////////////////
	// DomCharacterData subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCharacterData), Name = "DOMText")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMText init]: should never be used
	partial interface DomText {
		[Export ("wholeText", ArgumentSemantic.Copy)]
		string WholeText { get; }

		[Export ("splitText:")]
		DomText SplitText (uint /* unsigned int */ offset);

		[Export ("replaceWholeText:")]
		DomText ReplaceWholeText (string content);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCharacterData), Name = "DOMComment")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMComment init]: should never be used
	partial interface DomComment {
	}

	///////////////////////////
	// DomText subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomText), Name = "DOMCDATASection")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCDATASection init]: should never be used
	partial interface DomCDataSection {
	}

	///////////////////////////
	// DomDocument subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomDocument), Name = "DOMHTMLDocument")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMHTMLDocument init]: should never be used
	partial interface DomHtmlDocument {
		[Export ("embeds", ArgumentSemantic.Retain)]
		DomHtmlCollection Embeds { get; }

		[Export ("plugins", ArgumentSemantic.Retain)]
		DomHtmlCollection Plugins { get; }

		[Export ("scripts", ArgumentSemantic.Retain)]
		DomHtmlCollection Scripts { get; }

		[Export ("width")]
		int Width { get; } /* int, not NSInteger */

		[Export ("height")]
		int Height { get; } /* int, not NSInteger */

		[Export ("dir", ArgumentSemantic.Copy)]
		string Dir { get; set; }

		[Export ("designMode", ArgumentSemantic.Copy)]
		string DesignMode { get; set; }

		[Export ("compatMode", ArgumentSemantic.Copy)]
		string CompatMode { get; }

		[Export ("activeElement", ArgumentSemantic.Retain)]
		DomElement ActiveElement { get; }

		[Export ("bgColor", ArgumentSemantic.Copy)]
		string BackgroundColor { get; set; }

		[Export ("fgColor", ArgumentSemantic.Copy)]
		string ForegroundColor { get; set; }

		[Export ("alinkColor", ArgumentSemantic.Copy)]
		string ALinkColor { get; set; }

		[Export ("linkColor", ArgumentSemantic.Copy)]
		string LinkColor { get; set; }

		[Export ("vlinkColor", ArgumentSemantic.Copy)]
		string VLinkColor { get; set; }

		[Export ("open")]
		void Open ();

		[Export ("close")]
		void Close ();

		[Export ("write:")]
		void Write (string text);

		[Export ("writeln:")]
		void Writeln (string text);

		[Export ("clear")]
		void Clear ();

		[Export ("captureEvents")]
		void CaptureEvents ();

		[Export ("releaseEvents")]
		void ReleaseEvents ();

		[Export ("hasFocus")]
		bool HasFocus ();
	}

	//////////////////////////
	// DomElement subclasses

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLInputElement")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMHTMLElement init]: should never be used
	partial interface DomHtmlInputElement {
		[Export ("accept", ArgumentSemantic.Copy)]
		string Accept { get; set; }

		[Export ("alt", ArgumentSemantic.Copy)]
		string Alt { get; set; }

		[Export ("autofocus")]
		bool Autofocus { get; set; }

		[Export ("defaultChecked")]
		bool DefaultChecked { get; set; }

#if !XAMCORE_5_0
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'DefaultChecked' property instead.")]
		[Wrap ("DefaultChecked", IsVirtual = true)]
		bool defaultChecked { get; set; }
#endif

		[Export ("checked")]
		bool Checked { get; set; }

		[Export ("disabled")]
		bool Disabled { get; set; }

		//		[Export ("form")]
		//		DomHtmlFormElement Form { get; }

		//		[Export ("files")]
		//		DomFileList Files { get; }

		[Export ("indeterminate")]
		bool Indeterminate { get; set; }

		[Export ("maxLength")]
		int MaxLength { get; set; } /* int, not NSInteger */

		[Export ("multiple")]
		bool Multiple { get; set; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("readOnly")]
		bool ReadOnly { get; set; }

		[Export ("size", ArgumentSemantic.Copy)]
		string Size { get; set; }

		[Export ("src", ArgumentSemantic.Copy)]
		string Src { get; set; }

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; set; }

		[Export ("defaultValue", ArgumentSemantic.Copy)]
		string DefaultValue { get; set; }

		[Export ("value", ArgumentSemantic.Copy)]
		string Value { get; set; }

		[Export ("willValidate")]
		bool WillValidate { get; }

		[Export ("selectionStart")]
		int SelectionStart { get; set; } /* int, not NSInteger */

		[Export ("selectionEnd")]
		int SelectionEnd { get; set; } /* int, not NSInteger */

		[Export ("align", ArgumentSemantic.Copy)]
		string Align { get; set; }

		[Export ("useMap", ArgumentSemantic.Copy)]
		string UseMap { get; set; }

		[Export ("accessKey", ArgumentSemantic.Copy)]
		string AccessKey { get; set; }

		[Export ("altDisplayString", ArgumentSemantic.Copy)]
		string AltDisplayString { get; }

		[Export ("absoluteImageURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageURL { get; }

		[Export ("select")]
		void Select ();

		[Export ("setSelectionRange:end:")]
		void SetSelectionRange (int /* int, not NSInteger */ start, int /* int, not NSInteger */ end);

		[Export ("click")]
		void Click ();

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("files", ArgumentSemantic.Retain)]
		DomFileList Files { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTextAreaElement")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMHTMLElement init]: should never be used
	partial interface DomHtmlTextAreaElement {

		[Export ("accessKey", ArgumentSemantic.Copy)]
		string AccessKey { get; set; }

		[Export ("cols")]
		int Columns { get; set; } /* int, not NSInteger */

		[Export ("defaultValue", ArgumentSemantic.Copy)]
		string DefaultValue { get; set; }

		[Export ("disabled")]
		bool Disabled { get; set; }

		//		[Export ("form")]
		//		DomHtmlFormElement Form { get; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("readOnly")]
		bool ReadOnly { get; set; }

		[Export ("rows")]
		int Rows { get; set; } /* int, not NSInteger */

		[Export ("tabIndex")]
		int TabIndex { get; set; } /* int, not NSInteger */

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("value", ArgumentSemantic.Copy)]
		string Value { get; set; }

		[Export ("blur")]
		void Blur ();

		[Export ("focus")]
		void Focus ();

		[Export ("select")]
		void Select ();

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomElement), Name = "DOMHTMLElement")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMHTMLElement init]: should never be used
	partial interface DomHtmlElement {
		[Export ("idName", ArgumentSemantic.Copy)]
		string IdName { get; set; }

		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[Export ("lang", ArgumentSemantic.Copy)]
		string Lang { get; set; }

		[Export ("dir", ArgumentSemantic.Copy)]
		string Dir { get; set; }

		[Export ("tabIndex")]
		int TabIndex { get; set; } /* int, not NSInteger */

		[Export ("innerHTML", ArgumentSemantic.Copy)]
		string InnerHTML { get; set; }

		[Export ("innerText", ArgumentSemantic.Copy)]
		string InnerText { get; set; }

		[Export ("outerHTML", ArgumentSemantic.Copy)]
		string OuterHTML { get; set; }

		[Export ("outerText", ArgumentSemantic.Copy)]
		string OuterText { get; set; }

		[Export ("children", ArgumentSemantic.Retain)]
		DomHtmlCollection Children { get; }

		[Export ("contentEditable", ArgumentSemantic.Copy)]
		string ContentEditable { get; set; }

		[Export ("isContentEditable")]
		bool IsContentEditable { get; }

		[Export ("titleDisplayString", ArgumentSemantic.Copy)]
		string TitleDisplayString { get; }
	}

	//////////////////////////////////////////////////////////////////

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebArchive : NSCoding, NSCopying {
		[Export ("initWithMainResource:subresources:subframeArchives:")]
		NativeHandle Constructor (WebResource mainResource, NSArray subresources, NSArray subframeArchives);

		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		[Export ("mainResource")]
		WebResource MainResource { get; }

		[Export ("subresources")]
		WebResource [] Subresources { get; }

		[Export ("subframeArchives")]
		WebArchive [] SubframeArchives { get; }

		[Export ("data")]
		NSData Data { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebBackForwardList {
		[Export ("addItem:")]
		void AddItem (WebHistoryItem item);

		[Export ("goBack")]
		void GoBack ();

		[Export ("goForward")]
		void GoForward ();

		[Export ("goToItem:")]
		void GoToItem (WebHistoryItem item);

		[Export ("backItem")]
		WebHistoryItem BackItem ();

		[Export ("currentItem")]
		WebHistoryItem CurrentItem ();

		[Export ("forwardItem")]
		WebHistoryItem ForwardItem ();

		[Export ("backListWithLimit:")]
		WebHistoryItem [] BackListWithLimit (int /* int, not NSInteger */ limit);

		[Export ("forwardListWithLimit:")]
		WebHistoryItem [] ForwardListWithLimit (int /* int, not NSInteger */ limit);

		[Export ("backListCount")]
		int BackListCount { get; } /* int, not NSInteger */

		[Export ("forwardListCount")]
		int ForwardListCount { get; } /* int, not NSInteger */

		[Export ("containsItem:")]
		bool ContainsItem (WebHistoryItem item);

		[Export ("itemAtIndex:")]
		WebHistoryItem ItemAtIndex (int /* int, not NSInteger */ index);

		//Detected properties
		[Export ("capacity")]
		int Capacity { get; set; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebDataSource {
		[Export ("initWithRequest:")]
		NativeHandle Constructor (NSUrlRequest request);

		[Export ("data")]
		NSData Data { get; }

		[Export ("representation")]
		IWebDocumentRepresentation Representation { get; }

		[Export ("webFrame")]
		WebFrame WebFrame { get; }

		[Export ("initialRequest")]
		NSUrlRequest InitialRequest { get; }

		[Export ("request")]
		NSMutableUrlRequest Request { get; }

		[Export ("response")]
		NSUrlResponse Response { get; }

		[Export ("textEncodingName")]
		string TextEncodingName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isLoading")]
		bool IsLoading { get; }

		[Export ("pageTitle")]
		string PageTitle { get; }

		[Export ("unreachableURL")]
		NSUrl UnreachableURL { get; }

		[Export ("webArchive")]
		WebArchive WebArchive { get; }

		[Export ("mainResource")]
		WebResource MainResource { get; }

		[Export ("subresources")]
		WebResource [] Subresources { get; }

		[Export ("subresourceForURL:")]
		WebResource SubresourceForUrl (NSUrl url);

		[Export ("addSubresource:")]
		void AddSubresource (WebResource subresource);
	}

	interface IWebDocumentRepresentation { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface WebDocumentRepresentation {
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("setDataSource:")]
		void SetDataSource (WebDataSource dataSource);

		/// <param name="data">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("receivedData:withDataSource:")]
		void ReceivedData (NSData data, WebDataSource dataSource);

		/// <param name="error">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("receivedError:withDataSource:")]
		void ReceivedError (NSError error, WebDataSource dataSource);

		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("finishedLoadingWithDataSource:")]
		void FinishedLoading (WebDataSource dataSource);

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("canProvideDocumentSource")]
		bool CanProvideDocumentSource { get; }

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("documentSource")]
		string DocumentSource { get; }

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("title")]
		string Title { get; }
	}

	//
	// This is a protocol that is adopted, in some internal classes
	// this is a problem, so I am hiding it for now
	//

	//	[BaseType (typeof (NSObject))]
	//	[Model]
	//	partial interface WebDocumentView {
	//		[Abstract]
	//		[Export ("setDataSource:")]
	//		void SetDataSource (WebDataSource dataSource);
	//
	//		[Abstract]
	//		[Export ("dataSourceUpdated:")]
	//		void DataSourceUpdated (WebDataSource dataSource);
	//
	//		[Abstract]
	//		[Export ("setNeedsLayout:")]
	//		void SetNeedsLayout (bool flag);
	//
	//		[Abstract]
	//		[Export ("layout")]
	//		void Layout ();
	//
	//		[Abstract]
	//		[Export ("viewWillMoveToHostWindow:")]
	//		void ViewWillMoveToHostWindow (NSWindow hostWindow);
	//
	//		[Abstract]
	//		[Export ("viewDidMoveToHostWindow")]
	//		void ViewDidMoveToHostWindow ();
	//	}


	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSUrlDownload))]
	partial interface WebDownload {
	}

	interface IWebDownloadDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebDownloadDelegate {
		/// <param name="download">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("downloadWindowForAuthenticationSheet:"), DelegateName ("WebDownloadRequest"), DefaultValue (null)]
		NSWindow OnDownloadWindowForSheet (WebDownload download);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // invalid handle returned
	partial interface WebFrame {
		[Export ("initWithName:webFrameView:webView:")]
		NativeHandle Constructor (string name, WebFrameView view, WebView webView);

		[Export ("name")]
		string Name { get; }

		[Export ("webView")]
		WebView WebView { get; }

		[Export ("frameView")]
		WebFrameView FrameView { get; }

		[Export ("DOMDocument")]
		DomDocument DomDocument { get; }

		[Export ("frameElement")]
		DomHtmlElement FrameElement { get; }

		[Export ("loadRequest:")]
		void LoadRequest (NSUrlRequest request);

		[Export ("loadData:MIMEType:textEncodingName:baseURL:")]
		void LoadData (NSData data, string mimeType, string textDncodingName, NSUrl baseUrl);

		[Export ("loadHTMLString:baseURL:")]
		void LoadHtmlString (NSString htmlString, [NullAllowed] NSUrl baseUrl);

		[Export ("loadAlternateHTMLString:baseURL:forUnreachableURL:")]
		void LoadAlternateHtmlString (string htmlString, NSUrl baseURL, NSUrl forUnreachableURL);

		[Export ("loadArchive:")]
		void LoadArchive (WebArchive archive);

		[Export ("dataSource")]
		WebDataSource DataSource { get; }

		[Export ("provisionalDataSource")]
		WebDataSource ProvisionalDataSource { get; }

		[Export ("stopLoading")]
		void StopLoading ();

		[Export ("reload")]
		void Reload ();

		[Export ("reloadFromOrigin")]
		void ReloadFromOrigin ();

		[Export ("findFrameNamed:")]
		WebFrame FindFrameNamed (string name);

		[Export ("parentFrame")]
		WebFrame ParentFrame { get; }

		[Export ("childFrames")]
		WebFrame [] ChildFrames { get; }

		[Export ("windowObject")]
		WebScriptObject WindowObject { get; }

		[Export ("globalContext")]
		/* JSGlobalContextRef */
		IntPtr GlobalContext { get; }

		[Export ("javaScriptContext", ArgumentSemantic.Strong)]
		JSContext JavaScriptContext { get; }
	}

	interface IWebFrameLoadDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[Model]
	[Protocol (FormalSince = "10.11")]
	[BaseType (typeof (NSObject))]
	partial interface WebFrameLoadDelegate {
		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didStartProvisionalLoadForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void StartedProvisionalLoad (WebView sender, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didReceiveServerRedirectForProvisionalLoadForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void ReceivedServerRedirectForProvisionalLoad (WebView sender, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFailProvisionalLoadWithError:forFrame:"), EventArgs ("WebFrameError", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void FailedProvisionalLoad (WebView sender, NSError error, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didCommitLoadForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
#if XAMCORE_5_0
		void CommittedLoad (WebView sender, WebFrame forFrame);
#else
		void CommitedLoad (WebView sender, WebFrame forFrame);
#endif

		/// <param name="sender">To be added.</param>
		/// <param name="title">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didReceiveTitle:forFrame:"), EventArgs ("WebFrameTitle", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void ReceivedTitle (WebView sender, string title, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="image">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didReceiveIcon:forFrame:"), EventArgs ("WebFrameImage", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void ReceivedIcon (WebView sender, NSImage image, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFinishLoadForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void FinishedLoad (WebView sender, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFailLoadWithError:forFrame:"), EventArgs ("WebFrameError", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void FailedLoadWithError (WebView sender, NSError error, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didChangeLocationWithinPageForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void ChangedLocationWithinPage (WebView sender, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="toUrl">To be added.</param>
		/// <param name="secondsDelay">To be added.</param>
		/// <param name="fireDate">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:willPerformClientRedirectToURL:delay:fireDate:forFrame:"), EventArgs ("WebFrameClientRedirect", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void WillPerformClientRedirect (WebView sender, NSUrl toUrl, double secondsDelay, NSDate fireDate, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didCancelClientRedirectForFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void CanceledClientRedirect (WebView sender, WebFrame forFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:willCloseFrame:"), EventArgs ("WebFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void WillCloseFrame (WebView sender, WebFrame forFrame);

		/// <param name="webView">To be added.</param>
		/// <param name="windowObject">To be added.</param>
		/// <param name="forFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didClearWindowObject:forFrame:"), EventArgs ("WebFrameScriptFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void ClearedWindowObject (WebView webView, WebScriptObject windowObject, WebFrame forFrame);

		/// <param name="webView">To be added.</param>
		/// <param name="windowScriptObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:windowScriptObjectAvailable:"), EventArgs ("WebFrameScriptObject", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void WindowScriptObjectAvailable (WebView webView, WebScriptObject windowScriptObject);

		/// <param name="webView">To be added.</param>
		/// <param name="context">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didCreateJavaScriptContext:forFrame:"), EventArgs ("WebFrameJavaScriptContext", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void DidCreateJavaScriptContext (WebView webView, JSContext context, WebFrame frame);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSView))]
	partial interface WebFrameView {
		/// <param name="frameRect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frameRect);

		[Export ("webFrame")]
		WebFrame WebFrame { get; }

		// This is an NSVIew<WebDocumentView>, so we need to figure what to do about that
		[Export ("documentView")]
		NSView DocumentView { get; }

		[Export ("canPrintHeadersAndFooters")]
		bool CanPrintHeadersAndFooters { get; }

		[Export ("printOperationWithPrintInfo:")]
		NSPrintOperation GetPrintOperation (NSPrintInfo printInfo);

		[Export ("documentViewShouldHandlePrint")]
		bool DocumentViewShouldHandlePrint { get; }

		[Export ("printDocumentView")]
		void PrintDocumentView ();

		//Detected properties
		[Export ("allowsScrolling")]
		bool AllowsScrolling { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	interface WebHistory {
		[Export ("orderedLastVisitedDays")]
		NSCalendarDate [] OrderedLastVisitedDays { get; }

		[Export ("historyItemLimit")]
		int HistoryItemLimit { get; set; } /* int, not NSInteger */

		[Export ("historyAgeInDaysLimit")]
		int HistoryAgeInDaysLimit { get; set; } /* int, not NSInteger */

		[Static, Export ("optionalSharedHistory")]
		WebHistory OptionalSharedHistory { get; }

		[Static, Export ("setOptionalSharedHistory:")]
		void SetOptionalSharedHistory ([NullAllowed] WebHistory history);

		[Export ("loadFromURL:error:")]
		bool Load (NSUrl url, out NSError error);

		[Export ("saveToURL:error:")]
		bool Save (NSUrl url, out NSError error);

		[Export ("addItems:")]
		void AddItems (WebHistoryItem [] newItems);

		[Export ("removeItems:")]
		void RemoveItems (WebHistoryItem [] items);

		[Export ("removeAllItems")]
		void RemoveAllItems ();

		[Export ("orderedItemsLastVisitedOnDay:")]
		WebHistoryItem [] GetOrderedItemsLastVisitedOnDay (NSCalendarDate calendarDate);

		[Export ("itemForURL:")]
		WebHistoryItem GetHistoryItemForUrl (NSUrl url);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebHistoryItem : NSCopying {
		[Export ("initWithURLString:title:lastVisitedTimeInterval:")]
		NativeHandle Constructor (string urlString, string title, double lastVisitedTimeInterval);

		[Export ("originalURLString")]
		string OriginalUrlString { get; }

		[Export ("URLString")]
		string UrlString { get; }

		[Export ("title")]
		string Title { get; }

		[Export ("lastVisitedTimeInterval")]
		double LastVisitedTimeInterval { get; }

		[Export ("icon")]
		NSImage Icon { get; }

		//Detected properties
		[Export ("alternateTitle")]
		string AlternateTitle { get; set; }

		[Field ("WebHistoryItemChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	partial interface WebOpenPanelResultListener {
		[Abstract]
		[Export ("chooseFilename:")]
		void ChooseFilename (string filename);

		[Abstract]
		[Export ("chooseFilenames:")]
		void ChooseFilenames (string [] filenames);

		[Abstract]
		[Export ("cancel")]
		void Cancel ();
	}

	interface IWebOpenPanelResultListener { }

	interface IWebPolicyDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebPolicyDelegate {
		/// <param name="webView">To be added.</param>
		/// <param name="actionInformation">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <param name="decisionToken">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:decidePolicyForNavigationAction:request:frame:decisionListener:"), EventArgs ("WebNavigationPolicy", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[NoDefaultValue]
		void DecidePolicyForNavigation (WebView webView, NSDictionary actionInformation, NSUrlRequest request, WebFrame frame, NSObject decisionToken);

		/// <param name="webView">To be added.</param>
		/// <param name="actionInformation">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="newFrameName">To be added.</param>
		/// <param name="decisionToken">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:decidePolicyForNewWindowAction:request:newFrameName:decisionListener:"), EventArgs ("WebNewWindowPolicy", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[NoDefaultValue]
		void DecidePolicyForNewWindow (WebView webView, NSDictionary actionInformation, NSUrlRequest request, string newFrameName, NSObject decisionToken);

		/// <param name="webView">To be added.</param>
		/// <param name="mimeType">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <param name="decisionToken">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:decidePolicyForMIMEType:request:frame:decisionListener:"), EventArgs ("WebMimeTypePolicy", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[NoDefaultValue]
		void DecidePolicyForMimeType (WebView webView, string mimeType, NSUrlRequest request, WebFrame frame, NSObject decisionToken);

		/// <param name="webView">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:unableToImplementPolicyWithError:frame:"), EventArgs ("WebFailureToImplementPolicy", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UnableToImplementPolicy (WebView webView, NSError error, WebFrame frame);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WebActionNavigationTypeKey")]
		NSString WebActionNavigationTypeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WebActionElementKey")]
		NSString WebActionElementKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WebActionButtonKey")]
		NSString WebActionButtonKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WebActionModifierFlagsKey")]
		NSString WebActionModifierFlagsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WebActionOriginalURLKey")]
		NSString WebActionOriginalUrlKey { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Protocol, Model]
	partial interface WebPolicyDecisionListener {
		[Abstract]
		[Export ("use")]
		void Use ();

		[Abstract]
		[Export ("download")]
		void Download ();

		[Abstract]
		[Export ("ignore")]
		void Ignore ();
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebPreferences : NSCoding {
		[Static]
		[Export ("standardPreferences")]
		WebPreferences StandardPreferences { get; }

		[Export ("initWithIdentifier:")]
		NativeHandle Constructor (string identifier);

		[Export ("identifier")]
		string Identifier { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("arePlugInsEnabled")]
		bool PlugInsEnabled { get; [Bind ("setPlugInsEnabled:")] set; }

		//Detected properties
		[Export ("standardFontFamily")]
		string StandardFontFamily { get; set; }

		[Export ("fixedFontFamily")]
		string FixedFontFamily { get; set; }

		[Export ("serifFontFamily")]
		string SerifFontFamily { get; set; }

		[Export ("sansSerifFontFamily")]
		string SansSerifFontFamily { get; set; }

		[Export ("cursiveFontFamily")]
		string CursiveFontFamily { get; set; }

		[Export ("fantasyFontFamily")]
		string FantasyFontFamily { get; set; }

		[Export ("defaultFontSize")]
		int DefaultFontSize { get; set; } /* int, not NSInteger */

		[Export ("defaultFixedFontSize")]
		int DefaultFixedFontSize { get; set; } /* int, not NSInteger */

		[Export ("minimumFontSize")]
		int MinimumFontSize { get; set; } /* int, not NSInteger */

		[Export ("minimumLogicalFontSize")]
		int MinimumLogicalFontSize { get; set; } /* int, not NSInteger */

		[Export ("defaultTextEncodingName")]
		string DefaultTextEncodingName { get; set; }

		[Export ("userStyleSheetEnabled")]
		bool UserStyleSheetEnabled { get; set; }

		[Export ("userStyleSheetLocation", ArgumentSemantic.Retain)]
		NSUrl UserStyleSheetLocation { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("javaEnabled")]
		bool JavaEnabled { [Bind ("isJavaEnabled")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("javaScriptEnabled")]
		bool JavaScriptEnabled { [Bind ("isJavaScriptEnabled")] get; set; }

		[Export ("javaScriptCanOpenWindowsAutomatically")]
		bool JavaScriptCanOpenWindowsAutomatically { get; set; }

		[Export ("allowsAnimatedImages")]
		bool AllowsAnimatedImages { get; set; }

		[Export ("allowsAnimatedImageLooping")]
		bool AllowsAnimatedImageLooping { get; set; }

		[Export ("loadsImagesAutomatically")]
		bool LoadsImagesAutomatically { get; set; }

		[Export ("autosaves")]
		bool Autosaves { get; set; }

		[Export ("shouldPrintBackgrounds")]
		bool ShouldPrintBackgrounds { get; set; }

		[Export ("privateBrowsingEnabled")]
		bool PrivateBrowsingEnabled { get; set; }

		[Export ("tabsToLinks")]
		bool TabsToLinks { get; set; }

		[Export ("usesPageCache")]
		bool UsesPageCache { get; set; }

		[Export ("cacheModel")]
		WebCacheModel CacheModel { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	partial interface WebResource : NSCoding, NSCopying {
		[Export ("initWithData:URL:MIMEType:textEncodingName:frameName:")]
		NativeHandle Constructor (NSData data, NSUrl url, string mimeType, string textEncodingName, string frameName);

		[Export ("data")]
		NSData Data { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("MIMEType")]
		string MimeType { get; }

		[Export ("textEncodingName")]
		string TextEncodingName { get; }

		[Export ("frameName")]
		string FrameName { get; }
	}

	interface IWebResourceLoadDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebResourceLoadDelegate {
		/// <param name="sender">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:identifierForInitialRequest:fromDataSource:"), DelegateName ("WebResourceIdentifierRequest"), DefaultValue (null)]
		NSObject OnIdentifierForInitialRequest (WebView sender, NSUrlRequest request, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="redirectResponse">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:resource:willSendRequest:redirectResponse:fromDataSource:"), DelegateName ("WebResourceOnRequestSend"), DefaultValueFromArgument ("request")]
		NSUrlRequest OnSendRequest (WebView sender, NSObject identifier, NSUrlRequest request, NSUrlResponse redirectResponse, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didReceiveAuthenticationChallenge:fromDataSource:"), EventArgs ("WebResourceAuthenticationChallenge", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnReceivedAuthenticationChallenge (WebView sender, NSObject identifier, NSUrlAuthenticationChallenge challenge, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didCancelAuthenticationChallenge:fromDataSource:"), EventArgs ("WebResourceCancelledChallenge", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnCancelledAuthenticationChallenge (WebView sender, NSObject identifier, NSUrlAuthenticationChallenge challenge, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="responseReceived">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didReceiveResponse:fromDataSource:"), EventArgs ("WebResourceReceivedResponse", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnReceivedResponse (WebView sender, NSObject identifier, NSUrlResponse responseReceived, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="length">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didReceiveContentLength:fromDataSource:"), EventArgs ("WebResourceReceivedContentLength", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnReceivedContentLength (WebView sender, NSObject identifier, nint length, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didFinishLoadingFromDataSource:"), EventArgs ("WebResourceCompleted", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnFinishedLoading (WebView sender, NSObject identifier, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="identifier">To be added.</param>
		/// <param name="withError">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:resource:didFailLoadingWithError:fromDataSource:"), EventArgs ("WebResourceError", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnFailedLoading (WebView sender, NSObject identifier, NSError withError, WebDataSource dataSource);

		/// <param name="sender">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="dataSource">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:plugInFailedWithError:dataSource:"), EventArgs ("WebResourcePluginError", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void OnPlugInFailed (WebView sender, NSError error, WebDataSource dataSource);
	}

	interface IWebUIDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebUIDelegate {
		/// <param name="sender">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:createWebViewWithRequest:"), DelegateName ("CreateWebViewFromRequest"), DefaultValue (null)]
		WebView UICreateWebView (WebView sender, NSUrlRequest request);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewShow:")]
		void UIShow (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:createWebViewModalDialogWithRequest:"), DelegateName ("WebViewCreate"), DefaultValue (null)]
		WebView UICreateModalDialog (WebView sender, NSUrlRequest request);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewRunModal:")]
		void UIRunModal (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewClose:")]
		void UIClose (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewFocus:")]
		void UIFocus (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewUnfocus:")]
		void UIUnfocus (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewFirstResponder:"), DelegateName ("WebViewGetResponder"), DefaultValue (null)]
		NSResponder UIGetFirstResponder (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="newResponder">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:makeFirstResponder:"), EventArgs ("WebViewResponder", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIMakeFirstResponder (WebView sender, NSResponder newResponder);

		/// <param name="sender">To be added.</param>
		/// <param name="text">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setStatusText:"), EventArgs ("WebViewStatusText", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetStatusText (WebView sender, string text);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewStatusText:"), DelegateName ("WebViewGetString"), DefaultValue (null)]
		string UIGetStatusText (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewAreToolbarsVisible:"), DelegateName ("WebViewGetBool"), DefaultValue (null)]
		bool UIAreToolbarsVisible (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="visible">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setToolbarsVisible:"), EventArgs ("WebViewToolBars", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetToolbarsVisible (WebView sender, bool visible);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewIsStatusBarVisible:"), DelegateName ("WebViewGetBool"), DefaultValue (false)]
		bool UIIsStatusBarVisible (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="visible">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setStatusBarVisible:"), EventArgs ("WebViewStatusBar", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetStatusBarVisible (WebView sender, bool visible);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewIsResizable:"), DelegateName ("WebViewGetBool"), DefaultValue (null)]
		bool UIIsResizable (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="resizable">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setResizable:"), EventArgs ("WebViewResizable", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetResizable (WebView sender, bool resizable);

		/// <param name="sender">To be added.</param>
		/// <param name="newFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setFrame:"), EventArgs ("WebViewFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetFrame (WebView sender, CGRect newFrame);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewFrame:"), DelegateName ("WebViewGetRectangle"), DefaultValue (null)]
		CGRect UIGetFrame (WebView sender);

		/// <param name="sender">To be added.</param>
		/// <param name="withMessage">To be added.</param>
		/// <param name="initiatedByFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:"), EventArgs ("WebViewJavaScriptFrame", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIRunJavaScriptAlertPanelMessage (WebView sender, string withMessage, WebFrame initiatedByFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="withMessage">To be added.</param>
		/// <param name="initiatedByFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:"), DelegateName ("WebViewConfirmationPanel"), DefaultValue (null)]
		bool UIRunJavaScriptConfirmationPanel (WebView sender, string withMessage, WebFrame initiatedByFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="prompt">To be added.</param>
		/// <param name="defaultText">To be added.</param>
		/// <param name="initiatedByFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:"), DelegateName ("WebViewPromptPanel"), DefaultValue (null)]
		string UIRunJavaScriptTextInputPanelWithFrame (WebView sender, string prompt, string defaultText, WebFrame initiatedByFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <param name="initiatedByFrame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:runBeforeUnloadConfirmPanelWithMessage:initiatedByFrame:"), DelegateName ("WebViewJavaScriptFrame"), DefaultValue (null)]
		bool UIRunBeforeUnload (WebView sender, string message, WebFrame initiatedByFrame);

		/// <param name="sender">To be added.</param>
		/// <param name="resultListener">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:runOpenPanelForFileButtonWithResultListener:"), EventArgs ("WebViewRunOpenPanel", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIRunOpenPanelForFileButton (WebView sender, IWebOpenPanelResultListener resultListener);

		/// <param name="sender">To be added.</param>
		/// <param name="elementInformation">To be added.</param>
		/// <param name="modifierFlags">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:mouseDidMoveOverElement:modifierFlags:"), EventArgs ("WebViewMouseMoved", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIMouseDidMoveOverElement (WebView sender, NSDictionary elementInformation, NSEventModifierMask modifierFlags);

		/// <param name="sender">To be added.</param>
		/// <param name="forElement">To be added.</param>
		/// <param name="defaultMenuItems">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:contextMenuItemsForElement:defaultMenuItems:"), DelegateName ("WebViewGetContextMenuItems"), DefaultValue (null)]
		NSMenuItem [] UIGetContextMenuItems (WebView sender, NSDictionary forElement, NSMenuItem [] defaultMenuItems);

		/// <param name="webView">To be added.</param>
		/// <param name="validatedUserInterfaceItem">To be added.</param>
		/// <param name="defaultValidation">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:validateUserInterfaceItem:defaultValidation:"), DelegateName ("WebViewValidateUserInterface"), DefaultValueFromArgument ("defaultValidation")]
		bool UIValidateUserInterfaceItem (WebView webView, NSObject validatedUserInterfaceItem, bool defaultValidation);

		[Export ("webView:shouldPerformAction:fromSender:"), DelegateName ("WebViewPerformAction"), DefaultValue (null)]
		bool UIShouldPerformAction (WebView webView, Selector action, NSObject sender);

		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:dragDestinationActionMaskForDraggingInfo:"), DelegateName ("DragDestinationGetActionMask"), DefaultValue (0)]
		WebDragDestinationAction UIGetDragDestinationActionMask (WebView webView, INSDraggingInfo draggingInfo);

		[Export ("webView:willPerformDragDestinationAction:forDraggingInfo:"), EventArgs ("WebViewDrag", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIWillPerformDragDestination (WebView webView, WebDragDestinationAction action, INSDraggingInfo draggingInfo);

		/// <param name="webView">To be added.</param>
		/// <param name="point">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:dragSourceActionMaskForPoint:"), DelegateName ("DragSourceGetActionMask"), DefaultValue (0)]
		WebDragSourceAction UIDragSourceActionMask (WebView webView, CGPoint point);

		/// <param name="webView">To be added.</param>
		/// <param name="action">To be added.</param>
		/// <param name="sourcePoint">To be added.</param>
		/// <param name="pasteboard">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:willPerformDragSourceAction:fromPoint:withPasteboard:"), EventArgs ("WebViewPerformDrag", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIWillPerformDragSource (WebView webView, WebDragSourceAction action, CGPoint sourcePoint, NSPasteboard pasteboard);

		/// <param name="sender">To be added.</param>
		/// <param name="frameView">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:printFrameView:"), EventArgs ("WebViewPrint", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIPrintFrameView (WebView sender, WebFrameView frameView);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewHeaderHeight:"), DelegateName ("WebViewGetFloat"), DefaultValue (null)]
		float UIGetHeaderHeight (WebView sender); /* float, not CGFloat */

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewFooterHeight:"), DelegateName ("WebViewGetFloat"), DefaultValue (null)]
		float UIGetFooterHeight (WebView sender); /* float, not CGFloat */

		/// <param name="sender">To be added.</param>
		/// <param name="rect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:drawHeaderInRect:"), EventArgs ("WebViewHeader", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIDrawHeaderInRect (WebView sender, CGRect rect);

		/// <param name="sender">To be added.</param>
		/// <param name="rect">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:drawFooterInRect:"), EventArgs ("WebViewFooter", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIDrawFooterInRect (WebView sender, CGRect rect);

		/// <param name="sender">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:runJavaScriptAlertPanelWithMessage:"), EventArgs ("WebViewJavaScript", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UIRunJavaScriptAlertPanel (WebView sender, string message);

		/// <param name="sender">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:runJavaScriptConfirmPanelWithMessage:"), DelegateName ("WebViewPrompt"), DefaultValue (null)]
		bool UIRunJavaScriptConfirmPanel (WebView sender, string message);

		/// <param name="sender">To be added.</param>
		/// <param name="prompt">To be added.</param>
		/// <param name="defaultText">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:"), DelegateName ("WebViewJavaScriptInput"), DefaultValue (null)]
		string UIRunJavaScriptTextInputPanel (WebView sender, string prompt, string defaultText);

		/// <param name="sender">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:setContentRect:"), EventArgs ("WebViewContent", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void UISetContentRect (WebView sender, CGRect frame);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("webViewContentRect:"), DelegateName ("WebViewGetRectangle"), DefaultValue (null)]
		CGRect UIGetContentRect (WebView sender);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // crash on dispose, documented as "You can not create a WebScriptObject object directly."
	partial interface WebScriptObject {
		[Static, Export ("throwException:")]
		bool ThrowException (string exceptionMessage);

		[Export ("JSObject")]
		/* JSObjectRef */
		IntPtr JSObject { get; }

		[Export ("callWebScriptMethod:withArguments:")]
		NSObject CallWebScriptMethod (string name, NSObject [] arguments);

		[Export ("evaluateWebScript:")]
		NSObject EvaluateWebScript (string script);

		[Export ("removeWebScriptKey:")]
		void RemoveWebScriptKey (string name);

		[Export ("stringRepresentation")]
		string StringRepresentation { get; }

		[Export ("webScriptValueAtIndex:")]
		NSObject WebScriptValueAtIndex (int /* unsigned int */ index);

		[Export ("setWebScriptValueAtIndex:value:")]
		void SetWebScriptValueAtIndex (int /* unsigned int */ index, NSObject value);

		[Export ("setException:")]
		void SetException (string description);

		[Export ("JSValue")]
		JSValue JSValue { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSView),
		   Events = new Type [] {
			   typeof (WebFrameLoadDelegate),
			   typeof (WebDownloadDelegate),
			   typeof (WebResourceLoadDelegate),
			   typeof (WebUIDelegate),
			   typeof (WebPolicyDelegate),
		   },
		   Delegates = new string [] {
			   "WeakFrameLoadDelegate",
			   "WeakDownloadDelegate",
			   "WeakResourceLoadDelegate",
			   "WeakUIDelegate",
			   "WeakPolicyDelegate",
		   })]
	partial interface WebView : NSUserInterfaceValidations {
		[Static]
		[Export ("canShowMIMEType:")]
		bool CanShowMimeType (string MimeType);

		[Static]
		[Export ("canShowMIMETypeAsHTML:")]
		bool CanShowMimeTypeAsHtml (string mimeType);

		[Static]
		[Export ("MIMETypesShownAsHTML")]
		string [] MimeTypesShownAsHtml { get; set; }

		[Static]
		[Export ("URLFromPasteboard:")]
		NSUrl UrlFromPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("URLTitleFromPasteboard:")]
		string UrlTitleFromPasteboard (NSPasteboard pasteboard);

		[Static]
		[Export ("registerURLSchemeAsLocal:")]
		void RegisterUrlSchemeAsLocal (string scheme);

		[Export ("initWithFrame:frameName:groupName:")]
		NativeHandle Constructor (CGRect frame, [NullAllowed] string frameName, [NullAllowed] string groupName);

		/// <param name="frame">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("close")]
		void Close ();

		[Export ("mainFrame")]
		WebFrame MainFrame { get; }

		[Export ("selectedFrame")]
		WebFrame SelectedFrame { get; }

		[Export ("backForwardList")]
		WebBackForwardList BackForwardList { get; }

		[Export ("setMaintainsBackForwardList:")]
		void SetMaintainsBackForwardList (bool flag);

		[Export ("goBack")]
		bool GoBack ();

		[Export ("goForward")]
		bool GoForward ();

		[Export ("goToBackForwardItem:")]
		bool GoToBackForwardItem (WebHistoryItem item);

		[Export ("userAgentForURL:")]
		string UserAgentForUrl (NSUrl url);

		[Export ("supportsTextEncoding")]
		bool SupportsTextEncoding { get; }

		[Export ("stringByEvaluatingJavaScriptFromString:")]
		string StringByEvaluatingJavaScriptFromString (string script);

		[Export ("windowScriptObject")]
		WebScriptObject WindowScriptObject { get; }

		[Export ("searchFor:direction:caseSensitive:wrap:")]
		bool Search (string forString, bool forward, bool caseSensitive, bool wrap);

		[Static]
		[Export ("registerViewClass:representationClass:forMIMEType:")]
		void RegisterViewClass (Class viewClass, Class representationClass, string mimeType);

		[Export ("estimatedProgress")]
		double EstimatedProgress { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isLoading")]
		bool IsLoading { get; }

		[Export ("elementAtPoint:")]
		NSDictionary ElementAtPoint (CGPoint point);

		[Export ("pasteboardTypesForSelection")]
		NSPasteboard [] PasteboardTypesForSelection { get; }

		[Export ("writeSelectionWithPasteboardTypes:toPasteboard:")]
		void WriteSelection (NSObject [] types, NSPasteboard pasteboard);

		[Export ("pasteboardTypesForElement:")]
		NSObject [] PasteboardTypesForElement (NSDictionary element);

		[Export ("writeElement:withPasteboardTypes:toPasteboard:")]
		void WriteElement (NSDictionary element, NSObject [] pasteboardTypes, NSPasteboard toPasteboard);

		[Export ("moveDragCaretToPoint:")]
		void MoveDragCaretToPoint (CGPoint point);

		[Export ("removeDragCaret")]
		void RemoveDragCaret ();

		[Export ("mainFrameDocument")]
		DomDocument MainFrameDocument { get; }

		[Export ("mainFrameTitle")]
		string MainFrameTitle { get; }

		[Export ("mainFrameIcon")]
		NSImage MainFrameIcon { get; }

		//Detected properties
		[Export ("shouldCloseWithWindow")]
		bool ShouldCloseWithWindow { get; set; }

		[Export ("resourceLoadDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakResourceLoadDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakResourceLoadDelegate")]
		IWebResourceLoadDelegate ResourceLoadDelegate { get; set; }

		[Export ("downloadDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDownloadDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakDownloadDelegate")]
		IWebDownloadDelegate DownloadDelegate { get; set; }

		[Export ("frameLoadDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakFrameLoadDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakFrameLoadDelegate")]
		IWebFrameLoadDelegate FrameLoadDelegate { get; set; }

		[Export ("UIDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakUIDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakUIDelegate")]
		IWebUIDelegate UIDelegate { get; set; }

		[Export ("policyDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakPolicyDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakPolicyDelegate")]
		IWebPolicyDelegate PolicyDelegate { get; set; }

		[Export ("textSizeMultiplier")]
		float TextSizeMultiplier { get; set; } /* float, not CGFloat */

		[Export ("applicationNameForUserAgent")]
		string ApplicationNameForUserAgent { get; set; }

		[Export ("customUserAgent")]
		string CustomUserAgent { get; set; }

		[Export ("customTextEncodingName")]
		string CustomTextEncodingName { get; set; }

		[Export ("mediaStyle")]
		string MediaStyle { get; set; }

		[Export ("preferences", ArgumentSemantic.Retain)]
		WebPreferences Preferences { get; set; }

		[Export ("preferencesIdentifier")]
		string PreferencesIdentifier { get; set; }

		[Export ("hostWindow", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSWindow HostWindow { get; set; }

		[Export ("groupName")]
		string GroupName { get; set; }

		[Export ("drawsBackground")]
		bool DrawsBackground { get; set; }

		[Export ("shouldUpdateWhileOffscreen")]
		bool UpdateWhileOffscreen { get; set; }

		[Export ("mainFrameURL")]
		string MainFrameUrl { get; set; }

		// NSUserInterfaceValidations
		[Export ("reload:")]
		void Reload (NSObject sender);

		[Export ("reloadFromOrigin:")]
		void ReloadFromOrigin (NSObject sender);

		[Export ("canGoBack")]
		bool CanGoBack ();

		//[Export ("goBack:")]
		//void GoBack (NSObject sender);

		[Export ("canGoForward")]
		bool CanGoForward ();

		//[Export ("goForward:")]
		//void GoForward (NSObject sender);

		[Export ("canMakeTextLarger")]
		bool CanMakeTextLarger ();

		[Export ("makeTextLarger:")]
		void MakeTextLarger (NSObject sender);

		[Export ("canMakeTextSmaller")]
		bool CanMakeTextSmaller ();

		[Export ("makeTextSmaller:")]
		void MakeTextSmaller (NSObject sender);

		[Export ("canMakeTextStandardSize")]
		bool CanMakeTextStandardSize ();

		[Export ("makeTextStandardSize:")]
		void MakeTextStandardSize (NSObject sender);

		[Export ("toggleContinuousSpellChecking:")]
		void ToggleContinuousSpellChecking (NSObject sender);

		[Export ("toggleSmartInsertDelete:")]
		void ToggleSmartInsertDelete (NSObject sender);

		[Export ("setSelectedDOMRange:affinity:")]
		void SetSelectedDomRange ([NullAllowed] DomRange range, NSSelectionAffinity selectionAffinity);

		[Export ("selectedDOMRange")]
		DomRange SelectedDomRange { get; }

		[Export ("selectionAffinity")]
		NSSelectionAffinity SelectionAffinity { get; }

		[Export ("maintainsInactiveSelection")]
		bool MaintainsInactiveSelection { get; }

		[Export ("spellCheckerDocumentTag")]
		nint SpellCheckerDocumentTag { get; }

		[Export ("undoManager")]
		NSUndoManager UndoManager { get; }

		[Export ("styleDeclarationWithText:")]
		DomCssStyleDeclaration StyleDeclarationWithText (string text);

		//Detected properties
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Export ("typingStyle")]
		DomCssStyleDeclaration TypingStyle { get; set; }

		[Export ("smartInsertDeleteEnabled")]
		bool SmartInsertDeleteEnabled { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("continuousSpellCheckingEnabled")]
		bool ContinuousSpellCheckingEnabled { [Bind ("isContinuousSpellCheckingEnabled")] get; set; }

		[Export ("editingDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject EditingDelegate { get; set; }

		[Export ("replaceSelectionWithMarkupString:")]
		void ReplaceSelectionWithMarkupString (string markupString);

		[Export ("replaceSelectionWithArchive:")]
		void ReplaceSelectionWithArchive (WebArchive archive);

		[Export ("deleteSelection")]
		void DeleteSelection ();

		[Export ("applyStyle:")]
		void ApplyStyle (DomCssStyleDeclaration style);

		[Export ("cut:")]
		void Cut (NSObject sender);

		[Export ("paste:")]
		void Paste (NSObject sender);

		[Export ("copyFont:")]
		void CopyFont (NSObject sender);

		[Export ("pasteFont:")]
		void PasteFont (NSObject sender);

		[Export ("delete:")]
		void Delete (NSObject sender);

		[Export ("pasteAsPlainText:")]
		void PasteAsPlainText (NSObject sender);

		[Export ("pasteAsRichText:")]
		void PasteAsRichText (NSObject sender);

		[Export ("changeFont:")]
		void ChangeFont (NSObject sender);

		[Export ("changeAttributes:")]
		void ChangeAttributes (NSObject sender);

		[Export ("changeDocumentBackgroundColor:")]
		void ChangeDocumentBackgroundColor (NSObject sender);

		[Export ("changeColor:")]
		void ChangeColor (NSObject sender);

		[Export ("alignCenter:")]
		void AlignCenter (NSObject sender);

		[Export ("alignJustified:")]
		void AlignJustified (NSObject sender);

		[Export ("alignLeft:")]
		void AlignLeft (NSObject sender);

		[Export ("alignRight:")]
		void AlignRight (NSObject sender);

		[Export ("checkSpelling:")]
		void CheckSpelling (NSObject sender);

		[Export ("showGuessPanel:")]
		void ShowGuessPanel (NSObject sender);

		[Export ("performFindPanelAction:")]
		void PerformFindPanelAction (NSObject sender);

		[Export ("startSpeaking:")]
		void StartSpeaking (NSObject sender);

		[Export ("stopSpeaking:")]
		void StopSpeaking (NSObject sender);

		[Export ("moveToBeginningOfSentence:")]
		void MoveToBeginningOfSentence (NSObject sender);

		[Export ("moveToBeginningOfSentenceAndModifySelection:")]
		void MoveToBeginningOfSentenceAndModifySelection (NSObject sender);

		[Export ("moveToEndOfSentence:")]
		void MoveToEndOfSentence (NSObject sender);

		[Export ("moveToEndOfSentenceAndModifySelection:")]
		void MoveToEndOfSentenceAndModifySelection (NSObject sender);

		[Export ("selectSentence:")]
		void SelectSentence (NSObject sender);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMBlob")]
	[DisableDefaultCtor]
	partial interface DomBlob {
		[Export ("size")]
		ulong Size { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomBlob), Name = "DOMFile")]
	[DisableDefaultCtor]
	partial interface DomFile {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMFileList")]
	[DisableDefaultCtor]
	partial interface DomFileList {
		[Export ("length")]
		uint Length { get; } /* unsigned int */

		[Export ("item:")]
		DomFile GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFormElement")]
	[DisableDefaultCtor]
	partial interface DomHtmlFormElement {
		[Export ("acceptCharset", ArgumentSemantic.Copy)]
		string AcceptCharset { get; set; }

		[Export ("action", ArgumentSemantic.Copy)]
		string Action { get; set; }

		[Export ("enctype", ArgumentSemantic.Copy)]
		string EncodingType { get; set; }

		[Export ("encoding", ArgumentSemantic.Copy)]
		string Encoding { get; set; }

		[Export ("method", ArgumentSemantic.Copy)]
		string Method { get; set; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("target", ArgumentSemantic.Copy)]
		string Target { get; set; }

		[Export ("elements", ArgumentSemantic.Retain)]
		DomHtmlCollection Elements { get; }

		[Export ("length")]
		int Length { get; } /* unsigned int */

		[Export ("submit")]
		void Submit ();

		[Export ("reset")]
		void Reset ();
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLAnchorElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlAnchorElement {

		[Export ("charset")]
		string Charset { get; set; }

		[Export ("coords")]
		string Coords { get; set; }

		[Export ("href")]
		string HRef { get; set; }

		[Export ("hreflang")]
		string HRefLang { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("rel")]
		string Rel { get; set; }

		[Export ("rev")]
		string Rev { get; set; }

		[Export ("shape")]
		string Shape { get; set; }

		[Export ("target")]
		string Target { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("accessKey")]
		string AccessKey { get; set; }

		[Export ("hashName")]
		string HashName { get; }

		[Export ("host")]
		string Host { get; }

		[Export ("hostname")]
		string Hostname { get; }

		[Export ("pathname")]
		string Pathname { get; }

		[Export ("port")]
		string Port { get; }

		[Export ("protocol")]
		string Protocol { get; }

		[Export ("search")]
		string Search { get; }

		[Export ("text")]
		string Text { get; }

		[Export ("absoluteLinkURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageUrl { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLAppletElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlAppletElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("alt")]
		string Alt { get; set; }

		[Export ("archive")]
		string Archive { get; set; }

		[Export ("code")]
		string Code { get; set; }

		[Export ("codeBase")]
		string CodeBase { get; set; }

		[Export ("height")]
		string Height { get; set; }

		[Export ("hspace")]
		int HSpace { get; set; } /* int, not NSInteger */

		[Export ("name")]
		string Name { get; set; }

		[Export ("object")]
		string Object { get; set; }

		[Export ("vspace")]
		int VSpace { get; set; } /* int, not NSInteger */

		[Export ("width")]
		string Width { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLAreaElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlAreaElement {

		[Export ("alt")]
		string Alt { get; set; }

		[Export ("coords")]
		string Coords { get; set; }

		[Export ("href")]
		string HRef { get; set; }

		[Export ("noHref")]
		bool NoHRef { get; set; }

		[Export ("shape")]
		string Shape { get; set; }

		[Export ("target")]
		string Target { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("accessKey")]
		string AccessKey { get; set; }

		[Export ("hashName")]
		string HashName { get; }

		[Export ("host")]
		string Host { get; }

		[Export ("hostname")]
		string Hostname { get; }

		[Export ("pathname")]
		string Pathname { get; }

		[Export ("port")]
		string Port { get; }

		[Export ("protocol")]
		string Protocol { get; }

		[Export ("search")]
		string Search { get; }

		[Export ("absoluteLinkURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageUrl { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBRElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBRElement {

		[Export ("clear")]
		string Clear { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBaseElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBaseElement {

		[Export ("href")]
		string HRef { get; set; }

		[Export ("target")]
		string Target { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBaseFontElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBaseFontElement {

		[Export ("color")]
		string Color { get; set; }

		[Export ("face")]
		string Face { get; set; }

		[Export ("size")]
		string Size { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBodyElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBodyElement {

		[Export ("aLink")]
		string ALink { get; set; }

		[Export ("background")]
		string Background { get; set; }

		[Export ("bgColor")]
		string BgColor { get; set; }

		[Export ("link")]
		string Link { get; set; }

		[Export ("text")]
		string Text { get; set; }

		[Export ("vLink")]
		string VLink { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLButtonElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlButtonElement {

		[Export ("autofocus")]
		bool Autofocus { get; set; }

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("value")]
		string Value { get; set; }

		[Export ("willValidate")]
		bool WillValidate { get; }

		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("accessKey")]
		string AccessKey { get; set; }

		[Export ("click")]
		void Click ();
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDListElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDListElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDirectoryElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDirectoryElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDivElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDivElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLEmbedElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlEmbedElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("height")]
		int Height { get; set; } /* int, not NSInteger */

		[Export ("name")]
		string Name { get; set; }

		[Export ("src")]
		string Src { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("width")]
		int Width { get; set; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFieldSetElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFieldSetElement {

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFontElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFontElement {

		[Export ("color")]
		string Color { get; set; }

		[Export ("face")]
		string Face { get; set; }

		[Export ("size")]
		string Size { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFrameElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFrameElement {

		[Export ("frameBorder")]
		string FrameBorder { get; set; }

		[Export ("longDesc")]
		string LongDesc { get; set; }

		[Export ("marginHeight")]
		string MarginHeight { get; set; }

		[Export ("marginWidth")]
		string MarginWidth { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("noResize")]
		bool NoResize { get; set; }

		[Export ("scrolling")]
		string Scrolling { get; set; }

		[Export ("src")]
		string Src { get; set; }

		[Export ("contentDocument", ArgumentSemantic.Retain)]
		DomDocument ContentDocument { get; }

		[Export ("contentWindow", ArgumentSemantic.Retain)]
		DomAbstractView ContentWindow { get; }

		[Export ("location")]
		string Location { get; set; }

		[Export ("width")]
		int Width { get; } /* int, not NSInteger */

		[Export ("height")]
		int Height { get; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFrameSetElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFrameSetElement {

		[Export ("cols")]
		string Cols { get; set; }

		[Export ("rows")]
		string Rows { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHRElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHRElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("noShade")]
		bool NoShade { get; set; }

		[Export ("size")]
		string Size { get; set; }

		[Export ("width")]
		string Width { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHeadElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHeadElement {

		[Export ("profile")]
		string Profile { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHeadingElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHeadingElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHtmlElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHtmlElement {

		[Export ("version")]
		string Version { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLIFrameElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlIFrameElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("frameBorder")]
		string FrameBorder { get; set; }

		[Export ("height")]
		string Height { get; set; }

		[Export ("longDesc")]
		string LongDesc { get; set; }

		[Export ("marginHeight")]
		string MarginHeight { get; set; }

		[Export ("marginWidth")]
		string MarginWidth { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("scrolling")]
		string Scrolling { get; set; }

		[Export ("src")]
		string Src { get; set; }

		[Export ("width")]
		string Width { get; set; }

		[Export ("contentDocument", ArgumentSemantic.Retain)]
		DomDocument ContentDocument { get; }

		[Export ("contentWindow", ArgumentSemantic.Retain)]
		DomAbstractView ContentWindow { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLImageElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlImageElement {

		[Export ("name")]
		string Name { get; set; }

		[Export ("align")]
		string Align { get; set; }

		[Export ("alt")]
		string Alt { get; set; }

		[Export ("border")]
		string Border { get; set; }

		[Export ("height")]
		int Height { get; set; } /* int, not NSInteger */

		[Export ("hspace")]
		int HSpace { get; set; } /* int, not NSInteger */

		[Export ("isMap")]
		bool IsMap { get; set; }

		[Export ("longDesc")]
		string LongDesc { get; set; }

		[Export ("src")]
		string Src { get; set; }

		[Export ("useMap")]
		string UseMap { get; set; }

		[Export ("vspace")]
		int VSpace { get; set; } /* int, not NSInteger */

		[Export ("width")]
		int Width { get; set; } /* int, not NSInteger */

		[Export ("complete")]
		bool Complete { get; }

		[Export ("lowsrc")]
		string Lowsrc { get; set; }

		[Export ("naturalHeight")]
		int NaturalHeight { get; } /* int, not NSInteger */

		[Export ("naturalWidth")]
		int NaturalWidth { get; } /* int, not NSInteger */

		[Export ("x")]
		int X { get; } /* int, not NSInteger */

		[Export ("y")]
		int Y { get; } /* int, not NSInteger */

		[Export ("altDisplayString")]
		string AltDisplayString { get; }

		[Export ("absoluteImageURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageUrl { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLLIElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlLIElement {

		[Export ("type")]
		string Type { get; set; }

		[Export ("value")]
		int Value { get; set; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLLabelElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlLabelElement {

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("htmlFor")]
		string HtmlFor { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("accessKey")]
		string AccessKey { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLLegendElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlLegendElement {

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("align")]
		string Align { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("accessKey")]
		string AccessKey { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLLinkElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlLinkElement {

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("charset")]
		string Charset { get; set; }

		[Export ("href")]
		string HRef { get; set; }

		[Export ("hreflang")]
		string HRefLang { get; set; }

		[Export ("media")]
		string Media { get; set; }

		[Export ("rel")]
		string Rel { get; set; }

		[Export ("rev")]
		string Rev { get; set; }

		[Export ("target")]
		string Target { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("sheet", ArgumentSemantic.Retain)]
		DomStyleSheet Sheet { get; }

		[Export ("absoluteLinkURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageUrl { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMapElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMapElement {

		[Export ("areas", ArgumentSemantic.Retain)]
		DomHtmlCollection Areas { get; }

		[Export ("name")]
		string Name { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMarqueeElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMarqueeElement {

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMenuElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMenuElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMetaElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMetaElement {

		[Export ("content")]
		string Content { get; set; }

		[Export ("httpEquiv")]
		string HttpEquiv { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("scheme")]
		string Scheme { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLModElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlModElement {

		[Export ("cite")]
		string Cite { get; set; }

		[Export ("dateTime")]
		string DateTime { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLOListElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlOListElement {

		[Export ("compact")]
		bool Compact { get; set; }

		[Export ("start")]
		int Start { get; set; } /* int, not NSInteger */

		[Export ("type")]
		string Type { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLObjectElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlObjectElement {

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("code")]
		string Code { get; set; }

		[Export ("align")]
		string Align { get; set; }

		[Export ("archive")]
		string Archive { get; set; }

		[Export ("border")]
		string Border { get; set; }

		[Export ("codeBase")]
		string CodeBase { get; set; }

		[Export ("codeType")]
		string CodeType { get; set; }

		[Export ("data")]
		string Data { get; set; }

		[Export ("declare")]
		bool Declare { get; set; }

		[Export ("height")]
		string Height { get; set; }

		[Export ("hspace")]
		int HSpace { get; set; } /* int, not NSInteger */

		[Export ("name")]
		string Name { get; set; }

		[Export ("standby")]
		string Standby { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("useMap")]
		string UseMap { get; set; }

		[Export ("vspace")]
		int VSpace { get; set; } /* int, not NSInteger */

		[Export ("width")]
		string Width { get; set; }

		[Export ("contentDocument", ArgumentSemantic.Retain)]
		DomDocument ContentDocument { get; }

		[Export ("absoluteImageURL", ArgumentSemantic.Copy)]
		NSUrl AbsoluteImageUrl { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLOptGroupElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlOptGroupElement {

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("label")]
		string Label { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLOptionElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlOptionElement {

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("label")]
		string Label { get; set; }

		[Export ("defaultSelected")]
		bool DefaultSelected { get; set; }

		[Export ("selected")]
		bool Selected { get; set; }

		[Export ("value")]
		string Value { get; set; }

		[Export ("text")]
		string Text { get; }

		[Export ("index")]
		int Index { get; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMHTMLOptionsCollection")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlOptionsCollection {

		[Export ("selectedIndex")]
		int SelectedIndex { get; set; } /* int, not NSInteger */

		[Export ("length")]
		uint Length { get; set; } /* unsigned int */

		[Export ("namedItem:")]
		DomNode NamedItem (string name);

		[Export ("add:index:")]
		void Add (DomHtmlOptionElement option, uint /* unsigned int */ index);

		[Export ("remove:")]
		void Remove (uint /* unsigned int */ index);

		[Export ("item:")]
		DomNode GetItem (uint /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLParagraphElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlParagraphElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLParamElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlParamElement {

		[Export ("name")]
		string Name { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("value")]
		string Value { get; set; }

		[Export ("valueType")]
		string ValueType { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLPreElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlPreElement {

		[Export ("width")]
		int Width { get; set; } /* int, not NSInteger */

		[Export ("wrap")]
		bool Wrap { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLQuoteElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlQuoteElement {

		[Export ("cite")]
		string Cite { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLScriptElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlScriptElement {

		[Export ("text")]
		string Text { get; set; }

		[Export ("htmlFor")]
		string HtmlFor { get; set; }

		[Export ("event")]
		string Event { get; set; }

		[Export ("charset")]
		string Charset { get; set; }

		[Export ("defer")]
		bool Defer { get; set; }

		[Export ("src")]
		string Src { get; set; }

		[Export ("type")]
		string Type { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLSelectElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlSelectElement {

		[Export ("autofocus")]
		bool Autofocus { get; set; }

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }

		[Export ("multiple")]
		bool Multiple { get; set; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("size")]
		int Size { get; set; } /* int, not NSInteger */

		[Export ("type")]
		string Type { get; }

		[Export ("options", ArgumentSemantic.Retain)]
		DomHtmlOptionsCollection Options { get; }

		[Export ("length")]
		int Length { get; } /* int, not NSInteger */

		[Export ("selectedIndex")]
		int SelectedIndex { get; set; } /* int, not NSInteger */

		[Export ("value")]
		string Value { get; set; }

		[Export ("willValidate")]
		bool WillValidate { get; }

		[Export ("item:")]
		DomNode GetItem (uint /* unsigned int */ index);

		[Export ("namedItem:")]
		DomNode NamedItem (string name);

		[Export ("add:before:")]
		void Add (DomHtmlElement element, DomHtmlElement before);

		[Export ("remove:")]
		void Remove (int /* int, not NSInteger */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLStyleElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlStyleElement {

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("media")]
		string Media { get; set; }

		[Export ("type")]
		string Type { get; set; }

		[Export ("sheet", ArgumentSemantic.Retain)]
		DomStyleSheet Sheet { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableCaptionElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableCaptionElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableCellElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableCellElement {

		[Export ("cellIndex")]
		int CellIndex { get; } /* int, not NSInteger */

		[Export ("abbr")]
		string Abbr { get; set; }

		[Export ("align")]
		string Align { get; set; }

		[Export ("axis")]
		string Axis { get; set; }

		[Export ("bgColor")]
		string BgColor { get; set; }

		[Export ("ch")]
		string Ch { get; set; }

		[Export ("chOff")]
		string ChOff { get; set; }

		[Export ("colSpan")]
		int ColSpan { get; set; } /* int, not NSInteger */

		[Export ("headers")]
		string Headers { get; set; }

		[Export ("height")]
		string Height { get; set; }

		[Export ("noWrap")]
		bool NoWrap { get; set; }

		[Export ("rowSpan")]
		int RowSpan { get; set; } /* int, not NSInteger */

		[Export ("scope")]
		string Scope { get; set; }

		[Export ("vAlign")]
		string VAlign { get; set; }

		[Export ("width")]
		string Width { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableColElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableColElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("ch")]
		string Ch { get; set; }

		[Export ("chOff")]
		string ChOff { get; set; }

		[Export ("span")]
		int Span { get; set; } /* int, not NSInteger */

		[Export ("vAlign")]
		string VAlign { get; set; }

		[Export ("width")]
		string Width { get; set; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableElement {

		[Export ("caption", ArgumentSemantic.Retain)]
		DomHtmlTableCaptionElement Caption { get; set; }

		[Export ("tHead", ArgumentSemantic.Retain)]
		DomHtmlTableSectionElement THead { get; set; }

		[Export ("tFoot", ArgumentSemantic.Retain)]
		DomHtmlTableSectionElement TFoot { get; set; }

		[Export ("rows", ArgumentSemantic.Retain)]
		DomHtmlCollection Rows { get; }

		[Export ("tBodies", ArgumentSemantic.Retain)]
		DomHtmlCollection TBodies { get; }

		[Export ("align")]
		string Align { get; set; }

		[Export ("bgColor")]
		string BgColor { get; set; }

		[Export ("border")]
		string Border { get; set; }

		[Export ("cellPadding")]
		string CellPadding { get; set; }

		[Export ("cellSpacing")]
		string CellSpacing { get; set; }

		[Export ("frameBorders")]
		string FrameBorders { get; set; }

		[Export ("rules")]
		string Rules { get; set; }

		[Export ("summary")]
		string Summary { get; set; }

		[Export ("width")]
		string Width { get; set; }

		[Export ("createTHead")]
		DomHtmlElement CreateTHead ();

		[Export ("deleteTHead")]
		void DeleteTHead ();

		[Export ("createTFoot")]
		DomHtmlElement CreateTFoot ();

		[Export ("deleteTFoot")]
		void DeleteTFoot ();

		[Export ("createCaption")]
		DomHtmlElement CreateCaption ();

		[Export ("deleteCaption")]
		void DeleteCaption ();

		[Export ("insertRow:")]
		DomHtmlElement InsertRow (int /* int, not NSInteger */ index);

		[Export ("deleteRow:")]
		void DeleteRow (int /* int, not NSInteger */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableRowElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableRowElement {

		[Export ("rowIndex")]
		int RowIndex { get; } /* int, not NSInteger */

		[Export ("sectionRowIndex")]
		int SectionRowIndex { get; } /* int, not NSInteger */

		[Export ("cells", ArgumentSemantic.Retain)]
		DomHtmlCollection Cells { get; }

		[Export ("align")]
		string Align { get; set; }

		[Export ("bgColor")]
		string BgColor { get; set; }

		[Export ("ch")]
		string Ch { get; set; }

		[Export ("chOff")]
		string ChOff { get; set; }

		[Export ("vAlign")]
		string VAlign { get; set; }

		[Export ("insertCell:")]
		DomHtmlElement InsertCell (int /* int, not NSInteger */ index);

		[Export ("deleteCell:")]
		void DeleteCell (int /* int, not NSInteger */ index);
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableSectionElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	[Deprecated (PlatformName.MacOSX, 10, 14)]
	interface DomHtmlTableSectionElement {

		[Export ("align")]
		string Align { get; set; }

		[Export ("ch")]
		string Ch { get; set; }

		[Export ("chOff")]
		string ChOff { get; set; }

		[Export ("vAlign")]
		string VAlign { get; set; }

		[Export ("rows", ArgumentSemantic.Retain)]
		DomHtmlCollection Rows { get; }

		[Export ("insertRow:")]
		DomHtmlElement InsertRow (int /* int, not NSInteger */ index);

		[Export ("deleteRow:")]
		void DeleteRow (int /* int, not NSInteger */ index);
	}

	[iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum WKFullscreenState : long {
		NotInFullscreen,
		EnteringFullscreen,
		InFullscreen,
		ExitingFullscreen,
	}

	[iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum WKDialogResult : long {
		ShowDefault = 1,
		AskAgain,
		Handled,
	}

	[iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum WKCookiePolicy : long {
		Allow,
		Disallow,
	}

	[iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum WKInactiveSchedulingPolicy : long {
		Suspend,
		Throttle,
		None,
	}

	[NoiOS, NoMacCatalyst]
	[Native]
	public enum WKUserInterfaceDirectionPolicy : long {
		Content,
		System,
	}

	/// <summary>A page within a <see cref="WebKit.WKBackForwardList" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKBackForwardListItem_Ref/index.html">Apple documentation for <c>WKBackForwardListItem</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardListItem {

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("title")]
		[NullAllowed]
		string Title { get; }

		[Export ("initialURL", ArgumentSemantic.Copy)]
		NSUrl InitialUrl { get; }
	}

	/// <summary>The list of pages that can be reached by navigating forward or backward.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKBackForwardList_Ref/index.html">Apple documentation for <c>WKBackForwardList</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKBackForwardList {

		[Export ("currentItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem CurrentItem { get; }

		[Export ("backItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem BackItem { get; }

		[Export ("forwardItem", ArgumentSemantic.Strong)]
		[NullAllowed]
		WKBackForwardListItem ForwardItem { get; }

		[Export ("backList")]
		WKBackForwardListItem [] BackList { get; }

		[Export ("forwardList")]
		WKBackForwardListItem [] ForwardList { get; }

		/// <param name="index">To be added.</param>
		/// <summary>Gets the item at the specified index in the list, where the current item has index 0.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("itemAtIndex:")]
		[return: NullAllowed]
		WKBackForwardListItem ItemAtIndex (nint index);
	}

	/// <summary>A list of rules to apply to web content.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Apple: "You don’t create a WKContentRuleList directly."
	interface WKContentRuleList {
		[Export ("identifier")]
		string Identifier { get; }
	}

	/// <summary>A store that contents rules for web content.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // instances created with the default ctor crashes on dealloc
	interface WKContentRuleListStore {
		[Static]
		[Export ("defaultStore")]
		WKContentRuleListStore DefaultStore { get; }

		[Static]
		[Export ("storeWithURL:")]
		WKContentRuleListStore FromUrl (NSUrl url);

		[Export ("compileContentRuleListForIdentifier:encodedContentRuleList:completionHandler:")]
		[Async (XmlDocs = """
			<param name="identifier">The identifier for the newly compiled list.</param>
			<param name="encodedContentRuleList">JSON source to compile.</param>
			<summary>Compiles the provided list of rules, adds the list to the store with the specified <paramref name="identifier" />, and runs a handler that receives the content list and any error that is encountered.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous CompileContentRuleList operation.  The value of the TResult parameter is of type System.Action&lt;WebKit.WKContentRuleList,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void CompileContentRuleList (string identifier, string encodedContentRuleList, Action<WKContentRuleList, NSError> completionHandler);

		[Export ("lookUpContentRuleListForIdentifier:completionHandler:")]
		[Async (XmlDocs = """
			<param name="identifier">The identifer for the rule list to look up.</param>
			<summary>Asynchronously finds and returns the content rule list that is specified by the provided <paramref name="identifier" />.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LookUpContentRuleList operation.  The value of the TResult parameter is of type System.Action&lt;WebKit.WKContentRuleList,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void LookUpContentRuleList (string identifier, Action<WKContentRuleList, NSError> completionHandler);

		[Export ("removeContentRuleListForIdentifier:completionHandler:")]
		[Async (XmlDocs = """
			<param name="identifier">The identifier for the list to remove.</param>
			<summary>Asynchronously removes the content rule list that is specified by the provided <paramref name="identifier" />.</summary>
			<returns>A task that represents the asynchronous RemoveContentRuleList operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void RemoveContentRuleList (string identifier, Action<NSError> completionHandler);

		[Export ("getAvailableContentRuleListIdentifiers:")]
		[Async (XmlDocs = """
			<summary>Asynchronously retrieves the list of identifiers for available content rule lists.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetAvailableContentRuleListIdentifiers operation.  The value of the TResult parameter is of type System.Action&lt;System.String[]&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetAvailableContentRuleListIdentifiersAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void GetAvailableContentRuleListIdentifiers (Action<string []> callback);
	}

	/// <summary>Manages cookies for a <see cref="WebKit.WKWebsiteDataStore" />.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "WKHTTPCookieStore")]
	[DisableDefaultCtor]
	interface WKHttpCookieStore {
		[Export ("getAllCookies:")]
		[Async (XmlDocs = """
			<summary>Asynchronously fetches all the cookies.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetAllCookies operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSHttpCookie[]&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void GetAllCookies (Action<NSHttpCookie []> completionHandler);

		[Export ("setCookie:completionHandler:")]
		[Async (XmlDocs = """
			<param name="cookie">The cookie to set.</param>
			<summary>Sets the specified <paramref name="cookie" /> and runs a handler when the operation completes.</summary>
			<returns>A task that represents the asynchronous SetCookie operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void SetCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);

		[Export ("deleteCookie:completionHandler:")]
		[Async (XmlDocs = """
			<param name="cookie">The cookie to remove.</param>
			<summary>Deletes the specified <paramref name="cookie" /> from the store and runs a completion handler when the operation is complete.</summary>
			<returns>A task that represents the asynchronous DeleteCookie operation</returns>
			<remarks>
			          <para copied="true">The DeleteCookieAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void DeleteCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);

		/// <summary>Add an observer to the cookie store.</summary>
		/// <param name="observer">The observer to add.</param>
		/// <remarks>The cookie store only keeps a weak reference to the <paramref name="observer" />, a separate (strong) reference must be kept for the observer to not be collected by the garbage collector. Call <see cref="RemoveObserver" /> to remove the observer to stop observing.</remarks>
		[Export ("addObserver:")]
		void AddObserver (IWKHttpCookieStoreObserver observer);

		/// <summary>Remove an observer to the cookie store.</summary>
		/// <param name="observer">The observer to remove.</param>
		[Export ("removeObserver:")]
		void RemoveObserver (IWKHttpCookieStoreObserver observer);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("setCookiePolicy:completionHandler:")]
		[Async]
		void SetCookiePolicy (WKCookiePolicy policy, [NullAllowed] Action completionHandler);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("getCookiePolicy:")]
		[Async]
		void GetCookiePolicy (Action<WKCookiePolicy> completionHandler);

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Async]
		[Export ("setCookies:completionHandler:")]
		void SetCookies (NSHttpCookie [] cookies, [NullAllowed] Action completionHandler);

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Async]
		[Export ("getCookiesForURL:completionHandler:")]
		void GetCookies (NSUrl url, WKHttpCookieStoreGetCookiesCallback completionHandler);
	}

	delegate void WKHttpCookieStoreGetCookiesCallback (NSHttpCookie [] cookies);

	interface IWKHttpCookieStoreObserver { }

	/// <summary>Interface that represents the required members of the WKHttpCookieStoreObserver protocol.</summary>
	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKHTTPCookieStoreObserver")]
	interface WKHttpCookieStoreObserver {
		/// <param name="cookieStore">The store that changed.</param>
		/// <summary>Method that is called when a cookie changes in the cookie store.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("cookiesDidChangeInCookieStore:")]
		void CookiesDidChangeInCookieStore (WKHttpCookieStore cookieStore);
	}

	/// <summary>A frame within a page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKFrameInfo_Ref/index.html">Apple documentation for <c>WKFrameInfo</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // No public init method and every property is get-only
	interface WKFrameInfo : NSCopying {

#if !XAMCORE_5_0
		[Obsoleted (PlatformName.iOS, 26, 2, message: ".ctor is not usable.")]
		[Obsoleted (PlatformName.MacCatalyst, 26, 2, message: ".ctor is not usable.")]
		[Obsoleted (PlatformName.MacOSX, 26, 2, message: ".ctor is not usable.")]
		[Export ("init")]
		NativeHandle Constructor ();
#endif

		/// <summary>Gets a value that indicates whether the frame is the main frame or a subframe.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("mainFrame")]
		bool MainFrame { [Bind ("isMainFrame")] get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[MacCatalyst (13, 1)]
		[Export ("securityOrigin")]
		WKSecurityOrigin SecurityOrigin { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }
	}

	[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKFormInfo {

		[Export ("targetFrame")]
		WKFrameInfo TargetFrame { get; }

		[Export ("sourceFrame")]
		WKFrameInfo SourceFrame { get; }

		[Export ("submissionURL")]
		NSUrl SubmissionUrl { get; }

		[Export ("httpMethod")]
		string HttpMethod { get; }

		[Export ("formValues")]
		NSDictionary<NSString, NSString> FormValues { get; }
	}

	/// <summary>Tracks the loading progress of a page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKNavigation_Ref/index.html">Apple documentation for <c>WKNavigation</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKNavigation {

		[MacCatalyst (13, 1)]
		[Export ("effectiveContentMode")]
		WKContentMode EffectiveContentMode { get; }
	}

	/// <summary>Information about a navigation action. Can be used for policy decisions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKNavigationAction_Ref/index.html">Apple documentation for <c>WKNavigationAction</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKNavigationAction {

		[Export ("sourceFrame", ArgumentSemantic.Copy)]
		WKFrameInfo SourceFrame { get; }

		[Export ("targetFrame", ArgumentSemantic.Copy)]
		[NullAllowed]
		WKFrameInfo TargetFrame { get; }

		[Export ("navigationType")]
		WKNavigationType NavigationType { get; }

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[Export ("modifierFlags")]
		[iOS (18, 4), MacCatalyst (18, 4)]
#if __IOS__ || __MACCATALYST_
		UIKeyModifierFlags ModifierFlags { get; }
#else
		NSEventModifierMask ModifierFlags { get; }
#endif

		[Export ("buttonNumber")]
		[iOS (18, 4), MacCatalyst (18, 4)]
#if __IOS__ || __MACCATALYST_
		UIEventButtonMask ButtonNumber { get; }
#else
		nint ButtonNumber { get; }
#endif

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("shouldPerformDownload")]
		bool ShouldPerformDownload { get; }

		[iOS (26, 0), Mac (26, 0), MacCatalyst (26, 0)]
		[Export ("isContentRuleListRedirect")]
		bool IsContentRuleListRedirect { get; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("mainFrameNavigation")]
		WKNavigation MainFrameNavigation { get; }
	}

	/// <summary>Delegate object for <see cref="WebKit.WKNavigation" /> objects, provides methods relating to navigation and load policies.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKNavigationDelegate_Ref/index.html">Apple documentation for <c>WKNavigationDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKNavigationDelegate {

		/// <param name="webView">To be added.</param>
		/// <param name="navigationAction">To be added.</param>
		/// <param name="decisionHandler">To be added.</param>
		/// <summary>Assigns an action to be taken after the specified <paramref name="navigationAction" /> has been either canceled or allowed.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:decidePolicyForNavigationAction:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler);

		/// <param name="webView">To be added.</param>
		/// <param name="navigationResponse">To be added.</param>
		/// <param name="decisionHandler">To be added.</param>
		/// <summary>Assigns an action to be taken after the specified <paramref name="navigationResponse" /> has been either canceled or allowed.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:decidePolicyForNavigationResponse:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler);

		[MacCatalyst (13, 1)]
		[Export ("webView:decidePolicyForNavigationAction:preferences:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, WKWebpagePreferences preferences, Action<WKNavigationActionPolicy, WKWebpagePreferences> decisionHandler);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <summary>Method that is called when data begins to load.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didStartProvisionalNavigation:")]
		void DidStartProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <summary>Method that is called when a server redirect is received.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didReceiveServerRedirectForProvisionalNavigation:")]
		void DidReceiveServerRedirectForProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>Method that is called when a committed navigation fails after data has begun to load.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFailProvisionalNavigation:withError:")]
		void DidFailProvisionalNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <summary>Method that is called when content begins to load.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didCommitNavigation:")]
		void DidCommitNavigation (WKWebView webView, WKNavigation navigation);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <summary>Method that is called when all the data is loaded.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFinishNavigation:")]
		void DidFinishNavigation (WKWebView webView, WKNavigation navigation);

		/// <param name="webView">To be added.</param>
		/// <param name="navigation">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>Method that is called when a committed navigation fails.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didFailNavigation:withError:")]
		void DidFailNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		/// <param name="webView">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Method that is called when an authentication challenge is issued.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:didReceiveAuthenticationChallenge:completionHandler:")]
		void DidReceiveAuthenticationChallenge (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		/// <param name="webView">To be added.</param>
		/// <summary>Method that is called when a web view's content is terminated.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("webViewWebContentProcessDidTerminate:")]
		void ContentProcessDidTerminate (WKWebView webView);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("webView:authenticationChallenge:shouldAllowDeprecatedTLS:")]
		void ShouldAllowDeprecatedTls (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<bool> decisionHandler);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("webView:navigationAction:didBecomeDownload:")]
		void NavigationActionDidBecomeDownload (WKWebView webView, WKNavigationAction navigationAction, WKDownload download);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("webView:navigationResponse:didBecomeDownload:")]
		void NavigationResponseDidBecomeDownload (WKWebView webView, WKNavigationResponse navigationResponse, WKDownload download);

		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4), NoTV]
		[Export ("webView:shouldGoToBackForwardListItem:willUseInstantBack:completionHandler:")]
		void ShouldGoToBackForwardListItem (WKWebView webView, WKBackForwardListItem backForwardListItem, bool willUseInstantBack, WKNavigationDelegateShouldGoToBackForwardListItemCallback completionHandler);

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("webView:willSubmitForm:submissionHandler:")]
		void WillSubmitForm (WKWebView webView, WKFormInfo formInfo, WKNavigationDelegateWillSubmitFormCallback submissionHandler);
	}

	delegate void WKNavigationDelegateShouldGoToBackForwardListItemCallback (bool shouldGoToItem);

	delegate void WKNavigationDelegateWillSubmitFormCallback ();

	interface IWKNavigationDelegate { }

	/// <summary>Information about a navigation response. Can be used for policy decisions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKNavigationResponse_Ref/index.html">Apple documentation for <c>WKNavigationResponse</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKNavigationResponse {

		/// <summary>Gets a value that indicates whether the response resulted from a request that was sent by the main frame.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("forMainFrame")]
		bool IsForMainFrame { [Bind ("isForMainFrame")] get; }

		[Export ("response", ArgumentSemantic.Copy)]
		NSUrlResponse Response { get; }

		[Export ("canShowMIMEType")]
		bool CanShowMimeType { get; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("mainFrameNavigation")]
		WKNavigation MainFrameNavigation { get; }
	}

	/// <summary>Preference settings for a <see cref="WebKit.WKWebView" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKPreferences_Ref/index.html">Apple documentation for <c>WKPreferences</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKPreferences : NSSecureCoding {
		[Export ("minimumFontSize")]
		nfloat MinimumFontSize { get; set; }

		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'WKWebPagePreferences.AllowsContentJavaScript' instead.")]
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'WKWebPagePreferences.AllowsContentJavaScript' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'WKWebPagePreferences.AllowsContentJavaScript' instead.")]
		[Export ("javaScriptEnabled")]
		bool JavaScriptEnabled { get; set; }

		[Export ("javaScriptCanOpenWindowsAutomatically")]
		bool JavaScriptCanOpenWindowsAutomatically { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Feature no longer supported.")]
		[Export ("javaEnabled")]
		bool JavaEnabled { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Feature no longer supported.")]
		[Export ("plugInsEnabled")]
		bool PlugInsEnabled { get; set; }

		// Headers says 10,12,3 but it is not available likely they meant 10,12,4
		[NoiOS]
		[NoMacCatalyst]
		[Export ("tabFocusesLinks")]
		bool TabFocusesLinks { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fraudulentWebsiteWarningEnabled")]
		bool FraudulentWebsiteWarningEnabled { [Bind ("isFraudulentWebsiteWarningEnabled")] get; set; }

		[iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("shouldPrintBackgrounds")]
		bool ShouldPrintBackgrounds { get; set; }

		[Internal]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("textInteractionEnabled")]
		bool _OldTextInteractionEnabled { get; set; }

		[Internal]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("isTextInteractionEnabled")]
		bool _NewGetTextInteractionEnabled ();

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("siteSpecificQuirksModeEnabled")]
		bool SiteSpecificQuirksModeEnabled { [Bind ("isSiteSpecificQuirksModeEnabled")] get; set; }

		[iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("elementFullscreenEnabled")]
		bool ElementFullscreenEnabled { [Bind ("isElementFullscreenEnabled")] get; set; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("inactiveSchedulingPolicy", ArgumentSemantic.Assign)]
		WKInactiveSchedulingPolicy InactiveSchedulingPolicy { get; set; }
	}

	/// <summary>A message sent from a Web page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKScriptMessage/index.html">Apple documentation for <c>WKScriptMessage</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKScriptMessage {

		// May be typed as NSNumber, NSString, NSDate, NSArray,
		// NSDictionary, or NSNull, as it must map cleanly to JSON
		[Export ("body", ArgumentSemantic.Copy)]
		NSObject Body { get; }

		[Export ("webView", ArgumentSemantic.Weak)]
		[NullAllowed]
		WKWebView WebView { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("frameInfo", ArgumentSemantic.Copy)]
		WKFrameInfo FrameInfo { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("world")]
		WKContentWorld World { get; }
	}

	interface IWKScriptMessageHandler { }

	/// <summary>Allows messages from JavaScript to be handled by the app.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKScriptMessageHandler_Ref/index.html">Apple documentation for <c>WKScriptMessageHandler</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKScriptMessageHandler {

		/// <param name="userContentController">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <summary>Method that is called after a message is received from a script.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userContentController:didReceiveScriptMessage:")]
		[Abstract]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message);
	}

	/// <summary>Describes the hostname, protocol, and port number for a security origin.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKSecurityOrigin_Class_Ref/index.html">Apple documentation for <c>WKSecurityOrigin</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKSecurityOrigin {
		[Export ("protocol")]
		string Protocol { get; }

		[Export ("host")]
		string Host { get; }

		[Export ("port")]
		nint Port { get; }
	}


	/// <summary>Holds the specification for a snapshot of a Webpage taken with <see cref="WebKit.WKWebView.TakeSnapshotAsync(WebKit.WKSnapshotConfiguration)" />.</summary>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKSnapshotConfiguration : NSCopying {
		[Export ("rect")]
		CGRect Rect { get; set; }

		[Export ("snapshotWidth")]
		[NullAllowed]
		NSNumber SnapshotWidth { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("afterScreenUpdates")]
		bool AfterScreenUpdates { get; set; }
	}

	interface IWKUrlSchemeHandler { }
	/// <summary>Interface for handling arbitrary URL schemes.</summary>
	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKURLSchemeHandler")]
	interface WKUrlSchemeHandler {
		/// <param name="webView">The web view that is making the request.</param>
		/// <param name="urlSchemeTask">The task for which to load data.</param>
		/// <summary>Starts a URL scheme task that processes a URL and loads data for the specified <paramref name="webView" />.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("webView:startURLSchemeTask:")]
		void StartUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);

		/// <param name="webView">The web view that is making the request.</param>
		/// <param name="urlSchemeTask">The task for which to stop loading data.</param>
		/// <summary>Stops a URL scheme task that processes a URL and loads data for the specified <paramref name="webView" />.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("webView:stopURLSchemeTask:")]
		void StopUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);
	}

	interface IWKUrlSchemeTask { }

	/// <summary>Interface for a task that loads data from a URL with an arbitrary scheme.</summary>
	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKURLSchemeTask")]
	interface WKUrlSchemeTask {
		/// <summary>Gets the request.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		/// <param name="response">The response that was received.</param>
		/// <summary>Method that is called to indicate that the task received a response.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("didReceiveResponse:")]
		void DidReceiveResponse (NSUrlResponse response);

		/// <param name="data">The data that was received.</param>
		/// <summary>Method that is called to indicate that the task received the data.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("didReceiveData:")]
		void DidReceiveData (NSData data);

		/// <summary>Method that is called to indicate that the task is finished.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("didFinish")]
		void DidFinish ();

		/// <param name="error">The error that occurred.</param>
		/// <summary>Method that is called to indicate failure.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("didFailWithError:")]
		void DidFailWithError (NSError error);
	}

	/// <summary>Website data group that is identified by the website's domain and suffix.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKWebsiteDataRecord_Class_Ref/index.html">Apple documentation for <c>WKWebsiteDataRecord</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebsiteDataRecord {
		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("dataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> DataTypes { get; }
	}

	/// <summary>Contains NSString constants that represent data types for data related to websites.</summary>
	[MacCatalyst (13, 1)]
	[Static]
	interface WKWebsiteDataType {
		/// <summary>Gets an NSString that signifies a disk cache.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeDiskCache".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeDiskCache", "WebKit")]
		NSString DiskCache { get; }

		/// <summary>Gets an NSString that signifies an in-memory cache.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeMemoryCache".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeMemoryCache", "WebKit")]
		NSString MemoryCache { get; }

		/// <summary>Gets an NSString that signifies an offline HTML cache for a web app.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeOfflineWebApplicationCache".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeOfflineWebApplicationCache", "WebKit")]
		NSString OfflineWebApplicationCache { get; }

		/// <summary>Gets an NSString that signifies cookie data.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeCookies".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeCookies", "WebKit")]
		NSString Cookies { get; }

		/// <summary>Gets an NSString that signifies HTML storage for a session.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeSessionStorage".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeSessionStorage")]
		NSString SessionStorage { get; }

		/// <summary>Gets an NSString that signifies local HTML storage.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeLocalStorage".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeLocalStorage", "WebKit")]
		NSString LocalStorage { get; }

		/// <summary>Gets an NSString that signifies a WebSQL database.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeWebSQLDatabases".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeWebSQLDatabases", "WebKit")]
		NSString WebSQLDatabases { get; }

		/// <summary>Gets an NSString that signifies IndexedDB databases.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeIndexedDBDatabases".</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKWebsiteDataTypeIndexedDBDatabases", "WebKit")]
		NSString IndexedDBDatabases { get; }

		/// <summary>Gets an NSString that signifies a fetch cache.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeFetchCache".</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("WKWebsiteDataTypeFetchCache")]
		NSString FetchCache { get; }

		/// <summary>Gets an NSString that signifies service worker registrations.</summary>
		///         <value>The NSString object for "WKWebsiteDataTypeServiceWorkerRegistrations".</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("WKWebsiteDataTypeServiceWorkerRegistrations")]
		NSString ServiceWorkerRegistrations { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Field ("WKWebsiteDataTypeFileSystem")]
		NSString FileSystem { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeSearchFieldRecentSearches")]
		NSString SearchFieldRecentSearches { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeMediaKeys")]
		NSString MediaKeys { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeHashSalt")]
		NSString HashSalt { get; }

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("WKWebsiteDataTypeScreenTime")]
		NSString TypeScreenTime { get; }
	}

	[NoiOS, NoMacCatalyst]
	[Static]
	interface WebViewNotification {
		[Notification]
		[Field ("WebViewDidBeginEditingNotification")]
		NSString DidBeginEditing { get; }

		[Notification]
		[Field ("WebViewDidChangeNotification")]
		NSString DidChange { get; }

		[Notification]
		[Field ("WebViewDidEndEditingNotification")]
		NSString DidEndEditing { get; }

		[Notification]
		[Field ("WebViewDidChangeTypingStyleNotification")]
		NSString DidChangeTypingStyle { get; }

		[Notification]
		[Field ("WebViewDidChangeSelectionNotification")]
		NSString DidChangeSelection { get; }
	}

	delegate void WKWebsiteDataStoreFetchDataHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);
	delegate void WKWebsiteDataStoreRestoreDataHandler ([NullAllowed] NSError error);

	/// <summary>Data that is associated with a website, such as cookies and caches.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKWebsiteDataStore_Class_Ref/index.html">Apple documentation for <c>WKWebsiteDataStore</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSGenericException Reason: Calling [WKWebsiteDataStore init] is not supported.
	interface WKWebsiteDataStore : NSSecureCoding {

		[Static]
		[Export ("defaultDataStore")]
		WKWebsiteDataStore DefaultDataStore { get; }

		[Static]
		[Export ("nonPersistentDataStore")]
		WKWebsiteDataStore NonPersistentDataStore { get; }

		/// <summary>Gets a Boolean value that tells whether the store is persistent.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("persistent")]
		bool Persistent { [Bind ("isPersistent")] get; }

		[Static]
		[Export ("allWebsiteDataTypes")]
		NSSet<NSString> AllWebsiteDataTypes { get; }

		[Export ("fetchDataRecordsOfTypes:completionHandler:")]
		[Async (XmlDocs = """
			<param name="dataTypes">The data types for which to fetch website data.</param>
			<summary>Returns data records of the specified data types, and passes them to a handler when the operation completes.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous FetchDataRecordsOfTypes operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSArray&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void FetchDataRecordsOfTypes (NSSet<NSString> dataTypes, Action<NSArray> completionHandler);

		[Export ("removeDataOfTypes:forDataRecords:completionHandler:")]
		[Async (XmlDocs = """
			<param name="dataTypes">The types of data to remove.</param>
			<param name="dataRecords">The data records from which to delete data of the specified type.</param>
			<summary>Removes data of the specified type from the store, and passes the removed items to a completion handler.</summary>
			<returns>A task that represents the asynchronous RemoveDataOfTypes operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void RemoveDataOfTypes (NSSet<NSString> dataTypes, WKWebsiteDataRecord [] dataRecords, Action completionHandler);

		[Export ("removeDataOfTypes:modifiedSince:completionHandler:")]
		[Async (XmlDocs = """
			<param name="websiteDataTypes">The types of data to remove.</param>
			<param name="date">The date after which to remove all data of the specified type.</param>
			<summary>Removes data of the specified type from the store, and passes the removed items to a completion handler.</summary>
			<returns>A task that represents the asynchronous RemoveDataOfTypes operation</returns>
			<remarks>
			          <para copied="true">The RemoveDataOfTypesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void RemoveDataOfTypes (NSSet<NSString> websiteDataTypes, NSDate date, Action completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("httpCookieStore")]
		WKHttpCookieStore HttpCookieStore { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("identifier")]
		NSUuid Identifier { get; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("dataStoreForIdentifier:")]
		WKWebsiteDataStore Create (NSUuid identifier);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Async]
		[Export ("removeDataStoreForIdentifier:completionHandler:")]
		void Remove (NSUuid identifier, Action<NSError> completionHandler);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Async]
		[Export ("fetchAllDataStoreIdentifiers:")]
		void FetchAllDataStoreIdentifiers (Action<NSArray<NSUuid>> completionHandler);

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("proxyConfigurations", ArgumentSemantic.Copy), NullAllowed]
		NWProxyConfig [] ProxyConfigurations { get; set; }

		[Async]
		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("fetchDataOfTypes:completionHandler:")]
		void FetchData (NSSet<NSString> dataTypes, WKWebsiteDataStoreFetchDataHandler completionHandler);

		[Async]
		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("restoreData:completionHandler:")]
		void RestoreData (NSData data, WKWebsiteDataStoreRestoreDataHandler completionHandler);
	}

	[iOS (18, 4), NoTV]
	[MacCatalyst (18, 4)]
	[BaseType (typeof (NSObject))]
	interface WKOpenPanelParameters {
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; }

		[Export ("allowsDirectories")]
		bool AllowsDirectories { get; }
	}

#if XAMCORE_5_0
	delegate void WKUIDelegateRunJavaScriptTextInputPanelCallback ([NullAllowed] string result);
#endif

	/// <summary>A delegate object that allows presenting native UI elements on behalf of a Web page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKUIDelegate_Ref/index.html">Apple documentation for <c>WKUIDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKUIDelegate {

		/// <param name="webView">To be added.</param>
		/// <param name="configuration">To be added.</param>
		/// <param name="navigationAction">To be added.</param>
		/// <param name="windowFeatures">To be added.</param>
		/// <summary>Creates and configures a new <see cref="WebKit.WKWebView" />.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("webView:createWebViewWithConfiguration:forNavigationAction:windowFeatures:")]
		[return: NullAllowed]
		WKWebView CreateWebView (WKWebView webView, WKWebViewConfiguration configuration,
			WKNavigationAction navigationAction, WKWindowFeatures windowFeatures);

		/// <param name="webView">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Shows a JavaScript alert to the user.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptAlertPanel (WKWebView webView, string message, WKFrameInfo frame, Action completionHandler);

		/// <param name="webView">To be added.</param>
		/// <param name="message">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Shows a JavaScript confirmation dialog to the user.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptConfirmPanel (WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler);

#if !XAMCORE_5_0
		/// <param name="webView">To be added.</param>
		/// <param name="prompt">To be added.</param>
		/// <param name="defaultText">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="frame">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>Shows a JavaScript text input box to the user.</summary>
		/// <remarks>To be added.</remarks>
		[Obsolete ("It's not possible to call the completion handler with a null value using this method. Please see https://github.com/dotnet/macios/issues/15728 for a workaround.")]
		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
		void RunJavaScriptTextInputPanel (WKWebView webView, string prompt, [NullAllowed] string defaultText,
			WKFrameInfo frame, Action<string> completionHandler);
#endif

#if XAMCORE_5_0
		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
		void RunJavaScriptTextInputPanel (WKWebView webView, string prompt, [NullAllowed] string defaultText, WKFrameInfo frame, WKUIDelegateRunJavaScriptTextInputPanelCallback completionHandler);
#endif

		/// <param name="webView">To be added.</param>
		/// <param name="parameters">To be added.</param>
		/// <param name="frame">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[iOS (18, 4), NoTV]
		[MacCatalyst (18, 4)]
		[Export ("webView:runOpenPanelWithParameters:initiatedByFrame:completionHandler:")]
		void RunOpenPanel (WKWebView webView, WKOpenPanelParameters parameters, WKFrameInfo frame, Action<NSUrl []> completionHandler);

		/// <param name="webView">To be added.</param>
		/// <summary>Method that is called when <paramref name="webView" /> closes.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("webViewDidClose:")]
		void DidClose (WKWebView webView);

		/// <param name="webView">To be added.</param>
		/// <param name="elementInfo">To be added.</param>
		/// <summary>Method that is called to find out if the element should provide a preview.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Export ("webView:shouldPreviewElement:")]
		bool ShouldPreviewElement (WKWebView webView, WKPreviewElementInfo elementInfo);

		/// <param name="webView">To be added.</param>
		/// <param name="elementInfo">To be added.</param>
		/// <param name="previewActions">To be added.</param>
		/// <summary>Method that is called when the user peeks at content.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetContextMenuConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("webView:previewingViewControllerForElement:defaultActions:")]
		[return: NullAllowed]
		UIViewController GetPreviewingViewController (WKWebView webView, WKPreviewElementInfo elementInfo, IWKPreviewActionItem [] previewActions);

		/// <param name="webView">To be added.</param>
		/// <param name="previewingViewController">To be added.</param>
		/// <summary>Method that is called to respond when the user pops a preview action.</summary>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WillCommitContextMenu' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillCommitContextMenu' instead.")]
		[Export ("webView:commitPreviewingViewController:")]
		void CommitPreviewingViewController (WKWebView webView, UIViewController previewingViewController);

		[MacCatalyst (13, 1)]
		[NoMac]
		[Export ("webView:contextMenuConfigurationForElement:completionHandler:")]
		void SetContextMenuConfiguration (WKWebView webView, WKContextMenuElementInfo elementInfo, Action<UIContextMenuConfiguration> completionHandler);

		[MacCatalyst (13, 1)]
		[NoMac]
		[Export ("webView:contextMenuForElement:willCommitWithAnimator:")]
		void WillCommitContextMenu (WKWebView webView, WKContextMenuElementInfo elementInfo, IUIContextMenuInteractionCommitAnimating animator);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("webView:contextMenuWillPresentForElement:")]
		void ContextMenuWillPresent (WKWebView webView, WKContextMenuElementInfo elementInfo);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("webView:contextMenuDidEndForElement:")]
		void ContextMenuDidEnd (WKWebView webView, WKContextMenuElementInfo elementInfo);

		[Async]
		[NoMac, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("webView:requestDeviceOrientationAndMotionPermissionForOrigin:initiatedByFrame:decisionHandler:")]
		void RequestDeviceOrientationAndMotionPermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, Action<WKPermissionDecision> decisionHandler);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("webView:requestMediaCapturePermissionForOrigin:initiatedByFrame:type:decisionHandler:")]
		void RequestMediaCapturePermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, WKMediaCaptureType type, Action<WKPermissionDecision> decisionHandler);

		[Async]
		[NoMac, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("webView:showLockdownModeFirstUseMessage:completionHandler:")]
		void ShowLockDownMode (WKWebView webView, string firstUseMessage, Action<WKDialogResult> completionHandler);

		[NoMac, iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("webView:willPresentEditMenuWithAnimator:")]
		void WillPresentEditMenu (WKWebView webView, IUIEditMenuInteractionAnimating animator);

		[NoMac, iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("webView:willDismissEditMenuWithAnimator:")]
		void WillDismissEditMenu (WKWebView webView, IUIEditMenuInteractionAnimating animator);

		[NoMacCatalyst, iOS (26, 0), NoMac]
		[Export ("webView:insertInputSuggestion:")]
		void InsertInputSuggestion (WKWebView webView, UIInputSuggestion inputSuggestion);

		[Async]
		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("webView:requestGeolocationPermissionForOrigin:initiatedByFrame:decisionHandler:")]
		void RequestGeolocationPermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, WKUIDelegateRequestGeolocationPermissionCallback decisionHandler);
	}

	delegate void WKUIDelegateRequestGeolocationPermissionCallback (WKPermissionDecision decision);

	interface IWKUIDelegate { }

	/// <summary>Allows posting messages and injecting user scripts into a Web page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKUserContentController_Ref/index.html">Apple documentation for <c>WKUserContentController</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKUserContentController : NSSecureCoding {

		[Export ("userScripts")]
		WKUserScript [] UserScripts { get; }

		[Export ("addUserScript:")]
		void AddUserScript (WKUserScript userScript);

		[Export ("removeAllUserScripts")]
		void RemoveAllUserScripts ();

		[Export ("addScriptMessageHandler:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandler scriptMessageHandler, string name);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addScriptMessageHandler:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandler scriptMessageHandler, WKContentWorld world, string name);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addScriptMessageHandlerWithReply:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandlerWithReply scriptMessageHandlerWithReply, WKContentWorld contentWorld, string name);

		[Export ("removeScriptMessageHandlerForName:")]
		void RemoveScriptMessageHandler (string name);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeScriptMessageHandlerForName:contentWorld:")]
		void RemoveScriptMessageHandler (string name, WKContentWorld contentWorld);

		[MacCatalyst (13, 1)]
		[Export ("addContentRuleList:")]
		void AddContentRuleList (WKContentRuleList contentRuleList);

		[MacCatalyst (13, 1)]
		[Export ("removeContentRuleList:")]
		void RemoveContentRuleList (WKContentRuleList contentRuleList);

		[MacCatalyst (13, 1)]
		[Export ("removeAllContentRuleLists")]
		void RemoveAllContentRuleLists ();

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeAllScriptMessageHandlersFromContentWorld:")]
		void RemoveAllScriptMessageHandlers (WKContentWorld contentWorld);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeAllScriptMessageHandlers")]
		void RemoveAllScriptMessageHandlers ();

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("addBuffer:name:contentWorld:")]
		void AddBuffer (NSData buffer, string name, WKContentWorld world);

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("removeBufferWithName:contentWorld:")]
		void RemoveBuffer (string name, WKContentWorld world);
	}

	/// <summary>A script that can be injected into a Web page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKUserScript_Ref/index.html">Apple documentation for <c>WKUserScript</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // all properties are getters
	interface WKUserScript : NSCopying {

		[Export ("initWithSource:injectionTime:forMainFrameOnly:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithSource:injectionTime:forMainFrameOnly:inContentWorld:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly, WKContentWorld contentWorld);

		[Export ("source", ArgumentSemantic.Copy)]
		NSString Source { get; }

		[Export ("injectionTime")]
		WKUserScriptInjectionTime InjectionTime { get; }

		/// <summary>Gets a value that indicates whether the script is for the main frame only.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("forMainFrameOnly")]
		bool IsForMainFrameOnly { [Bind ("isForMainFrameOnly")] get; }
	}

	delegate void WKWebViewFetchDataHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);
	delegate void WKWebViewRestoreDataHandler ([NullAllowed] NSError error);

	/// <summary>Displays a Web page.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKWebView_Ref/index.html">Apple documentation for <c>WKWebView</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (
#if MONOMAC
		typeof (NSView)
#else
		typeof (UIView)
#endif
	)]
	[DisableDefaultCtor ()] // Crashes during deallocation in Xcode 6 beta 2. radar 17377712.
	interface WKWebView
#if MONOMAC
		: NSUserInterfaceValidations
#if XAMCORE_5_0
		, NSTextFinderClient
#endif
#endif
	{

		[DesignatedInitializer]
		[Export ("initWithFrame:configuration:")]
		NativeHandle Constructor (CGRect frame, WKWebViewConfiguration configuration);

		// (instancetype)initWithCoder:(NSCoder *)coder NS_UNAVAILABLE;
		// [Unavailable (PlatformName.iOS)]
		// [Unavailable (PlatformName.MacOSX)]
		// [Export ("initWithCoder:")]
		// NativeHandle Constructor (NSCoder coder);

		[Export ("configuration", ArgumentSemantic.Copy)]
		WKWebViewConfiguration Configuration { get; }

		[Export ("navigationDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakNavigationDelegate { get; set; }

		/// <summary>The web view's navigation delegate.</summary>
		///         <value>
		///           <para>This value can be <see langword="null" />.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>
		///           <para>
		///           </para>
		///         </remarks>
		[Wrap ("WeakNavigationDelegate")]
		IWKNavigationDelegate NavigationDelegate { get; set; }

		[Export ("UIDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakUIDelegate { get; set; }

		/// <summary>The web view's user interface delegate. </summary>
		///         <value>
		///           <para>This value can be <see langword="null" />.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>
		///           <para>
		///           </para>
		///         </remarks>
		[Wrap ("WeakUIDelegate")]
		IWKUIDelegate UIDelegate { get; set; }

		[Export ("backForwardList", ArgumentSemantic.Strong)]
		WKBackForwardList BackForwardList { get; }

		[Export ("title")]
		[NullAllowed]
		string Title { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSUrl Url { get; }

		/// <summary>A Boolean value indicating whether the view is currently loading content.</summary>
		///         <value>
		///           <para />
		///         </value>
		///         <remarks>
		///           <para />
		///         </remarks>
		[Export ("loading")]
		bool IsLoading { [Bind ("isLoading")] get; }

		[Export ("estimatedProgress")]
		double EstimatedProgress { get; }

		[Export ("hasOnlySecureContent")]
		bool HasOnlySecureContent { get; }

		[Export ("canGoBack")]
		bool CanGoBack { get; }

		[Export ("canGoForward")]
		bool CanGoForward { get; }

		[Export ("allowsBackForwardNavigationGestures")]
		bool AllowsBackForwardNavigationGestures { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("allowsMagnification")]
		bool AllowsMagnification { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("magnification")]
		nfloat Magnification { get; set; }

		[NoTV, NoiOS, NoMacCatalyst, Mac (27, 0)]
		[NullAllowed, Export ("refreshController", ArgumentSemantic.Strong)]
		NSRefreshController RefreshController { get; set; }

		[Export ("loadRequest:")]
		[return: NullAllowed]
		WKNavigation LoadRequest (NSUrlRequest request);

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("loadURL:")]
		[return: NullAllowed]
		WKNavigation LoadUrl (NSUrl url);

		[Export ("loadHTMLString:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadHtmlString (NSString htmlString, [NullAllowed] NSUrl baseUrl);

		/// <param name="htmlString">To be added.</param>
		///         <param name="baseUrl">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("LoadHtmlString ((NSString)htmlString, baseUrl)")]
		[return: NullAllowed]
		WKNavigation LoadHtmlString (string htmlString, NSUrl baseUrl);

		[Export ("goToBackForwardListItem:")]
		[return: NullAllowed]
		WKNavigation GoTo (WKBackForwardListItem item);

		[Export ("goBack")]
		[return: NullAllowed]
		WKNavigation GoBack ();

		[Export ("goForward")]
		[return: NullAllowed]
		WKNavigation GoForward ();

		[Export ("reload")]
		[return: NullAllowed]
		WKNavigation Reload ();

		[Export ("reloadFromOrigin")]
		[return: NullAllowed]
		WKNavigation ReloadFromOrigin ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Export ("evaluateJavaScript:completionHandler:")]
		[Async (XmlDocs = """
			<param name="javascript">The JavaScript string to evaluate</param>
			<summary>Evaluates the given JavaScript string.</summary>
			<returns>
			          <para>A task that represents the asynchronous EvaluateJavaScript operation.   The value of the TResult parameter is a <see cref="WebKit.WKJavascriptEvaluationResult" />.</para>
			        </returns>
			<remarks>
			          <para>This method will throw a <see cref="Foundation.NSErrorException" /> if the JavaScript is not evaluated successfully.</para>
			          <example>
			            <code lang="csharp lang-csharp"><![CDATA[
			var config = new WKWebViewConfiguration();
			var wk = new WKWebView(UIScreen.MainScreen.Bounds, config);
			var js = (NSString) "document.getElementById('foo').innerHTML = 'bar'";
			var result = await wk.EvaluateJavaScriptAsync(js); //== "bar"
			    ]]></code>
			          </example>
			          <para>The EvaluateJavaScriptAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para>The arguments to the handler are an <see cref="Foundation.NSObject" /> containing the results of the evaluation and an <see cref="Foundation.NSError" /> if an error. If an error occurred, the <c>result</c> argument will be <see langword="null" />. If no error occurred, the <c>error</c> argument will be <see langword="null" />.</para>
			          <example>
			            <code lang="csharp lang-csharp"><![CDATA[
			var config = new WKWebViewConfiguration();
			var wk = new WKWebView(UIScreen.MainScreen.Bounds, config);
			var js = (NSString) "document.getElementById('foo').innerHTML = 'bar'";
			WKJavascriptEvaluationResult handler = (NSObject result, NSError err) => { 
			  if(err is not null)
			  {
			    System.Console.WriteLine(err);
			  }
			  if(result is not null)
			  {
			     System.Console.WriteLine(result);
			  }
			};
			wk.EvaluateJavaScript(js, handler);
			    ]]></code>
			          </example>
			        </remarks>
			""")]
		void EvaluateJavaScript (NSString javascript, [NullAllowed] WKJavascriptEvaluationResult completionHandler);

		/// <include file="../docs/api/WebKit/WKWebView.xml" path="/Documentation/Docs[@DocId='M:WebKit.WKWebView.EvaluateJavaScript(System.String,WebKit.WKJavascriptEvaluationResult)']/*" />
		[Wrap ("EvaluateJavaScript ((NSString)javascript, completionHandler)")]
		[Async (XmlDocs = """
			<param name="javascript">A well-formed JavaScript expression.</param>
			<summary>Evaluates the given JavaScript string.</summary>
			<returns>A task that represents the asynchronous EvaluateJavaScript operation. The TResult holds the results of the evaluation.</returns>
			<remarks>
			          <para>This method will throw a <see cref="Foundation.NSErrorException" /> if the JavaScript is not evaluated successfully.</para>
			          <example>
			            <code lang="csharp lang-csharp"><![CDATA[
			var config = new WKWebViewConfiguration();
			var wk = new WKWebView(UIScreen.MainScreen.Bounds, config);
			var js = (NSString) "document.getElementById('foo').innerHTML = 'bar'";
			var result = await wk.EvaluateJavaScriptAsync(js); //== "bar"
			    ]]></code>
			          </example>
			        </remarks>
			""")]
		void EvaluateJavaScript (string javascript, WKJavascriptEvaluationResult completionHandler);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("setMagnification:centeredAtPoint:")]
		void SetMagnification (nfloat magnification, CGPoint centerPoint);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("scrollView", ArgumentSemantic.Strong)]
		UIScrollView ScrollView { get; }

		[MacCatalyst (13, 1)]
		[Export ("loadData:MIMEType:characterEncodingName:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadData (NSData data, string mimeType, string characterEncodingName, NSUrl baseUrl);

		[MacCatalyst (13, 1)]
		[Export ("loadFileURL:allowingReadAccessToURL:")]
		[return: NullAllowed]
		WKNavigation LoadFileUrl (NSUrl url, NSUrl readAccessUrl);

		[MacCatalyst (13, 1)]
		[Export ("customUserAgent")]
		[NullAllowed]
		string CustomUserAgent { get; set; }

		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'ServerTrust' property.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'ServerTrust' property.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ServerTrust' property.")]
		[Export ("certificateChain", ArgumentSemantic.Copy)]
		SecCertificate [] CertificateChain { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsLinkPreview")]
		bool AllowsLinkPreview { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("serverTrust")]
		SecTrust ServerTrust { get; }

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="snapshotConfiguration">The snapshot configuration to use.This parameter can be .</param>
			<summary>Asynchronously takes a snapshot of the current viewport.</summary>
			<returns>
			          <para>The result is of type System.Tasks.Task&lt;AppKit.NSImage&gt; on MacOS and System.Tasks.Task&lt;UIKit.UIImage&gt; on iOS.</para>
			        </returns>
			<remarks>
			          <para copied="true">The TakeSnapshotAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<UIImage, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("handlesURLScheme:")]
		bool HandlesUrlScheme (string urlScheme);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("evaluateJavaScript:inFrame:inContentWorld:completionHandler:")]
		void EvaluateJavaScript (string javaScriptString, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("callAsyncJavaScript:arguments:inFrame:inContentWorld:completionHandler:")]
		void CallAsyncJavaScript (string functionBody, [NullAllowed] NSDictionary<NSString, NSObject> arguments, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("createPDFWithConfiguration:completionHandler:")]
		void CreatePdf ([NullAllowed] WKPdfConfiguration pdfConfiguration, Action<NSData, NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("createWebArchiveDataWithCompletionHandler:")]
		void CreateWebArchive (Action<NSData, NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("findString:withConfiguration:completionHandler:")]
		void Find (string @string, [NullAllowed] WKFindConfiguration configuration, Action<WKFindResult> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("mediaType")]
		string MediaType { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("pageZoom")]
		nfloat PageZoom { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("printOperationWithPrintInfo:")]
		NSPrintOperation GetPrintOperation (NSPrintInfo printInfo);

		// Apple renamed those API since Xcode 12.5
		[Internal]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("closeAllMediaPresentations")]
		void _OldCloseAllMediaPresentations ();

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("closeAllMediaPresentationsWithCompletionHandler:")]
		void CloseAllMediaPresentations ([NullAllowed] Action completionHandler);

		[Internal]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("pauseAllMediaPlayback:")]
		void _OldPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("pauseAllMediaPlaybackWithCompletionHandler:")]
		void _NewPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("suspendAllMediaPlayback:")]
		void _OldSuspendAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("resumeAllMediaPlayback:")]
		void _OldResumeAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setAllMediaPlaybackSuspended:completionHandler:")]
		void SetAllMediaPlaybackSuspended (bool suspended, [NullAllowed] Action completionHandler);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("requestMediaPlaybackStateWithCompletionHandler:")]
		void RequestMediaPlaybackState (Action<WKMediaPlaybackState> completionHandler);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("startDownloadUsingRequest:completionHandler:")]
		void StartDownload (NSUrlRequest request, Action<WKDownload> completionHandler);

		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("resumeDownloadFromResumeData:completionHandler:")]
		void ResumeDownload (NSData resumeData, Action<WKDownload> completionHandler);

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("cameraCaptureState")]
		WKMediaCaptureState CameraCaptureState { get; }

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[NullAllowed, Export ("interactionState", ArgumentSemantic.Copy)]
		NSObject InteractionState { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadFileRequest:allowingReadAccessToURL:")]
		WKNavigation LoadFileRequest (NSUrlRequest request, NSUrl readAccessURL);

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadSimulatedRequest:response:responseData:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, NSUrlResponse response, NSData data);

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadSimulatedRequest:responseHTMLString:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, string htmlString);

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("microphoneCaptureState")]
		WKMediaCaptureState MicrophoneCaptureState { get; }

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setCameraCaptureState:completionHandler:")]
		void SetCameraCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);

		[Async]
		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setMicrophoneCaptureState:completionHandler:")]
		void SetMicrophoneCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("themeColor")]
		[NullAllowed]
		UIColor ThemeColor { get; }

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[NullAllowed, Export ("underPageBackgroundColor", ArgumentSemantic.Copy)]
		UIColor UnderPageBackgroundColor { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("fullscreenState")]
		WKFullscreenState FullscreenState { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("minimumViewportInset")]
		UIEdgeInsets MinimumViewportInset { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("maximumViewportInset")]
		UIEdgeInsets MaximumViewportInset { get; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("setMinimumViewportInset:maximumViewportInset:")]
		void SetViewportInsets (UIEdgeInsets minimumViewportInset, UIEdgeInsets maximumViewportInset);

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoTV]
		[Export ("findInteractionEnabled")]
		bool FindInteractionEnabled { [Bind ("isFindInteractionEnabled")] get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoTV]
		[Export ("findInteraction")]
		[NullAllowed]
		UIFindInteraction FindInteraction { get; }

		[MacCatalyst (16, 4), iOS (16, 4), NoTV]
		[Export ("inspectable")]
		bool Inspectable { [Bind ("isInspectable")] get; set; }

		[NoiOS, NoMacCatalyst]
		[Export ("goBack:")]
		void GoBack ([NullAllowed] NSObject sender);

		[NoiOS, NoMacCatalyst]
		[Export ("goForward:")]
		void GoForward ([NullAllowed] NSObject sender);

		[NoiOS, NoMacCatalyst]
		[Export ("reload:")]
		void Reload ([NullAllowed] NSObject sender);

		[NoiOS, NoMacCatalyst]
		[Export ("reloadFromOrigin:")]
		void ReloadFromOrigin ([NullAllowed] NSObject sender);

		[NoiOS, NoMacCatalyst]
		[Export ("stopLoading:")]
		void StopLoading ([NullAllowed] NSObject sender);

		[Mac (15, 0), iOS (18, 2), MacCatalyst (18, 0)]
		[Export ("writingToolsActive")]
		bool WritingToolsActive { [Bind ("isWritingToolsActive")] get; }

		[NoMacCatalyst, iOS (26, 0), NoMac]
		[Export ("conversationContext", ArgumentSemantic.Strong)]
		UIConversationContext ConversationContext { get; set; }

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Async]
		[Export ("fetchDataOfTypes:completionHandler:")]
		void FetchData (WKWebViewDataType dataTypes, WKWebViewFetchDataHandler completionHandler);

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Async]
		[Export ("restoreData:completionHandler:")]
		void RestoreData (NSData data, WKWebViewRestoreDataHandler completionHandler);

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("obscuredContentInsets", ArgumentSemantic.Assign)]
		UIEdgeInsets ObscuredContentInsets { get; set; }

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("isBlockedByScreenTime")]
		bool IsBlockedByScreenTime { get; }
	}

	/// <param name="result">The result of a successful evaluation. <see langword="null" /> if error occurred.</param>
	///     <param name="error">The exception that occurred. <see langword="null" /> if evaluation succeeded.</param>
	///     <summary>The result of evaluating JavaScript code.</summary>
	///     <remarks>
	///       <para>If evaluation was successful, <paramref name="error" /> will be <see langword="null" />. If an error occurred, <paramref name="result" /> will be <see langword="null" />.</para>
	///     </remarks>
	delegate void WKJavascriptEvaluationResult ([NullAllowed] NSObject result, [NullAllowed] NSError error);

	/// <summary>Properties configuring a <see cref="WebKit.WKWebView" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKWebViewConfiguration_Ref/index.html">Apple documentation for <c>WKWebViewConfiguration</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebViewConfiguration : NSCopying, NSSecureCoding {

		[Export ("processPool", ArgumentSemantic.Retain)]
		[Deprecated (PlatformName.iOS, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
		[Deprecated (PlatformName.MacOSX, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
		WKProcessPool ProcessPool { get; set; }

		[Export ("preferences", ArgumentSemantic.Retain)]
		WKPreferences Preferences { get; set; }

		[Export ("userContentController", ArgumentSemantic.Retain)]
		WKUserContentController UserContentController { get; set; }

		[Export ("suppressesIncrementalRendering")]
		bool SuppressesIncrementalRendering { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("websiteDataStore", ArgumentSemantic.Strong)]
		WKWebsiteDataStore WebsiteDataStore { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("applicationNameForUserAgent")]
		[NullAllowed]
		string ApplicationNameForUserAgent { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsAirPlayForMediaPlayback")]
		bool AllowsAirPlayForMediaPlayback { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsInlineMediaPlayback")]
		bool AllowsInlineMediaPlayback { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'RequiresUserActionForMediaPlayback' or 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'RequiresUserActionForMediaPlayback' or 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("mediaPlaybackRequiresUserAction")]
		bool MediaPlaybackRequiresUserAction { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'AllowsAirPlayForMediaPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AllowsAirPlayForMediaPlayback' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("mediaPlaybackAllowsAirPlay")]
		bool MediaPlaybackAllowsAirPlay { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 26, 0, message: "No longer supported.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, message: "No longer supported.")]
		[Export ("selectionGranularity")]
		WKSelectionGranularity SelectionGranularity { get; set; }

		[NoMac]
		[Deprecated (PlatformName.iOS, 10, 0, message: "Use 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'MediaTypesRequiringUserActionForPlayback' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("requiresUserActionForMediaPlayback")]
		bool RequiresUserActionForMediaPlayback { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("allowsPictureInPictureMediaPlayback")]
		bool AllowsPictureInPictureMediaPlayback { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("dataDetectorTypes", ArgumentSemantic.Assign)]
		WKDataDetectorTypes DataDetectorTypes { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("mediaTypesRequiringUserActionForPlayback", ArgumentSemantic.Assign)]
		WKAudiovisualMediaTypes MediaTypesRequiringUserActionForPlayback { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("ignoresViewportScaleLimits")]
		bool IgnoresViewportScaleLimits { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("setURLSchemeHandler:forURLScheme:")]
		void SetUrlSchemeHandler ([NullAllowed] IWKUrlSchemeHandler urlSchemeHandler, string urlScheme);

		[MacCatalyst (13, 1)]
		[Export ("urlSchemeHandlerForURLScheme:")]
		[return: NullAllowed]
		IWKUrlSchemeHandler GetUrlSchemeHandler (string urlScheme);

		[MacCatalyst (13, 1)]
		[Export ("defaultWebpagePreferences", ArgumentSemantic.Copy)]
		[NullAllowed]
		WKWebpagePreferences DefaultWebpagePreferences { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("limitsNavigationsToAppBoundDomains")]
		bool LimitsNavigationsToAppBoundDomains { get; set; }

		[iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("upgradeKnownHostsToHTTPS")]
		bool UpgradeKnownHostsToHttps { get; set; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("allowsInlinePredictions")]
		bool AllowsInlinePredictions { get; set; }

		[NoiOS, NoMacCatalyst]
		[Export ("userInterfaceDirectionPolicy", ArgumentSemantic.Assign)]
		WKUserInterfaceDirectionPolicy UserInterfaceDirectionPolicy { get; set; }

		[Mac (15, 0), iOS (18, 2), MacCatalyst (18, 0)]
		[Export ("supportsAdaptiveImageGlyph")]
		bool SupportsAdaptiveImageGlyph { get; set; }

		[Mac (15, 0), iOS (18, 2), MacCatalyst (18, 0)]
		[Export ("writingToolsBehavior")]
#if MONOMAC
		NSWritingToolsBehavior WritingToolsBehavior { get; set; }
#else
		UIWritingToolsBehavior WritingToolsBehavior { get; set; }
#endif

		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4), NoTV]
		[Export ("webExtensionController", ArgumentSemantic.Strong), NullAllowed]
		WKWebExtensionController WebExtensionController { get; set; }

		[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("showsSystemScreenTimeBlockingView")]
		bool ShowsSystemScreenTimeBlockingView { get; set; }
	}

	/// <summary>A pool of content processes.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKProcessPool_Ref/index.html">Apple documentation for <c>WKProcessPool</c></related>
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.iOS, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
	[Deprecated (PlatformName.MacOSX, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
	[Deprecated (PlatformName.MacCatalyst, 26, 0, message: "Use 'WKWebsiteDataStore' instead. Creating multiple instances of 'WKProcessPool' no longer works.")]
	[BaseType (typeof (NSObject))]
	interface WKProcessPool : NSSecureCoding {
		// as of Mac 10.10, iOS 8.0 Beta 2,
		// this interface is completely empty
	}

	/// <summary>WKWindowFeatures specifies optional attributes for the containing window when a new WKWebView is requested.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/WebKit/Reference/WKWindowFeatures_Ref/index.html">Apple documentation for <c>WKWindowFeatures</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWindowFeatures {
		// Filled in from open source headers

		[Internal, Export ("menuBarVisibility")]
		[NullAllowed]
		NSNumber menuBarVisibility { get; }

		[Internal, Export ("statusBarVisibility")]
		[NullAllowed]
		NSNumber statusBarVisibility { get; }

		[Internal, Export ("toolbarsVisibility")]
		[NullAllowed]
		NSNumber toolbarsVisibility { get; }

		[Internal, Export ("allowsResizing")]
		[NullAllowed]
		NSNumber allowsResizing { get; }

		[Internal, Export ("x")]
		[NullAllowed]
		NSNumber x { get; }

		[Internal, Export ("y")]
		[NullAllowed]
		NSNumber y { get; }

		[Internal, Export ("width")]
		[NullAllowed]
		NSNumber width { get; }

		[Internal, Export ("height")]
		[NullAllowed]
		NSNumber height { get; }
	}

#if MONOMAC
	interface UIPreviewActionItem { }
#endif

	interface IWKPreviewActionItem { }

	/// <summary>Interface that provides access to the properties of the preview action item for a web view.</summary>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TBD' instead.")]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface WKPreviewActionItem : UIPreviewActionItem {
		/// <summary>Gets the unique identifier of the preview action type.</summary>
		/// <value>The unique identifier of the preview action type.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSString Identifier { get; }
	}

	/// <summary>Contains preview action type identifiers.</summary>
	[NoMac]
	[Static]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TBD' instead.")]
	interface WKPreviewActionItemIdentifier {
		/// <summary>Gets the string that identifies the action that opens the item.</summary>
		///         <value>The string that identifies the action that opens the item.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKPreviewActionItemIdentifierOpen")]
		NSString Open { get; }

		/// <summary>Gets the string that identifies the action that adds the item to the user's reading list.</summary>
		///         <value>The string that identifies the action that adds the item to the user's reading list.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKPreviewActionItemIdentifierAddToReadingList")]
		NSString AddToReadingList { get; }

		/// <summary>Gets the string that identifies the action that copies the item.</summary>
		///         <value>The string that identifies the action that copies the item.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKPreviewActionItemIdentifierCopy")]
		NSString Copy { get; }

		/// <summary>Gets the string that identifies the action that shares the item.</summary>
		///         <value>The string that identifies the action that shares the item.</value>
		///         <remarks>To be added.</remarks>
		[Field ("WKPreviewActionItemIdentifierShare")]
		NSString Share { get; }
	}

	/// <summary>Contains the URL for a preview action item.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/WebKit/WKPreviewElementInfo">Apple documentation for <c>WKPreviewElementInfo</c></related>
	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WKContextMenuElementInfo' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WKContextMenuElementInfo' instead.")]
	[BaseType (typeof (NSObject))]
	interface WKPreviewElementInfo : NSCopying {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	public enum WKContentMode : long {
		Recommended,
		Mobile,
		Desktop,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebpagePreferences {

		[Export ("preferredContentMode", ArgumentSemantic.Assign)]
		WKContentMode PreferredContentMode { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowsContentJavaScript")]
		bool AllowsContentJavaScript { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoTV]
		[Export ("lockdownModeEnabled")]
		bool LockdownModeEnabled { [Bind ("isLockdownModeEnabled")] get; set; }

		[Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
		[Export ("preferredHTTPSNavigationPolicy", ArgumentSemantic.Assign)]
		WKWebpagePreferencesUpgradeToHttpsPolicy PreferredHttpsNavigationPolicy { get; set; }

		[NoTV, Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
		[Export ("securityRestrictionMode", ArgumentSemantic.Assign)]
		WKSecurityRestrictionMode SecurityRestrictionMode { get; set; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("alternateRequest", ArgumentSemantic.Copy)]
		NSUrlRequest AlternateRequest { get; set; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[NullAllowed, Export ("overrideReferrer", ArgumentSemantic.Copy)]
		string OverrideReferrer { get; set; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("allowsJSHandleCreationInPageWorld")]
		bool AllowsJSHandleCreationInPageWorld { get; set; }

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("globalPrivacyControlEnabled")]
		bool GlobalPrivacyControlEnabled { get; set; }
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKContextMenuElementInfo {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKContentWorld {

		[Static]
		[Export ("pageWorld")]
		WKContentWorld Page { get; }

		[Static]
		[Export ("defaultClientWorld")]
		WKContentWorld DefaultClient { get; }

		[Static]
		[Export ("worldWithName:")]
		WKContentWorld Create (string name);

		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Static]
		[Export ("worldWithConfiguration:")]
		WKContentWorld Create (WKContentWorldConfiguration configuration);

		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	interface WKContentWorldConfiguration : NSCopying, NSSecureCoding {

		[Export ("allowAccessingClosedShadowRoots")]
		bool AllowAccessingClosedShadowRoots { get; set; }

		[Export ("autofillScriptingEnabled")]
		bool AutofillScriptingEnabled { [Bind ("isAutofillScriptingEnabled")] get; set; }

		[Export ("elementUserInfoEnabled")]
		bool ElementUserInfoEnabled { [Bind ("isElementUserInfoEnabled")] get; set; }

		[Export ("legacyBuiltinOverridesEnabled")]
		bool LegacyBuiltinOverridesEnabled { [Bind ("isLegacyBuiltinOverridesEnabled")] get; set; }

		[Export ("nodeSnapshotCreationEnabled")]
		bool NodeSnapshotCreationEnabled { [Bind ("isNodeSnapshotCreationEnabled")] get; set; }

		[Export ("jsHandleCreationEnabled")]
		bool JSHandleCreationEnabled { [Bind ("isJSHandleCreationEnabled")] get; [Bind ("setJSHandleCreationEnabled:")] set; }

		[Export ("inspectable")]
		bool Inspectable { [Bind ("isInspectable")] get; set; }
	}

	delegate void WKJSHandleGetWindowProxyFrameCallback ([NullAllowed] WKFrameInfo frameInfo);

	[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKJSHandle : NSCopying {

		[Export ("sourceFrame", ArgumentSemantic.Copy)]
		WKFrameInfo SourceFrame { get; }

		[NullAllowed, Export ("contentWorld", ArgumentSemantic.Weak)]
		WKContentWorld ContentWorld { get; }

		[Async]
		[Export ("getWindowProxyFrameWithCompletionHandler:")]
		void GetWindowProxyFrame (WKJSHandleGetWindowProxyFrameCallback completionHandler);
	}

	[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject), Name = "WKDOMNodeSnapshot")]
	[DisableDefaultCtor]
	interface WKDomNodeSnapshot {
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	interface WKFindConfiguration : NSCopying {

		[Export ("backwards")]
		bool Backwards { get; set; }

		[Export ("caseSensitive")]
		bool CaseSensitive { get; set; }

		[Export ("wraps")]
		bool Wraps { get; set; }
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKFindResult : NSCopying {

		[Export ("matchFound")]
		bool MatchFound { get; }
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "WKPDFConfiguration")]
	interface WKPdfConfiguration : NSCopying {

		[Export ("rect", ArgumentSemantic.Assign)]
		CGRect Rect { get; set; }

		[iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("allowTransparentBackground")]
		bool AllowTransparentBackground { get; set; }
	}

	interface IWKScriptMessageHandlerWithReply { }

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface WKScriptMessageHandlerWithReply {

		[Abstract]
		[Export ("userContentController:didReceiveScriptMessage:replyHandler:")]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message, Action<NSObject, NSString> replyHandler);
	}

	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	enum WKDownloadRedirectPolicy : long {
		Cancel,
		Allow,
	}

	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	enum WKMediaPlaybackState : ulong {
		None,
		Paused,
		Suspended,
		Playing,
	}

	interface IWKDownloadDelegate { }

	delegate void WKDownloadDelegateDecidePlaceholderPolicyCallback (WKDownloadPlaceholderPolicy policy, [NullAllowed] NSUrl url);

	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKDownloadDelegate {

		[Abstract]
		[Export ("download:decideDestinationUsingResponse:suggestedFilename:completionHandler:")]
		void DecideDestination (WKDownload download, NSUrlResponse response, string suggestedFilename, Action<NSUrl> completionHandler);

		[Export ("download:willPerformHTTPRedirection:newRequest:decisionHandler:")]
		void WillPerformHttpRedirection (WKDownload download, NSHttpUrlResponse response, NSUrlRequest request, Action<WKDownloadRedirectPolicy> decisionHandler);

		[Export ("download:didReceiveAuthenticationChallenge:completionHandler:")]
		void DidReceiveAuthenticationChallenge (WKDownload download, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Export ("downloadDidFinish:")]
		void DidFinish (WKDownload download);

		[Export ("download:didFailWithError:resumeData:")]
		void DidFail (WKDownload download, NSError error, [NullAllowed] NSData resumeData);

		[iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("download:decidePlaceholderPolicy:")]
		void DecidePlaceholderPolicy (WKDownload download, WKDownloadDelegateDecidePlaceholderPolicyCallback completionHandler);

		[iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("download:didReceivePlaceholderURL:completionHandler:")]
		void DidReceivePlaceholderUrl (WKDownload download, NSUrl url, Action completionHandler);

		[iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("download:didReceiveFinalURL:")]
		void DidReceiveFinalUrl (WKDownload download, NSUrl url);
	}

	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKDownload : NSProgressReporting {

		[NullAllowed, Export ("originalRequest")]
		NSUrlRequest OriginalRequest { get; }

		[NullAllowed, Export ("webView", ArgumentSemantic.Weak)]
		WKWebView WebView { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKDownloadDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Export ("cancel:")]
		void Cancel ([NullAllowed] Action<NSData> completionHandler);

		[Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
		[Export ("userInitiated")]
		bool UserInitiated { [Bind ("isUserInitiated")] get; }

		[Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
		[Export ("originatingFrame")]
		WKFrameInfo OriginatingFrame { get; }

	}

	[iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
	[Native]
	public enum WKDownloadPlaceholderPolicy : long {
		Disable,
		Enable,
	}

	[Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[Native ("WKWebpagePreferencesUpgradeToHTTPSPolicy")]
	public enum WKWebpagePreferencesUpgradeToHttpsPolicy : long {
		KeepAsRequested,
		AutomaticFallbackToHttp,
		UserMediatedFallbackToHttp,
		ErrorOnFailure,
	}

	/// <summary>Security restriction modes for WebView content.</summary>
	[NoTV, Mac (26, 4), iOS (26, 4), MacCatalyst (26, 4)]
	[Native]
	public enum WKSecurityRestrictionMode : long {
		/// <summary>No additional security restrictions beyond WebKit defaults.</summary>
		None,
		/// <summary>Enhanced security protections optimized for maintaining web compatibility. Disables JIT compilation and enables increased MTE adoption.</summary>
		MaximizeCompatibility,
		/// <summary>Maximum security restrictions including feature disablement. Applied automatically by the system in Lockdown Mode.</summary>
		Lockdown,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	[ErrorDomain ("WKWebExtensionErrorDomain")]
	public enum WKWebExtensionError : long {
		Unknown = 1,
		ResourceNotFound,
		InvalidResourceCodeSignature,
		InvalidManifest,
		UnsupportedManifestVersion,
		InvalidManifestEntry,
		InvalidDeclarativeNetRequestEntry,
		InvalidBackgroundPersistence,
		InvalidArchive,
	}

	delegate void WKWebExtensionCreateCallback ([NullAllowed] WKWebExtension extension, [NullAllowed] NSError error);

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtension {
		[Static]
		[Export ("extensionWithAppExtensionBundle:completionHandler:")]
		[Async]
		void Create (NSBundle appExtensionBundle, WKWebExtensionCreateCallback completionHandler);

		[Static]
		[Export ("extensionWithResourceBaseURL:completionHandler:")]
		[Async]
		void Create (NSUrl resourceBaseUrl, WKWebExtensionCreateCallback completionHandler);

		[Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; }

		[Export ("manifest", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSObject> Manifest { get; }

		[Export ("manifestVersion")]
		double ManifestVersion { get; }

		[Export ("supportsManifestVersion:")]
		bool SupportsManifestVersion (double manifestVersion);

		[NullAllowed, Export ("defaultLocale", ArgumentSemantic.Copy)]
		NSLocale DefaultLocale { get; }

		[NullAllowed, Export ("displayName")]
		string DisplayName { get; }

		[NullAllowed, Export ("displayShortName")]
		string DisplayShortName { get; }

		[NullAllowed, Export ("displayVersion")]
		string DisplayVersion { get; }

		[NullAllowed, Export ("displayDescription")]
		string DisplayDescription { get; }

		[NullAllowed, Export ("displayActionLabel")]
		string DisplayActionLabel { get; }

		[NullAllowed, Export ("version")]
		string Version { get; }

		[Export ("iconForSize:")]
		[return: NullAllowed]
		UIImage GetIcon (CGSize size);

		[Export ("actionIconForSize:")]
		[return: NullAllowed]
		UIImage GetActionIcon (CGSize size);

		[Export ("requestedPermissions", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakRequestedPermissions { get; }

		WKWebExtensionPermission RequestedPermissions {
			[Wrap ("WKWebExtensionPermissionExtensions.ToFlags (WeakRequestedPermissions);")]
			get;
		}

		[Export ("optionalPermissions", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakOptionalPermissions { get; }

		WKWebExtensionPermission OptionalPermissions {
			[Wrap ("WKWebExtensionPermissionExtensions.ToFlags (WeakOptionalPermissions);")]
			get;
		}

		[Export ("requestedPermissionMatchPatterns", ArgumentSemantic.Copy)]
		NSSet<WKWebExtensionMatchPattern> RequestedPermissionMatchPatterns { get; }

		[Export ("optionalPermissionMatchPatterns", ArgumentSemantic.Copy)]
		NSSet<WKWebExtensionMatchPattern> OptionalPermissionMatchPatterns { get; }

		[Export ("allRequestedMatchPatterns", ArgumentSemantic.Copy)]
		NSSet<WKWebExtensionMatchPattern> AllRequestedMatchPatterns { get; }

		[Export ("hasBackgroundContent")]
		bool HasBackgroundContent { get; }

		[Export ("hasPersistentBackgroundContent")]
		bool HasPersistentBackgroundContent { get; }

		[Export ("hasInjectedContent")]
		bool HasInjectedContent { get; }

		[Export ("hasOptionsPage")]
		bool HasOptionsPage { get; }

		[Export ("hasOverrideNewTabPage")]
		bool HasOverrideNewTabPage { get; }

		[Export ("hasCommands")]
		bool HasCommands { get; }

		[Export ("hasContentModificationRules")]
		bool HasContentModificationRules { get; }
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionAction {
		[NullAllowed, Export ("webExtensionContext", ArgumentSemantic.Weak)]
		WKWebExtensionContext WebExtensionContext { get; }

		[NullAllowed, Export ("associatedTab", ArgumentSemantic.Weak)]
		IWKWebExtensionTab AssociatedTab { get; }

		[Export ("iconForSize:")]
		[return: NullAllowed]
		UIImage GetIcon (CGSize size);

		[Export ("label")]
		string Label { get; }

		[Export ("badgeText")]
		string BadgeText { get; }

		[Export ("hasUnreadBadgeText")]
		bool HasUnreadBadgeText { get; set; }

		[NullAllowed, Export ("inspectionName")]
		string InspectionName { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("menuItems", ArgumentSemantic.Copy)]
#if IOS || MACCATALYST
		UIMenuElement [] MenuItems { get; }
#else
		NSMenuItem [] MenuItems { get; }
#endif

		[Export ("presentsPopup")]
		bool PresentsPopup { get; }

#if !MONOMAC
		[NullAllowed, Export ("popupViewController")]
		UIViewController PopupViewController { get; }
#endif

#if MONOMAC
		[Export ("popupPopover"), NullAllowed]
		NSPopover PopupPopover { get; }
#endif

		[NullAllowed, Export ("popupWebView")]
		WKWebView PopupWebView { get; }

		[Export ("closePopup")]
		void ClosePopup ();
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionCommand {
		[NullAllowed, Export ("webExtensionContext", ArgumentSemantic.Weak)]
		WKWebExtensionContext WebExtensionContext { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("title")]
		string Title { get; }

		[NullAllowed, Export ("activationKey")]
		string ActivationKey { get; set; }

		[Export ("modifierFlags", ArgumentSemantic.Assign)]
#if __IOS__ || __MACCATALYST_
		UIKeyModifierFlags ModifierFlags { get; set; }
#else
		NSEventModifierMask ModifierFlags { get; set; }
#endif

		[Export ("menuItem", ArgumentSemantic.Copy)]
#if __IOS__ || __MACCATALYST_
		UIMenuElement MenuItem { get; }
#else
		NSMenuItem MenuItem { get; }
#endif

#if IOS || MACCATALYST
		[NullAllowed, Export ("keyCommand", ArgumentSemantic.Copy)]
		UIKeyCommand KeyCommand { get; }
#endif
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Flags]
	[Native]
	public enum WKWebExtensionTabChangedProperties : ulong {
		None = 0,
		Loading = 1uL << 1,
		Muted = 1uL << 2,
		Pinned = 1uL << 3,
		PlayingAudio = 1uL << 4,
		ReaderMode = 1uL << 5,
		Size = 1uL << 6,
		Title = 1uL << 7,
		Url = 1uL << 8,
		ZoomFactor = 1uL << 9,
	}

	interface IWKWebExtensionTab { }

	delegate void WKWebExtensionTabCallback ([NullAllowed] NSError error);
	delegate void WKWebExtensionTabDetectLocaleCallback ([NullAllowed] NSLocale locale, [NullAllowed] NSError error);
	delegate void WKWebExtensionTabDuplicateCallback ([NullAllowed] IWKWebExtensionTab duplicatedTab, [NullAllowed] NSError error);
	delegate void WKWebExtensionTabTakeSnapshotCallback ([NullAllowed] UIImage webpageImage, [NullAllowed] NSError error);

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface WKWebExtensionTab {
		[Export ("windowForWebExtensionContext:")]
		[return: NullAllowed]
		IWKWebExtensionWindow GetWindow (WKWebExtensionContext context);

		[Export ("indexInWindowForWebExtensionContext:")]
		nuint GetIndexInWindow (WKWebExtensionContext context);

		[Export ("parentTabForWebExtensionContext:")]
		[return: NullAllowed]
		IWKWebExtensionTab GetParentTab (WKWebExtensionContext context);

		[Async]
		[Export ("setParentTab:forWebExtensionContext:completionHandler:")]
		void SetParentTab ([NullAllowed] IWKWebExtensionTab parentTab, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("webViewForWebExtensionContext:")]
		[return: NullAllowed]
		WKWebView GetWebView (WKWebExtensionContext context);

		[Export ("titleForWebExtensionContext:")]
		[return: NullAllowed]
		string GetTitle (WKWebExtensionContext context);

		[Export ("isPinnedForWebExtensionContext:")]
		bool IsPinned (WKWebExtensionContext context);

		[Async]
		[Export ("setPinned:forWebExtensionContext:completionHandler:")]
		void SetPinned (bool pinned, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("isReaderModeAvailableForWebExtensionContext:")]
		bool IsReaderModeAvailable (WKWebExtensionContext context);

		[Export ("isReaderModeActiveForWebExtensionContext:")]
		bool IsReaderModeActive (WKWebExtensionContext context);

		[Async]
		[Export ("setReaderModeActive:forWebExtensionContext:completionHandler:")]
		void SetReaderModeActive (bool active, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("isPlayingAudioForWebExtensionContext:")]
		bool IsPlayingAudio (WKWebExtensionContext context);

		[Export ("isMutedForWebExtensionContext:")]
		bool IsMuted (WKWebExtensionContext context);

		[Async]
		[Export ("setMuted:forWebExtensionContext:completionHandler:")]
		void SetMuted (bool muted, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("sizeForWebExtensionContext:")]
		CGSize GetSize (WKWebExtensionContext context);

		[Export ("zoomFactorForWebExtensionContext:")]
		double GetZoomFactor (WKWebExtensionContext context);

		[Async]
		[Export ("setZoomFactor:forWebExtensionContext:completionHandler:")]
		void SetZoomFactor (double zoomFactor, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("urlForWebExtensionContext:")]
		[return: NullAllowed]
		NSUrl GetUrl (WKWebExtensionContext context);

		[Export ("pendingURLForWebExtensionContext:")]
		[return: NullAllowed]
		NSUrl GetPendingUrl (WKWebExtensionContext context);

		[Export ("isLoadingCompleteForWebExtensionContext:")]
		bool IsLoadingComplete (WKWebExtensionContext context);

		[Async]
		[Export ("detectWebpageLocaleForWebExtensionContext:completionHandler:")]
		void DetectWebpageLocale (WKWebExtensionContext context, WKWebExtensionTabDetectLocaleCallback completionHandler);

		[Async]
		[Export ("takeSnapshotUsingConfiguration:forWebExtensionContext:completionHandler:")]
		void TakeSnapshot (WKSnapshotConfiguration configuration, WKWebExtensionContext context, WKWebExtensionTabTakeSnapshotCallback completionHandler);

		[Async]
		[Export ("loadURL:forWebExtensionContext:completionHandler:")]
		void LoadUrl (NSUrl url, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Async]
		[Export ("reloadFromOrigin:forWebExtensionContext:completionHandler:")]
		void ReloadFromOrigin (bool fromOrigin, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Async]
		[Export ("goBackForWebExtensionContext:completionHandler:")]
		void GoBack (WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Async]
		[Export ("goForwardForWebExtensionContext:completionHandler:")]
		void GoForward (WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Async]
		[Export ("activateForWebExtensionContext:completionHandler:")]
		void Activate (WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("isSelectedForWebExtensionContext:")]
		bool IsSelected (WKWebExtensionContext context);

		[Async]
		[Export ("setSelected:forWebExtensionContext:completionHandler:")]
		void SetSelected (bool selected, WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Async]
		[Export ("duplicateUsingConfiguration:forWebExtensionContext:completionHandler:")]
		void Duplicate (WKWebExtensionTabConfiguration configuration, WKWebExtensionContext context, WKWebExtensionTabDuplicateCallback completionHandler);

		[Async]
		[Export ("closeForWebExtensionContext:completionHandler:")]
		void Close (WKWebExtensionContext context, WKWebExtensionTabCallback completionHandler);

		[Export ("shouldGrantPermissionsOnUserGestureForWebExtensionContext:")]
		bool ShouldGrantPermissionsOnUserGesture (WKWebExtensionContext context);

		[Export ("shouldBypassPermissionsForWebExtensionContext:")]
		bool ShouldBypassPermissions (WKWebExtensionContext context);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[ErrorDomain ("WKWebExtensionContextErrorDomain")]
	[Native]
	public enum WKWebExtensionContextError : long {
		Unknown = 1,
		AlreadyLoaded,
		NotLoaded,
		BaseUrlAlreadyInUse,
		NoBackgroundContent,
		BackgroundContentFailedToLoad,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	public enum WKWebExtensionContextPermissionStatus : long {
		DeniedExplicitly = -3,
		DeniedImplicitly = -2,
		RequestedImplicitly = -1,
		Unknown = 0,
		RequestedExplicitly = 1,
		GrantedImplicitly = 2,
		GrantedExplicitly = 3,
	}

	delegate void WKWebExtensionContextCallback ([NullAllowed] NSError error);

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionContext {
		[Static]
		[Export ("contextForExtension:")]
		WKWebExtensionContext Create (WKWebExtension extension);

		[Export ("initForExtension:")]
		[DesignatedInitializer]
		NativeHandle Constructor (WKWebExtension extension);

		[Export ("webExtension", ArgumentSemantic.Strong)]
		WKWebExtension WebExtension { get; }

		[NullAllowed, Export ("webExtensionController", ArgumentSemantic.Weak)]
		WKWebExtensionController WebExtensionController { get; }

		[Export ("loaded")]
		bool Loaded { [Bind ("isLoaded")] get; }

		[Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; }

		[Export ("baseURL", ArgumentSemantic.Copy)]
		NSUrl BaseUrl { get; set; }

		[Export ("uniqueIdentifier")]
		string UniqueIdentifier { get; set; }

		[Export ("inspectable")]
		bool Inspectable { [Bind ("isInspectable")] get; set; }

		[NullAllowed, Export ("inspectionName")]
		string InspectionName { get; set; }

		[NullAllowed, Export ("unsupportedAPIs", ArgumentSemantic.Copy)]
		NSSet<NSString> UnsupportedAPIs { get; set; }

		[NullAllowed, Export ("webViewConfiguration", ArgumentSemantic.Copy)]
		WKWebViewConfiguration WebViewConfiguration { get; }

		[NullAllowed, Export ("optionsPageURL", ArgumentSemantic.Copy)]
		NSUrl OptionsPageUrl { get; }

		[NullAllowed, Export ("overrideNewTabPageURL", ArgumentSemantic.Copy)]
		NSUrl OverrideNewTabPageUrl { get; }

		[Export ("grantedPermissions", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSDate> GrantedPermissions { get; set; }

		[Export ("grantedPermissionMatchPatterns", ArgumentSemantic.Copy)]
		NSDictionary<WKWebExtensionMatchPattern, NSDate> GrantedPermissionMatchPatterns { get; set; }

		[Export ("deniedPermissions", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSDate> DeniedPermissions { get; set; }

		[Export ("deniedPermissionMatchPatterns", ArgumentSemantic.Copy)]
		NSDictionary<WKWebExtensionMatchPattern, NSDate> DeniedPermissionMatchPatterns { get; set; }

		[Export ("hasRequestedOptionalAccessToAllHosts")]
		bool HasRequestedOptionalAccessToAllHosts { get; set; }

		[Export ("hasAccessToPrivateData")]
		bool HasAccessToPrivateData { get; set; }

		[Export ("currentPermissions", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakCurrentPermissions { get; }

		WKWebExtensionPermission CurrentPermission {
			[Wrap ("WKWebExtensionPermissionExtensions.ToFlags (WeakCurrentPermissions);")]
			get;
		}

		[Export ("currentPermissionMatchPatterns", ArgumentSemantic.Copy)]
		NSSet<WKWebExtensionMatchPattern> CurrentPermissionMatchPatterns { get; }

		[Export ("hasPermission:")]
		bool HasPermission (string permission);

		[Export ("hasPermission:inTab:")]
		bool HasPermission (string permission, [NullAllowed] IWKWebExtensionTab tab);

		[Export ("hasAccessToURL:")]
		bool HasAccessToUrl (NSUrl url);

		[Export ("hasAccessToURL:inTab:")]
		bool HasAccessToUrl (NSUrl url, [NullAllowed] IWKWebExtensionTab tab);

		[Export ("hasAccessToAllURLs")]
		bool HasAccessToAllUrls { get; }

		[Export ("hasAccessToAllHosts")]
		bool HasAccessToAllHosts { get; }

		[Export ("hasInjectedContent")]
		bool HasInjectedContent { get; }

		[Export ("hasInjectedContentForURL:")]
		bool HasInjectedContentForUrl (NSUrl url);

		[Export ("hasContentModificationRules")]
		bool HasContentModificationRules { get; }

		[Export ("permissionStatusForPermission:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (string permission);

		[Export ("permissionStatusForPermission:inTab:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (string permission, [NullAllowed] IWKWebExtensionTab tab);

		[Export ("setPermissionStatus:forPermission:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, string permission);

		[Export ("setPermissionStatus:forPermission:expirationDate:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, string permission, [NullAllowed] NSDate expirationDate);

		[Export ("permissionStatusForURL:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (NSUrl url);

		[Export ("permissionStatusForURL:inTab:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (NSUrl url, [NullAllowed] IWKWebExtensionTab tab);

		[Export ("setPermissionStatus:forURL:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, NSUrl url);

		[Export ("setPermissionStatus:forURL:expirationDate:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, NSUrl url, [NullAllowed] NSDate expirationDate);

		[Export ("permissionStatusForMatchPattern:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (WKWebExtensionMatchPattern pattern);

		[Export ("permissionStatusForMatchPattern:inTab:")]
		WKWebExtensionContextPermissionStatus GetPermissionStatus (WKWebExtensionMatchPattern pattern, [NullAllowed] IWKWebExtensionTab tab);

		[Export ("setPermissionStatus:forMatchPattern:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, WKWebExtensionMatchPattern pattern);

		[Export ("setPermissionStatus:forMatchPattern:expirationDate:")]
		void SetPermissionStatus (WKWebExtensionContextPermissionStatus status, WKWebExtensionMatchPattern pattern, [NullAllowed] NSDate expirationDate);

		[Async]
		[Export ("loadBackgroundContentWithCompletionHandler:")]
		void LoadBackgroundContent (WKWebExtensionContextCallback completionHandler);

		[Export ("actionForTab:")]
		[return: NullAllowed]
		WKWebExtensionAction GetAction ([NullAllowed] IWKWebExtensionTab tab);

		[Export ("performActionForTab:")]
		void PerformAction ([NullAllowed] IWKWebExtensionTab tab);

		[Export ("commands", ArgumentSemantic.Copy)]
		WKWebExtensionCommand [] Commands { get; }

		[Export ("performCommand:")]
		void PerformCommand (WKWebExtensionCommand command);

#if IOS || MACCATALYST
		[NoMac]
		[Export ("performCommandForKeyCommand:")]
		bool PerformCommand (UIKeyCommand keyCommand);
#endif

#if MONOMAC
		[Export ("performCommandForEvent:")]
		bool PerformCommand (NSEvent @event);

		[Export ("commandForEvent:")]
		[return: NullAllowed]
		WKWebExtensionCommand GetCommand (NSEvent @event);
#endif

		[Export ("menuItemsForTab:")]
#if IOS || MACCATALYST
		UIMenuElement [] GetMenuItems (IWKWebExtensionTab tab);
#else
		NSMenuItem [] GetMenuItems (IWKWebExtensionTab tab);
#endif

		[Export ("userGesturePerformedInTab:")]
		void UserGesturePerformed (IWKWebExtensionTab tab);

		[Export ("hasActiveUserGestureInTab:")]
		bool HasActiveUserGesture (IWKWebExtensionTab tab);

		[Export ("clearUserGestureInTab:")]
		void ClearUserGesture (IWKWebExtensionTab tab);

		[Export ("openWindows", ArgumentSemantic.Copy)]
		IWKWebExtensionWindow [] OpenWindows { get; }

		[NullAllowed, Export ("focusedWindow", ArgumentSemantic.Weak)]
		IWKWebExtensionWindow FocusedWindow { get; }

		[Export ("openTabs", ArgumentSemantic.Copy)]
		NSSet<IWKWebExtensionTab> OpenTabs { get; }

		[Export ("didOpenWindow:")]
		void DidOpenWindow (IWKWebExtensionWindow newWindow);

		[Export ("didCloseWindow:")]
		void DidCloseWindow (IWKWebExtensionWindow closedWindow);

		[Export ("didFocusWindow:")]
		void DidFocusWindow ([NullAllowed] IWKWebExtensionWindow focusedWindow);

		[Export ("didOpenTab:")]
		void DidOpenTab (IWKWebExtensionTab newTab);

		[Export ("didCloseTab:windowIsClosing:")]
		void DidCloseTab (IWKWebExtensionTab closedTab, bool windowIsClosing);

		[Export ("didActivateTab:previousActiveTab:")]
		void DidActivateTab (IWKWebExtensionTab activatedTab, [NullAllowed] IWKWebExtensionTab previousTab);

		[Export ("didSelectTabs:")]
		void DidSelectTabs (IWKWebExtensionTab [] selectedTabs);

		[Export ("didDeselectTabs:")]
		void DidDeselectTabs (IWKWebExtensionTab [] deselectedTabs);

		[Export ("didMoveTab:fromIndex:inWindow:")]
		void DidMoveTab (IWKWebExtensionTab movedTab, nuint index, [NullAllowed] IWKWebExtensionWindow oldWindow);

		[Export ("didReplaceTab:withTab:")]
		void DidReplaceTab (IWKWebExtensionTab oldTab, IWKWebExtensionTab newTab);

		[Export ("didChangeTabProperties:forTab:")]
		void DidChangeTabProperties (WKWebExtensionTabChangedProperties properties, IWKWebExtensionTab changedTab);

		[Notification]
		[Field ("WKWebExtensionContextErrorsDidUpdateNotification")]
		NSString ErrorsDidUpdateNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextPermissionsWereGrantedNotification")]
		NSString PermissionsWereGrantedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextPermissionsWereDeniedNotification")]
		NSString PermissionsWereDeniedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextGrantedPermissionsWereRemovedNotification")]
		NSString GrantedPermissionsWereRemovedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextDeniedPermissionsWereRemovedNotification")]
		NSString DeniedPermissionsWereRemovedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextPermissionMatchPatternsWereGrantedNotification")]
		NSString PermissionMatchPatternsWereGrantedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextPermissionMatchPatternsWereDeniedNotification")]
		NSString PermissionMatchPatternsWereDeniedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextGrantedPermissionMatchPatternsWereRemovedNotification")]
		NSString GrantedPermissionMatchPatternsWereRemovedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextDeniedPermissionMatchPatternsWereRemovedNotification")]
		NSString DeniedPermissionMatchPatternsWereRemovedNotification { get; }

		[Notification]
		[Field ("WKWebExtensionContextNotificationUserInfoKeyPermissions")]
		NSString NotificationUserInfoKeyPermissions { get; }

		[Notification]
		[Field ("WKWebExtensionContextNotificationUserInfoKeyMatchPatterns")]
		NSString NotificationUserInfoKeyMatchPatterns { get; }
	}

	delegate void WKWebExtensionControllerDelegateOpenNewWindowCallback ([NullAllowed] IWKWebExtensionWindow newWindow, [NullAllowed] NSError error);
	delegate void WKWebExtensionControllerDelegateOpenNewTabCallback ([NullAllowed] IWKWebExtensionTab newWindow, [NullAllowed] NSError error);
	delegate void WKWebExtensionControllerDelegateOpenOptionsCallback ([NullAllowed] NSError error);
	delegate void WKWebExtensionControllerDelegatePromptForPermissionsCallback (NSSet<NSString> allowedPermissions, [NullAllowed] NSDate expirationDate);
	delegate void WKWebExtensionControllerDelegatePromptForPermissionsToAccessUrlsCallback (NSSet<NSUrl> allowedUrls, [NullAllowed] NSDate expirationDate);
	delegate void WKWebExtensionControllerDelegatePromptForPermissionMatchPatternsCallback (NSSet<WKWebExtensionMatchPattern> allowedMatchPatterns, [NullAllowed] NSDate expirationDate);
	delegate void WKWebExtensionControllerDelegatePresentPopupForActionCallback ([NullAllowed] NSError error);
	delegate void WKWebExtensionControllerDelegateSendMessageCallback ([NullAllowed] NSObject replyMessage, [NullAllowed] NSError error);
	delegate void WKWebExtensionControllerDelegateConnectCallback ([NullAllowed] NSError error);

	interface IWKWebExtensionControllerDelegate { }

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface WKWebExtensionControllerDelegate {
		[Export ("webExtensionController:openWindowsForExtensionContext:")]
		IWKWebExtensionWindow [] OpenWindows (WKWebExtensionController controller, WKWebExtensionContext extensionContext);

		[Export ("webExtensionController:focusedWindowForExtensionContext:")]
		[return: NullAllowed]
		IWKWebExtensionWindow GetFocusedWindow (WKWebExtensionController controller, WKWebExtensionContext extensionContext);

		[Async]
		[Export ("webExtensionController:openNewWindowUsingConfiguration:forExtensionContext:completionHandler:")]
		void OpenNewWindow (WKWebExtensionController controller, WKWebExtensionWindowConfiguration configuration, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegateOpenNewWindowCallback completionHandler);

		[Async]
		[Export ("webExtensionController:openNewTabUsingConfiguration:forExtensionContext:completionHandler:")]
		void OpenNewTab (WKWebExtensionController controller, WKWebExtensionTabConfiguration configuration, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegateOpenNewTabCallback completionHandler);

		[Async]
		[Export ("webExtensionController:openOptionsPageForExtensionContext:completionHandler:")]
		void OpenOptions (WKWebExtensionController controller, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegateOpenOptionsCallback completionHandler);

		[Export ("webExtensionController:promptForPermissions:inTab:forExtensionContext:completionHandler:")]
		void PromptForPermissions (WKWebExtensionController controller, NSSet<NSString> permissions, [NullAllowed] IWKWebExtensionTab tab, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegatePromptForPermissionsCallback completionHandler);

		[Export ("webExtensionController:promptForPermissionToAccessURLs:inTab:forExtensionContext:completionHandler:")]
		void PromptForPermissionsToAccessUrls (WKWebExtensionController controller, NSSet<NSUrl> urls, [NullAllowed] IWKWebExtensionTab tab, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegatePromptForPermissionsToAccessUrlsCallback completionHandler);

		[Export ("webExtensionController:promptForPermissionMatchPatterns:inTab:forExtensionContext:completionHandler:")]
		void PromptForPermissionMatchPatterns (WKWebExtensionController controller, NSSet<WKWebExtensionMatchPattern> matchPatterns, [NullAllowed] IWKWebExtensionTab tab, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegatePromptForPermissionMatchPatternsCallback completionHandler);

		[Export ("webExtensionController:didUpdateAction:forExtensionContext:")]
		void DidUpdateAction (WKWebExtensionController controller, WKWebExtensionAction action, WKWebExtensionContext context);

		[Export ("webExtensionController:presentPopupForAction:forExtensionContext:completionHandler:")]
		void PresentPopupForAction (WKWebExtensionController controller, WKWebExtensionAction action, WKWebExtensionContext context, WKWebExtensionControllerDelegatePresentPopupForActionCallback completionHandler);

		[Async]
		[Export ("webExtensionController:sendMessage:toApplicationWithIdentifier:forExtensionContext:replyHandler:")]
		void SendMessage (WKWebExtensionController controller, NSObject message, [NullAllowed] string applicationIdentifier, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegateSendMessageCallback replyHandler);

		[Async]
		[Export ("webExtensionController:connectUsingMessagePort:forExtensionContext:completionHandler:")]
		void Connect (WKWebExtensionController controller, WKWebExtensionMessagePort port, WKWebExtensionContext extensionContext, WKWebExtensionControllerDelegateConnectCallback completionHandler);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Flags]
	enum WKWebExtensionDataType {
		[Field ("WKWebExtensionDataTypeLocal")]
		Local = 1,

		[Field ("WKWebExtensionDataTypeSession")]
		Session = 2,

		[Field ("WKWebExtensionDataTypeSynchronized")]
		Synchronized = 4,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	public enum WKWebExtensionWindowType : long {
		Normal,
		Popup,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	public enum WKWebExtensionWindowState : long {
		Normal,
		Minimized,
		Maximized,
		Fullscreen,
	}

	delegate void WKWebExtensionWindowCallback ([NullAllowed] NSError error);

	interface IWKWebExtensionWindow { }

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface WKWebExtensionWindow {
		[Export ("tabsForWebExtensionContext:")]
		IWKWebExtensionTab [] GetTabs (WKWebExtensionContext context);

		[Export ("activeTabForWebExtensionContext:")]
		[return: NullAllowed]
		IWKWebExtensionTab ActiveTab (WKWebExtensionContext context);

		[Export ("windowTypeForWebExtensionContext:")]
		WKWebExtensionWindowType GetWindowType (WKWebExtensionContext context);

		[Export ("windowStateForWebExtensionContext:")]
		WKWebExtensionWindowState GetWindowState (WKWebExtensionContext context);

		[Async]
		[Export ("setWindowState:forWebExtensionContext:completionHandler:")]
		void SetWindowState (WKWebExtensionWindowState state, WKWebExtensionContext context, WKWebExtensionWindowCallback completionHandler);

		[Export ("isPrivateForWebExtensionContext:")]
		bool IsPrivate (WKWebExtensionContext context);

#if MONOMAC
		[Export ("screenFrameForWebExtensionContext:")]
		CGRect GetScreenFrame (WKWebExtensionContext context);
#endif

		[Export ("frameForWebExtensionContext:")]
		CGRect GetFrame (WKWebExtensionContext context);

		[Async]
		[Export ("setFrame:forWebExtensionContext:completionHandler:")]
		void SetFrame (CGRect frame, WKWebExtensionContext context, WKWebExtensionWindowCallback completionHandler);

		[Async]
		[Export ("focusForWebExtensionContext:completionHandler:")]
		void Focus (WKWebExtensionContext context, WKWebExtensionWindowCallback completionHandler);

		[Async]
		[Export ("closeForWebExtensionContext:completionHandler:")]
		void Close (WKWebExtensionContext context, WKWebExtensionWindowCallback completionHandler);
	}

	delegate void WKWebExtensionControllerDataRecordCallback ([NullAllowed] WKWebExtensionDataRecord dataRecord);
	delegate void WKWebExtensionControllerDataRecordsCallback (WKWebExtensionDataRecord [] dataRecords);

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // added below to get the DesignatedInitializer attribute
	interface WKWebExtensionController {
		[DesignatedInitializer]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithConfiguration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (WKWebExtensionControllerConfiguration configuration);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		IWKWebExtensionControllerDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("configuration", ArgumentSemantic.Copy)]
		WKWebExtensionControllerConfiguration Configuration { get; }

		[Export ("loadExtensionContext:error:")]
		bool LoadExtensionContext (WKWebExtensionContext extensionContext, [NullAllowed] out NSError error);

		[Export ("unloadExtensionContext:error:")]
		bool UnloadExtensionContext (WKWebExtensionContext extensionContext, [NullAllowed] out NSError error);

		[Export ("extensionContextForExtension:")]
		[return: NullAllowed]
		WKWebExtensionContext GetExtensionContext (WKWebExtension extension);

		[Export ("extensionContextForURL:")]
		[return: NullAllowed]
		WKWebExtensionContext GetExtensionContext (NSUrl url);

		[Export ("extensions", ArgumentSemantic.Copy)]
		NSSet<WKWebExtension> Extensions { get; }

		[Export ("extensionContexts", ArgumentSemantic.Copy)]
		NSSet<WKWebExtensionContext> ExtensionContexts { get; }

		[Static]
		[Export ("allExtensionDataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakAllExtensionDataTypes { get; }

		[Static]
		WKWebExtensionDataType AllExtensionDataTypes {
			[Wrap ("WKWebExtensionDataTypeExtensions.ToFlags (WeakAllExtensionDataTypes);")]
			get;
		}

		[Async]
		[Export ("fetchDataRecordsOfTypes:completionHandler:")]
		void FetchDataRecords (NSSet<NSString> dataTypes, WKWebExtensionControllerDataRecordsCallback completionHandler);

		[Async]
		[Wrap ("FetchDataRecords (new NSSet<NSString> (dataTypes.ToArray ()), completionHandler);")]
		void FetchDataRecords (WKWebExtensionDataType dataTypes, WKWebExtensionControllerDataRecordsCallback completionHandler);

		[Async]
		[Export ("fetchDataRecordOfTypes:forExtensionContext:completionHandler:")]
		void FetchDataRecord (NSSet<NSString> dataTypes, WKWebExtensionContext extensionContext, WKWebExtensionControllerDataRecordCallback completionHandler);

		[Async]
		[Wrap ("FetchDataRecord (new NSSet<NSString> (dataTypes.ToArray ()), extensionContext, completionHandler);")]
		void FetchDataRecord (WKWebExtensionDataType dataTypes, WKWebExtensionContext extensionContext, WKWebExtensionControllerDataRecordCallback completionHandler);

		[Async]
		[Export ("removeDataOfTypes:fromDataRecords:completionHandler:")]
		void RemoveData (NSSet<NSString> dataTypes, WKWebExtensionDataRecord [] dataRecords, Action completionHandler);

		[Async]
		[Wrap ("RemoveData (new NSSet<NSString> (dataTypes.ToArray ()), dataRecords, completionHandler);")]
		void RemoveData (WKWebExtensionDataType dataTypes, WKWebExtensionDataRecord [] dataRecords, Action completionHandler);

		[Export ("didOpenWindow:")]
		void DidOpenWindow (IWKWebExtensionWindow newWindow);

		[Export ("didCloseWindow:")]
		void DidCloseWindow (IWKWebExtensionWindow closedWindow);

		[Export ("didFocusWindow:")]
		void DidFocusWindow ([NullAllowed] IWKWebExtensionWindow focusedWindow);

		[Export ("didOpenTab:")]
		void DidOpenTab (IWKWebExtensionTab newTab);

		[Export ("didCloseTab:windowIsClosing:")]
		void DidCloseTab (IWKWebExtensionTab closedTab, bool windowIsClosing);

		[Export ("didActivateTab:previousActiveTab:")]
		void DidActivateTab (IWKWebExtensionTab activatedTab, [NullAllowed] IWKWebExtensionTab previousTab);

		[Export ("didSelectTabs:")]
		void DidSelectTabs (IWKWebExtensionTab [] selectedTabs);

		[Export ("didDeselectTabs:")]
		void DidDeselectTabs (IWKWebExtensionTab [] deselectedTabs);

		[Export ("didMoveTab:fromIndex:inWindow:")]
		void DidMoveTab (IWKWebExtensionTab movedTab, nuint index, [NullAllowed] IWKWebExtensionWindow oldWindow);

		[Export ("didReplaceTab:withTab:")]
		void DidReplaceTab (IWKWebExtensionTab oldTab, IWKWebExtensionTab newTab);

		[Export ("didChangeTabProperties:forTab:")]
		void DidChangeTabProperties (WKWebExtensionTabChangedProperties properties, IWKWebExtensionTab changedTab);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionControllerConfiguration : NSSecureCoding, NSCopying {
		[Static]
		[Export ("defaultConfiguration")]
		WKWebExtensionControllerConfiguration GetDefaultConfiguration ();

		[Static]
		[Export ("nonPersistentConfiguration")]
		WKWebExtensionControllerConfiguration GetNonPersistentConfiguration ();

		[Static]
		[Export ("configurationWithIdentifier:")]
		WKWebExtensionControllerConfiguration Create (NSUuid identifier);

		[Export ("persistent")]
		bool Persistent { [Bind ("isPersistent")] get; }

		[NullAllowed, Export ("identifier", ArgumentSemantic.Copy)]
		NSUuid Identifier { get; }

		[NullAllowed, Export ("webViewConfiguration", ArgumentSemantic.Copy)]
		WKWebViewConfiguration WebViewConfiguration { get; set; }

		[NullAllowed, Export ("defaultWebsiteDataStore", ArgumentSemantic.Retain)]
		WKWebsiteDataStore DefaultWebsiteDataStore { get; set; }
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	[ErrorDomain ("WKWebExtensionDataRecordErrorDomain")]
	public enum WKWebExtensionDataRecordError : long {
		Unknown = 1,
		LocalStorageFailed,
		SessionStorageFailed,
		SynchronizedStorageFailed,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionDataRecord {
		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("uniqueIdentifier")]
		string UniqueIdentifier { get; }

		[Export ("containedDataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> WeakContainedDataTypes { get; }

		WKWebExtensionDataType ContainedDataTypes {
			[Wrap ("WKWebExtensionDataTypeExtensions.ToFlags (WeakContainedDataTypes);")]
			get;
		}

		[Export ("errors", ArgumentSemantic.Copy)]
		NSError [] Errors { get; }

		[Export ("totalSizeInBytes")]
		nuint TotalSizeInBytes { get; }

		[Export ("sizeInBytesOfTypes:")]
		nuint GetSizeInBytes (NSSet<NSString> dataTypes);

		[Wrap ("GetSizeInBytes (new NSSet<NSString> (dataTypes.ToArray ()));")]
		nuint GetSizeInBytes (WKWebExtensionDataType dataTypes);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	[ErrorDomain ("WKWebExtensionMessagePortErrorDomain")]
	public enum WKWebExtensionMessagePortError : long {
		Unknown = 1,
		NotConnected,
		MessageInvalid,
	}

	delegate void WKWebExtensionMessagePortMessageHandlerCallback ([NullAllowed] NSObject message, [NullAllowed] NSError error);
	delegate void WKWebExtensionMessagePortDisconnectHandlerCallback ([NullAllowed] NSError error);
	delegate void WKWebExtensionMessagePortSendMessageCallback ([NullAllowed] NSError error);

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionMessagePort {
		[NullAllowed, Export ("applicationIdentifier")]
		string ApplicationIdentifier { get; }

		[NullAllowed, Export ("messageHandler", ArgumentSemantic.Copy)]
		WKWebExtensionMessagePortMessageHandlerCallback MessageHandler { get; set; }

		[NullAllowed, Export ("disconnectHandler", ArgumentSemantic.Copy)]
		WKWebExtensionMessagePortDisconnectHandlerCallback DisconnectHandler { get; set; }

		[Export ("disconnected")]
		bool Disconnected { [Bind ("isDisconnected")] get; }

		[Async]
		[Export ("sendMessage:completionHandler:")]
		void SendMessage ([NullAllowed] NSObject message, [NullAllowed] WKWebExtensionMessagePortSendMessageCallback completionHandler);

		[Export ("disconnect")]
		void Disconnect ();

		[Export ("disconnectWithError:")]
		void Disconnect ([NullAllowed] NSError error);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionTabConfiguration {
		[NullAllowed, Export ("window", ArgumentSemantic.Strong)]
		IWKWebExtensionWindow Window { get; }

		[Export ("index")]
		nuint Index { get; }

		[NullAllowed, Export ("parentTab", ArgumentSemantic.Strong)]
		IWKWebExtensionTab ParentTab { get; }

		[NullAllowed, Export ("url", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("shouldBeActive")]
		bool ShouldBeActive { get; }

		[Export ("shouldAddToSelection")]
		bool ShouldAddToSelection { get; }

		[Export ("shouldBePinned")]
		bool ShouldBePinned { get; }

		[Export ("shouldBeMuted")]
		bool ShouldBeMuted { get; }

		[Export ("shouldReaderModeBeActive")]
		bool ShouldReaderModeBeActive { get; }
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionWindowConfiguration {
		[Export ("windowType")]
		WKWebExtensionWindowType WindowType { get; }

		[Export ("windowState")]
		WKWebExtensionWindowState WindowState { get; }

		[Export ("frame")]
		CGRect Frame { get; }

		[Export ("tabURLs", ArgumentSemantic.Copy)]
		NSUrl [] TabUrls { get; }

		[Export ("tabs", ArgumentSemantic.Copy)]
		IWKWebExtensionTab [] Tabs { get; }

		[Export ("shouldBeFocused")]
		bool ShouldBeFocused { get; }

		[Export ("shouldBePrivate")]
		bool ShouldBePrivate { get; }
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[ErrorDomain ("WKWebExtensionMatchPatternErrorDomain")]
	[Native]
	public enum WKWebExtensionMatchPatternError : long {
		Unknown = 1,
		InvalidScheme,
		InvalidHost,
		InvalidPath,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Native]
	[Flags]
	public enum WKWebExtensionMatchPatternOptions : ulong {
		None = 0,
		IgnoreSchemes = 1uL << 0,
		IgnorePaths = 1uL << 1,
		MatchBidirectionally = 1uL << 2,
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKWebExtensionMatchPattern : NSSecureCoding, NSCopying {
		[Static]
		[Export ("registerCustomURLScheme:")]
		void RegisterCustomUrlScheme (string urlScheme);

		[Static]
		[Export ("allURLsMatchPattern")]
		WKWebExtensionMatchPattern GetAllUrlsMatchPattern ();

		[Static]
		[Export ("allHostsAndSchemesMatchPattern")]
		WKWebExtensionMatchPattern GetAllHostsAndSchemesMatchPattern ();

		[Static]
		[Export ("matchPatternWithString:")]
		[return: NullAllowed]
		WKWebExtensionMatchPattern Create (string @string);

		[Static]
		[Export ("matchPatternWithScheme:host:path:")]
		[return: NullAllowed]
		WKWebExtensionMatchPattern Create (string scheme, string host, string path);

		/// <summary>Create a new <see cref="WKWebExtensionMatchPattern" /> for the specified pattern.</summary>
		/// <param name="string">The pattern to initialize the new instance with.</param>
		/// <param name="error">The error object, if an error occurred, otherwise null.</param>
		/// <returns>A newly created <see cref="WKWebExtensionMatchPattern" /> if successful, otherwise null.</returns>
		[DesignatedInitializer]
		[Export ("initWithString:error:")]
		[FactoryMethod]
		[return: NullAllowed]
		NativeHandle Constructor (string @string, [NullAllowed] out NSError error);

		/// <summary>Create a new <see cref="WKWebExtensionMatchPattern" /> for the specified scheme, host and path.</summary>
		/// <param name="scheme">The scheme to initialize the new instance with.</param>
		/// <param name="host">The host to initialize the new instance with.</param>
		/// <param name="path">The path to initialize the new instance with.</param>
		/// <param name="error">The error object, if an error occurred, otherwise null.</param>
		/// <returns>A newly created <see cref="WKWebExtensionMatchPattern" /> if successful, otherwise null.</returns>
		[DesignatedInitializer]
		[Export ("initWithScheme:host:path:error:")]
		[FactoryMethod]
		[return: NullAllowed]
		NativeHandle Constructor (string scheme, string host, string path, [NullAllowed] out NSError error);

		[Export ("string")]
		string String { get; }

		[NullAllowed, Export ("scheme")]
		string Scheme { get; }

		[NullAllowed, Export ("host")]
		string Host { get; }

		[NullAllowed, Export ("path")]
		string Path { get; }

		[Export ("matchesAllURLs")]
		bool MatchesAllUrls { get; }

		[Export ("matchesAllHosts")]
		bool MatchesAllHosts { get; }

		[Export ("matchesURL:")]
		bool MatchesUrl ([NullAllowed] NSUrl url);

		[Export ("matchesURL:options:")]
		bool MatchesUrl ([NullAllowed] NSUrl url, WKWebExtensionMatchPatternOptions options);

		[Export ("matchesPattern:")]
		bool MatchesPattern ([NullAllowed] WKWebExtensionMatchPattern pattern);

		[Export ("matchesPattern:options:")]
		bool MatchesPattern ([NullAllowed] WKWebExtensionMatchPattern pattern, WKWebExtensionMatchPatternOptions options);
	}

	[Mac (15, 4), iOS (18, 4), MacCatalyst (18, 4), NoTV]
	[Flags]
	enum WKWebExtensionPermission {
		[Field ("WKWebExtensionPermissionActiveTab")]
		ActiveTab = 1 << 0,

		[Field ("WKWebExtensionPermissionAlarms")]
		Alarms = 1 << 1,

		[Field ("WKWebExtensionPermissionClipboardWrite")]
		ClipboardWrite = 1 << 2,

		[Field ("WKWebExtensionPermissionContextMenus")]
		ContextMenus = 1 << 3,

		[Field ("WKWebExtensionPermissionCookies")]
		Cookies = 1 << 4,

		[Field ("WKWebExtensionPermissionDeclarativeNetRequest")]
		DeclarativeNetRequest = 1 << 5,

		[Field ("WKWebExtensionPermissionDeclarativeNetRequestFeedback")]
		DeclarativeNetRequestFeedback = 1 << 6,

		[Field ("WKWebExtensionPermissionDeclarativeNetRequestWithHostAccess")]
		DeclarativeNetRequestWithHostAccess = 1 << 7,

		[Field ("WKWebExtensionPermissionMenus")]
		Menus = 1 << 8,

		[Field ("WKWebExtensionPermissionNativeMessaging")]
		NativeMessaging = 1 << 9,

		[Field ("WKWebExtensionPermissionScripting")]
		Scripting = 1 << 10,

		[Field ("WKWebExtensionPermissionStorage")]
		Storage = 1 << 11,

		[Field ("WKWebExtensionPermissionTabs")]
		Tabs = 1 << 12,

		[Field ("WKWebExtensionPermissionUnlimitedStorage")]
		UnlimitedStorage = 1 << 13,

		[Field ("WKWebExtensionPermissionWebNavigation")]
		WebNavigation = 1 << 14,

		[Field ("WKWebExtensionPermissionWebRequest")]
		WebRequest = 1 << 15,
	}

	[Flags]
	[Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
	[Native]
	public enum WKWebViewDataType : ulong {
		SessionStorage = 1uL << 0,
	}
}
