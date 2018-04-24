using System;
using System.Threading;

#if __UNIFIED__
using Foundation;
using ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

using NUnit.Framework;

using Bindings.Test;

namespace Xamarin.Tests
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RuntimeTest
	{
		[Test]
		public void WrapperTypeLookupTest ()
		{
			using (var assigner = new MyProtocolAssigner ()) {
				assigner.SetProtocol ();
			}
		}

		class MyProtocolAssigner : ProtocolAssigner {
			public bool Called;
			public override void CompletedSetProtocol (IProtocolAssignerProtocol value)
			{
				Called = true;
			}
		}
	}
}
