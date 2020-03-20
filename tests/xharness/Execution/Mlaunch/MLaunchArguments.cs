using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xharness.Execution.Mlaunch {
	// mlaunch is really important and has a lot of arguments that are known but
	// used to be passed as strings. This class allows to add arguments without
	// knowing the exact string and will also validate that an argument that
	// needs a value does contain the value
	public abstract class MlaunchArgument {
		public abstract string AsCommandLineArgument ();
	}

	public abstract class SingleValueArgument : MlaunchArgument {
		readonly string argumentName;
		readonly string argumentValue;

		protected SingleValueArgument (string argumentName, string argumentValue)
		{
			this.argumentName = argumentName ?? throw new ArgumentNullException (nameof (argumentName));
			this.argumentValue = argumentValue ?? throw new ArgumentNullException (nameof (argumentValue));
		}

		public override string AsCommandLineArgument () => $"--{argumentName}={argumentValue}";
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

		public IEnumerator<MlaunchArgument> GetEnumerator () => arguments.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator () => arguments.GetEnumerator ();

		public override string ToString () => AsCommandLine ();
	}
}
