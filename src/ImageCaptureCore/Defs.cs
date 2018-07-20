//
// Copyright 2018 Microsoft
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

//
// Defs.cs: Enumerations and definitions for ImageCaptureCore
//
using System;

using ObjCRuntime;

namespace ImageCaptureCore {

	[Native]
	public enum ICExifOrientationType : ulong {
		Orientation1 = 1,
		Orientation2 = 2,
		Orientation3 = 3,
		Orientation4 = 4,
		Orientation5 = 5,
		Orientation6 = 6,
		Orientation7 = 7,
		Orientation8 = 8,
	}
	
	public enum ICReturnCodeOffset {
		Thumbnail = -21000,
		Metadata = -21050,
		Download = -21100,
		Delete = -21150,
		ExFat = -21200,
		Ptp = -21250,
		System = -21300,
	}
	
	[Native]
	public enum ICReturnCode : long {
		Success = 0,
		InvalidParam = -9922,
		CommunicationTimedOut = -9923,
		ScanOperationCanceled = -9924,
		ScannerInUseByLocalUser = -9925,
		ScannerInUseByRemoteUser = -9926,
		DeviceFailedToOpenSession = -9927,
		DeviceFailedToCloseSession = -9928,
		ScannerFailedToSelectFunctionalUnit = -9929,
		ScannerFailedToCompleteOverviewScan = -9930,
		ScannerFailedToCompleteScan = -9931,
		ReceivedUnsolicitedScannerStatusInfo = -9932,
		ReceivedUnsolicitedScannerErrorInfo = -9933,
		DownloadFailed = -9934,
		UploadFailed = -9935,
		FailedToCompletePassThroughCommand = -9936,
		DownloadCanceled = -9937,
		FailedToEnableTethering = -9938,
		FailedToDisableTethering = -9939,
		FailedToCompleteSendMessageRequest = -9940,
		DeleteFilesFailed = -9941,
		DeleteFilesCanceled = -9942,
		DeviceIsPasscodeLocked = -9943,
		DeviceFailedToTakePicture = -9944,
		DeviceSoftwareNotInstalled = -9945,
		DeviceSoftwareIsBeingInstalled = -9946,
		DeviceSoftwareInstallationCompleted = -9947,
		DeviceSoftwareInstallationCanceled = -9948,
		DeviceSoftwareInstallationFailed = -9949,
		DeviceSoftwareNotAvailable = -9950,
		DeviceCouldNotPair = -9951,
		DeviceCouldNotUnpair = -9952,
		DeviceNeedsCredentials = -9953,
		DeviceIsBusyEnumerating = -9954,
		DeviceCommandGeneralFailure = -9955,
		DeviceFailedToCompleteTransfer = -9956,
		DeviceFailedToSendData = -9957,
		SessionNotOpened = -9958,
		ThumbnailNotAvailable = ICReturnCodeOffset.Thumbnail,
		ThumbnailAlreadyFetching = ICReturnCodeOffset.Thumbnail - 1,
		ThumbnailCanceled = ICReturnCodeOffset.Thumbnail - 2,
		ThumbnailInvalid = ICReturnCodeOffset.Thumbnail - 3,
		ErrorDeviceEjected = ICReturnCodeOffset.System,
		MetadataNotAvailable = ICReturnCodeOffset.Metadata,
		MetadataAlreadyFetching = ICReturnCodeOffset.Metadata - 1,
		MetadataCanceled = ICReturnCodeOffset.Metadata - 2,
		MetadataInvalid = ICReturnCodeOffset.Metadata - 3,
		MultiErrorDictionary = -30000,
	}
	
	[Native]
	public enum ICDeviceType : ulong {
		Camera = 1,
		Scanner = 2,
	}
	
	[Native]
	public enum ICDeviceLocationType : ulong {
		Local = 256,
		Shared = 512,
		Bonjour = 1024,
		Bluetooth = 2048,
	}
	
	// Combination of ICDeviceTypeMask and ICDeviceLocationTypeMask
	[Native]
	[Flags]
	public enum ICBrowsedDeviceType : ulong {
		Camera = 1,
		Scanner = 2,
		Local = 256,
		Shared = 512,
		Bonjour = 1024,
		Bluetooth = 2048,
		Remote = 65024,

	}

	[Native]
	public enum ICScannerFunctionalUnitType : ulong {
		Flatbed = 0,
		PositiveTransparency = 1,
		NegativeTransparency = 2,
		DocumentFeeder = 3,
	}
	
	[Native]
	public enum ICScannerMeasurementUnit : ulong {
		Inches = 0,
		Centimeters = 1,
		Picas = 2,
		Points = 3,
		Twips = 4,
		Pixels = 5,
	}
	
	[Native]
	public enum ICScannerBitDepth : ulong {
		Bits1 = 1,
		Bits8 = 8,
		Bits16 = 16,
	}
	
	[Native]
	public enum ICScannerColorDataFormatType : ulong {
		Chunky = 0,
		Planar = 1,
	}
	
	[Native]
	public enum ICScannerPixelDataType : ulong {
		Bw = 0,
		Gray = 1,
		Rgb = 2,
		Palette = 3,
		Cmy = 4,
		Cmyk = 5,
		Yuv = 6,
		Yuvk = 7,
		Ciexyz = 8,
	}
	
	[Native]
	public enum ICScannerDocumentType : ulong {
		Default = 0,
		A4 = 1,
		B5 = 2,
		USLetter = 3,
		USLegal = 4,
		A5 = 5,
		IsoB4 = 6,
		IsoB6 = 7,
		USLedger = 9,
		USExecutive = 10,
		A3 = 11,
		IsoB3 = 12,
		A6 = 13,
		C4 = 14,
		C5 = 15,
		C6 = 16,
		Type4A0 = 17,
		Type2A0 = 18,
		A0 = 19,
		A1 = 20,
		A2 = 21,
		A7 = 22,
		A8 = 23,
		A9 = 24,
		A10 = 25,
		IsoB0 = 26,
		IsoB1 = 27,
		IsoB2 = 28,
		IsoB5 = 29,
		IsoB7 = 30,
		IsoB8 = 31,
		IsoB9 = 32,
		IsoB10 = 33,
		JisB0 = 34,
		JisB1 = 35,
		JisB2 = 36,
		JisB3 = 37,
		JisB4 = 38,
		JisB6 = 39,
		JisB7 = 40,
		JisB8 = 41,
		JisB9 = 42,
		JisB10 = 43,
		C0 = 44,
		C1 = 45,
		C2 = 46,
		C3 = 47,
		C7 = 48,
		C8 = 49,
		C9 = 50,
		C10 = 51,
		USStatement = 52,
		BusinessCard = 53,
		E = 60,
		Type3R = 61,
		Type4R = 62,
		Type5R = 63,
		Type6R = 64,
		Type8R = 65,
		S8R = 66,
		Type10R = 67,
		S10R = 68,
		Type11R = 69,
		Type12R = 70,
		S12R = 71,
		Instamatic110 = 72,
		ApsH = 73,
		ApsC = 74,
		ApsP = 75,
		Standard35 = 76,
		Mf = 77,
		Lf = 78,
	}
	
	public enum ICScannerFunctionalUnitState : uint {
		Ready = (1u << 0),
		ScanInProgress = (1u << 1),
		OverviewScanInProgress = (1u << 2),
	}
	
	[Native]
	public enum ICScannerFeatureType : ulong {
		Enumeration = 0,
		Range = 1,
		Boolean = 2,
		Template = 3,
	}
	
	[Native]
	public enum ICScannerTransferMode : ulong {
		FileBased = 0,
		MemoryBased = 1,
	}
}
