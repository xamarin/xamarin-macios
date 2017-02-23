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

			// Compat iOS/Mac
			case "compat-ios":
				return new CompatIos ();
			case "compat-mac":
				return new CompatMac ();
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
			CompilerOptions.Add ("-define:XAMCORE_3_0");
		}
	}

	abstract class XamCore2Common : Profile
	{
		protected XamCore2Common (ArchDefine arch)
		{
			CompilerOptions.Add ("-define:XAMCORE_2_0");
			CompilerOptions.Add ("-define:__UNIFIED__");

			if (arch != ArchDefine.None)
				CompilerOptions.Add ("-define:" + arch.ToString ());

			IgnorePaths.Add ("Compat.mac.cs");
			IgnorePaths.Add ("Compat.iOS.cs");

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

			CompilerOptions.Add ("-nostdlib");
			CompilerOptions.Add ("-r:mscorlib");
			CompilerOptions.Add ("-lib:../_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac");
		}
	}

	sealed class MacFullFrameworkXamCore2 : XamCore2Common
	{
		public MacFullFrameworkXamCore2 (ArchDefine arch) : base (arch)
		{
			CompilerOptions.Add ("-sdk:4.5");
			CompilerOptions.Add ("-define:NO_SYSTEM_DRAWING");
			CompilerOptions.Add ("-define:XAMMAC_SYSTEM_MONO");
		}
	}

	sealed class CompatIos : Profile
	{
		public CompatIos ()
		{
			IgnorePaths.Add ("Compat.iOS.cs");

			GlobalReplacements.Add (new XamarinCompatReplacement ("MonoTouch"));
			EnumBackingTypeReplacements.Add (new XamarinCompatReplacement ("MonoTouch"));
		}
	}

	sealed class CompatMac : Profile
	{
		public CompatMac ()
		{
			CompilerOptions.Add ("-sdk:4.0");
			CompilerOptions.Add ("-define:ARCH_32");
			CompilerOptions.Add ("-define:XAMMAC_SYSTEM_MONO");

			IgnorePaths.Add ("Compat.mac.cs");

			GlobalReplacements.Add (new XamarinCompatReplacement ("MonoMac"));
			EnumBackingTypeReplacements.Add (new XamarinCompatReplacement ("MonoMac"));
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

	class XamarinCompatReplacement : ExplicitReplacement
	{
		readonly string namespacePrefix;
		readonly bool isMonoMac;

		public XamarinCompatReplacement (string namespacePrefix)
		{
			if (namespacePrefix == null)
				throw new ArgumentNullException ("namespaceprefix",
					"provide an ns-prefix attribute in the profile XML element");

			this.namespacePrefix = namespacePrefix;
			isMonoMac = namespacePrefix == "MonoMac";
		}

		protected override string Evaluate (string input)
		{
			if (Context == ReplacementContext.EnumBackingType) {
				switch (input) {
				case "nint":
					return "int";
				case "nuint":
					return "uint";
				default:
					return null;
				}
			}

			switch (input) {
			case "nint":
			case "uint_compat_int":
			case "nuint_compat_int":
			case "nint_compat_int":
				return "int";
			case "System.nint":
			case "global::System.nint":
				return "System.Int32";
			case "nuint":
				return "uint";
			case "System.nuint":
			case "global::System.nuint":
				return "System.UInt32";
			case "nfloat":
				return "float";
			case "System.nfloat":
			case "global::System.nfloat":
				return "System.Single";
			case "nint.MaxValue":
				return "int.MaxValue";
			case "XamCore":
				return namespacePrefix;
			case "CGSize":
				return "System.Drawing.SizeF";
			case "CGPoint":
				return "System.Drawing.PointF";
			case "CGRect":
				return "System.Drawing.RectangleF";
			}

			if (input.StartsWith ("XamCore.", StringComparison.Ordinal))
				return namespacePrefix + input.Substring (7);
			else if (input.StartsWith ("global::XamCore.", StringComparison.Ordinal))
				return namespacePrefix + input.Substring (15);
			else if (input.StartsWith ("CGSize.", StringComparison.Ordinal))
				return "System.Drawing.SizeF" + input.Substring (6);
			else if (input.StartsWith ("CGPoint.", StringComparison.Ordinal))
				return "System.Drawing.PointF" + input.Substring (7);
			else if (input.StartsWith ("CGRect.", StringComparison.Ordinal))
				return "System.Drawing.RectangleF" + input.Substring (6);

			if (!isMonoMac)
				return null;

			switch (input) {
			case "OpenTK.Platform.MacOS":
			case "OpenTK.Platform":
			case "OpenTK.Graphics.OpenGL":
			case "OpenTK.Graphics":
			case "OpenTK":
				return "MonoMac.OpenGL";
			case "OpenTK.Audio.OpenAL":
			case "OpenTK.Audio":
				return "MonoMac.OpenAL";
			}

			if (input.StartsWith ("global::OpenTK.", StringComparison.Ordinal))
				return "MonoMac.OpenGL" + input.Substring (14);
			else if (input.StartsWith ("OpenTK.Graphics.", StringComparison.Ordinal))
				return "MonoMac" + input.Substring (15);

			return null;
		}
	}
}
