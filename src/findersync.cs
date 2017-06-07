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

	    [Export ("lastUsedDateForItemWithURL:")]
	    [return: NullAllowed]
	    NSDate GetLastUsedDateForItem (NSUrl itemURL);

	    [Export ("setLastUsedDate:forItemWithURL:completion:")]
	    void SetLastUsedDate (NSDate lastUsedDate, NSUrl itemURL, Action<NSError> completion);

	    [Export ("tagDataForItemWithURL:")]
	    [return: NullAllowed]
	    NSData GetTagDataForItem (NSUrl itemURL);

	    [Export ("setTagData:forItemWithURL:completion:")]
	    void SetTagData ([NullAllowed] NSData tagData, NSUrl itemURL, Action<NSError> completion);
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

	    [Export ("supportedMessageInterfaceNamesForItemWithURL:")]
	    string[] GetSupportedMessageInterfaceNamesForItem (NSUrl itemURL);

	    // @optional -(Protocol * _Nonnull)protocolForMessageInterface:(NSFileProviderMessageInterface * _Nonnull)messageInterface;
	    [Export ("protocolForMessageInterface:")]
	    Protocol GetProtocol (NSFileProviderMessageInterface messageInterface);

	    // @optional -(id _Nonnull)exportedObjectForMessageInterface:(NSFileProviderMessageInterface * _Nonnull)messageInterface itemURL:(NSURL * _Nonnull)itemURL error:(NSError * _Nullable * _Nullable)error;
	    [Export ("exportedObjectForMessageInterface:itemURL:error:")]
	    NSObject GetExportedObject (NSFileProviderMessageInterface messageInterface, NSUrl itemURL, [NullAllowed] out NSError error);

	    // @optional -(void)valuesForAttributes:(NSArray<NSURLResourceKey> * _Nonnull)attributes forItemWithURL:(NSURL * _Nonnull)itemURL completion:(void (^ _Nonnull)(NSDictionary<NSURLResourceKey,id> * _Nonnull, NSError * _Nullable))completion;
	    [Export ("valuesForAttributes:forItemWithURL:completion:")]
	    void ValuesForAttributes (string[] attributes, NSUrl itemURL, Action<NSDictionary<NSString, NSObject>, NSError> completion);
	}	

	[Mac (10, 10, onlyOn64: true)]
	[BaseType (typeof(NSObject))]
	interface FIFinderSync : NSExtensionRequestHandling, FIFinderSyncProtocol
	{
	}
}
#endif