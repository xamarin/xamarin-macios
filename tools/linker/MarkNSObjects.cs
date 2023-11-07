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

using System;

using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
#if NET
using Mono.Linker.Steps;
#endif

namespace Xamarin.Linker.Steps {

	public class MarkNSObjects : ExceptionalSubStep {

		static string ProductAssembly;

		protected override string Name { get; } = "MarkNSObjects";
		protected override int ErrorCode { get; } = 2080;

		public override SubStepTargets Targets {
			get { return SubStepTargets.Type; }
		}

		protected override void Process (TypeDefinition type)
		{
			if (ProductAssembly is null)
				ProductAssembly = (Profile.Current as BaseProfile).ProductAssembly;

			bool nsobject = type.IsNSObject (LinkContext);
			if (!nsobject && !type.IsNativeObject ())
				return;

			if (!IsProductType (type)) {
				// we need to annotate the parent type(s) of a nested type
				// otherwise the sweeper will not keep the parents (nor the children)
				if (type.IsNested) {
					var parent = type.DeclaringType;
					while (parent is not null) {
						Annotations.Mark (parent);
						parent = parent.DeclaringType;
					}
				}
				Annotations.Mark (type);
				Annotations.SetPreserve (type, TypePreserve.All);
			} else if (type.HasMethods) {
				PreserveIntPtrConstructor (type);
				if (nsobject)
					PreserveExportedMethods (type);
			}
		}

		void PreserveExportedMethods (TypeDefinition type)
		{
			foreach (var method in type.Methods) {
				if (!IsExportedMethod (method))
					continue;

				// not optimal if "Link all" is used as the override might be removed later
				if (!IsOverridenInUserCode (method))
					continue;

				Annotations.AddPreservedMethod (type, method);
			}
		}

		bool IsOverridenInUserCode (MethodDefinition method)
		{
			if (!method.IsVirtual)
				return false;

			var overrides = Annotations.GetOverrides (method);
			if (overrides is null)
				return false;

			foreach (var @override in overrides)
				if (!IsProductMethod (@override.Override))
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

		void PreserveIntPtrConstructor (TypeDefinition type)
		{
			foreach (MethodDefinition constructor in type.GetConstructors ()) {
				if (!constructor.HasParameters)
					continue;

#if NET
				if (constructor.Parameters.Count != 1 || !constructor.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
#else
				if (constructor.Parameters.Count != 1 || !constructor.Parameters [0].ParameterType.Is ("System", "IntPtr"))
#endif
					continue;

				Annotations.AddPreservedMethod (type, constructor);
				break; // only one .ctor can match this
			}
		}

		static bool IsProductMethod (MethodDefinition method)
		{
			return (method.DeclaringType.Module.Assembly.Name.Name == ProductAssembly);
		}

		bool IsProductType (TypeDefinition type)
		{
			if (LinkContext.App.SkipMarkingNSObjectsInUserAssemblies)
				return true;

			var name = type.Module.Assembly.Name.Name;
			switch (name) {
			case "Xamarin.Forms.Platform.iOS":
				return true;
			case "Xamarin.iOS":
				// for Catalyst this has extra stubs and must be considered has _product_ to remove extra binding code
				return true;
			default:
				return name == ProductAssembly;
			}
		}
	}
}
