using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;
using Metal;

using Vector4 = global::OpenTK.Vector4;
using OpenTK;

namespace MetalPerformanceShaders {

	// uses NSInteger
	[Mac (10, 13)]
	public struct MPSOffset {
		public nint X;
		public nint Y;
		public nint Z;
	}

	// really use double, not CGFloat
	[Mac (10, 13)]
	public struct MPSOrigin {
		public double X;
		public double Y;
		public double Z;
	}

	// really use double, not CGFloat
	[Mac (10, 13)]
	public struct MPSSize {
		public double Width;
		public double Height;
		public double Depth;
	}

	// uses NSUInteger
	[iOS (13,0), TV (13,0), Mac (10,15)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	public struct MPSDimensionSlice {
		public nuint Start;
		public nuint Length;
	}

	[Mac (10, 13)]
	public struct MPSRegion {
		public MPSOrigin Origin;
		public MPSSize Size;
	}

	// really use double, not CGFloat
	[Mac (10, 13)]
	public struct MPSScaleTransform {
		public double ScaleX;
		public double ScaleY;
		public double TranslateX;
		public double TranslateY;
	}

	[iOS (11,3), TV (11,3), Mac (10,13,4)]
	public struct MPSImageCoordinate {
		public nuint X;
		public nuint Y;
		public nuint Channel;
	}

	[iOS (11,3), TV (11,3), Mac (10,13,4)]
	public struct MPSImageRegion {
		public MPSImageCoordinate Offset;
		public MPSImageCoordinate Size;
	}

	// MPSImageHistogram.h
	[Mac (10, 13)]
	[StructLayout (LayoutKind.Explicit)]
	public struct MPSImageHistogramInfo {
		[FieldOffset (0)]
		public nuint NumberOfHistogramEntries;
#if ARCH_64
		[FieldOffset (8)]
#else
		[FieldOffset (4)]
#endif
		public bool HistogramForAlpha;
		[FieldOffset (16)]
		public Vector4 MinPixelValue;
		[FieldOffset (32)]
		public Vector4 MaxPixelValue;
	}

	// MPSTypes.h
	// FIXME: public delegate IMTLTexture MPSCopyAllocator (MPSKernel filter, IMTLCommandBuffer commandBuffer, IMTLTexture sourceTexture);
	public delegate NSObject MPSCopyAllocator (MPSKernel filter, NSObject commandBuffer, NSObject sourceTexture);
	// https://trello.com/c/GqtNId1C/517-generator-our-block-delegates-needs-to-use-wrapper-for-protocols

	[TV (11, 0), Mac (10, 13), iOS (11, 0)]
	public struct MPSMatrixCopyOffsets {
		public uint SourceRowOffset;
		public uint SourceColumnOffset;
		public uint DestinationRowOffset;
		public uint DestinationColumnOffset;
	}

	[TV (11, 0), Mac (10, 13), iOS (11, 0)]
	public struct MPSImageReadWriteParams {
		public nuint FeatureChannelOffset;
		public nuint NumberOfFeatureChannelsToReadWrite;
	}

	[TV (11, 0), Mac (10, 13), iOS (11, 0)]
	public struct MPSImageKeypointRangeInfo {
		public nuint MaximumKeypoints;
		public float MinimumThresholdValue;
	}

	[TV (11, 3), iOS (11, 3), Mac (10, 13, 4)]
	public struct MPSStateTextureInfo {
		public nuint Width;
		public nuint Height;
		public nuint Depth;
		public nuint ArrayLength;

#pragma warning disable 0169 // Avoid warning when building core.dll and the unused reserved fields
		nuint _PixelFormat;
		nuint _TextureType;
		nuint _TextureUsage;

		//NSUInteger _reserved [4];
		nuint Reserved0;
		nuint Reserved1;
		nuint Reserved2;
		nuint Reserved3;
#pragma warning restore 0169
#if !COREBUILD
		public MTLPixelFormat PixelFormat {
			get => (MTLPixelFormat) (ulong) _PixelFormat;
			set => _PixelFormat = (nuint) (ulong) value;
		}

		public MTLTextureType TextureType {
			get => (MTLTextureType) (ulong) _TextureType;
			set => _TextureType = (nuint) (ulong) value;
		}

		public MTLTextureUsage TextureUsage {
			get => (MTLTextureUsage) (ulong) _TextureUsage;
			set => _TextureUsage = (nuint) (ulong) value;
		}
#endif
	}

	[TV (12, 0), Mac (10, 14), iOS (12, 0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MPSAxisAlignedBoundingBox {
		public Vector3 Min;
		public Vector3 Max;
	}

	public static class MPSConstants
	{
		public const uint FunctionConstantIndex = 127;
		public const uint BatchSizeIndex = 126;
		public const uint UserConstantIndex = 125;
		public const uint NDArrayConstantIndex = 124;
		// Maximum number of textures depends on the platform
		// MaxTextures = 128 or 32,
	}

	[iOS (11,2), TV (11,2), Mac (10,13,2)]
	[Introduced (PlatformName.MacCatalyst, 13, 0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct MPSMatrixOffset
	{
		public uint RowOffset;
		public uint ColumnOffset;
	}
}
