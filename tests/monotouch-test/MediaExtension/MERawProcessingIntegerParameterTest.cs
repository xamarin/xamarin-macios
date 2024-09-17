#if HAS_MEDIAEXTENSION && NET
using Foundation;
using MediaExtension;

using NUnit.Framework;

namespace MonoTouchFixtures.MediaExtension {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MERawProcessingIntegerParameterTest {
		[Test]
		public void CtorTest_Neutral ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingIntegerParameter ("name", "key", "description", 3, 5, 1, 2, MERawProcessingIntegerParameterInitializationOption.NeutralValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual ((nint) 3, obj.InitialValue, "InitialValue");
				Assert.AreEqual ((nint) 3, obj.CurrentValue, "CurrentValue");
				Assert.AreEqual ((nint) 5, obj.MaximumValue, "MaximumValue");
				Assert.AreEqual ((nint) 1, obj.MinimumValue, "MinimumValue");
				Assert.IsTrue (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual ((nint) 2, neutralValue, "NeutralValue");
				Assert.IsFalse (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual ((nint) 0, cameraValue, "NeutralValue");
			});
		}

		[Test]
		public void CtorTest_Camera ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);

			using var obj = new MERawProcessingIntegerParameter ("name", "key", "description", 3, 5, 1, 2, MERawProcessingIntegerParameterInitializationOption.CameraValue);
			Assert.Multiple (() => {
				Assert.AreEqual ("name", obj.Name, "Name");
				Assert.AreEqual ("key", obj.Key, "Key");
				Assert.IsNull (obj.LongDescription, "LongDescription");
				Assert.AreEqual ((nint) 3, obj.InitialValue, "InitialValue");
				Assert.AreEqual ((nint) 3, obj.CurrentValue, "CurrentValue");
				Assert.AreEqual ((nint) 5, obj.MaximumValue, "MaximumValue");
				Assert.AreEqual ((nint) 1, obj.MinimumValue, "MinimumValue");
				Assert.IsFalse (obj.HasNeutralValue (out var neutralValue), "HasNeutralValue");
				Assert.AreEqual ((nint) 0, neutralValue, "NeutralValue");
				Assert.IsTrue (obj.HasCameraValue (out var cameraValue), "HasCameraValue");
				Assert.AreEqual ((nint) 2, cameraValue, "NeutralValue");
			});
		}
	}
}
#endif // HAS_MEDIAEXTENSION
