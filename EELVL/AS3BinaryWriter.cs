﻿using System;
using System.IO;
using System.Text;

namespace EELVL
{
	using static Helpers;

	internal class AS3BinaryWriter
	{
		private readonly Stream inner;

		public AS3BinaryWriter(Stream inner)
		{
			this.inner = inner;
		}

		private void Write(ReadOnlySpan<byte> data)
		{
			inner.Write(data);
		}

		private void WriteBigEndian(Span<byte> data)
		{
			if (BitConverter.IsLittleEndian) data.Reverse();
			Write(data);
		}

		public void WriteInt(int val) => WriteBigEndian(BitConverter.GetBytes(val));
		public void WriteUInt(uint val) => WriteBigEndian(BitConverter.GetBytes(val));
		public void WriteShort(short val) => WriteBigEndian(BitConverter.GetBytes(val));
		public void WriteUShort(ushort val) => WriteBigEndian(BitConverter.GetBytes(val));
		public void WriteFloat(float val) => WriteBigEndian(BitConverter.GetBytes(val));
		public void WriteBool(bool val) => Write(BitConverter.GetBytes(val));
		public void WriteString(string val)
		{
			byte[] data = Encoding.UTF8.GetBytes(val);
			WriteUShort((ushort)data.Length);
			Write(data);
		}
		public void WriteUShortArray(ushort[] val)
		{
			Span<byte> data = FromUShortArray(val);
			WriteUInt((uint)data.Length);
			Write(data);
		}
	}
}
