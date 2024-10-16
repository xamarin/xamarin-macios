using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CreateInstallerPackage : XamarinTask, ICancelableTask {
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs
		[Required]
		public string OutputDirectory { get; set; } = string.Empty;

		[Required]
		public string Name { get; set; } = string.Empty;

		[Required] // Used to get version
		public string AppManifest { get; set; } = string.Empty;

		[Required] // Used for variable substitution in AppendExtraArgs
		public string ProjectPath { get; set; } = string.Empty;

		[Required]  // Used for variable substitution in AppendExtraArgs
		public string AppBundleDir { get; set; } = string.Empty;

		[Required]   // Used for variable substitution in AppendExtraArgs
		public ITaskItem? MainAssembly { get; set; }

		[Required] // Should we even look at the PackageSigningKey?
				   // It has a default value when this is false, so we can't just switch off it being null
		public bool EnablePackageSigning { get; set; }

		public string ProductDefinition { get; set; } = string.Empty;

		public string PackageSigningKey { get; set; } = string.Empty;

		public string PackagingExtraArgs { get; set; } = string.Empty;

		// both input and output
		[Output]
		public string PkgPackagePath { get; set; } = string.Empty;

		public string ProductBuildPath { get; set; } = string.Empty;
		#endregion

		static string GetExecutable (List<string> arguments, string toolName, string toolPathOverride)
		{
			if (string.IsNullOrEmpty (toolPathOverride)) {
				arguments.Insert (0, toolName);
				return "xcrun";
			}
			return toolPathOverride;
		}

		string? GetProjectVersion ()
		{
			PDictionary plist;

			try {
				plist = PDictionary.FromFile (AppManifest)!;
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

		public override bool Execute ()
		{
			var args = GenerateCommandLineCommands ();
			var executable = GetExecutable (args, "productbuild", ProductBuildPath);
			cancellationTokenSource = new CancellationTokenSource ();
			ExecuteAsync (Log, executable, args, workingDirectory: OutputDirectory, cancellationToken: cancellationTokenSource.Token).Wait ();
			return !Log.HasLoggedErrors;
		}

		List<string> GenerateCommandLineCommands ()
		{
			Log.LogMessage ("Creating installer package");

			var args = new List<string> ();

			if (!string.IsNullOrEmpty (ProductDefinition)) {
				args.Add ("--product");
				args.Add (Path.GetFullPath (ProductDefinition));
			}

			args.Add ("--component");
			args.Add (Path.GetFullPath (AppBundleDir));
			args.Add ("/Applications");

			if (EnablePackageSigning) {
				args.Add ("--sign");
				args.Add (GetPackageSigningCertificateCommonName ());
			}

			if (!string.IsNullOrEmpty (PackagingExtraArgs)) {
				try {
					AppendExtraArgs (args, PackagingExtraArgs);
				} catch (FormatException) {
					Log.LogError (MSBStrings.E0123);
					return args;
				}
			}

			if (string.IsNullOrEmpty (PkgPackagePath)) {
				var projectVersion = GetProjectVersion ();
				string target = string.Format ("{0}{1}.pkg", Name, String.IsNullOrEmpty (projectVersion) ? "" : "-" + projectVersion);
				PkgPackagePath = Path.Combine (OutputDirectory, target);
			}
			PkgPackagePath = Path.GetFullPath (PkgPackagePath);
			args.Add (PkgPackagePath);

			Directory.CreateDirectory (Path.GetDirectoryName (PkgPackagePath));

			return args;
		}

		void AppendExtraArgs (List<string> args, string extraArgs)
		{
			var target = this.MainAssembly!.ItemSpec;

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
				args.Add (StringParserService.Parse (argv [i], customTags));
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

			X509Certificate2? best = null;
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

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
		}
	}
}
