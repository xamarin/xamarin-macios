using System;
using XamCore.PrintCore;

namespace XamCore.AppKit {
	public partial class NSPrintInfo {
		public PMPrintSession PMPrintSession {
			get {
				var ptr = GetPMPrintSession ();
				return new PMPrintSession (ptr, false);
			}
		}

		public PMPageFormat PMPageFormat {
			get {
				var ptr = GetPMPageFormat ();
				return new PMPageFormat (ptr, false);
			}
		}

		public PMPrintSettings PMPrintSettings {
			get {
				var ptr = GetPMPrintSettings ();
				return new PMPrintSettings (ptr, false);
			}
		}
	}
}
