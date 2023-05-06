using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Build.Framework;

using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

using SecKeychain = Xamarin.MacDev.Keychain;

namespace Xamarin.MacDev.Tasks {
	public abstract class DetectSigningIdentityTaskBase : XamarinTask {
		const string AutomaticProvision = "Automatic";
		const string AutomaticAdHocProvision = "Automatic:AdHoc";
		const string AutomaticAppStoreProvision = "Automatic:AppStore";
		const string AutomaticInHouseProvision = "Automatic:InHouse";

		static readonly string [] macAppStoreDistributionPrefixes = { "3rd Party Mac Developer Application", "Apple Distribution" };
		static readonly string [] macDirectDistributionPrefixes = { "Developer ID Application" };
		static readonly string [] macDevelopmentPrefixes = { "Mac Developer", "Apple Development" };

		protected string DeveloperRoot {
			get {
				return Sdks.GetAppleSdk (TargetFrameworkMoniker).DeveloperRoot;
			}
		}

		protected string [] DevelopmentPrefixes {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return IPhoneCertificate.DevelopmentPrefixes;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return macDevelopmentPrefixes;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected string [] DirectDistributionPrefixes {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return Array.Empty<string> ();
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return macDirectDistributionPrefixes;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

		protected string [] AppStoreDistributionPrefixes {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
					return IPhoneCertificate.DistributionPrefixes;
				case ApplePlatform.MacOSX:
				case ApplePlatform.MacCatalyst:
					return macAppStoreDistributionPrefixes;
				default:
					throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
				}
			}
		}

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

		string provisioningProfileName;
		string codesignCommonName;

		#region Inputs

		[Required]
		public string AppBundleName { get; set; }

		public string BundleIdentifier { get; set; }

		public string CodesignProvision { get; set; }

		public ITaskItem CodesignEntitlements { get; set; }

		public string CodesignRequireProvisioningProfile { get; set; }

		public string Keychain { get; set; }

		public string SigningKey { get; set; }

		public string ProvisioningProfile { get; set; }

		[Required]
		public string SdkPlatform { get; set; }

		public bool SdkIsSimulator { get; set; }

		public bool RequireCodeSigning { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DetectedAppId { get; set; }

		// This is input too
		[Output]
		public string DetectedCodeSigningKey { get; set; }

		[Output]
		public string DetectedCodesignAllocate { get; set; }

		[Output]
		public string DetectedDistributionType { get; set; }

		[Output]
		public string DetectedProvisioningProfile { get; set; }

		#endregion

		bool? requireProvisioningProfile;
		public bool RequireProvisioningProfile {
			get {
				// RequireProvisioningProfile:
				// * iOS, tvOS, watchOS: required if building for device or if a custom (.NET: non-empty) entitlement file is used
				// * macOS, Mac Catalyst: requirerd if a provisioning profile is specified
				// * Default logic is overridable by setting the "CodesignRequireProvisioningProfile=true|false" property

				if (!requireProvisioningProfile.HasValue) {
					if (string.IsNullOrEmpty (CodesignRequireProvisioningProfile)) {
						switch (Platform) {
						case ApplePlatform.iOS:
						case ApplePlatform.TVOS:
						case ApplePlatform.WatchOS:
							requireProvisioningProfile = !SdkIsSimulator || HasEntitlements;
							break;
						case ApplePlatform.MacCatalyst:
						case ApplePlatform.MacOSX:
							requireProvisioningProfile = !string.IsNullOrEmpty (CodesignProvision);
							break;
						default:
							throw new InvalidOperationException (string.Format (MSBStrings.InvalidPlatform, Platform));
						}
					} else {
						requireProvisioningProfile = string.Equals (CodesignRequireProvisioningProfile, "true", StringComparison.OrdinalIgnoreCase);
					}
				}
				return requireProvisioningProfile.Value;
			}
		}

		bool? hasEntitlements;
		bool HasEntitlements {
			get {
				if (!hasEntitlements.HasValue) {
					if (string.IsNullOrEmpty (CodesignEntitlements?.ItemSpec)) {
						// If no CodesignEntitlements was specified, we don't have any entitlements
						hasEntitlements = false;
					} else {
						if (IsDotNet) {
							// .NET: Check the file to see if there are any entitlements inside
							var entitlements = PDictionary.FromFile (CodesignEntitlements.ItemSpec);
							hasEntitlements = entitlements.Count > 0;
						} else {
							// Legacy Xamarin: to preserve backwards compat, consider the presence of a file enough to say we have entitlements.
							hasEntitlements = true;
						}
					}
				}
				return hasEntitlements.Value;
			}
		}

		class CodeSignIdentity {
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
				provision.ApplicationIdentifierPrefix [0] + "." + bundleId,
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
			Log.LogMessage (MessageImportance.High, MSBStrings.M0125);
			if (codesignCommonName is not null)
				Log.LogMessage (MessageImportance.High, "  Code Signing Key: \"{0}\" ({1})", codesignCommonName, DetectedCodeSigningKey);
			if (provisioningProfileName is not null)
				Log.LogMessage (MessageImportance.High, "  Provisioning Profile: \"{0}\" ({1})", provisioningProfileName, DetectedProvisioningProfile);
			Log.LogMessage (MessageImportance.High, "  Bundle Id: {0}", BundleIdentifier);
			Log.LogMessage (MessageImportance.High, "  App Id: {0}", DetectedAppId);
		}

		static bool MatchesAny (string name, string [] names)
		{
			for (int i = 0; i < names.Length; i++) {
				if (name == names [i])
					return true;
			}

			return false;
		}

		static bool StartsWithAny (string name, string [] prefixes)
		{
			foreach (var prefix in prefixes) {
				if (name.StartsWith (prefix, StringComparison.Ordinal))
					return true;
			}

			return false;
		}

		bool TryGetSigningCertificates (SecKeychain keychain, out IList<X509Certificate2> certs, string [] prefixes, bool allowZeroCerts)
		{
			var now = DateTime.Now;

			certs = new List<X509Certificate2> ();
			foreach (var certificate in keychain.GetAllSigningCertificates ()) {
				var cname = SecKeychain.GetCertificateCommonName (certificate);

				if (!StartsWithAny (cname, prefixes)) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0126, cname, string.Join ("', '", prefixes));
					continue;
				}

				if (now >= certificate.NotAfter) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0127, cname, certificate.NotAfter);
					continue;
				}

				certs.Add (certificate);
			}

			if (certs.Count == 0 && !allowZeroCerts) {
				var message = String.Format (MSBStrings.E0128, PlatformName);

				Log.LogError (message);
				return false;
			}

			return true;
		}

		bool TryGetSigningCertificates (SecKeychain keychain, out IList<X509Certificate2> certs, string name)
		{
			var now = DateTime.Now;

			certs = new List<X509Certificate2> ();
			foreach (var certificate in keychain.GetAllSigningCertificates ()) {
				var cname = SecKeychain.GetCertificateCommonName (certificate);

				if (!name.Equals (certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) && name != cname) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0129, cname, name);
					continue;
				}

				if (now >= certificate.NotAfter) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0127, cname, certificate.NotAfter);
					continue;
				}

				certs.Add (certificate);
			}

			if (certs.Count == 0) {
				Log.LogError (MSBStrings.E0130, PlatformName, SigningKey);
				return false;
			}

			return true;
		}

		bool TryGetSigningCertificates (out IList<X509Certificate2> certs, bool allowZeroCerts)
		{
			try {
				var keychain = !string.IsNullOrEmpty (Keychain) ? SecKeychain.Open (Keychain) : SecKeychain.Default;

				if (string.IsNullOrEmpty (SigningKey) || MatchesAny (SigningKey, DevelopmentPrefixes)) {
					// Note: we treat an empty signing key as "developer automatic".
					if (!TryGetSigningCertificates (keychain, out certs, DevelopmentPrefixes, allowZeroCerts))
						return false;
				} else if (MatchesAny (SigningKey, AppStoreDistributionPrefixes)) {
					if (!TryGetSigningCertificates (keychain, out certs, AppStoreDistributionPrefixes, false))
						return false;
				} else if (MatchesAny (SigningKey, DirectDistributionPrefixes)) {
					if (!TryGetSigningCertificates (keychain, out certs, DirectDistributionPrefixes, false))
						return false;
				} else {
					// The user has specified an exact name to match...
					if (!TryGetSigningCertificates (keychain, out certs, SigningKey))
						return false;
				}

				return true;
			} catch (Exception ex) {
				Log.LogError ("{0}", ex.Message);
				certs = null;
				return false;
			}
		}

		class SigningIdentityComparer : IComparer<CodeSignIdentity> {
			public int Compare (CodeSignIdentity x, CodeSignIdentity y)
			{
				// reverse sort by provisioning profile creation date
				return y.Profile.CreationDate.CompareTo (x.Profile.CreationDate);
			}
		}

		IList<MobileProvision> GetProvisioningProfiles (MobileProvisionPlatform platform, MobileProvisionDistributionType type, CodeSignIdentity identity, IList<X509Certificate2> certs)
		{
			var failures = new List<string> ();
			IList<MobileProvision> profiles;

			if (identity.BundleId is not null) {
				if (certs.Count > 0)
					profiles = MobileProvisionIndex.GetMobileProvisions (platform, identity.BundleId, type, certs, unique: true, failures: failures);
				else
					profiles = MobileProvisionIndex.GetMobileProvisions (platform, identity.BundleId, type, unique: true, failures: failures);
			} else if (certs.Count > 0) {
				profiles = MobileProvisionIndex.GetMobileProvisions (platform, type, certs, unique: true, failures: failures);
			} else {
				profiles = MobileProvisionIndex.GetMobileProvisions (platform, type, unique: true, failures: failures);
			}

			if (profiles.Count == 0) {
				foreach (var f in failures)
					Log.LogMessage (MessageImportance.Low, "{0}", f);

				Log.LogError (MSBStrings.E0131, AppBundleName, PlatformName);
				return null;
			}

			Log.LogMessage (MessageImportance.Low, "Available profiles:");
			foreach (var p in profiles)
				Log.LogMessage (MessageImportance.Low, "    {0}", p.Name);

			return profiles;
		}

		List<CodeSignIdentity> GetCodeSignIdentityPairs (IList<MobileProvision> profiles, IList<X509Certificate2> certs)
		{
			List<CodeSignIdentity> pairs;

			if (certs.Count > 0) {
				pairs = (from p in profiles
						 from c in certs
						 where p.DeveloperCertificates.Any (d => {
							 var rv = d.Thumbprint == c.Thumbprint;
							 if (!rv)
								 Log.LogMessage (MessageImportance.Low, MSBStrings.M0132, d.Thumbprint, c.Thumbprint);
							 return rv;
						 })
						 select new CodeSignIdentity { SigningKey = c, Profile = p }).ToList ();

				if (pairs.Count == 0) {
					Log.LogError (MSBStrings.E0133, PlatformName, AppBundleName);
					return null;
				}
			} else {
				pairs = (from p in profiles select new CodeSignIdentity { Profile = p }).ToList ();
			}

			return pairs;
		}

		CodeSignIdentity GetBestMatch (List<CodeSignIdentity> pairs, CodeSignIdentity identity)
		{
			var matches = new List<CodeSignIdentity> ();
			int bestMatchLength = 0;
			int matchLength;

			// find matching provisioning profiles with compatible appid, keeping only those with the longest matching (wildcard) ids
			Log.LogMessage (MessageImportance.Low, MSBStrings.M0134);
			foreach (var pair in pairs) {
				var appid = ConstructValidAppId (pair.Profile, identity.BundleId, out matchLength);
				if (appid is not null) {
					if (matchLength >= bestMatchLength) {
						if (matchLength > bestMatchLength) {
							bestMatchLength = matchLength;
							foreach (var previousMatch in matches)
								Log.LogMessage (MessageImportance.Low, MSBStrings.M0135, previousMatch.AppId, appid);
							matches.Clear ();
						}

						var match = identity.Clone ();
						match.SigningKey = pair.SigningKey;
						match.Profile = pair.Profile;
						match.AppId = appid;

						matches.Add (match);
					} else {
						string currentMatches = "";
						foreach (var match in matches)
							currentMatches += $"{match}; ";
						Log.LogMessage (MessageImportance.Low, MSBStrings.M0136, appid, currentMatches);
					}
				}
			}

			if (matches.Count == 0) {
				Log.LogWarning (MSBStrings.W0137);
				return identity;
			}

			if (matches.Count > 1) {
				var spaces = new string (' ', 3);

				Log.LogMessage (MessageImportance.Normal, MSBStrings.M0138);

				matches.Sort (new SigningIdentityComparer ());

				for (int i = 0; i < matches.Count; i++) {
					Log.LogMessage (MessageImportance.Normal, "{0,3}. Provisioning Profile: \"{1}\" ({2})", i + 1, matches [i].Profile.Name, matches [i].Profile.Uuid);

					if (matches [i].SigningKey is not null)
						Log.LogMessage (MessageImportance.Normal, "{0}  Signing Identity: \"{1}\"", spaces, SecKeychain.GetCertificateCommonName (matches [i].SigningKey));
				}
			}

			return matches [0];
		}

		public override bool Execute ()
		{
			var type = MobileProvisionDistributionType.Any;
			var identity = new CodeSignIdentity ();
			MobileProvisionPlatform platform;
			IList<MobileProvision> profiles;
			IList<X509Certificate2> certs;
			List<CodeSignIdentity> pairs;

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

			if (ProvisioningProfile == AutomaticAppStoreProvision)
				type = MobileProvisionDistributionType.AppStore;
			else if (ProvisioningProfile == AutomaticInHouseProvision)
				type = MobileProvisionDistributionType.InHouse;
			else if (ProvisioningProfile == AutomaticAdHocProvision)
				type = MobileProvisionDistributionType.AdHoc;

			DetectedCodesignAllocate = Path.Combine (DeveloperRoot, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", "codesign_allocate");
			DetectedDistributionType = type.ToString ();

			identity.BundleId = BundleIdentifier;
			DetectedAppId = BundleIdentifier; // default value that can be changed below

			if (Platform == ApplePlatform.MacOSX) {
				if (!RequireCodeSigning || !string.IsNullOrEmpty (DetectedCodeSigningKey)) {
					ReportDetectedCodesignInfo ();

					return !Log.HasLoggedErrors;
				}
			} else if (Platform == ApplePlatform.MacCatalyst) {
				var doesNotNeedCodeSigningCertificate = !RequireCodeSigning || !string.IsNullOrEmpty (DetectedCodeSigningKey);
				if (RequireProvisioningProfile)
					doesNotNeedCodeSigningCertificate = false;
				if (doesNotNeedCodeSigningCertificate) {
					DetectedCodeSigningKey = "-";

					ReportDetectedCodesignInfo ();

					return !Log.HasLoggedErrors;
				}
			} else {
				// Framework is either iOS, tvOS or watchOS
				if (SdkIsSimulator) {
					if (AppleSdkSettings.XcodeVersion.Major >= 8 && RequireProvisioningProfile) {
						// Note: Starting with Xcode 8.0, we need to codesign iOS Simulator builds that enable Entitlements
						// in order for them to run. The "-" key is a special value allowed by the codesign utility that
						// allows us to get away with not having an actual codesign key.
						DetectedCodeSigningKey = "-";

						if (!IsAutoCodeSignProfile (ProvisioningProfile)) {
							identity.Profile = MobileProvisionIndex.GetMobileProvision (platform, ProvisioningProfile);

							if (identity.Profile is null) {
								Log.LogError (MSBStrings.E0140, PlatformName, ProvisioningProfile);
								return false;
							}

							identity.AppId = ConstructValidAppId (identity.Profile, identity.BundleId);
							if (identity.AppId is null) {
								Log.LogError (MSBStrings.E0141, identity.BundleId, ProvisioningProfile);
								return false;
							}

							provisioningProfileName = identity.Profile.Name;

							DetectedProvisioningProfile = identity.Profile.Uuid;
							DetectedDistributionType = identity.Profile.DistributionType.ToString ();
						} else {
							certs = new X509Certificate2 [0];

							if ((profiles = GetProvisioningProfiles (platform, type, identity, certs)) is null)
								return false;

							if ((pairs = GetCodeSignIdentityPairs (profiles, certs)) is null)
								return false;

							var match = GetBestMatch (pairs, identity);
							identity.Profile = match.Profile;
							identity.AppId = match.AppId;

							if (identity.Profile is not null) {
								DetectedDistributionType = identity.Profile.DistributionType.ToString ();
								DetectedProvisioningProfile = identity.Profile.Uuid;
								provisioningProfileName = identity.Profile.Name;
							}

							DetectedAppId = identity.AppId;
						}
					} else {
						// Note: Do not codesign. Codesigning seems to break the iOS Simulator in older versions of Xcode.
						DetectedCodeSigningKey = null;
					}

					ReportDetectedCodesignInfo ();

					return !Log.HasLoggedErrors;
				}

				if (!SdkIsSimulator && !RequireCodeSigning) {
					// The "-" key is a special value allowed by the codesign utility that
					// allows us to get away with not having an actual codesign key.
					DetectedCodeSigningKey = "-";

					ReportDetectedCodesignInfo ();

					return !Log.HasLoggedErrors;
				}
			}

			// Note: if we make it this far, we absolutely need a codesigning certificate
			if (!TryGetSigningCertificates (out certs, false))
				return false;

			Log.LogMessage (MessageImportance.Low, "Available certificates:");
			foreach (var cert in certs)
				Log.LogMessage (MessageImportance.Low, "    {0}", SecKeychain.GetCertificateCommonName (cert));

			if (!RequireProvisioningProfile) {
				if (certs.Count > 1) {
					if (!string.IsNullOrEmpty (SigningKey))
						Log.LogMessage (MessageImportance.Normal, MSBStrings.M0142, SigningKey);
					else
						Log.LogMessage (MessageImportance.Normal, MSBStrings.M0143);

					for (int i = 0; i < certs.Count; i++) {
						Log.LogMessage (MessageImportance.Normal, "{0,3}. Signing Identity: {1} ({2})", i + 1,
										SecKeychain.GetCertificateCommonName (certs [i]), certs [i].Thumbprint);
					}
				}

				codesignCommonName = SecKeychain.GetCertificateCommonName (certs [0]);
				DetectedCodeSigningKey = certs [0].Thumbprint;

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			if (!IsAutoCodeSignProfile (ProvisioningProfile)) {
				identity.Profile = MobileProvisionIndex.GetMobileProvision (platform, ProvisioningProfile);

				if (identity.Profile is null) {
					Log.LogError (MSBStrings.E0144, PlatformName, ProvisioningProfile);
					return false;
				}

				var profile = identity.Profile; // capture ref for lambda

				if (certs.Count > 0) {
					identity.SigningKey = certs.FirstOrDefault (c => profile.DeveloperCertificates.Any (p => p.Thumbprint == c.Thumbprint));
					if (identity.SigningKey is null) {
						Log.LogError (MSBStrings.E0145, PlatformName, ProvisioningProfile);
						return false;
					}
				}

				identity.AppId = ConstructValidAppId (identity.Profile, identity.BundleId);
				if (identity.AppId is null) {
					Log.LogError (MSBStrings.E0141, identity.BundleId, ProvisioningProfile);
					return false;
				}

				if (identity.SigningKey is not null) {
					codesignCommonName = SecKeychain.GetCertificateCommonName (identity.SigningKey);
					DetectedCodeSigningKey = identity.SigningKey.Thumbprint;
				}

				provisioningProfileName = identity.Profile.Name;

				DetectedProvisioningProfile = identity.Profile.Uuid;
				DetectedDistributionType = identity.Profile.DistributionType.ToString ();
				DetectedAppId = identity.AppId;

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			if ((profiles = GetProvisioningProfiles (platform, type, identity, certs)) is null)
				return false;

			if ((pairs = GetCodeSignIdentityPairs (profiles, certs)) is null)
				return false;

			identity = GetBestMatch (pairs, identity);

			if (identity.Profile is not null && identity.AppId is not null) {
				codesignCommonName = identity.SigningKey is not null ? SecKeychain.GetCertificateCommonName (identity.SigningKey) : null;
				provisioningProfileName = identity.Profile.Name;

				DetectedCodeSigningKey = identity.SigningKey?.Thumbprint;
				DetectedProvisioningProfile = identity.Profile.Uuid;
				DetectedAppId = identity.AppId;

				ReportDetectedCodesignInfo ();
			} else {
				if (identity.SigningKey is not null) {
					Log.LogError (MSBStrings.E0146, identity.BundleId, identity.SigningKey);
				} else {
					Log.LogError (MSBStrings.E0148, identity.BundleId);
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
