using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Xamarin.Bundler {
	public partial class Assembly
	{
		public List<string> Satellites;

		// returns false if the assembly was not copied (because it was already up-to-date).
		bool CopyAssembly (string source, string target)
		{
			// TODO - We should move to mtouch's code, shared in common
			var copied = false;

			try {
				if (!Application.IsUptodate (source, target)) {
					copied = true;
					Application.CopyFile (source, target);
				}
			} catch (Exception e) {
				throw new MonoMacException (1009, true, e, "Could not copy the assembly '{0}' to '{1}': {2}", source, target, e.Message);
			}

			return copied;
		}

		public void ComputeSatellites ()
		{
			var path = Path.GetDirectoryName (FullPath);
			var satellite_name = Path.GetFileNameWithoutExtension (FullPath) + ".resources.dll";

			foreach (var subdir in Directory.GetDirectories (path)) {
				var culture_name = Path.GetFileName (subdir);
				CultureInfo ci;

				if (culture_name.IndexOf ('.') >= 0)
					continue; // cultures can't have dots. This way we don't check every *.app directory

				try {
					ci = CultureInfo.GetCultureInfo (culture_name);
				} catch {
					// nope, not a resource language
					continue;
				}

				if (ci == null)
					continue;

				var satellite = Path.Combine (subdir, satellite_name);
				if (File.Exists (satellite)) {
					if (Satellites == null)
						Satellites = new List<string> ();
					Satellites.Add (satellite);
				}
			}
		}

		public void CopySatellitesToDirectory (string directory)
		{
			if (Satellites == null)
				return;

			foreach (var a in Satellites) {
				string target_dir = Path.Combine (directory, Path.GetFileName (Path.GetDirectoryName (a)));
				string target_s = Path.Combine (target_dir, Path.GetFileName (a));

				if (!Directory.Exists (target_dir))
					Directory.CreateDirectory (target_dir);

				CopyAssembly (a, target_s);
			}
		}
	}
}
