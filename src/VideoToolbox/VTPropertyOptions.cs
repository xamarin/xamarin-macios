// 
// VTPropertyOptions.cs: Strongly Typed dictionary for VTPropertyKeys 
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

	public partial class VTPropertyOptions {
		public VTPropertyType Type { 
			get {
				var key = GetNSStringValue (VTPropertyKeys.Type);

				if (key == null)
					return VTPropertyType.Unset;
				if (key == VTPropertyTypeKeys.Enumeration)
					return VTPropertyType.Enumeration;
				if (key == VTPropertyTypeKeys.Boolean)
					return VTPropertyType.Boolean;
				if (key == VTPropertyTypeKeys.Number)
					return VTPropertyType.Number;
				return VTPropertyType.Unset;
			}
			set {
				switch (value) {
				case VTPropertyType.Enumeration:
					SetStringValue (VTPropertyKeys.Type, VTPropertyTypeKeys.Enumeration);
					break;
				case VTPropertyType.Boolean:
					SetStringValue (VTPropertyKeys.Type, VTPropertyTypeKeys.Boolean);
					break;
				case VTPropertyType.Number:
					SetStringValue (VTPropertyKeys.Type, VTPropertyTypeKeys.Number);
					break;
				default:
					SetStringValue (VTPropertyKeys.Type, null);
					break;
				}
			}
		}

		public VTReadWriteStatus ReadWriteStatus { 
			get {
				var key = GetNSStringValue (VTPropertyKeys.ReadWriteStatus);

				if (key == null)
					return VTReadWriteStatus.Unset;
				if (key == VTPropertyReadWriteStatusKeys.ReadOnly)
					return VTReadWriteStatus.ReadOnly;
				if (key == VTPropertyReadWriteStatusKeys.ReadWrite)
					return VTReadWriteStatus.ReadWrite;
				return VTReadWriteStatus.Unset;
			}
			set {
				switch (value) {
				case VTReadWriteStatus.ReadOnly:
					SetStringValue (VTPropertyKeys.ReadWriteStatus, VTPropertyReadWriteStatusKeys.ReadOnly);
					break;
				case VTReadWriteStatus.ReadWrite:
					SetStringValue (VTPropertyKeys.ReadWriteStatus, VTPropertyReadWriteStatusKeys.ReadWrite);
					break;
				default:
					SetStringValue (VTPropertyKeys.ReadWriteStatus, null);
					break;
				}
			}
		}
	}
}

