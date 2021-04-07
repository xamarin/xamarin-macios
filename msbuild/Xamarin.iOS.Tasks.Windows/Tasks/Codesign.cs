using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xamarin.iOS.Tasks.Windows.Properties;

namespace Xamarin.iOS.HotRestart.Tasks
{
	public class Codesign : Task, ICancelableTask
	{
		CancellationTokenSource cancellationSource;

		#region Inputs

		[Required]
		public string AppBundlePath { get; set; }

		[Required]
		public string BundleIdentifier { get; set; }

		[Required]
		public string CodeSigningPath { get; set; }

		[Required]
		public string ProvisioningProfilePath { get; set; }

		#endregion

		public override bool Execute()
		{
			try
			{
				var plistArgs = new Dictionary<string, string>
				{
					{ "CFBundleIdentifier", BundleIdentifier }
				};

				var signingArgs = new List<string>
				{
					$"-i=\"{AppBundlePath}\"",
					$"-p=\"{ProvisioningProfilePath}\"",
					$"-c=\"{CodeSigningPath}\"",
					$"-pk=\"{string.Join(",", plistArgs.Keys)}\"",
					$"-pv=\"{string.Join(",", plistArgs.Values)}\"",
				};

				// TODO: IDB ref
				var password = string.Empty;//new LocalCertificatesProvider(string.Empty).GetCertificatePassword(CodeSigningPath);

				if (string.IsNullOrEmpty(password))
				{
					throw new Exception(Resources.LocalCodesign_MissingPasswordFile);
				}

				// TODO: do we still need this message?
				//Log.LogMessage(MessageImportance.Low, Strings.LocalCodesign.ToolArguments($"{winiOSTerminalPath} {string.Join(" ", signingArgs)}"));

				signingArgs.Add($"-w=\"{password}\"");

				var process = new Process();

				cancellationSource = new CancellationTokenSource();
				//process.RunHotRestartAsync("sign", signingArgs, localContext, cancellationToken: cancellationSource.Token).Do(output =>
				//{
				//	Log.LogMessage(MessageImportance.Low, output);
				//}, error =>
				//{
				//	throw error;
				//}).Wait();
			}
			//catch (WindowsiOSException ex)
			//{
			//	Log.LogError(null, ex.ErrorCode, null, null, 0, 0, 0, 0, ex.FullMessage());
			//}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
			}

			return !Log.HasLoggedErrors;
		}

		public void Cancel() => cancellationSource?.Cancel();
	}
}
