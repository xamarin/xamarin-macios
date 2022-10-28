.PHONY: all test clean

all: test.exe

test.exe: ../build/common/NativeTypes/Primitives.cs ../build/common/NativeTypes/Drawing.cs test.cs
	mcs -unsafe -debug -out:$@ $+ -r:System.Drawing

test: test.exe
	mono --debug $<

clean:
	rm -f test.exe{,.mdb}
