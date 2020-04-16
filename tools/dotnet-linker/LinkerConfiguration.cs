using System;

namespace Xamarin.Linker {
	public class LinkerConfiguration {
		static LinkerConfiguration linker_configuration;
		public static LinkerConfiguration Instance {
			get {
				if (linker_configuration == null)
					linker_configuration = new LinkerConfiguration ();
				return linker_configuration;
			}
		}

		public LinkerConfiguration ()
		{
			Console.WriteLine ("LinkerConfiguration");
			Console.WriteLine ("    CUSTOM_LINKER_OPTIONS_FILE: {0}", Environment.GetEnvironmentVariable ("CUSTOM_LINKER_OPTIONS_FILE"));
		}

		public void DoSomething ()
		{
			Console.WriteLine ("DoSomething");
		}
	}
}
