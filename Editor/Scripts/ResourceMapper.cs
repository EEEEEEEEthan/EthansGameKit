using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor.Scripts
{
	[CreateAssetMenu(fileName = "ResourceMapper", menuName = "EthansGameKit/ResourceMapper")]
	internal class ResourceMapper : ScriptableObject
	{
		static string GetShorttenName(Type type, List<string> usings)
		{
			var fullName = type.FullName;
			Assert.IsNotNull(fullName);
			foreach (var u in usings)
				if (fullName.StartsWith(u))
				{
					fullName = fullName[(u.Length + 1)..];
					return fullName;
				}
			return fullName;
		}

		[CustomEditor(typeof(ResourceMapper))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				var target = (ResourceMapper)this.target;
				base.OnInspectorGUI();
				if (!target.rootFolder)
				{
					EditorGUILayout.HelpBox("Root Folder is required", MessageType.Error);
					return;
				}
				if (!target.script)
				{
					EditorGUILayout.HelpBox("Script is required", MessageType.Error);
					return;
				}
				var regionStart = $"#region {target.region}";
				var regionEnd = $"#endregion {target.region}";
				if (!target.script.text.Contains(regionStart) || !target.script.text.Contains(regionEnd))
				{
					EditorGUILayout.HelpBox("Region not found", MessageType.Error);
					return;
				}
				if (GUILayout.Button("Generate Code")) target.GenerateCode();
			}
		}

		[SerializeField] DefaultAsset rootFolder;
		[SerializeField] MonoScript script;
		[SerializeField] string region;
		[SerializeField, UnityEngine.Range(0, 10)] int indent;

		void GenerateCode()
		{
			var originalText = script.text;
			var patten = new Regex(@"using\s+([\w\.]+);");
			var matches = patten.Matches(originalText);
			var usings = new List<string>();
			for (var i = 0; i < matches.Count; i++)
			{
				var match = matches[i];
				usings.Add(match.Groups[1].Value);
			}
			var startMark = $"#region {region}";
			var endMark = $"#endregion {region}";
			var startIndex = originalText.IndexOf(startMark, StringComparison.Ordinal);
			var endIndex = originalText.IndexOf(endMark, StringComparison.Ordinal);
			var prefix = originalText[..(startIndex + startMark.Length)];
			var suffix = originalText[endIndex..];
			var builder = new StringBuilder();
			builder.AppendLine(prefix);
			builder.Append(GetBody(usings));
			builder.Append(suffix);
			var path = AssetDatabase.GetAssetPath(script);
			File.WriteAllText(path, builder.ToString());
			AssetDatabase.Refresh();
		}

		string GetBody(List<string> usings)
		{
			var builder = new StringBuilder();
			var indent = new string('\t', this.indent);
			var guids = AssetDatabase.FindAssets("", new[] { AssetDatabase.GetAssetPath(rootFolder) });
			const string resourceFolder = "Resources/";
			usings.Sort();
			usings.Reverse();
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
				if (asset is DefaultAsset) continue;
				path = path.Replace("\\", "/");
				var index = path.IndexOf(resourceFolder, StringComparison.Ordinal);
				if (index < 0) continue;
				var resourcePath = path[(index + resourceFolder.Length)..];
				var dir = Path.GetDirectoryName(resourcePath).Replace("\\", "/");
				var resourceName = Path.GetFileNameWithoutExtension(resourcePath);
				var extensionName = Path.GetExtension(resourcePath);
				extensionName = char.ToUpper(extensionName[1]) + extensionName[2..];
				var memberName = resourceName.Replace(" ", "").Replace("-", "") + extensionName;
				if (string.IsNullOrEmpty(dir))
					builder.AppendLine($"{indent}public const string {memberName} = \"{resourceName}\";");
				else
					builder.AppendLine($"{indent}public const string {memberName} = \"{dir}/{resourceName}\";");
			}
			builder.Append(indent);
			return builder.ToString();
		}
	}
}