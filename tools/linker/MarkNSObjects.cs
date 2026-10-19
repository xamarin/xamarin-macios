//
// MarkNSObjects.cs
//
// Authors:
//	Jb Evain (jbevain@novell.com)
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// (C) 2009 Novell, Inc.
// Copyright (C) 2011-2014 Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using Mono.Linker.Steps;

using Xamarin.Bundler;
using Xamarin.Tuner;

#nullable enable

namespace Xamarin.Linker.Steps {
#if !ASSEMBLY_PREPARER
	public class MarkNSObjects : ExceptionalSubStep, IMarkNSObjects {
		protected override string Name { get; } = "MarkNSObjects";
		protected override int ErrorCode { get; } = 2080;

		public override SubStepTargets Targets {
			get { return SubStepTargets.Type; }
		}

		AnnotationStore IMarkNSObjects.Annotations => Annotations;
		public Application App => base.LinkContext.App;
		DerivedLinkContext IMarkNSObjects.Context => Configuration.DerivedLinkContext;

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			if (Configuration.DerivedLinkContext.DidRunMarkNSObjectsStep)
				return false;

			return base.IsActiveFor (assembly);
		}

		protected override void Process (TypeDefinition type)
		{
			MarkNSObjectsImpl.ProcessType (this, type);
		}

		public bool PreserveType (TypeDefinition type, bool allMembers)
		{
			Annotations.Mark (type);
			if (allMembers)
				Annotations.SetPreserve (type, TypePreserve.All);
			return true;
		}

		public bool PreserveType (TypeDefinition onType, TypeDefinition type)
		{
			return PreserveType (type, allMembers: false);
		}

		public bool PreserveMethod (TypeDefinition onType, MethodDefinition method)
		{
			Annotations.AddPreservedMethod (onType, method);
			return true;
		}
	}
#endif

	public interface IMarkNSObjects {
		bool PreserveType (TypeDefinition type, bool allMembers);
		// Preserve 'type' if 'onType' is marked.
		bool PreserveType (TypeDefinition onType, TypeDefinition type);
		// Preserve 'method' if 'onType' is marked.
		bool PreserveMethod (TypeDefinition onType, MethodDefinition method);
		AnnotationStore Annotations { get; }
		Application App { get; }
		DerivedLinkContext Context { get; }
	}

	public class MarkNSObjectsImpl {

		public static bool ProcessType (IMarkNSObjects marker, TypeDefinition type)
		{
			var modified = false;

			bool nsobject = type.IsNSObject (marker.Context);
			if (!nsobject && !type.IsNativeObject ())
				return modified;

			if (!IsProductType (marker, type)) {
				// we need to annotate the parent type(s) of a nested type
				// otherwise the sweeper will not keep the parents (nor the children)
				if (type.IsNested) {
					var parent = type.DeclaringType;
					while (parent is not null) {
						modified |= marker.PreserveType (type, parent);
						parent = parent.DeclaringType;
					}
				}
				modified |= marker.PreserveType (type, allMembers: true);
			} else if (type.HasMethods) {
				modified |= PreserveIntPtrConstructor (marker, type);
				if (nsobject) {
					modified |= PreserveExportedMethods (marker, type);
					if (marker.App.Registrar == RegistrarMode.TrimmableStatic)
						modified |= PreserveVirtualOverrides (marker, type);
				}
			}

			return modified;
		}

		static bool PreserveExportedMethods (IMarkNSObjects marker, TypeDefinition type)
		{
			var modified = false;
			foreach (var method in type.Methods) {
				if (!IsExportedMethod (method))
					continue;

				// not optimal if "Link all" is used as the override might be removed later
				if (!IsOverridenInUserCode (marker, method))
					continue;

				modified |= marker.PreserveMethod (type, method);
			}
			return modified;
		}

		static bool IsOverridenInUserCode (IMarkNSObjects marker, MethodDefinition method)
		{
			if (!method.IsVirtual)
				return false;

			var overrides = marker.Annotations.GetOverrides (method);
			if (overrides is null)
				return false;

			foreach (var @override in overrides)
				if (!IsProductMethod (marker, @override.Override))
					return true;

			return false;
		}

		static bool IsExportedMethod (MethodDefinition method)
		{
			if (!method.HasCustomAttributes)
				return false;

			foreach (CustomAttribute attribute in method.CustomAttributes)
				if (attribute.Constructor.DeclaringType.Inherits (Namespaces.Foundation, "ExportAttribute"))
					return true;

			return false;
		}

		static bool PreserveIntPtrConstructor (IMarkNSObjects marker, TypeDefinition type)
		{
			var modified = false;
			foreach (MethodDefinition constructor in type.GetConstructors ()) {
				if (!constructor.HasParameters)
					continue;

				if (constructor.Parameters.Count != 1 || !constructor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
					continue;

				modified |= marker.PreserveMethod (type, constructor);
				break; // only one .ctor can match this
			}
			return modified;
		}

		static bool PreserveVirtualOverrides (IMarkNSObjects marker, TypeDefinition type)
		{
			// Preserve all virtual method overrides for product NSObject types.
			// When TrimMode=full, the trimmer may remove virtual overrides from
			// product types (like ClassHandle, ToString, Dispose, etc.), causing
			// incorrect base class behavior at runtime. These overrides are called
			// through virtual dispatch and must be preserved.
			// Collect first to avoid modifying the collection while iterating.
			List<MethodDefinition>? overrides = null;
			foreach (var method in type.Methods) {
				if (!method.IsVirtual || method.IsNewSlot || method.IsAbstract)
					continue;
				overrides ??= new List<MethodDefinition> ();
				overrides.Add (method);
			}
			if (overrides is null)
				return false;
			var modified = false;
			foreach (var method in overrides)
				modified |= marker.PreserveMethod (type, method);
			return modified;
		}

		static bool IsProductMethod (IMarkNSObjects marker, MethodDefinition method)
		{
			return method.DeclaringType.Module.Assembly.Name.Name == marker.App.Configuration.PlatformAssembly;
		}

		static bool IsProductType (IMarkNSObjects marker, TypeDefinition type)
		{
			if (marker.App.SkipMarkingNSObjectsInUserAssemblies)
				return true;

			var name = type.Module.Assembly.Name.Name;
			return name == marker.App.Configuration.PlatformAssembly;
		}
	}
}
