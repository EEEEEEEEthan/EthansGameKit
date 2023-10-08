using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CustomPropertyDrawer(typeof(ResourceCache<>), true)]
	class ResourceCachePropertyDrawer : PropertyDrawer
	{
		const float ident = 0;
		readonly HashSet<string> shownDetails = new();
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
			var pathProperty = property.FindPropertyRelative("resourcePath");
			var value = AssetDatabase.LoadAssetAtPath<Object>(pathProperty.stringValue);
			var prefabObjectPosition = new Rect(position.x, position.y, position.width, position.height);
			var propertyInfo = typeof(ResourceCache<>).BaseType.GetProperty("Value", (BindingFlags)0xffff);
			var newValue = EditorGUI.ObjectField(prefabObjectPosition, label, value, propertyInfo.PropertyType, false);
			if (newValue != value)
			{
				var path = AssetDatabase.GetAssetPath(newValue);
				pathProperty.stringValue = path;
			}
			if (showProperties)
			{
				// Indent the properties
				EditorGUI.indentLevel++;
				position.x += ident;
				position.width -= ident;

				// Display the alias property field
				position.y += EditorGUIUtility.singleLineHeight;
				var aliasPropertyPosition = new Rect(position.x, position.y, position.width, position.height);
				EditorGUI.PropertyField(aliasPropertyPosition, pathProperty);

				// Remove the indent
				EditorGUI.indentLevel--;
				position.x -= ident;
				position.width += ident;

				// Apply modified properties
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
	}
}
