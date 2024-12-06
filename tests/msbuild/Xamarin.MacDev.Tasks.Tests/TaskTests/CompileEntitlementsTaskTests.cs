using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

using Xamarin.MacDev;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	class CustomCompileEntitlements : CompileEntitlements {
		protected override MobileProvision GetMobileProvision (MobileProvisionPlatform platform, string uuid)
		{
			if (File.Exists (ProvisioningProfile))
				return MobileProvision.LoadFromFile (ProvisioningProfile);

			return null!;
		}
	}

	[TestFixture]
	public class CompileEntitlementsTaskTests : TestBase {
		CustomCompileEntitlements CreateEntitlementsTask (out string compiledEntitlements)
		{
			return CreateEntitlementsTask (out compiledEntitlements, out var _);
		}

		CustomCompileEntitlements CreateEntitlementsTask (out string compiledEntitlements, out string archivedEntitlements, string mobileProvision = "profile.mobileprovision")
		{
			var task = CreateTask<CustomCompileEntitlements> ();

			task.AppBundleDir = AppBundlePath;
			task.BundleIdentifier = "com.xamarin.MySingleView";
			task.CompiledEntitlements = new TaskItem (Path.Combine (MonoTouchProjectObjPath, "Entitlements.xcent"));
			task.Entitlements = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location)!, "Resources", "Entitlements.plist");
			if (!string.IsNullOrEmpty (mobileProvision))
				task.ProvisioningProfile = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location)!, "Resources", mobileProvision);
			task.SdkPlatform = "iPhoneOS";
			task.SdkVersion = "6.1";
			task.TargetFrameworkMoniker = "Xamarin.iOS,v1.0";

			compiledEntitlements = task.CompiledEntitlements.ItemSpec;
			archivedEntitlements = Path.Combine (AppBundlePath, "archived-expanded-entitlements.xcent");

			DeleteDirectory (Path.Combine (MonoTouchProjectPath, "bin"));
			DeleteDirectory (Path.Combine (MonoTouchProjectPath, "obj"));

			return task;
		}

		void DeleteDirectory (string directory)
		{
			if (!Directory.Exists (directory))
				return;
			Directory.Delete (directory, true);
		}

		[Test (Description = "Xambug #46298")]
		public void ValidateEntitlement ()
		{
			var task = CreateEntitlementsTask (out var compiledEntitlements, out var archivedEntitlements);
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsTrue (compiled.Get<PBoolean> (EntitlementKeys.GetTaskAllow)?.Value, "#1");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.Get<PString> ("application-identifier")?.Value, "#2");
			Assert.AreEqual ("Z8CSQKJE7R", compiled.Get<PString> ("com.apple.developer.team-identifier")?.Value, "#3");
			Assert.AreEqual ("applinks:*.xamarin.com", compiled.GetAssociatedDomains ().ToStringArray ().First (), "#4");
			Assert.AreEqual ("Z8CSQKJE7R.*", compiled.GetPassBookIdentifiers ().ToStringArray ().First (), "#5");
			Assert.AreEqual ("Z8CSQKJE7R.com.xamarin.MySingleView", compiled.GetUbiquityKeyValueStore (), "#6");
			Assert.AreEqual ("32UV7A8CDE.com.xamarin.MySingleView", compiled.GetKeychainAccessGroups ().ToStringArray ().First (), "#7");

			var archived = PDictionary.FromFile (archivedEntitlements);
			Assert.IsTrue (compiled.ContainsKey ("application-identifier"), "archived");
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsTrue (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			Assert.IsTrue (compiled.Get<PBoolean> (EntitlementKeys.AllowExecutionOfJitCode)?.Value, "#2");
		}

		[Test]
		public void AllowJit_False ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("com.apple.security.cs.allow-jit", new Dictionary<string, string> { {  "Type", "Boolean" }, { "Value", "false" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements, out var archivedEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=maccatalyst";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsTrue (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			Assert.IsFalse (compiled.Get<PBoolean> (EntitlementKeys.AllowExecutionOfJitCode)?.Value, "#2");

			Assert.That (archivedEntitlements, Does.Not.Exist, "No archived entitlements");
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
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsFalse (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
		}

		[Test]
		public void AppIdentifierPrefix ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("keychain-access-group", new Dictionary<string, string> { {  "Type", "String" }, { "Value", "$(AppIdentifierPrefix)org.xamarin" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements, out var archivedEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=ios";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsFalse (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			var kag = ((PString?) compiled ["keychain-access-group"])?.Value;
			Assert.That (kag, Is.EqualTo ("32UV7A8CDE.org.xamarin"), "value 1");

			var archived = PDictionary.FromFile (archivedEntitlements)!;
			Assert.IsTrue (archived.ContainsKey ("keychain-access-group"), "archived");
			var archivedKag = ((PString?) archived ["keychain-access-group"])?.Value;
			Assert.That (archivedKag, Is.EqualTo ("32UV7A8CDE.org.xamarin"), "archived value 1");
		}

		[Test]
		public void TeamIdentifierPrefix ()
		{
			var customEntitlements = new TaskItem [] {
				new TaskItem ("keychain-access-group", new Dictionary<string, string> { {  "Type", "String" }, { "Value", "$(TeamIdentifierPrefix)org.xamarin" } }),
			};
			var task = CreateEntitlementsTask (out var compiledEntitlements, out var archivedEntitlements);
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=ios";
			task.CustomEntitlements = customEntitlements;
			ExecuteTask (task);
			var compiled = PDictionary.FromFile (compiledEntitlements)!;
			Assert.IsFalse (compiled.ContainsKey (EntitlementKeys.AllowExecutionOfJitCode), "#1");
			var kag = ((PString?) compiled ["keychain-access-group"])?.Value;
			Assert.That (kag, Is.EqualTo ("Z8CSQKJE7R.org.xamarin"), "value 1");

			var archived = PDictionary.FromFile (archivedEntitlements)!;
			Assert.IsTrue (archived.ContainsKey ("keychain-access-group"), "archived");
			var archivedKag = ((PString?) archived ["keychain-access-group"])?.Value;
			Assert.That (archivedKag, Is.EqualTo ("Z8CSQKJE7R.org.xamarin"), "archived value 1");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_Invalid (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("invalid", 1, mode);
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Message, "Invalid value 'invalid' for the 'ValidateEntitlements' property. Valid values are: 'disable', 'warn' or 'error'.", "Error message");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Code, "MT7138", "Error code");
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_NotInProfile_Default (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("", 1, mode);
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Message, "The app requests the entitlement 'aps-environment', but the provisioning profile 'iOS Team Provisioning Profile: *' does not contain this entitlement.", "Error message");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Code, "MT7140", "Error code");
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_NotInProfile_Error (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("error", 1, mode);
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Message, "The app requests the entitlement 'aps-environment', but the provisioning profile 'iOS Team Provisioning Profile: *' does not contain this entitlement.", "Error message");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Code, "MT7140", "Error code");
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_NotInProfile_Warning (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("warn", 0, mode);
			Assert.AreEqual (1, Engine.Logger.WarningsEvents.Count, "WarningCount");
			Assert.AreEqual (Engine.Logger.WarningsEvents [0].Message, "The app requests the entitlement 'aps-environment', but the provisioning profile 'iOS Team Provisioning Profile: *' does not contain this entitlement.", "Warning message");
			Assert.AreEqual (Engine.Logger.WarningsEvents [0].Code, "MT7140", "Warning code");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_NoProfile_Error (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("error", 1, mode, mobileProvision: string.Empty);
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Message, "The app requests the entitlement 'aps-environment', but no provisioning profile has been specified. Please specify the name of the provisioning profile to use with the 'CodesignProvision' property in the project file.", "Error message");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Code, "MT7139", "Error code");
			Assert.AreEqual (0, GetWarningsSkippingIrrelevantWarnings ().Count, "WarningCount");
		}

		List<BuildWarningEventArgs> GetWarningsSkippingIrrelevantWarnings ()
		{
			return Engine.Logger.WarningsEvents.Where (v => {
				Console.WriteLine ($"Message: {v.Message}");
				return v.Message != ("Cannot expand $(AppIdentifierPrefix) in Entitlements.plist without a provisioning profile for key 'application-identifier' with value '$(AppIdentifierPrefix)$(CFBundleIdentifier)'.");
			}).ToList ();
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_NoProfile_Warning (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("warn", 0, mode, mobileProvision: string.Empty);
			var warnings = GetWarningsSkippingIrrelevantWarnings ();
			Assert.AreEqual (1, warnings.Count, "WarningCount");
			Assert.AreEqual (warnings [0].Message, "The app requests the entitlement 'aps-environment', but no provisioning profile has been specified. Please specify the name of the provisioning profile to use with the 'CodesignProvision' property in the project file.", "Error message");
			Assert.AreEqual (warnings [0].Code, "MT7139", "Warning code");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_MismatchedValue_Error (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("error", 1, mode, mobileProvision: "APS_Development_Environment_Profile.mobileprovision");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Message, "The app requests the entitlement 'aps-environment' with the value 'production', but the provisioning profile 'APS Development Environment Profile' grants it for the value 'development'.", "Error message");
			Assert.AreEqual (Engine.Logger.ErrorEvents [0].Code, "MT7137", "Error code");
			var warnings = GetWarningsSkippingIrrelevantWarnings ();
			Assert.AreEqual (0, warnings.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_MismatchedValue_Warning (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("warn", 0, mode, mobileProvision: "APS_Development_Environment_Profile.mobileprovision");
			Assert.AreEqual (1, Engine.Logger.WarningsEvents.Count, "WarningCount");
			Assert.AreEqual (Engine.Logger.WarningsEvents [0].Message, "The app requests the entitlement 'aps-environment' with the value 'production', but the provisioning profile 'APS Development Environment Profile' grants it for the value 'development'.", "Warning message");
			Assert.AreEqual (Engine.Logger.WarningsEvents [0].Code, "MT7137", "Warning code");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_MatchingValue_NoError (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("error", 0, mode, mobileProvision: "APS_Development_Environment_Profile.mobileprovision", apsEnvironmentValue: "development");
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_MatchingValue_NoWarning (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("warn", 0, mode, mobileProvision: "APS_Development_Environment_Profile.mobileprovision", apsEnvironmentValue: "development");
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		[Test]
		[TestCase (EntitlementsMode.InCustomEntitlements)]
		[TestCase (EntitlementsMode.InFile)]
		public void ValidateEntitlements_Disabled (EntitlementsMode mode)
		{
			ValidateEntitlementsImpl ("disable", 0, mode);
			Assert.AreEqual (0, Engine.Logger.WarningsEvents.Count, "WarningCount");
		}

		public enum EntitlementsMode {
			None,
			InFile,
			InCustomEntitlements,
		}
		
		CompileEntitlements ValidateEntitlementsImpl (string validateEntitlements, int expectedErrorCount, EntitlementsMode entitlementsMode, string mobileProvision = "profile.mobileprovision", string apsEnvironmentValue = "production")
		{
			var task = CreateTask<CustomCompileEntitlements> ();

			var dir = Cache.CreateTemporaryDirectory ();
			task.AppBundleDir = Path.Combine (dir, "TestApp.app");
			task.BundleIdentifier = "com.xamarin.compileentitlementstasktest";
			task.CompiledEntitlements = new TaskItem (Path.Combine (dir, "Entitlements.xcent"));
			switch (entitlementsMode) {
				case EntitlementsMode.InFile:
				var path = Path.Combine (dir, "Entitlements.plist");
				File.WriteAllText (path, $"<plist version=\"1.0\"><dict><key>aps-environment</key><string>{apsEnvironmentValue}</string></dict></plist>");
				task.Entitlements = path;
				break;
			case EntitlementsMode.InCustomEntitlements:
				var apsEnvironment = new TaskItem ("aps-environment");
				apsEnvironment.SetMetadata ("Type", "String");
				apsEnvironment.SetMetadata ("Value", apsEnvironmentValue);
				task.CustomEntitlements = new [] { apsEnvironment };
				break;
			}
			if (!string.IsNullOrEmpty (mobileProvision))
				task.ProvisioningProfile = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location)!, "Resources", mobileProvision);
			task.SdkPlatform = "iPhoneOS";
			// task.SdkVersion = "6.1";
			task.TargetFrameworkMoniker = ".NETCoreApp,Version=v6.0,Profile=ios";
			task.ValidateEntitlements = validateEntitlements;
			ExecuteTask (task, expectedErrorCount: expectedErrorCount);
			return task;
		}
	}
}
