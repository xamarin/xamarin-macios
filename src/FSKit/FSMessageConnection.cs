#if NET

using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#nullable enable

namespace FSKit {
	public partial class FSMessageConnection {
		public NSString GetLocalizedMessage (NSString message, NSString tableName, NSBundle bundle, params NSObject[] arguments)
		{
			var argumentPtrs = new IntPtr [arguments.Length];
			for (var i = 0; i < arguments.Length; i++)
				argumentPtrs [i] = arguments [i].GetNonNullHandle ($"{nameof (arguments)} [{i}]");

			var rv = Messaging.objc_msgSend_5_vargs (
				this.GetNonNullHandle ("this"),
				Selector.GetHandle ("localizedMessage:table:bundle:"),
				message.GetNonNullHandle (nameof (message)),
				tableName.GetNonNullHandle (nameof (message)),
				bundle.GetNonNullHandle (nameof (bundle)),
				argumentPtrs);

			GC.KeepAlive (message);
			GC.KeepAlive (tableName);
			GC.KeepAlive (bundle);
			GC.KeepAlive (arguments);

			return Runtime.GetNSObject<NSString> (rv)!;
		}

		public string GetLocalizedMessage (string message, string tableName, NSBundle bundle, params NSObject[] arguments)
		{
			return (string) GetLocalizedMessage ((NSString) message, (NSString) tableName, bundle, arguments);
		}
	}
}
#endif // NET
