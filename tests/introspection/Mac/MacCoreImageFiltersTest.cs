//
// Test the generated API for all Mac CoreImage filters
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacCoreImageFiltersTest : ApiCoreImageFiltersTest {
		protected override bool Skip (string nativeName)
		{
			switch (nativeName) {
			case "CIEdgePreserveUpsampleFilter": // Appears in 10.13 but not documented
			case "CIMaskedVariableBlur": // Appears removed in 10.11 but not documented
				if (Mac.CheckSystemVersion (10, 11))
					return true;
				break;
			case "CICMYKHalftone": // Renamed as CICmykHalftone
				return true;
			case "CIAreaMinMax":
			case "CICameraCalibrationLensCorrection":
			case "CIDither":
			case "CIGuidedFilter":
			case "CIMeshGenerator":
			case "CIMix":
			case "CISampleNearest":
				if (IntPtr.Size == 4)
					return true;
				break;
			}
			return base.Skip (nativeName);
		}
	}
}
