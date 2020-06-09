using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class CompileNativeCodeTaskBase : XamarinTask {

#region Inputs
		[Required]
		public ITaskItem [] CompileInfo { get; set; }

		[Required]
		public string MinimumOSVersion { get; set; }

		[Output]
		public ITaskItem [] ObjectFiles { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		[Required]
		public string SdkRoot { get; set; }

		[Required]
		public bool SdkIsSimulator { get; set; }
#endregion

		public override bool Execute ()
		{
			var processes = new Task<Execution> [CompileInfo.Length];
			var objectFiles = new List<ITaskItem> ();

			if (ObjectFiles != null)
				objectFiles.AddRange (ObjectFiles);

			for (var i = 0; i < CompileInfo.Length; i++) {
				var info = CompileInfo [i];
				var src = Path.GetFullPath (info.ItemSpec);
				var arguments = new List<string> ();

				arguments.Add ("clang");

				arguments.Add (PlatformFrameworkHelper.GetMinimumVersionArgument (TargetFrameworkMoniker, SdkIsSimulator, MinimumOSVersion));

				arguments.Add ("-isysroot");
				arguments.Add (SdkRoot);

				var args = info.GetMetadata ("Arguments");
				if (!StringUtils.TryParseArguments (args, out var parsed_args, out var ex)) {
					Log.LogError ("Could not parse the arguments '{0}': {1}", args, ex.Message);
					return false;
				}
				arguments.AddRange (parsed_args);

				var arch = info.GetMetadata ("Arch");
				if (!string.IsNullOrEmpty (arch)) {
					arguments.Add ("-arch");
					arguments.Add (arch);
				}

				var outputFile = info.GetMetadata ("OutputFile");
				if (string.IsNullOrEmpty (outputFile))
					outputFile = Path.ChangeExtension (src, ".o");
				outputFile = Path.GetFullPath (outputFile);
				arguments.Add ("-o");
				arguments.Add (outputFile);
				objectFiles.Add (new TaskItem (outputFile));

				arguments.Add ("-c");
				arguments.Add (src);

				processes [i] = ExecuteAsync ("xcrun", arguments, sdkDevPath: SdkDevPath);
			}

			System.Threading.Tasks.Task.WaitAll (processes);

			ObjectFiles = objectFiles.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
