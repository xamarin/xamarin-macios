// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.DataModel.PropertyTests;

public class FromDeclarationTests : BaseGeneratorTestClass {

	class TestDataFromPropertyDeclaration : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string automaticGetter = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	public string Name { get; }
}
";
			yield return [
				automaticGetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					ExportPropertyData = new (selector: "name"),
				}
			];

			const string marshallNativeException = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"", Property.MarshalNativeExceptions)]
	public string Name { get; }
}
";
			yield return [
				marshallNativeException,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [
						new ("ObjCBindings.ExportAttribute<ObjCBindings.Property>", ["name", "ObjCBindings.Property.MarshalNativeExceptions"])
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					ExportPropertyData = new (selector: "name", ArgumentSemantic.None, ObjCBindings.Property.MarshalNativeExceptions),
				}
			];

			const string valueTypeProperty = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	public int Name { get; }
}
";
			yield return [
				valueTypeProperty,
				new Property (
					name: "Name",
					returnType: ReturnTypeForInt (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					ExportPropertyData = new (selector: "name"),
				},
			];

			const string automaticGetterExportData = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	public string Name { 
		[Export<Property>(""myName"")]
		get; 
	}
}
";
			yield return [
				automaticGetterExportData,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: new (selector: "myName"),
							attributes: [
								new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["myName"]),
							],
							modifiers: []
						)
					]
				) {
					ExportPropertyData = new (selector: "name"),
				},
			];

			const string automaticGetterSetter = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	internal string Name { get; set; }
}
";

			yield return [
				automaticGetterSetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.InternalKeyword),
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
						)
					]) {
					ExportPropertyData = new (selector: "name"),
				},
			];

			const string automaticGetterSetterExportData = @"
using System;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	internal string Name { 
		[Export<Property>(""myName"")]
		get; 
		[Export<Property>(""setMyName"")]
		set; 
	}
}
";

			yield return [
				automaticGetterSetterExportData,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.InternalKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: new (selector: "myName"),
							attributes: [
								new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["myName"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: new (),
							exportPropertyData: new (selector: "setMyName"),
							attributes: [
								new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["setMyName"]),
							],
							modifiers: []
						)
					]) {
					ExportPropertyData = new (selector: "name"),
				},
			];

			const string manualGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name { get { return name; } }
}
";

			yield return [
				manualGetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string expressionGetter = @"
namespace Test;

public class TestClass {
	const string name = ""Test"";
	public string Name => name;
}
";

			yield return [
				expressionGetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

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
			yield return [
				expressionGetterSetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
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
					])
			];

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

			yield return [
				manualGetterSetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
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
					])
			];
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

			yield return [
				internalSetter,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: new (),
					attributes: [],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
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
							modifiers: [
								SyntaxFactory.Token (kind: SyntaxKind.InternalKeyword),
							]
						),
					])
			];

			const string propertyWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

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

			var propertyAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			propertyAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios"));

			yield return [
				propertyWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string propertyGetterWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

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

			var getterAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			var setterAvailabilityBuilder = SymbolAvailability.CreateBuilder ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			yield return [
				propertyGetterWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string propertyWithGetterAndSetterWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

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

			getterAvailabilityBuilder.Clear ();
			setterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			setterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios18.0"));

			yield return [
				propertyWithGetterAndSetterWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: setterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios18.0"]),
							],
							modifiers: []
						),
					])
			];

			const string propertyWithCustomType = @"
using System.Runtime.Versioning;
using ObjCBindings;

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
			getterAvailabilityBuilder.Clear ();
			setterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			setterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios18.0"));

			yield return [
				propertyWithCustomType,
				new Property (
					name: "Name",
					returnType: ReturnTypeForClass ("Utils.MyClass"),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: setterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios18.0"]),
							],
							modifiers: []
						),
					])
			];

			const string autoPropertyGetterWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get;
		set;
	}
}
";
			getterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			yield return [
				autoPropertyGetterWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string staticAutoPropertyGetterWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public static string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get;
		set;
	}
}
";
			getterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			yield return [
				staticAutoPropertyGetterWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (kind: SyntaxKind.StaticKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string partialStaticAutoPropertyGetterWithAttribute = @"
using System.Runtime.Versioning;
using ObjCBindings;

namespace Test;

public class TestClass {
	const string name = ""Test"";

	[SupportedOSPlatform (""ios"")]
	public static partial string Name {
		[SupportedOSPlatform (""ios17.0"")]
		get;
		set;
	}
}
";
			getterAvailabilityBuilder.Clear ();
			getterAvailabilityBuilder.Add (supportedPlatform: new SupportedOSPlatformData (platformName: "ios17.0"));
			yield return [
				partialStaticAutoPropertyGetterWithAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForString (),
					symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
					attributes: [
						new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
						SyntaxFactory.Token (kind: SyntaxKind.StaticKeyword),
						SyntaxFactory.Token (kind: SyntaxKind.PartialKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: getterAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [
								new (name: "System.Runtime.Versioning.SupportedOSPlatformAttribute", arguments: ["ios17.0"]),
							],
							modifiers: []
						),
						new (
							accessorKind: AccessorKind.Setter,
							symbolAvailability: propertyAvailabilityBuilder.ToImmutable (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						),
					])
			];

			const string nsObjectProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	public NSObject Name { get; }
}
";
			yield return [
				nsObjectProperty,
				new Property (
					name: "Name",
					returnType: ReturnTypeForNSObject (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					NeedsBackingField = true,
					RequiresDirtyCheck = true,
					ExportPropertyData = new (selector: "name"),
				}
			];

			const string nsObjectArrayProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name"")]
	public NSObject[] Name { get; }
}
";
			yield return [
				nsObjectArrayProperty,
				new Property (
					name: "Name",
					returnType: ReturnTypeForArray ("Foundation.NSObject"),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					NeedsBackingField = true,
					RequiresDirtyCheck = true,
					ExportPropertyData = new (selector: "name"),
				}
			];

			const string bindFromAttribute = @"
using System;
using Foundation;
using ObjCBindings;

namespace Test;

public class TestClass {

	[Export<Property>(""name""), BindFrom (typeof(NSNumber))]
	public int Name { get; }
}
";
			yield return [
				bindFromAttribute,
				new Property (
					name: "Name",
					returnType: ReturnTypeForInt (),
					symbolAvailability: new (),
					attributes: [
						new (name: "ObjCBindings.ExportAttribute<ObjCBindings.Property>", arguments: ["name"]),
						new (name: "ObjCBindings.BindFromAttribute", arguments: ["Foundation.NSNumber"]),
					],
					modifiers: [
						SyntaxFactory.Token (kind: SyntaxKind.PublicKeyword),
					],
					accessors: [
						new (
							accessorKind: AccessorKind.Getter,
							symbolAvailability: new (),
							exportPropertyData: null,
							attributes: [],
							modifiers: []
						)
					]
				) {
					ExportPropertyData = new (selector: "name"),
					BindAs = new ("Foundation.NSNumber"),
				}
			];
		}

		IEnumerator IEnumerable.GetEnumerator ()
			=> GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFromPropertyDeclaration>]
	void FromPropertyDeclaration (ApplePlatform platform, string inputText, Property expected)
	{
		var (compilation, syntaxTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (syntaxTrees);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ().OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		Assert.True (Property.TryCreate (declaration, semanticModel, out var changes));
		Assert.NotNull (changes);
		Assert.Equal (expected, changes);
	}
}
