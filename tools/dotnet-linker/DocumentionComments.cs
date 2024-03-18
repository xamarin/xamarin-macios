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
			var docCommentId = Mono.Cecil.Rocks.DocCommentId.GetDocCommentId (method);

			// the `docCommentId` is in the form of `M:<TypeName>.<NestedTypeName>.<MethodName>(...)`
			// and we need to skip the "M:" prefix and the type name(s) and keep just the method name
			// and the argument list for the signature

			var methodNameEnd = docCommentId.IndexOf ('(');
			if (methodNameEnd == -1) {
				// in some cases there aren't any brackets so the end of the doc comment is the end of the method name
				methodNameEnd = docCommentId.Length - 1;
			}

			var methodNameStart = 1 + docCommentId.LastIndexOf ('.', startIndex: methodNameEnd);

			return docCommentId.Substring (methodNameStart);
		}
	}
}
