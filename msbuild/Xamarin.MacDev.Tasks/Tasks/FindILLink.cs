using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks {
	public class FindILLink : XamarinBuildTask, ITaskCallback, ICancelableTask {
		[Output]
		public string ILLinkPath { get; set; }

		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId))
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var targetName = "ComputeILLinkTaskPath";
			var target = $@"<Target Name=""{targetName}"">
	<WriteLinesToFile File=""$(OutputFilePath)"" Lines=""$(ILLinkTasksAssembly)"" />
</Target>";

			var illinkTaskPath = ComputeValueUsingTarget (target, targetName);

			// Don't do anything else if something already went wrong (in particular don't check if illink.dll exists).
			if (Log.HasLoggedErrors)
				return false;

			if (!string.IsNullOrEmpty (illinkTaskPath))
				ILLinkPath = Path.Combine (Path.GetDirectoryName (illinkTaskPath), "illink.dll");

			if (!File.Exists (ILLinkPath))
				Log.LogError (MSBStrings.E7115 /*"The illink assembly doesn't exist: '{0}'" */, ILLinkPath);

			return !Log.HasLoggedErrors;
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		public bool ShouldCopyToBuildServer (ITaskItem item) => false;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}

