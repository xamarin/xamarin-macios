using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Mono.Options;

namespace xsiminstaller {
	class MainClass {
		static bool print_simulators;
		static int verbose;

		static bool TryExecuteAndCapture (string filename, string arguments, out string stdout)
		{
			var rv = TryExecuteAndCapture (filename, arguments, out stdout, out var stderr);
			if (!rv)
				Console.WriteLine (stderr);
			return rv;
		}

		static bool TryExecuteAndCapture (string filename, string arguments, out string stdout, out string stderr)
		{
			using (var p = new Process ()) {
				p.StartInfo.FileName = filename;
				p.StartInfo.Arguments = arguments;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.UseShellExecute = false;
				if (verbose > 0)
					Console.WriteLine ($"{filename} {arguments}");
				var output = new StringBuilder ();
				var error = new StringBuilder ();
				var outputDone = new ManualResetEvent (false);
				var errorDone = new ManualResetEvent (false);
				p.OutputDataReceived += (sender, args) =>
				{
					if (args.Data == null) {
						outputDone.Set ();
					} else {
						output.AppendLine (args.Data);
					}
				};
				p.ErrorDataReceived += (sender, args) =>
				{
					if (args.Data == null) {
						errorDone.Set ();
					} else {
						error.AppendLine (args.Data);
					}
				};
				p.Start ();
				p.BeginErrorReadLine ();
				p.BeginOutputReadLine ();
				p.WaitForExit ();
				outputDone.WaitOne ();
				errorDone.WaitOne ();
				stdout = output.ToString ();
				stderr = error.ToString ();
				if (verbose > 0 && p.ExitCode != 0)
					Console.WriteLine ("Failed to execute '{0} {1}'", filename, arguments);
				return p.ExitCode == 0;
			}
		}

		public static int Main (string [] args)
		{
			var exit_code = 0;
			string xcode_app = null;
			var install = new List<string> ();
			var only_check = false;
			var os = new OptionSet {
				{ "xcode=", "The Xcode.app to use", (v) => xcode_app = v },
				{ "install=", "ID of simulator to install. Can be repeated multiple times.", (v) => install.Add (v) },
				{ "only-check", "Only check if the simulators are installed or not. Prints the name of any missing simulators, and returns any non-installed simulators were found.", (v) => only_check = true },
				{ "print-simulators", "Print all detected simulators.", (v) => print_simulators = true },
				{ "v|verbose", "Increase verbosity", (v) => verbose++ },
				{ "q|quiet", "Decrease verbosity", (v) => verbose-- },
			};

			var others = os.Parse (args);
			if (others.Count () > 0) {
				Console.WriteLine ("Unexpected arguments:");
				foreach (var arg in others)
					Console.WriteLine ("\t{0}", arg);
				return 1;
			}

			if (string.IsNullOrEmpty (xcode_app)) {
				Console.WriteLine ("--xcode is required.");
				return 1;
			} else if (!Directory.Exists (xcode_app)) {
				Console.WriteLine ("The Xcode directory {0} does not exist.", xcode_app);
				return 1;
			}

			var plist = Path.Combine (xcode_app, "Contents", "Info.plist");
			if (!File.Exists (plist)) {
				Console.WriteLine ("The Info.plist {0} does not exist.", plist);
				return 1;
			}

			if (!TryExecuteAndCapture ("/usr/libexec/PlistBuddy", $"-c 'Print :DTXcode' '{plist}'", out var xcodeVersion))
				return 1;
			xcodeVersion = xcodeVersion.Trim ();

			if (!TryExecuteAndCapture ("/usr/libexec/PlistBuddy", $"-c 'Print :DVTPlugInCompatibilityUUID' '{plist}'", out var xcodeUuid))
				return 1;
			xcodeUuid = xcodeUuid.Trim ();

			xcodeVersion = string.Join (".", xcodeVersion.TrimStart ('0').ToCharArray ().Select ((v) => v.ToString ()));
			var url = $"https://devimages-cdn.apple.com/downloads/xcode/simulators/index-{xcodeVersion}-{xcodeUuid}.dvtdownloadableindex";
			var uri = new Uri (url);
			var tmpfile = Path.Combine (Path.GetTempPath (), Path.GetFileName (uri.LocalPath));
			if (!File.Exists (tmpfile)) {
				var wc = new WebClient ();
				wc.DownloadFile (uri, tmpfile);
			}
 			if (!TryExecuteAndCapture ("plutil", $"-convert xml1 -o - '{tmpfile}'", out var xml))
				return 1;

			var doc = new XmlDocument ();
			doc.LoadXml (xml);

			var downloadables = doc.SelectNodes ("//plist/dict/key[text()='downloadables']/following-sibling::array/dict");
			foreach (XmlNode downloadable in downloadables) {
				var nameNode = downloadable.SelectSingleNode ("key[text()='name']/following-sibling::string");
				var versionNode = downloadable.SelectSingleNode ("key[text()='version']/following-sibling::string");
				var sourceNode = downloadable.SelectSingleNode ("key[text()='source']/following-sibling::string");
				var identifierNode = downloadable.SelectSingleNode ("key[text()='identifier']/following-sibling::string");
				var fileSizeNode = downloadable.SelectSingleNode ("key[text()='fileSize']/following-sibling::integer");
				var installPrefixNode = downloadable.SelectSingleNode ("key[text()='userInfo']/following-sibling::dict/key[text()='InstallPrefix']/following-sibling::string");

				var version = versionNode.InnerText;
				var versions = version.Split ('.');
				var versionMajor = versions [0];
				var versionMinor = versions [1];
				var dict = new Dictionary<string, string> () {
					{ "DOWNLOADABLE_VERSION_MAJOR", versionMajor },
					{ "DOWNLOADABLE_VERSION_MINOR", versionMinor },
					{ "DOWNLOADABLE_VERSION", version },
				};

				var identifier = Replace (identifierNode.InnerText, dict);

				dict.Add ("DOWNLOADABLE_IDENTIFIER", identifier);

				var name = Replace (nameNode.InnerText, dict);
				var source = Replace (sourceNode.InnerText, dict);
				var installPrefix = Replace (installPrefixNode.InnerText, dict);
				var fileSize = long.Parse (fileSizeNode.InnerText);

				var installed = false;
				var updateAvailable = false;
				var installedVersion = "";

				if (only_check && !install.Contains (identifier))
					continue;

				if (TryExecuteAndCapture ($"pkgutil", $"--pkg-info {identifier}", out var pkgInfo, out _)) {
					var lines = pkgInfo.Split ('\n');
					installedVersion = lines.First ((v) => v.StartsWith ("version: ", StringComparison.Ordinal)).Substring ("version: ".Length);
					if (Version.Parse (installedVersion) >= Version.Parse (version)) {
						installed = true;
					} else {
						updateAvailable = true;
					}
				}


				var doInstall = false;
				if (install.Contains (identifier)) {
					if (installed) {
						if (!only_check && verbose >= 0)
							Console.WriteLine ($"Not installing '{identifier}' because it's already installed and up-to-date.");
					} else {
						doInstall = true;
					}
					install.Remove (identifier);
				}

				if (print_simulators) {
					Console.WriteLine (name);
					Console.Write ($"  Version: {version}");
					if (updateAvailable)
						Console.WriteLine ($" (an earlier version is installed: {installedVersion}");
					else if (!installed)
						Console.WriteLine ($" (not installed)");
					else
						Console.WriteLine ($" (installed)");
					Console.WriteLine ($"  Source: {source}");
					Console.WriteLine ($"  Identifier: {identifier}");
					Console.WriteLine ($"  InstallPrefix: {installPrefix}");
				}

				if (only_check) {
					if (doInstall) {
						if (updateAvailable) {
							Console.WriteLine (verbose > 0 ? $"The simulator '{name}' is installed, but an update is available." : name);
						} else {
							Console.WriteLine (verbose > 0 ? $"The simulator '{name}' is not installed." : name);
						}
						exit_code = 1;
					} else if (verbose > 0) {
						Console.WriteLine ($"The simulator '{name}' is installed.");
					}
				}
				if (doInstall && !only_check) {
					Console.WriteLine ($"Installing {name}...");
					Install (source, fileSize, installPrefix);
					Console.WriteLine ($"Installed {name} successfully.");
				}
			}

			if (install.Count > 0) {
				Console.WriteLine ("Unknown simulators: {0}", string.Join (", ", install));
				return 1;
			}

			return exit_code;
		}

		static bool Install (string source, long fileSize, string installPrefix)
		{
			var download_dir = "/tmp/x-provisioning";
			var filename = Path.GetFileName (source);
			var download_path = Path.Combine (download_dir, filename);
			var download = true;
			Directory.CreateDirectory (download_dir);

			if (!File.Exists (download_path)) {
				Console.WriteLine ($"Downloading {source} to {download_path}...");
			} else if (new FileInfo (download_path).Length != fileSize) {
				Console.WriteLine ($"Downloading {source} to {download_path} because the existing file's size {new FileInfo (download_path).Length} does not match the expected size {fileSize}...");
			} else {
				download = false;
			}
			if (download) {
				var downloadDone = new ManualResetEvent (false);
				var wc = new WebClient ();
				long lastProgress =  0;
				wc.DownloadProgressChanged += (sender, progress_args) => {
					var progress = progress_args.BytesReceived * 100 / fileSize;
					if (progress > lastProgress) {
						lastProgress = progress;
						Console.WriteLine ($"Downloaded {progress_args.BytesReceived}/{fileSize} bytes = {progress}%...");
					}
				};
				wc.DownloadFileCompleted += (sender, download_args) => {
					Console.WriteLine ("Download completed");
					if (download_args.Error != null) {
						Console.WriteLine ($"    with error: {download_args.Error}");
					}
					downloadDone.Set ();
				};
				wc.DownloadFileAsync (new Uri (source), download_path);
				downloadDone.WaitOne ();
			}
			var mount_point = Path.Combine (download_dir, filename + "-mount");
			Directory.CreateDirectory (mount_point);
			try {
				Console.WriteLine ($"Mounting {download_path} into {mount_point}...");
				if (!TryExecuteAndCapture ("hdiutil", $"attach '{download_path}' -mountpoint '{mount_point}' -quiet -nobrowse", out _)) {
					Console.WriteLine ("Mount failure!");
					return false;
				}

				try {
					var packages = Directory.GetFiles (mount_point, "*.pkg");
					if (packages.Length == 0) {
						Console.WriteLine ("Found no *.pkg files in the dmg.");
					} else if (packages.Length > 1) {
						Console.WriteLine ("Found more than one *.pkg file in the dmg:\n\t", string.Join ("\n\t", packages));
					} else {
						var expanded_path = Path.Combine (download_dir + "-expanded-pkg");
						if (Directory.Exists (expanded_path))
							Directory.Delete (expanded_path, true);
						if (!TryExecuteAndCapture ("pkgutil", $"--expand '{packages [0]}' '{expanded_path}'", out _)) {
							Console.WriteLine ($"Failed to expand {packages [0]}");
							return false;
						}
						try {
							var packageInfoPath = Path.Combine (expanded_path, "PackageInfo");
							var packageInfo = File.ReadAllText (packageInfoPath);
							packageInfo = packageInfo.Replace ("<pkg-info auth=\"root\" identifier=\"com.apple.pkg", "<pkg-info install-location=\"" + installPrefix + "\" auth=\"root\" identifier=\"com.apple.pkg");
							File.WriteAllText (packageInfoPath, packageInfo);

							var fixed_path = Path.Combine (Path.GetDirectoryName (download_path), Path.GetFileNameWithoutExtension (download_path) + "-fixed.pkg");
							if (File.Exists (fixed_path))
								File.Delete (fixed_path);
							try {
								if (!TryExecuteAndCapture ("pkgutil", $"--flatten '{expanded_path}' '{fixed_path}'", out _)) {
									Console.WriteLine ("Failed to create fixed package.");
									return false;
								}

								if (!TryExecuteAndCapture ("sudo", $"installer -pkg '{fixed_path}' -target / -verbose -dumplog", out _)) {
									Console.WriteLine ("Failed to install package.");
									return false;
								}
							} finally {
								if (File.Exists (fixed_path))
									File.Delete (fixed_path);
							}
						} finally {
							Directory.Delete (expanded_path, true);
						}
					}
				} finally {
					TryExecuteAndCapture ("hdiutil", $"detach '{mount_point}' -quiet", out _);
				}
			} finally {
				Directory.Delete (mount_point, true);
			}

			return true;
		}

		static string Replace (string value, Dictionary<string, string> replacements)
		{
			foreach (var kvp in replacements)
				value = value.Replace ($"$({kvp.Key})", kvp.Value);
			return value;
		}
	}
}
