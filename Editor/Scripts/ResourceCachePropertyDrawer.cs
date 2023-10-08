using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EthansGameKit.Editor
{
	abstract class CachePropertyDrawer : PropertyDrawer
	{
		const float ident = 0;
		readonly HashSet<string> shownDetails = new();
		protected abstract string PathFieldName { get; }
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Draw the foldout arrow
			var foldoutPosition = new Rect(position.x, position.y, EditorGUIUtility.singleLineHeight,
				EditorGUIUtility.singleLineHeight);
			var showProperties = shownDetails.Contains(property.propertyPath);
			showProperties = EditorGUI.Foldout(foldoutPosition, showProperties, GUIContent.none);
			if (!showProperties)
				shownDetails.Remove(property.propertyPath);
			else
				shownDetails.Add(property.propertyPath);
			position.height = EditorGUIUtility.singleLineHeight;
			var pathProperty = property.FindPropertyRelative(PathFieldName);
			var prefabObjectPosition = new Rect(position.x, position.y, position.width, position.height);
			var valueProperty = GetValueProperty(property);
			var value = LoadValue(pathProperty.stringValue, valueProperty.PropertyType);
			var newValue = EditorGUI.ObjectField(prefabObjectPosition, label, value, valueProperty.PropertyType, false);
			if (newValue != value)
			{
				var path = GetFilePath(newValue);
				pathProperty.stringValue = path;
			}
			if (showProperties)
			{
				EditorGUI.indentLevel++;
				position.x += ident;
				position.width -= ident;
				position.y += EditorGUIUtility.singleLineHeight;
				var aliasPropertyPosition = new Rect(position.x, position.y, position.width, position.height);
				EditorGUI.PropertyField(aliasPropertyPosition, pathProperty);
				EditorGUI.indentLevel--;
				position.x -= ident;
				position.width += ident;
				property.serializedObject.ApplyModifiedProperties();
			}
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var height = EditorGUIUtility.singleLineHeight;
			if (shownDetails.Contains(property.propertyPath))
			{
				height += EditorGUIUtility.singleLineHeight * 1.1f;
			}
			return height;
		}
		protected abstract string GetFilePath(Object value);
		protected abstract Object LoadValue(string path, Type type);
		PropertyInfo GetValueProperty(SerializedProperty property)
		{
			var propertyType = property.GetField().FieldType;
			return propertyType.GetProperty("Value", (BindingFlags)0xffff, true);
		}
	}

	[CustomPropertyDrawer(typeof(EditorAssetCache<>), true)]
	class AssetCachePropertyDrawer : CachePropertyDrawer
	{
		protected override string PathFieldName => "assetPath";
		protected override string GetFilePath(Object value)
		{
			return AssetDatabase.GetAssetPath(value);
		}
		protected override Object LoadValue(string path, Type type)
		{
			return AssetDatabase.LoadAssetAtPath(path, type);
		}
	}

	[CustomPropertyDrawer(typeof(ResourceCache<>), true)]
	class ResourceCachePropertyDrawer : CachePropertyDrawer
	{
		protected override string PathFieldName => "resourcePath";
		protected override string GetFilePath(Object value)
		{
			var path = AssetDatabase.GetAssetPath(value);
			// 在resources目录下的路径
			return path[(path.IndexOf("Resources", StringComparison.Ordinal) + 10)..];
		}
		protected override Object LoadValue(string path, Type type)
		{
			return Resources.Load(path, type);
		}
	}
}
