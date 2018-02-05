using System;
using PrintCore;

namespace AppKit {
	public partial class NSPrintInfo {
		public PMPrintSession GetPrintSession ()
		{
			var ptr = GetPMPrintSession ();
			return new PMPrintSession (ptr, false);
		}

		public PMPageFormat GetPageFormat ()
		{
			var ptr = GetPMPageFormat ();
			return new PMPageFormat (ptr, false);
		}

		public PMPrintSettings GetPrintSettings ()
		{
			var ptr = GetPMPrintSettings ();
			return new PMPrintSettings (ptr, false);
		}
	}
}
