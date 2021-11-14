using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;

namespace Xamarin.iOS.HotRestart {
	public class Application {
		static readonly object lockObject = new object ();
		static readonly string launchFlag = "Launch";

		static Action killApplicationAction;

		public static void Run (string [] args, Action<string> loadFrameworkAction, Action killApplicationAction)
		{
			Application.killApplicationAction = killApplicationAction;

			var documentsFolder = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);

			CreateLaunchFlag (documentsFolder);
			MonitorIncrementalDeployments (documentsFolder);
			var contentFolder = CopyBundleContentToAppSandBox (documentsFolder);

			Console.WriteLine ($"Content folder: {contentFolder}");

			var appFile = Directory.EnumerateFiles (contentFolder, "*.hotrestartapp").FirstOrDefault ();

			if (string.IsNullOrEmpty (appFile)) {
				throw new Exception ("Could not find a .hotrestartapp file representing your main app assembly.");
			}

			var xamarinAppPath = Path.Combine (contentFolder, $"{Path.GetFileNameWithoutExtension (appFile)}.dll");

			if (!File.Exists (xamarinAppPath)) {
				xamarinAppPath = Path.Combine (contentFolder, $"{Path.GetFileNameWithoutExtension (appFile)}.exe");
			}

			if (string.IsNullOrEmpty (xamarinAppPath)) {
				// TODO: show this error on the phone
				throw new Exception ("Could not find any executable in the bundle");
			}

			Console.WriteLine ($"App path {xamarinAppPath}");

			AppDomain.CurrentDomain.AssemblyResolve += (s, a) => {
				try {
					var requestedAssembly = new AssemblyName (a.Name);

					Console.WriteLine ($"Failed to load assembly {a.Name}. Trying from {contentFolder}...");

					return Assembly.LoadFrom (Path.Combine (contentFolder, $"{requestedAssembly.Name}.dll"));
				} catch (Exception ex) {
					Console.WriteLine ($"Could not resolve assembly {a.Name}. Details: {ex.Message}");

					return null;
				}
			};

			var formsApp = Assembly.LoadFile (xamarinAppPath);
			var iosAppDelegate = formsApp
				.GetTypes ()
				.FirstOrDefault (type => type.HasBaseType ("Microsoft.Maui.MauiUIApplicationDelegate")
					 || type.HasBaseType ("Xamarin.Forms.Platform.iOS.FormsApplicationDelegate"));

			if (iosAppDelegate == null) {
				// TODO: show this error on the phone
				throw new Exception ($"No class inheriting from a valid Application Delegate found in {xamarinAppPath}");
			}

			Console.WriteLine ($"AppDelegate name: {iosAppDelegate.Name}");

			try {
				LoadFrameworks (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Frameworks"), loadFrameworkAction);

				ForceLoadingAssemblies (iosAppDelegate, contentFolder);

				formsApp.EntryPoint.Invoke (null, new object [] { args });
			} catch (Exception ex) {
				var innerEx = GetInnerException (ex);

				Debug.WriteLine ($"An error occurred: '{innerEx.Message}'. Callstack: '{innerEx.StackTrace}'");
			}
		}

		static void LoadFrameworks (string frameworksDir, Action<string> loadFrameworkAction)
		{
			if (!Directory.Exists (frameworksDir))
				return;

			// there should not be sub-frameworks but...
			foreach (var frameworkDir in Directory.EnumerateDirectories (frameworksDir, "*.framework", SearchOption.AllDirectories)) {
				// the binary is a dynamic library with the same name as the framework itself
				loadFrameworkAction (Path.Combine (frameworkDir, Path.GetFileNameWithoutExtension (frameworkDir)));
			}
		}

		static string CopyBundleContentToAppSandBox (string rootFolder)
		{
			var bundledContentPath = Directory.EnumerateDirectories (AppDomain.CurrentDomain.BaseDirectory, "*.content").FirstOrDefault ();
			if (string.IsNullOrEmpty (bundledContentPath)) {
				throw new Exception ($"There's no content folder in {AppDomain.CurrentDomain.BaseDirectory}");
			}

			var installFlag = Path.Combine (rootFolder, "Installed");
			if (!File.Exists (installFlag)) {
				return bundledContentPath;
			}

			var contentFolder = Directory.EnumerateDirectories (rootFolder, "*.content").FirstOrDefault ();
			if (!string.IsNullOrEmpty (contentFolder)) {
				Directory.Delete (contentFolder, recursive: true);
			} else {
				contentFolder = Path.Combine (rootFolder, Path.GetFileName (bundledContentPath));
			}

			DirectoryCopy (bundledContentPath, contentFolder);
			File.Delete (installFlag);

			return bundledContentPath;
		}

		static void DirectoryCopy (string sourceDirName, string destDirName)
		{
			// Get the subdirectories for the specified directory.
			var dir = new DirectoryInfo (sourceDirName);
			var dirs = dir.GetDirectories ();

			if (!Directory.Exists (destDirName))
				Directory.CreateDirectory (destDirName);

			var files = dir.GetFiles ();

			foreach (var file in files) {
				var tempPath = Path.Combine (destDirName, file.Name);

				file.CopyTo (tempPath, false);
			}

			foreach (var subDir in dirs) {
				var tempPath = Path.Combine (destDirName, subDir.Name);

				DirectoryCopy (subDir.FullName, tempPath);
			}
		}

		static void ForceLoadingAssemblies (Type formsDelegate, string contentFolder)
		{
			// Create instance to force loading assemblies
			var i = Activator.CreateInstance (formsDelegate);

			// Try loading assemblies, this is needed to make references like PancakeView work
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies ().Select (x => x.Location);

			foreach (var assemblyPath in Directory.EnumerateFiles (contentFolder, "*.dll")
				.Where (a => !loadedAssemblies.Contains (a))) {
				var assembly = Assembly.LoadFrom (assemblyPath);

				foreach (var type in assembly.GetTypes ().Where (t => !t.IsAbstract
					  && !t.ContainsGenericParameters
					  && t.GetConstructor (Type.EmptyTypes) != null
					  && !t.CustomAttributes.Any (c => c.AttributeType.Name == "XamlFilePathAttribute"))) {
					// We should avoid Static Readonly fields, those might depend on Forms inicialization (i.e. PanCardViews.CardsView)
					if (type != null && !type.ContainStaticFields ()) {
						try {
							var x = Activator.CreateInstance (type);
							break;
						} catch (Exception ex) {
							var innerEx = GetInnerException (ex);

							Debug.WriteLine ($"Failed to force load assembly {assemblyPath}. Type:{type.FullName}. Exception: '{innerEx.Message}'. Callstack: '{innerEx.StackTrace}'");
						}
					}
				}
			}
		}

		static Exception GetInnerException (Exception ex)
		{
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}

			return ex;
		}

		static string GetFlagsFileName (string rootFolder) => Path.Combine (rootFolder, "Flags");

		static void CreateLaunchFlag (string rootFolder)
		{
			var flagsFileName = GetFlagsFileName (rootFolder);

			if (File.Exists (flagsFileName)) {
				File.WriteAllText (flagsFileName, launchFlag, Encoding.UTF8);
			}
		}

		static void MonitorIncrementalDeployments (string rootFolder)
		{
			var timer = new System.Timers.Timer {
				Interval = 200,
				AutoReset = true
			};

			timer.Elapsed += (sender, e) => {
				CheckApplicationFlags (rootFolder);
			};
			timer.Start ();
		}

		static void CheckApplicationFlags (string rootFolder)
		{
			lock (lockObject) {
				var flagsFileName = GetFlagsFileName (rootFolder);

				//Means that the app is already running but someone did an action over the app after launching, so this app could be out of date and must be exited
				if (File.Exists (flagsFileName) && File.ReadAllText (flagsFileName, Encoding.UTF8).Trim () != launchFlag) {
					killApplicationAction ();
				}
			}
		}
	}
}