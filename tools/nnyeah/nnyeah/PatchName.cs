using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Text;
using Mono.Collections.Generic;

#nullable disable

// This is based upon portions of these files from the MonoMod.Common project:
//   https://github.com/MonoMod/MonoMod.Common/blob/093ef864257b2eb332ef727e389c2679d98c594e/Utils/Extensions.Cecil.cs
//   https://github.com/MonoMod/MonoMod.Common/blob/093ef864257b2eb332ef727e389c2679d98c594e/Utils/Extensions.GetID.cs
//   https://github.com/MonoMod/MonoMod.Common/blob/093ef864257b2eb332ef727e389c2679d98c594e/Utils/Extensions.GetPatchName.cs
// 
// They are under the MIT license:
// The MIT License (MIT)

// Copyright (c) 2019 - 2020 0x0ade

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace MonoMod.Utils {
	static partial class Extensions {

		/// <summary>
		/// Get the "patch name" - the name of the target to patch - for the given member.
		/// </summary>
		/// <param name="mr">The member to get the patch name for.</param>
		/// <returns>The patch name.</returns>
		public static string GetPatchFullName (this MemberReference mr)
		{
			return (mr as ICustomAttributeProvider)?.GetPatchFullName (mr) ?? mr.FullName;
		}

		private static string GetPatchName (this ICustomAttributeProvider cap)
		{
			string name;

			CustomAttribute patchAttrib = cap.GetCustomAttribute ("MonoMod.MonoModPatch");
			if (patchAttrib is not null) {
				name = (string) patchAttrib.ConstructorArguments [0].Value;
				int dotIndex = name.LastIndexOf ('.');
				if (dotIndex != -1 && dotIndex != name.Length - 1) {
					name = name.Substring (dotIndex + 1);
				}
				return name;
			}

			// Backwards-compatibility: Check for patch_
			name = ((MemberReference) cap).Name;
			return name.StartsWith ("patch_", StringComparison.Ordinal) ? name.Substring (6) : name;
		}

		private static string GetPatchFullName (this ICustomAttributeProvider cap, MemberReference mr)
		{
			if (cap is TypeReference type) {
				CustomAttribute patchAttrib = cap.GetCustomAttribute ("MonoMod.MonoModPatch");
				string name;

				if (patchAttrib is not null) {
					name = (string) patchAttrib.ConstructorArguments [0].Value;
				} else {
					// Backwards-compatibility: Check for patch_
					name = ((MemberReference) cap).Name;
					name = name.StartsWith ("patch_", StringComparison.Ordinal) ? name.Substring (6) : name;
				}

				if (name.StartsWith ("global::", StringComparison.Ordinal))
					name = name.Substring (8); // Patch name is refering to a global type.
				else if (name.Contains (".", StringComparison.Ordinal) || name.Contains ("/", StringComparison.Ordinal)) { } // Patch name is already a full name.
				else if (!string.IsNullOrEmpty (type.Namespace))
					name = type.Namespace + "." + name;
				else if (type.IsNested)
					name = type.DeclaringType.GetPatchFullName () + "/" + name;

				if (mr is TypeSpecification) {
					// Collect TypeSpecifications and append formats back to front.
					List<TypeSpecification> formats = new List<TypeSpecification> ();
					TypeSpecification ts = (TypeSpecification) mr;
					do {
						formats.Add (ts);
					} while ((ts = (ts.ElementType as TypeSpecification)) is not null);

					StringBuilder builder = new StringBuilder (name.Length + formats.Count * 4);
					builder.Append (name);
					for (int formati = formats.Count - 1; formati > -1; --formati) {
						ts = formats [formati];

						if (ts.IsByReference)
							builder.Append ("&");
						else if (ts.IsPointer)
							builder.Append ("*");
						else if (ts.IsPinned) { } // FullName not overriden.
						else if (ts.IsSentinel) { } // FullName not overriden.
						else if (ts.IsArray) {
							ArrayType array = (ArrayType) ts;
							if (array.IsVector)
								builder.Append ("[]");
							else {
								builder.Append ("[");
								for (int i = 0; i < array.Dimensions.Count; i++) {
									if (i > 0)
										builder.Append (",");
									builder.Append (array.Dimensions [i].ToString ());
								}
								builder.Append ("]");
							}
						} else if (ts.IsRequiredModifier)
							builder.Append ("modreq(").Append (((RequiredModifierType) ts).ModifierType).Append (")");
						else if (ts.IsOptionalModifier)
							builder.Append ("modopt(").Append (((OptionalModifierType) ts).ModifierType).Append (")");
						else if (ts.IsGenericInstance) {
							GenericInstanceType gen = (GenericInstanceType) ts;
							builder.Append ("<");
							for (int i = 0; i < gen.GenericArguments.Count; i++) {
								if (i > 0)
									builder.Append (",");
								builder.Append (gen.GenericArguments [i].GetPatchFullName ());
							}
							builder.Append (">");
						} else if (ts.IsFunctionPointer) {
							FunctionPointerType fpt = (FunctionPointerType) ts;
							builder.Append (" ").Append (fpt.ReturnType.GetPatchFullName ()).Append (" *(");
							if (fpt.HasParameters)
								for (int i = 0; i < fpt.Parameters.Count; i++) {
									ParameterDefinition parameter = fpt.Parameters [i];
									if (i > 0)
										builder.Append (",");

									if (parameter.ParameterType.IsSentinel)
										builder.Append ("...,");

									builder.Append (parameter.ParameterType.FullName);
								}
							builder.Append (")");
						} else
							throw new NotSupportedException ($"MonoMod can't handle TypeSpecification: {type.FullName} ({type.GetType ()})");
					}

					name = builder.ToString ();
				}

				return name;
			}

			if (cap is FieldReference field) {
				return $"{field.FieldType.GetPatchFullName ()} {field.DeclaringType.GetPatchFullName ()}::{cap.GetPatchName ()}";
			}

			if (cap is MethodReference)
				throw new InvalidOperationException ("GetPatchFullName not supported on MethodReferences - use GetID instead");

			throw new InvalidOperationException ($"GetPatchFullName not supported on type {cap.GetType ()}");
		}

		/// <summary>
		/// Get a reference ID that is similar to the full name, but consistent between System.Reflection and Mono.Cecil.
		/// </summary>
		/// <param name="method">The method to get the ID for.</param>
		/// <param name="name">The name to use instead of the reference's own name.</param>
		/// <param name="type">The ID to use instead of the reference's declaring type ID.</param>
		/// <param name="withType">Whether the type ID should be included or not. System.Reflection avoids it by default.</param>
		/// <param name="simple">Whether the ID should be "simple" (name only).</param>
		/// <returns>The ID.</returns>
		public static string GetID (this MethodReference method, string name = null, string type = null, bool withType = true, bool simple = false)
		{
			StringBuilder builder = new StringBuilder ();

			if (simple) {
				if (withType && (type is not null || method.DeclaringType is not null))
					builder.Append (type ?? method.DeclaringType.GetPatchFullName ()).Append ("::");
				builder.Append (name ?? method.Name);
				return builder.ToString ();
			}

			builder
				.Append (method.ReturnType.GetPatchFullName ())
				.Append (" ");

			if (withType && (type is not null || method.DeclaringType is not null))
				builder.Append (type ?? method.DeclaringType.GetPatchFullName ()).Append ("::");

			builder
				.Append (name ?? method.Name);

			if (method is GenericInstanceMethod gim && gim.GenericArguments.Count != 0) {
				builder.Append ("<");
				Collection<TypeReference> arguments = gim.GenericArguments;
				for (int i = 0; i < arguments.Count; i++) {
					if (i > 0)
						builder.Append (",");
					builder.Append (arguments [i].GetPatchFullName ());
				}
				builder.Append (">");

			} else if (method.GenericParameters.Count != 0) {
				builder.Append ("<");
				Collection<GenericParameter> arguments = method.GenericParameters;
				for (int i = 0; i < arguments.Count; i++) {
					if (i > 0)
						builder.Append (",");
					builder.Append (arguments [i].Name);
				}
				builder.Append (">");
			}

			builder.Append ("(");

			if (method.HasParameters) {
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int i = 0; i < parameters.Count; i++) {
					ParameterDefinition parameter = parameters [i];
					if (i > 0)
						builder.Append (",");

					if (parameter.ParameterType.IsSentinel)
						builder.Append ("...,");

					builder.Append (parameter.ParameterType.GetPatchFullName ());
				}
			}

			builder.Append (")");

			return builder.ToString ();
		}

		/// <summary>
		/// Get a certain custom attribute from an attribute provider.
		/// </summary>
		/// <param name="cap">The attribute provider.</param>
		/// <param name="attribute">The custom attribute name.</param>
		/// <returns>The first matching custom attribute, or null if no matching attribute has been found.</returns>
		public static CustomAttribute GetCustomAttribute (this ICustomAttributeProvider cap, string attribute)
		{
			if (cap is null || !cap.HasCustomAttributes)
				return null;
			foreach (CustomAttribute attrib in cap.CustomAttributes)
				if (attrib.AttributeType.FullName == attribute)
					return attrib;
			return null;
		}
	}
}
