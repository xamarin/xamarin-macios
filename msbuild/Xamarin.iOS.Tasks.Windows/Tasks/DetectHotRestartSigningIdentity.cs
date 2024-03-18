using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.iOS.Windows;
using Xamarin.MacDev;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.iOS.HotRestart.Tasks {
	public class DetectHotRestartSigningIdentity : Task {
		static readonly string ProvisioningPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "Xamarin", "iOS", "Provisioning");
		static readonly string ProfilesPath = Path.Combine (ProvisioningPath, "Profiles");
		static readonly string CertificatesPath = Path.Combine (ProvisioningPath, "Certificates");

		const string AutomaticProvision = "Automatic";
		const string AutomaticAdHocProvision = "Automatic:AdHoc";
		const string AutomaticAppStoreProvision = "Automatic:AppStore";
		const string AutomaticInHouseProvision = "Automatic:InHouse";

		protected string PlatformName => "iOS";
		protected string ApplicationIdentifierKey => "application-identifier";
		protected string [] DevelopmentPrefixes => IPhoneCertificate.DevelopmentPrefixes;
		protected string [] AppStoreDistributionPrefixes => IPhoneCertificate.DistributionPrefixes;

		string provisioningProfileName;
		string codesignCommonName;
		IHotRestartClient hotRestartClient;

		#region Inputs

		public string BundleIdentifier { get; set; }

		public string SigningKey { get; set; }

		public string ProvisioningProfile { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DetectedCodeSigningPath { get; set; }

		[Output]
		public string DetectedProvisioningProfilePath { get; set; }

		[Output]
		public string DetectedProvisioningProfileId { get; set; }

		[Output]
		public string DetectedAppId { get; set; }

		[Output]
		public string DetectedBundleId { get; set; }

		#endregion

		public override bool Execute ()
		{
			IList<MobileProvision> profiles;
			IList<X509Certificate2> certs;
			List<CodeSignIdentity> pairs;

			var type = GetProvisioningDistributionType ();
			var platform = MobileProvisionPlatform.iOS;
			var identity = new CodeSignIdentity ();

			hotRestartClient = new HotRestartClient ();

			identity.BundleId = BundleIdentifier;
			if (string.IsNullOrEmpty (identity.BundleId))
				Log.LogError ("Info.plist does not define CFBundleIdentifier");

			DetectedBundleId = identity.BundleId;

			if (!TryGetSigningCertificates (out certs, false))
				return false;

			Log.LogMessage (MessageImportance.Low, "Available certificates:");

			foreach (var cert in certs)
				Log.LogMessage (MessageImportance.Low, "    {0}", GetCertificateCommonName (cert));

			if (!IsAutoCodeSignProfile (ProvisioningProfile)) {
				identity.Profile = MobileProvisionIndex.GetMobileProvision (platform, ProvisioningProfile);

				if (identity.Profile is null) {
					Log.LogError ("The specified " + PlatformName + " provisioning profile '{0}' could not be found. Please enable Automatic Provisioning from the iOS Bundle Signing page.", ProvisioningProfile);
					return false;
				}

				var profile = identity.Profile; // capture ref for lambda

				if (certs.Count > 0) {
					identity.SigningKey = certs.FirstOrDefault (c => profile.DeveloperCertificates.Any (p => p.Thumbprint == c.Thumbprint));
					if (identity.SigningKey is null) {
						Log.LogError ("No " + PlatformName + " signing identities match the specified provisioning profile '{0}'.", ProvisioningProfile);
						return false;
					}
				}

				identity.AppId = ConstructValidAppId (identity.Profile, identity.BundleId);

				if (identity.AppId is null) {
					Log.LogError ("Project bundle identifier '{0}' does not match specified provisioning profile '{1}'. Please enable Automatic Provisioning from the iOS Bundle Signing page.", identity.BundleId, ProvisioningProfile);
					return false;
				}

				if (identity.SigningKey is not null) {
					codesignCommonName = GetCertificateCommonName (identity.SigningKey);
					DetectedCodeSigningPath = Path.Combine (CertificatesPath, $"{identity.SigningKey.SerialNumber}.p12");
				}

				provisioningProfileName = identity.Profile.Name;

				DetectedAppId = identity.AppId;
				DetectedProvisioningProfileId = identity.Profile.Uuid;
				DetectedProvisioningProfilePath = Path.Combine (ProfilesPath, $"{DetectedProvisioningProfileId}.mobileprovision");

				ReportDetectedCodesignInfo ();

				return !Log.HasLoggedErrors;
			}

			if ((profiles = GetProvisioningProfiles (platform, type, identity, certs)) is null)
				return false;

			if ((pairs = GetCodeSignIdentityPairs (profiles, certs)) is null)
				return false;

			identity = GetBestMatch (pairs, identity);

			if (identity.Profile is not null && identity.AppId is not null) {
				codesignCommonName = identity.SigningKey is not null ? GetCertificateCommonName (identity.SigningKey) : null;
				provisioningProfileName = identity.Profile.Name;

				DetectedAppId = identity.AppId;
				DetectedCodeSigningPath = identity.SigningKey is not null ? Path.Combine (CertificatesPath, $"{identity.SigningKey.SerialNumber}.p12") : string.Empty;
				DetectedProvisioningProfileId = identity.Profile.Uuid;
				DetectedProvisioningProfilePath = Path.Combine (ProfilesPath, $"{DetectedProvisioningProfileId}.mobileprovision");

				ReportDetectedCodesignInfo ();
			} else {
				if (identity.SigningKey is not null) {
					Log.LogError ("Bundle identifier '{0}' does not match any installed provisioning profile for selected signing identity '{0}'. Please enable Automatic Provisioning from the iOS Bundle Signing page.", identity.BundleId, identity.SigningKey);
				} else {
					Log.LogError ("Bundle identifier '{0}' does not match any installed provisioning profile. Please enable Automatic Provisioning from the iOS Bundle Signing page.", identity.BundleId);
				}
			}

			return !Log.HasLoggedErrors;
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
			Log.LogMessage (MessageImportance.High, "Detected signing identity:");

			if (codesignCommonName is not null)
				Log.LogMessage (MessageImportance.High, "  Code Signing Key: \"{0}\" ({1})", codesignCommonName, DetectedCodeSigningPath);
			if (provisioningProfileName is not null)
				Log.LogMessage (MessageImportance.High, "  Provisioning Profile: \"{0}\" ({1})", provisioningProfileName, DetectedProvisioningProfilePath);
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

		bool TryGetSigningCertificates (out IList<X509Certificate2> certs, string [] prefixes, bool allowZeroCerts)
		{
			var now = DateTime.Now;

			certs = new List<X509Certificate2> ();

			foreach (var certificate in GetAllCertificates ()) {
				var cname = GetCertificateCommonName (certificate);

				if (!StartsWithAny (cname, prefixes)) {
					Log.LogMessage (MessageImportance.Low, "The certificate '{0}' does not match any of the prefixes '{1}'.", cname, string.Join ("', '", prefixes));
					continue;
				}

				if (now >= certificate.NotAfter) {
					Log.LogMessage (MessageImportance.Low, "The certificate '{0}' has expired ({1})", cname, certificate.NotAfter);
					continue;
				}

				certs.Add (certificate);
			}

			if (certs.Count == 0 && !allowZeroCerts) {
				var message = "No valid " + PlatformName + " code signing keys found in keychain. Please enable Automatic Provisioning from the iOS Bundle Signing page.";

				Log.LogError (message);

				return false;
			}

			return true;
		}

		bool TryGetSigningCertificates (out IList<X509Certificate2> certs, string name)
		{
			var now = DateTime.Now;

			certs = new List<X509Certificate2> ();

			foreach (var certificate in GetAllCertificates ()) {
				var cname = GetCertificateCommonName (certificate);

				if (!name.Equals (certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) && name != cname) {
					Log.LogMessage (MessageImportance.Low, "The certificate '{0}' does not match '{1}'.", cname, name);
					continue;
				}

				if (now >= certificate.NotAfter) {
					Log.LogMessage (MessageImportance.Low, "The certificate '{0}' has expired ({1})", cname, certificate.NotAfter);
					continue;
				}

				certs.Add (certificate);
			}

			if (certs.Count == 0) {
				Log.LogError (PlatformName + " code signing key '{0}' not found in keychain.", SigningKey);
				return false;
			}

			return true;
		}

		bool TryGetSigningCertificates (out IList<X509Certificate2> certs, bool allowZeroCerts)
		{
			try {
				if (string.IsNullOrEmpty (SigningKey) || MatchesAny (SigningKey, DevelopmentPrefixes)) {
					// Note: we treat an empty signing key as "developer automatic".
					if (!TryGetSigningCertificates (out certs, DevelopmentPrefixes, allowZeroCerts))
						return false;
				} else if (MatchesAny (SigningKey, AppStoreDistributionPrefixes)) {
					if (!TryGetSigningCertificates (out certs, AppStoreDistributionPrefixes, false))
						return false;
				} else {
					// The user has specified an exact name to match...
					if (!TryGetSigningCertificates (out certs, SigningKey))
						return false;
				}

				return true;
			} catch (Exception ex) {
				Log.LogError ("{0}", ex.Message);
				certs = null;
				return false;
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

				Log.LogError ($"Could not find any available provisioning profiles for {PlatformName}. Please enable Automatic Provisioning from the iOS Bundle Signing page.");

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
								 Log.LogMessage (MessageImportance.Low, "'{0}' doesn't match '{1}'.", d.Thumbprint, c.Thumbprint);
							 return rv;
						 })
						 select new CodeSignIdentity { SigningKey = c, Profile = p }).ToList ();

				if (pairs.Count == 0) {
					Log.LogError ("No installed provisioning profiles match the installed " + PlatformName + " signing identities. Please enable Automatic Provisioning from the iOS Bundle Signing page.");
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
			Log.LogMessage (MessageImportance.Low, "Finding matching provisioning profiles with compatible AppID, keeping only those with the longest matching (wildcard) IDs.");

			foreach (var pair in pairs) {
				var appid = ConstructValidAppId (pair.Profile, identity.BundleId, out matchLength);

				if (appid is not null) {
					if (matchLength >= bestMatchLength) {
						if (matchLength > bestMatchLength) {
							bestMatchLength = matchLength;
							foreach (var previousMatch in matches)
								Log.LogMessage (MessageImportance.Low, "AppID: {0} was ruled out because we found a better match: {1}.", previousMatch.AppId, appid);
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

						Log.LogMessage (MessageImportance.Low, "AppID: {0} was ruled out because we already found better matches: {1}.", appid, currentMatches);
					}
				}
			}

			if (matches.Count == 0) {
				Log.LogWarning ("No installed provisioning profiles match the bundle identifier {0}", identity.BundleId);

				return identity;
			}

			if (matches.Count > 1) {
				var spaces = new string (' ', 3);

				Log.LogMessage (MessageImportance.Normal, "Multiple provisioning profiles match the bundle identifier; using the first match.");

				matches.Sort (new SigningIdentityComparer ());

				for (int i = 0; i < matches.Count; i++) {
					Log.LogMessage (MessageImportance.Normal, "{0,3}. Provisioning Profile: \"{1}\" ({2})", i + 1, matches [i].Profile.Name, matches [i].Profile.Uuid);

					if (matches [i].SigningKey is not null)
						Log.LogMessage (MessageImportance.Normal, "{0}  Signing Identity: \"{1}\"", spaces, GetCertificateCommonName (matches [i].SigningKey));
				}
			}

			return matches [0];
		}

		MobileProvisionDistributionType GetProvisioningDistributionType ()
		{
			if (ProvisioningProfile == AutomaticAppStoreProvision)
				return MobileProvisionDistributionType.AppStore;
			else if (ProvisioningProfile == AutomaticInHouseProvision)
				return MobileProvisionDistributionType.InHouse;
			else if (ProvisioningProfile == AutomaticAdHocProvision)
				return MobileProvisionDistributionType.AdHoc;

			return MobileProvisionDistributionType.Any;
		}

		IEnumerable<X509Certificate2> GetAllCertificates () => hotRestartClient.CertificatesManager.GetInstalledCertificates ();

		string GetCertificateCommonName (X509Certificate2 certificate) => hotRestartClient.CertificatesManager.GetCertificateCommonName (certificate.SerialNumber);

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

		class SigningIdentityComparer : IComparer<CodeSignIdentity> {
			public int Compare (CodeSignIdentity x, CodeSignIdentity y)
			{
				// reverse sort by provisioning profile creation date
				return y.Profile.CreationDate.CompareTo (x.Profile.CreationDate);
			}
		}
	}
}
