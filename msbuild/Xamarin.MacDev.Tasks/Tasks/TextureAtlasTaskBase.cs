using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks {
	public abstract class TextureAtlasTaskBase : XcodeToolTaskBase {
		readonly Dictionary<string, List<ITaskItem>> atlases = new Dictionary<string, List<ITaskItem>> ();

		#region Inputs

		public ITaskItem [] AtlasTextures { get; set; }

		#endregion

		protected override string DefaultBinDir {
			get { return DeveloperRootBinDir; }
		}

		protected override string ToolName {
			get { return "TextureAtlas"; }
		}

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineBuilder args, ITaskItem input, ITaskItem output)
		{
			args.AppendFileNameIfNotNull (input.GetMetadata ("FullPath"));
			args.AppendFileNameIfNotNull (Path.GetDirectoryName (output.GetMetadata ("FullPath")));
		}

		protected override string GetBundleRelativeOutputPath (IList<string> prefixes, ITaskItem input)
		{
			// Note: if the relative input dir is "relative/texture.atlas", then the relative output path will be "relative/texture.atlasc"
			return Path.ChangeExtension (base.GetBundleRelativeOutputPath (prefixes, input), ".atlasc");
		}

		protected override IEnumerable<ITaskItem> GetCompiledBundleResources (ITaskItem input, ITaskItem output)
		{
			var bundleDir = output.GetMetadata ("LogicalName");

			if (!Directory.Exists (output.ItemSpec))
				yield break;

			foreach (var file in Directory.GetFiles (output.ItemSpec)) {
				var fileName = Path.GetFileName (file);

				var relative = Path.Combine (output.ItemSpec, fileName);
				var logical = Path.Combine (bundleDir, fileName);
				var item = new TaskItem (relative);

				item.SetMetadata ("LogicalName", logical);
				item.SetMetadata ("Optimize", "false");

				yield return item;
			}

			yield break;
		}

		protected override bool NeedsBuilding (ITaskItem input, ITaskItem output)
		{
			var plist = Path.Combine (output.ItemSpec, Path.GetFileNameWithoutExtension (input.ItemSpec) + ".plist");

			if (!File.Exists (plist))
				return true;

			var items = atlases [input.ItemSpec];

			foreach (var item in items) {
				if (File.GetLastWriteTimeUtc (item.ItemSpec) > File.GetLastWriteTimeUtc (plist))
					return true;
			}

			return false;
		}

		protected override IEnumerable<ITaskItem> EnumerateInputs ()
		{
			if (AtlasTextures is null)
				yield break;

			// group the atlas textures by their parent .atlas directories
			foreach (var item in AtlasTextures) {
				var atlas = Path.GetDirectoryName (BundleResource.GetVirtualProjectPath (ProjectDir, item, !string.IsNullOrEmpty (SessionId)));
				List<ITaskItem> items;

				if (!atlases.TryGetValue (atlas, out items)) {
					items = new List<ITaskItem> ();
					atlases.Add (atlas, items);
				}

				items.Add (item);
			}

			foreach (var atlas in atlases.Keys)
				yield return new TaskItem (atlas);

			yield break;
		}
	}
}
