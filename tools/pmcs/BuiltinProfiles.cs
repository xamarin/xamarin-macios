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
			// Watch
			case "watch":
				return new WatchXamCore3 (ArchDefine.None);
			case "watch-32":
				return new WatchXamCore3 (ArchDefine.ARCH_32);

			// TVOS
			case "tvos":
				return new WatchXamCore3 (ArchDefine.None);
			case "tvos-64":
				return new WatchXamCore3 (ArchDefine.ARCH_64);

			// iOS
			case "native":
				return new IosXamCore2 (ArchDefine.None);
			case "native-64":
				return new IosXamCore2 (ArchDefine.ARCH_64);
			case "native-32":
				return new IosXamCore2 (ArchDefine.ARCH_32);

			// Mac
			case "mobile":
				return new MacMobileFrameworkXamCore2 (ArchDefine.None);
			case "mobile-64":
				return new MacMobileFrameworkXamCore2 (ArchDefine.ARCH_64);
			case "mobile-32":
				return new MacMobileFrameworkXamCore2 (ArchDefine.ARCH_32);
			case "full":
				return new MacFullFrameworkXamCore2 (ArchDefine.None);
			case "full-64":
				return new MacFullFrameworkXamCore2 (ArchDefine.ARCH_64);
			case "full-32":
				return new MacFullFrameworkXamCore2 (ArchDefine.ARCH_32);

			case "compat-ios":
			case "compat-mac":
				throw new InvalidOperationException ("Classic should not be building");
			}

			return null;
		}
	}

	public enum ArchDefine
	{
		None,
		ARCH_32,
		ARCH_64
	}

	abstract class XamCore3Common : XamCore2Common
	{
		protected XamCore3Common (ArchDefine arch) : base (arch)
		{
		}
	}

	abstract class XamCore2Common : Profile
	{
		protected XamCore2Common (ArchDefine arch)
		{
			if (arch != ArchDefine.None)
				CompilerOptions.Add ("-define:" + arch.ToString ());

			GlobalReplacements.Add (new Xamarin20Replacement ());
			EnumBackingTypeReplacements.Add (new Xamarin20Replacement ());
		}
	}

	sealed class WatchXamCore3 : XamCore3Common
	{
		public WatchXamCore3 (ArchDefine arch) : base (arch)
		{
		}
	}

	sealed class IosXamCore2 : XamCore2Common
	{
		public IosXamCore2 (ArchDefine arch) : base (arch)
		{
		}
	}

	sealed class MacMobileFrameworkXamCore2 : XamCore2Common
	{
		public MacMobileFrameworkXamCore2 (ArchDefine arch) : base (arch)
		{
			CompilerExecutable = "../builds/mcs-mac32";
		}
	}

	sealed class MacFullFrameworkXamCore2 : XamCore2Common
	{
		public MacFullFrameworkXamCore2 (ArchDefine arch) : base (arch)
		{
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
