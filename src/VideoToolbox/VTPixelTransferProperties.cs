// 
// VTPixelTransferProperties.cs: Strongly Typed dictionary for VTPixelTransferPropertyKeys 
//
// Authors: Alex Soto (alex.soto@xamarin.com)
//     
// Copyright 2015 Xamarin Inc.
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using CoreFoundation;
using ObjCRuntime;
using Foundation;
using CoreMedia;
using CoreVideo;
using AVFoundation;

// VTPixelTransferProperties are available in iOS 9 radar://22614931 https://trello.com/c/bTl6hRu9
namespace VideoToolbox {
	public partial class VTPixelTransferProperties : DictionaryContainer {

		public VTScalingMode ScalingMode { 
			get {
				var key = GetNSStringValue (VTPixelTransferPropertyKeys.ScalingMode);

				if (key == null)
					return VTScalingMode.Unset;
				if (key == VTPixelTransferPropertyKeys.ScalingMode_Normal)
					return VTScalingMode.Normal;
				if (key == VTPixelTransferPropertyKeys.ScalingMode_CropSourceToCleanAperture)
					return VTScalingMode.CropSourceToCleanAperture;
				if (key == VTPixelTransferPropertyKeys.ScalingMode_Letterbox)
					return VTScalingMode.Letterbox;
				if (key == VTPixelTransferPropertyKeys.ScalingMode_Trim)
					return VTScalingMode.Trim;
				return VTScalingMode.Unset; 
			}
			set {
				switch (value) {
				case VTScalingMode.Normal:
					SetStringValue (VTPixelTransferPropertyKeys.ScalingMode, VTPixelTransferPropertyKeys.ScalingMode_Normal);
					break;
				case VTScalingMode.CropSourceToCleanAperture:
					SetStringValue (VTPixelTransferPropertyKeys.ScalingMode, VTPixelTransferPropertyKeys.ScalingMode_CropSourceToCleanAperture);
					break;
				case VTScalingMode.Letterbox:
					SetStringValue (VTPixelTransferPropertyKeys.ScalingMode, VTPixelTransferPropertyKeys.ScalingMode_Letterbox);
					break;
				case VTScalingMode.Trim:
					SetStringValue (VTPixelTransferPropertyKeys.ScalingMode, VTPixelTransferPropertyKeys.ScalingMode_Trim);
					break;
				default:
					SetStringValue (VTPixelTransferPropertyKeys.ScalingMode, null);
					break;
				}
			}
		}

		public VTDownsamplingMode DownsamplingMode { 
			get {
				var key = GetNSStringValue (VTPixelTransferPropertyKeys.DownsamplingMode);

				if (key == null)
					return VTDownsamplingMode.Unset;
				if (key == VTPixelTransferPropertyKeys.DownsamplingMode_Decimate)
					return VTDownsamplingMode.Decimate;
				if (key == VTPixelTransferPropertyKeys.DownsamplingMode_Average)
					return VTDownsamplingMode.Average;
				return VTDownsamplingMode.Unset; 
			}
			set {
				switch (value) {
				case VTDownsamplingMode.Decimate:
					SetStringValue (VTPixelTransferPropertyKeys.DownsamplingMode, VTPixelTransferPropertyKeys.DownsamplingMode_Decimate);
					break;
				case VTDownsamplingMode.Average:
					SetStringValue (VTPixelTransferPropertyKeys.DownsamplingMode, VTPixelTransferPropertyKeys.DownsamplingMode_Average);
					break;
				default:
					SetStringValue (VTPixelTransferPropertyKeys.DownsamplingMode, null);
					break;
				}
			}
		}

#if XAMCORE_2_0
		[iOS (10,0)]
		public VTColorPrimaries DestinationColorPrimaries { 
			get {
				var key = GetNSStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries);

				if (key == null)
					return VTColorPrimaries.Unset;
				if (key == CVImageBuffer.ColorPrimaries_ITU_R_709_2)
					return VTColorPrimaries.ItuR7092;
				if (key == CVImageBuffer.ColorPrimaries_EBU_3213)
					return VTColorPrimaries.Ebu3213;
				if (key == CVImageBuffer.ColorPrimaries_SMPTE_C)
					return VTColorPrimaries.SmpteC;
				if (key == CVImageBuffer.ColorPrimaries_P22)
					return VTColorPrimaries.P22;
				return VTColorPrimaries.Unset;
			}
			set {
				switch (value) {
				case VTColorPrimaries.ItuR7092:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries, CVImageBuffer.ColorPrimaries_ITU_R_709_2);
					break;
				case VTColorPrimaries.Ebu3213:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries, CVImageBuffer.ColorPrimaries_EBU_3213);
					break;
				case VTColorPrimaries.SmpteC:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries, CVImageBuffer.ColorPrimaries_SMPTE_C);
					break;
				case VTColorPrimaries.P22:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries, CVImageBuffer.ColorPrimaries_P22);
					break;
				default:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationColorPrimaries, null);
					break;
				}
			}
		}
#endif

		[iOS (10,0)]
		public VTTransferFunction DestinationTransferFunction { 
			get {
				var key = GetNSStringValue (VTPixelTransferPropertyKeys.DestinationTransferFunction);

				if (key == null)
					return VTTransferFunction.Unset;
				if (key == CVImageBuffer.TransferFunction_ITU_R_709_2)
					return VTTransferFunction.ItuR7092;
				if (key == CVImageBuffer.TransferFunction_SMPTE_240M_1995)
					return VTTransferFunction.Smpte240M1955;
				if (key == CVImageBuffer.TransferFunction_UseGamma)
					return VTTransferFunction.UseGamma;
				return VTTransferFunction.Unset;
			}
			set {
				switch (value) {
				case VTTransferFunction.ItuR7092:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationTransferFunction, CVImageBuffer.TransferFunction_ITU_R_709_2);
					break;
				case VTTransferFunction.Smpte240M1955:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationTransferFunction, CVImageBuffer.TransferFunction_SMPTE_240M_1995);
					break;
				case VTTransferFunction.UseGamma:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationTransferFunction, CVImageBuffer.TransferFunction_UseGamma);
					break;
				default:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationTransferFunction, null);
					break;
				}
			}
		}

		public VTYCbCrMatrix DestinationYCbCrMatrix { 
			get {
				var key = GetNSStringValue (VTPixelTransferPropertyKeys.DestinationYCbCrMatrix);

				if (key == null)
					return VTYCbCrMatrix.Unset;
				if (key == CVImageBuffer.YCbCrMatrix_ITU_R_709_2)
					return VTYCbCrMatrix.ItuR7092;
				if (key == CVImageBuffer.YCbCrMatrix_ITU_R_601_4)
					return VTYCbCrMatrix.ItuR6014;
				if (key == CVImageBuffer.YCbCrMatrix_SMPTE_240M_1995)
					return VTYCbCrMatrix.Smpte240M1955;
				return VTYCbCrMatrix.Unset; 
			}
			set {
				switch (value) {
				case VTYCbCrMatrix.ItuR7092:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationYCbCrMatrix, CVImageBuffer.YCbCrMatrix_ITU_R_709_2);
					break;
				case VTYCbCrMatrix.ItuR6014:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationYCbCrMatrix, CVImageBuffer.YCbCrMatrix_ITU_R_601_4);
					break;
				case VTYCbCrMatrix.Smpte240M1955:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationYCbCrMatrix, CVImageBuffer.YCbCrMatrix_SMPTE_240M_1995);
					break;
				default:
					SetStringValue (VTPixelTransferPropertyKeys.DestinationYCbCrMatrix, null);
					break;
				}
			}
		}
	}
}
