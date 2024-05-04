using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace EELVL
{
	using static Blocks;

	public class LevelBundle : IList<LevelBundleEntry>
	{
		private List<LevelBundleEntry> levels;

		public LevelBundle()
		{
			this.levels = new();
		}

		public LevelBundle(IEnumerable<LevelBundleEntry> levels)
		{
			this.levels = levels.ToList();
		}

		public void Add(LevelBundleEntry item) => levels.Add(item);
		public void Add(Level level, string? WorldID = null, string? Filename = null) =>
			Add(new LevelBundleEntry(level, WorldID, Filename));

		public static LevelBundle Open(ZipArchive archive)
		{
			LevelBundle bundle = new();
			int i = 0;
			foreach (ZipArchiveEntry entry in archive.Entries)
			{
				if (entry.FullName.ToLower().EndsWith(".eelvl"))
				{
					using Stream stream = entry.Open();
					bundle.Add(Level.Open(stream), i.ToString(), entry.FullName);
					i += 1;
				}
			}
			return bundle;
		}

		public static LevelBundle Open(string filename)
		{
			using ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Read);

			return Open(archive);
		}

		public void Save(ZipArchive archive)
		{
			Dictionary<string, int> ids = new();
			for (int i = 0; i < levels.Count; i++)
				if (levels[i].WorldID is string worldPortalID)
					ids.Add(worldPortalID, i);
			int filenameDigits = levels.Count.ToString().Length;
			for (int i = 0; i < levels.Count; i++)
			{
				Level level = levels[i].Level;
				if (ids.Count > 0)
				{
					Level newLevel = new(
						level.OwnerName,
						level.WorldName,
						level.Width,
						level.Height,
						level.Gravity,
						level.BackgroundColor,
						level.Description,
						level.Campaign,
						level.CrewID,
						level.CrewName,
						level.CrewStatus,
						level.Minimap,
						level.OwnerID
					);
					for (int l = 0; l < 2; l++)
						for (int x = 0; x < level.Width; x++)
							for (int y = 0; y < level.Height; y++)
								newLevel[l, x, y] =
									level[l, x, y] is WorldPortalBlock wp && ids.TryGetValue(wp.Target, out int newId)
									? new WorldPortalBlock(wp.BlockID, newId.ToString(), wp.Spawn)
									: level[l, x, y];
					level = newLevel;
				}
				string filename = levels[i].Filename ?? i.ToString().PadLeft(filenameDigits, '0');
				using Stream stream = archive.CreateEntry(filename, CompressionLevel.Optimal).Open();
				level.Save(stream);
			}
		}

		public void Save(string filename)
		{
			using ZipArchive archive = ZipFile.Open(filename, ZipArchiveMode.Create);

			Save(archive);
		}

		public LevelBundleEntry this[int index] { get => levels[index]; set => levels[index] = value; }
		public int Count => levels.Count;
		public bool IsReadOnly => false;
		public void Clear() => levels.Clear();
		public bool Contains(LevelBundleEntry item) => levels.Contains(item);
		public void CopyTo(LevelBundleEntry[] array, int arrayIndex) => levels.CopyTo(array, arrayIndex);
		public IEnumerator<LevelBundleEntry> GetEnumerator() => levels.GetEnumerator();
		public int IndexOf(LevelBundleEntry item) => levels.IndexOf(item);
		public void Insert(int index, LevelBundleEntry item) => levels.Insert(index, item);
		public bool Remove(LevelBundleEntry item) => levels.Remove(item);
		public void RemoveAt(int index) => levels.RemoveAt(index);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class LevelBundleEntry
	{
		public Level Level { get; set; }
		public string? WorldID { get; set; }
		public string? Filename { get; set; }

		public LevelBundleEntry(Level level, string? worldPortalID = null, string? filename = null)
		{
			Level = level;
			WorldID = worldPortalID;
			Filename = filename;
		}
	}
}
