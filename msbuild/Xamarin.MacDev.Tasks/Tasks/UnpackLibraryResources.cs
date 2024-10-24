using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class UnpackLibraryResources : XamarinTask, ITaskCallback, ICancelableTask {
		List<ITaskItem> unpackedResources = new List<ITaskItem> ();

		#region Inputs

		[Required]
		public string Prefix { get; set; } = string.Empty;

		[Required]
		public string IntermediateOutputPath { get; set; } = string.Empty;

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
		public ITaskItem [] PartialAppManifests { get; set; } = Array.Empty<ITaskItem> ();

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
			PartialAppManifest,
			SceneKitAsset,
		}

		public override bool Execute ()
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
			var partialAppManifests = new List<ITaskItem> ();
			var sceneKitAssets = new List<ITaskItem> ();

			foreach (var asm in ReferencedLibraries) {
				// mscorlib.dll was not coming out with ResolvedFrom == {TargetFrameworkDirectory}
				// and what we really care is where it comes from, not how it was resolved
				if (IsFrameworkAssembly (asm)) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0168, asm.ItemSpec);
				} else {
					var perAssemblyOutputPath = Path.Combine (IntermediateOutputPath, "unpack", asm.GetMetadata ("Filename"));
					var extracted = ExtractContentAssembly (asm.ItemSpec, perAssemblyOutputPath);

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
						case ResourceType.PartialAppManifest:
							partialAppManifests.Add (item);
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
			PartialAppManifests = partialAppManifests.ToArray ();
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

		class AssemblyResource {
			ResourceExtractor Extractor;
			public ManifestResource ManifestResource;
			public string Name;
			public AssemblyResource (ResourceExtractor extractor, ManifestResource manifestResource, string name)
			{
				Extractor = extractor;
				ManifestResource = manifestResource;
				Name = name;
			}
		}

		class ResourceExtractor : IDisposable, IEnumerable<AssemblyResource> {
			string assembly;
			FileStream? peStream;
			PEReader? peReader;
			MetadataReader? metadataReader;
			TaskLoggingHelper log;

			public ResourceExtractor (TaskLoggingHelper log, string assembly)
			{
				this.log = log;
				this.assembly = assembly;
			}

			public void Dispose ()
			{
				peReader?.Dispose ();
				peReader = null;
				peStream?.Dispose ();
				peStream = null;
			}

			public void WriteResourceTo (AssemblyResource resource, string path)
			{
				if (peReader is null)
					throw new ObjectDisposedException ("this");

				Directory.CreateDirectory (Path.GetDirectoryName (path));

				var manifestResource = resource.ManifestResource;
				var resourceDirectory = peReader.GetSectionData (peReader.PEHeaders.CorHeader!.ResourcesDirectory.RelativeVirtualAddress);
				var reader = resourceDirectory.GetReader ((int) manifestResource.Offset, resourceDirectory.Length - (int) manifestResource.Offset);
				var length = reader.ReadUInt32 ();
				if (length > reader.RemainingBytes)
					throw new BadImageFormatException ();
#if NET
				using var fs = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.Read);
				unsafe {
					var span = new ReadOnlySpan<byte> (reader.CurrentPointer, (int) length);
					fs.Write (span);
				}
#else
				var buffer = new byte [4096];
				using var fs = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.Read, buffer.Length);
				var left = (int) length;
				while (left > 0) {
					var read = Math.Min (left, buffer.Length);
					reader.ReadBytes (read, buffer, 0);
					fs.Write (buffer, 0, read);
					left -= read;
				}
#endif
			}

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return ((IEnumerable<AssemblyResource>) this).GetEnumerator ();
			}

			IEnumerator<AssemblyResource> IEnumerable<AssemblyResource>.GetEnumerator ()
			{
				if (!File.Exists (assembly))
					yield break;

				if (peStream is null) {
					peStream = File.OpenRead (assembly);
					peReader = new PEReader (peStream);
					metadataReader = PEReaderExtensions.GetMetadataReader (peReader);
					log.LogMessage (MessageImportance.Low, $"Inspecting resources in assembly {assembly}");
				}

				foreach (var manifestResourceHandle in metadataReader!.ManifestResources) {
					var manifestResource = metadataReader.GetManifestResource (manifestResourceHandle);
					if (!manifestResource.Implementation.IsNil)
						continue; // embedded resources have Implementation.IsNil = true, and those are the ones we care about

					var name = metadataReader.GetString (manifestResource.Name);
					if (string.IsNullOrEmpty (name))
						continue;

					yield return new AssemblyResource (this, manifestResource, name);
				}
			}
		}

		class AssemblyContentResource {
			public ResourceType Type;
			public ITaskItem Item;
			public AssemblyContentResource (ResourceType type, ITaskItem item)
			{
				Type = type;
				Item = item;
			}
		}

		List<AssemblyContentResource> ExtractContentAssembly (string assembly, string intermediatePath)
		{
			var rv = new List<AssemblyContentResource> ();

			if (!File.Exists (assembly)) {
				Log.LogMessage (MessageImportance.Low, $"Not inspecting assembly because it doesn't exist: {assembly}");
				return rv;
			}

			var asmWriteTime = File.GetLastWriteTimeUtc (assembly);
			using var extractor = new ResourceExtractor (Log, assembly);

			try {
				// Log.LogMessage (MessageImportance.Low, "  Searching resources in assembly: {0}", assembly);
				foreach (var embedded in extractor) {
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
					string itemType;
					switch (contentType) {
					case "content":
					case "page":
						rpath = UnmangleResource (contentValue);
						resourceType = ResourceType.BundleResource;
						itemType = contentType;
						break;
					case "item":
						var itemUnderscoreIndex = contentValue.IndexOf ('_');
						if (itemUnderscoreIndex == -1) {
							Log.LogMessage (MessageImportance.Low, $"    Not applicable resource (no item type in '{contentValue}'): {resourceName}");
							continue;
						}
						itemType = contentValue.Substring (0, itemUnderscoreIndex);
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
						case "PartialAppManifest":
							resourceType = ResourceType.PartialAppManifest;
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

					var path = Path.Combine (intermediatePath, itemType, rpath);
					var file = new FileInfo (path);

					var item = new TaskItem (path);
					item.SetMetadata ("LogicalName", rpath);
					item.SetMetadata ("Optimize", "false");
					item.SetMetadata ("BundledInAssembly", assembly);

					if (file.Exists && file.LastWriteTimeUtc >= asmWriteTime) {
						Log.LogMessage ($"    Up to date (contentType: {contentType} resourceType: {resourceType} resourceName: {resourceName}): {path}");
					} else {
						extractor.WriteResourceTo (embedded, path);
						unpackedResources.Add (item);
						Log.LogMessage ($"    Unpacked (contentType: {contentType} resourceType: {resourceType}  resourceName: {resourceName}): {path}");
					}
					rv.Add (new AssemblyContentResource (resourceType, item));
				}
			} catch (Exception e) {
				Log.LogMessage (MessageImportance.Low, $"Unable to load the resources from the assembly '{assembly}': {e}");
				return new List<AssemblyContentResource> ();
			}
			return rv;
		}

		// The opposite function is PackLibraryResources.EscapeMangledResource
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
					case 's': c = Path.DirectorySeparatorChar; break;
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

		public bool ShouldCreateOutputFile (ITaskItem item)
		{
			// Incremental builds are handled with stamp files in the .targets file, so there's no need to
			// create any output files on Windows.
			return false;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => ItemsFiles;

	}
}
