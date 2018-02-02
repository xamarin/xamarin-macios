using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests
{
	public enum LinkerOption
	{
		Unspecified,
		LinkAll,
		LinkSdk,
		DontLink,
		LinkPlatform, // only applicable for XM
	}

	public enum RegistrarOption
	{
		Unspecified,
		Dynamic,
		Static,
	}

	[Flags]
	enum I18N
	{
		None = 0,

		CJK = 1,
		MidEast = 2,
		Other = 4,
		Rare = 8,
		West = 16,

		All = CJK | MidEast | Other | Rare | West,
		Base
	}

	// This class represents options/logic that is identical between mtouch and mmp
	abstract class BundlerTool : Tool
	{
		public const string None = "None";

#pragma warning disable 0649 // Field 'X' is never assigned to, and will always have its default value Y
		// These map directly to mtouch/mmp options
		// These options are ordered alphabetically
		public string Abi; // This is --abi for mtouch and --arch for mmp
		public string Cache;
		public string [] CustomArguments; // Sometimes you want to pass invalid arguments to mtouch, in this case this array is used. No processing will be done, if quotes are required, they must be added to the arguments in the array.
		public bool? Debug;
		public bool? Extension;
		public string GccFlags; // This is --gcc_flags for mtouch and --link_flags for mmp
		public string HttpMessageHandler;
		public I18N I18N;
		public LinkerOption Linker;
		public string [] LinkSkip;
		public int [] NoWarn; // null array: nothing passed to mtouch/mmp. empty array: pass --nowarn (which means disable all warnings).
		public string [] Optimize;
		public Profile Profile;
		public bool? Profiling;
		public string [] References; // This is -r for mtouch and -a for mmp
		public RegistrarOption Registrar;
		public string ResponseFile;
		public string RootAssembly;
		public string Sdk;
		public string SdkRoot;
		public string TargetFramework;
		public string TargetVer; // this is --targetver for mtouch and --minos for mmp
		public int Verbosity;
		public int [] WarnAsError; // null array: nothing passed to mtouch/mmp. empty array: pass --warnaserror (which means makes all warnings errors).
		public string [] XmlDefinitions;

		// These are a bit smarter
		public bool NoPlatformAssemblyReference;
#pragma warning restore 0649

		bool IsMtouchTool {
			get {
				return GetType ().Name == "MTouchTool";
			}
		}

		protected string GetVerbosity ()
		{
			if (Verbosity == 0) {
				return string.Empty;
			} else if (Verbosity > 0) {
				return new string ('-', Verbosity).Replace ("-", "-v ");
			} else {
				return new string ('-', -Verbosity).Replace ("-", "-q ");
			}
		}

		protected string ToolArguments {
			get {
				var sb = new StringBuilder ();
				BuildArguments (sb);
				return sb.ToString ();
			}
		}

		protected virtual string GetDefaultAbi ()
		{
			return null;
		}

		protected virtual void BuildArguments (StringBuilder sb)
		{
			// Options are processed alphabetically

			if (Abi == None) {
				// add nothing
			} else if (!string.IsNullOrEmpty (Abi)) {
				sb.Append (IsMtouchTool ? " --abi " : " --arch ").Append (Abi);
			} else {
				var a = GetDefaultAbi ();
				if (!string.IsNullOrEmpty (a)) {
					sb.Append (IsMtouchTool ? " --abi " : " --arch ");
					sb.Append (a);
				}
			}

			if (!string.IsNullOrEmpty (Cache))
				sb.Append (" --cache ").Append (StringUtils.Quote (Cache));

			if (CustomArguments != null) {
				foreach (var arg in CustomArguments) {
					sb.Append (" ").Append (arg);
				}
			}

			if (Debug.HasValue && Debug.Value)
				sb.Append (" --debug");

			if (Extension == true)
				sb.Append (" --extension");

			if (!string.IsNullOrEmpty (GccFlags))
				sb.Append (IsMtouchTool ? " --gcc_flags " : " --link_flags ").Append (StringUtils.Quote (GccFlags));

			if (!string.IsNullOrEmpty (HttpMessageHandler))
				sb.Append (" --http-message-handler=").Append (StringUtils.Quote (HttpMessageHandler));

			if (I18N != I18N.None) {
				sb.Append (" --i18n ");
				int count = 0;
				if ((I18N & I18N.CJK) == I18N.CJK)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("cjk");
				if ((I18N & I18N.MidEast) == I18N.MidEast)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("mideast");
				if ((I18N & I18N.Other) == I18N.Other)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("other");
				if ((I18N & I18N.Rare) == I18N.Rare)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("rare");
				if ((I18N & I18N.West) == I18N.West)
					sb.Append (count++ == 0 ? string.Empty : ",").Append ("west");
			}

			switch (Linker) {
			case LinkerOption.LinkAll:
			case LinkerOption.Unspecified:
				break;
			case LinkerOption.DontLink:
				sb.Append (" --nolink");
				break;
			case LinkerOption.LinkSdk:
				sb.Append (" --linksdkonly");
				break;
			case LinkerOption.LinkPlatform:
				sb.Append (" --linkplatform");
				break;
			default:
				throw new NotImplementedException ();
			}

			if (LinkSkip?.Length > 0) {
				foreach (var ls in LinkSkip)
					sb.Append (" --linkskip:").Append (StringUtils.Quote (ls));
			}

			if (NoWarn != null) {
				if (NoWarn.Length > 0) {
					sb.Append (" --nowarn:");
					foreach (var code in NoWarn)
						sb.Append (code).Append (',');
					sb.Length--;
				} else {
					sb.Append (" --nowarn");
				}
			}

			if (Optimize != null) {
				foreach (var opt in Optimize)
					sb.Append (" --optimize:").Append (opt);
			}

			if (Profiling.HasValue)
				sb.Append (" --profiling:").Append (Profiling.Value ? "true" : "false");

			if (References != null) {
				foreach (var r in References)
					sb.Append (IsMtouchTool ? " -r:" : " -a:").Append (StringUtils.Quote (r));
			}

			switch (Registrar) {
			case RegistrarOption.Unspecified:
				break;
			case RegistrarOption.Dynamic:
				sb.Append (" --registrar:dynamic");
				break;
			case RegistrarOption.Static:
				sb.Append (" --registrar:static");
				break;
			default:
				throw new NotImplementedException ();
			}

			if (!string.IsNullOrEmpty (ResponseFile))
				sb.Append (" @").Append (StringUtils.Quote (ResponseFile));

			if (!string.IsNullOrEmpty (RootAssembly))
				sb.Append (" ").Append (StringUtils.Quote (RootAssembly));

			if (Sdk == None) {
				// do nothing	
			} else if (!string.IsNullOrEmpty (Sdk)) {
				sb.Append (" --sdk ").Append (Sdk);
			} else {
				sb.Append (" --sdk ").Append (Configuration.GetSdkVersion (Profile));
			}

			if (SdkRoot == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (SdkRoot)) {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (SdkRoot));
			} else {
				sb.Append (" --sdkroot ").Append (StringUtils.Quote (Configuration.xcode_root));
			}

			if (TargetFramework == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetFramework)) {
				sb.Append (" --target-framework ").Append (TargetFramework);
			} else if (!NoPlatformAssemblyReference) {
				// make the implicit default the way tests have been running until now, and at the same time the very minimum to make apps build.
				switch (Profile) {
				case Profile.iOS:
					sb.Append (" -r:").Append (StringUtils.Quote (Configuration.XamarinIOSDll));
					break;
				case Profile.tvOS:
				case Profile.watchOS:
					sb.Append (" --target-framework ").Append (Configuration.GetTargetFramework (Profile));
					sb.Append (" -r:").Append (StringUtils.Quote (Configuration.GetBaseLibrary (Profile)));
					break;
				case Profile.macOSFull:
				case Profile.macOSMobile:
				case Profile.macOSSystem:
					sb.Append (" --target-framework ").Append (Configuration.GetTargetFramework (Profile));
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			if (TargetVer == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetVer)) {
				sb.Append (IsMtouchTool ? " --targetver " : " --minos ").Append (TargetVer);
			}

			sb.Append (" ").Append (GetVerbosity ());

			if (WarnAsError != null) {
				if (WarnAsError.Length > 0) {
					sb.Append (" --warnaserror:");
					foreach (var code in WarnAsError)
						sb.Append (code).Append (',');
					sb.Length--;
				} else {
					sb.Append (" --warnaserror");
				}
			}

			if (XmlDefinitions?.Length > 0) {
				foreach (var xd in XmlDefinitions)
					sb.Append (" --xml:").Append (StringUtils.Quote (xd));
			}


		}

		public string CreateTemporaryDirectory ()
		{
			return Xamarin.Cache.CreateTemporaryDirectory ();
		}

		public void CreateTemporaryCacheDirectory ()
		{
			Cache = Path.Combine (CreateTemporaryDirectory (), "mtouch-test-cache");
			Directory.CreateDirectory (Cache);
		}

		public virtual int Execute ()
		{
			return Execute (ToolArguments, always_show_output: Verbosity > 0);
		}

		public virtual void AssertExecute (string message = null)
		{
			var rv = Execute ();
			if (rv == 0)
				return;
			var errors = Messages.Where ((v) => v.IsError).ToList ();
			Assert.Fail ($"Expected execution to succeed, but exit code was {rv}, and there were {errors.Count} error(s): {message}\n\t" +
				     string.Join ("\n\t", errors.Select ((v) => v.ToString ())));
		}

		public abstract void CreateTemporaryApp (Profile profile, string appName = "testApp", string code = null, string extraArg = "", string extraCode = null, string usings = null, bool use_csc = false);

		public static string CompileTestAppExecutable (string targetDirectory, string code = null, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp", string extraCode = null, string usings = null, bool use_csc = false)
		{
			if (code == null)
				code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
			if (usings != null)
				code = usings + "\n" + code;
			if (extraCode != null)
				code += extraCode;

			return CompileTestAppCode ("exe", targetDirectory, code, extraArg, profile, appName, use_csc);
		}

		public static string CompileTestAppLibrary (string targetDirectory, string code, string extraArg = null, Profile profile = Profile.iOS, string appName = "testApp")
		{
			return CompileTestAppCode ("library", targetDirectory, code, extraArg, profile, appName);
		}

		public static string CompileTestAppCode (string target, string targetDirectory, string code, string extraArg = "", Profile profile = Profile.iOS, string appName = "testApp", bool use_csc = false)
		{
			var ext = target == "exe" ? "exe" : "dll";
			var cs = Path.Combine (targetDirectory, "testApp.cs");
			var assembly = Path.Combine (targetDirectory, appName + "." + ext);
			var root_library = Configuration.GetBaseLibrary (profile);

			File.WriteAllText (cs, code);

			string output;
			StringBuilder args = new StringBuilder ();
			string fileName = Configuration.GetCompiler (profile, args, use_csc);
			args.AppendFormat ($" /noconfig /t:{target} /nologo /out:{StringUtils.Quote (assembly)} /r:{StringUtils.Quote (root_library)} {StringUtils.Quote (cs)} {extraArg}");
			if (ExecutionHelper.Execute (fileName, args.ToString (), out output) != 0) {
				Console.WriteLine ("{0} {1}", fileName, args);
				Console.WriteLine (output);
				throw new Exception (output);
			}

			return assembly;
		}
	}
}
