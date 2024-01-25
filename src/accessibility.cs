using System;

using CoreGraphics;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace Accessibility {

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface AXChart {
		[Abstract]
		[NullAllowed, Export ("accessibilityChartDescriptor", ArgumentSemantic.Strong)]
		AXChartDescriptor AccessibilityChartDescriptor { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum AXChartDescriptorContentDirection : long {
		LeftToRight = 0,
		RightToLeft,
		TopToBottom,
		BottomToTop,
		RadialClockwise,
		RadialCounterClockwise,
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum AXCustomContentImportance : ulong {
		Default,
		High,
	}

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
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

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Protocol]
	interface AXCustomContentProvider {
		[Abstract]
		[NullAllowed, Export ("accessibilityCustomContent", ArgumentSemantic.Copy)]
		AXCustomContent [] AccessibilityCustomContent { get; set; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("accessibilityCustomContentBlock", ArgumentSemantic.Copy)]
		Func<AXCustomContent []?> AccessibilityCustomContentHandler { get; set; }

	}

	interface IAXDataAxisDescriptor { }

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Protocol]
	interface AXDataAxisDescriptor : NSCopying {
		[Abstract]
		[Export ("title")]
		string Title { get; set; }

		[Abstract]
		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum AXHearingDeviceEar : ulong {
		None = 0,
		Left = 1 << 1,
		Right = 1 << 2,
		Both = Left | Right,
	}

	[Watch (8, 0), NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum AXNumericDataAxisDescriptorScale : long {
		Linear = 0,
		Log10,
		Ln,
	}

	delegate NSString ValueDescriptionProviderHandler (double dataValue);

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 3), TV (15, 2), Mac (12, 1), iOS (15, 2), MacCatalyst (15, 2)]
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

	[Watch (8, 3), TV (15, 2), Mac (12, 1), iOS (15, 2), MacCatalyst (15, 2)]
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

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	[Partial]
	partial interface AXAnimatedImagesUtilities {
		[Notification]
		[Field ("AXAnimatedImagesEnabledDidChangeNotification")]
		NSString AnimatedImagesEnabledDidChangeNotification { get; }
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Static]
	[Partial]
	partial interface AXPrefers {
		[Notification]
		[Field ("AXPrefersHorizontalTextLayoutDidChangeNotification")]
		NSString HorizontalTextLayoutDidChangeNotification { get; }
	}

}
