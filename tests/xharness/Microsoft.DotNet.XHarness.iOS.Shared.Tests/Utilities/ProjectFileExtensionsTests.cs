using System;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Tests.Utilities {

	[TestFixture]
	public class ProjectFileExtensionsTests {

		XmlDocument CreateDoc (string xml)
		{
			var doc = new XmlDocument ();
			doc.LoadXmlWithoutNetworkAccess (xml);
			return doc;
		}

		XmlDocument GetMSBuildProject (string snippet)
		{
			return CreateDoc ($@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
{snippet}
</Project>
");
		}

		[Test]
		public void GetInfoPListNode ()
		{
			// Exact Include
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><None Include=\"Info.plist\" /></ItemGroup>")), "None");
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><BundleResource Include=\"Info.plist\" /></ItemGroup>")), "BundleResource");
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Content Include=\"Info.plist\" /></ItemGroup>")), "Content");
			Assert.IsNull(ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Whatever Include=\"Info.plist\" /></ItemGroup>")), "Whatever");

			// With LogicalName
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><None Include=\"doc\"><LogicalName>Info.plist</LogicalName></None></ItemGroup>")), "None 2");
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><BundleResource Include=\"doc\"><LogicalName>Info.plist</LogicalName></BundleResource></ItemGroup>")), "BundleResource 2");
			Assert.IsNotNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Content Include=\"doc\"><LogicalName>Info.plist</LogicalName></Content></ItemGroup>")), "Content 2");
			Assert.IsNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Whatever Include=\"Info.plist\"><LogicalName>Info.plist</LogicalName></Whatever></ItemGroup>")), "Whatever 2");

			// With Link
			Assert.IsNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><None Include=\"doc\"><Link>Info.plist</Link></None></ItemGroup>")), "None 3");
			Assert.IsNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><BundleResource Include=\"doc\"><Link>Info.plist</Link></BundleResource></ItemGroup>")), "BundleResource 3");
			Assert.IsNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Content Include=\"doc\"><Link>Info.plist</Link></Content></ItemGroup>")), "Content 3");
			Assert.IsNull (ProjectFileExtensions.GetInfoPListNode (GetMSBuildProject ("<ItemGroup><Whatever Include=\"Info.plist\"><Link>Info.plist</Link></Whatever></ItemGroup>")), "Whatever 3");
		}
	}
}

