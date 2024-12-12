using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build {
	public class CopyItemMessageHandler : RequestHandler<CopyItemMessage> {
		static readonly ITracer tracer = Tracer.Get<CopyItemMessageHandler> ();

		protected override async Task ExecuteAsync (CopyItemMessage message)
		{
			await Task.Run (async () => {
				var targetPath = Path.GetFullPath (Path.Combine (
					MessagingContext.GetBuildPath (),
					message.AppName,
					message.SessionId,
					PlatformPath.GetPathForCurrentPlatform (message.ItemSpec)));

				if (File.Exists (targetPath)) {
					File.Delete (targetPath);
				} else {
					var directoryName = Path.GetDirectoryName (targetPath);
					if (directoryName is not null && !Directory.Exists (directoryName))
						Directory.CreateDirectory (directoryName);
				}

				using (var file = File.Create (targetPath))
				using (var stream = new MemoryStream (message.Content))
					await stream.CopyToAsync (file).ConfigureAwait (continueOnCapturedContext: false);
			});
		}
	}
}
