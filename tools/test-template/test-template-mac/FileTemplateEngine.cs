using System;
using System.IO;
using Xamarin.Bundler;

namespace Xamarin.Tests.Templating
{
	public class FileTemplateEngine
	{
		string SourceDirectory;
		string OutputDirectory;

		public FileTemplateEngine (string sourceDirectory, string outputDirectory)
		{
			SourceDirectory = sourceDirectory;
			OutputDirectory = outputDirectory;
		}

		public static FileTemplateEngine WithSubdirectory (string sourceDirectory, string outputDirectory, string subDirectory)
		{
			return new FileTemplateEngine (Path.Combine (sourceDirectory, subDirectory), Path.Combine (outputDirectory, subDirectory));
		}

		public string CopyFile (string fileName)
		{
			string srcPath = Path.Combine (SourceDirectory, fileName);
			string destPath = Path.Combine (OutputDirectory, fileName);

			string text = File.ReadAllText (srcPath);
			File.WriteAllText (destPath, text);
			return destPath;
		}

		public string CopyFileWithSubstitutions (string fileName, IReplacement replacement, string destFileName = null)
		{
			string srcPath = Path.Combine (SourceDirectory, fileName);
			string destPath = Path.Combine (OutputDirectory, destFileName ?? fileName);

			string text = replacement.Apply (File.ReadAllText (srcPath));
			File.WriteAllText (destPath, text);
			return destPath;
		}

		public string CopyTextWithSubstitutions (string text, string fileName, IReplacement replacement)
		{
			string destPath = Path.Combine (OutputDirectory, fileName);
			File.WriteAllText (destPath, replacement.Apply (text));
			return destPath;
		}

		public void CopyDirectory (string sourceName)
		{
			string srcPath = Path.Combine (SourceDirectory, sourceName);

			int ret = Invoker.RunCommand ("/bin/cp", $"-R {Path.Combine (SourceDirectory, sourceName)} {OutputDirectory}");
			if (ret != 0)
				throw new InvalidOperationException ($"CopyDirectory from {srcPath} to {OutputDirectory} failed with {ret} return code.");
		}
	}
}
