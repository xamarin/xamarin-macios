using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Accessibility {

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXCategoricalDataAxisDescriptor : AXDataAxisDescriptor {
		[Export ("categoryOrder", ArgumentSemantic.Copy)]
		string [] CategoryOrder { get; set; }

		[Export ("initWithTitle:categoryOrder:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, string [] categoryOrder);

		[Export ("initWithAttributedTitle:categoryOrder:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedTitle, string [] categoryOrder);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface AXChart {
		[Abstract]
		[NullAllowed, Export ("accessibilityChartDescriptor", ArgumentSemantic.Strong)]
		AXChartDescriptor AccessibilityChartDescriptor { get; set; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum AXChartDescriptorContentDirection : long {
		LeftToRight = 0,
		RightToLeft,
		TopToBottom,
		BottomToTop,
		RadialClockwise,
		RadialCounterClockwise,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXChartDescriptor : NSCopying {
		[NullAllowed, Export ("title")]
		string Title { get; set; }

		[NullAllowed, Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[NullAllowed, Export ("summary")]
		string Summary { get; set; }

		[Export ("contentDirection", ArgumentSemantic.Assign)]
		AXChartDescriptorContentDirection ContentDirection { get; set; }

		[Export ("contentFrame", ArgumentSemantic.Assign)]
		CGRect ContentFrame { get; set; }

		[Export ("series", ArgumentSemantic.Copy)]
		AXDataSeriesDescriptor [] Series { get; set; }

		[Export ("xAxis", ArgumentSemantic.Strong)]
		IAXDataAxisDescriptor XAxis { get; set; }

		[NullAllowed, Export ("yAxis", ArgumentSemantic.Strong)]
		AXNumericDataAxisDescriptor YAxis { get; set; }

		[NullAllowed, Export ("additionalAxes", ArgumentSemantic.Copy)]
		IAXDataAxisDescriptor [] AdditionalAxes { get; set; }

		[Export ("initWithTitle:summary:xAxisDescriptor:yAxisDescriptor:series:")]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] string summary, IAXDataAxisDescriptor xAxis, [NullAllowed] AXNumericDataAxisDescriptor yAxis, AXDataSeriesDescriptor [] series);

		[Export ("initWithAttributedTitle:summary:xAxisDescriptor:yAxisDescriptor:series:")]
		NativeHandle Constructor ([NullAllowed] NSAttributedString attributedTitle, [NullAllowed] string summary, IAXDataAxisDescriptor xAxis, AXNumericDataAxisDescriptor yAxis, AXDataSeriesDescriptor [] series);

		[Export ("initWithTitle:summary:xAxisDescriptor:yAxisDescriptor:additionalAxes:series:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string title, [NullAllowed] string summary, IAXDataAxisDescriptor xAxis, [NullAllowed] AXNumericDataAxisDescriptor yAxis, [NullAllowed] IAXDataAxisDescriptor [] additionalAxes, AXDataSeriesDescriptor [] series);

		[Export ("initWithAttributedTitle:summary:xAxisDescriptor:yAxisDescriptor:additionalAxes:series:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSAttributedString attributedTitle, [NullAllowed] string summary, IAXDataAxisDescriptor xAxis, [NullAllowed] AXNumericDataAxisDescriptor yAxis, [NullAllowed] IAXDataAxisDescriptor [] additionalAxes, AXDataSeriesDescriptor [] series);
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum AXCustomContentImportance : ulong {
		Default,
		High,
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXCustomContent : NSCopying, NSSecureCoding {
		[Static]
		[Export ("customContentWithLabel:value:")]
		AXCustomContent Create (string label, string value);

		[Static]
		[Export ("customContentWithAttributedLabel:attributedValue:")]
		AXCustomContent Create (NSAttributedString label, NSAttributedString value);

		[Export ("label")]
		string Label { get; }

		[Export ("attributedLabel", ArgumentSemantic.Copy)]
		NSAttributedString AttributedLabel { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("attributedValue", ArgumentSemantic.Copy)]
		NSAttributedString AttributedValue { get; }

		[Export ("importance", ArgumentSemantic.Assign)]
		AXCustomContentImportance Importance { get; set; }
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface AXCustomContentProvider {
		[Abstract]
		[NullAllowed, Export ("accessibilityCustomContent", ArgumentSemantic.Copy)]
		AXCustomContent [] AccessibilityCustomContent { get; set; }

		[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("accessibilityCustomContentBlock", ArgumentSemantic.Copy)]
		Func<AXCustomContent []?> AccessibilityCustomContentHandler { get; set; }

	}

	interface IAXDataAxisDescriptor { }

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface AXDataAxisDescriptor : NSCopying {
		[Abstract]
		[Export ("title")]
		string Title { get; set; }

		[Abstract]
		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXDataPoint : NSCopying {
		[Export ("xValue", ArgumentSemantic.Copy)]
		AXDataPointValue XValue { get; set; }

		[NullAllowed, Export ("yValue", ArgumentSemantic.Copy)]
		AXDataPointValue YValue { get; set; }

		[Export ("additionalValues", ArgumentSemantic.Copy)]
		AXDataPointValue [] AdditionalValues { get; set; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("attributedLabel", ArgumentSemantic.Copy)]
		NSAttributedString AttributedLabel { get; set; }

		[Export ("initWithX:y:")]
		NativeHandle Constructor (AXDataPointValue xValue, [NullAllowed] AXDataPointValue yValue);

		[Export ("initWithX:y:additionalValues:")]
		NativeHandle Constructor (AXDataPointValue xValue, [NullAllowed] AXDataPointValue yValue, [NullAllowed] AXDataPointValue [] additionalValues);

		[Export ("initWithX:y:additionalValues:label:")]
		[DesignatedInitializer]
		NativeHandle Constructor (AXDataPointValue xValue, [NullAllowed] AXDataPointValue yValue, [NullAllowed] AXDataPointValue [] additionalValues, [NullAllowed] string label);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXDataPointValue : NSCopying {
		[Export ("number", ArgumentSemantic.Assign)]
		double Number { get; set; }

		[Export ("category")]
		string Category { get; set; }

		[Static]
		[Export ("valueWithNumber:")]
		AXDataPointValue CreateValueWithNumber (double number);

		[Static]
		[Export ("valueWithCategory:")]
		AXDataPointValue CreateValueWithCategory (string category);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXDataSeriesDescriptor : NSCopying {
		[NullAllowed, Export ("name")]
		string Name { get; set; }

		[Export ("attributedName", ArgumentSemantic.Copy)]
		NSAttributedString AttributedName { get; set; }

		[Export ("isContinuous")]
		bool IsContinuous { get; set; }

		[Export ("dataPoints", ArgumentSemantic.Copy)]
		AXDataPoint [] DataPoints { get; set; }

		[Export ("initWithName:isContinuous:dataPoints:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string name, bool isContinuous, AXDataPoint [] dataPoints);

		[Export ("initWithAttributedName:isContinuous:dataPoints:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedName, bool isContinuous, AXDataPoint [] dataPoints);
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum AXHearingDeviceEar : ulong {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right,
	}

	[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Static]
	[Partial]
	partial interface AXHearingUtilities {
		[Field ("AXMFiHearingDeviceStreamingEarDidChangeNotification")]
		[Notification]
		NSString StreamingEarDidChangeNotification { get; }

		[Field ("AXMFiHearingDevicePairedUUIDsDidChangeNotification")]
		[Notification]
		NSString PairedUUIDsDidChangeNotification { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXLiveAudioGraph {
		[Static]
		[Export ("start")]
		void Start ();

		[Static]
		[Export ("updateValue:")]
		void Update (double value);

		[Static]
		[Export ("stop")]
		void Stop ();
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum AXNumericDataAxisDescriptorScale : long {
		Linear = 0,
		Log10,
		Ln,
	}

	delegate NSString ValueDescriptionProviderHandler (double dataValue);

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXNumericDataAxisDescriptor : AXDataAxisDescriptor {
		[Export ("scaleType", ArgumentSemantic.Assign)]
		AXNumericDataAxisDescriptorScale ScaleType { get; set; }

		[Export ("lowerBound")]
		double LowerBound { get; set; }

		[Export ("upperBound")]
		double UpperBound { get; set; }

		[Export ("valueDescriptionProvider", ArgumentSemantic.Copy)]
		ValueDescriptionProviderHandler ValueDescriptionProvider { get; set; }

		[Export ("gridlinePositions", ArgumentSemantic.Copy)]
		NSNumber [] GridlinePositions { get; set; }

		[Export ("initWithTitle:lowerBound:upperBound:gridlinePositions:valueDescriptionProvider:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string title, double lowerBound, double upperBound, [NullAllowed] NSNumber [] gridlinePositions, Func<double, NSString> valueDescriptionProvider);

		[Export ("initWithAttributedTitle:lowerBound:upperBound:gridlinePositions:valueDescriptionProvider:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSAttributedString attributedTitle, double lowerBound, double upperBound, [NullAllowed] NSNumber [] gridlinePositions, Func<double, NSString> valueDescriptionProvider);
	}

	[TV (15, 2), Mac (12, 1), iOS (15, 2), MacCatalyst (15, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXBrailleMap : NSCopying, NSSecureCoding {

		[Export ("dimensions")]
		CGSize Dimensions { get; }

		[Export ("setHeight:atPoint:")]
		void SetHeight (float status, CGPoint point);

		[Export ("heightAtPoint:")]
		float GetHeight (CGPoint point);

		[Export ("presentImage:")]
		void Present (CGImage image);
	}

	[TV (15, 2), Mac (12, 1), iOS (15, 2), MacCatalyst (15, 2)]
	[Protocol]
	interface AXBrailleMapRenderer {

#if !NET
		[Abstract]
#endif
		[Export ("accessibilityBrailleMapRenderRegion", ArgumentSemantic.Assign)]
		CGRect AccessibilityBrailleMapRenderRegion { get; set; }

#if !NET
		[Abstract]
#endif
		[Export ("accessibilityBrailleMapRenderer", ArgumentSemantic.Copy)]
		Action<AXBrailleMap> AccessibilityBrailleMapRenderer { get; set; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	[Partial]
	partial interface AXAnimatedImagesUtilities {
		[Notification]
		[Field ("AXAnimatedImagesEnabledDidChangeNotification")]
		NSString AnimatedImagesEnabledDidChangeNotification { get; }
	}

	[TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	[Partial]
	partial interface AXPrefers {
		[Notification]
		[Field ("AXPrefersHorizontalTextLayoutDidChangeNotification")]
		NSString HorizontalTextLayoutDidChangeNotification { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Notification]
		[Field ("AXPrefersNonBlinkingTextInsertionIndicatorDidChangeNotification")]
		NSString NonBlinkingTextInsertionIndicatorDidChangeNotification { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXRequest : NSCopying, NSSecureCoding {
		[Static]
		[Export ("currentRequest"), NullAllowed]
		AXRequest Current { get; }

		[Export ("technology")]
		[BindAs (typeof (AXTechnology))]
		NSString Technology { get; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	enum AXTechnology {
		[Field ("AXTechnologyVoiceOver")]
		VoiceOver,

		[Field ("AXTechnologySwitchControl")]
		SwitchControl,

		[Field ("AXTechnologyVoiceControl")]
		VoiceControl,

		[Field ("AXTechnologyFullKeyboardAccess")]
		FullKeyboardAccess,

		[Field ("AXTechnologySpeakScreen")]
		SpeakScreen,

		[Field ("AXTechnologyAutomation")]
		Automation,

		[Field ("AXTechnologyHoverText")]
		HoverText,

		[Field ("AXTechnologyZoom")]
		Zoom,
	}

	[iOS (18, 2), NoTV, NoMacCatalyst, NoMac]
	[Native]
	public enum AXFeatureOverrideSessionError : long {
		Undefined = 0,
		AppNotEntitled,
		OverrideIsAlreadyActive,
		OverrideNotFoundForUUID,
	}

	[iOS (18, 2), NoTV, NoMacCatalyst, NoMac]
	[Flags]
	[Native]
	public enum AXFeatureOverrideSessionOptions : ulong {
		Grayscale = 1uL << 0,
		InvertColors = 1uL << 1,
		VoiceControl = 1uL << 2,
		VoiceOver = 1uL << 3,
		Zoom = 1uL << 4,
	}

	[iOS (18, 2), NoTV, NoMacCatalyst, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXFeatureOverrideSession {
	}

	[iOS (18, 2), NoTV, NoMacCatalyst, NoMac]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXFeatureOverrideSessionManager {
		[Static]
		[Export ("sharedInstance")]
		AXFeatureOverrideSessionManager SharedInstance { get; }

		[Export ("beginOverrideSessionEnablingOptions:disablingOptions:error:")]
		[return: NullAllowed]
		AXFeatureOverrideSession BeginOverrideSession (AXFeatureOverrideSessionOptions enableOptions, AXFeatureOverrideSessionOptions disableOptions, [NullAllowed] out NSError error);

		[Export ("endOverrideSession:error:")]
		bool EndOverrideSession (AXFeatureOverrideSession session, [NullAllowed] out NSError error);
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface AXMathExpression {
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionNumber {
		[Export ("initWithContent:")]
		NativeHandle Constructor (string content);

		[Export ("content")]
		string Content { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionIdentifier {
		[Export ("initWithContent:")]
		NativeHandle Constructor (string content);

		[Export ("content")]
		string Content { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionOperator {
		[Export ("initWithContent:")]
		NativeHandle Constructor (string content);

		[Export ("content")]
		string Content { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionText {
		[Export ("initWithContent:")]
		NativeHandle Constructor (string content);

		[Export ("content")]
		string Content { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionFenced {
		[Export ("initWithExpressions:openString:closeString:")]
		NativeHandle Constructor (AXMathExpression [] expressions, string openString, string closeString);

		[Export ("expressions")]
		AXMathExpression [] Expressions { get; }

		[Export ("openString")]
		string OpenString { get; }

		[Export ("closeString")]
		string CloseString { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionRow {
		[Export ("initWithExpressions:")]
		NativeHandle Constructor (AXMathExpression [] expressions);

		[Export ("expressions")]
		AXMathExpression [] Expressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionTable {
		[Export ("initWithExpressions:")]
		NativeHandle Constructor (AXMathExpression [] expressions);

		[Export ("expressions")]
		AXMathExpression [] Expressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionTableRow {
		[Export ("initWithExpressions:")]
		NativeHandle Constructor (AXMathExpression [] expressions);

		[Export ("expressions")]
		AXMathExpression [] Expressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionTableCell {
		[Export ("initWithExpressions:")]
		NativeHandle Constructor (AXMathExpression [] expressions);

		[Export ("expressions")]
		AXMathExpression [] Expressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionUnderOver {
		[Export ("initWithBaseExpression:underExpression:overExpression:")]
		NativeHandle Constructor (AXMathExpression baseExpression, AXMathExpression underExpression, AXMathExpression overExpression);

		[Export ("baseExpression")]
		AXMathExpression BaseExpression { get; }

		[Export ("underExpression")]
		AXMathExpression UnderExpression { get; }

		[Export ("overExpression")]
		AXMathExpression OverExpression { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionSubSuperscript {
		[Export ("initWithBaseExpression:subscriptExpressions:superscriptExpressions:")]
		NativeHandle Constructor (AXMathExpression [] baseExpression, AXMathExpression [] subscriptExpressions, AXMathExpression [] superscriptExpressions);

		[Export ("baseExpression")]
		AXMathExpression BaseExpression { get; }

		[Export ("subscriptExpressions")]
		AXMathExpression [] SubscriptExpressions { get; }

		[Export ("superscriptExpressions")]
		AXMathExpression [] SuperscriptExpressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionFraction {
		[Export ("initWithNumeratorExpression:denimonatorExpression:")]
		NativeHandle Constructor (AXMathExpression numeratorExpression, AXMathExpression denimonatorExpression);

		[Export ("numeratorExpression")]
		AXMathExpression NumeratorExpression { get; }

		[Export ("denimonatorExpression")]
		AXMathExpression DenimonatorExpression { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionMultiscript {
		[Export ("initWithBaseExpression:prescriptExpressions:postscriptExpressions:")]
		NativeHandle Constructor (AXMathExpression baseExpression, AXMathExpressionSubSuperscript [] prescriptExpressions, AXMathExpressionSubSuperscript [] postscriptExpressions);

		[Export ("baseExpression")]
		AXMathExpression BaseExpression { get; }

		[Export ("prescriptExpressions")]
		AXMathExpressionSubSuperscript [] PrescriptExpressions { get; }

		[Export ("postscriptExpressions")]
		AXMathExpressionSubSuperscript [] PostscriptExpressions { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[BaseType (typeof (AXMathExpression))]
	[DisableDefaultCtor]
	interface AXMathExpressionRoot {
		[Export ("initWithRadicandExpressions:rootIndexExpression:")]
		NativeHandle Constructor (AXMathExpression [] radicandExpressions, AXMathExpression rootIndexExpression);

		[Export ("radicandExpressions")]
		AXMathExpression [] RadicandExpressions { get; }

		[Export ("rootIndexExpression")]
		AXMathExpression RootIndexExpression { get; }
	}

	[TV (18, 2), Mac (15, 2), iOS (18, 2), MacCatalyst (18, 2)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface AXMathExpressionProvider {
		[Abstract]
		[NullAllowed, Export ("accessibilityMathExpression")]
		AXMathExpression AccessibilityMathExpression { get; }
	}
}
