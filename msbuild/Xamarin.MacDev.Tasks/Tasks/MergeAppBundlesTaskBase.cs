using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;

using Xamarin.Bundler;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	// This task will take two or more app bundles and merge them into a universal/fat app bundle.
	// It will go through every file from the input app bundles and copy them to the output app bundle.
	//
	// If a file exists in more than one input app bundle, then the behavior depends on the file type:
	//
	// 1) MachO files are lipo'ed into a fat MachO file.
	// 2) Managed assemblies (*.dll, *.exe) and their related files (satellite assemblies, app config, debug files, etc). are put into an
	//    RuntimeIdentifier-specific subdirectory. Our runtime knows how to locate assemblies in this RuntimeIdentifier-specific directory.
	// 3) Other files that behave like managed assemblies (i.e. should be put into the architecture-specific subdirectory)
	//    are put there. These files are listed in the 'ArchitectureSpecificFiles' parameter.
	// 4) Directories are copied as is, since they can't have different content.
	// 5) If symlinks point to different files, an error is raised.
	// 6) Any other files will cause errors to be raised.
	public partial class MergeAppBundles : XamarinTask {

		#region Inputs
		// This is a list of files (filename only, no path, will match any file with the given name in the app bundle)
		// that can be put in a RID-specific subdirectory.
		public ITaskItem [] ArchitectureSpecificFiles { get; set; }

		// This is a list of files (filename only, no path, will match any file with the given name in the app bundle)
		// to ignore/skip.
		public ITaskItem [] IgnoreFiles { get; set; }

		// A list of the .app bundles to merge
		[Required]
		public ITaskItem [] InputAppBundles { get; set; }

		// The output app bundle
		[Required]
		public string OutputAppBundle { get; set; }

		[Required]
		public string SdkDevPath { get; set; }

		#endregion

		enum FileType {
			MachO,
			PEAssembly,
			ArchitectureSpecific,
			Directory,
			Symlink,
			Other,
		}

		class Entries : List<Entry> {
			public string BundlePath;
			public string SpecificSubdirectory;
		}

		class Entry {
			public MergeAppBundles Task;
			public Entries AppBundle;
			public string RelativePath;
			public FileType Type;
			public List<Entry> DependentFiles;

			public string FullPath => Path.Combine (AppBundle.BundlePath, RelativePath);

			void FindDependentFiles (Func<Entry, bool> condition)
			{
				var dependentFiles = AppBundle.Where (v => v != this).Where (condition).ToArray ();

				if (dependentFiles.Length > 0) {
					if (DependentFiles is null)
						DependentFiles = new List<Entry> ();

					foreach (var dependentFile in dependentFiles) {
						AppBundle.Remove (dependentFile);
						DependentFiles.Add (dependentFile);
					}
				}
			}

			public void FindDependentFiles ()
			{
				// pdb
				FindDependentFiles (v => string.Equals (v.RelativePath, Path.ChangeExtension (RelativePath, "pdb"), StringComparison.OrdinalIgnoreCase));

				// config
				FindDependentFiles (v => string.Equals (v.RelativePath, RelativePath + ".config", StringComparison.OrdinalIgnoreCase));

				// satellite assemblies
				var satelliteName = Path.GetFileNameWithoutExtension (RelativePath) + ".resources.dll";
				FindDependentFiles (v => {
					if (v.Type != FileType.PEAssembly)
						return false;

					// if the name isn't the satellite name, it's not a dependent assembly of ours
					if (!string.Equals (Path.GetFileName (v.RelativePath), satelliteName, StringComparison.OrdinalIgnoreCase))
						return false;

					// if it's not in an immediate subdirectory, it's not a dependent assembly of ours
					if (!string.Equals (Path.GetDirectoryName (Path.GetDirectoryName (v.RelativePath)), Path.GetDirectoryName (RelativePath), StringComparison.OrdinalIgnoreCase))
						return false;

					// if the name of the immediate subdirectory isn't a valid culture, then it's not a dependent assembly of ours
					var immediateSubDir = Path.GetFileName (Path.GetDirectoryName (v.RelativePath));
					var cultureInfo = CultureInfo.GetCultureInfo (immediateSubDir);
					if (cultureInfo is null)
						return false;

					return true;
				});

				// also add the directories where the satellite assemblies are
				if (DependentFiles?.Any () == true) {
					FindDependentFiles (v => {
						if (v.Type != FileType.Directory && v.Type != FileType.Symlink)
							return false;

						return DependentFiles.Any (df => {
							if (df.Type != FileType.PEAssembly)
								return false;

							if (Path.GetDirectoryName (df.RelativePath) != v.RelativePath)
								return false;

							return true;
						});
					});
				}
			}

			// Compare two entries. The entry type must be identical, and the comparison is otherwise specific to each entry type.
			public bool IsIdenticalTo (Entry other)
			{
				if (other is null)
					throw new ArgumentNullException (nameof (other));

				// If they're of different types, they're really different.
				if (other.Type != Type)
					return false;

				// Directories can't be different
				if (Type == FileType.Directory)
					return true;

				// Symlinks are different if they point to different locations
				if (Type == FileType.Symlink) {
					var thisTarget = PathUtils.GetSymlinkTarget (FullPath);
					var otherTarget = PathUtils.GetSymlinkTarget (other.FullPath);
					return string.Equals (thisTarget, otherTarget, StringComparison.Ordinal);
				}

				// Finally compare the contents of the files to determine equality.
				if (!FileUtils.CompareFiles (FullPath, other.FullPath))
					return false;

				// If the entries have dependent files, we must consider them as well, so that
				// the main file and all the dependent files are considered a single entity for
				// the purpose of determining equality
				if (DependentFiles is not null && other.DependentFiles is not null) {
					// check if there are different number of dependent files, if so, we're different
					if (DependentFiles.Count != other.DependentFiles.Count)
						return false;

					// group by relative path
					var grouped = DependentFiles.Union (other.DependentFiles).GroupBy (v => v.RelativePath);
					foreach (var group in grouped) {
						// the files don't match up (same number of files, but not the same filenames)
						var files = group.ToArray ();
						if (files.Length != 2)
							return false;

						// compare the dependent files.
						if (!files [0].IsIdenticalTo (files [1]))
							return false;
					}
				}

				return true;
			}

			public void CopyTo (string outputDirectory, string subDirectory = null)
			{
				string outputFile;

				if (subDirectory is null) {
					outputFile = Path.Combine (outputDirectory, RelativePath);
				} else {
					var relativeAppDir = Path.GetDirectoryName (RelativePath);
					if (string.IsNullOrEmpty (relativeAppDir)) {
						outputFile = Path.Combine (outputDirectory, subDirectory, RelativePath);
					} else {
						outputFile = Path.Combine (outputDirectory, relativeAppDir, subDirectory, Path.GetFileName (RelativePath));
					}
				}

				if (Type == FileType.Directory) {
					Directory.CreateDirectory (outputFile);
				} else if (Type == FileType.Symlink) {
					Directory.CreateDirectory (Path.GetDirectoryName (outputFile));
					var symlinkTarget = PathUtils.GetSymlinkTarget (FullPath);
					if (File.Exists (outputFile) && PathUtils.IsSymlink (outputFile) && PathUtils.GetSymlinkTarget (outputFile) == symlinkTarget) {
						File.SetLastWriteTimeUtc (outputFile, DateTime.UtcNow); // update the timestamp, because the file the symlink points to might have changed.
						Task.Log.LogMessage (MessageImportance.Low, "Target '{0}' is up-to-date", outputFile);
					} else {
						PathUtils.FileDelete (outputFile);
						PathUtils.Symlink (symlinkTarget, outputFile);
					}
				} else {
					Directory.CreateDirectory (Path.GetDirectoryName (outputFile));
					if (!FileCopier.IsUptodate (FullPath, outputFile, Task.FileCopierReportErrorCallback, Task.FileCopierLogCallback))
						File.Copy (FullPath, outputFile, true);
				}

				if (DependentFiles is not null) {
					foreach (var file in DependentFiles)
						file.CopyTo (outputDirectory, subDirectory);
				}
			}
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			if (InputAppBundles.Length == 0) {
				Log.LogError (MSBStrings.E7073 /* At least one app bundle must be specified. */);
				return false;
			}

			// If we only have a single input directory, then we can just copy that as-is
			if (InputAppBundles.Length == 1) {
				var sourceDirectory = Path.GetFullPath (InputAppBundles [0].ItemSpec);
				var targetDirectory = Path.GetFullPath (OutputAppBundle);

				// Make sure we have a trailing directory, so that UpdateDirectory copies the directory contents of the source directory.
				if (sourceDirectory [sourceDirectory.Length - 1] != Path.DirectorySeparatorChar)
					sourceDirectory += Path.DirectorySeparatorChar;

				Log.LogMessage (MessageImportance.Low, $"Copying the single input directory {sourceDirectory} to {targetDirectory}");
				FileCopier.UpdateDirectory (sourceDirectory, targetDirectory, FileCopierReportErrorCallback, FileCopierLogCallback);
				return !Log.HasLoggedErrors;
			}

			if (!Merge ())
				return false;

			return !Log.HasLoggedErrors;
		}

		bool Merge ()
		{
			// Some validation
			foreach (var input in InputAppBundles) {
				if (!Directory.Exists (input.ItemSpec)) {
					Log.LogError (MSBStrings.E7074 /* "The app bundle {0} does not exist." */, input.ItemSpec);
					return false;
				}
				var specificSubdirectory = input.GetMetadata ("SpecificSubdirectory");
				if (string.IsNullOrEmpty (specificSubdirectory)) {
					Log.LogError (MSBStrings.E7075 /* No 'SpecificSubDirectory' metadata was provided for the app bundle {0}. */, input.ItemSpec);
					return false;
				}
			}

			// Gather all the files in each input app bundle
			var inputFiles = new Entries [InputAppBundles.Length];
			for (var i = 0; i < InputAppBundles.Length; i++) {
				var input = InputAppBundles [i];
				var specificSubdirectory = input.GetMetadata ("SpecificSubdirectory");
				var fullInput = Path.GetFullPath (input.ItemSpec);
				// strip the trailing path separator
				if (fullInput [fullInput.Length - 1] == Path.DirectorySeparatorChar)
					fullInput = fullInput.Substring (0, fullInput.Length - 1);
				// get all the files and subdirectories in the input app bundle
				var files = Directory.GetFileSystemEntries (fullInput, "*", SearchOption.AllDirectories);
				var entries = new Entries () {
					BundlePath = fullInput,
					SpecificSubdirectory = specificSubdirectory,
				};
				// Remove any files inside directories which are symlinks (we only need to process the symlink itself)
				var symlinkDirectories = files.Where (v => PathUtils.IsSymlink (v) && Directory.Exists (v));
				if (symlinkDirectories.Any ()) {
					files = files.Where (file => !symlinkDirectories.Any (dir => file.StartsWith (dir + Path.DirectorySeparatorChar))).ToArray ();
				}
				foreach (var file in files) {
					var relativePath = file.Substring (fullInput.Length + 1);
					var entry = new Entry {
						Task = this,
						RelativePath = relativePath,
						AppBundle = entries,
						Type = GetFileType (file),
					};
					entries.Add (entry);
				}
				inputFiles [i] = entries;
			}

			// Group dependent files for assemblies
			for (var i = 0; i < inputFiles.Length; i++) {
				var list = inputFiles [i];
				var assemblies = list.Where (v => v.Type == FileType.PEAssembly).ToArray ();
				foreach (var assembly in assemblies) {
					assembly.FindDependentFiles ();
				}
			}

			// List the input
			foreach (var list in inputFiles) {
				Log.LogMessage (MessageImportance.Low, $"Input files found in {list.BundlePath}:");
				foreach (var file in list) {
					Log.LogMessage (MessageImportance.Low, $"    {file.RelativePath} Type: {file.Type} Dependent files: {file.DependentFiles?.Count.ToString () ?? "0"}");
					if (file.DependentFiles?.Any () == true) {
						foreach (var df in file.DependentFiles) {
							Log.LogMessage (MessageImportance.Low, $"        {df.RelativePath} Type: {df.Type}");
						}
					}
				}
			}

			// Group the input by relative path in the output app bundle
			var map = new Dictionary<string, List<Entry>> ();
			foreach (var list in inputFiles) {
				foreach (var file in list) {
					if (!map.TryGetValue (file.RelativePath, out var groupedList)) {
						map [file.RelativePath] = groupedList = new List<Entry> ();
					}
					groupedList.Add (file);
				}
			}

			// Remove any ignored files
			if (IgnoreFiles is not null && IgnoreFiles.Length > 0) {
				foreach (var spec in IgnoreFiles) {
					var file = spec.ItemSpec;
					if (map.Remove (file)) {
						Log.LogMessage (MessageImportance.Low, "Ignored the file '{0}'", file);
					} else {
						Log.LogMessage (MessageImportance.Normal, "Asked to ignore the file '{0}', but no such file was found in any of the input app bundles.", file);
					}
				}
			}

			// Verify that the type of the input for each target file is the same
			foreach (var kvp in map) {
				var types = kvp.Value.Select (v => v.Type).Distinct ();
				if (types.Count () > 1) {
					// Files of different types.
					Log.LogError (MSBStrings.E7079 /* Invalid app bundle: the file {0} has different types between the input app bundles. */, kvp.Value.First ().RelativePath);
					ListFiles (kvp.Value);
					return false;
				}
			}

			// Merge stuff
			Directory.CreateDirectory (OutputAppBundle);
			foreach (var kvp in map) {
				var relativePath = kvp.Key;
				var entries = kvp.Value;
				var outputFile = Path.Combine (OutputAppBundle, relativePath);

				if (entries.Count == 1) {
					// just copy the file(s) if there's only one
					Log.LogMessage (MessageImportance.Low, $"The file '{entries [0].RelativePath}' only exists in '{entries [0].AppBundle.BundlePath}' and will be copied as-is to the merged app bundle.");
					entries [0].CopyTo (OutputAppBundle);
					continue;
				}

				// If they're all the same, just copy the first one
				var identical = true;
				for (var i = 1; i < entries.Count; i++) {
					if (!entries [0].IsIdenticalTo (entries [i])) {
						identical = false;
						break;
					}
				}
				if (identical) {
					// All the input files are identical. Just copy the first one into the bundle.
					Log.LogMessage (MessageImportance.Low, $"All the files for '{entries [0].RelativePath}' are identical between all the input app bundles.");
					entries [0].CopyTo (OutputAppBundle);
					continue;
				}

				// Custom merging is needed, depending on the type
				switch (entries [0].Type) {
				case FileType.MachO:
					MergeMachOFiles (outputFile, entries);
					break;
				case FileType.PEAssembly:
				case FileType.ArchitectureSpecific:
					MergeArchitectureSpecific (entries);
					break;
				case FileType.Symlink:
					Log.LogError (MSBStrings.E7076 /* Can't merge the symlink '{0}', it has different targets */, entries [0].RelativePath);
					ListFiles (entries);
					break;
				default:
					Log.LogError (MSBStrings.E7077 /* Unable to merge the file '{0}', it's different between the input app bundles. */, entries [0].RelativePath);
					ListFiles (entries);
					break;
				}
			}

			return !Log.HasLoggedErrors;
		}

		void ListFiles (List<Entry> entries)
		{
			for (var i = 0; i < entries.Count; i++) {
				Log.LogError (MSBStrings.E7080 /* App bundle file #{0}: {1} */, i + 1, entries [i].FullPath);
			}
		}

		void MergeArchitectureSpecific (IList<Entry> inputs)
		{
			foreach (var input in inputs) {
				Log.LogMessage (MessageImportance.Low, $"Copying '{input.RelativePath}' to the specific subdirectory {input.AppBundle.SpecificSubdirectory} for the merged app bundle.");
				input.CopyTo (OutputAppBundle, input.AppBundle.SpecificSubdirectory);
			}
		}

		void MergeMachOFiles (string output, IList<Entry> input)
		{
			if (input.Any (v => v.DependentFiles?.Any () == true)) {
				Log.LogError (MSBStrings.E7078 /* Invalid app bundle: the Mach-O file {0} has dependent files. */, input.First ().RelativePath);
				return;
			}

			var sourceFiles = input.Select (v => v.FullPath).ToArray ();

			if (FileCopier.IsUptodate (sourceFiles, new string [] { output }, FileCopierReportErrorCallback, FileCopierLogCallback))
				return;

			Log.LogMessage (MessageImportance.Low, $"Lipoing '{input [0].RelativePath}' for the merged app bundle from the following sources:\n\t{string.Join ("\n\t", input.Select (v => v.FullPath))}");

			var arguments = new List<string> ();
			arguments.Add ("-create");
			arguments.Add ("-output");
			arguments.Add (output);
			arguments.AddRange (sourceFiles);
			ExecuteAsync ("lipo", arguments, sdkDevPath: SdkDevPath).Wait ();
		}

		FileType GetFileType (string path)
		{
			if (PathUtils.IsSymlink (path))
				return FileType.Symlink;

			if (Directory.Exists (path))
				return FileType.Directory;

			if (path.EndsWith (".exe", StringComparison.Ordinal) || path.EndsWith (".dll", StringComparison.Ordinal))
				return FileType.PEAssembly;

			if (MachO.IsMachOFile (path))
				return FileType.MachO;

			if (StaticLibrary.IsStaticLibrary (path))
				return FileType.MachO;

			if (ArchitectureSpecificFiles is not null) {
				var filename = Path.GetFileName (path);
				if (ArchitectureSpecificFiles.Any (v => v.ItemSpec == filename))
					return FileType.ArchitectureSpecific;
			}

			return FileType.Other;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
