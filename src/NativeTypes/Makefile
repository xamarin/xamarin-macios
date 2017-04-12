TT = "/Applications/Visual Studio.app/Contents/MacOS/lib/monodevelop/AddIns/MonoDevelop.TextTemplating/TextTransform.exe"

.PHONY: all test clean

all: Primitives.cs Drawing.cs test.exe

%.cs: %.tt
	mono $(TT) $< -o $@

test.exe: Primitives.cs Drawing.cs test.cs
	mcs -unsafe -debug -out:$@ $+ -r:System.Drawing -define:XAMCORE_2_0

test: test.exe
	mono --debug $<

clean:
	rm -f test.exe{,.mdb} Primitives.cs Drawing.cs
