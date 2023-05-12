using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Mono.Options;
using Xamarin.Utils;

namespace xibuild {
	class MainClass {

		static bool verbose;
		public static int Main (string [] args)
		{
			bool runTool = false;
			bool configGenerationOnly = false;
			string baseConfigFile = null;
			string verbosity = "";
			bool no_logo = true;
			bool show_help = false;
			OptionSet p = null;
			p = new OptionSet () {
				"xibuild: Run msbuild or a tool with a custom msbuild config file which adds fallback paths from $MSBuildExtensionsPathFallbackPathsOverride.",
				String.Empty,
				"Usage: xibuild [-v] [-c] [-t] [-m <base config file>] [-h] -- [path to managed tool] [arguments...]",
				String.Empty,
				"Default: Generate a temporary app.config file and run msbuild",
				String.Empty,
				"Options:",

				{ "c", "Generate config file only", c => configGenerationOnly = true }, // merge
				{ "t", "Path to the managed tool to run. If this and `-c` are not used, then this runs msbuild", t => runTool = true },
				{ "m=", "< base config file >: Config file to merge with the one from msbuild.dll.config", m => baseConfigFile = m },
				{ "h|?|help", "Show this help message and exit.", v => show_help = true },
				{ "v|verbosity:", "Show verbose output.", v => verbosity = string.IsNullOrEmpty (v) ? "diagnostic" : v },
				{ "nologo", "Do not display the startup banner and copyright message when running msbuild.", v => no_logo = true },

				String.Empty,
				"Note: Adds the path from the environment variable named",
				"`$MSBuildExtensionsPathFallbackPathsOverride` to the list of fallback paths in the generated app.config",
				String.Empty,
				"Examples:",
				String.Empty,
				"Generate /path/to/foo.exe.config:",
				"\txibuild -c /path/to/foo.exe",
				"Run msbuild with a custom app.config and the arguments passed to msbuild:",
				"\txibuild -- /v:diag /path/to/project.csproj",
				"Run managed_tool.exe with the arguments, and a custom app.config:",
				"\txibuild -t -- /path/to/managed_tool.exe args",
				String.Empty,
				"Add `-m /path/to/base.exe.config` to merge the generated app.config with base.exe.config ."
			};

			List<string> remaining = null;
			try {
				remaining = p.Parse (args);
			} catch (OptionException oe) {
				Console.WriteLine (oe.Message);
				return -1;
			}

			verbose = !string.IsNullOrEmpty (verbosity) && verbosity [0] == 'd';
			if (verbose)
				Console.WriteLine ($"Running xibuild with args: {String.Join (" ", args)}\n");

			if (show_help) {
				p.WriteOptionDescriptions (Console.Out);
				return 0;
			}

			if (configGenerationOnly && runTool) {
				Console.WriteLine ("Use either -c or -t, but not both.\n");
				p.WriteOptionDescriptions (Console.Out);
				return -1;
			}

			if (!String.IsNullOrEmpty (baseConfigFile) && !File.Exists (baseConfigFile)) {
				Console.WriteLine ($"Base config file {baseConfigFile} not found.");
				return -1;
			}

			if (configGenerationOnly && remaining.Count == 0) {
				Console.WriteLine ("Please specify the path to managed tool to generate an app.config file for it.");
				return -1;
			}

			if (runTool && remaining.Count == 0) {
				Console.WriteLine ("Please specify the path to managed tool to run.");
				return -1;
			}

			bool runMSBuild = !runTool && !configGenerationOnly;

			if (!runTool && !runMSBuild) {
				GenerateAppConfig (remaining [0] + ".config", baseConfigFile, out string _);
				return 0;
			}

			if (runMSBuild) {
				// Add back any arguments we consumed.
				if (!string.IsNullOrEmpty (verbosity))
					remaining.Add ($"/verbosity:{verbosity}");
				if (no_logo)
					remaining.Add ("/nologo");
			}
			var arguments = remaining.Skip (runMSBuild ? 0 : 1).ToArray ();
			return RunTool (
					toolPath: runMSBuild ? "msbuild" : remaining [0],
					combinedArgs: StringUtils.FormatArguments (arguments),
					baseConfigFile: runMSBuild ? null : baseConfigFile);
		}

		static int RunTool (string toolPath, string combinedArgs, string baseConfigFile)
		{
			var tmpMSBuildExePathForConfig = Path.GetTempFileName ();
			var configFilePath = tmpMSBuildExePathForConfig + ".config";
			try {
				GenerateAppConfig (configFilePath, baseConfigFile, out string MSBuildSdksPath);

				var psi = new ProcessStartInfo {
					FileName = toolPath,
					Arguments = combinedArgs,
					UseShellExecute = false,
				};
				// Required so that msbuild can read the correct config file
				psi.EnvironmentVariables ["MSBUILD_EXE_PATH"] = tmpMSBuildExePathForConfig;
				// MSBuildSDKsPath only works via an env var
				psi.EnvironmentVariables ["MSBuildSDKsPath"] = MSBuildSdksPath;

				var p = Process.Start (psi);

				p.WaitForExit ();
				return p.ExitCode;
			} finally {
				if (File.Exists (tmpMSBuildExePathForConfig))
					File.Delete (tmpMSBuildExePathForConfig);
				if (File.Exists (tmpMSBuildExePathForConfig))
					File.Delete (configFilePath);
			}
		}

		static void GenerateAppConfig (string targetConfigFile, string baseConfigFile, out string MSBuildSdksPath)
		{
			bool IsMacOS = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.OSX);

			//FIXME: Current? or from PATH?
			string mono = IsMacOS ? "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono" : "/usr/lib/mono";
			string vsVersion = "15.0";
			string MSBuildBin = Path.Combine (mono, "msbuild", vsVersion, "bin");

			string MSBuildConfig = Path.Combine (MSBuildBin, "MSBuild.dll.config");
			string MSBuildExtensionsPath = Path.Combine (mono, "xbuild");
			string FrameworksDirectory = Path.Combine (mono, "xbuild-frameworks");
			MSBuildSdksPath = Path.Combine (MSBuildBin, "Sdks");

			var dstXml = new XmlDocument ();

			var dstConfigNode = dstXml.CreateNode (XmlNodeType.Element, "configuration", "");
			dstXml.AppendChild (dstConfigNode);

			if (!String.IsNullOrEmpty (baseConfigFile) && File.Exists (baseConfigFile)) {
				var baseXml = new XmlDocument ();
				baseXml.Load (baseConfigFile);
				CopyConfigNode (baseXml, dstConfigNode);
			}

			// Copy over msbuild.dll.config
			{
				var msbuildXml = new XmlDocument ();
				msbuildXml.Load (MSBuildConfig);
				CopyConfigNode (msbuildXml, dstConfigNode);
			}

			var toolsets = dstXml.SelectSingleNode ("configuration/msbuildToolsets/toolset");

			var SearchPathsOS = IsMacOS ? "osx" : "unix";
			var projectImportSearchPaths = toolsets.SelectSingleNode ("projectImportSearchPaths");
			var searchPaths = projectImportSearchPaths.SelectSingleNode ($"searchPaths[@os='{SearchPathsOS}']") as XmlElement;

			//NOTE: on Linux, the searchPaths XML element does not exist, so we have to create it
			if (searchPaths is null) {
				searchPaths = dstXml.CreateElement ("searchPaths");
				searchPaths.SetAttribute ("os", SearchPathsOS);

				var property = dstXml.CreateElement ("property");
				property.SetAttribute ("name", "MSBuildExtensionsPath");
				property.SetAttribute ("value", "");
				searchPaths.AppendChild (property);

				property = dstXml.CreateElement ("property");
				property.SetAttribute ("name", "MSBuildExtensionsPath32");
				property.SetAttribute ("value", "");
				searchPaths.AppendChild (property);

				property = dstXml.CreateElement ("property");
				property.SetAttribute ("name", "MSBuildExtensionsPath64");
				property.SetAttribute ("value", "");
				searchPaths.AppendChild (property);

				projectImportSearchPaths.AppendChild (searchPaths);
			}

			string monoExternal = IsMacOS ? "/Library/Frameworks/Mono.framework/External/" : "/usr/lib/mono";
			string [] ProjectImportSearchPaths = new [] { Environment.GetEnvironmentVariable ("MSBuildExtensionsPathFallbackPathsOverride"), Path.Combine (monoExternal, "xbuild") };

			foreach (XmlNode property in searchPaths.SelectNodes ("property[starts-with(@name, 'MSBuildExtensionsPath')]/@value"))
				property.Value = string.Join (";", ProjectImportSearchPaths);

			SetToolsetProperty ("MSBuildToolsPath", MSBuildBin);
			SetToolsetProperty ("MSBuildToolsPath32", MSBuildBin);
			SetToolsetProperty ("MSBuildToolsPath64", MSBuildBin);
			SetToolsetProperty ("MSBuildExtensionsPath", MSBuildExtensionsPath);
			SetToolsetProperty ("MSBuildExtensionsPath32", MSBuildExtensionsPath);
			SetToolsetProperty ("MSBuildExtensionsPath64", MSBuildExtensionsPath);
			SetToolsetProperty ("RoslynTargetsPath", Path.Combine (MSBuildBin, "Roslyn"));

			dstXml.Save (targetConfigFile);
			return;

			void CopyConfigNode (XmlDocument src, XmlNode dstNode)
			{
				var srcConfigNode = src.SelectSingleNode ("configuration");
				if (srcConfigNode is not null && srcConfigNode.HasChildNodes) {
					srcConfigNode.ChildNodes.OfType<XmlNode> ().ToList ()
							.ForEach (node => dstNode.AppendChild (dstXml.ImportNode (node, deep: true)));
				}
			}

			/// <summary>
			/// If the value exists, sets value attribute, else creates the element
			/// </summary>
			void SetToolsetProperty (string name, string value)
			{
				if (string.IsNullOrEmpty (value))
					return;

				// MSBuild property names are case insensitive
				var valueAttribute = toolsets.SelectSingleNode ($"property[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{name.ToLowerInvariant ()}']/@value");
				if (valueAttribute is not null) {
					valueAttribute.Value = value;
				} else {
					var property = toolsets.OwnerDocument.CreateElement ("property");
					property.SetAttribute ("name", name);
					property.SetAttribute ("value", value);
					toolsets.PrependChild (property);
				}
			}
		}

	}
}
