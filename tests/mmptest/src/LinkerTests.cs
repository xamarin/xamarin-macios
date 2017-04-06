using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	public partial class MMPTests
	{
		[Test]
		public void UnifiedLinkingSDK_WithAllNonProductSkipped_Builds ()
		{
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { CSProjConfig = "<LinkMode>SdkOnly</LinkMode><MonoBundlingExtraArgs>--linkskip=mscorlib.dll --linkskip=System.Core.dll --linkskip=System.dll</MonoBundlingExtraArgs>" };
				TI.TestUnifiedExecutable (test);
			});
		}
	}
}
