// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using Microsoft.Macios.Generator.Extensions;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class TypeSymbolExtensionsSizeTests : BaseGeneratorTestClass {


	class TestDataTryGetBuiltInTypeSize : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// create properties so that we can get the return symbol and use it to calculate the size, 
			// we make sure that the result is the same as the one returned by the Stret.cs code.
			const string nfloatProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	NFloat Test { get; set; }
}
";
			yield return [nfloatProperty, typeof (NFloat)];

			const string charProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	char Test { get; set; }
}
";
			yield return [charProperty, typeof (Char)];

			const string boolProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	bool Test { get; set; }
}
";
			yield return [boolProperty, typeof (Boolean)];

			const string sbyteProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	sbyte Test { get; set; }
}
";
			yield return [sbyteProperty, typeof (SByte)];

			const string byteProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	byte Test { get; set; }
}
";
			yield return [byteProperty, typeof (Byte)];

			const string singleProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	Single Test { get; set; }
}
";
			yield return [singleProperty, typeof (Single)];

			const string intProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	int Test { get; set; }
}
";
			yield return [intProperty, typeof (Int32)];

			const string uintProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	uint Test { get; set; }
}
";
			yield return [uintProperty, typeof (UInt32)];

			const string doubleProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	double Test { get; set; }
}
";
			yield return [doubleProperty, typeof (Double)];

			const string int64Property = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	long Test { get; set; }
}
";
			yield return [int64Property, typeof (Int64)];

			const string uint64Property = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	ulong Test { get; set; }
}
";
			yield return [uint64Property, typeof (UInt64)];

			const string intPtrProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	IntPtr Test { get; set; }
}
";
			yield return [intPtrProperty, typeof (IntPtr)];

			const string uIntPtrProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	UIntPtr Test { get; set; }
}
";
			yield return [uIntPtrProperty, typeof (UIntPtr)];

			const string nintProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	nint Test { get; set; }
}
";
			yield return [nintProperty, typeof (nint)];

			const string nuintProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	nuint Test { get; set; }
}
";
			yield return [nuintProperty, typeof (nuint)];

			const string floatProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	float Test { get; set; }
}
";
			yield return [floatProperty, typeof (float)];

			const string stringProperty = @"
using System;
using System.Runtime.InteropServices;

namespace NS;

public class TestClass {
	string Test { get; set; }
}
";
			yield return [stringProperty, typeof (string)];

		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryGetBuiltInTypeSize>]
	public void TryGetBuiltInTypeSizeTests (ApplePlatform platform, string inputText, Type type)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		// 32bit
		var expectedResult = Stret.IsBuiltInType (type, false, out var expectedTypeSize);
		var result = symbol.Type.TryGetBuiltInTypeSize (false, out var returnTypeSize);
		Assert.Equal (expectedResult, result);
		Assert.Equal (expectedTypeSize, returnTypeSize);

		// 64bit
		var expectedResult64 = Stret.IsBuiltInType (type, false, out var expectedTypeSize64);
		var result64 = symbol.Type.TryGetBuiltInTypeSize (false, out var returnTypeSize64);
		Assert.Equal (expectedResult64, result64);
		Assert.Equal (expectedTypeSize64, returnTypeSize64);
	}
}
