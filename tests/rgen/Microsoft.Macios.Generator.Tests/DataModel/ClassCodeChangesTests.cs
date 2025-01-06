#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCBindings;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using Property = ObjCBindings.Property;

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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCRuntime", "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					]
				}
			];

			const string emptyClassWithBase = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

[BindingType<Class>]
public partial class MyClass : NSObject {
}
";

			yield return [
				emptyClassWithBase,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "Foundation.NSObject",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCRuntime", "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					]
				}
			];

			const string emptyClassWithBaseWithInterface = @"
using Foundation;
using ObjCRuntime;
using ObjCBindings;

namespace NS;

public interface IMyInterface {}

[BindingType<Class>]
public partial class MyClass : NSObject, IMyInterface {
}
";

			yield return [
				emptyClassWithBaseWithInterface,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "Foundation.NSObject",
					Interfaces = ["NS.IMyInterface"],
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCRuntime", "ObjCBindings" },
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCRuntime", "ObjCBindings" },
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: builder.ToImmutable ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>"),
						new ("System.Runtime.Versioning.UnsupportedOSPlatformAttribute", ["macos"]),
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["tvos17.0"]),
						new ("System.Runtime.Versioning.SupportedOSPlatformAttribute", ["ios17.0"]),
					],
					UsingDirectives = new HashSet<string> { "Foundation", "ObjCRuntime", "ObjCBindings", "System.Runtime.Versioning" },
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Constructors = [
						new (
							type: "MyClass",
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Constructors = [
						new (
							type: "MyClass",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: []
						),
						new (
							type: "MyClass",
							symbolAvailability: new (),
							attributes: [],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword)
							],
							parameters: [
								new (
									position: 0,
									type: "string",
									name: "name",
									isBlittable: false
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
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
							isBlittable: false,
							isSmartEnum: false,
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

			const string singlePropertySmartEnumClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public enum MyEnum {
	None = 0,
}

[BindingType<Class>]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial MyEnum Name { get; set; }
}
";

			yield return [
				singlePropertySmartEnumClass,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							type: "NS.MyEnum",
							isBlittable: true,
							isSmartEnum: true,
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

			const string singlePropertyEnumClass = @"
using ObjCBindings;

namespace NS;

public enum MyEnum {
	None = 0,
}

[BindingType<Class>]
public partial class MyClass {
	[Export<Property> (""name"")]
	public partial MyEnum Name { get; set; }
}
";

			yield return [
				singlePropertyEnumClass,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
					Modifiers = [
						SyntaxFactory.Token (SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (SyntaxKind.PartialKeyword)
					],
					Properties = [
						new (
							name: "Name",
							type: "NS.MyEnum",
							isBlittable: true,
							isSmartEnum: false,
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

			const string notificationPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Property> (""name"", Property.Notification)]
	public partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				notificationPropertyClass,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
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
							isBlittable: false,
							isSmartEnum: false,
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name", "ObjCBindings.Property.Notification"])
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

							ExportPropertyData = new ("name", ArgumentSemantic.None, Property.Notification)
						}
					]
				}
			];

			const string fieldPropertyClass = @"
using ObjCBindings;

namespace NS;

[BindingType]
public partial class MyClass {
	[Export<Field> (""CONSTANT"")]
	public static partial string Name { get; set; } = string.Empty;
}
";

			yield return [
				fieldPropertyClass,
				new CodeChanges (
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
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
							isBlittable: false,
							isSmartEnum: false,
							symbolAvailability: new (),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Field>", ["CONSTANT"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.StaticKeyword),
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

							ExportFieldData = new ("CONSTANT")
						}
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
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
							isBlittable: false,
							isSmartEnum: false,
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
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
							isBlittable: false,
							isSmartEnum: false,
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
							type: "string",
							isBlittable: false,
							isSmartEnum: false,
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false)
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false)
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings" },
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
							exportMethodData: new ("withName:"),
							attributes: [
								new ("ObjCBindings.ExportAttribute<ObjCBindings.Method>", ["withName:"])
							],
							modifiers: [
								SyntaxFactory.Token (SyntaxKind.PublicKeyword),
								SyntaxFactory.Token (SyntaxKind.PartialKeyword),
							],
							parameters: [
								new (position: 0, type: "string", name: "name", isBlittable: false)
							]
						),
						new (
							type: "NS.MyClass",
							name: "SetSurname",
							returnType: "void",
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
								new (position: 0, type: "string", name: "inSurname", isBlittable: false)
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings", "System" },
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
					bindingInfo: new (new BindingTypeData<Class> ()),
					name: "MyClass",
					@namespace: ["NS"],
					fullyQualifiedSymbol: "NS.MyClass",
					symbolAvailability: new ()
				) {
					Base = "object",
					Interfaces = ImmutableArray<string>.Empty,
					Attributes = [
						new ("ObjCBindings.BindingTypeAttribute<ObjCBindings.Class>")
					],
					UsingDirectives = new HashSet<string> { "ObjCBindings", "System" },
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

	[Fact]
	public void IsStaticPropertyTest ()
	{
		var changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());

		Assert.False (changes.IsStatic);

		changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Modifiers = [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.StaticKeyword),
			]
		};

		Assert.True (changes.IsStatic);
	}

	[Fact]
	public void IsPartialPropertyTest ()
	{
		var changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());

		Assert.False (changes.IsPartial);

		changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Modifiers = [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.PartialKeyword),
			]
		};

		Assert.True (changes.IsPartial);
	}

	[Fact]
	public void IsAbstractPropertyTest ()
	{
		var changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ());

		Assert.False (changes.IsAbstract);

		changes = new CodeChanges (
			bindingInfo: new (new BindingTypeData<Class> ()),
			name: "name1",
			@namespace: ["NS"],
			fullyQualifiedSymbol: "NS.name1",
			symbolAvailability: new ()) {
			Modifiers = [
				SyntaxFactory.Token (SyntaxKind.PublicKeyword),
				SyntaxFactory.Token (SyntaxKind.AbstractKeyword),
			]
		};

		Assert.True (changes.IsAbstract);
	}
}
