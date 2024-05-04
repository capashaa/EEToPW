using System;
using System.IO;
using System.Text;

namespace EELVL
{
	using static Helpers;

	internal class AS3BinaryReader
	{
		private readonly Stream inner;
		private byte[] buffer;

		public AS3BinaryReader(Stream inner)
		{
			this.inner = inner;
			this.buffer = new byte[256];
		}

		private Span<byte> Bytes(int count)
		{
			if (buffer.Length < count)
			{
				int length = buffer.Length;
				while (length < count) length *= 2;
				buffer = new byte[length];
			}

			int pos = 0;
			while (pos < count)
			{
				int read = inner.Read(buffer, pos, count);
				if (read == 0) throw new EndOfStreamException();
				pos += read;
			}

			return buffer[0..count];
		}

		private Span<byte> FromBigEndian(int count)
		{
			Span<byte> data = Bytes(count);
			if (BitConverter.IsLittleEndian) data.Reverse();
			return data;
		}

		public int ReadInt() => BitConverter.ToInt32(FromBigEndian(4));
		public uint ReadUInt() => BitConverter.ToUInt32(FromBigEndian(4));
		public short ReadShort() => BitConverter.ToInt16(FromBigEndian(2));
		public ushort ReadUShort() => BitConverter.ToUInt16(FromBigEndian(2));
		public float ReadFloat() => BitConverter.ToSingle(FromBigEndian(4));
		public bool ReadBool() => BitConverter.ToBoolean(Bytes(1));
		public string ReadString()
		{
			ushort length = ReadUShort();
			return Encoding.UTF8.GetString(Bytes(length));
		}
		public ushort[] ReadUShortArray()
		{
			uint length = ReadUInt();
			return ToUShortArray(Bytes((int)length));
		}
	}
}
