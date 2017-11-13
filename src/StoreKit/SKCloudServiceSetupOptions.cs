#if __IOS__

using System;
using XamCore.ObjCRuntime;
using XamCore.Foundation;

namespace XamCore.StoreKit {

	partial class SKCloudServiceSetupOptions {

		[iOS (10,1)]
		public virtual SKCloudServiceSetupAction? Action {
			get {
				return (SKCloudServiceSetupAction?) (SKCloudServiceSetupActionExtensions.GetValue (_Action));
			}
			set {
				if (value != null)
					_Action = SKCloudServiceSetupActionExtensions.GetConstant (value.Value);
				else
					throw new ArgumentNullException ("value");
			}
		}
	}
}

#endif // __IOS__