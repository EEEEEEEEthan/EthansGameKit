using System.Collections.Generic;
using EthansGameKit.Attributes;
using UnityEngine;

namespace EthansGameKit.VisualEffects
{
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteAlways]
	public class Outline : MonoBehaviour, IRefreshableItem
	{
#if UNITY_EDITOR
		[UnityEditor.CustomEditor(typeof(Outline))]
		class Editor : UnityEditor.Editor
		{
			// ReSharper disable once InconsistentNaming
			new Outline target => (Outline)base.target;
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var meshFilter = target.GetComponent<MeshFilter>();
				var mesh = meshFilter.sharedMesh;
				if (mesh.uv4.Length <= 0)
				{
					// warning
					UnityEditor.EditorGUILayout.HelpBox("mesh不支持outline。将进行动态转换。如果有大规模outline，建议预处理", UnityEditor.MessageType.Warning);
					if (GUILayout.Button("将mesh预处理为支持outline的格式"))
					{
						PreprocessMesh(mesh);
						UnityEditor.EditorUtility.SetDirty(mesh);
						if (target.enabled)
						{
							target.enabled = false;
							// ReSharper disable once Unity.InefficientPropertyAccess
							target.enabled = true;
						}
					}
					if (GUILayout.Button("将mesh的拷贝处理为支持outline的格式"))
					{
						var copiedMesh = Instantiate(mesh);
						PreprocessMesh(copiedMesh);
						var path = UnityEditor.AssetDatabase.GetAssetPath(mesh);
						if (!path.IsNullOrEmpty() && path.StartsWith("Assets"))
						{
							for (var i = 1;; ++i)
							{
								var folder = System.IO.Path.GetDirectoryName(path);
								var name = $"{System.IO.Path.GetFileNameWithoutExtension(path)} {i}";
								var extension = System.IO.Path.GetExtension(path);
								path = $"{folder}/{name}{extension}";
								if (!System.IO.File.Exists(path))
								{
									mesh.name = name;
									UnityEditor.AssetDatabase.CreateAsset(copiedMesh, path);
									UnityEditor.AssetDatabase.SaveAssets();
									break;
								}
							}
						}
						else
						{
							for (var i = 0;; ++i)
							{
								var name = i == 0 ? mesh.name : $"{mesh.name} {i}";
								path = $"Assets/{name}.asset";
								if (!System.IO.File.Exists(path))
								{
									mesh.name = name;
									UnityEditor.AssetDatabase.CreateAsset(copiedMesh, path);
									UnityEditor.AssetDatabase.SaveAssets();
									break;
								}
							}
						}
						UnityEditor.EditorGUIUtility.PingObject(copiedMesh);
						meshFilter.sharedMesh = copiedMesh;
						if (target.enabled)
						{
							target.enabled = false;
							// ReSharper disable once Unity.InefficientPropertyAccess
							target.enabled = true;
						}
					}
				}
			}
		}
#endif
		static readonly int property_color = Shader.PropertyToID("_Color");
		static readonly int property_width = Shader.PropertyToID("_Width");
		static void PreprocessMesh(Mesh mesh)
		{
			var vertex2normals = new Dictionary<Vector3, List<Vector3>>();
			var rawVertices = mesh.vertices;
			var rawNormals = mesh.normals;
			for (var i = rawVertices.Length; i-- > 0;)
			{
				var vertex = rawVertices[i];
				if (!vertex2normals.TryGetValue(vertex, out var normals))
					vertex2normals[vertex] = normals = new();
				normals.Add(rawNormals[i]);
			}
			var vertex2normal = new Dictionary<Vector3, Vector3>();
			foreach (var (vertex, normals) in vertex2normals)
				vertex2normal[vertex] = normals.Sum().normalized;
			var uvs = new List<Vector3>();
			foreach (var vertex in rawVertices)
			{
				var normal = vertex2normal[vertex];
				var uv = new Vector3(normal.x, normal.y, normal.z);
				uvs.Add(uv);
			}
			mesh.SetUVs(3, uvs);
		}
		[SerializeField, HDColor] Color color;
		[SerializeField, Range(0, 1)] float width;
		[SerializeField, HideInInspector] GameObject outlineObject;
		MaterialPropertyBlock propertyBlock;
		public Color Color
		{
			get => color;
			set
			{
				color = value;
				if (enabled) this.Refresh();
			}
		}
		public float Width
		{
			get => width;
			set
			{
				width = value;
				if (enabled) this.Refresh();
			}
		}
		void OnValidate()
		{
			if (enabled) this.Refresh();
		}
		async void OnEnable()
		{
			outlineObject = new("Outline");
			var outlineTransform = outlineObject.transform;
			outlineTransform.SetParent(transform, false);
			var meshRenderer = outlineObject.AddComponent<MeshRenderer>();
			var meshFilter = outlineObject.AddComponent<MeshFilter>();
			var materials = new Material[GetComponent<MeshRenderer>().sharedMaterials.Length];
			var cache = ResourceTable.Resources.EthansGameKit.Materials.Outline_Material;
			if (!cache.HasValue) await cache.LoadAsync();
			materials.MemSet(cache.Value);
			if (!enabled) return;
			meshRenderer.sharedMaterials = materials;
			var mesh = GetComponent<MeshFilter>().sharedMesh;
			if (mesh.uv4.Length <= 0)
			{
				var copiedMesh = Instantiate(mesh);
				PreprocessMesh(copiedMesh);
				meshFilter.sharedMesh = copiedMesh;
			}
			else
			{
				meshFilter.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
			}
			meshFilter.transform.parent = transform;
			meshFilter.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
			this.Refresh(true);
		}
		void OnDisable()
		{
			if (outlineObject)
			{
				outlineObject.gameObject.Destroy();
				outlineObject = null;
			}
		}
		void IRefreshableItem.OnRefresh()
		{
			if (!enabled) return;
			if (!outlineObject) return;
			var renderer = outlineObject.GetComponent<MeshRenderer>();
			if (propertyBlock is null)
			{
				propertyBlock = new();
				renderer.GetPropertyBlock(propertyBlock);
			}
			propertyBlock.SetColor(property_color, color);
			propertyBlock.SetFloat(property_width, width);
			renderer.SetPropertyBlock(propertyBlock);
		}
	}
}
