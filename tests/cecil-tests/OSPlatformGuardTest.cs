// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin;
using Xamarin.Utils;

namespace Cecil.Tests {
	[TestFixture]
	public partial class OSPlatformGuardTest {
		[Test]
		public void RedundantGuards ()
		{
			var failures = new Dictionary<string, string> ();
			foreach (var info in Helper.NetPlatformImplementationAssemblyDefinitions) {
				foreach (var failure in FindRedundantGuards (info.Assembly, info.Platform))
					failures [failure.Key] = failure.Value;
			}

			Helper.AssertFailures (failures, knownFailuresRedundantOSPlatformGuards, nameof (knownFailuresRedundantOSPlatformGuards), "Redundant OS platform version guards");
		}

		static IEnumerable<CustomAttribute> GetGuardAttributes (MethodDefinition method)
		{
			var attributes = method.CustomAttributes.AsEnumerable ();
			if (method.IsGetter) {
				var property = method.DeclaringType.Properties.Single (v => v.GetMethod == method);
				attributes = attributes.Concat (property.CustomAttributes);
			}
			return attributes.Where (v => v.AttributeType.Is ("System.Runtime.Versioning", "SupportedOSPlatformGuardAttribute"));
		}

		static Dictionary<string, string> FindRedundantGuards (AssemblyDefinition assembly, ApplePlatform platform)
		{
			var guards = assembly.EnumerateMethods ()
				.Select (v => (Method: v, Attributes: GetGuardAttributes (v).ToArray ()))
				.Where (v => v.Attributes.Length > 0)
				.ToDictionary (v => v.Method.FullName);
			var minimumVersion = NormalizeVersion (SdkVersions.GetMinVersion (platform));
			var failures = new Dictionary<string, string> ();

			foreach (var method in assembly.EnumerateMethods (v => v.HasBody)) {
				var branchTargets = new HashSet<Instruction> ();
				foreach (var instruction in method.Body.Instructions) {
					if (instruction.Operand is Instruction target)
						branchTargets.Add (target);
					else if (instruction.Operand is Instruction [] targets)
						branchTargets.UnionWith (targets);
				}
				foreach (var handler in method.Body.ExceptionHandlers) {
					branchTargets.Add (handler.HandlerStart);
					if (handler.FilterStart is not null)
						branchTargets.Add (handler.FilterStart);
				}

				foreach (var instruction in method.Body.Instructions) {
					if (instruction.OpCode != OpCodes.Call && instruction.OpCode != OpCodes.Callvirt)
						continue;
					if (instruction.Operand is not MethodReference calledMethod || calledMethod.ReturnType.MetadataType != MetadataType.Boolean)
						continue;

					Version? version = null;
					if (calledMethod.DeclaringType.Is ("System", "OperatingSystem")) {
						var arguments = GetConstantArguments (instruction, calledMethod.Parameters.Count, branchTargets);
						if (arguments is null)
							continue;
						var argumentOffset = 0;
						string? platformName;
						switch (calledMethod.Name) {
						case "IsIOSVersionAtLeast":
							platformName = platform == ApplePlatform.MacCatalyst ? "maccatalyst" : "ios";
							break;
						case "IsTvOSVersionAtLeast":
							platformName = "tvos";
							break;
						case "IsMacOSVersionAtLeast":
							platformName = "macos";
							break;
						case "IsMacCatalystVersionAtLeast":
							platformName = "maccatalyst";
							break;
						case "IsOSPlatformVersionAtLeast":
							platformName = arguments [0].Operand as string;
							argumentOffset = 1;
							break;
						default:
							continue;
						}
						if (!OSPlatformAttributeExtensions.TryParse (platformName, out ApplePlatform? guardPlatform, out var platformVersion) || platformVersion is not null)
							continue;
						if (calledMethod.Name == "IsOSPlatformVersionAtLeast" && guardPlatform == ApplePlatform.iOS && platform == ApplePlatform.MacCatalyst)
							guardPlatform = ApplePlatform.MacCatalyst;
						if (guardPlatform != platform)
							continue;
						version = GetVersion (arguments.Skip (argumentOffset).ToArray ());
					} else if (guards.TryGetValue (calledMethod.FullName, out var guard)) {
						foreach (var attribute in guard.Attributes) {
							if (!OSPlatformAttributeExtensions.TryParse ((string) attribute.ConstructorArguments [0].Value, out ApplePlatform? guardPlatform, out var guardVersion) || guardPlatform != platform)
								continue;
							if (guard.Method.Parameters.Count == 0) {
								version = NormalizeVersion (guardVersion ?? new Version (0, 0));
							} else if (guardVersion is null) {
								// Version arguments aren't part of the attribute contract. Only recognize the conventional signature.
								var parameterNames = new [] { "major", "minor", "build", "revision" };
								if (guard.Method.Parameters.Count > parameterNames.Length || guard.Method.Parameters.Where ((v, i) => v.Name != parameterNames [i] || v.ParameterType.MetadataType != MetadataType.Int32).Any ())
									continue;
								var arguments = GetConstantArguments (instruction, guard.Method.Parameters.Count, branchTargets);
								if (arguments is not null)
									version = GetVersion (arguments);
							}
						}
					}

					if (version is null || version > minimumVersion)
						continue;

					var key = $"{platform}: {method.FullName} -> {calledMethod.FullName} ({version})";
					failures [key] = $"{key}: the checked version is at or below the minimum supported version {minimumVersion}.";
				}
			}
			return failures;
		}

		static Version NormalizeVersion (Version version)
		{
			return new Version (version.Major, version.Minor, Math.Max (version.Build, 0), Math.Max (version.Revision, 0));
		}

		static Instruction []? GetConstantArguments (Instruction call, int count, HashSet<Instruction> branchTargets)
		{
			var arguments = new Instruction [count];
			var current = call.Previous;
			for (var i = count - 1; i >= 0; i--) {
				while (current?.OpCode == OpCodes.Nop)
					current = current.Previous;
				if (current is null)
					return null;
				arguments [i] = current;
				current = current.Previous;
			}

			if (count > 0) {
				// A jump into the argument sequence may supply different values on the evaluation stack.
				for (var instruction = arguments [0].Next; instruction is not null; instruction = instruction.Next) {
					if (branchTargets.Contains (instruction))
						return null;
					if (instruction == call)
						break;
				}
			}
			return arguments;
		}

		static Version? GetVersion (Instruction [] arguments)
		{
			if (arguments.Length == 0 || arguments.Length > 4)
				return null;
			var components = new int [4];
			for (var i = 0; i < arguments.Length; i++) {
				var instruction = arguments [i];
				switch (instruction.OpCode.Code) {
				case Code.Ldc_I4_0:
				case Code.Ldc_I4_1:
				case Code.Ldc_I4_2:
				case Code.Ldc_I4_3:
				case Code.Ldc_I4_4:
				case Code.Ldc_I4_5:
				case Code.Ldc_I4_6:
				case Code.Ldc_I4_7:
				case Code.Ldc_I4_8:
					components [i] = instruction.OpCode.Code - Code.Ldc_I4_0;
					break;
				case Code.Ldc_I4:
					components [i] = (int) instruction.Operand;
					break;
				case Code.Ldc_I4_S:
					components [i] = (sbyte) instruction.Operand;
					break;
				default:
					return null;
				}
				if (components [i] < 0)
					return null;
			}
			return new Version (components [0], components [1], components [2], components [3]);
		}
	}
}
