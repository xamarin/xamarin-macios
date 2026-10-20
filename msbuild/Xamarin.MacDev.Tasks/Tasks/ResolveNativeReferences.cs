using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
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
		CancellationTokenSource? cancellationTokenSource;

		#region Inputs

		[Required]
		public string? Architectures { get; set; }

		[Required]
		public string FrameworksDirectory { get; set; } = string.Empty;

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		public ITaskItem [] NativeReferences { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem []? NativeReferencesToValidate { get; set; }

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

		static void WarnIfIncorrectFrameworkExtension (TaskLoggingHelper log, string path)
		{
			var parent = Path.GetDirectoryName (path);
			if (parent is not null && parent.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && Path.GetFileName (path) == Path.GetFileNameWithoutExtension (parent)) {
				path = parent;
			} else if (path.EndsWith (".framework.zip", StringComparison.OrdinalIgnoreCase)) {
				path = Path.ChangeExtension (path, null);
			}

			if (path.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && !path.EndsWith (".framework", StringComparison.Ordinal))
				log.LogWarning (7198, path, MSBStrings.W7198 /* The framework '{0}' has an incorrectly capitalized extension. Rename it to use the lowercase '.framework' extension to ensure it is linked and bundled correctly. */, path);
		}

		public override bool Execute ()
		{
			if (NativeReferences.Length == 0 && References.Length == 0 && (NativeReferencesToValidate is null || NativeReferencesToValidate.Length == 0))
				return true;

			if (ShouldExecuteRemotely ())
				return ExecuteRemotely ();

			return ExecuteLocally ();
		}

		bool ExecuteLocally ()
		{
			var native_frameworks = new List<ITaskItem> ();
			var createdFiles = new List<string> ();

			cancellationTokenSource = new CancellationTokenSource ();

			if (NativeReferencesToValidate is not null) {
				foreach (var nr in NativeReferencesToValidate)
					WarnIfIncorrectFrameworkExtension (Log, nr.ItemSpec);
			}

			// there can be direct native references inside a project
			foreach (var nr in NativeReferences) {
				ProcessNativeReference (nr, native_frameworks, createdFiles, cancellationTokenSource.Token);
			}

			// or (managed) reference to an assembly that bind a framework
			foreach (var r in References) {
				// look for sidecar's manifest
				var resources = Path.ChangeExtension (r.ItemSpec, ".resources");
				if (Directory.Exists (resources)) {
					ProcessNativeReference (r, resources, native_frameworks, createdFiles, cancellationTokenSource.Token);
				} else {
					resources = resources + ".zip";
					if (File.Exists (resources))
						ProcessNativeReference (r, resources, native_frameworks, createdFiles, cancellationTokenSource.Token);
				}
			}

			NativeFrameworks = native_frameworks.ToArray ();
			TouchedFiles = createdFiles.Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;
		}

		void ProcessNativeReference (ITaskItem item, List<ITaskItem> native_frameworks, List<string> createdFiles, CancellationToken? cancellationTokenSource)
		{
			ProcessNativeReference (item, item.ItemSpec, native_frameworks, createdFiles, cancellationTokenSource);
		}

		void ProcessNativeReference (ITaskItem item, string name, List<ITaskItem> native_frameworks, List<string> createdFiles, CancellationToken? cancellationToken)
		{
			// '.' can be used to represent a file (instead of the directory)
			if (Path.GetFileName (name) == ".")
				name = Path.GetDirectoryName (name)!;

			WarnIfIncorrectFrameworkExtension (Log, name);

			var parentDirectory = Path.GetDirectoryName (name)!;

			// framework
			if (name.EndsWith (".framework", StringComparison.OrdinalIgnoreCase)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (name);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))!));
				native_frameworks.Add (nr);
				return;
			} else if (parentDirectory.EndsWith (".framework", StringComparison.OrdinalIgnoreCase) && Path.GetFileName (name) == Path.GetFileNameWithoutExtension (parentDirectory)) {
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (name);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec))!));
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
				if (!TryResolveXCFramework (this, TargetFrameworkMoniker, SdkIsSimulator, Architectures, name, GetIntermediateDecompressionDir (item), createdFiles, cancellationToken, out var nativeLibraryPath))
					return;
				var nr = new TaskItem (item);
				SetMetadataNativeLibrary (nr, nativeLibraryPath);
				native_frameworks.Add (nr);
				return;
			}

			// compressed framework
			if (name.EndsWith (".framework.zip", StringComparison.OrdinalIgnoreCase)) {
				if (!CompressionHelper.TryDecompress (this, name, Path.GetFileNameWithoutExtension (name), GetIntermediateDecompressionDir (item), createdFiles, cancellationToken, out var frameworkPath))
					return;
				var nr = new TaskItem (item);
				nr.ItemSpec = GetActualLibrary (frameworkPath);
				nr.SetMetadata ("Kind", "Framework");
				nr.SetMetadata ("PublishFolderType", "AppleFramework");
				nr.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (nr.ItemSpec)!)));
				native_frameworks.Add (nr);
				return;
			}

			// sidecar / binding resource package
			if (name.EndsWith (".resources", StringComparison.OrdinalIgnoreCase)) {
				ProcessSidecar (item, name, native_frameworks, createdFiles, cancellationToken);
				return;
			}

			// compressed sidecar / binding resource package
			if (name.EndsWith (".resources.zip", StringComparison.OrdinalIgnoreCase)) {
				ProcessSidecar (item, name, native_frameworks, createdFiles, cancellationToken);
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
			var isCompressedXcframework = CompressionHelper.IsCompressed (xcframework);
			var potentiallyCompressedFile = isCompressedXcframework ? Path.Combine (resourcePath, xcframework) : resourcePath;
			var manifestPath = Path.Combine (isCompressedXcframework ? Path.GetFileNameWithoutExtension (xcframework) : xcframework, "Info.plist");
			using var stream = CompressionHelper.TryGetPotentiallyCompressedFile (log, potentiallyCompressedFile, manifestPath);
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

		void SetMetadataNativeLibrary (ITaskItem item, string nativeLibraryPath)
		{
			item.ItemSpec = GetActualLibrary (nativeLibraryPath);
			if (item.ItemSpec.EndsWith (".a", StringComparison.OrdinalIgnoreCase)) {
				item.SetMetadata ("Kind", "Static");
				item.SetMetadata ("PublishFolderType", "StaticLibrary");
			} else if (item.ItemSpec.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase)) {
				item.SetMetadata ("Kind", "Dynamic");
				item.SetMetadata ("PublishFolderType", "DynamicLibrary");
			} else {
				item.SetMetadata ("Kind", "Framework");
				item.SetMetadata ("PublishFolderType", "AppleFramework");
			}
			item.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (item.ItemSpec)!)));
		}

		// Metadata we must NOT copy from a binding resource package's 'manifest' file. A binding resource
		// package is passive data that may come from a restored package, so its manifest must only
		// describe native-reference content - it must not be able to inject the build-controlled
		// path/layout/identity metadata below, which is used to compute where a native library is laid
		// out, reidentified or published (and could otherwise redirect output outside the intended output
		// directory). Everything else - including binding-defined metadata such as 'NoDSymUtil' or
		// 'NoSymbolStrip' - is legitimate native-reference content and is copied as-is.
		static readonly HashSet<string> blockedManifestMetadata = new HashSet<string> (StringComparer.OrdinalIgnoreCase) {
			"RelativePath",
			"ReidentifiedPath",
			"ComputedRelativePath",
			"DynamicLibraryId",
			"PublishFolderType",
			"TargetDirectory",
			"SourceDirectory",
		};

		// Returns true if 'path' is not a safe relative subpath: i.e. it's rooted/absolute, drive-qualified,
		// or contains a '..' traversal segment. Used to validate untrusted path-shaped values that come
		// from a binding resource package manifest before they're combined with a directory.
		static bool IsUnsafeRelativePath (string path)
		{
			if (string.IsNullOrEmpty (path))
				return false;
			// Drive-qualified (e.g. "C:..." on Windows) or alternate-data-stream separators.
			if (path.IndexOf (':') >= 0)
				return true;
			if (Path.IsPathRooted (path))
				return true;
			foreach (var segment in path.Split ('/', '\\')) {
				if (segment == "..")
					return true;
			}
			return false;
		}

		void ProcessSidecar (ITaskItem r, string resources, List<ITaskItem> native_frameworks, List<string> createdFiles, CancellationToken? cancellationToken)
		{
			if (!TryGetSidecarManifest (Log, resources, out var manifestContents))
				return;

			var isCompressed = CompressionHelper.IsCompressed (resources);
			var document = new XmlDocument ();
			document.LoadXmlWithoutNetworkAccess (manifestContents);
			foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {
				ITaskItem t = new TaskItem (r);
				var name = referenceNode.Attributes? ["Name"]?.Value.Trim ('\\', '/') ?? string.Empty;
				// A native reference without a (usable) name can't be processed; skip it instead of falling
				// through and accidentally treating the managed reference itself as a native reference.
				if (string.IsNullOrEmpty (name)) {
					Log.LogWarning (MSBStrings.W7182 /* Ignoring a native reference with no name in the binding resource package '{0}'. */, r.ItemSpec);
					continue;
				}
				// The 'Name' is combined with the binding resource package's directory to locate (and
				// potentially extract) the native reference, so make sure it can't be used to escape the
				// binding resource package using an absolute path or '..' traversal segments.
				if (IsUnsafeRelativePath (name)) {
					Log.LogError (MSBStrings.E7180 /* The native reference '{0}' in the binding resource package '{1}' is invalid: the name must be a relative path without '..' segments. */, name, r.ItemSpec);
					continue;
				}
				WarnIfIncorrectFrameworkExtension (Log, Path.Combine (resources, name));
				if (name.EndsWith (".xcframework", StringComparison.Ordinal) || name.EndsWith (".xcframework.zip", StringComparison.Ordinal)) {
					if (!TryResolveXCFramework (this, TargetFrameworkMoniker, SdkIsSimulator, Architectures, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, cancellationToken, out var nativeLibraryPath))
						continue;
					SetMetadataNativeLibrary (t, nativeLibraryPath);
				} else if (name.EndsWith (".framework", StringComparison.Ordinal)) {
					string? frameworkPath;
					if (!isCompressed) {
						frameworkPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (this, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, cancellationToken, out frameworkPath)) {
						continue;
					}
					t.ItemSpec = GetActualLibrary (frameworkPath);
					t.SetMetadata ("Kind", "Framework");
					t.SetMetadata ("PublishFolderType", "AppleFramework");
					t.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.GetFileName (Path.GetDirectoryName (t.ItemSpec)!)));
				} else if (name.EndsWith (".dylib", StringComparison.Ordinal)) {
					// macOS
					string? dylibPath;
					if (!isCompressed) {
						dylibPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (this, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, cancellationToken, out dylibPath)) {
						continue;
					}
					t.ItemSpec = dylibPath;
					t.SetMetadata ("Kind", "Dynamic");
					t.SetMetadata ("PublishFolderType", "DynamicLibrary");
				} else if (name.EndsWith (".a", StringComparison.Ordinal)) {
					// static library
					string? aPath;
					if (!isCompressed) {
						aPath = Path.Combine (resources, name);
					} else if (!CompressionHelper.TryDecompress (this, resources, name, GetIntermediateDecompressionDir (resources), createdFiles, cancellationToken, out aPath)) {
						continue;
					}
					t.ItemSpec = aPath;
					t.SetMetadata ("Kind", "Static");
					t.SetMetadata ("PublishFolderType", "StaticLibrary");
				} else {
					Log.LogWarning (MSBStrings.W7105 /* Unexpected extension '{0}' for native reference '{1}' in binding resource package '{2}'. */, Path.GetExtension (name), name, r.ItemSpec);
					t = r;
				}

				// defaults
				t.SetMetadata ("ForceLoad", "False");
				t.SetMetadata ("NeedsGccExceptionHandling", "False");
				t.SetMetadata ("IsCxx", "False");
				t.SetMetadata ("LinkWithSwiftSystemLibraries", "False");
				t.SetMetadata ("SmartLink", "True");

				// Values from the manifest, overriding the defaults above if provided. Skip the
				// build-controlled path/layout/identity metadata: the manifest is passive data and must not
				// be able to inject metadata that could redirect output outside the intended directory.
				foreach (XmlNode attribute in referenceNode.ChildNodes) {
					if (attribute.NodeType != XmlNodeType.Element)
						continue;
					if (blockedManifestMetadata.Contains (attribute.Name)) {
						Log.LogWarning (MSBStrings.W7179 /* Ignoring the metadata '{0}' for the native reference '{1}' in the binding resource package '{2}': this metadata is computed by the build and can't be set in a binding resource package manifest. */, attribute.Name, name, r.ItemSpec);
						continue;
					}
					try {
						t.SetMetadata (attribute.Name, attribute.InnerText);
					} catch (ArgumentException) {
						// Reserved MSBuild metadata names (e.g. 'FullPath') can't be set and shouldn't appear
						// in a manifest; ignore (and warn about) them rather than failing the build.
						Log.LogWarning (MSBStrings.W7179 /* Ignoring the metadata '{0}' for the native reference '{1}' in the binding resource package '{2}': this metadata is computed by the build and can't be set in a binding resource package manifest. */, attribute.Name, name, r.ItemSpec);
					}
				}

				native_frameworks.Add (t);
			}
		}

		/// <summary>
		/// Resolve an xcframework into a native library for a given platform.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="path">Either the path to a compressed xcframework (*.xcframework.zip), or an xcframework (*.xcframework).</param>
		/// <param name="nativeLibraryPath">A full path to the resolved native library within the xcframework. If 'resourcePath' is compressed, this will point to where the native library is decompressed on disk.</param>
		/// <param name="intermediateDecompressionDir"></param>
		/// <returns>True if a native library was successfully found. Otherwise false, and an error will have been printed to the log.</returns>
		public static bool TryResolveXCFramework (XamarinTask task, string targetFrameworkMoniker, bool isSimulator, string? architectures, string path, string intermediateDecompressionDir, List<string> createdFiles, CancellationToken? cancellationToken, [NotNullWhen (true)] out string? nativeLibraryPath)
		{
			string resourcePath;
			string xcframework;

			if (path.EndsWith (".zip", StringComparison.OrdinalIgnoreCase)) {
				resourcePath = path;
				xcframework = Path.GetFileNameWithoutExtension (path); // Remove the .zip extension
			} else {
				resourcePath = Path.GetDirectoryName (path)!;
				xcframework = Path.GetFileName (path);
			}
			return TryResolveXCFramework (task, targetFrameworkMoniker, isSimulator, architectures, resourcePath, xcframework, intermediateDecompressionDir, createdFiles, cancellationToken, out nativeLibraryPath);
		}

		/// <summary>
		/// Resolve an xcframework into a native library for a given platform.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="resourcePath">Either the path to a compressed xcframework, or the containing directory of an xcframework.</param>
		/// <param name="xcframework">The name of the xcframework.</param>
		/// <param name="nativeLibraryPath">A full path to the resolved native library within the xcframework. If 'resourcePath' is compressed, this will point to where the native library is decompressed on disk.</param>
		/// <param name="intermediateDecompressionDir"></param>
		/// <returns>True if a native library was successfully found. Otherwise false, and an error will have been printed to the log.</returns>
		public static bool TryResolveXCFramework (XamarinTask task, string targetFrameworkMoniker, bool isSimulator, string? architectures, string resourcePath, string xcframework, string intermediateDecompressionDir, List<string> createdFiles, CancellationToken? cancellationToken, [NotNullWhen (true)] out string? nativeLibraryPath)
		{
			var log = task.Log;
			nativeLibraryPath = null;

			try {
				if (!TryGetInfoPlist (log, resourcePath, xcframework, out var plist))
					return false;

				var isCompressed = CompressionHelper.IsCompressed (resourcePath);
				var xcframeworkPath = isCompressed ? resourcePath : Path.Combine (resourcePath, xcframework);
				if (!TryResolveXCFramework (log, plist, xcframeworkPath, targetFrameworkMoniker, isSimulator, architectures!, cancellationToken, out var nativeLibraryRelativePath))
					return false;

				if (!isCompressed && CompressionHelper.IsCompressed (xcframework)) {
					var zipPath = Path.Combine (resourcePath, xcframework);
					var xcframeworkName = Path.GetFileNameWithoutExtension (xcframework);
					if (!CompressionHelper.TryDecompress (task, zipPath, xcframeworkName, intermediateDecompressionDir, createdFiles, cancellationToken, out var decompressedXcframeworkPath))
						return false;

					nativeLibraryPath = Path.Combine (intermediateDecompressionDir, xcframeworkName, nativeLibraryRelativePath);
					return true;
				}

				if (!isCompressed) {
					nativeLibraryPath = Path.Combine (resourcePath, xcframework, nativeLibraryRelativePath);
					return true;
				}

				var zipResource = Path.Combine (xcframework, Path.GetDirectoryName (nativeLibraryRelativePath)!);
				if (!CompressionHelper.TryDecompress (task, resourcePath, zipResource, intermediateDecompressionDir, createdFiles, cancellationToken, out var decompressedPath))
					return false;

				nativeLibraryPath = Path.Combine (intermediateDecompressionDir, xcframework, nativeLibraryRelativePath);

				return true;
			} catch (Exception e) {
				log.LogError (MSBStrings.E0174 /* Failed to resolve the xcframework {0} in {1} due to an unexpected error: {2} */, xcframework, resourcePath, e.Message);
				log.LogErrorsFromException (e); // report the original exception too.
			}

			return false;
		}

		/// <summary>
		/// Resolve an xcframework into a native library for a given platform.
		/// </summary>
		/// <param name="log">The log to log any errors and/or warnings.</param>
		/// <param name="plist">The plist inside the xcframework.</param>
		/// <param name="xcframeworkPath">The path to the xcframework. This is only used for error messages, so it can also point to a compressed xcframework.</param>
		/// <param name="isSimulator">If we're targeting the simulator</param>
		/// <param name="targetFrameworkMoniker">The target framework moniker.</param>
		/// <param name="architectures">The target architectures</param>
		/// <param name="frameworkPath">A relative path to the resolved native library within the xcframework.</param>
		/// <returns>True if a native library was successfully found. Otherwise false, and an error will have been printed to the log.</returns>
		public static bool TryResolveXCFramework (TaskLoggingHelper log, PDictionary plist, string xcframeworkPath, string targetFrameworkMoniker, bool isSimulator, string architectures, CancellationToken? cancellationToken, [NotNullWhen (true)] out string? nativeLibraryPath)
		{
			nativeLibraryPath = null;
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
				log.LogError (MSBStrings.E0174a /* The xcframework {0} has an incorrect or unknown format and cannot be processed: expected the value for 'CFBundlePackageType' to be 'XFWK' in the xcframework's Info.plist, but it's '{1}'. */, xcframeworkPath, bundle_package_type?.Value);
				return false;
			}
			var available_libraries = plist.GetArray ("AvailableLibraries");
			if (available_libraries is null) {
				log.LogError (MSBStrings.E0174b /* The xcframework {0} has an incorrect or unknown format and cannot be processed: there's no 'AvailableLibraries' entry in the xcframework's Info.plist. */, xcframeworkPath);
				return false;
			} else if (available_libraries.Count == 0) {
				log.LogError (MSBStrings.E0174c /* The xcframework {0} has an incorrect or unknown format and cannot be processed: expected some 'AvailableLibraries' entries in the xcframework's Info.plist, but found 0 entries. */, xcframeworkPath);
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
						log.LogError (MSBStrings.E0175 /* No matching framework found inside '{0}'. SupportedPlatform: '{1}', SupportedPlatformVariant: '{2}', SupportedArchitectures: '{3}'. */, xcframeworkPath, platformName, variant, architectures);
						return false;
					}
				}
				var library_path = (PString?) item ["LibraryPath"];
				var library_identifier = (PString?) item ["LibraryIdentifier"];
				var libraryPath = Path.Combine (library_identifier!, library_path!);
				WarnIfIncorrectFrameworkExtension (log, Path.Combine (xcframeworkPath, libraryPath));
				nativeLibraryPath = GetActualLibrary (libraryPath);
				return true;
			}

			log.LogError (MSBStrings.E0175 /* No matching framework found inside '{0}'. SupportedPlatform: '{1}', SupportedPlatformVariant: '{2}', SupportedArchitectures: '{3}'. */, xcframeworkPath, platformName, variant, architectures);
			return false;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ()) {
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
			} else {
				cancellationTokenSource?.Cancel ();
			}
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
				var resourcesPackage = Path.Combine (Path.GetDirectoryName (reference.ItemSpec)!, Path.GetFileNameWithoutExtension (reference.ItemSpec)) + ".resources";
				if (Directory.Exists (resourcesPackage)) {
					var resources = CreateItemsForAllFilesRecursively (new string [] { resourcesPackage }).ToList ();
					Log.LogMessage (MessageImportance.Low, $"Found the resources directory '{resourcesPackage}' for the reference '{reference.ItemSpec}', copying {resources.Count} file(s) from it.");
					rv.AddRange (resources);
					continue;
				}
				var zipPackage = resourcesPackage + ".zip";
				if (File.Exists (zipPackage)) {
					Log.LogMessage (MessageImportance.Low, $"Found the resources zip file '{zipPackage}' for the reference '{reference.ItemSpec}', copying it.");
					rv.Add (new TaskItem (zipPackage));
				} else {
					Log.LogMessage (MessageImportance.Low, $"No resources directory ('{resourcesPackage}') nor resources zip file ('{zipPackage}') found for the reference '{reference.ItemSpec}'.");
				}
			}
			return rv;
		}
	}
}
