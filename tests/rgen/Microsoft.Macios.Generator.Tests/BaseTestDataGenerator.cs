using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Macios.Generator.Tests;

public class BaseTestDataGenerator {
	public static string ReadFileAsString (string file, [CallerFilePath] string filePath = "")
	{
		var directoryPath = Path.GetDirectoryName (filePath);
		var fullPath = Path.Join (directoryPath, "Data", file);
		return File.ReadAllText (fullPath);
	}
}
