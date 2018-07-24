using System;
using System.IO;

class Program {
	static void Main ()
	{
		foreach (var file in Directory.GetFiles (".", "*.unclassified")) {
			var todo = Path.ChangeExtension (file, ".todo");
			if (File.Exists (todo)) {
				Console.WriteLine ($"Appending {file} to {todo}");
				var content = "## appended from unclassified file" + Environment.NewLine + File.ReadAllText (file);
				File.AppendAllText (todo, content);
				File.Delete (file);
			} else {
				Console.WriteLine ($"Moving {file} to {todo}");
				File.Move (file, todo);
			}
		}
	}
}
