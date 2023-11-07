//
// Port of mono's unit tests for WeakAttribute from:
// https://github.com/mono/mono/blob/5bdaef7e5f6479cc4336bb809b419e85ad706dd7/mono/tests/weak-fields.cs
//
// Authors:
//	Zoltan Varga
//	Alex Soto <alexsoto@microsoft.com>
//
//
// Copyright 2018 Xamarin Inc. All rights reserved.
//

#if !NET // WeakAttribute is not supported in .NET
using System;
using System.Threading;

using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class WeakReferenceTest {
		public static class FinalizerHelpers {
			private static IntPtr aptr;

			private static unsafe void NoPinActionHelper (int depth, Action act)
			{
				// Avoid tail calls
				int* values = stackalloc int [20];
				aptr = new IntPtr (values);

				if (depth <= 0) {
					//
					// When the action is called, this new thread might have not allocated
					// anything yet in the nursery. This means that the address of the first
					// object that would be allocated would be at the start of the tlab and
					// implicitly the end of the previous tlab (address which can be in use
					// when allocating on another thread, at checking if an object fits in
					// this other tlab). We allocate a new dummy object to avoid this type
					// of false pinning for most common cases.
					//
					new object ();
					act ();
				} else {
					NoPinActionHelper (depth - 1, act);
				}
			}

			public static void PerformNoPinAction (Action act)
			{
				Thread thr = new Thread (() => NoPinActionHelper (128, act));
				thr.Start ();
				thr.Join ();
			}
		}

		[Test]
		public void WeakTest ()
		{
			//Finalizable.debug = true;
			var t = new Test ();

			FinalizerHelpers.PerformNoPinAction (delegate ()
			{
				FinalizerHelpers.PerformNoPinAction (delegate ()
				{
					t.Obj = new Finalizable ();
					t.Obj2 = new Finalizable ();
					t.Obj3 = new Finalizable ();
					t.Obj4 = Test.retain = new Finalizable ();
					Test.retain.a = 0x1029458;
				});
				GC.Collect (0);
				GC.Collect ();
				GC.WaitForPendingFinalizers ();
				GC.WaitForPendingFinalizers ();
				Assert.That (t.Obj, Is.Null, "'t.Obj' should be null");
				Assert.That (t.Obj2, Is.Null, "'t.Obj2' should be null");
				Assert.That (t.Obj3, Is.Not.Null, "'t.Obj3' should not be null");

				//overflow the nursery, make sure we fill it
#if __WATCHOS__
				for (int i = 0; i < 1000 * 100; ++i) // the apple watch doesn't have much memory, so try to not run into OOMs either. The nursery is 512k, so 100k objects should be more than enough to fill it.
#else
				for (int i = 0; i < 1000 * 1000 * 10; ++i)
#endif
					new OneField ();

				Exception ex = null;
				FinalizerHelpers.PerformNoPinAction (delegate ()
				{
					try {
						// This must be done on a separate thread so that the 'Test.retain' value doesn't
						// show up on the main thread's stack as a temporary value in registers the
						// GC can see.
						Assert.That (Test.retain.a, Is.EqualTo (0x1029458), "retain.a");
					} catch (Exception e) {
						ex = e;
					}
				});

				Test.retain = null;
			});

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.WaitForPendingFinalizers ();

			Assert.That (t.Obj4, Is.Null, "'t.Obj4' should be null");
		}
	}

	public class Test {
		public static Finalizable retain;

		[Weak]
		public object Obj;
		[Weak2]
		public object Obj3;
		[Weak]
		public object Obj2;
		[Weak]
		public Finalizable Obj4;
	}

	[AttributeUsage (AttributeTargets.Field)]
	public sealed class Weak2Attribute : Attribute {
	}

	public class Finalizable {
		public int a;
		public static bool debug;

		~Finalizable ()
		{
			if (debug)
				Console.WriteLine ("Finalized. {0}", a);
		}
	}

	public class OneField {
		int x;
	}
}
#endif // !NET
