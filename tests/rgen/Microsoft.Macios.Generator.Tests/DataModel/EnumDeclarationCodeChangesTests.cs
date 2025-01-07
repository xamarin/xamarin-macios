using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class EnumDeclarationCodeChangesTests : BaseGeneratorTestClass {
	CodeChanges CreateCodeChanges (ApplePlatform platform, string name, string inputText)
	{
		var (compilation, sourceTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		var enumDeclaration = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<EnumDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (enumDeclaration);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		var codeChange = CodeChanges.FromDeclaration (enumDeclaration, semanticModel);
		Assert.NotNull (codeChange);
		return codeChange.Value;
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CreateCodeChangeNoFieldsNoAttributes (ApplePlatform platform)
	{
		const string inputString = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {
}
";
		var codeChanges =
			CreateCodeChanges (platform, nameof (CreateCodeChangeNoFieldsNoAttributes), inputString);
		Assert.Equal ("AVFoundation.AVCaptureDeviceType", codeChanges.FullyQualifiedSymbol);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		Assert.Single (codeChanges.Attributes);
		Assert.Equal (AttributesNames.BindingAttribute, codeChanges.Attributes [0].Name);
		Assert.Empty (codeChanges.EnumMembers);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CreateCodeChangeFields (ApplePlatform platform)
	{
		const string inputString = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInTelephotoCamera"")]
	BuiltInTelephotoCamera,
}
";

		var codeChanges =
			CreateCodeChanges (platform, nameof (CreateCodeChangeNoFieldsNoAttributes), inputString);
		Assert.Equal ("AVFoundation.AVCaptureDeviceType", codeChanges.FullyQualifiedSymbol);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		Assert.Single (codeChanges.Attributes);
		Assert.Equal (AttributesNames.BindingAttribute, codeChanges.Attributes [0].Name);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		// validate that we have the 3 members and their attrs
		Assert.Equal (3, codeChanges.EnumMembers.Length);
		Assert.Equal ("BuiltInMicrophone", codeChanges.EnumMembers [0].Name);
		var expectedFields = new [] {
			"AVCaptureDeviceTypeBuiltInMicrophone", "AVCaptureDeviceTypeBuiltInWideAngleCamera",
			"AVCaptureDeviceTypeBuiltInTelephotoCamera"
		};
		for (var index = 0; index < expectedFields.Length; index++) {
			Assert.Equal ("ObjCBindings.FieldAttribute<ObjCBindings.EnumValue>",
				codeChanges.EnumMembers [index].Attributes [0].Name);
			Assert.Equal (expectedFields [index], codeChanges.EnumMembers [index].Attributes [0].Arguments [0]);
		}
	}

	[Theory]
	[AllSupportedPlatforms]
	public void CreateCodeChangeNoFieldAttributes (ApplePlatform platform)
	{
		const string inputString = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {
	// should be ignored
	BuiltInMicrophone,
	// should be ignored
	BuiltInWideAngleCamera,
	// should be ignored
	BuiltInTelephotoCamera,
}
";

		var codeChanges =
			CreateCodeChanges (platform, nameof (CreateCodeChangeNoFieldsNoAttributes), inputString);
		Assert.Equal ("AVFoundation.AVCaptureDeviceType", codeChanges.FullyQualifiedSymbol);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		Assert.Single (codeChanges.Attributes);
		Assert.Equal (AttributesNames.BindingAttribute, codeChanges.Attributes [0].Name);
		Assert.Empty (codeChanges.EnumMembers);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
	}


	[Theory]
	[AllSupportedPlatforms]
	public void CreateCodeChangeFieldsMissing (ApplePlatform platform)
	{
		const string inputString = @"
using System;
using Foundation;
using ObjCBindings;

namespace AVFoundation;

[BindingType]
public enum AVCaptureDeviceType {

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInMicrophone"")]
	BuiltInMicrophone,

	[Field<EnumValue> (""AVCaptureDeviceTypeBuiltInWideAngleCamera"")]
	BuiltInWideAngleCamera,

	// missing attr, this should not be present in the code changes, ie: it was removed by the user
	BuiltInTelephotoCamera,
}
";

		var codeChanges =
			CreateCodeChanges (platform, nameof (CreateCodeChangeNoFieldsNoAttributes), inputString);
		Assert.Equal ("AVFoundation.AVCaptureDeviceType", codeChanges.FullyQualifiedSymbol);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		Assert.Single (codeChanges.Attributes);
		Assert.Equal (AttributesNames.BindingAttribute, codeChanges.Attributes [0].Name);
		Assert.Equal (BindingType.SmartEnum, codeChanges.BindingType);
		// validate that we have the 3 members and their attrs
		Assert.Equal (2, codeChanges.EnumMembers.Length);
		Assert.Equal ("BuiltInMicrophone", codeChanges.EnumMembers [0].Name);
		var expectedFields = new [] {
			"AVCaptureDeviceTypeBuiltInMicrophone", "AVCaptureDeviceTypeBuiltInWideAngleCamera",
		};
		for (var index = 0; index < expectedFields.Length; index++) {
			Assert.Equal ("ObjCBindings.FieldAttribute<ObjCBindings.EnumValue>",
				codeChanges.EnumMembers [index].Attributes [0].Name);
			Assert.Equal (expectedFields [index], codeChanges.EnumMembers [index].Attributes [0].Arguments [0]);
		}
	}
}
