using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin;
using Xamarin.Tests;
using Xamarin.Utils;

namespace GeneratorTests {
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGenTests : BGenBase {
		[Test]
		public void PrecompiledApiAssemblyWithoutSources ()
		{
			var targetFramework = TargetFramework.Parse (BGenTool.GetTargetFramework (Profile.iOS));
			var baseLibrary = Configuration.GetBaseLibrary (targetFramework);
			var attributeLibrary = Configuration.GetBindingAttributePath (targetFramework);
			var compiledApi = BuildFile (Profile.iOS, bgen => {
				bgen.BaseLibrary = baseLibrary;
				bgen.AttributeLibrary = attributeLibrary;
			}, "bug15283.cs");
			var bgen = new BGenTool {
				Profile = Profile.iOS,
				BaseLibrary = baseLibrary,
				AttributeLibrary = attributeLibrary,
				CompiledApiDefinitionAssembly = compiledApi.CompiledApiDefinitionAssembly,
				TmpDirectory = Cache.CreateTemporaryDirectory (),
			};

			bgen.AssertExecute ("Generate from a precompiled API assembly");
			Assert.That (bgen.AssemblyPath, Is.EqualTo (Path.Combine (bgen.TmpDirectory, "binding.dll")), "Assembly path");
		}

		// Removing the following variable might make running the unit tests in VSMac fail.
		static Type variable_to_keep_reference_to_system_runtime_compilerservices_unsafe_assembly = typeof (System.Runtime.CompilerServices.Unsafe);

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BMac_Smoke (Profile profile)
		{
			BuildFile (profile, "bmac_smoke.cs");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void BMac_With_Hyphen_In_Name (Profile profile)
		{
			BuildFile (profile, "bmac-with-hyphen-in-name.cs");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void PropertyRedefinitionMac (Profile profile)
		{
			BuildFile (profile, "property-redefination-mac.cs");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void NSApplicationPublicEnsureMethods (Profile profile)
		{
			BuildFile (profile, "NSApplicationPublicEnsureMethods.cs");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void ProtocolDuplicateAbstract (Profile profile)
		{
			BuildFile (profile, "protocol-duplicate-abstract.cs");
		}

		[Test]
		public void Bug15283 ()
		{
			BuildFile (Profile.iOS, "bug15283.cs");
		}

		[Test]
		public void Bug15307 ()
		{
			BuildFile (Profile.iOS, "bug15307.cs");
		}

		[Test]
		public void Bug16036 ()
		{
			BuildFile (Profile.iOS, "bug16036.cs");
		}

		[Test]
		public void Bug17232 ()
		{
			BuildFile (Profile.iOS, "bug17232.cs");
		}

		[Test]
		public void Bug24078 ()
		{
			BuildFile (Profile.iOS, "bug24078-ignore-methods-events.cs");
		}

		[Test]
		public void Bug27428 ()
		{
			BuildFile (Profile.iOS, "bug27428.cs");
		}

		[Test]
		public void Bug27430 ()
		{
			BuildFile (Profile.iOS, "bug27430.cs");
		}

		[Test]
		public void Bug27986 ()
		{
			var bgen = BuildFile (Profile.iOS, false, "bug27986.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<ICustomAttributeProvider>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));

			var preserves = allMembers.Count ((v) => v.HasCustomAttributes && v.CustomAttributes.Any ((ca) => ca.AttributeType.Name == "PreserveAttribute"));
			Assert.That (preserves, Is.EqualTo (35), "Preserve attribute count"); // If you modified code that generates PreserveAttributes please update the preserve count
		}

		[Test]
		public void Bug29493 ()
		{
			var bgen = BuildFile (Profile.iOS, false, "bug29493.cs");

			// Check that there is no call to Class.GetHandle with a "global::"-prefixed string
			foreach (var method in bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((v) => v.Methods)) {
				if (!method.HasBody)
					continue;
				var instructions = method.Body.Instructions;
				foreach (var ins in instructions) {
					if (ins.OpCode.FlowControl != FlowControl.Call)
						continue;
					var mr = (MethodReference) ins.Operand;
					if (mr.DeclaringType.Namespace != "ObjCRuntime" && mr.DeclaringType.Name != "Class")
						continue;
					if (mr.Name != "GetHandle" && mr.Name != "GetHandleIntrinsic")
						continue;
					var str = (string) ins.Previous.Operand;
					if (str.StartsWith ("global::", StringComparison.Ordinal))
						Assert.Fail ($"Found a call to Class.GetHandle with an invalid ('global::'-prefixed) string in {method.FullName} at offset {ins.Offset}.\n\t{string.Join ("\n\t", instructions)}");
				}
			}
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		public void Bug31788 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "bug31788.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();

			bgen.AssertApiCallsMethod ("Test", "MarshalInProperty", "get_Shared", "xamarin_NativeHandle_objc_msgSend_exception", "MarshalInProperty.Shared getter");
			bgen.AssertApiCallsMethod ("Test", "MarshalOnProperty", "get_Shared", "xamarin_NativeHandle_objc_msgSend_exception", "MarshalOnProperty.Shared getter");
		}

		[Test]
		public void Bug34042 ()
		{
			BuildFile (Profile.iOS, "bug34042.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FactoryMethod (Profile profile)
		{
			var bgen = BuildFile (profile, "factory-method.cs");
			bgen.AssertNoWarnings ();

			const string type = "FactoryMethodTest.FactoryWidget";
			var widget = bgen.ApiAssembly.MainModule.GetType (type);

			// The factory methods are generated as public static methods returning the type.
			var create = widget.Methods.Single (m => m.Name == "Create");
			Assert.That (create.IsStatic, Is.True, "Create is static");
			Assert.That (create.IsPublic, Is.True, "Create is public");
			Assert.That (create.ReturnType.FullName, Is.EqualTo (type), "Create return type");
			Assert.That (create.Parameters.Select (p => p.ParameterType.FullName), Is.EqualTo (new [] { "System.String" }), "Create parameters");
			Assert.That (FactoryMethodHasBindingImplOption (create), Is.True, "Create has factory method metadata");
			// A nullable factory method disposes and returns null when the initializer fails.
			Assert.That (FactoryMethodDisposesOnFailure (create), Is.True, "Create handles nil");

			// The factory method name can be customized via [FactoryMethod ("...")].
			var createWithCount = widget.Methods.Single (m => m.Name == "CreateWithCount");
			Assert.That (createWithCount.IsStatic, Is.True, "CreateWithCount is static");
			Assert.That (createWithCount.IsPublic, Is.True, "CreateWithCount is public");
			Assert.That (createWithCount.ReturnType.FullName, Is.EqualTo (type), "CreateWithCount return type");
			Assert.That (createWithCount.Parameters.Select (p => p.ParameterType.FullName), Is.EqualTo (new [] { "System.String", "System.IntPtr" }), "CreateWithCount parameters");
			Assert.That (FactoryMethodHasBindingImplOption (createWithCount), Is.True, "CreateWithCount has factory method metadata");
			Assert.That (FactoryMethodDisposesOnFailure (createWithCount), Is.True, "CreateWithCount handles nil");

			// A non-nullable factory method (the initializer isn't failable) just returns
			// the new instance without a nil check.
			var createWithColor = widget.Methods.Single (m => m.Name == "CreateWithColor");
			Assert.That (createWithColor.IsStatic, Is.True, "CreateWithColor is static");
			Assert.That (createWithColor.IsPublic, Is.True, "CreateWithColor is public");
			Assert.That (createWithColor.ReturnType.FullName, Is.EqualTo (type), "CreateWithColor return type");
			Assert.That (FactoryMethodHasBindingImplOption (createWithColor), Is.True, "CreateWithColor has factory method metadata");
			Assert.That (FactoryMethodDisposesOnFailure (createWithColor), Is.False, "CreateWithColor doesn't handle nil");

			// The backing constructors are generated as internal (not public), so the
			// public API only exposes the factory methods.
			var factoryCtors = widget.Methods.Where (m => m.IsConstructor && m.Parameters.Count > 0 && m.Parameters [0].ParameterType.FullName == "System.String").ToArray ();
			Assert.That (factoryCtors, Is.Not.Empty, "backing constructors exist");
			foreach (var ctor in factoryCtors)
				Assert.That (ctor.IsAssembly, Is.True, $"{ctor.FullName} should be internal");
		}

		// A nullable factory method disposes the instance and returns null when the native
		// initializer fails; detect this by looking for a Dispose call in the method body.
		static bool FactoryMethodDisposesOnFailure (Mono.Cecil.MethodDefinition method)
		{
			return method.Body.Instructions.Any (i =>
				i.Operand is Mono.Cecil.MethodReference mr && mr.Name == "Dispose");
		}

		static bool FactoryMethodHasBindingImplOption (Mono.Cecil.MethodDefinition method)
		{
			var attribute = method.CustomAttributes.Single (a => a.AttributeType.Name == "BindingImplAttribute");
			var options = (int) attribute.ConstructorArguments [0].Value;
			return (options & 4) == 4; // BindingImplOptions.FactoryMethod
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FactoryMethodInternal (Profile profile)
		{
			var bgen = BuildFile (profile, "factory-method-internal.cs");
			bgen.AssertNoWarnings ();

			const string type = "FactoryMethodInternalTest.InternalFactoryWidget";
			var widget = bgen.ApiAssembly.MainModule.GetType (type);

			// An [Internal] factory method is generated as 'internal static' (not
			// 'public static'), and must still compile (no duplicate modifiers).
			var create = widget.Methods.Single (m => m.Name == "Create");
			Assert.That (create.IsStatic, Is.True, "Create is static");
			Assert.That (create.IsAssembly, Is.True, "Create is internal");
			Assert.That (create.ReturnType.FullName, Is.EqualTo (type), "Create return type");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FactoryMethodMultiple (Profile profile)
		{
			var bgen = BuildFile (profile, "factory-method-multiple.cs");
			bgen.AssertNoWarnings ();

			const string type = "FactoryMethodMultipleTest.MultiWidget";
			var widget = bgen.ApiAssembly.MainModule.GetType (type);

			// Both initializers are generated as separate public static factory methods.
			var createWithFoo = widget.Methods.Single (m => m.Name == "CreateWithFoo");
			Assert.That (createWithFoo.IsStatic, Is.True, "CreateWithFoo is static");
			Assert.That (createWithFoo.IsPublic, Is.True, "CreateWithFoo is public");
			Assert.That (createWithFoo.ReturnType.FullName, Is.EqualTo (type), "CreateWithFoo return type");
			Assert.That (createWithFoo.Parameters.Select (p => p.ParameterType.FullName), Is.EqualTo (new [] { "System.IntPtr" }), "CreateWithFoo parameters");
			Assert.That (FactoryMethodHasBindingImplOption (createWithFoo), Is.True, "CreateWithFoo has factory method metadata");
			Assert.That (FactoryMethodDisposesOnFailure (createWithFoo), Is.True, "CreateWithFoo handles nil");

			var createWithBar = widget.Methods.Single (m => m.Name == "CreateWithBar");
			Assert.That (createWithBar.IsStatic, Is.True, "CreateWithBar is static");
			Assert.That (createWithBar.IsPublic, Is.True, "CreateWithBar is public");
			Assert.That (createWithBar.ReturnType.FullName, Is.EqualTo (type), "CreateWithBar return type");
			Assert.That (createWithBar.Parameters.Select (p => p.ParameterType.FullName), Is.EqualTo (new [] { "System.IntPtr" }), "CreateWithBar parameters");
			Assert.That (FactoryMethodHasBindingImplOption (createWithBar), Is.True, "CreateWithBar has factory method metadata");
			Assert.That (FactoryMethodDisposesOnFailure (createWithBar), Is.True, "CreateWithBar handles nil");

			// A non-failable initializer: non-nullable return, so no nil-check/Dispose.
			var createWithBaz = widget.Methods.Single (m => m.Name == "CreateWithBaz");
			Assert.That (createWithBaz.IsStatic, Is.True, "CreateWithBaz is static");
			Assert.That (createWithBaz.IsPublic, Is.True, "CreateWithBaz is public");
			Assert.That (createWithBaz.ReturnType.FullName, Is.EqualTo (type), "CreateWithBaz return type");
			Assert.That (createWithBaz.Parameters.Select (p => p.ParameterType.FullName), Is.EqualTo (new [] { "System.IntPtr" }), "CreateWithBaz parameters");
			Assert.That (FactoryMethodHasBindingImplOption (createWithBaz), Is.True, "CreateWithBaz has factory method metadata");
			Assert.That (FactoryMethodDisposesOnFailure (createWithBaz), Is.False, "CreateWithBaz doesn't handle nil");

			// The backing 'init' message-send helpers are generated as private instance
			// methods (prefixed with an underscore) without an [Export] attribute, so the
			// public API only exposes the static factory methods and the 'init' selectors
			// aren't registered with the Objective-C runtime.
			var helperFoo = widget.Methods.Single (m => m.Name == "_CreateWithFoo");
			Assert.That (helperFoo.IsStatic, Is.False, "_CreateWithFoo is an instance method");
			Assert.That (helperFoo.IsPrivate, Is.True, "_CreateWithFoo is private");
			Assert.That (helperFoo.IsVirtual, Is.False, "_CreateWithFoo is not virtual");
			Assert.That (FactoryMethodHasExport (helperFoo), Is.False, "_CreateWithFoo has no [Export]");
			var helperBar = widget.Methods.Single (m => m.Name == "_CreateWithBar");
			Assert.That (helperBar.IsStatic, Is.False, "_CreateWithBar is an instance method");
			Assert.That (helperBar.IsPrivate, Is.True, "_CreateWithBar is private");
			Assert.That (helperBar.IsVirtual, Is.False, "_CreateWithBar is not virtual");
			Assert.That (FactoryMethodHasExport (helperBar), Is.False, "_CreateWithBar has no [Export]");
		}

		// Returns true if the method has an [Export] attribute.
		static bool FactoryMethodHasExport (Mono.Cecil.MethodDefinition method)
		{
			return method.CustomAttributes.Any (a => a.AttributeType.Name == "ExportAttribute");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FactoryMethodOutErrorWithoutNullableReturn (Profile profile)
		{
			var bgen = BuildFile (profile, false, "factory-method-error.cs");
			bgen.AssertWarning (1125, "The [FactoryMethod] binding method 'FactoryMethodErrorTest.FailableWidget.Constructor' has an 'out NSError' parameter, but its return value is not nullable. Add [return: NullAllowed] to the binding method so the generated factory method can return null when the native initializer fails.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NSCopyingNullability (Profile profile)
		{
			var bgen = BuildFile (profile, "nscopying-nullability.cs");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void EditorBrowsable (Profile profile)
		{
			var bgen = BuildFile (profile, false, true, "editor-browsable.cs");
			var types = bgen.ApiAssembly.MainModule.Types;

			var hasEditorBrowsableAttribute = new Func<ICustomAttributeProvider, bool> ((ICustomAttributeProvider provider) => {
				return provider.CustomAttributes.Any (v => v.AttributeType.Name == "EditorBrowsableAttribute");
			});

			var strongEnumType = types.Single (v => v.Name == "StrongEnum");
			Assert.That (hasEditorBrowsableAttribute (strongEnumType), Is.True, "StrongEnumType");
			var objcClassType = types.Single (v => v.Name == "ObjCClass");
			Assert.That (hasEditorBrowsableAttribute (objcClassType), Is.True, "ObjCClass");
		}

		static string RenderArgument (CustomAttributeArgument arg)
		{
			var td = arg.Type.Resolve ();
			// If it's an enum value, try to find the enum field name and return that.
			if (td?.BaseType?.Name == "Enum") {
				if (arg.Value is byte b2) {
					var fields = td.Fields
									.Where (f => f.HasConstant && (byte) f.Constant == b2)
									.OrderBy (f => f.Name);
					if (fields.Any ())
						return td.FullName + "." + fields.First ().Name;
				}
			}
			var obj = arg.Value;
			if (obj is null)
				return "null";

			if (obj is string str)
				return "\"" + str + "\"";

			if (obj is byte b)
				return b.ToString ();

			if (obj is int i32)
				return i32.ToString ();

			// Good enough for now, implement more cases as required.
			throw new NotImplementedException (obj.GetType ().FullName);
		}

		static IEnumerable<CustomAttribute> GetAvailabilityAttributes (ICustomAttributeProvider provider)
		{
			if (!provider.HasCustomAttributes)
				yield break;

			foreach (var ca in provider.CustomAttributes) {
				switch (ca.AttributeType.Name) {
				case "SupportedOSPlatformAttribute":
				case "UnsupportedOSPlatformAttribute":
				case "ObsoletedOSPlatformAttribute":
					yield return ca;
					break;
				}
			}
		}

		static string RenderSupportedOSPlatformAttributes (ICustomAttributeProvider provider)
		{
			var attributes = GetAvailabilityAttributes (provider).ToArray ();
			if (attributes is null || attributes.Length == 0)
				return string.Empty;
			var lines = new List<string> ();
			foreach (var ca in attributes)
				lines.Add (RenderSupportedOSPlatformAttribute (ca));
			lines.Sort ();
			return string.Join ("\n", lines).Replace ("\r", string.Empty);
		}

		static string RenderSupportedOSPlatformAttribute (CustomAttribute ca)
		{
			return "[" + ca.AttributeType.Name.Replace ("Attribute", "") + "(" + string.Join (", ", ca.ConstructorArguments.Select (arg => RenderArgument (arg))) + ")]";
		}

		[Test]
		public void Bug35176 ()
		{
			var bgen = BuildFile (Profile.iOS, "bug35176.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<ICustomAttributeProvider>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));
			const string attrib = "SupportedOSPlatformAttribute";
			var allSupportedAttributes = allMembers.SelectMany (v => v.CustomAttributes.Where (ca => ca.AttributeType.Name == attrib).Select (ca => new Tuple<ICustomAttributeProvider, CustomAttribute> (v, ca)));
			var renderedSupportedAttributes = allSupportedAttributes.Select (v => v.Item1.ToString () + ": " + RenderSupportedOSPlatformAttribute (v.Item2) + "");
			var preserves = allSupportedAttributes.Count ();
			var renderedAttributes = "\t" + string.Join ("\n\t", renderedSupportedAttributes.OrderBy (v => v)) + "\n";
			string expectedAttributes =
@"	Bug35176.IFooInterface: [SupportedOSPlatform(""ios"")]
	Bug35176.IFooInterface: [SupportedOSPlatform(""maccatalyst18.2"")]
	Bug35176.IFooInterface: [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.BarObject::BarView(): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.BarObject::BarView(): [SupportedOSPlatform(""maccatalyst18.2"")]
	UIKit.UIView Bug35176.BarObject::BarView(): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.BarObject::FooView(): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.BarObject::FooView(): [SupportedOSPlatform(""maccatalyst18.2"")]
	UIKit.UIView Bug35176.BarObject::FooView(): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.BarObject::get_BarView(): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.BarObject::get_BarView(): [SupportedOSPlatform(""maccatalyst18.3"")]
	UIKit.UIView Bug35176.BarObject::get_BarView(): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.BarObject::GetBarMember(System.Int32): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.BarObject::GetBarMember(System.Int32): [SupportedOSPlatform(""maccatalyst18.2"")]
	UIKit.UIView Bug35176.BarObject::GetBarMember(System.Int32): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.FooInterface_Extensions::GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.FooInterface_Extensions::GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""maccatalyst18.3"")]
	UIKit.UIView Bug35176.FooInterface_Extensions::GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.IFooInterface::_GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.IFooInterface::_GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""maccatalyst18.3"")]
	UIKit.UIView Bug35176.IFooInterface::_GetBarView(Bug35176.IFooInterface): [SupportedOSPlatform(""macos26.2"")]
	UIKit.UIView Bug35176.IFooInterface::get_BarView(): [SupportedOSPlatform(""ios"")]
	UIKit.UIView Bug35176.IFooInterface::get_BarView(): [SupportedOSPlatform(""maccatalyst18.3"")]
	UIKit.UIView Bug35176.IFooInterface::get_BarView(): [SupportedOSPlatform(""macos26.2"")]
";

			expectedAttributes = expectedAttributes.Replace ("\r", string.Empty);
			renderedAttributes = renderedAttributes.Replace ("\r", string.Empty);

			if (renderedAttributes != expectedAttributes) {
				Console.WriteLine ($"Expected:");
				Console.WriteLine (expectedAttributes);
				Console.WriteLine ($"Actual:");
				Console.WriteLine (renderedAttributes);
			}

			Assert.That (renderedAttributes, Is.EqualTo (expectedAttributes), "Introduced attributes");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void INativeObjectsInBlocks (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.AddTestApiDefinition ("inativeobjects-in-blocks.cs");
			bgen.AddExtraSourcesRelativeToGeneratorDirectory ("inativeobjects-in-blocks-sources.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void Bug36457 ()
		{
			BuildFile (Profile.iOS, "bug36457.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void Bug39614 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bug39614.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1103, "'FooType`1' does not live under a namespace; namespaces are a highly recommended .NET best practice");
		}

		[TestCase (Profile.iOS)]
		public void Bug18035 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("bug18025.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1103, "'FooType' does not live under a namespace; namespaces are a highly recommended .NET best practice");
		}

		[Test]
		public void Bug40282 ()
		{
			BuildFile (Profile.iOS, "bug40282.cs");
		}

		[Test]
		public void Bug42742 ()
		{
			var bgen = BuildFile (Profile.iOS, "bug42742.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<ICustomAttributeProvider>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));

			var preserves = allMembers.Sum ((v) => v.CustomAttributes.Count ((ca) => ca.AttributeType.Name == "AdviceAttribute"));
			Assert.That (preserves, Is.EqualTo (33), "Advice attribute count"); // If you modified code that generates AdviceAttributes please update the attribute count
		}

		[Test]
		public void Bug43579 ()
		{
			BuildFile (Profile.iOS, "bug43579.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void Bug46292 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.AddTestApiDefinition ("bug46292.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<ICustomAttributeProvider>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));

			var attribCount = allMembers.Count ((v) => v.HasCustomAttributes && v.CustomAttributes.Any ((ca) => ca.AttributeType.Name == "ObsoleteAttribute"));
			Assert.That (attribCount, Is.EqualTo (2), "attribute count");
		}

		[Test]
		public void Bug53076 ()
		{
			var bgen = BuildFile (Profile.iOS, "bug53076.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMethods = bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((type) => type.Methods);

			// Count all *Async methods whose first parameter is 'IMyFooProtocol'.
			var methodCount = allMethods.Count ((v) => v.Name.EndsWith ("Async", StringComparison.Ordinal) && v.Parameters.Count > 0 && v.Parameters [0].ParameterType.Name == "IMyFooProtocol");
			Assert.That (methodCount, Is.EqualTo (10), "Async method count");
		}

		[Test]
		public void Bug53076WithModel ()
		{
			var bgen = BuildFile (Profile.iOS, "bug53076withmodel.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMethods = bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((type) => type.Methods);

			// Count all *Async methods whose first parameter is 'IMyFooProtocol'.
			var methodCount = allMethods.Count ((v) => v.Name.EndsWith ("Async", StringComparison.Ordinal) && v.Parameters.Count > 0 && v.Parameters [0].ParameterType.Name == "IMyFooProtocol");
			Assert.That (methodCount, Is.EqualTo (10), "Async method count");
		}

		[Test]
		public void StackOverflow20696157 ()
		{
			BuildFile (Profile.iOS, (bgen) => {
				bgen.NoWarn = "1123";
			}, "sof20696157.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void TypesInMultipleNamespaces (Profile profile)
		{
			BuildFile (profile, "types-in-multiple-namespaces.cs");
		}

		[Test]
		public void HyphenInName ()
		{
			BuildFile (Profile.iOS, "btouch-with-hyphen-in-name.cs");
		}

		[Test]
		public void PropertyRedefinition ()
		{
			BuildFile (Profile.iOS, "property-redefination-ios.cs");
		}

		[Test]
		public void ArrayFromHandleBug ()
		{
			BuildFile (Profile.iOS, "arrayfromhandlebug.cs");
		}

		[Test]
		public void StrongDictSupportTemplatedDicts ()
		{
			BuildFile (Profile.iOS, "strong-dict-support-templated-dicts.cs");
		}

		[Test]
		public void GenericStrongDictionary ()
		{
			BuildFile (Profile.iOS, "generic-strong-dictionary.cs");
		}

		[Test]
		public void GenericNSObjectParameter ()
		{
			BuildFile (Profile.iOS, "generic-type-nsobject.cs");
		}

		[Test]
		public void BindAsTests ()
		{
			BuildFile (Profile.iOS, "bindastests.cs");
		}

		[Test]
		public void Forum54078 ()
		{
			var bgen = BuildFile (Profile.iOS, "forum54078.cs");

			var api = bgen.ApiAssembly;
			var type = api.MainModule.GetType ("Test", "CustomController");
			foreach (var method in type.Methods)
				Asserts.DoesNotThrowExceptions (method, type.FullName);
		}

		[Test]
		public void Desk63279 ()
		{
			BuildFile (Profile.iOS, "desk63279A.cs", "desk63279B.cs");
		}

		[Test]
		public void Desk79124 ()
		{
			var bgen = BuildFile (Profile.iOS, "desk79124.cs");
			bgen.AssertType ("Desk79124.WYPopoverBackgroundView/WYPopoverBackgroundViewAppearance");
		}

		[Test]
		public void MultipleApiDefinitions1 ()
		{
			BuildFile (Profile.iOS, "multiple-api-definitions1.cs");
		}

		[Test]
		public void MultipleApiDefinitions2 ()
		{
			BuildFile (Profile.iOS, "multiple-api-definitions2-a.cs", "multiple-api-definitions2-b.cs");
		}


		[Test]
		[TestCase (Profile.iOS)]
		public void INativeObjectArraysInBlocks (Profile profile)
		{
			BuildFile (profile, "inativeobject-arrays-in-blocks.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void ClassNameCollision (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.Sources.Add (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "classNameCollision-enum.cs"));
			bgen.ApiDefinitions.Add (Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "classNameCollision.cs"));
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void VirtualWrap (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("virtualwrap.cs");
			bgen.CreateTemporaryBinding ();
			bgen.ProcessEnums = true;
			bgen.AssertExecute ("build");

			// verify virtual methods
			var attribs = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot;
			bgen.AssertMethod ("WrapTest.MyFooClass", "FromUrl", attribs, null, "Foundation.NSUrl");
			bgen.AssertMethod ("WrapTest.MyFooClass", "FromUrl", attribs, null, "System.String");
			bgen.AssertMethod ("WrapTest.MyFooClass", "get_FooNSString", attribs | MethodAttributes.SpecialName, "Foundation.NSString");
			bgen.AssertMethod ("WrapTest.MyFooClass", "get_FooString", attribs | MethodAttributes.SpecialName, "System.String");

			// verify non-virtual methods
			attribs = MethodAttributes.Public | MethodAttributes.HideBySig;
			bgen.AssertMethod ("WrapTest.MyFooClass", "FromUrlN", attribs, null, "System.String");
			bgen.AssertMethod ("WrapTest.MyFooClass", "get_FooNSStringN", attribs | MethodAttributes.SpecialName, "Foundation.NSString");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NoAsyncInternalWrapper (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("noasyncinternalwrapper.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<MemberReference>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));

			Assert.That (allMembers.Count ((member) => member.Name == "RequiredMethodAsync"), Is.EqualTo (2), "Expected 2 RequiredMethodAsync members in generated code. If you modified code that generates RequiredMethodAsync (AsyncAttribute) please update the RequiredMethodAsync count.");

			var attribs = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
			bgen.AssertMethod ("NoAsyncInternalWrapperTests.MyFooDelegate_Extensions", "RequiredMethodAsync", attribs, "System.Threading.Tasks.Task", "NoAsyncInternalWrapperTests.IMyFooDelegate", "System.Int32");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NoAsyncWarningCS0219 (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("noasyncwarningcs0219.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FieldEnumTests (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.AddTestApiDefinition ("fieldenumtests.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void SmartEnumWithFramework (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.AddTestApiDefinition ("smartenumwithframework.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			bgen.AssertApiLoadsField ("SmartEnumWithFramework.FooEnumTestExtensions", "get_First", "ObjCRuntime.Libraries/CoreImage", "Handle", "First getter");
			bgen.AssertApiLoadsField ("SmartEnumWithFramework.FooEnumTestExtensions", "get_Second", "ObjCRuntime.Libraries/CoreImage", "Handle", "Second getter");
		}

		[Test]
		public void ForcedType ()
		{
			var bgen = BuildFile (Profile.iOS, false, "forcedtype.cs");

			var allMethods = bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((type) => type.Methods);

			// Count the number of calls to GetINativeObject
			var getINativeObjectCalls = allMethods.Sum ((method) => {
				if (!method.HasBody)
					return 0;
				return method.Body.Instructions.Count ((ins) => {
					if (ins.OpCode.FlowControl != FlowControl.Call)
						return false;
					var mr = (MethodReference) ins.Operand;
					return mr.Name == "GetINativeObject";
				});
			});

			Assert.That (getINativeObjectCalls, Is.EqualTo (12), "Preserve attribute count"); // If you modified code that generates PreserveAttributes please update the preserve count
		}

		[Test]
		public void IsDirectBinding ()
		{
			var bgen = BuildFile (Profile.iOS, "is-direct-binding.cs");

			var callsMethod = new Func<MethodDefinition, string, bool> ((method, name) => {
				return method.Body.Instructions.Any ((ins) => {
					switch (ins.OpCode.Code) {
					case Code.Call:
					case Code.Calli:
					case Code.Callvirt:
						var mr = (MethodReference) ins.Operand;
						return mr.Name == name;
					default:
						return false;
					}
				});
			});

			// The normal constructor should get the IsDirectBinding value, and call both objc_msgSend and objc_msgSendSuper
			var cConstructor = bgen.ApiAssembly.MainModule.GetType ("NS", "C").Methods.First ((v) => v.IsConstructor && !v.HasParameters && !v.IsStatic);
			Assert.That (callsMethod (cConstructor, "set_IsDirectBinding"), "C: set_IsDirectBinding");
			Assert.That (callsMethod (cConstructor, "get_IsDirectBinding"), "C: get_IsDirectBinding");
			Assert.That (callsMethod (cConstructor, "IntPtr_objc_msgSend"), "C: objc_msgSend");
			Assert.That (callsMethod (cConstructor, "IntPtr_objc_msgSendSuper"), "C: objc_msgSendSuper");

			// The constructor for a model should not get the IsDirectBinding value, because it's always 'false'. Neither should it call objc_msgSend, only objc_msgSendSuper
			var pConstructor = bgen.ApiAssembly.MainModule.GetType ("NS", "P").Methods.First ((v) => v.IsConstructor && !v.HasParameters && !v.IsStatic);
			Assert.That (callsMethod (pConstructor, "set_IsDirectBinding"), "P: set_IsDirectBinding");
			Assert.That (!callsMethod (pConstructor, "get_IsDirectBinding"), "P: get_IsDirectBinding");
			Assert.That (!callsMethod (pConstructor, "IntPtr_objc_msgSend"), "P: objc_msgSend");
			Assert.That (callsMethod (pConstructor, "IntPtr_objc_msgSendSuper"), "P: objc_msgSendSuper");

			// The constructor for a sealed class should not get the IsDirectBinding value, because it's always true. Neither should it call objc_msgSendSuper, only objc_msgSend.
			var sConstructor = bgen.ApiAssembly.MainModule.GetType ("NS", "S").Methods.First ((v) => v.IsConstructor && !v.HasParameters && !v.IsStatic);
			Assert.That (callsMethod (sConstructor, "set_IsDirectBinding"), "S: set_IsDirectBinding");
			Assert.That (!callsMethod (sConstructor, "get_IsDirectBinding"), "S: get_IsDirectBinding");
			Assert.That (callsMethod (sConstructor, "IntPtr_objc_msgSend"), "S: objc_msgSend");
			Assert.That (!callsMethod (sConstructor, "IntPtr_objc_msgSendSuper"), "S: objc_msgSendSuper");
		}

		[Test]
		public void Bug57531 () => BuildFile (Profile.iOS, "bug57531.cs");

		[Test]
		public void Bug57870 () => BuildFile (Profile.iOS, true, true, "bug57870.cs");

		[Test]
		public void GHIssue3869 () => BuildFile (Profile.iOS, "ghissue3869.cs");

		[Test]
		[TestCase ("issue3875.cs", "api0__Issue3875_AProtocol")]
		[TestCase ("issue3875B.cs", "BProtocol")]
		[TestCase ("issue3875C.cs", "api0__Issue3875_AProtocol")]
		public void Issue3875 (string file, string modelName)
		{
			var bgen = BuildFile (Profile.iOS, file);
			var attrib = bgen.ApiAssembly.MainModule.GetType ("Issue3875", "AProtocol").CustomAttributes.Where ((v) => v.AttributeType.Name == "RegisterAttribute").First ();
			Assert.That (attrib.ConstructorArguments [0].Value, Is.EqualTo (modelName), "Custom ObjC name");
		}

		[Test]
		public void GHIssue5444 () => BuildFile (Profile.iOS, "ghissue5444.cs");

		[Test]
		[TestCase (Profile.iOS)]
		public void GH5416_method (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("ghissue5416b.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1118, "[NullAllowed] should not be used on methods, like 'Foundation.NSString Method(Foundation.NSDate, Foundation.NSObject)', but only on properties, parameters and return values.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void GH5416_setter (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("ghissue5416a.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1118, "[NullAllowed] should not be used on methods, like 'System.Void set_Setter(Foundation.NSString)', but only on properties, parameters and return values.");
		}

		[Test]
		public void GHIssue5692 () => BuildFile (Profile.iOS, "ghissue5692.cs");

		[Test]
		public void GHIssue7304 () => BuildFile (Profile.macOSMobile, "ghissue7304.cs");

		[Test]
		public void RefOutParameters ()
		{
			BuildFile (Profile.macOSMobile, true, "ref-out-parameters.cs");
		}

		[Test]
		public void ReturnRelease ()
		{
			BuildFile (Profile.iOS, "return-release.cs");
		}

		[Test]
		public void GHIssue6626 () => BuildFile (Profile.iOS, "ghissue6626.cs");

		[Test]
		public void StrongDictsNativeEnums () => BuildFile (Profile.iOS, "strong-dict-native-enum.cs");

		[Test]
		public void IgnoreUnavailableProtocol ()
		{
			var bgen = BuildFile (Profile.iOS, "ignore-unavailable-protocol.cs");
			var myClass = bgen.ApiAssembly.MainModule.GetType ("NS", "MyClass");
			var myProtocol = bgen.ApiAssembly.MainModule.GetType ("NS", "IMyProtocol");
			var myClassInterfaces = myClass.Interfaces.Select (v => v.InterfaceType.Name).ToArray ();
			Assert.That (myClassInterfaces, Does.Not.Contain ("IMyProtocol"), "IMyProtocol");
			Assert.That (myProtocol, Is.Null, "MyProtocol null");
		}

		[Test]
		public void VSTS970507 ()
		{
			BuildFile (Profile.iOS, "vsts-970507.cs");
		}

		[Test]
		public void DiamondProtocol ()
		{
			BuildFile (Profile.iOS, "diamond-protocol.cs");
		}

		[Test]
		public void GHIssue9065_Sealed () => BuildFile (Profile.iOS, nowarnings: true, "ghissue9065.cs");

		[Test]
		public void GHIssue18645_DuplicatedFiled ()
		{
			BuildFile (Profile.iOS, (bgen) => {
				bgen.NoWarn = "1123";
			}, "ghissue18645.cs");
		}

		// looking for [BindingImpl (BindingImplOptions.Optimizable)]
		bool IsOptimizable (MethodDefinition method)
		{
			const int Optimizable = 0x2; // BindingImplOptions flag

			if (!method.HasCustomAttributes)
				return false;

			foreach (var ca in method.CustomAttributes) {
				if (ca.AttributeType.Name != "BindingImplAttribute")
					continue;
				foreach (var a in ca.ConstructorArguments)
					return (((int) a.Value & Optimizable) == Optimizable);
			}
			return false;
		}

		[Test]
		public void DisposeAttributeOptimizable ()
		{
			var profile = Profile.iOS;
			var bgen = BuildFile (profile, "dispose-attribute.cs");

			// processing custom attributes (like its properties) will call Resolve so we must be able to find the platform assembly to run this test
			var resolver = (BaseAssemblyResolver) bgen.ApiAssembly.MainModule.AssemblyResolver;
			resolver.AddSearchDirectory (Configuration.GetRefDirectory (profile.AsPlatform ()));

			// [Dispose] is, by default, not optimizable
			var with_dispose = bgen.ApiAssembly.MainModule.GetType ("NS", "WithDispose").Methods.First ((v) => v.Name == "Dispose");
			Assert.That (with_dispose, Is.Not.Null, "WithDispose");
			Assert.That (IsOptimizable (with_dispose), Is.False, "WithDispose/Optimizable");

			// [Dispose] can opt-in being optimizable
			var with_dispose_optin = bgen.ApiAssembly.MainModule.GetType ("NS", "WithDisposeOptInOptimizable").Methods.First ((v) => v.Name == "Dispose");
			Assert.That (with_dispose_optin, Is.Not.Null, "WithDisposeOptInOptimizable");
			Assert.That (IsOptimizable (with_dispose_optin), Is.True, "WithDisposeOptInOptimizable/Optimizable");

			// Without a [Dispose] attribute the generated method is optimizable
			var without_dispose = bgen.ApiAssembly.MainModule.GetType ("NS", "WithoutDispose").Methods.First ((v) => v.Name == "Dispose");
			Assert.That (without_dispose, Is.Not.Null, "WitoutDispose");
			Assert.That (IsOptimizable (without_dispose), Is.True, "WitoutDispose/Optimizable");
		}

		[Test]
		public void SnippetAttributesOptimizable ()
		{
			var profile = Profile.iOS;
			var bgen = BuildFile (profile, "snippet-attributes.cs");

			// processing custom attributes (like its properties) will call Resolve so we must be able to find the platform assembly to run this test
			var resolver = (BaseAssemblyResolver) bgen.ApiAssembly.MainModule.AssemblyResolver;
			resolver.AddSearchDirectory (Configuration.GetRefDirectory (profile.AsPlatform ()));

			// [SnippetAttribute] subclasses are, by default, not optimizable
			var not_opt = bgen.ApiAssembly.MainModule.GetType ("NS", "NotOptimizable");
			Assert.That (not_opt, Is.Not.Null, "NotOptimizable");
			var pre_not_opt = not_opt.Methods.First ((v) => v.Name == "Pre");
			Assert.That (IsOptimizable (pre_not_opt), Is.False, "NotOptimizable/Pre");
			var prologue_not_opt = not_opt.Methods.First ((v) => v.Name == "Prologue");
			Assert.That (IsOptimizable (prologue_not_opt), Is.False, "NotOptimizable/Prologue");
			var post_not_opt = not_opt.Methods.First ((v) => v.Name == "Post");
			Assert.That (IsOptimizable (post_not_opt), Is.False, "NotOptimizable/Post");

			// [SnippetAttribute] subclasses can opt-in being optimizable
			var optin_opt = bgen.ApiAssembly.MainModule.GetType ("NS", "OptInOptimizable");
			Assert.That (optin_opt, Is.Not.Null, "OptInOptimizable");
			var pre_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Pre");
			Assert.That (IsOptimizable (pre_optin_opt), Is.True, "OptInOptimizable/Pre");
			var prologue_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Prologue");
			Assert.That (IsOptimizable (prologue_optin_opt), Is.True, "OptInOptimizable/Prologue");
			var post_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Post");
			Assert.That (IsOptimizable (post_optin_opt), Is.True, "OptInOptimizable/Post");

			// Without a [SnippetAttribute] subclass attribute the generated method is optimizable
			var nothing = bgen.ApiAssembly.MainModule.GetType ("NS", "NoSnippet").Methods.First ((v) => v.Name == "Nothing");
			Assert.That (nothing, Is.Not.Null, "NoSnippet");
			Assert.That (IsOptimizable (nothing), Is.True, "Nothing/Optimizable");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NativeEnum (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = true;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.Sources = new string [] { Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nativeenum-extensions.cs") }.ToList ();
			bgen.ApiDefinitions = new string [] { Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "nativeenum.cs") }.ToList ();
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
		}

		[Test]
		public void DelegateWithINativeObjectReturnType ()
		{
			var bgen = BuildFile (Profile.iOS, "delegate-with-inativeobject-return-type.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("ObjCRuntime", "Trampolines").NestedTypes.First (v => v.Name == "DMyHandler");
			Assert.That (type, Is.Not.Null, "DMyHandler");
			var method = type.Methods.First (v => v.Name == "Invoke");
			Assert.That (method.ReturnType.FullName, Is.EqualTo ("ObjCRuntime.NativeHandle"), "Return type");
		}

		[Test]
		public void ProtocolBindProperty ()
		{
			var bgen = BuildFile (Profile.iOS, "protocol-bind-property.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyProtocol_Extensions");
			Assert.That (type, Is.Not.Null, "MyProtocol_Extensions");

			var method = type.Methods.First (v => v.Name == "GetOptionalProperty");
			var ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.That ((string) ldstr.Operand, Is.EqualTo ("isOptionalProperty"), "isOptionalProperty");


			method = type.Methods.First (v => v.Name == "SetOptionalProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.That ((string) ldstr.Operand, Is.EqualTo ("setOptionalProperty:"), "setOptionalProperty");

			type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyProtocolWrapper");
			Assert.That (type, Is.Not.Null, "MyProtocolWrapper");

			method = type.Methods.First (v => v.Name == "get_AbstractProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.That ((string) ldstr.Operand, Is.EqualTo ("isAbstractProperty"), "isAbstractProperty");

			method = type.Methods.First (v => v.Name == "set_AbstractProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.That ((string) ldstr.Operand, Is.EqualTo ("setAbstractProperty:"), "setAbstractProperty");
		}

		[Test]
		public void AbstractTypeTest ()
		{
			var bgen = BuildFile (Profile.iOS, "abstract-type.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyObject");
			Assert.That (type, Is.Not.Null, "MyObject");
			Assert.That (type.IsAbstract, Is.False, "IsAbstract");

			var method = type.Methods.First (v => v.Name == ".ctor" && !v.HasParameters && !v.IsStatic);
			Assert.That (method.IsFamily, Is.True, "IsProtected ctor");

			method = type.Methods.First (v => v.Name == "AbstractMember" && !v.HasParameters && !v.IsStatic);
			var throwInstruction = method.Body?.Instructions?.FirstOrDefault (v => v.OpCode == OpCodes.Throw);
			Assert.That (method.IsPublic, Is.True, "IsPublic ctor");
			Assert.That (method.IsVirtual, Is.True, "IsVirtual");
			Assert.That (method.IsAbstract, Is.False, "IsAbstract");
			Assert.That (throwInstruction, Is.Not.Null, "Throw");
		}

		[Test]
		[Ignore ("https://github.com/dotnet/roslyn/issues/61525")]
		public void NativeIntDelegates ()
		{
			var bgen = BuildFile (Profile.iOS, "nint-delegates.cs");

			Func<string, bool> verifyDelegate = (typename) => {
				// Assert that the return type from the delegate is IntPtr
				var type = bgen.ApiAssembly.MainModule.GetType ("NS", typename);
				Assert.That (type, Is.Not.Null, typename);
				var method = type.Methods.First (m => m.Name == "Invoke");
				Assert.That (method.MethodReturnType.CustomAttributes.FirstOrDefault (attr => attr.AttributeType.Name == "NativeIntegerAttribute"), Is.Not.Null, "Return type for delegate " + typename);
				foreach (var p in method.Parameters) {
					Assert.That (p.CustomAttributes.FirstOrDefault (attr => attr.AttributeType.Name == "NativeIntegerAttribute"), Is.Not.Null, $"Parameter {p.Name}'s type for delegate " + typename);
				}

				return false;
			};

			verifyDelegate ("D1");
			verifyDelegate ("D2");
			verifyDelegate ("D3");
			verifyDelegate ("NSTableViewColumnRowPredicate");
		}

		[Test]
		public void CSharp10Syntax ()
		{
			BuildFile (Profile.iOS, "csharp10syntax.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void AttributesFromInlinedProtocols (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());

			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("attributes-from-inlined-protocols.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			var type = bgen.ApiAssembly.MainModule.GetType ("NS", "TypeA");

			var expectedAttributes = new string [] {
@"[BindingImpl(3)]
[Export(""someMethod1:"")]
[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[UnsupportedOSPlatform(""tvos"")]",

@"[BindingImpl(3)]
[Export(""someMethod2:"")]
[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[UnsupportedOSPlatform(""tvos"")]",

@"[BindingImpl(3)]
[Export(""someMethod3:"")]
[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[UnsupportedOSPlatform(""tvos"")]",

@"[BindingImpl(3)]
[Export(""someMethod4:"")]
[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[UnsupportedOSPlatform(""tvos"")]",
			};

			int someMethodCount = expectedAttributes.Length;
			var someMethod = new MethodDefinition [someMethodCount];
			var renderedSomeMethod = new string [someMethodCount];
			var failures = new List<string> ();
			for (var i = 0; i < someMethodCount; i++) {
				someMethod [i] = type.Methods.Single (v => v.Name == "SomeMethod" + (i + 1).ToString ());
				renderedSomeMethod [i] = string.Join ("\n", someMethod [i].CustomAttributes.Select (ca => RenderSupportedOSPlatformAttribute (ca)).OrderBy (v => v));

				expectedAttributes [i] = expectedAttributes [i].Replace ("\r", string.Empty);
				renderedSomeMethod [i] = renderedSomeMethod [i].Replace ("\r", string.Empty);

				if (expectedAttributes [i] == renderedSomeMethod [i])
					continue;

				var msg =
					$"{someMethod [i].Name} has different attributes.\n" +
					$"Expected attributes:\n" +
					expectedAttributes [i] + "\n" +
					"Actual attributes:\n" +
					renderedSomeMethod [i];
				Console.WriteLine ($"❌ {msg}\n");
				failures.Add (msg);
			}

			Assert.That (failures, Is.Empty, "Failures");
		}

		[Test]
		public void NFloatType ()
		{
			var bgen = BuildFile (Profile.iOS, "nfloat.cs");

			var messaging = bgen.ApiAssembly.MainModule.Types.FirstOrDefault (v => v.Name == "Messaging");
			Assert.That (messaging, Is.Not.Null, "Messaging");
			if (messaging is null)
				return;
			var pinvoke = messaging.Methods.FirstOrDefault (v => v.Name == "xamarin_nfloat_objc_msgSend_exception");
			Assert.That (pinvoke, Is.Not.Null, "PInvoke");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NoAvailabilityForAccessors (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("no-availability-for-accessors.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			bgen.AssertMethod ("NS.Whatever", "get_PropA");
			bgen.AssertNoMethod ("NS.Whatever", "set_PropA", parameterTypes: "Foundation.NSObject");
			bgen.AssertMethod ("NS.Whatever", "set_PropB", parameterTypes: "Foundation.NSObject");
			bgen.AssertNoMethod ("NS.Whatever", "get_PropB");
			bgen.AssertMethod ("NS.Whatever", "get_IPropA");
			bgen.AssertNoMethod ("NS.Whatever", "set_IPropA", parameterTypes: "Foundation.NSObject");
			bgen.AssertMethod ("NS.Whatever", "set_IPropB", parameterTypes: "Foundation.NSObject");
			bgen.AssertNoMethod ("NS.Whatever", "get_IPropB");
			bgen.AssertMethod ("NS.Whatever", "get_IPropAOpt");
			bgen.AssertNoMethod ("NS.Whatever", "set_IPropAOpt", parameterTypes: "Foundation.NSObject");
			bgen.AssertMethod ("NS.Whatever", "set_IPropBOpt", parameterTypes: "Foundation.NSObject");
			bgen.AssertNoMethod ("NS.Whatever", "get_IPropBOpt");
			bgen.AssertMethod ("NS.Whatever", ".ctor");
			bgen.AssertMethod ("NS.Whatever", ".ctor", parameterTypes: "Foundation.NSObjectFlag");
			bgen.AssertMethod ("NS.Whatever", ".ctor", parameterTypes: "ObjCRuntime.NativeHandle");
			bgen.AssertPublicMethodCount ("NS.Whatever", 10); // 6 accessors + 3 constructors + ClassHandle getter

			bgen.AssertMethod ("NS.IIProtocol", "get_IPropA");
			bgen.AssertMethod ("NS.IIProtocol", "get_IPropAOpt");
			bgen.AssertNoMethod ("NS.IIProtocol", "set_IPropA", parameterTypes: "Foundation.NSObject");
			bgen.AssertMethod ("NS.IIProtocol", "set_IPropB", parameterTypes: "Foundation.NSObject");
			bgen.AssertMethod ("NS.IIProtocol", "set_IPropBOpt", parameterTypes: "Foundation.NSObject");
			bgen.AssertNoMethod ("NS.IIProtocol", "get_IPropB");
			bgen.AssertPublicMethodCount ("NS.IIProtocol", 4);

			bgen.AssertMethod ("NS.IProtocol_Extensions", "GetIPropAOpt", parameterTypes: "NS.IIProtocol");
			bgen.AssertMethod ("NS.IProtocol_Extensions", "SetIPropBOpt", parameterTypes: new string [] { "NS.IIProtocol", "Foundation.NSObject" });
			bgen.AssertPublicMethodCount ("NS.IProtocol_Extensions", 2);
		}

		[Test]
		public void GeneratedAttributeOnPropertyAccessors ()
		{
			var bgen = BuildFile (Profile.MacCatalyst, "generated-attribute-on-property-accessors.cs");

			var messaging = bgen.ApiAssembly.MainModule.Types.First (v => v.Name == "ISomething");
			var property = messaging.Properties.First (v => v.Name == "IsLoadedInProcess");
			var getter = messaging.Methods.First (v => v.Name == "get_IsLoadedInProcess");
			var expectedPropertyAttributes =
@"[SupportedOSPlatform(""maccatalyst"")]
[SupportedOSPlatform(""macos"")]
[UnsupportedOSPlatform(""ios"")]
[UnsupportedOSPlatform(""tvos"")]";
			expectedPropertyAttributes = expectedPropertyAttributes.Replace ("\r", string.Empty);

			Assert.That (RenderSupportedOSPlatformAttributes (property), Is.EqualTo (expectedPropertyAttributes), "Property attributes");
			Assert.That (RenderSupportedOSPlatformAttributes (getter), Is.EqualTo (string.Empty), "Getter Attributes");
		}

		[Test]
		public void GeneratedAttributeOnPropertyAccessors2 ()
		{
			var bgen = BuildFile (Profile.MacCatalyst, "generated-attribute-on-property-accessors2.cs");

			var messaging = bgen.ApiAssembly.MainModule.Types.First (v => v.Name == "ISomething");
			var property = messaging.Properties.First (v => v.Name == "MicrophoneEnabled");
			var getter = messaging.Methods.First (v => v.Name == "get_MicrophoneEnabled");
			var setter = messaging.Methods.First (v => v.Name == "set_MicrophoneEnabled");

			var expectedPropertyAttributes =
@"[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[SupportedOSPlatform(""macos"")]
[UnsupportedOSPlatform(""tvos"")]";
			var expectedSetterAttributes =
@"[SupportedOSPlatform(""ios"")]
[SupportedOSPlatform(""maccatalyst"")]
[SupportedOSPlatform(""macos"")]
[UnsupportedOSPlatform(""tvos"")]";

			expectedPropertyAttributes = expectedPropertyAttributes.Replace ("\r", string.Empty);
			expectedSetterAttributes = expectedSetterAttributes.Replace ("\r", string.Empty);

			Assert.That (RenderSupportedOSPlatformAttributes (property), Is.EqualTo (expectedPropertyAttributes), "Property attributes");
			Assert.That (RenderSupportedOSPlatformAttributes (getter), Is.EqualTo (string.Empty), "Getter Attributes");
			Assert.That (RenderSupportedOSPlatformAttributes (setter), Is.EqualTo (expectedSetterAttributes), "Setter Attributes");
		}

		[Test]
		public void DynamicDependencyAttribute ()
		{
			var bgen = BuildFile (Profile.macOSMobile, "dynamic-dependency-attribute.cs");

			var type = bgen.ApiAssembly.MainModule.Types.First (v => v.Name == "MyClass");
			var getter = type.Methods.First (v => v.Name == "get_CurrentContext");
			var setter = type.Methods.First (v => v.Name == "set_CurrentContext");
			var doSomething = type.Methods.First (v => v.Name == "DoSomething");
			var doSomethingElse = type.Methods.First (v => v.Name == "DoSomethingElse");
			var doAnother = type.Methods.First (v => v.Name == "DoAnother");
			var doYetAnother = type.Methods.First (v => v.Name == "DoYetAnother");
			var doAll = type.Methods.First (v => v.Name == "DoAll");

			// (DynamicallyAccessedMemberTypes, string, string) on property getter
			var getterDDA = getter.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (getterDDA.Length, Is.EqualTo (1), "Getter DynamicDependency count");
			Assert.That ((int) getterDDA [0].ConstructorArguments [0].Value, Is.EqualTo (7), "Getter DynamicDependency MemberTypes (PublicConstructors | NonPublicConstructors)");
			Assert.That (getterDDA [0].ConstructorArguments [1].Value, Is.EqualTo ("Foundation.NSProxy"), "Getter DynamicDependency TypeName");
			Assert.That (getterDDA [0].ConstructorArguments [2].Value, Is.EqualTo ("Microsoft.macOS"), "Getter DynamicDependency AssemblyName");

			// Setter should not have it
			var setterDDA = setter.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (setterDDA.Length, Is.EqualTo (0), "Setter DynamicDependency count");

			// (string, string, string) on method
			var methodDDA = doSomething.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (methodDDA.Length, Is.EqualTo (1), "DoSomething DynamicDependency count");
			Assert.That (methodDDA [0].ConstructorArguments [0].Value, Is.EqualTo ("Create"), "DoSomething DynamicDependency MemberSignature");
			Assert.That (methodDDA [0].ConstructorArguments [1].Value, Is.EqualTo ("NS.MyClass"), "DoSomething DynamicDependency TypeName");
			Assert.That (methodDDA [0].ConstructorArguments [2].Value, Is.EqualTo ("api0"), "DoSomething DynamicDependency AssemblyName");

			// (string) - single member signature
			var elseDDA = doSomethingElse.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (elseDDA.Length, Is.EqualTo (1), "DoSomethingElse DynamicDependency count");
			Assert.That (elseDDA [0].ConstructorArguments.Count, Is.EqualTo (1), "DoSomethingElse DynamicDependency arg count");
			Assert.That (elseDDA [0].ConstructorArguments [0].Value, Is.EqualTo ("Activate"), "DoSomethingElse DynamicDependency MemberSignature");

			// (DynamicallyAccessedMemberTypes, Type)
			var anotherDDA = doAnother.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (anotherDDA.Length, Is.EqualTo (1), "DoAnother DynamicDependency count");
			Assert.That ((int) anotherDDA [0].ConstructorArguments [0].Value, Is.EqualTo (8 | 512), "DoAnother DynamicDependency MemberTypes (PublicMethods | PublicProperties)");
			Assert.That (((Mono.Cecil.TypeReference) anotherDDA [0].ConstructorArguments [1].Value).Name, Is.EqualTo ("NSObject"), "DoAnother DynamicDependency Type");

			// (string, Type)
			var yetAnotherDDA = doYetAnother.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (yetAnotherDDA.Length, Is.EqualTo (1), "DoYetAnother DynamicDependency count");
			Assert.That (yetAnotherDDA [0].ConstructorArguments [0].Value, Is.EqualTo ("Create"), "DoYetAnother DynamicDependency MemberSignature");
			Assert.That (((Mono.Cecil.TypeReference) yetAnotherDDA [0].ConstructorArguments [1].Value).Name, Is.EqualTo ("NSObject"), "DoYetAnother DynamicDependency Type");

			// (DynamicallyAccessedMemberTypes.All, string, string)
			var allDDA = doAll.CustomAttributes.Where (ca => ca.AttributeType.Name == "DynamicDependencyAttribute").ToArray ();
			Assert.That (allDDA.Length, Is.EqualTo (1), "DoAll DynamicDependency count");
			Assert.That ((int) allDDA [0].ConstructorArguments [0].Value, Is.EqualTo (-1), "DoAll DynamicDependency MemberTypes (All)");
			Assert.That (allDDA [0].ConstructorArguments [1].Value, Is.EqualTo ("NS.MyClass"), "DoAll DynamicDependency TypeName");
			Assert.That (allDDA [0].ConstructorArguments [2].Value, Is.EqualTo ("api0"), "DoAll DynamicDependency AssemblyName");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NewerAvailabilityInInlinedProtocol (Profile profile)
		{
			var bgen = BuildFile (profile, "newer-availability-in-inlined-protocol.cs");

			var expectedMethods = new [] {
				new {
					Type = "Whatever",
					MethodCount = 21,
					Methods = new [] {
						new { Method = "get_IPropA", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Method = "get_IPropAOpt", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Method = "set_IPropB", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "set_IPropBOpt", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "get_IPropC", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Method = "set_IPropC", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Method = "get_IPropCOpt", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Method = "set_IPropCOpt", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },

						new { Method = "get_IPropD", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "get_IPropDOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "set_IPropE", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "set_IPropEOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "get_IPropF", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "set_IPropF", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "get_IPropFOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Method = "set_IPropFOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
					},
				},
				new {
					Type = "IIProtocol",
					MethodCount = 17,
					Methods = new [] {
						new { Method = ".cctor", Attributes = "" },
						new { Method = "get_IPropA", Attributes = "" },
						new { Method = "_GetIPropA", Attributes = "" },
						new { Method = "set_IPropB", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "_SetIPropB", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "get_IPropC", Attributes = "" },
						new { Method = "_GetIPropC", Attributes = "" },
						new { Method = "set_IPropC", Attributes = "" },
						new { Method = "_SetIPropC", Attributes = "" },
						new { Method = "get_IPropAOpt", Attributes = "" },
						new { Method = "_GetIPropAOpt", Attributes = "" },
						new { Method = "set_IPropBOpt", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "_SetIPropBOpt", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "get_IPropCOpt", Attributes = "" },
						new { Method = "get_IPropCOpt", Attributes = "" },
						new { Method = "set_IPropCOpt", Attributes = "" },
						new { Method = "_SetIPropCOpt", Attributes = "" },
					},
				},
				new {
					Type = "IProtocol_Extensions",
					MethodCount = 4,
					Methods = new [] {
						new { Method = "GetIPropAOpt", Attributes = "" },
						new { Method = "SetIPropBOpt", Attributes = "[SupportedOSPlatform(\"tvos150.0\")]" },
						new { Method = "GetIPropCOpt", Attributes = "" },
						new { Method = "SetIPropCOpt", Attributes = "" },
					},
				},
				new {
					Type = "IIProtocolLower",
					MethodCount = 17,
					Methods = new [] {
						new { Method = ".cctor", Attributes = "" },
						new { Method = "get_IPropD", Attributes = "" },
						new { Method = "_GetIPropD", Attributes = "" },
						new { Method = "set_IPropE", Attributes = "[SupportedOSPlatform(\"tvos110.0\")]" },
						new { Method = "_SetIPropE", Attributes = "[SupportedOSPlatform(\"tvos110.0\")]" },
						new { Method = "get_IPropF", Attributes = "" },
						new { Method = "_GetIPropF", Attributes = "" },
						new { Method = "set_IPropF", Attributes = "" },
						new { Method = "_SetIPropF", Attributes = "" },
						new { Method = "get_IPropDOpt", Attributes = "" },
						new { Method = "_GetIPropDOpt", Attributes = "" },
						new { Method = "set_IPropEOpt", Attributes = "[SupportedOSPlatform(\"tvos110.0\")]" },
						new { Method = "_SetIPropEOpt", Attributes = "[SupportedOSPlatform(\"tvos110.0\")]" },
						new { Method = "get_IPropFOpt", Attributes = "" },
						new { Method = "_GetIPropFOpt", Attributes = "" },
						new { Method = "set_IPropFOpt", Attributes = "" },
						new { Method = "_SetIPropFOpt", Attributes = "" },
					},
				},
				new {
					Type = "IProtocolLower_Extensions",
					MethodCount = 4,
					Methods = new [] {
						new { Method = "GetIPropDOpt", Attributes = "" },
						new { Method = "SetIPropEOpt", Attributes = "[SupportedOSPlatform(\"tvos110.0\")]" },
						new { Method = "GetIPropFOpt", Attributes = "" },
						new { Method = "SetIPropFOpt", Attributes = "" },
					},
				},
			};

			var expectedProperties = new []  {
				new {
					Type = "Whatever",
					PropertyCount = 13,
					Properties = new [] {
						new { Property = "IPropA", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Property = "IPropB", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Property = "IPropAOpt", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Property = "IPropBOpt", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Property = "IPropC", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Property = "IPropCOpt", Attributes = "[SupportedOSPlatform(\"tvos130.0\")]" },
						new { Property = "IPropD", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Property = "IPropE", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Property = "IPropDOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Property = "IPropEOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Property = "IPropF", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
						new { Property = "IPropFOpt", Attributes = "[SupportedOSPlatform(\"tvos120.0\")]" },
					},
				},
				new {
					Type = "IIProtocol",
					PropertyCount = 6,
					Properties = new [] {
						new { Property = "IPropA", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Property = "IPropB", Attributes = "" },
						new { Property = "IPropC", Attributes = "" },
						new { Property = "IPropAOpt", Attributes = "[SupportedOSPlatform(\"tvos140.0\")]" },
						new { Property = "IPropBOpt", Attributes = "" },
						new { Property = "IPropCOpt", Attributes = "" },
					},
				},
				new {
					Type = "IProtocol_Extensions",
					PropertyCount = 0,
					Properties = new [] {
						new { Property = "fake property for c# anonymous type compilation", Attributes = "..." },
					},
				},
				new {
					Type = "IIProtocolLower",
					PropertyCount = 6,
					Properties = new [] {
						new { Property = "IPropD", Attributes = "[SupportedOSPlatform(\"tvos100.0\")]" },
						new { Property = "IPropE", Attributes = "" },
						new { Property = "IPropF", Attributes = "" },
						new { Property = "IPropDOpt", Attributes = "[SupportedOSPlatform(\"tvos100.0\")]" },
						new { Property = "IPropEOpt", Attributes = "" },
						new { Property = "IPropFOpt", Attributes = "" },
					},
				},
				new {
					Type = "IProtocolLower_Extensions",
					PropertyCount = 0,
					Properties = new [] {
						new { Property = "fake property for c# anonymous type compilation", Attributes = "..." },
					},
				},
			};

			var failures = new List<string> ();

			Assert.Multiple (() => {

				foreach (var expected in expectedMethods) {
					var type = bgen.ApiAssembly.MainModule.Types.FirstOrDefault (v => v.Name == expected.Type);
					Assert.That (type, Is.Not.Null, $"Type not found: {expected.Type}");
					if (type is null)
						continue;
					Assert.That (type.Methods.Count, Is.EqualTo (expected.MethodCount), $"Unexpected method count for {expected.Type}.\n\tActual methods:\n\t\t{string.Join ("\n\t\t", type.Methods.Select (v => v.FullName))}");
					if (expected.MethodCount == 0)
						continue;
					foreach (var expectedMember in expected.Methods) {
						var member = type.Methods.SingleOrDefault (v => v.Name == expectedMember.Method);
						Assert.That (member, Is.Not.Null, $"Method not found: {expectedMember.Method} in {type.FullName}");
						if (member is null)
							continue;
						var renderedAttributes = RenderSupportedOSPlatformAttributes (member);
						var expectedAttributes = expectedMember.Attributes.Replace ("\r", string.Empty);
						if (renderedAttributes != expectedAttributes) {
							var msg =
								$"Property: {type.FullName}::{member.Name}\n" +
								$"\tExpected attributes:\n" +
								$"\t\t{string.Join ("\n\t\t", expectedMember.Attributes.Split ('\n'))}\n" +
								$"\tActual attributes:\n" +
								$"\t\t{string.Join ("\n\t\t", renderedAttributes.Split ('\n'))}";
							failures.Add (msg);
							Console.WriteLine ($"❌ {msg}");
						}
					}
				}

				foreach (var expected in expectedProperties) {
					var type = bgen.ApiAssembly.MainModule.Types.FirstOrDefault (v => v.Name == expected.Type);
					Assert.That (type, Is.Not.Null, $"Type not found: {expected.Type}");
					if (type is null)
						continue;
					Assert.That (type.Properties.Count, Is.EqualTo (expected.PropertyCount), $"Unexpected property count for {expected.Type}.\n\tActual properties:\n\t\t{string.Join ("\n\t\t", type.Properties.Select (v => v.Name))}");
					if (expected.PropertyCount == 0)
						continue;
					foreach (var expectedMember in expected.Properties) {
						var member = type.Properties.SingleOrDefault (v => v.Name == expectedMember.Property);
						Assert.That (member, Is.Not.Null, $"Property not found: {expectedMember.Property} in {type.FullName}");
						if (member is null)
							continue;
						var renderedAttributes = RenderSupportedOSPlatformAttributes (member);
						var expectedAttributes = expectedMember.Attributes.Replace ("\r", string.Empty);
						if (renderedAttributes != expectedAttributes) {
							var msg =
								$"Property: {type.FullName}::{member.Name}\n" +
								$"\tExpected attributes:\n" +
								$"\t\t{string.Join ("\n\t\t", expectedMember.Attributes.Split ('\n'))}\n" +
								$"\tActual attributes:\n" +
								$"\t\t{string.Join ("\n\t\t", renderedAttributes.Split ('\n'))}";
							failures.Add (msg);
							Console.WriteLine ($"❌ {msg}");
						}
					}
				}
			});

			Assert.That (failures, Is.Empty, "Failures");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void ErrorDomain (Profile profile)
		{
			BuildFile (profile, true, true, "errordomain.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void ObsoletedOSPlatform (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.AddTestApiDefinition ("obsoletedosplatform.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
		}

		[Test]
		public void InternalDelegate ()
		{
			BuildFile (Profile.iOS, "internal-delegate.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.MacCatalyst)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.tvOS)]
		public void XmlDocs (Profile profile)
		{
			var bgen = BuildFile (profile, false, true, "xmldocs.cs");
			Assert.That (bgen.XmlDocumentation, Does.Exist);
			var contents = File.ReadAllText (bgen.XmlDocumentation);
			var expectedContentsPath = Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", $"ExpectedXmlDocs.{profile.AsPlatform ().AsString ()}.xml");
			if (!File.Exists (expectedContentsPath))
				File.WriteAllText (expectedContentsPath, string.Empty);

			var expectedContents = File.ReadAllText (expectedContentsPath);

			// Fix up a few potential whitespace differences we don't care about.
			contents = contents.Trim ().Replace ("\r", "");
			expectedContents = expectedContents.Trim ().Replace ("\r", "");

			if (contents != expectedContents) {
				if (!string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("WRITE_KNOWN_FAILURES"))) {
					File.WriteAllText (expectedContentsPath, contents);
					Assert.That (contents, Is.EqualTo (expectedContents), $"Xml docs: The known failures have been updated in {expectedContentsPath}, so please commit the results. Re-running the test should now succeed.");
				} else {
					Assert.That (contents, Is.EqualTo (expectedContents), $"Xml docs: If this is expected, set the WRITE_KNOWN_FAILURES=1 environment variable, run the test again, and commit the changes to the {expectedContentsPath} file.");
				}
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.MacCatalyst)]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.tvOS)]
		public void PreviewAPIs (Profile profile)
		{
			var bgen = BuildFile (profile, false, true, "preview.cs");

			// Each Experimental attribute in the api definition has its own diagnostic ID (with an incremental number)
			// Here we collect all diagnostic IDS for all the Experimental attributes in the compiled assembly,
			// and assert that they're all present at least once.
			var module = bgen.ApiAssembly.MainModule;
			var allExperimentalAttributes = module.GetCustomAttributes ().Where (v => v.AttributeType.Name == "ExperimentalAttribute");
			var allExperimentalDiagnosticIds = allExperimentalAttributes.Select (v => (string) v.ConstructorArguments [0].Value).ToHashSet ();
			var previewApiCount = 32;
			var expectedDiagnosticIds = Enumerable.Range (1, previewApiCount).Select (v => $"BGEN{v:0000}").ToHashSet ();

			var unexpectedDiagnosticIds = allExperimentalDiagnosticIds.Except (expectedDiagnosticIds).OrderBy (v => v);
			var missingDiagnosticIds = expectedDiagnosticIds.Except (allExperimentalDiagnosticIds).OrderBy (v => v);

			Assert.That (unexpectedDiagnosticIds, Is.Empty, "No unexpected diagnostic IDs"); // you probably need to increase the previewApiCount variable above (if you added more definitions to the tests/preview.cs file).
			Assert.That (missingDiagnosticIds, Is.Empty, "No missing diagnostic IDs");
		}

		[Test]
		public void DelegateParameterAttributes ()
		{
			BuildFile (Profile.iOS, "delegate-parameter-attributes.cs");
		}

		[Test]
		public void Issue19612 ()
		{
			var profile = Profile.iOS;
			var filename = Path.Combine (Configuration.SourceRoot, "tests", "bgen", "tests", "issue19612.cs");

			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());

			// Compile the temporary assembly and pass the compiled assembly to the generator instead
			// of relying on the generator to compile.
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var tmpassembly = Path.Combine (tmpdir, "temporaryAssembly.dll");
			var cscArguments = new List<string> ();
			if (!StringUtils.TryParseArguments (Configuration.DotNetCscCommand, out var cscCommand, out var ex))
				throw new InvalidOperationException ($"Unable to parse the .NET csc command '{Configuration.DotNetCscCommand}': {ex.Message}");
			cscArguments.AddRange (cscCommand);
			var cscExecutable = cscArguments [0];
			cscArguments.RemoveAt (0);
			cscArguments.Add (filename);
			cscArguments.Add ($"/out:{tmpassembly}");
			cscArguments.Add ("/target:library");
			cscArguments.Add ($"/r:{Path.Combine (Configuration.DotNetBclDir, "System.Runtime.dll")}");
			var tf = TargetFramework.Parse (BGenTool.GetTargetFramework (profile));
			cscArguments.Add ($"/r:{Configuration.GetBindingAttributePath (tf)}");
			cscArguments.Add ($"/r:{Configuration.GetBaseLibrary (tf)}");
			BGenTool.AddPreviewNoWarn (cscArguments);
			var rv = ExecutionHelper.Execute (cscExecutable, cscArguments);
			Assert.That (rv, Is.EqualTo (0), "CSC exit code");

			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.CompiledApiDefinitionAssembly = tmpassembly;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.NoWarn = "1123";
			bgen.CreateTemporaryBinding (filename);
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.MacCatalyst)]
		public void BackingFieldType (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, true, true, "backingfieldtype.cs");

			const string nintName = "System.IntPtr";
			const string nuintName = "System.UIntPtr";

			var testCases = new [] {
				new { BackingFieldType = "NSNumber", NullableType = "Foundation.NSNumber", RenderedBackingFieldType = "Foundation.NSNumber", SimplifiedNullableType = "Foundation.NSNumber" },
				new { BackingFieldType = "NSInteger", NullableType = $"System.Nullable`1<{nintName}>", RenderedBackingFieldType = nintName, SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "NSUInteger", NullableType = $"System.Nullable`1<{nuintName}>", RenderedBackingFieldType = nuintName, SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "Int32", NullableType = $"System.Nullable`1<System.Int32>", RenderedBackingFieldType = "System.Int32", SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "Int64", NullableType = $"System.Nullable`1<System.Int64>", RenderedBackingFieldType = "System.Int64", SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "UInt32", NullableType = $"System.Nullable`1<System.UInt32>", RenderedBackingFieldType = "System.UInt32", SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "UInt64", NullableType = $"System.Nullable`1<System.UInt64>", RenderedBackingFieldType = "System.UInt64", SimplifiedNullableType = "System.Nullable`1" },
				new { BackingFieldType = "NSString", NullableType = $"Foundation.NSString", RenderedBackingFieldType = "Foundation.NSString", SimplifiedNullableType = "Foundation.NSString" },
			};

			foreach (var tc in testCases) {
				var getConstant = bgen.ApiAssembly.MainModule.GetType ("BackingField", $"{tc.BackingFieldType}FieldTypeExtensions").Methods.First ((v) => v.Name == "GetConstant");
				Assert.That (getConstant.ReturnType.FullName, Is.EqualTo (tc.NullableType), $"{tc.BackingFieldType}: GetConstant return type");

				var getValue = bgen.ApiAssembly.MainModule.GetType ("BackingField", $"{tc.BackingFieldType}FieldTypeExtensions").Methods.First ((v) => v.Name == "GetValue");
				Assert.That (getValue.Parameters [0].ParameterType.FullName, Is.EqualTo (tc.RenderedBackingFieldType), $"{tc.BackingFieldType}: GetValue parameter type");

				var toEnumArray = bgen.ApiAssembly.MainModule.GetType ("BackingField", $"{tc.BackingFieldType}FieldTypeExtensions").Methods.First ((v) => v.Name == "ToEnumArray");
				Assert.That (toEnumArray.ReturnType.IsArray, Is.True, $"{tc.BackingFieldType} ToEnumArray return type IsArray");
				Assert.That (toEnumArray.ReturnType.GetElementType ().Name, Is.EqualTo ($"{tc.BackingFieldType}FieldType"), $"{tc.BackingFieldType} ToEnumArray return type");
				Assert.That (toEnumArray.Parameters [0].ParameterType.IsArray, Is.True, $"{tc.BackingFieldType} ToEnumArray parameter type IsArray");
				Assert.That (toEnumArray.Parameters [0].ParameterType.GetElementType ().FullName, Is.EqualTo (tc.RenderedBackingFieldType), $"{tc.BackingFieldType} ToEnumArray parameter type");

				var toConstantArray = bgen.ApiAssembly.MainModule.GetType ("BackingField", $"{tc.BackingFieldType}FieldTypeExtensions").Methods.First ((v) => v.Name == "ToConstantArray");
				Assert.That (toConstantArray.ReturnType.IsArray, Is.True, $"{tc.BackingFieldType} ToConstantArray return type IsArray");
				Assert.That (toConstantArray.ReturnType.GetElementType ().FullName, Is.EqualTo (tc.SimplifiedNullableType), $"{tc.BackingFieldType} ToConstantArray return type");
				Assert.That (toConstantArray.Parameters [0].ParameterType.IsArray, Is.True, $"{tc.BackingFieldType} ToConstantArray parameter type IsArray");
				Assert.That (toConstantArray.Parameters [0].ParameterType.GetElementType ().Name, Is.EqualTo ($"{tc.BackingFieldType}FieldType"), $"{tc.BackingFieldType} ToConstantArray parameter type");
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void UnderlyingFieldType (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			BuildFile (profile, true, true, "underlyingfieldtype.cs");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void FieldNullability (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "field-nullability.cs");

			var generatedFile = Path.Combine (bgen.TmpDirectory!, "FieldNullability", "FieldConstants.g.cs");
			Assert.That (File.Exists (generatedFile), Is.True, "Generated file exists");
			var contents = File.ReadAllText (generatedFile);
			Assert.That (contents, Does.Contain ("public static NSString? NullableString"), "Nullable field");
			Assert.That (contents, Does.Contain ("public static NSString NonNullableString"), "Non-nullable field");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void NativeFieldGeneration (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "native-field-generation.cs");

			var structFile = Path.Combine (bgen.TmpDirectory!, "NativeFieldGeneration", "NativeStructFields.g.cs");
			Assert.That (File.Exists (structFile), Is.True, "Generated struct file exists");
			var structContents = File.ReadAllText (structFile);
			Assert.That (structContents, Does.Contain ("internal unsafe static partial class NativeStructFields"), "Internal field container");
			Assert.That (structContents, Does.Contain ("return Dlfcn.GetStruct<CGRect> (Libraries.__Internal.Handle, \"RequiredStruct\");"), "Struct");
			Assert.That (structContents, Does.Contain ("public static NSString? NullableString"), "Nullable field");

			var addressFile = Path.Combine (bgen.TmpDirectory!, "NativeFieldGeneration", "SymbolAddresses.g.cs");
			Assert.That (File.Exists (addressFile), Is.True, "Generated address file exists");
			var addressContents = File.ReadAllText (addressFile);
			Assert.That (addressContents, Does.Contain ("[Field (\"CallbackTable\",  \"__Internal\", SymbolAddress = true)]"), "Symbol address attribute");
			Assert.That (addressContents, Does.Contain ("return Dlfcn.GetIndirect (Libraries.__Internal.Handle, \"CallbackTable\");"), "Symbol address");
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		[TestCase (Profile.MacCatalyst)]
		[TestCase (Profile.macOSMobile)]
		public void AvailabilityAttributes (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "availability-attributes.cs");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void DelegatesWithNullableReturnType (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "delegate-nullable-return.cs");
			bgen.AssertNoWarnings ();

			var delegateCallback = bgen.ApiAssembly.MainModule.GetType ("NS", "MyCallback").Methods.First ((v) => v.Name == "EndInvoke");
			Assert.That (delegateCallback.MethodReturnType.CustomAttributes.Any (v => v.AttributeType.Name == "NullableAttribute"), "Nullable return type");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void GenericTypeNullability (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "generic-type-nullability.cs");
			bgen.AssertNoWarnings ();

			// Find the generated source file and check the property signatures
			var generatedFile = Path.Combine (bgen.TmpDirectory!, "NS", "Widget.g.cs");
			Assert.That (File.Exists (generatedFile), Is.True, "Generated file exists");
			var contents = File.ReadAllText (generatedFile);

			// Basic: two nullable generic args
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSError?>?"), "AuthenticateHandler should have nullable generic args");
			// Three nullable generic args
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSArray?, NSError?>?"), "CompletionHandler should have nullable generic args");
			// Non-nullable generic args should NOT have ?
			Assert.That (contents, Does.Contain ("Action<NSObject, NSError>?"), "NonNullableHandler should NOT have nullable generic args");

			// Value type between nullable reference types (int should never get ?)
			Assert.That (contents, Does.Contain ("Action<NSObject?, int, NSError?>?"), "WithValueType should not annotate value types");

			// Four nullable reference type args
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSString?, NSArray?, NSError?>?"), "ManyNullableArgs should handle 4 nullable args");

			// Mixed: first and last non-nullable, middle nullable
			Assert.That (contents, Does.Contain ("Action<NSObject, NSString?, NSError>?"), "MixedMiddleNullable should only annotate the middle arg");

			// Multiple value types (int, bool should never get ?)
			Assert.That (contents, Does.Contain ("Action<NSObject?, int, bool, NSError?>?"), "MultipleValueTypes should not annotate any value types");

			// Alternating nullable/non-nullable pattern
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSString, NSArray?, NSError, NSObject?>?"), "AlternatingNullability should preserve alternating pattern");

			// All non-nullable (5 reference type args, none should get ?)
			Assert.That (contents, Does.Contain ("Action<NSObject, NSString, NSArray, NSError, NSObject>?"), "AllNonNullable should not annotate any args");

			// Value type at the end
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSError?, int>?"), "ValueTypeAtEnd should not annotate trailing value type");

			// === Method parameter assertions ===

			// Method with nullable Action<NSObject?> parameter
			Assert.That (contents, Does.Contain ("Action<NSObject?>"), "DoSomething should have nullable generic arg in method parameter");

			// Method with mixed nullable/non-nullable Action parameter
			Assert.That (contents, Does.Contain ("Action<NSObject?, NSError>"), "DoSomethingElse should have mixed nullability in method parameter");

			// Async method: completion handler with nullable NSError should generate Tuple<bool,NSError?>
			Assert.That (contents, Does.Contain ("Action<bool, NSError?>"), "ConfirmAcquired should have nullable NSError in method parameter");
			Assert.That (contents, Does.Contain ("Tuple<bool,NSError?>"), "ConfirmAcquired async should generate Tuple with nullable NSError");

			// Async method: completion handler with non-nullable NSError should generate Tuple<bool,NSError>
			Assert.That (contents, Does.Contain ("Action<bool, NSError>"), "ConfirmAcquiredNonNull should have non-nullable NSError in method parameter");
			Assert.That (contents, Does.Contain ("Tuple<bool,NSError>"), "ConfirmAcquiredNonNull async should generate Tuple with non-nullable NSError");

			// Async method with array arg before NSError (depth-first byte counting)
			Assert.That (contents, Does.Contain ("Action<NSObject[]?, NSError?>"), "FetchItems should have nullable array and NSError");
			// When NSError is nullable, async uses Task<T> with error→exception; the result type preserves nullability
			Assert.That (contents, Does.Contain ("Task<NSObject[]?>"), "FetchItems async should return Task<NSObject[]?> (array nullability preserved)");

			// Async method with nullable result type
			Assert.That (contents, Does.Contain ("Task<NSObject?>"), "LoadData async should return Task<NSObject?>");
			// Async method with non-nullable result type
			Assert.That (contents, Does.Match (@"Task<NSObject>\s"), "LoadDataNonNull async should return Task<NSObject>");

			// Async method with nullable array result type
			Assert.That (contents, Does.Contain ("Task<NSObject[]?>"), "LoadItems async should return Task<NSObject[]?>");
			// Async method with non-nullable array result type
			Assert.That (contents, Does.Match (@"Task<NSObject\[\]>\s"), "LoadItemsNonNull async should return Task<NSObject[]>");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void DelegatesWithPointerTypes (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "delegate-types.cs");
			bgen.AssertNoWarnings ();

			var delegateCallback = bgen.ApiAssembly.MainModule.GetType ("NS", "MyCallback").Methods.First ((v) => v.Name == "EndInvoke");
			Assert.That (delegateCallback.MethodReturnType.ReturnType.IsPointer, Is.True, "Pointer return type");
			foreach (var p in delegateCallback.Parameters.Where (v => v.Name != "result")) {
				Assert.That (p.ParameterType.IsPointer, Is.True, $"Pointer parameter type: {p.Name}");
			}
		}

		[Test]
		[TestCase (Profile.MacCatalyst)]
		public void ProtocolWithBaseTypeButNoModel (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, false, "protocol-and-basetype-no-model.cs");
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1123, "The type Protocols.ProtocolWithBaseTypeButNoModel has a [Protocol] and a [BaseType] attribute, but no [Model] attribute. This is likely incorrect; either remove the [BaseType] attribute, or add a [Model] attribute.");
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void DesignatedInitializer (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "designated-initializer-issue-10106.cs");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void ReleaseAttribute (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "release-attribute.cs");
			bgen.AssertNoWarnings ();

			var passesOwnsEqualsTrue = new Func<MethodDefinition, bool> ((method) => {
				foreach (var ins in method.Body.Instructions) {
					switch (ins.OpCode.Code) {
					case Code.Call:
					case Code.Calli:
					case Code.Callvirt:
						var mr = (MethodReference) ins.Operand;
						switch (mr.Name) {
						case "GetINativeObject":
						case "GetNSObject":
						case "FromHandle":
							var prev = ins.Previous;
							return prev.OpCode.Code == Code.Ldc_I4_1;
						}
						break;
					}
				}
				return false;
			});

			// The last argument in the call to GetNSObject, GetINativeObject or FromHandle (or any other object-creating methods) must be 'true'.
			var methods = bgen.ApiAssembly.MainModule.GetType ("NS", "ReleaseAttributeTest").Methods
								.Where ((v) => !v.IsConstructor)
								.Where (v => v.Name != "get_ClassHandle");
			Assert.Multiple (() => {
				foreach (var method in methods)
					Assert.That (passesOwnsEqualsTrue (method), Is.True, method.Name);
			});
		}

		[Test]
		[TestCase (Profile.iOS)]
		public void BothProtectedAndInternal (Profile profile)
		{
			// https://github.com/dotnet/macios/issues/6889
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, "both-protected-and-internal.cs");
			bgen.AssertNoWarnings ();
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		public void SimulatorAvailabilityAttributes (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, true, true, "simulator-availability-attributes.cs");
			bgen.AssertNoWarnings ();

			var module = bgen.ApiAssembly.MainModule;

			// Verify [UnsupportedSimulator] is copied for the current platform
			var unsupportedAll = module.GetType ("NS", "UnsupportedOnAllSimulators");
			var unsupportedAttrs = unsupportedAll.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (unsupportedAttrs.Length, Is.EqualTo (1), "UnsupportedOnAllSimulators: one attribute for current platform");
			var platformName = (string) unsupportedAttrs [0].ConstructorArguments [0].Value;
			var expectedPlatform = profile == Profile.iOS ? "ios" : "tvos";
			Assert.That (platformName, Is.EqualTo (expectedPlatform), "UnsupportedOnAllSimulators platform name");

			// Verify simulator attributes are copied from methods too
			var unsupportedMethod = unsupportedAll.Methods.Single (m => m.Name == "UnsupportedMethod");
			var unsupportedMethodAttrs = unsupportedMethod.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (unsupportedMethodAttrs.Length, Is.EqualTo (1), "UnsupportedMethod: one attribute for current platform");
			Assert.That ((string) unsupportedMethodAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedPlatform), "UnsupportedMethod platform name");

			var supportedMethod = unsupportedAll.Methods.Single (m => m.Name == "SupportedMethod");
			var supportedMethodAttrs = supportedMethod.CustomAttributes
				.Where (a => a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (supportedMethodAttrs.Length, Is.EqualTo (1), "SupportedMethod: one attribute for current platform");
			var expectedMethodVersion = profile == Profile.iOS ? "ios17.0" : "tvos17.0";
			Assert.That ((string) supportedMethodAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedMethodVersion), "SupportedMethod platform name");

			// Verify only the current platform's attribute is emitted
			var iosOnly = module.GetType ("NS", "UnsupportedOnIosSimulatorOnly");
			var iosOnlyAttrs = iosOnly.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute")
				.ToArray ();
			if (profile == Profile.iOS)
				Assert.That (iosOnlyAttrs.Length, Is.EqualTo (1), "UnsupportedOnIosSimulatorOnly: present for iOS");
			else
				Assert.That (iosOnlyAttrs.Length, Is.EqualTo (0), "UnsupportedOnIosSimulatorOnly: absent for tvOS");

			// Verify [SupportedSimulator] with version is copied
			var supported = module.GetType ("NS", "SupportedOnSimulatorFromVersion");
			var supportedAttrs = supported.CustomAttributes
				.Where (a => a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (supportedAttrs.Length, Is.EqualTo (1), "SupportedOnSimulatorFromVersion: one attribute");
			var expectedVersion = profile == Profile.iOS ? "ios17.0" : "tvos17.0";
			Assert.That ((string) supportedAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedVersion), "SupportedOnSimulatorFromVersion platform name");

			// Verify no simulator attributes when none are specified
			var noAttrs = module.GetType ("NS", "NoSimulatorAttributes");
			var simulatorAttrs = noAttrs.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (simulatorAttrs.Length, Is.EqualTo (0), "NoSimulatorAttributes: no simulator attributes");

			// Verify simulator attributes on methods are copied for the current platform
			var methods = module.GetType ("NS", "SimulatorAvailabilityMethods");
			var unsupportedSimulatorMethod = methods.Methods.Single (m => m.Name == "Unsupported");
			var unsupportedSimulatorMethodAttrs = unsupportedSimulatorMethod.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (unsupportedSimulatorMethodAttrs.Length, Is.EqualTo (1), "Unsupported method: one UnsupportedSimulator attribute");
			Assert.That ((string) unsupportedSimulatorMethodAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedPlatform), "Unsupported method platform name");

			var supportedSimulatorMethod = methods.Methods.Single (m => m.Name == "Supported");
			var supportedSimulatorMethodAttrs = supportedSimulatorMethod.CustomAttributes
				.Where (a => a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (supportedSimulatorMethodAttrs.Length, Is.EqualTo (1), "Supported method: one SupportedSimulator attribute");
			Assert.That ((string) supportedSimulatorMethodAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedVersion), "Supported method platform name");

			var plainMethod = methods.Methods.Single (m => m.Name == "Plain");
			var plainMethodAttrs = plainMethod.CustomAttributes
				.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (plainMethodAttrs.Length, Is.EqualTo (0), "Plain method: no simulator attributes");

			// Verify a [SupportedSimulator] on a smart-enum [Field] member is propagated to the generated accessor
			var smartExtensions = module.GetType ("NS", "SmartEnumWithSimulatorFieldExtensions");
			Assert.That (smartExtensions, Is.Not.Null, "SmartEnumWithSimulatorFieldExtensions: generated");
			var supportedAccessor = smartExtensions.Properties.Single (p => p.Name == "SupportedSmartField");
			var accessorAttrs = supportedAccessor.CustomAttributes
				.Where (a => a.AttributeType.Name == "SupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (accessorAttrs.Length, Is.EqualTo (1), "SupportedSmartField accessor: one SupportedSimulator attribute");
			var expectedSmartVersion = profile == Profile.iOS ? "ios17.0" : "tvos17.0";
			Assert.That ((string) accessorAttrs [0].ConstructorArguments [0].Value, Is.EqualTo (expectedSmartVersion), "SupportedSmartField accessor platform name");

			// And a smart-enum member without simulator attributes must not gain any
			var plainAccessor = smartExtensions.Properties.Single (p => p.Name == "PlainSmartField");
			var plainAccessorAttrs = plainAccessor.CustomAttributes
				.Where (a => a.AttributeType.Name == "SupportedSimulatorAttribute" || a.AttributeType.Name == "UnsupportedSimulatorAttribute")
				.ToArray ();
			Assert.That (plainAccessorAttrs.Length, Is.EqualTo (0), "PlainSmartField accessor: no simulator attributes");
		}

		[Test]
		[TestCase (Profile.macOSMobile)]
		[TestCase (Profile.MacCatalyst)]
		public void SimulatorAvailabilityAttributes_NotEmittedForMacPlatforms (Profile profile)
		{
			Configuration.IgnoreIfIgnoredPlatform (profile.AsPlatform ());
			var bgen = BuildFile (profile, true, true, "simulator-availability-attributes.cs");
			bgen.AssertNoWarnings ();

			var module = bgen.ApiAssembly.MainModule;
			foreach (var typeName in new [] { "UnsupportedOnAllSimulators", "UnsupportedOnIosSimulatorOnly", "SupportedOnSimulatorFromVersion", "NoSimulatorAttributes" }) {
				var type = module.GetType ("NS", typeName);
				var simulatorAttrs = type.CustomAttributes
					.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
					.ToArray ();
				Assert.That (simulatorAttrs.Length, Is.EqualTo (0), $"{typeName}: no simulator attributes on Mac platforms");
			}

			var unsupportedAll = module.GetType ("NS", "UnsupportedOnAllSimulators");
			foreach (var methodName in new [] { "UnsupportedMethod", "SupportedMethod" }) {
				var method = unsupportedAll.Methods.Single (m => m.Name == methodName);
				var simulatorAttrs = method.CustomAttributes
					.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
					.ToArray ();
				Assert.That (simulatorAttrs.Length, Is.EqualTo (0), $"{methodName}: no simulator attributes on Mac platforms");
			}

			var methods = module.GetType ("NS", "SimulatorAvailabilityMethods");
			foreach (var method in methods.Methods.Where (m => !m.IsConstructor)) {
				var methodAttrs = method.CustomAttributes
					.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
					.ToArray ();
				Assert.That (methodAttrs.Length, Is.EqualTo (0), $"{method.Name}: no simulator attributes on Mac platforms");
			}

			// The smart-enum field accessor must not carry simulator attributes on Mac platforms either
			var smartExtensions = module.GetType ("NS", "SmartEnumWithSimulatorFieldExtensions");
			Assert.That (smartExtensions, Is.Not.Null, "SmartEnumWithSimulatorFieldExtensions: generated");
			foreach (var property in smartExtensions.Properties) {
				var accessorAttrs = property.CustomAttributes
					.Where (a => a.AttributeType.Name == "UnsupportedSimulatorAttribute" || a.AttributeType.Name == "SupportedSimulatorAttribute")
					.ToArray ();
				Assert.That (accessorAttrs.Length, Is.EqualTo (0), $"{property.Name} accessor: no simulator attributes on Mac platforms");
			}
		}
	}
}
