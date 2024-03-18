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

	[Watch (7, 0)]
	[ErrorDomain ("CLKWatchFaceLibraryErrorDomain")]
	[Native]
	public enum CLKWatchFaceLibraryErrorCode : long {
		NotFileUrl = 1,
		InvalidFile = 2,
		PermissionDenied = 3,
		FaceNotAvailable = 4,
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (NSObject))]
	interface CLKComplication : NSCopying {

		[Export ("family")]
		CLKComplicationFamily Family { get; }

		[Watch (7, 0)]
		[Export ("identifier")]
		string Identifier { get; }

		[Watch (7, 0)]
		[Export ("userInfo"), NullAllowed]
		NSDictionary UserInfo { get; }

		[Watch (7, 0)]
		[Export ("userActivity"), NullAllowed]
		NSUserActivity UserActivity { get; }

		[Watch (7, 0)]
		[Field ("CLKDefaultComplicationIdentifier")]
		NSString DefaultComplicationIdentifier { get; }
	}

	interface ICLKComplicationDataSource { }

	[Model, Protocol]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationDataSource {

		[Abstract]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'CLKComplicationDataSource.GetTimelineEndDate' instead.")]
		[Export ("getSupportedTimeTravelDirectionsForComplication:withHandler:")]
		void GetSupportedTimeTravelDirections (CLKComplication complication, Action<CLKComplicationTimeTravelDirections> handler);

		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Backwards extension and time travel are not longer supported.")]
		[Export ("getTimelineStartDateForComplication:withHandler:")]
		void GetTimelineStartDate (CLKComplication complication, Action<NSDate> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("getTimelineEndDateForComplication:withHandler:")]
		void GetTimelineEndDate (CLKComplication complication, Action<NSDate> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("getPrivacyBehaviorForComplication:withHandler:")]
		void GetPrivacyBehavior (CLKComplication complication, Action<CLKComplicationPrivacyBehavior> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("getTimelineAnimationBehaviorForComplication:withHandler:")]
		void GetTimelineAnimationBehavior (CLKComplication complication, Action<CLKComplicationTimelineAnimationBehavior> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Watch (6, 0)]
		[Export ("getAlwaysOnTemplateForComplication:withHandler:")]
		void GetAlwaysOnTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Abstract]
		[Export ("getCurrentTimelineEntryForComplication:withHandler:")]
		void GetCurrentTimelineEntry (CLKComplication complication, Action<CLKComplicationTimelineEntry> handler);

		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Backwards extension and time travel are not longer supported.")]
		[Export ("getTimelineEntriesForComplication:beforeDate:limit:withHandler:")]
		void GetTimelineEntriesBeforeDate (CLKComplication complication, NSDate beforeDate, nuint limit, Action<CLKComplicationTimelineEntry []> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("getLocalizableSampleTemplateForComplication:withHandler:")]
		void GetLocalizableSampleTemplate (CLKComplication complication, Action<CLKComplicationTemplate> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Watch (7, 0)]
		[Export ("getComplicationDescriptorsWithHandler:")]
		void GetComplicationDescriptors (Action<CLKComplicationDescriptor []> handler);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Watch (7, 0)]
		[Export ("handleSharedComplicationDescriptors:")]
		void HandleSharedComplicationDescriptors (CLKComplicationDescriptor [] complicationDescriptors);

		[Watch (9, 0), NoiOS]
		[Export ("widgetMigrator")]
		CLKComplicationWidgetMigrator WidgetMigrator { get; }
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Backwards extension and time travel is not longer supported.")]
		[Export ("earliestTimeTravelDate")]
		NSDate EarliestTimeTravelDate { get; }

		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Backwards extension and time travel is not longer supported.")]
		[Export ("latestTimeTravelDate")]
		NSDate LatestTimeTravelDate { get; }

		[Export ("reloadTimelineForComplication:")]
		void ReloadTimeline (CLKComplication complication);

		[Export ("extendTimelineForComplication:")]
		void ExtendTimeline (CLKComplication complication);

		[Watch (7, 0)]
		[Export ("reloadComplicationDescriptors")]
		void ReloadComplicationDescriptors ();
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKComplicationTemplate : NSCopying {

		[NullAllowed, Export ("tintColor", ArgumentSemantic.Copy)]
		UIColor TintColor { get; set; }

		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use the provided factories instead.")]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateModularSmallSimpleText Create (CLKTextProvider textProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateModularSmallSimpleImage Create (CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateModularSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateModularSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateModularSmallStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateModularSmallStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularSmallColumnsText Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:body1TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateModularLargeStandardBody Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateModularLargeTallBody {

		[Export ("headerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider HeaderTextProvider { get; set; }

		[Export ("bodyTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BodyTextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:bodyTextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider bodyTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:bodyTextProvider:")]
		CLKComplicationTemplateModularLargeTallBody Create (CLKTextProvider headerTextProvider, CLKTextProvider bodyTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularLargeTable Create (CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:row1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateModularLargeTable Create ([NullAllowed] CLKImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Watch (7, 0)]
		[Export ("initWithRow1ImageProvider:row1Column1TextProvider:row1Column2TextProvider:row2ImageProvider:row2Column1TextProvider:row2Column2TextProvider:row3ImageProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKImageProvider row1ImageProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, [NullAllowed] CLKImageProvider row2ImageProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, [NullAllowed] CLKImageProvider row3ImageProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		CLKComplicationTemplateModularLargeColumns Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithRow1ImageProvider:row1Column1TextProvider:row1Column2TextProvider:row2ImageProvider:row2Column1TextProvider:row2Column2TextProvider:row3ImageProvider:row3Column1TextProvider:row3Column2TextProvider:")]
		CLKComplicationTemplateModularLargeColumns Create ([NullAllowed] CLKImageProvider row1ImageProvider, CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, [NullAllowed] CLKImageProvider row2ImageProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider, [NullAllowed] CLKImageProvider row3ImageProvider, CLKTextProvider row3Column1TextProvider, CLKTextProvider row3Column2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateUtilitarianSmallFlat Create (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateUtilitarianSmallFlat Create (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallSquare {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateUtilitarianSmallSquare Create (CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateUtilitarianSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateUtilitarianSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateUtilitarianLargeFlat {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[NullAllowed]
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateUtilitarianLargeFlat Create (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateUtilitarianLargeFlat Create (CLKTextProvider textProvider, [NullAllowed] CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateCircularSmallSimpleText Create (CLKTextProvider textProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateCircularSmallSimpleImage Create (CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateCircularSmallRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle")]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateCircularSmallRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateCircularSmallStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateCircularSmallStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateCircularSmallStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:")]
		CLKComplicationTemplateExtraLargeSimpleText Create (CLKTextProvider textProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeSimpleImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateExtraLargeSimpleImage Create (CLKImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingText {

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateExtraLargeRingText Create (CLKTextProvider textProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeRingImage {

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider ImageProvider { get; set; }

		[Export ("fillFraction")]
		float FillFraction { get; set; }

		[Export ("ringStyle", ArgumentSemantic.Assign)]
		CLKComplicationRingStyle RingStyle { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:fillFraction:ringStyle:")]
		NativeHandle Constructor (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:fillFraction:ringStyle:")]
		CLKComplicationTemplateExtraLargeRingImage Create (CLKImageProvider imageProvider, float fillFraction, CLKComplicationRingStyle ringStyle);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateExtraLargeStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateExtraLargeStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Export ("highlightLine2")]
		bool HighlightLine2 { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateExtraLargeStackImage Create (CLKImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithRow1Column1TextProvider:row1Column2TextProvider:row2Column1TextProvider:row2Column2TextProvider:")]
		CLKComplicationTemplateExtraLargeColumnsText Create (CLKTextProvider row1Column1TextProvider, CLKTextProvider row1Column2TextProvider, CLKTextProvider row2Column1TextProvider, CLKTextProvider row2Column2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKImageProvider : NSCopying {

		[Deprecated (PlatformName.WatchOS, 7, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithOnePieceImage:")]
		NativeHandle Constructor (UIImage onePieceImage);

		[Watch (7, 0)]
		[Export ("initWithOnePieceImage:twoPieceImageBackground:twoPieceImageForeground:")]
		NativeHandle Constructor (UIImage onePieceImage, [NullAllowed] UIImage twoPieceImageBackground, [NullAllowed] UIImage twoPieceImageForeground);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithText:")]
		NativeHandle Constructor (string text);

		[Watch (7, 0)]
		[Export ("initWithText:shortText:")]
		NativeHandle Constructor (string text, [NullAllowed] string shortText);

		[Watch (7, 0)]
		[Export ("initWithText:shortText:accessibilityLabel:")]
		NativeHandle Constructor (string text, [NullAllowed] string shortText, [NullAllowed] string accessibilityLabel);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (6, 0)]
		[Export ("uppercase")]
		bool Uppercase { get; set; }

		[Watch (7, 0)]
		[Export ("initWithDate:units:")]
		NativeHandle Constructor (NSDate date, NSCalendarUnit calendarUnits);

		[Watch (7, 0)]
		[Export ("initWithDate:units:timeZone:")]
		NativeHandle Constructor (NSDate date, NSCalendarUnit calendarUnits, [NullAllowed] NSTimeZone timeZone);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithDate:")]
		NativeHandle Constructor (NSDate date);

		[Watch (7, 0)]
		[Export ("initWithDate:timeZone:")]
		NativeHandle Constructor (NSDate date, [NullAllowed] NSTimeZone timeZone);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithStartDate:endDate:")]
		NativeHandle Constructor (NSDate startDate, NSDate endDate);

		[Watch (7, 0)]
		[Export ("initWithStartDate:endDate:timeZone:")]
		NativeHandle Constructor (NSDate startDate, NSDate endDate, [NullAllowed] NSTimeZone timeZone);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
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

		[Watch (7, 0)]
		[Export ("initWithDate:style:units:")]
		NativeHandle Constructor (NSDate date, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Watch (7, 0)]
		[Export ("initWithDate:relativeToDate:style:units:")]
		NativeHandle Constructor (NSDate date, [NullAllowed] NSDate relativeDate, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Watch (7, 0)]
		[Static]
		[Export ("textProviderWithDate:relativeToDate:style:units:")]
		CLKRelativeDateTextProvider Create (NSDate date, [NullAllowed] NSDate relativeToDate, CLKRelativeDateStyle style, NSCalendarUnit calendarUnits);

		[Watch (7, 0)]
		[Export ("relativeToDate", ArgumentSemantic.Retain), NullAllowed]
		NSDate RelativeToDate { get; set; }
	}

	[Static]
	interface CLKLaunchOptionsKeys {

		[Field ("CLKLaunchedTimelineEntryDateKey")]
		NSString LaunchedTimelineEntryDate { get; }

		[Watch (7, 0)]
		[Field ("CLKLaunchedComplicationIdentifierKey")]
		NSString LaunchedComplicationIdentifierKey { get; }
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicBezelCircularText {
		[Export ("circularTemplate", ArgumentSemantic.Copy)]
		CLKComplicationTemplateGraphicCircular CircularTemplate { get; set; }

		[NullAllowed, Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithCircularTemplate:")]
		NativeHandle Constructor (CLKComplicationTemplateGraphicCircular circularTemplate);

		[Watch (7, 0)]
		[Export ("initWithCircularTemplate:textProvider:")]
		NativeHandle Constructor (CLKComplicationTemplateGraphicCircular circularTemplate, [NullAllowed] CLKTextProvider textProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithCircularTemplate:")]
		CLKComplicationTemplateGraphicBezelCircularText Create (CLKComplicationTemplateGraphicCircular circularTemplate);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithCircularTemplate:textProvider:")]
		CLKComplicationTemplateGraphicBezelCircularText Create (CLKComplicationTemplateGraphicCircular circularTemplate, [NullAllowed] CLKTextProvider textProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[Abstract] // <quote>An abstract superclass for all the circular graphic templates.</quote>
	[DisableDefaultCtor]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCircular {
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularClosedGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCircularClosedGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularClosedGaugeText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularClosedGaugeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider centerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicCircularImage Create (CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularOpenGaugeImage {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider BottomImageProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:bottomImageProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider bottomImageProvider, CLKTextProvider centerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeRangeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider leadingTextProvider, CLKTextProvider trailingTextProvider, CLKTextProvider centerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText {
		[Export ("gaugeProvider", ArgumentSemantic.Copy)]
		CLKGaugeProvider GaugeProvider { get; set; }

		[Export ("bottomTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider BottomTextProvider { get; set; }

		[Export ("centerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider CenterTextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:bottomTextProvider:centerTextProvider:")]
		CLKComplicationTemplateGraphicCircularOpenGaugeSimpleText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider bottomTextProvider, CLKTextProvider centerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerCircularImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithImageProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithImageProvider:")]
		CLKComplicationTemplateGraphicCornerCircularImage Create (CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeImage Create (CLKGaugeProvider gaugeProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeImage Create (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, CLKTextProvider outerTextProvider);

		[Watch (7, 0)]
		[Export ("initWithGaugeProvider:leadingTextProvider:trailingTextProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKTextProvider outerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeText Create (CLKGaugeProvider gaugeProvider, CLKTextProvider outerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithGaugeProvider:leadingTextProvider:trailingTextProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerGaugeText Create (CLKGaugeProvider gaugeProvider, [NullAllowed] CLKTextProvider leadingTextProvider, [NullAllowed] CLKTextProvider trailingTextProvider, CLKTextProvider outerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerStackText {
		[Export ("innerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider InnerTextProvider { get; set; }

		[Export ("outerTextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider OuterTextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithInnerTextProvider:outerTextProvider:")]
		NativeHandle Constructor (CLKTextProvider innerTextProvider, CLKTextProvider outerTextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithInnerTextProvider:outerTextProvider:")]
		CLKComplicationTemplateGraphicCornerStackText Create (CLKTextProvider innerTextProvider, CLKTextProvider outerTextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicCornerTextImage {
		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicCornerTextImage Create (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicRectangularLargeImage {
		[Export ("imageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider ImageProvider { get; set; }

		[Export ("textProvider", ArgumentSemantic.Copy)]
		CLKTextProvider TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithTextProvider:imageProvider:")]
		NativeHandle Constructor (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithTextProvider:imageProvider:")]
		CLKComplicationTemplateGraphicRectangularLargeImage Create (CLKTextProvider textProvider, CLKFullColorImageProvider imageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:body1TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:body2TextProvider:")]
		CLKComplicationTemplateGraphicRectangularStandardBody Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, [NullAllowed] CLKTextProvider body2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (7, 0)]
		[Export ("initWithHeaderTextProvider:body1TextProvider:gaugeProvider:")]
		NativeHandle Constructor (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Watch (7, 0)]
		[Export ("initWithHeaderImageProvider:headerTextProvider:body1TextProvider:gaugeProvider:")]
		NativeHandle Constructor ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderTextProvider:body1TextProvider:gaugeProvider:")]
		CLKComplicationTemplateGraphicRectangularTextGauge Create (CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithHeaderImageProvider:headerTextProvider:body1TextProvider:gaugeProvider:")]
		CLKComplicationTemplateGraphicRectangularTextGauge Create ([NullAllowed] CLKFullColorImageProvider headerImageProvider, CLKTextProvider headerTextProvider, CLKTextProvider body1TextProvider, CLKGaugeProvider gaugeProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLKFullColorImageProvider : NSCopying {
		[Static]
		[Export ("providerWithFullColorImage:")]
		CLKFullColorImageProvider Create (UIImage image);

		[Watch (6, 0)]
		[Static]
		[Export ("providerWithFullColorImage:tintedImageProvider:")]
		CLKFullColorImageProvider Create (UIImage image, [NullAllowed] CLKImageProvider tintedImageProvider);

		[Export ("image", ArgumentSemantic.Retain)]
		UIImage Image { get; set; }

		[Watch (6, 0)]
		[NullAllowed, Export ("tintedImageProvider", ArgumentSemantic.Retain)]
		CLKImageProvider TintedImageProvider { get; set; }

		[NullAllowed, Export ("accessibilityLabel", ArgumentSemantic.Retain)]
		string AccessibilityLabel { get; set; }

		[Watch (7, 0)]
		[Export ("init")]
		NativeHandle Constructor ();

		[Watch (7, 0)]
		[Export ("initWithFullColorImage:")]
		NativeHandle Constructor (UIImage fullColorImage);

		[Watch (7, 0)]
		[Export ("initWithFullColorImage:tintedImageProvider:")]
		NativeHandle Constructor (UIImage fullColorImage, [NullAllowed] CLKImageProvider tintedImageProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

		[Watch (5, 2)]
		[NullAllowed, Export ("accessibilityLabel")]
		string AccessibilityLabel { get; set; }
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (5, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (6, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularStackText {

		[Export ("line1TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line1TextProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1TextProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1TextProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicCircularStackText Create (CLKTextProvider line1TextProvider, CLKTextProvider line2TextProvider);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (6, 0)]
	[BaseType (typeof (CLKComplicationTemplateGraphicCircular))]
	interface CLKComplicationTemplateGraphicCircularStackImage {

		[Export ("line1ImageProvider", ArgumentSemantic.Copy)]
		CLKFullColorImageProvider Line1ImageProvider { get; set; }

		[Export ("line2TextProvider", ArgumentSemantic.Copy)]
		CLKTextProvider Line2TextProvider { get; set; }

		[Watch (7, 0)]
		[Export ("initWithLine1ImageProvider:line2TextProvider:")]
		NativeHandle Constructor (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);

		[Watch (7, 0)]
		[Static]
		[Export ("templateWithLine1ImageProvider:line2TextProvider:")]
		CLKComplicationTemplateGraphicCircularStackImage Create (CLKFullColorImageProvider line1ImageProvider, CLKTextProvider line2TextProvider);
	}

	[Watch (7, 0), iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	interface CLKWatchFaceLibrary {
		[Async]
		[Export ("addWatchFaceAtURL:completionHandler:")]
		void AddWatchFace (NSUrl fileUrl, Action<NSError> handler);
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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
	[Watch (7, 0)]
	[BaseType (typeof (CLKComplicationTemplate))]
	interface CLKComplicationTemplateGraphicExtraLargeCircular : NSSecureCoding {
	}

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
	[Watch (7, 0)]
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

	[Watch (7, 0)]
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

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("initWithIdentifier:displayName:supportedFamilies:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("initWithIdentifier:displayName:supportedFamilies:userInfo:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies, NSDictionary userInfo);

		[Deprecated (PlatformName.WatchOS, 9, 0, message: "Use WidgetKit instead.")]
		[Export ("initWithIdentifier:displayName:supportedFamilies:userActivity:")]
		NativeHandle Constructor (string identifier, string displayName, [BindAs (typeof (CLKComplicationFamily []))] NSNumber [] supportedFamilies, NSUserActivity userActivity);
	}

	[Watch (9, 0), NoiOS]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationWidgetMigrationConfiguration : NSCopying { }

	[Watch (9, 0), NoiOS]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface CLKComplicationWidgetMigrator {
		[Async]
		[Export ("getWidgetConfigurationFrom:completionHandler:")]
		void GetWidgetConfiguration (CLKComplicationDescriptor complicationDescriptor, Action<CLKComplicationWidgetMigrationConfiguration> completionHandler);
	}

	[Watch (9, 0), NoiOS]
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


	[Watch (9, 0), NoiOS]
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
