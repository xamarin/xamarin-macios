using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

using Xamarin.Utils;

namespace Xamarin.Tests
{
	public enum Profile {
		None,
		iOS, 
		tvOS, 
		watchOS,
		macClassic,
		macModern,
		macFull,
		macSystem,
	}
	
	class BGenTool : Tool
	{
		public Profile Profile;

		public List<string> ApiDefinitions = new List<string> ();
		public string TmpDirectory;

		protected override string ToolPath { get { return Configuration.BGenPath; } }
		protected override string MessagePrefix { get { return "BI"; } }
		protected override string MessageToolName { get { return "bgen"; } }

		string BuildArguments ()
		{
			var sb = new StringBuilder ();
			var targetFramework = (string) null;

			switch (Profile) {
			case Profile.None:
				break;
			case Profile.iOS:
				targetFramework = "Xamarin.iOS,v1.0";
				break;
			case Profile.tvOS:
				targetFramework = "Xamarin.TVOS,v1.0";
				break;
			case Profile.watchOS:
				targetFramework = "Xamarin.WatchOS,v1.0";
				break;
			case Profile.macClassic:
				targetFramework = "XamMac,v1.0";
				break;
			case Profile.macFull:
				targetFramework = "Xamarin.Mac,Version=v4.5,Profile=Full";
				break;
			case Profile.macModern:
				targetFramework = "Xamarin.Mac,Version=v2.0,Profile=Mobile";
				break;
			case Profile.macSystem:
				targetFramework = "Xamarin.Mac,Version=v4.5,Profile=System";
				break;
			default:
				throw new NotImplementedException ($"Profile: {Profile}");
			}

			if (!string.IsNullOrEmpty (targetFramework))
				sb.Append (" --target-framework=").Append (targetFramework);
			
			foreach (var ad in ApiDefinitions)
				sb.Append (" --api=").Append (StringUtils.Quote (ad));
			
			if (!string.IsNullOrEmpty (TmpDirectory))
				sb.Append (" --tmpdir=").Append (StringUtils.Quote (TmpDirectory));

			return sb.ToString ();
		}

		public void AssertExecute (string message)
		{
			Assert.AreEqual (0, Execute (BuildArguments ()), message);
		}

		public void AssertExecuteError (string message)
		{
			Assert.AreNotEqual (0, Execute (BuildArguments ()), message);
		}

		void EnsureTempDir ()
		{
			if (TmpDirectory == null)
				TmpDirectory = Cache.CreateTemporaryDirectory ();
		}

		public void CreateTemporaryBinding (string api_definition)
		{
			EnsureTempDir ();
			var api = Path.Combine (TmpDirectory, "api.cs");
			File.WriteAllText (api, api_definition);
			ApiDefinitions.Add (api);
			WorkingDirectory = TmpDirectory;
		}
	}
}
