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

		private static Dictionary<string, List<(string assembly, string hint)>> macCachedAssemblyInfo =
			new Dictionary<string, List<(string assembly, string hint)>> {
				{"MonoCSharpTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"Mono.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.CSharp.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_Mono.CSharp_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Mono.CSharp_test.dll"),
				}},
				{"MonoDataSqilteTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"Mono.Data.Sqlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Data.Sqlite.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_Mono.Data.Sqlite_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Mono.Data.Sqlite_test.dll"),
				}},
				{"MonoDataTdsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"Mono.Data.Tds", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Data.Tds.dll"),
					(assembly:"xammac_net_4_5_Mono.Data.Tds_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Mono.Data.Tds_test.dll"),
				}},
				{"MonoPoxisTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"Mono.Posix", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Posix.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_Mono.Posix_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Mono.Posix_test.dll"),
				}},
				{"MonoSecurtiyTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"Mono.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Security.dll"),
					(assembly:"xammac_net_4_5_Mono.Security_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Mono.Security_test.dll"),
				}},
				{"SystemComponentModelDataAnnotationsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.ComponentModel.DataAnnotations", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.ComponentModel.DataAnnotations.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll"),
				}},
				{"SystemConfigurationTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Configuration.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.Configuration_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Configuration_test.dll"),
				}},
				{"SystemCoreTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_System.Core_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Core_test.dll"),
				}},
				{"SystemDataLinqTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Data.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.Linq.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Data.Linq_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Data.Linq_test.dll"),
				}},
				{"SystemDataTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Configuration.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"Mono.Data.Sqlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Data.Sqlite.dll"),
					(assembly:"xammac_net_4_5_System.Data_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Data_test.dll"),
				}},
				{"SystemIOCompressionFileSystemTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.IO.Compression", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.IO.Compression.dll"),
					(assembly:"System.IO.Compression.FileSystem", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.IO.Compression.FileSystem.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_System.IO.Compression.FileSystem_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.IO.Compression.FileSystem_test.dll"),
				}},
				{"SystemIOCompressionTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.IO.Compression", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.IO.Compression.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.IO.Compression_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.IO.Compression_test.dll"),
				}},
				{"SystemIdentityModelTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.IdentityModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.IdentityModel.dll"),
					(assembly:"System.Runtime.Serialization", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Runtime.Serialization.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.IdentityModel_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.IdentityModel_test.dll"),
				}},
				{"SystemJsonTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Json.dll"),
					(assembly:"xammac_net_4_5_System.Json_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Json_test.dll"),
				}},
				{"SystemNetHttpTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Net.Http", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Net.Http.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Net.Http_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Net.Http_test.dll"),
				}},
				{"SystemNumericsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Numerics.dll"),
					(assembly:"xammac_net_4_5_System.Numerics_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Numerics_test.dll"),
				}},
				{"SystemRuntimeSerializationFormattersSoapTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Runtime.Serialization.Formatters.Soap", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Runtime.Serialization.Formatters.Soap.dll"),
					(assembly:"xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll"),
				}},
				{"SystemSecurityTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Security.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"xammac_net_4_5_System.Security_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Security_test.dll"),
				}},
				{"SystemServiceModelTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.ServiceModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.ServiceModel.dll"),
					(assembly:"System.Runtime.Serialization", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Runtime.Serialization.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.IdentityModel", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.IdentityModel.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Security.dll"),
					(assembly:"xammac_net_4_5_System.ServiceModel_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.ServiceModel_test.dll"),
				}},
				{"SystemTransactionsTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Transactions", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Transactions.dll"),
					(assembly:"xammac_net_4_5_System.Transactions_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Transactions_test.dll"),
				}},
				{"SystemXmlLinqTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.Linq.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xammac_net_4_5_System.Xml.Linq_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Xml.Linq_test.dll"),
				}},
				{"SystemXmlTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"xammac_net_4_5_System.Xml_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Xml_test.dll"),
				}},
				{"SystemTests", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"nunitlite", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/nunitlite.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"System.Configuration", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Configuration.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"Mono.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Mono.Security.dll"),
					(assembly:"xammac_net_4_5_System_test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System_test.dll"),
				}},
				{"MicrosoftCSharpXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Microsoft.CSharp.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"xammac_net_4_5_Microsoft.CSharp_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_Microsoft.CSharp_xunit-test.dll"),
				}},
				{"SystemCoreXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Microsoft.CSharp.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"xunit.execution.dotnet", hint:""),
					(assembly:"xammac_net_4_5_System.Core_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Core_xunit-test.dll"),
				}},
				{"SystemDataXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System.Data", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Data.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"xammac_net_4_5_System.Data_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Data_xunit-test.dll"),
				}},
				{"SystemJsonXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Json", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Json.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Json_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Json_xunit-test.dll"),
				}},
				{"SystemNumericsXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Numerics.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"Microsoft.CSharp", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/Microsoft.CSharp.dll"),
					(assembly:"xammac_net_4_5_System.Numerics_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Numerics_xunit-test.dll"),
				}},
				{"SystemRuntimeCompilerServicesXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Runtime.CompilerServices.Unsafe", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Runtime.CompilerServices.Unsafe.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll"),
				}},
				{"SystemSecurityXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Security", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Security.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Security_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Security_xunit-test.dll"),
				}},
				{"SystemXmlLinqXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"System.Xml", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System.Xml.Linq", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Xml.Linq.dll"),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System.Xml.Linq_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System.Xml.Linq_xunit-test.dll"),
				}},
				{"SystemXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"xammac_net_4_5_System_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_System_xunit-test.dll"),
				}},
				{"CorlibXunit", new List<(string assembly, string hint)> {
					(assembly:"mscorlib", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/mscorlib.dll"),
					(assembly:"xunit.core", hint:""),
					(assembly:"xunit.abstractions", hint:""),
					(assembly:"Xunit.NetCore.Extensions", hint:"{MONO_ROOT}external/xunit-binaries/Xunit.NetCore.Extensions.dll"),
					(assembly:"xunit.assert", hint:""),
					(assembly:"System", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.dll"),
					(assembly:"System.Core", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Core.dll"),
					(assembly:"System.Numerics", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Numerics.dll"),
					(assembly:"System.Runtime.CompilerServices.Unsafe", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/System.Runtime.CompilerServices.Unsafe.dll"),
					(assembly:"xammac_net_4_5_corlib_xunit-test.dll", hint:"{MONO_ROOT}mcs/class/lib/xammac_net_4_5/tests/xammac_net_4_5_corlib_xunit-test.dll"),
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
				throw new InvalidOperationException ("All iOS platforms must used the dlls from the SDK and not build their own tests.");
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
