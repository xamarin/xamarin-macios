#if !XAMCORE_5_0

using System;
using System.ComponentModel;

using Foundation;

using ObjCRuntime;

#nullable enable
#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace NewsstandKit {
	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("The NewsstandKit framework has been removed from iOS.")]
	public unsafe partial class NKAssetDownload : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.NewsstandKitRemoved); } }

		protected NKAssetDownload (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		protected internal NKAssetDownload (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NSUrlConnection DownloadWithDelegate (INSUrlConnectionDownloadDelegate downloadDelegate)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual string Identifier {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NKIssue? Issue {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NSUrlRequest UrlRequest {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NSDictionary? UserInfo {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
			set {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		protected override void Dispose (bool disposing)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}
	} /* class NKAssetDownload */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("The NewsstandKit framework has been removed from iOS.")]
	public unsafe partial class NKIssue : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.NewsstandKitRemoved); } }

		protected NKIssue (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		protected internal NKIssue (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NKAssetDownload AddAsset (NSUrlRequest request)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NSUrl ContentUrl {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NSDate Date {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NKAssetDownload [] DownloadingAssets {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual string Name {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NKIssueContentStatus Status {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public static NSString DownloadCompletedNotification {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		//
		// Notifications
		//
		public static partial class Notifications {
			public static NSObject ObserveDownloadCompleted (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
			public static NSObject ObserveDownloadCompleted (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}
	} /* class NKIssue */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("The NewsstandKit framework has been removed from iOS.")]
	public enum NKIssueContentStatus : long {
		None = 0,
		Downloading = 1,
		Available = 2,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete ("The NewsstandKit framework has been removed from iOS.")]
	public unsafe partial class NKLibrary : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.NewsstandKitRemoved); } }

		protected NKLibrary (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		protected internal NKLibrary (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NKIssue AddIssue (string name, NSDate date)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NKIssue? GetIssue (string name)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual void RemoveIssue (NKIssue issue)
		{
			throw new InvalidOperationException (Constants.NewsstandKitRemoved);
		}

		public virtual NKIssue? CurrentlyReadingIssue {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
			set {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NKAssetDownload [] DownloadingAssets {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public virtual NKIssue [] Issues {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}

		public static NKLibrary? SharedLibrary {
			get {
				throw new InvalidOperationException (Constants.NewsstandKitRemoved);
			}
		}
	} /* class NKLibrary */
}


#endif // !XAMCORE_5_0
