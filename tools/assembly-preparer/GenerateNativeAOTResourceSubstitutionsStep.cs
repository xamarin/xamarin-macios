// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;
using System.Xml.Linq;

using Mono.Cecil;

using Xamarin.Bundler;
using Xamarin.Linker;

#nullable enable

namespace Xamarin.Build {
	public class GenerateNativeAOTResourceSubstitutionsStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Generate NativeAOT Resource Substitutions";
		protected override int ErrorCode { get; } = 2570;

		readonly XElement linker = new XElement ("linker");

		protected override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return App.XamarinRuntime == XamarinRuntime.NativeAOT && assembly.MainModule.HasResources
				&& !App.Profile.IsProductAssembly (assembly) && !App.Profile.IsSdkAssembly (assembly);
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			var libraries = Xamarin.Bundler.Assembly.GetNativeLibraryResourceNames (assembly);
			var resources = assembly.MainModule.Resources.OfType<EmbeddedResource> ().Where (v => libraries.Contains (v.Name)).ToArray ();
			if (resources.Length == 0)
				return;

			linker.Add (new XElement ("assembly",
				new XAttribute ("fullname", assembly.Name.Name),
				resources.Select (v => new XElement ("resource",
					new XAttribute ("name", v.Name),
					new XAttribute ("action", "remove")))));
		}

		protected override void TryEndProcess ()
		{
			if (App.XamarinRuntime != XamarinRuntime.NativeAOT)
				return;

			var substitutionsFile = Path.Combine (Configuration.IntermediateOutputPath, "nativeaot-resource-substitutions.xml");
			Directory.CreateDirectory (Configuration.IntermediateOutputPath);
			new XDocument (linker).Save (substitutionsFile);
		}
	}
}
