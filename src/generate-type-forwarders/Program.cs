using System;
using System.IO;
using System.Linq;
using System.Text;

using Mono.Cecil;

namespace GenerateTypeForwarders {
	class MainClass {
		public static int Main (string [] args)
		{
			var forwardFrom = args [0];
			var forwardTo = args [1];
			var output = args [2];
			return Fix (forwardFrom, forwardTo, output);
		}

		static int Fix (string forwardFrom, string forwardTo, string output)
		{
			var from = AssemblyDefinition.ReadAssembly (forwardFrom);
			var to = AssemblyDefinition.ReadAssembly (forwardTo);
			var sb = new StringBuilder ();

			sb.AppendLine ("using System.Runtime.CompilerServices;");
			foreach (var type in from.MainModule.Types) {
				if (type.IsNotPublic)
					continue;

				var toType = to.MainModule.GetType (type.FullName);
				if (toType == null) {
					// FIXME: emit a class that throws PlatformNotSupportedExceptions
					sb.AppendLine ($"// Can't forward to {type.FullName}, it doesn't exist in the target assembly");
					continue;
				} else if (toType.IsNotPublic) {
					// FIXME: emit a class that throws PlatformNotSupportedExceptions
					sb.AppendLine ($"// Can't forward to {type.FullName}, it's not public in the target assembly");
					continue;
				}

				sb.Append ($"[assembly: TypeForwardedToAttribute (typeof (");
				sb.Append (type.Namespace);
				sb.Append (".");

				if (type.HasGenericParameters) {
					var nonGeneric = type.Name.Substring (0, type.Name.IndexOf ('`'));
					sb.Append (nonGeneric);
					sb.Append ('<');
					for (var i = 1; i < type.GenericParameters.Count; i++)
						sb.Append (',');
					sb.Append ('>');
				} else {
					sb.Append (type.Name);
				}
				sb.AppendLine ("))]");
			}
			File.WriteAllText (output, sb.ToString ());
			Console.WriteLine ($"Created type forwarders: {output}");
			return 0;
		}
	}
}

