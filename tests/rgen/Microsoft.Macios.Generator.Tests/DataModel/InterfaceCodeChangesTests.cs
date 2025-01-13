// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using Property = ObjCBindings.Property;

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

[BindingType<Protocol>]
public partial interface IProtocol {
}
";

			yield return [
				emptyInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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

[BindingType<Protocol>]
internal partial interface IProtocol {
}
";

			yield return [
				internalInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				singlePropertyInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							returnType: new (type: "string", isReferenceType: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("name")
						}
					]
				}
			];

			const string singlePropertySmartEnumInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public enum MyEnum {
	First,
}

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial MyEnum Name { get; set; }
}
";

			yield return [
				singlePropertySmartEnumInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							returnType: new ("NS.MyEnum", isSmartEnum: true, isBlittable: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("name")
						}
					]
				}
			];

			const string singlePropertyEnumInterface = @"
using ObjCBindings;

namespace NS;

public enum MyEnum {
	First,
}

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial MyEnum Name { get; set; }
}
";

			yield return [
				singlePropertyEnumInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							returnType: new ("NS.MyEnum", isBlittable: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("name")
						}
					]
				}
			];

			const string notificationPropertyInterface = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial interface IProtocol {
	[Field<Property> (""name"", Property.Notification)]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				notificationPropertyInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
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
							returnType: new ("string", isReferenceType: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.FieldAttribute<ObjCBindings.Property>", ["name", "ObjCBindings.Property.Notification"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportFieldData = new ("name", Property.Notification)
						}
					]
				}
			];

			const string multiPropertyInterfaceMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Property> (""name"")]
	public partial string Name { get; set; } = string.Empty;

	public partial string Surname { get; set; } = string.Empty;
}
";

			yield return [
				multiPropertyInterfaceMissingExport,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							returnType: new ("string", isReferenceType: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("name")
						}
					]
				}
			];

			const string multiPropertyInterface = @"
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
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
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							returnType: new ("string", isReferenceType: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("name")
						},
						new (
							name: "Surname",
							returnType: new ("string", isReferenceType: true),
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["surname"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							accessors: [
								new (
									accessorKind: AccessorKind.Getter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Setter,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
							]
						) {
							ExportPropertyData = new ("surname")
						},
					]
				}
			];

			const string singleMethodInterface = @"
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);
}
";

			yield return [
				singleMethodInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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
							returnType: new ("void"),
							symbolAvailability: new (),
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false),
							]
						),
					]
				}
			];

			const string multiMethodInterfaceMissingExport = @"
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);

	public void SetSurname (string inSurname) {}
}
";

			yield return [
				multiMethodInterfaceMissingExport,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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
							returnType: new ("void"),
							symbolAvailability: new (),
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false),
							]
						),
					]
				}
			];


			const string multiMethodInterface = @"
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {
	[Export<Method> (""withName:"")]
	public partial void SetName (string name);

	[Export<Method> (""withSurname:"")]
	public partial void SetSurname (string inSurname);
}
";
			yield return [
				multiMethodInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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
							returnType: new ("void"),
							symbolAvailability: new (),
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false),
							]
						),
						new (
							type: "NS.IProtocol",
							name: "SetSurname",
							returnType: new ("void"),
							symbolAvailability: new (),
							exportMethodData: new ("withSurname:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withSurname:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "inSurname", isBlittable: false),
							]
						),
					]
				}
			];

			const string singleEventInterface = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {

	public event EventHandler Changed { add; remove; }
}
";

			yield return [
				singleEventInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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
								new (
									accessorKind: AccessorKind.Add,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Remove,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								)
							])
					],
				}
			];

			const string multiEventInterface = @"
using System;
using ObjCBindings;

namespace NS;

[BindingType<Protocol>]
public partial interface IProtocol {

	public event EventHandler Changed { add; remove; }

	public event EventHandler Removed { add; remove; }
}
";

			yield return [
				multiEventInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Protocol> ()),
					name: "IProtocol",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.IProtocol",
					symbolAvailability: new ()
				) {
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Protocol>")
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
								new (
									accessorKind: AccessorKind.Add,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Remove,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								)
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
								new (
									accessorKind: AccessorKind.Add,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								),
								new (
									accessorKind: AccessorKind.Remove,
									symbolAvailability: new (),
									exportPropertyData: null,
									attributes: [],
									modifiers: []
								)
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
