#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NUnit.Framework;

using Xamarin.Tests;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class ValidateNoStaticLibrariesTests : TestBase {
		ValidateNoStaticLibraries CreateTask (string skipStaticLibraryValidation, params string[] paths)
		{
			var task = CreateTask<ValidateNoStaticLibraries> ();
			task.SkipStaticLibraryValidation = skipStaticLibraryValidation;
			task.ValidateItems = paths.Select (v => new TaskItem (v)).ToArray ();
			return task;
		}

		[Test]
		[TestCase ("error")]
		[TestCase ("false")]
		[TestCase ("")]
		public void StaticLibraries_Error (string skipStaticLibraryValidation)
		{
			var paths = new [] {
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libtest.a"),
				Path.Combine (Configuration.RootPath, "README.md"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libframework.dylib"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework", "SwiftTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework", "Info.plist"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "XStaticArTest.framework", "XStaticArTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "XStaticObjectTest.framework", "XStaticObjectTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "XTest.framework", "XTest"),
			};
			var task = CreateTask (skipStaticLibraryValidation, paths);
			ExecuteTask (task, expectedErrorCount: 3);
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");
			Assert.AreEqual ($"The library {paths [0]} is a static library, and static libraries are not supported with Hot Restart. Set 'SkipStaticLibraryValidation=true' in the project file to ignore this error.", Engine.Logger.ErrorEvents [0].Message, "Error message 1");
			Assert.AreEqual ($"The library {paths [5]} is a static library, and static libraries are not supported with Hot Restart. Set 'SkipStaticLibraryValidation=true' in the project file to ignore this error.", Engine.Logger.ErrorEvents [1].Message, "Error message 2");
			Assert.AreEqual ($"The file {paths [6]} is an object file, and object files are not supported with Hot Restart. Set 'SkipStaticLibraryValidation=true' in the project file to ignore this error.", Engine.Logger.ErrorEvents [2].Message, "Error message 3");
		}

		[Test]
		[TestCase ("warn")]
		public void StaticLibraries_Warn (string skipStaticLibraryValidation)
		{
			var paths = new [] {
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libtest.a"),
				Path.Combine (Configuration.RootPath, "README.md"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libframework.dylib"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/SwiftTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/Info.plist"),
			};
			var task = CreateTask (skipStaticLibraryValidation, paths);
			ExecuteTask (task, expectedErrorCount: 0);
			Assert.AreEqual (1, Engine.Logger.WarningsEvents.Count, "Warning Count");
			Assert.AreEqual ($"The library {paths [0]} is a static library, and static libraries are not supported with Hot Restart. Set 'SkipStaticLibraryValidation=true' in the project file to ignore this error.", Engine.Logger.WarningsEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase ("disable")]
		public void StaticLibraries_Disabled (string skipStaticLibraryValidation)
		{
			var paths = new [] {
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libtest.a"),
				Path.Combine (Configuration.RootPath, "README.md"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libframework.dylib"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/SwiftTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/Info.plist"),
				"/does/not/exist",
			};
			var task = CreateTask (skipStaticLibraryValidation, paths);
			ExecuteTask (task, expectedErrorCount: 0);
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");
		}

		[Test]
		[TestCase ("invalid")]
		public void StaticLibraries_Invalid (string skipStaticLibraryValidation)
		{
			var paths = new [] {
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libtest.a"),
				Path.Combine (Configuration.RootPath, "README.md"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "libframework.dylib"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/SwiftTest"),
				Path.Combine (Configuration.RootPath, "tests", "test-libraries", ".libs", "iossimulator", "SwiftTest.framework/Info.plist"),
				"/does/not/exist",
			};
			var task = CreateTask (skipStaticLibraryValidation, paths);
			ExecuteTask (task, expectedErrorCount: 1);
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");
			Assert.AreEqual ($"Unknown value for 'SkipStaticLibraryValidation': {skipStaticLibraryValidation}. Valid values are: 'true', 'false', 'warn'.", Engine.Logger.ErrorEvents [0].Message, "Error Message");
		}

		[Test]
		[TestCase ("")]
		[TestCase ("error")]
		public void StaticLibraries_Error_Inexistent (string skipStaticLibraryValidation)
		{
			var task = CreateTask (skipStaticLibraryValidation, "/does/not/exist");
			ExecuteTask (task, expectedErrorCount: 1);
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "Warning Count");
			Assert.AreEqual ($"The file '/does/not/exist' does not exist.", Engine.Logger.ErrorEvents [0].Message.TrimEnd (), "Error Message");
		}

		[Test]
		[TestCase ("warn")]
		public void StaticLibraries_Warn_Inexistent (string skipStaticLibraryValidation)
		{
			var task = CreateTask (skipStaticLibraryValidation, "/does/not/exist");
			ExecuteTask (task, expectedErrorCount: 0);
			Assert.AreEqual (1, Engine.Logger.WarningsEvents.Count, "Warning Count");
			Assert.AreEqual ($"The file '/does/not/exist' does not exist.", Engine.Logger.WarningsEvents [0].Message.TrimEnd (), "Error Message");
		}
	}
}
