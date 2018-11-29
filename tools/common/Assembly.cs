using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Mono.Cecil;
using MonoTouch.Tuner;
using ObjCRuntime;
using Xamarin;
using Xamarin.Utils;

#if MONOTOUCH
using PlatformException = Xamarin.Bundler.MonoTouchException;
#else
using PlatformException = Xamarin.Bundler.MonoMacException;
#endif


namespace Xamarin.Bundler {

	struct NativeReferenceMetadata
	{
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

	public partial class Assembly
	{
		public List<string> Satellites;
		public Application App { get { return Target.App; } }

		string full_path;
		bool? is_framework_assembly;

		public AssemblyDefinition AssemblyDefinition;
		public Target Target;
		public bool IsFrameworkAssembly { get { return is_framework_assembly.Value; } }
		public string FullPath {
			get {
				return full_path;
			}
			set {
				full_path = value;
				if (!is_framework_assembly.HasValue) {
					var real_full_path = Target.GetRealPath (full_path);
					is_framework_assembly = real_full_path.StartsWith (Path.GetDirectoryName (Path.GetDirectoryName (Target.Resolver.FrameworkDirectory)), StringComparison.Ordinal);
				}
			}
		}
		public string FileName { get { return Path.GetFileName (FullPath); } }
		public string Identity { get { return GetIdentity (FullPath); } }

		public static string GetIdentity (AssemblyDefinition ad)
		{
			return Path.GetFileNameWithoutExtension (ad.MainModule.FileName);
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
			}
			catch {
				// do not let stale file crash us
				Driver.Log (3, "Invalid debugging symbols for {0} ignored", FullPath);
			}
		}

		void AddResourceToBeRemoved (string resource)
		{
			if (link_with_resources == null)
				link_with_resources = new List<string> ();
			link_with_resources.Add (resource);
		}

		public void ExtractNativeLinkInfo ()
		{
			// ignore framework assemblies, they won't have any LinkWith attributes
			if (IsFrameworkAssembly)
				return;

			var assembly = AssemblyDefinition;
			if (!assembly.HasCustomAttributes)
				return;

			string resourceBundlePath = Path.ChangeExtension (FullPath, ".resources");
			string manifestPath = Path.Combine (resourceBundlePath, "manifest");
			bool hasResourceBundle = Directory.Exists (resourceBundlePath) && File.Exists (manifestPath);
			if (hasResourceBundle) {
				foreach (NativeReferenceMetadata metadata in ReadManifest (manifestPath)) {
					ProcessNativeReferenceOptions (metadata);

					if (metadata.LibraryName.EndsWith (".framework", StringComparison.Ordinal)) {
						AssertiOSVersionSupportsUserFrameworks (metadata.LibraryName);
						Frameworks.Add (metadata.LibraryName);
					} else {
						LinkWith.Add (metadata.LibraryName);
					}
				}
			}

			ProcessLinkWithAttributes (assembly);

			// Make sure there are no duplicates between frameworks and weak frameworks.
			// Keep the weak ones.
			if (Frameworks != null && WeakFrameworks != null)
				Frameworks.ExceptWith (WeakFrameworks);

			if (NeedsGccExceptionHandling) {
				if (LinkerFlags == null)
					LinkerFlags = new List<string> ();
				LinkerFlags.Add ("-lgcc_eh");
			}

		}

		IEnumerable <NativeReferenceMetadata> ReadManifest (string manifestPath)
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

				if (attr.Constructor == null)
					continue;

				TypeReference type = attr.Constructor.DeclaringType;
				if (!type.IsPlatformType ("ObjCRuntime", "LinkWithAttribute"))
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
					if (linkWith.LibraryName.EndsWith (".framework", StringComparison.Ordinal)) {
						AssertiOSVersionSupportsUserFrameworks (linkWith.LibraryName);

						Frameworks.Add (ExtractFramework (assembly, metadata));
					} else {
						LinkWith.Add (ExtractNativeLibrary (assembly, metadata));
					}
				}
			}
		}

		void AssertiOSVersionSupportsUserFrameworks (string path)
		{
#if MONOTOUCH
			if (App.Platform == Xamarin.Utils.ApplePlatform.iOS && App.DeploymentTarget.Major < 8) {
				throw ErrorHelper.CreateError (1305, "The binding library '{0}' contains a user framework ({0}), but embedded user frameworks require iOS 8.0 (the deployment target is {1}). Please set the deployment target in the Info.plist file to at least 8.0.",
					FileName, Path.GetFileName (path), App.DeploymentTarget);
			}
#endif
		}

		void ProcessNativeReferenceOptions (NativeReferenceMetadata metadata)
		{
			// We can't add -dead_strip if there are any LinkWith attributes where smart linking is disabled.
			if (!metadata.SmartLink)
				App.DeadStrip = false;

			// Don't add -force_load if the binding's SmartLink value is set and the static registrar is being used.
			if (metadata.ForceLoad && !(metadata.SmartLink && App.Registrar == RegistrarMode.Static))
				ForceLoad = true;

			if (!string.IsNullOrEmpty (metadata.LinkerFlags)) {
				if (LinkerFlags == null)
					LinkerFlags = new List<string> ();
				LinkerFlags.Add (metadata.LinkerFlags);
			}

			if (!string.IsNullOrEmpty (metadata.Frameworks)) {
				foreach (var f in metadata.Frameworks.Split (new char [] { ' ' })) {
					if (Frameworks == null)
						Frameworks = new HashSet<string> ();
					Frameworks.Add (f);
				}
			}

			if (!string.IsNullOrEmpty (metadata.WeakFrameworks)) {
				foreach (var f in metadata.WeakFrameworks.Split (new char [] { ' ' })) {
					if (WeakFrameworks == null)
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

		string ExtractNativeLibrary (AssemblyDefinition assembly, NativeReferenceMetadata metadata)
		{
			string path = Path.Combine (App.Cache.Location, metadata.LibraryName);

			if (!Application.IsUptodate (FullPath, path)) {
				Application.ExtractResource (assembly.MainModule, metadata.LibraryName, path, false);
				Driver.Log (3, "Extracted third-party binding '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, path);
				LogLinkWithAttribute (metadata.Attribute);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (path))
				ErrorHelper.Warning (1302, "Could not extract the native library '{0}' from '{1}'. " +
				"Please ensure the native library was properly embedded in the managed assembly " +
				"(if the assembly was built using a binding project, the native library must be included in the project, and its Build Action must be 'ObjcBindingNativeLibrary').",
					metadata.LibraryName, path);

			return path;
		}

		string ExtractFramework (AssemblyDefinition assembly, NativeReferenceMetadata metadata)
		{
			string path = Path.Combine (App.Cache.Location, metadata.LibraryName);

			var zipPath = path + ".zip";
			if (!Application.IsUptodate (FullPath, zipPath)) {
				Application.ExtractResource (assembly.MainModule, metadata.LibraryName, zipPath, false);
				Driver.Log (3, "Extracted third-party framework '{0}' from '{1}' to '{2}'", metadata.LibraryName, FullPath, zipPath);
				LogLinkWithAttribute (metadata.Attribute);
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", path);
			}

			if (!File.Exists (zipPath)) {
				ErrorHelper.Warning (1302, "Could not extract the native framework '{0}' from '{1}'. " +
					"Please ensure the native framework was properly embedded in the managed assembly " +
					"(if the assembly was built using a binding project, the native framework must be included in the project, and its Build Action must be 'ObjcBindingNativeFramework').",
					metadata.LibraryName, zipPath);
			} else {
				if (!Directory.Exists (path))
					Directory.CreateDirectory (path);

				if (Driver.RunCommand ("/usr/bin/unzip", string.Format ("-u -o -d {0} {1}", StringUtils.Quote (path), StringUtils.Quote (zipPath))) != 0)
					throw ErrorHelper.CreateError (1303, "Could not decompress the native framework '{0}' from '{1}'. Please review the build log for more information from the native 'unzip' command.", metadata.LibraryName, zipPath);
			}

			return path;
		}

		static void LogLinkWithAttribute (LinkWithAttribute linkWith)
		{
			Driver.Log (3, "    ForceLoad: {0}", linkWith.ForceLoad);
			Driver.Log (3, "    Frameworks: {0}", linkWith.Frameworks);
			Driver.Log (3, "    IsCxx: {0}", linkWith.IsCxx);
			Driver.Log (3, "    LinkerFlags: {0}", linkWith.LinkerFlags);
			Driver.Log (3, "    LinkTarget: {0}", linkWith.LinkTarget);
			Driver.Log (3, "    NeedsGccExceptionHandling: {0}", linkWith.NeedsGccExceptionHandling);
			Driver.Log (3, "    SmartLink: {0}", linkWith.SmartLink);
			Driver.Log (3, "    WeakFrameworks: {0}", linkWith.WeakFrameworks);
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
			if (Driver.GetFrameworks (App).TryGetValue (file, out var framework) && framework.Version > App.SdkVersion)
				ErrorHelper.Warning (135, "Did not link system framework '{0}' (referenced by assembly '{1}') because it was introduced in {2} {3}, and we're using the {2} {4} SDK.", file, FileName, App.PlatformName, framework.Version, App.SdkVersion);
			else if (Frameworks.Add (file))
				Driver.Log (3, "Linking with the framework {0} because it's referenced by a module reference in {1}", file, FileName);
		}

		public string GetCompressionLinkingFlag ()
		{
			switch(App.Platform) {
			case ApplePlatform.MacOSX:
				if (App.DeploymentTarget >= new Version (10, 11, 0))
					return "-lcompression";
				return "-weak-lcompression";
			case ApplePlatform.iOS:
				if (App.DeploymentTarget >= new Version (9,0))
					return "-lcompression";
				return "-weak-lcompression";
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				return "-lcompression";
			default:
				throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in {1}; please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new with a test case.", App.Platform, App.SdkVersion);
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

#if !MONOMAC
					if (App.IsSimulatorBuild && !Driver.IsFrameworkAvailableInSimulator (App, file)) {
						Driver.Log (3, "Not linking with {0} (referenced by a module reference in {1}) because it's not available in the simulator.", file, FileName);
						continue;
					}
#endif

					switch (file) {
					// special case
					case "__Internal":
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
					default:
#if MONOMAC
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
#endif

						// detect frameworks
						int f = name.IndexOf (".framework/", StringComparison.Ordinal);
						if (f > 0) {
							AddFramework (file);
						} else {
							if (UnresolvedModuleReferences == null)
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
			if (Satellites != null) {
				foreach (var satellite in Satellites)
					yield return satellite;
			}
		}

		public void ComputeSatellites ()
		{
			var path = Path.GetDirectoryName (FullPath);
			var satellite_name = Path.GetFileNameWithoutExtension (FullPath) + ".resources.dll";

			foreach (var subdir in Directory.GetDirectories (path)) {
				var culture_name = Path.GetFileName (subdir);
				CultureInfo ci;

				if (culture_name.IndexOf ('.') >= 0)
					continue; // cultures can't have dots. This way we don't check every *.app directory

				try {
					ci = CultureInfo.GetCultureInfo (culture_name);
				} catch {
					// nope, not a resource language
					continue;
				}

				if (ci == null)
					continue;

				var satellite = Path.Combine (subdir, satellite_name);
				if (File.Exists (satellite)) {
					if (Satellites == null)
						Satellites = new List<string> ();
					Satellites.Add (satellite);
				}
			}
		}

		public void CopySatellitesToDirectory (string directory)
		{
			if (Satellites == null)
				return;

			foreach (var a in Satellites) {
				string target_dir = Path.Combine (directory, Path.GetFileName (Path.GetDirectoryName (a)));
				string target_s = Path.Combine (target_dir, Path.GetFileName (a));

				if (!Directory.Exists (target_dir))
					Directory.CreateDirectory (target_dir);

				CopyAssembly (a, target_s);
			}
		}
	}

	public sealed class NormalizedStringComparer : IEqualityComparer<string>
	{
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
			if (x != null)
				x = x.Normalize (System.Text.NormalizationForm.FormD);
			if (y != null)
				y = y.Normalize (System.Text.NormalizationForm.FormD);
			return comparer.Equals (x, y);
		}

		public int GetHashCode (string obj)
		{
			return comparer.GetHashCode (obj?.Normalize (System.Text.NormalizationForm.FormD));
		}
	}

	public class AssemblyCollection : IEnumerable<Assembly>
	{
		Dictionary<string, Assembly> HashedAssemblies = new Dictionary<string, Assembly> (NormalizedStringComparer.OrdinalIgnoreCase);

		public void Add (Assembly assembly)
		{
			Assembly other;
			if (HashedAssemblies.TryGetValue (assembly.Identity, out other))
				throw ErrorHelper.CreateError (2018, "The assembly '{0}' is referenced from two different locations: '{1}' and '{2}'.", assembly.Identity, other.FullPath, assembly.FullPath);
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
