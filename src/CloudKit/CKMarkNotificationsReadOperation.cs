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
	[Register ("CKMarkNotificationsReadOperation", SkipRegistration = true)]
#if NET
	[UnsupportedOSPlatform ("ios", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("macos", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("tvos", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
	[UnsupportedOSPlatform ("maccatalyst", "Use 'CKDatabaseSubscription', 'CKFetchDatabaseChangesOperation' and 'CKFetchRecordZoneChangesOperation' instead.")]
#endif
	[EditorBrowsable (EditorBrowsableState.Never)]
	public unsafe partial class CKMarkNotificationsReadOperation : CKOperation {
		public override NativeHandle ClassHandle { get => throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms); }

		protected CKMarkNotificationsReadOperation (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		protected internal CKMarkNotificationsReadOperation (NativeHandle handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public CKMarkNotificationsReadOperation (CKNotificationID [] notificationIds)
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
		}

		public unsafe virtual CKMarkNotificationsReadHandler? Completed {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}

		public virtual CKNotificationID []? NotificationIds {
			get {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
			set {
				throw new PlatformNotSupportedException (Constants.TypeRemovedAllPlatforms);
			}
		}
	} /* class CKMarkNotificationsReadOperation */

	[EditorBrowsable (EditorBrowsableState.Never)]
	public delegate void CKMarkNotificationsReadHandler (CKNotificationID [] notificationIDsMarkedRead, NSError operationError);
}
#endif // !XAMCORE_5_0
