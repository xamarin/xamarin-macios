using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public class CreateXcArchive : XcodeBuildTask {

		// Task input parameters
		public string ProjectPath { get; set; } = string.Empty;

		public string SchemeName { get; set; } = string.Empty;

		public string Configuration { get; set; } = string.Empty;

		public string ArchivePlatform { get; set; } = string.Empty;

		public string DerivedDataPath { get; set; } = string.Empty;

		public string PackageCachePath { get; set; } = string.Empty;


		readonly string [] archive_args = new string [] {
			"BUILD_LIBRARY_FOR_DISTRIBUTION=YES", "ENABLE_BITCODE=NO", "OBJC_CFLAGS=\"-fno-objc-msgsend-selector-stubs -ObjC\"",
			"OTHER_LDFLAGS=\"-ObjC\"", "OTHER_SWIFT_FLAGS=\"-no-verify-emitted-module-interface\"",
			"SKIP_INSTALL=NO", "SWIFT_INSTALL_OBJC_HEADER=YES",
		};

		protected override string Command { get; set; } = "archive";

		protected override IList<string> GenerateCommandLineCommands ()
		{
			var args = new List<string> ();

			if (!string.IsNullOrEmpty (ProjectPath)) {
				args.Add ("-project");
				args.Add (ProjectPath);
			}

			if (!string.IsNullOrEmpty (SchemeName)) {
				args.Add ("-scheme");
				args.Add (SchemeName);
			}

			if (!string.IsNullOrEmpty (Configuration)) {
				args.Add ("-configuration");
				args.Add (Configuration);
			}

			if (!string.IsNullOrEmpty (ArchivePlatform)) {
				args.Add ("-destination");
				args.Add (ArchivePlatform);
			}

			if (!string.IsNullOrEmpty (OutputPath)) {
				args.Add ("-archivePath");
				args.Add (OutputPath);
			}

			if (!string.IsNullOrEmpty (DerivedDataPath)) {
				args.Add ("-derivedDataPath");
				args.Add (DerivedDataPath);
			}

			if (!string.IsNullOrEmpty (PackageCachePath)) {
				args.Add ("-packageCachePath");
				args.Add (PackageCachePath);
			}

			args.AddRange (archive_args);

			return args;
		}

		public override bool Execute ()
		{
			if (!Directory.Exists (ProjectPath)) {
				Log.LogError (MSBStrings.XcodeBuild_InvalidItem, ProjectPath);
				return false;
			}

			return base.Execute ();
		}

	}
}
