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
					ad = AssemblyDefinition.ReadAssembly (assembly);
				} else {
					ad = AssemblyDefinition.ReadAssembly (assembly, parameters);
				}
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

		public static IEnumerable PlatformAssemblies {
			get {
				// we want to process 32/64 bits individually since their content can differ
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "Xamarin.iOS.dll"));
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "Xamarin.iOS.dll"));

				// XamarinWatchOSDll is stripped of its IL
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "32bits", "Xamarin.WatchOS.dll"));
				// XamarinTVOSDll is stripped of it's IL
				yield return new TestCaseData (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "64bits", "Xamarin.TVOS.dll"));

				yield return new TestCaseData (Configuration.XamarinMacMobileDll);
				yield return new TestCaseData (Configuration.XamarinMacFullDll);
			}
		}

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
