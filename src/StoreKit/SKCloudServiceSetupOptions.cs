#if !MONOMAC && !TVOS

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.StoreKit {

	partial class SKCloudServiceSetupOptions {

		[iOS (10,1)]
		public virtual SKCloudServiceSetupAction Action {
			get {
				return (SKCloudServiceSetupAction) (SKCloudServiceSetupActionExtensions.GetValue (_Action));
			}
			set {
				_Action = SKCloudServiceSetupActionExtensions.GetConstant (value);
			}
		}
	}
}

#endif // !MONOMAC && !TVOS