using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

//TODO: Move the logic to MacDev so it can be shared and reused by the IDB Message Handler as well
namespace Xamarin.Messaging.Build {
	public class GetUniversalTargetIdentifierMessageHandler : RequestHandler<GetUniversalTypeIdentifierMessage, GetUniversalTypeIdentifierResult> {
		protected override async Task<GetUniversalTypeIdentifierResult> ExecuteAsync (GetUniversalTypeIdentifierMessage message)
		{
			return await Task.Run (() => {
				var response = new GetUniversalTypeIdentifierResult ();
				var tempFilePath = Path.Combine (Path.GetTempPath (), message.FileName);

				File.WriteAllBytes (tempFilePath, message.Payload);

				response.UniversalTypeIdentifier = GetUniversalTypeIdentifier (tempFilePath);

				return response;
			}).ConfigureAwait (continueOnCapturedContext: false);
		}

		string GetUniversalTypeIdentifier (string filePath)
		{
			var expression = new Regex ("kMDItemContentType = \"(?<UTI>[^,]+)\"");
			var stdout = new StringBuilder ();

			if (File.Exists (filePath)) {
				var startInfo = new ProcessStartInfo ();

				startInfo.WindowStyle = ProcessWindowStyle.Hidden;
				startInfo.FileName = "/usr/bin/mdls";
				startInfo.Arguments = string.Format ("\"{0}\" -name kMDItemContentType", filePath);
				startInfo.RedirectStandardOutput = true;
				startInfo.UseShellExecute = false;

				var proc = new Process ();

				proc.StartInfo = startInfo;
				proc.OutputDataReceived += (sender, e) => {
					stdout.AppendLine (e.Data);
				};

				proc.Start ();
				proc.BeginOutputReadLine ();
				proc.WaitForExit ();
			}

			var match = expression.Match (stdout.ToString ());

			if (match.Success) {
				if (match.Groups.Count > 1)
					return match.Groups [1].Value;
			}

			return string.Empty;
		}
	}
}
