using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public partial class ApiTest {
		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblyDefinitions))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void ARConfiguration_GetSupportedVideoFormats (AssemblyInfo info)
		{
			// all subclasses of ARConfiguration must (re)export 'GetSupportedVideoFormats'
			var assembly = info.Assembly;
			List<string>? failures = null;

			if (!assembly.EnumerateTypes ((type) => type.Is ("ARKit", "ARConfiguration")).Any ())
				Assert.Ignore ("This assembly doesn't contain ARKit.ARConfiguration");

			var subclasses = assembly.EnumerateTypes ((type) => !type.Is ("ARKit", "ARConfiguration") && type.IsSubclassOf ("ARKit", "ARConfiguration"));
			Assert.That (subclasses, Is.Not.Empty, "At least some subclasses");

			foreach (var type in subclasses) {
				var method = type.Methods.SingleOrDefault (m => m.Name == "GetSupportedVideoFormats" && m.IsPublic && m.IsStatic && !m.HasParameters);
				if (method is null)
					AddFailure (ref failures, $"The type {type.FullName} does not implement the method GetSupportedVideoFormats.");
			}

			Assert.That (failures, Is.Null.Or.Empty, "All subclasses from ARConfiguration must explicitly implement GetSupportedVideoFormats.");
		}

		static void AddFailure (ref List<string>? failures, string failure)
		{
			if (failures is null)
				failures = new List<string> ();

			failures.Add (failure);
			Console.WriteLine (failure);
		}

		// Capitalization rules:
		// * All types, methods, properties, fields, events must start with an upper-cased letter.
		// * All parameters must start with a lower-cased letter
		[Test]
		public void MustStartWithCapitalLetter ()
		{
			Configuration.IgnoreIfAnyIgnoredPlatforms ();

			var failures = new Dictionary<string, (string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var member in assembly.EnumeratePublicMembers ()) {
					// Presumably obsolete members have been obsoleted for a reason, so assume there's a correctly capitalized version.
					if (((ICustomAttributeProvider) member).IsObsolete ())
						continue;

					if (member is MethodDefinition method) {
						// skip constructors
						if (method.IsConstructor)
							continue;
						// skip property accessors
						if (method.IsPropertyAccessor ())
							continue;
						// skip event methods
						if (method.IsEventMethod ())
							continue;
						// skip operators
						if (method.IsOperator ())
							continue;
					} else if (member is FieldDefinition field) {
						if (field.Name == "value__" && !field.IsStatic && field.DeclaringType.IsEnum)
							continue;
					}

					if (!char.IsUpper (member.Name [0]) && !IsAcceptableCapitalization (member)) {
						var msg = $"The {member.GetType ().Name.Replace ("Definition", "").ToLower ()} '{member.FullName}' has incorrect capitalization: first letter is not upper case.";
						failures [msg] = new (msg, member.RenderLocation ());
					}
				}

				foreach (var method in assembly.EnumerateMethods (Helper.IsPubliclyVisible)) {
					foreach (var param in method.Parameters) {
						if (param.Index == 0 && method.IsExtensionMethod ())
							continue;
						if (!char.IsLower (param.Name [0])) {
							var msg = $"The parameter '{param.Name}' in the method '{method.FullName}' has incorrect capitalization: first letter is not lower case.";
							failures [msg] = new (msg, method.RenderLocation ());
						}
					}
				}
			}

			Helper.AssertFailures (failures, knownFailuresMustStartWithCapitalizationLetter, nameof (knownFailuresMustStartWithCapitalizationLetter), "Incorrect capitalization", (v) => $"{v.Location}: {v.Message}");
		}

		// The difference between this and the known failures is that these aren't failures (that should be fixed), they're exceptions we've deemed acceptable (and shouldn't be fixed).
		bool IsAcceptableCapitalization (MemberReference mr)
		{
			if (mr is FieldDefinition field) {
				switch (field.DeclaringType.FullName) {
				case "Metal.MTLFeatureSet":
					if (field.Name.StartsWith ("iOS_", StringComparison.Ordinal))
						return true;
					if (field.Name.StartsWith ("macOS_", StringComparison.Ordinal))
						return true;
					if (field.Name.StartsWith ("tvOS_", StringComparison.Ordinal))
						return true;
					break;
				case "Metal.MTLGpuFamily":
					if (field.Name.StartsWith ("iOSMac", StringComparison.Ordinal))
						return true;
					break;
				}
			} else if (mr is PropertyDefinition property) {
				switch (property.DeclaringType.FullName) {
				case "AVFoundation.AVMetadata":
					if (property.Name.StartsWith ("iTunes", StringComparison.Ordinal))
						return true;
					break;
				}
			}
			return false;
		}

		[Test]
		public void InvalidStrings ()
		{
			Configuration.IgnoreIfAnyIgnoredPlatforms ();

			var invalidStrings = new [] {
				new { Find = "URL", Replacement = "Url" }
			};

			var failures = new Dictionary<string, (string Message, string Location)> ();

			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				var assembly = info.Assembly;
				foreach (var member in assembly.EnumeratePublicMembers ()) {
					foreach (var term in invalidStrings) {
						if (member.Name.Contains (term.Find)) {
							var msg = $"The {member.GetType ().Name.Replace ("Definition", "").ToLower ()} '{member.FullName}' has an invalid term '{term.Find}'. Replace with: '{term.Replacement}'.";
							failures [msg] = new (msg, member.RenderLocation ());
						}
					}
				}
			}

			Helper.AssertFailures (failures, knownFailuresInvalidStrings, nameof (knownFailuresInvalidStrings), "In the file tests/cecil-tests/ApiTest.cs, read the guide carefully.", (v) => $"{v.Location}: {v.Message}");
		}
	}
}
