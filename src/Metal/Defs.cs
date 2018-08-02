#if XAMCORE_2_0 || !MONOMAC
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

namespace Metal {

	public struct MTLOrigin {
		public nint X, Y, Z;
		public MTLOrigin (nint x, nint y, nint z){
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString ()
		{
			return String.Format ("({0},{1},{2})", X, Y, Z);
		}
	}

	public struct MTLSize {
		public nint Width, Height, Depth;
		
		public MTLSize (nint width, nint height, nint depth){
			Width = width;
			Height = height;
			Depth = depth;
		}
	}

#if !COREBUILD
	public static class MTLVertexFormatExtensions {

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		[DllImport (Constants.MetalKitLibrary)]
		static extern /* MDLVertexFormat */ nuint MTKModelIOVertexFormatFromMetal (/* MTLVertexFormat */ nuint modelIODescriptor);

		[iOS (9,0)][Mac (10,11, onlyOn64 : true)]
		public static MDLVertexFormat ToModelVertexFormat (this MTLVertexFormat vertexFormat)
		{
			nuint mdlVertexFormat = MTKModelIOVertexFormatFromMetal ((nuint)(ulong)vertexFormat);
			return (MDLVertexFormat)(ulong)mdlVertexFormat;
		}
	}
#endif

	public struct MTLScissorRect {
		public nuint X, Y, Width, Height;

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

	public struct MTLViewport {
		public double OriginX, OriginY, Width, Height, ZNear, ZFar;
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

	[StructLayout (LayoutKind.Sequential)]
	public struct MTLSamplePosition
	{
		public float X;

		public float Y;

		public MTLSamplePosition (float x, float y)
		{
			this.X = x;
			this.Y = y;
		}
	}


	public struct MTLClearColor {
		public double Red, Green, Blue, Alpha;

		public MTLClearColor (double red, double green, double blue, double alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}
	}

	public struct MTLRegion {
		public MTLOrigin Origin;
		public MTLSize   Size;

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

	public struct MTLDispatchThreadgroupsIndirectArguments {
		public uint ThreadGroupsPerGrid1;
		public uint ThreadGroupsPerGrid2;
		public uint ThreadGroupsPerGrid3;
	}

	[NoMac]
	[iOS (10,0), TV (10,0)]
	public struct MTLStageInRegionIndirectArguments
	{
		public uint StageInOrigin1;
		public uint StageInOrigin2;
		public uint StageInOrigin3;

		public uint StageInSize1;
		public uint StageInSize2;
		public uint StageInSize3;		
	}

	public struct MTLDrawPrimitivesIndirectArguments {
		public uint VertexCount;
		public uint InstanceCount;
		public uint VertexStart;
		public uint BaseInstance;
	}

	public struct MTLDrawIndexedPrimitivesIndirectArguments {
		public uint IndexCount;
		public uint InstanceCount;
		public uint IndexStart;
		public uint BaseVertex;
		public uint BaseInstance;
	}
	
	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLSizeAndAlign
	{
		public nuint Size;
		public nuint Align;
		
		public MTLSizeAndAlign (nuint size, nuint align)
		{
			Size = size;
			Align = align;
		}
	
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLDrawPatchIndirectArguments
	{
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

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLQuadTessellationFactorsHalf
	{
		public ushort[] EdgeTessellationFactor;
		public ushort[] InsideTessellationFactor;
		
		public MTLQuadTessellationFactorsHalf (ushort[] edgeTessellationFactor, ushort[] insideTessellationFactor)
		{
			EdgeTessellationFactor = edgeTessellationFactor;
			InsideTessellationFactor = insideTessellationFactor;
		}
	
	}

	[iOS (10,0), TV (10,0), NoWatch, Mac (10,12)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MTLTriangleTessellationFactorsHalf
	{
		public ushort[] EdgeTessellationFactor;
		public ushort InsideTessellationFactor;
		
		public MTLTriangleTessellationFactorsHalf (ushort[] edgeTessellationFactor, ushort insideTessellationFactor)
		{
			EdgeTessellationFactor = edgeTessellationFactor;
			InsideTessellationFactor = insideTessellationFactor;
		}
	}

#if COREBUILD
	// IMTLCommandBuffer and IMTLTexture visibility are needed for MPSCopyAllocator - but they are generated later

	public partial interface IMTLCommandBuffer {
	}

	public partial interface IMTLTexture {
	}
#endif
}
#endif
