// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Build.Framework;

using Mono.Cecil;

using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class GenerateNativeAOTResourceSubstitutions : XamarinTask {
		[Required]
		public ITaskItem [] Assemblies { get; set; } = [];

		[Required]
		public string SubstitutionsFile { get; set; } = "";

		public override bool Execute ()
		{
			using var resolver = new DefaultAssemblyResolver ();
			foreach (var item in Assemblies)
				resolver.AddSearchDirectory (Path.GetDirectoryName (item.GetMetadata ("FullPath")));

			var linker = new XElement ("linker");
			foreach (var item in Assemblies) {
				using var assembly = AssemblyDefinition.ReadAssembly (item.ItemSpec, new ReaderParameters { AssemblyResolver = resolver });
				if (!assembly.MainModule.HasResources)
					continue;
				var libraries = Xamarin.Bundler.Assembly.GetNativeLibraryResourceNames (assembly);
				var resources = assembly.MainModule.Resources.OfType<EmbeddedResource> ().Where (v => libraries.Contains (v.Name)).ToArray ();
				if (resources.Length == 0)
					continue;

				linker.Add (new XElement ("assembly",
					new XAttribute ("fullname", assembly.Name.Name),
					resources.Select (v => new XElement ("resource",
						new XAttribute ("name", v.Name),
						new XAttribute ("action", "remove")))));
			}

			FileUtils.UpdateFile (SubstitutionsFile, path => new XDocument (linker).Save (path));
			return !Log.HasLoggedErrors;
		}
	}
}
