using System.Collections.Generic;
using System.IO;
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

		[SerializeField] TextAsset script;
		[SerializeField] string regionName;
		protected void Replace()
		{
			var startMark = $"#region {regionName}";
			var endMark = $"#endregion {regionName}";
			var newCode = script.text.ReplaceAsCode(startMark, endMark, Generate(), "\r\n", 2);
			Debug.Log(newCode);
			File.WriteAllText(AssetDatabase.GetAssetPath(script), newCode);
			AssetDatabase.Refresh();
		}
		protected abstract string Generate();
		void Clear()
		{
			var startMark = $"#region {regionName}";
			var endMark = $"#endregion {regionName}";
			var newCode = script.text.ReplaceAsCode(startMark, endMark, "", "\r\n", 2);
			File.WriteAllText(AssetDatabase.GetAssetPath(script), newCode);
			AssetDatabase.Refresh();
		}
	}
}
