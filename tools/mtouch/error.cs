// Copyright 2011-2012, Xamarin Inc. All rights reserved,

using System;
using System.Reflection;

namespace Xamarin.Bundler {

	public class MonoTouchException : Exception {
		//[Obsolete("localize this")]
		public MonoTouchException (int code, params object[] args) : 
			this (code, false, args)
		{
		}
	//	[Obsolete("localize this")]
		public MonoTouchException (int code, bool error, params object[] args) : 
			this (code, error, null, args)
		{
		}
	//	[Obsolete("localize this")]
		public MonoTouchException (int code, bool error, Exception innerException, params object[] args) : 
			base (String.Format (GetMessage(code), args), innerException)
		{
			Code = code;
			Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
		}

		static string GetMessage (int code)
		{
			Type resourceType = Type.GetType ("mtouch.Errors");
			string errorCode = string.Format ("MT{0:0000}", code);
			PropertyInfo prop = resourceType.GetProperty (errorCode, BindingFlags.NonPublic |
					BindingFlags.Static |
					BindingFlags.GetProperty);

			var errorMessage = prop == null ? String.Format (mtouch.Errors._default, errorCode) :
						(String) prop.GetValue (null);  
			return errorMessage;
		}

		public string FileName { get; set; }

		public int LineNumber { get; set; }

		public int Code { get; private set; }
		
		public bool Error { get; private set; }
		
		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		public override string ToString ()
		{
			if (string.IsNullOrEmpty (FileName)) {
				return String.Format ("{0} MT{1:0000}: {2}", Error ? "error" : "warning", Code, Message);
			} else {
				return String.Format ("{3}({4}): {0} MT{1:0000}: {2}", Error ? "error" : "warning", Code, Message, FileName, LineNumber);
			}
		}
	}
}
