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
using System.Reflection;

using ProductException = BindingException;

// Error allocation: the errors are listed (and documented) in $(TOP)/docs/website/generator-errors.md

public class BindingException : Exception {

	public BindingException (int code, bool error) :
		base (GetMessage (code))
	{
		Code = code;
		Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
	}

	public BindingException (int code, params object [] args) :
		this (code, false, args)
	{
	}

	public BindingException (int code, bool error, params object [] args) :
		this (code, error, null, args)
	{
	}

	public BindingException (int code, bool error, Exception innerException, params object [] args) :
		base (String.Format (GetMessage (code), args), innerException)
	{
		Code = code;
		Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
	}

	static string GetMessage (int code)
	{
		Type resourceType = Type.GetType ("bgen.Resources");
		string errorCode = string.Format ("BI{0:0000}", code);
		PropertyInfo prop = resourceType.GetProperty (errorCode, BindingFlags.NonPublic |
				BindingFlags.Static |
				BindingFlags.GetProperty);

		var errorMessage = prop is null ? String.Format (bgen.Resources._default, errorCode) :
					(String) prop.GetValue (null);
		return errorMessage;
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

	[ThreadStatic]
	static int verbosity;
	static public int Verbosity { get { return verbosity; } set { verbosity = value; } }

	[ThreadStatic]
	static Dictionary<int, WarningLevel> warning_levels;

	public static ProductException CreateError (int code)
	{
		return new ProductException (code, true);
	}

	public static ProductException CreateError (int code, params object [] args)
	{
		return new ProductException (code, true, args);
	}

	public static void Warning (int code)
	{
		Show (new ProductException (code, false));
	}

	public static void Warning (int code, params object [] args)
	{
		Show (new ProductException (code, false, args));
	}

	static public void Show (Exception e, bool rethrow_errors = true)
	{
		List<Exception> exceptions = new List<Exception> ();
		bool error = false;

		CollectExceptions (e, exceptions);

		if (rethrow_errors) {
			foreach (var ex in exceptions) {
				if (ex is ProductException pe && !pe.Error)
					continue;
				error = true;
				break;
			}
			if (error)
				throw e;
		}

		foreach (var ex in exceptions)
			ShowInternal (ex);
	}

	static void CollectExceptions (Exception ex, List<Exception> exceptions)
	{
#if NET_4_0
		AggregateException ae = ex as AggregateException;

		if (ae is not null) {
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

		if (mte is not null) {
			error = mte.Error;

			if (!error && GetWarningLevel (mte.Code) == WarningLevel.Disable)
				return false;

			Console.Out.WriteLine (mte.ToString ());

			if (Verbosity > 1) {
				Exception ie = e.InnerException;
				if (ie is not null) {
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

		if (warning_levels is null)
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
		if (warning_levels is null)
			warning_levels = new Dictionary<int, WarningLevel> ();

		if (code.HasValue)
			warning_levels [code.Value] = level;
		else
			warning_levels [-1] = level; // code -1: all codes.
	}

	public static void ClearWarningLevels ()
	{
		warning_levels?.Clear ();
	}
}
