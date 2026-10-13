// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Network;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWContentContextTest {
		[Test]
		public void IterateProtocolMetadata ()
		{
			TestRuntime.AssertXcodeVersion (10, 0);
			using var context = new NWContentContext ("Metadata iteration");
			using var metadata = new NWIPMetadata ();
			using var definition = metadata.ProtocolDefinition;
			context.SetMetadata (metadata);

			// Repeat to verify that the callback wrappers do not consume the context's references.
			for (var iteration = 0; iteration < 2; iteration++) {
				var definitions = new List<IntPtr> ();
				var values = new List<IntPtr> ();
				context.IterateProtocolMetadata ((protocol, value) => {
					definitions.Add (protocol.GetHandle ());
					values.Add (value.GetHandle ());
				});
				Assert.That (definitions, Is.EqualTo (new [] { (IntPtr) definition.Handle }), "Definitions");
				Assert.That (values, Is.EqualTo (new [] { (IntPtr) metadata.Handle }), "Metadata");
			}
		}
	}
}
