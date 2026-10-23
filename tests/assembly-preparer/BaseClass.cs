// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AssemblyPreparerTests;

public abstract class BaseClass {
	public void AssertPrepare (AssemblyPreparer preparer)
	{
		if (!preparer.Prepare (out var exceptions)) {
			foreach (var ex in exceptions) {
				Console.WriteLine (ex.ToString ());
				if (ex.InnerException is not null)
					Console.WriteLine ($"  Inner: {ex.InnerException}");
			}
			Assert.Fail ($"Prepare failed, exceptions:\n\t{string.Join ("\n\t", exceptions.Select (v => v.ToString ()))}");
		}
		Assert.That (exceptions, Is.Empty, "Exceptions");
	}

	public bool AssertPrepare (ApplePlatform platform, bool isCoreCLR, string code, out AssemblyDefinition assemblyDefinition, string? extraConfig = null)
	{
		return AssertPrepare (platform, isCoreCLR, RegistrarMode.Dynamic, code, out assemblyDefinition, extraConfig: extraConfig);
	}

	// returns true if the test assembly was modified
	public bool AssertPrepare (ApplePlatform platform, bool isCoreCLR, RegistrarMode registrar, string code, out AssemblyDefinition assemblyDefinition, bool hotReloadCompatibleBuild = false, string testAssemblyTrimMode = "link", string? inlineDlfcnMethods = null, string? extraConfig = null)
	{
		return AssertPrepare (platform, isCoreCLR, registrar, code, out assemblyDefinition, out _, hotReloadCompatibleBuild, testAssemblyTrimMode, inlineDlfcnMethods, extraConfig);
	}

	// Like the overload above, but also returns the AssemblyPreparer so tests can inspect state after
	// preparation (e.g. the collected native symbols in Configuration.DerivedLinkContext.RequiredSymbols).
	// returns true if the test assembly was modified
	public bool AssertPrepare (ApplePlatform platform, bool isCoreCLR, RegistrarMode registrar, string code, out AssemblyDefinition assemblyDefinition, out AssemblyPreparer preparer, bool hotReloadCompatibleBuild = false, string testAssemblyTrimMode = "link", string? inlineDlfcnMethods = null, string? extraConfig = null)
	{
		AssemblyPreparer? capturedPreparer = null;
		var rv = AssertPrepareCode (platform, isCoreCLR, p => {
			p.Registrar = registrar;
			capturedPreparer = p;
		}, code, out string outputPath, hotReloadCompatibleBuild, testAssemblyTrimMode, inlineDlfcnMethods, extraConfig);
		preparer = capturedPreparer!;
		var resolver = new DefaultAssemblyResolver ();
		var dirs = preparer!.Assemblies.Select (v => Path.GetDirectoryName (v.OutputPath)).Distinct ().ToList ();
		dirs.ForEach (v => resolver.AddSearchDirectory (v));
		var readerParameters = new ReaderParameters {
			ReadSymbols = true,
			AssemblyResolver = resolver,
		};
		assemblyDefinition = AssemblyDefinition.ReadAssembly (outputPath, readerParameters);
		return rv;
	}

	// Prepares the given code with the TrimmableStatic registrar and HotReloadCompatibleBuild
	// enabled, and returns the prepared user (Test) assembly + its companion (_Test.TypeMap) assembly.
	public void AssertPrepareHotReloadTrimmableStatic (ApplePlatform platform, bool isCoreCLR, string code, out bool userAssemblyWasSaved, out AssemblyDefinition userAssembly, out AssemblyDefinition companionAssembly)
	{
		Configuration.IgnoreIfIgnoredPlatform (platform);

		var csproj = $@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net$(BundledNETCoreAppTargetFrameworkVersion)-{platform.AsString ().ToLower ()}</TargetFramework>
        <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
		<UseFloatingTargetPlatformVersion>true</UseFloatingTargetPlatformVersion>
	</PropertyGroup>
</Project>
    ";

		var tmpdir = Xamarin.Cache.CreateTemporaryDirectory ();
		var assemblyDir = Path.Combine (tmpdir, "bin", "Debug");
		var outDir = Path.Combine (assemblyDir, "out");
		Directory.CreateDirectory (outDir);

		// AssemblyName points to the platform assembly (the entry assembly) so that the root type
		// map (which TrimmableRegistrarStep adds to the entry assembly on .NET <= 10) doesn't land
		// in the Test assembly - that keeps the Test assembly's modifications limited to what
		// ManagedRegistrarStep would emit, which is exactly what we're verifying gets relocated.
		var config = $@"
		AreAnyAssembliesTrimmed=true
		PublishTrimmed=true
		AssemblyName=Microsoft.{platform.AsString ()}.dll
		HotReloadCompatibleBuild=true
		IntermediateOutputPath={Path.Combine (tmpdir, "intermediate")}
		Platform={platform.AsString ()}
		PlatformAssembly=Microsoft.{platform.AsString ()}.dll
		SdkDevPath={Configuration.XcodeLocation}
		SdkVersion={Configuration.GetSdkVersion (platform)}
		TargetFramework={TargetFramework.GetTargetFramework (platform)}
		TypeMapAssemblyName=_TypeMap
		TypeMapOutputDirectory={outDir}
		UnmanagedCallersOnlyMapPath={Path.Combine (tmpdir, "uco.txt")}
		";
		var configpath = Path.Combine (tmpdir, "config.txt");
		File.WriteAllText (configpath, config);

		File.WriteAllText (Path.Combine (tmpdir, "Test.cs"), code);
		var csprojPath = Path.Combine (tmpdir, "Test.csproj");
		File.WriteAllText (csprojPath, csproj);
		var properties = new Dictionary<string, string> {
			{ "TreatWarningsAsErrors", "false" },
		};
		DotNet.AssertBuild (csprojPath, properties);

		var assemblies = Configuration.GetImplementationAssemblies (platform, isCoreCLR);
		assemblies.Add (Path.Combine (assemblyDir, "Test.dll"));
		var infos = assemblies.Select (v => new AssemblyPreparerInfo (v, Path.Combine (outDir, Path.GetFileName (v)), true, Path.GetFileNameWithoutExtension (v) == "Test" ? "copy" : "link")).ToArray ();
		var logger = new TestLogger () { Platform = platform };
		var preparer = new AssemblyPreparer (logger, infos, configpath);
		preparer.Registrar = RegistrarMode.TrimmableStatic;

		var testInfo = infos.Single (v => Path.GetFileNameWithoutExtension (v.InputPath) == "Test");
		// The assembly-preparer resets an assembly's OutputPath back to its InputPath when it decides
		// the assembly doesn't need to be re-serialized (see SaveAssembliesStep.OutputWithoutRewriting),
		// so capture the intended output path up front to reliably detect whether the user assembly was
		// actually written to the output directory.
		var expectedUserOutputPath = testInfo.OutputPath;

		AssertPrepare (preparer);

		var companionPath = Path.Combine (outDir, "_Test.TypeMap.dll");
		Assert.That (File.Exists (companionPath), Is.True, $"Companion assembly should exist at {companionPath}");
		preparer.Dispose ();

		var resolver = new DefaultAssemblyResolver ();
		resolver.AddSearchDirectory (outDir);
		var readerParameters = new ReaderParameters {
			ReadSymbols = false,
			AssemblyResolver = resolver,
		};
		// When the trampolines are relocated, the user assembly is left byte-unmodified, so it's
		// never re-saved to the output directory. Read it back from wherever it actually is.
		userAssemblyWasSaved = File.Exists (expectedUserOutputPath);
		var userAssemblyPath = userAssemblyWasSaved ? expectedUserOutputPath : testInfo.InputPath;
		userAssembly = AssemblyDefinition.ReadAssembly (userAssemblyPath, readerParameters);
		companionAssembly = AssemblyDefinition.ReadAssembly (companionPath, readerParameters);
	}

	// returns true if the test assembly was modified
	public bool AssertPrepareCode (ApplePlatform platform, bool isCoreCLR, Action<AssemblyPreparer>? configure, string code, out string outputPath, bool hotReloadCompatibleBuild = false, string testAssemblyTrimMode = "link", string? inlineDlfcnMethods = null, string? extraConfig = null, string extraCsproj = "")
	{
		using var preparer = CreatePreparer (platform, isCoreCLR, configure, code, out var testInfo, hotReloadCompatibleBuild: hotReloadCompatibleBuild, testAssemblyTrimMode: testAssemblyTrimMode, inlineDlfcnMethods: inlineDlfcnMethods, extraConfig: extraConfig ?? "", extraCsproj: extraCsproj);
		AssertPrepare (preparer);

		outputPath = testInfo.OutputPath;
		Console.WriteLine ("Output assembly: " + outputPath);
		return testInfo.InputPath != testInfo.OutputPath;
	}

	// Builds the provided code into a Test.dll and returns an AssemblyPreparer configured for it, without
	// running any preparation steps. Use this when a test needs to run a custom set of steps.
	public AssemblyPreparer CreatePreparer (ApplePlatform platform, bool isCoreCLR, Action<AssemblyPreparer>? configure, string code, out AssemblyPreparerInfo testInfo, string extraCsproj = "", string extraConfig = "", IEnumerable<(string FileName, byte [] Content)>? extraFiles = null, bool hotReloadCompatibleBuild = false, string testAssemblyTrimMode = "link", string? inlineDlfcnMethods = null, bool testAssemblyHasOriginalInputPath = false)
	{
		Configuration.IgnoreIfIgnoredPlatform (platform);

		var csproj = $@"
<Project Sdk=""Microsoft.NET.Sdk"">
	<PropertyGroup>
		<TargetFramework>net$(BundledNETCoreAppTargetFrameworkVersion)-{platform.AsString ().ToLower ()}</TargetFramework>
        <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
		<UseFloatingTargetPlatformVersion>true</UseFloatingTargetPlatformVersion>
	</PropertyGroup>
	{extraCsproj}
</Project>
    ";

		var tmpdir = Xamarin.Cache.CreateTemporaryDirectory ();

		if (extraFiles is not null) {
			foreach (var extraFile in extraFiles)
				File.WriteAllBytes (Path.Combine (tmpdir, extraFile.FileName), extraFile.Content);
		}

		var config = $@"
		AreAnyAssembliesTrimmed=true
		PublishTrimmed=true
		HotReloadCompatibleBuild={(hotReloadCompatibleBuild ? "true" : "false")}
		IntermediateOutputPath={Path.Combine (tmpdir, "intermediate")}
		Platform={platform.AsString ()}
		PlatformAssembly=Microsoft.{platform.AsString ()}.dll
		SdkDevPath={Configuration.XcodeLocation}
		SdkVersion={Configuration.GetSdkVersion (platform)}
		TargetFramework={TargetFramework.GetTargetFramework (platform)}
		{(inlineDlfcnMethods is null ? "" : $"InlineDlfcnMethods={inlineDlfcnMethods}")}
		{extraConfig}
		";
		var configpath = Path.Combine (tmpdir, "config.txt");
		File.WriteAllText (configpath, config);

		File.WriteAllText (Path.Combine (tmpdir, "Test.cs"), code);
		var csprojPath = Path.Combine (tmpdir, "Test.csproj");
		File.WriteAllText (csprojPath, csproj);
		var properties = new Dictionary<string, string> {
			{ "TreatWarningsAsErrors", "false" },
		};
		DotNet.AssertBuild (csprojPath, properties);
		var assemblyDir = Path.Combine (tmpdir, "bin", "Debug");

		var assemblies = Configuration.GetImplementationAssemblies (platform, isCoreCLR);
		assemblies.Add (Path.Combine (assemblyDir, "Test.dll"));
		var infos = assemblies.Select (v => {
			// The test assembly can be built as a reloadable (Copy) assembly to exercise the Hot Reload code paths.
			var isTestAssembly = Path.GetFileNameWithoutExtension (v) == "Test";
			var trimMode = isTestAssembly ? testAssemblyTrimMode : "link";
			string? originalInputPath = null;
			if (isTestAssembly && testAssemblyHasOriginalInputPath) {
				originalInputPath = Path.Combine (assemblyDir, "original", Path.GetFileName (v));
				var originalInputDirectory = Path.GetDirectoryName (originalInputPath);
				if (originalInputDirectory is null)
					throw new InvalidOperationException ($"Could not get the directory name for '{originalInputPath}'.");
				Directory.CreateDirectory (originalInputDirectory);
				File.Copy (v, originalInputPath);
			}
			return new AssemblyPreparerInfo (v, Path.Combine (assemblyDir, "out", Path.GetFileName (v)), originalInputPath, true, trimMode);
		}).ToArray ();
		var logger = new TestLogger () { Platform = platform };
		var preparer = new AssemblyPreparer (logger, infos, configpath);
		if (configure is not null)
			configure (preparer);

		testInfo = infos.Single (v => Path.GetFileNameWithoutExtension (v.InputPath) == "Test");
		return preparer;
	}
}
