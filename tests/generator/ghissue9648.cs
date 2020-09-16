using System;
using System.Drawing;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Issue9648
{

	public partial interface IBrokenProtocol
	{
		bool DidWeFixTheAPI ();
	}

	internal partial class BrokenProtocolWrapper
	{
		public bool DidWeFixTheAPI ()
			=> true;
	}

	[Partial]
	[Protocol]
	interface BrokenProtocol
	{
		[Abstract]
		[Export ("error")]
		bool IsError { get; }
	}
}
