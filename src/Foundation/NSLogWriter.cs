//
// Helper for Console to allow indirect access to `stdout` using NSLog
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Foundation {

	// this is created, by reflection, when using Console (static ctor)
	// the linker will include this type is Console is used anywhere in the app
	class NSLogWriter : TextWriter {
		
		[DllImport (Constants.FoundationLibrary)]
		extern static void NSLog (IntPtr format, IntPtr s);
		
		static NSString format = new NSString ("%@");
		
		StringBuilder sb;
		
		public NSLogWriter ()
		{
			sb = new StringBuilder ();
		}
		
		public override System.Text.Encoding Encoding {
			get { return System.Text.Encoding.UTF8; }
		}

		public override void Flush ()
		{
			try {
				using (var ns = new NSString (sb.ToString ()))
					NSLog (format.Handle, ns.Handle);
				sb.Length = 0;
			}
			catch (Exception) {
			}
		}

		// minimum to override - see http://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
		public override void Write (char value)
		{
			try {
				sb.Append (value);
			}
			catch (Exception) {
			}
		}
		
		// optimization (to avoid concatening chars)
		public override void Write (string value)
		{
			try {
				sb.Append (value);
				if (value != null && value.Length >= CoreNewLine.Length && EndsWithNewLine (value))
					Flush ();
			}
			catch (Exception) {
			}
		}

		bool EndsWithNewLine (string value)
		{
			for (int i = 0, v = value.Length - CoreNewLine.Length; i < CoreNewLine.Length; ++i, ++v) {
				if (value [v] != CoreNewLine [i])
					return false;
			}

			return true;
		}
		
		public override void WriteLine ()
		{
			try {
				Flush ();
			}
			catch (Exception) {
			}
		}
	}
}