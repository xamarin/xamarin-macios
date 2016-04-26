#import <Foundation/Foundation.h>

#include "rename.h"

#ifdef __cplusplus
extern "C" {
#endif

int theUltimateAnswer ();
void useZLib ();

/*
 * Various structs used in ObjCRegistrarTest
 */

struct Sd { double d1; } Sd;
struct Sdd { double d1; double d2; } Sdd;
struct Sddd { double d1; double d2; double d3; } Sddd;
struct Sdddd { double d1; double d2; double d3; double d4; } Sdddd;
struct Si { int i1; } Si;
struct Sii { int i1; int i2; } Sii;
struct Siii { int i1; int i2; int i3; } Siii;
struct Siiii { int i1; int i2; int i3; int i4; } Siiii;
struct Siiiii { int i1; int i2; int i3; int i4; int i5; } Siiiii;
struct Sid { int i1; double d2; } Sid;
struct Sdi { double d1; int i2; } Sdi;
struct Sidi { int i1; double d2; int i3; } Sidi;
struct Siid { int i1; int i2; double d3; } Siid;
struct Sddi { double d1; double d2; int i3; } Sddi;
struct Sl { long l1; } Sl;
struct Sll { long l1; long l2; } Sll;
struct Slll { long l1; long l2; long l3; } Slll;
struct Scccc { char c1; char c2; char c3; char c4; } Scccc;
struct Sffff { float f1; float f2; float f3; float f4; } Sffff;
struct Sif { int i1; float f2; } Sif;
struct Sf { float f1; } Sf;
struct Sff { float f1; float f2; } Sff;
struct Siff { int i1; float f2; float f3; } Siff;
struct Siiff { int i1; int i2; float f3; float f4; } Siiff;
struct Sfi { float f1; int i2; } Sfi;

typedef unsigned int (^RegistrarTestBlock) (unsigned int magic);

/*
 * ObjC test class used for registrar tests.
 */
@interface ObjCRegistrarTest : NSObject {
}
	@property int Pi1;
	@property int Pi2;
	@property int Pi3;
	@property int Pi4;
	@property int Pi5;
	@property int Pi6;
	@property int Pi7;
	@property int Pi8;
	@property int Pi9;
	@property float Pf1;
	@property float Pf2;
	@property float Pf3;
	@property float Pf4;
	@property float Pf5;
	@property float Pf6;
	@property float Pf7;
	@property float Pf8;
	@property float Pf9;
	@property double Pd1;
	@property double Pd2;
	@property double Pd3;
	@property double Pd4;
	@property double Pd5;
	@property double Pd6;
	@property double Pd7;
	@property double Pd8;
	@property double Pd9;
	@property char Pc1;
	@property char Pc2;
	@property char Pc3;
	@property char Pc4;
	@property char Pc5;

	@property struct Siid PSiid1;
	@property struct Sd PSd1;
	@property struct Sf PSf1;

	-(void) V;

	-(float) F;
	-(double) D;
	-(struct Sd) Sd;
	-(struct Sf) Sf;

	-(void) V:(int)i1 i:(int)i2 i:(int)i3 i:(int)i4 i:(int)i5 i:(int)i6 i:(int)i7; // 6 in regs, 7th in mem.
	-(void) V:(float)f1 f:(float)f2 f:(float)f3 f:(float)f4 f:(float)f5 f:(float)f6 f:(float)f7 f:(float)f8 f:(float)f9; // 8 in regs, 9th in mem.
	-(void) V:(int)i1 i:(int)i2 i:(int)i3 i:(int)i4 i:(int)i5 i:(int)i6 i:(int)i7 f:(float)f1 f:(float)f2 f:(float)f3 f:(float)f4 f:(float)f5 f:(float)f6 f:(float)f7 f:(float)f8 f:(float)f9; // 6 ints in regs, 8 floats in in regs, 1 int in mem, 1 float in mem.
	-(void) V:(double)d1 d:(double)d2 d:(double)d3 d:(double)d4 d:(double)d5 d:(double)d6 d:(double)d7 d:(double)d8 d:(double)d9; // 8 in regs, 9th in mem.
	-(void) V:(int)i1 i:(int)i2 Siid:(struct Siid)Siid1 i:(int)i3 i:(int)i4 d:(double)d1 d:(double)d2 d:(double)d3 i:(int)i5 i:(int)i6 i:(int)i7; 
	-(void) V:(int)i1 i:(int)i2 f:(float)f1 Siid:(struct Siid)Siid1 i:(int)i3 i:(int)i4 d:(double)d1 d:(double)d2 d:(double)d3 i:(int)i5 i:(int)i6 i:(int)i7;
	-(void) V:(char)c1 c:(char)c2 c:(char)c3 c:(char)c4 c:(char)c5 i:(int)i1 d:(double)d1;



	-(void) invoke_V;
	-(float) invoke_F;
	-(double) invoke_D;

	-(struct Sf) Sf_invoke;

	-(RegistrarTestBlock) methodReturningBlock;
	@property (nonatomic, readonly) RegistrarTestBlock propertyReturningBlock;
	-(bool) testBlocks;
@end

#ifdef __cplusplus
} /* extern "C" */
#endif

