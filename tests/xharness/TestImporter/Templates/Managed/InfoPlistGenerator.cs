using System;

namespace Xharness.TestImporter.Templates.Managed {
	/// <summary>
	/// Class that knows how to generate the plist of a test project.
	/// </summary>
	public class InfoPlistGenerator {
		public static readonly string ApplicationNameReplacement = "%APPLICATION NAME%";
		public static readonly string IndentifierReplacement = "%BUNDLE INDENTIFIER%";
		public static readonly string WatchAppIndentifierReplacement = "%WATCHAPP INDENTIFIER%";
		public static readonly string MinOSVersionReplacement = "%MINOSVERSION%";

		public static string GenerateCode (Platform platform, string template, string projectName)
		{
			if (template is null)
				throw new ArgumentNullException (nameof (template));
			if (projectName is null)
				throw new ArgumentNullException (nameof (projectName));
			// got the lines we want to add, read the template and substitute
			var result = template;
			result = result.Replace (ApplicationNameReplacement, projectName);
			result = result.Replace (IndentifierReplacement, $"com.xamarin.bcltests.{projectName}");
			result = result.Replace (WatchAppIndentifierReplacement, $"com.xamarin.bcltests.{projectName}.container");
			result = result.Replace (MinOSVersionReplacement, platform.GetMinOSVersion ());
			return result;
		}
	}
}
