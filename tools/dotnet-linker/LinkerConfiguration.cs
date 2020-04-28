using System;
using System.IO;

using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.Tuner {

}

namespace Xamarin.Linker {
	public class LinkerConfiguration {
		public ApplePlatform Platform { get; private set; }
		public string PlatformAssembly { get; private set; }
		public LinkMode LinkMode { get; private set; }

		public bool InsertTimestamps { get; private set; } = true;

		public bool InsaneVerbosity { get; private set; } = true;

		static LinkerConfiguration linker_configuration;
		public static LinkerConfiguration Instance {
			get {
				if (linker_configuration == null)
					linker_configuration = new LinkerConfiguration ();
				return linker_configuration;
			}
		}

		LinkerConfiguration ()
		{
			var linker_file = Environment.GetEnvironmentVariable ("CUSTOM_LINKER_OPTIONS_FILE");
			if (string.IsNullOrEmpty (linker_file))
				throw new Exception ($"No custom linker options file is specified (using the CUSTOM_LINKER_OPTIONS_FILE environment variable).");
			if (!File.Exists (linker_file))
				throw new FileNotFoundException ($"The custom linker file {linker_file} (specified using the CUSTOM_LINKER_OPTIONS_FILE environment variable) does not exist.");

			// not the historical tooling (mtouch) default, but that's what the simulator templates offered
			LinkMode = LinkMode.None;
			var lines = File.ReadAllLines (linker_file);
			for (var i = 0; i < lines.Length; i++) {
				var line = lines [i].TrimStart ();
				if (line.Length == 0 || line [0] == '#')
					continue; // Allow comments
				switch (line.ToLowerInvariant ()) {
				case "platform=ios":
					Platform = ApplePlatform.iOS;
					break;
				case "platform=tvos":
					Platform = ApplePlatform.TVOS;
					break;
				case "Platform=watchos":
					Platform = ApplePlatform.WatchOS;
					break;
				case "platform=macos":
					Platform = ApplePlatform.MacOSX;
					break;
				case "mtouchlink=full":
					LinkMode = LinkMode.All;
					break;
				case "mtouchlink=sdkonly":
					LinkMode = LinkMode.SDKOnly;
					break;
				case "mtouchlink=platform":
					LinkMode = LinkMode.Platform;
					break;
				case "mtouchlink=none":
					LinkMode = LinkMode.None;
					break;
				case string _ when line.StartsWith ("PlatformAssembly=", StringComparison.Ordinal):
					PlatformAssembly = line ["PlatformAssembly=".Length..];
					break;
				default:
					var eq = line.IndexOf ('=');
					if (eq == -1)
						throw new InvalidOperationException ($"Invalid syntax for line {i + 1} in {linker_file}: No equals sign.");
					throw new InvalidOperationException ($"Unknown key '{line[..eq]}' in {linker_file}");
				}
			}
		}

		public void Write ()
		{
			Console.WriteLine ($"LinkerConfiguration:");
			Console.WriteLine ($"    Platform: {Platform}");
			Console.WriteLine ($"    LinkMode: {LinkMode}");
			Console.WriteLine ($"    PlatformAssembly: {PlatformAssembly}");
		}
	}
}
