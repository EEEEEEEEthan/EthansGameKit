using EthansGameKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
	internal sealed class InspectorReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginDisabledGroup(true);
			if (property.isArray)
			{
				// 绘制数组或列表的标签
				EditorGUI.LabelField(position, label);
				EditorGUI.indentLevel++;
				EditorGUI.BeginChangeCheck();
				var newSize = EditorGUI.DelayedIntField(
					new(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
					"Size",
					property.arraySize,
					EditorStyles.numberField
				);
				if (EditorGUI.EndChangeCheck())
				{
					property.arraySize = newSize;
				}
				EditorGUI.indentLevel--;
				for (var i = 0; i < property.arraySize; i++)
				{
					EditorGUI.PropertyField(
						new(position.x, position.y + (i + 2) * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
						property.GetArrayElementAtIndex(i),
						new($"Element {i}"),
						true
					);
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}