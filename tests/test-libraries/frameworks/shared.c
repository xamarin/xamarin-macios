
#define PASTER(y) extern const char * get ## y () { return # y; }
#define CREATE_FUNC(name) PASTER(name)

CREATE_FUNC(NAME)
