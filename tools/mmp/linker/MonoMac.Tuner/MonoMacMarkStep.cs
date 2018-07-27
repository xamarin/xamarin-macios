// Copyright 2011-2013 Xamarin Inc., All rights reserved.
// adapted from MonoTouchMarkStep.cs, itself
// adapted from xtouch/tools/mtouch/Touch.Tuner/ManualMarkStep.cs

using System;
using System.Collections.Generic; 
using Mono.Cecil;
using Mono.Linker;
using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;
using Mono.Tuner;
using Mono.Cecil.Cil;

namespace MonoMac.Tuner {

	// XML definition files have their limits, i.e. they are good to keep stuff around unconditionnally
	// e.g. we do not want to force all/most Socket code around (non-network apps) because some types have unmanaged representation
	public class MonoMacMarkStep : CoreMarkStep {
		
		public static bool IsClassic { get { return Driver.IsClassic; } }
		public static bool IsUnified { get { return Driver.IsUnified; } }

		List<Exception> Exceptions = new List<Exception> ();

		public override void Process (LinkContext context)
		{
			PInvokeModules = (context as MonoMacLinkContext).PInvokeModules;
			base.Process (context);

			if (Exceptions.Count > 0)
				throw new AggregateException (Exceptions);
		}

		// NOTE: most custom marking should happen in MobileMarkStep
		// Xamarin.Mac has _limited_ support for using the "full" Mono framework

		protected override TypeDefinition MarkType (TypeReference reference)
		{
			TypeDefinition type = base.MarkType (GetOriginalType (reference));
			if (type == null)
				return null;

			switch (type.Module.Assembly.Name.Name) {
			case "System":
				ProcessSystem (type);
				break;
			case "System.Configuration":
				ProcessSystemConfiguration (type);
				break;
			case "System.Drawing":
				ProcessSystemDrawing (type);
				break;
			case "System.Web":
				ProcessSystemWeb (type);
				break;
			case "System.Xml":
				ProcessSystemXml (type);
				break;
			case "XamMac":
			case "Xamarin.Mac":
				ProcessXamarinMac (type);
				break;
			}
			
			return type;
		}
		
		// FIXME: we could be more precise (per field) but that would require a lot more maintenance for a very small gain
		void ProcessSystem (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Net":
				switch (type.Name) {
				case "WebRequest":
					// System.Net.WebRequest uses reflection to call MonoTouch.CoreFoundation.CFNetwork::GetDefaultProxy()
					string typename = Namespaces.CoreFoundation + ".CFNetwork";
					TypeDefinition cfnetwork = GetType (ProductAssembly, typename);
					if (cfnetwork != null)
						MarkNamedMethod (cfnetwork, "GetDefaultProxy");

					// FIXME: this is the non-MOBILE version
					if (true) { // Mono.Tuner.Profile.Current is MobileProfile)
						// limited machine.config support
						WebRequestConfiguration ();
					}
					break;
				}
				break;
			}
		}
		
		void WebRequestConfiguration ()
		{
			// MarkMethods is used because the default .ctor is needed by Activation.Create
			MarkMethods (GetType ("System.Configuration", "System.Configuration.ExeConfigurationHost"));
			
			AssemblyDefinition system = GetAssembly ("System");
			
			// types we could directly infer from machine.config
			MarkMethods (GetType (system, "System.Net.Configuration.DefaultProxySection"));
			MarkMethods (GetType (system, "System.Net.Configuration.NetSectionGroup"));
			MarkMethods (GetType (system, "System.Net.Configuration.SettingsSection"));
			MarkMethods (GetType (system, "System.Net.Configuration.WebRequestModulesSection"));
			MarkMethods (GetType (system, "System.Net.HttpRequestCreator"));
			MarkMethods (GetType (system, "System.Net.FileWebRequestCreator"));
			MarkMethods (GetType (system, "System.Net.FtpWebRequestCreator"));
			
			// types we cannot find (statiscally or using machine.config)
			MarkMethods (GetType (system, "System.ComponentModel.BooleanConverter"));
			MarkMethods (GetType (system, "System.ComponentModel.CollectionConverter"));
			MarkMethods (GetType (system, "System.ComponentModel.EnumConverter"));
			MarkMethods (GetType (system, "System.ComponentModel.StringConverter"));
			MarkMethods (GetType (system, "System.ComponentModel.TimeSpanConverter"));
			MarkMethods (GetType (system, "System.Net.Configuration.BypassElementCollection"));
			MarkMethods (GetType (system, "System.Net.Configuration.HttpWebRequestElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.Ipv6Element"));
			MarkMethods (GetType (system, "System.Net.Configuration.PerformanceCountersElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.ProxyElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.ServicePointManagerElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.SocketElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.WebProxyScriptElement"));
			MarkMethods (GetType (system, "System.Net.Configuration.WebRequestModuleElementCollection"));
			MarkMethods (GetType (system, "System.Net.Configuration.WebRequestModuleElement"));
			MarkMethods (GetType (system, "System.UriTypeConverter"));

			// required for WebClient using Mono 3.0 and machine.config from 4.0
			MarkMethods (GetType (system, "System.Net.Configuration.ConnectionManagementSection"));
			MarkMethods (GetType (system, "System.Net.Configuration.ConnectionManagementElementCollection"));
		}

		void ProcessSystemConfiguration (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Configuration":
				switch (type.Name) {
				case "ConfigurationManager":
					MarkMethods (GetType ("System.Configuration", "System.Configuration.AppSettingsSection"));
					MarkMethods (GetType ("System.Configuration", "System.Configuration.KeyValueConfigurationCollection"));
					break;
				}
				break;
			}
		}

		void ProcessSystemDrawing (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Drawing.Imaging":
				switch (type.Name) {
				case "BitmapData":
					AssemblyDefinition sd = GetAssembly ("System.Drawing");
					MarkFields (GetType (sd, "System.Drawing.Imaging.BitmapData"), false);
					break;
				}
				break;
			}
		}

		void ProcessSystemWeb (TypeDefinition type)
		{
			// always include icalls when anything from System.Web is referenced
			// ? not clear why (monolinker / mmp old rule) since no reflection usage could be found ?
			// FIXME: keep for compatibility until further research
			AssemblyDefinition sw = GetAssembly ("System.Web");
			MarkMethods (GetType (sw, "System.Web.Util.ICalls"));
		}

		// note: this is the non-MOBILE version
		void ProcessSystemXml (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Xml.Serialization":
				switch (type.Name) {
				// sample/macdoc
				case "XmlSerializer":
					AssemblyDefinition system = GetAssembly ("System");
					MarkMethods (GetType (system, "System.Diagnostics.SystemDiagnosticsSection"));
					break;
				}
				break;
			}
		}

		void ProcessXamarinMac (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "AppKit":
			case "MonoMac.AppKit":
				switch (type.Name) {
				case "NSGraphicsContext":
					// bug #16505 NSPrintPreviewGraphicsContext can be needed at runtime (as it's returned
					// instead of a NSGraphicsContext when printing)
					AssemblyDefinition xm = GetAssembly ((Profile.Current as BaseProfile).ProductAssembly);
					MarkMethods (GetType (xm, Namespaces.AppKit + ".NSPrintPreviewGraphicsContext"));
					break;
				}
				break;
			}
		}
	}
}