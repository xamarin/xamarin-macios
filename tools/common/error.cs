// Copyright 2011-2013, Xamarin Inc. All rights reserved,
// Copyright 2020 Microsoft Corp

using System;
using System.Text;

namespace Xamarin.Bundler {

#if MTOUCH || MONOTOUCH
	public class MonoTouchException : Exception {
		public const string Prefix = "MT";

		public MonoTouchException (int code, string message) :
			this (code, false, message)
		{
		}

		public MonoTouchException (int code, string message, params object[] args) : 
			this (code, false, message, args)
		{
		}

		public MonoTouchException (int code, bool error, string message) :
			this (code, error, null, message)
		{
		}

		public MonoTouchException (int code, bool error, string message, params object[] args) : 
			this (code, error, null, message, args)
		{
		}

		public MonoTouchException (int code, bool error, Exception innerException, string message) :
			base (message, innerException)
		{
			SetValues (code, error);
		}

		public MonoTouchException (int code, bool error, Exception innerException, string message, params object[] args) : 
			base (Format (message, args), innerException)
		{
			SetValues (code, error);
		}
#else
	public class MonoMacException : Exception {
		public const string Prefix = "MM";

		public MonoMacException (int code, string message) :
			this (code, false, message)
		{
		}

		public MonoMacException (int code, string message, params object[] args) :
			this (code, false, message, args)
		{
		}

		public MonoMacException (int code, bool error, string message) :
			this (code, error, null, message)
		{
		}

		public MonoMacException (int code, bool error, string message, params object[] args) :
			this (code, error, null, message, args)
		{
		}

		public MonoMacException (int code, bool error, Exception innerException, string message) :
			base (message, innerException)
		{
			SetValues (code, error);
		}

		public MonoMacException (int code, bool error, Exception innerException, string message, params object[] args) :
			base (Format (message, args), innerException)
		{
			SetValues (code, error);
		}
#endif

		public string FileName { get; set; }

		public int LineNumber { get; set; }

		public int Code { get; private set; }
		
		public bool Error { get; private set; }

		void SetValues (int code, bool error)
		{
			Code = code;
			Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
		}

		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public override string ToString ()
		{
			var sb = new StringBuilder ();
			if (!String.IsNullOrEmpty (FileName))
				sb.Append (FileName).Append ('(').Append (LineNumber).Append ("): ");

			sb.Append (Error ? "error" : "warning").Append (' ').Append (Prefix).Append (Code.ToString ("0000: ")).Append (Message);
			return sb.ToString ();
		}

		// this is to avoid having error message that can throw FormatException since they hide the real issue
		// so we need to fix it (first) before we can get the underlying root issue
		// this is getting more complex with localization since the strings (format) are not in the source itself
		static string Format (string message, params object[] args)
		{
			try {
				return String.Format (message, args);
			}
			catch {
				var sb = new StringBuilder (message);
				sb.Append (". String.Format failed! Arguments were:");
				if (args != null) {
					foreach (var arg in args) {
						sb.Append (" \"").Append (arg).Append ('"');
					}
				}
				sb.Append (". Please file an issue to report this incorrect error handling.");
				return sb.ToString ();
			}
		}
	}
}
