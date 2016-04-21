using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CreatePkgInfoTaskBase : Task
	{
		static readonly byte[] PkgInfoData = { 0X41, 0X50, 0X50, 0X4C, 0x3f, 0x3f, 0x3f, 0x3f };
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string OutputPath { get; set; }

		#endregion

		public override bool Execute ()
		{
			Log.LogTaskName ("CreatePkgInfo");
			Log.LogTaskProperty ("OutputPath", OutputPath);

			if (!File.Exists (OutputPath)) {
				Directory.CreateDirectory (Path.GetDirectoryName (OutputPath));

				using (var stream = File.OpenWrite (OutputPath)) {
					stream.Write (PkgInfoData, 0, PkgInfoData.Length);
				}
			}

			return true;
		}
	}
}
