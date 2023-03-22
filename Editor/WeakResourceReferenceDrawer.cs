using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CustomPropertyDrawer(typeof(WeakResourceReference))]
	public class WeakResourceReferenceDrawer : PropertyDrawer
	{
		bool expanded = true;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var pathProperty = property.FindPropertyRelative(nameof(WeakResourceReference.path));
			position.height = EditorGUIUtility.singleLineHeight;
			var foldoutLabel = $"{property.displayName}({nameof(WeakResourceReference)}) {pathProperty.stringValue}";
			expanded = EditorGUI.Foldout(position, expanded, foldoutLabel, true);
			if (expanded)
			{
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				var currentObject = AssetDatabase.LoadAssetAtPath<Object>(pathProperty.stringValue);
				EditorGUI.BeginChangeCheck();
				var selectedObject = EditorGUI.ObjectField(position, GUIContent.none, currentObject, typeof(Object), false);
				if (EditorGUI.EndChangeCheck())
					pathProperty.stringValue = AssetDatabase.GetAssetPath(selectedObject);
			}
			EditorGUI.EndProperty();
		}
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return expanded ? EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing : EditorGUIUtility.singleLineHeight;
		}
	}
}
