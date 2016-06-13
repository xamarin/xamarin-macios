using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks
{
	public abstract class DeleteBase : Task
	{
		readonly Delete delete = new Delete();

		public string SessionId { get; set; }

		[Required]
		public ITaskItem[] Files
		{
			get { return delete.Files; }
			set { delete.Files = value; }
		}

		public bool TreatErrorsAsWarnings
		{
			get { return delete.TreatErrorsAsWarnings; }
			set { delete.TreatErrorsAsWarnings = value; }
		}

		[Output]
		public ITaskItem[] DeletedFiles
		{
			get { return delete.DeletedFiles; }
			set { delete.DeletedFiles = value; }
		}

		public override bool Execute ()
		{
			delete.BuildEngine = this.BuildEngine;
			return delete.Execute ();
		}
	}
}