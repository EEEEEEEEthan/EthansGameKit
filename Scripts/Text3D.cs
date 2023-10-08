using UnityEngine;

namespace EthansGameKit
{
	public class Text3D : MonoBehaviour
	{
#if UNITY_EDITOR
		[UnityEditor.CustomEditor(typeof(Text3D))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var target = (Text3D)this.target;
				UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(widthMeshCollider)));
				if (target.widthMeshCollider)
				{
					UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(widthRigidbody)));
					if (target.widthRigidbody)
					{
						UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(massPerCharacter)));
					}
				}
				serializedObject.ApplyModifiedProperties();
				if (GUILayout.Button("重建"))
				{
					target.Rebuild();
				}
			}
		}
#endif
		[SerializeField] Text3DAsset asset;
		[SerializeField] Material surfaceMaterial;
		[SerializeField] Material sideMaterial;
		[SerializeField] Material bevelMaterial;
		[SerializeField, TextArea] string text;
		[SerializeField, HideInInspector] bool widthMeshCollider;
		[SerializeField, HideInInspector] bool widthRigidbody;
		[SerializeField, HideInInspector] float massPerCharacter = 0.5f;
		void Rebuild()
		{
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
						position.x += asset.BlankSpace;
						continue;
					case '\t':
						position.x += asset.BlankSpace * 4;
						continue;
					case '\n':
						position.x = 0;
						position.y -= asset.LineSpace;
						continue;
					case '\r':
						position.x = 0;
						continue;
					default:
					{
						var obj = new GameObject(c.ToString());
						obj.transform.SetParent(transform);
						obj.transform.localPosition = position;
						obj.transform.localEulerAngles = default;
						obj.transform.localScale = Vector3.one;
						var meshFilter = obj.AddComponent<MeshFilter>();
						var meshRenderer = obj.AddComponent<MeshRenderer>();
						meshRenderer.sharedMaterials = materials;
						var mesh = asset.Editor_GetMesh(c.ToString());
						if (!mesh) asset.Editor_AddTexts(new[] { c.ToString() });
						mesh = asset.Editor_GetMesh(c.ToString());
						meshFilter.sharedMesh = mesh;
						position.x += mesh.bounds.size.x + asset.CharacterSpace;
						if (widthMeshCollider)
						{
							var meshCollider = obj.AddComponent<MeshCollider>();
							meshCollider.sharedMesh = mesh;
							if (widthRigidbody)
							{
								meshCollider.isTrigger = false;
								meshCollider.convex = true;
							}
						}
						if (widthRigidbody)
						{
							var rigidBody = obj.AddComponent<Rigidbody>();
							rigidBody.mass = massPerCharacter;
						}
						break;
					}
				}
			}
		}
	}
}
