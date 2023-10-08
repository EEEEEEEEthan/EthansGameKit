using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EthansGameKit
{
	[CreateAssetMenu(menuName = PackageDefines.packageName + "/Text3DMeshGenerator")]
	public class Text3DAsset : ScriptableObject
	{
#if UNITY_EDITOR
		[UnityEditor.CustomEditor(typeof(Text3DAsset))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				var target = (Text3DAsset)this.target;
				base.OnInspectorGUI();
				if (GUILayout.Button("以字母顺序排序"))
				{
					UnityEditor.Undo.RecordObject(target, "以字母顺序排序");
					Array.Sort(target.texts);
					// 脏标记,撤销
					UnityEditor.EditorUtility.SetDirty(target);
				}
				if (!target.font)
				{
					// 警示符号,提示没有选择字体
					UnityEditor.EditorGUILayout.HelpBox("没有选择字体", UnityEditor.MessageType.Error);
					return;
				}
				var fontFilePath = UnityEditor.AssetDatabase.GetAssetPath(target.font);
				if (fontFilePath.IsNullOrEmpty() || fontFilePath == "Library/unity default resources")
				{
					// 警示符号,提示没有选择字体
					UnityEditor.EditorGUILayout.HelpBox("无法获取字体路径", UnityEditor.MessageType.Error);
					return;
				}
				if (GUILayout.Button("生成模型"))
				{
					target.Editor_Rebuild();
				}
			}
		}
#endif
		[SerializeField, Tooltip("空格宽度")] float blankSpace = 0.3f;
		[SerializeField, Tooltip("字间距")] float characterSpace = 0.1f;
		[SerializeField, Tooltip("行间距")] float lineSpace = 1;
		[SerializeField, Tooltip("背面可见")] bool backFace = true;
		[SerializeField, Tooltip("厚度")] float depth = 0.1f;
		[SerializeField, Tooltip("模型斜边宽度")] float bevel;
		[SerializeField, Tooltip("字体")] Font font;
		[SerializeField, TextArea] string[] texts;
		public float BlankSpace => blankSpace;
		public float CharacterSpace => characterSpace;
		public float LineSpace => lineSpace;
		public Mesh Editor_GetMesh(string key)
		{
#if UNITY_EDITOR
			var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
			foreach (var asset in assets)
				if (asset is Mesh mesh && mesh.name == key)
					return mesh;
#endif
			return null;
		}
		public void Editor_AddTexts(IEnumerable<string> texts)
		{
#if UNITY_EDITOR
			var dict = new SortedDictionary<string, string>();
			foreach (var line in this.texts)
			{
				if (line.IsNullOrEmpty()) continue;
				dict[line] = line;
			}
			foreach (var line in texts)
			{
				if (line.IsNullOrEmpty()) continue;
				dict[line] = line;
			}
			this.texts = dict.Keys.ToArray();
			Editor_Rebuild();
#endif
		}
		void Editor_Rebuild()
		{
#if UNITY_EDITOR
			if (texts.IsNullOrEmpty())
			{
				return;
			}
			var fontFilePath = UnityEditor.AssetDatabase.GetAssetPath(font);
			var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
			var old = new Dictionary<string, Mesh>();
			foreach (var asset in assets)
				if (asset is Mesh mesh)
					old[mesh.name] = mesh;
			var generator = new RuntimeText3DManager(fontFilePath, backFace, depth, bevel);
			var tobeAdded = new SortedDictionary<string, Mesh>();
			foreach (var text in texts)
			{
				if (!old.TryGetValue(text, out var mesh)) mesh = new();
				tobeAdded[text] = mesh;
				generator.BuildMesh(mesh, text, blankSpace, characterSpace, lineSpace);
				mesh.name = text;
			}
			foreach (var (name, mesh) in old)
			{
				if (!tobeAdded.ContainsKey(name))
					UnityEditor.AssetDatabase.RemoveObjectFromAsset(mesh);
			}
			foreach (var (name, mesh) in tobeAdded)
			{
				if (!old.ContainsKey(name))
					UnityEditor.AssetDatabase.AddObjectToAsset(mesh, this);
			}
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
#endif
		}
	}
}
