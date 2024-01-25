using Foundation;

using ObjCRuntime;
namespace Issue3875 {
	[Protocol, Model (Name = "BProtocol")]
	[BaseType (typeof (NSObject))]
	interface AProtocol {
	}
}
