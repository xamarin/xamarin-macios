using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;
using System.Linq;
using System.Drawing;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	// https://developer.apple.com/documentation/technotes/tn3125-inside-code-signing-provisioning-profiles
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
		[Output] // this is required to create an output file on Windows. Note: this is a relative path.
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

		// whether the default platform entitlements (inside the <platform>.sdk directory inside Xcode) is injected into the final entitlements
		// the default behavior is to inject if no Entitlements value has been specified.
		// Note: the presence or absence of CustomEntitlements does not affect the default behavior.
		public string InjectDefaultPlatformEntitlements { get; set; } = "";

		[Output]
		public ITaskItem? EntitlementsInExecutable { get; set; }

		[Output]
		public string EntitlementsInExecutableHash { get; set; } = string.Empty;

		[Output]
		public ITaskItem? EntitlementsInSignature { get; set; }

		public string ValidateEntitlements { get; set; } = string.Empty;
		#endregion

		bool BundleEntitlementsInExecutable {
			get {
				return SdkIsSimulator;
			}
		}

		protected string ApplicationIdentifierKey {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
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

				return Path.Combine (CurrentSdk.GetSdkPath (SdkVersion, false), "Entitlements.plist");
			}
		}

		protected HashSet<string> AllowedProvisioningKeys {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
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
					return AppBundleDir;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return Path.Combine (AppBundleDir, "Contents", "Resources");
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		bool IsDeviceOrDesktop {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					return !SdkIsSimulator;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return true;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		PString MergeEntitlementString (PString pstr, MobileProvision? profile, bool expandWildcards, string? key)
		{
			string TeamIdentifierPrefix;
			string AppIdentifierPrefix;

			if (string.IsNullOrEmpty (pstr.Value))
				return (PString) pstr.Clone ();

			if (profile is null && IsDeviceOrDesktop) {
				if (!warnedTeamIdentifierPrefix && pstr.Value.Contains ("$(TeamIdentifierPrefix)")) {
					Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0108b /* Cannot expand $(TeamIdentifierPrefix) in Entitlements.plist without a provisioning profile for key '{0}' with value '{1}' */, key, pstr.Value);
					warnedTeamIdentifierPrefix = true;
				}

				if (!warnedAppIdentifierPrefix && pstr.Value.Contains ("$(AppIdentifierPrefix)")) {
					Log.LogWarning (null, null, null, Entitlements, 0, 0, 0, 0, MSBStrings.W0109b /* Cannot expand $(AppIdentifierPrefix) in Entitlements.plist without a provisioning profile for key '{0}' with value '{1}' */, key, pstr.Value);
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

		PArray? MergeEntitlementArray (PArray array, MobileProvision? profile, string? key)
		{
			var result = new PArray ();

			foreach (var item in array) {
				PObject? value;

				if (item is PDictionary)
					value = MergeEntitlementDictionary ((PDictionary) item, profile);
				else if (item is PString)
					value = MergeEntitlementString ((PString) item, profile, false, key);
				else if (item is PArray)
					value = MergeEntitlementArray ((PArray) item, profile, key);
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
					value = MergeEntitlementString ((PString) value, profile, false, item.Key);
				else if (value is PArray)
					value = MergeEntitlementArray ((PArray) value, profile, item.Key);
				else
					value = value.Clone ();

				if (value is not null)
					result.Add (item.Key!, value);
			}

			return result;
		}

		void AddCustomEntitlements (PDictionary dict, MobileProvision? profile)
		{
			PListItemGroup.Merge (
				Log,
				dict,
				CustomEntitlements,
				(value, entitlement) => MergeEntitlementString (value, profile, entitlement == ApplicationIdentifierKey, entitlement),
				MSBStrings.E7102, /* Invalid value '{0}' for the entitlement '{1}' of type '{2}' specified in the CustomEntitlements item group. Expected no value at all. */
				MSBStrings.E7103, /* "Invalid value '{0}' for the entitlement '{1}' of type '{2}' specified in the CustomEntitlements item group. Expected 'true' or 'false'." */
				MSBStrings.E7104 /* "Unknown type '{0}' for the entitlement '{1}' specified in the CustomEntitlements item group. Expected 'Remove', 'Boolean', 'String', or 'StringArray'." */);
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

		protected virtual PDictionary GetCompiledEntitlements (MobileProvision? profile, IEnumerable<PDictionary> templates)
		{
			var entitlements = new PDictionary ();

			if (profile is not null) {
				// start off with the settings from the provisioning profile
				foreach (var item in profile.Entitlements) {
					var key = item.Key!;
					if (!AllowedProvisioningKeys.Contains (key)) {
						Log.LogMessage ($"The provisioning profile '{profile.Name}' contains the entitlement '{key}', but this entitlement is not in the list of allowed entitlements, and it won't be copied into the app's entitlements.");
						continue;
					}

					var value = item.Value;

					if (value is PDictionary)
						value = MergeEntitlementDictionary ((PDictionary) value, profile);
					else if (value is PString)
						value = MergeEntitlementString ((PString) value, profile, item.Key == ApplicationIdentifierKey, key);
					else if (value is PArray)
						value = MergeEntitlementArray ((PArray) value, profile, key);
					else
						value = value.Clone ();

					if (value is not null)
						entitlements.Add (key, value);
				}
			}

			// merge in the user's values
			foreach (var template in templates) {
				foreach (var item in template) {
					var value = item.Value;
					var key = item.Key!;

					if (key == ApplicationIdentifierKey) {
						var str = value as PString;

						// Ignore ONLY if it is empty, otherwise take the user's value
						if (string.IsNullOrEmpty (str?.Value)) {
							Log.LogMessage ($"The entitlement '{key}' is empty in the provided entitlements file (either user-supplied or the default), and will be ignored.");
							continue;
						}
					}

					if (value is PDictionary)
						value = MergeEntitlementDictionary ((PDictionary) value, profile);
					else if (value is PString)
						value = MergeEntitlementString ((PString) value, profile, key == ApplicationIdentifierKey, key);
					else if (value is PArray)
						value = MergeEntitlementArray ((PArray) value, profile, key);
					else
						value = value.Clone ();

					if (value is not null)
						entitlements [key] = value;
				}
			}

			// If we're building for macOS, and we're building for Debug, and the sandbox is enabled,
			// then also enable the "com.apple.security.network.client" entitlement (it's needed for the debugger to work).
			switch (Platform) {
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				if (Debug && entitlements.TryGetValue ("com.apple.security.app-sandbox", out PBoolean? sandbox) && sandbox.Value)
					entitlements ["com.apple.security.network.client"] = new PBoolean (true);
				break;
			}

			AddCustomEntitlements (entitlements, profile);

			return entitlements;
		}

		PDictionary GetArchivedExpandedEntitlements (IEnumerable<PDictionary> templates, PDictionary compiled)
		{
			var allowed = new HashSet<string> ();

			// the template (user-supplied Entitlements.plist file) is used to create a approved list of keys
			allowed.Add ("com.apple.developer.icloud-container-environment");
			foreach (var template in templates) {
				foreach (var item in template)
					allowed.Add (item.Key!);
			}
			// also allow any custom entitlements
			foreach (var item in CustomEntitlements)
				allowed.Add (item.ItemSpec);

			// now we duplicate the allowed keys from the compiled xcent file
			var archived = new PDictionary ();

			foreach (var item in compiled) {
				var key = item.Key!;
				if (allowed.Contains (key))
					archived.Add (key, item.Value.Clone ());
			}

			return archived;
		}

		// this virtual method is required for tests
		protected virtual MobileProvision GetMobileProvision (MobileProvisionPlatform platform, string name)
		{
			return MobileProvisionIndex.GetMobileProvision (platform, name);
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();

			MobileProvisionPlatform platform;
			MobileProvision? profile;
			var templates = new List<PDictionary> ();
			PDictionary compiled;
			PDictionary? archived = null;

			switch (SdkPlatform) {
			case "AppleTVSimulator":
			case "AppleTVOS":
				platform = MobileProvisionPlatform.tvOS;
				break;
			case "iPhoneSimulator":
			case "iPhoneOS":
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

			bool injectDefaultEntitlements;
			if (!string.IsNullOrEmpty (InjectDefaultPlatformEntitlements)) {
				injectDefaultEntitlements = string.Equals (InjectDefaultPlatformEntitlements, "true", StringComparison.OrdinalIgnoreCase);
			} else if (Platform == ApplePlatform.MacOSX || Platform == ApplePlatform.MacCatalyst) {
				injectDefaultEntitlements = false;
			} else {
				injectDefaultEntitlements = string.IsNullOrEmpty (Entitlements);
			}
			if (injectDefaultEntitlements) {
				try {
					var defaultEntitlements = PDictionary.OpenFile (DefaultEntitlementsPath);
					templates.Add (defaultEntitlements);
					Log.LogMessage (MessageImportance.Low, $"Loading default entitlements from: {DefaultEntitlementsPath}");
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0113, DefaultEntitlementsPath, ex.Message);
					return false;
				}
			}

			if (!string.IsNullOrEmpty (Entitlements)) {
				try {
					if (!File.Exists (Entitlements)) {
						Log.LogError (MSBStrings.E0112, Entitlements);
						return false;
					}
					var projectEntitlements = PDictionary.OpenFile (Entitlements);
					templates.Add (projectEntitlements);
					Log.LogMessage (MessageImportance.Low, $"Loading user requested entitlements from: {Entitlements}");
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0113, Entitlements, ex.Message);
					return false;
				}
			}

			compiled = GetCompiledEntitlements (profile, templates);

			/* The path to the entitlements must be resolved to the full path, because we might want to reference it from a containing project that just references this project,
			  * and in that case it becomes a bit complicated to resolve to a full path on disk when building remotely from Windows. Instead just resolve to a full path here,
			  * and use that from now on. This has to be done from a task, so that we get the full path on the mac when executed remotely from Windows. */
			var compiledEntitlementsFullPath = Path.GetFullPath (CompiledEntitlements!.ItemSpec);
			var compiledEntitlementsFullPathItem = new TaskItem (compiledEntitlementsFullPath);

			Directory.CreateDirectory (Path.GetDirectoryName (compiledEntitlementsFullPath)!);

			if (BundleEntitlementsInExecutable) {
				// Any entitlements the app desires are stored inside the executable for simulator builds,
				// and then the executable is signed with a placeholder signature ('-') + just a single
				// entitlement (com.apple.security.get-task-allow). One consequence of storing entitlements
				// this way is that no provisioning profile will be needed to sign the executable.
				var simulatedEntitlements = compiled;
				var simulatedXcent = Path.ChangeExtension (compiledEntitlementsFullPath, "").TrimEnd ('.') + "-Simulated.xcent";
				try {
					WriteXcent (simulatedEntitlements, simulatedXcent);
				} catch (Exception ex) {
					Log.LogError (MSBStrings.E0114, simulatedXcent, ex.Message);
					return false;
				}

				EntitlementsInExecutable = new TaskItem (simulatedXcent);
				using (var stream = File.OpenRead (simulatedXcent))
				using (var sha = SHA256.Create ())
					EntitlementsInExecutableHash = Convert.ToBase64String (sha.ComputeHash (stream));

				// No matter what, I haven't been able to make Xcode apply any entitlements to the when signing simulator apps
				compiled = new PDictionary ();
			} else {
				archived = GetArchivedExpandedEntitlements (templates, compiled);
			}

			ValidateAppEntitlements (profile, compiled);

			try {
				WriteXcent (compiled, compiledEntitlementsFullPath);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0114, compiledEntitlementsFullPathItem, ex.Message);
				return false;
			}

			if (archived is not null)
				SaveArchivedExpandedEntitlements (archived);

			EntitlementsInSignature = compiledEntitlementsFullPathItem;

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
				var plist = PDictionary.OpenFile (path);
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

		[Flags]
		enum EntitlementValidationMode {
			// the entitlement must be present in the provisioning profile, but the value for the entitlement is not verified
			PresenceInProfile = 1,
			// the entitlement is a string value, and the profile contains the valid values
			// for the entitlement, which are either a string or an array of strings
			StringOrArray = 2,
			// we're guessing the entitlement must be in the provisioning profile, but we don't really know.
			// this is used to just log a message, instead of reporting a warning or an error.
			WeakPresenceInProfile = 4,
			// the entitlement must be a boolean value, and the profile contains the valid value(s)
			BooleanOrArray = 8,
		}

		class EntitlementInfo {
			public string Identifier { get; set; } = string.Empty;
			public EntitlementValidationMode ValidationMode { get; set; }
			public EntitlementData? Content { get; set; }
		}

		enum EntitlementType {
			String,
			Boolean,
			ArrayOfStrings,
			DictionaryOfArrayOfStrings,
			ArrayOfStringDictionaries,
		}

		class EntitlementData {
			public string Identifier { get; set; }
			public ApplePlatform ValidPlatforms { get; set; }
			public EntitlementType Type { get; set; }
			public string []? AdditionalKeys { get; set; }
			public bool RequiresProvisioningProfile { get; set; }

			public EntitlementData (string identifier, ApplePlatform validPlatforms, EntitlementType type, string []? additionalKeys = null, bool requiresProvisioningProfile = true)
			{
				Identifier = identifier;
				ValidPlatforms = validPlatforms;
				Type = type;
				AdditionalKeys = additionalKeys;
				RequiresProvisioningProfile = requiresProvisioningProfile;
			}
		}

		static Dictionary<string, EntitlementData>? all_entitlements;
		static Dictionary<string, EntitlementData> GetAllEntitlements ()
		{
			if (all_entitlements is null) {
				var iOS = ApplePlatform.iOS;
				var tvOS = ApplePlatform.TVOS;
				var mobile = ApplePlatform.iOS | ApplePlatform.TVOS;
				var desktop = ApplePlatform.MacOSX | ApplePlatform.MacCatalyst;
				var iOSDesktop = iOS | desktop;
				var iOSMacCatalyst = ApplePlatform.iOS | ApplePlatform.MacCatalyst;
				var allPlatforms = mobile | desktop;
				var visionOS = (ApplePlatform) 0;

				var content = new EntitlementData [] {
					new EntitlementData ("application-identifier", mobile, EntitlementType.String, requiresProvisioningProfile: true ),
					new EntitlementData ("aps-environment", mobile, EntitlementType.String ),
					new EntitlementData ("beta-reports-active", allPlatforms, EntitlementType.Boolean, requiresProvisioningProfile: true ), // https://developer.apple.com/library/archive/qa/qa1830/_index.html
					new EntitlementData ("com.apple.application-identifier", desktop, EntitlementType.String, requiresProvisioningProfile: true ),
					new EntitlementData ("com.apple.developer.accessibility.merchant-api-control", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.app-compute-category", visionOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.applesignin", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.aps-environment", desktop, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.arkit.barcode-detection.allow", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.arkit.main-camera-access.allow", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.arkit.object-tracking-parameter-adjustment.allow", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.associated-appclip-app-identifiers", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.associated-domains", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.associated-domains.applinks.read-write", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.authentication-services.autofill-credential-provider", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.automated-device-enrollment.add-devices", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.automatic-assessment-configuration", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.avfoundation.multitasking-camera-access", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.avfoundation.uvc-device-access", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.browser.app-installation", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.calling-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-audio", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-charging", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-communication", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-maps", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-messaging", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-parking", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.carplay-quick-ordering", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.ClassKit-environment", iOSDesktop, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.contacts.notes", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.coreml.neural-engine-access", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.coremotion.head-pose", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.default-data-protection", mobile, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.device-information.user-assigned-device-name", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.devicecheck.appattest-environment", mobile, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.driverkit", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.allow-any-userclient-access", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.allow-third-party-userclients", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.communicates-with-drivers", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.audio", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.block-storage-device", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.hid.device", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.hid.eventservice", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.midi", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.networking", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.scsicontroller", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.family.serial", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.transport.hid", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.driverkit.transport.pci", desktop, EntitlementType.ArrayOfStringDictionaries, new string [] { "IOPCIMatch", "IOPCIPrimaryMatch", "IOPCISecondaryMatch", "IOPCIClassMatch" } ),
					new EntitlementData ("com.apple.developer.driverkit.transport.usb", desktop, EntitlementType.ArrayOfStringDictionaries, new string [] { "bConfigurationValue", "bDeviceClass", "bDeviceProtocol", "bDeviceSubClass", "bInterfaceClass", "bInterfaceNumber", "bInterfaceProtocol", "bInterfaceSubClass", "bcdDevice", "idProduct", "idProductArray", "idProductMask", "idVendor", } ),
					new EntitlementData ("com.apple.developer.driverkit.userclient-access", desktop, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.embedded-web-browser-engine", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.endpoint-security.client", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.exposure-notification", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.family-controls", iOSMacCatalyst, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.fileprovider.testing-mode", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.game-center", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.group-session", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.healthkit", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.healthkit.access", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.healthkit.background-delivery", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.healthkit.recalibrate-estimates", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.hid.virtual.device", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.homekit", mobile, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.icloud-container-development-container-identifiers", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.icloud-container-environment", allPlatforms, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.icloud-container-identifiers", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.icloud-services", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.in-app-identity-presentment", iOS, EntitlementType.DictionaryOfArrayOfStrings, new string [] { "document-types:us-drivers-license", "elements:given-name,family-name,portrait,address,issuing-authority,document-expiration-date,document-number,driving-privileges,age,date-of-birth," } ),
					new EntitlementData ("com.apple.developer.in-app-identity-presentment.merchant-identifiers", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.in-app-payments", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.journal.allow", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.kernel.extended-virtual-addressing", mobile, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.kernel.increased-memory-limit", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.location.push", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.mail-client", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.managed-app-distribution.install-ui", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.maps", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.marketplace.app-installation", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.matter.allow-setup-payload", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.media-device-discovery-extension", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.memory.transfer_accept", iOS, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.memory.transfer_send", iOS, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.messages.critical-messaging", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.messaging-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.navigation-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.HotspotConfiguration", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.manage-thread-network-credentials", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.multicast", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.multipath", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.networkextension", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.networking.slicing.appcategory", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.networking.slicing.trafficcategory", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.networking.vmnet", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.networking.vpn.api", iOSDesktop, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.networking.wifi-info", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.nfc.hce", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.nfc.hce.default-contactless-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.nfc.hce.iso7816.select-identifier-prefixes", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.nfc.readersession.formats", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.on-demand-install-capable", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.parent-application-identifiers", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.pass-type-identifiers", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.payment-pass-provisioning", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.persistent-content-capture", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.playable-content", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.proximity-reader.identity.display", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.proximity-reader.identity.read", iOS, EntitlementType.DictionaryOfArrayOfStrings, new string [] { "document-elements:given-name,family-name,portrait,address,issuing-authority,document-issue-date,document-expiration-date,document-number,driving-privileges,age,date-of-birth", "document-types:drivers-license" } ),
					new EntitlementData ("com.apple.developer.push-to-talk", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.screen-capture.include-passthrough", visionOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.secure-element-credential", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.secure-element-credential.default-contactless-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.sensitivecontentanalysis.client", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.sensorkit.reader.allow", iOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.severe-vehicular-crash-event", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.siri", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.spatial-audio.profile-access", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.storekit.external-link.account", mobile, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.storekit.external-purchase", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.storekit.external-purchase-link", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.sustained-execution", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.system-extension.install", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.system-extension.redistributable", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.team-identifier", allPlatforms, EntitlementType.String, requiresProvisioningProfile: true ),
					new EntitlementData ("com.apple.developer.translation-app", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.ubiquity-kvstore-identifier", allPlatforms, EntitlementType.String ),
					new EntitlementData ("com.apple.developer.upi-device-validation", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.user-management", tvOS, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.developer.usernotifications.filtering", iOSDesktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.video-subscriber-single-sign-on", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.weatherkit", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser-engine.host", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser-engine.networking", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser-engine.rendering", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser-engine.webcontent", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.developer.web-browser.public-key-credential", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.external-accessory.wireless-configuration", iOS, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.security.app-sandbox", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.application-groups", allPlatforms, EntitlementType.ArrayOfStrings ),
					new EntitlementData ("com.apple.security.assets.movies.read-only", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.assets.movies.read-write", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.assets.music.read-only", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.assets.music.read-write", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.assets.pictures.read-only", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.assets.pictures.read-write", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.automation.apple-events", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.allow-dyld-environment-variables", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.allow-jit", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.allow-unsigned-executable-memory", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.debugger", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.disable-executable-page-protection", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.cs.disable-library-validation", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.device.audio-input", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.device.bluetooth", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.device.camera", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.device.microphone", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.device.usb", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.files.all", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.files.downloads.read-only", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.files.downloads.read-write", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.files.user-selected.read-only", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.files.user-selected.read-write", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.get-task-allow", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.hypervisor", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.security.network.client", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.network.server", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.personal-information.addressbook", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.personal-information.calendars", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.personal-information.location", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.personal-information.photos-library", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.print", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.smartcard", desktop, EntitlementType.Boolean, requiresProvisioningProfile: false ),
					new EntitlementData ("com.apple.security.virtualization", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.smoot.subscriptionservice", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.vm.device-access", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.vm.hypervisor", desktop, EntitlementType.Boolean ),
					new EntitlementData ("com.apple.vm.networking", desktop, EntitlementType.Boolean ),
					new EntitlementData ("get-task-allow", allPlatforms, EntitlementType.Boolean ),
					new EntitlementData ("inter-app-audio", iOS, EntitlementType.Boolean ),
					new EntitlementData ("keychain-access-groups", allPlatforms, EntitlementType.ArrayOfStrings ),
				};
				all_entitlements = new Dictionary<string, EntitlementData> (content.Length);
				foreach (var item in content) {
					if (item.Identifier is not null)
						all_entitlements [item.Identifier] = item;
				}
			}
			return all_entitlements;
		}

		static EntitlementInfo? GetEntitlementInfo (ApplePlatform platform, string entitlement)
		{
			var dict = GetAllEntitlements ();
			if (dict.TryGetValue (entitlement, out var content)) {
				var validationMode = EntitlementValidationMode.PresenceInProfile;
				switch (content.Type) {
				case EntitlementType.String:
					validationMode = EntitlementValidationMode.StringOrArray;
					break;
				case EntitlementType.Boolean:
					validationMode = EntitlementValidationMode.BooleanOrArray;
					break;
				}
				return new EntitlementInfo {
					Identifier = entitlement,
					Content = content,
					ValidationMode = validationMode,
				};
			}

			// if we don't know the entitlement, assume it must be in the profile, but only log as a message, not as a warning or error.
			return new EntitlementInfo {
				Identifier = entitlement,
				ValidationMode = EntitlementValidationMode.WeakPresenceInProfile,
			};
		}

		static string GetPListType (PObject obj)
		{
			switch (obj) {
			case PBoolean _:
				return "bool";
			case PString _:
				return "string";
			case PArray _:
				return "array";
			case PDictionary _:
				return "dict";
			case PNumber _:
				return "integer";
			case PDate _:
				return "date";
			case PData _:
				return "data";
			case PReal _:
				return "real";
			default:
				return "unknown";
			}
		}

		void ValidateAppEntitlements (MobileProvision? profile, PDictionary requestedEntitlements)
		{
			var onlyWarn = false;
			switch (ValidateEntitlements?.ToLowerInvariant ()) {
			case "disable":
				return;
			case "warn":
				onlyWarn = true;
				break;
			case null: // default to 'error'
			case "":
			case "error":
				onlyWarn = false;
				break;
			default:
				Log.LogError (7138, null, MSBStrings.E7138, ValidateEntitlements); // Invalid value '{0}' for the 'ValidateEntitlements' property. Valid values are: 'disable', 'warn' or 'error'.
				return;
			}

			if (requestedEntitlements is null || requestedEntitlements.Count == 0) {
				// Everything is OK if the app doesn't request any entitlements.
				return;
			}

			var provisioningEntitlements = profile?.Entitlements;
			var provisioningProfileName = profile?.Name;
			foreach (var kvp in requestedEntitlements) {
				var key = kvp.Key;
				// https://developer.apple.com/documentation/technotes/tn3125-inside-code-signing-provisioning-profiles#Entitlements-on-macOS
				if (key is null || string.IsNullOrEmpty (key)) {
					Log.LogMessage (MessageImportance.Low, $"The app requested an empty entitlement, with value '{kvp.Value}'");
					continue;
				}

				var info = GetEntitlementInfo (Platform, key);
				if (info is null) {
					if (profile is null) {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but nothing is known about this entitlement to validate it. Assuming this is OK.");
					} else if (provisioningEntitlements is null || !provisioningEntitlements.TryGetValue<PObject> (key, out var _)) {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but provisioning profile {provisioningProfileName} does not grant this entitlement. However, nothing is known about this entitlement to validate it, so assuming this is OK.");
					} else {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', which the provisioning profile '{provisioningProfileName}' grants. Nothing is known about this entitlement, so no further validation can be done. Assuming this is OK.");
					}
					continue;
				}

				if (info.Content?.ValidPlatforms is not null && !info.Content.ValidPlatforms.HasFlag (Platform)) {
					LogEntitlementValidationFailure (onlyWarn, 7151, MSBStrings.E7151, key, Platform.AsString (), info.Content.ValidPlatforms.AsString ()); // The app requests the entitlement '{0}', but this entitlement is not allowed on the current platform ({1}). It's only allowed on: {2}.
					continue; // no need to report any other errors for this entitlement
				}

				if (info.ValidationMode.HasFlag (EntitlementValidationMode.StringOrArray)) {
					// entitlement is a string, provisioning profile has the entitlement with either a string or an array of strings of valid values for the entitlement
					ValidateTypeOrArray<PString, string> (profile, info, onlyWarn, key, kvp, provisioningProfileName, provisioningEntitlements, "string");
				}

				if (info.ValidationMode.HasFlag (EntitlementValidationMode.BooleanOrArray)) {
					// entitlement is a boolean, provisioning profile has the entitlement with either a boolean or an array of booleans of valid values for the entitlement
					ValidateTypeOrArray<PBoolean, bool> (profile, info, onlyWarn, key, kvp, provisioningProfileName, provisioningEntitlements, "bool");
				}

				if (info.ValidationMode.HasFlag (EntitlementValidationMode.PresenceInProfile)) {
					// entitlement is present in the provisioning profile, but the value for the entitlement is not verified
					if (profile is null) {
						LogEntitlementValidationFailure (onlyWarn, 7139, MSBStrings.E7139, key); // "The app requests the entitlement '{0}', but no provisioning profile has been specified. Please specify the name of the provisioning profile to use with the 'CodesignProvision' property in the project file.
					} else if (provisioningEntitlements is null || !provisioningEntitlements.TryGetValue<PObject> (key, out var _)) {
						LogEntitlementValidationFailure (onlyWarn, 7140, MSBStrings.E7140, key, provisioningProfileName); // The app requests the entitlement '{0}', but the provisioning profile '{1}' does not contain this entitlement.
					} else {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', which the provisioning profile '{provisioningProfileName}' grants.");
					}
				}

				if (info.ValidationMode.HasFlag (EntitlementValidationMode.WeakPresenceInProfile)) {
					// we're only guessing that the entitlement must be in the provisioning profile, so only log this, instead of reporting a warning or an error
					if (profile is null) {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but no provisioning profile has been specified. This is probably not OK.");
					} else if (provisioningEntitlements is null || !provisioningEntitlements.TryGetValue<PObject> (key, out var _)) {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but provisioning profile {provisioningProfileName} does not grant this entitlement. This is probably not OK.");
					} else {
						Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', which the provisioning profile '{provisioningProfileName}' grants. This is probably OK.");
					}
				}
			}
		}

		void ValidateTypeOrArray<T, M> (MobileProvision? profile, EntitlementInfo info, bool onlyWarn, string key, KeyValuePair<string, PObject> kvp, string? provisioningProfileName, PDictionary? provisioningEntitlements, string requiredType)
			where T : PObject
		{
			// entitlement is a boolean, provisioning profile has the entitlement with either a string or an array of strings of valid values for the entitlement
			if (profile is null) {
				if (info.Content?.RequiresProvisioningProfile == false) {
					Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but no provisioning profile has been specified. This is OK, because the entitlement '{key}' doesn't require a provisioning profile.");
				} else {
					LogEntitlementValidationFailure (onlyWarn, 7139, MSBStrings.E7139, key); // "The app requests the entitlement '{0}', but no provisioning profile has been specified. Please specify the name of the provisioning profile to use with the 'CodesignProvision' property in the project file.
				}
			} else if (!(kvp.Value is T requestedPEntitlement)) {
				LogEntitlementValidationFailure (onlyWarn, 7153, MSBStrings.E7153, key, kvp.Value, GetPListType (kvp.Value), requiredType); // The app requests the entitlement '{0}' with the value '{1}' of type '{2}', but this entitlement must be of type '{3}'.
			} else if (provisioningEntitlements is null || !provisioningEntitlements.TryGetValue<PObject> (key, out var provisioningEntitlement)) {
				if (info.Content?.RequiresProvisioningProfile == false) {
					Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', but the provisioning profile '{provisioningProfileName}' doesn't contain this entitlement. This is OK, because the entitlement '{key}' doesn't require a provisioning profile.");
				} else {
					LogEntitlementValidationFailure (onlyWarn, 7140, MSBStrings.E7140, key, provisioningProfileName); // The app requests the entitlement '{0}', but the provisioning profile '{1}' doesn't contain this entitlement.
				}
			} else if (provisioningEntitlement is PArray provisioningEntitlementArray) {
				var allowedEntitlements = provisioningEntitlementArray.Cast<T> ().Select (v => GetValue<T, M> (v)).ToArray ();
				var requestedEntitlement = GetValue<T, M> (requestedPEntitlement);
				if (allowedEntitlements.Any (allowed => CompareValues (requestedEntitlement, allowed))) {
					Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}' with the value '{requestedEntitlement}, which the provisioning profile '{provisioningProfileName}' grants, because it grants these values for this entitlement: {string.Join (", ", allowedEntitlements)}.");
				} else {
					LogEntitlementValidationFailure (onlyWarn, 7152, MSBStrings.E7152, key, requestedEntitlement, provisioningProfileName, string.Join (", ", allowedEntitlements.ToArray ())); // The app requests the entitlement '{0}' with the value '{1}', but the provisioning profile '{2}' grants it for the values '{3}'.
				}
			} else if (provisioningEntitlement is T provisioningPEntitlement) {
				var allowedEntitlement = GetValue<T, M> (provisioningPEntitlement);
				var requestedEntitlement = GetValue<T, M> (requestedPEntitlement);
				if (CompareValues (requestedEntitlement, allowedEntitlement)) {
					Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}' with the value '{requestedEntitlement}', which the provisioning profile '{provisioningProfileName}' grants, because it grants this value for this entitlement: '{allowedEntitlement}'.");
				} else {
					LogEntitlementValidationFailure (onlyWarn, 7137, MSBStrings.E7137, key, requestedEntitlement, provisioningProfileName, allowedEntitlement); // The app requests the entitlement '{0}' with the value '{1}', but the provisioning profile '{2}' grants it for the value '{3}'."
				}
			} else {
				Log.LogMessage (MessageImportance.Low, $"The app requests the entitlement '{key}', which the provisioning profile '{provisioningProfileName}' contains, but with unknown values. Assuming this is OK.");
			}
		}

		static bool CompareValues<M> (M requested, M allowed)
		{
			if (requested is string strRequested && allowed is string strAllowed) {
				return IsMatch (strRequested, strAllowed);
			} else if (requested is bool bRequested && allowed is bool bAllowed) {
				return bRequested == bAllowed;
			} else {
				throw new InvalidOperationException ($"Invalid type '{typeof (M)}' found while validating entitlements");
			}
		}

		static M GetValue<T, M> (PObject obj) where T : PObject
		{
			if (obj is PBoolean b && b.Value is M b2) {
				return b2;
			} else if (obj is PString s && s.Value is M s2) {
				return s2;
			} else {
				throw new InvalidOperationException ($"Invalid type '{GetPListType (obj)}' for entitlement '{obj}'");
			}
		}

		static bool IsMatch (string value, string matchingPattern)
		{
			if (matchingPattern.IndexOf ('*') >= 0) {
				var regexp = matchingPattern.Replace (".", "[.]").Replace ("*", ".*");
				return System.Text.RegularExpressions.Regex.IsMatch (value, regexp);
			} else {
				return string.Equals (value, matchingPattern, StringComparison.OrdinalIgnoreCase);
			}
		}

		void LogEntitlementValidationFailure (bool onlyWarn, int code, string message, params object? [] args)
		{
			if (BundleEntitlementsInExecutable) {
				Log.LogMessage (MessageImportance.Low, $"Entitlements are not a part of the code signature when building for the simulator, so while this is not an error now, it would be if building for device: {string.Format (message, args)}");
			} else if (onlyWarn) {
				Log.LogWarning (code, Entitlements, message, args);
			} else if (TargetFramework.Version.Major < 11) { // Only show warnings until .NET 11
				Log.LogWarning (code, Entitlements, message, args);
			} else {
				Log.LogError (code, Entitlements, message, args);
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item)
		{
			// EntitlementsInExecutable and EntitlementsInSignature are full paths on macOS,
			// which doesn't work correctly when trying to create such output files on Windows.
			var isFullPath = item == EntitlementsInExecutable || item == EntitlementsInSignature;
			return !isFullPath;
		}

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
