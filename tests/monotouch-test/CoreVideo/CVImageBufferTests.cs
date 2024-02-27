//
// Unit tests for CVImageBuffer
//
// Authors:
//	Alex Soto <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;
using CoreVideo;

namespace MonoTouchFixtures.CoreVideo {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CVImageBufferTests {

		[Test]
		public void CVImageBufferYCbCrMatrixTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var codepoint = CVImageBuffer.GetCodePoint (CVImageBufferYCbCrMatrix.ItuR2020);
			var matrixOption = CVImageBuffer.GetYCbCrMatrixOption (codepoint);
			Assert.AreEqual (CVImageBufferYCbCrMatrix.ItuR2020, matrixOption, "ItuR2020");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferYCbCrMatrix.ItuR601_4);
			matrixOption = CVImageBuffer.GetYCbCrMatrixOption (codepoint);
			Assert.AreEqual (CVImageBufferYCbCrMatrix.ItuR601_4, matrixOption, "ItuR601_4");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferYCbCrMatrix.ItuR709_2);
			matrixOption = CVImageBuffer.GetYCbCrMatrixOption (codepoint);
			Assert.AreEqual (CVImageBufferYCbCrMatrix.ItuR709_2, matrixOption, "ItuR709_2");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferYCbCrMatrix.Smpte240M1995);
			matrixOption = CVImageBuffer.GetYCbCrMatrixOption (codepoint);
			Assert.AreEqual (CVImageBufferYCbCrMatrix.Smpte240M1995, matrixOption, "Smpte240M1995");
		}

		[Test]
		public void CVImageBufferColorPrimariesTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var codepoint = CVImageBuffer.GetCodePoint (CVImageBufferColorPrimaries.ItuR2020);
			var matrixOption = CVImageBuffer.GetColorPrimariesOption (codepoint);
			Assert.AreEqual (CVImageBufferColorPrimaries.ItuR2020, matrixOption, "ItuR2020");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferColorPrimaries.Ebu3213);
			matrixOption = CVImageBuffer.GetColorPrimariesOption (codepoint);
			Assert.AreEqual (CVImageBufferColorPrimaries.Ebu3213, matrixOption, "Ebu3213");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferColorPrimaries.ItuR709_2);
			matrixOption = CVImageBuffer.GetColorPrimariesOption (codepoint);
			Assert.AreEqual (CVImageBufferColorPrimaries.ItuR709_2, matrixOption, "ItuR709_2");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferColorPrimaries.P22);
			matrixOption = CVImageBuffer.GetColorPrimariesOption (codepoint);
			Assert.AreEqual (CVImageBufferColorPrimaries.P22, matrixOption, "P22");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferColorPrimaries.SmpteC);
			matrixOption = CVImageBuffer.GetColorPrimariesOption (codepoint);
			Assert.AreEqual (CVImageBufferColorPrimaries.SmpteC, matrixOption, "SmpteC");
		}

		[Test]
		public void CVImageBufferTransferFunctionTest ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);

			var codepoint = CVImageBuffer.GetCodePoint (CVImageBufferTransferFunction.ItuR2100Hlg);
			var matrixOption = CVImageBuffer.GetTransferFunctionOption (codepoint);
			Assert.AreEqual (CVImageBufferTransferFunction.ItuR2100Hlg, matrixOption, "ItuR2100Hlg");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferTransferFunction.ItuR709_2);
			matrixOption = CVImageBuffer.GetTransferFunctionOption (codepoint);
			Assert.AreEqual (CVImageBufferTransferFunction.ItuR709_2, matrixOption, "ItuR709_2");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferTransferFunction.Smpte240M1995);
			matrixOption = CVImageBuffer.GetTransferFunctionOption (codepoint);
			Assert.AreEqual (CVImageBufferTransferFunction.Smpte240M1995, matrixOption, "Smpte240M1995");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferTransferFunction.SmpteST2084PQ);
			matrixOption = CVImageBuffer.GetTransferFunctionOption (codepoint);
			Assert.AreEqual (CVImageBufferTransferFunction.SmpteST2084PQ, matrixOption, "SmpteST2084PQ");

			codepoint = CVImageBuffer.GetCodePoint (CVImageBufferTransferFunction.SmpteST428_1);
			matrixOption = CVImageBuffer.GetTransferFunctionOption (codepoint);
			Assert.AreEqual (CVImageBufferTransferFunction.SmpteST428_1, matrixOption, "SmpteST428_1");
		}
	}
}
