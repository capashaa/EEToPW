using System;
using System.Collections;
using System.Collections.Generic;

namespace EELVL
{
	internal static class Helpers
	{
		public class LookupTable<TFrom, TTo> : IEnumerable<(TFrom, TTo)>
		{
			private readonly TTo defaultValue;
			private readonly Dictionary<TFrom, TTo> internalDict;

			public LookupTable(TTo defaultValue)
			{
				this.defaultValue = defaultValue;
				this.internalDict = new Dictionary<TFrom, TTo>();
			}

			public void Add(TTo to, params TFrom[] froms)
			{
				foreach (TFrom from in froms)
					internalDict[from] = to;
			}

			public TTo this[TFrom from] {
				get => internalDict.GetValueOrDefault(from, defaultValue);
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			public IEnumerator<(TFrom, TTo)> GetEnumerator()
			{
				foreach (KeyValuePair<TFrom, TTo> kvp in internalDict)
					yield return (kvp.Key, kvp.Value);
			}
		}

		public static ushort[] ToUShortArray(ReadOnlySpan<byte> data)
		{
			int count = data.Length / 2;

			ushort[] arr = new ushort[count];

			for (int i = 0; i < count; i++)
				arr[i] = (ushort)(data[2*i] << 8 | data[2*i + 1]);

			return arr;
		}

		public static Span<byte> FromUShortArray(ushort[] data)
		{
			int count = data.Length;

			byte[] arr = new byte[count * 2];

			for (int i = 0; i < count; i++)
			{
				arr[2 * i] = (byte)(data[i] >> 8);
				arr[2 * i + 1] = (byte)(data[i] & 255);
			}

			return arr;
		}
	}
}
