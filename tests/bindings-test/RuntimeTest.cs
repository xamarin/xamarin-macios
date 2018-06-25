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

namespace Xamarin.Tests
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RuntimeTest
	{
		[Test]
		public void WrapperTypeLookupTest ()
		{
			using (var assigner = new MyProtocolAssigner ()) {
				assigner.SetProtocol ();
			}
		}

		class MyProtocolAssigner : ProtocolAssigner {
			public bool Called;
			public override void CompletedSetProtocol (IProtocolAssignerProtocol value)
			{
				Called = true;
			}
		}

		public void EvilDeallocatorTest ()
		{
			// Create a few toggle-ref objects
			for (var i = 0; i < 10; i++) {
				var ed = new EvilDeallocator ();
				ed.MarkMeDirty ();
			}
			// Now create an object that will call a managed callback in its destructor
			using (var evil = new EvilDeallocator ()) {
				evil.EvilCallback += (int obj) => {
					// Running the GC will cause the GC to check the toggle-ref status
					// of the objects we created above.
					var t = new Thread (() => {
						GC.Collect ();
					});
					t.Start ();
					t.Join (); // If the test fails, this will deadlock.
				};
			}
		}

		[Test]
		public void MainThreadDeallocationTest ()
		{
			ObjCBlockTester.CallAssertMainThreadBlockRelease ((callback) => {
				callback (42);
			});
		}
	}
}
