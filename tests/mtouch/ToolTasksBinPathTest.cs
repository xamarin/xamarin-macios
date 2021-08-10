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

	public abstract class MetalTaskBase {

		protected virtual string OperatingSystem {
			get {
				throw new NotImplementedException ();
			}
		}

		protected abstract string DevicePlatformBinDir {
			get;
		}

		protected string SdkDevPath {
			get { return ""; }
		}
	}

	public abstract class MetalLibTaskBase {

		protected abstract string DevicePlatformBinDir {
			get;
		}

		protected string SdkDevPath {
			get { return ""; }
		}
	}
}

namespace Xamarin.Mac.Tasks {

	public class MetalPoker : Metal {

		public new string DevicePlatformBinDir {
			get { return base.DevicePlatformBinDir; }
		}
	}

	public class MetalLibPoker : MetalLib {

		public new string DevicePlatformBinDir {
			get { return base.DevicePlatformBinDir; }
		}
	}

	[TestFixture]
	public class ToolTasksBinPathTest {

		[Test]
		public void MetalBinPathTest ()
		{
			var metalTask = new MetalPoker ();
			CheckToolBinDir ("metal", metalTask.DevicePlatformBinDir);
		}

		[Test]
		public void MetalLibBinPathTest ()
		{
			var metalLibTask = new MetalLibPoker ();
			CheckToolBinDir ("metallib", metalLibTask.DevicePlatformBinDir);
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
			psi.EnvironmentVariables.Add ("DEVELOPER_DIR", Configuration.xcode_root);
			psi.EnvironmentVariables.Remove ("XCODE_DEVELOPER_DIR_PATH"); // VSfM sets XCODE_DEVELOPER_DIR_PATH, which confuses the command-line tools if it doesn't match xcode-select, so just unset it.
			var proc = Process.Start (psi);

			string output = proc.StandardOutput.ReadToEnd ();
			string err = proc.StandardError.ReadToEnd ();

			Assert.True (output.Contains (binDirToCheck), err);
		}
	}
}
