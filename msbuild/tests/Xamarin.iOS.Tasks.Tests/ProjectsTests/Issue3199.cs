using System;
using System.IO;
using System.Linq;
using System.Collections;

using Microsoft.Build.Evaluation;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class Issue3199 : ProjectTest
	{
		public Issue3199 () : base ("iPhone")
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("Issue3199", "iPhone", "Debug");
		}
	}
}
