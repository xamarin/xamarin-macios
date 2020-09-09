using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using Mono.Cecil;
using Mono.Linker;

using Xamarin.Bundler;
using Xamarin.Utils;

using ObjCRuntime;

namespace Xamarin.Linker {
	public class LinkerConfiguration {
		public List<Abi> Abis;
		// This is the AssemblyName MSBuild property for the main project (which is also the root/entry assembly)
		public string AssemblyName { get; private set; }
		public string ItemsDirectory { get; private set; }
		public ApplePlatform Platform { get; private set; }
		public string PlatformAssembly { get; private set; }
		public Version SdkVersion { get; private set; }
		public int Verbosity { get; private set; }

		static ConditionalWeakTable<LinkContext, LinkerConfiguration> configurations = new ConditionalWeakTable<LinkContext, LinkerConfiguration> ();

		public Application Application { get; private set; }
		public Target Target { get; private set; }

		public LinkContext Context { get; private set; }
		public DerivedLinkContext DerivedLinkContext { get; private set; }
		public Profile Profile { get; private set; }

		// The list of assemblies is populated in CollectAssembliesStep.
		public List<AssemblyDefinition> Assemblies = new List<AssemblyDefinition> ();
		
		string user_optimize_flags;

		public static LinkerConfiguration GetInstance (LinkContext context)
		{
			if (!configurations.TryGetValue (context, out var instance)) {
				if (!context.TryGetCustomData ("LinkerOptionsFile", out var linker_options_file))
					throw new Exception ($"No custom linker options file was passed to the linker (using --custom-data LinkerOptionsFile=...");
				instance = new LinkerConfiguration (linker_options_file) {
					Context = context,
				};

				configurations.Add (context, instance);
			}

			return instance;
		}

		LinkerConfiguration (string linker_file)
		{
			if (!File.Exists (linker_file))
				throw new FileNotFoundException ($"The custom linker file {linker_file} does not exist.");

			var lines = File.ReadAllLines (linker_file);
			var significantLines = new List<string> (); // This is the input the cache uses to verify if the cache is still valid
			for (var i = 0; i < lines.Length; i++) {
				var line = lines [i].TrimStart ();
				if (line.Length == 0 || line [0] == '#')
					continue; // Allow comments

				var eq = line.IndexOf ('=');
				if (eq == -1)
					throw new InvalidOperationException ($"Invalid syntax for line {i + 1} in {linker_file}: No equals sign.");

				significantLines.Add (line);

				var key = line [..eq];
				var value = line [(eq + 1)..];

				if (string.IsNullOrEmpty (value))
					continue;

				switch (key) {
				case "AssemblyName":
					AssemblyName = value;
					break;
				case "CacheDirectory":
					CacheDirectory = value;
					break;
				case "ItemsDirectory":
					ItemsDirectory = value;
					break;
				case "IsSimulatorBuild":
					IsSimulatorBuild = string.Equals ("true", value, StringComparison.OrdinalIgnoreCase);
					break;
				case "LinkMode":
					if (!Enum.TryParse<LinkMode> (value, true, out var lm))
						throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
					Application.LinkMode = lm;
					break;
				case "MarshalManagedExceptionMode":
					if (!string.IsNullOrEmpty (value)) {
						if (!Application.TryParseManagedExceptionMode (value, out var mode))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						Application.MarshalManagedExceptions = mode;
					}
					break;
				case "MarshalObjectiveCExceptionMode":
					if (!string.IsNullOrEmpty (value)) {
						if (!Application.TryParseObjectiveCExceptionMode (value, out var mode))
							throw new InvalidOperationException ($"Unable to parse the {key} value: {value} in {linker_file}");
						Application.MarshalObjectiveCExceptions = mode;
					}
					break;
				case "Optimize":
					user_optimize_flags = value;
					break;
				case "PartialStaticRegistrarLibrary":
					PartialStaticRegistrarLibrary = value;
					break;
				case "Platform":
					switch (value) {
					case "iOS":
						Platform = ApplePlatform.iOS;
						break;
					case "tvOS":
						Platform = ApplePlatform.TVOS;
						break;
					case "watchOS":
						Platform = ApplePlatform.WatchOS;
						break;
					case "macOS":
						Platform = ApplePlatform.MacOSX;
						break;
					default:
						throw new InvalidOperationException ($"Unknown platform: {value} for the entry {line} in {linker_file}");
					}
					break;
				case "PlatformAssembly":
					PlatformAssembly = Path.GetFileNameWithoutExtension (value);
					break;
				case "TargetArchitectures":
					if (!Enum.TryParse<Abi> (value, out var arch))
						throw new InvalidOperationException ($"Unknown target architectures: {value} in {linker_file}");
					// Add to the list of Abis as separate entries (instead of a flags enum value), because that way it's easier to enumerate over them.
					Abis = new List<Abi> ();
					for (var b = 0; b < 32; b++) {
						var a = (Abi) (1 << b);
						if ((a & arch) == a)
							Abis.Add (a);
					}
					break;
				case "TargetFramework":
					if (!TargetFramework.TryParse (value, out var tf))
						throw new InvalidOperationException ($"Invalid TargetFramework '{value}' in {linker_file}");
					Driver.TargetFramework = TargetFramework.Parse (value);
					break;
				case "Verbosity":
					if (!int.TryParse (value, out var verbosity))
						throw new InvalidOperationException ($"Invalid Verbosity '{value}' in {linker_file}");
					Verbosity = verbosity;
					break;
				default:
					throw new InvalidOperationException ($"Unknown key '{key}' in {linker_file}");
				}
			}

			ErrorHelper.Platform = Platform;

			// Optimizations.Parse can only be called after setting ErrorHelper.Platform
			if (!string.IsNullOrEmpty (user_optimize_flags)) {
				var messages = new List<ProductException> ();
				Application.Optimizations.Parse (Application.Platform, user_optimize_flags, messages);
				ErrorHelper.Show (messages);
			}

			Application.CreateCache (significantLines.ToArray ());
			Application.Cache.Location = CacheDirectory;
			Application.DeploymentTarget = DeploymentTarget;
			Application.SdkVersion = SdkVersion;

			switch (Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
			case ApplePlatform.WatchOS:
				Application.BuildTarget = IsSimulatorBuild ? BuildTarget.Simulator : BuildTarget.Device;
				break;
			case ApplePlatform.MacOSX:
			default:
				break;
			}

			if (Driver.TargetFramework.Platform != Platform)
				throw ErrorHelper.CreateError (99, "Inconsistent platforms. TargetFramework={0}, Platform={1}", Driver.TargetFramework.Platform, Platform);

			Application.InitializeCommon ();
		}

		public void Write ()
		{
			if (Verbosity > 0) {
				Console.WriteLine ($"LinkerConfiguration:");
				Console.WriteLine ($"    ABIs: {string.Join (", ", Abis.Select (v => v.AsArchString ()))}");
				Console.WriteLine ($"    AssemblyName: {AssemblyName}");
				Console.WriteLine ($"    CacheDirectory: {CacheDirectory}");
				Console.WriteLine ($"    DeploymentTarget: {DeploymentTarget}");
				Console.WriteLine ($"    ItemsDirectory: {ItemsDirectory}");
				Console.WriteLine ($"    IsSimulatorBuild: {IsSimulatorBuild}");
				Console.WriteLine ($"    LinkMode: {LinkMode}");
				Console.WriteLine ($"    MarshalManagedExceptions: {Application.MarshalManagedExceptions} (IsDefault: {Application.IsDefaultMarshalManagedExceptionMode})");
				Console.WriteLine ($"    MarshalObjectiveCExceptions: {Application.MarshalObjectiveCExceptions}");
				Console.WriteLine ($"    Optimize: {user_optimize_flags} => {Application.Optimizations}");
				Console.WriteLine ($"    PartialStaticRegistrarLibrary: {PartialStaticRegistrarLibrary}");
				Console.WriteLine ($"    Platform: {Platform}");
				Console.WriteLine ($"    PlatformAssembly: {PlatformAssembly}.dll");
				Console.WriteLine ($"    SdkVersion: {SdkVersion}");
				Console.WriteLine ($"    Verbosity: {Verbosity}");
			}
		}

		public string GetAssemblyFileName (AssemblyDefinition assembly)
		{
			// See: https://github.com/mono/linker/issues/1313
			// Call LinkContext.Resolver.GetAssemblyFileName (https://github.com/mono/linker/blob/da2cc0fcd6c3a8e8e5d1b5d4a655f3653baa8980/src/linker/Linker/AssemblyResolver.cs#L88) using reflection.
			var resolver = typeof (LinkContext).GetProperty ("Resolver").GetValue (Context);
			var filename = (string) resolver.GetType ().GetMethod ("GetAssemblyFileName", new Type [] { typeof (AssemblyDefinition) }).Invoke (resolver, new object [] { assembly });
			return filename;
		}

		public void WriteOutputForMSBuild (string itemName, List<MSBuildItem> items)
		{
			var xmlNs = XNamespace.Get ("http://schemas.microsoft.com/developer/msbuild/2003");
			var elements = items.Select (item =>
				new XElement (xmlNs + itemName,
					new XAttribute ("Include", item.Include),
						item.Metadata.Select (metadata => new XElement (xmlNs + metadata.Key, metadata.Value))));

			var document = new XDocument (
				new XElement (xmlNs + "Project",
					new XElement (xmlNs + "ItemGroup",
						elements)));

			document.Save (Path.Combine (ItemsDirectory, itemName + ".items"));
		}
	}
}

public class MSBuildItem {
	public string Include;
	public Dictionary<string, string> Metadata = new Dictionary<string, string> ();
}
