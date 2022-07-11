using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Xamarin.iOS.Tasks
{
	class XIProject : IDisposable {
		public string OriginalProjectDirectory { get; private set; }
		public IEnumerable<string> DependentSiblings { get; private set; }
		public string ProjectDirectory { get; private set; }

		static string ResolveSymlinks (string path)
		{
			using (var p = new Process ()) {
				p.StartInfo.FileName = "pwd";
				p.StartInfo.Arguments = "-P";
				p.StartInfo.WorkingDirectory = path;
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.Start ();
				return p.StandardOutput.ReadToEnd ().Trim ('\n', '\r', ' ', '\t');
			}
		}

		public static XIProject Clone (string projectDirectory, params string[] dependentSiblings)
		{
			var tmpdir = Path.GetTempFileName ();
			File.Delete (tmpdir);
			Directory.CreateDirectory (tmpdir);
			tmpdir = ResolveSymlinks (tmpdir); // We need to resolve any symlinks, otherwise MSBuild fails in strange ways.

			// Calculate the directories to copy
			var dirs = new List<string> (1 + dependentSiblings.Length);
			dirs.Add (projectDirectory);
			foreach (var ds in dependentSiblings)
				dirs.Add (Path.Combine (Path.GetDirectoryName (projectDirectory), ds));

			foreach (var dir in dirs) {
				// execute 'git clean -fxd' first so that we don't copy more than needed.
				using (var p = new Process ()) {
					p.StartInfo.FileName = "git";
					p.StartInfo.Arguments = "clean -xfd";
					p.StartInfo.WorkingDirectory = dir;
					p.StartInfo.UseShellExecute = false;
					p.Start ();
					Console.WriteLine ("cd {2} && {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments, p.StartInfo.WorkingDirectory);
					p.WaitForExit ();
				}

				using (var p = new Process ()) {
					p.StartInfo.FileName = "cp";
					p.StartInfo.Arguments = "-av \"" + dir + "\" \"" + tmpdir + "\"";
					p.StartInfo.WorkingDirectory = dir;
					p.StartInfo.UseShellExecute = false;
					Console.WriteLine ("cd {2} && {0} {1}", p.StartInfo.FileName, p.StartInfo.Arguments, p.StartInfo.WorkingDirectory);
					p.Start ();
					p.WaitForExit ();
				}
			}

			return new XIProject {
				OriginalProjectDirectory = projectDirectory,
				ProjectDirectory = tmpdir,
				DependentSiblings = dependentSiblings,
			};
		}

		public void Dispose ()
		{
			try {
				Directory.Delete (ProjectDirectory, true);
			} catch (Exception e) {
				Console.WriteLine ("Failed to delete directory {0}: {1}", ProjectDirectory, e);
			}
		}
	}
}
