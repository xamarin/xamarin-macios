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
				return new CompatIos ();
			case "compat-mac":
				throw new InvalidOperationException ("Classic should not be building");
			default:
				return new XamCore2Common ();
			}
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

	sealed class CompatIos : Profile
	{
		public CompatIos ()
		{
			IgnorePaths.Add ("Compat.iOS.cs");

			GlobalReplacements.Add (new XamarinCompatReplacement ("MonoTouch"));
			EnumBackingTypeReplacements.Add (new XamarinCompatReplacement ("MonoTouch"));
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
