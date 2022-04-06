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
#if NET
		[Ignore ("Ignore this until done https://github.com/xamarin/maccore/issues/2549")]
#endif
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class BGenTests
	{
		// Removing the following variable might make running the unit tests in VSMac fail.
		static Type variable_to_keep_reference_to_system_runtime_compilerservices_unsafe_assembly = typeof (System.Runtime.CompilerServices.Unsafe);

		[Test]
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
		[TestCase (Profile.macOSMobile)]
		public void BMac_Smoke (Profile profile)
		{
			BuildFile (profile, "bmac_smoke.cs");
		}

#if !NET // There's no System.Drawing in the .NET BCL
		[Test]
		[TestCase (Profile.macOSSystem)]
		public void BMac_NonAbsoluteReference_StillBuilds (Profile profile)
		{
			BuildFile (profile, true, false, new List<string> () { "System.Drawing" }, "bmac_smoke.cs");
		}
#endif

#if !NET
		[Test]
		[TestCase (Profile.macOSSystem)]
		public void BMac_AbsoluteSystemReference_StillBuilds (Profile profile)
		{
			BuildFile (profile, true, false, new List<string> () { "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/System.Drawing.dll" }, "bmac_smoke.cs");
		}
#endif

		[Test]
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
		[TestCase (Profile.macOSMobile)]
		public void BMac_With_Hyphen_In_Name (Profile profile)
		{
			BuildFile (profile, "bmac-with-hyphen-in-name.cs");
		}

		[Test]
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
		[TestCase (Profile.macOSMobile)]
		public void PropertyRedefinitionMac (Profile profile)
		{
			BuildFile (profile, "property-redefination-mac.cs");
		}

		[Test]
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
		[TestCase (Profile.macOSMobile)]
		public void NSApplicationPublicEnsureMethods (Profile profile)
		{
			BuildFile (profile, "NSApplicationPublicEnsureMethods.cs");
		}

		[Test]
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
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

#if !NET
		// error BI1055: bgen: Internal error: failed to convert type 'System.Runtime.Versioning.SupportedOSPlatformAttribute, System.Runtime, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'. Please file a bug report (https://github.com/xamarin/xamarin-macios/issues/new) with a test case.
		[Test]
		public void Bug15799 ()
		{
			BuildFile (Profile.iOS, "bug15799.cs");
		}
#endif

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

#if !NET
		// error BI1055: bgen: Internal error: failed to convert type 'System.Runtime.Versioning.SupportedOSPlatformAttribute, System.Runtime, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'. Please file a bug report (https://github.com/xamarin/xamarin-macios/issues/new) with a test case.
		[Test]
		public void Bug23041 ()
		{
			BuildFile (Profile.iOS, "bug23041.cs");
		}
#endif

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
#if !NET
		[TestCase (Profile.macOSFull)]
		[TestCase (Profile.macOSSystem)]
#endif
		[TestCase (Profile.macOSMobile)]
		public void Bug31788 (Profile profile)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.CreateTemporaryBinding (File.ReadAllText (Path.Combine (Configuration.SourceRoot, "tests", "generator", "bug31788.cs")));
			bgen.AssertExecute ("build");
			bgen.AssertNoWarnings ();

#if NET
			bgen.AssertApiCallsMethod ("Test", "MarshalInProperty", "get_Shared", "xamarin_NativeHandle_objc_msgSend_exception", "MarshalInProperty.Shared getter");
			bgen.AssertApiCallsMethod ("Test", "MarshalOnProperty", "get_Shared", "xamarin_NativeHandle_objc_msgSend_exception", "MarshalOnProperty.Shared getter");
#else
			bgen.AssertApiCallsMethod ("Test", "MarshalInProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend_exception", "MarshalInProperty.Shared getter");
			bgen.AssertApiCallsMethod ("Test", "MarshalOnProperty", "get_Shared", "xamarin_IntPtr_objc_msgSend_exception", "MarshalOnProperty.Shared getter");
#endif
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
#if NET
			const string attrib = "SupportedOSPlatformAttribute";
#else
			const string attrib = "IntroducedAttribute";
#endif
			var preserves = allMembers.Sum ((v) => v.CustomAttributes.Count ((ca) => ca.AttributeType.Name == attrib));
			Assert.AreEqual (10, preserves, "Introduced attribute count"); // If you modified code that generates IntroducedAttributes please update the attribute count
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

#if !NET
		// error BI1055: bgen: Internal error: failed to convert type 'System.Runtime.Versioning.SupportedOSPlatformAttribute, System.Runtime, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'. Please file a bug report (https://github.com/xamarin/xamarin-macios/issues/new) with a test case.
		[Test]
		public void StackOverflow20696157 ()
		{
			BuildFile (Profile.iOS, "sof20696157.cs");
		}
#endif

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
		public void GHIssue3869 () => BuildFile (Profile.iOS, "ghissue3869.cs");

		[Test]
#if NET
		[TestCase ("issue3875.cs", "api0__Issue3875_AProtocol")]
#else
		[TestCase ("issue3875.cs", "AProtocol")]
#endif
		[TestCase ("issue3875B.cs", "BProtocol")]
		[TestCase ("issue3875C.cs", "api0__Issue3875_AProtocol")]
		public void Issue3875 (string file, string modelName)
		{
			var bgen = BuildFile (Profile.iOS, file);
			var attrib = bgen.ApiAssembly.MainModule.GetType ("Issue3875", "AProtocol").CustomAttributes.Where ((v) => v.AttributeType.Name == "RegisterAttribute").First ();
			Assert.AreEqual (modelName, attrib.ConstructorArguments [0].Value, "Custom ObjC name");
		}

		[Test]
		public void GHIssue5444 () => BuildFile (Profile.iOS, "ghissue5444.cs");

		[Test]
		public void GH5416_method ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.AddTestApiDefinition ("ghissue5416b.cs");
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
			bgen.AssertWarning (1118, "[NullAllowed] should not be used on methods, like 'Foundation.NSString Method(Foundation.NSDate, Foundation.NSObject)', but only on properties, parameters and return values.");
		}

		[Test]
		public void GH5416_setter ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
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
			BuildFile (Profile.macOSMobile, true, "tests/ref-out-parameters.cs");
		}

		[Test]
		public void ReturnRelease ()
		{
			BuildFile (Profile.iOS, "tests/return-release.cs");
		}

		[Test]
		public void GHIssue6626 () => BuildFile (Profile.iOS, "ghissue6626.cs");

		[Test]
		public void StrongDictsNativeEnums () => BuildFile (Profile.iOS, "strong-dict-native-enum.cs");

		[Test]
		public void IgnoreUnavailableProtocol ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/ignore-unavailable-protocol.cs");
			var myClass = bgen.ApiAssembly.MainModule.GetType ("NS", "MyClass");
			var myProtocol = bgen.ApiAssembly.MainModule.GetType ("NS", "IMyProtocol");
			var myClassInterfaces = myClass.Interfaces.Select (v => v.InterfaceType.Name).ToArray ();
			Assert.That (myClassInterfaces, Does.Not.Contain ("IMyProtocol"), "IMyProtocol");
			Assert.IsNull (myProtocol, "MyProtocol null");
		}

		[Test]
		public void VSTS970507 ()
		{
			BuildFile (Profile.iOS, "tests/vsts-970507.cs");
		}

		[Test]
		public void DiamondProtocol ()
		{
			BuildFile (Profile.iOS, "tests/diamond-protocol.cs");
		}

		[Test]
		public void GHIssue9065_Sealed () => BuildFile (Profile.iOS, nowarnings: true, "ghissue9065.cs");

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
			var bgen = BuildFile (profile, "tests/dispose-attribute.cs");

			// processing custom attributes (like its properties) will call Resolve so we must be able to find the platform assembly to run this test
			var resolver = bgen.ApiAssembly.MainModule.AssemblyResolver as BaseAssemblyResolver;
#if NET
			resolver.AddSearchDirectory (Configuration.GetRefDirectory (profile.AsPlatform ()));
#else
			resolver.AddSearchDirectory (Path.Combine (Configuration.SdkRootXI, "lib/mono/Xamarin.iOS/"));
#endif

			// [Dispose] is, by default, not optimizable
			var with_dispose = bgen.ApiAssembly.MainModule.GetType ("NS", "WithDispose").Methods.First ((v) => v.Name == "Dispose");
			Assert.NotNull (with_dispose, "WithDispose");
			Assert.That (IsOptimizable (with_dispose), Is.False, "WithDispose/Optimizable");

			// [Dispose] can opt-in being optimizable
			var with_dispose_optin = bgen.ApiAssembly.MainModule.GetType ("NS", "WithDisposeOptInOptimizable").Methods.First ((v) => v.Name == "Dispose");
			Assert.NotNull (with_dispose_optin, "WithDisposeOptInOptimizable");
			Assert.That (IsOptimizable (with_dispose_optin), Is.True, "WithDisposeOptInOptimizable/Optimizable");

			// Without a [Dispose] attribute the generated method is optimizable
			var without_dispose = bgen.ApiAssembly.MainModule.GetType ("NS", "WithoutDispose").Methods.First ((v) => v.Name == "Dispose");
			Assert.NotNull (without_dispose, "WitoutDispose");
			Assert.That (IsOptimizable (without_dispose), Is.True, "WitoutDispose/Optimizable");
		}

		[Test]
		public void SnippetAttributesOptimizable ()
		{
			var profile = Profile.iOS;
			var bgen = BuildFile (profile, "tests/snippet-attributes.cs");

			// processing custom attributes (like its properties) will call Resolve so we must be able to find the platform assembly to run this test
			var resolver = bgen.ApiAssembly.MainModule.AssemblyResolver as BaseAssemblyResolver;
#if NET
			resolver.AddSearchDirectory (Configuration.GetRefDirectory (profile.AsPlatform ()));
#else
			resolver.AddSearchDirectory (Path.Combine (Configuration.SdkRootXI, "lib/mono/Xamarin.iOS/"));
#endif

			// [SnippetAttribute] subclasses are, by default, not optimizable
			var not_opt = bgen.ApiAssembly.MainModule.GetType ("NS", "NotOptimizable");
			Assert.NotNull (not_opt, "NotOptimizable");
			var pre_not_opt = not_opt.Methods.First ((v) => v.Name == "Pre");
			Assert.That (IsOptimizable (pre_not_opt), Is.False, "NotOptimizable/Pre");
			var prologue_not_opt = not_opt.Methods.First ((v) => v.Name == "Prologue");
			Assert.That (IsOptimizable (prologue_not_opt), Is.False, "NotOptimizable/Prologue");
			var post_not_opt = not_opt.Methods.First ((v) => v.Name == "Post");
			Assert.That (IsOptimizable (post_not_opt), Is.False, "NotOptimizable/Post");

			// [SnippetAttribute] subclasses can opt-in being optimizable
			var optin_opt = bgen.ApiAssembly.MainModule.GetType ("NS", "OptInOptimizable");
			Assert.NotNull (optin_opt, "OptInOptimizable");
			var pre_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Pre");
			Assert.That (IsOptimizable (pre_optin_opt), Is.True, "OptInOptimizable/Pre");
			var prologue_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Prologue");
			Assert.That (IsOptimizable (prologue_optin_opt), Is.True, "OptInOptimizable/Prologue");
			var post_optin_opt = optin_opt.Methods.First ((v) => v.Name == "Post");
			Assert.That (IsOptimizable (post_optin_opt), Is.True, "OptInOptimizable/Post");

			// Without a [SnippetAttribute] subclass attribute the generated method is optimizable
			var nothing = bgen.ApiAssembly.MainModule.GetType ("NS", "NoSnippet").Methods.First ((v) => v.Name == "Nothing");
			Assert.NotNull (nothing, "NoSnippet");
			Assert.That (IsOptimizable (nothing), Is.True, "Nothing/Optimizable");
		}

		[Test]
		public void NativeEnum ()
		{
			var bgen = new BGenTool ();
			bgen.Profile = Profile.iOS;
			bgen.ProcessEnums = true;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.Sources = new string [] { Path.Combine (Configuration.SourceRoot, "tests", "generator", "tests", "nativeenum-extensions.cs") }.ToList ();
			bgen.ApiDefinitions = new string [] { Path.Combine (Configuration.SourceRoot, "tests", "generator", "tests", "nativeenum.cs") }.ToList ();
			bgen.CreateTemporaryBinding ();
			bgen.AssertExecute ("build");
		}

		[Test]
		public void DelegateWithINativeObjectReturnType ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/delegate-with-inativeobject-return-type.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("ObjCRuntime", "Trampolines").NestedTypes.First (v => v.Name == "DMyHandler");
			Assert.NotNull (type, "DMyHandler");
			var method = type.Methods.First (v => v.Name == "Invoke");
#if NET
			Assert.AreEqual ("ObjCRuntime.NativeHandle", method.ReturnType.FullName, "Return type");
#else
			Assert.AreEqual ("System.IntPtr", method.ReturnType.FullName, "Return type");
#endif
		}

		[Test]
		public void ProtocolBindProperty ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/protocol-bind-property.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyProtocol_Extensions");
			Assert.NotNull (type, "MyProtocol_Extensions");

			var method = type.Methods.First (v => v.Name == "GetOptionalProperty");
			var ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.AreEqual ("isOptionalProperty", (string) ldstr.Operand, "isOptionalProperty");


			method = type.Methods.First (v => v.Name == "SetOptionalProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.AreEqual ("setOptionalProperty:", (string) ldstr.Operand, "setOptionalProperty");

			type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyProtocolWrapper");
			Assert.NotNull (type, "MyProtocolWrapper");

			method = type.Methods.First (v => v.Name == "get_AbstractProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.AreEqual ("isAbstractProperty", (string) ldstr.Operand, "isAbstractProperty");

			method = type.Methods.First (v => v.Name == "set_AbstractProperty");
			ldstr = method.Body.Instructions.Single (v => v.OpCode == OpCodes.Ldstr);
			Assert.AreEqual ("setAbstractProperty:", (string) ldstr.Operand, "setAbstractProperty");
		}

		[Test]
		public void AbstractTypeTest ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/abstract-type.cs");
			bgen.AssertExecute ("build");

			// Assert that the return type from the delegate is IntPtr
			var type = bgen.ApiAssembly.MainModule.GetType ("NS", "MyObject");
			Assert.NotNull (type, "MyObject");
#if NET
			Assert.IsFalse (type.IsAbstract, "IsAbstract");
#else
			Assert.IsTrue (type.IsAbstract, "IsAbstract");
#endif

			var method = type.Methods.First (v => v.Name == ".ctor" && !v.HasParameters && !v.IsStatic);
#if NET
			Assert.IsTrue (method.IsFamily, "IsProtected ctor");
#else
			Assert.IsFalse (method.IsPublic, "IsPublic ctor");
#endif

			method = type.Methods.First (v => v.Name == "AbstractMember" && !v.HasParameters && !v.IsStatic);
			var throwInstruction = method.Body?.Instructions?.FirstOrDefault (v => v.OpCode == OpCodes.Throw);
			Assert.IsTrue (method.IsPublic, "IsPublic ctor");
			Assert.IsTrue (method.IsVirtual, "IsVirtual");
#if NET
			Assert.IsFalse (method.IsAbstract, "IsAbstract");
			Assert.IsNotNull (throwInstruction, "Throw");
#else
			Assert.IsTrue (method.IsAbstract, "IsAbstract");
			Assert.IsNull (throwInstruction, "Throw");
#endif
		}

		[Test]
#if !NET
		[Ignore ("This only applies to .NET")]
#endif
		public void NativeIntDelegates ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/nint-delegates.cs");

			Func<string, bool> verifyDelegate = (typename) => {
				// Assert that the return type from the delegate is IntPtr
				var type = bgen.ApiAssembly.MainModule.GetType ("NS", typename);
				Assert.NotNull (type, typename);
				var method = type.Methods.First (m => m.Name == "Invoke");
				Assert.IsNotNull (method.MethodReturnType.CustomAttributes.FirstOrDefault (attr => attr.AttributeType.Name == "NativeIntegerAttribute"), "Return type for delegate " + typename);
				foreach (var p in method.Parameters) {
					Assert.IsNotNull (p.CustomAttributes.FirstOrDefault (attr => attr.AttributeType.Name == "NativeIntegerAttribute"), $"Parameter {p.Name}'s type for delegate " + typename);
				}

				return false;
			};

			verifyDelegate ("D1");
			verifyDelegate ("D2");
			verifyDelegate ("D3");
			verifyDelegate ("NSTableViewColumnRowPredicate");
		}

		[Test]
#if !NET
		[Ignore ("This only applies to .NET")]
#endif
		public void CSharp10Syntax ()
		{
			BuildFile (Profile.iOS, "tests/csharp10syntax.cs");
		}

		[Test]
		public void NFloatType ()
		{
			var bgen = BuildFile (Profile.iOS, "tests/nfloat.cs");

			var messaging = bgen.ApiAssembly.MainModule.Types.FirstOrDefault (v => v.Name == "Messaging");
			Assert.IsNotNull (messaging, "Messaging");
			var pinvoke = messaging.Methods.FirstOrDefault (v => v.Name == "xamarin_nfloat_objc_msgSend_exception");
			Assert.IsNotNull (pinvoke, "PInvoke");
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
			return BuildFile (profile, nowarnings, processEnums, Enumerable.Empty<string> (), filenames);
		}

		BGenTool BuildFile (Profile profile, bool nowarnings, bool processEnums, IEnumerable<string> references, params string [] filenames)
		{
			var bgen = new BGenTool ();
			bgen.Profile = profile;
			bgen.ProcessEnums = processEnums;
			bgen.Defines = BGenTool.GetDefaultDefines (bgen.Profile);
			bgen.References = references.ToList ();
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
