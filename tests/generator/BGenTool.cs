using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Mono.Cecil;

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
		AssemblyDefinition assembly;

		public Profile Profile;
		public bool ProcessEnums;

		public List<string> ApiDefinitions = new List<string> ();
		public string [] Defines;
		public string TmpDirectory;
		public string ResponseFile;
		public string WarnAsError; // Set to empty string to pass /warnaserror, set to non-empty string to pass /warnaserror:<nonemptystring>
		public string NoWarn; // Set to empty string to pass /nowarn, set to non-empty string to pass /nowarn:<nonemptystring>

		protected override string ToolPath { get { return Configuration.BGenPath; } }
		protected override string MessagePrefix { get { return "BI"; } }
		protected override string MessageToolName { get { return "bgen"; } }

		public BGenTool ()
		{
			EnvironmentVariables = new Dictionary<string, string> {
				{ "MD_MTOUCH_SDK_ROOT", Configuration.SdkRootXI },
				{ "XamarinMacFrameworkRoot", Configuration.SdkRootXM },
			};
		}

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

			if (!string.IsNullOrEmpty (ResponseFile))
				sb.Append (" @").Append (StringUtils.Quote (ResponseFile));

			if (ProcessEnums)
				sb.Append (" --process-enums");

			if (Defines != null) {
				foreach (var d in Defines)
					sb.Append (" -d ").Append (StringUtils.Quote (d));
			}

			if (WarnAsError != null) {
				sb.Append (" --warnaserror");
				if (WarnAsError.Length > 0)
					sb.Append (":").Append (StringUtils.Quote (WarnAsError));
			}

			if (NoWarn != null) {
				sb.Append (" --nowarn");
				if (NoWarn.Length > 0)
					sb.Append (":").Append (StringUtils.Quote (NoWarn));
			}

			return sb.ToString ();
		}

		public void AssertExecute (string message)
		{
			Assert.AreEqual (0, Execute (BuildArguments (), always_show_output: true), message);
		}

		public void AssertExecuteError (string message)
		{
			Assert.AreNotEqual (0, Execute (BuildArguments ()), message);
		}

		public void AssertPublicTypeCount (int count, string message = null)
		{
			LoadAssembly ();

			var actual = assembly.MainModule.Types.Where ((v) => v.IsPublic || v.IsNestedPublic);
			if (actual.Count () != count)
				Assert.Fail ($"Expected {count} public type(s), found {actual} public type(s). {message}\n\t{string.Join ("\n\t", actual.Select ((v) => v.FullName).ToArray ())}");
		}

		public void AssertPublicMethodCount (string typename, int count, string message = null)
		{
			LoadAssembly ();

			var t = assembly.MainModule.Types.FirstOrDefault ((v) => v.FullName == typename);
			var actual = t.Methods.Count ((v) => {
				if (v.IsPrivate || v.IsFamily || v.IsFamilyAndAssembly)
					return false;
				return true;
			});
			if (actual != count)
				Assert.Fail ($"Expected {count} publicly accessible method(s) in {typename}, found {actual} publicly accessible method(s). {message}");
		}

		public void AssertType (string fullname, TypeAttributes? attributes = null, string message = null)
		{
			LoadAssembly ();

			var allTypes = assembly.MainModule.GetTypes ().ToArray ();
			var t = allTypes.FirstOrDefault ((v) => v.FullName == fullname);
			if (t == null)
				Assert.Fail ($"No type named '{fullname}' in the generated assembly. {message}\nList of types:\n\t{string.Join ("\n\t", allTypes.Select ((v) => v.FullName))}");
			if (attributes != null)
				Assert.AreEqual (attributes.Value, t.Attributes, $"Incorrect attributes for type {fullname}.");
		}

		public void AssertMethod (string typename, string method, string returnType = null, params string [] parameterTypes)
		{
			LoadAssembly ();

			var t = assembly.MainModule.Types.First ((v) => v.FullName == typename);
			var m = t.Methods.FirstOrDefault ((v) => {
				if (v.Name != method)
					return false;
				if (v.Parameters.Count != parameterTypes.Length)
					return false;
				for (int i = 0; i < v.Parameters.Count; i++)
					if (v.Parameters [i].ParameterType.FullName != parameterTypes [i])
						return false;
				return true;
			});
			if (m == null)
				Assert.Fail ($"No method '{method}' with signature '{string.Join ("', '", parameterTypes)}' was found.");
		}

		void LoadAssembly ()
		{
			if (assembly == null)
				assembly = AssemblyDefinition.ReadAssembly (Path.Combine (TmpDirectory, "api.dll"));
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

		public static string [] GetDefaultDefines (Profile profile)
		{
			switch (profile) {
			case Profile.macFull:
			case Profile.macModern:
			case Profile.macSystem:
				return new string [] { "MONOMAC", "XAMCORE_2_0" };
			case Profile.macClassic:
				return new string [] { "MONOMAC" };
			case Profile.iOS:
				return new string [] { "IOS", "XAMCORE_2_0" };
			default:
				throw new NotImplementedException (profile.ToString ());
			}
		}
	}
}
