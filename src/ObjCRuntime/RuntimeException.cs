// Copyright 2011-2012, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

#nullable enable

namespace ObjCRuntime {
	public class RuntimeException : Exception {
		public RuntimeException (string message, params object? [] args)
			: base (string.Format (message, args))
		{
		}

		public RuntimeException (int code, string message, params object? [] args) :
			this (code, false, null, message, args)
		{
		}

		public RuntimeException (int code, bool error, string message, params object? [] args) :
			this (code, error, null, message, args)
		{
		}

		public RuntimeException (int code, bool error, Exception? innerException, string message, params object? [] args) :
			base (String.Format (message, args), innerException)
		{
			Code = code;
			Error = error;
		}

		/// <summary>The error code for the condition that triggered the exception.</summary>
		///         <value>The error code for the condition that triggered the exception.</value>
		///         <remarks>
		///         </remarks>
		public int Code { get; private set; }

		/// <summary>If this exception represents an error or a warning.</summary>
		///         <value>If this exception represents an error or a warning.</value>
		///         <remarks>
		///         </remarks>
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
}
