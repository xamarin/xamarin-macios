using System;

using Mono.Linker.Steps;

namespace Xamarin {

	public class SetupStep : BaseStep {

		protected override void Process ()
		{
			Console.WriteLine ("Hello SetupStep");
		}
	}
}
