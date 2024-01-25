using Foundation;

using ObjCRuntime;

namespace NS {
	[Abstract]
	[BaseType (typeof (NSObject))]
	public interface MyObject {
		[Abstract]
		[Export ("abstractMember")]
		void AbstractMember ();
	}
}
