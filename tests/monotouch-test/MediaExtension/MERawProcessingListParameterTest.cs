#if HAS_MEDIAEXTENSION && NET
using Foundation;
using MediaExtension;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaExtension {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MERawProcessingListParameterTest {
		[Test]
		public void CtorTest_Neutral ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var array = new MERawProcessingListElementParameter []
			{
				new MERawProcessingListElementParameter ("name0", "desc0", 1),
				new MERawProcessingListElementParameter ("name1", "desc1", 3),
				new MERawProcessingListElementParameter ("name2", "desc2", 5),
			};
			using var obj = new MERawProcessingListParameter ("name", "key", "description", array, 1, 3, MERawProcessingListParameterInitializationOption.NeutralValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual ((nint) 1, obj.InitialValue, "InitialValue");
				Assert.AreEqual ((nint) 1, obj.CurrentValue, "CurrentValue");
				Assert.IsTrue (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual ((nint) 3, neutralValue, "NeutralValue");
				Assert.IsFalse (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual ((nint) 0, cameraValue, "NeutralValue");
			});
		}

		[Test]
		public void CtorTest_Camera ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			var array = new MERawProcessingListElementParameter []
			{
				new MERawProcessingListElementParameter ("name0", "desc0", 1),
				new MERawProcessingListElementParameter ("name1", "desc1", 3),
				new MERawProcessingListElementParameter ("name2", "desc2", 5),
			};
			using var obj = new MERawProcessingListParameter ("name", "key", "description", array, 1, 3, MERawProcessingListParameterInitializationOption.CameraValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual ((nint) 1, obj.InitialValue, "InitialValue");
				Assert.AreEqual ((nint) 1, obj.CurrentValue, "CurrentValue");
				Assert.IsFalse (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual ((nint) 0, neutralValue, "NeutralValue");
				Assert.IsTrue (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual ((nint) 3, cameraValue, "NeutralValue");
			});
		}
	}
}
#endif // HAS_MEDIAEXTENSION
