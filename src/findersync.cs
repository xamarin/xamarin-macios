using System;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.AppKit;

#if XAMCORE_2_0
namespace XamCore.FinderSync {
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
	}	

	[Mac (10, 10, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface FIFinderSync : NSExtensionRequestHandling, FIFinderSyncProtocol
	{
	}
}
#endif