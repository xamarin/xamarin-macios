using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xamarin.iOS.HotRestart.Tasks
{
	public class CollectDynamicFrameworks : Task
	{
		#region Inputs

		[Required]
		public ITaskItem[] Frameworks { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] DynamicFrameworks { get; set; }

		#endregion

		public override bool Execute()
		{
			var frameworks = new List<ITaskItem>();

			foreach (var framework in Frameworks.Where(f => Path.GetExtension(f.ItemSpec.TrimEnd('\\')) == ".framework"))
			{
				framework.ItemSpec = framework.ItemSpec.TrimEnd('\\');

				if (frameworks.Any(x => x.ItemSpec == framework.ItemSpec))
				{
					continue;
				}

				var frameworkDirName = Path.GetFileName(framework.ItemSpec);

				// TODO: iSign.Core ref
				//try
				//{
				//	MachOReader.Load(Path.Combine(framework.ItemSpec, Path.GetFileNameWithoutExtension(frameworkDirName)), false, CPUType.CPU_TYPE_ARM64);
				//}
				//catch (Exception ex)
				//{
				//	if (ex is MissingInfoPlistException || ex is InvalidMachOTypeException)
				//	{
				//		Log.LogMessage(MessageImportance.Normal, Resources.CollectDynamicFrameworks_InvalidFramework, Path.GetFileName(framework.ItemSpec), ex.Message);
				//		continue;
				//	}
				//	else
				//	{
				//		Log.LogErrorFromException(ex);
				//		break;
				//	}
				//}

				framework.SetMetadata("FrameworkDir", $@"{frameworkDirName}\");

				frameworks.Add(framework);
			}

			DynamicFrameworks = frameworks.ToArray();

			return !Log.HasLoggedErrors;
		}
	}
}