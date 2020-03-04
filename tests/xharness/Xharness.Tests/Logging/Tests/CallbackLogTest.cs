using System;
using NUnit.Framework;
using Xharness.Logging;

namespace Xharness.Tests.Logging.Tests {

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
