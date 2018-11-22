using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BCLTestImporter
{
	// Added for the workaround so that it does not make the code uglier
	public partial struct BCLTestProjectDefinition
	{
		static Dictionary<string, List<(string assembly, string hint)>> cachedAssemblyInfo =
			new Dictionary<string, List<(string assembly, string hint)>>
			{
				{"SystemNumericsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Numerics.dll"),
					(assembly:"MONOTOUCH_System.Numerics_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Numerics_test.dll"),
				}},
				{"SystemRuntimeSerializationTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"System.Runtime.Serialization", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Runtime.Serialization.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"System.ServiceModel", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.ServiceModel.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Xml.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"MONOTOUCH_System.Runtime.Serialization_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Runtime.Serialization_test.dll"),
				}},
				{"SystemXmlLinqTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Xml.dll"),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Xml.Linq.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"MONOTOUCH_System.Xml.Linq_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Xml.Linq_test.dll"),
				}},
				{"MonoSecurityTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"Mono.Security", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Mono.Security.dll"),
					(assembly:"MONOTOUCH_Mono.Security_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_Mono.Security_test.dll"),
				}},
				{"SystemComponentModelDataAnnotationTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System.ComponentModel.DataAnnotations", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.ComponentModel.DataAnnotations.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"MONOTOUCH_System.ComponentModel.DataAnnotations_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.ComponentModel.DataAnnotations_test.dll"),
				}},
				{"SystemJsonTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Json.dll"),
					(assembly:"MONOTOUCH_System.Json_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Json_test.dll"),
				}},
				{"MonoDataTdsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"Mono.Data.Tds", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Mono.Data.Tds.dll"),
					(assembly:"MONOTOUCH_Mono.Data.Tds_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_Mono.Data.Tds_test.dll"),
				}},
				{"MonoCSharpTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"Mono.CSharp", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Mono.CSharp.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"MONOTOUCH_Mono.CSharp_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_Mono.CSharp_test.dll"),
				}},
				{"SystemJsonMicrosoftTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System.Json.Microsoft", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Json.Microsoft.dll"),
					(assembly:"MONOTOUCH_System.Json.Microsoft_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Json.Microsoft_test.dll"),
				}},
				{"MonoParallelTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"Mono.Parallel", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Mono.Parallel.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"MONOTOUCH_Mono.Parallel_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_Mono.Parallel_test.dll"),
				}},
				{"MonoTaskletsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"Mono.Tasklets", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Mono.Tasklets.dll"),
					(assembly:"MONOTOUCH_Mono.Tasklets_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_Mono.Tasklets_test.dll"),
				}},
				{"SystemThreadingTasksDataflowTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"System.Threading.Tasks.Dataflow", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Threading.Tasks.Dataflow.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/monotouch/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll"),
				}},
				{"SystemJsonXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Json.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"MONOTOUCH_System.Json_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Json_xunit-test.dll"),
				}},
				{"SystemNumericsXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Numerics.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/monotouch/Microsoft.CSharp.dll"),
					(assembly:"MONOTOUCH_System.Numerics_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Numerics_xunit-test.dll"),
				}},
				{"SystemLinqXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Xml.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Xml.Linq.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Core.dll"),
					(assembly:"MONOTOUCH_System.Xml.Linq_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Xml.Linq_xunit-test.dll"),
				}},
				{"SystemRuntimeCompilerServicesUnsafeXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/monotouch/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Runtime.CompilerServices.Unsafe", hint:"{MONO_ROOT}mcs/class/lib/monotouch/System.Runtime.CompilerServices.Unsafe.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"MONOTOUCH_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/monotouch/tests/MONOTOUCH_System.Runtime.CompilerServices.Unsafe_xunit-test.dll"),
				}},
			};

		public List<(string assembly, string hintPath)> GetCachedAssemblyInclusionInformation (string monoRootPath,
			Platform platform)
		{
			if (!monoRootPath.EndsWith ("/"))
				monoRootPath += "/";
			var info = cachedAssemblyInfo[Name];
			// lets fix the path
			var fixedResult = new List<(string assembly, string hintPath)> ();
			foreach (var (assembly, hin) in info){
				fixedResult.Add ((assembly, hin.Replace ("{MONO_ROOT}", monoRootPath)));
			}

			return fixedResult;
		}
	}
}
