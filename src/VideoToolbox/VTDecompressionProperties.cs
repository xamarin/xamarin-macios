// 
// VTDecompressionProperties.cs: Strongly Typed dictionary for VTDecompressionPropertyKey 
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
	public partial class VTDecompressionProperties {
		public CVPixelBufferPool PixelBufferPool {
			get {
				return GetNativeValue<CVPixelBufferPool> (VTDecompressionPropertyKey.PixelBufferPool);
			}
		}

		public VTFieldMode FieldMode { 
			get {
				var key = GetNSStringValue (VTDecompressionPropertyKey.FieldMode);

				if (key == null)
					return VTFieldMode.Unset;
				if (key == VTDecompressionPropertyKey.FieldMode_BothFields)
					return VTFieldMode.BothFields;
				if (key == VTDecompressionPropertyKey.FieldMode_TopFieldOnly)
					return VTFieldMode.TopFieldOnly;
				if (key == VTDecompressionPropertyKey.FieldMode_BottomFieldOnly)
					return VTFieldMode.BottomFieldOnly;
				if (key == VTDecompressionPropertyKey.FieldMode_SingleField)
					return VTFieldMode.SingleField;
				if (key == VTDecompressionPropertyKey.FieldMode_DeinterlaceFields)
					return VTFieldMode.DeinterlaceFields;
				return VTFieldMode.Unset;
			}
			set {
				switch (value) {
				case VTFieldMode.BothFields:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, VTDecompressionPropertyKey.FieldMode_BothFields);
					break;
				case VTFieldMode.TopFieldOnly:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, VTDecompressionPropertyKey.FieldMode_TopFieldOnly);
					break;
				case VTFieldMode.BottomFieldOnly:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, VTDecompressionPropertyKey.FieldMode_BottomFieldOnly);
					break;
				case VTFieldMode.SingleField:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, VTDecompressionPropertyKey.FieldMode_SingleField);
					break;
				case VTFieldMode.DeinterlaceFields:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, VTDecompressionPropertyKey.FieldMode_DeinterlaceFields);
					break;
				default:
					SetStringValue (VTDecompressionPropertyKey.FieldMode, null);
					break;
				}
			}
		}

		public VTDeinterlaceMode DeinterlaceMode { 
			get {
				var key = GetNSStringValue (VTDecompressionPropertyKey.DeinterlaceMode);

				if (key == null)
					return VTDeinterlaceMode.Unset;
				if (key == VTDecompressionPropertyKey.DeinterlaceMode_VerticalFilter)
					return VTDeinterlaceMode.VerticalFilter;
				if (key == VTDecompressionPropertyKey.DeinterlaceMode_Temporal)
					return VTDeinterlaceMode.Temporal;
				return VTDeinterlaceMode.Unset;
			}
			set {
				switch (value) {
				case VTDeinterlaceMode.VerticalFilter:
					SetStringValue (VTDecompressionPropertyKey.DeinterlaceMode, VTDecompressionPropertyKey.DeinterlaceMode_VerticalFilter);
					break;
				case VTDeinterlaceMode.Temporal:
					SetStringValue (VTDecompressionPropertyKey.DeinterlaceMode, VTDecompressionPropertyKey.DeinterlaceMode_Temporal);
					break;
				default:
					SetStringValue (VTDecompressionPropertyKey.DeinterlaceMode, null);
					break;
				}
			}
		}

		public VTOnlyTheseFrames OnlyTheseFrames { 
			get {
				var key = GetNSStringValue (VTDecompressionPropertyKey.OnlyTheseFrames);

				if (key == null)
					return VTOnlyTheseFrames.Unset;
				if (key == VTDecompressionPropertyKey.OnlyTheseFrames_AllFrames)
					return VTOnlyTheseFrames.AllFrames;
				if (key == VTDecompressionPropertyKey.OnlyTheseFrames_NonDroppableFrames)
					return VTOnlyTheseFrames.NonDroppableFrames;
				if (key == VTDecompressionPropertyKey.OnlyTheseFrames_IFrames)
					return VTOnlyTheseFrames.IFrames;
				if (key == VTDecompressionPropertyKey.OnlyTheseFrames_KeyFrames)
					return VTOnlyTheseFrames.KeyFrames;
				return VTOnlyTheseFrames.Unset;
			}
			set {
				switch (value) {
				case VTOnlyTheseFrames.AllFrames:
					SetStringValue (VTDecompressionPropertyKey.OnlyTheseFrames, VTDecompressionPropertyKey.OnlyTheseFrames_AllFrames);
					break;
				case VTOnlyTheseFrames.NonDroppableFrames:
					SetStringValue (VTDecompressionPropertyKey.OnlyTheseFrames, VTDecompressionPropertyKey.OnlyTheseFrames_NonDroppableFrames);
					break;
				case VTOnlyTheseFrames.IFrames:
					SetStringValue (VTDecompressionPropertyKey.OnlyTheseFrames, VTDecompressionPropertyKey.OnlyTheseFrames_IFrames);
					break;
				case VTOnlyTheseFrames.KeyFrames:
					SetStringValue (VTDecompressionPropertyKey.OnlyTheseFrames, VTDecompressionPropertyKey.OnlyTheseFrames_KeyFrames);
					break;
				default:
					SetStringValue (VTDecompressionPropertyKey.OnlyTheseFrames, null);
					break;
				}
			}
		}
	}
}

