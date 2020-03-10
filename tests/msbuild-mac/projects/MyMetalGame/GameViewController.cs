using System;
using System.Runtime.InteropServices;
using System.Threading;

using AppKit;
using Foundation;
using Metal;
using MetalKit;
using ModelIO;
using OpenTK;

namespace MyMetalGame {
	public partial class GameViewController : NSViewController, IMTKViewDelegate {
		struct Uniforms {
			public Matrix4 ModelviewProjectionMatrix;
			public Matrix4 NormalMatrix;
		}

		// The max number of command buffers in flight
		const int MaxInflightBuffers = 3;

		// Max API memory buffer size.
		const int MaxBytesPerFrame = 1024 * 1024;

		// view
		MTKView view;

		// controller
		Semaphore inflightSemaphore;
		IMTLBuffer dynamicConstantBuffer;
		int constantDataBufferIndex;

		// renderer
		IMTLDevice device;
		IMTLCommandQueue commandQueue;
		IMTLLibrary defaultLibrary;
		IMTLRenderPipelineState pipelineState;
		IMTLDepthStencilState depthState;

		// uniforms
		Matrix4 projectionMatrix;
		Matrix4 viewMatrix;
		float rotation;

		// meshes
		MTKMesh boxMesh;

		public GameViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			constantDataBufferIndex = 0;
			inflightSemaphore = new Semaphore (MaxInflightBuffers, MaxInflightBuffers);

			SetupMetal ();
			if (device != null) {
				SetupView ();
				LoadAssets ();
				Reshape ();
			} else {
				Console.WriteLine ("Metal is not supported on this device");
				View = new NSView (View.Frame);
			}
		}

		public void DrawableSizeWillChange (MTKView view, CoreGraphics.CGSize size)
		{
			Reshape ();
		}

		public void Draw (MTKView view)
		{
			Render ();
		}

		void LoadAssets ()
		{
			// Generate meshes
			MDLMesh mdl = MDLMesh.CreateBox (new Vector3 (2f, 2f, 2f), new Vector3i (1, 1, 1), MDLGeometryType.Triangles, false, new MTKMeshBufferAllocator (device));

			NSError error;
			boxMesh = new MTKMesh (mdl, device, out error);

			// Allocate one region of memory for the uniform buffer
			dynamicConstantBuffer = device.CreateBuffer (MaxBytesPerFrame, (MTLResourceOptions) 0);
			dynamicConstantBuffer.Label = "UniformBuffer";

			// Load the fragment program into the library
			IMTLFunction fragmentProgram = defaultLibrary.CreateFunction ("lighting_fragment");

			// Load the vertex program into the library
			IMTLFunction vertexProgram = defaultLibrary.CreateFunction ("lighting_vertex");

			// Create a vertex descriptor from the MTKMesh
			MTLVertexDescriptor vertexDescriptor = MTLVertexDescriptor.FromModelIO (boxMesh.VertexDescriptor);
			vertexDescriptor.Layouts [0].StepRate = 1;
			vertexDescriptor.Layouts [0].StepFunction = MTLVertexStepFunction.PerVertex;

			// Create a reusable pipeline state
			var pipelineStateDescriptor = new MTLRenderPipelineDescriptor {
				Label = "MyPipeline",
				SampleCount = view.SampleCount,
				VertexFunction = vertexProgram,
				FragmentFunction = fragmentProgram,
				VertexDescriptor = vertexDescriptor,
				DepthAttachmentPixelFormat = view.DepthStencilPixelFormat,
				StencilAttachmentPixelFormat = view.DepthStencilPixelFormat
			};

			pipelineStateDescriptor.ColorAttachments [0].PixelFormat = view.ColorPixelFormat;

			pipelineState = device.CreateRenderPipelineState (pipelineStateDescriptor, out error);
			if (pipelineState == null)
				Console.WriteLine ("Failed to created pipeline state, error {0}", error);

			var depthStateDesc = new MTLDepthStencilDescriptor {
				DepthCompareFunction = MTLCompareFunction.Less,
				DepthWriteEnabled = true
			};

			depthState = device.CreateDepthStencilState (depthStateDesc);
		}

		void SetupMetal ()
		{
			// Set the view to use the default device
			device = MTLDevice.SystemDefault;

			// Create a new command queue
			commandQueue = device.CreateCommandQueue ();

			// Load all the shader files with a metal file extension in the project
			defaultLibrary = device.CreateDefaultLibrary ();
		}

		void SetupView ()
		{
			view = (MTKView) View;
			view.Delegate = this;
			view.Device = device;

			// Setup the render target, choose values based on your app
			view.SampleCount = 4;
			view.DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;
		}

		void Render ()
		{
			inflightSemaphore.WaitOne ();

			Update ();

			// Create a new command buffer for each renderpass to the current drawable
			IMTLCommandBuffer commandBuffer = commandQueue.CommandBuffer ();
			commandBuffer.Label = "MyCommand";

			// Call the view's completion handler which is required by the view since it will signal its semaphore and set up the next buffer
			var drawable = view.CurrentDrawable;
			commandBuffer.AddCompletedHandler (buffer => {
				drawable.Dispose ();
				inflightSemaphore.Release ();
			});

			// Obtain a renderPassDescriptor generated from the view's drawable textures
			MTLRenderPassDescriptor renderPassDescriptor = view.CurrentRenderPassDescriptor;

			// If we have a valid drawable, begin the commands to render into it
			if (renderPassDescriptor != null) {
				// Create a render command encoder so we can render into something
				IMTLRenderCommandEncoder renderEncoder = commandBuffer.CreateRenderCommandEncoder (renderPassDescriptor);
				renderEncoder.Label = "MyRenderEncoder";
				renderEncoder.SetDepthStencilState (depthState);

				// Set context state
				renderEncoder.PushDebugGroup ("DrawCube");
				renderEncoder.SetRenderPipelineState (pipelineState);
				renderEncoder.SetVertexBuffer (boxMesh.VertexBuffers [0].Buffer, boxMesh.VertexBuffers [0].Offset, 0);
				renderEncoder.SetVertexBuffer (dynamicConstantBuffer, (nuint) Marshal.SizeOf<Uniforms> (), 1);

				MTKSubmesh submesh = boxMesh.Submeshes [0];
				// Tell the render context we want to draw our primitives
				renderEncoder.DrawIndexedPrimitives (submesh.PrimitiveType, submesh.IndexCount, submesh.IndexType, submesh.IndexBuffer.Buffer, submesh.IndexBuffer.Offset);
				renderEncoder.PopDebugGroup ();

				// We're done encoding commands
				renderEncoder.EndEncoding ();

				// Schedule a present once the framebuffer is complete using the current drawable
				commandBuffer.PresentDrawable (drawable);
			}

			// The render assumes it can now increment the buffer index and that the previous index won't be touched until we cycle back around to the same index
			constantDataBufferIndex = (constantDataBufferIndex + 1) % MaxInflightBuffers;

			// Finalize rendering here & push the command buffer to the GPU
			commandBuffer.Commit ();
		}

		void Update ()
		{
			var baseModel = Matrix4.Mult (CreateMatrixFromTranslation (0f, 0f, 5f), CreateMatrixFromRotation (rotation, 0f, 1f, 0f));
			var baseMv = Matrix4.Mult (viewMatrix, baseModel);
			var modelViewMatrix = Matrix4.Mult (baseMv, CreateMatrixFromRotation (rotation, 1f, 1f, 1f));

			var uniforms = new Uniforms ();
			uniforms.NormalMatrix = Matrix4.Invert (Matrix4.Transpose (modelViewMatrix));
			uniforms.ModelviewProjectionMatrix = Matrix4.Transpose (Matrix4.Mult (projectionMatrix, modelViewMatrix));

			int rawsize = Marshal.SizeOf<Uniforms> ();
			var rawdata = new byte [rawsize];

			GCHandle pinnedUniforms = GCHandle.Alloc (uniforms, GCHandleType.Pinned);
			IntPtr ptr = pinnedUniforms.AddrOfPinnedObject ();
			Marshal.Copy (ptr, rawdata, 0, rawsize);
			pinnedUniforms.Free ();

			Marshal.Copy (rawdata, 0, dynamicConstantBuffer.Contents + rawsize * constantDataBufferIndex, rawsize);
			rotation += .01f;
		}

		void Reshape ()
		{
			// When reshape is called, update the view and projection matricies since this means the view orientation or size changed
			var aspect = (float) (View.Bounds.Size.Width / View.Bounds.Size.Height);
			projectionMatrix = CreateMatrixFromPerspective (65f * ((float) Math.PI / 180f), aspect, .1f, 100f);

			viewMatrix = Matrix4.Identity;
		}

		public static Matrix4 CreateMatrixFromRotation (float radians, float x, float y, float z)
		{
			Vector3 v = Vector3.Normalize (new Vector3 (x, y, z));
			var cos = (float) Math.Cos (radians);
			var sin = (float) Math.Sin (radians);
			float cosp = 1f - cos;

			var m = new Matrix4 {
				Row0 = new Vector4 (cos + cosp * v.X * v.X, cosp * v.X * v.Y - v.Z * sin, cosp * v.X * v.Z + v.Y * sin, 0f),
				Row1 = new Vector4 (cosp * v.X * v.Y + v.Z * sin, cos + cosp * v.Y * v.Y, cosp * v.Y * v.Z - v.X * sin, 0f),
				Row2 = new Vector4 (cosp * v.X * v.Z - v.Y * sin, cosp * v.Y * v.Z + v.X * sin, cos + cosp * v.Z * v.Z, 0f),
				Row3 = new Vector4 (0f, 0f, 0f, 1f)
			};

			return m;
		}

		public static Matrix4 CreateMatrixFromTranslation (float x, float y, float z)
		{
			var m = Matrix4.Identity;
			m.Row0.W = x;
			m.Row1.W = y;
			m.Row2.W = z;
			m.Row3.W = 1f;
			return m;
		}

		public static Matrix4 CreateMatrixFromPerspective (float fovY, float aspect, float nearZ, float farZ)
		{
			float yscale = 1f / (float) Math.Tan (fovY * .5f);
			float xscale = yscale / aspect;
			float q = farZ / (farZ - nearZ);

			var m = new Matrix4 {
				Row0 = new Vector4 (xscale, 0f, 0f, 0f),
				Row1 = new Vector4 (0f, yscale, 0f, 0f),
				Row2 = new Vector4 (0f, 0f, q, q * -nearZ),
				Row3 = new Vector4 (0f, 0f, 1f, 0f)
			};

			return m;
		}
	}
}
