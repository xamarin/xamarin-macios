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

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, notAttributeInValue, true];
			}

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
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, wrongAttributeInValue, true];
			}
			
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
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, presentAttributeInValue, false];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof(TestDataSkipEnumValueDeclaration))]
	public void SkipEnumValueDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, sourceTrees) =
			CreateCompilation (nameof(SkipEnumValueDeclaration), platform, inputText);
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
			const string missingAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	public string Name { get; set; }
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, missingAttributeInProperty, true];
			}

			const string wrongAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Field<EnumValue> (""name"")]
	public string Name { get;set; }
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, wrongAttributeInProperty, true];
			}	
			
			const string fieldAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Field> (""name"")]
	public string Name { get;set; }
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, fieldAttributeInProperty, false];	
			}
			
			const string propertyAttributeInProperty = @"
using System;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

[BindingType]
public class TestClass {
	[Export<Property> (""name"")]
	public string Name { get;set; }
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [platform, propertyAttributeInProperty, false];	
			}
			
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof(TestDataSkipPropertyDeclaration))]
	public void SkipPropertyDeclaration (ApplePlatform platform, string inputText, bool expected)
	{
		var (compilation, sourceTrees) =
			CreateCompilation (nameof(SkipEnumValueDeclaration), platform, inputText);
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
}
