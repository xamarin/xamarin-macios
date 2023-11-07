using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Bundler;
using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.MacDev.Tasks {

	// We can get numerous types of native references:
	//
	//  *.dylib
	//  *.a
	//  *.framework
	//  *.xcframework
	//
	// They can come from:
	//
	//  - A NativeReference to the file/directory on disk (or even a file inside the directory).
	//  - A NativeReference to a zip of the above
	//  - A binding resource package next to an assembly
	//  - A zipped binding resource package
	//
	// Special considerations:
	// - We can only extract the files we need from any zipped reference, because this task must work on Windows (without a connection to a Mac),
	//   and a zip may contain symlinks for a different platform (and thus won't be needed). Example: an xcframework
	//   with a framework for macOS will likely have symlinks, but that shouldn't prevent the xcframework from being
	//   consumed in a build for iOS.
	public class ResolveNativeReferences : XamarinTask, ITaskCallback {
		#region Inputs

		[Required]
		public string? Architectures { get; set; }

		[Required]
		public string FrameworksDirectory { get; set; } = string.Empty;

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		public ITaskItem [] NativeReferences { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] References { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public bool SdkIsSimulator { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem []? NativeFrameworks { get; set; }

		[Output]
		public ITaskItem [] TouchedFiles { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		string GetIntermediateDecompressionDir (ITaskItem item)
		{
			return GetIntermediateDecompressionDir (item.ItemSpec);
		}

		string GetIntermediateDecompressionDir (string item)
		{
			return Path.Combine (IntermediateOutputPath, Path.GetFileName (item));
		}

		// returns the Mach-O file for the given path:
		// * for frameworks, returns foo.framework/foo
		// * for anything else, returns the input path
#if NET
		[return: NotNullIfNotNull (nameof (path))]
#else
		[return: NotNullIfNotNull ("path")]
#endif
		static string? GetActualLibrary (string? path)
		{
			if (path is null)
				return null;

			if (path.EndsWith (".framework", StringComparison.OrdinalIgnoreCase))
				return Path.Combine (path, Path.GetFileNameWithoutExtension (path));

			return path;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return ExecuteLocally ();
		}

		bool ExecuteLocally ()
		{
			var native_frameworks = new List<ITaskItem> ();
			var createdFiles = new List<string> ();

			// there can be direct native references inside a project
			foreach (var nr in NativeReferences) {
				ProcessNativeReference (nr, native_frameworks, createdFiles);
			}

			// or (managed) reference to an assembly that bind a framework
			foreach (var r in References) {
				// look for sidecar's manifest
				var resources = Path.ChangeExtension (r.ItemSpec, ".resources");
				if (Directory.Exists (resources)) {
					ProcessNativeReference (r, resources, native_frameworks, createdFiles);
				} else {
					resources = resources + ".zip";
					if (File.Exists (resources))
						ProcessNativeReference (r, resources, native_frameworks, createdFiles);
				}
			}

			NativeFrameworks = native_frameworks.ToArray ();
			TouchedFiles = createdFiles.Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;
		}

		void ProcessNativeReference (ITaskItem item, List<ITaskItem> native_frameworks, List<string> createdFiles)
		{
			ProcessNativeReference (item, item.ItemSpec, native_frameworks, createdFiles);
		}

		void ProcessNativeReference (ITaskItem item, string name, List<ITaskItem> native_frameworks, List<string> createdFiles)
		{
			// '.' can be used to represent a file (instead of the directory)
			if (Path.GetFileName (name) == ".")
				name = Path.GetDirectoryName (name);

			var parentDirectory = Path.GetDirectoryName (name);

			// framework
			if (name.EndsWith (".framework", StringComparison.OrdinalIgnoreCase)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (name);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))));
				native_frameworks.Add (nr);
				return;
			} else if (parentDirectory.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && Path.GetFileName (name) == Path.GetFileNameWithoutExtension (parentDirectory)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (name);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))));
				native_frameworks.Add (nr);
				return;
			}

			// dynamic library
			if (name.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = name;
				nr.SetMetadata ("Kind", "Dynamic");
				nr.SetMetadata ("PublishFolderType", "DynamicLibrary");
				native_frameworks.Add (nr);
				return;
			}

			// static library
			if (name.EndsWith (".a", StringComparison.OrdinalIgnoreCase)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = name;
				nr.SetMetadata ("Kind", "Static");
				nr.SetMetadata ("PublishFolderType", "StaticLibrary");
				native_frameworks.Add (nr);
				return;
			}

			// (compressed) xcframework
			if (name.EndsWith (".xcframework", StringComparison.OrdinalIgnoreCase) || name.EndsWith (".xcframework.zip", StringComparison.OrdinalIgnoreCase)) {
				if (!TryResolveXCFramework (Log, TargetFrameworkMoniker, SdkIsSimulator, Architectures, name, GetIntermediateDecompressionDir (item), createdFiles, out var frameworkPath))
					return;
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (frameworkPath);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))));
				native_frameworks.Add (nr);
				return;
			}

			// compressed framework
			if (name.EndsWith (".framework.zip", StringComparison.OrdinalIgnoreCase)) {
				if (!CompressionHelper.TryDecompress (Log, name, Path.GetFileNameWithoutExtension (name), GetIntermediateDecompressionDir (item), createdFiles, out var frameworkPath))
					return;
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (frameworkPath);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))));
				native_frameworks.Add (nr);
				return;
			}

			// sidecar / binding resource package
			if (name.EndsWith (".resources", StringComparison.OrdinalIgnoreCase)) {
				ProcessSidecar (item, name, native_frameworks, createdFiles);
				return;
			}

			// compressed sidecar / binding resource package
			if (name.EndsWith (".resources.zip", StringComparison.OrdinalIgnoreCase)) {
				ProcessSidecar (item, name, native_frameworks, createdFiles);
				return;
			}

			Log.LogWarning (MSBStrings.W7109 /* Unable to process the item '{0}' as a native reference: unknown type.* */, item.ItemSpec);
		}

		/// <summary>
		/// Finds the 'manifest' file inside a (potentially compressed) binding resource package.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="resources">Path to the binding resource package (as a zip file or a folder).</param>
		/// <param name="manifestContents">The contents of the 'manifest' file inside the binding resource package</param>
		/// <returns>True if the manifest was found.</returns>
		static bool TryGetSidecarManifest (TaskLoggingHelper log, string resources, [NotNullWhen (true)] out string? manifestContents)
		{
			using var stream = CompressionHelper.TryGetPotentiallyCompressedFile (log, resources, "manifest");

			if (stream is null) {
				manifestContents = null;
				return false;
			}

			using var streamReader = new StreamReader (stream);
			manifestContents = streamReader.ReadToEnd ();
			return true;
		}

		/// <summary>
		/// Finds the 'Info.plist' file inside a (potentially compressed) xcframework.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="resourcePath">Path to the location of the xcframework (as a zip file with an xcframework inside, or the container folder of an xcframework directory).</param>
		/// <param name="xcframework">The name of the xcframework to look for.</param>
		/// <param name="plist">The parsed Info.plist</param>
		/// <returns>True if the Info.plist was found and successfully parsed.</returns>
		static bool TryGetInfoPlist (TaskLoggingHelper log, string resourcePath, string xcframework, [NotNullWhen (true)] out PDictionary? plist)
		{
			var manifestPath = Path.Combine (xcframework, "Info.plist");
			using var stream = CompressionHelper.TryGetPotentiallyCompressedFile (log, resourcePath, manifestPath);
			if (stream is null) {
				plist = null;
				return false;
			}

			plist = (PDictionary?) PDictionary.FromStream (stream);
			if (plist is null) {
				log.LogError (MSBStrings.E7110 /* Could not load Info.plist '{0}' from the xcframework '{1}'.. */, manifestPath, resourcePath);
				return false;
			}

			return true;
		}


		void ProcessSidecar (ITaskItem r, string resources, List<ITaskItem> native_frameworks, List<string> createdFiles)
		{
			if (!TryGetSidecarManifest (Log, resources, out var manifestContents))
				return;

			var isCompressed = CompressionHelper.IsCompressed (resources);
			XmlDocument document = new XmlDocument ();
			document.LoadXmlWithoutNetworkAccess (manifestContents);
			foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {
				ITaskItem t = new TaskItem (r);
				var name = referenceNode.Attributes ["Name"].Value;
				switch (Path.GetExtension (name)) {
				case ".xcframework": {
					if (!TryResolveXCFramework (Log, TargetFrameworkMoniker, SdkIsSimulator, Architectures, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, out var frameworkPath))
						continue;
					t.ItemSpec = GetActualLibrary (frameworkPath);
					t.SetMetadata ("Kind", "Framework");
					t.SetMetadata ("PublishFolderType", "AppleFramework");
					t.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (t.ItemSpec))));
					break;
				}
				case ".framework": {
					string? frameworkPath;
					if (!isCompressed) {
						frameworkPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (Log, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, out frameworkPath)) {
						continue;
					}
					t.ItemSpec = GetActualLibrary (frameworkPath);
					t.SetMetadata ("Kind", "Framework");
					t.SetMetadata ("PublishFolderType", "AppleFramework");
					t.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (t.ItemSpec))));
					break;
				}
				case ".dylib": // macOS
					string? dylibPath;
					if (!isCompressed) {
						dylibPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (Log, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, out dylibPath)) {
						continue;
					}
					t.ItemSpec = dylibPath;
					t.SetMetadata ("Kind", "Dynamic");
					t.SetMetadata ("PublishFolderType", "DynamicLibrary");
					break;
				case ".a": // static library
					string? aPath;
					if (!isCompressed) {
						aPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (Log, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, out aPath)) {
						continue;
					}
					t.ItemSpec = aPath;
					t.SetMetadata ("Kind", "Static");
					t.SetMetadata ("PublishFolderType", "StaticLibrary");
					break;
				default:
					Log.LogWarning (MSBStrings.W7105 /* Unexpected extension '{0}' for native reference '{1}' in binding resource package '{2}'. */, Path.GetExtension (name), name, r.ItemSpec);
					t = r;
					break;
				}

				// defaults
				t.SetMetadata ("ForceLoad", "False");
				t.SetMetadata ("NeedsGccExceptionHandling", "False");
				t.SetMetadata ("IsCxx", "False");
				t.SetMetadata ("SmartLink", "True");

				// values from manifest, overriding defaults if provided
				foreach (XmlNode attribute in referenceNode.ChildNodes)
					t.SetMetadata (attribute.Name, attribute.InnerText);

				native_frameworks.Add (t);
			}
		}

		/// <summary>
		/// Resolve an xcframework into a framework for a given platform. 
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="path">Either the path to a compressed xcframework (*.xcframework.zip), or an xcframework (*.xcframework).</param>
		/// <param name="frameworkPath">A full path to the resolved framework within the xcframework. If 'resourcePath' is compressed, this will point to where the framework is decompressed on disk.</param>
		/// <param name="intermediateDecompressionDir"></param>
		/// <returns>True if a framework was succsesfully found. Otherwise false, and an error will have been printed to the log.</returns>
		public static bool TryResolveXCFramework (TaskLoggingHelper log, string targetFrameworkMoniker, bool isSimulator, string? architectures, string path, string intermediateDecompressionDir, List<string> createdFiles, [NotNullWhen (true)] out string? frameworkPath)
		{
			string resourcePath;
			string xcframework;

			if (path.EndsWith (".zip", StringComparison.OrdinalIgnoreCase)) {
				resourcePath = path;
				xcframework = Path.GetFileNameWithoutExtension (path); // Remove the .zip extension
			} else {
				resourcePath = Path.GetDirectoryName (path);
				xcframework = Path.GetFileName (path);
			}
			return TryResolveXCFramework (log, targetFrameworkMoniker, isSimulator, architectures, resourcePath, xcframework, intermediateDecompressionDir, createdFiles, out frameworkPath);
		}

		/// <summary>
		/// Resolve an xcframework into a framework for a given platform. 
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="resourcePath">Either the path to a compressed xcframework, or the containing directory of an xcframework.</param>
		/// <param name="xcframework">The name of the xcframework.</param>
		/// <param name="frameworkPath">A full path to the resolved .framework within the xcframework. If 'resourcePath' is compressed, this will point to where the framework is decompressed on disk.</param>
		/// <param name="intermediateDecompressionDir"></param>
		/// <returns>True if a framework was succsesfully found. Otherwise false, and an error will have been printed to the log.</returns>
		public static bool TryResolveXCFramework (TaskLoggingHelper log, string targetFrameworkMoniker, bool isSimulator, string? architectures, string resourcePath, string xcframework, string intermediateDecompressionDir, List<string> createdFiles, [NotNullWhen (true)] out string? frameworkPath)
		{
			frameworkPath = null;

			try {
				if (!TryGetInfoPlist (log, resourcePath, xcframework, out var plist))
					return false;

				var isCompressed = CompressionHelper.IsCompressed (resourcePath);
				var xcframeworkPath = isCompressed ? resourcePath : Path.Combine (resourcePath, xcframework);
				if (!TryResolveXCFramework (log, plist, xcframeworkPath, targetFrameworkMoniker, isSimulator, architectures!, out var frameworkRelativePath))
					return false;

				if (!isCompressed) {
					frameworkPath = Path.Combine (resourcePath, xcframework, frameworkRelativePath);
					return true;
				}

				var zipResource = Path.Combine (xcframework, Path.GetDirectoryName (frameworkRelativePath));
				if (!CompressionHelper.TryDecompress (log, resourcePath, zipResource, intermediateDecompressionDir, createdFiles, out var decompressedPath))
					return false;

				frameworkPath = Path.Combine (intermediateDecompressionDir, zipResource);

				return true;
			} catch (Exception) {
				log.LogError (MSBStrings.E0174, resourcePath);
			}

			return false;
		}

		/// <summary>
		/// Resolve an xcframework into a framework for a given platform. 
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="plist">The plist inside the xcframework.</param>
		/// <param name="xcframeworkPath">The path to the xcframework. This is only used for error messages, so it can also point to a compressed xcframework.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="frameworkPath">A relative path to the resolved framework within the xcframework.</param>
		/// <returns>True if a framework was succsesfully found. Otherwise false, and an error will have been printed to the log.</returns>
		internal static bool TryResolveXCFramework (TaskLoggingHelper log, PDictionary plist, string xcframeworkPath, string targetFrameworkMoniker, bool isSimulator, string architectures, [NotNullWhen (true)] out string? frameworkPath)
		{
			frameworkPath = null;
			var platform = PlatformFrameworkHelper.GetFramework (targetFrameworkMoniker);
			string platformName;
			switch (platform) {
			case ApplePlatform.MacCatalyst:
				platformName = "ios";
				break;
			case ApplePlatform.MacOSX:
				// PlatformFrameworkHelper.GetOperatingSystem returns "osx" which does not work for xcframework
				platformName = "macos";
				break;
			default:
				platformName = PlatformFrameworkHelper.GetOperatingSystem (targetFrameworkMoniker);
				break;
			}

			string? variant;
			if (platform == ApplePlatform.MacCatalyst) {
				variant = "maccatalyst";
			} else if (isSimulator) {
				variant = "simulator";
			} else {
				variant = null;
			}

			// plist structure https://github.com/spouliot/xcframework#infoplist
			var bundle_package_type = (PString?) plist ["CFBundlePackageType"];
			if (bundle_package_type?.Value != "XFWK") {
				log.LogError (MSBStrings.E0174 /* The xcframework {0} has an incorrect or unknown format and cannot be processed. */, xcframeworkPath);
				return false;
			}
			var available_libraries = plist.GetArray ("AvailableLibraries");
			if ((available_libraries is null) || (available_libraries.Count == 0)) {
				log.LogError (MSBStrings.E0174 /* The xcframework {0} has an incorrect or unknown format and cannot be processed. */, xcframeworkPath);
				return false;
			}

			var archs = architectures.Split (new char [] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (PDictionary item in available_libraries) {
				var supported_platform = (PString?) item ["SupportedPlatform"];
				if (!string.Equals (supported_platform?.Value, platformName, StringComparison.OrdinalIgnoreCase))
					continue;
				// optional key
				var supported_platform_variant = (PString?) item ["SupportedPlatformVariant"];
				if (supported_platform_variant?.Value != variant)
					continue;
				var supported_architectures = (PArray?) item ["SupportedArchitectures"];
				// each architecture we request must be present in the xcframework
				// but extra architectures in the xcframework are perfectly fine
				foreach (var arch in archs) {
					bool found = false;
					foreach (PString xarch in supported_architectures!) {
						found = String.Equals (arch, xarch.Value, StringComparison.OrdinalIgnoreCase);
						if (found)
							break;
					}
					if (!found) {
						log.LogError (MSBStrings.E0175 /* No matching framework found inside '{0}'. SupportedPlatform: '{0}', SupportedPlatformVariant: '{1}', SupportedArchitectures: '{2}'. */, xcframeworkPath, platformName, variant, architectures);
						return false;
					}
				}
				var library_path = (PString?) item ["LibraryPath"];
				var library_identifier = (PString?) item ["LibraryIdentifier"];
				frameworkPath = GetActualLibrary (Path.Combine (library_identifier!, library_path!));
				return true;
			}

			log.LogError (MSBStrings.E0175 /* No matching framework found inside '{0}'. SupportedPlatform: '{0}', SupportedPlatformVariant: '{1}', SupportedArchitectures: '{2}'. */, xcframeworkPath, platformName, variant, architectures);
			return false;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item)
		{
			// Don't copy any files to Windows, because
			// 1. They're not used in Inputs/Outputs, so the lack of them won't affect anything
			// 2. They may be directories, and as such we'd have to expand them to (potentially numerous and large) files to copy them (uselessly) to Windows.
			// 3. They may contain symlinks, which may not work correctly on Windows.
			return false;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			var rv = new List<ITaskItem> ();
			rv.AddRange (CreateItemsForAllFilesRecursively (NativeReferences));
			foreach (var reference in References) {
				var resourcesPackage = Path.Combine (Path.GetDirectoryName (reference.ItemSpec), Path.GetFileNameWithoutExtension (reference.ItemSpec)) + ".resources";
				if (Directory.Exists (resourcesPackage)) {
					var resources = CreateItemsForAllFilesRecursively (new string [] { resourcesPackage });
					rv.AddRange (resources);
					continue;
				}
				var zipPackage = resourcesPackage + ".zip";
				if (File.Exists (zipPackage))
					rv.Add (new TaskItem (zipPackage));
			}
			return rv;
		}
	}
}
