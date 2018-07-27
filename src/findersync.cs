using System;
using Foundation;
using ObjCRuntime;
using AppKit;

#if XAMCORE_2_0
namespace FinderSync {
	delegate void GetValuesCompletionHandler (NSDictionary<NSString, NSObject> values, NSError error);

	[Mac (10, 10, onlyOn64: true)]
	[BaseType (typeof(NSExtensionContext))]
	interface FIFinderSyncController : NSSecureCoding, NSCopying
	{
		[Static]
		[Export("defaultController")]
		FIFinderSyncController DefaultController { get; }

		[Export ("directoryURLs", ArgumentSemantic.Copy)]
		NSSet DirectoryUrls { get; set; }

		[Export ("setBadgeImage:label:forBadgeIdentifier:")]
		void SetBadgeImage (NSImage image, [NullAllowed] string label, string badgeID);

		[Export ("setBadgeIdentifier:forURL:")]
		void SetBadgeIdentifier (string badgeID, NSUrl url);

		[NullAllowed, Export ("targetedURL")]
		NSUrl TargetedURL { get; }

		[NullAllowed, Export ("selectedItemURLs")]
		NSUrl[] SelectedItemURLs { get; }

		[Mac (10,13, onlyOn64 : true)]
		[Export ("lastUsedDateForItemWithURL:")]
		[return: NullAllowed]
		NSDate GetLastUsedDate (NSUrl itemUrl);

		[Mac (10,13, onlyOn64 : true)]
		[Async, Export ("setLastUsedDate:forItemWithURL:completion:")]
		void SetLastUsedDate (NSDate lastUsedDate, NSUrl itemUrl, Action<NSError> completion);

		[Mac (10,13, onlyOn64 : true)]
		[Export ("tagDataForItemWithURL:")]
		[return: NullAllowed]
		NSData GetTagData (NSUrl itemUrl);

		[Async, Mac (10,13, onlyOn64 : true)]
		[Export ("setTagData:forItemWithURL:completion:")]
		void SetTagData ([NullAllowed] NSData tagData, NSUrl itemUrl, Action<NSError> completion);

		[Mac (10, 14, onlyOn64: true)]
		[Static]
		[Export ("extensionEnabled")]
		bool ExtensionEnabled { [Bind ("isExtensionEnabled")] get; }

		[Mac (10,14, onlyOn64: true)]
		[Static]
		[Export ("showExtensionManagementInterface")]
		void ShowExtensionManagementInterface ();
	}

	[Mac (10, 10, onlyOn64: true)]
	[Protocol (Name = "FIFinderSync")]
	interface FIFinderSyncProtocol
	{
		[Export ("menuForMenuKind:")]
		[return: NullAllowed]
		NSMenu GetMenu (FIMenuKind menuKind);

		[Export ("beginObservingDirectoryAtURL:")]
		void BeginObservingDirectory (NSUrl url);

		[Export ("endObservingDirectoryAtURL:")]
		void EndObservingDirectory (NSUrl url);

		[Export ("requestBadgeIdentifierForURL:")]
		void RequestBadgeIdentifier (NSUrl url);

		[Export ("toolbarItemName")]
		string ToolbarItemName { get; }

		[Export ("toolbarItemImage", ArgumentSemantic.Copy)]
		NSImage ToolbarItemImage { get; }

		[Export ("toolbarItemToolTip")]
		string ToolbarItemToolTip { get; }

		[Mac (10,13, onlyOn64 : true)]
		[Export ("supportedServiceNamesForItemWithURL:")]
		string[] SupportedServiceNames (NSUrl itemUrl);

#if FALSE // TODO: Activate after 10.13 foundation APIs have been merged.  Bug 57800
		[Mac (10,13, onlyOn64 : true)]
		[Export ("makeListenerEndpointForServiceName:andReturnError:")]
		[return: NullAllowed]
		NSXpcListenerEndpoint MakeListenerEndpoint (string serviceName, [NullAllowed] out NSError error);
#endif
		[Mac (10,13, onlyOn64 : true)]
		[Async, Export ("valuesForAttributes:forItemWithURL:completion:")]
		void GetValues (string[] attributes, NSUrl itemUrl, GetValuesCompletionHandler completion);
	}

	[Mac (10, 10, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface FIFinderSync : NSExtensionRequestHandling, FIFinderSyncProtocol
	{
	}
}
#endif