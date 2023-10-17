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

using System;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using JavaScriptCore;
using Security;

#if MONOMAC
using AppKit;
using UIColor=AppKit.NSColor;
using UIScrollView = AppKit.NSScrollView;
using UIImage = AppKit.NSImage;
using IUIContextMenuInteractionCommitAnimating = Foundation.NSObject;
using UIContextMenuConfiguration = Foundation.NSObject;
using UIEdgeInsets = AppKit.NSEdgeInsets;
using UIFindInteraction = Foundation.NSObject;
using UIViewController = AppKit.NSViewController;
using IUIEditMenuInteractionAnimating = Foundation.NSObject;
#else
#if __MACCATALYST__
using AppKit;
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
using NSResponder = UIKit.UIResponder;
using NSSelectionAffinity = Foundation.NSObject;
using NSUserInterfaceValidations = Foundation.NSObjectProtocol;
using NSView = UIKit.UIView;
using NSWindow = UIKit.UIWindow;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace WebKit {

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (WebScriptObject), Name = "DOMObject")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMObject init]: should never be used
	partial interface DomObject : NSCopying {
	}

	/////////////////////////
	// DomObject subclasses

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMAbstractView")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMAbstractView init]: should never be used
	partial interface DomAbstractView {
		[Export ("document", ArgumentSemantic.Retain)]
		DomDocument Document { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSCharsetRule")]
	[DisableDefaultCtor]
	partial interface DomCssCharsetRule {
		[Export ("encoding")]
		string Encoding { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSFontFaceRule")]
	[DisableDefaultCtor]
	partial interface DomCssFontFaceRule {
		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSPageRule")]
	[DisableDefaultCtor]
	partial interface DomCssPageRule {
		[Export ("selectorText")]
		string SelectorText { get; }

		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSStyleRule")]
	[DisableDefaultCtor]
	partial interface DomCssStyleRule {
		[Export ("selectorText")]
		string SelectorText { get; }

		[Export ("style", ArgumentSemantic.Strong)]
		DomCssStyleDeclaration Style { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCssRule), Name = "DOMCSSUnknownRule")]
	[DisableDefaultCtor]
	partial interface DomCssUnknownRule {
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSRuleList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSRuleList init]: should never be used
	partial interface DomCssRuleList {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomCssRule GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMCSSValue")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCSSValue init]: should never be used
	partial interface DomCssValue {
		[Export ("cssText", ArgumentSemantic.Copy)]
		string CssText { get; set; }

		[Export ("cssValueType")]
		DomCssValueType Type { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	interface IDomNodeFilter { }

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "DOMNodeFilter")]
	interface DomNodeFilter {
		[Export ("acceptNode:")]
		[Abstract]
		short AcceptNode (DomNode n);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMNodeList")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMNodeList init]: should never be used
	partial interface DomNodeList {
		[Export ("length")]
		int Count { get; } /* unsigned int */

		[Export ("item:")]
		DomNode GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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
		[Obsolete ("Use the 'Body' property instead.")]
		[Wrap ("Body", IsVirtual = true)]
		DomHtmlElement body { get; set; }
#endif

		[Export ("body", ArgumentSemantic.Retain)]
		DomHtmlElement Body { get; set; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Images' property instead.")]
		[Wrap ("Images", IsVirtual = true)]
		DomHtmlCollection images { get; }
#endif

		[Export ("images", ArgumentSemantic.Retain)]
		DomHtmlCollection Images { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Applets' property instead.")]
		[Wrap ("Applets", IsVirtual = true)]
		DomHtmlCollection applets { get; }
#endif

		[Export ("applets", ArgumentSemantic.Retain)]
		DomHtmlCollection Applets { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Links' property instead.")]
		[Wrap ("Links", IsVirtual = true)]
		DomHtmlCollection links { get; }
#endif

		[Export ("links", ArgumentSemantic.Retain)]
		DomHtmlCollection Links { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Forms' property instead.")]
		[Wrap ("Forms", IsVirtual = true)]
		DomHtmlCollection forms { get; }
#endif

		[Export ("forms", ArgumentSemantic.Retain)]
		DomHtmlCollection Forms { get; }

#if !XAMCORE_5_0
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMDocumentFragment")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMDocumentFragment init]: should never be used
	partial interface DomDocumentFragment {
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomNode), Name = "DOMEntityReference")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMEntityReference init]: should never be used
	partial interface DomEntityReference {
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject), Name = "DOMEventTarget")]
	[Protocol]
	[Model]
	partial interface DomEventTarget : NSCopying {
		[Export ("addEventListener:listener:useCapture:")]
		[Abstract]
		void AddEventListener (string type, IDomEventListener listener, bool useCapture);

		[Export ("removeEventListener:listener:useCapture:")]
		[Abstract]
		void RemoveEventListener (string type, IDomEventListener listener, bool useCapture);

		[Export ("dispatchEvent:")]
		[Abstract]
		bool DispatchEvent (DomEvent evt);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMEvent")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMEvent init]: should never be used
	partial interface DomEvent {
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("target", ArgumentSemantic.Retain)]
		[Protocolize]
		DomEventTarget Target { get; }

		[Export ("currentTarget", ArgumentSemantic.Retain)]
		[Protocolize]
		DomEventTarget CurrentTarget { get; }

		[Export ("eventPhase")]
		DomEventPhase EventPhase { get; }

		[Export ("bubbles")]
		bool Bubbles { get; }

		[Export ("cancelable")]
		bool Cancelable { get; }

		[Export ("timeStamp")]
		UInt64 TimeStamp { get; }

		[Export ("srcElement", ArgumentSemantic.Retain)]
		[Protocolize]
		DomEventTarget SourceElement { get; }

		[Export ("returnValue")]
		bool ReturnValue { get; set; }

		[Export ("cancelBubble")]
		bool CancelBubble { get; set; }

		[Export ("stopPropagation")]
		void StopPropagation ();

		[Export ("preventDefault")]
		void PreventDefault ();

#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initEvent:canBubbleArg:cancelableArg:")]
		void InitEvent (string eventTypeArg, bool canBubbleArg, bool cancelableArg);
#endif

#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
		[Export ("initEvent:canBubbleArg:cancelableArg:")]
		NativeHandle Constructor (string eventTypeArg, bool canBubbleArg, bool cancelableArg);
	}

	// Note: DOMMutationEvent is not bound since it is deprecated
	// by the W3C to be replaced with Mutation Observers

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomEvent), Name = "DOMOverflowEvent")]
	[DisableDefaultCtor]
	partial interface DomOverflowEvent {
#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initOverflowEvent:horizontalOverflow:verticalOverflow:")]
		void InitEvent (ushort orient, bool hasHorizontalOverflow, bool hasVerticalOverflow);
#endif
#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
		[Export ("initOverflowEvent:horizontalOverflow:verticalOverflow:")]
		NativeHandle Constructor (ushort orient, bool hasHorizontalOverflow, bool hasVerticalOverflow);

		[Export ("orient")]
		ushort Orient { get; }

		[Export ("horizontalOverflow")]
		bool HasHorizontalOverflow { get; }

		[Export ("verticalOverflow")]
		bool HasVerticalOverflow { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomEvent), Name = "DOMUIEvent")]
	[DisableDefaultCtor]
	partial interface DomUIEvent {
#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initUIEvent:canBubble:cancelable:view:detail:")]
		void InitEvent (string eventType, bool canBubble, bool cancelable, DomAbstractView view, int /* int, not NSInteger */ detail);
#endif
#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomUIEvent), Name = "DOMKeyboardEvent")]
	[DisableDefaultCtor]
	partial interface DomKeyboardEvent {
#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:altGraphKey:")]
		void InitEvent (string eventType, bool canBubble, bool cancelable, DomAbstractView view, string keyIdentifier, DomKeyLocation keyLocation, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, bool altGraphKey);

		[Obsolete ("Use the constructor instead.")]
		[Export ("initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:")]
		void InitEvent (string eventType, bool canBubble, bool cancelable, DomAbstractView view, string keyIdentifier, DomKeyLocation keyLocation, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey);
#endif

#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
		[Export ("initKeyboardEvent:canBubble:cancelable:view:keyIdentifier:keyLocation:ctrlKey:altKey:shiftKey:metaKey:altGraphKey:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, string keyIdentifier, DomKeyLocation keyLocation, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, bool altGraphKey);

#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomUIEvent), Name = "DOMMouseEvent")]
	[DisableDefaultCtor]
	partial interface DomMouseEvent {
#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initMouseEvent:canBubble:cancelable:view:detail:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:button:relatedTarget:")]
		void InitEvent (string eventType, bool canBubble, bool cancelable, DomAbstractView view, int /* int, not NSInteger */ detail, int /* int, not NSInteger */ screenX, int /* int, not NSInteger */ screenY, int /* int, not NSInteger */ clientX, int /* int, not NSInteger */ clientY, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, ushort button, [Protocolize] DomEventTarget relatedTarget);
#endif
#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
		[Export ("initMouseEvent:canBubble:cancelable:view:detail:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:button:relatedTarget:")]
		NativeHandle Constructor (string eventType, bool canBubble, bool cancelable, DomAbstractView view, int /* int, not NSInteger */ detail, int /* int, not NSInteger */ screenX, int /* int, not NSInteger */ screenY, int /* int, not NSInteger */ clientX, int /* int, not NSInteger */ clientY, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey, ushort button, [Protocolize] DomEventTarget relatedTarget);

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
		[Protocolize]
		DomEventTarget RelatedTarget { get; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomMouseEvent), Name = "DOMWheelEvent")]
	[DisableDefaultCtor]
	partial interface DomWheelEvent {
#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initWheelEvent:wheelDeltaY:view:screenX:screenY:clientX:clientY:ctrlKey:altKey:shiftKey:metaKey:")]
		void InitEvent (int /* int, not NSInteger */ wheelDeltaX, int /* int, not NSInteger */ wheelDeltaY, DomAbstractView view, int /* int, not NSInteger */ screenX, int /* int, not NSInteger */ screnY, int /* int, not NSInteger */ clientX, int /* int, not NSInteger */ clientY, bool ctrlKey, bool altKey, bool shiftKey, bool metaKey);
#endif
#if !XAMCORE_3_0
		[Sealed] // Just to avoid the duplicate selector error
#endif
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject), Name = "DOMEventListener")]
	[Model]
	[Protocol]
	partial interface DomEventListener {
		[Abstract]
		[Export ("handleEvent:")]
		void HandleEvent (DomEvent evt);
	}

	interface IDomEventListener { }

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomCharacterData), Name = "DOMComment")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMComment init]: should never be used
	partial interface DomComment {
	}

	///////////////////////////
	// DomText subclasses

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomText), Name = "DOMCDATASection")]
	[DisableDefaultCtor] // An uncaught exception was raised: +[DOMCDATASection init]: should never be used
	partial interface DomCDataSection {
	}

	///////////////////////////
	// DomDocument subclasses

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	partial interface WebDataSource {
		[Export ("initWithRequest:")]
		NativeHandle Constructor (NSUrlRequest request);

		[Export ("data")]
		NSData Data { get; }

		[Export ("representation")]
		[Protocolize]
		WebDocumentRepresentation Representation { get; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface WebDocumentRepresentation {
		[Abstract]
		[Export ("setDataSource:")]
		void SetDataSource (WebDataSource dataSource);

		[Abstract]
		[Export ("receivedData:withDataSource:")]
		void ReceivedData (NSData data, WebDataSource dataSource);

		[Abstract]
		[Export ("receivedError:withDataSource:")]
		void ReceivedError (NSError error, WebDataSource dataSource);

		[Abstract]
		[Export ("finishedLoadingWithDataSource:")]
		void FinishedLoading (WebDataSource dataSource);

		[Abstract]
		[Export ("canProvideDocumentSource")]
		bool CanProvideDocumentSource { get; }

		[Abstract]
		[Export ("documentSource")]
		string DocumentSource { get; }

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


	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSUrlDownload))]
	partial interface WebDownload {
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebDownloadDelegate {
		[Export ("downloadWindowForAuthenticationSheet:"), DelegateName ("WebDownloadRequest"), DefaultValue (null)]
		NSWindow OnDownloadWindowForSheet (WebDownload download);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[Model]
	[Protocol (FormalSince = "10.11")]
	[BaseType (typeof (NSObject))]
	partial interface WebFrameLoadDelegate {
		[Export ("webView:didStartProvisionalLoadForFrame:"), EventArgs ("WebFrame")]
		void StartedProvisionalLoad (WebView sender, WebFrame forFrame);

		[Export ("webView:didReceiveServerRedirectForProvisionalLoadForFrame:"), EventArgs ("WebFrame")]
		void ReceivedServerRedirectForProvisionalLoad (WebView sender, WebFrame forFrame);

		[Export ("webView:didFailProvisionalLoadWithError:forFrame:"), EventArgs ("WebFrameError")]
		void FailedProvisionalLoad (WebView sender, NSError error, WebFrame forFrame);

		[Export ("webView:didCommitLoadForFrame:"), EventArgs ("WebFrame")]
		void CommitedLoad (WebView sender, WebFrame forFrame);

		[Export ("webView:didReceiveTitle:forFrame:"), EventArgs ("WebFrameTitle")]
		void ReceivedTitle (WebView sender, string title, WebFrame forFrame);

		[Export ("webView:didReceiveIcon:forFrame:"), EventArgs ("WebFrameImage")]
		void ReceivedIcon (WebView sender, NSImage image, WebFrame forFrame);

		[Export ("webView:didFinishLoadForFrame:"), EventArgs ("WebFrame")]
		void FinishedLoad (WebView sender, WebFrame forFrame);

		[Export ("webView:didFailLoadWithError:forFrame:"), EventArgs ("WebFrameError")]
		void FailedLoadWithError (WebView sender, NSError error, WebFrame forFrame);

		[Export ("webView:didChangeLocationWithinPageForFrame:"), EventArgs ("WebFrame")]
		void ChangedLocationWithinPage (WebView sender, WebFrame forFrame);

		[Export ("webView:willPerformClientRedirectToURL:delay:fireDate:forFrame:"), EventArgs ("WebFrameClientRedirect")]
		void WillPerformClientRedirect (WebView sender, NSUrl toUrl, double secondsDelay, NSDate fireDate, WebFrame forFrame);

		[Export ("webView:didCancelClientRedirectForFrame:"), EventArgs ("WebFrame")]
		void CanceledClientRedirect (WebView sender, WebFrame forFrame);

		[Export ("webView:willCloseFrame:"), EventArgs ("WebFrame")]
		void WillCloseFrame (WebView sender, WebFrame forFrame);

		[Export ("webView:didClearWindowObject:forFrame:"), EventArgs ("WebFrameScriptFrame")]
		void ClearedWindowObject (WebView webView, WebScriptObject windowObject, WebFrame forFrame);

		[Export ("webView:windowScriptObjectAvailable:"), EventArgs ("WebFrameScriptObject")]
		void WindowScriptObjectAvailable (WebView webView, WebScriptObject windowScriptObject);

		[Export ("webView:didCreateJavaScriptContext:forFrame:"), EventArgs ("WebFrameJavaScriptContext")]
		void DidCreateJavaScriptContext (WebView webView, JSContext context, WebFrame frame);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSView))]
	partial interface WebFrameView {
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
#if NET
	[Protocol, Model]
#else
	[Model (AutoGeneratedName = true)]
	[Protocol]
#endif
	partial interface WebOpenPanelResultListener {
#if NET
		[Abstract]
#endif
		[Export ("chooseFilename:")]
		void ChooseFilename (string filename);

#if NET
		[Abstract]
#endif
		[Export ("chooseFilenames:")]
		void ChooseFilenames (string [] filenames);

#if NET
		[Abstract]
#endif
		[Export ("cancel")]
		void Cancel ();
	}

	interface IWebOpenPanelResultListener { }

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebPolicyDelegate {
		[Export ("webView:decidePolicyForNavigationAction:request:frame:decisionListener:"), EventArgs ("WebNavigationPolicy")]
		void DecidePolicyForNavigation (WebView webView, NSDictionary actionInformation, NSUrlRequest request, WebFrame frame, NSObject decisionToken);

		[Export ("webView:decidePolicyForNewWindowAction:request:newFrameName:decisionListener:"), EventArgs ("WebNewWindowPolicy")]
		void DecidePolicyForNewWindow (WebView webView, NSDictionary actionInformation, NSUrlRequest request, string newFrameName, NSObject decisionToken);

		[Export ("webView:decidePolicyForMIMEType:request:frame:decisionListener:"), EventArgs ("WebMimeTypePolicy")]
		void DecidePolicyForMimeType (WebView webView, string mimeType, NSUrlRequest request, WebFrame frame, NSObject decisionToken);

		[Export ("webView:unableToImplementPolicyWithError:frame:"), EventArgs ("WebFailureToImplementPolicy")]
		void UnableToImplementPolicy (WebView webView, NSError error, WebFrame frame);

		[Field ("WebActionNavigationTypeKey")]
		NSString WebActionNavigationTypeKey { get; }

		[Field ("WebActionElementKey")]
		NSString WebActionElementKey { get; }

		[Field ("WebActionButtonKey")]
		NSString WebActionButtonKey { get; }

		[Field ("WebActionModifierFlagsKey")]
		NSString WebActionModifierFlagsKey { get; }

		[Field ("WebActionOriginalURLKey")]
		NSString WebActionOriginalUrlKey { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
#if NET
	[Protocol, Model]
#else
	[Model (AutoGeneratedName = true)]
	[Protocol]
#endif
	partial interface WebPolicyDecisionListener {
#if NET
		[Abstract]
#endif
		[Export ("use")]
		void Use ();

#if NET
		[Abstract]
#endif
		[Export ("download")]
		void Download ();

#if NET
		[Abstract]
#endif
		[Export ("ignore")]
		void Ignore ();
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

		[Export ("javaEnabled")]
		bool JavaEnabled { [Bind ("isJavaEnabled")] get; set; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebResourceLoadDelegate {
		[Export ("webView:identifierForInitialRequest:fromDataSource:"), DelegateName ("WebResourceIdentifierRequest"), DefaultValue (null)]
		NSObject OnIdentifierForInitialRequest (WebView sender, NSUrlRequest request, WebDataSource dataSource);

		[Export ("webView:resource:willSendRequest:redirectResponse:fromDataSource:"), DelegateName ("WebResourceOnRequestSend"), DefaultValueFromArgument ("request")]
		NSUrlRequest OnSendRequest (WebView sender, NSObject identifier, NSUrlRequest request, NSUrlResponse redirectResponse, WebDataSource dataSource);

		[Export ("webView:resource:didReceiveAuthenticationChallenge:fromDataSource:"), EventArgs ("WebResourceAuthenticationChallenge")]
		void OnReceivedAuthenticationChallenge (WebView sender, NSObject identifier, NSUrlAuthenticationChallenge challenge, WebDataSource dataSource);

		[Export ("webView:resource:didCancelAuthenticationChallenge:fromDataSource:"), EventArgs ("WebResourceCancelledChallenge")]
		void OnCancelledAuthenticationChallenge (WebView sender, NSObject identifier, NSUrlAuthenticationChallenge challenge, WebDataSource dataSource);

		[Export ("webView:resource:didReceiveResponse:fromDataSource:"), EventArgs ("WebResourceReceivedResponse")]
		void OnReceivedResponse (WebView sender, NSObject identifier, NSUrlResponse responseReceived, WebDataSource dataSource);

		[Export ("webView:resource:didReceiveContentLength:fromDataSource:"), EventArgs ("WebResourceReceivedContentLength")]
		void OnReceivedContentLength (WebView sender, NSObject identifier, nint length, WebDataSource dataSource);

		[Export ("webView:resource:didFinishLoadingFromDataSource:"), EventArgs ("WebResourceCompleted")]
		void OnFinishedLoading (WebView sender, NSObject identifier, WebDataSource dataSource);

		[Export ("webView:resource:didFailLoadingWithError:fromDataSource:"), EventArgs ("WebResourceError")]
		void OnFailedLoading (WebView sender, NSObject identifier, NSError withError, WebDataSource dataSource);

		[Export ("webView:plugInFailedWithError:dataSource:"), EventArgs ("WebResourcePluginError")]
		void OnPlugInFailed (WebView sender, NSError error, WebDataSource dataSource);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (FormalSince = "10.11")]
	partial interface WebUIDelegate {
		[Export ("webView:createWebViewWithRequest:"), DelegateName ("CreateWebViewFromRequest"), DefaultValue (null)]
		WebView UICreateWebView (WebView sender, NSUrlRequest request);

		[Export ("webViewShow:")]
		void UIShow (WebView sender);

		[Export ("webView:createWebViewModalDialogWithRequest:"), DelegateName ("WebViewCreate"), DefaultValue (null)]
		WebView UICreateModalDialog (WebView sender, NSUrlRequest request);

		[Export ("webViewRunModal:")]
		void UIRunModal (WebView sender);

		[Export ("webViewClose:")]
		void UIClose (WebView sender);

		[Export ("webViewFocus:")]
		void UIFocus (WebView sender);

		[Export ("webViewUnfocus:")]
		void UIUnfocus (WebView sender);

		[Export ("webViewFirstResponder:"), DelegateName ("WebViewGetResponder"), DefaultValue (null)]
		NSResponder UIGetFirstResponder (WebView sender);

		[Export ("webView:makeFirstResponder:"), EventArgs ("WebViewResponder")]
		void UIMakeFirstResponder (WebView sender, NSResponder newResponder);

		[Export ("webView:setStatusText:"), EventArgs ("WebViewStatusText")]
		void UISetStatusText (WebView sender, string text);

		[Export ("webViewStatusText:"), DelegateName ("WebViewGetString"), DefaultValue (null)]
		string UIGetStatusText (WebView sender);

		[Export ("webViewAreToolbarsVisible:"), DelegateName ("WebViewGetBool"), DefaultValue (null)]
		bool UIAreToolbarsVisible (WebView sender);

		[Export ("webView:setToolbarsVisible:"), EventArgs ("WebViewToolBars")]
		void UISetToolbarsVisible (WebView sender, bool visible);

		[Export ("webViewIsStatusBarVisible:"), DelegateName ("WebViewGetBool"), DefaultValue (false)]
		bool UIIsStatusBarVisible (WebView sender);

		[Export ("webView:setStatusBarVisible:"), EventArgs ("WebViewStatusBar")]
		void UISetStatusBarVisible (WebView sender, bool visible);

		[Export ("webViewIsResizable:"), DelegateName ("WebViewGetBool"), DefaultValue (null)]
		bool UIIsResizable (WebView sender);

		[Export ("webView:setResizable:"), EventArgs ("WebViewResizable")]
		void UISetResizable (WebView sender, bool resizable);

		[Export ("webView:setFrame:"), EventArgs ("WebViewFrame")]
		void UISetFrame (WebView sender, CGRect newFrame);

		[Export ("webViewFrame:"), DelegateName ("WebViewGetRectangle"), DefaultValue (null)]
		CGRect UIGetFrame (WebView sender);

		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:"), EventArgs ("WebViewJavaScriptFrame")]
		void UIRunJavaScriptAlertPanelMessage (WebView sender, string withMessage, WebFrame initiatedByFrame);

		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:"), DelegateName ("WebViewConfirmationPanel"), DefaultValue (null)]
		bool UIRunJavaScriptConfirmationPanel (WebView sender, string withMessage, WebFrame initiatedByFrame);

		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:"), DelegateName ("WebViewPromptPanel"), DefaultValue (null)]
		string UIRunJavaScriptTextInputPanelWithFrame (WebView sender, string prompt, string defaultText, WebFrame initiatedByFrame);

		[Export ("webView:runBeforeUnloadConfirmPanelWithMessage:initiatedByFrame:"), DelegateName ("WebViewJavaScriptFrame"), DefaultValue (null)]
		bool UIRunBeforeUnload (WebView sender, string message, WebFrame initiatedByFrame);

		[Export ("webView:runOpenPanelForFileButtonWithResultListener:"), EventArgs ("WebViewRunOpenPanel")]
		void UIRunOpenPanelForFileButton (WebView sender, IWebOpenPanelResultListener resultListener);

		[Export ("webView:mouseDidMoveOverElement:modifierFlags:"), EventArgs ("WebViewMouseMoved")]
		void UIMouseDidMoveOverElement (WebView sender, NSDictionary elementInformation, NSEventModifierMask modifierFlags);

		[Export ("webView:contextMenuItemsForElement:defaultMenuItems:"), DelegateName ("WebViewGetContextMenuItems"), DefaultValue (null)]
		NSMenuItem [] UIGetContextMenuItems (WebView sender, NSDictionary forElement, NSMenuItem [] defaultMenuItems);

		[Export ("webView:validateUserInterfaceItem:defaultValidation:"), DelegateName ("WebViewValidateUserInterface"), DefaultValueFromArgument ("defaultValidation")]
		bool UIValidateUserInterfaceItem (WebView webView, NSObject validatedUserInterfaceItem, bool defaultValidation);

#if !NET
		[Export ("webView:shouldPerformAction:fromSender:"), DelegateName ("WebViewPerformAction"), DefaultValue (null)]
		bool UIShouldPerformActionfromSender (WebView webView, Selector action, NSObject sender);
#else
		[Export ("webView:shouldPerformAction:fromSender:"), DelegateName("WebViewPerformAction"), DefaultValue (null)]
		bool UIShouldPerformAction (WebView webView, Selector action, NSObject sender);
#endif

		[Export ("webView:dragDestinationActionMaskForDraggingInfo:"), DelegateName ("DragDestinationGetActionMask"), DefaultValue (0)]
#if NET
		WebDragDestinationAction UIGetDragDestinationActionMask (WebView webView, INSDraggingInfo draggingInfo);
#else
		NSEventModifierMask UIGetDragDestinationActionMask (WebView webView, NSDraggingInfo draggingInfo);
#endif

		[Export ("webView:willPerformDragDestinationAction:forDraggingInfo:"), EventArgs ("WebViewDrag")]
#if NET
		void UIWillPerformDragDestination (WebView webView, WebDragDestinationAction action, INSDraggingInfo draggingInfo);
#else
		void UIWillPerformDragDestination (WebView webView, WebDragDestinationAction action, NSDraggingInfo draggingInfo);
#endif

		[Export ("webView:dragSourceActionMaskForPoint:"), DelegateName ("DragSourceGetActionMask"), DefaultValue (0)]
#if NET
		WebDragSourceAction UIDragSourceActionMask (WebView webView, CGPoint point);
#else
		NSEventModifierMask UIDragSourceActionMask (WebView webView, CGPoint point);
#endif

		[Export ("webView:willPerformDragSourceAction:fromPoint:withPasteboard:"), EventArgs ("WebViewPerformDrag")]
		void UIWillPerformDragSource (WebView webView, WebDragSourceAction action, CGPoint sourcePoint, NSPasteboard pasteboard);

		[Export ("webView:printFrameView:"), EventArgs ("WebViewPrint")]
		void UIPrintFrameView (WebView sender, WebFrameView frameView);

		[Export ("webViewHeaderHeight:"), DelegateName ("WebViewGetFloat"), DefaultValue (null)]
		float UIGetHeaderHeight (WebView sender); /* float, not CGFloat */

		[Export ("webViewFooterHeight:"), DelegateName ("WebViewGetFloat"), DefaultValue (null)]
		float UIGetFooterHeight (WebView sender); /* float, not CGFloat */

		[Export ("webView:drawHeaderInRect:"), EventArgs ("WebViewHeader")]
		void UIDrawHeaderInRect (WebView sender, CGRect rect);

		[Export ("webView:drawFooterInRect:"), EventArgs ("WebViewFooter")]
		void UIDrawFooterInRect (WebView sender, CGRect rect);

		[Export ("webView:runJavaScriptAlertPanelWithMessage:"), EventArgs ("WebViewJavaScript")]
		void UIRunJavaScriptAlertPanel (WebView sender, string message);

		[Export ("webView:runJavaScriptConfirmPanelWithMessage:"), DelegateName ("WebViewPrompt"), DefaultValue (null)]
		bool UIRunJavaScriptConfirmPanel (WebView sender, string message);

		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:"), DelegateName ("WebViewJavaScriptInput"), DefaultValue (null)]
		string UIRunJavaScriptTextInputPanel (WebView sender, string prompt, string defaultText);

		[Export ("webView:setContentRect:"), EventArgs ("WebViewContent")]
		void UISetContentRect (WebView sender, CGRect frame);

		[Export ("webViewContentRect:"), DelegateName ("WebViewGetRectangle"), DefaultValue (null)]
		CGRect UIGetContentRect (WebView sender);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (NSView),
		   Events = new Type [] {
			   typeof (WebFrameLoadDelegate),
			   typeof (WebDownloadDelegate),
			   typeof (WebResourceLoadDelegate),
			   typeof (WebUIDelegate),
			   typeof (WebPolicyDelegate) },
		   Delegates = new string [] {
			   "WeakFrameLoadDelegate",
			   "WeakDownloadDelegate",
			   "WeakResourceLoadDelegate",
			   "WeakUIDelegate",
			   "WeakPolicyDelegate" }
		   )]
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

		[Wrap ("WeakResourceLoadDelegate")]
		[Protocolize]
		WebResourceLoadDelegate ResourceLoadDelegate { get; set; }

		[Export ("downloadDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDownloadDelegate { get; set; }

		[Wrap ("WeakDownloadDelegate")]
		[Protocolize]
		WebDownloadDelegate DownloadDelegate { get; set; }

		[Export ("frameLoadDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakFrameLoadDelegate { get; set; }

		[Wrap ("WeakFrameLoadDelegate")]
		[Protocolize]
		WebFrameLoadDelegate FrameLoadDelegate { get; set; }

		[Export ("UIDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakUIDelegate { get; set; }

		[Wrap ("WeakUIDelegate")]
		[Protocolize]
		WebUIDelegate UIDelegate { get; set; }

		[Export ("policyDelegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakPolicyDelegate { get; set; }

		[Wrap ("WeakPolicyDelegate")]
		[Protocolize]
		WebPolicyDelegate PolicyDelegate { get; set; }

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
		[Export ("editable")]
		bool Editable { [Bind ("isEditable")] get; set; }

		[Export ("typingStyle")]
		DomCssStyleDeclaration TypingStyle { get; set; }

		[Export ("smartInsertDeleteEnabled")]
		bool SmartInsertDeleteEnabled { get; set; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMBlob")]
	[DisableDefaultCtor]
	partial interface DomBlob {
		[Export ("size")]
		ulong Size { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomBlob), Name = "DOMFile")]
	[DisableDefaultCtor]
	partial interface DomFile {
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomObject), Name = "DOMFileList")]
	[DisableDefaultCtor]
	partial interface DomFileList {
		[Export ("length")]
		uint Length { get; } /* unsigned int */

		[Export ("item:")]
		DomFile GetItem (int /* unsigned int */ index);
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBRElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBRElement {

		[Export ("clear")]
		string Clear { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLBaseElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlBaseElement {

		[Export ("href")]
		string HRef { get; set; }

		[Export ("target")]
		string Target { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDListElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDListElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDirectoryElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDirectoryElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLDivElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlDivElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFieldSetElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFieldSetElement {

		[Export ("form", ArgumentSemantic.Retain)]
		DomHtmlFormElement Form { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLFrameSetElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlFrameSetElement {

		[Export ("cols")]
		string Cols { get; set; }

		[Export ("rows")]
		string Rows { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHeadElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHeadElement {

		[Export ("profile")]
		string Profile { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHeadingElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHeadingElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLHtmlElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlHtmlElement {

		[Export ("version")]
		string Version { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLLIElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlLIElement {

		[Export ("type")]
		string Type { get; set; }

		[Export ("value")]
		int Value { get; set; } /* int, not NSInteger */
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMapElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMapElement {

		[Export ("areas", ArgumentSemantic.Retain)]
		DomHtmlCollection Areas { get; }

		[Export ("name")]
		string Name { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMarqueeElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMarqueeElement {

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLMenuElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlMenuElement {

		[Export ("compact")]
		bool Compact { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLModElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlModElement {

		[Export ("cite")]
		string Cite { get; set; }

		[Export ("dateTime")]
		string DateTime { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLOptGroupElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlOptGroupElement {

		[Export ("disabled")]
		bool Disabled { get; set; }

		[Export ("label")]
		string Label { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLParagraphElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlParagraphElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLPreElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlPreElement {

		[Export ("width")]
		int Width { get; set; } /* int, not NSInteger */

		[Export ("wrap")]
		bool Wrap { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLQuoteElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlQuoteElement {

		[Export ("cite")]
		string Cite { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "No longer supported.")]
	[BaseType (typeof (DomHtmlElement), Name = "DOMHTMLTableCaptionElement")]
	[DisableDefaultCtor] // ObjCException: +[<TYPE> init]: should never be used
	interface DomHtmlTableCaptionElement {

		[Export ("align")]
		string Align { get; set; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum WKFullscreenState : long {
		NotInFullscreen,
		EnteringFullscreen,
		InFullscreen,
		ExitingFullscreen,
	}

	[iOS (16, 0), MacCatalyst (16, 0), Mac (13, 0)]
	[Native]
	public enum WKDialogResult : long {
		ShowDefault = 1,
		AskAgain,
		Handled,
	}

	[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Native]
	public enum WKCookiePolicy : long {
		Allow,
		Disallow,
	}

	[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[Native]
	public enum WKInactiveSchedulingPolicy : long {
		Suspend,
		Throttle,
		None,
	}

	[NoiOS, Mac (14, 0), NoMacCatalyst]
	[Native]
	public enum WKUserInterfaceDirectionPolicy : long {
		Content,
		System,
	}

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

		[Export ("itemAtIndex:")]
		[return: NullAllowed]
		WKBackForwardListItem ItemAtIndex (nint index);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKContentRuleList {
		[Export ("identifier")]
		string Identifier { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKContentRuleListStore {
		[Static]
		[Export ("defaultStore")]
		WKContentRuleListStore DefaultStore { get; }

		[Static]
		[Export ("storeWithURL:")]
		WKContentRuleListStore FromUrl (NSUrl url);

		[Export ("compileContentRuleListForIdentifier:encodedContentRuleList:completionHandler:")]
		[Async]
		void CompileContentRuleList (string identifier, string encodedContentRuleList, Action<WKContentRuleList, NSError> completionHandler);

		[Export ("lookUpContentRuleListForIdentifier:completionHandler:")]
		[Async]
		void LookUpContentRuleList (string identifier, Action<WKContentRuleList, NSError> completionHandler);

		[Export ("removeContentRuleListForIdentifier:completionHandler:")]
		[Async]
		void RemoveContentRuleList (string identifier, Action<NSError> completionHandler);

		[Export ("getAvailableContentRuleListIdentifiers:")]
		[Async]
		void GetAvailableContentRuleListIdentifiers (Action<string []> callback);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "WKHTTPCookieStore")]
	[DisableDefaultCtor]
	interface WKHttpCookieStore {
		[Export ("getAllCookies:")]
		[Async]
		void GetAllCookies (Action<NSHttpCookie []> completionHandler);

		[Export ("setCookie:completionHandler:")]
		[Async]
		void SetCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);

		[Export ("deleteCookie:completionHandler:")]
		[Async]
		void DeleteCookie (NSHttpCookie cookie, [NullAllowed] Action completionHandler);

		[Export ("addObserver:")]
		void AddObserver (IWKHttpCookieStoreObserver observer);

		[Export ("removeObserver:")]
		void RemoveObserver (IWKHttpCookieStoreObserver observer);

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("setCookiePolicy:completionHandler:")]
		[Async]
		void SetCookiePolicy (WKCookiePolicy policy, [NullAllowed] Action completionHandler);

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("getCookiePolicy:")]
		[Async]
		void GetCookiePolicy (Action<WKCookiePolicy> completionHandler);
	}

	interface IWKHttpCookieStoreObserver { }

	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKHTTPCookieStoreObserver")]
	interface WKHttpCookieStoreObserver {
		[Export ("cookiesDidChangeInCookieStore:")]
		void CookiesDidChangeInCookieStore (WKHttpCookieStore cookieStore);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKFrameInfo : NSCopying {

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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKNavigation {

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("effectiveContentMode")]
		WKContentMode EffectiveContentMode { get; }
	}

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

		[NoiOS]
		[NoMacCatalyst]
		[Export ("modifierFlags")]
		NSEventModifierMask ModifierFlags { get; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("buttonNumber")]
		nint ButtonNumber { get; }

		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("shouldPerformDownload")]
		bool ShouldPerformDownload { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKNavigationDelegate {

		[Export ("webView:decidePolicyForNavigationAction:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler);

		[Export ("webView:decidePolicyForNavigationResponse:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("webView:decidePolicyForNavigationAction:preferences:decisionHandler:")]
		void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, WKWebpagePreferences preferences, Action<WKNavigationActionPolicy, WKWebpagePreferences> decisionHandler);

		[Export ("webView:didStartProvisionalNavigation:")]
		void DidStartProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didReceiveServerRedirectForProvisionalNavigation:")]
		void DidReceiveServerRedirectForProvisionalNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFailProvisionalNavigation:withError:")]
		void DidFailProvisionalNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		[Export ("webView:didCommitNavigation:")]
		void DidCommitNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFinishNavigation:")]
		void DidFinishNavigation (WKWebView webView, WKNavigation navigation);

		[Export ("webView:didFailNavigation:withError:")]
		void DidFailNavigation (WKWebView webView, WKNavigation navigation, NSError error);

		[Export ("webView:didReceiveAuthenticationChallenge:completionHandler:")]
		void DidReceiveAuthenticationChallenge (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("webViewWebContentProcessDidTerminate:")]
		void ContentProcessDidTerminate (WKWebView webView);

		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("webView:authenticationChallenge:shouldAllowDeprecatedTLS:")]
		void ShouldAllowDeprecatedTls (WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<bool> decisionHandler);

		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("webView:navigationAction:didBecomeDownload:")]
		void NavigationActionDidBecomeDownload (WKWebView webView, WKNavigationAction navigationAction, WKDownload download);

		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("webView:navigationResponse:didBecomeDownload:")]
		void NavigationResponseDidBecomeDownload (WKWebView webView, WKNavigationResponse navigationResponse, WKDownload download);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKNavigationResponse {

		[Export ("forMainFrame")]
		bool IsForMainFrame { [Bind ("isForMainFrame")] get; }

		[Export ("response", ArgumentSemantic.Copy)]
		NSUrlResponse Response { get; }

		[Export ("canShowMIMEType")]
		bool CanShowMimeType { get; }
	}

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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("fraudulentWebsiteWarningEnabled")]
		bool FraudulentWebsiteWarningEnabled { [Bind ("isFraudulentWebsiteWarningEnabled")] get; set; }

		[Mac (13, 3), iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("shouldPrintBackgrounds")]
		bool ShouldPrintBackgrounds { get; set; }

		[Internal]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("textInteractionEnabled")]
		bool _OldTextInteractionEnabled { get; set; }

		[Internal]
		[Mac (12, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Export ("isTextInteractionEnabled")]
		bool _NewGetTextInteractionEnabled ();

		[Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("siteSpecificQuirksModeEnabled")]
		bool SiteSpecificQuirksModeEnabled { [Bind ("isSiteSpecificQuirksModeEnabled")] get; set; }

		[Mac (12, 3), iOS (15, 4), MacCatalyst (15, 4)]
		[Export ("elementFullscreenEnabled")]
		bool ElementFullscreenEnabled { [Bind ("isElementFullscreenEnabled")] get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("inactiveSchedulingPolicy", ArgumentSemantic.Assign)]
		WKInactiveSchedulingPolicy InactiveSchedulingPolicy { get; set; }
	}

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

		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("world")]
		WKContentWorld World { get; }
	}

	interface IWKScriptMessageHandler { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKScriptMessageHandler {

		[Export ("userContentController:didReceiveScriptMessage:")]
		[Abstract]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message);
	}

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


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKSnapshotConfiguration : NSCopying {
		[Export ("rect")]
		CGRect Rect { get; set; }

		[Export ("snapshotWidth")]
		[NullAllowed]
		NSNumber SnapshotWidth { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("afterScreenUpdates")]
		bool AfterScreenUpdates { get; set; }
	}

	interface IWKUrlSchemeHandler { }
	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKURLSchemeHandler")]
	interface WKUrlSchemeHandler {
		[Abstract]
		[Export ("webView:startURLSchemeTask:")]
		void StartUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);

		[Abstract]
		[Export ("webView:stopURLSchemeTask:")]
		void StopUrlSchemeTask (WKWebView webView, IWKUrlSchemeTask urlSchemeTask);
	}

	interface IWKUrlSchemeTask { }

	[MacCatalyst (13, 1)]
	[Protocol (Name = "WKURLSchemeTask")]
	interface WKUrlSchemeTask {
		[Abstract]
		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[Abstract]
		[Export ("didReceiveResponse:")]
		void DidReceiveResponse (NSUrlResponse response);

		[Abstract]
		[Export ("didReceiveData:")]
		void DidReceiveData (NSData data);

		[Abstract]
		[Export ("didFinish")]
		void DidFinish ();

		[Abstract]
		[Export ("didFailWithError:")]
		void DidFailWithError (NSError error);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebsiteDataRecord {
		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("dataTypes", ArgumentSemantic.Copy)]
		NSSet<NSString> DataTypes { get; }
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface WKWebsiteDataType {
		[Field ("WKWebsiteDataTypeDiskCache", "WebKit")]
		NSString DiskCache { get; }

		[Field ("WKWebsiteDataTypeMemoryCache", "WebKit")]
		NSString MemoryCache { get; }

		[Field ("WKWebsiteDataTypeOfflineWebApplicationCache", "WebKit")]
		NSString OfflineWebApplicationCache { get; }

		[Field ("WKWebsiteDataTypeCookies", "WebKit")]
		NSString Cookies { get; }

		[Field ("WKWebsiteDataTypeSessionStorage")]
		NSString SessionStorage { get; }

		[Field ("WKWebsiteDataTypeLocalStorage", "WebKit")]
		NSString LocalStorage { get; }

		[Field ("WKWebsiteDataTypeWebSQLDatabases", "WebKit")]
		NSString WebSQLDatabases { get; }

		[Field ("WKWebsiteDataTypeIndexedDBDatabases", "WebKit")]
		NSString IndexedDBDatabases { get; }

		[iOS (11, 3)]
		[MacCatalyst (13, 1)]
		[Field ("WKWebsiteDataTypeFetchCache")]
		NSString FetchCache { get; }

		[iOS (11, 3)]
		[MacCatalyst (13, 1)]
		[Field ("WKWebsiteDataTypeServiceWorkerRegistrations")]
		NSString ServiceWorkerRegistrations { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Field ("WKWebsiteDataTypeFileSystem")]
		NSString FileSystem { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeSearchFieldRecentSearches")]
		NSString SearchFieldRecentSearches { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeMediaKeys")]
		NSString MediaKeys { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Field ("WKWebsiteDataTypeHashSalt")]
		NSString HashSalt { get; }
	}

	[NoiOS, NoMacCatalyst, Mac (14, 0)]
	[Static]
	interface WebViewNotification {
		[Field ("WebViewDidBeginEditingNotification")]
		NSString DidBeginEditing { get; }

		[Field ("WebViewDidChangeNotification")]
		NSString DidChange { get; }

		[Field ("WebViewDidEndEditingNotification")]
		NSString DidEndEditing { get; }

		[Field ("WebViewDidChangeTypingStyleNotification")]
		NSString DidChangeTypingStyle { get; }

		[Field ("WebViewDidChangeSelectionNotification")]
		NSString DidChangeSelection { get; }
	}

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

		[Export ("persistent")]
		bool Persistent { [Bind ("isPersistent")] get; }

		[Static]
		[Export ("allWebsiteDataTypes")]
		NSSet<NSString> AllWebsiteDataTypes { get; }

		[Export ("fetchDataRecordsOfTypes:completionHandler:")]
		[Async]
		void FetchDataRecordsOfTypes (NSSet<NSString> dataTypes, Action<NSArray> completionHandler);

		[Export ("removeDataOfTypes:forDataRecords:completionHandler:")]
		[Async]
		void RemoveDataOfTypes (NSSet<NSString> dataTypes, WKWebsiteDataRecord [] dataRecords, Action completionHandler);

		[Export ("removeDataOfTypes:modifiedSince:completionHandler:")]
		[Async]
		void RemoveDataOfTypes (NSSet<NSString> websiteDataTypes, NSDate date, Action completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("httpCookieStore")]
		WKHttpCookieStore HttpCookieStore { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("identifier")]
		NSUuid Identifier { get; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("dataStoreForIdentifier:")]
		WKWebsiteDataStore Create (NSUuid identifier);

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Static]
		[Async]
		[Export ("removeDataStoreForIdentifier:completionHandler:")]
		void Remove (NSUuid identifier, Action<NSError> completionHandler);

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Static]
		[Async]
		[Export ("fetchAllDataStoreIdentifiers:")]
		void FetchAllDataStoreIdentifiers (Action<NSArray<NSUuid>> completionHandler);
	}

	[NoiOS, NoWatch, NoTV]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface WKOpenPanelParameters {
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; }

		[NoMacCatalyst]
		[Export ("allowsDirectories")]
		bool AllowsDirectories { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface WKUIDelegate {

		[Export ("webView:createWebViewWithConfiguration:forNavigationAction:windowFeatures:")]
		[return: NullAllowed]
		WKWebView CreateWebView (WKWebView webView, WKWebViewConfiguration configuration,
			WKNavigationAction navigationAction, WKWindowFeatures windowFeatures);

		[Export ("webView:runJavaScriptAlertPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptAlertPanel (WKWebView webView, string message, WKFrameInfo frame, Action completionHandler);

		[Export ("webView:runJavaScriptConfirmPanelWithMessage:initiatedByFrame:completionHandler:")]
		void RunJavaScriptConfirmPanel (WKWebView webView, string message, WKFrameInfo frame, Action<bool> completionHandler);

		[Export ("webView:runJavaScriptTextInputPanelWithPrompt:defaultText:initiatedByFrame:completionHandler:")]
		void RunJavaScriptTextInputPanel (WKWebView webView, string prompt, [NullAllowed] string defaultText,
			WKFrameInfo frame, Action<string> completionHandler);

		[NoiOS, NoWatch, NoTV]
		[NoMacCatalyst]
		[Export ("webView:runOpenPanelWithParameters:initiatedByFrame:completionHandler:")]
		void RunOpenPanel (WKWebView webView, WKOpenPanelParameters parameters, WKFrameInfo frame, Action<NSUrl []> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("webViewDidClose:")]
		void DidClose (WKWebView webView);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Export ("webView:shouldPreviewElement:")]
		bool ShouldPreviewElement (WKWebView webView, WKPreviewElementInfo elementInfo);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SetContextMenuConfiguration' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SetContextMenuConfiguration' instead.")]
		[MacCatalyst (13, 1)]
		[Export ("webView:previewingViewControllerForElement:defaultActions:")]
		[return: NullAllowed]
		UIViewController GetPreviewingViewController (WKWebView webView, WKPreviewElementInfo elementInfo, IWKPreviewActionItem [] previewActions);

		[NoMac]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WillCommitContextMenu' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillCommitContextMenu' instead.")]
		[Export ("webView:commitPreviewingViewController:")]
		void CommitPreviewingViewController (WKWebView webView, UIViewController previewingViewController);

		[MacCatalyst (13, 1)]
		[iOS (13, 0)]
		[NoMac]
		[Export ("webView:contextMenuConfigurationForElement:completionHandler:")]
		void SetContextMenuConfiguration (WKWebView webView, WKContextMenuElementInfo elementInfo, Action<UIContextMenuConfiguration> completionHandler);

		[MacCatalyst (13, 1)]
		[iOS (13, 0)]
		[NoMac]
		[Export ("webView:contextMenuForElement:willCommitWithAnimator:")]
		void WillCommitContextMenu (WKWebView webView, WKContextMenuElementInfo elementInfo, IUIContextMenuInteractionCommitAnimating animator);

		[iOS (13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("webView:contextMenuWillPresentForElement:")]
		void ContextMenuWillPresent (WKWebView webView, WKContextMenuElementInfo elementInfo);

		[iOS (13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("webView:contextMenuDidEndForElement:")]
		void ContextMenuDidEnd (WKWebView webView, WKContextMenuElementInfo elementInfo);

		[Async]
		[NoMac, NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("webView:requestDeviceOrientationAndMotionPermissionForOrigin:initiatedByFrame:decisionHandler:")]
		void RequestDeviceOrientationAndMotionPermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, Action<WKPermissionDecision> decisionHandler);

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("webView:requestMediaCapturePermissionForOrigin:initiatedByFrame:type:decisionHandler:")]
		void RequestMediaCapturePermission (WKWebView webView, WKSecurityOrigin origin, WKFrameInfo frame, WKMediaCaptureType type, Action<WKPermissionDecision> decisionHandler);

		[Async]
		[NoMac, iOS (16, 0), MacCatalyst (16, 0)] // headers say 13, is not true since the enum is from 16
		[Export ("webView:showLockdownModeFirstUseMessage:completionHandler:")]
		void ShowLockDownMode (WKWebView webView, string firstUseMessage, Action<WKDialogResult> completionHandler);

		[NoMac, iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("webView:willPresentEditMenuWithAnimator:")]
		void WillPresentEditMenu (WKWebView webView, IUIEditMenuInteractionAnimating animator);

		[NoMac, iOS (16, 4), MacCatalyst (16, 4)]
		[Export ("webView:willDismissEditMenuWithAnimator:")]
		void WillDismissEditMenu (WKWebView webView, IUIEditMenuInteractionAnimating animator);
	}

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
		void AddScriptMessageHandler ([Protocolize] WKScriptMessageHandler scriptMessageHandler, string name);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addScriptMessageHandler:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandler scriptMessageHandler, WKContentWorld world, string name);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addScriptMessageHandlerWithReply:contentWorld:name:")]
		void AddScriptMessageHandler (IWKScriptMessageHandlerWithReply scriptMessageHandlerWithReply, WKContentWorld contentWorld, string name);

		[Export ("removeScriptMessageHandlerForName:")]
		void RemoveScriptMessageHandler (string name);

		[Mac (11, 0), iOS (14, 0)]
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

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeAllScriptMessageHandlersFromContentWorld:")]
		void RemoveAllScriptMessageHandlers (WKContentWorld contentWorld);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("removeAllScriptMessageHandlers")]
		void RemoveAllScriptMessageHandlers ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // all properties are getters
	interface WKUserScript : NSCopying {

		[Export ("initWithSource:injectionTime:forMainFrameOnly:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithSource:injectionTime:forMainFrameOnly:inContentWorld:")]
		NativeHandle Constructor (NSString source, WKUserScriptInjectionTime injectionTime, bool isForMainFrameOnly, WKContentWorld contentWorld);

		[Export ("source", ArgumentSemantic.Copy)]
		NSString Source { get; }

		[Export ("injectionTime")]
		WKUserScriptInjectionTime InjectionTime { get; }

		[Export ("forMainFrameOnly")]
		bool IsForMainFrameOnly { [Bind ("isForMainFrameOnly")] get; }
	}

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

		[Wrap ("WeakNavigationDelegate")]
		[Protocolize]
		WKNavigationDelegate NavigationDelegate { get; set; }

		[Export ("UIDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakUIDelegate { get; set; }

		[Wrap ("WeakUIDelegate")]
		[Protocolize]
		WKUIDelegate UIDelegate { get; set; }

		[Export ("backForwardList", ArgumentSemantic.Strong)]
		WKBackForwardList BackForwardList { get; }

		[Export ("title")]
		[NullAllowed]
		string Title { get; }

		[Export ("URL", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSUrl Url { get; }

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

		[Export ("loadRequest:")]
		[return: NullAllowed]
		WKNavigation LoadRequest (NSUrlRequest request);

		[Export ("loadHTMLString:baseURL:")]
		[return: NullAllowed]
		WKNavigation LoadHtmlString (NSString htmlString, [NullAllowed] NSUrl baseUrl);

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
		[Async]
		void EvaluateJavaScript (NSString javascript, [NullAllowed] WKJavascriptEvaluationResult completionHandler);

		[Wrap ("EvaluateJavaScript ((NSString)javascript, completionHandler)")]
		[Async]
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
		[Async]
		[Export ("takeSnapshotWithConfiguration:completionHandler:")]
		void TakeSnapshot ([NullAllowed] WKSnapshotConfiguration snapshotConfiguration, Action<UIImage, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("handlesURLScheme:")]
		bool HandlesUrlScheme (string urlScheme);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("evaluateJavaScript:inFrame:inContentWorld:completionHandler:")]
		void EvaluateJavaScript (string javaScriptString, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("callAsyncJavaScript:arguments:inFrame:inContentWorld:completionHandler:")]
		void CallAsyncJavaScript (string functionBody, [NullAllowed] NSDictionary<NSString, NSObject> arguments, [NullAllowed] WKFrameInfo frame, WKContentWorld contentWorld, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("createPDFWithConfiguration:completionHandler:")]
		void CreatePdf ([NullAllowed] WKPdfConfiguration pdfConfiguration, Action<NSData, NSError> completionHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("createWebArchiveDataWithCompletionHandler:")]
		void CreateWebArchive (Action<NSData, NSError> completionHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Async]
		[Export ("findString:withConfiguration:completionHandler:")]
		void Find (string @string, [NullAllowed] WKFindConfiguration configuration, Action<WKFindResult> completionHandler);

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("mediaType")]
		string MediaType { get; set; }

		[Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("pageZoom")]
		nfloat PageZoom { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Mac (11, 0)]
		[Export ("printOperationWithPrintInfo:")]
		NSPrintOperation GetPrintOperation (NSPrintInfo printInfo);

		// Apple renamed those API since Xcode 12.5
		[Internal]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("closeAllMediaPresentations")]
		void _OldCloseAllMediaPresentations ();

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("closeAllMediaPresentationsWithCompletionHandler:")]
		void CloseAllMediaPresentations ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("pauseAllMediaPlayback:")]
		void _OldPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("pauseAllMediaPlaybackWithCompletionHandler:")]
		void _NewPauseAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("suspendAllMediaPlayback:")]
		void _OldSuspendAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Internal]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("resumeAllMediaPlayback:")]
		void _OldResumeAllMediaPlayback ([NullAllowed] Action completionHandler);

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setAllMediaPlaybackSuspended:completionHandler:")]
		void SetAllMediaPlaybackSuspended (bool suspended, [NullAllowed] Action completionHandler);

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("requestMediaPlaybackStateWithCompletionHandler:")]
		void RequestMediaPlaybackState (Action<WKMediaPlaybackState> completionHandler);

		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("startDownloadUsingRequest:completionHandler:")]
		void StartDownload (NSUrlRequest request, Action<WKDownload> completionHandler);

		[Mac (11, 3)]
		[iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Async]
		[Export ("resumeDownloadFromResumeData:completionHandler:")]
		void ResumeDownload (NSData resumeData, Action<WKDownload> completionHandler);

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("cameraCaptureState")]
		WKMediaCaptureState CameraCaptureState { get; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[NullAllowed, Export ("interactionState", ArgumentSemantic.Copy)]
		NSObject InteractionState { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadFileRequest:allowingReadAccessToURL:")]
		WKNavigation LoadFileRequest (NSUrlRequest request, NSUrl readAccessURL);

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadSimulatedRequest:response:responseData:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, NSUrlResponse response, NSData data);

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("loadSimulatedRequest:responseHTMLString:")]
		WKNavigation LoadSimulatedRequest (NSUrlRequest request, string htmlString);

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("microphoneCaptureState")]
		WKMediaCaptureState MicrophoneCaptureState { get; }

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setCameraCaptureState:completionHandler:")]
		void SetCameraCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);

		[Async]
		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("setMicrophoneCaptureState:completionHandler:")]
		void SetMicrophoneCaptureState (WKMediaCaptureState state, [NullAllowed] Action completionHandler);

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoTV]
		[Export ("themeColor")]
		[NullAllowed]
		UIColor ThemeColor { get; }

		[iOS (15, 0), MacCatalyst (15, 0), Mac (12, 0), NoTV]
		[NullAllowed, Export ("underPageBackgroundColor", ArgumentSemantic.Copy)]
		UIColor UnderPageBackgroundColor { get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), Mac (13, 0), NoTV]
		[Export ("fullscreenState")]
		WKFullscreenState FullscreenState { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("minimumViewportInset")]
		UIEdgeInsets MinimumViewportInset { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("maximumViewportInset")]
		UIEdgeInsets MaximumViewportInset { get; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("setMinimumViewportInset:maximumViewportInset:")]
		void SetViewportInsets (UIEdgeInsets minimumViewportInset, UIEdgeInsets maximumViewportInset);

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoWatch, NoTV]
		[Export ("findInteractionEnabled")]
		bool FindInteractionEnabled { [Bind ("isFindInteractionEnabled")] get; set; }

		[iOS (16, 0), MacCatalyst (16, 0), NoMac, NoWatch, NoTV]
		[Export ("findInteraction")]
		[NullAllowed]
		UIFindInteraction FindInteraction { get; }

		[Mac (13, 3), MacCatalyst (16, 4), iOS (16, 4), NoWatch, NoTV]
		[Export ("inspectable")]
		bool Inspectable { [Bind ("isInspectable")] get; set; }
	}

	delegate void WKJavascriptEvaluationResult (NSObject result, NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebViewConfiguration : NSCopying, NSSecureCoding {

		[Export ("processPool", ArgumentSemantic.Retain)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("defaultWebpagePreferences", ArgumentSemantic.Copy)]
		[NullAllowed]
		WKWebpagePreferences DefaultWebpagePreferences { get; set; }

		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("limitsNavigationsToAppBoundDomains")]
		bool LimitsNavigationsToAppBoundDomains { get; set; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0), NoTV]
		[Export ("upgradeKnownHostsToHTTPS")]
		bool UpgradeKnownHostsToHttps { get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("allowsInlinePredictions")]
		bool AllowsInlinePredictions { get; set; }

		[NoiOS, Mac (14, 0)]
		[Export ("userInterfaceDirectionPolicy", ArgumentSemantic.Assign)]
		WKUserInterfaceDirectionPolicy UserInterfaceDirectionPolicy { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKProcessPool : NSSecureCoding {
		// as of Mac 10.10, iOS 8.0 Beta 2,
		// this interface is completely empty
	}

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
	interface UIPreviewActionItem {}
#endif

	interface IWKPreviewActionItem { }

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TBD' instead.")]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface WKPreviewActionItem : UIPreviewActionItem {
		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSString Identifier { get; }
	}

	[NoMac]
	[Static]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TBD' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TBD' instead.")]
	interface WKPreviewActionItemIdentifier {
		[Field ("WKPreviewActionItemIdentifierOpen")]
		NSString Open { get; }

		[Field ("WKPreviewActionItemIdentifierAddToReadingList")]
		NSString AddToReadingList { get; }

		[Field ("WKPreviewActionItemIdentifierCopy")]
		NSString Copy { get; }

		[Field ("WKPreviewActionItemIdentifierShare")]
		NSString Share { get; }
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'WKContextMenuElementInfo' instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WKContextMenuElementInfo' instead.")]
	[BaseType (typeof (NSObject))]
	interface WKPreviewElementInfo : NSCopying {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	public enum WKContentMode : long {
		Recommended,
		Mobile,
		Desktop,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface WKWebpagePreferences {

		[Export ("preferredContentMode", ArgumentSemantic.Assign)]
		WKContentMode PreferredContentMode { get; set; }

		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("allowsContentJavaScript")]
		bool AllowsContentJavaScript { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0), NoWatch, NoTV]
		[Export ("lockdownModeEnabled")]
		bool LockdownModeEnabled { [Bind ("isLockdownModeEnabled")] get; set; }
	}

	[NoMac]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKContextMenuElementInfo {
		[NullAllowed, Export ("linkURL")]
		NSUrl LinkUrl { get; }
	}

	[Mac (11, 0)]
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

		[NullAllowed, Export ("name")]
		string Name { get; }
	}

	[Mac (11, 0)]
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

	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface WKFindResult : NSCopying {

		[Export ("matchFound")]
		bool MatchFound { get; }
	}

	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject), Name = "WKPDFConfiguration")]
	interface WKPdfConfiguration : NSCopying {

		[Export ("rect", ArgumentSemantic.Assign)]
		CGRect Rect { get; set; }

		[iOS (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export ("allowTransparentBackground")]
		bool AllowTransparentBackground { get; set; }
	}

	interface IWKScriptMessageHandlerWithReply { }

	[Mac (11, 0)]
	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface WKScriptMessageHandlerWithReply {

		[Abstract]
		[Export ("userContentController:didReceiveScriptMessage:replyHandler:")]
		void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message, Action<NSObject, NSString> replyHandler);
	}

	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
	[Native]
	enum WKDownloadRedirectPolicy : long {
		Cancel,
		Allow,
	}

	[Mac (11, 3)]
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

	[Mac (11, 3)]
	[iOS (14, 5)]
	[MacCatalyst (14, 5)]
#if NET
	[Protocol, Model]
#else
	[Protocol, Model (AutoGeneratedName = true)]
#endif
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
	}

	[Mac (11, 3)]
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
	}
}
