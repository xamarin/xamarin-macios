#nullable enable

#if MONOMAC

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
// TODO add reference to BindingHelpers
using BindingHelpers;

namespace PrintCore {

	public partial class PDEPlugInCallbackProtocol {

		public PMPrintSession? PrintSession {
			get => BindingHelpers.GetPtrToStruct<PMPrintSession> (_GetPrintSession ());
		}

		public PMPrintSession? PrintSettings {
			get => BindingHelpers.GetPtrToStruct<PMPrintSession> (_GetPrintSettings ());
		}
		
		public PMPageFormat? PageFormat {
			get => BindingHelpers.GetPtrToStruct<PMPageFormat> (_GetPageFormat ());
		}
		
		public PMPrinter? PMPrinter {
			get => BindingHelpers.GetPtrToStruct<PMPrinter> (_GetPMPrinter ());
		}
		
		// TODO Figure out how to make the ppd_file_s: https://gist.github.com/tj-devel709/ef051c6750fb304d826f58059d2c1acd#file-ppd-h-L286
		// public ppd_file_s? PpdFile {
		// 	get => BindingHelpers.GetPtrToStruct<ppd_file_s> (_GetPpdFile ());
		// }
	}
}

#endif
