using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.DotNet.XHarness.iOS.Shared.TestImporter.Templates.Managed {
	/// <summary>
	/// Class that knows how to generate the plist of a test project.
	/// </summary>
	public class InfoPlistGenerator {
		public static readonly string ApplicationNameReplacement = "%APPLICATION NAME%";
		public static readonly string IndentifierReplacement = "%BUNDLE INDENTIFIER%";
		public static readonly string WatchAppIndentifierReplacement = "%WATCHAPP INDENTIFIER%";

		public static string GenerateCode (string template, string projectName)
		{
			if (template == null)
				throw new ArgumentNullException (nameof (template));
			if (projectName == null)
				throw new ArgumentNullException (nameof (projectName));
			// got the lines we want to add, read the template and substitute
			var result = template;
			result = result.Replace (ApplicationNameReplacement, projectName);
			result = result.Replace (IndentifierReplacement, $"com.xamarin.bcltests.{projectName}");
			result = result.Replace (WatchAppIndentifierReplacement, $"com.xamarin.bcltests.{projectName}.container");
			return result;
		}
	}
}
