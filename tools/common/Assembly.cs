using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Mono.Cecil;
using Mono.Tuner;
using MonoTouch.Tuner;
using ObjCRuntime;
using Xamarin;
using Xamarin.Utils;

namespace Xamarin.Bundler {

	struct NativeReferenceMetadata {
		public bool ForceLoad;
		public string Frameworks;
		public string WeakFrameworks;
		public string LibraryName;
		public string LinkerFlags;
		public LinkTarget LinkTarget;
		public bool NeedsGccExceptionHandling;
		public bool IsCxx;
		public bool SmartLink;
		public DlsymOption Dlsym;

		// Optional
		public LinkWithAttribute Attribute;

		public NativeReferenceMetadata (LinkWithAttribute attribute)
		{
			ForceLoad = attribute.ForceLoad;
			Frameworks = attribute.Frameworks;
			WeakFrameworks = attribute.WeakFrameworks;
			LibraryName = attribute.LibraryName;
			LinkerFlags = attribute.LinkerFlags;
			LinkTarget = attribute.LinkTarget;
			NeedsGccExceptionHandling = attribute.NeedsGccExceptionHandling;
			IsCxx = attribute.IsCxx;
			SmartLink = attribute.SmartLink;
			Dlsym = attribute.Dlsym;
			Attribute = attribute;
		}
	}

	public partial class Assembly {
		public AssemblyBuildTarget BuildTarget;
		public string BuildTargetName;
		public bool IsCodeShared;
		public List<string> Satellites;
		public Application App { get { return Target.App; } }

		string full_path;
		bool? is_framework_assembly;

		public AssemblyDefinition AssemblyDefinition;
		public Target Target;
		public bool? IsFrameworkAssembly { get { return is_framework_assembly; } }
		public string FullPath {
			get {
				return full_path;
			}
			set {
				full_path = value;
				if (!is_framework_assembly.HasValue && !string.IsNullOrEmpty (full_path)) {
#if NET
					is_framework_assembly = Target.App.Configuration.FrameworkAssemblies.Contains (GetIdentity (full_path));
#else
					var real_full_path = Target.GetRealPath (full_path);
					is_framework_assembly = real_full_path.StartsWith (Path.GetDirectoryName (Path.GetDirectoryName (Target.Resolver.FrameworkDirectory)), StringComparison.Ordinal);
#endif
				}
			}
		}
		public string FileName { get { return Path.GetFileName (FullPath); } }
		public string Identity { get { return GetIdentity (AssemblyDefinition); } }

		public static string GetIdentity (AssemblyDefinition ad)
		{
			if (!string.IsNullOrEmpty (ad.MainModule.FileName))
				return Path.GetFileNameWithoutExtension (ad.MainModule.FileName);
			return ad.Name.Name;
		}

		public static string GetIdentity (string path)
		{
			return Path.GetFileNameWithoutExtension (path);
		}

		public bool EnableCxx;
		public bool NeedsGccExceptionHandling;
		public bool ForceLoad;
		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();
		public List<string> LinkerFlags = new List<string> (); // list of extra linker flags
		public List<string> LinkWith = new List<string> (); // list of paths to native libraries to link with, from LinkWith attributes
		public HashSet<ModuleReference> UnresolvedModuleReferences;
		public bool HasLinkWithAttributes { get; private set; }

		bool? symbols_loaded;

		List<string> link_with_resources; // a list of resources that must be removed from the app

		public Assembly (Target target, string path)
		{
			this.Target = target;
			this.FullPath = path;
		}

		public Assembly (Target target, AssemblyDefinition definition)
		{
			this.Target = target;
			this.AssemblyDefinition = definition;
			this.FullPath = definition.MainModule.FileName;
		}

		public bool HasValidSymbols {
			get {
				return AssemblyDefinition.MainModule.HasSymbols;
			}
		}

		public void LoadSymbols ()
		{
			if (symbols_loaded.HasValue)
				return;

			symbols_loaded = false;
			try {
				var pdb = Path.ChangeExtension (FullPath, ".pdb");
				if (File.Exists (pdb) || File.Exists (FullPath + ".mdb")) {
					AssemblyDefinition.MainModule.ReadSymbols ();
					symbols_loaded = true;
				}
			} catch {
				// do not let stale file crash us
				Driver.Log (3, "Invalid debugging symbols for {0} ignored", FullPath);
			}
		}

		void AddResourceToBeRemoved (string resource)
		{
			if (link_with_resources is null)
				link_with_resources = new List<string> ();
			link_with_resources.Add (resource);
		}

		public void ExtractNativeLinkInfo ()
		{
			// ignore framework assemblies, they won't have any LinkWith attributes
			if (IsFrameworkAssembly == true)
				return;

			var assembly = AssemblyDefinition;
			if (!assembly.HasCustomAttributes)
				return;

			string resourceBundlePath = Path.ChangeExtension (FullPath, ".resources");
			if (!Directory.Exists (resourceBundlePath)) {
				var zipPath = resourceBundlePath + ".zip";
				if (File.Exists (zipPath)) {
					var path = Path.Combine (App.Cache.Location, Path.GetFileName (resourceBundlePath));
					if (Driver.RunCommand ("/usr/bin/unzip", "-u", "-o", "-d", path, zipPath) != 0)
						throw ErrorHelper.CreateError (1306, Errors.MX1306 /* Could not decompress the file '{0}'. Please review the build log for more information from the native 'unzip' command. */, zipPath);
					resourceBundlePath = path;
				}
			}
			string manifestPath = Path.Combine (resourceBundlePath, "manifest");
			if (File.Exists (manifestPath)) {
				foreach (NativeReferenceMetadata metadata in ReadManifest (manifestPath)) {
					LogNativeReference (metadata);
					ProcessNativeReferenceOptions (metadata);

					switch (Path.GetExtension (metadata.LibraryName).ToLowerInvariant ()) {
					case ".framework":
						AssertiOSVersionSupportsUserFrameworks (metadata.LibraryName);
						Frameworks.Add (metadata.LibraryName);
#if MMP // HACK - MMP currently doesn't respect Frameworks on non-App - https://github.com/xamarin/xamarin-macios/issues/5203
						App.Frameworks.Add (metadata.LibraryName);
#endif
						break;
					case ".xcframework":
						// this is resolved, at msbuild time, into a framework
						// but we must ignore it here (can't be the `default` case)
						break;
					default:
#if MMP // HACK - MMP currently doesn't respect LinkWith - https://github.com/xamarin/xamarin-macios/issues/5203
						Driver.native_references.Add (metadata.LibraryName);
#endif
						LinkWith.Add (metadata.LibraryName);
						break;
					}
				}
			}

			ProcessLinkWithAttributes (assembly);

			// Make sure there are no duplicates between frameworks and weak frameworks.
			// Keep the weak ones.
			if (Frameworks is not null && WeakFrameworks is not null)
				Frameworks.ExceptWith (WeakFrameworks);

			if (NeedsGccExceptionHandling) {
				if (LinkerFlags is null)
					LinkerFlags = new List<string> ();
				LinkerFlags.Add ("-lgcc_eh");
			}

		}

		IEnumerable<NativeReferenceMetadata> ReadManifest (string manifestPath)
		{
			XmlDocument document = new XmlDocument ();
			document.LoadWithoutNetworkAccess (manifestPath);

			foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {

				NativeReferenceMetadata metadata = new NativeReferenceMetadata ();
				metadata.LibraryName = Path.Combine (Path.GetDirectoryName (manifestPath), referenceNode.Attributes ["Name"].Value);

				var attributes = new Dictionary<string, string> ();
				foreach (XmlNode attribute in referenceNode.ChildNodes)
					attributes [attribute.Name] = attribute.InnerText;

				metadata.ForceLoad = ParseAttributeWithDefault (attributes ["ForceLoad"], false);
				metadata.Frameworks = attributes ["Frameworks"];
				metadata.WeakFrameworks = attributes ["WeakFrameworks"];
				metadata.LinkerFlags = attributes ["LinkerFlags"];
				metadata.NeedsGccExceptionHandling = ParseAttributeWithDefault (attributes ["NeedsGccExceptionHandling"], false);
				metadata.IsCxx = ParseAttributeWithDefault (attributes ["IsCxx"], false);
				metadata.SmartLink = ParseAttributeWithDefault (attributes ["SmartLink"], true);

				// TODO - The project attributes do not contain these bits, is that OK?
				//metadata.LinkTarget = (LinkTarget) Enum.Parse (typeof (LinkTarget), attributes ["LinkTarget"]);
				//metadata.Dlsym = (DlsymOption)Enum.Parse (typeof (DlsymOption), attributes ["Dlsym"]);
				yield return metadata;
			}
		}

		static bool ParseAttributeWithDefault (string attribute, bool defaultValue) => string.IsNullOrEmpty (attribute) ? defaultValue : bool.Parse (attribute);

		void ProcessLinkWithAttributes (AssemblyDefinition assembly)
		{
			//
			// Tasks:
			// * Remove LinkWith attribute: this is done in the linker.
			// * Remove embedded resources related to LinkWith attribute from assembly: this is done at a later stage,
			//   here we just compile a list of resources to remove.
			// * Extract embedded resources related to LinkWith attribute to a file
			// * Modify the linker flags used to build/link the dylib (if fastdev) or the main binary (if !fastdev)
			// 

			for (int i = 0; i < assembly.CustomAttributes.Count; i++) {
				CustomAttribute attr = assembly.CustomAttributes [i];

				if (attr.Constructor is null)
					continue;

				TypeReference type = attr.Constructor.DeclaringType;
				if (!type.Is ("ObjCRuntime", "LinkWithAttribute"))
					continue;

				// Let the linker remove it the attribute from the assembly
				HasLinkWithAttributes = true;

				LinkWithAttribute linkWith = GetLinkWithAttribute (attr);
				NativeReferenceMetadata metadata = new NativeReferenceMetadata (linkWith);

				// If we've already processed this native library, skip it
				if (LinkWith.Any (x => Path.GetFileName (x) == metadata.LibraryName) || Frameworks.Any (x => Path.GetFileName (x) == metadata.LibraryName))
					continue;

				// Remove the resource from the assembly at a later stage.
				if (!string.IsNullOrEmpty (metadata.LibraryName))
					AddResourceToBeRemoved (metadata.LibraryName);

				ProcessNativeReferenceOptions (metadata);

				if (!string.IsNullOrEmpty (linkWith.LibraryName)) {
					switch (Path.GetExtension (linkWith.LibraryName).ToLowerInvariant ()) {
					case ".framework": {
						AssertiOSVersionSupportsUserFrameworks (linkWith.LibraryName);
						// TryExtractFramework prints a error/warning if something goes wrong, so no need for us to have an error handling path.
						if (TryExtractFramework (assembly, metadata, out var framework))
							Frameworks.Add (framework);
						break;
					}
					case ".xcframework":
						// this is resolved, at msbuild time, into a framework
						// but we must ignore it here (can't be the `default` case)
						break;
					default: {
						// TryExtractFramework prints a error/warning if something goes wrong, so no need for us to have an error handling path.
						if (TryExtractNativeLibrary (assembly, metadata, out var framework))
							LinkWith.Add (framework);
						break;
					}
					}
				}
			}
		}

		void AssertiOSVersionSupportsUserFrameworks (string path)
		{
			if (App.Platform == ApplePlatform.iOS && App.DeploymentTarget.Major < 8) {
				throw ErrorHelper.CreateError (1305, Errors.MT1305,
					FileName, Path.GetFileName (path), App.DeploymentTarget);
			}
		}

		void ProcessNativeReferenceOptions (NativeReferenceMetadata metadata)
		{
			// We can't add -dead_strip if there are any LinkWith attributes where smart linking is disabled.
			if (!metadata.SmartLink) {
				Driver.Log (3, $"The library '{metadata.LibraryName}', shipped with the assembly '{FullPath}', sets SmartLink=false, which will disable passing -dead_strip to the native linker (and make the app bigger).");
				App.DeadStrip = false;
			}

			// Don't add -force_load if the binding's SmartLink value is set and the static registrar is being used.
			if (metadata.ForceLoad && !(metadata.SmartLink && (App.Registrar == RegistrarMode.Static || App.Registrar == RegistrarMode.ManagedStatic)))
				ForceLoad = true;

			if (!string.IsNullOrEmpty (metadata.LinkerFlags)) {
				if (LinkerFlags is null)
					LinkerFlags = new List<string> ();
				if (!StringUtils.TryParseArguments (metadata.LinkerFlags, out string [] args, out var ex))
					throw ErrorHelper.CreateError (148, ex, Errors.MX0148, metadata.LinkerFlags, metadata.LibraryName, FileName, ex.Message);
				LinkerFlags.AddRange (args);
			}

			if (!string.IsNullOrEmpty (metadata.Frameworks)) {
				foreach (var f in metadata.Frameworks.Split (new char [] { ' ' })) {
					if (Frameworks is null)
						Frameworks = new HashSet<string> ();
					Frameworks.Add (f);
				}
			}

			if (!string.IsNullOrEmpty (metadata.WeakFrameworks)) {
				foreach (var f in metadata.WeakFrameworks.Split (new char [] { ' ' })) {
					if (WeakFrameworks is null)
						WeakFrameworks = new HashSet<string> ();
					WeakFrameworks.Add (f);
				}
			}

			if (metadata.NeedsGccExceptionHandling)
				NeedsGccExceptionHandling = true;

			if (metadata.IsCxx)
				EnableCxx = true;

#if MONOTOUCH
			if (metadata.Dlsym != DlsymOption.Default)
				App.SetDlsymOption (FullPath, metadata.Dlsym == DlsymOption.Required);
#endif
		}

		bool TryExtractNativeLibrary (AssemblyDefinition assembly, NativeReferenceMetadata metadata, out string library)
		{
			string path = Path.Combine (App.Cache.Location, metadata.LibraryName);

			library = null;

			if (!Application.IsUptodate (FullPath, path)) {
				if (!Application.ExtractResource (assembly.MainModule, metadata.LibraryName, path, false)) {
					ErrorHelper.Warning (1308, Errors.MX1308 /* Could not extract the native library '{0}' from the assembly '{1}', because it doesn't contain the resource '{2}'. */, metadata.LibraryName, FullPath, metadata.LibraryName);
					return false;
				}
				Driver.Log (3, "Extracted third-party binding '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, path);
				LogNativeReference (metadata);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (path))
				ErrorHelper.Warning (1302, Errors.MT1302, metadata.LibraryName, path);

			library = path;
			return true;
		}

		bool TryExtractFramework (AssemblyDefinition assembly, NativeReferenceMetadata metadata, out string framework)
		{
			string path = Path.Combine (App.Cache.Location, metadata.LibraryName);

			var zipPath = path + ".zip";

			framework = null;

			if (!Application.IsUptodate (FullPath, zipPath)) {
				if (!Application.ExtractResource (assembly.MainModule, metadata.LibraryName, zipPath, false)) {
					ErrorHelper.Warning (1307, Errors.MX1307 /* Could not extract the native framework '{0}' from the assembly '{1}', because it doesn't contain the resource '{2}'. */, metadata.LibraryName, FullPath, metadata.LibraryName);
					return false;
				}

				Driver.Log (3, "Extracted third-party framework '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, zipPath);
				LogNativeReference (metadata);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (zipPath)) {
				ErrorHelper.Warning (1302, Errors.MT1302, metadata.LibraryName, FullPath);
				if (assembly.MainModule.HasResources) {
					Driver.Log (3, $"The assembly {FullPath} has {assembly.MainModule.Resources.Count} resources:");
					foreach (var res in assembly.MainModule.Resources) {
						Driver.Log (3, $"    {res.ResourceType}: {res.Name}");
					}
				} else {
					Driver.Log (3, $"The assembly {FullPath} does not have any resources.");
				}
			} else {
				if (!Directory.Exists (path))
					Directory.CreateDirectory (path);

				if (Driver.RunCommand ("/usr/bin/unzip", "-u", "-o", "-d", path, zipPath) != 0)
					throw ErrorHelper.CreateError (1303, Errors.MT1303, metadata.LibraryName, zipPath);
			}

			framework = path;
			return true;
		}

		static void LogNativeReference (NativeReferenceMetadata metadata)
		{
			Driver.Log (3, "    LibraryName: {0}", metadata.LibraryName);
			Driver.Log (3, "    From: {0}", metadata.Attribute is not null ? "LinkWith" : "Binding Manifest");
			Driver.Log (3, "    ForceLoad: {0}", metadata.ForceLoad);
			Driver.Log (3, "    Frameworks: {0}", metadata.Frameworks);
			Driver.Log (3, "    IsCxx: {0}", metadata.IsCxx);
			Driver.Log (3, "    LinkerFlags: {0}", metadata.LinkerFlags);
			Driver.Log (3, "    LinkTarget: {0}", metadata.LinkTarget);
			Driver.Log (3, "    NeedsGccExceptionHandling: {0}", metadata.NeedsGccExceptionHandling);
			Driver.Log (3, "    SmartLink: {0}", metadata.SmartLink);
			Driver.Log (3, "    WeakFrameworks: {0}", metadata.WeakFrameworks);
		}

		public static LinkWithAttribute GetLinkWithAttribute (CustomAttribute attr)
		{
			LinkWithAttribute linkWith;

			var cargs = attr.ConstructorArguments;
			switch (cargs.Count) {
			case 3:
				linkWith = new LinkWithAttribute ((string) cargs [0].Value, (LinkTarget) cargs [1].Value, (string) cargs [2].Value);
				break;
			case 2:
				linkWith = new LinkWithAttribute ((string) cargs [0].Value, (LinkTarget) cargs [1].Value);
				break;
			case 0:
				linkWith = new LinkWithAttribute ();
				break;
			default:
			case 1:
				linkWith = new LinkWithAttribute ((string) cargs [0].Value);
				break;
			}

			foreach (var property in attr.Properties) {
				switch (property.Name) {
				case "NeedsGccExceptionHandling":
					linkWith.NeedsGccExceptionHandling = (bool) property.Argument.Value;
					break;
				case "WeakFrameworks":
					linkWith.WeakFrameworks = (string) property.Argument.Value;
					break;
				case "Frameworks":
					linkWith.Frameworks = (string) property.Argument.Value;
					break;
				case "LinkerFlags":
					linkWith.LinkerFlags = (string) property.Argument.Value;
					break;
				case "LinkTarget":
					linkWith.LinkTarget = (LinkTarget) property.Argument.Value;
					break;
				case "ForceLoad":
					linkWith.ForceLoad = (bool) property.Argument.Value;
					break;
				case "IsCxx":
					linkWith.IsCxx = (bool) property.Argument.Value;
					break;
				case "SmartLink":
					linkWith.SmartLink = (bool) property.Argument.Value;
					break;
				case "Dlsym":
					linkWith.Dlsym = (DlsymOption) property.Argument.Value;
					break;
				default:
					break;
				}
			}

			return linkWith;
		}

		void AddFramework (string file)
		{
			if (Driver.GetFrameworks (App).TryGetValue (file, out var framework)) {
				if (framework.Unavailable) {
					ErrorHelper.Warning (182, Errors.MX0182 /* Not linking with the framework {0} (referenced by a module reference in {1}) because it's not available on the current platform ({2}). */, framework.Name, FileName, App.PlatformName);
					return;
				}

				if (framework.Version > App.SdkVersion) {
					ErrorHelper.Warning (135, Errors.MX0135, file, FileName, App.PlatformName, framework.Version, App.SdkVersion);
					return;
				}
			}

			var strong = (framework is null) || (App.DeploymentTarget >= (App.IsSimulatorBuild ? framework.VersionAvailableInSimulator ?? framework.Version : framework.Version));
			if (strong) {
				if (Frameworks.Add (file))
					Driver.Log (3, "Linking with the framework {0} because it's referenced by a module reference in {1}", file, FileName);
			} else {
				if (WeakFrameworks.Add (file))
					Driver.Log (3, "Linking (weakly) with the framework {0} because it's referenced by a module reference in {1}", file, FileName);
			}
		}

		public string GetCompressionLinkingFlag ()
		{
			switch (App.Platform) {
			case ApplePlatform.MacOSX:
				if (App.DeploymentTarget >= new Version (10, 11, 0))
					return "-lcompression";
				return "-weak-lcompression";
			case ApplePlatform.iOS:
			case ApplePlatform.MacCatalyst:
				if (App.DeploymentTarget >= new Version (9, 0))
					return "-lcompression";
				return "-weak-lcompression";
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return "-lcompression";
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, App.Platform, App.SdkVersion);
			}
		}

		public void ComputeLinkerFlags ()
		{
			foreach (var m in AssemblyDefinition.Modules) {
				if (!m.HasModuleReferences)
					continue;

				foreach (var mr in m.ModuleReferences) {
					string name = mr.Name;
					if (string.IsNullOrEmpty (name))
						continue; // obfuscated assemblies.

					string file = Path.GetFileNameWithoutExtension (name);

					if (App.IsSimulatorBuild && !App.IsFrameworkAvailableInSimulator (file)) {
						Driver.Log (3, "Not linking with {0} (referenced by a module reference in {1}) because it's not available in the simulator.", file, FileName);
						continue;
					}

					switch (file) {
					// special case
					case "__Internal":
					case "System.Native":
					case "System.Security.Cryptography.Native.Apple":
					case "System.Net.Security.Native":
					// well known libs
					case "libc":
					case "libSystem":
					case "libobjc":
					case "libdyld":
					case "libsystem_kernel":
						break;
					case "sqlite3":
						LinkerFlags.Add ("-lsqlite3");
						Driver.Log (3, "Linking with {0} because it's referenced by a module reference in {1}", file, FileName);
						break;
					case "libsqlite3":
						// remove lib prefix
						LinkerFlags.Add ("-l" + file.Substring (3));
						Driver.Log (3, "Linking with {0} because it's referenced by a module reference in {1}", file, FileName);
						break;
					case "libcompression":
						LinkerFlags.Add (GetCompressionLinkingFlag ());
						break;
					case "libGLES":
					case "libGLESv2":
						// special case for OpenGLES.framework
						if (Frameworks.Add ("OpenGLES"))
							Driver.Log (3, "Linking with the framework OpenGLES because {0} is referenced by a module reference in {1}", file, FileName);
						break;
					case "vImage":
					case "vecLib":
						// sub-frameworks
						if (Frameworks.Add ("Accelerate"))
							Driver.Log (3, "Linking with the framework Accelerate because {0} is referenced by a module reference in {1}", file, FileName);
						break;
					case "openal32":
						if (Frameworks.Add ("OpenAL"))
							Driver.Log (3, "Linking with the framework OpenAL because {0} is referenced by a module reference in {1}", file, FileName);
						break;
#if NET
					case "Carbon":
						if (App.Platform != ApplePlatform.MacOSX) {
							Driver.Log (3, $"Not linking with the framework {file} (referenced by a module reference in {FileName}) because it doesn't exist on the target platform.");
							break;
						}
						break;
#endif
					default:
						if (App.Platform == ApplePlatform.MacOSX) {
							string path = Path.GetDirectoryName (name);
							if (!path.StartsWith ("/System/Library/Frameworks", StringComparison.Ordinal))
								continue;

							// CoreServices has multiple sub-frameworks that can be used by customer code
							if (path.StartsWith ("/System/Library/Frameworks/CoreServices.framework/", StringComparison.Ordinal)) {
								if (Frameworks.Add ("CoreServices"))
									Driver.Log (3, "Linking with the framework CoreServices because {0} is referenced by a module reference in {1}", file, FileName);
								break;
							}
							// ApplicationServices has multiple sub-frameworks that can be used by customer code
							if (path.StartsWith ("/System/Library/Frameworks/ApplicationServices.framework/", StringComparison.Ordinal)) {
								if (Frameworks.Add ("ApplicationServices"))
									Driver.Log (3, "Linking with the framework ApplicationServices because {0} is referenced by a module reference in {1}", file, FileName);
								break;
							}
						}

						// detect frameworks
						int f = name.IndexOf (".framework/", StringComparison.Ordinal);
						if (f > 0) {
							AddFramework (file);
						} else {
							if (UnresolvedModuleReferences is null)
								UnresolvedModuleReferences = new HashSet<ModuleReference> ();
							UnresolvedModuleReferences.Add (mr);
							Driver.Log (3, "Could not resolve the module reference {0} in {1}", file, FileName);
						}
						break;
					}
				}
			}
		}

		public override string ToString ()
		{
			return FileName;
		}

		// This returns the path to all related files:
		// * The assembly itself
		// * Any debug files (mdb/pdb)
		// * Any config files
		// * Any satellite assemblies
		public IEnumerable<string> GetRelatedFiles ()
		{
			yield return FullPath;
			var mdb = FullPath + ".mdb";
			if (File.Exists (mdb))
				yield return mdb;
			var pdb = Path.ChangeExtension (FullPath, ".pdb");
			if (File.Exists (pdb))
				yield return pdb;
			var config = FullPath + ".config";
			if (File.Exists (config))
				yield return config;
			if (Satellites is not null) {
				foreach (var satellite in Satellites)
					yield return satellite;
			}
		}

		public void ComputeSatellites ()
		{
			var satellite_name = Path.GetFileNameWithoutExtension (FullPath) + ".resources.dll";
			var path = Path.GetDirectoryName (FullPath);
			// first look if satellites are located in subdirectories of the current location of the assembly
			ComputeSatellites (satellite_name, path);
			if (Satellites is null) {
				// 2nd chance: satellite assemblies can come from different nugets (as dependencies)
				// they will be copied (at build time) into the destination directory (making them work at runtime)
				// but they won't be side-by-side the original assembly (which breaks our build time assumptions)
				path = Path.GetDirectoryName (App.RootAssemblies [0]);
				if (string.IsNullOrEmpty (path))
					path = Environment.CurrentDirectory;
				ComputeSatellites (satellite_name, path);
			}
		}

		void ComputeSatellites (string satellite_name, string path)
		{
			foreach (var subdir in Directory.GetDirectories (path)) {
				var culture_name = Path.GetFileName (subdir);
				CultureInfo ci;

				if (culture_name.IndexOf ('.') >= 0)
					continue; // cultures can't have dots. This way we don't check every *.app directory

				// well-known subdirectories (that are not cultures) to avoid (slow) exceptions handling
				switch (culture_name) {
				case "Facades":
				case "repl":
				case "device-builds":
				case "Design": // XF
					continue;
				}

				try {
					ci = CultureInfo.GetCultureInfo (culture_name);
				} catch {
					// nope, not a resource language
					continue;
				}

				if (ci is null)
					continue;

				var satellite = Path.Combine (subdir, satellite_name);
				if (File.Exists (satellite)) {
					if (Satellites is null)
						Satellites = new List<string> ();
					Satellites.Add (satellite);
				}
			}
		}

		public void CopySatellitesToDirectory (string directory)
		{
			if (Satellites is null)
				return;

			foreach (var a in Satellites) {
				string target_dir = Path.Combine (directory, Path.GetFileName (Path.GetDirectoryName (a)));
				string target_s = Path.Combine (target_dir, Path.GetFileName (a));

				if (!Directory.Exists (target_dir))
					Directory.CreateDirectory (target_dir);

				CopyAssembly (a, target_s);
			}
		}

		public delegate bool StripAssembly (string path);

		// returns false if the assembly was not copied (because it was already up-to-date).
		public bool CopyAssembly (string source, string target, bool copy_debug_symbols = true, StripAssembly strip = null)
		{
			var copied = false;

			try {
				var strip_assembly = strip is not null && strip (source);
				if (!Application.IsUptodate (source, target) && (strip_assembly || !Cache.CompareAssemblies (source, target))) {
					copied = true;
					if (strip_assembly) {
						PathUtils.FileDelete (target);
						Directory.CreateDirectory (Path.GetDirectoryName (target));
						MonoTouch.Tuner.Stripper.Process (source, target);
					} else {
						Application.CopyFile (source, target);
					}
				} else {
					Driver.Log (3, "Target '{0}' is up-to-date.", target);
				}

				// Update the debug symbols file even if the assembly didn't change.
				if (copy_debug_symbols && HasValidSymbols) {
					// Unfortunately Cecil won't tell us the path of the symbol file, so we have to try all we support (.pdb+.mdb)
					if (File.Exists (source + ".mdb"))
						Application.UpdateFile (source + ".mdb", target + ".mdb", true);

					var spdb = Path.ChangeExtension (source, "pdb");
					if (File.Exists (spdb))
						Application.UpdateFile (spdb, Path.ChangeExtension (target, "pdb"), true);
				}

				CopyConfigToDirectory (Path.GetDirectoryName (target));
			} catch (Exception e) {
				throw new ProductException (1009, true, e, Errors.MX1009, source, target, e.Message);
			}

			return copied;
		}

		public void CopyConfigToDirectory (string directory)
		{
			string config_src = FullPath + ".config";
			if (File.Exists (config_src)) {
				string config_target = Path.Combine (directory, FileName + ".config");
				Application.UpdateFile (config_src, config_target, true);
			}
		}

		public bool IsInterpreted {
			get {
				return App.IsInterpreted (Identity);
			}
		}

		public bool IsAOTCompiled {
			get {
				return App.IsAOTCompiled (Identity);
			}
		}
	}

	public sealed class NormalizedStringComparer : IEqualityComparer<string> {
		public static readonly NormalizedStringComparer OrdinalIgnoreCase = new NormalizedStringComparer (StringComparer.OrdinalIgnoreCase);

		StringComparer comparer;

		public NormalizedStringComparer (StringComparer comparer)
		{
			this.comparer = comparer;
		}

		public bool Equals (string x, string y)
		{
			// From what I gather it doesn't matter which normalization form
			// is used, but I chose Form D because HFS normalizes to Form D.
			if (x is not null)
				x = x.Normalize (System.Text.NormalizationForm.FormD);
			if (y is not null)
				y = y.Normalize (System.Text.NormalizationForm.FormD);
			return comparer.Equals (x, y);
		}

		public int GetHashCode (string obj)
		{
			return comparer.GetHashCode (obj?.Normalize (System.Text.NormalizationForm.FormD));
		}
	}

	public class AssemblyCollection : IEnumerable<Assembly> {
		Dictionary<string, Assembly> HashedAssemblies = new Dictionary<string, Assembly> (NormalizedStringComparer.OrdinalIgnoreCase);

		public void Add (Assembly assembly)
		{
			Assembly other;
			if (HashedAssemblies.TryGetValue (assembly.Identity, out other))
				throw ErrorHelper.CreateError (2018, Errors.MT2018, assembly.Identity, other.FullPath, assembly.FullPath);
			HashedAssemblies.Add (assembly.Identity, assembly);
		}

		public void AddRange (AssemblyCollection assemblies)
		{
			foreach (var a in assemblies)
				Add (a);
		}

		public int Count {
			get {
				return HashedAssemblies.Count;
			}
		}

		public IDictionary<string, Assembly> Hashed {
			get { return HashedAssemblies; }
		}

		public bool TryGetValue (string identity, out Assembly assembly)
		{
			return HashedAssemblies.TryGetValue (identity, out assembly);
		}

		public bool TryGetValue (AssemblyDefinition asm, out Assembly assembly)
		{
			return HashedAssemblies.TryGetValue (Assembly.GetIdentity (asm), out assembly);
		}

		public bool Contains (AssemblyDefinition asm)
		{
			return HashedAssemblies.ContainsKey (Assembly.GetIdentity (asm));
		}

		public bool ContainsKey (string identity)
		{
			return HashedAssemblies.ContainsKey (identity);
		}

		public void Remove (string identity)
		{
			HashedAssemblies.Remove (identity);
		}

		public void Remove (Assembly assembly)
		{
			Remove (assembly.Identity);
		}

		public Assembly this [string key] {
			get { return HashedAssemblies [key]; }
			set { HashedAssemblies [key] = value; }
		}

		public void Update (Target target, IEnumerable<AssemblyDefinition> assemblies)
		{
			// This function will remove any assemblies not in 'assemblies', and add any new assemblies.
			var current = new HashSet<string> (HashedAssemblies.Keys, HashedAssemblies.Comparer);
			foreach (var assembly in assemblies) {
				var identity = Assembly.GetIdentity (assembly);
				if (!current.Remove (identity)) {
					// new assembly
					var asm = new Assembly (target, assembly);
					Add (asm);
					Driver.Log (1, "The linker added the assembly '{0}' to '{1}' to satisfy a reference.", asm.Identity, target.App.Name);
				} else {
					this [identity].AssemblyDefinition = assembly;
				}
			}

			foreach (var removed in current) {
				Driver.Log (1, "The linker removed the assembly '{0}' from '{1}' since there is no more reference to it.", this [removed].Identity, target.App.Name);
				Remove (removed);
			}
		}

		#region Interface implementations
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public IEnumerator<Assembly> GetEnumerator ()
		{
			return HashedAssemblies.Values.GetEnumerator ();
		}

		#endregion
	}
}
