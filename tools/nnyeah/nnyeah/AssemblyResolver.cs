using System.IO;
using Mono.Cecil;


namespace Microsoft.MaciOS.Nnyeah {
	public class NNyeahAssemblyResolver : DefaultAssemblyResolver {
		public NNyeahAssemblyResolver (string infile, string xamarinAssembly) : base ()
		{
			// When resolving assemblies, look next to the platform assembly AND
			// next to the input file.
			// This must be done in a custom resolver to resolve type information
			// such as the base type from types we are processing
			// so we can answer questions like "Does this type derive from NSObject"
			// from any number of inheritance levels
			this.AddSearchDirectory (Path.GetDirectoryName (infile));
			this.AddSearchDirectory (Path.GetDirectoryName (xamarinAssembly));
		}
	}
}
