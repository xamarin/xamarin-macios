#if !__WATCHOS__ && !__TVOS__

using System;
using Foundation;
using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLIntersectionFunctionTableTests {
		IMTLDevice device;
		IMTLIntersectionFunctionTable functionTable;
		IMTLComputePipelineState pipelineState;
		IMTLLibrary library;
		IMTLFunction function;
		MTLIntersectionFunctionTableDescriptor descriptor;

		[SetUp]
		public void SetUp ()
		{

			TestRuntime.AssertXcodeVersion (12, 0);

			device = MTLDevice.SystemDefault;
			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			library = device.CreateDefaultLibrary ();
			if (library is null)  // this happens on a simulator
				Assert.Inconclusive ("Could not get the functions library for the device.");

			if (library.FunctionNames.Length == 0)
				Assert.Inconclusive ("Could not get functions for the pipeline.");

			function = library.CreateFunction (library.FunctionNames [0]);
			pipelineState = device.CreateComputePipelineState (function, MTLPipelineOption.ArgumentInfo, out MTLComputePipelineReflection reflection, out NSError error);

			if (error is not null) {
				Assert.Inconclusive ($"Could not create pipeline {error}");
			}
			descriptor = MTLIntersectionFunctionTableDescriptor.Create ();
			functionTable = pipelineState.CreateIntersectionFunctionTable (descriptor);
		}

		[TearDown]
		public void TearDown ()
		{
			functionTable?.Dispose ();
			pipelineState?.Dispose ();
			library?.Dispose ();
			function?.Dispose ();
			descriptor?.Dispose ();
		}

		[Test]
		public void SetBuffersTest ()
		{
			Assert.Throws<ArgumentNullException> (() => {
				functionTable.SetBuffers (null, new nuint [0], new NSRange ());
			}, "Null buffers should throw.");

			Assert.Throws<ArgumentNullException> (() => {
				functionTable.SetBuffers (new IMTLBuffer [0], null, new NSRange ());
			}, "Null offsets should throw.");

			// assert we do not crash or throw, we are testing the extension method
			Assert.DoesNotThrow (() => {
				functionTable.SetBuffers (new IMTLBuffer [0], new nuint [0], new NSRange ());
			}, "Should not throw");
		}

	}
}
#endif
