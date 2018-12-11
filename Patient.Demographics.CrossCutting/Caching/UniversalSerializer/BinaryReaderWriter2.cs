
// Copyright Christophe Bertrand.

using System;
using System.Text;
using System.IO;
using UniversalSerializerLib3.NumberTools;

// ###############################################################
// ###############################################################
// ###############################################################
// ###############################################################

namespace UniversalSerializerLib3.FileTools
{

	// ###############################################################
	// ###############################################################

	/// <summary>
	/// Allows use of protected Read7BitEncodedInt().
	/// </summary>
	internal class BinaryReader2 : BinaryReader
	{
		public BinaryReader2(Stream input)
			: base(input)
		{ }

		public BinaryReader2(Stream input, Encoding encoding)
			: base(input,encoding)
		{	}
        
		internal void SetPosition(long NewPosition)
		{
			if (base.BaseStream.Position != NewPosition)
				base.BaseStream.Position = NewPosition;
		}


		/// <summary>
		/// Renvoie -1 si on est à la fin du flux.
		/// Pour binaryformatter, qui nécessite un accès sans exception.
		/// </summary>
		/// <returns></returns>
		internal int ReadByteNoException()
		{
			return base.BaseStream.ReadByte();
		} 

		/// <summary>
		/// Reads compressed short from stream.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal short Read7BitEncodedShort()
		{
			return unchecked((short)this.Read7BitEncodedUShort());
		}

		/// <summary>
		/// Reads compressed short from stream using special encoding.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal short ReadSpecial7BitEncodedShort()
		{
			return unchecked(Numbers.Special16ToInt16((short)this.Read7BitEncodedUShort()));
		}

		/// <summary>
		/// Reads compressed ushort from stream.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal ushort Read7BitEncodedUShort()
		{
			ushort retValue = 0;
			int shifter = 0;

			while (shifter != 21) // 21 because 7*3 > 16 (bits).
			{
				byte b = this.ReadByte();
				retValue |= (ushort)((ushort)(b & 0x7f) << shifter);
				shifter += 7;
				if ((b & 0x80) == 0)
				{
					return retValue;
				}
			}
#if DEBUG
			throw new FormatException();
#else
			return 0;
#endif
		}

		/// <summary>
		/// Reads a compressed integer from the stream.
		/// </summary>
		/// <returns></returns>
		public new int Read7BitEncodedInt()
		{
			return base.Read7BitEncodedInt(); // go to the protected method.
		}

		/// <summary>
		/// Reads a compressed integer from the stream using special encoding.
		/// </summary>
		/// <returns></returns>
		public int ReadSpecial7BitEncodedInt()
		{
			return Numbers.Special32ToInt32(base.Read7BitEncodedInt()); // go to the protected method.
		}

		/// <summary>
		/// Reads a compressed unsigned integer from the stream.
		/// </summary>
		/// <returns></returns>
		public uint Read7BitEncodedUInt()
		{
			return unchecked((uint)base.Read7BitEncodedInt()); // go to the protected method.
		}

		/// <summary>
		/// Reads compressed long from stream.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal long Read7BitEncodedLong()
		{
			return unchecked((long)this.Read7BitEncodedULong());
		}

		/// <summary>
		/// Reads compressed long from stream special encoding.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal long ReadSpecial7BitEncodedLong()
		{
			return Numbers.Special64ToInt64( unchecked((long)this.Read7BitEncodedULong()));
		}

		/// <summary>
		/// Reads compressed long from stream.
		/// </summary>
		/// <returns></returns> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal ulong Read7BitEncodedULong()
		{
			ulong retValue = 0;
			int shifter = 0;

			while (shifter != 70) // 70 because 7*10 > 64 (bits).
			{
				byte b = this.ReadByte();
				retValue |= (ulong)(b & 0x7f) << shifter;
				shifter += 7;
				if ((b & 0x80) == 0)
				{
					return retValue;
				}
			}
#if DEBUG
			throw new FormatException();
#else
			return 0;
#endif
		}

	}

	/// <summary>
	/// Allows use of protected Write7BitEncodedInt().
	/// </summary>
	internal class BinaryWriter2 : BinaryWriter
	{
		public BinaryWriter2(Stream output)
			: base(output)
		{ }
		public BinaryWriter2(Stream output, Encoding encoding)
			: base(output, encoding)
		{ }
        
		/// <summary>
		/// Writes compressed short to stream.
		/// </summary>
		/// <param name="value"></param>
 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
 
		internal void Write7BitEncodedShort(short value)
		{
			this.Write7BitEncodedUShort(unchecked((ushort)value));
		}

		/// <summary>
		/// Writes compressed short to stream using special encoding.
		/// </summary>
		/// <param name="value"></param>
 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
 
		internal void WriteSpecial7BitEncodedShort(short value)
		{
			this.Write7BitEncodedUShort(unchecked((ushort) Numbers.Int16ToSpecial16(value)));
		}

		/// <summary>
		/// Writes compressed ushort to stream.
		/// </summary>
		/// <param name="value"></param> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal void Write7BitEncodedUShort(ushort value)
		{
			ushort number;

			for (number = value; number >= (ushort)128u; number >>= 7)
				this.Write(unchecked((byte)(number | (ushort)128u)));

			this.Write((byte)number); // remaining bits.
		}

		/// <summary>
		/// Writes a compressed integer to the stream.
		/// </summary>
		/// <param name="value"></param>
		public new void Write7BitEncodedInt(int value)
		{
			base.Write7BitEncodedInt(value); // go to the protected method.
		}

		/// <summary>
		/// Writes a compressed integer to the stream using special encoding.
		/// </summary>
		/// <param name="value"></param>
		public void WriteSpecial7BitEncodedInt(int value)
		{
			base.Write7BitEncodedInt(Numbers.Int32ToSpecial32(value)); // go to the protected method.
		}

		/// <summary>
		/// Writes a compressed unsigned integer to the stream.
		/// </summary>
		/// <param name="value"></param>
		public void Write7BitEncodedUInt(uint value)
		{
			base.Write7BitEncodedInt(unchecked((int)value)); // go to the protected method.
		}

		/// <summary>
		/// Writes compressed long to stream.
		/// </summary>
		/// <param name="value"></param> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal void Write7BitEncodedLong(long value)
		{
			this.Write7BitEncodedULong(unchecked((ulong)value));
		}

		/// <summary>
		/// Writes compressed long to stream using special encoding.
		/// </summary>
		/// <param name="value"></param> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal void WriteSpecial7BitEncodedLong(long value)
		{
			this.Write7BitEncodedULong(unchecked((ulong)Numbers.Int64ToSpecial64(value)));
		}

		/// <summary>
		/// Writes compressed ulong to stream.
		/// </summary>
		/// <param name="value"></param> 
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)] 
		internal void Write7BitEncodedULong(ulong value)
		{
			ulong number;

			for (number = value; number >= 128ul; number >>= 7)
				this.Write(unchecked((byte)(number | 128ul)));

			this.Write((byte)number); // remaining bits.
		}
	}

	// ###############################################################
	// ###############################################################

}

	// ###############################################################
	// ###############################################################

// ###############################################################
// ###############################################################
#if false // Kept for possible future use.
namespace UniversalSerializerLib3.NumberTools
{

	internal static class Numbers
	{

		#region Special integers

		/* 
		 * 'Special' integers are signed integers where negative values have their bits reversed (except the higher bit),
		 * then value is rotated one bit left, the higher bit goes to the lower site.
		 * The objective is to reduce the number of set (1) bits for small negative numbers.
		 * The compresser considers small numbers (positive or negative) as more frequent and optimizes their storage size.
		 * 
		 * The transcoding method is the same in the two directions, but I wrote one function for each direction for clarity reasons.
		 */

		internal static short Int16ToSpecial16(short i)
		{
			if (i >= 0)
				return (short)(i << 1);
			return unchecked((short)((i << 1) ^ -1));
		}

		internal static short Special16ToInt16(short i)
		{
			if ((i & 1) == 0)
				return (short)((uint)(ushort)i >> 1);
			return unchecked((short)((((uint)(ushort)i >> 1)) ^ -1));
		}

		internal static int Int32ToSpecial32(int i)
		{
			if (i >= 0)
				return i << 1;
			return (i << 1) ^ -1;
		}

		internal static int Special32ToInt32(int i)
		{
			if ((i & 1) == 0)
				return (int)((uint)i >> 1);
			return ((int)((uint)i >> 1)) ^ -1;
		}

		internal static long Int64ToSpecial64(long i)
		{
			if (i >= 0)
				return i << 1;
			return (i << 1) ^ -1L;
		}

		internal static long Special64ToInt64(long i)
		{
			if ((i & 1) == 0)
				return (long)((ulong)i >> 1);
			return ((long)((ulong)i >> 1)) ^ -1L;
		}

		#endregion Special integers

	}
}

#endif
	// ###############################################################
	// ###############################################################
	// ###############################################################




