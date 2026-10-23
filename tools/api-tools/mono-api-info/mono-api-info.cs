//
// mono-api-info.cs - Dumps public assembly information to an xml file.
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (C) 2003-2008 Novell, Inc (http://www.novell.com)
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Xml;

using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;

namespace Mono.ApiTools {

#if !EXCLUDE_DRIVER
	class Driver {
		public static int Main (string [] args)
		{
			bool showHelp = false;
			string? output = null;
			List<string>? asms = null;
			ApiInfoConfig config = new ApiInfoConfig ();

			var options = new Mono.Options.OptionSet {
				{ "abi",
					"Generate ABI, not API; contains only classes with instance fields which are not [NonSerialized].",
					v => config.AbiMode = v is not null },
				{ "f|follow-forwarders",
					"Follow type forwarders.",
					v => config.FollowForwarders = v is not null },
				{ "ignore-inherited-interfaces",
					"Ignore interfaces on the base type.",
					v => config.IgnoreInheritedInterfaces = v is not null },
				{ "ignore-resolution-errors",
					"Ignore any assemblies that cannot be found.",
					v => config.IgnoreResolutionErrors = v is not null },
				{ "d|L|lib|search-directory=",
					"Check for assembly references in {DIRECTORY}.",
					v => config.SearchDirectories.Add (v) },
				{ "r=",
					"Read and register the file {ASSEMBLY}, and add the directory containing ASSEMBLY to the search path.",
					v => config.ResolveFiles.Add (v) },
				{ "o|out|output=",
					"The output file. If not specified the output will be written to stdout.",
					v => output = v },
				{ "h|?|help",
					"Show this message and exit.",
					v => showHelp = v is not null },
				{ "contract-api",
					"Produces contract API with all members at each level of inheritance hierarchy",
					v => config.FullApiSet = v is not null },
			};

			try {
				asms = options.Parse (args);
			} catch (Mono.Options.OptionException e) {
				Console.WriteLine ("Option error: {0}", e.Message);
				asms = null;
			}

			if (showHelp || asms is null || asms.Count == 0) {
				Console.WriteLine ("usage: mono-api-info [OPTIONS+] ASSEMBLY+");
				Console.WriteLine ();
				Console.WriteLine ("Expose IL structure of CLR assemblies as XML.");
				Console.WriteLine ();
				Console.WriteLine ("Available Options:");
				Console.WriteLine ();
				options.WriteOptionDescriptions (Console.Out);
				Console.WriteLine ();
				return showHelp ? 0 : 1;
			}

			TextWriter? outputStream = null;
			try {
				if (!string.IsNullOrEmpty (output))
					outputStream = new StreamWriter (output);

				ApiInfo.Generate (asms, null, outputStream ?? Console.Out, config);
			} catch (Exception e) {
				Console.Error.WriteLine (e);
				return 1;
			} finally {
				outputStream?.Dispose ();
			}
			return 0;
		}
	}
#endif

	class State {
		public bool AbiMode { get; set; } = false;

		public bool FollowForwarders { get; set; } = false;

		public bool FullApiSet { get; set; } = false;

		public bool IgnoreResolutionErrors { get; set; } = false;

		public bool IgnoreInheritedInterfaces { get; set; } = false;

		public List<string> SearchDirectories { get; } = new List<string> ();

		public List<string> ResolveFiles { get; } = new List<string> ();

		public List<Stream> ResolveStreams { get; } = new List<Stream> ();

		public TypeHelper TypeHelper { get; private set; } = null!;

		public void ResolveTypes ()
		{
			TypeHelper = new TypeHelper (IgnoreResolutionErrors, IgnoreInheritedInterfaces);

			if (SearchDirectories is not null) {
				foreach (var v in SearchDirectories)
					TypeHelper.Resolver.AddSearchDirectory (v);
			}
			if (ResolveFiles is not null) {
				foreach (var v in ResolveFiles)
					TypeHelper.Resolver.ResolveFile (v);
			}
			if (ResolveStreams is not null) {
				foreach (var v in ResolveStreams)
					TypeHelper.Resolver.ResolveStream (v);
			}
		}
	}

	public class ApiInfoConfig {
		public bool AbiMode { get; set; } = false;

		public bool FollowForwarders { get; set; } = false;

		public bool FullApiSet { get; set; } = false;

		public bool IgnoreResolutionErrors { get; set; } = false;

		public bool IgnoreInheritedInterfaces { get; set; } = false;

		public List<string> SearchDirectories { get; set; } = new List<string> ();

		public List<string> ResolveFiles { get; set; } = new List<string> ();

		public List<Stream> ResolveStreams { get; set; } = new List<Stream> ();
	}

	public static class ApiInfo {
		public static void Generate (string assemblyPath, TextWriter outStream, ApiInfoConfig? config = null)
		{
			if (assemblyPath is null)
				throw new ArgumentNullException (nameof (assemblyPath));

			Generate (new [] { assemblyPath }, null, outStream, config);
		}

		public static void Generate (Stream assemblyStream, TextWriter outStream, ApiInfoConfig? config = null)
		{
			if (assemblyStream is null)
				throw new ArgumentNullException (nameof (assemblyStream));

			Generate (null, new [] { assemblyStream }, outStream, config);
		}

		public static void Generate (IEnumerable<string> assemblyPaths, TextWriter outStream, ApiInfoConfig? config = null)
		{
			Generate (assemblyPaths, null, outStream, config);
		}

		public static void Generate (IEnumerable<Stream> assemblyStreams, TextWriter outStream, ApiInfoConfig? config = null)
		{
			Generate (null, assemblyStreams, outStream, config);
		}

		public static void Generate (IEnumerable<string>? assemblyPaths, IEnumerable<Stream>? assemblyStreams, TextWriter outStream, ApiInfoConfig? config = null)
		{
			if (outStream is null)
				throw new ArgumentNullException (nameof (outStream));

			if (config is null)
				config = new ApiInfoConfig ();

			var state = new State {
				AbiMode = config.AbiMode,
				FollowForwarders = config.FollowForwarders,
				FullApiSet = config.FullApiSet,
				IgnoreResolutionErrors = config.IgnoreResolutionErrors,
				IgnoreInheritedInterfaces = config.IgnoreInheritedInterfaces,
			};
			state.SearchDirectories.AddRange (config.SearchDirectories);
			state.ResolveFiles.AddRange (config.ResolveFiles);
			state.ResolveStreams.AddRange (config.ResolveStreams);

			Generate (assemblyPaths, assemblyStreams, outStream, state);
		}

		internal static void Generate (IEnumerable<string>? assemblyFiles, IEnumerable<Stream>? assemblyStreams, TextWriter outStream, State? state = null)
		{
			if (outStream is null)
				throw new ArgumentNullException (nameof (outStream));

			if (state is null)
				state = new State ();

			state.ResolveTypes ();

			string windir = Environment.GetFolderPath (Environment.SpecialFolder.Windows);
			string pf = Environment.GetFolderPath (Environment.SpecialFolder.ProgramFiles);
			state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"assembly\GAC\MSDATASRC\7.0.3300.0__b03f5f7f11d50a3a"));

			var acoll = new AssemblyCollection (state);
			if (assemblyFiles is not null) {
				foreach (string arg in assemblyFiles) {
					acoll.Add (arg);

					if (arg.Contains ("v3.0")) {
						state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"Microsoft.NET\Framework\v2.0.50727"));
					} else if (arg.Contains ("v3.5")) {
						state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"Microsoft.NET\Framework\v2.0.50727"));
						state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"Microsoft.NET\Framework\v3.0\Windows Communication Foundation"));
					} else if (arg.Contains ("v4.0")) {
						if (arg.Contains ("Silverlight")) {
							state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (pf, @"Microsoft Silverlight\4.0.51204.0"));
						} else {
							state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"Microsoft.NET\Framework\v4.0.30319"));
							state.TypeHelper.Resolver.AddSearchDirectory (Path.Combine (windir, @"Microsoft.NET\Framework\v4.0.30319\WPF"));
						}
					} else {
						state.TypeHelper.Resolver.AddSearchDirectory (Path.GetDirectoryName (arg));
					}
				}
			}
			if (assemblyStreams is not null) {
				foreach (var arg in assemblyStreams) {
					acoll.Add (arg);
				}
			}

			var settings = new XmlWriterSettings {
				Indent = true,
			};
			using (var textWriter = XmlWriter.Create (outStream, settings)) {
				var writer = new WellFormedXmlWriter (textWriter);
				writer.WriteStartDocument ();
				acoll.Writer = writer;
				acoll.DoOutput ();
				writer.WriteEndDocument ();
				writer.Flush ();
			}
		}
	}

	class Utils {
		static char [] CharsToCleanup = new char [] { '<', '>', '/' };

		public static string CleanupTypeName (TypeReference type)
		{
			return CleanupTypeName (type.FullName);
		}

		public static string CleanupTypeName (string t)
		{
			if (t.IndexOfAny (CharsToCleanup) == -1)
				return t;
			var sb = new StringBuilder (t.Length);
			for (int i = 0; i < t.Length; i++) {
				var ch = t [i];
				switch (ch) {
				case '<':
					sb.Append ('[');
					break;
				case '>':
					sb.Append (']');
					break;
				case '/':
					sb.Append ('+');
					break;
				default:
					sb.Append (ch);
					break;
				}
			}
			return sb.ToString ();
		}
	}

	class AssemblyCollection {
		XmlWriter writer = null!;
		List<AssemblyDefinition> assemblies = new List<AssemblyDefinition> ();
		State state;

		public AssemblyCollection (State state)
		{
			this.state = state;
		}

		public bool Add (string name)
		{
			AssemblyDefinition assembly = LoadAssembly (name);
			assemblies.Add (assembly);
			return true;
		}

		public bool Add (Stream stream)
		{
			AssemblyDefinition assembly = LoadAssembly (stream);
			assemblies.Add (assembly);
			return true;
		}

		public void DoOutput ()
		{
			if (writer is null)
				throw new InvalidOperationException ("Document not set");

			writer.WriteStartElement ("assemblies");
			foreach (AssemblyDefinition a in assemblies) {
				AssemblyData data = new AssemblyData (writer, a, state);
				data.DoOutput ();
			}
			writer.WriteEndElement ();
		}

		public XmlWriter Writer {
			set { writer = value; }
		}

		AssemblyDefinition LoadAssembly (string assembly)
		{
			if (File.Exists (assembly))
				return state.TypeHelper.Resolver.ResolveFile (assembly);

			return state.TypeHelper.Resolver.Resolve (AssemblyNameReference.Parse (assembly), new ReaderParameters ());
		}

		AssemblyDefinition LoadAssembly (Stream assembly)
		{
			return state.TypeHelper.Resolver.ResolveStream (assembly);
		}
	}

	abstract class BaseData {
		protected XmlWriter writer;
		protected State state;

		protected BaseData (XmlWriter writer, State state)
		{
			this.writer = writer;
			this.state = state;
		}

		public abstract void DoOutput ();

		protected void AddAttribute (string name, string value)
		{
			writer.WriteAttributeString (name, value);
		}
	}

	class TypeForwardedToData : BaseData {
		AssemblyDefinition assembly;

		public TypeForwardedToData (XmlWriter writer, AssemblyDefinition assembly, State state)
			: base (writer, state)
		{
			this.assembly = assembly;
		}

		public override void DoOutput ()
		{
			foreach (ExportedType type in assembly.MainModule.ExportedTypes) {

				if (((uint) type.Attributes & 0x200000u) == 0)
					continue;

				writer.WriteStartElement ("attribute");
				AddAttribute ("name", typeof (TypeForwardedToAttribute).FullName!);
				writer.WriteStartElement ("properties");
				writer.WriteStartElement ("property");
				AddAttribute ("name", "Destination");
				AddAttribute ("value", Utils.CleanupTypeName (type.FullName));
				writer.WriteEndElement (); // properties
				writer.WriteEndElement (); // properties
				writer.WriteEndElement (); // attribute
			}
		}

		public static void OutputForwarders (XmlWriter writer, AssemblyDefinition assembly, State state)
		{
			TypeForwardedToData tftd = new TypeForwardedToData (writer, assembly, state);
			tftd.DoOutput ();
		}
	}

	class AssemblyData : BaseData {
		AssemblyDefinition assembly;

		public AssemblyData (XmlWriter writer, AssemblyDefinition assembly, State state)
			: base (writer, state)
		{
			this.assembly = assembly;
		}

		public override void DoOutput ()
		{
			if (writer is null)
				throw new InvalidOperationException ("Document not set");

			writer.WriteStartElement ("assembly");
			AssemblyNameDefinition aname = assembly.Name;
			AddAttribute ("name", aname.Name);
			AddAttribute ("version", aname.Version.ToString ());

			AttributeData.OutputAttributes (writer, state, assembly);

			var types = new List<TypeDefinition> ();
			if (assembly.MainModule.Types is not null) {
				types.AddRange (assembly.MainModule.Types);
			}

			if (state.FollowForwarders && assembly.MainModule.ExportedTypes is not null) {
				foreach (var t in assembly.MainModule.ExportedTypes) {
					var forwarded = t.Resolve ();
					if (forwarded is null) {
						throw new Exception ("Could not resolve forwarded type " + t.FullName + " in " + assembly.Name);
					}
					types.Add (forwarded);
				}
			}

			if (types.Count == 0) {
				writer.WriteEndElement (); // assembly
				return;
			}

			types.Sort (TypeReferenceComparer.Default);

			writer.WriteStartElement ("namespaces");

			string current_namespace = "$%&$&";
			bool in_namespace = false;
			foreach (TypeDefinition t in types) {
				if (string.IsNullOrEmpty (t.Namespace))
					continue;

				if (!state.AbiMode && ((t.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public))
					continue;

				if (t.DeclaringType is not null)
					continue; // enforce !nested

				if (t.Namespace != current_namespace) {
					current_namespace = t.Namespace;
					if (in_namespace) {
						writer.WriteEndElement (); // classes
						writer.WriteEndElement (); // namespace
					} else {
						in_namespace = true;
					}
					writer.WriteStartElement ("namespace");
					AddAttribute ("name", current_namespace);
					writer.WriteStartElement ("classes");
				}

				TypeData bd = new TypeData (writer, t, state);
				bd.DoOutput ();

			}

			if (in_namespace) {
				writer.WriteEndElement (); // classes
				writer.WriteEndElement (); // namespace
			}

			writer.WriteEndElement (); // namespaces

			writer.WriteEndElement (); // assembly
		}
	}

	abstract class MemberData : BaseData {
		MemberReference [] members;

		public MemberData (XmlWriter writer, MemberReference [] members, State state)
			: base (writer, state)
		{
			this.members = members;
		}

		protected virtual ICustomAttributeProvider? GetAdditionalCustomAttributeProvider (MemberReference member)
		{
			return null;
		}

		public override void DoOutput ()
		{
			writer.WriteStartElement (ParentTag);

			foreach (MemberReference member in members) {
				writer.WriteStartElement (Tag);
				AddAttribute ("name", GetName (member));
				if (!NoMemberAttributes)
					AddAttribute ("attrib", GetMemberAttributes (member) ?? "");
				AddExtraAttributes (member);

				var additionalProvider = GetAdditionalCustomAttributeProvider (member);
				if (additionalProvider is not null)
					AttributeData.OutputAttributes (writer, state, (ICustomAttributeProvider) member, additionalProvider);
				else
					AttributeData.OutputAttributes (writer, state, (ICustomAttributeProvider) member);

				AddExtraData (member);
				writer.WriteEndElement (); // Tag
			}

			writer.WriteEndElement (); // ParentTag
		}

		protected virtual void AddExtraData (MemberReference memberDefenition)
		{
		}

		protected virtual void AddExtraAttributes (MemberReference memberDefinition)
		{
		}

		protected virtual string GetName (MemberReference memberDefenition)
		{
			return "NoNAME";
		}

		protected virtual string? GetMemberAttributes (MemberReference memberDefenition)
		{
			return null;
		}

		public virtual bool NoMemberAttributes {
			get { return false; }
			set { }
		}

		public virtual string ParentTag {
			get { return "NoPARENTTAG"; }
		}

		public virtual string Tag {
			get { return "NoTAG"; }
		}

		public static void OutputGenericParameters (XmlWriter writer, IGenericParameterProvider provider, State state)
		{
			if (provider.GenericParameters.Count == 0)
				return;

			var gparameters = provider.GenericParameters;

			writer.WriteStartElement ("generic-parameters");

			foreach (GenericParameter gp in gparameters) {
				writer.WriteStartElement ("generic-parameter");
				writer.WriteAttributeString ("name", gp.Name);
				writer.WriteAttributeString ("attributes", ((int) gp.Attributes).ToString ());

				AttributeData.OutputAttributes (writer, state, gp);

				var constraints = gp.Constraints;
				if (constraints.Count == 0) {
					writer.WriteEndElement (); // generic-parameter
					continue;
				}

				writer.WriteStartElement ("generic-parameter-constraints");

				foreach (GenericParameterConstraint constraint in constraints) {
					writer.WriteStartElement ("generic-parameter-constraint");
					writer.WriteAttributeString ("name", Utils.CleanupTypeName (constraint.ConstraintType));
					writer.WriteEndElement (); // generic-parameter-constraint
				}

				writer.WriteEndElement (); // generic-parameter-constraints

				writer.WriteEndElement (); // generic-parameter
			}

			writer.WriteEndElement (); // generic-parameters
		}
	}

	class TypeData : MemberData {
		TypeDefinition type;

		public TypeData (XmlWriter writer, TypeDefinition type, State state)
			: base (writer, [], state)
		{
			this.type = type;
		}
		public override void DoOutput ()
		{
			if (writer is null)
				throw new InvalidOperationException ("Document not set");

			writer.WriteStartElement ("class");
			AddAttribute ("name", type.Name);
			string classType = GetClassType (type);
			AddAttribute ("type", classType);

			if (type.BaseType is not null)
				AddAttribute ("base", Utils.CleanupTypeName (type.BaseType));

			if (type.IsSealed)
				AddAttribute ("sealed", "true");

			if (type.IsAbstract)
				AddAttribute ("abstract", "true");

			if ((type.Attributes & TypeAttributes.Serializable) != 0 || type.IsEnum)
				AddAttribute ("serializable", "true");

			string charSet = GetCharSet (type);
			AddAttribute ("charset", charSet);

			string? layout = GetLayout (type);
			if (layout is not null)
				AddAttribute ("layout", layout);

			if (type.PackingSize >= 0) {
				AddAttribute ("pack", type.PackingSize.ToString ());
			}

			if (type.ClassSize >= 0) {
				AddAttribute ("size", type.ClassSize.ToString ());
			}

			if (type.IsEnum) {
				var value_type = GetEnumValueField (type);
				if (value_type is null)
					throw new NotSupportedException ();

				AddAttribute ("enumtype", Utils.CleanupTypeName (value_type.FieldType));
			}

			AttributeData.OutputAttributes (writer, state, type);

			var ifaces = state.TypeHelper.GetInterfaces (type).
				Where ((iface) => state.TypeHelper.IsPublic (iface)). // we're only interested in public interfaces
				OrderBy (s => s.FullName, StringComparer.Ordinal);

			if (ifaces.Any ()) {
				writer.WriteStartElement ("interfaces");
				foreach (TypeReference iface in ifaces) {
					writer.WriteStartElement ("interface");
					AddAttribute ("name", Utils.CleanupTypeName (iface));
					writer.WriteEndElement (); // interface
				}
				writer.WriteEndElement (); // interfaces
			}

			MemberData.OutputGenericParameters (writer, type, state);

			ArrayList members = new ArrayList ();

			FieldDefinition [] fields = GetFields (type);
			if (fields.Length > 0) {
				Array.Sort (fields, MemberReferenceComparer.Default);
				FieldData fd = new FieldData (writer, fields, state);
				members.Add (fd);
			}

			if (!state.AbiMode) {

				MethodDefinition [] ctors = GetConstructors (type);
				if (ctors.Length > 0) {
					Array.Sort (ctors, MethodDefinitionComparer.Default);
					members.Add (new ConstructorData (writer, ctors, state));
				}

				PropertyDefinition [] properties = GetProperties (type, state.FullApiSet);
				if (properties.Length > 0) {
					Array.Sort (properties, PropertyDefinitionComparer.Default);
					members.Add (new PropertyData (writer, properties, state));
				}

				EventDefinition [] events = GetEvents (type);
				if (events.Length > 0) {
					Array.Sort (events, MemberReferenceComparer.Default);
					members.Add (new EventData (writer, events, state));
				}

				MethodDefinition [] methods = GetMethods (type, state.FullApiSet);
				if (methods.Length > 0) {
					Array.Sort (methods, MethodDefinitionComparer.Default);
					members.Add (new MethodData (writer, methods, state));
				}
			}

			foreach (MemberData md in members)
				md.DoOutput ();

			var nested = type.NestedTypes;
			//remove non public(familiy) and nested in second degree
			for (int i = nested.Count - 1; i >= 0; i--) {
				TypeDefinition t = nested [i];
				if ((t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic ||
					(t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily ||
					(t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem) {
					// public
					if (t.DeclaringType == type)
						continue; // not nested of nested
				}

				nested.RemoveAt (i);
			}

			if (nested.Count > 0) {
				var nestedArray = nested.ToArray ();
				Array.Sort (nestedArray, TypeReferenceComparer.Default);

				writer.WriteStartElement ("classes");
				foreach (TypeDefinition t in nestedArray) {
					TypeData td = new TypeData (writer, t, state);
					td.DoOutput ();
				}
				writer.WriteEndElement (); // classes
			}

			writer.WriteEndElement (); // class
		}

		static FieldReference? GetEnumValueField (TypeDefinition type)
		{
			foreach (FieldDefinition field in type.Fields)
				if (field.IsSpecialName && field.Name == "value__")
					return field;

			return null;
		}

		protected override string GetMemberAttributes (MemberReference member)
		{
			if (member != type)
				throw new InvalidOperationException ("odd");

			return ((int) type.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		public static bool MustDocumentMethod (MethodDefinition method)
		{
			// All other methods
			MethodAttributes maskedAccess = method.Attributes & MethodAttributes.MemberAccessMask;
			return maskedAccess == MethodAttributes.Public
				|| maskedAccess == MethodAttributes.Family
				|| maskedAccess == MethodAttributes.FamORAssem;
		}

		string GetClassType (TypeDefinition t)
		{
			if (t.IsEnum)
				return "enum";

			if (t.IsValueType)
				return "struct";

			if (t.IsInterface)
				return "interface";

			if (state.TypeHelper.IsDelegate (t))
				return "delegate";

			if (t.IsPointer)
				return "pointer";

			return "class";
		}

		static string GetCharSet (TypeDefinition type)
		{
			TypeAttributes maskedStringFormat = type.Attributes & TypeAttributes.StringFormatMask;
			if (maskedStringFormat == TypeAttributes.AnsiClass)
				return CharSet.Ansi.ToString ();

			if (maskedStringFormat == TypeAttributes.AutoClass)
				return CharSet.Auto.ToString ();

			if (maskedStringFormat == TypeAttributes.UnicodeClass)
				return CharSet.Unicode.ToString ();

			return CharSet.None.ToString ();
		}

		static string? GetLayout (TypeDefinition type)
		{
			TypeAttributes maskedLayout = type.Attributes & TypeAttributes.LayoutMask;
			if (maskedLayout == TypeAttributes.AutoLayout)
				return LayoutKind.Auto.ToString ();

			if (maskedLayout == TypeAttributes.ExplicitLayout)
				return LayoutKind.Explicit.ToString ();

			if (maskedLayout == TypeAttributes.SequentialLayout)
				return LayoutKind.Sequential.ToString ();

			return null;
		}

		FieldDefinition [] GetFields (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			var fields = type.Fields;
			foreach (FieldDefinition field in fields) {
				if (field.IsSpecialName)
					continue;

				if (state.AbiMode && field.IsStatic)
					continue;

				// we're only interested in public or protected members
				FieldAttributes maskedVisibility = (field.Attributes & FieldAttributes.FieldAccessMask);
				if (state.AbiMode && !field.IsNotSerialized) {
					list.Add (field);
				} else {
					if (maskedVisibility == FieldAttributes.Public
						|| maskedVisibility == FieldAttributes.Family
						|| maskedVisibility == FieldAttributes.FamORAssem) {
						list.Add (field);
					}
				}
			}

			return (FieldDefinition []) list.ToArray (typeof (FieldDefinition));
		}


		internal PropertyDefinition [] GetProperties (TypeDefinition type, bool fullAPI)
		{
			var list = new List<PropertyDefinition> ();

			var t = type;
			do {
				var properties = t.Properties;//type.GetProperties (flags);
				foreach (PropertyDefinition property in properties) {
					MethodDefinition getMethod = property.GetMethod;
					MethodDefinition setMethod = property.SetMethod;

					bool hasGetter = (getMethod is not null) && MustDocumentMethod (getMethod);
					bool hasSetter = (setMethod is not null) && MustDocumentMethod (setMethod);

					// if neither the getter or setter should be documented, then
					// skip the property
					if (hasGetter || hasSetter) {

						if (t != type && list.Any (l => l.Name == property.Name))
							continue;

						list.Add (property);
					}
				}

				if (!fullAPI)
					break;

				if (t.IsInterface || t.IsEnum)
					break;

				if (t.BaseType is null || t.BaseType.FullName == "System.Object")
					t = null;
				else
					t = state.TypeHelper.GetBaseType (t);

			} while (t is not null);

			return list.ToArray ();
		}

		private MethodDefinition [] GetMethods (TypeDefinition type, bool fullAPI)
		{
			var list = new List<MethodDefinition> ();

			var t = type;
			do {
				var methods = t.Methods;//type.GetMethods (flags);
				foreach (MethodDefinition method in methods) {
					if (method.IsSpecialName && !method.Name.StartsWith ("op_", StringComparison.Ordinal))
						continue;

					// we're only interested in public or protected members
					if (!MustDocumentMethod (method))
						continue;

					if (t == type && IsFinalizer (method)) {
						string name = method.DeclaringType.Name;
						int arity = name.IndexOf ('`');
						if (arity > 0)
							name = name.Substring (0, arity);

						method.Name = "~" + name;
					}

					if (t != type && list.Any (l => l.DeclaringType != method.DeclaringType && l.Name == method.Name && l.Parameters.Count == method.Parameters.Count &&
											   l.Parameters.SequenceEqual (method.Parameters, new ParameterComparer ())))
						continue;

					list.Add (method);
				}

				if (!fullAPI)
					break;

				if (t.IsInterface || t.IsEnum)
					break;

				if (t.BaseType is null || t.BaseType.FullName == "System.Object")
					t = null;
				else
					t = state.TypeHelper.GetBaseType (t);

			} while (t is not null);

			return list.ToArray ();
		}

		sealed class ParameterComparer : IEqualityComparer<ParameterDefinition> {
			public bool Equals (ParameterDefinition? x, ParameterDefinition? y)
			{
				if (x is null || y is null)
					return x is null && y is null;
				return x.ParameterType.Name == y.ParameterType.Name;
			}

			public int GetHashCode (ParameterDefinition obj)
			{
				return obj.ParameterType.Name.GetHashCode ();
			}
		}

		static bool IsFinalizer (MethodDefinition method)
		{
			if (method.Name != "Finalize")
				return false;

			if (!method.IsVirtual)
				return false;

			if (method.Parameters.Count != 0)
				return false;

			return true;
		}

		private MethodDefinition [] GetConstructors (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			var ctors = type.Methods.Where (m => m.IsConstructor);//type.GetConstructors (flags);
			foreach (MethodDefinition constructor in ctors) {
				// we're only interested in public or protected members
				if (!MustDocumentMethod (constructor))
					continue;

				list.Add (constructor);
			}

			return (MethodDefinition []) list.ToArray (typeof (MethodDefinition));
		}

		private EventDefinition [] GetEvents (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			var events = type.Events;//type.GetEvents (flags);
			foreach (EventDefinition eventDef in events) {
				MethodDefinition addMethod = eventDef.AddMethod;//eventInfo.GetAddMethod (true);

				if (addMethod is null || !MustDocumentMethod (addMethod))
					continue;

				list.Add (eventDef);
			}

			return (EventDefinition []) list.ToArray (typeof (EventDefinition));
		}
	}

	class FieldData : MemberData {
		public FieldData (XmlWriter writer, FieldDefinition [] members, State state)
			: base (writer, members, state)
		{
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			FieldDefinition field = (FieldDefinition) memberDefenition;
			return field.Name;
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			FieldDefinition field = (FieldDefinition) memberDefenition;
			return ((int) field.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		protected override void AddExtraAttributes (MemberReference memberDefinition)
		{
			base.AddExtraAttributes (memberDefinition);

			FieldDefinition field = (FieldDefinition) memberDefinition;
			var fieldTypeName = NullabilityHelper.FormatTypeNameWithFullNullability (field.FieldType, field, field.DeclaringType);
			AddAttribute ("fieldtype", fieldTypeName);

			if (field.IsLiteral) {
				object value = field.Constant;//object value = field.GetValue (null);
				string? stringValue = null;
				//if (value is Enum) {
				//    // FIXME: when Mono bug #60090 has been
				//    // fixed, we should just be able to use
				//    // Convert.ToString
				//    stringValue = ((Enum) value).ToString ("D", CultureInfo.InvariantCulture);
				//}
				//else {
				stringValue = Convert.ToString (value, CultureInfo.InvariantCulture);
				//}

				if (stringValue is not null)
					AddAttribute ("value", stringValue);
			}
		}

		public override string ParentTag {
			get { return "fields"; }
		}

		public override string Tag {
			get { return "field"; }
		}
	}

	class PropertyData : MemberData {
		public PropertyData (XmlWriter writer, PropertyDefinition [] members, State state)
			: base (writer, members, state)
		{
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			PropertyDefinition prop = (PropertyDefinition) memberDefenition;
			return prop.Name;
		}

		MethodDefinition []? GetMethods (PropertyDefinition prop, out bool haveParameters)
		{
			MethodDefinition _get = prop.GetMethod;
			MethodDefinition _set = prop.SetMethod;
			bool haveGet = (_get is not null && TypeData.MustDocumentMethod (_get));
			bool haveSet = (_set is not null && TypeData.MustDocumentMethod (_set));
			haveParameters = haveGet || (haveSet && _set!.Parameters.Count > 1);
			MethodDefinition [] methods;

			if (haveGet && haveSet) {
				methods = new MethodDefinition [] { _get!, _set! };
			} else if (haveGet) {
				methods = new MethodDefinition [] { _get! };
			} else if (haveSet) {
				methods = new MethodDefinition [] { _set! };
			} else {
				//odd
				return null;
			}

			return methods;
		}

		protected override void AddExtraAttributes (MemberReference memberDefinition)
		{
			base.AddExtraAttributes (memberDefinition);

			PropertyDefinition prop = (PropertyDefinition) memberDefinition;
			var ptypeName = NullabilityHelper.FormatTypeNameWithFullNullability (prop.PropertyType, prop, prop.DeclaringType);
			AddAttribute ("ptype", ptypeName);

			bool haveParameters;
			MethodDefinition []? methods = GetMethods ((PropertyDefinition) memberDefinition, out haveParameters);

			if (methods is not null && haveParameters) {
				string parms = Parameters.GetSignature (methods [0].Parameters);
				if (!string.IsNullOrEmpty (parms))
					AddAttribute ("params", parms);
			}

		}

		protected override void AddExtraData (MemberReference memberDefenition)
		{
			base.AddExtraData (memberDefenition);

			bool haveParameters;
			MethodDefinition []? methods = GetMethods ((PropertyDefinition) memberDefenition, out haveParameters);

			if (methods is null)
				return;

			MethodData data = new MethodData (writer, methods, state);
			//data.NoMemberAttributes = true;
			data.DoOutput ();
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			PropertyDefinition prop = (PropertyDefinition) memberDefenition;
			return ((int) prop.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		public override string ParentTag {
			get { return "properties"; }
		}

		public override string Tag {
			get { return "property"; }
		}
	}

	class EventData : MemberData {
		public EventData (XmlWriter writer, EventDefinition [] members, State state)
			: base (writer, members, state)
		{
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			EventDefinition evt = (EventDefinition) memberDefenition;
			return evt.Name;
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			EventDefinition evt = (EventDefinition) memberDefenition;
			return ((int) evt.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		protected override void AddExtraAttributes (MemberReference memberDefinition)
		{
			base.AddExtraAttributes (memberDefinition);

			EventDefinition evt = (EventDefinition) memberDefinition;
			var evtTypeName = NullabilityHelper.FormatTypeNameWithFullNullability (evt.EventType, evt, evt.DeclaringType);
			AddAttribute ("eventtype", evtTypeName);
		}

		public override string ParentTag {
			get { return "events"; }
		}

		public override string Tag {
			get { return "event"; }
		}
	}

	class MethodData : MemberData {
		bool noAtts;

		public MethodData (XmlWriter writer, MethodDefinition [] members, State state)
			: base (writer, members, state)
		{
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			MethodDefinition method = (MethodDefinition) memberDefenition;
			string name = method.Name;
			string parms = Parameters.GetSignature (method.Parameters);

			return string.Format ("{0}({1})", name, parms);
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			MethodDefinition method = (MethodDefinition) memberDefenition;
			return ((int) (method.Attributes)).ToString (CultureInfo.InvariantCulture);
		}

		protected override ICustomAttributeProvider GetAdditionalCustomAttributeProvider (MemberReference member)
		{
			var mbase = (MethodDefinition) member;
			return mbase.MethodReturnType;
		}

		protected override void AddExtraAttributes (MemberReference memberDefinition)
		{
			base.AddExtraAttributes (memberDefinition);

			if (!(memberDefinition is MethodDefinition))
				return;

			MethodDefinition mbase = (MethodDefinition) memberDefinition;

			if (mbase.IsAbstract)
				AddAttribute ("abstract", "true");
			if (mbase.IsVirtual)
				AddAttribute ("virtual", "true");
			if (mbase.IsFinal && mbase.IsVirtual && mbase.IsReuseSlot)
				AddAttribute ("sealed", "true");
			if (mbase.IsStatic)
				AddAttribute ("static", "true");
			var baseMethod = state.TypeHelper.GetBaseMethodInTypeHierarchy (mbase);
			if (baseMethod is not null && baseMethod != mbase) {
				// This indicates whether this method is an override of another method.
				// This information is not necessarily available in the api info for any
				// particular assembly, because a method is only overriding another if
				// there is a base virtual function with the same signature, and that
				// base method can come from another assembly.
				AddAttribute ("is-override", "true");
			}
			string rettype = NullabilityHelper.FormatTypeNameWithFullNullability (mbase.MethodReturnType.ReturnType, mbase.MethodReturnType, mbase);
			if (rettype != "System.Void" || !mbase.IsConstructor) {
				AddAttribute ("returntype", rettype);
			}
			//
			//			if (mbase.MethodReturnType.HasCustomAttributes)
			//				AttributeData.OutputAttributes (writer, mbase.MethodReturnType);
		}

		protected override void AddExtraData (MemberReference memberDefenition)
		{
			base.AddExtraData (memberDefenition);

			if (!(memberDefenition is MethodDefinition))
				return;

			MethodDefinition mbase = (MethodDefinition) memberDefenition;

			ParameterData parms = new ParameterData (writer, mbase.Parameters, state) {
				HasExtensionParameter = mbase.CustomAttributes.Any (l => l.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute")
			};

			parms.DoOutput ();

			MemberData.OutputGenericParameters (writer, mbase, state);
		}

		public override bool NoMemberAttributes {
			get { return noAtts; }
			set { noAtts = value; }
		}

		public override string ParentTag {
			get { return "methods"; }
		}

		public override string Tag {
			get { return "method"; }
		}
	}

	class ConstructorData : MethodData {
		public ConstructorData (XmlWriter writer, MethodDefinition [] members, State state)
			: base (writer, members, state)
		{
		}

		public override string ParentTag {
			get { return "constructors"; }
		}

		public override string Tag {
			get { return "constructor"; }
		}
	}

	class ParameterData : BaseData {
		private IList<ParameterDefinition> parameters;

		public ParameterData (XmlWriter writer, IList<ParameterDefinition> parameters, State state)
			: base (writer, state)
		{
			this.parameters = parameters;
		}

		public bool HasExtensionParameter { get; set; }

		public override void DoOutput ()
		{
			bool first = true;
			writer.WriteStartElement ("parameters");
			foreach (ParameterDefinition parameter in parameters) {
				writer.WriteStartElement ("parameter");
				AddAttribute ("name", parameter.Name);
				AddAttribute ("position", parameter.Method.Parameters.IndexOf (parameter).ToString (CultureInfo.InvariantCulture));
				AddAttribute ("attrib", ((int) parameter.Attributes).ToString ());

				string direction = first && HasExtensionParameter ? "this" : "in";
				first = false;

				var pt = parameter.ParameterType;
				var brt = pt as ByReferenceType;
				if (brt is not null) {
					direction = parameter.IsOut ? "out" : "ref";
					pt = brt.ElementType;
				}

				AddAttribute ("type", NullabilityHelper.FormatTypeNameWithFullNullability (pt, parameter, parameter.Method as ICustomAttributeProvider));

				if (parameter.IsOptional) {
					AddAttribute ("optional", "true");
					if (parameter.HasConstant)
						AddAttribute ("defaultValue", parameter.Constant is null ? "NULL" : parameter.Constant.ToString () ?? "");
				}

				if (direction != "in")
					AddAttribute ("direction", direction);

				AttributeData.OutputAttributes (writer, state, parameter);
				writer.WriteEndElement (); // parameter
			}
			writer.WriteEndElement (); // parameters
		}
	}

	class AttributeData {
		State state;

		public AttributeData (State state)
		{
			this.state = state;
		}

		public void DoOutput (XmlWriter writer, IList<ICustomAttributeProvider> providers)
		{
			if (writer is null)
				throw new InvalidOperationException ("Document not set");

			if (providers is null || providers.Count == 0)
				return;

			if (!providers.Any ((provider) => provider is not null && provider.HasCustomAttributes))
				return;

			writer.WriteStartElement ("attributes");

			foreach (var provider in providers) {
				if (provider is null)
					continue;

				if (!provider.HasCustomAttributes)
					continue;

				var assembly = provider as AssemblyDefinition;
				if (assembly is not null && !state.FollowForwarders)
					TypeForwardedToData.OutputForwarders (writer, assembly, state);

				var attributes = provider.CustomAttributes.
					Where ((att) => !SkipAttribute (att)).
					OrderBy ((a) => a.Constructor.DeclaringType.FullName, StringComparer.Ordinal);

				foreach (var att in attributes) {
					string attName = Utils.CleanupTypeName (att.Constructor.DeclaringType);

					writer.WriteStartElement ("attribute");
					writer.WriteAttributeString ("name", attName);

					if (att.HasConstructorArguments) {
						writer.WriteStartElement ("arguments");
						foreach (var arg in att.ConstructorArguments) {
							writer.WriteStartElement ("argument");
							writer.WriteAttributeString ("type", arg.Type.FullName);
							if (arg.Value is null) {

							} else {
								writer.WriteAttributeString ("value", arg.Value.ToString ());
							}
							writer.WriteEndElement (); // argument
						}
						writer.WriteEndElement (); // arguments
					}

					if (att.HasProperties) {
						writer.WriteStartElement ("properties");
						foreach (var prop in att.Properties) {
							writer.WriteStartElement ("property");
							writer.WriteAttributeString ("name", prop.Name);
							writer.WriteAttributeString ("type", prop.Argument.Type.FullName);
							if (prop.Argument.Value is null) {

							} else {
								writer.WriteAttributeString ("value", prop.Argument.Value.ToString ());
							}
							writer.WriteEndElement (); // property
						}
						writer.WriteEndElement (); // properties
					}

					writer.WriteEndElement (); // attribute
				}
			}

			writer.WriteEndElement (); // attributes
		}

		bool SkipAttribute (CustomAttribute attribute)
		{
			switch (attribute.AttributeType.FullName) {
			case "System.Runtime.CompilerServices.NativeIntegerAttribute":
				return false;
			case "System.Runtime.CompilerServices.NullableAttribute":
			case "System.Runtime.CompilerServices.NullableContextAttribute":
				// Nullability is already rendered via '?' annotations on type names.
				return true;
			}

			if (!state.TypeHelper.IsPublic (attribute))
				return true;

			return attribute.Constructor.DeclaringType.Name.EndsWith ("TODOAttribute", StringComparison.Ordinal);
		}

		public static void OutputAttributes (XmlWriter writer, State state, params ICustomAttributeProvider [] providers)
		{
			var data = new AttributeData (state);
			data.DoOutput (writer, providers);
		}
	}

	static class Parameters {

		public static string GetSignature (IList<ParameterDefinition> infos)
		{
			if (infos is null || infos.Count == 0)
				return string.Empty;

			var signature = new StringBuilder ();
			for (int i = 0; i < infos.Count; i++) {

				if (i > 0)
					signature.Append (", ");

				ParameterDefinition info = infos [i];

				string modifier = string.Empty;
				if (info.ParameterType.IsByReference) {
					if ((info.Attributes & ParameterAttributes.In) != 0)
						modifier = "in";
					else if ((info.Attributes & ParameterAttributes.Out) != 0)
						modifier = "out";
				}

				if (modifier.Length > 0) {
					signature.Append (modifier);
					signature.Append (" ");
				}

				signature.Append (Utils.CleanupTypeName (info.ParameterType));
			}

			return signature.ToString ();
		}

	}

	static class NullabilityHelper {
		const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
		const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";

		// Gets the NullableContextAttribute flag from a method or type
		public static byte? GetNullableContextFlag (ICustomAttributeProvider provider)
		{
			if (provider is null)
				return null;

			if (!provider.HasCustomAttributes)
				return GetNullableContextFromParent (provider);

			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.FullName != NullableContextAttributeName)
					continue;
				if (attr.ConstructorArguments.Count == 1 && attr.ConstructorArguments [0].Value is byte b)
					return b;
			}

			return GetNullableContextFromParent (provider);
		}

		static byte? GetNullableContextFromParent (ICustomAttributeProvider? provider)
		{
			if (provider is MethodDefinition method)
				return GetNullableContextFlag (method.DeclaringType);
			if (provider is PropertyDefinition prop)
				return GetNullableContextFlag (prop.DeclaringType);
			if (provider is FieldDefinition field)
				return GetNullableContextFlag (field.DeclaringType);
			if (provider is EventDefinition evt)
				return GetNullableContextFlag (evt.DeclaringType);
			if (provider is TypeDefinition type) {
				if (type.DeclaringType is not null)
					return GetNullableContextFlag (type.DeclaringType);
				// Fall back to module-level NullableContextAttribute
				return GetNullableContextFlagFromAttributes (type.Module);
			}
			return null;
		}

		static byte? GetNullableContextFlagFromAttributes (ICustomAttributeProvider? provider)
		{
			if (provider is null || !provider.HasCustomAttributes)
				return null;

			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.FullName != NullableContextAttributeName)
					continue;
				if (attr.ConstructorArguments.Count == 1 && attr.ConstructorArguments [0].Value is byte b)
					return b;
			}
			return null;
		}

		// Gets the full NullableAttribute byte array from a provider.
		// Returns null if no NullableAttribute is present.
		static byte []? GetNullabilityBytesFromProvider (ICustomAttributeProvider provider)
		{
			if (!provider.HasCustomAttributes)
				return null;

			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.FullName != NullableAttributeName)
					continue;
				if (attr.ConstructorArguments.Count != 1)
					continue;

				var arg = attr.ConstructorArguments [0];
				if (arg.Value is byte b)
					return new byte [] { b };
				if (arg.Value is CustomAttributeArgument [] arr) {
					var result = new byte [arr.Length];
					for (int i = 0; i < arr.Length; i++) {
						if (arr [i].Value is byte val) {
							result [i] = val;
						}
					}
					return result;
				}
			}

			return null;
		}

		// Gets the full nullability byte array for a provider, falling back to NullableContext.
		static byte []? GetFullNullabilityBytes (ICustomAttributeProvider provider, ICustomAttributeProvider? context)
		{
			var bytes = GetNullabilityBytesFromProvider (provider);
			if (bytes is not null)
				return bytes;

			if (context is not null) {
				var contextFlag = GetNullableContextFlag (context);
				if (contextFlag.HasValue)
					return new byte [] { contextFlag.Value };
			}

			return null;
		}

		// Formats a type name with full nullability annotations, including generic type arguments.
		// This reads the NullableAttribute byte array and applies '?' at every position in
		// the type tree (depth-first), not just the top-level type.
		public static string FormatTypeNameWithFullNullability (TypeReference type, ICustomAttributeProvider provider, ICustomAttributeProvider? context)
		{
			var bytes = GetFullNullabilityBytes (provider, context);
			if (bytes is null)
				return Utils.CleanupTypeName (type);

			int index = 0;
			return FormatTypeWithNullabilityBytes (type, bytes, ref index);
		}

		// Recursively formats a type name, consuming nullability bytes in depth-first order.
		// Value types don't consume bytes. Reference types consume one byte each.
		// A single-byte array means uniform nullability for all positions.
		static string FormatTypeWithNullabilityBytes (TypeReference type, byte [] bytes, ref int index)
		{
			// Void is never nullable
			if (type.FullName == "System.Void")
				return "System.Void";

			// Value types don't consume bytes from the NullableAttribute array.
			// Their nullability is structural (Nullable<T>).
			if (type.IsValueType) {
				if (type is GenericInstanceType valueGenType) {
					// Handle Nullable<T>
					if (valueGenType.ElementType.FullName == "System.Nullable`1") {
						return FormatTypeWithNullabilityBytes (valueGenType.GenericArguments [0], bytes, ref index) + "?";
					}
					// Other generic value types: recurse into args
					var sb = new StringBuilder ();
					sb.Append (CleanForNullability (valueGenType.ElementType.FullName));
					sb.Append ('[');
					for (int i = 0; i < valueGenType.GenericArguments.Count; i++) {
						if (i > 0)
							sb.Append (", ");
						sb.Append (FormatTypeWithNullabilityBytes (valueGenType.GenericArguments [i], bytes, ref index));
					}
					sb.Append (']');
					return sb.ToString ();
				}
				return Utils.CleanupTypeName (type);
			}

			// Reference types consume one byte
			byte currentByte;
			if (bytes.Length == 1) {
				// Single byte: uniform nullability for all positions
				currentByte = bytes [0];
			} else {
				currentByte = index < bytes.Length ? bytes [index] : (byte) 0;
				index++;
			}
			bool isNullable = currentByte == 2;

			if (type is ArrayType arrType) {
				string ranks = new string (',', arrType.Rank - 1);
				return FormatTypeWithNullabilityBytes (arrType.ElementType, bytes, ref index) + "[" + ranks + "]" + (isNullable ? "?" : "");
			}

			if (type is GenericInstanceType genType) {
				var sb = new StringBuilder ();
				sb.Append (CleanForNullability (genType.ElementType.FullName));
				sb.Append ('[');
				for (int i = 0; i < genType.GenericArguments.Count; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append (FormatTypeWithNullabilityBytes (genType.GenericArguments [i], bytes, ref index));
				}
				sb.Append (']');
				if (isNullable)
					sb.Append ('?');
				return sb.ToString ();
			}

			return Utils.CleanupTypeName (type) + (isNullable ? "?" : "");
		}

		// Converts / to + for nested types (same as CleanupTypeName but without <> replacement,
		// since we build the generic argument list ourselves with nullability annotations).
		// The generic arity suffix (e.g., `2) is preserved because mono-api-html uses it to detect generics.
		static string CleanForNullability (string name)
		{
			return name.Replace ('/', '+');
		}
	}

	class TypeReferenceComparer : IComparer<TypeReference> {
		public static TypeReferenceComparer Default = new TypeReferenceComparer ();

		public int Compare (TypeReference? a, TypeReference? b)
		{
			if (a is null && b is null) return 0;
			if (a is null) return -1;
			if (b is null) return 1;
			int result = String.Compare (a.Namespace, b.Namespace, StringComparison.Ordinal);
			if (result != 0)
				return result;

			return String.Compare (a.Name, b.Name, StringComparison.Ordinal);
		}
	}

	class MemberReferenceComparer : IComparer {
		public static MemberReferenceComparer Default = new MemberReferenceComparer ();

		public int Compare (object? a, object? b)
		{
			if (a is null && b is null) return 0;
			if (a is null) return -1;
			if (b is null) return 1;
			MemberReference ma = (MemberReference) a;
			MemberReference mb = (MemberReference) b;
			return String.Compare (ma.Name, mb.Name, StringComparison.Ordinal);
		}
	}

	class PropertyDefinitionComparer : IComparer<PropertyDefinition> {
		public static PropertyDefinitionComparer Default = new PropertyDefinitionComparer ();

		public int Compare (PropertyDefinition? ma, PropertyDefinition? mb)
		{
			if (ma is null && mb is null) return 0;
			if (ma is null) return -1;
			if (mb is null) return 1;
			int res = String.Compare (ma.Name, mb.Name, StringComparison.Ordinal);
			if (res != 0)
				return res;

			if (!ma.HasParameters && !mb.HasParameters)
				return 0;

			if (!ma.HasParameters)
				return -1;

			if (!mb.HasParameters)
				return 1;

			return MethodDefinitionComparer.Compare (ma.Parameters, mb.Parameters);
		}
	}

	class MethodDefinitionComparer : IComparer {
		public static MethodDefinitionComparer Default = new MethodDefinitionComparer ();

		public int Compare (object? a, object? b)
		{
			if (a is null && b is null) return 0;
			if (a is null) return -1;
			if (b is null) return 1;
			MethodDefinition ma = (MethodDefinition) a;
			MethodDefinition mb = (MethodDefinition) b;
			int res = String.Compare (ma.Name, mb.Name, StringComparison.Ordinal);
			if (res != 0)
				return res;

			if (!ma.HasParameters && !mb.HasParameters)
				return 0;

			if (!ma.HasParameters)
				return -1;

			if (!mb.HasParameters)
				return 1;

			res = Compare (ma.Parameters, mb.Parameters);
			if (res != 0)
				return res;

			if (ma.HasGenericParameters != mb.HasGenericParameters)
				return ma.HasGenericParameters ? -1 : 1;

			if (ma.HasGenericParameters && mb.HasGenericParameters) {
				res = ma.GenericParameters.Count - mb.GenericParameters.Count;
				if (res != 0)
					return res;
			}

			// operators can differ by only return type
			return string.CompareOrdinal (ma.ReturnType.FullName, mb.ReturnType.FullName);
		}

		public static int Compare (IList<ParameterDefinition> pia, IList<ParameterDefinition> pib)
		{
			var res = pia.Count - pib.Count;
			if (res != 0)
				return res;

			string siga = Parameters.GetSignature (pia);
			string sigb = Parameters.GetSignature (pib);
			return String.Compare (siga, sigb, StringComparison.Ordinal);
		}
	}
}

