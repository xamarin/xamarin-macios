//
// CoreSpotlight bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

using System.ComponentModel;
using UniformTypeIdentifiers;

namespace CoreSpotlight {

	[Partial]
	interface CoreSpotlightConstants {
		[Field ("CoreSpotlightVersionNumber")]
		double CoreSpotlightVersionNumber { get; }

		[Field ("CoreSpotlightVersionString")]
		[Internal]
		IntPtr _CoreSpotlightVersionString { get; }

		[Static]
		string CoreSpotlightVersionString {
			[Wrap ("Marshal.PtrToStringUTF8 (_CoreSpotlightVersionString)!")]
			get;
		}
	}

	/// <summary>Handler for communication between the application and the index on the device. The app does not need to be running for this communication to occur.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSIndexExtensionRequestHandler_Class/index.html">Apple documentation for <c>CSIndexExtensionRequestHandler</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CSIndexExtensionRequestHandler : NSExtensionRequestHandling, CSSearchableIndexDelegate {

	}

	/// <summary>An author or a recipient stored in a <see cref="CoreSpotlight.CSSearchableItemAttributeSet" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSPerson_Class/index.html">Apple documentation for <c>CSPerson</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CSPerson : NSSecureCoding, NSCopying {

		/// <param name="displayName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="handles">To be added.</param>
		/// <param name="handleIdentifier">To be added.</param>
		/// <summary>Creates a new CSPerson with the specified handles and handle identifier.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithDisplayName:handles:handleIdentifier:")]
		NativeHandle Constructor ([NullAllowed] string displayName, string [] handles, NSString handleIdentifier);

		/// <summary>Gets the display name for the person.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("displayName")]
		string DisplayName { get; }

		/// <summary>Gets a list of contact handles for the person.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("handles")]
		string [] Handles { get; }

		/// <summary>Gets the key that represents the resource type that the handle represents.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("handleIdentifier")]
		NSString HandleIdentifier { get; }

		/// <summary>Gets or sets the contact identifier for the CSPerson.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>Setting this property to the <see cref="Contacts.CNContact.Identifier" /> property for the contact enables direct lookup of the contact.</remarks>
		[NullAllowed]
		[Export ("contactIdentifier")]
		string ContactIdentifier { get; set; }
	}

	/// <summary>A search index used by Spotlight.</summary>
	///     <remarks>
	///       <para>Typically, developers should use <see cref="CoreSpotlight.CSSearchableIndex.DefaultSearchableIndex" /> to index their application data.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSSearchableIndex_Class/index.html">Apple documentation for <c>CSSearchableIndex</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableIndex {

		/// <summary>Gets or sets the delegate that responds to requests for index-related tasks.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Export ("indexDelegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		ICSSearchableIndexDelegate IndexDelegate { get; set; }

		/// <summary>Gets the file protection class used by the search index.</summary>
		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("protectionClass")]
		[BindAs (typeof (NSFileProtectionType))]
		NSString ProtectionClass { get; }

		/// <summary>Gets a Boolean value that tells whether indexing is available.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("isIndexingAvailable")]
		bool IsIndexingAvailable { get; }

		/// <summary>Gets the default search index for the device.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("defaultSearchableIndex")]
		CSSearchableIndex DefaultSearchableIndex { get; }

		/// <param name="name">To be added.</param>
		/// <summary>Creates a new index on the device with the specified name.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		/// <param name="name">To be added.</param>
		/// <param name="protectionClass">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new index on the device with the specified name and protection class.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("initWithName:protectionClass:")]
		NativeHandle Constructor (string name, [NullAllowed] NSString protectionClass);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[iOS (17, 0), MacCatalyst (17, 0), NoTV]
		[Export ("initWithName:protectionClass:bundleIdentifier:options:")]
		NativeHandle Constructor (string name, [NullAllowed] NSString protectionClass, string bundleIdentifier, nint options);

		[Wrap ("this (name, protectionClass.GetConstant (), bundleIdentifier, options)")]
		[iOS (17, 0), MacCatalyst (17, 0), NoTV]
		NativeHandle Constructor (string name, NSFileProtectionType protectionClass, string bundleIdentifier, nint options);

		/// <param name="items">The items to index.</param>
		///         <param name="completionHandler">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Indexes the specified searchable items and runs <paramref name="completionHandler" /> when finished.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("indexSearchableItems:completionHandler:")]
		[Async (XmlDocs = """
			<param name="items">The items to index.</param>
			<summary>Asynchronously indexes the specified searchable items.</summary>
			<returns>A task that represents the asynchronous Index operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Index (CSSearchableItem [] items, [NullAllowed] Action<NSError> completionHandler);

		/// <param name="identifiers">To be added.</param>
		///         <param name="completionHandler">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Removes the identified items and runs <paramref name="completionHandler" /> when finished.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("deleteSearchableItemsWithIdentifiers:completionHandler:")]
		[Async (XmlDocs = """
			<param name="identifiers">To be added.</param>
			<summary>Asynchronously removes the identified items.</summary>
			<returns>A task that represents the asynchronous Delete operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Delete (string [] identifiers, [NullAllowed] Action<NSError> completionHandler);

		/// <param name="domainIdentifiers">The domain identifier for the items to delete.</param>
		///         <param name="completionHandler">Handler that is called after the index change is journaled. may be <see langword="null" />.
		///         <para tool="nullallowed">This parameter can be <see langword="null" />.</para></param>
		///         <summary>Removes all items from the specified domains and runs <paramref name="completionHandler" /> after the index change is journaled.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("deleteSearchableItemsWithDomainIdentifiers:completionHandler:")]
		[Async (XmlDocs = """
			<param name="domainIdentifiers">The domain identifier for the items to delete.</param>
			<summary>Asynchronously removes all items from the specified domains.</summary>
			<returns>A task that represents the asynchronous DeleteWithDomain operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void DeleteWithDomain (string [] domainIdentifiers, [NullAllowed] Action<NSError> completionHandler);

		/// <param name="completionHandler">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Removes all items and runs <paramref name="completionHandler" /> when finished.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("deleteAllSearchableItemsWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>Asynchronously removes all items.</summary>
			<returns>A task that represents the asynchronous DeleteAll operation</returns>
			<remarks>
			          <para copied="true">The DeleteAllAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void DeleteAll ([NullAllowed] Action<NSError> completionHandler);

		// from interface CSExternalProvider (CSSearchableIndex)

		[Async (ResultTypeName = "CSSearchableIndexBundleDataResult")]
		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("provideDataForBundle:identifier:type:completionHandler:")]
		void ProvideData (string bundle, string identifier, string type, Action<NSData, NSError> completionHandler);

		[Async (ResultTypeName = "CSSearchableIndexBundleDataResult")]
		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("fetchDataForBundleIdentifier:itemIdentifier:contentType:completionHandler:")]
		void FetchData (string bundleIdentifier, string itemIdentifier, UTType contentType, Action<NSData, NSError> completionHandler);
	}

	/// <summary>Completion handler used in <see cref="CoreSpotlight.CSSearchableIndex_CSOptionalBatchingExtension.FetchLastClientState(CoreSpotlight.CSSearchableIndex,CoreSpotlight.CSSearchableIndexFetchHandler)" />.</summary>
	delegate void CSSearchableIndexFetchHandler (NSData clientState, [NullAllowed] NSError error);

	delegate void CSSearchableIndexEndIndexHandler ([NullAllowed] NSError error);

	/// <summary>Extension methods for <see cref="CSSearchableIndex" />.</summary>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (CSSearchableIndex))]
	interface CSSearchableIndex_CSOptionalBatchingExtension {

		/// <summary>Begins an index update batch.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("beginIndexBatch")]
		void BeginIndexBatch ();

		/// <param name="clientState">To be added.</param>
		/// <param name="completionHandler">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Ends an index update batch, relying on the 250 bytes of information for crash recovery, and calls <paramref name="completionHandler" /> when finished.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("endIndexBatchWithClientState:completionHandler:")]
		void EndIndexBatch (NSData clientState, [NullAllowed] Action<NSError> completionHandler);

		/// <param name="completionHandler">To be added.</param>
		/// <summary>Fetches the client state and runs <paramref name="completionHandler" /> when finished..</summary>
		/// <remarks>To be added.</remarks>
		[Export ("fetchLastClientStateWithCompletionHandler:")]
		void FetchLastClientState (CSSearchableIndexFetchHandler completionHandler);

		[NoTV, Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("endIndexBatchWithExpectedClientState:newClientState:completionHandler:")]
		void EndIndexBatch ([NullAllowed] NSData expectedClientState, NSData newClientState, [NullAllowed] CSSearchableIndexEndIndexHandler completionHandler);
	}

	interface ICSSearchableIndexDelegate { }

	/// <summary>Delegate object providing members that are called when reindexing the index.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSSearchableIndexDelegate_Protocol/index.html">Apple documentation for <c>CSSearchableIndexDelegate</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface CSSearchableIndexDelegate {

		/// <param name="searchableIndex">To be added.</param>
		///         <param name="acknowledgementHandler">To be added.</param>
		///         <summary>Reindexes all items in the specified index and runs <paramref name="acknowledgementHandler" /> when finished.</summary>
		///         <remarks>To be added.</remarks>
		[Abstract]
		[Export ("searchableIndex:reindexAllSearchableItemsWithAcknowledgementHandler:")]
		void ReindexAllSearchableItems (CSSearchableIndex searchableIndex, Action acknowledgementHandler);

		/// <param name="searchableIndex">To be added.</param>
		///         <param name="identifiers">To be added.</param>
		///         <param name="acknowledgementHandler">To be added.</param>
		///         <summary>Reindexes the specified items in the specified index and runs <paramref name="acknowledgementHandler" /> when finished.</summary>
		///         <remarks>To be added.</remarks>
		[Abstract]
		[Export ("searchableIndex:reindexSearchableItemsWithIdentifiers:acknowledgementHandler:")]
		void ReindexSearchableItems (CSSearchableIndex searchableIndex, string [] identifiers, Action acknowledgementHandler);

		/// <param name="searchableIndex">To be added.</param>
		///         <summary>Method that is called after index throttling starts.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("searchableIndexDidThrottle:")]
		void DidThrottle (CSSearchableIndex searchableIndex);

		/// <param name="searchableIndex">To be added.</param>
		///         <summary>Method that is called after index throttling is stopped..</summary>
		///         <remarks>To be added.</remarks>
		[Export ("searchableIndexDidFinishThrottle:")]
		void DidFinishThrottle (CSSearchableIndex searchableIndex);

		/// <param name="searchableIndex">To be added.</param>
		///         <param name="itemIdentifier">To be added.</param>
		///         <param name="typeIdentifier">To be added.</param>
		///         <param name="outError">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("dataForSearchableIndex:itemIdentifier:typeIdentifier:error:")]
		[return: NullAllowed]
		NSData GetData (CSSearchableIndex searchableIndex, string itemIdentifier, string typeIdentifier, out NSError outError);

		/// <param name="searchableIndex">To be added.</param>
		///         <param name="itemIdentifier">To be added.</param>
		///         <param name="typeIdentifier">To be added.</param>
		///         <param name="inPlace">To be added.</param>
		///         <param name="outError">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("fileURLForSearchableIndex:itemIdentifier:typeIdentifier:inPlace:error:")]
		[return: NullAllowed]
		NSUrl GetFileUrl (CSSearchableIndex searchableIndex, string itemIdentifier, string typeIdentifier, bool inPlace, out NSError outError);

		[NoTV]
		[iOS (18, 4), Mac (15, 4), MacCatalyst (18, 4)]
		[Export ("searchableItemsForIdentifiers:searchableItemsHandler:")]
		void GetSearchableItems (string [] identifiers, CSSearchableIndexDelegateGetSearchableItemsHandler searchableItemsHandler);

		/// <param name="identifiers">The identifiers of the searchable items to retrieve.</param>
		/// <param name="protectionClass">The file protection class for the requested items.</param>
		/// <param name="searchableItemsHandler">The handler to invoke with the matching searchable items.</param>
		/// <summary>Provides searchable items for the specified identifiers and file protection class.</summary>
		[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
		[Export ("searchableItemsForIdentifiers:protectionClass:searchableItemsHandler:")]
		void GetSearchableItems (string [] identifiers, [BindAs (typeof (NSFileProtectionType))] NSString protectionClass, CSSearchableIndexDelegateGetSearchableItemsHandler searchableItemsHandler);

		[NoTV]
		[iOS (18, 4), Mac (15, 4), MacCatalyst (18, 4)]
		[Export ("searchableItemsDidUpdate:")]
		void DidUpdate (CSSearchableItem [] items);
	}

	delegate void CSSearchableIndexDelegateGetSearchableItemsHandler (CSSearchableItem [] items);

	/// <summary>Describes a searchable index.</summary>
	[NoTV, iOS (27, 0), Mac (27, 0), MacCatalyst (27, 0)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableIndexDescription : NSSecureCoding, NSCopying {
		/// <summary>Gets the file protection class associated with the searchable index.</summary>
		[NullAllowed, Export ("protectionClass")]
		[BindAs (typeof (NSFileProtectionType?))]
		NSString ProtectionClass { get; }
	}

	/// <summary>A uniquely identifiable, searchable object in a <see cref="CoreSpotlight.CSSearchableIndex" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSSearchableItem_Class/index.html">Apple documentation for <c>CSSearchableItem</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableItem : NSSecureCoding, NSCopying {

		/// <summary>Gets the action type for the searchable item.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSSearchableItemActionType")]
		NSString ActionType { get; }

		/// <summary>Gets the key for the item's unique user info dictionary.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSSearchableItemActivityIdentifier")]
		NSString ActivityIdentifier { get; }

		/// <summary>Gets a value that tells whether the continuation action is a search or query.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CSQueryContinuationActionType")]
		NSString ContinuationActionType { get; }

		/// <summary>Gets the user info key for the current query.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("CSSearchQueryString")]
		NSString QueryString { get; }

		/// <param name="uniqueIdentifier">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="domainIdentifier">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="attributeSet">To be added.</param>
		/// <summary>Creates a new CSSearchableItem with the specified values.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithUniqueIdentifier:domainIdentifier:attributeSet:")]
		NativeHandle Constructor ([NullAllowed] string uniqueIdentifier, [NullAllowed] string domainIdentifier, CSSearchableItemAttributeSet attributeSet);

		/// <summary>Gets or sets the value that developers can use to uniquely identify the searchable item within their applications.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("uniqueIdentifier")]
		string UniqueIdentifier { get; set; }

		/// <summary>Gets or sets the optional domain identifier or owner identifier.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("domainIdentifier")]
		string DomainIdentifier { get; set; }

		/// <summary>Gets or sets the searchable item's expiration date.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed] // <- null_resettable
		[Export ("expirationDate")]
		NSDate ExpirationDate { get; set; }

		/// <summary>Gets or sets the dictionary of attributes for the searchable item.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("attributeSet", ArgumentSemantic.Strong)]
		CSSearchableItemAttributeSet AttributeSet { get; set; }

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("compareByRank:")]
		NSComparisonResult CompareByRank (CSSearchableItem other);

		[NoTV]
		[Export ("isUpdate", ArgumentSemantic.Assign)]
		bool IsUpdate { get; set; }

#if !XAMCORE_5_0 && __TVOS__
		[Obsolete ("This property is not available on tvOS.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#endif
		[NoTV, iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("updateListenerOptions", ArgumentSemantic.Assign)]
		CSSearchableItemUpdateListenerOptions UpdateListenerOptions { get; set; }
	}

	/// <summary>Represents a string-like object that returns a locale-specific version of a string.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSLocalizedString_Class/index.html">Apple documentation for <c>CSLocalizedString</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSString))]
	// hack: it seems that generator.cs can't track NSCoding correctly ? maybe because the type is named NSString2 at that time
	interface CSLocalizedString : NSCoding {

		/// <param name="localizedStrings">To be added.</param>
		/// <summary>Creates a new CSLocalizedString with the specified dictionary of locale-specific strings.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithLocalizedStrings:")]
		NativeHandle Constructor (NSDictionary localizedStrings);

		/// <summary>Returns the string for the current locale.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("localizedString")]
		string GetLocalizedString ();
	}

	/// <summary>A key that app developers can associate with metadata for an indexable item that can appear in user search results.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSCustomAttributeKey_Class/index.html">Apple documentation for <c>CSCustomAttributeKey</c></related>
	[NoTV] // CS_TVOS_UNAVAILABLE
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // NSInvalidArgumentException Reason: You must call -[CSCustomAttributeKey initWithKeyName...]
	interface CSCustomAttributeKey : NSCopying, NSSecureCoding {

		/// <param name="keyName">To be added.</param>
		/// <summary>Creates a new CSCustomAttributeKey with the specified name.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithKeyName:")]
		NativeHandle Constructor (string keyName);

		/// <param name="keyName">The attribute key name.</param>
		/// <param name="searchable">Whether the attribute can be used as a search word.</param>
		/// <param name="searchableByDefault">Whether the attribute is searchable by default.</param>
		/// <param name="unique">Whether the attribute should be treated as unique in order to save storage space.</param>
		/// <param name="multiValued">Whether the attribute will likely be associated with arrays, hashes, or other compound values.</param>
		/// <summary>Creates a new CSCustomAttributeKey with the specified values.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithKeyName:searchable:searchableByDefault:unique:multiValued:")]
		NativeHandle Constructor (string keyName, bool searchable, bool searchableByDefault, bool unique, bool multiValued);

		/// <summary>Gets the attribute key name.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("keyName")]
		string KeyName { get; }

		/// <summary>Gets a value that tells whether the attribute can be used as a search word. The default is <see langword="true" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("searchable")]
		bool Searchable { [Bind ("isSearchable")] get; }

		/// <summary>Gets a value that tells whether the attribute is searchable by default. The default is <see langword="false" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("searchableByDefault")]
		bool SearchableByDefault { [Bind ("isSearchableByDefault")] get; }

		/// <summary>Gets a value that tells whether the attribute should be treated as unique in order to save storage space. The default is <see langword="false" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("unique")]
		bool Unique { [Bind ("isUnique")] get; }

		/// <summary>Gets a value that tells whether the attribute will likely be associated with arrays, hashes, or other compound values. The default is <see langword="false" />.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("multiValued")]
		bool MultiValued { [Bind ("isMultiValued")] get; }
	}

	/// <summary>Represents keys that identify commonly used mailboxes.</summary>
	/// <remarks>To be added.</remarks>
#if XAMCORE_5_0
	[NoTV]
#endif
	[MacCatalyst (13, 1)]
#if !XAMCORE_5_0 && __TVOS__
	[Obsolete ("This type is not available on tvOS.")]
	[EditorBrowsable (EditorBrowsableState.Never)]
#else
	[EditorBrowsable (EditorBrowsableState.Advanced)]
#endif
	[Static]
	interface CSMailboxKey {

		/// <summary>The key for the Inbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxInbox")]
		NSString Inbox { get; }

		/// <summary>The key for the Drafts mailbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxDrafts")]
		NSString Drafts { get; }

		/// <summary>The key for the Sent mailbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxSent")]
		NSString Sent { get; }

		/// <summary>The key for the Junk mailbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxJunk")]
		NSString Junk { get; }

		/// <summary>The key for the Trash mailbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxTrash")]
		NSString Trash { get; }

		/// <summary>The key for the Archive mailbox.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("CSMailboxArchive")]
		NSString Archive { get; }
	}

	/// <summary>Holds the actual content to be indexed for search.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/CoreSpotlight/Reference/CSSearchableItemAttributeSet_Class/index.html">Apple documentation for <c>CSSearchableItemAttributeSet</c></related>
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CSSearchableItemAttributeSet : NSCopying, NSSecureCoding {

		/// <param name="itemContentType">To be added.</param>
		/// <summary>Creates a new CSSearchableItemAttributeSet for the specified item content type.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use '.ctor(UTType)' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use '.ctor(UTType)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use '.ctor(UTType)' instead.")]
		[Export ("initWithItemContentType:")]
		NativeHandle Constructor (string itemContentType);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithContentType:")]
		NativeHandle Constructor (UTType contentType);

		// FIXME: Should we keep all the following Categories inline? or should we make them actual [Category] interfaces
		// There are no methods on any of the following categories, just properties

		// CSSearchableItemAttributeSet_Documents.h
		// CSSearchableItemAttributeSet (CSDocuments) Category
		/// <summary>Gets or sets the subject for a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("subject")]
		string Subject { get; set; }

		/// <summary>Gets or sets the theme of a document.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("theme")]
		string Theme { get; set; }

		/// <summary>Gets or sets a description for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentDescription")]
		string ContentDescription { get; set; }

		/// <summary>Gets or sets the identifier for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("identifier")]
		string Identifier { get; set; }

		/// <summary>Gets or sets an intended audience descriptor for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audiences")]
		string [] Audiences { get; set; }

		/// <summary>Gets or sets the size of a file.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("fileSize")]
		NSNumber FileSize { get; set; }

		/// <summary>Gets or sets the number of pages in a document item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pageCount", ArgumentSemantic.Strong)]
		NSNumber PageCount { get; set; }

		/// <summary>Gets or sets the page width.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pageWidth", ArgumentSemantic.Strong)]
		NSNumber PageWidth { get; set; }

		/// <summary>Gets or sets the page height.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pageHeight", ArgumentSemantic.Strong)]
		NSNumber PageHeight { get; set; }

		/// <summary>Gets or sets a description of the encryption method used for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("securityMethod")]
		string SecurityMethod { get; set; }

		/// <summary>The name of the software tool that was used to create an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("creator")]
		string Creator { get; set; }

		/// <summary>Gets or sets the name of the application that encoded content within a PDF stream.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("encodingApplications")]
		string [] EncodingApplications { get; set; }

		/// <summary>Gets or sets a document kind.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("kind")]
		string Kind { get; set; }

		/// <summary>Gets or sets an array that contains the names of all of the fonts used in a document.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("fontNames")]
		string [] FontNames { get; set; }

		// CSSearchableItemAttributeSet_Events.h
		// CSSearchableItemAttributeSet (CSEvents) Category

		/// <summary>Gets or sets the date at which an item is due.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("dueDate", ArgumentSemantic.Strong)]
		NSDate DueDate { get; set; }

		/// <summary>Gets or sets the date on which an item was finished.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("completionDate", ArgumentSemantic.Strong)]
		NSDate CompletionDate { get; set; }

		/// <summary>Gets or sets a starting date for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("startDate", ArgumentSemantic.Strong)]
		NSDate StartDate { get; set; }

		/// <summary>Gets or sets an item's end date.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("endDate", ArgumentSemantic.Strong)]
		NSDate EndDate { get; set; }

		/// <summary>Gets or sets an array of important dates for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("importantDates")]
		NSDate [] ImportantDates { get; set; }

		/// <summary>Gets a value that tells whether an event is an all-day event.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Export ("allDay", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSNumber AllDay { get; set; }

		// CSSearchableItemAttributeSet_General.h
		// CSSearchableItemAttributeSet (CSGeneral) Category

		/// <summary>Gets or sets the display name for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("displayName")]
		string DisplayName { get; set; }

		/// <summary>Gets or sets an array of localized display names for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("alternateNames")]
		string [] AlternateNames { get; set; }

		/// <summary>Gets or sets the absolute path to an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("path")]
		string Path { get; set; }

		/// <summary>Gets or sets the URL of indexable content.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentURL", ArgumentSemantic.Strong)]
		NSUrl ContentUrl { get; set; }

		/// <summary>Gets or sets the URL for the thumbnail image of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("thumbnailURL", ArgumentSemantic.Strong)]
		NSUrl ThumbnailUrl { get; set; }

		/// <summary>Gets or sets data that represent the thumbnail image of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("thumbnailData", ArgumentSemantic.Copy)]
		NSData ThumbnailData { get; set; }

		/// <summary>Gets or sets the list of unique identifiers for items that are related to an activity.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("relatedUniqueIdentifier")]
		string RelatedUniqueIdentifier { get; set; }

		/// <summary>Gets or sets the most recent date that a metadata attribute was changed.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("metadataModificationDate", ArgumentSemantic.Strong)]
		NSDate MetadataModificationDate { get; set; }

		/// <summary>Gets or sets the Uniform Type Identifier (UTI), of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentType")]
		string ContentType { get; set; }

		/// <summary>Gets or sets the hierarchy of types for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentTypeTree")]
		string [] ContentTypeTree { get; set; }

		/// <summary>Gets or sets an array of keywords for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("keywords")]
		string [] Keywords { get; set; }

		/// <summary>Gets or sets the title of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("title")]
		string Title { get; set; }

		/// <summary>Gets or sets the version of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("version")]
		string Version { get; set; }

		/// <summary>Gets or sets an identifier for an item that is related to, but not linked with, an activity.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("weakRelatedUniqueIdentifier", ArgumentSemantic.Copy)]
		string WeakRelatedUniqueIdentifier { get; set; }

		/// <summary>Gets or sets a tag that can be used to group items for deletion.</summary>
		///         <value>
		///           <para>Developers can set this value to <see langword="null" /> to remove the item from all groups.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("domainIdentifier")]
		string DomainIdentifier { get; set; }

		// CSSearchableItemAttributeSet_Images.h
		// CSSearchableItemAttributeSet (CSImages) Category

		/// <summary>Gets the height of a visual media item in pixels.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pixelHeight", ArgumentSemantic.Strong)]
		NSNumber PixelHeight { get; set; }

		/// <summary>Gets the width of a visual media item in pixels.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pixelWidth", ArgumentSemantic.Strong)]
		NSNumber PixelWidth { get; set; }

		/// <summary>Gets or sets the number of pixels in an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("pixelCount", ArgumentSemantic.Strong)]
		NSNumber PixelCount { get; set; }

		/// <summary>Gets or sets a string representation, such as <c>RGB</c>, of a color space model for an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("colorSpace")]
		string ColorSpace { get; set; }

		/// <summary>Gets or sets the bit depth of image or audio items.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("bitsPerSample", ArgumentSemantic.Strong)]
		NSNumber BitsPerSample { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that indicates if flash was used to capture an image.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that a flash was used. <c>0</c> indicates that no flash was used.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("flashOn", ArgumentSemantic.Strong)]
		NSNumber FlashOn { [Bind ("isFlashOn")] get; set; }

		/// <summary>Gets or sets the focal length of the device that was used to capture an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("focalLength", ArgumentSemantic.Strong)]
		NSNumber FocalLength { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that indicates if the focal length of the device that was used to capture an image was 35 mm.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that the focal length was 35mm. <c>0</c> indicates that it was not.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("focalLength35mm", ArgumentSemantic.Strong)]
		NSNumber FocalLength35mm { [Bind ("isFocalLength35mm")] get; set; }

		/// <summary>Gets or sets the manufacturer of the device that captured an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("acquisitionMake")]
		string AcquisitionMake { get; set; }

		/// <summary>Gets or sets the model of the device that captured an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("acquisitionModel")]
		string AcquisitionModel { get; set; }

		/// <summary>Gets or sets the owner of the camera that captured an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("cameraOwner")]
		string CameraOwner { get; set; }

		/// <summary>Gets or sets the lens model on the device that was used to capture an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("lensModel")]
		string LensModel { get; set; }

		/// <summary>Gets or sets the ISO speed that was used to capture an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("ISOSpeed", ArgumentSemantic.Strong)]
		NSNumber IsoSpeed { get; set; }

		/// <summary>Gets or sets a number that represents the orientation of image item data.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates portrait mode. <c>0</c> indicates landscape mode.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("orientation", ArgumentSemantic.Strong)]
		NSNumber Orientation { get; set; }

		/// <summary>Gets or sets a list of strings that represent layer names.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("layerNames")]
		string [] LayerNames { get; set; }

		/// <summary>Gets or sets the white balance of the device that recorded an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("whiteBalance", ArgumentSemantic.Strong)]
		NSNumber WhiteBalance { get; set; }

		/// <summary>Gets or sets the logarithmic APEX value that describes the aperture of the capturing device at the time that an image was captured.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("aperture", ArgumentSemantic.Strong)]
		NSNumber Aperture { get; set; }

		/// <summary>Gets or sets the color profile used in an image media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("profileName")]
		string ProfileName { get; set; }

		/// <summary>Gets or sets the resolution width of an image in DPI.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("resolutionWidthDPI", ArgumentSemantic.Strong)]
		NSNumber ResolutionWidthDpi { get; set; }

		/// <summary>Gets or sets the resolution height of an image in DPI.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("resolutionHeightDPI", ArgumentSemantic.Strong)]
		NSNumber ResolutionHeightDPI { get; set; }

		/// <summary>Gets or sets a value that describes the exposure mode that was used to capture an image.</summary>
		///         <value>
		///           <para>
		///             <c>0</c> indicates that exposure was handled automatically. <c>1</c> indicates that the exposure was handled manually. <c>2</c> indicates that auto-bracketing was used.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("exposureMode", ArgumentSemantic.Strong)]
		NSNumber ExposureMode { get; set; }

		/// <summary>Gets or sets the exposure time that was used to capture an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("exposureTime", ArgumentSemantic.Strong)]
		NSNumber ExposureTime { get; set; }

		/// <summary>Gets or sets the EXIF version for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("EXIFVersion")]
		string ExifVersion { get; set; }

		/// <summary>Gets or sets the EXIF version for a media item's GPS metadata.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("EXIFGPSVersion")]
		string ExifGpsVersion { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that indicates if the color representation used by an image has an alpha channel.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that an alpha channel is present. <c>0</c> indicates that an alpha channel is not present.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("hasAlphaChannel", ArgumentSemantic.Strong)]
		NSNumber HasAlphaChannel { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that tells whether red-eye reduction was used to capture an image.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that red-eye reduction was used. <c>0</c> indicates that red-eye reduction was not used.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("redEyeOn", ArgumentSemantic.Strong)]
		NSNumber RedEyeOn { [Bind ("isRedEyeOn")] get; set; }

		/// <summary>Gets or sets the metering mode that was used on the device to capture an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("meteringMode")]
		string MeteringMode { get; set; }

		/// <summary>Gets or sets the smallest F number for a lens.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("maxAperture", ArgumentSemantic.Strong)]
		NSNumber MaxAperture { get; set; }

		/// <summary>Gets or sets the focal ratio that was used when capturing an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("fNumber", ArgumentSemantic.Strong)]
		NSNumber FNumber { get; set; }

		/// <summary>Gets or sets a string that describes the exposure program that was used to capture an image.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("exposureProgram")]
		string ExposureProgram { get; set; }

		/// <summary>Gets or sets a string description of the exposure time that was used to capture an image item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("exposureTimeString")]
		string ExposureTimeString { get; set; }

		// CSSearchableItemAttributeSet_Media.h
		// CSSearchableItemAttributeSet (CSMedia) Category

		/// <summary>Gets or sets a list of the names of the editors who have contributed to the file.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("editors")]
		string [] Editors { get; set; }

		/// <summary>Gets or sets a list of people represented in an image or document.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("participants")]
		string [] Participants { get; set; }

		/// <summary>Gets or sets a list of projects to which a file belongs.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("projects")]
		string [] Projects { get; set; }

		/// <summary>Gets or sets the date on which an item was downloaded.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("downloadedDate", ArgumentSemantic.Strong)]
		NSDate DownloadedDate { get; set; }

		/// <summary>Gets or sets an array of URLs, email subject lines, ore etc., that represent the sources from which a media item was obtained.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentSources")]
		string [] ContentSources { get; set; }

		/// <summary>Gets or sets a comment for a media file.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("comment")]
		string Comment { get; set; }

		/// <summary>Gets or sets the date that a media item was copyrighted.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("copyright")]
		string Copyright { get; set; }

		/// <summary>Gets or sets the most recent date on which an item was used.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("lastUsedDate", ArgumentSemantic.Strong)]
		NSDate LastUsedDate { get; set; }

		/// <summary>Gets or sets the creation date of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentCreationDate", ArgumentSemantic.Strong)]
		NSDate ContentCreationDate { get; set; }

		/// <summary>Gets or sets the date when a content item was most recently modified.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentModificationDate", ArgumentSemantic.Strong)]
		NSDate ContentModificationDate { get; set; }

		/// <summary>Gets or sets the date that a media item was added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("addedDate", ArgumentSemantic.Strong)]
		NSDate AddedDate { get; set; }

		/// <summary>Gets or sets the length of time that a playable content item plays.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("duration", ArgumentSemantic.Strong)]
		NSNumber Duration { get; set; }

		/// <summary>Gets or sets the list of the names of contacts who are associated with an item, but who are not the authors of the item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contactKeywords")]
		string [] ContactKeywords { get; set; }

		/// <summary>Gets or sets the codecs that were used to encode a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("codecs")]
		string [] Codecs { get; set; }

		/// <summary>Gets or sets an array of strings that represent the media types that are contained in a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("mediaTypes")]
		string [] MediaTypes { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that tells whether a media item is streamable.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that the item can be streamed. <c>0</c> indicates that the item cannot be streamed.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("streamable", ArgumentSemantic.Strong)]
		NSNumber Streamable { [Bind ("isStreamable")] get; set; }

		/// <summary>Gets or sets the combined audio and visual bitrate of a video media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("totalBitRate", ArgumentSemantic.Strong)]
		NSNumber TotalBitRate { get; set; }

		/// <summary>Gets or set the video bitrate of a video media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("videoBitRate", ArgumentSemantic.Strong)]
		NSNumber VideoBitRate { get; set; }

		/// <summary>Gets or sets the bitrate of the audio portion of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audioBitRate", ArgumentSemantic.Strong)]
		NSNumber AudioBitRate { get; set; }

		/// <summary>Gets or sets a value that is <c>0</c> to represent "fast start" and <c>1</c> to represent the Real Time Streaming Protocol.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("deliveryType", ArgumentSemantic.Strong)]
		NSNumber DeliveryType { get; set; }

		/// <summary>Gets or sets a list of contributing organizations for an item's content.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("organizations")]
		string [] Organizations { get; set; }

		/// <summary>Gets or sets the role associated with the creator of content.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("role")]
		string Role { get; set; }

		/// <summary>Gets or sets a list of languages for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("languages")]
		string [] Languages { get; set; }

		/// <summary>Gets or sets a string that represents rights information for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("rights")]
		string Rights { get; set; }

		/// <summary>Gets or sets a list of publishers for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("publishers")]
		string [] Publishers { get; set; }

		/// <summary>Gets or sets a list of contributors to the production of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contributors")]
		string [] Contributors { get; set; }

		/// <summary>Gets or sets an arbitrary string that is intended to represent the geographica or temporal scope of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("coverage")]
		string [] Coverage { get; set; }

		/// <summary>Gets or sets a user-supplied media item rating.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("rating", ArgumentSemantic.Strong)]
		NSNumber Rating { get; set; }

		/// <summary>Gets or sets the rating description.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("ratingDescription")]
		NSNumber RatingDescription { get; set; }

		/// <summary>Gets or sets the play count for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("playCount", ArgumentSemantic.Strong)]
		NSNumber PlayCount { get; set; }

		/// <summary>Gets or sets an information string.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("information")]
		string Information { get; set; }

		/// <summary>Gets or sets the director of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("director")]
		string Director { get; set; }

		/// <summary>Gets or sets the producer for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("producer")]
		string Producer { get; set; }

		/// <summary>Gets or sets a string that describes the genre of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("genre")]
		string Genre { get; set; }

		/// <summary>Gets or sets a list of performers in a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("performers")]
		string [] Performers { get; set; }

		/// <summary>Gets or sets a description of the original format of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("originalFormat")]
		string OriginalFormat { get; set; }

		/// <summary>Gets or sets a description of the original source of a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("originalSource")]
		string OriginalSource { get; set; }

		/// <summary>Gets or sets a value with Boolean semantics that tells whether a media item is local.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that the media item is local. <c>0</c> indicates that the item remote.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("local", ArgumentSemantic.Strong)]
		NSNumber Local { [Bind ("isLocal")] get; set; }

		/// <summary>Gets or sets a value that represents whether a media item is explicit.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("contentRating", ArgumentSemantic.Strong)]
		NSNumber ContentRating { get; set; }

		/// <summary>Gets or sets the url for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("URL", ArgumentSemantic.Strong)]
		NSUrl Url { get; set; }

		// CSSearchableItemAttributeSet_Media.h
		// CSSearchableItemAttributeSet (CSMusic) Category

		/// <summary>Gets or sets the frequency at which audio data was sampled when it was recorded, in Hz.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audioSampleRate", ArgumentSemantic.Strong)]
		NSNumber AudioSampleRate { get; set; }

		/// <summary>Gets or sets the number of channels in an audio data file.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audioChannelCount", ArgumentSemantic.Strong)]
		NSNumber AudioChannelCount { get; set; }

		/// <summary>Gets or sets the tempo of a musical audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("tempo", ArgumentSemantic.Strong)]
		NSNumber Tempo { get; set; }

		/// <summary>Gets or sets a string representation of the key signature of a musical piece.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("keySignature")]
		string KeySignature { get; set; }

		/// <summary>Gets or sets the time signature of a musical audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("timeSignature")]
		string TimeSignature { get; set; }

		/// <summary>Gets or sets the name of the application that encoded the audio data in a file.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audioEncodingApplication")]
		string AudioEncodingApplication { get; set; }

		/// <summary>Gets or sets the composer for an audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("composer")]
		string Composer { get; set; }

		/// <summary>Gets or sets the lyricist for a song.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("lyricist")]
		string Lyricist { get; set; }

		/// <summary>Gets or sets the album for audio items.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("album")]
		string Album { get; set; }

		/// <summary>Gets or sets the artist for a media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("artist")]
		string Artist { get; set; }

		/// <summary>Gets or sets the track number of an audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("audioTrackNumber", ArgumentSemantic.Strong)]
		NSNumber AudioTrackNumber { get; set; }

		/// <summary>Gets or sets the date on which a musical media item was recorded.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("recordingDate", ArgumentSemantic.Strong)]
		NSDate RecordingDate { get; set; }

		/// <summary>Gets or sets the genre of an audio media item that contains music.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("musicalGenre")]
		string MusicalGenre { get; set; }

		/// <summary>Gets or sets a number with Boolean semantics that indicates whether the MIDI sequence in a file is set up for use with general MIDI devices.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that the sequence is set up for general MIDI devices. <c>0</c> indicates that it is not.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("generalMIDISequence", ArgumentSemantic.Strong)]
		NSNumber GeneralMidiSequence { [Bind ("isGeneralMIDISequence")] get; set; }

		/// <summary>Gets or sets a string that represents the category of musical instrument in an audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("musicalInstrumentCategory")]
		string MusicalInstrumentCategory { get; set; }

		/// <summary>Gets or sets a string that represents the name of the musical instrument in an audio media item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("musicalInstrumentName")]
		string MusicalInstrumentName { get; set; }

		// CSSearchableItemAttributeSet_Messaging.h
		// CSSearchableItemAttributeSet (CSMessaging) Category

		/// <summary>Gets or sets the account identifier for a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("accountIdentifier")]
		string AccountIdentifier { get; set; }

		/// <summary>Gets or sets an array of account handles that a message is associated with.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("accountHandles")]
		string [] AccountHandles { get; set; }

		/// <summary>Gets or sets the UTF-8 string representation of the HTML contents of a document.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("HTMLContentData", ArgumentSemantic.Copy)]
		NSData HtmlContentData { get; set; }

		/// <summary>Gets or sets the text content of a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("textContent")]
		string TextContent { get; set; }

		/// <summary>Gets or sets an array of <see cref="CoreSpotlight.CSPerson" /> objects that represent the authors of a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("authors", ArgumentSemantic.Copy)]
		CSPerson [] Authors { get; set; }

		/// <summary>Gets or sets an array of <see cref="CoreSpotlight.CSPerson" /> objects that represent the recipients on the To: line of an email.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("primaryRecipients", ArgumentSemantic.Copy)]
		CSPerson [] PrimaryRecipients { get; set; }

		/// <summary>Gets or sets an array of <see cref="CoreSpotlight.CSPerson" /> objects that comprise the recipients on the Cc: field.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("additionalRecipients", ArgumentSemantic.Copy)]
		CSPerson [] AdditionalRecipients { get; set; }

		/// <summary>Gets or sets an array of <see cref="CoreSpotlight.CSPerson" /> objects that represent the recipients on the Bcc: line of an email.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("hiddenAdditionalRecipients", ArgumentSemantic.Copy)]
		CSPerson [] HiddenAdditionalRecipients { get; set; }

		/// <summary>Gets or sets a dictionary that contains the headers for an email message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("emailHeaders", ArgumentSemantic.Copy)]
		NSDictionary EmailHeaders { get; set; }

		/// <summary>Gets or sets a list of mailbox identifiers for a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("mailboxIdentifiers")]
		string [] MailboxIdentifiers { get; set; }

		/// <summary>Gets or sets the list of names of the people who have authored a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("authorNames")]
		string [] AuthorNames { get; set; }

		/// <summary>Gets or sets a list of recipient names associated with a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("recipientNames")]
		string [] RecipientNames { get; set; }

		/// <summary>Gets or sets an array of author email addresses.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("authorEmailAddresses")]
		string [] AuthorEmailAddresses { get; set; }

		/// <summary>Gets or sets a list of recipient email addresses associated with a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("recipientEmailAddresses")]
		string [] RecipientEmailAddresses { get; set; }

		/// <summary>Gets or sets an array of author addresses.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("authorAddresses")]
		string [] AuthorAddresses { get; set; }

		/// <summary>Gets or sets a list of recipient addresses associated with a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("recipientAddresses")]
		string [] RecipientAddresses { get; set; }

		/// <summary>Gets or sets an array of phone numbers related to a message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("phoneNumbers")]
		string [] PhoneNumbers { get; set; }

		/// <summary>Gets or sets a list of email messages that are associated with a file's author.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("emailAddresses")]
		string [] EmailAddresses { get; set; }

		/// <summary>Gets or sets an array of instant message addresses for an instant message.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("instantMessageAddresses")]
		string [] InstantMessageAddresses { get; set; }

		/// <summary>Gets or sets a value with Boolean semantics that tells whether a message is likely to be considered junk.</summary>
		///         <value>
		///           <para>
		///             <c>1</c> indicates that the item is likely junk. <c>0</c> indicates that the item is not likely junk.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Export ("likelyJunk", ArgumentSemantic.Strong)]
		NSNumber LikelyJunk { [Bind ("isLikelyJunk")] get; set; }

		// CSSearchableItemAttributeSet_Places.h
		// CSSearchableItemAttributeSet (CSPlaces) Category

		/// <summary>Gets or sets a brief description of an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("headline")]
		string Headline { get; set; }

		/// <summary>Gets or sets a string that contains instructions for an item. E.g., a warning or note.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("instructions")]
		string Instructions { get; set; }

		/// <summary>Gets or sets the city of an item's origin.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("city")]
		string City { get; set; }

		/// <summary>Gets or sets the state or province information for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("stateOrProvince")]
		string StateOrProvince { get; set; }

		/// <summary>Gets or sets the name of the country/region where a media item was created.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("country")]
		string Country { get; set; }

		/// <summary>Gets or sets the altitude, in meters, above the WGS84 datum sea level.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("altitude", ArgumentSemantic.Strong)]
		NSNumber Altitude { get; set; }

		/// <summary>Gets or sets the north latitude for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("latitude", ArgumentSemantic.Strong)]
		NSNumber Latitude { get; set; }

		/// <summary>Gets or sets the east longitude for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("longitude", ArgumentSemantic.Strong)]
		NSNumber Longitude { get; set; }

		/// <summary>Gets or sets a speed value in kilometers/hour.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("speed", ArgumentSemantic.Strong)]
		NSNumber Speed { get; set; }

		/// <summary>Gets or sets the timestamp for an item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("timestamp", ArgumentSemantic.Strong)]
		NSDate Timestamp { get; set; }

		/// <summary>Gets or sets direction in which an image was taken.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("imageDirection", ArgumentSemantic.Strong)]
		NSNumber ImageDirection { get; set; }

		/// <summary>Gets or sets the possibly user-specified location name of a point of interest.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("namedLocation")]
		string NamedLocation { get; set; }

		/// <summary>Gets or sets the direction of travel as a number of degrees clockwise from true north.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSTrack", ArgumentSemantic.Strong)]
		NSNumber GpsTrack { get; set; }

		/// <summary>Gets or sets the GPS receiver's status.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSStatus")]
		string GpsStatus { get; set; }

		/// <summary>Gets or sets the measurement precision mode for the GPS receiver.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSMeasureMode")]
		string GpsMeasureMode { get; set; }

		/// <summary>Gets or sets the dilution of precision for GPS data.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDOP", ArgumentSemantic.Strong)]
		NSNumber GpsDop { get; set; }

		/// <summary>Gets or sets the coordinate system and reference points that are used in conjunction with GPS data.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSMapDatum")]
		string GpsMapDatum { get; set; }

		/// <summary>Gets or sets the latitude of a GPS destination.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDestLatitude", ArgumentSemantic.Strong)]
		NSNumber GpsDestLatitude { get; set; }

		/// <summary>Gets or sets the longitude of a GPS destination.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDestLongitude", ArgumentSemantic.Strong)]
		NSNumber GpsDestLongitude { get; set; }

		/// <summary>Gets or sets the bearing to a destination.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDestBearing", ArgumentSemantic.Strong)]
		NSNumber GpsDestBearing { get; set; }

		/// <summary>Gets or sets the distance to a destination.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDestDistance", ArgumentSemantic.Strong)]
		NSNumber GpsDestDistance { get; set; }

		/// <summary>Gets or sets the location processing method for the GPS receiver.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSProcessingMethod")]
		string GpsProcessingMethod { get; set; }

		/// <summary>Gets or sets GPS area information for a place item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSAreaInformation")]
		string GpsAreaInformation { get; set; }

		/// <summary>Gets or sets the GPS date stamp.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDateStamp", ArgumentSemantic.Strong)]
		NSDate GpsDateStamp { get; set; }

		/// <summary>Gets or sets a differential correction to GPS.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("GPSDifferental", ArgumentSemantic.Strong)]
#if XAMCORE_5_0
		NSNumber GpsDifferential { get; set; }
#else
		[Obsolete ("Use 'GpsDifferential' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		NSNumber GpsDifferental { get; set; }

		[Wrap ("GpsDifferental")]
		NSNumber GpsDifferential { get; set; }
#endif

		/// <summary>Gets or sets the fully formatted geographic address of the item.</summary>
		///         <value>
		///           <para>Map Kit provides this address.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fullyFormattedAddress")]
		string FullyFormattedAddress { get; set; }

		/// <summary>Gets or sets the postal code for the item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("postalCode")]
		string PostalCode { get; set; }

		/// <summary>Gets or sets a secondary street component, such as a house number.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("subThoroughfare")]
		string SubThoroughfare { get; set; }

		/// <summary>Gets or sets the street name for the item.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("thoroughfare")]
		string Thoroughfare { get; set; }

		// CSActionExtras

		/// <summary>Gets or sets a value that tells whether the number associated with an item can be called.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("supportsPhoneCall", ArgumentSemantic.Strong)]
		NSNumber SupportsPhoneCall { get; set; }

		/// <summary>Gets or sets a value that tells whether a location item can be navigated to.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("supportsNavigation", ArgumentSemantic.Strong)]
		NSNumber SupportsNavigation { get; set; }

		[NoTV, NoMac, iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("CSActionIdentifier")]
		NSString ActionIdentifier { get; }

		[NoTV, NoMac, iOS (15, 0), MacCatalyst (18, 0)]
		[Export ("actionIdentifiers", ArgumentSemantic.Copy)]
		string [] ActionIdentifiers { get; set; }

		[NullAllowed]
		[NoTV, NoMac, iOS (15, 0), MacCatalyst (18, 0)]
		[Export ("sharedItemContentType", ArgumentSemantic.Copy)]
		UTType SharedItemContentType { get; set; }

		// CSContainment

		/// <summary>Gets or sets the title of the item's container.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("containerTitle")]
		string ContainerTitle { get; set; }

		/// <summary>Gets or sets the localized container display name.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("containerDisplayName")]
		string ContainerDisplayName { get; set; }

		/// <summary>Gets or sets the identifier of the item's container.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("containerIdentifier")]
		string ContainerIdentifier { get; set; }

		/// <summary>Gets or sets the order of the item within its container.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("containerOrder", ArgumentSemantic.Strong)]
		NSNumber ContainerOrder { get; set; }

		// CSCustomAttributes

		[Internal]
		[Export ("setValue:forCustomKey:")]
		void SetValue ([NullAllowed] INSSecureCoding value, CSCustomAttributeKey key);

		[Internal]
		[Export ("valueForCustomKey:")]
		[return: NullAllowed]
		INSSecureCoding ValueForCustomKey (CSCustomAttributeKey key);

		// CSSearchableItemAttributeSet_CSGeneral

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("userCreated", ArgumentSemantic.Strong)]
		[Internal] // We would like to use [BindAs (typeof (bool?))]
		NSNumber _IsUserCreated { [Bind ("isUserCreated")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("userOwned", ArgumentSemantic.Strong)]
		[Internal] // We would like to use[BindAs (typeof (bool?))]
		NSNumber _IsUserOwned { [Bind ("isUserOwned")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("userCurated", ArgumentSemantic.Strong)]
		[Internal] // We would like to use [BindAs (typeof (bool?))]
		NSNumber _IsUserCurated { [Bind ("isUserCurated")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("rankingHint", ArgumentSemantic.Strong)]
		NSNumber RankingHint { get; set; }

		[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("darkThumbnailURL", ArgumentSemantic.Strong)]
		NSUrl DarkThumbnailUrl { get; set; }

		// CSSearchableItemAttributeSet_CSItemProvider

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("providerDataTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] ProviderDataTypeIdentifiers { get; set; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("providerFileTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] ProviderFileTypeIdentifiers { get; set; }

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("providerInPlaceFileTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] ProviderInPlaceFileTypeIdentifiers { get; set; }

		[NoTV]
		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("moveFrom:")]
		void MoveFrom (CSSearchableItemAttributeSet sourceAttributeSet);

		[NoTV]
		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("isPriority", ArgumentSemantic.Strong), NullAllowed]
		NSNumber IsPriority { get; }

		[NoTV]
		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("textContentSummary", ArgumentSemantic.Copy), NullAllowed]
		string TextContentSummary { get; }

		[NoTV]
		[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("transcribedTextContent", ArgumentSemantic.Strong), NullAllowed]
		string TranscribedTextContent { get; set; }
	}

	/// <include file="../docs/api/CoreSpotlight/CSSearchQuery.xml" path="/Documentation/Docs[@DocId='T:CoreSpotlight.CSSearchQuery']/*" />
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CSSearchQuery {
		/// <param name="queryString">To be added.</param>
		/// <param name="attributes">
		///           <para>A list of strings from <see cref="CoreSpotlight.CSSearchableItemAttributeSet" /> to match.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>Creates a new search query object for the specified query string and attributes.</summary>
		/// <remarks>
		///           <para>For more information on the query string format, see Apple's documentation for the <format type="text/html"><a href="https://developer.apple.com/reference/CoreSpotlight/CSSearchQuery">CSSearchQuery object</a></format>.</para>
		///         </remarks>
		[Deprecated (PlatformName.iOS, 16, 0, message: "Use the constructor that takes a 'CSSearchQueryContext' parameter instead.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 0, message: "Use the constructor that takes a 'CSSearchQueryContext' parameter instead.")]
		[Deprecated (PlatformName.MacOSX, 13, 0, message: "Use the constructor that takes a 'CSSearchQueryContext' parameter instead.")]
		[Export ("initWithQueryString:attributes:")]
		NativeHandle Constructor (string queryString, [NullAllowed] string [] attributes);

		[iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithQueryString:queryContext:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string queryString, [NullAllowed] CSSearchQueryContext queryContext);

		/// <summary>Gets a value that tells whether the query has been canceled.</summary>
		///         <value>A value that tells whether the query has been canceled.</value>
		///         <remarks>To be added.</remarks>
		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }

		/// <summary>Gets the current number of found items.</summary>
		///         <value>The current number of found items.</value>
		///         <remarks>This number will grow as more search results become ready.</remarks>
		[Export ("foundItemCount")]
		nuint FoundItemCount { get; }

		/// <summary>Gets or sets a handler that, if it is not <see langword="null" />, is run when a batch of search results is ready.</summary>
		///         <value>
		///           <para>A a handler that, if it is not <see langword="null" />, is run when a batch of search results is ready.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("foundItemsHandler", ArgumentSemantic.Copy)]
		Action<CSSearchableItem []> FoundItemsHandler { get; set; }

		/// <summary>Gets or sets a handler that, if it is not <see langword="null" />, is run when the search completes.</summary>
		///         <value>
		///           <para>A  handler that, if supplied, is run when the search completes.</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NullAllowed, Export ("completionHandler", ArgumentSemantic.Copy)]
		Action<NSError> CompletionHandler { get; set; }

		/// <summary>Gets or sets an array of protection classes for the indexed data.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("protectionClasses", ArgumentSemantic.Copy)]
		string [] ProtectionClasses { get; set; }

		/// <summary>Starts the search.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("start")]
		void Start ();

		/// <summary>Cancels the current search and calls <see cref="CompletionHandler" />, if present, with <see cref="CoreSpotlight.CSSearchQueryErrorCode.Cancelled" />.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("cancel")]
		void Cancel ();
	}

	[Abstract]
	[NoTV, iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface CSImportExtension : NSExtensionRequestHandling {
		[Export ("updateAttributes:forFileAtURL:error:")]
		bool Update (CSSearchableItemAttributeSet attributes, NSUrl contentUrl, [NullAllowed] out NSError error);
	}

	[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (CSSearchQuery))]
	[DisableDefaultCtor]
	interface CSUserQuery {
		[Export ("initWithUserQueryString:userQueryContext:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string userQueryString, [NullAllowed] CSUserQueryContext userQueryContext);

		[Export ("foundSuggestionCount")]
		nint FoundSuggestionCount { get; }

		[NullAllowed, Export ("foundSuggestionsHandler", ArgumentSemantic.Copy)]
		Action<NSArray<CSSuggestion>> FoundSuggestionsHandler { get; set; }

		[Export ("start")]
		void Start ();

		[Export ("cancel")]
		void Cancel ();

		[Static]
		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prepare")]
		void Prepare ();

		[Static]
		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prepareProtectionClasses:")]
		void Prepare (NSString [] protectionClasses);

		[Static]
		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Wrap ("Prepare (protectionClasses.ToConstantArray ()!)")]
		void Prepare (NSFileProtectionType [] protectionClasses);

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("userEngagedWithItem:visibleItems:userInteractionType:")]
		void UserEngaged (CSSearchableItem item, CSSearchableItem [] visibleItems, CSUserInteraction userInteractionType);

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("userEngagedWithSuggestion:visibleSuggestions:userInteractionType:")]
		void UserEngaged (CSSuggestion suggestion, CSSuggestion [] visibleSuggestions, CSUserInteraction userInteractionType);
	}

	[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (CSSearchQueryContext))]
	[DisableDefaultCtor]
	interface CSUserQueryContext {
		[Static]
		[Export ("userQueryContext")]
		CSUserQueryContext UserQueryContext { get; }

		[Static]
		[Export ("userQueryContextWithCurrentSuggestion:")]
		CSUserQueryContext Create ([NullAllowed] CSSuggestion currentSuggestion);

		[Export ("maxSuggestionCount")]
		nint MaxSuggestionCount { get; set; }

		[Export ("enableRankedResults")]
		bool EnableRankedResults { get; set; }

		[Export ("maxResultCount")]
		nint MaxResultCount { get; set; }

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("disableSemanticSearch")]
		bool DisableSemanticSearch { get; set; }

		[Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("maxRankedResultCount")]
		nint MaxRankedResultCount { get; set; }
	}


	[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CSSuggestion : NSSecureCoding, NSCopying {

		[Field ("CSSuggestionHighlightAttributeName")]
		NSString HighlightAttributeName { get; }

		[Export ("localizedAttributedSuggestion")]
		NSAttributedString LocalizedAttributedSuggestion { get; }

		[Export ("suggestionKind")]
		CSSuggestionKind SuggestionKind { get; }

		[Export ("compare:")]
		NSComparisonResult Compare (CSSuggestion other);

		[Export ("compareByRank:")]
		NSComparisonResult CompareByRank (CSSuggestion other);

		[iOS (17, 0), MacCatalyst (17, 0), NoTV]
		[Export ("score")]
		NSNumber Score { get; }

		[iOS (17, 0), MacCatalyst (17, 0), NoTV]
		[Export ("suggestionDataSources")]
		NSObject [] SuggestionDataSources { get; }
	}

	[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface CSSearchQueryContext : NSSecureCoding, NSCopying {
		[Export ("fetchAttributes", ArgumentSemantic.Strong)]
		string [] FetchAttributes { get; set; }

		[Export ("filterQueries", ArgumentSemantic.Copy)]
		string [] FilterQueries { get; set; }

		[NullAllowed, Export ("keyboardLanguage", ArgumentSemantic.Strong)]
		string KeyboardLanguage { get; set; }

#if XAMCORE_5_0
		/// <summary>Gets or sets the sources that the query can search.</summary>
		[NoTV, NoiOS, NoMacCatalyst]
#elif !__MACOS__
		/// <summary>Gets or sets the sources that the query can search.</summary>
		[Obsolete ("This property is not available on this platform.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#else
		/// <summary>Gets or sets the sources that the query can search.</summary>
#endif
		[Export ("sourceOptions", ArgumentSemantic.Assign)]
		CSSearchQuerySourceOptions SourceOptions { get; set; }
	}

#if XAMCORE_5_0
	/// <summary>Specifies the sources that a Core Spotlight query can search.</summary>
	[NoTV, NoiOS, NoMacCatalyst]
#else
#if !__MACOS__
	/// <summary>Specifies the sources that a Core Spotlight query can search.</summary>
	[Obsolete ("This enum is not available on this platform.")]
	[EditorBrowsable (EditorBrowsableState.Never)]
#else
	/// <summary>Specifies the sources that a Core Spotlight query can search.</summary>
#endif
	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
#endif
	[Flags]
	[Native]
#if XAMCORE_5_0
	public enum CSSearchQuerySourceOptions : ulong {
#else
	public enum CSSearchQuerySourceOptions : long {
#endif
		Default = 0,
		AllowMail = 1 << 0,
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	[Native]
	public enum CSSuggestionKind : long {
		None,
		Custom,
		Default,
	}

#if XAMCORE_5_0
	[NoTV]
#else
#if __TVOS__
	[Obsolete ("This enum is not available on tvOS.")]
	[EditorBrowsable (EditorBrowsableState.Never)]
#endif
	[TV (18, 4)]
#endif
	[Flags]
	[Native]
	[iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
	public enum CSSearchableItemUpdateListenerOptions : ulong {
		Default = 0,
		Summarization = 1 << 1,
		Priority = 1 << 2,
	}
}
