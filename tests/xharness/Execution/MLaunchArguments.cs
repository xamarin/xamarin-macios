using System;
using System.Collections.Generic;
using System.Linq;

namespace Xharness.Execution {
	// mlaunch is really important and has a lot of arguments that are known but
	// used to be passed as strings. This class allows to add arguments without
	// knowing the exact string and will also validate that an argument that
	// needs a value does contain the value
	public enum MlaunchArgumentType {
		SdkRoot,
		ListDev,
		ListSim,
		OutputFormat,
		ListExtraData,
	}

	struct MlaunchArgument {
		public MlaunchArgumentType Type { get; set; }
		public string Value { get; set; }

		public MlaunchArgument (MlaunchArgumentType type, string value)
		{
			Type = type;
			Value = value;
			if (!IsValid (out var reason)) 
				throw new ArgumentException (reason);
		}

		public MlaunchArgument (MlaunchArgumentType type)
		{
			Type = type;
			Value = null;
			if (!IsValid (out var reason))
				throw new ArgumentException (reason);
		} 

		bool IsValid (out string reason)
		{
			reason = "";
			switch (Type) {
			case MlaunchArgumentType.SdkRoot:
			case MlaunchArgumentType.ListDev:
			case MlaunchArgumentType.ListSim:
			case MlaunchArgumentType.OutputFormat:
				if (Value == null) {
					reason = $"Argument type {Type} requires a value.";
					return false;
				}
				return true;
			case MlaunchArgumentType.ListExtraData:
				if (Value != null) {
					reason = $"Argument type {Type} does not take a value.";
					return false;
				}
				return true;
			default:
				break;
			}
			return true;
		}

		// return the string needed to be used in the cmd
		public string AsCommandLineArgument () => Type switch
		{
			MlaunchArgumentType.SdkRoot => $"--sdkroot={Value}",
			MlaunchArgumentType.ListDev => $"--listdev={Value}",
			MlaunchArgumentType.ListSim => $"--listsim={Value}",
			MlaunchArgumentType.OutputFormat => $"--output-format={Value}",
			MlaunchArgumentType.ListExtraData => "--list-extra-data",
			_ => null,
		};
	}

	public class MlaunchArguments {
		readonly List<MlaunchArgument> arguments = new List<MlaunchArgument> ();

		public MlaunchArguments (params MlaunchArgumentType [] args)
		{
			foreach (var arg in args)
				Add (arg);
		}

		public MlaunchArguments (params (MlaunchArgumentType type, string value) [] args)
		{
			foreach (var arg in args)
				Add (arg);
		}

		public void Add (MlaunchArgumentType type) => arguments.Add (new MlaunchArgument (type));
		public void Add ((MlaunchArgumentType type, string value) arg) => arguments.Add (new MlaunchArgument (arg.type, arg.value));
		public void Add (params MlaunchArgumentType [] types) => Array.ForEach (types, t => Add (t));
		public void Add (params (MlaunchArgumentType type, string value) [] arguments) => Array.ForEach (arguments, arg => Add (arg));
		public string AsCommandLine () => string.Join (" ", arguments.Select (a => a.AsCommandLineArgument ()));
		public IEnumerable<(MlaunchArgumentType type, string value)> GetArguments ()
		{
			foreach (var arg in arguments)
				yield return (type: arg.Type, value: arg.Value);
		}

	}
}
