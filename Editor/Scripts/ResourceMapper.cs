using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CreateAssetMenu(fileName = "ResourceMapper", menuName = PackageDefines.packageName + "/" + nameof(ResourceMapper))]
	class ResourceMapper : CodeGenerator
	{
		[MenuItem("Tools/" + PackageDefines.packageName + "/ResourceMapper.GenerateAll")]
		static void GenerateAll()
		{
			var guids = AssetDatabase.FindAssets("t:ResourceMapper");
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var mapper = AssetDatabase.LoadAssetAtPath<ResourceMapper>(path);
				mapper.Replace();
			}
		}
		[SerializeField] DefaultAsset resourceFolder;
		protected override string Generate()
		{
			var dir = AssetDatabase.GetAssetPath(resourceFolder);
			return GetCodeBlock(dir, 0);
		}
		string GetCodeBlock(string dir, int indent)
		{
			if (dir.IsNullOrEmpty()) throw new("dir is null or empty");
			var strIdent = new string('\t', indent);
			var strIdent2 = new string('\t', indent + 1);
			var strIdent3 = new string('\t', indent + 2);
			var builder = new StringBuilder();
			builder.AppendLine($"{strIdent}public static class {Path.GetFileName(dir)}");
			builder.AppendLine($"{strIdent}{{");
			List<string> allAssets = new();
			foreach (var subDir in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
			{
				builder.AppendLine(GetCodeBlock(subDir.Replace("\\", "/"), indent + 1));
			}
			foreach (var filePath in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
			{
				var file = filePath.Replace("\\", "/");
				var extension = Path.GetExtension(file);
				if (extension.Trim().ToLower() == ".meta") continue;
				var fileUnderResource = file[..^extension.Length].Replace(AssetDatabase.GetAssetPath(resourceFolder), "")[1..];
				var asset = Resources.Load(fileUnderResource);
				if (!asset) continue;
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var type = asset.GetType();
				var propertyName = $"{fileNameWithoutExtension}_{type.Name}";
				builder.AppendLine($"{strIdent2}public static ITimedCache<{type.FullName}> {propertyName} {{ get; }} = new ResourceCache<{type.FullName}>(\"{fileUnderResource}\");");
				allAssets.Add(propertyName);
				var assets = Resources.LoadAll(fileUnderResource);
				if (assets.Length > 1)
				{
					var subPropertyName = $"{fileNameWithoutExtension}_AllAssets";
					builder.AppendLine($"{strIdent2}public static ITimedCache<UnityEngine.Object[]> {subPropertyName} {{ get; }} = new ResourceGroupCache<UnityEngine.Object>(\"{fileUnderResource}\");");
					allAssets.Add(subPropertyName);
				}
			}
			if (allAssets.Count > 0)
			{
				builder.AppendLine($"{strIdent2}public static IReadOnlyDictionary<string, ITimedCache> AllAssets {{ get; }} = new Dictionary<string, ITimedCache>");
				builder.AppendLine($"{strIdent2}{{");
				foreach (var asset in allAssets)
				{
					builder.AppendLine($"{strIdent3}[\"{asset}\"] = {asset},");
				}
				builder.AppendLine($"{strIdent2}}};");
			}
			builder.AppendLine($"{strIdent}}}");
			return builder.ToString();
		}
	}
}
