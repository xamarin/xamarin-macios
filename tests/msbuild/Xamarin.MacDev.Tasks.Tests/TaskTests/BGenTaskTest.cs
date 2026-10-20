using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Utilities;

using NUnit.Framework;

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
	}
}
