using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	static class Sprite2Mesh
	{
		[MenuItem("Assets/EthansGameKit/Sprite to Mesh", true)]
		static bool ValidateToMesh()
		{
			if (Selection.objects.Length != 1) return false;
			return Selection.activeObject is Sprite;
		}
		[MenuItem("Assets/EthansGameKit/Sprite to Mesh")]
		static void ToMesh()
		{
			var sprite = Selection.activeObject as Sprite;
			if (sprite == null)
			{
				EditorUtility.DisplayDialog("Error", "Please select a sprite first!", "OK");
				return;
			}
			var mesh = new Mesh();
			var shapeCount = sprite.GetPhysicsShapeCount();
			for (var i = 0; i < shapeCount; ++i)
			{
				var list = new List<Vector2>();
				sprite.GetPhysicsShape(i, list);
				var vertices = new Vector3[list.Count];
				for (var j = 0; j < list.Count; ++j)
					vertices[j] = list[j];
			}
			mesh.RecalculateBounds();
			AssetDatabase.CreateAsset(mesh, "Assets/" + sprite.name + ".asset");
			EditorGUIUtility.PingObject(mesh);
		}
	}
}
