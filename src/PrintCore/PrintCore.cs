//
// PrintCore.cs: PrintCore bindings
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
using System.Runtime.Versioning;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreFoundation;
using PMObject = System.IntPtr;
using OSStatus = System.Int32;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PrintCore {
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPrintCoreBase : NativeObject {
		[Preserve (Conditional = true)]
		internal PMPrintCoreBase (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.PrintCoreLibrary)]
		internal extern static OSStatus PMRetain (PMObject obj);

		[DllImport (Constants.PrintCoreLibrary)]
		internal extern static OSStatus PMRelease (PMObject obj);

		protected internal override void Retain ()
		{
			PMRetain (Handle);
		}

		protected internal override void Release ()
		{
			PMRelease (Handle);
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPrintException : Exception {
		public PMPrintException (PMStatusCode code) : base (code.ToString ()) { }
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPrintSession : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreateSession (out IntPtr session);

		[Preserve (Conditional = true)]
		internal PMPrintSession (NativeHandle handle, bool owns) : base (handle, owns) { }

		static IntPtr Create ()
		{
			var code = PMCreateSession (out var value);
			if (code == PMStatusCode.Ok)
				return value;
			throw new PMPrintException (code);
		}

		public PMPrintSession ()
			: base (Create (), true)
		{
		}

		public static PMStatusCode TryCreate (out PMPrintSession? session)
		{
			IntPtr value;
			var code = PMCreateSession (out value);
			if (code == PMStatusCode.Ok) {
				session = new PMPrintSession (value, true);
				return PMStatusCode.Ok;
			}
			session = null;
			return code;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionError (IntPtr handle);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionSetError (IntPtr handle, PMStatusCode code);

		public PMStatusCode SessionError {
			get {
				return PMSessionError (Handle);
			}
			set {
				PMSessionSetError (Handle, value);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionDefaultPrintSettings (IntPtr session, IntPtr settings);

		public void AssignDefaultSettings (PMPrintSettings settings)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));
			PMSessionDefaultPrintSettings (Handle, settings.Handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionDefaultPageFormat (IntPtr session, IntPtr pageFormat);

		public void AssignDefaultPageFormat (PMPageFormat pageFormat)
		{
			if (pageFormat is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (pageFormat));
			PMSessionDefaultPageFormat (Handle, pageFormat.Handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionCreatePrinterList (IntPtr printSession, out IntPtr printerListArray, out int index, out IntPtr printer);
		public PMStatusCode CreatePrinterList (out string? []? printerList, out int index, out PMPrinter? printer)
		{
			var code = PMSessionCreatePrinterList (Handle, out var array, out index, out var printerHandle);
			if (code != PMStatusCode.Ok) {
				printerList = null;
				printer = null;
				return code;
			}

			printerList = CFArray.StringArrayFromHandle (array, true);
			if (printerHandle != IntPtr.Zero) {
				// Now get the printer, we do not own it, so retain.
				printer = new PMPrinter (printerHandle, owns: false);
			} else
				printer = null;

			return PMStatusCode.Ok;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionValidatePrintSettings (IntPtr handle, IntPtr printSettings, out byte changed);

		public PMStatusCode ValidatePrintSettings (PMPrintSettings settings, out bool changed)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));

			var code = PMSessionValidatePrintSettings (Handle, settings.Handle, out var c);
			if (code != PMStatusCode.Ok) {
				changed = false;
				return code;
			}
			changed = c != 0;
			return code;
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPrintSettings : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePrintSettings (out IntPtr session);

		[Preserve (Conditional = true)]
		internal PMPrintSettings (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		static IntPtr Create ()
		{
			var code = PMCreatePrintSettings (out var value);
			if (code == PMStatusCode.Ok)
				return value;
			throw new PMPrintException (code);
		}

		public PMPrintSettings ()
			: base (Create (), true)
		{
		}

		public static PMStatusCode TryCreate (out PMPrintSettings? settings)
		{
			IntPtr value;
			var code = PMCreatePrintSettings (out value);
			if (code == PMStatusCode.Ok) {
				settings = new PMPrintSettings (value, true);
				return PMStatusCode.Ok;
			}
			settings = null;
			return code;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetFirstPage (IntPtr handle, out uint first);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetFirstPage (IntPtr handle, uint first, byte lockb);
		public uint FirstPage {
			get {
				PMGetFirstPage (Handle, out var val);
				return val;
			}
			set {
				PMSetFirstPage (Handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetLastPage (IntPtr handle, out uint last);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetLastPage (IntPtr handle, uint last, byte lockb);
		public uint LastPage {
			get {
				PMGetLastPage (Handle, out var val);
				return val;
			}
			set {
				PMSetLastPage (Handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetPageRange (IntPtr handle, out uint minPage, out uint maxPage);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetPageRange (IntPtr handle, uint minPage, uint maxPage);
		public PMStatusCode GetPageRange (out uint minPage, out uint maxPage)
		{
			return PMGetPageRange (Handle, out minPage, out maxPage);
		}

		public PMStatusCode SetPageRange (uint minPage, uint maxPage)
		{
			return PMSetPageRange (Handle, minPage, maxPage);
		}


		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCopyPrintSettings (IntPtr source, IntPtr dest);

		public PMStatusCode CopySettings (PMPrintSettings destination)
		{
			if (destination is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destination));
			return PMCopyPrintSettings (Handle, destination.Handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetCopies (IntPtr handle, out uint copies);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetCopies (IntPtr handle, uint copies, byte elock);

		public uint Copies {
			get {
				if (PMGetCopies (Handle, out var c) == PMStatusCode.Ok)
					return c;
				else
					return 0;
			}
			set {
				PMSetCopies (Handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetCollate (IntPtr handle, out byte collate);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetCollate (IntPtr handle, byte collate);

		public bool Collate {
			get {
				if (PMGetCollate (Handle, out var c) == PMStatusCode.Ok)
					return c != 0;
				else
					return false;
			}
			set {
				PMSetCollate (Handle, (byte) (value ? 1 : 0));
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetDuplex (IntPtr handle, out PMDuplexMode mode);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetDuplex (IntPtr handle, PMDuplexMode mode);

		public PMDuplexMode DuplexMode {
			get {
				if (PMGetDuplex (Handle, out var c) == PMStatusCode.Ok)
					return c;
				else
					return PMDuplexMode.None;
			}
			set {
				PMSetDuplex (Handle, value);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetScale (IntPtr handle, out double mode);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetScale (IntPtr handle, double scale);

		public double Scale {
			get {
				if (PMGetScale (Handle, out var c) == PMStatusCode.Ok)
					return c;
				else
					return 100;
			}
			set {
				PMSetScale (Handle, value);
			}
		}

	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPageFormat : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePageFormat (out IntPtr handle);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePageFormatWithPMPaper (out IntPtr handle, IntPtr paper);

		[Preserve (Conditional = true)]
		internal PMPageFormat (NativeHandle handle, bool owns) : base (handle, owns) { }

		static IntPtr Create (PMPaper? paper = null)
		{
			IntPtr value;
			PMStatusCode code;
			if (paper is null) {
				code = PMCreatePageFormat (out value);
			} else {
				code = PMCreatePageFormatWithPMPaper (out value, paper.Handle);
			}
			if (code == PMStatusCode.Ok)
				return value;

			throw new PMPrintException (code);
		}

		public PMPageFormat (PMPaper? paper = null)
			: base (Create (paper), true)
		{
		}

		public static PMStatusCode TryCreate (out PMPageFormat? pageFormat, PMPaper? paper = null)
		{
			PMStatusCode code;
			IntPtr value;

			if (paper is null)
				code = PMCreatePageFormat (out value);
			else
				code = PMCreatePageFormatWithPMPaper (out value, paper.Handle);

			if (code == PMStatusCode.Ok) {
				pageFormat = new PMPageFormat (value, true);
				return PMStatusCode.Ok;
			}
			pageFormat = null;
			return code;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetOrientation (IntPtr handle, PMOrientation orientation, byte setToFalse);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetOrientation (IntPtr handle, out PMOrientation orientation);

		public PMOrientation Orientation {
			get {
				if (PMGetOrientation (Handle, out var o) == PMStatusCode.Ok)
					return o;
				else
					return PMOrientation.Portrait;
			}
			set {
				PMSetOrientation (Handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetAdjustedPageRect (IntPtr pageFormat, out PMRect pageRect);
		public PMRect AdjustedPageRect {
			get {
				if (PMGetAdjustedPageRect (Handle, out var rect) == PMStatusCode.Ok)
					return rect;
				return new PMRect (0, 0, 0, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetAdjustedPaperRect (IntPtr pageFormat, out PMRect pageRect);
		public PMRect AdjustedPaperRect {
			get {
				if (PMGetAdjustedPaperRect (Handle, out var rect) == PMStatusCode.Ok)
					return rect;
				return new PMRect (0, 0, 0, 0);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPaper : PMPrintCoreBase {
		[Preserve (Conditional = true)]
		internal PMPaper (NativeHandle handle, bool owns) : base (handle, owns) { }
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPaperGetID (IntPtr handle, out IntPtr str);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPaperGetWidth (IntPtr handle, out double v);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPaperGetHeight (IntPtr handle, out double v);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPaperGetMargins (IntPtr handle, out PMPaperMargins margins);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPaperCreateLocalizedName (IntPtr handle, IntPtr printer, out IntPtr name);

		public string? ID {
			get {
				var code = PMPaperGetID (Handle, out var s);
				if (code != PMStatusCode.Ok)
					return null;
				return CFString.FromHandle (s);
			}
		}

		public double Width {
			get {
				var code = PMPaperGetWidth (Handle, out var s);
				if (code != PMStatusCode.Ok)
					return 0;
				return s;
			}
		}

		public double Height {
			get {
				var code = PMPaperGetHeight (Handle, out var s);
				if (code != PMStatusCode.Ok)
					return 0;
				return s;
			}
		}

		public PMPaperMargins? Margins {
			get {
				var code = PMPaperGetMargins (Handle, out var margins);
				if (code != PMStatusCode.Ok)
					return null;
				return margins;
			}
		}

		public string? GetLocalizedName (PMPrinter printer)
		{
			if (printer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (printer));
			var code = PMPaperCreateLocalizedName (Handle, printer.Handle, out var name);
			if (code != PMStatusCode.Ok)
				return null;
			return CFString.FromHandle (name, true);
		}

	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMPrinter : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreateGenericPrinter (out IntPtr session);

		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterCreateFromPrinterID (IntPtr id);

		[Preserve (Conditional = true)]
		internal PMPrinter (NativeHandle handle, bool owns) : base (handle, owns) { }

		static IntPtr Create ()
		{
			var code = PMCreateGenericPrinter (out var value);
			if (code == PMStatusCode.Ok)
				return value;
			throw new PMPrintException (code);
		}

		public PMPrinter ()
			: base (Create (), true)
		{
		}

		static IntPtr Create (string printerId)
		{
			if (printerId is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (printerId));

			var printerIdHandle = CFString.CreateNative (printerId);
			try {
				var value = PMPrinterCreateFromPrinterID (printerIdHandle);
				if (value == IntPtr.Zero)
					throw new PMPrintException (PMStatusCode.InvalidPrinter);
				return value;
			} finally {
				CFString.ReleaseNative (printerIdHandle);
			}
		}

		public PMPrinter (string printerId)
			: base (Create (printerId), true)
		{
		}

		public static PMStatusCode TryCreate (out PMPrinter? printer)
		{
			IntPtr value;
			var code = PMCreateGenericPrinter (out value);
			if (code == PMStatusCode.Ok) {
				printer = new PMPrinter (value, owns: true);
				return PMStatusCode.Ok;
			}
			printer = null;
			return code;
		}

		public static PMPrinter? TryCreate (string printerId)
		{
			using (var idf = new CFString (printerId)) {
				var h = PMPrinterCreateFromPrinterID (idf.Handle);
				if (h == IntPtr.Zero)
					return null;
				return new PMPrinter (h, owns: true);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterGetName (IntPtr handle);
		public string? Name => CFString.FromHandle (PMPrinterGetName (Handle));

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterCopyDeviceURI (IntPtr handle, out IntPtr url);

		public PMStatusCode TryGetDeviceUrl (out NSUrl? url)
		{
			var code = PMPrinterCopyDeviceURI (Handle, out var urlH);
			if (code != PMStatusCode.Ok) {
				url = null;
				return code;
			}
			url = Runtime.GetNSObject<NSUrl> (urlH, true);
			return PMStatusCode.Ok;
		}

		public NSUrl? DeviceUrl {
			get {
				var code = PMPrinterCopyDeviceURI (Handle, out var urlH);
				if (code != PMStatusCode.Ok)
					return null;

				return Runtime.GetNSObject<NSUrl> (urlH, true);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetMakeAndModelName (IntPtr printer, out IntPtr makeAndModel);

		public string? MakeAndModel {
			get {
				if (PMPrinterGetMakeAndModelName (Handle, out var v) == PMStatusCode.Ok) {
					return CFString.FromHandle (v);
				}
				return null;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetState (IntPtr printer, out PMPrinterState state);

		// Return is overloaded - if negative, a PMStatusCode.
		public PMPrinterState PrinterState {
			get {
				var code = PMPrinterGetState (Handle, out var s);
				if (code == PMStatusCode.Ok)
					return s;

				return (PMPrinterState) code;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetMimeTypes (IntPtr printer, IntPtr settings, out IntPtr arrayStr);

		public PMStatusCode TryGetMimeTypes (PMPrintSettings settings, out string? []? mimeTypes)
		{
			var code = PMPrinterGetMimeTypes (Handle, settings.GetHandle (), out var m);
			if (code != PMStatusCode.Ok) {
				mimeTypes = null;
				return code;
			}
			mimeTypes = CFArray.StringArrayFromHandle (m);
			return PMStatusCode.Ok;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetPaperList (IntPtr printer, out IntPtr arrayStr);
		public PMStatusCode TryGetPaperList (out PMPaper []? paperList)
		{
			var code = PMPrinterGetPaperList (Handle, out var m);
			if (code != PMStatusCode.Ok) {
				paperList = null;
				return code;
			}
			paperList = (PMPaper []) CFArray.ArrayFromHandleFunc<PMPaper> (m, (handle) => new PMPaper (handle, false))!;
			return PMStatusCode.Ok;
		}

		public PMPaper [] PaperList {
			get {
				if (PMPrinterGetPaperList (Handle, out var m) != PMStatusCode.Ok)
					return Array.Empty<PMPaper> ();

				return (PMPaper []) CFArray.ArrayFromHandleFunc<PMPaper> (m, (handle) => new PMPaper (handle, false))!;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterPrintWithFile (IntPtr handle, IntPtr settings, IntPtr pageFormat, IntPtr strMimeType, IntPtr fileUrl);

		public PMStatusCode TryPrintFile (PMPrintSettings settings, PMPageFormat? pageFormat, NSUrl fileUrl, string? mimeType = null)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));
			if (fileUrl is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (fileUrl));

			IntPtr mime = CFString.CreateNative (mimeType);
			try {
				return PMPrinterPrintWithFile (Handle, settings.Handle, pageFormat.GetHandle (), mime, fileUrl.Handle);
			} finally {
				CFString.ReleaseNative (mime);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterPrintWithProvider (IntPtr printer, IntPtr settings, IntPtr pageFormat, IntPtr strMimeType, IntPtr cgDataProvider);

		public PMStatusCode TryPrintFromProvider (PMPrintSettings settings, PMPageFormat? pageFormat, CGDataProvider provider, string? mimeType = null)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));
			if (provider is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (provider));

			IntPtr mime = CFString.CreateNative (mimeType);
			try {
				return PMPrinterPrintWithProvider (Handle, settings.Handle, pageFormat.GetHandle (), mime, provider.Handle);
			} finally {
				CFString.ReleaseNative (mime);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetOutputResolution (IntPtr printer, IntPtr printSettings, out PMResolution resolutionP);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterSetOutputResolution (IntPtr printer, IntPtr printSettings, ref PMResolution resolutionP);

		public PMResolution GetOutputResolution (PMPrintSettings settings)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));

			if (PMPrinterGetOutputResolution (Handle, settings.Handle, out var res) == PMStatusCode.Ok)
				return res;
			return new PMResolution (0, 0);
		}

		public void SetOutputResolution (PMPrintSettings settings, PMResolution res)
		{
			if (settings is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (settings));
			PMPrinterSetOutputResolution (Handle, settings.Handle, ref res);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterSetDefault (IntPtr printer);

		public PMStatusCode SetDefault ()
		{
			return PMPrinterSetDefault (Handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsFavorite (IntPtr printer);
		public bool IsFavorite => PMPrinterIsFavorite (Handle) != 0;

		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsDefault (IntPtr printer);
		public bool IsDefault => PMPrinterIsDefault (Handle) != 0;

		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsPostScriptCapable (IntPtr printer);
		public bool IsPostScriptCapable => PMPrinterIsPostScriptCapable (Handle) != 0;

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterIsPostScriptPrinter (IntPtr printer, out byte isps);
		public bool IsPostScriptPrinter {
			get {
				if (PMPrinterIsPostScriptPrinter (Handle, out var r) == PMStatusCode.Ok)
					return r != 0;
				return false;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterIsRemote (IntPtr printer, out byte isrem);
		public bool IsRemote {
			get {
				if (PMPrinterIsRemote (Handle, out var r) == PMStatusCode.Ok)
					return r != 0;
				return false;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterGetID (IntPtr printer);

		public string? Id {
			get {
				return CFString.FromHandle (PMPrinterGetID (Handle));
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterCopyHostName (IntPtr printer, out IntPtr hostName);

		public string? HostName {
			get {
				PMStatusCode code = PMPrinterCopyHostName (Handle, out var hostName);
				if (code != PMStatusCode.Ok)
					return null;
				return CFString.FromHandle (hostName, true);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("macos")]
#endif
	public class PMServer : PMPrintCoreBase {
		// A private constructor so that nobody can create an instance of this class.
		PMServer ()
			: base (IntPtr.Zero, true)
		{
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMServerLaunchPrinterBrowser (IntPtr server, IntPtr dictFutureUse);

		public static PMStatusCode LaunchPrinterBrowser ()
		{
			return PMServerLaunchPrinterBrowser (IntPtr.Zero /* Server Local */, IntPtr.Zero);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMServerCreatePrinterList (IntPtr server, out IntPtr printerListArray);
		public static PMStatusCode CreatePrinterList (out PMPrinter []? printerList)
		{
			var code = PMServerCreatePrinterList (IntPtr.Zero /* ServerLocal */, out var arr);
			if (code != PMStatusCode.Ok) {
				printerList = null;
				return code;
			}
			printerList = CFArray.ArrayFromHandleFunc<PMPrinter> (arr, (handle) => new PMPrinter (handle, false), true);
			return PMStatusCode.Ok;
		}
	}
}
