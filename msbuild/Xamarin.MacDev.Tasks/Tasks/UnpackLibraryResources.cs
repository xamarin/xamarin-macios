using System;
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

		#endregion

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

			var results = new List<ITaskItem> ();

			foreach (var asm in ReferencedLibraries) {
				// mscorlib.dll was not coming out with ResolvedFrom == {TargetFrameworkDirectory}
				// and what we really care is where it comes from, not how it was resolved
				if (IsFrameworkAssembly (asm)) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0168, asm.ItemSpec);
				} else {
					var perAssemblyOutputPath = Path.Combine (IntermediateOutputPath, "unpack", asm.GetMetadata ("Filename"));
					var extracted = ExtractContentAssembly (asm.ItemSpec, perAssemblyOutputPath);

					results.AddRange (extracted);

					var itemsFile = asm.GetMetadata ("ItemsFile");
					itemsFile = itemsFile.Replace ('\\', Path.DirectorySeparatorChar);
					WriteItemsToFile.Write (this, itemsFile, extracted, "_BundleResourceWithLogicalName", true, true);
				}
			}

			BundleResourcesWithLogicalNames = results.ToArray ();
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

		List<ITaskItem> ExtractContentAssembly (string assembly, string intermediatePath)
		{
			var rv = new List<ITaskItem> ();

			if (!File.Exists (assembly)) {
				Log.LogMessage (MessageImportance.Low, $"Not inspecting assembly because it doesn't exist: {assembly}");
				return rv;
			}

			try {
				var asmWriteTime = File.GetLastWriteTimeUtc (assembly);
				using var peStream = File.OpenRead (assembly);
				using var peReader = new PEReader (peStream);
				var metadataReader = PEReaderExtensions.GetMetadataReader (peReader);
				Log.LogMessage (MessageImportance.Low, $"Inspecting resources in assembly {assembly}");
				foreach (var manifestResourceHandle in metadataReader.ManifestResources) {
					var manifestResource = metadataReader.GetManifestResource (manifestResourceHandle);
					if (!manifestResource.Implementation.IsNil)
						continue; // embedded resources have Implementation.IsNil = true, and those are the ones we care about

					var name = metadataReader.GetString (manifestResource.Name);
					if (string.IsNullOrEmpty (name))
						continue;

					string rpath;

					if (name.StartsWith ("__" + Prefix + "_content_", StringComparison.Ordinal)) {
						var mangled = name.Substring (("__" + Prefix + "_content_").Length);
						rpath = UnmangleResource (mangled);
					} else if (name.StartsWith ("__" + Prefix + "_page_", StringComparison.Ordinal)) {
						var mangled = name.Substring (("__" + Prefix + "_page_").Length);
						rpath = UnmangleResource (mangled);
					} else {
						continue;
					}

					var path = Path.Combine (intermediatePath, rpath);
					var file = new FileInfo (path);

					var item = new TaskItem (path);
					item.SetMetadata ("LogicalName", rpath);
					item.SetMetadata ("Optimize", "false");

					if (file.Exists && file.LastWriteTimeUtc >= asmWriteTime) {
						Log.LogMessage ("    Up to date: {0}", rpath);
					} else {
						Log.LogMessage ("    Unpacking: {0}", rpath);

						Directory.CreateDirectory (Path.GetDirectoryName (path));

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
						unpackedResources.Add (item);
					}

					rv.Add (item);
				}
			} catch (Exception e) {
				Log.LogMessage (MessageImportance.Low, $"Unable to load the resources from the assembly '{assembly}': {e}");
				return new List<ITaskItem> ();
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

	}
}
