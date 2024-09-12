using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using CoreFoundation;
using Darwin;
using Foundation;
using ObjCRuntime;

#if NET

// Let's hope that by .NET 11 we've ironed out all the bugs in the API.
// This can of course be adjusted as needed (until we've released as stable).
#if NET110_0_OR_GREATER
#define STABLE_FSKIT
#endif

using FSDirectoryCookie = System.UIntPtr /* NSUInteger= nuint */;
using FSDirectoryVerifier = System.UIntPtr /* NSUInteger= nuint */;
using FSOperationID = System.UInt64;

namespace FSKit {
#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	delegate void FetchInstalledExtensionsCallback ([NullAllowed] FSModuleIdentity[] identities, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface FSClient
	{
		[Static]
		[Async]
		[Export ("fetchInstalledExtensionsWithCompletionHandler:")]
		void FetchInstalledExtensions (FetchInstalledExtensionsCallback results);

		[Static]
		[Export ("installedExtensionsWithError:")]
		[return: NullAllowed]
		FSModuleIdentity[] GetInstalledExtensions (out NSError error);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	public enum FSContainerState : long
	{
		NotReady = 0,
		Blocked,
		Ready,
		Active,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (FSEntityIdentifier))]
	interface FSContainerIdentifier
	{
		[Export ("volumeIdentifier")]
		FSVolumeIdentifier VolumeIdentifier { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSEntityIdentifier : INSCopying, INSSecureCoding
	{
		[Export ("initWithUUID:")]
		NativeHandle Constructor (NSUuid uuid);

		[Export ("initWithUUID:data:")]
		NativeHandle Constructor (NSUuid uuid, NSData qualifier);

		// There's no documentation on what the 'bytes' pointer is, so wait with the binding for it.
		// [Export ("initWithUUID:byteQualifier:")]
		// NativeHandle Constructor (NSUuid uuid, IntPtr /* sbyte* */ bytes);

		// There's no documentation on what the 'bytes' pointer is, so wait with the binding for it.
		// [Export ("initWithUUID:longByteQualifier:")]
		// NativeHandle Constructor (NSUuid uuid, IntPtr /* sbyte* */ bytes);

		[Static]
		[Export ("identifier")]
		FSEntityIdentifier Create ();

		[Static]
		[Export ("identifierWithUUID:")]
		FSEntityIdentifier Create (NSUuid uuid);

		[Static]
		[Export ("identifierWithUUID:data:")]
		FSEntityIdentifier Create (NSUuid uuid, NSData qualifier);

		// There's no documentation on what the 'bytes' pointer is, so wait with the binding for it.
		// [Static]
		// [Export ("identifierWithUUID:byteQualifier:")]
		// FSEntityIdentifier CreateWithByteQualifier (NSUuid uuid, IntPtr /* sbyte* */ bytes);

		// There's no documentation on what the 'bytes' pointer is, so wait with the binding for it.
		// [Static]
		// [Export ("identifierWithUUID:longByteQualifier:")]
		// FSEntityIdentifier CreateWithLongByteQualifier (NSUuid uuid, IntPtr /* sbyte* */ bytes);

		[Export ("uuid", ArgumentSemantic.Retain)]
		NSUuid Uuid { get; set; }

		[NullAllowed, Export ("qualifier", ArgumentSemantic.Retain)]
		NSData Qualifier { get; set; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Category]
	[BaseType (typeof (NSUuid))]
	interface NSUuid_FSEntityIdentifier
	{
		[Export ("fs_containerIdentifier")]
		FSContainerIdentifier GetFSContainerIdentifier ();

		[Export ("fs_entityIdentifier")]
		FSEntityIdentifier GetFSEntityIdentifier ();

		[Export ("fs_volumeIdentifier")]
		FSVolumeIdentifier GetFSVolumeIdentifier ();
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface FSFileName : INSSecureCoding, INSCopying
	{
		[Export ("data")]
		NSData Data { get; }

		[NullAllowed, Export ("string")]
		string String { get; }

		// The C# binding for this ends up being the same as the the initWithCString: selector, which we've already bound.
		// [Export ("initWithCString:")]
		// [DesignatedInitializer]
		// [Internal]
		// NativeHandle Constructor (IntPtr name);

		[Export ("initWithBytes:length:")]
		[Internal]
		NativeHandle InitWithBytes (IntPtr bytes, nuint length);

		[Export ("initWithData:")]
		NativeHandle Constructor (NSData name);

		[Export ("initWithString:")]
		NativeHandle Constructor (string name);

		// The C# binding for this ends up being the same as the the nameWithString: selector, which we've already bound.
		// [Static]
		// [Export ("nameWithCString:")]
		// [Internal]
		// FSFileName _Create (IntPtr name);

		[Static]
		[Export ("nameWithBytes:length:")]
		[Internal]
		FSFileName _Create (IntPtr bytes, nuint length);

		[Static]
		[Export ("nameWithData:")]
		FSFileName Create (NSData name);

		[Static]
		[Export ("nameWithString:")]
		FSFileName Create (string name);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSFileSystemBaseWipeResourceCompletionHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSFileSystemBase
	{
		[Abstract]
		[NullAllowed, Export ("errorState", ArgumentSemantic.Strong)]
		NSError ErrorState { get; set; }

		[Abstract]
		[Export ("containerState", ArgumentSemantic.Assign)]
		FSContainerState ContainerState { get; set; }

		[Abstract]
		[Export ("wipeResource:includingRanges:excludingRanges:completionHandler:")]
		void WipeResource (FSBlockDeviceResource resource, NSIndexSet includingRanges, NSIndexSet excludingRanges, FSFileSystemBaseWipeResourceCompletionHandler completionHandler);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	public enum FSItemAttribute : long {
		Uid = 1 << 0,
		Gid = 1 << 1,
		Mode = 1 << 2,
		Type = 1 << 3,
		LinkCount = 1 << 4,
		Flags = 1 << 5,
		Size = 1 << 6,
		AllocSize = 1 << 7,
		FileId = 1 << 8,
		ParentId = 1 << 9,
		SupportsLimitedXAttrs = 1 << 10,
		/// <summary>Inhibit Kernel Offloaded IO.</summary>
		InhibitKoio = 1 << 11,
		ModifyTime = 1 << 12,
		AddedTime = 1 << 13,
		ChangeTime = 1 << 14,
		AccessTime = 1 << 15,
		BirthTime = 1 << 16,
		BackupTime = 1 << 17,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	public enum FSItemType : long
	{
		Unknown = 0,
		File,
		Directory,
		Symlink,
		Fifo,
		CharDevice,
		BlockDevice,
		Socket,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSItemAttributes : INSSecureCoding
	{
		[Export ("invalidateAllProperties")]
		void InvalidateAllProperties ();

		[Export ("uid")]
		uint Uid { get; set; }

		[Export ("gid")]
		uint Gid { get; set; }

		[Export ("mode")]
		uint Mode { get; set; }

		[Export ("type", ArgumentSemantic.Assign)]
		FSItemType Type { get; set; }

		[Export ("linkCount")]
		uint LinkCount { get; set; }

		[Export ("flags")]
		uint Flags { get; set; }

		[Export ("size")]
		ulong Size { get; set; }

		[Export ("allocSize")]
		ulong AllocSize { get; set; }

		[Export ("fileID")]
		ulong FileId { get; set; }

		[Export ("parentID")]
		ulong ParentId { get; set; }

		[Export ("supportsLimitedXAttrs")]
		bool SupportsLimitedXAttrs { get; set; }

		// KOIO = Kernel Offloaded IO
		[Export ("inhibitKOIO")]
		bool InhibitKoio { get; set; }

		[Export ("modifyTime")]
		TimeSpec ModifyTime { get; set; }

		[Export ("addedTime")]
		TimeSpec AddedTime { get; set; }

		[Export ("changeTime")]
		TimeSpec ChangeTime { get; set; }

		[Export ("accessTime")]
		TimeSpec AccessTime { get; set; }

		[Export ("birthTime")]
		TimeSpec BirthTime { get; set; }

		[Export ("backupTime")]
		TimeSpec BackupTime { get; set; }

		[Export ("isValid:")]
		bool IsValid (FSItemAttribute attribute);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (FSItemAttributes))]
	interface FSItemSetAttributesRequest
	{
		[Export ("consumedAttributes")]
		FSItemAttribute ConsumedAttributes { get; set; }

		[Export ("wasConsumed:")]
		bool WasConsumed (FSItemAttribute attribute);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof(NSObject))]
	interface FSItemGetAttributesRequest : INSSecureCoding
	{
		[Export ("wantedAttributes")]
		FSItemAttribute WantedAttributes { get; set; }

		[Export ("isWanted:")]
		bool IsWanted (FSItemAttribute attribute);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSItem
	{
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (FSItem))]
	interface FSUnaryItem
	{
		[Export ("queue", ArgumentSemantic.Retain)]
		DispatchQueue Queue { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Partial]
	interface FSKitConstants
	{
		[Field ("FSKitVersionNumber")]
		double FSKitVersionNumber { get; }

		[Field ("FSKitVersionString")]
		[Internal]
		IntPtr _FSKitVersionString { get; }

		[Static]
		string FSKitVersionString {
			[Wrap ("Marshal.PtrToStringUTF8 (_FSKitVersionString)!")]
			get;
		}
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSMessageConnectionDidCompleteHandler ([NullAllowed] NSError deliveryError);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSMessageConnection
	{
		[Abstract]
		[Export ("logMessage:")]
		void LogMessage (string str);

		[Abstract]
		[Export ("didCompleteWithError:completionHandler:")]
		void DidComplete ([NullAllowed] NSError taskError, FSMessageConnectionDidCompleteHandler completionHandler);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Static]
	interface FSModuleIdentityAttribute {
		[Field ("FSModuleIdentityAttributeSupportsServerURLs")]
		NSString SupportsServerUrls { get; }

		[Field ("FSModuleIdentityAttributeSupportsBlockResources")]
		NSString SupportsBlockResources { get; }

		// KOIO = Kernel Offloaded IO
		[Field ("FSModuleIdentityAttributeSupportsKOIO")]
		NSString SupportsKoio { get; }

		[Field ("FSModuleIdentityAttributeShortName")]
		NSString ShortName { get; }

		[Field ("FSModuleIdentityAttributeMediaTypes")]
		NSString MediaTypes { get; }

		[Field ("FSModuleIdentityAttributePersonalities")]
		NSString Personalities { get; }

		[Field ("FSModuleIdentityAttributeCheckOptionSyntax")]
		NSString CheckOptionSyntax { get; }

		[Field ("FSModuleIdentityAttributeFormatOptionSyntax")]
		NSString FormatOptionSyntax { get; }

		[Field ("FSModuleIdentityAttributeActivateOptionSyntax")]
		NSString ActivateOptionSyntax { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[StrongDictionary (nameof (FSModuleIdentityAttribute), Suffix = "")]
	interface FSModuleIdentityAttributes {
		/* There's no documentation about the types of these properties, so I just guessed the types for these properties based on the names whenever possible, otherwise bound as NSObject */
		bool SupportsServerUrls { get; }
		bool SupportsBlockResources { get; }
		bool SupportsKoio { get; } // KOIO = Kernel Offloaded IO
		string ShortName { get; }
		NSObject MediaTypes { get; }
		NSObject Personalities { get; }
		NSObject CheckOptionSyntax { get; }
		NSObject FormatOptionSyntax { get; }
		NSObject ActivateOptionSyntax { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSModuleIdentity
	{
		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("attributes")]
		NSDictionary WeakAttributes { get; }

		[Wrap ("WeakAttributes")]
		FSModuleIdentityAttributes Attributes { get; }

		[Export ("url")]
		NSUrl Url { get; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; }

		[Export ("system")]
		bool IsSystem { [Bind ("isSystem")] get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	public enum FSMatchResult : long
	{
		NotRecognized = 0,
		Recognized,
		UsableButLimited,
		Usable,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSResource : INSSecureCoding
	{
		[Export ("isRevoked")]
		bool IsRevoked { get; }

		[Export ("makeProxy")]
		FSResource MakeProxy ();

		[Export ("revoke")]
		void Revoke ();
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface FSMetadataBlockRange
	{
		[Export ("startBlockOffset")]
		long StartBlockOffset { get; }

		[Export ("blockLength")]
		uint BlockLength { get; }

		[Export ("numberOfBlocks")]
		uint NumberOfBlocks { get; }

		[Export ("initWithOffset:blockLength:numberOfBlocks:")]
		NativeHandle Constructor (long startBlockOffset, uint blockLength, uint numberOfBlocks);

		[Static]
		[Export ("rangeWithOffset:blockLength:numberOfBlocks:")]
		FSMetadataBlockRange Create (long startBlockOffset, uint blockLength, uint numberOfBlocks);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSBlockDeviceResourceReadReplyHandler (nuint actuallyRead, [NullAllowed] NSError error);
#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSBlockDeviceResourceWriteReplyHandler (nuint actuallyWritten, [NullAllowed] NSError error);
#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSBlockDeviceResourceMetadataReplyHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (FSResource))]
	[DisableDefaultCtor]
	interface FSBlockDeviceResource
	{
		[Static]
		[Export ("proxyResourceForBSDName:")]
		[return: NullAllowed]
		FSBlockDeviceResource CreateProxyResource (string bsdName);

		[Static]
		[Export ("proxyResourceForBSDName:isWritable:")]
		[return: NullAllowed]
		FSBlockDeviceResource CreateProxyResource (string bsdName, bool isWritable);

		[Export ("BSDName", ArgumentSemantic.Copy)]
		string BsdName { get; }

		[Export ("writable")]
		bool Writable { [Bind ("isWritable")] get; }

		[Export ("blockSize")]
		ulong BlockSize { get; }

		[Export ("blockCount")]
		ulong BlockCount { get; }

		[Export ("physicalBlockSize")]
		ulong PhysicalBlockSize { get; }

		[Export ("terminated")]
		bool Terminated { [Bind ("isTerminated")] get; }

		[Export ("terminate")]
		void Terminate ();

		[Async]
		[Export ("readInto:startingAt:length:replyHandler:")]
		void Read (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceReadReplyHandler reply);

		[Export ("synchronousReadInto:startingAt:length:replyHandler:")]
		void SynchronousRead (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceReadReplyHandler reply);

		[Async]
		[Export ("writeFrom:startingAt:length:replyHandler:")]
		void Write (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceWriteReplyHandler reply);

		[Export ("synchronousWriteFrom:startingAt:length:replyHandler:")]
		void SynchronousWrite (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceWriteReplyHandler reply);

		[Export ("synchronousMetadataReadInto:startingAt:length:replyHandler:")]
		void SynchronousMetadataRead (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Export ("synchronousMetadataReadInto:startingAt:length:readAheadExtents:readAheadCount:replyHandler:")]
		void SynchronousMetadataRead (IntPtr buffer, long offset, nuint length, IntPtr readAheadExtents, nint readAheadExtentsCount, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Async]
		[Export ("metadataWriteFrom:startingAt:length:replyHandler:")]
		void MetadataWrite (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Export ("synchronousMetadataWriteFrom:startingAt:length:replyHandler:")]
		void SynchronousMetadataWrite (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Async]
		[Export ("delayedMetadataWriteFrom:startingAt:length:replyHandler:")]
		void DelayedMetadataWriteFrom (IntPtr buffer, long offset, nuint length, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Export ("synchronousMetadataFlushWithReplyHandler:")]
		void SynchronousMetadataFlush (FSBlockDeviceResourceMetadataReplyHandler reply);

		[Export ("synchronousMetadataClear:wait:replyHandler:")]
		void SynchronousMetadataClear (FSMetadataBlockRange[] rangesToClear, bool wait, FSBlockDeviceResourceMetadataReplyHandler reply);

		[Export ("synchronousMetadataPurge:replyHandler:")]
		void SynchronousMetadataPurge (FSMetadataBlockRange[] rangesToPurge, FSBlockDeviceResourceMetadataReplyHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSManageableResourceMaintenanceOperationsHandler ([NullAllowed] NSProgress progress, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSManageableResourceMaintenanceOperations
	{
		[Export ("checkWithParameters:connection:taskID:replyHandler:")]
		void Check (string[] parameters, FSMessageConnection connection, NSUuid taskId, FSManageableResourceMaintenanceOperationsHandler reply);

		[Export ("formatWithParameters:connection:taskID:replyHandler:")]
		void Format (string[] parameters, FSMessageConnection connection, NSUuid taskId, FSManageableResourceMaintenanceOperationsHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof(FSResource), Name = "FSGenericURLResource")]
	[DisableDefaultCtor]
	interface FSGenericUrlResource
	{
		[Export ("URL", ArgumentSemantic.Strong)]
		NSUrl Url { get; }

		[Static]
		[Export ("resourceWithURL:")]
		[return: NullAllowed]
		FSGenericUrlResource Create (NSUrl url);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	interface FSProbeResult : NSSecureCoding {
		[Export ("result")]
		FSMatchResult Result { get; }

		[Export ("name", ArgumentSemantic.Copy), NullAllowed]
		string Name { get; }

		[Export ("containerID"), NullAllowed]
		FSContainerIdentifier ContainerId { get; }

		[return: NullAllowed]
		[Static]
		[Export ("resultWithResult:name:containerID:")]
		FSProbeResult Create (FSMatchResult result, [NullAllowed] string name, [NullAllowed] FSContainerIdentifier containerUuid);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSBlockDeviceOperationsProbeResult ([NullAllowed] FSProbeResult result, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSBlockDeviceOperations
	{
		[Abstract]
		[Export ("probeResource:replyHandler:")]
		void ProbeResource (FSResource resource, FSBlockDeviceOperationsProbeResult reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSUnaryFileSystem : FSFileSystemBase {
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSUnaryFileSystemOperationsLoadResourceResult ([NullAllowed] FSVolume volume, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSUnaryFileSystemOperations
	{
		[Abstract]
		[Export ("loadResource:options:replyHandler:")]
		void LoadResource (FSResource resource, string[] options, FSUnaryFileSystemOperationsLoadResourceResult reply);

		[Export ("didFinishLoading")]
		void DidFinishLoading ();
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (FSEntityIdentifier))]
	interface FSVolumeIdentifier {
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[ErrorDomain ("FSVolumeErrorDomain")]
	[Native]
	public enum FSVolumeErrorCode : long
	{
		BadDirectoryCookie = 1,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	public enum FSDeactivateOptions : long
	{
		Force = 1 << 0,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	interface FSVolumeSupportedCapabilities : INSSecureCoding
	{
		[Export ("supportsPersistentObjectIDs")]
		bool SupportsPersistentObjectIds { get; set; }

		[Export ("supportsSymbolicLinks")]
		bool SupportsSymbolicLinks { get; set; }

		[Export ("supportsHardLinks")]
		bool SupportsHardLinks { get; set; }

		[Export ("supportsJournal")]
		bool SupportsJournal { get; set; }

		[Export ("supportsActiveJournal")]
		bool SupportsActiveJournal { get; set; }

		[Export ("doesNotSupportRootTimes")]
		bool DoesNotSupportRootTimes { get; set; }

		[Export ("supportsSparseFiles")]
		bool SupportsSparseFiles { get; set; }

		[Export ("supportsZeroRuns")]
		bool SupportsZeroRuns { get; set; }

		[Export ("supportsCaseSensitiveNames")]
		bool SupportsCaseSensitiveNames { get; set; }

		[Export ("supportsCasePreservingNames")]
		bool SupportsCasePreservingNames { get; set; }

		[Export ("supportsFastStatFS")]
		bool SupportsFastStatFS { get; set; }

		[Export ("supports2TBFiles")]
		bool Supports2TBFiles { get; set; }

		[Export ("supportsOpenDenyModes")]
		bool SupportsOpenDenyModes { get; set; }

		[Export ("supportsHiddenFiles")]
		bool SupportsHiddenFiles { get; set; }

		[Export ("doesNotSupportVolumeSizes")]
		bool DoesNotSupportVolumeSizes { get; set; }

		[Export ("supports64BitObjectIDs")]
		bool Supports64BitObjectIds { get; set; }

		[Export ("supportsDocumentID")]
		bool SupportsDocumentId { get; set; }

		[Export ("doesNotSupportImmutableFiles")]
		bool DoesNotSupportImmutableFiles { get; set; }

		[Export ("doesNotSupportSettingFilePermissions")]
		bool DoesNotSupportSettingFilePermissions { get; set; }

		[Export ("supportsSharedSpace")]
		bool SupportsSharedSpace { get; set; }

		[Export ("supportsVolumeGroups")]
		bool SupportsVolumeGroups { get; set; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface FSVolume
	{
		[Export ("volumeID", ArgumentSemantic.Strong)]
		FSVolumeIdentifier VolumeId { get; }

		[Export ("name", ArgumentSemantic.Copy)]
		FSFileName Name { get; set; }

		[DesignatedInitializer]
		[Export ("initWithVolumeID:volumeName:")]
		NativeHandle Constructor (FSVolumeIdentifier volumeId, FSFileName volumeName);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Static]
	interface FSConstants {
		[Field ("FSDirectoryCookieInitial")]
		nuint FSDirectoryCookieInitial { get; }

		[Field ("FSDirectoryVerifierInitial")]
		nuint FSDirectoryVerifierInitial { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumePathConfOperations
	{
		[Abstract]
		[Export ("maxLinkCount")]
		int MaxLinkCount { get; }

		[Abstract]
		[Export ("maxNameLength")]
		int MaxNameLength { get; }

		[Abstract]
		[Export ("chownRestricted")]
		bool ChownRestricted { [Bind ("isChownRestricted")] get; }

		[Abstract]
		[Export ("longNameTruncated")]
		bool LongNameTruncated { [Bind ("isLongNameTruncated")] get; }

		[Abstract]
		[Export ("maxXattrSizeInBits")]
		int MaxXattrSizeInBits { get; }

		[Abstract]
		[Export ("maxFileSizeInBits")]
		int MaxFileSizeInBits { get; }
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	interface IFSVolumePathConfOperations {}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface FSStatFSResult : INSSecureCoding
	{
		[Export ("blockSize")]
		ulong BlockSize { get; set; }

		[Export ("ioSize")]
		ulong IoSize { get; set; }

		[Export ("totalBlocks")]
		ulong TotalBlocks { get; set; }

		[Export ("availableBlocks")]
		ulong AvailableBlocks { get; set; }

		[Export ("freeBlocks")]
		ulong FreeBlocks { get; set; }

		[Export ("usedBlocks")]
		ulong UsedBlocks { get; set; }

		[Export ("totalBytes")]
		ulong TotalBytes { get; set; }

		[Export ("availableBytes")]
		ulong AvailableBytes { get; set; }

		[Export ("freeBytes")]
		ulong FreeBytes { get; set; }

		[Export ("usedBytes")]
		ulong UsedBytes { get; set; }

		[Export ("totalFiles")]
		ulong TotalFiles { get; set; }

		[Export ("freeFiles")]
		ulong FreeFiles { get; set; }

		[Export ("filesystemSubType")]
		uint FilesystemSubType { get; set; }

		[Export ("filesystemTypeName", ArgumentSemantic.Copy)]
		string FilesystemTypeName { get; }

		[Export ("initWithFSTypeName:")]
		NativeHandle Constructor (string filesystemTypeName);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate bool FSDirectoryEntryPacker (FSFileName name, FSItemType itemType, ulong itemId, FSDirectoryCookie nextCookie, [NullAllowed] FSItemAttributes itemAttributes, bool isLast);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsMountHandler ([NullAllowed] FSItem rootItem, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsSynchronizeHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsAttributesHandler ([NullAllowed] FSItemAttributes attributes, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsLookupItemHandler ([NullAllowed] FSItem item, [NullAllowed] FSFileName itemName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsReclaimHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsReadSymbolicLinkHandler ([NullAllowed] FSFileName attributes, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsCreateItemHandler ([NullAllowed] FSItem newItem, [NullAllowed] FSFileName newItemName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsCreateLinkHandler ([NullAllowed] FSFileName newItemName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsRemoveItemHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsRenameItemHandler ([NullAllowed] FSFileName newName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsEnumerateDirectoryHandler (FSDirectoryVerifier currentVerifier, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsActivateHandler ([NullAllowed] FSItem rootItem, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOperationsDeactivateHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeOperations : FSVolumePathConfOperations
	{
		[Abstract]
		[Export ("supportedVolumeCapabilities")]
		FSVolumeSupportedCapabilities SupportedVolumeCapabilities { get; }

		[Abstract]
		[Export ("volumeStatistics")]
		FSStatFSResult VolumeStatistics { get; }

		[Abstract]
		[Export ("mountWithOptions:replyHandler:")]
		void Mount (string[] options, FSVolumeOperationsMountHandler reply);

		[Abstract]
		[Export ("unmountWithReplyHandler:")]
		void Unmount (Action reply);

		[Abstract]
		[Export ("synchronizeWithReplyHandler:")]
		void Synchronize (FSVolumeOperationsSynchronizeHandler reply);

		[Abstract]
		[Export ("getAttributes:ofItem:replyHandler:")]
		void GetAttributes (FSItemGetAttributesRequest desiredAttributes, FSItem item, FSVolumeOperationsAttributesHandler reply);

		[Abstract]
		[Export ("setAttributes:onItem:replyHandler:")]
		void SetAttributes (FSItemSetAttributesRequest newAttributes, FSItem item, FSVolumeOperationsAttributesHandler reply);

		[Abstract]
		[Export ("lookupItemNamed:inDirectory:replyHandler:")]
		void LookupItem (FSFileName name, FSItem directory, FSVolumeOperationsLookupItemHandler reply);

		[Abstract]
		[Export ("reclaimItem:replyHandler:")]
		void Reclaim (FSItem item, FSVolumeOperationsReclaimHandler reply);

		[Abstract]
		[Export ("readSymbolicLink:replyHandler:")]
		void ReadSymbolicLink (FSItem item, FSVolumeOperationsReadSymbolicLinkHandler reply);

		[Abstract]
		[Export ("createItemNamed:type:inDirectory:attributes:replyHandler:")]
		void CreateItem (FSFileName name, FSItemType type, FSItem directory, FSItemSetAttributesRequest newAttributes, FSVolumeOperationsCreateItemHandler reply);

		[Abstract]
		[Export ("createSymbolicLinkNamed:inDirectory:attributes:linkContents:replyHandler:")]
		void CreateSymbolicLink (FSFileName name, FSItem directory, FSItemSetAttributesRequest newAttributes, FSFileName contents, FSVolumeOperationsCreateItemHandler reply);

		[Abstract]
		[Export ("createLinkToItem:named:inDirectory:replyHandler:")]
		void CreateLink (FSItem item, FSFileName name, FSItem directory, FSVolumeOperationsCreateLinkHandler reply);

		[Abstract]
		[Export ("removeItem:named:fromDirectory:replyHandler:")]
		void RemoveItem (FSItem item, FSFileName name, FSItem directory, FSVolumeOperationsRemoveItemHandler reply);

		[Abstract]
		[Export ("renameItem:inDirectory:named:toNewName:inDirectory:overItem:replyHandler:")]
		void RenameItem (FSItem item, FSItem sourceDirectory, FSFileName sourceName, FSFileName destinationName, FSItem destinationDirectory, [NullAllowed] FSItem overItem, FSVolumeOperationsRenameItemHandler reply);

		[Abstract]
		[Export ("enumerateDirectory:startingAtCookie:verifier:providingAttributes:usingBlock:replyHandler:")]
		void EnumerateDirectory (FSItem directory, FSDirectoryCookie startingAt, FSDirectoryVerifier verifier, [NullAllowed] FSItemGetAttributesRequest attributes, FSDirectoryEntryPacker packer, FSVolumeOperationsEnumerateDirectoryHandler reply);

		[Abstract]
		[Export ("activateWithOptions:replyHandler:")]
		void Activate (string[] options, FSVolumeOperationsActivateHandler reply);

		[Abstract]
		[Export ("deactivateWithOptions:replyHandler:")]
		void Deactivate (FSDeactivateOptions options, FSVolumeOperationsDeactivateHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	enum FSSetXattrPolicy : ulong {
		AlwaysSet   = 0,
		MustCreate  = 1,
		MustReplace = 2,
		Delete      = 3,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeXattrOperationsGetHandler ([NullAllowed] NSData value, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeXattrOperationsSetHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeXattrOperationsListHandler ([NullAllowed] FSFileName[] value, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeXattrOperations
	{
		[Export ("xattrOperationsInhibited")]
		bool XattrOperationsInhibited { get; set; }

		[return: NullAllowed] // header says to return null instead of empty array
		[Export ("supportedXattrNamesForItem:")]
		FSFileName [] GetSupportedXattrNames (FSItem item);

		[Abstract]
		[Export ("xattrNamed:ofItem:replyHandler:")]
		void GetXattr (FSFileName name, FSItem item, FSVolumeXattrOperationsGetHandler reply);

		[Abstract]
		[Export ("setXattrNamed:toData:onItem:policy:replyHandler:")]
		void SetXattr (FSFileName name, [NullAllowed] NSData value, FSItem item, FSSetXattrPolicy policy, FSVolumeXattrOperationsSetHandler reply);

		[Abstract]
		[Export ("listXattrsOfItem:replyHandler:")]
		void ListXattrs (FSItem item, FSVolumeXattrOperationsListHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Native]
	[Flags]
	enum FSVolumeOpenModes : ulong {
		Read = 0x00000001,/* FREAD */
		Write = 0x00000002, /* FWRITE */
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeOpenCloseOperationsHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeOpenCloseOperations
	{
		[Abstract]
		[Export ("openItem:withModes:replyHandler:")]
		void OpenItem (FSItem item, FSVolumeOpenModes mode, FSVolumeOpenCloseOperationsHandler reply);

		[Abstract]
		[Export ("closeItem:keepingModes:replyHandler:")]
		void CloseItem (FSItem item, FSVolumeOpenModes mode, FSVolumeOpenCloseOperationsHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeReadWriteOperationsReadHandler (nuint actuallyRead, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeReadWriteOperationsWriteHandler (nuint actuallyWritten, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeReadWriteOperations
	{
		[Abstract]
		[Export ("readFromFile:offset:length:intoBuffer:replyHandler:")]
		void Read (FSItem item, ulong offset, nuint length, NSMutableData buffer, FSVolumeReadWriteOperationsReadHandler reply);

		[Abstract]
		[Export ("writeContents:toFile:atOffset:replyHandler:")]
		void Write (NSData contents, FSItem item, ulong offset, FSVolumeReadWriteOperationsWriteHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Flags]
	[Native]
	enum FSAccessMask : ulong {
		ReadData        = (1<<1),
		ListDirectory   = ReadData,
		WriteData       = (1<<2),
		AddFile         = WriteData,
		Execute         = (1<<3),
		Search          = Execute,
		Delete          = (1<<4),
		AppendData      = (1<<5),
		AddSubdirectory = AppendData,
		DeleteChild     = (1<<6),
		ReadAttributes  = (1<<7),
		WriteAttributes = (1<<8),
		ReadXattr       = (1<<9),
		WriteXattr      = (1<<10),
		ReadSecurity    = (1<<11),
		WriteSecurity   = (1<<12),
		TakeOwnership   = (1<<13),
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeAccessCheckOperationsCheckAccessHandler (bool shouldAllowAccess, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeAccessCheckOperations
	{
		[Export ("accessCheckOperationsInhibited")]
		bool AccessCheckOperationsInhibited { get; set; }

		[Abstract]
		[Export ("checkAccessToItem:requestedAccess:replyHandler:")]
		void CheckAccess (FSItem theItem, FSAccessMask access, FSVolumeAccessCheckOperationsCheckAccessHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeRenameOperationsSetVolumeNameHandler (FSFileName newName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeRenameOperations
	{
		[Export ("volumeRenameOperationsInhibited")]
		bool VolumeRenameOperationsInhibited { get; set; }

		[Abstract]
		[Export ("setVolumeName:replyHandler:")]
		void RenameVolume (FSFileName name, FSVolumeRenameOperationsSetVolumeNameHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Flags]
	public enum FSBlockmapFlags : uint {
		Read       = 0x000100,
		Write      = 0x000200,
		Async      = 0x000400,
		NoCache    = 0x000800,
		FileIssued = 0x001000,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	public enum FSExtentType
	{
		Data = 0,
		Zero = 1,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate bool FSExtentPacker (FSBlockDeviceResource resource, FSExtentType type, ulong logicalOffset, ulong physicalOffset, uint length);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeKoioOperationsHandler ([NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeKoioOperationsCreateFileHandler ([NullAllowed] FSItem newItem, [NullAllowed] FSFileName newItemName, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumeKoioOperationsLookupItemHandler ([NullAllowed] FSItem newItem, [NullAllowed] FSFileName newItemName, [NullAllowed] NSError error);

	// KOIO = Kernel Offloaded IO
#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (Name = "FSVolumeKOIOOperations", BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumeKoioOperations
	{
		[Abstract]
		[Export ("blockmapFile:range:startIO:flags:operationID:usingPacker:replyHandler:")]
		void BlockmapFile (FSItem item, NSRange theRange, bool firstIo, FSBlockmapFlags flags, FSOperationID operationId, FSExtentPacker packer, FSVolumeKoioOperationsHandler reply);

		[Abstract]
		[Export ("endIO:range:status:flags:operationID:replyHandler:")]
		void EndIo (FSItem item, NSRange originalRange, NSError ioStatus, FSBlockmapFlags flags, FSOperationID operationId, FSVolumeKoioOperationsHandler reply);

		[Abstract]
		[Export ("createFileNamed:inDirectory:attributes:usingPacker:replyHandler:")]
		void CreateFile (FSFileName name, FSItem directory, FSItemSetAttributesRequest newAttributes, FSExtentPacker packer, FSVolumeKoioOperationsCreateFileHandler reply);

		[Abstract]
		[Export ("lookupItemNamed:inDirectory:usingPacker:replyHandler:")]
		void LookupItem (FSFileName name, FSItem directory, FSExtentPacker packer, FSVolumeKoioOperationsLookupItemHandler reply);
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Flags]
	enum FSPreallocateFlags : uint {
		All        = 0x00000002,
		Contig     = 0x00000004,
		FromEof    = 0x00000010,
		FromVol    = 0x00000020,
	}

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	delegate void FSVolumePreallocateOperationsHandler (nuint bytesAllocated, [NullAllowed] NSError error);

#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[Mac (15, 0)]
	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface FSVolumePreallocateOperations
	{
		[Export ("preallocateOperationsInhibited")]
		bool PreallocateOperationsInhibited { get; set; }

		[Abstract]
		[Export ("preallocate:offset:length:flags:usingPacker:replyHandler:")]
		void Preallocate (FSItem item, ulong offset, nuint length, FSPreallocateFlags flags, FSExtentPacker packer, FSVolumePreallocateOperationsHandler reply);
	}
}

#endif // NET
