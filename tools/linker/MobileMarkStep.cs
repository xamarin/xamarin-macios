// Copyright 2011-2013 Xamarin Inc., All rights reserved.
// adapted from MonoTouchMarkStep.cs, itself
// adapted from xtouch/tools/mtouch/Touch.Tuner/ManualMarkStep.cs

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Tuner;

namespace Xamarin.Linker.Steps {

	// XML definition files have their limits, i.e. they are good to keep stuff around unconditionnally
	// e.g. we do not want to force all/most Socket code around (non-network apps) because some types have unmanaged representation
	//
	// Generated backend fields inside monomac.dll are also removed if only used (i.e. set to null)
	// inside the Dispose method
	public class MobileMarkStep : MarkStep {

		protected virtual bool DebugBuild { get; set; }

		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base._context;
			}
		}

		public override void Process (LinkContext context)
		{
			base.Process (context);

			// deal with [TypeForwardedTo] pseudo-attributes
			foreach (AssemblyDefinition assembly in _context.GetAssemblies ()) {
				if (!assembly.MainModule.HasExportedTypes)
					continue;

				foreach (var exported in assembly.MainModule.ExportedTypes) {
					if (!exported.IsForwarder)
						continue;
					var type = exported.Resolve ();
					if (!Annotations.IsMarked (type))
						continue;
					Annotations.Mark (exported);
				}
			}
		}

		protected AssemblyDefinition GetAssembly (string assemblyName)
		{
			AssemblyDefinition ad;
			_context.TryGetLinkedAssembly (assemblyName, out ad);
			return ad;
		}

		protected TypeDefinition GetType (string assemblyName, string typeName)
		{
			AssemblyDefinition ad = GetAssembly (assemblyName);
			return ad == null ? null : GetType (ad, typeName);
		}

		protected TypeDefinition GetType (AssemblyDefinition assembly, string typeName)
		{
			return assembly.MainModule.GetType (typeName);
		}

		protected override TypeDefinition MarkType (TypeReference reference)
		{
			TypeDefinition type = base.MarkType (GetOriginalType (reference));
			if (type == null)
				return null;

			switch (type.Module.Assembly.Name.Name) {
			case "mscorlib":
				ProcessCorlib (type);
				break;
			case "System":
				ProcessSystem (type);
				break;
			case "System.Core":
				ProcessSystemCore (type);
				break;
			case "System.Data":
				ProcessSystemData (type);
				break;
			case "System.Data.Services.Client":
				ProcessSystemDataServicesClient (type);
				break;
			case "System.Runtime.Serialization":
				ProcessSystemRuntimeSerialization (type);
				break;
			case "System.ServiceModel":
				ProcessSystemServiceModel (type);
				break;
			case "System.Web.Services":
				ProcessSystemWebServices (type);
				break;
			case "System.Xml":
				ProcessSystemXml (type);
				break;
			case "System.Xml.Linq":
				ProcessSystemXmlLinq (type);
				break;
			case "Mono.Security":
				ProcessMonoSecurity (type);
				break;
			case "System.ComponentModel.Composition":
				ProcessSystemComponentModelComposition (type);
				break;
			}

			return type;
		}

		protected override void MarkMethods (TypeDefinition type)
		{
			// type can be null if we're not linking the assembly where the type reside,
			// e.g. --linkskip=System, in such case GetType returns null and an NRE can occur
			if (type == null)
				return;
			base.MarkMethods (type);
		}

		protected void MarkConstructors (TypeDefinition type)
		{
			if ((type == null) || !type.HasMethods)
				return;
			foreach (MethodDefinition ctor in type.Methods) {
				if (ctor.IsConstructor)
					MarkMethod (ctor);
			}
		}

		protected IDictionary<string, List<MethodDefinition>> PInvokeModules { get; set; }

		protected override MethodDefinition MarkMethod (MethodReference reference)
		{
			var method = base.MarkMethod (reference);
			if ((method != null) && !method.HasBody && method.IsPInvokeImpl) {
				// for some C++ stuff HasPInvokeInfo can be true without giving back any info
				PInvokeInfo info = method.PInvokeInfo;
				if (info != null) {
					var m = info.Module;
					Annotations.Mark (m);
					string name = m.Name;
					if (PInvokeModules != null && !String.IsNullOrEmpty (name)) {
						List<MethodDefinition> methods;
						if (!PInvokeModules.TryGetValue (name, out methods))
							PInvokeModules.Add (name, methods = new List<MethodDefinition> ());
						methods.Add (method);
					}
				}
			}

			return method;
		}

		protected override void MarkCustomAttribute (CustomAttribute ca)
		{
			base.MarkCustomAttribute (ca);

			if (ca.HasConstructorArguments) {
				PreserveTypeConverters (ca);
				if (DebugBuild)
					PreserveDebuggerAttributes (ca);
			}
		}

		void PreserveTypeConverters (CustomAttribute ca)
		{
			if (!ca.Constructor.DeclaringType.Is ("System.ComponentModel", "TypeConverterAttribute"))
				return;

			var type = (ca.ConstructorArguments [0].Value as TypeReference);
			// note: it could be a string (valid or not) and we do not support that since it's too late
			// to add new assemblies into the pipeline
			if (type != null)
				MarkDefaultConstructor (type.Resolve ());
		}

		void PreserveDebuggerAttributes (CustomAttribute ca)
		{
			if (!ca.AttributeType.Is ("System.Diagnostics", "DebuggerTypeProxyAttribute"))
				return;

			// preserve the type of the proxy for better debugging
			var arg = ca.ConstructorArguments [0];
			var type = arg.Type;
			if (!type.Is ("System", "Type"))
				return;

			var tr = (arg.Value as TypeReference);
			if (tr == null)
				return;

			var td = tr.Resolve ();
			// the debugger will instantiate the ctor (signature based on the type)
			MarkConstructors (td);
			// and will query its properties (getters), so they must be preserved
			// anything else will be marked when the linker process the code
			if (!td.HasProperties)
				return;
			foreach (var p in td.Properties) {
				MarkProperty (p);
				MarkMethod (p.GetMethod);
			}
		}

		// let product filters what get's included to help preserve extraneous serialization members
		// e.g. ensure we don't keep stuff that triggers DRM checks
		protected virtual IMetadataTokenProvider FilterExtraSerializationMembers (IMetadataTokenProvider provider)
		{
			return provider;
		}

		// in the cases where tracking the type is not possible (or easy), e.g. instances kept in a collection
		void MarkMetadata (IMetadataTokenProvider tp)
		{
			var provider = FilterExtraSerializationMembers (tp);
			if (provider == null)
				return;

			TypeReference tr = (provider as TypeReference);
			if (tr != null) {
				MarkType (tr);
				return;
			}
			MethodReference mr = (provider as MethodReference);
			if (mr != null) {
				MarkMethod (mr);
				return;
			}
			PropertyDefinition pd = (provider as PropertyDefinition);
			if (pd != null) {
				MarkProperty (pd);
				if (pd.GetMethod != null)
					MarkMethod (pd.GetMethod);
				if (pd.SetMethod != null)
					MarkMethod (pd.SetMethod);
				return;
			}
			FieldReference fr = (provider as FieldReference);
			if (fr != null) {
				MarkField (fr);
				return;
			}
			EventDefinition ed = (provider as EventDefinition);
			if (ed != null) {
				MarkEvent (ed);
				return;
			}
			throw new NotImplementedException (provider.ToString ());
		}

		// FIXME: we could be more precise (per field) but that would require a lot more maintenance for a very small gain

		void ProcessCorlib (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Runtime.CompilerServices":
				switch (type.Name) {
					case "AsyncTaskMethodBuilder":
					case "AsyncTaskMethodBuilder`1":
					if (DebugBuild) {
						MarkNamedMethod (type, "SetNotificationForWaitCompletion");
						MarkNamedMethod (type, "get_ObjectIdForDebugger");
					}
					break;
				}
				break;
			case "System.Threading.Tasks":
				switch (type.Name) {
					case "Task":
					if (DebugBuild)
						MarkNamedMethod (type, "NotifyDebuggerOfWaitCompletion");
					break;
				}
				break;
			case "System.Security.Cryptography":
				switch (type.Name) {
				case "Aes":
#if MONOTOUCH
					// Aes uses Activator.Create to be able to instantiate types from System.Core
					TypeDefinition aes = GetType ("System.Core", "System.Security.Cryptography.AesManaged");
					MarkMethods (aes);
#else
					// for historical reasons monotouch.dll shipped AesManaged and XA used (mono default's) CSP
					TypeDefinition aes = GetType ("System.Core", "System.Security.Cryptography.AesCryptoServiceProvider");
					MarkMethods (aes);
#endif
					break;
				}
				break;
			}
		}

		void ProcessSystem (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.ComponentModel":
				switch (type.Name) {
				case "TypeDescriptor":
					// DefaultConverters are created using Activator.CreateInstance
					// keep in sync with ReflectTypeDescriptionProvider.cs
					MarkMethods (GetType ("System", "System.ComponentModel.BooleanConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.ByteConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.SByteConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.CharConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.DoubleConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.StringConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.Int32Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.Int16Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.Int64Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.SingleConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.UInt16Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.UInt32Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.UInt64Converter"));
					MarkMethods (GetType ("System", "System.ComponentModel.TypeConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.CultureInfoConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.DateTimeConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.DateTimeOffsetConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.DecimalConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.TimeSpanConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.GuidConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.ArrayConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.CollectionConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.EnumConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.ReferenceConverter"));
					MarkMethods (GetType ("System", "System.ComponentModel.NullableConverter"));
					// special case - it's not in the Hashtable from the above file (but needed)
					MarkMethods (GetType ("System", "System.ComponentModel.ComponentConverter"));
					break;
				}
				break;
			case "System.Diagnostics":
				switch (type.Name) {
				// see mono/metadata/process.c
				case "FileVersionInfo":
				case "ProcessModule":
					// fields are initialized by the runtime, if the type is here then all (instance) fields must be present
					MarkFields (type, false);
					break;
				}
				break;
			case "System.Net.Sockets":
				switch (type.Name) {
				case "IPAddress":
					// mono/metadata/socket-io.c directly access 'm_Address' and 'm_Numbers'
					MarkFields (type, false);
					break;
				case "IPv6MulticastOption":
					// mono/metadata/socket-io.c directly access 'group' and 'ifIndex' private instance fields
					MarkFields (type, false);
					break;
				case "LingerOption":
					// mono/metadata/socket-io.c directly access 'enabled' and 'seconds' private instance fields
					MarkFields (type, false);
					break;
				case "MulticastOption":
					// mono/metadata/socket-io.c directly access 'group' and 'local' private instance fields
					MarkFields (type, false);
					break;
				case "Socket":
					// mono/metadata/socket-io.c directly access 'ipv4Supported', 'ipv6Supported' (static) and 'socket' (instance)
					MarkFields (type, true);
					break;
				case "SocketAddress":
					// mono/metadata/socket-io.c directly access 'data'
					MarkFields (type, false);
					break;
				}
				break;
			case "":
				if (!type.IsNested)
					break;

				switch (type.Name) {
				case "SocketAsyncResult":
					// mono/metadata/socket-io.h defines this structure (MonoSocketAsyncResult) for the runtime usage
					MarkFields (type, false);
					break;
				case "ProcessAsyncReader":
					// mono/metadata/socket-io.h defines this structure (MonoSocketAsyncResult) for the runtime usage
					MarkFields (type, false);
					break;
				}
				break;
			}
		}

		void ProcessSystemCore (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Linq":
				switch (type.Name) {
				case "Queryable":
					// reflection is used and the generic version might not be used elsewhere in the app, ref: #37563
					TypeDefinition enumquery = GetType ("System.Core", "System.Linq.EnumerableQuery`1");
					MarkConstructors (enumquery);
					break;
				}
				break;
			case "System.Linq.Expressions":
				switch (type.Name) {
				case "Expression`1":
					// internal Expression (Expression body, ReadOnlyCollection<ParameterExpression> parameters)
					// is called thru reflection by Expression.CreateExpressionOf and since it does a 
					// `typeof(Expression<>)` we have our clue to include the (lone) .ctor - ref bug #14863
					MarkConstructors (type);
					break;
				}
				break;
			}
		}

		void ProcessSystemData (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Data.SqlTypes":
				switch (type.Name) {
				case "SqlXml":
					// TODO: Needed only if CreateSqlReaderDelegate is used
					TypeDefinition xml_reader = GetType ("System.Xml", "System.Xml.XmlReader");
					MarkNamedMethod (xml_reader, "CreateSqlReader");
					break;
				}
				break;
			}
		}

		void ProcessSystemDataServicesClient (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Data.Services.Client":
				switch (type.Name) {
				case "TypeSystem":
					// the .cctor does a bunch of reflection on String, DateTime and Math members
					// some stuff on Microsoft.VisualBasic too but we do not ship that
					TypeDefinition string_td = GetType ("mscorlib", "System.String");
					MarkNamedMethod (string_td, "Contains");
					MarkNamedMethod (string_td, "EndsWith");
					MarkNamedMethod (string_td, "StartsWith");
					MarkNamedMethod (string_td, "IndexOf");
					MarkNamedMethod (string_td, "Replace");
					MarkNamedMethod (string_td, "Substring");
					MarkNamedMethod (string_td, "ToLower");
					MarkNamedMethod (string_td, "ToUpper");
					MarkNamedMethod (string_td, "Trim");
					MarkNamedMethod (string_td, "Concat");
					MarkNamedMethod (string_td, "get_Length");
					TypeDefinition datetime_td = GetType ("mscorlib", "System.DateTime");
					MarkNamedMethod (datetime_td, "get_Day");
					MarkNamedMethod (datetime_td, "get_Hour");
					MarkNamedMethod (datetime_td, "get_Month");
					MarkNamedMethod (datetime_td, "get_Minute");
					MarkNamedMethod (datetime_td, "get_Second");
					MarkNamedMethod (datetime_td, "get_Year");
					TypeDefinition math_td = GetType ("mscorlib", "System.Math");
					MarkNamedMethod (math_td, "Round");
					MarkNamedMethod (math_td, "Floor");
					MarkNamedMethod (math_td, "Ceiling");
					break;
				}
				break;
			}
		}

		bool system_runtime_serialization = false;

		void ProcessSystemRuntimeSerialization (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Runtime.Serialization.Json":
				switch (type.Name) {
				case "JsonSerializationWriter":
					// note: "Item" (indexer) is also used thru reflection but you can't create an app without it
					TypeDefinition dic = GetType ("mscorlib", "System.Collections.Generic.Dictionary`2");
					MarkNamedMethod (dic, "get_Keys");
					break;
				case "JsonFormatWriterInterpreter":
					TypeDefinition jwd = GetType ("System.Runtime.Serialization", "System.Runtime.Serialization.Json.JsonWriterDelegator");
					MarkMethods (jwd);
					break;
				}
				break;
			case "System.Runtime.Serialization":
				// MS referencesource use reflection to call the required methods to serialize each PrimitiveDataContract subclasses
				// this goes thru XmlFormatGeneratorStatics and it's a better candidate (than PrimitiveDataContract) as there are other callers
				switch (type.Name) {
				case "XmlFormatGeneratorStatics":
					TypeDefinition xwd = GetType ("System.Runtime.Serialization", "System.Runtime.Serialization.XmlWriterDelegator");
					MarkMethods (xwd);
					TypeDefinition xoswc = GetType ("System.Runtime.Serialization", "System.Runtime.Serialization.XmlObjectSerializerWriteContext");
					MarkMethods (xoswc);
					TypeDefinition xosrc = GetType ("System.Runtime.Serialization", "System.Runtime.Serialization.XmlObjectSerializerReadContext");
					MarkMethods (xosrc);
					TypeDefinition xrd = GetType ("System.Runtime.Serialization", "System.Runtime.Serialization.XmlReaderDelegator");
					MarkMethods (xrd);
					break;
				case "CollectionDataContract":
					// ensure the nested type, DictionaryEnumerator and GenericDictionaryEnumerator`2, can be created thru reflection
					foreach (var nt in type.NestedTypes)
						MarkConstructors (nt);
					break;
				}

				if (system_runtime_serialization)
					break;
				system_runtime_serialization = true;
				// if we're keeping this assembly and use the Serialization namespace inside user code then we
				// must bring the all the members decorated with [Data[Contract|Member]] attributes from the SDK
				var members = LinkContext.DataContract;
				foreach (var member in members)
					MarkMetadata (member);
				members.Clear ();
				break;
			}
		}

		void ProcessSystemServiceModel (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.ServiceModel.MonoInternal":
				switch (type.Name) {
				case "ClientRuntimeChannel":
					TypeDefinition nop = GetType ("System.ServiceModel", "System.ServiceModel.Channels.IChannelFactory`1");
					MarkNamedMethod (nop, "CreateChannel");
					break;
				}
				break;
			}
		}

		bool system_web_services_description = false;

		void ProcessSystemWebServices (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Web.Services.Description":
				if (system_web_services_description)
					break;
				system_web_services_description = true;
				// that's an all-or-nothing proposition: the whole hand will be taken if you point a finger at it
				foreach (var t in type.Module.Assembly.MainModule.Types) {
					if (t.Namespace != "System.Web.Services.Description")
						continue;
					MarkMethods (GetType ("System.Web.Services", t.FullName));
				}
				break;
			}
		}

		bool system_xml_serialization = false;

		void ProcessSystemXml (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Xml.Xsl":
				switch (type.Name) {
				case "XslCompiledTransform":
					TypeDefinition nop = GetType ("System.Xml", "System.Xml.Xsl.NoOperationDebugger");
					// only available on the mobile profile
					if (nop != null) {
						MarkNamedMethod (nop, "OnCompile");
						MarkNamedMethod (nop, "OnExecute");
					}
					break;
				}
				break;
			case "System.Xml.Serialization":
				if (system_xml_serialization)
					break;
				switch (type.Name) {
				case "XmlIgnoreAttribute":
					break;
				default:
					// if we're keeping this assembly and use the Serialization namespace inside user code
					// then we must bring the all the members decorated with [Xml*] attributes from the SDK
					system_xml_serialization = true;
					var members = LinkContext.XmlSerialization;
					foreach (var member in members)
						MarkMetadata (member);
					members.Clear ();
					break;
				}
				break;
			}
		}

		void ProcessSystemXmlLinq (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Xml.Linq":
				switch (type.Name) {
				case "XElement":
					// The addition of [XmlTypeConvertor ("ConvertForAssignment")] means the linker must keep the
					// ConvertForAssignment method around
					MarkNamedMethod (type, "ConvertForAssignment");
					break;
				}
				break;
			}
		}

		void ProcessMonoSecurity (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "Mono.Security.Protocol.Tls":
				switch (type.Name) {
				case "SslClientStream":
					MarkNamedMethod (type, "get_SelectedClientCertificate");
					break;
				case "SslStreamBase":
					MarkNamedMethod (type, "get_ServerCertificate");
					break;
				}
				break;
			}
		}

		void ProcessSystemComponentModelComposition (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.ComponentModel.Composition":
				switch (type.Name) {
				case "ExportServices":
					MarkNamedMethod (type, "CreateStronglyTypedLazyOfT");
					MarkNamedMethod (type, "CreateStronglyTypedLazyOfTM");
					MarkNamedMethod (type, "CreateSemiStronglyTypedLazy");
					break;
				}
				break;
			case "System.ComponentModel.Composition.ReflectionModel":
				switch (type.Name) {
				case "ExportFactoryCreator":
					MarkNamedMethod (type, "CreateStronglyTypedExportFactoryOfT");
					MarkNamedMethod (type, "CreateStronglyTypedExportFactoryOfTM");
					break;
				}
				break;
			}
		}
	}
}
