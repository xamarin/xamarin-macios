//
// Copyright 2011, Novell, Inc.
// Copyright 2011, Regan Sarwas
//
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
// PdfKit.cs: Bindings for the PdfKit API
//
#if MONOMAC || (IOS && XAMCORE_2_0)

using System;
#if MONOMAC
using XamCore.AppKit;
#else
using XamCore.UIKit;
using NSColor = XamCore.UIKit.UIColor;
using NSFont = XamCore.UIKit.UIFont;
using NSImage = XamCore.UIKit.UIImage;
using NSView = XamCore.UIKit.UIView;
using NSEdgeInsets = XamCore.UIKit.UIEdgeInsets;
using NSEvent = XamCore.UIKit.UIEvent;
using NSBezierPath = XamCore.UIKit.UIBezierPath;
using NSTextAlignment = XamCore.UIKit.UITextAlignment;
// HACK: to make intermediate dll build
using NSPrintInfo = XamCore.Foundation.NSObject;
using NSPrintOperation = XamCore.Foundation.NSObject;
#endif
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.CoreGraphics;

// Verify/Test Delegate Models
// Check for missing NullAllowed on all object properties
// Test methods returning typed arrays in lieu of NSArray
// Check classes with no public inits - Should I make the constructors private?
// Check the few abnormal properties

namespace XamCore.PdfKit {

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationKey {

		[Field ("PDFAnnotationKeyAppearanceDictionary", "+PDFKit")]
		AppearanceDictionary,

		[Field ("PDFAnnotationKeyAppearanceState", "+PDFKit")]
		AppearanceState,

		[Field ("PDFAnnotationKeyBorder", "+PDFKit")]
		Border,

		[Field ("PDFAnnotationKeyColor", "+PDFKit")]
		Color,

		[Field ("PDFAnnotationKeyContents", "+PDFKit")]
		Contents,

		[Field ("PDFAnnotationKeyFlags", "+PDFKit")]
		Flags,

		[Field ("PDFAnnotationKeyDate", "+PDFKit")]
		Date,

		[Field ("PDFAnnotationKeyName", "+PDFKit")]
		Name,

		[Field ("PDFAnnotationKeyPage", "+PDFKit")]
		Page,

		[Field ("PDFAnnotationKeyRect", "+PDFKit")]
		Rect,

		[Field ("PDFAnnotationKeySubtype", "+PDFKit")]
		Subtype,

		[Field ("PDFAnnotationKeyAction", "+PDFKit")]
		Action,

		[Field ("PDFAnnotationKeyAdditionalActions", "+PDFKit")]
		AdditionalActions,

		[Field ("PDFAnnotationKeyBorderStyle", "+PDFKit")]
		BorderStyle,

		[Field ("PDFAnnotationKeyDefaultAppearance", "+PDFKit")]
		DefaultAppearance,

		[Field ("PDFAnnotationKeyDestination", "+PDFKit")]
		Destination,

		[Field ("PDFAnnotationKeyHighlightingMode", "+PDFKit")]
		HighlightingMode,

		[Field ("PDFAnnotationKeyInklist", "+PDFKit")]
		Inklist,

		[Field ("PDFAnnotationKeyInteriorColor", "+PDFKit")]
		InteriorColor,

		[Field ("PDFAnnotationKeyLinePoints", "+PDFKit")]
		LinePoints,

		[Field ("PDFAnnotationKeyLineEndingStyles", "+PDFKit")]
		LineEndingStyles,

		[Field ("PDFAnnotationKeyIconName", "+PDFKit")]
		IconName,

		[Field ("PDFAnnotationKeyOpen", "+PDFKit")]
		Open,

		[Field ("PDFAnnotationKeyParent", "+PDFKit")]
		Parent,

		[Field ("PDFAnnotationKeyPopup")]
		Popup,

		[Field ("PDFAnnotationKeyQuadding", "+PDFKit")]
		Quadding,

		[Field ("PDFAnnotationKeyQuadPoints", "+PDFKit")]
		QuadPoints,

		[Field ("PDFAnnotationKeyTextLabel", "+PDFKit")]
		TextLabel,

		[Field ("PDFAnnotationKeyWidgetDownCaption", "+PDFKit")]
		WidgetDownCaption,

		[Field ("PDFAnnotationKeyWidgetBorderColor", "+PDFKit")]
		WidgetBorderColor,

		[Field ("PDFAnnotationKeyWidgetBackgroundColor", "+PDFKit")]
		WidgetBackgroundColor,

		[Field ("PDFAnnotationKeyWidgetCaption", "+PDFKit")]
		WidgetCaption,

		[Field ("PDFAnnotationKeyWidgetDefaultValue", "+PDFKit")]
		WidgetDefaultValue,

		[Field ("PDFAnnotationKeyWidgetFieldFlags", "+PDFKit")]
		WidgetFieldFlags,

		[Field ("PDFAnnotationKeyWidgetFieldType", "+PDFKit")]
		WidgetFieldType,

		[Field ("PDFAnnotationKeyWidgetAppearanceDictionary", "+PDFKit")]
		WidgetAppearanceDictionary,

		[Field ("PDFAnnotationKeyWidgetMaxLen", "+PDFKit")]
		WidgetMaxLen,

		[Field ("PDFAnnotationKeyWidgetOptions", "+PDFKit")]
		WidgetOptions,

		[Field ("PDFAnnotationKeyWidgetRotation", "+PDFKit")]
		WidgetRotation,

		[Field ("PDFAnnotationKeyWidgetRolloverCaption", "+PDFKit")]
		WidgetRolloverCaption,

		[Field ("PDFAnnotationKeyWidgetTextLabelUI", "+PDFKit")]
		WidgetTextLabelUI,

		[Field ("PDFAnnotationKeyWidgetValue", "+PDFKit")]
		WidgetValue,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationSubtype {

		[Field ("PDFAnnotationSubtypeText", "+PDFKit")]
		Text,

		[Field ("PDFAnnotationSubtypeLink", "+PDFKit")]
		Link,

		[Field ("PDFAnnotationSubtypeFreeText", "+PDFKit")]
		FreeText,

		[Field ("PDFAnnotationSubtypeLine", "+PDFKit")]
		Line,

		[Field ("PDFAnnotationSubtypeSquare", "+PDFKit")]
		Square,

		[Field ("PDFAnnotationSubtypeCircle", "+PDFKit")]
		Circle,

		[Field ("PDFAnnotationSubtypeHighlight", "+PDFKit")]
		Highlight,

		[Field ("PDFAnnotationSubtypeUnderline", "+PDFKit")]
		Underline,

		[Field ("PDFAnnotationSubtypeStrikeOut", "+PDFKit")]
		StrikeOut,

		[Field ("PDFAnnotationSubtypeInk", "+PDFKit")]
		Ink,

		[Field ("PDFAnnotationSubtypeStamp", "+PDFKit")]
		Stamp,

		[Field ("PDFAnnotationSubtypePopup", "+PDFKit")]
		Popup,

		[Field ("PDFAnnotationSubtypeWidget", "+PDFKit")]
		Widget,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationWidgetSubtype {

		[Field ("PDFAnnotationWidgetSubtypeButton", "+PDFKit")]
		Button,

		[Field ("PDFAnnotationWidgetSubtypeChoice", "+PDFKit")]
		Choice,

		[Field ("PDFAnnotationWidgetSubtypeSignature", "+PDFKit")]
		Signature,

		[Field ("PDFAnnotationWidgetSubtypeText", "+PDFKit")]
		Text,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationLineEndingStyle {

		[Field ("PDFAnnotationLineEndingStyleNone", "+PDFKit")]
		None,

		[Field ("PDFAnnotationLineEndingStyleSquare", "+PDFKit")]
		Square,

		[Field ("PDFAnnotationLineEndingStyleCircle", "+PDFKit")]
		Circle,

		[Field ("PDFAnnotationLineEndingStyleDiamond", "+PDFKit")]
		Diamond,

		[Field ("PDFAnnotationLineEndingStyleOpenArrow", "+PDFKit")]
		OpenArrow,

		[Field ("PDFAnnotationLineEndingStyleClosedArrow", "+PDFKit")]
		ClosedArrow,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationTextIconType {

		[Field ("PDFAnnotationTextIconTypeComment", "+PDFKit")]
		Comment,

		[Field ("PDFAnnotationTextIconTypeKey", "+PDFKit")]
		Key,

		[Field ("PDFAnnotationTextIconTypeNote", "+PDFKit")]
		Note,

		[Field ("PDFAnnotationTextIconTypeHelp", "+PDFKit")]
		Help,

		[Field ("PDFAnnotationTextIconTypeNewParagraph", "+PDFKit")]
		NewParagraph,

		[Field ("PDFAnnotationTextIconTypeParagraph", "+PDFKit")]
		Paragraph,

		[Field ("PDFAnnotationTextIconTypeInsert", "+PDFKit")]
		Insert,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	enum PdfAnnotationHighlightingMode {

		[Field ("PDFAnnotationHighlightingModeNone", "+PDFKit")]
		None,

		[Field ("PDFAnnotationHighlightingModeInvert", "+PDFKit")]
		Invert,

		[Field ("PDFAnnotationHighlightingModeOutline", "+PDFKit")]
		Outline,

		[Field ("PDFAnnotationHighlightingModePush", "+PDFKit")]
		Push,
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	[Static]
	interface PdfAppearanceCharacteristicsKeys {

		[Field ("PDFAppearanceCharacteristicsKeyBackgroundColor", "+PDFKit")]
		NSString BackgroundColorKey { get; }

		[Field ("PDFAppearanceCharacteristicsKeyBorderColor", "+PDFKit")]
		NSString BorderColorKey { get; }

		[Field ("PDFAppearanceCharacteristicsKeyRotation", "+PDFKit")]
		NSString RotationKey { get; }

		[Field ("PDFAppearanceCharacteristicsKeyCaption", "+PDFKit")]
		NSString CaptionKey { get; }

		[Field ("PDFAppearanceCharacteristicsKeyRolloverCaption", "+PDFKit")]
		NSString RolloverCaptionKey { get; }

		[Field ("PDFAppearanceCharacteristicsKeyDownCaption", "+PDFKit")]
		NSString DownCaptionKey { get; }
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	[Static]
	interface PdfBorderKeys {

		[Field ("PDFBorderKeyLineWidth", "+PDFKit")]
		NSString LineWidthKey { get; }

		[Field ("PDFBorderKeyStyle", "+PDFKit")]
		NSString StyleKey { get; }

		[Field ("PDFBorderKeyDashPattern", "+PDFKit")]
		NSString DashPatternKey { get; }
	}

	[NoMac]
	[iOS (11,0)]
	[Internal]
	[Static]
	interface PdfDocumentAttributeKeys {

		[Field ("PDFDocumentTitleAttribute", "+PDFKit")]
		NSString TitleKey { get; }

		[Field ("PDFDocumentAuthorAttribute", "+PDFKit")]
		NSString AuthorKey { get; }

		[Field ("PDFDocumentSubjectAttribute", "+PDFKit")]
		NSString SubjectKey { get; }

		[Field ("PDFDocumentCreatorAttribute", "+PDFKit")]
		NSString CreatorKey { get; }

		[Field ("PDFDocumentProducerAttribute", "+PDFKit")]
		NSString ProducerKey { get; }

		[Field ("PDFDocumentCreationDateAttribute", "+PDFKit")]
		NSString CreationDateKey { get; }

		[Field ("PDFDocumentModificationDateAttribute", "+PDFKit")]
		NSString ModificationDateKey { get; }

		[Field ("PDFDocumentKeywordsAttribute", "+PDFKit")]
		NSString KeywordsKey { get; }
	}

	[NoMac]
	[iOS (11,0)]
	[StrongDictionary ("PdfDocumentAttributeKeys")]
	interface PdfDocumentAttributes {

		string Title { get; set; }
		string Author { get; set; }
		string Subject { get; set; }
		string Creator { get; set; }
		string Producer { get; set; }
		NSDate CreationDate { get; set; }
		NSDate ModificationDate { get; set; }
		string [] Keywords { get; set; }
	}

	[NoMac]
	[iOS (11,0)]
	[Internal]
	[Static]
	interface PdfDocumentWriteOptionKeys {

		[Field ("PDFDocumentOwnerPasswordOption", "+PDFKit")]
		NSString OwnerPasswordKey { get; }

		[Field ("PDFDocumentUserPasswordOption", "+PDFKit")]
		NSString UserPasswordKey { get; }
	}

	[NoMac]
	[iOS (11,0)]
	[StrongDictionary ("PdfDocumentWriteOptionKeys")]
	interface PdfDocumentWriteOptions {

		string OwnerPassword { get; set; }
		string UserPassword { get; set; }
	}

	[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFAppearanceCharacteristics")]
	interface PdfAppearanceCharacteristics : NSCopying {

		[Export ("controlType", ArgumentSemantic.Assign)]
		PdfWidgetControlType ControlType { get; set; }

		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[NullAllowed, Export ("borderColor", ArgumentSemantic.Copy)]
		NSColor BorderColor { get; set; }

		[Export ("rotation")]
		nint Rotation { get; set; }

		[NullAllowed, Export ("caption")]
		string Caption { get; set; }

		[NullAllowed, Export ("rolloverCaption")]
		string RolloverCaption { get; set; }

		[NullAllowed, Export ("downCaption")]
		string DownCaption { get; set; }

		[Export ("appearanceCharacteristicsKeyValues", ArgumentSemantic.Copy)]
		NSDictionary WeakAppearanceCharacteristicsKeyValues { get; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFAction")]
#if XAMCORE_2_0
	[Abstract]
#endif
	interface PdfAction : NSCopying {
		//This is an abstract superclass with no public init - should it have a private constructor??
		//As it is, I can create instances, that segfault when you access the type method.
		//marking the method as [Abstract] doesn't work because the subclasses do not explictly
		//define this method (although they implement it)
		[Export ("type")]
		string Type { get; }
	}

	[iOS (11,0)]
	[BaseType (typeof (PdfAction), Name="PDFActionGoTo")]
	interface PdfActionGoTo {

		[DesignatedInitializer]
		[Export ("initWithDestination:")]
		IntPtr Constructor (PdfDestination destination);

		[Export ("destination")]
		PdfDestination Destination { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (PdfAction), Name = "PDFActionNamed")]
	interface PdfActionNamed {

		[DesignatedInitializer]
		[Export ("initWithName:")]
		IntPtr Constructor (PdfActionNamedName name);

		[Export ("name")]
		PdfActionNamedName Name { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (PdfAction), Name = "PDFActionRemoteGoTo")]
	interface PdfActionRemoteGoTo {

		[DesignatedInitializer]
		[Export ("initWithPageIndex:atPoint:fileURL:")]
		IntPtr Constructor (nint pageIndex, CGPoint point, NSUrl fileUrl);

		[Export ("pageIndex")]
		nint PageIndex { get; set; }

		[Export ("point")]
		CGPoint Point { get; set; }

		[Export ("URL")]
		NSUrl Url { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (PdfAction), Name = "PDFActionResetForm")]
	interface PdfActionResetForm {
		//has a public Init ???
		
		//NSArray of NSString
		[Export ("fields"), NullAllowed]
		string [] Fields { get; set; }

		[Export ("fieldsIncludedAreCleared")]
		bool FieldsIncludedAreCleared { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (PdfAction), Name = "PDFActionURL")]
	interface PdfActionUrl {

		[DesignatedInitializer]
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("URL")]
		NSUrl Url { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFAnnotation")]
	interface PdfAnnotation : NSCoding, NSCopying {

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("initWithBounds:forType:withProperties:")]
		[DesignatedInitializer]
		IntPtr Constructor (CGRect bounds, NSString annotationType, [NullAllowed] NSDictionary properties);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Wrap ("this (bounds, annotationType.GetConstant (), properties)")]
		IntPtr Constructor (CGRect bounds, PdfAnnotationKey annotationType, [NullAllowed] NSDictionary properties);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead")]
		//[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead")] // Headers do not show changes on Mac
		[Export ("initWithBounds:")]
		IntPtr Constructor (CGRect bounds);

		[Export ("page")]
		PdfPage Page { get; set; }

#if XAMCORE_4_0
		[Protected]
		[Export ("type")]
		NSString Type { get; set; }
#else
		[Export ("type")]
		string Type { get; set; }
#endif

		[Export ("bounds")]
		CGRect Bounds { get; set; }
		
		[Export ("modificationDate")]
		NSDate ModificationDate { get; set; }

		[Export ("userName")]
		string UserName { get; set; }

		[NoiOS]
		[Export ("popup")]
		PdfAnnotationPopup Popup { get; set; }

		[Export ("shouldDisplay")]
		bool ShouldDisplay { get; set; }
		
		[Export ("shouldPrint")]
		bool ShouldPrint { get; set; }

		[Export ("border")]
		PdfBorder Border { get; set; }

		[Export ("color")]
		NSColor Color { get; set; }

		[Export ("mouseUpAction")]
		PdfAction MouseUpAction { get; set; }

		[Export ("contents")]
		string Contents { get; set; }
		
		[Export ("toolTip")]
		string ToolTip { get; }

		[Export ("hasAppearanceStream")]
		bool HasAppearanceStream { get; }

		[Export ("removeAllAppearanceStreams")]
		void RemoveAllAppearanceStreams ();

		[NoiOS]
		[Export ("drawWithBox:")]
		void Draw (PdfDisplayBox box);

		[NoMac] // [FAIL] Selector not found for PdfKit.PdfAnnotation : [set]action 
		[Export ("action", ArgumentSemantic.Strong), NullAllowed]
		PdfAction Action { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("drawWithBox:inContext:")]
		void Draw (PdfDisplayBox box, CGContext context);

		[Internal]
		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("setValue:forAnnotationKey:")]
		bool _SetValue (IntPtr value, NSString key);

		[Internal]
		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("valueForAnnotationKey:")]
		[return: NullAllowed]
		IntPtr _GetValue (NSString key);

		[Protected]
		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("setBoolean:forAnnotationKey:")]
		bool SetValue (bool boolean, NSString key);

		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Wrap ("SetValue (boolean, key.GetConstant ())")]
		bool SetValue (bool boolean, PdfAnnotationKey key);

		[Protected]
		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("setRect:forAnnotationKey:")]
		bool SetValue (CGRect rect, NSString key);

		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Wrap ("SetValue (rect, key.GetConstant ())")]
		bool SetValue (CGRect rect, PdfAnnotationKey key);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("annotationKeyValues", ArgumentSemantic.Copy)]
		NSDictionary AnnotationKeyValues { get; }

		[Protected]
		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Export ("removeValueForAnnotationKey:")]
		void RemoveValue (NSString key);

		[NoMac] // [Mac (10,12)] Headers do not show changes on Mac, but added on iOS 11 headers
		[Wrap ("RemoveValue (key.GetConstant ())")]
		void RemoveValue (PdfAnnotationKey key);

		// PDFAnnotation (PDFAnnotationUtilities) Category

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("font", ArgumentSemantic.Copy)]
		NSFont Font { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("fontColor", ArgumentSemantic.Copy)]
		NSColor FontColor { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("alignment", ArgumentSemantic.Assign)]
		NSTextAlignment Alignment { get; set; }
		
		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("startPoint", ArgumentSemantic.Assign)]
		CGPoint StartPoint { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("endPoint", ArgumentSemantic.Assign)]
		CGPoint EndPoint { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("startLineStyle", ArgumentSemantic.Assign)]
		PdfLineStyle StartLineStyle { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("endLineStyle", ArgumentSemantic.Assign)]
		PdfLineStyle EndLineStyle { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Static]
		[Export ("lineStyleFromName:")]
		PdfLineStyle GetLineStyle (string fromName);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Static]
		[Export ("nameForLineStyle:")]
		string GetName (PdfLineStyle style);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("iconType", ArgumentSemantic.Assign)]
		PdfTextAnnotationIconType IconType { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("quadrilateralPoints", ArgumentSemantic.Copy)]
		NSValue [] QuadrilateralPoints { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("markupType", ArgumentSemantic.Assign)]
		PdfMarkupType MarkupType { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("widgetFieldType")]
		string WidgetFieldType { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("widgetControlType", ArgumentSemantic.Assign)]
		PdfWidgetControlType WidgetControlType { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("multiline")]
		bool Multiline { [Bind ("isMultiline")] get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("isPasswordField")]
		bool IsPasswordField { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("comb")]
		bool Comb { [Bind ("hasComb")] get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("maximumLength")]
		nint MaximumLength { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("widgetStringValue")]
		string WidgetStringValue { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("widgetDefaultStringValue")]
		string WidgetDefaultStringValue { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsToggleToOff")]
		bool AllowsToggleToOff { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("radiosInUnison")]
		bool RadiosInUnison { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("readOnly")]
		bool ReadOnly { [Bind ("isReadOnly")] get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("listChoice")]
		bool ListChoice { [Bind ("isListChoice")] get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("choices", ArgumentSemantic.Copy)]
		string [] Choices { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("values", ArgumentSemantic.Copy)]
		string [] Values { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("buttonWidgetState", ArgumentSemantic.Assign)]
		PdfWidgetCellState ButtonWidgetState { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("buttonWidgetStateString")]
		string ButtonWidgetStateString { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("open")]
		bool Open { [Bind ("isOpen")] get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("paths")]
		NSBezierPath [] Paths { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("addBezierPath:")]
		void AddBezierPath (NSBezierPath path);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("removeBezierPath:")]
		void RemoveBezierPath (NSBezierPath path);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("destination", ArgumentSemantic.Strong)]
		PdfDestination Destination { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("fieldName")]
		string FieldName { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("caption")]
		string Caption { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationButtonWidget")]
	interface PdfAnnotationButtonWidget {
		[Export ("controlType")]
		PdfWidgetControlType ControlType { get; set; }
		
		[Export ("state")]
		nint State { get; set; }

		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }
		
		[Export ("allowsToggleToOff")]
		bool AllowsToggleToOff { get; set; }

		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("fontColor")]
		NSColor FontColor { get; set; }

		[Export ("caption")]
		string Caption { get; set; }

		[Export ("fieldName")]
		string FieldName { get; set; }

		[Export ("onStateValue")]
		string OnStateValue { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationChoiceWidget")]
	interface PdfAnnotationChoiceWidget {
		[Export ("stringValue")]
		string Text { get; set; }

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("fontColor")]
		NSColor FontColor { get; set; }

		[Export ("fieldName")]
		string FieldName { get; set; }

		[Export ("isListChoice")]
		bool IsListChoice { get; set; }
		
		// NSArray of NSString
		[Export ("choices")]
		string [] Choices { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationCircle")]
	interface PdfAnnotationCircle {
		[Export ("interiorColor")]
		NSColor InteriorColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationFreeText")]
	interface PdfAnnotationFreeText {
		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("fontColor")]
		NSColor FontColor { get; set; }

		[Export ("alignment")]
		NSTextAlignment Alignment { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationInk")]
	interface PdfAnnotationInk {
		[Export ("paths")]
		NSBezierPath [] Paths { get; }

		[Export ("addBezierPath:")]
		void AddBezierPathpath (NSBezierPath path);

		[Export ("removeBezierPath:")]
		void RemoveBezierPathpath (NSBezierPath path);
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationLine")]
	interface PdfAnnotationLine {
		[Export ("startPoint")]
		CGPoint StartPoint { get; set; }

		[Export ("endPoint")]
		CGPoint EndPoint { get; set; }

		[Export ("startLineStyle")]
		PdfLineStyle StartLineStyle { get; set; }

		[Export ("endLineStyle")]
		PdfLineStyle EndLineStyle { get; set; }

		[Export ("interiorColor")]
		NSColor InteriorColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationLink")]
	interface PdfAnnotationLink {
		[Export ("destination")]
		PdfDestination Destination { get; set; }

		[Export ("URL")]
		NSUrl Url { get; set; }
		
		[Export ("setHighlighted:")]
		void SetHighlighted (bool highlighted);
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationMarkup")]
	interface PdfAnnotationMarkup {
		[Export ("quadrilateralPoints", ArgumentSemantic.Assign), NullAllowed]
#if XAMCORE_2_0
		NSArray WeakQuadrilateralPoints { get; set; }
#else
		NSArray QuadrilateralPoints { get; set; }
#endif

		[Export ("markupType")]
		PdfMarkupType MarkupType { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationPopup")]
	interface PdfAnnotationPopup {
		[Export ("isOpen")]
		bool IsOpen { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationSquare")]
	interface PdfAnnotationSquare {
		[Export ("interiorColor")]
		NSColor InteriorColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationStamp")]
	interface PdfAnnotationStamp {
		[Export ("name")]
		string Name { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationText")]
	interface PdfAnnotationText {
		[Export ("iconType")]
		PdfTextAnnotationIconType IconType { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationTextWidget")]
	interface PdfAnnotationTextWidget {
		[Export ("stringValue")]
		string StringValue { get; set; }

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Export ("rotation")]
		int Rotation { get; set; } // (int) rotation;

		[Export ("font")]
		NSFont Font { get; set; }

		[Export ("fontColor")]
		NSColor FontColor { get; set; }

		[Export ("alignment")]
		NSTextAlignment Alignment { get; set; }
		
		[Export ("maximumLength")]
		nint MaximumLength { get; set; }

		[Export ("fieldName")]
		string FieldName { get; set; }

		[Export ("attributedStringValue")]
		NSAttributedString AttributedStringValue { get; set; }

		[Export ("isMultiline")]
		bool IsMultiline { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFBorder")]
	interface PdfBorder : NSCoding, NSCopying {
		[Export ("style")]
		PdfBorderStyle Style { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("horizontalCornerRadius")]
		nfloat HorizontalCornerRadius { get; set; }

		[Export ("verticalCornerRadius")]
		nfloat VerticalCornerRadius { get; set; }

		[Export ("dashPattern", ArgumentSemantic.Assign), NullAllowed]
#if XAMCORE_2_0
		NSArray WeakDashPattern { get; set; }
#else
		NSArray DashPattern { get; set; }
#endif

		[NoMac]
		[Export ("borderKeyValues", ArgumentSemantic.Copy)]
		NSDictionary WeakBorderKeyValues { get; }

		[Export ("drawInRect:")]
		void Draw (CGRect rect);
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFDestination")]
	interface PdfDestination : NSCopying {

		[DesignatedInitializer]
		[Export ("initWithPage:atPoint:")]
		IntPtr Constructor (PdfPage page, CGPoint point);

		[Export ("page")]
		PdfPage Page { get; }

		[Export ("point")]
		CGPoint Point { get; }

		[Mac (10, 7)]
		[Export ("zoom")]
		nfloat Zoom { get; set; }

		//Should Compare be more more .Net ified ?
		[Export ("compare:")]
		NSComparisonResult Compare (PdfDestination destination);
	}

	//Add attributes for delegates/events
	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFDocument", Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (PdfDocumentDelegate) })]
	interface PdfDocument : NSCopying {

		[Field ("PDFDocumentDidUnlockNotification", "+PDFKit")]
		[Notification]
		NSString DidUnlockNotification { get; }

		[Field ("PDFDocumentDidBeginFindNotification", "+PDFKit")]
		[Notification]
		NSString DidBeginFindNotification { get; }

		[Field ("PDFDocumentDidEndFindNotification", "+PDFKit")]
		[Notification]
		NSString DidEndFindNotification { get; }

		[Field ("PDFDocumentDidBeginPageFindNotification", "+PDFKit")]
		[Notification]
		NSString DidBeginPageFindNotification { get; }

		[Field ("PDFDocumentDidEndPageFindNotification", "+PDFKit")]
		[Notification]
		NSString DidEndPageFindNotification { get; }

		[Field ("PDFDocumentDidFindMatchNotification", "+PDFKit")]
		[Notification]
		NSString DidFindMatchNotification { get; }

		[Field ("PDFDocumentDidBeginWriteNotification", "+PDFKit")]
		[Notification]
		NSString DidBeginWriteNotification { get; }

		[Field ("PDFDocumentDidEndWriteNotification", "+PDFKit")]
		[Notification]
		NSString DidEndWriteNotification { get; }

		[Field ("PDFDocumentDidBeginPageWriteNotification", "+PDFKit")]
		[Notification]
		NSString DidBeginPageWriteNotification { get; }

		[Field ("PDFDocumentDidEndPageWriteNotification", "+PDFKit")]
		[Notification]
		NSString DidEndPageWriteNotification { get; }

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSUrl url);

		[Export ("initWithData:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSData data);

		[Export ("documentURL")]
		NSUrl DocumentUrl { get; }

		[Export ("documentRef")]
		CGPDFDocument Document { get; }

#if IOS
		[Advice ("Use the strongly typed '[Get|Set]DocumentAttributes' instead.")]
#endif
		[Export ("documentAttributes")]
		NSDictionary DocumentAttributes { get; set; }

		[NoMac]
		[Wrap ("new PdfDocumentAttributes (DocumentAttributes)")]
		PdfDocumentAttributes GetDocumentAttributes ();

		[NoMac]
		[Wrap ("DocumentAttributes = attributes?.Dictionary")]
		void SetDocumentAttributes (PdfDocumentAttributes attributes);

#if XAMCORE_4_0 || IOS
		[Export ("majorVersion")]
		nint MajorVersion { get; }

		[Export ("minorVersion")]
		nint MinorVersion { get; }
#else
		[Export ("majorVersion")]
		int MajorVersion { get; } /* int, not NSInteger */

		[Export ("minorVersion")]
		int MinorVersion { get; } /* int, not NSInteger */
#endif

		[Export ("isEncrypted")]
		bool IsEncrypted { get; }

		[Export ("isLocked")]
		bool IsLocked { get; }

		[Export ("unlockWithPassword:")]
		bool Unlock (string password);

		[Export ("allowsPrinting")]
		bool AllowsPrinting { get; }

		[Export ("allowsCopying")]
		bool AllowsCopying { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsDocumentChanges")]
		bool AllowsDocumentChanges { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsDocumentAssembly")]
		bool AllowsDocumentAssembly { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsContentAccessibility")]
		bool AllowsContentAccessibility { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsCommenting")]
		bool AllowsCommenting { get; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("allowsFormFieldEntry")]
		bool AllowsFormFieldEntry { get; }

		[Export ("permissionsStatus")]
		PdfDocumentPermissions PermissionsStatus { get; }

		[Export ("string")]
		string Text { get; }
		
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PdfDocumentDelegate Delegate { get; set; }
		
		[Export ("dataRepresentation")]
		NSData GetDataRepresentation ();

		[Export ("dataRepresentationWithOptions:")]
		NSData GetDataRepresentation (NSDictionary options);

		[Export ("writeToFile:")]
		bool Write (string path);

		[Export ("writeToFile:withOptions:")]
		bool Write (string path, NSDictionary options);

		[NoMac]
		[Wrap ("Write (path, options.Dictionary)")]
		bool Write (string path, PdfDocumentWriteOptions options);

		[Export ("writeToURL:")]
		bool Write (NSUrl url);

		[Export ("writeToURL:withOptions:")]
		bool Write (NSUrl url, NSDictionary options);

		[NoMac]
		[Wrap ("Write (url, options?.Dictionary)")]
		bool Write (NSUrl url, PdfDocumentWriteOptions options);

		[NullAllowed]
		[Export ("outlineRoot")]
		PdfOutline OutlineRoot { get; set; }

		[Export ("outlineItemForSelection:")]
		PdfOutline OutlineItem (PdfSelection selection);

		[Export ("pageCount")]
		nint PageCount { get; }

		[Export ("pageAtIndex:")]
		PdfPage GetPage (nint index);

		[Export ("indexForPage:")]
		nint GetPageIndex (PdfPage page);

		[Export ("insertPage:atIndex:")]
		void InsertPage (PdfPage page, nint index);

		[Export ("removePageAtIndex:")]
		void RemovePage (nint index);

		[Export ("exchangePageAtIndex:withPageAtIndex:")]
		void ExchangePages (nint indexA, nint indexB);

		[Export ("pageClass")]
		Class PageClass { get; }

		[Wrap ("Class.Lookup (PageClass)")]
		Type PageType { get; }

		[Export ("findString:withOptions:")]
#if !XAMCORE_4_0
		[Obsolete ("Use 'Find (string, NSStringCompareOptions)' instead.")]
		PdfSelection [] Find (string text, nint options);

		[Wrap ("Find (text: text, options: (nint) (int) compareOptions)")]
#endif
		PdfSelection [] Find (string text, NSStringCompareOptions compareOptions);

		[Export ("beginFindString:withOptions:")]
#if !XAMCORE_4_0
		[Obsolete ("Use 'FindAsync (string, NSStringCompareOptions)' instead.")]
		void FindAsync (string text, nint options);

		[Wrap ("FindAsync (text: text, options: (nint) (int) compareOptions)")]
#endif
		void FindAsync (string text, NSStringCompareOptions compareOptions);

		[Export ("beginFindStrings:withOptions:")]
#if !XAMCORE_4_0
		[Obsolete ("Use 'FindAsync (string [], NSStringCompareOptions)' instead.")]
		void FindAsync (string [] text, nint options);

		[Wrap ("FindAsync (text: text, options: (nint) (int) compareOptions)")]
#endif
		void FindAsync (string [] text, NSStringCompareOptions compareOptions);

		[Export ("findString:fromSelection:withOptions:")]
#if !XAMCORE_4_0
		[Obsolete ("Use 'Find (string, PdfSelection, NSStringCompareOptions)' instead.")]
		PdfSelection Find (string text, PdfSelection selection, nint options);

		[Wrap ("Find (text: text, selection: selection, options: (nint) (int) compareOptions)")]
#endif
		PdfSelection Find (string text, PdfSelection selection, NSStringCompareOptions compareOptions);

		[Export ("isFinding")]
		bool IsFinding { get; }

		[Export ("cancelFindString")]
		void CancelFind ();

		[Export ("selectionForEntireDocument")]
		PdfSelection SelectEntireDocument ();

		[Export ("selectionFromPage:atPoint:toPage:atPoint:")]
		PdfSelection GetSelection (PdfPage startPage, CGPoint startPoint, PdfPage endPage, CGPoint endPoint);

		[Export ("selectionFromPage:atCharacterIndex:toPage:atCharacterIndex:")]
		PdfSelection GetSelection (PdfPage startPage, nint startCharIndex, PdfPage endPage, nint endCharIndex);

		[NoiOS]
		[Mac (10,7)]
		[Export ("printOperationForPrintInfo:scalingMode:autoRotate:")]
		[return: NullAllowed]
		NSPrintOperation GetPrintOperation ([NullAllowed] NSPrintInfo printInfo, PdfPrintScalingMode scaleMode, bool doRotate);
	}

	[iOS (11,0)]
#if IOS
	[BaseType (typeof (NSObject), Name = "PDFDocumentDelegate")]
	[Protocol]
#else
	[BaseType (typeof (NSObject))]
	[Protocol (IsInformal = true)]
#endif
	[Model]
	interface PdfDocumentDelegate {

		[Export ("documentDidUnlock:"), EventArgs ("NSNotification")]
		void DidUnlock (NSNotification notification);

		[Export ("documentDidBeginDocumentFind:"), EventArgs ("NSNotification")]
		void DidBeginDocumentFind (NSNotification notification);

		[Export ("didMatchString:"), EventArgs ("PdfSelection")]
		void DidMatchString (PdfSelection sender);

		[Export ("classForPage"), IgnoredInDelegate]
		Class GetClassForPage ();

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[DelegateName ("ClassForAnnotationTypeDelegate"), DefaultValue (null)]
		[Export ("classForAnnotationType:")]
		Class GetClassForAnnotationType (string annotationType);

		[NoiOS]
		[Export ("classForAnnotationClass:"), DelegateName ("ClassForAnnotationClassDelegate"), DefaultValue (null)]
#if XAMCORE_4_0
		Class GetClassForAnnotationClass (Class sender);
#else		
		Class ClassForAnnotationClass (Class sender);
#endif

		[Export ("documentDidEndDocumentFind:"), EventArgs ("NSNotification")]
		void FindFinished (NSNotification notification);

		[Export ("documentDidBeginPageFind:"), EventArgs ("NSNotification")]
		void PageFindStarted (NSNotification notification);

		[Export ("documentDidEndPageFind:"), EventArgs ("NSNotification")]
		void PageFindFinished (NSNotification notification);

		[Export ("documentDidFindMatch:"), EventArgs ("NSNotification")]
		void MatchFound (NSNotification notification);
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFOutline")]
	interface PdfOutline {

		[Export ("document")]
		PdfDocument Document { get; }

		[Export ("parent"), NullAllowed]
		PdfOutline Parent { get; }

		[Export ("numberOfChildren")]
		nint ChildrenCount { get; }

		[Export ("index")]
		nint Index { get; }

		[Export ("childAtIndex:")]
		PdfOutline Child (nint index);

		[Export ("insertChild:atIndex:")]
		void InsertChild (PdfOutline child, nint index);

		[Export ("removeFromParent")]
		void RemoveFromParent ();

		[Export ("label")]
		string Label { get; set; }

		[Export ("isOpen")]
		bool IsOpen { get; set; }

		[Export ("destination"), NullAllowed]
		PdfDestination Destination { get; set; }

		[Export ("action"), NullAllowed]
		PdfAction Action { get; set; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFPage")]
	interface PdfPage : NSCopying {

		[DesignatedInitializer]
		[Export ("initWithImage:")]
		IntPtr Constructor (NSImage image);

		[Export ("document"), NullAllowed]
		PdfDocument Document { get; }

		[Export ("pageRef"), NullAllowed]
		CGPDFPage Page { get; }

		[Export ("label"), NullAllowed]
		string Label { get; }

		[Export ("boundsForBox:")]
		CGRect GetBoundsForBox (PdfDisplayBox box);

		[Export ("setBounds:forBox:")]
		void SetBoundsForBox (CGRect bounds, PdfDisplayBox box);

		[Export ("rotation")]
		nint Rotation { get; set; } // - (NSInteger) rotation; - rotation is not consistently declared

		//Check  Docs say: "array will _most likely_ be typed to subclasses of the PdfAnnotation class"
		//do they mean that if it isn't a subclass it is the base class ??
		//Maybe we should be safe and return NSArray ??
		[Export ("annotations")]
		PdfAnnotation [] Annotations { get; }

		[Export ("displaysAnnotations")]
		bool DisplaysAnnotations { get; set; }

		[Export ("addAnnotation:")]
		void AddAnnotation (PdfAnnotation annotation);

		[Export ("removeAnnotation:")]
		void RemoveAnnotation (PdfAnnotation annotation);

		[Export ("annotationAtPoint:")]
		[return: NullAllowed]
		PdfAnnotation GetAnnotation (CGPoint point);

		[Mac (10,12)]
		[Export ("transformForBox:")]
		CGAffineTransform GetTransform (PdfDisplayBox box);

		[NoiOS]
		[Mac (10,12)]
		[Export ("drawWithBox:")]
		void Draw (PdfDisplayBox box);

		[Mac (10,12)]
		[Export ("drawWithBox:toContext:")]
		void Draw (PdfDisplayBox box, CGContext context);

		[Mac (10,12)]
		[Export ("transformContext:forBox:")]
		void TransformContext (CGContext context, PdfDisplayBox box);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("thumbnailOfSize:forBox:")]
		NSImage GetThumbnail (CGSize size, PdfDisplayBox box);

		[NoiOS]
		[Export ("transformContextForBox:")]
		void TransformContext (PdfDisplayBox box);

		[Export ("numberOfCharacters")]
		nint CharacterCount { get; }

		[Export ("string")]
		string Text { get; }

		[Export ("attributedString")]
		NSAttributedString AttributedString { get; }

		[Export ("characterBoundsAtIndex:")]
		CGRect GetCharacterBounds (nint index);

		[Export ("characterIndexAtPoint:")]
		nint GetCharacterIndex (CGPoint point);

		[Export ("selectionForRect:")]
		[return: NullAllowed]
		PdfSelection GetSelection (CGRect rect);

		[Export ("selectionForWordAtPoint:")]
		[return: NullAllowed]
		PdfSelection SelectWord (CGPoint point);

		[Export ("selectionForLineAtPoint:")]
		[return: NullAllowed]
		PdfSelection SelectLine (CGPoint point);

		[Export ("selectionFromPoint:toPoint:")]
		[return: NullAllowed]
		PdfSelection GetSelection (CGPoint startPoint, CGPoint endPoint);

		[Export ("selectionForRange:")]
		[return: NullAllowed]
		PdfSelection GetSelection (NSRange range);

		[Export ("dataRepresentation")]
		NSData DataRepresentation { get; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "PDFSelection")]
	[DisableDefaultCtor] // An uncaught exception was raised: init: not a valid initializer for PDFSelection
	interface PdfSelection : NSCopying {

		[DesignatedInitializer]
		[Export ("initWithDocument:")]
		IntPtr Constructor (PdfDocument document);

		[Export ("pages")]
		PdfPage [] Pages { get; }

		[Export ("color", ArgumentSemantic.Copy), NullAllowed]
		NSColor Color { get; set; }

		[Export ("string"), NullAllowed]
		string Text { get; }

		[Export ("attributedString"), NullAllowed]
		NSAttributedString AttributedString { get; }

		[Export ("boundsForPage:")]
		CGRect GetBoundsForPage (PdfPage page);

		[Mac (10, 7)]
		[Export ("numberOfTextRangesOnPage:")]
		nuint GetNumberOfTextRanges (PdfPage page);

		[Mac (10, 7)]
		[Export ("rangeAtIndex:onPage:")]
		NSRange GetRange (nuint index, PdfPage page);

		[Export ("selectionsByLine")]
		PdfSelection [] SelectionsByLine ();

		[Export ("addSelection:")]
		void AddSelection (PdfSelection selection);

		[Export ("addSelections:")]
		void AddSelections (PdfSelection [] selections);

		[Export ("extendSelectionAtEnd:")]
		void ExtendSelectionAtEnd (nint succeed);

		[Export ("extendSelectionAtStart:")]
		void ExtendSelectionAtStart (nint precede);

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("extendSelectionForLineBoundaries")]
		void ExtendSelectionForLineBoundaries ();

		[Export ("drawForPage:active:")]
		void Draw (PdfPage page, bool active);

		[Export ("drawForPage:withBox:active:")]
		void Draw (PdfPage page, PdfDisplayBox box, bool active);
	}

	[iOS (11,0)]
	[BaseType (typeof (NSView), Name = "PDFThumbnailView")]
	interface PdfThumbnailView : NSCoding {

		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Field ("PDFThumbnailViewDocumentEditedNotification", "+PDFKit")]
		[Notification]
		NSString DocumentEditedNotification { get; }

		[Export ("PDFView"), NullAllowed]
		PdfView PdfView { get; set; }

		[Export ("thumbnailSize")]
		CGSize ThumbnailSize { get; set; }

		[NoMac]
		[Export ("layoutMode")]
		PdfThumbnailLayoutMode LayoutMode { get; set; }

		[NoMac]
		[Export ("contentInset")]
		NSEdgeInsets ContentInset { get; set; }

		[NoiOS]
		[Export ("maximumNumberOfColumns")]
		nint MaximumNumberOfColumns { get; set; }

		[NoiOS]
		[Export ("labelFont")]
		NSFont LabelFont { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[NoiOS]
		[Export ("allowsDragging")]
		bool AllowsDragging { get; set; }

		[NoiOS]
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("selectedPages", ArgumentSemantic.Strong), NullAllowed]
		PdfPage [] SelectedPages { get; }
	}

	[iOS (11,0)]
	[BaseType (typeof (NSView), Name = "PDFView", Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (PdfViewDelegate) })]
	interface PdfView :
#if IOS
	UIGestureRecognizerDelegate
#else
	NSMenuDelegate, NSAnimationDelegate
#endif
	{
		[Export ("initWithFrame:")]
		IntPtr Constructor (CGRect frame);

		[Export ("document"), NullAllowed]
		PdfDocument Document { get; set; }

		[Export ("canGoToFirstPage")]
		bool CanGoToFirstPage { get; }
	
		//Verify
		[Export ("goToFirstPage:")]
		void GoToFirstPage (NSObject sender);

		[Export ("canGoToLastPage")]
		bool CanGoToLastPage { get; }

		[Export ("goToLastPage:")]
		void GoToLastPage (NSObject sender);

		[Export ("canGoToNextPage")]
		bool CanGoToNextPage { get; }

		[Export ("goToNextPage:")]
		void GoToNextPage (NSObject sender);

		[Export ("canGoToPreviousPage")]
		bool CanGoToPreviousPage { get; }

		[Export ("goToPreviousPage:")]
		void GoToPreviousPage (NSObject sender);

		[Export ("canGoBack")]
		bool CanGoBack { get; }

		[Export ("goBack:")]
		void GoBack (NSObject sender);

		[Export ("canGoForward")]
		bool CanGoForward { get; }

		[Export ("goForward:")]
		void GoForward (NSObject sender);

		[Export ("currentPage")]
		PdfPage CurrentPage { get; }

		[Export ("goToPage:")]
		void GoToPage (PdfPage page);

		[Export ("currentDestination")]
		PdfDestination CurrentDestination { get; }

		[Export ("goToDestination:")]
		void GoToDestination (PdfDestination destination);

		[Export ("goToSelection:")]
		void GoToSelection (PdfSelection selection);

		[Export ("goToRect:onPage:")]
		void GoToRectangle (CGRect rect, PdfPage page);

		[Export ("displayMode")]
		PdfDisplayMode DisplayMode { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("displayDirection")]
		PdfDisplayDirection DisplayDirection { get; set; }

		[Export ("displaysPageBreaks")]
		bool DisplaysPageBreaks { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("pageBreakMargins")]
		NSEdgeInsets PageBreakMargins { get; set; }

		[Export ("displayBox")]
		PdfDisplayBox DisplayBox { get; set; }

		[Export ("displaysAsBook")]
		bool DisplaysAsBook { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("displaysRTL")]
		bool DisplaysRtl { get; set; }

		[Export ("shouldAntiAlias")]
		bool ShouldAntiAlias { get; set; }

		[Export ("greekingThreshold")]
		nfloat GreekingThreshold { get; set; }

		[Export ("takeBackgroundColorFrom:")]
		void TakeBackgroundColor (NSObject sender);

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Mac (10,7)]
		[Export ("interpolationQuality", ArgumentSemantic.Assign)]
		PdfInterpolationQuality InterpolationQuality { get; set; }

		[NoMac]
		[Export ("usePageViewController:withViewOptions:")]
		void UsePageViewController (bool enable, [NullAllowed] NSDictionary viewOptions);

		[NoMac]
		[Export ("isUsingPageViewController")]
		bool IsUsingPageViewController { get; }
	
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PdfViewDelegate Delegate { get; set; }

		[Export ("scaleFactor")]
		nfloat ScaleFactor { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("minScaleFactor")]
		nfloat MinScaleFactor { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("maxScaleFactor")]
		nfloat MaxScaleFactor { get; set; }

		[Export ("zoomIn:")]
		void ZoomIn (NSObject sender);

		[Export ("canZoomIn")]
		bool CanZoomIn { get; }

		[Export ("zoomOut:")]
		void ZoomOut (NSObject sender);

		[Export ("canZoomOut")]
		bool CanZoomOut { get; }

		[Export ("autoScales")]
		bool AutoScales { get; set; }

		[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		[Export ("scaleFactorForSizeToFit")]
		nfloat ScaleFactorForSizeToFit { get; }

		[Export ("areaOfInterestForMouse:")]
		PdfAreaOfInterest GetAreaOfInterest (NSEvent mouseEvent);

		[Mac (10,10,3)]
		[Export ("areaOfInterestForPoint:")]
		PdfAreaOfInterest GetAreaOfInterest (CGPoint point);

		[NoiOS]
		[Export ("setCursorForAreaOfInterest:")]
		void SetCursor (PdfAreaOfInterest area);

		[Export ("performAction:")]
		void PerformAction (PdfAction action);

		[Export ("currentSelection")]
		PdfSelection CurrentSelection { get; set; }

		[Export ("setCurrentSelection:animate:")]
		void SetCurrentSelection (PdfSelection selection, bool animate);

		[Export ("clearSelection")]
		void ClearSelection ();

		[Export ("selectAll:")]
		void SelectAll ([NullAllowed] NSObject sender);

		[Export ("scrollSelectionToVisible:")]
		void ScrollSelectionToVisible (NSObject sender);
	
		[Export ("highlightedSelections")]
		PdfSelection [] HighlightedSelections { get; set; }

		[Export ("takePasswordFrom:")]
		void TakePasswordFrom (NSObject sender);

		[NoiOS]
		[Export ("drawPage:")]
		void DrawPage (PdfPage page);

		[Mac (10,12)]
		[Export ("drawPage:toContext:")]
		void DrawPage (PdfPage page, CGContext context);

		[Mac (10,12)]
		[Export ("drawPagePost:toContext:")]
		void DrawPagePost (PdfPage page, CGContext context);

		[NoiOS]
		[Export ("drawPagePost:")]
		void DrawPagePost (PdfPage page);

		[Export ("copy:")]
		void Copy ([NullAllowed] NSObject sender);

		[NoiOS]
		[Export ("printWithInfo:autoRotate:")]
		void Print (NSPrintInfo printInfo, bool doRotate);

		[NoiOS]
		[Export ("printWithInfo:autoRotate:pageScaling:")]
		void Print (NSPrintInfo printInfo, bool doRotate, PdfPrintScalingMode scaleMode);

		[Export ("pageForPoint:nearest:")]
		PdfPage GetPage (CGPoint point, bool nearest);

		[Export ("convertPoint:toPage:")]
		CGPoint ConvertPointToPage (CGPoint point, PdfPage page);

		[Export ("convertRect:toPage:")]
		CGRect ConvertRectangleToPage (CGRect rect, PdfPage page);

		[Export ("convertPoint:fromPage:")]
		CGPoint ConvertPointFromPage (CGPoint point, PdfPage page);

		[Export ("convertRect:fromPage:")]
		CGRect ConvertRectangleFromPage (CGRect rect, PdfPage page);

		[Export ("documentView")]
		NSView DocumentView { get; }

		[Export ("layoutDocumentView")]
		void LayoutDocumentView ();

		[Export ("annotationsChangedOnPage:")]
		void AnnotationsChanged (PdfPage page);

		[Export ("rowSizeForPage:")]
		CGSize RowSize (PdfPage page);

		[NoiOS]
		[Export ("allowsDragging")]
		bool AllowsDragging { get; set; }
	
		[Export ("visiblePages")]
		PdfPage [] VisiblePages { get; }

		[Export ("enableDataDetectors")]
		bool EnableDataDetectors { get; set; }

		[Field("PDFViewChangedHistoryNotification", "+PDFKit")]
		[Notification]
		NSString ChangedHistoryNotification { get; }

		[Field("PDFViewDocumentChangedNotification", "+PDFKit")]
		[Notification]
		NSString DocumentChangedNotification { get; }

		[Field ("PDFViewPageChangedNotification", "+PDFKit")]
		[Notification]
		NSString PageChangedNotification { get; }

		[Field ("PDFViewScaleChangedNotification", "+PDFKit")]
		[Notification]
		NSString ScaleChangedNotification { get; }

		[Field ("PDFViewAnnotationHitNotification", "+PDFKit")]
		[Notification (typeof (PdfViewAnnotationHitEventArgs))]
		NSString AnnotationHitNotification { get; }

		[Field ("PDFViewCopyPermissionNotification", "+PDFKit")]
		[Notification]
		NSString CopyPermissionNotification { get; }

		[Field ("PDFViewPrintPermissionNotification", "+PDFKit")]
		[Notification]
		NSString PrintPermissionNotification { get; }

		[Field ("PDFViewAnnotationWillHitNotification", "+PDFKit")]
		[Notification]
		NSString AnnotationWillHitNotification { get; }

		[Field ("PDFViewSelectionChangedNotification", "+PDFKit")]
		[Notification]
		NSString SelectionChangedNotification { get; }

		[Field ("PDFViewDisplayModeChangedNotification", "+PDFKit")]
		[Notification]
		NSString DisplayModeChangedNotification { get; }

		[Field ("PDFViewDisplayBoxChangedNotification", "+PDFKit")]
		[Notification]
		NSString DisplayBoxChangedNotification { get; }

		[Field ("PDFViewVisiblePagesChangedNotification", "+PDFKit")]
		[Notification]
		NSString VisiblePagesChangedNotification { get; }

		//[NoiOS]
		//[NoMac] // [Mac (10,13)] Headers do not show changes on Mac
		//[Export ("acceptsDraggedFiles")]
		//bool AcceptsDraggedFiles { get; set; }
	}
	
	[NoiOS]
	interface PdfViewAnnotationHitEventArgs {
		[Export ("PDFAnnotationHit")]
		PdfAnnotation AnnotationHit { get; }
	}
	
	//Verify delegate methods.  There are default actions (not just return null ) that should occur
	//if the delegate does not implement the method.
	[iOS (11,0)]
#if IOS
	[BaseType (typeof (NSObject), Name = "PDFViewDelegate")]
	[Protocol]
#else
	[BaseType (typeof (NSObject))]
	[Protocol (IsInformal = true)]
#endif
	[Model]
	interface PdfViewDelegate {
		//from docs: 'By default, the scale factor is restricted to a range between 0.1 and 10.0 inclusive.'
		[NoiOS]
		[Export ("PDFViewWillChangeScaleFactor:toScale:"), DelegateName ("PdfViewScale"), DefaultValueFromArgument ("scale")]
		nfloat WillChangeScaleFactor (PdfView sender, nfloat scale);

		[Export ("PDFViewWillClickOnLink:withURL:"), EventArgs ("PdfViewUrl")]
		void WillClickOnLink (PdfView sender, NSUrl url);

		// from the docs: 'By default, this method uses the string, if any, associated with the
		// 'Title' key in the view's PDFDocument attribute dictionary. If there is no such string,
		// this method uses the last path component if the document is URL-based.
		[NoiOS]
		[Export ("PDFViewPrintJobTitle:"), DelegateName ("PdfViewTitle"), DefaultValue ("String.Empty")]
		string TitleOfPrintJob (PdfView sender);

		[Export ("PDFViewPerformFind:"), EventArgs ("PdfView")]
		void PerformFind (PdfView sender);

		[Export ("PDFViewPerformGoToPage:"), EventArgs ("PdfView")]
		void PerformGoToPage (PdfView sender);

		[NoiOS]
		[Export ("PDFViewPerformPrint:"), EventArgs ("PdfView")]
		void PerformPrint (PdfView sender);

		[Export ("PDFViewOpenPDF:forRemoteGoToAction:"), EventArgs ("PdfViewAction")]
		void OpenPdf (PdfView sender, PdfActionRemoteGoTo action);
	}

}
#endif // MONOMAC || (IOS && XAMCORE_2_0)
