using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using SecKeychain = Xamarin.MacDev.Keychain;

namespace Xamarin.MacDev.Tasks
{
	public abstract class DetectSigningIdentityTaskBase : Task
	{
		const string AutomaticProvision = "Automatic";
		const string AutomaticAdHocProvision = "Automatic:AdHoc";
		const string AutomaticAppStoreProvision = "Automatic:AppStore";
		const string AutomaticInHouseProvision = "Automatic:InHouse";
		
		protected abstract string DeveloperRoot { get; }
		protected abstract string[] DevelopmentPrefixes { get; }
		protected abstract string[] DirectDistributionPrefixes { get; }
		protected abstract string[] AppStoreDistributionPrefixes { get; }
		protected abstract PlatformFramework Framework { get; }
		protected abstract string PlatformName { get ; }
		protected abstract string ApplicationIdentifierKey { get; }

		string provisioningProfileName;
		string codesignCommonName;

		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleName { get; set; }

		[Required]
		public string AppManifest { get; set; }

		public string Keychain { get; set; }

		public string SigningKey { get; set; }

		public string ProvisioningProfile { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		public bool RequireCodeSigning { get; set; }

		public bool RequireProvisioningProfile { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DetectedAppId { get; set; }

		[Output]
		public string DetectedBundleId { get; set; }

		[Output]
		public string DetectedBundleVersion { get; set; }

		[Output]
		public string DetectedCodeSigningKey { get; set; }

		[Output]
		public string DetectedCodesignAllocate { get; set; }

		[Output]
		public string DetectedDistributionType { get; set; }

		[Output]
		public string DetectedProvisioningProfile { get; set; }

		#endregion

		class CodeSignIdentity
		{
			public X509Certificate2 SigningKey { get; set; }
			public MobileProvision Profile { get; set; }
			public string BundleId { get; set; }
			public string AppId { get; set; }

			public CodeSignIdentity Clone ()
			{
				return new CodeSignIdentity {
					SigningKey = SigningKey,
					Profile = Profile,
					BundleId = BundleId,
					AppId = AppId
				};
			}
		}

		static bool IsAutoCodeSignProfile (string value)
		{
			if (string.IsNullOrEmpty (value))
				return true;

			switch (value) {
			case AutomaticAppStoreProvision:
			case AutomaticAdHocProvision:
			case AutomaticInHouseProvision:
			case AutomaticProvision:
				return true;
			default:
				return false;
			}
		}

		static string GetDefaultBundleId (string bundleName, string suggestion)
		{
			if (string.IsNullOrEmpty (suggestion))
				return "com.your-company." + bundleName;

			if (suggestion.EndsWith ("*", StringComparison.Ordinal))
				return suggestion.Substring (0, suggestion.Length - 1) + bundleName;

			return suggestion;
		}

		string GetProfileBundleId (MobileProvision provision)
		{
			if (!provision.Entitlements.ContainsKey (ApplicationIdentifierKey))
				return null;

			var id = ((PString) provision.Entitlements [ApplicationIdentifierKey]).Value;
			int i = id.IndexOf ('.') + 1;
			if (i > 0 && i < id.Length)
				return id.Substring (i);

			return null;
		}

		string ConstructValidAppId (MobileProvision provision, string bundleId)
		{
			int matchLength;

			return ConstructValidAppId (provision, bundleId, out matchLength);
		}

		string ConstructValidAppId (MobileProvision provision, string bundleId, out int matchLength)
		{
			if (!provision.Entitlements.ContainsKey (ApplicationIdentifierKey)) {
				matchLength = 0;
				return null;
			}

			return ConstructValidAppId (
				provision.ApplicationIdentifierPrefix[0] + "." + bundleId,
				((PString) provision.Entitlements [ApplicationIdentifierKey]).Value,
				out matchLength
			);
		}

		static string ConstructValidAppId (string appid, string allowed, out int matchLength)
		{
			// The user can't have a wildcard ID as their actual app id
			if (appid.Contains ("*")) {
				matchLength = 0;
				return null;
			}

			// Next check if we have an exact match
			if (allowed == appid) {
				matchLength = allowed.Length;
				return appid;
			}

			// Finally if the profile is a wildcard, ensure that the appid matches it for everything before the '*'
			int star = allowed.IndexOf ('*');
			if (star != -1 && star + 1 == allowed.Length && appid.Length >= star && appid.StartsWith (allowed.Substring (0, star), StringComparison.Ordinal)) {
				matchLength = star;
				return appid;
			}

			// It does not match
			matchLength = 0;

			return null;
		}

		void ReportDetectedCodesignInfo ()
		{
			Log.LogMessage (MessageImportance.High, "Detected signing identity:");
			if (codesignCommonName != null)
				Log.LogMessage (MessageImportance.High, "  Code Signing Key: \"{0}\" ({1})", codesignCommonName, DetectedCodeSigningKey);
			if (provisioningProfileName != null)
				Log.LogMessage (MessageImportance.High, "  Provisioning Profile: \"{0}\" ({1})", provisioningProfileName, DetectedProvisioningProfile);
			Log.LogMessage (MessageImportance.High, "  Bundle Id: {0}", DetectedBundleId);
			Log.LogMessage (MessageImportance.High, "  App Id: {0}", DetectedAppId);
		}

		static bool MatchesAny (string name, string[] names)
		{
			for (int i = 0; i < names.Length; i++) {
				if (name == names[i])
					return true;
			}

			return false;
		}

		static bool StartsWithAny (string name, string[] prefixes)
		{
			foreach (var prefix in prefixes) {
				if (name.StartsWith (prefix, StringComparison.Ordinal))
					return true;
			}

			return false;
		}

		bool TryGetSigningCertificates (SecKeychain keychain, out IList<X509Certificate2> certs, string[] prefixes)
		{
			var now = DateTime.Now;

			certs = keychain.FindNamedSigningCertificates (x => StartsWithAny (x, prefixes)).Where (x => now < x.NotAfter).ToList ();

			if (certs.Count == 0) {
				if (SdkIsSimulator) {
					// We don't codesign Simulator builds, so this is actually valid...
					return true;
				}

				var message = "No valid " + PlatformName + " code signing keys found in keychain. You need to request a codesigning certificate from https://developer.apple.com.";

				Log.LogError (message);
				return false;
			}

			return true;
		}

		bool TryGetSigningCertificates (SecKeychain keychain, out IList<X509Certificate2> certs, string name)
		{
			var now = DateTime.Now;

			certs = keychain.FindNamedSigningCertificates (x => x == name).Where (x => now < x.NotAfter).ToList ();

			if (certs.Count == 0) {
				Log.LogError (PlatformName + " code signing key '{0}' not found in keychain.", SigningKey);
				return false;
			}

			return true;
		}

		class SigningIdentityComparer : IComparer<CodeSignIdentity>
		{
			public int Compare (CodeSignIdentity x, CodeSignIdentity y)
			{
				// reverse sort by provisioning profile creation date
				return y.Profile.CreationDate.CompareTo (x.Profile.CreationDate);
			}
		}

		public override bool Execute ()
		{
			var type = MobileProvisionDistributionType.Any;
			var identity = new CodeSignIdentity ();
			MobileProvisionPlatform platform;
			IList<MobileProvision> profiles;
			IList<X509Certificate2> certs;
			PDictionary plist;

			Log.LogTaskName ("DetectSigningIdentity");
			Log.LogTaskProperty ("AppBundleName", AppBundleName);
			Log.LogTaskProperty ("AppManifest", AppManifest);
			Log.LogTaskProperty ("Keychain", Keychain);
			Log.LogTaskProperty ("ProvisioningProfile", ProvisioningProfile);
			Log.LogTaskProperty ("RequireCodesigning", RequireCodeSigning);
			Log.LogTaskProperty ("SdkPlatform", SdkPlatform);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);
			Log.LogTaskProperty ("SigningKey", SigningKey);

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
			default:
				Log.LogError ("Unknown SDK platform: {0}", SdkPlatform);
				return false;
			}

			if (ProvisioningProfile == AutomaticAppStoreProvision)
				type = MobileProvisionDistributionType.AppStore;
			else if (ProvisioningProfile == AutomaticInHouseProvision)
				type = MobileProvisionDistributionType.InHouse;
			else if (ProvisioningProfile == AutomaticAdHocProvision)
				type = MobileProvisionDistributionType.AdHoc;

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, "Error loading '{0}': {1}", AppManifest, ex.Message);
				return false;
			}

			DetectedCodesignAllocate = Path.Combine (DeveloperRoot, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "codesign_allocate");
			DetectedBundleVersion = plist.GetCFBundleVersion ();
			DetectedDistributionType = type.ToString ();

			identity.BundleId = plist.GetCFBundleIdentifier ();
			if (string.IsNullOrEmpty (identity.BundleId))
				identity.BundleId = null;

			if (Framework == PlatformFramework.MacOS && !RequireCodeSigning) {
				DetectedBundleId = identity.BundleId ?? GetDefaultBundleId (AppBundleName, null);
				DetectedAppId = DetectedBundleId;

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			try {
				var keychain = !string.IsNullOrEmpty (Keychain) ? SecKeychain.Open (Keychain) : SecKeychain.Default;

				if (string.IsNullOrEmpty (SigningKey) || MatchesAny (SigningKey, DevelopmentPrefixes)) {
					// Note: we treat an empty signing key as "developer automatic".
					if (!TryGetSigningCertificates (keychain, out certs, DevelopmentPrefixes))
						return false;
				} else if (MatchesAny (SigningKey, AppStoreDistributionPrefixes)) {
					if (!TryGetSigningCertificates (keychain, out certs, AppStoreDistributionPrefixes))
						return false;
				} else if (MatchesAny (SigningKey, DirectDistributionPrefixes)) {
					if (!TryGetSigningCertificates (keychain, out certs, DirectDistributionPrefixes))
						return false;
				} else {
					// The user has specified an exact name to match...
					if (!TryGetSigningCertificates (keychain, out certs, SigningKey))
						return false;
				}
			} catch (Exception ex) {
				Log.LogError ("{0}", ex.Message);
				return false;
			}

			if (!RequireProvisioningProfile && string.IsNullOrEmpty (ProvisioningProfile)) {
				if (certs.Count > 0) {
					if (certs.Count > 1) {
						if (!string.IsNullOrEmpty (SigningKey))
							Log.LogMessage (MessageImportance.Normal, "Multiple signing identities match '{0}'; using the first match.", SigningKey);
						else
							Log.LogMessage (MessageImportance.Normal, "Multiple signing identities found; using the first identity.");

						for (int i = 0; i < certs.Count; i++) {
							Log.LogMessage (MessageImportance.Normal, "{0,3}. Signing Identity: {1} ({2})", i + 1,
							                SecKeychain.GetCertificateCommonName (certs[i]), certs[i].Thumbprint);
						}
					}

					codesignCommonName = SecKeychain.GetCertificateCommonName (certs[0]);
					DetectedCodeSigningKey = certs[0].Thumbprint;
				}

				DetectedBundleId = identity.BundleId ?? GetDefaultBundleId (AppBundleName, null);
				DetectedAppId = DetectedBundleId;

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			if (!IsAutoCodeSignProfile (ProvisioningProfile)) {
				identity.Profile = MobileProvisionIndex.GetMobileProvision (platform, ProvisioningProfile);

				if (identity.Profile == null) {
					Log.LogError ("The specified " + PlatformName + " provisioning profile '{0}' could not be found", ProvisioningProfile);
					return false;
				}

				var profile = identity.Profile; // capture ref for lambda

				if (certs.Count > 0) {
					identity.SigningKey = certs.FirstOrDefault (c => profile.DeveloperCertificates.Any (p => p.Thumbprint == c.Thumbprint));
					if (identity.SigningKey == null) {
						Log.LogError ("No " + PlatformName + " signing identities match the specified provisioning profile '{0}'.", ProvisioningProfile);
						return false;
					}
				}

				if (identity.BundleId == null) {
					identity.BundleId = GetDefaultBundleId (AppBundleName, GetProfileBundleId (identity.Profile));
					Log.LogWarning (null, null, null, AppManifest, 0, 0, 0, 0, "Project does not have bundle identifier specified. Using '{0}' to match provisioning profile.", identity.BundleId);
				}

				identity.AppId = ConstructValidAppId (identity.Profile, identity.BundleId);
				if (identity.AppId == null) {
					Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, "Project bundle identifier '{0}' does not match specified provisioning profile '{1}'", identity.BundleId, ProvisioningProfile);
					return false;
				}

				if (identity.SigningKey != null) {
					codesignCommonName = SecKeychain.GetCertificateCommonName (identity.SigningKey);
					DetectedCodeSigningKey = identity.SigningKey.Thumbprint;
				}

				provisioningProfileName = identity.Profile.Name;

				DetectedProvisioningProfile = identity.Profile.Uuid;
				DetectedDistributionType = identity.Profile.DistributionType.ToString ();
				DetectedBundleId = identity.BundleId;
				DetectedAppId = identity.AppId;

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			if (identity.BundleId != null) {
				if (certs.Count > 0)
					profiles = MobileProvisionIndex.GetMobileProvisions (platform, identity.BundleId, type, certs);
				else
					profiles = MobileProvisionIndex.GetMobileProvisions (platform, identity.BundleId, type);
			} else if (certs.Count > 0) {
				profiles = MobileProvisionIndex.GetMobileProvisions (platform, type, certs);
			} else {
				profiles = MobileProvisionIndex.GetMobileProvisions (platform, type);
			}

			List<CodeSignIdentity> pairs;

			if (certs.Count > 0) {
				pairs = (from p in profiles
						 from c in certs
						 where p.DeveloperCertificates.Any (d => d.Thumbprint == c.Thumbprint)
						 select new CodeSignIdentity { SigningKey = c, Profile = p }).ToList ();

				if (pairs.Count == 0) {
					Log.LogError ("No installed provisioning profiles match the installed " + PlatformName + " signing identities.");
					return false;
				}
			} else {
				pairs = (from p in profiles select new CodeSignIdentity { Profile = p }).ToList ();
			}

			if (identity.BundleId != null) {
				var matches = new List<CodeSignIdentity> ();
				int bestMatchLength = 0;
				int matchLength;

				// find matching provisioning profiles with compatible appid, keeping only those with the longest matching (wildcard) ids
				foreach (var pair in pairs) {
					var appid = ConstructValidAppId (pair.Profile, identity.BundleId, out matchLength);
					if (appid != null && matchLength >= bestMatchLength) {
						if (matchLength > bestMatchLength) {
							bestMatchLength = matchLength;
							matches.Clear ();
						}

						var match = identity.Clone ();
						match.SigningKey = pair.SigningKey;
						match.Profile = pair.Profile;
						match.AppId = appid;

						matches.Add (match);
					}
				}

				if (matches.Count == 0) {
					Log.LogWarning (null, null, null, AppManifest, 0, 0, 0, 0, "No installed provisioning profiles match the bundle identifier.");
				} else {
					if (matches.Count > 1) {
						var spaces = new string (' ', 3);

						Log.LogMessage (MessageImportance.Normal, "Multiple provisioning profiles match the bundle identifier; using the first match.");

						matches.Sort (new SigningIdentityComparer ());

						for (int i = 0; i < matches.Count; i++) {
							Log.LogMessage (MessageImportance.Normal, "{0,3}. Provisioning Profile: \"{1}\" ({2})", i + 1, matches[i].Profile.Name, matches[i].Profile.Uuid);

							if (matches[i].SigningKey != null)
								Log.LogMessage (MessageImportance.Normal, "{0}  Signing Identity: \"{1}\"", spaces, SecKeychain.GetCertificateCommonName (matches[i].SigningKey));
						}
					}

					identity = matches[0];
				}
			} else {
				// pick a provisioning profile to provide appid and better default bundle identifier, preferring wildcard bundle identifiers
				foreach (var pair in pairs) {
					var suggestion = GetProfileBundleId (pair.Profile);
					bool wildcard = (suggestion != null) && suggestion.EndsWith ("*", StringComparison.Ordinal);

					if (wildcard || identity.Profile == null) {
						identity.Profile = pair.Profile;
						identity.SigningKey = pair.SigningKey;
						identity.BundleId = GetDefaultBundleId (AppBundleName, suggestion);
						identity.AppId = ConstructValidAppId (pair.Profile, identity.BundleId);
					}

					if (wildcard)
						break;
				}

				Log.LogWarning (null, null, null, AppManifest, 0, 0, 0, 0, "No bundle identifier specified. Using '{0}' to match an installed provisioning profile.", identity.BundleId);
			}

			if (identity.Profile != null && identity.AppId != null) {
				codesignCommonName = identity.SigningKey != null ? SecKeychain.GetCertificateCommonName (identity.SigningKey) : null;
				provisioningProfileName = identity.Profile.Name;

				DetectedCodeSigningKey = identity.SigningKey?.Thumbprint;
				DetectedProvisioningProfile = identity.Profile.Uuid;
				DetectedBundleId = identity.BundleId;
				DetectedAppId = identity.AppId;

				ReportDetectedCodesignInfo ();
			} else {
				if (identity.SigningKey != null) {
					Log.LogError ("Bundle identifier '{0}' does not match any installed provisioning profile for selected signing identity '{0}'.", identity.BundleId, identity.SigningKey);
				} else {
					Log.LogError ("Bundle identifier '{0}' does not match any installed provisioning profile.", identity.BundleId);
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
