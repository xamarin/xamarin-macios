using System;
using System.Threading.Tasks;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.HomeKit {

	[TV (10,0)]
	[iOS (8,0)]
	public partial class HMService {

#if !WATCH && !TVOS
		public void UpdateAssociatedServiceType (HMServiceType serviceType, Action<NSError> completion)
		{
			UpdateAssociatedServiceType (serviceType.GetConstant (), completion);
		}

		public Task UpdateAssociatedServiceTypeAsync (HMServiceType serviceType)
		{
			return UpdateAssociatedServiceTypeAsync (serviceType.GetConstant ());
		}

#if !XAMCORE_3_0
		[Obsolete]
		public Task UpdateNameAsync (HMServiceType serviceType)
		{
			return UpdateNameAsync (serviceType.GetConstant ());
		}
#endif
#endif
	}
}