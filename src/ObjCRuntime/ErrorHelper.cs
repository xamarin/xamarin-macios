// Copyright 2014, Xamarin Inc. All rights reserved,

using System;
using System.Collections.Generic;

#if MONOTOUCH
#if MTOUCH
using ProductException=Xamarin.Bundler.MonoTouchException;
#else
#if XAMCORE_2_0
using ProductException=ObjCRuntime.RuntimeException;
#else
using ProductException=MonoTouch.RuntimeException;
#endif
#endif
#elif MONOMAC
#if MMP
using ProductException=Xamarin.Bundler.MonoMacException;
#else
#if XAMCORE_2_0
using ProductException=ObjCRuntime.RuntimeException;
#else
using ProductException=MonoMac.RuntimeException;
#endif
#endif
#else
#error Only supports XI or XM
#endif

#if !MTOUCH && !MMP
#if XAMCORE_2_0
using ObjCRuntime;
#endif
#endif

#if MMP || MMP_TEST || MTOUCH
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	static class ErrorHelper {
		public enum WarningLevel
		{
			Error = -1,
			Warning = 0,
			Disable = 1,
		}
#if MONOTOUCH
		const string Prefix = "MT";
#else
		const string Prefix = "MM";
#endif
		static Dictionary<int, WarningLevel> warning_levels;
		public static int Verbosity { get; set; }

#if MTOUCH || MMP
#pragma warning disable 649
		public static Func<Exception, bool> IsExpectedException;
		public static Action<int> ExitCallback;
#pragma warning restore 649
#endif

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
			
			return WarningLevel.Warning;;
		}

		public static void SetWarningLevel (WarningLevel level, int? code = null /* if null, apply to all warnings */)
		{
			if (warning_levels == null)
				warning_levels = new Dictionary<int, WarningLevel> ();
			if (code.HasValue) {
				warning_levels [code.Value] = level;
			} else {
				warning_levels [-1] = level; // code -1: all codes.
			}
		}

		public static ProductException CreateError (int code, string message, params object[] args)
		{
			return new ProductException (code, true, message, args);
		}

#if (MTOUCH || MMP) && !MMP_TEST && !WIN32
		public static void SetLocation (Application app, ProductException ex, Mono.Cecil.MethodDefinition method)
		{
			if (!method.HasBody)
				return;

#if MTOUCH
			app.LoadSymbols ();
#endif

			if (method.Body.Instructions.Count == 0)
				return;

			var seq = method.DebugInformation.GetSequencePoint (method.Body.Instructions [0]);

			if (seq == null)
				return;

			ex.FileName = seq.Document.Url;
			ex.LineNumber = seq.StartLine;
		}

		public static ProductException CreateError (Application app, int code, Mono.Cecil.MemberReference member, string message, params object[] args)
		{
			return Create (app, code, true, null, member, message, args);
		}

		public static ProductException CreateError (Application app, int code, Mono.Cecil.MethodDefinition location, string message, params object[] args)
		{
			return Create (app, code, true, null, location, message, args);
		}

		public static ProductException CreateError (Application app, int code, Mono.Cecil.ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, true, null, provider, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, Mono.Cecil.MethodDefinition location, string message, params object[] args)
		{
			return Create (app, code, true, innerException, location, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, Mono.Cecil.TypeReference location, string message, params object[] args)
		{
			return Create (app, code, true, innerException, location, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, Mono.Cecil.ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, true, innerException, provider, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Mono.Cecil.MemberReference member, string message, params object [] args)
		{
			return Create (app, code, false, null, member, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Mono.Cecil.MethodDefinition location, string message, params object[] args)
		{
			return Create (app, code, false, null, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Mono.Cecil.ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, false, null, provider, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, Mono.Cecil.MethodDefinition location, string message, params object[] args)
		{
			return Create (app, code, false, innerException, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, Mono.Cecil.TypeReference location, string message, params object[] args)
		{
			return Create (app, code, false, innerException, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, Mono.Cecil.ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, false, innerException, provider, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, Mono.Cecil.ICustomAttributeProvider provider, string message, params object [] args)
		{
			if (provider is Mono.Cecil.MemberReference member)
				return Create (app, code, error, innerException, member, message, args);

			if (provider is Mono.Cecil.TypeReference type)
				return Create (app, code, error, innerException, type, message, args);

			return new ProductException (code, error, innerException, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, Mono.Cecil.MemberReference member, string message, params object [] args)
		{
			Mono.Cecil.MethodReference method = member as Mono.Cecil.MethodReference;
			if (method == null) {
				var property = member as Mono.Cecil.PropertyDefinition;
				if (property != null) {
					method = property.GetMethod;
					if (method == null)
						method = property.SetMethod;
				}
			}
			return Create (app, code, error, innerException, method == null ? null : method.Resolve (), message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, Mono.Cecil.MethodDefinition location, string message, params object [] args)
		{
			var e = new ProductException (code, error, innerException, message, args);
			if (location != null)
				SetLocation (app, e, location);
			return e;
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, Mono.Cecil.TypeReference location, string message, params object [] args)
		{
			var e = new ProductException (code, error, innerException, message, args);
			if (location != null) {
				var td = location.Resolve ();

				if (td.HasMethods) {
					foreach (var method in td.Methods) {
						if (!method.IsConstructor)
							continue;
						SetLocation (app, e, method);
						if (e.FileName != null)
							break;
					}
				}
			}
			return e;
		}
#endif

		public static ProductException CreateError (int code, Exception innerException, string message, params object[] args)
		{
			return new ProductException (code, true, innerException, message, args);
		}

		public static ProductException CreateWarning (int code, string message, params object[] args)
		{
			return new ProductException (code, false, message, args);
		}

		public static void Error (int code, Exception innerException, string message, params object[] args)
		{
			throw new ProductException (code, true, innerException, message, args);
		}

		public static void Error (int code, string message, params object[] args)
		{
			throw new ProductException (code, true, message, args);
		}

		public static void Warning (int code, string message, params object[] args)
		{
			Show (new ProductException (code, false, message, args));
		}

		public static void Warning (int code, Exception innerException, string message, params object[] args)
		{
			Show (new ProductException (code, false, innerException, message, args));
		}

		public static void Show (IEnumerable<Exception> list)
		{
			List<Exception> exceptions = new List<Exception> ();
			bool error = false;

			foreach (var e in list)
				CollectExceptions (e, exceptions);

			foreach (var ex in exceptions)
				error |= ShowInternal (ex);

			if (error)
				Exit (1);
		}

		static public void Show (Exception e)
		{
			List<Exception> exceptions = new List<Exception> ();
			bool error = false;

			CollectExceptions (e, exceptions);

			foreach (var ex in exceptions)
				error |= ShowInternal (ex);

			if (error)
				Exit (1);
		}

		static void Exit (int exitCode)
		{
#if MTOUCH
			if (ExitCallback != null)
				ExitCallback (exitCode);
#endif
			Environment.Exit (exitCode);
		}

		static void CollectExceptions (Exception ex, List<Exception> exceptions)
		{
			AggregateException ae = ex as AggregateException;

			if (ae != null && ae.InnerExceptions.Count > 0) {
				foreach (var ie in ae.InnerExceptions)
					CollectExceptions (ie, exceptions);
			} else {
				exceptions.Add (ex);
			}
		}

		static bool ShowInternal (Exception e)
		{
			ProductException mte = (e as ProductException);
			bool error = true;

			if (mte != null) {
				error = mte.Error;

				if (!error && GetWarningLevel (mte.Code) == WarningLevel.Disable)
					return false; // This is an ignored warning.
				
				Console.Error.WriteLine (mte.ToString ());

				// Errors with code >= 9000 are activation/licensing errors.
				// PROTIP: do not show a stack trace that points the exact method where the license checks are done
				if (mte.Code > 8999)
					return error;

				if (Verbosity > 1)
					ShowInner (e);

				if (Verbosity > 2 && !string.IsNullOrEmpty (e.StackTrace))
					Console.Error.WriteLine (e.StackTrace);
#if MTOUCH || MMP
			} else if (IsExpectedException == null || !IsExpectedException (e)) {
				Console.Error.WriteLine ("error " + Prefix + "0000: Unexpected error - Please file a bug report at http://bugzilla.xamarin.com");
				Console.Error.WriteLine (e.ToString ());
#endif
			} else {
				Console.Error.WriteLine (e.ToString ());
				if (Verbosity > 1)
					ShowInner (e);
				if (Verbosity > 2 && !string.IsNullOrEmpty (e.StackTrace))
					Console.Error.WriteLine (e.StackTrace);
			}

			return error;
		}

		static void ShowInner (Exception e)
		{
			Exception ie = e.InnerException;
			if (ie == null)
				return;

			if (Verbosity > 3) {
				Console.Error.WriteLine ("--- inner exception");
				Console.Error.WriteLine (ie);
				Console.Error.WriteLine ("---");
			} else {
				Console.Error.WriteLine ("\t{0}", ie.Message);
			}
			ShowInner (ie);
		}
	}
}
