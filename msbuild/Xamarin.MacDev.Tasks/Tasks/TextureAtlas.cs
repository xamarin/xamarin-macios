using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class TextureAtlas : XcodeToolTaskBase, ICancelableTask {
		readonly Dictionary<string, (ITaskItem Item, List<ITaskItem> Items)> atlases = new ();

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

		protected override string GetBundleRelativeOutputPath (ITaskItem input)
		{
			// Note: if the relative input dir is "relative/texture.atlas", then the relative output path will be "relative/texture.atlasc"
			return Path.ChangeExtension (base.GetBundleRelativeOutputPath (input), ".atlasc");
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
				item.SetMetadata ("LocalDefiningProjectFullPath", input.GetMetadata ("LocalDefiningProjectFullPath"));
				item.SetMetadata ("LocalMSBuildProjectFullPath", input.GetMetadata ("LocalMSBuildProjectFullPath"));

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

			var atlas = atlases [input.ItemSpec];

			foreach (var item in atlas.Items) {
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
				var vpp = BundleResource.GetVirtualProjectPath (this, item);
				var atlasName = Path.GetDirectoryName (vpp);
				var logicalName = item.GetMetadata ("LogicalName");

				if (!atlases.TryGetValue (atlasName, out var atlas)) {
					var atlasItem = new TaskItem (atlasName);
					atlasItem.SetMetadata ("LocalDefiningProjectFullPath", item.GetMetadata ("LocalDefiningProjectFullPath"));
					atlasItem.SetMetadata ("LocalMSBuildProjectFullPath", item.GetMetadata ("LocalMSBuildProjectFullPath"));
					var atlasLogicalName = Path.GetFileNameWithoutExtension (atlasName) + ".plist";
					atlasItem.SetMetadata ("LogicalName", atlasLogicalName);
					atlas = new (atlasItem, new List<ITaskItem> ());
					atlases.Add (atlasName, atlas);
				}

				atlas.Items.Add (item);
			}

			foreach (var atlas in atlases)
				yield return atlas.Value.Item;
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return base.Execute ();
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
