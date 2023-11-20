using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.Build.Framework;

using Microsoft.Build.Utilities;
using System.Security.Cryptography.X509Certificates;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CreateInstallerPackageTaskBase : XamarinToolTask {
		#region Inputs
		[Required]
		public string OutputDirectory { get; set; }

		[Required]
		public string Name { get; set; }

		[Required] // Used to get version
		public string AppManifest { get; set; }

		[Required] // Used for variable substitution in AppendExtraArgs
		public string ProjectPath { get; set; }

		[Required]  // Used for variable substitution in AppendExtraArgs
		public string AppBundleDir { get; set; }

		[Required]   // Used for variable substitution in AppendExtraArgs
		public ITaskItem MainAssembly { get; set; }

		[Required] // Should we even look at the PackageSigningKey?
				   // It has a default value when this is false, so we can't just switch off it being null
		public bool EnablePackageSigning { get; set; }

		public string ProductDefinition { get; set; }

		public string PackageSigningKey { get; set; }

		public string PackagingExtraArgs { get; set; }

		// both input and output
		[Output]
		public string PkgPackagePath { get; set; }
		#endregion

		string GetProjectVersion ()
		{
			PDictionary plist;

			try {
				plist = PDictionary.FromFile (AppManifest);
			} catch (Exception ex) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E0010, AppManifest, ex.Message);
				return null;
			}

			if (plist is null) {
				Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, MSBStrings.E0122, AppManifest);
				return null;
			}

			return plist.GetCFBundleShortVersionString ();
		}

		protected override string ToolName {
			get { return "productbuild"; }
		}

		protected override string GenerateFullPathToTool ()
		{
			return @"/usr/bin/productbuild";
		}

		protected override string GetWorkingDirectory ()
		{
			return OutputDirectory;
		}

		protected override string GenerateCommandLineCommands ()
		{
			Log.LogMessage ("Creating installer package");

			var args = new CommandLineArgumentBuilder ();

			if (!string.IsNullOrEmpty (ProductDefinition)) {
				args.Add ("--product");
				args.AddQuoted (Path.GetFullPath (ProductDefinition));
			}

			args.Add ("--component");
			args.AddQuoted (Path.GetFullPath (AppBundleDir));
			args.Add ("/Applications");

			if (EnablePackageSigning) {
				args.Add ("--sign");
				args.AddQuoted (GetPackageSigningCertificateCommonName ());
			}

			if (!string.IsNullOrEmpty (PackagingExtraArgs)) {
				try {
					AppendExtraArgs (args, PackagingExtraArgs);
				} catch (FormatException) {
					Log.LogError (MSBStrings.E0123);
					return string.Empty;
				}
			}

			if (string.IsNullOrEmpty (PkgPackagePath)) {
				string projectVersion = GetProjectVersion ();
				string target = string.Format ("{0}{1}.pkg", Name, String.IsNullOrEmpty (projectVersion) ? "" : "-" + projectVersion);
				PkgPackagePath = Path.Combine (OutputDirectory, target);
			}
			PkgPackagePath = Path.GetFullPath (PkgPackagePath);
			args.AddQuoted (PkgPackagePath);

			Directory.CreateDirectory (Path.GetDirectoryName (PkgPackagePath));

			return args.ToString ();
		}

		void AppendExtraArgs (CommandLineArgumentBuilder args, string extraArgs)
		{
			var target = this.MainAssembly.ItemSpec;

			string [] argv = CommandLineArgumentBuilder.Parse (extraArgs);
			var customTags = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase) {
				{ "projectdir",   Path.GetDirectoryName (this.ProjectPath) },
			// Apparently msbuild doesn't propagate the solution path, so we can't get it. - MTouchTaskBase.cs
			// 	{ "solutiondir",  proj.ParentSolution is not null ? proj.ParentSolution.BaseDirectory : proj.BaseDirectory },
				{ "appbundledir", this.AppBundleDir },
				{ "targetpath",   Path.Combine (Path.GetDirectoryName (target), Path.GetFileName (target)) },
				{ "targetdir",    Path.GetDirectoryName (target) },
				{ "targetname",   Path.GetFileName (target) },
				{ "targetext",    Path.GetExtension (target) },
			};

			for (int i = 0; i < argv.Length; i++)
				args.AddQuoted (StringParserService.Parse (argv [i], customTags));
		}

		string GetPackageSigningCertificateCommonName ()
		{
			var certificates = Keychain.Default.GetAllSigningCertificates ();
			DateTime now = DateTime.Now;
			string key;

			if (string.IsNullOrEmpty (PackageSigningKey))
				key = "3rd Party Mac Developer Installer";
			else
				key = PackageSigningKey;

			X509Certificate2 best = null;
			foreach (var cert in certificates) {
				if (now < cert.NotBefore || now >= cert.NotAfter)
					continue;

				string name = Keychain.GetCertificateCommonName (cert);
				bool matches;

				if (key == "3rd Party Mac Developer Installer" || key == "Developer ID Installer") {
					matches = name.StartsWith (key, StringComparison.Ordinal);
				} else {
					matches = name == key || cert.Thumbprint == key;
				}

				if (matches && (best is null || cert.NotAfter > best.NotAfter))
					best = cert;
			}

			if (best is null) {
				string msg = string.Format (MSBStrings.E0124, key);
				Log.LogError (msg);
				return string.Empty;
			}

			return Keychain.GetCertificateCommonName (best);
		}
	}
}
