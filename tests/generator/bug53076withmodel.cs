using System;

using Foundation;
using ObjCRuntime;

namespace Bug53076WithModelTest {
	[Protocol][Model]
	[BaseType (typeof (NSObject))]
	interface MyFooProtocol {

		[Abstract]
		[Async]
		[Export ("requiredMethod:completion:")]
		void RequiredMethod (int arg1, Action<NSError> err);

		[Async]
		[Export ("optional:completion:")]
		void OptionalMethod (int arg1, Action<NSError> err);

		[Abstract]
		[Async]
		[Export ("requiredReturnMethod:completion:")]
		bool RequiredReturnMethod (int arg1, Action<NSError> err);

		[Async]
		[Export ("optionalReturn:completion:")]
		bool OptionalReturnMethod (int arg1, Action<NSError> err);

		[Abstract]
		[Async (ResultTypeName = "RequiredReturnMethodObjResult")]
		[Export ("requiredReturnMethodObj:completion:")]
		bool RequiredReturnMethodObj (int arg1, Action<NSError,NSObject> err);

		[Async (ResultTypeName = "RequiredReturnMethodObjResult")]
		[Export ("optionalReturnObj:completion:")]
		bool OptionalReturnMethodObj (int arg1, Action<NSError,NSObject> err);
	}
	
	[BaseType (typeof (NSObject))]
	interface RequiredReturnMethodObjResult {}
}

