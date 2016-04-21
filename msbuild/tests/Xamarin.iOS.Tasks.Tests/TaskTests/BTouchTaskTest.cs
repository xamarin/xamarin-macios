using System;
using System.IO;

using Microsoft.Build.Utilities;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	class CustomBTouchTask : BTouch
	{
		public CustomBTouchTask ()
		{
		}

		public new string GenerateCommandLineCommands ()
		{
			return base.GenerateCommandLineCommands ();
		}
	}

	[TestFixture]
	public class BTouchTaskTests : TestBase
	{
		CustomBTouchTask Task {
			get; set;
		}

		public override void Setup ()
		{
			base.Setup ();

			Task = CreateTask<CustomBTouchTask> ();

			Task.ApiDefinitions = new [] { new TaskItem ("apidefinition.cs") };
			Task.References = new [] { new TaskItem ("a.dll"), new TaskItem ("b.dll"), new TaskItem ("c.dll") };
		}

		[Test]
		public void StandardCommandline ()
		{
			var args = Task.GenerateCommandLineCommands ();
			Assert.IsTrue (args.Contains ("-r " + Path.Combine (Environment.CurrentDirectory, "a.dll")), "#1a");
			Assert.IsTrue (args.Contains ("-r " + Path.Combine (Environment.CurrentDirectory, "b.dll")), "#1b");
			Assert.IsTrue (args.Contains ("-r " + Path.Combine (Environment.CurrentDirectory, "c.dll")), "#1c");
		}

	}
}

