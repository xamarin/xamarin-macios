using System;
using XamCore.ObjCRuntime;

namespace XamCore.InputMethodKit {
	public partial class IMKServer {

		public IMKServer (string connectionName, string identifier)
		{
			Handle = Init (connectionName, identifier);
		}

		public IMKServer (string name, Class controllerClassId, Class delegateClassId)
		{
			Handle = Init (name, controllerClassId, delegateClassId);
		}
	}
}
