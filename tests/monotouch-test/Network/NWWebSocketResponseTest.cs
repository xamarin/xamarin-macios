// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Network;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWWebSocketResponseTest {
		[Test]
		public void EnumerateAdditionalHeaders ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			using var response = new NWWebSocketResponse (NWWebSocketResponseStatus.Accept, "test");
			response.SetHeader ("X-First", "first");
			response.SetHeader ("X-Second", "second");
			var headers = new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);
			var completed = response.EnumerateAdditionalHeaders ((name, value) => {
				headers.Add (name, value);
			});
			Assert.That (completed, Is.True, "Completed");
			Assert.That (headers.Count, Is.EqualTo (2), "Header count");
			Assert.That (headers ["X-First"], Is.EqualTo ("first"), "First header");
			Assert.That (headers ["X-Second"], Is.EqualTo ("second"), "Second header");
		}
	}
}
