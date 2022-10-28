// Copyright 2020, Microsoft Corp. All rights reserved,

using System;
using System.Collections.Generic;

using ProductException = ObjCRuntime.RuntimeException;

namespace ObjCRuntime {
	static partial class ErrorHelper {
		public static void Show (Exception e)
		{
			var list = new List<Exception> ();
			var errors = new List<Exception> ();

			// Unwrap AggregateExceptions.
			CollectExceptions (e, list);

			foreach (var ex in list) {
				var pe = ex as ProductException;

				// Show the exception
				Runtime.NSLog (ex.ToString ());

				// Add to list of errors if it's an error
				if (pe?.Error == false) {
					// This is a warning, we don't need to do anything
				} else {
					errors.Add (ex);
				}
			}

			// No actual errors, we're done
			if (errors.Count == 0)
				return;

			// Errors were found, we need to throw
			if (errors.Count == 1)
				throw errors [0];

			// Use an AggregateException for multiple errors.
			throw new AggregateException (errors);
		}
	}
}
