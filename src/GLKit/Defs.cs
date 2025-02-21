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
		/// <summary>To be added.</summary>
		Position,
		/// <summary>To be added.</summary>
		Normal,
		/// <summary>To be added.</summary>
		Color,
		/// <summary>To be added.</summary>
		TexCoord0,
		/// <summary>To be added.</summary>
		TexCoord1,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyLight.h
	public enum GLKLightingType {
		/// <summary>To be added.</summary>
		PerVertex,
		/// <summary>To be added.</summary>
		PerPixel,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyTexture.h
	public enum GLKTextureEnvMode {
		/// <summary>To be added.</summary>
		Replace,
		/// <summary>To be added.</summary>
		Modulate,
		/// <summary>To be added.</summary>
		Decal,
	}

	// GLenum (32 bits on 64 bit hardware) -> GLKEffectPropertyTexture.h
	public enum GLKTextureTarget {
		/// <summary>To be added.</summary>
		Texture2D = 0x0DE1,    // GL_TEXTURE_2D
		/// <summary>To be added.</summary>
		CubeMap = 0x8513, // GL_TEXTURE_CUBE_MAP
		/// <summary>To be added.</summary>
		TargetCt = 2,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKEffectPropertyFog.h
	public enum GLKFogMode {
		/// <summary>The fog is calculated using Math.Exp(-density * distance).</summary>
		Exp = 0,
		/// <summary>The fog is calculated using Math.Exp(-(density * distance) ^2).</summary>
		Exp2,
		/// <summary>The fog is calculated using (end - distance) / (end - start).</summary>
		Linear,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableColorFormat {
		/// <summary>To be added.</summary>
		RGBA8888 = 0,
		/// <summary>To be added.</summary>
		RGB565,
		/// <summary>To be added.</summary>
		SRGBA8888,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableDepthFormat {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		Format16,
		/// <summary>To be added.</summary>
		Format24,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableStencilFormat {
		/// <summary>To be added.</summary>
		FormatNone,
		/// <summary>To be added.</summary>
		Format8,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKView.h
	public enum GLKViewDrawableMultisample {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		Sample4x,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKTextureLoader.h
	public enum GLKTextureInfoAlphaState {
		/// <summary>To be added.</summary>
		None,
		/// <summary>To be added.</summary>
		NonPremultiplied,
		/// <summary>To be added.</summary>
		Premultiplied,
	}

	// GLint (32 bits on 64 bit hardware) -> GLKTextureLoader.h
	public enum GLKTextureInfoOrigin {
		/// <summary>To be added.</summary>
		Unknown = 0,
		/// <summary>To be added.</summary>
		TopLeft,
		/// <summary>To be added.</summary>
		BottomLeft,
	}

	// GLuint (we'll keep `int` for compatibility) -> GLKTextureLoader.h
	public enum GLKTextureLoaderError {
		/// <summary>To be added.</summary>
		FileOrURLNotFound = 0,
		/// <summary>To be added.</summary>
		InvalidNSData = 1,
		/// <summary>To be added.</summary>
		InvalidCGImage = 2,
		/// <summary>To be added.</summary>
		UnknownPathType = 3,
		/// <summary>To be added.</summary>
		UnknownFileType = 4,
		/// <summary>To be added.</summary>
		PVRAtlasUnsupported = 5,
		/// <summary>To be added.</summary>
		CubeMapInvalidNumFiles = 6,
		/// <summary>To be added.</summary>
		CompressedTextureUpload = 7,
		/// <summary>To be added.</summary>
		UncompressedTextureUpload = 8,
		/// <summary>To be added.</summary>
		UnsupportedCubeMapDimensions = 9,
		/// <summary>To be added.</summary>
		UnsupportedBitDepth = 10,
		/// <summary>To be added.</summary>
		UnsupportedPVRFormat = 11,
		/// <summary>To be added.</summary>
		DataPreprocessingFailure = 12,
		/// <summary>To be added.</summary>
		MipmapUnsupported = 13,
		/// <summary>To be added.</summary>
		UnsupportedOrientation = 14,
		/// <summary>To be added.</summary>
		ReorientationFailure = 15,
		/// <summary>To be added.</summary>
		AlphaPremultiplicationFailure = 16,
		/// <summary>To be added.</summary>
		InvalidEAGLContext = 17,
		/// <summary>To be added.</summary>
		IncompatibleFormatSRGB = 18,
		/// <summary>To be added.</summary>
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
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public uint Type;
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public uint Size;
#if XAMCORE_5_0
		byte normalized;
		public bool Normalized {
			get => normalized != 0;
			set => normalized = value.AsByte ();
		}
#else
		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MarshalAs (UnmanagedType.I1)]
		public bool Normalized;
#endif

#if !COREBUILD
		[DllImport (Constants.GLKitLibrary, EntryPoint = "GLKVertexAttributeParametersFromModelIO")]
#if XAMCORE_5_0
		extern static GLKVertexAttributeParameters FromVertexFormat_ (nuint vertexFormat);
#else
		extern static GLKVertexAttributeParametersInternal FromVertexFormat_ (nuint vertexFormat);
#endif

		public static GLKVertexAttributeParameters FromVertexFormat (MDLVertexFormat vertexFormat)
		{
#if XAMCORE_5_0
			return FromVertexFormat_ ((nuint) (ulong) vertexFormat);
#else
			var tmp = FromVertexFormat_ ((nuint) (ulong) vertexFormat);
			var rv = new GLKVertexAttributeParameters ();
			rv.Type = tmp.Type;
			rv.Size = tmp.Size;
			rv.Normalized = tmp.Normalized != 0;
			return rv;
#endif
		}
#endif
	}

#if !XAMCORE_5_0
	[StructLayout (LayoutKind.Sequential)]
	struct GLKVertexAttributeParametersInternal {
		public uint Type;
		public uint Size;
		public byte Normalized;
	}
#endif // !XAMCORE_5_0
}
