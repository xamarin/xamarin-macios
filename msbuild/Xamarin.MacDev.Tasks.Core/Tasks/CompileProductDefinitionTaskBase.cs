using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class CompileProductDefinitionTaskBase : XamarinTask {
		#region Inputs

		public string ProductDefinition { get; set; }

		[Required]
		public string OutputDirectory { get; set; }

		[Required]
		public string TargetArchitectures { get; set; }
		#endregion

		#region Outputs

		[Output]
		public ITaskItem CompiledProductDefinition { get; set; }

		#endregion

		protected TargetArchitecture architectures;

		public override bool Execute ()
		{
			PDictionary plist;

			if (File.Exists (ProductDefinition)) {
				try {
					plist = PDictionary.FromFile (ProductDefinition);
				} catch (Exception ex) {
					LogProductDefinitionError (MSBStrings.E0010, ProductDefinition, ex.Message);
					return false;
				}
			} else {
				plist = new PDictionary ();
			}

			if (!string.IsNullOrEmpty (TargetArchitectures) && !Enum.TryParse (TargetArchitectures, out architectures)) {
				LogProductDefinitionError (MSBStrings.E0012, TargetArchitectures);
				return false;
			}

			// productbuild can do a guess of the targeted architectures if not provided, but the guess
			// is very simple : on Catalina and lower, it will suppose it's x86_64 (even with an arm64 slice).
			HashSet <string> archStrings = new HashSet <string> (architectures.ToArray ().Select (a => a.ToNativeArchitecture ()));
			if (plist.TryGetValue (ProductDefinitionKeys.Architectures, out PArray archArray)) {
				var existingArchs = archArray.ToStringArray ();
				if (!archStrings.SetEquals (existingArchs)) {
					LogProductDefinitionWarning (MSBStrings.E7072, string.Join (", ", existingArchs), string.Join (", ", archStrings));
				}
			}

			if (archArray == null) {
				archArray = new PArray ();
				foreach (var arch in archStrings) {
					archArray.Add (new PString (arch));
				}
			}
			plist [ProductDefinitionKeys.Architectures] = archArray;

			CompiledProductDefinition = new TaskItem (Path.Combine (OutputDirectory, "Product.plist"));
			plist.Save (CompiledProductDefinition.ItemSpec, true, false);

			return !Log.HasLoggedErrors;
		}

		protected void LogProductDefinitionError (string format, params object [] args)
		{
			// Log an error linking to the Product.plist file
			Log.LogError (null, null, null, ProductDefinition, 0, 0, 0, 0, format, args);
		}

		protected void LogProductDefinitionWarning (string format, params object [] args)
		{
			// Log a warning linking to the Product.plist file
			Log.LogWarning (null, null, null, ProductDefinition, 0, 0, 0, 0, format, args);
		}
	}

	public static class ProductDefinitionKeys {
		public const string Architectures = "arch";
	}
}
