using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

using Xamarin.iOS.Tasks;
using Xamarin.MacDev;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	class CustomCompileEntitlements : CompileEntitlements {
		protected override MobileProvision? GetMobileProvision (MobileProvisionPlatform platform, string uuid)
		{
			if (File.Exists (ProvisioningProfile))
				return MobileProvision.LoadFromFile (ProvisioningProfile);

			return null;
		}
	}

	[TestFixture]
	public class CompileEntitlementsTaskTests : TestBase {
		CustomCompileEntitlements CreateEntitlementsTask (out string compiledEntitlements)
		{
			var task = CreateTask<CustomCompileEntitlements> ();

			task.AppBundleDir = AppBundlePath;
			task.BundleIdentifier = "com.xamarin.MySingleView";
			task.CompiledEntitlements = new TaskItem (Path.Combine (MonoTouchProjectObjPath, "Entitlements.xcent"));
			task.Entitlements = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "Entitlements.plist");
			task.ProvisioningProfile = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "Resources", "profile.mobileprovision");
			task.SdkPlatform = "iPhoneOS";
			task.SdkVersion = "6.1";
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";

			compiledEntitlements = task.CompiledEntitlements.ItemSpec;

			return task;
		}

		[Test (Description = "Xambug #46298")]
		public void ValidateEntitlement ()
		{
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsTrue (compiled.Get<PBoolean> (EntitlementKeys.GetTaskAllow).Value, "#1");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.Get<PString> ("application-identifier").Value, "#2");
			Assert.AreEqual ("Z8CSQKJE7R", compiled.Get<PString> ("com.apple.developer.team-identifier").Value, "#3");
			Assert.AreEqual ("applinks:*.xamarin.com", compiled.GetAssociatedDomains ().ToStringArray ().First (), "#4");
			Assert.AreEqual ("Z8CSQKJE7R.*", compiled.GetPassBookIdentifiers ().ToStringArray ().First (), "#5");
			Assert.AreEqual ("Z8CSQKJE7R.com.xamarin.MySingleView", compiled.GetUbiquityKeyValueStore (), "#6");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.GetKeychainAccessGroups ().ToStringArray ().First (), "#7");
		}

		[TestCase ("Invalid", null, "Unknown type 'Invalid' for the entitlement 'com.xamarin.custom.entitlement' specified in the CustomEntitlements item group. Expected 'Remove', 'Boolean', 'String', or 'StringArray'.")]
		[TestCase ("Boolean", null, "Invalid value '' for the entitlement 'com.xamarin.custom.entitlement' of type 'Boolean' specified in the CustomEntitlements item group. Expected 'true' or 'false'.")]
		[TestCase ("Boolean", "invalid", "Invalid value 'invalid' for the entitlement 'com.xamarin.custom.entitlement' of type 'Boolean' specified in the CustomEntitlements item group. Expected 'true' or 'false'.")]
		[TestCase ("Remove", "invalid", "Invalid value 'invalid' for the entitlement 'com.xamarin.custom.entitlement' of type 'Remove' specified in the CustomEntitlements item group. Expected no value at all.")]
		public void InvalidCustomEntitlements (string type, string? value, string errorMessage)
		{
			var dict = new Dictionary<string, string> {
				{ "Type", type }
			};
			if (value is not null)
				dict ["Value"] = value;

			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.xamarin.custom.entitlement", dict)
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task, expectedErrorCount: 1);
			Assert.AreEqual (errorMessage, Engine.Logger.ErrorEvents [0].Message, "Error message");
		}

		[Test]
		[TestCase ("a-string-value")]
		[TestCase ("")]
		[TestCase (null)]
		public void CustomEntitlemements_String (string value)
		{
			var dict = new Dictionary<string, string> {
				{ "Type", "String" },
				{ "Value", value },
			};
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.xamarin.custom.entitlement", dict)
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.AreEqual (value ?? string.Empty, compiled.GetString ("com.xamarin.custom.entitlement")?.Value, "#1");
		}

		[Test]
		public void CustomEntitlemements_StringArray ()
		{
			var dict = new Dictionary<string, string> {
				{ "Type", "StringArray" },
				{ "Value", "A;B;C" },
			};
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.xamarin.custom.entitlement", dict)
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			var array = compiled.GetArray ("com.xamarin.custom.entitlement");
			Assert.NotNull (array, "array");
			Assert.AreEqual (new string [] { "A", "B", "C" }, array.ToStringArray (), "array contents");
		}

		[Test]
		[TestCase (",")]
		[TestCase ("😁")]
		public void CustomEntitlemements_StringArray_CustomSeparator (string separator)
		{
			var dict = new Dictionary<string, string> {
				{ "Type", "StringArray" },
				{ "Value", $"A;B;C{separator}D{separator}E" },
				{ "ArraySeparator", separator },
			};
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.xamarin.custom.entitlement", dict)
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			var array = compiled.GetArray ("com.xamarin.custom.entitlement");
			Assert.NotNull (array, "array");
			Assert.AreEqual (new string [] { "A;B;C", "D", "E" }, array.ToStringArray (), "array contents");
		}

		[Test]
		public void AllowJit_Default ()
		{
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsFalse (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
		}

		[Test]
		public void AllowJit_True ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.apple.security.cs.allow-jit", new Dictionary<string, string> { {  "Type", "Boolean" }, { "Value", "true" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsTrue (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			Assert.IsTrue (compiled.Get<PBoolean> (EntitlementKeys.AllowExecutionOfJitCode).Value, "#2");
		}

		[Test]
		public void AllowJit_False ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.apple.security.cs.allow-jit", new Dictionary<string, string> { {  "Type", "Boolean" }, { "Value", "false" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsTrue (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			Assert.IsFalse (compiled.Get<PBoolean> (EntitlementKeys.AllowExecutionOfJitCode).Value, "#2");
		}

		[Test]
		public void AllowJit_None ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.apple.security.cs.allow-jit", new Dictionary<string, string> { {  "Type", "Remove" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements);
			Assert.IsFalse (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
		}

	}
}
