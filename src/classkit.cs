//
// ClassKit bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0
using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System.Reflection;

namespace XamCore.ClassKit {

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[Native]
	enum CLSBinaryValueType : nint {
		TrueFalse = 0,
		PassFail,
		YesNo,
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[Native]
	enum CLSContextType : nint {
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
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[Native]
	[ErrorDomain ("CLSErrorCodeDomain")]
	public enum CLSErrorCode : nint {
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
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
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
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[Static]
	interface CLSErrorUserInfoKeys {

		[Field ("CLSErrorObjectKey")]
		NSString ObjectKey { get; }

		[Field ("CLSErrorUnderlyingErrorsKey")]
		NSString UnderlyingErrorsKey { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	enum CLSPredicateKeyPaths {
		[Field ("CLSPredicateKeyPathDateCreated")]
		DateCreated,
		[Field ("CLSPredicateKeyPathIdentifier")]
		Identifier,
		[Field ("CLSPredicateKeyPathTitle")]
		Title,
		[Field ("CLSPredicateKeyPathUniversalLinkURL")]
		UniversalLinkUrl,
		[Field ("CLSPredicateKeyPathTopic")]
		Topic,
		[Field ("CLSPredicateKeyPathParent")]
		Parent,
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLSObject : NSSecureCoding {

		[Export ("dateCreated")]
		NSDate DateCreated { get; }

		[Export ("dateLastModified")]
		NSDate DateLastModified { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSActivity {

		[Export ("progress")]
		double Progress { get; set; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("primaryActivityItem", ArgumentSemantic.Assign)]
		CLSActivityItem PrimaryActivityItem { get; set; }

		[Export ("addProgressRangeFromStart:toEnd:")]
		void AddProgressRange (double start, double end);

		[Export ("addAdditionalActivityItem:")]
		void AddActivityItem (CLSActivityItem activityItem);

		[Export ("additionalActivityItems")]
		CLSActivityItem [] AdditionalActivityItems { get; }

		// From CLSActivity (Activation) Category

		[Export ("started")]
		bool Started { [Bind ("isStarted")] get; }

		[Export ("start")]
		void Start ();

		[Export ("stop")]
		void Stop ();
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSActivityItem {

		[Export ("title")]
		string Title { get; set; }

		[Export ("identifier")]
		string Identifier { get; }
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSBinaryItem {

		[Export ("value")]
		bool Value { get; set; }

		[Export ("valueType")]
		CLSBinaryValueType ValueType { get; }

		[Export ("initWithIdentifier:title:type:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string title, CLSBinaryValueType valueType);
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSObject))]
	[DisableDefaultCtor]
	interface CLSContext {

		[Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("universalLinkURL", ArgumentSemantic.Assign)]
		NSUrl UniversalLinkUrl { get; set; }

		[Export ("type")]
		CLSContextType Type { get; }

		[Export ("title")]
		string Title { get; set; }

		[Export ("displayOrder")]
		nint DisplayOrder { get; set; }

		[Protected]
		[NullAllowed, Export ("topic")]
		NSString WeakTopic { get; set; }

		[Export ("initWithType:identifier:title:")]
		[DesignatedInitializer]
		IntPtr Constructor (CLSContextType type, string identifier, string title);

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("becomeActive")]
		void BecomeActive ();

		[Export ("resignActive")]
		void ResignActive ();

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

		// From CLSContext (Activity) Category

		[NullAllowed, Export ("currentActivity")]
		CLSActivity CurrentActivity { get; }

		[Export ("createNewActivity")]
		CLSActivity CreateNewActivity ();
	}

	interface ICLSDataStoreDelegate { }

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CLSDataStoreDelegate {

		[Abstract]
		[Export ("createContextForIdentifier:parentContext:parentIdentifierPath:")]
		CLSContext CreateContext (string identifier, CLSContext parentContext, string [] parentIdentifierPath);
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CLSDataStore {

		[Static]
		[Export ("shared")]
		CLSDataStore Shared { get; }

		[Export ("mainAppContext")]
		CLSContext MainAppContext { get; }

		[NullAllowed, Export ("activeContext")]
		CLSContext ActiveContext { get; }

		[NullAllowed, Export ("runningActivity")]
		CLSActivity RunningActivity { get; }

		[Wrap ("WeakDelegate"), Protocolize]
		[NullAllowed]
		CLSDataStoreDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Async]
		[Export ("saveWithCompletion:")]
		void Save ([NullAllowed] Action<NSError> completion);

		// From CLSDataStore (Contexts) Category

		[Async]
		[Export ("contextsMatchingPredicate:completion:")]
		void FindContextsMatching (NSPredicate predicate, Action<CLSContext [], NSError> completion);

		[Async]
		[Export ("contextsMatchingIdentifierPath:completion:")]
		void FindContextsMatching (string [] identifierPath, Action<CLSContext [], NSError> completion);

		[Export ("removeContext:")]
		void Remove (CLSContext context);
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSQuantityItem {

		[Export ("quantity")]
		double Quantity { get; set; }

		[Export ("initWithIdentifier:title:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string title);
	}

	[NoWatch, NoTV, NoMac, iOS (11,4)]
	[BaseType (typeof (CLSActivityItem))]
	[DisableDefaultCtor]
	interface CLSScoreItem {

		[Export ("score")]
		double Score { get; set; }

		[Export ("maxScore")]
		double MaxScore { get; set; }

		[Export ("initWithIdentifier:title:score:maxScore:")]
		[DesignatedInitializer]
		IntPtr Constructor (string identifier, string title, double score, double maxScore);
	}
}
#endif
