using System;
using System.Xml.Linq;
using System.IO;

namespace XamarinAnalysisDoc
{
	class MainClass
	{
		static string analysis_path;

		public static int Main (string [] args)
		{
			if (args.Length < 1) {
				Console.WriteLine ("Usage: mono xamarin-analysis-doc.exe /path/to/Xamarin.iOS.Analysis.targets");
				return 1;
			}

			analysis_path = args [0];

			if (!File.Exists (analysis_path)) {
				Console.WriteLine ($"Cannot find {analysis_path}");
				return 1;
			}

			GenerateAnalysisDoc (analysis_path);

			return 0;
		}

		static void GenerateAnalysisDoc (string path)
		{
			var section = "id:{c29b69f5-08e4-4dcc-831e-7fd692ab0886}\n";
			section += "title:Xamarin Analysis rules\n\n";
			section += "[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)\n";
			section += "[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)\n";
			section += "[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)\n\n";

			var root = XDocument.Load (path);

			XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

			foreach (XElement target in root.Descendants (ns + "Target")) {
				var nameAttr = target.Attribute ("Name").Value;
				var rule = nameAttr.Split ('_');
				var ruleID = rule [0];
				var ruleName = rule [1];

				section += $"<h3><a name=\"{ruleID}\"/>{ruleID}: {ruleName}</h3>\n\n";

				foreach (var xpaResult in target.Descendants (ns + "XamarinProjectAnalysisResult").Elements ()) {
					if (xpaResult.Name.LocalName != "Category") {
						section += "* **" + xpaResult.Name.LocalName + ":** ";
						section += xpaResult.Value + "\n";
					}
				}
				section += "\n";
			}

			File.WriteAllText ("xamarin-analysis.md", section);
		}
	}
}
