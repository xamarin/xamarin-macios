using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Metal;
using ModelIO;
using ObjCRuntime;

#if MONOMAC
using AppKit;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace MetalKit {

#if !MONOMAC
	// stubs to ease compilation using [NoiOS], [NoTV]
	interface NSDisplayGamut { }
#endif

	[MacCatalyst (13, 1)]
	[Static]
	interface MTKModel {

		[Field ("MTKModelErrorDomain")]
		NSString ErrorDomain { get; }

		[Field ("MTKModelErrorKey")]
		NSString ErrorKey { get; }
	}

	[MacCatalyst (13, 1)]
#if MONOMAC
	[BaseType (typeof (AppKit.NSView))]
#else
	[BaseType (typeof (UIKit.UIView))]
#endif
	interface MTKView : NSCoding, CALayerDelegate {
		[Export ("initWithFrame:device:")]
		[DesignatedInitializer]
		NativeHandle Constructor (CGRect frameRect, [NullAllowed] IMTLDevice device);

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		MTKViewDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[NullAllowed, Export ("device", ArgumentSemantic.Retain)]
		IMTLDevice Device { get; set; }

		[NullAllowed, Export ("currentDrawable")]
		ICAMetalDrawable CurrentDrawable { get; }

		[Export ("framebufferOnly")]
		bool FramebufferOnly { get; set; }

		[Export ("presentsWithTransaction")]
		bool PresentsWithTransaction { get; set; }

		[Export ("colorPixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat ColorPixelFormat { get; set; }

		[Export ("depthStencilPixelFormat", ArgumentSemantic.Assign)]
		MTLPixelFormat DepthStencilPixelFormat { get; set; }

		[Export ("sampleCount", ArgumentSemantic.Assign)]
		nuint SampleCount { get; set; }

		[Export ("clearColor", ArgumentSemantic.Assign)]
		MTLClearColor ClearColor { get; set; }

		[Export ("clearDepth")]
		double ClearDepth { get; set; }

		[Export ("clearStencil")]
		uint ClearStencil { get; set; }

		[NullAllowed, Export ("depthStencilTexture")]
		IMTLTexture DepthStencilTexture { get; }

		[NullAllowed, Export ("multisampleColorTexture")]
		IMTLTexture MultisampleColorTexture { get; }

		[Export ("releaseDrawables")]
		void ReleaseDrawables ();

		[NullAllowed, Export ("currentRenderPassDescriptor")]
		MTLRenderPassDescriptor CurrentRenderPassDescriptor { get; }

		[Export ("preferredFramesPerSecond", ArgumentSemantic.Assign)]
		nint PreferredFramesPerSecond { get; set; }

		[Export ("enableSetNeedsDisplay")]
		bool EnableSetNeedsDisplay { get; set; }

		[Export ("autoResizeDrawable")]
		bool AutoResizeDrawable { get; set; }

		[Export ("drawableSize", ArgumentSemantic.Assign)]
		CGSize DrawableSize { get; set; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Export ("draw")]
		void Draw ();

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[NullAllowed, Export ("colorspace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("preferredDevice")]
		IMTLDevice PreferredDevice { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("preferredDrawableSize")]
		CGSize PreferredDrawableSize { get; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("depthStencilAttachmentTextureUsage", ArgumentSemantic.Assign)]
		MTLTextureUsage DepthStencilAttachmentTextureUsage { get; set; }

		[iOS (13, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("multisampleColorAttachmentTextureUsage", ArgumentSemantic.Assign)]
		MTLTextureUsage MultisampleColorAttachmentTextureUsage { get; set; }

		[Mac (13, 0), iOS (16, 0), TV (16,0), MacCatalyst (16, 0)]
		[Export ("depthStencilStorageMode", ArgumentSemantic.Assign)]
		MTLStorageMode DepthStencilStorageMode { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface MTKViewDelegate {
		[Abstract]
		[Export ("mtkView:drawableSizeWillChange:")]
		void DrawableSizeWillChange (MTKView view, CGSize size);

		[Abstract]
		[Export ("drawInMTKView:")]
		void Draw (MTKView view);
	}

	[MacCatalyst (13, 1)]
	[Static]
	interface MTKTextureLoaderError {

		[Field ("MTKTextureLoaderErrorDomain")]
		NSString Domain { get; }

		[Field ("MTKTextureLoaderErrorKey")]
		NSString Key { get; }
	}

	[MacCatalyst (13, 1)]
	[Static, Internal]
	interface MTKTextureLoaderKeys {

		[Field ("MTKTextureLoaderOptionAllocateMipmaps")]
		NSString AllocateMipmapsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("MTKTextureLoaderOptionGenerateMipmaps")]
		NSString GenerateMipmapsKey { get; }

		[Field ("MTKTextureLoaderOptionSRGB")]
		NSString SrgbKey { get; }

		[Field ("MTKTextureLoaderOptionTextureUsage")]
		NSString TextureUsageKey { get; }

		[Field ("MTKTextureLoaderOptionTextureCPUCacheMode")]
		NSString TextureCpuCacheModeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("MTKTextureLoaderOptionTextureStorageMode")]
		NSString TextureStorageModeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("MTKTextureLoaderOptionCubeLayout")]
		NSString CubeLayoutKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("MTKTextureLoaderOptionOrigin")]
		NSString OriginKey { get; }

		[iOS (17, 0), Mac (14, 0), TV (17,0), MacCatalyst (17, 0)]
		[Field ("MTKTextureLoaderOptionLoadAsArray")]
		NSString LoadAsArrayKey { get; }

	}

	[MacCatalyst (13, 1)]
	enum MTKTextureLoaderCubeLayout {
		[Field ("MTKTextureLoaderCubeLayoutVertical")]
		Vertical,
	}

	[MacCatalyst (13, 1)]
	enum MTKTextureLoaderOrigin {
		[Field ("MTKTextureLoaderOriginTopLeft")]
		TopLeft,
		[Field ("MTKTextureLoaderOriginBottomLeft")]
		BottomLeft,
		[Field ("MTKTextureLoaderOriginFlippedVertically")]
		FlippedVertically,
	}

	[StrongDictionary ("MTKTextureLoaderKeys")]
	interface MTKTextureLoaderOptions {
		bool AllocateMipmaps { get; set; }

		[MacCatalyst (13, 1)]
		bool GenerateMipmaps { get; set; }

		bool Srgb { get; set; }

		[iOS (17, 0), Mac (14, 0), TV (17,0), MacCatalyst (17, 0)]
		bool LoadAsArray { get; set; }
	}

	[MacCatalyst (13, 1)]
	delegate void MTKTextureLoaderCallback ([NullAllowed] IMTLTexture texture, [NullAllowed] NSError error);

	[MacCatalyst (13, 1)]
	delegate void MTKTextureLoaderArrayCallback (IMTLTexture [] textures, [NullAllowed] NSError error);

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTKTextureLoader {
		[Export ("device")]
		IMTLDevice Device { get; }

		[Export ("initWithDevice:")]
		NativeHandle Constructor (IMTLDevice device);

		[Export ("newTextureWithContentsOfURL:options:completionHandler:"), Internal]
		void FromUrl (NSUrl url, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[Wrap ("FromUrl (url, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromUrl (NSUrl url, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[Export ("newTextureWithData:options:completionHandler:"), Internal]
		void FromData (NSData data, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[Wrap ("FromData (data, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromData (NSData data, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[Export ("newTextureWithCGImage:options:completionHandler:"), Internal]
		void FromCGImage (CGImage cgImage, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[Wrap ("FromCGImage (cgImage, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromCGImage (CGImage cgImage, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[Export ("newTextureWithContentsOfURL:options:error:"), Internal]
		[return: NullAllowed]
		IMTLTexture FromUrl (NSUrl url, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("FromUrl (url, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromUrl (NSUrl url, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("newTexturesWithContentsOfURLs:options:completionHandler:")]
		[Async]
		void FromUrls (NSUrl [] urls, [NullAllowed] NSDictionary options, MTKTextureLoaderArrayCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Wrap ("FromUrls (urls, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromUrls (NSUrl [] urls, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderArrayCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("newTexturesWithContentsOfURLs:options:error:")]
		IMTLTexture [] FromUrls (NSUrl [] urls, [NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("FromUrls (urls, options.GetDictionary (), out error)")]
		IMTLTexture [] FromUrls (NSUrl [] urls, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[Export ("newTextureWithData:options:error:"), Internal]
		[return: NullAllowed]
		IMTLTexture FromData (NSData data, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("FromData (data, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromData (NSData data, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[Export ("newTextureWithCGImage:options:error:"), Internal]
		[return: NullAllowed]
		IMTLTexture FromCGImage (CGImage cgImage, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("FromCGImage (cgImage, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromCGImage (CGImage cgImage, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("newTextureWithName:scaleFactor:bundle:options:completionHandler:")]
		[Async]
		void FromName (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Wrap ("FromName (name, scaleFactor, bundle, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromName (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("newTextureWithName:scaleFactor:displayGamut:bundle:options:completionHandler:")]
		[Async]
		void FromName (string name, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("FromName (name, scaleFactor, displayGamut, bundle, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromName (string name, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("newTexturesWithNames:scaleFactor:bundle:options:completionHandler:")]
		[Async]
		void FromNames (string [] names, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, MTKTextureLoaderArrayCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Wrap ("FromNames (names, scaleFactor, bundle, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromNames (string [] names, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderArrayCallback completionHandler);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Async]
		[Export ("newTexturesWithNames:scaleFactor:displayGamut:bundle:options:completionHandler:")]
		void FromNames (string [] names, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, MTKTextureLoaderArrayCallback completionHandler);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("FromNames (names, scaleFactor, displayGamut, bundle, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromNames (string [] names, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderArrayCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("newTextureWithMDLTexture:options:completionHandler:")]
		[Async]
		void FromTexture (MDLTexture texture, [NullAllowed] NSDictionary options, MTKTextureLoaderCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Wrap ("FromTexture (texture, options.GetDictionary (), completionHandler)")]
		[Async]
		void FromTexture (MDLTexture texture, [NullAllowed] MTKTextureLoaderOptions options, MTKTextureLoaderCallback completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("newTextureWithMDLTexture:options:error:")]
		[return: NullAllowed]
		IMTLTexture FromTexture (MDLTexture texture, [NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("FromTexture (texture, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromTexture (MDLTexture texture, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("newTextureWithName:scaleFactor:bundle:options:error:")]
		[return: NullAllowed]
		IMTLTexture FromName (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, out NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("FromName (name, scaleFactor, bundle, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromName (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, out NSError error);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Export ("newTextureWithName:scaleFactor:displayGamut:bundle:options:error:")]
		[return: NullAllowed]
		IMTLTexture FromName (string name, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary options, [NullAllowed] out NSError error);

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[Wrap ("FromName (name, scaleFactor, displayGamut, bundle, options.GetDictionary (), out error)")]
		[return: NullAllowed]
		IMTLTexture FromName (string name, nfloat scaleFactor, NSDisplayGamut displayGamut, [NullAllowed] NSBundle bundle, [NullAllowed] MTKTextureLoaderOptions options, [NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init is NS_UNAVAILABLE
	interface MTKMeshBufferAllocator : MDLMeshBufferAllocator {
		[Export ("initWithDevice:")]
		NativeHandle Constructor (IMTLDevice device);

		[Export ("device")]
		IMTLDevice Device { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTKMeshBuffer : MDLMeshBuffer, MDLNamed {
		[Export ("buffer")]
		IMTLBuffer Buffer { get; }

		[Export ("offset")]
		nuint Offset { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface MTKSubmesh {
		[Export ("primitiveType")]
		MTLPrimitiveType PrimitiveType { get; }

		[Export ("indexType")]
		MTLIndexType IndexType { get; }

		[Export ("indexBuffer")]
		MTKMeshBuffer IndexBuffer { get; }

		[Export ("indexCount")]
		nuint IndexCount { get; }

		[NullAllowed, Export ("mesh", ArgumentSemantic.Weak)]
		MTKMesh Mesh { get; }

		[Export ("name")]
		string Name { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // init NS_UNAVAILABLE
	interface MTKMesh {
		[Export ("initWithMesh:device:error:")]
		NativeHandle Constructor (MDLMesh mesh, IMTLDevice device, out NSError error);

		// generator does not like `out []` -> https://trello.com/c/sZYNalbB/524-generator-support-out
		[Internal] // there's another, manual, public API exposed
		[Static]
		[Export ("newMeshesFromAsset:device:sourceMeshes:error:")]
		[return: NullAllowed]
		MTKMesh [] FromAsset (MDLAsset asset, IMTLDevice device, out NSArray sourceMeshes, out NSError error);

		[Export ("vertexBuffers")]
		MTKMeshBuffer [] VertexBuffers { get; }

		[Export ("vertexDescriptor")]
		MDLVertexDescriptor VertexDescriptor { get; }

		[Export ("submeshes")]
		MTKSubmesh [] Submeshes { get; }

		[Export ("vertexCount")]
		nuint VertexCount { get; }

		[Export ("name")]
		string Name { get; set; }
	}
}
