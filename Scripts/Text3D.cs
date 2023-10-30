using UnityEditor;
using UnityEngine;

namespace EthansGameKit
{
	public class Text3D : MonoBehaviour
	{
#if UNITY_EDITOR
		[CustomEditor(typeof(Text3D))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var target = (Text3D)this.target;
				serializedObject.ApplyModifiedProperties();
				if (PrefabUtility.GetNearestPrefabInstanceRoot(target.gameObject) || PrefabUtility.GetPrefabAssetType(target.gameObject) != PrefabAssetType.NotAPrefab)
				{
					// 警告: prefab无法重建
					EditorGUILayout.HelpBox("Prefab无法重建3D网格", MessageType.Warning);
				}
				else if (GUILayout.Button("重建3D网格"))
				{
					Undo.RecordObject(target.gameObject, "重建3D网格");
					target.Editor_Rebuild();
					EditorUtility.SetDirty(target.gameObject);
				}
			}
		}
#endif
		[SerializeField] EditorAssetCache<Text3DAsset> asset;
		[SerializeField] GameObject prefab;
		[SerializeField] Material surfaceMaterial;
		[SerializeField] Material sideMaterial;
		[SerializeField] Material bevelMaterial;
		[SerializeField, TextArea] string text;
		void OnDrawGizmosSelected()
		{
			foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
			{
				Gizmos.matrix = meshRenderer.transform.localToWorldMatrix;
				var localBounds = meshRenderer.localBounds;
				Gizmos.DrawWireCube(localBounds.center, localBounds.size);
			}
		}
		void Editor_Rebuild()
		{
			if (!Application.isEditor)
			{
				Debug.LogError("Editor only!");
				return;
			}
#if UNITY_EDITOR
			for (var i = transform.childCount; i-- > 0;)
			{
				transform.GetChild(i).gameObject.Destroy();
			}
			var materials = new[] { surfaceMaterial, sideMaterial, bevelMaterial };
			Vector3 position = default;
			foreach (var c in text)
			{
				switch (c)
				{
					case ' ':
						position.x += asset.Value.BlankSpace;
						continue;
					case '\t':
						position.x += asset.Value.BlankSpace * 4;
						continue;
					case '\n':
						position.x = 0;
						position.y -= asset.Value.LineSpace;
						continue;
					case '\r':
						position.x = 0;
						continue;
					default:
					{
						var obj = prefab ? (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform) : new();
						obj.name = c.ToString();
						obj.transform.SetParent(transform);
						obj.transform.localPosition = position;
						obj.transform.localEulerAngles = default;
						obj.transform.localScale = Vector3.one;
						var meshFilter = obj.GetOrAddComponent<MeshFilter>();
						var meshRenderer = obj.GetOrAddComponent<MeshRenderer>();
						meshRenderer.sharedMaterials = materials;
						var mesh = asset.Value.Editor_GetMesh(c.ToString());
						if (!mesh) asset.Value.Editor_AddTexts(new[] { c.ToString() });
						mesh = asset.Value.Editor_GetMesh(c.ToString());
						meshFilter.sharedMesh = mesh;
						position.x += meshRenderer.localBounds.size.x + asset.Value.CharacterSpace;
						if (obj.TryGetComponent<MeshCollider>(out var meshCollider))
						{
							meshCollider.sharedMesh = mesh;
						}
						break;
					}
				}
			}
#endif
		}
	}
}
