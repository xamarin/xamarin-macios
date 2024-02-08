//
// ClassKit bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using System.Reflection;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ClassKit {

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Native]
	enum CLSBinaryValueType : long {
		TrueFalse = 0,
		PassFail,
		YesNo,
		[MacCatalyst (14, 0)]
		CorrectIncorrect,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Native]
	enum CLSContextType : long {
		None = 0,
		App,
		Chapter,
		Section,
		Level,
		Page,
		Task,
		Challenge,
		Quiz,
		Exercise,
		Lesson,
		Book,
		Game,
		Document,
		Audio,
		Video,
		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		Course,
		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		Custom,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Native]
	[ErrorDomain ("CLSErrorCodeDomain")]
	public enum CLSErrorCode : long {
		None = 0,
		ClassKitUnavailable,
		InvalidArgument,
		InvalidModification,
		AuthorizationDenied,
		DatabaseInaccessible,
		Limits,
		InvalidCreate,
		InvalidUpdate,
		PartialFailure,
		InvalidAccountCredentials,
	}


	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	enum CLSContextTopic {
		[Field ("CLSContextTopicMath")]
		Math,
		[Field ("CLSContextTopicScience")]
		Science,
		[Field ("CLSContextTopicLiteracyAndWriting")]
		LiteracyAndWriting,
		[Field ("CLSContextTopicWorldLanguage")]
		WorldLanguage,
		[Field ("CLSContextTopicSocialScience")]
		SocialScience,
		[Field ("CLSContextTopicComputerScienceAndEngineering")]
		ComputerScienceAndEngineering,
		[Field ("CLSContextTopicArtsAndMusic")]
		ArtsAndMusic,
		[Field ("CLSContextTopicHealthAndFitness")]
		HealthAndFitness,
		// CLSContext.Topic should be nullable, thus we should add a null option
		[Field (null)]
		None = 1000,
	}


	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0), iOS (14, 0)]
	[Native]
	public enum CLSProgressReportingCapabilityKind : long {
		Duration = 0,
		Percent,
		Binary,
		Quantity,
		Score,
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Static]
	interface CLSErrorUserInfoKeys {

		[Field ("CLSErrorObjectKey")]
		NSString ObjectKey { get; }

		[Field ("CLSErrorUnderlyingErrorsKey")]
		NSString UnderlyingErrorsKey { get; }

		[Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CLSErrorSuccessfulObjectsKey")]
		NSString SuccessfulObjectsKey { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Static]
	interface CLSPredicateKeyPath {
		[Field ("CLSPredicateKeyPathDateCreated")]
		NSString DateCreated { get; }

		[Field ("CLSPredicateKeyPathIdentifier")]
		NSString Identifier { get; }

		[Field ("CLSPredicateKeyPathTitle")]
		NSString Title { get; }

		[Field ("CLSPredicateKeyPathUniversalLinkURL")]
		NSString UniversalLinkUrl { get; }

		[Field ("CLSPredicateKeyPathTopic")]
		NSString Topic { get; }

		[Field ("CLSPredicateKeyPathParent")]
		NSString Parent { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLSObject : NSSecureCoding {

		[Export ("dateCreated", ArgumentSemantic.Strong)]
		NSDate DateCreated { get; }

		[Export ("dateLastModified", ArgumentSemantic.Strong)]
		NSDate DateLastModified { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSActivity {

		[Export ("progress")]
		double Progress { get; set; }

		[Export ("duration")]
		double Duration { get; }

		[NullAllowed]
		[Export ("primaryActivityItem", ArgumentSemantic.Strong)]
		CLSActivityItem PrimaryActivityItem { get; set; }

		[Export ("addProgressRangeFromStart:toEnd:")]
		void AddProgressRange (double start, double end);

		[Export ("addAdditionalActivityItem:")]
		void AddAdditionalActivityItem (CLSActivityItem activityItem);

		[Export ("additionalActivityItems", ArgumentSemantic.Strong)]
		CLSActivityItem [] AdditionalActivityItems { get; }

		// From CLSActivity (Activation) Category

		[Export ("started")]
		bool Started { [Bind ("isStarted")] get; }

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();

		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[Export ("removeAllActivityItems")]
		void RemoveAllActivityItems ();
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSActivityItem {

		[Export ("title")]
		string Title { get; set; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSBinaryItem {

		[Export ("value")]
		bool Value { get; set; }

		[Export ("valueType", ArgumentSemantic.Assign)]
		CLSBinaryValueType ValueType { get; }

		[Export ("initWithIdentifier:title:type:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string title, CLSBinaryValueType valueType);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSContext {

		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		[Export ("identifierPath", ArgumentSemantic.Copy)]
		string [] IdentifierPath { get; }

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("universalLinkURL", ArgumentSemantic.Strong)]
		NSUrl UniversalLinkUrl { get; set; }

		[Export ("type", ArgumentSemantic.Assign)]
		CLSContextType Type { get; }

		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("customTypeName")]
		string CustomTypeName { get; set; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("displayOrder")]
		nint DisplayOrder { get; set; }

		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("summary")]
		string Summary { get; set; }

		[iOS (13, 4)]
		[MacCatalyst (14, 0)]
		[NullAllowed, Export ("thumbnail", ArgumentSemantic.Assign)]
		CGImage Thumbnail { get; set; }

		[Protected]
		[NullAllowed, Export ("topic", ArgumentSemantic.Copy)]
		NSString WeakTopic { get; set; }

		[Export ("initWithType:identifier:title:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CLSContextType type, string identifier, string title);

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }

		[Export ("becomeActive")]
		void BecomeActive ();

		[Export ("resignActive")]
		void ResignActive ();

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("assignable")]
		bool Assignable { [Bind ("isAssignable")] get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("suggestedAge", ArgumentSemantic.Assign)]
		NSRange SuggestedAge { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("suggestedCompletionTime", ArgumentSemantic.Assign)]
		NSRange SuggestedCompletionTime { get; set; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("progressReportingCapabilities", ArgumentSemantic.Copy)]
		NSSet<CLSProgressReportingCapability> ProgressReportingCapabilities { get; }

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("setType:")]
		void SetType (CLSContextType type);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("addProgressReportingCapabilities:")]
		void AddProgressReportingCapabilities (NSSet<CLSProgressReportingCapability> capabilities);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("resetProgressReportingCapabilities")]
		void ResetProgressReportingCapabilities ();

		// From CLSContext (Hierarchy) Category

		[NullAllowed, Export ("parent")]
		CLSContext Parent { get; }

		[Export ("removeFromParent")]
		void RemoveFromParent ();

		[Export ("addChildContext:")]
		void AddChild (CLSContext childContext);

		[Async]
		[Export ("descendantMatchingIdentifierPath:completion:")]
		void FindDescendantMatching (string [] identifierPath, Action<CLSContext, NSError> completion);

		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[Export ("navigationChildContexts", ArgumentSemantic.Copy)]
		CLSContext [] NavigationChildContexts { get; }

		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[Export ("addNavigationChildContext:")]
		void AddNavigationChild (CLSContext childContext);

		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[Export ("removeNavigationChildContext:")]
		void RemoveNavigationChild (CLSContext childContext);

		// From CLSContext (Activity) Category

		[NullAllowed, Export ("currentActivity", ArgumentSemantic.Strong)]
		CLSActivity CurrentActivity { get; }

		[Export ("createNewActivity")]
		CLSActivity CreateNewActivity ();
	}

	interface ICLSDataStoreDelegate { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CLSDataStoreDelegate {

		[Abstract]
		[Export ("createContextForIdentifier:parentContext:parentIdentifierPath:")]
		[return: NullAllowed]
		CLSContext CreateContext (string identifier, CLSContext parentContext, string [] parentIdentifierPath);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLSDataStore {

		[Static]
		[Export ("shared", ArgumentSemantic.Strong)]
		CLSDataStore Shared { get; }

		[Export ("mainAppContext", ArgumentSemantic.Strong)]
		CLSContext MainAppContext { get; }

		[NullAllowed, Export ("activeContext", ArgumentSemantic.Strong)]
		CLSContext ActiveContext { get; }

		[NullAllowed, Export ("runningActivity", ArgumentSemantic.Strong)]
		CLSActivity RunningActivity { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ICLSDataStoreDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Export ("saveWithCompletion:")]
		void Save ([NullAllowed] Action<NSError> completion);

		[MacCatalyst (14, 0)]
		[Export ("completeAllAssignedActivitiesMatching:")]
		void CompleteAllAssignedActivitiesMatching (string [] contextPath);

		// From CLSDataStore (Contexts) Category

		[Async]
		[Export ("contextsMatchingPredicate:completion:")]
		void FindContextsMatching (NSPredicate predicate, Action<CLSContext [], NSError> completion);

		[Async]
		[Export ("contextsMatchingIdentifierPath:completion:")]
		void FindContextsMatching (string [] identifierPath, Action<CLSContext [], NSError> completion);

		[Export ("removeContext:")]
		void Remove (CLSContext context);

		[Introduced (PlatformName.MacCatalyst, 14, 5)]
		[Mac (11, 3)]
		[iOS (14, 5)]
		[Async]
		[Export ("fetchActivityForURL:completion:")]
		void FetchActivity (NSUrl url, Action<CLSActivity, NSError> completion);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSQuantityItem {

		[Export ("quantity")]
		double Quantity { get; set; }

		[Export ("initWithIdentifier:title:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string title);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSScoreItem {

		[Export ("score")]
		double Score { get; set; }

		[Export ("maxScore")]
		double MaxScore { get; set; }

		[Export ("initWithIdentifier:title:score:maxScore:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier, string title, double score, double maxScore);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0)]
	[Protocol]
	interface CLSContextProvider {
		[Abstract]
		[Export ("updateDescendantsOfContext:completion:")]
		void UpdateDescendants (CLSContext context, Action<NSError> completion);
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[NoWatch, NoTV, Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSProgressReportingCapability {
		[Export ("kind", ArgumentSemantic.Assign)]
		CLSProgressReportingCapabilityKind Kind { get; }

		[NullAllowed]
		[Export ("details")]
		string Details { get; }

		[Export ("initWithKind:details:")]
		NativeHandle Constructor (CLSProgressReportingCapabilityKind kind, [NullAllowed] string details);
	}

}
