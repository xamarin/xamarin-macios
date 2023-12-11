using System;

using Foundation;
using ObjCRuntime;
using UIKit;

namespace iosbindinglib {
	[Protocol]
	[BaseType (typeof (NSObject))]
	public interface ReaderProtocol {
	}

	[BaseType (typeof (NSObject))]
	public interface Reader : ReaderProtocol {
	}
}
