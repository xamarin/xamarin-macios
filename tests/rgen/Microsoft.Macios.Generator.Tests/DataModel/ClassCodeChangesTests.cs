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

public class ClassCodeChangesTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer comparer = new ();

	class TestDataCodeChangesFromClassDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string emptyClass = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
}
";

			yield return [
				emptyClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					]
				}
			];

			const string singleConstructorClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	string name = string.Empty;

	public MyClass () {}
}
";

			yield return [
				singleConstructorClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Constructors = [
						new (
							type: "NS.MyClass",
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

[BindingType]
public partial class MyClass {
	string name = string.Empty;

	public MyClass () {}

	public MyClass(string inName) {
		name = inName;
	}
}
";

			yield return [
				multiConstructorClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Constructors = [
						new (
							type: "NS.MyClass",
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: []
						),
						new (
							type: "NS.MyClass",
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: [
								new (
									position: 0,
									type: "string",
									name: "inName"
								)
							]
						),
					]
				}
			];

			const string singlePropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				singlePropertyClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, [], []),
								new (AccessorKind.Setter, [], []),
							]
						)
					]
				}
			];

			const string multiPropertyClassMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyClassMissingExport,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, [], []),
								new (AccessorKind.Setter, [], []),
							]
						)
					]
				}
			];

			const string multiPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
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
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Properties = [
						new (
							name: "Name",
							type: "string",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, [], []),
								new (AccessorKind.Setter, [], []),
							]
						),
						new (
							name: "Surname",
							type: "string",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["surname"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (AccessorKind.Getter, [], []),
								new (AccessorKind.Setter, [], []),
							]
						),
					]
				}
			];

			const string singleMethodClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);
}
";

			yield return [
				singleMethodClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "inName")
							]
						),
					]
				}
			];

			const string multiMethodClassMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);

	public void SetSurname (string inSurname) {}
}
";

			yield return [
				multiMethodClassMissingExport,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "inName")
							]
						),
					]
				}
			];


			const string multiMethodClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);

	[Export<Method> (""withSurname:"")]
	public partial void SetSurname (string inSurname);
}
";
			yield return [
				multiMethodClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Methods = [
						new (
							type: "NS.MyClass",
							name: "SetName",
							returnType: "void",
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (0, "string", "inName")
							]
						),
						new (
							type: "NS.MyClass",
							name: "SetSurname",
							returnType: "void",
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

[BindingType]
public partial class MyClass {

	public event EventHandler Changed { add; remove; }
}
";

			yield return [
				singleEventClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Events = [
						new (
							name: "Changed",
							type: "System.EventHandler",
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, [], []),
								new (AccessorKind.Remove, [], [])
							])
					],
				}
			];

			const string multiEventClass = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {

	public event EventHandler Changed { add; remove; }

	public event EventHandler Removed { add; remove; }
}
";

			yield return [
				multiEventClass,
				new CodeChanges (
					bindingType: BindingType.Class,
					fullyQualifiedSymbol: "NS.MyClass"
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					Events = [
						new (
							name: "Changed",
							type: "System.EventHandler",
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, [], []),
								new (AccessorKind.Remove, [], [])
							]
						),
						new (
							name: "Removed",
							type: "System.EventHandler",
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
							],
							accessors: [
								new (AccessorKind.Add, [], []),
								new (AccessorKind.Remove, [], [])
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
		var (compilation, sourceTrees) =
			CreateCompilation (nameof (CodeChangesFromClassDeclaration), platform, inputText);
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
