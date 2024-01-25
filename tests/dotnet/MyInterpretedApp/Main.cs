using System;
using System.Runtime.CompilerServices;

using Foundation;

using UIKit;

namespace MyInterpretedApp {
	public class Application {
		static void Main (string [] args)
		{
			Console.WriteLine ($"Execution mode: {GetExecutionMode ()}");
			UIApplication.Main (args, null, typeof (AppDelegate));
		}

		public static string GetExecutionMode ()
		{
			if (!RuntimeFeature.IsDynamicCodeSupported)
				return "AOT";
			if (RuntimeFeature.IsDynamicCodeCompiled)
				return "JIT";
			return "Interpreter";
		}
	}
}
