using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Utilities;

using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class BGenTaskTests : TestBase {
		[Test]
		public void StandardCommandline ()
		{
			var task = CreateTask<BGen> ();

			task.CompiledApiDefinitionAssembly = new TaskItem ("compiled-api-definitions.dll");
			task.GeneratedSourcesFileList = Path.Combine (Cache.CreateTemporaryDirectory (), "generated-sources.txt");
			task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b.dll"), new TaskItem ("c.dll") };
			task.ResponseFilePath = Path.Combine (Cache.CreateTemporaryDirectory (), "response-file.txt");

			var args = task.GenerateCommandLineArguments ();
			args.AddRange (File.ReadAllLines (task.ResponseFilePath));
			Assert.That (args, Does.Contain ("/compiled-api-definition-assembly:compiled-api-definitions.dll"));
			Assert.That (args, Does.Contain ("/sourceonly:" + Path.GetFullPath (task.GeneratedSourcesFileList)));
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "a.dll")), "#1a");
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "b.dll")), "#1b");
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "c.dll")), "#1c");
		}

		[Test]
		public void GeneratedSourceOutputsAreRequired ()
		{
			var task = CreateTask<BGen> ();
			task.CompiledApiDefinitionAssembly = new TaskItem ("compiled-api-definitions.dll");

			Assert.That (task.Execute (), Is.False);
			Assert.That (task.Log.HasLoggedErrors, Is.True);
		}

		[Test]
		public void CompiledApiDocumentationIsCopied ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var compiledApiDefinitionAssembly = Path.Combine (directory, "compiled-api-definitions.dll");
			var documentationFile = Path.ChangeExtension (compiledApiDefinitionAssembly, ".xml");
			File.WriteAllText (documentationFile, "");

			var task = CreateTask<BGen> ();
			task.CompiledApiDefinitionAssembly = new TaskItem (compiledApiDefinitionAssembly);

			Assert.That (task.GetAdditionalItemsToBeCopied ().Select (item => item.ItemSpec), Does.Contain (documentationFile));
		}

		[Test]
		public void Bug656983 ()
		{
			var task = CreateTask<BGen> ();

			task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b.dll"), new TaskItem ("c.dll") };
			task.ProjectDir = "~/"; // not important, but required (so can't be null)
			task.ResponseFilePath = Path.Combine (Cache.CreateTemporaryDirectory (), "response-file.txt");

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			task.OutputAssembly = null; // default, but important for the bug (in case that default changes)
#pragma warning restore CS8625
			task.ExtraArgs = "-invalid";
			var args = task.GenerateCommandLineArguments ();
			args.AddRange (File.ReadAllLines (task.ResponseFilePath));
			Assert.That (args.Contains ("-invalid"), "incorrect ExtraArg not causing an exception");
		}

		[Test]
		public void ExecutesInProcess ()
		{
			var task = CreateTask<BGen> ();
			var temporaryDirectory = Cache.CreateTemporaryDirectory ();

			task.BGenToolPath = "/does/not/exist";
			task.BGenToolExe = "bgen.dll";
			task.CompiledApiDefinitionAssembly = new TaskItem ("ignored.dll");
			task.ExtraArgs = "/help";
			task.GeneratedSourcesDir = temporaryDirectory;
			task.GeneratedSourcesFileList = Path.Combine (temporaryDirectory, "generated-sources.txt");
			task.ResponseFilePath = Path.Combine (temporaryDirectory, "response-file.txt");

			ExecuteTask (task);
		}

		[TestCase (ApplePlatform.iOS, false)]
		[TestCase (ApplePlatform.iOS, true)]
		[TestCase (ApplePlatform.TVOS, false)]
		[TestCase (ApplePlatform.TVOS, true)]
		[TestCase (ApplePlatform.MacOSX, false)]
		[TestCase (ApplePlatform.MacOSX, true)]
		[TestCase (ApplePlatform.MacCatalyst, false)]
		[TestCase (ApplePlatform.MacCatalyst, true)]
		public void GeneratesBindingSourcesInProcess (ApplePlatform platform, bool includeAttributes)
		{
			Configuration.IgnoreIfIgnoredPlatform (platform);
			var temporaryDirectory = Cache.CreateTemporaryDirectory ();
			var apiDefinition = Path.Combine (temporaryDirectory, "api.cs");
			var compiledApiDefinition = Path.Combine (temporaryDirectory, "api.dll");
			File.WriteAllText (apiDefinition, $$"""
				// Copyright (c) Microsoft Corporation.
				// Licensed under the MIT License.

				using System.Diagnostics.CodeAnalysis;
				using System.Runtime.CompilerServices;
				using Foundation;

				namespace InProcessTest {
					[BaseType (typeof (NSObject))]
					{{(includeAttributes ? "[Experimental (\"BGEN0001\")]" : "")}}
					interface Binding {
						[Export ("oldMethod")]
						{{(includeAttributes ? "[OverloadResolutionPriority (-1)]" : "")}}
						void OldMethod ();
					}
				}
				""" + "\n");

			var targetFramework = TargetFramework.GetTargetFramework (platform);
			var references = Directory.GetFiles (Configuration.DotNetBclDir, "*.dll");
			var task = CreateTask<BGen> ();
			InitializeExecutionTask (task);
			task.BGenToolPath = "/does/not/exist";
			task.BGenToolExe = "bgen.dll";
			task.BaseLibDll = Configuration.GetBaseLibrary (platform);
			task.AttributeAssembly = Configuration.GetBindingAttributePath (targetFramework);
			task.CompiledApiDefinitionAssembly = new TaskItem (compiledApiDefinition);
			task.References = references.Select (v => new TaskItem (v)).ToArray ();
			task.TargetFrameworkMoniker = targetFramework.ToString ();

			var compilerCommand = StringUtils.ParseArguments (Configuration.DotNetCscCommand);
			var compilerArguments = compilerCommand.Skip (1).ToList ();
			compilerArguments.AddRange (new [] {
				"/noconfig",
				"/nostdlib",
				"/target:library",
				$"/out:{compiledApiDefinition}",
				$"/r:{task.BaseLibDll}",
				$"/r:{task.AttributeAssembly}",
				apiDefinition,
			});
			compilerArguments.AddRange (references.Select (v => $"/r:{v}"));
			var exitCode = ExecutionHelper.Execute (compilerCommand [0], compilerArguments, out var compilerOutput);
			Assert.That (exitCode, Is.EqualTo (0), $"Compile API definition: {compilerOutput}");

			ExecuteTask (task);

			Assert.That (File.Exists (task.GeneratedSourcesFileList), Is.True, "Generated source list");
			var generatedSources = File.ReadAllLines (task.GeneratedSourcesFileList);
			Assert.That (generatedSources, Is.Not.Empty, "Generated sources");
			foreach (var source in generatedSources)
				Assert.That (File.Exists (source), Is.True, $"Generated source: {source}");
			var bindingSource = generatedSources.Single (v => Path.GetFileName (v) == "Binding.g.cs");
			var binding = File.ReadAllText (bindingSource);
			Assert.That (binding, Does.Contain ("partial class Binding"), "Generated binding type");
			if (includeAttributes) {
				Assert.That (binding, Does.Contain ("[Experimental (\"BGEN0001\")]"), "Experimental attribute");
				Assert.That (binding, Does.Contain ("[OverloadResolutionPriority (-1)]"), "Overload priority");
			}
		}

		[Test]
		public void ExecutesExternalProcess ()
		{
			var task = CreateTask<BGen> ();
			var temporaryDirectory = Cache.CreateTemporaryDirectory ();

			task.BGenToolPath = "/does/not/exist";
			task.BGenToolExe = "bgen.dll";
			task.CompiledApiDefinitionAssembly = new TaskItem ("ignored.dll");
			task.ExtraArgs = "/help";
			task.GeneratedSourcesDir = temporaryDirectory;
			task.GeneratedSourcesFileList = Path.Combine (temporaryDirectory, "generated-sources.txt");
			task.ResponseFilePath = Path.Combine (temporaryDirectory, "response-file.txt");
			task.UseExternalProcess = true;

			ExecuteTask (task, expectedErrorCount: 1);
		}

		[Test]
		public void InProcessFailureWithoutDiagnosticLogsError ()
		{
			var task = CreateTask<FailingBGen> ();
			InitializeExecutionTask (task);

			ExecuteTask (task, expectedErrorCount: 1);
			Assert.That (Engine.Logger.ErrorEvents.Select (v => v.Message), Does.Contain ("The binding generator failed for unknown reasons. Please file an issue at https://github.com/dotnet/macios/issues/new/choose."));
		}

		void InitializeExecutionTask (BGen task)
		{
			var temporaryDirectory = Cache.CreateTemporaryDirectory ();
			task.CompiledApiDefinitionAssembly = new TaskItem ("ignored.dll");
			task.GeneratedSourcesDir = temporaryDirectory;
			task.GeneratedSourcesFileList = Path.Combine (temporaryDirectory, "generated-sources.txt");
			task.ResponseFilePath = Path.Combine (temporaryDirectory, "response-file.txt");
		}

		sealed class FailingBGen : BGen {
			protected override int ExecuteBGen (List<string> args)
			{
				return 1;
			}
		}
	}
}
