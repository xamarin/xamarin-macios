#nullable enable

using System;
using System.Threading.Tasks;

using ObjCRuntime;
using Foundation;

namespace HomeKit {

	public partial class HMService {

#if !TVOS
		public void UpdateAssociatedServiceType (HMServiceType serviceType, Action<NSError> completion)
		{
			UpdateAssociatedServiceType (serviceType.GetConstant (), completion);
		}

		public Task UpdateAssociatedServiceTypeAsync (HMServiceType serviceType)
		{
			return UpdateAssociatedServiceTypeAsync (serviceType.GetConstant ());
		}
#endif
	}
}
