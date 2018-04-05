using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Tests;

namespace GeneratorTests
{
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGenTests
	{
		[Test]
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void BMac_Smoke (Profile profile)
		{
			BuildFile (profile, "bmac_smoke.cs");
		}

		[Test]
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void BMac_With_Hyphen_In_Name (Profile profile)
		{
			BuildFile (profile, "bmac-with-hyphen-in-name.cs");
		}

		[Test]
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void PropertyRedefinitionMac (Profile profile)
		{
			BuildFile (profile, "property-redefination-mac.cs");
		}

		[Test]
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void NSApplicationPublicEnsureMethods (Profile profile)
		{
			BuildFile (profile, "NSApplicationPublicEnsureMethods.cs");
		}

		[Test]
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
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
		public void Bug15799 ()
		{
			BuildFile (Profile.iOS, "bug15799.cs");
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
		public void Bug23041 ()
		{
			BuildFile (Profile.iOS, "bug23041.cs");
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
			Assert.AreEqual (28, preserves, "Preserve attribute count"); // If you modified code that generates PreserveAttributes please update the preserve count
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
		[TestCase (Profile.macOSClassic)]
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSMobile)]
		public void Bug31788 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug31788.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();

			bgen.AssertApiCallsMethod ("Test", "MarshalInProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend", "MarshalInProperty.Shared getter");
			bgen.AssertApiCallsMethod ("Test", "MarshalOnProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend", "MarshalOnProperty.Shared getter");
		}

		[Test]
		public void Bug34042 ()
		{
			BuildFile (Profile.iOS, "bug34042.cs");
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

			var preserves = allMembers.Sum ((v) => v.CustomAttributes.Count ((ca) => ca.AttributeType.Name == "IntroducedAttribute"));
			Assert.AreEqual (8, preserves, "Introduced attribute count"); // If you modified code that generates IntroducedAttributes please update the attribute count
		}

		[Test]
		public void Bug36457 ()
		{
			BuildFile (Profile.iOS, "bug36457.cs");
		}

		[Test]
		public void Bug39614 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("bug39614.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1103, "'FooType`1' does not live under a namespace; namespaces are a highly recommended .NET best practice");
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
			Assert.AreEqual (24, preserves, "Advice attribute count"); // If you modified code that generates AdviceAttributes please update the attribute count
		}

		[Test]
		public void Bug43579 ()
		{
			BuildFile (Profile.iOS, "bug43579.cs");
		}

		[Test]
		public void Bug46292 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
			Assert.AreEqual (2, attribCount, "attribute count");
		}

		[Test]
		public void Bug53076 ()
		{
			var bgen = BuildFile (Profile.iOS, "bug53076.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMethods = bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((type) => type.Methods);

			// Count all *Async methods whose first parameter is 'IMyFooProtocol'.
			var methodCount = allMethods.Count ((v) => v.Name.EndsWith ("Async", StringComparison.Ordinal) && v.Parameters.Count > 0 && v.Parameters [0].ParameterType.Name == "IMyFooProtocol");
			Assert.AreEqual (10, methodCount, "Async method count");
		}

		[Test]
		public void Bug53076WithModel ()
		{
			var bgen = BuildFile (Profile.iOS, "bug53076withmodel.cs");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMethods = bgen.ApiAssembly.MainModule.GetTypes ().SelectMany ((type) => type.Methods);

			// Count all *Async methods whose first parameter is 'IMyFooProtocol'.
			var methodCount = allMethods.Count ((v) => v.Name.EndsWith ("Async", StringComparison.Ordinal) && v.Parameters.Count > 0 && v.Parameters [0].ParameterType.Name == "IMyFooProtocol");
			Assert.AreEqual (10, methodCount, "Async method count");
		}

		[Test]
		public void StackOverflow20696157 ()
		{
			BuildFile (Profile.iOS, "sof20696157.cs");
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
		[Ignore ("Fails with: api.cs(10,17): error CS0246: The type or namespace name `CBUUID' could not be found. Are you missing `CoreBluetooth' using directive?")]
		public void GenericStrongDictionary ()
		{
			BuildFile (Profile.iOS, "generic-strong-dictionary.cs");
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
		public void ClassNameCollision ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.Sources.Add (Path.Combine (Configuration.SourceRoot, "tests", "generator", "classNameCollision-enum.cs"));
			bgen.ApiDefinitions.Add (Path.Combine (Configuration.SourceRoot, "tests", "generator", "classNameCollision.cs"));
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void VirtualWrap ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
		public void NoAsyncInternalWrapper ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("noasyncinternalwrapper.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");

			var allTypes = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allMembers = ((IEnumerable<MemberReference>) allTypes)
				.Union (allTypes.SelectMany ((type) => type.Methods))
				.Union (allTypes.SelectMany ((type) => type.Fields))
				.Union (allTypes.SelectMany ((type) => type.Properties));

			Assert.AreEqual (1, allMembers.Count ((member) => member.Name == "RequiredMethodAsync"), "Expected 1 RequiredMethodAsync members in generated code. If you modified code that generates RequiredMethodAsync (AsyncAttribute) please update the RequiredMethodAsync count.");

			var attribs = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
			bgen.AssertMethod ("NoAsyncInternalWrapperTests.MyFooDelegate_Extensions", "RequiredMethodAsync", attribs, "System.Threading.Tasks.Task", "NoAsyncInternalWrapperTests.IMyFooDelegate", "System.Int32");
		}

		[Test]
		public void NoAsyncWarningCS0219 ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("noasyncwarningcs0219.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void FieldEnumTests ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.ProcessEnums = true;
			bgen.AddTestApiDefinition ("fieldenumtests.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();
		}

		[Test]
		public void SmartEnumWithFramework ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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

			Assert.AreEqual (12, getINativeObjectCalls, "Preserve attribute count"); // If you modified code that generates PreserveAttributes please update the preserve count
		}

		[Test]
		public void IsDirectBinding ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/is-direct-binding.cs");

			var callsMethod = new Func<MethodDefinition, string, bool> ((method, name) => {
				return method.Body.Instructions.Any ((ins) => {
					switch (ins.OpCode.Code) {
					case Code.Call:
					case Code.Calli:
					case Code.Callvirt:
						var mr = ins.Operand as MethodReference;
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
		public void Issue3875 ()
		{
			var bgen = BuildFile (Profile.iOS, "issue3875.cs");
			var attrib = bgen.ApiAssembly.MainModule.GetType ("Issue3875", "AProtocol").CustomAttributes.Where ((v) => v.AttributeType.Name == "RegisterAttribute").First ();
			Assert.AreEqual ("api0__Issue3875_AProtocol", attrib.ConstructorArguments [0].Value, "Custom ObjC name");
		}

		BGenTool BuildFile (Profile profile, params string [] filenames)
		{
			return BuildFile (profile, true, false, filenames);
		}

		BGenTool BuildFile (Profile profile, bool nowarnings, params string [] filenames)
		{
			return BuildFile (profile, nowarnings, false, filenames);
		}

		BGenTool BuildFile (Profile profile, bool nowarnings, bool processEnums, params string [] filenames)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = processEnums;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			TestContext.Out.WriteLine (TestContext.CurrentContext.Test.FullName);
			foreach (var filename in filenames)
				TestContext.Out.WriteLine ($"\t{filename}");
			bgen.CreateTemporaryBinding (filenames.Select ((filename) => File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", filename))).ToArray ());
			bgen.AssertExecute ("build");
			if (nowarnings)
				bgen.AssertNoWarnings ();
			return bgen;
		}
	}
}
