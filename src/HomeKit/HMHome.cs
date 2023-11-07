#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Foundation;

namespace HomeKit {

	public partial class HMHome {
		public HMService []? GetServices (HMServiceType serviceTypes)
		{
			return GetServices (serviceTypes.ToArray ());
		}

#if !NET
		[NoTV]
		[NoWatch]
#if (WATCH || TVOS)
		[Obsolete ("This API is not available on this platform.")]
#endif // WATCH || TVOS
		[Obsoleted (PlatformName.iOS, 9, 0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		public virtual void RemoveUser (HMUser user, Action<NSError> completion)
		{
			throw new NotSupportedException ();
		}

		[NoTV]
		[NoWatch]
#if (WATCH || TVOS)
		[Obsolete ("This API is not available on this platform.")]
#endif // WATCH || TVOS
		[Obsoleted (PlatformName.iOS, 9, 0, PlatformArchitecture.All, message: "This API in now prohibited on iOS. Use 'ManageUsers' instead.")]
		public virtual Task RemoveUserAsync (HMUser user)
		{
			throw new NotSupportedException ();
		}
#endif
	}
}
