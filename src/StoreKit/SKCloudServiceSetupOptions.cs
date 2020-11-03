#if __IOS__

using System;
using ObjCRuntime;
using Foundation;

namespace StoreKit {

	partial class SKCloudServiceSetupOptions {

		public virtual SKCloudServiceSetupAction? Action {
			get {
				return (SKCloudServiceSetupAction?) (SKCloudServiceSetupActionExtensions.GetValue (_Action));
			}
			set {
				_Action = value != null ? SKCloudServiceSetupActionExtensions.GetConstant (value.Value) : null;
			}
		}
	}
}

#endif // __IOS__