using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

#nullable enable

namespace Xamarin.Generator.Traceability
{
	// Debugging complex systems can be hard.
	// The Foo function could be generated 50 times in 50 contexts, but only
	//   one of them is relevant for debugging now.
	// Often to understand a leaf context it is necessary to know for what class/inlined type
	//   we are handling
	// When we crash, it would be useful to know which namespace/class/member we were in before blowing up
	// This is a simple and crude attempt to get us some of what a real traceability framework provides
	//   while fitting in with the existing infrastructure


	// A GeneratorContext is a global static state that can:
	// - Have spans (contexts of current work) pushed and popped from it
	// - Enable/Disable a detailed tracing span where other APIs will print detailed state information
	// Both creating a span 'BeginSpan` and enabling tracing `TracingEnabled` return an IDisposable that
	// must be kept as a `using var` for the API to act as expected.
	// 
	// Example:
	// using var span1 = GeneratorContext.BeginSpan ("Class: Foo"); // At some top level function
	// using var span2 = GeneratorContext.BeginSpan ("Method: Bar"); // At some mid-level function outside code being debugged
	// using var detailedTracing = GeneratorContext.TracingEnabled (mi.Name == "FooBar"); // Enables DetailedTracingEnabled only if condition is true
	// GeneratorContext.Trace (mi.DeclaringType);   // Print object only if DetailedTracingEnabled is set
	// GeneratorContext.TraceLog (() => $"My expensive value: {mi.Foo}");   // Call function and print only if DetailedTracingEnabled is set
	// GeneratorContext.TraceEnumerable ("Members", memberAvailability); // Enumerate and print each value only if DetailedTracingEnabled is set
	public static class GeneratorContext {
		// Each span is a unit of work, say "Generating Class Foo"
		// Span are often nested:
		//   "Generating Class Foo"
		//   "Generating Method Bar"
		[ThreadStatic]
		static Stack<string> Context = new Stack<string>();

		// Should Trace APIs do anything
		[ThreadStatic]
		static bool DetailedTracingEnabled = false;

		// Only print the stack state before the first print until a Begin/End
		[ThreadStatic]
		static bool ShouldPrintContext = false;

		// How many spans deep is the current context
		public static int CurrentDepth => Context.Count;

		static void Begin (string context)
		{
			Context.Push (context);
			ShouldPrintContext = true;
		}

		static void End ()
		{
			Context.Pop ();
			ShouldPrintContext = true;
		}

		public static SpanOwner BeginSpan (string context)
		{
			Begin (context);
			return new SpanOwner (CurrentDepth);
		}

		public static TracingOwner TracingEnabled (bool condition)
		{
			DetailedTracingEnabled = condition;
			ShouldPrintContext = true;
			return new TracingOwner ();
		}

		public static void EndTracing ()
		{
			DetailedTracingEnabled = false;
			ShouldPrintContext = true;
		}

		static string Indent (int count) => new string ('\t', count);
		static string IndentForContext () => new string ('\t', CurrentDepth + 1);

		static void PrintContext ()
		{
			if (ShouldPrintContext) {
				int indent = 0;

				foreach (string span in Context.Reverse()) {
					Console.Error.WriteLine (Indent(indent) + span);
					indent += 1;
				}
				ShouldPrintContext = false;
			}
		}

		public static void Trace (object value)
		{
			if (DetailedTracingEnabled) {
				PrintContext ();
				Console.Error.WriteLine (IndentForContext () + value);
			}
		}

		public static void TraceLog (Func<string> logger)
		{
			if (DetailedTracingEnabled) {
				PrintContext ();
				Console.Error.WriteLine (IndentForContext () + logger());
			}
		}

		public static void TraceEnumerable (string title, IEnumerable list)
		{
			if (DetailedTracingEnabled) {
				PrintContext ();
				Console.Error.WriteLine ($"{IndentForContext ()}{title}:");
				foreach (var value in list) {
					Console.Error.WriteLine ($"{IndentForContext ()}\t{value}");
				}
			}
		}

		public static void TraceSection ()
		{
			if (DetailedTracingEnabled) {
				PrintContext ();
				Console.Error.WriteLine ();
			}
		}

		public static void TraceStackTrace ()
		{
			if (DetailedTracingEnabled) {
				PrintContext ();
				StackTrace st = new StackTrace(true);
				Console.Error.WriteLine ();
				for (int i = 1; i < st.FrameCount; i++) {
                    if (st.GetFrame(i) is StackFrame frame) {
						Console.Error.WriteLine ($"{IndentForContext ()}{frame.GetMethod()} in {Path.GetFileName(frame.GetFileName())} at {frame.GetFileLineNumber()}");
					}
				}
				Console.Error.WriteLine ();
			}
		}

		public class SpanOwner : IDisposable
		{
			// The depth of context when we were created
			// If this does not match doing dispose, someone misused the API
			// and we should complain
			int Depth;

			public SpanOwner (int depth)
			{
				Depth = depth;
			}

			public void Dispose ()
			{
				if (GeneratorContext.CurrentDepth != Depth) {
					Console.Error.WriteLine ("Disposing GeneratorContext Span with mismatch of context state:");
					Console.Error.WriteLine ($"\tExpected {Depth} but found {GeneratorContext.CurrentDepth}.");
					Console.Error.WriteLine ("Generated trace spans may be incorrect due to an API misuse.");
				}
				GeneratorContext.End ();
			}
		}

		public class TracingOwner : IDisposable
		{
			public void Dispose ()
			{
				GeneratorContext.EndTracing ();
			}
		}
	}
}
