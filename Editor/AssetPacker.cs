using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EthansGameKit.Editor
{
	class AssetPacker : EditorWindow
	{
		[MenuItem("Tools/Ethan's Game Kit/Asset Packer")]
		static void ShowWindow()
		{
			var window = GetWindow<AssetPacker>();
			window.titleContent = new("Asset Packer");
			window.Show();
		}
		[SerializeField] Object asset;
		[SerializeField] Object[] objectsToAdd = Array.Empty<Object>();
		SerializedObject serializedObject;
		SerializedProperty property_asset;
		SerializedProperty property_objectsToAdd;
		void OnEnable()
		{
			serializedObject = new(this);
			property_asset = serializedObject.FindProperty(nameof(asset));
			property_objectsToAdd = serializedObject.FindProperty(nameof(objectsToAdd));
		}
		void OnGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property_asset);
			EditorGUILayout.PropertyField(property_objectsToAdd);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
			if (GUILayout.Button("Copy & Add"))
			{
				serializedObject.Update();
				if (!asset)
				{
					asset = property_asset.objectReferenceValue = CreateInstance<AssetPack>();
					AssetDatabase.CreateAsset(asset, "Assets/AssetPack.asset");
					EditorGUIUtility.PingObject(asset);
				}
				foreach (var obj in objectsToAdd)
				{
					var toAdd = obj;
					if (toAdd is GameObject)
					{
						Debug.LogError("GameObject对象无法添加", toAdd);
						continue;
					}
					var path = AssetDatabase.GetAssetPath(toAdd);
					if (!path.IsNullOrEmpty())
					{
						toAdd = Instantiate(obj);
						toAdd.name = obj.name;
					}
					AssetDatabase.AddObjectToAsset(toAdd, asset);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					Undo.RecordObject(asset, "Add Objects");
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
