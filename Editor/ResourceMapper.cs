using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CreateAssetMenu(fileName = "ResourceMapper", menuName = "EthansGameKit/ResourceMapper")]
	class ResourceMapper : CodeGenerator
	{
		[SerializeField] DefaultAsset resourceFolder;
		[SerializeField] bool @public;
		protected override string Generate()
		{
			var builder = new StringBuilder();
			var folderPath = AssetDatabase.GetAssetPath(resourceFolder);
			var dir2caches = new Dictionary<string, List<string>>();
			var @public = this.@public ? "public " : "";
			foreach (var guid in AssetDatabase.FindAssets("t:GameObject", new[] { folderPath }))
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var suffix = assetPath[(folderPath.Length + 1)..];
				var suffixWithoutExtension = suffix[..^7];
				var propertyName = $"GameObject_{suffixWithoutExtension.Replace('/', '_')}";
				builder.AppendLine($"{@public}static ResourceCache<GameObject> {propertyName} {{ get; }} = new(\"{suffixWithoutExtension}\");");
				var lastSlash = suffixWithoutExtension.LastIndexOf('/');
				if (lastSlash >= 0)
				{
					var dir = suffixWithoutExtension[..lastSlash];
					if (!dir2caches.TryGetValue(dir, out var list))
						dir2caches[dir] = list = new();
					list.Add(propertyName + ",");
				}
			}
			foreach (var (dir, caches) in dir2caches)
			{
				builder.AppendLine($"{@public}static IReadOnlyList<ResourceCache<GameObject>> GameObjectGroup_{dir.Replace('/', '_')} {{ get; }} = new ResourceCache<GameObject>[]");
				builder.AppendLine("{");
				foreach (var cache in caches)
					builder.AppendLine($"\t{cache}");
				builder.AppendLine("};");
			}
			return builder.ToString();
		}
	}
}
