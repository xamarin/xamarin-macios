using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Tests;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class PreviewApi {
		[TestCase ("CryptoTokenKit", "APL0001", "10.0")]
		// [TestCase ("FSKit", "APL0002", "11.0")]
		public void EverythingInNamespace (string ns, string expectedDiagnosticId, string stableInDotNetVersion)
		{
			// Verify that all types in the given namespace have an Experimental attribute.
			var failures = new HashSet<string> ();
			var isAttributeExpected = Version.Parse (Configuration.DotNetTfm.Replace ("net", "")) < Version.Parse (stableInDotNetVersion);

			var typesInNamespace = new List<TypeDefinition> ();
			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				typesInNamespace.AddRange (info.Assembly.EnumerateTypes (t => t.Namespace == ns));
			}

			if (typesInNamespace.Count == 0)
				failures.Add ($"No types found for the namespace {ns}, something is very wrong somewhere.");
			foreach (var type in typesInNamespace) {
				var hasAttribute = TryGetExperimentalAttribute (type, out var diagnosticId);

				if (isAttributeExpected) {
					if (hasAttribute) {
						if (diagnosticId != expectedDiagnosticId)
							failures.Add ($"The type '{type.FullName}' has the [Experimental] attribute as expected, but the diagnostic id is incorrect (expected '{expectedDiagnosticId}', actual '{diagnosticId}')");
					} else {
						failures.Add ($"The type '{type.FullName}' is supposed to be in preview in until .NET {stableInDotNetVersion}, but it does not have an [Experimental] attribute.");
					}
				} else {
					if (hasAttribute)
						failures.Add ($"The type '{type.FullName}' is supposed to be stable in .NET {stableInDotNetVersion}, but it has an [Experimental] attribute.");
				}
			}

			// This looks complicated, but it's to render test failures better than "Assert.That (failures, Is.Empty)"
			Assert.Multiple (() => {
				foreach (var failure in failures)
					Assert.Fail (failure);
			});
		}

		bool TryGetExperimentalAttribute (ICustomAttributeProvider type, [NotNullWhen (true)] out string? diagnosticId)
		{
			diagnosticId = null;

			if (!type.HasCustomAttributes)
				return false;

			foreach (var ca in type.CustomAttributes) {
				if (ca.AttributeType.Name == "ExperimentalAttribute" && ca.AttributeType.Namespace == "System.Diagnostics.CodeAnalysis") {
					diagnosticId = (string) ca.ConstructorArguments [0].Value;
					return true;
				}
			}

			return false;
		}
	}
}
