using Ionic.Zip;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xamarin.iOS.HotRestart.Tasks
{
	public class UnpackFrameworks : Task
	{
		#region Inputs

		[Required]
		public ITaskItem[] ReferencedAssemblies { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] Frameworks { get; set; }

		#endregion

		public override bool Execute()
		{
			var frameworks = new List<ITaskItem>();

			Directory.CreateDirectory(Path.Combine(IntermediateOutputPath, "Frameworks"));

			foreach (var assemblyPath in ReferencedAssemblies.Distinct().Where(x => !IsFrameworkItem(x)))
			{
				var assembly = AssemblyDefinition.ReadAssembly(assemblyPath.ItemSpec);

				// We should only get the embedded resources that ends with .framework
				var embeddedFrameworks = assembly.MainModule.Resources.Where(x => Path.GetExtension(x.Name) == ".framework");

				foreach (var resource in embeddedFrameworks)
				{
					var embeddedFramework = resource as EmbeddedResource;

					if (embeddedFramework == null)
						continue;

					var frameworkPath = Path.Combine(IntermediateOutputPath, "Frameworks", embeddedFramework.Name);
					var frameworkZipPath = frameworkPath + ".zip";

					// The frameworks are embedded as zip files
					using (var fileStream = File.OpenWrite(frameworkZipPath))
					{
						embeddedFramework.GetResourceStream().CopyTo(fileStream);
					}

					// Unzip the framework
					using (var zipFile = ZipFile.Read(frameworkZipPath))
					{
						zipFile.ExtractAll(frameworkPath, ExtractExistingFileAction.OverwriteSilently);
					}

					File.Delete(frameworkZipPath);

					var taskItem = new TaskItem(frameworkPath);

					frameworks.Add(taskItem);
				}
			}

			Frameworks = frameworks.ToArray();

			return true;
		}

		// TODO: Remove this once it's moved to the SDK
		bool IsFrameworkItem(ITaskItem item)
		{
			return (bool.TryParse(item.GetMetadata("FrameworkFile"), out var isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata("ResolvedFrom") == "{TargetFrameworkDirectory}" ||
				item.GetMetadata("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}
	}
}