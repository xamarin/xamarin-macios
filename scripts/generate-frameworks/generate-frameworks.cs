using System.IO;
using System.Text;

try {
	var actualArgumentCount = 5;
	if (args.Length != actualArgumentCount) {
		Console.WriteLine ($"Need {actualArgumentCount} arguments, got {args.Length} arguments");
		return 1;
	}

	var csharpOutput = args [0];

	args = args.Skip (1).ToArray ();

	var idx = 0;
	var iosframeworks = args [idx++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	var macosframeworks = args [idx++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	var tvosframeworks = args [idx++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	var maccatalystframeworks = args [idx++].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	var allframeworks = new string [] [] {
		iosframeworks,
		macosframeworks,
		tvosframeworks,
		maccatalystframeworks,
	};
	var names = new string [] {
		"iosframeworks",
		"macosframeworks",
		"tvosframeworks",
		"maccatalystframeworks",
	};

	var all = new HashSet<string> ();
	foreach (var fws in allframeworks)
		foreach (var fw in fws)
			all.Add (fw);

	var sb = new StringBuilder ();
	sb.AppendLine ("using System.Collections.Generic;");
	sb.AppendLine ();
	sb.AppendLine ("partial class Frameworks {");

	for (int i = 0; i < names.Length; i++) {
		var name = names [i];
		var frameworks = allframeworks [i];
		sb.AppendLine ($"\t// GENERATED FILE - DO NOT EDIT");
		sb.AppendLine ($"\tinternal readonly HashSet<string> {name} = new HashSet<string> {{");
		foreach (var fw in frameworks.OrderBy (v => v)) {
			sb.AppendLine ($"\t\t\"{fw}\",");
		}
		sb.AppendLine ("\t};");
	}

	var allArray = all.ToArray ();
	Array.Sort (allArray);
	foreach (var fw in allArray)
		sb.AppendLine ($"\tbool? _{fw.Replace (".", "")};");
	foreach (var fw in allArray)
		sb.AppendLine ($"\tpublic bool Have{fw} {{ get {{ if (!_{fw}.HasValue) _{fw} = GetValue (\"{fw}\"); return _{fw}.Value; }} }}");
	sb.AppendLine ("}");

	File.WriteAllText (csharpOutput, sb.ToString ());

	return 0;
} catch (Exception e) {
	Console.WriteLine ("Failed: {0}", e);
	return 1;
}
