using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class PropertyCodeChangeTests : BaseGeneratorTestClass {
	[Fact]
	public void CompareDiffName ()
	{
		var x = new PropertyCodeChange ("First", "string", [], [], []);
		var y = new PropertyCodeChange ("Second", "string", [], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffType ()
	{
		var x = new PropertyCodeChange ("First", "string", [], [], []);
		var y = new PropertyCodeChange ("First", "int", [], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffAttrs ()
	{
		var x = new PropertyCodeChange ("First", "string", [
			new ("Attr1"),
			new ("Attr2"),
		], [], []);
		var y = new PropertyCodeChange ("First", "int", [
			new ("Attr2"),
		], [], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffModifiers ()
	{
		var x = new PropertyCodeChange ("First", "string", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.AbstractKeyword)
		], []);
		var y = new PropertyCodeChange ("First", "int", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], []);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareDiffAccessors ()
	{
		var x = new PropertyCodeChange ("First", "string", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		]);
		var y = new PropertyCodeChange ("First", "int", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, [], []),
		]);

		Assert.False (x.Equals (y));
		Assert.False (y.Equals (x));
		Assert.False (x == y);
		Assert.True (x != y);
	}

	[Fact]
	public void CompareEquals ()
	{
		var x = new PropertyCodeChange ("First", "string", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		]);
		var y = new PropertyCodeChange ("First", "string", [
			new ("Attr1"),
			new ("Attr2"),
		], [
			SyntaxFactory.Token (SyntaxKind.PublicKeyword)
		], [
			new (AccessorKind.Getter, [], []),
			new (AccessorKind.Setter, [], []),
		]);

		Assert.True (x.Equals (y));
		Assert.True (y.Equals (x));
		Assert.True (x == y);
		Assert.False (x != y);
	}

	class TestDataFromPropertyDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string automaticGetter = @"
using System;

namespace Test;

public class TestClass {

	public string Name { get; }
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					automaticGetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], [])
						])
				];
			}

			const string automaticGetterSetter = @"
using System;

namespace Test;

public class TestClass {

	internal string Name { get; set; }
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					automaticGetterSetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.InternalKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
							new (AccessorKind.Setter, [], [])
						])
				];
			}

			const string manualGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name { get { return name; } }
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					manualGetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
						])
				];
			}

			const string expressionGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name => name;
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					expressionGetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
						])
				];
			}

			const string expressionGetterSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get => name;
		set => name = value;
	}
}
";
			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					expressionGetterSetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
							new (AccessorKind.Setter, [], []),
						])
				];
			}


			const string manualGetterSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get { return name; }
		set { name = value; }
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					manualGetterSetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
							new (AccessorKind.Setter, [], []),
						])
				];
			}

			const string internalSetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name {
		get { return name; }
		internal set { name = value; }
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					internalSetter,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
							new (AccessorKind.Setter, [], [
								SyntaxFactory.Token (SyntaxKind.InternalKeyword),
							]),
						])
				];
			}

			const string propertyWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		get { return name; }
		set { name = value; }
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					propertyWithAttribute,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
						],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [], []),
							new (AccessorKind.Setter, [], []),
						])
				];
			}

			const string propertyGetterWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get { return name; }
		set { name = value; }
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					propertyGetterWithAttribute,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
						],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], []),
							new (AccessorKind.Setter, [], []),
						])
				];
			}

			const string propertyWithGetterAndSetterWithAttribute = @"
using System.Runtime.Versioning;
namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get { return name; }
		[SupportedOSPlatform (""ios18.0"")]
		set { name = value; }
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					propertyWithGetterAndSetterWithAttribute,
					new PropertyCodeChange (
						name: "Name",
						type: "string",
						attributes: [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
						],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], []),
							new (AccessorKind.Setter, [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							], []),
						])
				];
			}

			const string propertyWithCustomType = @"
using System.Runtime.Versioning;

namespace Utils {
	public class MyClass {}
}

namespace Test {
	public class TestClass {
		Utils.MyClass name = new (); 

		[SupportedOSPlatform (""ios"")]
		public Utils.MyClass Name {
			[SupportedOSPlatform (""ios17.0"")]
			get { return name; }
			[SupportedOSPlatform (""ios18.0"")]
			set { name = value; }
		}
	}
}
";

			foreach (var platform in Configuration.GetIncludedPlatforms ()) {
				yield return [
					platform,
					propertyWithCustomType,
					new PropertyCodeChange (
						name: "Name",
						type: "Utils.MyClass",
						attributes: [
							new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios"]),
						],
						modifiers: [
							SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						],
						accessors: [
							new (AccessorKind.Getter, [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
							], []),
							new (AccessorKind.Setter, [
								new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios18.0"]),
							], []),
						])
				];
			}
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataFromPropertyDeclaration))]
	void FromPropertyDeclaration (ApplePlatform platform, string inputText, PropertyCodeChange expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof (FromPropertyDeclaration),
			platform, inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (PropertyCodeChange.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
