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
			var allTypes = allTypeDefinitions.Select (v => v.FullName).ToArray ();

			var allTypeNames = allTypes.OrderBy (v => v).ToArray ();
			var expectedTypes = new string [] {
				"<Module>",
				"api0.Messaging",
				"Protocols.IProtocolWithConstructors",
				"Protocols.ProtocolWithConstructorsWrapper",
			};
			CollectionAssert.AreEqual (expectedTypes, allTypeNames, "Types");

			var allMethods = allTypeDefinitions.SelectMany (v => v.Methods).Select (v => v.ToString ()).OrderBy (v => v).ToArray ();
			var expectedMethods = new string [] {
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend_ref_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle*)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSend(System.IntPtr,System.IntPtr)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper_ref_NativeHandle(System.IntPtr,System.IntPtr,ObjCRuntime.NativeHandle*)",
				"ObjCRuntime.NativeHandle api0.Messaging::NativeHandle_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSend_IntPtr(System.IntPtr,System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSend(System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSendSuper_IntPtr(System.IntPtr,System.IntPtr,System.IntPtr)",
				"System.IntPtr api0.Messaging::IntPtr_objc_msgSendSuper(System.IntPtr,System.IntPtr)",
				"System.Void api0.Messaging::.cctor()",
				"System.Void Protocols.ProtocolWithConstructorsWrapper::.ctor(ObjCRuntime.NativeHandle,System.Boolean)",
				"T Protocols.IProtocolWithConstructors::Create(Foundation.NSDate)",
				"T Protocols.IProtocolWithConstructors::CreateInstance()",
				"T Protocols.IProtocolWithConstructors::CreateInstance(Foundation.NSError&)",
				"T Protocols.IProtocolWithConstructors::CreateInstance(System.String)",
			};

			CollectionAssert.AreEqual (expectedMethods, allMethods, "Types");
		}
	}
}

