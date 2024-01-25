using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Xamarin.Messaging.Build.Contracts;
using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build {
	public class CompareFilesMessageHandler : RequestHandler<CompareItemsMessage, CompareItemsResult> {
		protected override async Task<CompareItemsResult> ExecuteAsync (CompareItemsMessage message)
		{
			return await Task.Run (() => {
				var buildPath = Path.Combine (MessagingContext.GetBuildPath (), message.AppName, message.SessionId);
				var files = new List<string> ();

				using (var hashAlgorithm = Hash.GetAlgorithm ()) {
					foreach (var file in message.Items) {
						var targetPath = Path.Combine (buildPath, PlatformPath.GetPathForCurrentPlatform (file.ItemSpec));

						if (!File.Exists (targetPath)) {
							files.Add (file.ItemSpec);
						} else {

							using (var stream = File.OpenRead (targetPath)) {
								var localHash = hashAlgorithm.ComputeHashAsString (stream);

								if (file.Hash != localHash) {
									files.Add (file.ItemSpec);
								}
							}
						}
					}
				}

				return new CompareItemsResult { MissingFiles = files };
			}).ConfigureAwait (continueOnCapturedContext: false);
		}
	}
}
