// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Formatters;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Formatters;

public class PropertyFormatterTests : BaseGeneratorTestClass {

	class TestDataToDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string stringProperty = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public string Property { get; set; }
}
";
			yield return [stringProperty, "public string Property"];

			const string nullableStringProperty = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public string? Property { get; set; }
}
";
			yield return [nullableStringProperty, "public string? Property"];


			const string internalStringProperty = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	internal string Property { get; set; }
}
";
			yield return [internalStringProperty, "internal string Property"];

			const string partialStringProperty = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public partial string Property { get; set; }
}
";
			yield return [partialStringProperty, "public partial string Property"];

			const string nullableStructProperty = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}
[BindingType<Class>]
public partial class MyClass {
	public partial MyStruct? Property { get; set; }
}
";
			yield return [nullableStructProperty, "public partial NS.MyStruct? Property"];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataToDeclaration>]
	public void ToDeclarationTests (ApplePlatform platform, string inputText, string expectedDeclaration)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		Assert.True (Property.TryCreate (declaration, semanticModel, out var property));
		Assert.NotNull (property);
		var propertyDeclaration = property.ToDeclaration ();
		Assert.NotNull (propertyDeclaration);
		Assert.Equal (expectedDeclaration, propertyDeclaration.ToString ());
	}
}
