using System;
namespace Xharness.BCLTestImporter.Templates {

	// interface that will help locate the test assemblies that are used to create the apps. This way, we can
	// point to a specific location from which the asseblies well be referenced. The idea is to allow to download
	// precompiled assemblies and use them, for example, download the precompiled bcl from mono.
	public interface IAssemblyLocator {
		/// <summary>
		/// Get the location of the assemblies to use as references. Assemblies depend on the platform targeted.
		/// </summary>
		/// <param name="platform">The platform whose asseblies we want to use.</param>
		/// <returns>The root dir in which the precompiled assemblies can be found.</returns>
		string GetAssembliesRootLocation (Platform platform);
	}
}
