// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using ClangSharp;
using ClangSharp.Interop;
using Extrospection;
using Mono.Cecil;
using Sharpie.Bind;

namespace Xamarin.Tests;

[TestFixture]
[NonParallelizable]
public class NullabilityTests {
	const string Framework = "NullabilityTests";

	static void AddAttribute (ModuleDefinition module, Mono.Cecil.ICustomAttributeProvider provider, string name, object value)
	{
		var argumentType = value is byte ? module.TypeSystem.Byte : module.TypeSystem.String;
		var attributeType = new TypeReference ("Tests", name, module, module);
		var constructor = new MethodReference (".ctor", module.TypeSystem.Void, attributeType) { HasThis = true };
		constructor.Parameters.Add (new ParameterDefinition (argumentType));
		var attribute = new CustomAttribute (constructor);
		attribute.ConstructorArguments.Add (new CustomAttributeArgument (argumentType, value));
		provider.CustomAttributes.Add (attribute);
	}

	internal static void SetContext (ModuleDefinition module, Mono.Cecil.ICustomAttributeProvider provider, byte value)
	{
		foreach (var attribute in provider.CustomAttributes.Where (v => v.AttributeType.Name == "NullableContextAttribute").ToArray ())
			provider.CustomAttributes.Remove (attribute);
		AddAttribute (module, provider, "NullableContextAttribute", value);
		provider.CustomAttributes.Last ().AttributeType.Namespace = "System.Runtime.CompilerServices";
	}

	static string [] Check (string declarations, Action<ModuleDefinition, TypeDefinition>? prepare = null)
	{
		var directory = Cache.CreateTemporaryDirectory ();
		var headers = Path.Combine (directory, Framework + ".framework", "Headers");
		Directory.CreateDirectory (headers);
		var header = Path.Combine (headers, Framework + ".h");
		File.WriteAllText (header, declarations);

		using var resolver = new DefaultAssemblyResolver ();
		resolver.AddSearchDirectory (TestContext.CurrentContext.TestDirectory);
		resolver.AddSearchDirectory (RuntimeEnvironment.GetRuntimeDirectory ());
		using var assembly = AssemblyDefinition.ReadAssembly (Path.Combine (TestContext.CurrentContext.TestDirectory, "UnitTests.dll"),
			new ReaderParameters { AssemblyResolver = resolver });
		var module = assembly.MainModule;
		var fixture = module.GetType ("Xamarin.Tests.CallbackFixtures");
		AddAttribute (module, fixture, "RegisterAttribute", "Callbacks");
		prepare?.Invoke (module, fixture);
		var check = new NullabilityCheck (new BindingResult ());
		foreach (var method in fixture.Methods) {
			var selector = method.Name;
			if (method.IsGetter)
				selector = selector.Substring (4);
			else if (method.IsSetter)
				selector = "set" + selector.Substring (4);
			selector += new string (':', method.Parameters.Count);
			AddAttribute (module, method, "ExportAttribute", selector);
			check.VisitManagedMethod (method);
		}

		var previousPlatform = Helpers.Platform;
		Helpers.Platform = Platforms.macOS;
		Log.On (Framework).Clear ();
		try {
			using var index = CXIndex.Create ();
			var result = CXTranslationUnit.TryParse (index, header, ["-x", "objective-c", "-fblocks", "-Wno-objc-root-class"], [],
				CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes | CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes, out var handle);
			Assert.That (result, Is.EqualTo (CXErrorCode.CXError_Success), "Parsing synthetic header");
			using var unit = TranslationUnit.GetOrCreate (handle);
			Assert.That (unit, Is.Not.Null, "Translation unit");
			Assert.That (unit.Handle.NumDiagnostics, Is.Zero, "Synthetic header diagnostics");
			check.Visit (unit.TranslationUnitDecl);
			return Log.On (Framework).Distinct ().ToArray ();
		} finally {
			Log.On (Framework).Clear ();
			Helpers.Platform = previousPlatform;
			Directory.Delete (directory, true);
		}
	}

	static string Missing (string method, string location) =>
		$"!missing-null-allowed! '{method}' is missing a '?' on {location}";

	static string Extra (string method, string location) =>
		$"!extra-null-allowed! '{method}' has an extraneous '?' on {location}";

	[Test]
	public void NamedParametersAndReturn ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Named:(id _Nullable (^ _Nonnull)(id _Nullable, id _Nonnull))callback;
			- (void)NullableReturn:(id _Nullable (^ _Nonnull)(id _Nonnull, id _Nonnull))callback;
			@end
			""");
		const string method = "System.Void Xamarin.Tests.CallbackFixtures::Named(Xamarin.Tests.CallbackFixtures/NamedCallback)";
		Assert.That (messages, Is.EquivalentTo (new [] {
			Missing (method, "parameter 'callback' block parameter #0"),
			Extra (method, "parameter 'callback' block parameter #1"),
			Missing (method, "parameter 'callback' block return type"),
		}));
	}

	[Test]
	public void CallbackObjectIsIndependent ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Optional:(id _Nonnull (^ _Nullable)(id _Nonnull, id _Nullable))callback;
			- (void)Uniform:(void (^ _Nonnull)(id _Nullable, id _Nullable))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void TypedefsAndSharedUses ()
	{
		var messages = Check ("""
			typedef id _Nullable (^Creator)(id _Nonnull, id _Nullable);
			typedef Creator Alias;
			@interface Callbacks
			- (void)Named:(id _Nonnull (^ _Nonnull)(id _Nonnull, id _Nullable))callback;
			- (void)Optional:(Alias _Nullable)callback;
			@end
			""");
		Assert.That (messages, Is.EqualTo (new [] {
			Missing ("System.Void Xamarin.Tests.CallbackFixtures::Optional(Xamarin.Tests.CallbackFixtures/NamedCallback)", "parameter 'callback' block return type"),
		}));
	}

	[Test]
	public void GenericParametersAndReturns ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Action:(void (^ _Nonnull)(id _Nullable, id _Nullable))callback;
			- (void)Func:(id _Nullable (^ _Nonnull)(id _Nullable))callback;
			- (void)Factory:(id _Nullable (^ _Nonnull)(void))callback;
			- (void)Reordered:(id _Nullable (^ _Nonnull)(id _Nonnull, id _Nullable, id _Nullable))callback;
			- (void)Annotated:(id _Nullable (^ _Nonnull)(id _Nullable))callback;
			- (void)ValueLast:(void (^ _Nonnull)(id _Nullable, int))callback;
			@end
			""");
		Assert.That (messages, Is.EquivalentTo (new [] {
			Missing ("System.Void Xamarin.Tests.CallbackFixtures::Action(System.Action`2<System.Object,System.Object>)", "parameter 'callback' block parameter #0"),
			Missing ("System.Void Xamarin.Tests.CallbackFixtures::Factory(System.Func`1<System.Object>)", "parameter 'callback' block return type"),
		}));
	}

	[Test]
	public void GenericValueTypeOffsets ()
	{
		var messages = Check ("""
			struct Pair { void *item; int count; };
			@interface Callbacks
			- (void)Tuple:(void (^ _Nonnull)(struct Pair, id _Nullable))callback;
			- (void)Array:(void (^ _Nonnull)(id _Nonnull, id _Nullable))callback;
			- (void)ArrayElement:(void (^ _Nonnull)(id _Nonnull, id _Nonnull))callback;
			- (void)NestedGeneric:(void (^ _Nonnull)(id _Nonnull, id _Nonnull))callback;
			- (void)Value:(void (^ _Nonnull)(int, id _Nullable))callback;
			- (void)NullableValue:(void (^ _Nonnull)(id _Nullable, id _Nullable))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void PropertiesAndReturnedCallbacks ()
	{
		var messages = Check ("""
			typedef id _Nullable (^Creator)(id _Nonnull, id _Nullable);
			@interface Callbacks
			@property (copy, nullable) Creator ReadWrite;
			@property (readonly, nullable) Creator ReadOnly;
			- (Creator _Nullable)Returned;
			@property (copy, nullable) id _Nullable (^Generic)(id _Nonnull);
			@end
			""");
		Assert.That (messages, Is.EquivalentTo (new [] {
			Missing ("System.Void Xamarin.Tests.CallbackFixtures::set_ReadWrite(Xamarin.Tests.CallbackFixtures/NamedCallback)", "parameter 'value' block return type"),
			Missing ("Xamarin.Tests.CallbackFixtures/NamedCallback Xamarin.Tests.CallbackFixtures::get_ReadOnly()", "return type block return type"),
			Missing ("Xamarin.Tests.CallbackFixtures/NamedCallback Xamarin.Tests.CallbackFixtures::Returned()", "return type block return type"),
		}));
	}

	[Test]
	public void ByReferenceValuesAndReferents ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Reference:(void (^ _Nonnull)(_Bool * _Nonnull, id _Nullable * _Nonnull, id _Nullable * _Nonnull))callback;
			@end
			""");
		Assert.That (messages, Is.EqualTo (new [] {
			Missing ("System.Void Xamarin.Tests.CallbackFixtures::Reference(Xamarin.Tests.CallbackFixtures/ReferenceCallback)", "parameter 'callback' block parameter #2"),
		}));
	}

	[Test]
	public void UnspecifiedAndUnannotatedNativeTypes ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Named:(id _Null_unspecified (^)(id _Null_unspecified, id _Null_unspecified))callback;
			- (void)Optional:(id (^)(id, id))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}

	[TestCase ((byte) 1, 0)]
	[TestCase ((byte) 0, 0)]
	[TestCase ((byte) 2, 2)]
	public void EnclosingContext (byte context, int expectedCount)
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Context:(id _Nonnull (^ _Nonnull)(id _Nonnull))callback;
			@end
			""", (module, fixture) => {
			var callback = fixture.NestedTypes.Single (v => v.Name == "ContextCallback");
			var invoke = callback.Methods.Single (v => v.Name == "Invoke");
			invoke.CustomAttributes.Clear ();
			callback.CustomAttributes.Clear ();
			SetContext (module, fixture, context);
			SetContext (module, fixture.Methods.Single (v => v.Name == "Context"), 1);
		});
		Assert.That (messages, Has.Length.EqualTo (expectedCount));
	}

	[Test]
	public void ExplicitObliviousContextStopsLookup ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Context:(id _Nonnull (^ _Nonnull)(id _Nonnull))callback;
			@end
			""", (module, fixture) => {
			SetContext (module, fixture, 2);
			var callback = fixture.NestedTypes.Single (v => v.Name == "ContextCallback");
			SetContext (module, callback, 2);
			SetContext (module, callback.Methods.Single (v => v.Name == "Invoke"), 0);
			SetContext (module, fixture.Methods.Single (v => v.Name == "Context"), 1);
		});
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void ParameterCountMismatchIsIgnored ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Named:(void (^ _Nonnull)(id _Nonnull))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void OmittedCompletionHandler ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)Projected:(void (^ _Nonnull)(void (^ _Nonnull)(id _Nullable, id _Nullable)))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}

	[Test]
	public void PrependedDeclaringType ()
	{
		var messages = Check ("""
			@interface Callbacks
			- (void)WithSender:(void (^ _Nonnull)(id _Nullable))callback;
			@end
			""");
		Assert.That (messages, Is.Empty);
	}
}

public class CallbackFixtures {
	public delegate object NamedCallback (object required, object? optional);
	public delegate object? NullableReturnCallback (object first, object second);
	public delegate void UniformCallback (object? first, object? second);
	public delegate T ReorderedCallback<T, U> (U first, T second, object? third) where T : class? where U : class?;
	public delegate T? AnnotatedCallback<T> (T? value) where T : class?;
	public delegate void ReferenceCallback (ref bool stop, ref object? nullable, out object required);
	public delegate object ContextCallback (object value);

	public void Named (NamedCallback callback) { }
	public void NullableReturn (NullableReturnCallback callback) { }
	public void Optional (NamedCallback? callback) { }
	public void Uniform (UniformCallback callback) { }
	public void Action (Action<object, object?> callback) { }
	public void Func (Func<object?, object?> callback) { }
	public void Factory (Func<object> callback) { }
	public void Reordered (ReorderedCallback<object?, object> callback) { }
	public void Annotated (AnnotatedCallback<object> callback) { }
	public void Tuple (Action<(object?, int), object?> callback) { }
	public void Array (Action<int [], object?> callback) { }
	public void ArrayElement (Action<object? [], object> callback) { }
	public void NestedGeneric (Action<Dictionary<string, object?>, object> callback) { }
	public void Value (Action<int, object?> callback) { }
	public void ValueLast (Action<object?, int> callback) { }
	public void NullableValue (Action<int?, object?> callback) { }
	public void Reference (ReferenceCallback callback) { }
	public void Context (ContextCallback callback) { }
	public void Projected (Action callback) { }
	public void WithSender (Action<CallbackFixtures, object?> callback) { }
	public NamedCallback? ReadWrite { get; set; }
	public NamedCallback? ReadOnly => null;
	public NamedCallback? Returned () => null;
	public Func<object, object?>? Generic { get; set; }
}

[TestFixture]
public class ManagedCallbackNullabilityTests {
	static void WithFixtures (Action<ModuleDefinition, TypeDefinition> test)
	{
		using var resolver = new DefaultAssemblyResolver ();
		resolver.AddSearchDirectory (TestContext.CurrentContext.TestDirectory);
		resolver.AddSearchDirectory (RuntimeEnvironment.GetRuntimeDirectory ());
		using var assembly = AssemblyDefinition.ReadAssembly (Path.Combine (TestContext.CurrentContext.TestDirectory, "UnitTests.dll"),
			new ReaderParameters { AssemblyResolver = resolver });
		test (assembly.MainModule, assembly.MainModule.GetType ("Xamarin.Tests.CallbackFixtures"));
	}

	[TestCase ("Named", 0, "System.Object", (byte) 1)]
	[TestCase ("Named", 1, "System.Object", (byte) 2)]
	[TestCase ("Named", -1, "System.Object", (byte) 1)]
	[TestCase ("NullableReturn", -1, "System.Object", (byte) 2)]
	[TestCase ("Optional", -1, "System.Object", (byte) 1)]
	[TestCase ("Uniform", 0, "System.Object", (byte) 2)]
	[TestCase ("Uniform", 1, "System.Object", (byte) 2)]
	[TestCase ("Action", 0, "System.Object", (byte) 1)]
	[TestCase ("Action", 1, "System.Object", (byte) 2)]
	[TestCase ("Func", 0, "System.Object", (byte) 2)]
	[TestCase ("Func", -1, "System.Object", (byte) 2)]
	[TestCase ("Factory", -1, "System.Object", (byte) 1)]
	[TestCase ("Reordered", 0, "System.Object", (byte) 1)]
	[TestCase ("Reordered", 1, "System.Object", (byte) 2)]
	[TestCase ("Reordered", 2, "System.Object", (byte) 2)]
	[TestCase ("Reordered", -1, "System.Object", (byte) 2)]
	[TestCase ("Annotated", 0, "System.Object", (byte) 2)]
	[TestCase ("Annotated", -1, "System.Object", (byte) 2)]
	[TestCase ("Tuple", 1, "System.Object", (byte) 2)]
	[TestCase ("Array", 1, "System.Object", (byte) 2)]
	[TestCase ("ArrayElement", 1, "System.Object", (byte) 1)]
	[TestCase ("NestedGeneric", 1, "System.Object", (byte) 1)]
	[TestCase ("Value", 1, "System.Object", (byte) 2)]
	[TestCase ("ValueLast", 1, "System.Int32", (byte) 1)]
	[TestCase ("NullableValue", 0, "System.Nullable`1<System.Int32>", (byte) 2)]
	[TestCase ("NullableValue", 1, "System.Object", (byte) 2)]
	[TestCase ("Reference", 0, "System.Boolean", (byte) 1)]
	[TestCase ("Reference", 1, "System.Object", (byte) 2)]
	[TestCase ("Reference", 2, "System.Object", (byte) 1)]
	public void DecodeSignature (string name, int index, string expectedType, byte expectedNullability)
	{
		WithFixtures ((module, fixture) => {
			var method = fixture.Methods.Single (v => v.Name == name);
			var callback = method.Parameters [0];
			var invoke = callback.ParameterType.Resolve ().Methods.Single (v => v.Name == "Invoke");
			var check = new NullabilityCheck (new BindingResult ());
			var provider = index < 0 ? (Mono.Cecil.ICustomAttributeProvider) invoke.MethodReturnType : invoke.Parameters [index];
			var type = index < 0 ? invoke.ReturnType : invoke.Parameters [index].ParameterType;
			var actual = NullabilityCheck.GetCallbackTypeNullability (type, NullabilityCheck.GetNullable (provider), check.GetNullableContext (invoke),
				callback.ParameterType as GenericInstanceType, NullabilityCheck.GetNullable (callback), check.GetNullableContext (method));
			Assert.Multiple (() => {
				Assert.That (actual.Type.FullName, Is.EqualTo (expectedType), "Resolved type");
				Assert.That (actual.Nullability, Is.EqualTo ((NullabilityCheck.Null) expectedNullability), "Nullability");
			});
		});
	}

	[TestCase ((byte) 0)]
	[TestCase ((byte) 1)]
	[TestCase ((byte) 2)]
	public void NearestContext (byte context)
	{
		WithFixtures ((module, fixture) => {
			var callback = fixture.NestedTypes.Single (v => v.Name == "ContextCallback");
			var invoke = callback.Methods.Single (v => v.Name == "Invoke");
			invoke.CustomAttributes.Clear ();
			callback.CustomAttributes.Clear ();
			NullabilityTests.SetContext (module, fixture, context);
			Assert.That (new NullabilityCheck (new BindingResult ()).GetNullableContext (invoke), Is.EqualTo ((NullabilityCheck.Null) context), "Enclosing type");
			NullabilityTests.SetContext (module, callback, 0);
			Assert.That (new NullabilityCheck (new BindingResult ()).GetNullableContext (invoke), Is.EqualTo (NullabilityCheck.Null.Oblivious), "Explicit delegate context");
			NullabilityTests.SetContext (module, invoke, 2);
			Assert.That (new NullabilityCheck (new BindingResult ()).GetNullableContext (invoke), Is.EqualTo (NullabilityCheck.Null.Annotated), "Invoke context");
			NullabilityTests.SetContext (module, invoke, 0);
			Assert.That (new NullabilityCheck (new BindingResult ()).GetNullableContext (invoke), Is.EqualTo (NullabilityCheck.Null.Oblivious), "Explicit method context");
		});
	}
}
