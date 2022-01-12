using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Tests;

#nullable enable

namespace Cecil.Tests {

	public class Helper {

		static Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition> ();

		// make sure we load assemblies only once into memory
		public static AssemblyDefinition? GetAssembly (string assembly, ReaderParameters? parameters = null)
		{
			if (!File.Exists (assembly))
				return null;
			if (!cache.TryGetValue (assembly, out var ad)) {
				if (parameters == null) {
					var resolver = new DefaultAssemblyResolver ();
					resolver.AddSearchDirectory (GetBCLDirectory (assembly));
					parameters = new ReaderParameters () {
						AssemblyResolver = resolver,
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

		public static string GetBCLDirectory (string assembly)
		{
			var rv = string.Empty;

			switch (Path.GetFileName (assembly)) {
			case "Xamarin.iOS.dll":
				rv = Path.GetDirectoryName (Configuration.XamarinIOSDll);
				break;
			case "Xamarin.WatchOS.dll":
				rv = Path.GetDirectoryName (Configuration.XamarinWatchOSDll);
				break;
			case "Xamarin.TVOS.dll":
				rv = Path.GetDirectoryName (Configuration.XamarinTVOSDll);
				break;
			case "Xamarin.Mac.dll":
				rv = Path.GetDirectoryName (assembly);
				break;
			case "Xamarin.MacCatalyst.dll":
				rv = Path.GetDirectoryName (Configuration.XamarinCatalystDll);
				break;
			default:
				throw new NotImplementedException (assembly);
			}

			return rv;
		}

		public static IEnumerable PlatformAssemblies {
			get {
				// we want to process 32/64 bits individually since their content can differ
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "iOS", "Xamarin.iOS.dll"));
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "iOS", "Xamarin.iOS.dll"));

				// XamarinWatchOSDll is stripped of its IL
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "watchOS", "Xamarin.WatchOS.dll"));
				// XamarinTVOSDll is stripped of it's IL
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "tvOS", "Xamarin.TVOS.dll"));

				yield return new TestCaseData (Configuration.XamarinMacMobileDll);
				yield return new TestCaseData (Configuration.XamarinMacFullDll);
			}
		}

		public static IEnumerable NetPlatformAssemblies => Configuration.GetRefLibraries ();

		public static IEnumerable TaskAssemblies {
			get {
				yield return CreateTestFixtureDataFromPath (Path.Combine (Configuration.SdkRootXI, "lib", "msbuild", "iOS", "Xamarin.iOS.Tasks.dll"));
				yield return CreateTestFixtureDataFromPath (Path.Combine (Configuration.SdkRootXM, "lib", "msbuild",  "Xamarin.Mac.Tasks.dll"));
			}
		}

		static TestFixtureData CreateTestFixtureDataFromPath (string path)
		{
			var rv = new TestFixtureData (path);
			rv.SetArgDisplayNames (Path.GetFileName (path));
			return rv;
		}
	}
}
