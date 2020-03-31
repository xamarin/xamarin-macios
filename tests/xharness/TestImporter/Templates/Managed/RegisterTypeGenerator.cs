using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Xharness.TestImporter.Templates.Managed {
	public static class RegisterTypeGenerator {

		static readonly string UsingReplacement = "%USING%";
		static readonly string KeysReplacement = "%KEY VALUES%";
		static readonly string IsxUnitReplacement = "%IS XUNIT%";

		public static async Task<string> GenerateCodeAsync ((string FailureMessage, Dictionary<string, Type> Types) typeRegistration, bool isXunit,
			Stream template)
		{
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			var namespaces = new List<string> ();  // keep track of the namespaces to remove warnings
			if (!string.IsNullOrEmpty (typeRegistration.FailureMessage)) {
				keyValuesStringBuilder.AppendLine ($"#error {typeRegistration.FailureMessage}");
			} else {
				foreach (var a in typeRegistration.Types.Keys) {
					var t = typeRegistration.Types [a];
					if (!string.IsNullOrEmpty (t.Namespace)) {
						if (!namespaces.Contains (t.Namespace)) {
							namespaces.Add (t.Namespace);
							importStringBuilder.AppendLine ($"using {t.Namespace};");
						}
						keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{a}\", typeof ({t.FullName})}}, ");
					}
				}
			}
			
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(template)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (UsingReplacement, importStringBuilder.ToString ());
				result = result.Replace (KeysReplacement, keyValuesStringBuilder.ToString ());
				result = result.Replace (IsxUnitReplacement, (isXunit)? "true" : "false");
				return result;
			}
		}
	}
}
