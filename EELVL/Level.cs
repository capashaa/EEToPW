using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;

namespace EELVL
{
	using static Blocks;

	public class Level
	{
		public string WorldName { get; set; }
		public string Description { get; set; }

		public int Width { get; }
		public int Height { get; }

		public string OwnerName { get; set; }
		public string OwnerID { get; set; }

		public string CrewName { get; set; }
		public string CrewID { get; set; }
		public int CrewStatus { get; set; }

		public uint BackgroundColor { get; set; }
		public float Gravity { get; set; }
		public bool Minimap { get; set; }
		public bool Campaign { get; set; }

		private readonly Block?[,,] blocks;
		public Block this[int l, int x, int y] {
			get => blocks[l, x, y] ?? Empty;
			set => blocks[l, x, y] = value.BlockID == 0 ? null : value;
		}

		public Level(
			string ownerName,
			string worldName,
			int width,
			int height,
			float gravity,
			uint backgroundColor,
			string description,
			bool campaign,
			string crewID,
			string crewName,
			int crewStatus,
			bool minimap,
			string ownerID
		)
		{
			OwnerName = ownerName;
			WorldName = worldName;
			Width = width;
			Height = height;
			Gravity = gravity;
			BackgroundColor = backgroundColor;
			Description = description;
			Campaign = campaign;
			CrewID = crewID;
			CrewName = crewName;
			CrewStatus = crewStatus;
			Minimap = minimap;
			OwnerID = ownerID;

			blocks = new Block[2, Width, Height];
		}

		public Level(int width, int height, int borderID = 9) : this("", "Untitled World", width, height, 1, 0, "", false, "", "", 1, true, "")
		{
			Block border = new(borderID);
			for (int x = 0; x < Width; x++)
			{
				blocks[0, x, 0] = border;
				blocks[0, x, Height - 1] = border;
			}
			for (int y = 1; y < Height - 1; y++)
			{
				blocks[0, 0, y] = border;
				blocks[0, Width - 1, y] = border;
			}
		}

		public static Level ReadMeta(Stream stream)
		{
			AS3BinaryReader reader = new(stream);

			return new(
				reader.ReadString(),
				reader.ReadString(),
				reader.ReadInt(),
				reader.ReadInt(),
				reader.ReadFloat(),
				reader.ReadUInt(),
				reader.ReadString(),
				reader.ReadBool(),
				reader.ReadString(),
				reader.ReadString(),
				reader.ReadInt(),
				reader.ReadBool(),
				reader.ReadString()
			);
		}

		public void ReadContent(Stream stream)
		{
			AS3BinaryReader reader = new(stream);

			try
			{
				while (true)
				{
					(Block b, int l, IEnumerable<(ushort, ushort)> locs) = Read(reader);

					foreach ((ushort x, ushort y) in locs)
						blocks[l, x, y] = b;
				}
			}
			catch (EndOfStreamException) { }
		}

		public static Level Open(Stream file)
		{
			using DeflateStream stream = new(file, CompressionMode.Decompress, true);

			Level level = ReadMeta(stream);
			level.ReadContent(stream);
			return level;
		}

		public static Level Open(string filename)
		{
			using FileStream file = File.OpenRead(filename);

			return Open(file);
		}

		public void WriteMeta(Stream stream)
		{
			AS3BinaryWriter writer = new(stream);

			writer.WriteString(OwnerName);
			writer.WriteString(WorldName);
			writer.WriteInt(Width);
			writer.WriteInt(Height);
			writer.WriteFloat(Gravity);
			writer.WriteUInt(BackgroundColor);
			writer.WriteString(Description);
			writer.WriteBool(Campaign);
			writer.WriteString(CrewID);
			writer.WriteString(CrewName);
			writer.WriteInt(CrewStatus);
			writer.WriteBool(Minimap);
			writer.WriteString(OwnerID);
		}

		public void WriteContent(Stream stream)
		{
			AS3BinaryWriter writer = new(stream);

			Dictionary<(int, Block), List<(ushort, ushort)>> byID = new();

			for (int l = 0; l < 2; l++)
			{
				for (ushort y = 0; y < Height; y++)
				{
					for (ushort x = 0; x < Width; x++)
					{
						if (blocks[l, x, y] is Block b)
						{
							if (!byID.ContainsKey((l, b))) byID[(l, b)] = new List<(ushort, ushort)>();

							byID[(l, b)].Add((x, y));
						}
					}
				}
			}

			foreach (((int l, Block b), List<(ushort, ushort)> locs) in byID)
				Write(writer, b, l, locs);
		}

		public void Save(Stream file)
		{
			using DeflateStream stream = new(file, CompressionMode.Compress, CompressionLevel.BestCompression, true);
			
			WriteMeta(stream);
			WriteContent(stream);
		}

		public void Save(string filename)
		{
			using FileStream file = File.OpenWrite(filename);
			
			Save(file);
		}
	}
}
