using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace BCLTestImporter {
	public static class RegisterTypeGenerator {

		static string UsingReplacement = "%USING%";
		static string KeysReplacement = "%KEY VALUES%";
		static string IsxUnitReplacement = "%IS XUNIT%";
		
		// the following cache is a workaround until mono does provide the required binaries precompiled, at that point
		// we will remove the dict and we will use the refection based method.
		static Dictionary<string, (string testNamespace, string testAssembly, string testType)> cache = new Dictionary<string, (string testNamespace, string testAssembly, string testType)> {
			{"SystemNumericsTests", ("MonoTests.System.Numerics",  "MONOTOUCH_System.Numerics_test.dll", "MonoTests.System.Numerics.BigIntegerTest")},
			{"SystemRuntimeSerializationTests", ("MonoTests", "MONOTOUCH_System.Runtime.Serialization_test.dll", "MonoTests.XmlComparer")},
			{"SystemXmlLinqTests", ("MonoTests.System.Xml", "MONOTOUCH_System.Xml.Linq_test.dll", "MonoTests.System.Xml.ExtensionsTest")},
			{"MonoSecurityTests", ("MonoTests.System.Security.Cryptography", "MONOTOUCH_Mono.Security_test.dll", "MonoTests.System.Security.Cryptography.SHA224ManagedTest")},
			{"SystemComponentModelDataAnnotationTests", ("MonoTests.System.ComponentModel.DataAnnotations", "MONOTOUCH_System.ComponentModel.DataAnnotations_test.dll", "MonoTests.System.ComponentModel.DataAnnotations.AssociationAttributeTest")},
			{"SystemJsonTests", ("MonoTests.System", "MONOTOUCH_System.Json_test.dll", "MonoTests.System.JsonValueTests")},
			{"MonoDataTdsTests", ("MonoTests.Mono.Data.Tds", "MONOTOUCH_Mono.Data.Tds_test.dll", "MonoTests.Mono.Data.Tds.TdsConnectionPoolTest")},
			{"MonoCSharpTests", ("MonoTests.Visit", "MONOTOUCH_Mono.CSharp_test.dll", "MonoTests.Visit.ASTVisitorTest")},
			{"SystemJsonMicrosoftTests", ("MonoTests.System", "MONOTOUCH_System.Json.Microsoft_test.dll", "MonoTests.System.JsonValueTests")},
			{"MonoParallelTests", ("MonoTests.Mono.Threading", "MONOTOUCH_Mono.Parallel_test.dll", "MonoTests.Mono.Threading.SnziTests")},
			{"MonoTaskletsTests", ("MonoTests.System", "MONOTOUCH_Mono.Tasklets_test.dll", "MonoTests.System.ContinuationsTest")},
			{"SystemThreadingTasksDataflowTests", ("MonoTests", "MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll", "MonoTests.TestScheduler")},
			{"SystemJsonXunit", ("System.Json.Tests", "MONOTOUCH_System.Json_xunit-test.dll", "System.Json.Tests.JsonArrayTests")},
			{"SystemNumericsXunit", ("System.Numerics.Tests", "MONOTOUCH_System.Numerics_xunit-test.dll", "System.Numerics.Tests.GenericVectorTests")},
			{"SystemLinqXunit", ("Microsoft.Test.ModuleCore", "MONOTOUCH_System.Xml.Linq_xunit-test.dll", "Microsoft.Test.ModuleCore.LtmContext")},
			{"SystemRuntimeCompilerServicesUnsafeXunit", ("System.Runtime.CompilerServices", "MONOTOUCH_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", "System.Runtime.CompilerServices.UnsafeTests")},
		};

		public static async Task<string> GenerateCodeAsync (string projectName, bool isXunit, string templatePath)
		{
			var cachedData = cache[projectName];
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			importStringBuilder.AppendLine ($"using {cachedData.testNamespace};");
			keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{cachedData.testAssembly}\", typeof ({cachedData.testType})}}, ");
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (UsingReplacement, importStringBuilder.ToString ());
				result = result.Replace (KeysReplacement, keyValuesStringBuilder.ToString ());
				result = result.Replace (IsxUnitReplacement, (isXunit)? "true" : "false");
				return result;
			}
		}

		public static async Task<string> GenerateCodeAsync (Dictionary<string, Type> typeRegistration, bool isXunit,
			string templatePath)
		{
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			var namespaces = new List<string> ();  // keep track of the namespaces to remove warnings
			foreach (var a in typeRegistration.Keys) {
				var t = typeRegistration [a];
				if (!string.IsNullOrEmpty (t.Namespace)) {
					if (!namespaces.Contains (t.Namespace)) {
						namespaces.Add (t.Namespace);
						importStringBuilder.AppendLine ($"using {t.Namespace};");
					}
					keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{a}\", typeof ({t.FullName})}}, ");
				}
			}
			
			// got the lines we want to add, read the template and substitute
			using (var reader = new StreamReader(templatePath)) {
				var result = await reader.ReadToEndAsync ();
				result = result.Replace (UsingReplacement, importStringBuilder.ToString ());
				result = result.Replace (KeysReplacement, keyValuesStringBuilder.ToString ());
				result = result.Replace (IsxUnitReplacement, (isXunit)? "true" : "false");
				return result;
			}
		}
		
		// Generates the code for the type registration using the give path to the template to use
		public static string GenerateCode (Dictionary <string, Type> typeRegistration, bool isXunit, string templatePath) => GenerateCodeAsync (typeRegistration, isXunit, templatePath).Result;
	}
}
