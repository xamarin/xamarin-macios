// Copyright 2014, Xamarin Inc. All rights reserved,

#nullable enable

#if MTOUCH || MMP || MMP_TEST || MTOUCH_TESTS
#define BUNDLER
#endif

using System;
using System.Collections.Generic;

#if !BUNDLER && !TESTS
using ProductException = ObjCRuntime.RuntimeException;
#endif

#nullable enable

#if BUNDLER || MSBUILD_TASKS || TESTS
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	static partial class ErrorHelper {
		public static ProductException CreateError (int code, string message, params object? [] args)
		{
			return new ProductException (code, true, null, message, args);
		}

		public static ProductException CreateError (int code, Exception? innerException, string message, params object? [] args)
		{
			return new ProductException (code, true, innerException, message, args);
		}

		public static ProductException CreateWarning (int code, string message, params object? [] args)
		{
			return new ProductException (code, false, null, message, args);
		}

		public static ProductException CreateWarning (int code, Exception? innerException, string message, params object? [] args)
		{
			return new ProductException (code, false, innerException, message, args);
		}

		internal static IList<Exception> CollectExceptions (IEnumerable<Exception> exceptions)
		{
			var rv = new List<Exception> ();
			foreach (var ex in exceptions)
				CollectExceptions (ex, rv);
			return rv;
		}

		static void CollectExceptions (Exception ex, List<Exception> exceptions)
		{
			if ((ex is AggregateException ae) && (ae.InnerExceptions.Count > 0)) {
				foreach (var ie in ae.InnerExceptions)
					CollectExceptions (ie, exceptions);
			} else {
				exceptions.Add (ex);
			}
		}
	}
}
