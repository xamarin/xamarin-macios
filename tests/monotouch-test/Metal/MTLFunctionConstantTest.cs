#if !__WATCHOS__

using System;

using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLFunctionConstantTest {

		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
		}

		[Test]
		public void GetNameTest ()
		{
			var constant = new MTLFunctionConstant ();
			Assert.IsNull (constant.Name); // defualt value is null
		}

		[Test]
		public void GetTypeTest ()
		{
			var constant = new MTLFunctionConstant ();
			Assert.AreEqual (MTLDataType.None, constant.Type); // default value is none
		}

		[Test]
		public void GetIndexTest ()
		{
			var constant = new MTLFunctionConstant ();
			Assert.AreEqual ((nuint) 0, constant.Index, $"Index is {constant.Index}"); // default value is 0
		}

		[Test]
		public void GetIsRequiredTest ()
		{
			var constant = new MTLFunctionConstant ();
			Assert.False (constant.IsRequired); // defualt value is false
		}
	}
}

#endif // !__WATCHOS__
