using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests {
	public enum LinkerOption {
		Unspecified,
		LinkAll,
		LinkSdk,
		DontLink,
		LinkPlatform, // only applicable for XM
	}

	public enum RegistrarOption {
		Unspecified,
		Dynamic,
		Static,
	}

	[Flags]
	enum I18N {
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
	abstract class BundlerTool : Tool {
		public const string None = "None";
		public bool AlwaysShowOutput;

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
		public string Interpreter;

		// These are a bit smarter
		public bool NoPlatformAssemblyReference;
#pragma warning restore 0649

		bool IsMtouchTool {
			get {
				return GetType ().Name == "MTouchTool";
			}
		}

		protected void AddVerbosity (IList<string> args)
		{
			if (Verbosity == 0) {
				// do nothing
			} else if (Verbosity > 0) {
				args.Add ("-" + new string ('v', Verbosity));
			} else {
				args.Add ("-" + new string ('q', -Verbosity));
			}
		}

		protected IList<string> ToolArguments {
			get {
				var sb = new List<string> ();
				BuildArguments (sb);
				return sb;
			}
		}

		protected virtual string GetDefaultAbi ()
		{
			return null;
		}

		protected virtual void BuildArguments (IList<string> sb)
		{
			// Options are processed alphabetically

			if (Abi == None) {
				// add nothing
			} else if (!string.IsNullOrEmpty (Abi)) {
				sb.Add (IsMtouchTool ? "--abi" : "--arch");
				sb.Add (Abi);
			} else {
				var a = GetDefaultAbi ();
				if (!string.IsNullOrEmpty (a)) {
					sb.Add (IsMtouchTool ? "--abi" : "--arch");
					sb.Add (a);
				}
			}

			if (!string.IsNullOrEmpty (Cache)) {
				sb.Add ("--cache");
				sb.Add (Cache);
			}

			if (CustomArguments is not null) {
				foreach (var arg in CustomArguments) {
					sb.Add (arg);
				}
			}

			if (Debug.HasValue && Debug.Value)
				sb.Add ("--debug");

			if (Extension == true)
				sb.Add ("--extension");

			if (!string.IsNullOrEmpty (GccFlags)) {
				sb.Add (IsMtouchTool ? "--gcc_flags" : "--link_flags");
				sb.Add (GccFlags);
			}

			if (!string.IsNullOrEmpty (HttpMessageHandler))
				sb.Add ($"--http-message-handler={HttpMessageHandler}");

			if (I18N != I18N.None) {
				sb.Add ("--i18n");
				var i18n = new List<string> ();
				if ((I18N & I18N.CJK) == I18N.CJK)
					i18n.Add ("cjk");
				if ((I18N & I18N.MidEast) == I18N.MidEast)
					i18n.Add ("mideast");
				if ((I18N & I18N.Other) == I18N.Other)
					i18n.Add ("other");
				if ((I18N & I18N.Rare) == I18N.Rare)
					i18n.Add ("rare");
				if ((I18N & I18N.West) == I18N.West)
					i18n.Add ("west");
				sb.Add (string.Join (",", i18n));
			}

			switch (Linker) {
			case LinkerOption.LinkAll:
			case LinkerOption.Unspecified:
				break;
			case LinkerOption.DontLink:
				sb.Add ("--nolink");
				break;
			case LinkerOption.LinkSdk:
				sb.Add ("--linksdkonly");
				break;
			case LinkerOption.LinkPlatform:
				sb.Add ("--linkplatform");
				break;
			default:
				throw new NotImplementedException ();
			}

			if (LinkSkip?.Length > 0) {
				foreach (var ls in LinkSkip)
					sb.Add ($"--linkskip:{ls}");
			}

			if (NoWarn is not null) {
				if (NoWarn.Length > 0) {
					sb.Add ($"--nowarn:{string.Join (",", NoWarn.Select ((v) => v.ToString ()))}");
				} else {
					sb.Add ("--nowarn");
				}
			}

			if (Optimize is not null) {
				foreach (var opt in Optimize)
					sb.Add ($"--optimize:{opt}");
			}

			if (Profiling.HasValue)
				sb.Add ($"--profiling:{(Profiling.Value ? "true" : "false")}");

			if (References is not null) {
				foreach (var r in References)
					sb.Add ((IsMtouchTool ? "-r:" : "-a:") + r);
			}

			switch (Registrar) {
			case RegistrarOption.Unspecified:
				break;
			case RegistrarOption.Dynamic:
				sb.Add ("--registrar:dynamic");
				break;
			case RegistrarOption.Static:
				sb.Add ("--registrar:static");
				break;
			default:
				throw new NotImplementedException ();
			}

			if (!string.IsNullOrEmpty (ResponseFile))
				sb.Add ("@" + ResponseFile);

			if (!string.IsNullOrEmpty (RootAssembly))
				sb.Add (RootAssembly);

			if (Sdk == None) {
				// do nothing	
			} else if (!string.IsNullOrEmpty (Sdk)) {
				sb.Add ("--sdk");
				sb.Add (Sdk);
			} else {
				sb.Add ("--sdk");
				sb.Add (Configuration.GetSdkVersion (Profile));
			}

			if (SdkRoot == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (SdkRoot)) {
				sb.Add ("--sdkroot");
				sb.Add (SdkRoot);
			} else {
				sb.Add ("--sdkroot");
				sb.Add (Configuration.xcode_root);
			}

			if (TargetFramework == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetFramework)) {
				sb.Add ("--target-framework");
				sb.Add (TargetFramework);
			} else {
				switch (Profile) {
				case Profile.iOS:
				case Profile.tvOS:
				case Profile.watchOS:
				case Profile.macOSFull:
				case Profile.macOSMobile:
				case Profile.macOSSystem:
					sb.Add ("--target-framework");
					sb.Add (Configuration.GetTargetFramework (Profile));
					if (!NoPlatformAssemblyReference)
						sb.Add ("-r:" + Configuration.GetBaseLibrary (Profile));
					break;
				default:
					throw new NotImplementedException ();
				}
			}

			if (TargetVer == None) {
				// do nothing
			} else if (!string.IsNullOrEmpty (TargetVer)) {
				sb.Add (IsMtouchTool ? "--targetver" : "--minos");
				sb.Add (TargetVer);
			}

			AddVerbosity (sb);

			if (WarnAsError is not null) {
				if (WarnAsError.Length > 0) {
					sb.Add ($"--warnaserror:{string.Join (",", WarnAsError.Select ((v) => v.ToString ()))}");
				} else {
					sb.Add ("--warnaserror");
				}
			}

			if (XmlDefinitions?.Length > 0) {
				foreach (var xd in XmlDefinitions)
					sb.Add ($"--xml:{xd}");
			}

			if (Interpreter is not null) {
				if (Interpreter.Length == 0)
					sb.Add ("--interpreter");
				else
					sb.Add ("--interpreter=" + Interpreter);
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
			return Execute (ToolArguments, always_show_output: Verbosity > 0 || AlwaysShowOutput);
		}

		public virtual void AssertExecute (string message = null)
		{
			var rv = Execute ();
			if (rv == 0)
				return;
			var errors = Messages.Where ((v) => v.IsError).ToList ();
			Assert.Fail ($"Expected execution to succeed, but exit code was {rv}, and there were {errors.Count} error(s).\nCommand: {ToolPath} {StringUtils.FormatArguments (ToolArguments)}\nMessage: {message}\n\t" +
					 string.Join ("\n\t", errors.Select ((v) => v.ToString ())));
		}

		public void AssertExecuteFailure (string message = null)
		{
			Assert.AreEqual (1, Execute (), message);
		}

		public abstract void CreateTemporaryApp (Profile profile, string appName = "testApp", string code = null, IList<string> extraArgs = null, string extraCode = null, string usings = null);

		public static string CreateCode (string code, string usings, string extraCode)
		{
			if (code is null)
				code = "public class TestApp { static void Main () { System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ()); } }";
			if (usings is not null)
				code = usings + "\n" + code;
			if (extraCode is not null)
				code += extraCode;
			return code;
		}

		public static string CompileTestAppExecutable (string targetDirectory, string code = null, IList<string> extraArgs = null, Profile profile = Profile.iOS, string appName = "testApp", string extraCode = null, string usings = null)
		{
			return CompileTestAppCode ("exe", targetDirectory, CreateCode (code, usings, extraCode), extraArgs, profile, appName);
		}

		public static string CompileTestAppLibrary (string targetDirectory, string code, IList<string> extraArgs = null, Profile profile = Profile.iOS, string appName = "testApp")
		{
			return CompileTestAppCode ("library", targetDirectory, code, extraArgs, profile, appName);
		}

		public static string CompileTestAppCode (string target, string targetDirectory, string code, IList<string> extraArgs = null, Profile profile = Profile.iOS, string appName = "testApp")
		{
			var ext = target == "exe" ? "exe" : "dll";
			var cs = Path.Combine (targetDirectory, "testApp.cs");
			var assembly = Path.Combine (targetDirectory, appName + "." + ext);
			var root_library = Configuration.GetBaseLibrary (profile);

			File.WriteAllText (cs, code);

			string output;
			var args = new List<string> ();
			string fileName = Configuration.GetCompiler (profile, args);
			args.Add ("/noconfig");
			args.Add ($"/t:{target}");
			args.Add ("/nologo");
			args.Add ($"/out:{assembly}");
			args.Add ($"/r:{root_library}");
			args.Add (cs);
			if (extraArgs is not null)
				args.AddRange (extraArgs);
			if (ExecutionHelper.Execute (fileName, args, out output) != 0) {
				Console.WriteLine ("{0} {1}", fileName, args);
				Console.WriteLine (output);
				throw new Exception (output);
			}

			return assembly;
		}

		// The directory where the assemblies are located in the built app
		public abstract string GetAppAssembliesDirectory ();

		// The path to the platform assembly in the built app.
		public string GetPlatformAssemblyInApp ()
		{
			var asm = Path.GetFileName (Configuration.GetBaseLibrary (Profile));
			return Path.Combine (GetAppAssembliesDirectory (), asm);
		}
	}
}
