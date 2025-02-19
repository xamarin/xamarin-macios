// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Microsoft.Macios.Generator.Attributes;
using ObjCRuntime;

namespace Microsoft.Macios.Generator.IO;

partial class TabbedStringBuilder {

	public TabbedStringBuilder AppendExportData<T> (in ExportData<T> exportData) where T : Enum
	{
		// Try to write the smaller amount of data. We are using the old ExportAttribute until we make the final move
		if (exportData.ArgumentSemantic != ArgumentSemantic.None) {
			WriteLine ($"[Export (\"{exportData.Selector}\", ArgumentSemantic.{exportData.ArgumentSemantic})]");
		} else {
			WriteLine ($"[Export (\"{exportData.Selector}\")]");
		}
		return this;
	}
}
