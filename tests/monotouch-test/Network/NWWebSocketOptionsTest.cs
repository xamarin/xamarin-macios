
using Network;

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
					Assert.That (otherOptions.Handle, Is.Not.EqualTo (IntPtr.Zero));
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
			Assert.That (defaultValue, Is.False, "defaultValue");
			options.AutoReplyPing = true;
			Assert.That (options.AutoReplyPing, Is.True, "new value");
		}

		[Test]
		public void TestMaxMessageSize ()
		{
			var defaultValue = options.MaximumMessageSize;
			Assert.That (defaultValue, Is.EqualTo ((nuint) 0), "defaultValue");
			nuint newValue = 40;
			options.MaximumMessageSize = newValue;
			Assert.That (options.MaximumMessageSize, Is.EqualTo (newValue), "new value");
		}

		[Test]
		public void TestSkipHandShake ()
		{
			Assert.That (options.SkipHandShake, Is.False, "defaultValue");
			options.SkipHandShake = true;
			Assert.That (options.SkipHandShake, Is.True, "new value");
		}

		[Test]
		public void TestClientRequenHandlerNullQ () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (null, (r) => { }));

		[Test]
		public void TestClientRequestHandlerNullCallback () => Assert.Throws<ArgumentNullException> (() => options.SetClientRequestHandler (DispatchQueue.CurrentQueue, null));


	}
}
