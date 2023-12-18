using System;
using System.Threading.Tasks;
using CloudKit;
using ObjCRuntime;

namespace Foundation {
	public partial class NSItemProvider {
#if !NET && MONOMAC
		[Obsolete ("Use RegisterCloudKitShare (CloudKitRegistrationPreparationAction) instead.")]
		public virtual void RegisterCloudKitShare (Action<CloudKitRegistrationPreparationHandler> preparationHandler)
		{
			CloudKitRegistrationPreparationAction action = handler => preparationHandler (handler);
			RegisterCloudKitShare (action);
		}
#endif

#if MONOMAC
		public virtual Task<CloudKitRegistrationPreparationHandler> RegisterCloudKitShareAsync ()
		{
			var tcs = new TaskCompletionSource<CloudKitRegistrationPreparationHandler> ();
			CloudKitRegistrationPreparationAction action = (handler) => {
				tcs.SetResult (handler);
			};
			RegisterCloudKitShare (action);
			return tcs.Task;
		}
#endif

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public NSProgress LoadObject<T> (Action<T, NSError> completionHandler) where T : NSObject, INSItemProviderReading
		{
			return LoadObject (new Class (typeof (T)), (rv, err) => {
				var obj = rv as T;
				if (obj is null && rv is not null)
					obj = Runtime.ConstructNSObject<T> (rv.Handle);
				completionHandler (obj, err);
			});
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public Task<T> LoadObjectAsync<T> () where T : NSObject, INSItemProviderReading
		{
			var rv = LoadObjectAsync (new Class (typeof (T)));
			return rv.ContinueWith ((v) => {
				var obj = v.Result as T;
				if (obj is null && v.Result is not null)
					obj = Runtime.ConstructNSObject<T> (v.Result.Handle);
				return obj;
			});
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public Task<T> LoadObjectAsync<T> (out NSProgress result) where T : NSObject, INSItemProviderReading
		{
			var rv = LoadObjectAsync (new Class (typeof (T)), out result);
			return rv.ContinueWith ((v) => {
				var obj = v.Result as T;
				if (obj is null && v.Result is not null)
					obj = Runtime.ConstructNSObject<T> (v.Result.Handle);
				return obj;
			});
		}
	}
}
