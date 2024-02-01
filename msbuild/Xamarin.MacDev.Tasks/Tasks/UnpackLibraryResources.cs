using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class UnpackLibraryResources : XamarinTask, ITaskCallback, ICancelableTask {
		MetadataLoadContext? universe;
		List<ITaskItem> unpackedResources = new List<ITaskItem> ();

		#region Inputs

		[Required]
		public string Prefix { get; set; } = string.Empty;

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] ReferenceAssemblies { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public ITaskItem [] ReferencedLibraries { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public ITaskItem [] TargetFrameworkDirectory { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		[Output]
		public ITaskItem []? BundleResourcesWithLogicalNames { get; set; }

		// This is required to copy the .items file back to Windows for remote builds.
		[Output]
		public ITaskItem [] ItemsFiles { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] UnpackedResources { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] AtlasTextures { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] ColladaAssets { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] CoreMLModels { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] ImageAssets { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] InterfaceDefinitions { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] SceneKitAssets { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		enum ResourceType {
			AtlasTexture,
			BundleResource,
			ColladaAsset,
			CoreMLModel,
			ImageAsset,
			InterfaceDefinition,
			SceneKitAsset,
		}

		public override bool Execute ()
		{
			try {
				return ExecuteImpl ();
			} finally {
				universe?.Dispose ();
			}
		}

		bool ExecuteImpl ()
		{
			if (ShouldExecuteRemotely ()) {
				var result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

				if (result && BundleResourcesWithLogicalNames is not null) {
					// Fix LogicalName path for Windows
					foreach (var resource in BundleResourcesWithLogicalNames) {
						var logicalName = resource.GetMetadata ("LogicalName");

						if (!string.IsNullOrEmpty (logicalName)) {
							resource.SetMetadata ("LogicalName", logicalName.Replace ("/", "\\"));
						}
					}
				}
				return result;
			}

			var bundleResources = new List<ITaskItem> ();
			var atlasTextures = new List<ITaskItem> ();
			var colladaAssets = new List<ITaskItem> ();
			var coreMLModels = new List<ITaskItem> ();
			var imageAssets = new List<ITaskItem> ();
			var interfaceDefinitions = new List<ITaskItem> ();
			var sceneKitAssets = new List<ITaskItem> ();

			foreach (var asm in ReferencedLibraries) {
				// mscorlib.dll was not coming out with ResolvedFrom == {TargetFrameworkDirectory}
				// and what we really care is where it comes from, not how it was resolved
				if (IsFrameworkAssembly (asm)) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0168, asm.ItemSpec);
				} else {
					var perAssemblyOutputPath = Path.Combine (IntermediateOutputPath, "unpack", asm.GetMetadata ("Filename"));
					var extracted = ExtractContentAssembly (asm.ItemSpec, perAssemblyOutputPath).ToArray ();

					foreach (var tuple in extracted) {
						var resourceType = tuple.Type;
						var item = tuple.Item;
						switch (resourceType) {
						case ResourceType.AtlasTexture:
							atlasTextures.Add (item);
							break;
						case ResourceType.BundleResource:
							bundleResources.Add (item);
							break;
						case ResourceType.ColladaAsset:
							colladaAssets.Add (item);
							break;
						case ResourceType.CoreMLModel:
							coreMLModels.Add (item);
							break;
						case ResourceType.ImageAsset:
							imageAssets.Add (item);
							break;
						case ResourceType.InterfaceDefinition:
							interfaceDefinitions.Add (item);
							break;
						case ResourceType.SceneKitAsset:
							sceneKitAssets.Add (item);
							break;
						default:
							Log.LogError ($"Unknown resource type: {resourceType}"); // FIXME: better error.
							break;
						}
					}
				}
			}

			BundleResourcesWithLogicalNames = bundleResources.ToArray ();
			AtlasTextures = atlasTextures.ToArray ();
			ColladaAssets = colladaAssets.ToArray ();
			CoreMLModels = coreMLModels.ToArray ();
			ImageAssets = imageAssets.ToArray ();
			InterfaceDefinitions = interfaceDefinitions.ToArray ();
			SceneKitAssets = sceneKitAssets.ToArray ();
			UnpackedResources = unpackedResources.ToArray ();

			return !Log.HasLoggedErrors;
		}

		bool IsFrameworkAssembly (ITaskItem asm)
		{
			var asm_path = asm.GetMetadata ("FullPath");
			foreach (var dir in TargetFrameworkDirectory) {
				if (asm_path.StartsWith (dir.GetMetadata ("FullPath"), StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

		IEnumerable<(ResourceType Type, ITaskItem Item)> ExtractContentAssembly (string assembly, string intermediatePath)
		{
			if (!File.Exists (assembly)) {
				Log.LogMessage (MessageImportance.Low, $"Not inspecting assembly because it doesn't exist: {assembly}");
				yield break;
			}

			var asmWriteTime = File.GetLastWriteTimeUtc (assembly);
			var manifestResources = GetAssemblyManifestResources (assembly).ToArray ();
			// Log.LogMessage (MessageImportance.Low, $"Inspecting assembly with {manifestResources.Length} resources: {assembly}");
			if (!manifestResources.Any ()) {
				Log.LogMessage (MessageImportance.Low, $"  No resources found in: {assembly}");
				yield break;
			}

			// Log.LogMessage (MessageImportance.Low, "  Searching resources in assembly: {0}", assembly);
			foreach (var embedded in manifestResources) {
				string rpath;

				var resourceName = embedded.Name;
				var startsWith = "__" + Prefix + "_";
				if (!resourceName.StartsWith (startsWith, StringComparison.Ordinal)) {
					Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (does not match prefix): {resourceName}");
					continue;
				}

				var underscoreIndex = resourceName.IndexOf ('_', startsWith.Length);
				if (underscoreIndex == -1) {
					Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (no content type found): {resourceName}");
					continue;
				}
				var contentType = resourceName.Substring (startsWith.Length, underscoreIndex - startsWith.Length);
				var contentValue = resourceName.Substring (underscoreIndex + 1);
				ResourceType resourceType;
				switch (contentType) {
				case "content":
				case "page":
					rpath = UnmangleResource (contentValue);
					resourceType = ResourceType.BundleResource;
					break;
				case "item":
					var itemUnderscoreIndex = contentValue.IndexOf ('_');
					if (itemUnderscoreIndex == -1) {
						Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (no item type in '{contentValue}'): {resourceName}");
						continue;
					}
					var itemType = contentValue.Substring (0, itemUnderscoreIndex);
					var itemValue = contentValue.Substring (itemUnderscoreIndex + 1);
					rpath = UnmangleResource (itemValue);
					switch (itemType) {
					case "AtlasTexture":
						resourceType = ResourceType.AtlasTexture;
						break;
					case "BundleResource":
						resourceType = ResourceType.BundleResource;
						break;
					case "Collada":
						resourceType = ResourceType.ColladaAsset;
						break;
					case "CoreMLModel":
						resourceType = ResourceType.CoreMLModel;
						break;
					case "ImageAsset":
						resourceType = ResourceType.ImageAsset;
						break;
					case "InterfaceDefinition":
						resourceType = ResourceType.InterfaceDefinition;
						break;
					case "SceneKitAsset":
						resourceType = ResourceType.SceneKitAsset;
						break;
					default:
						Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (unknown item type in '{itemType}'): {resourceName}");
						continue;
					}
					break;
				default:
					Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (unknown content type '{contentType}'): {resourceName}");
					continue;
				}

				var path = Path.Combine (intermediatePath, rpath);
				var file = new FileInfo (path);

				var item = new TaskItem (path);
				item.SetMetadata ("LogicalName", rpath);
				item.SetMetadata ("Optimize", "false");
				item.SetMetadata ("BundledInAssembly", assembly);

				if (file.Exists && file.LastWriteTimeUtc >= asmWriteTime) {
					Log.LogMessage ($"    Up to date (contentType: {contentType} resourceType: {resourceType}: {path}");
				} else {
					Log.LogMessage ($"    Unpacking (contentType: {contentType} resourceType: {resourceType}: {path}");

					Directory.CreateDirectory (Path.GetDirectoryName (path));

					using (var stream = File.Open (path, FileMode.Create)) {
						using (var resource = embedded.Open ())
							resource.CopyTo (stream);
					}

					unpackedResources.Add (item);
				}

				yield return (resourceType, item);
			}

			yield break;
		}

		static string UnmangleResource (string mangled)
		{
			var unmangled = new StringBuilder (mangled.Length);
			bool escaped = false;

			for (int i = 0; i < mangled.Length; i++) {
				char c = mangled [i];

				if (c == '_' && !escaped) {
					escaped = true;
					continue;
				}

				if (escaped) {
					switch (c) {
					case 'b': c = '\\'; break;
					case 'f': c = '/'; break;
					case '_': c = '_'; break;
					default: throw new FormatException ("Invalid resource name: " + mangled);
					}

					escaped = false;
				}

				unmangled.Append (c);
			}

			if (escaped)
				throw new FormatException ("Invalid resource name: " + mangled);

			return unmangled.ToString ();
		}

		public class ManifestResource {
			readonly Func<Stream> callback;

			public ManifestResource (string name, Func<Stream> streamCallback)
			{
				callback = streamCallback;
				Name = name;
			}

			public string Name {
				get; private set;
			}

			public Stream Open ()
			{
				return callback ();
			}
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			if (item.IsFrameworkItem ())
				return false;

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => UnpackedResources.Contains (item) == true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => ItemsFiles;

		IEnumerable<ManifestResource> GetAssemblyManifestResources (string fileName)
		{
			if (universe is null)
				universe = new MetadataLoadContext (new PathAssemblyResolver (ReferenceAssemblies.Select (v => v.ItemSpec)));

			Assembly assembly;
			try {
				assembly = universe.LoadFromAssemblyPath (fileName);
			} catch (Exception e) {
				Log.LogMessage (MessageImportance.Low, $"Unable to load the assembly '{fileName}: {e}");
				yield break;
			}

			foreach (var resourceName in assembly.GetManifestResourceNames ()) {
				var info = assembly.GetManifestResourceInfo (resourceName);
				if (!info.ResourceLocation.HasFlag (ResourceLocation.Embedded))
					continue;
				yield return new ManifestResource (resourceName, () => assembly.GetManifestResourceStream (resourceName));
			}
		}
	}
}
