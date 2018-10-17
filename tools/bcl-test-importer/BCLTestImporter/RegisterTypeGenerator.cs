using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

namespace BCLTestImporter {
	public static class RegisterTypeGenerator {

		static string UsingReplacement = "%USING%";
		static string KeysReplacement = "%KEY VALUES%";
		static string IsxUnitReplacement = "%IS XUNIT%";

		public static async Task<string> GenerateCodeAsync (Dictionary<string, Type> typeRegistration, bool isXunit,
			string templatePath)
		{
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			var namespaces = new List<string> ();  // keep track of the namespaces to remove warnings
			foreach (var a in typeRegistration.Keys) {
				var t = typeRegistration [a];
				if (!string.IsNullOrEmpty (t.Namespace)) {
					if (!namespaces.Contains (t.Namespace)) {
						namespaces.Add (t.Namespace);
						importStringBuilder.AppendLine ($"using {t.Namespace};");
					}
					keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{a}\", typeof ({t.FullName})}}, ");
				}
			}
			
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (UsingReplacement, importStringBuilder.ToString ());
				result = result.Replace (KeysReplacement, keyValuesStringBuilder.ToString ());
				result = result.Replace (IsxUnitReplacement, (isXunit)? "true" : "false");
				return result;
			}
		}
		
		// Generates the code for the type registration using the give path to the template to use
		public static string GenerateCode (Dictionary <string, Type> typeRegistration, bool isXunit, string templatePath) => GenerateCodeAsync (typeRegistration, isXunit, templatePath).Result;
	}
}
