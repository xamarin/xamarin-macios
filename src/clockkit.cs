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
using Intents;
using ObjCRuntime;
using UIKit;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ClockKit {

	[ErrorDomain ("CLKWatchFaceLibraryErrorDomain")]
	[Native]
	public enum CLKWatchFaceLibraryErrorCode : long {
		NotFileUrl = 1,
		InvalidFile = 2,
		PermissionDenied = 3,
		FaceNotAvailable = 4,
	}

	[BaseType (typeof (NSObject))]
	interface CLKComplication : NSCopying {

		[Export ("family")]
		CLKComplicationFamily Family { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("userInfo"), NullAllowed]
		NSDictionary UserInfo { get; }

		[Export ("userActivity"), NullAllowed]
		NSUserActivity UserActivity { get; }

		[Field ("CLKDefaultComplicationIdentifier")]
		NSString DefaultComplicationIdentifier { get; }
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

		[Export ("getAlwaysOnTemplateForComplication:withHandler:")]
		void GetAlwaysOnTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Abstract]
		[Export ("getCurrentTimelineEntryForComplication:withHandler:")]
		void GetCurrentTimelineEntry (CLKComplication complication, Action<CLKComplicationTimelineEntry> handler);

		[Export ("getTimelineEntriesForComplication:beforeDate:limit:withHandler:")]
		void GetTimelineEntriesBeforeDate (CLKComplication complication, NSDate beforeDate, nuint limit, Action<CLKComplicationTimelineEntry []> handler);

		[Export ("getTimelineEntriesForComplication:afterDate:limit:withHandler:")]
		void GetTimelineEntriesAfterDate (CLKComplication complication, NSDate afterDate, nuint limit, Action<CLKComplicationTimelineEntry []> handler);

		[Export ("getNextRequestedUpdateDateWithHandler:")]
		void GetNextRequestedUpdateDate (Action<NSDate> handler);

		[Export ("requestedUpdateDidBegin")]
		void RequestedUpdateDidBegin ();

		[Export ("requestedUpdateBudgetExhausted")]
		void RequestedUpdateBudgetExhausted ();

		// this was @required in watchOS 2.x but is now deprecated and downgraded to @optional in watchOS 3 (betas)
		[Export ("getPlaceholderTemplateForComplication:withHandler:")]
		void GetPlaceholderTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Export ("getLocalizableSampleTemplateForComplication:withHandler:")]
		void GetLocalizableSampleTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Export ("getComplicationDescriptorsWithHandler:")]
		void GetComplicationDescriptors (Action<CLKComplicationDescriptor []> handler);

		[Export ("handleSharedComplicationDescriptors:")]
		void HandleSharedComplicationDescriptors (CLKComplicationDescriptor [] complicationDescriptors);

		[NoiOS]
		[Export ("widgetMigrator")]
		CLKComplicationWidgetMigrator WidgetMigrator { get; }
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

		[Export ("reloadComplicationDescriptors")]
		void ReloadComplicationDescriptors ();
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKComplicationTemplate : NSCopying {

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Copy)]
		UIColor TintColor { get; set; }

		[Export ("init")]
		NativeHandle Constructor ();
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateModularSmallSimpleText Create (CLKTextProvider textProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateModularSmallSimpleImage Create (CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateModularSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateModularSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateModularSmallStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateModularSmallStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
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

		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularSmallColumnsText Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
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

		[Export ("initWithHeaderTextProvider:body1TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Export ("initWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeTallBody {

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("bodyTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BodyTextProvider { get; set; }

		[Export ("initWithHeaderTextProvider:bodyTextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider bodyTextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:bodyTextProvider:")]
		CLKComplicationTemplateModularLargeTallBody Create (CLKTextProvider headerTextProvider, CLKTextProvider bodyTextProvider);
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

		[Export ("initWithHeaderTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularLargeTable Create (CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularLargeTable Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
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

		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Export ("initWithRow1ImageProvider:row1Column1TextProvider:row1Column2TextProvider:row2ImageProvider:row2Column1TextProvider:row2Column2TextProvider:row3ImageProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider row1ImageProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, [NullAllowed] CLKImageProvider row2ImageProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, [NullAllowed] CLKImageProvider row3ImageProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		CLKComplicationTemplateModularLargeColumns Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Static]
		[Export ("templateWithRow1ImageProvider:row1Column1TextProvider:row1Column2TextProvider:row2ImageProvider:row2Column1TextProvider:row2Column2TextProvider:row3ImageProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		CLKComplicationTemplateModularLargeColumns Create ([NullAllowed] CLKImageProvider row1ImageProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, [NullAllowed] CLKImageProvider row2ImageProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, [NullAllowed] CLKImageProvider row3ImageProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateUtilitarianSmallFlat Create (CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateUtilitarianSmallFlat Create (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallSquare {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateUtilitarianSmallSquare Create (CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateUtilitarianSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateUtilitarianSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianLargeFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateUtilitarianLargeFlat Create (CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateUtilitarianLargeFlat Create (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateCircularSmallSimpleText Create (CLKTextProvider textProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateCircularSmallSimpleImage Create (CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateCircularSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateCircularSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateCircularSmallStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateCircularSmallStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateExtraLargeSimpleText Create (CLKTextProvider textProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateExtraLargeSimpleImage Create (CLKImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateExtraLargeRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateExtraLargeRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateExtraLargeStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateExtraLargeStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

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

		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateExtraLargeColumnsText Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
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
	[DisableDefaultCtor]
	interface CLKImageProvider : NSCopying {

		[Export ("init")]
		NativeHandle Constructor ();

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

		[Export ("initWithOnePieceImage:")]
		NativeHandle Constructor (UIImage onePieceImage);

		[Export ("initWithOnePieceImage:twoPieceImageBackground:twoPieceImageForeground:")]
		NativeHandle Constructor (UIImage onePieceImage, [NullAllowed] UIImage twoPieceImageBackground, [NullAllowed] UIImage twoPieceImageForeground);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKTextProvider : NSCopying {

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use overloaded constructors.")]
		[Export ("init")]
		NativeHandle Constructor ();

		// FIXME: expose gracefully
		[Static, Internal]
		[Export ("textProviderWithFormat:", IsVariadic = true)]
		CLKTextProvider Create (string format, IntPtr varArgs);

		[Export ("tintColor", ArgumentSemantic.Retain)]
		UIColor TintColor { get; set; }

		// Localizable (CLKTextProvider)
		// but static methods are not great candidates for extensions methods
		// so they are inlined inside the actual type

		[Static]
		[Export ("localizableTextProviderWithStringsFileTextKey:")]
		CLKTextProvider CreateLocalizable (string textKey);

		[Static]
		[Export ("localizableTextProviderWithStringsFileTextKey:shortTextKey:")]
		CLKTextProvider CreateLocalizable (string textKey, [NullAllowed] string shortTextKey);

		[Static]
		[Export ("localizableTextProviderWithStringsFileFormatKey:textProviders:")]
		CLKTextProvider CreateLocalizable (string formatKey, CLKTextProvider [] textProviders);

		[NullAllowed]
		[Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }
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

		[Export ("initWithText:")]
		NativeHandle Constructor (string text);

		[Export ("initWithText:shortText:")]
		NativeHandle Constructor (string text, [NullAllowed] string shortText);

		[Export ("initWithText:shortText:accessibilityLabel:")]
		NativeHandle Constructor (string text, [NullAllowed] string shortText, [NullAllowed] string accessibilityLabel);
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

		[Export ("uppercase")]
		bool Uppercase { get; set; }

		[Export ("initWithDate:units:")]
		NativeHandle Constructor (NSDate date, NSCalendarUnit calendarUnits);

		[Export ("initWithDate:units:timeZone:")]
		NativeHandle Constructor (NSDate date, NSCalendarUnit calendarUnits, [NullAllowed] NSTimeZone timeZone);
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

		[Export ("initWithDate:")]
		NativeHandle Constructor (NSDate date);

		[Export ("initWithDate:timeZone:")]
		NativeHandle Constructor (NSDate date, [NullAllowed] NSTimeZone timeZone);
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

		[Export ("initWithStartDate:endDate:")]
		NativeHandle Constructor (NSDate startDate, NSDate endDate);

		[Export ("initWithStartDate:endDate:timeZone:")]
		NativeHandle Constructor (NSDate startDate, NSDate endDate, [NullAllowed] NSTimeZone timeZone);
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

		[Export ("initWithDate:style:units:")]
		NativeHandle Constructor (NSDate date, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Export ("initWithDate:relativeToDate:style:units:")]
		NativeHandle Constructor (NSDate date, [NullAllowed] NSDate relativeDate, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Static]
		[Export ("textProviderWithDate:relativeToDate:style:units:")]
		CLKRelativeDateTextProvider Create (NSDate date, [NullAllowed] NSDate relativeToDate, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Export ("relativeToDate", ArgumentSemantic.Retain), NullAllowed]
		NSDate RelativeToDate { get; set; }
	}

	[Static]
	interface CLKLaunchOptionsKeys {

		[Field ("CLKLaunchedTimelineEntryDateKey")]
		NSString LaunchedTimelineEntryDate { get; }

		[Field ("CLKLaunchedComplicationIdentifierKey")]
		NSString LaunchedComplicationIdentifierKey { get; }
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicBezelCircularText {
		[Export ("circularTemplate", ArgumentSemantic.Copy)]
		CLKComplicationTemplateGraphicCircular CircularTemplate { get; set; }

		[NullAllowed, Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("initWithCircularTemplate:")]
		NativeHandle Constructor (CLKComplicationTemplateGraphicCircular circularTemplate);

		[Export ("initWithCircularTemplate:textProvider:")]
		NativeHandle Constructor (CLKComplicationTemplateGraphicCircular circularTemplate, [NullAllowed] CLKTextProvider textProvider);

		[Static]
		[Export ("templateWithCircularTemplate:")]
		CLKComplicationTemplateGraphicBezelCircularText Create (CLKComplicationTemplateGraphicCircular circularTemplate);

		[Static]
		[Export ("templateWithCircularTemplate:textProvider:")]
		CLKComplicationTemplateGraphicBezelCircularText Create (CLKComplicationTemplateGraphicCircular circularTemplate, [NullAllowed] CLKTextProvider textProvider);
	}

	[Abstract] // <quote>An abstract superclass for all the circular graphic templates.</quote>
	[DisableDefaultCtor]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCircular {
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularClosedGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithGaugeProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithGaugeProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCircularClosedGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularClosedGaugeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularClosedGaugeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicCircularImage Create (CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularOpenGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider BottomImageProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularOpenGaugeRangeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("leadingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider LeadingTextProvider { get; set; }

		[Export ("trailingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TrailingTextProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeRangeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BottomTextProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerCircularImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicCornerCircularImage Create (CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[NullAllowed, Export ("leadingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider LeadingTextProvider { get; set; }

		[NullAllowed, Export ("trailingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TrailingTextProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithGaugeProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithGaugeProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeImage Create (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerGaugeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[NullAllowed, Export ("leadingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider LeadingTextProvider { get; set; }

		[NullAllowed, Export ("trailingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TrailingTextProvider { get; set; }

		[Export ("outerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider OuterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider outerTextProvider);

		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKTextProvider outerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider outerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeText Create (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKTextProvider outerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerStackText {
		[Export ("innerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider InnerTextProvider { get; set; }

		[Export ("outerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider OuterTextProvider { get; set; }

		[Export ("initWithInnerTextProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKTextProvider innerTextProvider, CLKTextProvider outerTextProvider);

		[Static]
		[Export ("templateWithInnerTextProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerStackText Create (CLKTextProvider innerTextProvider, CLKTextProvider outerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerTextImage {
		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerTextImage Create (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicRectangularLargeImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicRectangularLargeImage Create (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicRectangularStandardBody {
		[NullAllowed, Export ("headerImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider HeaderImageProvider { get; set; }

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("body1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Body1TextProvider { get; set; }

		[NullAllowed, Export ("body2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Body2TextProvider { get; set; }

		[Export ("initWithHeaderTextProvider:body1TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Export ("initWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicRectangularTextGauge {
		[NullAllowed, Export ("headerImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider HeaderImageProvider { get; set; }

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("body1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Body1TextProvider { get; set; }

		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("initWithHeaderTextProvider:body1TextProvider:gaugeProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:gaugeProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:gaugeProvider:")]
		CLKComplicationTemplateGraphicRectangularTextGauge Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:gaugeProvider:")]
		CLKComplicationTemplateGraphicRectangularTextGauge Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKFullColorImageProvider : NSCopying {
		[Static]
		[Export ("providerWithFullColorImage:")]
		CLKFullColorImageProvider Create (UIImage image);

		[Static]
		[Export ("providerWithFullColorImage:tintedImageProvider:")]
		CLKFullColorImageProvider Create (UIImage image, [NullAllowed] CLKImageProvider tintedImageProvider);

		[Export ("image", ArgumentSemantic.Retain)]
		UIImage Image { get; set; }

		[NullAllowed, Export ("tintedImageProvider", ArgumentSemantic.Retain)]
		CLKImageProvider TintedImageProvider { get; set; }

		[NullAllowed, Export ("accessibilityLabel", ArgumentSemantic.Retain)]
		string AccessibilityLabel { get; set; }

		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("initWithFullColorImage:")]
		NativeHandle Constructor (UIImage fullColorImage);

		[Export ("initWithFullColorImage:tintedImageProvider:")]
		NativeHandle Constructor (UIImage fullColorImage, [NullAllowed] CLKImageProvider tintedImageProvider);
	}

	[BaseType (typeof (NSObject))]
	[Abstract] // <quote>An abstract superclass that...</quote>
	[DisableDefaultCtor]
	interface CLKGaugeProvider : NSCopying {
		[Export ("style", ArgumentSemantic.Assign)]
		CLKGaugeProviderStyle Style { get; }

		[NullAllowed, Export ("gaugeColors")]
		UIColor [] GaugeColors { get; }

		[NullAllowed, Export ("gaugeColorLocations")]
		[BindAs (typeof (float []))] // between 0.0 and 1.0
		NSNumber [] GaugeColorLocations { get; }

		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }
	}

	[BaseType (typeof (CLKGaugeProvider))]
	[DisableDefaultCtor]
	interface CLKSimpleGaugeProvider {
		[Static]
		[Export ("gaugeProviderWithStyle:gaugeColors:gaugeColorLocations:fillFraction:")]
		CLKSimpleGaugeProvider Create (CLKGaugeProviderStyle style, [NullAllowed] UIColor [] gaugeColors, [NullAllowed][BindAs (typeof (float []))] NSNumber [] gaugeColorLocations, float fillFraction);

		[Static]
		[Export ("gaugeProviderWithStyle:gaugeColor:fillFraction:")]
		CLKSimpleGaugeProvider Create (CLKGaugeProviderStyle style, UIColor color, float fillFraction);

		[Export ("fillFraction")]
		float FillFraction { get; }

		[Field ("CLKSimpleGaugeProviderFillFractionEmpty")]
		float FillFractionEmpty { get; }
	}

	[BaseType (typeof (CLKGaugeProvider))]
	[DisableDefaultCtor]
	interface CLKTimeIntervalGaugeProvider {
		[Static]
		[Export ("gaugeProviderWithStyle:gaugeColors:gaugeColorLocations:startDate:endDate:")]
		CLKTimeIntervalGaugeProvider Create (CLKGaugeProviderStyle style, [NullAllowed] UIColor [] gaugeColors, [NullAllowed][BindAs (typeof (float []))] NSNumber [] gaugeColorLocations, NSDate startDate, NSDate endDate);

		[Static]
		[Export ("gaugeProviderWithStyle:gaugeColors:gaugeColorLocations:startDate:startFillFraction:endDate:endFillFraction:")]
		CLKTimeIntervalGaugeProvider Create (CLKGaugeProviderStyle style, [NullAllowed] UIColor [] gaugeColors, [NullAllowed][BindAs (typeof (float []))] NSNumber [] gaugeColorLocations, NSDate startDate, float startFillFraction, NSDate endDate, float endFillFraction);

		[Export ("startDate")]
		NSDate StartDate { get; }

		[Export ("endDate")]
		NSDate EndDate { get; }

		[Export ("startFillFraction")]
		float StartFillFraction { get; }

		[Export ("endFillFraction")]
		float EndFillFraction { get; }
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicCircularStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicCircularStackImage Create (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	interface CLKWatchFaceLibrary {
		[Async]
		[Export ("addWatchFaceAtURL:completionHandler:")]
		void AddWatchFace (NSUrl fileUrl, Action<NSError> handler);
	}

	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicRectangularFullImage : NSSecureCoding {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicRectangularFullImage Create (CLKFullColorImageProvider imageProvider);
	}

	[Advice ("This class is an abstract super class in the runtime. Do use one of its children.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicExtraLargeCircular : NSSecureCoding {
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularStackText {
		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularStackImage {
		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularStackImage Create (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeSimpleText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BottomTextProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeSimpleText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeRangeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("leadingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider LeadingTextProvider { get; set; }

		[Export ("trailingTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TrailingTextProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeRangeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider BottomImageProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularOpenGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularImage Create (CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularClosedGaugeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Export ("initWithGaugeProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);

		[Static]
		[Export ("templateWithGaugeProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularClosedGaugeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);
	}

	[BaseType (typeof (CLKComplicationTemplateGraphicExtraLargeCircular))]
	interface CLKComplicationTemplateGraphicExtraLargeCircularClosedGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("initWithGaugeProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Static]
		[Export ("templateWithGaugeProvider:imageProvider:")]
		CLKComplicationTemplateGraphicExtraLargeCircularClosedGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKComplicationDescriptor : NSCopying, NSSecureCoding {
		[Export ("identifier")]
		string Identifier { get; }

		[Export ("displayName")]
		string DisplayName { get; }

		[Export ("supportedFamilies")]
		[BindAs (typeof (CLKComplicationFamily []))]
		NSNumber [] SupportedFamilies { get; }

		[NullAllowed, Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[NullAllowed, Export ("userActivity")]
		NSUserActivity UserActivity { get; }

		[Export ("initWithIdentifier:displayName:supportedFamilies:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies);

		[Export ("initWithIdentifier:displayName:supportedFamilies:userInfo:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies, NSDictionary userInfo);

		[Export ("initWithIdentifier:displayName:supportedFamilies:userActivity:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies, NSUserActivity userActivity);
	}

	[NoiOS]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationWidgetMigrationConfiguration : NSCopying { }

	[NoiOS]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationWidgetMigrator {
		[Async]
		[Export ("getWidgetConfigurationFrom:completionHandler:")]
		void GetWidgetConfiguration (CLKComplicationDescriptor complicationDescriptor, Action<CLKComplicationWidgetMigrationConfiguration> completionHandler);
	}

	[NoiOS]
	[BaseType (typeof (CLKComplicationWidgetMigrationConfiguration))]
	[DisableDefaultCtor]
	interface CLKComplicationIntentWidgetMigrationConfiguration {
		[Export ("kind")]
		string Kind { get; }

		[Export ("extensionBundleIdentifier")]
		string ExtensionBundleIdentifier { get; }

		[Export ("intent", ArgumentSemantic.Copy)]
		INIntent Intent { get; }

		[Export ("localizedDisplayName")]
		string LocalizedDisplayName { get; }

		[Export ("initWithKind:extensionBundleIdentifier:intent:localizedDisplayName:")]
		NativeHandle Constructor (string kind, string extensionBundleIdentifier, INIntent intent, string localizedDisplayName);

		[Static]
		[Export ("intentWidgetMigrationConfigurationWithKind:extensionBundleIdentifier:intent:localizedDisplayName:")]
		CLKComplicationIntentWidgetMigrationConfiguration Create (string kind, string extensionBundleIdentifier, INIntent intent, string localizedDisplayName);
	}


	[NoiOS]
	[BaseType (typeof (CLKComplicationWidgetMigrationConfiguration))]
	[DisableDefaultCtor]
	interface CLKComplicationStaticWidgetMigrationConfiguration {
		[Export ("kind")]
		string Kind { get; }

		[Export ("extensionBundleIdentifier")]
		string ExtensionBundleIdentifier { get; }

		[Export ("initWithKind:extensionBundleIdentifier:")]
		NativeHandle Constructor (string kind, string extensionBundleIdentifier);

		[Static]
		[Export ("staticWidgetMigrationConfigurationWithKind:extensionBundleIdentifier:")]
		CLKComplicationStaticWidgetMigrationConfiguration Create (string kind, string extensionBundleIdentifier);
	}


}
