MonoMac provides an API compatible interface to the OpenTK GL.* functions.
This is accomplished by forking a few files from OpenTK and removing their
delegate / context infrastructure and just doing direct pinvokes to 
OpenGL.framework.

Unsupported extensions and methods have been commented out to avoid runtime
exceptions.

With the exception of OpenTK/Graphics/OpenGL/* the rest of the files are
a direct copy from OpenTK with the namespaces changed.

This was forked from OpenTK r3066, by Kenneth Pouncey.
