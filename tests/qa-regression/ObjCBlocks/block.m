#import <Foundation/Foundation.h>

@interface ObjCBlocksTest : NSObject

- (void)doInvoke:(void (^)(void))blockHandler;

@end

@implementation ObjCBlocksTest : NSObject

- (void)doInvoke:(void (^)(void))blockHandler
{
	printf ("[native] ENTER ObjCBlocksTest_Invoke (blockHandler = %p)\n",
		blockHandler);
	blockHandler ();
	printf ("[native] LEAVE ObjCBlocksTest_Invoke\n");
}

@end
