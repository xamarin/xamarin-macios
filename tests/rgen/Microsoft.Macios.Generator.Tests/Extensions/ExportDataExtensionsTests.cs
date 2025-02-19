// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable APL0003
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Extensions;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class ExportDataExtensionsTests {
	class TestDataGetSelectorName : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// selection of example selectors
			yield return [
				new ExportData<ObjCBindings.Method> ("RTFDFileWrapperFromRange:documentAttributes:"),
				"selRTFDFileWrapperFromRange_DocumentAttributes_XHandle",
				false
			];

			yield return [
				new ExportData<ObjCBindings.Method> ("RTFDFileWrapperFromRange:documentAttributes:"),
				"selRTFDFileWrapperFromRange_DocumentAttributes_",
				true
			];

			yield return [
				new ExportData<ObjCBindings.Method> ("RTFDFromRange:documentAttributes:"),
				"selRTFDFromRange_DocumentAttributes_XHandle",
				false
			];

			yield return [
				new ExportData<ObjCBindings.Method> ("RTFDFromRange:documentAttributes:"),
				"selRTFDFromRange_DocumentAttributes_",
				true
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof (TestDataGetSelectorName))]
	void GetSelectorFieldName (ExportData<ObjCBindings.Method> exportData, string expectedFieldName, bool inline)
		=> Assert.Equal (expectedFieldName, exportData.GetSelectorFieldName (inline));

}
