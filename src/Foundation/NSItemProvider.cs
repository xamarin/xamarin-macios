using System;
using System.Threading.Tasks;
using CloudKit;
using ObjCRuntime;

namespace Foundation 
{
#if (MONOMAC || IOS) && XAMCORE_2_0 // Only 64-bit on mac
	public partial class NSItemProvider
	{
#if !XAMCORE_4_0 && MONOMAC
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

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		public NSProgress LoadObject<T> (Action<T, NSError> completionHandler) where T: NSObject, INSItemProviderReading
		{
			return LoadObject (new Class (typeof (T)), (rv, err) =>
			{
				var obj = rv as T;
				if (obj == null && rv != null)
					obj = Runtime.ConstructNSObject<T> (rv.Handle);
				completionHandler (obj, err);
			});
		}

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		public Task<T> LoadObjectAsync<T> () where T: NSObject, INSItemProviderReading
		{
			var rv = LoadObjectAsync (new Class (typeof (T)));
			return rv.ContinueWith ((v) =>
			{
				var obj = v.Result as T;
				if (obj == null && v.Result != null)
					obj = Runtime.ConstructNSObject<T> (v.Result.Handle);
				return obj;
			});
		}

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		public Task<T> LoadObjectAsync<T> (out NSProgress result) where T: NSObject, INSItemProviderReading
		{
			var rv = LoadObjectAsync (new Class (typeof (T)), out result);
			return rv.ContinueWith ((v) =>
			{
				var obj = v.Result as T;
				if (obj == null && v.Result != null)
					obj = Runtime.ConstructNSObject<T> (v.Result.Handle);
				return obj;
			});
		}
	}
#endif // (MONOMAC || IOS) && XAMCORE_2_0
}
