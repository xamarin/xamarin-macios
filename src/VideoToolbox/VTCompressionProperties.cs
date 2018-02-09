// 
// VTCompressionProperties.cs: Strongly Typed dictionary for VTCompressionPropertyKeys 
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

namespace VideoToolbox {
	public partial class VTCompressionProperties {
		public VTProfileLevel ProfileLevel {
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.ProfileLevel);

				if (key == null)
					return VTProfileLevel.Unset;
				if (key == VTProfileLevelKeys.H264_Baseline_1_3)
					return VTProfileLevel.H264Baseline13;
				if (key == VTProfileLevelKeys.H264_Baseline_3_0)
					return VTProfileLevel.H264Baseline30;
				if (key == VTProfileLevelKeys.H264_Baseline_3_1)
					return VTProfileLevel.H264Baseline31;
				if (key == VTProfileLevelKeys.H264_Baseline_3_2)
					return VTProfileLevel.H264Baseline32;
				if (key == VTProfileLevelKeys.H264_Baseline_4_0)
					return VTProfileLevel.H264Baseline40;
				if (key == VTProfileLevelKeys.H264_Baseline_4_1)
					return VTProfileLevel.H264Baseline41;
				if (key == VTProfileLevelKeys.H264_Baseline_4_2)
					return VTProfileLevel.H264Baseline42;
				if (key == VTProfileLevelKeys.H264_Baseline_5_0)
					return VTProfileLevel.H264Baseline50;
				if (key == VTProfileLevelKeys.H264_Baseline_5_1)
					return VTProfileLevel.H264Baseline51;
				if (key == VTProfileLevelKeys.H264_Baseline_5_2)
					return VTProfileLevel.H264Baseline52;
				if (key == VTProfileLevelKeys.H264_Baseline_AutoLevel)
					return VTProfileLevel.H264BaselineAutoLevel;
				if (key == VTProfileLevelKeys.H264_Main_3_0)
					return VTProfileLevel.H264Main30;
				if (key == VTProfileLevelKeys.H264_Main_3_1)
					return VTProfileLevel.H264Main31;
				if (key == VTProfileLevelKeys.H264_Main_3_2)
					return VTProfileLevel.H264Main32;
				if (key == VTProfileLevelKeys.H264_Main_4_0)
					return VTProfileLevel.H264Main40;
				if (key == VTProfileLevelKeys.H264_Main_4_1)
					return VTProfileLevel.H264Main41;
				if (key == VTProfileLevelKeys.H264_Main_4_2)
					return VTProfileLevel.H264Main42;
				if (key == VTProfileLevelKeys.H264_Main_5_0)
					return VTProfileLevel.H264Main50;
				if (key == VTProfileLevelKeys.H264_Main_5_1)
					return VTProfileLevel.H264Main51;
				if (key == VTProfileLevelKeys.H264_Main_5_2)
					return VTProfileLevel.H264Main52;
				if (key == VTProfileLevelKeys.H264_Main_AutoLevel)
					return VTProfileLevel.H264MainAutoLevel;
				if (key == VTProfileLevelKeys.H264_Extended_5_0)
					return VTProfileLevel.H264Extended50;
				if (key == VTProfileLevelKeys.H264_Extended_AutoLevel)
					return VTProfileLevel.H264ExtendedAutoLevel;
				if (key == VTProfileLevelKeys.H264_High_3_0)
					return VTProfileLevel.H264High30;
				if (key == VTProfileLevelKeys.H264_High_3_1)
					return VTProfileLevel.H264High31;
				if (key == VTProfileLevelKeys.H264_High_3_2)
					return VTProfileLevel.H264High32;
				if (key == VTProfileLevelKeys.H264_High_4_0)
					return VTProfileLevel.H264High40;
				if (key == VTProfileLevelKeys.H264_High_4_1)
					return VTProfileLevel.H264High41;
				if (key == VTProfileLevelKeys.H264_High_4_2)
					return VTProfileLevel.H264High42;
				if (key == VTProfileLevelKeys.H264_High_5_0)
					return VTProfileLevel.H264High50;
				if (key == VTProfileLevelKeys.H264_High_5_1)
					return VTProfileLevel.H264High51;
				if (key == VTProfileLevelKeys.H264_High_5_2)
					return VTProfileLevel.H264High52;
				if (key == VTProfileLevelKeys.H264_High_AutoLevel)
					return VTProfileLevel.H264HighAutoLevel;
				if (key == VTProfileLevelKeys.MP4V_Simple_L0)
					return VTProfileLevel.MP4VSimpleL0;
				if (key == VTProfileLevelKeys.MP4V_Simple_L1)
					return VTProfileLevel.MP4VSimpleL1;
				if (key == VTProfileLevelKeys.MP4V_Simple_L2)
					return VTProfileLevel.MP4VSimpleL2;
				if (key == VTProfileLevelKeys.MP4V_Simple_L3)
					return VTProfileLevel.MP4VSimpleL3;
				if (key == VTProfileLevelKeys.MP4V_Main_L2)
					return VTProfileLevel.MP4VMainL2;
				if (key == VTProfileLevelKeys.MP4V_Main_L3)
					return VTProfileLevel.MP4VMainL3;
				if (key == VTProfileLevelKeys.MP4V_Main_L4)
					return VTProfileLevel.MP4VMainL4;
				if (key == VTProfileLevelKeys.MP4V_AdvancedSimple_L0)
					return VTProfileLevel.MP4VAdvancedSimpleL0;
				if (key == VTProfileLevelKeys.MP4V_AdvancedSimple_L1)
					return VTProfileLevel.MP4VAdvancedSimpleL1;
				if (key == VTProfileLevelKeys.MP4V_AdvancedSimple_L2)
					return VTProfileLevel.MP4VAdvancedSimpleL2;
				if (key == VTProfileLevelKeys.MP4V_AdvancedSimple_L3)
					return VTProfileLevel.MP4VAdvancedSimpleL3;
				if (key == VTProfileLevelKeys.MP4V_AdvancedSimple_L4)
					return VTProfileLevel.MP4VAdvancedSimpleL4;
				if (key == VTProfileLevelKeys.H263_Profile0_Level10)
					return VTProfileLevel.H263Profile0Level10;
				if (key == VTProfileLevelKeys.H263_Profile0_Level45)
					return VTProfileLevel.H263Profile0Level45;
				if (key == VTProfileLevelKeys.H263_Profile3_Level45)
					return VTProfileLevel.H263Profile3Level45;
				if (key == VTProfileLevelKeys.Hevc_Main_AutoLevel)
					return VTProfileLevel.HevcMainAutoLevel;
				if (key == VTProfileLevelKeys.Hevc_Main10_AutoLevel)
					return VTProfileLevel.HevcMain10AutoLevel;
				return VTProfileLevel.Unset;
			}
			set {
				switch (value) {
				case VTProfileLevel.H264Baseline13:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_1_3);
					break;
				case VTProfileLevel.H264Baseline30:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_3_0);
					break;
				case VTProfileLevel.H264Baseline31:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_3_1);
					break;
				case VTProfileLevel.H264Baseline32:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_3_2);
					break;
				case VTProfileLevel.H264Baseline40:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_4_0);
					break;
				case VTProfileLevel.H264Baseline41:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_4_1);
					break;
				case VTProfileLevel.H264Baseline42:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_4_2);
					break;
				case VTProfileLevel.H264Baseline50:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_5_0);
					break;
				case VTProfileLevel.H264Baseline51:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_5_1);
					break;
				case VTProfileLevel.H264Baseline52:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_5_2);
					break;
				case VTProfileLevel.H264BaselineAutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Baseline_AutoLevel);
					break;
				case VTProfileLevel.H264Main30:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_3_0);
					break;
				case VTProfileLevel.H264Main31:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_3_1);
					break;
				case VTProfileLevel.H264Main32:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_3_2);
					break;
				case VTProfileLevel.H264Main40:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_4_0);
					break;
				case VTProfileLevel.H264Main41:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_4_1);
					break;
				case VTProfileLevel.H264Main42:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_4_2);
					break;
				case VTProfileLevel.H264Main50:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_5_0);
					break;
				case VTProfileLevel.H264Main51:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_5_1);
					break;
				case VTProfileLevel.H264Main52:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_5_2);
					break;
				case VTProfileLevel.H264MainAutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Main_AutoLevel);
					break;
				case VTProfileLevel.H264Extended50:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Extended_5_0);
					break;
				case VTProfileLevel.H264ExtendedAutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_Extended_AutoLevel);
					break;
				case VTProfileLevel.H264High30:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_3_0);
					break;
				case VTProfileLevel.H264High31:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_3_1);
					break;
				case VTProfileLevel.H264High32:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_3_2);
					break;
				case VTProfileLevel.H264High40:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_4_0);
					break;
				case VTProfileLevel.H264High41:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_4_1);
					break;
				case VTProfileLevel.H264High42:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_4_2);
					break;
				case VTProfileLevel.H264High50:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_5_0);
					break;
				case VTProfileLevel.H264High51:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_5_1);
					break;
				case VTProfileLevel.H264High52:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_5_2);
					break;
				case VTProfileLevel.H264HighAutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H264_High_AutoLevel);
					break;
				case VTProfileLevel.MP4VSimpleL0:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Simple_L0);
					break;
				case VTProfileLevel.MP4VSimpleL1:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Simple_L1);
					break;
				case VTProfileLevel.MP4VSimpleL2:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Simple_L2);
					break;
				case VTProfileLevel.MP4VSimpleL3:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Simple_L3);
					break;
				case VTProfileLevel.MP4VMainL2:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Main_L2);
					break;
				case VTProfileLevel.MP4VMainL3:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Main_L3);
					break;
				case VTProfileLevel.MP4VMainL4:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_Main_L4);
					break;
				case VTProfileLevel.MP4VAdvancedSimpleL0:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_AdvancedSimple_L0);
					break;
				case VTProfileLevel.MP4VAdvancedSimpleL1:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_AdvancedSimple_L1);
					break;
				case VTProfileLevel.MP4VAdvancedSimpleL2:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_AdvancedSimple_L2);
					break;
				case VTProfileLevel.MP4VAdvancedSimpleL3:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_AdvancedSimple_L3);
					break;
				case VTProfileLevel.MP4VAdvancedSimpleL4:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.MP4V_AdvancedSimple_L4);
					break;
				case VTProfileLevel.H263Profile0Level10:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H263_Profile0_Level10);
					break;
				case VTProfileLevel.H263Profile0Level45:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H263_Profile0_Level45);
					break;
				case VTProfileLevel.H263Profile3Level45:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.H263_Profile3_Level45);
					break;
				case VTProfileLevel.HevcMainAutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.Hevc_Main_AutoLevel);
					break;
				case VTProfileLevel.HevcMain10AutoLevel:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, VTProfileLevelKeys.Hevc_Main10_AutoLevel);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.ProfileLevel, null);
					break;
				}
			}
		}

		[Mac (10,9)]
		public VTH264EntropyMode H264EntropyMode { 
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.H264EntropyMode);

				if (key == null)
					return VTH264EntropyMode.Unset;
				if (key == VTH264EntropyModeKeys.CAVLC)
					return VTH264EntropyMode.Cavlc;
				if (key == VTH264EntropyModeKeys.CABAC)
					return VTH264EntropyMode.Cabac;
				return VTH264EntropyMode.Unset;
			}
			set {
				switch (value) {
				case VTH264EntropyMode.Cavlc:
					SetStringValue (VTCompressionPropertyKey.H264EntropyMode, VTH264EntropyModeKeys.CAVLC);
					break;
				case VTH264EntropyMode.Cabac:
					SetStringValue (VTCompressionPropertyKey.H264EntropyMode, VTH264EntropyModeKeys.CABAC);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.H264EntropyMode, null);
					break;
				}
			}
		}

		public List<VTDataRateLimit> DataRateLimits { 
			get { 
				using (var arr = GetNativeValue <NSArray> (VTCompressionPropertyKey.DataRateLimits)) {
					if (arr == null)
						return null;

					var list = new List<VTDataRateLimit> ();
					for (nuint i = 0; i < (nuint)arr.Count; i += 2) {
						var rateLimit = new VTDataRateLimit (
#if XAMCORE_2_0
							arr.GetItem <NSNumber> (i).UInt32Value,
							arr.GetItem <NSNumber> (i + 1).DoubleValue
#else
							arr.GetItem <NSNumber> ((nint)i).UInt32Value,
							arr.GetItem <NSNumber> ((nint)(i + 1)).DoubleValue
#endif
						);
						list.Add (rateLimit);
					}
					return list;
				}
			}
			set {
				if (value != null) {
					using (var arr = new NSMutableArray (capacity: (nuint)(value.Count * 2))) {
						foreach (var item in value) {
							arr.Add (NSNumber.FromUInt32 (item.NumberOfBytes));
							arr.Add (NSNumber.FromDouble (item.Seconds));
						}
						SetNativeValue (VTCompressionPropertyKey.DataRateLimits, arr);
					}
				} else
					SetNativeValue (VTCompressionPropertyKey.DataRateLimits, null);
			}
		}

		public VTFieldDetail FieldDetail { 
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.FieldDetail);

				if (key == null)
					return VTFieldDetail.Unset;
				if (key == CVImageBuffer.FieldDetailTemporalTopFirst)
					return VTFieldDetail.TemporalTopFirst;
				if (key == CVImageBuffer.FieldDetailTemporalBottomFirst)
					return VTFieldDetail.TemporalBottomFirst;
				if (key == CVImageBuffer.FieldDetailSpatialFirstLineEarly)
					return VTFieldDetail.SpatialFirstLineEarly;
				if (key == CVImageBuffer.FieldDetailSpatialFirstLineLate)
					return VTFieldDetail.SpatialFirstLineLate;
				return VTFieldDetail.Unset;
			}
			set {
				switch (value) {
				case VTFieldDetail.TemporalTopFirst:
					SetStringValue (VTCompressionPropertyKey.FieldDetail, CVImageBuffer.FieldDetailTemporalTopFirst);
					break;
				case VTFieldDetail.TemporalBottomFirst:
					SetStringValue (VTCompressionPropertyKey.FieldDetail, CVImageBuffer.FieldDetailTemporalBottomFirst);
					break;
				case VTFieldDetail.SpatialFirstLineEarly:
					SetStringValue (VTCompressionPropertyKey.FieldDetail, CVImageBuffer.FieldDetailSpatialFirstLineEarly);
					break;
				case VTFieldDetail.SpatialFirstLineLate:
					SetStringValue (VTCompressionPropertyKey.FieldDetail, CVImageBuffer.FieldDetailSpatialFirstLineLate);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.FieldDetail, null);
					break;
				}
			}
		}
#if XAMCORE_2_0
		public VTColorPrimaries ColorPrimaries { 
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.ColorPrimaries);

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
					SetStringValue (VTCompressionPropertyKey.ColorPrimaries, CVImageBuffer.ColorPrimaries_ITU_R_709_2);
					break;
				case VTColorPrimaries.Ebu3213:
					SetStringValue (VTCompressionPropertyKey.ColorPrimaries, CVImageBuffer.ColorPrimaries_EBU_3213);
					break;
				case VTColorPrimaries.SmpteC:
					SetStringValue (VTCompressionPropertyKey.ColorPrimaries, CVImageBuffer.ColorPrimaries_SMPTE_C);
					break;
				case VTColorPrimaries.P22:
					SetStringValue (VTCompressionPropertyKey.ColorPrimaries, CVImageBuffer.ColorPrimaries_P22);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.ColorPrimaries, null);
					break;
				}
			}
		}
#endif

		public VTTransferFunction TransferFunction { 
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.TransferFunction);

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
					SetStringValue (VTCompressionPropertyKey.TransferFunction, CVImageBuffer.TransferFunction_ITU_R_709_2);
					break;
				case VTTransferFunction.Smpte240M1955:
					SetStringValue (VTCompressionPropertyKey.TransferFunction, CVImageBuffer.TransferFunction_SMPTE_240M_1995);
					break;
				case VTTransferFunction.UseGamma:
					SetStringValue (VTCompressionPropertyKey.TransferFunction, CVImageBuffer.TransferFunction_UseGamma);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.TransferFunction, null);
					break;
				}
			}
		}

		public VTYCbCrMatrix YCbCrMatrix { 
			get {
				var key = GetNSStringValue (VTCompressionPropertyKey.YCbCrMatrix);

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
					SetStringValue (VTCompressionPropertyKey.YCbCrMatrix, CVImageBuffer.YCbCrMatrix_ITU_R_709_2);
					break;
				case VTYCbCrMatrix.ItuR6014:
					SetStringValue (VTCompressionPropertyKey.YCbCrMatrix, CVImageBuffer.YCbCrMatrix_ITU_R_601_4);
					break;
				case VTYCbCrMatrix.Smpte240M1955:
					SetStringValue (VTCompressionPropertyKey.YCbCrMatrix, CVImageBuffer.YCbCrMatrix_SMPTE_240M_1995);
					break;
				default:
					SetStringValue (VTCompressionPropertyKey.YCbCrMatrix, null);
					break;
				}
			}
		}

		public VTMultiPassStorage MultiPassStorage {
			get	{
				return GetNativeValue<VTMultiPassStorage> (VTCompressionPropertyKey.MultiPassStorage);
			}
			set {
				SetNativeValue (VTCompressionPropertyKey.MultiPassStorage, value);
			}
		}
	}
}

