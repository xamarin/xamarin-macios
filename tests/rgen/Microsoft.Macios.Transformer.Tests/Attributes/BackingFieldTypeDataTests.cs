// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Extensions;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class BackingFieldTypeDataTests : BaseTransformerTestClass {
	
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";
			const string nsnumberBackingFieldType = @"
using System;
using Foundation;
using ObjCRuntime;

[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
[BackingFieldType (typeof (NSNumber))]
public enum ASAuthorizationProviderExtensionEncryptionAlgorithm {
	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmECDHE_A256GCM"")]
	EcdheA256Gcm,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P256_SHA256_AES_GCM_256"")]
	HpkeP256Sha256AesGcm256,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P384_SHA384_AES_GCM_256"")]
	HpkeP384Sha384AesGcm256,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_Curve25519_SHA256_ChachaPoly"")]
	HpkeCurve25519Sha256ChachaPoly,
}
";
			yield return [(Source: nsnumberBackingFieldType, Path: path), new BackingFieldTypeData ("Foundation.NSNumber")];
			
			const string nsstringBackingFieldType = @"
using System;
using Foundation;
using ObjCRuntime;

[NoTV, Mac (15, 0), NoiOS, NoMacCatalyst]
[BackingFieldType (typeof (NSString))]
public enum ASAuthorizationProviderExtensionEncryptionAlgorithm {
	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmECDHE_A256GCM"")]
	EcdheA256Gcm,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P256_SHA256_AES_GCM_256"")]
	HpkeP256Sha256AesGcm256,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_P384_SHA384_AES_GCM_256"")]
	HpkeP384Sha384AesGcm256,

	[Field (""ASAuthorizationProviderExtensionEncryptionAlgorithmHPKE_Curve25519_SHA256_ChachaPoly"")]
	HpkeCurve25519Sha256ChachaPoly,
}
";
			yield return [(Source: nsstringBackingFieldType, Path: path), new BackingFieldTypeData ("Foundation.NSString")];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryeCreateTests (ApplePlatform platform, (string Source, string Path) source, BackingFieldTypeData expectedData)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.ForSource (source);
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<BaseTypeDeclarationSyntax> ()
			.LastOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		var exportData = symbol.GetAttribute<BackingFieldTypeData> (
			AttributesNames.BackingFieldTypeAttribute, BackingFieldTypeData.TryParse);
		Assert.Equal (expectedData, exportData);
	}
}
