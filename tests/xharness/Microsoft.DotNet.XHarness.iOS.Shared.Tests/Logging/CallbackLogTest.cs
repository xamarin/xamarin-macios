using System;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Logging {

	[TestFixture]
	public class CallbackLogTest {

		[Test]
		public void OnWriteTest ()
		{
			var message = "This is a log message";
			bool called = false;
			string data = null;

			Action<string> cb = (d) => {
				called = true;
				data = d;
			};

			var log = new CallbackLog (cb);
			log.Write (message);
			Assert.IsTrue (called, "Callback was not called");
			Assert.IsNotNull (data, "data");
			StringAssert.EndsWith (message, data, "message"); // TODO: take time stamp into account
		}
	}
}
