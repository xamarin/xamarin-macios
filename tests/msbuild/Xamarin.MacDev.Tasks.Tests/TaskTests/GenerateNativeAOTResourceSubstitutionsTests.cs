// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Build.Utilities;

using Mono.Cecil;

using NUnit.Framework;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class GenerateNativeAOTResourceSubstitutionsTests : TestBase {
		static void AddLinkWith (AssemblyDefinition assembly, string libraryName, string attributeNamespace = "ObjCRuntime")
		{
			var module = assembly.MainModule;
			var type = new TypeReference (attributeNamespace, "LinkWithAttribute", module, module.TypeSystem.CoreLibrary);
			var constructor = new MethodReference (".ctor", module.TypeSystem.Void, type) { HasThis = true };
			constructor.Parameters.Add (new ParameterDefinition (module.TypeSystem.String));
			var attribute = new CustomAttribute (constructor);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (module.TypeSystem.String, libraryName));
			assembly.CustomAttributes.Add (attribute);
		}

		[Test]
		public void OnlyNativeLibraryResourcesAreExcluded ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var assemblyPath = Path.Combine (directory, "Binding.dll");
			using var assembly = AssemblyDefinition.CreateAssembly (new AssemblyNameDefinition ("Binding", new Version (1, 0)), "Binding", ModuleKind.Dll);
			var nativeResources = new [] { "libNative & Helpers.a", "Native.framework", "MixedCase.A" };
			var managedResources = new [] { "Strings.resources", "Unrelated.a", "__monotouch_content_image.png" };
			foreach (var name in nativeResources.Concat (managedResources))
				assembly.MainModule.Resources.Add (new EmbeddedResource (name, ManifestResourceAttributes.Public, new byte [] { 1, 2, 3 }));
			foreach (var name in nativeResources)
				AddLinkWith (assembly, name.ToLowerInvariant ());
			AddLinkWith (assembly, nativeResources [0]);
			AddLinkWith (assembly, "Missing.a");
			AddLinkWith (assembly, "Unrelated.a", "Unrelated");
			assembly.Write (assemblyPath);
			var originalBytes = File.ReadAllBytes (assemblyPath);

			var task = CreateTask<GenerateNativeAOTResourceSubstitutions> ();
			task.Assemblies = new [] { new TaskItem (assemblyPath) };
			task.SubstitutionsFile = Path.Combine (directory, "intermediate", "substitutions.xml");
			ExecuteTask (task);

			var document = XDocument.Load (task.SubstitutionsFile);
			var assemblyElement = document.Descendants ("assembly").Single ();
			Assert.That ((string?) assemblyElement.Attribute ("fullname"), Is.EqualTo ("Binding"), "Assembly name");
			Assert.That (assemblyElement.Elements ("resource").Select (v => (string?) v.Attribute ("name")), Is.EquivalentTo (nativeResources), "Excluded resources");
			Assert.That (assemblyElement.Elements ("resource").Select (v => (string?) v.Attribute ("action")), Is.All.EqualTo ("remove"), "Resource actions");
			Assert.That (File.ReadAllBytes (assemblyPath), Is.EqualTo (originalBytes), "Binding must remain unchanged for native extraction");

			var timestamp = File.GetLastWriteTimeUtc (task.SubstitutionsFile);
			ExecuteTask (task);
			Assert.That (File.GetLastWriteTimeUtc (task.SubstitutionsFile), Is.EqualTo (timestamp), "Unchanged substitutions must not be rewritten");

			task.Assemblies = [];
			ExecuteTask (task);
			Assert.That (XDocument.Load (task.SubstitutionsFile).Descendants ("assembly"), Is.Empty, "Removed references must not leave stale exclusions");
		}

		[Test]
		public void AssemblyWithoutNativeResources ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var assemblyPath = Path.Combine (directory, "Managed.dll");
			using var assembly = AssemblyDefinition.CreateAssembly (new AssemblyNameDefinition ("Managed", new Version (1, 0)), "Managed", ModuleKind.Dll);
			assembly.MainModule.Resources.Add (new EmbeddedResource ("Strings.resources", ManifestResourceAttributes.Public, new byte [] { 1 }));
			assembly.Write (assemblyPath);

			var task = CreateTask<GenerateNativeAOTResourceSubstitutions> ();
			task.Assemblies = new [] { new TaskItem (assemblyPath) };
			task.SubstitutionsFile = Path.Combine (directory, "substitutions.xml");
			ExecuteTask (task);
			Assert.That (XDocument.Load (task.SubstitutionsFile).Descendants ("assembly"), Is.Empty, "Managed resources must not be excluded");
		}
	}
}
