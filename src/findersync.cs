using System;
using Foundation;
using ObjCRuntime;
using AppKit;

namespace FinderSync {
	delegate void GetValuesCompletionHandler (NSDictionary<NSString, NSObject> values, NSError error);

	[BaseType (typeof (NSExtensionContext))]
	interface FIFinderSyncController : NSSecureCoding, NSCopying {
		[Static]
		[Export ("defaultController")]
		FIFinderSyncController DefaultController { get; }

		[NullAllowed] // null_resettable
		[Export ("directoryURLs", ArgumentSemantic.Copy)]
		NSSet DirectoryUrls { get; set; }

		[Export ("setBadgeImage:label:forBadgeIdentifier:")]
		void SetBadgeImage (NSImage image, [NullAllowed] string label, string badgeID);

		[Export ("setBadgeIdentifier:forURL:")]
		void SetBadgeIdentifier (string badgeID, NSUrl url);

		[NullAllowed, Export ("targetedURL")]
		NSUrl TargetedURL { get; }

		[NullAllowed, Export ("selectedItemURLs")]
		NSUrl [] SelectedItemURLs { get; }

		[Export ("lastUsedDateForItemWithURL:")]
		[return: NullAllowed]
		NSDate GetLastUsedDate (NSUrl itemUrl);

		[Async, Export ("setLastUsedDate:forItemWithURL:completion:")]
		void SetLastUsedDate (NSDate lastUsedDate, NSUrl itemUrl, Action<NSError> completion);

		[Export ("tagDataForItemWithURL:")]
		[return: NullAllowed]
		NSData GetTagData (NSUrl itemUrl);

		[Async]
		[Export ("setTagData:forItemWithURL:completion:")]
		void SetTagData ([NullAllowed] NSData tagData, NSUrl itemUrl, Action<NSError> completion);

		[Static]
		[Export ("extensionEnabled")]
		bool ExtensionEnabled { [Bind ("isExtensionEnabled")] get; }

		[Static]
		[Export ("showExtensionManagementInterface")]
		void ShowExtensionManagementInterface ();
	}

	[Protocol (Name = "FIFinderSync")]
	interface FIFinderSyncProtocol {
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

		[Export ("supportedServiceNamesForItemWithURL:")]
		string [] SupportedServiceNames (NSUrl itemUrl);

#if FALSE // TODO: Activate after 10.13 foundation APIs have been merged.  Bug 57800
		[Export ("makeListenerEndpointForServiceName:andReturnError:")]
		[return: NullAllowed]
		NSXpcListenerEndpoint MakeListenerEndpoint (string serviceName, [NullAllowed] out NSError error);
#endif
		[Async, Export ("valuesForAttributes:forItemWithURL:completion:")]
		void GetValues (string [] attributes, NSUrl itemUrl, GetValuesCompletionHandler completion);
	}

	[BaseType (typeof (NSObject))]
	interface FIFinderSync : NSExtensionRequestHandling, FIFinderSyncProtocol {
	}
}
