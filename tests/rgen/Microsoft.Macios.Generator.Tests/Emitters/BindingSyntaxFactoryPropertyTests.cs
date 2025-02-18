// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#pragma warning disable APL0003

using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.DataModel;
using ObjCRuntime;
using Xunit;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;
using static Microsoft.Macios.Generator.Tests.TestDataFactory;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class BindingSyntaxFactoryPropertyTests {


	class TestDataPropertyInvocationsGetterTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{

			var property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForString (),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")), false)!;",
				"CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")), false)!;"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForString (isNullable: true),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")), false);",
				"CFString.FromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")), false);"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForArray ("string"),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")), false)!;",
				"CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")), false)!;"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForArray ("string", isNullable: true),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")), false);",
				"CFArray.StringArrayFromHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")), false);"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForInt (),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"global::ObjCRuntime.Messaging.int_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\"));",
				"global::ObjCRuntime.Messaging.int_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\"));"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForInt (isUnsigned: true),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"global::ObjCRuntime.Messaging.uint_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\"));",
				"global::ObjCRuntime.Messaging.uint_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\"));"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForBool (),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"global::ObjCRuntime.Messaging.bool_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")) != 0;",
				"global::ObjCRuntime.Messaging.bool_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")) != 0;"
			];
			
			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForNSObject (),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};
			
			yield return [
				property,
				"ret = Runtime.GetNSObject<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")))!;",
				"ret = Runtime.GetNSObject<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")))!;"
			];
			
			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForNSObject (isNullable: true),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};
			
			yield return [
				property,
				"ret = Runtime.GetNSObject<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")));",
				"ret = Runtime.GetNSObject<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")));"
			];

			property = new Property (
				name: "MyProperty",
				returnType: ReturnTypeForArray("Foundation.NSObject", isNSObject: true),
				symbolAvailability: new (),
				attributes: [],
				modifiers: [],
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
				ExportPropertyData = new ("myProperty", ArgumentSemantic.None, ObjCBindings.Property.IsThreadSafe)
			};

			yield return [
				property,
				"ret = CFArray.ArrayFromHandle<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend (this.Handle, Selector.GetHandle (\"myProperty\")))!;",
				"ret = CFArray.ArrayFromHandle<Foundation.NSObject> (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper (this.Handle, Selector.GetHandle (\"myProperty\")))!;"
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataPropertyInvocationsGetterTests))]
	void PropertyInvocationsGetterTests (Property property, string getter, string superGetter)
	{
		var invocations = GetInvocations (property);
		var s = invocations.Getter.Send.ToString ();
		var ss = invocations.Getter.SendSuper.ToString ();
		Assert.Equal (getter, invocations.Getter.Send.ToString ());
		Assert.Equal (superGetter, invocations.Getter.SendSuper.ToString ());
	}
}

#pragma warning restore APL0003
