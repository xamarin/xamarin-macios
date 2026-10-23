// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Xml.Linq;

using MonoTouch.Tuner;

namespace AssemblyPreparerTests;

public class GenerateNativeAOTResourceSubstitutionsStepTests : BaseClass {
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

	[TestCase ("link")]
	[TestCase ("copy")]
	public void OnlyNativeLibraryResourcesAreExcluded (string trimMode)
	{
		using var preparer = CreatePreparer (ApplePlatform.iOS, false, null, "class MyClass {}", out var testInfo, extraConfig: "XamarinRuntime=NativeAOT", testAssemblyTrimMode: trimMode);
		var context = preparer.Configuration.DerivedLinkContext;
		new LoadAssembliesStep ().Process (context);
		var assembly = context.GetAssemblies ().Single (v => v.Name.Name == "Test");
		var nativeResources = new [] { "libNative & Helpers.a", "Native.framework", "MixedCase.A" };
		var managedResources = new [] { "Strings.resources", "Unrelated.a", "__monotouch_content_image.png" };
		foreach (var name in nativeResources.Concat (managedResources))
			assembly.MainModule.Resources.Add (new EmbeddedResource (name, ManifestResourceAttributes.Public, new byte [] { 1, 2, 3 }));
		foreach (var name in nativeResources)
			AddLinkWith (assembly, name.ToLowerInvariant ());
		AddLinkWith (assembly, nativeResources [0]);
		AddLinkWith (assembly, "Missing.a");
		AddLinkWith (assembly, "Unrelated.a", "Unrelated");
		var originalBytes = File.ReadAllBytes (testInfo.InputPath);

		new GenerateNativeAOTResourceSubstitutionsStep ().Process (context);
		Assert.That (preparer.Configuration.Exceptions, Is.Empty, "Step exceptions");
		var substitutionsPath = Path.Combine (preparer.IntermediateOutputPath, "nativeaot-resource-substitutions.xml");
		var document = XDocument.Load (substitutionsPath);
		var assemblyElement = document.Descendants ("assembly").Single ();
		Assert.That ((string?) assemblyElement.Attribute ("fullname"), Is.EqualTo ("Test"), "Assembly name");
		Assert.That (assemblyElement.Elements ("resource").Select (v => (string?) v.Attribute ("name")), Is.EquivalentTo (nativeResources), "Excluded resources");
		Assert.That (assemblyElement.Elements ("resource").Select (v => (string?) v.Attribute ("action")), Is.All.EqualTo ("remove"), "Resource actions");
		Assert.That (assembly.MainModule.Resources.Select (v => v.Name), Is.EquivalentTo (nativeResources.Concat (managedResources)), "Resources must remain available for extraction");
		Assert.That (File.ReadAllBytes (testInfo.InputPath), Is.EqualTo (originalBytes), "Input assembly must remain unchanged");
		Assert.That (preparer.Configuration.ModifiedAssemblies, Is.Empty, "The step must not rewrite assemblies");

		assembly.MainModule.Resources.Clear ();
		new GenerateNativeAOTResourceSubstitutionsStep ().Process (context);
		Assert.That (XDocument.Load (substitutionsPath).Descendants ("assembly"), Is.Empty, "Removed resources must not leave stale exclusions");
	}

	[TestCase (XamarinRuntime.NativeAOT)]
	[TestCase (XamarinRuntime.CoreCLR)]
	public void AssemblyWithoutNativeResources (XamarinRuntime runtime)
	{
		using var preparer = CreatePreparer (ApplePlatform.iOS, false, null, "class MyClass {}", out _, extraConfig: $"XamarinRuntime={runtime}");
		var context = preparer.Configuration.DerivedLinkContext;
		new LoadAssembliesStep ().Process (context);
		var assembly = context.GetAssemblies ().Single (v => v.Name.Name == "Test");
		assembly.MainModule.Resources.Add (new EmbeddedResource ("Strings.resources", ManifestResourceAttributes.Public, new byte [] { 1 }));
		new GenerateNativeAOTResourceSubstitutionsStep ().Process (context);
		Assert.That (preparer.Configuration.Exceptions, Is.Empty, "Step exceptions");
		var substitutionsPath = Path.Combine (preparer.IntermediateOutputPath, "nativeaot-resource-substitutions.xml");
		if (runtime == XamarinRuntime.NativeAOT) {
			Assert.That (XDocument.Load (substitutionsPath).Descendants ("assembly"), Is.Empty, "Managed resources must not be excluded");
		} else {
			Assert.That (substitutionsPath, Does.Not.Exist, "Other runtimes must not generate ILC substitutions");
		}
	}
}
