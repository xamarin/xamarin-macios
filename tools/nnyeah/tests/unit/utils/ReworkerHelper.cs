using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin;
using Xamarin.Utils;
using System.Collections.Generic;
using Xamarin.Tests;
using Mono.Cecil;

namespace Microsoft.MaciOS.Nnyeah.Tests {
	public static class ReworkerHelper {
		public static ReaderParameters ReaderParameters {
			get {
				var legacyPlatform = Compiler.XamarinPlatformLibraryPath (PlatformName.macOS);
				var netPlatform = Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS);

				// We must use a resolver here as the types will be Resolved()'ed later
				return new ReaderParameters { AssemblyResolver = new NNyeahAssemblyResolver (legacyPlatform, netPlatform) };
			}
		}

		public static async Task<TypeDefinition> CompileTypeForTest (string code, string typeName = "Foo")
		{
			string lib = await TestRunning.BuildTemporaryLibrary (code);
			var module = ModuleDefinition.ReadModule (lib, ReaderParameters);
			return module.GetType (typeName);
		}

		public static Reworker? CreateReworker (string modulePath)
		{
			var readerParameters = ReaderParameters;
			var resolver = (readerParameters.AssemblyResolver as NNyeahAssemblyResolver)!;
			var stm = new FileStream (modulePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var moduleToEdit = ModuleDefinition.ReadModule (stm, readerParameters);
			var typeAndModuleMap = new TypeAndModuleMap (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
				Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), resolver);

			var moduleContainer = new ModuleContainer (moduleToEdit, typeAndModuleMap.XamarinModule,
				typeAndModuleMap.MicrosoftModule);

			return Reworker.CreateReworker (stm, moduleContainer, typeAndModuleMap.TypeMap);
		}

		public static ModuleDefinition? GetReworkedModule (string inModule)
		{
			var reworker = CreateReworker (inModule);
			if (reworker is null)
				return null;

			var outStm = new MemoryStream ();
			reworker.Rework (outStm);
			outStm.Seek (0, SeekOrigin.Begin);
			return ModuleDefinition.ReadModule (outStm, ReaderParameters);
		}
	}
}

