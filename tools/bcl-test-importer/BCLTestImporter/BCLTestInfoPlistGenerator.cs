using System;
using System.IO;
using System.Threading.Tasks;

namespace BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate the plist of a test project.
	/// </summary>
	public class BCLTestInfoPlistGenerator {
		internal static string ApplicationNameReplacement = "%APPLICATION NAME%";
		internal static string IndentifierReplacement = "%BUNDLE INDENTIFIER%";
		internal static string WatchAppIndentifierReplacement = "%WATCHAPP INDENTIFIER%";
	
		public static async Task<string> GenerateCodeAsync (string templatePath, string projectName)
		{
			if (templatePath == null)
				throw new ArgumentNullException (nameof (templatePath));
			if (projectName == null)
				throw new ArgumentNullException (nameof (projectName));
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (ApplicationNameReplacement, projectName);
				result = result.Replace (IndentifierReplacement, $"com.xamarin.bcltests.{projectName}");
				result = result.Replace (WatchAppIndentifierReplacement, $"com.xamarin.bcltests.{projectName}.container");
				return result;
			}
		}

		// Generates the code for the type registration using the give path to the template to use
		public static string GenerateCode (string templatePath, string projectName) => GenerateCodeAsync (templatePath, projectName).Result;
	}
}