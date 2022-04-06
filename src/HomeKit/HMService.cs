using System;
using System.Threading.Tasks;

using ObjCRuntime;
using Foundation;

namespace HomeKit {

#if NET
	[SupportedOSPlatform ("tvos10.0")]
	[SupportedOSPlatform ("ios8.0")]
#else
	[TV (10,0)]
	[iOS (8,0)]
#endif
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
