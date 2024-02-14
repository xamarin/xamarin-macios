#if !XAMCORE_5_0

#nullable enable

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreFoundation;
using CoreLocation;
using UIKit;
using MediaPlayer;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace AssetsLibrary {

	public delegate void ALAssetsEnumerator (ALAsset result, nint index, ref bool stop);

	public delegate void ALAssetsLibraryGroupsEnumerationResultsDelegate (ALAssetsGroup group, ref bool stop);

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public enum ALAssetsError : int {
		UnknownError = -1,
		WriteFailedError = -3300,
		WriteBusyError = -3301,
		WriteInvalidDataError = -3302,
		WriteIncompatibleDataError = -3303,
		WriteDataEncodingError = -3304,
		WriteDiskSpaceError = -3305,
		DataUnavailableError = -3310,
		AccessUserDeniedError = -3311,
		AccessGloballyDeniedError = -3312,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	static public partial class ALAssetsErrorExtensions {
		public static NSString? GetDomain (this ALAssetsError self)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	[Native]
	public enum ALAssetOrientation : long {
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		UpMirrored = 4,
		DownMirrored = 5,
		LeftMirrored = 6,
		RightMirrored = 7,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	[Flags]
	[Native]
	public enum ALAssetsGroupType : ulong {
		Library = 1,
		Album = 2,
		Event = 4,
		Faces = 8,
		SavedPhotos = 16,
		GroupPhotoStream = 32,
		All = 4294967295,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public enum ALAssetType : int {
		Video = 0,
		Photo = 1,
		Unknown = 2,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	[Native]
	public enum ALAuthorizationStatus : long {
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Authorized = 3,
	}

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public unsafe partial class ALAsset : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.AssetsLibraryRemoved); } }

		public ALAsset () : base (NSObjectFlag.Empty)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected ALAsset (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected internal ALAsset (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual CGImage AspectRatioThumbnail ()
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual ALAssetRepresentation RepresentationForUti (string uti)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void SetImageData (NSData imageData, NSDictionary metadata, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> SetImageDataAsync (NSData imageData, NSDictionary metadata)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void SetVideoAtPath (NSUrl videoPathURL, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> SetVideoAtPathAsync (NSUrl videoPathURL)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual NSObject ValueForProperty (NSString property)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteModifiedImageToSavedToPhotosAlbum (NSData imageData, NSDictionary metadata, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteModifiedImageToSavedToPhotosAlbumAsync (NSData imageData, NSDictionary metadata)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteModifiedVideoToSavedPhotosAlbum (NSUrl videoPathURL, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteModifiedVideoToSavedPhotosAlbumAsync (NSUrl videoPathURL)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual ALAssetRepresentation DefaultRepresentation {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual bool Editable {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual ALAsset OriginalAsset {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual CGImage Thumbnail {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyAssetURL {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyDate {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyDuration {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyLocation {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyOrientation {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyRepresentations {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyType {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyURLs {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _TypePhoto {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _TypeUnknown {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _TypeVideo {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public ALAssetType AssetType {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public CLLocation Location {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public double Duration {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public ALAssetOrientation Orientation {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public NSDate Date {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public string [] Representations {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public NSDictionary UtiToUrlDictionary {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public NSUrl? AssetUrl {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	} /* class ALAsset */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public unsafe partial class ALAssetRepresentation : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.AssetsLibraryRemoved); } }

		public ALAssetRepresentation () : base (NSObjectFlag.Empty)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected ALAssetRepresentation (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected internal ALAssetRepresentation (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual nuint GetBytes (global::System.IntPtr buffer, long offset, nuint length, out NSError error)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual CGImage GetFullScreenImage ()
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual CGImage GetImage ()
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual CGImage GetImage (NSDictionary options)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual CGSize Dimensions {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual string Filename {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual NSDictionary Metadata {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual ALAssetOrientation Orientation {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual float Scale {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual long Size {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual NSUrl Url {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual string Uti {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	} /* class ALAssetRepresentation */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public unsafe partial class ALAssetsFilter : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.AssetsLibraryRemoved); } }

		public ALAssetsFilter () : base (NSObjectFlag.Empty)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected ALAssetsFilter (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected internal ALAssetsFilter (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public static ALAssetsFilter AllAssets {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static ALAssetsFilter AllPhotos {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static ALAssetsFilter AllVideos {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	} /* class ALAssetsFilter */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public unsafe partial class ALAssetsGroup : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.AssetsLibraryRemoved); } }

		public ALAssetsGroup () : base (NSObjectFlag.Empty)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected ALAssetsGroup (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected internal ALAssetsGroup (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual bool AddAsset (ALAsset asset)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void Enumerate (ALAssetsEnumerator result)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void Enumerate (NSEnumerationOptions options, ALAssetsEnumerator result)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void Enumerate (NSIndexSet indexSet, NSEnumerationOptions options, ALAssetsEnumerator result)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual void SetAssetsFilter (ALAssetsFilter filter)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		internal virtual NSObject ValueForProperty (NSString property)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual nint Count {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual bool Editable {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public virtual CGImage PosterImage {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _Name {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PersistentID {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _PropertyUrl {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		internal static NSString _Type {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public NSString Name {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public ALAssetsGroupType Type {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public string PersistentID {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public NSUrl PropertyUrl {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	} /* class ALAssetsGroup */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public unsafe partial class ALAssetsLibrary : NSObject {
		public override NativeHandle ClassHandle { get { throw new InvalidOperationException (Constants.AssetsLibraryRemoved); } }

		public ALAssetsLibrary () : base (NSObjectFlag.Empty)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected ALAssetsLibrary (NSObjectFlag t) : base (t)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		protected internal ALAssetsLibrary (NativeHandle handle) : base (handle)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void AddAssetsGroupAlbum (string name, global::System.Action<ALAssetsGroup> resultBlock, global::System.Action<NSError> failureBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void AssetForUrl (NSUrl assetURL, global::System.Action<ALAsset> resultBlock, global::System.Action<NSError> failureBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public static void DisableSharedPhotoStreamsSupport ()
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void Enumerate (ALAssetsGroupType types, ALAssetsLibraryGroupsEnumerationResultsDelegate enumerationBlock, global::System.Action<NSError> failureBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void GroupForUrl (NSUrl groupURL, global::System.Action<ALAssetsGroup> resultBlock, global::System.Action<NSError> failureBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public virtual bool VideoAtPathIsIsCompatibleWithSavedPhotosAlbum (NSUrl videoPathURL)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteImageToSavedPhotosAlbum (NSData imageData, NSDictionary metadata, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteImageToSavedPhotosAlbumAsync (NSData imageData, NSDictionary metadata)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteImageToSavedPhotosAlbum (CGImage imageData, NSDictionary metadata, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteImageToSavedPhotosAlbumAsync (CGImage imageData, NSDictionary metadata)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteImageToSavedPhotosAlbum (CGImage imageData, ALAssetOrientation orientation, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteImageToSavedPhotosAlbumAsync (CGImage imageData, ALAssetOrientation orientation)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual void WriteVideoToSavedPhotosAlbum (NSUrl videoPathURL, global::System.Action<NSUrl, NSError>? completionBlock)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public unsafe virtual Task<NSUrl> WriteVideoToSavedPhotosAlbumAsync (NSUrl videoPathURL)
		{
			throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
		}

		public static ALAuthorizationStatus AuthorizationStatus {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		[Advice ("Use ALAssetsLibrary.Notifications.ObserveChanged helper method instead.")]
		public static NSString ChangedNotification {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static NSString DeletedAssetGroupsKey {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static NSString InsertedAssetGroupsKey {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static NSString UpdatedAssetGroupsKey {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public static NSString UpdatedAssetsKey {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		//
		// Notifications
		//
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete (Constants.AssetsLibraryRemoved)]
		public static partial class Notifications {
			public static NSObject ObserveChanged (EventHandler<NSNotificationEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
			public static NSObject ObserveChanged (NSObject objectToObserve, EventHandler<NSNotificationEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
			public static NSObject ObserveChanged (EventHandler<AssetsLibrary.ALAssetLibraryChangedEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
			public static NSObject ObserveChanged (NSObject objectToObserve, EventHandler<AssetsLibrary.ALAssetLibraryChangedEventArgs> handler)
			{
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	} /* class ALAssetsLibrary */

	[EditorBrowsable (EditorBrowsableState.Never)]
	[Obsolete (Constants.AssetsLibraryRemoved)]
	public partial class ALAssetLibraryChangedEventArgs : NSNotificationEventArgs {
		public ALAssetLibraryChangedEventArgs (NSNotification notification) : base (notification)
		{
		}

		public Foundation.NSSet UpdatedAssets {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public Foundation.NSSet InsertedAssetGroups {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public Foundation.NSSet UpdatedAssetGroups {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}

		public Foundation.NSSet DeletedAssetGroupsKey {
			get {
				throw new InvalidOperationException (Constants.AssetsLibraryRemoved);
			}
		}
	}

}

#endif // !XAMCORE_5_0
