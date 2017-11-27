//
// BuiltinProfiles.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;

namespace Xamarin.Pmcs.Profiles
{
	public static class BuiltinProfiles
	{
		public static Profile Get (string profileName)
		{
			switch (profileName) {
			case "compat-ios":
			case "compat-mac":
				throw new InvalidOperationException ("Classic should not be building");
			default:
				return new XamCore2Common ();
			}

			return null;
		}
	}

	class XamCore2Common : Profile
	{
		public XamCore2Common ()
		{
			GlobalReplacements.Add (new Xamarin20Replacement ());
			EnumBackingTypeReplacements.Add (new Xamarin20Replacement ());
		}
	}

	class Xamarin20Replacement : ExplicitReplacement
	{
		protected override string Evaluate (string input)
		{
			if (Context == ReplacementContext.EnumBackingType) {
				switch (input) {
				case "nint":
					return "long";
				case "nuint":
					return "ulong";
				default:
					return null;
				}
			}

			switch (input) {
			case "uint_compat_int":
				return "uint";
			case "nuint_compat_int":
				return "ulong";
			case "nint_compat_int":
				return "long";
			case "NSAction":
				return "global::System.Action";
			case "XamCore":
				return String.Empty;
			}

			if (input.StartsWith ("XamCore.", StringComparison.Ordinal))
				return input.Substring (8);
			else if (input.StartsWith ("global::XamCore.", StringComparison.Ordinal))
				return "global::" + input.Substring (16);

			return null;
		}
	}
}
