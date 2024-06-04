using System;
using System.Linq;

using NUnit.Framework;
using Xamarin.Tests;

namespace GeneratorTests {
	[TestFixture ()]
	[Parallelizable (ParallelScope.All)]
	public class ProtocolTests : BGenBase {
#if !NET
		[Ignore ("This only applies to .NET")]
#endif
		[TestCase (Profile.MacCatalyst)]
		[TestCase (Profile.iOS)]
		public void Members (Profile profile)
		{
			var bgen = BuildFile (profile, "tests/protocols.cs");

			var allTypeDefinitions = bgen.ApiAssembly.MainModule.GetTypes ().ToArray ();
			var allTypes = allTypeDefinitions.Select (v => v.FullName).OrderBy (v => v).ToArray ();
			var expectedTypes = new string [] {
				"<Module>",
				"api0.Messaging",
				"Protocols.IProtocolWithConstructors",
				"Protocols.IProtocolWithStaticMembers",
				"Protocols.ProtocolWithConstructorsWrapper",
				"Protocols.ProtocolWithStaticMembersWrapper",
			};
			var missingTypes = expectedTypes.Except (allTypes);
			var extraTypes = allTypes.Except (expectedTypes);

			if (missingTypes.Any () || extraTypes.Any ()) {
				Console.WriteLine ("Expected Types (formatted to be copy-pasted into ProtocolTests.cs):");
				foreach (var t in allTypes)
					Console.WriteLine ($"				\"{t}\",");
			}

			Assert.That (missingTypes, Is.Empty, "Missing Types");
			Assert.That (extraTypes, Is.Empty, "Extra Types");

			var allMethods = allTypeDefinitions.SelectMany (v => v.Methods).Select (v => v.ToString ()).OrderBy (v => v).ToArray ();
			var expectedMethods = new string [] {
				"Foundation.NSDate Protocols.IProtocolWithStaticMembers::GetDateProperty()",
				"Foundation.NSString Protocols.IProtocolWithStaticMembers::Method(Foundation.NSDate)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend_ref_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle*)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend(System.IntPtr,System.IntPtr)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper_ref_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle*)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
				"ObjCRuntime.NativeHandle Protocols.IProtocolWithStaticMembers::Method()",
				"System.Boolean Protocols.IProtocolWithStaticMembers::GetProperty()",
				"System.Byte api0.Messaging::bool_objc_msgSend(System.IntPtr,System.IntPtr)",
				"System.Byte api0.Messaging::bool_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
				"System.Int32 api0.Messaging::int_objc_msgSend_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"System.Int32 api0.Messaging::int_objc_msgSendSuper_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"System.Int32 Protocols.IProtocolWithStaticMembers::Method(System.String)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSend_IntPtr(System.IntPtr,System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSend(System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSendSuper_IntPtr(System.IntPtr,System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
				"System.String Protocols.IProtocolWithStaticMembers::GetStringProperty()",
				"System.String Protocols.IProtocolWithStaticMembers::Method(Foundation.NSError&)",
				"System.Void api0.Messaging::.cctor()",
				"System.Void api0.Messaging::void_objc_msgSend_bool(System.IntPtr,System.IntPtr,System.Byte)",
				"System.Void api0.Messaging::void_objc_msgSend_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"System.Void api0.Messaging::void_objc_msgSendSuper_bool(System.IntPtr,System.IntPtr,System.Byte)",
				"System.Void api0.Messaging::void_objc_msgSendSuper_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"System.Void Protocols.IProtocolWithStaticMembers::SetDateProperty(Foundation.NSDate)",
				"System.Void Protocols.IProtocolWithStaticMembers::SetProperty(System.Boolean)",
				"System.Void Protocols.IProtocolWithStaticMembers::SetStringProperty(System.String)",
				"System.Void Protocols.ProtocolWithConstructorsWrapper::.ctor(ObjCRuntime.NativeHandle,System.Boolean)",
				"System.Void Protocols.ProtocolWithStaticMembersWrapper::.ctor(ObjCRuntime.NativeHandle,System.Boolean)",
				"T Protocols.IProtocolWithConstructors::Create(Foundation.NSDate)",
				"T Protocols.IProtocolWithConstructors::CreateInstance()",
				"T Protocols.IProtocolWithConstructors::CreateInstance(Foundation.NSError&)",
				"T Protocols.IProtocolWithConstructors::CreateInstance(System.String)",
			};

			var missingMethods = expectedMethods.Except (allMethods);
			var extraMethods = allMethods.Except (expectedMethods);

			if (missingMethods.Any () || extraMethods.Any ()) {
				Console.WriteLine ("Expected Methods (formatted to be copy-pasted into ProtocolTests.cs):");
				foreach (var t in allMethods)
					Console.WriteLine ($"				\"{t}\",");
			}

			Assert.That (missingMethods, Is.Empty, "Missing Methods");
			Assert.That (extraMethods, Is.Empty, "Extra Methods");
		}
	}
}

