using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Bundler;
using Xamarin.Localization.MSBuild;
using Xamarin.MacDev;
using Xamarin.Messaging.Build.Client;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	/// <summary>
	/// This task will extract the specified zip file into the specified extraction path.
	/// This task works on Windows too, but if the task encounters a symlink while extracting, an error will be shown.
	/// </summary>
	public class Unzip : XamarinTask, ITaskCallback {
		[Required]
		public ITaskItem? ZipFilePath { get; set; }

		[Required]
		public string ExtractionPath { get; set; } = string.Empty;

		// The file or directory to extract from the zip file.
		// If not specified, the entire zip file is extracted.
		public string Resource { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] TouchedFiles { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			return ExecuteLocally ();
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		bool ExecuteLocally ()
		{
			var createdFiles = new List<string> ();
			if (!CompressionHelper.TryDecompress (Log, ZipFilePath!.ItemSpec, Resource, ExtractionPath, createdFiles, out var _))
				return false;

			TouchedFiles = createdFiles.Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
