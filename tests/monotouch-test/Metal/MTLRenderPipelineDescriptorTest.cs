#if !__WATCHOS__
using Foundation;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class MTLRenderPipelineDescriptorTest {
		MTLRenderPipelineDescriptor descriptor = null;

		[SetUp]
		public void SetUp ()
		{
			TestRuntime.AssertXcodeVersion (13, 0);
			descriptor = new MTLRenderPipelineDescriptor ();
		}

		[TearDown]
		public void TearDown ()
		{
			if (descriptor is not null)
				descriptor.Dispose ();
			descriptor = null;
		}

		[Test]
		public void LabelTest ()
		{
			var label = "my label";
			Assert.DoesNotThrow (() => {
				descriptor.Label = label;
			}, "Set label");
			Assert.AreEqual (label, descriptor.Label, "Get label");
		}

		[Test]
		public void VertexFunctionTest ()
			=> Assert.DoesNotThrow (() => {
				var buffers = descriptor.VertexBuffers;
			});

		[Test]
		public void FragmentFunctionTest ()
			=> Assert.DoesNotThrow (() => {
				var functions = descriptor.FragmentBuffers;
			});

		[Test]
		public void VertexDescriptorTest ()
			=> Assert.DoesNotThrow (() => {
				var vertext = descriptor.VertexDescriptor;
			});

		[Test]
		public void SampleCountTest ()
			=> Assert.DoesNotThrow (() => {
				var count = descriptor.SampleCount;
			});

		[Test]
		public void AlphaToCoverageEnabledTest ()
			=> Assert.DoesNotThrow (() => {
				var enabled = descriptor.AlphaToCoverageEnabled;
			});

		[Test]
		public void AlphaToOneEnabledTest ()
			=> Assert.DoesNotThrow (() => {
				var enabled = descriptor.AlphaToOneEnabled;
			});

		[Test]
		public void RasterizationEnabledTest ()
			=> Assert.DoesNotThrow (() => {
				var enabled = descriptor.RasterizationEnabled;
			});

		[Test]
		public void ResetTest ()
			=> Assert.DoesNotThrow (() => descriptor.Reset ());

		[Test]
		public void ColorAttachments ()
			=> Assert.DoesNotThrow (() => {
				var attachment = descriptor.ColorAttachments;
			});

		[Test]
		public void DepthAttachmentPixelFormatTest ()
			=> Assert.DoesNotThrow (() => {
				var depth = descriptor.DepthAttachmentPixelFormat;
			});

		[Test]
		public void StencilAttachmentPixelFormatTest ()
			=> Assert.DoesNotThrow (() => {
				var stencil = descriptor.StencilAttachmentPixelFormat;
			});

		[Test]
		public void InputPrimitiveTopologyTest ()
			=> Assert.DoesNotThrow (() => {
				var input = descriptor.InputPrimitiveTopology;
			});

		[Test]
		public void TessellationPartitionModeTest ()
			=> Assert.DoesNotThrow (() => {
				var mode = descriptor.TessellationPartitionMode;
			});

		[Test]
		public void MaxTessellationFactorTest ()
			=> Assert.DoesNotThrow (() => {
				var factor = descriptor.MaxTessellationFactor;
			});

		[Test]
		public void IsTessellationFactorScaleEnabledTest ()
			=> Assert.DoesNotThrow (() => {
				var enabled = descriptor.IsTessellationFactorScaleEnabled;
			});

		[Test]
		public void TessellationFactorFormatTest ()
			=> Assert.DoesNotThrow (() => {
				var format = descriptor.TessellationFactorFormat;
			});

		[Test]
		public void TessellationControlPointIndexTypeTest ()
			=> Assert.DoesNotThrow (() => {
				var point = descriptor.TessellationControlPointIndexType;
			});
	}
}
#endif
