// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.IO;

using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.Linker {

	public abstract class BaseProfile {

		protected abstract bool IsSdk (string assemblyName);
	}

	public class ProfilePoker : MobileProfile {

		static ProfilePoker p = new ProfilePoker ();

		public static bool IsWellKnownSdk (string assemblyName)
		{
			return p.IsSdk (assemblyName);
		}
	}

	[TestFixture]
	public partial class SdkTest {
		
		static string ClassicPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/2.1/"); } }
		static string UnifiedPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.iOS/"); } }
		static string tvOSPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.TVOS/"); } }
		static string watchOSPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.WatchOS/"); } }

		void BCL (string path)
		{
			foreach (var file in Directory.GetFiles (path, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				switch (aname) {
				case "FSharp.Core":
				case "I18N":
				case "I18N.CJK":
				case "I18N.MidEast":
				case "I18N.Other":
				case "I18N.Rare":
				case "I18N.West":
					// need to be sure they are link-capable to include them into MobileProfile
					break;
				case "MonoTouch.Dialog-1":
				case "MonoTouch.NUnitLite":
				case "monotouch":
				case "Xamarin.iOS":
				case "Xamarin.TVOS":
				case "Xamarin.WatchOS":
					// product assembly (use a different check than SDK/BCL)
					break;
				case "Newtonsoft.Json":
				case "Xamarin.iOS.Tasks":
				case "Xamarin.iOS.Tasks.Core":
				case "Xamarin.ObjcBinding.Tasks":
				case "Xamarin.MacDev":
				case "Xamarin.MacDev.Tasks":
				case "Xamarin.MacDev.Tasks.Core":
				case "Xamarin.Analysis.Tasks":
					// other stuff that is not part of the SDK but shipped in the same 2.1 directory
					if (path != ClassicPath)
						Assert.Fail (aname);
					break;
				default:
					Assert.IsTrue (ProfilePoker.IsWellKnownSdk (aname), aname);
					break;
				}
			}
		}

		void REPL (string path)
		{
			var repl = Path.Combine (path, "repl");
			foreach (var file in Directory.GetFiles (repl, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				switch (aname) {
				// sub-list that are SDK assemblies
				case "mscorlib":
				case "System":
				case "System.Core":
				case "System.Xml":
				case "Mono.CSharp":
					Assert.IsTrue (ProfilePoker.IsWellKnownSdk (aname), aname);
					break;
				default:
					Assert.Fail (aname);
					break;
				}
			}
		}

		void Facades (string path)
		{
			var facades = Path.Combine (path, "Facades");
			foreach (var file in Directory.GetFiles (facades, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				Assert.IsTrue (ProfilePoker.IsWellKnownSdk (aname), aname);
			}
		}

		[Test]
		public void iOS_Classic ()
		{
			BCL (ClassicPath);
			REPL (ClassicPath);
			Facades (ClassicPath);
		}

		[Test]
		public void iOS_Unified ()
		{
			BCL (UnifiedPath);
			REPL (UnifiedPath);
			Facades (UnifiedPath);
		}

		[Test]
		public void tvOS ()
		{
			BCL (tvOSPath);
			REPL (tvOSPath);
			Facades (tvOSPath);
		}

		[Test]
		public void watchOS ()
		{
			BCL (watchOSPath);
			REPL (watchOSPath);
			Facades (watchOSPath);
		}
	}
}