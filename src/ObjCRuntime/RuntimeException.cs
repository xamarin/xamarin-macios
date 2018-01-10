// Copyright 2011-2012, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

#if XAMCORE_2_0
namespace ObjCRuntime {
#endif

	public class RuntimeException : Exception {
		public RuntimeException (string message, params object[] args)
			: base (string.Format (message, args))
		{
		}

		public RuntimeException (int code, string message, params object[] args) : 
			this (code, false, message, args)
		{
		}
		
		public RuntimeException (int code, bool error, string message, params object[] args) : 
			this (code, error, null, message, args)
		{
		}
		
		public RuntimeException (int code, bool error, Exception innerException, string message, params object[] args) : 
			base (String.Format (message, args), innerException)
		{
			Code = code;
			Error = error;
		}
		
		public int Code { get; private set; }
		
		public bool Error { get; private set; }
		
		// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
		/*
		public override string ToString ()
		{
			return String.Format ("{0} MT{1:0000}: {2}",
			                      Error ? "error" : "warning", Code, Message);
		}
		*/
	}
#if XAMCORE_2_0
}
#endif
