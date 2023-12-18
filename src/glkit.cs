//
// GLKit.cs: bindings to the iOS5/Lion GLKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011 Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreFoundation;
using ModelIO;

#if NET
using Vector3 = global::System.Numerics.Vector3;
using Vector4 = global::System.Numerics.Vector4;
using Matrix3 = global::CoreGraphics.RMatrix3;
using Matrix4 = global::System.Numerics.Matrix4x4;
#else
using Vector3 = global::OpenTK.Vector3;
using Vector4 = global::OpenTK.Vector4;
using Matrix3 = global::OpenTK.Matrix3;
using Matrix4 = global::OpenTK.Matrix4;
#endif // NET 

#if MONOMAC
#if NET
using pfloat = System.Runtime.InteropServices.NFloat;
#else
using pfloat = System.nfloat;
#endif
using AppKit;
using EAGLSharegroup = Foundation.NSObject;
using EAGLContext = Foundation.NSObject;
using UIView = AppKit.NSView;
using UIImage = AppKit.NSImage;
using UIViewController = AppKit.NSViewController;
#else
using OpenGLES;
using UIKit;
using pfloat = System.Single;
using NSOpenGLContext = Foundation.NSObject;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace GLKit {

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[Static]
	interface GLKModelError {

		[Field ("kGLKModelErrorDomain")]
		NSString Domain { get; }

		[Field ("kGLKModelErrorKey")]
		NSString Key { get; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	interface GLKBaseEffect : GLKNamedEffect {
		[Export ("colorMaterialEnabled", ArgumentSemantic.Assign)]
		bool ColorMaterialEnabled { get; set; }

		[Export ("useConstantColor", ArgumentSemantic.Assign)]
		bool UseConstantColor { get; set; }

		[Export ("transform")]
		GLKEffectPropertyTransform Transform { get; }

		[Export ("light0")]
		GLKEffectPropertyLight Light0 { get; }

		[Export ("light1")]
		GLKEffectPropertyLight Light1 { get; }

		[Export ("light2")]
		GLKEffectPropertyLight Light2 { get; }

		[Export ("lightingType", ArgumentSemantic.Assign)]
		GLKLightingType LightingType { get; set; }

		[Export ("lightModelAmbientColor", ArgumentSemantic.Assign)]
		Vector4 LightModelAmbientColor { [Align (16)] get; set; }

		[Export ("material")]
		GLKEffectPropertyMaterial Material { get; }

		[Export ("texture2d0")]
		GLKEffectPropertyTexture Texture2d0 { get; }

		[Export ("texture2d1")]
		GLKEffectPropertyTexture Texture2d1 { get; }

		[NullAllowed] // by default this property is null
		[Export ("textureOrder", ArgumentSemantic.Copy)]
		GLKEffectPropertyTexture [] TextureOrder { get; set; }

		[Export ("constantColor", ArgumentSemantic.Assign)]
		Vector4 ConstantColor { [Align (16)] get; set; }

		[Export ("fog")]
		GLKEffectPropertyFog Fog { get; }

		[Export ("label", ArgumentSemantic.Copy)]
		[DisableZeroCopy]
		[NullAllowed] // default is null on iOS 5.1.1
		string Label { get; set; }

		[Export ("lightModelTwoSided", ArgumentSemantic.Assign)]
		bool LightModelTwoSided { get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	interface GLKEffectProperty {
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKEffectProperty))]
	interface GLKEffectPropertyFog {
		[Export ("mode", ArgumentSemantic.Assign)]
		GLKFogMode Mode { get; set; }

		[Export ("color", ArgumentSemantic.Assign)]
		Vector4 Color { [Align (16)] get; set; }

		[Export ("density", ArgumentSemantic.Assign)]
		float Density { get; set; } /* GLfloat = float */

		[Export ("start", ArgumentSemantic.Assign)]
		float Start { get; set; } /* GLfloat = float */

		[Export ("end", ArgumentSemantic.Assign)]
		float End { get; set; } /* GLfloat = float */

		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKEffectProperty))]
	interface GLKEffectPropertyLight {
		[Export ("position", ArgumentSemantic.Assign)]
		Vector4 Position { [Align (16)] get; set; }

		[Export ("ambientColor", ArgumentSemantic.Assign)]
		Vector4 AmbientColor { [Align (16)] get; set; }

		[Export ("diffuseColor", ArgumentSemantic.Assign)]
		Vector4 DiffuseColor { [Align (16)] get; set; }

		[Export ("specularColor", ArgumentSemantic.Assign)]
		Vector4 SpecularColor { [Align (16)] get; set; }

		[Export ("spotDirection", ArgumentSemantic.Assign)]
		Vector3 SpotDirection { get; set; }

		[Export ("spotExponent", ArgumentSemantic.Assign)]
		float SpotExponent { get; set; } /* GLfloat = float */

		[Export ("spotCutoff", ArgumentSemantic.Assign)]
		float SpotCutoff { get; set; } /* GLfloat = float */

		[Export ("constantAttenuation", ArgumentSemantic.Assign)]
		float ConstantAttenuation { get; set; } /* GLfloat = float */

		[Export ("linearAttenuation", ArgumentSemantic.Assign)]
		float LinearAttenuation { get; set; } /* GLfloat = float */

		[Export ("quadraticAttenuation", ArgumentSemantic.Assign)]
		float QuadraticAttenuation { get; set; } /* GLfloat = float */

		[NullAllowed] // by default this property is null
		[Export ("transform", ArgumentSemantic.Retain)]
		GLKEffectPropertyTransform Transform { get; set; }

		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { get; set; }

	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKEffectProperty))]
	interface GLKEffectPropertyMaterial {
		[Export ("diffuseColor", ArgumentSemantic.Assign)]
		Vector4 DiffuseColor { [Align (16)] get; set; }

		[Export ("specularColor", ArgumentSemantic.Assign)]
		Vector4 SpecularColor { [Align (16)] get; set; }

		[Export ("emissiveColor", ArgumentSemantic.Assign)]
		Vector4 EmissiveColor { [Align (16)] get; set; }

		[Export ("shininess", ArgumentSemantic.Assign)]
		float Shininess { get; set; } /* GLfloat = float */

		[Export ("ambientColor", ArgumentSemantic.Assign)]
		Vector4 AmbientColor { [Align (16)] get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKEffectProperty))]
	interface GLKEffectPropertyTexture {
		[Export ("target", ArgumentSemantic.Assign)]
		GLKTextureTarget Target { get; set; }

		[Export ("envMode", ArgumentSemantic.Assign)]
		GLKTextureEnvMode EnvMode { get; set; }

		[Export ("enabled", ArgumentSemantic.Assign)]
		bool Enabled { get; set; }

		[Export ("name", ArgumentSemantic.Assign)]
		uint GLName { get; set; } /* GLuint = uint32_t */

	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKEffectProperty))]
	interface GLKEffectPropertyTransform {
		[Export ("normalMatrix")]
		Matrix3 NormalMatrix { get; }

		[Export ("modelviewMatrix", ArgumentSemantic.Assign)]
		Matrix4 ModelViewMatrix { [Align (16)] get; set; }

		[Export ("projectionMatrix", ArgumentSemantic.Assign)]
		Matrix4 ProjectionMatrix { [Align (16)] get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // - (nullable instancetype)init NS_UNAVAILABLE;
	interface GLKMesh {
		[Export ("initWithMesh:error:")]
		NativeHandle Constructor (MDLMesh mesh, out NSError error);

		// generator does not like `out []` -> https://trello.com/c/sZYNalbB/524-generator-support-out
		[Internal] // there's another, manual, public API exposed
		[Static]
		[Export ("newMeshesFromAsset:sourceMeshes:error:")]
		[return: NullAllowed]
		GLKMesh [] FromAsset (MDLAsset asset, out NSArray sourceMeshes, out NSError error);

		[Export ("vertexCount")]
		nuint VertexCount { get; }

		[Export ("vertexBuffers")]
		GLKMeshBuffer [] VertexBuffers { get; }

		[Export ("vertexDescriptor")]
		MDLVertexDescriptor VertexDescriptor { get; }

		[Export ("submeshes")]
		GLKSubmesh [] Submeshes { get; }

		[Export ("name")]
		string Name { get; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GLKMeshBuffer : MDLMeshBuffer {
		[Export ("glBufferName")]
		uint GlBufferName { get; }

		[Export ("offset")]
		nuint Offset { get; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface GLKMeshBufferAllocator : MDLMeshBufferAllocator {
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GLKNamedEffect {
		[Abstract]
		[Export ("prepareToDraw")]
		void PrepareToDraw ();
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (GLKBaseEffect))]
	interface GLKReflectionMapEffect : GLKNamedEffect {
		[Export ("textureCubeMap")]
		GLKEffectPropertyTexture TextureCubeMap { get; }

		[Export ("matrix", ArgumentSemantic.Assign)]
		Matrix3 Matrix { get; set; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	interface GLKSkyboxEffect : GLKNamedEffect {
		[Export ("center", ArgumentSemantic.Assign)]
		Vector3 Center { get; set; }

		[Export ("xSize", ArgumentSemantic.Assign)]
		float XSize { get; set; } /* GLfloat = float */

		[Export ("ySize", ArgumentSemantic.Assign)]
		float YSize { get; set; } /* GLfloat = float */

		[Export ("zSize", ArgumentSemantic.Assign)]
		float ZSize { get; set; } /* GLfloat = float */

		[Export ("textureCubeMap")]
		GLKEffectPropertyTexture TextureCubeMap { get; }

		[Export ("transform")]
		GLKEffectPropertyTransform Transform { get; }

		[NullAllowed] // by default this property is null
		[Export ("label", ArgumentSemantic.Copy)]
		[DisableZeroCopy]
		string Label { get; set; }

		[Export ("draw")]
		void Draw ();
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // (nullable instancetype)init NS_UNAVAILABLE;
	interface GLKSubmesh {
		// Problematic, OpenTK has this define in 3 namespaces in:
		// OpenTK.Graphics.ES11.DataType
		// OpenTK.Graphics.ES20.DataType
		// OpenTK.Graphics.ES30.DataType
		[Export ("type")]
		uint Type { get; }

		//  Problematic, OpenTK has this define in 3 namespaces in:
		// OpenTK.Graphics.ES11.BeginMode
		// OpenTK.Graphics.ES20.BeginMode
		// OpenTK.Graphics.ES30.BeginMode
		[Export ("mode")]
		uint Mode { get; }

		[Export ("elementCount")]
		int ElementCount { get; }

		[Export ("elementBuffer")]
		GLKMeshBuffer ElementBuffer { get; }

		[NullAllowed, Export ("mesh", ArgumentSemantic.Weak)]
		GLKMesh Mesh { get; }

		[Export ("name")]
		string Name { get; }
	}

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	interface GLKTextureInfo : NSCopying {
		[Export ("width")]
		int Width { get; } /* GLuint = uint32_t */

		[Export ("height")]
		int Height { get; } /* GLuint = uint32_t */

		[Export ("alphaState")]
		GLKTextureInfoAlphaState AlphaState { get; }

		[Export ("textureOrigin")]
		GLKTextureInfoOrigin TextureOrigin { get; }

		[Export ("containsMipmaps")]
		bool ContainsMipmaps { get; }

		[Export ("name")]
		uint Name { get; } /* GLuint = uint32_t */

		[Export ("target")]
		GLKTextureTarget Target { get; }

		[Export ("mimapLevelCount")]
		uint MimapLevelCount { get; }

		[Export ("arrayLength")]
		uint ArrayLength { get; }

		[Export ("depth")]
		uint Depth { get; }
	}

	delegate void GLKTextureLoaderCallback (GLKTextureInfo textureInfo, NSError error);

	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	interface GLKTextureLoader {
		[Static]
		[Export ("textureWithContentsOfFile:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo FromFile (string path, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("textureWithContentsOfURL:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo FromUrl (NSUrl url, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("textureWithContentsOfData:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo FromData (NSData data, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("textureWithCGImage:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo FromImage (CGImage cgImage, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("cubeMapWithContentsOfFiles:options:error:"), Internal]
		[return: NullAllowed]
		GLKTextureInfo CubeMapFromFiles (NSArray paths, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("cubeMapWithContentsOfFile:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo CubeMapFromFile (string path, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("cubeMapWithContentsOfURL:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo CubeMapFromUrl (NSUrl url, [NullAllowed] NSDictionary textureOperations, out NSError error);

		[Static]
		[Export ("textureWithName:scaleFactor:bundle:options:error:")]
		[return: NullAllowed]
		GLKTextureInfo FromName (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary<NSString, NSNumber> options, out NSError outError);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithShareContext:")]
		NativeHandle Constructor (NSOpenGLContext context);

		[NoMac]
		[Export ("initWithSharegroup:")]
		NativeHandle Constructor (EAGLSharegroup sharegroup);

		[Export ("textureWithContentsOfFile:options:queue:completionHandler:")]
		[Async]
		void BeginTextureLoad (string file, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("textureWithContentsOfURL:options:queue:completionHandler:")]
		[Async]
		void BeginTextureLoad (NSUrl filePath, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("textureWithContentsOfData:options:queue:completionHandler:")]
		[Async]
		void BeginTextureLoad (NSData data, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("textureWithCGImage:options:queue:completionHandler:")]
		[Async]
		void BeginTextureLoad (CGImage image, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("cubeMapWithContentsOfFiles:options:queue:completionHandler:"), Internal]
		[Async]
		void BeginLoadCubeMap (NSArray filePaths, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("cubeMapWithContentsOfFile:options:queue:completionHandler:")]
		[Async]
		void BeginLoadCubeMap (string fileName, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("cubeMapWithContentsOfURL:options:queue:completionHandler:")]
		[Async]
		void BeginLoadCubeMap (NSUrl filePath, [NullAllowed] NSDictionary textureOperations, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback onComplete);

		[Export ("textureWithName:scaleFactor:bundle:options:queue:completionHandler:")]
		[Async]
		void BeginTextureLoad (string name, nfloat scaleFactor, [NullAllowed] NSBundle bundle, [NullAllowed] NSDictionary<NSString, NSNumber> options, [NullAllowed] DispatchQueue queue, GLKTextureLoaderCallback block);

		[Field ("GLKTextureLoaderApplyPremultiplication")]
		NSString ApplyPremultiplication { get; }

		[Field ("GLKTextureLoaderGenerateMipmaps")]
		NSString GenerateMipmaps { get; }

		[Field ("GLKTextureLoaderOriginBottomLeft")]
		NSString OriginBottomLeft { get; }

		[Field ("GLKTextureLoaderGrayscaleAsAlpha")]
		NSString GrayscaleAsAlpha { get; }

		[Field ("GLKTextureLoaderSRGB")]
		NSString SRGB { get; }

		[Field ("GLKTextureLoaderErrorDomain")]
		NSString ErrorDomain { get; }

		[Field ("GLKTextureLoaderErrorKey")]
		NSString ErrorKey { get; }

		[Field ("GLKTextureLoaderGLErrorKey")]
		NSString GLErrorKey { get; }
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (UIView), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (GLKViewDelegate) })]
	interface GLKView {
		[Export ("initWithFrame:")]
		NativeHandle Constructor (CGRect frame);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GLKViewDelegate Delegate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("context", ArgumentSemantic.Retain)]
		EAGLContext Context { get; set; }

		[Export ("drawableWidth")]
		nint DrawableWidth { get; }

		[Export ("drawableHeight")]
		nint DrawableHeight { get; }

		[Export ("drawableColorFormat")]
		GLKViewDrawableColorFormat DrawableColorFormat { get; set; }

		[Export ("drawableDepthFormat")]
		GLKViewDrawableDepthFormat DrawableDepthFormat { get; set; }

		[Export ("drawableStencilFormat")]
		GLKViewDrawableStencilFormat DrawableStencilFormat { get; set; }

		[Export ("drawableMultisample")]
		GLKViewDrawableMultisample DrawableMultisample { get; set; }

		[Export ("enableSetNeedsDisplay")]
		bool EnableSetNeedsDisplay { get; set; }

		[Export ("initWithFrame:context:")]
		NativeHandle Constructor (CGRect frame, EAGLContext context);

		[Export ("bindDrawable")]
		void BindDrawable ();

		[Export ("snapshot")]
		UIImage Snapshot ();

		[Export ("display")]
		void Display ();

		[Export ("deleteDrawable")]
		void DeleteDrawable ();
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GLKViewDelegate {
		[Abstract]
		[Export ("glkView:drawInRect:"), EventArgs ("GLKViewDraw")]
		void DrawInRect (GLKView view, CGRect rect);
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (UIViewController))]
	interface GLKViewController : GLKViewDelegate {
		[Export ("initWithNibName:bundle:")]
		[PostGet ("NibBundle")]
		NativeHandle Constructor ([NullAllowed] string nibName, [NullAllowed] NSBundle bundle);

		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[Export ("framesPerSecond")]
		nint FramesPerSecond { get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Export ("framesDisplayed")]
		nint FramesDisplayed { get; }

		[Export ("timeSinceFirstResume")]
		double TimeSinceFirstResume { get; }

		[Export ("timeSinceLastResume")]
		double TimeSinceLastResume { get; }

		[Export ("timeSinceLastUpdate")]
		double TimeSinceLastUpdate { get; }

		[Export ("timeSinceLastDraw")]
		double TimeSinceLastDraw { get; }

		[Export ("pauseOnWillResignActive")]
		bool PauseOnWillResignActive { get; set; }

		[Export ("resumeOnDidBecomeActive")]
		bool ResumeOnDidBecomeActive { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		GLKViewControllerDelegate Delegate { get; set; }

		// Pseudo-documented, if the user overrides it, call this instead of the delegate method
		[Export ("update")]
		void Update ();
	}

	[NoMac]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface GLKViewControllerDelegate {
		[Abstract]
		[Export ("glkViewControllerUpdate:")]
		void Update (GLKViewController controller);

		[Export ("glkViewController:willPause:")]
		void WillPause (GLKViewController controller, bool pause);
	}
}
