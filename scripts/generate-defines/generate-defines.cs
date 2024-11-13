using System.IO;
using System.Text;

try {
	var expectedArgumentCount = 2;
	if (args.Length != expectedArgumentCount) {
		Console.WriteLine ($"Need {expectedArgumentCount} arguments, got {args.Length} arguments");
		return 1;
	}

	var output = args [0];
	var frameworks = args [1].Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

	var sb = new StringBuilder ();
	foreach (var fw in frameworks.OrderBy (v => v))
		sb.AppendLine ($"-d:HAS_{fw.ToUpperInvariant ()}");
	File.WriteAllText (output, sb.ToString ());

	return 0;
} catch (Exception e) {
	Console.WriteLine ("Failed: {0}", e);
	return 1;
}
