//
// Unit tests for GKEntity
//
// Authors:
//	Alex Soto <alex.soto@xamarin.com>
//	
//
// Copyright 2015 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using System;
using OpenTK;

#if XAMCORE_2_0
using Foundation;
using GameplayKit;
#else
using MonoTouch.Foundation;
using MonoTouch.GameplayKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.GamePlayKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class GKEntityTests {

		[Test]
		public void GetAndRemoveTest ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var entity = GKEntity.GetEntity ();
			entity.AddComponent (new NumberComponent (10));
			entity.AddComponent (new NameComponent ("Ten"));
			Assert.IsTrue (entity.Components.Length == 2, "entity.Components length must be 2");

			// Test component retrieval by type
			var component = entity.GetComponent (typeof(NumberComponent)) as NumberComponent;
			Assert.NotNull (component, "Component must not be null");
			Assert.IsTrue (component.Id == 10, "Component Id must be 10");

			// Test component removal by type
			Assert.NotNull (entity.GetComponent (typeof (NameComponent)), "Component typeof NameComponent must not be null");
			entity.RemoveComponent (typeof(NameComponent));
			Assert.IsTrue (entity.Components.Length == 1, "entity.Components length must be 1");
			Assert.IsNull (entity.GetComponent (typeof (NameComponent)), "Component typeof NameComponent must be null");
		}

		[ExpectedException (typeof (ArgumentNullException))]
		[Test]
		public void BadGetComponent ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var entity = GKEntity.GetEntity ();
			entity.GetComponent (null);
		}

		[ExpectedException (typeof (ArgumentNullException))]
		[Test]
		public void BadRemoval ()
		{
			TestRuntime.AssertXcodeVersion (7, 0);

			var entity = GKEntity.GetEntity ();
			entity.RemoveComponent (null);
		}
	}

	[Preserve (AllMembers = true)]
	class NameComponent : GKComponent { 

		public string Name { get; private set; }

		public NameComponent (string name)
		{
			Name = name;
		}

		public NameComponent (IntPtr handle) : base (handle) { }
	}

	[Preserve (AllMembers = true)]
	class NumberComponent : GKComponent { 

		public int Id { get; private set; }

		public NumberComponent (int id)
		{
			Id = id;;
		}

		public NumberComponent (IntPtr handle) : base (handle) { }
	}
}

#endif // __WATCHOS__
