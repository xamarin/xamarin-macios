using System;
using System.IO;

using Extrospection;

class Program {
	static void Main (string [] args)
	{
		var dir = args.Length == 0 ? "." : args [0];
		foreach (var file in Directory.GetFiles (dir, "*.unclassified")) {
			var last = file.LastIndexOf ('-');
			var fx = file.Substring (last + 1, file.Length - last - 14);
			if (Helpers.Filter (fx)) {
				Console.WriteLine ($"{fx} is ignored, skipping...");
				continue;
			}

			var todo = Path.ChangeExtension (file, ".todo");
			if (File.Exists (todo)) {
				Console.WriteLine ($"Appending {file} to {todo}");
				var content = File.ReadAllText (file);
				File.AppendAllText (todo, content);
				File.Delete (file);
			} else {
				Console.WriteLine ($"Moving {file} to {todo}");
				File.Move (file, todo);
			}
		}
	}
}
