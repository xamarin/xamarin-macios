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
		static Dictionary<string, List<(string assembly, string hint)>> iOSCachedAssemblyInfo =
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

		private static Dictionary<string, List<(string assembly, string hint)>> macCachedAssemblyInfo =
			new Dictionary<string, List<(string assembly, string hint)>> {
				{"MonoCSharp", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"Mono.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.CSharp.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_Mono.CSharp_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Mono.CSharp_test.dll"),
				}},
				{"MonoDataSqilte", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"Mono.Data.Sqlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Data.Sqlite.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_Mono.Data.Sqlite_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Mono.Data.Sqlite_test.dll"),
				}},
				{"MonoDataTds", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"Mono.Data.Tds", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Data.Tds.dll"),
					(assembly:"xammac_net_4_5_Mono.Data.Tds_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Mono.Data.Tds_test.dll"),
				}},
				{"MonoPoxis", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"Mono.Posix", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Posix.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_Mono.Posix_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Mono.Posix_test.dll"),
				}},
				{"MonoSecurtiy", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"Mono.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Security.dll"),
					(assembly:"xammac_net_4_5_Mono.Security_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Mono.Security_test.dll"),
				}},
				{"SystemComponentModelDataAnnotations", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.ComponentModel.DataAnnotations", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.ComponentModel.DataAnnotations.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll"),
				}},
				{"SystemConfiguration", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Configuration.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.Configuration_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Configuration_test.dll"),
				}},
				{"SystemCore", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_System.Core_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Core_test.dll"),
				}},
				{"SystemDataLinq", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Data.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.Linq.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Data.Linq_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Data.Linq_test.dll"),
				}},
				{"SystemData", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Configuration.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"Mono.Data.Sqlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Data.Sqlite.dll"),
					(assembly:"xammac_net_4_5_System.Data_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Data_test.dll"),
				}},
				{"SystemIOCompressionFileSystem", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.IO.Compression", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.IO.Compression.dll"),
					(assembly:"System.IO.Compression.FileSystem", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.IO.Compression.FileSystem.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_System.IO.Compression.FileSystem_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.IO.Compression.FileSystem_test.dll"),
				}},
				{"SystemIOCompression", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.IO.Compression", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.IO.Compression.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.IO.Compression_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.IO.Compression_test.dll"),
				}},
				{"SystemIdentityModel", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.IdentityModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.IdentityModel.dll"),
					(assembly:"System.Runtime.Serialization", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Runtime.Serialization.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.IdentityModel_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.IdentityModel_test.dll"),
				}},
				{"SystemJson", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Json.dll"),
					(assembly:"xammac_net_4_5_System.Json_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Json_test.dll"),
				}},
				{"SystemNetHttp", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Net.Http", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Net.Http.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Net.Http_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Net.Http_test.dll"),
				}},
				{"SystemNumerics", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Numerics.dll"),
					(assembly:"xammac_net_4_5_System.Numerics_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Numerics_test.dll"),
				}},
				{"SystemRuntimeSerializationFormattersSoap", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Runtime.Serialization.Formatters.Soap", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Runtime.Serialization.Formatters.Soap.dll"),
					(assembly:"xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll"),
				}},
				{"SystemSecurity", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Security.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.Security_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Security_test.dll"),
				}},
				{"SystemServiceModel", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.ServiceModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.ServiceModel.dll"),
					(assembly:"System.Runtime.Serialization", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Runtime.Serialization.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.IdentityModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.IdentityModel.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Security.dll"),
					(assembly:"xammac_net_4_5_System.ServiceModel_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.ServiceModel_test.dll"),
				}},
				{"SystemTransactions", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Transactions", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Transactions.dll"),
					(assembly:"xammac_net_4_5_System.Transactions_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Transactions_test.dll"),
				}},
				{"SystemXmlLinq", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.Linq.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xammac_net_4_5_System.Xml.Linq_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Xml.Linq_test.dll"),
				}},
				{"SystemXml", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"xammac_net_4_5_System.Xml_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Xml_test.dll"),
				}},
				{"System", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Configuration.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"Mono.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Mono.Security.dll"),
					(assembly:"xammac_net_4_5_System_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System_test.dll"),
				}},
				{"MicrosoftCSharpXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Microsoft.CSharp.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"xammac_net_4_5_Microsoft.CSharp_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_Microsoft.CSharp_xunit-test.dll"),
				}},
				{"SystemCoreXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Microsoft.CSharp.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"xunit.execution.dotnet", hint:""),
					(assembly:"xammac_net_4_5_System.Core_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Core_xunit-test.dll"),
				}},
				{"SystemDataXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Data.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"xammac_net_4_5_System.Data_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Data_xunit-test.dll"),
				}},
				{"SystemJsonXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Json.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Json_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Json_xunit-test.dll"),
				}},
				{"SystemNumericsXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Numerics.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/Microsoft.CSharp.dll"),
					(assembly:"xammac_net_4_5_System.Numerics_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Numerics_xunit-test.dll"),
				}},
				{"SystemRuntimeCompilerServicesXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Runtime.CompilerServices.Unsafe", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Runtime.CompilerServices.Unsafe.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll"),
				}},
				{"SystemSecurityXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Security.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Security_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Security_xunit-test.dll"),
				}},
				{"SystemXmlLinqXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Xml.Linq.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Xml.Linq_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System.Xml.Linq_xunit-test.dll"),
				}},
				{"SystemXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"xammac_net_4_5_System_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_System_xunit-test.dll"),
				}},
				{"CorlibXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Core.dll"),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Numerics.dll"),
					(assembly:"System.Runtime.CompilerServices.Unsafe", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/System.Runtime.CompilerServices.Unsafe.dll"),
					(assembly:"xammac_net_4_5_corlib_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5-darwin/tests/xammac_net_4_5_corlib_xunit-test.dll"),
				}},
		};
		public List<(string assembly, string hintPath)> GetCachedAssemblyInclusionInformation (string monoRootPath,
			Platform platform)
		{
			if (!monoRootPath.EndsWith ("/"))
				monoRootPath += "/";
			var info = new List<(string assembly, string hintPath)> ();
			switch (platform){
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				info = iOSCachedAssemblyInfo[Name]; 
				break;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				info = macCachedAssemblyInfo[Name];
				break;
			}
			// lets fix the path
			var fixedResult = new List<(string assembly, string hintPath)> ();
			foreach (var (assembly, hin) in info){
				fixedResult.Add ((assembly, hin.Replace ("{MONO_ROOT}", monoRootPath)));
			}

			return fixedResult;
		}
	}
}
