using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Mono.Options;

namespace xibuild {
	class MainClass {

		public static int Main (string [] args)
		{
			bool runTool = false;
			bool configGenerationOnly = false;
			string baseConfigFile = null;

			OptionSet p = new OptionSet ()
				.Add ("c", c => configGenerationOnly = true) // merge
				.Add ("t", t => runTool = true)
				.Add ("m=", m => baseConfigFile = m)
				.Add ("h|?|help", v => ShowHelp ());

			Console.WriteLine ($"Running xibuild with args: {String.Join (" ", args)}");
			List<string> remaining = null;

			try {
				remaining = p.Parse (args);
			} catch (OptionException oe) {
				Console.WriteLine (oe.Message);
				return -1;
			}

			if (configGenerationOnly && runTool) {
				Console.WriteLine ("Use either -c or -t, but not both.\n");
				ShowHelp ();
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
				GenerateAppConfig (remaining [0] + ".config", baseConfigFile);
				return 0;
			}

			return RunTool (
					toolPath: runMSBuild ? "msbuild" : remaining [0],
					combinedArgs: BuildQuotedCommandLine (remaining, runMSBuild ? 0 : 1),
					baseConfigFile: runMSBuild ? null : baseConfigFile);

			string BuildQuotedCommandLine (List<string> a, int skip) => String.Join (" ", a.Skip (skip).Select (arg => $"\"{arg}\""));

			void ShowHelp ()
			{
				Console.WriteLine ("xibuild: Run msbuild or a tool with a custom msbuild config file which adds fallback paths from MSBuildExtensionsPathFallbackPathsOverride.\n");
				Console.WriteLine ("Usage: xibuild [-c] [-t] [-m <base config file>] [-h] -- [path to managed tool] [arguments]");
				Console.WriteLine ("Default: Generate a temporary app.config file and run msbuild");
				Console.WriteLine ();
				Console.WriteLine ("\t-c: Generate config file only");
				Console.WriteLine ("\t-t: Path to the managed tool to run. If this and `-c` are not used, then this runs msbuild");
				Console.WriteLine ("\t-m <base config file>: Config file to merge with the one from msbuild.dll.config");
				Console.WriteLine ("\t-h: help");
				Console.WriteLine ("\nNote: Adds the path from the environment variable MSBuildExtensionsPathFallbackPathsOverride to the list of fallback paths in the generated app.config");
				Console.WriteLine ("\nExamples:");
				Console.WriteLine ("xibuild -c /path/to/foo.exe : Generate /path/to/foo.exe.config");
				Console.WriteLine ("xibuild -- /v:diag /path/to/project.csproj : Run msbuild with a custom app.config and the arguments passed to msbuild.");
				Console.WriteLine ("xibuild -t -- /path/to/managed_tool.exe args : Run managed_tool.exe with the arguments, and a custom app.config");
				Console.WriteLine ("");
				Console.WriteLine ("Add `-m /path/to/base.exe.config` to merge the generated app.config with base.exe.config .");
			}
		}

		static int RunTool (string toolPath, string combinedArgs, string baseConfigFile)
		{
			var tmpMSBuildExePathForConfig = Path.GetTempFileName ();
			var configFilePath = tmpMSBuildExePathForConfig + ".config";

			GenerateAppConfig (configFilePath, baseConfigFile);

			// Required so that msbuild can read the correct config file
			Environment.SetEnvironmentVariable ("MSBUILD_EXE_PATH", tmpMSBuildExePathForConfig, EnvironmentVariableTarget.Process);

			var p = Process.Start (new ProcessStartInfo {
				FileName = toolPath,
				Arguments = combinedArgs,
				UseShellExecute = false,
				RedirectStandardInput = true,
			});

			p.WaitForExit ();
			return p.ExitCode;
		}

		static void GenerateAppConfig (string targetConfigFile, string baseConfigFile)
		{
			bool IsMacOS = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.OSX);

			//FIXME: Current? or from PATH?
			string mono = IsMacOS ? "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono" : "/usr/lib/mono";
			string vsVersion = "15.0";
			string MSBuildBin = Path.Combine (mono, "msbuild", vsVersion, "bin");

			string MSBuildConfig = Path.Combine (MSBuildBin, "MSBuild.dll.config");
			string MSBuildExtensionsPath = Path.Combine (mono, "xbuild");
			string FrameworksDirectory = Path.Combine (mono, "xbuild-frameworks");
			string MSBuildSdksPath = Path.Combine (MSBuildBin, "Sdks");

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
			if (searchPaths == null) {
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
			SetToolsetProperty ("TargetFrameworkRootPath", FrameworksDirectory + Path.DirectorySeparatorChar); //NOTE: Must include trailing \
			SetToolsetProperty ("MSBuildSdksPath", MSBuildSdksPath);

			dstXml.Save (targetConfigFile);
			return;

			void CopyConfigNode (XmlDocument src, XmlNode dstNode)
			{
				var srcConfigNode = src.SelectSingleNode ("configuration");
				if (srcConfigNode != null && srcConfigNode.HasChildNodes) {
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

				var valueAttribute = toolsets.SelectSingleNode ($"property[@name='{name}']/@value");
				if (valueAttribute != null) {
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
