using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CreatePkgInfoTaskBase : XamarinTask {
		static readonly byte [] PkgInfoData = { 0X41, 0X50, 0X50, 0X4C, 0x3f, 0x3f, 0x3f, 0x3f };
		#region Inputs

		[Required]
		public string OutputPath { get; set; }

		#endregion

		public override bool Execute ()
		{
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
