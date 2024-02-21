using System;
using System.Runtime.Versioning;

#nullable enable

namespace Foundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public sealed class NSExceptionError : NSError {
		Exception exception;

		public Exception Exception { get => exception; }

		public NSExceptionError (Exception exception)
			: base ((NSString) exception.GetType ().FullName, exception.HResult, GetDictionary (exception))
		{
			this.exception = exception;
			IsDirectBinding = false;
		}

		static NSDictionary GetDictionary (Exception e)
		{
			var dict = new NSMutableDictionary ();
			dict [NSError.LocalizedDescriptionKey] = (NSString) e.Message;
			dict [NSError.LocalizedFailureReasonErrorKey] = (NSString) e.Message;
			return dict;
		}
	}
}
