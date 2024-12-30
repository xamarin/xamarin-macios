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

public class InterfaceCodeChangesTests : BaseGeneratorTestClass {
	readonly CodeChangesEqualityComparer comparer = new ();

	class TestDataCodeChangesFromClassDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string emptyInterface = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
}
";

			yield return [
				emptyInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCBindings", "ObjCRuntime" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
				}
			];

			const string internalInterface = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType]
internal partial interface IProtocol {
}
";

			yield return [
				internalInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
          UsingDirectives = new HashSet<string> { "Foundation", "ObjCBindings", "ObjCRuntime" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.InternalKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
				}
			];

			const string singlePropertyInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				singlePropertyInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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

			const string multiPropertyInterfaceMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyInterfaceMissingExport,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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

			const string multiPropertyInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	[Export<Property> (""surname"")]
	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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

			const string singleMethodInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);
}
";

			yield return [
				singleMethodInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.IProtocol",
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
								new (0, "string", "inName")
							]
						),
					]
				}
			];

			const string multiMethodInterfaceMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);

	public void SetSurname (string inSurname) {}
}
";

			yield return [
				multiMethodInterfaceMissingExport,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.IProtocol",
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
								new (0, "string", "inName")
							]
						),
					]
				}
			];


			const string multiMethodInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string inName);

	[Export<Method> (""withSurname:"")]
	public partial void SetSurname (string inSurname);
}
";
			yield return [
				multiMethodInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Methods = [
						new (
							type: "NS.IProtocol",
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
								new (0, "string", "inName")
							]
						),
						new (
							type: "NS.IProtocol",
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

			const string singleEventInterface = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {

	public event EventHandler Changed { add; remove; }
}
";

			yield return [
				singleEventInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "System", "ObjCBindings" },
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

			const string multiEventInterface = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {

	public event EventHandler Changed { add; remove; }

	public event EventHandler Removed { add; remove; }
}
";

			yield return [
				multiEventInterface,
				new CodeChanges (
					bindingType: BindingType.Protocol,
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute")
					],
					UsingDirectives = new HashSet<string> { "System", "ObjCBindings" },
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
	void CodeChangesFromInterfaceDeclaration (ApplePlatform platform, string inputText, CodeChanges expected)
	{
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<InterfaceDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		var changes = CodeChanges.FromDeclaration (node, semanticModel);
		Assert.NotNull (changes);
		Assert.Equal (expected, changes.Value, comparer);
	}
}
