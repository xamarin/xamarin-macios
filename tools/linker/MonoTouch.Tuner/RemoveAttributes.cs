using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;

using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class RemoveAttributes : CoreRemoveAttributes {

		static readonly HashSet<string> attribute_types = new HashSet<string> (StringComparer.Ordinal) {
			// Used by CA routines to check how attribute behaves. Removing this attributes causes fallback to default
			// attribute usage value which almost never matches the actual attribute usage value (most notably Inherited value)
			// "System.AttributeUsageAttribute",
			"System.CLSCompliantAttribute",
			"System.CodeDom.Compiler.GeneratedCodeAttribute",
			"System.ComponentModel.BrowsableAttribute",
			"System.ComponentModel.CategoryAttribute",
			"System.ComponentModel.DefaultEventAttribute",
			"System.ComponentModel.DefaultPropertyAttribute",
			// http://mef.codeplex.com/wikipage?title=Exports%20and%20Metadata
			// "System.ComponentModel.DefaultValueAttribute",
			"System.ComponentModel.Design.Serialization.DesignerSerializerAttribute",
			"System.ComponentModel.Design.Serialization.RootDesignerSerializerAttribute",
			"System.ComponentModel.DesignerAttribute",
			"System.ComponentModel.DesignerCategoryAttribute",
			"System.ComponentModel.DesignerSerializationVisibilityAttribute",
			"System.ComponentModel.DesignTimeVisibleAttribute",
			"System.ComponentModel.EditorAttribute",
			"System.ComponentModel.EditorBrowsableAttribute",
			"System.ComponentModel.InstallerTypeAttribute",
			"System.ComponentModel.MergablePropertyAttribute",
			"System.ComponentModel.NotifyParentPropertyAttribute",
			"System.ComponentModel.ReadOnlyAttribute",
			"System.ComponentModel.RecommendedAsConfigurableAttribute",
			"System.ComponentModel.ToolboxItemAttribute",
			// https://bugzilla.xamarin.com/show_bug.cgi?id=14456
			// "System.ComponentModel.TypeConverterAttribute",
			"System.Configuration.ConfigurationCollectionAttribute",
			"System.Configuration.ConfigurationPropertyAttribute",
			"System.Configuration.IntegerValidatorAttribute",
			"System.Diagnostics.ConditionalAttribute",
			"System.Diagnostics.MonitoringDescriptionAttribute",
			"System.Diagnostics.SwitchLevelAttribute",
			"System.Diagnostics.CodeAnalysis.SuppressMessageAttribute",
			// required for serialization, e.g. https://bugzilla.xamarin.com/show_bug.cgi?id=11135
			//"System.FlagsAttribute",
			"System.IO.IODescriptionAttribute",
			// for PlayScript runtime, ref: https://bugzilla.xamarin.com/show_bug.cgi?id=14165
			// "System.ParamArrayAttribute",
			"System.Reflection.AssemblyCompanyAttribute",
			"System.Reflection.AssemblyConfigurationAttribute",
			"System.Reflection.AssemblyCopyrightAttribute",
			"System.Reflection.AssemblyDefaultAliasAttribute",
			"System.Reflection.AssemblyDelaySignAttribute",
			"System.Reflection.AssemblyDescriptionAttribute",
			"System.Reflection.AssemblyFileVersionAttribute",
			"System.Reflection.AssemblyInformationalVersionAttribute",
			"System.Reflection.AssemblyKeyFileAttribute",
			"System.Reflection.AssemblyProductAttribute",
			"System.Reflection.AssemblyTitleAttribute",
			"System.Reflection.AssemblyTrademarkAttribute",
			"System.Reflection.DefaultMemberAttribute",
			"System.Resources.NeutralResourcesLanguageAttribute",
			"System.Resources.SatelliteContractVersionAttribute",
			"System.Runtime.TargetedPatchingOptOutAttribute",
			// internal in MS referencesource and not used by the mono runtime
			"System.Runtime.ForceTokenStabilizationAttribute",
			"System.Runtime.CompilerServices.CompilationRelaxationsAttribute",
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Runtime.CompilerServices.DecimalConstantAttribute",
			"System.Runtime.CompilerServices.DefaultDependencyAttribute",
			//"System.Runtime.CompilerServices.ExtensionAttribute", used at runtime by LINQ, see bug #3028
			//"System.Runtime.CompilerServices.RuntimeCompatibilityAttribute", used at runtime by runtime wrapped exception handling
			"System.Runtime.CompilerServices.StringFreezingAttribute",
			"System.Runtime.ConstrainedExecution.PrePrepareMethodAttribute",
			"System.Runtime.ConstrainedExecution.ReliabilityContractAttribute",
			"System.Runtime.InteropServices.ClassInterfaceAttribute",
			"System.Runtime.InteropServices.ComCompatibleVersionAttribute",
			"System.Runtime.InteropServices.ComDefaultInterfaceAttribute",
			"System.Runtime.InteropServices.ComImportAttribute",
			"System.Runtime.InteropServices.ComVisibleAttribute",
			"System.Runtime.InteropServices.DispIdAttribute",
			"System.Runtime.InteropServices.GuidAttribute",
			"System.Runtime.InteropServices.InterfaceTypeAttribute",
			"System.Runtime.InteropServices.LCIDConversionAttribute",
			"System.Runtime.InteropServices.TypeLibImportClassAttribute",
			"System.Runtime.InteropServices.TypeLibVersionAttribute",
			// used for bindings, ref maccore 1ab986d5b0fa6f4fb1117161685289854a229077
			// "System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute",
			"System.Runtime.Remoting.Metadata.SoapTypeAttribute",
			"System.Security.AllowPartiallyTrustedCallersAttribute",
			// internal in MS referencesource and not used by the mono runtime
			"System.Security.DynamicSecurityMethodAttribute",
			"System.Security.SecurityCriticalAttribute",
			"System.Security.SecuritySafeCriticalAttribute",
			"System.Security.SuppressUnmanagedCodeSecurityAttribute",
			"System.Security.UnverifiableCodeAttribute",
			// it's not a normal custom attribtue (security attributes are encoded differently)
			// but mcs has a bug that will encoded it as a normal custom attributes #30590
			"System.Security.Permissions.HostProtectionAttribute",
			"System.SRDescriptionAttribute",
			"System.Timers.TimersDescriptionAttribute",
		};

		// we could remove them if XmlSerialization is not linked into the .app
		// "System.Xml.Serialization.XmlAnyAttributeAttribute",
		// "System.Xml.Serialization.XmlAnyElementAttribute",
		// "System.Xml.Serialization.XmlArrayAttribute",
		// "System.Xml.Serialization.XmlArrayItemAttribute",
		// "System.Xml.Serialization.XmlAttributeAttribute",
		// "System.Xml.Serialization.XmlElementAttribute",
		// "System.Xml.Serialization.XmlEnumAttribute",
		// "System.Xml.Serialization.XmlIgnoreAttribute",
		// "System.Xml.Serialization.XmlNamespaceDeclarationsAttribute",
		// "System.Xml.Serialization.XmlRootAttribute",
		// "System.Xml.Serialization.XmlSchemaProviderAttribute",
		// "System.Xml.Serialization.XmlTextAttribute",
		// "System.Xml.Serialization.XmlTypeAttribute",

#if DEBUG
		HashSet<string> keep = new HashSet<string> ();
#endif
		bool debug_build;

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);
			debug_build = context.GetParameter ("debug-build") == "True";
		}

		protected override bool DebugBuild {
			get { return debug_build; }
		}

		protected override bool IsRemovedAttribute (CustomAttribute attribute)
		{
			if (base.IsRemovedAttribute (attribute))
				return true;

			var type = attribute.Constructor.DeclaringType;
			if (type.Namespace.StartsWith ("System", StringComparison.Ordinal)) {
				// FullName can allocate memory, precheck avoids it
				if (attribute_types.Contains (type.FullName))
					return true;
			}
#if DEBUG
			// report attributes that we do not remove so we can audit them on new projects
			if (!keep.Contains (type.FullName)) {
				keep.Add (type.FullName);
				Console.WriteLine ("Keeping attribute: {0}", type.FullName);
			}
#endif
			return false;
		}

		protected override void WillRemoveAttribute (ICustomAttributeProvider provider, CustomAttribute attribute)
		{
			var attr_type = attribute.AttributeType;
			switch (attr_type.Namespace) {
			case "System.Runtime.CompilerServices":
				switch (attr_type.Name) {
				case "CompilerGeneratedAttribute":
					LinkContext.StoreCustomAttribute (provider, attribute);
					break;
				}
				break;
			}

			base.WillRemoveAttribute (provider, attribute);
		}
	}
}
