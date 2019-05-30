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
		public bool InProcess = true; // if executed using an in-process bgen. Ignored if using the classic bgen (for XM/Classic), in which case we'll always use the out-of-process bgen.
		public bool ProcessEnums;

		public List<string> ApiDefinitions = new List<string> ();
		public List<string> Sources = new List<string> ();
		public List<string> References = new List<string> ();

		public string [] Defines;
		public string TmpDirectory;
		public string ResponseFile;
		public string WarnAsError; // Set to empty string to pass /warnaserror, set to non-empty string to pass /warnaserror:<nonemptystring>
		public string NoWarn; // Set to empty string to pass /nowarn, set to non-empty string to pass /nowarn:<nonemptystring>
		public string Out;

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

		string [] BuildArgumentArray ()
		{
			var sb = new List<string> ();
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
				sb.Add ($"--target-framework={targetFramework}");

			foreach (var ad in ApiDefinitions)
				sb.Add ($"--api={ad}");

			foreach (var s in Sources)
				sb.Add ($"-s={s}");

			foreach (var r in References)
				sb.Add ($"-r={r}");

			if (!string.IsNullOrEmpty (TmpDirectory))
				sb.Add ($"--tmpdir={TmpDirectory}");

			if (!string.IsNullOrEmpty (ResponseFile))
				sb.Add ($"@{ResponseFile}");

			if (!string.IsNullOrEmpty (Out))
				sb.Add ($"--out={Out}");

			if (ProcessEnums)
				sb.Add ("--process-enums");

			if (Defines != null) {
				foreach (var d in Defines)
					sb.Add ($"-d={d}");
			}

			if (WarnAsError != null) {
				var arg = "--warnaserror";
				if (WarnAsError.Length > 0)
					arg += ":" + WarnAsError;
				sb.Add (arg);
			}

			if (NoWarn != null) {
				var arg = "--nowarn";
				if (NoWarn.Length > 0)
					arg += ":" + NoWarn;
				sb.Add (arg);
			}
			sb.Add ("-v");
			return sb.ToArray ();
		}

		public void AssertExecute (string message)
		{
			Assert.AreEqual (0, Execute (), message);
		}

		public void AssertExecuteError (string message)
		{
			Assert.AreNotEqual (0, Execute (), message);
		}

		int Execute ()
		{
			var arguments = BuildArgumentArray ();
			var in_process = InProcess && Profile != Profile.macOSClassic;
			if (in_process) {
				int rv;
				var previous_environment = new Dictionary<string, string> ();
				foreach (var kvp in EnvironmentVariables) {
					previous_environment [kvp.Key] = Environment.GetEnvironmentVariable (kvp.Key);
					Environment.SetEnvironmentVariable (kvp.Key, kvp.Value);
				}
				ThreadStaticTextWriter.ReplaceConsole (Output);
				try {
					rv = BindingTouch.Main (arguments);
				} finally {
					ThreadStaticTextWriter.RestoreConsole ();
					foreach (var kvp in previous_environment) {
						Environment.SetEnvironmentVariable (kvp.Key, kvp.Value);
					}
				}
				Console.WriteLine (Output);
				ParseMessages ();
				return rv;
			}
			return Execute (string.Join (" ", StringUtils.Quote (arguments)), always_show_output: true);
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
				assembly = AssemblyDefinition.ReadAssembly (Out ?? (Path.Combine (TmpDirectory, Path.GetFileNameWithoutExtension (ApiDefinitions [0]).Replace ('-', '_') + ".dll")));
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
			Out = Path.Combine (WorkingDirectory, "api0.dll");
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

	// This class will replace stdout/stderr with its own thread-static storage for stdout/stderr.
	// This means we're capturing stdout/stderr per thread.
	class ThreadStaticTextWriter : TextWriter
	{
		[ThreadStatic]
		static TextWriter current_writer;

		static ThreadStaticTextWriter instance = new ThreadStaticTextWriter ();
		static object lock_obj = new object ();
		static int counter;

		static TextWriter original_stdout;
		static TextWriter original_stderr;

		public static void ReplaceConsole (StringBuilder sb)
		{
			lock (lock_obj) { 
				if (counter == 0) {
					original_stdout = Console.Out;
					original_stderr = Console.Error;
					Console.SetOut (instance);
					Console.SetError (instance);
				}
				counter++;
				current_writer = new StringWriter (sb);
			}
		}

		public static void RestoreConsole ()
		{ 
			lock (lock_obj) {
				current_writer.Dispose ();
				current_writer = null;
				counter--;
				if (counter == 0) {
					Console.SetOut (original_stdout);
					Console.SetError (original_stderr);
					original_stdout = null;
					original_stderr = null;
				}
			}
		}

		ThreadStaticTextWriter ()
		{
		}

		public TextWriter CurrentWriter {
			get {
				if (current_writer == null)
					return original_stdout;
				return current_writer;
			}
		}

		public override Encoding Encoding => Encoding.UTF8;

		public override void WriteLine ()
		{
			lock (lock_obj)
				CurrentWriter.WriteLine ();
		}

		public override void Write (char value)
		{
			lock (lock_obj)
				CurrentWriter.Write (value);
		}

		public override void Write (string value)
		{
			lock (lock_obj)
				CurrentWriter.Write (value);
		}

		public override void Write (char [] buffer)
		{
			lock (lock_obj)
				CurrentWriter.Write (buffer);
		}

		public override void WriteLine (string value)
		{
			lock (lock_obj)
				CurrentWriter.WriteLine (value);
		}
	}
}
