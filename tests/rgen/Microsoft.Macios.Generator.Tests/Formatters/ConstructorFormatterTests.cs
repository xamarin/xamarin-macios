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

public class ConstructorFormatterTests : BaseGeneratorTestClass {
	
	class TestDataToDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string emptyConstructor = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public MyClass ();
}
";
			yield return [emptyConstructor, "public MyClass ()"];
			
			const string genericConstructor = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass<T> where T : Enum {
	public MyClass ();
}
";
			yield return [genericConstructor, "public MyClass ()"];
			
			const string noSpaceParamsMethod = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public MyClass();
}
";
			yield return [noSpaceParamsMethod, "public MyClass ()"];
			
			const string singleParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public MyClass (string myParam);
}
";
			yield return [singleParamConstructor, "public MyClass (string myParam)"];
			
			const string severalParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public MyClass (string myParam, int myParamInt);
}
";
			yield return [severalParamConstructor, "public MyClass (string myParam, int myParamInt)"];
			
			const string nullableParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;
[BindingType<Class>]
public partial class MyClass {
	public MyClass(string? myParam);
}
";
			yield return [nullableParamConstructor, "public MyClass (string? myParam)"];
			
			const string customParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(MyStruct myParam);
}
";
			yield return [customParamConstructor, "public MyClass (NS.MyStruct myParam)"];
			
			const string nullableCustomParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(MyStruct? myParam);
}
";
			yield return [nullableCustomParamConstructor, "public MyClass (NS.MyStruct? myParam)"];
			
			const string refCustomParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(ref MyStruct myParam);
}
";
			yield return [refCustomParamConstructor, "public MyClass (ref NS.MyStruct myParam)"];
			
			const string inCustomParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(in MyStruct myParam);
}
";
			yield return [inCustomParamConstructor, "public MyClass (in NS.MyStruct myParam)"];
			
			const string refReadonlyCustomParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(ref readonly MyStruct myParam);
}
";
			yield return [refReadonlyCustomParamConstructor, "public MyClass (ref readonly NS.MyStruct myParam)"];
			
			const string arrayParamConbstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(MyStruct[] myParam);
}
";
			
			yield return [arrayParamConbstructor, "public MyClass (NS.MyStruct[] myParam)"];
			
			const string nullableArrayParamConstructor = @"
using System;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(MyStruct[]? myParam);
}
";
			
			yield return [nullableArrayParamConstructor, "public MyClass (NS.MyStruct[]? myParam)"];
			
			const string genericParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public MyClass(List<string> myParam);
}
";
			
			yield return [genericParamConstructor, "public MyClass (System.Collections.Generic.List<string> myParam)"];
			
			const string genericCustomParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(List<MyStruct> myParam);
}
";
			
			yield return [genericCustomParamConstructor, "public MyClass (System.Collections.Generic.List<NS.MyStruct> myParam)"];
			
			const string nullableGenericParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public MyClass(List<string>? myParam);
}
";
			
			yield return [nullableGenericParamConstructor, "public MyClass (System.Collections.Generic.List<string>? myParam)"];
			
			const string nullableGenericCustomParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass(List<MyStruct>? myParam);
}
";
			
			yield return [nullableGenericCustomParamConstructor, "public MyClass (System.Collections.Generic.List<NS.MyStruct>? myParam)"];
			
			const string nullableNullableGenericParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public MyClass (List<string?>? myParam);
}
";
			
			yield return [nullableNullableGenericParamConstructor, "public MyClass (System.Collections.Generic.List<string?>? myParam)"];
			
			const string nullableNullableGenericCustomParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

public struct MyStruct {
	public string Name;
	public string Surname;
}

[BindingType<Class>]
public partial class MyClass {
	public MyClass (List<MyStruct?>? myParam);
}
";
			
			yield return [nullableNullableGenericCustomParamConstructor, "public MyClass (System.Collections.Generic.List<NS.MyStruct?>? myParam)"];
			
			
			const string genericDictParamConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public MyClass (Dictionary<string, string> myParam);
}
";
			
			yield return [genericDictParamConstructor, "public MyClass (System.Collections.Generic.Dictionary<string, string> myParam)"];
			
			const string paramsConstructor = @"
using System;
using System.Collections.Generic;
using ObjCBindings;
namespace NS;

[BindingType<Class>]
public partial class MyClass {
	public MyClass(params string[] data);
}
";
			
			yield return [paramsConstructor, "public MyClass (params string[] data)"];
			
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
			.OfType<ConstructorDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		Assert.NotNull (semanticModel);
		Assert.True(Constructor.TryCreate (declaration, semanticModel, out var constructor));
		Assert.NotNull(constructor);
		var constructorDeclaration= constructor.ToDeclaration ();
		Assert.NotNull (constructorDeclaration);
		Assert.Equal(expectedDeclaration, constructorDeclaration.ToString());
	}
}
