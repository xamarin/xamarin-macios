using Foundation;

namespace AppKit {
	partial class NSResponder {
#if !XAMCORE_2_0
		public NSObject ValidRequestorForSendType (string sendType, string returnType)
		{
			return ValidRequestorForSendTypereturnType (sendType, returnType);
		}
#endif
	}
}