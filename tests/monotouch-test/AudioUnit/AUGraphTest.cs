//
// Unit tests for AUGraph
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__
using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

using Foundation;
using AudioUnit;
using ObjCRuntime;

namespace MonoTouchFixtures.AudioUnit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AUGraphTest {
		[Test]
		public void BasicOperations ()
		{
			using (var aug = new AUGraph ()) {
				aug.Open ();
				Assert.IsTrue (aug.IsOpen, "#0");
				Assert.IsFalse (aug.IsInitialized, "#0a");
				Assert.IsFalse (aug.IsRunning, "#0b");

				var node = aug.AddNode (AudioComponentDescription.CreateOutput (AudioTypeOutput.Generic));
				int count;
				Assert.AreEqual (AUGraphError.OK, aug.GetNodeCount (out count), "#1");
				Assert.AreEqual (1, count, "#2");

				var info = aug.GetNodeInfo (node);
				Assert.IsNotNull (info, "#3");

				int node2;
				Assert.AreEqual (AUGraphError.OK, aug.GetNode (0, out node2), "#4");
				Assert.AreEqual (1, node2, "#4a");

				float max_load;
				Assert.AreEqual (AUGraphError.OK, aug.GetMaxCPULoad (out max_load));
			}
		}

		[Test]
		public void Connections ()
		{
			using (var aug = new AUGraph ()) {
				aug.Open ();

				var node_1 = aug.AddNode (AudioComponentDescription.CreateGenerator (AudioTypeGenerator.AudioFilePlayer));
				var node_2 = aug.AddNode (AudioComponentDescription.CreateOutput (AudioTypeOutput.Generic));

				Assert.AreEqual (AUGraphError.OK, aug.ConnnectNodeInput (node_1, 0, node_2, 0), "#1");
				uint count;
				aug.GetNumberOfInteractions (out count);
				Assert.AreEqual (1, count, "#2");

				Assert.AreEqual (AUGraphError.OK, aug.Initialize (), "#3");

				Assert.AreEqual (AUGraphError.OK, aug.ClearConnections (), "#4");
				aug.GetNumberOfInteractions (out count);
				Assert.AreEqual (0, count, "#5");
			}
		}

		[Test]
		public void CreateTest ()
		{
			int errCode;
			using (var aug = AUGraph.Create (out errCode)) {
				Assert.NotNull (aug, "CreateTest");
				Assert.AreEqual (0, errCode, "CreateTest");

				// Make sure it is a working instance
				aug.Open ();
				Assert.IsTrue (aug.IsOpen, "CreateTest #0");
				Assert.IsFalse (aug.IsInitialized, "CreateTest #0a");
				Assert.IsFalse (aug.IsRunning, "CreateTest #0b");
			}
		}

		[DllImport (Constants.AudioToolboxLibrary, EntryPoint = "NewAUGraph")]
		static extern int NewAUGraph (ref IntPtr outGraph);

		[Test]
		public void GetNativeTest ()
		{
			IntPtr ret = IntPtr.Zero;
			var errCode = NewAUGraph (ref ret);
			Assert.AreEqual (0, errCode, "GetNativeTest");
			Assert.That (ret, Is.Not.EqualTo (IntPtr.Zero), "ret");

			using (var aug = Runtime.GetINativeObject<AUGraph> (ret, true)) {
				Assert.NotNull (aug, "CreateTest");
				Assert.That ((IntPtr) aug.Handle, Is.EqualTo (ret), "Handle");

				// Make sure it is a working instance
				aug.Open ();
				Assert.IsTrue (aug.IsOpen, "CreateTest #0");
				Assert.IsFalse (aug.IsInitialized, "CreateTest #0a");
				Assert.IsFalse (aug.IsRunning, "CreateTest #0b");
			}
		}
	}
}

#endif // !__WATCHOS__
