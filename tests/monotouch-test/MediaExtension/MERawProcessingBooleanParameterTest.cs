#if HAS_MEDIAEXTENSION && NET
using Foundation;
using MediaExtension;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaExtension {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MERawProcessingBooleanParameterTest {
		[Test]
		public void CtorTest_Neutral ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingBooleanParameter ("name", "key", "description", false, true, MERawProcessingBooleanParameterInitializationOption.NeutralValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.IsFalse (obj.InitialValue, "InitialValue");
				Assert.IsFalse (obj.CurrentValue, "CurrentValue");
				Assert.IsTrue (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.IsTrue (neutralValue, "NeutralValue");
				Assert.IsFalse (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.IsFalse (cameraValue, "NeutralValue");
			});
		}

		[Test]
		public void CtorTest_Camera ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingBooleanParameter ("name", "key", "description", false, true, MERawProcessingBooleanParameterInitializationOption.CameraValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.IsFalse (obj.InitialValue, "InitialValue");
				Assert.IsFalse (obj.CurrentValue, "CurrentValue");
				Assert.IsFalse (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.IsFalse (neutralValue, "NeutralValue");
				Assert.IsTrue (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.IsTrue (cameraValue, "NeutralValue");
			});
		}
	}
}
#endif // HAS_MEDIAEXTENSION
