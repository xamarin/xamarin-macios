using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Cecil;

using Xamarin.Bundler;
using Registrar;

namespace Xamarin.Bundler
{
	class PInvokeWrapperGenerator
	{
		public Application App;
		public Dictionary<string,string> signatures = new Dictionary<string, string> ();
		public List<Exception> exceptions = new List<Exception> ();
		public StringBuilder signature = new StringBuilder ();
		public HashSet<string> names = new HashSet<string> ();

		public AutoIndentStringBuilder sb = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder hdr = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder decls = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder mthds = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder ifaces = new AutoIndentStringBuilder ();
						
		public StaticRegistrar Registrar;
		public string HeaderPath;
		public string SourcePath;

		bool first;

		public bool Started {
			get {
				return first;
			}
		}

		public void Start ()
		{							
			if (App.EnableDebug)
				hdr.WriteLine ("#define DEBUG 1");

			hdr.WriteLine ("#include <stdarg.h>");
			hdr.WriteLine ("#include <xamarin/xamarin.h>");
			hdr.WriteLine ("#include <objc/objc.h>");
			hdr.WriteLine ("#include <objc/runtime.h>");
			hdr.WriteLine ("#include <objc/message.h>");

			Registrar.GeneratePInvokeWrappersStart (hdr, decls, mthds, ifaces) ;

			mthds.WriteLine ($"#include \"{Path.GetFileName (HeaderPath)}\"");

			sb.WriteLine ("extern \"C\" {");
		}

		public void End ()
		{
			if (!first)
				throw new Exception ("Generator not started");

			sb.WriteLine ("}");

			Registrar.GeneratePInvokeWrappersEnd ();

			Driver.WriteIfDifferent (HeaderPath, hdr.ToString () + "\n" + decls.ToString () + "\n" + ifaces.ToString () + "\n") ;
			Driver.WriteIfDifferent (SourcePath, mthds.ToString () + "\n" + sb.ToString () + "\n");
		}

		public void ProcessMethod (MethodDefinition method)
		{
			if (!first) {
				Start ();
				first = true;
			}

			Registrar.GeneratePInvokeWrapper (this, method);
		}
	}
}

