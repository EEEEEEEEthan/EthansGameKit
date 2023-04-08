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
			}
		}

		public static void Replace(TextAsset script, string startMark, string endMark, string replace)
		{
#if UNITY_EDITOR
			var newCode = Replace(script.text, startMark, endMark, replace);
			System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(script), newCode);
			AssetDatabase.Refresh();
#endif
		}
		public static string Replace(string code, string startMark, string endMark, string replace)
		{
			var oldCodeLines = code.Split(Environment.NewLine);
			var startIndex = -1;
			var endIndex = -1;
			var ident = "";
			for (var i = 0; i < oldCodeLines.Length; i++)
			{
				if (startIndex < 0)
				{
					var idx = oldCodeLines[i].IndexOf(startMark, StringComparison.Ordinal);
					if (idx > 0)
					{
						startIndex = i + 1;
						ident = oldCodeLines[i][..idx];
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
				var lines = replace.Split(Environment.NewLine);
				foreach (var t in lines)
					newLines.Add($"{ident}{t}");
				var newCodeLines = new string[oldCodeLines.Length - (endIndex - startIndex - 1) + newLines.Count];
				Array.Copy(oldCodeLines, 0, newCodeLines, 0, startIndex);
				newLines.CopyTo(newCodeLines, startIndex);
				Array.Copy(oldCodeLines, endIndex, newCodeLines, startIndex + newLines.Count, oldCodeLines.Length - endIndex);
				return string.Join(Environment.NewLine, newCodeLines);
			}
			Debug.LogError("Start and/or end marks not found.");
			return code;
		}
		[SerializeField] TextAsset script;
		[SerializeField] string regionName;
		public void Replace()
		{
			var startMark = $"#region {regionName}";
			var endMark = $"#endregion {regionName}";
			CodeGeneratorUtility.Replace(script, startMark, endMark, Generate());
		}
		protected abstract string Generate();
	}
}
