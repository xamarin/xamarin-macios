// Copyright 2014, Xamarin Inc. All rights reserved,

#if MTOUCH || MMP || MMP_TEST || MTOUCH_TESTS
#define BUNDLER
#endif

using System;
using System.Collections.Generic;

#if !BUNDLER
using ProductException=ObjCRuntime.RuntimeException;
#endif

#if BUNDLER
namespace Xamarin.Bundler {
#else
namespace ObjCRuntime {
#endif
	static partial class ErrorHelper {
		public static ProductException CreateError (int code, string message, params object [] args)
		{
			return new ProductException (code, true, message, args);
		}

		public static ProductException CreateError (int code, Exception innerException, string message, params object [] args)
		{
			return new ProductException (code, true, innerException, message, args);
		}

		public static ProductException CreateWarning (int code, string message, params object [] args)
		{
			return new ProductException (code, false, message, args);
		}

		public static ProductException CreateWarning (int code, Exception innerException, string message, params object [] args)
		{
			return new ProductException (code, false, innerException, message, args);
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
	}
}
