using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System;

#if MONOMAC
using AppKit;
#else
using UIKit;
using NSView = AppKit.UIView;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace PrintCore {

	[Mac (13,0)]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PDEPanel
	{
		[Abstract]
		[Export ("willShow")]
		void WillShow ();

		[Abstract]
		[Export ("shouldHide")]
		bool ShouldHide { get; }

		[Abstract]
		[Export ("saveValuesAndReturnError:")]
		bool SaveValuesAndReturnError ([NullAllowed] out NSError error);

		[Abstract]
		[Export ("restoreValuesAndReturnError:")]
		bool RestoreValuesAndReturnError ([NullAllowed] out NSError error);

		[NullAllowed, Export ("supportedPPDOptionKeys")]
		string[] SupportedPpdOptionKeys { get; }

		[Abstract]
		[Export ("PPDOptionKeyValueDidChange:ppdChoice:")]
		void PpdOptionKeyValueDidChange (string option, string choice);

		[Abstract]
		[NullAllowed, Export ("panelView")]
		NSView PanelView { get; }

		[Abstract]
		[Export ("panelName")]
		string PanelName { get; }

		[Abstract]
		[Export ("panelKind")]
		string PanelKind { get; }

		[Abstract]
		[NullAllowed, Export ("summaryInfo")]
		NSDictionary<NSString, NSString> SummaryInfo { get; }

		[Export ("shouldShowHelp")]
		bool ShouldShowHelp { get; }

		[Export ("shouldPrint")]
		bool ShouldPrint { get; }

		[Export ("printWindowWillClose:")]
		void PrintWindowWillClose (bool userCanceled);
	}

	[Mac (13,0)]
	[Protocol]
	[BaseType (typeof (NSObject))]
	interface PDEPlugIn
	{
		[Export ("initWithBundle:")]
		NativeHandle Constructor (NSBundle bundle);

		[Abstract]
		[Export ("PDEPanelsForType:withHostInfo:")]
		[return: NullAllowed]
		PDEPanel[] PDEPanelsForType (string pdeType, IPDEPlugInCallbackProtocol host);
	}

	interface IPDEPlugInCallbackProtocol { }

	[Mac (13,0)]
	[Protocol]
	interface PDEPlugInCallbackProtocol
	{
		[Abstract]
		[Internal]
		[Export ("printSession")]
		// Original: unsafe PMPrintSession* PrintSession { get; }
		IntPtr _GetPrintSession ();

		[Abstract]
		[Internal]
		[Export ("printSettings")]
		// Original: unsafe PMPrintSession* PrintSettings { get; }
		IntPtr _GetPrintSettings ();

		[Abstract]
		[Internal]
		[Export ("pageFormat")]
		// Original: unsafe PMPageFormat* PageFormat { get; }
		IntPtr _GetPageFormat ();

		[Abstract]
		[Internal]
		[Export ("PMPrinter")]
		// Original: unsafe PMPrinter* PMPrinter { get; }
		IntPtr _GetPMPrinter ();

		[Abstract]
		[Internal]
		[Export ("ppdFile")]
		// Original: unsafe ppd_file_s* PpdFile { get; }
		IntPtr _GetPpdFile ();

		[Abstract]
		[Export ("willChangePPDOptionKeyValue:ppdChoice:")]
		bool PpdChoice (string option, string choice);
	}
}
