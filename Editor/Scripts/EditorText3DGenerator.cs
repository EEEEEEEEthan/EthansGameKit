using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CreateAssetMenu(menuName = PackageDefines.packageName + "/Text3DMeshGenerator")]
	class EditorText3DGenerator : ScriptableObject
	{
		[CustomEditor(typeof(EditorText3DGenerator))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				var target = (EditorText3DGenerator)this.target;
				base.OnInspectorGUI();
				if (GUILayout.Button("以字母顺序排序"))
				{
					Undo.RecordObject(target, "以字母顺序排序");
					Array.Sort(target.texts);
					// 脏标记,撤销
					EditorUtility.SetDirty(target);
				}
				if (!target.font)
				{
					// 警示符号,提示没有选择字体
					EditorGUILayout.HelpBox("没有选择字体", MessageType.Error);
					return;
				}
				var fontFilePath = AssetDatabase.GetAssetPath(target.font);
				if (fontFilePath.IsNullOrEmpty() || fontFilePath == "Library/unity default resources")
				{
					// 警示符号,提示没有选择字体
					EditorGUILayout.HelpBox("无法获取字体路径", MessageType.Error);
					return;
				}
				if (GUILayout.Button("生成模型"))
				{
					if (target.texts.IsNullOrEmpty())
					{
						return;
					}
					var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
					var old = new Dictionary<string, Mesh>();
					foreach (var asset in assets)
						if (asset is Mesh mesh)
							old[mesh.name] = mesh;
					var generator = new RuntimeText3DManager(fontFilePath, target.backFace, target.depth, target.bevel);
					var tobeAdded = new SortedDictionary<string, Mesh>();
					foreach (var text in target.texts)
					{
						if (!old.TryGetValue(text, out var mesh)) mesh = new();
						tobeAdded[text] = mesh;
						generator.BuildMesh(mesh, text, target.blankSpace, target.characterSpace, target.lineSpace);
						mesh.name = text;
					}
					foreach (var (name, mesh) in old)
					{
						if (!tobeAdded.ContainsKey(name))
							AssetDatabase.RemoveObjectFromAsset(mesh);
					}
					foreach (var (name, mesh) in tobeAdded)
					{
						if (!old.ContainsKey(name))
							AssetDatabase.AddObjectToAsset(mesh, target);
					}
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
		}

		[SerializeField, Tooltip("空格宽度")] float blankSpace = 0.3f;
		[SerializeField, Tooltip("字间距")] float characterSpace = 0.1f;
		[SerializeField, Tooltip("行间距")] float lineSpace = 1;
		[SerializeField, Tooltip("背面可见")] bool backFace = true;
		[SerializeField, Tooltip("厚度")] float depth = 0.1f;
		[SerializeField, Tooltip("模型斜边宽度")] float bevel;
		[SerializeField, Tooltip("字体")] Font font;
		[SerializeField, TextArea] string[] texts;
	}
}
