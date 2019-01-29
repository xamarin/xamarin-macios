//
// PrintCore.cs: PrintCore bindings
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2016 Microsoft Inc
//
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
using CoreFoundation;
using PMObject=System.IntPtr;
using OSStatus=System.Int32;

namespace PrintCore {
	class NativeInvoke {
		[DllImport (Constants.PrintCoreLibrary)]
		internal extern static OSStatus PMRetain (PMObject obj);

		[DllImport (Constants.PrintCoreLibrary)]
		internal extern static OSStatus PMRelease (PMObject obj);

		
	}
	
	public class PMPrintCoreBase : IDisposable, INativeObject {
		internal IntPtr handle;

		internal PMPrintCoreBase ()
		{
			// For delayed initialization cases.
		}
		
		internal PMPrintCoreBase (IntPtr handle) : this (handle, false) {}
		internal PMPrintCoreBase (IntPtr handle, bool owns)
		{
			if (!owns)
				NativeInvoke.PMRetain (handle);
			this.handle = handle;
		}

		~PMPrintCoreBase ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle => handle;

		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				NativeInvoke.PMRelease (handle);
				handle = IntPtr.Zero;
			}
		}
	}

	public class PMPrintException : Exception {
		public PMPrintException (PMStatusCode code) : base (code.ToString ()){}
	}
	
	public class PMPrintSession : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreateSession (out IntPtr session);

		internal PMPrintSession (IntPtr handle, bool owns) : base (handle, owns) {}
		
		public PMPrintSession ()
		{
			IntPtr value;
			var code = PMCreateSession (out value);
			if (code == PMStatusCode.Ok)
				handle = value;
			else
				throw new PMPrintException (code);
		}

		public static PMStatusCode TryCreate (out PMPrintSession session)
		{
			IntPtr value;
			var code = PMCreateSession (out value);
			if (code == PMStatusCode.Ok){
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
				return PMSessionError (handle);
			}
			set {
				PMSessionSetError (handle, value);
			}
		}
		
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionDefaultPrintSettings (IntPtr session, IntPtr settings);

		public void AssignDefaultSettings (PMPrintSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));
			PMSessionDefaultPrintSettings (handle, settings.handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionDefaultPageFormat (IntPtr session, IntPtr pageFormat);
		
		public void AssignDefaultPageFormat (PMPageFormat pageFormat)
		{
			if (pageFormat == null)
				throw new ArgumentNullException (nameof (pageFormat));
			PMSessionDefaultPageFormat (handle, pageFormat.Handle);
		}
		
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSessionCreatePrinterList (IntPtr printSession, out IntPtr printerListArray, out int index, out IntPtr printer);

		internal static string [] FetchArray (IntPtr cfArrayString, bool owns)
		{
			string [] ret;
			// Fetc the values from the array that we own.
			var arr = new CFArray (cfArrayString, owns: owns);
			int c = (int) arr.Count;
			ret = new string [c];
			for (int i = 0; i < c; i++)
				ret [i] = CFString.FetchString (arr.GetValue (i));
			arr.Dispose ();

			return ret;
		}
		
		public PMStatusCode CreatePrinterList (out string [] printerList, out int index, out PMPrinter printer)
		{
			IntPtr array, printerHandle;
			var code = PMSessionCreatePrinterList (handle, out array, out index, out printerHandle);
			if (code != PMStatusCode.Ok){
				printerList = null;
				printer = null;
				return code;
			}

			printerList = NSArray.StringArrayFromHandle (array);
			CFObject.CFRelease (array);
			if (printerHandle != IntPtr.Zero){
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
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));
			
			byte c;
			var code = PMSessionValidatePrintSettings (handle, settings.handle, out c);
			if (code != PMStatusCode.Ok){
				changed = false;
				return code;
			}
			changed = c != 0;
			return code;
		}
	}

	public class PMPrintSettings : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePrintSettings (out IntPtr session);
		
		internal PMPrintSettings (IntPtr handle, bool owns) : base (handle, owns) {}
		public PMPrintSettings ()
		{
			IntPtr value;
			var code = PMCreatePrintSettings (out value);
			if (code == PMStatusCode.Ok)
				handle = value;
			else
				throw new PMPrintException (code);
		}

		public static PMStatusCode TryCreate (out PMPrintSettings settings)
		{
			IntPtr value;
			var code = PMCreatePrintSettings (out value);
			if (code == PMStatusCode.Ok){
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
				uint val = 0;
				PMGetFirstPage (handle, out val);
				return val;
			}
			set {
				PMSetFirstPage (handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetLastPage (IntPtr handle, out uint last);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetLastPage (IntPtr handle, uint last, byte lockb);
		public uint LastPage {
			get {
				uint val = 0;
				PMGetLastPage (handle, out val);
				return val;
			}
			set {
				PMSetLastPage (handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetPageRange (IntPtr handle, out uint minPage, out uint maxPage);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetPageRange (IntPtr handle, uint minPage, uint maxPage);
		public PMStatusCode GetPageRange (out uint minPage, out uint maxPage)
		{
			return PMGetPageRange (handle, out minPage, out maxPage);
		}

		public PMStatusCode SetPageRange (uint minPage, uint maxPage)
		{
			return PMSetPageRange (handle, minPage, maxPage);
		}
		
			
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCopyPrintSettings (IntPtr source, IntPtr dest);
		
		public PMStatusCode CopySettings (PMPrintSettings destination)
		{
			if (destination == null)
				throw new ArgumentNullException (nameof (destination));
			return PMCopyPrintSettings (handle, destination.handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetCopies (IntPtr handle, out uint copies);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetCopies (IntPtr handle, uint copies, byte elock);

		public uint Copies {
			get {
				uint c;
				if (PMGetCopies (handle, out c) == PMStatusCode.Ok)
					return c;
				else
					return 0;
			}
			set {
				PMSetCopies (handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetCollate (IntPtr handle, out byte collate);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetCollate (IntPtr handle, byte collate);

		public bool Collate {
			get {
				byte c;
				if (PMGetCollate (handle, out c) == PMStatusCode.Ok)
					return c != 0;
				else
					return false;
			}
			set {
				PMSetCollate (handle, (byte) (value ? 1 : 0));
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetDuplex (IntPtr handle, out PMDuplexMode mode);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetDuplex (IntPtr handle, PMDuplexMode mode);

		public PMDuplexMode DuplexMode {
			get {
				PMDuplexMode c;
				if (PMGetDuplex (handle, out c) == PMStatusCode.Ok)
					return c;
				else
					return PMDuplexMode.None;
			}
			set {
				PMSetDuplex (handle, value);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetScale (IntPtr handle, out double mode);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMSetScale (IntPtr handle, double scale);

		public double Scale {
			get {
				double c;
				if (PMGetScale (handle, out c) == PMStatusCode.Ok)
					return c;
				else
					return 100;
			}
			set {
				PMSetScale (handle, value);
			}
		}
		
	}

	public class PMPageFormat : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePageFormat (out IntPtr handle);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreatePageFormatWithPMPaper (out IntPtr handle, IntPtr paper);

		internal PMPageFormat (IntPtr handle, bool owns): base (handle, owns) {}
		
		public PMPageFormat (PMPaper paper = null)
		{
			IntPtr value;
			PMStatusCode code;
			if (paper == null)
				code = PMCreatePageFormat (out value);
			else
				code = PMCreatePageFormatWithPMPaper (out value, paper.Handle);
			
			if (code == PMStatusCode.Ok)
				handle = value;
			else
				throw new PMPrintException (code);
		}

		public static PMStatusCode TryCreate (out PMPageFormat pageFormat, PMPaper paper = null)
		{
			PMStatusCode code;
			IntPtr value;
			
			if (paper == null)
				code = PMCreatePageFormat (out value);
			else
				code = PMCreatePageFormatWithPMPaper (out value, paper.Handle);
			
			if (code == PMStatusCode.Ok){
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
				PMOrientation o;
				if (PMGetOrientation (handle, out o) == PMStatusCode.Ok)
					return o;
				else
					return PMOrientation.Portrait;
			}
			set {
				PMSetOrientation (handle, value, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetAdjustedPageRect (IntPtr pageFormat, out PMRect pageRect);
		public PMRect AdjustedPageRect {
			get {
				PMRect rect;
				if (PMGetAdjustedPageRect (handle, out rect) == PMStatusCode.Ok)
					return rect;
				return new PMRect (0, 0, 0, 0);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMGetAdjustedPaperRect (IntPtr pageFormat, out PMRect pageRect);
		public PMRect AdjustedPaperRect {
			get {
				PMRect rect;
				if (PMGetAdjustedPaperRect (handle, out rect) == PMStatusCode.Ok)
					return rect;
				return new PMRect (0, 0, 0, 0);
			}
		}
	}

	public class PMPaper : PMPrintCoreBase {
		internal PMPaper (IntPtr handle, bool owns) : base (handle, owns) {}
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
		
		public string ID {
			get {
				IntPtr s;
				var code = PMPaperGetID (handle, out s);
				if (code != PMStatusCode.Ok)
					return null;
				return CFString.FetchString (s);
			}
		}

		public double Width {
			get {
				double s; 
				var code = PMPaperGetWidth (handle, out s);
				if (code != PMStatusCode.Ok)
					return 0;
				return s;
			}
		}
			
		public double Height {
			get {
				double s; 
				var code = PMPaperGetHeight (handle, out s);
				if (code != PMStatusCode.Ok)
					return 0;
				return s;
			}
		}

		public PMPaperMargins? Margins {
			get {
				PMPaperMargins margins;
				var code = PMPaperGetMargins (handle, out margins);
				if (code != PMStatusCode.Ok)
					return null;
				return margins;
			}
		}

		public string GetLocalizedName (PMPrinter printer)
		{
			if (printer == null)
				throw new ArgumentNullException (nameof (printer));
			IntPtr name;
			var code = PMPaperCreateLocalizedName (handle, printer.handle, out name);
			if (code != PMStatusCode.Ok)
				return null;
			var str = CFString.FetchString (name);
			CFObject.CFRelease (name);
			return str;
		}
		
	}

	public class PMPrinter : PMPrintCoreBase {
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMCreateGenericPrinter (out IntPtr session);
		
		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterCreateFromPrinterID (IntPtr id);

		internal PMPrinter (IntPtr handle, bool owns) : base (handle, owns) {}
		public PMPrinter ()
		{
			IntPtr value;
			var code = PMCreateGenericPrinter (out value);
			if (code == PMStatusCode.Ok)
				handle = value;
			else
				throw new PMPrintException (code);
		}

		public PMPrinter (string printerId)
		{
			using (var idf = new CFString (printerId)){
				var value = PMPrinterCreateFromPrinterID (idf.Handle);
				if (value == IntPtr.Zero)
					throw new PMPrintException (PMStatusCode.InvalidPrinter);
					
				handle = value;
			}
		}
		
		public static PMStatusCode TryCreate (out PMPrinter printer)
		{
			IntPtr value;
			var code = PMCreateGenericPrinter (out value);
			if (code == PMStatusCode.Ok){
				printer = new PMPrinter (value, owns: true);
				return PMStatusCode.Ok;
			}
			printer = null;
			return code;
		}

		public static PMPrinter TryCreate (string printerId)
		{
			using (var idf = new CFString (printerId)){
				var h = PMPrinterCreateFromPrinterID (idf.Handle);
				if (h == IntPtr.Zero)
					return null;
				return new PMPrinter (h, owns: true);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterGetName (IntPtr handle);
		public string Name => CFString.FetchString (PMPrinterGetName (handle));

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterCopyDeviceURI (IntPtr handle, out IntPtr url);

		public PMStatusCode TryGetDeviceUrl (out NSUrl url)
		{
			IntPtr urlH;
			
			var code = PMPrinterCopyDeviceURI (handle, out urlH);
			if (code != PMStatusCode.Ok){
				url = null;
				return code;
			}
			url = Runtime.GetNSObject<NSUrl> (urlH, true);
			return PMStatusCode.Ok;
		}

		public NSUrl DeviceUrl {
			get {
				IntPtr urlH;
			
				var code = PMPrinterCopyDeviceURI (handle, out urlH);
				if (code != PMStatusCode.Ok)
					return null;

				return Runtime.GetNSObject<NSUrl> (urlH, true);
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetMakeAndModelName(IntPtr printer, out IntPtr makeAndModel);

		public string MakeAndModel {
			get {
				IntPtr v;
				if (PMPrinterGetMakeAndModelName (handle, out v) == PMStatusCode.Ok){
					return CFString.FetchString (v);
				}
				return null;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetState(IntPtr printer, out PMPrinterState state);

		// Return is overloaded - if negative, a PMStatusCode.
		public PMPrinterState PrinterState {
			get {
				PMPrinterState s;
				var code = PMPrinterGetState (handle, out s);
				if (code == PMStatusCode.Ok)
					return s;

				return (PMPrinterState) code;
			}
		}
		
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetMimeTypes (IntPtr printer, IntPtr settings, out IntPtr arrayStr);

		public PMStatusCode TryGetMimeTypes (PMPrintSettings settings, out string [] mimeTypes)
		{
			IntPtr m;
			var code = PMPrinterGetMimeTypes (handle, settings == null ? IntPtr.Zero : settings.Handle, out m);
			if (code != PMStatusCode.Ok){
				mimeTypes = null;
				return code;
			}
			mimeTypes = NSArray.StringArrayFromHandle (m);
			return PMStatusCode.Ok;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetPaperList (IntPtr printer, out IntPtr arrayStr);
		public PMStatusCode TryGetPaperList (out PMPaper [] paperList)
		{
			IntPtr m;
			var code = PMPrinterGetPaperList (handle, out m);
			if (code != PMStatusCode.Ok){
				paperList = null;
				return code;
			}
			int c = (int) CFArray.GetCount (m);
			paperList = new PMPaper [c];
			for (int i = 0; i < c; i++)
				paperList [i] = new PMPaper (CFArray.CFArrayGetValueAtIndex (m, i), owns: false);

			return PMStatusCode.Ok;
		}

		public PMPaper [] PaperList {
			get {
				IntPtr m;
				if (PMPrinterGetPaperList (handle, out m) != PMStatusCode.Ok)
					return new PMPaper [0];

				int c = (int) CFArray.GetCount (m);
				var paperList = new PMPaper [c];
				for (int i = 0; i < c; i++)
					paperList [i] = new PMPaper (CFArray.CFArrayGetValueAtIndex (m, i), owns: false);

				return paperList;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterPrintWithFile (IntPtr handle, IntPtr settings, IntPtr pageFormat, IntPtr strMimeType, IntPtr fileUrl);

		public PMStatusCode TryPrintFile (PMPrintSettings settings, PMPageFormat pageFormat, NSUrl fileUrl, string mimeType = null)
		{
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));
			if (fileUrl == null)
				throw new ArgumentNullException (nameof (fileUrl));
				    
			IntPtr mime = CFString.LowLevelCreate (mimeType);
			var code = PMPrinterPrintWithFile (handle, settings.handle, pageFormat == null ? IntPtr.Zero : pageFormat.handle, mime, fileUrl.Handle);
			if (mime != IntPtr.Zero)
				CFObject.CFRelease (mime);
			return code;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterPrintWithProvider (IntPtr printer, IntPtr settings, IntPtr pageFormat, IntPtr strMimeType, IntPtr cgDataProvider);

		public PMStatusCode TryPrintFromProvider (PMPrintSettings settings, PMPageFormat pageFormat, CGDataProvider provider, string mimeType = null)
		{
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));
			if (provider == null)
				throw new ArgumentNullException (nameof (provider));
				    
			IntPtr mime = CFString.LowLevelCreate (mimeType);
			var code = PMPrinterPrintWithProvider (handle, settings.handle, pageFormat == null ? IntPtr.Zero : pageFormat.handle, mime, provider.Handle);
			if (mime != IntPtr.Zero)
				CFObject.CFRelease (mime);
			return code;
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterGetOutputResolution (IntPtr printer, IntPtr printSettings, out PMResolution resolutionP);
		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterSetOutputResolution (IntPtr printer, IntPtr printSettings, ref PMResolution resolutionP);

		public PMResolution GetOutputResolution (PMPrintSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));

			PMResolution res;
			if (PMPrinterGetOutputResolution (handle, settings.Handle, out res) == PMStatusCode.Ok)
				return res;
			return new PMResolution (0, 0);
		}

		public void SetOutputResolution (PMPrintSettings settings, PMResolution res)
		{
			if (settings == null)
				throw new ArgumentNullException (nameof (settings));
			PMPrinterSetOutputResolution (handle, settings.Handle, ref res);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterSetDefault (IntPtr printer);

		public PMStatusCode SetDefault ()
		{
			return PMPrinterSetDefault (handle);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsFavorite (IntPtr printer);
		public bool IsFavorite => PMPrinterIsFavorite (handle) != 0;
		
		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsDefault (IntPtr printer);
		public bool IsDefault => PMPrinterIsDefault (handle) != 0;

		[DllImport (Constants.PrintCoreLibrary)]
		extern static byte PMPrinterIsPostScriptCapable (IntPtr printer);
		public bool IsPostScriptCapable => PMPrinterIsPostScriptCapable (handle) != 0;

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterIsPostScriptPrinter  (IntPtr printer, out byte isps);
		public bool IsPostScriptPrinter {
			get {
				byte r;
				if (PMPrinterIsPostScriptPrinter (handle, out r) == PMStatusCode.Ok)
					return r != 0;
				return false;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterIsRemote  (IntPtr printer, out byte isrem);
		public bool IsRemote {
			get {
				byte r;
				if (PMPrinterIsRemote (handle, out r) == PMStatusCode.Ok)
					return r != 0;
				return false;
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static IntPtr PMPrinterGetID (IntPtr printer);

		public string Id {
			get {
				return CFString.FetchString (PMPrinterGetID (handle));
			}
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMPrinterCopyHostName (IntPtr printer, out IntPtr hostName);

		public string HostName {
			get {
				PMStatusCode code = PMPrinterCopyHostName (handle, out IntPtr hostName);
				if (code != PMStatusCode.Ok)
					return null;
				return CFString.FetchString (hostName, true);
			}
		}
	}

	public class PMServer : PMPrintCoreBase {
		PMServer () {}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMServerLaunchPrinterBrowser (IntPtr server, IntPtr dictFutureUse);

		public static PMStatusCode LaunchPrinterBrowser ()
		{
			return PMServerLaunchPrinterBrowser (IntPtr.Zero /* Server Local */, IntPtr.Zero);
		}

		[DllImport (Constants.PrintCoreLibrary)]
		extern static PMStatusCode PMServerCreatePrinterList (IntPtr server, out IntPtr printerListArray);
		public static PMStatusCode CreatePrinterList (out PMPrinter [] printerList)
		{
			IntPtr arr;
			var code = PMServerCreatePrinterList (IntPtr.Zero /* ServerLocal */, out arr);
			if (code != PMStatusCode.Ok){
				printerList = null;
				return code;
			}
			int c = (int) CFArray.GetCount (arr);
			printerList = new PMPrinter [c];
			for (int i = 0; i < c; i++)
				printerList [i] = new PMPrinter (CFArray.CFArrayGetValueAtIndex (arr, i), owns: false);

			CFObject.CFRelease (arr);
			return PMStatusCode.Ok;
		}
	}
}
