#import <Foundation/Foundation.h>

#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#include <zlib.h>
#include "libtest.h"

int
theUltimateAnswer ()
{
	return 42;
}

void useZLib ()
{
	printf ("ZLib version: %s\n", zlibVersion ());
}

void x_call_block (x_block_callback block)
{
	block ();
}

typedef matrix_float2x2 (*func_x_get_matrix_float2x2_msgSend) (id self, SEL sel);
void
x_get_matrix_float2x2 (id self, const char *sel,
		float* r0c0, float* r0c1,
		float* r1c0, float* r1c1)
{
	matrix_float2x2 rv;
#if __i386__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __x86_64__
	IMP msgSend = (IMP) objc_msgSend;
#elif __arm64__
	IMP msgSend = (IMP) objc_msgSend;
#elif __arm__
	IMP msgSend = (IMP) objc_msgSend_stret;
#else
#error unknown architecture
#endif
	rv = ((func_x_get_matrix_float2x2_msgSend) msgSend) (self, sel_registerName (sel));
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
}

typedef matrix_float3x3 (*func_x_get_matrix_float3x3_msgSend) (id self, SEL sel);
void
x_get_matrix_float3x3 (id self, const char *sel,
		float* r0c0, float* r0c1, float* r0c2,
		float* r1c0, float* r1c1, float* r1c2,
		float* r2c0, float* r2c1, float* r2c2)
{
	matrix_float3x3 rv;
#if __i386__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __x86_64__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __arm64__
	IMP msgSend = (IMP) objc_msgSend;
#elif __arm__
	IMP msgSend = (IMP) objc_msgSend_stret;
#else
#error unknown architecture
#endif
	rv = ((func_x_get_matrix_float3x3_msgSend) msgSend) (self, sel_registerName (sel));
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
}

typedef matrix_float4x4 (*func_x_get_matrix_float4x4_msgSend) (id self, SEL sel);
void
x_get_matrix_float4x4 (id self, const char *sel,
		float* r0c0, float* r0c1, float* r0c2, float* r0c3,
		float* r1c0, float* r1c1, float* r1c2, float* r1c3,
		float* r2c0, float* r2c1, float* r2c2, float* r2c3,
		float* r3c0, float* r3c1, float* r3c2, float* r3c3)
{
	matrix_float4x4 rv;
#if __i386__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __x86_64__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __arm64__
	IMP msgSend = (IMP) objc_msgSend;
#elif __arm__
	IMP msgSend = (IMP) objc_msgSend_stret;
#else
#error unknown architecture
#endif
	rv = ((func_x_get_matrix_float4x4_msgSend) msgSend) (self, sel_registerName (sel));
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];
	*r0c3 = rv.columns[3][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];
	*r1c3 = rv.columns[3][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
	*r2c3 = rv.columns[3][2];

	*r3c0 = rv.columns[0][3];
	*r3c1 = rv.columns[1][3];
	*r3c2 = rv.columns[2][3];
	*r3c3 = rv.columns[3][3];
}

typedef matrix_float4x3 (*func_x_get_matrix_float4x3_msgSend) (id self, SEL sel);
void
x_get_matrix_float4x3 (id self, const char *sel,
		float* r0c0, float* r0c1, float* r0c2, float* r0c3,
		float* r1c0, float* r1c1, float* r1c2, float* r1c3,
		float* r2c0, float* r2c1, float* r2c2, float* r2c3)
{
	matrix_float4x3 rv;
#if __i386__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __x86_64__
	IMP msgSend = (IMP) objc_msgSend_stret;
#elif __arm64__
	IMP msgSend = (IMP) objc_msgSend;
#elif __arm__
	IMP msgSend = (IMP) objc_msgSend_stret;
#else
#error unknown architecture
#endif
	rv = ((func_x_get_matrix_float4x3_msgSend) msgSend) (self, sel_registerName (sel));
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];
	*r0c3 = rv.columns[3][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];
	*r1c3 = rv.columns[3][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
	*r2c3 = rv.columns[3][2];
}
#if !TARGET_OS_WATCH
void
x_mdltransformcomponent_get_local_transform (id<MDLTransformComponent> self, NSTimeInterval time,
		float* r0c0, float* r0c1, float* r0c2, float* r0c3,
		float* r1c0, float* r1c1, float* r1c2, float* r1c3,
		float* r2c0, float* r2c1, float* r2c2, float* r2c3,
		float* r3c0, float* r3c1, float* r3c2, float* r3c3)
{
	matrix_float4x4 rv;
	rv = [self localTransformAtTime: time];
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];
	*r0c3 = rv.columns[3][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];
	*r1c3 = rv.columns[3][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
	*r2c3 = rv.columns[3][2];

	*r3c0 = rv.columns[0][3];
	*r3c1 = rv.columns[1][3];
	*r3c2 = rv.columns[2][3];
	*r3c3 = rv.columns[3][3];
}

void
x_mdltransform_create_global_transform (MDLObject *object, NSTimeInterval time,
		float* r0c0, float* r0c1, float* r0c2, float* r0c3,
		float* r1c0, float* r1c1, float* r1c2, float* r1c3,
		float* r2c0, float* r2c1, float* r2c2, float* r2c3,
		float* r3c0, float* r3c1, float* r3c2, float* r3c3)
{
	matrix_float4x4 rv;
	rv = [MDLTransform globalTransformWithObject: object atTime: time];
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];
	*r0c3 = rv.columns[3][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];
	*r1c3 = rv.columns[3][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
	*r2c3 = rv.columns[3][2];

	*r3c0 = rv.columns[0][3];
	*r3c1 = rv.columns[1][3];
	*r3c2 = rv.columns[2][3];
	*r3c3 = rv.columns[3][3];
}

void
x_mdltransform_get_rotation_matrix (MDLTransform *self, NSTimeInterval time,
		float* r0c0, float* r0c1, float* r0c2, float* r0c3,
		float* r1c0, float* r1c1, float* r1c2, float* r1c3,
		float* r2c0, float* r2c1, float* r2c2, float* r2c3,
		float* r3c0, float* r3c1, float* r3c2, float* r3c3)
{
	matrix_float4x4 rv;
	rv = [self rotationMatrixAtTime: time];
	*r0c0 = rv.columns[0][0];
	*r0c1 = rv.columns[1][0];
	*r0c2 = rv.columns[2][0];
	*r0c3 = rv.columns[3][0];

	*r1c0 = rv.columns[0][1];
	*r1c1 = rv.columns[1][1];
	*r1c2 = rv.columns[2][1];
	*r1c3 = rv.columns[3][1];

	*r2c0 = rv.columns[0][2];
	*r2c1 = rv.columns[1][2];
	*r2c2 = rv.columns[2][2];
	*r2c3 = rv.columns[3][2];

	*r3c0 = rv.columns[0][3];
	*r3c1 = rv.columns[1][3];
	*r3c2 = rv.columns[2][3];
	*r3c3 = rv.columns[3][3];
}
#endif // !TARGET_OS_WATCH

@interface UltimateMachine : NSObject {

}
- (int) getAnswer;
+ (UltimateMachine *) sharedInstance;
@end

@implementation UltimateMachine
{

}
- (int) getAnswer
{
	return 42;
}

static UltimateMachine *shared;

+ (UltimateMachine *) sharedInstance
{
	if (shared == nil)
		shared = [[UltimateMachine alloc] init];
	return shared;
}
@end

@interface FakeType2 : NSObject {
}
-(BOOL) isKindOfClass: (Class) cls;
@end

@implementation FakeType2
{
}
- (BOOL) isKindOfClass: (Class) cls;
{
	if (cls == objc_getClass ("FakeType1"))
		return YES;
	
	return [super isKindOfClass: cls];
}
@end

/*
 * ObjC test class used for registrar tests.
*/
@implementation ObjCRegistrarTest
{
}
	-(void) V
	{
	}

	-(float) F
	{
		return _Pf1;
	}

	-(double) D
	{
		return _Pd1;
	}

	-(struct Sd) Sd
	{
		return _PSd;
	}

	-(struct Sf) Sf
	{
		return _PSf;
	}

	-(void) V:(int)i1 i:(int)i2 i:(int)i3 i:(int)i4 i:(int)i5 i:(int)i6 i:(int)i7
	{
		// x86_64: 6 in regs, 7th in mem.
		_Pi1 = i1; _Pi2 = i2; _Pi3 = i3; _Pi4 = i4; _Pi5 = i5; _Pi6 = i6; _Pi7 = i7;
	}

	-(void) V:(float)f1 f:(float)f2 f:(float)f3 f:(float)f4 f:(float)f5 f:(float)f6 f:(float)f7 f:(float)f8 f:(float)f9
	{
		// x86_64: 8 in regs, 9th in mem.
		_Pf1 = f1; _Pf2 = f2; _Pf3 = f3; _Pf4 = f4; _Pf5 = f5; _Pf6 = f6; _Pf7 = f7; _Pf8 = f8; _Pf9 = f9;
	}

	-(void) V:(int)i1 i:(int)i2 i:(int)i3 i:(int)i4 i:(int)i5 i:(int)i6 i:(int)i7 f:(float)f1 f:(float)f2 f:(float)f3 f:(float)f4 f:(float)f5 f:(float)f6 f:(float)f7 f:(float)f8 f:(float)f9
	{
		// x86_64: 6 ints in regs, 8 floats in in regs, 1 int in mem, 1 float in mem.
		_Pi1 = i1; _Pi2 = i2; _Pi3 = i3; _Pi4 = i4; _Pi5 = i5; _Pi6 = i6; _Pi7 = i7;
		_Pf1 = f1; _Pf2 = f2; _Pf3 = f3; _Pf4 = f4; _Pf5 = f5; _Pf6 = f6; _Pf7 = f7; _Pf8 = f8; _Pf9 = f9;
	}

	-(void) V:(double)d1 d:(double)d2 d:(double)d3 d:(double)d4 d:(double)d5 d:(double)d6 d:(double)d7 d:(double)d8 d:(double)d9
	{
		// x86_64: 8 in regs, 9th in mem.
		_Pd1 = d1; _Pd2 = d2; _Pd3 = d3; _Pd4 = d4; _Pd5 = d5; _Pd6 = d6; _Pd7 = d7; _Pd8 = d8; _Pd9 = d9;
	}

	-(void) V:(int)i1 i:(int)i2 Siid:(struct Siid)s1 i:(int)i3 i:(int)i4 d:(double)d1 d:(double)d2 d:(double)d3 i:(int)i5 i:(int)i6 i:(int)i7
	{
		_Pi1 = i1; _Pi2 = i2; _PSiid = s1; _Pi3 = i3; _Pi4 = i4; _Pd1 = d1; _Pd2 = d2; _Pd3 = d2; _Pi5 = i5; _Pi6 = i6; _Pi7 = i7;
	}

	-(void) V:(int)i1 i:(int)i2 f:(float)f1 Siid:(struct Siid)s1 i:(int)i3 i:(int)i4 d:(double)d1 d:(double)d2 d:(double)d3 i:(int)i5 i:(int)i6 i:(int)i7
	{
		_Pi1 = i1; _Pi2 = i2; _Pf1 = f1; _PSiid = s1; _Pi3 = i3; _Pi4 = i4; _Pd1 = d1; _Pd2 = d2; _Pd3 = d3; _Pi5 = i5; _Pi6 = i6; _Pi7 = i7;
	}

	-(void) V:(char)c1 c:(char)c2 c:(char)c3 c:(char)c4 c:(char)c5 i:(int)i1 d:(double)d1
	{
		_Pc1 = c1; _Pc2 = c2; _Pc3 = c3; _Pc4 = c4; _Pc5 = c5; _Pi1 = i1; _Pd1 = d1;
	}

	-(void) V:(out id  _Nullable *)n1 n:(out NSString * _Nullable *)n2
	{
		abort (); // this method is supposed to be overridden
	}

	/*
	 * Invoke method
	 */

	-(void) invoke_V
	{
		[self V];
	}

	-(float) invoke_F
	{
		return [self F];
	}

	-(double) invoke_D
	{
		return [self D];
	}

	-(struct Sf) Sf_invoke
	{
		return [self Sf];
	}

	-(void) invoke_V_null_out
	{
		[self V:nil n:nil];
	}

	/*
	 * API returning blocks.
	 */
	-(RegistrarTestBlock) methodReturningBlock
	{
		return nil;
	}

	-(bool) testBlocks
	{
		unsigned int output;
		unsigned int expected;
		unsigned int input;

		input = 0xdeadf00d;
		expected = 0x1337b001;
		output = [self methodReturningBlock] (input);
		if (output != expected) {
			NSLog (@"methodReturningBlock didn't return the expected value 0x%x for the input value 0x%x, but got instead 0x%x.", expected, input, output);
			return false;
		}

		input = 0xdeadf11d;
		expected = 0x7b001133;
		output = self.propertyReturningBlock (input);
		if (output != expected) {
			NSLog (@"propertyReturningBlock didn't return the expected value 0x%x for the input value 0x%x, but got instead 0x%x.", expected, input, output);
			return false;
		}

		return true;
	}

	-(void) idAsIntPtr: (id)p1
	{
		// Nothing to do here.
	}

#include "libtest.methods.m"

	-(void) outNSErrorOnStack:(int)i1 i:(int)i2 i:(int)i3 i:(int)i4 i:(int)i5 i:(int)i6 err:(NSError **)err
	{
		// Nothing to do here
	}

	
	-(void) outNSErrorOnStack:(id)obj1 obj:(id)obj2 obj:(id)obj3 int64:(long long)l4 i:(int)i5 err:(NSError **)err
	{
		// Nothing to do here
	}
@end

@implementation ProtocolAssigner
-(void) setProtocol
{
	ObjCProtocolTestImpl *p = [[ObjCProtocolTestImpl alloc] init];
	[self completedSetProtocol: p];
}

-(void) completedSetProtocol: (id<ProtocolAssignerProtocol>) value
{
	assert (!"THIS FUNCTION SHOULD BE OVERRIDDEN");
}
@end

@implementation ObjCProtocolTestImpl
@end

@implementation ObjCExceptionTest
{
}
	-(void) throwObjCException
	{
		[NSException raise:@"Some exception" format:@"exception was thrown"];
	}

	-(void) throwManagedException
	{
		abort (); // this method should be overridden in managed code.
	}

	-(void) invokeManagedExceptionThrower
	{
		[self throwManagedException];	
	}

	-(void) invokeManagedExceptionThrowerAndRethrow
	{
		@try {
			[self throwManagedException];			
		} @catch (id exc) {
			[NSException raise:@"Caught exception" format:@"exception was rethrown"];
		}
	}
	-(void) invokeManagedExceptionThrowerAndCatch
	{
		@try {
			[self throwManagedException];			
		} @catch (id exc) {
			// do nothing
		}
	}
@end

@interface CtorChaining1 : NSObject
	@property BOOL initCalled;
	@property BOOL initCallsInitCalled;
	-(instancetype) init;
	-(instancetype) initCallsInit:(int) value;
@end
@implementation CtorChaining1
-(instancetype) init
{
	self.initCalled = YES;
	return [super init];
}
-(instancetype) initCallsInit:(int) value
{
	self.initCallsInitCalled = YES;
	return [self init];
}
@end

@implementation ObjCProtocolClassTest
-(void) idAsIntPtr: (id)p1
{
	// Do nothing
}
@end

static volatile int freed_blocks = 0;
static volatile int called_blocks = 0;

@implementation ObjCBlockTester
static Class _TestClass = NULL;

+ (Class)TestClass {
	return _TestClass;
}

+ (void)setTestClass:(Class) value {
	_TestClass = value;
}

-(void) classCallback: (void (^)(int32_t magic_number))completionHandler
{
	assert (!"THIS FUNCTION SHOULD BE OVERRIDDEN");
}

-(void) callClassCallback
{
	__block bool called = false;
	[self classCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		called = true;
	}];
	assert (called);
}

-(void) callRequiredCallback
{
	__block bool called = false;
	[self.TestObject requiredCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		called = true;
	}];
	assert (called);
}

+(void) callRequiredStaticCallback
{
	__block bool called = false;
	[self.TestClass requiredStaticCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		called = true;
	}];
	assert (called);
}

-(void) callOptionalCallback
{
	__block bool called = false;
	[self.TestObject optionalCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		called = true;
	}];
	assert (called);
}

+(void) callOptionalStaticCallback
{
	__block bool called = false;
	[self.TestClass optionalStaticCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		called = true;
	}];
	assert (called);
}

+(void) callAssertMainThreadBlockRelease: (outerBlock) completionHandler
{
	MainThreadDeallocator *obj = [[MainThreadDeallocator alloc] init];
	__block bool success = false;

	dispatch_sync (dispatch_get_global_queue (DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul), ^{
		completionHandler (^(int magic_number)
		{
			assert (magic_number == 42);
			assert ([NSThread isMainThread]); // This may crash way after the failed test has finished executing.
			success = obj != NULL; // this captures the 'obj', and it's only freed when the block is freed.
		});
    });
	assert (success);
    [obj release];
}

-(void) callAssertMainThreadBlockReleaseCallback
{
	MainThreadDeallocator *obj = [[MainThreadDeallocator alloc] init];
	__block bool success = false;

	dispatch_sync (dispatch_get_global_queue (DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul), ^{
		[self assertMainThreadBlockReleaseCallback: ^(int magic_number)
		{
			assert (magic_number == 42);
			assert ([NSThread isMainThread]); // This may crash way after the failed test has finished executing.
			success = obj != NULL; // this captures the 'obj', and it's only freed when the block is freed.
		}];
    });
	assert (success);
    [obj release];
}

-(void) assertMainThreadBlockReleaseCallback: (innerBlock) completionHandler
{
	assert (!"THIS FUNCTION SHOULD BE OVERRIDDEN");
}

-(void) testFreedBlocks
{
	FreedNotifier* obj = [[FreedNotifier alloc] init];
	__block bool success = false;
	[self classCallback: ^(int magic_number)
	{
		assert (magic_number == 42);
		success = obj != NULL; // this captures the 'obj', and it's only freed when the block is freed.
	}];
	assert (success);
	[obj release];
}
+(int) freedBlockCount
{
	return freed_blocks;
}
+(int) calledBlockCount
{
	return called_blocks;
}

static void block_called ()
{
	OSAtomicIncrement32 (&called_blocks);
}

+(void) callProtocolWithBlockProperties: (id<ProtocolWithBlockProperties>) obj required: (bool) required instance: (bool) instance;
{
	if (required) {
		if (instance) {
			obj.myRequiredProperty ();
		} else {
			[[(NSObject *) obj class] myRequiredStaticProperty] ();
		}
	} else {
		if (instance) {
			obj.myOptionalProperty ();
		} else {
			[[(NSObject *) obj class] myOptionalStaticProperty] ();
		}
	}
}

+(void) callProtocolWithBlockReturnValue: (id<ObjCProtocolBlockTest>) obj required: (bool) required instance: (bool) instance;
{
	if (required) {
		if (instance) {
			[obj requiredReturnValue] (42);
		} else {
			[[(NSObject *) obj class] requiredStaticReturnValue] (42);
		}
	} else {
		if (instance) {
			[obj optionalReturnValue] (42);
		} else {
			[[(NSObject *) obj class] optionalStaticReturnValue] (42);
		}
	}
}

+(void) callProtocolWithBlockPropertiesRequired: (id<ProtocolWithBlockProperties>) obj
{
	obj.myRequiredProperty ();
}

+(void) setProtocolWithBlockProperties: (id<ProtocolWithBlockProperties>) obj required: (bool) required instance: (bool) instance
{
	simple_callback callback = ^{ block_called (); };
	if (required) {
		if (instance) {
			obj.myRequiredProperty = callback;
		} else {
			[[(NSObject *) obj class] setMyRequiredStaticProperty: callback];
		}
	} else {
		if (instance) {
			obj.myOptionalProperty = callback;
		} else {
			[[(NSObject *) obj class] setMyOptionalStaticProperty: callback];
		}
	}
}

@end

@implementation FreedNotifier
-(void) dealloc
{
	OSAtomicIncrement32 (&freed_blocks);
	[super dealloc];
}
@end

@implementation EvilDeallocator
-(void) dealloc
{
	if (self.evilCallback != NULL)
		self.evilCallback (314);
	[super dealloc];
}
@end

@implementation MainThreadDeallocator : NSObject {
}
-(void) dealloc
{
	assert ([NSThread isMainThread]);
	[super dealloc];
}
@end

#include "libtest.decompile.m"
