using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Utils;

namespace Xamarin.Tests
{
	class BGenTool : Tool
	{
		AssemblyDefinition assembly;

		public Profile Profile;
		public bool ProcessEnums;

		public List<string> ApiDefinitions = new List<string> ();
		public List<string> Sources = new List<string> ();
		public List<string> References = new List<string> ();

		public string [] Defines;
		public string TmpDirectory;
		public string ResponseFile;
		public string WarnAsError; // Set to empty string to pass /warnaserror, set to non-empty string to pass /warnaserror:<nonemptystring>
		public string NoWarn; // Set to empty string to pass /nowarn, set to non-empty string to pass /nowarn:<nonemptystring>

		protected override string ToolPath { get { return Profile == Profile.macOSClassic ? Configuration.BGenClassicPath : Configuration.BGenPath; } }
		protected override string MessagePrefix { get { return "BI"; } }
		protected override string MessageToolName { get { return Profile == Profile.macOSClassic ? "bgen-classic" : "bgen"; } }

		public BGenTool ()
		{
			EnvironmentVariables = new Dictionary<string, string> {
				{ "MD_MTOUCH_SDK_ROOT", Configuration.SdkRootXI },
				{ "XamarinMacFrameworkRoot", Configuration.SdkRootXM },
			};
		}

		public void AddTestApiDefinition (string filename)
		{
			ApiDefinitions.Add (Path.Combine (Configuration.SourceRoot, "tests", "generator", filename));
		}

		public AssemblyDefinition ApiAssembly {
			get {
				LoadAssembly ();
				return assembly;
			}
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
			case Profile.macOSClassic:
				targetFramework = "XamMac,v1.0";
				break;
			case Profile.macOSFull:
				targetFramework = "Xamarin.Mac,Version=v4.5,Profile=Full";
				break;
			case Profile.macOSMobile:
				targetFramework = "Xamarin.Mac,Version=v2.0,Profile=Mobile";
				break;
			case Profile.macOSSystem:
				targetFramework = "Xamarin.Mac,Version=v4.5,Profile=System";
				break;
			default:
				throw new NotImplementedException ($"Profile: {Profile}");
			}

			if (!string.IsNullOrEmpty (targetFramework))
				sb.Append (" --target-framework=").Append (targetFramework);

			foreach (var ad in ApiDefinitions)
				sb.Append (" --api=").Append (StringUtils.Quote (ad));

			foreach (var s in Sources)
				sb.Append (" -s=").Append (StringUtils.Quote (s));

			foreach (var r in References)
				sb.Append (" -r=").Append (StringUtils.Quote (r));

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

		public void AssertApiCallsMethod (string caller_namespace, string caller_type, string caller_method, string @called_method, string message)
		{
			var type = ApiAssembly.MainModule.GetType (caller_namespace, caller_type);
			var method = type.Methods.First ((v) => v.Name == caller_method);

			AssertApiCallsMethod (method, called_method, message);
		}

		public void AssertApiCallsMethod (MethodReference method, string called_method, string message)
		{
			var instructions = method.Resolve ().Body.Instructions;
			foreach (var ins in instructions) {
				if (ins.OpCode.FlowControl != FlowControl.Call)
					continue;
				var mr = ins.Operand as MethodReference;
				if (mr == null)
					continue;
				if (mr.Name == called_method)
					return;
			}

			Assert.Fail ($"Could not find any instructions calling {called_method} in {method.FullName}: {message}\n\t{string.Join ("\n\t", instructions)}");
		}

		public void AssertApiLoadsField (string caller_type, string caller_method, string declaring_type, string field, string message)
		{
			var type = ApiAssembly.MainModule.GetTypes ().First ((v) => v.FullName == caller_type);
			var method = type.Methods.First ((v) => v.Name == caller_method);

			AssertApiLoadsField (method, declaring_type, field, message);
		}

		public void AssertApiLoadsField (MethodReference method, string declaring_type, string field, string message)
		{
			var instructions = method.Resolve ().Body.Instructions;
			foreach (var ins in instructions) {
				if (ins.OpCode.Code != Code.Ldsfld && ins.OpCode.Code != Code.Ldfld)
					continue;
				var fr = ins.Operand as FieldReference;
				if (fr == null)
					continue;
				if (fr.DeclaringType.FullName != declaring_type)
					continue;
				if (fr.Name == field)
					return;
			}

			Assert.Fail ($"Could not find any instructions loading the field {declaring_type}.{field} in {method.FullName}: {message}\n\t{string.Join ("\n\t", instructions)}");
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
			AssertMethod (typename, method, null, returnType, parameterTypes);
		}

		public void AssertMethod (string typename, string method, MethodAttributes? attributes = null, string returnType = null, params string [] parameterTypes)
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
			if (attributes.HasValue)
				Assert.AreEqual (attributes.Value, m.Attributes, "Attributes for {0}", m.FullName);
		}

		void LoadAssembly ()
		{
			if (assembly == null)
				assembly = AssemblyDefinition.ReadAssembly (Path.Combine (TmpDirectory, Path.GetFileNameWithoutExtension (ApiDefinitions [0]).Replace ('-', '_') + ".dll"));
		}

		void EnsureTempDir ()
		{
			if (TmpDirectory == null)
				TmpDirectory = Cache.CreateTemporaryDirectory ();
		}

		public void CreateTemporaryBinding (params string [] api_definition)
		{
			EnsureTempDir ();
			for (int i = 0; i < api_definition.Length; i++) {
				var api = Path.Combine (TmpDirectory, $"api{i}.cs");
				File.WriteAllText (api, api_definition [i]);
				ApiDefinitions.Add (api);
			}
			WorkingDirectory = TmpDirectory;
		}

		public static string [] GetDefaultDefines (Profile profile)
		{
			switch (profile) {
			case Profile.macOSFull:
			case Profile.macOSMobile:
			case Profile.macOSSystem:
				return new string [] { "MONOMAC", "XAMCORE_2_0" };
			case Profile.macOSClassic:
				return new string [] { "MONOMAC" };
			case Profile.iOS:
				return new string [] { "IOS", "XAMCORE_2_0" };
			default:
				throw new NotImplementedException (profile.ToString ());
			}
		}
	}
}
