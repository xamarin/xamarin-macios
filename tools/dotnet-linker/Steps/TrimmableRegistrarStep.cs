// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;

using Xamarin.Bundler;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Mono.Cecil.Rocks;
using Registrar;

#nullable enable

namespace Xamarin.Linker {
	public class TrimmableRegistrarStep : ConfigurationAwareStep {
		protected override string Name { get; } = "TrimmableRegistrar";
		protected override int ErrorCode { get; } = 2470;

		AppBundleRewriter abr { get { return Configuration.AppBundleRewriter; } }
		List<(string Path, AssemblyDefinition Assembly, string? OriginatingAssembly)> addedAssemblies = new ();
		List<Exception> exceptions = new List<Exception> ();

		void AddException (Exception exception)
		{
			if (exceptions is null)
				exceptions = new List<Exception> ();
			exceptions.Add (exception);
		}

		protected override void TryProcess ()
		{
			base.TryProcess ();

			if (App.Registrar != RegistrarMode.TrimmableStatic)
				return;

			if (App.IsPostProcessingAssemblies)
				return;

			Configuration.Application.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));
		}

		// Add [assembly: AssemblyMetadata ("IsTrimmable", "True")] to the given assembly.
		//
		// The type map assemblies are written to disk and then passed to ILLink as ordinary input assemblies
		// (this happens when PrepareAssemblies=true, where we generate the type map assemblies before ILLink runs).
		// ILLink only trims assemblies that are marked as trimmable when TrimMode is 'partial' (which is the
		// default for our apps) - any other assembly is copied as-is, which also roots everything it references.
		// The type map assemblies reference every Objective-C type in the app, so if they're not trimmed, nothing
		// else can be trimmed either.
		void MarkAssemblyAsTrimmable (AssemblyDefinition assembly)
		{
			var attribute = abr.CreateAttribute (abr.AssemblyMetadataAttribute_Constructor_String_String);
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, "IsTrimmable"));
			attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, "True"));
			assembly.CustomAttributes.Add (attribute);
		}

		AssemblyDefinition CreateTypeMapRootAssembly (ModuleParameters moduleParameters, IEnumerable<AssemblyDefinition> assemblies)
		{
			AssemblyDefinition rootTypeMapAssembly;

			// .NET 10 doesn't support a separate root type map assembly, so we have to add these attributes to the entry assembly instead.
			var useEntryAssemblyAsRootTypeMapAssembly = App.TargetFramework.Version.Major <= 10;
			var createdRootTypeMapAssemblyPath = Path.Combine (App.TypeMapOutputDirectory, App.TypeMapAssemblyName + ".dll");

			if (useEntryAssemblyAsRootTypeMapAssembly) {
				rootTypeMapAssembly = Configuration.EntryAssembly;
			} else {
				var rootTypeMapAssemblyName = new AssemblyNameDefinition (App.TypeMapAssemblyName, new Version (1, 0, 0, 0));
				rootTypeMapAssembly = AssemblyDefinition.CreateAssembly (rootTypeMapAssemblyName, rootTypeMapAssemblyName.Name, moduleParameters);
				Annotations.SetAction (rootTypeMapAssembly, AssemblyAction.Link);
				addedAssemblies.Add ((createdRootTypeMapAssemblyPath, rootTypeMapAssembly, Configuration.PlatformAssembly + ".dll"));

#if !ASSEMBLY_PREPARER
				// We're running from inside the linker, but the TypeMapEntryAssembly property can only be set using a command-line
				// argument, so we need to cheat a bit here and use reflection to set it. This will go away once we're not running
				// as a custom linker step anymore.
				var typeMapEntryAssemblyProperty = this.Context.GetType ().GetProperty ("TypeMapEntryAssembly");
				if (typeMapEntryAssemblyProperty is null)
					throw ErrorHelper.CreateError (99, "Could not find the 'TypeMapEntryAssembly' property on the linker context.");
				typeMapEntryAssemblyProperty.SetValue (this.Context, App.TypeMapAssemblyName);
#endif
			}

			abr.SetCurrentAssembly (rootTypeMapAssembly);

			// Don't mark the entry assembly as trimmable, we only want to do this for the assembly we created ourselves.
			if (!useEntryAssemblyAsRootTypeMapAssembly)
				MarkAssemblyAsTrimmable (rootTypeMapAssembly);

			foreach (var assembly in assemblies.OrderBy (v => v.FullName)) {
				var typeMapAssemblyName = RegistrarCompanionAssembly.GetName (assembly);

				/*
				 * [assembly: TypeMapAssemblyTarget<NSObject> ("...")]
				 */
				var attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type, abr.Foundation_NSObject));
				attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, typeMapAssemblyName));
				rootTypeMapAssembly.CustomAttributes.Add (attribute);

				/*
				 * [assembly: TypeMapAssemblyTarget<SkippedObjectiveCTypeUniverse> ("...")]
				 */
				attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type, abr.ObjCRuntime_SkippedObjectiveCTypeUniverse));
				attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, typeMapAssemblyName));
				rootTypeMapAssembly.CustomAttributes.Add (attribute);

				/*
				 * [assembly: TypeMapAssemblyTarget<INativeObject> ("...")]
				 */
				attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type, abr.ObjCRuntime_INativeObject));
				attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, typeMapAssemblyName));
				rootTypeMapAssembly.CustomAttributes.Add (attribute);

				/*
				 * [assembly: TypeMapAssemblyTarget<ProtocolProxyAttribute> ("...")]
				 */
				attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type, abr.ObjCRuntime_ProtocolProxyAttribute));
				attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, typeMapAssemblyName));
				rootTypeMapAssembly.CustomAttributes.Add (attribute);

				/*
				 * [assembly: TypeMapAssemblyTarget<ProtocolAttribute> ("...")]
				 */
				attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssemblyTargetAttribute_1_Constructor_String_Type_Type, abr.Foundation_ProtocolAttribute));
				attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, typeMapAssemblyName));
				rootTypeMapAssembly.CustomAttributes.Add (attribute);
			}
			abr.SaveCurrentAssembly ();
			abr.ClearCurrentAssembly ();

			if (!useEntryAssemblyAsRootTypeMapAssembly) {
				WriteDeterministically (rootTypeMapAssembly, createdRootTypeMapAssemblyPath);
			}
			return rootTypeMapAssembly;
		}

		// Writes the assembly to disk with a deterministic module version id (MVID) and timestamp.
		// The MVIDs of the typemap assemblies end up in the generated registrar code, so if we let Cecil
		// compute a new random MVID every time, the registrar code would change on every build, and we'd
		// have to recompile (and relink) it every time.
		void WriteDeterministically (AssemblyDefinition assembly, string path)
		{
			// Type-map assemblies are usually small enough to serialize in memory, avoiding a
			// temporary disk write just to detect unchanged output during incremental builds.
			using var contents = new MemoryStream ();
			assembly.Write (contents, new WriterParameters {
				DeterministicMvid = true,
				Timestamp = 0,
			});
			if (File.Exists (path)) {
				using var existing = File.OpenRead (path);
				contents.Position = 0;
				if (Cache.CompareStreams (App, existing, contents)) {
					App.Log (3, "Target {0} is up-to-date.", path);
					return;
				}
			}
			using var output = File.Create (path);
			contents.Position = 0;
			contents.CopyTo (output);
		}

		MethodReference CreateMethodReference (MethodReference methodReference, params TypeReference [] declaringTypeGenericArguments)
		{
			var methodDeclaringType = methodReference.DeclaringType;
			if (methodDeclaringType.HasGenericParameters) {
				if (declaringTypeGenericArguments.Length != methodDeclaringType.GenericParameters.Count)
					throw new ArgumentException ($"The number of generic arguments provided ({declaringTypeGenericArguments.Length}) does not match the number of generic parameters of the method's declaring type ({methodDeclaringType.GenericParameters.Count}).", nameof (declaringTypeGenericArguments));

				methodDeclaringType = methodDeclaringType.MakeGenericInstanceType (declaringTypeGenericArguments);
			}

			var method = new MethodReference (methodReference.Name, methodReference.ReturnType, methodDeclaringType) {
				HasThis = methodReference.HasThis,
				ExplicitThis = methodReference.ExplicitThis,
				CallingConvention = methodReference.CallingConvention,
			};

			foreach (var parameter in methodReference.Parameters)
				method.Parameters.Add (new ParameterDefinition (parameter.ParameterType));

			return abr.CurrentAssembly.MainModule.ImportReference (method);
		}

		static string GetNamespace (TypeReference tr)
		{
			return tr.FullName.Length == tr.Name.Length ? "" : tr.FullName.Substring (0, tr.FullName.Length - tr.Name.Length - 1).Replace (".", "__").Replace ("/", "__");
		}

		// Emits IL that throws a RuntimeException. This is used for the CreateObject method of the proxy types
		// we generate for generic types: we can't construct an instance of a generic type from a native handle
		// (we'd need the generic arguments, which we don't have), and emitting a 'newobj' instruction for an
		// open generic type produces invalid IL, so throw an exception instead.
		void EmitThrowCannotConstructGenericType (ILProcessor il, TypeReference type)
		{
			il.Append (il.Create (OpCodes.Ldc_I4, 4133));
			il.Append (il.Create (OpCodes.Ldstr, $"Cannot construct an instance of the type '{type.FullName}' from Objective-C because the type is generic."));
			il.Append (il.Create (OpCodes.Call, abr.Runtime_CreateRuntimeException));
			il.Append (il.Create (OpCodes.Throw));
		}

		// The types named by our type-map entries are only mentioned in the custom attribute blobs (as
		// assembly-qualified names), which means the type map assembly ends up without a TypeRef row for them.
		// That's valid metadata: ECMA-335 II.23.3 only requires the type to be stored as a SerString with its
		// canonical name, and neither the CustomAttribute (II.22.10) nor the TypeRef (II.22.38) validity rules
		// require a corresponding TypeRef row. The runtime agrees: it resolves these types by parsing the string,
		// not by looking at the TypeRef table (and the type map design explicitly says the assembly name in
		// TypeMapAssemblyTargetAttribute doesn't need a matching AssemblyRef row either).
		//
		// crossgen2 nonetheless assumes such a TypeRef row exists: it can't resolve the type back to a module
		// token, and crashes with a NotImplementedException (in ModuleTokenResolver.GetModuleTokenForType) when it
		// ReadyToRun-compiles the type map assembly. See https://github.com/dotnet/runtime/issues/131527.
		//
		// Work around that by emitting an unused method that loads the token of every externally defined type we
		// name, which makes Cecil emit the corresponding TypeRef rows. The method is never called, so it's trimmed
		// away again by ILLink.
		void EmitTypeReferencesForTypeMaps (AssemblyDefinition typeMapAssembly)
		{
			// Only crossgen2 needs this, and the added metadata isn't free, so don't do it for the other runtimes.
			// We don't narrow this down to ReadyToRun builds, because that would mean different behavior between
			// .NET versions (ReadyToRun is a .NET 11+ feature), and the cost for the interpreted CoreCLR
			// configurations is small (~64 KB per runtime identifier in the platform type map assembly).
			if (App.XamarinRuntime != XamarinRuntime.CoreCLR)
				return;

			var module = typeMapAssembly.MainModule;
			var externalTypes = new List<TypeReference> ();
			var seen = new HashSet<string> (StringComparer.Ordinal);

			foreach (var attribute in typeMapAssembly.CustomAttributes) {
				foreach (var argument in attribute.ConstructorArguments) {
					if (argument.Value is not TypeReference type)
						continue;
					// Types defined in the type map assembly itself already have a TypeDef row.
					if (type.Scope == module)
						continue;
					// Deduplicate on the scope too: two different assemblies can have types with the same full name.
					if (seen.Add (type.Scope?.Name + "!" + type.FullName))
						externalTypes.Add (type);
				}
			}

			if (externalTypes.Count == 0)
				return;

			var holderType = new TypeDefinition ("", "<TypeReferences>", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed, abr.System_Object);
			module.Types.Add (holderType);

			var method = holderType.AddMethod ("KeepTypeReferences", MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static, abr.System_Void);
			method.CreateBody (out var il);
			foreach (var type in externalTypes) {
				// 'ldtoken' works for open generic types too, unlike a field or parameter of that type.
				il.Append (il.Create (OpCodes.Ldtoken, type));
				il.Append (il.Create (OpCodes.Pop));
			}
			il.Append (il.Create (OpCodes.Ret));
		}

		protected override void TryEndProcess (out List<Exception>? exceptions)
		{
			CustomAttribute attribute;
			ILProcessor il;

			base.TryEndProcess ();

			if (App.Registrar != RegistrarMode.TrimmableStatic) {
				exceptions = null;
				return;
			}

			if (App.IsPostProcessingAssemblies) {
				// The assembly-preparer already created the type map assemblies, and the
				// TypeMapEntryAssembly MSBuild property tells ILLink about the root type map
				// assembly via --typemap-entry-assembly, so there's nothing to do here.
				exceptions = null;
				return;
			}

			abr.SetCurrentAssembly (abr.PlatformAssembly);
			abr.ObjCRuntime_NSObjectProxyAttribute.Resolve ().IsPublic = true;
			abr.ObjCRuntime_ProtocolProxyAttribute.Resolve ().IsPublic = true;
			abr.ObjCRuntime_INativeObjectProxyAttribute.Resolve ().IsPublic = true;
			abr.ObjCRuntime_SkippedObjectiveCTypeUniverse.Resolve ().IsPublic = true;
			abr.SaveCurrentAssembly ();
			abr.ClearCurrentAssembly ();

			Directory.CreateDirectory (App.TypeMapOutputDirectory);

			var typesByAssembly = App.StaticRegistrar.Types.GroupBy (v => v.Key.Module.Assembly);
			var skippedTypesByAssembly = App.StaticRegistrar.SkippedTypes.GroupBy (v => v.Skipped.Module.Assembly).ToDictionary (v => v.Key, v => v.ToList ());
			// Workaround for https://github.com/dotnet/runtime/issues/127504
			// Tracking issue: https://github.com/dotnet/macios/issues/25275
			//
			// Build a set of types that are the "actual" (non-generic) target of skipped type associations.
			// These are types like NSOrderedSet, NSArray, NSDictionary etc. that have generic variants
			// (NSOrderedSet<T>, NSArray<T>, NSDictionary<TKey,TValue>) mapping to the same ObjC class.
			//
			// These types need unconditional (2-arg) TypeMap entries instead of conditional (3-arg) ones
			// because of a bug in the linker's TypeMapHandler: when it processes the
			// TypeMapAssociationAttribute<SkippedObjectiveCTypeUniverse> for these types, it calls
			// MarkInstantiated directly (bypassing MarkRequirementsForInstantiatedTypes), which poisons
			// the IsInstantiated flag and prevents ProcessType from ever being called. This means their
			// conditional TypeMapAttribute entries (which require ProcessType to be promoted from
			// _unmarkedExternalTypeMapEntries) are silently trimmed by the linker.
			//
			// The runtime fix (https://github.com/dotnet/runtime/pull/127504) is only available in
			// .NET 11+, so the workaround is only applied when targeting .NET 10 or earlier.
			//
			// The workaround consists of two parts:
			//   1. This HashSet identifying the affected types (only populated when the workaround applies).
			//   2. The conditional block below (lines starting with 'if (applyTypeMapWorkaround && skippedActualTypes.Contains (td))')
			//      that uses the 2-arg TypeMapAttribute constructor for these types instead of the 3-arg one.
			//
			// To remove this workaround once the minimum supported .NET version includes the fix:
			//   1. Delete this HashSet, the applyTypeMapWorkaround variable, and this comment.
			//   2. Remove the 'if (applyTypeMapWorkaround && skippedActualTypes.Contains (td))' branch below, keeping only the 'else' branch.
			//   3. Verify by running: make build run-bare TEST_VARIATION='release|trimmable-static-registrar-all-optimizations-linkall' \
			//        RUN_ARGUMENTS="--test MonoTouchFixtures.Foundation.NSOrderedSetTest"
			//      in tests/monotouch-test/dotnet/MacCatalyst (the MakeNSOrderedSet_WithNull test is a good canary).
			var applyTypeMapWorkaround = App.TargetFramework.Version.Major <= 10;
			var skippedActualTypes = applyTypeMapWorkaround
				? new HashSet<TypeDefinition> (App.StaticRegistrar.SkippedTypes.Select (v => v.Actual.Type.Resolve ()))
				: new HashSet<TypeDefinition> ();

			var copyAssemblyParametersFrom = abr.PlatformAssembly.MainModule;
			var assemblyParameters = new ModuleParameters {
				Kind = copyAssemblyParametersFrom.Kind,
				Runtime = copyAssemblyParametersFrom.Runtime,
				Architecture = copyAssemblyParametersFrom.Architecture,
				AssemblyResolver = copyAssemblyParametersFrom.AssemblyResolver,
				MetadataResolver = copyAssemblyParametersFrom.MetadataResolver,
			};

			var rootTypeMapAssembly = CreateTypeMapRootAssembly (assemblyParameters, typesByAssembly.Select (v => v.Key));

			var categoryMethodsByType = App.StaticRegistrar.Types
				.Where (v => v.Value.IsCategory)
				.SelectMany (v => v.Value.Methods!.Select (m => (Type: v.Value.BaseType!.Type, Method: m)))
				.GroupBy (v => v.Type)
				.ToDictionary (v => v.Key, v => v.Select (m => m.Method).ToList ());

			var trampolinesByMethod = Configuration.AssemblyTrampolineInfos
				.SelectMany (v => v.Value.Select (t => (Assembly: v.Key, TrampolineInfo: t)))
				.ToDictionary (v => v.TrampolineInfo.Target, v => v.TrampolineInfo);

			var trampolinesByType = Configuration.AssemblyTrampolineInfos
				.SelectMany (v => v.Value.Select (t => (Assembly: v.Key, TrampolineInfo: t)))
				.GroupBy (v => v.TrampolineInfo.Target.DeclaringType)
				.ToDictionary (v => v.Key, v => v.Select (t => t.TrampolineInfo).ToList ());


			// If we need to modify an assembly that's not the typemap assembly, do it after we've finished writing the typemap assembly,
			// to avoid having to switch between assemblies (we cache a lot of stuff, and those caches will have to be re-created for every switch).
			var postActionsByAssembly = new Dictionary<AssemblyDefinition, List<Action<AssemblyDefinition>>> ();

			// The fix for https://github.com/dotnet/runtime/issues/127004 is only available in .NET 11+, so we
			// still need the workaround (adding the proxy type's attribute to the source type) for .NET 10.
			// Tracking issue: https://github.com/dotnet/macios/issues/25276
			var needsTypeMapWorkaround = App.TargetFramework.Version.Major <= 10;
			void addPostAction (AssemblyDefinition assembly, Action<AssemblyDefinition> action)
			{
				if (!postActionsByAssembly.TryGetValue (assembly, out var actions)) {
					actions = new List<Action<AssemblyDefinition>> ();
					postActionsByAssembly.Add (assembly, actions);
				}
				actions.Add (action);
			}

			foreach (var typesInAssembly in typesByAssembly.OrderBy (v => v.Key.FullName)) {
				var assembly = typesInAssembly.Key;
				var types = typesInAssembly.ToList ();

				// Get the companion assembly (it may already have been created by ManagedRegistrarStep,
				// which emits the registrar trampolines into it when HotReloadCompatibleBuild is enabled).
				var companion = RegistrarCompanionAssembly.GetOrCreate (Configuration, assembly);
				var typeMapAssembly = companion.Assembly;
				var typeMapAssemblyPath = companion.Path;
				addedAssemblies.Add ((typeMapAssemblyPath, typeMapAssembly, assembly.MainModule.FileName));

				var accessesAssemblies = companion.AccessesAssemblies;
				accessesAssemblies.Add (assembly);

				abr.SetCurrentAssembly (typeMapAssembly);

				MarkAssemblyAsTrimmable (typeMapAssembly);

				/*
				 * [assembly: IgnoresAccessChecksTo ("...")] (the attribute type is created by RegistrarCompanionAssembly.GetOrCreate)
				 */
				var ignoredAccessChecksCtor = companion.IgnoresAccessChecksToCtor;

				// INativeObject subclasses
				var inativeObjectTypes = StaticRegistrar.GetAllTypes (assembly).Where (t => !t.IsInterface && !t.IsAbstract && t.IsNativeObject ());
				foreach (var tr in inativeObjectTypes.OrderBy (v => v.FullName)) {
					var inativeObjCtor = AppBundleRewriter.FindINativeObjectConstructor (tr);
					if (inativeObjCtor is null)
						continue;

					var trImported = typeMapAssembly.MainModule.ImportReference (tr);
					var trNamespace = GetNamespace (tr);

					/*
					* [..._Proxy]
					* sealed class ..._Proxy : INativeObjectProxyAttribute {
					* }
					*/
					var proxyType = new TypeDefinition (trNamespace, tr.Name + "_Proxy", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, abr.ObjCRuntime_INativeObjectProxyAttribute);
					typeMapAssembly.MainModule.Types.Add (proxyType);

					/* default ctor */
					var ctor = proxyType.AddMethod (".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, abr.System_Void);
					il = ctor.Body.GetILProcessor ();
					il.Append (il.Create (OpCodes.Ldarg_0));
					il.Append (il.Create (OpCodes.Call, abr.ObjCRuntime_INativeObjectProxyAttribute__ctor));
					il.Append (il.Create (OpCodes.Ret));

					/*
					* public override INativeObject? CreateObject (IntPtr handle, bool owns)
					* {
					*     return new ... (handle, owns);
					* }	
					*/
					var createObjectMethod = proxyType.AddMethod ("CreateObject", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
					createObjectMethod.AddParameter (abr.System_IntPtr); // handle
					createObjectMethod.AddParameter (abr.System_Boolean); // owns
					il = createObjectMethod.Body.GetILProcessor ();
					if (tr.HasGenericParameters) {
						EmitThrowCannotConstructGenericType (il, tr);
					} else {
						il.Append (il.Create (OpCodes.Ldarg_1));
						if (inativeObjCtor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
							il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle));
						il.Append (il.Create (OpCodes.Ldarg_2));
						il.Append (il.Create (OpCodes.Newobj, abr.CurrentAssembly.MainModule.ImportReference (inativeObjCtor)));
						il.Append (il.Create (OpCodes.Ret));
					}

					// We add the proxy type as an attribute to itself
					attribute = abr.CreateAttribute (ctor);
					proxyType.CustomAttributes.Add (attribute);

					/*
					 * Add the [TypeMapAssociation] attribute for the protocol wrapper type as well
					 *
					 * [assembly: TypeMapAssociation<INativeObject> (typeof (...), typeof (...))]
					 */
					attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssociationAttribute_1_Constructor_Type_Type, abr.ObjCRuntime_INativeObject));
					attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
					attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, proxyType));
					typeMapAssembly.CustomAttributes.Add (attribute);
				}

				foreach (var kvp in typesInAssembly.OrderBy (v => v.Key.FullName)) {
					var tr = kvp.Key;
					var trNamespace = GetNamespace (tr);
					var trImported = typeMapAssembly.MainModule.ImportReference (tr);
					var td = tr.Resolve ();
					var objcType = kvp.Value;
					var objcClassName = objcType.ExportedName;
					var isCustomType = App.StaticRegistrar.IsCustomType (objcType);

					if (!objcType.IsProtocol && !objcType.IsCategory) {
						if (applyTypeMapWorkaround && skippedActualTypes.Contains (td)) {
							// Workaround for https://github.com/dotnet/runtime/issues/127504
							// Tracking issue: https://github.com/dotnet/macios/issues/25275
							// Use the 2-arg (unconditional) TypeMap constructor for types that are
							// the target of a SkippedObjectiveCTypeUniverse association, because
							// their conditional (3-arg) entries would be incorrectly trimmed.
							// See the comment where skippedActualTypes is created for full details.
							attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAttribute_1_Constructor_String_Type, abr.Foundation_NSObject));
							attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, objcClassName));
							attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
						} else {
							/*
							 * [assembly: TypeMap<NSObject> ("Objective-C class name", typeof (...), typeof (...))]
							 */
							attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAttribute_1_Constructor_String_Type_Type, abr.Foundation_NSObject));
							attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, objcClassName));
							attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
							attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
						}
						typeMapAssembly.CustomAttributes.Add (attribute);

						/*
						 * [..._Proxy]
						 * sealed class ..._Proxy : NSObjectProxy {
						 * }
						 */
						var proxyType = new TypeDefinition (trNamespace, tr.Name + "_Proxy", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, abr.ObjCRuntime_NSObjectProxyAttribute);
						typeMapAssembly.MainModule.Types.Add (proxyType);

						/* default ctor */
						var ctor = proxyType.AddMethod (".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, abr.System_Void);
						il = ctor.Body.GetILProcessor ();
						il.Append (il.Create (OpCodes.Ldarg_0));
						il.Append (il.Create (OpCodes.Call, abr.ObjCRuntime_NSObjectProxy__ctor));
						il.Append (il.Create (OpCodes.Ret));

						/*
						 * public override NSObject? CreateObject (IntPtr handle)
						 * {
						 *     return new ... (handle);
						 * }	
						 */
						var createObjectMethod = proxyType.AddMethod ("CreateObject", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.Foundation_NSObject);
						createObjectMethod.AddParameter (abr.System_IntPtr); // handle
						il = createObjectMethod.Body.GetILProcessor ();
						if (td.HasGenericParameters) {
							EmitThrowCannotConstructGenericType (il, td);
						} else {
							var nativeHandleCtor = AppBundleRewriter.FindNSObjectConstructor (td);
							if (nativeHandleCtor is not null) {
								il.Append (il.Create (OpCodes.Ldarg_1));
								if (nativeHandleCtor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
									il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle));
								il.Append (il.Create (OpCodes.Newobj, abr.CurrentAssembly.MainModule.ImportReference (nativeHandleCtor)));
							} else {
								il.Append (il.Create (OpCodes.Ldnull));
							}
							il.Append (il.Create (OpCodes.Ret));
						}

						/*
						 * public override IntPtr GetClassHandle (out bool is_custom_type)
						 * {
						 * 	   is_custom_type = ...;
						 * 	   return Class.GetHandle ("...");
						 * }
						 */
						var getClassHandleMethod = proxyType.AddMethod ("GetClassHandle", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.System_IntPtr);
						getClassHandleMethod.AddParameter (abr.System_Boolean.MakeByReferenceType ()); // is_custom_type
						il = getClassHandleMethod.Body.GetILProcessor ();
						il.Append (il.Create (OpCodes.Ldarg_1));
						il.Append (il.Create (isCustomType ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0));
						il.Append (il.Create (OpCodes.Stind_I1));
						il.Append (il.Create (OpCodes.Ldstr, objcClassName));
						il.Append (il.Create (OpCodes.Call, abr.Class_GetHandle__System_String));
						il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_IntPtr));
						il.Append (il.Create (OpCodes.Ret));

						/*
						 * public override IntPtr LookupUnmanagedFunction (string name)
						 * {
						 *     if (name == "funcA")
						 *         return &funcA;
						 *     if (name == "funcB")
						 *         return &funcB;
						 *     return IntPtr.Zero;
						 * }
						 *
						 * This method is only emitted if there are any UnmanagedCallersOnly methods to look up,
						 * otherwise the base implementation (which returns IntPtr.Zero) is good enough.
						 */

						// Get all the UnmanagedCallersOnly methods we need to be able to find for the current type, which includes:
						// - methods from the type itself
						// - methods from categories on the type
						var uco = new List<TrampolineInfo> ();
						if (categoryMethodsByType.Remove (td, out var categoryMethods)) {
							foreach (var m in categoryMethods.OrderBy (v => v.FullName)) {
								if (!trampolinesByMethod.Remove (m.Method!, out var info)) {
									AddException (ErrorHelper.CreateWarning (4191, Errors.MX4191 /* Could not find the trampoline for the category method {0}. */, m.Method?.FullName));
									continue;
								}
								trampolinesByType.Remove (m.CategoryType!.Type.Resolve ());
								uco.Add (info);
								accessesAssemblies.Add (info.Trampoline.Module.Assembly);
							}
						}
						if (trampolinesByType.Remove (td, out var trampolines)) {
							uco.AddRange (trampolines);
							foreach (var info in trampolines) {
								trampolinesByMethod.Remove (info.Target);
							}
						}

						var ucos = uco.OrderBy (v => v.UnmanagedCallersOnlyEntryPoint).ToList ();
						if (ucos.Count > 0) {
							var lookupUnmanagedFunctionMethod = proxyType.AddMethod ("LookupUnmanagedFunction", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.System_IntPtr);
							lookupUnmanagedFunctionMethod.AddParameter (abr.System_String); // name
							il = lookupUnmanagedFunctionMethod.Body.GetILProcessor ();

							var ldcI4 = il.Create (OpCodes.Ldc_I4_0);
							for (var i = 0; i < ucos.Count; i++) {
								var info = ucos [i];
								var isLast = i == ucos.Count - 1;
								var falseTarget = isLast ? ldcI4 : il.Create (OpCodes.Nop);
								il.Append (il.Create (OpCodes.Ldarg_1));
								il.Append (il.Create (OpCodes.Ldstr, info.UnmanagedCallersOnlyEntryPoint));
								il.Append (il.Create (OpCodes.Call, abr.System_String__op_Equality_String_String));
								il.Append (il.Create (OpCodes.Brfalse_S, falseTarget));
								//     return &Method;
								il.Append (il.Create (OpCodes.Ldftn, abr.CurrentAssembly.MainModule.ImportReference (info.Trampoline)));
								il.Append (il.Create (OpCodes.Ret));
								if (!isLast)
									il.Append (falseTarget);
							}
							// return IntPtr.Zero
							il.Append (ldcI4);
							il.Append (il.Create (OpCodes.Conv_I));
							il.Append (il.Create (OpCodes.Ret));
						}

						// We add the proxy type as an attribute to itself
						attribute = abr.CreateAttribute (ctor);
						proxyType.CustomAttributes.Add (attribute);

						// We also add the proxy type as an attribute to the type, as a workaround for https://github.com/dotnet/runtime/issues/127004
						// Tracking issue: https://github.com/dotnet/macios/issues/25276
						if (needsTypeMapWorkaround) {
							addPostAction (td.Module.Assembly, assembly => {
								var attribute = new CustomAttribute (assembly.MainModule.ImportReference (ctor)); // don't use abr.CreateAttribute here, because the ctor has already been marked
								td.CustomAttributes.Add (attribute);
							});
						}

						/*
						 * Add the [TypeMapAssociation] attribute for this type and its proxy
						 *
						 * [assembly: TypeMapAssociation<NSObject> (typeof (...), typeof (...))]
						 */
						attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssociationAttribute_1_Constructor_Type_Type, abr.Foundation_NSObject));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, proxyType));
						typeMapAssembly.CustomAttributes.Add (attribute);
					}

					if (objcType.IsProtocol && objcType.ProtocolWrapperType is not null) {
						/*
						 * [..._Proxy]
						 * sealed class ..._Proxy : ProtocolProxyAttribute {
						 * }
						 */
						var proxyType = new TypeDefinition (trNamespace, tr.Name + "_Proxy", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, abr.ObjCRuntime_ProtocolProxyAttribute);
						typeMapAssembly.MainModule.Types.Add (proxyType);

						/* default ctor */
						var ctor = proxyType.AddMethod (".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, abr.System_Void);
						il = ctor.Body.GetILProcessor ();
						il.Append (il.Create (OpCodes.Ldarg_0));
						il.Append (il.Create (OpCodes.Call, abr.ObjCRuntime_ProtocolProxy__ctor));
						il.Append (il.Create (OpCodes.Ret));

						/*
						 * public override INativeObject? CreateObject (IntPtr handle, bool owns)
						 * {
						 *     return new ... (handle, owns);
						 * }	
						 */
						var createObjectMethod = proxyType.AddMethod ("CreateObject", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.ObjCRuntime_INativeObject);
						createObjectMethod.AddParameter (abr.System_IntPtr); // handle
						createObjectMethod.AddParameter (abr.System_Boolean); // owns
						createObjectMethod.CreateBody (out il);
						var protocolWrapperType = objcType.ProtocolWrapperType.Resolve ();
						if (protocolWrapperType.HasGenericParameters) {
							EmitThrowCannotConstructGenericType (il, protocolWrapperType);
						} else {
							var nativeHandleCtor = AppBundleRewriter.FindINativeObjectConstructor (protocolWrapperType);
							if (nativeHandleCtor is not null) {
								il.Append (il.Create (OpCodes.Ldarg_1));
								if (nativeHandleCtor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
									il.Append (il.Create (OpCodes.Call, abr.NativeObject_op_Implicit_NativeHandle));
								il.Append (il.Create (OpCodes.Ldarg_2));
								il.Append (il.Create (OpCodes.Newobj, abr.CurrentAssembly.MainModule.ImportReference (nativeHandleCtor)));
							} else {
								il.Append (il.Create (OpCodes.Ldnull));
							}
							il.Append (il.Create (OpCodes.Ret));
						}

						/*
						 * public override string GetName ()
						 * {
						 * 	   return "";
						 * }
						 */
						var getProtocolNameMethod = new MethodDefinition ("GetProtocolName", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, abr.System_String);
						il = getProtocolNameMethod.Body.GetILProcessor ();
						il.Append (il.Create (OpCodes.Ldstr, objcType.ProtocolName));
						il.Append (il.Create (OpCodes.Ret));
						proxyType.Methods.Add (getProtocolNameMethod);

						// We add the proxy type as an attribute to itself
						attribute = abr.CreateAttribute (ctor);
						proxyType.CustomAttributes.Add (attribute);

						/*
						 * Add the [TypeMapAssociation] attribute for the protocol wrapper type as well
						 *
						 * [assembly: TypeMapAssociation<ProtocolProxyAttribute> (typeof (...), typeof (...))]
						 */
						attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssociationAttribute_1_Constructor_Type_Type, abr.ObjCRuntime_ProtocolProxyAttribute));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, proxyType));
						typeMapAssembly.CustomAttributes.Add (attribute);

						/*
						 * Add the [TypeMapAssociation] attribute for the protocol wrapper type as well
						 *
						 * [assembly: TypeMapAssociation<ProtocolAttribute> (typeof (...), typeof (...))]
						 */
						attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssociationAttribute_1_Constructor_Type_Type, abr.Foundation_ProtocolAttribute));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, trImported));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, abr.CurrentAssembly.MainModule.ImportReference (objcType.ProtocolWrapperType)));
						typeMapAssembly.CustomAttributes.Add (attribute);

						// We also add the proxy type as an attribute to the type, as a workaround for https://github.com/dotnet/runtime/issues/127004
						// Tracking issue: https://github.com/dotnet/macios/issues/25276
						if (needsTypeMapWorkaround) {
							addPostAction (td.Module.Assembly, assembly => {
								var attribute = new CustomAttribute (assembly.MainModule.ImportReference (ctor)); // don't use abr.CreateAttribute here, because the ctor has already been marked
								td.CustomAttributes.Add (attribute);
							});
						}
					}
				}

				foreach (var accessesAssembly in accessesAssemblies.OrderBy (v => v.FullName)) {
					var attrib = abr.CreateAttribute (ignoredAccessChecksCtor);
					attrib.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_String, accessesAssembly.Name.Name));
					typeMapAssembly.CustomAttributes.Add (attrib);
				}

				if (skippedTypesByAssembly.Remove (assembly, out var skippedTypes)) {
					foreach (var skipped in skippedTypes.OrderBy (v => v.Skipped.FullName)) {
						/*
						 * [assembly: TypeMapAssociation<SkippedObjectiveCTypeUniverse> (typeof (...), typeof (...))]
						 */
						attribute = abr.CreateAttribute (CreateMethodReference (abr.TypeMapAssociationAttribute_1_Constructor_Type_Type, abr.ObjCRuntime_SkippedObjectiveCTypeUniverse));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, typeMapAssembly.MainModule.ImportReference (skipped.Skipped)));
						attribute.ConstructorArguments.Add (new CustomAttributeArgument (abr.System_Type, typeMapAssembly.MainModule.ImportReference (skipped.Actual.Type)));
						typeMapAssembly.CustomAttributes.Add (attribute);
					}
				}

				EmitTypeReferencesForTypeMaps (typeMapAssembly);

				abr.ClearCurrentAssembly ();

				WriteDeterministically (typeMapAssembly, typeMapAssemblyPath);
			}

			foreach (var kvp in postActionsByAssembly) {
				var assembly = kvp.Key;
				var actions = kvp.Value;
				abr.SetCurrentAssembly (assembly);
				foreach (var action in actions) {
					action (assembly);
				}
				abr.ClearCurrentAssembly ();
			}

#if ASSEMBLY_PREPARER
			Configuration.AddedAssemblies.AddRange (addedAssemblies);
#else
			// Since we're running inside the trimmer, we need to make sure the trimmer knows about the assemblies we've created.
			// This will go away once we're running outside of the trimmer.
			var managedAssemblyToLinkItems = new List<MSBuildItem> ();
			var resolver = abr.PlatformAssembly.MainModule.AssemblyResolver;
			var getAssembly = resolver.GetType ().GetMethod ("GetAssembly", new Type [] { typeof (string) })!;
			var cacheAssembly = resolver.GetType ().GetMethod ("CacheAssembly", new Type [] { typeof (AssemblyDefinition) })!;
			foreach (var aa in addedAssemblies) {
				var asm = aa.Assembly;
				var fn = Path.Combine (App.TypeMapOutputDirectory, asm.Name.Name + ".dll");
				var asmDef = (AssemblyDefinition) getAssembly.Invoke (resolver, [fn])!;
				cacheAssembly.Invoke (resolver, [asmDef]);
				var action = Annotations.GetAction (asm);
				Annotations.SetAction (asmDef, action);

				var linkedPath = Path.Combine (Configuration.IntermediateLinkDir, asm.Name.Name + ".dll");
				managedAssemblyToLinkItems.Add (new MSBuildItem (linkedPath, new Dictionary<string, string> {
					{ "TrimMode", "link" },
				}));
			}

			Configuration.WriteOutputForMSBuild ("ManagedAssemblyToLink", managedAssemblyToLinkItems);
#endif

			// Report back any exceptions that occurred during the processing.
			exceptions = this.exceptions;
		}
	}
}
