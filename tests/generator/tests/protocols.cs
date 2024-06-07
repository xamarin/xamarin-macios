using System;
using Foundation;
using ObjCRuntime;

namespace Protocols {
	[Protocol]
	interface ProtocolWithConstructors {
		/// <summary>init</summary>
		[Abstract]
		[Export ("init")]
		NativeHandle Constructor ();

		/// <summary>initWithValue:</summary>
		[Export ("initWithValue:")]
		NativeHandle Constructor (string p0);

		/// <summary>initWithError:</summary>
		[Export ("initWithError:")]
		NativeHandle Constructor (out NSError error);

		/// <summary>initWithCustomName:</summary>
		[Bind ("Create")]
		[Export ("initWithCustomName:")]
		NativeHandle Constructor (NSDate error);
	}

	[Protocol]
	interface ProtocolWithStaticMembers {
		/// <summary>member</summary>
		[Static]
		[Abstract]
		[Export ("member")]
		NativeHandle Method ();

		/// <summary>memberWithValue</summary>
		[Static]
		[Export ("memberWithValue:")]
		int Method (string p0);

		/// <summary>methodWithError</summary>
		[Static]
		[Export ("methodWithError:")]
		string Method (out NSError error);

		/// <summary>methodWitHDate</summary>
		[Static]
		[Export ("methodWithDate:")]
		NSString Method (NSDate error);

		/// <summary>property</summary>
		[Static]
		[Export ("property")]
		bool Property { get; set; }

		/// <summary>stringProperty</summary>
		[Static]
		[Export ("stringProperty")]
		string StringProperty { get; set; }

		/// <summary>dateProperty</summary>
		[Static]
		[Export ("dateProperty")]
		NSDate DateProperty { get; set; }
	}

	[Protocol]
	interface OptionalProtocolCompat {
		[Export ("optionalMethod:outValue:refValue:")]
		int OptionalMethod (int @event, out byte outValue, ref short refValue);

		[Export ("optionalProperty")]
		int OptionalProperty { get; set; }

		[Internal]
		[Export ("internalOptionalMethod:outValue:refValue:")]
		int InternalOptionalMethod (int @event, out byte outValue, ref short refValue);

		[Internal]
		[Export ("internalOptionalProperty")]
		int InternalOptionalProperty { get; set; }

		[Static]
		[Export ("staticOptionalMethod:outValue:refValue:")]
		int StaticOptionalMethod (int @event, out byte outValue, ref short refValue);

		[Static]
		[Export ("staticOptionalProperty")]
		int StaticOptionalProperty { get; set; }

		[Export ("optionalPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject OptionalPropertyWeakSemantics { get; set; }

		[Static]
		[Export ("staticOptionalPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject StaticOptionalPropertyWeakSemantics { get; set; }
	}

	[Protocol]
	interface RequiredProtocolCompat {
		[Abstract]
		[Export ("requiredMethod:outValue:refValue:")]
		int RequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Export ("requiredProperty")]
		int RequiredProperty { get; set; }

		[Abstract]
		[Internal]
		[Export ("internalRequiredMethod:outValue:refValue:")]
		int InternalRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Internal]
		[Export ("internalRequiredProperty")]
		int InternalRequiredProperty { get; set; }

		[Abstract]
		[Static]
		[Export ("staticRequiredMethod:outValue:refValue:")]
		int StaticRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Static]
		[Export ("staticRequiredProperty")]
		int StaticRequiredProperty { get; set; }

		[Abstract]
		[Export ("requiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject RequiredPropertyWeakSemantics { get; set; }

		[Abstract]
		[Static]
		[Export ("staticRequiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject StaticRequiredPropertyWeakSemantics { get; set; }
	}

	[Protocol]
	interface RequiredProtocolCompatWithExtensions {
		[Abstract (GenerateExtensionMethod = true)]
		[Export ("requiredMethod:outValue:refValue:")]
		int RequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract (GenerateExtensionMethod = true)]
		[Export ("requiredProperty")]
		int RequiredProperty { get; set; }

		[Abstract (GenerateExtensionMethod = true)]
		[Internal]
		[Export ("internalRequiredMethod:outValue:refValue:")]
		int InternalRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract (GenerateExtensionMethod = true)]
		[Internal]
		[Export ("internalRequiredProperty")]
		int InternalRequiredProperty { get; set; }

		[Abstract (GenerateExtensionMethod = true)]
		[Static]
		[Export ("staticRequiredMethod:outValue:refValue:")]
		int StaticRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract (GenerateExtensionMethod = true)]
		[Static]
		[Export ("staticRequiredProperty")]
		int StaticRequiredProperty { get; set; }

		[Abstract (GenerateExtensionMethod = true)]
		[Export ("requiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject RequiredPropertyWeakSemantics { get; set; }

		[Abstract (GenerateExtensionMethod = true)]
		[Static]
		[Export ("staticRequiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject StaticRequiredPropertyWeakSemantics { get; set; }
	}

	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface OptionalProtocol {
		[Export ("optionalMethod:outValue:refValue:")]
		int OptionalMethod (int @event, out byte outValue, ref short refValue);

		[Export ("optionalProperty")]
		int OptionalProperty { get; set; }

		[Export ("nullableOptionalProperty")]
		[NullAllowed]
		string NullableOptionalProperty { get; set; }

		[Internal]
		[Export ("internalOptionalMethod:outValue:refValue:")]
		int InternalOptionalMethod (int @event, out byte outValue, ref short refValue);

		[Internal]
		[Export ("internalOptionalProperty")]
		int InternalOptionalProperty { get; set; }

		[Static]
		[Export ("staticOptionalMethod:outValue:refValue:")]
		int StaticOptionalMethod (int @event, out byte outValue, ref short refValue);

		[Static]
		[Export ("staticOptionalProperty")]
		int StaticOptionalProperty { get; set; }

		[Static]
		[Export ("nullableStaticOptionalProperty")]
		[NullAllowed]
		string NullableStaticOptionalProperty { get; set; }

		[Export ("optionalPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject OptionalPropertyWeakSemantics { get; set; }

		[Static]
		[Export ("staticOptionalPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject StaticOptionalPropertyWeakSemantics { get; set; }

		[Async]
		[Export ("optionalAsyncMethod:")]
		void OptionalAsyncMethod (Action callback);

		[Async]
		[Static]
		[Export ("staticOptionalAsyncMethod:")]
		void StaticOptionalAsyncMethod (Action callback);
	}

	[Protocol (BackwardsCompatibleCodeGeneration = false)]
	interface RequiredProtocol {
		[Abstract]
		[Export ("requiredMethod:outValue:refValue:")]
		int RequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Export ("requiredProperty")]
		int RequiredProperty { get; set; }

		[Abstract]
		[Export ("nullableRequiredProperty")]
		[NullAllowed]
		string NullableRequiredProperty { get; set; }

		[Abstract]
		[Internal]
		[Export ("internalRequiredMethod:outValue:refValue:")]
		int InternalRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Internal]
		[Export ("internalRequiredProperty")]
		int InternalRequiredProperty { get; set; }

		[Abstract]
		[Static]
		[Export ("staticRequiredMethod:outValue:refValue:")]
		int StaticRequiredMethod (int @event, out byte outValue, ref short refValue);

		[Abstract]
		[Static]
		[Export ("staticRequiredProperty")]
		int StaticRequiredProperty { get; set; }

		[Abstract]
		[Static]
		[Export ("nullableStaticRequiredProperty")]
		[NullAllowed]
		string NullableStaticRequiredProperty { get; set; }

		[Abstract]
		[Export ("requiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject RequiredPropertyWeakSemantics { get; set; }

		[Abstract]
		[Static]
		[Export ("staticRequiredPropertyWeakSemantics", ArgumentSemantic.Weak)]
		NSObject StaticRequiredPropertyWeakSemantics { get; set; }

		[Abstract]
		[Async]
		[Export ("requiredAsyncMethod:")]
		void RequiredAsyncMethod (Action callback);

		[Abstract]
		[Async]
		[Static]
		[Export ("staticRequiredAsyncMethod:")]
		void StaticRequiredAsyncMethod (Action callback);
	}

	[BaseType (typeof (NSObject))]
	interface MyObject : OptionalProtocol, RequiredProtocol {
	}

	[BaseType (typeof (NSObject))]
	interface MyObject2 : OptionalProtocolCompat, RequiredProtocolCompat {
	}
}
