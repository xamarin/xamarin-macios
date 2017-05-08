// copied from MonoTouch mtouch code
// Copyright 2011-2013, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

namespace Xamarin.Bundler {
	public class MonoMacException : Exception {
		
		public MonoMacException (int code, string message, params object[] args) : 
			this (code, false, message, args)
		{
		}

		public MonoMacException (int code, bool error, string message, params object[] args) : 
			this (code, error, null, message, args)
		{
		}

		public MonoMacException (int code, bool error, Exception innerException, string message, params object[] args) : 
			base (String.Format (message, args), innerException)
		{
			Code = code;
			Error = error;
		}

		public string FileName { get; set; }

		public int LineNumber { get; set; }

		public int Code { get; private set; }
		
		public bool Error { get; private set; }
		
		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public override string ToString ()
		{
			if (string.IsNullOrEmpty (FileName)) {
				return String.Format ("{0} MM{1:0000}: {2}", Error ? "error" : "warning", Code, Message);
			} else {
				return String.Format ("{3}({4}): {0} MM{1:0000}: {2}", Error ? "error" : "warning", Code, Message, FileName, LineNumber);
			}
		}
	}
}
