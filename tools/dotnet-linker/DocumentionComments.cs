using System;
using System.Collections.Generic;
using System.Text;

using Mono.Cecil;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Utils {
	// signature format: https://github.com/dotnet/csharpstandard/blob/standard-v6/standard/documentation-comments.md#d42-id-string-format
	public static class DocumentationComments {
		public static string GetSignature (IMetadataTokenProvider member)
		{
			if (member is FieldDefinition fd)
				return GetSignature (fd);

			if (member is MethodDefinition md)
				return GetSignature (md);

			if (member is TypeDefinition td)
				return GetSignature (td);

			throw ErrorHelper.CreateError (99, $"Unable to get the doc signature for {member.GetType ().FullName}");
		}

		public static string GetSignature (TypeDefinition type)
		{
			if (type.IsNested)
				return type.FullName.Replace ('/', '.');
			return type.FullName;
		}

		public static string GetSignature (FieldDefinition field)
		{
			return field.Name.Replace ('.', '#');
		}

		public static string GetSignature (MethodDefinition method)
		{
			var sb = new StringBuilder ();
			sb.Append (method.Name.Replace ('.', '#'));

			if (method.HasGenericParameters) {
				sb.Append ($"``{method.GenericParameters.Count}");
			}

			sb.Append ('(');
			for (var i = 0; i < method.Parameters.Count; i++) {
				if (i > 0)
					sb.Append (',');

				var parameterType = method.Parameters [i].ParameterType;
				WriteTypeSignature (sb, parameterType);
			}
			sb.Append (')');

			return sb.ToString ();
		}

		static void WriteTypeSignature (StringBuilder sb, TypeReference type)
		{
			if (type is ByReferenceType brt) {
				WriteTypeSignature (sb, brt.GetElementType ());
				sb.Append ('@');
				return;
			}

			if (type is ArrayType at) {
				WriteTypeSignature (sb, at.GetElementType ());
				sb.Append ("[]");
				return;
			}

			if (type is PointerType pt) {
				WriteTypeSignature (sb, pt.GetElementType ());
				sb.Append ('*');
				return;
			}

			if (type is GenericParameter gp) {
				if (gp.Type == GenericParameterType.Type) {
					sb.Append ('`');
				} else {
					sb.Append ("``");
				}

				sb.Append (gp.Position.ToString ());
				return;
			}

			sb.Append (type.FullName.Replace ('/', '.'));
		}
	}
}
