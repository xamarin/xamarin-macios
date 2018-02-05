//
// ILReader to parse the byte array provided by MethodBase.GetMethodBody ().GetILAsByteArray () into better-looking IL instructions.
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2018 Microsoft Inc. All rights reserved.
//

// Adapted code from here: https://blogs.msdn.microsoft.com/haibo_luo/2005/10/04/read-il-from-methodbody/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Linker.Shared
{
	public class ILInstruction
	{
		public MethodBase Method;
		public OpCode OpCode;
		public int Offset;
		public object Operand;

		public ILInstruction (MethodBase method, int offset, OpCode opcode, object operand = null)
		{
			this.Method = method;
			this.OpCode = opcode;
			this.Offset = offset;
			this.Operand = operand;
		}

		public override string ToString ()
		{
			var methodOperand = Operand as MethodBase;
			if (methodOperand != null)
				return $"IL_{Offset:0000} {OpCode} {methodOperand.DeclaringType.FullName}.{methodOperand.Name}";
			return $"IL_{Offset:0000} {OpCode} {(Operand is MethodBase ? ((MethodBase) Operand).Name : Operand?.ToString ())}";
		}
	}

	public class ILReader : IEnumerable<ILInstruction>
	{
		List<ILInstruction> instructions;

		static OpCode [] oneByteOpcodes = new OpCode [0x100];
		static OpCode [] twoByteOpcodes = new OpCode [0x100];
		static ILReader ()
		{
			foreach (var fi in typeof (OpCodes).GetFields (BindingFlags.Public | BindingFlags.Static)) {
				var opCode = (OpCode) fi.GetValue (null);
				var value = (ushort) opCode.Value;
				if (value < 0x100)
					oneByteOpcodes [value] = opCode;
				else if ((value & 0xff00) == 0xfe00)
					twoByteOpcodes [value & 0xff] = opCode;
			}
		}

		public ILReader (MethodBase method)
		{
			instructions = Parse (method);
		}

		IEnumerator<ILInstruction> IEnumerable<ILInstruction>.GetEnumerator ()
		{
			return instructions.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return instructions.GetEnumerator ();
		}

		List<ILInstruction> Parse (MethodBase method)
		{
			var rv = new List<ILInstruction> ();
			var position = 0;

			var body = method.GetMethodBody ();
			if (body == null)
				return rv;

			var bytes = body.GetILAsByteArray ();
			while (position < bytes.Length)
				rv.Add (ReadInstruction (method, bytes, ref position));

			return rv;
		}

		static ILInstruction ReadInstruction (MethodBase method, byte [] bytes, ref int position)
		{
			var offset = position;
			var opCode = OpCodes.Nop;

			// read first 1 or 2 bytes as opCode
			Byte code = ReadByte (bytes, ref position);
			if (code != 0xFE)
				opCode = oneByteOpcodes [code];
			else {
				code = ReadByte (bytes, ref position);
				opCode = twoByteOpcodes [code];
			}

			switch (opCode.OperandType) {
			case OperandType.InlineNone:
				return new ILInstruction (method, offset, opCode);

			case OperandType.ShortInlineBrTarget:
				return new ILInstruction (method, offset, opCode, ReadSByte (bytes, ref position));

			case OperandType.InlineBrTarget:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.ShortInlineI:
				return new ILInstruction (method, offset, opCode, ReadByte (bytes, ref position));

			case OperandType.InlineI:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineI8:
				return new ILInstruction (method, offset, opCode, ReadInt64 (bytes, ref position));

			case OperandType.ShortInlineR:
				return new ILInstruction (method, offset, opCode, ReadSingle (bytes, ref position));

			case OperandType.InlineR:
				return new ILInstruction (method, offset, opCode, ReadDouble (bytes, ref position));

			case OperandType.ShortInlineVar:
				return new ILInstruction (method, offset, opCode, ReadByte (bytes, ref position));

			case OperandType.InlineVar:
				return new ILInstruction (method, offset, opCode, ReadUInt16 (bytes, ref position));

			case OperandType.InlineString:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineSig:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineField:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineType:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineTok:
				return new ILInstruction (method, offset, opCode, ReadInt32 (bytes, ref position));

			case OperandType.InlineMethod:
				return new ILInstruction (method, offset, opCode, method.Module.ResolveMethod (ReadInt32 (bytes, ref position)));

			case OperandType.InlineSwitch:
				var cases = ReadInt32 (bytes, ref position);
				var deltas = new int [cases];
				for (var i = 0; i < cases; i++)
					deltas [i] = ReadInt32 (bytes, ref position);
				return new ILInstruction (method, offset, opCode, deltas);

			default:
				throw new BadImageFormatException ("unexpected OperandType " + opCode.OperandType);
			}
		}

		static byte ReadByte (byte[] bytes, ref int position)
		{
			return bytes [position++];
		}

		static sbyte ReadSByte (byte [] bytes, ref int position)
		{
			return (sbyte) ReadByte (bytes, ref position);
		}

		static ushort ReadUInt16 (byte [] bytes, ref int position)
		{
			position += 2;
			return BitConverter.ToUInt16 (bytes, position - 2);
		}

		static uint ReadUInt32 (byte [] bytes, ref int position)
		{
			position += 4;
			return BitConverter.ToUInt32 (bytes, position - 4);
		}

		static ulong ReadUInt64 (byte [] bytes, ref int position)
		{
			position += 8;
			return BitConverter.ToUInt64 (bytes, position - 8);
		}

		static int ReadInt32 (byte [] bytes, ref int position)
		{
			position += 4;
			return BitConverter.ToInt32 (bytes, position - 4);
		}

		static long ReadInt64 (byte [] bytes, ref int position)
		{
			position += 8;
			return BitConverter.ToInt64 (bytes, position - 8);
		}

		static float ReadSingle (byte [] bytes, ref int position)
		{
			position += 4;
			return BitConverter.ToSingle (bytes, position - 4);
		}

		static double ReadDouble (byte [] bytes, ref int position)
		{
			position += 8;
			return BitConverter.ToDouble (bytes, position - 8);
		}
	}
}
