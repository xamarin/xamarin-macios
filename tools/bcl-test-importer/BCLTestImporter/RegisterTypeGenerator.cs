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
		static Dictionary<string, (string testNamespace, string testAssembly, string testType)> iOSCache = new Dictionary<string, (string testNamespace, string testAssembly, string testType)> {
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
		
		static Dictionary<string, (string testNamespace, string testAssembly, string testType)> macCache = new Dictionary<string, (string testNamespace, string testAssembly, string testType)> {
			{"MonoCSharpTests", ("MonoTests.Visit",  "xammac_net_4_5_Mono.CSharp_test.dll", "MonoTests.Visit.ASTVisitorTest")},
			{"MonoDataSqilteTests", ("MonoTests.Mono.Data.Sqlite",  "xammac_net_4_5_Mono.Data.Sqlite_test.dll", "MonoTests.Mono.Data.Sqlite.SqliteiOS82BugTests")},
			{"MonoDataTdsTests", ("MonoTests.Mono.Data.Tds",  "xammac_net_4_5_Mono.Data.Tds_test.dll", "MonoTests.Mono.Data.Tds.TdsConnectionPoolTest")},
			{"MonoPoxisTests", ("MonoTests.System.IO",  "xammac_net_4_5_Mono.Posix_test.dll", "MonoTests.System.IO.StdioFileStreamTest")},
			{"MonoSecurtiyTests", ("MonoTests.System.Security.Cryptography",  "xammac_net_4_5_Mono.Security_test.dll", "MonoTests.System.Security.Cryptography.SHA224ManagedTest")},
			{"SystemComponentModelDataAnnotationsTests", ("MonoTests.System.ComponentModel.DataAnnotations",  "xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll", "MonoTests.System.ComponentModel.DataAnnotations.AssociatedMetadataTypeTypeDescriptionProviderTests")},
			{"SystemConfigurationTests", ("MonoTests.System.Configuration",  "xammac_net_4_5_System.Configuration_test.dll", "MonoTests.System.Configuration.ProviderCollectionTest")},
			{"SystemCoreTests", ("MonoTests.System.Threading",  "xammac_net_4_5_System.Core_test.dll", "MonoTests.System.Threading.ReaderWriterLockSlimTests")},
			{"SystemDataLinqTests", ("DbLinqTest",  "xammac_net_4_5_System.Data.Linq_test.dll", "DbLinqTest.MsSqlDataContextTest")},
			{"SystemDataTests", ("MonoTests.System.Xml",  "xammac_net_4_5_System.Data_test.dll", "MonoTests.System.Xml.XmlDataDocumentTest2")},
			{"SystemIOCompressionFileSystemTests", ("MonoTests.System.IO.Compression.FileSystem",  "xammac_net_4_5_System.IO.Compression.FileSystem_test.dll", "MonoTests.System.IO.Compression.FileSystem.ZipArchiveTests")},
			{"SystemIOCompressionTests", ("MonoTests.System.IO.Compression",  "xammac_net_4_5_System.IO.Compression_test.dll", "MonoTests.System.IO.Compression.ZipArchiveTests")},
			{"SystemIdentityModelTests", ("MonoTests.System.IdentityModel.Tokens",  "xammac_net_4_5_System.IdentityModel_test.dll", "MonoTests.System.IdentityModel.Tokens.InMemorySymmetricSecurityKeyTest")},
			{"SystemJsonTests", ("MonoTests.System",  "xammac_net_4_5_System.Json_test.dll", "MonoTests.System.JsonValueTests")},
			{"SystemNetHttpTests", ("MonoTests.System.Net.Http",  "xammac_net_4_5_System.Net.Http_test.dll", "MonoTests.System.Net.Http.ByteArrayContentTest")},
			{"SystemNumericsTests", ("MonoTests.System.Numerics",  "xammac_net_4_5_System.Numerics_test.dll", "MonoTests.System.Numerics.BigIntegerTest")},
			{"SystemRuntimeSerializationFormattersSoapTests", ("MonoTests.System.Runtime.Serialization.Formatters.Soap",  "xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll", "MonoTests.System.Runtime.Serialization.Formatters.Soap.SerializationCallbackTest")},
			{"SystemSecurityTests", ("MonoCasTests.System.Security.Cryptography",  "xammac_net_4_5_System.Security_test.dll", "MonoCasTests.System.Security.Cryptography.CryptographicAttributeObjectCas")},
			{"SystemTransactionsTests", ("MonoTests.System.Transactions",  "xammac_net_4_5_System.Transactions_test.dll", "MonoTests.System.Transactions.AsyncTest")},
			{"SystemXmlLinqTests", ("MonoTests.System.Xml",  "xammac_net_4_5_System.Xml.Linq_test.dll", "MonoTests.System.Xml.ExtensionsTest")},
			{"SystemXmlTests", ("nist_dom.fundamental",  "xammac_net_4_5_System.Xml_test.dll", "nist_dom.fundamental.AttrTest")},
			{"SystemTests", ("MonoCasTests.System",  "xammac_net_4_5_System_test.dll", "MonoCasTests.System.FileStyleUriParserCas")},
			{"MicrosoftCSharpXunit", ("Microsoft.CSharp.RuntimeBinder.Tests",  "xammac_net_4_5_Microsoft.CSharp_xunit-test.dll", "Microsoft.CSharp.RuntimeBinder.Tests.AccessTests")},
			{"SystemCoreXunit", ("System.Dynamic.Tests",  "xammac_net_4_5_System.Core_xunit-test.dll", "System.Dynamic.Tests.BinaryOperationTests")},
			{"SystemDataXunit", ("System.Data.SqlClient.Tests",  "xammac_net_4_5_System.Data_xunit-test.dll", "System.Data.SqlClient.Tests.CloneTests")},
			{"SystemJsonXunit", ("System.Json.Tests",  "xammac_net_4_5_System.Json_xunit-test.dll", "System.Json.Tests.JsonArrayTests")},
			{"SystemNumericsXunit", ("System.Numerics.Tests",  "xammac_net_4_5_System.Numerics_xunit-test.dll", "System.Numerics.Tests.GenericVectorTests")},
			{"SystemRuntimeCompilerServicesXunit", ("System.Runtime.CompilerServices",  "xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", "System.Runtime.CompilerServices.UnsafeTests")},
			{"SystemXmlLinqXunit", ("Microsoft.Test.ModuleCore",  "xammac_net_4_5_System.Xml.Linq_xunit-test.dll", "Microsoft.Test.ModuleCore.LtmContext")},
			{"SystemServiceModelTests", ("MonoTests.System.ServiceModel",  "xammac_net_4_5_System.ServiceModel_test.dll", "MonoTests.System.ServiceModel.Bug36080")},
			{"SystemSecurityXunit", ("System.Security.Cryptography.Pkcs.Tests",  "xammac_net_4_5_System.Security_xunit-test.dll", "System.Security.Cryptography.Pkcs.Tests.CryptographicAttributeObjectCollectionTests")},
			{"SystemXunit", ("RegexTestNamespace",  "xammac_net_4_5_System_xunit-test.dll", "RegexTestNamespace.RegexTestClass")},
			{"CorlibXunit", ("System.Collections.Generic.Tests",  "xammac_net_4_5_corlib_xunit-test.dll", "System.Collections.Generic.Tests.ByteComparersTests")},
			};

		public static async Task<string> GenerateCodeAsync (string projectName, bool isXunit, string templatePath, Platform  platform)
		{
			Dictionary<string, (string testNamespace, string testAssembly, string testType)> cache = null;
			switch (platform){
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				cache = iOSCache;
				break;
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				cache = macCache;
				break;
			}
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			importStringBuilder.AppendLine ($"using {cache[projectName].testNamespace};");
			keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{cache[projectName].testAssembly}\", typeof ({cache[projectName].testType})}}, ");
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
