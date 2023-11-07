using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace Xamarin.iOS.Tasks {
	public class CollectMonotouchReferences : Task {

		const string MsBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		const string MonoTouchUnifiedProjectGuid = "FEACFBD2-3405-455C-9665-78FE426C6842";

		#region Inputs

		public ITaskItem [] References { get; set; }

		#endregion

		[Output]
		public ITaskItem [] MonoTouchReferences { get; set; }

		public override bool Execute ()
		{
			var result = new List<ITaskItem> ();

			if (References is not null) {
				foreach (var reference in References) {
					try {
						var oldProjectSystem = false;

						using (var reader = XmlReader.Create (reference.ItemSpec)) {
							if (reader.ReadToDescendant ("ProjectTypeGuids", MsBuildNamespace)) {
								oldProjectSystem = true;

								var projectTypeGuids = reader.ReadString ();

								if (projectTypeGuids is null)
									projectTypeGuids = string.Empty;

								projectTypeGuids = projectTypeGuids.ToUpperInvariant ();

								if (projectTypeGuids.Contains (MonoTouchUnifiedProjectGuid.ToUpperInvariant ())) {
									result.Add (reference);
								}
							}
						}

						if (!oldProjectSystem) {
							var proj = Project.FromFile (reference.ItemSpec, new ProjectOptions ());
							var capabilities = proj.GetItems ("ProjectCapability");

							if (capabilities.Any (x => x.EvaluatedInclude == "Apple") && capabilities.Any (x => x.EvaluatedInclude == "Managed")) {
								result.Add (reference);
							}
						}
					} catch {
						continue;
					}
				}
			}

			MonoTouchReferences = result.ToArray ();

			return true;
		}
	}
}
