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
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (_Action));
				return (SKCloudServiceSetupAction?) (SKCloudServiceSetupActionExtensions.GetValue (_Action!));
			}
			set {
				_Action = value != null ? SKCloudServiceSetupActionExtensions.GetConstant (value.Value) : null;
			}
		}
	}
}

#endif // __IOS__
