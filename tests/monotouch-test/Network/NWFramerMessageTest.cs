#if !__WATCHOS__
using System;
using System.Text;

using Foundation;
using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWFramerMessageTest {
		NWFramerMessage message;
		NWFramer framer;

		string identifier = "TestFramer";

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		NWFramerStartResult StartCallback (NWFramer nWFramer)
		{
			framer = nWFramer;
			return NWFramerStartResult.Ready;
		}

		[SetUp]
		public void SetUp ()
		{
			using (var definition = NWProtocolDefinition.CreateFramerDefinition (identifier, NWFramerCreateFlags.Default, StartCallback)) {
				message = NWFramerMessage.Create (definition);
			}
		}

		[TearDown]
		public void TearDown ()
		{
			message.Dispose ();
		}

		[Test]
		public void TestGetObject ()
		{
			// store an NSObject
			var storedValue = new NSNumber (30);
			message.SetObject ("test", storedValue);

			var result = message.GetObject<NSNumber> ("test");
			Assert.IsNotNull (result, "Null");
			Assert.AreEqual (storedValue, result, "Equal");
		}

		[Test]
		public void TestGetObjectMissingKey ()
		{
			var result = message.GetObject<NSNumber> ("test");
			Assert.IsNull (result, "Null");
		}

		[Test]
		public void TestGetData ()
		{
			var dataString = "My super string.";
			var data = Encoding.UTF8.GetBytes (dataString);
			message.SetData ("test", data);

			ReadOnlySpan<byte> outData;
			var found = message.GetData ("test", data.Length, out outData);

			Assert.IsTrue (found, "Found");
			Assert.AreEqual (data.Length, outData.Length, "Legth");
			Assert.AreEqual (dataString, Encoding.UTF8.GetString (outData), "Equal");
		}

		[Test]
		public void TestGetDataMissingKey ()
		{
			ReadOnlySpan<byte> outData;
			var found = message.GetData ("test", 23, out outData);
			Assert.IsFalse (found, "Found");
			Assert.AreEqual (0, outData.Length, "Length");
		}
	}
}
#endif
