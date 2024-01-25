// Copyright 2016, Xamarin Inc. All rights reserved.

#nullable enable

#if !COREBUILD && !NET

using System;
using System.Threading.Tasks;

#if MONOMAC || IOS
using Contacts;
#endif
using CloudKit;

using Foundation;

using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CloudKit {

#if !NET
	public partial class CKQueryNotification {

		[Obsolete ("Empty stub (not public API). Use 'DatabaseScope' instead.")]
		public virtual bool IsPublicDatabase { get; }
	}
#endif

#if !NET && !WATCH
	public partial class CKOperation {

		[Obsoleted (PlatformName.iOS, 9, 3, message: "Do not use; this API was removed and will always return 0.")]
		public virtual ulong ActivityStart ()
		{
			return 0;
		}

		[Deprecated (PlatformName.iOS, 9, 0, message: "Empty stub (rejected by Apple). Use 'QualityOfService' property.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Empty stub (rejected by Apple). Use 'QualityOfService' property.")]
		public virtual bool UsesBackgroundSession { get; set; }
	}

	public partial class CKNotificationID {

		[Obsolete ("This type is not meant to be created by user code.")]
		public CKNotificationID ()
		{
		}
	}

	public partial class CKContainer {
#if __IOS__ || MONOMAC
		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverAllIdentities' instead.")]
		public virtual void DiscoverAllContactUserInfos (Action<CKDiscoveredUserInfo[], NSError> completionHandler) 
			=> throw new NotSupportedException ();

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverAllIdentities' instead.")]
		public virtual Task<CKDiscoveredUserInfo[]> DiscoverAllContactUserInfosAsync ()
			=> Task.FromException<CKDiscoveredUserInfo[]> (new NotSupportedException ());
#endif

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverUserIdentityWithEmailAddress' instead.")]
		public virtual void DiscoverUserInfo (string email, Action<CKDiscoveredUserInfo, NSError> completionHandler)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverUserIdentityWithEmailAddress' instead.")]
		public virtual Task<CKDiscoveredUserInfo> DiscoverUserInfoAsync (string email)
			=> Task.FromException<CKDiscoveredUserInfo> (new NotSupportedException ());

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverUserIdentity' instead.")]
		public virtual void DiscoverUserInfo (CKRecordID userRecordId, Action<CKDiscoveredUserInfo, NSError> completionHandler)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throw a 'NotSupportedException' (not a public API). Use 'DiscoverUserIdentity' instead.")]
		public virtual Task<CKDiscoveredUserInfo> DiscoverUserInfoAsync (CKRecordID userRecordId)
			=> Task.FromException<CKDiscoveredUserInfo> (new NotSupportedException ());
	}
#if !TVOS
	public partial class CKDiscoverAllContactsOperation {

		[Obsolete ("Empty stub (not a public API).")]
		public virtual Action<CKDiscoveredUserInfo [], NSError>? DiscoverAllContactsHandler { get; set; }

	}
#endif

	public delegate void CKDiscoverUserInfosCompletionHandler (NSDictionary emailsToUserInfos, NSDictionary userRecordIdsToUserInfos, NSError operationError);

#if !WATCH
	[Obsoleted (PlatformName.iOS, 14, 0, message: "Use 'CKDiscoverUserIdentitiesOperation' instead.")]
	public partial class CKDiscoverUserInfosOperation : CKOperation {

		public CKDiscoverUserInfosOperation () : base () { }

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		protected CKDiscoverUserInfosOperation (Foundation.NSObjectFlag t)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		protected CKDiscoverUserInfosOperation (NativeHandle handle)
			=> throw new NotSupportedException ();

		[Obsolete ("Empty stub (not a public API).")]
		public virtual CKDiscoverUserInfosCompletionHandler? Completed { get; set; }

		[Obsolete ("Always throws 'NotSupportedException' (not a public API).")]
		public CKDiscoverUserInfosOperation (string [] emailAddresses, CKRecordID [] userRecordIDs)
			=> throw new NotSupportedException ();

		[Obsolete ("Empty stub (not a public API).")]
		public virtual string []? EmailAddresses { get; set; }

		[Obsolete ("Empty stub (not a public API).")]
		public virtual CKRecordID []? UserRecordIds { get; set; }

#pragma warning disable CS0809
		[Obsolete ("Empty stub (not a public API).")]
		public override NativeHandle ClassHandle { get; }
#pragma warning restore CS0809
	}
#endif

	public partial class CKSubscription {
		[Obsolete ("Always throws 'NotSupportedException' (not a public API). Use 'CKRecordZoneSubscription' instead.")]
		public CKSubscription (CKRecordZoneID zoneId, CKSubscriptionOptions subscriptionOptions)
			=> throw new NotSupportedException ();

		[Obsolete ("Always throws 'NotSupportedException' (not a public API). Use 'CKRecordZoneSubscription' instead.")]
		public CKSubscription (CKRecordZoneID zoneId, string subscriptionId, CKSubscriptionOptions subscriptionOptions)
			=> throw new NotSupportedException ();
#if !WATCH
		[Obsolete ("Empty stub (not a public API). Use 'CKRecordZoneSubscription' intead.")]
		public virtual CKSubscriptionOptions SubscriptionOptions { get; }
#endif
	}

#if MONOMAC || IOS
	public partial class CKDiscoveredUserInfo {
		[Obsolete ("Empty stub (not public API).")]
		public virtual CNContact? DisplayContact { get; }

		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'DisplayContact.GivenName'.")]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'DisplayContact.GivenName'.")]
		[Obsoleted (PlatformName.MacOSX, 12, 0, message : "Use 'DisplayContact.GivenName'.")]
		[Obsoleted (PlatformName.iOS, 15, 0, message : "Use 'DisplayContact.GivenName'.")]
		public virtual string? FirstName {
			get { return null; }
		}

		[Deprecated (PlatformName.MacOSX, 10, 11, message : "Use 'DisplayContact.FamilyName'.")]
		[Deprecated (PlatformName.iOS, 9, 0, message : "Use 'DisplayContact.FamilyName'.")]
		[Obsoleted (PlatformName.MacOSX, 12, 0, message : "Use 'DisplayContact.FamilyName'.")]
		[Obsoleted (PlatformName.iOS, 15, 0, message : "Use 'DisplayContact.FamilyName'.")]
		public virtual string? LastName {
			get { return null; }
		}
	}
#endif

#endif

#if WATCH

	public partial class CKModifyBadgeOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyBadgeOperation () : this (0)
		{
		}
	}

	public partial class CKModifyRecordZonesOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyRecordZonesOperation () : this (null, null)
		{
		}
	}

	public partial class CKModifyRecordsOperation {

		// `init` does not work on watchOS but we can keep compatibility with a different init
		public CKModifyRecordsOperation () : this (null, null)
		{
		}
	}
#endif
}

#endif
