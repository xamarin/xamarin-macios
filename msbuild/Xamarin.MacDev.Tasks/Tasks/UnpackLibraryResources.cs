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

		#endregion

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

			var results = new List<ITaskItem> ();

			foreach (var asm in ReferencedLibraries) {
				// mscorlib.dll was not coming out with ResolvedFrom == {TargetFrameworkDirectory}
				// and what we really care is where it comes from, not how it was resolved
				if (IsFrameworkAssembly (asm)) {
					Log.LogMessage (MessageImportance.Low, MSBStrings.M0168, asm.ItemSpec);
				} else {
					var perAssemblyOutputPath = Path.Combine (IntermediateOutputPath, "unpack", asm.GetMetadata ("Filename"));
					var extracted = ExtractContentAssembly (asm.ItemSpec, perAssemblyOutputPath).ToArray ();

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

		IEnumerable<ITaskItem> ExtractContentAssembly (string assembly, string intermediatePath)
		{
			if (!File.Exists (assembly)) {
				Log.LogMessage (MessageImportance.Low, $"Not inspecting assembly because it doesn't exist: {assembly}");
				yield break;
			}

			var asmWriteTime = File.GetLastWriteTimeUtc (assembly);
			var manifestResources = GetAssemblyManifestResources (assembly).ToArray ();
			if (!manifestResources.Any ())
				yield break;

			Log.LogMessage (MessageImportance.Low, $"Inspecting assembly with {manifestResources.Length} resources: {assembly}");
			foreach (var embedded in manifestResources) {
				string rpath;

				if (embedded.Name.StartsWith ("__" + Prefix + "_content_", StringComparison.Ordinal)) {
					var mangled = embedded.Name.Substring (("__" + Prefix + "_content_").Length);
					rpath = UnmangleResource (mangled);
				} else if (embedded.Name.StartsWith ("__" + Prefix + "_page_", StringComparison.Ordinal)) {
					var mangled = embedded.Name.Substring (("__" + Prefix + "_page_").Length);
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

					using (var stream = File.Open (path, FileMode.Create)) {
						using (var resource = embedded.Open ())
							resource.CopyTo (stream);
					}

					unpackedResources.Add (item);
				}

				yield return item;
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
