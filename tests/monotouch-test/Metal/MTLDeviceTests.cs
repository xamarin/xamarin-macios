#if MONOMAC || __IOS__

using System;
using System.IO;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using Metal;

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	[Preserve (AllMembers = true)]
	public class MTLDeviceTests {
		[SetUp]
		public void Setup ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
		}

#if __MACOS__ || __MACCATALYST__
		[Test]
		public void GetAllDevicesTest ()
		{
#if __MACCATALYST__
			TestRuntime.AssertXcodeVersion (13, 0);
#endif
			NSObject refObj = new NSObject ();
			var devices = MTLDevice.GetAllDevices ();

			// It's possible to run on a system that does not support metal,
			// in which case we'll get an empty array of devices.
			Assert.IsNotNull (devices, "MTLDevices.GetAllDevices not null");
		}
#endif

#if __MACOS__
		[Test]
		public void GetAllDevicesTestOutObserver ()
		{
			var devices = MTLDevice.GetAllDevices ((IMTLDevice device, NSString notifyName) => { }, out var observer);

			// It's possible to run on a system that does not support metal,
			// in which case we'll get an empty array of devices.
			Assert.IsNotNull (devices, "MTLDevices.GetAllDevices not null");

			MTLDevice.RemoveObserver (observer);
		}
#endif

		[Test]
		public void SystemDefault ()
		{
			Assert.DoesNotThrow (() => { var obj = MTLDevice.SystemDefault; }, "No exception");
		}

		[DllImport (Constants.libcLibrary)]
		static extern int getpagesize ();

		[DllImport (Constants.libcLibrary)]
		static extern IntPtr mmap (IntPtr start, nint length, int prot, int flags, int fd, nint offset);
		static IntPtr AllocPageAligned (int pages, out int length)
		{
			length = pages * getpagesize ();
			var rv = mmap (IntPtr.Zero, length, 0x1 /* PROT_READ */ | 0x2 /*   */, 0x0002 /* MAP_PRIVATE */ | 0x1000 /* MAP_ANONYMOUS */, 0, 0);
			return rv;
		}
		[DllImport (Constants.libcLibrary)]
		static extern int munmap (IntPtr addr, nint size);
		static void FreePageAligned (IntPtr ptr, int length)
		{
			munmap (ptr, length);
		}

		[Test]
		public void ReturnReleaseTest ()
		{
			// This test tries to exercise all the Metal API that has a
			// ReturnRelease attribute. To test that the attribute does the
			// right thing: run the test app using instruments, run the test
			// several times by tapping on it, and do a heap mark between each
			// test. Then verify that there's at least one heap shot with 0
			// memory increase, which means that nothing is leaking.
			var device = MTLDevice.SystemDefault;
			IntPtr buffer_mem;
			int buffer_length;
			bool freed;
			byte [] buffer_bytes;

			// some older hardware won't have a default
			if (device is null)
				Assert.Inconclusive ("Metal is not supported");

			// Apple claims that "Indirect command buffers" are available with MTLGPUFamilyCommon2, but it crashes on at least one machine.
			// Log what the current device supports, just to have it in the log.
#if NET
			foreach (MTLFeatureSet fs in Enum.GetValues<MTLFeatureSet> ()) {
#else
			foreach (MTLFeatureSet fs in Enum.GetValues (typeof (MTLFeatureSet))) {
#endif
				Console.WriteLine ($"This device supports feature set: {fs}: {device.SupportsFeatureSet (fs)}");
			}
			if (TestRuntime.CheckXcodeVersion (11, 0)) {
#if NET
				foreach (var gf in Enum.GetValues<MTLGpuFamily> ()) {
#else
				foreach (MTLGpuFamily gf in Enum.GetValues (typeof (MTLGpuFamily))) {
#endif
					Console.WriteLine ($"This device supports Gpu family: {gf}: {device.SupportsFamily (gf)}");
				}
			}


			string metal_code = File.ReadAllText (Path.Combine (NSBundle.MainBundle.ResourcePath, "metal-sample.metal"));
			string metallib_path = Path.Combine (NSBundle.MainBundle.ResourcePath, "default.metallib");
			string fragmentshader_path = Path.Combine (NSBundle.MainBundle.ResourcePath, "fragmentShader.metallib");

#if !__MACOS__ && !__MACCATALYST__
			if (Runtime.Arch == Arch.SIMULATOR)
				Assert.Ignore ("Metal isn't available in the simulator");
#endif
			using (var hd = new MTLHeapDescriptor ()) {
				hd.CpuCacheMode = MTLCpuCacheMode.DefaultCache;
				hd.StorageMode = MTLStorageMode.Private;
				using (var txt = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 40, 40, false)) {
					var sa = device.GetHeapTextureSizeAndAlign (txt);
					hd.Size = sa.Size;
					using (var heap = device.CreateHeap (hd)) {
						Assert.IsNotNull (heap, $"NonNullHeap");
					}
				}
			}

			using (var queue = device.CreateCommandQueue ()) {
				Assert.IsNotNull (queue, "Queue: NonNull 1");
			}

#if __MACOS__
			if (TestRuntime.CheckXcodeVersion (10, 0) && device.SupportsFeatureSet (MTLFeatureSet.macOS_GPUFamily2_v1)) {
				using (var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 64, 64, false)) {
					descriptor.StorageMode = MTLStorageMode.Private;
					using (var texture = device.CreateSharedTexture (descriptor)) {
						Assert.IsNotNull (texture, "CreateSharedTexture (MTLTextureDescriptor): NonNull");

						using (var handle = texture.CreateSharedTextureHandle ())
						using (var shared = device.CreateSharedTexture (handle))
							Assert.IsNotNull (texture, "CreateSharedTexture (MTLSharedTextureHandle): NonNull");
					}
				}
			}
#endif

			using (var queue = device.CreateCommandQueue (10)) {
				Assert.IsNotNull (queue, "Queue: NonNull 2");
			}

			using (var buffer = device.CreateBuffer (1024, MTLResourceOptions.CpuCacheModeDefault)) {
				Assert.IsNotNull (buffer, "CreateBuffer: NonNull 1");
			}

			buffer_mem = AllocPageAligned (1, out buffer_length);
			using (var buffer = device.CreateBuffer (buffer_mem, (nuint) buffer_length, MTLResourceOptions.CpuCacheModeDefault)) {
				Assert.IsNotNull (buffer, "CreateBuffer: NonNull 2");
			}
			FreePageAligned (buffer_mem, buffer_length);

			buffer_bytes = new byte [getpagesize ()];
			using (var buffer = device.CreateBuffer (buffer_bytes, MTLResourceOptions.CpuCacheModeDefault)) {
				Assert.IsNotNull (buffer, "CreateBuffer: NonNull 3");
			}

			buffer_mem = AllocPageAligned (1, out buffer_length);
			freed = false;
#if __MACOS__
			var resourceOptions7 = MTLResourceOptions.StorageModeManaged;
#else
			var resourceOptions7 = MTLResourceOptions.CpuCacheModeDefault;
#endif
			using (var buffer = device.CreateBufferNoCopy (buffer_mem, (nuint) buffer_length, resourceOptions7, (pointer, length) => { FreePageAligned (pointer, (int) length); freed = true; })) {
				Assert.IsNotNull (buffer, "CreateBufferNoCopy: NonNull 1");
			}
			Assert.IsTrue (freed, "CreateBufferNoCopy: Freed 1");

			using (var descriptor = new MTLDepthStencilDescriptor ())
			using (var dss = device.CreateDepthStencilState (descriptor)) {
				Assert.IsNotNull (dss, "CreateDepthStencilState: NonNull 1");
			}

			using (var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 64, 64, false)) {
				using (var texture = device.CreateTexture (descriptor))
					Assert.NotNull (texture, "CreateTexture: NonNull 1");

				using (var surface = new IOSurface.IOSurface (new IOSurface.IOSurfaceOptions {
					Width = 64,
					Height = 64,
					BytesPerElement = 4,
				})) {
					using (var texture = device.CreateTexture (descriptor, surface, 0))
						Assert.NotNull (texture, "CreateTexture: NonNull 2");
				}
			}

			using (var descriptor = new MTLSamplerDescriptor ())
			using (var sampler = device.CreateSamplerState (descriptor))
				Assert.IsNotNull (sampler, "CreateSamplerState: NonNull 1");

			using (var library = device.CreateDefaultLibrary ())
				Assert.IsNotNull (library, "CreateDefaultLibrary: NonNull 1");

			using (var library = device.CreateLibrary (metallib_path, out var error)) {
				Assert.IsNotNull (library, "CreateLibrary: NonNull 1");
				Assert.IsNull (error, "CreateLibrary: NonNull error 1");
			}

			using (var data = DispatchData.FromByteBuffer (File.ReadAllBytes (metallib_path)))
			using (var library = device.CreateLibrary (data, out var error)) {
				Assert.IsNotNull (library, "CreateLibrary: NonNull 2");
				Assert.IsNull (error, "CreateLibrary: NonNull error 2");
			}

			using (var compile_options = new MTLCompileOptions ())
			using (var library = device.CreateLibrary (metal_code, compile_options, out var error)) {
				Assert.IsNotNull (library, "CreateLibrary: NonNull 3");
				Assert.IsNull (error, "CreateLibrary: NonNull error 3");
			}

			using (var compile_options = new MTLCompileOptions ()) {
				device.CreateLibrary (metal_code, compile_options, (library, error) => {
					Assert.IsNotNull (library, "CreateLibrary: NonNull 4");
					Assert.IsNull (error, "CreateLibrary: NonNull error 4");
				});
			}

			using (var library = device.CreateDefaultLibrary (NSBundle.MainBundle, out var error)) {
				Assert.IsNotNull (library, "CreateDefaultLibrary: NonNull 2");
				Assert.IsNull (error, "CreateDefaultLibrary: NonNull error 2");
			}

			using (var descriptor = new MTLRenderPipelineDescriptor ())
			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("vertexShader")) {
				descriptor.VertexFunction = func;
				descriptor.ColorAttachments [0].PixelFormat = MTLPixelFormat.BGRA8Unorm_sRGB;
				using (var rps = device.CreateRenderPipelineState (descriptor, out var error)) {
					Assert.IsNotNull (rps, "CreateRenderPipelineState: NonNull 1");
					Assert.IsNull (error, "CreateRenderPipelineState: NonNull error 1");
				}
			}

			using (var descriptor = new MTLRenderPipelineDescriptor ())
			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("vertexShader")) {
				descriptor.VertexFunction = func;
				descriptor.ColorAttachments [0].PixelFormat = MTLPixelFormat.BGRA8Unorm_sRGB;
				using (var rps = device.CreateRenderPipelineState (descriptor, MTLPipelineOption.BufferTypeInfo, out var reflection, out var error)) {
					Assert.IsNotNull (rps, "CreateRenderPipelineState: NonNull 2");
					Assert.IsNull (error, "CreateRenderPipelineState: NonNull error 2");
					Assert.IsNotNull (reflection, "CreateRenderPipelineState: NonNull reflection 2");
				}
			}

			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("grayscaleKernel"))
			using (var cps = device.CreateComputePipelineState (func, MTLPipelineOption.ArgumentInfo, out var reflection, out var error)) {
				Assert.IsNotNull (cps, "CreateComputePipelineState: NonNull 1");
				Assert.IsNull (error, "CreateComputePipelineState: NonNull error 1");
				Assert.IsNotNull (reflection, "CreateComputePipelineState: NonNull reflection 1");
			}

			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("grayscaleKernel"))
			using (var cps = device.CreateComputePipelineState (func, out var error)) {
				Assert.IsNotNull (cps, "CreateComputePipelineState: NonNull 2");
				Assert.IsNull (error, "CreateComputePipelineState: NonNull error 2");
			}

			using (var descriptor = new MTLComputePipelineDescriptor ())
			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("grayscaleKernel")) {
				descriptor.ComputeFunction = func;
				using (var cps = device.CreateComputePipelineState (descriptor, MTLPipelineOption.BufferTypeInfo, out var reflection, out var error)) {
					Assert.IsNotNull (cps, "CreateComputePipelineState: NonNull 3");
					Assert.IsNull (error, "CreateComputePipelineState: NonNull error 3");
					Assert.IsNotNull (reflection, "CreateComputePipelineState: NonNull reflection 3");
				}
			}

			using (var fence = device.CreateFence ()) {
				Assert.IsNotNull (fence, "CreateFence 1: NonNull");
			}

			var url = "file://" + metallib_path;
			url = url.Replace (" ", "%20"); // url encode!
			using (var library = device.CreateLibrary (new NSUrl (url), out var error)) {
#if NET
				// Looks like creating a library with a url always fails: https://forums.developer.apple.com/thread/110416
				Assert.IsNotNull (library, "CreateLibrary (NSUrl, NSError): Null");
				Assert.IsNull (error, "CreateLibrary (NSUrl, NSError): NonNull error");
#else
				// Looks like creating a library with a url always fails: https://forums.developer.apple.com/thread/110416
				Assert.IsNull (library, "CreateLibrary (NSUrl, NSError): Null");
				Assert.IsNotNull (error, "CreateLibrary (NSUrl, NSError): NonNull error");
#endif
			}

			using (var library = device.CreateArgumentEncoder (new MTLArgumentDescriptor [] { new MTLArgumentDescriptor () { DataType = MTLDataType.Int } })) {
				Assert.IsNotNull (library, "CreateArgumentEncoder (MTLArgumentDescriptor[]): NonNull");
			}

			// Apple's charts say that "Indirect command buffers" are supported with MTLGpuFamilyCommon2
			var supportsIndirectCommandBuffers = TestRuntime.CheckXcodeVersion (11, 0) && device.SupportsFamily (MTLGpuFamily.Common2);
#if __MACOS__
			// but something's not quite right somewhere, so on macOS verify that the device supports a bit more than what Apple says.
			supportsIndirectCommandBuffers &= device.SupportsFeatureSet (MTLFeatureSet.macOS_GPUFamily2_v1);
#endif
			if (supportsIndirectCommandBuffers) {
				using (var descriptor = new MTLIndirectCommandBufferDescriptor ()) {
					using (var library = device.CreateIndirectCommandBuffer (descriptor, 1, MTLResourceOptions.CpuCacheModeDefault)) {
						Assert.IsNotNull (library, "CreateIndirectCommandBuffer: NonNull");
					}
				}

				using (var evt = device.CreateEvent ()) {
					Assert.IsNotNull (evt, "CreateEvent: NonNull");
				}

				using (var evt = device.CreateSharedEvent ()) {
					Assert.IsNotNull (evt, "CreateSharedEvent: NonNull");
				}

				using (var evt1 = device.CreateSharedEvent ())
				using (var evt_handle = evt1.CreateSharedEventHandle ())
				using (var evt = device.CreateSharedEvent (evt_handle)) {
					Assert.IsNotNull (evt, "CreateSharedEvent (MTLSharedEventHandle): NonNull");
				}
			}

			using (var descriptor = new MTLRenderPipelineDescriptor ())
			using (var library = device.CreateDefaultLibrary ())
			using (var func = library.CreateFunction ("vertexShader")) {
				descriptor.VertexFunction = func;
				descriptor.ColorAttachments [0].PixelFormat = MTLPixelFormat.BGRA8Unorm_sRGB;
				using (var rps = device.CreateRenderPipelineState (descriptor, MTLPipelineOption.ArgumentInfo, out var reflection, out var error)) {
					Assert.IsNotNull (rps, "CreateRenderPipelineState (MTLTileRenderPipelineDescriptor, MTLPipelineOption, MTLRenderPipelineReflection, NSError): NonNull");
					Assert.IsNull (error, "CreateRenderPipelineState (MTLTileRenderPipelineDescriptor, MTLPipelineOption, MTLRenderPipelineReflection, NSError: NonNull error");
					Assert.IsNotNull (reflection, "CreateRenderPipelineState (MTLTileRenderPipelineDescriptor, MTLPipelineOption, MTLRenderPipelineReflection, NSError): NonNull reflection");
				}
			}

			using (var buffer = device.CreateBuffer (1024, MTLResourceOptions.CpuCacheModeDefault))
			using (var descriptor = new MTLTextureDescriptor ())
			using (var texture = buffer.CreateTexture (descriptor, 0, 256)) {
				Assert.IsNotNull (buffer, "MTLBuffer.CreateTexture (MTLTextureDescriptor, nuint, nuint): NonNull");
			}

			using (var descriptor = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 64, 64, false))
			using (var texture = device.CreateTexture (descriptor)) {
				using (var view = texture.CreateTextureView (MTLPixelFormat.RGBA8Unorm)) {
					Assert.IsNotNull (view, "MTLTexture.CreateTextureView (MTLPixelFormat): nonnull");
				}
				using (var view = texture.CreateTextureView (MTLPixelFormat.RGBA8Unorm, MTLTextureType.k2D, new NSRange (0, 1), new NSRange (0, 1))) {
					Assert.IsNotNull (view, "MTLTexture.CreateTextureView (MTLPixelFormat, MTLTextureType, NSRange, NSRange): nonnull");
				}
			}

			using (var library = device.CreateLibrary (fragmentshader_path, out var error)) {
				Assert.IsNull (error, "MTLFunction.CreateArgumentEncoder: library creation failure");
				using (var func = library.CreateFunction ("fragmentShader2")) {
					using (var enc = func.CreateArgumentEncoder (0)) {
						Assert.IsNotNull (enc, "MTLFunction.CreateArgumentEncoder (nuint): NonNull");
					}
					using (var enc = func.CreateArgumentEncoder (0, out var reflection)) {
						Assert.IsNotNull (enc, "MTLFunction.CreateArgumentEncoder (nuint, MTLArgument): NonNull");
						Assert.IsNotNull (reflection, "MTLFunction.CreateArgumentEncoder (nuint, MTLArgument): NonNull reflection");
					}
				}
			}

			using (var library = device.CreateDefaultLibrary ()) {
				using (var func = library.CreateFunction ("grayscaleKernel")) {
					Assert.IsNotNull (func, "CreateFunction (string): nonnull");
				}
				if (TestRuntime.CheckXcodeVersion (9, 0)) { // MTLFunctionConstantValues didn't have a default ctor until Xcode 9.
					using (var constants = new MTLFunctionConstantValues ())
					using (var func = library.CreateFunction ("grayscaleKernel", constants, out var error)) {
						Assert.IsNotNull (func, "CreateFunction (string, MTLFunctionConstantValues, NSError): nonnull");
						Assert.IsNull (error, "CreateFunction (string, MTLFunctionConstantValues, NSError): null error");
					}
				}
			}

			using (var hd = new MTLHeapDescriptor ()) {
				hd.CpuCacheMode = MTLCpuCacheMode.DefaultCache;
				hd.StorageMode = MTLStorageMode.Private;
				using (var txt = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 40, 40, false)) {
					var sa = device.GetHeapTextureSizeAndAlign (txt);
					hd.Size = sa.Size;
					using (var heap = device.CreateHeap (hd))
					using (var buffer = heap.CreateBuffer (1024, MTLResourceOptions.StorageModePrivate)) {
						Assert.IsNotNull (buffer, "MTLHeap.CreateBuffer (nuint, MTLResourceOptions): nonnull");
					}
				}
			}

			using (var hd = new MTLHeapDescriptor ()) {
				hd.CpuCacheMode = MTLCpuCacheMode.DefaultCache;
#if __MACOS__ || __MACCATALYST__
				hd.StorageMode = MTLStorageMode.Private;
#else
				hd.StorageMode = MTLStorageMode.Shared;
#endif
				using (var txt = MTLTextureDescriptor.CreateTexture2DDescriptor (MTLPixelFormat.RGBA8Unorm, 40, 40, false)) {
					var sa = device.GetHeapTextureSizeAndAlign (txt);
					hd.Size = sa.Size;

					using (var heap = device.CreateHeap (hd)) {
#if __MACOS__ || __MACCATALYST__
						txt.StorageMode = MTLStorageMode.Private;
#endif
						using (var texture = heap.CreateTexture (txt)) {
							Assert.IsNotNull (texture, "MTLHeap.CreateTexture (MTLTextureDescriptor): nonnull");
						}
					}
				}
			}

			using (var scope = MTLCaptureManager.Shared.CreateNewCaptureScope (device)) {
				Assert.IsNotNull (scope, "MTLCaptureManager.CreateNewCaptureScope (MTLDevice): nonnull");
			}

			using (var queue = device.CreateCommandQueue ())
			using (var scope = MTLCaptureManager.Shared.CreateNewCaptureScope (queue)) {
				Assert.IsNotNull (scope, "MTLCaptureManager.CreateNewCaptureScope (MTLCommandQueue): nonnull");
			}

			TestRuntime.AssertXcodeVersion (10, 0);
			using (var evt = device.CreateSharedEvent ())
			using (var shared = evt.CreateSharedEventHandle ()) {
				Assert.IsNotNull (shared, "MTLSharedEvent.CreateSharedEvent: NonNull");
			}
		}
	}
}
#endif
