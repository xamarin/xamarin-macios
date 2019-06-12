using System;
using System.IO;

namespace BCLTestImporter {
	/// <summary>
	/// Class that knows how to generate the plist of a test project.
	/// </summary>
	public class BCLTestInfoPlistGenerator {
		internal static string ApplicationNameReplacement = "%APPLICATION NAME%";
		internal static string IndentifierReplacement = "%BUNDLE INDENTIFIER%";
		internal static string WatchAppIndentifierReplacement = "%WATCHAPP INDENTIFIER%";
	
		public static string GenerateCode (string templatePath, string projectName)
		{
			if (templatePath == null)
				throw new ArgumentNullException (nameof (templatePath));
			if (projectName == null)
				throw new ArgumentNullException (nameof (projectName));
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(templatePath)) {
				var result = reader.ReadToEnd ();
				result = result.Replace (ApplicationNameReplacement, projectName);
				result = result.Replace (IndentifierReplacement, $"com.xamarin.bcltests.{projectName}");
				result = result.Replace (WatchAppIndentifierReplacement, $"com.xamarin.bcltests.{projectName}.container");
				return result;
			}
		}
		
		public static void GenerateCodeToFile (string templatePath, string projectName, string destinationPath)
		{
			var plist = GenerateCode (templatePath, projectName);
			using (var file = new StreamWriter (destinationPath, false)) { // false is do not append
				file.Write (plist);
			}
		}

	}
}