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

using System;
using System.Threading;

#if XAMCORE_2_0
using Foundation;
#else
using MonoTouch;
using MonoTouch.Foundation;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class WeakReferenceTest {

		[Test]
		public void WeakTest ()
		{
			//Finalizable.debug = true;
			var t = new Test ();
			var thread = new Thread (delegate () {
				t.Obj = new Finalizable ();
				t.Obj2 = new Finalizable ();
				t.Obj3 = new Finalizable ();
				t.Obj4 = Test.retain = new Finalizable ();
				Test.retain.a = 0x1029458;
			});
			thread.Start ();
			thread.Join ();
			GC.Collect (0);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			GC.WaitForPendingFinalizers ();
			Assert.That (t.Obj, Is.Null, "'t.Obj' should be null");
			Assert.That (t.Obj2, Is.Null, "'t.Obj2' should be null");
			Assert.That (t.Obj3, Is.Not.Null, "'t.Obj3' should not be null");

			//overflow the nursery, make sure we fill it
			for (int i = 0; i < 1000 * 1000 * 10; ++i)
				new OneField ();

			Exception ex = null;
			thread = new Thread (() => {
				try {
					// This must be done on a separate thread so that the 'Test.retain' value doesn't
					// show up on the main thread's stack as a temporary value in registers the
					// GC can see.
					Assert.That (Test.retain.a, Is.EqualTo (0x1029458), "retain.a");
				} catch (Exception e) {
					ex = e;
				}
			});
			thread.Start ();
			thread.Join ();

			Test.retain = null;
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
