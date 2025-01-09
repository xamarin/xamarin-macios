// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker.Steps;

#nullable enable

namespace Mono.Linker {

	public class LinkContext : IMetadataResolver, ITryResolveMetadata {

		readonly Pipeline _pipeline;
		readonly Dictionary<string, AssemblyAction> _actions;
		readonly Dictionary<string, string> _parameters;
		int? _targetRuntime;

		readonly AssemblyResolver _resolver;
		readonly AnnotationStore _annotations;
		readonly List<MessageContainer> _cachedWarningMessageContainers;
		readonly ILogger _logger;
		readonly Dictionary<AssemblyDefinition, bool> _isTrimmable;

		internal Pipeline Pipeline {
			get { return _pipeline; }

		}

		public AnnotationStore Annotations => _annotations;

		internal bool DeterministicOutput { get; set; }

		internal int ErrorsCount { get; set; }

		internal string OutputDirectory { get; set; }

		AssemblyAction TrimAction { get; set; } = AssemblyAction.Save;

		AssemblyAction DefaultAction { get; set; } = AssemblyAction.Copy;

		internal bool LinkSymbols { get; set; }

		internal bool PreserveSymbolPaths { get; set; }

		bool KeepComInterfaces { get; set; }

		bool KeepMembersForDebugger { get; set; } = true;

		bool IgnoreUnresolved { get; set; } = true;

		bool EnableReducedTracing { get; set; }

		bool KeepUsedAttributeTypesOnly { get; set; }

		bool EnableSerializationDiscovery { get; set; }

		bool DisableOperatorDiscovery { get; set; }

		/// <summary>
		/// Option to not special case EventSource.
		/// Currently, values are hard-coded and does not have a command line option to control
		/// </summary>
		bool DisableEventSourceSpecialHandling { get; set; }

		bool IgnoreDescriptors { get; set; }

		bool IgnoreSubstitutions { get; set; }

		bool IgnoreLinkAttributes { get; set; }

		internal List<PInvokeInfo> PInvokes { get; set; }

		internal string? PInvokesListFile;

		bool StripSecurity { get; set; }

		Dictionary<string, AssemblyAction> Actions {
			get { return _actions; }
		}

		internal AssemblyResolver Resolver {
			get { return _resolver; }
		}

		ISymbolReaderProvider SymbolReaderProvider { get; set; }

		bool LogMessages { get; set; }

		HashSet<int> NoWarn { get; set; }

		bool NoTrimWarn { get; set; }

		Dictionary<int, bool> WarnAsError { get; set; }

		bool GeneralWarnAsError { get; set; }

		WarnVersion WarnVersion { get; set; }

		internal HashSet<string>? TraceAssembly { get; set; }

		bool AddReflectionAnnotations { get; set; }

		internal string? AssemblyListFile { get; set; }

		List<IMarkHandler> MarkHandlers { get; }

		Dictionary<string, bool> SingleWarn { get; set; }

		bool GeneralSingleWarn { get; set; }

		HashSet<string> AssembliesWithGeneratedSingleWarning { get; set; }

		public LinkContext (Pipeline pipeline, ILogger logger, string outputDirectory)
		{
			_pipeline = pipeline;
			_logger = logger ?? throw new ArgumentNullException (nameof (logger));
			_actions = new Dictionary<string, AssemblyAction> ();
			_parameters = new Dictionary<string, string> (StringComparer.Ordinal);
			_cachedWarningMessageContainers = new List<MessageContainer> ();
			_isTrimmable = new Dictionary<AssemblyDefinition, bool> ();
			OutputDirectory = outputDirectory;

			SymbolReaderProvider = new DefaultSymbolReaderProvider (false);

			PInvokes = new List<PInvokeInfo> ();
			NoWarn = new HashSet<int> ();
			GeneralWarnAsError = false;
			WarnAsError = new Dictionary<int, bool> ();
			WarnVersion = WarnVersion.Latest;
			MarkHandlers = new List<IMarkHandler> ();
			GeneralSingleWarn = false;
			SingleWarn = new Dictionary<string, bool> ();
			AssembliesWithGeneratedSingleWarning = new HashSet<string> ();

			DisableEventSourceSpecialHandling = true;
			_annotations = new AnnotationStore (this);
			_resolver = new AssemblyResolver (this, new ReaderParameters ());
		}

		public TypeDefinition? GetType (string fullName)
		{
			int pos = fullName.IndexOf (',');
			fullName = TypeReferenceExtensions.ToCecilName (fullName);
			if (pos == -1) {
				foreach (AssemblyDefinition asm in GetReferencedAssemblies ()) {
					var type = asm.MainModule.GetType (fullName);
					if (type is not null)
						return type;
				}

				return null;
			}

			string asmname = fullName.Substring (pos + 1);
			fullName = fullName.Substring (0, pos);
			AssemblyDefinition? assembly = Resolve (AssemblyNameReference.Parse (asmname));
			return assembly?.MainModule.GetType (fullName);
		}

		AssemblyDefinition? TryResolve (string name)
		{
			return TryResolve (new AssemblyNameReference (name, new Version ()));
		}

		AssemblyDefinition? TryResolve (AssemblyNameReference name)
		{
			return _resolver.Resolve (name, probing: true);
		}

		AssemblyDefinition? Resolve (IMetadataScope scope)
		{
			AssemblyNameReference reference = GetReference (scope);
			return _resolver.Resolve (reference);
		}

		public AssemblyDefinition? Resolve (AssemblyNameReference name)
		{
			return _resolver.Resolve (name);
		}

		internal void RegisterAssembly (AssemblyDefinition assembly)
		{
			if (SeenFirstTime (assembly)) {
				SafeReadSymbols (assembly);
				Annotations.SetAction (assembly, CalculateAssemblyAction (assembly));
			}
		}

		protected bool SeenFirstTime (AssemblyDefinition assembly)
		{
			return !_annotations.HasAction (assembly);
		}

		void SafeReadSymbols (AssemblyDefinition assembly)
		{
			if (assembly.MainModule.HasSymbols)
				return;

			if (SymbolReaderProvider is null)
				throw new InvalidOperationException ("Symbol provider is not set");

			try {
				var symbolReader = SymbolReaderProvider.GetSymbolReader (
					assembly.MainModule,
					GetAssemblyLocation (assembly));

				if (symbolReader is null)
					return;

				try {
					assembly.MainModule.ReadSymbols (symbolReader);
				} catch {
					symbolReader.Dispose ();
					return;
				}

				// Add symbol reader to annotations only if we have successfully read it
				_annotations.AddSymbolReader (assembly, symbolReader);
			} catch { }
		}

		ICollection<AssemblyDefinition> ResolveReferences (AssemblyDefinition assembly)
		{
			List<AssemblyDefinition> references = new List<AssemblyDefinition> ();
			if (assembly is null)
				return references;

			foreach (AssemblyNameReference reference in assembly.MainModule.AssemblyReferences) {
				AssemblyDefinition? definition = Resolve (reference);
				if (definition is not null)
					references.Add (definition);
			}

			return references;
		}

		static AssemblyNameReference GetReference (IMetadataScope scope)
		{
			AssemblyNameReference reference;
			if (scope is ModuleDefinition moduleDefinition) {
				AssemblyDefinition asm = moduleDefinition.Assembly;
				reference = asm.Name;
			} else
				reference = (AssemblyNameReference) scope;

			return reference;
		}

		void RegisterAssemblyAction (string assemblyName, AssemblyAction action)
		{
			_actions [assemblyName] = action;
		}

#if !FEATURE_ILLINK
		void SetAction (AssemblyDefinition assembly, AssemblyAction defaultAction)
		{
			if (!_actions.TryGetValue (assembly.Name.Name, out AssemblyAction action))
				action = defaultAction;

			Annotations.SetAction (assembly, action);
		}
#endif
		AssemblyAction CalculateAssemblyAction (AssemblyDefinition assembly)
		{
			if (_actions.TryGetValue (assembly.Name.Name, out AssemblyAction action)) {
				if (IsCPPCLIAssembly (assembly.MainModule) && action != AssemblyAction.Copy && action != AssemblyAction.Skip) {
					return AssemblyAction.Copy;
				}

				return action;
			}

			if (IsCPPCLIAssembly (assembly.MainModule))
				return DefaultAction == AssemblyAction.Skip ? DefaultAction : AssemblyAction.Copy;

			if (IsTrimmable (assembly))
				return TrimAction;

			return DefaultAction;

			static bool IsCPPCLIAssembly (ModuleDefinition module)
			{
				foreach (var type in module.Types)
					if (type.Namespace == "<CppImplementationDetails>" ||
						type.Namespace == "<CrtImplementationDetails>")
						return true;

				return false;
			}
		}

		bool IsTrimmable (AssemblyDefinition assembly)
		{
			if (_isTrimmable.TryGetValue (assembly, out bool isTrimmable))
				return isTrimmable;

			if (!assembly.HasCustomAttributes) {
				_isTrimmable.Add (assembly, false);
				return false;
			}

			foreach (var ca in assembly.CustomAttributes) {
				if (!ca.AttributeType.IsTypeOf<AssemblyMetadataAttribute> ())
					continue;

				var args = ca.ConstructorArguments;
				if (args.Count != 2)
					continue;

				if (args [0].Value is not string key || !key.Equals ("IsTrimmable", StringComparison.OrdinalIgnoreCase))
					continue;

				if (args [1].Value is not string value || !value.Equals ("True", StringComparison.OrdinalIgnoreCase)) {
					continue;
				}

				isTrimmable = true;
			}

			_isTrimmable.Add (assembly, isTrimmable);
			return isTrimmable;
		}

		internal AssemblyDefinition [] GetAssemblies ()
		{
			var cache = _resolver.AssemblyCache;
			AssemblyDefinition [] asms = new AssemblyDefinition [cache.Count];
			cache.Values.CopyTo (asms, 0);
			return asms;
		}

		public AssemblyDefinition? GetLoadedAssembly (string name)
		{
			if (!string.IsNullOrEmpty (name) && _resolver.AssemblyCache.TryGetValue (name, out var ad))
				return ad;

			return null;
		}

		public string GetAssemblyLocation (AssemblyDefinition assembly)
		{
			return Resolver.GetAssemblyLocation (assembly);
		}

		IEnumerable<AssemblyDefinition> GetReferencedAssemblies ()
		{
			var assemblies = GetAssemblies ();

			foreach (var assembly in assemblies)
				yield return assembly;

			var loaded = new HashSet<AssemblyDefinition> (assemblies);
			var toProcess = new Queue<AssemblyDefinition> (assemblies);

			while (toProcess.Count > 0) {
				var assembly = toProcess.Dequeue ();
				foreach (var reference in ResolveReferences (assembly)) {
					if (!loaded.Add (reference))
						continue;
					yield return reference;
					toProcess.Enqueue (reference);
				}
			}
		}

		void SetCustomData (string key, string value)
		{
			_parameters [key] = value;
		}

		public bool HasCustomData (string key)
		{
			return _parameters.ContainsKey (key);
		}

		public bool TryGetCustomData (string key, out string? value)
		{
			return _parameters.TryGetValue (key, out value);
		}

		public void Dispose ()
		{
			_resolver.Dispose ();
		}

		internal void LogMessage (string message)
		{
			if (!LogMessages)
				return;

			LogMessage (MessageContainer.CreateInfoMessage (message));
		}

		void LogDiagnostic (string message)
		{
			if (!LogMessages)
				return;

			LogMessage (MessageContainer.CreateDiagnosticMessage (message));
		}

		public void LogMessage (MessageContainer message)
		{
			if (message == MessageContainer.Empty)
				return;

			if ((message.Category == MessageCategory.Diagnostic ||
				message.Category == MessageCategory.Info) && !LogMessages)
				return;

			if (message.Category == MessageCategory.Error || message.Category == MessageCategory.WarningAsError)
				ErrorsCount++;

			_logger.LogMessage (message);
		}


		/// <summary>
		/// Display an error message to the end user.
		/// </summary>
		/// <param name="text">Humanly readable message describing the error</param>
		/// <param name="code">Unique error ID. Please see https://github.com/dotnet/runtime/blob/main/docs/tools/illink/error-codes.md for the list of errors and possibly add a new one</param>
		/// <param name="subcategory">Optionally, further categorize this error</param>
		/// <param name="origin">Filename, line, and column where the error was found</param>
		internal void LogError (string text, int code, string subcategory = "", MessageOrigin? origin = null)
		{
			var error = MessageContainer.CreateErrorMessage (text, code, subcategory, origin);
			LogMessage (error);
		}

		internal bool IsWarningAsError (int warningCode)
		{
			bool value;
			if (GeneralWarnAsError)
				return !WarnAsError.TryGetValue (warningCode, out value) || value;

			return WarnAsError.TryGetValue (warningCode, out value) && value;
		}

		bool IsSingleWarn (string assemblyName)
		{
			bool value;
			if (GeneralSingleWarn)
				return !SingleWarn.TryGetValue (assemblyName, out value) || value;

			return SingleWarn.TryGetValue (assemblyName, out value) && value;
		}

		static WarnVersion GetWarningVersion ()
		{
			// This should return an increasing WarnVersion for new warning waves.
			return WarnVersion.ILLink5;
		}

		readonly Dictionary<MethodReference, MethodDefinition?> methodresolveCache = new ();
		readonly Dictionary<FieldReference, FieldDefinition?> fieldresolveCache = new ();
		readonly Dictionary<TypeReference, TypeDefinition?> typeresolveCache = new ();
		readonly Dictionary<ExportedType, TypeDefinition?> exportedTypeResolveCache = new ();

		/// <summary>
		/// Tries to resolve the MethodReference to a MethodDefinition and logs a warning if it can't
		/// </summary>
		public MethodDefinition? Resolve (MethodReference methodReference)
		{
			if (methodReference is MethodDefinition methodDefinition)
				return methodDefinition;

			if (methodReference is null)
				return null;

			if (methodresolveCache.TryGetValue (methodReference, out MethodDefinition? md)) {
				if (md is null && !IgnoreUnresolved)
					ReportUnresolved (methodReference);
				return md;
			}

#pragma warning disable RS0030 // Cecil's resolve is banned -- this provides the wrapper
			md = methodReference.Resolve ();
#pragma warning restore RS0030
			if (md is null && !IgnoreUnresolved)
				ReportUnresolved (methodReference);

			methodresolveCache.Add (methodReference, md);
			return md;
		}

		/// <summary>
		/// Tries to resolve the MethodReference to a MethodDefinition and returns null if it can't
		/// </summary>
		public MethodDefinition? TryResolve (MethodReference methodReference)
		{
			if (methodReference is MethodDefinition methodDefinition)
				return methodDefinition;

			if (methodReference is null)
				return null;

			if (methodresolveCache.TryGetValue (methodReference, out MethodDefinition? md))
				return md;

#pragma warning disable RS0030 // Cecil's resolve is banned -- this method provides the wrapper
			md = methodReference.Resolve ();
#pragma warning restore RS0030
			methodresolveCache.Add (methodReference, md);
			return md;
		}

		/// <summary>
		/// Tries to resolve the FieldReference to a FieldDefinition and logs a warning if it can't
		/// </summary>
		public FieldDefinition? Resolve (FieldReference fieldReference)
		{
			if (fieldReference is FieldDefinition fieldDefinition)
				return fieldDefinition;

			if (fieldReference is null)
				return null;

			if (fieldresolveCache.TryGetValue (fieldReference, out FieldDefinition? fd)) {
				if (fd is null && !IgnoreUnresolved)
					ReportUnresolved (fieldReference);
				return fd;
			}

			fd = fieldReference.Resolve ();
			if (fd is null && !IgnoreUnresolved)
				ReportUnresolved (fieldReference);

			fieldresolveCache.Add (fieldReference, fd);
			return fd;
		}

		/// <summary>
		/// Tries to resolve the FieldReference to a FieldDefinition and returns null if it can't
		/// </summary>
		public FieldDefinition? TryResolve (FieldReference fieldReference)
		{
			if (fieldReference is FieldDefinition fieldDefinition)
				return fieldDefinition;

			if (fieldReference is null)
				return null;

			if (fieldresolveCache.TryGetValue (fieldReference, out FieldDefinition? fd))
				return fd;

			fd = fieldReference.Resolve ();
			fieldresolveCache.Add (fieldReference, fd);
			return fd;
		}

		/// <summary>
		/// Tries to resolve the TypeReference to a TypeDefinition and logs a warning if it can't
		/// </summary>
		public TypeDefinition? Resolve (TypeReference typeReference)
		{
			if (typeReference is TypeDefinition typeDefinition)
				return typeDefinition;

			if (typeReference is null)
				return null;

			if (typeresolveCache.TryGetValue (typeReference, out TypeDefinition? td)) {
				if (td is null && !IgnoreUnresolved)
					ReportUnresolved (typeReference);
				return td;
			}

			//
			// Types which never have TypeDefinition or can have ambiguous definition should not be passed in
			//
			if (typeReference is GenericParameter || (typeReference is TypeSpecification && typeReference is not GenericInstanceType))
				throw new NotSupportedException ($"TypeDefinition cannot be resolved from '{typeReference.GetType ()}' type");

#pragma warning disable RS0030
			td = typeReference.Resolve ();
#pragma warning restore RS0030
			if (td is null && !IgnoreUnresolved)
				ReportUnresolved (typeReference);

			typeresolveCache.Add (typeReference, td);
			return td;
		}

		/// <summary>
		/// Tries to resolve the TypeReference to a TypeDefinition and returns null if it can't
		/// </summary>
		public TypeDefinition? TryResolve (TypeReference typeReference)
		{
			if (typeReference is TypeDefinition typeDefinition)
				return typeDefinition;

			if (typeReference is null || typeReference is GenericParameter)
				return null;

			if (typeresolveCache.TryGetValue (typeReference, out TypeDefinition? td))
				return td;

			if (typeReference is TypeSpecification ts) {
				if (typeReference is FunctionPointerType) {
					td = null;
				} else {
					//
					// It returns element-type for arrays and also element type for wrapping types like ByReference, PinnedType, etc
					//
					td = TryResolve (ts.GetElementType ());
				}
			} else {
#pragma warning disable RS0030
				td = typeReference.Resolve ();
#pragma warning restore RS0030
			}

			typeresolveCache.Add (typeReference, td);
			return td;
		}

		readonly HashSet<MethodDefinition> _processed_bodies_for_method = new HashSet<MethodDefinition> (2048);

		readonly HashSet<MemberReference> unresolved_reported = new ();

		readonly HashSet<ExportedType> unresolved_exported_types_reported = new ();

		protected virtual void ReportUnresolved (FieldReference fieldReference)
		{
			if (unresolved_reported.Add (fieldReference))
				LogError (string.Format ("FailedToResolveFieldElementMessage", fieldReference.FullName), 9999);
		}

		protected virtual void ReportUnresolved (MethodReference methodReference)
		{
			if (unresolved_reported.Add (methodReference))
				LogError (string.Format ("FailedToResolveMethodElementMessage", methodReference.GetDisplayName ()), 9999);
		}

		protected virtual void ReportUnresolved (TypeReference typeReference)
		{
			if (unresolved_reported.Add (typeReference))
				LogError (string.Format ("FailedToResolveTypeElementMessage", typeReference.GetDisplayName ()), 9999);
		}

		protected virtual void ReportUnresolved (ExportedType et)
		{
			if (unresolved_exported_types_reported.Add (et))
				LogError (string.Format ("FailedToResolveTypeElementMessage", et.Name), 9999);
		}
	}
}

#nullable restore
