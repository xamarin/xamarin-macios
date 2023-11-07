using System;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using Mono.Cecil;
using Microsoft.MaciOS.Nnyeah;
using Xamarin;
using System.Linq;


namespace Microsoft.MaciOS.Nnyeah.Tests {
	[TestFixture]
	public class DependencyRemovedTests {
		[TestCase ("nint")]
		[TestCase ("nuint")]
		[TestCase ("nfloat")]
		public async Task BasicDependencyRemoved (string type)
		{
			var dir = Cache.CreateTemporaryDirectory ($"DependencyRemoved_{type}");
			var code = @$"
using System;
public class Foo {{
	public {type} Ident ({type} e) => e;
}}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");
			var targetRewrite = Path.Combine (dir, "NoNameRemoved.dll");

			Assert.DoesNotThrow (() => {
				AssemblyConverter.Convert (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
					Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), expectedOutputFile,
					targetRewrite, verbose: false, forceOverwrite: true, suppressWarnings: true);
			}, $"Failed to process assembly for type {type}");

			Assert.IsTrue (File.Exists (targetRewrite), $"target file not created for type {type}");
			var module = ModuleDefinition.ReadModule (targetRewrite);

			var platform = module.XamarinPlatformName ();
			Assert.AreEqual (Microsoft.MaciOS.Nnyeah.PlatformName.None, platform, "still has Xamarin dependency");
		}

		[TestCase]
		public async Task CorrectAttributes ()
		{
			var dir = Cache.CreateTemporaryDirectory ("CorrectAttributes");
			var code = @"
using System;
public class Foo {
	public nint Ident (nint e) => e;
}
";
			await TestRunning.BuildLibrary (code, "NoName", dir);
			var expectedOutputFile = Path.Combine (dir, "NoName.dll");
			var targetRewrite = Path.Combine (dir, "NoNameRemoved.dll");

			Assert.DoesNotThrow (() => {
				AssemblyConverter.Convert (Compiler.XamarinPlatformLibraryPath (PlatformName.macOS),
					Compiler.MicrosoftPlatformLibraryPath (PlatformName.macOS), expectedOutputFile,
					targetRewrite, verbose: false, forceOverwrite: true, suppressWarnings: true);
			}, $"Failed to process assembly for type nint");

			Assert.IsTrue (File.Exists (targetRewrite), $"target file not created for type nint");
			var module = ModuleDefinition.ReadModule (targetRewrite);

			var nativeIntAttribute = module.GetType ("System.Runtime.CompilerServices.NativeIntegerAttribute")!;

			Assert.IsTrue (ContainsAttribute (nativeIntAttribute, "System.Runtime.CompilerServices.CompilerGeneratedAttribute"), "No CompilerGenerateAttribute");
			Assert.IsTrue (ContainsAttribute (nativeIntAttribute, "Microsoft.CodeAnalysis.EmbeddedAttribute"), "No Microsoft.CodeAnalysis.EmbeddedAttribute");
			Assert.IsTrue (ContainsAttribute (nativeIntAttribute, "System.AttributeUsageAttribute"), "No System.AttributeUsageAttribute");
		}

		static bool ContainsAttribute (TypeDefinition theType, string attributeName)
		{
			return theType.CustomAttributes.Any (attr => attr.AttributeType.FullName == attributeName);
		}
	}
}
