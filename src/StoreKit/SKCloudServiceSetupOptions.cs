#nullable enable

#if __IOS__

using System;
using ObjCRuntime;
using Foundation;

namespace StoreKit {

	partial class SKCloudServiceSetupOptions {

		public virtual SKCloudServiceSetupAction? Action {
			get {
				if (_Action is null)
					return null;
				return (SKCloudServiceSetupAction?) (SKCloudServiceSetupActionExtensions.GetValue (_Action));
			}
			set {
				_Action = value is not null ? SKCloudServiceSetupActionExtensions.GetConstant (value.Value) : null;
			}
		}
	}
}

#endif // __IOS__
