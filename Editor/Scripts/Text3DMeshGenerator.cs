using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CreateAssetMenu(menuName = PackageDefines.packageName + "/Text3DMeshGenerator")]
	class Text3DMeshGenerator : ScriptableObject
	{
		[CustomEditor(typeof(Text3DMeshGenerator))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				var target = (Text3DMeshGenerator)this.target;
				base.OnInspectorGUI();
				if (GUILayout.Button("Generate"))
				{
					if (target.texts.IsNullOrEmpty())
					{
						return;
					}
					var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
					foreach (var asset in assets)
						if (asset is Mesh)
							AssetDatabase.RemoveObjectFromAsset(asset);
					var fontFilePath = AssetDatabase.GetAssetPath(target.font);
					var generator = new RuntimeText3DManager(fontFilePath, 0.1f, true);
					foreach (var text in target.texts)
					{
						var mesh = new Mesh();
						generator.BuildMesh(mesh, text);
						AssetDatabase.AddObjectToAsset(mesh, target);
					}
					AssetDatabase.SaveAssets();
				}
			}
		}

		[SerializeField] Font font;
		[SerializeField, TextArea] string[] texts;
	}
}
