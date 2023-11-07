using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Linker.Steps;
using Xamarin.Tuner;

#nullable enable

namespace Xamarin.Linker {

	// Problems:
	// * `Dispose` set the generated backing fields to `null` which means
	//   the linker will mark every backing fields, even if not used 
	//   elsewhere (generally properties) inside the class
	// * Backing fields increase the memory footprint of the managed peer
	//   instance (for the type and all it's subclasses)
	// * Backing fields also increase the app size. Not a huge problem as
	//   they are all declared _weakly_ as `NSObject` but still...
	//
	// Solution:
	// * When the linker process a `Dispose` method of an `NSObject` 
	//   subclass with the _optimizable_ attribute then we remove the
	//   method body. This way the linker cannot mark the fields.
	// * Before saving back the assemblies we replace the cached method
	//   body and NOP every field that were not marked by something else
	//   than the `Dispose` method.
	public class BackingFieldDelayHandler : ConfigurationAwareMarkHandler {

		protected override string Name { get; } = "Backing Fields Optimizer";
		protected override int ErrorCode { get; } = 2400;

		public override void Initialize (LinkContext context, MarkContext markContext)
		{
			base.Initialize (context);
			markContext.RegisterMarkMethodAction (ProcessMethod);
		}

		// cache `Dispose` body of optimization NSObject subclasses
		static Dictionary<MethodDefinition, MethodBody> dispose = new ();

		protected override void Process (MethodDefinition method)
		{
			if (!method.HasParameters || !method.IsVirtual || !method.HasBody)
				return;
			if (method.Name != "Dispose")
				return;
			// only process methods that are marked as optimizable
			if (!method.IsBindingImplOptimizableCode (LinkContext))
				return;
			var t = method.DeclaringType;
			if (!t.IsNSObject (LinkContext))
				return;

			// keep original for later (if needed)
			dispose.Add (method, method.Body);

			// setting body to null will only cause it to be reloaded again
			// same if we don't get a new IL processor
			// and we do not want that (as it would mark the fields)
			var body = new MethodBody (method);
			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Ret);
			method.Body = body;
		}

		public static void ReapplyDisposedFields (DerivedLinkContext context, string operation)
		{
			// note: all methods in the dictionary are marked (since they were added from an IMarkHandler)
			foreach ((var method, var body) in dispose) {
				foreach (var ins in body.Instructions) {
					switch (ins.OpCode.OperandType) {
					case OperandType.InlineField:
						var fr = ins.Operand as FieldReference;
						var field = fr?.Resolve ();
						var isMarked = field is not null && context.Annotations.IsMarked (field);
						if (!isMarked) {
							var store_field = ins;
							var load_null = ins.Previous;
							var load_this = ins.Previous.Previous;
							if (OptimizeGeneratedCodeHandler.ValidateInstruction (method, store_field, operation, Code.Stfld) &&
								OptimizeGeneratedCodeHandler.ValidateInstruction (method, load_null, operation, Code.Ldnull) &&
								OptimizeGeneratedCodeHandler.ValidateInstruction (method, load_this, operation, Code.Ldarg_0)) {
								store_field.OpCode = OpCodes.Nop;
								load_null.OpCode = OpCodes.Nop;
								load_this.OpCode = OpCodes.Nop;
							}
						}
						break;
					}
				}
				method.Body = body;
			}
		}
	}

	public class BackingFieldReintroductionSubStep : ExceptionalSubStep {
		public override SubStepTargets Targets => SubStepTargets.Assembly;
		protected override string Name => "Backing Field Reintroduction";
		protected override int ErrorCode { get; } = 2410;

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);
			BackingFieldDelayHandler.ReapplyDisposedFields (LinkContext, Name);
		}
	}
}
