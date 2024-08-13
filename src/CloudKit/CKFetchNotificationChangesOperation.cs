#if !XAMCORE_5_0

using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;

using Foundation;
using ObjCRuntime;

#nullable enable

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CloudKit {
	[Register ("CKFetchNotificationChangesOperation", SkipRegistration = true)]
#if NET
	[UnsupportedOSPlatform ("ios", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("macos", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("tvos", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("maccatalyst", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
#else
	[Obsolete ("Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
#endif
	[EditorBrowsable (EditorBrowsableState.Never)]
	public unsafe partial class CKFetchNotificationChangesOperation : CKOperation {
		public override NativeHandle ClassHandle { get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms); }

		protected CKFetchNotificationChangesOperation (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		protected internal CKFetchNotificationChangesOperation (NativeHandle handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public CKFetchNotificationChangesOperation ()
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public CKFetchNotificationChangesOperation (CKServerChangeToken? previousServerChangeToken)
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public unsafe virtual global::System.Action<CKServerChangeToken, NSError>? Completed {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public virtual bool MoreComing {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public unsafe virtual global::System.Action<CKNotification>? NotificationChanged {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public virtual CKServerChangeToken? PreviousServerChangeToken {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public virtual nuint ResultsLimit {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}
	} /* class CKFetchNotificationChangesOperation */
}

#endif // !XAMCORE_5_0
