// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace AssemblyPreparerTests;

public class RegistrarTests : BaseClass {
	[TestCase ("", "", true)]
	[TestCase ("", ".0", true)]
	[TestCase ("", ".0.0", true)]
	[TestCase (".0", "", true)]
	[TestCase (".0", ".0", true)]
	[TestCase (".0", ".0.0", true)]
	[TestCase (".0.0", "", true)]
	[TestCase (".0.0", ".0", true)]
	[TestCase (".0.0", ".0.0", true)]
	[TestCase ("", ".1", false)]
	[TestCase (".0", ".1", false)]
	[TestCase (".0.0", ".0.1", false)]
	[TestCase (".1", "", true)]
	[TestCase (".1", ".0", true)]
	[TestCase (".0.1", ".0.0", true)]
	public void SdkVersionComponents (string sdkSuffix, string introducedSuffix, bool available)
	{
		var platform = ApplePlatform.MacCatalyst;
		Configuration.IgnoreIfIgnoredPlatform (platform);

		var sdk = Version.Parse (Configuration.GetSdkVersion (platform));
		var version = $"{sdk.Major}.{sdk.Minor}";
		var sdkVersion = version + sdkSuffix;
		var introducedVersion = version + introducedSuffix;
		var code = $$"""
			using Foundation;
			using System.Runtime.Versioning;

			[SupportedOSPlatform ("maccatalyst{{introducedVersion}}")]
			class IntroducedType : NSObject {
			}

			class DerivedType : IntroducedType {
			}

			class Consumer : NSObject {
				[Export ("consume:")]
				public void Consume (IntroducedType value) {}

				[Export ("produce")]
				public IntroducedType Produce () => null;

				[Export ("value")]
				public IntroducedType Value { get; set; }
			}
			""";

		using var preparer = CreatePreparer (platform, false, p => p.Registrar = RegistrarMode.ManagedStatic, code, out _, extraConfig: $"SdkVersion={sdkVersion}");
		if (available) {
			AssertPrepare (preparer);
		} else {
			Assert.That (preparer.Prepare (out _), Is.False, "Prepare");
			var logger = (TestLogger) preparer.Configuration.Logger;
			var errors = logger.Errors.Where (v => v.Code == 4162 && v.Message.Contains ("'IntroducedType'")).ToArray ();
			foreach (var usage in new [] { "a base type of DerivedType", "a parameter in Consumer.Consume", "a return type in Consumer.Produce", "the property type of Consumer.Value" }) {
				Assert.That (errors.Select (v => v.Message), Has.Some.Contains ($"used as {usage}"), usage);
			}
			foreach (var error in errors) {
				Assert.That (error.Message, Does.Contain ($"not available in MacCatalyst {sdkVersion} (it was introduced in MacCatalyst {introducedVersion})"), "Version diagnostic");
			}
		}
	}

	[Test]
	public void ModelClassAsGenericArgument ()
	{
		var code = @"
		using Foundation;
		using ObjCRuntime;

		class MyClass : NSObject {
			[Export (""sessionOptions"")]
			public NSDictionary<NSString, NSFileManagerDelegate> GetSessionOptions ()
			{
				throw new System.NotImplementedException ();
			}
		}
		";

		using var preparer = CreatePreparer (ApplePlatform.iOS, false, p => p.Registrar = RegistrarMode.ManagedStatic, code, out _);
		preparer.Prepare (out _);

		var logger = (TestLogger) preparer.Configuration.Logger;
		var exception = logger.Errors.Single (v => v.Code == 4192);
		Assert.That (exception.Message, Is.EqualTo ("The registrar cannot use the model class 'Foundation.NSFileManagerDelegate' as a generic type argument in the generic type 'Foundation.NSDictionary`2<Foundation.NSString,Foundation.NSFileManagerDelegate>'. Use the protocol interface instead of the model class."));
	}

	[Test]
	public void ProtocolInterfaceAsGenericArgument ()
	{
		var code = @"
		using Foundation;
		using ObjCRuntime;

		class MyClass : NSObject {
			[Export (""sessionOptions"")]
			public NSDictionary<NSString, INSFileManagerDelegate> GetSessionOptions ()
			{
				throw new System.NotImplementedException ();
			}
		}
		";

		AssertPrepare (ApplePlatform.iOS, false, RegistrarMode.ManagedStatic, code, out _);
	}
}
