using System;
using System.Diagnostics;
using NUnit.Framework;
using Xamarin.Tests;

namespace AppleSdkSettings {

	public static class XcodeVersion {
		public static int Major { get { return Configuration.XcodeVersion.Major; } }
	}
}

namespace Xamarin.MacDev.Tasks {

	public class MetalPoker : Metal {
		public new string GenerateFullPathToTool ()
		{
			return base.GenerateFullPathToTool ();
		}
	}

	public class MetalLibPoker : MetalLib {

		public new string GenerateFullPathToTool ()
		{
			return base.GenerateFullPathToTool ();
		}
	}

	[TestFixture]
	public class ToolTasksBinPathTest {

		[Test]
		public void MetalBinPathTest ()
		{
			var metalTask = new MetalPoker ();
			metalTask.SdkDevPath = string.Empty;
			CheckToolBinDir ("metal", metalTask.GenerateFullPathToTool ());
		}

		[Test]
		public void MetalLibBinPathTest ()
		{
			var metalLibTask = new MetalLibPoker ();
			metalLibTask.SdkDevPath = string.Empty;
			CheckToolBinDir ("metallib", metalLibTask.GenerateFullPathToTool ());
		}

		public void CheckToolBinDir (string taskName, string binDirToCheck)
		{
			var psi = new ProcessStartInfo ("xcrun") {
				Arguments = $"-f {taskName}",
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			psi.EnvironmentVariables ["DEVELOPER_DIR"] = Configuration.xcode_root;
			psi.EnvironmentVariables.Remove ("XCODE_DEVELOPER_DIR_PATH"); // VSfM sets XCODE_DEVELOPER_DIR_PATH, which confuses the command-line tools if it doesn't match xcode-select, so just unset it.
			var proc = Process.Start (psi);

			string output = proc.StandardOutput.ReadToEnd ();
			string err = proc.StandardError.ReadToEnd ();

			Assert.True (output.Contains (binDirToCheck), err);
		}
	}
}
