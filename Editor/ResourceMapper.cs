using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CreateAssetMenu(fileName = "ResourceMapper", menuName = "EthansGameKit/ResourceMapper")]
	class ResourceMapper : CodeGenerator
	{
		class ResourceInfo
		{
			public string guid;
			public string path;
			public string alias;
		}

		class ResourceIndexer
		{
			const string filePath = "ResourceMap.txt";
			readonly Dictionary<string, ResourceInfo> guid2Info = new();
			readonly SortedDictionary<string, ResourceInfo> alias2Info = new();
			public ResourceIndexer()
			{
				if (System.IO.File.Exists(filePath))
				{
					var lines = System.IO.File.ReadAllLines(filePath);
					foreach (var line in lines)
					{
						var parts = line.Split('\t');
						var info = new ResourceInfo { alias = parts[0], guid = parts[1], path = parts[2] };
						guid2Info.Add(info.guid, info);
						alias2Info.Add(info.alias, info);
					}
				}
			}
			public bool ContainsAlias(string alias)
			{
				return alias2Info.ContainsKey(alias);
			}
			public void Add(string guid, string path, string alias)
			{
				Assert.IsFalse(guid2Info.ContainsKey(guid));
				Assert.IsFalse(alias2Info.ContainsKey(alias));
				var info = new ResourceInfo { guid = guid, path = path, alias = alias };
				guid2Info.Add(guid, info);
				alias2Info.Add(alias, info);
			}
			public bool TryGetViaGuid(string guid, out ResourceInfo info)
			{
				return guid2Info.TryGetValue(guid, out info);
			}
			public void Save()
			{
				var builder = new StringBuilder();
				foreach (var (alias, info) in alias2Info)
					builder.AppendLine($"{alias}\t{info.guid}\t{info.path}");
				System.IO.File.WriteAllText("ResourceMap.txt", builder.ToString());
			}
		}

		static ResourceIndexer indexer;
		static ResourceIndexer Indexer => indexer ??= new();
		static void UpdateGuidMap()
		{
			var guids = AssetDatabase.FindAssets("", new[] { "Assets" });
			var dirty = false;
			foreach (var guid in guids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				if (!Indexer.TryGetViaGuid(guid, out _))
				{
					var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
					var alias = $"{asset.GetType().Name}_{asset.name}";
					var i = 0;
					while (Indexer.ContainsAlias(alias))
					{
						alias = $"{asset.GetType().Name}_{asset.name}_{++i}";
					}
					Indexer.Add(guid, assetPath, alias);
					dirty = true;
				}
			}
			if (dirty) Indexer.Save();
		}
		[SerializeField] DefaultAsset resourceFolder;
		[SerializeField] bool @public;
		protected override string Generate()
		{
			UpdateGuidMap();
			var builder = new StringBuilder();
			var folderPath = AssetDatabase.GetAssetPath(resourceFolder);
			var dir2Caches = new Dictionary<string, List<string>>();
			var @public = this.@public ? "public " : "";
			foreach (var guid in AssetDatabase.FindAssets("", new[] { folderPath }))
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
				var type = obj.GetType();
				if (type.Namespace != null && type.Namespace.StartsWith("UnityEditor")) continue;
				var suffix = assetPath[(folderPath.Length + 1)..];
				var extension = System.IO.Path.GetExtension(assetPath);
				var suffixWithoutExtension = suffix[..^extension.Length];
				Indexer.TryGetViaGuid(guid, out var info);
				var propertyName = info.alias;
				builder.AppendLine($"{@public}static ResourceCache<{type.FullName}> {propertyName} {{ get; }} = new(\"{suffixWithoutExtension}\");");
				var lastSlash = suffixWithoutExtension.LastIndexOf('/');
				if (lastSlash >= 0)
				{
					var dir = suffixWithoutExtension[..lastSlash];
					if (!dir2Caches.TryGetValue(dir, out var list))
						dir2Caches[dir] = list = new();
					list.Add(propertyName + ",");
				}
			}
			foreach (var (dir, caches) in dir2Caches)
			{
				builder.AppendLine($"{@public}static IReadOnlyList<ITimedCache> ResourceGroup_{dir.Replace('/', '_')} {{ get; }} = new ITimedCache[]");
				builder.AppendLine("{");
				foreach (var cache in caches)
					builder.AppendLine($"\t{cache}");
				builder.AppendLine("};");
			}
			return builder.ToString();
		}
	}
}
