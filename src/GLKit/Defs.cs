//
// GLKit/Defs.cs: basic definitions for GLKit
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2011-2014 Xamarin, Inc.
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
using System.Runtime.InteropServices;

using Foundation;

using ModelIO;

using ObjCRuntime;

#nullable enable

namespace GLKit {

	// GLint (32 bits on 64 bit hardware) -> GLKEffects.h
	public enum GLKVertexAttrib {
		Position, Normal, Color, TexCoord0, TexCoord1
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyLight.h
	public enum GLKLightingType {
		PerVertex,
		PerPixel
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyTexture.h
	public enum GLKTextureEnvMode {
		Replace, Modulate, Decal
	}

	// GLenum (32 bits on 64 bit hardware) -> GLKEffectPropertyTexture.h
	public enum GLKTextureTarget {
		Texture2D = 0x0DE1,    // GL_TEXTURE_2D
		CubeMap = 0x8513, // GL_TEXTURE_CUBE_MAP
		TargetCt = 2
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyFog.h
	public enum GLKFogMode {
		Exp = 0,
		Exp2,
		Linear
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableColorFormat {
		RGBA8888 = 0,
		RGB565,
		SRGBA8888
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableDepthFormat {
		None, Format16, Format24,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableStencilFormat {
		FormatNone, Format8
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableMultisample {
		None, Sample4x
	}

	// GLint (32 bits on 64 bit hardware) -> GLKTextureLoader.h
	public enum GLKTextureInfoAlphaState {
		None, NonPremultiplied, Premultiplied
	}

	// GLint (32 bits on 64 bit hardware) -> GLKTextureLoader.h
	public enum GLKTextureInfoOrigin {
		Unknown = 0,
		TopLeft,
		BottomLeft
	}

	// GLuint (we'll keep `int` for compatibility) -> GLKTextureLoader.h
	public enum GLKTextureLoaderError {
		FileOrURLNotFound = 0,
		InvalidNSData = 1,
		InvalidCGImage = 2,
		UnknownPathType = 3,
		UnknownFileType = 4,
		PVRAtlasUnsupported = 5,
		CubeMapInvalidNumFiles = 6,
		CompressedTextureUpload = 7,
		UncompressedTextureUpload = 8,
		UnsupportedCubeMapDimensions = 9,
		UnsupportedBitDepth = 10,
		UnsupportedPVRFormat = 11,
		DataPreprocessingFailure = 12,
		MipmapUnsupported = 13,
		UnsupportedOrientation = 14,
		ReorientationFailure = 15,
		AlphaPremultiplicationFailure = 16,
		InvalidEAGLContext = 17,
		IncompatibleFormatSRGB = 18,
		UnsupportedTextureTarget = 19,
	}

	// glVertexAttribPointer structure values, again, problems with definitions being in different namespaces
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("tvos")]
	[ObsoletedOSPlatform ("tvos12.0", "Use 'Metal' instead.")]
	[ObsoletedOSPlatform ("macos10.14", "Use 'Metal' instead.")]
	[ObsoletedOSPlatform ("ios12.0", "Use 'Metal' instead.")]
#else
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'Metal' instead.")]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'Metal' instead.")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct GLKVertexAttributeParameters {
		public uint Type;
		public uint Size;
		[MarshalAs (UnmanagedType.I1)]
		public bool Normalized;

#if !COREBUILD
		[DllImport (Constants.GLKitLibrary, EntryPoint = "GLKVertexAttributeParametersFromModelIO")]
		extern static GLKVertexAttributeParameters FromVertexFormat_ (nuint vertexFormat);

		public static GLKVertexAttributeParameters FromVertexFormat (MDLVertexFormat vertexFormat)
		{
			return FromVertexFormat_ ((nuint) (ulong) vertexFormat);
		}
#endif
	}
}
