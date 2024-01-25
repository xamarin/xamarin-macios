#if !__WATCHOS__
using System;

using CoreFoundation;

using Foundation;

using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWWebSocketOptionsTest {

		NWWebSocketOptions options;

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);

		[SetUp]
		public void SetUp ()
		{
			options = new NWWebSocketOptions (NWWebSocketVersion.Version13);
		}

		[TearDown]
		public void TearDown ()
		{
			options.Dispose ();
		}

		[Test]
		public void TestConstructorInvalidVersion ()
		{
			Assert.DoesNotThrow (() => {
				using (var otherOptions = new NWWebSocketOptions (NWWebSocketVersion.Invalid))
					Assert.AreNotEqual (IntPtr.Zero, otherOptions.Handle);
			});
		}

		[Test]
		public void TestSetHeader () => Assert.DoesNotThrow (() => options.SetHeader ("CustomHeader", "hola"));

		[Test]
		public void TestSetHeaderNullName () => Assert.Throws<ArgumentNullException> (() => options.SetHeader (null, "hola"));


		[Test]
		public void TestSetHeaderNullValue () => Assert.DoesNotThrow (() => options.SetHeader ("CustomHeader", null));

		[Test]
		public void TestAddSubprotocol () => Assert.DoesNotThrow (() => options.AddSubprotocol ("Protobuf"));

		[Test]
		public void TestAddSubprotocolNullValue () => Assert.Throws<ArgumentNullException> (() => options.AddSubprotocol (null));

		[Test]
		public void TestAutoReplyPing ()
		{
			var defaultValue = options.AutoReplyPing;
			Assert.IsFalse (defaultValue, "defaultValue");
			options.AutoReplyPing = true;
			Assert.IsTrue (options.AutoReplyPing, "new value");
		}

		[Test]
		public void TestMaxMessageSize ()
		{
			var defaultValue = options.MaximumMessageSize;
			Assert.AreEqual (defaultValue, (nuint) 0, "defaultValue");
			nuint newValue = 40;
			options.MaximumMessageSize = newValue;
			Assert.AreEqual (newValue, options.MaximumMessageSize, "new value");
		}

		[Test]
		public void TestSkipHandShake ()
		{
			Assert.IsFalse (options.SkipHandShake, "defaultValue");
			options.SkipHandShake = true;
			Assert.IsTrue (options.SkipHandShake, "new value");
		}

		[Test]
		public void TestClientRequenHandlerNullQ () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (null, (r) => { }));

		[Test]
		public void TestClientRequestHandlerNullCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (DispatchQueue.CurrentQueue, null));


	}
}
#endif
