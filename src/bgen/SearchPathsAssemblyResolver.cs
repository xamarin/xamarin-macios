using System;
using System.IO;
using System.Reflection;

#nullable enable

class SearchPathsAssemblyResolver : MetadataAssemblyResolver {
	readonly string [] libraryPaths;
	readonly string [] references;

	public SearchPathsAssemblyResolver (string [] libraryPaths, string [] references)
	{
		this.libraryPaths = libraryPaths;
		this.references = references;
	}

	public override Assembly? Resolve (MetadataLoadContext context, AssemblyName assemblyName)
	{
		string? name = assemblyName.Name;
		if (name is not null) {
			foreach (var asm in context.GetAssemblies ()) {
				if (asm.GetName ().Name == name)
					return asm;
			}

			string dllName = name + ".dll";
			foreach (var libraryPath in libraryPaths) {
				string path = Path.Combine (libraryPath, dllName);
				if (File.Exists (path)) {
					return context.LoadFromAssemblyPath (path);
				}
			}
			foreach (var reference in references) {
				if (Path.GetFileName (reference).Equals (dllName, StringComparison.OrdinalIgnoreCase)) {
					return context.LoadFromAssemblyPath (reference);
				}
			}
		}
		return null;
	}
}
