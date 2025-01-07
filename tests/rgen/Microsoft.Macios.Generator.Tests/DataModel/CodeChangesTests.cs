using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class CodeChangesTests : BaseGeneratorTestClass {
	class TestDataSkipEnumValueDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string notAttributeInValue = @"
using System;
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;

[BindingType]
enum AVMediaCharacteristics {
	Visual = 0,
}
";

			yield return [notAttributeInValue, true];

			const string wrongAttributeInValue = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
enum AVMediaCharacteristics {
	[Field<Property> (""AVMediaCharacteristicVisual"")]
	Visual = 0,
}
";
			yield return [wrongAttributeInValue, true];

			const string presentAttributeInValue = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
enum AVMediaCharacteristics {
	[Field<EnumValue> (""AVMediaCharacteristicVisual"")]
	Visual = 0,
}
";
			yield return [presentAttributeInValue, false];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataSkipEnumValueDeclaration>]
	public void SkipEnumValueDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, sourceTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<EnumMemberDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		Assert.Equal (expected, CodeChanges.Skip (node, semanticModel));
	}


	class TestDataSkipPropertyDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			const string notPartialProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	public string Name { get; set; }
}
";

			yield return [notPartialProperty, true];

			const string missingAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	public partial string Name { get; set; }
}
";
			yield return [missingAttributeInProperty, true];

			const string wrongAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Field<EnumValue> (""name"")]
	public partial string Name { get;set; }
}
";
			yield return [wrongAttributeInProperty, true];

			const string fieldAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Field> (""name"")]
	public partial string Name { get;set; }
}
";
			yield return [fieldAttributeInProperty, false];

			const string propertyAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Property> (""name"")]
	public partial string Name { get;set; }
}
";
			yield return [propertyAttributeInProperty, false];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataSkipPropertyDeclaration>]
	public void SkipPropertyDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, sourceTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		Assert.Equal (expected, CodeChanges.Skip (node, semanticModel));
	}

	class TestDataSkipMethodDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string notPartialMethod = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Method> (""name"")]
	public void GetName() {}
}
";
			yield return [notPartialMethod, true];

			const string wrongAttributeFlag = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Property> (""name"")]
	public partial void GetName();
}
";
			yield return [wrongAttributeFlag, true];

			const string correctMethod = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Method> (""name"")]
	public partial void GetName();
}
";
			yield return [correctMethod, false];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataSkipMethodDeclaration>]
	public void SkipMethodDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<MethodDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		Assert.Equal (expected, CodeChanges.Skip (node, semanticModel));
	}
}
