using System;
using System.Text;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public abstract class GenerateBundleNameTaskBase : XamarinTask {
		[Required]
		public string ProjectName { get; set; }

		[Output]
		public string BundleName { get; set; }

		static string SanitizeName (string name)
		{
			var sb = new StringBuilder (name.Length);

			foreach (var c in name) {
				if (char.IsLetterOrDigit (c) || c == '_')
					sb.Append (c);
			}

			return sb.ToString ();
		}

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (ProjectName)) {
				Log.LogError (MSBStrings.E0150);
				return false;
			}

			BundleName = SanitizeName (ProjectName);

			Log.LogMessage (MSBStrings.M0151, BundleName);

			return true;
		}
	}
}
