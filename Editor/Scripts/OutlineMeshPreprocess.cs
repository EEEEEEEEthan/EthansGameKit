using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	static class OutlineMeshPreprocess
	{
		[MenuItem("Assets/" + PackageDefines.packageName + "/Preprocess mesh(es) for outline")]
		static void Preprocess()
		{
			foreach (var obj in Selection.objects)
			{
				if (obj is Mesh mesh)
				{
					if (!AssetDatabase.GetAssetPath(obj).IsNullOrEmpty())
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
						EditorUtility.SetDirty(mesh);
					}
				}
			}
		}
		[MenuItem("Assets/" + PackageDefines.packageName + "/Preprocess mesh(es) for outline", true)]
		static bool ValidPreprocess()
		{
			foreach (var obj in Selection.objects)
			{
				if (obj is Mesh)
					if (!AssetDatabase.GetAssetPath(obj).IsNullOrEmpty())
						return true;
			}
			return false;
		}
	}
}
