using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class TextureAtlas : XcodeToolTaskBase, ICancelableTask {
		readonly Dictionary<string, (string LogicalName, List<ITaskItem> Items)> atlases = new ();

		#region Inputs

		public ITaskItem [] AtlasTextures { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		protected override string DefaultBinDir {
			get { return DeveloperRootBinDir; }
		}

		protected override string ToolName {
			get { return "TextureAtlas"; }
		}

		protected override void AppendCommandLineArguments (IDictionary<string, string> environment, CommandLineBuilder args, ITaskItem input, ITaskItem output)
		{
			Log.LogWarning ($"AppendCommandLineArguments ({input.ItemSpec}, {output.ItemSpec}) => {input.GetMetadata ("FullPath")} {output.GetMetadata ("FullPath")}");
			args.AppendFileNameIfNotNull (input.GetMetadata ("FullPath"));
			args.AppendFileNameIfNotNull (Path.GetDirectoryName (output.GetMetadata ("FullPath")));
		}

		protected override string GetBundleRelativeOutputPath (IList<string> prefixes, ITaskItem input)
		{
			// Note: if the relative input dir is "relative/texture.atlas", then the relative output path will be "relative/texture.atlasc"
			var rv = Path.ChangeExtension (base.GetBundleRelativeOutputPath (prefixes, input), ".atlasc");
			Log.LogWarning ($"GetBundleRelativeOutputPath ({input.ItemSpec}) => {rv}");
			return rv;
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

				Log.LogWarning ($"GetCompiledBundleResources ({input.ItemSpec}, {output.ItemSpec}) => {item.ItemSpec} LogicalName={logical}");

				yield return item;
			}

			yield break;
		}

		protected override bool NeedsBuilding (ITaskItem input, ITaskItem output)
		{
			var plist = Path.Combine (output.ItemSpec, Path.GetFileNameWithoutExtension (input.ItemSpec) + ".plist");

			if (!File.Exists (plist))
				return true;

			var items = atlases [input.ItemSpec].Items;

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
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			foreach (var item in AtlasTextures) {
				var vpp = BundleResource.GetVirtualProjectPath (this, ProjectDir, item);
				var atlas = Path.GetDirectoryName (vpp);
				Log.LogWarning ($"Processing atlas {item.ItemSpec} with LogicalName={item.GetMetadata ("LogicalName")} VirtualProjectPath={vpp} and atlas name {atlas}");

				if (!atlases.TryGetValue (atlas, out var tuple)) {
					tuple.Items = new List<ITaskItem> ();
					var itemLogicalName = BundleResource.GetLogicalName (this, ProjectDir, prefixes, item);
					tuple.LogicalName = Path.GetDirectoryName (itemLogicalName);
					atlases.Add (atlas, tuple);
					Log.LogWarning ($"    => created new atlas {atlas} with new LogicalName {tuple.LogicalName} and itemLogicalName {itemLogicalName}");
				} else {
					Log.LogWarning ($"    => added to atlas {atlas} with LogicalName {tuple.LogicalName}");
				}
				var items = tuple.Items;

				items.Add (item);
			}

			foreach (var kvp in atlases) {
				var atlas = kvp.Key;
				var logicalName = kvp.Value.LogicalName;
				var rv = new TaskItem (atlas);
				rv.SetMetadata ("LogicalName", logicalName);
				yield return rv;
			}

			yield break;
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
