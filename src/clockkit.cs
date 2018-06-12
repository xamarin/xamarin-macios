//
// ClockKit bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace ClockKit {
	
	[BaseType (typeof (NSObject))]
	interface CLKComplication : NSCopying {

		[Export ("family")]
		CLKComplicationFamily Family { get; }
	}

	interface ICLKComplicationDataSource { }

	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationDataSource {

		[Abstract]
		[Export ("getSupportedTimeTravelDirectionsForComplication:withHandler:")]
		void GetSupportedTimeTravelDirections (CLKComplication complication, Action<CLKComplicationTimeTravelDirections> handler);

		[Export ("getTimelineStartDateForComplication:withHandler:")]
		void GetTimelineStartDate (CLKComplication complication, Action<NSDate> handler);

		[Export ("getTimelineEndDateForComplication:withHandler:")]
		void GetTimelineEndDate (CLKComplication complication, Action<NSDate> handler);

		[Export ("getPrivacyBehaviorForComplication:withHandler:")]
		void GetPrivacyBehavior (CLKComplication complication, Action<CLKComplicationPrivacyBehavior> handler);

		[Export ("getTimelineAnimationBehaviorForComplication:withHandler:")]
		void GetTimelineAnimationBehavior (CLKComplication complication, Action<CLKComplicationTimelineAnimationBehavior> handler);

		[Abstract]
		[Export ("getCurrentTimelineEntryForComplication:withHandler:")]
		void GetCurrentTimelineEntry (CLKComplication complication, Action<CLKComplicationTimelineEntry> handler);

		[Export ("getTimelineEntriesForComplication:beforeDate:limit:withHandler:")]
		void GetTimelineEntriesBeforeDate (CLKComplication complication, NSDate beforeDate, nuint limit, Action<CLKComplicationTimelineEntry []> handler);

		[Export ("getTimelineEntriesForComplication:afterDate:limit:withHandler:")]
		void GetTimelineEntriesAfterDate (CLKComplication complication, NSDate afterDate, nuint limit, Action<CLKComplicationTimelineEntry []> handler);
	
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'WKRefreshBackgroundTask' instead.")]
		[Export ("getNextRequestedUpdateDateWithHandler:")]
		void GetNextRequestedUpdateDate (Action<NSDate> handler);

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'WKRefreshBackgroundTask' instead.")]
		[Export ("requestedUpdateDidBegin")]
		void RequestedUpdateDidBegin ();

		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'WKRefreshBackgroundTask' instead.")]
		[Export ("requestedUpdateBudgetExhausted")]
		void RequestedUpdateBudgetExhausted ();

		// this was @required in watchOS 2.x but is now deprecated and downgraded to @optional in watchOS 3 (betas)
		[Deprecated (PlatformName.WatchOS, 3, 0, message: "Use 'GetLocalizableSampleTemplate' instead.")]
		[Export ("getPlaceholderTemplateForComplication:withHandler:")]
		void GetPlaceholderTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Watch (3,0)]
		[Export ("getLocalizableSampleTemplateForComplication:withHandler:")]
		void GetLocalizableSampleTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // Default constructor not allowed for ClockKit.CLKComplicationServer : Objective-C exception thrown.  Name: NSInternalInconsistencyException Reason: You cannot alloc/init new instances of CLKComplicationServer. Use +sharedInstance.
	interface CLKComplicationServer {

		[Field ("CLKComplicationServerActiveComplicationsDidChangeNotification")]
		[Notification]
		NSString ActiveComplicationsDidChangeNotification { get; }

		[Static]
		[Export ("sharedInstance")]
		CLKComplicationServer SharedInstance { get; }

		[Export ("activeComplications"), NullAllowed]
		CLKComplication [] ActiveComplications { get; }

		[Export ("earliestTimeTravelDate")]
		NSDate EarliestTimeTravelDate { get; }

		[Export ("latestTimeTravelDate")]
		NSDate LatestTimeTravelDate { get; }

		[Export ("reloadTimelineForComplication:")]
		void ReloadTimeline (CLKComplication complication);

		[Export ("extendTimelineForComplication:")]
		void ExtendTimeline (CLKComplication complication);
	}

	[BaseType (typeof (NSObject))]
	interface CLKComplicationTemplate : NSCopying {

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Copy)]
		UIColor TintColor { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallColumnsText {

		[Export ("row1Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column1TextProvider { get; set; }

		[Export ("row1Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column2TextProvider { get; set; }

		[Export ("row2Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column1TextProvider { get; set; }

		[Export ("row2Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column2TextProvider { get; set; }

		[Export ("column2Alignment")]
		CLKComplicationColumnAlignment Column2Alignment { get; set; }

		[Export ("highlightColumn2")]
		bool HighlightColumn2 { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeStandardBody {

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("body1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Body1TextProvider { get; set; }

		[NullAllowed]
		[Export ("body2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Body2TextProvider { get; set; }

		[NullAllowed]
		[Export ("headerImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider HeaderImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeTallBody {

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("bodyTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BodyTextProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeTable {

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("row1Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column1TextProvider { get; set; }

		[Export ("row1Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column2TextProvider { get; set; }

		[Export ("row2Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column1TextProvider { get; set; }

		[Export ("row2Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column2TextProvider { get; set; }

		[NullAllowed]
		[Export ("headerImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider HeaderImageProvider { get; set; }

		[Export ("column2Alignment")]
		CLKComplicationColumnAlignment Column2Alignment { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeColumns {

		[Export ("row1Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column1TextProvider { get; set; }

		[Export ("row1Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column2TextProvider { get; set; }

		[Export ("row2Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column1TextProvider { get; set; }

		[Export ("row2Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column2TextProvider { get; set; }

		[Export ("row3Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row3Column1TextProvider { get; set; }

		[Export ("row3Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row3Column2TextProvider { get; set; }

		[NullAllowed]
		[Export ("row1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Row1ImageProvider { get; set; }

		[NullAllowed]
		[Export ("row2ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Row2ImageProvider { get; set; }

		[NullAllowed]
		[Export ("row3ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Row3ImageProvider { get; set; }

		[Export ("column2Alignment")]
		CLKComplicationColumnAlignment Column2Alignment { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallSquare {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianLargeFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleText {
		
		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleImage {
		
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingText {
		
		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingImage {
		
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackText {
		
		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackImage {
		
		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }
	}

	[Watch (3,0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeColumnsText {
		
		[Export ("row1Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column1TextProvider { get; set; }

		[Export ("row1Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row1Column2TextProvider { get; set; }

		[Export ("row2Column1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column1TextProvider { get; set; }

		[Export ("row2Column2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Row2Column2TextProvider { get; set; }

		[Export ("column2Alignment", ArgumentSemantic.Assign)]
		CLKComplicationColumnAlignment Column2Alignment { get; set; }

		[Export ("highlightColumn2")]
		bool HighlightColumn2 { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface CLKComplicationTimelineEntry {

		[Static]
		[Export ("entryWithDate:complicationTemplate:")]
		CLKComplicationTimelineEntry Create (NSDate date, CLKComplicationTemplate complicationTemplate);

		[Static]
		[Export ("entryWithDate:complicationTemplate:timelineAnimationGroup:")]
		CLKComplicationTimelineEntry Create (NSDate date, CLKComplicationTemplate complicationTemplate, [NullAllowed] string timelineAnimationGroup);

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }

		[Export ("complicationTemplate", ArgumentSemantic.Copy)]
		CLKComplicationTemplate ComplicationTemplate { get; set; }

		[NullAllowed, Export ("timelineAnimationGroup")]
		string TimelineAnimationGroup { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface CLKImageProvider : NSCopying {

		[Static]
		[Export ("imageProviderWithOnePieceImage:")]
		CLKImageProvider Create (UIImage onePieceImage);

		[Static]
		[Export ("imageProviderWithOnePieceImage:twoPieceImageBackground:twoPieceImageForeground:")]
		CLKImageProvider Create (UIImage onePieceImage, [NullAllowed] UIImage twoPieceImageBackground, [NullAllowed] UIImage twoPieceImageForeground);

		[NullAllowed]
		[Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Retain)]
		UIColor TintColor { get; set; }

		[Export ("onePieceImage", ArgumentSemantic.Retain)]
		UIImage OnePieceImage { get; set; }

		[NullAllowed, Export ("twoPieceImageBackground", ArgumentSemantic.Retain)]
		UIImage TwoPieceImageBackground { get; set; }

		[NullAllowed, Export ("twoPieceImageForeground", ArgumentSemantic.Retain)]
		UIImage TwoPieceImageForeground { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface CLKTextProvider : NSCopying {

		// FIXME: expose gracefully
		[Static, Internal]
		[Export ("textProviderWithFormat:", IsVariadic = true)]
		CLKTextProvider Create (string format, IntPtr varArgs);

		[Export ("tintColor", ArgumentSemantic.Retain)]
		UIColor TintColor { get; set; }

		// Localizable (CLKTextProvider)
		// but static methods are not great candidates for extensions methods
		// so they are inlined inside the actual type

		[Watch (3,0)]
		[Static]
		[Export ("localizableTextProviderWithStringsFileTextKey:")]
		CLKTextProvider CreateLocalizable (string textKey);

		[Watch (3,0)]
		[Static]
		[Export ("localizableTextProviderWithStringsFileTextKey:shortTextKey:")]
		CLKTextProvider CreateLocalizable (string textKey, [NullAllowed] string shortTextKey);

		[Watch (3,0)]
		[Static]
		[Export ("localizableTextProviderWithStringsFileFormatKey:textProviders:")]
		CLKTextProvider CreateLocalizable (string formatKey, CLKTextProvider[] textProviders);
	}

	[BaseType (typeof (CLKTextProvider))]
	interface CLKSimpleTextProvider {

		[Static]
		[Export ("textProviderWithText:")]
		CLKSimpleTextProvider FromText (string text);

		[Static]
		[Export ("textProviderWithText:shortText:")]
		CLKSimpleTextProvider FromText (string text, [NullAllowed] string shortText);

		[Static]
		[Export ("textProviderWithText:shortText:accessibilityLabel:")]
		CLKSimpleTextProvider FromText (string text, [NullAllowed] string shortText, [NullAllowed] string accessibilityLabel);

		[Export ("text")]
		string Text { get; set; }

		[NullAllowed]
		[Export ("shortText")]
		string ShortText { get; set; }

		[NullAllowed]
		[Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }
	}

	[BaseType (typeof (CLKTextProvider))]
	interface CLKDateTextProvider {

		[Static]
		[Export ("textProviderWithDate:units:")]
		CLKDateTextProvider FromDate (NSDate date, NSCalendarUnit calendarUnits);

		[Static]
		[Export ("textProviderWithDate:units:timeZone:")]
		CLKDateTextProvider FromDate (NSDate date, NSCalendarUnit calendarUnits, [NullAllowed] NSTimeZone timeZone);

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }

		[Export ("calendarUnits")]
		NSCalendarUnit CalendarUnits { get; set; }

		[NullAllowed]
		[Export ("timeZone", ArgumentSemantic.Retain)]
		NSTimeZone TimeZone { get; set; }
	}

	[BaseType (typeof (CLKTextProvider))]
	interface CLKTimeTextProvider {

		[Static]
		[Export ("textProviderWithDate:")]
		CLKTimeTextProvider FromDate (NSDate date);

		[Static]
		[Export ("textProviderWithDate:timeZone:")]
		CLKTimeTextProvider FromDate (NSDate date, [NullAllowed] NSTimeZone timeZone);

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }

		[NullAllowed]
		[Export ("timeZone", ArgumentSemantic.Retain)]
		NSTimeZone TimeZone { get; set; }
	}

	[BaseType (typeof (CLKTextProvider))]
	interface CLKTimeIntervalTextProvider {

		[Static]
		[Export ("textProviderWithStartDate:endDate:")]
		CLKTimeIntervalTextProvider FromStartDate (NSDate startDate, NSDate endDate);

		[Static]
		[Export ("textProviderWithStartDate:endDate:timeZone:")]
		CLKTimeIntervalTextProvider FromStartDate (NSDate startDate, NSDate endDate, [NullAllowed] NSTimeZone timeZone);

		[Export ("startDate", ArgumentSemantic.Retain)]
		NSDate StartDate { get; set; }

		[Export ("endDate", ArgumentSemantic.Retain)]
		NSDate EndDate { get; set; }

		[NullAllowed]
		[Export ("timeZone", ArgumentSemantic.Retain)]
		NSTimeZone TimeZone { get; set; }
	}

	[BaseType (typeof (CLKTextProvider))]
	interface CLKRelativeDateTextProvider {

		[Static]
		[Export ("textProviderWithDate:style:units:")]
		CLKRelativeDateTextProvider FromDate (NSDate date, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Export ("date", ArgumentSemantic.Retain)]
		NSDate Date { get; set; }

		[Export ("relativeDateStyle")]
		CLKRelativeDateStyle RelativeDateStyle { get; set; }

		[Export ("calendarUnits")]
		NSCalendarUnit CalendarUnits { get; set; }
	}

	[Static]
	interface CLKLaunchOptionsKeys {

		[Field ("CLKLaunchedTimelineEntryDateKey")]
		NSString LaunchedTimelineEntryDate { get; }
	}
}

