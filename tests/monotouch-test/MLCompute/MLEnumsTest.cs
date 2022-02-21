#if !__WATCHOS__

using System;

using Foundation;
using MLCompute;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.MLCompute {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MLEnumsTest {

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			TestRuntime.AssertNotSimulator ("https://github.com/xamarin/maccore/issues/2271");
		}

		[Test]
		public void GetDebugDescription ()
		{
			Assert.That (MLCActivationType.ReLU.GetDebugDescription (), Is.EqualTo ("ReLU"), "MLCActivationType");
			Assert.That (MLCArithmeticOperation.Add.GetDebugDescription (), Is.EqualTo ("Add"), "MLCArithmeticOperation");
			Assert.That (MLCPaddingPolicy.UsePaddingSize.GetDebugDescription (), Is.EqualTo ("Use Padding Size"), "MLCPaddingPolicy");
			Assert.That (MLCLossType.MeanAbsoluteError.GetDebugDescription (), Is.EqualTo ("Absolute Error"), "MLCLossType");
			Assert.That (MLCReductionType.ArgMax.GetDebugDescription (), Is.EqualTo ("Arg Max"), "MLCReductionType");
			Assert.That (MLCPaddingType.Constant.GetDebugDescription (), Is.EqualTo ("Constant"), "MLCPaddingType");
			Assert.That (MLCConvolutionType.Standard.GetDebugDescription (), Is.EqualTo ("Standard"), "MLCConvolutionType");
			Assert.That (MLCPoolingType.L2Norm.GetDebugDescription (), Is.EqualTo ("L2 Norm"), "MLCPoolingType");
			Assert.That (MLCSoftmaxOperation.LogSoftmax.GetDebugDescription (), Is.EqualTo ("Log Softmax"), "MLCSoftmaxOperation");
			Assert.That (MLCSampleMode.Nearest.GetDebugDescription (), Is.EqualTo ("Nearest"), "MLCSampleMode");
			Assert.That (MLCLstmResultMode.OutputAndStates.GetDebugDescription (), Is.EqualTo ("Output and States"), "MLCLstmResultMode");
		}
	}
}

#endif // !__WATCHOS__
