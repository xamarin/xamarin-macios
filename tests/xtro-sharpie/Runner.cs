using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {
	
	public class Runner {

		public Runner ()
		{
		}

		public void Execute (string pchFile, IEnumerable<string> assemblyNames)
		{
			var managed_reader = new AssemblyReader () {
				new DesignatedInitializerCheck (),
				new DllImportCheck (),
				new EnumCheck (),
				new FieldCheck (), // very noisy now, some stuff are still manually bound / harder to detect
				new ObjCInterfaceCheck (),
				new ObjCProtocolCheck (),
				new SelectorCheck (),
				new SimdCheck (),
//				new ListNative (), // for debug
			};
			foreach (var assemblyName in assemblyNames) {
				var name = Path.GetFileNameWithoutExtension (assemblyName);
				if (name.EndsWith (".iOS", StringComparison.Ordinal))
					Helpers.Platform = "ios";
				else if (name.EndsWith (".Mac", StringComparison.Ordinal))
					Helpers.Platform = "osx";
				else if (name.EndsWith (".WatchOS", StringComparison.Ordinal))
					Helpers.Platform = "watchos";
				else if (name.EndsWith (".TVOS", StringComparison.Ordinal))
					Helpers.Platform = "tvos";
				managed_reader.Load (assemblyName);
			}

			var reader = new AstReader ();
			foreach (var v in managed_reader) {
				reader.TranslationUnitParsed += tu => {
					tu.DeclFilter = ModuleExclusionList.DeclFilter;
					tu.Accept (v);
				};
			}

			reader.Load (pchFile);

			managed_reader.End ();
		}
	}

	class ModuleExclusionList {
		static List<string> macOSXExclusionList = new List<string> () {
			 // Nice to have someday
			"IOBluetooth", "IOBluetoothUI", "PubSub", "CryptoTokenKit", "DiscRecording", "DiscRecordingUI", "ImageCaptureCore", "OSAKit", "AudioVideoBridging", "Automator", "ImageCapture",

			 // Maybe?
			"ICADevices", "OpenDirectory", "IMServicePlugIn", "PreferencePanes", "ScreenSaver", "CoreMediaIO", "SecurityInterface",

			 // Nope
			"InstallerPlugins", "JavaVM", "ExceptionHandling", "JavaFrameEmbedding",

			// Deprecated so double Nope
			"SyncServices", "CalendarStore",
		};

		static IEnumerable<string> _exclusionList;
		static IEnumerable<string> ExclusionList {
			get {
				if (_exclusionList == null) {
					switch (Helpers.Platform) {
					case "osx":
						_exclusionList = macOSXExclusionList;
						break;
					default:
						_exclusionList = Enumerable.Empty <string> ();
						break;
					}
				}
				return _exclusionList;
			}
		}

		public static bool DeclFilter (Decl d)
		{
			if (d.OwningModule != null) {
				foreach (var item in ExclusionList)
					if (d.InModule (item))
						return false;
			}
			return true;
		}
	}

	class AssemblyReader : IEnumerable<BaseVisitor> {

		public void Load (string filename)
		{
			var ad = AssemblyDefinition.ReadAssembly (filename);
			foreach (var v in Visitors) {
				v.VisitManagedAssembly (ad);
				foreach (var module in ad.Modules) {
					v.VisitManagedModule (module);
					if (!module.HasTypes)
						continue;
					foreach (var td in module.Types)
						ProcessType (v, td);
				}
			}
		}

		void ProcessType (BaseVisitor v, TypeDefinition type)
		{
			v.VisitManagedType (type);
			if (type.HasMethods) {
				foreach (var md in type.Methods)
					v.VisitManagedMethod (md);
			}

			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (v, nested);
			}
		}

		List<BaseVisitor> Visitors { get; } = new List<BaseVisitor> ();

		public void Add (BaseVisitor visitor)
		{
			Visitors.Add (visitor);
		}

		public void End ()
		{
			foreach (var v in Visitors)
				v.End ();
		}

		public IEnumerator<BaseVisitor> GetEnumerator ()
		{
			return Visitors.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return Visitors.GetEnumerator ();
		}
	}

	public class BaseVisitor : AstVisitor {

		public virtual void VisitManagedAssembly (AssemblyDefinition assembly)
		{
		}

		public virtual void VisitManagedModule (ModuleDefinition module)
		{
		}

		public virtual void VisitManagedType (TypeDefinition type)
		{
		}

		public virtual void VisitManagedMethod (MethodDefinition method)
		{
		}

		// last chance to report errors
		public virtual void End ()
		{
		}

		protected string GetDeclaringHeaderFile (Decl decl)
		{
			var header_file = decl.PresumedLoc.FileName;
			var fxh = header_file.IndexOf (".framework/Headers/", StringComparison.Ordinal);
			if (fxh > 0) {
				var start = header_file.LastIndexOf ('/', fxh) + 1;
				return header_file.Substring (start, fxh - start);
			}
			return null;
		}
	}


	// debug
	class ListNative : BaseVisitor {
		
		public override void VisitDecl (Decl decl)
		{
			if (decl is FunctionDecl) {
				;
			} else if (decl is VarDecl) {
				;
			} else if (decl is ObjCProtocolDecl) {
				;
			} else if (decl is ObjCInterfaceDecl) {
				;
			} else if (decl is EnumDecl) {
				;
			} else {
				Console.WriteLine ("{0}\t{1}", decl, decl.GetType ().Name);
			}
		}
	}
}
