// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class PrepareNativeReferencesTaskTests : TestBase {
		PrepareNativeReferences CreatePrepareTask (string kind, string? dlsym)
		{
			var reference = new TaskItem ("NativeLibrary");
			reference.SetMetadata ("Kind", kind);
			if (dlsym is not null)
				reference.SetMetadata ("Dlsym", dlsym);

			var task = CreateTask<PrepareNativeReferences> ();
			task.IntermediateOutputPath = Cache.CreateTemporaryDirectory ();
			task.NativeReferences = new ITaskItem [] { reference };
			return task;
		}

		[Test]
		public void Dlsym (
			[Values (null, "", "Default", "Required", "Disabled", "required", "DISABLED", "0", "1", "2")] string? value,
			[Values ("Static", "Dynamic", "Framework")] string kind)
		{
			var task = CreatePrepareTask (kind, value);

			ExecuteTask (task);

			var expected = value switch {
				null or "" => null,
				"0" => "Default",
				"1" or "required" => "Required",
				"2" or "DISABLED" => "Disabled",
				_ => value,
			};
			var property = expected is null ? "" : $", Dlsym = ObjCRuntime.DlsymOption.{expected}";
			var path = Path.Combine (task.IntermediateOutputPath, "LinkWithAttributes.cs");
			Assert.That (task.LinkWithAttributes?.ItemSpec, Is.EqualTo (path), "Generated file");
			var attributes = File.ReadAllText (path);
			Assert.That (attributes, Is.EqualTo ($"[assembly: ObjCRuntime.LinkWith (\"NativeLibrary\"{property}, SmartLink = true)]{System.Environment.NewLine}"), "LinkWith attributes");
		}

		[TestCase ("Invalid")]
		[TestCase ("true")]
		[TestCase ("3")]
		[TestCase ("-1")]
		[TestCase ("Required, Disabled")]
		public void InvalidDlsym (string value)
		{
			var task = CreatePrepareTask ("Static", value);

			ExecuteTask (task, 1);

			var error = Engine.Logger.ErrorEvents.Single ();
			Assert.That (error.Message, Does.Contain (value), "Invalid value");
			Assert.That (error.Message, Does.Contain ("NativeLibrary"), "Native reference");
			Assert.That (File.ReadAllText (Path.Combine (task.IntermediateOutputPath, "LinkWithAttributes.cs")), Does.Not.Contain ("Dlsym ="), "No invalid generated property");
		}

		[Test]
		public void DlsymMetadataChanges ()
		{
			var task = CreatePrepareTask ("Static", "Required");
			ExecuteTask (task);
			var path = Path.Combine (task.IntermediateOutputPath, "LinkWithAttributes.cs");
			Assert.That (File.ReadAllText (path), Does.Contain ("Dlsym = ObjCRuntime.DlsymOption.Required"), "Initial value");

			task.NativeReferences [0].SetMetadata ("Dlsym", "Disabled");
			ExecuteTask (task);
			Assert.That (File.ReadAllText (path), Does.Contain ("Dlsym = ObjCRuntime.DlsymOption.Disabled"), "Changed value");

			task.NativeReferences [0].RemoveMetadata ("Dlsym");
			ExecuteTask (task);
			Assert.That (File.ReadAllText (path), Does.Not.Contain ("Dlsym ="), "Removed value");
		}
	}
}
