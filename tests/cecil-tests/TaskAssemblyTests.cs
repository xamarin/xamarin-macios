using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	[TestFixtureSource (typeof (Helper), "TaskAssemblies")]
	public class TaskAssemblyTests {
		string assembly;

		public TaskAssemblyTests (string assembly)
		{
			this.assembly = assembly;
		}

		bool IsTaskType (TypeDefinition? td)
		{
			if (td == null)
				return false;

			if (td.HasInterfaces) {
				foreach (var iface in td.Interfaces) {
					if (iface.InterfaceType.Namespace == "Microsoft.Build.Framework" && iface.InterfaceType.Name == "ITask")
						return true;
				}
			}

			return IsTaskType (td.BaseType?.Resolve ());
		}

		[Test]
		public void EnsureOnlyCodeInBaseTasks ()
		{
			var parameters = new ReaderParameters (ReadingMode.Deferred);
			var resolver = new DefaultAssemblyResolver ();
			resolver.AddSearchDirectory ("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/msbuild/Current/bin");
			parameters.AssemblyResolver = resolver;

			var asm = Helper.GetAssembly (assembly, parameters)!;
			var checking_types = new List<TypeDefinition> ();
			var should_be_abstract = new List<string> ();
			foreach (var type in asm.MainModule.Types.OrderBy (v => v.FullName)) {
				if (!IsTaskType (type)) {
					// Console.WriteLine ($"ℹ️ {type.FullName} does not implement ITask");
					continue;
				}

				if (type.IsAbstract)
					continue;

				// All the Base and Core classes should be abstract
				if (type.Name.EndsWith ("Base", StringComparison.Ordinal) || type.Name.EndsWith ("Core", StringComparison.Ordinal)) {
					should_be_abstract.Add (type.FullName);
					continue;
				}

				checking_types.Add (type);
			}

			var failures = new List<string> ();
			foreach (var type in checking_types) {
				// We don't care about default constructors
				var methods = type.Methods.Where (v => {
					if (v.IsConstructor && v.Parameters.Count == 0 && v.HasBody) {
						var skipNop = new Func<Instruction, Instruction?> (v => {
							if (v == null)
								return null;
							while (v.OpCode.Code == Code.Nop)
								v = v.Next;
							return v;
						});
						// There should be this sequence of instructions, otherwise the default constructor has user code, and shouldn't be ignored:
						var ins = skipNop (v.Body.Instructions [0]);
						if (ins?.OpCode.Code != Code.Ldarg_0)
							return true;
						ins = skipNop (ins.Next);
						if (ins?.OpCode.Code != Code.Call)
							return true;
						ins = skipNop (ins.Next);
						if (ins?.OpCode.Code != Code.Ret)
							return true;
						ins = skipNop (ins.Next);
						if (ins != null)
							return true;
						return false;
					}
					return true;
				});

				var failed = methods.Any () || type.HasProperties || type.HasFields;
				var known_failure = IsKnownFailure (type.FullName);

				if (!failed) {
					if (known_failure) {
						failures.Add ($"{type.FullName} is marked as a known failure when it didn't fail. Maybe the list of known failures need to be updated?");
						continue;
					}
					continue; // We successfully succeeded!
				}

				if (failed && known_failure) {
					Console.WriteLine ($"⚠️  {type.FullName} is known failure");
					continue;
				}

				// We failed, and this type is not a known failure
				var sb = new StringBuilder ();
				sb.AppendLine ($"{type.FullName} is not an empty type:");
				if (methods.Any ()) {
					sb.AppendLine ($"    It has {methods.Count ()} methods:");
					foreach (var method in methods)
						sb.AppendLine ($"        {method.FullName}");
				}
				if (type.HasProperties) {
					sb.AppendLine ($"    It has {type.Properties.Count} properties:");
					foreach (var property in type.Properties)
						sb.AppendLine ($"        {property.Name}");
				}
				if (type.HasProperties) {
					sb.AppendLine ($"    It has {type.Fields.Count} fields:");
					foreach (var field in type.Fields)
						sb.AppendLine ($"        {field.Name}");
				}
				failures.Add (sb.ToString ());
				Console.WriteLine ($"❌ {sb}");
			}

			Assert.That (failures, Is.Empty, "Types with code");
			Assert.That (checking_types.Count, Is.AtLeast (50), "Checked types"); // Make sure the initial type filtering doesn't filter away too much by mistake
			Assert.That (should_be_abstract, Is.Empty, "Classes that should be abstract");
		}

		bool IsKnownFailure (string typename)
		{
			switch (typename) {
			case "Xamarin.MacDev.Tasks.UnpackLibraryResources":
				return true;
			}

			switch (Path.GetFileNameWithoutExtension (assembly)) {
			case "Xamarin.iOS.Tasks":
				// No other known failures
				return false;
			case "Xamarin.Mac.Tasks":
				switch (typename) {
				case "Xamarin.Mac.Tasks.CodesignVerify":
				case "Xamarin.Mac.Tasks.IBTool":
				case "Xamarin.Mac.Tasks.Metal":
				case "Xamarin.Mac.Tasks.MetalLib":
					return true;
				}
				return false;
			default:
				throw new NotImplementedException ($"Unknown assembly: {assembly}");
			}
		}
	}
}

