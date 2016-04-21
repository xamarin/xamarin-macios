// Copyright (C) 2011,2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.ObjcBinding.Tasks {
	public class BTouch : ToolTask {
		public ITaskItem[] AdditionalLibPaths { get; set; }
		
		public bool AllowUnsafeBlocks { get; set; }
		public string CompilerPath { get; set; }
		public bool NoStdLib { get; set; }

		[Required]
		public string BaseLibDll { get; set; }

		[Required]
		public string BTouchToolPath { get; set; }

		[Required]
		public ITaskItem[] ApiDefinitions { get; set; }
		
		public ITaskItem[] CoreSources { get; set; }
		
		public string DefineConstants { get; set; }
		
		public bool EmitDebugInformation { get; set; }
		
		public string GeneratedSourcesDirectory { get; set; }
		
		[Output]
		public string GeneratedSourcesFileList { get; set; }
		
		public string Namespace { get; set; }
		
		public ITaskItem[] NativeLibraries { get; set; }
		
		public string OutputAssembly { get; set; }
		
		public ITaskItem[] References { get; set; }
		
		public ITaskItem[] Resources { get; set; }
		
		public ITaskItem[] Sources { get; set; }
		
		protected override string ToolName {
			get { return "btouch"; }
		}
		
		protected override string GenerateFullPathToTool ()
		{
			return Path.Combine (BTouchToolPath, ToolExe);
		}
		
		protected override string GenerateCommandLineCommands ()
		{
			var cmd = new CommandLineBuilder ();
#if DEBUG
			cmd.AppendSwitch ("/v");
#endif
			if (NoStdLib)
				cmd.AppendSwitch ("/nostdlib");
			cmd.AppendSwitchIfNotNull ("/compiler:", CompilerPath);
			cmd.AppendSwitchIfNotNull ("/baselib:", BaseLibDll);
			cmd.AppendSwitchIfNotNull ("/out:", OutputAssembly);

			if (NoStdLib) {
				string dir;
				if (!string.IsNullOrEmpty (BaseLibDll))
					dir = Path.GetDirectoryName (BaseLibDll);
				else
					dir = null;
				cmd.AppendSwitchIfNotNull ("/lib:", dir);
				cmd.AppendSwitchIfNotNull ("/r:", Path.Combine (dir, "mscorlib.dll"));
			}

			if (EmitDebugInformation)
				cmd.AppendSwitch ("/debug");
			
			if (AllowUnsafeBlocks)
				cmd.AppendSwitch ("/unsafe");
			
			cmd.AppendSwitchIfNotNull ("/ns:", Namespace);
			
			if (!string.IsNullOrEmpty (DefineConstants)) {
				string[] strv = DefineConstants.Split (new [] { ';' });
				var sanitized = new List<string> ();
				
				foreach (var str in strv) {
					if (str != string.Empty)
						sanitized.Add (str);
				}
				
				if (sanitized.Count > 0)
					cmd.AppendSwitchIfNotNull ("/d:", string.Join (";", sanitized.ToArray ()));
			}
			
			//cmd.AppendSwitch ("/e");
			
			foreach (var item in ApiDefinitions)
				cmd.AppendFileNameIfNotNull (item);
			
			if (CoreSources != null) {
				foreach (var item in CoreSources)
					cmd.AppendSwitchIfNotNull ("/s:", item);
			}
			
			if (Sources != null) {
				foreach (var item in Sources)
					cmd.AppendSwitchIfNotNull ("/x:", item);
			}
			
			if (AdditionalLibPaths != null) {
				foreach (var item in AdditionalLibPaths)
					cmd.AppendSwitchIfNotNull ("/lib:", item);
			}

			if (References != null) {
				foreach (var item in References)
					cmd.AppendSwitchIfNotNull ("-r ", item);
			}
			
			if (Resources != null) {
				foreach (var item in Resources) {
					List<string> args = new List<string> ();
					string id;
					
					args.Add (item.ToString ());
					id = item.GetMetadata ("LogicalName");
					if (!string.IsNullOrEmpty (id))
						args.Add (id);
					
					cmd.AppendSwitchIfNotNull ("/res:", args.ToArray (), ",");
				}
			}
			
			if (NativeLibraries != null) {
				foreach (var item in NativeLibraries) {
					List<string> args = new List<string> ();
					string id;
					
					args.Add (item.ToString ());
					id = item.GetMetadata ("LogicalName");
					if (string.IsNullOrEmpty (id))
						id = Path.GetFileName (args[0]);
					args.Add (id);
					
					cmd.AppendSwitchIfNotNull ("/link-with:", args.ToArray (), ",");
				}
			}
			
			if (GeneratedSourcesDirectory != null)
				cmd.AppendSwitchIfNotNull ("/tmpdir:", GeneratedSourcesDirectory);
			
			if (GeneratedSourcesFileList != null)
				cmd.AppendSwitchIfNotNull ("/sourceonly:", GeneratedSourcesFileList);
			
			return cmd.ToString ();
		}

		void PrintITaskItems (ITaskItem [] items)
		{
			if (items == null || items.Length == 0)
				return;

			foreach (var i in items)
				Log.LogMessage (MessageImportance.Low, "    {0}", i.ItemSpec);
		}

		public override bool Execute ()
		{
			Log.LogMessage (MessageImportance.Low, "BTouch Task");
			Log.LogMessage (MessageImportance.Low, "  AdditionalLibPaths: {0}", AdditionalLibPaths == null || AdditionalLibPaths.Length == 0 ? "<none>" : string.Empty);
			PrintITaskItems (AdditionalLibPaths);
			Log.LogMessage (MessageImportance.Low, "  AllowUnsafeBlocks: {0}", AllowUnsafeBlocks);
			Log.LogMessage (MessageImportance.Low, "  ApiDefinitions: {0}", ApiDefinitions);
			Log.LogMessage (MessageImportance.Low, "  CoreSources: {0}", CoreSources);
			Log.LogMessage (MessageImportance.Low, "  DefineConstants: {0}", DefineConstants);
			Log.LogMessage (MessageImportance.Low, "  EmitDebugInformation: {0}", EmitDebugInformation);
			Log.LogMessage (MessageImportance.Low, "  GeneratedSourcesDirectory: {0}", GeneratedSourcesDirectory);
			Log.LogMessage (MessageImportance.Low, "  GeneratedSourcesFileList: {0}", GeneratedSourcesFileList);
			Log.LogMessage (MessageImportance.Low, "  Namespace: {0}", Namespace);
			Log.LogMessage (MessageImportance.Low, "  NativeLibraries: {0}", NativeLibraries == null || NativeLibraries.Length == 0 ? "<none>" : string.Empty);
			PrintITaskItems (NativeLibraries);
			Log.LogMessage (MessageImportance.Low, "  OutputAssembly: {0}", OutputAssembly);
			Log.LogMessage (MessageImportance.Low, "  References: {0}", References == null || References.Length == 0 ? "<none>" : string.Empty);
			PrintITaskItems (References);
			Log.LogMessage (MessageImportance.Low, "  Sources: {0}", Sources == null || Sources.Length == 0 ? "<none>" : string.Empty);
			PrintITaskItems (Sources);

			if (ApiDefinitions.Length == 0) {
				Log.LogError ("No API definition file specified.");
				return false;
			}
			
			return base.Execute ();
		}
	}
}
