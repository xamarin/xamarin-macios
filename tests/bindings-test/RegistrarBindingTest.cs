using System;
using System.Threading;

#if __UNIFIED__
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

using NUnit.Framework;

using Bindings.Test;

namespace Xamarin.BindingTests
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RegistrarBindingTest
	{
		[Test]
		public void BlockCallback ()
		{
			using (var obj = new BlockCallbackTester ()) {
				obj.CallClassCallback ();

				obj.TestObject = new BlockCallbackClass ();
				obj.CallOptionalCallback ();
				obj.CallRequiredCallback ();
				ObjCBlockTester.TestClass = new Class (typeof (BlockCallbackClass));
				ObjCBlockTester.CallRequiredStaticCallback ();
				ObjCBlockTester.CallOptionalStaticCallback ();

				obj.TestObject = new BlockCallbackClassExplicit ();
				obj.CallOptionalCallback ();
				obj.CallRequiredCallback ();
				ObjCBlockTester.TestClass = new Class (typeof (BlockCallbackClassExplicit));
				ObjCBlockTester.CallRequiredStaticCallback ();
				ObjCBlockTester.CallOptionalStaticCallback ();
			}
		}

		class BlockCallbackClass : NSObject, IObjCProtocolBlockTest
		{
			public void RequiredCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("optionalCallback:")]
			public void OptionalCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("requiredStaticCallback:")]
			public static void RequiredStaticCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("optionalStaticCallback:")]
			public static void OptionalStaticCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			public Action<int> RequiredReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("optionalReturnValue")]
			public Action<int> OptionalReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("requiredStaticReturnValue")]
			public static Action<int> RequiredStaticReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("optionalStaticReturnValue")]
			public static Action<int> OptionalStaticReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}
		}

		class BlockCallbackClassExplicit : NSObject, IObjCProtocolBlockTest
		{
			// Explicitly implemented interface member
			void IObjCProtocolBlockTest.RequiredCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("optionalCallback:")]
			public void OptionalCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("requiredStaticCallback:")]
			public static void RequiredStaticCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			[Export ("optionalStaticCallback:")]
			public static void OptionalRequiredCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}

			// Explicitly implemented interface member
			Action<int> IObjCProtocolBlockTest.RequiredReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("optionalReturnValue")]
			public Action<int> OptionalReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("requiredStaticReturnValue")]
			public static Action<int> RequiredStaticReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}

			[Export ("optionalStaticReturnValue")]
			public static Action<int> OptionalStaticReturnValue ()
			{
				return new Action<int> ((v) => {
					Assert.AreEqual (42, v, "RequiredReturnValue");
				});
			}
		}

		public class BlockCallbackTester : ObjCBlockTester
		{
			public override void ClassCallback (Action<int> completionHandler)
			{
				completionHandler (42);
			}
		}

		public class PropertyBlock : NSObject, IProtocolWithBlockProperties {
			[Export ("myOptionalProperty")]
			public SimpleCallback MyOptionalProperty { get; set; }

			public SimpleCallback MyRequiredProperty { get; set; }

			[Export ("myOptionalStaticProperty")]
			public static SimpleCallback MyOptionalStaticProperty { get; set; }

			[Export ("myRequiredStaticProperty")]
			public static SimpleCallback MyRequiredStaticProperty { get; set; }
		}

		[Test]
		[TestCase (true, true)]
		[TestCase (true, false)]
		[TestCase (false, true)]
		[TestCase (false, false)]
		public void ProtocolWithBlockProperties (bool required, bool instance)
		{
			using (var pb = new PropertyBlock ()) {
				var callbackCalled = false;
				SimpleCallback action = () => {
					callbackCalled = true;
				};
				if (required) {
					if (instance) {
						pb.MyRequiredProperty = action;
					} else {
						PropertyBlock.MyRequiredStaticProperty = action;
					}
				} else {
					if (instance) {
						pb.MyOptionalProperty = action;
					} else {
						PropertyBlock.MyOptionalStaticProperty = action;
					}
				}
				ObjCBlockTester.CallProtocolWithBlockProperties (pb, required, instance);
				Assert.IsTrue (callbackCalled, "Callback");
			}
		}

		[Test]
		[TestCase (true, true)]
		[TestCase (true, false)]
		[TestCase (false, true)]
		[TestCase (false, false)]
		public void ProtocolWithNativeBlockProperties (bool required, bool instance)
		{
			using (var pb = new PropertyBlock ()) {
				var calledCounter = ObjCBlockTester.CalledBlockCount;
				ObjCBlockTester.SetProtocolWithBlockProperties (pb, required, instance);
				if (required) {
					if (instance) {
						pb.MyRequiredProperty ();
					} else {
						PropertyBlock.MyRequiredStaticProperty ();
					}
				} else {
					if (instance) {
						pb.MyOptionalProperty ();
					} else {
						PropertyBlock.MyOptionalStaticProperty ();
					}
				}
				Assert.AreEqual (calledCounter + 1, ObjCBlockTester.CalledBlockCount, "Blocks called");
			}
		}
		[Test]
		[TestCase (true, true)]
		[TestCase (true, false)]
		[TestCase (false, true)]
		[TestCase (false, false)]
		public void ProtocolWithReturnValues (bool required, bool instance)
		{
			using (var pb = new BlockCallbackClass ()) {
				ObjCBlockTester.CallProtocolWithBlockReturnValue (pb, required, instance);
			}
		}
	}
}
