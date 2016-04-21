using System;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.iOS.Tasks
{
	public abstract class DetectBundleIdentifierTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		public string AppBundleName { get; set; }

		[Required]
		public string AppManifest { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string DetectedBundleId { get; set; }

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

		static string GetDefaultBundleId (string bundleName, string suggestion)
		{
			if (string.IsNullOrEmpty (suggestion))
				return "com.your-company." + bundleName;

			if (suggestion.EndsWith ("*", StringComparison.Ordinal))
				return suggestion.Substring (0, suggestion.Length - 1) + bundleName;

			return suggestion;
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
				matchLength = appid.Length;
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

		public override bool Execute ()
		{
			PDictionary plist;

			Log.LogTaskName ("DetectBundleIdentifier");
			Log.LogTaskProperty ("AppBundleName", AppBundleName);
			Log.LogTaskProperty ("AppManifest", AppManifest);
			Log.LogTaskProperty ("SdkIsSimulator", SdkIsSimulator);

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, "Error loading '{0}': {1}", AppManifest, ex.Message);
				return false;
			}

			var bundleId = plist != null ? plist.GetCFBundleIdentifier () : null;

			if (string.IsNullOrEmpty (bundleId))
				bundleId = GetDefaultBundleId (AppBundleName, null);

			DetectedBundleId = bundleId;

			return !Log.HasLoggedErrors;
		}
	}
}
