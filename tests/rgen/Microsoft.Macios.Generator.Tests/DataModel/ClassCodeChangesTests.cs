#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.DataModel;

public class ClassCodeChangesTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer comparer = new ();

	class TestDataCodeChangesFromClassDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var builder = SymbolAvailability.CreateBuilder ();
			builder.Add (new SupportedOSPlatformData ("ios17.0"));
			builder.Add (new SupportedOSPlatformData ("tvos17.0"));
			builder.Add (new UnsupportedOSPlatformData ("macos"));

			const string emptyClass = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
}
";

			yield return [
				emptyClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					]
				}
			];

			const string internalClass = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
internal partial class MyClass {
}
";

			yield return [
				internalClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.InternalKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					]
				}
			];

			const string emptyClassAvailability = @"
using System.Runtime.Versioning;
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
[SupportedOSPlatform (""ios17.0"")]
[SupportedOSPlatform (""tvos17.0"")]
[UnsupportedOSPlatform (""macos"")]
public partial class MyClass {
}
";

			yield return [
				emptyClassAvailability,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: builder.ToImmutable ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>"),
						new ("System.Runtime.Versioning.UnsupportedOSPlatformAttribute", ["macos"]),
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["tvos17.0"]),
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					]
				}
			];

			const string singleConstructorClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	string name = string.Empty;

	public MyClass () {}
}
";

			yield return [
				singleConstructorClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Constructors = [
						new (
							type: "NS.MyClass",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: []
						)
					]
				}
			];

			const string multiConstructorClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	string name = string.Empty;

	public MyClass () {}

	public MyClass(string name) {
		name = name;
	}
}
";

			yield return [
				multiConstructorClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Constructors = [
						new (
							type: "NS.MyClass",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: []
						),
						new (
							type: "NS.MyClass",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: [
								new (
									position: 0,
									type: "string",
									name: "name"
								)
							]
						),
					]
				}
			];

			const string singlePropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				singlePropertyClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, new (), [], []),
								new (AccessorKind.Setter, new (), [], []),
							]
						)
					]
				}
			];

			const string multiPropertyClassMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyClassMissingExport,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, new (), [], []),
								new (AccessorKind.Setter, new (), [], []),
							]
						)
					]
				}
			];

			const string multiPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	[Export<Property> (""surname"")]
	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, new (), [], []),
								new (AccessorKind.Setter, new (), [], []),
							]
						),
						new (
							name: "Surname",
							type: "string",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["surname"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, new (), [], []),
								new (AccessorKind.Setter, new (), [], []),
							]
						),
					]
				}
			];

			const string singleMethodClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);
}
";

			yield return [
				singleMethodClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "name")
							]
						),
					]
				}
			];

			const string multiMethodClassMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);

	public void SetSurname (string inSurname) {}
}
";

			yield return [
				multiMethodClassMissingExport,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "name")
							]
						),
					]
				}
			];


			const string multiMethodClass = @"
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);

	[Export<Method> (""withSurname:"")]
	public partial void SetSurname (string inSurname);
}
";
			yield return [
				multiMethodClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "name")
							]
						),
						new (
							type: "NS.MyClass",
							name: "SetSurname",
							returnType: "void",
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withSurname:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "inSurname")
							]
						),
					]
				}
			];

			const string singleEventClass = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {

	public event EventHandler Changed { add; remove; }
}
";

			yield return [
				singleEventClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Events = [
						new (
							name: "Changed",
							type: "System.EventHandler",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, new (), [], []),
								new (AccessorKind.Remove, new (), [], [])
							])
					],
				}
			];

			const string multiEventClass = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass {

	public event EventHandler Changed { add; remove; }

	public event EventHandler Removed { add; remove; }
}
";

			yield return [
				multiEventClass,
				new CodeChanges (
					bindingData: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Events = [
						new (
							name: "Changed",
							type: "System.EventHandler",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, new (), [], []),
								new (AccessorKind.Remove, new (), [], [])
							]
						),
						new (
							name: "Removed",
							type: "System.EventHandler",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, new (), [], []),
								new (AccessorKind.Remove, new (), [], [])
							]
						),
					],
				}
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataCodeChangesFromClassDeclaration>]
	void CodeChangesFromClassDeclaration (ApplePlatform platform, string inputText, CodeChanges expected)
	{
		var (compilation, sourceTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		var changes = CodeChanges.FromDeclaration (node, semanticModel);
		Assert.NotNull (changes);
		Assert.Equal (expected, changes.Value, comparer);
	}
}
