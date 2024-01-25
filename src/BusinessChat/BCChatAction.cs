#nullable enable

using System;
using System.Collections.Generic;

using Foundation;

namespace BusinessChat {
	public partial class BCChatAction {
		public static void OpenTranscript (string businessIdentifier, Dictionary<BCParameterName, string> intentParameters)
		{
			var keys = new NSString [intentParameters.Keys.Count];
			var values = new NSString [intentParameters.Keys.Count];
			var index = 0;
			foreach (var k in intentParameters.Keys) {
				if (k.GetConstant () is NSString s) {
					keys [index] = s;
					values [index] = new NSString (intentParameters [k]);
					index++;
				}
			}
			using (var dict = NSDictionary<NSString, NSString>.FromObjectsAndKeys (values, keys, keys.Length))
				OpenTranscript (businessIdentifier, dict);
		}
	}
}
