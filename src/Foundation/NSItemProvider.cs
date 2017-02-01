using System;
using System.Threading.Tasks;
using XamCore.CloudKit;

namespace XamCore.Foundation 
{
#if MONOMAC && XAMCORE_2_0 // Only 64-bit on mac
	public partial class NSItemProvider
	{
#if !XAMCORE_4_0
		public unsafe virtual void RegisterCloudKitShare (Action<CloudKitRegistrationPreparationHandler> preparationHandler)
		{
			CloudKitRegistrationPreparationAction action = handler => preparationHandler (handler);
			RegisterCloudKitShare (action);
		}
#endif
		
		public unsafe virtual Task<CloudKitRegistrationPreparationHandler> RegisterCloudKitShareAsync ()
		{
			var tcs = new TaskCompletionSource<CloudKitRegistrationPreparationHandler> ();
			CloudKitRegistrationPreparationAction action = (handler) => {
				tcs.SetResult (handler);
			};
			RegisterCloudKitShare (action);
			return tcs.Task;
		}
	}
#endif
}
