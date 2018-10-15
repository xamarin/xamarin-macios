using System;
using System.Text;
using System.Diagnostics;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class GetPropertyValueTaskBase : Task
	{
		public GetPropertyValueTaskBase ()
		{
			PropertyValue = string.Empty;
		}

		public string SessionId { get; set; }

		[Required]
		public ITaskItem FileName { get; set; }

		[Required]
		public string PropertyName { get; set; }
		
		public string TargetFrameworkIdentifier { get; set; }

		[Output]
		public string PropertyValue { get; set; }

		public override bool Execute ()
		{
			try {
				string result;
				if (!TryRunMSBuildGetPropertyValueTarget (FileName.GetMetadata ("FullPath"), PropertyName, TargetFrameworkIdentifier, out result)) {
					LogEndExecutionWithError (result);
				} else { 
					var value = ParsePropertyValueFromTargetResult (result);
					PropertyValue = value;
					LogEndExecution ();
				}
			} catch (Exception e) {
				LogError (e);
			}

			return true;
		}

		static bool TryRunMSBuildGetPropertyValueTarget (string path, string propertyName, string targetFrameworkIdentifier, out string result)
		{
			var arguments = string.Format ("\"{0}\" /t:targetGetPropertyValue_{1}", path, propertyName); 
			
			if (!string.IsNullOrEmpty (targetFrameworkIdentifier)) {
				arguments = string.Format ("{0} /p:TargetFrameworkIdentifier={1}", arguments, targetFrameworkIdentifier);
			}
			
			var psi = new ProcessStartInfo ("/Library/Frameworks/Mono.framework/Commands/msbuild", arguments);

			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;

			var stdOutput = new StringBuilder ();
			var stdError = new StringBuilder ();

			var process = new Process ();
			process.StartInfo = psi;
			
			process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) {
				stdOutput.AppendLine (e.Data);
			};

			process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e) {
				stdError.AppendLine (e.Data);
			};

			process.Start ();
			
			process.BeginOutputReadLine ();
			
			process.BeginErrorReadLine ();

			process.WaitForExit ();

			var success = process.ExitCode == 0;
			if (success) {
				result = stdOutput.ToString ();
			} else {
				result = stdError.ToString ();
			}

			return success;
		}

		static readonly string PropertyValuePatternStart = "{PropertyValue=";
		static readonly string PropertyValuePatternEnd = "}";

		static string ParsePropertyValueFromTargetResult (string result)
		{
			if (string.IsNullOrEmpty (result) || !result.Contains (PropertyValuePatternStart))
				return string.Empty;

			var indexStart = result.IndexOf (PropertyValuePatternStart, StringComparison.Ordinal);
			var indexEnd = result.IndexOf (PropertyValuePatternEnd, StringComparison.Ordinal);

			var start = indexStart + PropertyValuePatternStart.Length;
 			var length = indexEnd - (indexStart + PropertyValuePatternStart.Length); 
			var value = result.Substring (start, length);

			return value;
		}

		protected void LogEndExecutionWithError (string error)
		{
			Log.LogError (error);
		}

		protected void LogError (Exception e)
		{
			Log.LogErrorFromException (e);
		}

		protected void LogEndExecution ()
		{
			Log.LogTaskProperty ("PropertyValue", PropertyValue);
		}
	}
}
