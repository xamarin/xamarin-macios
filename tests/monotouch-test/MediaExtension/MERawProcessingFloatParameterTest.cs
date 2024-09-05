#if HAS_MEDIAEXTENSION && NET
using Foundation;
using MediaExtension;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaExtension {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MERawProcessingFloatParameterTest {
		[Test]
		public void CtorTest_Neutral ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingFloatParameter ("name", "key", "description", 1.2f, 3.4f, 0.1f, 1.1f, MERawProcessingFloatParameterInitializationOption.NeutralValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual (1.2f, obj.InitialValue, "InitialValue");
				Assert.AreEqual (1.2f, obj.CurrentValue, "CurrentValue");
				Assert.AreEqual (3.4f, obj.MaximumValue, "MaximumValue");
				Assert.AreEqual (0.1f, obj.MinimumValue, "MinimumValue");
				Assert.IsTrue (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual (1.1f, neutralValue, "NeutralValue");
				Assert.IsFalse (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual (0f, cameraValue, "NeutralValue");
			});
		}

		[Test]
		public void CtorTest_Camera ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingFloatParameter ("name", "key", "description", 1.2f, 3.4f, 0.1f, 1.1f, MERawProcessingFloatParameterInitializationOption.CameraValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual (1.2f, obj.InitialValue, "InitialValue");
				Assert.AreEqual (1.2f, obj.CurrentValue, "CurrentValue");
				Assert.AreEqual (3.4f, obj.MaximumValue, "MaximumValue");
				Assert.AreEqual (0.1f, obj.MinimumValue, "MinimumValue");
				Assert.IsFalse (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual (0f, neutralValue, "NeutralValue");
				Assert.IsTrue (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual (1.1f, cameraValue, "NeutralValue");
			});
		}
	}
}
#endif // HAS_MEDIAEXTENSION
