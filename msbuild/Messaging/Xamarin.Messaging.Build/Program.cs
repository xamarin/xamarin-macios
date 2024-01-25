using System.Threading.Tasks;

using Xamarin.Messaging.Client;

namespace Xamarin.Messaging.Build {
	class Program {
		static async Task Main (string [] args)
		{
			var topicGenerator = new TopicGenerator ();
			var arguments = new AgentArgumentsParser ().ParseArguments (args);
			var agent = new BuildAgent (topicGenerator, arguments.Version, arguments.VersionInfo);
			var runner = new AgentConsoleRunner<BuildAgent> (agent, arguments);

			await runner.RunAsync ().ConfigureAwait (continueOnCapturedContext: false);
		}
	}
}
