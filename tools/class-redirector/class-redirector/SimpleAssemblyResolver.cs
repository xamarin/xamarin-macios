using System.IO;
using Mono.Cecil;

#nullable enable

namespace ClassRedirector {
	public class SimpleAssemblyResolver : DefaultAssemblyResolver {
		public SimpleAssemblyResolver (params string [] filesOrDirectories)
			: base ()
		{
			foreach (var fileOrDirectory in filesOrDirectories) {
				if (File.Exists (fileOrDirectory)) {
					AddSearchDirectory (Path.GetDirectoryName (fileOrDirectory));
				} else if (Directory.Exists (fileOrDirectory)) {
					AddSearchDirectory (fileOrDirectory);
				}
			}
		}
	}
}

