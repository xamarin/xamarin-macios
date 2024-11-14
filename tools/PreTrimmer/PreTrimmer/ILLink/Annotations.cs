// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//
// Annotations.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2007 Novell, Inc.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.Linker {
	public class AnnotationStore {
		protected readonly LinkContext context;

		protected readonly Dictionary<AssemblyDefinition, AssemblyAction> assembly_actions = new Dictionary<AssemblyDefinition, AssemblyAction> ();
		internal readonly HashSet<IMetadataTokenProvider> marked = new HashSet<IMetadataTokenProvider> ();
		internal readonly Dictionary<TypeDefinition, TypePreserve> preserved_types = new Dictionary<TypeDefinition, TypePreserve> ();
		internal readonly Dictionary<IMemberDefinition, List<IMemberDefinition>> preserved_members = new Dictionary<IMemberDefinition, List<IMemberDefinition>> ();
		protected readonly Dictionary<AssemblyDefinition, ISymbolReader> symbol_readers = new Dictionary<AssemblyDefinition, ISymbolReader> ();
		readonly Dictionary<object, Dictionary<IMetadataTokenProvider, object>> custom_annotations = new Dictionary<object, Dictionary<IMetadataTokenProvider, object>> ();
		protected readonly HashSet<CustomAttribute> marked_attributes = new HashSet<CustomAttribute> ();

		internal AnnotationStore (LinkContext context)
		{
			this.context = context;
			TypeMapInfo = new TypeMapInfo (context);
			SubstitutionInfo = new ();
		}

		internal bool ProcessSatelliteAssemblies { get; set; } = true;

		TypeMapInfo TypeMapInfo { get; }

		SubstitutionInfo SubstitutionInfo { get; }

		public AssemblyAction GetAction (AssemblyDefinition assembly)
		{
			if (assembly_actions.TryGetValue (assembly, out AssemblyAction action))
				return action;

			throw new InvalidOperationException ($"No action for the assembly {assembly.Name} defined");
		}

		public void SetAction (AssemblyDefinition assembly, AssemblyAction action)
		{
			assembly_actions [assembly] = action;
		}

		public bool HasAction (AssemblyDefinition assembly)
		{
			return assembly_actions.ContainsKey (assembly);
		}

		public void SetAction (MethodDefinition method, MethodAction action)
		{
			SubstitutionInfo.SetMethodAction (method, action);
		}

		public void SetStubValue (MethodDefinition method, object value)
		{
			SubstitutionInfo.SetMethodStubValue (method, value);
		}

		public void Mark (IMetadataTokenProvider provider)
		{
			marked.Add (provider);
		}

		public void Mark (CustomAttribute attribute)
		{
			marked_attributes.Add (attribute);
		}

		public bool IsMarked (IMetadataTokenProvider provider)
		{
			return marked.Contains (provider);
		}

		public bool IsMarked (CustomAttribute attribute)
		{
			return marked_attributes.Contains (attribute);
		}

		public void SetPreserve (TypeDefinition type, TypePreserve preserve)
		{
			Debug.Assert (preserve != TypePreserve.Nothing);
			if (!preserved_types.TryGetValue (type, out TypePreserve existing)) {
				preserved_types.Add (type, preserve);
				return;
			}
			Debug.Assert (existing != TypePreserve.Nothing);
			var newPreserve = ChoosePreserveActionWhichPreservesTheMost (existing, preserve);
			if (newPreserve != existing) {
				preserved_types [type] = newPreserve;
			}
		}

		static TypePreserve ChoosePreserveActionWhichPreservesTheMost (TypePreserve leftPreserveAction, TypePreserve rightPreserveAction)
		{
			if (leftPreserveAction == rightPreserveAction)
				return leftPreserveAction;

			if (leftPreserveAction == TypePreserve.All || rightPreserveAction == TypePreserve.All)
				return TypePreserve.All;

			if (leftPreserveAction == TypePreserve.Nothing)
				return rightPreserveAction;

			if (rightPreserveAction == TypePreserve.Nothing)
				return leftPreserveAction;

			if ((leftPreserveAction == TypePreserve.Methods && rightPreserveAction == TypePreserve.Fields) ||
				(leftPreserveAction == TypePreserve.Fields && rightPreserveAction == TypePreserve.Methods))
				return TypePreserve.All;

			return rightPreserveAction;
		}

		/// <summary>
		/// Returns a list of all known methods that override <paramref name="method"/>.
		/// The list may be incomplete if other overrides exist in assemblies that haven't been processed by TypeMapInfo yet
		/// </summary>
		public IEnumerable<OverrideInformation>? GetOverrides (MethodDefinition method)
		{
			return TypeMapInfo.GetOverrides (method);
		}

		public void AddPreservedMethod (TypeDefinition type, MethodDefinition method)
		{
			AddPreservedMember (type, method);
		}

		public void AddPreservedField (TypeDefinition type, FieldDefinition field)
		{
			AddPreservedMember (type, field);
		}

		void AddPreservedMethod (MethodDefinition key, MethodDefinition method)
		{
			throw new NotImplementedException ();
			// AddPreservedMethod (key as IMemberDefinition, method);
		}

		List<IMemberDefinition>? GetPreservedMethods (IMemberDefinition definition)
		{
			if (preserved_members.TryGetValue (definition, out List<IMemberDefinition>? preserved))
				return preserved;

			return null;
		}

		void AddPreservedMember (TypeDefinition definition, IMemberDefinition method)
		{
			if (!preserved_members.TryGetValue (definition, out List<IMemberDefinition>? members)) {
				members = new List<IMemberDefinition> ();
				preserved_members [definition] = members;
			}

			members.Add (method);
		}

		internal void AddSymbolReader (AssemblyDefinition assembly, ISymbolReader symbolReader)
		{
			symbol_readers [assembly] = symbolReader;
		}
		public void CloseSymbolReader (AssemblyDefinition assembly)
		{
			if (!symbol_readers.TryGetValue (assembly, out ISymbolReader? symbolReader))
				return;

			symbol_readers.Remove (assembly);
			symbolReader.Dispose ();
		}

		public object? GetCustomAnnotation (object key, IMetadataTokenProvider item)
		{
			if (!custom_annotations.TryGetValue (key, out Dictionary<IMetadataTokenProvider, object>? slots))
				return null;

			if (!slots.TryGetValue (item, out object? value))
				return null;

			return value;
		}

		public void SetCustomAnnotation (object key, IMetadataTokenProvider item, object value)
		{
			if (!custom_annotations.TryGetValue (key, out Dictionary<IMetadataTokenProvider, object>? slots)) {
				slots = new Dictionary<IMetadataTokenProvider, object> ();
				custom_annotations.Add (key, slots);
			}

			slots [item] = value;
		}
	}
}
