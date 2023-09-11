using System;
using System.Collections.Generic;
using EthansGameKit.Collections;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	class FlagIt : ScriptableSingleton<FlagIt>
	{
		class PostAssetImport : AssetPostprocessor
		{
			static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
			}
		}

		[Serializable]
		class Guid2Flags : SerializableDictionary<string, BitCollection>
		{
		}

		[SerializeField] Guid2Flags guid2Flags = new();
		void Reset()
		{
			guid2Flags.Clear();
		}
		void AddFlag(string guid, int flag)
		{
			if (!guid2Flags.TryGetValue(guid, out var flags))
				guid2Flags[guid] = flags = BitCollection.Generate();
			flags.Set(flag, true);
			flags.Trim();
		}
		void RemoveFlag(string guid, int flag)
		{
			if (guid2Flags.TryGetValue(guid, out var flags))
			{
				flags.Set(flag, false);
				flags.Trim();
				if (flags.IsEmpty())
					guid2Flags.Remove(guid);
			}
		}
		IEnumerable<string> Search(BitCollection flags)
		{
			foreach (var (guid, f) in guid2Flags)
				if ((f & flags) == flags)
					yield return guid;
		}
	}
}
