using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace BCLTestImporter {
	public static class RegisterTypeGenerator {

		static readonly string UsingReplacement = "%USING%";
		static readonly string KeysReplacement = "%KEY VALUES%";
		static readonly string IsxUnitReplacement = "%IS XUNIT%";

		// the following cache is a workaround until mono does provide the required binaries precompiled, at that point
		// we will remove the dict and we will use the refection based method.
		internal static Dictionary<string, (string testNamespace, string testAssembly, string testType) []> MacCache = new Dictionary<string, (string testNamespace, string testAssembly, string testType) []> {
			{"MonoCSharpTests", new [] {("MonoTests.Visit",  "xammac_net_4_5_Mono.CSharp_test.dll", "MonoTests.Visit.ASTVisitorTest")} },
			{"MonoDataSqliteTests", new [] {("MonoTests.Mono.Data.Sqlite",  "xammac_net_4_5_Mono.Data.Sqlite_test.dll", "MonoTests.Mono.Data.Sqlite.SqliteiOS82BugTests")}},
			{"MonoDataTdsTests", new [] {("MonoTests.Mono.Data.Tds",  "xammac_net_4_5_Mono.Data.Tds_test.dll", "MonoTests.Mono.Data.Tds.TdsConnectionPoolTest")}},
			{"MonoPoxisTests", new [] {("MonoTests.System.IO",  "xammac_net_4_5_Mono.Posix_test.dll", "MonoTests.System.IO.StdioFileStreamTest")}},
			{"MonoSecurityTests", new [] {("MonoTests.System.Security.Cryptography",  "xammac_net_4_5_Mono.Security_test.dll", "MonoTests.System.Security.Cryptography.SHA224ManagedTest")}},
			{"SystemComponentModelDataAnnotationsTests", new [] {("MonoTests.System.ComponentModel.DataAnnotations",  "xammac_net_4_5_System.ComponentModel.DataAnnotations_test.dll", "MonoTests.System.ComponentModel.DataAnnotations.AssociatedMetadataTypeTypeDescriptionProviderTests")} },
			{"SystemConfigurationTests", new [] {("MonoTests.System.Configuration",  "xammac_net_4_5_System.Configuration_test.dll", "MonoTests.System.Configuration.ProviderCollectionTest")}},
			{"SystemCoreTests", new [] {("MonoTests.System.Threading",  "xammac_net_4_5_System.Core_test.dll", "MonoTests.System.Threading.ReaderWriterLockSlimTests")}},
			{"SystemDataLinqTests", new [] {("DbLinqTest",  "xammac_net_4_5_System.Data.Linq_test.dll", "DbLinqTest.MsSqlDataContextTest")}},
			{"SystemDataTests", new [] {("MonoTests.System.Xml",  "xammac_net_4_5_System.Data_test.dll", "MonoTests.System.Xml.XmlDataDocumentTest2")}},
			{"SystemIOCompressionFileSystemTests", new [] {("MonoTests.System.IO.Compression.FileSystem",  "xammac_net_4_5_System.IO.Compression.FileSystem_test.dll", "MonoTests.System.IO.Compression.FileSystem.ZipArchiveTests")}},
			{"SystemIOCompressionTests", new [] {("MonoTests.System.IO.Compression",  "xammac_net_4_5_System.IO.Compression_test.dll", "MonoTests.System.IO.Compression.ZipArchiveTests")}},
			{"SystemIdentityModelTests", new [] {("MonoTests.System.IdentityModel.Tokens",  "xammac_net_4_5_System.IdentityModel_test.dll", "MonoTests.System.IdentityModel.Tokens.InMemorySymmetricSecurityKeyTest")}},
			{"SystemJsonTests", new [] {("MonoTests.System",  "xammac_net_4_5_System.Json_test.dll", "MonoTests.System.JsonValueTests")}},
			{"SystemNetHttpTests", new [] {("MonoTests.System.Net.Http",  "xammac_net_4_5_System.Net.Http_test.dll", "MonoTests.System.Net.Http.ByteArrayContentTest")}},
			{"SystemNumericsTests", new [] {("MonoTests.System.Numerics",  "xammac_net_4_5_System.Numerics_test.dll", "MonoTests.System.Numerics.BigIntegerTest")}},
			{"SystemRuntimeSerializationFormattersSoapTests", new [] {("MonoTests.System.Runtime.Serialization.Formatters.Soap",  "xammac_net_4_5_System.Runtime.Serialization.Formatters.Soap_test.dll", "MonoTests.System.Runtime.Serialization.Formatters.Soap.SerializationCallbackTest")}},
			{"SystemSecurityTests", new [] {("MonoCasTests.System.Security.Cryptography",  "xammac_net_4_5_System.Security_test.dll", "MonoCasTests.System.Security.Cryptography.CryptographicAttributeObjectCas")}},
			{"SystemTransactionsTests", new [] {("MonoTests.System.Transactions",  "xammac_net_4_5_System.Transactions_test.dll", "MonoTests.System.Transactions.AsyncTest")}},
			{"SystemXmlLinqTests", new [] {("MonoTests.System.Xml",  "xammac_net_4_5_System.Xml.Linq_test.dll", "MonoTests.System.Xml.ExtensionsTest")}},
			{"SystemXmlTests", new [] {("nist_dom.fundamental",  "xammac_net_4_5_System.Xml_test.dll", "nist_dom.fundamental.AttrTest")}},
			{"SystemTests", new [] {("MonoCasTests.System",  "xammac_net_4_5_System_test.dll", "MonoCasTests.System.FileStyleUriParserCas")}},
			{"MicrosoftCSharpXunit", new [] {("Microsoft.CSharp.RuntimeBinder.Tests",  "xammac_net_4_5_Microsoft.CSharp_xunit-test.dll", "Microsoft.CSharp.RuntimeBinder.Tests.AccessTests")}},
			{"SystemCoreXunit", new [] {("System.Dynamic.Tests",  "xammac_net_4_5_System.Core_xunit-test.dll", "System.Dynamic.Tests.BinaryOperationTests")}},
			{"SystemDataXunit", new [] {("System.Data.SqlClient.Tests",  "xammac_net_4_5_System.Data_xunit-test.dll", "System.Data.SqlClient.Tests.CloneTests")}},
			{"SystemJsonXunit", new [] {("System.Json.Tests",  "xammac_net_4_5_System.Json_xunit-test.dll", "System.Json.Tests.JsonArrayTests")}},
			{"SystemNumericsXunit", new [] {("System.Numerics.Tests",  "xammac_net_4_5_System.Numerics_xunit-test.dll", "System.Numerics.Tests.GenericVectorTests")}},
			{"SystemRuntimeCompilerServicesXunit", new [] {("System.Runtime.CompilerServices",  "xammac_net_4_5_System.Runtime.CompilerServices.Unsafe_xunit-test.dll", "System.Runtime.CompilerServices.UnsafeTests")}},
			{"SystemXmlLinqXunit", new [] {("Microsoft.Test.ModuleCore",  "xammac_net_4_5_System.Xml.Linq_xunit-test.dll", "Microsoft.Test.ModuleCore.LtmContext")}},
			{"SystemServiceModelTests", new [] {("MonoTests.System.ServiceModel",  "xammac_net_4_5_System.ServiceModel_test.dll", "MonoTests.System.ServiceModel.Bug36080")}},
			{"SystemSecurityXunit", new [] {("System.Security.Cryptography.Pkcs.Tests",  "xammac_net_4_5_System.Security_xunit-test.dll", "System.Security.Cryptography.Pkcs.Tests.CryptographicAttributeObjectCollectionTests")}},
			{"SystemXunit", new [] {("RegexTestNamespace",  "xammac_net_4_5_System_xunit-test.dll", "RegexTestNamespace.RegexTestClass")}},
			{"CorlibXunit", new [] {("System.Collections.Generic.Tests",  "xammac_net_4_5_corlib_xunit-test.dll", "System.Collections.Generic.Tests.ByteComparersTests")}},
			};

		public static async Task<string> GenerateCodeAsync (string projectName, bool isXunit, string templatePath, Platform  platform)
		{
			Dictionary<string, (string testNamespace, string testAssembly, string testType)[]> cache = null;
			switch (platform){
			case Platform.iOS:
			case Platform.TvOS:
			case Platform.WatchOS:
				throw new InvalidOperationException ("All iOS projects should be using the sdk test assemblies and not compile them.");
			case Platform.MacOSFull:
			case Platform.MacOSModern:
				cache = MacCache;
				break;
			}
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			foreach (var typeInfo in cache [projectName]) {
				importStringBuilder.AppendLine ($"using {typeInfo.testNamespace};");
				keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{typeInfo.testAssembly}\", typeof ({typeInfo.testType})}}, ");
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

		public static async Task<string> GenerateCodeAsync ((string FailureMessage, Dictionary<string, Type> Types) typeRegistration, bool isXunit,
			string templatePath)
		{
			var importStringBuilder = new StringBuilder ();
			var keyValuesStringBuilder = new StringBuilder ();
			var namespaces = new List<string> ();  // keep track of the namespaces to remove warnings
			if (!string.IsNullOrEmpty (typeRegistration.FailureMessage)) {
				keyValuesStringBuilder.AppendLine ($"#error {typeRegistration.FailureMessage}");
			} else {
				foreach (var a in typeRegistration.Types.Keys) {
					var t = typeRegistration.Types [a];
					if (!string.IsNullOrEmpty (t.Namespace)) {
						if (!namespaces.Contains (t.Namespace)) {
							namespaces.Add (t.Namespace);
							importStringBuilder.AppendLine ($"using {t.Namespace};");
						}
						keyValuesStringBuilder.AppendLine ($"\t\t\t{{ \"{a}\", typeof ({t.FullName})}}, ");
					}
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
	}
}
