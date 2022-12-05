using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	public class Helper {

		static Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition> ();

		// make sure we load assemblies only once into memory
		public static AssemblyDefinition GetAssembly (string assembly, ReaderParameters? parameters = null, bool readSymbols = false)
		{
			Assert.That (assembly, Does.Exist, "Assembly existence");
			if (!cache.TryGetValue (assembly, out var ad)) {
				if (parameters == null) {
					var resolver = new DefaultAssemblyResolver ();
					resolver.AddSearchDirectory (GetBCLDirectory (assembly));
					parameters = new ReaderParameters () {
						AssemblyResolver = resolver,
						ReadSymbols = readSymbols,
					};
				}

				ad = AssemblyDefinition.ReadAssembly (assembly, parameters);
				cache.Add (assembly, ad);
			}
			return ad;
		}

		public static IEnumerable<MethodDefinition> FilterMethods (AssemblyDefinition assembly, Func<MethodDefinition, bool>? filter)
		{
			foreach (var module in assembly.Modules) {
				foreach (var type in module.Types) {
					foreach (var method in FilterMethods (type, filter))
						yield return method;
				}
			}
			yield break;
		}

		static IEnumerable<MethodDefinition> FilterMethods (TypeDefinition type, Func<MethodDefinition, bool>? filter)
		{
			if (type.HasMethods) {
				foreach (var method in type.Methods) {
					if ((filter == null) || filter (method))
						yield return method;
				}
			}
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes) {
					foreach (var method in FilterMethods (nested, filter))
						yield return method;
				}
			}
			yield break;
		}

		public static IEnumerable<PropertyDefinition> FilterProperties (AssemblyDefinition assembly, Func<PropertyDefinition, bool>? filter)
		{
			foreach (var module in assembly.Modules) {
				foreach (var type in module.Types) {
					foreach (var property in FilterProperties (type, filter))
						yield return property;
				}
			}
			yield break;
		}

		static IEnumerable<PropertyDefinition> FilterProperties (TypeDefinition type, Func<PropertyDefinition, bool>? filter)
		{
			if (type.HasProperties) {
				foreach (var property in type.Properties) {
					if ((filter is null) || filter (property))
						yield return property;
				}
			}
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes) {
					foreach (var property in FilterProperties (nested, filter))
						yield return property;
				}
			}
			yield break;
		}

		public static IEnumerable<TypeDefinition> FilterTypes (AssemblyDefinition assembly, Func<TypeDefinition, bool>? filter)
		{
			foreach (var module in assembly.Modules) {
				foreach (var type in module.Types) {
					if ((filter is null) || filter (type))
						yield return type;
				}
			}
			yield break;
		}

		public static IEnumerable<FieldDefinition> FilterFields (AssemblyDefinition assembly, Func<FieldDefinition, bool>? filter)
		{
			foreach (var module in assembly.Modules) {
				foreach (var type in module.Types) {
					foreach (var field in FilterFields (type, filter))
						yield return field;
				}
			}
			yield break;
		}

		static IEnumerable<FieldDefinition> FilterFields (TypeDefinition type, Func<FieldDefinition, bool>? filter)
		{
			if (type.HasFields) {
				foreach (var field in type.Fields) {
					if ((filter is null) || filter (field))
						yield return field;
				}
			}
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes) {
					foreach (var field in FilterFields (nested, filter))
						yield return field;
				}
			}
			yield break;
		}

		public static string GetBCLDirectory (string assembly)
		{
			var rv = string.Empty;
			var isDotNet = !assembly.Contains ("Library/Frameworks/Xamarin.iOS.framework") && !assembly.Contains ("Library/Frameworks/Xamarin.Mac.framework");

			switch (Configuration.GetPlatform (assembly, isDotNet)) {
			case ApplePlatform.iOS:
				rv = Path.GetDirectoryName (Configuration.XamarinIOSDll);
				break;
			case ApplePlatform.WatchOS:
				rv = Path.GetDirectoryName (Configuration.XamarinWatchOSDll);
				break;
			case ApplePlatform.TVOS:
				rv = Path.GetDirectoryName (Configuration.XamarinTVOSDll);
				break;
			case ApplePlatform.MacOSX:
				rv = Path.GetDirectoryName (assembly);
				break;
			case ApplePlatform.MacCatalyst:
				rv = Path.GetDirectoryName (Configuration.XamarinCatalystDll);
				break;
			default:
				throw new NotImplementedException (assembly);
			}

			return rv;
		}

		static IEnumerable<string> PlatformAssemblies {
			get {
				if (!Configuration.include_legacy_xamarin)
					yield break;

				if (Configuration.include_ios) {
					// we want to process 32/64 bits individually since their content can differ
					yield return Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "iOS", "Xamarin.iOS.dll");
					yield return Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "iOS", "Xamarin.iOS.dll");
				}

				if (Configuration.include_watchos) {
					// XamarinWatchOSDll is stripped of its IL
					yield return Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "watchOS", "Xamarin.WatchOS.dll");
				}

				if (Configuration.include_tvos) {
					// XamarinTVOSDll is stripped of it's IL
					yield return Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "tvOS", "Xamarin.TVOS.dll");
				}

				if (Configuration.include_mac) {
					yield return Configuration.XamarinMacMobileDll;
					yield return Configuration.XamarinMacFullDll;
				}
			}
		}

		static IList<AssemblyInfo>? platform_assembly_definitions;
		public static IEnumerable<AssemblyInfo> PlatformAssemblyDefinitions {
			get {
				if (platform_assembly_definitions is null) {
					platform_assembly_definitions = PlatformAssemblies
						.Select (v => new AssemblyInfo (v, GetAssembly (v, readSymbols: true), false))
						.ToArray ();
				}
				return platform_assembly_definitions;
			}
		}


		static IEnumerable<string> NetPlatformAssemblies => Configuration.GetRefLibraries ();

		static IList<AssemblyInfo>? net_platform_assembly_definitions;
		public static IEnumerable<AssemblyInfo> NetPlatformAssemblyDefinitions {
			get {
				if (net_platform_assembly_definitions is null) {
					net_platform_assembly_definitions = NetPlatformAssemblies
						.Select (v => new AssemblyInfo (v, GetAssembly (v, readSymbols: false), true))
						.ToArray ();
				}
				return net_platform_assembly_definitions;
			}
		}

		static IEnumerable<string> NetPlatformImplementationAssemblies => Configuration.GetBaseLibraryImplementations ();

		static IList<AssemblyInfo>? net_platform_assembly_implemnetation_assembly_definitions;
		public static IEnumerable<AssemblyInfo> NetPlatformImplementationAssemblyDefinitions {
			get {
				if (net_platform_assembly_implemnetation_assembly_definitions is null) {
					net_platform_assembly_implemnetation_assembly_definitions = NetPlatformImplementationAssemblies
						.Select (v => new AssemblyInfo (v, GetAssembly (v, readSymbols: true), true))
						.ToArray ();
				}
				return net_platform_assembly_implemnetation_assembly_definitions;
			}
		}

		public static IEnumerable<TestFixtureData> TaskAssemblies {
			get {
				if (Configuration.include_ios)
					yield return CreateTestFixtureDataFromPath (Path.Combine (Configuration.SdkRootXI, "lib", "msbuild", "iOS", "Xamarin.iOS.Tasks.dll"));
				if (Configuration.include_mac)
					yield return CreateTestFixtureDataFromPath (Path.Combine (Configuration.SdkRootXM, "lib", "msbuild", "Xamarin.Mac.Tasks.dll"));
			}
		}

		static TestFixtureData CreateTestFixtureDataFromPath (string path)
		{
			var rv = new TestFixtureData (path);
			rv.SetArgDisplayNames (Path.GetFileName (path));
			return rv;
		}
	}

	public static class CompatExtensions {
		// cecil-tests is not NET5 yet, this is required to foreach over a dictionary
		public static void Deconstruct<T1, T2> (this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
		{
			key = tuple.Key;
			value = tuple.Value;
		}
	}

	public class AssemblyInfo {
		public AssemblyDefinition Assembly;
		public string Path;
		public ApplePlatform Platform;

		public AssemblyInfo (string path, AssemblyDefinition assembly, bool isDotNet)
		{
			Assembly = assembly;
			Path = path;
			Platform = Configuration.GetPlatform (path, isDotNet);
		}

		public override string ToString ()
		{
			// The returned text will show up in VSMac's unit test pad
			return Path.Replace (Configuration.RootPath, string.Empty).TrimStart ('/');
		}
	}
}
