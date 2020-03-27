using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Execution.Mlaunch {
	// mlaunch is really important and has a lot of arguments that are known but
	// used to be passed as strings. This class allows to add arguments without
	// knowing the exact string and will also validate that an argument that
	// needs a value does contain the value
	public abstract class MlaunchArgument {
		public abstract string AsCommandLineArgument ();

		protected static string Escape (string value) => StringUtils.FormatArguments (value);

		public override bool Equals (object obj)
		{
			return obj is MlaunchArgument arg && arg.AsCommandLineArgument () == AsCommandLineArgument ();
		}

		public override int GetHashCode ()
		{
			return AsCommandLineArgument ().GetHashCode ();
		}
	}

	public abstract class SingleValueArgument : MlaunchArgument {
		readonly string argumentName;
		readonly string argumentValue;
		readonly bool useEqualSign;

		protected SingleValueArgument (string argumentName, string argumentValue, bool useEqualSign = true)
		{
			this.argumentName = argumentName ?? throw new ArgumentNullException (nameof (argumentName));
			this.argumentValue = argumentValue ?? throw new ArgumentNullException (nameof (argumentValue));
			this.useEqualSign = useEqualSign;
		}

		public override string AsCommandLineArgument ()
		{
			if (useEqualSign)
				return Escape ($"--{argumentName}={argumentValue}");
			else
				return $"--{argumentName} {Escape (argumentValue)}";
		}
	}

	public abstract class OptionArgument : MlaunchArgument {
		readonly string argumentName;

		protected OptionArgument (string argumentName)
		{
			this.argumentName = argumentName ?? throw new ArgumentNullException (nameof (argumentName));
		}

		public override string AsCommandLineArgument () => $"--{argumentName}";
	}

	public class MlaunchArguments : IEnumerable<MlaunchArgument> {
		readonly List<MlaunchArgument> arguments = new List<MlaunchArgument> ();

		public MlaunchArguments (params MlaunchArgument [] args)
		{
			arguments.AddRange (args);
		}

		public void Add (MlaunchArgument arg) => arguments.Add (arg);

		public void AddRange (IEnumerable<MlaunchArgument> args) => arguments.AddRange (args);

		public string AsCommandLine () => string.Join (" ", arguments.Select (a => a.AsCommandLineArgument ()));

		public IEnumerator<MlaunchArgument> GetEnumerator () => arguments.GetEnumerator ();

		IEnumerator IEnumerable.GetEnumerator () => arguments.GetEnumerator ();

		public override string ToString () => AsCommandLine ();

		public override bool Equals (object obj)
		{
			return obj is MlaunchArguments arg && arg.AsCommandLine () == AsCommandLine ();
		}

		public override int GetHashCode ()
		{
			return AsCommandLine ().GetHashCode ();
		}
	}
}
