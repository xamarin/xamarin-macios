//
// Defs.cs: PrintCore definitions
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2016 Microsoft Inc
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using ObjCRuntime;
using Foundation;

using PMObject = System.IntPtr;
using OSStatus = System.Int32;
using System.Runtime.Versioning;

namespace PrintCore {

	public enum PMStatusCode {
		Ok = 0,
		GeneralError = -30870,
		OutOfScope = -30871,
		InvalidParameter = -50,
		NoDefaultPrinter = -30872,
		NotImplemented = -30873,
		NoSuchEntry = -30874,
		InvalidPrintSettings = -30875,
		InvalidPageFormat = -30876,
		ValueOutOfRange = -30877,
		LockIgnored = -30878,

		InvalidPrintSession = -30879,
		InvalidPrinter = -30880,
		ObjectInUse = -30881,
		InvalidPreset = -30899,

		AllocationFailure = -108,
		InternalError = -30870,

		InvalidIndex = -30882,
		StringConversionFailure = -30883,
		XMLParseError = -30884,

		InvalidJobTemplate = -30885,
		InvalidPrinterInfo = -30886,
		InvalidConnection = -30887,
		InvalidKey = -30888,
		InvalidValue = -30889,
		InvalidAllocator = -30890,
		InvalidTicket = -30891,
		InvalidItem = -30892,
		InvalidType = -30893,
		InvalidReply = -30894,
		InvalidFileType = -30895,
		InvalidObject = -30896,
		InvalidPaper = -30897,
		InvalidCalibrationTarget = -30898,

		NoDefaultItem = -9500,
		NoDefaultSettings = -9501,
		InvalidPdeContext = -9530,
		DontSwitchPdeError = -9531,
		UnableToFindProcess = -9532,
		FeatureNotInstalled = -9533,

		InvalidPbmRef = -9540,
		NoSelectedPrinters = -9541,
		InvalidLookupSpec = -9542,
		SyncRequestFailed = -9543,
		EditRequestFailed = -9544,
		PrBrowserNoUI = -9545,

		TicketTypeNotFound = -9580,
		UpdateTicketFailed = -9581,
		ValidateTicketFailed = -9582,
		SubTicketNotFound = -9583,
		InvalidSubTicket = -9584,
		DeleteSubTicketFailed = -9585,
		ItemIsLocked = -9586,
		TicketIsLocked = -9587,
		TemplateIsLocked = -9588,
		KeyNotFound = -9589,
		KeyNotUnique = -9590,
		UnknownDataType = -9591,

		CreateMessageFailed = -9620,
		ServerCommunicationFailed = -9621,
		KeyOrValueNotFound = -9623,
		MessagingError = -9624,

		ServerNotFound = -9630,
		ServerAlreadyRunning = -9631,
		ServerSuspended = -9632,
		ServerAttributeRestricted = -9633,
		FileOrDirOperationFailed = -9634,
		UserOrGroupNotFound = -9635,
		PermissionError = -9636,
		UnknownMessage = -9637,
		QueueNotFound = -9638,
		QueueAlreadyExists = -9639,
		QueueJobFailed = -9640,
		JobNotFound = -9641,
		JobBusy = -9642,
		JobCanceled = -9643,
		DocumentNotFound = -9644,

		PMSymbolNotFound = -9660,
		IOMSymbolNotFound = -9661,
		CvmSymbolNotFound = -9662,
		InvalidPMContext = -9663,
		InvalidIOMContext = -9664,
		InvalidCvmContext = -9665,
		InvalidJobID = -9666,
		NoPrinterJobID = -9667,
		JobStreamOpenFailed = -9668,
		JobStreamReadFailed = -9669,
		JobStreamEndError = -9670,
		JobManagerAborted = -9671,
		JobGetTicketBadFormatError = -9672,
		JobGetTicketReadError = -9673,

		PluginNotFound = -9701,
		PluginRegisterationFailed = -9702,
		FontNotFound = -9703,
		FontNameTooLong = -9704,
		GeneralCGError = -9705,
		InvalidState = -9706,
		UnexpectedImagingError = -9707,

		InvalidPrinterAddress = -9780,
		OpenFailed = -9781,
		ReadFailed = -9782,
		WriteFailed = -9783,
		StatusFailed = -9784,
		CloseFailed = -9785,
		UnsupportedConnection = -9786,
		IOAttrNotAvailable = -9787,
		ReadGotZeroData = -9788,
	}

	public enum PMPrinterState : System.UInt16 {
		Idle = 3,
		Processing = 4,
		Stopped = 5
	}

	public enum PMDuplexMode : System.UInt32 {
		None = 1,
		NoTumble = 2,
		Tumble = 3,
		SimplexTumble = 4
	}

	public enum PMOrientation : System.UInt16 {
		Portrait = 1,
		Landscape = 2,
		ReversePortrait = 3,
		ReverseLandscape = 4
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PMResolution {
		double hRes, vRes;
		public double HorizontalResolution => hRes;
		public double VerticalResolution => vRes;

		public PMResolution (double horizontal, double vertical)
		{
			hRes = horizontal;
			vRes = vertical;
		}
		public override string ToString ()
		{
			return $"[HorizontalResolution={hRes},VerticalResolution={vRes}]";
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PMRect {
		internal double top, left, bottom, right;

		public double Top => top;
		public double Left => left;
		public double Bottom => bottom;
		public double Right => right;

		public PMRect (double top, double bottom, double left, double right)
		{
			this.top = top;
			this.bottom = bottom;
			this.left = left;
			this.right = right;
		}

		public override string ToString ()
		{
			return $"[Top={top},Bottom={bottom},Left={left},Right={right}]";
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct PMPaperMargins {
		PMRect Rect;

		public double Top => Rect.top;
		public double Left => Rect.left;
		public double Bottom => Rect.bottom;
		public double Right => Rect.right;

		public PMPaperMargins (double top, double bottom, double left, double right)
		{
			Rect = new PMRect (top, bottom, left, right);
		}

		public override string ToString ()
		{
			return Rect.ToString ();
		}
	}
}
