using System;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public abstract class SetEnvironmentVariableTaskBase : Task {
		#region Inputs

		[Required]
		public string Name { get; set; }

		public string Value { get; set; }

		#endregion

		public override bool Execute ()
		{
			Environment.SetEnvironmentVariable (Name, Value);
			return true;
		}
	}
}
