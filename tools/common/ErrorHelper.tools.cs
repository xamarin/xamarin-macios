// Copyright 2020, Microsoft Corp. All rights reserved,

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.Bundler {
	public static partial class ErrorHelper {
		public static ApplePlatform Platform;

		internal static string Prefix {
			get {
				switch (Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
				case ApplePlatform.WatchOS:
				case ApplePlatform.MacCatalyst:
				case ApplePlatform.None: // Return "MT" by default instead of throwing an exception, because any exception here will most likely hide whatever other error we're trying to show.
					return "MT";
				case ApplePlatform.MacOSX:
					return "MM";
				default:
					// Do not use the ErrorHandler machinery, because it will probably end up recursing and eventually throwing a StackOverflowException.
					throw new InvalidOperationException ($"Unknown platform: {Platform}");
				}
			}
		}

		public enum WarningLevel {
			Error = -1,
			Warning = 0,
			Disable = 1,
		}

		static Dictionary<int, WarningLevel> warning_levels;
		public static int Verbosity { get; set; }

#pragma warning disable 649
		public static Func<Exception, bool> IsExpectedException;
		public static Action<int> ExitCallback;
#pragma warning restore 649

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
			if (code.HasValue) {
				warning_levels [code.Value] = level;
			} else {
				warning_levels [-1] = level; // code -1: all codes.
			}
		}

		public static void ParseWarningLevel (WarningLevel level, string value)
		{
			if (string.IsNullOrEmpty (value)) {
				SetWarningLevel (level);
			} else {
				foreach (var code in value.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					SetWarningLevel (level, int.Parse (code));
			}
		}

		public static void SetLocation (Application app, ProductException ex, MethodDefinition method, Instruction instruction = null)
		{
			if (!method.HasBody)
				return;

			if (instruction is null && method.Body.Instructions.Count == 0)
				return;

			if (instruction is null)
				instruction = method.Body.Instructions [0];

			app.LoadSymbols ();

			if (!method.DebugInformation.HasSequencePoints)
				return;

			// Find the sequence point with the highest offset that is less than or equal to the instruction's offset
			SequencePoint seq = null;
			foreach (var pnt in method.DebugInformation.SequencePoints) {
				if (pnt.Offset > instruction.Offset)
					continue;

				if (seq is not null && seq.Offset >= pnt.Offset)
					continue;

				seq = pnt;
			}
			if (seq is null)
				return;

			ex.FileName = seq.Document.Url;
			ex.LineNumber = seq.StartLine;
		}

		public static ProductException CreateError (Application app, int code, MemberReference member, string message, params object [] args)
		{
			return Create (app, code, true, null, member, null, message, args);
		}

		public static ProductException CreateError (Application app, int code, MethodDefinition location, string message, params object [] args)
		{
			return Create (app, code, true, null, location, null, message, args);
		}

		public static ProductException CreateError (Application app, int code, MethodDefinition location, Instruction instruction, string message, params object [] args)
		{
			return Create (app, code, true, null, location, instruction, message, args);
		}

		public static ProductException CreateError (Application app, int code, ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, true, null, provider, null, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, MethodDefinition location, string message, params object [] args)
		{
			return Create (app, code, true, innerException, location, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, TypeReference location, string message, params object [] args)
		{
			return Create (app, code, true, innerException, location, message, args);
		}

		public static ProductException CreateError (Application app, int code, Exception innerException, ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, true, innerException, provider, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, MemberReference member, string message, params object [] args)
		{
			return Create (app, code, false, null, member, null, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, MemberReference member, Instruction instruction, string message, params object [] args)
		{
			return Create (app, code, false, null, member, instruction, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, MethodDefinition location, string message, params object [] args)
		{
			return Create (app, code, false, null, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, false, null, provider, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, MethodDefinition location, string message, params object [] args)
		{
			return Create (app, code, false, innerException, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, MethodDefinition location, Instruction instruction, string message, params object [] args)
		{
			return Create (app, code, false, innerException, location, instruction, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, TypeReference location, string message, params object [] args)
		{
			return Create (app, code, false, innerException, location, message, args);
		}

		public static ProductException CreateWarning (Application app, int code, Exception innerException, ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, false, innerException, provider, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, ICustomAttributeProvider provider, string message, params object [] args)
		{
			return Create (app, code, error, innerException, provider, null, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, ICustomAttributeProvider provider, Instruction instruction, string message, params object [] args)
		{
			if (provider is MemberReference member) {
				if (instruction is not null)
					return Create (app, code, error, innerException, member, instruction, message, args);
				return Create (app, code, error, innerException, member, null, message, args);
			}

			if (provider is TypeReference type)
				return Create (app, code, error, innerException, type, message, args);

			return new ProductException (code, error, innerException, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, MemberReference member, Instruction instruction, string message, params object [] args)
		{
			var method = member as MethodReference;
			if (method is null) {
				var property = member as PropertyDefinition;
				if (property is not null) {
					method = property.GetMethod;
					if (method is null)
						method = property.SetMethod;
				}
			}
			return Create (app, code, error, innerException, method is null ? null : method.Resolve (), instruction, message, args);
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, MethodDefinition location, Instruction instruction, string message, params object [] args)
		{
			var e = new ProductException (code, error, innerException, message, args);
			if (location is not null)
				SetLocation (app, e, location, instruction);
			return e;
		}

		public static ProductException Create (Application app, int code, bool error, Exception innerException, TypeReference location, string message, params object [] args)
		{
			var e = new ProductException (code, error, innerException, message, args);
			if (location is not null) {
				var td = location.Resolve ();

				if (td.HasMethods) {
					foreach (var method in td.Methods) {
						if (!method.IsConstructor)
							continue;
						SetLocation (app, e, method);
						if (e.FileName is not null)
							break;
					}
				}
			}
			return e;
		}

		public static void Warning (int code, string message, params object [] args)
		{
			Show (new ProductException (code, false, message, args));
		}

		public static void Warning (int code, Exception innerException, string message, params object [] args)
		{
			Show (new ProductException (code, false, innerException, message, args));
		}

		// Shows any warnings, and if there are any errors, throws an AggregateException.
		public static void ThrowIfErrors (IList<Exception> exceptions)
		{
			if (exceptions?.Any () != true)
				return;

			// Separate warnings from errors
			var grouped = exceptions.GroupBy ((v) => (v as ProductException)?.Error == false);

			var warnings = grouped.SingleOrDefault ((v) => v.Key);
			if (warnings?.Any () == true)
				Show (warnings);

			var errors = grouped.SingleOrDefault ((v) => !v.Key);
			if (errors?.Any () == true)
				throw new AggregateException (errors);
		}

		public static void Show (IEnumerable<Exception> list)
		{
			var exceptions = CollectExceptions (list);
			bool error = false;

			foreach (var ex in exceptions)
				error |= ShowInternal (ex);

			if (error)
				Exit (1);
		}

		public static void Show (Exception e)
		{
			Show (new Exception [] { e });
		}

		static void Exit (int exitCode)
		{
			if (ExitCallback is not null)
				ExitCallback (exitCode);
			Environment.Exit (exitCode);
		}

		static bool ShowInternal (Exception e)
		{
			ProductException mte = (e as ProductException);
			bool error = true;

			if (mte is not null) {
				error = mte.Error;

				if (!error && GetWarningLevel (mte.Code) == WarningLevel.Disable)
					return false; // This is an ignored warning.

				Console.Error.WriteLine (mte.ToString ());

				ShowInner (e);

				if (Verbosity > 2 && !string.IsNullOrEmpty (e.StackTrace))
					Console.Error.WriteLine (e.StackTrace);
			} else if (IsExpectedException is null || !IsExpectedException (e)) {
				Console.Error.WriteLine ("error " + Prefix + "0000: Unexpected error - Please file a bug report at https://github.com/xamarin/xamarin-macios/issues/new");
				Console.Error.WriteLine (e.ToString ());
			} else {
				Console.Error.WriteLine (e.ToString ());
				ShowInner (e);
				if (Verbosity > 2 && !string.IsNullOrEmpty (e.StackTrace))
					Console.Error.WriteLine (e.StackTrace);
			}

			return error;
		}

		static void ShowInner (Exception e)
		{
			Exception ie = e.InnerException;
			if (ie is null)
				return;

			if (Verbosity > 3) {
				Console.Error.WriteLine ("--- inner exception");
				Console.Error.WriteLine (ie);
				Console.Error.WriteLine ("---");
			} else if (Verbosity > 0 || ie is ProductException) {
				Console.Error.WriteLine ("\t{0}", ie.Message);
			}
			ShowInner (ie);
		}
	}
}
