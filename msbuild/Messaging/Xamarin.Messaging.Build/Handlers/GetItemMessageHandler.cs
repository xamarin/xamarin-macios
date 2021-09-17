using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build {
	public class GetItemMessageHandler : RequestHandler<GetItemMessage, GetItemResult> {
		static readonly ITracer tracer = Tracer.Get<CopyItemMessageHandler> ();

		protected override async Task<GetItemResult> ExecuteAsync (GetItemMessage message)
		{
			return await Task.Run<GetItemResult> (() => {
				var targetPath = Path.GetFullPath (Path.Combine (
					MessagingContext.GetBuildPath (),
					message.AppName,
					message.SessionId,
					PlatformPath.GetPathForCurrentPlatform (message.ItemSpec)));

				var result = new GetItemResult ();

				if (File.Exists (targetPath)) {
					result.FileExists = true;

					// Avoiding sending the payload if the file is too large based on the defined limit by the the client
					if (new System.IO.FileInfo (targetPath).Length > message.LimitLength)
						result.FileLengthExceedsLimit = true;
					else
						result.Content = File.ReadAllBytes (targetPath);
				}

				return result;
			}).ConfigureAwait (continueOnCapturedContext: false);
		}
	}
}
