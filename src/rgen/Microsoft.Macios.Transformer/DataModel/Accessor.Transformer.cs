// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Macios.Transformer.Attributes;

namespace Microsoft.Macios.Generator.DataModel;

readonly partial struct Accessor {
	
	public ExportData? ExportPropertyData { get; init; }
}
