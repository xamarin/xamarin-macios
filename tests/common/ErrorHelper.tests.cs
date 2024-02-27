// Copyright 2020, Microsoft Corp. All rights reserved,

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public static partial class ErrorHelper {
		public static ApplePlatform Platform;

		internal static string Prefix {
			get {
				return "TESTS";
			}
		}

		public enum WarningLevel {
			Error = -1,
			Warning = 0,
			Disable = 1,
		}

		static Dictionary<int, WarningLevel>? warning_levels;

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
	}
}
