using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	public abstract class CodeGenerator : ScriptableObject
	{
		[CustomEditor(typeof(CodeGenerator), true), CanEditMultipleObjects]
		public class Editor : UnityEditor.Editor
		{
			// ReSharper disable once InconsistentNaming
			new IEnumerable<CodeGenerator> targets
			{
				get
				{
					foreach (var target in base.targets)
						yield return target as CodeGenerator;
				}
			}
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				if (GUILayout.Button("Generate"))
				{
					foreach (var target in targets)
					{
						target.Replace();
					}
				}
				if (GUILayout.Button("Clear"))
				{
					foreach (var target in targets)
					{
						target.Clear();
					}
				}
			}
		}

		static string Replace(string @this, string startMark, string endMark, string replace, int ident = 0)
		{
			var oldCodeLines = @this.Split("\n");
			var startIndex = -1;
			var endIndex = -1;
			for (var i = 0; i < oldCodeLines.Length; i++)
			{
				if (startIndex < 0)
				{
					var idx = oldCodeLines[i].IndexOf(startMark, StringComparison.Ordinal);
					if (idx > 0)
					{
						startIndex = i + 1;
					}
				}
				if (oldCodeLines[i].Contains(endMark))
				{
					endIndex = i;
					break;
				}
			}
			if (startIndex != -1 && endIndex != -1)
			{
				var newLines = new List<string>();
				var lines = replace.Split("\n");
				foreach (var t in lines)
				{
					var trimed = t.TrimEnd();
					if (trimed.IsNullOrEmpty()) continue;
					newLines.Add($"{new string('\t', ident)}{trimed}");
				}
				var newCodeLines = new string[oldCodeLines.Length - (endIndex - startIndex - 1) + newLines.Count - 1];
				Array.Copy(oldCodeLines, 0, newCodeLines, 0, startIndex);
				newLines.CopyTo(newCodeLines, startIndex);
				Array.Copy(oldCodeLines, endIndex, newCodeLines, startIndex + newLines.Count, oldCodeLines.Length - endIndex);
				return string.Join(Environment.NewLine, newCodeLines);
			}
			Debug.LogError("Start and/or end marks not found.");
			return @this;
		}
		[SerializeField] TextAsset script;
		[SerializeField] string regionName;
		protected void Replace()
		{
			var startMark = $"#region {regionName}";
			var endMark = $"#endregion {regionName}";
			var newCode = Replace(script.text, startMark, endMark, Generate(), 2);
			Debug.Log(newCode);
			System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(script), newCode);
			AssetDatabase.Refresh();
		}
		protected abstract string Generate();
		void Clear()
		{
			var startMark = $"#region {regionName}";
			var endMark = $"#endregion {regionName}";
			var newCode = Replace(script.text, startMark, endMark, "", 2);
			System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(script), newCode);
			AssetDatabase.Refresh();
		}
	}
}
