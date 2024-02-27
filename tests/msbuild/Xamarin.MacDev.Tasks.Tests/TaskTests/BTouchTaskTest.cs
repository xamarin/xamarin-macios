using System;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {
	class CustomBTouchTask : BTouch {
		public string GetCommandLineCommands ()
		{
			return base.GenerateCommandLineCommands ();
		}
	}

	[TestFixture]
	public class BTouchTaskTests : TestBase {
		[Test]
		public void StandardCommandline ()
		{
			var task = CreateTask<CustomBTouchTask> ();

			task.ApiDefinitions = new [] { new TaskItem ("apidefinition.cs") };
			task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b.dll"), new TaskItem ("c.dll") };
			task.ResponseFilePath = Path.Combine (Cache.CreateTemporaryDirectory (), "response-file.txt");

			var args = task.GetCommandLineCommands () + " " + File.ReadAllText (task.ResponseFilePath);
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "a.dll")), "#1a");
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "b.dll")), "#1b");
			Assert.That (args, Does.Contain ("-r:" + Path.Combine (Environment.CurrentDirectory, "c.dll")), "#1c");
		}

		[Test]
		public void Bug656983 ()
		{
			var task = CreateTask<CustomBTouchTask> ();

			task.ApiDefinitions = new [] { new TaskItem ("apidefinition.cs") };
			task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b.dll"), new TaskItem ("c.dll") };
			task.ProjectDir = "~/"; // not important, but required (so can't be null)
			task.ResponseFilePath = Path.Combine (Cache.CreateTemporaryDirectory (), "response-file.txt");

			task.OutputAssembly = null; // default, but important for the bug (in case that default changes)
			task.ExtraArgs = "-invalid";
			var args = task.GetCommandLineCommands () + " " + File.ReadAllText (task.ResponseFilePath);
			Assert.That (args.Contains (" -invalid"), "incorrect ExtraArg not causing an exception");
		}
	}
}
