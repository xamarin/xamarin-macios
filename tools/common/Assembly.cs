using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Mono.Cecil;
using Mono.Tuner;
using ObjCRuntime;
using Xamarin;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Bundler {

	struct NativeReferenceMetadata {
		public bool ForceLoad;
		public string? Frameworks;
		public string? WeakFrameworks;
		public string? LibraryName;
		public string? LinkerFlags;
		public LinkTarget LinkTarget;
		public bool NeedsGccExceptionHandling;
		public bool IsCxx;
		public bool LinkWithSwiftSystemLibraries;
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
			LinkWithSwiftSystemLibraries = attribute.LinkWithSwiftSystemLibraries;
			SmartLink = attribute.SmartLink;
			Dlsym = attribute.Dlsym;
			Attribute = attribute;
		}
	}

	public partial class Assembly {
		public Application App;

		string full_path;
		bool? is_framework_assembly;

		public AssemblyDefinition AssemblyDefinition;
		public bool? IsFrameworkAssembly { get { return is_framework_assembly; } }

		public string FullPath {
			get {
				return full_path;
			}
			[MemberNotNull (nameof (full_path))]
			set {
				full_path = value;
				if (!is_framework_assembly.HasValue && !string.IsNullOrEmpty (full_path)) {
#if !LEGACY_TOOLS
					is_framework_assembly = App.Configuration.FrameworkAssemblies.Contains (GetIdentity (full_path));
#else
					var real_full_path = Application.GetRealPath (App, full_path);
					is_framework_assembly = real_full_path.StartsWith (Path.GetDirectoryName (Path.GetDirectoryName (App.Resolver.FrameworkDirectory))!, StringComparison.Ordinal);
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
		public bool LinkWithSwiftSystemLibraries;
		public bool NeedsGccExceptionHandling;
		public bool ForceLoad;
		public HashSet<string> Frameworks = new HashSet<string> ();
		public HashSet<string> WeakFrameworks = new HashSet<string> ();
		public List<string> LinkerFlags = new List<string> (); // list of extra linker flags
		public List<string> LinkWith = new List<string> (); // list of paths to native libraries to link with, from LinkWith attributes
		public HashSet<ModuleReference>? UnresolvedModuleReferences;
		public bool HasLinkWithAttributes { get; private set; }

		bool? symbols_loaded;

		List<string>? link_with_resources; // a list of resources that must be removed from the app

		public Assembly (Application app, AssemblyDefinition definition)
		{
			this.App = app;
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
				if (File.Exists (pdb) && !string.IsNullOrEmpty (AssemblyDefinition.MainModule.FileName)) {
					AssemblyDefinition.MainModule.ReadSymbols ();
					symbols_loaded = true;
				}
			} catch {
				// do not let stale file crash us
				App.Log (3, "Invalid debugging symbols for {0} ignored", FullPath);
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
			if (Directory.Exists (resourceBundlePath)) {
				App.Log (3, $"Found a binding resource package for the assembly '{FullPath}' in {resourceBundlePath}, so not looking for any libraries embedded in the assembly.");
				return;
			}
			var zipPath = resourceBundlePath + ".zip";
			if (File.Exists (zipPath)) {
				App.Log (3, $"Found a binding resource package for the assembly '{FullPath}' in {zipPath}, so not looking for any libraries embedded in the assembly.");
				return;
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

				var linkWith = GetLinkWithAttribute (attr);
				var metadata = new NativeReferenceMetadata (linkWith);

				// If we've already processed this native library, skip it
				if (LinkWith.Any (x => Path.GetFileName (x) == metadata.LibraryName) || Frameworks.Any (x => Path.GetFileName (x) == metadata.LibraryName))
					continue;

				// Remove the resource from the assembly at a later stage.
				if (!StringUtils.IsNullOrEmpty (metadata.LibraryName))
					AddResourceToBeRemoved (metadata.LibraryName);

				ProcessNativeReferenceOptions (metadata);

				if (!StringUtils.IsNullOrEmpty (linkWith.LibraryName)) {
					switch (Path.GetExtension (linkWith.LibraryName).ToLowerInvariant ()) {
					case ".framework": {
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

		void ProcessNativeReferenceOptions (NativeReferenceMetadata metadata)
		{
			// We can't add -dead_strip if there are any LinkWith attributes where smart linking is disabled.
			if (!metadata.SmartLink) {
				App.Log (3, $"The library '{metadata.LibraryName}', shipped with the assembly '{FullPath}', sets SmartLink=false, which will disable passing -dead_strip to the native linker (and make the app bigger).");
				App.DeadStrip = false;
			}

			// Don't add -force_load if the binding's SmartLink value is set and the static registrar is being used.
			if (metadata.ForceLoad && !(metadata.SmartLink && (App.Registrar == RegistrarMode.Static || App.Registrar == RegistrarMode.ManagedStatic || App.Registrar == RegistrarMode.TrimmableStatic)))
				ForceLoad = true;

			if (!StringUtils.IsNullOrEmpty (metadata.LinkerFlags)) {
				if (LinkerFlags is null)
					LinkerFlags = new List<string> ();
				if (!StringUtils.TryParseArguments (metadata.LinkerFlags, out var args, out var ex))
					throw ErrorHelper.CreateError (148, ex, Errors.MX0148, metadata.LinkerFlags, metadata.LibraryName, FileName, ex.Message);
				LinkerFlags.AddRange (args);
			}

			if (!StringUtils.IsNullOrEmpty (metadata.Frameworks)) {
				foreach (var f in metadata.Frameworks.Split (new char [] { ' ' })) {
					if (Frameworks is null)
						Frameworks = new HashSet<string> ();
					Frameworks.Add (f);
				}
			}

			if (!StringUtils.IsNullOrEmpty (metadata.WeakFrameworks)) {
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

			if (metadata.LinkWithSwiftSystemLibraries)
				LinkWithSwiftSystemLibraries = true;
		}

		bool TryExtractNativeLibrary (AssemblyDefinition assembly, NativeReferenceMetadata metadata, [NotNullWhen (true)] out string? library)
		{
			if (metadata.LibraryName is null || App.Cache is null) {
				library = null;
				return false;
			}
			var path = Path.Combine (App.Cache.GetLocation (App), metadata.LibraryName);

			library = null;

			if (!Application.IsUptodate (App, FullPath, path)) {
				if (!Application.ExtractResource (assembly.MainModule, metadata.LibraryName, path, false)) {
					ErrorHelper.Warning (App, 1308, Errors.MX1308 /* Could not extract the native library '{0}' from the assembly '{1}', because it doesn't contain the resource '{2}'. */, metadata.LibraryName, FullPath, metadata.LibraryName);
					return false;
				}
				App.Log (3, "Extracted third-party binding '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, path);
				LogNativeReference (metadata);
			} else {
				App.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (path))
				ErrorHelper.Warning (App, 1302, Errors.MT1302, metadata.LibraryName, path);

			library = path;
			return true;
		}

		bool TryExtractFramework (AssemblyDefinition assembly, NativeReferenceMetadata metadata, [NotNullWhen (true)] out string? framework)
		{
			if (metadata.LibraryName is null || App.Cache is null) {
				framework = null;
				return false;
			}
			var path = Path.Combine (App.Cache.GetLocation (App), metadata.LibraryName);

			var zipPath = path + ".zip";

			framework = null;

			if (!Application.IsUptodate (App, FullPath, zipPath)) {
				if (!Application.ExtractResource (assembly.MainModule, metadata.LibraryName, zipPath, false)) {
					ErrorHelper.Warning (App, 1307, Errors.MX1307 /* Could not extract the native framework '{0}' from the assembly '{1}', because it doesn't contain the resource '{2}'. */, metadata.LibraryName, FullPath, metadata.LibraryName);
					return false;
				}

				App.Log (3, "Extracted third-party framework '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, zipPath);
				LogNativeReference (metadata);
			} else {
				App.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (zipPath)) {
				ErrorHelper.Warning (App, 1302, Errors.MT1302, metadata.LibraryName, FullPath);
				if (assembly.MainModule.HasResources) {
					App.Log (3, $"The assembly {FullPath} has {assembly.MainModule.Resources.Count} resources:");
					foreach (var res in assembly.MainModule.Resources) {
						App.Log (3, $"    {res.ResourceType}: {res.Name}");
					}
				} else {
					App.Log (3, $"The assembly {FullPath} does not have any resources.");
				}
			} else {
				if (!Directory.Exists (path))
					Directory.CreateDirectory (path);

				if (Driver.RunCommand (App, "/usr/bin/unzip", "-u", "-o", "-d", path, zipPath) != 0)
					throw ErrorHelper.CreateError (1303, Errors.MT1303, metadata.LibraryName, zipPath);
			}

			framework = path;
			return true;
		}

		void LogNativeReference (NativeReferenceMetadata metadata)
		{
			App.Log (3, "    LibraryName: {0}", metadata.LibraryName);
			App.Log (3, "    From: {0}", metadata.Attribute is not null ? "LinkWith" : "Binding Manifest");
			App.Log (3, "    ForceLoad: {0}", metadata.ForceLoad);
			App.Log (3, "    Frameworks: {0}", metadata.Frameworks);
			App.Log (3, "    IsCxx: {0}", metadata.IsCxx);
			App.Log (3, "    LinkWithSwiftSystemLibraries: {0}", metadata.LinkWithSwiftSystemLibraries);
			App.Log (3, "    LinkerFlags: {0}", metadata.LinkerFlags);
			App.Log (3, "    LinkTarget: {0}", metadata.LinkTarget);
			App.Log (3, "    NeedsGccExceptionHandling: {0}", metadata.NeedsGccExceptionHandling);
			App.Log (3, "    SmartLink: {0}", metadata.SmartLink);
			App.Log (3, "    WeakFrameworks: {0}", metadata.WeakFrameworks);
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
				case "LinkWithSwiftSystemLibraries":
					linkWith.LinkWithSwiftSystemLibraries = (bool) property.Argument.Value;
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
				if (framework.IsFrameworkUnavailable (App)) {
					ErrorHelper.Warning (App, 182, Errors.MX0182 /* Not linking with the framework {0} (referenced by a module reference in {1}) because it's not available on the current platform ({2}). */, framework.Name, FileName, App.PlatformName);
					return;
				}

				if (framework.Version > App.SdkVersion) {
					ErrorHelper.Warning (App, 135, Errors.MX0135, file, FileName, App.PlatformName, framework.Version, App.SdkVersion);
					return;
				}
			}

			var strong = (framework is null) || (App.DeploymentTarget >= (App.IsSimulatorBuild ? framework.VersionAvailableInSimulator ?? framework.Version : framework.Version));
			if (strong) {
				if (Frameworks.Add (file))
					App.Log (3, "Linking with the framework {0} because it's referenced by a module reference in {1}", file, FileName);
			} else {
				if (WeakFrameworks.Add (file))
					App.Log (3, "Linking (weakly) with the framework {0} because it's referenced by a module reference in {1}", file, FileName);
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

					if (App.IsFrameworkUnavailable (file)) {
						App.Log (3, "Not linking with {0} (referenced by a module reference in {1}) because it's not available in the current SDK.", file, FileName);
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
						App.Log (3, "Linking with {0} because it's referenced by a module reference in {1}", file, FileName);
						break;
					case "libsqlite3":
						// remove lib prefix
						LinkerFlags.Add ("-l" + file.Substring (3));
						App.Log (3, "Linking with {0} because it's referenced by a module reference in {1}", file, FileName);
						break;
					case "libcompression":
						LinkerFlags.Add (GetCompressionLinkingFlag ());
						break;
					case "libGLES":
					case "libGLESv2":
						// special case for OpenGLES.framework
						if (Frameworks.Add ("OpenGLES"))
							App.Log (3, "Linking with the framework OpenGLES because {0} is referenced by a module reference in {1}", file, FileName);
						break;
					case "vImage":
					case "vecLib":
						// sub-frameworks
						if (Frameworks.Add ("Accelerate"))
							App.Log (3, "Linking with the framework Accelerate because {0} is referenced by a module reference in {1}", file, FileName);
						break;
					case "openal32":
						if (Frameworks.Add ("OpenAL"))
							App.Log (3, "Linking with the framework OpenAL because {0} is referenced by a module reference in {1}", file, FileName);
						break;
#if !LEGACY_TOOLS
					case "Carbon":
						if (App.Platform != ApplePlatform.MacOSX) {
							App.Log (3, $"Not linking with the framework {file} (referenced by a module reference in {FileName}) because it doesn't exist on the target platform.");
							break;
						}
						break;
#endif
					default:
						if (App.Platform == ApplePlatform.MacOSX) {
							var path = Path.GetDirectoryName (name);
							if (path?.StartsWith ("/System/Library/Frameworks", StringComparison.Ordinal) != true)
								continue;

							// CoreServices has multiple sub-frameworks that can be used by customer code
							if (path.StartsWith ("/System/Library/Frameworks/CoreServices.framework/", StringComparison.Ordinal)) {
								if (Frameworks.Add ("CoreServices"))
									App.Log (3, "Linking with the framework CoreServices because {0} is referenced by a module reference in {1}", file, FileName);
								break;
							}
							// ApplicationServices has multiple sub-frameworks that can be used by customer code
							if (path.StartsWith ("/System/Library/Frameworks/ApplicationServices.framework/", StringComparison.Ordinal)) {
								if (Frameworks.Add ("ApplicationServices"))
									App.Log (3, "Linking with the framework ApplicationServices because {0} is referenced by a module reference in {1}", file, FileName);
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
							App.Log (3, "Could not resolve the module reference {0} in {1}", file, FileName);
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

		public bool IsDedupAssembly { get; set; } = false;

		public bool IsInterpreted {
			get {
				return IsDedupAssembly ? false : App.IsInterpreted (Identity);
			}
		}

		public bool IsAOTCompiled {
			get {
				return IsDedupAssembly ? true : App.IsAOTCompiled (Identity);
			}
		}
	}

	public class AssemblyCollection : IEnumerable<Assembly> {
		Dictionary<string, Assembly> HashedAssemblies = new Dictionary<string, Assembly> (NormalizedStringComparer.OrdinalIgnoreCase);

		public void Add (Assembly assembly)
		{
			if (HashedAssemblies.TryGetValue (assembly.Identity, out var other))
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

		public bool TryGetValue (string identity, [NotNullWhen (true)] out Assembly? assembly)
		{
			return HashedAssemblies.TryGetValue (identity, out assembly);
		}

		public bool TryGetValue (AssemblyDefinition asm, [NotNullWhen (true)] out Assembly? assembly)
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

		public void Update (Application app, IEnumerable<AssemblyDefinition> assemblies)
		{
			// This function will remove any assemblies not in 'assemblies', and add any new assemblies.
			var current = new HashSet<string> (HashedAssemblies.Keys, HashedAssemblies.Comparer);
			foreach (var assembly in assemblies) {
				var identity = Assembly.GetIdentity (assembly);
				if (!current.Remove (identity)) {
					// new assembly
					var asm = new Assembly (app, assembly);
					Add (asm);
					app.Log (1, "The linker added the assembly '{0}' to '{1}' to satisfy a reference.", asm.Identity, app.Name);
				} else {
					this [identity].AssemblyDefinition = assembly;
				}
			}

			foreach (var removed in current) {
				app.Log (1, "The linker removed the assembly '{0}' from '{1}' since there is no more reference to it.", this [removed].Identity, app.Name);
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
