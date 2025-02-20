//
// API for the Metal framework
//
// Authors:
//   Miguel de Icaza
//
// Copyrigh 2014, Xamarin Inc.
//
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Foundation;
using ModelIO;
using ObjCRuntime;

#nullable enable

namespace Metal {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLOrigin {
		public nint X;
		public nint Y;
		public nint Z;

		public MTLOrigin (nint x, nint y, nint z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString ()
		{
			return String.Format ("({0},{1},{2})", X, Y, Z);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLSize {
		public nint Width;
		public nint Height;
		public nint Depth;

		public MTLSize (nint width, nint height, nint depth)
		{
			Width = width;
			Height = height;
			Depth = depth;
		}
	}

#if !COREBUILD
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public static class MTLVertexFormatExtensions {

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MDLVertexFormat */ nuint MTKModelIOVertexFormatFromMetal (/* MTLVertexFormat */ nuint modelIODescriptor);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static MDLVertexFormat ToModelVertexFormat (this MTLVertexFormat vertexFormat)
		{
			nuint mdlVertexFormat = MTKModelIOVertexFormatFromMetal ((nuint) (ulong) vertexFormat);
			return (MDLVertexFormat) (ulong) mdlVertexFormat;
		}
	}
#endif

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLScissorRect {
		public nuint X;
		public nuint Y;
		public nuint Width;
		public nuint Height;

		public MTLScissorRect (nuint x, nuint y, nuint width, nuint height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public override string ToString ()
		{
			return String.Format ("({0},{1},{2},{3}", X, Y, Width, Height);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLViewport {
		public double OriginX;
		public double OriginY;
		public double Width;
		public double Height;
		public double ZNear;
		public double ZFar;

		public MTLViewport (double originX, double originY, double width, double height, double znear, double zfar)
		{
			OriginX = originX;
			OriginY = originY;
			Width = width;
			Height = height;
			ZNear = znear;
			ZFar = zfar;
		}

		public override string ToString ()
		{
			return String.Format ("({0},{1},{2},{3} Znear={4} Zfar={5})", OriginX, OriginY, Width, Height, ZNear, ZFar);
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLSamplePosition {
		public float X;

		public float Y;

		public MTLSamplePosition (float x, float y)
		{
			this.X = x;
			this.Y = y;
		}
	}


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLClearColor {
		public double Red;
		public double Green;
		public double Blue;
		public double Alpha;

		public MTLClearColor (double red, double green, double blue, double alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLRegion {
		public MTLOrigin Origin;
		public MTLSize Size;

		public MTLRegion (MTLOrigin origin, MTLSize size)
		{
			Origin = origin;
			Size = size;
		}

		public static MTLRegion Create1D (nuint x, nuint width)
		{
			return Create1D ((nint) x, (nint) width);
		}

		public static MTLRegion Create1D (nint x, nint width)
		{
			var region = new MTLRegion ();
			region.Origin.X = x;
			region.Origin.Y = 0;
			region.Origin.Z = 0;
			region.Size.Width = width;
			region.Size.Height = 1;
			region.Size.Depth = 1;
			return region;
		}

		public static MTLRegion Create2D (nuint x, nuint y, nuint width, nuint height)
		{
			return Create2D ((nint) x, (nint) y, (nint) width, (nint) height);
		}

		public static MTLRegion Create2D (nint x, nint y, nint width, nint height)
		{
			var region = new MTLRegion ();
			region.Origin.X = x;
			region.Origin.Y = y;
			region.Origin.Z = 0;
			region.Size.Width = width;
			region.Size.Height = height;
			region.Size.Depth = 1;
			return region;
		}

		public static MTLRegion Create3D (nuint x, nuint y, nuint z, nuint width, nuint height, nuint depth)
		{
			return Create3D ((nint) x, (nint) y, (nint) z, (nint) width, (nint) height, (nint) depth);
		}

		public static MTLRegion Create3D (nint x, nint y, nint z, nint width, nint height, nint depth)
		{
			var region = new MTLRegion ();
			region.Origin.X = x;
			region.Origin.Y = y;
			region.Origin.Z = z;
			region.Size.Width = width;
			region.Size.Height = height;
			region.Size.Depth = depth;
			return region;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	[StructLayout (LayoutKind.Explicit)]
	public struct MTLClearValue {
		[FieldOffset (0)]
		public MTLClearColor Color;
		[FieldOffset (0)]
		public double Depth;
		[FieldOffset (0)]
		public ulong Stencil;

		public MTLClearValue (MTLClearColor color)
		{
			Depth = 0;
			Stencil = 0;
			Color = color;
		}

		public MTLClearValue (double depth)
		{
			Color.Red = 0;
			Stencil = 0;

			Depth = depth;
			Color.Green = 0;
			Color.Blue = 0;
			Color.Alpha = 0;
		}

		public MTLClearValue (ulong stencil)
		{
			Color.Red = 0;
			Depth = 0;

			Stencil = stencil;
			Color.Green = 0;
			Color.Blue = 0;
			Color.Alpha = 0;
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLDispatchThreadgroupsIndirectArguments {
		public uint ThreadGroupsPerGrid1;
		public uint ThreadGroupsPerGrid2;
		public uint ThreadGroupsPerGrid3;
	}

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLStageInRegionIndirectArguments {
		public uint StageInOrigin1;
		public uint StageInOrigin2;
		public uint StageInOrigin3;

		public uint StageInSize1;
		public uint StageInSize2;
		public uint StageInSize3;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLDrawPrimitivesIndirectArguments {
		public uint VertexCount;
		public uint InstanceCount;
		public uint VertexStart;
		public uint BaseInstance;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public struct MTLDrawIndexedPrimitivesIndirectArguments {
		public uint IndexCount;
		public uint InstanceCount;
		public uint IndexStart;
		public uint BaseVertex;
		public uint BaseInstance;
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLSizeAndAlign {
		public nuint Size;
		public nuint Align;

		public MTLSizeAndAlign (nuint size, nuint align)
		{
			Size = size;
			Align = align;
		}

	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLDrawPatchIndirectArguments {
		public uint PatchCount;
		public uint InstanceCount;
		public uint PatchStart;
		public uint BaseInstance;

		public MTLDrawPatchIndirectArguments (uint pathCount, uint instanceCount, uint patchStart, uint baseInstance)
		{
			PatchCount = pathCount;
			InstanceCount = instanceCount;
			PatchStart = patchStart;
			BaseInstance = baseInstance;
		}

	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLQuadTessellationFactorsHalf {
#if XAMCORE_5_0
		ushort edgeTessellationFactor0;
		ushort edgeTessellationFactor1;
		ushort edgeTessellationFactor2;
		ushort edgeTessellationFactor3;
		ushort insideTessellationFactor0;
		ushort insideTessellationFactor1;

		public ushort [] EdgeTessellationFactor {
			get => new ushort [] { edgeTessellationFactor0, edgeTessellationFactor1, edgeTessellationFactor2, edgeTessellationFactor3 };
			set {
				if (value.Length > 4)
					throw new ArgumentOutOfRangeException ($"The '{nameof (value)}' array length can't be greater than 4.");

				edgeTessellationFactor0 = value.Length >= 1 ? value [0] : 0;
				edgeTessellationFactor1 = value.Length >= 2 ? value [1] : 0;
				edgeTessellationFactor2 = value.Length >= 3 ? value [2] : 0;
				edgeTessellationFactor3 = value.Length >= 4 ? value [3] : 0;
			}
		}

		public ushort [] InsideTessellationFactor {
			get => new ushort [] { insideTessellationFactor0, insideTessellationFactor1 };
			set {
				if (value.Length > 2)
					throw new ArgumentOutOfRangeException ($"The '{nameof (value)}' array length can't be greater than 2.");

				insideTessellationFactor0 = value.Length >= 1 ? value [0] : 0;
				insideTessellationFactor1 = value.Length >= 2 ? value [1] : 0;
			}
		}
#else
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 4)]
		public ushort [] EdgeTessellationFactor;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 2)]
		public ushort [] InsideTessellationFactor;
#endif

		public MTLQuadTessellationFactorsHalf (ushort [] edgeTessellationFactor, ushort [] insideTessellationFactor)
		{
			if (edgeTessellationFactor.Length > 4)
				throw new ArgumentOutOfRangeException ($"The '{nameof (edgeTessellationFactor)}' array length can't be greater than 4.");

			if (insideTessellationFactor.Length > 2)
				throw new ArgumentOutOfRangeException ($"The '{nameof (insideTessellationFactor)}' array length can't be greater than 2.");
#if XAMCORE_5_0
			edgeTessellationFactor0 = edgeTessellationFactor.Length >= 1 ? edgeTessellationFactor [0] : 0;
			edgeTessellationFactor1 = edgeTessellationFactor.Length >= 2 ? edgeTessellationFactor [1] : 0;
			edgeTessellationFactor2 = edgeTessellationFactor.Length >= 3 ? edgeTessellationFactor [2] : 0;
			edgeTessellationFactor3 = edgeTessellationFactor.Length >= 4 ? edgeTessellationFactor [3] : 0;
			insideTessellationFactor0 = insideTessellationFactor.Length >= 1 ? insideTessellationFactor [0] : 0;
			insideTessellationFactor1 = insideTessellationFactor.Length >= 2 ? insideTessellationFactor [1] : 0;
#else
			EdgeTessellationFactor = edgeTessellationFactor;
			InsideTessellationFactor = insideTessellationFactor;
#endif
		}

	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("tvos")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("maccatalyst")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLTriangleTessellationFactorsHalf {
#if XAMCORE_5_0
		ushort edgeTessellationFactor0;
		ushort edgeTessellationFactor1;
		ushort edgeTessellationFactor2;

		public ushort [] EdgeTessellationFactor {
			get => new ushort [] { edgeTessellationFactor0, edgeTessellationFactor1, edgeTessellationFactor2 };
			set {
				if (value.Length > 3)
					throw new ArgumentOutOfRangeException ($"The '{nameof (value)}' array length can't be greater than 3.");
				edgeTessellationFactor0 = value.Length >= 1 ? value [0] : 0;
				edgeTessellationFactor1 = value.Length >= 2 ? value [1] : 0;
				edgeTessellationFactor2 = value.Length >= 3 ? value [2] : 0;
			}
		}
#else
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = 3)]
		public ushort [] EdgeTessellationFactor;
#endif
		public ushort InsideTessellationFactor;

		public MTLTriangleTessellationFactorsHalf (ushort [] edgeTessellationFactor, ushort insideTessellationFactor)
		{
			if (edgeTessellationFactor.Length > 3)
				throw new ArgumentOutOfRangeException ($"The '{nameof (edgeTessellationFactor)}' array length can't be greater than 3.");
#if XAMCORE_5_0
			edgeTessellationFactor0 = edgeTessellationFactor.Length >= 1 ? edgeTessellationFactor [0] : 0;
			edgeTessellationFactor1 = edgeTessellationFactor.Length >= 2 ? edgeTessellationFactor [1] : 0;
			edgeTessellationFactor2 = edgeTessellationFactor.Length >= 3 ? edgeTessellationFactor [2] : 0;
#else
			EdgeTessellationFactor = edgeTessellationFactor;
#endif
			InsideTessellationFactor = insideTessellationFactor;
		}
	}

#if COREBUILD
	// IMTLCommandBuffer and IMTLTexture visibility are needed for MPSCopyAllocator - but they are generated later

	public partial interface IMTLCommandBuffer {
	}

	public partial interface IMTLTexture {
	}
#endif // COREBUILD
#if MONOMAC
#if NET
	[SupportedOSPlatform ("macos")]
	[UnsupportedOSPlatform ("ios")]
	[UnsupportedOSPlatform ("tvos")]
#else
	[NoiOS]
	[NoTV]
#endif
	public struct MTLIndirectCommandBufferExecutionRange {
		public uint Location;
		public uint Length;

		public MTLIndirectCommandBufferExecutionRange (uint location, uint length)
		{
			Location = location;
			Length = length;
		}
	}
#endif // MONOMAC

#if NET
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("tvos13.0")]
	[SupportedOSPlatform ("maccatalyst")]
#else
	[iOS (13, 0)]
	[TV (13, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLTextureSwizzleChannels {
#if COREBUILD
		// keep size identical
		byte Red, Green, Blue, Alpha;
#else
		public MTLTextureSwizzle Red;

		public MTLTextureSwizzle Green;

		public MTLTextureSwizzle Blue;

		public MTLTextureSwizzle Alpha;
#endif
	}

#if IOS || MONOMAC || COREBUILD || TVOS

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos16.0")]
#else
	[Introduced (PlatformName.iOS, 13,0, PlatformArchitecture.All)]
	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Introduced (PlatformName.TvOS, 16, 0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLVertexAmplificationViewMapping {
		public uint ViewportArrayIndexOffset;

		public uint RenderTargetArrayIndexOffset;
	}

#if NET
	[SupportedOSPlatform ("ios13.0")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos17.0")]
#else
	[Introduced (PlatformName.iOS, 13,0, PlatformArchitecture.All)]
	[Introduced (PlatformName.MacCatalyst, 13, 4)]
	[Introduced (PlatformName.TvOS, 17,0)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLCoordinate2D {
		public float X;

		public float Y;
	}
#endif

#if NET
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios14.0")]
	[SupportedOSPlatform ("tvos16.1")]
#else
	[MacCatalyst (14, 0)]
	[iOS (14, 0)]
	[TV (16, 1)]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLAccelerationStructureSizes {
		public nuint AccelerationStructureSize;

		public nuint BuildScratchBufferSize;

		public nuint RefitScratchBufferSize;
	}

#if NET
	[SupportedOSPlatform ("ios16.0")]
	[SupportedOSPlatform ("maccatalyst16.0")]
	[SupportedOSPlatform ("macos13.0")]
	[SupportedOSPlatform ("tvos16.0")]
#else
	[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
	[NativeName ("MTLResourceID")]
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLResourceId {
		public ulong Impl;
	}

}
