using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Xamarin;

namespace xharness
{
	public class TVOSTarget : iOSTarget
	{
		public override string Suffix {
			get {
				return "-tvos";
			}
		}

		protected override string BindingsProjectTypeGuids {
			get {
				return "{4A1ED743-3331-459B-915A-4B17C7B6DBB6}";
			}
		}

		protected override string ProjectTypeGuids {
			get {
				return "{06FA79CB-D6CD-4721-BB4B-1BD202089C55}";
			}
		}

		protected override string TargetFrameworkIdentifier {
			get {
				return "Xamarin.TVOS";
			}
		}

		protected override string Imports {
			get {
				return IsFSharp ? "TVOS\\Xamarin.TVOS.FSharp.targets" : "TVOS\\Xamarin.TVOS.CSharp.targets";
			}
		}

		protected override string BindingsImports {
			get {
				return IsFSharp ? "TVOS\\Xamarin.TVOS.ObjCBinding.FSharp.targets" : "TVOS\\Xamarin.TVOS.ObjCBinding.CSharp.targets";
			}
		}

		public override string SimulatorArchitectures {
			get {
				return "x86_64";
			}
		}

		public override string DeviceArchitectures {
			get {
				return "ARM64";
			}
		}

		protected override string GetMinimumOSVersion(string templateMinimumOSVersion)
		{
			return "9.0";
		}

		protected override int[] UIDeviceFamily {
			get {
				return new int [] { 3 };
			}
		}

		protected override string AdditionalDefines {
			get {
				return "XAMCORE_2_0;XAMCORE_3_0;";
			}
		}

		public override string Platform {
			get {
				return "tvos";
			}
		}

		protected override bool SupportsBitcode {
			get {
				return true;
			}
		}

		static Dictionary<string, string> project_guids = new Dictionary<string, string> ();

		protected override void ProcessProject ()
		{
			base.ProcessProject ();

			var srcDirectory = Path.Combine (Harness.RootDirectory, "..", "src");

			string project_guid;
			var mt_nunitlite_project_path = Path.GetFullPath (Path.Combine (srcDirectory, "MonoTouch.NUnitLite.tvos.csproj"));
			if (!project_guids.TryGetValue (mt_nunitlite_project_path, out project_guid)) {
				XmlDocument mt_nunitlite_project = new XmlDocument ();
				mt_nunitlite_project.LoadWithoutNetworkAccess (mt_nunitlite_project_path);
				project_guid = mt_nunitlite_project.GetProjectGuid ();
				project_guids [mt_nunitlite_project_path] = project_guid;
			}
			inputProject.CreateProjectReferenceValue ("MonoTouch.NUnitLite", mt_nunitlite_project_path, project_guid, "MonoTouch.NUnitLite");

			inputProject.AddExtraMtouchArgs ("--bitcode:asmonly", "iPhone", "Release");
			inputProject.SetMtouchUseLlvm (true, "iPhone", "Release");
		}
	}
}

