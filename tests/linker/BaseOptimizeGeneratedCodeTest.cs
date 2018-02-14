//
// Unit tests for the linker's OptimizeGeneratedCodeSubStep.
//
// This class is included in both Xamarin.Mac and Xamarin.iOS's link all tests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace Linker.Shared
{
	[Preserve (AllMembers = true)]
	public abstract class BaseOptimizeGeneratedCodeTest
	{
#if LINKALL
		[Test]
		public void SetupBlock_CustomDelegate ()
		{
			int counter = 0;
			var action = new Action (() => counter++);
			Custom (action);
			CustomWithAttribute (action);
			Assert.AreEqual (2, counter, "Counter");
		}

		delegate void CustomDelegate (IntPtr block);

		[BindingImpl (BindingImplOptions.Optimizable)]
		unsafe void Custom (Action callback)
		{
			BlockLiteral block = new BlockLiteral ();
			CustomDelegate block_callback = BlockCallback;
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[UserDelegateType (typeof (Action))]
		delegate void CustomDelegateWithAttribute (IntPtr block);

		[BindingImpl (BindingImplOptions.Optimizable)]
		unsafe void CustomWithAttribute (Action callback)
		{
			BlockLiteral block = new BlockLiteral ();
			CustomDelegateWithAttribute block_callback = BlockCallback;
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[Test]
		public void SetupBlockPerfTest ()
		{
			const int iterations = 5000;

			// Set the XAMARIN_IOS_SKIP_BLOCK_CHECK environment variable to skip a few expensive validation checks done in the simulator
			var skipBlockCheck = Environment.GetEnvironmentVariable ("XAMARIN_IOS_SKIP_BLOCK_CHECK");
			try {
				Environment.SetEnvironmentVariable ("XAMARIN_IOS_SKIP_BLOCK_CHECK", "1");
				var unoptimizedCounter = 0;
				var unoptimizedAction = new Action (() => unoptimizedCounter++);

				var optimizedCounter = 0;
				var optimizedAction = new Action (() => optimizedCounter++);

				// Warm up
				SetupBlockOptimized (optimizedAction);
				SetupBlockUnoptimized (unoptimizedAction);

				// Run unoptimized
				var unoptimizedWatch = System.Diagnostics.Stopwatch.StartNew ();
				unoptimizedCounter = 0;
				for (var i = 0; i < iterations; i++)
					SetupBlockUnoptimized (unoptimizedAction);
				unoptimizedWatch.Stop ();
				Assert.AreEqual (iterations, unoptimizedCounter, "Unoptimized Counter");

				// Run optimized
				var optimizedWatch = System.Diagnostics.Stopwatch.StartNew ();
				optimizedCounter = 0;
				for (var i = 0; i < iterations; i++)
					SetupBlockOptimized (optimizedAction);
				optimizedWatch.Stop ();
				Assert.AreEqual (iterations, optimizedCounter, "Optimized Counter");

				//Console.WriteLine ("Optimized: {0} ms", optimizedWatch.ElapsedMilliseconds);
				//Console.WriteLine ("Unoptimized: {0} ms", unoptimizedWatch.ElapsedMilliseconds);
				//Console.WriteLine ("Speedup: {0}x", unoptimizedWatch.ElapsedTicks / (double) optimizedWatch.ElapsedTicks);
				// My testing found a 12-16x speedup on device and a 15-20x speedup in the simulator/desktop.
				// Setting to 8 to have a margin for random stuff happening, but this may still have to be adjusted.
				var speedup = 8;
				Assert.That (unoptimizedWatch.ElapsedTicks / (double) optimizedWatch.ElapsedTicks, Is.GreaterThan (speedup), $"At least {speedup}x speedup");
			} finally {
				Environment.SetEnvironmentVariable ("XAMARIN_IOS_SKIP_BLOCK_CHECK", skipBlockCheck);
			}
		}

		[Test]
		public void SetupBlockCodeTest ()
		{
			int counter = 0;
			var action = new Action (() => counter++);
			SetupBlockOptimized_SpecificArgument (block_callback, action);
			SetupBlockOptimized_SpecificArgument (action, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, 2, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, 2, 3, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, 2, 3, 4, block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, 2, 3, 4, 5, block_callback: block_callback);
			SetupBlockOptimized_SpecificArgument (action, 0, 1, 2, 3, 4, 5, 6, block_callback: block_callback);
			SetupBlockOptimized_LoadField (action);
			SetupBlockOptimized_LoadStaticField (action);
			SetupBlockOptimized_CallVirtualMethod (action);
			SetupBlockOptimized_CallStaticMethod (action);
			SetupBlockOptimized_LoadLocalVariable0 (action);
			SetupBlockOptimized_LoadLocalVariable1 (action);
			SetupBlockOptimized_LoadLocalVariable2 (action);
			SetupBlockOptimized_LoadLocalVariable3 (action);
			SetupBlockOptimized_LoadLocalVariable4 (action);
			SetupBlockOptimized_LoadLocalVariable (action);

			Assert.AreEqual (19, counter, "Counter");
		}

		[DllImport (Constants.libcLibrary)]
		extern static void dispatch_sync (IntPtr queue, IntPtr block);
		static Action<IntPtr> block_callback = BlockCallback;

		[MonoPInvokeCallback (typeof (Action<IntPtr>))]
		unsafe static void BlockCallback (IntPtr block)
		{
			var descriptor = (BlockLiteral*) block;
			var del = (Action) (descriptor->Target);
			del ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		unsafe void SetupBlockOptimized (Action callback)
		{
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		unsafe void SetupBlockUnoptimized (Action callback)
		{
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action<IntPtr> block_callback, Action callback)
		{
			// ldarg_0
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, Action<IntPtr> block_callback)
		{
			// ldarg_1
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, int dummy1, Action<IntPtr> block_callback)
		{
			// ldarg_2
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, int dummy1, int dummy2, Action<IntPtr> block_callback)
		{
			// ldarg_3
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, int dummy1, int dummy2, int dummy3, Action<IntPtr> block_callback)
		{
			// ldarg_S
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, int dummy1, int dummy2, int dummy3, int dummy4, Action<IntPtr> block_callback)
		{
			// ldarg_S
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (Action callback, int dummy1, int dummy2, int dummy3, int dummy4, int dummy5, Action<IntPtr> block_callback)
		{
			// ldarg_S
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_SpecificArgument (
			Action callback,
			int dummy000 = 0, int dummy010 = 0, int dummy020 = 0, int dummy030 = 0, int dummy040 = 0, int dummy050 = 0, int dummy060 = 0, int dummy070 = 0, int dummy080 = 0, int dummy090 = 0,
			int dummy001 = 0, int dummy011 = 0, int dummy021 = 0, int dummy031 = 0, int dummy041 = 0, int dummy051 = 0, int dummy061 = 0, int dummy071 = 0, int dummy081 = 0, int dummy091 = 0,
			int dummy002 = 0, int dummy012 = 0, int dummy022 = 0, int dummy032 = 0, int dummy042 = 0, int dummy052 = 0, int dummy062 = 0, int dummy072 = 0, int dummy082 = 0, int dummy092 = 0,
			int dummy003 = 0, int dummy013 = 0, int dummy023 = 0, int dummy033 = 0, int dummy043 = 0, int dummy053 = 0, int dummy063 = 0, int dummy073 = 0, int dummy083 = 0, int dummy093 = 0,
			int dummy004 = 0, int dummy014 = 0, int dummy024 = 0, int dummy034 = 0, int dummy044 = 0, int dummy054 = 0, int dummy064 = 0, int dummy074 = 0, int dummy084 = 0, int dummy094 = 0,
			int dummy005 = 0, int dummy015 = 0, int dummy025 = 0, int dummy035 = 0, int dummy045 = 0, int dummy055 = 0, int dummy065 = 0, int dummy075 = 0, int dummy085 = 0, int dummy095 = 0,
			int dummy006 = 0, int dummy016 = 0, int dummy026 = 0, int dummy036 = 0, int dummy046 = 0, int dummy056 = 0, int dummy066 = 0, int dummy076 = 0, int dummy086 = 0, int dummy096 = 0,
			int dummy007 = 0, int dummy017 = 0, int dummy027 = 0, int dummy037 = 0, int dummy047 = 0, int dummy057 = 0, int dummy067 = 0, int dummy077 = 0, int dummy087 = 0, int dummy097 = 0,
			int dummy008 = 0, int dummy018 = 0, int dummy028 = 0, int dummy038 = 0, int dummy048 = 0, int dummy058 = 0, int dummy068 = 0, int dummy078 = 0, int dummy088 = 0, int dummy098 = 0,
			int dummy009 = 0, int dummy019 = 0, int dummy029 = 0, int dummy039 = 0, int dummy049 = 0, int dummy059 = 0, int dummy069 = 0, int dummy079 = 0, int dummy089 = 0, int dummy099 = 0,

			int dummy100 = 0, int dummy110 = 0, int dummy120 = 0, int dummy130 = 0, int dummy140 = 0, int dummy150 = 0, int dummy160 = 0, int dummy170 = 0, int dummy180 = 0, int dummy190 = 0,
			int dummy101 = 0, int dummy111 = 0, int dummy121 = 0, int dummy131 = 0, int dummy141 = 0, int dummy151 = 0, int dummy161 = 0, int dummy171 = 0, int dummy181 = 0, int dummy191 = 0,
			int dummy102 = 0, int dummy112 = 0, int dummy122 = 0, int dummy132 = 0, int dummy142 = 0, int dummy152 = 0, int dummy162 = 0, int dummy172 = 0, int dummy182 = 0, int dummy192 = 0,
			int dummy103 = 0, int dummy113 = 0, int dummy123 = 0, int dummy133 = 0, int dummy143 = 0, int dummy153 = 0, int dummy163 = 0, int dummy173 = 0, int dummy183 = 0, int dummy193 = 0,
			int dummy104 = 0, int dummy114 = 0, int dummy124 = 0, int dummy134 = 0, int dummy144 = 0, int dummy154 = 0, int dummy164 = 0, int dummy174 = 0, int dummy184 = 0, int dummy194 = 0,
			int dummy105 = 0, int dummy115 = 0, int dummy125 = 0, int dummy135 = 0, int dummy145 = 0, int dummy155 = 0, int dummy165 = 0, int dummy175 = 0, int dummy185 = 0, int dummy195 = 0,
			int dummy106 = 0, int dummy116 = 0, int dummy126 = 0, int dummy136 = 0, int dummy146 = 0, int dummy156 = 0, int dummy166 = 0, int dummy176 = 0, int dummy186 = 0, int dummy196 = 0,
			int dummy107 = 0, int dummy117 = 0, int dummy127 = 0, int dummy137 = 0, int dummy147 = 0, int dummy157 = 0, int dummy167 = 0, int dummy177 = 0, int dummy187 = 0, int dummy197 = 0,
			int dummy108 = 0, int dummy118 = 0, int dummy128 = 0, int dummy138 = 0, int dummy148 = 0, int dummy158 = 0, int dummy168 = 0, int dummy178 = 0, int dummy188 = 0, int dummy198 = 0,
			int dummy109 = 0, int dummy119 = 0, int dummy129 = 0, int dummy139 = 0, int dummy149 = 0, int dummy159 = 0, int dummy169 = 0, int dummy179 = 0, int dummy189 = 0, int dummy199 = 0,

			int dummy200 = 0, int dummy210 = 0, int dummy220 = 0, int dummy230 = 0, int dummy240 = 0, int dummy250 = 0, int dummy260 = 0, int dummy270 = 0, int dummy280 = 0, int dummy290 = 0,
			int dummy201 = 0, int dummy211 = 0, int dummy221 = 0, int dummy231 = 0, int dummy241 = 0, int dummy251 = 0, int dummy261 = 0, int dummy271 = 0, int dummy281 = 0, int dummy291 = 0,
			int dummy202 = 0, int dummy212 = 0, int dummy222 = 0, int dummy232 = 0, int dummy242 = 0, int dummy252 = 0, int dummy262 = 0, int dummy272 = 0, int dummy282 = 0, int dummy292 = 0,
			int dummy203 = 0, int dummy213 = 0, int dummy223 = 0, int dummy233 = 0, int dummy243 = 0, int dummy253 = 0, int dummy263 = 0, int dummy273 = 0, int dummy283 = 0, int dummy293 = 0,
			int dummy204 = 0, int dummy214 = 0, int dummy224 = 0, int dummy234 = 0, int dummy244 = 0, int dummy254 = 0, int dummy264 = 0, int dummy274 = 0, int dummy284 = 0, int dummy294 = 0,
			int dummy205 = 0, int dummy215 = 0, int dummy225 = 0, int dummy235 = 0, int dummy245 = 0, int dummy255 = 0, int dummy265 = 0, int dummy275 = 0, int dummy285 = 0, int dummy295 = 0,
			int dummy206 = 0, int dummy216 = 0, int dummy226 = 0, int dummy236 = 0, int dummy246 = 0, int dummy256 = 0, int dummy266 = 0, int dummy276 = 0, int dummy286 = 0, int dummy296 = 0,
			int dummy207 = 0, int dummy217 = 0, int dummy227 = 0, int dummy237 = 0, int dummy247 = 0, int dummy257 = 0, int dummy267 = 0, int dummy277 = 0, int dummy287 = 0, int dummy297 = 0,
			int dummy208 = 0, int dummy218 = 0, int dummy228 = 0, int dummy238 = 0, int dummy248 = 0, int dummy258 = 0, int dummy268 = 0, int dummy278 = 0, int dummy288 = 0, int dummy298 = 0,
			int dummy209 = 0, int dummy219 = 0, int dummy229 = 0, int dummy239 = 0, int dummy249 = 0, int dummy259 = 0, int dummy269 = 0, int dummy279 = 0, int dummy289 = 0, int dummy299 = 0,

			Action<IntPtr> block_callback = null
		)
		{
			// ldarg
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		Action<IntPtr> block_callback_instance_field;
		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadField (Action callback)
		{
			// ldfld
			BlockLiteral block = new BlockLiteral ();
			block_callback_instance_field = block_callback;
			block.SetupBlock (block_callback_instance_field, callback);
			block_callback_instance_field = null;
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadStaticField (Action callback)
		{
			// ldsfld
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		protected virtual Action<IntPtr> GetBlockCallbackInstance ()
		{
			return block_callback;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_CallVirtualMethod (Action callback)
		{
			// calli
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (GetBlockCallbackInstance (), callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		static Action<IntPtr> GetBlockCallbackStatic ()
		{
			return block_callback;
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_CallStaticMethod (Action callback)
		{
			// call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (GetBlockCallbackStatic (), callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable0 (Action callback)
		{
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc_0

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable1 (Action callback)
		{
			int dummy0 = 0;
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc_1

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();

			// Use the variables so that the compiler doesn't optimize them away
			GC.KeepAlive (dummy0);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable2 (Action callback)
		{
			int dummy0 = 0, dummy1 = 0;
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc_2

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();

			// Use the variables so that the compiler doesn't optimize them away
			GC.KeepAlive (dummy0);
			GC.KeepAlive (dummy1);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable3 (Action callback)
		{
			int dummy0 = 0, dummy1 = 0, dummy2 = 0;
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc_3

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();

			// Use the variables so that the compiler doesn't optimize them away
			GC.KeepAlive (dummy0);
			GC.KeepAlive (dummy1);
			GC.KeepAlive (dummy2);
		}
		
		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable4 (Action callback)
		{
			int dummy0 = 0, dummy1 = 0, dummy2 = 0, dummy3 = 0;
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc_S

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();

			// Use the variables so that the compiler doesn't optimize them away
			GC.KeepAlive (dummy0);
			GC.KeepAlive (dummy1);
			GC.KeepAlive (dummy2);
			GC.KeepAlive (dummy3);
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		void SetupBlockOptimized_LoadLocalVariable (Action callback)
		{
			int dummy000 = 0, dummy010 = 0, dummy020 = 0, dummy030 = 0, dummy040 = 0, dummy050 = 0, dummy060 = 0, dummy070 = 0, dummy080 = 0, dummy090 = 0;
			int dummy001 = 0, dummy011 = 0, dummy021 = 0, dummy031 = 0, dummy041 = 0, dummy051 = 0, dummy061 = 0, dummy071 = 0, dummy081 = 0, dummy091 = 0;
			int dummy002 = 0, dummy012 = 0, dummy022 = 0, dummy032 = 0, dummy042 = 0, dummy052 = 0, dummy062 = 0, dummy072 = 0, dummy082 = 0, dummy092 = 0;
			int dummy003 = 0, dummy013 = 0, dummy023 = 0, dummy033 = 0, dummy043 = 0, dummy053 = 0, dummy063 = 0, dummy073 = 0, dummy083 = 0, dummy093 = 0;
			int dummy004 = 0, dummy014 = 0, dummy024 = 0, dummy034 = 0, dummy044 = 0, dummy054 = 0, dummy064 = 0, dummy074 = 0, dummy084 = 0, dummy094 = 0;
			int dummy005 = 0, dummy015 = 0, dummy025 = 0, dummy035 = 0, dummy045 = 0, dummy055 = 0, dummy065 = 0, dummy075 = 0, dummy085 = 0, dummy095 = 0;
			int dummy006 = 0, dummy016 = 0, dummy026 = 0, dummy036 = 0, dummy046 = 0, dummy056 = 0, dummy066 = 0, dummy076 = 0, dummy086 = 0, dummy096 = 0;
			int dummy007 = 0, dummy017 = 0, dummy027 = 0, dummy037 = 0, dummy047 = 0, dummy057 = 0, dummy067 = 0, dummy077 = 0, dummy087 = 0, dummy097 = 0;
			int dummy008 = 0, dummy018 = 0, dummy028 = 0, dummy038 = 0, dummy048 = 0, dummy058 = 0, dummy068 = 0, dummy078 = 0, dummy088 = 0, dummy098 = 0;
			int dummy009 = 0, dummy019 = 0, dummy029 = 0, dummy039 = 0, dummy049 = 0, dummy059 = 0, dummy069 = 0, dummy079 = 0, dummy089 = 0, dummy099 = 0;
			int dummy100 = 0, dummy110 = 0, dummy120 = 0, dummy130 = 0, dummy140 = 0, dummy150 = 0, dummy160 = 0, dummy170 = 0, dummy180 = 0, dummy190 = 0;
			int dummy101 = 0, dummy111 = 0, dummy121 = 0, dummy131 = 0, dummy141 = 0, dummy151 = 0, dummy161 = 0, dummy171 = 0, dummy181 = 0, dummy191 = 0;
			int dummy102 = 0, dummy112 = 0, dummy122 = 0, dummy132 = 0, dummy142 = 0, dummy152 = 0, dummy162 = 0, dummy172 = 0, dummy182 = 0, dummy192 = 0;
			int dummy103 = 0, dummy113 = 0, dummy123 = 0, dummy133 = 0, dummy143 = 0, dummy153 = 0, dummy163 = 0, dummy173 = 0, dummy183 = 0, dummy193 = 0;
			int dummy104 = 0, dummy114 = 0, dummy124 = 0, dummy134 = 0, dummy144 = 0, dummy154 = 0, dummy164 = 0, dummy174 = 0, dummy184 = 0, dummy194 = 0;
			int dummy105 = 0, dummy115 = 0, dummy125 = 0, dummy135 = 0, dummy145 = 0, dummy155 = 0, dummy165 = 0, dummy175 = 0, dummy185 = 0, dummy195 = 0;
			int dummy106 = 0, dummy116 = 0, dummy126 = 0, dummy136 = 0, dummy146 = 0, dummy156 = 0, dummy166 = 0, dummy176 = 0, dummy186 = 0, dummy196 = 0;
			int dummy107 = 0, dummy117 = 0, dummy127 = 0, dummy137 = 0, dummy147 = 0, dummy157 = 0, dummy167 = 0, dummy177 = 0, dummy187 = 0, dummy197 = 0;
			int dummy108 = 0, dummy118 = 0, dummy128 = 0, dummy138 = 0, dummy148 = 0, dummy158 = 0, dummy168 = 0, dummy178 = 0, dummy188 = 0, dummy198 = 0;
			int dummy109 = 0, dummy119 = 0, dummy129 = 0, dummy139 = 0, dummy149 = 0, dummy159 = 0, dummy169 = 0, dummy179 = 0, dummy189 = 0, dummy199 = 0;
			int dummy200 = 0, dummy210 = 0, dummy220 = 0, dummy230 = 0, dummy240 = 0, dummy250 = 0, dummy260 = 0, dummy270 = 0, dummy280 = 0, dummy290 = 0;
			int dummy201 = 0, dummy211 = 0, dummy221 = 0, dummy231 = 0, dummy241 = 0, dummy251 = 0, dummy261 = 0, dummy271 = 0, dummy281 = 0, dummy291 = 0;
			int dummy202 = 0, dummy212 = 0, dummy222 = 0, dummy232 = 0, dummy242 = 0, dummy252 = 0, dummy262 = 0, dummy272 = 0, dummy282 = 0, dummy292 = 0;
			int dummy203 = 0, dummy213 = 0, dummy223 = 0, dummy233 = 0, dummy243 = 0, dummy253 = 0, dummy263 = 0, dummy273 = 0, dummy283 = 0, dummy293 = 0;
			int dummy204 = 0, dummy214 = 0, dummy224 = 0, dummy234 = 0, dummy244 = 0, dummy254 = 0, dummy264 = 0, dummy274 = 0, dummy284 = 0, dummy294 = 0;
			int dummy205 = 0, dummy215 = 0, dummy225 = 0, dummy235 = 0, dummy245 = 0, dummy255 = 0, dummy265 = 0, dummy275 = 0, dummy285 = 0, dummy295 = 0;
			int dummy206 = 0, dummy216 = 0, dummy226 = 0, dummy236 = 0, dummy246 = 0, dummy256 = 0, dummy266 = 0, dummy276 = 0, dummy286 = 0, dummy296 = 0;
			int dummy207 = 0, dummy217 = 0, dummy227 = 0, dummy237 = 0, dummy247 = 0, dummy257 = 0, dummy267 = 0, dummy277 = 0, dummy287 = 0, dummy297 = 0;
			int dummy208 = 0, dummy218 = 0, dummy228 = 0, dummy238 = 0, dummy248 = 0, dummy258 = 0, dummy268 = 0, dummy278 = 0, dummy288 = 0, dummy298 = 0;
			int dummy209 = 0, dummy219 = 0, dummy229 = 0, dummy239 = 0, dummy249 = 0, dummy259 = 0, dummy269 = 0, dummy279 = 0, dummy289 = 0, dummy299 = 0;
			var block_callback = BaseOptimizeGeneratedCodeTest.block_callback; // ldloc

			// Load the trampoline from a instance method call
			BlockLiteral block = new BlockLiteral ();
			block.SetupBlock (block_callback, callback);
			Bindings.Test.CFunctions.x_call_block (ref block);
			block.CleanupBlock ();

			// Use the variables so that the compiler doesn't optimize them away
			GC.KeepAlive (dummy000); GC.KeepAlive (dummy010); GC.KeepAlive (dummy020); GC.KeepAlive (dummy030); GC.KeepAlive (dummy040); GC.KeepAlive (dummy050); GC.KeepAlive (dummy060); GC.KeepAlive (dummy070); GC.KeepAlive (dummy080); GC.KeepAlive (dummy090);
			GC.KeepAlive (dummy001); GC.KeepAlive (dummy011); GC.KeepAlive (dummy021); GC.KeepAlive (dummy031); GC.KeepAlive (dummy041); GC.KeepAlive (dummy051); GC.KeepAlive (dummy061); GC.KeepAlive (dummy071); GC.KeepAlive (dummy081); GC.KeepAlive (dummy091);
			GC.KeepAlive (dummy002); GC.KeepAlive (dummy012); GC.KeepAlive (dummy022); GC.KeepAlive (dummy032); GC.KeepAlive (dummy042); GC.KeepAlive (dummy052); GC.KeepAlive (dummy062); GC.KeepAlive (dummy072); GC.KeepAlive (dummy082); GC.KeepAlive (dummy092);
			GC.KeepAlive (dummy003); GC.KeepAlive (dummy013); GC.KeepAlive (dummy023); GC.KeepAlive (dummy033); GC.KeepAlive (dummy043); GC.KeepAlive (dummy053); GC.KeepAlive (dummy063); GC.KeepAlive (dummy073); GC.KeepAlive (dummy083); GC.KeepAlive (dummy093);
			GC.KeepAlive (dummy004); GC.KeepAlive (dummy014); GC.KeepAlive (dummy024); GC.KeepAlive (dummy034); GC.KeepAlive (dummy044); GC.KeepAlive (dummy054); GC.KeepAlive (dummy064); GC.KeepAlive (dummy074); GC.KeepAlive (dummy084); GC.KeepAlive (dummy094);
			GC.KeepAlive (dummy005); GC.KeepAlive (dummy015); GC.KeepAlive (dummy025); GC.KeepAlive (dummy035); GC.KeepAlive (dummy045); GC.KeepAlive (dummy055); GC.KeepAlive (dummy065); GC.KeepAlive (dummy075); GC.KeepAlive (dummy085); GC.KeepAlive (dummy095);
			GC.KeepAlive (dummy006); GC.KeepAlive (dummy016); GC.KeepAlive (dummy026); GC.KeepAlive (dummy036); GC.KeepAlive (dummy046); GC.KeepAlive (dummy056); GC.KeepAlive (dummy066); GC.KeepAlive (dummy076); GC.KeepAlive (dummy086); GC.KeepAlive (dummy096);
			GC.KeepAlive (dummy007); GC.KeepAlive (dummy017); GC.KeepAlive (dummy027); GC.KeepAlive (dummy037); GC.KeepAlive (dummy047); GC.KeepAlive (dummy057); GC.KeepAlive (dummy067); GC.KeepAlive (dummy077); GC.KeepAlive (dummy087); GC.KeepAlive (dummy097);
			GC.KeepAlive (dummy008); GC.KeepAlive (dummy018); GC.KeepAlive (dummy028); GC.KeepAlive (dummy038); GC.KeepAlive (dummy048); GC.KeepAlive (dummy058); GC.KeepAlive (dummy068); GC.KeepAlive (dummy078); GC.KeepAlive (dummy088); GC.KeepAlive (dummy098);
			GC.KeepAlive (dummy009); GC.KeepAlive (dummy019); GC.KeepAlive (dummy029); GC.KeepAlive (dummy039); GC.KeepAlive (dummy049); GC.KeepAlive (dummy059); GC.KeepAlive (dummy069); GC.KeepAlive (dummy079); GC.KeepAlive (dummy089); GC.KeepAlive (dummy099);
			GC.KeepAlive (dummy100); GC.KeepAlive (dummy110); GC.KeepAlive (dummy120); GC.KeepAlive (dummy130); GC.KeepAlive (dummy140); GC.KeepAlive (dummy150); GC.KeepAlive (dummy160); GC.KeepAlive (dummy170); GC.KeepAlive (dummy180); GC.KeepAlive (dummy190);
			GC.KeepAlive (dummy101); GC.KeepAlive (dummy111); GC.KeepAlive (dummy121); GC.KeepAlive (dummy131); GC.KeepAlive (dummy141); GC.KeepAlive (dummy151); GC.KeepAlive (dummy161); GC.KeepAlive (dummy171); GC.KeepAlive (dummy181); GC.KeepAlive (dummy191);
			GC.KeepAlive (dummy102); GC.KeepAlive (dummy112); GC.KeepAlive (dummy122); GC.KeepAlive (dummy132); GC.KeepAlive (dummy142); GC.KeepAlive (dummy152); GC.KeepAlive (dummy162); GC.KeepAlive (dummy172); GC.KeepAlive (dummy182); GC.KeepAlive (dummy192);
			GC.KeepAlive (dummy103); GC.KeepAlive (dummy113); GC.KeepAlive (dummy123); GC.KeepAlive (dummy133); GC.KeepAlive (dummy143); GC.KeepAlive (dummy153); GC.KeepAlive (dummy163); GC.KeepAlive (dummy173); GC.KeepAlive (dummy183); GC.KeepAlive (dummy193);
			GC.KeepAlive (dummy104); GC.KeepAlive (dummy114); GC.KeepAlive (dummy124); GC.KeepAlive (dummy134); GC.KeepAlive (dummy144); GC.KeepAlive (dummy154); GC.KeepAlive (dummy164); GC.KeepAlive (dummy174); GC.KeepAlive (dummy184); GC.KeepAlive (dummy194);
			GC.KeepAlive (dummy105); GC.KeepAlive (dummy115); GC.KeepAlive (dummy125); GC.KeepAlive (dummy135); GC.KeepAlive (dummy145); GC.KeepAlive (dummy155); GC.KeepAlive (dummy165); GC.KeepAlive (dummy175); GC.KeepAlive (dummy185); GC.KeepAlive (dummy195);
			GC.KeepAlive (dummy106); GC.KeepAlive (dummy116); GC.KeepAlive (dummy126); GC.KeepAlive (dummy136); GC.KeepAlive (dummy146); GC.KeepAlive (dummy156); GC.KeepAlive (dummy166); GC.KeepAlive (dummy176); GC.KeepAlive (dummy186); GC.KeepAlive (dummy196);
			GC.KeepAlive (dummy107); GC.KeepAlive (dummy117); GC.KeepAlive (dummy127); GC.KeepAlive (dummy137); GC.KeepAlive (dummy147); GC.KeepAlive (dummy157); GC.KeepAlive (dummy167); GC.KeepAlive (dummy177); GC.KeepAlive (dummy187); GC.KeepAlive (dummy197);
			GC.KeepAlive (dummy108); GC.KeepAlive (dummy118); GC.KeepAlive (dummy128); GC.KeepAlive (dummy138); GC.KeepAlive (dummy148); GC.KeepAlive (dummy158); GC.KeepAlive (dummy168); GC.KeepAlive (dummy178); GC.KeepAlive (dummy188); GC.KeepAlive (dummy198);
			GC.KeepAlive (dummy109); GC.KeepAlive (dummy119); GC.KeepAlive (dummy129); GC.KeepAlive (dummy139); GC.KeepAlive (dummy149); GC.KeepAlive (dummy159); GC.KeepAlive (dummy169); GC.KeepAlive (dummy179); GC.KeepAlive (dummy189); GC.KeepAlive (dummy199);
			GC.KeepAlive (dummy200); GC.KeepAlive (dummy210); GC.KeepAlive (dummy220); GC.KeepAlive (dummy230); GC.KeepAlive (dummy240); GC.KeepAlive (dummy250); GC.KeepAlive (dummy260); GC.KeepAlive (dummy270); GC.KeepAlive (dummy280); GC.KeepAlive (dummy290);
			GC.KeepAlive (dummy201); GC.KeepAlive (dummy211); GC.KeepAlive (dummy221); GC.KeepAlive (dummy231); GC.KeepAlive (dummy241); GC.KeepAlive (dummy251); GC.KeepAlive (dummy261); GC.KeepAlive (dummy271); GC.KeepAlive (dummy281); GC.KeepAlive (dummy291);
			GC.KeepAlive (dummy202); GC.KeepAlive (dummy212); GC.KeepAlive (dummy222); GC.KeepAlive (dummy232); GC.KeepAlive (dummy242); GC.KeepAlive (dummy252); GC.KeepAlive (dummy262); GC.KeepAlive (dummy272); GC.KeepAlive (dummy282); GC.KeepAlive (dummy292);
			GC.KeepAlive (dummy203); GC.KeepAlive (dummy213); GC.KeepAlive (dummy223); GC.KeepAlive (dummy233); GC.KeepAlive (dummy243); GC.KeepAlive (dummy253); GC.KeepAlive (dummy263); GC.KeepAlive (dummy273); GC.KeepAlive (dummy283); GC.KeepAlive (dummy293);
			GC.KeepAlive (dummy204); GC.KeepAlive (dummy214); GC.KeepAlive (dummy224); GC.KeepAlive (dummy234); GC.KeepAlive (dummy244); GC.KeepAlive (dummy254); GC.KeepAlive (dummy264); GC.KeepAlive (dummy274); GC.KeepAlive (dummy284); GC.KeepAlive (dummy294);
			GC.KeepAlive (dummy205); GC.KeepAlive (dummy215); GC.KeepAlive (dummy225); GC.KeepAlive (dummy235); GC.KeepAlive (dummy245); GC.KeepAlive (dummy255); GC.KeepAlive (dummy265); GC.KeepAlive (dummy275); GC.KeepAlive (dummy285); GC.KeepAlive (dummy295);
			GC.KeepAlive (dummy206); GC.KeepAlive (dummy216); GC.KeepAlive (dummy226); GC.KeepAlive (dummy236); GC.KeepAlive (dummy246); GC.KeepAlive (dummy256); GC.KeepAlive (dummy266); GC.KeepAlive (dummy276); GC.KeepAlive (dummy286); GC.KeepAlive (dummy296);
			GC.KeepAlive (dummy207); GC.KeepAlive (dummy217); GC.KeepAlive (dummy227); GC.KeepAlive (dummy237); GC.KeepAlive (dummy247); GC.KeepAlive (dummy257); GC.KeepAlive (dummy267); GC.KeepAlive (dummy277); GC.KeepAlive (dummy287); GC.KeepAlive (dummy297);
			GC.KeepAlive (dummy208); GC.KeepAlive (dummy218); GC.KeepAlive (dummy228); GC.KeepAlive (dummy238); GC.KeepAlive (dummy248); GC.KeepAlive (dummy258); GC.KeepAlive (dummy268); GC.KeepAlive (dummy278); GC.KeepAlive (dummy288); GC.KeepAlive (dummy298);
			GC.KeepAlive (dummy209); GC.KeepAlive (dummy219); GC.KeepAlive (dummy229); GC.KeepAlive (dummy239); GC.KeepAlive (dummy249); GC.KeepAlive (dummy259); GC.KeepAlive (dummy269); GC.KeepAlive (dummy279); GC.KeepAlive (dummy289); GC.KeepAlive (dummy299);
		}
#endif // LINKALL
	}
}
