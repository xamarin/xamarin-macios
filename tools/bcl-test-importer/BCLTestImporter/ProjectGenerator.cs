using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BCLTestImporter {
	public class ProjectGenerator {

		static readonly string NameKey = "%NAME%";
		static readonly string ReferencesKey = "%REFERENCES%";
		static readonly string RegisterTypeKey = "%REGISTER TYPE%";

		//list of reference that we are already adding, and we do not want to readd (although it is just a warning)
		static readonly List<string> excludeDlls = new List<string> {
			"mscorlib",
			"nunitlite",
			"System",
			"System.Xml",
			"System.Xml.Linq",
		};

		// creates the reference node
		static string GetReferenceNode (string assemblyName, string hintPath = null)
		{
			// lets not compliate our lifes with Xml, we just need to replace two things
			if (string.IsNullOrEmpty (hintPath)) {
				return $"<Reference Include=\"{assemblyName}\" />";
			} else {
				// the hint path is using unix separators, we need to use windows ones
				hintPath = hintPath.Replace ('/', '\\');
				var sb = new StringBuilder ();
				sb.AppendLine ($"<Reference Include=\"{assemblyName}\" >");
				sb.AppendLine ($"<HintPath>{hintPath}</HintPath>");
				sb.AppendLine ("</Reference>");
				return sb.ToString ();
			}
		}

		static string GetRegisterTypeNode (string registerPath)
		{
			var sb = new StringBuilder ();
			sb.AppendLine ($"<Compile Include=\"{registerPath}\">");
			sb.AppendLine ($"<Link>{Path.GetFileName (registerPath)}</Link>");
			sb.AppendLine ("</Compile>");
			return sb.ToString ();
		}
		public static string Generate (string projectName, string registerPath, List<(string assembly, string hintPath)> info, string templatePath)
		{
			var sb = new StringBuilder ();
			foreach (var assemblyInfo in info) {
				if (!excludeDlls.Contains (assemblyInfo.assembly))
				sb.AppendLine (GetReferenceNode (assemblyInfo.assembly, assemblyInfo.hintPath));
			}

			using (StreamReader reader = new StreamReader(templatePath))
			{
				string result = reader.ReadToEnd();
				result = result.Replace (NameKey, projectName);
				result = result.Replace (ReferencesKey, sb.ToString ());
				result = result.Replace (RegisterTypeKey, GetRegisterTypeNode (registerPath));
				return result;
			}
		}
	}
}
