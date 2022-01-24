using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xamarin.iOS.Tasks.Windows;

namespace Xamarin.iOS.HotRestart.Tasks {
	public class UnpackFrameworks : Task {
		#region Inputs

		[Required]
		public ITaskItem [] ReferencedAssemblies { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] Frameworks { get; set; }

		#endregion

		public override bool Execute ()
		{
			var frameworks = new List<ITaskItem> ();

			Directory.CreateDirectory (Path.Combine (IntermediateOutputPath, "Frameworks"));

			foreach (var assemblyPath in ReferencedAssemblies.Distinct ().Where (x => !IsFrameworkItem (x))) {
				var assembly = AssemblyDefinition.ReadAssembly (assemblyPath.ItemSpec);
				// We should only get the embedded resources that ends with .framework
				var embeddedFrameworks = assembly.MainModule.Resources.Where (x => Path.GetExtension (x.Name) == ".framework");

				foreach (var resource in embeddedFrameworks) {
					var embeddedFramework = resource as EmbeddedResource;

					if (embeddedFramework == null)
						continue;

					var frameworkPath = Path.Combine (IntermediateOutputPath, "Frameworks", embeddedFramework.Name);
					var frameworkZipPath = frameworkPath + ".zip";

					// The frameworks are embedded as zip files
					using (var fileStream = File.OpenWrite (frameworkZipPath)) {
						embeddedFramework.GetResourceStream ().CopyTo (fileStream);
					}

					Zip.Extract (frameworkZipPath, frameworkPath);
					File.Delete (frameworkZipPath);

					frameworks.Add (new TaskItem (frameworkPath));
				}
			}

			Frameworks = frameworks.ToArray ();

			return true;
		}

		bool IsFrameworkItem (ITaskItem item)
		{
			return (bool.TryParse (item.GetMetadata ("FrameworkFile"), out var isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata ("ResolvedFrom") == "{TargetFrameworkDirectory}" ||
				item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}
	}
}
