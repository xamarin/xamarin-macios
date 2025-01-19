// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.Macios.Transformer.Extensions;

static class CSharpCommandLineParserExtensions {

	public static CSharpCommandLineArguments ParseRsp (this CSharpCommandLineParser self, string rspFile, string workingDirectory, string? sdkDirectory)
	{
		var args = new [] { $"@{rspFile}"};
		var parseResult = self.Parse (
			args: args,
			baseDirectory: workingDirectory,
			sdkDirectory: Path.GetDirectoryName (typeof(object).GetTypeInfo ().Assembly.ManifestModule
				.FullyQualifiedName)//sdkDirectory
		);

		if (parseResult.Errors.Length > 0) {
			var sb = new StringBuilder ();
			foreach (var resultError in parseResult.Errors) {
				sb.AppendLine (resultError.ToString ());
			}
			throw new Exception ($"Error parsing the RSP file {sb}");
		}

		return parseResult;
	}
	
}
