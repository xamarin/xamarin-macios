//
// error.cs: Error handling code for bmac/btouch
//
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com
//   Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin, Inc.
//
//

using System;
using System.Collections.Generic;

using ProductException=BindingException;

// Error allocation: the errors are listed (and documented) in $(TOP)/docs/website/generator-errors.md

public class BindingException : Exception {
	
	public BindingException (int code, string message, params object[] args) : 
		this (code, false, message, args)
	{
	}

	public BindingException (int code, bool error, string message, params object[] args) : 
		this (code, error, null, message, args)
	{
	}

	public BindingException (int code, bool error, Exception innerException, string message, params object[] args) : 
		base (String.Format (message, args), innerException)
	{
		Code = code;
		Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
	}

	public int Code { get; private set; }
	
	public bool Error { get; private set; }
	
	// http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
	public override string ToString ()
	{
		 return String.Format ("{0} BI{1:0000}: {3}: {2}",
			Error ? "error" : "warning", Code, Message, BindingTouch.ToolName);
	}
}

public static class ErrorHelper {

	public enum WarningLevel {
		Error = -1,
		Warning = 0,
		Disable = 1,
	}
	
	static public int Verbosity { get; set; }
	static Dictionary<int, WarningLevel> warning_levels;
	
	public static ProductException CreateError (int code, string message, params object[] args)
	{
		return new ProductException (code, true, message, args);
	}

	static public void Show (Exception e)
	{
		List<Exception> exceptions = new List<Exception> ();
		bool error = false;

		CollectExceptions (e, exceptions);

		foreach (var ex in exceptions)
			error |= ShowInternal (ex);

		if (error)
			Environment.Exit (1);
	}

	static void CollectExceptions (Exception ex, List<Exception> exceptions)
	{
#if NET_4_0
		AggregateException ae = ex as AggregateException;

		if (ae != null) {
			foreach (var ie in ae.InnerExceptions)
				CollectExceptions (ie, exceptions);
		} else {
			exceptions.Add (ex);
		}
#else
		exceptions.Add (ex);
#endif
	}

	static bool ShowInternal (Exception e)
	{
		BindingException mte = (e as BindingException);
		bool error = true;

		if (mte != null) {
			error = mte.Error;

			if (!error && GetWarningLevel (mte.Code) == WarningLevel.Disable)
				return false;

			Console.Out.WriteLine (mte.ToString ());
			
			if (Verbosity > 1) {
				Exception ie = e.InnerException;
				if (ie != null) {
					if (Verbosity > 3) {
						Console.Error.WriteLine ("--- inner exception");
						Console.Error.WriteLine (ie);
						Console.Error.WriteLine ("---");
					} else {
						Console.Error.WriteLine ("\t{0}", ie.Message);
					}
				}
			}
			
			if (Verbosity > 2)
				Console.Error.WriteLine (e.StackTrace);
		} else {
			Console.Out.WriteLine ("error BI0000: Unexpected error - Please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new");
			Console.Out.WriteLine (e.ToString ());
			Console.Out.WriteLine (Environment.StackTrace);
		}
		return error;
	}

	public static WarningLevel GetWarningLevel (int code)
	{
		WarningLevel level;

		if (warning_levels == null)
			return WarningLevel.Warning;

		// code -1: all codes
		if (warning_levels.TryGetValue (-1, out level))
			return level;

		if (warning_levels.TryGetValue (code, out level))
			return level;

		return WarningLevel.Warning;
	}

	public static void SetWarningLevel (WarningLevel level, int? code = null /* if null, apply to all warnings */)
	{
		if (warning_levels == null)
			warning_levels = new Dictionary<int, WarningLevel> ();

		if (code.HasValue)
			warning_levels [code.Value] = level;
		else
			warning_levels [-1] = level; // code -1: all codes.
	}
}
