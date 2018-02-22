#if XAMCORE_2_0
using System;
using System.Collections.Generic;
using XamCore.Foundation;

namespace XamCore.BusinessChat {
	public partial class BCChatAction {
		public static void OpenTranscript (string businessIdentifier, Dictionary<BCParameterName, string> intentParameters) {
			var keys = Array.ConvertAll<string, NSString> (new List<string>(intentParameters.Values).ToArray (), v => new NSString (v));
			var values = Array.ConvertAll<BCParameterName, NSString> (new List<BCParameterName> (intentParameters.Keys).ToArray (), k => k.GetConstant ());
			OpenTranscript (businessIdentifier, NSDictionary<NSString, NSString>.FromObjectsAndKeys (values, keys));
		}
	}
}
#endif
