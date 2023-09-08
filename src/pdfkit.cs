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

using System;
#if MONOMAC
using AppKit;
using UIViewController = Foundation.NSObject;
using UIFindInteraction = Foundation.NSObject;
using UIFindInteractionDelegate = Foundation.NSObject;
using UIView = AppKit.NSView;
#else
using UIKit;
using NSColor = UIKit.UIColor;
using NSFont = UIKit.UIFont;
using NSImage = UIKit.UIImage;
using NSView = UIKit.UIView;
using NSEdgeInsets = UIKit.UIEdgeInsets;
using NSEvent = UIKit.UIEvent;
using NSBezierPath = UIKit.UIBezierPath;
using NSTextAlignment = UIKit.UITextAlignment;
// HACK: to make intermediate dll build, since we use these 
// types in a few [NoiOS] members (this way we avoid numerous #ifdefs later)
using NSPrintInfo = Foundation.NSObject;
using NSPrintOperation = Foundation.NSObject;
#endif
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using System.ComponentModel;

#if !NET
using NativeHandle = System.IntPtr;
#endif

// Verify/Test Delegate Models
// Check for missing NullAllowed on all object properties
// Test methods returning typed arrays in lieu of NSArray
// Check classes with no public inits - Should I make the constructors private?
// Check the few abnormal properties

namespace PdfKit {

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
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

	[Native]
	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
	public enum PdfAccessPermissions : ulong {
		LowQualityPrinting = (1uL << 0),
		HighQualityPrinting = (1uL << 1),
		DocumentChanges = (1uL << 2),
		DocumentAssembly = (1uL << 3),
		ContentCopying = (1uL << 4),
		ContentAccessibility = (1uL << 5),
		Commenting = (1uL << 6),
		FormFieldEntry = (1uL << 7),
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Static]
	interface PdfBorderKeys {

		[Field ("PDFBorderKeyLineWidth", "+PDFKit")]
		NSString LineWidthKey { get; }

		[Field ("PDFBorderKeyStyle", "+PDFKit")]
		NSString StyleKey { get; }

		[Field ("PDFBorderKeyDashPattern", "+PDFKit")]
		NSString DashPatternKey { get; }
	}

	[MacCatalyst (13, 1)]
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

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[StrongDictionary ("PdfPageImageInitializationOptionKeys")]
	interface PdfPageImageInitializationOption {
		CGRect MediaBox { get; set; }
		int Rotation { get; set; }
		bool UpscaleIfSmaller { get; set; }
		double CompressionQuality { get; set; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Static]
	interface PdfPageImageInitializationOptionKeys {
		[Field ("PDFPageImageInitializationOptionMediaBox")]
		NSString MediaBoxKey { get; }

		[Field ("PDFPageImageInitializationOptionRotation")]
		NSString RotationKey { get; }

		[Field ("PDFPageImageInitializationOptionUpscaleIfSmaller")]
		NSString UpscaleIfSmallerKey { get; }

		[Field ("PDFPageImageInitializationOptionCompressionQuality")]
		NSString CompressionQualityKey { get; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[Internal]
	[Static]
	interface PdfDocumentWriteOptionKeys {

		[Field ("PDFDocumentOwnerPasswordOption", "+PDFKit")]
		NSString OwnerPasswordKey { get; }

		[Field ("PDFDocumentUserPasswordOption", "+PDFKit")]
		NSString UserPasswordKey { get; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Field ("PDFDocumentAccessPermissionsOption", "+PDFKit")]
		NSString AccessPermissionsKey { get; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Field ("PDFDocumentBurnInAnnotationsOption", "+PDFKit")]
		NSString BurnInAnnotationsKey { get; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Field ("PDFDocumentSaveTextFromOCROption", "+PDFKit")]
		NSString SaveTextFromOcrKey { get; }

		[iOS (16, 4), Mac (13, 3), MacCatalyst (16, 4)]
		[Field ("PDFDocumentSaveImagesAsJPEGOption", "+PDFKit")]
		NSString SaveImagesAsJpegKey { get; }

		[iOS (16, 4), Mac (13, 3), MacCatalyst (16, 4)]
		[Field ("PDFDocumentOptimizeImagesForScreenOption", "+PDFKit")]
		NSString OptimizeImagesForScreenKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("PdfDocumentWriteOptionKeys")]
	interface PdfDocumentWriteOptions {

		string OwnerPassword { get; set; }
		string UserPassword { get; set; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		string AccessPermissions { get; set; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		bool BurnInAnnotations { get; set; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		bool SaveTextFromOcr { get; set; }

		[iOS (16, 4), Mac (13, 3), MacCatalyst (16, 4)]
		bool SaveImagesAsJpeg { get; set; }

		[iOS (16, 4), Mac (13, 3), MacCatalyst (16, 4)]
		bool OptimizeImagesForScreen { get; set; }
	}

	[MacCatalyst (13, 1)]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFAction")]
	[Abstract]
	interface PdfAction : NSCopying {
		//This is an abstract superclass with no public init - should it have a private constructor??
		//As it is, I can create instances, that segfault when you access the type method.
		//marking the method as [Abstract] doesn't work because the subclasses do not explictly
		//define this method (although they implement it)
		[Export ("type")]
		string Type { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PdfAction), Name = "PDFActionGoTo")]
	interface PdfActionGoTo {

		[DesignatedInitializer]
		[Export ("initWithDestination:")]
		NativeHandle Constructor (PdfDestination destination);

		[Export ("destination")]
		PdfDestination Destination { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PdfAction), Name = "PDFActionNamed")]
	interface PdfActionNamed {

		[DesignatedInitializer]
		[Export ("initWithName:")]
		NativeHandle Constructor (PdfActionNamedName name);

		[Export ("name")]
		PdfActionNamedName Name { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PdfAction), Name = "PDFActionRemoteGoTo")]
	interface PdfActionRemoteGoTo {

		[DesignatedInitializer]
		[Export ("initWithPageIndex:atPoint:fileURL:")]
		NativeHandle Constructor (nint pageIndex, CGPoint point, NSUrl fileUrl);

		[Export ("pageIndex")]
		nint PageIndex { get; set; }

		[Export ("point")]
		CGPoint Point { get; set; }

		[Export ("URL")]
		NSUrl Url { get; set; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (PdfAction), Name = "PDFActionResetForm")]
	interface PdfActionResetForm {
		// - (instancetype)init NS_DESIGNATED_INITIALIZER;
		[Export ("init")]
		[DesignatedInitializer]
		NativeHandle Constructor ();

		//NSArray of NSString
		[Export ("fields"), NullAllowed]
		string [] Fields { get; set; }

		[Export ("fieldsIncludedAreCleared")]
		bool FieldsIncludedAreCleared { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (PdfAction), Name = "PDFActionURL")]
	interface PdfActionUrl {

		[DesignatedInitializer]
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("URL"), NullAllowed]
		NSUrl Url { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFAnnotation")]
	interface PdfAnnotation : NSCoding, NSCopying {

		[MacCatalyst (13, 1)]
		[Export ("initWithBounds:forType:withProperties:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect bounds, NSString annotationType, [NullAllowed] NSDictionary properties);

		[MacCatalyst (13, 1)]
		[Wrap ("this (bounds, annotationType.GetConstant ()!, properties)")]
		NativeHandle Constructor (CGRect bounds, PdfAnnotationKey annotationType, [NullAllowed] NSDictionary properties);

		[Deprecated (PlatformName.iOS, 11, 0, message: "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.")]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use '.ctor (CGRect, PDFAnnotationKey, NSDictionary)' instead.")]
		[Export ("initWithBounds:")]
		NativeHandle Constructor (CGRect bounds);

		[Export ("page")]
		[NullAllowed]
		PdfPage Page { get; set; }

#if NET
		[Protected]
		[Export ("type")]
		[NullAllowed]
		NSString Type { get; set; }
#else
		[Export ("type")]
		[NullAllowed]
		string Type { get; set; }
#endif

		[Export ("bounds")]
		CGRect Bounds { get; set; }

		[Export ("modificationDate")]
		[NullAllowed]
		NSDate ModificationDate { get; set; }

		[Export ("userName")]
		[NullAllowed]
		string UserName { get; set; }

		[Export ("popup")]
		[NullAllowed]
#if MONOMAC
		PdfAnnotationPopup Popup { get; set; }
#else
		PdfAnnotation Popup { get; set; }
#endif

		[Export ("shouldDisplay")]
		bool ShouldDisplay { get; set; }

		[Export ("shouldPrint")]
		bool ShouldPrint { get; set; }

		[Export ("border")]
		[NullAllowed]
		PdfBorder Border { get; set; }

		[Export ("color")]
		NSColor Color { get; set; }

		[NoiOS]
		[NullAllowed]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[NoMacCatalyst]
		[Export ("mouseUpAction")]
		PdfAction MouseUpAction { get; set; }

		[Export ("contents")]
		[NullAllowed]
		string Contents { get; set; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("toolTip")]
		[NullAllowed]
		string ToolTip { get; }

		[Export ("hasAppearanceStream")]
		bool HasAppearanceStream { get; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("removeAllAppearanceStreams")]
		void RemoveAllAppearanceStreams ();

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("drawWithBox:")]
		void Draw (PdfDisplayBox box);

		[MacCatalyst (13, 1)]
		[Export ("action", ArgumentSemantic.Strong), NullAllowed]
		PdfAction Action { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("highlighted")]
		bool Highlighted { [Bind ("isHighlighted")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("drawWithBox:inContext:")]
		void Draw (PdfDisplayBox box, CGContext context);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("setValue:forAnnotationKey:")]
		bool _SetValue (IntPtr value, NSString key);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("valueForAnnotationKey:")]
		[return: NullAllowed]
		IntPtr _GetValue (NSString key);

		[Protected]
		[MacCatalyst (13, 1)]
		[Export ("setBoolean:forAnnotationKey:")]
		bool SetValue (bool boolean, NSString key);

		[MacCatalyst (13, 1)]
		[Wrap ("SetValue (boolean, key.GetConstant ()!)")]
		bool SetValue (bool boolean, PdfAnnotationKey key);

		[Protected]
		[MacCatalyst (13, 1)]
		[Export ("setRect:forAnnotationKey:")]
		bool SetValue (CGRect rect, NSString key);

		[MacCatalyst (13, 1)]
		[Wrap ("SetValue (rect, key.GetConstant ()!)")]
		bool SetValue (CGRect rect, PdfAnnotationKey key);

		[MacCatalyst (13, 1)]
		[Export ("annotationKeyValues", ArgumentSemantic.Copy)]
		NSDictionary AnnotationKeyValues { get; }

		[Protected]
		[MacCatalyst (13, 1)]
		[Export ("removeValueForAnnotationKey:")]
		void RemoveValue (NSString key);

		[MacCatalyst (13, 1)]
		[Wrap ("RemoveValue (key.GetConstant ()!)")]
		void RemoveValue (PdfAnnotationKey key);

		// PDFAnnotation (PDFAnnotationUtilities) Category

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("font", ArgumentSemantic.Copy)]
		NSFont Font { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fontColor", ArgumentSemantic.Copy)]
		NSColor FontColor { get; set; }

		[iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("interiorColor", ArgumentSemantic.Copy)]
		NSColor InteriorColor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("alignment", ArgumentSemantic.Assign)]
		NSTextAlignment Alignment { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("startPoint", ArgumentSemantic.Assign)]
		CGPoint StartPoint { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("endPoint", ArgumentSemantic.Assign)]
		CGPoint EndPoint { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("startLineStyle", ArgumentSemantic.Assign)]
		PdfLineStyle StartLineStyle { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("endLineStyle", ArgumentSemantic.Assign)]
		PdfLineStyle EndLineStyle { get; set; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("lineStyleFromName:")]
		PdfLineStyle GetLineStyle (string fromName);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("nameForLineStyle:")]
		string GetName (PdfLineStyle style);

		[MacCatalyst (13, 1)]
		[Export ("iconType", ArgumentSemantic.Assign)]
		PdfTextAnnotationIconType IconType { get; set; }

		[Internal]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("quadrilateralPoints", ArgumentSemantic.Copy)]
		IntPtr _QuadrilateralPoints { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("markupType", ArgumentSemantic.Assign)]
		PdfMarkupType MarkupType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("widgetFieldType")]
		string WidgetFieldType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("widgetControlType", ArgumentSemantic.Assign)]
		PdfWidgetControlType WidgetControlType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("multiline")]
		bool Multiline { [Bind ("isMultiline")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("isPasswordField")]
		bool IsPasswordField { get; }

		[MacCatalyst (13, 1)]
		[Export ("comb")]
		bool Comb { [Bind ("hasComb")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumLength")]
		nint MaximumLength { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("widgetStringValue")]
		string WidgetStringValue { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("widgetDefaultStringValue")]
		string WidgetDefaultStringValue { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsToggleToOff")]
		bool AllowsToggleToOff { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("radiosInUnison")]
		bool RadiosInUnison { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("readOnly")]
		bool ReadOnly { [Bind ("isReadOnly")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("listChoice")]
		bool ListChoice { [Bind ("isListChoice")] get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("choices", ArgumentSemantic.Copy)]
		string [] Choices { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("values", ArgumentSemantic.Copy)]
		string [] Values { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("buttonWidgetState", ArgumentSemantic.Assign)]
		PdfWidgetCellState ButtonWidgetState { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("buttonWidgetStateString")]
		string ButtonWidgetStateString { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("open")]
		bool Open { [Bind ("isOpen")] get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("paths")]
		NSBezierPath [] Paths { get; }

		[MacCatalyst (13, 1)]
		[Export ("addBezierPath:")]
		void AddBezierPath (NSBezierPath path);

		[MacCatalyst (13, 1)]
		[Export ("removeBezierPath:")]
		void RemoveBezierPath (NSBezierPath path);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("destination", ArgumentSemantic.Strong)]
		PdfDestination Destination { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fieldName")]
		string FieldName { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("caption")]
		string Caption { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("backgroundColor", ArgumentSemantic.Copy)]
		NSColor BackgroundColor { get; set; }

		[iOS (11, 2)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("stampName")]
		string StampName { get; set; }

		[iOS(17, 0), Mac (14, 0), MacCatalyst (17, 0)]
		[Export("activatableTextField")]
		bool ActivatableTextField { [Bind("isActivatableTextField")] get; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
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
	[NoMacCatalyst]
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
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationCircle")]
	interface PdfAnnotationCircle {
		[Export ("interiorColor")]
		NSColor InteriorColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
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
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationInk")]
	interface PdfAnnotationInk {
		[Export ("paths")]
		NSBezierPath [] Paths { get; }

#if !NET
		[Export ("addBezierPath:")]
		void AddBezierPathpath (NSBezierPath path);

		[Export ("removeBezierPath:")]
		void RemoveBezierPathpath (NSBezierPath path);
#else
		[Export ("addBezierPath:")]
		void AddBezierPath (NSBezierPath path);

		[Export ("removeBezierPath:")]
		void RemoveBezierPath (NSBezierPath path);
#endif
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
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
	[NoMacCatalyst]
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
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationMarkup")]
	interface PdfAnnotationMarkup {
		[Export ("quadrilateralPoints", ArgumentSemantic.Assign), NullAllowed]
		NSArray WeakQuadrilateralPoints { get; set; }

		[Export ("markupType")]
		PdfMarkupType MarkupType { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationPopup")]
	interface PdfAnnotationPopup {
		[Export ("isOpen")]
		bool IsOpen { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationSquare")]
	interface PdfAnnotationSquare {
		[Export ("interiorColor")]
		NSColor InteriorColor { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationStamp")]
	interface PdfAnnotationStamp {
		[Export ("name")]
		string Name { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
	[BaseType (typeof (PdfAnnotation), Name = "PDFAnnotationText")]
	interface PdfAnnotationText {
		[Export ("iconType")]
		PdfTextAnnotationIconType IconType { get; set; }
	}

	[NoiOS]
	[Deprecated (PlatformName.MacOSX, 10, 12)]
	[NoMacCatalyst]
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

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFBorder")]
	interface PdfBorder : NSCoding, NSCopying {
		[Export ("style")]
		PdfBorderStyle Style { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("dashPattern", ArgumentSemantic.Assign), NullAllowed]
		NSArray WeakDashPattern { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("borderKeyValues", ArgumentSemantic.Copy)]
		NSDictionary WeakBorderKeyValues { get; }

		[Export ("drawInRect:")]
		void Draw (CGRect rect);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFDestination")]
	interface PdfDestination : NSCopying {

		[MacCatalyst (13, 1)]
		[Field ("kPDFDestinationUnspecifiedValue")]
		nfloat UnspecifiedValue { get; }

		[DesignatedInitializer]
		[Export ("initWithPage:atPoint:")]
		NativeHandle Constructor (PdfPage page, CGPoint point);

		[Export ("page")]
		[NullAllowed]
		PdfPage Page { get; }

		[Export ("point")]
		CGPoint Point { get; }

		[Export ("zoom")]
		nfloat Zoom { get; set; }

		//Should Compare be more more .Net ified ?
		[Export ("compare:")]
		NSComparisonResult Compare (PdfDestination destination);
	}

	//Add attributes for delegates/events
	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
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

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Field ("PDFDocumentFoundSelectionKey")]
		NSString FoundSelectionKey { get; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Field ("PDFDocumentPageIndexKey")]
		NSString PageIndexKey { get; }

		// - (instancetype)init NS_DESIGNATED_INITIALIZER;
		[Export ("init")]
		[DesignatedInitializer]
		NativeHandle Constructor ();

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithData:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data);

		[Export ("documentURL")]
		[NullAllowed]
		NSUrl DocumentUrl { get; }

		[Export ("documentRef")]
		[NullAllowed]
		CGPDFDocument Document { get; }

		[Advice ("Use the strongly typed '[Get|Set]DocumentAttributes' instead.")]
		[Export ("documentAttributes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSDictionary DocumentAttributes { get; set; }

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0)]
		[Export ("accessPermissions")]
		PdfAccessPermissions AccessPermissions { get; }

		[Wrap ("new PdfDocumentAttributes (DocumentAttributes)")]
		PdfDocumentAttributes GetDocumentAttributes ();

		[Wrap ("DocumentAttributes = attributes?.GetDictionary ()")]
		void SetDocumentAttributes ([NullAllowed] PdfDocumentAttributes attributes);

#if NET || IOS
		[Export ("majorVersion")]
		nint MajorVersion { get; }

		[Export ("minorVersion")]
		nint MinorVersion { get; }
#else
		[NoiOS]
		[Export ("majorVersion")]
		int MajorVersion { get; } /* int, not NSInteger */

		[NoiOS]
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

		[MacCatalyst (13, 1)]
		[Export ("allowsDocumentChanges")]
		bool AllowsDocumentChanges { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsDocumentAssembly")]
		bool AllowsDocumentAssembly { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsContentAccessibility")]
		bool AllowsContentAccessibility { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsCommenting")]
		bool AllowsCommenting { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsFormFieldEntry")]
		bool AllowsFormFieldEntry { get; }

		[Export ("permissionsStatus")]
		PdfDocumentPermissions PermissionsStatus { get; }

		[Export ("string")]
		[NullAllowed]
		string Text { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PdfDocumentDelegate Delegate { get; set; }

		[Export ("dataRepresentation")]
		[return: NullAllowed]
		NSData GetDataRepresentation ();

		[Export ("dataRepresentationWithOptions:")]
		[return: NullAllowed]
		NSData GetDataRepresentation (NSDictionary options);

		[Export ("writeToFile:")]
		bool Write (string path);

		[Export ("writeToFile:withOptions:")]
		bool Write (string path, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("Write (path, options.GetDictionary ()!)")]
		bool Write (string path, PdfDocumentWriteOptions options);

		[Export ("writeToURL:")]
		bool Write (NSUrl url);

		[Export ("writeToURL:withOptions:")]
		bool Write (NSUrl url, [NullAllowed] NSDictionary options);

		[MacCatalyst (13, 1)]
		[Wrap ("Write (url, options.GetDictionary ()!)")]
		bool Write (NSUrl url, PdfDocumentWriteOptions options);

		[NullAllowed]
		[Export ("outlineRoot")]
		PdfOutline OutlineRoot { get; set; }

		[Export ("outlineItemForSelection:")]
		[return: NullAllowed]
		PdfOutline OutlineItem (PdfSelection selection);

		[Export ("pageCount")]
		nint PageCount { get; }

		[Export ("pageAtIndex:")]
		[return: NullAllowed]
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
#if MONOMAC && !NET
		[Obsolete ("Use 'Find (string, NSStringCompareOptions)' instead.")]
		PdfSelection [] Find (string text, nint options);

		[Wrap ("Find (text: text, options: (nint) (int) compareOptions)", IsVirtual = true)]
#endif
		PdfSelection [] Find (string text, NSStringCompareOptions compareOptions);

		[Export ("beginFindString:withOptions:")]
#if MONOMAC && !NET
		[Obsolete ("Use 'FindAsync (string, NSStringCompareOptions)' instead.")]
		[return: NullAllowed]
		void FindAsync (string text, nint options);

		[Wrap ("FindAsync (text: text, options: (nint) (int) compareOptions)", IsVirtual = true)]
#endif
		[return: NullAllowed]
		void FindAsync (string text, NSStringCompareOptions compareOptions);

		[Export ("beginFindStrings:withOptions:")]
#if MONOMAC && !NET
		[Obsolete ("Use 'FindAsync (string [], NSStringCompareOptions)' instead.")]
		[return: NullAllowed]
		void FindAsync (string [] text, nint options);

		[Wrap ("FindAsync (text: text, options: (nint) (int) compareOptions)", IsVirtual = true)]
#endif
		[return: NullAllowed]
		void FindAsync (string [] text, NSStringCompareOptions compareOptions);

		[Export ("findString:fromSelection:withOptions:")]
#if MONOMAC && !NET
		[Obsolete ("Use 'Find (string, PdfSelection, NSStringCompareOptions)' instead.")]
		[return: NullAllowed]
		PdfSelection Find (string text, [NullAllowed] PdfSelection selection, nint options);

		[Wrap ("Find (text: text, selection: selection, options: (nint) (int) compareOptions)", IsVirtual = true)]
#endif
		[return: NullAllowed]
		PdfSelection Find (string text, [NullAllowed] PdfSelection selection, NSStringCompareOptions compareOptions);

		[Export ("isFinding")]
		bool IsFinding { get; }

		[Export ("cancelFindString")]
		void CancelFind ();

		[Export ("selectionForEntireDocument")]
		[return: NullAllowed]
		PdfSelection SelectEntireDocument ();

		[Export ("selectionFromPage:atPoint:toPage:atPoint:")]
		[return: NullAllowed]
		PdfSelection GetSelection (PdfPage startPage, CGPoint startPoint, PdfPage endPage, CGPoint endPoint);

		[Export ("selectionFromPage:atCharacterIndex:toPage:atCharacterIndex:")]
		[return: NullAllowed]
		PdfSelection GetSelection (PdfPage startPage, nint startCharIndex, PdfPage endPage, nint endCharIndex);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("printOperationForPrintInfo:scalingMode:autoRotate:")]
		[return: NullAllowed]
#pragma warning disable 0618 // 'PdfPrintScalingMode' is obsolete: 'This type is not available on iOS.'
		NSPrintOperation GetPrintOperation ([NullAllowed] NSPrintInfo printInfo, PdfPrintScalingMode scaleMode, bool doRotate);
#pragma warning restore
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFDocumentDelegate")]
#if IOS
	[Protocol]
#else
	[Protocol (FormalSince = "10.13", Name = "PDFDocumentDelegate")]
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

		[MacCatalyst (13, 1)]
		[DelegateName ("ClassForAnnotationTypeDelegate"), DefaultValue (null)]
		[Export ("classForAnnotationType:")]
		Class GetClassForAnnotationType (string annotationType);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12, message: "Use 'GetClassForAnnotationType' instead.")]
		[NoMacCatalyst]
		[Export ("classForAnnotationClass:"), DelegateName ("ClassForAnnotationClassDelegate"), DefaultValue (null)]
#if NET
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

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "PDFOutline")]
	interface PdfOutline {

		// - (instancetype)init NS_DESIGNATED_INITIALIZER;
		[Export ("init")]
		[DesignatedInitializer]
		NativeHandle Constructor ();

		[Export ("document")]
		[NullAllowed]
		PdfDocument Document { get; }

		[Export ("parent"), NullAllowed]
		PdfOutline Parent { get; }

		[Export ("numberOfChildren")]
		nint ChildrenCount { get; }

		[Export ("index")]
		nint Index { get; }

		[return: NullAllowed]
		[Export ("childAtIndex:")]
		PdfOutline Child (nint index);

		[Export ("insertChild:atIndex:")]
		void InsertChild (PdfOutline child, nint index);

		[Export ("removeFromParent")]
		void RemoveFromParent ();

		[Export ("label")]
		[NullAllowed]
		string Label { get; set; }

		[Export ("isOpen")]
		bool IsOpen { get; set; }

		[Export ("destination"), NullAllowed]
		PdfDestination Destination { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("action"), NullAllowed]
		PdfAction Action { get; set; }
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject), Name = "PDFPage")]
	interface PdfPage : NSCopying {

		// - (instancetype)init NS_DESIGNATED_INITIALIZER;
		[Export ("init")]
		[DesignatedInitializer]
		NativeHandle Constructor ();

		[Export ("initWithImage:")]
		NativeHandle Constructor (NSImage image);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[DesignatedInitializer]
		[Export ("initWithImage:options:")]
		NativeHandle Constructor (NSImage image, NSDictionary options);

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Wrap ("this (image, options.GetDictionary ()!)")]
		NativeHandle Constructor (NSImage image, PdfPageImageInitializationOption options);

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

		[MacCatalyst (13, 1)]
		[Export ("transformForBox:")]
		CGAffineTransform GetTransform (PdfDisplayBox box);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("drawWithBox:")]
		void Draw (PdfDisplayBox box);

		[MacCatalyst (13, 1)]
		[Export ("drawWithBox:toContext:")]
		void Draw (PdfDisplayBox box, CGContext context);

		[MacCatalyst (13, 1)]
		[Export ("transformContext:forBox:")]
		void TransformContext (CGContext context, PdfDisplayBox box);

		[MacCatalyst (13, 1)]
		[Export ("thumbnailOfSize:forBox:")]
		NSImage GetThumbnail (CGSize size, PdfDisplayBox box);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("transformContextForBox:")]
		void TransformContext (PdfDisplayBox box);

		[Export ("numberOfCharacters")]
		nint CharacterCount { get; }

		[Export ("string")]
		[NullAllowed]
		string Text { get; }

		[Export ("attributedString")]
		[NullAllowed]
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

		[Export ("dataRepresentation"), NullAllowed]
		NSData DataRepresentation { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFSelection")]
	[DisableDefaultCtor] // An uncaught exception was raised: init: not a valid initializer for PDFSelection
	interface PdfSelection : NSCopying {

		[DesignatedInitializer]
		[Export ("initWithDocument:")]
		NativeHandle Constructor (PdfDocument document);

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

		[Export ("numberOfTextRangesOnPage:")]
		nuint GetNumberOfTextRanges (PdfPage page);

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

		[MacCatalyst (13, 1)]
		[Export ("extendSelectionForLineBoundaries")]
		void ExtendSelectionForLineBoundaries ();

		[Export ("drawForPage:active:")]
		void Draw (PdfPage page, bool active);

		[Export ("drawForPage:withBox:active:")]
		void Draw (PdfPage page, PdfDisplayBox box, bool active);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSView), Name = "PDFThumbnailView")]
	interface PdfThumbnailView : NSCoding {

		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Field ("PDFThumbnailViewDocumentEditedNotification", "+PDFKit")]
		[Notification]
		NSString DocumentEditedNotification { get; }

		[Export ("PDFView", ArgumentSemantic.Weak)]
		[NullAllowed]
		PdfView PdfView { get; set; }

		[Export ("thumbnailSize")]
		CGSize ThumbnailSize { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("layoutMode")]
		PdfThumbnailLayoutMode LayoutMode { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("contentInset")]
		NSEdgeInsets ContentInset { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("maximumNumberOfColumns")]
		nint MaximumNumberOfColumns { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("labelFont")]
		[NullAllowed]
		NSFont LabelFont { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSColor BackgroundColor { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("allowsDragging")]
		bool AllowsDragging { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[Export ("allowsMultipleSelection")]
		bool AllowsMultipleSelection { get; set; }

		[Export ("selectedPages", ArgumentSemantic.Strong), NullAllowed]
		PdfPage [] SelectedPages { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSView), Name = "PDFView", Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (PdfViewDelegate) })]
	interface PdfView :
#if IOS
	UIGestureRecognizerDelegate, UIFindInteractionDelegate
#else
	NSMenuDelegate, NSAnimationDelegate
#endif
	{
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("document"), NullAllowed]
		PdfDocument Document { get; set; }

		[Export ("canGoToFirstPage")]
		bool CanGoToFirstPage { get; }

		//Verify
		[Export ("goToFirstPage:")]
		void GoToFirstPage ([NullAllowed] NSObject sender);

		[Export ("canGoToLastPage")]
		bool CanGoToLastPage { get; }

		[Export ("goToLastPage:")]
		void GoToLastPage ([NullAllowed] NSObject sender);

		[Export ("canGoToNextPage")]
		bool CanGoToNextPage { get; }

		[Export ("goToNextPage:")]
		void GoToNextPage ([NullAllowed] NSObject sender);

		[Export ("canGoToPreviousPage")]
		bool CanGoToPreviousPage { get; }

		[Export ("goToPreviousPage:")]
		void GoToPreviousPage ([NullAllowed] NSObject sender);

		[Export ("canGoBack")]
		bool CanGoBack { get; }

		[Export ("goBack:")]
		void GoBack ([NullAllowed] NSObject sender);

		[Export ("canGoForward")]
		bool CanGoForward { get; }

		[Export ("goForward:")]
		void GoForward ([NullAllowed] NSObject sender);

		[Export ("currentPage")]
		[NullAllowed]
		PdfPage CurrentPage { get; }

		[Export ("goToPage:")]
		void GoToPage (PdfPage page);

		[Export ("currentDestination")]
		[NullAllowed]
		PdfDestination CurrentDestination { get; }

		[Export ("goToDestination:")]
		void GoToDestination (PdfDestination destination);

		[Export ("goToSelection:")]
		void GoToSelection (PdfSelection selection);

		[Export ("goToRect:onPage:")]
		void GoToRectangle (CGRect rect, PdfPage page);

		[Export ("displayMode")]
		PdfDisplayMode DisplayMode { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("displayDirection")]
		PdfDisplayDirection DisplayDirection { get; set; }

		[Export ("displaysPageBreaks")]
		bool DisplaysPageBreaks { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("pageBreakMargins")]
		NSEdgeInsets PageBreakMargins { get; set; }

		[Export ("displayBox")]
		PdfDisplayBox DisplayBox { get; set; }

		[Export ("displaysAsBook")]
		bool DisplaysAsBook { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("displaysRTL")]
		bool DisplaysRtl { get; set; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("shouldAntiAlias")]
		bool ShouldAntiAlias { get; set; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("greekingThreshold")]
		nfloat GreekingThreshold { get; set; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("takeBackgroundColorFrom:")]
		void TakeBackgroundColor (NSObject sender);

		[Export ("backgroundColor")]
		NSColor BackgroundColor { get; set; }

		[Export ("interpolationQuality", ArgumentSemantic.Assign)]
		PdfInterpolationQuality InterpolationQuality { get; set; }

		[iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("pageShadowsEnabled")]
		bool PageShadowsEnabled { get; [Bind ("enablePageShadows:")] set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("usePageViewController:withViewOptions:")]
		void UsePageViewController (bool enable, [NullAllowed] NSDictionary viewOptions);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("isUsingPageViewController")]
		bool IsUsingPageViewController { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		PdfViewDelegate Delegate { get; set; }

		[Export ("scaleFactor")]
		nfloat ScaleFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("minScaleFactor")]
		nfloat MinScaleFactor { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maxScaleFactor")]
		nfloat MaxScaleFactor { get; set; }

		[Export ("zoomIn:")]
		void ZoomIn ([NullAllowed] NSObject sender);

		[Export ("canZoomIn")]
		bool CanZoomIn { get; }

		[Export ("zoomOut:")]
		void ZoomOut ([NullAllowed] NSObject sender);

		[Export ("canZoomOut")]
		bool CanZoomOut { get; }

		[Export ("autoScales")]
		bool AutoScales { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("scaleFactorForSizeToFit")]
		nfloat ScaleFactorForSizeToFit { get; }

		[Export ("areaOfInterestForMouse:")]
		PdfAreaOfInterest GetAreaOfInterest (NSEvent mouseEvent);

		[MacCatalyst (13, 1)]
		[Export ("areaOfInterestForPoint:")]
		PdfAreaOfInterest GetAreaOfInterest (CGPoint point);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("setCursorForAreaOfInterest:")]
		void SetCursor (PdfAreaOfInterest area);

		[Export ("performAction:")]
		void PerformAction (PdfAction action);

		[Export ("currentSelection")]
		[NullAllowed]
		PdfSelection CurrentSelection { get; set; }

		[Export ("setCurrentSelection:animate:")]
		void SetCurrentSelection ([NullAllowed] PdfSelection selection, bool animate);

		[Export ("clearSelection")]
		void ClearSelection ();

		[Export ("selectAll:")]
		void SelectAll ([NullAllowed] NSObject sender);

		[Export ("scrollSelectionToVisible:")]
		void ScrollSelectionToVisible ([NullAllowed] NSObject sender);

		[Export ("highlightedSelections")]
		[NullAllowed]
		PdfSelection [] HighlightedSelections { get; set; }

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("takePasswordFrom:")]
		void TakePasswordFrom (NSObject sender);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("drawPage:")]
		void DrawPage (PdfPage page);

		[MacCatalyst (13, 1)]
		[Export ("drawPage:toContext:")]
		void DrawPage (PdfPage page, CGContext context);

		[MacCatalyst (13, 1)]
		[Export ("drawPagePost:toContext:")]
		void DrawPagePost (PdfPage page, CGContext context);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 12)]
		[NoMacCatalyst]
		[Export ("drawPagePost:")]
		void DrawPagePost (PdfPage page);

		[Export ("copy:")]
		void Copy ([NullAllowed] NSObject sender);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("printWithInfo:autoRotate:")]
		void Print (NSPrintInfo printInfo, bool doRotate);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("printWithInfo:autoRotate:pageScaling:")]
#pragma warning disable // 0618: 'PdfPrintScalingMode' is obsolete: 'This type is not available on iOS.'
		void Print (NSPrintInfo printInfo, bool doRotate, PdfPrintScalingMode scaleMode);
#pragma warning restore

		[Export ("pageForPoint:nearest:")]
		[return: NullAllowed]
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
		[NullAllowed]
		NSView DocumentView { get; }

		[Export ("layoutDocumentView")]
		void LayoutDocumentView ();

		[Export ("annotationsChangedOnPage:")]
		void AnnotationsChanged (PdfPage page);

		[Export ("rowSizeForPage:")]
		CGSize RowSize (PdfPage page);

		[NoiOS]
		[Deprecated (PlatformName.MacOSX, 10, 13)]
		[NoMacCatalyst]
		[Export ("allowsDragging")]
		bool AllowsDragging { get; set; }

		[Export ("visiblePages")]
		PdfPage [] VisiblePages { get; }

		[Export ("enableDataDetectors")]
		bool EnableDataDetectors { get; set; }

		[Field ("PDFViewChangedHistoryNotification", "+PDFKit")]
		[Notification]
		NSString ChangedHistoryNotification { get; }

		[Field ("PDFViewDocumentChangedNotification", "+PDFKit")]
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

		[NoiOS]
		[NoMacCatalyst]
		[Export ("acceptsDraggedFiles")]
		bool AcceptsDraggedFiles { get; set; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[NullAllowed, Export ("pageOverlayViewProvider", ArgumentSemantic.Weak)]
		IPdfPageOverlayViewProvider PageOverlayViewProvider { get; set; }

		[iOS (16, 0), Mac (13, 0), MacCatalyst (16, 0)]
		[Export ("inMarkupMode")]
		bool InMarkupMode { [Bind ("isInMarkupMode")] get; set; }

		[iOS (16, 0), NoMac, MacCatalyst (16, 0)]
		[Export ("findInteraction")]
		UIFindInteraction FindInteraction { get; }

		[iOS (16, 0), NoMac, MacCatalyst (16, 0)]
		[Export ("findInteractionEnabled")]
		bool FindInteractionEnabled { [Bind ("isFindInteractionEnabled")] get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
	interface PdfViewAnnotationHitEventArgs {
		[Export ("PDFAnnotationHit")]
		PdfAnnotation AnnotationHit { get; }
	}

	//Verify delegate methods.  There are default actions (not just return null ) that should occur
	//if the delegate does not implement the method.
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "PDFViewDelegate")]
#if IOS
	[Protocol]
#else
	[Protocol (FormalSince = "10.12", Name = "PDFViewDelegate")]
#endif
	[Model]
	interface PdfViewDelegate {
		//from docs: 'By default, the scale factor is restricted to a range between 0.1 and 10.0 inclusive.'
		[NoiOS]
		[NoMacCatalyst]
		[Export ("PDFViewWillChangeScaleFactor:toScale:"), DelegateName ("PdfViewScale"), DefaultValueFromArgument ("scale")]
		nfloat WillChangeScaleFactor (PdfView sender, nfloat scale);

		[Export ("PDFViewWillClickOnLink:withURL:"), EventArgs ("PdfViewUrl")]
		void WillClickOnLink (PdfView sender, NSUrl url);

		// from the docs: 'By default, this method uses the string, if any, associated with the
		// 'Title' key in the view's PDFDocument attribute dictionary. If there is no such string,
		// this method uses the last path component if the document is URL-based.
		[NoiOS]
		[NoMacCatalyst]
		[Export ("PDFViewPrintJobTitle:"), DelegateName ("PdfViewTitle"), DefaultValue ("String.Empty")]
		string TitleOfPrintJob (PdfView sender);

		[Export ("PDFViewPerformFind:"), EventArgs ("PdfView")]
		void PerformFind (PdfView sender);

		[Export ("PDFViewPerformGoToPage:"), EventArgs ("PdfView")]
		void PerformGoToPage (PdfView sender);

		[NoiOS]
		[NoMacCatalyst]
		[Export ("PDFViewPerformPrint:"), EventArgs ("PdfView")]
		void PerformPrint (PdfView sender);

		[Export ("PDFViewOpenPDF:forRemoteGoToAction:"), EventArgs ("PdfViewAction")]
		void OpenPdf (PdfView sender, PdfActionRemoteGoTo action);

		[iOS (13, 0)]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("PDFViewParentViewController")]
		UIViewController ParentViewController { get; }
	}

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	interface IPdfPageOverlayViewProvider { }

	[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Protocol (Name = "PDFPageOverlayViewProvider")]
	interface PdfPageOverlayViewProvider {
		[Abstract]
		[Export ("pdfView:overlayViewForPage:")]
		[return: NullAllowed]
		UIView GetOverlayView (PdfView view, PdfPage page);

		[Export ("pdfView:willDisplayOverlayView:forPage:")]
		void WillDisplayOverlayView (PdfView pdfView, UIView overlayView, PdfPage page);

		[Export ("pdfView:willEndDisplayingOverlayView:forPage:")]
		void WillEndDisplayingOverlayView (PdfView pdfView, UIView overlayView, PdfPage page);
	}
}
