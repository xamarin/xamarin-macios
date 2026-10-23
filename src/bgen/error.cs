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

using System.Collections.Generic;
using System.Reflection;

using ProductException = BindingException;

#nullable enable

// Error allocation: the errors are listed (and documented) in $(TOP)/docs/website/generator-errors.md

public class BindingException : Exception {

	public BindingException (int code, bool error) :
		base (GetMessage (code))
	{
		Code = code;
		Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
	}

	public BindingException (int code, params object? [] args) :
		this (code, false, args)
	{
	}

	public BindingException (int code, bool error, params object? [] args) :
		this (code, error, null, args)
	{
	}

	public BindingException (int code, bool error, Exception? innerException, params object? [] args) :
		base (String.Format (GetMessage (code), args), innerException)
	{
		Code = code;
		Error = error || ErrorHelper.GetWarningLevel (code) == ErrorHelper.WarningLevel.Error;
	}

	static string GetMessage (int code)
	{
		var resourceType = Type.GetType ("bgen.Resources");
		var errorCode = string.Format ("BI{0:0000}", code);
		var prop = resourceType?.GetProperty (errorCode, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);
		var errorMessage = (string?) prop?.GetValue (null);
		if (errorMessage is null)
			errorMessage = string.Format (bgen.Resources._default, errorCode);
		return errorMessage;
	}

	public int Code { get; private set; }

	public bool Error { get; private set; }

#if MSBUILD_TASKS
	public Xamarin.Bundler.ProductException ToProductException ()
	{
		return new Xamarin.Bundler.ProductException (Code, Error, Message);
	}
#endif

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
	static Dictionary<int, WarningLevel>? warning_levels;

	public static ProductException CreateError (int code)
	{
		return new ProductException (code, true);
	}

	public static ProductException CreateError (int code, params object? [] args)
	{
		return new ProductException (code, true, args);
	}

	public static ProductException CreateWarning (int code, params object? [] args)
	{
		return new ProductException (code, false, args);
	}

	public static void Warning (IToolLog log, int code)
	{
		Show (log, new ProductException (code, false));
	}

	public static void Warning (IToolLog log, int code, params object? [] args)
	{
		Show (log, new ProductException (code, false, args));
	}

	static public void Show (IToolLog log, Exception e, bool rethrow_errors = true)
	{
		var exceptions = new List<Exception> ();
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
			ShowInternal (log, ex);
	}

	static void CollectExceptions (Exception ex, List<Exception> exceptions)
	{
#if NET_4_0
		var ae = ex as AggregateException;

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

	static bool ShowInternal (IToolLog log, Exception e)
	{
		var mte = (e as BindingException);
		bool error = true;

		if (mte is not null) {
			error = mte.Error;

			if (!error && GetWarningLevel (mte.Code) == WarningLevel.Disable)
				return false;

			if (error)
#if MSBUILD_TASKS
				log.LogError (mte.ToProductException ());
#else
				log.LogError (mte);
#endif
			else
#if MSBUILD_TASKS
				log.LogWarning (mte.ToProductException ());
#else
				log.LogWarning (mte);
#endif

			if (log.Verbosity > 1) {
				var ie = e.InnerException;
				if (ie is not null) {
					if (log.Verbosity > 3) {
						log.LogError ("--- inner exception");
						log.LogError (ie.ToString ());
						log.LogError ("---");
					} else {
						log.LogError ($"\t{ie.Message}");
					}
				}
			}

			if (log.Verbosity > 2 && e.StackTrace is not null) {
				if (error)
					log.LogError (e.StackTrace);
				else
					log.Log (e.StackTrace);
			}
		} else {
			log.LogError ("error BI0000: Unexpected error - Please file a bug report at https://github.com/dotnet/macios/issues/new");
			log.LogException (e);
			log.LogError (Environment.StackTrace);
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
