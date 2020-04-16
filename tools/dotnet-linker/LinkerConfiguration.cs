using System;
using System.IO;

using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.Tuner {

}

namespace Xamarin.Linker {
	public class LinkerConfiguration {
		public ApplePlatform Platform;
		public string PlatformAssembly;
		public LinkMode LinkMode;

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
			var linker_file = Environment.GetEnvironmentVariable ("CUSTOM_LINKER_OPTIONS_FILE");
			if (string.IsNullOrEmpty (linker_file))
				throw new Exception ($"No custom linker options file is specified (using the CUSTOM_LINKER_OPTIONS_FILE environment variable).");
			if (!File.Exists (linker_file))
				throw new FileNotFoundException ($"The custom linker file {linker_file} (specifed using the CUSTOM_LINKER_OPTIONS_FILE environment variable) does not exist.");
			var lines = File.ReadAllLines (linker_file);
			for (var i = 0; i < lines.Length; i++) {
				var line = lines [i].TrimStart ();
				if (line.Length == 0 || line [0] == '#')
					continue; // Allow comments
				var eq = line.IndexOf ('=');
				if (eq == -1)
					throw new InvalidOperationException ($"Invalid syntax for line {i + 1} in {linker_file}: No equals sign.");
				var key = line.Substring (0, eq);
				var value = eq + 1 == line.Length ? string.Empty : line.Substring (eq + 1);
				switch (key) {
				case "Platform":
					switch (value) {
					case "iOS":
						Platform = ApplePlatform.iOS;
						break;
					case "tvOS":
						Platform = ApplePlatform.TVOS;
						break;
					case "watchOS":
						Platform = ApplePlatform.WatchOS;
						break;
					case "macOS":
						Platform = ApplePlatform.MacOSX;
						break;
					default:
						throw new InvalidOperationException ($"Invalid platform '{value}' in {linker_file}.");
					}
					break;
				case "MtouchLink":
					switch (value.ToLowerInvariant ()) {
					case "":
					case "full":
						LinkMode = LinkMode.All;
						break;
					case "sdkonly":
						LinkMode = LinkMode.SDKOnly;
						break;
					case "platform":
						LinkMode = LinkMode.Platform;
						break;
					case "none":
						LinkMode = LinkMode.None;
						break;
					}
					break;
				case "PlatformAssembly":
					PlatformAssembly = value;
					break;
				default:
					throw new InvalidOperationException ($"Unknown key {key} in {linker_file}");
				}
			}

			Console.WriteLine ($"LinkerConfiguration:");
			Console.WriteLine ($"    Platform: {Platform}");
			Console.WriteLine ($"    LinkMode: {LinkMode}");
			Console.WriteLine ($"    PlatformAssembly: {PlatformAssembly}");
		}

		public void DoSomething ()
		{
		}
	}
}
