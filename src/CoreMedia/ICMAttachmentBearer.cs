using ObjCRuntime;
using System.Runtime.Versioning;

namespace CoreMedia {

	// empty interface used as a marker to state which CM objects DO support the API
	[Watch (6,0)]
	public interface ICMAttachmentBearer : INativeObject {}

}
