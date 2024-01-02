using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CompileEntitlements : XamarinTask, ITaskCallback, ICancelableTask {
		bool warnedTeamIdentifierPrefix;
		bool warnedAppIdentifierPrefix;

		static readonly HashSet<string> macAllowedProvisioningKeys = new HashSet<string> {
			"com.apple.application-identifier",
			"com.apple.developer.aps-environment",
			"com.apple.developer.default-data-protection",
			//"com.apple.developer.icloud-container-development-container-identifiers",
			//"com.apple.developer.icloud-container-identifiers",
			//"com.apple.developer.icloud-container-environment",
			//"com.apple.developer.icloud-services",
			"com.apple.developer.pass-type-identifiers",
			"com.apple.developer.team-identifier",
			//"com.apple.developer.ubiquity-container-identifiers",
			"get-task-allow",
		};

		static readonly HashSet<string> iOSAllowedProvisioningKeys = new HashSet<string> {
			"application-identifier",
			"aps-environment",
			"beta-reports-active",
			"com.apple.developer.default-data-protection",

			"com.apple.developer.icloud-container-environment",
			"com.apple.developer.icloud-container-identifiers",
			"com.apple.developer.pass-type-identifiers",
			"com.apple.developer.team-identifier",
			"com.apple.developer.ubiquity-container-identifiers",
			"get-task-allow"
		};


		#region Inputs

		[Required]
		public string AppBundleDir { get; set; } = string.Empty;

		[Required]
		public string BundleIdentifier { get; set; } = string.Empty;

		[Required]
		public ITaskItem? CompiledEntitlements { get; set; }

		public ITaskItem [] CustomEntitlements { get; set; } = Array.Empty<ITaskItem> ();

		public bool Debug { get; set; }

		public string Entitlements { get; set; } = string.Empty;

		public string ProvisioningProfile { get; set; } = string.Empty;

		public bool SdkIsSimulator { get; set; }

		[Required]
		public string SdkPlatform { get; set; } = string.Empty;

		[Required]
		public string SdkVersion { get; set; } = string.Empty;

		[Output]
		public ITaskItem? EntitlementsInExecutable { get; set; }

		[Output]
		public ITaskItem? EntitlementsInSignature { get; set; }

		#endregion

		protected string ApplicationIdentifierKey {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return "application-identifier";
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return "com.apple.application-identifier";
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		string DefaultEntitlementsPath {
			get {
				if (ShouldExecuteRemotely ()) {
					return "Entitlements.plist";
				}

				return Path.Combine (Sdks.GetAppleSdk (TargetFrameworkMoniker).GetSdkPath (SdkVersion, false), "Entitlements.plist");
			}
		}

		protected HashSet<string> AllowedProvisioningKeys {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return iOSAllowedProvisioningKeys;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return macAllowedProvisioningKeys;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected string EntitlementBundlePath {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return AppBundleDir;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine (AppBundleDir, "Contents", "Resources");
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		PString MergeEntitlementString (PString pstr, MobileProvision? profile, bool expandWildcards)
		{
			string TeamIdentifierPrefix;
			string AppIdentifierPrefix;

			if (string.IsNullOrEmpty (pstr.Value))
				return (PString) pstr.Clone ();

			if (profile is null) {
				if (!warnedTeamIdentifierPrefix && pstr.Value.Contains ("$(TeamIdentifierPrefix)")) {
					Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0108);
					warnedTeamIdentifierPrefix = true;
				}

				if (!warnedAppIdentifierPrefix && pstr.Value.Contains ("$(AppIdentifierPrefix)")) {
					Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0109);
					warnedAppIdentifierPrefix = true;
				}
			}

			if (profile is not null && profile.ApplicationIdentifierPrefix.Count > 0)
				AppIdentifierPrefix = profile.ApplicationIdentifierPrefix [0] + ".";
			else
				AppIdentifierPrefix = string.Empty;

			if (profile is not null && profile.TeamIdentifierPrefix.Count > 0)
				TeamIdentifierPrefix = profile.TeamIdentifierPrefix [0] + ".";
			else
				TeamIdentifierPrefix = AppIdentifierPrefix;

			var customTags = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase) {
				{ "TeamIdentifierPrefix", TeamIdentifierPrefix },
				{ "AppIdentifierPrefix",  AppIdentifierPrefix },
				{ "CFBundleIdentifier",   BundleIdentifier },
			};

			var expanded = StringParserService.Parse (pstr.Value, customTags);

			if (expandWildcards && expanded.IndexOf ('*') != -1) {
				int asterisk = expanded.IndexOf ('*');
				string prefix;

				if (expanded.StartsWith (TeamIdentifierPrefix, StringComparison.Ordinal))
					prefix = TeamIdentifierPrefix;
				else if (expanded.StartsWith (AppIdentifierPrefix, StringComparison.Ordinal))
					prefix = AppIdentifierPrefix;
				else
					prefix = string.Empty;

				var baseBundleIdentifier = expanded.Substring (prefix.Length, asterisk - prefix.Length);

				if (!BundleIdentifier.StartsWith (baseBundleIdentifier, StringComparison.Ordinal))
					expanded = expanded.Replace ("*", BundleIdentifier);
				else
					expanded = prefix + BundleIdentifier;
			}

			return new PString (expanded);
		}

		PArray? MergeEntitlementArray (PArray array, MobileProvision? profile)
		{
			var result = new PArray ();

			foreach (var item in array) {
				PObject? value;

				if (item is PDictionary)
					value = MergeEntitlementDictionary ((PDictionary) item, profile);
				else if (item is PString)
					value = MergeEntitlementString ((PString) item, profile, false);
				else if (item is PArray)
					value = MergeEntitlementArray ((PArray) item, profile);
				else
					value = item.Clone ();

				if (value is not null)
					result.Add (value);
			}

			if (result.Count > 0)
				return result;

			return null;
		}

		PDictionary MergeEntitlementDictionary (PDictionary dict, MobileProvision? profile)
		{
			var result = new PDictionary ();

			foreach (var item in dict) {
				PObject? value = item.Value;

				if (value is PDictionary)
					value = MergeEntitlementDictionary ((PDictionary) value, profile);
				else if (value is PString)
					value = MergeEntitlementString ((PString) value, profile, false);
				else if (value is PArray)
					value = MergeEntitlementArray ((PArray) value, profile);
				else
					value = value.Clone ();

				if (value is not null)
					result.Add (item.Key!, value);
			}

			return result;
		}

		void AddCustomEntitlements (PDictionary dict)
		{
			if (CustomEntitlements is null)
				return;

			// Process any custom entitlements from the 'CustomEntitlements' item group. These are applied last, and will override anything else.
			// Possible values:
			//     <ItemGroup>
			//         <CustomEntitlements Include="name.of.entitlement" Type="Boolean" Value="true" /> <!-- value can be 'false' too (case doesn't matter) -->
			//         <CustomEntitlements Include="name.of.entitlement" Type="String" Value="stringvalue" />
			//         <CustomEntitlements Include="name.of.entitlement" Type="StringArray" Value="a;b" /> <!-- array of strings, separated by semicolon -->
			//         <CustomEntitlements Include="name.of.entitlement" Type="StringArray" Value="a😁b" ArraySeparator="😁" /> <!-- array of strings, separated by 😁 -->
			//         <CustomEntitlements Include="name.of.entitlement" Type="Remove" /> <!-- This will remove the corresponding entitlement  -->
			//     </ItemGroup>

			foreach (var item in CustomEntitlements) {
				var entitlement = item.ItemSpec;
				var type = item.GetMetadata ("Type");
				var value = item.GetMetadata ("Value");
				switch (type.ToLowerInvariant ()) {
				case "remove":
					if (!string.IsNullOrEmpty (value))
						Log.LogError (MSBStrings.E7102, /* Invalid value '{0}' for the entitlement '{1}' of type '{2}' specified in the CustomEntitlements item group. Expected no value at all. */ value, entitlement, type);
					dict.Remove (entitlement);
					break;
				case "boolean":
					bool booleanValue;
					if (string.Equals (value, "true", StringComparison.OrdinalIgnoreCase)) {
						booleanValue = true;
					} else if (string.Equals (value, "false", StringComparison.OrdinalIgnoreCase)) {
						booleanValue = false;
					} else {
						Log.LogError (MSBStrings.E7103, /* "Invalid value '{0}' for the entitlement '{1}' of type '{2}' specified in the CustomEntitlements item group. Expected 'true' or 'false'." */ value, entitlement, type);
						continue;
					}

					dict [entitlement] = new PBoolean (booleanValue);
					break;
				case "string":
					dict [entitlement] = new PString (value ?? string.Empty);
					break;
				case "stringarray":
					var arraySeparator = item.GetMetadata ("ArraySeparator");
					if (string.IsNullOrEmpty (arraySeparator))
						arraySeparator = ";";
					var arrayContent = value.Split (new string [] { arraySeparator }, StringSplitOptions.None);
					var parray = new PArray ();
					foreach (var element in arrayContent)
						parray.Add (new PString (element));
					dict [entitlement] = parray;
					break;
				default:
					Log.LogError (MSBStrings.E7104, /* "Unknown type '{0}' for the entitlement '{1}' specified in the CustomEntitlements item group. Expected 'Remove', 'Boolean', 'String', or 'StringArray'." */ type, entitlement);
					break;
				}
			}
		}

		static bool AreEqual (byte [] x, byte [] y)
		{
			if (x.Length != y.Length)
				return false;

			for (int i = 0; i < x.Length; i++) {
				if (x [i] != y [i])
					return false;
			}

			return true;
		}

		static void WriteXcent (PObject doc, string path)
		{
			var buf = doc.ToByteArray (false);

			using (var stream = new MemoryStream ()) {
				stream.Write (buf, 0, buf.Length);

				var src = stream.ToArray ();
				bool save;

				// Note: if the destination file already exists, only re-write it if the content will change
				if (File.Exists (path)) {
					var dest = File.ReadAllBytes (path);

					save = !AreEqual (src, dest);
				} else {
					save = true;
				}

				if (save)
					File.WriteAllBytes (path, src);
			}
		}

		protected virtual PDictionary GetCompiledEntitlements (MobileProvision? profile, PDictionary template)
		{
			var entitlements = new PDictionary ();

			if (profile is not null) {
				// start off with the settings from the provisioning profile
				foreach (var item in profile.Entitlements) {
					var key = item.Key!;
					if (!AllowedProvisioningKeys.Contains (key))
						continue;

					var value = item.Value;

					if (key == "com.apple.developer.icloud-container-environment")
						value = new PString ("Development");
					else if (value is PDictionary)
						value = MergeEntitlementDictionary ((PDictionary) value, profile);
					else if (value is PString)
						value = MergeEntitlementString ((PString) value, profile, item.Key == ApplicationIdentifierKey);
					else if (value is PArray)
						value = MergeEntitlementArray ((PArray) value, profile);
					else
						value = value.Clone ();

					if (value is not null)
						entitlements.Add (key, value);
				}
			}

			// merge in the user's values
			foreach (var item in template) {
				var value = item.Value;
				var key = item.Key!;

				if (key == "com.apple.developer.ubiquity-container-identifiers" ||
					key == "com.apple.developer.icloud-container-identifiers" ||
					key == "com.apple.developer.icloud-container-environment" ||
					key == "com.apple.developer.icloud-services") {
					if (profile is null)
						Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0110, key);
					else if (!profile.Entitlements.ContainsKey (key))
						Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0111, key);
				} else if (key == ApplicationIdentifierKey) {
					var str = value as PString;

					// Ignore ONLY if it is empty, otherwise take the user's value
					if (str is null || string.IsNullOrEmpty (str.Value))
						continue;
				}

				if (value is PDictionary)
					value = MergeEntitlementDictionary ((PDictionary) value, profile);
				else if (value is PString)
					value = MergeEntitlementString ((PString) value, profile, key == ApplicationIdentifierKey);
				else if (value is PArray)
					value = MergeEntitlementArray ((PArray) value, profile);
				else
					value = value.Clone ();

				if (value is not null)
					entitlements [key] = value;
			}

			switch (Platform) {
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				if (Debug && entitlements.TryGetValue ("com.apple.security.app-sandbox", out PBoolean? sandbox) && sandbox.Value)
					entitlements ["com.apple.security.network.client"] = new PBoolean (true);
				break;
			}

			AddCustomEntitlements (entitlements);

			return entitlements;
		}

		static PDictionary GetArchivedExpandedEntitlements (PDictionary template, PDictionary compiled)
		{
			var allowed = new HashSet<string> ();

			// the template (user-supplied Entitlements.plist file) is used to create a approved list of keys
			allowed.Add ("com.apple.developer.icloud-container-environment");
			foreach (var item in template)
				allowed.Add (item.Key!);

			// now we duplicate the allowed keys from the compiled xcent file
			var archived = new PDictionary ();

			foreach (var item in compiled) {
				var key = item.Key!;
				if (allowed.Contains (key))
					archived.Add (key, item.Value.Clone ());
			}

			return archived;
		}

		protected virtual MobileProvision GetMobileProvision (MobileProvisionPlatform platform, string name)
		{
			return MobileProvisionIndex.GetMobileProvision (platform, name);
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			MobileProvisionPlatform platform;
			MobileProvision? profile;
			PDictionary template;
			PDictionary compiled;
			PDictionary archived;
			string path;

			switch (SdkPlatform) {
			case "AppleTVSimulator":
			case "AppleTVOS":
				platform = MobileProvisionPlatform.tvOS;
				break;
			case "iPhoneSimulator":
			case "WatchSimulator":
			case "iPhoneOS":
			case "WatchOS":
				platform = MobileProvisionPlatform.iOS;
				break;
			case "MacOSX":
				platform = MobileProvisionPlatform.MacOS;
				break;
			case "MacCatalyst":
				platform = MobileProvisionPlatform.MacOS;
				break;
			default:
				Log.LogError (MSBStrings.E0048, SdkPlatform);
				return false;
			}

			if (!string.IsNullOrEmpty (ProvisioningProfile)) {
				if ((profile = GetMobileProvision (platform, ProvisioningProfile)) is null) {
					Log.LogError (MSBStrings.E0049, ProvisioningProfile);
					return false;
				}
			} else {
				profile = null;
			}

			if (!string.IsNullOrEmpty (Entitlements)) {
				if (!File.Exists (Entitlements)) {
					Log.LogError (MSBStrings.E0112, Entitlements);
					return false;
				}

				path = Entitlements;
			} else {
				path = DefaultEntitlementsPath;
			}

			try {
				template = PDictionary.FromFile (path)!;
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0113, path, ex.Message);
				return false;
			}

			compiled = GetCompiledEntitlements (profile, template);
			archived = GetArchivedExpandedEntitlements (template, compiled);

			try {
				Directory.CreateDirectory (Path.GetDirectoryName (CompiledEntitlements!.ItemSpec));
				WriteXcent (compiled, CompiledEntitlements.ItemSpec);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0114, CompiledEntitlements, ex.Message);
				return false;
			}

			SaveArchivedExpandedEntitlements (archived);

			if (Platform == Utils.ApplePlatform.MacCatalyst) {
				EntitlementsInSignature = CompiledEntitlements;
			} else if (SdkIsSimulator) {
				if (compiled.Count > 0) {
					EntitlementsInExecutable = CompiledEntitlements;
				}
			} else {
				EntitlementsInSignature = CompiledEntitlements;
			}

			return !Log.HasLoggedErrors;
		}

		bool SaveArchivedExpandedEntitlements (PDictionary archived)
		{
			if (Platform == Utils.ApplePlatform.MacCatalyst) {
				// I'm not sure if we need this in catalyst or not, but skip it until it's proven we actually need it.
				return true;
			}

			var path = Path.Combine (EntitlementBundlePath, "archived-expanded-entitlements.xcent");

			if (File.Exists (path)) {
				var plist = PDictionary.FromFile (path)!;
				var src = archived.ToXml ();
				var dest = plist.ToXml ();

				if (src == dest)
					return true;
			}

			try {
				archived.Save (path, true);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0115, ex.Message);
				return false;
			}

			return true;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (!string.IsNullOrEmpty (Entitlements))
				yield return new TaskItem (Entitlements);
			else
				yield return new TaskItem (DefaultEntitlementsPath);
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
