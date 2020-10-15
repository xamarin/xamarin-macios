using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.iOS.Tasks
{
	[TestFixture (Description = @"Ensures that all non-abstract task classes have empty bodies, 
since all code must exist in the base classes instead, so it can be reused from Windows tasks.")]
	public class EnsureEmptyTasks
	{
		static readonly Regex EmptyClassExpr = new Regex (@"public class (?<name>[^\s]+)\s?:\s?[^\{]*?\{\s*\}", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
		static readonly Regex ClassNameExpr = new Regex (@"public class (?<name>[^\s]+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
		static readonly Regex AbstractClassExpr = new Regex (@"abstract class ", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

		static readonly string TasksPath = Path.Combine (Configuration.SourceRoot, "msbuild", "Xamarin.iOS.Tasks", "Tasks");

		[TestCaseSource ("TaskFiles")]
		[Test]
		public void VerifyEmpty (string taskFile)
		{
			var contents = File.ReadAllText (Path.Combine (TasksPath, taskFile));
			var emptyClass = EmptyClassExpr.Match (contents);
			if (!emptyClass.Success) {
				var className = ClassNameExpr.Match (contents);
				if (className.Success)
					Assert.Fail ("{0} must not contain any implementation code.", className.Groups["name"].Value);
				else
					Assert.Fail ("{0} must contain a single class without any implementation code.", taskFile);
			}
		}

		static object[] TaskFiles
		{
			get
			{
				return Directory.EnumerateFiles (TasksPath)
					.Where (file => !AbstractClassExpr.IsMatch(File.ReadAllText(file)))
					.Select (file => Path.GetFileName (file))
					.ToArray ();
			}
		}
	}
}
