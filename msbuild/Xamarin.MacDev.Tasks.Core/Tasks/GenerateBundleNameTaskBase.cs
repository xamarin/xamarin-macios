using System;
using System.Text;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GenerateBundleNameTaskBase : Task
	{
		public string SessionId { get; set; }

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
			Log.LogTaskName ("GenerateBundleName");
			Log.LogTaskProperty ("ProjectName", ProjectName);

			if (string.IsNullOrEmpty (ProjectName)) {
				Log.LogError ("  AssemblyPath cannot be null or empty");
				return false;
			}

			BundleName = SanitizeName (ProjectName);

			Log.LogMessage ("Generated bundle name: {0}", BundleName);

			return true;
		}
	}
}
