using System;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Utils;

namespace GenerateFrameworksConstants {
	class MainClass {
		public static int Main (string [] args)
		{
			var platform = args [0];
			var output = args [1];
			return Fix (GetPlatform (platform), output);
		}

		static ApplePlatform GetPlatform (string platform)
		{
			switch (platform.ToLowerInvariant ()) {
			case "ios":
				return ApplePlatform.iOS;
			case "tvos":
				return ApplePlatform.TVOS;
			case "watchos":
				return ApplePlatform.WatchOS;
			case "macos":
				return ApplePlatform.MacOSX;
			case "maccatalyst":
				return ApplePlatform.MacCatalyst;
			default:
				throw new NotImplementedException ($"Unknown platform: {platform}");
			}
		}

		static int Fix (ApplePlatform platform, string output)
		{
			var frameworks = Frameworks.GetFrameworks (platform, false).Values.Where (v => !v.Unavailable);
			var sb = new StringBuilder ();

#if NET
			sb.AppendLine ("#if NET");
#else
			sb.AppendLine ("#if !NET");
#endif
			sb.AppendLine ("namespace ObjCRuntime {");
			sb.AppendLine ("\tpublic static partial class Constants {");
			foreach (var grouped in frameworks.GroupBy (v => v.Version)) {
				sb.AppendLine ($"\t\t// {platform} {grouped.Key}");
				foreach (var fw in grouped.OrderBy (v => v.Name))
					sb.AppendLine ($"\t\tpublic const string {fw.Namespace}Library = \"{fw.LibraryPath}\";");
				sb.AppendLine ();
			}
			sb.AppendLine ("\t}");
			sb.AppendLine ("}");
#if NET
			sb.AppendLine ("#endif // NET");
#else
			sb.AppendLine ("#endif // !NET");
#endif

			File.WriteAllText (output, sb.ToString ());
			return 0;
		}
	}
}
